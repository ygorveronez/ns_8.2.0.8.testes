using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Frota
{
    public class FiltroRelatorioDespesaDetalhadaOrdemServico
    {
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public int NumeroOS { get; set; }
        public List<int> Veiculo { get; set; }
        public List<int> ModeloVeiculo { get; set; }
        public List<int> MarcaVeiculo { get; set; }
        public int Empresa { get; set; }
        public List<long> TipoOrdemServico { get; set; }
        public List<double> LocalManutencao { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOrdemServicoFrota> Situacoes { get; set; }
        public List<int> Servico { get; set; }
        public List<int> Produto { get; set; }
        public List<int> Equipamento { get; set; }
        public int Responsavel { get; set; }
        public int CodigoGrupoProduto { get; set; }
        public List<int> CentroResultado { get; set; }
        public List<int> Mecanicos { get; set; }
    }
}
