using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.Pessoas
{
    public sealed class PessoaClassificacao : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.PessoaClassificacao>
    {
        #region Construtores

        public PessoaClassificacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Pessoas.PessoaClassificacao> Consultar(string descricao, PessoaClasse? classe)
        {
            var consultaPessoaClassificacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.PessoaClassificacao>();

            if (!string.IsNullOrWhiteSpace(descricao))
                consultaPessoaClassificacao = consultaPessoaClassificacao.Where(o => o.Descricao.Contains(descricao));

            if (classe.HasValue)
                consultaPessoaClassificacao = consultaPessoaClassificacao.Where(o => o.Classe == classe.Value);

            return consultaPessoaClassificacao;
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Pessoas.PessoaClassificacao> Consultar(string descricao, PessoaClasse? classe, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaPessoaClassificacao = Consultar(descricao, classe);

            return ObterLista(consultaPessoaClassificacao, parametrosConsulta);
        }

        public int ContarConsulta(string descricao, PessoaClasse? classe)
        {
            var consultaPessoaClassificacao = Consultar(descricao, classe);

            return consultaPessoaClassificacao.Count();
        }

        public Dominio.Entidades.Embarcador.Pessoas.PessoaClassificacao BuscarPorDescricao(string descricaoClassificacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pessoas.PessoaClassificacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.PessoaClassificacao>();

            query = query.Where(o => o.Descricao == descricaoClassificacao);

            return query.FirstOrDefault();
        }

        #endregion
    }
}
