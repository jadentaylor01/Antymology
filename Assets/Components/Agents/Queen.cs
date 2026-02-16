using UnityEngine;
using Antymology.Terrain;

namespace Antymology.Agents
{
    public class QueenAnt : Ant
    {
        void tickSimulation()
        {
            tryDown(); // Make sure ants don't float if another ant dug below.
            checkAcid();
            damage(healthDecayRate * healthDecayMultiplier);

            forward();
            if (currentHealth > maxHealth * 0.5f) {
                createNestBlock();
            } else
            {
                consumeMulchBlockBelow();
            }
            

            ticksElapsed++;
        }
        
        /// <summary>
        /// Queen ants can create a nest block where they standing, which will spawn new ants.
        /// They will place the nest down and then move on top of it.
        /// </summary>
        public void createNestBlock()
        {
            if (lookUp() is AirBlock)
            {
                // TODO: Check if there is an another ant on the block.
                // Can only create a nest if there is air above the ant to prevent ants from creating nests that would put them inside the block
                Vector3 currentPosition = transform.position + new Vector3(0, 1, 0);
                WorldManager.Instance.SetBlock((int)currentPosition.x, (int)currentPosition.y, (int)currentPosition.z, new NestBlock());
                damage(maxHealth / 3); // Reduce health by 1/3 when creating a nest.
                tryUp();
            }
            
        }
    }
}

