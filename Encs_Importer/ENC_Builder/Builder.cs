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
using Encs_Importer.ISO_IEC_8211;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Data;

using Encs_Importer.Model_S101;

namespace Encs_Importer.ENC_Builder
{
    class Builder 
    {
        Dataset dataset; //{Point1[], MultiPoint1[], Curve1[], CompositeCurve1[], Surface1[], Features} 

        public enum ENCODING_TYPE {ISO8211} // 사용할 인코딩 타입을 추가 하여 사용.
        public enum DATA_TYPE {S101} // 사용할 데이터 타입을 추가 하여 사용.

        DATA_TYPE dataType; //바인딩할 데이터 타입 설정 변수
        ENCODING_TYPE EncodingType; //인코딩 타입 설정 변수
        
        ISO_8211 iSO_8211; // ISO8211 인코딩 타입 변수
        S101 s101; // S-101 바인딩 데이터 타입 변수

        public Builder(ENCODING_TYPE type, DATA_TYPE dataType) // 빌더 생성자(초기화) 함수
        {
            this.dataType = dataType; 
            this.EncodingType = type;
            iSO_8211 = new ISO_8211();
        }

        public void Build(byte[] readDataArray) // 전자해도 원본 데이터(.000) 빌더 함수
        {

            switch (EncodingType)
            {
                case ENCODING_TYPE.ISO8211:
                    if (dataType == DATA_TYPE.S101)
                           s101 =   S_101BinderingTo(iSO_8211.Load_ISO_IEC_8211(readDataArray));
                    break;
                default:
                    break;
            }
            BindingInputSchema();
        }

        private void BindingInputSchema()  
        {
            switch (dataType)
            {
                case DATA_TYPE.S101:
                    dataset = new Dataset();

                    //Information Types
                    InformationTypes informationTypes = new InformationTypes();
                    for (int i = 0; i < s101.itr.Count; i++)
                    {
                        InformationTypeRecord informationType = s101.itr[i];
                        string name = s101.dsgir.dsid.ITCS[informationType.irid.nitc];
                        
                        if(name.Equals("SpatialQuality"))
                        {
                            SpatialQuality sq = new SpatialQuality();
                            sq.id = i.ToString();
                            //qualityOfPosition
                        }
                        else if(name.Equals("ChartNote"))
                        {

                        }
                    }

                    //Points
                    Point1[] points = new Point1[s101.pr.Count];
                    for (int i = 0; i < s101.pr.Count; i++)
                    {
                        PointRecord point = s101.pr[i];
                        Point1 inputPoint = new Point1();
                        inputPoint.id = (i + 1).ToString();
                        //fix 
                        Coordinate2D coor = new Coordinate2D();
                        coor.x = point.c2it[0].coor.xcoo;
                        coor.y = point.c2it[0].coor.ycoo;
                        inputPoint.Coordinate2D = coor;
                        points[i] = inputPoint;
                    }
                    dataset.Points = points;

                    //MultiPoints
                    MultiPoint1[] multiPoints = new MultiPoint1[s101.mpr.Count];
                    for (int i = 0; i < s101.mpr.Count; i++)
                    {
                        MultiPointRecord multiPoint = s101.mpr[i];
                        //fix
                        for (int z = 0; z < multiPoint.c3il.Count; z++)
                        {
                            var coorList = multiPoint.c3il[z];
                            MultiPoint1 inputMultiPoint = new MultiPoint1();
                            inputMultiPoint.id = (i + 1).ToString();
                            inputMultiPoint.Coordinate3D = new Coordinate3D[coorList.coor.Count];
                            for (int k = 0; k < coorList.coor.Count; k++)
                            {
                                Coordinate3D coor = new Coordinate3D();
                                coor.x = coorList.coor[k].xcoo;
                                coor.y = coorList.coor[k].ycoo;
                                coor.z = coorList.coor[k].zcoo;
                                inputMultiPoint.Coordinate3D[k] = coor;
                            }
                            multiPoints[z] = inputMultiPoint;
                           
                        }
                        
                    }
                    dataset.MultiPoints = multiPoints;

                    //Curves
                    Curve1[] curves = new Curve1[s101.cr.Count];
                    for (int i = 0; i < s101.cr.Count; i++)
                    {
                        var curve = s101.cr[i];
                        Curve1 inputCurve = new Curve1();
                        inputCurve.id = (i + 1).ToString();

                        inputCurve.Boundary = new BoundaryRelation[curve.ptas.Count];
                        for (int z = 0; z < curve.ptas.Count; z++)
                        {
                            BoundaryRelation br = new BoundaryRelation();

                            var ptas = curve.ptas[z];
                            br.@ref = ptas.rrid.ToString();

                            BoundaryType type = (BoundaryType)Enum.Parse(typeof(BoundaryType), ptas.topi.ToString());
                            br.boundaryType = type;
       
                            inputCurve.Boundary[z] = br;
                        }

                        Segment[] segment = new Segment[curve.segh.Count];
                        for (int z = 0; z < curve.segh.Count; z++)
			            {
                            Segment sgm = new Segment();
			                var segh = curve.segh[z];
                            InterpolationType type =(InterpolationType) Enum.Parse(typeof(InterpolationType),segh.intp.ToString());
                            sgm.interpolation = type;

                        
                            List<Coordinate2D> coores = new List<Coordinate2D>();
                           foreach (var c2il in segh.c2il)
	                       {
                               foreach (var coor in c2il.coor)
                               {
                                   Coordinate2D coor2d = new Coordinate2D();
                                   coor2d.y = coor.ycoo;
                                   coor2d.x = coor.xcoo;
                                   coores.Add(coor2d);
                               }
                            }
                            sgm.ControlPoint = coores.ToArray();
                            segment[z] = sgm;
			            }
                        inputCurve.Segment = segment;

                        curves[i] = inputCurve;
                    }
                    dataset.Curves = curves;

                    //CompositeCurves
                    CompositeCurve1[] compositeCurves = new CompositeCurve1[s101.ccr.Count];
                    for (int i = 0; i < s101.ccr.Count; i++)
                    {
                        var compositeCurve = s101.ccr[i];
                        CompositeCurve1 inputCompositeCurve = new CompositeCurve1();
                        inputCompositeCurve.id = (i + 1).ToString();
                        List<CurveRelation> cl = new List<CurveRelation>();
                        List<CurveRelation> ccl = new List<CurveRelation>();
                        foreach (var cuco in compositeCurve.cuco)
                        {
                            if (cuco.rrnm == 120) //curve
                            {
                                CurveRelation cr = new CurveRelation();
                                cr.@ref = cuco.rrid.ToString();
                                cr.orientation = (Orientation)Enum.Parse(typeof(Orientation), cuco.ornt.ToString());
                                cl.Add(cr);
                            }
                            else if (cuco.rrnm == 125) //compositCurve
                            {
                                CurveRelation cr = new CurveRelation();
                                cr.@ref = cuco.rrid.ToString();
                                cr.orientation = (Orientation)Enum.Parse(typeof(Orientation), cuco.ornt.ToString());
                                ccl.Add(cr);
                            }
                        }
                        inputCompositeCurve.Curve = cl.ToArray();
                        inputCompositeCurve.CompositeCurve1 = ccl.ToArray();
                        compositeCurves[i] = inputCompositeCurve;
                    }
                    dataset.CompositeCurves = compositeCurves;

                    //surfaces //fix
                    Surface1[] surfaces = new Surface1[s101.sr.Count];
                    for (int i = 0; i < s101.sr.Count; i++)
                    {
                        var surface = s101.sr[i];
                        Surface1 inputSurface = new Surface1();
                        inputSurface.id = (i + 1).ToString();
                        List<Ring> interiorRing = new List<Ring>();
                        List<Ring> ExteriorRing = new List<Ring>();
                        foreach (var rias in surface.rias)
                        {
                            Ring ring = new Ring();
                            List<CurveRelation> crl = new List<CurveRelation>();
                            List<ItemsChoiceType> ict = new List<ItemsChoiceType>();
                            if (rias.rrnm == 120) //curve
                            {
                                CurveRelation cr = new CurveRelation();
                                cr.@ref = rias.rrid.ToString();
                                cr.orientation = (Orientation)Enum.Parse(typeof(Orientation), rias.ornt.ToString());
                                ict.Add(ItemsChoiceType.Curve);
                                crl.Add(cr);
                            }
                            else if (rias.rrnm == 125)//compositeCurve
                            {
                                CurveRelation cr = new CurveRelation();
                                cr.@ref = rias.rrid.ToString();
                                cr.orientation = (Orientation)Enum.Parse(typeof(Orientation), rias.ornt.ToString());
                                ict.Add(ItemsChoiceType.CompositeCurve);
                                crl.Add(cr);
                            }
                            ring.Items = crl.ToArray();
                            ring.ItemsElementName = ict.ToArray();

                            if (rias.usag == 1)
                            {
                                interiorRing.Add(ring);
                            }
                            else if (rias.usag == 2)
                            {
                                ExteriorRing.Add(ring);
                            }
                        }
                        inputSurface.InnerRing = interiorRing.ToArray();
                        inputSurface.OuterRing = ExteriorRing.ToArray();
                        surfaces[i] = inputSurface;
                    }
                    dataset.Surfaces = surfaces;

                    //feature
                    Features features = new Features();
                    List<DepthArea> dal = new List<DepthArea>();
                    List<Landmark> lm = new List<Landmark>();
                    for (int i = 0; i < s101.ftr.Count; i++)
                    {
                        var feature = s101.ftr[i];

                        string featureName = s101.dsgir.dsid.FTCS[feature.frid.nftc];

                        if (featureName.Equals("DepthArea"))
                        {
                            DepthArea da = new DepthArea();
                            da.id = (i + 1).ToString();

                            foreach (var attr in feature.attr)
                            {
                                string attrName = s101.dsgir.dsid.ATCS[attr.natc];
                                if (attrName.Equals("depthRangeMinimumValue"))
                                {
                                    if (attr.atvl.Equals(""))
                                        da.depthValue1Specified = false;
                                    else
                                    {
                                        da.depthValue1Specified = true;
                                        da.depthRangeMinimumValue = Convert.ToDouble(attr.atvl);
                                    }
                                }
                                else if (attrName.Equals("depthRangeMaximumValue"))
                                {
                                    if (attr.atvl.Equals(""))
                                        da.depthValue2Specified = false;
                                    else
                                    {
                                        da.depthValue2Specified = true;
                                        da.depthRangeMaximumValue = Convert.ToDouble(attr.atvl);
                                    }
                                }
                                
                            }

                            List<CurveRelation> cl = new List<CurveRelation>();
                            List<CurveRelation> ccl = new List<CurveRelation>();
                            List<MaskedRelation> pl = new List<MaskedRelation>();
                            List<MaskedRelation> mpl = new List<MaskedRelation>();
                            List<MaskedRelation> sl = new List<MaskedRelation>();
                            foreach (var spas in feature.spas)
                            {
                                if (spas.rrnm == S101.Point)
                                {
                                    MaskedRelation mr = new MaskedRelation();
                                    da.primitive = GeometricPrimitive.Point;
                                    mr.@ref = spas.rrid.ToString();
                                    pl.Add(mr);
                                }
                                else if (spas.rrnm == S101.MultiPoint)
                                {
                                    MaskedRelation mr = new MaskedRelation();
                                    da.primitive = GeometricPrimitive.MultiPoint;
                                    mr.@ref = spas.rrid.ToString();
                                    mpl.Add(mr);
                                }
                                else if (spas.rrnm == S101.Curve)
                                {
                                    CurveRelation cr = new CurveRelation();
                                    da.primitive = GeometricPrimitive.Curve;
                                    cr.@ref = spas.rrid.ToString();
                                    cr.orientation = (Orientation)Enum.Parse(typeof(Orientation), spas.ornt.ToString());
                                    cl.Add(cr);
                                }
                                else if (spas.rrnm == S101.Surface)
                                {
                                    MaskedRelation mr = new MaskedRelation();
                                    da.primitive = GeometricPrimitive.Surface;
                                    mr.@ref = spas.rrid.ToString();
                                    sl.Add(mr);
                                }
                                else if (spas.rrnm == S101.CompositeCurve)
                                {
                                    CurveRelation cr = new CurveRelation();
                                    da.primitive = GeometricPrimitive.Curve;
                                    cr.@ref = spas.rrid.ToString();
                                    cr.orientation = (Orientation)Enum.Parse(typeof(Orientation), spas.ornt.ToString());
                                    ccl.Add(cr);
                                }

                            }
                            da.Curve = cl.ToArray();
                            da.Point = pl.ToArray();
                            da.Surface = sl.ToArray();
                            da.CompositeCurve = ccl.ToArray();
                            da.PointSet = mpl.ToArray();
                            dal.Add(da);
                            
                        }
                        else if (featureName.Equals("Landmark"))
                        {
                            Landmark landmark = new Landmark();
                            landmark.id = (i + 1).ToString();

                            foreach (var attr in feature.attr)
                            {
                                int d = 3;
                            }
                            foreach (var inas in feature.inas)
                            {
                                string informationName = s101.dsgir.dsid.IACS[inas.niac];
                                string roleName = s101.dsgir.dsid.ARCS[inas.narc];
                            
                                int s = 5;
                                foreach (var natc in inas.natc)
                                {
                                    
                                }
                            }
                            landmark.objectName = "11";
                            List<CurveRelation> cl = new List<CurveRelation>();
                            List<CurveRelation> ccl = new List<CurveRelation>();
                            List<MaskedRelation> pl = new List<MaskedRelation>();
                            List<MaskedRelation> mpl = new List<MaskedRelation>();
                            List<MaskedRelation> sl = new List<MaskedRelation>();
                            foreach (var spas in feature.spas)
                            {
                                if (spas.rrnm == S101.Point)
                                {
                                    MaskedRelation mr = new MaskedRelation();
                                    landmark.primitive = GeometricPrimitive.Point;
                                    mr.@ref = spas.rrid.ToString();
                                    pl.Add(mr);
                                }
                                else if (spas.rrnm == S101.MultiPoint)
                                {
                                    MaskedRelation mr = new MaskedRelation();
                                    landmark.primitive = GeometricPrimitive.Point;
                                    mr.@ref = spas.rrid.ToString();
                                    mpl.Add(mr);
                                }
                                else if (spas.rrnm == S101.Curve)
                                {
                                    CurveRelation cr = new CurveRelation();
                                    landmark.primitive = GeometricPrimitive.Curve;
                                    cr.@ref = spas.rrid.ToString();
                                    cr.orientation = (Orientation)Enum.Parse(typeof(Orientation), spas.ornt.ToString());
                                    cl.Add(cr);
                                }
                                else if (spas.rrnm == S101.Surface)
                                {
                                    MaskedRelation mr = new MaskedRelation();
                                    landmark.primitive = GeometricPrimitive.Point;
                                    mr.@ref = spas.rrid.ToString();
                                    sl.Add(mr);
                                }
                                else if (spas.rrnm == S101.CompositeCurve)
                                {
                                    CurveRelation cr = new CurveRelation();
                                    landmark.primitive = GeometricPrimitive.Curve;
                                    cr.@ref = spas.rrid.ToString();
                                    cr.orientation = (Orientation)Enum.Parse(typeof(Orientation), spas.ornt.ToString());
                                    ccl.Add(cr);
                                }

                            }
                            landmark.CompositeCurve = ccl.ToArray();
                            landmark.Curve = cl.ToArray();
                            landmark.Point = pl.ToArray();
                            landmark.PointSet = mpl.ToArray();
                            landmark.Surface = sl.ToArray();

                            lm.Add(landmark);
                        }
                        else if (featureName.Equals("LandArea"))
                        {
                           
                        }
                        
                    }

                    features.DepthArea = dal.ToArray();
                    features.Landmark = lm.ToArray();
                    dataset.Features = features;
   
                    break;
                default:
                    break;
            }
           
        }  //바인딩 input schema 처리 함수 

        public void Generator_XML(string fileLocation) // XML 생성자(초기화) 함수
        {
            switch (dataType)
            {
                case DATA_TYPE.S101:
                     XmlSerializer ser = new XmlSerializer(typeof(Dataset));
                     ser.Serialize(new StreamWriter(fileLocation), dataset);
                    break;
                default:
                    break;
            }
            Console.WriteLine("done");
           
            
        }

        private S101 S_101BinderingTo(List<List<Common.Common_Record>> builderRecordList) //S-101 구조로 바인딩 함수
        {

            S101 s101 = new S101();
            foreach (var records in builderRecordList)
            {
                string typeTag = records[0].tag;
                if (typeTag.Equals("DSID"))
                {
                    #region DSID
                    DataSetGeneralInformationRecord dsgir = new DataSetGeneralInformationRecord();

                    foreach (var record in records)
                    {
                        DataSetGeneralInformationRecord.DSGIR_Type tag = (DataSetGeneralInformationRecord.DSGIR_Type)Enum.Parse(typeof(DataSetGeneralInformationRecord.DSGIR_Type), record.tag);
                        switch (tag)
                        {
                            case DataSetGeneralInformationRecord.DSGIR_Type.DSID:
                               
                                dsgir.dsid.rcnm = record.SubRecords[0].value.uintValue;
                                dsgir.dsid.rcid = record.SubRecords[1].value.uintValue;
                                dsgir.dsid.ensp = record.SubRecords[2].value.stringValue;
                                dsgir.dsid.ened = record.SubRecords[3].value.stringValue;
                                dsgir.dsid.prsp = record.SubRecords[4].value.stringValue;
                                dsgir.dsid.pred = record.SubRecords[5].value.stringValue;
                                dsgir.dsid.prof = record.SubRecords[6].value.stringValue;
                                dsgir.dsid.dsnm = record.SubRecords[7].value.stringValue;
                                dsgir.dsid.dstl = record.SubRecords[8].value.stringValue;
                                dsgir.dsid.dsrd = record.SubRecords[9].value.stringValue;
                                dsgir.dsid.dslg = record.SubRecords[10].value.stringValue;
                                dsgir.dsid.dsab = record.SubRecords[11].value.stringValue;
                                dsgir.dsid.dsed = record.SubRecords[12].value.stringValue;
                                dsgir.dsid.dstc = new List<uint>();
                                for (int i = 13; i < record.SubRecords.Count; i++)
                                {
                                    dsgir.dsid.dstc.Add(record.SubRecords[i].value.uintValue);
                                }
                                break;
                            case DataSetGeneralInformationRecord.DSGIR_Type.DSSI:
                                dsgir.dsid.dssi = new DataSetGeneralInformationRecord.DSID.DSSI();
                                dsgir.dsid.dssi.dcox = record.SubRecords[0].value.doubleValue;
                                dsgir.dsid.dssi.dcoy = record.SubRecords[1].value.doubleValue;
                                dsgir.dsid.dssi.dcoz = record.SubRecords[2].value.doubleValue;
                                dsgir.dsid.dssi.cmfx = record.SubRecords[3].value.uintValue;
                                dsgir.dsid.dssi.cmfy = record.SubRecords[4].value.uintValue;
                                dsgir.dsid.dssi.cmfz = record.SubRecords[5].value.uintValue;
                                dsgir.dsid.dssi.noir = record.SubRecords[6].value.uintValue;
                                dsgir.dsid.dssi.nopn = record.SubRecords[7].value.uintValue;
                                dsgir.dsid.dssi.nomn = record.SubRecords[8].value.uintValue;
                                dsgir.dsid.dssi.nocn = record.SubRecords[9].value.uintValue;
                                dsgir.dsid.dssi.noxn = record.SubRecords[10].value.uintValue;
                                dsgir.dsid.dssi.nosn = record.SubRecords[11].value.uintValue;
                                dsgir.dsid.dssi.nofr = record.SubRecords[12].value.uintValue;
                                break;
                            case DataSetGeneralInformationRecord.DSGIR_Type.ATCS:
                                for (int i = 0; i < record.SubRecords.Count; i+=2)
                                {
                                    dsgir.dsid.ATCS.Add((ushort)record.SubRecords[i + 1].value.uintValue, record.SubRecords[i].value.stringValue);
                                }
                                break;
                            case DataSetGeneralInformationRecord.DSGIR_Type.ITCS:
                                for (int i = 0; i < record.SubRecords.Count; i += 2)
                                {
                                    dsgir.dsid.ITCS.Add((ushort)record.SubRecords[i + 1].value.uintValue, record.SubRecords[i].value.stringValue);
                                }
                                break;
                            case DataSetGeneralInformationRecord.DSGIR_Type.FTCS:
                                for (int i = 0; i < record.SubRecords.Count; i += 2)
                                {
                                    dsgir.dsid.FTCS.Add((ushort)record.SubRecords[i + 1].value.uintValue, record.SubRecords[i].value.stringValue);
                                }
                                break;
                            case DataSetGeneralInformationRecord.DSGIR_Type.IACS:             
                                for (int i = 0; i < record.SubRecords.Count; i += 2)
                                {
                                    dsgir.dsid.IACS.Add((ushort)record.SubRecords[i + 1].value.uintValue, record.SubRecords[i].value.stringValue);
                                }
                                break;
                            case DataSetGeneralInformationRecord.DSGIR_Type.FACS:
                                for (int i = 0; i < record.SubRecords.Count; i += 2)
                                {
                                    dsgir.dsid.FACS.Add((ushort)record.SubRecords[i + 1].value.uintValue, record.SubRecords[i].value.stringValue);
                                }
                                break;
                            case DataSetGeneralInformationRecord.DSGIR_Type.ARCS:
                                for (int i = 0; i < record.SubRecords.Count; i += 2)
                                {
                                    dsgir.dsid.ARCS.Add((ushort)record.SubRecords[i + 1].value.uintValue, record.SubRecords[i].value.stringValue);
                                }
                                break;
                            case DataSetGeneralInformationRecord.DSGIR_Type.ATTR:
                                for (int i = 0; i < record.SubRecords.Count;)
                                {
                                    CommonRecord.ATTR attr = new CommonRecord.ATTR();
                                    attr.natc = (ushort) record.SubRecords[i++].value.uintValue;
                                    attr.atix = (ushort)record.SubRecords[i++].value.uintValue;
                                    attr.paix = (ushort)record.SubRecords[i++].value.uintValue;
                                    attr.atin = record.SubRecords[i++].value.uintValue;
                                    attr.atvl = record.SubRecords[i++].value.stringValue;
                                    dsgir.dsid.ATTR.Add(attr);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    s101.dsgir = dsgir;
                    #endregion
                }
                else if (typeTag.Equals("CSID"))
                {
                    #region CSID
                    DataSetCoordinateReferenceSystemRecord dscrsr = new DataSetCoordinateReferenceSystemRecord();

                    foreach (var record in records)
                    {
                        DataSetCoordinateReferenceSystemRecord.DSCRSR_Type tag = (DataSetCoordinateReferenceSystemRecord.DSCRSR_Type)Enum.Parse(typeof(DataSetCoordinateReferenceSystemRecord.DSCRSR_Type), record.tag);
                        switch (tag)
                        {
                            case DataSetCoordinateReferenceSystemRecord.DSCRSR_Type.CSID:
                                dscrsr.csid.rcnm = record.SubRecords[0].value.uintValue;
                                dscrsr.csid.rcid = record.SubRecords[1].value.uintValue;
                                dscrsr.csid.ncrc = record.SubRecords[2].value.uintValue;
                                break;
                            case DataSetCoordinateReferenceSystemRecord.DSCRSR_Type.CRSH:
                                DataSetCoordinateReferenceSystemRecord.CRSH crsh = new DataSetCoordinateReferenceSystemRecord.CRSH();
                                crsh.crix = record.SubRecords[0].value.uintValue;
                                crsh.crst = record.SubRecords[1].value.uintValue;
                                crsh.csty = record.SubRecords[2].value.uintValue;
                                crsh.crnm = record.SubRecords[3].value.stringValue;
                                crsh.crsi = record.SubRecords[4].value.stringValue;
                                crsh.crss = record.SubRecords[5].value.uintValue;
                                crsh.scri = record.SubRecords[6].value.stringValue;
                                dscrsr.csid.crsh.Add(crsh);
                                break;
                            case DataSetCoordinateReferenceSystemRecord.DSCRSR_Type.CSAX:
                                DataSetCoordinateReferenceSystemRecord.CRSH targetCrshForCsax = dscrsr.csid.crsh[dscrsr.csid.crsh.Count - 1];
                                targetCrshForCsax.csax = new List<DataSetCoordinateReferenceSystemRecord.CRSH.CSAX>();
                                for (int i = 0; i < record.SubRecords.Count; i+=2)
                                {
                                    DataSetCoordinateReferenceSystemRecord.CRSH.CSAX csax = new DataSetCoordinateReferenceSystemRecord.CRSH.CSAX();
                                    csax.axty = record.SubRecords[i].value.uintValue;
                                    csax.axum = record.SubRecords[i + 1].value.uintValue;
                                    targetCrshForCsax.csax.Add(csax);
                                }
                                dscrsr.csid.crsh[dscrsr.csid.crsh.Count - 1] = targetCrshForCsax;
                                break;
                            case DataSetCoordinateReferenceSystemRecord.DSCRSR_Type.PROJ:
                                DataSetCoordinateReferenceSystemRecord.CRSH targetCrshForProj = dscrsr.csid.crsh[dscrsr.csid.crsh.Count - 1];
                                targetCrshForProj.proj.prom = record.SubRecords[0].value.uintValue;
                                targetCrshForProj.proj.prp1 = record.SubRecords[1].value.doubleValue;
                                targetCrshForProj.proj.prp2 = record.SubRecords[2].value.doubleValue;
                                targetCrshForProj.proj.prp3 = record.SubRecords[3].value.doubleValue;
                                targetCrshForProj.proj.prp4 = record.SubRecords[4].value.doubleValue;
                                targetCrshForProj.proj.prp5 = record.SubRecords[5].value.doubleValue;
                                targetCrshForProj.proj.feas = record.SubRecords[6].value.doubleValue;
                                targetCrshForProj.proj.fnor = record.SubRecords[7].value.doubleValue;
                                dscrsr.csid.crsh[dscrsr.csid.crsh.Count - 1] = targetCrshForProj;
                                break;
                            case DataSetCoordinateReferenceSystemRecord.DSCRSR_Type.GDAT:
                                DataSetCoordinateReferenceSystemRecord.CRSH targetCrshForGdt = dscrsr.csid.crsh[dscrsr.csid.crsh.Count - 1];
                                targetCrshForGdt.gdat.dtnm = record.SubRecords[0].value.stringValue;
                                targetCrshForGdt.gdat.elnm = record.SubRecords[1].value.stringValue;
                                targetCrshForGdt.gdat.esma = record.SubRecords[2].value.doubleValue;
                                targetCrshForGdt.gdat.espt = record.SubRecords[3].value.uintValue;
                                targetCrshForGdt.gdat.espm = record.SubRecords[4].value.doubleValue;
                                targetCrshForGdt.gdat.cmnm = record.SubRecords[5].value.stringValue;
                                targetCrshForGdt.gdat.cmgl = record.SubRecords[6].value.doubleValue;
                                dscrsr.csid.crsh[dscrsr.csid.crsh.Count - 1] = targetCrshForGdt;
                                break;
                            case DataSetCoordinateReferenceSystemRecord.DSCRSR_Type.VDAT:
                                DataSetCoordinateReferenceSystemRecord.CRSH targetCrshForVdt = dscrsr.csid.crsh[dscrsr.csid.crsh.Count - 1];
                                targetCrshForVdt.vdat.dtnm = record.SubRecords[0].value.stringValue;
                                targetCrshForVdt.vdat.dtid = record.SubRecords[1].value.stringValue;
                                targetCrshForVdt.vdat.dtsr = record.SubRecords[2].value.uintValue;
                                targetCrshForVdt.vdat.scri = record.SubRecords[3].value.stringValue;
                                dscrsr.csid.crsh[dscrsr.csid.crsh.Count - 1] = targetCrshForVdt;
                                break;
                            default:
                                break;
                        }
                    }
                    s101.dscrsr = dscrsr;
                    #endregion
                }
                else if (typeTag.Equals("IRID"))
                {
                    #region
                    InformationTypeRecord itr = new InformationTypeRecord();
                    foreach (var record in records)
                    {
                        InformationTypeRecord.ITR_Tyte tag = (InformationTypeRecord.ITR_Tyte)Enum.Parse(typeof(InformationTypeRecord.ITR_Tyte), record.tag);
                        switch (tag)
                        {
                            case InformationTypeRecord.ITR_Tyte.IRID:
                                itr.irid.rcnm = record.SubRecords[0].value.uintValue;
                                itr.irid.rcid = record.SubRecords[1].value.uintValue;
                                itr.irid.nitc = (ushort)record.SubRecords[2].value.uintValue;
                                itr.irid.rver = (ushort)record.SubRecords[3].value.uintValue;
                                itr.irid.ruin = record.SubRecords[4].value.uintValue;
                                break;
                            case InformationTypeRecord.ITR_Tyte.ATTR:
                                
                                for (int i = 0; i < record.SubRecords.Count;)
                                {
                                    CommonRecord.ATTR attr = new CommonRecord.ATTR();
                                    attr.natc = (ushort) record.SubRecords[i++].value.uintValue;
                                    attr.atix = (ushort)record.SubRecords[i++].value.uintValue;
                                    attr.paix = (ushort)record.SubRecords[i++].value.uintValue;
                                    attr.atin = record.SubRecords[i++].value.uintValue;
                                    attr.atvl = record.SubRecords[i++].value.stringValue;
                                    itr.irid.attr.Add(attr);
                                }
                                break;
                            case InformationTypeRecord.ITR_Tyte.INAS:
                                
                                CommonRecord.INAS inas = new CommonRecord.INAS();
                                inas.rrnm = record.SubRecords[0].value.uintValue;
                                inas.rrid = record.SubRecords[1].value.uintValue;
                                inas.niac = (ushort)record.SubRecords[2].value.uintValue;
                                inas.narc = (ushort)record.SubRecords[3].value.uintValue;
                                inas.iuin = record.SubRecords[4].value.uintValue;
                                inas.natc = new List<CommonRecord.INAS.NATC>();
                                for (int i = 5; i < record.SubRecords.Count;)
                                {
                                    CommonRecord.INAS.NATC natc = new CommonRecord.INAS.NATC();
                                    natc.natc = (ushort)record.SubRecords[i++].value.uintValue;
                                    natc.atix = (ushort)record.SubRecords[i++].value.uintValue;
                                    natc.paix = (ushort)record.SubRecords[i++].value.uintValue;
                                    natc.atin = record.SubRecords[i++].value.uintValue;
                                    natc.atvl = record.SubRecords[i++].value.stringValue;
                                    inas.natc.Add(natc);
                                }
                                itr.irid.inas.Add(inas);
                                break;
                            default:
                                break;
                        }
                    }
                    s101.itr.Add(itr);
                    #endregion
                }
                else if (typeTag.Equals("PRID"))
                {
                    #region
                    PointRecord pr = new PointRecord();                    
                    foreach (var record in records)
                    {
                        PointRecord.PR_Type tag = (PointRecord.PR_Type)Enum.Parse(typeof(PointRecord.PR_Type), record.tag);
                        switch (tag)
                        {
                            case PointRecord.PR_Type.PRID:
                                pr.prid.rcnm = record.SubRecords[0].value.uintValue;
                                pr.prid.rcid = record.SubRecords[1].value.uintValue;
                                pr.prid.rver = (ushort) record.SubRecords[2].value.uintValue;
                                pr.prid.ruin = record.SubRecords[3].value.uintValue;
                                break;
                            case PointRecord.PR_Type.INAS:
                                 CommonRecord.INAS inas = new CommonRecord.INAS();
                                inas.rrnm = record.SubRecords[0].value.uintValue;
                                inas.rrid = record.SubRecords[1].value.uintValue;
                                inas.niac = (ushort)record.SubRecords[2].value.uintValue;
                                inas.narc = (ushort)record.SubRecords[3].value.uintValue;
                                inas.iuin = record.SubRecords[4].value.uintValue;
                                inas.natc = new List<CommonRecord.INAS.NATC>();
                                for (int i = 5; i < record.SubRecords.Count;)
                                {
                                    CommonRecord.INAS.NATC natc = new CommonRecord.INAS.NATC();
                                    natc.natc = (ushort)record.SubRecords[i++].value.uintValue;
                                    natc.atix = (ushort)record.SubRecords[i++].value.uintValue;
                                    natc.paix = (ushort)record.SubRecords[i++].value.uintValue;
                                    natc.atin = record.SubRecords[i++].value.uintValue;
                                    natc.atvl = record.SubRecords[i++].value.stringValue;
                                    inas.natc.Add(natc);
                                }
                                pr.inas.Add(inas);
                                break;
                            case PointRecord.PR_Type.C2IT:
                                CommonRecord.C2IT c2it = new CommonRecord.C2IT();
                                CommonRecord.Coordinate2D<int> coor_c2it = new CommonRecord.Coordinate2D<int>();
                                coor_c2it.ycoo = record.SubRecords[0].value.intValue;
                                coor_c2it.xcoo = record.SubRecords[1].value.intValue;
                                c2it.coor = coor_c2it;
                                pr.c2it.Add(c2it);
                                break;
                            case PointRecord.PR_Type.C3IT:
                                CommonRecord.C3IT c3it = new CommonRecord.C3IT();
                                c3it.vcid = record.SubRecords[0].value.uintValue;
                                CommonRecord.Coordinate3D<int> coor_c3it = new CommonRecord.Coordinate3D<int>();
                                coor_c3it.ycoo = record.SubRecords[0].value.intValue;
                                coor_c3it.xcoo = record.SubRecords[1].value.intValue;
                                coor_c3it.zcoo = record.SubRecords[2].value.intValue;
                                c3it.coor = coor_c3it;
                                pr.c3it.Add(c3it);
                                break;
                            case PointRecord.PR_Type.C2FT:
                                CommonRecord.C2FT c2ft = new CommonRecord.C2FT();
                                CommonRecord.Coordinate2D<double> coor_c2ft = new CommonRecord.Coordinate2D<double>();
                                coor_c2ft.ycoo = record.SubRecords[0].value.doubleValue;
                                coor_c2ft.xcoo = record.SubRecords[1].value.doubleValue;
                                c2ft.coor = coor_c2ft;
                                pr.c2ft.Add(c2ft);
                                break;
                            case PointRecord.PR_Type.C3FT:
                                CommonRecord.C3FT c3ft = new CommonRecord.C3FT();
                                c3ft.vcid = record.SubRecords[0].value.uintValue;
                                CommonRecord.Coordinate3D<double> coor_c3ft = new CommonRecord.Coordinate3D<double>();
                                coor_c3ft.ycoo = record.SubRecords[0].value.doubleValue;
                                coor_c3ft.xcoo = record.SubRecords[1].value.doubleValue;
                                coor_c3ft.zcoo = record.SubRecords[2].value.doubleValue;
                                c3ft.coor = coor_c3ft;
                                pr.c3ft.Add(c3ft);
                                break;
                            default:
                                break;
                        }
                    }
                    s101.pr.Add(pr);
                    #endregion
                }
                else if (typeTag.Equals("MRID"))
                {
                    #region MRID
                    MultiPointRecord mpr = new MultiPointRecord();
                    foreach (var record in records)
                    {
                        MultiPointRecord.MPR_Type tag = (MultiPointRecord.MPR_Type)Enum.Parse(typeof(MultiPointRecord.MPR_Type), record.tag);
                        switch (tag)
                        {
                            case MultiPointRecord.MPR_Type.MRID:
                                mpr.mrid.rcnm = record.SubRecords[0].value.uintValue;
                                mpr.mrid.rcid = record.SubRecords[1].value.uintValue;
                                mpr.mrid.rver = (ushort) record.SubRecords[2].value.uintValue;
                                mpr.mrid.ruin = record.SubRecords[3].value.uintValue;
                                break;
                            case MultiPointRecord.MPR_Type.INAS:
                                CommonRecord.INAS inas = new CommonRecord.INAS();
                                inas.rrnm = record.SubRecords[0].value.uintValue;
                                inas.rrid = record.SubRecords[1].value.uintValue;
                                inas.niac = (ushort)record.SubRecords[2].value.uintValue;
                                inas.narc = (ushort)record.SubRecords[3].value.uintValue;
                                inas.iuin = record.SubRecords[4].value.uintValue;
                                inas.natc = new List<CommonRecord.INAS.NATC>();
                                for (int i = 5; i < record.SubRecords.Count;)
                                {
                                    CommonRecord.INAS.NATC natc = new CommonRecord.INAS.NATC();
                                    natc.natc = (ushort)record.SubRecords[i++].value.uintValue;
                                    natc.atix = (ushort)record.SubRecords[i++].value.uintValue;
                                    natc.paix = (ushort)record.SubRecords[i++].value.uintValue;
                                    natc.atin = record.SubRecords[i++].value.uintValue;
                                    natc.atvl = record.SubRecords[i++].value.stringValue;
                                    inas.natc.Add(natc);
                                }
                                mpr.inas.Add(inas);
                                break;
                            case MultiPointRecord.MPR_Type.COCC:
                                mpr.cocc.coui = record.SubRecords[0].value.uintValue;
                                mpr.cocc.coix = (ushort)record.SubRecords[1].value.uintValue;
                                mpr.cocc.ncor = (ushort)record.SubRecords[2].value.uintValue;
                                break;
                            case MultiPointRecord.MPR_Type.C2IL:
                                CommonRecord.C2IL c2il = new CommonRecord.C2IL();
                                for (int i = 0; i < record.SubRecords.Count; i++)
                                {
                                    CommonRecord.Coordinate2D<int> coor = new CommonRecord.Coordinate2D<int>();
                                    coor.ycoo = record.SubRecords[i++].value.intValue;
                                    coor.xcoo = record.SubRecords[i].value.intValue;
                                    c2il.coor.Add(coor);
                                }
                                mpr.c2il.Add(c2il);
                                break;
                            case MultiPointRecord.MPR_Type.C3IL:
                                CommonRecord.C3IL c3il = new CommonRecord.C3IL();
                                c3il.vcid = record.SubRecords[0].value.uintValue;
                                for (int i = 1; i < record.SubRecords.Count; i++)
                                {
                                    CommonRecord.Coordinate3D<int> coor = new CommonRecord.Coordinate3D<int>();
                                    coor.ycoo = record.SubRecords[i++].value.intValue;
                                    coor.xcoo = record.SubRecords[i++].value.intValue;
                                    coor.zcoo = record.SubRecords[i].value.intValue;
                                    c3il.coor.Add(coor);
                                }
                                mpr.c3il.Add(c3il);
                                break;
                            case MultiPointRecord.MPR_Type.C2FL:
                                CommonRecord.C2FL c2fl = new CommonRecord.C2FL();
                                for (int i = 0; i < record.SubRecords.Count; i++)
                                {
                                    CommonRecord.Coordinate2D<double> coor = new CommonRecord.Coordinate2D<double>();
                                    coor.ycoo = record.SubRecords[i++].value.doubleValue;
                                    coor.xcoo = record.SubRecords[i].value.doubleValue;
                                    c2fl.coor.Add(coor);
                                }
                                mpr.c2fl.Add(c2fl);
                                break;
                            case MultiPointRecord.MPR_Type.C3FL:
                                CommonRecord.C3FL c3fl = new CommonRecord.C3FL();
                                c3fl.vcid = record.SubRecords[0].value.uintValue;
                                for (int i = 1; i < record.SubRecords.Count; i++)
                                {
                                    CommonRecord.Coordinate3D<double> coor = new CommonRecord.Coordinate3D<double>();
                                    coor.ycoo = record.SubRecords[i++].value.doubleValue;
                                    coor.xcoo = record.SubRecords[i++].value.doubleValue;
                                    coor.ycoo = record.SubRecords[i].value.doubleValue;
                                    c3fl.coor.Add(coor);
                                }
                                mpr.c3fl.Add(c3fl);
                                break;
                            default:
                                break;
                        }
                    }
                    s101.mpr.Add(mpr);
                    #endregion
                }
                else if (typeTag.Equals("CRID"))
                {
                    #region CRID
                    CurveRecord cr = new CurveRecord();
                    foreach (var record in records)
                    {
                        CurveRecord.CR_Type tag = (CurveRecord.CR_Type)Enum.Parse(typeof(CurveRecord.CR_Type), record.tag);
                        switch (tag)
                        {
                            case CurveRecord.CR_Type.CRID:
                                cr.crid.rcnm = record.SubRecords[0].value.uintValue;
                                cr.crid.rcid = record.SubRecords[1].value.uintValue;
                                cr.crid.rver = (ushort)record.SubRecords[2].value.uintValue;
                                cr.crid.ruin = record.SubRecords[3].value.uintValue;
                                break;
                            case CurveRecord.CR_Type.INAS:
                                CommonRecord.INAS inas = new CommonRecord.INAS();
                                inas.rrnm = record.SubRecords[0].value.uintValue;
                                inas.rrid = record.SubRecords[1].value.uintValue;
                                inas.niac = (ushort)record.SubRecords[2].value.uintValue;
                                inas.narc = (ushort)record.SubRecords[3].value.uintValue;
                                inas.iuin = record.SubRecords[4].value.uintValue;
                                inas.natc = new List<CommonRecord.INAS.NATC>();
                                for (int i = 5; i < record.SubRecords.Count;)
                                {
                                    CommonRecord.INAS.NATC natc = new CommonRecord.INAS.NATC();
                                    natc.natc = (ushort)record.SubRecords[i++].value.uintValue;
                                    natc.atix = (ushort)record.SubRecords[i++].value.uintValue;
                                    natc.paix = (ushort)record.SubRecords[i++].value.uintValue;
                                    natc.atin = record.SubRecords[i++].value.uintValue;
                                    natc.atvl = record.SubRecords[i++].value.stringValue;
                                    inas.natc.Add(natc);
                                }
                                cr.inas.Add(inas);
                                break;
                            case CurveRecord.CR_Type.PTAS:
                                for (int i = 0; i < record.SubRecords.Count; i++)
                                {
                                    CurveRecord.PTAS ptas = new CurveRecord.PTAS();
                                    ptas.rrnm = record.SubRecords[i++].value.uintValue;
                                    ptas.rrid = record.SubRecords[i++].value.uintValue;
                                    ptas.topi = record.SubRecords[i].value.uintValue;
                                    cr.ptas.Add(ptas);
                                }
                                break;
                            case CurveRecord.CR_Type.SECC:
                                cr.secc.seui = record.SubRecords[0].value.uintValue;
                                cr.secc.seix = (ushort)record.SubRecords[1].value.uintValue;
                                cr.secc.nseg = (ushort)record.SubRecords[2].value.uintValue;
                                break;
                            case CurveRecord.CR_Type.SEGH:
                                CurveRecord.SEGH segh = new CurveRecord.SEGH();
                                segh.intp = record.SubRecords[0].value.uintValue;
                                //segh.circ = record.SubRecords[1].value.uintValue;
                                //segh.coor.ycoo = record.SubRecords[2].value.doubleValue;
                                //segh.coor.xcoo = record.SubRecords[3].value.doubleValue;
                                //segh.dist = record.SubRecords[4].value.doubleValue;
                                //segh.disu = record.SubRecords[5].value.uintValue;
                                //segh.sbrg = record.SubRecords[6].value.doubleValue;
                                //segh.angl = record.SubRecords[7].value.doubleValue;
                                cr.segh.Add(segh);
                                break;
                            case CurveRecord.CR_Type.COCC:
                                CurveRecord.SEGH segh_Cocc = cr.segh[cr.segh.Count-1];
                                segh_Cocc.cocc.coui = record.SubRecords[0].value.uintValue;
                                segh_Cocc.cocc.coix = (ushort)record.SubRecords[1].value.uintValue;
                                segh_Cocc.cocc.ncor = (ushort)record.SubRecords[2].value.uintValue;
                                cr.segh[cr.segh.Count - 1] = segh_Cocc;
                                break;
                            case CurveRecord.CR_Type.C2IL:
                                CurveRecord.SEGH segh_C2IL = cr.segh[cr.segh.Count - 1];
                                CommonRecord.C2IL c2il = new CommonRecord.C2IL();
                                for (int i = 0; i < record.SubRecords.Count; i++)
                                {
                                    CommonRecord.Coordinate2D<int> coor = new CommonRecord.Coordinate2D<int>();
                                    coor.ycoo = record.SubRecords[i++].value.intValue;
                                    coor.xcoo = record.SubRecords[i].value.intValue;
                                    c2il.coor.Add(coor);
                                }
                                segh_C2IL.c2il.Add(c2il);
                                cr.segh[cr.segh.Count - 1] = segh_C2IL;
                                break;
                            case CurveRecord.CR_Type.C3IL:
                                CurveRecord.SEGH segh_C3IL = cr.segh[cr.segh.Count - 1];
                                 CommonRecord.C3IL c3il = new CommonRecord.C3IL();
                                c3il.vcid = record.SubRecords[0].value.uintValue;
                                for (int i = 1; i < record.SubRecords.Count; i++)
                                {
                                    CommonRecord.Coordinate3D<int> coor = new CommonRecord.Coordinate3D<int>();
                                    coor.ycoo = record.SubRecords[i++].value.intValue;
                                    coor.xcoo = record.SubRecords[i++].value.intValue;
                                    coor.zcoo = record.SubRecords[i].value.intValue;
                                    c3il.coor.Add(coor);
                                }
                                segh_C3IL.c3il.Add(c3il);
                                cr.segh[cr.segh.Count - 1] = segh_C3IL;
                                break;
                            case CurveRecord.CR_Type.C2FL:
                                CurveRecord.SEGH segh_C2FL = cr.segh[cr.segh.Count - 1];
                                CommonRecord.C2FL c2fl = new CommonRecord.C2FL();
                                for (int i = 0; i < record.SubRecords.Count; i++)
                                {
                                    CommonRecord.Coordinate2D<double> coor = new CommonRecord.Coordinate2D<double>();
                                    coor.ycoo = record.SubRecords[i++].value.doubleValue;
                                    coor.xcoo = record.SubRecords[i].value.doubleValue;
                                    c2fl.coor.Add(coor);
                                }
                                segh_C2FL.c2fl.Add(c2fl);
                                cr.segh[cr.segh.Count - 1] = segh_C2FL;
                                break;
                            case CurveRecord.CR_Type.C3FL:
                                CurveRecord.SEGH segh_C3FL = cr.segh[cr.segh.Count - 1];
                                 CommonRecord.C3FL c3fl = new CommonRecord.C3FL();
                                c3fl.vcid = record.SubRecords[0].value.uintValue;
                                for (int i = 1; i < record.SubRecords.Count; i++)
                                {
                                    CommonRecord.Coordinate3D<double> coor = new CommonRecord.Coordinate3D<double>();
                                    coor.ycoo = record.SubRecords[i++].value.doubleValue;
                                    coor.xcoo = record.SubRecords[i++].value.doubleValue;
                                    coor.ycoo = record.SubRecords[i].value.doubleValue;
                                    c3fl.coor.Add(coor);
                                }
                                segh_C3FL.c3fl.Add(c3fl);
                                cr.segh[cr.segh.Count - 1] = segh_C3FL;
                                break;
                            default:
                                break;
                        }
                    }
                    s101.cr.Add(cr);
                    #endregion
                }
                else if (typeTag.Equals("CCID"))
                {
                    #region CCID
                    CompositeCurveRecord ccr = new CompositeCurveRecord();
                    foreach (var record in records)
                    {
                        CompositeCurveRecord.CCR_Type tag = (CompositeCurveRecord.CCR_Type)Enum.Parse(typeof(CompositeCurveRecord.CCR_Type), record.tag);
                        switch (tag)
                        {
                            case CompositeCurveRecord.CCR_Type.CCID:
                                ccr.ccid.rcnm = record.SubRecords[0].value.uintValue;
                                ccr.ccid.rcid = record.SubRecords[1].value.uintValue;
                                ccr.ccid.rver = (ushort) record.SubRecords[2].value.uintValue;
                                ccr.ccid.ruin = record.SubRecords[3].value.uintValue;
                                break;
                            case CompositeCurveRecord.CCR_Type.INAS:
                                 CommonRecord.INAS inas = new CommonRecord.INAS();
                                inas.rrnm = record.SubRecords[0].value.uintValue;
                                inas.rrid = record.SubRecords[1].value.uintValue;
                                inas.niac = (ushort)record.SubRecords[2].value.uintValue;
                                inas.narc = (ushort)record.SubRecords[3].value.uintValue;
                                inas.iuin = record.SubRecords[4].value.uintValue;
                                inas.natc = new List<CommonRecord.INAS.NATC>();
                                for (int i = 5; i < record.SubRecords.Count;)
                                {
                                    CommonRecord.INAS.NATC natc = new CommonRecord.INAS.NATC();
                                    natc.natc = (ushort)record.SubRecords[i++].value.uintValue;
                                    natc.atix = (ushort)record.SubRecords[i++].value.uintValue;
                                    natc.paix = (ushort)record.SubRecords[i++].value.uintValue;
                                    natc.atin = record.SubRecords[i++].value.uintValue;
                                    natc.atvl = record.SubRecords[i++].value.stringValue;
                                    inas.natc.Add(natc);
                                }
                                ccr.inas.Add(inas);
                                break;
                            case CompositeCurveRecord.CCR_Type.CCOC:
                                ccr.ccoc.ccui = record.SubRecords[0].value.uintValue;
                                ccr.ccoc.ccix = (ushort) record.SubRecords[1].value.uintValue;
                                ccr.ccoc.ncco = (ushort) record.SubRecords[2].value.uintValue;
                                break;
                            case CompositeCurveRecord.CCR_Type.CUCO:
                                for (int i = 0; i < record.SubRecords.Count; i++)
                                {
                                    CompositeCurveRecord.CUCO cuco = new CompositeCurveRecord.CUCO();
                                    cuco.rrnm = record.SubRecords[i++].value.uintValue;
                                    cuco.rrid = record.SubRecords[i++].value.uintValue;
                                    cuco.ornt = record.SubRecords[i].value.uintValue;
                                    ccr.cuco.Add(cuco);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    s101.ccr.Add(ccr);
                    #endregion
                }
                else if (typeTag.Equals("SRID"))
                {
                    #region SRID
                    SurfaceRecord sr = new SurfaceRecord();
                    foreach (var record in records)
                    {
                        SurfaceRecord.SR_Type tag = (SurfaceRecord.SR_Type)Enum.Parse(typeof(SurfaceRecord.SR_Type), record.tag);

                        switch (tag)
                        {
                            case SurfaceRecord.SR_Type.SRID:
                                sr.srid.rcnm = record.SubRecords[0].value.uintValue;
                                sr.srid.rcid = record.SubRecords[1].value.uintValue;
                                sr.srid.rver = (ushort) record.SubRecords[2].value.uintValue;
                                sr.srid.ruin = record.SubRecords[3].value.uintValue;
                                break;
                            case SurfaceRecord.SR_Type.INAS:
                                CommonRecord.INAS inas = new CommonRecord.INAS();
                                inas.rrnm = record.SubRecords[0].value.uintValue;
                                inas.rrid = record.SubRecords[1].value.uintValue;
                                inas.niac = (ushort)record.SubRecords[2].value.uintValue;
                                inas.narc = (ushort)record.SubRecords[3].value.uintValue;
                                inas.iuin = record.SubRecords[4].value.uintValue;
                                inas.natc = new List<CommonRecord.INAS.NATC>();
                                for (int i = 5; i < record.SubRecords.Count;)
                                {
                                    CommonRecord.INAS.NATC natc = new CommonRecord.INAS.NATC();
                                    natc.natc = (ushort)record.SubRecords[i++].value.uintValue;
                                    natc.atix = (ushort)record.SubRecords[i++].value.uintValue;
                                    natc.paix = (ushort)record.SubRecords[i++].value.uintValue;
                                    natc.atin = record.SubRecords[i++].value.uintValue;
                                    natc.atvl = record.SubRecords[i++].value.stringValue;
                                    inas.natc.Add(natc);
                                }
                                sr.inas.Add(inas);
                                break;
                            case SurfaceRecord.SR_Type.RIAS:
                                for (int i = 0; i < record.SubRecords.Count; i++)
                                {
                                    SurfaceRecord.RIAS rias = new SurfaceRecord.RIAS();
                                    rias.rrnm =  record.SubRecords[i++].value.uintValue;
                                    rias.rrid = record.SubRecords[i++].value.uintValue;
                                    rias.ornt = record.SubRecords[i++].value.uintValue;
                                    rias.usag = record.SubRecords[i++].value.uintValue;
                                    rias.raui = record.SubRecords[i].value.uintValue;
                                    sr.rias.Add(rias);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    s101.sr.Add(sr);
                    #endregion
                }
                else if (typeTag.Equals("FRID"))
                {
                    #region FRID
                    FeatureTypeRecord ftr = new FeatureTypeRecord();
                    foreach (var record in records)
                    {
                        FeatureTypeRecord.FTR_Type tag = (FeatureTypeRecord.FTR_Type)Enum.Parse(typeof(FeatureTypeRecord.FTR_Type), record.tag);
                        switch (tag)
                        {
                            case FeatureTypeRecord.FTR_Type.FRID:
                                ftr.frid.rcnm = record.SubRecords[0].value.uintValue;
                                ftr.frid.rcid = record.SubRecords[1].value.uintValue;
                                ftr.frid.nftc = (ushort) record.SubRecords[2].value.uintValue;
                                ftr.frid.rver = (ushort) record.SubRecords[3].value.uintValue;
                                ftr.frid.ruin = record.SubRecords[4].value.uintValue;
                                break;
                            case FeatureTypeRecord.FTR_Type.FOID:
                                ftr.foid.agen = (ushort) record.SubRecords[0].value.uintValue;
                                ftr.foid.fidn = record.SubRecords[1].value.uintValue;
                                ftr.foid.fids = (ushort) record.SubRecords[2].value.uintValue;
                                break;
                            case FeatureTypeRecord.FTR_Type.ATTR:
                                for (int i = 0; i < record.SubRecords.Count; )
                                {
                                    CommonRecord.ATTR attr = new CommonRecord.ATTR();
                                    attr.natc = (ushort)record.SubRecords[i++].value.uintValue;
                                    attr.atix = (ushort)record.SubRecords[i++].value.uintValue;
                                    attr.paix = (ushort)record.SubRecords[i++].value.uintValue;
                                    attr.atin = record.SubRecords[i++].value.uintValue;
                                    attr.atvl = record.SubRecords[i++].value.stringValue;
                                    ftr.attr.Add(attr);
                                }
                                break;
                            case FeatureTypeRecord.FTR_Type.INAS:
                                CommonRecord.INAS inas = new CommonRecord.INAS();
                                inas.rrnm = record.SubRecords[0].value.uintValue;
                                inas.rrid = record.SubRecords[1].value.uintValue;
                                inas.niac = (ushort)record.SubRecords[2].value.uintValue;
                                inas.narc = (ushort)record.SubRecords[3].value.uintValue;
                                inas.iuin = record.SubRecords[4].value.uintValue;
                                inas.natc = new List<CommonRecord.INAS.NATC>();
                                for (int i = 5; i < record.SubRecords.Count;)
                                {
                                    CommonRecord.INAS.NATC natc = new CommonRecord.INAS.NATC();
                                    natc.natc = (ushort)record.SubRecords[i++].value.uintValue;
                                    natc.atix = (ushort)record.SubRecords[i++].value.uintValue;
                                    natc.paix = (ushort)record.SubRecords[i++].value.uintValue;
                                    natc.atin = record.SubRecords[i++].value.uintValue;
                                    natc.atvl = record.SubRecords[i++].value.stringValue;
                                    inas.natc.Add(natc);
                                }
                                ftr.inas.Add(inas);
                                break;
                            case FeatureTypeRecord.FTR_Type.SPAS:
                                for (int i = 0; i < record.SubRecords.Count; i++)
                                {
                                    FeatureTypeRecord.SPAS spas = new FeatureTypeRecord.SPAS();
                                    spas.rrnm =  record.SubRecords[i++].value.uintValue;
                                    spas.rrid = record.SubRecords[i++].value.uintValue;
                                    spas.ornt = record.SubRecords[i++].value.uintValue;
                                    spas.smin = record.SubRecords[i++].value.uintValue;
                                    spas.smax = record.SubRecords[i++].value.uintValue;
                                    spas.saui = record.SubRecords[i].value.uintValue;
                                    ftr.spas.Add(spas);
                                }
                                break;
                            case FeatureTypeRecord.FTR_Type.FASC:
                                FeatureTypeRecord.FASC fasc = new FeatureTypeRecord.FASC();
                                fasc.rrnm = record.SubRecords[0].value.uintValue;
                                fasc.rrid = record.SubRecords[1].value.uintValue;
                                fasc.nfac = (ushort) record.SubRecords[2].value.uintValue;
                                fasc.narc = (ushort) record.SubRecords[3].value.uintValue;
                                fasc.faui = record.SubRecords[4].value.uintValue;
                                for (int i = 5; i < record.SubRecords.Count; i++)
                                {
                                    FeatureTypeRecord.FASC.NATC natc = new FeatureTypeRecord.FASC.NATC();
                                    natc.natc = (ushort)record.SubRecords[i++].value.uintValue;
                                    natc.atix = (ushort)record.SubRecords[i++].value.uintValue;
                                    natc.paix = (ushort)record.SubRecords[i++].value.uintValue;
                                    natc.atin = record.SubRecords[i++].value.uintValue;
                                    natc.atvl = record.SubRecords[i].value.stringValue;
                                    fasc.natc.Add(natc);
                                }
                                ftr.fasc.Add(fasc);
                                break;
                            case FeatureTypeRecord.FTR_Type.THAS:
                                for (int i = 0; i < record.SubRecords.Count; i++)
                                {
                                    FeatureTypeRecord.THAS thas = new FeatureTypeRecord.THAS();
                                    thas.rrnm = record.SubRecords[i++].value.uintValue;
                                    thas.rrid = record.SubRecords[i++].value.uintValue;
                                    thas.taui = record.SubRecords[i].value.uintValue;
                                    ftr.thas.Add(thas);
                                }
                                break;
                            case FeatureTypeRecord.FTR_Type.MASK:
                                for (int i = 0; i < record.SubRecords.Count; i++)
                                {
                                    FeatureTypeRecord.MASK mask = new FeatureTypeRecord.MASK();
                                    mask.rrnm = record.SubRecords[i++].value.uintValue;
                                    mask.rrid = record.SubRecords[i++].value.uintValue;
                                    mask.mind = record.SubRecords[i++].value.uintValue;
                                    mask.muin = record.SubRecords[i].value.uintValue;
                                    ftr.mask.Add(mask);
                                }
                                break;
                            default:
                                break;
                        }

                    }
                    s101.ftr.Add(ftr);
                    #endregion
                }
            }
            return s101;
        }
 
    
    }
}
