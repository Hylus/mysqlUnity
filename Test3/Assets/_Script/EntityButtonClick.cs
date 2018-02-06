using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntityButtonClick : MonoBehaviour {

    private void Awake()
    {
       gameObject.GetComponent<Button>().onClick.AddListener(() =>
       {
           OnClick(gameObject.GetComponent<Button>());
       });
    }

    public void OnClick(Button button)
    {
        var cmd = new EntityButtonClickCmd();
        cmd.Button = button;
        Mediator.Instance.Publish<EntityButtonClickCmd>(cmd);    
    }
}
