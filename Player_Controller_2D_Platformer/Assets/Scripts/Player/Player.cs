using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Player_Controller))]
public class Player : MonoBehaviour
{
    Player_Controller controller;

    void Start()
    {
        controller = GetComponent<Player_Controller>();
    }
}