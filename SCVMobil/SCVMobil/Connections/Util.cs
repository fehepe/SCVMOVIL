using System;
using System.Collections.Generic;
using System.Text;

namespace SCVMobil
{
    class Util
    {
        public static object Coalesce(object inObj, object outObj)
        {
            if(inObj == null)
            {
                return outObj;
            }
            else
            {
                return inObj;
            }
        }

        public static object CoalesceStr(object inObj, object outObj)
        {
            if (inObj == null)
            {
                return outObj.ToString();
            }
            else
            {
                return inObj.ToString();
            }
        }
    }
}
