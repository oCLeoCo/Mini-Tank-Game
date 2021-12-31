using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class PlayerTankController : MonoBehaviour
{
    public GameObject shell, bullet;	
    Transform Turret;
    Transform bulletSpawnPoint;    
    private float curSpeed, targetSpeed, rotSpeed;
    private float turretRotSpeed = 2f;
    private float maxForwardSpeed = 150.0f;
    private float maxBackwardSpeed = -150.0f;
    protected float[] shootRate = new float[2]; //0 = shell, 1 = bullet
    protected float[] elapsedTime = new float[2];
    protected float shootRateOfShell = 1.2f;
    protected float shootRateOfBullet = 0.2f;
    protected float shellElapsedTime;    
    protected float bulletElapsedTime;
    public bool onPanel { get; set; }
    public UnityAction<float[]> onCoolDown;
    public UnityAction<float[]> getShootRate;
    void Awake()
    {
        shootRate[0] = shootRateOfShell;
        shootRate[1] = shootRateOfBullet;
        elapsedTime[0] = shellElapsedTime;
        elapsedTime[1] = bulletElapsedTime;  
    }    
    void Start()
    {       
        onPanel = false;
        rotSpeed = 70.0f;
        Turret = gameObject.transform.GetChild(0).transform;
        bulletSpawnPoint = Turret.GetChild(0).transform;
        getShootRate.Invoke(shootRate);
    }
    void OnEndGame()
    {
        // Don't allow any more control changes when the game ends
        this.enabled = false;
    }

    void Update()
    {
        UpdateControl();
        UpdateWeapon();
    }
    
    void UpdateControl()
    {
        //AIMING WITH THE MOUSE
        // Generate a plane that intersects the transform's position with an upwards normal.
        Plane playerPlane = new Plane(Vector3.up, transform.position + new Vector3(0, 0, 0));

        // Generate a ray from the cursor position
        Ray RayCast = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Determine the point where the cursor ray intersects the plane.
        float HitDist = 0;

        // If the ray is parallel to the plane, Raycast will return false.
        if (playerPlane.Raycast(RayCast, out HitDist))
        {
            // Get the point along the ray that hits the calculated distance.
            Vector3 RayHitPoint = RayCast.GetPoint(HitDist);

            Quaternion targetRotation = Quaternion.LookRotation(RayHitPoint - transform.position);
            Turret.transform.rotation = Quaternion.Slerp(Turret.transform.rotation, targetRotation, Time.deltaTime * turretRotSpeed);
        }

        if (Input.GetKey(KeyCode.W))
        {
            targetSpeed = maxForwardSpeed;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            targetSpeed = maxBackwardSpeed;
        }
        else
        {
            targetSpeed = 0;
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(0, -rotSpeed * Time.deltaTime, 0.0f);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(0, rotSpeed * Time.deltaTime, 0.0f);
        }

        //Determine current speed
        curSpeed = Mathf.Lerp(curSpeed, targetSpeed, 7.0f * Time.deltaTime);
        transform.Translate(Vector3.forward * Time.deltaTime * curSpeed);    
    }

    void UpdateWeapon()
    {
        if(elapsedTime[0] < shootRate[0]) 
        {
            elapsedTime[0] += Time.deltaTime;
            elapsedTime[0] = elapsedTime[0] > shootRate[0]? shootRate[0]: elapsedTime[0];
            onCoolDown?.Invoke(elapsedTime);
        }
        if(elapsedTime[1] < shootRate[1])
        {
            elapsedTime[1] += Time.deltaTime;
            elapsedTime[1] = elapsedTime[1] > shootRate[1]? shootRate[1]: elapsedTime[1];
            onCoolDown?.Invoke(elapsedTime);
        }        
        if(Input.GetMouseButtonDown(0))
        {
            if (elapsedTime[0] >= shootRate[0] &&
                !onPanel)
            {
                elapsedTime[0] = 0.0f;
                onCoolDown?.Invoke(elapsedTime);
                Instantiate(shell, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            }
        }
        if(Input.GetMouseButton(1))
            if (elapsedTime[1] >= shootRate[1] &&
                !onPanel)
            {
                elapsedTime[1] = 0.0f;
                onCoolDown?.Invoke(elapsedTime);
                Instantiate(bullet, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            }        
    }
}