using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;

namespace Repositorio.Embarcador.Logistica
{
    public sealed class ManobraAcao : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.ManobraAcao>
    {
        #region Construtores

        public ManobraAcao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        public IQueryable<Dominio.Entidades.Embarcador.Logistica.ManobraAcao> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaManobraAcao filtrosPesquisa)
        {
            var consultaManobraAcao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ManobraAcao>();

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                consultaManobraAcao = consultaManobraAcao.Where(o => o.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.CodigoCentroCarregamento > 0)
            {
                var consultaCentroCarregamentoManobraAcao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoManobraAcao>()
                    .Where(o => o.CentroCarregamento.Codigo == filtrosPesquisa.CodigoCentroCarregamento);

                consultaManobraAcao = consultaManobraAcao.Where(o => consultaCentroCarregamentoManobraAcao.Where(obj => obj.Acao.Codigo == o.Codigo).Any());
            }

            if (filtrosPesquisa.Tipo.HasValue)
                consultaManobraAcao = consultaManobraAcao.Where(o => o.Tipo == filtrosPesquisa.Tipo.Value);

            return consultaManobraAcao;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Logistica.ManobraAcao BuscarPorCodigo(int codigo)
        {
            var consultaManobraAcao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ManobraAcao>()
                .Where(o => o.Codigo == codigo);

            return consultaManobraAcao.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.ManobraAcao> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaManobraAcao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaManobraAcao = Consultar(filtrosPesquisa);

            return ObterLista(consultaManobraAcao, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaManobraAcao filtrosPesquisa)
        {
            var consultaManobraAcao = Consultar(filtrosPesquisa);

            return consultaManobraAcao.Count();
        }

        #endregion
    }
}
