using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardMouseDetector : MonoBehaviour
{
    private float oldY = Consts.Y_HANDCARDS;
    private readonly float offset = Consts.HANDCARDS_OFFSET;
    public bool isPicked = false;
    public Card card;

    void Start()
    {
        oldY = transform.position.y;
    }

    public void TogglePick()
    {
        if (!isPicked)
        {
            isPicked = true;
            transform.position = new Vector3(transform.position.x, oldY + offset, transform.position.z);
        }
        else
        {
            isPicked = false;
            transform.position = new Vector3(transform.position.x, oldY, transform.position.z);
        }
    }

    public void SetPick(bool pick)
    {
        Debug.Log("SetPick " + pick);
        isPicked = pick;
    }
}
