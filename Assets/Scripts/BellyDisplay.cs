using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BellyDisplay : MonoBehaviour
{
    public Image[] bellySlot;
    public BellyFrog bellyFrog;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateUI()
    {
        if (bellyFrog.belly.Count < 1)
        {
            foreach (var item in bellySlot)
            {
                item.gameObject.SetActive(false);
            }

            return;
        }

        for (int i = 0; i < bellyFrog.belly.Count; i++)
        {
            bellySlot[i].gameObject.SetActive(true);
            bellySlot[i].sprite = bellyFrog.belly[i].myImage;
        }
    }
}
