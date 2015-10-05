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

namespace Encs_Importer.Common
{
    public class Common_SubRecord
    {
        public string tag { get; set; }
        public ValueType value { get; set; }
        public class ValueType
        {
            public int intValue { get; set; }
            public uint uintValue { get; set; }
            public string stringValue { get; set; }
            public double doubleValue { get; set; }

            public System.Type type { get; set; }


            public ValueType(Type type)
            {
                this.type = type;
            }

            public void SetValue(int value)
            {
                intValue = value;
            }
            public void SetValue(uint value)
            {
                uintValue = value;
            }
            public void SetValue(string value)
            {
                stringValue = value;
            }
            public void SetValue(double value)
            {
                doubleValue = value;
            }
        }
    }
    
}
