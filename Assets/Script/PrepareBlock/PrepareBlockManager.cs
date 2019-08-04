using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PrepareBlockManager : MonoBehaviour {

    public static PrepareBlockManager instance;


    public Button[] blocks;

	// Use this for initialization
	void Start () {
        //Check if instance already exists
        if (instance == null)
        {
            //if not, set instance to this
            instance = this;
        }

        //If instance already exists and it's not this:
        //else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            //Destroy(gameObject);
    }


    public Button GetBlockSlot(int index) {
        if(index < blocks.Length)
            return blocks[index];

        return null;
    }
}
