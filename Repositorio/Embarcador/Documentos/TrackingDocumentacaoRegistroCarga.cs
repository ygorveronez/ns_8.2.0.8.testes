using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Documentos
{
    public class TrackingDocumentacaoRegistroCarga : RepositorioBase<Dominio.Entidades.Embarcador.Documentos.TrackingDocumentacaoRegistroCarga>
    {
        public TrackingDocumentacaoRegistroCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Documentos.TrackingDocumentacaoRegistroCarga BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.TrackingDocumentacaoRegistroCarga>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Carga> BuscarCargas(bool cargaIMO, int codigoOperadorCarga, int codigoPedidoViagemNavio, int codigoPortoDestino, int codigoPortoOrigem, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTrackingDocumentacao tipoTrackingDocumentacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoTrackingDocumentacao situacaoTrackingDocumentacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            if (cargaIMO)
                query = query.Where(o => o.Pedido.PossuiCargaPerigosa == true);
            if (codigoOperadorCarga > 0)
                query = query.Where(o => o.Carga.Operador.Codigo == codigoOperadorCarga);
            if (codigoPedidoViagemNavio > 0)
                query = query.Where(o => o.Pedido.PedidoViagemNavio.Codigo == codigoPedidoViagemNavio);
            if (codigoPortoDestino > 0)
                query = query.Where(o => o.Pedido.PortoDestino.Codigo == codigoPortoDestino);
            if (codigoPortoOrigem > 0)
                query = query.Where(o => o.Pedido.Porto.Codigo == codigoPortoOrigem);
            if (tipoTrackingDocumentacao == TipoTrackingDocumentacao.Feeder)
                query = query.Where(o => o.TipoPropostaMultimodal == TipoPropostaMultimodal.Feeder);

            if (situacaoTrackingDocumentacao == SituacaoTrackingDocumentacao.SemRegistros)
            {
                var queryRegistroCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.TrackingDocumentacaoRegistroCarga>();
                query = query.Where(o => !queryRegistroCarga.Any(r => r.Carga == o.Carga));
            }

            return query.Select(o => o.Carga).ToList();
        }
    }
}

