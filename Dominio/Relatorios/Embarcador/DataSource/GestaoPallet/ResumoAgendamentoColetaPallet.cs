using System;

namespace Dominio.Relatorios.Embarcador.DataSource.GestaoPallet
{
    public class ResumoAgendamentoColetaPallet
    {
        public int NumeroOrdem { get; set; }

        public DateTime DataOrdem { get; set; }

        public string Carga { get; set; }

        public string Filial { get; set; }

        public string Cliente { get; set; }

        public int QuantidadePallet { get; set; }

        public string Solicitante { get; set; }

        public string Transportador { get; set; }

        public string Placa { get; set; }

        public string Motorista { get; set; }
    }
}
