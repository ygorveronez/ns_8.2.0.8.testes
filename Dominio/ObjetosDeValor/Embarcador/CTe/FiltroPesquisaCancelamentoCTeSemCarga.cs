using System;

namespace Dominio.ObjetosDeValor.Embarcador.CTe
{
    public class FiltroPesquisaCancelamentoCTeSemCarga
    {
       
        public int NumeroInicial { get; set; }
        public int NumeroFinal { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCTeSemCarga StatusCancelamento { get; set; }
    }
}
