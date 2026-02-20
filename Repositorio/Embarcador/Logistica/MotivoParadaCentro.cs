using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class MotivoParadaCentro : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.MotivoParadaCentro>
    {
        public MotivoParadaCentro(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Logistica.MotivoParadaCentro> Consultar((int CentroCarregamento, string Descricao, SituacaoAtivoPesquisa Situacao) filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MotivoParadaCentro>();

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                query = query.Where(o => o.Descricao == filtrosPesquisa.Descricao);

            if (filtrosPesquisa.CentroCarregamento > 0)
                query = query.Where(o => o.CentroCarregamento.Codigo == filtrosPesquisa.CentroCarregamento);

            if (filtrosPesquisa.Situacao != SituacaoAtivoPesquisa.Todos)
                query = query.Where(o => o.Ativo == (filtrosPesquisa.Situacao == SituacaoAtivoPesquisa.Ativo));

            return query;
        }

        #endregion

        public List<Dominio.Entidades.Embarcador.Logistica.MotivoParadaCentro> BuscarPorDataECentroCarregamento(int codigoCentroCarregamento, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MotivoParadaCentro>();

            query = query.Where(o => o.Ativo == true);
            query = query.Where(o => o.CentroCarregamento.Codigo == codigoCentroCarregamento);
            query = query.Where(o => o.DataInicio.Date <= data && o.DataFim.Date >= data);
            
            return query
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.MotivoParadaCentro> BuscarPorPeriodoECentroCarregamento(int codigoCentroCarregamento, DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MotivoParadaCentro>();

            query = query.Where(o => o.Ativo == true);
            query = query.Where(o => o.CentroCarregamento.Codigo == codigoCentroCarregamento);
            /**
             * 19/02/2021
             * A query a seguir estava o.DataInicio >= dataInicial || o.DataFim <= dataFinal
             * Não tenho certeza do pqe. Se já passou um mês da data e nada acontece, pode 
             * apagar esse comentário.
             * Obrigado
             */
            query = query.Where(o => o.DataInicio < dataFinal && o.DataFim > dataInicial);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.MotivoParadaCentro> BuscarAtivos()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MotivoParadaCentro>();

            query = query.Where(o => o.Ativo == true);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.MotivoParadaCentro> Consultar((int CentroCarregamento, string Descricao, SituacaoAtivoPesquisa Situacao) filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = Consultar(filtrosPesquisa);

            return ObterLista(query, parametrosConsulta);
        }

        public int ContarConsulta((int CentroCarregamento, string Descricao, SituacaoAtivoPesquisa Situacao) filtrosPesquisa)
        {
            var query = Consultar(filtrosPesquisa);

            return query.Count();
        }
    }
}
