using System;

namespace Dominio.ObjetosDeValor.Embarcador.Container
{
    public class ControleContainer
    {
        #region Propriedades

        public int Codigo { get; set; }

        public string NumeroContainer { get; set; }

        public string TipoContainer { get; set; }

        public int Carga { get; set; }

        public string CargaEmbarcador { get; set; }

        public int CargaAtual { get; set; }

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

        public int DiasEmPosse { get; set; }

        public decimal ValorDiaria { get; set; }

        public DateTime DataEmbarque { get; set; } //o nome no front foi alterado para Data Porto

        public DateTime DataEmbarqueNavio { get; set; }

        public string Justificativa { get; set; }

        public string NumeroBooking { get; set; }

        public string NumeroBookingAgrupada { get; set; }

        public string JustificativaDescritiva { get; set; }

        public string Pedido { get; set; }

        public string PedidoAgrupado { get; set; }

        public string NumeroEXP { get; set; }

        public string NumeroEXPAgrupada { get; set; }

        public string FilialCargaAtual { get; set; }

        public string CNPJFilialCargaAtual { get; set; }

        public string NumeroCargaAgrupada { get; set; }

        public string AreaEsperaVazio { get; set; }

        public int CodigoTipoContainer { get; set; }

        public string FilialCargaOrigem { get; set; }

        public string CNPJFilialCargaOrigem { get; set; }

        #endregion Propriedades

        #region Propriedades com Regras

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
