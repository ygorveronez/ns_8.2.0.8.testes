using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;

namespace Repositorio.Embarcador.Logistica
{
    public class TermoQuitacao : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.TermoQuitacao.TermoQuitacao>
    {
        #region Construtores

        public TermoQuitacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Logistica.TermoQuitacao.TermoQuitacao> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaTermoQuitacao filtrosPesquisa)
        {
            var consultaTermoQuitacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.TermoQuitacao.TermoQuitacao>();

            if (filtrosPesquisa.Numero > 0)
                consultaTermoQuitacao = consultaTermoQuitacao.Where(o => o.Numero == filtrosPesquisa.Numero);

            if (filtrosPesquisa.CodigoTransportador > 0)
                consultaTermoQuitacao = consultaTermoQuitacao.Where(o => o.Transportador.Codigo == filtrosPesquisa.CodigoTransportador);

            if (filtrosPesquisa.DataBaseInicial.HasValue)
                consultaTermoQuitacao = consultaTermoQuitacao.Where(o => o.DataBase >= filtrosPesquisa.DataBaseInicial.Value);

            if (filtrosPesquisa.DataBaseLimite.HasValue)
                consultaTermoQuitacao = consultaTermoQuitacao.Where(o => o.DataBase <= filtrosPesquisa.DataBaseLimite.Value.Add(System.DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.Situacao.HasValue)
                consultaTermoQuitacao = consultaTermoQuitacao.Where(o => o.Situacao == filtrosPesquisa.Situacao);

            return consultaTermoQuitacao;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Logistica.TermoQuitacao.TermoQuitacao BuscarPorCodigo(int codigo)
        {
            var consultaTermoQuitacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.TermoQuitacao.TermoQuitacao>()
                .Where(avaria => avaria.Codigo == codigo);

            return consultaTermoQuitacao.FirstOrDefault();
        }

        public int BuscarProximoNumero()
        {
            var consultaTermoQuitacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.TermoQuitacao.TermoQuitacao>();
            int? ultimoNumero = consultaTermoQuitacao.Max(o => (int?)o.Numero);

            return ultimoNumero.HasValue ? (ultimoNumero.Value + 1) : 1;
        }

        public List<Dominio.Entidades.Embarcador.Logistica.TermoQuitacao.TermoQuitacao> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaTermoQuitacao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaTermoQuitacao = Consultar(filtrosPesquisa);

            consultaTermoQuitacao = consultaTermoQuitacao
                .Fetch(o => o.Transportador);

            return ObterLista(consultaTermoQuitacao, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaTermoQuitacao filtrosPesquisa)
        {
            var consultaTermoQuitacao = Consultar(filtrosPesquisa);

            return consultaTermoQuitacao.Count();
        }

        #endregion
    }
}
