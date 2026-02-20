using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Ocorrencia
{
    public class Ocorrencia
    {
        public string DataOcorrencia { get; set; }
        public int ProtocoloIntegracaoCarga { get; set; }
        public string ProtocoloIntegracaoPedido { get; set; }
        public int CodigoOcorrencia { get; set; }
        public int NumeroOcorrencia { get; set; }
        public int CodigoNotaFiscal { get; set; }
        public int NumeroNotaFiscal { get; set; }
        public List<int> NumerosNotasFiscais { get; set; }
        public TipoOcorrencia TipoOcorrencia { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Remetente { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Destinatario { get; set; }
        public Embarcador.Pessoas.Empresa Empresa { get; set; }
        public decimal ValorOcorrencia { get; set; }
        public string Observacao { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public int Protocolo { get; set; }
        public SituacaoOcorrencia SituacaoOcorrencia { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe> CTe { get; set; }
        public string SerieNotaFiscal { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaDocumentoParaEmissaoNFSManual> DocsParaEmissaoNFS { get; set; }
        public decimal KM { get; set; }
        public string Natureza { get; set; }
        public string GrupoOcorrencia { get; set; }
        public string Razao { get; set; }
        public int NotaFiscalDevolucao { get; set; }
        public string SolicitacaoCliente { get; set; }
    }
}
