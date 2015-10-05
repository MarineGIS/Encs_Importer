using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encs_Importer.Common;

namespace Encs_Importer.ENC_Builder
{
    public class Model_S_101
    {
        internal DSGIR dsgir;
        internal DSCRSR dscrsr;
        internal List<IR> irList;
        internal List<PR> prList;
        internal List<MPR> mprList;
        internal List<CR> crList;
        internal List<CCR> ccrList;
        internal List<SR> srList;
        internal List<FTR> ftrList;
        
        public Model_S_101()
        {
            dsgir = new DSGIR();
            dscrsr = new DSCRSR();
            prList = new List<PR>();
            irList = new List<IR>();
            mprList = new List<MPR>();
            crList = new List<CR>();
            ccrList = new List<CCR>();
            srList = new List<SR>();
            ftrList = new List<FTR>();
        }
        internal class DSGIR //Data Set General Information record
        {
            internal Common_Record DSID;
            internal Common_Record DSSI;
            internal List<Common_Record> ATTR;
            public DSGIR()
            {
                ATTR = new List<Common_Record>();
            }
        }
        internal class DSCRSR //Data Set Coordinate Reference System record
        {
            internal Common_Record CSID;
            internal List<M_CRSH> crshs;

            internal class M_CRSH
            {
                internal Common_Record CRSH;
                internal Common_Record CSAX;
                internal Common_Record VDAT;
            }
            public DSCRSR()
            {
                crshs = new List<M_CRSH>();
            }
        }
        internal class IR //Information Record
        {
            internal Common_Record IRID;
            internal List<Common_Record> ATTR;
            internal List<Common_Record> INAS;
            public IR()
            {
                ATTR = new List<Common_Record>();
                INAS = new List<Common_Record>();
            }
            
        }
        internal class PR // Point Record
        {
            internal Common_Record PRID;
            internal List<Common_Record> INAS;
            internal List<Common_Record> C2IT;
            internal List<Common_Record> C3IT;
            public PR()
            {
                INAS = new List<Common_Record>();
                C2IT = new List<Common_Record>();
                C3IT = new List<Common_Record>();
            }
        }
        internal class MPR //Multi Point Record
        {
            internal Common_Record MRID;
            internal List<Common_Record> INAS;
            internal List<Common_Record> C2IL;
            internal List<Common_Record> C3IL;
            public MPR()
            {
                INAS = new List<Common_Record>();
                C2IL = new List<Common_Record>();
                C3IL = new List<Common_Record>();
            }
        }
        internal class CR //Curve Record
        {
            internal Common_Record CRID;
            internal List<Common_Record> INAS;
            internal Common_Record PTAS;
            internal M_SEGH SEGH;
            public CR()
            {
                INAS = new List<Common_Record>();
            }
            public class M_SEGH
            {
                internal Common_Record _SEGH;
                internal List<Common_Record> C2IL;
                public M_SEGH()
                {
                    C2IL = new List<Common_Record>();
                }
            }
        }
        internal class CCR //Composit curve Record
        {
            internal Common_Record CCID;
            internal List<Common_Record> INAS;
            internal List<Common_Record> CUCO;

            public CCR()
            {
                INAS = new List<Common_Record>();
                CUCO = new List<Common_Record>();
            }
        }
        internal class SR // Surface Record
        {
            internal Common_Record SRID;
            internal List<Common_Record> INAS;
            internal List<Common_Record> RIAS;
            public SR()
            {
                INAS = new List<Common_Record>();
                RIAS = new List<Common_Record>();
            }

        }
        internal class FTR // Feature Type Record
        {
            internal Common_Record FRID;
            internal Common_Record FOID;

            internal List<Common_Record> ATTR;
            internal List<Common_Record> INAS;
            internal List<Common_Record> SPAS;
            internal List<Common_Record> FASC;
            internal List<Common_Record> THAS;
            internal List<Common_Record> MASK;
          
            public FTR()
            {
                ATTR = new List<Common_Record>();
                INAS = new List<Common_Record>();
                SPAS = new List<Common_Record>();
                FASC = new List<Common_Record>();
                THAS = new List<Common_Record>();
                MASK = new List<Common_Record>();
            }
        }


    }
}
