
using UnityEngine;
using Antymology.Terrain;

namespace Antymology.Agents
{

    public class Ant : MonoBehaviour
    {
        /// <summary>
        /// Health of the ant, when it reaches 0 the ant will be destroyed.
        /// </summary>
        public int health = 10000;
        /// <summary>
        /// How fast the health decreases per simulation tick.
        /// </summary>
        public int healthDecayRate = 1;
         
        /// <summary>
        /// How long in seconds between each simulation tick.
        /// </summary>
        public float simulationTickRate = 1f;

        /// <summary>
        /// How many simulation ticks have elapsed since the ant was created.
        /// </summary>
        private int ticksElapsed = 0;

        /// <summary>
        /// The raw data of the underlying world structure.
        /// </summary>
        public WorldManager worldManager;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            // Start up the main loop, we dont want this once per frame so we can assign a custom tick rate.
            InvokeRepeating("tickSimulation", 1f, simulationTickRate);
        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// The main loop of the ant, this is where all the logic for the ant will go. This is called once per simulation tick, which is determined by the simulationTickRate variable.
        /// </summary>
        void tickSimulation()
        {
            manageHealth();
            Debug.Log("" + ticksElapsed);

            // For now have ants move forward, right, forward, left, then repeat. This is just to have some movement in the simulation, this will be replaced with more complex behavior in the future.
            if (ticksElapsed % 4 == 0 || ticksElapsed % 4 == 2)
            {
                forward();
            }

            if (ticksElapsed % 4 == 1)
            {
                turnRight();
            }

            if (ticksElapsed % 4 == 3)
            {
                turnLeft();
            }

            // Debug.Log(health);
            ticksElapsed++;
        }
        
        void manageHealth()
        {
            health -= healthDecayRate;
            if (health <= 0)
            {
                Destroy(gameObject);
            } 

        }

        void forward()
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
        /// Turns the ant 90 degrees to the right.
        /// </summary>
        void turnRight()
        {
            Vector3 currentRotation = transform.eulerAngles;
            transform.eulerAngles = new Vector3(currentRotation.x, currentRotation.y + 90, currentRotation.z);
        }

        /// <summary>
        /// Turns the ant 90 degrees to the left.
        /// </summary>
        void turnLeft()
        {
            Vector3 currentRotation = transform.eulerAngles;
            transform.eulerAngles = new Vector3(currentRotation.x, currentRotation.y - 90, currentRotation.z);
        }

        /// <summary>
        /// Looks at the block in front of the ant.
        /// </summary>
        /// <returns>AbstractBlock in front of the ant</returns>
        AbstractBlock lookForward()
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
        AbstractBlock lookForwardDown()
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
        AbstractBlock lookForwardDown2()
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
        AbstractBlock lookForwardUp()
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
        AbstractBlock lookBelow()
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
        /// Looks at the block to the right of the ant.
        /// </summary>
        /// <returns>AbstractBlock to the right of the ant</returns>
        AbstractBlock lookRight()
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
        AbstractBlock lookLeft()
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
