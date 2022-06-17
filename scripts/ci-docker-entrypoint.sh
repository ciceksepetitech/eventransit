#!/bin/sh
set -e

# if  [ -n "$ASPNETCORE_ENVIRONMENT" ] && [ "$ASPNETCORE_ENVIRONMENT" != "Development" ]; then envtpl -in ./appsettings.json.tmpl > ./appsettings.$ASPNETCORE_ENVIRONMENT.json; fi

# Overwrite shell process with the dotnet process using exec system call.
# exec call replaces shell process with the dotnet process so that it runs as PID 1.
# It is due to the fact that container engine sends the signals (SIGTERM) to PID 1.
# This allows dotnet process to carry graceful shutdown.
exec dotnet EvenTransit.UI.dll $@
