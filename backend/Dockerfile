FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app


COPY MedicalDemo.csproj ./
RUN dotnet restore


COPY . .
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/out .

EXPOSE 5109
ENTRYPOINT ["dotnet", "MedicalDemo.dll"]
