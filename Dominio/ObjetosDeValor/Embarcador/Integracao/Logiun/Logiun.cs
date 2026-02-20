using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Logiun
{
    public class CancelamentoTransferencia
    {
        public string CnpjEmpresaOrigem { get; set; }
        public string CnpjEmpresaDestino { get; set; }
        public string CnpjEmpresaTransportadora { get; set; }
        public string DataTransferencia { get; set; }
        public string NumeroViagem { get; set; }
        public string MotivoCancelamento { get; set; }
    }
    public class Transferencia
    {
        public string CnpjEmpresaOrigem { get; set; }
        public string RazaoSocialOrigem { get; set; }
        public string CnpjEmpresaDestino { get; set; }
        public string RazaoSocialDestino { get; set; }
        public string CnpjEmpresaTransportadora { get; set; }
        public string RazaoSocialTransportadora { get; set; }
        public string DataTransferencia { get; set; }
        public string NumeroViagem { get; set; }
        public string DocumentoMotorista { get; set; }
        public string NomeMotorista { get; set; }
        public string PlacaVeiculo { get; set; }
        public decimal ValorFrete { get; set; }
        public string Observacao { get; set; }
        public List<Embalagem> Embalagens { get; set; }
    }

    public class Embalagem
    {
        public string CodigoEmbalagem { get; set; }
        public string NumeroNotaFiscal { get; set; }
        public string SerieNotaFiscal { get; set; }
        public int Quantidade { get; set; }
        public int QuantidadeAvaria { get; set; }
        public decimal ValorUnitarioEmbalagem { get; set; }
    }
}
