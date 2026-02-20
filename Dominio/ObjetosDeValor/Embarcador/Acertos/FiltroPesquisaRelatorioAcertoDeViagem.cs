using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Acertos
{
    public class FiltroPesquisaRelatorioAcertoDeViagem
    {
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public DateTime DataInicialFechamento { get; set; }
        public DateTime DataFinalFechamento { get; set; }
        public SituacaoAcertoViagem Situacao { get; set; }
        public TipoMotorista TipoMotorista { get; set; }
        public SituacaoAtivoPesquisa StatusMotorista { get; set; }
        public bool UltimoAcerto { get; set; }
        public int AcertoViagem { get; set; }
        public int Motorista { get; set; }
        public int Segmento { get; set; }
        public int VeiculoTracao { get; set; }
        public int VeiculoReboque { get; set; }
    }
}
