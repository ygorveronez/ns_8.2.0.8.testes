using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Roteirizador
{
    public class RoteirizadorIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Roteirizador.RoteirizadorIntegracao>
    {
        #region Construtores

        public RoteirizadorIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public RoteirizadorIntegracao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Publicos

        public List<Dominio.Entidades.Embarcador.Roteirizador.RoteirizadorIntegracao> BuscarIntegracoesPendentes(int numeroRegistrosPorVez)
        {
            IQueryable<Dominio.Entidades.Embarcador.Roteirizador.RoteirizadorIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Roteirizador.RoteirizadorIntegracao>();

            query = query.Where(obj => obj.SituacaoIntegracao == SituacaoIntegracao.AgIntegracao && obj.Usuario == null);

            return query
                .Take(numeroRegistrosPorVez)
                .ToList();
        }

        public Task<Dominio.Entidades.Embarcador.Roteirizador.RoteirizadorIntegracao> BuscarPorCodigoArquivoAsync(int codigoArquivoTransacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Roteirizador.RoteirizadorIntegracao>();

            var result = query.Where(roteirizadorIntegracao => roteirizadorIntegracao.ArquivosTransacao.Any(arquivo => arquivo.Codigo == codigoArquivoTransacao));

            return result.FirstOrDefaultAsync(CancellationToken);
        }

        #endregion Métodos Publicos
    }
}
