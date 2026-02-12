
using UnityEngine;
using Antymology.Terrain;

namespace Antymology.Agents
{

    public class Ant : MonoBehaviour
    {

        /// <summary>
        /// The raw data of the underlying world structure.
        /// </summary>
        public WorldManager worldManager;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            forward();
        }

        void forward()
        {
            Vector3 currentPosition = transform.position;
            AbstractBlock blockBelow = worldManager.GetBlock((int)currentPosition.x, (int)currentPosition.y, (int)currentPosition.z);
            // Debug.Log("Current position: " + currentPosition);
            Debug.Log("" + lookForward());

        }

        void turnRight()
        {
            
        }

        void turnLeft()
        {
            
        }

        /// <summary>
        /// Looks at the block in front of the ant.
        /// </summary>
        /// <returns>AbstractBlock in front of the ant</returns>
        AbstractBlock lookForward()
        {
            Vector3 currentPosition = transform.position + new Vector3(0, 1, 0);
            Vector3 forwardDirection = transform.forward;
            AbstractBlock blockInFront = worldManager.GetBlock(
                (int)(currentPosition.x + forwardDirection.x),
                (int)(currentPosition.y + forwardDirection.y),
                (int)(currentPosition.z + forwardDirection.z)
            );
            return blockInFront;
        }

    }
}
