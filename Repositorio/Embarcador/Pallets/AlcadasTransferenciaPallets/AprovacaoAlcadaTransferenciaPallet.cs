using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pallets.AlcadasTransferenciaPallets
{
    public class AprovacaoAlcadaTransferenciaPallet : RegraAutorizacao.AprovacaoAlcada<
        Dominio.Entidades.Embarcador.Pallets.AlcadasTransferencia.AprovacaoAlcadaTransferenciaPallet,
        Dominio.Entidades.Embarcador.Pallets.AlcadasTransferencia.RegraAutorizacaoTransferenciaPallet,
        Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPallet
    >
    {
        #region Construtores

        public AprovacaoAlcadaTransferenciaPallet(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPallet> Consultar(Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaTransferenciaPalletAprovacao filtrosPesquisa)
        {
            var consultaTransferencia = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPallet>();
            var consultaAlcadaTransferencia = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.AlcadasTransferencia.AprovacaoAlcadaTransferenciaPallet>()
                .Where(o => !o.Bloqueada);

            if (filtrosPesquisa.Codigo > 0)
                consultaTransferencia = consultaTransferencia.Where(o => o.Codigo == filtrosPesquisa.Codigo);

            if (filtrosPesquisa.CodigoFilial > 0)
                consultaTransferencia = consultaTransferencia.Where(o => o.Solicitacao.Filial.Codigo == filtrosPesquisa.CodigoFilial);

            if (filtrosPesquisa.CodigoSetor > 0)
                consultaTransferencia = consultaTransferencia.Where(o => o.Solicitacao.Setor.Codigo == filtrosPesquisa.CodigoSetor);

            if (filtrosPesquisa.CodigoTurno > 0)
                consultaTransferencia = consultaTransferencia.Where(o => o.Solicitacao.Turno.Codigo == filtrosPesquisa.CodigoTurno);

            if (filtrosPesquisa.DataInicial.HasValue)
                consultaTransferencia = consultaTransferencia.Where(o => o.Solicitacao.Data >= filtrosPesquisa.DataInicial.Value);

            if (filtrosPesquisa.DataLimite.HasValue)
                consultaTransferencia = consultaTransferencia.Where(o => o.Solicitacao.Data <= filtrosPesquisa.DataLimite.Value.Add(System.DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoTransferenciaPallet.Todas)
                consultaTransferencia = consultaTransferencia.Where(o => o.Situacao == filtrosPesquisa.Situacao);

            if (filtrosPesquisa.CodigoUsuario > 0)
                consultaAlcadaTransferencia = consultaAlcadaTransferencia.Where(o => o.Usuario.Codigo == filtrosPesquisa.CodigoUsuario);

            if (filtrosPesquisa.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoTransferenciaPallet.AguardandoAprovacao)
                consultaAlcadaTransferencia = consultaAlcadaTransferencia.Where(o => o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente);

            return consultaTransferencia.Where(o => consultaAlcadaTransferencia.Where(a => a.OrigemAprovacao.Codigo == o.Codigo).Any());
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPallet> Consultar(Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaTransferenciaPalletAprovacao filtrosPesquisa, string propriedadeOrdenacao, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var transferencias = Consultar(filtrosPesquisa);

            return ObterLista(transferencias, propriedadeOrdenacao, direcaoOrdenacao, inicioRegistros, maximoRegistros);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaTransferenciaPalletAprovacao filtrosPesquisa)
        {
            var transferencias = Consultar(filtrosPesquisa);

            return transferencias.Count();
        }

        #endregion
    }
}
