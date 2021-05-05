using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spinBox : MonoBehaviour
{
    [Range (50,150)]
    public float spinSpeed;
    // Start is called before the first frame update
    void Start()
    {
        spinSpeed = 100;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, spinSpeed, 0) * Time.deltaTime);
    }
}
