using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Improview.Extensions
{
    public static class HasFile
    {
        public static bool HasVideoFile(this HttpPostedFileBase file)
        {
            return (file != null && file.ContentLength > 0) ? true : false;
        }
    }
}