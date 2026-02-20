using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaJanelaCarregamentoTransportadorHistorico : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorHistorico>
    {
        #region Construtores

        public CargaJanelaCarregamentoTransportadorHistorico(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorHistorico> BuscarPorCargaJanelaCarregamentoTransportador(int codigo)
        {
            var consultaHistorico = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorHistorico>()
                .Where(obj => obj.CargaJanelaCarregamentoTransportador.Codigo == codigo);

            return consultaHistorico.ToList();
        }
        public bool ExistePorCargaJanelaCarregamentoTransportadorETipo(int codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargaJanelaCarregamentoTransportadorHistorico tipo)
        {
            var consultaHistorico = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorHistorico>()
                .Where(obj => obj.CargaJanelaCarregamentoTransportador.Codigo == codigo && obj.Tipo == tipo);

            return consultaHistorico.Count() > 0;
        }
        public int ContarPorCargaJanelaCarregamentoESituacao(int codigoCargaJanelaCarregamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargaJanelaCarregamentoTransportadorHistorico tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorHistorico>();

            var result = from obj in query where obj.CargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Codigo == codigoCargaJanelaCarregamento && obj.Tipo == tipo select obj;

            return result.Count();
        }

        public int ContarPorCargaESituacao(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargaJanelaCarregamentoTransportadorHistorico tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorHistorico>();

            var result = from obj in query where obj.CargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga.Codigo == codigoCarga && obj.Tipo == tipo select obj;

            return result.Count();
        }
        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorHistorico> ConsultarPorCargaESituacao(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargaJanelaCarregamentoTransportadorHistorico tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorHistorico>();

            var result = from obj in query where obj.CargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga.Codigo == codigoCarga && obj.Tipo == tipo select obj;

            return result.ToList();
        }
        #endregion
    }
}
