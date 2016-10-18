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

        #endregion

        #region PUBLIC VARS
        public GameObject ClassGO { get; set; } //Prefab

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

        //This main method rendering one class diagram
        public void renderClassDiagram(ThreeDUMLAPI.ClassDiagram classdiagram, Dictionary<string, IXmlNode> AllMessages)
        {
            foreach (Class classe in classdiagram.SoftwareEntities)
            {
                //print(classe.Name);
                //if (classe.Name == "FabricaSemanticos")
                //    Debug.Log("ClassDiagram: " + classe.Name + "  <--Nome\n" + classe.ClassAttributesCount + "  <--N Att   N Op--> " + classe.ClassMethodsCount + "\n\tID:" + c.Id);
                GameObject c = (GameObject)Instantiate(ClassGO, new Vector3(i, 0, 0), Quaternion.identity);
                string atributos = "", metodos = "";
                c.name = classe.Name;

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

                //Adicionar for para pegar texto
                if (classe.ClassAttributesCount > 0)
                {
                    foreach (Attribute at in classe.ClassAttributes)
                    {
                        //Debug.Log("AT ---> " + at.Name + "\n\n");
                        atributos += (at.Name + "\n\n");
                    }
                }
                Attributes_text.transform.position = new Vector3(AttributesBox.transform.position.x, AttributesBox.transform.position.y - 0.25f, AttributesBox.transform.position.z - 0.5f);
                Attributes_text.GetComponent<TextMesh>().text = atributos;

                SecDiv.transform.position = new Vector3(SecDiv.transform.position.x, AttributesBox.transform.position.y - (AttributesBox.localScale.y) * 0.5f, SecDiv.transform.position.z);

                MethodsBox.transform.localScale = new Vector3(MethodsBox.transform.localScale.x, (classe.ClassMethodsCount) * 0.6f, MethodsBox.transform.localScale.z);
                MethodsBox.transform.position = new Vector3(MethodsBox.transform.position.x, SecDiv.position.y - (MethodsBox.localScale.y) * 0.5f - SecDiv.localScale.y, MethodsBox.transform.position.z);

                //Adicionar for para pegar texto
                if (classe.ClassMethodsCount > 0)
                {
                    foreach (Method m in classe.ClassMethods)
                    {
                        //print("\t" + m.Name);

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
                }
                Methods_text.transform.position = new Vector3(MethodsBox.transform.position.x, MethodsBox.transform.position.y - 0.25f, MethodsBox.transform.position.z - 0.5f);
                Methods_text.GetComponent<TextMesh>().text = metodos;


                i += 7;

            }
        }
        #endregion

        #region PRIVATE METHODS
        #endregion
    }
}