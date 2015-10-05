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

namespace Encs_Importer.Model_S101
{
    class PointRecord : CommonRecord
    {
        internal enum PR_Type { PRID,INAS,C2IT,C3IT,C2FT,C3FT}

        internal PRID prid;
        internal List<INAS> inas;
        internal List<C2IT> c2it;
        internal List<C3IT> c3it;
        internal List<C2FT> c2ft;
        internal List<C3FT> c3ft;

        public PointRecord()
        {
            prid = new PRID();
            inas = new List<INAS>();
            c2it = new List<C2IT>();
            c3it = new List<C3IT>();
            c2ft = new List<C2FT>();
            c3ft = new List<C3FT>();
        }
        internal class PRID
        {
            internal uint rcnm; //1byte
            internal uint rcid;
            internal ushort rver;
            internal uint ruin; //1byte
        }
    }
}
