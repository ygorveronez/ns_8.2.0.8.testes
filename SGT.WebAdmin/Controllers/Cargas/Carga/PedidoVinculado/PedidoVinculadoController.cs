using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.PedidoVinculado
{
    [CustomAuthorize("Cargas/Carga", "Logistica/JanelaCarregamento")]
    public class PedidoVinculadoController : BaseController
    {
		#region Construtores

		public PedidoVinculadoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> AdicionarPedidoVinculado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento repConfiguracao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repConfiguracaoPedido.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork).BuscarPrimeiroRegistro();
                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                int.TryParse(Request.Params("ContainerTipo"), out int codigoContainerTipo);
                int.TryParse(Request.Params("Container"), out int codigoContainer);
                int.TryParse(Request.Params("TaraContainer"), out int taraContainer);
                int.TryParse(Request.Params("CargaOrigem"), out int codigoCargaOrigem);
                int.TryParse(Request.Params("Carga"), out int codigoCarga);
                int.TryParse(Request.Params("CodigoPedido"), out int codigoPedido);
                int.TryParse(Request.Params("EnderecoDestinatario"), out int enderecoDestinatario);
                int.TryParse(Request.Params("EnderecoRecebedor"), out int enderecoRecebedor);
                int.TryParse(Request.Params("EnderecoExpedidor"), out int enderecoExpedidor);
                bool.TryParse(Request.Params("AtualizarPedido"), out bool atualizarPedido);
                List<int> cargaPedidos = Request.GetListParam<int>("CargaPedidos");

                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Expedidor")), out double cpfCnpjExpedidor);
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Recebedor")), out double cpfCnpjRecebedor);
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Remetente")), out double cpfCnpjRemetente);
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Destinatario")), out double cpfCnpjDestinatario);

                Enum.TryParse(Request.Params("TipoPedidoVinculado"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedidoVinculado tipoPedidoVinculado);

                string numeroPedido = Request.Params("NumeroPedido");
                string lacreContainerUm = Request.Params("LacreContainerUm");
                string lacreContainerDois = Request.Params("LacreContainerDois");
                string lacreContainerTres = Request.Params("LacreContainerTres");

                string numeroBooking = Request.GetStringParam("NumeroBooking");
                string observacaoInterna = Request.GetStringParam("ObservacaoInterna");
                int codigoMontagemContainer = Request.GetIntParam("MontagemContainer");
                int numeroPallets = Request.GetIntParam("NumeroPallets");

                DateTime dataPrevisaoSaida = Request.GetDateTimeParam("DataPrevisaoSaida");
                DateTime dataPrevisaoEntrega = Request.GetDateTimeParam("DataPrevisaoEntrega");
                DateTime dataAgendamento = Request.GetDateTimeParam("DataAgendamento");

                if (dataAgendamento > DateTime.MinValue && dataAgendamento < DateTime.Now)
                    return new JsonpResult(false, true, "A data do agendamento não pode ser menor que a data atual.");

                if (dataPrevisaoEntrega > DateTime.MinValue && dataPrevisaoEntrega < DateTime.Now && !(configuracaoPedido?.IgnorarValidacoesDatasPrevisaoAoEditarPedido ?? false))
                    return new JsonpResult(false, true, "A data de Previsão Entrega não pode ser menor que a data atual.");

                if (dataPrevisaoSaida > DateTime.MinValue && dataPrevisaoSaida < DateTime.Now && !(configuracaoPedido?.IgnorarValidacoesDatasPrevisaoAoEditarPedido ?? false))
                    return new JsonpResult(false, true, "A data de Previsão Saída não pode ser menor que a data atual.");

                if (dataPrevisaoEntrega < dataPrevisaoSaida && !(configuracaoPedido?.IgnorarValidacoesDatasPrevisaoAoEditarPedido ?? false))
                    return new JsonpResult(false, true, "A data de Previsão Entrega não pode ser menor que a data de Previsão Saída.");

                if (dataPrevisaoEntrega < dataPrevisaoSaida && !(configuracaoPedido?.IgnorarValidacoesDatasPrevisaoAoEditarPedido ?? false))
                    return new JsonpResult(false, true, "A data de Previsão Entrega não pode ser menor que a data de Previsão Saída.");


                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
                Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem = repCarga.BuscarPorCodigoFetch(codigoCargaOrigem);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

                if (cargaOrigem?.CargaBloqueadaParaEdicaoIntegracao ?? false)
                    return new JsonpResult(false, "A carga está bloqueada para a edição.");

                unitOfWork.Start();
                int codigoCargaPedido = 0;
                string retorno = "";

                if (atualizarPedido && codigoPedido > 0 && codigoCargaOrigem > 0)
                {
                    retorno = Servicos.Embarcador.Carga.CargaPedido.AtualizarPedido(codigoContainerTipo, dataPrevisaoSaida, dataPrevisaoEntrega, dataAgendamento, enderecoExpedidor, enderecoRecebedor, enderecoDestinatario, taraContainer, codigoPedido, codigoContainer, lacreContainerUm, lacreContainerDois, lacreContainerTres, codigoCargaOrigem, cpfCnpjRemetente, cpfCnpjDestinatario, cpfCnpjRecebedor, numeroPedido, tipoPedidoVinculado, unitOfWork, _conexao.StringConexao, TipoServicoMultisoftware, ConfiguracaoEmbarcador, cpfCnpjExpedidor, Auditado, out bool atualizouDataPrevisaoSaida, numeroBooking, codigoMontagemContainer, out bool atualizouDataPrevisaoEntrega, out bool atualizouRemetente, out bool atualizouDestinatario, numeroPallets, observacaoInterna);
                    if (atualizouDataPrevisaoSaida && dataPrevisaoSaida > DateTime.MinValue && configuracao.ValidarDataPrevisaoSaidaPedidoMenorDataAtual)
                    {
                        List<int> codigosPedidosAtualizar = repCargaPedido.BuscarCodigosPedidoPorCarga(codigoCarga > 0 ? codigoCarga : codigoCargaOrigem);
                        foreach (int codigoPedidoAtualizar in codigosPedidosAtualizar)
                        {
                            if (codigoPedidoAtualizar != codigoPedido)
                            {
                                Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoAtualizar = repPedido.BuscarPorCodigo(codigoPedidoAtualizar, true);
                                pedidoAtualizar.DataPrevisaoSaida = dataPrevisaoSaida;
                                repPedido.Atualizar(pedidoAtualizar, Auditado);
                            }
                        }
                    }
                    if (atualizouDataPrevisaoEntrega && dataPrevisaoEntrega > DateTime.MinValue && configuracao.ValidarDataPrevisaoSaidaPedidoMenorDataAtual)
                    {
                        List<int> codigosPedidosAtualizar = repCargaPedido.BuscarCodigosPedidoPorCarga(codigoCarga > 0 ? codigoCarga : codigoCargaOrigem);
                        foreach (int codigoPedidoAtualizar in codigosPedidosAtualizar)
                        {
                            if (codigoPedidoAtualizar != codigoPedido)
                            {
                                Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoAtualizar = repPedido.BuscarPorCodigo(codigoPedidoAtualizar, true);
                                pedidoAtualizar.PrevisaoEntrega = dataPrevisaoEntrega;
                                repPedido.Atualizar(pedidoAtualizar, Auditado);
                            }
                        }
                    }

                    if ((atualizouDestinatario || atualizouRemetente) && (cargaOrigem?.TipoOperacao?.ConfiguracaoCarga?.AdicionarBLComoOutroDocumentoAutomaticamenteNaCarga ?? false))
                    {
                        List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais = repPedidoXMLNotaFiscal.BuscarNotasFiscaisPorPedido(codigoPedido);
                        if (notasFiscais != null && notasFiscais.Count > 0)
                        {
                            foreach (var notaFiscal in notasFiscais)
                            {
                                if (cpfCnpjDestinatario > 0 && atualizouDestinatario)
                                    notaFiscal.Destinatario = repCliente.BuscarPorCPFCNPJ(cpfCnpjDestinatario);
                                if (cpfCnpjRemetente > 0 && atualizouRemetente)
                                    notaFiscal.Emitente = repCliente.BuscarPorCPFCNPJ(cpfCnpjRemetente);

                                repXMLNotaFiscal.Atualizar(notaFiscal);
                            }
                        }
                    }
                }
                else
                {
                    switch (tipoPedidoVinculado)
                    {
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedidoVinculado.Normal:
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedidoVinculado.Subcontratacao:
                            retorno = Servicos.Embarcador.Carga.CargaPedido.CriarPedidoNormalOuSubcontratacao(dataPrevisaoSaida, dataPrevisaoEntrega, dataAgendamento, taraContainer, codigoContainer, lacreContainerUm, lacreContainerDois, lacreContainerTres, codigoCargaOrigem, cpfCnpjRemetente, cpfCnpjDestinatario, cpfCnpjRecebedor, numeroPedido, tipoPedidoVinculado, unitOfWork, _conexao.StringConexao, TipoServicoMultisoftware, ConfiguracaoEmbarcador, out codigoCargaPedido, 0, configuracaoGeralCarga);
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedidoVinculado.EncaixeSubContratacao:
                            Servicos.Embarcador.Carga.PedidoVinculado.CriarCargaDeEncaixe(cargaOrigem, carga, cpfCnpjExpedidor, cpfCnpjRecebedor, unitOfWork, TipoServicoMultisoftware, ConfiguracaoEmbarcador, configuracaoGeralCarga);
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedidoVinculado.EncaixePedidoSubContratacao:
                            Servicos.Embarcador.Carga.PedidoVinculado.CriarPedidoDeEncaixe(cargaOrigem, cargaPedidos, cpfCnpjExpedidor, cpfCnpjRecebedor, unitOfWork, TipoServicoMultisoftware, ConfiguracaoEmbarcador, configuracaoGeralCarga);
                            Servicos.Embarcador.Carga.PedidoVinculado.AjustarCargaEncaixada(cargaOrigem, ConfiguracaoEmbarcador, unitOfWork, TipoServicoMultisoftware);
                            break;
                    }

                    if (carga?.RotaRecorrente ?? false)
                    {
                        carga.RotaRecorrente = false;
                        repCarga.Atualizar(carga);
                    }
                }

                if (!string.IsNullOrWhiteSpace(retorno))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, retorno);
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaOrigem, null, (atualizarPedido ? "Atualizou" : "Adicionou") + " pedido vinculado.", unitOfWork);

                unitOfWork.CommitChanges();

                Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();
                serHubCarga.InformarCargaAtualizada(codigoCargaOrigem, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, _conexao.StringConexao);

                var dynCarga = serCarga.ObterDetalhesDaCarga(cargaOrigem, TipoServicoMultisoftware, unitOfWork);

                return new JsonpResult(new
                {
                    Carga = dynCarga
                });
            }
            catch (ServicoException excecao)
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

        public async Task<IActionResult> ExcluirPedidosSemDocumento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_PermitirRemoverPedido))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                unitOfWork.Start();

                int codigoCarga = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

                if (codigoCarga == 0)
                    return new JsonpResult(false, true, "Carga não localizada.");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, true, "Carga não localizada.");

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(codigoCarga);

                if (cargaPedidos == null || cargaPedidos.Count == 0)
                    return new JsonpResult(false, true, "Carga não possui pedidos.");

                int qtdPedidos = cargaPedidos.Count();
                int qtdPedidosRemovidos = 0;
                bool removerPedido = false;// TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS;

                foreach (var cargaPedidoRemover in cargaPedidos)
                {
                    if ((cargaPedidoRemover.NotasFiscais == null || cargaPedidoRemover.NotasFiscais.Count == 0) && (cargaPedidoRemover.CargaPedidoDocumentosCTe == null || cargaPedidoRemover.CargaPedidoDocumentosCTe.Count == 0))
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repositorioCargaPedido.BuscarPorCodigo(cargaPedidoRemover.Codigo);

                        Servicos.Embarcador.Carga.CargaPedido.RemoverPedidoVinculadoCarga(cargaPedido, unitOfWork, ConfiguracaoEmbarcador, TipoServicoMultisoftware, this.Cliente, removerPedido: removerPedido);
                        qtdPedidosRemovidos++;
                    }
                }

                if (qtdPedidosRemovidos == 0)
                    return new JsonpResult(false, true, "Esta carga não possui nenhum pedido sem documentação.");
                else if (qtdPedidosRemovidos >= qtdPedidos)
                    return new JsonpResult(false, true, "Todos os pedidos desta carga não possuem documentação. É necessário ter ao menos um pedido na carga.");

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Excluiu pedido vinculado sem documentação.", unitOfWork);

                unitOfWork.CommitChanges();

                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                Servicos.Embarcador.Hubs.Carga servicoHubCarga = new Servicos.Embarcador.Hubs.Carga();
                servicoHubCarga.InformarCargaAtualizada(codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, _conexao.StringConexao);
                var cargaRetornar = servicoCarga.ObterDetalhesDaCarga(carga, TipoServicoMultisoftware, unitOfWork);

                return new JsonpResult(new
                {
                    Carga = cargaRetornar
                });
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao remover os pedidos sem documentos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPedidoVinculado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_PermitirRemoverPedido))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                unitOfWork.Start();

                int codigoCargaPedido = Request.GetIntParam("CodigoCargaPedido");
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repositorioCargaPedido.BuscarPorCodigo(codigoCargaPedido);
                int codigoCarga = cargaPedido?.Carga?.Codigo ?? 0;

                bool removerPedido = TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS;

                Servicos.Embarcador.Carga.CargaPedido.RemoverPedidoVinculadoCarga(cargaPedido, unitOfWork, ConfiguracaoEmbarcador, TipoServicoMultisoftware, this.Cliente, removerPedido: removerPedido);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Excluiu pedido vinculado.", unitOfWork);

                unitOfWork.CommitChanges();

                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                Servicos.Embarcador.Hubs.Carga servicoHubCarga = new Servicos.Embarcador.Hubs.Carga();
                servicoHubCarga.InformarCargaAtualizada(codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, _conexao.StringConexao);
                var cargaRetornar = servicoCarga.ObterDetalhesDaCarga(carga, TipoServicoMultisoftware, unitOfWork);

                return new JsonpResult(new
                {
                    Carga = cargaRetornar
                });
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Não é possível remover este pedido da carga pois já existe vínculo com o Tracking. Favor verifique a possibilidade de cancelar a carga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConsultarContainerEMP()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCargaPedido = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.EMP.ContainerEMP repContainerEMP = new Repositorio.Embarcador.EMP.ContainerEMP(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.Container repContainer = new Repositorio.Embarcador.Pedidos.Container(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repositorioCargaPedido.BuscarPorCodigo(codigoCargaPedido);

                if (cargaPedido == null)
                    return new JsonpResult(false, true, "Pedido não foi encontrado.");

                if (string.IsNullOrWhiteSpace(cargaPedido.Pedido.NumeroOS))
                    return new JsonpResult(false, true, "Pedido não possui número da O.S. informado.");

                if (!repContainerEMP.ExistePorNumeroOS(cargaPedido.Pedido.NumeroOS))
                    return new JsonpResult(false, true, "Não há Container EMP com o número da O.S. do pedido.");

                Dominio.Entidades.Embarcador.EMP.ContainerEMP containerEMP = repContainerEMP.BuscarPendentePorNumeroOS(cargaPedido.Pedido.NumeroOS);

                if (containerEMP == null)
                    return new JsonpResult(false, true, "Não há Container EMP pendente com o número da O.S. desse pedido.");

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaPedido.Pedido;
                Dominio.Entidades.Embarcador.Pedidos.Container container = repContainer.BuscarPorNumero(containerEMP.NumeroContainer);

                if (container == null)
                    return new JsonpResult(false, true, "Container não foi encontrado através do número: " + containerEMP.NumeroContainer + ", favor cadastre manualmente antes de realizar o vínculo.");

                unitOfWork.Start();

                pedido.Initialize();
                pedido.Container = container;
                pedido.TaraContainer = containerEMP.ValorTaraEspecifica.ToString();
                pedido.LacreContainerUm = containerEMP.Lacres;
                repPedido.Atualizar(pedido, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    Container = new
                    {
                        Codigo = container.Codigo,
                        Descricao = container.Descricao
                    },
                    TaraContainer = containerEMP.ValorTaraEspecifica.ToString(),
                    LacreContainerUm = containerEMP.Lacres
                });
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Houve uma falha ao consultar Container EMP.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConsultarContainerOSMae()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCargaPedido = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.EMP.ContainerEMP repContainerEMP = new Repositorio.Embarcador.EMP.ContainerEMP(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.Container repContainer = new Repositorio.Embarcador.Pedidos.Container(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoAdicional repositorioPedidoAdicionais = new Repositorio.Embarcador.Pedidos.PedidoAdicional(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repositorioCargaPedido.BuscarPorCodigo(codigoCargaPedido);

                if (cargaPedido == null)
                    return new JsonpResult(false, true, "Pedido não foi encontrado.");

                Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional pedidoAdicionais = repositorioPedidoAdicionais.BuscarPorPedido(cargaPedido.Pedido.Codigo);

                if (string.IsNullOrWhiteSpace(pedidoAdicionais?.NumeroOSMae ?? ""))
                    return new JsonpResult(false, true, "Pedido não possui número da O.S. mãe informado.");

                if (!repositorioCargaPedido.ExisteCargaMaePorNumeroOS(cargaPedido.Codigo, pedidoAdicionais.NumeroOSMae))
                    return new JsonpResult(false, true, "Não há carga mãe compatível.");

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoMae = repositorioCargaPedido.BuscarCargaMaePorNumeroOSComFetch(cargaPedido.Codigo, pedidoAdicionais.NumeroOSMae);

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaPedido.Pedido;
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoMae = cargaPedidoMae.Pedido;
                Dominio.Entidades.Embarcador.Pedidos.Container container = cargaPedidoMae.Pedido.Container;

                unitOfWork.Start();

                pedido.Initialize();
                pedido.Container = container;
                pedido.TaraContainer = pedidoMae.TaraContainer;
                if (!string.IsNullOrEmpty(pedidoMae.LacreContainerUm))
                    pedido.LacreContainerUm = pedidoMae.LacreContainerUm;
                repPedido.Atualizar(pedido, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    Container = new
                    {
                        Codigo = container.Codigo,
                        Descricao = container.Descricao
                    },
                    TaraContainer = pedidoMae.TaraContainer,
                    LacreContainerUm = pedidoMae.LacreContainerUm
                });
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Houve uma falha ao consultar Container.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
