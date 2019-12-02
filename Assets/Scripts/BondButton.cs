using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BondButton : MonoBehaviour
{
    public GameObject molecule;

    public void setBondMode(MoleculeBehaviour.Mode mode)
    {
        MoleculeBehaviour moleculeScript = molecule.GetComponent<MoleculeBehaviour>();
        moleculeScript.setMode(mode);
        gameObject.GetComponent<Image>().color = Color.red;
    }

    public void setSingleBondMode()
    {
        setBondMode(MoleculeBehaviour.Mode.SingleBondStage1);
    }

    public void setDoubleBondMode()
    {
        setBondMode(MoleculeBehaviour.Mode.DoubleBondStage1);
    }

    public void setTripleBondMode()
    {
        setBondMode(MoleculeBehaviour.Mode.TripleBondStage1);
    }

    public void setElectronMode()
    {
        setBondMode(MoleculeBehaviour.Mode.ElectronsStage1);
    }

    public void setPositiveMode()
    {
        setBondMode(MoleculeBehaviour.Mode.PositiveStage);
    }

    public void setNegativeMode()
    {
        setBondMode(MoleculeBehaviour.Mode.NegativeStage);
    }

    public static void resetBondButtons()
    {
        foreach (Image image in GameObject.Find("Canvas/BondToggles").GetComponentsInChildren<Image>())
        {
            image.color = Color.white;
        }
    }
}
