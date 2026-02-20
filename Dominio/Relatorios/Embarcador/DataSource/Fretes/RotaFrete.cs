using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Fretes
{
    public class RotaFrete
    {
        #region Propriedades

        public int Codigo { get; set; }
        public string Descricao { get; set; }
        public string CodigoIntegracao { get; set; }
        public int TempoViagemHoras { get; set; }
        public decimal Quilometros { get; set; }
        public int VelocidadeMediaCarregado { get; set; }
        public int VelocidadeMediaVazio { get; set; }
        public string Observacao { get; set; }
        public string Detalhes { get; set; }
        public TipoRotaFrete TipoRota { get; set; }
        public RetornoCargaTipo TipoCarregamentoIda { get; set; }
        public RetornoCargaTipo TipoCarregamentoVolta { get; set; }
        public string Situacao { get; set; }
        public string GrupoPessoas { get; set; }
        public string Remetente { get; set; }
        public string TipoOperacao { get; set; }
        public string Origem { get; set; }
        public string Destino { get; set; }
        public string Destinatario { get; set; }
        public string EstadoDestino { get; set; }
        public string CodigoIntegracaoValePedagio { get; set; }
        public string CEP { get; set; }
        public int LeadTimeDias { get; set; }
        public string Transportador { get; set; }
        public decimal PercentualCarga { get; set; }
        public string Distribuidor { get; set; }
        private bool RotaExclusivaCompraValePedagio { get; set; }
        public string Fronteira { get; set; }
        public string TempoCarregamento { get; set; }
        public string TempoDescarga { get; set; }
        public long TempoMedioPermanenciaFronteira { get; set; }

        #endregion

        #region Propriedades com Regras

        public string DescricaoTipoRota
        {
            get { return TipoRota.ObterDescricao(); }
        }

        public string DescricaoTipoCarregamentoIda
        {
            get { return TipoCarregamentoIda.ObterDescricao(); }
        }

        public string DescricaoTipoCarregamentoVolta
        {
            get { return TipoRota == TipoRotaFrete.IdaVolta ? TipoCarregamentoVolta.ObterDescricao() : string.Empty; }
        }

        public string RotaExclusivaCompraValePedagioFormatada
        {
            get { return RotaExclusivaCompraValePedagio ? "Sim" : "Não"; }
        }

        public string TempoFronteira 
        {
            get { return TempoMedioPermanenciaFronteira > 0 ? $"{TempoMedioPermanenciaFronteira / 60:D2}:{TempoMedioPermanenciaFronteira % 60:D2}" : string.Empty; }
        }

        #endregion
    }
}

