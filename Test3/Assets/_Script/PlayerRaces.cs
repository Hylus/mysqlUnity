using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRaces : MonoBehaviour {

    [SerializeField] int maxEntityQuantity;
    [SerializeField] GameObject prefabButton;
    [SerializeField] Button addEntityButton;


    private void Awake()
    {
        Mediator.Instance.Subscribe<CreatedNewEntityCmd>(OnCreatedNewEntityCmd);
        Mediator.Instance.Subscribe<RefreshListCmd>(OnRefreshListCmd);
    }

    void Start()
    {
        InspectEntityButton();
        ShowCreatedEntities();
    }

    void ClearEntityShowList()
    {
        Button[] b = gameObject.GetComponentsInChildren<Button>();
        for(int i=1; i< b.Length; i++)
        {
            Destroy(b[i].gameObject);
        }
    }

    void InspectEntityButton()
    {
        if (DataBaseConnection.Instance.EntityQuantity() >= maxEntityQuantity)
        {
            addEntityButton.interactable = false;
            addEntityButton.GetComponentInChildren<Text>().text = "You've got max " + maxEntityQuantity + " entities";
        }
        else
        {
            addEntityButton.interactable = true;
        }
    }

    void OnCreatedNewEntityCmd(CreatedNewEntityCmd cmd)
    {
        ShowCreatedEntities();
        InspectEntityButton();
    }

    void ShowCreatedEntities()
    {       
        int entityAmonut = DataBaseConnection.Instance.EntityQuantity();

        if (entityAmonut == -1)
        {
            Debug.LogError("Database Interrupt");
        }
        else if (entityAmonut > 0)
        {
            string[] entities = new string[entityAmonut];
            DataBaseConnection.Instance.EntitiesName(ref entities);

            ClearEntityShowList();
            foreach (var entity in entities)
            {
                GameObject go = Instantiate(prefabButton, transform);
                go.transform.SetAsLastSibling();
                go.GetComponentInChildren<Text>().text = entity;
            } 
        }
    }

    void OnRefreshListCmd(RefreshListCmd cmd)
    {
        Refresh();
    }

    public void Refresh()
    {
        ClearEntityShowList();
        ShowCreatedEntities();

    }

    private void OnDestroy()
    {
        Mediator.Instance.DeleteSubscriber<CreatedNewEntityCmd>(OnCreatedNewEntityCmd);
        Mediator.Instance.DeleteSubscriber<RefreshListCmd>(OnRefreshListCmd);
    }


}
