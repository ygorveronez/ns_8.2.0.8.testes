using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoDexco : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDexco>
    {
        public IntegracaoDexco(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDexco BuscarIntegracao()
        {
            var consultaIntegracaoMicDta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDexco>();
            return consultaIntegracaoMicDta.FirstOrDefault();

        }
    }
}
