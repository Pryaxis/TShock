###### ID
9

###### Direction
Server -> Client

### Structure
| Description | Type |
|-------------|------|
| Status Max  | byte |
| Status Text | [[Network Text]] |
| [[#Flags]]  | byte |

###### Flags
| Description | Value |
|-------------|------|
| Hide Percentage | `1 << 0` |
| Show Shadows    | `1 << 1` |