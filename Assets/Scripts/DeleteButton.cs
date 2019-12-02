using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeleteButton : MonoBehaviour
{
    // Start is called before the first frame update
    public void startDelete()
    {
        gameObject.GetComponent<Image>().color = Color.red;
        GameObject.Find("Molecule").GetComponent<MoleculeBehaviour>().setMode(MoleculeBehaviour.Mode.Delete);
    }

    public static void resetDeleteButton()
    {
        foreach (Image image in GameObject.Find("Canvas/DeleteButton").GetComponents<Image>())
        {
            image.color = Color.white;
        }
    }
}
