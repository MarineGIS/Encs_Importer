/*
Copyright [2015] [DSU_ITC]

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Encs_Importer.XML
{
    class Reader
    {
        string path;
        public enum MODE {FILE }

        XmlDocument xmlDoc;
        XmlNamespaceManager nsmgr;

        public Reader(string path)
        {
            this.path = path;
        }
        public void Read(MODE mode,string prefix, string nsUrl)
        {
            xmlDoc = new XmlDocument();
            if (mode == MODE.FILE)
            {
                xmlDoc.Load(path);
                nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
                nsmgr.AddNamespace(prefix,nsUrl);
            }   
        }
        public XmlNode GetNode(string nodePath)
        {   
            return xmlDoc.SelectSingleNode(nodePath,nsmgr);
        }
        public XmlNode GetNode(XmlNode node, string name)
        {
            return node.SelectSingleNode(name, nsmgr);
        }
        public XmlNodeList GetNodeList(XmlNode node,string nodeName)
        {
            return node.SelectNodes(nodeName,nsmgr);
        }
        
    }
}
