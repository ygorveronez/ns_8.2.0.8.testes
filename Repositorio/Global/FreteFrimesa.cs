using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class FreteFrimesa : RepositorioBase<Dominio.Entidades.FreteFrimesa>, Dominio.Interfaces.Repositorios.FreteFrimesa
    {
        public FreteFrimesa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.FreteFrimesa BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.FreteFrimesa>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public decimal BuscarValorPorRotaTipoVeiculo(int codigoEmpresa, int codigoRota, int codigoTipoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.FreteFrimesa>();
            var result = from obj in query
                         where 
                            obj.Empresa.Codigo == codigoEmpresa &&
                            obj.Rota.Codigo == codigoRota &&
                            obj.TipoVeiculo.Codigo == codigoTipoVeiculo &&
                            obj.Status == "A"
                         select obj;
            return result.FirstOrDefault().ValorFrete;
        }

        public Dominio.Entidades.FreteFrimesa BuscarPorRotaVeiculo(int codigoEmpresa, string rota, string tipoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.FreteFrimesa>();
            var result = from obj in query
                         where
                            obj.Empresa.Codigo == codigoEmpresa &&
                            rota.Contains(obj.Rota.Descricao) &&
                            tipoVeiculo.Contains(obj.TipoVeiculo.Descricao)
                         select obj;
            return result.FirstOrDefault();
        }

    }
}