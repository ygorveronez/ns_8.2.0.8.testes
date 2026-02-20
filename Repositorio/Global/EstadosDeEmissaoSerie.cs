using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class EstadosDeEmissaoSerie : RepositorioBase<Dominio.Entidades.EstadosDeEmissaoSerie>, Dominio.Interfaces.Repositorios.EstadosDeEmissaoSerie
    {
        public EstadosDeEmissaoSerie(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.EstadosDeEmissaoSerie BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EstadosDeEmissaoSerie>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.EstadosDeEmissaoSerie> BuscarPorConfiguracao(int codigoConfiguracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EstadosDeEmissaoSerie>();
            var result = from obj in query where obj.ConfiguracaoEmpresa.Codigo == codigoConfiguracao select obj;
            return result.ToList();
        }

        public Dominio.Entidades.EstadosDeEmissaoSerie BuscarPorUF(int codigoConfiguracao, string siglaUF)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EstadosDeEmissaoSerie>();
            var result = from obj in query where obj.ConfiguracaoEmpresa.Codigo == codigoConfiguracao && obj.Estado.Sigla == siglaUF select obj;
            return result.FirstOrDefault();
        }
    }
}