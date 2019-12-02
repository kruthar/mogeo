using System;
using UnityEngine;

[Serializable]
public class Bond
{
    public int a1;
    public int a2;
    public int type;

    public Bond(int anchor1, int anchor2, int type)
    {
        a1 = anchor1;
        a2 = anchor2;
        this.type = type;
    }

    public string serialize()
    {
        return a1 + "|"
            + a2 + "|"
            + type;
    }

    public static Bond deserialize(string encoded)
    {
        string[] parts = encoded.Split('|');

        if (parts.Length < 3)
        {
            // TODO: Should probably through an exception here?
            Debug.Log("Expecting 3 parts when deserializing Atom");
            return null;
        }

        return new Bond(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));
    }
}
