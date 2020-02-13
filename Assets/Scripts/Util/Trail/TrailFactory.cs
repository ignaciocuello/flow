using UnityEngine;

public class TrailFactory : Singleton<TrailFactory> {

    [SerializeField]
    private GameObject trailParentPrefab;

    public GameObject CreateTrailParent(string parentName, bool mirrorSpriteRenderer, GameObject trailChildPrefab)
    {
        GameObject trailParentObj = Instantiate(trailParentPrefab, transform);

        TrailParent trailParent = trailParentObj.GetComponent<TrailParent>();
        trailParent.TrailChildPrefab = trailChildPrefab;
        trailParent.MirrorSpriteRenderer = mirrorSpriteRenderer;

        trailParentObj.name = parentName;

        return trailParentObj;
    }
}
