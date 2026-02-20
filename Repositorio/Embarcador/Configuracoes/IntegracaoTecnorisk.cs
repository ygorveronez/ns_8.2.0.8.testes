using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoTecnorisk : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTecnorisk>
    {
        public IntegracaoTecnorisk(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTecnorisk Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTecnorisk>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }

}