using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class Banco : RepositorioBase<Dominio.Entidades.Banco>, Dominio.Interfaces.Repositorios.Banco
    {
        public Banco(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Banco BuscarPorNumero(int numero, List<Dominio.Entidades.Banco> lstBancos = null)
        {
            if (lstBancos == null)
            {
                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Banco>();
                var result = from obj in query where obj.Numero == numero select obj;
                return result.FirstOrDefault();
            }
            else
                return lstBancos.Where(obj=> obj.Numero == numero).FirstOrDefault();
        }

        public Dominio.Entidades.Banco BuscarPorCodigo(int codigo, List<Dominio.Entidades.Banco> lstBancos = null)
        {
            if (lstBancos == null)
            {
                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Banco>();
                var result = from obj in query where obj.Codigo == codigo select obj;
                return result.FirstOrDefault();
            }
            else
                return lstBancos.Where(obj => obj.Codigo == codigo).FirstOrDefault();
        }

        public List<Dominio.Entidades.Banco> BuscarPorCodigos(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Banco>();

            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;                        
            
            return result.ToList();
        }

        public dynamic Consultar(int numero, string descricao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Banco>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (numero > 0)
                result.Where(o => o.Numero.Equals(numero));

            return result.OrderBy(o => o.Numero).Skip(inicioRegistros).Take(maximoRegistros).Select(o => new { o.Codigo, Numero = string.Format("{0:000}", o.Numero), o.Descricao }).ToList();
        }

        public int ContarConsulta(int numero, string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Banco>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (numero > 0)
                result = result.Where(o => o.Numero.Equals(numero));

            return result.Count();
        }

        public List<Dominio.Entidades.Banco> Consultar(int numero, string descricao, string propriedadeOrdenacao, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Banco>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (numero > 0)
                result = result.Where(o => o.Numero.Equals(numero));

            return result.OrderBy(propriedadeOrdenacao + (direcaoOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public bool ContemBancoCadastrado(int numero, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Banco>();

            query = query.Where(obj => obj.Numero == numero);
            if (codigo > 0)
                query = query.Where(obj => obj.Codigo != codigo);

            return query.Any();
        }

    }
}
