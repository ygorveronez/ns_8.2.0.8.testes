using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Frota
{
    public class FiltroRelatorioOrdemServico
    {
        public int? NumeroInicial { get; set; }

        public int? NumeroFinal { get; set; }

        public DateTime? DataInicial { get; set; }

        public DateTime? DataFinal { get; set; }

        public List<Enumeradores.SituacaoOrdemServicoFrota> Situacao { get; set; }

        public List<Enumeradores.TipoManutencaoOrdemServicoFrota> TipoManutencao { get; set; }

        public List<int> Veiculos { get; set; }

        public List<int> Motoristas { get; set; }

        public List<double> LocaisManutencao { get; set; }

        public List<int> Servicos { get; set; }

        public List<long> Tipos { get; set; }

        public List<int> Equipamentos { get; set; }

        public Enumeradores.TipoOficina? TipoOrdemServico { get; set; }

        public int MarcaVeiculo { get; set; }

        public int ModeloVeiculo { get; set; }

        public List<int> GrupoServicos { get; set; }

        public List<int> CentroResultados { get; set; }

        public List<int> Segmentos { get; set; }

        public List<int> CidadesPessoa { get; set; }

        public List<string> UFsPessoa { get; set; }

        public int OperadorLancamentoDocumento { get; set; }

        public int OperadorFinalizouDocumento { get; set; }

        public DateTime? DataInicialInclusao { get; set; }

        public DateTime? DataFinalInclusao { get; set; }

        public List<int> Mecanicos { get; set; }

        public int TempoPrevisto { get; set; }

        public int TempoExecutado { get; set; }

        public DateTime? DataInicialLimiteExecucao { get; set; }

        public DateTime? DataFinalLimiteExecucao { get; set; }

        public PrioridadeOrdemServico Prioridade { get; set; }

        public DateTime? DataLiberacaoInicio { get; set; }

        public DateTime? DataLiberacaoFim { get; set; }

        public DateTime? DataFechamentoInicio { get; set; }

        public DateTime? DataFechamentoFim { get; set; }

        public DateTime? DataReaberturaInicio { get; set; }

        public DateTime? DataReaberturaFim { get; set; }

        public List<int> CodigosProdutoTMS { get; set; }

        public List<int> CodigosGrupoProdutoTMS{ get; set; }
    }
}