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
    class FeatureTypeRecord : CommonRecord
    {
        internal enum FTR_Type { FRID,FOID,ATTR,INAS,SPAS,FASC,THAS,MASK }

        internal FRID frid;
        internal FOID foid;
        internal List<ATTR> attr;
        internal List<INAS> inas;
        internal List<SPAS> spas;
        internal List<FASC> fasc;
        internal List<THAS> thas;
        internal List<MASK> mask;

        public FeatureTypeRecord()
        {
            frid = new FRID();
            foid = new FOID();
            attr = new List<ATTR>();
            inas = new List<INAS>();
            spas = new List<SPAS>();
            fasc = new List<FASC>();
            thas = new List<THAS>();
            mask = new List<MASK>();
        }
        internal class FRID
        {
            internal uint rcnm; //1byte
            internal uint rcid;
            internal ushort nftc;
            internal ushort rver;
            internal uint ruin; //1byte
        }
        internal class FOID
        {
            internal ushort agen;
            internal uint fidn;
            internal ushort fids;
        }
        internal class SPAS
        {
            internal uint rrnm; //1byte
            internal uint rrid;
            internal uint ornt;//1byte
            internal uint smin;
            internal uint smax;
            internal uint saui; //1byte
        }
        internal class FASC
        {
            internal uint rrnm;
            internal uint rrid;
            internal ushort nfac;
            internal ushort narc;
            internal uint faui;
            internal List<NATC> natc;

            public FASC()
            {
                natc = new List<NATC>();
            }
            internal class NATC
            {
                internal ushort natc;
                internal ushort atix;
                internal ushort paix;
                internal uint atin;
                internal string atvl;
            }
        }
        internal class THAS
        {
            internal uint rrnm;//1byte
            internal uint rrid;
            internal uint taui; //1byte
        }
        internal class MASK
        {
            internal uint rrnm;//1byte
            internal uint rrid;
            internal uint mind;//1byte
            internal uint muin; //1byte
        }
    }
}
