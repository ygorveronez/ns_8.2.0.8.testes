using CsvHelper.Configuration.Attributes;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Isis
{
    public class Carregamento
    {
        [Name("Year")]
        [Index(0)]
        public string AnoNota { get; set; }

        [Name("Company")]
        [Index(1)]
        public string Company { get { return "BRZL"; } }

        [Name("Packing Form Number")]
        [Index(2)]
        public string NumeroPedido { get; set; }

        [Name("Order Transaction Date")]
        [Index(3)]
        public string DataAlocacao { get; set; }

        [Name("Packing Form Date")]
        [Index(4)]
        public string DataFaturamento { get; set; }

        [Name("Customer Order Processing")]
        [Index(5)]
        public string CodigoIntegracaoCliente { get; set; }

        [Name("Customer Holding")]
        [Index(6)]
        public string CodigoIntegracaoGrupoCliente { get; set; }

        [Name("Customer Ship To")]
        [Index(7)]
        public string CodigoIntegracaoClienteDenovo { get { return this.CodigoIntegracaoCliente; } }

        [Name("Customer Ship To Name")]
        [Index(8)]
        public string NomeCliente { get; set; }

        [Name("CNPJ / CPF")]
        [Index(9)]
        public string CNPJCliente { get; set; }

        [Name("Address")]
        [Index(10)]
        public string Endereco { get; set; }

        [Name("City Name")]
        [Index(11)]
        public string Cidade { get; set; }

        [Name("State")]
        [Index(12)]
        public string Estado { get; set; }

        [Name("CEP")]
        [Index(13)]
        public string Cep { get; set; }

        [Name("Region Code")]
        [Index(14)]
        public string CodigoRegiao { get; set; }

        [Name("PO Number")]
        [Index(15)]
        public string ObservacaoPedido { get; set; }

        [Name("Data Esperada de Recebimento do Cliente")]
        [Index(16)]
        public string DataPrevisaoEntrega { get; set; }

        [Name("Order Number")]
        [Index(17)]
        public string NumeroOrdem { get; set; }

        [Name("Volumes (Master Cartons)")]
        [Index(18)]
        public string Caixas { get; set; }

        [Name("Volumes (M3)")]
        [Index(19)]
        public string Volumes { get; set; }

        [Name("Peso (Kg)")]
        [Index(20)]
        public string Peso { get; set; }

        [Name("Total Amount")]
        [Index(21)]
        public string ValorNota { get; set; }

        [Name("Gross Sales ($)")]
        [Index(22)]
        public string GrossSales { get; set; }

        [Name("Fiscal Flag")]
        [Index(23)]
        public string FiscalFlag { get; set; }

        [Name("Y")]
        [Index(24)]
        public string HoldLF { get { return ""; } }

        [Name("STATUS 01 (LP)")]
        [Index(25)]
        public string Status01LP { get { return "1A"; } }

        [Name("AA")]
        [Index(26)]
        public string StatusGeodis { get { return ""; } }

        [Name("Carrier Code")]
        [Index(27)]
        public string CarrierCode { get; set; }

        [Name("Data Expedição")]
        [Index(28)]
        public string DataExpedicao { get; set; }

        [Name("Hora Expedição")]
        [Index(29)]
        public string HoraExpedicao { get; set; }

        [Name("Tipo de Veículo")]
        [Index(30)]
        public string TipoVeiculo { get; set; }

        [Name("Sequencia Veiculo")]
        [Index(31)]
        public string SequenciaVeiculo { get; set; }

        [Name("Modal")]
        [Index(32)]
        public string Modal { get; set; }

        [Name("Etiquetagem")]
        [Index(33)]
        public string Etiquetagem { get; set; }

        [Name("AI")]
        [Index(34)]
        public string Vas { get { return string.Empty; } }

        [Name("ISCA")]
        [Index(35)]
        public string Isca { get; set; }

        [Name("AK")]
        [Index(36)]
        public string NumeroNF { get { return string.Empty; } }

        [Name("Número de Transporte")]
        [Index(37)]
        public string NroTransporte { get; set; }

        [Name("Status Mattel")]
        [Index(38)]
        public string StatusMattel { get { return "LPF"; } }

        [Name("Facility")]
        [Index(39)]
        public string Inventario { get; set; }

        [Name("Bulk of Pallet B/P")]
        [Index(40)]
        public string BulkOfPallet { get; set; }

        [Name("Warehouse")]
        [Index(41)]
        public string Filial { get; set; }

        [Name("Invoice ISIS Number")]
        [Index(42)]
        public string InvoiceISISNumber { get { return "0"; } }
    }
}
