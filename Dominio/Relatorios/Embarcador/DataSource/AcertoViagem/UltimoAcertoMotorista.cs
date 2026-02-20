using System;

namespace Dominio.Relatorios.Embarcador.DataSource.AcertoViagem
{
    public class UltimoAcertoMotorista
    {
        public string CodigoIntegracao { get; set; }
        public string Motorista { get; set; }
        public string CPF { get; set; }
        public DateTime DataAcerto { get; set; }
        public DateTime DataContratacao { get; set; }        
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public DateTime DataFechamento { get; set; }
        public string Situacao { get; set; }
        public string Frota { get; set; }

        public string DataAcertoFormatada
        {
            get { return DataAcerto != DateTime.MinValue ? DataAcerto.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataContratacaoFormatada
        {
            get { return DataContratacao != DateTime.MinValue ? DataContratacao.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DataInicialFormatada
        {
            get { return DataInicial != DateTime.MinValue ? DataInicial.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataFinalFormatada
        {
            get { return DataFinal != DateTime.MinValue ? DataFinal.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataFechamentoFormatada
        {
            get { return DataFechamento != DateTime.MinValue ? DataFechamento.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }
    }
}
