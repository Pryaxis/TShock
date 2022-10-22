
# Docker Instructions
# Build Image:
#  docker build -t tshock .
# and run:
#  docker run -p 7777:7777 -p 7878:7878 \
#             -v <save path>:/tshock \
#             -v <world path>:/worlds \
#             -v <plugin path>:/plugins \
#             --rm -it tshock -world /worlds/<world file> <flags>

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS builder

ARG ARCH=linux-x64

# Copy build context
WORKDIR /TShock
COPY . ./

# Build and package release
RUN dotnet build
WORKDIR /TShock/TShockLauncher
RUN dotnet publish -o output/ -r ${ARCH} -f net6.0 -c Release -p:PublishSingleFile=true --self-contained false

# Runtime image
FROM mcr.microsoft.com/dotnet/runtime:6.0 AS runner
WORKDIR /server
COPY --from=builder /TShock/TShockLauncher/output ./
RUN mkdir -p /tshock /worlds /plugins

VOLUME ["/tshock", "/worlds", "/plugins"]
EXPOSE 7777 7878

ENTRYPOINT [ \
  "./TShock.Server", \
  "-configpath", "/tshock", \
  "-logpath", "/tshock/logs", \
  "-crashdir", "/tshock/crashes", \
  "-worldselectpath", "/worlds", \
  "-additionalplugins", "/plugins" \
]