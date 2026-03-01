# TARGETPLATFORM and BUILDPLATFORM are automatically filled in by Docker buildx.
# They should not be set in the global scope manually.

FROM --platform=${BUILDPLATFORM} mcr.microsoft.com/dotnet/sdk:9.0 AS builder

# Install msgfmt for i18n
RUN apt-get update && apt-get install -y gettext

# Copy build context
WORKDIR /TShock
COPY . ./

# Build and package release based on target architecture
RUN dotnet build -v m
WORKDIR /TShock/TShockLauncher

# Make TARGETPLATFORM available to the container.
ARG TARGETPLATFORM

RUN \ 
  case "${TARGETPLATFORM}" in \
    "linux/amd64") export ARCH="linux-x64" \
    ;; \
    "linux/arm64") export ARCH="linux-arm64" \
    ;; \
    "linux/arm/v7") export ARCH="linux-arm" \
    ;; \
    "windows/amd64") export ARCH="win-x64" \
    ;; \
    *) echo "Error: Unsupported platform ${TARGETPLATFORM}" && exit 1 \
    ;; \
  esac && \
  dotnet publish -o output/ -r "${ARCH}" -v m -f net9.0 -c Release -p:PublishSingleFile=true --self-contained false

# Runtime image
FROM mcr.microsoft.com/dotnet/runtime:9.0 AS linux_base
FROM mcr.microsoft.com/dotnet/runtime:9.0-nanoserver-ltsc2022 AS windows_base

FROM ${TARGETOS}_base AS final

WORKDIR /server
COPY --from=builder /TShock/TShockLauncher/output ./

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
