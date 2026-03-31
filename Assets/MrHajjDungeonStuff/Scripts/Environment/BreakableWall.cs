using UnityEngine;

public class BreakableWall : MonoBehaviour, IInteractable
{
    public void Interacted(){
        this.gameObject.SetActive(false);
    }
}
