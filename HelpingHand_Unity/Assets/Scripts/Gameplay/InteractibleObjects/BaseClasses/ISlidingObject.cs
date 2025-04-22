using System;

public interface ISlidingObject
{
    float SliderMaxValue { get; }
    float SliderMinValue { get; }
    float SliderValue { get; }
    float SliderSpeed { get; }
    float SliderMovementDirection { get; }

    event Action<float> OnSliderValueChanged;
    event Action<bool> OnSliderPointerDown;
}
