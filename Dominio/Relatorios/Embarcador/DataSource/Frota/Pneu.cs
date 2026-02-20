using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Frota
{
    public sealed class Pneu 
    {
        #region Propriedades

        public int Codigo { get; set; } 
        public string NumeroFogo { get; set; } 
        public string Frota { get; set; } 
        public string Placa { get; set; }
        public string ModeloVeicular { get; set; } 
        public string MarcaPneu { get; set; } 
        private Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoBandaRodagemPneu TipoBandaRodagem { get; set; } 
        private Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPneu StatusPneu { get; set; } 
        private DateTime DataEntrada { get; set; } 
        public string ModeloPneu { get; set; } 
        public string BandaRodagem { get; set; } 
        public string AlmoxarifadoAtual { get; set; } 
        public string Movimentacao { get; set; } 
        public int KmRodado { get; set; } 
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.VidaPneu VidaUtil { get; set; } 
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAquisicaoPneu TPAquisicao { get; set; } 

        #endregion

        #region Propriedades com regras

        public string DataEntradaFormatada 
        {
            get { return DataEntrada != DateTime.MinValue ? DataEntrada.ToString("dd/MM/yyyy") : string.Empty;  }
        }

        public string TipoBandaRodagemDescricao 
        { 
            get { return Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoBandaRodagemPneuHelper.ObterDescricao(TipoBandaRodagem); }
        }

        public string StatusPneuDescricao
        {
            get { return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPneuHelper.ObterDescricao(StatusPneu); }
        }

        public string VidaUtilDescricao
        {
            get { return Dominio.ObjetosDeValor.Embarcador.Enumeradores.VidaPneuHelper.ObterDescricao(VidaUtil); }
        }

        public string TPAquisicaoDescricao
        {
            get { return Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAquisicaoPneuHelper.ObterDescricao(TPAquisicao); }
        }


        #endregion

    }
}
