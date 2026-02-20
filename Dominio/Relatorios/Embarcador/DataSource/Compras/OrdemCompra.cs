using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Compras
{
    public class OrdemCompra
    {
        public int Numero { get; set; }

        public DateTime Data { get; set; }

        public DateTime DataPrevisao { get; set; }

        public string Situacao { get; set; }

        public string Operador { get; set; }

        public string Transportador { get; set; }

        public string Fornecedor { get; set; }

        public string Observacao { get; set; }

        public string Placa { get; set; }

        public string Aprovador { get; set; }

        //

        public string Motorista { get; set; }

        public string MotoristaCFP { get; set; }

        public string MotoristaFuncao { get; set; }

        public string MotoristaSetor { get; set; }

        public string MotoristaNascimento { get; set; }

        public string MotoristaRG { get; set; }

        public string FornecedorCNPJ { get; set; }

        public string FornecedorEndereco { get; set; }

        public string LocalidadeFornecedor { get; set; }

        public string FornecedorBairro { get; set; }

        public string FornecedorFone { get; set; }

        public string MotivoOrdemCompra { get; set; }

        public string CondicaoPagamento { get; set; }
    }
}
