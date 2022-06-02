#!/bin/sh
set -e

if  [ -n "$ASPNETCORE_ENVIRONMENT" ] && [ "$ASPNETCORE_ENVIRONMENT" != "Development" ]; then envtpl -in ./appsettings.json.tmpl > ./appsettings.$ASPNETCORE_ENVIRONMENT.json; fi

# Istio proxy does not become available as soon as the main container starts.
# The workaround below ensures the sidecar is initialized and up before we
# start our container.
if [ "$CS_ISTIO_SIDECAR_ENABLED" == "true" ]
then
   until curl -sS --head http://localhost:15000 > /dev/null
   do
      echo "Waiting for Istio sidecar proxy to be up"
      sleep 0.5
   done

   echo "Istio proxy is up, execing the binary"
fi

# Overwrite shell process with the dotnet process using exec system call.
# exec call replaces shell process with the dotnet process so that it runs as PID 1.
# It is due to the fact that container engine sends the signals (SIGTERM) to PID 1.
# This allows dotnet process to carry graceful shutdown.
exec dotnet EvenTransit.UI.dll $@
