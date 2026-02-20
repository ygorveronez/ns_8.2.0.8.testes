using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoUnilever : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoUnilever>
    {
        public IntegracaoUnilever(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoUnilever Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoUnilever>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }

}