using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Pessoas
{
    public class GrupoPessoasFaturaCanhoto : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFaturaCanhoto>
    {
        #region Construtores

        public GrupoPessoasFaturaCanhoto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public GrupoPessoasFaturaCanhoto(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken)
        {
        }

        public Task<List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFaturaCanhoto>> BuscarPorGrupoPessoasAsync(int codigoGrupoPessoas, CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFaturaCanhoto>();

            var result = from obj in query where obj.GrupoPessoas.Codigo == codigoGrupoPessoas select obj;

            return result.ToListAsync(cancellationToken);
        }

        public Task<List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFaturaCanhoto>> BuscarPorGrupoPessoasETipoIntegracaoAsync(int codigoGrupoPessoas, Dominio.ObjetosDeValor.Enumerador.TipoIntegracaoCanhoto tipoIntegracaocanhoto, CancellationToken cancellationToken)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFaturaCanhoto> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFaturaCanhoto>()
                .Where(obj =>
                    obj.GrupoPessoas.Codigo == codigoGrupoPessoas &&
                    obj.TipoIntegracaoCanhoto == tipoIntegracaocanhoto);

            return query.ToListAsync(cancellationToken);
        }

        #endregion
    }
}
