PROJECT_NAME ?= Stein

.PHONY: pack clear-pack install uninstall reinstall 

pack:
	dotnet pack ./src

clear-pack:
	nuget locals all -clear && dotnet pack ./src

install:
	dotnet tool install -g --add-source ./package Stein

uninstall:
	dotnet tool uninstall Stein -g

reinstall:
	make uninstall && make install
