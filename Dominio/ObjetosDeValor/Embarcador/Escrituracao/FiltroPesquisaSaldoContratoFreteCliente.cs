using System;

namespace Dominio.ObjetosDeValor.Embarcador.Escrituracao
{
    public class FiltroPesquisaSaldoContratoFreteCliente
    {
        public int CodigoContrato { get; set; }

        public int NumeroCarga { get; set; }

        public double Cliente { get; set; }

        public int Transportador { get; set; }

        public DateTime DataInicialContrato { get; set; }
        public DateTime DataFinalContrato { get; set; }

    }
}
