﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackActionMonster : StateMachineBehaviour
{
    private Monster monster;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (monster == null) monster = animator.GetComponent<EnemieController>().monster;
        animator.SetBool("Attack", false);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Monster.Direction currentDirection = Monster.Direction.stay;

        monster.AttackingMode(true);
        monster.SetAnimDirection(currentDirection); //Escolhe a animação dependendo apenas da direção atual
        monster.SetVelocity(currentDirection); //Ecolhe a velocidade dependendo da direção atual
        monster.FlipCharacter(currentDirection); //Inverte o sentido dependendo da direção atual

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
