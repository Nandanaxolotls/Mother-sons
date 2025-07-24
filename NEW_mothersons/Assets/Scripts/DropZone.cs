using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

public class DropZone : MonoBehaviour
{
    public string acceptedTag;
    public int rewardPoints = 1;
    public int penaltyPoints = -1;
    public float delayBeforeCheck = 0.2f;
    public float feedbackDuration = 1.0f;

    [Header("Visual Feedback")]
    public Renderer zoneRenderer; // assign the Renderer of the drop box
    public Color correctColor = Color.green;
    public Color incorrectColor = Color.red;
    private Color originalColor;

    private void Start()
    {
        if (zoneRenderer == null)
            zoneRenderer = GetComponent<Renderer>();

        if (zoneRenderer != null)
            originalColor = zoneRenderer.material.color;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Defected") || other.CompareTag("Undefected"))
        {
            StartCoroutine(HandleDropObject(other.gameObject));
        }
    }

    private IEnumerator HandleDropObject(GameObject obj)
    {
        yield return new WaitForSeconds(delayBeforeCheck);

        if (obj == null)
            yield break;

        XRGrabInteractable grabInteractable = obj.GetComponent<XRGrabInteractable>();
        if (grabInteractable != null && grabInteractable.isSelected)
            yield break; // Still held

        string objectTag = obj.tag;

        if (objectTag == acceptedTag)
        {
            ScoreManager.instance.AddPoints(rewardPoints);
            Debug.Log("Correct drop! +" + rewardPoints);
            StartCoroutine(FlashZoneColor(correctColor));
        }
        else
        {
            ScoreManager.instance.AddPoints(penaltyPoints);
            Debug.Log("Incorrect drop! " + penaltyPoints);
            StartCoroutine(FlashZoneColor(incorrectColor));
        }

        Destroy(obj);
    }

    private IEnumerator FlashZoneColor(Color color)
    {
        if (zoneRenderer != null)
        {
            zoneRenderer.material.color = color;
            yield return new WaitForSeconds(feedbackDuration);
            zoneRenderer.material.color = originalColor;
        }
    }
}
