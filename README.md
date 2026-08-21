# Physics Car Game (Unity / WebGL)

A 2.5D arcade car race with custom physics, built with a focus on WebGL performance.

## Tech Stack
* **Engine:** Unity 2023+ (WebGL-optimized)
* **Architecture / DI:** VContainer
* **Asynchrony:** UniTask & Native Awaitable
* **Asset Management:** Addressables

## Core Physics Features
* **Custom Raycast Suspension:** Manual calculations of spring and damper forces via `Physics.Raycast`.
* **Linked Wheel Axes:** Synchronized grounding logic for paired wheels to improve vehicle stability.
* **Physical Wheel Detachment:** Zero-allocation separation and physical explosion of wheels upon critical roof-impact.

## Local Setup
1. Clone the repository.
2. Open the project.
3. Build Addressables Groups (Default Build Script).
4. Open BootScene (must be index 0 in Build Settings) and press **Play**.