using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.NFS
{
    public class LancamentoNFSManualDesconto : RepositorioBase<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManualDesconto>
    {
        #region Construtores

        public LancamentoNFSManualDesconto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.Carga> BuscarCargasParaLancamentoNFSManualDesconto(Dominio.ObjetosDeValor.Embarcador.NFS.FiltroPesquisaCargaDescontoParaEmissaoNFSManual filtrosPesquisa)
        {
            var consultaLancamentoNFSManualDesconto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManualDesconto>()
                .Where(o => o.Cancelado == false);

            var consultaCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Carga>()
                .Where(o =>
                    filtrosPesquisa.SituacoesCargasPermitidas.Contains(o.SituacaoCarga) &&
                    o.ValorFreteResidual < 0m &&
                    !consultaLancamentoNFSManualDesconto.Any(d => d.Carga.Codigo == o.Codigo)
                );

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoCargaEmbarcador))
                consultaCarga = consultaCarga.Where(o => o.CodigoCargaEmbarcador == filtrosPesquisa.CodigoCargaEmbarcador);

            if (filtrosPesquisa.CodigoTransportador > 0)
                consultaCarga = consultaCarga.Where(o => o.Empresa.Codigo == filtrosPesquisa.CodigoTransportador);

            if (filtrosPesquisa.Codigosfilial?.Count > 0)
                consultaCarga = consultaCarga.Where(o => filtrosPesquisa.Codigosfilial.Contains(o.Filial.Codigo));

            if (filtrosPesquisa.CodigoFilial > 0)
                consultaCarga = consultaCarga.Where(o => o.Filial.Codigo == filtrosPesquisa.CodigoFilial);

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
                consultaCarga = consultaCarga.Where(o => o.TipoOperacao.Codigo == filtrosPesquisa.CodigoTipoOperacao);

            if (filtrosPesquisa.DataInicial.HasValue)
                consultaCarga = consultaCarga.Where(o => o.DataCriacaoCarga >= filtrosPesquisa.DataInicial.Value.Date);

            if (filtrosPesquisa.DataFinal.HasValue)
                consultaCarga = consultaCarga.Where(o => o.DataCriacaoCarga < filtrosPesquisa.DataFinal.Value.AddDays(1).Date);

            if (filtrosPesquisa.Moeda.HasValue)
                consultaCarga = consultaCarga.Where(o => o.Moeda == filtrosPesquisa.Moeda);

            return consultaCarga
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> BuscarCargasPedidoPorLancamentoNFSManual(int codigoLancamentoNFSManual)
        {
            var consultaLancamentoNFSManualDesconto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManualDesconto>()
                .Where(o => o.LancamentoNFSManual.Codigo == codigoLancamentoNFSManual);

            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o => consultaLancamentoNFSManualDesconto.Any(d => d.Carga.Codigo == o.Carga.Codigo));

            return consultaCargaPedido
                .Fetch(o => o.Pedido)
                .Fetch(o => o.Carga)
                .ToList();
        }

        public void CancelarPorLancamentoNFSManual(int codigoLancamentoNFSManual)
        {
            if (UnitOfWork.IsActiveTransaction())
            {
                UnitOfWork.Sessao
                    .CreateQuery(
                        @"update LancamentoNFSManualDesconto
                             set Cancelado = 1
                           where LancamentoNFSManual.Codigo = :codigoLancamentoNFSManual"
                    )
                    .SetParameter("codigoLancamentoNFSManual", codigoLancamentoNFSManual)
                    .ExecuteUpdate();
            }
            else
            {
                try
                {
                    UnitOfWork.Start();

                    UnitOfWork.Sessao
                        .CreateQuery(
                            @"update LancamentoNFSManualDesconto
                                    set Cancelado = 1
                                where LancamentoNFSManual.Codigo = :codigoLancamentoNFSManual"
                        )
                        .SetParameter("codigoLancamentoNFSManual", codigoLancamentoNFSManual)
                        .ExecuteUpdate();

                    UnitOfWork.CommitChanges();
                }
                catch
                {
                    UnitOfWork.Rollback();
                    throw;
                }
            }
        }

        public List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManualDesconto> Consultar(int codigoLancamentoNFSManual, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaLancamentoNFSManualDesconto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManualDesconto>()
                .Where(o => o.LancamentoNFSManual.Codigo == codigoLancamentoNFSManual);

            consultaLancamentoNFSManualDesconto = consultaLancamentoNFSManualDesconto
                .Fetch(o => o.Carga);

            return ObterLista(consultaLancamentoNFSManualDesconto, parametrosConsulta);
        }

        public int ContarConsulta(int codigoLancamentoNFSManual)
        {
            var consultaLancamentoNFSManualDesconto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManualDesconto>()
                .Where(o => o.LancamentoNFSManual.Codigo == codigoLancamentoNFSManual);

            return consultaLancamentoNFSManualDesconto.Count();
        }

        #endregion
    }
}
