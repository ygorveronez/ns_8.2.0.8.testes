using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Acertos
{
    public class FiltroPesquisaRelatorioTempoDeViagem
    {
        public int Motorista { get; set; }
        public int Veiculo { get; set; }
        public DateTime DataInicial{ get; set; }
        public DateTime DataFinal { get; set; }
        public SituacaoAcertoViagem Situacao { get; set; }
    }
}
