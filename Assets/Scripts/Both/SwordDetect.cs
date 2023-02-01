using Assets.Scripts.Both.Creature.Attackable.SkillExecute;
using System.Collections;
using UnityEngine;

public class SwordDetect : SkillActive
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        //Debug.Log("Sword Skill Start");
    }

    protected override void SpecializedBehaviour()
    {
        base.SpecializedBehaviour();

        StartCoroutine(SlashDuration());
        var orien = owner.Animator.GetInteger("orientation");
        owner.Animator.SetInteger("orientation", orien < 0 ? orien : -orien);
        owner.Animator.SetBool("isAttack", true);
    }

    private IEnumerator SlashDuration() //Delay animation
    {
        yield return new WaitForSeconds(skillTags[0].Duration);

        owner.Animator.SetBool("isAttack", false);
        owner.Animator.SetInteger("orientation", Mathf.Abs(owner.Animator.GetInteger("orientation")));
    }
}
