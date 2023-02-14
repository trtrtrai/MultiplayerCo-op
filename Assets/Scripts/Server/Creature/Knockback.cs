using System.Collections;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    [SerializeField] Rigidbody2D rigid;
    [SerializeField] float strength, delay;
    [SerializeField] Vector3 impacter;

    public void Setup(Vector3 impacter, float strength, float delay)
    {
        this.strength = strength;
        this.delay = delay;
        this.impacter = impacter;
        rigid = GetComponent<Rigidbody2D>();

        PlayFeedback();
    }

    private void PlayFeedback()
    {
        Vector2 direction = (transform.position - impacter).normalized;
        rigid.AddForce(direction * (impacter.y < gameObject.transform.position.y ? strength + 0.5f : strength), ForceMode2D.Impulse);
        //immun
        StartCoroutine(Reset());
    }

    private IEnumerator Reset()
    {
        yield return new WaitForSeconds(delay);
        Destroy(this);
    }
}
