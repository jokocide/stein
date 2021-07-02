PROJECT_NAME ?= Dagger

.PHONY: build-tool install reinstall

build-tool:
	dotnet pack

install:
	dotnet tool install -g --add-source .\nupkg Dagger.App

reinstall:
	dotnet tool uninstall -g Dagger.App && make install