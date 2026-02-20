using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoDestinadosSAP : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDestinadosSAP>
    {
        public IntegracaoDestinadosSAP(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDestinadosSAP Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDestinadosSAP>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }

}