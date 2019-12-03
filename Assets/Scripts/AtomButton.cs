using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AtomButton : MonoBehaviour
{
    public void newAtom(string atomType)
    {
        GameObject molecule = GameObject.Find("Molecule");
        Vector3 newLoc = molecule.transform.position;
        if (existsCenteredAtom())
        {
            newLoc = new Vector3(Random.Range(-.5f, .5f), Random.Range(-.5f, .5f), newLoc.z);
        }

        AtomBehaviour atomScript = AtomBehaviour.instantiateAtom(newLoc, atomType);
    }

    public bool existsCenteredAtom()
    {
        foreach (AtomBehaviour atom in FindObjectsOfType<AtomBehaviour>())
        {
            if (atom.transform.position.Equals(new Vector3(0, 0, 0)))
            {
                return true;
            }
        }

        return false;
    }
}
