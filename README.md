# 3D Maze Game

A **3D first-person maze navigation game** built in Unity with C#, featuring momentum-based movement, multiple hazard systems, AI enemies, and a persistent level select system.

![Unity](https://img.shields.io/badge/Unity-2022.3-black?logo=unity)
![C#](https://img.shields.io/badge/C%23-.NET-purple?logo=csharp)
![Platform](https://img.shields.io/badge/Platform-Windows%20%7C%20Mac-blue)

---

## Gameplay Features

- **First-Person Movement** — Momentum-based player movement with sprint mechanics
- **Camera-Relative Controls** — Smooth, intuitive directional movement using CharacterController
- **Multiple Hazard Systems** — Navigate traps, turrets, and AI enemies
- **Level Select System** — Persistent level selection with overhead camera preview
- **Death & Win UI** — Full game over and victory screen flow with respawn logic

---

## Hazard Systems

### 1. Spike Traps
- Animated using a **state machine** with Lerp-based movement
- Timed activation patterns to challenge player timing

### 2. Turret Shooter
- Fires **physics-driven projectiles** at the player
- Rotates to track player position

### 3. Wandering AI Enemies
- **Region-bounded wandering** — enemies patrol set areas
- Switches to chase behavior when player is detected

---

## Tech Stack

| Tool | Purpose |
|------|---------|
| Unity 2022.3 | Game engine |
| C# | Game logic & state machines |
| NavMesh AI | Enemy pathfinding |
| CharacterController | Player movement physics |
| TextMesh Pro | UI elements |

---

## How to Play

1. Download the latest release from the [Releases](#) page
2. Extract the zip file
3. Run `MazeGame.exe` (Windows) or open the `.app` (Mac)
4. Select a level from the **Level Select** screen
5. Navigate through the maze, avoid hazards, and reach the exit!

**Controls:**
| Key | Action |
|-----|--------|
| WASD | Move |
| Shift | Sprint |
| Mouse | Look around |

---

## About the Developer

Built by **Ansaf Khan** — Software Developer with 3+ years of experience in .NET/C#, currently completing an MS in Applied Computer Science at Fairleigh Dickinson University, Vancouver.

- 🌐 [Portfolio](https://github.com/Ansafkhan)
- 💼 [LinkedIn](https://www.linkedin.com/in/ansaf-khan/)
