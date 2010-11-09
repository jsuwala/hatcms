using System;
using System.IO;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;

namespace HatCMS
{
    /// <summary>
    /// A dependecy that requires that a file exists on the file system.
    /// </summary>
    public class CmsFileDependency : CmsDependency
    {
        private string FullFilePath = "";


        /// <summary>
        /// set to DateTime.MinValue if no Last Modified date check is done.
        /// </summary>
        private DateTime FileShouldBeLastModifiedAfter = DateTime.MinValue;

        public CmsFileDependency(string fullfilepath)
        {
            FullFilePath = fullfilepath;
            FileShouldBeLastModifiedAfter = DateTime.MinValue;
        }

        public static CmsFileDependency UnderAppPath(string pathUnderAppPath)
        {
            string fullFilePath = System.Web.HttpContext.Current.Server.MapPath(CmsContext.ApplicationPath + pathUnderAppPath);
            return new CmsFileDependency(fullFilePath);
        }

        public static CmsFileDependency UnderAppPath(string pathUnderAppPath, DateTime modifiedAfter)
        {
            string fullFilePath = System.Web.HttpContext.Current.Server.MapPath(CmsContext.ApplicationPath + pathUnderAppPath);
            return new CmsFileDependency(fullFilePath, modifiedAfter);
        }

        public CmsFileDependency(string fullfilepath, DateTime fileshouldbelastmodifiedafter)
        {
            FullFilePath = fullfilepath;
            FileShouldBeLastModifiedAfter = fileshouldbelastmodifiedafter;
        }

        public override string GetContentHash()
        {
            return FullFilePath.Trim().ToLower();
        }

        public override CmsDependencyMessage[] ValidateDependency()
        {
            List<CmsDependencyMessage> ret = new List<CmsDependencyMessage>();
            try
            {
                if (File.Exists(FullFilePath))
                {
                    if (FileShouldBeLastModifiedAfter.Ticks != DateTime.MinValue.Ticks)
                    {
                        FileInfo fi = new FileInfo(FullFilePath);
                        if (fi.LastWriteTime.Ticks < FileShouldBeLastModifiedAfter.Ticks)
                            ret.Add(CmsDependencyMessage.Error("\""+FullFilePath+"\" should have a last modified date after "+FileShouldBeLastModifiedAfter.ToString("d MMM yyyy hh:mm")));
                    }
                }
                else
                {
                    ret.Add(CmsDependencyMessage.Error("File does not exist: \""+FullFilePath+"\""));
                }
            }
            catch(Exception ex)
            {
                ret.Add(CmsDependencyMessage.Error(ex));
            }
            return ret.ToArray();
        }
    }
}