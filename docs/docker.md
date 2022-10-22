# Docker Setup

## Build Image:
`docker build -t tshock .`
## and run:
```bash
docker run -p 7777:7777 -p 7878:7878 \
           -v <save path>:/tshock \
           -v <world path>:/worlds \
           -v <plugin path>:/plugins \
           --rm -it tshock [-world /worlds/<world file>] <other cmdline flags>
```