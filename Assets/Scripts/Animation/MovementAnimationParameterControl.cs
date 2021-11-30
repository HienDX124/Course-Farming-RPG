﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementAnimationParameterControl : MonoBehaviour
{

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        EventHandler.MovementEvent += SetAnimationParameters;

    }

    private void OnDisable()
    {
        EventHandler.MovementEvent -= SetAnimationParameters;
    }

    private void SetAnimationParameters(float inputX, float inputY, bool isWalking,
        bool isRunning, bool isIdle, bool isCarrying, ToolEffect toolEffect,
        bool isUsingToolRight, bool isUsingToolLeft, bool isUsingToolUp, bool isUsingToolDown,
        bool isLiftingToolRight, bool isLiftingToolLeft, bool isLiftingToolUp, bool isLiftingToolDown,
        bool isPickingRight, bool isPickingLeft, bool isPickingUp, bool isPickingDown,
        bool isSwingingToolRight, bool isSwingingToolLeft, bool isSwingingToolUp, bool isSwingingToolDown,
        bool idleUp, bool idleDown, bool idleLeft, bool idleRight)
    {
        animator.SetFloat(Settings.xInput, inputX);
        animator.SetFloat(Settings.yInput, inputY);
        animator.SetBool(Settings.isWalking, isWalking);
        animator.SetBool(Settings.isRunning, isRunning);

        animator.SetInteger(Settings.toolEffect, (int)toolEffect);
        //  Not done here

        if (isUsingToolRight) animator.SetTrigger(Settings.isUsingToolRight);
        if (isUsingToolLeft) animator.SetTrigger(Settings.isUsingToolLeft);
        if (isUsingToolUp) animator.SetTrigger(Settings.isUsingToolUp);
        if (isUsingToolDown) animator.SetTrigger(Settings.isUsingToolDown);

        if (isLiftingToolRight) animator.SetTrigger(Settings.isLiftingToolRight);
        if (isLiftingToolLeft) animator.SetTrigger(Settings.isLiftingToolLeft);
        if (isLiftingToolUp) animator.SetTrigger(Settings.isLiftingToolUp);
        if (isLiftingToolDown) animator.SetTrigger(Settings.isLiftingToolDown);

        if (isPickingRight) animator.SetTrigger(Settings.isPickingRight);
        if (isPickingLeft) animator.SetTrigger(Settings.isPickingLeft);
        if (isPickingUp) animator.SetTrigger(Settings.isPickingUp);
        if (isPickingDown) animator.SetTrigger(Settings.isPickingDown);

        if (isSwingingToolRight) animator.SetTrigger(Settings.isSwingingToolRight);
        if (isSwingingToolLeft) animator.SetTrigger(Settings.isSwingingToolLeft);
        if (isSwingingToolUp) animator.SetTrigger(Settings.isSwingingToolUp);
        if (isSwingingToolDown) animator.SetTrigger(Settings.isSwingingToolDown);

        if (idleUp) animator.SetTrigger(Settings.idleUp);
        if (idleDown) animator.SetTrigger(Settings.idleDown);
        if (idleLeft) animator.SetTrigger(Settings.idleLeft);
        if (idleRight) animator.SetTrigger(Settings.idleRight);



    }

    private void AnimationEventPlayFootstepSound()
    {

    }
}
