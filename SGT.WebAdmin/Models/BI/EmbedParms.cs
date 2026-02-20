namespace SGT.WebAdmin.Models.BI
{
    public class EmbedParms : EmbedConfig
    {

        public string UserName { get; set; }
        public string Password { get; set; }
        public string ApplicationId { get; set; }
        public Guid? ReportID { get; set; }
        public Guid WorkspaceId { get; set; }
        public int CodigoFormulario { get; set; }
        public string TokenAutentication { get; internal set; }
    }
}