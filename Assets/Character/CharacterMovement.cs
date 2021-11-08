using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    Animator animator;
    PlayerInput input;
    int isWalkingHash;
    int isSprintingHash;
    int isCrouchingHash;
    int isCrouchWalkingHash;
    int isCombatStanceHash;
    int isKickingHash;
    int isLegSweepHash;

    Vector2 currentMovement;
    Vector3 positionToLookAt;

    [SerializeField]
    float rotationSpeed;
    bool movementPressed;
    bool sprintPressed;
    bool crouchPressed;
    bool keyPressed;
    bool isCombatStance;
    bool isKicking;
    bool isLegSweep;
    bool activeCombatAnimation;
    void Awake() {
        input = new PlayerInput();
        input.CharacterControls.Movement.performed += ctx => {
            currentMovement = ctx.ReadValue<Vector2>();
            movementPressed = currentMovement.x != 0 || currentMovement.y != 0;
        }; 
        input.CharacterControls.Sprint.performed += ctx => sprintPressed = ctx.ReadValueAsButton();
        input.CharacterControls.Crouch.performed += ctx => crouchPressed = ctx.ReadValueAsButton();
    }
    void Start() {
        animator = GetComponent<Animator>();
        isWalkingHash = Animator.StringToHash("isWalking");
        isSprintingHash = Animator.StringToHash("isSprinting");
        isCrouchingHash = Animator.StringToHash("isCrouching");
        isCrouchWalkingHash = Animator.StringToHash("isCrouchWalking");
        isCombatStanceHash = Animator.StringToHash("isCombatStance");
        isKickingHash = Animator.StringToHash("isKicking");
        isLegSweepHash = Animator.StringToHash("isLegSweep");
    }
    void Update() {
        handleKeyPress();
        handleMovementLocks();
        handleRotation();
        handleCombat();
        handleMovement();
    }
    void handleKeyPress() {
        bool wPressed = Input.GetKey("w");
        bool aPressed = Input.GetKey("a");
        bool sPressed = Input.GetKey("s");
        bool dPressed = Input.GetKey("d");
        crouchPressed = Input.GetKey("left ctrl");

        isCombatStance = Input.GetKey("mouse 1");
        isKicking = Input.GetKey("g");
        isLegSweep = Input.GetKey("t");

        keyPressed = wPressed || aPressed || sPressed || dPressed;
        
    }
    void handleMovementLocks() {
        activeCombatAnimation = 
        this.animator.GetCurrentAnimatorStateInfo(0).IsName("Combat Stance") ||
        this.animator.GetCurrentAnimatorStateInfo(0).IsName("Kicking") ||
        this.animator.GetCurrentAnimatorStateInfo(0).IsName("Leg Sweep");
    }
    void handleRotation() {
        if(activeCombatAnimation) {
            Plane playerPlane = new Plane(Vector3.up ,transform.position);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float hitdist;
            if (playerPlane.Raycast(ray, out hitdist)) {
                Vector3 targetPoint = ray.GetPoint(hitdist);
                Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation , targetRotation, rotationSpeed*Time.deltaTime);
            }
        }
        if (!activeCombatAnimation) {
            
            Vector3 currentPosition = transform.position;
            Vector3 newPosition = new Vector3(currentMovement.x, 0, currentMovement.y);
            positionToLookAt = currentPosition + newPosition;
            transform.LookAt(positionToLookAt);
        }
    }
    void handleCombat() {
        if (isCombatStance) {
            animator.SetBool(isCombatStanceHash, true);
        }
        if (!isCombatStance) {
            animator.SetBool(isCombatStanceHash, false);
        }
        if (isKicking) {
            animator.SetBool(isKickingHash, true);
        }
        if (!isKicking) {
            animator.SetBool(isKickingHash, false);
        }        
        if (isLegSweep) {
            animator.SetBool(isLegSweepHash, true);
        }
        if (!isLegSweep) {
            animator.SetBool(isLegSweepHash, false);
        }     
    }
    void handleMovement() {
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isSprinting = animator.GetBool(isSprintingHash);
        bool isCrouching = animator.GetBool(isCrouchingHash);
        bool isCrouchWalking = animator.GetBool(isCrouchWalkingHash);

        if(!activeCombatAnimation) {
            // Set Walking Animation 
            if (keyPressed && !crouchPressed && !isWalking && !isSprinting) {
                animator.SetBool(isWalkingHash, true);
            }
            if ((!keyPressed && isWalking) || sprintPressed) {
                animator.SetBool(isWalkingHash, false);
            }
            // Set Sprinting Animation 
            if ((keyPressed && sprintPressed) && !isSprinting) {
                animator.SetBool(isSprintingHash, true);
            }
            if (((!keyPressed && !sprintPressed) && isSprinting) || !sprintPressed) {
                animator.SetBool(isSprintingHash, false);
            }
            // Set Crouching Animation 
            if ((crouchPressed && !isCrouching) && !isSprinting && !isWalking) {
                animator.SetBool(isCrouchingHash, true);
            }
            if ((!crouchPressed && isCrouching) || sprintPressed)  {
                animator.SetBool(isCrouchingHash, false);
            }
            // Set Crouch Walking Animation 
            if ((crouchPressed && keyPressed) && !isSprinting && !isWalking) {
                animator.SetBool(isCrouchWalkingHash, true);
            } 
            if ((!crouchPressed) || (crouchPressed && !keyPressed) || sprintPressed) {
                animator.SetBool(isCrouchWalkingHash, false);
            }
        }
    }
    void OnAnimatorMove() {
        Animator animator = GetComponent<Animator>();     
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isSprinting = animator.GetBool(isSprintingHash);        
        bool isCrouchWalking = animator.GetBool(isCrouchWalkingHash);

        if (!activeCombatAnimation) {
            Vector3 newPosition = transform.position;
            if (isWalking) { 
                if(currentMovement.y > 0) {
                    newPosition.z += animator.GetFloat("WalkSpeed") * Time.deltaTime;
                }
                if(currentMovement.y < 0 ) {
                    newPosition.z -= animator.GetFloat("WalkSpeed") * Time.deltaTime;
                }
                if(currentMovement.x > 0) {
                    newPosition.x += animator.GetFloat("WalkSpeed") * Time.deltaTime;
                }
                if(currentMovement.x < 0 ) {
                    newPosition.x -= animator.GetFloat("WalkSpeed") * Time.deltaTime;
                }
                transform.position = newPosition;
            }
            if (isCrouchWalking) { 
                if(currentMovement.y > 0) {
                    newPosition.z += animator.GetFloat("CrouchWalkSpeed") * Time.deltaTime;
                }
                if(currentMovement.y < 0 ) {
                    newPosition.z -= animator.GetFloat("CrouchWalkSpeed") * Time.deltaTime;
                }
                if(currentMovement.x > 0) {
                    newPosition.x += animator.GetFloat("CrouchWalkSpeed") * Time.deltaTime;
                }
                if(currentMovement.x < 0 ) {
                    newPosition.x -= animator.GetFloat("CrouchWalkSpeed") * Time.deltaTime;
                }
                transform.position = newPosition;
            }
            if (isSprinting) { 
                if(currentMovement.y > 0) {
                    newPosition.z += animator.GetFloat("SprintSpeed") * Time.deltaTime;
                }
                if(currentMovement.y < 0 ) {
                    newPosition.z -= animator.GetFloat("SprintSpeed") * Time.deltaTime;
                }
                if(currentMovement.x > 0) {
                    newPosition.x += animator.GetFloat("SprintSpeed") * Time.deltaTime;
                }
                if(currentMovement.x < 0 ) {
                    newPosition.x -= animator.GetFloat("SprintSpeed") * Time.deltaTime;
                }
                transform.position = newPosition;
            }
        }
    }

    void OnEnable() {
        input.CharacterControls.Enable();
    }
    void OnDisable() {
        input.CharacterControls.Disable();
    }
}
