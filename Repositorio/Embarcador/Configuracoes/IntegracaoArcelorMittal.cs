using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoArcelorMittal : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoArcelorMittal>
    {
        public IntegracaoArcelorMittal(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoArcelorMittal Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoArcelorMittal>();
            return consultaIntegracao.FirstOrDefault();
        }
    }
}
