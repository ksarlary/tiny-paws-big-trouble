using UnityEngine;

public class PlayerAttackAnimationRelay : MonoBehaviour
{
    [SerializeField] private PlayerAttack playerAttack;

    private void Awake()
    {
        if (playerAttack == null)
        {
            playerAttack = GetComponentInParent<PlayerAttack>();
        }
    }

    public void PerformAttackHit()
    {
        if (playerAttack == null)
        {
            Debug.LogError(
                "PlayerAttackAnimationRelay: PlayerAttack reference is missing.",
                this
            );

            return;
        }

        playerAttack.PerformAttackHit();
    }

    public void FinishAttack()
    {
        if (playerAttack == null)
        {
            Debug.LogError(
                "PlayerAttackAnimationRelay: PlayerAttack reference is missing.",
                this
            );

            return;
        }

        playerAttack.FinishAttack();
    }
}