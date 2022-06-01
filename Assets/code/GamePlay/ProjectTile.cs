using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectTile : MonoBehaviour
{
    private Vector2 direction;
    private float speed;

    public Vector2 Direction { get => direction; set => direction = value; }
    public float Speed { get => speed; set => speed = value; }

    private void Update()
    {
        Vector2 pos = direction * speed * Time.deltaTime * Netwoking.SERVER_UPDATE_TIME;
        transform.position += new Vector3(pos.x, pos.y, 0);
    }
}
