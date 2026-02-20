using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Frota
{
    public class FiltroRelatorioManutencaoVeiculo
    {
        public List<int> CodigosServico { get; set; }
        public int CodigoVeiculo { get; set; }
        public int CodigoEmpresa { get; set; }
        public int CodigoEquipamento { get; set; }
        public bool VisualizarServicosPendentesManutencao { get; set; }
        public bool VisualizarSomenteServicosExecutadosAnteriormente { get; set; }
        public List<TipoManutencaoServicoVeiculo> TiposManutencao { get; set; }
        public string PropriedadeVeiculo { get; set; } //P - PROPRIO | T - TERCEIRO
        public double CnpjCpfLocalManutencao { get; set; }
        public int KMAtual { get; set; }
        public int HorimetroAtual { get; set; }
        public bool UtilizarValidadeServicoPeloGrupoServicoOrdemServico { get; set; }
        public bool VisualizarVeiculosEquipamentosAcoplados { get; set; }
        public List<int> CodigosReboques { get; set; }
        public List<int> CodigosEquipamentosAcoplados { get; set; }
        public int CodigoModeloVeiculo { get; set; }
        public int CodigoMarcaVeiculo { get; set; }
        public int CodigoModeloEquipamento { get; set; }
        public int CodigoMarcaEquipamento { get; set; }
        public List<int> CodigosSegmentoVeiculo { get; set; }
        public int CodigoCentroResultado { get; set; }
        public int CodigoFuncionarioResponsavel { get; set; }
        public bool VisualizarSomenteVeiculosAtivos { get; set; }
    }
}
