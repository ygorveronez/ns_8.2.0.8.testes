using System;

namespace Dominio.Relatorios.Embarcador.DataSource.GestaoPatio
{
    public class CheckListCarga
    {
        public DateTime Chegada { get; set; }
        public string Liberacao { get; set; }
        public string Viagem { get; set; }
        public string Placa { get; set; }
        public string Transportadora { get; set; }
        public string Motorista { get; set; }
        public int QuantidadePaletesVazios { get; set; }
        public int QuantidadePaletesComProduto { get; set; }
        public string NotaFiscal { get; set; }
        public int QuantidadeCaixasDevolucao { get; set; }
        public string TipoDevolucao { get; set; }
        public string Motivo { get; set; }
        public string Lacre { get; set; }
        public string NumerosPedidosEmbarcador { get; set; }
    }
}
