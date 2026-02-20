using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Veiculos
{
    public class ResponsavelVeiculo
    {
        public int Codigo { get; set; }
        public string Placa { get; set; }
        public string FuncionarioResponsavel { get; set; }
        public string FuncionarioLancamento { get; set; }
        public DateTime DataLancamento { get; set; }
        public string Observacao { get; set; }

        public string DataLancamentoFormatada
        {
            get { return DataLancamento != DateTime.MinValue ? DataLancamento.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }
    }
}
