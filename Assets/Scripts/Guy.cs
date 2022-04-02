using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

enum AnimType
{
   Standing,
   Walking,
   Attacking
}

public class Guy : MonoBehaviour
{
    public float moveSpeed;
    public Sprite[] standSprites;
    public Sprite[] walkSprites;
    public Sprite[] attackSprites;
    public float reach;
    public float chopTime;
    public float thornDamageTime;
    public int thornDamage;
    public HealthBar healthBar;
    

    private Vector3 moveDir;
    private Vector3 faceDir;
    private Quaternion rot;
    private SpriteRenderer spriteRenderer;
    private Sprite[] currentSprites;

    private float frameTime = 1;
    private int frame;
    private float animTimer;
    private AnimType currentAnimation;

    private CircleCollider2D circleCollider;

    private Dictionary<AnimType, AnimType> animTransitions = new Dictionary<AnimType, AnimType>();
    private bool moving;
    private bool inThorns;
    private float thornTimer;

    private bool frozen;
    private Vector3Int chopTarget;
    private float chopTimer;
    private bool chopping;


    // Start is called before the first frame update
    void Start()
    {
       spriteRenderer = GetComponent<SpriteRenderer>();
       circleCollider = GetComponent<CircleCollider2D>();

       SetAnimation(AnimType.Standing);
       animTransitions.Add(AnimType.Standing, AnimType.Standing);
       animTransitions.Add(AnimType.Walking, AnimType.Walking);
       animTransitions.Add(AnimType.Attacking, AnimType.Standing);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMovement();
        UpdateAttack();
        UpdateAnimation();
        UpdateChopping();
        UpdateThornDamage();
    }
    void UpdateChopping()
    {
        
        if (Input.GetKey(KeyCode.Space) & ! moving){
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

    void UpdateAnimation()
    {
        animTimer += Time.deltaTime;
        if (animTimer >= frameTime) {
            frame += 1;
            if (frame >= currentSprites.Length) {
                frame = 0;
                SetAnimation(animTransitions[currentAnimation]);
            }
            spriteRenderer.sprite = currentSprites[frame];
            animTimer = 0;
        }
    }

    private void SetAnimation(AnimType animType) {
        // Debug.LogFormat("Set anim: {0}", animType);
        currentAnimation = animType;
        switch (animType){
            case AnimType.Standing:
                currentSprites = standSprites;
                break;
            case AnimType.Walking:
                currentSprites = walkSprites;
                break;
            case AnimType.Attacking:
                currentSprites = attackSprites;
                break;
        }
        frame = 0;
        spriteRenderer.sprite = currentSprites[frame];
        animTimer = 0;        
    }
    
    void UpdateMovement()
    {        
        moveDir = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.W)) {
            moveDir += new Vector3(0, 1, 0);
        }
        if (Input.GetKey(KeyCode.S)) {
            moveDir += new Vector3(0, -1, 0);
        }
        if (Input.GetKey(KeyCode.A)) {
            moveDir += new Vector3(-1, 0, 0);
        }
        if (Input.GetKey(KeyCode.D)) {
            moveDir += new Vector3(1, 0, 0);
        }

        // TODO ANIMTAION
        if (moveDir.magnitude > 0){
            moving = true;
        }
        else {
            moving = false;
        }
        moveDir.Normalize();

        faceDir = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.UpArrow)) {
            faceDir += new Vector3(0, 1, 0);
        }
        if (Input.GetKey(KeyCode.DownArrow)) {
            faceDir += new Vector3(0, -1, 0);
        }
        if (Input.GetKey(KeyCode.LeftArrow)) {
            faceDir += new Vector3(-1, 0, 0);
        }
        if (Input.GetKey(KeyCode.RightArrow)) {
            faceDir += new Vector3(1, 0, 0);
        }

        if (faceDir.magnitude > 0){
            rot = Quaternion.FromToRotation(new Vector3(0, 1, 0), faceDir);
        }

        if (! frozen) {
            float speedMultiplier = GameHelper.Thorns.SpeedMultiplier(transform.position);
            transform.position += moveDir * moveSpeed * speedMultiplier * 0.01f;  // scaling to make 1 reasonable
            transform.rotation = rot;
        }
    }

    private void UpdateAttack() {
        
        if (Input.GetKeyDown(KeyCode.Space)) {
            Attack();
        }
    }

    private void Attack(){
        Debug.Log("Attack");
        SetAnimation(AnimType.Attacking);

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
        if (healthBar.Value == 0) {
            GameHelper.LoseGame();
        }
    }

    public void Freeze() {
        frozen = true;
    }
    public void Unfreeze() {
        frozen = false;
    }

    public void StartChopping(Vector3Int cellCoords){
        chopTarget = cellCoords;
        chopping = true;
    }
}
