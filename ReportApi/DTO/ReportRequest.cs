namespace ReportApi.DTO;





public enum FileType
{
    PDF = 0,
    CSV = 1,
    EXCEL = 2
}




public class RequestInformation
{
    public int CodigoEmpresa { get; set; }
    public int CodigoUsuario { get; set; }
    
    public AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServico { get; set; }
}