using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Character
{
    public float reach;
    public float attackInterval;
    public int attackDamage;
    private float bufferFrac = 0.1f;
    private bool attacking;
    private float attackTimer;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.enemy = true;
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (GameHelper.gameRunning) {
            base.SetAnimation(AnimType.Walking);
            UpdateActions();        
        }
        base.Update();
    }

    private void UpdateActions() {
        Vector3 playeVec = GameHelper.Player.transform.position - transform.position;

        if (playeVec.magnitude > reach && ! attacking) {
            base.moveDir = playeVec.normalized;
            base.faceDir = moveDir;
        }
        else if (playeVec.magnitude > reach * (1 + bufferFrac) && attacking) {
            base.SetAnimation(AnimType.Walking);
            attacking = false;
        }
        else {
            if (! attacking) {
                attacking = true;
                base.SetAnimation(AnimType.Attacking);
                base.moveDir = Vector3.zero;
            }

            attackTimer += Time.deltaTime;
            if (attackTimer >= attackInterval) {
                Attack();
                attackTimer = 0;
            }
        }
    }

    private void Attack(){
        GameHelper.Player.TakeDamage(attackDamage);
    }
}
