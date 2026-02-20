using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class ValoresPorDocumentos : RepositorioBase<Dominio.Entidades.ValoresPorDocumentos>, Dominio.Interfaces.Repositorios.ValoresPorDocumentos
    {
        public ValoresPorDocumentos(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.ValoresPorDocumentos BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ValoresPorDocumentos>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.ValoresPorDocumentos> BuscarPorPlano(int codigoPlano)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ValoresPorDocumentos>();
            var result = from obj in query where obj.Plano.Codigo == codigoPlano select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.ValoresPorDocumentos> BuscarPorPlanoOrdenado(int codigoPlano)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ValoresPorDocumentos>();
            var result = from obj in query
                         where obj.Plano.Codigo == codigoPlano
                         orderby obj.Descricao
                         select obj;

            return result.ToList();
        }

        public List<string> BuscarDescricoes(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ValoresPorDocumentos>();
            var result = from obj in query where obj.Plano.Status == "A" select obj;

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Plano.Empresa.Codigo == codigoEmpresa);

            return result.Select(o => o.Descricao).Distinct().ToList();
        }
    }
}
