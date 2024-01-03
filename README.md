# Keypad-Editor
Keypad-Editor is a program that uses to customization Keypad.
## What is Keypad
Keypad is my own device. Main goal of this is helping users to work on the computer and optimizing their work. It requires 2 programs. The first one is editor for device (you are here) and the second one is driver.
### Idea
The idea of creating this device was born in November of 2022. It's going to be a project for business contest "Techno-leaders of the future" (Техно-лидеры будущего). I was inspired by the Stream deck. And wanted to make cheaper variant of this.
### Functionality
Device can:
- open files, folders in explorer and web sites
- type text
- press combination of keys as a keyboard
### Upgrading
In the future I'm going to add new functionality:
- Group system. User may want to set some actions to device but it has limited number of buttons. This feature can help to solve it. User can set to a key opening new group. After pressing that key device will change its configuration and user will be able to set actions to a "clear" group. It also will help to sort users actions.
- Scripting system. It must to make device more useful. User will be able to write his own custom actions based on actions that are already in Keypad.
- Interaction with other programs via API. I'm going to add interaction with some famous program like an OBS studio and other.
## Technologies
It was written in C# in .net 8 with using WPF.
## Building project
To build the project you must to have .net 8 installed on your computer.
To do this, open in console folder with `Keypad-Editor.csproject` file and then write `dotnet build`. After that you'll get an exe file in the `bin` folder.
However you need to put there `data` folder which includes settings and configuration file. You shouldn't change something there manually.
## How to use
Window is divided into 3 parts. Left side is using to choose a key which you are going to customize. The middle part is using to choose the action that device will execute. The right part has some controls that are used in the setting selected action. After clicking to the apply button your settings will be saved to the file.