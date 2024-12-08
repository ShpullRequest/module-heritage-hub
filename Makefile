.PHONY: help add-migrations

help:
	@echo "Список команд:"
	@echo "	make add-migrations {name} - создать миграцию"
	@echo "	make apply-migrations - применить миграции"

add-migrations:
ifeq ($(strip $(name)),)
	@echo "Ошибка: необходимо указать имя миграции. Пример: make add-migrations name=NewMigrationName"
	@exit 1
else
	dotnet ef migrations add $(name) --project ModuleHeritageHub.Infrastructure --startup-project ModuleHeritageHub.API
endif

apply-migrations:
	dotnet ef database update --project ModuleHeritageHub.Infrastructure --startup-project ModuleHeritageHub.API
