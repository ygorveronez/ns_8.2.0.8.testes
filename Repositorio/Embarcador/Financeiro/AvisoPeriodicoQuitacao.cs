using Dominio.ObjetosDeValor.Embarcador.Financeiro;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Financeiro
{
    public class AvisoPeriodicoQuitacao : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.AvisoPeriodicoQuitacao>
    {
        #region Construtor
        public AvisoPeriodicoQuitacao(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion


        #region Metodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Financeiro.AvisoPeriodicoQuitacao> Consulta(FiltroPesquisaAvisoPeriodico filtroPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.AvisoPeriodicoQuitacao>();

            if (filtroPesquisa.CodigoTransportador > 0)
                query = query.Where(x => x.Transportador.Codigo == filtroPesquisa.CodigoTransportador);

            if (filtroPesquisa.NumeroAviso > 0)
                query = query.Where(x => x.Numero == filtroPesquisa.NumeroAviso);

            if (filtroPesquisa.Situacao.HasValue)
                query = query.Where(x => x.SituacaoAvisoPeriodico == filtroPesquisa.Situacao.Value);

            if (filtroPesquisa.DataInicial.HasValue)
                query = query.Where(x => x.DataInicial >= filtroPesquisa.DataInicial.Value);

            if (filtroPesquisa.DataFinal.HasValue)
                query = query.Where(x => x.DataFinal <= filtroPesquisa.DataFinal.Value);

            if (filtroPesquisa.DataGeracaoInicial.HasValue)
                query = query.Where(x => x.DataCriacao >= filtroPesquisa.DataGeracaoInicial.Value);

            if (filtroPesquisa.DataGeracaoFinal.HasValue)
                query = query.Where(x => x.DataCriacao <= filtroPesquisa.DataGeracaoFinal.Value.AddDays(1));

            return query;
        }
        #endregion



        #region Metodos Públicos
        public List<Dominio.Entidades.Embarcador.Financeiro.AvisoPeriodicoQuitacao> Consultar(FiltroPesquisaAvisoPeriodico filtroPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var consulta = Consulta(filtroPesquisa);
            return ObterLista(consulta, parametroConsulta);
        }

        public int ContarConsulta(FiltroPesquisaAvisoPeriodico filtroPesquisa)
        {
            var consulta = Consulta(filtroPesquisa);
            return consulta.Count();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.AvisoPeriodicoQuitacao> BuscarPorCodigos(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.AvisoPeriodicoQuitacao>();

            var result = from o in query where codigos.Contains(o.Codigo) select o;

            return result.ToList();

        }

        public Dominio.Entidades.Embarcador.Financeiro.AvisoPeriodicoQuitacao BuscarUltimoAvisoGeradoPorTransportador(int codigoTransportador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.AvisoPeriodicoQuitacao>();
            query = from obj in query where obj.Transportador.Codigo == codigoTransportador orderby obj.Codigo descending select obj;

            return query.FirstOrDefault();
        }
        public int BuscarUltimoNumeroGerado()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.AvisoPeriodicoQuitacao>();
            var ultimoNumero = query.OrderByDescending(x => x.Numero).Select(x => x.Numero).FirstOrDefault();

            return ultimoNumero != null && ultimoNumero > 0 ? ultimoNumero : 0;
        }
        #endregion

    }
}
