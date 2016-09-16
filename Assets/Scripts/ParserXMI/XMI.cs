///////////////////////////////////////////////////////////
//  XMI.cs
//  Implementation of the Class XMI
//  Generated by Enterprise Architect
//  Created on:      07-mar-2016 12:02:20
//  Original author: hercules
///////////////////////////////////////////////////////////

using System;
using System.Globalization;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using UnityEngine;
using ThreeDUMLAPI;

namespace ParserXMI {
	public class XMI {

        private XmlDocument ParserXMI { get; set; }
        public List<IXmlNode> Packages { get; private set; }
        public List<IXmlNode> Diagrams { get; private set; }
        public List<IXmlNode> Classes { get; private set; }
        public List<IXmlNode> Relationships { get; private set; }
        public List<IXmlNode> Lifelines { get; private set; }
        public List<IXmlNode> Messages { get; private set; }

        //<XMI:Extension>
        //    <elements>
        //        <element>
        //            <links>
        //               XMI:id class       <links>
        public Dictionary<string, List<IXmlNode>> Links { get; set; }

        public XMI(string url)
        {
            ParserXMI = new XmlDocument();
            ParserXMI.Load(url);

            if(validationXMI())
            {
                Packages = new List<IXmlNode>();
                Diagrams = new List<IXmlNode>();
                Classes = new List<IXmlNode>();
                Relationships = new List<IXmlNode>();
                Lifelines = new List<IXmlNode>();
                Messages = new List<IXmlNode>();
                Links = new Dictionary<string, List<IXmlNode>>();

                ReadNodes(ParserXMI.DocumentElement);
                FilterPackages();

                //test
                foreach(IXmlNode m in Messages){
                    //Debug.Log("Name:" + m.Name + "\tLabel:" + m.Label + "\tId:" + m.Id + "\tSource:" + m.IdSource + "\tTarget:" + m.IdTarget);
                }
            }
        }

        private bool validationXMI()
        {
            XmlNode noderoot = this.ParserXMI.DocumentElement;
            if (noderoot.Name == "xmi:XMI")
            {
                //Console.WriteLine("It is XMI");
                return true;
            }
            else
            {
                //Console.WriteLine("It is not XMI");
                return false;
            }
        }

        private void ReadNodes(XmlNode node)
        {
            foreach(XmlNode n in node)
            {
                BuildPackage(n);

                BuildDiagram(n);

                //TO CLASS
                BuildClass(n);
                BuildClassElement(n);
                BuildRelationshipPackagedElement(n);
                BuildRelationshipLink(n);
                BuildRelationshipConnector(n);
                BuildLinks(n);

                //TO LIFELINE
                BuildLifeline(n);
                BuildLifelineElement(n);
                BuildMessages(n);
                BuildSourceAndTargetLifelineToMessage(n);

                ReadNodes(n);
            }
        }

        private void AddAttributes(XmlNode node , IXmlNode n)
        {
            n.Tag = node.Name;
            foreach(XmlNode att in node.Attributes)
            {
                switch(att.Name)
                {
                    case "xmi:type":
                        n.Type = att.Value;
                        break;

                    case "type":
                        n.Type = att.Value;
                        break;

                    case "xmi:id":
                        n.Id = att.Value;
                        break;

                    case "xmi:idref":
                        n.Id = att.Value;
                        break;

                    case "name":
                        n.Name = att.Value;
                        break;

                    case "represents":
                        n.Represents = att.Value;
                        break;

                    case "visibility":
                        n.Visibility = att.Value;
                        break;

                    case "isAbstract":
                        n.IsAbstract = att.Value;
                        break;

                    case "package":
                        n.IdPackage = att.Value;
                        break;

                    case "geometry":
                        n.Geometry = att.Value;
                        BreakAttribute(att.Value, n);
                        break;

                    case "subject":
                        n.Subject = att.Value;
                        break;

                    case "seqno":
                        n.Seqno = att.Value;
                        break;

                    case "style":
                        n.Style = att.Value;
                        break;

                    case "start":
                        n.Start = att.Value;
                        break;

                    case "end":
                        n.End = att.Value;
                        break;

                    case "messageKind":
                        n.MessageKind = att.Value;
                        break;

                    case "messageSort":
                        n.MessageSort = att.Value;
                        break;

                    case "sendEvent":
                        n.SendEvent = att.Value;
                        break;

                    case "receiveEvent":
                        n.ReceiveEvent = att.Value;
                        break;

                    case "mt":
                        n.Label = att.Value;
                        break;

                    case "sequence_points":
                        n.Sequence_Points = att.Value;
                        BreakAttribute(att.Value, n);
                        break;
                }
            }
        }

        private void BreakAttribute(string attr, IXmlNode n)
        {
            char[] char1 = { ';' };
            char[] char2 = { '=' };
            string[] a1 = attr.Split(char1);

            foreach (string g in a1)
            {
                //Debug.Log(g);
                string[] a2 = g.Split(char2);

                if (a2.Length > 1)
                {
                    //Debug.Log(a2[0] + " " + a2[1]);

                    switch (a2[0])
                    {
                        case "Left":
                            //Debug.Log("LLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLL");
                            n.Left = float.Parse(a2[1], CultureInfo.InvariantCulture.NumberFormat);
                            break;

                        case "Top":
                            n.Top = float.Parse(a2[1], CultureInfo.InvariantCulture.NumberFormat);
                            break;

                        case "Right":
                            n.Right = float.Parse(a2[1], CultureInfo.InvariantCulture.NumberFormat);
                            break;

                        case "Bottom":
                            n.Bottom = float.Parse(a2[1], CultureInfo.InvariantCulture.NumberFormat);
                            break;

                        case "PtStartX":
                            n.PtStartX = float.Parse(a2[1], CultureInfo.InvariantCulture.NumberFormat);
                            break;

                        case "PtStartY":
                            n.PtStartY = float.Parse(a2[1], CultureInfo.InvariantCulture.NumberFormat);
                            break;

                        case "PtEndX":
                            n.PtEndX = float.Parse(a2[1], CultureInfo.InvariantCulture.NumberFormat);
                            break;

                        case "PtEndY":
                            n.PtEndY = float.Parse(a2[1], CultureInfo.InvariantCulture.NumberFormat);
                            break;
                    }
                }
            }
        }


        private void BuildPackage(XmlNode node)
        {
            if (node.Name == "packagedElement" /*&& node.ParentNode.ParentNode.Name == "uml:Model"*/)
            {
                IXmlNode n = new Package();
                AddAttributes(node, n);
                Packages.Add(n);
            }
        } 
  
        private void BuildDiagram(XmlNode node)
        {
            if(node.Name == "diagram")
            {
                IXmlNode n = new Node();
                AddAttributes(node, n);

                foreach(XmlNode subnode in node.ChildNodes)
                {
                    switch(subnode.Name)
                    {
                        case "model":
                            AddAttributes(subnode, n);
                            break;

                        case "properties":
                            AddAttributes(subnode, n);
                            break;

                        case "elements":
                            foreach(XmlNode element in subnode.ChildNodes)
                            {
                                Node e = new Node();
                                AddAttributes(element, e);
                                n.ChildNodes.Add(e);
                            }
                            break;
                    }
                }
                Diagrams.Add(n);
            }
        }

        //remove os pacotes que n�o est�o associados aos diagramas
        private void FilterPackages()
        {
            List<IXmlNode> tempPack = Clone(Packages);
            Packages.Clear();

            foreach (IXmlNode d in Diagrams)
            {
                foreach( IXmlNode tp in tempPack )
                {
                    if(d.IdPackage == tp.Id)
                    {
                        Packages.Add(tp);
                    }
                }
            }
        }


        //TO CLASS
        private void BuildClass(XmlNode node)
        {
            if (node.Name == "packagedElement" && node.Attributes["xmi:type"].Value == "uml:Class" )
            {
                IXmlNode n = new Class();
                AddAttributes(node, n);
                foreach (XmlNode subnode in node.ChildNodes)
                {
                    switch (subnode.Name)
                    {
                        case "model":
                            AddAttributes(subnode, n);
                            break;

                        case "properties":
                            AddAttributes(subnode, n);
                            break;
                    }
                }
                Classes.Add(n);  
            }    
        }

        private void BuildClassElement(XmlNode node)
        {
            if (node.Name == "element" && node.ParentNode.Name == "elements" && node.ParentNode.ParentNode.Name == "xmi:Extension")
            {
                foreach(IXmlNode n in Classes)
                {
                    if(node.Attributes["xmi:idref"].Value == n.Id)
                    {
                        AddAttributes(node,n);
                        foreach(XmlNode subnode in node.ChildNodes)
                        {
                            switch (subnode.Name)
                            {
                                case "model":
                                    AddAttributes(subnode, n);
                                    break;

                                case "links":
                                    foreach(XmlNode link in subnode.ChildNodes)
                                    {
                                        IXmlNode l = new Relationship();
                                        l.Tag = link.Name;
                                        AddAttributes(link, l);
                                        n.ChildNodes.Add(l);
                                    }
                                    break;
                            }
                        }
                    }
                }
            }


            if(node.Name == "element" && node.ParentNode.Name == "elements" && node.ParentNode.ParentNode.Name == "diagram")
            {
                foreach (IXmlNode n in Classes)
                {
                    if(node.Attributes["subject"].Value == n.Id)
                    {
                        AddAttributes(node , n);
                    }
                }
            }

        }

        private void BuildRelationshipPackagedElement(XmlNode node)
        {
            if (node.Name == "packagedElement" )
            {
                if (node.Attributes["xmi:type"].Value == "uml:Realization" || node.Attributes["xmi:type"].Value == "uml:Association")
                {
                    IXmlNode n = new Relationship();
                    n.Tag = n.Name;
                    AddAttributes(node, n);
                    Relationships.Add(n);
                }  
            }
        }

        private void BuildRelationshipLink(XmlNode node)
        {
            if (node.Name == "links" && node.ParentNode.Name == "element")
            {
                foreach(XmlNode subnode in node)
                {
                    foreach(IXmlNode r in Relationships )
                    {
                        if (subnode.Attributes["xmi:id"].Value == r.Id)
                        {
                            AddAttributes(subnode,r);
                        }
                    }
                }
            }
        }

            private void AddGeneralizationToRelationships(XmlNode node)
        {
            if (node.Name == "connector")
            {
                foreach (XmlNode subnode in node.ChildNodes)
                {
                    if (subnode.Name == "properties")
                    {
                        if (subnode.Attributes["ea_type"].Value == "Generalization")
                        {
                            IXmlNode r = new Relationship();
                            r.Name = "Generalization";
                            AddAttributes(node, r);
                            AddAttributes(subnode, r);
                            Relationships.Add(r);
                        }
                    }
                }
            }
        }

        private void BuildRelationshipConnector(XmlNode node)
        {
            AddGeneralizationToRelationships(node);

            if (node.Name == "connector")
            {
                foreach(IXmlNode n in Relationships)
                {
                    if(node.Attributes["xmi:idref"].Value == n.Id)
                    {
                        foreach(XmlNode subnode in node.ChildNodes)
                        {
                            switch(subnode.Name)
                            {
                                case "source":
                                    n.IdSource = subnode.Attributes["xmi:idref"].Value;
                                    break;

                                case "target":
                                    n.IdTarget = subnode.Attributes["xmi:idref"].Value;
                                    n.Aggregation = subnode.ChildNodes.Item(2).Attributes["aggregation"].Value;
                                    break;

                                case "properties":
                                    n.EA_Type = subnode.Attributes["ea_type"].Value;
                                    break;
                            }
                        }
                    }
                }
            }
        }

        //tag <links>
        private void BuildLinks(XmlNode node)
        {
            if(node.Name == "links" && node.ParentNode.Name == "element" && node.ParentNode.ParentNode.Name == "elements" && node.ParentNode.ParentNode.ParentNode.Name == "xmi:Extension")
            {
                foreach (IXmlNode c in Classes)
                {
                    if (c.Id == node.ParentNode.Attributes["xmi:idref"].Value)
                    {
                        if (node.ChildNodes.Count > 0)
                        {
                            List<IXmlNode> links = new List<IXmlNode>();

                            foreach (XmlNode subnode in node.ChildNodes)
                            {
                                //Debug.Log("LINKS: "+subnode.Name + "\t" + subnode.Attributes["xmi:id"].Value + "\t" + subnode.Attributes["start"].Value + "\t" + subnode.Attributes["end"].Value);
                                Relationship l = new Relationship();
                                AddAttributes(subnode, l);
                                links.Add(l);
                            }
                            Links.Add(c.Id, links);
                        }
                    }
                }
            }
        }


        //TO LIFELINE
        private void BuildLifeline(XmlNode node)
        {
            if(node.Name == "lifeline")
            {
                IXmlNode n = new Lifeline();
                AddAttributes(node, n);
                Lifelines.Add(n);
            }
        }

        private void BuildLifelineElement(XmlNode node)
        {
            if (node.Name == "element" && node.ParentNode.Name == "elements" && node.ParentNode.ParentNode.Name == "diagram")
            {
                foreach(IXmlNode l in Lifelines)
                {
                    if(node.Attributes["subject"].Value == l.Id)
                    {
                        AddAttributes(node , l);
                    }
                }
            }
        }

        private void BuildMessages(XmlNode node)
        {
            if (node.Name == "message" && node.ParentNode.Name == "ownedBehavior")
            {
                IXmlNode n = new Method();
                AddAttributes(node, n);
                Messages.Add(n);
            }
        }

        private void BuildSourceAndTargetLifelineToMessage(XmlNode node)
        {
            if(node.Name == "connector")
            {
                foreach(IXmlNode m in Messages)
                {
                    if(node.Attributes["xmi:idref"].Value == m.Id)
                    {
                        foreach(XmlNode n in node.ChildNodes)
                        {
                            if (n.Name == "source" && n.ChildNodes[0].Attributes["type"].Value == "Sequence")
                            {
                                m.IdSource = n.Attributes["xmi:idref"].Value;
                                //Debug.Log("IdSource: " + m.IdSource);
                            }

                            if(n.Name == "target" && n.ChildNodes[0].Attributes["type"].Value == "Sequence")
                            {
                                m.IdTarget = n.Attributes["xmi:idref"].Value;
                                //Debug.Log("IdTarget: " + m.IdTarget);
                            }

                            if(n.Name == "labels")
                            {
                                AddAttributes(n,m);
                            }

                            if(n.Name == "extendedProperties")
                            {
                                AddAttributes(n,m);
                            }

                            if(n.Name == "appearance")
                            {
                                AddAttributes(n,m);
                            }
                        }
                    }
                }
            }
        }


        private List<IXmlNode> Clone(List<IXmlNode> c)
        {
            List<IXmlNode> clone = new List<IXmlNode>();
            foreach(IXmlNode cc in c)
            {
                clone.Add(cc);
            }
            return clone;
        }

		~XMI(){

		}

	}//end XMI

}//end namespace ParserXMI