using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace View
{
    public class MenuInteraction : MonoBehaviour
    {
        #region PRIVATE VARS
        private Visar3DDynamic visar3d;

        private GameObject control;
        private GameObject GoToClass;
        private GameObject GoToLifeline;
        private GameObject CameraDestination;
        private List<GameObject> ObjectsRelations = new List<GameObject>(); //os objetos de diferentes diagramas que estão relacionados
        private bool ApplyHighlight = false;
        private Vector3 PositionCamera;
        private float rate_lerp = 1f;
        private float speed_lerp = 0f;
        private float i_lerp = 0.05f;
        #endregion

        #region PUBLIC VARS
        public GameObject Camera;
        public GameObject Menu;
        public GameObject ClassGO;
        public GameObject LifelineGO;

        public Material BlackMaterial;
        public Material ClassMaterial;
        public Material LifelineMaterial;
        public Material LineRendererMaterial;

        public Dictionary<string, ThreeDUMLAPI.Class> ListClass = new Dictionary<string, ThreeDUMLAPI.Class>();
        public Dictionary<string, ThreeDUMLAPI.Lifeline> ListLifeline = new Dictionary<string, ThreeDUMLAPI.Lifeline>();

        public Dictionary<ThreeDUMLAPI.Class, GameObject> Classes = new Dictionary<ThreeDUMLAPI.Class, GameObject>();
        public Dictionary<ThreeDUMLAPI.Lifeline, GameObject> Lifelines = new Dictionary<ThreeDUMLAPI.Lifeline, GameObject>();

        //Relacionamentos entre classes e lifelines
        public Dictionary<GameObject, GameObject> Relationship = new Dictionary<GameObject, GameObject>();
        
        //Relacionamentos entre classes e lifelines, porém com LineRenderer
        public Dictionary<LineRenderer, Dictionary<GameObject, GameObject>> LineRenderes = new Dictionary<LineRenderer, Dictionary<GameObject, GameObject>>();

        public GameObject ObjectClicked { get; set; }
        public string IdObjectClicked { get; set; }
        public string TypeObjectClicked { get; set; }

        #endregion

        #region MESCLANDO E CONFIGURANDO Visar3ddynamic e MenuInteraction

        public void SetarClasses(Dictionary<ThreeDUMLAPI.Class, GameObject> classes)
        {
            foreach (KeyValuePair<ThreeDUMLAPI.Class, GameObject> c in classes)
            {
                c.Value.GetComponent<Click>().MenuInteraction = this.gameObject;
                c.Value.GetComponent<Click>().Id = c.Key.Id;
                c.Value.GetComponent<Click>().Type = "Class";

                ListClass.Add(c.Key.Id, c.Key);
            }
            Classes = classes;
        }
        
        public void SetarLifelines(Dictionary<ThreeDUMLAPI.Lifeline, GameObject> lifelines)
        {
            foreach (KeyValuePair<ThreeDUMLAPI.Lifeline, GameObject> l in lifelines)
            {
                l.Value.GetComponent<Click>().MenuInteraction = this.gameObject;
                l.Value.GetComponent<Click>().Id = l.Key.Id;
                l.Value.GetComponent<Click>().Type = "Lifeline";

                ListLifeline.Add(l.Key.Id, l.Key);
            }
            Lifelines = lifelines;
        }

        public void SetarRelationship(Dictionary<LineRenderer, Dictionary<GameObject, GameObject>> relationship)
        {
            foreach(KeyValuePair<LineRenderer, Dictionary<GameObject, GameObject>> r in relationship)
            {
                foreach (KeyValuePair<GameObject, GameObject> rr in r.Value)
                {
                    Relationship.Add(rr.Key , rr.Value);
                }
            }
            LineRenderes = relationship;
        }

        #endregion


        void Start()
        {
            ClassGO = this.GetComponent<Visar3DDynamic>().ClassGO;
            LifelineGO = this.GetComponent<Visar3DDynamic>().LifelineGO;
            LineRendererMaterial = this.GetComponent<Visar3DDynamic>().LineRendererMaterial;

            BotaoCloseDoMenu();
        }

        void Update()
        {
            VerificaSeAlgumObjetoFoiClicado();
            ViewPoint();
        }

        #region MÉTODOS EM STAR()

        void BotaoCloseDoMenu()
        {
            Transform close = Menu.transform.FindChild("BtClose");
            Button btclose = close.GetComponent<Button>();

            btclose.onClick.AddListener(delegate()
            {
                FecharMenu();
            });
        }

        void FecharMenu()
        {
            PositionCamera = new Vector3(Camera.transform.position.x, Camera.transform.position.y, Camera.transform.position.z);

            ResetarVariaveisDeControleDoClick();
            ResetarVisibilidadesDosBotoes();
            HabilitarColliders();

            Menu.SetActive(false);
        }

        void ResetarVariaveisDeControleDoClick()
        {
            this.ObjectClicked = null;
        }

        void ResetarVisibilidadesDosBotoes()
        {
            Menu.transform.FindChild("BtClass").GetComponent<Button>().interactable = true;
            Menu.transform.FindChild("BtLifeline").GetComponent<Button>().interactable = true;
            Menu.transform.FindChild("BtPackage").GetComponent<Button>().interactable = true;
        }

        void HabilitarColliders()
        {
            foreach (KeyValuePair<ThreeDUMLAPI.Class, GameObject> o in Classes)
            {
                o.Value.GetComponent<BoxCollider>().enabled = true;
            }

            foreach (KeyValuePair<ThreeDUMLAPI.Lifeline, GameObject> o in Lifelines)
            {
                o.Value.GetComponent<BoxCollider>().enabled = true;
            }
        }

        #endregion

        #region MÉTODOS EM UPDATE()
        
            #region MÉTODOS DENTRO DE VerificaSeAlgumObjetoFoiClicado()
            
            void VerificaSeAlgumObjetoFoiClicado()
            {
                if (ObjectClicked)
                {
                    AbrirMenu();
                }
            }

            void AbrirMenu()
            {
                Menu.SetActive(true);
                VisibilidadeDosBotoes();
            }

            void VisibilidadeDosBotoes()
            {
                //Configurar Class
                if (this.TypeObjectClicked.Equals("Class"))
                {
                    Button btclass = Menu.transform.FindChild("BtClass").GetComponent<Button>();
                    btclass.interactable = false;
                }

                //Configurar Lifeline
                if (this.TypeObjectClicked.Equals("Lifeline"))
                {
                    Button btlifeline = Menu.transform.FindChild("BtLifeline").GetComponent<Button>();
                    btlifeline.interactable = false;
                }
            }
            
            #endregion

            #region MÉTODOS DENTRO DE ViewPoint()

            void ViewPoint()
            {
                if (this.CameraDestination)
                {
                    //print("this.CameraDestination é " + this.CameraDestination.name);

                    float distance = 1f;
                    float dest_x = this.CameraDestination.transform.position.x;
                    float dest_y = this.CameraDestination.transform.position.y;
                    float dest_z = this.CameraDestination.transform.position.z;
                    float cam_z = PositionCamera.z;

                    dest_z = dest_z - distance;
                    speed_lerp = 1 / rate_lerp * Time.deltaTime;

                    if (Vector3.Distance(Camera.transform.position, new Vector3(dest_x, dest_y, dest_z)) > 0.1f)
                    {
                        Camera.transform.position = Vector3.Lerp(Camera.transform.position, new Vector3(dest_x, dest_y, dest_z), speed_lerp);
                        Camera.transform.rotation = Quaternion.Lerp(Camera.transform.rotation, this.CameraDestination.transform.rotation, speed_lerp);
                    }
                    else
                    {
                        this.CameraDestination = null;
                        ResetarGoToGameObject();
                        ResetarVariaveisDeControleDoClick();
                        print("Parou");
                    }
                }
            }

            void ResetarGoToGameObject()
            {
                GoToClass = null;
                GoToLifeline = null;

                print("ResetarGoToGameObject: GoToClass:" + GoToClass + "\tGoToLifeline:" + GoToLifeline);
            }

            #endregion

        #endregion

        #region MÉTODOS USANDOS EM Click

        public void DesabilitarColliders()
        {
            foreach (KeyValuePair<ThreeDUMLAPI.Class, GameObject> o in Classes)
            {
                o.Value.GetComponent<BoxCollider>().enabled = false;
            }

            foreach (KeyValuePair<ThreeDUMLAPI.Lifeline, GameObject> o in Lifelines)
            {
                o.Value.GetComponent<BoxCollider>().enabled = false;
            }
        }

        public void SetaGoToGameObject()
        {
            //Se foi uma classe que foi clicada...
            if (this.TypeObjectClicked.Equals("Class"))
            {
                //Setar o botao da lifeline
                if (Relationship.ContainsKey(this.ObjectClicked))
                {
                    GoToLifeline = Relationship[this.ObjectClicked];
                }
                //print("A classe " + this.ObjectClicked.name + " foi clicada e sua Lifeline é " + GoToLifeline.name);
            }

            //Se foi uma lifeline que foi clicada...
            if (this.TypeObjectClicked.Equals("Lifeline"))
            {
                //Setar o botao da classe
                foreach (KeyValuePair<GameObject, GameObject> r in Relationship)
                {
                    if (this.ObjectClicked.Equals(r.Value))
                    {
                        GoToClass = r.Key;
                    }
                }
                //print("A Lifeline " + this.ObjectClicked.name + " foi clicada  e sua classe é " + GoToClass.name);
            }
        }

        public void AddOnClickAosBotoesDoMenu()
        {
            Button btclass = Menu.transform.FindChild("BtClass").GetComponent<Button>();
            btclass.onClick.AddListener(delegate()
            {
                print("Go to class " + GoToClass.name);
                FecharMenu();
                this.CameraDestination = GoToClass;
            });

            Button btlifeline = Menu.transform.FindChild("BtLifeline").GetComponent<Button>();
            btlifeline.onClick.AddListener(delegate()
            {
                print("Go to lifeline " + GoToLifeline.name);
                FecharMenu();
                this.CameraDestination = GoToLifeline;
            });

            Button bthighlight = Menu.transform.FindChild("BtHighlight").GetComponent<Button>();
            bthighlight.onClick.AddListener(delegate()
            {
                //print("Highlight relatioships... ");
                MudarMaterialParaBlack();
                FecharMenu();
            });

            Button btdisablehighlight = Menu.transform.FindChild("BtDisableHighlight").GetComponent<Button>();
            btdisablehighlight.onClick.AddListener(delegate()
            {
                //print("Highlight relatioships... ");
                ResetarMaterials();
                FecharMenu();
            });

        }

        void MudarMaterialParaBlack()
        {
            ObjectsRelations.Clear();

            if (ListClass.ContainsKey(this.IdObjectClicked))
            {
                ThreeDUMLAPI.Class c = ListClass[this.IdObjectClicked];
                GameObject cGO = Classes[c];

                ObjectsRelations.Add(cGO); //Added class
                ObjectsRelations.Add(Relationship[cGO]); //Add lifeline

                ObjectsRelations[0].transform.FindChild("ClassBox").GetComponent<Renderer>().material = ClassMaterial;
                ObjectsRelations[0].transform.FindChild("MethodsBox").GetComponent<Renderer>().material = ClassMaterial;

                ObjectsRelations[1].transform.FindChild("Object").GetComponent<Renderer>().material = LifelineMaterial;
                ObjectsRelations[1].transform.FindChild("Line").GetComponent<Renderer>().material = LifelineMaterial;
            }

            if (ListLifeline.ContainsKey(this.IdObjectClicked))
            {
                ThreeDUMLAPI.Lifeline lifeline = ListLifeline[this.IdObjectClicked];

                if (Lifelines.ContainsKey(lifeline))
                {
                    GameObject lGO = Lifelines[lifeline];

                    foreach (KeyValuePair<GameObject, GameObject> r in Relationship)
                    {
                        if (r.Value.Equals(lGO))
                        {
                            ObjectsRelations.Add(r.Key);
                            ObjectsRelations.Add(r.Value);

                            ObjectsRelations[0].transform.FindChild("ClassBox").GetComponent<Renderer>().material = ClassMaterial;
                            ObjectsRelations[0].transform.FindChild("MethodsBox").GetComponent<Renderer>().material = ClassMaterial;

                            ObjectsRelations[1].transform.FindChild("Object").GetComponent<Renderer>().material = LifelineMaterial;
                            ObjectsRelations[1].transform.FindChild("Line").GetComponent<Renderer>().material = LifelineMaterial;
                        }
                    }
                }
            }

            //Comparando e aplicando BlackMaterial se não tiver no relacionamento
            foreach (GameObject o in ObjectsRelations)
            {
                foreach (KeyValuePair<LineRenderer, Dictionary<GameObject, GameObject>> line in LineRenderes)
                {
                    line.Key.GetComponent<Renderer>().material = LineRendererMaterial;

                    if (line.Value.ContainsKey(o) || line.Value.ContainsValue(o))
                    {

                    }
                    else
                    {
                        foreach (KeyValuePair<GameObject, GameObject> lineline in line.Value)
                        {
                            lineline.Key.transform.FindChild("ClassBox").GetComponent<Renderer>().material = BlackMaterial;
                            lineline.Key.transform.FindChild("MethodsBox").GetComponent<Renderer>().material = BlackMaterial;

                            lineline.Value.transform.FindChild("Object").GetComponent<Renderer>().material = BlackMaterial;
                            lineline.Value.transform.FindChild("Line").GetComponent<Renderer>().material = BlackMaterial;

                            line.Key.GetComponent<Renderer>().material = BlackMaterial;
                        }
                    }
                }

            }
        }

        void ResetarMaterials()
        {
            foreach (KeyValuePair<LineRenderer, Dictionary<GameObject, GameObject>> line in LineRenderes)
            {
                line.Key.GetComponent<Renderer>().material = LineRendererMaterial;
                foreach (KeyValuePair<GameObject, GameObject> go in line.Value)
                {
                    go.Key.transform.FindChild("ClassBox").GetComponent<Renderer>().material = ClassMaterial;
                    go.Key.transform.FindChild("MethodsBox").GetComponent<Renderer>().material = ClassMaterial;

                    go.Value.transform.FindChild("Object").GetComponent<Renderer>().material = LifelineMaterial;
                    go.Value.transform.FindChild("Line").GetComponent<Renderer>().material = LifelineMaterial;
                }
            }
        }

        #endregion
    
    }
}