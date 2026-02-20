using System;

namespace Dominio.ObjetosDeValor.EDI.Notfis
{
    public class ComplementoNotaFiscal
    {
        public string CodigoPerfilFiscal { get; set; }
        public string TipoPeriodoEntrega { get; set; }
        public DateTime DataInicioEntrega { get; set; }
        public DateTime SetHoraInicioEntrega
        {
            set
            {
                this.DataInicioEntrega = new DateTime(this.DataInicioEntrega.Year, this.DataInicioEntrega.Month, this.DataInicioEntrega.Day, value.Hour, value.Minute, value.Second);
            }
        }
        public DateTime DataFimEntrega { get; set; }
        public DateTime SetHoraFimEntrega
        {
            set
            {
                this.DataFimEntrega = new DateTime(this.DataFimEntrega.Year, this.DataFimEntrega.Month, this.DataFimEntrega.Day, value.Hour, value.Minute, value.Second);
            }
        }
        public string LocalDesembarque { get; set; }
        public string CalculoFreteDiferenciado { get; set; }
        public string TabelaFrete { get; set; }

        public string CGCEntrega1 { get; set; }
        public string SerieEntrega1 { get; set; }
        public string SerieVenda1 { get; set; }
        public string Numero1 { get; set; }

        public string CGCEntrega2 { get; set; }
        public string SerieEntrega2 { get; set; }
        public string SerieVenda2 { get; set; }
        public string Numero2 { get; set; }

        public string CGCEntrega3 { get; set; }
        public string SerieEntrega3 { get; set; }
        public string SerieVenda3 { get; set; }
        public string Numero3 { get; set; }

        public string CGCEntrega4 { get; set; }
        public string SerieEntrega4 { get; set; }
        public string SerieVenda4 { get; set; }
        public string Numero4 { get; set; }

        public string CGCEntrega5 { get; set; }
        public string SerieEntrega5 { get; set; }
        public string SerieVenda5 { get; set; }
        public string Numero5 { get; set; }
        public string ChaveNFe { get; set; }

        public string Filler1 { get; set; }
        public string TipoVeiculo { get; set; }
        public string Filler2 { get; set; }
    }
}
