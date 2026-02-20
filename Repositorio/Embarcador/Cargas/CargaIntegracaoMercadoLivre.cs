using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaIntegracaoMercadoLivre : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre>
    {
        public CargaIntegracaoMercadoLivre(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Globais

        public Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre BuscarPorCodigo(int codigoCargaIntegracaoMercadoLivre)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre>();

            query = query.Where(o => o.Codigo == codigoCargaIntegracaoMercadoLivre);

            return query.Fetch(o => o.HandlingUnit)
                        .Fetch(o => o.Carga)
                        .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre> BuscarPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre BuscarPorHandlingUnitIDECarga(string handlingUnitID, int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre>();

            query = query.Where(o => o.HandlingUnit.ID == handlingUnitID && o.Carga.Codigo == codigoCarga);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre BuscarPorRotaEFacilityECarga(int rota, string facility, int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre>();

            query = query.Where(o => o.HandlingUnit.Rota == rota && o.HandlingUnit.Facility == facility && o.Carga.Codigo == codigoCarga);

            return query.FirstOrDefault();
        }

        public string BuscarNumeroCargaPorHandlingUnit(int codigoHandlingUnit)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre>();

            query = query.Where(o => o.HandlingUnit.Codigo == codigoHandlingUnit &&
                                     o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada &&
                                     o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada);

            return query.Select(o => o.Carga.CodigoCargaEmbarcador).FirstOrDefault();
        }

        public List<int> BuscarCodigosPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.MercadoLivreHandlingUnitSituacao situacao, int limiteRegistros, int[] codigosDiff = null)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre>();

            query = query.Where(o => o.HandlingUnit.Situacao == situacao);

            if (codigosDiff != null && codigosDiff.Length > 0)
                query = query.Where(o => !codigosDiff.Contains(o.Codigo));

            return query.Select(o => o.Codigo).Take(limiteRegistros).ToList();
        }

        public List<int> BuscarCodigosPorSituacaoAgConfirmacao(int limiteRegistros)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre>();

            query = query.Where(o => o.HandlingUnit.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MercadoLivreHandlingUnitSituacao.AgConfirmacao 
                                    && o.HandlingUnit.DataConfirmarProcessamento <= System.DateTime.Now);

            return query.Select(o => o.Codigo).Take(limiteRegistros).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre> Consultar(int codigoCarga, string propOrdenar, string dirOrdenar, int inicio, int limite)
        {
            return ObterQueryConsulta(codigoCarga, propOrdenar, dirOrdenar, inicio, limite).ToList();
        }

        public int ContarConsulta(int codigoCarga)
        {
            return ObterQueryConsulta(codigoCarga).Count();
        }

        public bool ExisteInvalidoPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.HandlingUnit.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.MercadoLivreHandlingUnitSituacao.Sucesso);

            return query.Select(o => o.Codigo).Any();
        }

        public bool ExisteComSituacaoPorCarga(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MercadoLivreHandlingUnitSituacao situacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.HandlingUnit.Situacao == situacao);

            return query.Select(o => o.Codigo).Any();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre> ObterQueryConsulta(int codigoCarga, string propOrdenar = "", string dirOrdenar = "", int inicio = 0, int limite = 0)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga);

            if (!string.IsNullOrWhiteSpace(propOrdenar) && !string.IsNullOrWhiteSpace(dirOrdenar))
                query = query.OrderBy(propOrdenar + " " + dirOrdenar);

            if (inicio > 0 || limite > 0)
                query = query.Skip(inicio).Take(limite);

            return query;
        }

        #endregion
    }
}
