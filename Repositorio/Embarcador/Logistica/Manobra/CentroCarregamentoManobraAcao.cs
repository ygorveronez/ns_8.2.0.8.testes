using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;

namespace Repositorio.Embarcador.Logistica
{
    public sealed class CentroCarregamentoManobraAcao : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoManobraAcao>
    {
        #region Construtores

        public CentroCarregamentoManobraAcao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoManobraAcao> BuscarPorCentroCarregamento(int codigoCentroCarregamento)
        {
            var consultaCentroCarregamentoManobraAcao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoManobraAcao>()
                .Where(o => o.CentroCarregamento.Codigo == codigoCentroCarregamento);

            return consultaCentroCarregamentoManobraAcao.ToList();
        }

        public Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoManobraAcao BuscarPorCentroCarregamentoEAcao(int codigoCentroCarregamento, int codigoAcao)
        {
            var consultaCentroCarregamentoManobraAcao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoManobraAcao>()
                .Where(o => (o.CentroCarregamento.Codigo == codigoCentroCarregamento) && (o.Acao.Codigo == codigoAcao));

            return consultaCentroCarregamentoManobraAcao.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoManobraAcao BuscarPorCentroCarregamentoETipoAcao(int codigoCentroCarregamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoManobraAcao Tipo)
        {
            var consultaCentroCarregamentoManobraAcao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoManobraAcao>()
                .Where(o => (o.CentroCarregamento.Codigo == codigoCentroCarregamento) && (o.Acao.Tipo == Tipo));

            return consultaCentroCarregamentoManobraAcao.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoManobraAcao BuscarPorCodigo(int codigo)
        {
            var consultaCentroCarregamentoManobraAcao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoManobraAcao>()
                .Where(o => o.Codigo == codigo);

            return consultaCentroCarregamentoManobraAcao.FirstOrDefault();
        }

        #endregion
    }
}
