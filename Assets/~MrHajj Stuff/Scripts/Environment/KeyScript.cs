using UnityEngine;

public class KeyScript : PickupBase
{
    [Header("Key")]
    [SerializeField] DoorScript door;
    public override void PickupEffect(){
        base.PickupEffect();
        door.Unlock();
    }
}
