using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Container
{
    public sealed class FiltroPesquisaRelatorioHistoricoMovimentacaoContainer
    {
        public string Carga { get; set; }
        public string Container { get; set; }
        public StatusColetaContainer? SituacaoContainer { get; set; }
        public DateTime DataInicialColeta { get; set; }
        public DateTime DataFinalColeta { get; set; }
        public double LocalEsperaVazio { get; set; }
        public double LocalAtual { get; set; }
        public double LocalColeta { get; set; }
        public int DiasPosseInicial { get; set; }
        public int DiasPosseFinal { get; set; }
        public DateTime DataPorto { get; set; }
        public DateTime DataMovimentacao { get; set; }
        public string NumeroEXP { get; set; }
        public string NumeroPedido { get; set; }
        public string NumeroBooking { get; set; }
        public int Filial { get; set; }
        public int TipoContainer { get; set; }
    }
}