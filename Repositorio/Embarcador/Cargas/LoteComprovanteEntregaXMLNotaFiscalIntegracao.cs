using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public class LoteComprovanteEntregaXMLNotaFiscalIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.LoteComprovanteEntregaXMLNotaFiscalIntegracao>
    {

        private readonly bool _integracaoTransportador;

        #region Construtores

        public LoteComprovanteEntregaXMLNotaFiscalIntegracao(UnitOfWork unitOfWork) : this(unitOfWork, integracaoTransportador: false) { }

        public LoteComprovanteEntregaXMLNotaFiscalIntegracao(UnitOfWork unitOfWork, bool integracaoTransportador) : base(unitOfWork)
        {
            _integracaoTransportador = integracaoTransportador;
        }

        #endregion


        #region Metodos Publicos

        public int ContarPorLote(int codigo, SituacaoIntegracao situacao, bool integracaoFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.LoteComprovanteEntregaXMLNotaFiscalIntegracao>();

            var result = from obj in query where obj.XMLNotaFiscalComprovanteEntrega.LoteComprovanteEntrega.Codigo == codigo && obj.SituacaoIntegracao == situacao && obj.IntegracaoFilialEmissora == integracaoFilialEmissora && obj.TipoIntegracao.IntegracaoTransportador == _integracaoTransportador select obj;

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.LoteComprovanteEntregaXMLNotaFiscalIntegracao> BuscarPorCodigo(int codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.LoteComprovanteEntregaXMLNotaFiscalIntegracao>();

            var result = from obj in query where obj.XMLNotaFiscalComprovanteEntrega.LoteComprovanteEntrega.Codigo == codigo select obj;

            if (situacao.HasValue)
                result = result.Where(o => o.SituacaoIntegracao == situacao.Value);

            if (tipo.HasValue)
                result = result.Where(o => o.TipoIntegracao.Tipo == tipo.Value);

            return result.Fetch(obj => obj.TipoIntegracao).ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.LoteComprovanteEntregaXMLNotaFiscalIntegracao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.LoteComprovanteEntregaXMLNotaFiscalIntegracao>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.LoteComprovanteEntregaXMLNotaFiscalIntegracao> Consultar(int codigoLote, int codigoXmlComprovante, SituacaoIntegracao? situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.LoteComprovanteEntregaXMLNotaFiscalIntegracao>();

            var result = from obj in query where obj.TipoIntegracao.IntegracaoTransportador == _integracaoTransportador select obj;

            if (codigoLote > 0)
                result = result.Where(o => o.XMLNotaFiscalComprovanteEntrega.LoteComprovanteEntrega.Codigo == codigoLote);

            if (codigoXmlComprovante > 0)
                result = result.Where(o => o.XMLNotaFiscalComprovanteEntrega.Codigo == codigoXmlComprovante);

            if (situacao.HasValue)
                result = result.Where(o => o.SituacaoIntegracao == situacao);

            if (tipo.HasValue)
                result = result.Where(o => o.TipoIntegracao.Tipo == tipo);

            return result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending")).Skip(inicio).Take(limite)
                .Fetch(obj => obj.TipoIntegracao)
                .ToList();
        }

        public int ContarConsulta(int codigoLote, int codigoXmlComprovante, SituacaoIntegracao? situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.LoteComprovanteEntregaXMLNotaFiscalIntegracao>();

            var result = from obj in query where obj.TipoIntegracao.IntegracaoTransportador == _integracaoTransportador select obj;

            if (codigoXmlComprovante > 0)
                result = result.Where(o => o.XMLNotaFiscalComprovanteEntrega.Codigo == codigoXmlComprovante);

            if (codigoLote > 0)
                result = result.Where(o => o.XMLNotaFiscalComprovanteEntrega.LoteComprovanteEntrega.Codigo == codigoLote);

            if (situacao.HasValue)
                result = result.Where(o => o.SituacaoIntegracao == situacao);

            if (tipo.HasValue)
                result = result.Where(o => o.TipoIntegracao.Tipo == tipo);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.LoteComprovanteEntregaXMLNotaFiscalIntegracao> BuscarLoteComprovanteEntregaPendente(int tentativasLimite, double tempoProximaTentativaMinutos, string propOrdenacao, string dirOrdenacao, int maximoRegistros, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioIntegracao tipoEnvio)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.LoteComprovanteEntregaXMLNotaFiscalIntegracao>();

            var result = from obj in query where obj.TipoIntegracao.TipoEnvio == tipoEnvio && (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao || (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao && obj.NumeroTentativas < tentativasLimite && obj.DataIntegracao <= DateTime.Now.AddMinutes(-tempoProximaTentativaMinutos))) select obj;

            return result
                .Fetch(obj => obj.XMLNotaFiscalComprovanteEntrega)
                .OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(0).Take(maximoRegistros).ToList();
        }

        #endregion
    }
}
