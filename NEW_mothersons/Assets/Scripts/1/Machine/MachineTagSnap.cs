using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;

public class TagRestrictedSnapZone : XRSocketInteractor
{
    public List<string> acceptedTags;

    public override bool CanSelect(IXRSelectInteractable interactable)
    {
        return acceptedTags.Contains(interactable.transform.tag) && base.CanSelect(interactable);
    }
}
