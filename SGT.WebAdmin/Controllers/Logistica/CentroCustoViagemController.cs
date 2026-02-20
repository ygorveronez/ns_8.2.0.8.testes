using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/CentroCustoViagem")]
    public class CentroCustoViagemController : BaseController
    {
		#region Construtores

		public CentroCustoViagemController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.Entidades.Embarcador.Logistica.CentroCustoViagem centroCustoViagem = new Dominio.Entidades.Embarcador.Logistica.CentroCustoViagem();

                PreencherCentroCustoViagem(centroCustoViagem);

                unitOfWork.Start();

                Repositorio.Embarcador.Logistica.CentroCustoViagem repositorio = new Repositorio.Embarcador.Logistica.CentroCustoViagem(unitOfWork);

                repositorio.Inserir(centroCustoViagem, Auditado);

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
                Repositorio.Embarcador.Logistica.CentroCustoViagem repositorio = new Repositorio.Embarcador.Logistica.CentroCustoViagem(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.CentroCustoViagem centroCustoViagem = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (centroCustoViagem == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherCentroCustoViagem(centroCustoViagem);

                unitOfWork.Start();

                repositorio.Atualizar(centroCustoViagem, Auditado);

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
                Repositorio.Embarcador.Logistica.CentroCustoViagem repositorio = new Repositorio.Embarcador.Logistica.CentroCustoViagem(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.CentroCustoViagem centroCustoViagem = repositorio.BuscarPorCodigo(codigo, auditavel: false);

                if (centroCustoViagem == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    centroCustoViagem.Codigo,
                    centroCustoViagem.Descricao,
                    Status = centroCustoViagem.Ativo,
                    centroCustoViagem.CodigoIntegracao,
                    OpenTech = new
                    {
                        CodigoTransportadorOpenTech = centroCustoViagem?.CodigoTransportadorOpenTech
                    },
                    RepomFrete = new
                    {
                        CodigoFilialRepom = centroCustoViagem?.CodigoFilialRepom
                    }
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

        public async Task<IActionResult> BuscarPorFuncionario()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.CentroCustoViagem repositorio = new Repositorio.Embarcador.Logistica.CentroCustoViagem(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.CentroCustoViagem centroCustoViagem = repositorio.BuscarPorCodigo(codigo, auditavel: false);

                if (centroCustoViagem == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    centroCustoViagem.Codigo,
                    centroCustoViagem.Descricao,
                    Status = centroCustoViagem.Ativo,
                    centroCustoViagem.CodigoIntegracao
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
                Repositorio.Embarcador.Logistica.CentroCustoViagem repositorio = new Repositorio.Embarcador.Logistica.CentroCustoViagem(unitOfWork);


                Dominio.Entidades.Embarcador.Logistica.CentroCustoViagem CentroCustoViagem = repositorio.BuscarPorCodigo(codigo, auditavel: true);


                if (CentroCustoViagem == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repositorio.Deletar(CentroCustoViagem, Auditado);

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
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

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

        #endregion

        #region Métodos Privados

        private void PreencherCentroCustoViagem(Dominio.Entidades.Embarcador.Logistica.CentroCustoViagem CentroCustoViagem)
        {

            string descricao = Request.Params("Descricao") ?? string.Empty;
            string codigoIntegracao = Request.Params("CodigoIntegracao") ?? string.Empty;
            bool.TryParse(Request.Params("Status"), out bool ativo);

            CentroCustoViagem.Descricao = descricao;
            CentroCustoViagem.CodigoIntegracao = codigoIntegracao;
            CentroCustoViagem.Ativo = ativo;

            dynamic openTech = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("OpenTech"));
            CentroCustoViagem.CodigoTransportadorOpenTech = (int?)openTech.CodigoTransportadorOpenTech;

            dynamic repomFrete = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("RepomFrete"));
            CentroCustoViagem.CodigoFilialRepom = repomFrete.CodigoFilialRepom;
        }
        private Models.Grid.Grid GridPesquisa()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.Prop("Codigo");
            grid.Prop("Descricao").Nome("Descrição").Tamanho(35).Align(Models.Grid.Align.left);
            grid.Prop("CodigoIntegracao").Nome("Código Integração").Tamanho(35).Align(Models.Grid.Align.left);
            grid.Prop("Situacao").Nome("Situação").Tamanho(25).Align(Models.Grid.Align.left);

            return grid;
        }
        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Logistica.CentroCustoViagem repCentroCustoViagem = new Repositorio.Embarcador.Logistica.CentroCustoViagem(unitOfWork);

            // Dados do filtro
            Enum.TryParse(Request.Params("Status"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status);

            string descricao = Request.Params("Descricao");
            string codigoIntegracao = Request.Params("CodigoIntegracao") ?? string.Empty;


            // Consulta
            List<Dominio.Entidades.Embarcador.Logistica.CentroCustoViagem> listaGrid = repCentroCustoViagem.Consultar(codigoIntegracao, descricao, status, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repCentroCustoViagem.ContarConsulta(codigoIntegracao, descricao, status);

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.Codigo,
                            obj.Descricao,
                            obj.CodigoIntegracao,
                            Situacao = obj.DescricaoAtivo
                        };

            return lista.ToList();
        }
        #endregion
    }
}
