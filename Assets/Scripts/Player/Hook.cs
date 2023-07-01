using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour
{
    public Player Player;

    private Rigidbody2D m_Rigidbody;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
    }

    public void Launch(Vector2 direction, float power)
    {
        m_Rigidbody.AddForce(direction * power, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("TetherableWall"))
        {
            Stick();
        }
    }

    private void Stick()
    {
        m_Rigidbody.bodyType = RigidbodyType2D.Static;
        if (Player != null)
        {
            Player.MoveToHook((Vector2) transform.position);
        }
    }
}
