using Dominio.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal
{
    public sealed class Empresa
    {
        public int Codigo { get; set; }
        public string CNPJ { get; set; }
        public int CodigoIBGE { get; set; }
        public string NomeCertificado { get; set; }
        public string SenhaCertificado { get; set; }
        public string NomeCertificadoKeyVault { get; set; }
        public TipoAmbiente TipoAmbiente { get; set; }
    }
}
