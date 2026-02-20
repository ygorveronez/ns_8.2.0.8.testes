using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Pessoas
{
    public class GrupoPessoaTipoOcorrencia : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoaTipoOcorrencia>
    {
        #region Construtores

        public GrupoPessoaTipoOcorrencia(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public GrupoPessoaTipoOcorrencia(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoaTipoOcorrencia> BuscarTodosPorEntidade(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas entidadeGrupoPessoa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoaTipoOcorrencia>();
            query = query.Where(obj => obj.GrupoPessoa == entidadeGrupoPessoa);
            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Pessoas.GrupoPessoaTipoOcorrencia BuscarPorTipoOcorrencia(int tipoOcorrencia, int grupoPessoas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoaTipoOcorrencia>();

            var result = from obj in query where obj.TipoOcorrencia.Codigo == tipoOcorrencia && obj.GrupoPessoa.Codigo == grupoPessoas select obj;

            return result.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoaTipoOcorrencia> BuscarOcorrenciaCanhotoAsync(int grupoPessoas)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoaTipoOcorrencia> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoaTipoOcorrencia>()
                .Where(obj => obj.TipoOcorrencia.OcorrenciaExclusivaParaCanhotosDigitalizados && obj.GrupoPessoa.Codigo == grupoPessoas);

            return query.FirstOrDefaultAsync(CancellationToken);
        }

        public Dominio.Entidades.TipoDeOcorrenciaDeCTe BuscarTipoOcorrenciaPorGrupoPessoasECodigoIntegracao(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas, string codigoIntegracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoaTipoOcorrencia> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoaTipoOcorrencia>();

            query = query.Where(o => o.GrupoPessoa == grupoPessoas && o.CodigoIntegracao == codigoIntegracao);

            return query.Select(o => o.TipoOcorrencia).FirstOrDefault();
        }

        #endregion
    }
}
