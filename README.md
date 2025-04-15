# Startup Surge (Unity 2D)
An addictive 2D idle/tycoon-style game where players hire employees, upgrade systems, and earn passive income — all with a smooth UI, engaging mechanics, and rewarding progression.

## 🎮 Features
- 💰 Click-to-Earn Mechanic with Upgrade System
- 🧍‍♂️ Employee Hiring with Two Spawn Zones (A & B)
- 💼 15 Unique Employee Positions with Smart Spawning
- 📈 Passive Income System based on Employee Count
- 🎥 Dynamic Camera Movement to Spawned Employees
- 📜 Main Menu and Credits Scene
- 🧠 End Game Condition (after 15 employees hired)
- 🔊 Background Music (persists across scenes)
- 💬 Pop-Up Text Animations for Feedback
- 🎨 Simple & Clean UI using TextMeshPro

## 🛠️ Technologies Used
- Unity Engine (2D)
- C#
- TextMeshPro
- AudioSource System
- Unity UI System

## 🎬 Scenes Included
- **Main Menu:** Game title, Start, Continue, Credits, Exit
- **Game Scene:** Core gameplay with UI and hiring system
- **Credits Scene:** Developer credits + Exit button
- **End Game Panel:** Displays message and button when game is completed

## 📁 Key Scripts
- `GameManager.cs`: Core game logic (spawning, cash, upgrades)
- `GameEnd.cs`: Handles end-of-game logic
- `StartGameButton.cs`, `CreditButton.cs`, `ExitButton.cs`: Menu Navigation
- `CoinPopup.cs`, `PopupText.cs`: Visual feedback systems
- `BackgroundMusic.cs`: Persistent music across scenes
- `AutoScroll.cs`: Optional UI enhancements
- `EmployeeCoinSpawner.cs`: Handles coin spawns from employees

## 🧠 Game Logic Highlights
- **Smart Employee Spawning:** 15 positions across two zones with prefab assignment based on spawn area.
- **Prefab Diversity:** Each spawn position uses a specific prefab, adding variety.
- **Progressive Costing:** Hiring and upgrades become more expensive over time, encouraging planning.
- **Camera Follow:** Smooth camera pans to new hires and then resets.

## 📸 Preview
[![IMAGE ALT TEXT HERE](https://img.youtube.com/vi/C2qGslOkIuM/0.jpg)](https://www.youtube.com/watch?v=C2qGslOkIuM)

---

🧑‍💻 Developed by: Deep Darji
