using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AtomBehaviour : MonoBehaviour
{
    private static int idcounter = 0;
    public int id;
    public string atomType = null;
    public bool followMouse = false;

    private HashSet<BondBehaviour> bonds = new HashSet<BondBehaviour>();
    private MoleculeBehaviour molecule;
    private int charge = 0;

    public void initializeAtom(string atype)
    {
        initializeAtom(atype, 0);
    }

    public void initializeAtom(string atype, int acharge)
    {
        Debug.Log("initing " + atype);
        id = ++idcounter;
        atomType = atype;
        charge = acharge;
        molecule = GameObject.Find("Molecule").GetComponent<MoleculeBehaviour>();
        Debug.Log("molecule: " + molecule);
        Debug.Log("setting color");
        setAtomColor();
        Debug.Log("setting text");
        gameObject.GetComponentInChildren<TextMeshPro>().SetText(atype);
        Debug.Log("setting charge");
        setAtomChargeText();
        Debug.Log("done init");

    }

    // Based on this Atom's type, set is material color
    public void setAtomColor() { 
        Renderer mr = gameObject.GetComponent<Renderer>();
        gameObject.GetComponentInChildren<TextMeshPro>().color = Color.black;

        switch (atomType)
        {
            // z is used for invisible anchors when saving Electrions
            case "z":
                mr.enabled = false;
                break;
            case "H":
                mr.material.color = Color.white;
                break;
            case "C":
                mr.material.color = Color.grey;
                break;
            case "N":
                mr.material.color = Color.blue;
                break;
            case "O":
                mr.material.color = Color.red;
                break;
            case "F":
                mr.material.color = Color.black;
                gameObject.GetComponentInChildren<TextMeshPro>().color = Color.white;
                break;
            case "Na":
                mr.material.color = Color.cyan;
                break;
            case "P":
                mr.material.color = new Color(1, 0.644f, 0);
                break;
            case "S":
                mr.material.color = Color.magenta;
                break;
            case "Cl":
                mr.material.color = Color.green;
                break;
        }
    }

    // Change this Atom's material color to the highlight color
    public void setAtomHighlight()
    {
        Renderer mr = gameObject.GetComponent<Renderer>();

        mr.material.color = Color.yellow;
        gameObject.GetComponentInChildren<TextMeshPro>().color = Color.black;
    }

    private void moveToMouse()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.WorldToScreenPoint(transform.position).z;  
        transform.position = Camera.main.ScreenToWorldPoint(mousePos);
    }

    public void removeBond(BondBehaviour bond)
    {
        bonds.Remove(bond);
    }

    public void addBond(BondBehaviour bond)
    {
        bonds.Add(bond);
    }

    // Update is called once per frame
    void Update()
    {
        // When Atoms are first created they are set with followMouse = true,
        // during this time they should stay under the mouse cursor until the 
        // primary mouse button is clicked, then they will be locked into place.
        if (followMouse)
        {
            moveToMouse();

            if (Input.GetMouseButtonDown(0))
            {
                followMouse = false;
            }
        }

        // Always face towards the camera so that the Text Label can be seen
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position, Camera.main.transform.up);
    }

    private void OnMouseDrag()
    {
        // When we drag over an atom (and we are in None mode), relocate to the mouse point
        // Then tell all attached anchors to restretch to the new location.
        if (molecule.getMode().Equals(MoleculeBehaviour.Mode.None) || molecule.getMode().Equals(MoleculeBehaviour.Mode.AdjustingAtom))
        {
            moveToMouse();
            foreach (BondBehaviour bond in bonds)
            {
                if (bond != null)
                {
                    bond.resetAnchorPositions();
                }
            }

            molecule.setMode(MoleculeBehaviour.Mode.AdjustingAtom);
        }
    }
    
    private void OnMouseOver()
    {
        if (!followMouse) //TODO: This should only go off during the bond or electron set mode
        {
            setAtomHighlight();
        }
    }

    private void OnMouseExit()
    {
        setAtomColor();
    }

    private void OnMouseDown()
    {
        if (molecule.getMode().Equals(MoleculeBehaviour.Mode.Delete))
        {
            destroySelf();
            DeleteButton.resetDeleteButton();
            molecule.setMode(MoleculeBehaviour.Mode.None);
        }
    }

    private void OnMouseUp()
    {
        GameObject newBond = null;
        switch (molecule.getMode())
        {
            case MoleculeBehaviour.Mode.SingleBondStage1: 
                newBond = Instantiate((GameObject)Resources.Load("Prefabs/SingleBond", typeof(GameObject)), transform.position, Quaternion.identity, molecule.transform);
                molecule.setMode(MoleculeBehaviour.Mode.SingleBondStage2);
                break;
            case MoleculeBehaviour.Mode.DoubleBondStage1:
                newBond = Instantiate((GameObject)Resources.Load("Prefabs/DoubleBond", typeof(GameObject)), transform.position, Quaternion.identity, molecule.transform);
                molecule.setMode(MoleculeBehaviour.Mode.DoubleBondStage2);
                break;
            case MoleculeBehaviour.Mode.TripleBondStage1:
                newBond = Instantiate((GameObject)Resources.Load("Prefabs/TripleBond", typeof(GameObject)), transform.position, Quaternion.identity, molecule.transform);
                molecule.setMode(MoleculeBehaviour.Mode.TripleBondStage2);
                break;
            case MoleculeBehaviour.Mode.ElectronsStage1:
                newBond = Instantiate((GameObject)Resources.Load("Prefabs/Electrons", typeof(GameObject)), transform.position, Quaternion.identity, molecule.transform);
                molecule.setMode(MoleculeBehaviour.Mode.ElectronsStage2);
                break;
            case MoleculeBehaviour.Mode.PositiveStage:
                charge += 1;
                setAtomChargeText();
                break;
            case MoleculeBehaviour.Mode.NegativeStage:
                charge -= 1;
                setAtomChargeText();
                break;

        }

        if (newBond != null) 
        {
            BondBehaviour bondScript = newBond.GetComponent<BondBehaviour>();
            bondScript.initializeBond(this);
            bonds.Add(bondScript);
        }

        if (molecule.getMode().Equals(MoleculeBehaviour.Mode.AdjustingAtom))
        {
            molecule.setMode(MoleculeBehaviour.Mode.None);
        }
    }

    private void setAtomChargeText()
    {
        Debug.Log("setting charge");
        string chargeText = "";

        if (charge == 1)
        {
            chargeText = "<sup>+<sup>";
        }
        else if (charge > 1)
        {
            chargeText = "<sup>+" + charge + "<sup>";
        }
        else if (charge == -1)
        {
            chargeText = "<sup>-<sup>";
        }
        else if (charge < -1)
        {
            chargeText = "<sup>" + charge + "<sup>";
        }
        Debug.Log("found charge");
        gameObject.GetComponentInChildren<TextMeshPro>().SetText(atomType + chargeText);
        Debug.Log("done changing charge");
        Debug.Log("fetching molecule: " + GameObject.Find("Molecule").GetComponent<MoleculeBehaviour>());
        Debug.Log("none: " + MoleculeBehaviour.Mode.None);
        GameObject.Find("Molecule").GetComponent<MoleculeBehaviour>().setMode(MoleculeBehaviour.Mode.None);
        Debug.Log("done setting mode");
    }

    public Atom getAtom()
    {
        return new Atom(id, transform.position.x, transform.position.y, transform.position.z, atomType, charge);
    }

    public static AtomBehaviour instantiateAtom(Vector3 location, string type)
    {
        MoleculeBehaviour molecule = GameObject.Find("Molecule").GetComponent<MoleculeBehaviour>();
        Debug.Log("molecule: " + molecule);
        GameObject newAtom = Instantiate((GameObject)Resources.Load("Prefabs/Atom", typeof(GameObject)), location, Quaternion.identity, molecule.transform);
        Debug.Log("atom: " + newAtom);
        AtomBehaviour atomScript = newAtom.GetComponent<AtomBehaviour>();
        Debug.Log("atomscript: " + atomScript);
        atomScript.initializeAtom(type);
        return atomScript;
    }

    public void destroySelf()
    {
        foreach (BondBehaviour bond in bonds)
        {
            if (bond != null)
            {
                bond.destroySelf();
            }
        }

        Destroy(this.gameObject);
    }
}
