using Dominio.ObjetosDeValor.Embarcador.Consulta;
using Dominio.ObjetosDeValor.Embarcador.Ocorrencia;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas.ControleEntrega
{
    public class OcorrenciaColetaEntrega : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega>
    {
        public OcorrenciaColetaEntrega(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public OcorrenciaColetaEntrega(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }


        public int ObterVolumesTotais(int codigoControleEntrega, int tipoOcorrencia)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega>();
            var result = query.Where(obj => obj.CargaEntrega.Codigo == codigoControleEntrega && obj.TipoDeOcorrencia.Codigo == tipoOcorrencia);
            return result.Sum(obj => obj.Volumes);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega> BuscarPorControleEntrega(int codigoControleEntrega)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega>();
            var result = query.Where(obj => obj.CargaEntrega.Codigo == codigoControleEntrega);
            return result
                .Fetch(obj => obj.TipoDeOcorrencia)
                .ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega>> BuscarPorCargaAsync(int codigoCarga)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega>().Where(obj => obj.CargaEntrega.Carga.Codigo == codigoCarga);

            return query.Fetch(obj => obj.TipoDeOcorrencia).ToListAsync(CancellationToken);
        }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega BuscarEmTransitoPorCargaEntrega(int codigoCargaEntrega)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega>();

            var result = query.Where(obj => obj.CargaEntrega.Codigo == codigoCargaEntrega
                        && obj.TipoDeOcorrencia.Ativo
                        && (obj.TipoDeOcorrencia.CodigoProceda.Equals("001") || obj.TipoDeOcorrencia.Descricao.Equals("ENTREGA REALIZADA")));

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega> BuscarPorControleEntregas(List<int> codigosControleEntrega)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega>();
            var result = query.Where(obj => codigosControleEntrega.Contains(obj.CargaEntrega.Codigo));
            return result
                .Fetch(obj => obj.TipoDeOcorrencia)
                .OrderByDescending(obj => obj.DataOcorrencia)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega> BuscarPorCargaEntregaCodigoCarga(int codigoCarga)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega>();
            var result = query.Where(obj => obj.CargaEntrega.Carga.Codigo == codigoCarga);
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega> BuscarPendentesEnvio(int maximo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega>();
            var result = from obj in query where obj.PendenteEnvioEmail select obj;

            if (maximo > 0)
                result = result.Take(maximo);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega> BuscarVisiveisAoClientePorCargaEntrega(int codigoCargaEntrega)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega>();
            var result = query.Where(obj => obj.CargaEntrega.Codigo == codigoCargaEntrega && obj.TipoDeOcorrencia.NaoIndicarAoCliente != true);

            return result
                .Fetch(obj => obj.TipoDeOcorrencia)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega> Consultar(FiltroPesquisaOcorrencia filtrosPesquisa, ParametroConsulta parametrosConsulta)
        {
            var query = _Consultar(filtrosPesquisa);

            return ObterLista(query, parametrosConsulta);
        }

        public int ContarConsulta(FiltroPesquisaOcorrencia filtrosPesquisa)
        {
            var query = _Consultar(filtrosPesquisa);

            return query.Count();
        }

        public void ExcluirTodosPorCargaEntrega(int codigoCargaEntrega)
        {
            UnitOfWork.Sessao.CreateQuery("DELETE OcorrenciaColetaEntrega obj WHERE obj.CargaEntrega IN (:cargaEntrega)")
                             .SetInt32("cargaEntrega", codigoCargaEntrega)
                             .ExecuteUpdate();
        }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega BuscarPorCargaEntrega(int codigoCargaEntrega)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega>();
            var result = query.Where(obj => obj.CargaEntrega.Codigo == codigoCargaEntrega);
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega> BuscarPorCodigosEntregas(List<int> codigoCargasEntrega)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega>();
            var result = query.Where(obj => codigoCargasEntrega.Contains(obj.CargaEntrega.Codigo));
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega> BuscarPorOcorrencia(int codigoOcorrencia)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega>()
                .Where(o => o.TipoDeOcorrencia.Codigo == codigoOcorrencia);

            return query.ToList();
        }


        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega BuscarPorTipoOcorrenciaCargaEntrega(int codigoTipoOcorrencia, int codigoCargaEntrega)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega>()
                .Where(o => o.CargaEntrega.Codigo == codigoCargaEntrega && o.TipoDeOcorrencia.Codigo == codigoTipoOcorrencia);

            return query.OrderByDescending(x => x.DataOcorrencia).FirstOrDefault();
        }

        public bool ExisteOcorrenciaPorCargaEntregaETipoOcorrencia(int codigoCargaEntrega, DateTime dataOcorrencia, int? codigoTipoOcorrencia = null)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega>();

            return query.Any(o =>
                o.CargaEntrega.Codigo == codigoCargaEntrega &&
                o.TipoDeOcorrencia.Codigo == codigoTipoOcorrencia &&
                o.DataOcorrencia.Date == dataOcorrencia.Date &&
                o.DataOcorrencia.Hour == dataOcorrencia.Hour &&
                o.DataOcorrencia.Minute == dataOcorrencia.Minute
            );
        }



        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega> _Consultar(FiltroPesquisaOcorrencia filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega>();

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoCarga))
                query = query.Where(obj => obj.CargaEntrega.Carga.CodigoCargaEmbarcador == filtrosPesquisa.CodigoCarga);

            if (filtrosPesquisa.CodigosEmpresa?.Count > 0)
                query = query.Where(obj => obj.CargaEntrega.Carga.Empresa.Codigo == filtrosPesquisa.CodigosEmpresa.FirstOrDefault());

            if (filtrosPesquisa.DataInicial.HasValue)
                query = query.Where(obj => obj.DataOcorrencia >= filtrosPesquisa.DataInicial.Value.Date);

            if (filtrosPesquisa.DataLimite.HasValue)
                query = query.Where(obj => obj.DataOcorrencia <= filtrosPesquisa.DataLimite.Value.Date.AddDays(1).AddMilliseconds(-1));

            if (filtrosPesquisa.CodigoNotaFiscal > 0)
                query = query.Where(obj => obj.CargaEntrega.NotasFiscais.Any(x => x.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo == filtrosPesquisa.CodigoNotaFiscal));

            return query;
        }

        #endregion
    }
}
