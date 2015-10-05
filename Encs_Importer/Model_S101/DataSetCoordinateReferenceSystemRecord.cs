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
    class DataSetCoordinateReferenceSystemRecord
    {
        internal enum DSCRSR_Type { CSID,CRSH,CSAX,PROJ,GDAT,VDAT}

        internal CSID csid;

        public DataSetCoordinateReferenceSystemRecord()
        {
            csid = new CSID();
        }
        internal class CSID 
        {
            internal uint rcnm; //1byte
            internal uint rcid;
            internal uint ncrc; //1byte

            internal List<CRSH> crsh;
            public CSID()
            {
                crsh = new List<CRSH>();
            }
        }
        internal class CRSH
        {
            internal uint crix; //1byte
            internal uint crst; //1byte
            internal uint csty; //1byte
            internal string crnm;
            internal string crsi;
            internal uint crss; //1byte
            internal string scri;

            internal List<CSAX> csax;
            internal PROJ proj;
            internal GDAT gdat;
            internal VDAT vdat;

            public CRSH()
            {
                csax = new List<CSAX>();
                proj = new PROJ();
                gdat = new GDAT();
                vdat = new VDAT();
            }
            internal class CSAX
            {
                internal uint axty; //1byte
                internal uint axum; //1byte
            }
            internal class PROJ
            {
                internal uint prom; //1byte
                internal double prp1;
                internal double prp2;
                internal double prp3;
                internal double prp4;
                internal double prp5;
                internal double feas;
                internal double fnor;
            }
            internal class GDAT
            {
                internal string dtnm;
                internal string elnm;
                internal double esma;
                internal uint espt; //1byte
                internal double espm;
                internal string cmnm;
                internal double cmgl;
            }
            internal class VDAT
            {
                internal string dtnm;
                internal string dtid;
                internal uint dtsr; //1byte
                internal string scri;
            }
        }
       
    }
}
