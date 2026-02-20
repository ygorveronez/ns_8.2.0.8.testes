using System;

namespace Dominio.ObjetosDeValor.Embarcador.Container
{
    public sealed class FiltroPesquisaControleContainer
    {
        public string CodigoCargaEmbarcador { get; set; }
        public string NumeroContainer { get; set; }
        public Enumeradores.StatusColetaContainer? StatusContainer { get; set; }
        public DateTime DataInicialColeta { get; set; }
        public DateTime DataFinalColeta { get; set; }
        public double LocalEsperaVazio { get; set; }
        public double LocalAtual { get; set; }
        public double LocalColeta { get; set; }
        public DateTime DataEmbarque { get; set; }
        public DateTime DataUltimaMovimentacao { get; set; }
        public bool SomenteExcedidos { get; set; }
        public int DiasPosse { get; set; }
        public int DiasPosseFim { get; set; }
        public double AreaDeRedex { get; set; }
        public string NumeroEXP { get; set; }
        public string NumeroPedido { get; set; }
        public string NumeroBooking { get; set; }
        public int FilialAtual { get; set; }
        public int TipoContainer { get; set; }
        public DateTime DataEmbarqueNavioInicial { get; set; }
        public DateTime DataEmbarqueNavioFinal { get; set; }



    }
}