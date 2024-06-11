using UnityEngine;
using System.Collections;

public class PlaneDestruction : MonoBehaviour
{
    public float destructionDuration = 5.0f; // Durée totale de la décomposition en secondes
    public GameObject[] planeParts; // Les parties de l'avion à désactiver progressivement

    private bool isDesintegrating = false;
    private float destructionStartTime;

    void OnCollisionEnter(Collision collision)
    {
        // Commencez la décomposition lorsque l'avion touche un objet ou le sol
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
            // Calculez la fraction de temps écoulé
            elapsedTime = Time.time - destructionStartTime;
            float fraction = elapsedTime / destructionDuration;

            // Désactivez progressivement les parties de l'avion
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

        // Désactivez toutes les parties à la fin de la décomposition
        foreach (var part in planeParts)
        {
            part.SetActive(false);
        }

        // Vous pouvez également détruire l'objet entier si nécessaire
        Destroy(gameObject);
    }
}
