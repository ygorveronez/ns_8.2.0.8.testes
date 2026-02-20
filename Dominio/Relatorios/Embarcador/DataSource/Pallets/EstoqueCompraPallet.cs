using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Pallets
{
    public class EstoqueCompraPallet
    {
        public int Codigo { get; set; }
        public DateTime Data { get; set; }
        public string Filial { get; set; }
        public string FilialCnpj { get; set; }
        public string FilialCodigoIntegracao { get; set; }
        public string Fornecedor { get; set; }
        public string FornecedorCpfCnpj { get; set; }
        public string FornecedorCodigoIntegracao { get; set; }
        public string Transportador { get; set; }
        public string TransportadorCnpj { get; set; }
        public string TransportadorCodigoIntegracao { get; set; }
        public int NumeroNfe { get; set; }
        public int Quantidade { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal ValorUnitario { get; set; }
    }
}
