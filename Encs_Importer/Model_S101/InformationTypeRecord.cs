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
    class InformationTypeRecord : CommonRecord
    {
        internal enum ITR_Tyte{ IRID,ATTR,INAS}

        internal IRID irid;

        public InformationTypeRecord()
        {
            irid = new IRID();
        }
        internal class IRID
        {
            internal uint rcnm; //1byte
            internal uint rcid;
            internal ushort nitc;
            internal ushort rver;
            internal uint ruin; //1byte

            internal List<ATTR> attr;
            internal List<INAS> inas;

            public IRID()
            {
                attr = new List<ATTR>();
                inas = new List<INAS>();
            }
        }

    }
}
