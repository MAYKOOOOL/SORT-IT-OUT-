using UnityEngine;

public class parahiro : MonoBehaviour
{
    private Animator animator;
    private bool isPlaying = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            if (!isPlaying)
            {
                animator.Play("walk"); // Replace with the name of your walk animation
                isPlaying = true;
            }
        }
        else
        {
            if (isPlaying)
            {
                animator.Play("idle"); // Replace with your idle animation name
                isPlaying = false;
            }
        }
    }
}
