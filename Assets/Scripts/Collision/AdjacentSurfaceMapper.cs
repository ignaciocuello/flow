using RotaryHeart.Lib.SerializableDictionary;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AdjacentSurfaceMapper : MonoBehaviour {

    /* the directions in which a surface is checked */
    private static Vector2[] DIRECTIONS = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

    /* the fixed length of the results array, used to receive collisions from 'Cast' */
    private const int MAX_RESULTS_LENGTH = 10;

    [SerializeField]
    private float upCastDistance = 0.025f;
    [SerializeField]
    private float downCastDistance = 0.025f;
    [SerializeField]
    private float leftCastDistance = 0.1f;
    [SerializeField]
    private float rightCastDistance = 0.1f;

    /* the maximum distance to 'Cast' the collider in different directions (up, down, left and right) */
    private Dictionary<Vector2, float> castDistanceMap;

    private Collider2D collider2d;

    //the results array used to find collisions cast in different directions
    //don't want to have to create and destroy this constantly
    private RaycastHit2D[] resultsArray;

    void Awake()
    {
        collider2d = GetComponent<Collider2D>();
        resultsArray = new RaycastHit2D[MAX_RESULTS_LENGTH];

        castDistanceMap = new Dictionary<Vector2, float>()
        {
            { Vector2.up, upCastDistance },
            { Vector2.down, downCastDistance },
            { Vector2.left, leftCastDistance },
            { Vector2.right, rightCastDistance }
        };
    }

    public Dictionary<Vector2, GameObject[]> CalculateAdjacentSurfaceMap()
    {
        Dictionary<Vector2, GameObject[]> directionMap = new Dictionary<Vector2, GameObject[]>();
        GameObject[] collidedObjs;

        foreach (Vector2 direction in DIRECTIONS)
        {
            if ((collidedObjs = CollisionAgainstSurfaceIn(direction)) != null)
            {
                directionMap.Add(direction, collidedObjs);
            }
        }

        return directionMap;
    }

    GameObject[] CollisionAgainstSurfaceIn(Vector2 direction)
    {
        List<GameObject> collidedObjs = new List<GameObject>();

        int numCollisions = collider2d.Cast(direction, resultsArray, castDistanceMap[direction], ignoreSiblingColliders: true);
        for (int i = 0; i < numCollisions; i++)
        {
            RaycastHit2D hit = resultsArray[i];
            if (hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("Terrain"))
            {
                collidedObjs.Add(hit.collider.gameObject);
            }
        }

        return collidedObjs.Count != 0 ? collidedObjs.ToArray() : null;
    }
}
