using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.Integracao.MercadoLivre
{
    [CustomAuthorize("Cargas/Carga", "Logistica/JanelaCarregamento")]
    public class CargaIntegracaoMercadoLivreController : BaseController
    {
		#region Construtores

		public CargaIntegracaoMercadoLivreController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCargaPedido = Request.GetIntParam("CargaPedido");
                TipoIntegracaoMercadoLivre tipoIntegracaoMercadoLivre = Request.GetEnumParam<TipoIntegracaoMercadoLivre>("TipoIntegracaoMercadoLivre");
                bool rotaEFacility = tipoIntegracaoMercadoLivre == TipoIntegracaoMercadoLivre.RotaEFacility ? true : false;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Handling Unit", "HandlingUnit", 20, Models.Grid.Align.left, false, !rotaEFacility);
                grid.AdicionarCabecalho("Rota", "Rota", 20, Models.Grid.Align.left, false, rotaEFacility);
                grid.AdicionarCabecalho("Facility", "Facility", 20, Models.Grid.Align.left, false, rotaEFacility);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação", "Situacao", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Mensagem", "Mensagem", 25, Models.Grid.Align.left, false);

                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaIntegracaoMercadoLivre repCargaIntegracaoEmbarcador = new Repositorio.Embarcador.Cargas.CargaIntegracaoMercadoLivre(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigo(codigoCargaPedido);

                int countIntegracoes = repCargaIntegracaoEmbarcador.ContarConsulta(cargaPedido.Carga.Codigo);

                List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre> listaIntegracoes = countIntegracoes > 0 ? repCargaIntegracaoEmbarcador.Consultar(cargaPedido.Carga.Codigo, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite) : new List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre>();

                grid.setarQuantidadeTotal(countIntegracoes);

                grid.AdicionaRows(listaIntegracoes.Select(o => new
                {
                    o.Codigo,
                    HandlingUnit = o.HandlingUnit.ID,
                    Rota = o.HandlingUnit.Rota,
                    Facility = o.HandlingUnit.Facility,
                    Data = o.HandlingUnit.DataInclusao?.ToString("dd/MM/yyyy HH:mm:ss"),
                    Situacao = o.HandlingUnit.Situacao.ObterDescricao(),
                    Mensagem = o.HandlingUnit.Mensagem
                }).ToList());

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar os HU's vinculados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaDetalhesHandLingUnit()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisaDetalhesHandLingUnit());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        public async Task<IActionResult> VincularHandlingUnit()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_InformarDocumentosFiscais))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int codigoCargaPedido = Request.GetIntParam("CargaPedido");
                TipoIntegracaoMercadoLivre tipoIntegracaoMercadoLivre = Request.GetEnumParam<TipoIntegracaoMercadoLivre>("TipoIntegracaoMercadoLivre");
                int rota = Request.GetIntParam("Rota");
                string handlingUnitID = Request.GetStringParam("HandlingUnitID");
                string facility = Request.GetStringParam("Facility");

                if (tipoIntegracaoMercadoLivre == TipoIntegracaoMercadoLivre.RotaEFacility)
                {
                    if (rota == 0)
                        return new JsonpResult(false, true, "Informe a Rota para adicionar.");

                    if (string.IsNullOrWhiteSpace(facility))
                        return new JsonpResult(false, true, "Informe o Facility para adicionar.");
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(handlingUnitID))
                        return new JsonpResult(false, true, "Informe um Handling Unit ID para adicionar.");
                }

                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigo(codigoCargaPedido);

                if (cargaPedido == null)
                    return new JsonpResult(false, true, "Pedido não encontrado.");

                if (cargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe)
                    return new JsonpResult(false, true, $"A situação da carga ({cargaPedido.Carga.SituacaoCarga.ObterDescricao()}) não permite a inclusão de documentos.");

                if (cargaPedido.Carga.ProcessandoDocumentosFiscais)
                    return new JsonpResult(false, true, "A carga está processando os documentos fiscais, não sendo possível incluir documentos.");

                Repositorio.Embarcador.Cargas.CargaIntegracaoMercadoLivre repCargaIntegracaoMercadoLivre = new Repositorio.Embarcador.Cargas.CargaIntegracaoMercadoLivre(unitOfWork);
                Servicos.Embarcador.Integracao.MercadoLivre.IntegracaoMercadoLivre svcMercadoLivre = new Servicos.Embarcador.Integracao.MercadoLivre.IntegracaoMercadoLivre(unitOfWork);

                if (tipoIntegracaoMercadoLivre == TipoIntegracaoMercadoLivre.RotaEFacility)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre cargaIntegracaoMercadoLivre = repCargaIntegracaoMercadoLivre.BuscarPorRotaEFacilityECarga(rota, facility, cargaPedido.Carga.Codigo);

                    if (cargaIntegracaoMercadoLivre != null)
                        return new JsonpResult(false, true, "Rota e Facility já foi adicionada na carga.");

                    svcMercadoLivre.AdicionarHandlingUnitParaConsulta(null, rota, facility, cargaPedido.Carga, unitOfWork);
                }
                else
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre cargaIntegracaoMercadoLivre = repCargaIntegracaoMercadoLivre.BuscarPorHandlingUnitIDECarga(handlingUnitID, cargaPedido.Carga.Codigo);

                    if (cargaIntegracaoMercadoLivre != null)
                        return new JsonpResult(false, true, "Handling Unit ID já foi adicionada na carga.");

                    svcMercadoLivre.AdicionarHandlingUnitParaConsulta(new List<string>() { handlingUnitID }, 0, null, cargaPedido.Carga, unitOfWork);
                }

                Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();
                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                serHubCarga.InformarCargaAtualizada(cargaPedido.Carga.Codigo, TipoAcaoCarga.Alterada, unitOfWork.StringConexao);

                return new JsonpResult(servicoCarga.ObterDetalhesDaCarga(cargaPedido.Carga, TipoServicoMultisoftware, unitOfWork));
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao vincular o Handling Unit informado.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RemoverHandlingUnit()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_InformarDocumentosFiscais))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.CargaIntegracaoMercadoLivre repCargaIntegracaoMercadoLivre = new Repositorio.Embarcador.Cargas.CargaIntegracaoMercadoLivre(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre cargaIntegracaoMercadoLivre = repCargaIntegracaoMercadoLivre.BuscarPorCodigo(codigo, false);

                Servicos.Embarcador.Integracao.MercadoLivre.IntegracaoMercadoLivre svcMercadoLivre = new Servicos.Embarcador.Integracao.MercadoLivre.IntegracaoMercadoLivre(unitOfWork);

                if (!svcMercadoLivre.RemoverHandlingUnitDaCargaPedido(out string mensagemErro, cargaIntegracaoMercadoLivre, TipoServicoMultisoftware, unitOfWork, Auditado))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, mensagemErro);
                }

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao remover o Handling Unit informado.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfirmarProcessamentoHandlingUnit()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_InformarDocumentosFiscais))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.CargaIntegracaoMercadoLivre repCargaIntegracaoMercadoLivre = new Repositorio.Embarcador.Cargas.CargaIntegracaoMercadoLivre(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre cargaIntegracaoMercadoLivre = repCargaIntegracaoMercadoLivre.BuscarPorCodigo(codigo, false);

                Servicos.Embarcador.Integracao.MercadoLivre.IntegracaoMercadoLivre svcMercadoLivre = new Servicos.Embarcador.Integracao.MercadoLivre.IntegracaoMercadoLivre(unitOfWork);

                if (!svcMercadoLivre.ConfirmarProcessamentoHandlingUnitDaCargaPedido(out string mensagemErro, cargaIntegracaoMercadoLivre, TipoServicoMultisoftware, unitOfWork, Auditado))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, mensagemErro);
                }

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao remover o Handling Unit informado.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReprocessarHandlingUnit()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_InformarDocumentosFiscais))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.CargaIntegracaoMercadoLivre repCargaIntegracaoMercadoLivre = new Repositorio.Embarcador.Cargas.CargaIntegracaoMercadoLivre(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre cargaIntegracaoMercadoLivre = repCargaIntegracaoMercadoLivre.BuscarPorCodigo(codigo, false);

                Servicos.Embarcador.Integracao.MercadoLivre.IntegracaoMercadoLivre svcMercadoLivre = new Servicos.Embarcador.Integracao.MercadoLivre.IntegracaoMercadoLivre(unitOfWork);

                if (!svcMercadoLivre.ReprocessarHandLingUnitDaCargaPedido(out string mensagemErro, cargaIntegracaoMercadoLivre, TipoServicoMultisoftware, unitOfWork, Auditado))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, mensagemErro);
                }

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao reprocessar o Handling Unit informado.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DesconsiderarDocumentoMercadoLivre()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_InformarDocumentosFiscais))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int codigo = Request.GetIntParam("Codigo");
                int codigoCargaIntegracaoMercadoLivre = Request.GetIntParam("CodigoCargaIntegracaoMercadoLivre");

                Repositorio.Embarcador.Cargas.CargaIntegracaoMercadoLivre repCargaIntegracaoMercadoLivre = new Repositorio.Embarcador.Cargas.CargaIntegracaoMercadoLivre(unitOfWork);
                Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetail repMercadoLivreHandlingUnitDetail = new Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetail(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoMercadoLivre cargaIntegracaoMercadoLivre = repCargaIntegracaoMercadoLivre.BuscarPorCodigo(codigoCargaIntegracaoMercadoLivre, false);
                Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail mercadoLivreHandLingUnitDetail = repMercadoLivreHandlingUnitDetail.BuscarPorCodigo(codigo, false);

                Servicos.Embarcador.Integracao.MercadoLivre.IntegracaoMercadoLivre svcMercadoLivre = new Servicos.Embarcador.Integracao.MercadoLivre.IntegracaoMercadoLivre(unitOfWork);

                if (!svcMercadoLivre.DesconsiderarDocumentoMercadoLivre(out string mensagemErro, cargaIntegracaoMercadoLivre, mercadoLivreHandLingUnitDetail, TipoServicoMultisoftware, unitOfWork, Auditado))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, mensagemErro);
                }

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao desconsiderar o documento informado.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterConfiguracaoIntegracaoMercadoLivre()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
           
            try
            {
                Repositorio.Embarcador.Configuracoes.IntegracaoMercadoLivre repConfiguracaoIntegracaoMercadoLivre = new Repositorio.Embarcador.Configuracoes.IntegracaoMercadoLivre(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMercadoLivre configuracaoMercadoLivre = repConfiguracaoIntegracaoMercadoLivre.BuscarPrimeiroRegistro();

                dynamic configuracao = new
                {
                    LimparComposicaoCargaRetiradaRotaFacility = configuracaoMercadoLivre?.LimparComposicaoCargaRetiradaRotaFacility ?? false
                };

                return await Task.FromResult(new JsonpResult(configuracao));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar as configurações de integração do mercado livre.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

            
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid ObterGridPesquisaDetalhesHandLingUnit()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                // Busca Dados
                int totalRegistros = 0;

                Dominio.ObjetosDeValor.Embarcador.Integracao.MercadoLivre.FiltroPesquisaMercadoLivreHandlingUnitDetail filtroPesquisa = new Dominio.ObjetosDeValor.Embarcador.Integracao.MercadoLivre.FiltroPesquisaMercadoLivreHandlingUnitDetail()
                {
                    CodigoCargaMercadoLivre = Request.GetIntParam("Codigo"),
                    SituacaoIntegracaoMercadoLivre = Request.GetEnumParam<SituacaoIntegracaoMercadoLivre>("SituacaoIntegracaoMercadoLivre"),
                    ExibirApenasDocumentosComMensagemErro = Request.GetBoolParam("ExibirApenasDocumentosComMensagemErro")
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoCargaIntegracaoMercadoLivre", false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Situacao, "Situacao", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.TipoDoDocumento, "TipoDoDocumento", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.ChaveDeAcesso, "ChaveDeAcesso", 20, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.ValorMercadoria, "ValorDeMercadoria", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Mensagem, "Mensagem", 20, Models.Grid.Align.right, false);

                Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetail repMercadoLivreHandlingUnitDetail = new Repositorio.Embarcador.Integracao.MercadoLivreHandlingUnitDetail(unitOfWork);

                totalRegistros = repMercadoLivreHandlingUnitDetail.ContarConsultarMercadoLivreHandlingUnitDetailSQL(filtroPesquisa);
                IList<Dominio.ObjetosDeValor.Embarcador.Integracao.MercadoLivre.ConsultaMercadoLivreHandlingUnitDetail> lista = totalRegistros > 0 ? repMercadoLivreHandlingUnitDetail.ConsultarMercadoLivreHandlingUnitDetailSQL(filtroPesquisa, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite) : new List<Dominio.ObjetosDeValor.Embarcador.Integracao.MercadoLivre.ConsultaMercadoLivreHandlingUnitDetail>();

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
