using System;

namespace Dominio.ObjetosDeValor.Relatorios
{
    public class RelatorioDuplicatas
    {
        public int CodigoDuplicata { get; set; }

        public int Numero { get; set; }

        public Dominio.Enumeradores.TipoDuplicata Tipo { get; set; }

        public string DescricaoTipo
        {
            get
            {
                return this.Tipo.ToString("G");
            }
        }

        public DateTime DataLancamento { get; set; }

        public string Documento { get; set; }

        public DateTime DataDocumento { get; set; }

        public double CpfCnpjPessoa { get; set; }

        public string NomePessoa { get; set; }

        public string CpfMotorista { get; set; }

        public string NomeMotorista { get; set; }

        public string Veiculo1 { get; set; }

        public string Veiculo2 { get; set; }

        public string Veiculo3 { get; set; }

        public decimal Valor { get; set; }

        public decimal Acrescimo { get; set; }

        public decimal Desconto { get; set; }

        public decimal Total { get; set; }

        public string Observacao { get; set; }

        public int CodigoParcela { get; set; }

        public int Parcela { get; set; }

        public decimal ValorParcela { get; set; }

        public DateTime DataVcto { get; set; }

        public decimal ValorPgto { get; set; }

        public DateTime? DataPgto { get; set; }

        public string ObservacaoBaixa { get; set; }

    }
}
