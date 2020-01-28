FROM mcr.microsoft.com/dotnet/core/runtime:3.0

COPY bin/Release/netcoreapp3.0/publish/ app/

ENTRYPOINT ["dotnet", "app/MassTransit.Azure.Producer.dll"]