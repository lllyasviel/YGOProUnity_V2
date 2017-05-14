// Copyright (C) 2014 Stephan Bouchard - All Rights Reserved
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TMPro
{

    public static class TMPro_ExtensionMethods
    {

        public static string ArrayToString(this char[] chars)
        {
            string s = string.Empty;

            for (int i = 0; i < chars.Length && chars[i] != 0; i++)
            {
                s += chars[i];
            }

            return s;
        }


        public static int FindInstanceID <T> (this List<T> list, T target) where T : Object
        {
            int targetID = target.GetInstanceID();
            
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].GetInstanceID() == targetID)
                    return i;
            }
            return -1;
        }


        public static bool Compare(this Color32 a, Color32 b)
        {
            return a.r == b.r && a.g == b.g && a.b == b.b && a.a == b.a;
        }

		public static bool CompareRGB(this Color32 a, Color32 b)
		{
			return a.r == b.r && a.g == b.g && a.b == b.b;
		}

		public static bool Compare(this Color a, Color b)
        {
            return a.r == b.r && a.g == b.g && a.b == b.b && a.a == b.a;
        }


		public static bool CompareRGB(this Color a, Color b)
		{
			return a.r == b.r && a.g == b.g && a.b == b.b;
		}


        public static bool Compare(this Vector3 v1, Vector3 v2, int accuracy)
        {
            bool x = (int)(v1.x * accuracy) == (int)(v2.x * accuracy);
            bool y = (int)(v1.y * accuracy) == (int)(v2.y * accuracy);
            bool z = (int)(v1.z * accuracy) == (int)(v2.z * accuracy);

            return x && y && z;
        }

        public static bool Compare(this Quaternion q1, Quaternion q2, int accuracy)
        {
            bool x = (int)(q1.x * accuracy) == (int)(q2.x * accuracy);
            bool y = (int)(q1.y * accuracy) == (int)(q2.y * accuracy);
            bool z = (int)(q1.z * accuracy) == (int)(q2.z * accuracy);
            bool w = (int)(q1.w * accuracy) == (int)(q2.w * accuracy);

            return x && y && z && w;
        }
    }
}
