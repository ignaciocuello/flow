using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TerrainUtil : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		if (transform.localScale.x != 1.0f || transform.localScale.y != 1.0f)
        {
            Vector2 size = transform.localScale;

            GetComponent<SpriteRenderer>().size = size;
            GetComponent<BoxCollider2D>().size = size;

            transform.localScale = Vector3.one;
        }

        if (GetComponent<SpriteRenderer>().size.x < 0.0f)
        {
            Vector2 size = GetComponent<SpriteRenderer>().size;
            GetComponent<SpriteRenderer>().size = new Vector2(-size.x, size.y);

            GetComponent<BoxCollider2D>().size = new Vector2(-size.x, GetComponent<BoxCollider2D>().size.y);
        }
	}
}
