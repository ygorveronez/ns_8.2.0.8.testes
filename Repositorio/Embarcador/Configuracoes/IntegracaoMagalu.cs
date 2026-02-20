using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoMagalu : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMagalu>
    {
        public IntegracaoMagalu(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMagalu Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMagalu>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }
}
