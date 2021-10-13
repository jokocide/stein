PROJECT_NAME ?= Stein

.PHONY: build install uninstall reinstall build-install build-reinstall

pack:
	nuget locals all -clear && dotnet pack ./src

install:
	dotnet tool install -g --add-source ./Package stein

uninstall:
	dotnet tool uninstall stein -g

reinstall:
	make uninstall && make install

build:
	dotnet run --project ./src/stein build ../jokoci.de

serve:
	dotnet run --project ./src/stein serve ../jokoci.de