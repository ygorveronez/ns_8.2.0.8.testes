using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Documentos
{
    public class TrackingDocumentacao : RepositorioBase<Dominio.Entidades.Embarcador.Documentos.TrackingDocumentacao>
    {
        public TrackingDocumentacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Documentos.TrackingDocumentacao BuscarPorCodigo(long codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.TrackingDocumentacao>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public int BuscarProximoNumero()
        {
            IQueryable<Dominio.Entidades.Embarcador.Documentos.TrackingDocumentacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.TrackingDocumentacao>();

            return (query.Max(o => (int?)o.Numero) ?? 0) + 1;
        }


        public List<Dominio.Entidades.Embarcador.Documentos.TrackingDocumentacao> BuscarDocumentacoesAgIntegracao(int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.TrackingDocumentacao>();
            query = query.Where(o => o.IntegracaoPendente == true);

            return query.Skip(inicio).Take(limite).ToList();
        }

        public int ContarDocumentacoesAgIntegracao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.TrackingDocumentacao>();
            query = query.Where(o => o.IntegracaoPendente == true);

            return query.Count();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Documentos.TrackingDocumentacao> _Consultar(int codigoPedidoViagemDirecao, int codigoPortoOrigem, int codigoPortoDestino, DateTime? dataInicial, DateTime? dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTrackingDocumentacao tipoTrackingDocumentacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIMO tipoIMO)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.TrackingDocumentacao>();

            var result = from obj in query select obj;

            // Filtros
            if (codigoPedidoViagemDirecao > 0)
                result = result.Where(o => o.PedidoViagemNavio.Codigo == codigoPedidoViagemDirecao);

            if (codigoPortoOrigem > 0)
                result = result.Where(o => o.PortoOrigem.Codigo == codigoPortoOrigem);

            if (codigoPortoDestino > 0)
                result = result.Where(o => o.PortoDestino.Codigo == codigoPortoDestino);

            if (dataInicial.HasValue && dataInicial.Value > DateTime.MinValue)
                result = result.Where(o => o.DataGeracao.Value.Date >= dataInicial.Value.Date);

            if (dataFinal.HasValue && dataFinal.Value > DateTime.MinValue)
                result = result.Where(o => o.DataGeracao.Value.Date <= dataFinal.Value.Date);

            if (tipoTrackingDocumentacao != TipoTrackingDocumentacao.Todos)
                result = result.Where(o => o.TipoTrackingDocumentacao == tipoTrackingDocumentacao);

            if (tipoIMO != TipoIMO.Todos)
                result = result.Where(o => o.TipoIMO == tipoIMO);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Documentos.TrackingDocumentacao> Consultar(int codigoPedidoViagemDirecao, int codigoPortoOrigem, int codigoPortoDestino, DateTime? dataInicial, DateTime? dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTrackingDocumentacao tipoTrackingDocumentacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIMO tipoIMO, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(codigoPedidoViagemDirecao, codigoPortoOrigem, codigoPortoDestino, dataInicial, dataFinal, tipoTrackingDocumentacao, tipoIMO);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(int codigoPedidoViagemDirecao, int codigoPortoOrigem, int codigoPortoDestino, DateTime? dataInicial, DateTime? dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTrackingDocumentacao tipoTrackingDocumentacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIMO tipoIMO)
        {
            var result = _Consultar(codigoPedidoViagemDirecao, codigoPortoOrigem, codigoPortoDestino, dataInicial, dataFinal, tipoTrackingDocumentacao, tipoIMO);

            return result.Count();
        }
    }
}

