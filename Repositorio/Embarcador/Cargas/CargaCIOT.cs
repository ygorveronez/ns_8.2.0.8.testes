using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaCIOT : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaCIOT>
    {
        public CargaCIOT(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public CargaCIOT(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Cargas.CargaCIOT BuscarPorCodigo(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCIOT> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCIOT>();

            query = query.Where(obj => obj.Codigo == codigo);

            return query.Fetch(o => o.CIOT).FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Cargas.CargaCIOT> BuscarPorCodigoAsync(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCIOT> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCIOT>();

            query = query.Where(obj => obj.Codigo == codigo);

            return query.Fetch(o => o.CIOT).FirstOrDefaultAsync(CancellationToken);
        }

        public bool ExistePorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCIOT>();

            query = query.Where(obj => obj.Carga.Codigo == codigoCarga);

            return query.Any();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCIOT BuscarPorCarga(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCIOT> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCIOT>();

            query = query.Where(obj => obj.Carga.Codigo == carga);

            return query.Fetch(o => o.CIOT).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCIOT BuscarPorCargaCIOT(int carga, int ciot)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCIOT> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCIOT>();

            query = query.Where(obj => obj.Carga.Codigo == carga && obj.CIOT.Codigo == ciot);

            return query.Fetch(o => o.CIOT).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCIOT> BuscarPorCargas(List<int> codigosCarga)
        {
            List<Dominio.Entidades.Embarcador.Cargas.CargaCIOT> resultado = new List<Dominio.Entidades.Embarcador.Cargas.CargaCIOT>();
            int take = 1000;
            int start = 0;

            while (start < codigosCarga.Count)
            {
                List<int> tmp = codigosCarga.Skip(start).Take(take).ToList();
                List<Dominio.Entidades.Embarcador.Cargas.CargaCIOT> parcial = SessionNHiBernate
                    .Query<Dominio.Entidades.Embarcador.Cargas.CargaCIOT>().Where(obj => tmp.Contains(obj.Carga.Codigo)).ToList();

                resultado.AddRange(parcial);
                start += take;
            }

            return resultado;
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCIOT BuscarPorNumeroCargaFilial(string numeroCarga, string codigoFilial)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCIOT>();
            var result = from obj in query
                         where obj.Carga.CodigoCargaEmbarcador == numeroCarga && (obj.Carga.Filial.CodigoFilialEmbarcador == codigoFilial || obj.Carga.Filial.OutrosCodigosIntegracao.Contains(codigoFilial))
                         select obj;
            return result.FirstOrDefault();
        }

        public bool ExisteCIOTPorCarga(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCIOT> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCIOT>();

            query = query.Where(obj => obj.Carga.Codigo == carga);

            return query.Any();
        }

        public Task<bool> ExisteCIOTPorCargaAsync(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCIOT> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCIOT>();

            query = query.Where(obj => obj.Carga.Codigo == carga);

            return query.AnyAsync();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCIOT BuscarPorContrato(int contratoFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCIOT>();
            var result = from obj in query where obj.ContratoFrete.Codigo == contratoFrete select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCIOT BuscarPorNumeroContrato(string numeroContrato)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCIOT>();
            var result = from obj in query where obj.CIOT.Numero == numeroContrato select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCIOT> BuscarPorCIOT(int ciot)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCIOT>();
            var result = from obj in query where obj.CIOT.Codigo == ciot select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCIOT BuscarPrimeiroPorCIOT(int ciot)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCIOT> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCIOT>();

            query = query.Where(o => o.CIOT.Codigo == ciot);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCIOT> BuscarPorCIOTAgSerAdicionado(int ciot)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCIOT>();
            var result = from obj in query where obj.CIOT.Codigo == ciot && !obj.CargaAdicionadaAoCIOT select obj;
            return result.ToList();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCIOT> _ConsultarCargasCIOT(int ciot)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCIOT>();

            var result = from obj in query where obj.CIOT.Codigo == ciot select obj;

            // Filtros

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Carga> ConsultarCargasCIOT(int ciot, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _ConsultarCargasCIOT(ciot);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.Select(o => o.Carga).ToList();
        }

        public int ContarConsultaCargasCIOT(int ciot)
        {
            var result = _ConsultarCargasCIOT(ciot);

            return result.Count();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCIOT> _Consultar(int carga, int codigoCiot)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCIOT>();

            var result = from obj in query where obj.Carga.Codigo == carga select obj;

            if (codigoCiot > 0)
                result = from obj in result where obj.CIOT.Codigo == codigoCiot select obj;

            // Filtros

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCIOT> Consultar(int carga, int codigoCiot, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(carga, codigoCiot);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Cargas.CargaCIOT>> ConsultarAsync(int carga, int codigoCiot, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(carga, codigoCiot);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToListAsync(CancellationToken);
        }

        public int ContarConsulta(int carga, int codigoCIOT)
        {
            var result = _Consultar(carga, codigoCIOT);

            return result.Count();
        }

        public Task<int> ContarConsultaAsync(int carga, int codigoCIOT)
        {
            var result = _Consultar(carga, codigoCIOT);

            return result.CountAsync(CancellationToken);
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Documentos.CargaCIOT> ConsultarRelatorio(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaCIOT filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaCargaCIOT = new ConsultaCargaCIOT().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaCargaCIOT.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Documentos.CargaCIOT)));

            return consultaCargaCIOT.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Documentos.CargaCIOT>();
        }

        public int ContarConsultaRelatorio(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaCIOT filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consultaCargaCIOT = new ConsultaCargaCIOT().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaCargaCIOT.SetTimeout(600).UniqueResult<int>();
        }

        public bool ExistePorContratoFrete(int codigoContratoFrete)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCIOT> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCIOT>();

            query = query.Where(o => o.ContratoFrete.Codigo == codigoContratoFrete && o.CIOT.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Cancelado && o.CIOT.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Pendencia);

            return query.Any();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Documentos.CargaCIOTPedido> ConsultarRelatorioCargaCIOTPedido(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaCIOTPedido filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaCargaCIOT = new ConsultaCargaCIOTPedido().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaCargaCIOT.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Documentos.CargaCIOTPedido)));

            return consultaCargaCIOT.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Documentos.CargaCIOTPedido>();
        }

        public int ContarConsultaRelatorioCargaCIOTPedido(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaCIOTPedido filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consultaCargaCIOT = new ConsultaCargaCIOTPedido().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaCargaCIOT.SetTimeout(600).UniqueResult<int>();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCIOT BuscarPorCodigoArquivo(int codigoArquivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCIOT>();

            var result = from obj in query where obj.ArquivosTransacao.Any(o => o.Codigo == codigoArquivo) select obj;

            return result.FirstOrDefault();
        }
    }
}
