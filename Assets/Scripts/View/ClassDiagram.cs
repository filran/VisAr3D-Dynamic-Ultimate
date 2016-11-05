//
// This class rendering ONE class diagram
// To facilitate, look for the class diagram with the name of the sequence diagram
//

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ThreeDUMLAPI;

namespace View
{
    public class ClassDiagram : MonoBehaviour
    {
        #region PRIVATE VARS
        float i = 0;
                                                                    //origin    destination
        private Dictionary<LineRenderer, Dictionary<GameObject, GameObject>> LineRenderes = new Dictionary<LineRenderer, Dictionary<GameObject, GameObject>>();

        #endregion

        #region PUBLIC VARS
        public Dictionary<Class, GameObject> Classes = new Dictionary<Class, GameObject>(); //key:class value:class like gameobject
        public GameObject ClassGO { get; set; } //Prefab
        //public Material lineMaterial;
        #endregion

        #region UNITY METHODS
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            UpdateLineRenderer();
        }
        #endregion

        #region PUBLIC METHODS
        //This main method rendering one class diagram
        public void renderClassDiagram(ThreeDUMLAPI.ClassDiagram classdiagram, Dictionary<string, IXmlNode> AllMessagesSignatures)
        {
            GameObject Diagram = new GameObject(classdiagram.Name);

            foreach (Class classe in classdiagram.SoftwareEntities)
            {
                float left = VirtualEnvironment.scale(classe.Position[classdiagram.Id]["Left"]);
                float top = -VirtualEnvironment.scale(classe.Position[classdiagram.Id]["Top"]);

                GameObject c = (GameObject)Instantiate(ClassGO, new Vector3(left, top, 0), Quaternion.identity);

                Classes.Add(classe, c);
                string atributos = "", metodos = "";
                c.name = classe.Name;

                if (classe.ClassMethodsCount > 0)
                {
                    #region loop_method
                    foreach (Method m in classe.ClassMethods)
                    {
                        //print(m.Id);
                        if (AllMessagesSignatures.ContainsKey(m.Id) && m.Name != classe.Name)
                        {
                            if (m.scope == "Private")
                            {
                                metodos += "-  ";
                            }
                            else if (m.scope == "Public")
                            {
                                metodos += "+  ";
                            }
                            else if (m.scope == "Protected")
                            {
                                metodos += "#  ";
                            }
                            metodos += (m.Name);
                            if (m.ClassMethodsParametersCount > 0)
                            {
                                string ret = ""; int k = 0; bool tem = false;
                                metodos += "(";
                                foreach (Parameter p in m.ClassMethodsParameters)
                                {
                                    if (p.Id.Contains("RETURNID"))
                                    {
                                        ret = p.Type; k++; tem = true;
                                    }
                                    else
                                    {
                                        metodos += p.Type;
                                        k++;
                                        if (k < m.ClassMethodsParametersCount)
                                        {
                                            metodos += ", ";
                                        }
                                    }
                                }
                                if (tem)
                                    metodos += "): " + ret + "\n\n";
                                else
                                    metodos += ")\n\n";
                            }
                            else
                            {
                                metodos += "\n";
                            }
                        }
                        else
                        {
                            classe.ClassMethodsCount--;
                        }
                    }
                    #endregion
                }

                if (classe.ClassAttributesCount > 0)
                {
                    foreach (Attribute at in classe.ClassAttributes)
                    {
                        atributos += (at.Name + "\n\n");
                    }
                }

                #region renderizando
                Transform classBox = c.transform.FindChild("ClassBox");
                Transform MethodsBox = c.transform.FindChild("MethodsBox");
                Transform FirstDiv = c.transform.FindChild("firstDivider");
                Transform Methods_text = c.transform.FindChild("Methods_text");
                Transform className = c.transform.FindChild("className");

                className.GetComponent<TextMesh>().text = classe.Name;
                className.transform.position = new Vector3(classBox.transform.position.x, classBox.transform.position.y, classBox.transform.position.z - 0.5f);

                classBox.transform.position = new Vector3(classBox.transform.position.x, classBox.transform.position.y, classBox.transform.position.z);

                FirstDiv.transform.position = new Vector3(FirstDiv.transform.position.x, classBox.transform.position.y - (classBox.localScale.y) * 0.5f, FirstDiv.transform.position.z);

                MethodsBox.transform.localScale = new Vector3(MethodsBox.transform.localScale.x, (classe.ClassMethodsCount) * 0.6f, MethodsBox.transform.localScale.z);
                MethodsBox.transform.position = new Vector3(MethodsBox.transform.position.x, FirstDiv.position.y - (MethodsBox.localScale.y) * 0.5f - FirstDiv.localScale.y, MethodsBox.transform.position.z);

                Methods_text.transform.position = new Vector3(MethodsBox.transform.position.x, MethodsBox.transform.position.y - 0.25f, MethodsBox.transform.position.z - 0.5f);
                Methods_text.GetComponent<TextMesh>().text = metodos;

                c.transform.parent = Diagram.transform;
                
                //resize
                c.transform.localScale = new Vector3(.6f, .6f, .6f);
                #endregion
            }

            #region cria os relacionamentos
            foreach (KeyValuePair<Class, GameObject> c in Classes)
            {
                string s = c.Key.Name + "\n";
                foreach (KeyValuePair<IXmlNode, IXmlNode> r in c.Key.Relationships)
                {
                    Dictionary<GameObject, GameObject> pairs = new Dictionary<GameObject, GameObject>();
                    if (r.Value != null)
                    {
                        GameObject line = new GameObject("relationship");
                        LineRenderer lineRender = line.AddComponent<LineRenderer>();
                        
                        lineRender.transform.parent = c.Value.transform;
                        lineRender.SetWidth(.15f,.15f);
                        lineRender.SetPosition(0, c.Value.transform.FindChild("firstDivider").transform.position);
                        lineRender.SetPosition(1, FindClasses(r.Value).transform.FindChild("firstDivider").transform.position);
                                                
                        pairs.Add(c.Value,FindClasses(r.Value));
                        LineRenderes.Add(lineRender , pairs);
                    }
                }
            }
            #endregion
        }
        #endregion

        #region PRIVATE METHODS        
        GameObject FindClasses(IXmlNode c)
        {
            GameObject g = new GameObject("FindClasses");

            foreach (KeyValuePair<Class, GameObject> gg in Classes)
            {
                g.transform.parent = gg.Value.transform;
                //print("C: " + c.Name + " gg.key: " + gg.Key.Name);
                if (c.Equals(gg.Key))
                {
                    g.transform.parent = gg.Value.transform;
                    //print("EQUAL     C: " + c.Name + " gg.key: " + gg.Key.Name);
                    g = gg.Value;
                }
            }
            return g;
        }
        
        void UpdateLineRenderer()
        {
            if(LineRenderes.Count > 0)
            {
                foreach (KeyValuePair<LineRenderer, Dictionary<GameObject, GameObject>> l in LineRenderes)
                {
                    foreach (KeyValuePair<GameObject, GameObject> g in l.Value)
                    {
                        l.Key.SetPosition(0, g.Key.transform.FindChild("firstDivider").transform.position);
                        l.Key.SetPosition(1, g.Value.transform.FindChild("firstDivider").transform.position);
                    }
                }
            }
        }
        #endregion
    }
}