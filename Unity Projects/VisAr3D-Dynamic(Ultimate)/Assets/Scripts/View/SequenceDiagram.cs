//
// This class rendering ONE sequence diagram
//

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ThreeDUMLAPI;

namespace View
{
    public class SequenceDiagram : MonoBehaviour
    {
        #region PRIVATE VARS
        //This variable save a couple lifelines
        private Dictionary<Lifeline , GameObject> Lifelines = new Dictionary<Lifeline,GameObject>();
        //This variable save a couple lifelines
        private Dictionary<Method, GameObject> Methods = new Dictionary<Method, GameObject>();
        //Start Update line renderer
        private bool UpdateLineRenderer = false;
        #endregion

        #region PUBLIC VARS
        public GameObject LifelineGO { get; set; } //Prefab
        #endregion

        #region UNITY METHODS
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        #endregion

        #region PUBLIC METHODS

        //This main method rendering one sequence diagram
        public void renderSequenceDiagram(Package package)
        {
            //Show Diagrams
            foreach (ThreeDUMLAPI.SequenceDiagram sequencediagram in package.SequenceDiagrams)
            {
                //Create Diagram GameObject
                GameObject Diagram = new GameObject(sequencediagram.Name);

                //Counts
                float CountLifelines = 0;
                float AmountLifelines = sequencediagram.CountLifelines;

                //Show Lifelines
                foreach(Lifeline lifeline in sequencediagram.SoftwareEntities)
                {
                    //Count
                    CountLifelines++;

                    //Render each lifeline
                    GameObject l = (GameObject)Instantiate(LifelineGO, new Vector3(scale(lifeline.Left), 0, 0), Quaternion.identity);

                    //Save lifeline and his Gameobject
                    Lifelines.Add(lifeline, l);

                    //Set the name to Lifeline GameObject
                    l.name = lifeline.Name;
                    l.transform.FindChild("ObjectName").GetComponent<TextMesh>().text = lifeline.Name;

                    //Find the Line in Lifeline Prefab
                    Transform line = l.transform.FindChild("Line");

                    //Now... set the line height
                    line.transform.localScale = new Vector3(line.transform.localScale.x, scale(lifeline.Bottom), line.transform.localScale.z);

                    //Set the line position
                    line.transform.position = new Vector3(line.transform.position.x, -scale(lifeline.Bottom)*0.5f, line.transform.position.z);

                    //Set the messagens
                    setMessages();

                    //Set parent
                    l.transform.SetParent(Diagram.transform);
                }
            }
        }

        #endregion

        #region PRIVATE METHODS

        //Set the messagens
        private void setMessages()
        {
            //Loop in Couple Lifelines I
            foreach(KeyValuePair<Lifeline,GameObject> l in Lifelines)
            {
                //Loop Messages I
                foreach(Method m in l.Key.Methods){
                    //Loop in Couple Lifelines I
                    foreach (KeyValuePair<Lifeline, GameObject> ll in Lifelines)
                    {
                        //If the message has a destiny
                        if( m.IdTarget.Equals(ll.Key.Id) )
                        {
                            if (!IfDuplicated("id", m.Id, Methods))
                            {
                                //Create Method GameObject Parent
                                GameObject Method = new GameObject(m.Name);
                                Method.transform.position = new Vector3(centerMethodName(l.Value.transform.position,ll.Value.transform.position), scale(m.PtStartY) + .25f, l.Value.transform.position.z);

                                //Create binding dynamics with Line Renderer
                                GameObject LineRendererGO = new GameObject();
                                LineRendererGO.name = "LineRenderer";

                                LineRenderer LineRenderer = LineRendererGO.AddComponent<LineRenderer>();
                                LineRenderer.transform.position = new Vector3(l.Value.transform.position.x, scale(m.PtStartY), l.Value.transform.position.z);
                                LineRenderer.SetWidth(.05f, .2f);
                                LineRenderer.SetPosition(0, new Vector3(l.Value.transform.position.x, scale(m.PtStartY), l.Value.transform.position.z));
                                LineRenderer.SetPosition(1, new Vector3(ll.Value.transform.position.x, scale(m.PtStartY), ll.Value.transform.position.z));

                                //Set the messages name
                                GameObject MethodNameGO = new GameObject();
                                MethodNameGO.name = "Text";
                                MethodNameGO.transform.localPosition = Method.transform.position;

                                TextMesh MethodNameTextGO = MethodNameGO.AddComponent<TextMesh>();
                                MethodNameTextGO.text = m.Name;
                                MethodNameTextGO.characterSize = .15f;
                                MethodNameTextGO.anchor = TextAnchor.MiddleCenter;
                                MethodNameTextGO.alignment = TextAlignment.Center;

                                //Debug.Log("Method: " + m.Name + "\n\t" + "Lifeline Origin: " + l.Key.Name + " - x: " + l.Value.transform.position.x + "\n\t" + "Lifeline Destination: " + ll.Key.Name + " - x: " + ll.Value.transform.position.x + "\n\t" + "Distance: " + centerMethodName(l.Value.transform.position, ll.Value.transform.position));

                                //Set parents
                                LineRendererGO.transform.SetParent(Method.transform);
                                MethodNameGO.transform.SetParent(Method.transform);
                                Method.transform.SetParent(l.Value.transform);

                                //Add Method in Methods
                                Methods.Add(m, Method);
                            }
                        }
                    }

                }
            }
        }

        //Calculate the difference between two vectors
        private float centerMethodName(Vector3 origin, Vector3 destination)
        {
            return (origin.x + destination.x) / 2;
        }

        #region IfDuplicated
        private bool IfDuplicated(string att, string value, Dictionary<Lifeline, GameObject> list, bool r = false)
        {
            foreach (KeyValuePair<Lifeline, GameObject> l in list)
            {
                switch (att)
                {
                    case "id":
                        if (l.Key.Id.Equals(value))
                            r = true;
                        break;

                    case "name":
                        if (l.Key.Name.Equals(value))
                            r = true;
                        break;
                }
            }
            return r;
        }

        private bool IfDuplicated(string att, string value, Dictionary<Method, GameObject> list, bool r = false)
        {
            foreach (KeyValuePair<Method, GameObject> l in list)
            {
                switch (att)
                {
                    case "id":
                        if (l.Key.Id.Equals(value))
                            r = true;
                        break;

                    case "name":
                        if (l.Key.Name.Equals(value))
                            r = true;
                        break;
                }
            }
            return r;
        }

        private bool IfDuplicated(string att, string value, Dictionary<SoftwareEntity, SoftwareEntity> list, bool r = false)
        {
            foreach (KeyValuePair<SoftwareEntity, SoftwareEntity> l in list)
            {
                switch (att)
                {
                    case "id":
                        if (l.Key.Id.Equals(value))
                            r = true;
                        break;

                    case "name":
                        if (l.Key.Name.Equals(value))
                            r = true;
                        break;
                }
            }
            return r;
        }
        #endregion IfDuplicated

        //Return the scale to Unity
        private float scale(float n)
        {
            return n * 0.02f;
        }
        #endregion
    }
}