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
    class MultiPointRecord :CommonRecord
    {
        internal enum MPR_Type { MRID,INAS,COCC,C2IL,C3IL,C2FL,C3FL }

        internal MRID mrid;
        internal List<INAS> inas;
        internal COCC cocc;
        internal List<C2IL> c2il;
        internal List<C3IL> c3il;
        internal List<C2FL> c2fl;
        internal List<C3FL> c3fl;

        public MultiPointRecord()
        {
            mrid = new MRID();
            inas = new List<INAS>();
            cocc = new COCC();
            c2il = new List<C2IL>();
            c3il = new List<C3IL>();
            c2fl = new List<C2FL>();
            c3fl = new List<C3FL>();
        }

        internal class MRID
        {
            internal uint rcnm; //1byte
            internal uint rcid;
            internal ushort rver;
            internal uint ruin; //1byte
        }
    }
}
