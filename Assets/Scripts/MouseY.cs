using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseY : MonoBehaviour
{
    public float sensetivity = 250f;
    public float maxAngle = 90;
    public float minAngle = -90;

    // Update is called once per frame
    void Update()
    {
        if (InputManager.instance.movingCamera)
        {
            _verticalRot -= Input.GetAxis("Mouse Y") * Time.deltaTime * sensetivity;

            _verticalRot = Mathf.Clamp(_verticalRot, minAngle, maxAngle);

            transform.localEulerAngles = new Vector3(_verticalRot, 0, 0);
        }
    }

    private float _verticalRot = 0;
}
