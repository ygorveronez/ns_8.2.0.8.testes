using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Frota
{
    public sealed class FiltroPesquisaRelatorioServicoVeiculo
    {
        public List<int> CodigosServico { get; set; }
        public int CodigoEmpresa { get; set; }
        public int CodigoGrupoServico { get; set; }
        public SituacaoAtivoPesquisa Situacao { get; set; }
        public TipoManutencaoServicoVeiculo? TipoManutencao { get; set; }
        public MotivoServicoVeiculo? Motivo { get; set; }
    }
}
