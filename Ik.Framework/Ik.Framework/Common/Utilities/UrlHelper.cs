using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.Common.Utilities
{
    public class UrlHelper
    {
        private static readonly string[] _allowed_second_domain = new string[] { "com", "net", "org", "gov" };

        public static string BuildUrl(string src1, string src2, params string[] targets)
        {
            var newArr = new List<string>();
            newArr.Add(src1);
            newArr.Add(src2);
            newArr.AddRange(targets);
            newArr.RemoveAll(d => string.IsNullOrWhiteSpace(d));
            if (newArr.Count <= 1)
                return newArr.FirstOrDefault();

            var builder = new StringBuilder();
            builder.Append(src1);
            for (var i = 1; i < newArr.Count; i++)
            {
                var firstEndsWithSlash = newArr[i - 1].EndsWith("/");
                var secondStartsWithSlash = newArr[i].StartsWith("/");
                if (firstEndsWithSlash && secondStartsWithSlash)
                {
                    builder.Append(newArr[i].TrimStart('/'));
                }
                else if (!firstEndsWithSlash && !secondStartsWithSlash)
                {
                    builder.Append('/');
                    builder.Append(newArr[i]);
                }
                else
                {
                    builder.Append(newArr[i]);
                }
            }
            return builder.ToString();
        }

        public static string GetContentType(string fileName)
        {
            string contentType = "application/octet-stream";

            if (Path.HasExtension(fileName))
            {
                var surfix = Path.GetExtension(fileName).ToUpper();

                if (string.IsNullOrWhiteSpace(surfix))
                    return contentType;

                if (surfix == ".323")
                    contentType = "text/h323";
                else if (surfix == ".ACX")
                    contentType = "application/internet-property-stream";
                else if (surfix == ".AI")
                    contentType = "application/postscript";
                else if (surfix == ".AIF")
                    contentType = "audio/x-aiff";
                else if (surfix == ".AIFC")
                    contentType = "audio/x-aiff";
                else if (surfix == ".AIFF")
                    contentType = "audio/x-aiff";
                else if (surfix == ".ASF")
                    contentType = "video/x-ms-asf";
                else if (surfix == ".SR")
                    contentType = "video/x-ms-asf";
                else if (surfix == ".SX")
                    contentType = "video/x-ms-asf";
                else if (surfix == ".AU")
                    contentType = "audio/basic";
                else if (surfix == ".AVI")
                    contentType = "video/x-msvideo";
                else if (surfix == ".AXS")
                    contentType = "application/olescript";
                else if (surfix == ".BAS")
                    contentType = "text/plain";
                else if (surfix == ".BCPIO")
                    contentType = "application/x-bcpio";
                else if (surfix == ".BIN")
                    contentType = "application/octet-stream";
                else if (surfix == ".BMP")
                    contentType = "image/bmp";
                else if (surfix == ".C")
                    contentType = "text/plain";
                else if (surfix == ".CAT")
                    contentType = "application/vnd.ms-pkiseccat";
                else if (surfix == ".CDF")
                    contentType = "application/x-cdf";
                else if (surfix == ".CER")
                    contentType = "application/x-x509-ca-cert";
                else if (surfix == ".CLASS")
                    contentType = "application/octet-stream";
                else if (surfix == ".CLP")
                    contentType = "application/x-msclip";
                else if (surfix == ".CMX")
                    contentType = "image/x-cmx";
                else if (surfix == ".COD")
                    contentType = "image/cis-cod";
                else if (surfix == ".CPIO")
                    contentType = "application/x-cpio";
                else if (surfix == ".CRD")
                    contentType = "application/x-mscardfile";
                else if (surfix == ".CRL")
                    contentType = "application/pkix-crl";
                else if (surfix == ".CRT")
                    contentType = "application/x-x509-ca-cert";
                else if (surfix == ".CSH")
                    contentType = "application/x-csh";
                else if (surfix == ".CSS")
                    contentType = "text/css";
                else if (surfix == ".DCR")
                    contentType = "application/x-director";
                else if (surfix == ".DER")
                    contentType = "application/x-x509-ca-cert";
                else if (surfix == ".DIR")
                    contentType = "application/x-director";
                else if (surfix == ".DLL")
                    contentType = "application/x-msdownload";
                else if (surfix == ".DMS")
                    contentType = "application/octet-stream";
                else if (surfix == ".DOC")
                    contentType = "application/msword";
                else if (surfix == ".DOT")
                    contentType = "application/msword";
                else if (surfix == ".DVI")
                    contentType = "application/x-dvi";
                else if (surfix == ".DXR")
                    contentType = "application/x-director";
                else if (surfix == ".EPS")
                    contentType = "application/postscript";
                else if (surfix == ".ETX")
                    contentType = "text/x-setext";
                else if (surfix == ".EVY")
                    contentType = "application/envoy";
                else if (surfix == ".EXE")
                    contentType = "application/octet-stream";
                else if (surfix == ".FIF")
                    contentType = "application/fractals";
                else if (surfix == ".FLR")
                    contentType = "x-world/x-vrml";
                else if (surfix == ".GIF")
                    contentType = "image/gif";
                else if (surfix == ".GTAR")
                    contentType = "application/x-gtar";
                else if (surfix == ".GZ")
                    contentType = "application/x-gzip";
                else if (surfix == ".H")
                    contentType = "text/plain";
                else if (surfix == ".HDF")
                    contentType = "application/x-hdf";
                else if (surfix == ".HLP")
                    contentType = "application/winhlp";
                else if (surfix == ".HQX")
                    contentType = "application/mac-binhex40";
                else if (surfix == ".HTA")
                    contentType = "application/hta";
                else if (surfix == ".HTC")
                    contentType = "text/x-component";
                else if (surfix == ".HTM")
                    contentType = "text/html";
                else if (surfix == ".HTML")
                    contentType = "text/html";
                else if (surfix == ".HTT")
                    contentType = "text/webviewhtml";
                else if (surfix == ".ICO")
                    contentType = "image/x-icon";
                else if (surfix == ".IEF")
                    contentType = "image/ief";
                else if (surfix == ".III")
                    contentType = "application/x-iphone";
                else if (surfix == ".INS")
                    contentType = "application/x-internet-signup";
                else if (surfix == ".ISP")
                    contentType = "application/x-internet-signup";
                else if (surfix == ".JFIF")
                    contentType = "image/pipeg";
                else if (surfix == ".JPG")
                    contentType = "image/jpeg";
                else if (surfix == ".JPEG")
                    contentType = "image/jpeg";
                else if (surfix == ".PNG")
                    contentType = "image/png";
            }
            return contentType;
        }

        public static string GetImageSurfix(string contentType)
        {
            if (!string.IsNullOrWhiteSpace(contentType))
            {
                contentType = contentType.ToLower();
                switch (contentType)
                {
                    case "image/bmp": return ".bmp";
                    case "image/gif": return ".gif";
                    case "image/jpeg": return ".jpg";
                    case "image/jpg": return ".jpg";
                    case "image/png": return ".png";
                    default: return string.Empty;
                }
            }
            else
                return string.Empty;
        }

        public static string GetRootDomain(Uri uri)
        {
            string rootDomain;
            switch (uri.HostNameType)
            {
                case UriHostNameType.Dns:
                    {
                        if (uri.IsLoopback)
                        {
                            rootDomain = uri.Host;
                        }
                        else
                        {
                            string host = uri.Host.ToLower();
                            var hosts = host.Split('.');
                            var len = hosts.Length;

                            var section2 = hosts[len - 2];
                            if (_allowed_second_domain.Any(d => d == section2))
                            {
                                rootDomain = string.Format("{0}.{1}.{2}", hosts[len - 3], hosts[len - 2], hosts[len - 1]);
                            }
                            else
                            {
                                rootDomain = string.Format("{0}.{1}", hosts[len - 2], hosts[len - 1]);
                            }
                        }
                    }
                    break;
                default:
                    rootDomain = uri.Host;
                    break;
            }
            return rootDomain;
        }
    }
}

#region copyright
/*
*.NET基础开发框架
*Copyright (C) 。。。
*地址：git@github.com:gangzaicd/Ik.Framework.git
*作者：到大叔碗里来（大叔）
*QQ：397754531
*eMail：gangzaicd@163.com
*/
#endregion copyright
