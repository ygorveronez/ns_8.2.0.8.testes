using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoMicDta : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMicDta>
    {
        public IntegracaoMicDta(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMicDta Buscar()
        {
            var consultaIntegracaoMicDta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMicDta>();

            return consultaIntegracaoMicDta.FirstOrDefault();
        }

        #endregion
    }
}
