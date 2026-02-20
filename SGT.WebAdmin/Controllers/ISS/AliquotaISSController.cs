using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Importacao;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.ISS
{
    [CustomAuthorize("ISS/AliquotaISS")]
    public class AliquotaISSController : BaseController
    {
		#region Construtores

		public AliquotaISSController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                return ObterGridPesquisa(unitOfWork, false);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                return ObterGridPesquisa(unitOfWork, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.ISS.AliquotaISS repositorioAliquotaISS = new Repositorio.Embarcador.ISS.AliquotaISS(unitOfWork);
                Dominio.Entidades.Embarcador.ISS.AliquotaISS aliquotaISS = new Dominio.Entidades.Embarcador.ISS.AliquotaISS();

                PreencherAliquotaISS(aliquotaISS, unitOfWork);

                repositorioAliquotaISS.Inserir(aliquotaISS, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.ISS.AliquotaISS repositorioAliquotaISS = new Repositorio.Embarcador.ISS.AliquotaISS(unitOfWork);
                Dominio.Entidades.Embarcador.ISS.AliquotaISS aliquotaISS = repositorioAliquotaISS.BuscarPorCodigo(codigo, true);

                if (aliquotaISS == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherAliquotaISS(aliquotaISS, unitOfWork);

                repositorioAliquotaISS.Atualizar(aliquotaISS, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.ISS.AliquotaISS repositorioAliquotaISS = new Repositorio.Embarcador.ISS.AliquotaISS(unitOfWork);
                Dominio.Entidades.Embarcador.ISS.AliquotaISS aliquotaISS = repositorioAliquotaISS.BuscarPorCodigo(codigo, false);

                if (aliquotaISS == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var dynAliquotaISS = new
                {
                    aliquotaISS.Codigo,
                    aliquotaISS.Descricao,
                    aliquotaISS.Ativo,
                    aliquotaISS.Aliquota,
                    Localidade = new { Codigo = aliquotaISS.Localidade?.Codigo ?? 0, Descricao = aliquotaISS.Localidade?.Descricao ?? string.Empty },
                    DataInicio = aliquotaISS.DataInicio.ToDateString(),
                    DataFim = aliquotaISS.DataFim.ToDateString(),
                    aliquotaISS.RetemISS
                };

                return new JsonpResult(dynAliquotaISS);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.ISS.AliquotaISS repositorioAliquotaISS = new Repositorio.Embarcador.ISS.AliquotaISS(unitOfWork);
                Dominio.Entidades.Embarcador.ISS.AliquotaISS aliquotaISS = repositorioAliquotaISS.BuscarPorCodigo(codigo, true);

                if (aliquotaISS == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repositorioAliquotaISS.Deletar(aliquotaISS, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfiguracaoImportacao()
        {
            List<ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoAliquotaISS();

            return new JsonpResult(configuracoes.ToList());
        }

        public async Task<IActionResult> Importar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.ISS.AliquotaISS repositorioAliquotaISS = new Repositorio.Embarcador.ISS.AliquotaISS(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repositorioConfiguracao.BuscarConfiguracaoPadrao();


                unitOfWork.Start();

                List<Dominio.Entidades.Embarcador.ISS.AliquotaISS> listaAliquotaISS = new List<Dominio.Entidades.Embarcador.ISS.AliquotaISS>();

                int importados = 0;

                RetornoImportacao retorno = Servicos.Embarcador.Importacao.Importacao.PreencherImportacaoManual(Request, listaAliquotaISS, ((dados) =>
                {
                    Servicos.Embarcador.ISS.ImportacaoAliquotaISS servicoImportacaoAliquotaISS = new Servicos.Embarcador.ISS.ImportacaoAliquotaISS(unitOfWork, TipoServicoMultisoftware, dados, configuracao);

                    importados++;

                    return servicoImportacaoAliquotaISS.ObterAliquotaISSImportar();
                }));

                if (retorno == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao Importar o Arquivo.");

                foreach (Dominio.Entidades.Embarcador.ISS.AliquotaISS aliquotaISS in listaAliquotaISS)
                    repositorioAliquotaISS.Inserir(aliquotaISS, Auditado);


                unitOfWork.CommitChanges();

                retorno.Importados = importados;

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao Importar o Arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherAliquotaISS(Dominio.Entidades.Embarcador.ISS.AliquotaISS aliquotaISS, Repositorio.UnitOfWork unitOfWork)
        {
            int codigoLocalidade = Request.GetIntParam("Localidade");

            Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(unitOfWork);

            aliquotaISS.Descricao = Request.GetStringParam("Descricao");
            aliquotaISS.Ativo = Request.GetBoolParam("Ativo");
            aliquotaISS.Localidade = codigoLocalidade > 0 ? repositorioLocalidade.BuscarPorCodigo(codigoLocalidade) : null;
            aliquotaISS.DataInicio = Request.GetNullableDateTimeParam("DataInicio");
            aliquotaISS.DataFim = Request.GetNullableDateTimeParam("DataFim");
            aliquotaISS.RetemISS = Request.GetBoolParam("RetemISS");
            aliquotaISS.Aliquota = Request.GetDecimalParam("Aliquota");
        }

        private Dominio.ObjetosDeValor.Embarcador.ISS.FiltroPesquisaAliquotaISS ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.ISS.FiltroPesquisaAliquotaISS()
            {
                Descricao = Request.GetStringParam("Descricao"),
                Ativo = Request.GetEnumParam("Ativo", SituacaoAtivoPesquisa.Ativo),
                CodigoLocalidade = Request.GetIntParam("Localidade"),
                DataInicio = Request.GetNullableDateTimeParam("DataInicio"),
                DataFim = Request.GetNullableDateTimeParam("DataFim"),
                SituacaoVigencia = Request.GetEnumParam<SituacaoVigencia>("Vigencia")
            };
        }

        private IActionResult ObterGridPesquisa(Repositorio.UnitOfWork unitOfWork, bool exportacao = false)
        {
            Dominio.ObjetosDeValor.Embarcador.ISS.FiltroPesquisaAliquotaISS filtrosPesquisa = ObterFiltrosPesquisa();

            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Descrição", "Descricao", 40, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Munícipio", "DescricaoLocalidade", 40, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Alíquota", "Aliquota", 40, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Retém ISS", "DescricaoRetemISS", 40, Models.Grid.Align.left, true);

            if (filtrosPesquisa.Ativo == SituacaoAtivoPesquisa.Todos)
                grid.AdicionarCabecalho("Status", "DescricaoAtivo", 15, Models.Grid.Align.center, false);


            Repositorio.Embarcador.ISS.AliquotaISS repositorioAliquotaISS = new Repositorio.Embarcador.ISS.AliquotaISS(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
            List<Dominio.Entidades.Embarcador.ISS.AliquotaISS> listaAliquotaISS = repositorioAliquotaISS.Consultar(filtrosPesquisa, parametrosConsulta);
            grid.setarQuantidadeTotal(repositorioAliquotaISS.ContarConsulta(filtrosPesquisa));

            var lista = (from p in listaAliquotaISS
                         select new
                         {
                             p.Codigo,
                             p.Descricao,
                             DescricaoLocalidade = p.Localidade?.Descricao ?? string.Empty,
                             p.Aliquota,
                             DescricaoRetemISS = p.RetemISS.ObterDescricao(),
                             DescricaoAtivo = p.Ativo.ObterDescricaoAtivo()
                         }).ToList();

            grid.AdicionaRows(lista);

            if (exportacao)
            {
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            else
            {
                return new JsonpResult(grid);
            }

        }

        private List<ConfiguracaoImportacao> ConfiguracaoImportacaoAliquotaISS()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

            var configuracoes = new List<ConfiguracaoImportacao>();
            int tamanho = 150;

            configuracoes.Add(new ConfiguracaoImportacao() { Id = 1, Descricao = "Descrição", Propriedade = "Descricao", Tamanho = tamanho, CampoInformacao = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 2, Descricao = "Descrição Município", Propriedade = "DescricaoLocalidade", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 3, Descricao = "Código IBGE Município", Propriedade = "CodigoIBGELocalidade", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 4, Descricao = "Alíquota", Propriedade = "Aliquota", Tamanho = tamanho, CampoInformacao = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 5, Descricao = "Data Inicial", Propriedade = "DataInicial", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 6, Descricao = "Data Final", Propriedade = "DataFinal", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 7, Descricao = "Retém ISS", Propriedade = "RetemISS", Tamanho = tamanho, CampoInformacao = true });

            return configuracoes;
        }

        #endregion
    }
}
