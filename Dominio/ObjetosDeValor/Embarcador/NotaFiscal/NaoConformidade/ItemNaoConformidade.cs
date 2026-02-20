using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.NotaFiscal
{
    public sealed class ItemNaoConformidade
    {
        public int Codigo { get; set; }

        public string Descricao { get; set; }

        public Enumeradores.GrupoNC Grupo { get; set; }

        public Enumeradores.SubGrupoNC SubGrupo { get; set; }

        public Enumeradores.AreaNC Area { get; set; }

        public Enumeradores.TipoRegraNaoConformidade TipoRegra { get; set; }

        public List<ItemNaoConformidadeParticipantes> Participantes { get; set; }

        public List<ItemNaoConformidadeTiposOperacao> TiposOperacao { get; set; }

        public List<ItemNaoConformidadeCFOP> CFOP { get; set; }
        public List<ItemNaoConformidadeFilial> Filial { get; set; }
        public List<ItemNaoConformidadeFornecedor> Fornecedor { get; set; }

    }
}
