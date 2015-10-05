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
    class CommonRecord
    {
        internal class ATTR
        {
            internal ushort natc;
            internal ushort atix;
            internal ushort paix;
            internal uint atin; //1byte
            internal string atvl;
        }
        internal class INAS
        {
            internal uint rrnm;//1byte
            internal uint rrid;
            internal ushort niac;
            internal ushort narc;
            internal uint iuin; //1byte

            internal List<NATC> natc;

            internal struct NATC
            {
                internal ushort natc;
                internal ushort atix;
                internal ushort paix;
                internal uint atin;
                internal string atvl;
            }
        }

        #region Spatail
        internal class COCC
        {
            internal uint coui;//1byte
            internal ushort coix;
            internal ushort ncor;
        }
        internal class C2IT
        {
            internal Coordinate2D<int> coor;
            public C2IT()
            {
                coor = new Coordinate2D<int>();
            }
        }
        internal class C3IT
        {
            internal uint vcid; //1byte
            internal Coordinate3D<int> coor;
            public C3IT()
            {
                coor = new Coordinate3D<int>();
            }
        }
        internal class C2FT
        {
            internal Coordinate2D<double> coor;
            public C2FT()
            {
                coor = new Coordinate2D<double>();
            }
            
        }
        internal class C3FT
        {
            internal uint vcid; //1byte
            internal Coordinate3D<double> coor;
            public C3FT()
            {
                coor = new Coordinate3D<double>();
            }
        }
        internal class C2IL
        {
            internal List<Coordinate2D<int>> coor;
            public C2IL()
            {
                coor = new List<Coordinate2D<int>>();
            }
        }
        internal class C3IL
        {
            internal uint vcid; //1byte
            internal List<Coordinate3D<int>> coor;
            public C3IL()
            {
                coor = new List<Coordinate3D<int>>();
            }
        }
        internal class C2FL
        {
            internal List<Coordinate2D<double>> coor;
            public C2FL()
            {
                coor = new List<Coordinate2D<double>>();
            }
        }
        internal class C3FL
        {
            internal uint vcid; //1byte
            internal List<Coordinate3D<double>> coor;
            public C3FL()
            {
                coor = new List<Coordinate3D<double>>();
            }
        }




        //base coordinate
        internal class Coordinate2D<T>
        {
            internal T ycoo;
            internal T xcoo;
        }
        internal class Coordinate3D<T>:Coordinate2D<T>
        {
            internal T zcoo;
        }
         #endregion
    }
}
