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

WORKDIR /app

COPY --from=build-env /artifacts .

ADD /scripts/ci-docker-entrypoint.sh ./ci-docker-entrypoint.sh
RUN chmod +x ./ci-docker-entrypoint.sh


ENV ASPNETCORE_URLS=http://*:5000
EXPOSE 5000

ENTRYPOINT ["bash", "ci-docker-entrypoint.sh"]
