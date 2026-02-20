using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.PagamentoMotorista
{
    public class PagamentoMotoristaIntegracaoRetorno : RepositorioBase<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno>
    {
        public PagamentoMotoristaIntegracaoRetorno(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno BuscarPorCodigoPagamentoEComRetorno(int codigoPagamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno>();
            var result = from obj in query where obj.PagamentoMotoristaTMS.Codigo == codigoPagamento && obj.CodigoRetorno != "" && obj.CodigoRetorno != "0" select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno> Consultar(int codigoPagamento, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno>();

            var result = from obj in query select obj;

            if (codigoPagamento > 0)
                result = result.Where(o => o.PagamentoMotoristaTMS.Codigo == codigoPagamento && o.PagamentoMotoristaIntegracaoEnvio == null);

            return result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending")).Skip(inicio).Take(limite)
                .ToList();
        }

        public int ContarConsulta(int codigoPagamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno>();

            var result = from obj in query select obj;

            if (codigoPagamento > 0)
                result = result.Where(o => o.PagamentoMotoristaTMS.Codigo == codigoPagamento && o.PagamentoMotoristaIntegracaoEnvio == null);

            return result.Count();
        }


        public List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno> ConsultarPorEnvio(int CodigopagamentoMotoristaIntegracaoEnvio, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno>();

            var result = from obj in query select obj;

            if (CodigopagamentoMotoristaIntegracaoEnvio > 0)
                result = result.Where(o => o.PagamentoMotoristaIntegracaoEnvio.Codigo == CodigopagamentoMotoristaIntegracaoEnvio);

            return result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending")).Skip(inicio).Take(limite)
                .ToList();
        }

        public int ContarConsultaPorEnvio(int CodigopagamentoMotoristaIntegracaoEnvio)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno>();

            var result = from obj in query select obj;

            if (CodigopagamentoMotoristaIntegracaoEnvio > 0)
                result = result.Where(o => o.PagamentoMotoristaIntegracaoEnvio.Codigo == CodigopagamentoMotoristaIntegracaoEnvio);

            return result.Count();
        }


    }
}
