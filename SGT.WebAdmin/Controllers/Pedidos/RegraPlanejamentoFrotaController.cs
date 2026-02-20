using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pedidos
{
    [CustomAuthorize("Pedidos/RegraPlanejamentoFrota")]
    public class RegraPlanejamentoFrotaController : BaseController
    {
		#region Construtores

		public RegraPlanejamentoFrotaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Públicos

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaRegraPlanejamentoFrota filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                Repositorio.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota repositorioRegraPlanejamentoFrota = new Repositorio.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota(unitOfWork);

                int tamanhoAdicionalColuna = 4;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Regra Planejamento Frota", "RegraPlanejamentoFrotaDescricao", (10 + tamanhoAdicionalColuna), Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Número Sequencial Regra", "NumeroSequencial", (8 + tamanhoAdicionalColuna), Models.Grid.Align.left, true);                
                grid.AdicionarCabecalho("Situação", "Ativo", (8 + tamanhoAdicionalColuna), Models.Grid.Align.left, true);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                int totalRegistros = repositorioRegraPlanejamentoFrota.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota> listaRegraPlanejamentoFrota = totalRegistros > 0 ? repositorioRegraPlanejamentoFrota.Consultar(filtrosPesquisa, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite) : new List<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota>();

                dynamic lista = (
                    from obj in listaRegraPlanejamentoFrota
                    select new
                    {
                        obj.Codigo,
                        RegraPlanejamentoFrotaDescricao = obj.Descricao,
                        NumeroSequencial = obj.NumeroSequencial,
                        Ativo = obj.DescricaoAtivo,
                    }
                ).ToList();

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(lista);

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
                Repositorio.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota repositorioRegraPlanejamentoFrota = new Repositorio.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota(unitOfWork);
                Repositorio.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrotaCEPOrigem repositorioRegraPlanejamentoFrotaCEPOrigem = new Repositorio.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrotaCEPOrigem(unitOfWork);
                Repositorio.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrotaCEPDestino repositorioRegraPlanejamentoFrotaCEPDestino = new Repositorio.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrotaCEPDestino(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota = new Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota();

                string descricaoOrigem = string.Empty;
                string descricaoDestino = string.Empty;
                int numeroSequencial = repositorioRegraPlanejamentoFrota.ObterProximoNumeroSequencial();

                unitOfWork.Start();

                PreencherRegraPlanejamentoFrota(regraPlanejamentoFrota, unitOfWork);

                regraPlanejamentoFrota.NumeroSequencial = numeroSequencial;

                SetarLocalidadesOrigem(descricaoOrigem, regraPlanejamentoFrota, unitOfWork);
                SetarClientesOrigem(descricaoOrigem, regraPlanejamentoFrota, unitOfWork);
                SetarEstadosOrigem(descricaoOrigem, regraPlanejamentoFrota, unitOfWork);
                SetarRegioesOrigem(descricaoOrigem, regraPlanejamentoFrota, unitOfWork);
                SetarRotasOrigem(descricaoOrigem, regraPlanejamentoFrota, unitOfWork);
                SetarPaisesOrigem(descricaoOrigem, regraPlanejamentoFrota, unitOfWork);

                SetarLocalidadesDestino(descricaoDestino, regraPlanejamentoFrota, unitOfWork);
                SetarClientesDestino(descricaoDestino, regraPlanejamentoFrota, unitOfWork);
                SetarEstadosDestino(descricaoDestino, regraPlanejamentoFrota, unitOfWork);
                SetarRegioesDestino(descricaoDestino, regraPlanejamentoFrota, unitOfWork);
                SetarRotasDestino(descricaoDestino, regraPlanejamentoFrota, unitOfWork);
                SetarPaisesDestino(descricaoDestino, regraPlanejamentoFrota, unitOfWork);

                SetarGrupoPessoas(regraPlanejamentoFrota, unitOfWork);
                SetarTipoOperacao(regraPlanejamentoFrota, unitOfWork);
                SetarTipoDeCarga(regraPlanejamentoFrota, unitOfWork);
                SetarCentroResultado(regraPlanejamentoFrota, unitOfWork);
                SetarModeloVeicularCargaTracao(regraPlanejamentoFrota, unitOfWork);
                SetarModeloVeicularCargaReboque(regraPlanejamentoFrota, unitOfWork);
                SetarTecnologiaRastreador(regraPlanejamentoFrota, unitOfWork);
                SetarLicenca(regraPlanejamentoFrota, unitOfWork);
                SetarLiberacaoGR(regraPlanejamentoFrota, unitOfWork);
                SetarLiberacaoGRVeiculo(regraPlanejamentoFrota, unitOfWork);

                SetarTipoPropriedade(regraPlanejamentoFrota, unitOfWork);
                SetarTipoCarroceria(regraPlanejamentoFrota, unitOfWork);
                SetarTipoProprietarioVeiculo(regraPlanejamentoFrota, unitOfWork);
                SetarCategoriaHabilitacao(regraPlanejamentoFrota, unitOfWork);
                SetarTipoRodado(regraPlanejamentoFrota, unitOfWork);
                SetarCondicaoLicenca(regraPlanejamentoFrota, unitOfWork);
                SetarCondicaoLiberacaoGR(regraPlanejamentoFrota, unitOfWork);
                SetarModeloVeicularCarga(regraPlanejamentoFrota, unitOfWork);
                SetarNivelCooperado(regraPlanejamentoFrota, unitOfWork);

                repositorioRegraPlanejamentoFrota.Inserir(regraPlanejamentoFrota, Auditado);

                SalvarCEPsOrigem(descricaoOrigem, regraPlanejamentoFrota, unitOfWork);
                SalvarCEPsDestino(descricaoDestino, regraPlanejamentoFrota, unitOfWork);

                if (!string.IsNullOrWhiteSpace(descricaoOrigem) && descricaoOrigem.StartsWith(" / "))
                    regraPlanejamentoFrota.DescricaoOrigem = descricaoOrigem.Remove(0, 3);
                else
                    regraPlanejamentoFrota.DescricaoOrigem = descricaoOrigem;

                if (!string.IsNullOrWhiteSpace(descricaoDestino) && descricaoDestino.StartsWith(" / "))
                    regraPlanejamentoFrota.DescricaoDestino = descricaoDestino.Remove(0, 3);
                else
                    regraPlanejamentoFrota.DescricaoDestino = descricaoDestino;

                repositorioRegraPlanejamentoFrota.Atualizar(regraPlanejamentoFrota);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, regraPlanejamentoFrota, null, "Adicionou Regra para Planejamento de Frota.", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
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
                Repositorio.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota repositorioRegraPlanejamentoFrota = new Repositorio.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota(unitOfWork);
                Repositorio.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrotaCEPOrigem repositorioRegraPlanejamentoFrotaCEPOrigem = new Repositorio.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrotaCEPOrigem(unitOfWork);
                Repositorio.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrotaCEPDestino repositorioRegraPlanejamentoFrotaCEPDestino = new Repositorio.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrotaCEPDestino(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                string descricaoOrigem = string.Empty;
                string descricaoDestino = string.Empty;

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota = repositorioRegraPlanejamentoFrota.BuscarPorCodigo(codigo, true);

                if (regraPlanejamentoFrota == null)
                    throw new ControllerException("Não foi possível encontrar o registro.");

                PreencherRegraPlanejamentoFrota(regraPlanejamentoFrota, unitOfWork);

                SetarLocalidadesOrigem(descricaoOrigem, regraPlanejamentoFrota, unitOfWork);
                SetarClientesOrigem(descricaoOrigem, regraPlanejamentoFrota, unitOfWork);
                SetarEstadosOrigem(descricaoOrigem, regraPlanejamentoFrota, unitOfWork);
                SetarRegioesOrigem(descricaoOrigem, regraPlanejamentoFrota, unitOfWork);
                SetarRotasOrigem(descricaoOrigem, regraPlanejamentoFrota, unitOfWork);
                SetarPaisesOrigem(descricaoOrigem, regraPlanejamentoFrota, unitOfWork);

                SetarLocalidadesDestino(descricaoDestino, regraPlanejamentoFrota, unitOfWork);
                SetarClientesDestino(descricaoDestino, regraPlanejamentoFrota, unitOfWork);
                SetarEstadosDestino(descricaoDestino, regraPlanejamentoFrota, unitOfWork);
                SetarRegioesDestino(descricaoDestino, regraPlanejamentoFrota, unitOfWork);
                SetarRotasDestino(descricaoDestino, regraPlanejamentoFrota, unitOfWork);
                SetarPaisesDestino(descricaoDestino, regraPlanejamentoFrota, unitOfWork);

                SalvarCEPsOrigem(descricaoOrigem, regraPlanejamentoFrota, unitOfWork);
                SalvarCEPsDestino(descricaoDestino, regraPlanejamentoFrota, unitOfWork);

                SetarGrupoPessoas(regraPlanejamentoFrota, unitOfWork);
                SetarTipoOperacao(regraPlanejamentoFrota, unitOfWork);
                SetarTipoDeCarga(regraPlanejamentoFrota, unitOfWork);
                SetarCentroResultado(regraPlanejamentoFrota, unitOfWork);
                SetarModeloVeicularCargaTracao(regraPlanejamentoFrota, unitOfWork);
                SetarModeloVeicularCargaReboque(regraPlanejamentoFrota, unitOfWork);
                SetarTecnologiaRastreador(regraPlanejamentoFrota, unitOfWork);
                SetarLicenca(regraPlanejamentoFrota, unitOfWork);
                SetarLiberacaoGR(regraPlanejamentoFrota, unitOfWork);
                SetarLiberacaoGRVeiculo(regraPlanejamentoFrota, unitOfWork);
                SetarTipoPropriedade(regraPlanejamentoFrota, unitOfWork);
                SetarTipoCarroceria(regraPlanejamentoFrota, unitOfWork);
                SetarTipoProprietarioVeiculo(regraPlanejamentoFrota, unitOfWork);
                SetarCategoriaHabilitacao(regraPlanejamentoFrota, unitOfWork);
                SetarTipoRodado(regraPlanejamentoFrota, unitOfWork);
                SetarCondicaoLicenca(regraPlanejamentoFrota, unitOfWork);
                SetarCondicaoLiberacaoGR(regraPlanejamentoFrota, unitOfWork);
                SetarModeloVeicularCarga(regraPlanejamentoFrota, unitOfWork);
                SetarNivelCooperado(regraPlanejamentoFrota, unitOfWork);

                if (!string.IsNullOrWhiteSpace(descricaoOrigem) && descricaoOrigem.StartsWith(" / "))
                    regraPlanejamentoFrota.DescricaoOrigem = descricaoOrigem.Remove(0, 3);
                else
                    regraPlanejamentoFrota.DescricaoOrigem = descricaoOrigem;

                if (!string.IsNullOrWhiteSpace(descricaoDestino) && descricaoDestino.StartsWith(" / "))
                    regraPlanejamentoFrota.DescricaoDestino = descricaoDestino.Remove(0, 3);
                else
                    regraPlanejamentoFrota.DescricaoDestino = descricaoDestino;

                repositorioRegraPlanejamentoFrota.Atualizar(regraPlanejamentoFrota, Auditado);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, regraPlanejamentoFrota, $"Regra para Planejamento de Frota Atualizada.", unitOfWork);

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

                Repositorio.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota repositorioRegraPlanejamentoFrota = new Repositorio.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota = repositorioRegraPlanejamentoFrota.BuscarPorCodigo(codigo);

                if (regraPlanejamentoFrota == null)
                    throw new ControllerException("Não foi possível encontrar o registro.");

                return new JsonpResult(ObterDetalhesRegraPlanejamentoFrota(regraPlanejamentoFrota));
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
                Repositorio.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota repositorioRegraPlanejamentoFrota = new Repositorio.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota(unitOfWork);
                Repositorio.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrotaCEPOrigem repositorioRegraPlanejamentoFrotaCEPOrigem = new Repositorio.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrotaCEPOrigem(unitOfWork);
                Repositorio.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrotaCEPDestino repositorioRegraPlanejamentoFrotaCEPDestino = new Repositorio.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrotaCEPDestino(unitOfWork);

                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota = repositorioRegraPlanejamentoFrota.BuscarPorCodigo(codigo, true);

                if (regraPlanejamentoFrota == null)
                    throw new ControllerException("Não foi possível encontrar o registro.");

                List<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrotaCEPOrigem> regrasPlanejamentoFrotaCEPOrigem = repositorioRegraPlanejamentoFrotaCEPOrigem.BuscarPorRegraPlanejamentoFrota(codigo);
                List<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrotaCEPDestino> regrasPlanejamentoFrotaCEPDestino = repositorioRegraPlanejamentoFrotaCEPDestino.BuscarPorRegraPlanejamentoFrota(codigo);

                regraPlanejamentoFrota.Origens = null;
                regraPlanejamentoFrota.EstadosOrigem = null;
                regraPlanejamentoFrota.PaisesOrigem = null;
                regraPlanejamentoFrota.RegioesOrigem = null;
                regraPlanejamentoFrota.ClientesOrigem = null;
                regraPlanejamentoFrota.RotasOrigem = null;

                regraPlanejamentoFrota.Destinos = null;
                regraPlanejamentoFrota.EstadosDestino = null;
                regraPlanejamentoFrota.PaisesDestino = null;
                regraPlanejamentoFrota.RegioesDestino = null;
                regraPlanejamentoFrota.ClientesDestino = null;
                regraPlanejamentoFrota.RotasDestino = null;

                regraPlanejamentoFrota.GrupoPessoas = null;
                regraPlanejamentoFrota.TiposOperacao = null;
                regraPlanejamentoFrota.TiposDeCarga = null;
                regraPlanejamentoFrota.CentrosResultado = null;
                regraPlanejamentoFrota.ModelosVeicularCargaTracao = null;
                regraPlanejamentoFrota.ModelosVeicularCargaReboque = null;
                regraPlanejamentoFrota.TecnologiaRastreadores = null;
                regraPlanejamentoFrota.Licencas = null;
                regraPlanejamentoFrota.LiberacoesGR = null;
                regraPlanejamentoFrota.LiberacoesGRVeiculo = null;

                foreach (Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrotaCEPOrigem regraPlanejamentoFrotaCEPOrigem in regrasPlanejamentoFrotaCEPOrigem)
                    repositorioRegraPlanejamentoFrotaCEPOrigem.Deletar(regraPlanejamentoFrotaCEPOrigem, Auditado);

                foreach (Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrotaCEPDestino regraPlanejamentoFrotaCEPDestino in regrasPlanejamentoFrotaCEPDestino)
                    repositorioRegraPlanejamentoFrotaCEPDestino.Deletar(regraPlanejamentoFrotaCEPDestino, Auditado);

                repositorioRegraPlanejamentoFrota.Deletar(regraPlanejamentoFrota, Auditado);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, regraPlanejamentoFrota, null, $"Removeu a Regra de Planejamento de Frota: {regraPlanejamentoFrota.Descricao}", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();

                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherRegraPlanejamentoFrota(Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota, Repositorio.UnitOfWork unitOfWork)
        {
            regraPlanejamentoFrota.Descricao = Request.GetStringParam("Descricao");
            regraPlanejamentoFrota.VigenciaInicial = Request.GetNullableDateTimeParam("VigenciaInicial");
            regraPlanejamentoFrota.VigenciaFinal = Request.GetNullableDateTimeParam("VigenciaFinal");
            regraPlanejamentoFrota.Ativo = Request.GetBoolParam("Status");
            regraPlanejamentoFrota.ValorDeDaMercadoria = Request.GetDecimalParam("ValorDeDaMercadoria");
            regraPlanejamentoFrota.ValorAteDaMercadoria = Request.GetDecimalParam("ValorAteDaMercadoria");
            regraPlanejamentoFrota.ApenasVeiculosComRastreadorAtivo = Request.GetBoolParam("ApenasVeiculosComRastreadorAtivo");
            regraPlanejamentoFrota.ApenasVeiculoQuePossuiTravaQuintaRoda = Request.GetBoolParam("ApenasVeiculoQuePossuiTravaQuintaRoda");
            regraPlanejamentoFrota.ApenasVeiculoQuePossuiImobilizador = Request.GetBoolParam("ApenasVeiculoQuePossuiImobilizador");
            regraPlanejamentoFrota.ApenasTracaoComIdadeMaxima = Request.GetBoolParam("ApenasTracaoComIdadeMaxima");
            regraPlanejamentoFrota.ApenasReboqueComIdadeMaxima = Request.GetBoolParam("ApenasReboqueComIdadeMaxima");
            regraPlanejamentoFrota.LimitarPelaAlturaCarreta = Request.GetBoolParam("LimitarPelaAlturaCarreta");
            regraPlanejamentoFrota.ApenasComInformacoesDeIscaInformadaNoPedido = Request.GetBoolParam("ApenasComInformacoesDeIscaInformadaNoPedido");
            regraPlanejamentoFrota.ApenasComInformacoesDeEscoltaInformadaNoPedido = Request.GetBoolParam("ApenasComInformacoesDeEscoltaInformadaNoPedido");
            regraPlanejamentoFrota.LimitarPelaAlturaCavalo = Request.GetBoolParam("LimitarPelaAlturaCavalo");
            regraPlanejamentoFrota.IdadeMaximaTracao = Request.GetIntParam("IdadeMaximaTracao");
            regraPlanejamentoFrota.IdadeMaximaReboque = Request.GetIntParam("IdadeMaximaReboque");
            regraPlanejamentoFrota.QuantidadeIsca = Request.GetIntParam("QuantidadeIsca");
            regraPlanejamentoFrota.QuantidadeEscolta = Request.GetIntParam("QuantidadeEscolta");
            regraPlanejamentoFrota.MetrosAlturaCarreta = Request.GetDecimalParam("MetrosAlturaCarreta");
            regraPlanejamentoFrota.MetrosAlturaCavalo = Request.GetDecimalParam("MetrosAlturaCavalo");
            regraPlanejamentoFrota.TipoFrota = Request.GetEnumParam("TipoFrota", TipoFrota.NaoDefinido);
            regraPlanejamentoFrota.QuantidadeCarga = Request.GetIntParam("QuantidadeCarga");
            regraPlanejamentoFrota.PeriodoQuantidadeCarga = Request.GetIntParam("PeriodoQuantidadeCarga");
            regraPlanejamentoFrota.TipoPeriodoQuantidadeCarga = Request.GetEnumParam("TipoPeriodoQuantidadeCarga", DiaSemanaMesAno.Dia);
            regraPlanejamentoFrota.ValidarQuantidadeVeiculoEReboque = Request.GetBoolParam("ValidarQuantidadeVeiculoEReboque");
            regraPlanejamentoFrota.ValidarPorQuantidadeMotorista = Request.GetBoolParam("ValidarPorQuantidadeMotorista");
        }

        private Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaRegraPlanejamentoFrota ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaRegraPlanejamentoFrota filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaRegraPlanejamentoFrota()
            {
                NumeroSequencial = Request.GetIntParam("NumeroSequencial"),
                Descricao = Request.GetStringParam("Descricao"),
                Ativo = Request.GetNullableEnumParam<SituacaoAtivoPesquisa>("Status"),
                VigenciaInicial = Request.GetNullableDateTimeParam("VigenciaInicial"),
                VigenciaFinal = Request.GetNullableDateTimeParam("VigenciaFinal"),
                GrupoPessoa = Request.GetIntParam("GrupoPessoas"),
                TiposOperacao = Request.GetListParam<int>("TipoOpercao"),
                TipoCarga = Request.GetListParam<int>("TipoCarga"),
                CentroResultdo = Request.GetListParam<int>("CentroResultdo"),
                ModeloVeicular = Request.GetListParam<int>("ModeloVeicular"),
                NivelCooperado = Request.GetListParam<int>("NivelCooperado"),
                CidadeOrigem = Request.GetIntParam("CidadeOrigem"),
                EstadoOrigem = Request.GetStringParam("EstadoOrigem"),
                CidadeDestino = Request.GetIntParam("CidadeDestino"),
                EstadoDestino = Request.GetStringParam("EstadoDestino"),
            };

            return filtrosPesquisa;
        }

        private dynamic ObterDetalhesRegraPlanejamentoFrota(Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota)
        {

            var retorno = new
            {
                regraPlanejamentoFrota.Descricao,
                regraPlanejamentoFrota.NumeroSequencial,
                VigenciaInicial = regraPlanejamentoFrota.VigenciaInicial?.ToDateString() ?? string.Empty,
                VigenciaFinal = regraPlanejamentoFrota.VigenciaFinal?.ToDateString() ?? string.Empty,
                regraPlanejamentoFrota.Ativo,
                regraPlanejamentoFrota.ValorDeDaMercadoria,
                regraPlanejamentoFrota.ValorAteDaMercadoria,
                regraPlanejamentoFrota.ApenasVeiculosComRastreadorAtivo,
                regraPlanejamentoFrota.ApenasVeiculoQuePossuiTravaQuintaRoda,
                regraPlanejamentoFrota.ApenasVeiculoQuePossuiImobilizador,
                regraPlanejamentoFrota.ApenasTracaoComIdadeMaxima,
                regraPlanejamentoFrota.ApenasReboqueComIdadeMaxima,
                regraPlanejamentoFrota.LimitarPelaAlturaCarreta,
                regraPlanejamentoFrota.ApenasComInformacoesDeIscaInformadaNoPedido,
                regraPlanejamentoFrota.ApenasComInformacoesDeEscoltaInformadaNoPedido,
                regraPlanejamentoFrota.LimitarPelaAlturaCavalo,
                regraPlanejamentoFrota.IdadeMaximaTracao,
                regraPlanejamentoFrota.IdadeMaximaReboque,
                regraPlanejamentoFrota.QuantidadeIsca,
                regraPlanejamentoFrota.QuantidadeEscolta,
                regraPlanejamentoFrota.MetrosAlturaCarreta,
                regraPlanejamentoFrota.MetrosAlturaCavalo,
                regraPlanejamentoFrota.TipoFrota,
                regraPlanejamentoFrota.QuantidadeCarga,
                regraPlanejamentoFrota.PeriodoQuantidadeCarga,
                regraPlanejamentoFrota.TipoPeriodoQuantidadeCarga,
                regraPlanejamentoFrota.ValidarQuantidadeVeiculoEReboque,
                regraPlanejamentoFrota.ValidarPorQuantidadeMotorista,
                Origens = ObterOrigens(regraPlanejamentoFrota),
                Destinos = ObterDestinos(regraPlanejamentoFrota),
                EstadosOrigem = ObterEstadosOrigem(regraPlanejamentoFrota),
                EstadosDestino = ObterEstadosDestino(regraPlanejamentoFrota),
                RegioesOrigem = ObterRegioesOrigem(regraPlanejamentoFrota),
                RotasOrigem = ObterRotasOrigem(regraPlanejamentoFrota),
                PaisesOrigem = ObterPaisesOrigem(regraPlanejamentoFrota),
                RegioesDestino = ObterRegioesDestino(regraPlanejamentoFrota),
                RotasDestino = ObterRotasDestino(regraPlanejamentoFrota),
                PaisesDestino = ObterPaisesDestino(regraPlanejamentoFrota),
                ClientesOrigem = ObterClientesOrigem(regraPlanejamentoFrota),
                ClientesDestino = ObterClientesDestino(regraPlanejamentoFrota),
                CEPsOrigem = ObterCEPSOrigem(regraPlanejamentoFrota),
                CEPsDestino = ObterCEPSDestino(regraPlanejamentoFrota),
                GruposPessoas = ObterGrupoPessoas(regraPlanejamentoFrota),
                TiposOperacao = ObterTipoOperacao(regraPlanejamentoFrota),
                TiposCargas = ObterTipoDeCarga(regraPlanejamentoFrota),
                CentrosResultado = ObterCentroResultado(regraPlanejamentoFrota),
                ModelosVeicularesTracaoCarga = ObterModeloVeicularCargaTracao(regraPlanejamentoFrota),
                ModelosVeicularesReboqueCarga = ObterModeloVeicularCargaReboque(regraPlanejamentoFrota),
                TecnologiaRastreadores = ObterTecnologiaRastreador(regraPlanejamentoFrota),
                Licencas = ObterLicenca(regraPlanejamentoFrota),
                LiberacoesGR = ObterLiberacaoGR(regraPlanejamentoFrota),
                LiberacoesGRVeiculo = ObterLiberacaoGRVeiculo(regraPlanejamentoFrota),
                TipoDePropriedade = ObterTipoPropriedade(regraPlanejamentoFrota),
                TipoDeCarroceria = ObterTipoCarroceria(regraPlanejamentoFrota),
                TipoDeProprietario = ObterTipoProprietarioVeiculo(regraPlanejamentoFrota),
                CategoriaDaCNH = ObterCategoriaHabilitacao(regraPlanejamentoFrota),
                TipoDaTracao = ObterTipoRodado(regraPlanejamentoFrota),
                CondicaoParaLicencas = ObterCondicaoLicenca(regraPlanejamentoFrota),
                ModelosVeicularesCarga = ObterModeloVeicularCarga(regraPlanejamentoFrota),
                NiveisCooperados = ObterNivelCooperado(regraPlanejamentoFrota),
                CondicaoParaLiberacoesGR = ObterCondicaoLiberacaoGR(regraPlanejamentoFrota)
            };

            return retorno;
        }

        private dynamic ObterGrupoPessoas(Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota)
        {
            return (from obj in regraPlanejamentoFrota.GrupoPessoas
                    select new
                    {
                        Codigo = obj.Codigo,
                        Descricao = obj.Descricao
                    }).ToList();
        }

        private dynamic ObterTipoOperacao(Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota)
        {
            return (from obj in regraPlanejamentoFrota.TiposOperacao
                    select new
                    {
                        Codigo = obj.Codigo,
                        Descricao = obj.Descricao
                    }).ToList();
        }

        private dynamic ObterTipoDeCarga(Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota)
        {
            return (from obj in regraPlanejamentoFrota.TiposDeCarga
                    select new
                    {
                        Codigo = obj.Codigo,
                        Descricao = obj.Descricao
                    }).ToList();
        }

        private dynamic ObterCentroResultado(Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota)
        {
            return (from obj in regraPlanejamentoFrota.CentrosResultado
                    select new
                    {
                        Codigo = obj.Codigo,
                        Descricao = obj.Descricao
                    }).ToList();
        }

        private dynamic ObterModeloVeicularCargaTracao(Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota)
        {
            return (from obj in regraPlanejamentoFrota.ModelosVeicularCargaTracao
                    select new
                    {
                        Codigo = obj.Codigo,
                        Descricao = obj.Descricao
                    }).ToList();
        }

        private dynamic ObterModeloVeicularCargaReboque(Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota)
        {
            return (from obj in regraPlanejamentoFrota.ModelosVeicularCargaReboque
                    select new
                    {
                        Codigo = obj.Codigo,
                        Descricao = obj.Descricao
                    }).ToList();
        }

        private dynamic ObterTecnologiaRastreador(Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota)
        {
            return (from obj in regraPlanejamentoFrota.TecnologiaRastreadores
                    select new
                    {
                        Codigo = obj.Codigo,
                        Descricao = obj.Descricao
                    }).ToList();
        }

        private dynamic ObterLicenca(Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota)
        {
            return (from obj in regraPlanejamentoFrota.Licencas
                    select new
                    {
                        Codigo = obj.Codigo,
                        Descricao = obj.Descricao
                    }).ToList();
        }
        private dynamic ObterLiberacaoGR(Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota)
        {
            return (from obj in regraPlanejamentoFrota.LiberacoesGR
                    select new
                    {
                        Codigo = obj.Codigo,
                        Descricao = obj.Descricao
                    }).ToList();
        }

        private dynamic ObterLiberacaoGRVeiculo(Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota)
        {
            return (from obj in regraPlanejamentoFrota.LiberacoesGRVeiculo
                    select new
                    {
                        Codigo = obj.Codigo,
                        Descricao = obj.Descricao
                    }).ToList();
        }

        private dynamic ObterTipoPropriedade(Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota)
        {
            return regraPlanejamentoFrota.TiposPropriedade.ToList();
        }

        private dynamic ObterTipoCarroceria(Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota)
        {
            return regraPlanejamentoFrota.TiposCarroceria.ToList();
        }

        private dynamic ObterTipoProprietarioVeiculo(Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota)
        {
            return regraPlanejamentoFrota.TiposProprietarioVeiculo.ToList();
        }

        private dynamic ObterCategoriaHabilitacao(Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota)
        {
            return regraPlanejamentoFrota.CategoriasHabilitacao.ToList();
        }

        private dynamic ObterTipoRodado(Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota)
        {
            return regraPlanejamentoFrota.TiposRodado.ToList();
        }

        private dynamic ObterCondicaoLicenca(Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota)
        {
            return regraPlanejamentoFrota.CondicaoLicencas.ToList();
        }
        private dynamic ObterCondicaoLiberacaoGR(Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota)
        {
            return regraPlanejamentoFrota.CondicaoLiberacoesGR.ToList();
        }
        private dynamic ObterModeloVeicularCarga(Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota)
        {
            return (from obj in regraPlanejamentoFrota.ModelosVeicularesCarga
                    select new
                    {
                        Codigo = obj.Codigo,
                        Descricao = obj.Descricao
                    }).ToList();
        }

        private dynamic ObterNivelCooperado(Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota)
        {
            return (from obj in regraPlanejamentoFrota.NiveisCooperados
                    select new
                    {
                        Codigo = obj.Codigo,
                        Descricao = obj.Descricao
                    }).ToList();
        }
        #endregion


        #region Métodos Privados Listas Tela

        private void SetarGrupoPessoas(Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pessoas.GrupoPessoas repositorioGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);

            dynamic listaDinamica = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("GruposPessoas"));

            if (regraPlanejamentoFrota.GrupoPessoas == null)
                regraPlanejamentoFrota.GrupoPessoas = new List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas>();
            else
                regraPlanejamentoFrota.GrupoPessoas.Clear();

            foreach (var dadoDinamico in listaDinamica)
            {
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas entidade = repositorioGrupoPessoas.BuscarPorCodigo((int)dadoDinamico.Codigo);

                regraPlanejamentoFrota.GrupoPessoas.Add(entidade);
            }
        }

        private void SetarTipoOperacao(Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

            dynamic listaDinamica = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TiposOperacao"));

            if (regraPlanejamentoFrota.TiposOperacao == null)
                regraPlanejamentoFrota.TiposOperacao = new List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();
            else
                regraPlanejamentoFrota.TiposOperacao.Clear();

            foreach (var dadoDinamico in listaDinamica)
            {
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao entidade = repositorioTipoOperacao.BuscarPorCodigo((int)dadoDinamico.Codigo);

                regraPlanejamentoFrota.TiposOperacao.Add(entidade);
            }
        }

        private void SetarTipoDeCarga(Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);

            dynamic listaDinamica = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TiposCargas"));

            if (regraPlanejamentoFrota.TiposDeCarga == null)
                regraPlanejamentoFrota.TiposDeCarga = new List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>();
            else
                regraPlanejamentoFrota.TiposDeCarga.Clear();

            foreach (var dadoDinamico in listaDinamica)
            {
                Dominio.Entidades.Embarcador.Cargas.TipoDeCarga entidade = repositorioTipoDeCarga.BuscarPorCodigo((int)dadoDinamico.Codigo);

                regraPlanejamentoFrota.TiposDeCarga.Add(entidade);
            }
        }

        private void SetarCentroResultado(Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.CentroResultado repositorioCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);

            dynamic listaDinamica = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("CentrosResultado"));

            if (regraPlanejamentoFrota.CentrosResultado == null)
                regraPlanejamentoFrota.CentrosResultado = new List<Dominio.Entidades.Embarcador.Financeiro.CentroResultado>();
            else
                regraPlanejamentoFrota.CentrosResultado.Clear();

            foreach (var dadoDinamico in listaDinamica)
            {
                Dominio.Entidades.Embarcador.Financeiro.CentroResultado entidade = repositorioCentroResultado.BuscarPorCodigo((int)dadoDinamico.Codigo);

                regraPlanejamentoFrota.CentrosResultado.Add(entidade);
            }
        }

        private void SetarModeloVeicularCargaTracao(Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);

            dynamic listaDinamica = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ModelosVeicularesTracaoCarga"));

            if (regraPlanejamentoFrota.ModelosVeicularCargaTracao == null)
                regraPlanejamentoFrota.ModelosVeicularCargaTracao = new List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>();
            else
                regraPlanejamentoFrota.ModelosVeicularCargaTracao.Clear();

            foreach (var dadoDinamico in listaDinamica)
            {
                Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga entidade = repositorioModeloVeicularCarga.BuscarPorCodigo((int)dadoDinamico.Codigo);

                regraPlanejamentoFrota.ModelosVeicularCargaTracao.Add(entidade);
            }
        }

        private void SetarModeloVeicularCargaReboque(Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);

            dynamic listaDinamica = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ModelosVeicularesReboqueCarga"));

            if (regraPlanejamentoFrota.ModelosVeicularCargaReboque == null)
                regraPlanejamentoFrota.ModelosVeicularCargaReboque = new List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>();
            else
                regraPlanejamentoFrota.ModelosVeicularCargaReboque.Clear();

            foreach (var dadoDinamico in listaDinamica)
            {
                Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga entidade = repositorioModeloVeicularCarga.BuscarPorCodigo((int)dadoDinamico.Codigo);

                regraPlanejamentoFrota.ModelosVeicularCargaReboque.Add(entidade);
            }
        }

        private void SetarTecnologiaRastreador(Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Veiculos.TecnologiaRastreador repositorioTecnologiaRastreador = new Repositorio.Embarcador.Veiculos.TecnologiaRastreador(unitOfWork);

            dynamic listaDinamica = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TecnologiaRastreadores"));

            if (regraPlanejamentoFrota.TecnologiaRastreadores == null)
                regraPlanejamentoFrota.TecnologiaRastreadores = new List<Dominio.Entidades.Embarcador.Veiculos.TecnologiaRastreador>();
            else
                regraPlanejamentoFrota.TecnologiaRastreadores.Clear();

            foreach (var dadoDinamico in listaDinamica)
            {
                Dominio.Entidades.Embarcador.Veiculos.TecnologiaRastreador entidade = repositorioTecnologiaRastreador.BuscarPorCodigo((int)dadoDinamico.Codigo, false);

                regraPlanejamentoFrota.TecnologiaRastreadores.Add(entidade);
            }
        }

        private void SetarLicenca(Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.Licenca repositorioLicenca = new Repositorio.Embarcador.Configuracoes.Licenca(unitOfWork);

            dynamic listaDinamica = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Licencas"));

            if (regraPlanejamentoFrota.Licencas == null)
                regraPlanejamentoFrota.Licencas = new List<Dominio.Entidades.Embarcador.Configuracoes.Licenca>();
            else
                regraPlanejamentoFrota.Licencas.Clear();

            foreach (var dadoDinamico in listaDinamica)
            {
                Dominio.Entidades.Embarcador.Configuracoes.Licenca entidade = repositorioLicenca.BuscarPorCodigo((int)dadoDinamico.Codigo);

                regraPlanejamentoFrota.Licencas.Add(entidade);
            }
        }
        private void SetarLiberacaoGR(Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.Licenca repositorioLicenca = new Repositorio.Embarcador.Configuracoes.Licenca(unitOfWork);

            dynamic listaDinamica = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("LiberacoesGR"));

            if (regraPlanejamentoFrota.LiberacoesGR == null)
                regraPlanejamentoFrota.LiberacoesGR = new List<Dominio.Entidades.Embarcador.Configuracoes.Licenca>();
            else
                regraPlanejamentoFrota.LiberacoesGR.Clear();

            foreach (var dadoDinamico in listaDinamica)
            {
                Dominio.Entidades.Embarcador.Configuracoes.Licenca entidade = repositorioLicenca.BuscarPorCodigo((int)dadoDinamico.Codigo);

                regraPlanejamentoFrota.LiberacoesGR.Add(entidade);
            }
        }

        private void SetarLiberacaoGRVeiculo(Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.Licenca repositorioLicenca = new Repositorio.Embarcador.Configuracoes.Licenca(unitOfWork);

            dynamic listaDinamica = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("LiberacoesGRVeiculo"));

            if (regraPlanejamentoFrota.LiberacoesGRVeiculo == null)
                regraPlanejamentoFrota.LiberacoesGRVeiculo = new List<Dominio.Entidades.Embarcador.Configuracoes.Licenca>();
            else
                regraPlanejamentoFrota.LiberacoesGRVeiculo.Clear();

            foreach (var dadoDinamico in listaDinamica)
            {
                Dominio.Entidades.Embarcador.Configuracoes.Licenca entidade = repositorioLicenca.BuscarPorCodigo((int)dadoDinamico.Codigo);
                regraPlanejamentoFrota.LiberacoesGRVeiculo.Add(entidade);
            }
        }

        private void SetarTipoPropriedade(Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropriedade> listaDinamica = Request.GetListEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropriedade>("TipoDePropriedade");

            if (regraPlanejamentoFrota.TiposPropriedade == null)
                regraPlanejamentoFrota.TiposPropriedade = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropriedade>();
            else
                regraPlanejamentoFrota.TiposPropriedade.Clear();

            foreach (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropriedade dadoDinamico in listaDinamica)
                regraPlanejamentoFrota.TiposPropriedade.Add(dadoDinamico);
        }

        private void SetarTipoCarroceria(Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCarroceria> listaDinamica = Request.GetListEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCarroceria>("TipoDeCarroceria");

            if (regraPlanejamentoFrota.TiposCarroceria == null)
                regraPlanejamentoFrota.TiposCarroceria = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCarroceria>();
            else
                regraPlanejamentoFrota.TiposCarroceria.Clear();

            foreach (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCarroceria dadoDinamico in listaDinamica)
                regraPlanejamentoFrota.TiposCarroceria.Add(dadoDinamico);

        }

        private void SetarTipoProprietarioVeiculo(Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProprietarioVeiculo> listaDinamica = Request.GetListEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProprietarioVeiculo>("TipoDeProprietario");

            if (regraPlanejamentoFrota.TiposProprietarioVeiculo == null)
                regraPlanejamentoFrota.TiposProprietarioVeiculo = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProprietarioVeiculo>();
            else
                regraPlanejamentoFrota.TiposProprietarioVeiculo.Clear();

            foreach (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProprietarioVeiculo dadoDinamico in listaDinamica)
                regraPlanejamentoFrota.TiposProprietarioVeiculo.Add(dadoDinamico);

        }

        private void SetarCategoriaHabilitacao(Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.CategoriaHabilitacao> listaDinamica = Request.GetListEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.CategoriaHabilitacao>("CategoriaDaCNH");

            if (regraPlanejamentoFrota.CategoriasHabilitacao == null)
                regraPlanejamentoFrota.CategoriasHabilitacao = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.CategoriaHabilitacao>();
            else
                regraPlanejamentoFrota.CategoriasHabilitacao.Clear();

            foreach (Dominio.ObjetosDeValor.Embarcador.Enumeradores.CategoriaHabilitacao dadoDinamico in listaDinamica)
                regraPlanejamentoFrota.CategoriasHabilitacao.Add(dadoDinamico);
        }

        private void SetarTipoRodado(Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRodado> listaDinamica = Request.GetListEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRodado>("TipoDaTracao");

            if (regraPlanejamentoFrota.TiposRodado == null)
                regraPlanejamentoFrota.TiposRodado = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRodado>();
            else
                regraPlanejamentoFrota.TiposRodado.Clear();

            foreach (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRodado dadoDinamico in listaDinamica)
                regraPlanejamentoFrota.TiposRodado.Add(dadoDinamico);
        }

        private void SetarCondicaoLicenca(Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoLicenca> listaDinamica = Request.GetListEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoLicenca>("CondicaoParaLicencas");

            if (regraPlanejamentoFrota.CondicaoLicencas == null)
                regraPlanejamentoFrota.CondicaoLicencas = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoLicenca>();
            else
                regraPlanejamentoFrota.CondicaoLicencas.Clear();

            foreach (Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoLicenca dadoDinamico in listaDinamica)
                regraPlanejamentoFrota.CondicaoLicencas.Add(dadoDinamico);
        }

        private void SetarCondicaoLiberacaoGR(Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoLiberacaoGR> listaDinamica = Request.GetListEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoLiberacaoGR>("CondicaoParaLiberacoesGR");

            if (regraPlanejamentoFrota.CondicaoLiberacoesGR == null)
                regraPlanejamentoFrota.CondicaoLiberacoesGR = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoLiberacaoGR>();
            else
                regraPlanejamentoFrota.CondicaoLiberacoesGR.Clear();

            foreach (Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoLiberacaoGR dadoDinamico in listaDinamica)
                regraPlanejamentoFrota.CondicaoLiberacoesGR.Add(dadoDinamico);
        }
        private void SetarModeloVeicularCarga(Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);

            dynamic listaDinamica = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ModelosVeicularesCarga"));

            if (regraPlanejamentoFrota.ModelosVeicularesCarga == null)
                regraPlanejamentoFrota.ModelosVeicularesCarga = new List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>();
            else
                regraPlanejamentoFrota.ModelosVeicularesCarga.Clear();

            foreach (var dadoDinamico in listaDinamica)
            {
                Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga entidade = repositorioModeloVeicularCarga.BuscarPorCodigo((int)dadoDinamico.Codigo);

                regraPlanejamentoFrota.ModelosVeicularesCarga.Add(entidade);
            }
        }

        private void SetarNivelCooperado(Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pessoas.TipoTerceiro repositorioTipoTerceiro = new Repositorio.Embarcador.Pessoas.TipoTerceiro(unitOfWork);

            dynamic listaDinamica = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("NiveisCooperados"));

            if (regraPlanejamentoFrota.NiveisCooperados == null)
                regraPlanejamentoFrota.NiveisCooperados = new List<Dominio.Entidades.Embarcador.Pessoas.TipoTerceiro>();
            else
                regraPlanejamentoFrota.NiveisCooperados.Clear();

            foreach (var dadoDinamico in listaDinamica)
            {
                Dominio.Entidades.Embarcador.Pessoas.TipoTerceiro entidade = repositorioTipoTerceiro.BuscarPorCodigo((int)dadoDinamico.Codigo);

                regraPlanejamentoFrota.NiveisCooperados.Add(entidade);
            }
        }
        #endregion


        #region Métodos Privados Origem

        private dynamic ObterCEPSOrigem(Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota)
        {
            return (from obj in regraPlanejamentoFrota.CEPsOrigem
                    select new
                    {
                        Codigo = obj.Codigo.ToString(),
                        CEPInicial = string.Format(@"{0:00\.000\-000}", obj.CEPInicial),
                        CEPFinal = string.Format(@"{0:00\.000\-000}", obj.CEPFinal)
                    }).ToList();
        }

        private dynamic ObterOrigens(Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota)
        {
            return (from obj in regraPlanejamentoFrota.Origens
                    select new
                    {
                        obj.Codigo,
                        Descricao = obj.DescricaoCidadeEstado
                    }).ToList();
        }

        private dynamic ObterClientesOrigem(Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota)
        {
            return (from obj in regraPlanejamentoFrota.ClientesOrigem
                    select new
                    {
                        obj.Codigo,
                        obj.Descricao
                    }).ToList();
        }

        private dynamic ObterPaisesOrigem(Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota)
        {
            return (from obj in regraPlanejamentoFrota.PaisesOrigem
                    select new
                    {
                        obj.Codigo,
                        obj.Descricao
                    }).ToList();
        }

        private dynamic ObterEstadosOrigem(Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota)
        {
            return (from obj in regraPlanejamentoFrota.EstadosOrigem
                    select new
                    {
                        Codigo = obj.Sigla,
                        Descricao = obj.Nome
                    }).ToList();
        }

        private dynamic ObterRegioesOrigem(Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota)
        {
            return (from obj in regraPlanejamentoFrota.RegioesOrigem
                    select new
                    {
                        Codigo = obj.Codigo,
                        obj.Descricao
                    }).ToList();
        }

        private dynamic ObterRotasOrigem(Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota)
        {
            return (from obj in regraPlanejamentoFrota.RotasOrigem
                    select new
                    {
                        Codigo = obj.Codigo,
                        obj.Descricao
                    }).ToList();
        }

        private void SetarLocalidadesOrigem(string descricaoOrigem, Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

            dynamic origens = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Origens"));

            regraPlanejamentoFrota.Origens = new List<Dominio.Entidades.Localidade>();

            foreach (var origem in origens)
            {
                Dominio.Entidades.Localidade localidade = repLocalidade.BuscarPorCodigo((int)origem.Codigo);

                regraPlanejamentoFrota.Origens.Add(localidade);

                descricaoOrigem += " / " + localidade.DescricaoCidadeEstado;
            }
        }

        private void SetarEstadosOrigem(string descricaoOrigem, Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);

            dynamic estadosOrigem = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("EstadosOrigem"));

            regraPlanejamentoFrota.EstadosOrigem = new List<Dominio.Entidades.Estado>();

            foreach (var estadoOrigem in estadosOrigem)
            {
                Dominio.Entidades.Estado estado = repEstado.BuscarPorSigla((string)estadoOrigem.Codigo);

                regraPlanejamentoFrota.EstadosOrigem.Add(estado);

                descricaoOrigem += " / " + estado.Nome;
            }
        }

        private void SetarPaisesOrigem(string descricaoOrigem, Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Pais repositorioPais = new Repositorio.Pais(unitOfWork);
            dynamic paisesOrigem = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("PaisesOrigem"));
            regraPlanejamentoFrota.PaisesOrigem = new List<Dominio.Entidades.Pais>();

            foreach (var paisOrigem in paisesOrigem)
            {
                Dominio.Entidades.Pais pais = repositorioPais.BuscarPorCodigo((int)paisOrigem.Codigo);

                regraPlanejamentoFrota.PaisesOrigem.Add(pais);

                descricaoOrigem += $" / {pais.Nome}";
            }
        }

        private void SetarRegioesOrigem(string descricaoOrigem, Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Localidades.Regiao repRegiao = new Repositorio.Embarcador.Localidades.Regiao(unitOfWork);

            dynamic regioesOrigem = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("RegioesOrigem"));

            regraPlanejamentoFrota.RegioesOrigem = new List<Dominio.Entidades.Embarcador.Localidades.Regiao>();

            foreach (var regiaoOrigem in regioesOrigem)
            {
                Dominio.Entidades.Embarcador.Localidades.Regiao regiao = repRegiao.BuscarPorCodigo((int)regiaoOrigem.Codigo);

                regraPlanejamentoFrota.RegioesOrigem.Add(regiao);

                descricaoOrigem += " / " + regiao.Descricao;
            }
        }

        private void SetarRotasOrigem(string descricaoOrigem, Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.RotaFrete repRota = new Repositorio.RotaFrete(unitOfWork);

            dynamic rotasOrigem = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("RotasOrigem"));

            regraPlanejamentoFrota.RotasOrigem = new List<Dominio.Entidades.RotaFrete>();

            foreach (var rotaOrigem in rotasOrigem)
            {
                Dominio.Entidades.RotaFrete rota = repRota.BuscarPorCodigo((int)rotaOrigem.Codigo);

                regraPlanejamentoFrota.RotasOrigem.Add(rota);

                descricaoOrigem += " / " + rota.Descricao;
            }
        }

        private void SetarClientesOrigem(string descricaoOrigem, Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            dynamic clientesOrigem = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ClientesOrigem"));

            regraPlanejamentoFrota.ClientesOrigem = new List<Dominio.Entidades.Cliente>();

            foreach (var clienteOrigem in clientesOrigem)
            {
                double.TryParse(Utilidades.String.OnlyNumbers((string)clienteOrigem.Codigo), out double cpfCnpjCliente);

                Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpjCliente);

                regraPlanejamentoFrota.ClientesOrigem.Add(cliente);

                descricaoOrigem += " / " + cliente.Descricao;
            }
        }

        private void SalvarCEPsOrigem(string descricaoOrigem, Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrotaCEPOrigem repositorioRegraPlanejamentoFrotaCEPOrigem = new Repositorio.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrotaCEPOrigem(unitOfWork);

            dynamic cepsOrigem = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaCEPsOrigem"));

            if (regraPlanejamentoFrota.CEPsOrigem != null && regraPlanejamentoFrota.CEPsOrigem.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var cepOrigem in cepsOrigem)
                {
                    int codigo = 0;

                    if (int.TryParse((string)cepOrigem.Codigo, out codigo))
                        codigos.Add(codigo);
                }

                List<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrotaCEPOrigem> cepsDeletar = (from obj in regraPlanejamentoFrota.CEPsOrigem where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < cepsDeletar.Count; i++)
                    repositorioRegraPlanejamentoFrotaCEPOrigem.Deletar(cepsDeletar[i]);
            }

            foreach (var cepOrigem in cepsOrigem)
            {
                int codigo = 0;

                Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrotaCEPOrigem cep = null;

                if (cepOrigem.Codigo != null && int.TryParse((string)cepOrigem.Codigo, out codigo))
                    cep = repositorioRegraPlanejamentoFrotaCEPOrigem.BuscarPorCodigo(codigo, false);

                if (cep == null)
                    cep = new Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrotaCEPOrigem();

                string cepInicial = (string)cepOrigem.CEPInicial;
                string cepFinal = (string)cepOrigem.CEPFinal;

                cep.RegraPlanejamentoFrota = regraPlanejamentoFrota;
                cep.CEPInicial = int.Parse(Utilidades.String.OnlyNumbers(cepInicial));
                cep.CEPFinal = int.Parse(Utilidades.String.OnlyNumbers(cepFinal));

                if (cep.Codigo > 0)
                    repositorioRegraPlanejamentoFrotaCEPOrigem.Atualizar(cep);
                else
                    repositorioRegraPlanejamentoFrotaCEPOrigem.Inserir(cep);

                descricaoOrigem += " / " + cepInicial + " à " + cepFinal;
            }
        }

        #endregion Origens



        #region Destinos

        private dynamic ObterCEPSDestino(Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota)
        {
            return (from obj in regraPlanejamentoFrota.CEPsDestino
                    select new
                    {
                        Codigo = obj.Codigo.ToString(),
                        CEPInicial = string.Format(@"{0:00\.000\-000}", obj.CEPInicial),
                        CEPFinal = string.Format(@"{0:00\.000\-000}", obj.CEPFinal),
                        obj.DiasUteis
                    }).ToList();
        }

        private dynamic ObterDestinos(Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota)
        {
            return (from obj in regraPlanejamentoFrota.Destinos
                    select new
                    {
                        obj.Codigo,
                        Descricao = obj.DescricaoCidadeEstado
                    }).ToList();
        }

        private dynamic ObterClientesDestino(Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota)
        {
            return (from obj in regraPlanejamentoFrota.ClientesDestino
                    select new
                    {
                        obj.Codigo,
                        obj.Descricao
                    }).ToList();
        }

        private dynamic ObterPaisesDestino(Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota)
        {
            return (from obj in regraPlanejamentoFrota.PaisesDestino
                    select new
                    {
                        obj.Codigo,
                        obj.Descricao
                    }).ToList();
        }

        private dynamic ObterEstadosDestino(Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota)
        {
            return (from obj in regraPlanejamentoFrota.EstadosDestino
                    select new
                    {
                        Codigo = obj.Sigla,
                        Descricao = obj.Nome
                    }).ToList();
        }

        private dynamic ObterRegioesDestino(Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota)
        {
            return (from obj in regraPlanejamentoFrota.RegioesDestino
                    select new
                    {
                        Codigo = obj.Codigo,
                        obj.Descricao
                    }).ToList();
        }

        private dynamic ObterRotasDestino(Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota)
        {
            return (from obj in regraPlanejamentoFrota.RotasDestino
                    select new
                    {
                        Codigo = obj.Codigo,
                        obj.Descricao
                    }).ToList();
        }

        private void SetarLocalidadesDestino(string descricaoDestino, Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

            dynamic destinos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Destinos"));

            regraPlanejamentoFrota.Destinos = new List<Dominio.Entidades.Localidade>();

            foreach (var destino in destinos)
            {
                Dominio.Entidades.Localidade localidade = repLocalidade.BuscarPorCodigo((int)destino.Codigo);

                regraPlanejamentoFrota.Destinos.Add(localidade);

                descricaoDestino += " / " + localidade.DescricaoCidadeEstado;
            }
        }

        private void SetarEstadosDestino(string descricaoDestino, Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);

            dynamic estadosDestino = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("EstadosDestino"));

            regraPlanejamentoFrota.EstadosDestino = new List<Dominio.Entidades.Estado>();

            foreach (var estadoDestino in estadosDestino)
            {
                Dominio.Entidades.Estado estado = repEstado.BuscarPorSigla((string)estadoDestino.Codigo);

                regraPlanejamentoFrota.EstadosDestino.Add(estado);

                descricaoDestino += " / " + estado.Nome;
            }
        }

        private void SetarPaisesDestino(string descricaoDestino, Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Pais repositorioPais = new Repositorio.Pais(unitOfWork);
            dynamic paisesDestino = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("PaisesDestino"));
            regraPlanejamentoFrota.PaisesDestino = new List<Dominio.Entidades.Pais>();

            foreach (var paisDestino in paisesDestino)
            {
                Dominio.Entidades.Pais pais = repositorioPais.BuscarPorCodigo((int)paisDestino.Codigo);

                regraPlanejamentoFrota.PaisesDestino.Add(pais);

                descricaoDestino += $" / {pais.Nome}";
            }
        }

        private void SetarRegioesDestino(string descricaoDestino, Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Localidades.Regiao repRegiao = new Repositorio.Embarcador.Localidades.Regiao(unitOfWork);

            dynamic regioesDestino = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("RegioesDestino"));

            regraPlanejamentoFrota.RegioesDestino = new List<Dominio.Entidades.Embarcador.Localidades.Regiao>();

            foreach (var regiaoDestino in regioesDestino)
            {
                Dominio.Entidades.Embarcador.Localidades.Regiao regiao = repRegiao.BuscarPorCodigo((int)regiaoDestino.Codigo);

                regraPlanejamentoFrota.RegioesDestino.Add(regiao);

                descricaoDestino += " / " + regiao.Descricao;
            }
        }

        private void SetarClientesDestino(string descricaoDestino, Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            dynamic clientesDestino = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ClientesDestino"));

            regraPlanejamentoFrota.ClientesDestino = new List<Dominio.Entidades.Cliente>();

            foreach (var clienteDestino in clientesDestino)
            {
                double.TryParse(Utilidades.String.OnlyNumbers((string)clienteDestino.Codigo), out double cpfCnpjCliente);

                Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpjCliente);

                regraPlanejamentoFrota.ClientesDestino.Add(cliente);

                descricaoDestino += " / " + cliente.Descricao;
            }
        }

        private void SalvarCEPsDestino(string descricaoDestino, Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrotaCEPDestino repostorioRegraPlanejamentoFrotaCEPDestino = new Repositorio.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrotaCEPDestino(unitOfWork);

            dynamic cepsDestino = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaCEPsDestino"));

            if (regraPlanejamentoFrota.CEPsDestino != null && regraPlanejamentoFrota.CEPsDestino.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var cepDestino in cepsDestino)
                {
                    int codigo = 0;

                    if (int.TryParse((string)cepDestino.Codigo, out codigo))
                        codigos.Add((int)cepDestino.Codigo);
                }

                List<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrotaCEPDestino> cepsDeletar = (from obj in regraPlanejamentoFrota.CEPsDestino where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < cepsDeletar.Count; i++)
                    repostorioRegraPlanejamentoFrotaCEPDestino.Deletar(cepsDeletar[i]);
            }

            foreach (var cepDestino in cepsDestino)
            {
                Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrotaCEPDestino cep = null;

                int codigo = 0;

                if (cepDestino.Codigo != null && int.TryParse((string)cepDestino.Codigo, out codigo))
                    cep = repostorioRegraPlanejamentoFrotaCEPDestino.BuscarPorCodigo((int)cepDestino.Codigo, false);

                if (cep == null)
                    cep = new Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrotaCEPDestino();

                string cepInicial = (string)cepDestino.CEPInicial;
                string cepFinal = (string)cepDestino.CEPFinal;

                cep.RegraPlanejamentoFrota = regraPlanejamentoFrota;
                cep.CEPInicial = int.Parse(Utilidades.String.OnlyNumbers(cepInicial));
                cep.CEPFinal = int.Parse(Utilidades.String.OnlyNumbers(cepFinal));
                cep.DiasUteis = ((string)cepDestino.DiasUteis).ToInt();

                if (cep.Codigo > 0)
                    repostorioRegraPlanejamentoFrotaCEPDestino.Atualizar(cep);
                else
                    repostorioRegraPlanejamentoFrotaCEPDestino.Inserir(cep);

                descricaoDestino += " / " + cepInicial + " à " + cepFinal;
            }
        }

        private void SetarRotasDestino(string descricaoDestino, Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota regraPlanejamentoFrota, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.RotaFrete repRota = new Repositorio.RotaFrete(unitOfWork);

            dynamic rotasDestino = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("RotasDestino"));

            regraPlanejamentoFrota.RotasDestino = new List<Dominio.Entidades.RotaFrete>();

            foreach (var rotaDestino in rotasDestino)
            {
                Dominio.Entidades.RotaFrete rota = repRota.BuscarPorCodigo((int)rotaDestino.Codigo);

                regraPlanejamentoFrota.RotasDestino.Add(rota);

                descricaoDestino += " / " + rota.Descricao;
            }
        }

        #endregion
    }
}
