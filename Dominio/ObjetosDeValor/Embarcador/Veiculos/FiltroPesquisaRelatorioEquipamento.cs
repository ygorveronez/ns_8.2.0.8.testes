using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Veiculos
{
    public sealed class FiltroPesquisaRelatorioEquipamento
    {
        public DateTime DataAquisicaoInicial { get; set; }
        public DateTime DataAquisicaoFinal { get; set; }
        public int AnoFabricacao { get; set; }
        public int CodigoModelo { get; set; }
        public int CodigoMarca { get; set; }
        public List<int> CodigosSegmento { get; set; }
        public SituacaoAtivoPesquisa Ativo { get; set; }
        public int CodigoVeiculo { get; set; }
        public int CodigoCentroResultado { get; set; }
        public int Neokohm { get; set; }
    }
}
