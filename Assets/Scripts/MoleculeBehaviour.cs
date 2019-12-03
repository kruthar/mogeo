using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Web;

public class MoleculeBehaviour : MonoBehaviour {
    public float rotationSpeed = 10;
    public float zoomSpeed = 1;
    private GameObject instructions;
    private Mode mode = Mode.None;
    private Phase phase = Phase.Build;

    public enum Mode 
    {
        None,
        SingleBondStage1,
        SingleBondStage2,
        DoubleBondStage1,
        DoubleBondStage2,
        TripleBondStage1,
        TripleBondStage2,
        ElectronsStage1,
        ElectronsStage2,
        PositiveStage,
        NegativeStage,
        AdjustingAtom,
        Delete
    }

    public enum Phase
    {
        Build,
        Adjust
    }

    public void setMode(Mode mod)
    {
        GameObject instructions = GameObject.Find("Instructions");
        mode = mod;

        switch (mod)
        {
            case Mode.None:
                instructions.GetComponent<Text>().text = "You can rearrange your molecule by dragging the atoms and electrons around. Use the scroll wheel to zoom.";
                break;
            case Mode.SingleBondStage1:
                instructions.GetComponent<Text>().text = "Select the first atom to attach a single bond to.";
                break;
            case Mode.SingleBondStage2:
                instructions.GetComponent<Text>().text = "Select the second atom to attach a single bond to.";
                break;
            case Mode.DoubleBondStage1:
                instructions.GetComponent<Text>().text = "Select the first atom to attach a double bond to.";
                break;
            case Mode.DoubleBondStage2:
                instructions.GetComponent<Text>().text = "Select the second atom to attach a double bond to.";
                break;
            case Mode.TripleBondStage1:
                instructions.GetComponent<Text>().text = "Select the first atom to attach a triple bond to.";
                break;
            case Mode.TripleBondStage2:
                instructions.GetComponent<Text>().text = "Select the second atom to attach a triple bond to.";
                break;
            case Mode.ElectronsStage1:
                instructions.GetComponent<Text>().text = "Select an atom to attach a pair of electrons to.";
                break;
            case Mode.ElectronsStage2:
                instructions.GetComponent<Text>().text = "Select the directions the pair of electrons should face.";
                break;
            case Mode.PositiveStage:
                instructions.GetComponent<Text>().text = "Select an atom to give a positive charge.";
                break;
            case Mode.NegativeStage:
                instructions.GetComponent<Text>().text = "Select an atom to give a negative charge.";
                break;
            case Mode.AdjustingAtom:
                instructions.GetComponent<Text>().text = "Place the atom in your molecule.";
                break;
            case Mode.Delete:
                instructions.GetComponent<Text>().text = "Click on an Atom, Bond or set of Electrons to delete.";
                break;
        }
    }

    public Mode getMode()
    {
        return mode;
    }

    public void setPhase(Phase phas)
    {
        phase = phas;
        GameObject instructions = GameObject.Find("Instructions");

        switch (phas)
        {
            case Phase.Adjust:
                instructions.GetComponent<Text>().text = "Rotate you molecule by dragging the mouse around the screen. Adjust your bond angles by dragging the atoms and electrons around.";
                break;
            case Phase.Build:
                instructions.GetComponent<Text>().text = "Add more Atoms and Bonds to your molecule with the buttons on the right and left. Click save when you are finished.";
                break;
        }
    }

    public Phase getPhase()
    {
        return phase;
    }

    public void centerMolecule()
    {
        transform.rotation = Quaternion.identity; 
    }

    public void resetAllBonds()
    {
        foreach (BondBehaviour bond in gameObject.GetComponentsInChildren<BondBehaviour>())
        {
            bond.resetAnchorPositions();
        }
    }

    // Update is called once per frame
    void Update () {
        // Only apply movements when no other mode is turned on
        if (mode.Equals(Mode.None))
        {
            // When the Primary Mouse button is held down (Mouse Drag), then rotate the Molecule based on Mouse Movement
            if (phase.Equals(Phase.Adjust) && Input.GetMouseButton(0))
            {
                transform.Rotate(0, Input.GetAxis("Mouse X") * -rotationSpeed, 0, Space.World);
                transform.Rotate(Input.GetAxis("Mouse Y") * rotationSpeed, 0, 0, Space.World);
                resetAllBonds();
            }

            // Use the Mouse zoom input to move the molecule towards or away from the camera
            Vector3 curPos = transform.position;
            curPos.z += Input.mouseScrollDelta.y * zoomSpeed;
            transform.position = curPos;
        }

        // Always reset the atom/bond buttons no matter what we clicked on
        if (Input.GetMouseButtonDown(0))
        {
            BondButton.resetBondButtons();
        }
    }

    public string serializeMolecule()
    {
        return getMolecule().serialize();
    }

    public Molecule getMolecule()
    {
        // Bonds go first so that any missing "hidden" anchors on elctrons can be created
        BondBehaviour[] bondBehaviours = gameObject.GetComponentsInChildren<BondBehaviour>();
        Bond[] bonds = new Bond[bondBehaviours.Length];

        for (int i = 0; i < bonds.Length; i++)
        {
            bonds[i] = bondBehaviours[i].getBond();
        }

        AtomBehaviour[] atomBehaviours = gameObject.GetComponentsInChildren<AtomBehaviour>();
        Atom[] atoms = new Atom[atomBehaviours.Length];

        for (int i = 0; i < atoms.Length; i++)
        {
            atoms[i] = atomBehaviours[i].getAtom();
        }
        
        return new Molecule(atoms, bonds);
    }

    public void recreateMolecule(string encodedSave)
    {
        if (encodedSave != null)
        {
            Molecule molecule = Molecule.deserialize(encodedSave);
            Hashtable newAtoms = new Hashtable();

            foreach (Atom atom in molecule.a)
            {
                if (atom != null)
                {
                    GameObject newAtom = Instantiate((GameObject)Resources.Load("Prefabs/Atom", typeof(GameObject)), new Vector3(atom.x, atom.y, atom.z), Quaternion.identity, transform);
                    AtomBehaviour atomScript = newAtom.GetComponent<AtomBehaviour>();
                    atomScript.initializeAtom(atom.type, atom.charge);
                    newAtoms.Add(atom.id, atomScript);
                }
            }

            foreach (Bond bond in molecule.b)
            {
                if (bond != null)
                {
                    GameObject newBond = null;
                    switch (bond.type)
                    {
                        case (int)BondBehaviour.BondType.Single:
                            newBond = Instantiate((GameObject)Resources.Load("Prefabs/SingleBond", typeof(GameObject)), transform.position, Quaternion.identity, transform);
                            break;
                        case (int)BondBehaviour.BondType.Double:
                            newBond = Instantiate((GameObject)Resources.Load("Prefabs/DoubleBond", typeof(GameObject)), transform.position, Quaternion.identity, transform);
                            break;
                        case (int)BondBehaviour.BondType.Triple:
                            newBond = Instantiate((GameObject)Resources.Load("Prefabs/TripleBond", typeof(GameObject)), transform.position, Quaternion.identity, transform);
                            break;
                        case (int)BondBehaviour.BondType.Electrons:
                            newBond = Instantiate((GameObject)Resources.Load("Prefabs/Electrons", typeof(GameObject)), transform.position, Quaternion.identity, transform);
                            break;
                    }

                    if (newBond != null)
                    {
                        AtomBehaviour anchor1 = (AtomBehaviour)newAtoms[bond.a1];
                        AtomBehaviour anchor2 = (AtomBehaviour)newAtoms[bond.a2];
                        BondBehaviour bondScript = newBond.GetComponent<BondBehaviour>();
                        bondScript.initializeBond(anchor1, anchor2);

                        if (anchor1 != null)
                        {
                            anchor1.addBond(bondScript);
                        }

                        if (anchor2 != null)
                        {
                            anchor2.addBond(bondScript);
                        }
                    }
                }
            }
        }
    }
}
