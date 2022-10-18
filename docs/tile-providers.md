TShock ships with two tile providers, which replace the Terraria tile datastore with different systems. Additionally, one major plugin developer provides an additional set of tile providers. For developers, these providers implement the `ITile` interface and register themselves as Tile Providers. `ITile` is provided by `OTAPI`.

Terraria, by default, stores tiles in a relatively unoptimized way. Hypothetically, this is the fastest way to access tiles, but it does so at the cost of memory. If you're running a Terraria server with limited memory, you may want to get back memory and trade off processing power instead. That's what these providers do.

## Constileation

Constileation is the newest tile provider shipped by TShock. It's faster than HeapTile, and saves memory. It uses 14 bytes per tile. Start your TShock server with the `-c` or `-constileation` command line argument to use this provider.

## HeapTile

HeapTile is one of the earliest tile providers shipped by TShock. Again, it offers memory advantages, but is really slow compared to Constileation and Tiled. Start your TShock server with the `-heaptile` command line argument to use this provider.

## Tiled

The [tiled plugin for TShock](https://github.com/thanatos-tshock/Tiled) by [thanatos](https://github.com/thanatos-tshock) offers additional tile providers, including their `1d`, `2d`, and `struct` providers. We urge you to check out and compare all tile providers to find the one that best suits your needs. Tiled attempts to bring the best of both worlds, offering tile providers that minimize memory usage while offering modest performance.
