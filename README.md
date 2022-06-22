# NicehashBot

Built-in Features in current release:

- Manage pools (create/edit/delete)
- Manage orders (create/recreate/refill/edit/delete)

     Automatically manage orders for (Bot On mode):
recreating – recreates automatic expired and completed orders (but first preventing from completing)
refilling – automatic refill order with own defined amount if only a amount x is left in the order
price adjustment – keep price as low as possible to get hash (max. increase in x% and decrease prices)
own price step – define a custom price step for price adjustments (only smaller steps than predefined)
order handling – automatic calculation of price/speed and necessary price/speed/order adjustments
OneShot feature – the Bot create a new order if difficulty of a coin is low to get wished profit
Interval orders – automatic creating of order(s) to a predefined interval and timerange

- Order handler engine is full-configurable to fit the most needs for different use cases
- OneShot runs in background and can be used at the same time as all other Bot functions
- Interval order(s) run also in background and are full-configurable over a config file
- Info section to get algorithms specifications and own NiceHash BTC wallet address
- Fast ability to manual control a order with speed/cancel buttons in main window
- Option to filter orders for specific bot instance regarding algo and/or market
- Support for creating of order bunches, also with different price steps in percent
- Easy configurable and full control over timings for different Bot tasks
- Hidden settings can be changed over a config file (for experienced users)
- Optimized stability for 24/7 continuous operation, error log and auto-restart
- Console window showing clean colorized tasks, important events and failures
