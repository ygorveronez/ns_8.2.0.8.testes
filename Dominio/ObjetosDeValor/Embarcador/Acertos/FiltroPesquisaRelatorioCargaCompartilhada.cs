using System;

namespace Dominio.ObjetosDeValor.Embarcador.Acertos
{
    public class FiltroPesquisaRelatorioCargaCompartilhada
    {
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public int Carga { get; set; }
        public int NumeroAcerto { get; set; }
        public int CodigoMotorista { get; set; }
    }
}
