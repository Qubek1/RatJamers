# Rat Jamers

**Rat Jammers** is a local PvP game where you manage a team of rat developers racing to finish a game as fast as possible. Play mini-games to boost your own team or sabotage your opponent.

## Built With

*   **Game Engine:** [Unity](https://unity.com)
*   **Programming Language:** C#
*   **2D Graphics:** [Krita](https://krita.org/en/)

## Gameplay Preview

https://github.com/user-attachments/assets/83b477d1-3043-4e47-bb13-79965d6ee264

## Cables Minigame

In cables minigame first player starts by tangling the cables thus sabotaging enemy team. Then the sabotaged player need to fix it by untangling the cables.

<img width="350" alt="cables tangling" src="https://github.com/user-attachments/assets/7e04d921-e59c-41a5-9299-8f974a6f94c3" />
<img width="350" alt="cables untangling" src="https://github.com/user-attachments/assets/0c62a101-20f2-40e6-80af-1f2ebcb374de" />

### Implementation

[CablesMinigameController.cs](Assets/Minigames/Cables/CablesMinigameController.cs)

Cables are a collection of gameObjects with HingeJoint2D, that are connected together in a chain. The Visualization is created with LineRenderer.

<img height="300" alt="cables hinge joints 2D" src="https://github.com/user-attachments/assets/8ef34c0f-fdcc-4d00-a3ce-7cce2b55ba89" />

### Overlaps

Overlaps are managed with [CablesOverlapsController.cs](Assets/Minigames/Cables/CablesOverlapsController.cs) script. 

*    **Finding Overlaps** - First we divide space into a grid, and assign every point in cables to the grid cells. Now we can quickly iterate through points that are close to each other. To check if consecutive points from one cable intersect the other, we use vector dot product. 
*    **Maintaining Overlaps between frames** - While position of an overlap might change between frames, we need to maintain which cable is on top of another. In order to do that we store all of the overlaps in a list, then in the next update we assign them to each other by distance.
*    **Rendering** - To show which cable is on top we create new LineRenderer that will render just a small fragment to cover the cable below.
<img  height="200" alt="cables overlap rendering" src="https://github.com/user-attachments/assets/14b41c49-e583-471d-9f04-c2c16ede73ba" />
