using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Canhotos
{
    public class ConfiguracaoFTPBaixaCanhoto : RepositorioBase<Dominio.Entidades.Embarcador.Canhotos.ConfiguracaoFTPBaixaCanhoto>
    {
        public ConfiguracaoFTPBaixaCanhoto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Canhotos.ConfiguracaoFTPBaixaCanhoto BuscarConfiguracaoPadrao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.ConfiguracaoFTPBaixaCanhoto>();

            var result = from obj in query  select obj;

            return result.FirstOrDefault();
        }
    }
}
