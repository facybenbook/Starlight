﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIPlayerHilight : MonoBehaviour
{
    //Enum to determine which player we take input from
    public Players playerInput = Players.P1;

    //Reference to our player controller and custom input settings
    ControllerInput ourController;
    PlayerInputs ourCustomInputs;

    //The selectable UI element that this hilight starts on
    public Selectable startingSelectable;
    //The current selectable UI element that we're hilighting
    private Selectable currentSelectable;

    //The amount of cooldown time between moving between different buttons
    private float moveDelay = 0.1f;
    //The current amount of time remaining for our move delay
    private float currentDelayTime = 0;

    //The size that we scale up the UI element so that it's bigger than the one its behind
    private Vector2 sizeDiff = new Vector2(10,10);



    //Function called on initialize
    private void Start()
    {
        //Getting the reference to the player input for this selected player
        switch(this.playerInput)
        {
            case Players.P1:
                this.ourController = ControllerInputManager.P1Controller;
                this.ourCustomInputs = GlobalData.globalReference.GetComponent<CustomInputSettings>().p1Inputs;
                this.GetComponent<Image>().color = GlobalData.globalReference.P1HilightColor;
                break;

            case Players.P2:
                this.ourController = ControllerInputManager.P2Controller;
                this.ourCustomInputs = GlobalData.globalReference.GetComponent<CustomInputSettings>().p2Inputs;
                this.GetComponent<Image>().color = GlobalData.globalReference.P2HilightColor;
                break;

            default:
                this.ourController = ControllerInputManager.P1Controller;
                this.ourCustomInputs = GlobalData.globalReference.GetComponent<CustomInputSettings>().p1Inputs;
                this.GetComponent<Image>().color = GlobalData.globalReference.P1HilightColor;
                break;
        }
        
        //Setting our currently selected hilight to our starting selectable
        this.ChangeHilight(this.startingSelectable);
    }


	//Function called when this component is enabled
    private void OnEnable()
    {
        Canvas.ForceUpdateCanvases();
        //Setting our currently selected hilight to our starting selectable
        this.ChangeHilight(this.startingSelectable);
    }
	

	// Update is called once per frame
	private void Update ()
    {
        //If the player presses the A button on the controller or the spacebar or enter buttons on keyboard, the player presses the current selectable
        if(this.ourController.CheckButtonPressed(ControllerButtons.A_Button) || Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            this.ClickSelected();
        }

        //If our current delay time is above 0, we need to count it down
        if (this.currentDelayTime > 0)
        {
            this.currentDelayTime -= Time.fixedDeltaTime;
            return;
        }
        //If we don't have to wait for the delay, we can take directional input
        else
        {
            //If our player presses the up button on the D-pad, the left joystick is up, or W or Up arrow on the keyboard are pressed
            if (this.ourController.CheckButtonPressed(ControllerButtons.D_Pad_Up) ||
                -this.ourController.CheckStickValue(ControllerSticks.Left_Stick_Y) > 0.5f ||
                Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                //If our current selectable has a UI connection up
                if (this.currentSelectable.navigation.selectOnUp != null)
                {
                    this.ChangeHilight(this.currentSelectable.navigation.selectOnUp);
                }
            }
            //If our player presses the down button on the D-pad, the left joystick is down, or S or Down arrow on the keyboard are pressed
            else if (this.ourController.CheckButtonPressed(ControllerButtons.D_Pad_Down) ||
                -this.ourController.CheckStickValue(ControllerSticks.Left_Stick_Y) < -0.5f ||
                Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                //If our current selectable has a UI connection down
                if (this.currentSelectable.navigation.selectOnDown != null)
                {
                    this.ChangeHilight(this.currentSelectable.navigation.selectOnDown);
                }
            }
            //If our player presses the left button on the D-pad, the left joystick is left, or A or Left arrow on the keyboard are pressed
            else if (this.ourController.CheckButtonPressed(ControllerButtons.D_Pad_Left) ||
                this.ourController.CheckStickValue(ControllerSticks.Left_Stick_X) < -0.5f ||
                Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                //If our current selectable has a UI connection left
                if (this.currentSelectable.navigation.selectOnLeft != null)
                {
                    this.ChangeHilight(this.currentSelectable.navigation.selectOnLeft);
                }
            }
            //If our player presses the right button on the D-pad, the left joystick is right, or D or Right arrow on the keyboard are pressed
            else if (this.ourController.CheckButtonPressed(ControllerButtons.D_Pad_Right) ||
                this.ourController.CheckStickValue(ControllerSticks.Left_Stick_X) > 0.5f ||
                Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                //If our current button has a UI connection right
                if (this.currentSelectable.navigation.selectOnRight != null)
                {
                    this.ChangeHilight(this.currentSelectable.navigation.selectOnRight);
                }
            }
        }
    }


    //Function called from Update to change our hilighted selectable
    private void ChangeHilight(Selectable newSelect_)
    {
        //Setting the delay time so we don't move through the menu too fast
        this.currentDelayTime = this.moveDelay;

        //Setting our currently selected highlight UI element to the new one
        this.currentSelectable = newSelect_;

        //Setting our position to new UI element
        this.GetComponent<RectTransform>().position = newSelect_.GetComponent<RectTransform>().position;
        //Setting our scale to the same as the new UI element plus our size difference
        this.GetComponent<RectTransform>().sizeDelta = new Vector2(newSelect_.GetComponent<RectTransform>().rect.width + this.sizeDiff.x,
                                                                newSelect_.GetComponent<RectTransform>().rect.height + this.sizeDiff.y);
    }


    //Function called from Update to click the currently hilighted selectable
    private void ClickSelected()
    {
        //If this selectable is a button, we click it
        if(this.currentSelectable.GetType() == typeof(Button))
        {
            this.currentSelectable.GetComponent<Button>().onClick.Invoke();
        }

        //If this selectable is a toggle, we toggle it to the opposite value
        else if(this.currentSelectable.GetType() == typeof(Toggle))
        {
            this.currentSelectable.GetComponent<Toggle>().isOn = !this.currentSelectable.GetComponent<Toggle>().isOn;
        }

        //If this selectable is a drop-down menu, we drop it down
        else if(this.currentSelectable.GetType() == typeof(Dropdown))
        {
            this.currentSelectable.GetComponent<Dropdown>().Show();
        }
    }
}