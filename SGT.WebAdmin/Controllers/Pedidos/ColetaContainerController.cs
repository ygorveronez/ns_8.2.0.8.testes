using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pedidos
{
    [CustomAuthorize("Cargas/Carga")]
    public class ColetaContainerController : BaseController
    {
		#region Construtores

		public ColetaContainerController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número Container", "NumeroContainer", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Coleta", "DataColeta", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Free Time", "FreeTime", 10, Models.Grid.Align.left, false);

                Dominio.ObjetosDeValor.Embarcador.Container.FiltroPesquisaControleContainer filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Container.FiltroPesquisaControleContainer()
                {
                    StatusContainer = StatusColetaContainer.EmAreaEsperaVazio,
                    NumeroContainer = Request.GetStringParam("NumeroContainer"),
                    DataInicialColeta = Request.GetDateTimeParam("DataColeta"),
                    LocalEsperaVazio = Request.GetDoubleParam("Local")
                };

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.Pedidos.ColetaContainer repColetaContainer = new Repositorio.Embarcador.Pedidos.ColetaContainer(unitOfWork);
                IList<Dominio.ObjetosDeValor.Embarcador.Container.ControleContainer> Coletacontainers = repColetaContainer.BuscarContainersPesquisa(filtrosPesquisa, parametrosConsulta);

                grid.setarQuantidadeTotal(repColetaContainer.ContarContainersPesquisa(filtrosPesquisa));

                var lista = (from p in Coletacontainers
                             select new
                             {
                                 p.Codigo,
                                 p.NumeroContainer,
                                 p.DataColeta,
                                 p.FreeTime
                             }).ToList();

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

        [AllowAuthenticate]
        public async Task<IActionResult> VincularContainerCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                DateTime dataColetaContainer = Request.GetDateTimeParam("DataColetaContainer");

                if (dataColetaContainer == DateTime.MinValue)
                    throw new ControllerException("Por favor informe a data de coleta container");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Pedidos.ColetaContainerAnexo repositorioColetaContainerAnexo = new Repositorio.Embarcador.Pedidos.ColetaContainerAnexo(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(Request.GetIntParam("Carga"));

                if (carga == null)
                    throw new ControllerException("Carga não encontrada.");

                if ((carga.SituacaoCarga == SituacaoCarga.Cancelada) || (carga.SituacaoCarga == SituacaoCarga.Encerrada) || (carga.SituacaoCarga == SituacaoCarga.Anulada))
                    throw new ControllerException("Situação atual da carga não permite informar container.");

                Repositorio.Embarcador.Pedidos.Container repositorioContainer = new Repositorio.Embarcador.Pedidos.Container(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.Container container = repositorioContainer.BuscarPorCodigo(Request.GetIntParam("Container"));

                if (container == null)
                    throw new ControllerException("Container não encontrado.");

                Servicos.Embarcador.Pedido.ColetaContainer servicoColetaContainer = new Servicos.Embarcador.Pedido.ColetaContainer(unitOfWork);
                Servicos.Embarcador.Pedido.ConferenciaContainer servicoConferenciaContainer = new Servicos.Embarcador.Pedido.ConferenciaContainer(unitOfWork, Auditado);
                Dominio.Entidades.Embarcador.Pedidos.ColetaContainer coletaContainer = servicoColetaContainer.VincularContainerAoColetaContainerCargaColeta(carga, container, dataColetaContainer);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemMovimentacaoContainer OrigemMovimentacaoContainer = OrigemMovimentacaoContainer.UsuarioInterno;

                if (this.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    OrigemMovimentacaoContainer = OrigemMovimentacaoContainer.PortalTransportador;

                if (VerificarDeveAtualizarColetaContainer(coletaContainer, carga, unitOfWork))
                {
                    Dominio.ObjetosDeValor.Embarcador.Pedido.AtualizarColetaContainerParametro parametrosColetaContainer = new Dominio.ObjetosDeValor.Embarcador.Pedido.AtualizarColetaContainerParametro()
                    {
                        CargaDaColeta = carga,
                        coletaContainer = coletaContainer,
                        DataAtualizacao = DateTime.Now,
                        LocalAtual = carga.DadosSumarizados?.ClientesRemetentes?.FirstOrDefault() ?? null,
                        Status = StatusColetaContainer.EmDeslocamentoVazio,
                        Usuario = this.Usuario,
                        OrigemMonimentacaoContainer = OrigemMovimentacaoContainer,
                        InformacaoOrigemMonimentacaoContainer = InformacaoOrigemMovimentacaoContainer.VincularContainerCarga,
                    };

                    servicoColetaContainer.AtualizarSituacaoColetaContainerEGerarHistorico(parametrosColetaContainer);
                }

                servicoConferenciaContainer.Atualizar(carga, container);
                if (carga.TipoOperacao?.ObrigarInformarRICnaColetaDeConteiner ?? false)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.ColetaContainerAnexo> anexosColetaContainerAnexo = repositorioColetaContainerAnexo.BuscarPorColetaContainerECarga(coletaContainer?.Codigo ?? 0, carga?.Codigo ?? 0);

                    if (anexosColetaContainerAnexo.Count == 0)
                        throw new ControllerException("É obrigatório informar o Ric do Container para poder avançar a carga!");
                }

                if (carga.TipoOperacao?.ObrigatorioVincularContainerCarga ?? false)
                {
                    Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                    Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega primeiraColeta = repositorioCargaEntrega.BuscarColetaNaOrigemPorCarga(carga.Codigo);

                    if (primeiraColeta?.Situacao == SituacaoEntrega.NaoEntregue)
                    {
                        Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork).BuscarPrimeiroRegistro();
                        Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacaoParametro = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork).BuscarPorCodigoFetch(primeiraColeta.Carga.TipoOperacao?.Codigo ?? 0);

                        Servicos.Embarcador.Notificacao.NotificacaoMTrack servicoNotificacaoMTrack = new Servicos.Embarcador.Notificacao.NotificacaoMTrack(unitOfWork);
                        Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FinalizarEntregaParametros parametrosFinalizarEntrega = new Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FinalizarEntregaParametros()
                        {
                            cargaEntrega = primeiraColeta,
                            dataInicioEntrega = dataColetaContainer,
                            dataTerminoEntrega = dataColetaContainer,
                            dataConfirmacao = dataColetaContainer,
                            configuracaoEmbarcador = ConfiguracaoEmbarcador,
                            tipoServicoMultisoftware = TipoServicoMultisoftware,
                            sistemaOrigem = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema,
                            OrigemSituacaoEntrega = OrigemSituacaoEntrega.UsuarioMultiEmbarcador,
                            Container = container.Codigo,
                            DataColetaContainer = dataColetaContainer,
                            auditado = Auditado,
                            configuracaoControleEntrega = configuracaoControleEntrega,
                            tipoOperacaoParametro = tipoOperacaoParametro,
                            TornarFinalizacaoDeEntregasAssincrona = configuracaoControleEntrega.TornarFinalizacaoDeEntregasAssincrona
                        };

                        Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.FinalizarEntrega(parametrosFinalizarEntrega, unitOfWork);
                        servicoNotificacaoMTrack.NotificarMudancaCargaEntrega(primeiraColeta, carga.Motoristas.ToList(), AdminMultisoftware.Dominio.Enumeradores.MobileHubs.EntregaConfirmadaNoEmbarcador, notificarSignalR: true, codigoClienteMultisoftware: Empresa.Codigo);
                    }
                }

                if (carga.TipoOperacao?.ObrigarInformarRICnaColetaDeConteiner ?? false)
                {
                    Repositorio.Embarcador.Pedidos.ColetaContainerAnexoRic repositorioRic = new Repositorio.Embarcador.Pedidos.ColetaContainerAnexoRic(unitOfWork);
                    Dominio.Entidades.Embarcador.Pedidos.ColetaContainerAnexoRic ric = repositorioRic.BuscarPorCodigoContainer(container.Codigo);
                    if (ric != null)
                    {
                        ric.ArmadorBooking = Request.GetStringParam("ArmadorBooking", string.Empty);
                        ric.DataDeColeta = dataColetaContainer;
                        ric.Motorista = Request.GetStringParam("Motorista", string.Empty);
                        ric.Placa = Request.GetStringParam("Placa", string.Empty);
                        ric.TipoContainer = Request.GetStringParam("TipoContainerRic", string.Empty);
                        ric.TaraContainer = Request.GetIntParam("TaraContainer");
                        ric.Transportadora = Request.GetStringParam("Transportadora", string.Empty);
                        repositorioRic.Atualizar(ric, Auditado);
                    }
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, coletaContainer, null, $"Vinculou Container {container.Numero} a carga {carga.CodigoCargaEmbarcador}", unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, $"Vinculou Container {container.Numero} a carga.", unitOfWork);

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
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao vincular container a carga. Verifique se já não existe outro container com a mesma carga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> RemoverContainerCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(Request.GetIntParam("Carga"));

                if (carga == null)
                    throw new ControllerException("Carga não encontrada.");

                Servicos.Embarcador.Pedido.ColetaContainer servicoColetaContainer = new Servicos.Embarcador.Pedido.ColetaContainer(unitOfWork);
                Repositorio.Embarcador.Pedidos.ColetaContainer repositorioColetaContainer = new Repositorio.Embarcador.Pedidos.ColetaContainer(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.ColetaContainer coletaContainer = repositorioColetaContainer.BuscarPorCodigo(Request.GetIntParam("Codigo"));
                string numeroContainer = coletaContainer.Container.Numero;

                servicoColetaContainer.RemoverContainerCarga(carga, coletaContainer, this.Usuario);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, coletaContainer, null, $"Removeu Container {numeroContainer} da carga {carga.CodigoCargaEmbarcador}.", unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, $"Removeu Container {numeroContainer} da carga.", unitOfWork);

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
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao vincular container a carga. Verifique se já não existe outro container com a mesma carga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        [AllowAuthenticate]
        public async Task<IActionResult> RemoverContainerRetiradaContainer()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga");
                bool usuarioPossuiPermissaoParaRemoverContainerCarga = permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_PermiteRemoverContainerVinculadoEmCarga);

                if (!usuarioPossuiPermissaoParaRemoverContainerCarga)
                    throw new ControllerException("Usuário não possui permissão para remover container.");

                unitOfWork.Start();

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Pedidos.RetiradaContainer repositorioRetiradaContainer = new Repositorio.Embarcador.Pedidos.RetiradaContainer(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(Request.GetIntParam("Carga"));
                Servicos.Embarcador.Pedido.ColetaContainer servicoColetaContainer = new Servicos.Embarcador.Pedido.ColetaContainer(unitOfWork);

                if (carga == null)
                    throw new ControllerException("Carga não encontrada.");

                if ((carga.SituacaoCarga == SituacaoCarga.Cancelada) || (carga.SituacaoCarga == SituacaoCarga.Encerrada) || carga.SituacaoCarga == SituacaoCarga.EmTransporte || carga.SituacaoCarga == SituacaoCarga.AgImpressaoDocumentos || carga.SituacaoCarga == SituacaoCarga.PendeciaDocumentos || (carga.SituacaoCarga == SituacaoCarga.Anulada))
                    throw new ControllerException("Situação atual da carga não permite remover container.");

                Repositorio.Embarcador.Pedidos.ColetaContainer repositorioColetaContainer = new Repositorio.Embarcador.Pedidos.ColetaContainer(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.RetiradaContainer retiradaContainer = repositorioRetiradaContainer.BuscarPorCarga(carga.Codigo);

                Dominio.Entidades.Embarcador.Pedidos.ColetaContainer coletaContainer;
                Dominio.Entidades.Embarcador.Pedidos.Container container = null;
                if (retiradaContainer != null)
                {
                    coletaContainer = retiradaContainer.ColetaContainer;
                    container = retiradaContainer.Container;
                }
                else
                    coletaContainer = repositorioColetaContainer.BuscarPorCargaAtual(carga.Codigo);

                if (coletaContainer == null)
                    return new JsonpResult(false, true, "Coleta container não encontrada para a carga");

                if (container == null)
                    container = coletaContainer.Container;

                if (container == null)
                    return new JsonpResult(false, true, "Coleta container e container não encontrada para a carga");

                Repositorio.Embarcador.Pedidos.ColetaContainerHistorico repositorioColetaContainerhistorico = new Repositorio.Embarcador.Pedidos.ColetaContainerHistorico(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.ColetaContainerHistorico ultimoHistoricoCargaAnterior = repositorioColetaContainerhistorico.BuscarUltimoHistoricoCargaAnteriorPorColetaContainer(coletaContainer.Codigo, coletaContainer.CargaAtual.Codigo);

                coletaContainer.CargaAtual = ultimoHistoricoCargaAnterior?.Carga ?? coletaContainer.CargaDeColeta;

                servicoColetaContainer.AtualizarSituacaoColetaContainerEGerarHistorico(new Dominio.ObjetosDeValor.Embarcador.Pedido.AtualizarColetaContainerParametro()
                {
                    AreaEsperaVazio = ultimoHistoricoCargaAnterior?.Local ?? coletaContainer.LocalColeta,
                    coletaContainer = coletaContainer,
                    DataAtualizacao = DateTime.Now,
                    LocalAtual = ultimoHistoricoCargaAnterior?.Local ?? coletaContainer.LocalColeta,
                    Status = StatusColetaContainer.EmAreaEsperaVazio,
                    Usuario = this.Usuario,
                    OrigemMonimentacaoContainer = OrigemMovimentacaoContainer.UsuarioInterno,
                    InformacaoOrigemMonimentacaoContainer = InformacaoOrigemMovimentacaoContainer.AoRemoverContainerCarga
                });

                if (retiradaContainer != null)
                {
                    retiradaContainer.Container = null;
                    retiradaContainer.ColetaContainer = null;
                    repositorioRetiradaContainer.Atualizar(retiradaContainer);
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, $"Removeu Container {container.Numero} da carga atravéz da retirada container.", unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, coletaContainer, null, $"Removeu Container {container.Numero} a carga {carga.CodigoCargaEmbarcador}", unitOfWork);

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
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao vincular container a carga. Verifique se já não existe outro container com a mesma carga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarContainerCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(Request.GetIntParam("Carga"));

                if (carga == null)
                    throw new ControllerException("Carga não encontrada.");

                Repositorio.Embarcador.Pedidos.ColetaContainer repColetaContainer = new Repositorio.Embarcador.Pedidos.ColetaContainer(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.ColetaContainer coletaContainer;


                if (carga.TipoOperacao?.OperacaoTransferenciaContainer ?? false)
                {
                    Dominio.Entidades.Embarcador.Pedidos.ColetaContainer coletaContainerDeletar;
                    coletaContainer = repColetaContainer.BuscarPorCargaAtual(carga.Codigo);

                    if (coletaContainer != null && coletaContainer.Container == null)//POG verificar se tem para essa carga uma coleta container com container.
                    {
                        coletaContainerDeletar = coletaContainer;

                        if (coletaContainer != null && coletaContainer.Container == null)
                            repColetaContainer.DeletarPorCodigo(coletaContainerDeletar.Codigo);

                        coletaContainer = repColetaContainer.BuscarPorCargaAtualComContainer(carga.Codigo);
                    }
                }
                else
                    coletaContainer = repColetaContainer.BuscarPorCargaDeColeta(carga.Codigo);

                return new JsonpResult(ObterDadosContainerCarga(carga, coletaContainer), true, "Sucesso");
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

        #endregion Métodos Globais

        #region Métodos Privados

        private dynamic ObterDadosContainerCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Pedidos.ColetaContainer coletaContainer)
        {
            if (coletaContainer != null)
                return new
                {
                    Codigo = coletaContainer.Codigo,
                    Carga = coletaContainer.CargaDeColeta != null ? coletaContainer.CargaDeColeta.Codigo : 0,
                    Container = new { Codigo = coletaContainer.Container?.Codigo ?? 0, Descricao = coletaContainer.Container?.Descricao ?? "" },
                    DataColetaContainer = coletaContainer.DataColeta.HasValue ? coletaContainer.DataColeta.Value.ToString("dd/MM/yyyy HH:mm") : "",
                    PodeRemoverContainer = (coletaContainer.Container != null) && PermitirRemoverContainerCarga(carga)
                };

            return new
            {
                Codigo = 0,
                Carga = carga.Codigo,
                Container = new { Codigo = 0, Descricao = "" },
                DataColetaContainer = "",
                PodeRemoverContainer = false
            };
        }

        private bool PermitirRemoverContainerCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            List<SituacaoCarga> situacoesPermitemRemoverContainer = new List<SituacaoCarga>()
            {
                SituacaoCarga.Nova,
                SituacaoCarga.AgTransportador,
                SituacaoCarga.NaLogistica,
                SituacaoCarga.AgNFe
            };

            return situacoesPermitemRemoverContainer.Contains(carga.SituacaoCarga);
        }

        private bool VerificarDeveAtualizarColetaContainer(Dominio.Entidades.Embarcador.Pedidos.ColetaContainer coletaContainer, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.ColetaContainerHistorico repositorioColetaContainerhistorico = new Repositorio.Embarcador.Pedidos.ColetaContainerHistorico(unitOfWork);

            if (coletaContainer.IsChangedByPropertyName("Container")) // se mudou container sempre vai para deslocamento vazio (é a carga de coleta ou carga de transferencia na qual muda a carga no container)
                return true;

            if (carga.TipoOperacao?.OperacaoTransferenciaContainer ?? false)
            {
                // só deve passar para emDeslocamentoVazio se esta em area de espera vazio e se nao passou por em deslocamento vazio antes.
                List<Dominio.Entidades.Embarcador.Pedidos.ColetaContainerHistorico> historicos = repositorioColetaContainerhistorico.BuscarPorColetaContainer(coletaContainer.Codigo);

                if (historicos != null && historicos.Count > 0)
                {
                    Dominio.Entidades.Embarcador.Pedidos.ColetaContainerHistorico ultimoHistorico = historicos.OrderByDescending(o => o.DataHistorico).FirstOrDefault();
                    //var esteveEmDeslocamentoVazio = historicos.Any(o => o.Status == StatusColetaContainer.EmDeslocamentoVazio);
                    //if (ultimoHistorico?.Status == StatusColetaContainer.EmAreaEsperaVazio && !esteveEmDeslocamentoVazio)

                    if (ultimoHistorico?.Status == StatusColetaContainer.EmAreaEsperaVazio)
                        return true;

                    return false;
                }
                else
                    return true;

            }

            return false;
        }

        #endregion Métodos Privados
    }
}

