namespace Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas;

public class ParametrosOfertasResultadoConsulta
{
    public int Codigo { get; set; }
    public string Descricao { get; set; }
    public string CodigoIntegracao { get; set; }
    public bool Ativo { get; set; }
    public string CodigosEmpresasString { get; set; }
    public string CodigosFiliaisString { get; set; }
    public string CodigosTiposCargasString { get; set; }
    public string CodigosTiposOperacoesString { get; set; }
}
