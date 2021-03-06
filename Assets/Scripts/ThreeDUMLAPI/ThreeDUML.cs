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
        public List<IXmlNode> Packages { get; private set; }

        public List<IXmlNode> AllDiagrams { get; private set; }

        public Dictionary<string, List<IXmlNode>> AllLinks { get; private set; }

		public ThreeDUML(string url){
            TheXMI = new XMI(@url);
            Packages = new List<IXmlNode>();

            AllDiagrams = TheXMI.Diagrams;

            AllLinks = TheXMI.Links;

            AddPackages();
            AddDiagramsToPackages();

            AddClassesToDiagrams();
            AddRelationshipsToClassDiagrams();

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
            foreach(Package p in TheXMI.Packages)
            {
                Packages.Add(p);
            }
        }

        private void AddDiagramsToPackages()
        {
            foreach(Package p in Packages)
            {
                foreach(Node d in TheXMI.Diagrams)
                {
                    if(p.Id == d.IdPackage)
                    {
                        if(d.Type == "Logical")
                        {
                            p.ClassDiagrams.Add(InterchangeSoftwareEntity(d,new ClassDiagram()));
                        }
                        else if (d.Type == "Sequence")
                        {
                            p.SequenceDiagrams.Add(InterchangeSoftwareEntity(d, new SequenceDiagram()));
                        }
                    }
                }
            }
        }

        private void AddLifelinesToDiagrams()
        {
            foreach(Package p in Packages)
            {
                foreach(SequenceDiagram d in p.SequenceDiagrams)
                {
                    foreach(IXmlNode e in d.ChildNodes)
                    {
                        foreach(IXmlNode l in TheXMI.Lifelines)
                        {
                            if(e.Subject == l.Id)
                            {
                                //Debug.Log("Lifeline: " + l.Id + " " + l.Name);
                                Lifeline thelifeline = new Lifeline();
                                d.SoftwareEntities.Add(InterchangeSoftwareEntity(l,thelifeline));
                            }
                        }
                    }
                }
            }
        }

        private void AddMessagesToLifeline()
        {
            foreach (Package p in Packages)
            {
                foreach (SequenceDiagram d in p.SequenceDiagrams)
                {
                    foreach (Lifeline l in d.SoftwareEntities)
                    {
                        d.CountLifelines++; //amount life
                        foreach (IXmlNode m in TheXMI.Messages)
                        {
                            if (m.IdSource == l.Id)
                            {
                                foreach(Lifeline ll in d.SoftwareEntities)
                                {
                                    if(m.IdTarget == ll.Id)
                                    {
                                        if(ll.Left > l.Left)
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

            foreach(Package p in Packages)
            {
                //Debug.Log("\tPackage p in Packages");

                foreach(ClassDiagram d in p.ClassDiagrams)
                {
                    //Debug.Log("\t\tClassDiagram d in p.ClassDiagrams - qtdChildren:" + d.ChildNodes.Count);

                    foreach(IXmlNode e in d.ChildNodes)
                    {
                        //Debug.Log("\t\t\tIXmlNode e in d.ChildNodes");

                        foreach(IXmlNode c in TheXMI.Classes)
                        {
                            //Debug.Log("\t\t\t\tIXmlNode c in TheXMI.Classes");

                            if(e.Subject == c.Id)
                            {
                                //Debug.Log("\t\t\t\t\te.Subject == c.Id");

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
        }

        private void AddRelationshipsToClassDiagrams()
        {
            string s = "";
            //string s = "AddRelationshipsToClassDiagrams\n";
            foreach(Package p in Packages)
            {
                //s += "\tPackage p in Packages\n";
                foreach(ClassDiagram d in p.ClassDiagrams)
                {
                    //s += "\t\tClassDiagram d in p.ClassDiagrams\n";
                    //s += "\t\t"+d.Name+" - "+d.Id+"\n";
                    foreach(Class c in d.SoftwareEntities)
                    {
                        foreach (IXmlNode link in c.Links)
                        {
                            //s += "c.Links: " + link.Tag + "\t" + link.Start + "\t" + link.End + "\n";
                        }

                        //s += "\t\t\tClass c in d.SoftwareEntities\n";
                        //s += "\t\t\t"+c.Name+" - "+c.Id+"\n";
                        //s += "Relationships\n";
                        foreach (Relationship r in TheXMI.Relationships)
                        {
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