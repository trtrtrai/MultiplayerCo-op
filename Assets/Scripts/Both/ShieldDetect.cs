using Assets.Scripts.Both.Creature.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldDetect : MonoBehaviour
{
    [SerializeField] private PlayerController owner;

    // Start is called before the first frame update
    void Start()
    {
        owner = gameObject.transform.parent.GetComponent<PlayerController>();

        if (owner is null)
        {
            Debug.Log("Shield noname!");
            //Destroy
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.name);
    }
}
