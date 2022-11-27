# Water puzzle game 2D

This is a puzzle game in which you need to draw the lines in a way that will direct them toward the goal, the blue hexagonal tile.

There is about 40 puzzles for now.

To play it, I've made a [Github Pages repository](https://github.com/isirode/WaterPuzzleGame2DDemo), [this is](https://isirode.github.io/WaterPuzzleGame2DDemo) the URL of the game.

## Running the project

You can open the repository from the Unity hub, once you pulled it.

### Dependencies

* Miror
* Liquid-Simulation
* LiquidSimulationExtension
* Unity2DControllers
* DrawingLines2D

## Known issues

Do not use the templates : this reset the tags, the layers as well. Furthermore, it duplicates / make prebab variants of the used prefabs, but in the scene, it will redirect you to the variants.
That can be difficult to see that you have not modifie the main prebab, causing unexpected bugs.
Duplicate the scene instead.

Do not set the layer of water particle prebabs, it seem to make the shader water effect to bug.

Some of the scenes could be bugged.

There is a water bug, where the shader is not applied anymore, in some scenes, it was fixed in the later scenes, but it still need to be done in the first scenes.

## Partitipating

Open the [DEVELOPER.md](./DEVELOPER.md) section.
