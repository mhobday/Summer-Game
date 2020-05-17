using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour
{
    public GameObject grapple;
    public GameObject Player;
    GameObject curHook;
    // Start is called before the first frame update
    void Start()
    {
        curHook = Instantiate(grapple, Vector2.zero, Quaternion.identity);
        curHook.GetComponent<HingeJoint2D>().connectedBody = Player.GetComponent<Rigidbody2D>();
        curHook.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0) && !curHook.activeSelf)
        {
            Vector2 mouseLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Player.GetComponent<Rigidbody2D>().freezeRotation = false;
            curHook.transform.position = mouseLocation;
            curHook.SetActive(true);
        }
        if(Input.GetButtonDown("Jump"))
        {
            curHook.SetActive(false);
            Player.transform.rotation = Quaternion.Euler(0,0,0);
            Player.GetComponent<Rigidbody2D>().freezeRotation = true;
        }
    }
}
