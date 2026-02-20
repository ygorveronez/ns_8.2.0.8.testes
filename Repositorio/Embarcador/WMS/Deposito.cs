using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.WMS
{
    public class Deposito : RepositorioBase<Dominio.Entidades.Embarcador.WMS.Deposito>
    {
        public Deposito(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.WMS.Deposito BuscarPorCodigoIntegracao(string codigoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.Deposito>();

            var result = from obj in query where obj.CodigoIntegracao == codigoIntegracao && obj.Ativo select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.WMS.Deposito BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.Deposito>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.WMS.Deposito> BuscarDepositos()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.Deposito>();
            var result = from obj in query select obj;
            return result.ToList();
        }

        private IQueryable<Dominio.Entidades.Embarcador.WMS.Deposito> _Consultar(string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.Deposito>();

            var result = from obj in query select obj;

            // Filtros
            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            return result;
        }

        public List<Dominio.Entidades.Embarcador.WMS.Deposito> Consultar(string descricao, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(descricao);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(string descricao)
        {
            var result = _Consultar(descricao);

            return result.Count();
        }

        public Dominio.Entidades.Embarcador.WMS.Deposito BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.Pedido.Deposito != null select obj;
            return result.Select(c => c.Pedido.Deposito).FirstOrDefault();
        }

        #endregion
    }
}
