using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterButton : MonoBehaviour
{
    public void center()
    {
        GameObject.Find("Molecule").GetComponent<MoleculeBehaviour>().centerMolecule();
    }
}
