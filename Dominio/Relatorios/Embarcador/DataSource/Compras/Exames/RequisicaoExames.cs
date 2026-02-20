using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Compras
{
    public class RequisicaoExames
    {
        public int Numero { get; set; }

        public DateTime DataPrevisao { get; set; }

        public string Operador { get; set; }

        public string Observacao { get; set; }


        public string Motorista { get; set; }

        public string MotoristaCFP { get; set; }

        public string MotoristaFuncao { get; set; }

        public string MotoristaSetor { get; set; }

        public string MotoristaNascimento { get; set; }

        public string MotoristaRG { get; set; }


        public string Fornecedor { get; set; }

        public string FornecedorCNPJ { get; set; }

        public string FornecedorEndereco { get; set; }

        public string LocalidadeFornecedor { get; set; }

        public string FornecedorBairro { get; set; }

        public string FornecedorFone { get; set; }
    }
}
