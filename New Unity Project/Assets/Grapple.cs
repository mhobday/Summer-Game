using UnityEngine;
using System.Collections;

public class Grapple : MonoBehaviour
{
    [SerializeField] private GameObject player;
    //Rigidbody of the player
    Rigidbody2D body;
    //Grappling hook sprite
    LineRenderer line;
    //Joint that controls grapple physics
    DistanceJoint2D joint;
    //Location of the mouse when clicked
    Vector3 targetPos;
    //Raycast checking for rope collision with a wall
    RaycastHit2D hit;
    //Determines if the hook is being reeled in
    bool reel = false;
    //Max Grappling hook length
    public float distance = 10f;
    //All things the grappling hook can collide with
    public LayerMask mask;
    //Speed at which the hook reels in
    public float step = .05f;
    //Minumum length of the hook
    public float minLength = 1f;
    //If the hook has just been jumped off of
    public bool hookJump = false;
    int counter = 0;

    // Use this for initialization
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        joint = GetComponent<DistanceJoint2D>();
        line = GetComponent<LineRenderer>();
        disableHook();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && joint.enabled)
        {
            reel = !reel;
        }
        if (Input.GetMouseButtonDown(0))
        {
            //Find mouse position
            targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPos.z = 0;
            //Check for collisions between the player and the mouse position
            hit = Physics2D.Raycast(transform.position, targetPos - transform.position, distance, mask);
            //On contact with something with a rigid body
            if (hit.collider != null && hit.collider.gameObject.GetComponent<Rigidbody2D>() != null)
            {
                joint.enabled = true;
                Vector2 connectPoint = hit.point - new Vector2(hit.collider.transform.position.x, hit.collider.transform.position.y);
                connectPoint.x = connectPoint.x / hit.collider.transform.localScale.x;
                connectPoint.y = connectPoint.y / hit.collider.transform.localScale.y;
                joint.connectedAnchor = connectPoint;

                joint.connectedBody = hit.collider.gameObject.GetComponent<Rigidbody2D>();
                joint.distance = Vector2.Distance(transform.position, hit.point);

                line.enabled = true;
                line.SetPosition(0, transform.position);
                line.SetPosition(1, hit.point);
            }
        }
        if (Input.GetButtonDown("Jump"))
        {
            hookJump = true;
        }
        if (Input.GetAxisRaw("Vertical") < 0)
        {
            disableHook();
        }
    }
    void FixedUpdate()
    {
        //Moves player towards the hook until it is destroyed
        if (reel == true)
        {
            if (Vector2.Distance(joint.connectedAnchor, Vector2.MoveTowards(body.position, joint.connectedAnchor, 1)) > minLength)
            {
                pullInPlayer();
            }
            else
            {
                disableHook();
            }
        }
        if(hookJump)
        {
            hookJump = false;
        }
        /*
        if(line.enabled)
        {
            line.SetPosition(1, joint.connectedBody.transform.TransformPoint(joint.connectedAnchor));
        }

        if (Input.GetMouseButton(0))
        {

            line.SetPosition(0, transform.position);
        }
        */
    }
    public void disableHook()
    {
        joint.enabled = false;
        line.enabled = false;
        reel = false;
    }
    public void pullInPlayer()
    {
        Vector3 speed = Vector3.MoveTowards(body.position, joint.connectedAnchor, step);
        Vector3 Velocity = body.velocity;
        body.velocity = Vector3.SmoothDamp(body.velocity, speed, ref Velocity, player.GetComponent<CharacterController2D>().MovementSmoothing);
        joint.distance -= step * Time.deltaTime;
    }


}