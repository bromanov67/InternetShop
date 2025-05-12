# Используем .NET SDK для сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1. Копируем ВСЕ файлы решения (включая подпроекты)
COPY . .

# 2. Восстанавливаем зависимости через .sln-файл
RUN dotnet restore "InternetShop.sln"

# 3. Публикуем веб-проект (убедитесь в правильности пути!)
RUN dotnet publish "InternetShop/InternetShop.Web.csproj" -c Release -o /app/publish

# Собираем финальный образ
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "InternetShop.Web.dll"]