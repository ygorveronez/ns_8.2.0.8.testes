using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaTabelaFreteCliente : Repositorio.RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteCliente>
    {
        public CargaTabelaFreteCliente(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public CargaTabelaFreteCliente(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteCliente BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteCliente>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<string> BuscarCodigosIntegracaoPorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteCliente>();
            var result = from obj in query where obj.Carga.Codigo == carga select obj.TabelaFreteCliente.CodigoIntegracao;
            return result.ToList();
        }

        public List<string> BuscarCodigosIntegracaoConfiguracaoTabelaFretePorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteCliente>();
            var result = from obj in query where obj.Carga.Codigo == carga select obj.TabelaFreteCliente.TabelaFrete.CodigoIntegracao;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteCliente> BuscarPorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteCliente>();
            var result = from obj in query where obj.Carga.Codigo == carga select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteCliente BuscarPrimeiroPorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteCliente>();
            var result = from obj in query where obj.Carga.Codigo == carga select obj;
            return result.FirstOrDefault();
        }
        public async Task<Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteCliente> BuscarPrimeiroPorCargaAsync(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteCliente>();
            var result = from obj in query where obj.Carga.Codigo == carga select obj;
            return await result.FirstOrDefaultAsync();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteCliente BuscarPorCarga(int carga, bool tabelaFreteFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteCliente>();
            var result = from obj in query where obj.Carga.Codigo == carga && obj.TabelaFreteFilialEmissora == tabelaFreteFilialEmissora select obj;
            return result.Fetch(obj => obj.Carga).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteCliente> BuscarPorTabelaEPeriodo(int codigoTabelaFrete, DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteCliente>();

            query = query.Where(o => o.Carga.DataCriacaoCarga >= dataInicial.Date && o.Carga.DataCriacaoCarga < dataFinal.AddDays(1).Date && o.TabelaFreteCliente.Codigo == codigoTabelaFrete);

            query = query.Where(o => o.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao ||
                                     o.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgImpressaoDocumentos ||
                                     o.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte ||
                                     o.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada);

            return query.ToList();
        }

        #endregion
    }
}
