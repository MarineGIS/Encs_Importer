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
    class CurveRecord : CommonRecord
    {
        internal enum CR_Type { CRID,INAS,PTAS,SECC,SEGH,COCC,C2IL,C3IL,C2FL,C3FL}

        internal CRID crid;
        internal List<INAS> inas;
        internal List<PTAS> ptas;
        internal SECC secc;
        internal List<SEGH> segh;

        public CurveRecord()
        {
            crid = new CRID();
            inas = new List<INAS>();
            ptas = new List<PTAS>();
            secc = new SECC();
            segh = new List<SEGH>();
        }

        internal class CRID
        {
            internal uint rcnm; //1byte
            internal uint rcid;
            internal ushort rver;
            internal uint ruin; //1byte
        }
        internal class PTAS
        {
            internal uint rrnm; //1byte
            internal uint rrid;
            internal uint topi; //1byte
        }
        internal class SECC
        {
            internal uint seui; //1byte
            internal ushort seix;
            internal ushort nseg;
        }
        internal class SEGH
        {
            internal uint intp; //1byte
            internal uint circ; //1byte
            internal Coordinate2D<double> coor;
            internal double dist;
            internal uint disu; //1byte
            internal double sbrg;
            internal double angl;

            internal COCC cocc;
            internal List<C2IL> c2il;
            internal List<C3IL> c3il;
            internal List<C2FL> c2fl;
            internal List<C3FL> c3fl;

            public SEGH()
            {
                coor = new Coordinate2D<double>();
                cocc = new COCC();
                c2il = new List<C2IL>();
                c3il = new List<C3IL>();
                c2fl = new List<C2FL>();
                c3fl = new List<C3FL>();
            }
        }
    }
}
