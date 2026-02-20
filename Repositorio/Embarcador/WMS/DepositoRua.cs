using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.WMS
{
    public class DepositoRua : RepositorioBase<Dominio.Entidades.Embarcador.WMS.DepositoRua>
    {

        public DepositoRua(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.WMS.DepositoRua BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.DepositoRua>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public int ContarRuasPorDeposito(int deposito)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.DepositoRua>();
            var result = from obj in query where obj.Deposito.Codigo == deposito select obj;
            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.WMS.DepositoRua> BuscarPorDeposito(int deposito)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.DepositoRua>();
            var result = from obj in query where obj.Deposito.Codigo == deposito select obj;
            return result.ToList();
        }

        private IQueryable<Dominio.Entidades.Embarcador.WMS.DepositoRua> _Consultar(string descricao, int deposito)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.DepositoRua>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (deposito > 0)
                result = result.Where(o => o.Deposito.Codigo == deposito);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.WMS.DepositoRua> Consultar(string descricao, int deposito, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(descricao, deposito);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(string descricao, int deposito)
        {
            var result = _Consultar(descricao, deposito);

            return result.Count();
        }
    }
}
