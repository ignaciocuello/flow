using UnityEngine;

[ExecuteInEditMode]
public class RecallBase : MonoBehaviour
{
    //TODO edit as only works with one playere
    private Player player;

    [SerializeField]
    private GameObject body;

    [SerializeField]
    private GameObject phantomPrefab;

    [SerializeField]
    private float rotationSpeed;

    /* the child triangle of this recall base */
    [SerializeField]
    private Transform triangleTransform;

    /* the last position in which the player was in this collider */
    //private Vector3 previousPlayerPos;

    [SerializeField]
    private bool stickToBase;

    // Update is called once per frame
    void Update()
    {
        if (stickToBase)
        {
            transform.localPosition = new Vector3(-body.transform.localScale.x / 2.0f, 0.0f, 0.0f);

            Vector3 pos = transform.position;
            transform.position = new Vector3(pos.x +
                (transform.parent.localScale.x < 0.0f ? -1.0f : 1.0f) * transform.localScale.x / 2.0f, pos.y, pos.z);
        }

        if (player != null)
        {
            float rotationAngle = Vector2.SignedAngle(Vector2.up, player.transform.position - triangleTransform.position);
            triangleTransform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationAngle);
        }
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            player = collider.GetComponent<Player>();
            //destroy any old phantoms
            //player.ClearPhantom();
        }
    }

    public void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            //previousPlayerPos = player.transform.position;
        }
    }

    public void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            //destroy any old phantoms
            /*player.ClearPhantom();

            player.Phantom = Instantiate(phantomPrefab).GetComponent<Phantom>();

            SpriteRenderer sr = player.Phantom.GetComponent<SpriteRenderer>();

            sr.sprite = player.EntitySpriteRenderer.sprite;
            sr.flipX = player.EntitySpriteRenderer.flipX;
            sr.transform.position = previousPlayerPos;
            //THIS MIGHT HAVE BROKENN IT, was player.EntitySpriteRenderer.transform.localScale before
            sr.transform.localScale = player.transform.localScale;

            player = null;*/
        }
    }
}
