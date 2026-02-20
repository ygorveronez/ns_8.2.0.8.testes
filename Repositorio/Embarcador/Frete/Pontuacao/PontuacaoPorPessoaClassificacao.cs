using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frete.Pontuacao
{
    public class PontuacaoPorPessoaClassificacao : RepositorioBase<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorPessoaClassificacao>
    {
        #region Construtores

        public PontuacaoPorPessoaClassificacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorPessoaClassificacao> BuscarTodas(int inicio, int limite)
        {
            var consultaPontuacaoPorPessoaClassificacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorPessoaClassificacao>();

            return consultaPontuacaoPorPessoaClassificacao.OrderBy(o => o.Codigo).Skip(inicio).Take(limite).ToList();
        }

        public int ContarTodas()
        {
            var consultaPontuacaoPorPessoaClassificacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorPessoaClassificacao>();

            return consultaPontuacaoPorPessoaClassificacao.Count();
        }

        public bool VerificarExistePorPessoaClassificacao(int codigoPessoaClassificacao)
        {
            return VerificarExistePorPessoaClassificacao(codigoPessoaClassificacao, codigo: 0);
        }

        public bool VerificarExistePorPessoaClassificacao(int codigoPessoaClassificacao, int codigo)
        {
            var consultaPontuacaoPorPessoaClassificacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorPessoaClassificacao>()
                .Where(o => o.PessoaClassificacao.Codigo == codigoPessoaClassificacao && o.Codigo != codigo);

            return consultaPontuacaoPorPessoaClassificacao.Count() > 0;
        }

        #endregion
    }
}
