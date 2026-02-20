using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoGadle : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGadle>
    {
        public IntegracaoGadle(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGadle Buscar()
        {
            var consultaIntegracaoGadle = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGadle>();

            return consultaIntegracaoGadle.FirstOrDefault();
        }

        #endregion
    }
}
