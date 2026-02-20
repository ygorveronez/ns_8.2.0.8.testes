using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Pallets
{
    public sealed class ControleEntradaSaidaPallet
    {
        public string Cliente { get; set; }

        public int Codigo { get; set; }

        public DateTime Data { get; set; }

        public string Filial { get; set; }

        public string FilialCnpj { get; set; }

        public string FilialCodigoIntegracao { get; set; }

        public string NaturezaMovimentacao { get; set; }

        public string Observacao { get; set; }

        public int QuantidadeEntrada { get; set; }

        public int QuantidadeSaida { get; set; }

        public int SaldoTotal { get; set; }

        public string Setor { get; set; }

        public string TipoLancamento { get; set; }

        public string Transportador { get; set; }

        public string TransportadorCnpj { get; set; }

        public string TransportadorCodigoIntegracao { get; set; }
    }
}
