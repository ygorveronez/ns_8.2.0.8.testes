using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class MonitoramentoEventoTipoIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTipoIntegracao>
    {
        public MonitoramentoEventoTipoIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTipoIntegracao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTipoIntegracao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTipoIntegracao> BuscarPorMonitoramentoEvento(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTipoIntegracao>();
            var result = from obj in query where obj.MonitoramentoEvento.Codigo == codigo select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTipoIntegracao BuscarPorMonitoramentoEventoETipoIntegracao(int codigoMonitoramentoEvento, int codigoTipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTipoIntegracao>();
            var result = from obj in query
                         where obj.MonitoramentoEvento.Codigo == codigoMonitoramentoEvento &&
                               obj.TipoIntegracao.Codigo == codigoTipoIntegracao
                         select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> BuscarTipoIntegracaoPorMonitoramentoEvento(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTipoIntegracao>();
            var result = from obj in query where obj.MonitoramentoEvento.Codigo == codigo select obj.TipoIntegracao.Tipo;
            return result.ToList();
        }
    }
}
