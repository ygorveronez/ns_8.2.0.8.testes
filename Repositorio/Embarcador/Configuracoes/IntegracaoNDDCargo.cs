using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoNDDCargo : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoNDDCargo>
    {
        public IntegracaoNDDCargo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoNDDCargo Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoNDDCargo>();

            return consultaIntegracao.FirstOrDefault();
        }
    }
}