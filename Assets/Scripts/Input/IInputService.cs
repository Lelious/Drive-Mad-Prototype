namespace InputModule
{
    public interface IInputService
    {
        public bool IsLocked { get; set; }
        public DirectionValue MoveDirection { get; }
        public void SetUiMoveValue(DirectionValue value);
        public void Reload();
    }
}
