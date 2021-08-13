using UnityEngine;

public class Cursor : MonoBehaviour 
{    
	void Update () 
	{
        if (Time.timeScale == 0)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, float.MaxValue, 1 << LayerMask.NameToLayer("Ground")))
        {
            transform.position = hit.point;
        }

        transform.rotation = Quaternion.identity;
	}
}
