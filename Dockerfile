FROM mcr.microsoft.com/dotnet/core/sdk:3.1
WORKDIR /app

EXPOSE 33000

COPY ./ ./

RUN dotnet publish -c Release -o out

ENTRYPOINT ["dotnet", "./out/Chat.Server.dll"]