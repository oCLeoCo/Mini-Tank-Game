using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

public class SelfFSM : FSM 
{
    public enum FSMState
    {
        None,   // 0
        Patrol, // 1
        Chase,  // 2
        Attack, // 3
        Dead,   // 4
    }
    GameManager gameManager;
    //public GameObject label;
    public UnityAction<int> onHealthChange;
    float shellResistance = 0;
    float bulletResistance = 0.5f;

    //Current state that the NPC is reaching
    public FSMState curState;

    //Speed of the tank
    public float curSpeed = 150.0f;

    //Tank Rotation Speed
    public float curRotSpeed = 2.0f;

    //Bullet
    public GameObject Bullet;

    //Whether the NPC is destroyed or not
    private bool bDead;
    public float health = 100;


    //Initialize the Finite state machine for the NPC tank
	protected override void Initialize () 
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        curState = FSMState.Patrol;
        bDead = false;
        elapsedTime = 0.0f;
        shootRate = 3.0f;

        //Get the list of points
        pointList = GameObject.FindGameObjectsWithTag("WandarPoint");

        //Set Random destination point first
        FindNextPoint();

        //Get the target enemy(Player)
        GameObject objPlayer = GameObject.FindGameObjectWithTag("Player");
        playerTransform = objPlayer.transform;

        //if(!playerTransform)
            //print("Player doesn't exist.. Please add one with Tag named 'Player'");

        //Get the turret of the tank
        turret = gameObject.transform.GetChild(0).transform;
        bulletSpawnPoint = turret.GetChild(0).transform;
	}

    //Update each frame. Called by Unity Update()
    protected override void FSMUpdate()
    {
        switch (curState)
        {   // Only call UpdateState HERE.
            case FSMState.Patrol: UpdatePatrolState(); break;
            case FSMState.Chase: UpdateChaseState(); break;
            case FSMState.Attack: UpdateAttackState(); break;
            case FSMState.Dead: UpdateDeadState(); break;
        }

        //Update the time
        elapsedTime += Time.deltaTime;

        //Go to dead state is no health left
        if (health <= 0)
            curState = FSMState.Dead; // Cannot call UpdateDeadState()
    }

    /// <summary>
    /// Patrol state
    /// </summary>
    protected void UpdatePatrolState()
    {
        // Update Label to Patrol
        //label.GetComponent<Text>().text = "Patrol : " + health;

        //Find another random patrol point if the current point is reached
        if (Vector3.Distance(transform.position, destPos) <= 100.0f)
        {
            //print("Reached to the destination point\ncalculating the next point");
            FindNextPoint(); // find next random wanderpoint
        }
        //Check the distance with player tank (d)
        //When the distance is near, transition to chase state                 d <= 300
        else if (Vector3.Distance(transform.position, playerTransform.position) <= 300.0f)
        {
            //print("Switch to Chase Position");
            curState = FSMState.Chase;  // DO NOT call UpdateChaseState()
        }

        //Rotate to the target point
        Quaternion targetRotation = Quaternion.LookRotation(destPos - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * curRotSpeed);  

        //Go Forward by one step depends on Time.deltaTime
        transform.Translate(Vector3.forward * Time.deltaTime * curSpeed);
    }

    /// <summary>
    /// Chase state
    /// </summary>
    protected void UpdateChaseState()
    {
        // Update Label to Chase
        //label.GetComponent<Text>().text = "Chase : " + health;

        //Set the target position as the player position
        destPos = playerTransform.position;

        //Check the distance with player tank
        //When the distance is near, transition to attack state
        float dist = Vector3.Distance(transform.position, playerTransform.position);
        if (dist <= 200.0f) // d <= 200
        {
            curState = FSMState.Attack; // switch to attack
        }
        //Go back to patrol is it become too far
        else if (dist >= 300.0f) // d >= 300
        {
            curState = FSMState.Patrol; // switch to patrol
        }

        //Go Forward
        transform.Translate(Vector3.forward * Time.deltaTime * curSpeed);
    }

    /// <summary>
    /// Attack state
    /// </summary>
    protected void UpdateAttackState()
    {
        // Update Label to Attack
        //label.GetComponent<Text>().text = "Attack : " + health;

        //Set the target position as the player position
        destPos = playerTransform.position;

        //Check the distance with the player tank
        float dist = Vector3.Distance(transform.position, playerTransform.position);
        if (dist >= 200.0f && dist < 300.0f)  // 200 <= d < 300
        {
            //Rotate to the target point. To the player, destPos
            Quaternion targetRotation = Quaternion.LookRotation(destPos - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * curRotSpeed);  

            //Go Forward to the player
            transform.Translate(Vector3.forward * Time.deltaTime * curSpeed);

            curState = FSMState.Attack;
        }
        //Transition to patrol is the tank become too far
        else if (dist >= 300.0f)
        {
            curState = FSMState.Patrol;
        }        

        //Always Turn the turret towards the player
        Quaternion turretRotation = Quaternion.LookRotation(destPos - turret.position);
        turret.rotation = Quaternion.Slerp(turret.rotation, turretRotation, Time.deltaTime * curRotSpeed); 

        //Shoot the bullets
        ShootBullet();
    }

    /// <summary>
    /// Dead state
    /// </summary>
    protected void UpdateDeadState()
    {
        //Show the dead animation with some physics effects
        if (!bDead)
        {
            bDead = true;
            Explode();
        }
    }

    /// <summary>
    /// Shoot the bullet from the turret
    /// </summary>
    private void ShootBullet()
    {
        if (elapsedTime >= shootRate)
        {
            //Shoot the bullet
            Instantiate(Bullet, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            elapsedTime = 0.0f;
        }
    }

    /// <summary>
    /// Check the collision with the bullet
    /// </summary>
    /// <param name="collision"></param>
    void OnCollisionEnter(Collision collision)
    {
        //Reduce health
        if (collision.gameObject.tag == "Bullet")
        {
            SelfBullet selfBullet = collision.gameObject.GetComponent<SelfBullet>();
            //HealthChange(selfBullet.damage, selfBullet.type);
            //Debug.Log(health);
        }       // hp -= bullet's damage
    }
    public void HealthChange(int value, int type)
    {   //type 1 shell, type 2 bullet, type 3 repair
        float change = 0;
        switch(type)
        {
            case 1:
                change = -(float)value * ( 1 - shellResistance );
                break;
            case 2:
                change = -(float)value * ( 1 - bulletResistance );
                break;
            default:
                health += value;
                break;
        }
        health += (int)change;
        //onHealthChange.Invoke(health);
    }
    /// <summary>
    /// Find the next semi-random patrol point
    /// </summary>
    protected void FindNextPoint()
    {
        //print("Finding next point");
        int rndIndex = Random.Range(0, pointList.Length);
        float rndRadius = 10.0f;
        
        Vector3 rndPosition = Vector3.zero;
        destPos = pointList[rndIndex].transform.position + rndPosition;

        //Check Range
        //Prevent to decide the random point as the same as before
        if (IsInCurrentRange(destPos))
        {
            rndPosition = new Vector3(Random.Range(-rndRadius, rndRadius), 0.0f, Random.Range(-rndRadius, rndRadius));
            destPos = pointList[rndIndex].transform.position + rndPosition;
        }
    }

    /// <summary>
    /// Check whether the next random position is the same as current tank position
    /// </summary>
    /// <param name="pos">position to check</param>
    protected bool IsInCurrentRange(Vector3 pos)
    {
        float xPos = Mathf.Abs(pos.x - transform.position.x);
        float zPos = Mathf.Abs(pos.z - transform.position.z);

        if (xPos <= 50 && zPos <= 50)
            return true;

        return false;
    }

    protected void Explode()
    {
        //float rndX = Random.Range(10.0f, 30.0f);
        //float rndZ = Random.Range(10.0f, 30.0f);
        //for (int i = 0; i < 3; i++)
        //{
            //GetComponent<Rigidbody>().AddExplosionForce(10000000.0f, transform.position - new Vector3(rndX, 10.0f, rndZ), 40.0f, 10.0f);
            //GetComponent<Rigidbody>().velocity = transform.TransformDirection(new Vector3(rndX, 20.0f, rndZ));
        //}

        //Destroy(gameObject, 1.5f);
        Destroy (gameObject);
    }

}