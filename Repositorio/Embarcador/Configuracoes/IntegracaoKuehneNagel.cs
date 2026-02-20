using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoKuehneNagel : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKuehneNagel>
    {
        #region Construtores

        public IntegracaoKuehneNagel(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKuehneNagel Buscar()
        {
            var consultaIntegracaoKuehneNagel = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKuehneNagel>();

            return consultaIntegracaoKuehneNagel.FirstOrDefault();
        }

        #endregion Métodos Públicos
    }
}
