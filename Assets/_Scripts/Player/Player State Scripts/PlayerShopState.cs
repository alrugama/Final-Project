using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This State is responsible for managing menu navigation (I THINK)
// We might not need this state depending on how we implement our menu system
public class PlayerShopState : PlayerBaseState
{
    private Camera mainCamera;
    private bool fireButtonDown = false;
    private bool tabButtonDown = false;
    public bool running = true;
    public PlayerInput playerInput;
    
    private void Awake()
    {
        mainCamera = Camera.main;
    }
    public override void EnterState(PlayerStateManager Player)
    {
        playerInput = Player.playerInput;
        mainCamera = Camera.main;
        running = false;
        playerInput.ShopKeeper.DisplayShop();
    }

    public override void UpdateState(PlayerStateManager Player)
    {
        GetMovementInput();
        GetPointerInput();
        GetFireInput();
        GetTabInput();
        // GetInteractInput();
        if(!playerInput.ShopKeeper.inDistance)
        {
            running = true;
            playerInput.ShopKeeper.CloseShop();
            Player.SwitchState(Player.RunGunState);
        }
    }

    private void GetFireInput()
    {
        if (Input.GetAxisRaw("Fire1") > 0)
        {
            if (fireButtonDown == false)
            {
                fireButtonDown = true;
                playerInput.OnFireButtonPressed?.Invoke();
            }
        }
        else
        {
            if (fireButtonDown == true)
            {
                fireButtonDown = false;
                playerInput.OnFireButtonReleased?.Invoke();
            }
        }
    }

    private void GetPointerInput()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = mainCamera.nearClipPlane;
        var mouseInWorldSpace = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        // This invokes AgentRenderer.FaceDirection and PlayerWeapon.AimWeapon
        playerInput.OnPointerPositionChange?.Invoke(mouseInWorldSpace);
    }


    private void GetMovementInput()
    {
        playerInput.OnMovementKeyPressed?.Invoke(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));
    }

    private void GetTabInput()
    {
        if (Input.GetAxisRaw("Tab") > 0)
        {
            Debug.Log("Tab key pressed");
            if (tabButtonDown == false)
            {
                tabButtonDown = true;
                playerInput.OnTabKeyPressed?.Invoke();
            }
            // Debug.Log("Tab key pressed");
        }
        else
        {
            if (tabButtonDown == true)
            {
                tabButtonDown = false;
            }
        }
    }

    private void GetInteractInput()
    {
        // Create new Vector2 when dodge button (left shift) pressed
        if (Input.GetAxisRaw("Interact") > 0) 
        {
            if (running == false)
            {
                running = true;
                playerInput.ShopKeeper.CloseShop();
                playerInput.OnInteractKeyPressed?.Invoke();
            }  
        }
        else{
            if (running == true)
            {
                running = false;
            }
        }
    }
}
