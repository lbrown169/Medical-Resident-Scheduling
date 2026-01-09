FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

COPY MedicalDemo.sln ./
COPY MedicalDemo.csproj ./
RUN dotnet restore MedicalDemo.sln

COPY . .
RUN dotnet publish MedicalDemo.sln -c Release -o out --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/out .

EXPOSE 5109
ENTRYPOINT ["dotnet", "MedicalDemo.dll"]
