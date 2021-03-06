﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomInputSettings : MonoBehaviour
{
    //The static reference for this component
    static public CustomInputSettings globalReference;

    //Player inputs for each player
    public PlayerInputs p1Inputs;
    public PlayerInputs p2Inputs;



    // Use this for initialization
    private void Awake()
    {
        //If the global reference is null, this component becomes the global reference
        if(globalReference == null)
        {
            globalReference = this;
        }
        //If there's already a global reference, this component is destroyed
        else
        {
            Destroy(this);
        }
    }
}

//Class used by CustomInputSettings to hold all input buttons for a given player
[System.Serializable]
public class PlayerInputs
{
    //Bool that determines if the Y movement is inverted or not
    public bool invertYMovement = true;

    //~~~~~~~~~~~~~~~~~~~~~~~CONTROLLER INPUT~~~~~~~~~~~~~~~~~~~~~~~~~
    //The controller button for pausing the game
    public ControllerButtons pause_Controller = ControllerButtons.Start_Button;

    [Space(8)]

    //The controller stick for moving left and right
    public ControllerSticks moveLeftRight_Controller = ControllerSticks.Left_Stick_X;
    //The controller stick for moving up and down
    public ControllerSticks moveUpDown_Controller = ControllerSticks.Left_Stick_Y;
    //The controller stick for aiming left and right
    public ControllerSticks aimLeftRightStick = ControllerSticks.Right_Stick_X;
    //The controller stick for aiming up and down
    public ControllerSticks aimUpDownStick = ControllerSticks.Right_Stick_Y;

    [Space(8)]

    //The controller button used to fire the main weapon
    public ControllerButtons mainFireButton_Controller = ControllerButtons.A_Button;
    //The controller button used to fire the secondary weapon
    public ControllerButtons secondaryFireButton_Controller = ControllerButtons.B_Button;

    [Space(8)]

    //The controller button used to boost forward
    public ControllerButtons boostButton_Controller = ControllerButtons.Right_Trigger;
    //The controller button used to break
    public ControllerButtons breakButton_Controller = ControllerButtons.Left_Trigger;

    [Space(8)]

    //The controller button used to tilt/roll right
    public ControllerButtons rollRight_Controller = ControllerButtons.Right_Bumper;
    //The controller button used to tilt/roll left
    public ControllerButtons rollLeft_Controller = ControllerButtons.Left_Bumper;

    [Space(8)]

    //The controller button used to invert Y movement controls
    public ControllerButtons invertY_Controller = ControllerButtons.Back_Button;


    [Space(18)]

    //~~~~~~~~~~~~~~~~~~~~~~~KEYBOARD/MOUSE INPUT~~~~~~~~~~~~~~~~~~~~~~~~~
    //The keyboard input for pausing the game
    public KeyCode pause_Keyboard = KeyCode.Escape;

    [Space(8)]

    //The keyboard input for moving left
    public KeyCode moveLeft_Keyboard = KeyCode.A;
    //The keyboard input for moving right
    public KeyCode moveRight_Keyboard = KeyCode.D;
    //The keyboard input for moving up
    public KeyCode moveUp_Keyboard = KeyCode.W;
    //The keyboard input for moving down
    public KeyCode moveDown_Keyboard = KeyCode.S;

    [Space(8)]

    //The keyboard/mouse button used to fire the main weapon
    public KeyCode mainFireButton_Keyboard = KeyCode.Mouse0;
    //The keyboard/mouse button used to fire the secondary weapon
    public KeyCode secondaryFireButton_Keyboard = KeyCode.Mouse1;

    [Space(8)]
    
    //The keyboard/mouse button used to boost forward
    public KeyCode boostButton_Keyboard = KeyCode.Space;
    //The keyboard/mouse button used to break
    public KeyCode breakButton_Keyboard = KeyCode.LeftShift;

    [Space(8)]

    //The keyboard/mouse button used to tilt/roll right
    public KeyCode rollRight_Keyboard = KeyCode.E;
    //The keyboard/mouse button used to tilt/roll left
    public KeyCode rollLeft_Keyboard = KeyCode.Q;

    [Space(8)]

    //The keyboard/mouse button used to invert Y movement controls
    public KeyCode invertY_Keyboard = KeyCode.I;
}