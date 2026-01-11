using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float forwardOffset;
    [SerializeField] private float xFollowSpeed;
    [SerializeField] private float yFollowSpeed;

    private float targetX;
    private float targetY;

    private void FixedUpdate()
    {
        targetX = Mathf.Lerp(targetX, (forwardOffset * player.localScale.x), Time.deltaTime * xFollowSpeed);
        targetY = Mathf.Lerp(transform.position.y, player.position.y, Time.deltaTime * yFollowSpeed);
        transform.position = new Vector3(player.position.x + targetX, targetY, transform.position.z);
    }
}
