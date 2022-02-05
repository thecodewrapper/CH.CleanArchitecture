#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Presentation/CH.CleanArchitecture.Presentation.Web/CH.CleanArchitecture.Presentation.Web.csproj", "CH.CleanArchitecture.Presentation.Web/"]
COPY ["Presentation/CH.CleanArchitecture.Presentation.Framework/CH.CleanArchitecture.Presentation.Framework.csproj", "CH.CleanArchitecture.Presentation.Framework/"]
COPY ["Core/CH.CleanArchitecture.Core.Application/CH.CleanArchitecture.Core.Application.csproj", "CH.CleanArchitecture.Core.Application/"]
COPY ["Core/CH.CleanArchitecture.Core.Domain/CH.CleanArchitecture.Core.Domain.csproj", "CH.CleanArchitecture.Core.Domain/"]
COPY ["Infrastructure/CH.CleanArchitecture.Infrastructure/CH.CleanArchitecture.Infrastructure.csproj", "CH.CleanArchitecture.Infrastructure/"]
COPY ["Infrastructure/CH.CleanArchitecture.Infrastructure.Shared/CH.CleanArchitecture.Infrastructure.Shared.csproj", "CH.CleanArchitecture.Infrastructure.Shared/"]
COPY ["Infrastructure/CH.CleanArchitecture.Infrastructure.Auditing/CH.CleanArchitecture.Infrastructure.Auditing.csproj", "CH.CleanArchitecture.Infrastructure.Auditing/"]
COPY ["Infrastructure/CH.CleanArchitecture.Infrastructure.Resources/CH.CleanArchitecture.Infrastructure.Resources.csproj", "CH.CleanArchitecture.Infrastructure.Resources/"]
COPY ["CH.CleanArchitecture.Common/CH.CleanArchitecture.Common.csproj", "CH.CleanArchitecture.Common/"]
RUN dotnet restore "CH.CleanArchitecture.Presentation.Web/CH.CleanArchitecture.Presentation.Web.csproj"
COPY . .
WORKDIR "/src/CH.CleanArchitecture.Presentation.Web"
RUN dotnet build "CH.CleanArchitecture.Presentation.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CH.CleanArchitecture.Presentation.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CH.CleanArchitecture.Presentation.Web.dll"]
