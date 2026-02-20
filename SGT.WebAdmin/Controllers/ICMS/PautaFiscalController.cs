using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.ICMS
{
    [CustomAuthorize("ICMS/PautaFiscal")]
    public class PautaFiscalController : BaseController
    {
		#region Construtores

		public PautaFiscalController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.ICMS.PautaFiscal repPautaFiscal = new Repositorio.Embarcador.ICMS.PautaFiscal(unitOfWork);

                string siglaEstado = Request.GetStringParam("Estado");
                string tarifa = Request.GetStringParam("Tarifa");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = Request.GetEnumParam("Ativo", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Estado", "Estado", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tarifa", "Tarifa", 50, Models.Grid.Align.left, true);
                if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoAtivo", 15, Models.Grid.Align.center, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                List<Dominio.Entidades.Embarcador.ICMS.PautaFiscal> listaGrid = repPautaFiscal.Consultar(siglaEstado, tarifa, ativo, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repPautaFiscal.ContarConsulta(siglaEstado, tarifa, ativo);

                var lista = from obj in listaGrid
                            select new
                            {
                                obj.Codigo,
                                Estado = obj.Estado.Nome,
                                obj.Tarifa,
                                obj.DescricaoAtivo
                            };

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(lista.ToList());
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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.ICMS.PautaFiscal repPautaFiscal = new Repositorio.Embarcador.ICMS.PautaFiscal(unitOfWork);
                Repositorio.Embarcador.ICMS.PautaFiscalTipoCarga repPautaFiscalTipoCarga = new Repositorio.Embarcador.ICMS.PautaFiscalTipoCarga(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.ICMS.PautaFiscal pautaFiscal = repPautaFiscal.BuscarPorCodigo(codigo);

                if (pautaFiscal == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                List<Dominio.Entidades.Embarcador.ICMS.PautaFiscalTipoCarga> tiposCarga = repPautaFiscalTipoCarga.BuscarPorPautaFiscal(codigo);

                var retorno = new
                {
                    pautaFiscal.Codigo,
                    Estado = new { Codigo = pautaFiscal.Estado.Sigla, Descricao = pautaFiscal.Estado.Nome },
                    pautaFiscal.Tarifa,
                    pautaFiscal.Ativo,
                    pautaFiscal.DistanciaKMInicial,
                    pautaFiscal.DistanciaKMFinal,
                    ValorTonelada = pautaFiscal.ValorTonelada.ToString("n4"),
                    ValorViagem = pautaFiscal.ValorViagem.ToString("n4"),
                    ValorM3 = pautaFiscal.ValorM3.ToString("n4"),
                    ValorVolumeMST = pautaFiscal.ValorVolumeMST.ToString("n4"),
                    TiposCarga = (from obj in tiposCarga
                                  select new
                                  {
                                      obj.TipoCarga.Codigo,
                                      obj.TipoCarga.Descricao,
                                  }).ToList()
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.ICMS.PautaFiscal repPautaFiscal = new Repositorio.Embarcador.ICMS.PautaFiscal(unitOfWork);

                Dominio.Entidades.Embarcador.ICMS.PautaFiscal pautaFiscal = new Dominio.Entidades.Embarcador.ICMS.PautaFiscal();

                PreencheEntidade(pautaFiscal, unitOfWork);

                if (!ValidaEntidade(pautaFiscal, out string erro))
                    return new JsonpResult(false, true, erro);

                repPautaFiscal.Inserir(pautaFiscal, Auditado);

                SalvarTipoCargas(pautaFiscal, unitOfWork, null);

                unitOfWork.CommitChanges();

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

                Repositorio.Embarcador.ICMS.PautaFiscal repPautaFiscal = new Repositorio.Embarcador.ICMS.PautaFiscal(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.ICMS.PautaFiscal pautaFiscal = repPautaFiscal.BuscarPorCodigo(codigo, true);

                if (pautaFiscal == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencheEntidade(pautaFiscal, unitOfWork);

                if (!ValidaEntidade(pautaFiscal, out string erro))
                    return new JsonpResult(false, true, erro);

                Dominio.Entidades.Auditoria.HistoricoObjeto historico = repPautaFiscal.Atualizar(pautaFiscal, Auditado);

                SalvarTipoCargas(pautaFiscal, unitOfWork, historico);

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

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.ICMS.PautaFiscal repPautaFiscal = new Repositorio.Embarcador.ICMS.PautaFiscal(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.ICMS.PautaFiscal pautaFiscal = repPautaFiscal.BuscarPorCodigo(codigo);

                if (pautaFiscal == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repPautaFiscal.Deletar(pautaFiscal, Auditado);

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
                    return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencheEntidade(Dominio.Entidades.Embarcador.ICMS.PautaFiscal pautaFiscal, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);

            pautaFiscal.Estado = repEstado.BuscarPorSigla(Request.GetStringParam("Estado"));
            pautaFiscal.Tarifa = Request.GetStringParam("Tarifa");
            pautaFiscal.DistanciaKMInicial = Request.GetIntParam("DistanciaKMInicial");
            pautaFiscal.DistanciaKMFinal = Request.GetIntParam("DistanciaKMFinal");
            pautaFiscal.ValorTonelada = Request.GetDecimalParam("ValorTonelada");
            pautaFiscal.ValorViagem = Request.GetDecimalParam("ValorViagem");
            pautaFiscal.ValorM3 = Request.GetDecimalParam("ValorM3");
            pautaFiscal.ValorVolumeMST = Request.GetDecimalParam("ValorVolumeMST");
            pautaFiscal.Ativo = Request.GetBoolParam("Ativo");
        }

        private void SalvarTipoCargas(Dominio.Entidades.Embarcador.ICMS.PautaFiscal pautaFiscal, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Auditoria.HistoricoObjeto historico)
        {
            Repositorio.Embarcador.ICMS.PautaFiscalTipoCarga repPautaFiscalTipoCarga = new Repositorio.Embarcador.ICMS.PautaFiscalTipoCarga(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
            dynamic dynTiposCarga = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.GetStringParam("TiposCarga"));

            List<int> codigosTipoCarga = new List<int>();

            foreach (var dynTipoCarga in dynTiposCarga)
            {
                int codigoTipoCarga = ((string)dynTipoCarga.Codigo).ToInt();
                codigosTipoCarga.Add(codigoTipoCarga);

                Dominio.Entidades.Embarcador.ICMS.PautaFiscalTipoCarga pautaFiscalTipoCargaSalvar = repPautaFiscalTipoCarga.BuscarPorPautaFiscalETipoCarga(pautaFiscal.Codigo, codigoTipoCarga);
                if (pautaFiscalTipoCargaSalvar != null)
                    continue;

                pautaFiscalTipoCargaSalvar = new Dominio.Entidades.Embarcador.ICMS.PautaFiscalTipoCarga()
                {
                    PautaFiscal = pautaFiscal,
                    TipoCarga = repositorioTipoDeCarga.BuscarPorCodigo(codigoTipoCarga)
                };

                repPautaFiscalTipoCarga.Inserir(pautaFiscalTipoCargaSalvar, Auditado, historico);
            }


            List<Dominio.Entidades.Embarcador.ICMS.PautaFiscalTipoCarga> tiposCargaPautaDeletar = repPautaFiscalTipoCarga.BuscarPorPautaFiscalETipoCargaDiferente(pautaFiscal.Codigo, codigosTipoCarga);
            foreach (var tipoCargaPautaDeletar in tiposCargaPautaDeletar)
                repPautaFiscalTipoCarga.Deletar(tipoCargaPautaDeletar, Auditado, historico);
        }

        private bool ValidaEntidade(Dominio.Entidades.Embarcador.ICMS.PautaFiscal pautaFiscal, out string msgErro)
        {
            msgErro = "";

            if (pautaFiscal.Estado == null)
            {
                msgErro = "Estado é obrigatório.";
                return false;
            }

            int quantidadeValoresPreenchidos = 0;
            bool valorM3Informado = false;

            if (pautaFiscal.ValorTonelada > 0) quantidadeValoresPreenchidos++;
            if (pautaFiscal.ValorViagem > 0) quantidadeValoresPreenchidos++;
            if (pautaFiscal.ValorM3 > 0)
            {
                quantidadeValoresPreenchidos++;
                valorM3Informado = true;
            }
            if (pautaFiscal.ValorVolumeMST > 0 && !valorM3Informado) quantidadeValoresPreenchidos++;

            if (quantidadeValoresPreenchidos > 1)
            {
                msgErro = "Só é possível informar um único valor.";
                return false;
            }

            if (pautaFiscal.Tarifa.Length == 0)
            {
                msgErro = "Tarifa é obrigatória.";
                return false;
            }

            return true;
        }

        private void PropOrdena(ref string propOrdenar)
        {
            if (propOrdenar == "Estado") propOrdenar = "Estado.Sigla";
        }

        #endregion
    }
}
