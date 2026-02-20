using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.NotaFiscal
{
    public class RetornoConsultaChave
    {
        public string Codigo { get; set; }
        public string Chave { get; set; }
        public string Numero { get; set; }
        public string MensagemRetorno { get; set; }
        public ClassificacaoNFe ClassificacaoNFe { get; set; }
    }
}
