#Dodo Runner

**Dodo Runner** is a mobile **endless runner game built in Unity**, developed with a strong focus on **clean architecture, scalable systems, and mobile performance**.

This project was created as part of my **Build in Public** journey to demonstrate **real-world Unity engineering practices**, not just gameplay features.

---

## 🎮 Game Overview

Dodo Runner is a lane-based endless runner where the player avoids obstacles, collects coins, and uses powerups to survive longer as the game speed and difficulty gradually increase.

The primary goal of this project was **system design and maintainable architecture**, not cloning an existing game.

---

## ✨ Gameplay Features

- Swipe-based controls:
  - **Left / Right** → Change lanes
  - **Up** → Jump
  - **Down** → Slide
- Procedurally generated platforms & obstacles
- Time-based difficulty ramp (speed & obstacle probability)
- Coin collection & score multiplier system
- Powerups:
  - 🧲 Magnet
  - 🛡 Shield
  - ✖ Double Score
- First-time guided tutorial system
- Pause, Resume, Restart & Game Over flow

---

## 🧠 Technical Architecture

The project is structured using a **service-driven architecture** with clear separation of responsibilities.

### 🔹 Core Systems
- **GameService** – Application-level orchestration
- **GameLoop** – Central update & fixed-update flow
- **PlayerService** – Player lifecycle, input & movement
- **PlatformService** – Platform pooling & procedural spawning
- **ObstacleService** – Obstacle loading, pooling & balancing
- **CoinService** – Coin pooling, magnet logic & scoring
- **PowerupService** – Powerup lifecycle & timers
- **TutorialService** – First-time user guidance & input gating
- **ScoreService** – Distance score, coins & multipliers

All systems communicate through a **custom event-driven layer**, avoiding tight coupling.

---

## 🏗 Design Patterns Used

- **State Machine**
  - Player movement and action states
- **Observer / Event System**
  - UI updates, scoring, powerups
- **Object Pooling**
  - Platforms, obstacles, coins, powerups
- **Service Layer**
  - Eliminates God MonoBehaviours
- **MVC-style separation**
  - Controller / View / Data
- **Singleton**
  - Core services
- **SOLID Principles**
  - Maintainable and extensible codebase

---

## ⚙ Performance & Optimization

- **Unity Addressables**
  - Async asset loading
  - Memory-safe content management
- Mobile-first pooling strategy
- Minimal runtime allocations
- Decoupled UI and gameplay logic
- Time-based difficulty scaling (not frame-based)

Designed to run smoothly on mobile devices.

---

## 🧪 Tutorial System (First Run Only)

- Step-by-step guided tutorial
- Input gating per tutorial stage
- Controlled obstacle & coin spawning
- Automatically disabled after completion
- Progress saved using `PlayerPrefs`

Ensures player learning without affecting long-term gameplay flow.

---

## 📱 Platform

- **Android (APK)**
- Built using **Unity & C#**
- Touch input (mobile-first)

---

## 🚀 Getting Started (For Reviewers)

1. Clone the repository
2. Open the project in **Unity Hub**
3. Load the Gameplay scene
4. Press Play or build for Android

> Note: Unity Addressables must be properly configured before running the project.

---

## ▶️ Gameplay Video

🎥 **Watch full gameplay here:**  
👉 [Gameplay_Trailer](https://youtu.be/ffU62zzC-XU)

The video demonstrates:
- Responsive swipe controls
- Difficulty ramping over time
- Powerups in action
- Clean death & game-over flow
- Overall system stability and performance

---

## 📦 Play the Game

🎮 **itch.io (Recommended):**  
👉 https://dip-2332.itch.io/dodorun

📱 **Android APK (Google Drive):**  
👉 https://drive.google.com/file/d/1SRWtW0amS6LzUE5yRmR3Erpqvpy5uNvi/view

---

## 🎯 Project Goals

- Demonstrate scalable Unity architecture
- Apply real-world design patterns
- Build mobile-optimized gameplay systems
- Write clean, readable, maintainable code
- Avoid tightly coupled or tutorial-style implementations

---

## 🙌 Feedback

This repository is shared for **learning, review, and feedback**.  
If you’re a Unity developer or reviewer, I’d genuinely appreciate suggestions or critiques.

⭐ If you find the architecture useful, feel free to star the repo.

