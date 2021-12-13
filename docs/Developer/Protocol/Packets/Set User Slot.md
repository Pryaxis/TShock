###### ID
3

###### Direction
Server -> Client

### Structure
| Description | Type |
|-------------|------|
| Player ID                                | byte |
| Run Check Bytes in Client Loop Thread[^1] | bool |

[^1]: This is a special flag that will make the client read packets on the TCP networking thread, rather than the main thread. The vanilla server will _always_ send `false`.