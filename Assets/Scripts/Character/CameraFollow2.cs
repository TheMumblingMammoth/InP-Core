using UnityEngine;

public class CameraFollow2 : MonoBehaviour
{
    private Transform player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, player.position-Vector3.forward*10, 0.2f); // -Vector3.forward ��� ����, ����� ������ ���������� ����� ������, � �� ������ 
    }
}
