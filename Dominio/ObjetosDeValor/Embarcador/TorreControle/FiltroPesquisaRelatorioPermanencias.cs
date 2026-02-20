using System;

namespace Dominio.ObjetosDeValor.Embarcador.TorreControle
{
    public class FiltroPesquisaRelatorioPermanencias
    {
        public string Carga { get; set; }
        public DateTime? DataCarregamentoInicial { get; set; }
        public DateTime? DataCarregamentoFinal { get; set; }
        public string Placa { get; set; }
        public int? CodigoTransportador { get; set; }
        public int? CodigoGrupoPessoas { get; set; }
        public DateTime? DataCriacaoCargaInicial { get; set; }
        public DateTime? DataCriacaoCargaFinal { get; set; }
        public DateTime? DataAgendamentoColetaInicial { get; set; }
        public DateTime? DataAgendamentoColetaFinal { get; set; }
        public DateTime? DataAgendamentoEntregaInicial { get; set; }
        public DateTime? DataAgendamentoEntregaFinal { get; set; }
        public int? CodigoFilial { get; set; }
        public int? CodigoCliente { get; set; }
        public bool? CodigoTipoParada { get; set; }
    }
}