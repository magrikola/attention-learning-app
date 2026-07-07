\# Real-Time Attention Adaptive Mobile Learning App for Children Using Computer Vision



\## Project Description



This project is a Unity-based mobile learning application designed for children. The application includes four attention-based mini-games and a parent panel that displays daily and weekly performance reports.



The main purpose of the project is to support attention monitoring during gameplay. The system uses attention states such as \*\*Focused\*\* and \*\*Not Focused\*\* to control the scoring logic in the games. The application also collects gameplay data and presents the results in the parent panel.



\## Team Members



\* Merve Çiftçi

\* Emirhan Türk



\## Unity Version



Unity 2022.3.62f3



Please open the project with Unity 2022.3.62f3 or a compatible Unity 2022 LTS version.



\## How to Open the Project



1\. Download the zip file.

2\. Extract the zip file to a folder.

3\. Open Unity Hub.

4\. Click \*\*Add\*\* or \*\*Open project from disk\*\*.

5\. Select the extracted project folder.

6\. Wait for Unity to import the project files.

&#x20;  The first opening may take some time because Unity will recreate the Library folder.

7\. Open the main scene from:



```text

Assets/Scenes/MainMenu.unity

```



8\. Press the \*\*Play\*\* button in the Unity Editor.



\## Main Scenes



The project contains the following main scenes:



```text

MainMenu

GameSelection

Game1

Game2

Game3

Game4

ParentPanel

```



The application starts from the \*\*MainMenu\*\* scene.



\## How to Run the Application in Unity Editor



1\. Open the project in Unity.

2\. Open the \*\*MainMenu\*\* scene.

3\. Press the \*\*Play\*\* button.

4\. Use the main menu to navigate between the game selection screen, mini-games, and parent panel.



\## How to Build for Android



1\. Open the project in Unity.

2\. Go to:



```text

File > Build Settings

```



3\. Select \*\*Android\*\* as the target platform.

4\. Click \*\*Switch Platform\*\* if Android is not already selected.

5\. Make sure the required scenes are added to \*\*Scenes In Build\*\*.

6\. Click \*\*Build\*\* or \*\*Build and Run\*\*.

7\. Install the generated APK file on an Android device.



\## Required Packages



This project uses Unity packages including:



\* TextMeshPro

\* AR Foundation

\* ARCore XR Plugin



If Unity asks to restore or install packages, please allow Unity to complete the package restoration process through Package Manager.



\## Important Notes



\* The following folders are not included in the zip file because Unity automatically recreates them:



```text

Library

Temp

Obj

Logs

Build

Builds

.vs

```



\* The first project opening may take a few minutes.

\* If Unity asks to reimport assets or restore packages, please allow the process to finish.

\* The parent panel displays daily and weekly reports based on the gameplay data collected during the mini-games.

\* The application should be started from the \*\*MainMenu\*\* scene.



\## Recommended Project Folder Structure



After extracting the zip file, the project folder should include:



```text

Assets

Packages

ProjectSettings

README.md

```



These folders are enough to open and run the Unity project.



