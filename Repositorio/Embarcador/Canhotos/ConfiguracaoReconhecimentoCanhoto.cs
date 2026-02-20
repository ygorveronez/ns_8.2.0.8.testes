using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Canhotos
{
    public class ConfiguracaoReconhecimentoCanhoto : RepositorioBase<Dominio.Entidades.Embarcador.Canhotos.ConfiguracaoReconhecimentoCanhoto> 
    {
        public ConfiguracaoReconhecimentoCanhoto(UnitOfWork unitOfWork) : base(unitOfWork) { }


        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Canhotos.ConfiguracaoReconhecimentoCanhoto> Consultar((string Descricao, SituacaoAtivoPesquisa Situacao) filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.ConfiguracaoReconhecimentoCanhoto>();

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                query = query.Where(o => o.Descricao == filtrosPesquisa.Descricao);

            if (filtrosPesquisa.Situacao != SituacaoAtivoPesquisa.Todos)
                query = query.Where(o => o.Ativo == (filtrosPesquisa.Situacao == SituacaoAtivoPesquisa.Ativo));

            return query;
        }

        #endregion

        public List<Dominio.Entidades.Embarcador.Canhotos.ConfiguracaoReconhecimentoCanhoto> BuscarAtivos()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.ConfiguracaoReconhecimentoCanhoto>();

            query = query.Where(o => o.Ativo == true);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Canhotos.ConfiguracaoReconhecimentoCanhoto> Consultar((string Descricao, SituacaoAtivoPesquisa Situacao) filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = Consultar(filtrosPesquisa);

            return ObterLista(query, parametrosConsulta);
        }
        
        public int ContarConsulta((string Descricao, SituacaoAtivoPesquisa Situacao) filtrosPesquisa)
        {
            var query = Consultar(filtrosPesquisa);

            return query.Count();
        }
    }
}
