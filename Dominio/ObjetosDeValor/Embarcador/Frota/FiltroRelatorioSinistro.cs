using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Frota
{
    public class FiltroRelatorioSinistro
    {
        public int NumeroSinistro { get; set; }
        public CausadorSinistro? CausadorSinistro { get; set; }
        public int CodigoTipoSinistro { get; set; }
        public string NumeroBoletimOcorrencia { get; set; }
        public DateTime DataSinistroInicial { get; set; }
        public DateTime DataSinistroFinal { get; set; }
        public int CodigoCidade { get; set; }
        public int CodigoVeiculo { get; set; }
        public int CodigoVeiculoReboque { get; set; }
        public int CodigoMotorista { get; set; }
        public int NumeroOrdemServico { get; set; }
        public IndicadorPagadorSinistro? IndicacaoPagador { get; set; }
        public TipoHistoricoInfracao? SituacaoSinistro { get; set; }
    }
}
