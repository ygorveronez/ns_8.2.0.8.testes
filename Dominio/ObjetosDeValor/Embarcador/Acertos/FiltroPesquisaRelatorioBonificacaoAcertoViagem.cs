using System;

namespace Dominio.ObjetosDeValor.Embarcador.Acertos
{
    public class FiltroPesquisaRelatorioBonificacaoAcertoViagem
    {
        public DateTime DataInicialAcerto { get; set; }
        public DateTime DataFinalAcerto { get; set; }
        public int NumeroAcerto { get; set; }
        public int CodigoMotorista { get; set; }
        public int TipoBonificacao { get; set; }
        public DateTime DataInicialBonificacao { get; set; }
        public DateTime DataFinalBonificacao { get; set; }
    }
}
