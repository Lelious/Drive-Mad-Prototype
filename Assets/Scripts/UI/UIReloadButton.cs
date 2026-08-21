using InputModule;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;

public class UIReloadButton : MonoBehaviour, IPointerClickHandler
{
    private IInputService _inputService;

    [Inject]
    public void Construct(IInputService inputService)
    {
        _inputService = inputService;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _inputService.Reload();
    }
}
