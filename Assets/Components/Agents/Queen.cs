using UnityEngine;

namespace Antymology.Agents
{
    public class QueenAnt : Ant
    {
        void tickSimulation()
        {
            tryDown(); // Make sure ants don't float if another ant dug below.
            checkAcid();
            manageHealth();

            forward();

            ticksElapsed++;
        }
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}

