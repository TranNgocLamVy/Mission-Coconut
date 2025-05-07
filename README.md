# Mission Coconut

## 🎮 Decription
Mission Coconut is a multiplayer first-person shooter (FPS) based on a humorous myth from the game development community called ”load-bearing coconut”. Set in a chaotic maze overrun by zombies and parasites, players must work together or compete to find a mysterious coconut—the key objective of the game.

## Screenshots
![Game Screenshot](./Screenshots/Screenshot%202025-05-07%20020633.png)
![Game Screenshot](./Screenshots/Screenshot%202025-05-07%20020713.png)
![Game Screenshot](./Screenshots/Screenshot%202025-05-07%20020734.png)
![Game Screenshot](./Screenshots/Screenshot%202025-05-07%20020833.png)
![Game Screenshot](./Screenshots/Screenshot%202025-05-07%20020843.png)
![Game Screenshot](./Screenshots/Screenshot%202025-05-07%20020857.png)
![Game Screenshot](./Screenshots/Screenshot%202025-05-07%20020909.png)
![Game Screenshot](./Screenshots/Screenshot%202025-05-07%20020943.png)
![Game Screenshot](./Screenshots/Screenshot%202025-05-07%20021021.png)
![Game Screenshot](./Screenshots/Screenshot%202025-05-07%20021029.png)
![Game Screenshot](./Screenshots/Screenshot%202025-05-07%20021100.png)
![Game Screenshot](./Screenshots/Screenshot%202025-05-07%20021213.png)
![Game Screenshot](./Screenshots/Screenshot%202025-05-07%20021242.png)


## 🛠 Tech Stack
- **Game Engine**: Unity Engine
- **Language**: C#

## ✅ Features

### 🎮 Core Gameplay Features
- **First Person Shooter**
- **Multiplayer (COOP)** - Using PhotonPun framework
- **Attack System** - Gun with multiple mode (Single, Burst, Auto) and accurate reload system.
- **Item collection** - Player can pick up:
  - Ammo: Gain 30 ammo
  - First aid: Gain 1 first aid (can be use to heal 30hp)
- **Player health system**

### 🧠 AI & Systems
- **Enemy AI** - Built with NavMeshAgent for movement and Finite State Machine (FSM) for state management contain these behaviours: Idle, Roaming, Chasing, Attack, Die.
- **Enemy Detection** - Enemy can detect player when player stand **infront** of enemy's view or when player stand too close to them. Enemy can also detect player when player attack them or other enemy near them.

### Visuals & Audio
- **Seperate Camera** - 1 for world object (other player, enemy, surrounding, ...) and 1 for UI (HP, Ammo and player hand in first person view)
- **Player animation** - Player can view other player animation like moving, jumping and shooting.
- **Enemy animation** - Enemy also have some animation: walk, run, attack and die.
- **3D sound effect** - Player can hear different sound effect depend on where the audio source are (left, right, close, far, ...). Those sound effect include: Gun, reload, footstep and some enemy sound.

## 🎮 Control
- **Movement** - `A / S / W / D`
- **Shoot** - `Left mouse`
- **Change gun mode** - `Q`
- **Reload** - `R`
- **Pick up item** - `F`
- **Use item** - `E`

## License  
This project is released under the **MIT License**. Feel free to modify and distribute it.  