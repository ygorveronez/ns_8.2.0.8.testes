using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas
{
    public class ModeloVeicularCarga : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>
    {
        public ModeloVeicularCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public ModeloVeicularCarga(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga BuscarTracaoReboque()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>();
            var result = from obj in query where obj.ModeloTracaoReboquePadrao select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public async Task<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> BuscarPorCodigoAsync(int codigo, CancellationToken cancellationToken)
        {
            return await this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>()
                .Where(x => x.Codigo == codigo).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>> BuscarPorCodigosAsync(List<int> codigos, int pageSize = 2000)
        {
            List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> resultado = new();

            if (codigos == null || codigos.Count == 0)
                return resultado;

            List<int> codigosUnicos = codigos.Distinct().ToList();

            for (int i = 0; i < codigosUnicos.Count; i += pageSize)
            {
                var bloco = codigosUnicos.Skip(i).Take(pageSize).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>()
                    .Where(obj => bloco.Contains(obj.Codigo));

                var encontrados = await query.ToListAsync(CancellationToken);
                resultado.AddRange(encontrados);
            }

            return resultado;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> BuscarPorCodigos(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>();
            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;
            return result.ToList();
        }
        public List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> BuscarPorCodigo(int[] codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>();
            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> BuscarTodosReboques()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>();
            var result = from obj in query where obj.Ativo && (obj.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCarga.Geral || obj.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCarga.Reboque) select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga buscarPorCodigoIntegracao(string codigoIntegracao, List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> lstModeloVeicularCarga = null)
        {
            if (lstModeloVeicularCarga != null && lstModeloVeicularCarga.Count > 0)
                return lstModeloVeicularCarga.Where(obj => obj.CodigoIntegracao == codigoIntegracao || obj.CodigosIntegracao.Contains(codigoIntegracao)).FirstOrDefault();

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>();
            var result = from obj in query where (obj.CodigoIntegracao == codigoIntegracao || obj.CodigosIntegracao.Contains(codigoIntegracao)) && obj.Ativo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga buscarPorCodigoIntegracaoEEixo(string codigoIntegracao, string eixo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>();
            var result = from obj in query where (obj.CodigoIntegracao == codigoIntegracao || obj.CodigosIntegracao.Contains(codigoIntegracao)) && obj.Ativo && obj.Descricao == eixo || obj.Descricao.Contains(eixo) select obj;
            return result.FirstOrDefault();
        }
        public List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> buscarPorCodigosIntegracaoEDescricoesComFetch(List<string> codigosIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>();
            var result = from obj in query
                         where (codigosIntegracao.Contains(obj.CodigoIntegracao) || codigosIntegracao.Contains(obj.Descricao) ||
                         obj.CodigosIntegracao.Any(item => codigosIntegracao.Contains(item))) && obj.Ativo
                         select obj;
            result = result.Fetch(obj => obj.CodigosIntegracao);
            return result.ToList();
        }


        public List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> BuscarPorCodigosIntegracao(List<string> codigoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>();
            var result = from obj in query where codigoIntegracao.Contains(obj.CodigoIntegracao) && obj.Ativo select obj;
            return result.ToList();
        }

        public async Task<List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>> BuscarPorCodigosIntegracaoAsync(List<string> codigoIntegracao, int pageSize = 2000)
        {
            List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> resultado = new();

            if (codigoIntegracao == null || codigoIntegracao.Count == 0)
                return resultado;

            var codigosUnicos = codigoIntegracao.Select(c => c.Trim()).Distinct().ToList();

            for (int i = 0; i < codigosUnicos.Count; i += pageSize)
            {
                var bloco = codigosUnicos.Skip(i).Take(pageSize).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>()
                    .Where(obj => bloco.Contains(obj.CodigoIntegracao) && obj.Ativo);

                var encontrados = await query.ToListAsync(CancellationToken);
                resultado.AddRange(encontrados);
            }

            return resultado;
        }

        public Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga buscarPorDescricao(string descricao, List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> lstModeloVeicularCarga = null)
        {
            if (lstModeloVeicularCarga != null && lstModeloVeicularCarga.Count > 0)
                return lstModeloVeicularCarga.Where(obj => obj.Descricao == descricao && obj.Ativo).FirstOrDefault();

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>();
            var result = from obj in query where obj.Descricao == descricao && obj.Ativo select obj;
            return result.FirstOrDefault();
        }

        public async Task<List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>> BuscarPorDescricoesAsync(List<string> descricoes, int pageSize = 2000)
        {
            if (descricoes == null || descricoes.Count == 0)
                return new List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>();

            var descricoesUnicas = descricoes.Select(d => d.Trim()).Distinct().ToList();

            List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> resultado = new();

            for (int i = 0; i < descricoesUnicas.Count; i += pageSize)
            {
                var bloco = descricoesUnicas.Skip(i).Take(pageSize).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>()
                    .Where(m => bloco.Contains(m.Descricao) && m.Ativo);

                var encontrados = await query.ToListAsync(CancellationToken);
                resultado.AddRange(encontrados);
            }

            return resultado;
        }

        public Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga BuscarPorDescricaoETipo(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCarga tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>();
            var result = from obj in query where obj.Descricao == descricao && obj.Ativo && obj.Tipo == tipo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga BuscarPorCodigoEmbarcadorDiferenteDe(string codigoIntegracao, int modelo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>();
            var result = from obj in query where obj.CodigosIntegracao.Contains(codigoIntegracao) && obj.Codigo != modelo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> Consultar(List<int> codigos, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCarga> tipos, decimal? capacidadePesoTransporte, List<int> modelos, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, int codigoTipoCarga = 0, int codigoEmpresa = 0, string codigoIntegracao = "")
        {
            var result = MontarConsulta(codigos, descricao, ativo, tipos, capacidadePesoTransporte, modelos, codigoTipoCarga, codigoEmpresa, codigoIntegracao);
            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(List<int> codigos, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCarga> tipos, decimal? capacidadePesoTransporte, List<int> modelos, int codigoTipoCarga = 0, int codigoEmpresa = 0, string codigoIntegracao = "")
        {
            var result = MontarConsulta(codigos, descricao, ativo, tipos, capacidadePesoTransporte, modelos, codigoTipoCarga, codigoEmpresa, codigoIntegracao);

            return result.Count();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> MontarConsulta(List<int> codigos, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCarga> tipos, decimal? capacidadePesoTransporte, List<int> modelos, int codigoTipoCarga, int codigoEmpresa, string codigoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>();

            var result = from obj in query select obj;

            if (codigos != null && codigos.Count > 0)
                result = result.Where(o => codigos.Contains(o.Codigo));

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(mvc => mvc.Descricao.Contains(descricao));

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);
            else if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);

            if (capacidadePesoTransporte.HasValue)
                result = result.Where(mvc => (mvc.CapacidadePesoTransporte + mvc.ToleranciaPesoExtra) >= capacidadePesoTransporte.Value &&
                                             (mvc.CapacidadePesoTransporte - mvc.ToleranciaPesoMenor) <= capacidadePesoTransporte.Value);

            if (tipos != null && tipos.Count > 0)
                result = result.Where(o => tipos.Contains(o.Tipo));

            if (codigoTipoCarga > 0)
                result = result.Where(o => o.TiposCarga.Any(tipoCarga => tipoCarga.TipoDeCarga.Codigo == codigoTipoCarga));

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (!string.IsNullOrWhiteSpace(codigoIntegracao))
                result = result.Where(mvc => mvc.CodigoIntegracao == codigoIntegracao);

            if (modelos != null && modelos.Count > 0)
                result = result.Where(mvc => modelos.Contains(mvc.Codigo));


            return result;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> BuscarTodosAtivos()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>();

            var result = from obj in query where obj.Ativo select obj;

            return result.ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular> BuscarAtivos()
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>();

            query = query.Where(o => o.Ativo);

            return query.Select(o => new Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular()
            {
                CodigoIntegracao = o.CodigoIntegracao,
                Descricao = o.Descricao,
                TipoModeloVeicular = o.Tipo
            }).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> BuscarModelosVeicularCargaPendenteIntegracao(int quantidade)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>();

            query = query.Where(o => ((bool?)o.IntegradoERP ?? false) == false);

            return query.Take(quantidade).ToList();
        }
        public int ObterTotalDosModelosPententesIntegracaoFaltantes()
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>();
            query = query.Where(o => ((bool?)o.IntegradoERP ?? false) == false);
            return query.Count();
        }

        #region Relatórios

        public IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.ModeloVeicularCarga.ModeloVeicularCarga> ConsultarRelatorioModeloVeicularCarga(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaModeloVeicularCarga filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = new ConsultaModeloVeicularCarga().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Cargas.ModeloVeicularCarga.ModeloVeicularCarga)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Cargas.ModeloVeicularCarga.ModeloVeicularCarga>();
        }

        public int ContarConsultaRelatorioModeloVeicularCarga(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaModeloVeicularCarga filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var query = new ConsultaModeloVeicularCarga().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        #endregion
    }
}
