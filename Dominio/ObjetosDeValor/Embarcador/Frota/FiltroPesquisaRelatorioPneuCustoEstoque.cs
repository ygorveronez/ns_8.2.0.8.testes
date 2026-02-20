using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Frota
{
    public class FiltroPesquisaRelatorioPneuCustoEstoque
    {
        public DateTime DataAquisicaoInicial { get; set; }
        public DateTime DataAquisicaoFinal { get; set; }
        public VidaPneu? Vida { get; set; }
        public int CodigoBandaRodagem { get; set; }
        public int CodigoDimensao { get; set; }
        public int CodigoMarca { get; set; }
        public int CodigoModelo { get; set; }
        public int CodigoPneu { get; set; }
        public int CodigoServicoVeiculoFrota { get; set; }
        public int CodigoVeiculo { get; set; }
        public int CodigoAlmoxarifado { get; set; }
        public List<EstadoPneu> EstadoAtualPneu { get; set; }
        public List<SituacaoPneu> Situacao { get; set; }
    }
}
