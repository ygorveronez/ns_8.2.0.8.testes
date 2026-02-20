using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.Fretes
{
    [CustomAuthorize("Fretes/RegrasInclusaoICMS")]
    public class RegrasInclusaoICMSController : BaseController
    {
		#region Construtores

		public RegrasInclusaoICMSController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Frete.RegrasInclusaoICMS repRegrasInclusaoICMS = new Repositorio.Embarcador.Frete.RegrasInclusaoICMS (unitOfWork);
                Dominio.Entidades.Embarcador.Frete.RegrasInclusaoICMS regrasInclusaoICMS = new Dominio.Entidades.Embarcador.Frete.RegrasInclusaoICMS();

                PreencherDados(regrasInclusaoICMS, unitOfWork);

                repRegrasInclusaoICMS.Inserir(regrasInclusaoICMS, Auditado);

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
                Repositorio.Embarcador.Frete.RegrasInclusaoICMS repRegrasInclusaoICMS = new Repositorio.Embarcador.Frete.RegrasInclusaoICMS(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.RegrasInclusaoICMS regrasInclusaoICMS = repRegrasInclusaoICMS.BuscarPorCodigo(codigo, true);

                PreencherDados(regrasInclusaoICMS, unitOfWork);

                if (regrasInclusaoICMS == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repRegrasInclusaoICMS.Atualizar(regrasInclusaoICMS, Auditado);

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

                Repositorio.Embarcador.Frete.RegrasInclusaoICMS repRegrasInclusaoICMS = new Repositorio.Embarcador.Frete.RegrasInclusaoICMS(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.RegrasInclusaoICMS regrasInclusaoICMS = repRegrasInclusaoICMS.BuscarPorCodigo(codigo, true);


                if (regrasInclusaoICMS == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    regrasInclusaoICMS.TipoPessoa,
                    GrupoPessoa = new  { regrasInclusaoICMS.GrupoPessoas?.Descricao, regrasInclusaoICMS.GrupoPessoas?.Codigo },
                    Pessoa = new { regrasInclusaoICMS.Pessoa?.Descricao, regrasInclusaoICMS.Pessoa?.Codigo },
                    TipoOperacao = new  { regrasInclusaoICMS.TipoOperacao?.Descricao , regrasInclusaoICMS.TipoOperacao?.Codigo},
                    regrasInclusaoICMS.Situacao
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
                Repositorio.Embarcador.Frete.RegrasInclusaoICMS repRegrasInclusaoICMS = new Repositorio.Embarcador.Frete.RegrasInclusaoICMS(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.RegrasInclusaoICMS regrasInclusaoICMS = repRegrasInclusaoICMS.BuscarPorCodigo(codigo, auditavel: true);

                if (regrasInclusaoICMS == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repRegrasInclusaoICMS.Deletar(regrasInclusaoICMS, Auditado);

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
                grid.AdicionarCabecalho("Tipo de Pessoa", "TipoPessoa", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Grupo de Pessoas", "GrupoPessoas", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Pessoa", "Pessoa", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo de Operação", "TipoOperacao", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 30, Models.Grid.Align.left, true);



                Repositorio.Embarcador.Frete.RegrasInclusaoICMS repRegrasInclusaoICMS = new Repositorio.Embarcador.Frete.RegrasInclusaoICMS(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRegrasInclusaoICMS filtrosPesquisa = ObterFiltrosPesquisa();

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                int totalRegistros = repRegrasInclusaoICMS.ContarConsulta(filtrosPesquisa);

                List<Dominio.Entidades.Embarcador.Frete.RegrasInclusaoICMS> listaRegrasInclusaoICMS = totalRegistros > 0 ? repRegrasInclusaoICMS.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Frete.RegrasInclusaoICMS>();

                var lista = (from p in listaRegrasInclusaoICMS
                             select new
                             {
                                 p.Codigo,
                                 TipoPessoa = p.TipoPessoa.ObterDescricao(),
                                 GrupoPessoas = p?.GrupoPessoas?.Descricao ?? "",
                                 Pessoa = p.Pessoa?.Nome ?? "",
                                 TipoOperacao = p.TipoOperacao?.Descricao,
                                 Situacao = p.Situacao.ObterDescricaoAtivo()
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

        private Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRegrasInclusaoICMS ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRegrasInclusaoICMS()
            {
                TipoPessoa = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa>("TipoPessoa"),
                CodigoGrupoPessoas = Request.GetIntParam("GrupoPessoa"),
                CodigoPessoa = Request.GetDoubleParam("Pessoa"),
                CodigoTipoOperacao = Request.GetIntParam("TipoOperacao"),
                Situacao = Request.GetNullableBoolParam("Situacao"),

            };
        }

        private void PreencherDados(Dominio.Entidades.Embarcador.Frete.RegrasInclusaoICMS regrasInclusaoICMS, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
            Repositorio.Cliente repPessoa = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);


            regrasInclusaoICMS.Codigo = Request.GetIntParam("Codigo");
            regrasInclusaoICMS.TipoPessoa = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa>("TipoPessoa");
            regrasInclusaoICMS.GrupoPessoas = repGrupoPessoas.BuscarPorCodigo(Request.GetIntParam("GrupoPessoa"));
            regrasInclusaoICMS.Pessoa = repPessoa.BuscarPorCPFCNPJ(Request.GetDoubleParam("Pessoa"));
            regrasInclusaoICMS.TipoOperacao = repTipoOperacao.BuscarPorCodigo(Request.GetIntParam("TipoOperacao"));
            regrasInclusaoICMS.Situacao = Request.GetBoolParam("Situacao");
           
        }

      
        }


        #endregion
    }

