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

namespace Encs_Importer.ISO_IEC_8211
{
    public class ISO_8211_Leader
    {
        internal char[] recordLength; //5char / Integer value name: rl
        internal char InterchangeLevel;
        internal char LeaderIdentifier;
        internal char InlineCodeExtendsionIndicator;
        internal char versionNumber;
        internal char applicationIndicator;
        internal char[] FieldControlLength; //2char / Integer value name : fcl
        internal char[] baseAddressOfFieldArea; //5char / Integer value name : bafa
        internal char[] extendedCharacterSetIndicator; //3char / Integer value name : ecsi
        internal char sizeOfFieldLengthField;
        internal char sizeOfFieldPositionField;
        internal char reservedForFutureStandardization;
        internal char sizeOfFieldTagField;

        internal int rl;
        internal int fcl;
        internal int bafa;
        internal string ecsi;
        const int leaderSize = 24;
        public ISO_8211_Leader()
        {
            // TODO: Complete member initialization
            this.recordLength = new char[5];
            this.FieldControlLength = new char[2];
            this.baseAddressOfFieldArea = new char[5];
            this.extendedCharacterSetIndicator = new char[3];
        }
        public int SIZE
        {
            get { return leaderSize; }
        }     
        public override string ToString()
        {
            string LeaderData = "LeaderData \nreacordLength = " + rl + "\nInterchangeLevel = " + InterchangeLevel + "\nLeaderIdentifier = " + LeaderIdentifier + "\nInlineCodeExtendsionIndicator = " + InlineCodeExtendsionIndicator
               + "\nversionNumber = " + versionNumber + "\napplicationIndicator = " + applicationIndicator + "\nfieldCOntrolLength = " + fcl + "\nbaseAddressOfFieldArea = " + bafa
               + "\nextendedCharacterSetIndicator = " + ecsi + "\nsizeOfFieldLengthField = " + sizeOfFieldLengthField + "\nsizeOfFieldPositionField = " + sizeOfFieldPositionField
               + "\nreservedForFutureStandardization = " + reservedForFutureStandardization + "\nsizeOfFieldTagField = " + sizeOfFieldTagField;
            return LeaderData;
        }
        public void setValue(char[] leaderBuffData)
        {
            int position = 0;
            Array.Copy(leaderBuffData,position, this.recordLength,0, this.recordLength.Length); position += this.recordLength.Length;
            this.rl = Utilities.charArray_to_int32(this.recordLength,this.recordLength.Length);

            this.InterchangeLevel = leaderBuffData[position++];
            this.LeaderIdentifier = leaderBuffData[position++];
            this.InlineCodeExtendsionIndicator = leaderBuffData[position++];
            this.versionNumber = leaderBuffData[position++];
            this.applicationIndicator = leaderBuffData[position++];

            Array.Copy(leaderBuffData,position, this.FieldControlLength,0,this.FieldControlLength.Length); position += this.FieldControlLength.Length;
            this.fcl = Utilities.charArray_to_int32(this.FieldControlLength, this.FieldControlLength.Length);

            Array.Copy(leaderBuffData,position, this.baseAddressOfFieldArea,0, this.baseAddressOfFieldArea.Length); position += this.baseAddressOfFieldArea.Length;
            this.bafa = Utilities.charArray_to_int32(this.baseAddressOfFieldArea, this.baseAddressOfFieldArea.Length);

            Array.Copy(leaderBuffData,position, this.extendedCharacterSetIndicator,0, this.extendedCharacterSetIndicator.Length); position += this.extendedCharacterSetIndicator.Length;
            this.ecsi = new string(this.extendedCharacterSetIndicator);

            this.sizeOfFieldLengthField = leaderBuffData[position++];
            this.sizeOfFieldPositionField = leaderBuffData[position++];
            this.reservedForFutureStandardization = leaderBuffData[position++];
            this.sizeOfFieldTagField = leaderBuffData[position++];
        }

      
    }
}
