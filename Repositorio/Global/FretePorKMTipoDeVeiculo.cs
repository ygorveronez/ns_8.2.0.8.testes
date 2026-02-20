using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;

namespace Repositorio
{
    public class FretePorKMTipoDeVeiculo : RepositorioBase<Dominio.Entidades.FretePorKMTipoDeVeiculo>
    {
        public FretePorKMTipoDeVeiculo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.FretePorKMTipoDeVeiculo BuscarPorTipoVeiculo(int tipoVeiculo, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.FretePorKMTipoDeVeiculo>();

            var result = from obj in query where obj.TipoVeiculo.Codigo == tipoVeiculo && obj.Empresa.Codigo == codigoEmpresa select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.FretePorKMTipoDeVeiculo> BuscarTabelasAtivasPorTipo(int codigoEmpresa, Dominio.Enumeradores.TipoCalculoFreteKMTipoVeiculo tipoCalculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.FretePorKMTipoDeVeiculo>();

            var result = from obj in query where obj.Status == "A" && obj.Empresa.Codigo == codigoEmpresa && obj.TipoCalculo == tipoCalculo select obj;

            return result.ToList();
        }

        public Dominio.Entidades.FretePorKMTipoDeVeiculo BuscarPorCodigo(int codigo, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.FretePorKMTipoDeVeiculo>();

            var result = from obj in query where obj.Codigo == codigo && obj.Empresa.Codigo == codigoEmpresa select obj;

            return result.FirstOrDefault();
        }

        public IQueryable<Dominio.Entidades.FretePorKMTipoDeVeiculo> _Consultar(int codigoEmpresa, string status, string tipoVeiculo, int KMInicial, int KMFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.FretePorKMTipoDeVeiculo>();

            var result = from obj in query
                         where
                             obj.Empresa.Codigo == codigoEmpresa
                         select obj;

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            if (!string.IsNullOrWhiteSpace(tipoVeiculo))
                result = result.Where(o => o.TipoVeiculo.Descricao.Contains(tipoVeiculo));

            if (KMInicial > 0)
                result = result.Where(o => o.KMFranquia >= KMInicial);

            if (KMFinal > 0)
                result = result.Where(o => o.KMFranquia <= KMFinal);

            return result;
        }

        public List<Dominio.Entidades.FretePorKMTipoDeVeiculo> Consultar(int codigoEmpresa, string status, string tipoVeiculo, int KMInicial, int KMFinal, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(codigoEmpresa, status, tipoVeiculo, KMInicial, KMFinal);

            return result.Fetch(o => o.TipoVeiculo)
                         .OrderBy(o => o.TipoVeiculo.Descricao)
                         .Skip(inicioRegistros)
                         .Take(maximoRegistros)
                         .ToList();
        }

        public int ContarConsulta(int codigoEmpresa, string status, string tipoVeiculo, int KMInicial, int KMFinal)
        {
            var result = _Consultar(codigoEmpresa, status, tipoVeiculo, KMInicial, KMFinal);

            return result.Count();
        }
    }
}
