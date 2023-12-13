using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager singleton;
    
    [SerializeField][Tooltip("Whether or not player input is enabled")]
    private bool playerInputEnabled = true;

    // Private Input Variables
    private KeyCode jumpKey = KeyCode.Z;
    private KeyCode dashKey = KeyCode.C;
    private KeyCode fireKey = KeyCode.Space;
    private KeyCode meleeKey = KeyCode.X;
    private KeyCode sneakKey = KeyCode.LeftShift;
    private KeyCode upKey = KeyCode.UpArrow;
    private KeyCode downKey = KeyCode.DownArrow;
    private KeyCode escapeKey = KeyCode.Escape;
    private KeyCode enterKey = KeyCode.Return;

    // Player Input Variables
    private bool rightPressed;
    private bool leftPressed;
    private bool jumpPressed;
    private bool jumpHolding;
    private bool jumpReleased;
    private bool dashPressed;
    private bool meleePressed;
    private bool fireReleased;
    private bool fireHolding;
    private bool sneakHolding;
    private bool upHolding;
    private bool downHolding;
    private bool closePressed;
    private bool continuePressed;
    private bool escapePressed;
    private float xInput;
    
    private void Awake() {
        if (singleton != null && singleton != this) { 
            Destroy(this.gameObject); 
        } else { 
            singleton = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        continuePressed = Input.anyKeyDown;
        closePressed = Input.GetKeyDown(enterKey);
        escapePressed = Input.GetKeyDown(escapeKey);
        if (closePressed) LevelUI.singleton.ExitPopUp();
        if (escapePressed) GameManager.singleton.ToggleOptions();

        if (playerInputEnabled) {
            xInput = Input.GetAxisRaw("Horizontal");
            jumpPressed = Input.GetKeyDown(jumpKey);
            jumpHolding = Input.GetKey(jumpKey);
            jumpReleased = Input.GetKeyUp(jumpKey);
            dashPressed = Input.GetKeyDown(dashKey);
            sneakHolding = Input.GetKey(sneakKey);
            meleePressed = Input.GetKeyDown(meleeKey);
            fireReleased = Input.GetKeyUp(fireKey);
            fireHolding = Input.GetKey(fireKey);
            upHolding = Input.GetKey(upKey);
            downHolding = Input.GetKey(downKey);
        } else {
            xInput = 0f;
            jumpPressed = false;
            jumpHolding = false;
            jumpReleased = false;
            dashPressed = false;
            sneakHolding = false;
            meleePressed = false;
            fireReleased = false;
            fireHolding = false;
            upHolding = false;
            downHolding = false;
        }
    }

    // All getters for Player Input
    public float XInput {get {return xInput;}}
    public bool PlayerInputEnabled {get {return playerInputEnabled;} set {playerInputEnabled = value;}}
    public bool LeftPressed {get {return leftPressed;}}
    public bool JumpPressed {get {return jumpPressed;}}
    public bool JumpHolding {get {return jumpHolding;}}
    public bool JumpReleased {get {return jumpReleased;}}
    public bool DashPressed {get {return dashPressed;}}
    public bool MeleePressed {get {return meleePressed;}}
    public bool FireReleased {get {return fireReleased;}}
    public bool FireHolding {get {return fireHolding;}}
    public bool SneakHolding {get {return sneakHolding;}}
    public bool UpHolding {get {return upHolding;}}
    public bool DownHolding {get {return downHolding;}}
    public bool ContinuePressed {get {return continuePressed;}}
    public bool ClosedPressed {get{return closePressed;}}
}
