using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class ServicoNFsePorCidade : RepositorioBase<Dominio.Entidades.ServicoNFsePorCidade>, Dominio.Interfaces.Repositorios.ServicoNFsePorCidade
    {
        public ServicoNFsePorCidade(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.ServicoNFsePorCidade BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ServicoNFsePorCidade>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.ServicoNFsePorCidade> BuscarPorConfiguracao(int codigoConfiguracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ServicoNFsePorCidade>();
            var result = from obj in query where obj.ConfiguracaoEmpresa.Codigo == codigoConfiguracao select obj;
            return result.ToList();
        }

        public Dominio.Entidades.ServicoNFsePorCidade BuscarPorCidade(int codigoConfiguracao, int codigoLocalidade)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ServicoNFsePorCidade>();
            var result = from obj in query where obj.ConfiguracaoEmpresa.Codigo == codigoConfiguracao && obj.Localidade.Codigo == codigoLocalidade select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.ServicoNFsePorCidade BuscarPorCidadeIBGE(int codigoConfiguracao, int codigoIBGE)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ServicoNFsePorCidade>();
            var result = from obj in query where obj.ConfiguracaoEmpresa.Codigo == codigoConfiguracao && obj.Localidade.CodigoIBGE == codigoIBGE select obj;
            return result.FirstOrDefault();
        }
    }
}