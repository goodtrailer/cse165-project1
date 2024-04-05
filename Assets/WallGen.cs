using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGen : MonoBehaviour
{
    public GameObject brickPrefab;
    public float radius;
    public int count;
    public int wallHeight;
    public float brickHeight;


    void spawnRing(int h, float offset) {

        for(int i = 0; i < count; i++) {
            float theta = 2*Mathf.PI / count * (i+offset);
            float deg = Mathf.Rad2Deg * -theta + 90;

            
            Vector3 pos = new Vector3(); 
            pos.x = radius * Mathf.Cos(theta);
            pos.y = h*brickHeight;
            pos.z = radius * Mathf.Sin(theta);

            Quaternion rot = Quaternion.Euler(new Vector3(0, deg, 0));

            Object.Instantiate(brickPrefab, pos, rot, transform);
        }

    }

    // Start is called before the first frame update
    void Start()
    {

        for (int h = 0; h < wallHeight; h++) {
            float offset = h%2 == 1 ? 0.5f : 0.0f;
            spawnRing(h,offset);
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
