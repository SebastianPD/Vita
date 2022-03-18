using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Step;
using Step.Interpreter;

public class StepEngine : MonoBehaviour
{
    // Start is called before the first frame update
    Module mod;
    void Start()
    {
        mod = new Module("Module");
        //DebugTest();
        //MakeModule("Dialouge 1: Licking door knobs is illegal on other planets.");
        //MakeModule("Dialouge 2: It's dangerous to go alone. Please take this.");
        //MakeModule("Dialouge 3: supercalifragilisticexpialidocious");

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MakeModule(string x) 
    {
        mod.AddDefinitions(x);
    }

    public bool MakeACall(string x) 
    {
        try
        {
            mod.ParseAndExecute("[PlayerAction " + x + "]");
            //mod.CallPredicate("PlayerAction", c# object) to document objects.


            return true;
        }
        catch
        {
          
            return false;
        }


    }

    void DebugTest() 
    {
        var m = new Module("test");
        m.AddDefinitions("[randomly] Test: a", "Test: b", "Test: c ");
        var gotA = 0;
        var gotB = 0;
        var gotC = 0;

        for (var i = 0; i < 100; i++)
        {
            var s = m.Call("Test");
            switch (s)
            {
                case "A":
                    gotA++;
                    Debug.Log("a");
                    break;

                case "B":
                    gotB++;
                    Debug.Log("b");
                    break;

                case "C":
                    gotC++;
                    Debug.Log("c");
                    break;

                default:
                    Debug.Log("failed");
                    break;
            }
        }

        m.AddDefinitions(
              "[randomly] PlayerAction sit: you sat down",
              "PlayerAction item: you got an item",
              "Method ?x: [PlayerAction ?x]",
              "Test2: [Method sit]",
              "Test3: [Method ?x]");
              Debug.Log(m.ParseAndExecute("[PlayerAction ?x]"));
    }
}
