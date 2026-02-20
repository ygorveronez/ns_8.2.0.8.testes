using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public sealed class FiltroPesquisaPedidoCliente
    {
        #region Propriedades

        public int CodigoGrupoPessoaClienteFornecedor { get; set; }

        public double CpfCnpjClienteFornecedor { get; set; }

        public DateTime? DataInicial { get; set; }

        public DateTime? DataLimite { get; set; }

        public string DescricaoFiltro { get; set; }

        public List<Enumeradores.SituacaoAcompanhamentoPedido> SituacoesAcompanhamentoPedido { get; set; }

        public AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware? TipoServicoMultisoftware { get; set; }

        public Enumeradores.SituacaoAcompanhamentoPedido? SituacaoAcompanhamentoPedido { get; set; }

        public List<int> CodigosTiposOperacao { get; set; }

        public List<int> ListaVendedor { get; set; }

        public List<int> ListaCodigosGruposPessoaPortalAcesso { get; set; }

        public string CodigoVendedor { get; set; }

        #endregion Propriedades

        #region Propriedades com Regras

        #endregion Propriedades com Regras
    }
}
