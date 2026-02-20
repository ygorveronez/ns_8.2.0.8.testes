using System.Collections.Generic;
using System.Linq;


namespace Repositorio
{
    public class ConfiguracaoAutDownloadXML : RepositorioBase<Dominio.Entidades.ConfiguracaoAutDownloadXML>, Dominio.Interfaces.Repositorios.ConfiguracaoAutDownloadXML
    {
        public ConfiguracaoAutDownloadXML(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.ConfiguracaoAutDownloadXML BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConfiguracaoAutDownloadXML>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
        public List<Dominio.Entidades.ConfiguracaoAutDownloadXML> BuscarPorConfiguracao(int codigoConfiguracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConfiguracaoAutDownloadXML>();
            var result = from obj in query where obj.Configuracao.Codigo == codigoConfiguracao select obj;
            return result.ToList();
        }

    }
}
