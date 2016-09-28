///////////////////////////////////////////////////////////
//  TheCore.cs
//  Implementation of the Class TheCore
//  Generated by Enterprise Architect
//  Created on:      15-mar-2016 08:28:14
//  Original author: Filipe
///////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ParserXMI;
using UnityEngine;

namespace ThreeDUMLAPI {
	public class ThreeDUML : IXmlNode {

        public XMI TheXMI;
        public Dictionary<string, IXmlNode> Packages { get; private set; }

        public Dictionary<string, IXmlNode> AllDiagrams { get; private set; }

        public Dictionary<string, List<IXmlNode>> AllLinks { get; private set; }

		public ThreeDUML(string url){
            TheXMI = new XMI(@url);
            //Packages = new Dictionary<string, IXmlNode>();
            Packages = TheXMI.Packages;

            AllDiagrams = TheXMI.Diagrams;

            AllLinks = TheXMI.Links;

            //Packages();
            AddDiagramsToPackages();

            //Class Diagram
            AddClassesToDiagrams();
            AddRelationshipsToClassDiagrams();

            //Sequence Diagram
            AddLifelinesToDiagrams();
            AddMessagesToLifeline();

            //TEST
            string _return_ = "\nALL LINKS\n\n";
            foreach (KeyValuePair<string, List<IXmlNode>> l in AllLinks)
            {
                _return_ += l.Key+"\n";
                foreach(IXmlNode ll in l.Value)
                {
                    _return_ += "\t"+ ll.Tag+"\t"+ll.Id+"\t"+ll.Start+"\t"+ll.End+"\n";
                }
            }
            Debug.Log(_return_);
        }

        //TROCA DE VALROES ENTRE AS CLASSES
        public IXmlNode InterchangeSoftwareEntity(IXmlNode old, IXmlNode entity)
        {
            entity.ChildNodes = old.ChildNodes;
            entity.EA_Type = old.EA_Type;
            entity.Aggregation = old.Aggregation;
            entity.End = old.End;
            entity.Geometry = old.Geometry;
            entity.Left = old.Left;
            entity.Top = old.Top;
            entity.Right = old.Right;
            entity.Bottom = old.Bottom;
            entity.Id = old.Id;
            entity.IdPackage = old.IdPackage;
            entity.IdSource = old.IdSource;
            entity.IdTarget = old.IdTarget;
            entity.IsAbstract = old.IsAbstract;
            entity.Name = old.Name;
            entity.Seqno = old.Seqno;
            entity.Start = old.Start;
            entity.Style = old.Style;
            entity.Subject = old.Subject;
            entity.Tag = old.Tag;
            entity.Type = old.Type;
            entity.Visibility = old.Visibility;
            entity.Represents = old.Represents;
            entity.MessageKind = old.MessageKind;
            entity.MessageSort = old.MessageSort;
            entity.SendEvent = old.SendEvent;
            entity.ReceiveEvent = old.ReceiveEvent;
            entity.Label = old.Label;
            entity.PtStartX = old.PtStartX;
            entity.PtStartY = old.PtStartY;
            entity.PtEndX = old.PtEndX;
            entity.PtEndY = old.PtEndY;
            entity.Dist = old.Dist;
            entity.Direction = old.Direction;

            return entity;
        }

        private void AddPackages()
        {
            Packages = TheXMI.Packages;
            //foreach(Package p in TheXMI.Packages)
            //{
            //    if (!Packages.ContainsKey(p.Id))
            //        Packages.Add(p.Id ,p);
            //}
        }

        private void AddDiagramsToPackages()
        {
            foreach(KeyValuePair<string, IXmlNode> dig in TheXMI.Diagrams)
            {
                IXmlNode d = dig.Value;
                Package p = (Package) Packages[d.IdPackage];
                if(d.Type == "Logical") {
                    p.ClassDiagrams.Add(InterchangeSoftwareEntity(d,new ClassDiagram()));
                } else if (d.Type == "Sequence") {
                    p.SequenceDiagrams.Add(InterchangeSoftwareEntity(d, new SequenceDiagram()));
                }
            }
        }

        private void AddLifelinesToDiagrams()
        {
            foreach (KeyValuePair<string, IXmlNode> pack in Packages)
            {
                foreach (SequenceDiagram d in ((Package)pack.Value).SequenceDiagrams)
                {
                    foreach(IXmlNode e in d.ChildNodes)
                    {
                        if (TheXMI.Lifelines.ContainsKey(e.Subject))
                        {
                            IXmlNode l = TheXMI.Lifelines[e.Subject];
                            //Debug.Log("Lifeline: " + l.Id + " " + l.Name);
                            Lifeline thelifeline = new Lifeline();
                            d.SoftwareEntities.Add(InterchangeSoftwareEntity(l, thelifeline));
                        }
                    }
                }
            }
        }

        private void AddMessagesToLifeline()
        {
            foreach (KeyValuePair<string, IXmlNode> p in Packages)
            {
                foreach (SequenceDiagram d in ((Package) p.Value).SequenceDiagrams)
                {
                    foreach (Lifeline l in d.SoftwareEntities)
                    {
                        d.CountLifelines++; //amount life
                        foreach (KeyValuePair<string, IXmlNode> mes in TheXMI.Messages)
                        {
                            IXmlNode m = mes.Value;
                            if (m.IdSource == l.Id)
                            {
                                foreach (Lifeline ll in d.SoftwareEntities)
                                {
                                    if (m.IdTarget == ll.Id)
                                    {
                                        if (ll.Left > l.Left)
                                        {
                                            m.Dist = ll.Left - l.Left;
                                            m.Direction = "right";
                                        }
                                        else
                                        {
                                            m.Dist = l.Left - ll.Left;
                                            m.Direction = "left";
                                        }
                                    }
                                }

                                Method method = new Method();
                                method.Left = l.Left;
                                l.AddMethod(InterchangeSoftwareEntity(m, method));
                                d.CountMessages++; //amount messages
                            }
                        }
                    }
                }
            }
        }

        private void AddClassesToDiagrams()
        {
            //Debug.Log("AddClassesToDiagrams");

            foreach (KeyValuePair<string, IXmlNode> pair in Packages)
            {
                //Debug.Log("\tPackage p in Packages");

                foreach (ClassDiagram d in ((Package) pair.Value).ClassDiagrams)
                {
                    //Debug.Log("\t\tClassDiagram d in p.ClassDiagrams - qtdChildren:" + d.ChildNodes.Count);

                    foreach(IXmlNode e in d.ChildNodes)
                    {
                        //Debug.Log("\t\t\tIXmlNode e in d.ChildNodes");
                        if (TheXMI.Classes.ContainsKey(e.Subject)) {
                            IXmlNode c = TheXMI.Classes[e.Subject];
                            Class theclass = new Class();
                            theclass.Geometry = e.Geometry;
                            theclass.Subject = e.Subject;
                            theclass.Seqno = e.Seqno;
                            theclass.Style = e.Style;

                            d.SoftwareEntities.Add(InterchangeSoftwareEntity(c, theclass));
                        }
                    }
                }
            }
        }

        private void AddRelationshipsToClassDiagrams()
        {
            string s = "";
            //string s = "AddRelationshipsToClassDiagrams\n";
            foreach (KeyValuePair<string, IXmlNode> pack in Packages)
            {
                //s += "\tPackage p in Packages\n";
                foreach (ClassDiagram d in ((Package) pack.Value).ClassDiagrams)
                {
                    //s += "\t\tClassDiagram d in p.ClassDiagrams\n";
                    //s += "\t\t"+d.Name+" - "+d.Id+"\n";
                    foreach(Class c in d.SoftwareEntities)
                    {
                        //foreach (IXmlNode link in c.Links)
                        //{
                        //    s += "c.Links: " + link.Tag + "\t" + link.Start + "\t" + link.End + "\n";
                        //}

                        //s += "\t\t\tClass c in d.SoftwareEntities\n";
                        //s += "\t\t\t"+c.Name+" - "+c.Id+"\n";
                        //s += "Relationships\n";
                        foreach (KeyValuePair<string, IXmlNode> rel in TheXMI.Relationships)
                        {
                            Relationship r = (Relationship) rel.Value;
                            //s += r.Tag+"\t"+r.Name+"\t"+r.Start+"\t"+r.End+"\n";

                            if(c.Id == r.IdSource)
                            {
                                //s += "\t\t\t\t"+r.EA_Type+" - "+r.Aggregation+" - "+r.Id+"\n";
                                //s += "\t\t\t\t\tTarget: " + r.IdTarget + " - " + r.FindById(d.SoftwareEntities, r.IdTarget).Name + "\n\n";

                                switch(r.EA_Type)
                                {
                                    case "Aggregation":
                                        if(r.Aggregation == "shared") //agregacao
                                        {
                                            Relationship rr = new Aggregation();
                                            c.AddRelationshipWith(InterchangeSoftwareEntity(r, rr), r.FindById(d.SoftwareEntities, r.IdTarget));
                                        }
                                        else if(r.Aggregation == "composite") //composicao
                                        {
                                            Relationship rr = new Composition();
                                            c.AddRelationshipWith(InterchangeSoftwareEntity(r, rr), r.FindById(d.SoftwareEntities, r.IdTarget));
                                        }
                                        break;

                                    case "Association":
                                            Relationship rrr = new Association();
                                            c.AddRelationshipWith(InterchangeSoftwareEntity(r, rrr), r.FindById(d.SoftwareEntities, r.IdTarget));
                                        break;

                                    case "Generalization":
                                            Relationship rrrr = new Generalization();
                                            c.AddRelationshipWith(InterchangeSoftwareEntity(r, rrrr), r.FindById(d.SoftwareEntities, r.IdTarget));
                                        break;
                                }
                            }
                        }
                    }
                }
            }

            //foreach(Relationship r in TheXMI.Relationships)
            //{
            //    Debug.Log(r.Name + " | " + r.Id + " | " + r.EA_Type + " | " + r.Aggregation + " | source: " + r.IdSource + " | target: " + r.IdTarget);
            //}

            Debug.Log(s);
        }

        ~ThreeDUML()
        {

		}

    }//end ThreeDUML

}//end namespace ThreeDUMLAPI