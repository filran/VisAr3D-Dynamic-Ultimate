///////////////////////////////////////////////////////////
//  SequenceDiagram.cs
//  Implementation of the Class SequenceDiagram
//  Generated by Enterprise Architect
//  Created on:      15-mar-2016 14:08:33
//  Original author: Filipe
///////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;



using ThreeDUMLAPI;
namespace ThreeDUMLAPI {
    public class SequenceDiagram : Diagram {

        public int CountLifelines { get; set; }
        public int CountMessages { get; set; }

        public SequenceDiagram(){
            //CountLifelines = SoftwareEntities.Count;
		}

		~SequenceDiagram(){

		}

	}//end SequenceDiagram

}//end namespace ThreeDUMLAPI