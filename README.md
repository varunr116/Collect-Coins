# Collect-Coins

Setup Steps

Prerequisites

Unity: Unity 6.0 (6000.0.49f1) with Android Build Support installed.

Android SDK/NDK: Properly configured in Unity’s Preferences → External Tools.

TextMeshPro: Automatically included; no extra setup required.

Git (optional): For version control and repository management.

Clone & Import

Clone or download the project repository.

Open Unity Hub → Click “Add” → Navigate to the project folder → Open.

Wait for Unity to import all assets, scenes, and scripts.

Configure Build Settings

Go to File → Build Settings.

Switch Platform to Android.

Ensure the following scenes (in order) are added:

Bootstrap.unity (persistent managers)

Entry.unity (welcome/login)

Register.unity (user registration)

Login.unity (authentication)

MainMenu.unity (navigation hub)

GameScene.unity (coin tap game)

Android Player Settings

Open Edit → Project Settings → Player.

Under Android tab:

Set Company Name and Product Name.

Define Package Name (e.g., com.yourcompany.cointapgame).

Minimum API Level: Android 7.0 (API 24).

Target API Level: Automatic (highest installed).

Project Overview
This Unity application combines a professional Figma-style prototype flow with a coin tap mini-game. It emphasizes scalable architecture, core functionality, animations, and adherence to SOLID and OOP principles. The app targets Android devices (minimum API 24) and is built with Unity 2021.3+.

Key highlights:

Game Architecture & Core Functionality: Modular singleton-based system (persistent managers, event-driven updates, object pooling for coins).

Animations & UI Transitions: Smooth scene and UI animations using Unity’s default UI components (no external sprites or images).

SOLID & OOP Concepts: Separation of concerns across managers (GameManager, UIManager, AuthManager, etc.), clear interface boundaries, dependency inversion (via scriptable configuration), and extensibility hooks.

Authentication & Local Data Storage: New-user registration with phone/password validation, storing credentials in a local users.json file under Unity’s persistent data path.

Professional UX: Figma-inspired transitions, animated feedback (success, error, button interactions), and mobile-first optimizations (Android back button, haptics).

Build Instructions
Development Build
Pre-Build Checklist

Verify all scenes are listed in Build Settings in the correct sequence.

Ensure Android platform is selected and settings are configured.

Confirm persistent managers are properly assigned in the Bootstrap scene.

Double-check that audio, config assets, and prefabs are referenced correctly.

Creating the Development APK

Confirm:

App launches to the Entry screen.

User registration, login flow, and JSON storage work as intended.

Scene transitions and animations are smooth.

Coin tap gameplay (30-second timer, coin spawning, scoring) functions correctly.

Pause system works without freezing the timer.

Audio (BGM/SFX) plays and crossfades appropriately.

Android back button behaves contextually (exit confirmation on main menu, returns to menu from game).

JSON file (users.json) is created under Application.persistentDataPath and persists across app restarts.

All animations, transitions, and audio behaviors remain intact.
