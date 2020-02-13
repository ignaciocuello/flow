using UnityEngine;

public class ParticleDestroyOnFinish : MonoBehaviour {

	void Update () {
		if (!GetComponent<ParticleSystem>().IsAlive())
        {
            Destroy(gameObject);
        }
	}
}
