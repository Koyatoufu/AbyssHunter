using UnityEngine;

public class CBillBoard : MonoBehaviour
{
    void Update ()
    {
        if (Camera.main == null)
            return;

        Vector3 dir = transform.position - Camera.main.transform.position;

        transform.rotation = Quaternion.LookRotation(dir.normalized);
	}
}
