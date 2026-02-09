# Custom Plane AR Tracking

## Overview

This Unity project demonstrates an AR application using AR Foundation. It features custom plane detection and visualization, interactive object placement, and user interface controls for object manipulation.

## Features

### 1. AR Plane Detection & Visualization

- **Custom Visualization**: Detected planes are visualized with a custom material.
- **Decorations**: Supports optional FBX models as child objects on detected planes.
- **Script**: `CustomPlaneVisualizer.cs`

### 2. Object Spawning (Tap-to-Place)

- **Interaction**: Users can tap on a detected plane to spawn a 3D object.
- **Constraint**: The system enforces a limit of **one spawned object per session**.
- **Optimization**: AR Plane detection is automatically disabled after the object is spawned to save performance and battery.
- **Script**: `ObjectSpawner.cs`

### 3. User Interface

- **Welcome Screen**:
  - Appears on startup with instructions.
  - **Interactive Slider**: allows users to browse through usage instructions (non-button interaction).
  - **Start Button**: Hides the welcome screen and initializes AR tracking.
  - **Script**: `WelcomeUIController.cs`
- **Color Control**:
  - On-screen buttons (Red, Green, Blue, Yellow) allow real-time color changing of the spawned object.
  - Checks if an object exists before applying changes.
  - **Script**: `ColorController.cs`

### 4. Object Manipulation

- The application supports standard AR gestures:
  - **Pinch**: Scale the object.
  - **Twist**: Rotate the object.
    _(Managed by AR interaction components)_

## Project Structure

- **Assets/Scripts/AR/**
  - `CustomPlaneVisualizer.cs`: Handles plane visuals.
  - `ObjectSpawner.cs`: Manages raycasting and object instantiation.
  - `ObjectTransform.cs`: Handles object transformation logic.
- **Assets/Scripts/UI/**
  - `WelcomeUIController.cs`: Manages the welcome flow and instruction slider.
  - `ColorController.cs`: Handles color change buttons.

## How to Use

1.  **Launch the App**: You will see the Welcome Screen.
2.  **Read Instructions**: Drag the slider to read through the 5-step guide.
3.  **Start AR**: Tap the "Start" button. This enables the camera and plane detection.
4.  **Scan Surfaces**: Move your device to detect flat surfaces (floors/tables).
5.  **Place Object**: Tap on a visualized plane to place your object.
6.  **Customize**: Use the colored buttons to change the object's color.
7.  **Interact**: Use two fingers to pinch (scale) or twist (rotate) the object.
