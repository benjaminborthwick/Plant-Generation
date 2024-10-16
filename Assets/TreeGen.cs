using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGen : MonoBehaviour
{
    [SerializeField]
    private int seed = 9183457;
    [SerializeField]
    private int iterations = 40;
    List<Bud> buds;
    List<Bud> budsToCull;
    List<Bud> budsToAdd;

    // Start is called before the first frame update
    void Start()
    {
        UnityEngine.Random.InitState(seed);
        buds = new List<Bud>();
        budsToCull = new List<Bud>();
        budsToAdd = new List<Bud>();
        // initialize trees
        for (int i = -1; i <= 1; i++) {
            for (int j = -1; j <= 1; j++) {
                buds.Add(new Bud(new Vector3(120 * i, 0, 120 * j), 0));
            }   
        }
        
        // update live buds
        int iter = 0;
        while (iter++ < iterations && buds.Count > 0) {
            foreach (Bud bud in buds) {
                bud.update(budsToCull, budsToAdd);
            }
            while (budsToCull.Count > 0) {
                budsToCull[0].Unalive(buds);
                budsToCull.RemoveAt(0);
            }
            while (budsToAdd.Count > 0) {
                buds.Add(budsToAdd[0]);
                budsToAdd.RemoveAt(0);
            }
        }
        while (buds.Count > 0) {
            buds[0].Unalive(buds);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
