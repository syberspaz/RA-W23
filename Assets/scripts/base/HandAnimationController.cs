using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//based on this video: https://youtu.be/8PCNNro7Rt0
public class HandAnimationController : MonoBehaviour
{
    [SerializeField] InputActionProperty triggerValueAction;
    [SerializeField] InputActionProperty gripValueAction;
    
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float triggerValue = triggerValueAction.action.ReadValue<float>();
        animator.SetFloat("Trigger", triggerValue);

        float gripValue = gripValueAction.action.ReadValue<float>();
        animator.SetFloat("Grip", gripValue);
    }
}
