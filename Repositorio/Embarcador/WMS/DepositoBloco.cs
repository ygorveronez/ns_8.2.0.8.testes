using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.WMS
{
    public class DepositoBloco : RepositorioBase<Dominio.Entidades.Embarcador.WMS.DepositoBloco>
    {
        public DepositoBloco(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.WMS.DepositoBloco BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.DepositoBloco>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public int ContarBlocosPorRua(int rua)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.DepositoBloco>();
            var result = from obj in query where obj.Rua.Codigo == rua select obj;
            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.WMS.DepositoBloco> BuscarPorRua(int rua)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.DepositoBloco>();
            var result = from obj in query where obj.Rua.Codigo == rua select obj;
            return result.ToList();
        }

        private IQueryable<Dominio.Entidades.Embarcador.WMS.DepositoBloco> _Consultar(string descricao, int deposito, int rua)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.DepositoBloco>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (deposito > 0)
                result = result.Where(o => o.Rua.Deposito.Codigo == deposito);

            if (rua > 0)
                result = result.Where(o => o.Rua.Codigo == rua);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.WMS.DepositoBloco> Consultar(string descricao, int deposito, int rua, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(descricao, deposito, rua);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(string descricao, int deposito, int rua)
        {
            var result = _Consultar(descricao, deposito, rua);

            return result.Count();
        }
    }
}
