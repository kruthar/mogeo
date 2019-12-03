using System;
using UnityEngine;

[Serializable]
public class Molecule
{
    public Atom[] a;
    public Bond[] b;

    public Molecule(Atom[] atoms, Bond[] bonds)
    {
        a = atoms;
        b = bonds;
    }

    public string serialize()
    {
        string result = "";

        foreach (Atom atom in a)
        {
            result += atom.serialize() + ",";
        }

        result += "_";

        foreach (Bond bond in b)
        {
            result += bond.serialize() + ",";
        }

        return result;
    }

    public static Molecule deserialize(string encoded)
    {
        string[] parts = encoded.Split('_');

        if (parts.Length < 2)
        {
            // TODO: Throw an exception here?
            Debug.Log("Expecting two major parts in encoded string when spliting on _");
            return null;
        }

        string[] encodedAtoms = parts[0].Split(',');
        Atom[] atoms = new Atom[encodedAtoms.Length];

        for (int i = 0; i < encodedAtoms.Length; i++)
        {
            atoms[i] = Atom.deserialize(encodedAtoms[i]);
        }

        string[] encodedBonds = parts[1].Split(',');
        Bond[] bonds = new Bond[encodedBonds.Length];

        for (int i = 0; i < encodedBonds.Length; i++)
        {
            bonds[i] = Bond.deserialize(encodedBonds[i]);
        }

        return new Molecule(atoms, bonds);
    }
}
