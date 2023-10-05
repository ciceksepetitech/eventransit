FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env

COPY /scripts /scripts
COPY /Directory.Build.targets .
COPY /EvenTransit.sln .
COPY /test /test
COPY /src /src
COPY /.editorconfig /.editorconfig

RUN dotnet build -c Release
RUN dotnet test -c Release --no-restore --no-build

WORKDIR /src/EvenTransit.UI

RUN ["dotnet", "publish", "--configuration", "Release", "--no-build", "--no-restore", "-v", "m", "-o", "../../artifacts"]

FROM mcr.microsoft.com/dotnet/aspnet:6.0

RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/' /etc/ssl/openssl.cnf \
    && sed -i 's/CipherString = DEFAULT@SECLEVEL=2/CipherString = DEFAULT@SECLEVEL=1/' /etc/ssl/openssl.cnf

# Install the agent
RUN apt-get update && apt-get install -y wget ca-certificates gnupg \
&& echo 'deb http://apt.newrelic.com/debian/ newrelic non-free' | tee /etc/apt/sources.list.d/newrelic.list \
&& wget https://download.newrelic.com/548C16BF.gpg \
&& apt-key add 548C16BF.gpg \
&& apt-get update \
&& apt-get install -y newrelic-dotnet-agent


ARG NEW_RELIC_LICENSE_KEY
# Enable the agent
ENV CORECLR_ENABLE_PROFILING=1 \
CORECLR_PROFILER={36032161-FFC0-4B61-B559-F6C5D41BAE5A} \
CORECLR_NEWRELIC_HOME=/usr/local/newrelic-dotnet-agent \
CORECLR_PROFILER_PATH=/usr/local/newrelic-dotnet-agent/libNewRelicProfiler.so \
NEW_RELIC_LICENSE_KEY=${NEW_RELIC_LICENSE_KEY:-"local"} \
NEW_RELIC_APP_NAME=EvenTransit

WORKDIR /app

COPY --from=build-env /artifacts .

ADD /scripts/ci-docker-entrypoint.sh ./ci-docker-entrypoint.sh
RUN chmod +x ./ci-docker-entrypoint.sh


ENV ASPNETCORE_URLS=http://*:5000
EXPOSE 5000

ENTRYPOINT ["bash", "ci-docker-entrypoint.sh"]
