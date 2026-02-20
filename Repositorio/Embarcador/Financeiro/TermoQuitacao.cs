using Dominio.ObjetosDeValor.Embarcador.Financeiro;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Financeiro
{
    public class TermoQuitacao : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro>
    {
        #region Construtor
        public TermoQuitacao(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        #endregion

        #region Metodos Privado

        private IQueryable<Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro> Consulta(FiltroPesquisaTermoQuitacao filtroPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro>();
            var subquery = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AprovacaoAlcadaEstornoProvisao>();

            if (filtroPesquisa.Transportador > 0)
                query = query.Where(x => x.Transportador.Codigo == filtroPesquisa.Transportador);

            if (filtroPesquisa.NumeroTermo > 0)
                query = query.Where(x => x.NumeroTermo == filtroPesquisa.NumeroTermo);

            if (filtroPesquisa.SitaucaoTermoQuitacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoTermoQuitacaoFinanceiro.Todas)
                query = query.Where(x => x.SituacaoTermoQuitacao == filtroPesquisa.SitaucaoTermoQuitacao);

            if (filtroPesquisa.SituacaoAprovacaoTransportador == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAprovacaoTermoQuitacaoTransportador.Aprovado)
                query = query.Where(x => x.SituacaoAprovacaoTransportador == filtroPesquisa.SituacaoAprovacaoTransportador);

            if (filtroPesquisa.SituacaoAprovacaoTransportador == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAprovacaoTermoQuitacaoTransportador.Reprovado)
                query = query.Where(x => x.SituacaoAprovacaoTransportador == filtroPesquisa.SituacaoAprovacaoTransportador);

            if (filtroPesquisa.SituacaoAprovacaoTransportador == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAprovacaoTermoQuitacaoTransportador.Pendente)
                query = query.Where(x => x.SituacaoAprovacaoTransportador != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAprovacaoTermoQuitacaoTransportador.Reprovado && x.SituacaoAprovacaoTransportador != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAprovacaoTermoQuitacaoTransportador.Aprovado);

            if (filtroPesquisa.DataInicio.HasValue)
                query = query.Where(x => x.DataInicial >= filtroPesquisa.DataInicio.Value);

            if (filtroPesquisa.DataFinal.HasValue)
                query = query.Where(x => x.DataFinal < filtroPesquisa.DataFinal.Value.AddDays(1));

            if (filtroPesquisa.DataVigenciaInicial != System.DateTime.MinValue)
                query = query.Where(x => filtroPesquisa.DataVigenciaInicial <= x.DataInicial);

            if (filtroPesquisa.DataVigenciaFinal != System.DateTime.MinValue)
                query = query.Where(x => x.DataFinal <= filtroPesquisa.DataVigenciaFinal);

            if (filtroPesquisa.SitaucaoAprovacaoProvisao.HasValue)
                query = query.Where(x => subquery.Any(o => o.TermoQuitacaoFinanceiro.Codigo == x.Codigo && o.Situacao == filtroPesquisa.SitaucaoAprovacaoProvisao.Value));

            if (filtroPesquisa.ProvisaoPendente.HasValue)
            {
                if (filtroPesquisa.ProvisaoPendente.Value)
                    query = query.Where(x => subquery.Any(o => o.TermoQuitacaoFinanceiro.Codigo == x.Codigo && o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente));
                else
                    query = query.Where(x => subquery.Any(o => o.TermoQuitacaoFinanceiro.Codigo == x.Codigo && o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Aprovada));
            }

            return query;
        }
        #endregion

        #region Metodos Publicos

        public List<Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro> Consultar(FiltroPesquisaTermoQuitacao filtroPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var consulta = Consulta(filtroPesquisa);
            return ObterLista(consulta, parametroConsulta);
        }

        public int ContarConsulta(FiltroPesquisaTermoQuitacao filtroPesquisa)
        {
            var consulta = Consulta(filtroPesquisa);
            return consulta.Count();
        }

        public Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro BuscarUltimoTermoGeradoPorTransportador(int codigoTransportador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro>();
            query = from obj in query where obj.Transportador.Codigo == codigoTransportador orderby obj.Codigo descending select obj;

            return query.FirstOrDefault();
        }
        public int ObterUltimoNumero()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro>();

            var result = from obj in query select obj;

            int? retorno = result.Max(o => (int?)o.NumeroTermo);

            return retorno.HasValue ? retorno.Value : 0;
        }
        public Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro BuscarPorCodigo(int codigoTermo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro>();
            query = from obj in query where obj.Codigo == codigoTermo select obj;

            return query.FirstOrDefault();
        }


        #endregion
    }
}
