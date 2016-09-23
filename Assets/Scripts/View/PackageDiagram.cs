//
// This class rendering ONE sequence diagram
//

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ThreeDUMLAPI;

namespace View
{
    public class PackageDiagram : MonoBehaviour
    {
        #region PRIVATE VARS
        #endregion

        #region PUBLIC VARS
        public GameObject PackageGO; //Prefab
        #endregion

        #region PUBLIC METHODS
        //This main method rendering the package diagram
        public Dictionary<string, GameObject> renderPackageDiagram(Dictionary<string, IXmlNode> packages) {
            Dictionary<string, GameObject> packageGOs = new Dictionary<string, GameObject>();
            int pos = 0; // Não encontrei a posição dos pacotes, então tive que criar...
            foreach (KeyValuePair<string, IXmlNode> pack in packages) {
                Package p = pack.Value as Package;
                //Debug.Log("Pacote " + p.Id + " -- Nome: " + p.Name);

                GameObject packGO = (GameObject) Instantiate(PackageGO);
                packGO.transform.parent = transform;
                packGO.transform.localPosition = new Vector3(pos, 0, 0);
                packGO.name = p.Name;
                packGO.GetComponentInChildren<TextMesh>().text = p.Name;
                pos += 2;
                packageGOs.Add(pack.Key, packGO);
            }
            return packageGOs;
        }
        #endregion

        #region PRIVATE METHODS
        #endregion
    }
}