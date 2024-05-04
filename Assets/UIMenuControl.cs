using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIMenuControl : MonoBehaviour
{
    public GameObject NonNetworkedGameObject;
    public InputActionReference UIActivateReference = null;

    // Start is called before the first frame update
    void Start()
    {
        NonNetworkedGameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
