using System.Linq;

namespace Repositorio.Embarcador.Ocorrencias
{
    public class ConfiguracaoFTPEnvioOcoren : RepositorioBase<Dominio.Entidades.Embarcador.Ocorrencias.ConfiguracaoFTPEnvioOcoren>
    {
        public ConfiguracaoFTPEnvioOcoren(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Ocorrencias.ConfiguracaoFTPEnvioOcoren BuscarConfiguracaoPadrao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.ConfiguracaoFTPEnvioOcoren>();

            var result = from obj in query select obj;

            return result.FirstOrDefault();
        }
    }
}
