
# Opewin - Windows desktop application that can control other application's placement on your desktop via HotKey

Opewin is a Windows desktop application that can control other application's placement on your desktop via HotKey.
You can asign the HotKey and set the operations(we call that "Ope") executed when you put thier corresponding HotKey. 
Each Ope can set by writing our DSL(we call that "OpeScript"). 
<br/>
<br/>

# Settings
- To open the setting window, double click notification area icon following:<br/>
![icon](img/notification_area_icon.png)
<br/>
<br/>

# About OpeScript
In this section, we describe the definition of OpeScript.<br/>
By using them, you can control other App's window flexibly!
<br/>
<br/>

## Valuables
|Valuable |Description |
|---|---|
|Count |Indicate how many times the same hot key was inputted repeatedly.<br/>Note that this value was set as "0" when you input any hot key first time and incremented from the second time.|
<br/>
<br/>

## Functions
|Function |Description |
|---|---|
|Maximize()                              |Maximize current active window. |
|Minimize()                              |Minize current active window.  |
|Restore()                               |Restore current active window.  |
|MoveTo(double rate_x, double rate_y)    |Move the current active window to the position specified by arguments.|
|ResizeTo(double rate_x, double rate_y)  |Resize the current active window to the size specified by arguments.|
|MoveBy(double rate_x, double rate_y)    |Not implemented yet...|
|ResizeBy(double rate_x, double rate_y)  |Not implemented yet...|
|ChangeMonitorFw()                       |Move the current active window to next monitor.|
|ChangeMonitorBw()                       |Move the current active window to previous monitor.|
|ResetCount()                            |Reset the repeat counter.|
