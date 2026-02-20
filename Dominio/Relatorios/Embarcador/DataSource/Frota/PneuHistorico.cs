using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Relatorios.Embarcador.DataSource.Frota
{
    public sealed class PneuHistorico
    {
        #region Propriedades 

        public int Codigo { get; set; }
        public DateTime Data { get; set; }
        public string BandaRodagem { get; set; }
        public string BandaRodagemHistorico { get; set; }
        public string Descricao { get; set; }
        public string Dimensao { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string NumeroFogo { get; set; }
        public VidaPneu Vida { get; set; }
        public string ObservacaoSucata { get; set; }
        public string Servicos { get; set; }
        public int KmAtualRodado { get; set; }
        public string DTO { get; set; }
        public decimal CustoEstimadoReforma { get; set; }
        public DateTime DataMovimentacao { get; set; }
        public string UsuarioOperador { get; set; }
        public string MotivoSucata { get; set; }
        public string Almoxarifado { get; set; }
        public TipoAquisicaoPneu TipoAquisicao { get; set; }
        public decimal Sulco { get; set; }
        public decimal SulcoAnterior { get; set; }
        public decimal ValorResidualAtualPneu { get; set; }

        public string ObservacaoPneu { get; set; }
        #endregion

        public string DataFormatada
        {
            get { return Data.ToString("dd/MM/yyyy HH:mm"); }
        }

        public string DataMovimentacaoFormatada
        {
            get { return DataMovimentacao != DateTime.MinValue ? DataMovimentacao.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string VidaDescricao
        {
            get { return Vida.ObterDescricao(); }
        }

        public string TipoAquisicaoDescricao
        {
            get { return TipoAquisicao.ObterDescricao(); }
        }

        public decimal SulcoGasto
        {
            get { return (Sulco > SulcoAnterior) ? Sulco - SulcoAnterior : SulcoAnterior - Sulco; }
        }
    }
}
