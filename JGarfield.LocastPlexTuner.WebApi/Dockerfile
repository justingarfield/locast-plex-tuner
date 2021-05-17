FROM mcr.microsoft.com/dotnet/aspnet:5.0

RUN apt-get update \
	&& apt-get install -y --no-install-recommends ffmpeg

COPY bin/Release/net5.0/publish/ App/
WORKDIR /App
ENTRYPOINT ["dotnet", "JGarfield.LocastPlexTuner.WebApi.dll"]
