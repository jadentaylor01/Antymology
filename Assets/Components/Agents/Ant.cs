
using UnityEngine;
using Antymology.Terrain;
using System.Configuration.Assemblies;

namespace Antymology.Agents
{

    public class Ant : MonoBehaviour
    {
        /// <summary>
        /// Health of the ant, when it reaches 0 the ant will be destroyed.
        /// </summary>
        public int currentHealth = 10000;
        /// <summary>
        /// How fast the health decreases per simulation tick.
        /// </summary>
        
        /// <summary>
        /// The max health of the ant.
        /// </summary>
        public int maxHealth = 10000;

        public int healthDecayRate = 1;

        /// <summary>
        /// A multiplier so that health can decay faster when the ant is in an acidic region.
        /// </summary>
        public int healthDecayMultiplier = 1;

        /// <summary>
        /// How much health is restored when the ant consumes a mulch block.
        /// </summary>
        public int foodHealRate = 100;
         
        /// <summary>
        /// How long in seconds between each simulation tick.
        /// </summary>
        public float simulationTickRate = 1f;

        /// <summary>
        /// How many simulation ticks have elapsed since the ant was created.
        /// </summary>
        public int ticksElapsed = 0;

        /// <summary>
        /// True if this ant is on the same block as another ant. We set this with collider triggers.
        /// </summary>
        public bool overlappingWithOtherAnt = false;


        /// <summary>
        /// The raw data of the underlying world structure.
        /// </summary>
        public WorldManager worldManager;

        public bool userControlsEnabled = false;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            // Start up the main loop, we dont want this once per frame so we can assign a custom tick rate.
            InvokeRepeating("tickSimulation", 1f, simulationTickRate);
            currentHealth = maxHealth;
        }

        void OnTriggerStay(Collider insideCollider) {
            if (insideCollider.CompareTag("Ant")) {
                overlappingWithOtherAnt = true;
            }
        }

        void OnTriggerExit(Collider exitedCollider) {
            if (exitedCollider.CompareTag("Ant")) {
                overlappingWithOtherAnt = false;
            }
        }

        void Update()
        {
            if (userControlsEnabled)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    forward();
                }
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    turnRight();
                }
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    turnLeft();
                }
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    consumeMulchBlockBelow();
                }
            }
        }

        /// <summary>
        /// The main loop of the ant, this is where all the logic for the ant will go. This is called once per simulation tick, which is determined by the simulationTickRate variable.
        /// </summary>
        void tickSimulation()
        {   
            tryDown(); // Make sure ants don't float if another ant dug below.
            checkAcid();
            damage(healthDecayRate * healthDecayMultiplier);

            Debug.Log("" + ticksElapsed);

            // For now have ants move forward, right, forward, left, then repeat. This is just to have some movement in the simulation, this will be replaced with more complex behavior in the future.
            // if (ticksElapsed % 4 == 0 || ticksElapsed % 4 == 2)
            // {
            //     forward();
            // }

            // if (ticksElapsed % 4 == 1)
            // {
            //     turnRight();
            // }

            // if (ticksElapsed % 4 == 3)
            // {
            //     consumeMulchBlockBelow();
            // }

            // Debug.Log(health);
            ticksElapsed++;
        }
        
        /// <summary>
        /// Manages the health of the ant, decreasing it by the healthDecayRate each simulation tick and destroying the ant if its health reaches 0 or below.
        /// </summary>
        public void damage(int damageAmount = 0)
        {
            currentHealth -= damageAmount;
            if (currentHealth <= 0)
            {
                Destroy(gameObject);
            } 

        }

        public void heal(int healAmount)
        {
            currentHealth += healAmount;
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
        }

        /// <summary>
        /// Changes the healthDecayMultiplier so that the ant loses health faster when it is on an acidic block.
        /// </summary>
        public void checkAcid()
        {
            if (lookBelow() is AcidicBlock)
            {
                healthDecayMultiplier = 2;
            }
            else
            {
                healthDecayMultiplier = 1;
            }
        }

        /// <summary>
        /// Consumes the block below the ant. The block must be mulch and there can only be one ant on the block that is being consumed.
        /// </summary>
        public void consumeMulchBlockBelow()
        {
            if (lookBelow() is MulchBlock && !overlappingWithOtherAnt)
            {
                dig();
                heal(foodHealRate);
            }
        }

        /// <summary>
        /// Digs the block below the ant, replacing it with an air block. The ant will then try to move down.
        /// </summary>
        public void dig()
        {
            if (lookBelow() is not ContainerBlock)
            {
                Vector3 currentPosition = transform.position;
                WorldManager.Instance.SetBlock((int)currentPosition.x, (int)currentPosition.y, (int)currentPosition.z, new AirBlock());
                tryDown();
            }
        }

        /// <summary>
        /// Moves the ant forward if possible. The ant will try to move forward, if it cannot move forward it will try to move forward and down, if it cannot move forward and down it will try to move forward and up, if it cannot move in any of those directions it will not move at all.
        /// </summary>
        public void forward()
        {
            // Check to find if the ant needs to move forward, forward down, forward up, or if it cannot move at all
            if (lookForward() is AirBlock) {
                // The block forward is air
                if (lookForwardDown() is AirBlock)
                {
                    // There is no block below the forward block, can only move if there is a block below it
                    if (!(lookForwardDown2() is AirBlock))
                    {
                        // There is a block to move on to, can move forward and down
                        transform.position += transform.forward;
                        transform.position += new Vector3(0, -1, 0);
                    } 
                } else
                {
                    // There is a block below the forward block, can move forward
                    transform.position += transform.forward;
                }
            } else {
                // The block forward is not air, check if it can move up
                if (lookForwardUp() is AirBlock) {
                    // There is not a block above the forward block, can move forward and up
                    transform.position += transform.forward;
                    transform.position += new Vector3(0, 1, 0);
                    
                }
            }
        }

        /// <summary>
        /// Tries to move the ant down, the ant can only move down if there is an air block below it. Ants must dig first if they want to move down on a block that is not air.
        /// </summary>
        public void tryDown()
        {
            if (lookBelow() is AirBlock)
            {
                transform.position += new Vector3(0, -1, 0);
            }
        }

        public void tryUp()
        {
            if (lookUp() is AirBlock)
            {
                transform.position += new Vector3(0, 1, 0);
            }
        }

        /// <summary>
        /// Turns the ant 90 degrees to the right.
        /// </summary>
        public void turnRight()
        {
            Vector3 currentRotation = transform.eulerAngles;
            transform.eulerAngles = new Vector3(currentRotation.x, currentRotation.y + 90, currentRotation.z);
        }

        /// <summary>
        /// Turns the ant 90 degrees to the left.
        /// </summary>
        public void turnLeft()
        {
            Vector3 currentRotation = transform.eulerAngles;
            transform.eulerAngles = new Vector3(currentRotation.x, currentRotation.y - 90, currentRotation.z);
        }

        /// <summary>
        /// Looks at the block in front of the ant.
        /// </summary>
        /// <returns>AbstractBlock in front of the ant</returns>
        public AbstractBlock lookForward()
        {
            
            Vector3 currentPosition = transform.position + new Vector3(0, 1, 0);
            Vector3 blockOffset = transform.forward;
            Vector3 forwardBlockPosition = currentPosition + blockOffset;
            AbstractBlock blockForward = worldManager.GetBlock(
                (int)forwardBlockPosition.x,
                (int)forwardBlockPosition.y,
                (int)forwardBlockPosition.z
            );
            return blockForward;
        }

        /// <summary>
        /// Looks at the block forward and down 1 from the ant.
        /// </summary>
        /// <returns>AbstractBlock forward and down 1 from the ant</returns>
        public AbstractBlock lookForwardDown()
        {
            Vector3 currentPosition = transform.position + new Vector3(0, 1, 0);
            Vector3 blockOffset = transform.forward - new Vector3(0, 1, 0);
            Vector3 forwardDownBlockPosition = currentPosition + blockOffset;
            AbstractBlock blockForwardDown = worldManager.GetBlock(
                (int)forwardDownBlockPosition.x,
                (int)forwardDownBlockPosition.y,
                (int)forwardDownBlockPosition.z
            );
            return blockForwardDown;
        }

        /// <summary>
        /// Looks at the block forward and down 2 from the ant.
        /// </summary>
        /// <returns>AbstractBlock forward and down 2 from the ant</returns>
        public AbstractBlock lookForwardDown2()
        {
            Vector3 currentPosition = transform.position + new Vector3(0, 1, 0);
            Vector3 blockOffset = transform.forward - new Vector3(0, 2, 0);
            Vector3 forwardBlockDown2Position = currentPosition + blockOffset;
            AbstractBlock blockForwardDown2 = worldManager.GetBlock(
                (int)forwardBlockDown2Position.x,
                (int)forwardBlockDown2Position.y,
                (int)forwardBlockDown2Position.z
            );
            return blockForwardDown2;
        }

        /// <summary>
        /// Looks at the block forward and up 1 from the ant.
        /// </summary>
        /// <returns>AbstractBlock forward and up 1 from the ant</returns>
        public AbstractBlock lookForwardUp()
        {
            Vector3 currentPosition = transform.position + new Vector3(0, 1, 0);
            Vector3 blockOffset = transform.forward + new Vector3(0, 1, 0);
            Vector3 forwardUpBlockPosition = currentPosition + blockOffset;
            AbstractBlock blockForwardUp = worldManager.GetBlock(
                (int)forwardUpBlockPosition.x,
                (int)forwardUpBlockPosition.y,
                (int)forwardUpBlockPosition.z
            );
            return blockForwardUp;
        }

        /// <summary>
        /// Looks at the block below the ant.
        /// </summary>
        /// <returns>AbstractBlock below the ant</returns>
        public AbstractBlock lookBelow()
        {
            Vector3 currentPosition = transform.position;
            AbstractBlock blockBelow = worldManager.GetBlock(
                (int)currentPosition.x, 
                (int)currentPosition.y, 
                (int)currentPosition.z
            );
            return blockBelow;
        }

        /// <summary>
        /// Looks at the block above the ant.
        /// </summary>
        /// <returns>AbstractBlock above the ant</returns>
        public AbstractBlock lookUp()
        {
            Vector3 currentPosition = transform.position + new Vector3(0, 2, 0);
            AbstractBlock blockUp = worldManager.GetBlock(
                (int)currentPosition.x, 
                (int)currentPosition.y, 
                (int)currentPosition.z
            );
            return blockUp;
        }

        /// <summary>
        /// Looks at the block to the right of the ant.
        /// </summary>
        /// <returns>AbstractBlock to the right of the ant</returns>
        public AbstractBlock lookRight()
        {
            Vector3 currentPosition = transform.position + new Vector3(0, 1, 0);
            Vector3 blockOffset = transform.right;
            Vector3 rightBlockPosition = currentPosition + blockOffset;
            AbstractBlock blockRight = worldManager.GetBlock(
                (int)rightBlockPosition.x,
                (int)rightBlockPosition.y,
                (int)rightBlockPosition.z
            );
            return blockRight;
        }

        /// <summary>
        /// Looks at the block to the left of the ant.
        /// </summary>
        /// <returns>AbstractBlock to the left of the ant</returns>
        public AbstractBlock lookLeft()
        {
            Vector3 currentPosition = transform.position + new Vector3(0, 1, 0);
            Vector3 blockOffset = -transform.right;
            Vector3 leftBlockPosition = currentPosition + blockOffset;
            AbstractBlock blockLeft = worldManager.GetBlock(
                (int)leftBlockPosition.x,
                (int)leftBlockPosition.y,
                (int)leftBlockPosition.z
            );
            return blockLeft;
        }

    }
}
