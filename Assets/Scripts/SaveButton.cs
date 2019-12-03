using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Web;

public class SaveButton : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void initialize();

    [DllImport("__Internal")]
    private static extern void showSaveURL(string str);

    // Start is called before the first frame update
    public void save()
    {
        string saveURL = "https://kruthar.github.io/mogeo/index.html?save=" + GameObject.Find("Molecule").GetComponent<MoleculeBehaviour>().serializeMolecule();
        showSaveURL(saveURL);
    }

    void Start()
    {
        initialize();
    }
}
