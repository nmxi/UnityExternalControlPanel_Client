using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdTag : MonoBehaviour {
    [SerializeField] private int _id;

    public int ID {
        set {
            _id = value;
        }
        get {
            return _id;
        }
    }
}
