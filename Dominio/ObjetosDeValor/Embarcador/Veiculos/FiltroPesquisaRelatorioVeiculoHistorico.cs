using System;

namespace Dominio.ObjetosDeValor.Embarcador.Veiculos
{
    public class FiltroPesquisaRelatorioVeiculoHistorico
    {
        public int CodigoVeiculo { get; set; }
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
    }
}
