# Water puzzle game 2D

This is a puzzle game in which you need to draw the lines in a way that will direct them toward the goal, the blue hexagonal tile.

There is about 40 puzzles for now.

To play it, I've made a [Github Pages repository](https://github.com/isirode/WaterPuzzleGame2DDemo), you can play it [here](https://isirode.github.io/WaterPuzzleGame2DDemo).

## Running the project

You can open the repository from the Unity hub, once you pulled it.

It will indicate a warning, it is because Mirror is imported as an asset, but I excluded it from Git so that it does not pollute the project.

In the project, you will have errors, ignore the ones about Mirror.

Import it as an Asset : Version 2022.10.0 - November 10, 2022

### Dependencies

* [mob-sakai/GitDependencyResolverForUnity](https://github.com/mob-sakai/GitDependencyResolverForUnity)
  * It what I use to resolve Git sub-dependencies
  * It might have an infinite loop dependency import problem (I had it, I did not manage to reproduce it, it is reported on the issue tracker of it)
* Mirror
* LiquidSimulationExtension
  * Liquid-Simulation
* Unity2DControllers
* DrawingLines2D

## Known issues

Do not use the templates : this reset the tags, the layers as well. Furthermore, it duplicates / make prebab variants of the used prefabs, but in the scene, it will redirect you to the variants.
That can be difficult to see that you have not modifie the main prebab, causing unexpected bugs.
Duplicate the scene instead.

Do not set the layer of water particle prebabs, it seem to make the shader water effect to bug.

Some of the scenes could be bugged.

~~There is a water bug, where the shader is not applied anymore, in some scenes, it was fixed in the later scenes, but it still need to be done in the first scenes.~~ It should not be the case anymore, but I will let it there in case I see it again.

The water shader is displaying the outline of the water particles (a circle), I can fix it but then, some of the lines are not displayed anymore.

The UI is in a very beta phase.

The "back" button does not seem to be working in some scenes.

It is possible to draw multiple times, but it should not be.

## Partitipating

Open the [DEVELOPER.md](./DEVELOPER.md) section.
