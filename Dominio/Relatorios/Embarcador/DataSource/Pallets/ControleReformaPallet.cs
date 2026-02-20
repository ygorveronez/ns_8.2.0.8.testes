using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Pallets
{
    public class ControleReformaPallet
    {
        public int Codigo { get; set; }
        public DateTime Data { get; set; }
        public string DataRetorno { get; set; }
        public string Filial { get; set; }
        public string FilialCnpj { get; set; }
        public string FilialCodigoIntegracao { get; set; }
        public string Fornecedor { get; set; }
        public string FornecedorCpfCnpj { get; set; }
        public string FornecedorCodigoIntegracao { get; set; }
        public string Transportador { get; set; }
        public string TransportadorCnpj { get; set; }
        public string TransportadorCodigoIntegracao { get; set; }
        public string Nfe { get; set; }
        public decimal NfeValorTotal { get; set; }
        public string NfeRetorno { get; set; }
        public decimal NfeRetornoValorTotal { get; set; }
        public string Nfs { get; set; }
        public decimal NfsValorTotal { get; set; }
        public int Quantidade { get; set; }
        public int QuantidadeRetorno { get; set; }
    }
}
