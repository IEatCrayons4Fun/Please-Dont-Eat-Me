using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : PickupBase
{
    public UnityEvent triggered;

    public override void PickupEffect(){
        triggered?.Invoke();
    }
}
