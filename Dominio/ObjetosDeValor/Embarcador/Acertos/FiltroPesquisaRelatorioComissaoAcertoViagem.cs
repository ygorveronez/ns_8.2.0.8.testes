using System;

namespace Dominio.ObjetosDeValor.Embarcador.Acertos
{
    public class FiltroPesquisaRelatorioComissaoAcertoViagem
    {
        public bool ExibirOcorrencias { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public DateTime DataVencimentoInicial { get; set; }
        public DateTime DataVencimentoFinal { get; set; }
        public int CodigoAcertoViagem { get; set; }
        public int CodigoMotorista { get; set; }
        public int CodigoSegmento { get; set; }
    }
}
