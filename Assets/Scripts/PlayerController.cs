using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private float mouseSensitivityX;
    [SerializeField]
    private float mouseSensitivityY;

    [SerializeField]
    private float ThrusterForce = 1000f;

    [SerializeField]
    private float thrusterFuelBurnSpeed=1f;
    [SerializeField]
    private float thrusterFuelRegenSpeed=03f;
    private float thrusterFuelAmount = 1f;

    public float GetThrusterFuelAmount()
    {
        return thrusterFuelAmount;
    }

   [Header("Joint Option")]
    [SerializeField]
    private float jointSpring;
    [SerializeField]
    private float jointMaxForce;

    private PlayerMotor motor;
    private ConfigurableJoint joint;
    private Animator animator;
    CharacterController playerController;
    CapsuleCollider capsuleCollider;

    private void Awake()
    {
        // Temporaire, ces 2 composants n'arrètent pas de se rajouter d'eux-mêmes et ils m'ennuient passablement
        playerController = GetComponent<CharacterController>();
        capsuleCollider = GetComponent<CapsuleCollider>();
       
        playerController.enabled = false;
        capsuleCollider.enabled = false;
      
    }
    private void Start()
    {
        motor = GetComponent<PlayerMotor>();
        joint= GetComponent<ConfigurableJoint>();
        animator = GetComponent<Animator>();

        // sauvegarde dynamique des paramètres de joint
        jointSpring = joint.yDrive.positionSpring;
        jointMaxForce = joint.yDrive.maximumForce;
        // paramétrage initiale 
        setJointSettings(jointSpring);

        //Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (PauseMenu.isOn)
        {
            if (Cursor.lockState != CursorLockMode.None)
            {   // Active le curseur quand on entre dans le menu pause
                Cursor.lockState = CursorLockMode.None;
            }

            motor.Move(Vector3.zero);
            motor.Rotate(Vector3.zero);
            motor.RotateCamera(0f);
            motor.ApplyThruster(Vector3.zero);

            return;
        }

        if (Cursor.lockState != CursorLockMode.Locked)
        {// Désactive le curseur quand on est pas dans le menu pause 
            Cursor.lockState = CursorLockMode.Locked;
        }

        RaycastHit _hit;
        if (Physics.Raycast(transform.position,Vector3.down,out _hit,100f))
        {
            joint.targetPosition = new Vector3(0f,-_hit.point.y, 0f);
        }
        else
        {
            joint.targetPosition = new Vector3(0f, 0f, 0f);
        }
        
        // calculer la vitesse du mvt du joueur
        float xMove = Input.GetAxis("Horizontal");
        float zMove = Input.GetAxis("Vertical");

        Vector3 moveHorizontal = transform.right * xMove;
        Vector3 moveVertical = transform.forward * zMove;

        Vector3 velocity = (moveHorizontal + moveVertical) *speed;

        // jouer animation
        animator.SetFloat("forwardVelocity", zMove);
        motor.Move(velocity);

        // calculer la rotation du joueur vector3 ftw
        float yRot = Input.GetAxisRaw("Mouse X");

        Vector3 rotation = new Vector3(0, yRot, 0)*mouseSensitivityX;

        motor.Rotate(rotation);

        // calculer la rotation du joueur vector3 ftw
        float xRot = Input.GetAxisRaw("Mouse Y");

        float cameraRotationX = xRot * mouseSensitivityY;

        motor.RotateCamera(cameraRotationX);

        // Gestion du jetpack

        Vector3 ThrusterVelocity = Vector3.zero;
        if (Input.GetButton("Jump")&& thrusterFuelAmount>0)
        {
            thrusterFuelAmount -= thrusterFuelBurnSpeed * Time.deltaTime;

            if (thrusterFuelAmount>=0.05f)
            {
                ThrusterVelocity = Vector3.up * ThrusterForce;
                setJointSettings(0f);
            }
            
        }
        else
        {
            thrusterFuelAmount += thrusterFuelRegenSpeed * Time.deltaTime;
            setJointSettings(jointSpring);
        }
        thrusterFuelAmount = Mathf.Clamp(thrusterFuelAmount,0f, 1f);
        motor.ApplyThruster(ThrusterVelocity);
    }

    private void setJointSettings(float _jointSpring)
    {
        joint.yDrive = new JointDrive { positionSpring = _jointSpring, maximumForce = jointMaxForce };
    }
}
