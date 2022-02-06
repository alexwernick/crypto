FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build

ARG BUILDCONFIG=RELEASE
ARG VERSION=1.0.0

# copy csproj and restore as distinct layers
COPY ./Crypto/Crypto.csproj ./Crypto/
COPY ./nuget.config ./
RUN dotnet restore Crypto/Crypto.csproj

# copy everything else and build
COPY ./ ./
WORKDIR /Crypto/
RUN dotnet publish -c $BUILDCONFIG -o out /p:Version=$VERSION

# build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine
WORKDIR /app
COPY --from=build /Crypto/out ./

EXPOSE 5000
ENTRYPOINT ["dotnet", "Crypto.dll"]