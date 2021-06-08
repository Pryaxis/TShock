## Sending Packets Across the Network
When packets are actually written and read, they are prefixed with their length and ID.

| Description | Type |
|-------------|------|
| Length      | ushort |
| Packet ID   | byte |
| Data        | `byte[Length - 3]` |

## Connection Process
TODO: Document the packets used and process that goes into connecting to a server, or accepting a client (both sides).

## Packets
- [[Connect Request]]
- [[Disconnect]]
- [[Set User Slot]]
- [[Player Info]]
- [[Sync Inventory Slot]]