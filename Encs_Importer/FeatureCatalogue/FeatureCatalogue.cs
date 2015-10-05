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

namespace Encs_Importer.FeatureCatalogue
{
    class FeatureCatalogue
    {
        List<FeatureType> featureTypeList;

        internal List<FeatureType> FeatureTypeList
        {
            get { return featureTypeList; }
            set { featureTypeList = value; }
        }
        string path;

        public FeatureCatalogue(string path)
        {
            this.path = path;
            featureTypeList = new List<FeatureType>();
        }

        public void Load_XML_FC()
        {
            XML.Reader reader = new XML.Reader(path);
            reader.Read(XML.Reader.MODE.FILE,"S100FC","http://www.iho.int/S100FC");

            XmlNode ftn = reader.GetNode(@"/S100FC:S100_FC_FeatureCatalogue/S100FC:S100_FC_FeatureTypes");
            XmlNodeList ftl = reader.GetNodeList(ftn, "S100FC:S100_FC_FeatureType");

            

            foreach (XmlNode node in ftl)
            {
                FeatureType featureType = new FeatureType();

                featureType.Name = reader.GetNode(node,"S100FC:name").InnerText;
                featureType.Definition = reader.GetNode(node, "S100FC:definition").InnerText;
                featureType.Code = reader.GetNode(node,"S100FC:code").InnerText;
                featureType.Prim = (FeatureType.SpatialPrimitiveType)Enum.Parse(typeof(FeatureType.SpatialPrimitiveType), reader.GetNode(node, "S100FC:permittedPrimitives").InnerText);
                FeatureTypeList.Add(featureType);
            }
        }
    }
}
