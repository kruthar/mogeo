using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhaseToggle : MonoBehaviour
{
    private MoleculeBehaviour molecule;
    private GameObject atomToggles;
    private GameObject bondToggles;

    public void Start()
    {
        molecule = GameObject.Find("Molecule").GetComponent<MoleculeBehaviour>();
        atomToggles = GameObject.Find("AtomToggles");
        bondToggles = GameObject.Find("BondToggles");
    }

    public void buildPhase()
    {
        molecule.setPhase(MoleculeBehaviour.Phase.Build);
        highlightThisButton();
        atomToggles.SetActive(true);
        bondToggles.SetActive(true);

        molecule.centerMolecule();
    }

    public void adjustPhase()
    {
        molecule.setPhase(MoleculeBehaviour.Phase.Adjust);
        highlightThisButton();
        atomToggles.SetActive(false);
        bondToggles.SetActive(false);
    }

    private void highlightThisButton()
    {
        foreach (PhaseToggle toggle in transform.parent.GetComponentsInChildren<PhaseToggle>())
        {
            toggle.GetComponentInChildren<Image>().color = Color.white;
        }

        this.GetComponentInChildren<Image>().color = Color.red;
    }
}
