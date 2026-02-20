using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class ConsultaCarga
    {
        #region Propriedades sem Regras

        public int Codigo { get; set; }

        public string Descricao { get; set; }

        public int CodigoVeiculo { get; set; }

        public int CodigoMotorista { get; set; }

        public string CodigoCargaEmbarcador { get; set; }

        public string TipoCarga { get; set; }

        public string ModeloVeicular { get; set; }

        public int NumeroReboques { get; set; }

        public string Transportador { get; set; }

        public string Filial { get; set; }

        public string TipoOperacao { get; set; }

        public string Veiculo { get; set; }

        public string Motorista { get; set; }

        public SituacaoCarga Situacao { get; set; }

        public DateTime DataCriacaoCarga { get; set; }

        public DateTime DataCarregamento { get; set; }

        public string NumeroCTes { get; set; }

        public string Remetentes { get; set; }

        public string Origem { get; set; }

        public string Destinatarios { get; set; }

        public string Destino { get; set; }

        public bool CargaFechada { get; set; }

        public DateTime DataEmissaoDocumentos { get; set; }

        public decimal ValorFrete { get; set; }

        public int QuantidadeDocumentosFrete { get; set; }

        public bool NaoPermitirGerarAtendimento { get; set; }

        public string ObservacaoRelatorioDeEmbarque { get; set; }

        public string FilialVenda { get; set; }

        #endregion Propriedades sem Regras

        #region Propriedades com Regras

        public string Origens { get { return $"{Remetentes}, {Origem}"; } }

        public string Destinos { get { return $"{Destinatarios}, {Destino}"; } }

        public string DataCriacaoCargaDescricao { get { return DataCriacaoCarga != DateTime.MinValue ? DataCriacaoCarga.ToString("dd/MM/yyyy") : string.Empty; } }

        public string DataCarregamentoDescricao { get { return DataCarregamento != DateTime.MinValue ? DataCarregamento.ToString("dd/MM/yyyy") : string.Empty; } }

        public string DataEmissaoDocumentosDescricao { get { return DataEmissaoDocumentos != DateTime.MinValue ? DataEmissaoDocumentos.ToString("dd/MM/yyyy") : string.Empty; } }

        public string CargaFechadaDescricao { get { return CargaFechada ? "Sim" : "Não"; } }

        public string SituacaoDescricao { get { return Situacao.ObterDescricao(); } }

        #endregion Propriedades com Regras
    }
}
