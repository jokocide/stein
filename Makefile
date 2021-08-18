PROJECT_NAME ?= Stein

.PHONY: build install uninstall reinstall build-install build-reinstall

build:
	nuget locals all -clear && dotnet pack

install:
	dotnet tool install -g --add-source ./Package stein

uninstall:
	dotnet tool uninstall stein -g

reinstall:
	make uninstall && make install

build-install:
	make build && make install

build-reinstall:
	make build && make reinstall