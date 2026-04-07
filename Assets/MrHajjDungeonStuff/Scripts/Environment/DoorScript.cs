using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public void Unlock(){
        this.gameObject.SetActive(false);
    }
}
