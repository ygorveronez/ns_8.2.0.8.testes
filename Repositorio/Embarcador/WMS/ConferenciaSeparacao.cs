using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.WMS
{
    public class ConferenciaSeparacao : RepositorioBase<Dominio.Entidades.Embarcador.WMS.ConferenciaSeparacao>
    {

        public ConferenciaSeparacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.WMS.ConferenciaSeparacao> BuscarPorCarga(int carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria tipoRecebimentoMercadoria)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.ConferenciaSeparacao>();
            var result = from obj in query where obj.Expedicao.Carga.Codigo == carga && obj.TipoRecebimentoMercadoria == tipoRecebimentoMercadoria select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.WMS.ConferenciaSeparacao BuscarPorExpedicaoECodigoBarras(int expedicao, string codigoBarras, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria tipoRecebimentoMercadoria)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.ConferenciaSeparacao>();
            var result = from obj in query where obj.Expedicao.Codigo == expedicao && obj.CodigoBarras.ToUpper() == codigoBarras.ToUpper() && obj.TipoRecebimentoMercadoria == tipoRecebimentoMercadoria select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.WMS.ConferenciaSeparacao BuscarPorCargaECodigoBarras(int carga, string codigoBarras, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria tipoRecebimentoMercadoria)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.ConferenciaSeparacao>();
            var result = from obj in query where obj.Expedicao.Carga.Codigo == carga && obj.CodigoBarras.ToUpper() == codigoBarras.ToUpper() && obj.TipoRecebimentoMercadoria == tipoRecebimentoMercadoria select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.WMS.ConferenciaSeparacao BuscarPorCargaECodigoValido(int carga, string codigoBarras, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria tipoRecebimentoMercadoria)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.ConferenciaSeparacao>();
            var result = from obj in query where obj.Expedicao.Carga.Codigo == carga && obj.CodigoBarras.ToUpper().Substring(0, 23).Equals(codigoBarras.ToUpper().Substring(0, 23)) && obj.TipoRecebimentoMercadoria == tipoRecebimentoMercadoria select obj;
            return result.FirstOrDefault();
        }        

        public int QuantidadeConferidaPorCarga(int carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria tipoRecebimentoMercadoria)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.ConferenciaSeparacao>();

            var result = from obj in query where obj.Expedicao.Carga.Codigo == carga && obj.TipoRecebimentoMercadoria == tipoRecebimentoMercadoria select obj;

            return result.Count();
        }

        private IQueryable<Dominio.Entidades.Embarcador.WMS.ConferenciaSeparacao> _Consultar(int carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria tipoRecebimentoMercadoria)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.ConferenciaSeparacao>();

            var result = from obj in query where obj.Expedicao.Carga.Codigo == carga && obj.TipoRecebimentoMercadoria == tipoRecebimentoMercadoria select obj;

            return result;
        }

        public List<Dominio.Entidades.Embarcador.WMS.ConferenciaSeparacao> Consultar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria tipoRecebimentoMercadoria, int carga, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(carga, tipoRecebimentoMercadoria);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria tipoRecebimentoMercadoria, int carga)
        {
            var result = _Consultar(carga, tipoRecebimentoMercadoria);

            return result.Count();
        }
    }
}
