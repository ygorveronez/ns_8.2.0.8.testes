using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.ICMS
{
    [CustomAuthorize("ICMS/RegraExtensao")]
    public class RegraExtensaoController : BaseController
    {
		#region Construtores

		public RegraExtensaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Extensão", "Extensao", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo Propriedade", "TipoPropriedade", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Modelo Veicular", "ModeloVeicular", 20, Models.Grid.Align.center, true);
                Dominio.ObjetosDeValor.Embarcador.ICMS.RegraExtensao filtroPesquisa = ObterFiltrosPesquisa();

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.ICMS.RegraExtensao repRegraExtensao = new Repositorio.Embarcador.ICMS.RegraExtensao(unitOfWork);
                int totalRegistro = repRegraExtensao.ContarConsulta(filtroPesquisa);
                List<Dominio.Entidades.Embarcador.ICMS.RegraExtensao> regrasExtensao = (totalRegistro > 0) ? repRegraExtensao.Consultar(filtroPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.ICMS.RegraExtensao>();

                var retorno = (
                    from regraExtensao in regrasExtensao
                    select new
                    {
                        regraExtensao.Codigo,
                        regraExtensao.Extensao,
                        ModeloVeicular = regraExtensao.ModeloVeicularCarga?.Descricao ?? string.Empty,
                        TipoPropriedade = regraExtensao.TipoPropriedade.ObterDescricao(),
                    }
                ).ToList();

                grid.AdicionaRows(retorno);
                grid.setarQuantidadeTotal(totalRegistro);

                return new JsonpResult(grid);
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.ICMS.RegraExtensao repositorioRegraExtensao = new Repositorio.Embarcador.ICMS.RegraExtensao(unitOfWork);
                Dominio.Entidades.Embarcador.ICMS.RegraExtensao regraExtensao = new Dominio.Entidades.Embarcador.ICMS.RegraExtensao();

                PreencherEntidade(regraExtensao, unitOfWork);

                repositorioRegraExtensao.Inserir(regraExtensao, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException exececao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, exececao.Message);
            }
            catch (ServicoException exececao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, exececao.Message);
            }
            catch (Exception exececao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(exececao);
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

                Repositorio.Embarcador.ICMS.RegraExtensao repRegraExtensao = new Repositorio.Embarcador.ICMS.RegraExtensao(unitOfWork);
                Dominio.Entidades.Embarcador.ICMS.RegraExtensao regraExtensao = repRegraExtensao.BuscarPorCodigo(codigo, false);

                if (regraExtensao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherEntidade(regraExtensao, unitOfWork);

                repRegraExtensao.Atualizar(regraExtensao, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException exececao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, exececao.Message);
            }
            catch (ServicoException exececao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, exececao.Message);
            }
            catch (Exception exececao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(exececao);
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

                Repositorio.Embarcador.ICMS.RegraExtensao repRegraExtensao = new Repositorio.Embarcador.ICMS.RegraExtensao(unitOfWork);
                Dominio.Entidades.Embarcador.ICMS.RegraExtensao regraExtensao = repRegraExtensao.BuscarPorCodigo(codigo, false);

                if (regraExtensao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var retorno = new
                {
                    regraExtensao.TipoPropriedade,
                    regraExtensao.Extensao,
                    regraExtensao.Codigo,
                    ModeloVeicular = new { Codigo = regraExtensao.ModeloVeicularCarga?.Codigo ?? 0, Descricao = regraExtensao.ModeloVeicularCarga?.Descricao ?? string.Empty }
                };

                return new JsonpResult(retorno);
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

                Repositorio.Embarcador.ICMS.RegraExtensao repRegraExtensao = new Repositorio.Embarcador.ICMS.RegraExtensao(unitOfWork);
                Dominio.Entidades.Embarcador.ICMS.RegraExtensao regraExtensao = repRegraExtensao.BuscarPorCodigo(codigo, false);

                if (regraExtensao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repRegraExtensao.Deletar(regraExtensao, Auditado); 

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
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

        #endregion

        #region Métodos Privados

        private void PreencherEntidade(Dominio.Entidades.Embarcador.ICMS.RegraExtensao regraExtensao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeiculaCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);

            regraExtensao.TipoPropriedade = Request.GetEnumParam<TipoPropriedadeVeiculo>("TipoPropriedade");
            regraExtensao.ModeloVeicularCarga = repModeloVeiculaCarga.BuscarPorCodigo(Request.GetIntParam("ModeloVeicular"));
            regraExtensao.Extensao = Request.GetStringParam("Extensao");

        }

        private Dominio.ObjetosDeValor.Embarcador.ICMS.RegraExtensao ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.ICMS.RegraExtensao()
            {
                Extensao = Request.GetStringParam("Extensao"),
                TipoPropriedade = Request.GetNullableEnumParam<TipoPropriedadeVeiculo>("TipoPropriedade"),
                CodigoModeloVeicular = Request.GetIntParam("ModeloVeicular")

            };
        }

        private string ObterPropriedadeOrdenar(string prop)
        {
            return prop;
        }


        #endregion

    }
}
