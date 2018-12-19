# Virtual Exhibitions

VR Exhibitions on Vive. A hackathon prototype created at the [4th Swiss Open Cultural Data Hackathon](http://make.opendata.ch/wiki/event:2018-10). Read more about our project on the [Opendata Wiki](http://make.opendata.ch/wiki/project:virtual_3d_exhibition). This repository is more for the technically inclined people.

## Vision

The goal is to create a customisable
virtual exhibition space with a focus on GLAM datasets.

### Core Features

 * Virtual 3D world with positioned art objects
 * freely movable character / player
 * Generic: Support for multiple / interchangeable exhibitions
 * Interface / communication between back-end (exhibition provider) and front-end (3D world)

### Optional Features

 * Exhibition creator: UI for creating exhibitions
 * Nation wide exhibitions: Main menu is map of nation and multiple exhibitions are located at their real-world location
 * Exhibition generator: Deeply, automagically created exhibitions based on parameters
 * Multimedia
 * VR

## Unity

**We use Unity 2017.4.2**

Set up your Unity and IDE as you wish.
IntelliJ Rider is recommended for everyone that uses IDEA / PyCharm on a regular basis.

**GIT & Unity**:
In Unity set meta files to be visible and textual via:

 * `Edit > Project Settings > Editor`
 1. Set `Version Control Mode` to `Visible Meta Files`
 2. Set `Asset Serialization Mode`to `Force Text`
 
These steps are taken from https://stackoverflow.com/a/18225479

## Project Structure

```
 /
	+ Assets/                     - All custom unity stuff in here
		|
		+ Fonts/                  - Fonts files, if custom fonts are used
		|
		+ Materials/              - Unity Materials, if materials are used
		|
		+ Modules/                - Per self-contained feature a module
		|
		+ Prefabs/                - Unity Prefabs, if if prefabs are used
		|
		+ Scenes/                 - Home of the scenes, e.g. 'Worlds'
		|
		+ Scripts/                - Home of the scripts, e.g. the logic
		|
		+ Sprites/                - Home of any sprites used
	|
	+ Library/                    - Libraries for unity to work
	|
	+ ProjectSettings/            - Self explanatory
	|
	+ UnityPackageManager/)       - Unity stuff
```
 
## Getting Started

To start working on the project

 1. Start Unity3d
 2. On the start screen click the `Open` button
 3. Select `VirtualExhibitions` from this repository
 
## Contributors

 * Ivan Giangreco
 * Ralph Gasser
 * Silvan Heller
 * Mahnaz Parian
 * Loris Sauter

