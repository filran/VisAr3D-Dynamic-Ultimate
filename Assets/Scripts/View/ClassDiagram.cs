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
        #endregion

        #region PUBLIC VARS
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
        public void renderClassDiagram(ThreeDUMLAPI.ClassDiagram classdiagram)
        {
            foreach(SoftwareEntity c in classdiagram.SoftwareEntities)
            {
                print(c.Name);
                if (c.Name == "FabricaSemanticos")
                    Debug.Log("ClassDiagram: " + c.Name + "  <--Nome\n" + c.ClassAttributesCount + "  <--N Att   N Op--> " + c.ClassMethodsCount + "\n\tID:" + c.Id);


            }
        }
        #endregion

        #region PRIVATE METHODS
        #endregion
    }
}