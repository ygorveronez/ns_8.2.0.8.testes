using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Veiculos
{
    public sealed class FiltroPesquisaRelatorioManutencao
    {
        public int CodigoTipoMovimento { get; set; }
        public List<int> CodigosNaturezaOperacao { get; set; }
        public int CodigoVeiculo { get; set; }
        public List<int> CodigosSegmento { get; set; }
        public List<int> CodigosEquipamento { get; set; }
        public List<int> CodigoPlacas { get; set; }
        public int CodigoEmpresa { get; set; }
        public double CnpjCpfFornecedor { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public List<int> CentrosResultado { get; set; }
        public List<int> Produtos { get; set; }
        public bool? ExibirApenasComVeiculoOuEquipamento { get; set; }
        public SituacaoDocumentoEntrada? SituacaoLancDocEntrada { get; set; }
        public List<int> CodigosVeiculo { get; set; }

    }
}
