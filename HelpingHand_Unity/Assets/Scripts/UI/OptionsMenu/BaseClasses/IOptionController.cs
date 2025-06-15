using System;

public interface IOptionController
{
    public event Action<IOptionController> OnSelected;

    public string Description { get; }

    void SetDefault();
}
