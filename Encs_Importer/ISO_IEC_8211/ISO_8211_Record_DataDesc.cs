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

namespace Encs_Importer.ISO_IEC_8211
{
    public class ISO_8211_Record_DataDesc
    {
        internal char[] field_control;
        internal string field_name;
        internal string array_desc_str;
        internal List<string> array_descriptions;
        internal string format_control_str;
        internal List<FormatControl> format_controls;

        internal enum FORMAT_CONTROL { A, I, R, B, b1, b2 ,b4};

        public class FormatControl
        {
            internal FORMAT_CONTROL type;
            internal int length;
        }
    }
   
}
