﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationOverrides : MonoBehaviour
{
    [SerializeField] private GameObject character = null;
    [SerializeField] private SO_AnimationType[] so_AnimationTypeArray = null;

    private Dictionary<AnimationClip, SO_AnimationType> animationTypeDictionaryByAnimation;
    private Dictionary<string, SO_AnimationType> animationTypeDictionaryByCompositeAttributeKey;

    private void Start()
    {
        // Initialise animation type dictionary keyed up by animation clip
        animationTypeDictionaryByAnimation = new Dictionary<AnimationClip, SO_AnimationType>();

        foreach (SO_AnimationType item in so_AnimationTypeArray)
        {
            animationTypeDictionaryByAnimation.Add(item.animationClip, item);
        }

        // Initialise animation typy dictionary keyed by string
        animationTypeDictionaryByCompositeAttributeKey = new Dictionary<string, SO_AnimationType>();
        foreach (SO_AnimationType item in so_AnimationTypeArray)
        {
            string key = item.characterPart.ToString() + item.partVariantColour.ToString() + item.partVariantType.ToString() + item.animationName.ToString();
            animationTypeDictionaryByCompositeAttributeKey.Add(key, item);
        }
    }

    public void ApplyCharacterCustomisationParameters(List<CharacterAttribute> characterAttributeList)
    {
        // Stopwatch s1 = StopWatch.StartNew();

        // Loop through all character attributes and set the animation override controller for each
        foreach (CharacterAttribute characterAttribute in characterAttributeList)
        {
            Animator currentAnimator = null;
            List<KeyValuePair<AnimationClip, AnimationClip>> animsKeyValuePairList = new List<KeyValuePair<AnimationClip, AnimationClip>>();

            string animatorSOAssetName = characterAttribute.characterPart.ToString();

            // Find animators in scene that match scriptable object animator type
            Animator[] animatorsArray = character.GetComponentsInChildren<Animator>();

            foreach (Animator animator in animatorsArray)
            {
                if (animator.name == animatorSOAssetName)
                {
                    currentAnimator = animator;
                    break;
                }
            }

            // Get base current animations for animator
            AnimatorOverrideController aoc = new AnimatorOverrideController(currentAnimator.runtimeAnimatorController);
            List<AnimationClip> animationsList = new List<AnimationClip>(aoc.animationClips);

            foreach (AnimationClip animationClip in animationsList)
            {
                // Find animation in dictionary
                SO_AnimationType so_AnimationType;
                bool foundAnimation = animationTypeDictionaryByAnimation.TryGetValue(animationClip, out so_AnimationType);

                if (foundAnimation)
                {
                    string key = characterAttribute.characterPart.ToString() + characterAttribute.partVariantColour.ToString() +
                        characterAttribute.partVariantType.ToString() + so_AnimationType.animationName.ToString();

                    SO_AnimationType swapSO_AnimationType;
                    bool foundSwapAnimation = animationTypeDictionaryByCompositeAttributeKey.TryGetValue(key, out swapSO_AnimationType);

                    if (foundSwapAnimation)
                    {
                        AnimationClip swapAnimationClip = swapSO_AnimationType.animationClip;
                        animsKeyValuePairList.Add(new KeyValuePair<AnimationClip, AnimationClip>(animationClip, swapAnimationClip));
                    }
                }
            }
            // Apply animation updates to animation override controller and then update animator with the new controller
            aoc.ApplyOverrides(animsKeyValuePairList);
            currentAnimator.runtimeAnimatorController = aoc;
        }

        // s1.Stop();
        // UnityEngine.Debug.Log("Time to apply character customisation: " + s1.Elapsed + " elapeds seconds");
    }
}
