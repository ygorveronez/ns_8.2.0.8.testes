using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Escrituracao
{
    public class MovimentoContratoFreteCliente : RepositorioBase<Dominio.Entidades.Embarcador.Escrituracao.MovimentoContratoFreteCliente>
    {

        public MovimentoContratoFreteCliente(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Escrituracao.MovimentoContratoFreteCliente> BuscarPorCodigoContrato(int codigoContrato, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Escrituracao.MovimentoContratoFreteCliente> consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.MovimentoContratoFreteCliente>();

            consulta = consulta.Where(obj => obj.ContratoFreteCliente.Codigo == codigoContrato);

            return ObterLista(consulta, parametrosConsulta);
        }

        public decimal ConsultarSaldoUtilizadoPorCodigoContrato(int codigoContrato)
        {
            IQueryable<decimal> consultaDebito = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.MovimentoContratoFreteCliente>()
                .Where(obj => obj.ContratoFreteCliente.Codigo == codigoContrato && obj.TipoMovimentoContrato == Dominio.ObjetosDeValor.Enumerador.TipoMovimentoContrato.Debito)
                .Select(x => x.Valor);

            IQueryable<decimal> consultaCredito = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.MovimentoContratoFreteCliente>()
                .Where(obj => obj.ContratoFreteCliente.Codigo == codigoContrato && obj.TipoMovimentoContrato == Dominio.ObjetosDeValor.Enumerador.TipoMovimentoContrato.Credito)
                .Select(x => x.Valor);

            decimal totalDebito = consultaDebito.Any() ? consultaDebito.Sum() : 0;
            decimal totalCredito = consultaCredito.Any() ? consultaCredito.Sum() : 0;

            return totalDebito - totalCredito;
        }

        public bool ExistePorCarga(int codigoCarga, bool apenasCredito = false)
        {
            IQueryable<Dominio.Entidades.Embarcador.Escrituracao.MovimentoContratoFreteCliente> consultaDebito = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.MovimentoContratoFreteCliente>()
                .Where(obj => obj.Carga.Codigo == codigoCarga);

            if (apenasCredito)
                consultaDebito = consultaDebito.Where(obj => obj.TipoMovimentoContrato == Dominio.ObjetosDeValor.Enumerador.TipoMovimentoContrato.Credito);

            return consultaDebito.Any();
        }

        #endregion
    }
}