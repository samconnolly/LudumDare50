using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private bool frozen;

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
            transform.position += moveDir * moveSpeed * 0.01f;  // scaling to make 1 reasonable
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
        
        if (count > 0) {
            foreach (Collider2D collider in circleOverlaps) {
                if (collider != null) {
                    if (collider.tag == "Thorn") {
                        collider.GetComponent<Thorn>().Kill();
                    }
                }
            }
        }
    }

    public void Freeze() {
        frozen = true;
    }
    public void Unfreeze() {
        frozen = false;
    }
}
