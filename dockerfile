FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["SmallClientBusiness/SmallClientBusiness.csproj", "SmallClientBusiness/"]
COPY ["SmallClientBusiness.BL/SmallClientBusiness.BL.csproj", "SmallClientBusiness.BL/"]
COPY ["SmallClientBusiness.Common/SmallClientBusiness.Common.csproj", "SmallClientBusiness.Common/"]
COPY ["SmallClientBusiness.DAL/SmallClientBusiness.DAL.csproj", "SmallClientBusiness.DAL/"]
RUN dotnet restore "SmallClientBusiness/SmallClientBusiness.csproj"
COPY . .
WORKDIR "/src/SmallClientBusiness"

RUN dotnet build "SmallClientBusiness.csproj" -c Release -o /app/build
FROM build AS publish

RUN dotnet publish "SmallClientBusiness.csproj" -c Release -o /app/publish
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SmallClientBusiness.dll"]
