TShock ships with two tile providers, which replace the Terraria tile datastore with different systems. Additionally, one major plugin developer provides an additional set of tile providers. For developers, these providers implement the `ITile` interface and register themselves as Tile Providers. `ITile` is provided by `OTAPI`.

Terraria, by default, stores tiles in a relatively unoptimized way. Hypothetically, this is the fastest way to access tiles, but it does so at the cost of memory. If you're running a Terraria server with limited memory, you may want to get back memory and trade off processing power instead. That's what these providers do.

## Constileation

Constileation is the newest tile provider shipped by TShock. It's faster than HeapTile, and saves memory. It uses 14 bytes per tile. Start your TShock server with the `-c` or `-constileation` command line argument to use this provider.

## HeapTile

HeapTile is one of the earliest tile providers shipped by TShock. Again, it offers memory advantages, but is really slow compared to Constileation and Tiled. Start your TShock server with the `-heaptile` command line argument to use this provider.

## Tiled

The [tiled plugin for TShock](https://github.com/thanatos-tshock/Tiled) by [thanatos](https://github.com/thanatos-tshock) offers additional tile providers, including their `1d`, `2d`, and `struct` providers. We urge you to check out and compare all tile providers to find the one that best suits your needs. Tiled attempts to bring the best of both worlds, offering tile providers that minimize memory usage while offering modest performance.

## Tile provider benchmarks

[@SignatureBeef](https://github.com/SignatureBeef) did benchmarks of various tile providers. Some run examples from [TSAPI PR #231](https://github.com/Pryaxis/TSAPI/pull/231) are reproduced here. For these, these are the providers:

* `Stock` is the Terraria server stock configuration.
* `Heap` is `HeapTile`
* `Constileation` is `Constileation`
* `1d` is from Tiled
* `2d` is from Tiled
* `Struct` is from Tiled

Bench: how fast each provider can call .active, for every tile in a small world

|               Method |      Mean |    Error |   StdDev | Ratio | RatioSD |
|--------------------- |----------:|---------:|---------:|------:|--------:|
|         Active_Stock |  31.78 ms | 0.456 ms | 0.426 ms |  1.00 |    0.00 |
|          Active_Heap | 122.07 ms | 2.413 ms | 2.370 ms |  3.84 |    0.09 |
| Active_Constileation |  40.41 ms | 0.575 ms | 0.510 ms |  1.27 |    0.02 |
|            Active_1d |  52.94 ms | 0.731 ms | 0.648 ms |  1.66 |    0.03 |
|            Active_2d |  54.96 ms | 1.083 ms | 1.064 ms |  1.73 |    0.05 |
|        Active_Struct |  61.59 ms | 0.651 ms | 0.544 ms |  1.93 |    0.03 |

Bench: how fast the provider can issue and store its own data, for every tile in a small world

|                       Method |      Mean |    Error |   StdDev | Ratio | RatioSD |
|----------------------------- |----------:|---------:|---------:|------:|--------:|
|         AssignFromSelf_Stock |  34.30 ms | 0.486 ms | 0.454 ms |  1.00 |    0.00 |
|          AssignFromSelf_Heap | 267.03 ms | 3.455 ms | 3.062 ms |  7.79 |    0.15 |
| AssignFromSelf_Constileation |  42.30 ms | 0.664 ms | 0.621 ms |  1.23 |    0.03 |
|            AssignFromSelf_1d | 208.46 ms | 2.823 ms | 2.640 ms |  6.08 |    0.10 |
|            AssignFromSelf_2d | 242.42 ms | 4.669 ms | 6.234 ms |  7.11 |    0.24 |
|        AssignFromSelf_Struct | 278.45 ms | 4.580 ms | 4.284 ms |  8.12 |    0.13 |

Bench: how fast the provider can translate other tile data, such as Terraria.Tile, for every tile in a small world

|                       Method |     Mean |   Error |  StdDev | Ratio |
|----------------------------- |---------:|--------:|--------:|------:|
|         AssignFromTile_Stock | 433.3 ms | 7.22 ms | 6.40 ms |  1.00 |
|          AssignFromTile_Heap | 210.4 ms | 3.33 ms | 3.11 ms |  0.49 |
| AssignFromTile_Constileation | 195.7 ms | 2.45 ms | 2.29 ms |  0.45 |
|            AssignFromTile_1d | 239.0 ms | 2.07 ms | 1.83 ms |  0.55 |
|            AssignFromTile_2d | 240.5 ms | 3.03 ms | 2.69 ms |  0.56 |
|        AssignFromTile_Struct | 277.6 ms | 3.74 ms | 3.49 ms |  0.64 |

Bench: how fast a provider can clear tile data (using methods), for every tile in a small world

|            Method |     Mean |    Error |   StdDev | Ratio | RatioSD |
|------------------ |---------:|---------:|---------:|------:|--------:|
|         Clear_Stock | 308.8 ms |  3.90 ms |  3.65 ms |  1.00 |    0.00 |
|          Clear_Heap | 479.1 ms |  9.20 ms |  9.84 ms |  1.55 |    0.03 |
| Clear_Constileation | 331.2 ms |  2.16 ms |  1.92 ms |  1.07 |    0.01 |
|            Clear_1d | 557.8 ms |  3.24 ms |  3.03 ms |  1.81 |    0.03 |
|            Clear_2d | 692.9 ms | 20.61 ms | 59.78 ms |  2.14 |    0.16 |
|        Clear_Struct | 789.2 ms | 13.95 ms | 22.93 ms |  2.60 |    0.11 |

Bench: how fast a provider can run similar logic to the clear world function found in vanilla, for every tile in a small world

|                   Method |      Mean |    Error |   StdDev | Ratio | RatioSD |
|------------------------- |----------:|---------:|---------:|------:|--------:|
|         ClearWorld_Stock |  79.27 ms | 0.669 ms | 0.593 ms |  1.00 |    0.00 |
|          ClearWorld_Heap | 239.87 ms | 2.324 ms | 2.060 ms |  3.03 |    0.03 |
| ClearWorld_Constileation |  62.48 ms | 0.948 ms | 0.887 ms |  0.79 |    0.01 |
|            ClearWorld_1d | 139.73 ms | 1.621 ms | 1.516 ms |  1.76 |    0.02 |
|            ClearWorld_2d | 151.27 ms | 2.341 ms | 2.190 ms |  1.91 |    0.03 |
|        ClearWorld_Struct | 177.16 ms | 3.190 ms | 2.828 ms |  2.23 |    0.05 |

Bench: how fast a provider can issue tile data, for every tile in a small world

|             Method |      Mean |    Error |   StdDev | Ratio | RatioSD |
|------------------- |----------:|---------:|---------:|------:|--------:|
|         Gets_Stock |  17.49 ms | 0.224 ms | 0.210 ms |  1.00 |    0.00 |
|          Gets_Heap | 100.88 ms | 2.007 ms | 2.465 ms |  5.76 |    0.17 |
| Gets_Constileation |  27.15 ms | 0.513 ms | 0.455 ms |  1.55 |    0.03 |
|            Gets_1d |  29.46 ms | 0.431 ms | 0.403 ms |  1.68 |    0.03 |
|            Gets_2d |  27.93 ms | 0.258 ms | 0.242 ms |  1.60 |    0.02 |
|        Gets_Struct |  33.77 ms | 0.671 ms | 1.719 ms |  1.85 |    0.08 |

Bench: how fast a provider can change the type of a tile, for every tile in a small world

|             Method |      Mean |    Error |   StdDev | Ratio | RatioSD |
|------------------- |----------:|---------:|---------:|------:|--------:|
|         Type_Stock |  28.41 ms | 0.420 ms | 0.393 ms |  1.00 |    0.00 |
|          Type_Heap | 120.99 ms | 2.317 ms | 2.480 ms |  4.25 |    0.10 |
| Type_Constileation |  36.95 ms | 0.732 ms | 0.977 ms |  1.30 |    0.04 |
|            Type_1d |  38.65 ms | 0.686 ms | 1.027 ms |  1.38 |    0.04 |
|            Type_2d |  39.64 ms | 0.405 ms | 0.359 ms |  1.40 |    0.02 |
|             Type_Struct |  43.09 ms | 0.749 ms | 0.664 ms |  1.52 |    0.03 |

Bench: test how fast a provider can call basic actions, like clearing, type change and .active, for every tile in a small world

|            Method |       Mean |    Error |   StdDev | Ratio | RatioSD |
|------------------ |-----------:|---------:|---------:|------:|--------:|
|         Use_Stock |   359.4 ms |  4.67 ms |  4.14 ms |  1.00 |    0.00 |
|          Use_Heap |   666.4 ms | 10.78 ms | 10.08 ms |  1.86 |    0.04 |
| Use_Constileation |   407.3 ms |  5.87 ms |  5.20 ms |  1.13 |    0.02 |
|            Use_1d |   654.5 ms |  7.30 ms |  6.83 ms |  1.82 |    0.03 |
|            Use_2d |   741.7 ms |  8.19 ms |  7.66 ms |  2.07 |    0.03 |
|        Use_Struct | 1,235.3 ms |  7.60 ms |  7.11 ms |  3.44 |    0.03 |
