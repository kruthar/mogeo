using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BondBehaviour : MonoBehaviour
{
    public enum BondType
    {
        Single = 1,
        Double = 2,
        Triple = 3,
        Electrons = 4
    }

    public bool followMouse;
    private AtomBehaviour[] anchors;
    public BondType bondType;

    private MoleculeBehaviour molecule;

    public void initializeBond(AtomBehaviour anchor)
    {
        followMouse = true;
        anchors = new AtomBehaviour[2];
        anchors[0] = anchor;
        molecule = GameObject.Find("Molecule").GetComponent<MoleculeBehaviour>();
    }

    public void initializeBond(AtomBehaviour anchor1, AtomBehaviour anchor2)
    {
        anchors = new AtomBehaviour[2];
        anchors[0] = anchor1;
        anchors[1] = anchor2;
        molecule = GameObject.Find("Molecule").GetComponent<MoleculeBehaviour>();
        resetAnchorPositions();
    }

    public void resetAnchorPositions()
    {
        if (bondType.Equals(BondType.Electrons))
        {
            resetElectronAnchor();
        }
        else
        {
            stretchBondToAnchors();
        }
    }

    private void resetElectronAnchor()
    {
        transform.position = anchors[0].transform.position;

        // If an Electron has an anchors[1] this means we are loading it from a save for the first time.
        // After we apply the correct rotation, delete the anchor so that can function normally.
        if (anchors[1] != null)
        {
            transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward, anchors[1].transform.position - anchors[0].transform.position);
            Destroy(anchors[1].gameObject);
            anchors[1] = null;
        }
    }

    private void stretchBondToAnchors()
    {
        // First step is to attach one end to the first anchor
        transform.position = anchors[0].transform.position;

        // Second step is to put the other end to the second anchor
        float length = Vector3.Distance(anchors[1].transform.position, anchors[0].transform.position) / 2;
        transform.localScale = new Vector3(1, 1, length);
        transform.rotation = Quaternion.LookRotation(anchors[1].transform.position - anchors[0].transform.position, Camera.main.transform.up);
    }

    // Update is called once per frame
    void Update()
    {
        // When Bonds are first created they are set with followMouse = true,
        // during this time they should keep one end under the mouse cursor until the 
        // primary mouse button is clicked, then they will be locked into place.
        // The other end should stay anchored to the Atom it was attached to.
        if (followMouse)
        {
            if (bondType.Equals(BondType.Electrons))
            {
                updateElectrons();
            }
            else
            {
                updateBond();
            }
        }
    }

    private void updateBond() {
        // Set the transform position to the anchor atom
        transform.position = anchors[0].transform.position;

        // Get the world position of the mouse
        Vector3 newPos = Input.mousePosition;
        newPos.z = Camera.main.WorldToScreenPoint(transform.position).z;
        Vector3 anchor2 = Camera.main.ScreenToWorldPoint(newPos);

        // Stretch the bond to the mouse anchor
        float length = Vector3.Distance(anchor2, transform.position) / 2;
        transform.localScale = new Vector3(1, 1, length);
        transform.rotation = Quaternion.LookRotation(anchor2 - transform.position, Camera.main.transform.up);

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Cast out a Ray to the mouse point to see if we have released the Bond over an Atom
            // If we have hit an Atom other then our current Anchor, then lets anchor to that Atom
            // Otherwise, destroy this Bond, it is invalid.
            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject.GetComponent<AtomBehaviour>() != null && !ReferenceEquals(hit.collider.gameObject, anchors[0].gameObject))
            {
                // If we have properly attached to another atom, then formally
                // anchor this Atom, then update the transform to go through
                // the center of both Atoms.
                anchors[1] = hit.collider.gameObject.GetComponent<AtomBehaviour>();
                hit.collider.gameObject.GetComponent<AtomBehaviour>().addBond(this);
                stretchBondToAnchors();
                molecule.setMode(MoleculeBehaviour.Mode.None);
            }
            else
            {
                anchors[0].removeBond(this);
                molecule.setMode(MoleculeBehaviour.Mode.None);
                Destroy(this.gameObject);
            }

            followMouse = false;
        }
    }

    public void updateElectrons()
    { 
        if (molecule.getPhase().Equals(MoleculeBehaviour.Phase.Build))
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Vector3.Distance(Camera.main.transform.position, anchors[0].transform.position);
            transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward, Camera.main.ScreenToWorldPoint(mousePos) - transform.position);
        }

        if (Input.GetMouseButtonDown(0))
        {
            molecule.setMode(MoleculeBehaviour.Mode.None);
            followMouse = false;
        }
    }

    public Bond getBond()
    {
        if (anchors[1] == null)
        {
            AtomBehaviour atomScript = AtomBehaviour.instantiateAtom(transform.position + Vector3.Normalize(transform.up), "z");
            anchors[1] = atomScript;
        }

        return new Bond(anchors[0].id, anchors[1].id, (int) bondType);
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

    private void OnMouseOver()
    {
        // Only set the bond highlight if we are in Delete mode because single/double/and triples have no other action to perform.
        // Always set highlight for electrons because they can be dragged.
        if (molecule.getMode().Equals(MoleculeBehaviour.Mode.Delete) || bondType.Equals(BondType.Electrons))
        {
            setBondHighlight();
        }
    }

    private void OnMouseExit()
    {
        setBondColor();
    }

    public void setBondColor()
    {
        foreach (Renderer mr in transform.GetComponentsInChildren<Renderer>())
        {
            mr.material.color = Color.white;
        }
    }

    public void setBondHighlight()
    {
        foreach (Renderer mr in transform.GetComponentsInChildren<Renderer>())
        {
            mr.material.color = Color.yellow;
        }
    }

    private void OnMouseDrag()
    {
        if (
            bondType.Equals(BondType.Electrons) &&
            (molecule.getMode().Equals(MoleculeBehaviour.Mode.None) || molecule.getMode().Equals(MoleculeBehaviour.Mode.AdjustingAtom))
            )
        {
            updateElectrons();
            molecule.setMode(MoleculeBehaviour.Mode.AdjustingAtom);
        }
    }

    private void OnMouseUp()
    {
        if (molecule.getMode().Equals(MoleculeBehaviour.Mode.AdjustingAtom))
        {
            molecule.setMode(MoleculeBehaviour.Mode.None);
        }
    }

    public void destroySelf()
    {
        Destroy(this.gameObject);
    }
}
