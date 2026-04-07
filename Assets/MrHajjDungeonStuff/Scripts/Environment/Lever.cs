using UnityEngine;

public class Lever : MonoBehaviour, IInteractable
{
    [SerializeField] DoorScript door;
    public void Interacted(){
        door.Unlock();
    }
}
