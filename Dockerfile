FROM microsoft/dotnet:2.0-runtime

COPY _dist/ /app

ENV ASPNETCORE_URLS=http://0.0.0.0:5000
CMD dotnet /app/MyApp.dll