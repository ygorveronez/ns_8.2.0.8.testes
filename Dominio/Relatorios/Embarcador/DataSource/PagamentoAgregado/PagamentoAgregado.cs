using System;

namespace Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado
{
    public class PagamentoAgregado
    {
        public int Numero {get; set;}
        public DateTime DataPagamento { get; set; }
        public string RazaoEmpresa { get; set; }
        public DateTime DataGeracao { get; set; }
        public string ResponsavelEmpresa { get; set; }
        public string CNPJEmpresa { get; set; }
        public string EnderecoEmpresa { get; set; }
        public string BairroEmpresa { get; set; }
        public string CidadeEmpresa { get; set; }
        public string EstadoEmpresa { get; set; }
        public string CEPEmpresa { get; set; }
        public string NomeCliente { get; set; }
        public double CNPJCliente { get; set; }
        public string EnderecoCliente { get; set; }
        public string BairroCliente { get; set; }
        public string CidadeCliente { get; set; }
        public string EstadoCliente { get; set; }
        public string CEPCliente { get; set; }
        public DateTime DataFinal { get; set; }
        public DateTime DataInicial { get; set; }
        public decimal Valor { get; set; }
        public string Observacao { get; set; }
        public string AgenciaCliente { get; set; }
        public string DigitoAgenciaCliente { get; set; }
        public string ContaCliente { get; set; }
        public int TipoContaCliente { get; set; }
        public string BancoCliente { get; set; }
        public Int64 ContemDescontoAcrescimo { get; set; }
        public Int64 ContemaAdiantamento { get; set; }
    }
}
