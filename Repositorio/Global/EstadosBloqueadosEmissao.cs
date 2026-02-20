using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class EstadosBloqueadosEmissao : RepositorioBase<Dominio.Entidades.EstadosBloqueadosEmissao>, Dominio.Interfaces.Repositorios.EstadosBloqueadosEmissao
    {
        public EstadosBloqueadosEmissao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.EstadosBloqueadosEmissao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EstadosBloqueadosEmissao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.EstadosBloqueadosEmissao> BuscarPorConfiguracao(int codigoConfiguracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EstadosBloqueadosEmissao>();
            var result = from obj in query where obj.ConfiguracaoEmpresa.Codigo == codigoConfiguracao select obj;
            return result.ToList();
        }

        public Dominio.Entidades.EstadosBloqueadosEmissao BuscarPorUF(int codigoConfiguracao, string siglaUF)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EstadosBloqueadosEmissao>();
            var result = from obj in query where obj.ConfiguracaoEmpresa.Codigo == codigoConfiguracao && obj.Estado.Sigla == siglaUF select obj;
            return result.FirstOrDefault();
        }
    }
}