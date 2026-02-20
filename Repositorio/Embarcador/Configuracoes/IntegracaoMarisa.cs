using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoMarisa : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMarisa>
    {
        public IntegracaoMarisa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMarisa BuscarDadosIntegracao()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMarisa>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }

}