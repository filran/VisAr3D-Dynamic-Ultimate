//
// Main Project Class
//

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ThreeDUMLAPI;

namespace View {
    public class Visar3DDynamic : MonoBehaviour
    {
        #region PRIVATE VARS
        private ThreeDUML XMI;
        #endregion

        #region PUBLIC VARS
        public GameObject LifelineGO; //Lifeline Prefab
        public string XMIURL; //XMI file path 
        #endregion

        #region UNITY METHODS
        // Use this for initialization
	    void Start () {
            //Load XMI file
            ThreeDUML XMI = new ThreeDUML(XMIURL);

            //Esta variável de controle evita de renderizar mais de uma vez o Diagrama de Sequencia... Verificar isso!!!!
            int loopSeq = 1;

            //Open packages
            foreach(Package package in XMI.Packages)
            {
                //Render Sequence Diagram if exists
                if (package.SequenceDiagrams.Count > 0)
                {
                    foreach(ThreeDUMLAPI.SequenceDiagram s in package.SequenceDiagrams)
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
	
	    // Update is called once per frame
	    void Update () {

        }
        #endregion

        #region PRIVATE METHODS
        
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