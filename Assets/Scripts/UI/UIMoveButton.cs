using InputModule;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;

public class UIMoveButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField] private DirectionValue _directionValue;

    private IInputService _inputService;

    [Inject]
    public void Construct(IInputService inputService)
    {
        _inputService = inputService;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _inputService.SetUiMoveValue(_directionValue);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _inputService.SetUiMoveValue(DirectionValue.None);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _inputService.SetUiMoveValue(DirectionValue.None);
    }
}