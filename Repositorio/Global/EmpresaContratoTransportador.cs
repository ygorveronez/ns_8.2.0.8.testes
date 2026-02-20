using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class EmpresaContratoTransportador : RepositorioBase<Dominio.Entidades.EmpresaContratoTransportador>
    {
        public EmpresaContratoTransportador(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.EmpresaContratoTransportador> BuscarPorContrato(int codigoContrato)
        {
            IQueryable<Dominio.Entidades.EmpresaContratoTransportador> query = this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaContratoTransportador>();

            query = query.Where(o => o.Contrato.Codigo == codigoContrato);

            return query.ToList();
        }

        public Dominio.Entidades.EmpresaContratoTransportador BuscarPorContratoETransportador(int codigoContrato, int codigoEmpresa)
        {
            IQueryable<Dominio.Entidades.EmpresaContratoTransportador> query = this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaContratoTransportador>();

            query = query.Where(o => o.Contrato.Codigo == codigoContrato && o.Empresa.Codigo == codigoEmpresa);

            return query.FirstOrDefault();
        }

        public bool PossuiTransportadorEmOutroContrato(int codigoContrato, List<int> codigosTransportadores)
        {
            IQueryable<Dominio.Entidades.EmpresaContratoTransportador> query = this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaContratoTransportador>();

            query = query.Where(o => o.Contrato.Codigo != codigoContrato && codigosTransportadores.Contains(o.Empresa.Codigo));

            return query.Any();
        }

        #endregion
    }
}
