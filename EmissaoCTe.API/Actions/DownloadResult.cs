using System.Web.Mvc;

namespace EmissaoCTe.API
{
    public class DownloadResult : ActionResult
    {
        private string _Path;
        private string _FileName;

        public DownloadResult() { }

        public DownloadResult(string filePath)
        {
            this.FilePath = filePath;
        }

        public DownloadResult(string filePath, string fileName)
        {
            this.FilePath = filePath;
            this.FileName = fileName;
        }

        public string FilePath
        {
            get
            {
                return _Path;
            }
            set
            {
                _Path = value;
            }
        }

        public string FileName
        {
            get
            {
                return _FileName;
            }
            set
            {
                _FileName = value;
            }
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (!string.IsNullOrEmpty(_FileName))
                context.HttpContext.Response.AddHeader("content-disposition", string.Concat("attachment; filename=", FileName));
            context.HttpContext.Response.TransmitFile(_Path);
        }
    }
}