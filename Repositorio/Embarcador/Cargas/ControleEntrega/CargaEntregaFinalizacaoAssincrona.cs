using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Cargas.ControleEntrega
{
    public class CargaEntregaFinalizacaoAssincrona : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFinalizacaoAssincrona>
    {
        public CargaEntregaFinalizacaoAssincrona(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public CargaEntregaFinalizacaoAssincrona(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos
        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFinalizacaoAssincrona BuscarPorCodigo(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFinalizacaoAssincrona> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFinalizacaoAssincrona>();
            query = query.Where(obj => obj.Codigo == codigo);
            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFinalizacaoAssincrona BuscarPorCargaEntrega(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();
            query = query.Where(entrega => entrega.Codigo == codigo);
            return query.Select(entrega => entrega.CargaEntregaFinalizacaoAssincrona).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFinalizacaoAssincrona> BuscarPendentesProcessamento(int maximoRegistos = 5)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFinalizacaoAssincrona> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFinalizacaoAssincrona>();
            query = query.Where(i => i.SituacaoProcessamento == SituacaoProcessamentoIntegracao.AguardandoProcessamento || (i.SituacaoProcessamento == SituacaoProcessamentoIntegracao.ErroProcessamento && i.NumeroTentativas < 3));
            return query.Take(maximoRegistos).ToList();
        }
        #endregion
    }
}