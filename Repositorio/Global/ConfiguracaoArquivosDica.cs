using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class ConfiguracaoArquivosDica : RepositorioBase<Dominio.Entidades.ConfiguracaoArquivosDica>, Dominio.Interfaces.Repositorios.ConfiguracaoArquivosDica
    {
        public ConfiguracaoArquivosDica(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.ConfiguracaoArquivosDica BuscarPorCodigo(int codigoArquivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConfiguracaoArquivosDica>();
            var result = from obj in query where obj.Codigo == codigoArquivo select obj;
            return result.FirstOrDefault();
        }
        public Dominio.Entidades.ConfiguracaoArquivosDica BuscarPorCodigo(int codigoConfiguracao, int codigoArquivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConfiguracaoArquivosDica>();
            var result = from obj in query where obj.Codigo == codigoArquivo && obj.Configuracao.Codigo == codigoConfiguracao select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.ConfiguracaoArquivosDica> BuscarPorConfiguracao(int codigoConfiguracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConfiguracaoArquivosDica>();
            var result = from obj in query where obj.Configuracao.Codigo == codigoConfiguracao select obj;
            return result.ToList();
        }
    }
}
