using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoProtheus : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoProtheus>
    {
        public IntegracaoProtheus(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoProtheus Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoProtheus>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }

}