<link href="https://raw.github.com/clownfart/Markdown-CSS/master/markdown.css" rel="stylesheet"></link>

# Install instructions & basic usage

[Back to index](index.md.html)

----

1. Assuming you've extracted TShock, you're done as far as files go. Run the TerrariaServer.exe file in the folder you've extracted TShock into.
2. Check to verify that the server window states that some version of TerrariaShock is now running. If this is not the case, stop. Re-download all files and extract them to a new folder, being sure to keep the file structure in tact.
3. Select a world and port to start the server. *TShock now uses its configuration file to control the number of players on the server. You can edit this value in the configuration file, discussed later. If you are generating a new world, you may experience a significantly longer time as the world creates itself. This is normal.
4. Once the server is finished starting, you will be greeted with TShock's main console window. You will see a message in yellow that states "*To become superadmin, join the game and type /auth*" preceding a series of numbers. This set of numbers we will refer to as the "authcode" in succeeding steps.
5. Connect to the server. Your IP address is 127.0.0.1, and the port will by default be on what you entered in the server console.
6. Immediately chat the following: "**/auth [authcode]**". Replace [authcode] with the code given in the server console. Example: /auth 123456.
7. Next, we will create a user account that you can login to. In the game, chat the command "**/user add [username]:[password] superadmin**". Replace [username] and [password] respectively with your appropriate details. You should be able to remeber your password, and it shouldn't be easily guessed. From now on, the phrase "run the command" is synonymous with "chat in the game chat box". In addition, where brackets ([]) are, we will assume you will replace those brackets with information that you have created.
8. Assuming the last step was a success, login. Run the command "**/login [username] [password]**".
9. To finalize installation, run the command "**/auth-verify**". This will disable the authcode, enable any previously disabled functionality, and allow the server to be used in production.

----

### Basic Usage<a id="Basics"></a>

Now that TShock is running, you may be interested in a few other features prior to playing Terraria.

* You can add admins through two methods. If the user is already registered, you can use "**/user group [username] [group-to-change-to]**". By default, TShock comes with the "vip" group, the "trustedadmin" group, and the "newadmin" group. If the user has yet to register, you can use "**/user add [username]:[password] [group]**" to generate an account with elevated permissions for them.
* When you join the server and already have an account, the server will ask for your account password, even if the server has no password setup. In the event that you set a password on the server, unregistered users will be required to enter it. Users that already have an account must enter their own password.
* If a user wishes to change accounts but retain their group, a config option exists that will allow you to allow users to login to accounts with any username.

----

## Closing remarks<a id="Closing"></a>

Thanks for downloading TShock. Your continued support helps make TShock what it is today. We wouldn't be here without you.

From everyone at TShock, thank-you.