using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform _movePoint;

    public void SetMovePoint(Transform movePoint) => _movePoint = movePoint;

    private void Awake()
    {
        if (_movePoint == null)
        {
            return;
        }
    }

    private void LateUpdate()
    {
        if (_movePoint == null) return;

        transform.position = _movePoint.position;
    }
}
