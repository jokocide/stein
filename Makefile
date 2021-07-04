PROJECT_NAME ?= Dagger

.PHONY: build install uninstall reinstall build-install build-reinstall

build:
	nuget locals all -clear && dotnet pack

install:
	dotnet tool install -g --add-source ./Build Jokocide.Dagger

uninstall:
	dotnet tool uninstall jokocide.dagger -g

reinstall:
	make uninstall && make install

build-install:
	make build && make install

build-reinstall:
	make build && make reinstall