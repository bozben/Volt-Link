using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    private void Start()
    {
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            float length = anim.GetCurrentAnimatorStateInfo(0).length;
            Destroy(gameObject, length);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}