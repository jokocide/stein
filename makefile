PROJECT_NAME ?= Stein

.PHONY: build install uninstall reinstall build-install build-reinstall

pack:
	nuget locals all -clear && dotnet pack ./src

install:
	dotnet tool install -g --add-source ./Package Stein

uninstall:
	dotnet tool uninstall Stein -g

reinstall:
	make uninstall && make install

build:
	dotnet run --project ./src/Stein.Console build ../jokoci.de

serve:
	dotnet run --project ./src/Stein.Console serve ../jokoci.de
