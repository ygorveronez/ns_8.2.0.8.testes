using System;

namespace Dominio.ObjetosDeValor.Embarcador.CTe
{
    public class Periodo
    {
        public Periodo(DateTime dataInicial, DateTime dataFinal)
        {
            DataInicial = dataInicial;
            DataFinal = dataFinal;
        }

        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }

        public string Descricao
        {
            get
            {
                return DataInicial.ToString("dd") + " à " + DataFinal.ToString("dd/MM/yy");
            }
        }
    }
}
