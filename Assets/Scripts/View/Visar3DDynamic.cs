//
// Main Project Class
//

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ThreeDUMLAPI;
using System;

namespace View {
    public class Visar3DDynamic : MonoBehaviour
    {
        #region PRIVATE VARS
        private ThreeDUML XMI;
        private string SequecenDiagramName;
        private Dictionary<ThreeDUMLAPI.Class, GameObject> Classes = new Dictionary<Class, GameObject>();
        private Dictionary<ThreeDUMLAPI.Lifeline, GameObject> Lifelines = new Dictionary<Lifeline, GameObject>();
        private GameObject LineRendererParent = new GameObject("LineRendererParent"); //Create LineRenderer Parent
        private Dictionary<string, Dictionary<ThreeDUMLAPI.Package, GameObject>> Packages = new Dictionary<string, Dictionary<Package, GameObject>>();
                                                //origin:Class    destination:Lifeline
        private Dictionary<LineRenderer, Dictionary<GameObject, GameObject>> LineRenderes;
        private Dictionary<LineRenderer, Dictionary<GameObject, GameObject>> LineRendererPackages;
        private int CountPairs = 0;
        #endregion

        #region PUBLIC VARS
        public GameObject LifelineGO; //Lifeline Prefab        
        public GameObject ClassGO; //Class Prefab
        public string XMIURL; //XMI file path 
        public GameObject PackageDiagramPrefab;
        #endregion

        #region UNITY METHODS
	    void Start () {
            //Load XMI file
            XMI = new ThreeDUML(XMIURL);
            LineRenderes = new Dictionary<LineRenderer, Dictionary<GameObject, GameObject>>();
            LineRendererPackages = new Dictionary<LineRenderer, Dictionary<GameObject, GameObject>>();

            addPackageDiagram(new Vector3(0, 2, 0));

            //Esta variável de controle evita de renderizar mais de uma vez o Diagrama de Sequencia... Verificar isso!!!!
            int loopSeq = 1;

            //Open packages (to improve this routine because it is repeating below for class diagram)
            foreach (KeyValuePair<string, IXmlNode> pair in XMI.Packages)
            {
                Package package = pair.Value as Package;

                //Render Sequence Diagram if exists and its Class Diagram
                if (package.SequenceDiagrams.Count > 0)
                {
                    foreach (ThreeDUMLAPI.SequenceDiagram s in package.SequenceDiagrams)
                    {
                        if (loopSeq == 1)
                        {
                            SequecenDiagramName = s.Name;
                            addSequenceDiagram(package);
                            loopSeq = 0;
                        }
                    }
                }
            }

            //Open packages (improve me!!!)
            foreach (KeyValuePair<string, IXmlNode> pair in XMI.Packages)
            {
                Package package = pair.Value as Package;

                //Render Class Diagram if exists
                if(package.ClassDiagrams.Count > 0)
                {
                    foreach (ThreeDUMLAPI.ClassDiagram c in package.ClassDiagrams)
                    {
                        //if class diagram's name is equal to seq diagram's name
                        if(c.Name.Equals(SequecenDiagramName))
                        {
                            addClassDiagram(c);
                        }
                    }
                }
            }           

            RelationshipBetweenDiagrams();

            //TESTS -------------------------------------------------------------
            //foreach(KeyValuePair<string,IXmlNode> p in XMI.Packages)
            //{
            //    print("Pacote: "+p.Value.Name);
            //}
            
            //foreach(KeyValuePair<ThreeDUMLAPI.Class, GameObject> c in Classes)
            //{
            //    if(c.Key.Name.Equals("FabricaSemanticos"))
            //    {
            //        print(c.Key.Name + "its package is " + c.Key.IdPackage);
            //    }
            //}

        }

        void Update()
        {
            if(CountPairs.Equals(LineRenderes.Keys.Count))
            {
                UpdateLineRenderer();
            }
        }
        #endregion

        #region PRIVATE METHODS
        //Render Package Diagram
        private void addPackageDiagram()
        {
            GameObject packageDiagramGO = (GameObject)Instantiate(PackageDiagramPrefab, Vector3.zero, Quaternion.identity);
            packageDiagramGO.GetComponent<PackageDiagram>().renderPackageDiagram(XMI.Packages);
        }
        private void addPackageDiagram(Vector3 position)
        {
            GameObject packageDiagramGO = (GameObject)Instantiate(PackageDiagramPrefab, position, Quaternion.identity);
            packageDiagramGO.GetComponent<PackageDiagram>().renderPackageDiagram(XMI.Packages);
            
            Packages = packageDiagramGO.GetComponent<PackageDiagram>().Packages;
        }

        //Render Sequence Diagram
        private void addSequenceDiagram(Package package)
        {
            //ONE sequence diagram added
            SequenceDiagram SeqDiagComp = this.gameObject.AddComponent<SequenceDiagram>();

            //Add prefab
            SeqDiagComp.LifelineGO = LifelineGO;

            //Render
            SeqDiagComp.renderSequenceDiagram(package);

            //Save the lifelines
            Lifelines = SeqDiagComp.Lifelines;
        }

        //Render Class Diagram
        private void addClassDiagram(ThreeDUMLAPI.ClassDiagram classdiagram)
        {
            //ONE class diagram added
            ClassDiagram ClassDiagComp = this.gameObject.AddComponent<ClassDiagram>();

            ClassDiagComp.ClassGO = ClassGO;

            //Render
            ClassDiagComp.renderClassDiagram(classdiagram, XMI.AllMessagesSignatures);

            //Save the classes
            Classes = ClassDiagComp.Classes;
        }

        //RelationshipBetweenDiagrams (Linerenderes)
        private void RelationshipBetweenDiagrams()
        {
            string equals = "";
            int count = 0;

            foreach (KeyValuePair<Class, GameObject> c in Classes)
            {
                foreach (KeyValuePair<ThreeDUMLAPI.Lifeline, GameObject> l in Lifelines)
                {
                    if (c.Key.Name.Equals(l.Key.Name)) //Is it equal?
                    {
                        equals += c.Key.Name + " class = " + l.Key.Name + " lifeline"+"\n";
                        count++;

                        GameObject line = new GameObject("line_"+c.Key.Name);
                        LineRenderer lineRenderer = line.AddComponent<LineRenderer>();
                        lineRenderer.SetPosition(0, c.Value.transform.FindChild("firstDivider").position);
                        lineRenderer.SetPosition(1, l.Value.transform.position);
                        lineRenderer.SetWidth(.25f, .25f);

                        Dictionary<GameObject, GameObject> pairs = new Dictionary<GameObject, GameObject>();
                        pairs.Add(c.Value, l.Value);
                        LineRenderes.Add(lineRenderer, pairs);
                    }
                }

                //Link between classes and packages
                if (Packages.ContainsKey(c.Key.IdPackage))
                {
                    //Find the package by id
                    Dictionary<ThreeDUMLAPI.Package, GameObject> package = Packages[c.Key.IdPackage];

                    foreach(KeyValuePair<ThreeDUMLAPI.Package, GameObject> p in package)
                    {
                        //print("FabricaSemanticos está no pacote "+ p.Key.Name +" com id " + p.Key.Id );
                        //Create LineRenderer
                        GameObject line = new GameObject("line_package");
                        LineRenderer lineRenderer = line.AddComponent<LineRenderer>();
                        lineRenderer.SetPosition(0, c.Value.transform.FindChild("firstDivider").transform.position);
                        lineRenderer.SetPosition(1 , p.Value.transform.position);
                        lineRenderer.SetWidth(.25f, .25f);

                        Dictionary<GameObject, GameObject> pairs = new Dictionary<GameObject, GameObject>();
                        pairs.Add(c.Value, p.Value);
                        LineRendererPackages.Add(lineRenderer, pairs);
                    }
                }
            }
            CountPairs = count;
            //print("EQUALS\n"+equals);
        }

        private void UpdateLineRenderer()
        {
            if (LineRenderes.Keys.Count > 0 )
            {
                //Classes and Lifelines
                foreach (KeyValuePair<LineRenderer, Dictionary<GameObject, GameObject>> l in LineRenderes)
                {
                    foreach (KeyValuePair<GameObject, GameObject> g in l.Value)
                    {
                        l.Key.SetPosition(0, g.Key.transform.FindChild("firstDivider").position);
                        l.Key.SetPosition(1, g.Value.transform.position);
                        l.Key.SetWidth(.25f, .25f);
                    }
                }

                //Classes and Packages
                foreach (KeyValuePair<LineRenderer, Dictionary<GameObject, GameObject>> lp in LineRendererPackages)
                {
                    foreach (KeyValuePair<GameObject, GameObject> gg in lp.Value)
                    {
                        lp.Key.SetPosition(0, gg.Key.transform.FindChild("firstDivider").position); //class
                        lp.Key.SetPosition(1, gg.Value.transform.position); //package
                        lp.Key.SetWidth(.25f, .25f);
                    }
                }
            }
        }

        #endregion
    }
}