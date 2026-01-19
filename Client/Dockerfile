FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY Client/*.csproj ./Client/
RUN dotnet restore "Client/Client.csproj" || true

# Copy everything and publish
COPY . .
WORKDIR /src/Client
RUN dotnet publish -c Release -o /app/publish

FROM nginx:stable-alpine
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
