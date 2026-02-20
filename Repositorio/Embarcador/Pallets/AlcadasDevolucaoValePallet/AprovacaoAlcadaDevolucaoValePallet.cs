using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pallets.AlcadasDevolucaoValePallet
{
    public class AprovacaoAlcadaDevolucaoValePallet : RegraAutorizacao.AprovacaoAlcada<
        Dominio.Entidades.Embarcador.Pallets.AlcadasDevolucaoValePallet.AprovacaoAlcadaDevolucaoValePallet,
        Dominio.Entidades.Embarcador.Pallets.AlcadasDevolucaoValePallet.RegraAutorizacaoDevolucaoValePallet,
        Dominio.Entidades.Embarcador.Pallets.DevolucaoValePallet
    >
    {
        #region Construtores

        public AprovacaoAlcadaDevolucaoValePallet(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Pallets.DevolucaoValePallet> Consultar(Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaDevolucaoValePalletAprovacao filtrosPesquisa)
        {
            var consultaDevolucaoValePallet = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.DevolucaoValePallet>();
            var consultaAlcadaDevolucaoValePallet = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.AlcadasDevolucaoValePallet.AprovacaoAlcadaDevolucaoValePallet>()
                .Where(o => !o.Bloqueada);

            if (filtrosPesquisa.Codigo > 0)
                consultaDevolucaoValePallet = consultaDevolucaoValePallet.Where(o => o.Codigo == filtrosPesquisa.Codigo);

            if (filtrosPesquisa.CodigoFilial > 0)
                consultaDevolucaoValePallet = consultaDevolucaoValePallet.Where(o => o.Filial.Codigo == filtrosPesquisa.CodigoFilial);

            if (filtrosPesquisa.CodigoSetor > 0)
                consultaDevolucaoValePallet = consultaDevolucaoValePallet.Where(o => o.Setor.Codigo == filtrosPesquisa.CodigoSetor);

            if (filtrosPesquisa.CodigoTransportador > 0)
                consultaDevolucaoValePallet = consultaDevolucaoValePallet.Where(o => o.ValePallet.Devolucao.Transportador.Codigo == filtrosPesquisa.CodigoTransportador);

            if (filtrosPesquisa.DataInicial.HasValue)
                consultaDevolucaoValePallet = consultaDevolucaoValePallet.Where(o => o.Data >= filtrosPesquisa.DataInicial.Value);

            if (filtrosPesquisa.DataLimite.HasValue)
                consultaDevolucaoValePallet = consultaDevolucaoValePallet.Where(o => o.Data <= filtrosPesquisa.DataLimite.Value.Add(System.DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDevolucaoValePallet.Todas)
                consultaDevolucaoValePallet = consultaDevolucaoValePallet.Where(o => o.Situacao == filtrosPesquisa.Situacao);

            if (filtrosPesquisa.CodigoUsuario > 0)
                consultaAlcadaDevolucaoValePallet = consultaAlcadaDevolucaoValePallet.Where(o => o.Usuario.Codigo == filtrosPesquisa.CodigoUsuario);

            if (filtrosPesquisa.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDevolucaoValePallet.AguardandoAprovacao)
                consultaAlcadaDevolucaoValePallet = consultaAlcadaDevolucaoValePallet.Where(o => o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente);

            return consultaDevolucaoValePallet.Where(o => consultaAlcadaDevolucaoValePallet.Where(a => a.OrigemAprovacao.Codigo == o.Codigo).Any());
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Pallets.DevolucaoValePallet> Consultar(Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaDevolucaoValePalletAprovacao filtrosPesquisa, string propriedadeOrdenacao, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var devolucoesValePallet = Consultar(filtrosPesquisa);

            return ObterLista(devolucoesValePallet, propriedadeOrdenacao, direcaoOrdenacao, inicioRegistros, maximoRegistros);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaDevolucaoValePalletAprovacao filtrosPesquisa)
        {
            var devolucoesValePallet = Consultar(filtrosPesquisa);

            return devolucoesValePallet.Count();
        }

        #endregion
    }
}
