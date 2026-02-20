using System;

namespace Dominio.ObjetosDeValor.Embarcador.Veiculos
{
    public class FiltroPesquisaReceitaDespesa
    {
        public int CodigoVeiculo { get; set; }
        public int CodigoSegmentoVeiculo { get; set; }
        public int CodigoModeloVeicular { get; set; }
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        
    }
}
