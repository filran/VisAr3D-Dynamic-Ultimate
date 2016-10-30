using UnityEngine;
using System.Collections;

namespace View
{
    public class Click : MonoBehaviour
    {

        public GameObject MenuInteraction;
        public string Id;
        public string Type;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnMouseOver()
        {
            if (Input.GetMouseButtonDown(1))
            { //botão direito
                MenuInteraction.GetComponent<MenuInteraction>().HideRelationships = !MenuInteraction.GetComponent<MenuInteraction>().HideRelationships;
                MenuInteraction.GetComponent<MenuInteraction>().DesabilitarColliders();

                MenuInteraction.GetComponent<MenuInteraction>().ObjectClicked = this.gameObject;
                MenuInteraction.GetComponent<MenuInteraction>().IdObjectClicked = this.Id;
                MenuInteraction.GetComponent<MenuInteraction>().TypeObjectClicked = this.Type;

                MenuInteraction.GetComponent<MenuInteraction>().SetaGoToGameObject();
                MenuInteraction.GetComponent<MenuInteraction>().AddOnClickAosBotoesDoMenu(); 

                //print("Testando o botão direito! Este objeto é do tipo " + Type + " com ID " + Id);
            }
        }
    }
}