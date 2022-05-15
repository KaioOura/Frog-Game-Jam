using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BellyDisplay : MonoBehaviour
{
    public Image[] bellySlot;
    public BellyFrog bellyFrog;

    public GameObject mealBG;
    public Image mealImage;

    public Image ballon;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateMealUI(Meal meal = null)
    {
        if (meal == null)
        {
            mealBG.SetActive(false);
            return;
        }

        mealBG.SetActive(true);

        mealImage.sprite = meal.image;
    }

    public void UpdateUI()
    {
        if (bellyFrog.belly.Count < 1)
        {
            ballon.enabled = false;
            foreach (var item in bellySlot)
            {
                item.gameObject.SetActive(false);
            }

            return;
        }

        ballon.enabled = true;

        for (int i = 0; i < bellyFrog.belly.Count; i++)
        {
            bellySlot[i].gameObject.SetActive(true);
            bellySlot[i].sprite = bellyFrog.belly[i].myImage;
        }
    }
}
