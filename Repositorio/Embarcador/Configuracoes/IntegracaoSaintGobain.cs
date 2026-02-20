using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoSaintGobain : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSaintGobain>
    {
        public IntegracaoSaintGobain(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSaintGobain Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSaintGobain>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }
}
