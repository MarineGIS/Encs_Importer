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
    class S101
    {
        internal const int FeatureType = 100;
        internal const int Point = 110;
        internal const int MultiPoint = 115;
        internal const int Curve = 120;
        internal const int CompositeCurve = 125;
        internal const int Surface = 130;
        internal const int InformationType = 150;


        internal DataSetGeneralInformationRecord dsgir;
        internal DataSetCoordinateReferenceSystemRecord dscrsr;
        internal List<InformationTypeRecord> itr;
        internal List<PointRecord> pr;
        internal List<MultiPointRecord> mpr;
        internal List<CurveRecord> cr;
        internal List<CompositeCurveRecord> ccr;
        internal List<SurfaceRecord> sr;
        internal List<FeatureTypeRecord> ftr;

        public S101()
        {
            dsgir = new DataSetGeneralInformationRecord();
            dscrsr = new DataSetCoordinateReferenceSystemRecord();
            itr = new List<InformationTypeRecord>();
            pr = new List<PointRecord>();
            mpr = new List<MultiPointRecord>();
            cr = new List<CurveRecord>();
            ccr = new List<CompositeCurveRecord>();
            sr = new List<SurfaceRecord>();
            ftr = new List<FeatureTypeRecord>();
        }
    } // S-101 구조의 데이터 레코드 구조 클래스
}
