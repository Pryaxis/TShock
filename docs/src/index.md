# TShock Downloaded Documentation

*Created for TShock version: 3.5.x.x*

*Last updated: 2/25/2012*

----

## Preface

Welcome to the official TShock for Terraria downloaded documentation. This guide will walk through the installation and basic configuration of your newly downloaded TShock server, and should provide a basic knowledge as to how to get help from outside resources if needed.

## Resources

* [The Confluence wiki](http://develop.tshock.co:8080/) contains the most up to date information compiled by the community members. If your question isn't answered here, you might find it there.
* [The forums](http://tshock.co/xf/) provide an excellent place to ask other TShock users and developers questions. Please refrain from making posts about questions that may be answered here, however.
* [Our Github page](http://github.com/TShock/TShock) is where you'll be able to find the source code and the bug tracker.
* [IRC](irc://irc.shankshock.com/terraria) is our IRC channel, if you prefer that medium for support.
* Lastly, we can be found in the "Nyx" channel on the Teamspeak 3 server: ts3.shankshock.com, port 9987.

----

## Table of contents

1. [Installation](#Installation)
2. [Configuration](#Configuration)
3. [Basic usage](#Basic usage)
4. [Plugins](#Plugins)

----

### Installation <a id="Installation"></a>

1. Assuming you've extracted TShock, you're done as far as files go. Run the TerrariaServer.exe file in the folder you've extracted TShock into.
2. Check to verify that the server window states that some version of TerrariaShock is now running. If this is not the case, stop. Re-download all files and extract them to a new folder, being sure to keep the file structure in tact.
3. Select a world and port to start the server. *TShock now uses its configuration file to control the number of players on the server. You can edit this value in the configuration file, discussed later. If you are generating a new world, you may experience a significantly longer time as the world creates itself. This is normal.
4. Once the server is finished starting, you will be greeted with TShock's main console window. You will see a message in yellow that states "*To become superadmin, join the game and type /auth*" preceding a series of numbers. This set of numbers we will refer to as the "authcode" in succeeding steps.
5. Connect to the server. Your IP address is 127.0.0.1, and the port will by default be on what you entered in the server console.
6. Immediately chat the following: "**/auth [authcode]**". Replace [authcode] with the code given in the server console. Example: /auth 123456.
7. Next, we will create a user account that you can login to. In the game, chat the command "**/user add [username]:[password] superadmin**". Replace [username] and [password] respectively with your appropriate details. You should be able to remeber your password, and it shouldn't be easily guessed. From now on, the phrase "run the command" is synonymous with "chat in the game chat box". In addition, where brackets ([]) are, we will assume you will replace those brackets with information that you have created.
8. Assuming the last step was a success, login. Run the command "**/login [username] [password]**".
9. To finalize installation, run the command "**/auth-verify**". This will disable the authcode, enable any previously disabled functionality, and allow the server to be used in production.

### Basic Usage

Now that TShock is running, you may be interested in a few other features prior to playing Terraria.

* You can add admins through two methods. If the user is already registered, you can use "**/user group [username] [group-to-change-to]**". By default, TShock comes with the "vip" group, the "trustedadmin" group, and the "newadmin" group.
* When you join the server and already have an account, the server will ask for your account password, even if the server has no password setup. In the event that you set a password on the server, unregistered users will be required to enter it. Users that already have an account must enter their own password.
* If a user wishes to change accounts but retain their group, a config option exists that will allow you to allow users to login to accounts with any username.