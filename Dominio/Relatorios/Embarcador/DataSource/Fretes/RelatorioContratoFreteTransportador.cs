using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Relatorios.Embarcador.DataSource.Fretes
{
    public class RelatorioContratoFreteTransportador
    {
        #region Propriedades

        public int Codigo { get; set; }
        public int NumeroSequencial { get; set; }
        public string NumeroEmbarcador { get; set; }
        public string Descricao { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public string Transportador { get; set; }
        public string TipoContratoFrete { get; set; }
        public bool Situacao { get; set; }
        public string Cavalo { get; set; }
        public string Carreta { get; set; }
        public string TipoVeiculo { get; set; }
        public SituacaoContratoFreteTransportador Status { get; set; }
        public string ContratoTransportador { get; set; }
        public string IDExterno { get; set; }
        public string StatusAceiteContrato { get; set; }
        public PeriodoAcordoContratoFreteTransportador TipoFechamento { get; set; }
        public string ValorMensal { get; set; }
        public string ModeloVeicular { get; set; }
        public decimal ValorAcordado { get; set; }
        public int QuantidadeVeiculo { get; set; }
        public string QuantidadeAproxCargasMensal { get; set; }
        public string TipoCarga { get; set; }
        public string CanalEntrega { get; set; }
        public PontoPlanejamentoTransporte PontoPlanejamentoTransporte { get; set; }
        public TipoIntegracaoUnilever TipoIntegracao { get; set; }
        public TipoGrupoCarga GrupoCarga { get; set; }
        public string TabelasFrete { get; set; }
        public string Observacao { get; set; }

        #endregion

        #region Propriedades com Regras

        public string Numero
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.NumeroEmbarcador))
                    return this.NumeroEmbarcador;
                else
                    return this.NumeroSequencial.ToString();
            }
        }

        public string DescricaoSituacao
        {
            get
            {
                return this.Situacao ? "Ativo" : "Inativo";
            }
        }

        public string VigenciaInicial
        {
            get
            {
                return this.DataInicial.ToString("dd/MM/yyyy");
            }
        }

        public string VigenciaFinal
        {
            get
            {
                return this.DataFinal.ToString("dd/MM/yyyy");
            }
        }

        public string DescricaoStatus
        {
            get { return Status.ObterDescricao(); }
        }

        public string DescricaoTipoFechamento
        {
            get { return TipoFechamento.ObterDescricao(); }
        }

        public string DescricaoPontoPlanejamentoTransporte
        {
            get { return PontoPlanejamentoTransporte.ObterDescricao(); }
        }

        public string DescricaoTipoIntegracao
        {
            get { return TipoIntegracao.ObterDescricao(); }
        }

        public string DescricaoGrupoCarga
        {
            get { return GrupoCarga.ObterDescricao(); }
        }

        #endregion
    }
}