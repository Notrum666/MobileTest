using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float tubeRadius;
    [SerializeField]
    float controlSensetivity;
    [SerializeField]
    GameObject obstacleManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!obstacleManager.GetComponent<ObstacleManager>().IsGameActive)
            return;

        if (Input.touches.Length > 0)
        {
            Vector3 delta = new Vector3(Input.touches[0].deltaPosition.x, 0.0f, Input.touches[0].deltaPosition.y) * controlSensetivity;

            Vector3 newPosition = gameObject.transform.position + delta;
            if (newPosition.sqrMagnitude > tubeRadius * tubeRadius)
                newPosition = newPosition.normalized * tubeRadius;

            gameObject.transform.position = newPosition;
        }

        Ray ray = new Ray(transform.position, Vector3.down);
    }
}
