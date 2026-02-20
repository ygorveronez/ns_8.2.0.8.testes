using System.Collections.Generic;
using System.Linq;


namespace Repositorio
{
    public class ConfiguracaoAverbacaoClientes : RepositorioBase<Dominio.Entidades.ConfiguracaoAverbacaoClientes>, Dominio.Interfaces.Repositorios.ConfiguracaoAverbacaoClientes
    {
        public ConfiguracaoAverbacaoClientes(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.ConfiguracaoAverbacaoClientes BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConfiguracaoAverbacaoClientes>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
        public List<Dominio.Entidades.ConfiguracaoAverbacaoClientes> BuscarPorConfiguracao(int codigoConfiguracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConfiguracaoAverbacaoClientes>();
            var result = from obj in query where obj.Configuracao.Codigo == codigoConfiguracao select obj;
            return result.ToList();
        }

    }
}
