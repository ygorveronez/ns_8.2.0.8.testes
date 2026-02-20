using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas
{
    [CustomAuthorize("Logistica/PrazoSituacaoCarga")]
    public class PrazoSituacaoCargaController : BaseController
    {
		#region Construtores

		public PrazoSituacaoCargaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
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
                grid.AdicionarCabecalho("Situação", "DescricaoSituacaoCarga", 55, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tempo", "DescricaoTempo", 25, Models.Grid.Align.left, true);

                SituacaoCargaJanelaCarregamento? situacaoCarga = Request.GetNullableEnumParam<SituacaoCargaJanelaCarregamento>("SituacaoCarga");
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Logistica.PrazoSituacaoCarga repositorioPrazoSituacaoCarga = new Repositorio.Embarcador.Logistica.PrazoSituacaoCarga(unitOfWork);
                int totalRegistros = repositorioPrazoSituacaoCarga.ContarConsulta(situacaoCarga);
                List<Dominio.Entidades.Embarcador.Logistica.PrazoSituacaoCarga> listaPrazoSituacaoCarga = (totalRegistros > 0) ? repositorioPrazoSituacaoCarga.Consultar(situacaoCarga, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Logistica.PrazoSituacaoCarga>();

                var listaPrazoSituacaoCargaRetornar = (
                    from o in listaPrazoSituacaoCarga
                    select new
                    {
                        Codigo = o.Codigo,
                        o.DescricaoSituacaoCarga,
                        o.DescricaoTempo
                    }
                ).ToList();

                grid.AdicionaRows(listaPrazoSituacaoCargaRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Logistica.PrazoSituacaoCarga repositorioPrazoSituacaoCarga = new Repositorio.Embarcador.Logistica.PrazoSituacaoCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.PrazoSituacaoCarga prazoSituacaoCarga = new Dominio.Entidades.Embarcador.Logistica.PrazoSituacaoCarga();

                PreencherPrazoSituacaoCarga(prazoSituacaoCarga, unitOfWork);
                repositorioPrazoSituacaoCarga.Inserir(prazoSituacaoCarga, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
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
                Repositorio.Embarcador.Logistica.PrazoSituacaoCarga repositorioPrazoSituacaoCarga = new Repositorio.Embarcador.Logistica.PrazoSituacaoCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.PrazoSituacaoCarga prazoSituacaoCarga = repositorioPrazoSituacaoCarga.BuscarPorCodigo(codigo, true);

                if (prazoSituacaoCarga == null)
                    throw new ControllerException("Não foi possível encontrar o registro.");

                PreencherPrazoSituacaoCarga(prazoSituacaoCarga, unitOfWork);
                repositorioPrazoSituacaoCarga.Atualizar(prazoSituacaoCarga, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
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
                Repositorio.Embarcador.Logistica.PrazoSituacaoCarga repositorioPrazoSituacaoCarga = new Repositorio.Embarcador.Logistica.PrazoSituacaoCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.PrazoSituacaoCarga prazoSituacaoCarga = repositorioPrazoSituacaoCarga.BuscarPorCodigo(codigo);

                var prazoSituacaoCargaRetornar = new
                {
                    prazoSituacaoCarga.Codigo,
                    Tempo = prazoSituacaoCarga.DescricaoTempo,
                    prazoSituacaoCarga.SituacaoCarga,
                    prazoSituacaoCarga.NotificarTransportadorPorEmailAoEsgotarPrazo
                };

                return new JsonpResult(prazoSituacaoCargaRetornar);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
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
                Repositorio.Embarcador.Logistica.PrazoSituacaoCarga repositorioPrazoSituacaoCarga = new Repositorio.Embarcador.Logistica.PrazoSituacaoCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.PrazoSituacaoCarga prazoSituacaoCarga = repositorioPrazoSituacaoCarga.BuscarPorCodigo(codigo);

                if (prazoSituacaoCarga == null)
                    throw new ControllerException("Não foi possível encontrar o registro.");

                repositorioPrazoSituacaoCarga.Deletar(prazoSituacaoCarga, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                if (ExcessaoPorPossuirDependeciasNoBanco(excecao))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
                else
                {
                    Servicos.Log.TratarErro(excecao);
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

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DescricaoSituacaoCarga")
                return "SituacaoCarga";

            if (propriedadeOrdenar == "DescricaoTempo")
                return "Tempo";

            return propriedadeOrdenar;
        }

        private void PreencherPrazoSituacaoCarga(Dominio.Entidades.Embarcador.Logistica.PrazoSituacaoCarga prazoSituacaoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            prazoSituacaoCarga.NotificarTransportadorPorEmailAoEsgotarPrazo = Request.GetBoolParam("NotificarTransportadorPorEmailAoEsgotarPrazo");
            prazoSituacaoCarga.SituacaoCarga = Request.GetEnumParam<SituacaoCargaJanelaCarregamento>("SituacaoCarga");
            prazoSituacaoCarga.Tempo = (int)Request.GetTimeParam("Tempo").TotalMinutes;

            ValidarPrazoSituacaoCarga(prazoSituacaoCarga, unitOfWork);
        }

        public void ValidarPrazoSituacaoCarga(Dominio.Entidades.Embarcador.Logistica.PrazoSituacaoCarga prazoSituacaoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.PrazoSituacaoCarga repositorioPrazoSituacaoCarga = new Repositorio.Embarcador.Logistica.PrazoSituacaoCarga(unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.PrazoSituacaoCarga> prazos = repositorioPrazoSituacaoCarga.BuscarTodos();

            if ((prazoSituacaoCarga.Codigo == 0) && prazos.Any(o => o.SituacaoCarga == prazoSituacaoCarga.SituacaoCarga))
                throw new ControllerException("A situação da carga informada já esta cadastrada");

            SituacaoCargaJanelaCarregamento[] situacoesMaior = null;
            SituacaoCargaJanelaCarregamento[] situacoesMenor = null;

            switch (prazoSituacaoCarga.SituacaoCarga)
            {
                case SituacaoCargaJanelaCarregamento.AgAprovacaoComercial:
                    situacoesMaior = new SituacaoCargaJanelaCarregamento[] { SituacaoCargaJanelaCarregamento.AgConfirmacaoTransportador, SituacaoCargaJanelaCarregamento.AgLiberacaoParaTransportadores, SituacaoCargaJanelaCarregamento.ProntaParaCarregamento, SituacaoCargaJanelaCarregamento.SemTransportador, SituacaoCargaJanelaCarregamento.SemValorFrete, SituacaoCargaJanelaCarregamento.ReprovacaoComercial };
                    situacoesMenor = new SituacaoCargaJanelaCarregamento[] { };
                    break;
                case SituacaoCargaJanelaCarregamento.AgLiberacaoParaTransportadores:
                    situacoesMaior = new SituacaoCargaJanelaCarregamento[] { SituacaoCargaJanelaCarregamento.AgConfirmacaoTransportador, SituacaoCargaJanelaCarregamento.AgEncosta, SituacaoCargaJanelaCarregamento.ProntaParaCarregamento, SituacaoCargaJanelaCarregamento.SemTransportador, SituacaoCargaJanelaCarregamento.SemValorFrete, SituacaoCargaJanelaCarregamento.ReprovacaoComercial };
                    situacoesMenor = new SituacaoCargaJanelaCarregamento[] { SituacaoCargaJanelaCarregamento.AgAprovacaoComercial };
                    break;
                case SituacaoCargaJanelaCarregamento.SemValorFrete:
                    situacoesMaior = new SituacaoCargaJanelaCarregamento[] { SituacaoCargaJanelaCarregamento.AgConfirmacaoTransportador, SituacaoCargaJanelaCarregamento.AgEncosta, SituacaoCargaJanelaCarregamento.ProntaParaCarregamento, SituacaoCargaJanelaCarregamento.SemTransportador, SituacaoCargaJanelaCarregamento.ReprovacaoComercial };
                    situacoesMenor = new SituacaoCargaJanelaCarregamento[] { SituacaoCargaJanelaCarregamento.AgAprovacaoComercial, SituacaoCargaJanelaCarregamento.AgLiberacaoParaTransportadores };
                    break;
                case SituacaoCargaJanelaCarregamento.SemTransportador:
                    situacoesMaior = new SituacaoCargaJanelaCarregamento[] { SituacaoCargaJanelaCarregamento.AgConfirmacaoTransportador, SituacaoCargaJanelaCarregamento.AgEncosta, SituacaoCargaJanelaCarregamento.ProntaParaCarregamento, SituacaoCargaJanelaCarregamento.ReprovacaoComercial };
                    situacoesMenor = new SituacaoCargaJanelaCarregamento[] { SituacaoCargaJanelaCarregamento.AgAprovacaoComercial, SituacaoCargaJanelaCarregamento.AgLiberacaoParaTransportadores, SituacaoCargaJanelaCarregamento.SemValorFrete };
                    break;
                case SituacaoCargaJanelaCarregamento.AgConfirmacaoTransportador:
                    situacoesMaior = new SituacaoCargaJanelaCarregamento[] { SituacaoCargaJanelaCarregamento.AgEncosta, SituacaoCargaJanelaCarregamento.ProntaParaCarregamento, SituacaoCargaJanelaCarregamento.ReprovacaoComercial };
                    situacoesMenor = new SituacaoCargaJanelaCarregamento[] { SituacaoCargaJanelaCarregamento.AgAprovacaoComercial, SituacaoCargaJanelaCarregamento.AgLiberacaoParaTransportadores, SituacaoCargaJanelaCarregamento.SemValorFrete, SituacaoCargaJanelaCarregamento.SemTransportador };
                    break;

                case SituacaoCargaJanelaCarregamento.AgEncosta:
                    situacoesMaior = new SituacaoCargaJanelaCarregamento[] { SituacaoCargaJanelaCarregamento.ProntaParaCarregamento,  SituacaoCargaJanelaCarregamento.ReprovacaoComercial };
                    situacoesMenor = new SituacaoCargaJanelaCarregamento[] { SituacaoCargaJanelaCarregamento.AgAprovacaoComercial, SituacaoCargaJanelaCarregamento.AgLiberacaoParaTransportadores, SituacaoCargaJanelaCarregamento.SemValorFrete, SituacaoCargaJanelaCarregamento.SemTransportador, SituacaoCargaJanelaCarregamento.AgConfirmacaoTransportador };
                    break;

                case SituacaoCargaJanelaCarregamento.ProntaParaCarregamento:
                    situacoesMaior = new SituacaoCargaJanelaCarregamento[] { SituacaoCargaJanelaCarregamento.ReprovacaoComercial };
                    situacoesMenor = new SituacaoCargaJanelaCarregamento[] { SituacaoCargaJanelaCarregamento.AgAprovacaoComercial, SituacaoCargaJanelaCarregamento.AgLiberacaoParaTransportadores, SituacaoCargaJanelaCarregamento.SemValorFrete, SituacaoCargaJanelaCarregamento.SemTransportador, SituacaoCargaJanelaCarregamento.AgConfirmacaoTransportador, SituacaoCargaJanelaCarregamento.AgEncosta };
                    break;

                case SituacaoCargaJanelaCarregamento.ReprovacaoComercial:
                    situacoesMaior = new SituacaoCargaJanelaCarregamento[] { };
                    situacoesMenor = new SituacaoCargaJanelaCarregamento[] { SituacaoCargaJanelaCarregamento.AgAprovacaoComercial, SituacaoCargaJanelaCarregamento.AgLiberacaoParaTransportadores, SituacaoCargaJanelaCarregamento.SemValorFrete, SituacaoCargaJanelaCarregamento.SemTransportador, SituacaoCargaJanelaCarregamento.AgConfirmacaoTransportador, SituacaoCargaJanelaCarregamento.AgEncosta, SituacaoCargaJanelaCarregamento.ProntaParaCarregamento };
                    break;
                case SituacaoCargaJanelaCarregamento.LiberarAutomaticamenteFaturamento:
                    situacoesMaior = new SituacaoCargaJanelaCarregamento[] { }; 
                    situacoesMenor = new SituacaoCargaJanelaCarregamento[] { SituacaoCargaJanelaCarregamento.ProntaParaCarregamento };
                    break;
            }

            if ((from o in prazos where (situacoesMaior.Contains(o.SituacaoCarga) && o.Tempo >= prazoSituacaoCarga.Tempo) || (situacoesMenor.Contains(o.SituacaoCarga) && o.Tempo <= prazoSituacaoCarga.Tempo) select o).Any())
                throw new ControllerException("O tempo está em conflito com os demais registros.");
        }

        #endregion
    }
}
