using UnityEngine;
using System.Collections;

public class PlaneDestruction : MonoBehaviour
{
    public float destructionDuration = 5.0f; // Dur�e totale de la d�composition en secondes
    public GameObject[] planeParts; // Les parties de l'avion � d�sactiver progressivement

    private bool isDesintegrating = false;
    private float destructionStartTime;

    void OnCollisionEnter(Collision collision)
    {
        // Commencez la d�composition lorsque l'avion touche un objet ou le sol
        if (!isDesintegrating)
        {
            isDesintegrating = true;
            destructionStartTime = Time.time;
            StartCoroutine(Desintegrate());
        }
    }

    private IEnumerator Desintegrate()
    {
        float elapsedTime = 0f;

        while (elapsedTime < destructionDuration)
        {
            // Calculez la fraction de temps �coul�
            elapsedTime = Time.time - destructionStartTime;
            float fraction = elapsedTime / destructionDuration;

            // D�sactivez progressivement les parties de l'avion
            int partsToDisable = Mathf.FloorToInt(fraction * planeParts.Length);

            for (int i = 0; i < partsToDisable; i++)
            {
                if (planeParts[i].activeSelf)
                {
                    planeParts[i].SetActive(false);
                }
            }

            yield return null;
        }

        // D�sactivez toutes les parties � la fin de la d�composition
        foreach (var part in planeParts)
        {
            part.SetActive(false);
        }

        // Vous pouvez �galement d�truire l'objet entier si n�cessaire
        Destroy(gameObject);
    }
}
