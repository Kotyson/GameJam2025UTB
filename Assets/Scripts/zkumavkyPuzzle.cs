using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class zkumavkyPuzzle : MonoBehaviour
{
    public UnityEvent puzzleSolved;

    public int goodPositions = 0;
    public int badPositions = 0;

    public enum LightState
    {
        Off,
        On,
        Neutral
    }

    [Serializable]
    public class zkumavka
    {
        public Light[] lightPair;
        public LightState state = LightState.Neutral;
        public leverInteraction[] lever;
    }

    public List<zkumavka> zkumavky;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CheckAllLeverStates();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CheckAllLeverStates()
    {
        getAllLeverStates();
        goodPositions = 0;
        badPositions = 0;

        if (zkumavky == null || zkumavky.Count == 0)
        {
            Debug.LogWarning("No zkumavky defined!");
            return;


        }

        // Track the total number of levers we expect
        int expectedTotalLevers = 0;

        foreach (zkumavka zk in zkumavky)
        {
            if (zk.lever == null)
                continue;

            expectedTotalLevers += zk.lever.Length;

            foreach (leverInteraction lever in zk.lever)
            {
                if (lever == null)
                    continue;

                // Use the correct enum from the global scope (LeverState)
                if (lever.state == LeverState.On)
                {
                    goodPositions++;
                    Debug.Log($"Lever {lever.name} is ON, good positions: {goodPositions}");
                }
                else if (lever.state == LeverState.Off)
                {
                    badPositions++;
                    Debug.Log($"Lever {lever.name} is OFF, bad positions: {badPositions}");
                }
                else
                {
                    Debug.Log($"Lever {lever.name} is NEUTRAL");
                    // Neutral levers don't affect the counts
                }
            }

        }
        checkLightStates();

        // Check if puzzle is solved
        if (goodPositions == zkumavky.Count && badPositions == 0)
        {
            // All zkumavky are good!
            CommandManager.instance.changeMeter(-1);
            Debug.Log("All zkumavky are correctly configured!");
        }



    }

    public void lightsOn(zkumavka zk, Color color)
    {
        foreach (Light light in zk.lightPair)
        {
            light.enabled = true;
            light.color = color;
        }
    }

    public void lightsOff(zkumavka zk)
    {
        foreach (Light light in zk.lightPair)
        {
            light.enabled = false;
        }
    }

    public void checkLightStates()
    {
        if (goodPositions >= 2 || badPositions >= 2)
        {
            if (goodPositions >= 2)
            {
                foreach (zkumavka zk in zkumavky)
                {
                    foreach (leverInteraction lv in zk.lever)
                    {
                        if (lv.state == LeverState.On)
                        {
                            lightsOn(zk, Color.green);
                        }
                    }

                }
            }
            else
            {
                foreach (zkumavka zk in zkumavky)
                {
                    foreach (leverInteraction lv in zk.lever)
                    {
                        if (lv.state == LeverState.Off)
                        {
                            lightsOn(zk, Color.red);
                        }
                    }

                }
            }
        }
        else
        {
            foreach (zkumavka zk in zkumavky)
            {
                lightsOff(zk);
            }
        }
    }

    public void getAllLeverStates()
    {
        string str = "";
        foreach (zkumavka zk in zkumavky)
        {
            foreach (leverInteraction lever in zk.lever)
            {
                str += lever.state.ToString() + " ";
            }
            str += "\n";
        }
        Debug.Log(str);
    }

    public void leverInteraction()
    {
        Debug.Log("Lever interaction zkumavky");
        CheckAllLeverStates();
    }
}
