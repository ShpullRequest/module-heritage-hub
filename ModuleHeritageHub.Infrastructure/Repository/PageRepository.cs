using Microsoft.EntityFrameworkCore;
using ModuleHeritageHub.Domain.DTO;
using ModuleHeritageHub.Domain.Exceptions;
using ModuleHeritageHub.Domain.Model;
using ModuleHeritageHub.Infrastructure.DB;

namespace ModuleHeritageHub.Infrastructure.Repository
{
    public class PageRepository(DBContext context)
    {
        private readonly DBContext _context = context;

        public async Task<PageDTO> Create(PageCreateDTO data, Guid userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var page = new Page
                {
                    Title = data.Title,
                    Description = data.Description,
                    InfoList = data.InfoList,
                };

                await _context.Pages.AddAsync(page);
                await _context.SaveChangesAsync();

                var pageFirstVersion = new PageVersion
                {
                    PageId = page.Id,
                    OwnerId = userId,
                    Body = data.Body,
                    CreatedAt = DateTime.UtcNow,
                    EditedAt = DateTime.UtcNow
                };

                await _context.PageVersions.AddAsync(pageFirstVersion);
                await _context.SaveChangesAsync();

                var pageVersionImages = data.Images.Select(imageId => new PageVersionImage
                {
                    VersionId = pageFirstVersion.Id,
                    ImageId = imageId
                }).ToList();

                if (pageVersionImages.Any())
                {
                    await _context.PageVersionImages.AddRangeAsync(pageVersionImages);
                }

                page.CurrentVersionId = pageFirstVersion.Id;
                
                _context.Pages.Update(page);
                await _context.SaveChangesAsync();

                var loadedPage = await _context.Pages
                    .Include(p => p.CurrentVersion)
                        .ThenInclude(cv => cv.Images)
                            .ThenInclude(cvi => cvi.Image)
                    .FirstOrDefaultAsync(p => p.Id == page.Id)
                    ?? throw new InvalidOperationException("Failed fetch created page");

                var pageDTO = new PageDTO
                {
                    Id = loadedPage.Id,
                    Title = loadedPage.Title,
                    Description = loadedPage.Description,
                    InfoList = loadedPage.InfoList,
                    Body = loadedPage.CurrentVersion.Body,
                    Images = loadedPage.CurrentVersion.Images.ToDictionary(pvi => pvi.ImageId, pvi => pvi.Image.Path)
                };
                 

                await transaction.CommitAsync();

                return pageDTO;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<PageDTO> GetById(Guid pageId)
        {
            var page = await _context.Pages
                .Include(p => p.CurrentVersion)
                    .ThenInclude(cv => cv.Images)
                        .ThenInclude(cvi => cvi.Image)
                .FirstOrDefaultAsync(p => p.Id == pageId)
                ?? throw new NotFoundException("Page not found");
            
            return new PageDTO
            {
                Id = page.Id,
                Title = page.Title,
                Description = page.Description,
                InfoList = page.InfoList,
                Body = page.CurrentVersion.Body,
                Images = page.CurrentVersion.Images.ToDictionary(pvi => pvi.ImageId, pvi => pvi.Image.Path)
            };
        }

        public async Task<PageDTO[]> GetList()
        {
            var pages = await _context.Pages
                .Include(p => p.CurrentVersion)
                    .ThenInclude(cv => cv.Images)
                        .ThenInclude(cvi => cvi.Image)
                .ToArrayAsync();

            return pages.Select(p => new PageDTO
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                InfoList = p.InfoList,
                Body = p.CurrentVersion.Body,
                Images = p.CurrentVersion.Images.ToDictionary(pvi => pvi.ImageId, pvi => pvi.Image.Path)
            }).ToArray();
        }

        public async Task<PageVersionDTO[]> GetVersions(Guid pageId)
        {
            var page = await _context.Pages
                .Select(p => new {
                    p.Id,
                    p.CurrentVersionId,
                })
                .FirstOrDefaultAsync(p => p.Id == pageId)
                ?? throw new NotFoundException("Page not found");
            

            var pageVersions = await _context.PageVersions
                .Where(pv => pv.PageId == pageId)
                .Include(pv => pv.Owner)
                    .ThenInclude(o => o.Image)
                .Include(pv => pv.Images)
                    .ThenInclude(pvi => pvi.Image)
                .ToArrayAsync();
            
            return pageVersions.Select(pv => new PageVersionDTO
            {
                VersionId = pv.Id,
                PageId = pv.PageId,
                Owner = new UserDTO
                {
                    Id = pv.Owner.Id,
                    Login = pv.Owner.Login,
                    FirstName = pv.Owner.FirstName,
                    LastName = pv.Owner.LastName,
                    Role = pv.Owner.Role.ToString(),
                    ImageUrl = pv.Owner.Image?.Path,
                    CreatedAt = pv.Owner.CreatedAt,
                },
                Body = pv.Body,
                EditedAt = pv.EditedAt,
                CreatedAt = pv.CreatedAt,
                Images = pv.Images.ToDictionary(pvi => pvi.ImageId, pvi => pvi.Image.Path),
                CurrentUse = pv.Id == page.CurrentVersionId,
            }).ToArray();
        }

        public async Task<PageDTO> Update(Guid pageId, PageEditDTO data)
        {
            var page = await _context.Pages
                .Include(p => p.CurrentVersion)
                    .ThenInclude(cv => cv.Images)
                        .ThenInclude(cvi => cvi.Image)
                .FirstOrDefaultAsync(p => p.Id == pageId)
                ?? throw new NotFoundException("Page not found");
            
            page.Title = data.Title;
            page.Description = data.Description;
            page.InfoList = data.InfoList;

            var updatedVersion = false;
            if (data.CurrentVersionId != null && page.CurrentVersionId != data.CurrentVersionId)
            {
                var pageVersionExists = await _context.PageVersions.AnyAsync(pv => pv.PageId == page.Id && pv.Id == data.CurrentVersionId);
                if (!pageVersionExists)
                {
                    throw new InvalidOperationException("Page version not for this page.");
                }

                page.CurrentVersionId = data.CurrentVersionId;
                updatedVersion = true;
            }

            _context.Pages.Update(page);
            await _context.SaveChangesAsync();

            if (updatedVersion) {
                page = await _context.Pages
                    .Include(p => p.CurrentVersion)
                        .ThenInclude(cv => cv.Images)
                            .ThenInclude(cvi => cvi.Image)
                    .FirstOrDefaultAsync(p => p.Id == pageId)
                    ?? throw new InvalidOperationException("Failed fetch updated data");
            }

            return new PageDTO
            {
                Id = page.Id,
                Title = page.Title,
                Description = page.Description,
                InfoList = page.InfoList,
                Body = page.CurrentVersion.Body,
                Images = page.CurrentVersion.Images.ToDictionary(pvi => pvi.ImageId, pvi => pvi.Image.Path)
            };
        }

        public async Task Delete(Guid pageId)
        {
            var page = await _context.Pages
                .FirstOrDefaultAsync(p => p.Id == pageId) 
                ?? throw new NotFoundException("Page not found");

            _context.Pages.Remove(page);
            await _context.SaveChangesAsync();
        }

        public async Task<PageDTO> CreateVersion(Guid pageId, PageVersionCreateEditDTO data, Guid userId)
        {
            var page = await _context.Pages
                .FirstOrDefaultAsync(p => p.Id == pageId)
                ?? throw new NotFoundException("Page not found");
            
            using var transaction = await _context.Database.BeginTransactionAsync();

            try 
            {
                var pageVersion = new PageVersion
                {
                    PageId = page.Id,
                    OwnerId = userId,
                    Body = data.Body,
                    CreatedAt = DateTime.UtcNow,
                    EditedAt = DateTime.UtcNow
                };

                await _context.PageVersions.AddAsync(pageVersion);
                await _context.SaveChangesAsync();

                var pageVersionImages = data.Images.Select(imageId => new PageVersionImage
                {
                    VersionId = pageVersion.Id,
                    ImageId = imageId
                }).ToList();

                if (pageVersionImages.Any())
                {
                    await _context.PageVersionImages.AddRangeAsync(pageVersionImages);
                }

                page.CurrentVersionId = pageVersion.Id;
                
                _context.Pages.Update(page);
                await _context.SaveChangesAsync();

                var updatedPage = await _context.Pages
                    .Include(p => p.CurrentVersion)
                        .ThenInclude(cv => cv.Images)
                            .ThenInclude(cvi => cvi.Image)
                    .FirstOrDefaultAsync(p => p.Id == pageId)
                    ?? throw new InvalidOperationException("Updated page not found");

                var pageDTO = new PageDTO
                {
                    Id = updatedPage.Id,
                    Title = updatedPage.Title,
                    Description = updatedPage.Description,
                    InfoList = updatedPage.InfoList,
                    Body = updatedPage.CurrentVersion.Body,
                    Images = updatedPage.CurrentVersion.Images.ToDictionary(pvi => pvi.ImageId, pvi => pvi.Image.Path)
                };

                await transaction.CommitAsync();
                return pageDTO;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<PageVersionDTO> UpdateVersion(Guid pageId, Guid versionId, PageVersionCreateEditDTO data, Guid userId)
        {
            var page = await _context.Pages
                .Select(p => new
                {
                    p.Id,
                    p.CurrentVersionId,
                })
                .FirstOrDefaultAsync(p => p.Id == pageId)
                ?? throw new InvalidOperationException("Page not found");

            var pageVersion = await _context.PageVersions
                .Include(pv => pv.Owner)
                    .ThenInclude(o => o.Image)
                .Include(pv => pv.Images)
                    .ThenInclude(pvi => pvi.Image)
                .FirstOrDefaultAsync(pv => pv.PageId == pageId && pv.Id == versionId)
                ?? throw new NotFoundException("Page version not found");
            
            pageVersion.OwnerId = userId;
            pageVersion.Body = data.Body;
            pageVersion.EditedAt = DateTime.UtcNow;

            var pageImageIds = pageVersion.Images.Select(pvi => pvi.ImageId).ToList();

            var imagesToRemove = pageVersion.Images
                .Where(pvi => !data.Images.Contains(pvi.ImageId))
                .ToList();
            
            foreach (var imageToRemove in imagesToRemove)
            {
                _context.PageVersionImages.Remove(imageToRemove);
            }

            var newImages = data.Images.Where(imageId => !pageImageIds.Contains(imageId)).ToList();
            foreach (var imageId in newImages)
            {
                var imageExists = await _context.Images.FindAsync(imageId);
                if (imageExists != null)
                {
                    pageVersion.Images.Add(new PageVersionImage
                    {
                        VersionId = pageVersion.Id,
                        ImageId = imageId
                    });
                }
                else
                {
                    throw new InvalidOperationException($"Image with ID {imageId} not found");
                }
            }

            await _context.SaveChangesAsync();

            return new PageVersionDTO
            {
                VersionId = pageVersion.Id,
                PageId = pageVersion.PageId,
                Owner = new UserDTO
                {
                    Id = pageVersion.Owner.Id,
                    Login = pageVersion.Owner.Login,
                    FirstName = pageVersion.Owner.FirstName,
                    LastName = pageVersion.Owner.LastName,
                    Role = pageVersion.Owner.Role.ToString(),
                    ImageUrl = pageVersion.Owner.Image?.Path,
                    CreatedAt = pageVersion.Owner.CreatedAt,
                },
                Body = pageVersion.Body,
                EditedAt = pageVersion.EditedAt,
                CreatedAt = pageVersion.CreatedAt,
                Images = pageVersion.Images.ToDictionary(pvi => pvi.ImageId, pvi => pvi.Image.Path),
                CurrentUse = pageVersion.Id == page.CurrentVersionId,
            };
        }

        public async Task<PageDTO> DeleteVersion(Guid pageId, Guid versionId) {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try 
            {
                var page = await _context.Pages
                    .FirstOrDefaultAsync(p => p.Id == pageId)
                    ?? throw new NotFoundException("Page not found");

                var pageVersion = await _context.PageVersions
                    .FirstOrDefaultAsync(pv => pv.PageId == pageId && pv.Id == versionId)
                    ?? throw new NotFoundException("Page version not found");

                if (page.CurrentVersionId == pageVersion.Id)
                {
                    var altPageVersion = await _context.PageVersions
                        .FirstOrDefaultAsync(pv => pv.PageId == pageId && pv.Id != versionId)
                        ?? throw new InvalidOperationException("The page has no alternative versions.");

                    page.CurrentVersion = altPageVersion;
                    page.CurrentVersionId = altPageVersion.Id;
                    _context.Pages.Update(page);
                }

                _context.PageVersions.Remove(pageVersion);
                await _context.SaveChangesAsync();

                var updatedPage = await _context.Pages
                    .Include(p => p.CurrentVersion)
                        .ThenInclude(cv => cv.Images)
                            .ThenInclude(cvi => cvi.Image)
                    .FirstOrDefaultAsync(p => p.Id == pageId)
                    ?? throw new InvalidOperationException("Updated page not found");

                var pageDTO = new PageDTO
                {
                    Id = updatedPage.Id,
                    Title = updatedPage.Title,
                    Description = updatedPage.Description,
                    InfoList = updatedPage.InfoList,
                    Body = updatedPage.CurrentVersion.Body,
                    Images = updatedPage.CurrentVersion.Images.ToDictionary(pvi => pvi.ImageId, pvi => pvi.Image.Path)
                };

                await transaction.CommitAsync();
                return pageDTO;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}