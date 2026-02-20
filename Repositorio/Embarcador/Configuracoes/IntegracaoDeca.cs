using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoDeca : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDeca>
    {
        public IntegracaoDeca(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDeca Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDeca>();

            return consultaIntegracao.FirstOrDefault();
        }

        public bool PossuiIntegracaoBalanca()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDeca>()
                .Where(o => o.PossuiIntegracaoBalanca);

            return consultaIntegracao.Any();
        }

        #endregion
    }
}