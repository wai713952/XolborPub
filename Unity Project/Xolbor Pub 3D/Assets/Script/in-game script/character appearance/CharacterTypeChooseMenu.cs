using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterTypeChooseMenu : MonoBehaviour
{
    public List<GameObject> headTypeButton = new List<GameObject>();
    public List<GameObject> bodyTypeButton = new List<GameObject>();

    public List<GameObject> bodyTypeObject = new List<GameObject>();

    public int headTypeIndex;
    public int bodyTypeIndex;

    public string buttonName;

    public void ChooseHeadTypeButton()    // tied to head choice button
    {
        buttonName = EventSystem.current.currentSelectedGameObject.name;

        for(int i = 0; i < headTypeButton.Count; i++)
        {
            if (buttonName == headTypeButton[i].name)
            {
                headTypeIndex = i;
                break;
            }
        }
    }

    public void ChooseBodyTypeButton()    // tied to head choice button
    {
        buttonName = EventSystem.current.currentSelectedGameObject.name;

        for (int i = 0; i < bodyTypeButton.Count; i++)
        {
            if (buttonName == bodyTypeButton[i].name)
            {
                bodyTypeIndex = i;
                break;
            }
        }
    }
}
