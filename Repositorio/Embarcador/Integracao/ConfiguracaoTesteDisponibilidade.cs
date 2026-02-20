using System.Linq;

namespace Repositorio.Embarcador.Integracao
{
    public class ConfiguracaoTesteDisponibilidade : RepositorioBase<Dominio.Entidades.Embarcador.Integracao.ConfiguracaoTesteDisponibilidade>
    {
        public ConfiguracaoTesteDisponibilidade(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Integracao.ConfiguracaoTesteDisponibilidade ObterConfiguracao()
        {
            var consultaConfiguracaoTesteDisponibilidade = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.ConfiguracaoTesteDisponibilidade>();

            return consultaConfiguracaoTesteDisponibilidade.FirstOrDefault();
        }
    }
}
