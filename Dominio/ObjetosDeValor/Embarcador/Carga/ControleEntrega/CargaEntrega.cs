using System;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega
{
    public sealed class CargaEntrega
    {
        public int Codigo { get; set; }

        public bool Calculada { get; set; }
        public int OrdemPrevista { get; set; }
        public int OrdemRealizada { get; set; }
        public Dominio.Entidades.Cliente Cliente { get; set; }
        public double? CodigoCliente { get; set; }
        public int Distancia { get; set; }
        public int TempoExtraEntrega { get; set; }
        public int DistanciaAteDestino { get; set; }
        public DateTime? DataInicioEntregaPrevista { get; set; }
        public DateTime? DataInicioEntregaRealizada { get; set; }
        public DateTime? DataFimEntregaPrevista { get; set; }
        public DateTime? DataFimEntregaRealizada { get; set; }
        public DateTime? DataPrevisaoEntregaTransportador { get; set; }
        public bool Coleta { get; set; }
        public bool Fronteira { get; set; }
        public decimal? Peso { get; set; }
        public CargaEntregaComposicaoPrevisao ComposicaoPrevisao { get; set; }

        public DateTime? DataEntradaRaio { get; set; }

        public DateTime? DataSaidaRaio { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega Situacao { get; set; }

        public Dominio.Entidades.Localidade Localidade { get; set; }
    }
}
