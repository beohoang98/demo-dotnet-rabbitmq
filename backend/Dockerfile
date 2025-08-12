FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build

WORKDIR /app

COPY . .

RUN dotnet restore
RUN dotnet build --no-restore
RUN dotnet publish --no-restore -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS runtime

WORKDIR /app

COPY --from=build /app/out/. .

ENTRYPOINT [ "/bin/sh", "-c" ]

CMD [ "dotnet", "dictionary.api.dll" ]
