using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Frota
{
    public class Sinistro
    {
        #region Propriedades
        public int Codigo { get; set; }
        public int NumeroSinistro { get; set; }
        public CausadorSinistro CausadorSinistro { get; set; }
        public string TipoSinistro { get; set; }
        public string NumeroBoletimOcorrencia { get; set; }
        public DateTime DataEmissaoDados { get; set; }
        public DateTime DataHoraSinistro { get; set; }
        public string Endereco { get; set; }
        public string Local { get; set; }
        public string Cidade { get; set; }
        public string PlacaCavalo { get; set; }
        public string PlacaReboque { get; set; }
        public string Motorista { get; set; }
        public TipoEnvolvidoSinistro Tipo { get; set; }
        public string Nome { get; set; }
        public string CPF { get; set; }
        public string TelefoneContato1 { get; set; }
        public string TelefoneContato2 { get; set; }
        public string Veiculo { get; set; }
        public string Observacao { get; set; }
        public int NumeroOrdemServico { get; set; }
        public string LocalManutencao { get; set; }
        public SituacaoOrdemServicoFrota SituacaoOS { get; set; }
        public IndicadorPagadorSinistro IndicacaoPagador { get; set; }
        public string Movimento { get; set; }
        public string Pessoa { get; set; }
        public DateTime DataEmissaoTitulo { get; set; }
        public DateTime DataVencimentoTitulo { get; set; }
        public string NumeroDocumento { get; set; }
        public decimal ValorOriginal { get; set; }
        public string ObservacaoTitulo { get; set; }
        public int DocumentoEntrada { get; set; }
        public TipoHistoricoInfracao SituacaoSinistro { get; set; }
        #endregion

        #region Propriedades com Regras

        public string CausadorSinistroDescricao
        {
            get { return CausadorSinistro.ObterDescricao(); }
        }
         public string TipoDescricao
        {
            get { return Tipo.ObterDescricao(); }
        }
        public string IndicacaoPagadorDescricao
        {
            get { return IndicacaoPagador.ObterDescricao(); }
        }
         public string SituacaoOSDescricao
        {
            get { return SituacaoOS.ObterDescricao(); }
        }
         public string SituacaoSinistroDescricao
        {
            get { return SituacaoSinistro.ObterDescricao(); }
        }

        #endregion
    }
}
