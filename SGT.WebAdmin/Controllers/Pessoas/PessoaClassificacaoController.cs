using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pessoas
{
    [CustomAuthorize("Pessoas/PessoaClassificacao")]
    public class PessoaClassificacaoController : BaseController
    {
		#region Construtores

		public PessoaClassificacaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.Entidades.Embarcador.Pessoas.PessoaClassificacao pessoaClassificacao = new Dominio.Entidades.Embarcador.Pessoas.PessoaClassificacao();

                PreencherPessoaClassificacao(pessoaClassificacao);

                unitOfWork.Start();

                Repositorio.Embarcador.Pessoas.PessoaClassificacao repositorio = new Repositorio.Embarcador.Pessoas.PessoaClassificacao(unitOfWork);

                repositorio.Inserir(pessoaClassificacao, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
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
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Pessoas.PessoaClassificacao repositorio = new Repositorio.Embarcador.Pessoas.PessoaClassificacao(unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.PessoaClassificacao pessoaClassificacao = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (pessoaClassificacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherPessoaClassificacao(pessoaClassificacao);

                unitOfWork.Start();

                repositorio.Atualizar(pessoaClassificacao, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
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
                Repositorio.Embarcador.Pessoas.PessoaClassificacao repositorio = new Repositorio.Embarcador.Pessoas.PessoaClassificacao(unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.PessoaClassificacao pessoaClassificacao = repositorio.BuscarPorCodigo(codigo, auditavel: false);

                if (pessoaClassificacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    pessoaClassificacao.Codigo,
                    pessoaClassificacao.Descricao,
                    pessoaClassificacao.Classe,
                    pessoaClassificacao.Cor
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
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
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Pessoas.PessoaClassificacao repositorio = new Repositorio.Embarcador.Pessoas.PessoaClassificacao(unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.PessoaClassificacao pessoaClassificacao = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (pessoaClassificacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repositorio.Deletar(pessoaClassificacao, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisa();
                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherPessoaClassificacao(Dominio.Entidades.Embarcador.Pessoas.PessoaClassificacao pessoaClassificacao)
        {
            pessoaClassificacao.Classe = Request.GetEnumParam<PessoaClasse>("Classe");
            pessoaClassificacao.Cor = Request.GetStringParam("Cor");
            pessoaClassificacao.Descricao = Request.GetStringParam("Descricao");

            if (pessoaClassificacao.Cor == "#FFFFFF")
                pessoaClassificacao.Cor = string.Empty;
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 45, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Classe", "Classe", 15, Models.Grid.Align.left, true);

                string descricao = Request.GetStringParam("Descricao");
                PessoaClasse? classe = Request.GetNullableEnumParam<PessoaClasse>("Classe");
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.Pessoas.PessoaClassificacao repositorio = new Repositorio.Embarcador.Pessoas.PessoaClassificacao(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(descricao, classe);
                List<Dominio.Entidades.Embarcador.Pessoas.PessoaClassificacao> listaPessoaClassificacao = (totalRegistros > 0) ? repositorio.Consultar(descricao, classe, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Pessoas.PessoaClassificacao>();

                var listaPessoaClassificacaoRetornar = (
                    from classificacao in listaPessoaClassificacao
                    select new
                    {
                        classificacao.Codigo,
                        classificacao.Descricao,
                        Classe = classificacao.Classe.ObterDescricao()
                    }
                ).ToList();

                grid.AdicionaRows(listaPessoaClassificacaoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        #endregion
    }
}
