using System;

namespace Dominio.Relatorios.Embarcador.DataSource.GestaoPatio
{
    public class CheckListGuarita
    {
        public int CodigoCheck { get; set; }
        public DateTime DataAbertura { get; set; }
        public int KMAtual { get; set; }
        public string EntradaSaida { get; set; }
        public string Observacao { get; set; }
        public int CodigoPergunta { get; set; }
        public string DescricaoPergunta { get; set; }
        public string DescricaoCategoria { get; set; }
        public string TipoCategoria { get; set; }
        public int CodigoTipo { get; set; }
        public string RespostaPergunta { get; set; }
        public bool OpcaoPergunta { get; set; }
        public int CodigoAlternativa { get; set; }
        public string DescricaoAlternativa { get; set; }
        public int OrdemAlternativa { get; set; }
        public bool MarcadoAlternativa { get; set; }
        public string Operador { get; set; }
        public string Carga { get; set; }
        public int OrdemServico { get; set; }
        public string Veiculo { get; set; }
        public string Motorista { get; set; }
        private string CPF { get; set; }

        public string DescricaoDataAbertura
        {
            get
            {
                if (DataAbertura != DateTime.MinValue)
                    return DataAbertura.ToString("dd/MM/yyyy HH:mm");
                else
                    return string.Empty;
            }
        }

        public string CPFFormatado { get => CPF.ObterCpfFormatado(); }
    }
}
