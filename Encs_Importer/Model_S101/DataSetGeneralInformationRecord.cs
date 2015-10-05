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
    class DataSetGeneralInformationRecord : CommonRecord
    {
        internal enum DSGIR_Type { DSID, DSSI, ATCS, ITCS, FTCS, IACS, FACS, ARCS, ATTR }
        internal DSID dsid;

        public DataSetGeneralInformationRecord()
        {
            dsid = new DSID();
        }
        internal class DSID
        {
            internal uint rcnm; //1byte
            internal uint rcid;
            internal string ensp;
            internal string ened;
            internal string prsp;
            internal string pred;
            internal string prof;
            internal string dsnm;
            internal string dstl;
            internal string dsrd; // 8
            internal string dslg;
            internal string dsab;
            internal string dsed;
            internal List<uint> dstc;

            internal DSSI dssi;
            internal Dictionary<ushort, string> ATCS;
            internal Dictionary<ushort, string> ITCS;
            internal Dictionary<ushort, string> FTCS;
            internal Dictionary<ushort, string> IACS;
            internal Dictionary<ushort, string> FACS;
            internal Dictionary<ushort, string> ARCS;
            internal List<ATTR> ATTR;

            public DSID()
            {
                dstc = new List<uint>();
                dssi = new DSSI();
                ATCS = new Dictionary<ushort, string>();
                ITCS = new Dictionary<ushort, string>();
                FTCS = new Dictionary<ushort, string>();
                IACS = new Dictionary<ushort, string>();
                FACS = new Dictionary<ushort, string>();
                ARCS = new Dictionary<ushort, string>();
                ATTR = new List<ATTR>();
            }
            internal class DSSI
            {
                internal double dcox;
                internal double dcoy;
                internal double dcoz;
                internal uint cmfx;
                internal uint cmfy;
                internal uint cmfz;
                internal uint noir;
                internal uint nopn;
                internal uint nomn;
                internal uint nocn;
                internal uint noxn;
                internal uint nosn;
                internal uint nofr;
            }
        }
       
    }
}
