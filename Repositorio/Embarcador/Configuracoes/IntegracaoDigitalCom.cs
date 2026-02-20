using NHibernate.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoDigitalCom : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDigitalCom>
    {
        public IntegracaoDigitalCom(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoDigitalCom(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDigitalCom Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDigitalCom>();

            return consultaIntegracao.FirstOrDefault();
        }

        public async Task<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDigitalCom> BuscarAsync()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDigitalCom>();

            return await consultaIntegracao.FirstOrDefaultAsync(CancellationToken);
        }

        #endregion
    }

}