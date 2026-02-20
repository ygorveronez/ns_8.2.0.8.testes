namespace AdminMultisoftware.Dominio.Enumeradores
{
    public enum OrigemAuditadoAdmin
    {
        Sistema = 0
    }

    public static class OrigemAuditadoAdminHelper
    {
        public static string ObterDescricao(this OrigemAuditadoAdmin origem)
        {
            switch (origem)
            {
                case OrigemAuditadoAdmin.Sistema: return "Sistema";
                default: return string.Empty;
            }
        }
    }
}