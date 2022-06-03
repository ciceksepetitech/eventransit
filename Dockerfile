FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env

COPY /cicd /cicd
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

WORKDIR /app

COPY --from=build-env /artifacts .

ADD /cicd/ci-docker-entrypoint.sh ./ci-docker-entrypoint.sh
RUN chmod +x ./ci-docker-entrypoint.sh

RUN apt-get update && \
    apt-get install -y curl && \
    rm -rf /var/lib/apt/lists/*

ENV ENVTPL_VERSION=0.2.3
RUN \
   curl -Ls https://github.com/arschles/envtpl/releases/download/${ENVTPL_VERSION}/envtpl_linux_amd64 > /usr/local/bin/envtpl &&\
   chmod +x /usr/local/bin/envtpl;

ENV ASPNETCORE_URLS=http://*:5000
EXPOSE 5000

ENTRYPOINT ["bash", "ci-docker-entrypoint.sh"]
