using System;

namespace Dominio.ObjetosDeValor.Embarcador.Chamado
{
    public sealed class FiltroPesquisaControleChamadoTMS
    {
        public string CodigoCargaEmbarcador { get; set; }
        public int CodigoMotivoChamado { get; set; }
        public int CodigoVeiculo { get; set; }
        public int CodigoMotorista { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public int NumeroInicial { get; set; }
        public int NumeroFinal { get; set; }
        public int NumeroCTe { get; set; }
    }
}
