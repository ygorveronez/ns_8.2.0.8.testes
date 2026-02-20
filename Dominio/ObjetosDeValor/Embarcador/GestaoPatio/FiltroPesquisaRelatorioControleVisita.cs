using System;

namespace Dominio.ObjetosDeValor.Embarcador.GestaoPatio;

public sealed class FiltroPesquisaRelatorioControleVisita
{
    public DateTime DataInicialEntrada { get; set; }
    public DateTime DataFinalEntrada { get; set; }
    public DateTime DataInicialSaida { get; set; }
    public DateTime DataFinalSaida { get; set; }
    public int Setor { get; set; }
    public int Autorizador { get; set; }
    public double CPF { get; set; }
}

