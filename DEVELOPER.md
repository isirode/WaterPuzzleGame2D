# Developer

## Contributing

You can open a PR if there is a feature you need or a bug to fix.

There is a list of features below which could be interesting.

## Features

- [ ] UI
  - [ ] Main screen
    - [ ] Put a drawing area to the right
  - [ ] Area to display messages
  - [ ] Better layout (bottom etc)

- [ ] Levels
  - [x] Levels with distance limiter
  - [ ] Levels with length limiter
  - [ ] Levels with multiple lines 

- [ ] Utilities
  - [ ] Move them in another repository when appropriate
  - [ ] Find a MonoBehaviour in scene, assign it

Need to find a way to update multiple scenes easily.
- Add them to the build
- Update / replace a GameObject inside

Level management:
- Could have a bunch of prebabs which are load consecutively
- Could have scenes containing only the environments

Level system should be distinct that of the scenes.
Could use a asset system.

Fixme : the next level system for the normal scenes level was made promptly -> it required to be done properly

## TODO

- [ ] Use the LevelSystem library
- [ ] Fix the Mirror issue
  - [ ] Put the Mirror example in a separate project
  - [ ] Fork Mirror, export it as a Git dependency (not currently possible since there is not package.json file as the root)

## Bug fixing

- [ ] Lines are not displaying correctly when the water is displayed correctly
- [ ] Fix the links of the dependencies in the README.md
- [ ] Give appropriate credits
