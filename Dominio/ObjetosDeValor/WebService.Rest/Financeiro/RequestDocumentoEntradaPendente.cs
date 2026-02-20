
namespace Dominio.ObjetosDeValor.WebService.Rest.Financeiro
{
    public class RequestDocumentoEntradaPendente
    {
        public string CodigoTipoMovimento { get; set; }
        public int Inicio { get; set; }
        public int QuantidadeRegistros { get; set; }
    }
}
