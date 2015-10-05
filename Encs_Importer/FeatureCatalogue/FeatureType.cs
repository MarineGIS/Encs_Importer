﻿/*
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

namespace Encs_Importer.FeatureCatalogue
{
    class FeatureType
    {
        string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        string definition;

        public string Definition
        {
            get { return definition; }
            set { definition = value; }
        }
        string code;

        public string Code
        {
            get { return code; }
            set { code = value; }
        }
        SpatialPrimitiveType prim;

        internal SpatialPrimitiveType Prim
        {
            get { return prim; }
            set { prim = value; }
        }
        public enum SpatialPrimitiveType{
            noGeometry, point, pointSet,curve,surface,coverage,arcByCenterPoint,circleByCenterPoint
        }

    }
}