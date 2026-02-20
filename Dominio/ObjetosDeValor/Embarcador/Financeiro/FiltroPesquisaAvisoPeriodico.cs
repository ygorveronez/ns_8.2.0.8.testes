using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class FiltroPesquisaAvisoPeriodico
    {
        public int NumeroAviso { get; set; }
        public SituacaoAvisoPeriodicoQuitacao? Situacao { get; set; }
        public DateTime? DataGeracaoInicial { get; set; }
        public DateTime? DataGeracaoFinal { get; set; }
        public DateTime? DataFinal { get; set; }
        public DateTime? DataInicial { get; set; }
        public int CodigoTransportador { get; set; }
    }
}
