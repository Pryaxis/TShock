# TShock Terraria Server

## Quick start guide

First you need a machine with [Docker][Docker] installed. Everything from here on out assumes the docker service is running _(you may need to start the service after install)_.

### Create directory to save your world to

Next create a directory for your world file, configuration, and logs

```bash
mkdir -p $HOME/terraria/world
```

### Creating a fresh world

For the first run you will need to generate a new world with a size where: _1=Small, 2=Medium, 3=Large_

```bash
sudo docker run -it -p 7777:7777 --rm -v $HOME/terraria/world:/root/.local/share/Terraria/Worlds terraria:latest -world /root/.local/share/Terraria/Worlds/<world_name_here>.wld -autocreate <world_size_number_here>
```

**Note:** If you close the the terminal, the server will stop running.  You will need to restart with a preexisting world. It may
be worth while to close after creation anyway to update the initial `config.json` settings.

To create a world with a few more initial options, you can do so in an interactive mode.

```bash
sudo docker run -it -p 7777:7777 --rm -v $HOME/terraria/world:/root/.local/share/Terraria/Worlds terraria:latest
```

### To start with a preexisting world

```bash
sudo docker run -d --rm -p 7777:7777 -v $HOME/terraria/world:/root/.local/share/Terraria/Worlds --name="terraria" -e WORLD_FILENAME=<.wld world_filename_here> terraria:latest
```

**Note:** This command is designed to run in the background, and it is safe to close the terminal window.

Any `config.json` in the directory will automatically be loaded.  The `<world_file_name>.wld` should be the name of your wld file in your $HOME/terraria/world directory.

## Updating your container

Updating is easy!  

1. Grab the latest terraria container

    ```bash
    docker pull terraria:latest
    ```

2. First we need to find our running container to stop, so we can later restart with the latest

    ```bash
    docker container ls | grep terraria
    ```

    The first few numbers and letters, on a line, are the container hash.  Remember the first 3 or so letters or numbers

    Example:

    ```bash
    f25261ac55a4        terraria:latest   "/bin/sh bootstrap.s…"   3 minutes ago       Up 3 minutes        0.0.0.0:7777->7777/tcp, 7878/tcp   reverent_solomon
    ```

    `f25` would be the first few letters/numbers of the container hash

    **NOTE:** If you see multiple lines, find the one that still has an `up` status.

3. Stop and remove the container

    ```bash
    docker container rm -f xxx # xxx is the letters/numbers from the last step
    ```

4. Start your container again with your world _(see the [Quick start](#Quick-start-guide))_

## [Virtual] Machine Setup

Provision a linux machine that can support docker and containerization.  For more information visit [docker][Docker].  For a small or medium world with no more than 8 users a linux machine with 1-1.5GB of ram should suffice.  **If you are running a vm in the cloud, make sure to expose tcp port 7777 and udp port 7777.**

## Running a container image

Whether you build your own container, or use [my container](https://hub.docker.com/r/terraria) published to docker hub,
we are ready to run our terraria server!

**Note:** For a full set of docker run options go [here](https://docs.docker.com/engine/reference/run/)

### First run

The first run of the server will give you the opportunity to create a world, and it will generate
the server's config file.  You may wish to add the config file for many reasons, but one of which is to
add a password to your server.

```bash
docker run -it --rm -p 7777:7777 -v $HOME/terraria/world:/root/.local/share/Terraria/Worlds terraria:latest
```

Let's break down this command:

| Command Part | Description |
| ------------ | ----------- |
| `docker run` | tells linux to run a docker image |
| `-it` | run interactively and output the text to terminal |
| `--rm` | remove docker container when the container stops or fails |
| `-p 7777:7777` | exposes terraria port &lt;host machine side>:&lt;container side> |
| `-v $HOME/terraria/world:/root/.local/share/Terraria/Worlds` | maps a folder on the host machine into the container for saving the .wld file.  This does not have to be `$HOME/terraria/world`.  Anything left of the `:` is host machine directory |
| `terraria` | the name of the docker image that will run. |
| `:latest` | the tag, which defaults to `latest` if not specified.  `latest` is the most recently published container |

* The config file can be found in the directory specified by the `-v` volume.
* If the terminal window is shut down, that will exit the process.  Make sure to do so after the world is created!

### Running with an existing generated world

After a world has been generated, you may want to load directly into it.  

```bash
docker run -d --rm -p 7777:7777 -v $HOME/terraria/world:/root/.local/share/Terraria/Worlds terraria:latest -world /root/.local/share/Terraria/Worlds/<world_filename_here>.wld
```

Let's break down the command:

| Command Part | Description |
| ------------ | ----------- |
| `-d` | run this in the background.  It is okay to close the terminal window, the container will continue to run |
| `-world /root/.local/share/Terraria/Worlds/<world_filename_here>.wld` | specifies the world file name you wish to immediately load into |

* for the other parts check out the [First run](#First-run) section
* check out additional server startup flags [here](https://tshock.readme.io/docs/command-line-parameters).  They go on
after the `terraria:latest` portion of the line

## Plugin support

A volume exists to support plugins.  Create a folder, not inside your `/world` folder, for your plugins

```bash
mkdir ServerPlugins
```

Mount the plugins directory with an additional -v switch on your `docker run ...` command

```bash
-v <path_to_your_ServerPlugins_folder>:/plugins
```

## Logs

A separate directory can be volumed in for storing logs outside of the image

```bash
-v <path_to_store_logs>:/tshock/logs
```

## *Notes*

* `sudo` may be required to run docker commands.

* Please post to the [TShock](https://github.com/Pryaxis/TShock/discussions) team with questions on how to run a server.

* Any [additional command-line instructions](https://tshock.readme.io/docs/command-line-parameters) can be added to the end of either method for launching a server.  Docker maps the $HOME/terraria/world linux-host folder to the /tshock/world container-folder.

* Expecting your server to run for a while?  Add `--log-opt max-size=200k` to limit your log file size.  Otherwise one day you will wake up to see all your hdd space chewed up for a terraria docker setup!


[TShock]: https://github.com/Pryaxis/TShock/releases
[Docker]: https://docs.docker.com/get-docker/
