using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public  class IntegracaoDansales : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDansales>
    {
        #region Construtores

        public IntegracaoDansales(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDansales Buscar()
        {
            var consultaIntegracaoDansales = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDansales>();

            return consultaIntegracaoDansales.FirstOrDefault();
        }

        #endregion Métodos Públicos
    }
}
