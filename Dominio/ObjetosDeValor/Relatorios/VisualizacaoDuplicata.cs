using System;

namespace Dominio.ObjetosDeValor.Relatorios
{
    public class VisualizacaoDuplicata
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

        public string IePessoa { get; set; }

        public string NomePessoa { get; set; }

        public string EnderecoPessoa { get; set; }

        public string MunicipioUf { get; set; }

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

        public string Embarcador { get; set; }

        public string TipoVeiculo { get; set; }

        public string DadosBancarios { get; set; }

        public string LocalidadeOrigem { get; set; }

        public string LocalidadeDestino { get; set; }

        public decimal AdicionaisPeso { get; set; }

        public int AdicionaisVolumes { get; set; }
    }
}
