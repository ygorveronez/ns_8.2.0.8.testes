using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaIntegracaoNatura : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoNatura>
    {
        public CargaIntegracaoNatura(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoNatura> BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoNatura>();
            query = query.Where(o => o.Carga.Codigo == codigoCarga);
            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoNatura> BuscarPorCargaPedido(int codigoCargaPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoNatura> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoNatura>();

            query = query.Where(o => o.CargaPedido.Codigo == codigoCargaPedido);

            return query.ToList();
        }

        public decimal BuscarValorFretePorCargaPedido(int codigoCargaPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoNatura> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoNatura>();

            query = query.Where(o => o.CargaPedido.Codigo == codigoCargaPedido);

            return query.Sum(o => (decimal?)o.DocumentoTransporte.ValorFrete) ?? 0m;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoNatura> BuscarPorCargaPedidoOuCarga(int codigoCarga, int codigoCargaPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoNatura> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoNatura>();

            query = query.Where(o => o.CargaPedido.Codigo == codigoCargaPedido || (o.CargaPedido == null && o.Carga.Codigo == codigoCarga));

            return query.ToList();
        }

        public int ContarConsultaPorCargaPedidoOuCarga(int codigoCarga, int codigoCargaPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoNatura> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoNatura>();

            query = query.Where(o => o.CargaPedido.Codigo == codigoCargaPedido || (o.CargaPedido == null && o.Carga.Codigo == codigoCarga));

            return query.Count();
        }

        public bool ExistePorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoNatura> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoNatura>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga || o.CargaPedido.Carga.Codigo == codigoCarga);

            return query.Select(o => o.Codigo).Any();
        }

        public bool ExisteDTSemNotaFiscalPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoNatura> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoNatura>();

            query = query.Where(o => (o.Carga.Codigo == codigoCarga || o.CargaPedido.Carga.Codigo == codigoCarga) && !o.DocumentoTransporte.NotasFiscais.Select(n => n.Codigo).Any());

            return query.Select(o => o.Codigo).Any();
        }

        public List<Dominio.Entidades.Embarcador.Integracao.DTNatura> ConsultarPorCargaPedidoOuCarga(int codigoCarga, int codigoCargaPedido, int inicio, int limite)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoNatura> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoNatura>();

            query = query.Where(o => o.CargaPedido.Codigo == codigoCargaPedido || (o.CargaPedido == null && o.Carga.Codigo == codigoCarga));

            return query.Select(o => o.DocumentoTransporte).Skip(inicio).Take(limite).ToList();
        }

        public List<string> BuscarDTsEmCarga(int codigoCarga, IEnumerable<int> codigosDTs)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoNatura> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoNatura>();

            query = query.Where(o => codigosDTs.Contains(o.DocumentoTransporte.Codigo) &&
                                     o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada &&
                                     o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada &&
                                     o.Carga.Codigo != codigoCarga);

            return query.Select(o => o.Carga.CodigoCargaEmbarcador).Distinct().ToList();
        }
    }
}
