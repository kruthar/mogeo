using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Web;

public class BrowserScript : MonoBehaviour
{
    public MoleculeBehaviour molecule;

    [DllImport("__Internal")]
    private static extern void initialize();

    [DllImport("__Internal")]
    private static extern void showSaveURL(string str);

    // Start is called before the first frame update
    public void click()
    {
        string saveURL = "https://kruthar.github.io/mogeo/index.html?save=" + molecule.serializeMolecule();
        showSaveURL(saveURL);
    }

    void Start()
    {
        initialize();
    }
}
