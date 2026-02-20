using System;

namespace Dominio.Relatorios.Embarcador.DataSource.AcertoViagem
{
    public class AcertoDeViagem
    {
        public DateTime Data { get; set; }
        public int NumeroAcerto { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public DateTime DataFechamento { get; set; }
        public string Motorista { get; set; }
        public string CodigoIntegracao { get; set; }
        public string CPF { get; set; }
        public string Frota { get; set; }
        public string Segmento { get; set; }
        public string Situacao { get; set; }
        public string Operador { get; set; }
        public string OperadorInicio { get; set; }
    }
}
