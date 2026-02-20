
using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoDownloadArquivos : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDownloadArquivos>
    {
        public ConfiguracaoDownloadArquivos(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDownloadArquivos BuscarConfiguracaoPadrao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDownloadArquivos> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDownloadArquivos>();

            return query.FirstOrDefault();
        }
    }
}
