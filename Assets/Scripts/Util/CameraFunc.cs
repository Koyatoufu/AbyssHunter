using UnityEngine;
using System.Collections;

public static class CameraFunc
{
    public static IEnumerator ShakeCoroutine(float fDuration,float fMagnitude)
    {
        Camera camera = Camera.main;

        if (camera == null)
            yield break;

        Vector3 orgin = camera.transform.localPosition;

        while(fDuration>0f)
        {
            fDuration -= Time.deltaTime;

            float x = Random.Range(-1f, 1f);
            float y = Random.Range(-1f, 1f);

            camera.transform.localPosition = new Vector3(x + orgin.x * fMagnitude, orgin.y + y*fMagnitude, orgin.z);
            yield return null;
        }

        camera.transform.localPosition = new Vector3(0f,1f,-5f);
        yield return null;
    }
}
