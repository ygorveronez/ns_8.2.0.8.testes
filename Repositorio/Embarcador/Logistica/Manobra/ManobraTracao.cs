using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;

namespace Repositorio.Embarcador.Logistica
{
    public sealed class ManobraTracao : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.ManobraTracao>
    {
        #region Construtores

        public ManobraTracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        public IQueryable<Dominio.Entidades.Embarcador.Logistica.ManobraTracao> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaManobraTracao filtrosPesquisa)
        {
            var consultaManobraTracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ManobraTracao>();

            if (filtrosPesquisa.CodigoCentroCarregamento > 0)
                consultaManobraTracao = consultaManobraTracao.Where(o => o.CentroCarregamento.Codigo == filtrosPesquisa.CodigoCentroCarregamento);

            return consultaManobraTracao;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Logistica.ManobraTracao BuscarAtivaPorManobraAtual(int codigoManobra)
        {
            var consultaManobraTracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ManobraTracao>()
                .Where(o => (o.ManobraAtual.Codigo == codigoManobra) && (o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoManobraTracao.EmManobra));

            return consultaManobraTracao.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.ManobraTracao BuscarPorCodigo(int codigo)
        {
            var consultaManobraTracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ManobraTracao>()
                .Where(o => o.Codigo == codigo);

            return consultaManobraTracao.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.ManobraTracao BuscarPorManobraAtual(int codigoManobra)
        {
            var consultaManobraTracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ManobraTracao>()
                .Where(o => o.ManobraAtual.Codigo == codigoManobra);

            return consultaManobraTracao.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.ManobraTracao BuscarPorManobraReservada(int codigoManobra)
        {
            var consultaManobraTracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ManobraTracao>()
                .Where(o => o.ManobraReservada.Codigo == codigoManobra);

            return consultaManobraTracao.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.ManobraTracao BuscarPorTracao(int codigoTracao)
        {
            var consultaManobraTracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ManobraTracao>()
                .Where(o => o.Tracao.Codigo == codigoTracao);

            return consultaManobraTracao.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.ManobraTracao> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaManobraTracao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaManobraTracao = Consultar(filtrosPesquisa);

            return ObterLista(consultaManobraTracao, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaManobraTracao filtrosPesquisa)
        {
            var consultaManobraTracao = Consultar(filtrosPesquisa);

            return consultaManobraTracao.Count();
        }

        #endregion
    }
}
