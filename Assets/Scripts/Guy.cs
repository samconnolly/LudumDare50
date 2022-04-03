using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Cinemachine;

public class Guy : Character
{
    public float reach;
    public float chopTime;
    public float thornDamageTime;
    public int thornDamage;
    public StatBar healthBar;
    

    private CircleCollider2D circleCollider;
    private bool inThorns;
    private float thornTimer;

    private Vector3Int chopTarget;
    private float chopTimer;
    private bool chopping;
    private CinemachineImpulseSource impulseSource;


    // Start is called before the first frame update
    protected override void Start()
    {
        base.enemy = false;
        base.Start();
        circleCollider = GetComponent<CircleCollider2D>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (GameHelper.gameRunning) {
            UpdateControl();
            UpdateChopping();
            UpdateThornDamage();
        }
        base.Update();
    }
    void UpdateChopping()
    {
        
        if (Input.GetKey(KeyCode.Space) & ! base.moving){
            if (chopping){
                chopTimer += Time.deltaTime;
                if (chopTimer >= chopTime) {
                    GameHelper.TopTileMap.GetComponent<ThornGrowth>().RemoveThorns(chopTarget);
                    chopping = false;
                    chopTimer = 0;
                    Debug.Log("finished chopping");
                }
            }
        }
        else if (chopping) {
            Debug.Log("abandoned chopping");
            chopping = false;
            chopTimer = 0;
        }
    }
    void UpdateThornDamage()
    {
        bool thorny = GameHelper.Thorns.IsThorny(transform.position);
        if (thorny) {
            if (! inThorns) {
                inThorns = true;
                thornTimer = thornDamageTime;
            }

            thornTimer += Time.deltaTime;
            if (thornTimer >= thornDamageTime) {
                TakeDamage(thornDamage);
                thornTimer = 0;
            }
        }
        else {
            inThorns = false;
            thornTimer = 0;
        }
    }

    void UpdateControl()
    {   

        if (Input.GetKeyDown(KeyCode.Space)) {
            Attack();
        }

        moveDir = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) ||
            Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A)) {
            base.faceDir = new Vector3(0, 0, 0);
            if (Input.GetKey(KeyCode.W)) {
                moveDir += new Vector3(0, 1, 0);
                base.faceDir += new Vector3(0, 1, 0);
            }
            if (Input.GetKey(KeyCode.S)) {
                moveDir += new Vector3(0, -1, 0);
                base.faceDir += new Vector3(0, -1, 0);
            }
            if (Input.GetKey(KeyCode.A)) {
                moveDir += new Vector3(-1, 0, 0);
                base.faceDir += new Vector3(-1, 0, 0);
            }
            if (Input.GetKey(KeyCode.D)) {
                moveDir += new Vector3(1, 0, 0);
                base.faceDir += new Vector3(1, 0, 0);
            }
        }

        if (moveDir.magnitude > 0){
            moving = true;
        }
        else {
            moving = false;
        }
        moveDir.Normalize();

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) ||
            Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow)) {
            base.faceDir = new Vector3(0, 0, 0);
            if (Input.GetKey(KeyCode.UpArrow)) {
                base.faceDir += new Vector3(0, 1, 0);
            }
            if (Input.GetKey(KeyCode.DownArrow)) {
                base.faceDir += new Vector3(0, -1, 0);
            }
            if (Input.GetKey(KeyCode.LeftArrow)) {
                base.faceDir += new Vector3(-1, 0, 0);
            }
            if (Input.GetKey(KeyCode.RightArrow)) {
                base.faceDir += new Vector3(1, 0, 0);
            }
        }

        // if (faceDir.magnitude > 0){
        //     rot = Quaternion.FromToRotation(new Vector3(0, 1, 0), faceDir);
        // }
    }

    private void Attack(){
        Debug.Log("Attack");
        impulseSource.GenerateImpulse();
        base.SetAnimation(AnimType.Attacking);

        ContactFilter2D contactFilter = new ContactFilter2D();
        Collider2D[] circleOverlaps = new Collider2D[5];
        int count = circleCollider.OverlapCollider(contactFilter, circleOverlaps);
        
        // attack
        if (count > 0) {
            foreach (Collider2D collider in circleOverlaps) {
                if (collider != null) {
                    Debug.Log(collider.gameObject);
                    if (collider.tag == "Thorn") {
                        // collider.GetComponent<Thorn>().Kill();
                    }
                }
            }
        }

        // chop        
        Vector2 fw_reach = transform.position + transform.up * reach;
        if (GameHelper.Thorns.IsThorny(fw_reach)) {
            Debug.Log("Start chopping...");
            Vector3Int cellCoords = GameHelper.TopTileMap.LocalToCell(fw_reach);
            StartChopping(cellCoords);
        }
        
    }

    public void TakeDamage(int damage) {
        Debug.LogFormat("Ow... ({0} damage)", damage);
        healthBar.Subtract(damage);
        impulseSource.GenerateImpulse();
        if (healthBar.Value == 0) {
            GameHelper.LoseGame();
        }
    }

    public void StartChopping(Vector3Int cellCoords){
        chopTarget = cellCoords;
        chopping = true;
    }
}
