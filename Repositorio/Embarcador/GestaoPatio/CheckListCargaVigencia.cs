using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.GestaoPatio
{
    public class CheckListCargaVigencia : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaVigencia>
    {
        #region Construtores

        public CheckListCargaVigencia(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public CheckListCargaVigencia(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaVigencia> Consultar(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaCheckListVigencia filtrosPesquisa)
        {
            var consultaChecklist = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaVigencia>();

            if (filtrosPesquisa.DataInicial.HasValue)
                consultaChecklist = consultaChecklist.Where(o => o.DataFimVigencia.Date >= filtrosPesquisa.DataInicial);

            if (filtrosPesquisa.DataFinal.HasValue)
                consultaChecklist = consultaChecklist.Where(o => o.DataFimVigencia.Date < filtrosPesquisa.DataFinal.Value.AddDays(1));

            if (filtrosPesquisa.CodigoFilial > 0)
                consultaChecklist = consultaChecklist.Where(o => o.Filial.Codigo == filtrosPesquisa.CodigoFilial);

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
                consultaChecklist = consultaChecklist.Where(o => o.TipoOperacao.Codigo == filtrosPesquisa.CodigoTipoOperacao);

            if (filtrosPesquisa.Ativo.HasValue)
                consultaChecklist = consultaChecklist.Where(o => o.Ativo == filtrosPesquisa.Ativo);

            return consultaChecklist;
        }

        #endregion Métodos Privados

        #region Métodos Públicos Consulta

        public Task<List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaVigencia>> ConsultarAsync(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaCheckListVigencia filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaChecklist = Consultar(filtrosPesquisa);

            return ObterListaAsync(consultaChecklist, parametrosConsulta);
        }

        public Task<int> ContarConsultaAsync(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaCheckListVigencia filtrosPesquisa)
        {
            var consultaChecklist = Consultar(filtrosPesquisa);

            return consultaChecklist.CountAsync(CancellationToken);
        }

        #endregion Métodos Públicos Consulta

        #region Métodos Públicos

        public Task<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaVigencia> BuscarPorFilialETipoOperacaoAsync(int codigoFilial, int codigoTipoOperacao)
        {
            var consultaCheckListCargaVigencia = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaVigencia>()
                .Where(o => o.Ativo && o.Filial.Codigo == codigoFilial && (o.TipoOperacao == null || o.TipoOperacao.Codigo == codigoTipoOperacao));

            return consultaCheckListCargaVigencia
                .OrderBy(vigencia => vigencia.TipoOperacao == null)
                .ThenByDescending(vigencia => vigencia.DataFimVigencia)
                .FirstOrDefaultAsync(CancellationToken);
        }

        #endregion Métodos Públicos
    }
}
