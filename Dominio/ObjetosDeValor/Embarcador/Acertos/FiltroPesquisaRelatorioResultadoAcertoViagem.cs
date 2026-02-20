using System;

namespace Dominio.ObjetosDeValor.Embarcador.Acertos
{
    public class FiltroPesquisaRelatorioResultadoAcertoViagem
    {
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public int Motorista { get; set; }
        public int SegmentoVeiculo { get; set; }
        public int VeiculoTracao { get; set; }
        public int VeiculoReboque { get; set; }
        public int GrupoPessoa { get; set; }
        public int ModeloVeiculo { get; set; }
    }
}
