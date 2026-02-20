using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Escrituracao
{
    public class SaldoProvisao
    {
        public double CNPJCompanhia { get; set; }

        public string NomeCompanhia { get; set; }

        public string TipoCompanhia { get; set; }

        public string Companhia { get; set; }
        
        public int TipoTransporte { get; set; }

        public string CNPJTransportadora { get; set; }

        public string NomeTransportadora { get; set; }

        public string Transportadora { get; set; }

        public string Origem { get; set; }

        public string Destino { get; set; }

        public string Carga { get; set; }

        public DateTime DataEmissao { get; set; }

        public double CNPJEmitente { get; set; }

        public string NomeEmitente { get; set; }

        public string TipoEmitente { get; set; }

        public string Emitente { get; set; }

        public int NotaFiscal { get; set; }

        public int CTe { get; set; }

        public int Serie { get; set; }

        public string MeioTransporte { get; set; }

        public string CentroCusto { get; set; }

        public DateTime DataLancamento { get; set; }

        public string ContaContabil { get; set; }

        public string TipoContabilizacao { get; set; }

        public string TipoLancamento { get; set; }

        public decimal ValorLancamento { get; set; }

        public decimal Credito { get; set; }

        public decimal Debito { get; set; }

        public decimal Aliquota { get; set; }

        public decimal ValorICMS { get; set; }

        public decimal ICMSRetido { get; set; }

        public string MotivoCancelamento { get; set; }

        public string MesComp { get; set; }

        public string Filial { get; set; }

        public string MesCont { get; set; }

        public int Ano { get; set; }

    }
}
