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
        #endregion

        #region PUBLIC VARS
        public GameObject LifelineGO; //Lifeline Prefab
        public string XMIURL; //XMI file path 
        public GameObject PackageDiagramPrefab;
        #endregion

        #region UNITY METHODS
	    void Start () {
            //Load XMI file
            XMI = new ThreeDUML(XMIURL);

            addPackageDiagram(new Vector3(0, 2, 0));

            //Esta variável de controle evita de renderizar mais de uma vez o Diagrama de Sequencia... Verificar isso!!!!
            int loopSeq = 1;

            //Open packages
            foreach (KeyValuePair<string, IXmlNode> pair in XMI.Packages)
            {
                Package package = pair.Value as Package;
                //Render Sequence Diagram if exists
                if (package.SequenceDiagrams.Count > 0)
                {
                    foreach (ThreeDUMLAPI.SequenceDiagram s in package.SequenceDiagrams)
                    {
                        if (loopSeq == 1)
                        {
                            addSequenceDiagram(package);
                            loopSeq = 0;
                        }
                    }
                }
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
        }
        #endregion
    }
}