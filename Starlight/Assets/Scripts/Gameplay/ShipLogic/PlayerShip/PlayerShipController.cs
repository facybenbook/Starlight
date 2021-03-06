﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FreeMovementFlight))]
[RequireComponent(typeof(RailMovementFlight))]
[RequireComponent(typeof(HealthAndArmor))]
[RequireComponent(typeof(ShipEnergy))]
[RequireComponent(typeof(CustomShipTextures))]
public class PlayerShipController : MonoBehaviour
{
    //Enum to determine which player controls this ship
    public Players playerController = Players.P1;

    //References to the public static ship controllers for each ship
    public static PlayerShipController p1ShipRef;
    public static PlayerShipController p2ShipRef;

    //Reference to this ship's Health and Armor component
    [HideInInspector]
    public HealthAndArmor ourHealth;
    //Reference to this ship's energy component
    [HideInInspector]
    public ShipEnergy ourEnergy;

    //The controller input that we use for this ship
    [HideInInspector]
    public ControllerInput ourController;

    //The input settings for this player
    [HideInInspector]
    public PlayerInputs ourCustomInputs;

    [Space(8)]

    //References to this ship's different movement mechanic scripts
    [HideInInspector]
    public FreeMovementFlight ourFreeMovement;
    [HideInInspector]
    public RailMovementFlight ourRailMovement;

    //The main weapon for this ship
    public Weapon mainWeapon;
    //The secondary weapon for this ship
    public Weapon secondaryWeapon;

    [Space(8)]

    //The game objects that are used as a gyroscope to pivot our ship model without having to deal with annoying rotation problems
    public Transform xGyroscope;
    public Transform yGyroscope;
    public Transform zGyroscope;

    [Space(8)]

    //The health object for our shield
    public HealthAndArmor shipShield;

    [Space(8)]

    //The health object for our cockpit
    public HealthAndArmor shipCockpit;

    [Space(8)]

    //The list of all wing objects that are attached to this ship
    public List<ShipWingLogic> shipWings;

    [Space(8)]

    //The list of all engine objects that are attached to this ship
    public List<ShipEngineLogic> shipEngines;

    //The audio emitter for the engine sound effect
    public ExtraSoundEmitterSettings engineSoundEmitter;

    //The amount of energy used each frame to boost
    public float boostEnergyCost = 2;
    //The amount of energy used each frame to break
    public float breakEnergyCost = 1.5f;

    [Space(8)]

    //The default pitch for the engine sound emitter
    private float defaultPitch = 1f;
    //The pitch for the engine sound emitter while boosting
    [Range(0f, 3f)]
    public float boostSoundPitch = 1.5f;
    //The pitch for the engine sound emitter while breaking
    [Range(0f, 3f)]
    public float breakSoundPitch = 0.5f;
    //The speed that this ship interpolates the sound pitch
    [Range(0.01f, 0.99f)]
    public float pitchInterpSpeed = 0.8f;

    //Bools that let other scripts know if we're currently boosting or breaking
    [HideInInspector]
    public bool isShipBoosting = false;
    [HideInInspector]
    public bool isShipBreaking = false;

    //Bools that let other scripts know if we're currently tilting or rolling
    [HideInInspector]
    public bool isShipTilting = false;




    //Function called when this object is created
    private void Awake()
    {
        //Getting the movement component references
        this.ourFreeMovement = this.GetComponent<FreeMovementFlight>();
        this.ourRailMovement = this.GetComponent<RailMovementFlight>();

        //Getting our controller input based on which player this is
        switch(this.playerController)
        {
            case Players.P1:
                //Making sure there's not already a static reference to the p1 ship
                if (p1ShipRef == null)
                {
                    p1ShipRef = this;
                    this.ourController = ControllerInputManager.P1Controller;
                    this.ourCustomInputs = CustomInputSettings.globalReference.p1Inputs;
                }
                //If there's already a static reference for the p1 ship and not one for the p2 ship
                else if(p2ShipRef == null)
                {
                    this.playerController = Players.P2;
                    p2ShipRef = this;
                    this.ourController = ControllerInputManager.P2Controller;
                    this.ourCustomInputs = CustomInputSettings.globalReference.p2Inputs;
                }
                //If there are already static references to both ships, we disable this object
                {
                    this.gameObject.SetActive(false);
                }
                break;

            case Players.P2:
                //Making sure there's not already a static reference to the p2 ship
                if (p2ShipRef == null)
                {
                    p2ShipRef = this;
                    this.ourController = ControllerInputManager.P2Controller;
                    this.ourCustomInputs = CustomInputSettings.globalReference.p2Inputs;
                }
                //If there's already a static reference for the p2 ship, we disable this object
                else
                {
                    this.gameObject.SetActive(false);
                }
                break;

            default:
                //Making sure there's not already a static reference to the p1 ship
                if (p1ShipRef == null)
                {
                    p1ShipRef = this;
                    this.ourController = ControllerInputManager.P1Controller;
                    this.ourCustomInputs = CustomInputSettings.globalReference.p1Inputs;
                }
                //If there's already a static reference for the p1 ship and not one for the p2 ship
                else if (p2ShipRef == null)
                {
                    this.playerController = Players.P2;
                    p2ShipRef = this;
                    this.ourController = ControllerInputManager.P2Controller;
                    this.ourCustomInputs = CustomInputSettings.globalReference.p2Inputs;
                }
                //If there are already static references to both ships, we disable this object
                {
                    this.gameObject.SetActive(false);
                }
                break;
        }

        //Passing our controller input to our movement mechanic scripts
        this.ourFreeMovement.ourShip = this;
        this.ourRailMovement.ourShip = this;

        //Getting our health and armor component reference
        this.ourHealth = this.GetComponent<HealthAndArmor>();
        //Getting our energy component reference
        this.ourEnergy = this.GetComponent<ShipEnergy>();

        //Looping through all of our weapons, wings and engines to tell them what player ID we are
        if (this.playerController == Players.P1)
        {
            this.mainWeapon.objectIDType = AttackerID.Player1;
            this.secondaryWeapon.objectIDType = AttackerID.Player1;
            this.shipCockpit.objectIDType = AttackerID.Player1;
        }
        else
        {
            this.mainWeapon.objectIDType = AttackerID.Player2;
            this.secondaryWeapon.objectIDType = AttackerID.Player2;
            this.shipCockpit.objectIDType = AttackerID.Player2;
        }
        foreach (ShipWingLogic wing in this.shipWings)
        {
            if(this.playerController == Players.P1)
            {
                wing.objectIDType = AttackerID.Player1;
            }
            else if (this.playerController == Players.P2)
            {
                wing.objectIDType = AttackerID.Player2;
            }
        }
        foreach(ShipEngineLogic engine in this.shipEngines)
        {
            if (this.playerController == Players.P1)
            {
                engine.objectIDType = AttackerID.Player1;
            }
            else if (this.playerController == Players.P2)
            {
                engine.objectIDType = AttackerID.Player2;
            }
        }

        //Getting the default pitch for the engine sound effect
        this.defaultPitch = this.engineSoundEmitter.ownerAudio.pitch;
    }


    //Function called from PlayerStartingPositin.Awake to set the player ship controller IDs
    public void SetPlayerShipID(Players playerID_)
    {
        //Setting this ship's controller to the given ID
        this.playerController = playerID_;

        //Getting our controller input based on which player this is
        switch (playerID_)
        {
            case Players.P1:
                this.gameObject.SetActive(true);
                p1ShipRef = this;
                this.ourController = ControllerInputManager.P1Controller;
                this.ourCustomInputs = CustomInputSettings.globalReference.p1Inputs;
                this.GetComponent<CameraWeight>().playerThatCanFollow = Players.P1;
                this.ourRailMovement.railParentObj.GetComponent<CameraWeight>().playerThatCanFollow = Players.P1;
                this.GetComponent<CustomShipTextures>().SetPlayerShipID(Players.P1);
                this.shipShield.objectIDType = AttackerID.Player1;
                break;

            case Players.P2:
                this.gameObject.SetActive(true);
                p2ShipRef = this;
                this.ourController = ControllerInputManager.P2Controller;
                this.ourCustomInputs = CustomInputSettings.globalReference.p2Inputs;
                this.GetComponent<CameraWeight>().playerThatCanFollow = Players.P2;
                this.ourRailMovement.railParentObj.GetComponent<CameraWeight>().playerThatCanFollow = Players.P2;
                this.GetComponent<CustomShipTextures>().SetPlayerShipID(Players.P2);
                this.shipShield.objectIDType = AttackerID.Player2;

                //If this ship was set as the p1 ship reference, we remove it
                if (p1ShipRef == this)
                {
                    p1ShipRef = null;
                }
                break;

            default:
                this.gameObject.SetActive(true);
                p1ShipRef = this;
                this.ourController = ControllerInputManager.P1Controller;
                this.ourCustomInputs = CustomInputSettings.globalReference.p1Inputs;
                this.GetComponent<CameraWeight>().playerThatCanFollow = Players.P1;
                this.ourRailMovement.railParentObj.GetComponent<CameraWeight>().playerThatCanFollow = Players.P1;
                this.GetComponent<CustomShipTextures>().SetPlayerShipID(Players.P1);
                this.shipShield.objectIDType = AttackerID.Player1;
                break;
        }

        //Looping through all of our weapons, wings and engines to tell them what player ID we are
        if (this.playerController == Players.P1)
        {
            this.mainWeapon.objectIDType = AttackerID.Player1;
            this.secondaryWeapon.objectIDType = AttackerID.Player1;
            this.shipCockpit.objectIDType = AttackerID.Player1;
        }
        else
        {
            this.mainWeapon.objectIDType = AttackerID.Player2;
            this.secondaryWeapon.objectIDType = AttackerID.Player2;
            this.shipCockpit.objectIDType = AttackerID.Player2;
        }
        foreach (ShipWingLogic wing in this.shipWings)
        {
            if (this.playerController == Players.P1)
            {
                wing.objectIDType = AttackerID.Player1;
            }
            else if (this.playerController == Players.P2)
            {
                wing.objectIDType = AttackerID.Player2;
            }
        }
        foreach (ShipEngineLogic engine in this.shipEngines)
        {
            if (this.playerController == Players.P1)
            {
                engine.objectIDType = AttackerID.Player1;
            }
            else if (this.playerController == Players.P2)
            {
                engine.objectIDType = AttackerID.Player2;
            }
        }
    }


	// Update is called once per frame
	private void Update ()
    {
        //If the game is paused, we stop taking input
        if(PauseGame.isGamePaused)
        {
            return;
        }

		//If we have a main weapon, we pass it the controller input and keyboard input for the main fire
        if(this.mainWeapon != null)
        {
            this.mainWeapon.FireWeapon(this.ourController.CheckButtonPressed(this.ourCustomInputs.mainFireButton_Controller),
                                        this.ourController.CheckButtonDown(this.ourCustomInputs.mainFireButton_Controller),
                                        this.ourController.CheckButtonReleased(this.ourCustomInputs.mainFireButton_Controller));

            this.mainWeapon.FireWeapon(Input.GetKeyDown(this.ourCustomInputs.mainFireButton_Keyboard),
                                        Input.GetKey(this.ourCustomInputs.mainFireButton_Keyboard),
                                        Input.GetKeyUp(this.ourCustomInputs.mainFireButton_Keyboard));
        }

        //If we have a secondary weapon, we pass it the controller input for the secondary fire
        if(this.secondaryWeapon != null)
        {
            this.secondaryWeapon.FireWeapon(this.ourController.CheckButtonPressed(this.ourCustomInputs.secondaryFireButton_Controller),
                                        this.ourController.CheckButtonDown(this.ourCustomInputs.secondaryFireButton_Controller),
                                        this.ourController.CheckButtonReleased(this.ourCustomInputs.secondaryFireButton_Controller));
            
            this.secondaryWeapon.FireWeapon(Input.GetKeyDown(this.ourCustomInputs.secondaryFireButton_Keyboard),
                                        Input.GetKey(this.ourCustomInputs.secondaryFireButton_Keyboard),
                                        Input.GetKeyUp(this.ourCustomInputs.secondaryFireButton_Keyboard));
        }

        //Checking to see if we're pressing a boost button and if we're not currently breaking
        if ((this.ourController.CheckButtonDown(this.ourCustomInputs.boostButton_Controller) ||
            Input.GetKey(this.ourCustomInputs.boostButton_Keyboard)) &&
            !this.isShipBreaking)
        {
            //Making sure we have enough energy
            if (this.ourEnergy.CanUseEnergy(this.boostEnergyCost))
            {
                this.isShipBoosting = true;
            }
            //If not, we aren't boosting
            else
            {
                this.isShipBoosting = false;
            }
        }
        //Otherwise, we aren't boosting
        else
        {
            this.isShipBoosting = false;
        }

        //Checking to see if we're pressing a break button and if we're not currently boosting
        if ((this.ourController.CheckButtonDown(this.ourCustomInputs.breakButton_Controller) ||
            Input.GetKey(this.ourCustomInputs.breakButton_Keyboard)) &&
            !this.isShipBoosting)
        {
            //Making sure we have enough energy
            if (this.ourEnergy.CanUseEnergy(this.breakEnergyCost))
            {
                this.isShipBreaking = true;
            }
            //If not, we aren't breaking
            else
            {
                this.isShipBreaking = false;
            }
        }
        //Otherwise, we aren't breaking
        else
        {
            this.isShipBreaking = false;
        }

        //If the player presses the button to invert Y movement controls
        if (this.ourController.CheckButtonPressed(this.ourCustomInputs.invertY_Controller) || Input.GetKeyDown(this.ourCustomInputs.invertY_Keyboard))
        {
            this.ourCustomInputs.invertYMovement = !this.ourCustomInputs.invertYMovement;
        }
        
        //Updating our ship's health
        this.UpdateHealth();

        //Updating our ship engine's sound pitch
        this.UpdateEngineSoundPitch();
	}


    //Function called from Update to find out how much health and shields our ship has
    private void UpdateHealth()
    {
        //Int to hold the sum of all damage taken from each ship component
        int damageTakenSum = 0;

        //If our shield has taken health damage, that means it's no longer up so excess damage is dealt to the ship
        if(this.shipShield.currentHealth < this.shipShield.maxHealth)
        {
            //Disabling the ship shield
            this.shipShield.gameObject.SetActive(false);

            //Adding the damage to our sum
            damageTakenSum += (this.shipShield.maxHealth - this.shipShield.currentHealth);

            //Setting the shield's health back to max to max
            this.shipShield.currentHealth = this.shipShield.maxHealth;
        }

        //Looping through each ship wing
        foreach(ShipWingLogic wing in this.shipWings)
        {
            //If our shield is still up, this wing is invulnerable
            if(this.shipShield.currentShields > 0)
            {
                wing.gameObject.SetActive(false);
            }
            //Otherwise, this wing is vulnerable
            else
            {
                wing.gameObject.SetActive(true);
            }

            //Adding the amount of damage to our sum
            damageTakenSum += wing.maxHealth - wing.currentHealth;
        }

        //Looping through each ship engine
        foreach(ShipEngineLogic engine in this.shipEngines)
        {
            //If our shield is still up, this engine is invulnerable
            if (this.shipShield.currentShields > 0)
            {
                engine.gameObject.SetActive(false);
            }
            //Otherwise, this engine is vulnerable
            else
            {
                engine.gameObject.SetActive(true);
            }

            //Adding the amount of damage to our sum
            damageTakenSum += engine.maxHealth - engine.currentHealth;
        }

        //If our shield is still up, the cockpit is invulnerable
        if(this.shipShield.currentShields > 0)
        {
            this.shipCockpit.gameObject.SetActive(false);
        }
        //Otherwise, this engine is vulnerable
        else
        {
            this.shipCockpit.gameObject.SetActive(true);
        }

        //Adding the amount of damage to the cockpit to our sum
        damageTakenSum += this.shipCockpit.maxHealth - this.shipCockpit.currentHealth;
        
        //Setting our health value minus the total damage taken
        this.ourHealth.currentHealth = this.ourHealth.maxHealth - damageTakenSum;
        //Setting our shield's current and max values
        this.ourHealth.maxShield = this.shipShield.maxShield;
        this.ourHealth.currentShields = this.shipShield.currentShields;
    }


    //Function called from Update to interpolate our engine sound emitter pitch based on our speed
    private void UpdateEngineSoundPitch()
    {
        //If we're boosting, we interpolate to the boost pitch
        if(this.isShipBoosting && !this.isShipBreaking)
        {
            this.engineSoundEmitter.ownerAudio.pitch += (this.boostSoundPitch - this.engineSoundEmitter.ownerAudio.pitch) * this.pitchInterpSpeed;
        }
        //If we're breaking, we interpolate to the break pitch
        else if(this.isShipBreaking && !this.isShipBoosting)
        {
            this.engineSoundEmitter.ownerAudio.pitch += (this.breakSoundPitch - this.engineSoundEmitter.ownerAudio.pitch) * this.pitchInterpSpeed;
        }
        //If we're neither boosting or breaking, we interpolate to the default pitch
        else
        {
            this.engineSoundEmitter.ownerAudio.pitch += (this.defaultPitch - this.engineSoundEmitter.ownerAudio.pitch) * this.pitchInterpSpeed;
        }
    }


    //Function called when this object hits a trigger collider
    private void OnTriggerEnter(Collider collider_)
    {
        //If the object hit has a RegionZone.cs component, we change our movement behaviors
        if(collider_.gameObject.GetComponent<RegionZone>())
        {
            //If the region zone effects this player
            if ((collider_.gameObject.GetComponent<RegionZone>().affectPlayer1 && this.playerController == Players.P1) ||
                (collider_.gameObject.GetComponent<RegionZone>().affectPlayer2 && this.playerController == Players.P2))
            {
                //If the region has rail movement
                /*if (collider_.gameObject.GetComponent<RegionZone>().movementType == RegionZone.RegionMovement.Rail)
                {
                    //We disable our free movement controls and enable our rail movement controls
                    this.ourFreeMovement.enabled = false;
                    this.ourRailMovement.enabled = true;

                    //Setting the new direction for our rail movement
                    this.ourRailMovement.SetNewRailDirection(collider_);
                }*/
                //If the region has free movement
                if (collider_.gameObject.GetComponent<RegionZone>().movementType == RegionZone.RegionMovement.Free)
                {
                    //We disable our rail movement controls and enable our free movement controls
                    this.ourRailMovement.BeforeDisable();
                    this.ourRailMovement.enabled = false;
                    this.ourFreeMovement.enabled = true;
                }
            }
        }
    }


    //Function called externally to toggle invincibility for this ship
    public void ToggleIFrames(bool isInvincible_)
    {
        //Setting the invulnerability for the cockpit
        this.shipCockpit.isInvulnerable = isInvincible_;

        //Looping through all of our wings and setting their invincibility
        foreach(ShipWingLogic wing in this.shipWings)
        {
            wing.isInvulnerable = isInvincible_;
        }

        //Looping through all of our engines and setting their invincibility
        foreach(ShipEngineLogic enging in this.shipEngines)
        {
            enging.isInvulnerable = isInvincible_;
        }
    }
}
