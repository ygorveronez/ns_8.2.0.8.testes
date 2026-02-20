using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Frete;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Fretes
{
    [CustomAuthorize(new string[] { "BuscarResumoAprovacao", "DetalhesAutorizacao", "PesquisaAutorizacoes", "VerificarVinculoDeRotas", "BuscarPracaPedagioTarifaRota" }, "Fretes/TabelaFreteCliente")]
    public class TabelaFreteClienteController : BaseController
    {
        #region Construtores

        public TabelaFreteClienteController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteCliente filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                Repositorio.Embarcador.Frete.TabelaFreteCliente repTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unitOfWork);

                bool exibirColunaSituacaoAprovacao = this.ConfiguracaoEmbarcador?.UtilizarAlcadaAprovacaoTabelaFrete ?? false;
                bool exibirColunaSituacaoAjuste = this.ConfiguracaoEmbarcador?.ExibirSituacaoAjusteTabelaFrete ?? false;
                int tamanhoAdicionalColuna = 4;

                if (exibirColunaSituacaoAprovacao)
                    tamanhoAdicionalColuna -= 2;

                if (exibirColunaSituacaoAjuste)
                    tamanhoAdicionalColuna -= 2;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Fretes.TabelaFreteCliente.Codigo, "CodigoIntegracao", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Fretes.TabelaFreteCliente.Tabela, "TabelaFrete", (10 + tamanhoAdicionalColuna), Models.Grid.Align.left, true);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                {
                    grid.AdicionarCabecalho(Localization.Resources.Fretes.TabelaFreteCliente.Transportador, "Empresa", (11 + tamanhoAdicionalColuna), Models.Grid.Align.left, true);
                    grid.AdicionarCabecalho(Localization.Resources.Fretes.TabelaFreteCliente.Origem, "Origem", (11 + tamanhoAdicionalColuna), Models.Grid.Align.left, true);
                    grid.AdicionarCabecalho(Localization.Resources.Fretes.TabelaFreteCliente.Destino, "Destino", (11 + tamanhoAdicionalColuna), Models.Grid.Align.left, true);
                    grid.AdicionarCabecalho(Localization.Resources.Fretes.TabelaFreteCliente.Vigencia, "Vigencia", 11, Models.Grid.Align.left, true);
                }
                else
                {
                    grid.AdicionarCabecalho(Localization.Resources.Fretes.TabelaFreteCliente.Origem, "Origem", (15 + tamanhoAdicionalColuna), Models.Grid.Align.left, true);
                    grid.AdicionarCabecalho(Localization.Resources.Fretes.TabelaFreteCliente.Destino, "Destino", (15 + tamanhoAdicionalColuna), Models.Grid.Align.left, true);
                    grid.AdicionarCabecalho(Localization.Resources.Fretes.TabelaFreteCliente.Vigencia, "Vigencia", (14 + tamanhoAdicionalColuna), Models.Grid.Align.left, true);
                }

                if (exibirColunaSituacaoAprovacao)
                    grid.AdicionarCabecalho(Localization.Resources.Fretes.TabelaFreteCliente.SituacaoAprovacao, "SituacaoAlteracao", 12, Models.Grid.Align.left, true);

                if (exibirColunaSituacaoAjuste)
                    grid.AdicionarCabecalho(Localization.Resources.Fretes.TabelaFreteCliente.SituacaoAjuste, "DescricaoSituacaoAjusteTabelaFrete", 12, Models.Grid.Align.left, true);

                grid.AdicionarCabecalho(Localization.Resources.Fretes.TabelaFreteCliente.RegiaoDeOrigem, "RegiaoOrigem", (8 + tamanhoAdicionalColuna), Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Fretes.TabelaFreteCliente.StatusAceiteTabela, "StatusAceiteTabela", (8 + tamanhoAdicionalColuna), Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Fretes.TabelaFreteCliente.Situacao, "Ativo", (8 + tamanhoAdicionalColuna), Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Fretes.TabelaFreteCliente.ContratoTransportador, "ContratoTransporteFrete", (8 + tamanhoAdicionalColuna), Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação Integração", "DescricaoSituacaoIntegracao", (8 + tamanhoAdicionalColuna), Models.Grid.Align.left, false, true);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenar == "TabelaFrete")
                    propOrdenar = "TabelaFrete.Descricao";
                else if (propOrdenar == "Origem")
                    propOrdenar = "ClienteOrigem.Nome";
                else if (propOrdenar == "Destino")
                    propOrdenar = "ClienteDestino.Nome " + grid.dirOrdena + ", RegiaoDestino.Descricao " + grid.dirOrdena + ", EstadoDestino.Nome";
                else if (propOrdenar == "Vigencia")
                    propOrdenar = "Vigencia.DataInicial";
                else if (propOrdenar == "Empresa")
                    propOrdenar = "Empresa.RazaoSocial";

                int totalRegistros = repTabelaFreteCliente.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> listaTabelaFreteCliente = totalRegistros > 0 ? repTabelaFreteCliente.Consultar(filtrosPesquisa, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite) : new List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();
                Repositorio.Embarcador.Frete.AjusteTabelaFrete repositorioAjusteTabelaFrete = new Repositorio.Embarcador.Frete.AjusteTabelaFrete(unitOfWork);
                List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> tabelasClienteAjuste = new List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();

                if (exibirColunaSituacaoAjuste)
                    tabelasClienteAjuste = repositorioAjusteTabelaFrete.ObterAjustePorTabelaFreteCliente(listaTabelaFreteCliente.Select(obj => obj.Codigo).ToList());

                dynamic lista = (
                    from obj in listaTabelaFreteCliente
                    select new
                    {
                        obj.Codigo,
                        obj.CodigoIntegracao,
                        TabelaFrete = obj.TabelaFrete.Descricao,
                        Origem = obj.DescricaoOrigem,
                        Empresa = obj.Empresa?.Descricao ?? "",
                        Destino = obj.DescricaoDestino,
                        Vigencia = obj.DescricaoVigencia,
                        Ativo = obj.DescricaoAtivo,
                        SituacaoAlteracao = obj.SituacaoAlteracao.ObterDescricaoPorTabelaFreteCliente(),
                        StatusAceiteTabela = obj?.StatusAceiteTabela ?? "",
                        RegiaoOrigem = string.Join(", ", obj.RegioesOrigem.Select(x => x.Descricao).ToList()),
                        DescricaoSituacaoAjusteTabelaFrete = exibirColunaSituacaoAjuste ? ObterSituacaoAjusteTabelaFrete(obj, tabelasClienteAjuste) : "",
                        ContratoTransporteFrete = obj.ContratoTransporteFrete?.Descricao ?? "",
                        DescricaoSituacaoIntegracao = obj.SituacaoIntegracaoTabelaFreteCliente.ObterDescricao(),
                    }
                ).ToList();

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
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
                Repositorio.Embarcador.Frete.TabelaFreteCliente repTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unitOfWork);
                Repositorio.Embarcador.Frete.TabelaFreteClienteCEPOrigem repTabelaFreteClienteCEPOrigem = new Repositorio.Embarcador.Frete.TabelaFreteClienteCEPOrigem(unitOfWork);
                Repositorio.Embarcador.Frete.TabelaFreteClienteCEPDestino repTabelaFreteClienteCEPDestino = new Repositorio.Embarcador.Frete.TabelaFreteClienteCEPDestino(unitOfWork);
                Servicos.Embarcador.Frete.TabelaFreteCliente servicoTabelaFreteCliente = new Servicos.Embarcador.Frete.TabelaFreteCliente(unitOfWork);
                Servicos.Embarcador.Frete.TabelaFreteAprovacaoAlcada servicoTabelaFreteAprovacao = new Servicos.Embarcador.Frete.TabelaFreteAprovacaoAlcada(unitOfWork);
                Servicos.Embarcador.Frete.TabelaFreteIntegracao servicoTabelaFreteIntegracao = new Servicos.Embarcador.Frete.TabelaFreteIntegracao(unitOfWork);
                Servicos.Embarcador.Frete.TabelaFreteClienteIntegracao servicoTabelaFreteClienteIntegracao = new Servicos.Embarcador.Frete.TabelaFreteClienteIntegracao(unitOfWork);

                Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente = new Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente();

                string descricaoOrigem = string.Empty;
                string descricaoDestino = string.Empty;

                unitOfWork.Start();

                PreencherTabelaFreteCliente(tabelaFreteCliente, unitOfWork);

                ValidarTabelaFreteCliente(tabelaFreteCliente, unitOfWork);

                SetarLocalidadesOrigem(ref descricaoOrigem, ref tabelaFreteCliente, unitOfWork);
                SetarClientesOrigem(ref descricaoOrigem, ref tabelaFreteCliente, unitOfWork);
                SetarEstadosOrigem(ref descricaoOrigem, ref tabelaFreteCliente, unitOfWork);
                SetarRegioesOrigem(ref descricaoOrigem, ref tabelaFreteCliente, unitOfWork);
                SetarRotasOrigem(ref descricaoOrigem, ref tabelaFreteCliente, unitOfWork);
                SetarPaisesOrigem(ref descricaoOrigem, ref tabelaFreteCliente, unitOfWork);
                SetarLocalidadesDestino(ref descricaoDestino, ref tabelaFreteCliente, unitOfWork);
                SetarClientesDestino(ref descricaoDestino, ref tabelaFreteCliente, unitOfWork);
                SetarEstadosDestino(ref descricaoDestino, ref tabelaFreteCliente, unitOfWork);
                SetarRegioesDestino(ref descricaoDestino, ref tabelaFreteCliente, unitOfWork);
                SetarRotasDestino(ref descricaoDestino, ref tabelaFreteCliente, unitOfWork);
                SetarPaisesDestino(ref descricaoDestino, ref tabelaFreteCliente, unitOfWork);
                SetarTiposOperacao(tabelaFreteCliente, unitOfWork);
                SetarTransportadoresTerceiros(tabelaFreteCliente, unitOfWork);
                SalvarFronteiras(ref tabelaFreteCliente, unitOfWork);
                SetarTiposCarga(tabelaFreteCliente, unitOfWork);

                repTabelaFreteCliente.Inserir(tabelaFreteCliente, Auditado);

                SalvarCEPsOrigem(ref descricaoOrigem, tabelaFreteCliente, unitOfWork);
                SalvarCEPsDestino(ref descricaoDestino, tabelaFreteCliente, unitOfWork);
                SalvarModelosVeicularesCarga(tabelaFreteCliente, unitOfWork, true);

                if (!string.IsNullOrWhiteSpace(descricaoOrigem) && descricaoOrigem.StartsWith(" / "))
                    tabelaFreteCliente.DescricaoOrigem = descricaoOrigem.Remove(0, 3);
                else
                    tabelaFreteCliente.DescricaoOrigem = descricaoOrigem;

                if (!string.IsNullOrWhiteSpace(descricaoDestino) && descricaoDestino.StartsWith(" / "))
                    tabelaFreteCliente.DescricaoDestino = descricaoDestino.Remove(0, 3);
                else
                    tabelaFreteCliente.DescricaoDestino = descricaoDestino;

                if (tabelaFreteCliente.Vigencia != null && tabelaFreteCliente.Vigencia.Empresa != null && tabelaFreteCliente.Vigencia.Empresa?.Codigo != tabelaFreteCliente.Empresa?.Codigo)
                    throw new ControllerException(Localization.Resources.Fretes.TabelaFreteCliente.NaoEPermitidoUtilizarAVigenciaDeOutroTransportador);

                if (tabelaFreteCliente.ContratoTransporteFrete != null && tabelaFreteCliente.Empresa != null && tabelaFreteCliente.ContratoTransporteFrete.Transportador?.Codigo != tabelaFreteCliente.Empresa?.Codigo)
                    throw new ControllerException(Localization.Resources.Fretes.TabelaFreteCliente.NaoEPossivelCadastrarUmaTabelaParaUmClienteComUmTransportadorDiferenteDoContrato);

                repTabelaFreteCliente.Atualizar(tabelaFreteCliente);

                List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteCEPOrigem> cepsOrigem = repTabelaFreteClienteCEPOrigem.BuscarPorTabelaFrete(tabelaFreteCliente.Codigo);
                List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteCEPDestino> cepsDestino = repTabelaFreteClienteCEPDestino.BuscarPorTabelaFrete(tabelaFreteCliente.Codigo);

                Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteClienteExiste = repTabelaFreteCliente.BuscarTabelaComMesmaIncidencia(tabelaFreteCliente, cepsOrigem, cepsDestino);

                if (tabelaFreteClienteExiste == null || tabelaFreteClienteExiste.Codigo == tabelaFreteCliente.Codigo || tabelaFreteCliente?.CanalEntrega?.Codigo != tabelaFreteClienteExiste?.CanalEntrega?.Codigo)
                {
                    Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosValores parametrosValores = PreencherParametrosValores();
                    parametrosValores.TabelaFreteCliente = tabelaFreteCliente;
                    servicoTabelaFreteCliente.SalvarValores(parametrosValores, Auditado);
                    if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                        ValidarCamposObrigatoriosModeloVeicular(unitOfWork, tabelaFreteCliente);
                    SalvarSubcontratacoes(tabelaFreteCliente, unitOfWork);

                    servicoTabelaFreteAprovacao.AtualizarAprovacao(tabelaFreteCliente, Usuario, TipoServicoMultisoftware);
                    servicoTabelaFreteIntegracao.AdicionarAlteracao(tabelaFreteCliente);
                    servicoTabelaFreteClienteIntegracao.AdicionarIntegracoes(tabelaFreteCliente);

                    unitOfWork.CommitChanges();

                    return new JsonpResult(true);
                }
                else
                    throw new ControllerException(Localization.Resources.Fretes.TabelaFreteCliente.JaExisteUmFreteCadastradoParaEssaConfiguracaoDeFrete);
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
                return new JsonpResult(false, Localization.Resources.Fretes.TabelaFreteCliente.OcorreuUmaFalhaAoAdicionar);
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
                Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente = repositorioTabelaFreteCliente.BuscarPorCodigo(codigo, true);

                if (tabelaFreteCliente == null)
                    throw new ControllerException(Localization.Resources.Fretes.TabelaFreteCliente.NaoFoiPossivelEncontrarORegistro);

                if (new Servicos.Embarcador.Frete.MensagemAlertaTabelaFreteCliente(unitOfWork).IsMensagemSemConfirmacao(tabelaFreteCliente, TipoMensagemAlerta.AjusteTabelaFreteCliente))
                    throw new ControllerException("Não é possível alterar valores da tabela de frete com ajuste aguardando retorno");

                Repositorio.Embarcador.Frete.TabelaFreteClienteCEPOrigem repositorioTabelaFreteClienteCEPOrigem = new Repositorio.Embarcador.Frete.TabelaFreteClienteCEPOrigem(unitOfWork);
                Repositorio.Embarcador.Frete.TabelaFreteClienteCEPDestino repositorioTabelaFreteClienteCEPDestino = new Repositorio.Embarcador.Frete.TabelaFreteClienteCEPDestino(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
                Servicos.Embarcador.Frete.TabelaFreteCliente servicoTabelaFreteCliente = new Servicos.Embarcador.Frete.TabelaFreteCliente(unitOfWork);
                Servicos.Embarcador.Frete.TabelaFreteAprovacaoAlcada servicoTabelaFreteAprovacao = new Servicos.Embarcador.Frete.TabelaFreteAprovacaoAlcada(unitOfWork);
                Servicos.Embarcador.Frete.TabelaFreteIntegracao servicoTabelaFreteIntegracao = new Servicos.Embarcador.Frete.TabelaFreteIntegracao(unitOfWork);
                Servicos.Embarcador.Frete.TabelaFreteClienteIntegracao servicoTabelaFreteClienteIntegracao = new Servicos.Embarcador.Frete.TabelaFreteClienteIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracaoLBC = repositorioTipoIntegracao.BuscarPorTipo(TipoIntegracao.LBC);
                string descricaoOrigem = string.Empty;
                string descricaoDestino = string.Empty;

                servicoTabelaFreteCliente.DuplicarParaHistoricoAlteracao(tabelaFreteCliente, this.Usuario);

                PreencherTabelaFreteCliente(tabelaFreteCliente, unitOfWork);
                SetarLocalidadesOrigem(ref descricaoOrigem, ref tabelaFreteCliente, unitOfWork);
                SetarClientesOrigem(ref descricaoOrigem, ref tabelaFreteCliente, unitOfWork);
                SetarEstadosOrigem(ref descricaoOrigem, ref tabelaFreteCliente, unitOfWork);
                SetarRegioesOrigem(ref descricaoOrigem, ref tabelaFreteCliente, unitOfWork);
                SetarRotasOrigem(ref descricaoOrigem, ref tabelaFreteCliente, unitOfWork);
                SetarPaisesOrigem(ref descricaoOrigem, ref tabelaFreteCliente, unitOfWork);
                SetarTiposOperacao(tabelaFreteCliente, unitOfWork);
                SetarTransportadoresTerceiros(tabelaFreteCliente, unitOfWork);
                SetarLocalidadesDestino(ref descricaoDestino, ref tabelaFreteCliente, unitOfWork);
                SetarClientesDestino(ref descricaoDestino, ref tabelaFreteCliente, unitOfWork);
                SetarEstadosDestino(ref descricaoDestino, ref tabelaFreteCliente, unitOfWork);
                SetarRegioesDestino(ref descricaoDestino, ref tabelaFreteCliente, unitOfWork);
                SetarRotasDestino(ref descricaoDestino, ref tabelaFreteCliente, unitOfWork);
                SetarPaisesDestino(ref descricaoDestino, ref tabelaFreteCliente, unitOfWork);
                SalvarCEPsOrigem(ref descricaoOrigem, tabelaFreteCliente, unitOfWork);
                SalvarCEPsDestino(ref descricaoDestino, tabelaFreteCliente, unitOfWork);
                SalvarModelosVeicularesCarga(tabelaFreteCliente, unitOfWork, false);
                SalvarFronteiras(ref tabelaFreteCliente, unitOfWork);
                SetarTiposCarga(tabelaFreteCliente, unitOfWork);

                if (!string.IsNullOrWhiteSpace(descricaoOrigem) && descricaoOrigem.StartsWith(" / "))
                    tabelaFreteCliente.DescricaoOrigem = descricaoOrigem.Remove(0, 3);
                else
                    tabelaFreteCliente.DescricaoOrigem = descricaoOrigem;

                if (!string.IsNullOrWhiteSpace(descricaoDestino) && descricaoDestino.StartsWith(" / "))
                    tabelaFreteCliente.DescricaoDestino = descricaoDestino.Remove(0, 3);
                else
                    tabelaFreteCliente.DescricaoDestino = descricaoDestino;

                tabelaFreteCliente.PendenteIntegracao = tabelaFreteCliente.PendenteIntegracao || ((tipoIntegracaoLBC != null) && tabelaFreteCliente.IsChangedByPropertyName("TipoIntegracao"));

                repositorioTabelaFreteCliente.Atualizar(tabelaFreteCliente, Auditado);

                if (tabelaFreteCliente.Vigencia != null && tabelaFreteCliente.Vigencia.Empresa != null && tabelaFreteCliente.Vigencia.Empresa?.Codigo != tabelaFreteCliente.Empresa?.Codigo)
                    throw new ControllerException(Localization.Resources.Fretes.TabelaFreteCliente.NaoEPermitidoUtilizarAVigenciaDeOutroTransportador);

                List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteCEPOrigem> cepsOrigem = repositorioTabelaFreteClienteCEPOrigem.BuscarPorTabelaFrete(tabelaFreteCliente.Codigo);
                List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteCEPDestino> cepsDestino = repositorioTabelaFreteClienteCEPDestino.BuscarPorTabelaFrete(tabelaFreteCliente.Codigo);
                Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteClienteExiste = !tabelaFreteCliente.Ativo ? null : repositorioTabelaFreteCliente.BuscarTabelaComMesmaIncidencia(tabelaFreteCliente, cepsOrigem, cepsDestino);

                if (tabelaFreteClienteExiste?.Codigo == tabelaFreteCliente.Codigo)
                    throw new ControllerException(Localization.Resources.Fretes.TabelaFreteCliente.JaExisteUmFreteCadastradoParaEssaConfiguracaoDeFrete);

                Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosValores parametrosValores = PreencherParametrosValores();
                parametrosValores.TabelaFreteCliente = tabelaFreteCliente;
                servicoTabelaFreteCliente.SalvarValores(parametrosValores, Auditado);
                SalvarSubcontratacoes(tabelaFreteCliente, unitOfWork);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                    ValidarCamposObrigatoriosModeloVeicular(unitOfWork, tabelaFreteCliente);

                if (PermitirSolicitarAprovacao(tabelaFreteCliente, tipoIntegracaoLBC, unitOfWork))
                    servicoTabelaFreteAprovacao.AtualizarAprovacao(tabelaFreteCliente, Usuario, TipoServicoMultisoftware);
                else if (tabelaFreteCliente.SituacaoAlteracao.IsAlteracaoTabelaFreteClienteLiberada() && (tipoIntegracaoLBC != null))
                {
                    servicoTabelaFreteClienteIntegracao.AdicionarIntegracao(tabelaFreteCliente, tipoIntegracaoLBC);
                    tabelaFreteCliente.SituacaoIntegracaoTabelaFreteCliente = SituacaoIntegracaoTabelaFreteCliente.AguardandoIntegracao;
                }

                servicoTabelaFreteIntegracao.AdicionarAlteracao(tabelaFreteCliente);
                servicoTabelaFreteClienteIntegracao.AdicionarIntegracoes(tabelaFreteCliente);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Fretes.TabelaFreteCliente.OcorreuUmaFalhaAoAtualizar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarRota()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unitOfWork);
                Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(unitOfWork);
                int codigo = Request.GetIntParam("Codigo");

                int codigoRotaFrete = Request.GetIntParam("RotaFrete");

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente = repositorioTabelaFreteCliente.BuscarPorCodigo(codigo, true);

                if (tabelaFreteCliente == null)
                    throw new ControllerException(Localization.Resources.Fretes.TabelaFreteCliente.NaoFoiPossivelEncontrarORegistro);

                tabelaFreteCliente.RotaFrete = repRotaFrete.BuscarPorCodigo(codigoRotaFrete);

                repositorioTabelaFreteCliente.Atualizar(tabelaFreteCliente, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Fretes.TabelaFreteCliente.OcorreuUmaFalhaAoAtualizar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Frete.TabelaFreteCliente repTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unidadeDeTrabalho);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete repositorioConfiguracaoTabelaFrete = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete(unidadeDeTrabalho);
                Repositorio.Embarcador.Frete.TabelaFreteClienteFrequenciaEntrega repositorioFrequenciaEntrega = new Repositorio.Embarcador.Frete.TabelaFreteClienteFrequenciaEntrega(unidadeDeTrabalho);
                Repositorio.Embarcador.Frete.TabelaFreteClienteModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Frete.TabelaFreteClienteModeloVeicularCarga(unidadeDeTrabalho);
                Servicos.Embarcador.Frete.MensagemAlertaTabelaFreteCliente servicoMensagemAlertaTabelaFreteCliente = new Servicos.Embarcador.Frete.MensagemAlertaTabelaFreteCliente(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente = repTabelaFreteCliente.BuscarPorCodigo(codigo);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTabelaFrete configuracaoTabelaFrete = repositorioConfiguracaoTabelaFrete.BuscarPrimeiroRegistro();
                List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteFrequenciaEntrega> restricoesEntrega = repositorioFrequenciaEntrega.BuscarPorCodigoTabelaFreteCliente(tabelaFreteCliente.Codigo);
                List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteModeloVeicularCarga> modelosVeicularesCarga = repositorioModeloVeicularCarga.BuscarPorTabelaFreteCliente(tabelaFreteCliente.Codigo);
                bool ajusteAguardandoRetorno = servicoMensagemAlertaTabelaFreteCliente.IsMensagemSemConfirmacao(tabelaFreteCliente, TipoMensagemAlerta.AjusteTabelaFreteCliente);

                return new JsonpResult(ObterDetalhesTabelaFreteCliente(tabelaFreteCliente, false, false, (configuracaoTabelaFrete.PermitirInformarLeadTimeTabelaFreteCliente && (tabelaFreteCliente.TabelaFrete.TipoCalculo == TipoCalculoTabelaFrete.PorPedido || tabelaFreteCliente.TabelaFrete.TipoCalculo == TipoCalculoTabelaFrete.PorPedidosAgrupados)), ajusteAguardandoRetorno, restricoesEntrega, modelosVeicularesCarga));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Fretes.TabelaFreteCliente.OcorreuUmaFalhaAoBuscarPorCodigo);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarParaDuplicar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Frete.TabelaFreteCliente repTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unidadeDeTrabalho);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete repositorioConfiguracaoTabelaFrete = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete(unidadeDeTrabalho);
                Repositorio.Embarcador.Frete.TabelaFreteClienteFrequenciaEntrega repositorioFrequenciaEntrega = new Repositorio.Embarcador.Frete.TabelaFreteClienteFrequenciaEntrega(unidadeDeTrabalho);
                Repositorio.Embarcador.Frete.TabelaFreteClienteModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Frete.TabelaFreteClienteModeloVeicularCarga(unidadeDeTrabalho);
                Servicos.Embarcador.Frete.MensagemAlertaTabelaFreteCliente servicoMensagemAlertaTabelaFreteCliente = new Servicos.Embarcador.Frete.MensagemAlertaTabelaFreteCliente(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente = repTabelaFreteCliente.BuscarPorCodigo(codigo);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTabelaFrete configuracaoTabelaFrete = repositorioConfiguracaoTabelaFrete.BuscarPrimeiroRegistro();
                List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteFrequenciaEntrega> restricoesEntrega = repositorioFrequenciaEntrega.BuscarPorCodigoTabelaFreteCliente(tabelaFreteCliente.Codigo);
                List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteModeloVeicularCarga> modelosVeicularesCarga = repositorioModeloVeicularCarga.BuscarPorTabelaFreteCliente(tabelaFreteCliente.Codigo);
                bool ajusteAguardandoRetorno = servicoMensagemAlertaTabelaFreteCliente.IsMensagemSemConfirmacao(tabelaFreteCliente, TipoMensagemAlerta.AjusteTabelaFreteCliente);

                return new JsonpResult(ObterDetalhesTabelaFreteCliente(tabelaFreteCliente, true, Request.GetBoolParam("DuplicarComoRetorno"), (configuracaoTabelaFrete.PermitirInformarLeadTimeTabelaFreteCliente && (tabelaFreteCliente.TabelaFrete.TipoCalculo == TipoCalculoTabelaFrete.PorPedido || tabelaFreteCliente.TabelaFrete.TipoCalculo == TipoCalculoTabelaFrete.PorPedidosAgrupados)), ajusteAguardandoRetorno, restricoesEntrega, modelosVeicularesCarga));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Fretes.TabelaFreteCliente.OcorreuUmaFalhaAoBuscarPorCodigo);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarResumoAprovacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoTabelaFreteCliente = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente = repositorioTabelaFreteCliente.BuscarPorCodigo(codigoTabelaFreteCliente);

                if (tabelaFreteCliente == null)
                    return new JsonpResult(false, Localization.Resources.Fretes.TabelaFreteCliente.NaoFoiPossivelEncontrarORegistro);

                if (tabelaFreteCliente.SituacaoAlteracao == SituacaoAlteracaoTabelaFrete.SemRegraAprovacao)
                    return new JsonpResult(new
                    {
                        tabelaFreteCliente.Codigo,
                        DescricaoSituacao = tabelaFreteCliente.SituacaoAlteracao.ObterDescricaoPorTabelaFreteCliente(),
                        PossuiAlcada = true,
                        PossuiRegras = false
                    });

                Repositorio.Embarcador.Frete.TabelaFreteAlteracao repositorioTabelaFreteAlteracao = new Repositorio.Embarcador.Frete.TabelaFreteAlteracao(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.TabelaFreteAlteracao tabelaFreteAlteracao = repositorioTabelaFreteAlteracao.BuscarUltimaPorTabelaFreteCliente(tabelaFreteCliente.Codigo);

                if (tabelaFreteAlteracao == null)
                    return new JsonpResult(new
                    {
                        tabelaFreteCliente.Codigo,
                        PossuiAlcada = false
                    });

                Repositorio.Embarcador.Frete.AlcadasTabelaFrete.AprovacaoAlcadaTabelaFrete repositorioAprovacao = new Repositorio.Embarcador.Frete.AlcadasTabelaFrete.AprovacaoAlcadaTabelaFrete(unitOfWork);
                int aprovacoes = repositorioAprovacao.ContarAprovacoes(tabelaFreteAlteracao.Codigo);
                int aprovacoesNecessarias = repositorioAprovacao.ContarAprovacoesNecessarias(tabelaFreteAlteracao.Codigo);
                int reprovacoes = repositorioAprovacao.ContarReprovacoes(tabelaFreteAlteracao.Codigo);

                return new JsonpResult(new
                {
                    tabelaFreteCliente.Codigo,
                    AprovacoesNecessarias = aprovacoesNecessarias,
                    Aprovacoes = aprovacoes,
                    Reprovacoes = reprovacoes,
                    DescricaoSituacao = tabelaFreteAlteracao.SituacaoAlteracao.ObterDescricaoPorTabelaFreteCliente(),
                    PossuiAlcada = true,
                    PossuiRegras = tabelaFreteAlteracao.SituacaoAlteracao != SituacaoAlteracaoTabelaFrete.SemRegraAprovacao
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Fretes.TabelaFreteCliente.OcorreuUmaFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DetalhesAutorizacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frete.AlcadasTabelaFrete.AprovacaoAlcadaTabelaFrete repositorioAprovacao = new Repositorio.Embarcador.Frete.AlcadasTabelaFrete.AprovacaoAlcadaTabelaFrete(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.AprovacaoAlcadaTabelaFrete autorizacao = repositorioAprovacao.BuscarPorCodigo(codigo);

                if (autorizacao == null)
                    return new JsonpResult(false, Localization.Resources.Fretes.TabelaFreteCliente.NaoFoiPossivelEncontrarORegistro);

                return new JsonpResult(new
                {
                    autorizacao.Codigo,
                    Regra = autorizacao.Descricao,
                    Situacao = autorizacao.Situacao.ObterDescricao(),
                    Usuario = autorizacao.Usuario?.Nome ?? string.Empty,
                    PodeAprovar = autorizacao.IsPermitirAprovacaoOuReprovacao(this.Usuario.Codigo),
                    Data = autorizacao.Data.HasValue ? autorizacao.Data.Value.ToString("dd/MM/yyyy") : string.Empty,
                    Motivo = string.IsNullOrWhiteSpace(autorizacao.Motivo) ? string.Empty : autorizacao.Motivo,
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Fretes.TabelaFreteCliente.OcorreuUmaFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> PesquisaAutorizacoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Regra", false);
                grid.AdicionarCabecalho("Data", false);
                grid.AdicionarCabecalho("Motivo", false);
                grid.AdicionarCabecalho(Localization.Resources.Fretes.TabelaFreteCliente.Usuario, "Usuario", 40, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Fretes.TabelaFreteCliente.Prioridade, "PrioridadeAprovacao", 20, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho(Localization.Resources.Fretes.TabelaFreteCliente.Situacao, "Situacao", 20, Models.Grid.Align.center, false);

                int codigoTabelaFreteCliente = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente = repositorioTabelaFreteCliente.BuscarPorCodigo(codigoTabelaFreteCliente);
                int totalRegistros = 0;
                List<Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.AprovacaoAlcadaTabelaFrete> listaAutorizacao;
                Dominio.Entidades.Embarcador.Frete.TabelaFreteAlteracao tabelaFreteAlteracao = null;

                if (tabelaFreteCliente != null)
                {
                    Repositorio.Embarcador.Frete.TabelaFreteAlteracao repositorioTabelaFreteAlteracao = new Repositorio.Embarcador.Frete.TabelaFreteAlteracao(unitOfWork);
                    tabelaFreteAlteracao = repositorioTabelaFreteAlteracao.BuscarUltimaPorTabelaFreteCliente(tabelaFreteCliente.Codigo);
                }

                if (tabelaFreteAlteracao != null)
                {
                    Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                    Repositorio.Embarcador.Frete.AlcadasTabelaFrete.AprovacaoAlcadaTabelaFrete repositorioAprovacao = new Repositorio.Embarcador.Frete.AlcadasTabelaFrete.AprovacaoAlcadaTabelaFrete(unitOfWork);
                    totalRegistros = repositorioAprovacao.ContarAutorizacoes(tabelaFreteAlteracao.Codigo);
                    listaAutorizacao = totalRegistros > 0 ? repositorioAprovacao.ConsultarAutorizacoes(tabelaFreteAlteracao.Codigo, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.AprovacaoAlcadaTabelaFrete>();
                }
                else
                    listaAutorizacao = new List<Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.AprovacaoAlcadaTabelaFrete>();

                var lista = (
                    from autorizacao in listaAutorizacao
                    select new
                    {
                        autorizacao.Codigo,
                        PrioridadeAprovacao = autorizacao.RegraAutorizacao?.PrioridadeAprovacao ?? 0,
                        Situacao = autorizacao.Situacao.ObterDescricao(),
                        Usuario = autorizacao.Usuario?.Nome,
                        Regra = autorizacao.Descricao,
                        Data = autorizacao.Data.HasValue ? autorizacao.Data.ToString() : string.Empty,
                        Motivo = string.IsNullOrWhiteSpace(autorizacao.Motivo) ? string.Empty : autorizacao.Motivo,
                        DT_RowColor = autorizacao.ObterCorGrid()
                    }
                ).ToList();

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Fretes.TabelaFreteCliente.OcorreuUmaFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReprocessarRegras()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frete.TabelaFreteCliente repositorio = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente = repositorio.BuscarPorCodigo(codigo);

                if (tabelaFreteCliente == null)
                    return new JsonpResult(false, true, Localization.Resources.Fretes.TabelaFreteCliente.NaoFoiPossivelEncontrarORegistro);

                if (tabelaFreteCliente.SituacaoAlteracao != SituacaoAlteracaoTabelaFrete.SemRegraAprovacao)
                    return new JsonpResult(false, true, Localization.Resources.Fretes.TabelaFreteCliente.ASituacaoNaoPermiteEstaOperacao);

                unitOfWork.Start();

                new Servicos.Embarcador.Frete.TabelaFreteAprovacaoAlcada(unitOfWork).AtualizarAprovacao(tabelaFreteCliente, Usuario, TipoServicoMultisoftware);

                unitOfWork.CommitChanges();

                return new JsonpResult(tabelaFreteCliente.SituacaoAlteracao != SituacaoAlteracaoTabelaFrete.SemRegraAprovacao);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, Localization.Resources.Fretes.TabelaFreteCliente.OcorreuUmaFalhaAoReprocessarAsRegras);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        //Temporário, remover 
        public async Task<IActionResult> ReprocessarTabelasAntigas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Frete.TabelaFreteCliente repTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unitOfWork);

                List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> tabelasExistentes = repTabelaFreteCliente.BuscarTodos();


                for (var i = 0; i < tabelasExistentes.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFrete = repTabelaFreteCliente.BuscarPorCodigo(tabelasExistentes[i].Codigo);

                    unitOfWork.Start();

                    if (tabelaFrete.ClienteOrigem != null && tabelaFrete.ClientesOrigem.Count <= 0)
                        tabelaFrete.ClientesOrigem.Add(tabelaFrete.ClienteOrigem);

                    if (tabelaFrete.ClienteDestino != null && tabelaFrete.ClientesDestino.Count <= 0)
                        tabelaFrete.ClientesDestino.Add(tabelaFrete.ClienteDestino);

                    if (tabelaFrete.EstadoDestino != null && tabelaFrete.EstadosDestino.Count <= 0)
                        tabelaFrete.EstadosDestino.Add(tabelaFrete.EstadoDestino);

                    if (tabelaFrete.RegiaoDestino != null && tabelaFrete.RegioesDestino.Count <= 0)
                        tabelaFrete.RegioesDestino.Add(tabelaFrete.RegiaoDestino);

                    tabelaFrete.DescricaoOrigem = string.Empty;
                    tabelaFrete.DescricaoDestino = string.Empty;

                    foreach (Dominio.Entidades.Cliente cliente in tabelaFrete.ClientesOrigem)
                        tabelaFrete.DescricaoOrigem += cliente.Descricao + " / ";

                    foreach (Dominio.Entidades.Localidade localidade in tabelaFrete.Origens)
                        tabelaFrete.DescricaoOrigem += localidade.DescricaoCidadeEstado + " / ";

                    foreach (Dominio.Entidades.Estado estado in tabelaFrete.EstadosOrigem)
                        tabelaFrete.DescricaoOrigem += estado.Nome + " / ";

                    foreach (Dominio.Entidades.Embarcador.Localidades.Regiao regiao in tabelaFrete.RegioesOrigem)
                        tabelaFrete.DescricaoOrigem += regiao.Descricao + " / ";

                    foreach (Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteCEPOrigem faixaCEP in tabelaFrete.CEPsOrigem)
                        tabelaFrete.DescricaoOrigem += string.Format(@"{0:00\.000\-000}", faixaCEP.CEPInicial) + " à " + string.Format(@"{0:00\.000\-000}", faixaCEP.CEPFinal) + " / ";

                    foreach (Dominio.Entidades.RotaFrete rota in tabelaFrete.RotasOrigem)
                        tabelaFrete.DescricaoOrigem += rota.Descricao + " / ";

                    foreach (Dominio.Entidades.Pais pais in tabelaFrete.PaisesOrigem)
                        tabelaFrete.DescricaoOrigem += pais.Descricao + " / ";

                    foreach (Dominio.Entidades.Cliente cliente in tabelaFrete.ClientesDestino)
                        tabelaFrete.DescricaoDestino += cliente.Descricao + " / ";

                    foreach (Dominio.Entidades.Localidade localidade in tabelaFrete.Destinos)
                        tabelaFrete.DescricaoDestino += localidade.DescricaoCidadeEstado + " / ";

                    foreach (Dominio.Entidades.Estado estado in tabelaFrete.EstadosDestino)
                        tabelaFrete.DescricaoDestino += estado.Nome + " / ";

                    foreach (Dominio.Entidades.Embarcador.Localidades.Regiao regiao in tabelaFrete.RegioesDestino)
                        tabelaFrete.DescricaoDestino += regiao.Descricao + " / ";

                    foreach (Dominio.Entidades.RotaFrete rota in tabelaFrete.RotasDestino)
                        tabelaFrete.DescricaoDestino += rota.Descricao + " / ";

                    foreach (Dominio.Entidades.Pais pais in tabelaFrete.PaisesDestino)
                        tabelaFrete.DescricaoDestino += pais.Descricao + " / ";

                    foreach (Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteCEPDestino faixaCEP in tabelaFrete.CEPsDestino)
                        tabelaFrete.DescricaoDestino += string.Format(@"{0:00\.000\-000}", faixaCEP.CEPInicial) + " à " + string.Format(@"{0:00\.000\-000}", faixaCEP.CEPFinal) + " / ";

                    if (tabelaFrete.DescricaoOrigem.Length > 3)
                        tabelaFrete.DescricaoOrigem = Utilidades.String.Left(tabelaFrete.DescricaoOrigem, tabelaFrete.DescricaoOrigem.Length - 3);

                    if (tabelaFrete.DescricaoDestino.Length > 3)
                        tabelaFrete.DescricaoDestino = Utilidades.String.Left(tabelaFrete.DescricaoDestino, tabelaFrete.DescricaoDestino.Length - 3);

                    repTabelaFreteCliente.Atualizar(tabelaFrete);

                    unitOfWork.CommitChanges();

                    unitOfWork.FlushAndClear();
                }

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Fretes.TabelaFreteCliente.OcorreuUmaFalhaAoAdicionar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Frete.TabelaFreteCliente repTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unidadeDeTrabalho);
                Repositorio.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete repItemParametroBase = new Repositorio.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete(unidadeDeTrabalho);
                Repositorio.Embarcador.Frete.ParametroBaseCalculoTabelaFrete repParametroBase = new Repositorio.Embarcador.Frete.ParametroBaseCalculoTabelaFrete(unidadeDeTrabalho);
                Repositorio.Embarcador.Frete.TabelaFreteClienteAlteracao repAlteracoes = new Repositorio.Embarcador.Frete.TabelaFreteClienteAlteracao(unidadeDeTrabalho);
                Repositorio.Embarcador.Frete.TabelaFreteClienteCEPOrigem repCepOrigem = new Repositorio.Embarcador.Frete.TabelaFreteClienteCEPOrigem(unidadeDeTrabalho);
                Repositorio.Embarcador.Frete.TabelaFreteClienteCEPDestino repCepDestino = new Repositorio.Embarcador.Frete.TabelaFreteClienteCEPDestino(unidadeDeTrabalho);

                unidadeDeTrabalho.Start();

                int codigoTabelaFrete = int.Parse(Request.Params("Codigo"));

                Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente = repTabelaFreteCliente.BuscarPorCodigo(codigoTabelaFrete, true);

                foreach (var parametro in tabelaFreteCliente.ParametrosBaseCalculo)
                {
                    foreach (var item in parametro.ItensBaseCalculo)
                        repItemParametroBase.Deletar(item);

                    repParametroBase.Deletar(parametro);
                }

                repAlteracoes.DeletarPorTabelaFreteCliente(codigoTabelaFrete);
                repCepOrigem.DeletarPorTabelaFreteCliente(codigoTabelaFrete);
                repCepDestino.DeletarPorTabelaFreteCliente(codigoTabelaFrete);

                repTabelaFreteCliente.Deletar(tabelaFreteCliente, Auditado);

                unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, Localization.Resources.Fretes.TabelaFreteCliente.NaoFoiPossivelExcluirORegistroPoisOMesmoJaPossuiVinculoComOutrosRecursosDoSistemaRecomendamosQueVoceInativeORegistroCasoNaoDesejaMaisUtilizaLo);

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Fretes.TabelaFreteCliente.OcorreuUmaFalhaAoExcluir);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> VincularRotasSemParar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unitOfWork);

                List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> listaTabelaFreteCliente = repositorioTabelaFreteCliente.BuscarRotasAtivasVigentes();

                if (listaTabelaFreteCliente.Any(h => h.VinculadoSemParar.HasValue == false || h.VinculadoSemParar.Value == false))
                    return new JsonpResult(false);

                listaTabelaFreteCliente.ForEach(x => x.VinculadoSemParar = false);
                listaTabelaFreteCliente.ForEach(x => repositorioTabelaFreteCliente.Atualizar(x));

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, Localization.Resources.Fretes.TabelaFreteCliente.OcorreuUmaFalhaAoSolicitarAVinculacaoDasRotas);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> VerificarVinculoDeRotas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unitOfWork);

                bool vinculadas = repositorioTabelaFreteCliente.ExistemRotasAtivasVigentesSemVinculoSemParar();

                return new JsonpResult(new
                {
                    Vinculadas = vinculadas
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Fretes.TabelaFreteCliente.OcorreuUmaFalhaAoValidarOVinculoDasRotas);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPracaPedagioTarifaRota()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid() { header = new List<Models.Grid.Head>() };
                grid.AdicionarCabecalho("CodigoModeloVeicularCarga", false);
                grid.AdicionarCabecalho(Localization.Resources.Fretes.TabelaFreteCliente.ModeloVeicularDeCarga, "DescricaoModeloVeicularCarga", 55, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Fretes.TabelaFreteCliente.Eixos, "Eixos", 15, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho(Localization.Resources.Fretes.TabelaFreteCliente.Pracas, "Pracas", 15, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho(Localization.Resources.Fretes.TabelaFreteCliente.ValorTotal, "ValorTotal", 15, Models.Grid.Align.right, false);

                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Logistica.PracaPedagioTarifa repPracaPedagioTarifa = new Repositorio.Embarcador.Logistica.PracaPedagioTarifa(unitOfWork);
                List<Dominio.ObjetosDeValor.Embarcador.Frete.TarifaModeloVeicular> tarifasModeloVeicularSumatizadas = repPracaPedagioTarifa.BuscarSumarizadasPorRotaFrete(codigo);
                var lista = (
                    from row in tarifasModeloVeicularSumatizadas
                    select new
                    {
                        CodigoModeloVeicularCarga = row.ModeloVeicularCarga.Codigo,
                        DescricaoModeloVeicularCarga = row.ModeloVeicularCarga.Descricao,
                        Eixos = row.ModeloVeicularCarga.NumeroEixos,
                        Pracas = String.Empty,
                        ValorTotal = row.Tarifa.ToString("n4")
                    }
                ).ToList();

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(lista?.Count ?? 0);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Fretes.TabelaFreteCliente.OcorreuUmaFalhaAoBuscarAsTarifasDasPracasDePedagioParaARota);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private bool PermitirSolicitarAprovacao(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracaoLBC, Repositorio.UnitOfWork unitOfWork)
        {
            if (!tabelaFreteCliente.Ativo)
                return false;

            if (tipoIntegracaoLBC == null)
                return true;

            if (
                tabelaFreteCliente.IsChangedByPropertyName("Origens") ||
                tabelaFreteCliente.IsChangedByPropertyName("EstadosOrigem") ||
                tabelaFreteCliente.IsChangedByPropertyName("PaisesOrigem") ||
                tabelaFreteCliente.IsChangedByPropertyName("RegioesOrigem") ||
                tabelaFreteCliente.IsChangedByPropertyName("RotasOrigem") ||
                tabelaFreteCliente.IsChangedByPropertyName("ClientesOrigem") ||
                tabelaFreteCliente.IsChangedByPropertyName("CEPsOrigem") ||
                tabelaFreteCliente.IsChangedByPropertyName("Destinos") ||
                tabelaFreteCliente.IsChangedByPropertyName("EstadosDestino") ||
                tabelaFreteCliente.IsChangedByPropertyName("PaisesDestino") ||
                tabelaFreteCliente.IsChangedByPropertyName("RegioesDestino") ||
                tabelaFreteCliente.IsChangedByPropertyName("RotasDestino") ||
                tabelaFreteCliente.IsChangedByPropertyName("ClientesDestino") ||
                tabelaFreteCliente.IsChangedByPropertyName("CEPsDestino") ||
                tabelaFreteCliente.IsChangedByPropertyName("DescricaoDestino") ||
                tabelaFreteCliente.IsChangedByPropertyName("CanalVenda") ||
                tabelaFreteCliente.IsChangedByPropertyName("CanalEntrega")
            )
                return true;

            Repositorio.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete repositorioItemParametroBaseCalculo = new Repositorio.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete(unitOfWork);

            if (
                (tabelaFreteCliente.TabelaFrete.ParametroBase.HasValue && repositorioItemParametroBaseCalculo.ExistePendenteAprovacaoPorParametrosTabelaFrete(tabelaFreteCliente.Codigo)) ||
                (!tabelaFreteCliente.TabelaFrete.ParametroBase.HasValue && repositorioItemParametroBaseCalculo.ExistePendenteAprovacaoPorTabelaFrete(tabelaFreteCliente.Codigo))
            )
            {
                tabelaFreteCliente.PermitirCalcularFreteEmAlteracao = true;
                return true;
            }

            return false;
        }

        private void PreencherTabelaFreteCliente(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.TabelaFrete repositorioTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unitOfWork);
            Repositorio.Embarcador.Frete.VigenciaTabelaFrete repositorioVigencia = new Repositorio.Embarcador.Frete.VigenciaTabelaFrete(unitOfWork);
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Rateio.RateioFormula repositorioRateioFormula = new Repositorio.Embarcador.Rateio.RateioFormula(unitOfWork);
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.RotaFrete repositorioRotaFrete = new Repositorio.RotaFrete(unitOfWork);
            Repositorio.Embarcador.Pedidos.CanalEntrega repositorioCanalEntrega = new Repositorio.Embarcador.Pedidos.CanalEntrega(unitOfWork);
            Repositorio.Embarcador.Frete.ContratoTransporteFrete repContratoTransporteFrete = new Repositorio.Embarcador.Frete.ContratoTransporteFrete(unitOfWork);
            Repositorio.Embarcador.Pedidos.CanalVenda repositorioCanalVenda = new Repositorio.Embarcador.Pedidos.CanalVenda(unitOfWork);

            int codigoVigencia = Request.GetIntParam("Vigencia");
            int codigoTabelaFrete = Request.GetIntParam("TabelaFrete");
            int codigoFormulaRateio = Request.GetIntParam("FormulaRateio");
            int codigoEmpresa = Request.GetIntParam("Empresa");
            double codigoFronteira = Request.GetDoubleParam("Fronteira");
            double cpfCnpjTomador = Request.GetDoubleParam("Tomador");
            int codigoRotaFrete = Request.GetIntParam("RotaFrete");
            int codigoCanalEntrega = Request.GetIntParam("CanalEntrega");
            int codigoContratoTransporteFrete = Request.GetIntParam("ContratoTransporteFrete");
            int codigoCanalVenda = Request.GetIntParam("CanalVenda");

            tabelaFreteCliente.PrioridadeUso = Request.GetNullableIntParam("PrioridadeUso");
            tabelaFreteCliente.Moeda = Request.GetEnumParam<MoedaCotacaoBancoCentral>("Moeda");
            tabelaFreteCliente.PercentualCobrancaPadraoTerceiros = Request.GetDecimalParam("PercentualCobrancaPadrao");

            tabelaFreteCliente.PercentualCobrancaVeiculoFrota = Request.GetDecimalParam("PercentualCobrancaVeiculoFrota");
            tabelaFreteCliente.TabelaFrete = repositorioTabelaFrete.BuscarPorCodigo(codigoTabelaFrete);
            tabelaFreteCliente.Tomador = cpfCnpjTomador > 0d ? repositorioCliente.BuscarPorCPFCNPJ(cpfCnpjTomador) : null;
            tabelaFreteCliente.Vigencia = codigoVigencia > 0 ? repositorioVigencia.BuscarPorCodigo(codigoVigencia) : null;

            if (tabelaFreteCliente.Vigencia != null && tabelaFreteCliente.Vigencia.TabelaFrete.Codigo != (tabelaFreteCliente?.TabelaFrete?.Codigo ?? 0))
                throw new ControllerException("Não é possível selecionar uma vigencia de outra tabela, por favor refaça o processo selecionado a vigencia correta");


            tabelaFreteCliente.FormulaRateio = codigoFormulaRateio > 0 ? repositorioRateioFormula.BuscarPorCodigo(codigoFormulaRateio) : null;
            tabelaFreteCliente.Fronteira = codigoFronteira > 0 ? repositorioCliente.BuscarPorCPFCNPJ(codigoFronteira) : null;
            tabelaFreteCliente.Empresa = codigoEmpresa > 0 ? repositorioEmpresa.BuscarPorCodigo(codigoEmpresa) : null;
            tabelaFreteCliente.IncluirICMSValorFrete = Request.GetBoolParam("IncluirICMSValorFrete");
            tabelaFreteCliente.HerdarInclusaoICMSTabelaFrete = Request.GetBoolParam("HerdarInclusaoICMSTabelaFrete");
            tabelaFreteCliente.FreteValidoParaQualquerDestino = Request.GetBoolParam("FreteValidoParaQualquerDestino");
            tabelaFreteCliente.FreteValidoParaQualquerOrigem = Request.GetBoolParam("FreteValidoParaQualquerOrigem");
            tabelaFreteCliente.TipoRateioDocumentos = Request.GetEnumParam<TipoEmissaoCTeDocumentos>("TipoRateioDocumentos");
            tabelaFreteCliente.Ativo = Request.GetBoolParam("Ativo");
            tabelaFreteCliente.PercentualICMSIncluir = tabelaFreteCliente.IncluirICMSValorFrete ? Request.GetDecimalParam("PercentualICMSIncluir") : 0m;
            tabelaFreteCliente.TipoPagamento = Request.GetEnumParam<TipoPagamentoEmissao>("TipoPagamento");
            tabelaFreteCliente.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
            tabelaFreteCliente.Quilometragem = Request.GetIntParam("Quilometragem");
            tabelaFreteCliente.RotaFrete = codigoRotaFrete > 0 ? repositorioRotaFrete.BuscarPorCodigo(codigoRotaFrete) : null;
            tabelaFreteCliente.ObservacaoInterna = Request.GetStringParam("ObservacaoInterna");
            tabelaFreteCliente.LeadTime = Request.GetIntParam("LeadTime");
            tabelaFreteCliente.LeadTimeMinutos = Request.GetIntParam("LeadTimeMinutos");
            tabelaFreteCliente.LeadTimeTransportador = Request.GetIntParam("LeadTimeTransportador");
            tabelaFreteCliente.TipoLeadTime = Request.GetEnumParam<PadraoTempoDiasMinutos>("TipoLeadTime", PadraoTempoDiasMinutos.Minutos);
            tabelaFreteCliente.CanalEntrega = codigoCanalEntrega > 0 ? repositorioCanalEntrega.BuscarPorCodigo(codigoCanalEntrega) : null;
            tabelaFreteCliente.PercentualRota = Request.GetDecimalParam("PercentualRota");
            tabelaFreteCliente.QuantidadeEntregas = Request.GetIntParam("QuantidadeEntregas");
            tabelaFreteCliente.CapacidadeOTM = Request.GetNullableBoolParam("CapacidadeOTM");
            tabelaFreteCliente.DominioOTM = Request.GetEnumParam<DominioOTM>("DominioOTM");
            tabelaFreteCliente.PontoPlanejamentoTransporte = Request.GetEnumParam<PontoPlanejamentoTransporte>("PontoPlanejamentoTransporte");
            tabelaFreteCliente.TipoIntegracao = Request.GetNullableEnumParam<TipoIntegracaoUnilever>("TipoIntegracao");
            tabelaFreteCliente.IDExterno = Request.GetStringParam("IDExterno");
            tabelaFreteCliente.StatusAceiteTabela = Request.GetStringParam("StatusAceiteTabela");
            tabelaFreteCliente.ContratoTransporteFrete = codigoContratoTransporteFrete > 0 ? repContratoTransporteFrete.BuscarPorCodigo(codigoContratoTransporteFrete) : null;
            tabelaFreteCliente.CanalVenda = codigoCanalVenda > 0 ? repositorioCanalVenda.BuscarPorCodigo(codigoCanalVenda) : null;
            tabelaFreteCliente.TipoGrupoCarga = Request.GetEnumParam<TipoGrupoCarga>("TipoGrupoCarga");
            tabelaFreteCliente.GerenciarCapacidade = Request.GetBoolParam("GerenciarCapacidade");
            tabelaFreteCliente.EstruturaTabela = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.EstruturaTabela>("EstruturaTabela");
            tabelaFreteCliente.ObrigatorioInformarValePedagioCarga = Request.GetBoolParam("ObrigatorioInformarValePedagioCarga");
            tabelaFreteCliente.PermitirCalcularFreteEmAlteracao = false;

            decimal percentualCobrancaPadrao = 0;
            if (!string.IsNullOrWhiteSpace(Request.Params("PercentualCobrancaPadrao")))
                decimal.TryParse(Request.Params("PercentualCobrancaPadrao"), out percentualCobrancaPadrao);

            if (percentualCobrancaPadrao > 100)
                throw new CustomException("Conferir o valor 'Cobrança Padrão'");

            tabelaFreteCliente.PercentualCobrancaPadraoTerceiros = percentualCobrancaPadrao;


            decimal percentualCobrancaVeiculoFrota = 0;
            if (!string.IsNullOrWhiteSpace(Request.Params("PercentualCobrancaVeiculoFrota")))
                decimal.TryParse(Request.Params("PercentualCobrancaVeiculoFrota"), out percentualCobrancaVeiculoFrota);

            if (percentualCobrancaVeiculoFrota > 100)
                throw new CustomException("Conferir o valor 'Cobrança Veículo Frota'");

            tabelaFreteCliente.PercentualCobrancaVeiculoFrota = percentualCobrancaVeiculoFrota;

            SetarRestricoesEntrega(ref tabelaFreteCliente, unitOfWork);
        }

        private dynamic ObterCEPSOrigem(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, bool dadosParaDuplicar, bool duplicarComoRetorno)
        {
            if (duplicarComoRetorno)
                return ObterCEPSDestino(tabelaFreteCliente, dadosParaDuplicar, false);

            return (from obj in tabelaFreteCliente.CEPsOrigem
                    select new
                    {
                        Codigo = dadosParaDuplicar ? Guid.NewGuid().ToString() : obj.Codigo.ToString(),
                        CEPInicial = string.Format(@"{0:00\.000\-000}", obj.CEPInicial),
                        CEPFinal = string.Format(@"{0:00\.000\-000}", obj.CEPFinal)
                    }).ToList();
        }

        private dynamic ObterCEPSDestino(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, bool dadosParaDuplicar, bool duplicarComoRetorno)
        {
            if (duplicarComoRetorno)
                return ObterCEPSOrigem(tabelaFreteCliente, dadosParaDuplicar, false);

            return (from obj in tabelaFreteCliente.CEPsDestino
                    select new
                    {
                        Codigo = dadosParaDuplicar ? Guid.NewGuid().ToString() : obj.Codigo.ToString(),
                        CEPInicial = string.Format(@"{0:00\.000\-000}", obj.CEPInicial),
                        CEPFinal = string.Format(@"{0:00\.000\-000}", obj.CEPFinal),
                        obj.DiasUteis
                    }).ToList();
        }

        private dynamic ObterOrigens(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, bool duplicarComoRetorno)
        {
            if (duplicarComoRetorno)
                return ObterDestinos(tabelaFreteCliente, false);

            return (from obj in tabelaFreteCliente.Origens
                    select new
                    {
                        obj.Codigo,
                        Descricao = obj.DescricaoCidadeEstado
                    }).ToList();
        }

        private dynamic ObterDestinos(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, bool duplicarComoRetorno)
        {
            if (duplicarComoRetorno)
                return ObterOrigens(tabelaFreteCliente, false);

            return (from obj in tabelaFreteCliente.Destinos
                    select new
                    {
                        obj.Codigo,
                        Descricao = obj.DescricaoCidadeEstado
                    }).ToList();
        }

        private dynamic ObterClientesOrigem(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, bool duplicarComoRetorno)
        {
            if (duplicarComoRetorno)
                return ObterClientesDestino(tabelaFreteCliente, false);

            return (from obj in tabelaFreteCliente.ClientesOrigem
                    select new
                    {
                        obj.Codigo,
                        obj.Descricao
                    }).ToList();
        }

        private dynamic ObterClientesDestino(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, bool duplicarComoRetorno)
        {
            if (duplicarComoRetorno)
                return ObterClientesOrigem(tabelaFreteCliente, false);

            return (from obj in tabelaFreteCliente.ClientesDestino
                    select new
                    {
                        obj.Codigo,
                        obj.Descricao
                    }).ToList();
        }

        private dynamic ObterPaisesOrigem(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, bool duplicarComoRetorno)
        {
            if (duplicarComoRetorno)
                return ObterPaisesDestino(tabelaFreteCliente, false);

            return (from obj in tabelaFreteCliente.PaisesOrigem
                    select new
                    {
                        obj.Codigo,
                        obj.Descricao
                    }).ToList();
        }

        private dynamic ObterPaisesDestino(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, bool duplicarComoRetorno)
        {
            if (duplicarComoRetorno)
                return ObterPaisesOrigem(tabelaFreteCliente, false);

            return (from obj in tabelaFreteCliente.PaisesDestino
                    select new
                    {
                        obj.Codigo,
                        obj.Descricao
                    }).ToList();
        }

        private dynamic ObterEstadosOrigem(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, bool duplicarComoRetorno)
        {
            if (duplicarComoRetorno)
                return ObterEstadosDestino(tabelaFreteCliente, false);

            return (from obj in tabelaFreteCliente.EstadosOrigem
                    select new
                    {
                        Codigo = obj.Sigla,
                        Descricao = obj.Nome
                    }).ToList();
        }

        private dynamic ObterEstadosDestino(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, bool duplicarComoRetorno)
        {
            if (duplicarComoRetorno)
                return ObterEstadosOrigem(tabelaFreteCliente, false);

            return (from obj in tabelaFreteCliente.EstadosDestino
                    select new
                    {
                        Codigo = obj.Sigla,
                        Descricao = obj.Nome
                    }).ToList();
        }

        private dynamic ObterRegioesOrigem(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, bool duplicarComoRetorno)
        {
            if (duplicarComoRetorno)
                return ObterRegioesDestino(tabelaFreteCliente, false);

            return (from obj in tabelaFreteCliente.RegioesOrigem
                    select new
                    {
                        Codigo = obj.Codigo,
                        obj.Descricao
                    }).ToList();
        }

        private dynamic ObterRegioesDestino(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, bool duplicarComoRetorno)
        {
            if (duplicarComoRetorno)
                return ObterRegioesOrigem(tabelaFreteCliente, false);

            return (from obj in tabelaFreteCliente.RegioesDestino
                    select new
                    {
                        Codigo = obj.Codigo,
                        obj.Descricao
                    }).ToList();
        }

        private dynamic ObterRotasOrigem(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, bool duplicarComoRetorno)
        {
            if (duplicarComoRetorno)
                return ObterRotasDestino(tabelaFreteCliente, false);

            return (from obj in tabelaFreteCliente.RotasOrigem
                    select new
                    {
                        Codigo = obj.Codigo,
                        obj.Descricao
                    }).ToList();
        }

        private dynamic ObterRotasDestino(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, bool duplicarComoRetorno)
        {
            if (duplicarComoRetorno)
                return ObterRotasOrigem(tabelaFreteCliente, false);

            return (from obj in tabelaFreteCliente.RotasDestino
                    select new
                    {
                        Codigo = obj.Codigo,
                        obj.Descricao
                    }).ToList();
        }

        private dynamic ObterValoresItensBaseDeCalculoSemParametroBase(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, bool ajusteAguardandoRetorno)
        {
            List<TabelaFreteClienteItemParametroBaseCalculo> valoresItensBaseCalculo = new List<TabelaFreteClienteItemParametroBaseCalculo>();

            return valoresItensBaseCalculo = tabelaFreteCliente.ItensBaseCalculo.Select(obj => new TabelaFreteClienteItemParametroBaseCalculo()
            {
                Codigo = obj.CodigoObjeto,
                CodigoItem = 0,
                TipoValor = obj.TipoValor,
                TipoObjeto = obj.TipoObjeto,
                Valor = (ajusteAguardandoRetorno && obj.Situacao.IsAguardandoIntegracao())
                        ? obj.ValorOriginal
                        : obj.Valor
            }).ToList();
        }

        private dynamic ObterValoresItensBaseDeCalculoComParametroBase(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, bool ajusteAguardandoRetorno)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete repositorioItemParametro = new Repositorio.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete(unitOfWork);
            List<int> codigosItens = repositorioItemParametro.BuscarPorCodigosItensPorTabelaFrete(tabelaFreteCliente.Codigo);
            List<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete> itens = repositorioItemParametro.BuscarItensComParametroPorCodigos(codigosItens, tabelaFreteCliente.Codigo);
            List<TabelaFreteClienteItemParametroBaseCalculo> valoresItensBaseCalculo = new List<TabelaFreteClienteItemParametroBaseCalculo>();

            return valoresItensBaseCalculo = itens.Select(item => new TabelaFreteClienteItemParametroBaseCalculo()
            {
                Codigo = item.CodigoObjeto,
                CodigoItem = item.ParametroBaseCalculo.CodigoObjeto,
                TipoValor = item.TipoValor,
                TipoObjeto = item.TipoObjeto,
                Valor = (ajusteAguardandoRetorno && item.Situacao.IsAguardandoIntegracao())
                        ? item.ValorOriginal
                        : item.Valor
            }).ToList();
        }

        private dynamic ObterDetalhesTabelaFreteCliente(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, bool dadosParaDuplicar, bool duplicarComoRetorno, bool permiteLeadTime, bool ajusteAguardandoRetorno, List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteFrequenciaEntrega> restricoesEntrega, List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteModeloVeicularCarga> modelosVeicularesCarga)
        {
            dynamic observacoes = null;
            dynamic valoresMinimosGarantidos = null;
            dynamic valoresMaximos = null;
            dynamic valoresBases = null;
            dynamic valoresExcedentes = new List<object>();
            dynamic percentuaisPagamentoAgregados = new List<object>();

            if (tabelaFreteCliente.TabelaFrete.ParametroBase == null)
            {
                valoresMinimosGarantidos = new List<object>()
                {
                    new {
                            CodigoItem = 0,
                            ValorMinimoGarantido = tabelaFreteCliente.ValorMinimoGarantido
                    }
                };

                valoresMaximos = new List<object>()
                {
                    new {
                            CodigoItem = 0,
                            tabelaFreteCliente.ValorMaximo
                    }
                };

                valoresBases = new List<object>()
                {
                    new {
                            CodigoItem = 0,
                            tabelaFreteCliente.ValorBase
                    }
                };

                observacoes = new List<object>()
                {
                    new {
                        CodigoItem = 0,
                        Observacao = tabelaFreteCliente.Observacao,
                        ObservacaoTerceiro = tabelaFreteCliente.ObservacaoTerceiro,
                        ImprimirObservacaoCTe = tabelaFreteCliente.ImprimirObservacaoCTe
                    }
                };

                if (tabelaFreteCliente.TabelaFrete.PermiteValorAdicionalPorEntregaExcedente && tabelaFreteCliente.TabelaFrete.NumeroEntregas.Count > 0)
                {
                    valoresExcedentes.Add(new
                    {
                        CodigoItem = 0,
                        Valor = tabelaFreteCliente.ValorEntregaExcedente,
                        Tipo = "EntregaExcedente"
                    });
                }

                if (tabelaFreteCliente.TabelaFrete.PermiteValorAdicionalPorPalletExcedente && tabelaFreteCliente.TabelaFrete.Pallets.Count > 0)
                {
                    valoresExcedentes.Add(new
                    {
                        CodigoItem = 0,
                        Valor = tabelaFreteCliente.ValorPalletExcedente,
                        Tipo = "PalletExcedente"
                    });
                }

                if (tabelaFreteCliente.TabelaFrete.PermiteValorAdicionalPorPesoExcedente && tabelaFreteCliente.TabelaFrete.PesoExcecente > 0m && tabelaFreteCliente.TabelaFrete.PesosTransportados.Count > 0)
                {
                    valoresExcedentes.Add(new
                    {
                        CodigoItem = 0,
                        Valor = tabelaFreteCliente.ValorPesoExcedente,
                        Tipo = "PesoExcedente"
                    });
                }

                if (tabelaFreteCliente.TabelaFrete.PermiteValorAdicionalPorQuilometragemExcedente && tabelaFreteCliente.TabelaFrete.QuilometragemExcedente > 0m && tabelaFreteCliente.TabelaFrete.Distancias.Count > 0)
                {
                    valoresExcedentes.Add(new
                    {
                        CodigoItem = 0,
                        Valor = tabelaFreteCliente.ValorQuilometragemExcedente,
                        Tipo = "QuilometragemExcedente"
                    });
                }

                if (tabelaFreteCliente.TabelaFrete.PermiteValorAdicionalPorAjudanteExcedente && tabelaFreteCliente.TabelaFrete.Ajudantes.Count > 0)
                {
                    valoresExcedentes.Add(new
                    {
                        CodigoItem = 0,
                        Valor = tabelaFreteCliente.ValorAjudanteExcedente,
                        Tipo = "AjudanteExcedente"
                    });
                }

                if (tabelaFreteCliente.TabelaFrete.PermiteValorAdicionalPorHoraExcedente && tabelaFreteCliente.TabelaFrete.Horas.Count > 0)
                {
                    valoresExcedentes.Add(new
                    {
                        CodigoItem = 0,
                        Valor = tabelaFreteCliente.ValorHoraExcedente,
                        Tipo = "HoraExcedente"
                    });
                }

                if (tabelaFreteCliente.TabelaFrete.PermiteValorAdicionalPorPacoteExcedente && tabelaFreteCliente.TabelaFrete.Pacotes.Count > 0)
                {
                    valoresExcedentes.Add(new
                    {
                        CodigoItem = 0,
                        Valor = tabelaFreteCliente.ValorPacoteExcedente,
                        Tipo = "PacoteExcedente"
                    });
                }

                percentuaisPagamentoAgregados.Add(new
                {
                    CodigoItem = 0,
                    Valor = tabelaFreteCliente.PercentualPagamentoAgregado,
                    Tipo = "PagamentoAgregado"
                });
            }
            else
            {
                observacoes = (from obj in tabelaFreteCliente.ParametrosBaseCalculo
                               select new
                               {
                                   CodigoItem = obj.CodigoObjeto,
                                   Observacao = obj.Observacao,
                                   ObservacaoTerceiro = obj.ObservacaoTerceiro,
                                   ImprimirObservacaoCTe = obj.ImprimirObservacaoCTe
                               }).ToList();

                valoresMinimosGarantidos = (from obj in tabelaFreteCliente.ParametrosBaseCalculo
                                            select new
                                            {
                                                CodigoItem = obj.CodigoObjeto,
                                                obj.ValorMinimoGarantido
                                            }).ToList();

                valoresMaximos = (from obj in tabelaFreteCliente.ParametrosBaseCalculo
                                  select new
                                  {
                                      CodigoItem = obj.CodigoObjeto,
                                      obj.ValorMaximo
                                  }).ToList();

                valoresBases = (from obj in tabelaFreteCliente.ParametrosBaseCalculo
                                select new
                                {
                                    CodigoItem = obj.CodigoObjeto,
                                    obj.ValorBase
                                }).ToList();

                if (tabelaFreteCliente.TabelaFrete.PermiteValorAdicionalPorEntregaExcedente && tabelaFreteCliente.TabelaFrete.NumeroEntregas.Count > 0)
                {
                    valoresExcedentes.AddRange(from obj in tabelaFreteCliente.ParametrosBaseCalculo
                                               select new
                                               {
                                                   CodigoItem = obj.CodigoObjeto,
                                                   Valor = obj.ValorEntregaExcedente,
                                                   Tipo = "EntregaExcedente"
                                               });
                }

                if (tabelaFreteCliente.TabelaFrete.PermiteValorAdicionalPorPalletExcedente && tabelaFreteCliente.TabelaFrete.Pallets.Count > 0)
                {
                    valoresExcedentes.AddRange(from obj in tabelaFreteCliente.ParametrosBaseCalculo
                                               select new
                                               {
                                                   CodigoItem = obj.CodigoObjeto,
                                                   Valor = obj.ValorPalletExcedente,
                                                   Tipo = "PalletExcedente"
                                               });
                }

                if (tabelaFreteCliente.TabelaFrete.PermiteValorAdicionalPorPesoExcedente && tabelaFreteCliente.TabelaFrete.PesoExcecente > 0m && tabelaFreteCliente.TabelaFrete.PesosTransportados.Count > 0)
                {
                    valoresExcedentes.AddRange(from obj in tabelaFreteCliente.ParametrosBaseCalculo
                                               select new
                                               {
                                                   CodigoItem = obj.CodigoObjeto,
                                                   Valor = obj.ValorPesoExcedente,
                                                   Tipo = "PesoExcedente"
                                               });
                }

                if (tabelaFreteCliente.TabelaFrete.PermiteValorAdicionalPorQuilometragemExcedente && tabelaFreteCliente.TabelaFrete.QuilometragemExcedente > 0m && tabelaFreteCliente.TabelaFrete.Distancias.Count > 0)
                {
                    valoresExcedentes.AddRange(from obj in tabelaFreteCliente.ParametrosBaseCalculo
                                               select new
                                               {
                                                   CodigoItem = obj.CodigoObjeto,
                                                   Valor = obj.ValorQuilometragemExcedente,
                                                   Tipo = "QuilometragemExcedente"
                                               });
                }

                if (tabelaFreteCliente.TabelaFrete.PermiteValorAdicionalPorAjudanteExcedente && tabelaFreteCliente.TabelaFrete.Ajudantes.Count > 0)
                {
                    valoresExcedentes.AddRange(from obj in tabelaFreteCliente.ParametrosBaseCalculo
                                               select new
                                               {
                                                   CodigoItem = obj.CodigoObjeto,
                                                   Valor = obj.ValorAjudanteExcedente,
                                                   Tipo = "AjudanteExcedente"
                                               });
                }

                if (tabelaFreteCliente.TabelaFrete.PermiteValorAdicionalPorHoraExcedente && tabelaFreteCliente.TabelaFrete.Horas.Count > 0)
                {
                    valoresExcedentes.AddRange(from obj in tabelaFreteCliente.ParametrosBaseCalculo
                                               select new
                                               {
                                                   CodigoItem = obj.CodigoObjeto,
                                                   Valor = obj.ValorHoraExcedente,
                                                   Tipo = "HoraExcedente"
                                               });
                }

                if (tabelaFreteCliente.TabelaFrete.PermiteValorAdicionalPorPacoteExcedente && tabelaFreteCliente.TabelaFrete.Pacotes.Count > 0)
                {
                    valoresExcedentes.AddRange(from obj in tabelaFreteCliente.ParametrosBaseCalculo
                                               select new
                                               {
                                                   CodigoItem = obj.CodigoObjeto,
                                                   Valor = obj.ValorPacoteExcedente,
                                                   Tipo = "PacoteExcedente"
                                               });
                }

                percentuaisPagamentoAgregados.AddRange(
                    from obj in tabelaFreteCliente.ParametrosBaseCalculo
                    select new
                    {
                        CodigoItem = obj.CodigoObjeto,
                        Valor = obj.PercentualPagamentoAgregado,
                        Tipo = "PagamentoAgregado"
                    });
            }

            List<TabelaFreteClienteItemParametroBaseCalculo> valoresItens = tabelaFreteCliente.TabelaFrete.ParametroBase == null
                ? ObterValoresItensBaseDeCalculoSemParametroBase(tabelaFreteCliente, ajusteAguardandoRetorno)
                : ObterValoresItensBaseDeCalculoComParametroBase(tabelaFreteCliente, ajusteAguardandoRetorno);

            var retorno = new
            {
                tabelaFreteCliente.PrioridadeUso,
                tabelaFreteCliente.Moeda,
                Ativo = dadosParaDuplicar ? true : tabelaFreteCliente.Ativo,
                Codigo = dadosParaDuplicar ? 0 : tabelaFreteCliente.Codigo,
                tabelaFreteCliente.CodigoIntegracao,
                tabelaFreteCliente.Quilometragem,
                tabelaFreteCliente.TabelaFrete.NaoPermitirLancarValorPorTipoDeCarga,
                PercentualCobrancaPadrao = tabelaFreteCliente.PercentualCobrancaPadraoTerceiros > 0 ? tabelaFreteCliente.PercentualCobrancaPadraoTerceiros.ToString("n2") : "0,00",
                PercentualCobrancaVeiculoFrota = tabelaFreteCliente.PercentualCobrancaVeiculoFrota > 0 ? tabelaFreteCliente.PercentualCobrancaVeiculoFrota.ToString("n2") : "0,00",
                tabelaFreteCliente.TipoRateioDocumentos,
                Tomador = new
                {
                    Codigo = tabelaFreteCliente.Tomador?.CPF_CNPJ ?? 0D,
                    Descricao = tabelaFreteCliente.Tomador?.Nome ?? string.Empty
                },
                TabelaFrete = new
                {
                    tabelaFreteCliente.TabelaFrete.Codigo,
                    tabelaFreteCliente.TabelaFrete.Descricao
                },
                Vigencia = new
                {
                    Codigo = tabelaFreteCliente.Vigencia?.Codigo ?? 0,
                    Descricao = tabelaFreteCliente.Vigencia != null ? tabelaFreteCliente.Vigencia.Descricao : string.Empty
                },
                FormulaRateio = new
                {
                    Codigo = tabelaFreteCliente.FormulaRateio?.Codigo ?? 0,
                    Descricao = tabelaFreteCliente.FormulaRateio?.Descricao ?? string.Empty
                },
                Empresa = new
                {
                    Codigo = tabelaFreteCliente.Empresa?.Codigo ?? 0,
                    Descricao = tabelaFreteCliente.Empresa?.Descricao ?? string.Empty
                },
                tabelaFreteCliente.IncluirICMSValorFrete,
                tabelaFreteCliente.PercentualICMSIncluir,
                tabelaFreteCliente.HerdarInclusaoICMSTabelaFrete,
                tabelaFreteCliente.ObrigatorioInformarValePedagioCarga,
                tabelaFreteCliente.FreteValidoParaQualquerDestino,
                tabelaFreteCliente.FreteValidoParaQualquerOrigem,
                tabelaFreteCliente.TipoPagamento,
                tabelaFreteCliente.TipoGrupoCarga,
                tabelaFreteCliente.GerenciarCapacidade,
                tabelaFreteCliente.EstruturaTabela,
                tabelaFreteCliente.LeadTime,
                tabelaFreteCliente.LeadTimeMinutos,
                tabelaFreteCliente.LeadTimeTransportador,
                tabelaFreteCliente.TipoLeadTime,
                tabelaFreteCliente.ObservacaoInterna,
                PercentualRota = tabelaFreteCliente.PercentualRota != null ? tabelaFreteCliente.PercentualRota.Value.ToString("n2") : null,
                QuantidadeEntregas = tabelaFreteCliente.QuantidadeEntregas != null ? tabelaFreteCliente.QuantidadeEntregas : null,
                RestricaoEntrega = restricoesEntrega != null ? restricoesEntrega.Select(o => o.DiaSemana).ToList() : null,
                TransportadoresTerceiros = (
                from obj in tabelaFreteCliente.TransportadoresTerceiros
                select new
                {
                    obj.Codigo,
                    Descricao = obj.Descricao
                }
            ).ToList(),
                TiposOperacao = (
                from obj in tabelaFreteCliente.TiposOperacao
                select new
                {
                    obj.Codigo,
                    Descricao = obj.Descricao
                }
            ).ToList(),
                TiposCarga = (
                from obj in tabelaFreteCliente.TiposCarga
                select new
                {
                    obj.Codigo,
                    Descricao = obj.Descricao
                }
            ).ToList(),
                Origens = ObterOrigens(tabelaFreteCliente, duplicarComoRetorno),
                Destinos = ObterDestinos(tabelaFreteCliente, duplicarComoRetorno),
                EstadosOrigem = ObterEstadosOrigem(tabelaFreteCliente, duplicarComoRetorno),
                EstadosDestino = ObterEstadosDestino(tabelaFreteCliente, duplicarComoRetorno),
                RegioesOrigem = ObterRegioesOrigem(tabelaFreteCliente, duplicarComoRetorno),
                RotasOrigem = ObterRotasOrigem(tabelaFreteCliente, duplicarComoRetorno),
                PaisesOrigem = ObterPaisesOrigem(tabelaFreteCliente, duplicarComoRetorno),
                RegioesDestino = ObterRegioesDestino(tabelaFreteCliente, duplicarComoRetorno),
                RotasDestino = ObterRotasDestino(tabelaFreteCliente, duplicarComoRetorno),
                PaisesDestino = ObterPaisesDestino(tabelaFreteCliente, duplicarComoRetorno),
                ClientesOrigem = ObterClientesOrigem(tabelaFreteCliente, duplicarComoRetorno),
                ClientesDestino = ObterClientesDestino(tabelaFreteCliente, duplicarComoRetorno),
                CEPsOrigem = ObterCEPSOrigem(tabelaFreteCliente, dadosParaDuplicar, duplicarComoRetorno),
                CEPsDestino = ObterCEPSDestino(tabelaFreteCliente, dadosParaDuplicar, duplicarComoRetorno),
                Fronteiras = tabelaFreteCliente.Fronteiras.Select(o => new { Descricao = o.Descricao, Codigo = o.Codigo }).ToList(),
                Valores = valoresItens,
                Observacoes = observacoes,
                ValoresMinimosGarantidos = valoresMinimosGarantidos,
                ValoresMaximos = valoresMaximos,
                ValoresBases = valoresBases,
                ValoresExcedentes = valoresExcedentes,
                PercentuaisPagamentoAgregados = percentuaisPagamentoAgregados,
                Subcontratacoes = (
                from obj in tabelaFreteCliente.Subcontratacoes
                select new
                {
                    Codigo = dadosParaDuplicar ? Guid.NewGuid().ToString() : obj.Codigo.ToString(),
                    PercentualDesconto = new { val = obj.PercentualDesconto, tipo = "decimal" },
                    ValorFixoSubContratacaoParcial = new { val = obj.ValorFixoSubContratacaoParcial, tipo = "decimal" },
                    Pessoa = new
                    {
                        Codigo = obj.Pessoa.CPF_CNPJ,
                        Descricao = obj.Pessoa.Descricao
                    },
                    Valores = (
                        from val in obj.Valores
                        select new
                        {
                            Codigo = dadosParaDuplicar ? Guid.NewGuid().ToString() : val.Codigo.ToString(),
                            Justificativa = new
                            {
                                val.Justificativa.Codigo,
                                val.Justificativa.Descricao
                            },
                            Valor = new { val = val.Valor, tipo = "decimal" }
                        }
                    ).ToList()
                }
            ).ToList(),
                SubcontratacoesGeral = (
                from obj in tabelaFreteCliente.SubcontratacoesGerais
                select new
                {
                    Codigo = dadosParaDuplicar ? Guid.NewGuid().ToString() : obj.Codigo.ToString(),
                    Valores = (
                        from val in tabelaFreteCliente.SubcontratacoesGerais
                        select new
                        {
                            Codigo = dadosParaDuplicar ? Guid.NewGuid().ToString() : val.Codigo.ToString(),
                            Justificativa = new
                            {
                                val.Justificativa.Codigo,
                                val.Justificativa.Descricao
                            },
                            Valor = new { val = val.Valor, tipo = "decimal" }
                        }
                    ).ToList()
                }
            ).ToList(),
                RotaFrete = new
                {
                    Codigo = tabelaFreteCliente.RotaFrete?.Codigo ?? 0,
                    Descricao = tabelaFreteCliente.RotaFrete?.Descricao ?? string.Empty
                },
                PermiteLeadTime = permiteLeadTime,
                CanalEntrega = new
                {
                    Codigo = tabelaFreteCliente.CanalEntrega?.Codigo ?? 0,
                    Descricao = tabelaFreteCliente.CanalEntrega?.Descricao ?? string.Empty
                },
                ContratoTransporteFrete = new
                {
                    Codigo = tabelaFreteCliente.ContratoTransporteFrete?.Codigo ?? 0,
                    Descricao = tabelaFreteCliente.ContratoTransporteFrete?.Descricao ?? string.Empty
                },
                CanalVenda = new
                {
                    Codigo = tabelaFreteCliente.CanalVenda?.Codigo ?? 0,
                    Descricao = tabelaFreteCliente.CanalVenda?.Descricao ?? string.Empty
                },
                DominioOTM = tabelaFreteCliente.DominioOTM,
                CapacidadeOTM = tabelaFreteCliente.CapacidadeOTM,
                PontoPlanejamentoTransporte = tabelaFreteCliente.PontoPlanejamentoTransporte,
                TipoIntegracao = tabelaFreteCliente?.TipoIntegracao,
                IDExterno = tabelaFreteCliente?.IDExterno ?? string.Empty,
                StatusAceiteTabela = tabelaFreteCliente?.StatusAceiteTabela ?? string.Empty,
                ModelosVeicularesCarga = ObterModelosVeicularesTabelaFreteCliente(modelosVeicularesCarga, tabelaFreteCliente.TabelaFrete.ModelosReboque.ToList())
            };

            return retorno;
        }

        private dynamic ObterModelosVeicularesTabelaFreteCliente(List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteModeloVeicularCarga> modelosVeicularesCarga, List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modeloVeicularCargaTabelaFrete)
        {
            List<dynamic> retornoModelos = new List<dynamic>();
            List<int> codigosModelos = modelosVeicularesCarga.Count > 0 ? modelosVeicularesCarga.Select(x => x.ModeloVeicularCarga.Codigo).ToList() : new List<int>();

            List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosNaoSalvos = modeloVeicularCargaTabelaFrete.Where(x => !codigosModelos.Contains(x.Codigo)).ToList();

            foreach (var modeloVeicularCarga in modelosNaoSalvos)
                retornoModelos.Add(new
                {
                    Codigo = 0,
                    CodigoModeloVeicularCarga = modeloVeicularCarga.Codigo,
                    CapacidadeOTM = "",
                    ModeloVeicularCarga = modeloVeicularCarga.Descricao,
                    PercentualRota = "",
                    QuantidadeEntregas = "",
                    DescricaoCapacidadeOTM = "",
                    DT_Enable = true
                });

            foreach (var modeloVeicularCarga in modelosVeicularesCarga)
                retornoModelos.Add(new
                {
                    Codigo = modeloVeicularCarga.Codigo,
                    CodigoModeloVeicularCarga = modeloVeicularCarga.ModeloVeicularCarga.Codigo,
                    modeloVeicularCarga.CapacidadeOTM,
                    ModeloVeicularCarga = modeloVeicularCarga.ModeloVeicularCarga.Descricao,
                    PercentualRota = modeloVeicularCarga.PercentualRota.ToString("n2"),
                    QuantidadeEntregas = (modeloVeicularCarga.QuantidadeEntregas > 0) ? modeloVeicularCarga.QuantidadeEntregas.ToString("n0") : "",
                    DescricaoCapacidadeOTM = modeloVeicularCarga.CapacidadeOTM.ObterDescricao(),
                    DT_Enable = true
                });

            return retornoModelos;
        }

        private string ObterSituacaoAjusteTabelaFrete(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> tabelasClienteAjuste)
        {
            return (from obj in tabelasClienteAjuste where obj.TabelaOriginaria.Codigo == tabelaFreteCliente.Codigo select obj).FirstOrDefault()?.AjusteTabelaFrete?.Situacao?.ObterDescricao() ?? "";
        }

        private void SalvarSubcontratacoes(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.TabelaFreteClienteSubContratacao repTabelaFreteClienteSubContratacao = new Repositorio.Embarcador.Frete.TabelaFreteClienteSubContratacao(unitOfWork);

            Repositorio.Cliente repPessoa = new Repositorio.Cliente(unitOfWork);

            dynamic subcontratacoes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Subcontratacoes"));

            dynamic subcontratacoesGerais = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("SubcontratacoesGeral"));


            if (tabelaFreteCliente.Subcontratacoes != null && tabelaFreteCliente.Subcontratacoes.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var subcontratacao in subcontratacoes)
                    codigos.Add((int)subcontratacao.Codigo);

                List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteSubContratacao> subcontratacoesDeletar = (from obj in tabelaFreteCliente.Subcontratacoes where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < subcontratacoesDeletar.Count; i++)
                    repTabelaFreteClienteSubContratacao.Deletar(subcontratacoesDeletar[i]);
            }

            foreach (var subcontratacao in subcontratacoes)
            {
                Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteSubContratacao sub = null;

                int codigo = 0;

                if (subcontratacao.Codigo != null && int.TryParse((string)subcontratacao.Codigo, out codigo))
                    sub = repTabelaFreteClienteSubContratacao.BuscarPorCodigo(codigo);

                if (sub == null)
                    sub = new Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteSubContratacao();

                sub.TabelaFreteCliente = tabelaFreteCliente;
                sub.Pessoa = repPessoa.BuscarPorCPFCNPJ((double)subcontratacao.Pessoa.Codigo);
                sub.PercentualDesconto = (decimal)subcontratacao.PercentualDesconto;
                sub.ValorFixoSubContratacaoParcial = (decimal)subcontratacao.ValorFixoSubContratacaoParcial;

                if (sub.Codigo > 0)
                    repTabelaFreteClienteSubContratacao.Atualizar(sub);
                else
                    repTabelaFreteClienteSubContratacao.Inserir(sub);

                SalvarValoresSubcontratacao(sub, subcontratacao.Valores, unitOfWork);
            }

            if (subcontratacoesGerais != null)
            {
                foreach (var subcontracaoGeral in subcontratacoesGerais)
                {
                    SalvarValoresSubcontracaoGeral(tabelaFreteCliente, subcontracaoGeral.Valores, unitOfWork);
                }

            }
        }

        private void SalvarValoresSubcontratacao(Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteSubContratacao sub, dynamic valores, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Frete.TabelaFreteClienteSubContratacaoAcrescimoDesconto repTabelaFreteClienteSubContratacaoAcrescimoDesconto = new Repositorio.Embarcador.Frete.TabelaFreteClienteSubContratacaoAcrescimoDesconto(unidadeTrabalho);
            Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unidadeTrabalho);

            if (sub.Valores != null && sub.Valores.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var valor in valores)
                    if (valor.Codigo != null)
                        codigos.Add((int)valor.Codigo);

                List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteSubContratacaoAcrescimoDesconto> valoresDeletar = (from obj in sub.Valores where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < valoresDeletar.Count; i++)
                    repTabelaFreteClienteSubContratacaoAcrescimoDesconto.Deletar(valoresDeletar[i]);
            }

            foreach (var valor in valores)
            {
                Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteSubContratacaoAcrescimoDesconto val = null;

                int codigo = 0;

                if (valor.Codigo != null && int.TryParse((string)valor.Codigo, out codigo))
                    val = repTabelaFreteClienteSubContratacaoAcrescimoDesconto.BuscarPorCodigo(codigo, false);

                if (val == null)
                    val = new Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteSubContratacaoAcrescimoDesconto();

                val.TabelaFreteClienteSubContratacao = sub;
                val.Justificativa = repJustificativa.BuscarPorCodigo((int)valor.Justificativa.Codigo);
                val.Valor = (decimal)valor.Valor;

                if (val.Codigo > 0)
                    repTabelaFreteClienteSubContratacaoAcrescimoDesconto.Atualizar(val);
                else
                    repTabelaFreteClienteSubContratacaoAcrescimoDesconto.Inserir(val);
            }
        }

        private void SalvarValoresSubcontracaoGeral(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, dynamic valores, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Frete.TabelaFreteClienteSubContratacaoAcrescimoDescontoGeral repTabelaFreteClienteSubContratacaoAcrescimoDescontoGeral = new Repositorio.Embarcador.Frete.TabelaFreteClienteSubContratacaoAcrescimoDescontoGeral(unidadeTrabalho);
            Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unidadeTrabalho);

            if (valores != null && valores.Count > 0)
            {
                List<int> codigos = new List<int>();
                List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteSubContratacaoAcrescimoDescontoGeral> Valores = new List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteSubContratacaoAcrescimoDescontoGeral>();

                foreach (var valor in valores)
                {
                    if (valor.Codigo != null)
                    {
                        codigos.Add((int)valor.Codigo);
                        Valores.Add(repTabelaFreteClienteSubContratacaoAcrescimoDescontoGeral.BuscarPorCodigo((int)valor.Codigo, false));
                    }

                }

                List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteSubContratacaoAcrescimoDescontoGeral> valoresDeletar = (from obj in Valores where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < valoresDeletar.Count; i++)
                    repTabelaFreteClienteSubContratacaoAcrescimoDescontoGeral.Deletar(valoresDeletar[i]);
            }
            else if (valores.Count == 0)
            {
                var valoresDeletar = repTabelaFreteClienteSubContratacaoAcrescimoDescontoGeral.BuscarPorTabelaFrete(tabelaFreteCliente.Codigo);

                for (var i = 0; i < valoresDeletar.Count; i++)
                    repTabelaFreteClienteSubContratacaoAcrescimoDescontoGeral.Deletar(valoresDeletar[i]);
            }

            foreach (var valor in valores)
            {
                Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteSubContratacaoAcrescimoDescontoGeral val = null;

                int codigo = 0;

                if (valor.Codigo != null && int.TryParse((string)valor.Codigo, out codigo))
                    val = repTabelaFreteClienteSubContratacaoAcrescimoDescontoGeral.BuscarPorCodigo(codigo, false);

                if (val == null)
                    val = new Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteSubContratacaoAcrescimoDescontoGeral();

                val.Valor = valor.Valor;
                val.TabelaFreteCliente = tabelaFreteCliente;
                val.Justificativa = repJustificativa.BuscarPorCodigo((int)valor.Justificativa.Codigo);

                if (val.Codigo > 0)
                    repTabelaFreteClienteSubContratacaoAcrescimoDescontoGeral.Atualizar(val);
                else
                    repTabelaFreteClienteSubContratacaoAcrescimoDescontoGeral.Inserir(val);
            }
        }

        private void SalvarModelosVeicularesCarga(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, Repositorio.UnitOfWork unitOfWork, bool inclusao)
        {
            Repositorio.Embarcador.Frete.TabelaFreteClienteModeloVeicularCarga repositorioTabelaFreteClienteModeloVeicularCarga = new Repositorio.Embarcador.Frete.TabelaFreteClienteModeloVeicularCarga(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
            dynamic modelosVeicularesCarga = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(Request.Params("ModelosVeicularesCarga"));
            List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade> valoresAlterados = new List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade>();
            Repositorio.Embarcador.Cargas.TipoIntegracao reTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao integracaoLBC = reTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.LBC);


            foreach (var modeloVeicularCarga in modelosVeicularesCarga)
            {
                int codigo = ((string)modeloVeicularCarga.Codigo).ToInt();
                Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteModeloVeicularCarga modeloVeicularCargaSalvar = null;

                if (codigo > 0 && !inclusao)
                    modeloVeicularCargaSalvar = repositorioTabelaFreteClienteModeloVeicularCarga.BuscarPorCodigo(codigo, true);

                decimal? percentualRota = ((string)modeloVeicularCarga.PercentualRota).ToNullableDecimal();
                int? quantidadeEntrega = ((string)modeloVeicularCarga.QuantidadeEntregas).ToNullableInt();
                bool? capacidadeOTM = ((string)modeloVeicularCarga.CapacidadeOTM).ToNullableBool();
                bool permitirAdicionar = (percentualRota.HasValue || percentualRota != 0) || quantidadeEntrega.HasValue || capacidadeOTM.HasValue;

                if (!permitirAdicionar)
                {

                    if (modeloVeicularCargaSalvar != null)
                    {
                        valoresAlterados.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                        {
                            De = modeloVeicularCarga.ModeloVeicularCarga.Descricao,
                            Para = "",
                            Propriedade = "ModelosVeicularesCarga"
                        });

                        repositorioTabelaFreteClienteModeloVeicularCarga.Deletar(modeloVeicularCarga);
                    }

                    continue;
                }

                if (modeloVeicularCargaSalvar == null)
                    modeloVeicularCargaSalvar = new Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteModeloVeicularCarga();

                modeloVeicularCargaSalvar.TabelaFreteCliente = tabelaFreteCliente;
                modeloVeicularCargaSalvar.ModeloVeicularCarga = repositorioModeloVeicularCarga.BuscarPorCodigo(((string)modeloVeicularCarga.CodigoModeloVeicularCarga).ToInt());
                modeloVeicularCargaSalvar.PercentualRota = ((string)modeloVeicularCarga.PercentualRota).ToDecimal();
                modeloVeicularCargaSalvar.QuantidadeEntregas = ((string)modeloVeicularCarga.QuantidadeEntregas).ToInt();
                modeloVeicularCargaSalvar.CapacidadeOTM = ((string)modeloVeicularCarga.CapacidadeOTM).ToBool();

                if (modeloVeicularCargaSalvar.Codigo > 0)
                {
                    List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade> alteracoes = modeloVeicularCargaSalvar.GetCurrentChanges();

                    foreach (Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade alteracao in alteracoes)
                        valoresAlterados.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                        {
                            De = alteracao.De,
                            Para = alteracao.Para,
                            Propriedade = $"ModelosVeicularesCarga.{alteracao.Propriedade} - {modeloVeicularCargaSalvar.ModeloVeicularCarga.Descricao}".Left(200)
                        });

                    modeloVeicularCargaSalvar.PendenteIntegracao = (alteracoes.Count > 0);

                    repositorioTabelaFreteClienteModeloVeicularCarga.Atualizar(modeloVeicularCargaSalvar);
                }
                else
                {
                    valoresAlterados.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                    {
                        De = "",
                        Para = modeloVeicularCargaSalvar.ModeloVeicularCarga.Descricao,
                        Propriedade = "ModelosVeicularesCarga"
                    });

                    modeloVeicularCargaSalvar.PendenteIntegracao = true;

                    repositorioTabelaFreteClienteModeloVeicularCarga.Inserir(modeloVeicularCargaSalvar);
                }
            }

            tabelaFreteCliente.SetExternalChanges(valoresAlterados);
        }

        private void SetarLocalidadesOrigem(ref string descricaoOrigem, ref Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFrete, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeTrabalho);

            dynamic origens = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Origens"));

            tabelaFrete.Origens = new List<Dominio.Entidades.Localidade>();

            foreach (var origem in origens)
            {
                Dominio.Entidades.Localidade localidade = repLocalidade.BuscarPorCodigo((int)origem.Codigo);

                tabelaFrete.Origens.Add(localidade);

                descricaoOrigem += " / " + localidade.DescricaoCidadeEstado;
            }
        }

        private void SetarEstadosOrigem(ref string descricaoOrigem, ref Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFrete, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Estado repEstado = new Repositorio.Estado(unidadeTrabalho);

            dynamic estadosOrigem = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("EstadosOrigem"));

            tabelaFrete.EstadosOrigem = new List<Dominio.Entidades.Estado>();

            foreach (var estadoOrigem in estadosOrigem)
            {
                Dominio.Entidades.Estado estado = repEstado.BuscarPorSigla((string)estadoOrigem.Codigo);

                tabelaFrete.EstadosOrigem.Add(estado);

                descricaoOrigem += " / " + estado.Nome;
            }
        }

        private void SetarPaisesOrigem(ref string descricaoOrigem, ref Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFrete, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Pais repositorioPais = new Repositorio.Pais(unidadeTrabalho);
            dynamic paisesOrigem = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("PaisesOrigem"));
            tabelaFrete.PaisesOrigem = new List<Dominio.Entidades.Pais>();

            foreach (var paisOrigem in paisesOrigem)
            {
                Dominio.Entidades.Pais pais = repositorioPais.BuscarPorCodigo((int)paisOrigem.Codigo);

                tabelaFrete.PaisesOrigem.Add(pais);

                descricaoOrigem += $" / {pais.Nome}";
            }
        }

        private void SetarRegioesOrigem(ref string descricaoOrigem, ref Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFrete, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Localidades.Regiao repRegiao = new Repositorio.Embarcador.Localidades.Regiao(unidadeTrabalho);

            dynamic regioesOrigem = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("RegioesOrigem"));

            tabelaFrete.RegioesOrigem = new List<Dominio.Entidades.Embarcador.Localidades.Regiao>();

            foreach (var regiaoOrigem in regioesOrigem)
            {
                Dominio.Entidades.Embarcador.Localidades.Regiao regiao = repRegiao.BuscarPorCodigo((int)regiaoOrigem.Codigo);

                tabelaFrete.RegioesOrigem.Add(regiao);

                descricaoOrigem += " / " + regiao.Descricao;
            }
        }

        private void SetarRotasOrigem(ref string descricaoOrigem, ref Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFrete, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.RotaFrete repRota = new Repositorio.RotaFrete(unidadeTrabalho);

            dynamic rotasOrigem = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("RotasOrigem"));

            tabelaFrete.RotasOrigem = new List<Dominio.Entidades.RotaFrete>();

            foreach (var rotaOrigem in rotasOrigem)
            {
                Dominio.Entidades.RotaFrete rota = repRota.BuscarPorCodigo((int)rotaOrigem.Codigo);

                tabelaFrete.RotasOrigem.Add(rota);

                descricaoOrigem += " / " + rota.Descricao;
            }
        }

        private void SetarClientesOrigem(ref string descricaoOrigem, ref Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFrete, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeTrabalho);

            dynamic clientesOrigem = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ClientesOrigem"));

            tabelaFrete.ClientesOrigem = new List<Dominio.Entidades.Cliente>();

            foreach (var clienteOrigem in clientesOrigem)
            {
                double.TryParse(Utilidades.String.OnlyNumbers((string)clienteOrigem.Codigo), out double cpfCnpjCliente);

                Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpjCliente);

                tabelaFrete.ClientesOrigem.Add(cliente);
            }

            if (tabelaFrete.ClientesOrigem.Count > 0)
            {
                List<Dominio.Entidades.Cliente> clientesPai = tabelaFrete.ClientesOrigem.Where(cliente => cliente.PossuiFilialCliente).ToList();

                descricaoOrigem = (clientesPai.Count > 0) ? string.Join(" / ", clientesPai.Select(cliente => cliente.Descricao)) : string.Join(" / ", tabelaFrete.ClientesOrigem.Select(cliente => cliente.Descricao));
            }
        }

        private void SalvarCEPsOrigem(ref string descricaoOrigem, Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFrete, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Frete.TabelaFreteClienteCEPOrigem repTabelaFreteClienteCEPOrigem = new Repositorio.Embarcador.Frete.TabelaFreteClienteCEPOrigem(unidadeTrabalho);

            dynamic cepsOrigem = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaCEPsOrigem"));

            if (tabelaFrete.CEPsOrigem != null && tabelaFrete.CEPsOrigem.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var cepOrigem in cepsOrigem)
                {
                    int codigo = 0;

                    if (int.TryParse((string)cepOrigem.Codigo, out codigo))
                        codigos.Add(codigo);
                }

                List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteCEPOrigem> cepsDeletar = (from obj in tabelaFrete.CEPsOrigem where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < cepsDeletar.Count; i++)
                    repTabelaFreteClienteCEPOrigem.Deletar(cepsDeletar[i]);
            }

            foreach (var cepOrigem in cepsOrigem)
            {
                int codigo = 0;

                Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteCEPOrigem cep = null;

                if (cepOrigem.Codigo != null && int.TryParse((string)cepOrigem.Codigo, out codigo))
                    cep = repTabelaFreteClienteCEPOrigem.BuscarPorCodigo(codigo, false);

                if (cep == null)
                    cep = new Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteCEPOrigem();

                string cepInicial = (string)cepOrigem.CEPInicial;
                string cepFinal = (string)cepOrigem.CEPFinal;

                cep.TabelaFreteCliente = tabelaFrete;
                cep.CEPInicial = int.Parse(Utilidades.String.OnlyNumbers(cepInicial));
                cep.CEPFinal = int.Parse(Utilidades.String.OnlyNumbers(cepFinal));

                if (cep.Codigo > 0)
                    repTabelaFreteClienteCEPOrigem.Atualizar(cep);
                else
                    repTabelaFreteClienteCEPOrigem.Inserir(cep);

                descricaoOrigem += " / " + cepInicial + " à " + cepFinal;
            }
        }

        private void SetarLocalidadesDestino(ref string descricaoDestino, ref Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFrete, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeTrabalho);

            dynamic destinos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Destinos"));

            tabelaFrete.Destinos = new List<Dominio.Entidades.Localidade>();

            foreach (var destino in destinos)
            {
                Dominio.Entidades.Localidade localidade = repLocalidade.BuscarPorCodigo((int)destino.Codigo);

                tabelaFrete.Destinos.Add(localidade);

                descricaoDestino += " / " + localidade.DescricaoCidadeEstado;
            }
        }

        private void SetarEstadosDestino(ref string descricaoDestino, ref Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFrete, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Estado repEstado = new Repositorio.Estado(unidadeTrabalho);

            dynamic estadosDestino = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("EstadosDestino"));

            tabelaFrete.EstadosDestino = new List<Dominio.Entidades.Estado>();

            foreach (var estadoDestino in estadosDestino)
            {
                Dominio.Entidades.Estado estado = repEstado.BuscarPorSigla((string)estadoDestino.Codigo);

                tabelaFrete.EstadosDestino.Add(estado);

                descricaoDestino += " / " + estado.Nome;
            }
        }

        private void SetarPaisesDestino(ref string descricaoDestino, ref Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFrete, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Pais repositorioPais = new Repositorio.Pais(unidadeTrabalho);
            dynamic paisesDestino = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("PaisesDestino"));
            tabelaFrete.PaisesDestino = new List<Dominio.Entidades.Pais>();

            foreach (var paisDestino in paisesDestino)
            {
                Dominio.Entidades.Pais pais = repositorioPais.BuscarPorCodigo((int)paisDestino.Codigo);

                tabelaFrete.PaisesDestino.Add(pais);

                descricaoDestino += $" / {pais.Nome}";
            }
        }

        private void SetarRegioesDestino(ref string descricaoDestino, ref Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFrete, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Localidades.Regiao repRegiao = new Repositorio.Embarcador.Localidades.Regiao(unidadeTrabalho);

            dynamic regioesDestino = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("RegioesDestino"));

            tabelaFrete.RegioesDestino = new List<Dominio.Entidades.Embarcador.Localidades.Regiao>();

            foreach (var regiaoDestino in regioesDestino)
            {
                Dominio.Entidades.Embarcador.Localidades.Regiao regiao = repRegiao.BuscarPorCodigo((int)regiaoDestino.Codigo);

                tabelaFrete.RegioesDestino.Add(regiao);

                descricaoDestino += " / " + regiao.Descricao;
            }
        }

        private void SetarClientesDestino(ref string descricaoDestino, ref Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFrete, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeTrabalho);

            dynamic clientesDestino = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ClientesDestino"));

            tabelaFrete.ClientesDestino = new List<Dominio.Entidades.Cliente>();

            foreach (var clienteDestino in clientesDestino)
            {
                double.TryParse(Utilidades.String.OnlyNumbers((string)clienteDestino.Codigo), out double cpfCnpjCliente);

                Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpjCliente);

                tabelaFrete.ClientesDestino.Add(cliente);
            }

            if (tabelaFrete.ClientesDestino.Count > 0)
            {
                List<Dominio.Entidades.Cliente> clientesPai = tabelaFrete.ClientesDestino.Where(cliente => cliente.PossuiFilialCliente).ToList();

                descricaoDestino = (clientesPai.Count > 0) ? string.Join(" / ", clientesPai.Select(cliente => cliente.Descricao)) : string.Join(" / ", tabelaFrete.ClientesDestino.Select(cliente => cliente.Descricao));
            }
        }

        private void SalvarCEPsDestino(ref string descricaoDestino, Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFrete, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Frete.TabelaFreteClienteCEPDestino repTabelaFreteClienteCEPDestino = new Repositorio.Embarcador.Frete.TabelaFreteClienteCEPDestino(unidadeTrabalho);

            dynamic cepsDestino = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaCEPsDestino"));

            if (tabelaFrete.CEPsDestino != null && tabelaFrete.CEPsDestino.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var cepDestino in cepsDestino)
                {
                    int codigo = 0;

                    if (int.TryParse((string)cepDestino.Codigo, out codigo))
                        codigos.Add((int)cepDestino.Codigo);
                }

                List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteCEPDestino> cepsDeletar = (from obj in tabelaFrete.CEPsDestino where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < cepsDeletar.Count; i++)
                    repTabelaFreteClienteCEPDestino.Deletar(cepsDeletar[i]);
            }

            foreach (var cepDestino in cepsDestino)
            {
                Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteCEPDestino cep = null;

                int codigo = 0;

                if (cepDestino.Codigo != null && int.TryParse((string)cepDestino.Codigo, out codigo))
                    cep = repTabelaFreteClienteCEPDestino.BuscarPorCodigo((int)cepDestino.Codigo, false);

                if (cep == null)
                    cep = new Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteCEPDestino();

                string cepInicial = (string)cepDestino.CEPInicial;
                string cepFinal = (string)cepDestino.CEPFinal;

                cep.TabelaFreteCliente = tabelaFrete;
                cep.CEPInicial = int.Parse(Utilidades.String.OnlyNumbers(cepInicial));
                cep.CEPFinal = int.Parse(Utilidades.String.OnlyNumbers(cepFinal));
                cep.DiasUteis = ((string)cepDestino.DiasUteis).ToInt();

                if (cep.Codigo > 0)
                    repTabelaFreteClienteCEPDestino.Atualizar(cep);
                else
                    repTabelaFreteClienteCEPDestino.Inserir(cep);

                descricaoDestino += " / " + cepInicial + " à " + cepFinal;
            }
        }

        private void SetarRotasDestino(ref string descricaoDestino, ref Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFrete, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.RotaFrete repRota = new Repositorio.RotaFrete(unidadeTrabalho);

            dynamic rotasDestino = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("RotasDestino"));

            tabelaFrete.RotasDestino = new List<Dominio.Entidades.RotaFrete>();

            foreach (var rotaDestino in rotasDestino)
            {
                Dominio.Entidades.RotaFrete rota = repRota.BuscarPorCodigo((int)rotaDestino.Codigo);

                tabelaFrete.RotasDestino.Add(rota);

                descricaoDestino += " / " + rota.Descricao;
            }
        }

        private void SetarTiposOperacao(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFrete, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unidadeTrabalho);

            dynamic tiposOperacao = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TiposOperacao"));

            tabelaFrete.TiposOperacao = new List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            foreach (var tipoOperacao in tiposOperacao)
            {
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOp = repTipoOperacao.BuscarPorCodigo((int)tipoOperacao.Codigo);

                tabelaFrete.TiposOperacao.Add(tipoOp);
            }
        }

        private void SetarTransportadoresTerceiros(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFrete, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeTrabalho);

            dynamic transportadoresTerceiros = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TransportadoresTerceiros"));

            tabelaFrete.TransportadoresTerceiros = new List<Dominio.Entidades.Cliente>();

            foreach (var transportadorTerceiro in transportadoresTerceiros)
            {
                Dominio.Entidades.Cliente transp = repCliente.BuscarPorCPFCNPJ((double)transportadorTerceiro.Codigo);

                tabelaFrete.TransportadoresTerceiros.Add(transp);
            }

            if (tabelaFrete.TabelaFrete.ObrigatorioInformarTerceiro && tabelaFrete.TransportadoresTerceiros.Count == 0)
                throw new ControllerException(Localization.Resources.Fretes.TabelaFreteCliente.EObrigatorioInformarAoMenosUmTransportadorTerceiroParaEssaTabela);
        }

        private void SalvarFronteiras(ref Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFrete, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            List<double> fronteiras = Newtonsoft.Json.JsonConvert.DeserializeObject<List<double>>(Request.Params("Fronteiras"));

            if (tabelaFrete.Fronteiras == null)
            {
                tabelaFrete.Fronteiras = new List<Dominio.Entidades.Cliente>();
            }
            else
            {
                List<Dominio.Entidades.Cliente> fronteirasDeletar = tabelaFrete.Fronteiras.Where(o => !fronteiras.Contains(o.Codigo)).ToList();

                foreach (Dominio.Entidades.Cliente fronteiraDeletar in fronteirasDeletar)
                    tabelaFrete.Fronteiras.Remove(fronteiraDeletar);
            }

            foreach (double fronteira in fronteiras)
            {
                if (!tabelaFrete.Fronteiras.Any(o => o.CPF_CNPJ == fronteira))
                {
                    Dominio.Entidades.Cliente fronteiraObj = repCliente.BuscarPorCPFCNPJ(fronteira);

                    tabelaFrete.Fronteiras.Add(fronteiraObj);
                }
            }
        }

        private void SetarRestricoesEntrega(ref Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFrete, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.TabelaFreteClienteFrequenciaEntrega repositorioTabelaFreteClienteFrequenciaEntrega = new Repositorio.Embarcador.Frete.TabelaFreteClienteFrequenciaEntrega(unitOfWork);

            List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteFrequenciaEntrega> tabelaFreteClienteFrequenciaEntregas = repositorioTabelaFreteClienteFrequenciaEntrega.BuscarPorCodigoTabelaFreteCliente(tabelaFrete.Codigo);

            List<DiaSemana> diasSemanaRestricaoEntrega = Request.GetListEnumParam<DiaSemana>("RestricaoEntrega");

            foreach (Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteFrequenciaEntrega frequenciaEntrega in tabelaFreteClienteFrequenciaEntregas)
            {
                if (diasSemanaRestricaoEntrega.Contains(frequenciaEntrega.DiaSemana))
                    diasSemanaRestricaoEntrega.Remove(frequenciaEntrega.DiaSemana);
                else
                    repositorioTabelaFreteClienteFrequenciaEntrega.Deletar(frequenciaEntrega);
            }

            foreach (DiaSemana diaSemana in diasSemanaRestricaoEntrega)
            {
                Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteFrequenciaEntrega restricaoEntrega = new Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteFrequenciaEntrega();
                restricaoEntrega.TabelaFreteCliente = tabelaFrete;
                restricaoEntrega.DiaSemana = diaSemana;
                repositorioTabelaFreteClienteFrequenciaEntrega.Inserir(restricaoEntrega);
            }

        }

        private void SetarTiposCarga(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFrete, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unidadeTrabalho);

            dynamic tiposCarga = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TiposCarga"));

            tabelaFrete.TiposCarga = new List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>();

            foreach (var tipoCarga in tiposCarga)
            {
                Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoC = repTipoCarga.BuscarPorCodigo((int)tipoCarga.Codigo);

                tabelaFrete.TiposCarga.Add(tipoC);
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosValores PreencherParametrosValores()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosValores()
            {
                Valores = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Valores")),
                Observacoes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Observacoes")),
                ValoresMinimosGarantidos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ValoresMinimosGarantidos")),
                ValoresMaximos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ValoresMaximos")),
                ValoresBases = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ValoresBases")),
                ValoresExcedentes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ValoresExcedentes")),
                PercentuaisPagamentoAgregados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("PercentuaisPagamentoAgregados"))
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteCliente ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            int numeroVigencia = Request.GetIntParam("Vigencia");

            Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteCliente filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteCliente()
            {
                CodigoTabelaFrete = Request.GetIntParam("TabelaFrete"),
                RotaFrete = Request.GetIntParam("RotaFrete"),
                CEPOrigem = Request.GetStringParam("CEPOrigem").ObterSomenteNumeros().ToInt(),//OLHAR
                CEPDestino = Request.GetStringParam("CEPDestino").ObterSomenteNumeros().ToInt(),//OLHAR
                Vigencia = numeroVigencia,
                SomenteEmVigencia = numeroVigencia > 0 ? false : Request.GetBoolParam("SomenteEmVigencia"),
                CpfCnpjTransportadorTerceiro = Request.GetDoubleParam("TransportadorTerceiro"),
                PossuiRota = Request.GetEnumParam<PossuiRota>("PossuiRota"),
                Ativo = Request.GetNullableEnumParam<SituacaoAtivoPesquisa>("Ativo"),
                SituacaoTabelaFrete = Request.GetNullableEnumParam<SituacaoAtivoPesquisa>("SituacaoTabelaFrete"),
                TipoPagamento = Request.GetNullableEnumParam<TipoPagamentoEmissao>("TipoPagamento"),
                CodigoIntegracao = Request.GetStringParam("CodigoIntegracao"),
                SituacaoAlteracaoTabelaFrete = Request.GetNullableEnumParam<SituacaoAlteracaoTabelaFrete>("SituacaoAlteracao"),
                CodigoCanalEntrega = Request.GetIntParam("CanalEntrega"),
                CodigoLocalidadeOrigemFiltro = Request.GetIntParam("LocalidadeOrigem"),
                CodigoRegiaoOrigemFiltro = Request.GetIntParam("RegiaoOrigem"),
                CodigoLocalidadeDestinoFiltro = Request.GetIntParam("LocalidadeDestino"),
                CodigoRegiaoDestinoFiltro = Request.GetIntParam("RegiaoDestino"),
                CodigoTipoOperacaoFiltro = Request.GetIntParam("TipoOperacao"),
                CodigoEmpresaFiltro = Request.GetIntParam("Empresa"),
                CpfCnpjRemetenteFiltro = Request.GetDoubleParam("Remetente"),
                CpfCnpjDestinatarioFiltro = Request.GetDoubleParam("Destinatario"),
                CpfCnpjTomadorFiltro = Request.GetDoubleParam("Tomador"),
                EstadoDestino = Request.GetStringParam("EstadoDestino"),
                EstadoOrigem = Request.GetStringParam("EstadoOrigem"),
                ContratoTransporteFrete = Request.GetIntParam("ContratoTransporteFrete"),
                SituacaoIntegracaoTabelaFreteCliente = Request.GetNullableEnumParam<SituacaoIntegracaoTabelaFreteCliente>("SituacaoIntegracao"),
            };

            return filtrosPesquisa;
        }

        private void ValidarTabelaFreteCliente(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente, Repositorio.UnitOfWork unitOfWork)
        {
            if (tabelaFreteCliente.ContratoTransporteFrete == null)
                return;

            DateTime dataInicioContrato = tabelaFreteCliente.ContratoTransporteFrete.DataInicio;
            DateTime dataFinalContrato = tabelaFreteCliente.ContratoTransporteFrete.DataFim;
            DateTime dataVigenciaInicial = tabelaFreteCliente.Vigencia.DataInicial;
            DateTime? dataVigenciaFinal = tabelaFreteCliente.Vigencia?.DataFinal;

            if (dataVigenciaInicial < dataInicioContrato || dataVigenciaFinal < dataInicioContrato || !dataVigenciaFinal.HasValue || dataVigenciaInicial > dataFinalContrato || dataVigenciaFinal > dataFinalContrato)
                throw new ControllerException("A vigência informada não pode estar fora do período do contrato do transportador.");
        }

        private void ValidarCamposObrigatoriosModeloVeicular(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente)
        {
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracaoLBC = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork).BuscarPorTipo(TipoIntegracao.LBC);

            if (tipoIntegracaoLBC == null)
                return;

            Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unitOfWork);
            IList<(int Codigo, string Descricao)> listaDadosModeloVeicularCarga = repositorioTabelaFreteCliente.BuscarDadosModeloVeicularCargaPorParametroBaseComValorInformado(tabelaFreteCliente.Codigo);

            //if (listaDadosModeloVeicularCarga.Count == 0)
            //    throw new ControllerException("É necessário informar ao menos um valor na Tabela de Valores de Frete.");

            Repositorio.Embarcador.Frete.TabelaFreteClienteModeloVeicularCarga repositorioTabelaFreteClienteModeloVeicularCarga = new Repositorio.Embarcador.Frete.TabelaFreteClienteModeloVeicularCarga(unitOfWork);
            List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteModeloVeicularCarga> tabelaFreteClienteModelosVeicularesCarga = repositorioTabelaFreteClienteModeloVeicularCarga.BuscarPorTabelaFreteCliente(tabelaFreteCliente.Codigo);

            foreach ((int Codigo, string Descricao) dadosModeloVeicularCarga in listaDadosModeloVeicularCarga)
            {
                Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteModeloVeicularCarga tabelaFreteClienteModeloVeicularCarga = tabelaFreteClienteModelosVeicularesCarga.FirstOrDefault(o => o.ModeloVeicularCarga.Codigo == dadosModeloVeicularCarga.Codigo);

                if ((tabelaFreteClienteModeloVeicularCarga == null) || (tabelaFreteClienteModeloVeicularCarga.QuantidadeEntregas <= 0))
                    throw new ControllerException($"É obrigatório informar a Quantidade de entregas no Modelo Veicular {dadosModeloVeicularCarga.Descricao}");
            }
        }

        #endregion
    }
}

