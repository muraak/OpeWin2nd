
# Opewin - Windows desktop application that can control other application's placement on your desktop via HotKey

Opewin is a Windows desktop application that can control other application's placement on your desktop via HotKey.
You can asign the HotKey and set the operations(we call that "Ope") executed when you put thier corresponding HotKey. 
Each Ope can set by writing our DSL(we call that "OpeScript"). 


# Adding Ope
	1. Right click OpeWin icon on your task tray. 
	2. Click "setting" menu.
	3. Click "add" button on the OpeWin setting window.

# Register hotkey
	1. Move the cursor to line you want to set.
	2. Press keys you want to set as hotkey.

# Unregister hotkey
	1. Move the cursor to line you want to set.
	2. Press esc key.

# Settings

# Definition of OpeScript
## Valuables
|Valuable |Description |
|---|---|
|Count |Indicate count of times inputing same hot key repeatedly. |
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
