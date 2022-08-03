using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Rigidbody2D rigid;
    [SerializeField] SpriteRenderer myRend;

    [Space(10)]
    [SerializeField] AudioSource source;

    public void SetAlive(bool IsAlive)
    {
        rigid.isKinematic = !IsAlive;
        gameObject.SetActive(IsAlive);
        transform.position = new Vector2(0, -2.657f);
    }

    public void SetSprite(Sprite icon)
    {
        myRend.sprite = icon;
    }

    public void Move(float targetX)
    {
        if(transform.position.x + targetX < -2.2f || transform.position.x + targetX > 2.2f)
        {
            return;
        }

        if(source.isPlaying)
        {
            source.Stop();
        }

        source.Play();

        transform.position += new Vector3(targetX, 0, 0);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Manager.Instance.TakeDamage();
        collision.gameObject.SetActive(false);
    }
}
