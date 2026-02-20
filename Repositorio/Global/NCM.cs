using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class NCM : RepositorioBase<Dominio.Entidades.NCM>, Dominio.Interfaces.Repositorios.NCM
    {
        public NCM(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.NCM BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NCM>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.NCM BuscarPorNumero(string numero)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NCM>();
            var result = from obj in query where obj.Numero.Contains(numero) select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.NCM> Consultar(string numero, string descricao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NCM>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(numero))
                result = result.Where(o => o.Numero.Contains(numero));

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            return result.OrderBy(o => o.Numero).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(string numero, string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NCM>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(numero))
                result = result.Where(o => o.Numero.Contains(numero));

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            return result.Count();
        }

        public List<Dominio.Entidades.NCM> ConsultarNCM(string codigoNCM, string descricao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NCM>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(codigoNCM))
                result = result.Where(o => o.Numero.StartsWith(codigoNCM));

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultaNCM(string codigoNCM, string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NCM>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(codigoNCM))
                result = result.Where(o => o.Numero.StartsWith(codigoNCM));

            return result.Count();
        }
    }
}
