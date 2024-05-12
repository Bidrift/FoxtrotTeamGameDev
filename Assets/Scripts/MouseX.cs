using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseX : MonoBehaviour
{
    public float sensitivity = 200f;

    // Update is called once per frame
    void Update()
    {
        if (InputManager.instance.movingCamera)
        {
            var horizontalAim = Input.GetAxis("Mouse X") * Time.deltaTime * sensitivity;
            transform.Rotate(0, horizontalAim, 0);
        }
    }
}
