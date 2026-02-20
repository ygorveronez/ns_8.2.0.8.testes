using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Pessoas
{
    public class GrupoPessoas : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas>
    {
        public GrupoPessoas(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public GrupoPessoas(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> ConsultarTodas(string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas>();
            var result = from obj in query select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

        public List<int> BuscarCodigosGrupoPessoaIntegrarEmbarcador()
        {
            IQueryable<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas>();

            query = query.Where(o => o.Ativo && o.UtilizaMultiEmbarcador == true && o.HabilitarIntegracaoOcorrenciasTMSWSMultiEmbarcador == true);

            return query.Select(o => o.Codigo).ToList();
        }

        public bool ExisteGrupoPessoaPorDescricao(string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas>()
                .Where(obj => obj.Descricao == descricao);

            return query.ToList().Count > 0;
        }

        public int ContarConsultaTodas()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas>();
            var result = from obj in query select obj;

            return result.Count();
        }

        public Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas BuscarPrimeiroGrupoCliente(Dominio.Entidades.Cliente cliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas>();
            var result = from obj in query where obj.Clientes.Contains(cliente) select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas BuscarGrupoCliente(double cnpj)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();
            var result = from obj in query where obj.CPF_CNPJ == cnpj select obj;
            return result.Select(o => o.GrupoPessoas)?.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
        public async Task<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> BuscarPorCodigoAsync(int codigo)
        {
            return await this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas>()
                .Where(x => x.Codigo == codigo).FirstOrDefaultAsync();
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> BuscarPorCodigos(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas>();
            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;
            return result.ToList();
        }

        public async Task<List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas>> BuscarPorCodigosAsync(List<int> codigos)
        {
            return await this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas>()
                .Where(x => codigos.Contains(x.Codigo))
                .ToListAsync();
        }

        public List<string> BuscarDescricaoPorCodigo(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas>();
            query = query.Where(obj => codigos.Contains(obj.Codigo));
            return query.Select(o => o.Descricao).ToList();
        }

        public async Task<List<string>> BuscarDescricaoPorCodigoAsync(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas>();
            query = query.Where(obj => codigos.Contains(obj.Codigo));
            return await query.Select(o => o.Descricao).ToListAsync();
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> BuscarQueNaoImportaDocumentoDestinadoTransporte()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas>();
            var result = from obj in query where obj.NaoImportarDocumentoDestinadoTransporte == true select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> BuscarGrupoPessoasEnvioEmailCadastroVeiculo()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas>();
            var result = from obj in query where obj.EnviarNovoVeiculoEmail == true select obj;
            return result.ToList();
        }

        public bool ContemPessoasBloqueadasFinanceiramento(int codigoGrupo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();
            var result = from obj in query where obj.GrupoPessoas.Codigo == codigoGrupo && obj.SituacaoFinanceira == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFinanceira.Bloqueada select obj;
            return result.Any();
        }

        public bool ContemGrupoPessoasEnvioEmailCadastroVeiculo()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas>();
            var result = from obj in query where obj.EnviarNovoVeiculoEmail == true select obj;
            return result.Any();
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> BuscarPorCodigo(int[] codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas>();
            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;
            return result.ToList();
        }

        public async Task<List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas>> BuscarPorCodigoAsync(int[] codigos)
        {
            return await this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas>()
                .Where(x => codigos.Contains(x.Codigo)).ToListAsync();
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> BuscarPorCodigo(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas>();
            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Cliente> BuscarClientesPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();
            var result = from obj in query where obj.GrupoPessoas.Codigo == codigo select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas BuscarPorCodigoIntegracao(string codigoIntegracao, List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> lstGrupoPessoas = null)
        {
            if (lstGrupoPessoas != null)
                return lstGrupoPessoas.Where(x => x.CodigoIntegracao == codigoIntegracao).FirstOrDefault();

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas>();
            var result = from obj in query where obj.CodigoIntegracao == codigoIntegracao select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas BuscarPorTipoIntegracao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao, bool ativo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas>();

            var result = from obj in query where obj.TipoIntegracao.Tipo == tipoIntegracao && obj.Ativo == ativo select obj;

            return result.FirstOrDefault();
        }

        public List<string> BuscarListaCNPJRaiz(int codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasRaizCNPJ>();
            var result = from obj in query where obj.GrupoPessoas.Codigo == codigos select obj;
            return result.Select(obj => obj.RaizCNPJ).ToList();
        }

        public Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas BuscarPorRaizCNPJ(string raizCNPJ, List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> lstGrupoPessoas = null)
        {
            if (lstGrupoPessoas != null)
                return lstGrupoPessoas.Where(grupoPessoas => grupoPessoas.GrupoPessoasRaizesCNPJ.Any(o => o.RaizCNPJ.Equals(raizCNPJ))).FirstOrDefault();
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas>().Where(grupoPessoas => grupoPessoas.GrupoPessoasRaizesCNPJ.Any(o => o.RaizCNPJ.Equals(raizCNPJ))).FirstOrDefault();
        }

        public int BuscarCodigoPorRaizCNPJ(string raizCNPJ)
        {
            var consultaGrupoPessoas = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas>()
                .Where(grupoPessoas => grupoPessoas.GrupoPessoasRaizesCNPJ.Any(o => o.RaizCNPJ.Equals(raizCNPJ)));

            return consultaGrupoPessoas.Select(grupoPessoas => grupoPessoas.Codigo).FirstOrDefault();
        }

        public void AtualizarGrupoPessoaCliente(int codigoGrupoPessoas, string raizCnpj)
        {
            if (codigoGrupoPessoas == 0 || string.IsNullOrEmpty(raizCnpj) || raizCnpj.Length > 8)
                return;

            List<string> raizes = new List<string>();
            raizes.Add(raizCnpj);

            if (raizCnpj[0] == '0')
                raizes.Add(raizCnpj.Substring(1));

            var querySql = $@"UPDATE T_CLIENTE SET GRP_CODIGO = {codigoGrupoPessoas} WHERE CASE 
                                    WHEN LEN(cast((convert(bigint,CLI_CGCCPF)) as varchar(20))) < 14 THEN REPLACE(CAST(LEFT(CONVERT(VARCHAR(50), CLI_CGCCPF, 3), 8) AS VARCHAR), '.', '')
                                    WHEN LEN(cast((convert(bigint,CLI_CGCCPF)) as varchar(20))) >= 14 THEN REPLACE(CAST(LEFT(CONVERT(VARCHAR(50), CLI_CGCCPF, 3), 9) AS VARCHAR), '.', '')
                                END
                                IN ({string.Join(", ", raizes.Select(item => "\'" + item + "\'"))})";

            var query = this.SessionNHiBernate.CreateSQLQuery(querySql);

            query.ExecuteUpdate();
        }

        public IList<string> ObterRaizesExistentes(List<string> raizesCnpj)
        {

            var querySql = "SELECT obj.RaizCNPJ FROM GrupoPessoasRaizCNPJ obj WHERE obj.RaizCNPJ IN (:Raizes)";
            var query = this.SessionNHiBernate.CreateQuery(querySql);
            query.SetParameterList("Raizes", raizesCnpj);

            return query.SetTimeout(600).List<string>();
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> Consultar(Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaGrupoPessoas filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> consultaGrupoPessoas = Consultar(filtrosPesquisa);

            return ObterLista(consultaGrupoPessoas, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaGrupoPessoas filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> consultaGrupoPessoas = Consultar(filtrosPesquisa);

            return consultaGrupoPessoas.Count();
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> BuscarQueArmazenaCanhotoCTe()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas>();

            query = query.Where(o => o.ArmazenaCanhotoFisicoCTe == true);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> BuscarGruposComIntegracaoVeiculosMultiEmbarcador()
        {
            IQueryable<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas>();

            query = query.Where(o => o.UtilizaMultiEmbarcador.Value && o.HabilitarIntegracaoVeiculoMultiEmbarcador.Value);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas BuscarPorDescricao(string descricaoGrupoPessoas)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas>();

            query = query.Where(o => o.Descricao == descricaoGrupoPessoas && o.Ativo);

            return query.FirstOrDefault();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> Consultar(Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaGrupoPessoas filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                result = result.Where(obj => obj.Descricao.Contains(filtrosPesquisa.Descricao));

            if ((int)filtrosPesquisa.TipoGrupoPessoas > 0)
                result = result.Where(obj => obj.TipoGrupoPessoas == filtrosPesquisa.TipoGrupoPessoas);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.RaizCNPJ))
                result = result.Where(obj => obj.GrupoPessoasRaizesCNPJ.Any(o => o.RaizCNPJ.Equals(filtrosPesquisa.RaizCNPJ)));

            if (filtrosPesquisa.Ativo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
            {
                if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                    result = result.Where(obj => obj.Ativo);
                else if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                    result = result.Where(obj => !obj.Ativo);
            }

            if (filtrosPesquisa.CnpjCpfCliente > 0)
                result = result.Where(obj => obj.Clientes.Any(o => o.CPF_CNPJ == filtrosPesquisa.CnpjCpfCliente));

            if (filtrosPesquisa.CodigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == filtrosPesquisa.CodigoEmpresa);

            if (filtrosPesquisa.ListaCodigoGrupoPessoasPermitidos?.Count > 0)
                result = result.Where(o => filtrosPesquisa.ListaCodigoGrupoPessoasPermitidos.Contains(o.Codigo));

            return result;
        }

        #endregion

        #region Relatórios

        public IList<Dominio.Relatorios.Embarcador.DataSource.Pessoas.GrupoPessoas> ConsultarRelatorioGrupoPessoas(Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaRelatorioGrupoPessoas filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaGrupoPessoas = new ConsultaGrupoPessoas().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaGrupoPessoas.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Pessoas.GrupoPessoas)));

            return consultaGrupoPessoas.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Pessoas.GrupoPessoas>();
        }

        public int ContarConsultaRelatorioGrupoPessoas(Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaRelatorioGrupoPessoas filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consultaGrupoPessoas = new ConsultaGrupoPessoas().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaGrupoPessoas.SetTimeout(600).UniqueResult<int>();
        }

        #endregion
    }
}
