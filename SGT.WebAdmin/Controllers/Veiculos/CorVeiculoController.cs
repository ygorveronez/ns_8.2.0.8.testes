using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.Email
{
    [CustomAuthorize("Veiculos/CorVeiculo")]
    public class CorVeiculoController : BaseController
    {
		#region Construtores

		public CorVeiculoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
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
                // Inicia transacao
                unitOfWork.Start();

                Repositorio.Embarcador.Veiculos.CorVeiculo repCorVeiculo = new Repositorio.Embarcador.Veiculos.CorVeiculo(unitOfWork);
                Dominio.Entidades.Embarcador.Veiculos.CorVeiculo corVeiculo = new Dominio.Entidades.Embarcador.Veiculos.CorVeiculo();

                PreencherDados(corVeiculo, unitOfWork);
                repCorVeiculo.Inserir(corVeiculo, Auditado);

                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
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
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Veiculos.CorVeiculo repCorVeiculo = new Repositorio.Embarcador.Veiculos.CorVeiculo(unitOfWork);
                Dominio.Entidades.Embarcador.Veiculos.CorVeiculo corVeiculo = repCorVeiculo.BuscarPorCodigo(codigo);

                PreencherDados(corVeiculo, unitOfWork);

                repCorVeiculo.Atualizar(corVeiculo, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
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

                Repositorio.Embarcador.Veiculos.CorVeiculo repCorVeiculo = new Repositorio.Embarcador.Veiculos.CorVeiculo(unitOfWork);

                Dominio.Entidades.Embarcador.Veiculos.CorVeiculo corVeiculo = repCorVeiculo.BuscarPorCodigo(codigo);

                if (corVeiculo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    corVeiculo.Codigo,
                    Situacao = corVeiculo.Ativo,
                    CodigoIntegracao = corVeiculo.CodigoIntegracao,
                    Descricao = corVeiculo.Descricao

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
                int codigo = int.Parse(Request.Params("codigo"));

                Repositorio.Embarcador.Veiculos.CorVeiculo repCorVeiculo = new Repositorio.Embarcador.Veiculos.CorVeiculo(unitOfWork);

                Dominio.Entidades.Embarcador.Veiculos.CorVeiculo corVeiculo = repCorVeiculo.BuscarPorCodigo(codigo);

                repCorVeiculo.Deletar(corVeiculo, Auditado);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possivel excluir registro!");
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExcluir);
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados 

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Situação", "Situacao", 30, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Descrição", "Descricao", 30, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Còdigo Integração", "CodigoIntegracao", 30, Models.Grid.Align.left, true);

            Repositorio.Embarcador.Veiculos.CorVeiculo repCorVeiculo = new Repositorio.Embarcador.Veiculos.CorVeiculo(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaCorVeiculo filtrosPesquisa = ObterFiltrosPesquisa();

            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

            int totalRegistros = repCorVeiculo.ContarConsulta(filtrosPesquisa);

            List<Dominio.Entidades.Embarcador.Veiculos.CorVeiculo> listaCorVeiculo = totalRegistros > 0 ? repCorVeiculo.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Veiculos.CorVeiculo>();

            var lista = (from p in listaCorVeiculo
                            select new
                            {
                                p.Codigo,
                                p.Descricao,
                                Situacao = p.Ativo ? "Ativo" : "Inativo",
                                p.CodigoIntegracao,
                            }).ToList();

            grid.AdicionaRows(lista);
            grid.setarQuantidadeTotal(totalRegistros);

            return grid;
        }

        private void PreencherDados(Dominio.Entidades.Embarcador.Veiculos.CorVeiculo corVeiculo, Repositorio.UnitOfWork unitOfWork)
        {

            corVeiculo.CodigoIntegracao = Request.GetStringParam("Codigointegracao");
            corVeiculo.Descricao = Request.GetStringParam("Descricao");
            corVeiculo.Codigo = Request.GetIntParam("Codigo");
            corVeiculo.Ativo = Request.GetBoolParam("Situacao");

        }

        private Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaCorVeiculo ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaCorVeiculo()
            {
                Descricao = Request.GetStringParam("Descricao"),
                Situacao = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa>("Situacao"),
                CodigoIntegracao = Request.GetStringParam("CodigoIntegracao")
            };
        }

        private void SalvarTiposOperacao(Dominio.Entidades.Embarcador.Email.EmailDocumentacaoCarga emailDocumentacaoCarga, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unidadeDeTrabalho);
            dynamic tiposOperacao = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TiposOperacoes"));

            if (emailDocumentacaoCarga.TiposOperacao == null)
                emailDocumentacaoCarga.TiposOperacao = new List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();
            else
            {
                List<int> codigos = new List<int>();

                foreach (dynamic tipoOperacao in tiposOperacao)
                    codigos.Add((int)tipoOperacao.Tipo?.Codigo);

                List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> tiposDeletar = emailDocumentacaoCarga.TiposOperacao.Where(o => !codigos.Contains(o.Codigo)).ToList();

                foreach (Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacaoDeletar in tiposDeletar)
                    emailDocumentacaoCarga.TiposOperacao.Remove(tipoOperacaoDeletar);
            }

            foreach (var tipoOperacao in tiposOperacao)
            {
                if (emailDocumentacaoCarga.TiposOperacao.Any(o => o.Codigo == (int)tipoOperacao.Tipo?.Codigo))
                    continue;

                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacaoObj = repositorioTipoOperacao.BuscarPorCodigo((int)tipoOperacao.Tipo.Codigo);
                emailDocumentacaoCarga.TiposOperacao.Add(tipoOperacaoObj);
            }
        }

        #endregion
    }
}
