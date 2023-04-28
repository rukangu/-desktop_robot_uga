# Articulated Robot in AGXUnity
This scene demonstrates the stability and robustness of AGXUnity when it comes to simulate articulated robots and grasping with frictional contacts.

It also demonstrates how AGXUnity supports articulated/hierarchical bodies.

This is done through a baseclass ArticulatedRigidBodyBase as the root of the robot arm. Each RigidBody has a flag: RelativeTransform==true. This allows for relative motion in the tree. 

## Input
This example uses the new InputManager in Unity which must be installed before loading this scene. It can be found in the Package manager.

## Keyboard control
The robot can be controlled using the Keyboard:

```
A/D - rotate base joint
S/W - rotate shoulder joint
Q/E - rotate elbow joint
O/P - rotate wrist1
K/L - rotate wrist2
N/M - rotate wrist3
V/B - rotate hand
X - close pincher
Z - open pincher
```

## Gamepad control
The best way of controlling the robot is using a Gamepad.

```
Left/Right trigger - open/close grasping device
Left/Right Shoulder - Rotate Hand
Left Horizontal - Rotate base
Left Vertical - Shoulder up/down
Right Horizontal - Wrist1
Right Vertical - Elbow
D-Pad horizontal - Wrist3
D-Pad vertical - Wrist2
```


