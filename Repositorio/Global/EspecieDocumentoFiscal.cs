using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class EspecieDocumentoFiscal : RepositorioBase<Dominio.Entidades.EspecieDocumentoFiscal>, Dominio.Interfaces.Repositorios.EspecieDocumentoFiscal
    {
        public EspecieDocumentoFiscal(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.EspecieDocumentoFiscal> BuscarTodos()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EspecieDocumentoFiscal>();

            var result = from obj in query orderby obj.Descricao ascending select obj;

            return result.ToList();
        }

        public Dominio.Entidades.EspecieDocumentoFiscal BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EspecieDocumentoFiscal>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.EspecieDocumentoFiscal BuscarPorSigla(string sigla)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EspecieDocumentoFiscal>();

            var result = from obj in query where obj.Sigla.Equals(sigla) select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.EspecieDocumentoFiscal> Consultar(string descricao, string sigla, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EspecieDocumentoFiscal>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(o => o.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(sigla))
                query = query.Where(o => o.Sigla.Contains(sigla));

            return query.OrderBy(propOrdena + " " + dirOrdena).Skip(inicio).Take(limite).ToList();
        }

        public int ContarConsulta(string descricao, string sigla)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EspecieDocumentoFiscal>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(o => o.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(sigla))
                query = query.Where(o => o.Sigla.Contains(sigla));

            return query.Count();
        }
    }
}
