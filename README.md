# Toolkit
Toolkit is intended to be an all-in-one plugin solution for my Unturned(3.x) server, [The Nexis Realms](http://nexisrealms.com). My goal for this project was to combine all the plugin elements I used on my server into one single plugin that didn't rely on MySQL. Toolkit instead uses JSON as a back-end database that is only updated when the plugin is loaded or unloaded. Toolkit is designed to be used by both players and admins, containing many admin tools making it more simple to make changes to the plugin in-game. Each feature can be easily enabled/disabled via plugin configuration.

# Features

  - Custom death messages
  - Player-to-player teleporting (tpa)
  - Player connection/disconnection messages
  - Player-to-player Credit Payments
  - Item Buy/Sell Shop
  - Vehicle Buy Shop
  - Player Location Warping
  - Automatted chat feedback, based on chat keywords
  - Majority controlled daytime, when saying "day" in chat
  - Automatted server messages
  - UconomyUI-compatible (show player credits on-screen)
  - Experience exchange for credits

In-game admins can also:
  - Add/delete warp locations/nodes
  - Add/delete/edit items in the shop
  - Add/delete/edit vehicles in the shop
  - Add a new automatted message

# Commands
#### Admin Commands
Add Item: Add a new item to shop
```sh
/additem <id> <name> <buyprice> <sellprice>
```
Add Kit: Create a new kit based on all items you currently have
```sh
/addkit <name> [cost]
```
Add Message: Add a new message to the automatted messages
```sh
/addmessage <message> [color]
```
Add Vehicle: Add a new vehicle to shop
```sh
/addvehicle <id> <name> <buyprice>
```
Add Warp: Adds new warp location from your current position
```sh
/addwarp <name> <price>
```
Add Warp Node: Adds a warp spawn location to an existing warp
```sh
/addwarpnode <warp>
```
Delete Item: Delete an existing item in the shop
```sh
/delitem <id>
```
Delete Kit: Delete an existing kit (non-recoverable)
```sh
/delkit <name>
```
Delete Vehicle: Delete an existing vehicle in the shop
```sh
/delvehicle <id>
```
Delete Warp: Permanently delete a warp location and ALL nodes associated with it
```sh
/delwarp <name>
```
Edit Item: Edit an existing item in the shop
```sh
/edititem <id> <name> <buyprice> <sellprice>
```
Edit Vehicle: Edit an existing vehicle in the shop
```sh
/editvehicle <id> <name> <buyprice>
```
Toolkit: Admin control for Toolkit plugin
```sh
/toolkit <help|info|update>
```

#### Player Commands
Balance: View your credit balance
```sh
/balance
```
Buy: Buy an item that spawns in your inventory
```sh
/buy <id> [amount]
```
Cost: Return the buy price of an item
```sh
/cost <id>
```
Exchange: Exchange your experience for credits
```sh
/exchange <xp>
```
Kit: Purchase a pre-made kit of items
```sh
/kit <name>
```
Kits: A list of all available kits for purchase
```sh
/kits
```
Pay: Gives another player some of your credits
```sh
/pay <name> <amount>
```
Sell: Sell items in your inventory for credits
```sh
/sell <id> [amount]
```
TPA: Teleport to another player location
```sh
/tpa <name|accept|deny>
```
Buy Vehicle: Buy a vehicle that spawns to your location
```sh
/vbuy <id>
```
Vehicle Cost: Return the price of a vehicle
```sh
/vcost <id>
```
Warp: Warp to a specific location
```sh
/warp <location>
```
Warps: A list of all available warp locations
```sh
/warps
```
