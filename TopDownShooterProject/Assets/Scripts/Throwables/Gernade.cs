using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Gernade : Throwable
{
    public ParticleSystem Explosion;
    public float blastRadius;
    public AudioClip explosionSound;
    public new AudioSource audio;
    public override void Landed()
    {
        StartCoroutine(BlowUp());
    }
    public override void AudioCallout()
    {
        AudioManger.instance.Play("Gernade");
    }
    IEnumerator BlowUp()
    {
        transform.rotation = Quaternion.Euler(-90, 0, 0);
        
        yield return new WaitForSeconds(.5f);

        audio.PlayOneShot(explosionSound);

        Collider[] colliders = Physics.OverlapSphere(transform.position, blastRadius);

        Explosion.Play();

        foreach (Collider targets in colliders)
        {
            // Hit here
            if (targets.tag == "Enemy")
            {
                targets.GetComponent<EnemyManager>().Damage(100);
            }
        }

        yield return new WaitForSeconds(Explosion.main.duration);

        Explosion.Stop();
        Destroy(gameObject, .2f);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Gernade))]
public class GernadeExplosionRadius : Editor 
{
	void OnSceneGUI() {

		Gernade gernade = (Gernade)target;
        Handles.color = Color.red; 

        Handles.DrawWireArc(gernade.transform.position, Vector3.up, Vector3.forward, 360, gernade.blastRadius);
	}
}
#endif