# Base image for the ASP.NET Core app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# --- Build ASP.NET Core Application ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MilvusLangchainSemanticSearch/MilvusLangchainSemanticSearch.csproj", "MilvusLangchainSemanticSearch/"]
RUN dotnet restore "MilvusLangchainSemanticSearch/MilvusLangchainSemanticSearch.csproj"
COPY . .
WORKDIR "/src/MilvusLangchainSemanticSearch"
RUN dotnet build "MilvusLangchainSemanticSearch.csproj" -c $BUILD_CONFIGURATION -o /app/build

# --- Publish ASP.NET Core Application ---
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MilvusLangchainSemanticSearch.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# --- Final stage: Combine everything ---
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# --- Start the ASP.NET Core App ---
ENTRYPOINT ["dotnet", "MilvusLangchainSemanticSearch.dll"]
