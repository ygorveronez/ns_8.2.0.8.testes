using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.CTe
{
    public sealed class FiltroPesquisaAutorizacaoCTeLote
    {
        public int NumeroInicial { get; set; }
        public int NumeroFinal { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal> TipoModal { get; set; }
        public List<string> Status { get; set; }
        public int CodigoEmpresa { get; set; }
        public int CodigoPortoOrigem { get; set; }
        public int CodigoTerminalOrigem { get; set; }
        public int CodigoTerminalDestino { get; set; }
        public int CodigoViagem { get; set; }

        public string NumeroBooking { get; set; }
        public string NumeroOS { get; set; }
        public string NumeroControle { get; set; }
        public int CodigoContainer { get; set; }
        public string NumeroNF { get; set; }

        public List<Dominio.Enumeradores.StatusAverbacaoCTe> StatusAverbacao { get; set; }
    }
}
