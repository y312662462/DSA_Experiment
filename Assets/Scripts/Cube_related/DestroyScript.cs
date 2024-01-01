using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyScript : MonoBehaviour
{
    public GameObject m_Blue_Particle_Prefab;
    public GameObject m_Red_Particle_Prefab;
    public GameObject m_Gray_Particle_Prefab;

    [HideInInspector] public ScoreManager scoreManager;

    private Vector3 previousePos;
    private InstantiateObjects instantiateObjects;

    private SliceSoundManager sliceSoundManager;







    void Start()
    {
        if (FindObjectOfType<SliceSoundManager>() != null)
        {
            sliceSoundManager = FindObjectOfType<SliceSoundManager>();
        }
        StartCoroutine(FindScoreManager());
        instantiateObjects = FindObjectOfType<InstantiateObjects>();

    }



    void Update()
    {
        previousePos = transform.position;

    }


    private void OnTriggerEnter(Collider other)
    {
        if (Vector3.Angle(transform.position - previousePos, other.transform.up) > 100 || Vector3.Angle(transform.position - previousePos, -other.transform.up) > 100)
        {


            if (other.gameObject.CompareTag("BlueCube"))
            {

                sliceSoundManager.PlaySliceSound();

                GameObject blueParticle = Instantiate(m_Blue_Particle_Prefab, other.transform.position, Quaternion.identity, instantiateObjects.transform);

                Destroy(blueParticle, 2f);

                Destroy(other.transform.parent.gameObject);

                scoreManager.animator.SetInteger("rand", Random.Range(-2, 3));


                if (gameObject.CompareTag("BlueSaber"))
                {
                    scoreManager.CorrectColorCube();
                }

                else if (gameObject.CompareTag("RedSaber"))
                {
                    scoreManager.WrongColorCube();
                }

            }
            else if (other.gameObject.CompareTag("RedCube"))
            {
                sliceSoundManager.PlaySliceSound();

                GameObject redParticle = Instantiate(m_Red_Particle_Prefab, other.transform.position, Quaternion.identity, instantiateObjects.transform);

                Destroy(redParticle, 2f);

                Destroy(other.transform.parent.gameObject);

                scoreManager.animator.SetInteger("rand", Random.Range(-2, 3));


                if (gameObject.CompareTag("RedSaber"))
                {
                    scoreManager.CorrectColorCube();
                }

                else if (gameObject.CompareTag("BlueSaber"))
                {
                    scoreManager.WrongColorCube();
                }
            }

            else if (other.gameObject.CompareTag("PulishCube"))
            {
                sliceSoundManager.PlaySliceSound();

                GameObject grayParticle = Instantiate(m_Gray_Particle_Prefab, other.transform.position, Quaternion.identity, instantiateObjects.transform);

                Destroy(grayParticle, 2f);

                Destroy(other.transform.parent.gameObject);

                scoreManager.animator.SetInteger("rand", Random.Range(-2, 3));//what`s the function of the animator?


                scoreManager.PulishCube();
            }
        }
    }

    public IEnumerator FindScoreManager()
    {
        yield return new WaitForSeconds(3.5f);
        scoreManager = FindObjectOfType<ScoreManager>();

    }
}
