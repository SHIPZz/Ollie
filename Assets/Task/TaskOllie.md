Task: Add system for holiday events into the game (empty unity project)  
System for holiday events should adapt if an input config changes.  
Time given: 48 hours since you have received this document.

Here is an example config (json), which game must use (use it):

| {    "holidaySchedule":	\[		{			"startDate": "2025-10-25T11:59:00.000Z",			"endDate": "2025-11-10T11:59:00.000Z",			"holidayType": "halloween"		},		{			"startDate": "2025-12-01T11:59:00.000Z",			"endDate": "2025-12-26T11:59:00.000Z",			"holidayType": "xmas"		}	\]} |
| :---- |

Requirements:

* Config must load from the project (config must be within Assets or Assets/Resources folder \- your choice)  
* All dates within config are always in ISO 8601 format.  
* Game must use UTC device time for date checking.  
* There also should be ability to test if holidays change in-game (debug tool to shift time).  
* You need to only implement “xmas” and “halloween” for the game  
* “xmas” (only while “xmas” active) must display a text within top of the screen “XMAS”  
* “halloween” (only while “halloween” active) must display text within bottom of the screen “HALLOWEEN”  
* It should be possible for multiple holidays active at the same time.  
* If config is empty or not present at all \- game should still work (no exceptions thrown)  
* If structure of a config is incorrect \- game should still work (no exception thrown)  
* Basically if anything goes wrong \- game should still work  
* Debug tool should be visible within development build and editor  
* Debug tool should also display an active holiday currently  
* No need to add your own settings to a config  
* Recommended unity version is 2021.3.38f1

All changes must pushed to your public repository and you also should build and share an Android APK build

Your work will be judged by:

* Readability of your code  
* Presense of the build  
* Presense of bugs (the less \- the better)  
* Completion percent of all requirements  
* How well your code works around edge and negative cases