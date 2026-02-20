using Dominio.Excecoes.Embarcador;
using Repositorio.Embarcador.Configuracoes;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Configuracoes
{
    [CustomAuthorize("Configuracoes/Palletizacao")]
    public class PalletizacaoController : BaseController
    {
		#region Construtores

		public PalletizacaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Palletizacao repPalletizacao = new Repositorio.Embarcador.Configuracoes.Palletizacao(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.Palletizacao palletizacao = new Dominio.Entidades.Embarcador.Configuracoes.Palletizacao();

                PreencherDados(palletizacao, unitOfWork);

                repPalletizacao.Inserir(palletizacao, Auditado);

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
                Palletizacao repPalletizacao = new Repositorio.Embarcador.Configuracoes.Palletizacao(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.Palletizacao palletizacao = repPalletizacao.BuscarPorCodigo(codigo);

                PreencherDados(palletizacao, unitOfWork);

                if (palletizacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repPalletizacao.Atualizar(palletizacao, Auditado);

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

                Palletizacao repPalletizacao = new Repositorio.Embarcador.Configuracoes.Palletizacao(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.Palletizacao palletizacao  = repPalletizacao.BuscarPorCodigo(codigo);


                if (palletizacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    palletizacao.Codigo,
                    Pessoa = new { palletizacao.Pessoa?.Descricao, palletizacao.Pessoa?.Codigo },
                    GrupoPessoas = new { palletizacao.GrupoPessoas?.Descricao, palletizacao.GrupoPessoas?.Codigo },
                    palletizacao.Descricao,
                    palletizacao.CodigoIntegracao,
                    palletizacao.Ativo,
                    palletizacao.TipoPessoa,
                    palletizacao.TipoPalletizacao,
                    palletizacao.Altura,
                    palletizacao.Largura,
                    palletizacao.Comprimento,
                    palletizacao.PalletMisto
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
                Palletizacao repPalletizacao = new Repositorio.Embarcador.Configuracoes.Palletizacao(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.Palletizacao palletizacao = repPalletizacao.BuscarPorCodigo(codigo, auditavel: true);

                if (palletizacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repPalletizacao.Deletar(palletizacao, Auditado);

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

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                var grid = ObterGridPesquisa();

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

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Código integração", "CodigoIntegracao", 30, Models.Grid.Align.left, true);

                Palletizacao repPalletizacao = new Repositorio.Embarcador.Configuracoes.Palletizacao(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Configuracoes.FiltroPesquisaPalletizacao filtrosPesquisa = ObterFiltrosPesquisa();

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                int totalRegistros = repPalletizacao.ContarConsulta(filtrosPesquisa);

                List<Dominio.Entidades.Embarcador.Configuracoes.Palletizacao> ListaPalletizacao = totalRegistros > 0 ? repPalletizacao.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Configuracoes.Palletizacao>();

                var lista = (from p in ListaPalletizacao
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 p.CodigoIntegracao,
                             }).ToList();

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Configuracoes.FiltroPesquisaPalletizacao ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Configuracoes.FiltroPesquisaPalletizacao()
            {
                CodigoIntegracao = Request.GetStringParam("CodigoIntegracao"),
                Descricao = Request.GetStringParam("Descricao"),
                Ativo = Request.GetNullableBoolParam("Ativo"),
            };
        }

        private void PreencherDados(Dominio.Entidades.Embarcador.Configuracoes.Palletizacao palletizacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
            

            double codigoPessoa = Request.GetDoubleParam("Pessoa");
            int codigoPessoaGrupo = Request.GetIntParam("GrupoPessoas");

            palletizacao.Descricao = Request.GetStringParam("Descricao");
            palletizacao.Codigo = Request.GetIntParam("Codigo");
            palletizacao.Pessoa = repCliente.BuscarPorCPFCNPJ(codigoPessoa);
            palletizacao.GrupoPessoas = repGrupoPessoas.BuscarPorCodigo(codigoPessoaGrupo);
            palletizacao.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
            palletizacao.TipoPalletizacao = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPalletizacao>("TipoPalletizacao");
            palletizacao.TipoPessoa = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa>("TipoPessoa");
            palletizacao.Altura = Request.GetDecimalParam("Altura");
            palletizacao.Largura = Request.GetDecimalParam("Largura");
            palletizacao.Comprimento = Request.GetDecimalParam("Comprimento");
            palletizacao.PalletMisto = Request.GetBoolParam("PalletMisto");
            palletizacao.Ativo = Request.GetBoolParam("Ativo");

        }
        #endregion
    }
}
