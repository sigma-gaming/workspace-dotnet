.PHONY: dev test build

dev:
	@echo "Starting development servers..."
	dotnet watch run --project src/Apis/Games

test:
	dotnet test

build:
	dotnet build