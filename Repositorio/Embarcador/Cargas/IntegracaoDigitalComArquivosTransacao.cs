using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas
{
    public class IntegracaoDigitalComArquivosTransacao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.IntegracaoDigitalCom.IntegracaoDigitalComArquivosTransacao>
    {
        #region Construtores
        public IntegracaoDigitalComArquivosTransacao(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public IntegracaoDigitalComArquivosTransacao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public Task<List<Dominio.Entidades.Embarcador.Cargas.IntegracaoDigitalCom.IntegracaoDigitalComArquivosTransacao>> BuscarPorCargaAsync(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.IntegracaoDigitalCom.IntegracaoDigitalComArquivosTransacao>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga);

            return query.ToListAsync(CancellationToken);
        }

        #endregion Métodos Públicos
    }
}
