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
    class ISO_8211_Directory
    {
        char[] tag;

        public char[] Tag
        {
            get { return tag; }
            set { tag = value; }
        }
        int length;

        public int Length
        {
            get { return length; }
            set { length = value; }
        }
        int position;

        public int Position
        {
            get { return position; }
            set { position = value; }
        }

        public ISO_8211_Directory()
        {
            tag = new char[4];
        }
        

    }
}
