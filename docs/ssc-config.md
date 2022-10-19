## Enabled  
Enable server side characters, causing client data to be saved on the server instead of the client.
* **Field type**: `Boolean`
* **Default**: `False`

## LogonDiscardThreshold  
Time, in milliseconds, to disallow discarding items after logging in when ServerSideCharacters is ON.
* **Field type**: `Int32`
* **Default**: `250`

## ServerSideCharacterSave  
How often SSC should save, in minutes.
* **Field type**: `Int32`
* **Default**: `5`

## StartingHealth  
The starting default health for new players when SSC is enabled.
* **Field type**: `Int32`
* **Default**: `100`

## StartingInventory  
The starting default inventory for new players when SSC is enabled.
* **Field type**: `List`1`
* **Default**: `System.Collections.Generic.List\`1[TShockAPI.NetItem]`

## StartingMana  
The starting default mana for new players when SSC is enabled.
* **Field type**: `Int32`
* **Default**: `20`

## WarnPlayersAboutBypassPermission  
Warns players and the console if a player has the tshock.ignore.ssc permission with data in the SSC table.
* **Field type**: `Boolean`
* **Default**: `True`

