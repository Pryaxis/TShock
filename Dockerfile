FROM mono:6.8.0.96 AS build

ARG BUILD_MODE=Release

ADD https://dist.nuget.org/win-x86-commandline/v5.5.1/nuget.exe /
COPY . /src
RUN chmod +x /nuget.exe && \
    # build TShock
    mono /nuget.exe restore /src/TerrariaServerAPI/ && \
    xbuild /src/TerrariaServerAPI/TShock.4.OTAPI.sln /p:Configuration=$BUILD_MODE && \
    cd /src/TerrariaServerAPI/TShock.Modifications.Bootstrapper/bin/$BUILD_MODE && \
    mono TShock.Modifications.Bootstrapper.exe -in=OTAPI.dll -mod=../../../TShock.Modifications.**/bin/$BUILD_MODE/TShock.Modifications.*.dll -o=Output/OTAPI.dll && \
    cd / && \
    xbuild /src/TerrariaServerAPI/TerrariaServerAPI/TerrariaServerAPI.csproj /p:Configuration=$BUILD_MODE && \
    mono /nuget.exe restore /src/ && \
    xbuild /src/TShock.sln /p:Configuration=$BUILD_MODE && \
    # create final output
    mkdir -p /out/ServerPlugins && \
    cp /src/packages/BCrypt.Net.0.1.0/lib/net35/* /out/ && \
    cp /src/packages/MySql.Data.6.9.8/lib/net45/* /out/ && \
    cp /src/packages/Newtonsoft.Json.10.0.3/lib/net45/* /out/ && \
    cp /src/prebuilts/* /out/ && \
    cp /src/TerrariaServerAPI/TerrariaServerAPI/bin/$BUILD_MODE/OTAPI.dll /out/ && \
    cp /src/TerrariaServerAPI/TerrariaServerAPI/bin/$BUILD_MODE/TerrariaServer.* /out/ && \
    cp /src/TShockAPI/bin/$BUILD_MODE/TShockAPI.* /out/ && \
    mv /out/TShockAPI.dll /out/ServerPlugins/TShockAPI.dll

FROM mono:6.8.0.96-slim

# documenting ports
EXPOSE 7777 7878

# env used in the bootstrap
ENV CONFIGPATH=/root/.local/share/Terraria/Worlds
ENV LOGPATH=/tshock/logs
ENV WORLD_FILENAME=""

# Allow for external data
VOLUME ["/root/.local/share/Terraria/Worlds", "/tshock/logs", "/plugins"]

# copy game files
COPY --from=build /out/ /tshock/

# copy bootstrapper
COPY ./docker/bootstrap.sh /tshock/bootstrap.sh

# install nuget to grab tshock dependencies
RUN apt-get update -y && \
    apt-get install -y nuget && \
    rm -rf /var/lib/apt/lists/* /tmp/* && \
    chmod +x /tshock/bootstrap.sh

# Set working directory to server
WORKDIR /tshock

# run the bootstrap, which will copy the TShockAPI.dll before starting the server
ENTRYPOINT [ "/bin/sh", "bootstrap.sh" ]