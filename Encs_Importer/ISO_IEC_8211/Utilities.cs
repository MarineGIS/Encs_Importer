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
    class Utilities
    {
        public static int charArray_to_int32(char[] array, int size)
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
