# MultiMatrix--Puzzle

*A dynamic tile-sliding puzzle game built in C# with multiple difficulty levels and tree-based move tracking!*

---

## 🧩 About the Game

**MultiMatrix Puzzle** is a modern take on the classic 15-puzzle game — but with a twist! Choose from **three difficulty levels**:

* 🟢 Easy: 2×2 Grid
* 🟡 Medium: 3×3 Grid
* 🔴 Hard: 4×4 Grid

Your goal is simple: slide the tiles into order with the fewest moves possible. But behind the scenes, every move is being tracked in a **tree data structure** — enabling smart **Undo/Redo** functionality.

---

## ⚙️ Features

✨ **Multiple Grid Sizes** – 2x2, 3x3, and 4x4 boards
🎛️ **Difficulty Selection** – Pick your challenge level
👤 **Player Name Input** – Personalized gameplay
⏱️ **Timer + Move Counter** – Track your performance
🔄 **Undo / Redo** – Tree-based navigation of move history
📂 **Score Logging** – Saves your win stats to `scores.txt`

---

## 🖥️ Technologies Used

* 🧠 **C# (Windows Forms)**
* 🌳 **Tree Data Structure** (for undo/redo history)
* 🎨 **WinForms UI** (custom buttons, labels, and layout)

---

## 🕹️ How to Play

1. **Launch the app** – The game opens with a level selector.
2. **Enter your name** – Personalize your session.
3. **Slide the tiles** – Use mouse clicks to move adjacent tiles.
4. **Undo/Redo moves** – Explore past and future states.
5. **Win the game** – Arrange the numbers in order and beat your best time!

---


## 🗂️ File Structure Highlights

```plaintext
📁 _15_Puzzle_Game_Tree
 ├─ Program.cs        → App entry point and level selection
 ├─ LevelForm.cs      → UI for difficulty selection
 ├─ NameInputForm.cs  → UI for player name input
 ├─ PuzzleForm.cs     → Main puzzle logic, UI, and tree-based state tracking
 ├─ TreeNode.cs       → Custom tree node for undo/redo
 └─ scores.txt        → Auto-generated log of game results
```

---

## 📥 Installation & Run

1. Clone this repo:

   ```bash
   git clone https://github.com/yourusername/MultiMatrix-Puzzle.git
   ```
2. Open in **Visual Studio**
3. Run the project (`F5`)

---

## 💡 Future Ideas

* 🧠 AI Solver (using A\* or BFS)
* 🌐 Online leaderboard
* 🎨 Custom themes or tile images
* 📱 Mobile version (Xamarin/MAUI)

---

## 🏁 Contribute or Fork

Feel free to open issues, suggest features, or fork the project to customize!

---

## 🧑‍💻 Author

Built with 💙 by Syed Ali Hassan & Muhammad Ahmad
If you use this for learning, give the repo a ⭐!


