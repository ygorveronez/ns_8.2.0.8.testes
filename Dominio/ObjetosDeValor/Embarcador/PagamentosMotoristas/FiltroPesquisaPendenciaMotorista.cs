using System;

namespace Dominio.ObjetosDeValor.Embarcador.PagamentosMotoristas
{
    public sealed class FiltroPesquisaPendenciaMotorista
    {
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public decimal? ValorInicial { get; set; }
        public decimal? ValorFinal { get; set; }
        public Enumeradores.SituacaoPendenciaMotorista? Situacao { get; set; }
        public int CodigoMotorista { get; set; }
    }
}
