using System;
using UnityEngine;

[Serializable]
public class Atom
{
    public int id;
    public float x;
    public float y;
    public float z;
    public string type;
    public int charge;

    public Atom(int id, float x, float y, float z, string type, int charge)
    {
        this.id = id;
        this.x = x;
        this.y = y;
        this.z = z;
        this.type = type;
        this.charge = charge;
    }

    public string serialize()
    {
        return id + "|"
            + x.ToString("F2") + "|"
            + y.ToString("F2") + "|"
            + z.ToString("F2") + "|"
            + type + "|"
            + charge;
    }

    public static Atom deserialize(string encoded)
    {
        string[] parts = encoded.Split('|');

        if (parts.Length < 6)
        {
            // TODO: Should probably through an exception here?
            Debug.Log("Expecting 6 parts when deserializing Atom");
            return null;
        }

        return new Atom(int.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]), parts[4], int.Parse(parts[5]));
    }
}
