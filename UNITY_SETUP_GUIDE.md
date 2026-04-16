# Unity 6 Meta Quest Setup Guide

You made the right call switching to Unity 6! The WebXR PWA pipeline is notoriously frustrating, whereas Unity is natively built for Meta Quest. This means your `.apk` will build flawlessly and run at much higher performance.

Since Unity is a visual editor that relies on complex `.scene` and `.meta` files that the editor generates automatically, the cleanest way to do this is to open Unity, pick their official VR template, and drop the scripts I just generated for you into it.

Here is the exact step-by-step guide to get your APK in under 10 minutes:

### Step 1: Create the Project
1. Open **Unity Hub**.
2. Click **New project** in the top right.
3. At the top, ensure your Editor Version is set to **Unity 6** (e.g., `6000.0.x`).
4. On the left menu, select **VR**.
5. Select the **VR Core** template (this automatically configures Meta Quest plugins, Android build settings, and the XR Interaction Toolkit).
6. Name your project (e.g., `TowerDefenseVR`) and click **Create project**.

### Step 2: Add the Scripts
1. Once the Unity Editor opens, look at the **Project** window at the bottom.
2. Open the **Assets** folder, then open (or create) a **Scripts** folder.
3. Drag and drop all the `.cs` files from this repository's `Assets/Scripts` folder into that Unity window.

### Step 3: Setup the Scene
1. The VR Core template gives you a floor and an XR Rig (the player) automatically.
2. **Create the Core:**
   - Right-click in the **Hierarchy** (top left) -> `3D Object` -> `Sphere`. Name it `Core`.
   - Set its position to `(0, 1, 0)`.
   - Drag the `CoreManager.cs` script onto it.
3. **Create the Game Manager:**
   - Right-click in the **Hierarchy** -> `Create Empty`. Name it `GameManager`.
   - Drag the `GameManager.cs` script onto it.
   - In the Inspector, drag the `Core` object you made into the `Core Manager` slot on the script.

4. **Setup the Audio:**
   - Drag your `music.mp3` file from the `WebXR_Backup` folder into your Unity `Assets` window.
   - Select the `GameManager` object in your Hierarchy.
   - In the Inspector, click `Add Component` and search for `Audio Source`.
   - Drag your imported `music.mp3` file into the `AudioClip` slot of the Audio Source.
   - Check the `Play On Awake` and `Loop` checkboxes.
5. **Create the Path Prefab:**
   - Right-click in the **Hierarchy** -> `3D Object` -> `Cube`. Name it `PathVisual`.
   - Set its scale to `(1, 0.1, 1)` so it lays flat like a sci-fi walkway.
   - Create a new Material, set its color to Cyan, enable Emission, and assign it to the Cube.
   - Drag the `PathVisual` down into your **Project** window to make it a Prefab.
   - Delete `PathVisual` from the scene.
   - Select the `GameManager` and drag your new `PathVisual` prefab into the `Path Prefab` slot on the script.

6. **Create the Floor/Tower Placement:**
   - Select the `Plane` (or whatever floor the template gave you).
   - In the top right of the Inspector, click the **Layer** dropdown and select `Add Layer...`. Type `Floor` into an empty slot.
   - Select your Plane again and assign its Layer to your new `Floor` layer.
   - Find your **Right Controller** in the Hierarchy (usually under `XR Origin` -> `Camera Offset`).
   - Drag the `TowerPlacement.cs` script onto the Right Controller.
   - In the Tower Placement script inspector, change the `Placeable Layer` dropdown to `Floor`.
5. **Create the Tower Prefab:**
   - Create a Cylinder in the scene. Name it `Tower`.
   - Drag the `Tower.cs` script onto it.
   - Drag the Cylinder from the Hierarchy down into your Project window to make it a blue **Prefab**.
   - Delete the Cylinder from the scene.
   - Select your XR Controller again, and drag your new Tower prefab into the `Tower Prefab` slot on the `TowerPlacement` script.

### Step 4: Build the APK!
1. Before building, press **Ctrl + S** (or Cmd + S) to save your Scene. Give it a name like `GameScene`.
2. Go to **File** -> **Build Profiles** (or **Build Settings** in older versions).
3. Select **Android** on the left.
4. Very Important: Click the **Add Open Scenes** button at the top of that window! If you don't do this, your APK will build an empty void.
5. Make sure your Meta Quest is plugged into your PC, and select it from the "Run Device" dropdown.
6. Click **Build and Run**!

Unity will now compile the scripts, generate the `.apk`, and automatically push it directly into your Meta Quest headset! You can find it under your "Unknown Sources" tab in the App Library.