namespace EmissaoCTe.API.Models.BI
{
    public class EmbedParms : EmbedConfig
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ApplicationId { get; set; }
        public string ReportID { get; set; }
        public string WorkspaceId { get; set; }
    }
}