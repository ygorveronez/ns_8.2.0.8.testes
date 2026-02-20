using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaPedidoDocumentoMDFe : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoMDFe>
    {
        public CargaPedidoDocumentoMDFe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public CargaPedidoDocumentoMDFe(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken)
        {
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoMDFe> Consultar(int codigoCargaPedido, string propOrdenacao, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoMDFe>();

            query = query.Where(o => o.CargaPedido.Codigo == codigoCargaPedido);

            return query.Fetch(o => o.MDFe).OrderBy(propOrdenacao + " " + dirOrdena).Skip(inicio).Take(limite).ToList();
        }

        public int ContarConsulta(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoMDFe>();

            query = query.Where(o => o.CargaPedido.Codigo == codigoCargaPedido);

            return query.Count();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoMDFe BuscarPorMDFeECargaPedido(int codigoMDFe, int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoMDFe>();

            query = query.Where(o => o.CargaPedido.Codigo == codigoCargaPedido && o.MDFe.Codigo == codigoMDFe);

            return query.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoMDFe> BuscarPorMDFeECargaPedidoAsync(int codigoMDFe, int codigoCargaPedido, CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoMDFe>();

            query = query.Where(o => o.CargaPedido.Codigo == codigoCargaPedido && o.MDFe.Codigo == codigoMDFe);

            return query.FirstOrDefaultAsync(cancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoMDFe> BuscarPorCargaPedido(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoMDFe>();

            query = query.Where(o => o.CargaPedido.Codigo == codigoCargaPedido);

            return query.Fetch(o => o.MDFe).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoMDFe> BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoMDFe>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo == codigoCarga);

            return query.ToList();
        }

        public List<string> BuscarNumeroCargaPorMDFe(int codigoMDFe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoMDFe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoMDFe>();

            query = query.Where(o => o.MDFe.Codigo == codigoMDFe &&
                                     o.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada &&
                                     o.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada);

            return query.Select(o => o.CargaPedido.Carga.CodigoCargaEmbarcador).ToList();
        }

        public bool ExistePorMDFe(int codigoMDFe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoMDFe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoMDFe>();

            query = query.Where(o => o.MDFe.Codigo == codigoMDFe &&
                                     o.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada &&
                                     o.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada);

            return query.Select(o => o.Codigo).Any();
        }

        public bool ExistePorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoMDFe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoMDFe>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo == codigoCarga);

            return query.Select(o => o.Codigo).Any();
        }

        public bool ExisteMDFeSemAverbacaoValidaPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoMDFe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoMDFe>();
            IQueryable<Dominio.Entidades.MDFeSeguro> querySeguroMDFe = this.SessionNHiBernate.Query<Dominio.Entidades.MDFeSeguro>();
            
            query = query.Where(o => o.CargaPedido.Carga.Codigo == codigoCarga);

            querySeguroMDFe = querySeguroMDFe.Where(o => query.Any(q => q.MDFe == o.MDFe) && (o.NumeroAverbacao == "" || o.NumeroAverbacao == null || o.NumeroAverbacao == "99999"));
            
            return querySeguroMDFe.Select(o => o.Codigo).Any();
        }
    }
}
