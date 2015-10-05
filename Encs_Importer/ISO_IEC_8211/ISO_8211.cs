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
using Encs_Importer.Factory;
using Encs_Importer.Common;
using System.Collections;

namespace Encs_Importer.ISO_IEC_8211
{ 
    public class ISO_8211 // 원본데이터 파싱 클래스
    {
        private byte[] fullData; // byte[] 타입 원본데이터 변수 
        private List<ISO_8211_DataRecordBase> recordList; // 레코드 목록 변수

        //0x1F field terminator (FT) ▼ 31
        //0x1E Unit terminator (UT) ▲ 30
        //0x3B fieldControl terminator (FCT) ; 59
        //0x26 DataField Name terminator (DFNT) & 38
        private enum DELIMITERS { FT = 0x1F, UT = 0x1E, FCT = 0x3B, DFNT = 0x26 } // 구분자
       
        public ISO_8211() // 생성자(초기화) 함수 
        {
            recordList = new List<ISO_8211_DataRecordBase>();
        }

        internal List<List<Common_Record>> Load_ISO_IEC_8211(byte[] fullData) // 원본데이터를 레코드 목록 형식으로 변환 함수
        {
            recordList.Clear();
            this.fullData = fullData;
            int pos = 0;
            while (pos < fullData.Length)
            {
                ISO_8211_DataRecordBase record = LoadRecord(pos);
                pos += record.leader.rl;
                recordList.Add(record);
            }

            List<List<Common_Record>> builderRecordList = new List<List<Common_Record>>();

            for (int i = 1; i < recordList.Count; i++)
            {
                List<Common_Record> RecordList = new List<Common_Record>();
                ISO_8211_DataRecord dataRecord = (ISO_8211_DataRecord)recordList[i];
                foreach (var item in dataRecord.records)
                {
                    Common_Record record = new Common_Record();
                    record.tag = item.tag;
                    foreach (var subItem in item.subRecords)
                    {
                        Common_SubRecord sub = new Common_SubRecord();
                        sub.tag = subItem.tag;
                        System.Type type = ReadDataOfDataFormat(subItem.type);
                        sub.value = new Common_SubRecord.ValueType(type);

                        if (type == typeof(string))
                        {
                            string value = System.Text.Encoding.ASCII.GetString(subItem.value);
                            sub.value.SetValue(value);
                        }
                        else if (type == typeof(uint))
                        {
                            byte[] temp = new byte[4];
                            for (int j = 0; j < subItem.value.Length; j++)
                            {
                                temp[j] = Convert.ToByte(subItem.value[j]);
                            }
                            uint value = BitConverter.ToUInt32(temp, 0);
                            sub.value.SetValue(value);

                        }
                        else if (type == typeof(int))
                        {
                            byte[] temp = new byte[4];
                            for (int j = 0; j < subItem.value.Length; j++)
                            {
                                temp[j] = Convert.ToByte(subItem.value[j]);
                            }
                            int value = BitConverter.ToInt32(temp, 0);
                            sub.value.SetValue(value);
                        }
                        else if (type == typeof(double))
                        {
                            byte[] temp = new byte[8];
                            for (int j = 0; j < subItem.value.Length; j++)
                            {
                                temp[j] = Convert.ToByte(subItem.value[j]);
                            }
                            double value = BitConverter.ToDouble(temp, 0);
                            sub.value.SetValue(value);
                        }

                        record.SubRecords.Add(sub);
                    }
                    RecordList.Add(record);
                }
                builderRecordList.Add(RecordList);
            }
            return builderRecordList;
        }

        private Type ReadDataOfDataFormat(ISO_8211_Record_DataDesc.FORMAT_CONTROL fORMAT_CONTROL) //ISO8211 레코드 데이터 포맷 처리 함수
        {
            switch (fORMAT_CONTROL)
            {
                case ISO_8211_Record_DataDesc.FORMAT_CONTROL.A:
                    return typeof(string);
                case ISO_8211_Record_DataDesc.FORMAT_CONTROL.I:
                    break;
                case ISO_8211_Record_DataDesc.FORMAT_CONTROL.R:
                    break;
                case ISO_8211_Record_DataDesc.FORMAT_CONTROL.B:
                    break;
                case ISO_8211_Record_DataDesc.FORMAT_CONTROL.b1:
                    return typeof(uint);
                case ISO_8211_Record_DataDesc.FORMAT_CONTROL.b2:
                    return typeof(int);
                case ISO_8211_Record_DataDesc.FORMAT_CONTROL.b4:
                    return typeof(double);
                default:
                    break;
            }
            return null;
        }

        private ISO_8211_DataRecordBase LoadRecord(int fullPos) 
        {
            ISO_8211_DataRecordBase record = null;
            ISO_8211_Leader leader = Load_Leader(fullPos);

            if (leader.LeaderIdentifier == 'L')
            {
                record = new ISO_8211_DataRecordHeader();
                record.leader = leader;
                record.directoryList = Load_Directory(record.leader, fullPos);
                ((ISO_8211_DataRecordHeader)record).ddrList = Load_DataDescriptionRecord(((ISO_8211_DataRecordHeader)record).leader, ((ISO_8211_DataRecordHeader)record).directoryList, fullPos);
            }
            else if (leader.LeaderIdentifier == 'D' || leader.LeaderIdentifier == 'R')
            {
                record = new ISO_8211_DataRecord();
                record.leader = leader;
                record.directoryList = Load_Directory(record.leader, fullPos);
                Load_DataRecord(((ISO_8211_DataRecord)record), fullPos);
            }

            return record;
        }

        private ISO_8211_Leader Load_Leader(int fullPos) // Load Leader
        {
            ISO_8211_Leader leader = new ISO_8211_Leader();
            char[] buffLeader = new char[leader.SIZE];
            Array.Copy(fullData, fullPos, buffLeader, 0, leader.SIZE);
            leader.setValue(buffLeader);
            return leader;
        }

        private List<ISO_8211_Directory> Load_Directory(ISO_8211_Leader leader, int fullPos) //Load Directory
        {
            List<ISO_8211_Directory> dirList = new List<ISO_8211_Directory>();
            int directory_size = leader.bafa - leader.SIZE;
            char[] directoryBuff = new char[directory_size];
            Array.Copy(fullData, leader.SIZE + fullPos, directoryBuff, 0, directory_size);

            int field_length_size = leader.sizeOfFieldLengthField - 48;
            char[] field_size_buff = new char[field_length_size];

            int pos_size = leader.sizeOfFieldPositionField - 48;
            char[] pos_size_buff = new char[pos_size];

            int pos = 0;
            while (directoryBuff[pos] != (int)DELIMITERS.UT)
            {
                ISO_8211_Directory directory = new ISO_8211_Directory();
                Array.Copy(directoryBuff, pos, directory.Tag, 0, 4); // The size of field tag field is always 4 in S-57. S-101? 
                pos += 4;

                //Console.Write("Directory Tag name = " + new string(directory.Tag));

                Array.Copy(directoryBuff, pos, field_size_buff, 0, field_length_size);
                directory.Length = Utilities.charArray_to_int32(field_size_buff, field_size_buff.Length);
                pos += field_length_size;
                //Console.Write(" Directory field length = " + directory.Length);

                Array.Copy(directoryBuff, pos, pos_size_buff, 0, pos_size);
                directory.Position = Utilities.charArray_to_int32(pos_size_buff, pos_size_buff.Length);
                pos += pos_size;
                //Console.WriteLine(" Directory position = " + directory.Position);
                dirList.Add(directory);
            }
        
            return dirList;
        }

        private Dictionary<string, ISO_8211_Record_DataDesc> Load_DataDescriptionRecord(ISO_8211_Leader leader, List<ISO_8211_Directory> dirList, int fullPos) //Load DataDescriptions Record
        {
            Dictionary<string, ISO_8211_Record_DataDesc> descList = new Dictionary<string, ISO_8211_Record_DataDesc>();
            int descPos = leader.bafa+fullPos;
            ISO_8211_Directory entry;
            int sp = 0;
            int ep = 0;

            for (int i = 0; i < dirList.Count; i++)
            {
                entry = dirList[i];
                sp = descPos;
                ep = descPos;

                ISO_8211_Record_DataDesc desc = new ISO_8211_Record_DataDesc();

                //Field controls
                while (fullData[ep++] != (int)DELIMITERS.FCT) ;
                char[] fieldControlBuff = new char[ep - sp];
                Array.Copy(fullData, sp, fieldControlBuff, 0, ep - sp);
                desc.field_control = fieldControlBuff;
                sp = ep;
                //Console.WriteLine("Field Controls : " + new string(fieldControlBuff));

                //DataField name
                while (fullData[ep++] != (int)DELIMITERS.FT) ;
                char[] dataFieldNameBuff = new char[ep - sp];
                Array.Copy(fullData, sp, dataFieldNameBuff, 0, ep - sp);
                desc.field_name = new string(dataFieldNameBuff);
                sp = ep;
                //Console.WriteLine("DataField name : " + new string(dataFieldNameBuff));

                //Array descriptor
                while (fullData[ep++] != (int)DELIMITERS.UT) ;
                char[] arrayDescStrBuff = new char[ep - sp];
                Array.Copy(fullData, sp, arrayDescStrBuff, 0, ep - sp);
                if (arrayDescStrBuff.Contains((char)DELIMITERS.FT))
                {
                    int arrayDescIndex = 0;
                    while (arrayDescStrBuff[arrayDescIndex++] != (char)DELIMITERS.FT) ;
                    char[] arrayDescBuff = new char[arrayDescIndex];
                    Array.Copy(arrayDescStrBuff, 0, arrayDescBuff, 0, arrayDescIndex);
                    desc.array_desc_str = new string(arrayDescBuff);
                    desc.array_descriptions = SplitArrayDescStr(desc.array_desc_str,leader.sizeOfFieldTagField-48);
                    //Console.WriteLine("Array descriptor : " + desc.array_desc_str);

                    int typeIndex = arrayDescIndex;
                    while (arrayDescStrBuff[typeIndex++] != (char)DELIMITERS.UT) ;
                    char[] fmfBuff = new char[typeIndex - arrayDescIndex];
                    Array.Copy(arrayDescStrBuff, arrayDescIndex, fmfBuff, 0, typeIndex - arrayDescIndex);
                    desc.format_control_str = new string(fmfBuff);
                    desc.format_controls = SplitFormatControlStr(desc.format_control_str);
                    //Console.WriteLine("Format Control : " + desc.format_control_str);
                }
                else
                {
                    desc.array_desc_str = new string(arrayDescStrBuff);
                    desc.array_descriptions = SplitArrayDescStr(desc.array_desc_str, leader.sizeOfFieldTagField - 48);
                    //Console.WriteLine("Array descriptor : " + desc.array_desc_str.ToString());
                }
                descList.Add(new string(dirList[i].Tag),desc);
                descPos = descPos + entry.Length;
                //Console.WriteLine();
            }
            return descList;
        }

        private void Load_DataRecord(ISO_8211_DataRecord dataRecord, int fullPos) //Load Data Record
        {
            ISO_8211_Leader leader = dataRecord.leader;
            List<ISO_8211_Directory> dirList = dataRecord.directoryList;
            Dictionary<string, ISO_8211_Record_DataDesc> ddrList = ((ISO_8211_DataRecordHeader)recordList[0]).ddrList;
            dataRecord.records = new List<ISO_8211_Record_Field>();
            byte[] recordBuff = new byte[leader.rl - leader.bafa];
            Array.Copy(fullData, leader.bafa + fullPos, recordBuff, 0, leader.rl - leader.bafa);
     
            int unitPos = 0;
            for (int i = 0; i < dirList.Count; i++)
            {
                string recordTag = new string(dirList[i].Tag);
                ISO_8211_Record_DataDesc ddr = ddrList[recordTag];

                ISO_8211_Record_Field record = new ISO_8211_Record_Field();
                record.tag = recordTag;

                int fieldLength = dirList[i].Length;
                byte[] unitBuff = new byte[fieldLength];
                Array.Copy(recordBuff, unitPos, unitBuff, 0, fieldLength);
                unitPos += fieldLength;

                int fieldPos = 0;
                if (ddr.array_desc_str[0].Equals('*'))
                {
                    while (unitBuff[fieldPos] != (int)DELIMITERS.UT)
                    {
                        for (int k = 0; k < ddr.array_descriptions.Count; k++)
                        {
                            string tag = ddr.array_descriptions[k];
                            ISO_8211_Record_DataDesc.FormatControl fc = ddr.format_controls[k];
                            ISO_8211_Record_SubField subRecord = new ISO_8211_Record_SubField();
                            fieldPos = subRecordBinding(fc, unitBuff, fieldPos, subRecord);
                            subRecord.tag = tag;
                            record.subRecords.Add(subRecord);
                        }
                    }
                }
                else
                {
                    
                    for (int z = 0; z < ddr.array_descriptions.Count; z++)
                    {
                        string tag = ddr.array_descriptions[z];
                        ISO_8211_Record_DataDesc.FormatControl fc = ddr.format_controls[z];
                        int length = fc.length;

                        if (tag[0].Equals('\\') && tag[1].Equals('*'))
                        {
                            string[] tags = tag.Substring(2).Split('!');
                            while (unitBuff[fieldPos] != (int)DELIMITERS.UT)
                            {
                                for (int k = 0; k < tags.Length; k++)
                                {
                                    string nextTag = tags[k];
                                    ISO_8211_Record_DataDesc.FormatControl nextFc = ddr.format_controls[z+k];
                                    ISO_8211_Record_SubField subRecord = new ISO_8211_Record_SubField();
                                    fieldPos = subRecordBinding(nextFc, unitBuff, fieldPos, subRecord);
                                    subRecord.tag = nextTag;
                                    record.subRecords.Add(subRecord);
                                }
                            }
                        }
                        else
                        {
                            ISO_8211_Record_SubField subRecord = new ISO_8211_Record_SubField();
                            fieldPos = subRecordBinding(fc, unitBuff, fieldPos, subRecord);
                            subRecord.tag = tag;

                            record.subRecords.Add(subRecord);
                        }

                    }
                }
                dataRecord.records.Add(record);
            }
            
        }

        private int subRecordBinding(ISO_8211_Record_DataDesc.FormatControl fc, byte[] unitBuff, int fieldPos,ISO_8211_Record_SubField subRecord)
        {
            int length = fc.length;
            byte[] buff;
            switch (fc.type)
            {
                case ISO_8211_Record_DataDesc.FORMAT_CONTROL.A:
                    StringBuilder sb = new StringBuilder();
                    char ch;
                    if (length == 0)
                    {
                        while ((ch =Convert.ToChar(unitBuff[fieldPos++])) != (char)DELIMITERS.FT)
                            sb.Append(ch);
                    }
                    else
                    {
                        for (int t = 0; t < length; t++)
                        {
                            ch = Convert.ToChar(unitBuff[fieldPos++]);
                            sb.Append(ch);
                        }
                    }
                    subRecord.type = ISO_8211_Record_DataDesc.FORMAT_CONTROL.A;
                    subRecord.value = Encoding.ASCII.GetBytes(sb.ToString());
                    break;
                case ISO_8211_Record_DataDesc.FORMAT_CONTROL.I:
                    break;
                case ISO_8211_Record_DataDesc.FORMAT_CONTROL.R:
                    break;
                case ISO_8211_Record_DataDesc.FORMAT_CONTROL.B:
                    break;
                case ISO_8211_Record_DataDesc.FORMAT_CONTROL.b1:
                    buff = new byte[length];
                    Array.Copy(unitBuff, fieldPos, buff, 0, length);
                    fieldPos += length;
                    subRecord.type = ISO_8211_Record_DataDesc.FORMAT_CONTROL.b1;
                    subRecord.value = buff;
                    break;
                case ISO_8211_Record_DataDesc.FORMAT_CONTROL.b2:
                    buff = new byte[length];
                    Array.Copy(unitBuff, fieldPos, buff, 0, length);
                    fieldPos += length;
                    subRecord.type = ISO_8211_Record_DataDesc.FORMAT_CONTROL.b2;
                    subRecord.value = buff;
                    break;
                case ISO_8211_Record_DataDesc.FORMAT_CONTROL.b4:
                    buff = new byte[length];
                    Array.Copy(unitBuff, fieldPos, buff, 0, length);
                    fieldPos += length;
                    subRecord.type = ISO_8211_Record_DataDesc.FORMAT_CONTROL.b4;
                    subRecord.value = buff;
                    break;
                default:
                    break;
            }
            return fieldPos;
        }

        private List<string> SplitArrayDescStr(string str,int splitSize)
        {
            List<string> descriptions = new List<string>();
            StringBuilder sb = new StringBuilder();
            foreach (var item in str)
            {
                if (item == (int)DELIMITERS.FT)
                {
                    descriptions.Add(sb.ToString());
                    sb.Clear();
                }
                if (sb.Length > splitSize)
                {
                    if (sb.ToString()[0].Equals('\\'))
                    {
                        string rTag = sb.ToString().Substring(sb.ToString().IndexOf('*')+1);
                        sb.Clear();
                        sb.Append("\\*" + rTag+item);
                        continue;
                    }
                    if (sb.ToString()[0].Equals('*'))
                    {
                        descriptions.Add(sb.ToString().Substring(1));
                        sb.Clear();
                        
                        continue;
                    }
                    descriptions.Add(sb.ToString().Substring(0, 4));
                    string temp = sb.ToString().Substring(4);
                    sb.Clear();
                    sb.Append(temp);
                }

                if (!item.Equals('!'))
                {
                    sb.Append(item);
                    continue;
                }
               
                descriptions.Add(sb.ToString());
                sb.Clear();
                                

            }
            return descriptions;
        }

        private List<ISO_8211_Record_DataDesc.FormatControl> SplitFormatControlStr(string str)
        {
            List<ISO_8211_Record_DataDesc.FormatControl> formatControls = new List<ISO_8211_Record_DataDesc.FormatControl>();
            string[] items = str.Substring(1, str.Length - 3).Split(',');

            foreach (var item in items)
            {
                ISO_8211_Record_DataDesc.FormatControl fc;
                if (item[0] <= '9' && item[0] >= '0')
                {
                    int index = 0;
                    while (item[index] >= '0' && item[index] <= '9') index++;
                    int repetition = Convert.ToInt32(item.Substring(0, index));

                    fc = Parse_format_string(item.Substring(index));
                    for (int i = 0; i < repetition; i++)
                    {
                        formatControls.Add(fc);
                    }
                }
                else 
                {
                    fc = Parse_format_string(item);
                    formatControls.Add(fc);
                }
            }
            return formatControls;

        }

        private ISO_8211_Record_DataDesc.FormatControl Parse_format_string(string str)
        {
            ISO_8211_Record_DataDesc.FormatControl fc= new ISO_8211_Record_DataDesc.FormatControl();
            char type = str[0];

            if (type == 'b')
            {
                char ch2 = str[1];
                char ch3 = str[2];
                if (ch2 == '1')
                    fc.type = ISO_8211_Record_DataDesc.FORMAT_CONTROL.b1;
                else if (ch2 == '2')
                    fc.type = ISO_8211_Record_DataDesc.FORMAT_CONTROL.b2;
                else if (ch2 == '4')
                    fc.type = ISO_8211_Record_DataDesc.FORMAT_CONTROL.b4;
                else
                    Console.WriteLine("problem in parse_format_string");

                fc.length = ch3 - 48;
                return fc;
            }

            if (type == 'A')
                fc.type = ISO_8211_Record_DataDesc.FORMAT_CONTROL.A;
            else if (type == 'I')
                fc.type = ISO_8211_Record_DataDesc.FORMAT_CONTROL.I;
            else if (type == 'R')
                fc.type = ISO_8211_Record_DataDesc.FORMAT_CONTROL.R;
            else if (type == 'B')
                fc.type = ISO_8211_Record_DataDesc.FORMAT_CONTROL.B;

            if (str.Length > 1)
            {
                fc.length = Convert.ToInt32(str.Substring(str.LastIndexOf('(')+1, str.LastIndexOf(')')-2));
                if (type == 'B')
                    fc.length /= 8;
            }
            else
                fc.length =0;
            return fc;
        }

        private int charArray_to_int32(char[] array, int size)
        {
            if (size > 32)
                return 0;

            int baseValue = 1;
            int result = 0;
            for (int i = size; i > 0; i--)
            {
                int code = array[i - 1];
                result += (code - 48) * baseValue;
                baseValue *= 10;
            }
            return result;
        }
    }

}
