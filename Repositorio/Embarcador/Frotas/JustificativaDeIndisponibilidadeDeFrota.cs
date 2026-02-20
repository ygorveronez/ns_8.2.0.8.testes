using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Frotas
{
    public class JustificativaDeIndisponibilidadeDeFrota : RepositorioBase<Dominio.Entidades.Embarcador.Frotas.JustificativaDeIndisponibilidadeDeFrota>
    {
        #region Construtores

        public JustificativaDeIndisponibilidadeDeFrota(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Frotas.JustificativaDeIndisponibilidadeDeFrota> Consultar(string descricao, SituacaoAtivoPesquisa status, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frotas.JustificativaDeIndisponibilidadeDeFrota>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(x => x.Descricao == descricao || x.Descricao.Contains(descricao));

            if(status != SituacaoAtivoPesquisa.Todos)
            {
                bool ativo = status == SituacaoAtivoPesquisa.Ativo;
                query = query.Where(x => x.Ativo == ativo);
            }

            return ObterLista(query, parametroConsulta);
        }

        public int ContarConsulta(string descricao, SituacaoAtivoPesquisa status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frotas.JustificativaDeIndisponibilidadeDeFrota>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(x => x.Descricao == descricao || x.Descricao.Contains(descricao));

            if (status != SituacaoAtivoPesquisa.Todos)
            {
                bool ativo = status == SituacaoAtivoPesquisa.Ativo;
                query = query.Where(x => x.Ativo == ativo);
            }

            return query.Count();
        }

        #endregion

      
    }
}
