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
        int i = 0;
        private Dictionary<Class, GameObject> Classes { get; set; } //key:class value:class like gameobject
                                                                    //origin    destination
        private Dictionary<LineRenderer, Dictionary<GameObject, GameObject>> LineRenderes = new Dictionary<LineRenderer, Dictionary<GameObject, GameObject>>();

        #endregion

        #region PUBLIC VARS
        public GameObject ClassGO { get; set; } //Prefab
        public Material lineMaterial;

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
            Classes = new Dictionary<Class, GameObject>();
            foreach (Class classe in classdiagram.SoftwareEntities)
            {
                GameObject c = (GameObject)Instantiate(ClassGO, new Vector3(i, 0, 0), Quaternion.identity);
                Classes.Add(classe, c);
                string atributos = "", metodos = "";
                c.name = classe.Name;

                if (classe.ClassMethodsCount > 0)
                {
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
                }
                if (classe.ClassAttributesCount > 0)
                {
                    foreach (Attribute at in classe.ClassAttributes)
                    {
                        //Debug.Log("AT ---> " + at.Name + "\n\n");
                        atributos += (at.Name + "\n\n");
                    }
                }
                Transform classBox = c.transform.FindChild("ClassBox");
                Transform AttributesBox = c.transform.FindChild("AttributesBox");
                Transform MethodsBox = c.transform.FindChild("MethodsBox");
                Transform FirstDiv = c.transform.FindChild("firstDivider");
                Transform SecDiv = c.transform.FindChild("secondDivider");
                Transform Attributes_text = c.transform.FindChild("Attributes_text");
                Transform Methods_text = c.transform.FindChild("Methods_text");
                Transform className = c.transform.FindChild("className");

                className.GetComponent<TextMesh>().text = classe.Name;

                classBox.transform.position = new Vector3(classBox.transform.position.x, classBox.transform.position.y, classBox.transform.position.z);
                className.transform.position = new Vector3(classBox.transform.position.x, classBox.transform.position.y, classBox.transform.position.z - 0.5f);

                FirstDiv.transform.position = new Vector3(FirstDiv.transform.position.x, classBox.transform.position.y - (classBox.localScale.y) * 0.5f, FirstDiv.transform.position.z);

                AttributesBox.transform.localScale = new Vector3(AttributesBox.transform.localScale.x, classe.ClassAttributesCount * 0.65f, AttributesBox.transform.localScale.z);
                AttributesBox.transform.position = new Vector3(AttributesBox.transform.position.x, FirstDiv.position.y - (AttributesBox.localScale.y) * 0.5f - FirstDiv.localScale.y, AttributesBox.transform.position.z);

                Attributes_text.transform.position = new Vector3(AttributesBox.transform.position.x, AttributesBox.transform.position.y - 0.25f, AttributesBox.transform.position.z - 0.5f);
                Attributes_text.GetComponent<TextMesh>().text = atributos;

                SecDiv.transform.position = new Vector3(SecDiv.transform.position.x, AttributesBox.transform.position.y - (AttributesBox.localScale.y) * 0.5f, SecDiv.transform.position.z);

                MethodsBox.transform.localScale = new Vector3(MethodsBox.transform.localScale.x, (classe.ClassMethodsCount) * 0.6f, MethodsBox.transform.localScale.z);
                MethodsBox.transform.position = new Vector3(MethodsBox.transform.position.x, SecDiv.position.y - (MethodsBox.localScale.y) * 0.5f - SecDiv.localScale.y, MethodsBox.transform.position.z);

                Methods_text.transform.position = new Vector3(MethodsBox.transform.position.x, MethodsBox.transform.position.y - 0.25f, MethodsBox.transform.position.z - 0.5f);
                Methods_text.GetComponent<TextMesh>().text = metodos;


                i += 7;

            }

            foreach (KeyValuePair<Class, GameObject> c in Classes)
            {
                string s = c.Key.Name + "\n";
                foreach (KeyValuePair<IXmlNode, IXmlNode> r in c.Key.Relationships)
                {
                    GameObject line = new GameObject("Line Renderer");
                    line.name = c.Value.name;
                    Dictionary<GameObject, GameObject> pairs = new Dictionary<GameObject, GameObject>();
                    print(c.Key.Name + "  " + c.Value.name);
                    if (r.Value != null)
                    {
                        pairs.Add(c.Value, FindClasses(r.Value));
                        LineRenderes.Add(line.AddComponent<LineRenderer>(), pairs);
                    }
                }
            }

        }
        #endregion

        #region PRIVATE METHODS
        GameObject FindClasses(IXmlNode c)
        {
            GameObject g = new GameObject("FindClasses");

            foreach (KeyValuePair<Class, GameObject> gg in Classes)
            {
                print("C: " + c.Name + " gg.key: " + gg.Key.Name);
                if (c.Equals(gg.Key))
                {
                    print("EQUAL     C: " + c.Name + " gg.key: " + gg.Key.Name);
                    g = gg.Value;
                }
            }
            return g;
        }
        void UpdateLineRenderer()
        {
            foreach (KeyValuePair<LineRenderer, Dictionary<GameObject, GameObject>> l in LineRenderes)
            {
                foreach (KeyValuePair<GameObject, GameObject> g in l.Value)
                {
                    l.Key.SetPosition(0, g.Key.transform.FindChild("secondDivider").position);
                    l.Key.SetPosition(1, g.Value.transform.FindChild("secondDivider").position);
                    l.Key.SetWidth(.25f, .25f);
                    l.Key.material = lineMaterial;
                    l.Key.SetColors(Color.grey, Color.grey);
                }
            }
        }

        #endregion
    }
}