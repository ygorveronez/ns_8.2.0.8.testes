using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frete
{
    public class ContratoFreteTransportadorAcordo : RepositorioBase<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorAcordo>
    {
        #region Construtores

        public ContratoFreteTransportadorAcordo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorAcordo> ConsultarPorContrato(int codigoContrato)
        {
            var consultaAcordo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorAcordo>()
                .Where(acordo => acordo.ContratoFrete.Codigo == codigoContrato && acordo.Periodo == 0);

            return consultaAcordo;
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public List<int> BuscarAcordosNaoPesentesNaLista(int codigoContrato, List<int> codigos)
        {
            var consultaAcordo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorAcordo>()
               .Where(acordo => acordo.ContratoFrete.Codigo == codigoContrato && !codigos.Contains(acordo.Codigo));

            return consultaAcordo.Select(acordo => acordo.Codigo).ToList();
        }

        public Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorAcordo BuscarPorCodigo(int codigo)
        {
            var consultaAcordo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorAcordo>()
               .Where(acordo => acordo.Codigo == codigo);

            return consultaAcordo.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorAcordo> BuscarPorContrato(int codigoContrato)
        {
            var consultaAcordo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorAcordo>()
                .Where(acordo => acordo.ContratoFrete.Codigo == codigoContrato);

            return consultaAcordo.ToList();
        }

        public List<int> BuscarCodigosModeloVeicularPorContrato(int codigoContrato)
        {
            var consultaAcordo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorAcordo>()
                .Where(acordo => acordo.ContratoFrete.Codigo == codigoContrato);

            return consultaAcordo.Select(x => x.ModeloVeicular.Codigo).ToList();
        }

        public Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorAcordo BuscarPorContratoEAcordo(int codigoContrato, int codigoAcordo)
        {
            var consultaAcordo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorAcordo>()
                .Where(acordo => acordo.ContratoFrete.Codigo == codigoContrato && acordo.Codigo == codigoAcordo);

            return consultaAcordo.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorAcordo BuscarPorContratoEModelo(int codigoContrato, int codigoModelo)
        {
            var consultaAcordo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorAcordo>()
                .Where(acordo => acordo.ContratoFrete.Codigo == codigoContrato && acordo.ModeloVeicular.Codigo == codigoModelo);

            return consultaAcordo
                .Fetch(obj => obj.ModeloVeicular)
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorAcordo> ConsultarPorContrato(int codigoContrato, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaAcordo = ConsultarPorContrato(codigoContrato);

            return ObterLista(consultaAcordo, parametrosConsulta);
        }

        public int ContarConsultaPorContrato(int codigoContrato)
        {
            var consultaAcordo = ConsultarPorContrato(codigoContrato);

            return consultaAcordo.Count();
        }

        #endregion Métodos Públicos
    }
}
