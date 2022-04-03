using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimType
{
   Standing,
   Walking,
   Attacking
}
public enum Dir
{
   Up,
   Down,
   Left,
   Right
}

public class Character : MonoBehaviour
{

    public float moveSpeed;
    public Sprite[] standSpritesUp;
    public Sprite[] standSpritesDown;
    public Sprite[] standSpritesLeft;
    public Sprite[] standSpritesRight;
    public Sprite[] walkSpritesUp;
    public Sprite[] walkSpritesDown;
    public Sprite[] walkSpritesLeft;
    public Sprite[] walkSpritesRight;
    public Sprite[] attackSpritesUp;
    public Sprite[] attackSpritesDown;
    public Sprite[] attackSpritesLeft;
    public Sprite[] attackSpritesRight;
    

    protected Vector3 moveDir;
    protected Vector3 faceDir;
    protected Dir dir = Dir.Down;
    private Quaternion rot;
    protected float speedMultiplier;
    private SpriteRenderer spriteRenderer;
    private Dictionary<Dir, Sprite[]> currentSprites;
    private Dictionary<Dir, Sprite[]> standSprites;
    private Dictionary<Dir, Sprite[]> walkSprites;
    private Dictionary<Dir, Sprite[]> attackSprites;


    private float frameTime = 1 / GameHelper.frameRate;
    private int frame = 0;
    private float animTimer;
    private AnimType currentAnimation;

    private Dictionary<AnimType, AnimType> animTransitions = new Dictionary<AnimType, AnimType>();
    private Vector3 truePosition;
    private float posQuant = 1.0f /(32.0f * 1.0f);
    protected bool moving;
    private Rigidbody2D rigidbody;

    private bool frozen;


    // Start is called before the first frame update
    protected virtual void Start()
    {
        truePosition = transform.position;  // unquantised position

        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody = GetComponent<Rigidbody2D>();

        standSprites = new Dictionary<Dir, Sprite[]>{};
        standSprites.Add(Dir.Up, standSpritesUp);
        standSprites.Add(Dir.Down, standSpritesDown);
        standSprites.Add(Dir.Left, standSpritesLeft);
        standSprites.Add(Dir.Right, standSpritesRight);

        walkSprites = new Dictionary<Dir, Sprite[]>{};
        walkSprites.Add(Dir.Up, walkSpritesUp);
        walkSprites.Add(Dir.Down, walkSpritesDown);
        walkSprites.Add(Dir.Left, walkSpritesLeft);
        walkSprites.Add(Dir.Right, walkSpritesRight);

        attackSprites = new Dictionary<Dir, Sprite[]>{};
        attackSprites.Add(Dir.Up, attackSpritesUp);
        attackSprites.Add(Dir.Down, attackSpritesDown);
        attackSprites.Add(Dir.Left, attackSpritesLeft);
        attackSprites.Add(Dir.Right, attackSpritesRight);

        SetAnimation(AnimType.Standing);
        animTransitions.Add(AnimType.Standing, AnimType.Standing);
        animTransitions.Add(AnimType.Walking, AnimType.Walking);
        animTransitions.Add(AnimType.Attacking, AnimType.Standing);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
        if (GameHelper.gameRunning) {
            UpdateAnimation();
            UpdateMovement();
        }
    }
    void UpdateMovement()
    {
        if (! frozen) {
            speedMultiplier = GameHelper.Thorns.SpeedMultiplier(transform.position);
            
            // prevent drifting
            if ((truePosition - transform.position).magnitude > posQuant){
                truePosition = transform.position;
            }

            truePosition += moveDir * moveSpeed * speedMultiplier * Time.deltaTime;  // scaling to make 1 reasonable
            
            Vector3 qPos = new Vector3(Mathf.Round(truePosition.x / posQuant) * posQuant,
                                       Mathf.Round(truePosition.y / posQuant) * posQuant, 0);
            // transform.position = qPos;
            rigidbody.MovePosition(qPos);

            // transform.rotation = rot;

            if (faceDir.y > 0 && Mathf.Abs(faceDir.y) > Mathf.Abs(faceDir.x)){
                dir = Dir.Up;
            }            
            else if (faceDir.y < 0 && Mathf.Abs(faceDir.y) > Mathf.Abs(faceDir.x)){
                dir = Dir.Down;
            }            
            else if (faceDir.x > 0 && Mathf.Abs(faceDir.x) > Mathf.Abs(faceDir.y)){
                dir = Dir.Right;
            }
            else if (faceDir.x < 0 && Mathf.Abs(faceDir.x) > Mathf.Abs(faceDir.y)){
                dir = Dir.Left;
            }
            else {
                dir = Dir.Down; // should only be at the start when stationary
            }
        }
    }

    void UpdateAnimation()
    {
        animTimer += Time.deltaTime;
        if (animTimer >= frameTime) {
            frame += 1;
            if (frame >= currentSprites[dir].Length) {
                frame = 0;
                SetAnimation(animTransitions[currentAnimation]);
            }
            animTimer = 0;
        }
        spriteRenderer.sprite = currentSprites[dir][frame]; // update regardless because of direction changes
    }

    protected void SetAnimation(AnimType animType) {
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
            default:
                Debug.Log("No anim type???");
                break;
        }
        frame = 0;
        spriteRenderer.sprite = currentSprites[dir][frame];
        animTimer = 0;        
    }
}
