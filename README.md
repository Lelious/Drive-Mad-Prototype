# Physics Car Game (Unity / WebGL)

A 2.5D arcade car race with custom physics, built with a focus on WebGL performance.

** [Play the game on Itch.io (Private Access)](https://lelious.itch.io/drive-mad-prototype?secret=6pFIB3NlelIXMEWsICg1Yuic7g)**

## Tech Stack

* **Engine:** Unity 6.3 LTS (6000.3.13f1)
* **Architecture / DI:** VContainer
* **Asynchrony:** UniTask & Native Awaitable
* **Asset Management:** Addressables

## Core Physics Features

* **Custom Raycast Suspension:** Manual calculations of spring and damper forces via Physics.Raycast.
* **Linked Wheel Axes:** Synchronized grounding logic for paired wheels to improve vehicle stability.
* **Physical Wheel Detachment:** Zero-allocation separation and physical explosion of wheels upon critical roof-impact.

## Local Setup

1. Clone the repository.
2. Open the project.
3. Build Addressables Groups (Default Build Script).
4. Open BootScene (must be index 0 in Build Settings) and press **Play**.

## Controls

* <kbd>A</kbd> / <kbd>Left Arrow</kbd> - Move backward.
* <kbd>D</kbd> / <kbd>Right Arrow</kbd> - Move forward.
* <kbd>R</kbd> - Restart game.
* **On-Screen UI** - Alternative touch controls for mobile and WebGL players.
