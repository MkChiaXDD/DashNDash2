using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    [SerializeField] private Animator anim;
    
    public void PlayDash()
    {
        anim.SetTrigger("Dash");
    }

    public void StopDash()
    {
        anim.SetTrigger("StopDash");
    }
}
