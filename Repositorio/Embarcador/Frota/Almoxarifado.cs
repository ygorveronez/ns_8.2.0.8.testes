using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;

namespace Repositorio.Embarcador.Frota
{
    public class Almoxarifado : RepositorioBase<Dominio.Entidades.Embarcador.Frota.Almoxarifado>
    {
        #region Construtores

        public Almoxarifado(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Frota.Almoxarifado> Consultar(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaAlmoxarifado filtrosPesquisa)
        {
            var consultaAlmoxarifado = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.Almoxarifado>();

            if (filtrosPesquisa.CodigoEmpresa > 0)
                consultaAlmoxarifado = consultaAlmoxarifado.Where(o => o.Empresa.Codigo == filtrosPesquisa.CodigoEmpresa);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                consultaAlmoxarifado = consultaAlmoxarifado.Where(o => o.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.SituacaoAtivo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
            {
                var ativo = filtrosPesquisa.SituacaoAtivo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo ? true : false;

                consultaAlmoxarifado = consultaAlmoxarifado.Where(o => o.Ativo == ativo);
            }

            return consultaAlmoxarifado;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Frota.Almoxarifado BuscarPorCodigo(int codigo)
        {
            var almoxarifado = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.Almoxarifado>()
                .Where(o => o.Codigo == codigo)
                .FirstOrDefault();

            return almoxarifado;
        }

        public List<Dominio.Entidades.Embarcador.Frota.Almoxarifado> BuscarPorCodigos(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.Almoxarifado>();

            query = query.Where(o => codigos.Contains(o.Codigo));

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Frota.Almoxarifado BuscarPrimeiroAlmoxarifado(int codigoEmpresa)
        {
            var almoxarifado = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.Almoxarifado>();

            if (codigoEmpresa > 0)
                almoxarifado = almoxarifado.Where(o => o.Empresa.Codigo == codigoEmpresa);

            return almoxarifado.FirstOrDefault(); ;
        }

        public List<Dominio.Entidades.Embarcador.Frota.Almoxarifado> Consultar(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaAlmoxarifado filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaAlmoxarifado = Consultar(filtrosPesquisa);

            return ObterLista(consultaAlmoxarifado, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaAlmoxarifado filtrosPesquisa)
        {
            var consultaAlmoxarifado = Consultar(filtrosPesquisa);

            return consultaAlmoxarifado.Count();
        }

        #endregion
    }
}
