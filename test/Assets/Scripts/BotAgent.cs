using MLAgents;
using UnityEngine;

public class BotAgent : Agent {

    [SerializeField]
    private float maxHealth = 200f;

    [SerializeField]
    private float health;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private PlayerController controller;

    private Vector3 spawnPosition;

    public Vector3 SpawnPosition {
        get { return spawnPosition;  }
        internal set { spawnPosition = value; farthestDistanceFromSpawn = 0; }
    }

    internal Quaternion spawnRotation;

    private float Health {
        get { return health; }
        set {
            health = value;
            if (health <= 0) {
                health = 0;
                Die();
            }
            else if (value < health) {//we took damage, but didn't die. Just a small penalty
                AddReward(-.2f);
            }
        }
    }

    [SerializeField]//selected item in hotbar
    private GameObject currentItem;

    private TrainingArea trainingArea;

    private GameObject CurrentItem {
        get { return currentItem; }
        set {
            currentItem = value;
            currentWeapon = currentItem.GetComponent<Weapon>();
        }
    }

    private Weapon currentWeapon;

    private float farthestDistanceFromSpawn;//the record distance the bot has traveled from its spawn position. used to reward bots when the venture from the spawn position to find the other bots

    // Start is called before the first frame update
    void Start() {
        spawnRotation = transform.localRotation;
        Health = maxHealth;
        CurrentItem = currentItem;
        currentWeapon.WeaponHolder = this;
        trainingArea = GetComponentInParent<TrainingArea>();
    }

    private void Die() {
        animator.SetTrigger("Die");
        AddReward(-1);
        Done();
    }

    //called by the ML Agents system, so although its backwards, this is the only place we find out that our training run has ended (when the reward value leaves the threshold). 
    //Because of this, we need to reset the whole area here and all other agents in it
    public override void AgentReset() {
        Debug.Log("RESETTING AGENT");
        currentWeapon.Ammo = currentWeapon.MaxAmmo;
        Health = maxHealth;
        trainingArea.ResetArea(this);
    }

    public override void AgentAction(float[] vectorAction, string textAction) {
        //vectorAction[0] = horizontal
        //vectorAction[1] = vertical
        //vectorAction[2] = mousex
        //vectorAction[3] = mousey
        //vectorAction[4] = sprint
        //vectorAction[5] = aim
        //vectorAction[6] = jump
        //vectorAction[7] = shoot
        //vectorAction[8] = reload

        bool sprint = vectorAction[4] > 0 ? true : false;//more than 0 the ai wants to sprint, less than it wants to walk
        bool aim = vectorAction[5] > 0 ? true : false;//more than 0 the ai wants to aim, less than it wants to not
        bool jump = vectorAction[6] > 0 ? true : false;//more than 0 the ai wants to jump, less than it wants to not
        bool shoot = vectorAction[7] > 0 ? true : false;//more than 0 the ai wants to shoot, less than it wants to not
        bool reload = vectorAction[8] > 0 ? true : false;//more than 0 the ai wants to reload, less than it wants to not

        if (shoot) {
            //Debug.Log("The bot is trying to shoot!");
        }

        controller.SetInput(Mathf.Clamp(vectorAction[0], -1, 1), Mathf.Clamp(vectorAction[1], -1, 1), Mathf.Clamp(vectorAction[2], -1, 1), Mathf.Clamp(vectorAction[3], -1, 1), sprint, aim, jump);

  /*      foreach (BotAgent agent in FindObjectsOfType<BotAgent>()) {
            if (agent != this) {
                controller.playerHead.transform.LookAt(agent.transform.position);
            }
        }*/

        if (currentWeapon != null) {
            if (shoot) {
                currentWeapon.Fire();
            }
            if (reload) {
                //add a penalty if the bot reloads before finishing the clip
                float reloadReward = -1f + (1 / (currentWeapon.Ammo + 1));
                // -1 + (2 / 22) = -0.90909090909 penalty while reloading while full
                // -1 + (2 / 1) = 1 reward reloading while empty

                reloadReward /= 2;//little less, should come out to near -.5 or .5 instead of -1 and 1
                (currentWeapon.WeaponHolder as BotAgent).AddReward(reloadReward);
                currentWeapon.Reload();
            }
        }

        AddReward(-1f / agentParameters.maxStep);

        float distanceToSpawn = Vector3.Distance(spawnPosition, transform.position);

        if (distanceToSpawn > farthestDistanceFromSpawn) {
            AddReward(1f / agentParameters.maxStep);
            farthestDistanceFromSpawn = distanceToSpawn;
        }

        float closestAngle = 361;//The  angle between the Agent that is closest to the center of this agent's vision found so far
        foreach (BotAgent agent in trainingArea.team1) {
            float angle = Vector3.Angle(transform.forward,agent.transform.position - transform.position);
            if (angle < closestAngle && agent != this) {
                closestAngle = angle;
            }
        }
        foreach (BotAgent agent in trainingArea.team2) {
            float angle = Vector3.Angle(transform.forward, agent.transform.position - transform.position);
            if (angle < closestAngle && agent != this) {
                closestAngle = angle;
            }
        }

        //aim reward algorithm
        float reward = CalculateAimReward(closestAngle) * 4;

        //Debug.Log("How close bot " + gameObject.name + " is looking at another bot: " + closestAngle + ". Reward for this angle: " + reward);

        AddReward(reward / agentParameters.maxStep);//10th of reward

    }

    //scales angle down to a reward instead of it ranging from 0-180
    private float CalculateAimReward(float angleToTarget) {
        //Depending on where angleToTarget is between minAngle and maxAngle, this function will return a reward up to minReward * -1
        float minAngle = 0;
        float maxAngle = 180;

        float minReward = -1;
        float maxReward = 2;

        float reward = (angleToTarget - minAngle) / (maxAngle - minAngle) * (maxReward - minReward) + minReward;

        return reward * -1;//inverted reward, should return 1 while the angleToTarget is 0 and -1 while angleToTarget is 360
    }

    public override void CollectObservations() {
        AddVectorObs(transform.position.x);
        AddVectorObs(transform.position.y);
        AddVectorObs(transform.position.z);

        AddVectorObs(currentWeapon.Ammo);
    }

    internal void Damage(float damage) {
        Debug.Log("Damaging");
        Health -= damage;
    }

    public override float[] Heuristic() {
        return base.Heuristic();
    }

}
