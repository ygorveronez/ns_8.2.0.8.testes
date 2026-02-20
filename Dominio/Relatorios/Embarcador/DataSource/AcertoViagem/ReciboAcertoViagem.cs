using System;

namespace Dominio.Relatorios.Embarcador.DataSource.AcertoViagem
{
    public class ReciboAcertoViagem
    {
        public int NumeroAcerto { get; set; }
        public int NumeroRecibo { get; set; }
        public string Motorista { get; set; }
        public decimal ValorTotal { get; set; }
        public string ValorExtenso { get; set; }
        public string Veiculos { get; set; }
        public string FrotaVeiculos { get; set; }
        public string Proprietario { get; set; }
        public string Operador { get; set; }
        public DateTime DataFechamentoAcerto { get; set; }
        public DateTime DataInicioAcerto { get; set; }
        public DateTime DataFimAcerto { get; set; }
        public string DescricaoDespesa { get; set; }
        public decimal ValorDespesa { get; set; }

        public string ObservacaoFixa { get; set; }

        public string DescricaoDataFechamentoAcerto
        {
            get
            {
                if (DataFechamentoAcerto != DateTime.MinValue)
                    return DataFechamentoAcerto.ToString("dd/MM/yyyy");
                else
                    return string.Empty;
            }
        }

        public string DescricaoDataInicioAcerto
        {
            get
            {
                if (DataInicioAcerto != DateTime.MinValue)
                    return DataInicioAcerto.ToString("dd/MM/yyyy");
                else
                    return string.Empty;
            }
        }

        public string DescricaoDataFimAcerto
        {
            get
            {
                if (DataFimAcerto != DateTime.MinValue)
                    return DataFimAcerto.ToString("dd/MM/yyyy");
                else
                    return string.Empty;
            }
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
