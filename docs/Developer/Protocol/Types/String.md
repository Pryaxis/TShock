Terraria uses the built-in .NET types `System.IO.BinaryReader` and `System.IO.BinaryWriter` to send strings across the network. This is accomplished using the `ReadString()` and `Write(String)` methods respectively.

| Description | Type |
|-------------|------|
| Length     | 7-bit encoded int |
| Characters | `byte[Length]` |