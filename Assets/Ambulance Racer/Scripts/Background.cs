using UnityEngine;

public class Background : MonoBehaviour
{
    public float speed;
    [SerializeField] Vector2 dir;
    [SerializeField] Material mat;

    private void Update()
    {
        mat.mainTextureOffset = speed * Time.time * dir;
    }
}