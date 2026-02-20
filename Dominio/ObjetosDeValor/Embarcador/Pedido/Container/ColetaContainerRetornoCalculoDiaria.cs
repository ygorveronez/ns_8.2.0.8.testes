using System;

namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public class ColetaContainerRetornoCalculoDiaria
    {
        #region Propriedades

        public int Codigo { get; set; }

        public string NumeroContainer { get; set; }

        public int Filial { get; set; }

        public DateTime DataColeta { get; set; }

        public Enumeradores.StatusColetaContainer Status { get; set; }

        public DateTime DataUltimaMovimentacao { get; set; }

        public double LocalColeta { get; set; }

        public double LocalAtual { get; set; }

        public double LocalEmbarque { get; set; }

        public string ClienteLocalAtual { get; set; }

        public string ClienteLocalEmbarque { get; set; }

        public string ClienteLocalColeta { get; set; }

        public int FreeTime { get; set; }

        public decimal ValorDiaria { get; set; }

        public int DiasEmPosse { get; set; }

        #endregion Propriedades

        #region Propriedades com Regras

        public DateTime DataLimiteFreeTime
        {
            get
            {
                if (FreeTime > 0)
                    return DataColeta.AddDays(FreeTime - 1);

                return DateTime.MinValue;
            }
        }

        public int DiasExcesso
        {
            get
            {
                return DiasEmPosse - FreeTime;
            }
        }

        public decimal ValorDevido
        {
            get
            {
                return DiasExcesso * ValorDiaria;
            }
        }

        #endregion Propriedades com Regras
    }
}
