# Docker Setup

In order to run TShock in a docker container, you would need to have mountpoints for
 - `/tshock` (TShock config files, logs and crash reports)
 - `/worlds`
 - `/plugins`
 - `/server` (optional, if you want to mount TShock's cwd)

These folders can be mounted using `-v <host_folder>:<container>`

Open ports can also be passed through using `-p <host_port>:<container_port>`.
 - `7777` for Terraria
 - `7878` for TShock's REST API

For Example:
```bash
# Building the image using buildx and loading it into docker
docker buildx build -t tshock:latest --load .

# Running the image
docker run -p 7777:7777 -p 7878:7878 \
           -v /home/cider/tshock/:/tshock \
           -v /home/cider/.local/share/Terraria/Worlds:/worlds \
           -v /home/cider/tshock/plugins:/plugins \
           --rm -it tshock:latest \
           -world /worlds/backflip.wld -motd "OMFG DOCKER"
```

## Building for Other Platforms

Using `docker buildx`, you could build [multi-platform images](https://docs.docker.com/build/building/multi-platform/) for TShock.

For Example:
```bash
# Building the image using buildx and loading it into docker
docker buildx build -t tshock:linux-arm64 --platform linux/arm64 --load .

# Running the image
docker run -p 7777:7777 -p 7878:7878 \
           -v /home/cider/tshock/:/tshock \
           -v /home/cider/.local/share/Terraria/Worlds:/worlds \
           -v /home/cider/tshock/plugins:/plugins \
           --rm -it tshock:linux-arm64 \
           -world /worlds/backflip.wld -motd "ARM64 ftw"
```
