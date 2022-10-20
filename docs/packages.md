TShock supports downloading and installing plugins from NuGet repositories.
This allows it to automatically download the plugin as well as all of the extra things that the plugin needs.
For developers, this makes distributing plugins easier.

This functionality is accessible via the TShock.Server executable used to run the server normally.

Under Linux:
```
./TShock.Server plugins
```

Under Windows (cmd.exe):
```
TShock.Server plugins
```

The documentation for the commands is included in the help functionality.
A copy of the help output in English can be found in [packages-help.txt](packages-help.txt).
This file primarily exists to document the `packages.json`.

The file format is currently simple, including only a single object, containing a key `packages` that has a map of package IDs to their versions.

An example `packages.json` is shown below:
```
{
    "packages": {
        "Commandy.20.10.22.Test": "0.0.1"
    }
}
```

The name of the plugin is specified as the key, with the version as the value.
