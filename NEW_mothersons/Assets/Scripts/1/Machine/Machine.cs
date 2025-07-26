using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MachineSequence : MonoBehaviour
{
    public Transform part1;
    public Transform part2;
    public Transform part3;
    public float SecondObjectYTarget = -0.545f;
    public float ThirdObjectXTarget = 0;
    public float moveDuration = 0.5f;
    public Renderer buttonRenderer;
    private Color buttonOriginalColor;

    public TagRestrictedSnapZone snapZone; // Assign in inspector
    public XRInteractionManager interactionManager; // Assign this too (usually in your scene)

    private Vector3 part1StartRot;
    private Vector3 part2StartPos;
    private Vector3 part3StartPos;

    private void Start()
    {
        part1StartRot = part1.eulerAngles;
        part2StartPos = part2.position;
        part3StartPos = part3.position;
        buttonOriginalColor = buttonRenderer.material.color;
    }

    public void StartSequence()
    {
        StopAllCoroutines();
        StartCoroutine(RunSequence());
    }

    private IEnumerator RunSequence()
    {
        buttonRenderer.material.color = Color.red;
        // 1. Rotate part1 to Y = 0
        yield return RotateTo(part1, new Vector3(part1StartRot.x, 0f, part1StartRot.z), 1f);

        //// 2. Move part2 to Y = 2.882
        //Vector3 part2Target = new Vector3(part2StartPos.x, 0, part2StartPos.z);
        //yield return MoveTo(part2, part2Target, 1f);

        Vector3 firstTarget = new Vector3(part2.localPosition.x, SecondObjectYTarget, part2.localPosition.z);
        Coroutine firstMove = StartCoroutine(MoveToPosition(part2, firstTarget, moveDuration));


        // 3. Wait 5 sec
        yield return new WaitForSeconds(5f);

        // 4. Return part1 and part2
        yield return RotateTo(part1, part1StartRot, 1f);
        yield return MoveTo(part2, part2StartPos, 1f);

        // 5. Unsnapping before moving part3
        GameObject snappedObject = null;
        if (snapZone.hasSelection)
        {
            var interactable = snapZone.firstInteractableSelected;
            snappedObject = interactable.transform.gameObject;
            interactionManager.SelectExit(snapZone, interactable); // Force unsnap
            snapZone.enabled = false;
        }
        yield return new WaitForSeconds(2f);

        Vector3 SecondTarget = new Vector3( ThirdObjectXTarget, part3.localPosition.y, part3.localPosition.z);
        Coroutine SecondMove = StartCoroutine(MoveToPosition(part3, SecondTarget, moveDuration));

        yield return new WaitForSeconds(1f);

        yield return MoveTo(part3, part3StartPos, 1f);
        snapZone.enabled = true;
        if (snappedObject != null)
        {
            GrabActivateObject tooltip = snappedObject.GetComponent<GrabActivateObject>();
            if (tooltip != null)
            {
                tooltip.enabled = true;
            }
        }
        buttonRenderer.material.color = buttonOriginalColor;
    }

    IEnumerator MoveToPosition(Transform target, Vector3 destination, float duration)
    {
        Vector3 start = target.localPosition;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            target.localPosition = Vector3.Lerp(start, destination, t);
            yield return null;
        }
        target.localPosition = destination;
    }
    private IEnumerator RotateTo(Transform target, Vector3 toRotation, float duration)
    {
        Quaternion from = target.rotation;
        Quaternion to = Quaternion.Euler(toRotation);
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            target.rotation = Quaternion.Slerp(from, to, t);
            yield return null;
        }
    }

    private IEnumerator MoveTo(Transform target, Vector3 toPosition, float duration)
    {
        Vector3 from = target.position;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            target.position = Vector3.Lerp(from, toPosition, t);
            yield return null;
        }
    }
}
