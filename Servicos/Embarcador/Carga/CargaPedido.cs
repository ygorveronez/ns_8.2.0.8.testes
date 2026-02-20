using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Carga
{
    public class CargaPedido : ServicoBase
    {
        #region Construtores        

        public CargaPedido(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }

        #endregion

        #region Métodos Públicos

        public static string AtualizarPedido(int codigoContainerTipo, DateTime dataPrevisaoSaida, DateTime dataPrevisaoEntrega, DateTime dataAgendamento, int enderecoExpedidor, int enderecoRecebedor, int enderecoDestinatario, int taraContainer, int codigoPedido, int codigoContainer, string lacreContainerUm, string lacreContainerDois, string lacreContainerTres, int codigoCarga, double cpfCnpjRemetente, double cpfCnpjDestinatario, double cpfCnpjRecebedor, string numeroPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedidoVinculado tipoPedidoVinculado, Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, double cpfCnpjExpedidor, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, out bool atualizouDataPrevisaoSaida, string numeroBooking, int codigoMontagemContainer, out bool atualizouDataPrevisaoEntrega, out bool atualizouRemetente, out bool atualizouDestinatario, int numeroPallets, string observacaoInterna)
        {
            atualizouDataPrevisaoSaida = false;
            atualizouDataPrevisaoEntrega = false;
            atualizouRemetente = false;
            atualizouDestinatario = false;
            Repositorio.Embarcador.Pedidos.Container repContainer = new Repositorio.Embarcador.Pedidos.Container(unidadeTrabalho);
            Repositorio.Embarcador.WMS.MontagemContainerNotaFiscal repositorioMontagemContainerNf = new Repositorio.Embarcador.WMS.MontagemContainerNotaFiscal(unidadeTrabalho);
            Repositorio.Embarcador.Pedidos.ContainerTipo repContainerTipo = new Repositorio.Embarcador.Pedidos.ContainerTipo(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unidadeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeTrabalho);
            Repositorio.Embarcador.Pessoas.ClienteOutroEndereco repClienteOutroEndereco = new Repositorio.Embarcador.Pessoas.ClienteOutroEndereco(unidadeTrabalho);
            Repositorio.Embarcador.Pedidos.PedidoEndereco repPedidoEndereco = new Repositorio.Embarcador.Pedidos.PedidoEndereco(unidadeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unidadeTrabalho);
            Servicos.WebService.Carga.Pedido serWSPedido = new Servicos.WebService.Carga.Pedido(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe repCargaPedidoDocumentoCTe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe(unidadeTrabalho);
            Repositorio.ContainerCTE repContainerCTE = new Repositorio.ContainerCTE(unidadeTrabalho);
            Repositorio.Embarcador.CTe.CTeContainerDocumento repCTeContainerDocumento = new Repositorio.Embarcador.CTe.CTeContainerDocumento(unidadeTrabalho);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscalEletronica = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repositorioConfiguracaoPedido.BuscarConfiguracaoPadrao();

            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCargaEPedido(codigoCarga, codigoPedido);

            if (carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe && carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova)
                return "A situação atual da carga não permite que seja adicionado um novo pedido à mesma.";

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoClonado = repPedido.BuscarPorCodigo(codigoPedido, true);
            cargaPedido.Initialize();

            pedidoClonado.NumeroPaletes = numeroPallets;
            pedidoClonado.ObservacaoInterna = observacaoInterna;

            pedidoClonado.NumeroPedidoEmbarcador = numeroPedido;
            pedidoClonado.NumeroBooking = numeroBooking;
            pedidoClonado.AdicionadaManualmente = true;
            if (codigoContainer > 0)
            {
                pedidoClonado.Container = repContainer.BuscarPorCodigo(codigoContainer);
                if (pedidoClonado.Container.Tara > 0)
                    pedidoClonado.TaraContainer = Utilidades.String.OnlyNumbers(pedidoClonado.Container.Tara.ToString("n0"));
                if (codigoContainerTipo > 0 && pedidoClonado.Container.ContainerTipo == null)
                {
                    pedidoClonado.Container.ContainerTipo = repContainerTipo.BuscarPorCodigo(codigoContainerTipo);
                    repContainer.Atualizar(pedidoClonado.Container, auditado);
                }
            }
            else
            {
                pedidoClonado.Container = null;
                pedidoClonado.TaraContainer = "";
            }
            if (taraContainer > 0)
                pedidoClonado.TaraContainer = Utilidades.String.OnlyNumbers(taraContainer.ToString("n0"));
            pedidoClonado.LacreContainerDois = lacreContainerDois;
            pedidoClonado.LacreContainerTres = lacreContainerTres;
            pedidoClonado.LacreContainerUm = lacreContainerUm;

            if (dataPrevisaoSaida > DateTime.MinValue)
            {
                if (!pedidoClonado.DataPrevisaoSaida.HasValue || dataPrevisaoSaida != pedidoClonado.DataPrevisaoSaida)
                    atualizouDataPrevisaoSaida = true;
                pedidoClonado.DataPrevisaoSaida = dataPrevisaoSaida;
            }
            else if (pedidoClonado.DataPrevisaoSaida > DateTime.MinValue)
                pedidoClonado.DataPrevisaoSaida = null;
            if (dataPrevisaoEntrega > DateTime.MinValue)
            {
                if (!pedidoClonado.PrevisaoEntrega.HasValue || dataPrevisaoEntrega != pedidoClonado.PrevisaoEntrega)
                    atualizouDataPrevisaoEntrega = true;
                pedidoClonado.PrevisaoEntrega = dataPrevisaoEntrega;
            }
            else if (pedidoClonado.PrevisaoEntrega > DateTime.MinValue)
                pedidoClonado.PrevisaoEntrega = null;

            if (pedidoClonado.Recebedor != null)
                pedidoClonado.Destino = pedidoClonado.Recebedor.Localidade;
            else if (pedidoClonado.Destinatario != null)
                pedidoClonado.Destino = pedidoClonado.Destinatario.Localidade;
            if (pedidoClonado.Expedidor != null)
                pedidoClonado.Origem = pedidoClonado.Expedidor.Localidade;
            else if (pedidoClonado.Remetente != null)
                pedidoClonado.Origem = pedidoClonado.Remetente.Localidade;

            if (dataAgendamento > DateTime.MinValue)
            {
                pedidoClonado.DataAgendamento = dataAgendamento;
                if (dataAgendamento.Date < DateTime.Now.Date)
                    return "A data do agendamento não pode ser menor que a data atual.";
            }
            else if (pedidoClonado.DataAgendamento > DateTime.MinValue)
                pedidoClonado.DataAgendamento = null;

            if (enderecoDestinatario > 0)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEnderecoDestino = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco();
                pedidoEnderecoDestino.ClienteOutroEndereco = repClienteOutroEndereco.BuscarPorCodigo(enderecoDestinatario);
                if (pedidoEnderecoDestino.ClienteOutroEndereco != null)
                {
                    PreecherOutroEnderecoPedido(ref pedidoEnderecoDestino);
                    repPedidoEndereco.Inserir(pedidoEnderecoDestino);

                    if (pedidoEnderecoDestino.Localidade != null)
                    {
                        pedidoClonado.Destino = pedidoEnderecoDestino.Localidade;
                        pedidoClonado.EnderecoDestino = pedidoEnderecoDestino;
                        pedidoClonado.UsarOutroEnderecoDestino = true;
                    }
                }
            }
            else if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                pedidoClonado.Destino = pedidoClonado.Destinatario?.Localidade ?? null;
                pedidoClonado.EnderecoDestino = null;
                pedidoClonado.UsarOutroEnderecoDestino = false;
            }

            if (enderecoRecebedor > 0)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEnderecoRecebedor = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco();
                pedidoEnderecoRecebedor.ClienteOutroEndereco = repClienteOutroEndereco.BuscarPorCodigo(enderecoRecebedor);
                if (pedidoEnderecoRecebedor.ClienteOutroEndereco != null)
                {
                    PreecherOutroEnderecoPedido(ref pedidoEnderecoRecebedor);
                    repPedidoEndereco.Inserir(pedidoEnderecoRecebedor);

                    if (pedidoEnderecoRecebedor.Localidade != null)
                    {
                        pedidoClonado.Destino = pedidoEnderecoRecebedor.Localidade;
                        pedidoClonado.EnderecoRecebedor = pedidoEnderecoRecebedor;
                        pedidoClonado.UsarOutroEnderecoDestino = true;
                    }
                }
            }
            else
            {
                pedidoClonado.EnderecoRecebedor = null;
                pedidoClonado.UsarOutroEnderecoDestino = false;
            }

            if (cpfCnpjRemetente > 0d)
            {
                atualizouRemetente = true;
                pedidoClonado.Remetente = repCliente.BuscarPorCPFCNPJ(cpfCnpjRemetente);
                if (configuracao.UtilizaEmissaoMultimodal && pedidoClonado.Remetente != null)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoOrigem = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco();
                    serWSPedido.PreecherEnderecoPedidoPorCliente(enderecoOrigem, pedidoClonado.Remetente);
                    repPedidoEndereco.Inserir(enderecoOrigem);
                    pedidoClonado.EnderecoOrigem = enderecoOrigem;
                }
            }
            else if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                pedidoClonado.Remetente = null;

            if (enderecoExpedidor > 0)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEnderecoExpedidor = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco();
                pedidoEnderecoExpedidor.ClienteOutroEndereco = repClienteOutroEndereco.BuscarPorCodigo(enderecoExpedidor);
                if (pedidoEnderecoExpedidor.ClienteOutroEndereco != null)
                {
                    PreecherOutroEnderecoPedido(ref pedidoEnderecoExpedidor);
                    repPedidoEndereco.Inserir(pedidoEnderecoExpedidor);

                    if (pedidoEnderecoExpedidor.Localidade != null)
                    {
                        pedidoClonado.Origem = pedidoEnderecoExpedidor.Localidade;
                        pedidoClonado.EnderecoExpedidor = pedidoEnderecoExpedidor;
                        pedidoClonado.UsarOutroEnderecoOrigem = true;
                    }
                }
            }
            else
            {
                pedidoClonado.EnderecoExpedidor = null;
                pedidoClonado.UsarOutroEnderecoOrigem = false;
            }


            if (cpfCnpjDestinatario > 0d)
            {
                pedidoClonado.Destinatario = repCliente.BuscarPorCPFCNPJ(cpfCnpjDestinatario);
                atualizouDestinatario = true;
            }
            else if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                pedidoClonado.Destinatario = null;

            if (cpfCnpjRecebedor > 0d)
                pedidoClonado.Recebedor = repCliente.BuscarPorCPFCNPJ(cpfCnpjRecebedor);
            else
                pedidoClonado.Recebedor = null;

            if (cpfCnpjExpedidor > 0d)
                pedidoClonado.Expedidor = repCliente.BuscarPorCPFCNPJ(cpfCnpjExpedidor);
            else
                pedidoClonado.Expedidor = null;

            if (pedidoClonado.PedidoViagemNavio != null && pedidoClonado.Container != null)
            {
                if (pedidoClonado.TipoOperacao == null || pedidoClonado.TipoOperacao.TipoPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.VAS)
                {
                    if (!configuracaoPedido.NaoValidarMesmaViagemEMesmoContainer)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoExistente = repPedido.ValidarViagemContainer(pedidoClonado.PedidoViagemNavio.Codigo, pedidoClonado.Container.Codigo, pedidoClonado.Codigo);
                        if (pedidoExistente != null)
                            return "já existe outro pedido contendo o mesmo container para o mesmo navio/viagem e direção. Nº " + pedidoExistente.Numero.ToString("D") + " Booking: " + pedidoExistente.NumeroBooking;
                    }
                }
            }

            if (pedidoClonado.Container != null && pedidoClonado.Container.ContainerTipo != null && pedidoClonado.ContainerTipoReserva != null && !string.IsNullOrWhiteSpace(pedidoClonado.Container.ContainerTipo.CodigoIntegracao) && !string.IsNullOrWhiteSpace(pedidoClonado.ContainerTipoReserva.CodigoIntegracao))
            {
                if (pedidoClonado.Container.ContainerTipo.CodigoIntegracao != pedidoClonado.ContainerTipoReserva.CodigoIntegracao)
                {
                    return "não é possível selecionar um Container com o tipo diferente do informado no Tipo do Container de Reserva";
                }
            }

            if (cargaPedido.CTeEmitidoNoEmbarcador && pedidoClonado?.Container != null)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe> ctesEmbarcador = repCargaPedidoDocumentoCTe.BuscarPorCargaPedido(cargaPedido.Codigo);
                if (ctesEmbarcador != null && ctesEmbarcador.Count > 0)
                {
                    foreach (var cteEmbarcador in ctesEmbarcador)
                    {
                        if (cteEmbarcador == null || cteEmbarcador.CTe == null)
                            continue;

                        repCTeContainerDocumento.DeletarPorCTe(cteEmbarcador.CTe.Codigo);
                        repContainerCTE.DeletarPorCTe(cteEmbarcador.CTe.Codigo);

                        Dominio.Entidades.ContainerCTE container = new Dominio.Entidades.ContainerCTE()
                        {
                            Container = cargaPedido.Pedido.Container,
                            CTE = cteEmbarcador.CTe,
                            Lacre1 = cargaPedido.Pedido.LacreContainerUm,
                            Lacre2 = cargaPedido.Pedido.LacreContainerDois,
                            Lacre3 = cargaPedido.Pedido.LacreContainerTres,
                            Numero = cargaPedido.Pedido.Container.Numero
                        };
                        repContainerCTE.Inserir(container);
                        if (cteEmbarcador.CTe.Documentos != null && cteEmbarcador.CTe.Documentos.Count > 0)
                        {
                            foreach (var nota in cteEmbarcador.CTe.Documentos)
                            {
                                Dominio.Entidades.Embarcador.CTe.CTeContainerDocumento notaContainer = new Dominio.Entidades.Embarcador.CTe.CTeContainerDocumento()
                                {
                                    DocumentosCTE = nota,
                                    Chave = nota.ChaveNFE,
                                    ContainerCTE = container,
                                    Numero = nota.Numero,
                                    Serie = !string.IsNullOrWhiteSpace(nota.Serie) ? nota.Serie : "1",
                                    TipoDocumento = Dominio.Enumeradores.TipoDocumentoCTe.NFe,
                                    UnidadeMedidaRateada = 0
                                };

                                if (!string.IsNullOrWhiteSpace(notaContainer.Chave))
                                    notaContainer.XMLNotaFiscal = repXMLNotaFiscalEletronica.BuscarPorChave(notaContainer.Chave);

                                repCTeContainerDocumento.Inserir(notaContainer);
                            }
                        }
                    }
                }
            }

            if (codigoMontagemContainer > 0)
            {
                Repositorio.Embarcador.WMS.MontagemContainer repositorioMontagemContainer = new Repositorio.Embarcador.WMS.MontagemContainer(unidadeTrabalho);
                Pedido.NotaFiscal servicoCargaNotaFiscal = new Pedido.NotaFiscal(unidadeTrabalho);

                Dominio.Entidades.Embarcador.WMS.MontagemContainer montagemContainer = repositorioMontagemContainer.BuscarPorCodigo(codigoMontagemContainer, false);
                List<Dominio.Entidades.Embarcador.WMS.MontagemContainerNotaFiscal> listaNfsMontagemContainer = repositorioMontagemContainerNf.BuscarPorMontagemContainer(codigoMontagemContainer);

                foreach (Dominio.Entidades.Embarcador.WMS.MontagemContainerNotaFiscal xml in listaNfsMontagemContainer)
                    servicoCargaNotaFiscal.InserirNotaCargaPedido(xml.XMLNotaFiscal, cargaPedido, tipoServicoMultisoftware, TipoNotaFiscal.Venda, configuracao, false, out bool alteradoTipoDeCarga, auditado);

                pedidoClonado.MontagemContainer = montagemContainer;
            }

            repPedido.Atualizar(pedidoClonado, auditado);

            if (cpfCnpjRecebedor > 0d)
                cargaPedido.Recebedor = repCliente.BuscarPorCPFCNPJ(cpfCnpjRecebedor);
            else
                cargaPedido.Recebedor = null;

            if (cpfCnpjExpedidor > 0d)
                cargaPedido.Expedidor = repCliente.BuscarPorCPFCNPJ(cpfCnpjExpedidor);
            else
                cargaPedido.Expedidor = null;

            if (cargaPedido.Expedidor != null)
                cargaPedido.Origem = cargaPedido.Expedidor.Localidade;

            if (cargaPedido.Recebedor != null && cargaPedido.Pedido.EnderecoRecebedor != null && cargaPedido.Pedido.EnderecoRecebedor.Localidade != null)
                cargaPedido.Destino = cargaPedido.Pedido.EnderecoRecebedor.Localidade;
            else if (cargaPedido.Recebedor != null && enderecoDestinatario == 0)
                cargaPedido.Destino = cargaPedido.Recebedor.Localidade;
            else if (pedidoClonado.Destinatario != null && pedidoClonado.Destinatario.Localidade != null)
                cargaPedido.Destino = pedidoClonado.Destinatario.Localidade;

            if (cargaPedido.Expedidor != null && cargaPedido.Pedido.EnderecoExpedidor != null && cargaPedido.Pedido.EnderecoExpedidor.Localidade != null)
                cargaPedido.Origem = cargaPedido.Pedido.EnderecoExpedidor.Localidade;
            else if (cargaPedido.Expedidor != null && enderecoExpedidor == 0)
                cargaPedido.Origem = cargaPedido.Expedidor.Localidade;
            else if (pedidoClonado.Remetente != null && pedidoClonado.Remetente.Localidade != null)
                cargaPedido.Origem = pedidoClonado.Remetente.Localidade;

            repCargaPedido.Atualizar(cargaPedido, auditado);

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                Dominio.Entidades.Cliente tomador = cargaPedido.ObterTomador();

                if (tomador != null)
                {
                    if (tomador.Bloqueado)
                        return $"O cliente {tomador.Descricao} está bloqueado! Motivo: {tomador.MotivoBloqueio}.";
                    else if (tomador.GrupoPessoas?.Bloqueado ?? false)
                        return $"O grupo de pessoas {tomador.GrupoPessoas.Descricao} está bloqueado! Motivo: {tomador.GrupoPessoas.MotivoBloqueio}.";
                }
            }

            if (carga != null)
            {
                Servicos.Embarcador.Carga.CargaDadosSumarizados cargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unidadeTrabalho);
                cargaDadosSumarizados.AlterarDadosSumarizadosCarga(ref carga, configuracao, unidadeTrabalho, tipoServicoMultisoftware);
            }

            return "";
        }

        public static string CriarPedidoNormalOuSubcontratacao(DateTime dataPrevisaoSaida, DateTime dataPrevisaoEntrega, DateTime dataAgendamento, int taraContainer, int codigoContainer, string lacreContainerUm, string lacreContainerDois, string lacreContainerTres, int codigoCarga, double cpfCnpjRemetente, double cpfCnpjDestinatario, double cpfCnpjRecebedor, string numeroPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedidoVinculado tipoPedidoVinculado, Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, out int codigoCargaPedido, int codigoTipoDeCarga, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga)
        {
            codigoCargaPedido = 0;

            Repositorio.Embarcador.Pedidos.Container repContainer = new Repositorio.Embarcador.Pedidos.Container(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unidadeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeTrabalho);
            Repositorio.Embarcador.Pedidos.PedidoEndereco repPedidoEndereco = new Repositorio.Embarcador.Pedidos.PedidoEndereco(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unidadeTrabalho);
            Servicos.Embarcador.Pedido.Pedido svcPedido = new Servicos.Embarcador.Pedido.Pedido();
            Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repIntegracaoIntercab.BuscarIntegracao();
            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

            if (carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe && carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova)
                return "A situação atual da carga não permite que seja adicionado um novo pedido à mesma.";

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoExistente = carga.Pedidos.FirstOrDefault();

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoClonado = cargaPedidoExistente.Pedido.Clonar();
            cargaPedidoExistente.Pedido.ControleNumeracao = cargaPedidoExistente.Pedido.Codigo;
            repPedido.Atualizar(cargaPedidoExistente.Pedido);

            Utilidades.Object.DefinirListasGenericasComoNulas(pedidoClonado);

            pedidoClonado.Codigo = 0;
            pedidoClonado.ValorFreteNegociado = 0m;
            pedidoClonado.ValorFreteToneladaNegociado = 0m;
            pedidoClonado.Numero = repPedido.BuscarProximoNumero();
            pedidoClonado.NumeroPedidoEmbarcador = numeroPedido;
            pedidoClonado.AdicionadaManualmente = true;
            pedidoClonado.SituacaoAcompanhamentoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcompanhamentoPedido.AgColeta;
            if (codigoTipoDeCarga > 0)
                pedidoClonado.TipoDeCarga = repTipoDeCarga.BuscarPorCodigo(codigoTipoDeCarga);

            if (dataPrevisaoSaida > DateTime.MinValue)
                pedidoClonado.DataPrevisaoSaida = dataPrevisaoSaida;
            if (dataPrevisaoEntrega > DateTime.MinValue)
                pedidoClonado.PrevisaoEntrega = dataPrevisaoEntrega;
            if (dataAgendamento > DateTime.MinValue)
                pedidoClonado.DataAgendamento = dataAgendamento;

            if (codigoContainer > 0)
            {
                pedidoClonado.Container = repContainer.BuscarPorCodigo(codigoContainer);
                if (pedidoClonado.Container.Tara > 0)
                    pedidoClonado.TaraContainer = Utilidades.String.OnlyNumbers(pedidoClonado.Container.Tara.ToString("n0"));
            }
            else
            {
                pedidoClonado.Container = null;
                pedidoClonado.TaraContainer = "";
            }
            if (taraContainer > 0)
                pedidoClonado.TaraContainer = Utilidades.String.OnlyNumbers(taraContainer.ToString("n0"));
            pedidoClonado.LacreContainerDois = lacreContainerDois;
            pedidoClonado.LacreContainerTres = lacreContainerTres;
            pedidoClonado.LacreContainerUm = lacreContainerUm;
            pedidoClonado.DespachoTransitoAduaneiro = false;
            pedidoClonado.NumeroDTA = string.Empty;
            pedidoClonado.Seguro = false;
            pedidoClonado.ValorTotalCarga = 0m;
            pedidoClonado.EscoltaArmada = false;
            pedidoClonado.QtdEscolta = 0;
            pedidoClonado.Ajudante = false;
            pedidoClonado.QtdAjudantes = 0;
            pedidoClonado.PossuiCarga = false;
            pedidoClonado.ValorCarga = 0m;
            pedidoClonado.PossuiDescarga = false;
            pedidoClonado.ValorDescarga = 0m;
            pedidoClonado.PossuiDeslocamento = false;
            pedidoClonado.ValorDeslocamento = 0m;
            pedidoClonado.PossuiDiaria = false;
            pedidoClonado.ValorDiaria = 0m;

            if (cpfCnpjRemetente > 0d)
                pedidoClonado.Remetente = repCliente.BuscarPorCPFCNPJ(cpfCnpjRemetente);

            if (cpfCnpjDestinatario > 0d)
                pedidoClonado.Destinatario = repCliente.BuscarPorCPFCNPJ(cpfCnpjDestinatario);

            if (cpfCnpjRecebedor > 0d)
                pedidoClonado.Recebedor = repCliente.BuscarPorCPFCNPJ(cpfCnpjRecebedor);

            if (pedidoClonado.Recebedor != null)
                pedidoClonado.Destino = pedidoClonado.Recebedor.Localidade;
            else if (pedidoClonado.Destinatario != null)
                pedidoClonado.Destino = pedidoClonado.Destinatario.Localidade;

            if (pedidoClonado.Expedidor != null)
                pedidoClonado.Origem = pedidoClonado.Expedidor.Localidade;
            else if (pedidoClonado.Remetente != null)
                pedidoClonado.Origem = pedidoClonado.Remetente.Localidade;

            if (pedidoClonado.Container != null && pedidoClonado.Container.ContainerTipo != null && pedidoClonado.ContainerTipoReserva != null && !string.IsNullOrWhiteSpace(pedidoClonado.Container.ContainerTipo.CodigoIntegracao) && !string.IsNullOrWhiteSpace(pedidoClonado.ContainerTipoReserva.CodigoIntegracao))
            {
                if (pedidoClonado.Container.ContainerTipo.CodigoIntegracao != pedidoClonado.ContainerTipoReserva.CodigoIntegracao)
                {
                    return "não é possível selecionar um Container com o tipo diferente do informado no Tipo do Container de Reserva";
                }
            }

            pedidoClonado.EnderecoOrigem = null;
            pedidoClonado.EnderecoDestino = null;
            pedidoClonado.UsarOutroEnderecoOrigem = false;
            pedidoClonado.UsarOutroEnderecoDestino = false;

            Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoOrigem = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco();
            Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoDestino = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco();

            if (pedidoClonado.Recebedor != null)
                svcPedido.PreecherEnderecoPedido(ref enderecoDestino, pedidoClonado.Recebedor);
            else if (pedidoClonado.Destinatario != null)
                svcPedido.PreecherEnderecoPedido(ref enderecoDestino, pedidoClonado.Destinatario);

            if (pedidoClonado.Expedidor != null)
                svcPedido.PreecherEnderecoPedido(ref enderecoOrigem, pedidoClonado.Expedidor);
            else if (pedidoClonado.Remetente != null)
                svcPedido.PreecherEnderecoPedido(ref enderecoOrigem, pedidoClonado.Remetente);

            repPedidoEndereco.Inserir(enderecoOrigem);
            repPedidoEndereco.Inserir(enderecoDestino);

            pedidoClonado.EnderecoOrigem = enderecoOrigem;
            pedidoClonado.EnderecoDestino = enderecoDestino;

            if (pedidoClonado.GrupoPessoas != null && pedidoClonado.GrupoPessoas.Bloqueado)
                return ("O grupo de pessoas está bloqueado! Motivo: " + pedidoClonado.GrupoPessoas.MotivoBloqueio);

            if (pedidoClonado.Remetente != null && pedidoClonado.Remetente.Bloqueado)
                return ("O Remetente está bloqueado! Motivo: " + pedidoClonado.Remetente.MotivoBloqueio);

            if (pedidoClonado.Destinatario != null && pedidoClonado.Destinatario.Bloqueado)
                return ("O Destinatário está bloqueado! Motivo: " + pedidoClonado.Destinatario.MotivoBloqueio);

            if (pedidoClonado.Remetente != null && pedidoClonado.Remetente.GrupoPessoas != null && pedidoClonado.Remetente.GrupoPessoas.Bloqueado)
                return ("O Grupo de Pessoa do Remetente está bloqueado! Motivo: " + pedidoClonado.Remetente.GrupoPessoas.MotivoBloqueio);

            if (pedidoClonado.Destinatario != null && pedidoClonado.Destinatario.GrupoPessoas != null && pedidoClonado.Destinatario.GrupoPessoas.Bloqueado)
                return ("O Grupo de Pessoa do Destinatário está bloqueado! Motivo: " + pedidoClonado.Destinatario.GrupoPessoas.MotivoBloqueio);

            repPedido.Inserir(pedidoClonado);

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoNova = CriarCargaPedido(carga, pedidoClonado, null, unidadeTrabalho, stringConexao, tipoServicoMultisoftware, configuracao, false, configuracaoGeralCarga);

            if (cpfCnpjRecebedor > 0d)
                cargaPedidoNova.Recebedor = repCliente.BuscarPorCPFCNPJ(cpfCnpjRecebedor);

            if (integracaoIntercab?.BuscarTipoServicoModeloDocumentoVinculadoCarga ?? false)
                cargaPedidoNova.TipoContratacaoCarga = cargaPedidoExistente.TipoContratacaoCarga;
            else if (tipoPedidoVinculado == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedidoVinculado.Normal)
                cargaPedidoNova.TipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Normal;
            else if (tipoPedidoVinculado == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedidoVinculado.Subcontratacao)
                cargaPedidoNova.TipoContratacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada;

            repCargaPedido.Atualizar(cargaPedidoNova);
            codigoCargaPedido = cargaPedidoNova.Codigo;

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                Dominio.Entidades.Cliente tomador = cargaPedidoNova.ObterTomador();

                if (tomador != null)
                {
                    if (tomador.Bloqueado)
                        return $"O cliente {tomador.Descricao} está bloqueado! Motivo: {tomador.MotivoBloqueio}.";
                    else if (tomador.GrupoPessoas?.Bloqueado ?? false)
                        return $"O grupo de pessoas {tomador.GrupoPessoas.Descricao} está bloqueado! Motivo: {tomador.GrupoPessoas.MotivoBloqueio}.";
                }
            }

            return "";
        }

        public static Dominio.Entidades.Embarcador.Cargas.CargaPedido CriarCargaPedido(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoEncaixe, Repositorio.UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, bool pedidoEncaixe, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga)
        {
            return CriarCargaPedido(carga, pedido, cargaPedidoEncaixe, unitOfWork, stringConexao, tipoServicoMultisoftware, configuracao, pedidoEncaixe, NumeroReboque.SemReboque, TipoCarregamentoPedido.Normal, configuracaoGeralCarga);
        }

        public static Dominio.Entidades.Embarcador.Cargas.CargaPedido CriarCargaPedido(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoEncaixe, Repositorio.UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, bool pedidoEncaixe, NumeroReboque numeroReboque, TipoCarregamentoPedido tipoCarregamentoPedido, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga)
        {

            Repositorio.Embarcador.Pedidos.PedidoProduto repPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(unitOfWork);

            Servicos.Embarcador.Pedido.NotaFiscal serCargaNotaFiscal = new Servicos.Embarcador.Pedido.NotaFiscal(unitOfWork);

            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Servicos.Embarcador.Carga.CargaPedido serCargaPedido = new Servicos.Embarcador.Carga.CargaPedido(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> listaPedidoProduto = repPedidoProduto.BuscarPorPedido(pedido.Codigo);
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = new Dominio.Entidades.Embarcador.Cargas.CargaPedido();
            cargaPedido.Carga = carga;
            cargaPedido.CargaOrigem = carga;
            cargaPedido.Pedido = pedido;
            cargaPedido.Peso = pedido.PesoTotal;
            cargaPedido.PesoLiquido = pedido.PesoLiquidoTotal;
            cargaPedido.CargaPedidoEncaixe = cargaPedidoEncaixe;
            cargaPedido.PedidoEncaixado = pedidoEncaixe;
            cargaPedido.NumeroReboque = numeroReboque;
            cargaPedido.TipoCarregamentoPedido = tipoCarregamentoPedido;
            cargaPedido.OrdemEntrega = pedido.OrdemEntregaProgramada;

            if (pedido.ReentregaSolicitada)
            {
                cargaPedido.ReentregaSolicitada = true;
                pedido.ReentregaSolicitada = false;
                repPedido.Atualizar(pedido);
            }

            serCargaPedido.SetarDadosCargaPedido(ref cargaPedido, carga, pedido, tipoServicoMultisoftware, unitOfWork, configuracao, configuracaoGeralCarga);

            cargaPedido.Ativo = true;
            cargaPedido.ValorFrete = 0m;
            cargaPedido.SituacaoEmissao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNF.Nova;
            cargaPedido.Codigo = (int)repCargaPedido.Inserir(cargaPedido);

            //TODO: PPC - Adicionado log temporário para identificar problema no cargaPedido.Peso.
            Servicos.Log.TratarErro($"Pedido {cargaPedido.Pedido.NumeroPedidoEmbarcador} - CargaPedido.Codigo = {cargaPedido.Codigo} - Peso Total.: {cargaPedido.Peso}. CargaPedido.CriarCargaPedido", "PesoCargaPedido");

            serCargaPedido.SetarContaContabilPedido(cargaPedido, pedido, unitOfWork);
            serCargaPedido.AdicionarProdutosCargaPedido(cargaPedido, listaPedidoProduto, configuracao.UsarPesoProdutoSumarizacaoCarga, unitOfWork);

            if (cargaPedido.ReentregaSolicitada && !cargaPedido.PedidoEncaixado)
            {
                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> xMLNotaFiscals = repPedidoXMLNotaFiscal.BuscarNotasFiscaisPorPedido(pedido.Codigo);
                foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal in xMLNotaFiscals)
                {
                    if (xmlNotaFiscal.SituacaoEntregaNotaFiscal != SituacaoNotaFiscal.Entregue && xmlNotaFiscal.SituacaoEntregaNotaFiscal != SituacaoNotaFiscal.EntregueParcial)
                    {
                        serCargaNotaFiscal.InformarDadosNotaCarga(xmlNotaFiscal, cargaPedido, tipoServicoMultisoftware, out string msgAlerta);

                        Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado() { TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema };
                        Servicos.Auditoria.Auditoria.Auditar(auditado, xmlNotaFiscal, "Adicionado por pedido de reentrega", unitOfWork);
                    }
                }
            }

            return cargaPedido;
        }

        public void SetarContaContabilPedido(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork)
        {
            if (pedido.ContaContabil != null)
            {
                Repositorio.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao repositorioCargaPedidoContaContabilContabilizacao = new Repositorio.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao contaContabil = repositorioCargaPedidoContaContabilContabilizacao.BuscarFirstOrDefaultPorCargaPedido(cargaPedido.Codigo);

                if (contaContabil == null)
                    contaContabil = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao();

                contaContabil.CargaPedido = cargaPedido;
                contaContabil.PlanoConta = pedido.ContaContabil;
                contaContabil.TipoContabilizacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContabilizacao.Debito;
                contaContabil.TipoContaContabil = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.TotalReceber;

                if (contaContabil.Codigo > 0)
                    repositorioCargaPedidoContaContabilContabilizacao.Atualizar(contaContabil);
                else
                    repositorioCargaPedidoContaContabilContabilizacao.Inserir(contaContabil);
            }
        }

        public async Task SetarContaContabilPedidoAsync(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork)
        {
            if (pedido.ContaContabil == null)
                return;

            Repositorio.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao repositorioCargaPedidoContaContabilContabilizacao = new Repositorio.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao contaContabil = repositorioCargaPedidoContaContabilContabilizacao.BuscarFirstOrDefaultPorCargaPedido(cargaPedido.Codigo);

            if (contaContabil == null)
                contaContabil = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao();

            contaContabil.CargaPedido = cargaPedido;
            contaContabil.PlanoConta = pedido.ContaContabil;
            contaContabil.TipoContabilizacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContabilizacao.Debito;
            contaContabil.TipoContaContabil = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil.TotalReceber;

            if (contaContabil.Codigo > 0)
                await repositorioCargaPedidoContaContabilContabilizacao.AtualizarAsync(contaContabil);
            else
                await repositorioCargaPedidoContaContabilContabilizacao.InserirAsync(contaContabil);

        }

        public string SetarDadosCargaPedido(ref Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga, List<Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga> clientesDescarga = null, bool? possuiRegraTomador = null, bool? utilizarDistribuidorPorRegiaoNaRegiaoDestino = null)
        {
            string mensagem = "";

            Servicos.Embarcador.Carga.CargaPedido serCargaPedido = new CargaPedido(unitOfWork);
            Servicos.Embarcador.Carga.CTe serCTe = new Embarcador.Carga.CTe(unitOfWork);

            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoAdicional repositorioPedidoAdicional = new Repositorio.Embarcador.Pedidos.PedidoAdicional(unitOfWork);

            Dominio.Entidades.Localidade origem = pedido.Origem;
            Dominio.Entidades.Localidade destino = pedido.Destino;
            Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional pedidoAdicional = repositorioPedidoAdicional.BuscarPorPedido(pedido.Codigo);

            if (carga.Carregamento == null || carga.Carregamento.CarregamentoRedespacho || configuracao.NaoGerarCarregamentoRedespacho)
                cargaPedido.Expedidor = pedido.Expedidor;
            else if (pedido.Expedidor != null && tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                cargaPedido.Expedidor = pedido.Expedidor;

            cargaPedido.Tomador = pedido.Tomador;

            if (carga.Carregamento == null || (!carga.Carregamento.CarregamentoRedespacho && !(carga.Carregamento?.SessaoRoteirizador?.RoteirizacaoRedespacho ?? false)))
                cargaPedido.Recebedor = pedido.Recebedor;
            else if (pedido.Recebedor != null && tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                cargaPedido.Recebedor = pedido.Recebedor;

            cargaPedido.QtVolumes = pedido.QtVolumes;

            if (cargaPedido.ReentregaSolicitada && configuracao.NaoEmitirDocumentosEmCargasDeReentrega)
            {
                cargaPedido.PedidoSemNFe = true;
                //cargaPedido.CTeEmitidoNoEmbarcador = true;
                cargaPedido.CTesEmitidos = true;
            }

            if (cargaPedido.Pedido.CanalEntrega?.LiberarPedidoSemNFeAutomaticamente ?? false)
                cargaPedido.PedidoSemNFe = true;

            if (pedido.PontoPartida == null)
            {
                if (cargaPedido.Recebedor != null && carga.TipoOperacao != null && carga.TipoOperacao.UsarRecebedorComoPontoPartidaCarga)
                {
                    cargaPedido.PontoPartida = cargaPedido.Recebedor;
                    cargaPedido.PossuiColetaEquipamentoPontoPartida = true;
                }
                else
                {
                    cargaPedido.PontoPartida = null;
                    cargaPedido.PossuiColetaEquipamentoPontoPartida = false;
                }
            }
            else
                cargaPedido.PontoPartida = pedido.PontoPartida;

            cargaPedido.ElementoPEP = pedido.ElementoPEP;
            cargaPedido.CentroResultado = pedido.CentroResultadoEmbarcador;

            if (cargaPedido.Codigo > 0)
                SetarContaContabilPedido(cargaPedido, pedido, unitOfWork);

            cargaPedido.TipoTomador = pedido.TipoTomador;
            cargaPedido.TipoRateio = serCTe.BuscarTipoEmissaoDocumentosCTe(cargaPedido, tipoServicoMultisoftware, unitOfWork);

            if (carga.TipoOperacao != null)
            {
                if (carga.TipoOperacao.UsarConfiguracaoEmissao && carga.TipoOperacao.ModeloDocumentoFiscal != null)
                    cargaPedido.ModeloDocumentoFiscal = carga.TipoOperacao.ModeloDocumentoFiscal;
            }

            if (cargaPedido.Recebedor == null && cargaPedido.Pedido.Destinatario != null)
            {
                Repositorio.Embarcador.Pessoas.ClienteDescarga repClienteDescarga = new Repositorio.Embarcador.Pessoas.ClienteDescarga(unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga clienteDescarga = null;
                Dominio.Entidades.Cliente distribuidor = null;

                if (clientesDescarga == null)
                    clienteDescarga = repClienteDescarga.BuscarPorPessoa(cargaPedido.Pedido.Destinatario.CPF_CNPJ);
                else
                {
                    double dest = cargaPedido.Pedido.Destinatario.CPF_CNPJ;
                    clienteDescarga = (from obj in clientesDescarga where obj.Cliente.CPF_CNPJ == dest select obj).FirstOrDefault();
                }

                if (clienteDescarga != null && clienteDescarga.Distribuidor != null)
                    distribuidor = repCliente.BuscarPorCPFCNPJ(clienteDescarga.Distribuidor?.CNPJ_SemFormato.ToDouble() ?? 0d);


                bool consideraRecebedorDaCargaNaGeracaoMDFe = carga?.TipoOperacao?.ConfiguracaoCarga?.GerarMDFeParaRecebedorDaCarga ?? false;
                bool existeRecebedorNaCarga = carga?.Pedidos?.Any(x => x?.Recebedor != null) ?? false;

                if (consideraRecebedorDaCargaNaGeracaoMDFe && existeRecebedorNaCarga && (carga.TipoDeCarga != null))
                {
                    Repositorio.Embarcador.Localidades.DistribuidorRegiao repositorioDistribuidorPorRegiao = new Repositorio.Embarcador.Localidades.DistribuidorRegiao(unitOfWork);

                    distribuidor = repositorioDistribuidorPorRegiao.BuscarDistribuidorAtivoPorTipoCargaERegiao(carga.TipoDeCarga.Codigo, cargaPedido.Pedido.Destinatario?.Localidade.Regiao?.Codigo ?? 0);

                    if (distribuidor != null)
                        cargaPedido.NaoConsiderarRecebedorParaEmitirDocumentos = true;
                }
                else
                {
                    if ((utilizarDistribuidorPorRegiaoNaRegiaoDestino ?? false) && (carga.TipoDeCarga != null))
                    {
                        Repositorio.Embarcador.Localidades.DistribuidorRegiao repositorioDistribuidorPorRegiao = new Repositorio.Embarcador.Localidades.DistribuidorRegiao(unitOfWork);

                        distribuidor = repositorioDistribuidorPorRegiao.BuscarDistribuidorAtivoPorTipoCargaERegiao(carga.TipoDeCarga.Codigo, cargaPedido.Pedido.Destinatario?.Localidade.Regiao?.Codigo ?? 0);

                        if (distribuidor != null)
                            cargaPedido.NaoConsiderarRecebedorParaEmitirDocumentos = true;
                    }

                }

                if (distribuidor != null)
                {
                    destino = distribuidor.Localidade;
                    cargaPedido.Recebedor = distribuidor;

                    if (!(carga.TipoOperacao?.ConfiguracaoCarga?.GerarRedespachoAutomaticamenteAposEmissaoDocumentos ?? false))
                    {
                        cargaPedido.PendenteGerarCargaDistribuidor = true;
                        carga.PendenteGerarCargaDistribuidor = true;
                    }

                    repCarga.Atualizar(carga);
                }
            }

            if (carga.Carregamento?.Recebedor != null)
            {
                cargaPedido.Recebedor = carga.Carregamento?.Recebedor;
                cargaPedido.Destino = carga.Carregamento?.Recebedor?.Localidade;
            }

            if (carga.Carregamento?.Expedidor != null)
            {
                cargaPedido.Expedidor = carga.Carregamento?.Expedidor;
                cargaPedido.Origem = carga.Carregamento?.Expedidor?.Localidade;
            }

            if (!cargaPedido.Pedido.UsarTipoTomadorPedido)
            {
                Dominio.Entidades.Embarcador.Pedidos.RegraTomador regraTomador = null;
                if (!possuiRegraTomador.HasValue || possuiRegraTomador.Value)
                    regraTomador = Servicos.Embarcador.Pedido.RegraTomador.BuscarRegraTomador(cargaPedido, tipoServicoMultisoftware, unitOfWork);

                if (regraTomador != null)
                {
                    cargaPedido.TipoTomador = regraTomador.TipoTomador;
                    cargaPedido.Tomador = regraTomador.Tomador;
                    cargaPedido.RegraTomador = regraTomador;
                    pedido.UsarTipoPagamentoNF = false;


                    if (regraTomador.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente)
                        pedido.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Pago;
                    else if (regraTomador.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario)
                        pedido.TipoPagamento = Dominio.Enumeradores.TipoPagamento.A_Pagar;
                    else
                        pedido.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Outros;
                }
                else
                    cargaPedido.RegraTomador = null;

            }
            else
            {
                cargaPedido.TipoTomador = cargaPedido.Pedido.TipoTomador;
                if (cargaPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros)
                    cargaPedido.Tomador = cargaPedido.Pedido.Tomador;
                else
                    cargaPedido.Tomador = null;
            }


            if (cargaPedido.Expedidor != null)
            {
                origem = cargaPedido.Expedidor.Localidade;
                cargaPedido.TipoEmissaoCTeParticipantes = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor;
            }

            if (cargaPedido.Recebedor != null)
            {
                if (!(carga.TipoOperacao?.UtilizarRecebedorApenasComoParticipante ?? false))
                    destino = cargaPedido.Recebedor.Localidade;
                cargaPedido.TipoEmissaoCTeParticipantes = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor;
            }

            if (cargaPedido.Recebedor != null && cargaPedido.Expedidor != null)
                cargaPedido.TipoEmissaoCTeParticipantes = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor;

            VerificarFilialEmissaoCargaPedido(cargaPedido, configuracaoGeralCarga);

            if (cargaPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros && cargaPedido.Tomador == null)
                mensagem = "É obrigatório informar o Tomador quando o tipo de tomador for Outros";

            if (cargaPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor && cargaPedido.Expedidor == null)
                mensagem = "É obrigatório informar o Expedidor quando o tipo de tomador for Expedidor";

            if (cargaPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor && cargaPedido.Recebedor == null)
                mensagem = "É obrigatório informar o Recebedor quando o tipo de tomador for Recebedor";

            cargaPedido.Origem = origem;
            cargaPedido.Destino = destino;

            bool possuiCTe = false;
            bool possuiNFS = false;
            bool possuiNFSManual = false;
            serCargaPedido.VerificarQuaisDocumentosDeveEmitir(carga, cargaPedido, cargaPedido.Origem, cargaPedido.Destino, tipoServicoMultisoftware, unitOfWork, out possuiCTe, out possuiNFS, out possuiNFSManual, out Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscalIntramunicipal, configuracao, out bool sempreDisponibilizarDocumentoNFSManual);

            cargaPedido.PossuiCTe = possuiCTe;
            cargaPedido.PossuiNFS = possuiNFS;
            cargaPedido.PossuiNFSManual = possuiNFSManual;
            cargaPedido.ModeloDocumentoFiscalIntramunicipal = modeloDocumentoFiscalIntramunicipal;
            cargaPedido.DisponibilizarDocumentoNFSManual = sempreDisponibilizarDocumentoNFSManual;
            cargaPedido.Incoterm = pedidoAdicional?.Incoterm;
            cargaPedido.TransitoAduaneiro = pedidoAdicional?.TransitoAduaneiro;
            cargaPedido.NotificacaoCRT = pedidoAdicional?.NotificacaoCRT;
            cargaPedido.DtaRotaPrazoTransporte = pedidoAdicional?.DtaRotaPrazoTransporte ?? string.Empty;
            cargaPedido.TipoEmbalagem = pedidoAdicional?.TipoEmbalagem;
            cargaPedido.DetalheMercadoria = pedidoAdicional?.DetalheMercadoria ?? string.Empty;

            if (pedido.PedidoSVM)
                cargaPedido.Redespacho = true;

            return mensagem;
        }

        public async Task<string> SetarDadosCargaPedidoAsync(Dominio.ObjetosDeValor.Embarcador.Carga.SetarDadosCargaPedido setarDadosCargaPedido, Repositorio.UnitOfWork unitOfWork)
        {
            string mensagem = "";

            Servicos.Embarcador.Carga.CargaPedido servicoCargaPedido = new CargaPedido(unitOfWork);
            Servicos.Embarcador.Carga.CTe servicoCTe = new Embarcador.Carga.CTe(unitOfWork);

            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            Dominio.Entidades.Localidade origem = setarDadosCargaPedido.Pedido.Origem;
            Dominio.Entidades.Localidade destino = setarDadosCargaPedido.Pedido.Destino;

            Dominio.ObjetosDeValor.Embarcador.Carga.DadosCrtMicPedido dadosCrtMicPedido = null;

            if (setarDadosCargaPedido.DadosCrtMicPedido?.Count > 0)
                dadosCrtMicPedido = setarDadosCargaPedido.DadosCrtMicPedido.FirstOrDefault(x => x.CodigoPedido == setarDadosCargaPedido.Pedido.Codigo);

            if (setarDadosCargaPedido.Pedido.Expedidor != null && (setarDadosCargaPedido.Carga.Carregamento == null || setarDadosCargaPedido.Carga.Carregamento.CarregamentoRedespacho || setarDadosCargaPedido.Configuracao.NaoGerarCarregamentoRedespacho))
                setarDadosCargaPedido.CargaPedido.Expedidor = setarDadosCargaPedido.Pedido.Expedidor;
            else if (setarDadosCargaPedido.Pedido.Expedidor != null && setarDadosCargaPedido.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                setarDadosCargaPedido.CargaPedido.Expedidor = setarDadosCargaPedido.Pedido.Expedidor;

            if (setarDadosCargaPedido.Pedido.Tomador != null)
                setarDadosCargaPedido.CargaPedido.Tomador = new Dominio.Entidades.Cliente() { CPF_CNPJ = setarDadosCargaPedido.Pedido.Tomador.Codigo };

            if (setarDadosCargaPedido.Pedido.Recebedor != null && (setarDadosCargaPedido.Carga.Carregamento == null || (!setarDadosCargaPedido.Carga.Carregamento.CarregamentoRedespacho && !(setarDadosCargaPedido.Carga.Carregamento?.SessaoRoteirizador?.RoteirizacaoRedespacho ?? false))))
                setarDadosCargaPedido.CargaPedido.Recebedor = setarDadosCargaPedido.Pedido.Recebedor;
            else if (setarDadosCargaPedido.Pedido.Recebedor != null && setarDadosCargaPedido.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                setarDadosCargaPedido.CargaPedido.Recebedor = setarDadosCargaPedido.Pedido.Recebedor;

            setarDadosCargaPedido.CargaPedido.QtVolumes = setarDadosCargaPedido.Pedido.QtVolumes;

            if (setarDadosCargaPedido.CargaPedido.ReentregaSolicitada && setarDadosCargaPedido.Configuracao.NaoEmitirDocumentosEmCargasDeReentrega)
            {
                setarDadosCargaPedido.CargaPedido.PedidoSemNFe = true;
                setarDadosCargaPedido.CargaPedido.CTesEmitidos = true;
            }

            if (setarDadosCargaPedido.Pedido.CanalEntrega?.LiberarPedidoSemNFeAutomaticamente ?? false)
                setarDadosCargaPedido.CargaPedido.PedidoSemNFe = true;

            if (setarDadosCargaPedido.Pedido.PontoPartida == null)
            {
                if (setarDadosCargaPedido.CargaPedido.Recebedor != null && setarDadosCargaPedido.Carga.TipoOperacao != null && setarDadosCargaPedido.Carga.TipoOperacao.UsarRecebedorComoPontoPartidaCarga)
                {
                    setarDadosCargaPedido.CargaPedido.PontoPartida = setarDadosCargaPedido.CargaPedido.Recebedor;
                    setarDadosCargaPedido.CargaPedido.PossuiColetaEquipamentoPontoPartida = true;
                }
                else
                {
                    setarDadosCargaPedido.CargaPedido.PontoPartida = null;
                    setarDadosCargaPedido.CargaPedido.PossuiColetaEquipamentoPontoPartida = false;
                }
            }
            else
                setarDadosCargaPedido.CargaPedido.PontoPartida = new Dominio.Entidades.Cliente() { CPF_CNPJ = setarDadosCargaPedido.Pedido.PontoPartida.Codigo };

            setarDadosCargaPedido.CargaPedido.ElementoPEP = setarDadosCargaPedido.Pedido.ElementoPEP;
            setarDadosCargaPedido.CargaPedido.CentroResultado = setarDadosCargaPedido.Pedido.CentroResultadoEmbarcador;

            if (setarDadosCargaPedido.CargaPedido.Codigo > 0)
                await SetarContaContabilPedidoAsync(setarDadosCargaPedido.CargaPedido, setarDadosCargaPedido.Pedido, unitOfWork);

            setarDadosCargaPedido.CargaPedido.TipoTomador = setarDadosCargaPedido.Pedido.TipoTomador;
            setarDadosCargaPedido.CargaPedido.TipoRateio = servicoCTe.BuscarTipoEmissaoDocumentosCTe(setarDadosCargaPedido.CargaPedido, setarDadosCargaPedido.TipoServicoMultisoftware, unitOfWork, setarDadosCargaPedido.Tomador);

            if (setarDadosCargaPedido.Carga.TipoOperacao != null && setarDadosCargaPedido.Carga.TipoOperacao.UsarConfiguracaoEmissao && setarDadosCargaPedido.Carga.TipoOperacao.ModeloDocumentoFiscal != null)
                setarDadosCargaPedido.CargaPedido.ModeloDocumentoFiscal = setarDadosCargaPedido.Carga.TipoOperacao.ModeloDocumentoFiscal;

            if (setarDadosCargaPedido.CargaPedido.Recebedor == null && setarDadosCargaPedido.Pedido.Destinatario != null)
            {
                Repositorio.Embarcador.Pessoas.ClienteDescarga repClienteDescarga = new Repositorio.Embarcador.Pessoas.ClienteDescarga(unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga clienteDescarga = null;
                Dominio.Entidades.Cliente distribuidor = null;

                if (setarDadosCargaPedido.ClientesDescarga == null)
                    clienteDescarga = await repClienteDescarga.BuscarPorPessoaAsync(setarDadosCargaPedido.Pedido.Destinatario.Codigo);
                else
                {
                    double dest = setarDadosCargaPedido.Pedido.Destinatario.Codigo;
                    clienteDescarga = (from obj in setarDadosCargaPedido.ClientesDescarga where obj.Cliente.CPF_CNPJ == dest select obj).FirstOrDefault();
                }

                if (clienteDescarga != null && clienteDescarga.Distribuidor != null)
                    distribuidor = await repositorioCliente.BuscarPorCPFCNPJAsync(clienteDescarga.Distribuidor?.CNPJ_SemFormato.ToDouble() ?? 0d);

                if ((setarDadosCargaPedido.UtilizarDistribuidorPorRegiaoNaRegiaoDestino ?? false) && (setarDadosCargaPedido.Carga.TipoDeCarga != null))
                {
                    Repositorio.Embarcador.Localidades.DistribuidorRegiao repositorioDistribuidorPorRegiao = new Repositorio.Embarcador.Localidades.DistribuidorRegiao(unitOfWork);

                    distribuidor = repositorioDistribuidorPorRegiao.BuscarDistribuidorAtivoPorTipoCargaERegiao(setarDadosCargaPedido.Carga.TipoDeCarga.Codigo, setarDadosCargaPedido.Pedido.Destinatario?.Regiao?.Codigo ?? 0);

                    if (distribuidor != null)
                        setarDadosCargaPedido.CargaPedido.NaoConsiderarRecebedorParaEmitirDocumentos = true;
                }

                if (distribuidor != null)
                {
                    destino = distribuidor.Localidade;
                    setarDadosCargaPedido.CargaPedido.Recebedor = distribuidor;

                    if (!(setarDadosCargaPedido.Carga.TipoOperacao?.ConfiguracaoCarga?.GerarRedespachoAutomaticamenteAposEmissaoDocumentos ?? false))
                    {
                        setarDadosCargaPedido.CargaPedido.PendenteGerarCargaDistribuidor = true;
                        setarDadosCargaPedido.Carga.PendenteGerarCargaDistribuidor = true;
                    }

                    await repositorioCarga.AtualizarAsync(setarDadosCargaPedido.Carga);
                }
            }

            if (setarDadosCargaPedido.Carga.Carregamento?.Recebedor != null)
            {
                setarDadosCargaPedido.CargaPedido.Recebedor = setarDadosCargaPedido.Carga.Carregamento?.Recebedor;
                setarDadosCargaPedido.CargaPedido.Destino = setarDadosCargaPedido.Carga.Carregamento?.Recebedor?.Localidade;
            }

            if (setarDadosCargaPedido.Carga.Carregamento?.Expedidor != null)
            {
                setarDadosCargaPedido.CargaPedido.Expedidor = setarDadosCargaPedido.Carga.Carregamento?.Expedidor;
                setarDadosCargaPedido.CargaPedido.Origem = setarDadosCargaPedido.Carga.Carregamento?.Expedidor?.Localidade;
            }

            if (!setarDadosCargaPedido.Pedido.UsarTipoTomadorPedido)
            {
                Dominio.Entidades.Embarcador.Pedidos.RegraTomador regraTomador = null;
                if (!setarDadosCargaPedido.PossuiRegraTomador.HasValue || setarDadosCargaPedido.PossuiRegraTomador.Value)
                {
                    Servicos.Embarcador.Pedido.RegraTomador servicoRegraTomador = new Pedido.RegraTomador(unitOfWork);

                    regraTomador = await servicoRegraTomador.BuscarRegraTomadorAsync(new Dominio.ObjetosDeValor.Embarcador.Regras.FiltroRegraTomador()
                    {
                        CargaPedido = setarDadosCargaPedido.CargaPedido,
                        PossuiRegra = true,
                        TipoServicoMultisoftware = setarDadosCargaPedido.TipoServicoMultisoftware,
                        RegrasTomadores = setarDadosCargaPedido.RegrasTomadores,
                        Filiais = setarDadosCargaPedido.Filiais,
                        RegrasTomadoresSemTomador = setarDadosCargaPedido.RegrasTomadoresSemTomador
                    });
                }

                if (regraTomador != null)
                {
                    setarDadosCargaPedido.CargaPedido.TipoTomador = regraTomador.TipoTomador;
                    setarDadosCargaPedido.CargaPedido.Tomador = regraTomador.Tomador;
                    setarDadosCargaPedido.CargaPedido.RegraTomador = regraTomador;
                    setarDadosCargaPedido.Pedido.UsarTipoPagamentoNF = false;


                    if (regraTomador.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente)
                        setarDadosCargaPedido.Pedido.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Pago;
                    else if (regraTomador.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario)
                        setarDadosCargaPedido.Pedido.TipoPagamento = Dominio.Enumeradores.TipoPagamento.A_Pagar;
                    else
                        setarDadosCargaPedido.Pedido.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Outros;
                }
                else
                    setarDadosCargaPedido.CargaPedido.RegraTomador = null;

            }
            else
            {
                setarDadosCargaPedido.CargaPedido.TipoTomador = setarDadosCargaPedido.CargaPedido.Pedido.TipoTomador;
                if (setarDadosCargaPedido.CargaPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros)
                    setarDadosCargaPedido.CargaPedido.Tomador = setarDadosCargaPedido.CargaPedido.Pedido.Tomador;
                else
                    setarDadosCargaPedido.CargaPedido.Tomador = null;
            }

            if (setarDadosCargaPedido.CargaPedido.Expedidor != null)
            {
                origem = setarDadosCargaPedido.CargaPedido.Expedidor.Localidade;
                setarDadosCargaPedido.CargaPedido.TipoEmissaoCTeParticipantes = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor;
            }

            if (setarDadosCargaPedido.CargaPedido.Recebedor != null)
            {
                if (!(setarDadosCargaPedido.Carga.TipoOperacao?.UtilizarRecebedorApenasComoParticipante ?? false))
                    destino = setarDadosCargaPedido.CargaPedido.Recebedor.Localidade;

                setarDadosCargaPedido.CargaPedido.TipoEmissaoCTeParticipantes = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor;
            }

            if (setarDadosCargaPedido.CargaPedido.Recebedor != null && setarDadosCargaPedido.CargaPedido.Expedidor != null)
                setarDadosCargaPedido.CargaPedido.TipoEmissaoCTeParticipantes = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor;

            VerificarFilialEmissaoCargaPedido(setarDadosCargaPedido.CargaPedido, setarDadosCargaPedido.ConfiguracaoGeralCarga);

            if (setarDadosCargaPedido.CargaPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros && setarDadosCargaPedido.CargaPedido.Tomador == null)
                mensagem = "É obrigatório informar o Tomador quando o tipo de tomador for Outros";

            if (setarDadosCargaPedido.CargaPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor && setarDadosCargaPedido.CargaPedido.Expedidor == null)
                mensagem = "É obrigatório informar o Expedidor quando o tipo de tomador for Expedidor";

            if (setarDadosCargaPedido.CargaPedido.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor && setarDadosCargaPedido.CargaPedido.Recebedor == null)
                mensagem = "É obrigatório informar o Recebedor quando o tipo de tomador for Recebedor";

            setarDadosCargaPedido.CargaPedido.Origem = new Dominio.Entidades.Localidade() { Codigo = origem.Codigo, TipoEmissaoIntramunicipal = origem.TipoEmissaoIntramunicipal };
            setarDadosCargaPedido.CargaPedido.Destino = new Dominio.Entidades.Localidade() { Codigo = destino.Codigo };

            Dominio.ObjetosDeValor.Embarcador.Carga.RetornoVerificacaoDocumentoDeveEmitir retorno = await servicoCargaPedido.VerificarQuaisDocumentosDeveEmitirPorTomadorCodigoAsync(setarDadosCargaPedido.Carga, setarDadosCargaPedido.CargaPedido, setarDadosCargaPedido.Tomador, setarDadosCargaPedido.CargaPedido.Origem, setarDadosCargaPedido.CargaPedido.Destino, setarDadosCargaPedido.TipoServicoMultisoftware, unitOfWork, setarDadosCargaPedido.Configuracao);

            setarDadosCargaPedido.CargaPedido.PossuiCTe = retorno.PossuiCTe;
            setarDadosCargaPedido.CargaPedido.PossuiNFS = retorno.PossuiNFS;
            setarDadosCargaPedido.CargaPedido.PossuiNFSManual = retorno.PossuiNFSManual;
            setarDadosCargaPedido.CargaPedido.ModeloDocumentoFiscalIntramunicipal = retorno.ModeloDocumentoFiscalIntramunicipal;
            setarDadosCargaPedido.CargaPedido.DisponibilizarDocumentoNFSManual = retorno.SempreDisponibilizarDocumentoNFSManual;
            setarDadosCargaPedido.CargaPedido.Incoterm = dadosCrtMicPedido?.Incoterm;
            setarDadosCargaPedido.CargaPedido.TransitoAduaneiro = dadosCrtMicPedido?.TransitoAduaneiro;
            setarDadosCargaPedido.CargaPedido.NotificacaoCRT = dadosCrtMicPedido?.NotificacaoCRT;
            setarDadosCargaPedido.CargaPedido.DtaRotaPrazoTransporte = dadosCrtMicPedido?.DtaRotaPrazoTransporte ?? string.Empty;
            setarDadosCargaPedido.CargaPedido.TipoEmbalagem = dadosCrtMicPedido?.TipoEmbalagem;
            setarDadosCargaPedido.CargaPedido.DetalheMercadoria = dadosCrtMicPedido?.DetalheMercadoria ?? string.Empty;

            if (setarDadosCargaPedido.Pedido.PedidoSVM)
                setarDadosCargaPedido.CargaPedido.Redespacho = true;

            return mensagem;
        }

        public void VerificarSePossuiPedidoFilialEmissora(ref Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            if (carga.Filial != null && carga.TipoOperacao != null)
            {
                if (carga.Filial.EmpresaEmissora != null && carga.TipoOperacao.EmiteCTeFilialEmissora)
                {
                    Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga);

                    if (repCargaPedido.VerificarSeOperacaoTeraEmissaoFilialEmissoraPorCarga(carga.Codigo))
                    {
                        if (carga.EmpresaFilialEmissora == null)
                        {
                            Repositorio.Embarcador.Filiais.EstadoDestinoEmpresaEmissora repositorioEstadoDestinoEmpresaEmissora = new Repositorio.Embarcador.Filiais.EstadoDestinoEmpresaEmissora(unitOfWork);
                            List<Dominio.Entidades.Embarcador.Filiais.EstadoDestinoEmpresaEmissora> estadosDestinoEmpresaEmissora = carga.Filial != null ? repositorioEstadoDestinoEmpresaEmissora.BuscarPorFilial(carga.Filial.Codigo) : new List<Dominio.Entidades.Embarcador.Filiais.EstadoDestinoEmpresaEmissora>();
                            Dominio.Entidades.Embarcador.Filiais.EstadoDestinoEmpresaEmissora estadoDestino = estadosDestinoEmpresaEmissora.Find(e => e.Estado.Codigo == cargaPedidos.FirstOrDefault()?.Destino?.Estado.Codigo);

                            if ((estadosDestinoEmpresaEmissora.Count > 0) && estadoDestino != null)
                                carga.EmpresaFilialEmissora = estadoDestino.Empresa;
                            else
                                carga.EmpresaFilialEmissora = carga.Filial.EmpresaEmissora;
                        }
                    }
                    else
                        carga.EmpresaFilialEmissora = null;
                }
            }
        }

        public void VerificarFilialEmissaoCargaPedido(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga)
        {
            if (cargaPedido.CargaOrigem.Filial != null && cargaPedido.CargaOrigem.Filial.EmpresaEmissora != null && cargaPedido.CargaOrigem.TipoOperacao != null && cargaPedido.CargaOrigem.TipoOperacao.EmiteCTeFilialEmissora)
            {
                if (cargaPedido.Expedidor != null)
                {
                    if (cargaPedido.CargaOrigem.GrupoPessoaPrincipal != null && cargaPedido.CargaOrigem.GrupoPessoaPrincipal.EmitirSempreComoRedespacho)
                        cargaPedido.TipoContratacaoCargaSubContratacaoFilialEmissora = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho;
                    else
                    {
                        if (cargaPedido.CargaPedidoTrechoAnterior != null || (!cargaPedido.CargaPedidoFilialEmissora || configuracaoGeralCarga.GerarCargaComFluxoFilialEmissoraComExpedidor) || cargaPedido.EmitirComplementarFilialEmissora || cargaPedido.ProximoTrechoComplementaFilialEmissora)
                        {
                            if (configuracaoGeralCarga.GerarCargaComFluxoFilialEmissoraComExpedidor)
                                cargaPedido.CargaPedidoFilialEmissora = true;

                            if (cargaPedido.Recebedor == null)
                                cargaPedido.TipoContratacaoCargaSubContratacaoFilialEmissora = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho;
                            else
                                cargaPedido.TipoContratacaoCargaSubContratacaoFilialEmissora = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.RedespachoIntermediario;
                        }
                        else
                        {
                            cargaPedido.TipoContratacaoCargaSubContratacaoFilialEmissora = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada;
                            if (!cargaPedido.PedidoEncaixado && !cargaPedido.PossuiNFSManual && !(cargaPedido.ObterTomador()?.NaoEmitirCTeFilialEmissora ?? false))
                            {
                                cargaPedido.CargaPedidoFilialEmissora = true;
                                if (cargaPedido.EmitirComplementarFilialEmissora)
                                    cargaPedido.TipoContratacaoCarga = TipoContratacaoCarga.Redespacho;
                            }
                            else
                            {
                                cargaPedido.CargaPedidoFilialEmissora = false;
                                if (cargaPedido.EmitirComplementarFilialEmissora && !cargaPedido.PossuiNFSManual)
                                    cargaPedido.TipoContratacaoCarga = TipoContratacaoCarga.Normal;
                            }
                        }
                    }
                }
                else
                {
                    if (!cargaPedido.PedidoEncaixado && !cargaPedido.PossuiNFSManual && !(cargaPedido.ObterTomador()?.NaoEmitirCTeFilialEmissora ?? false))
                    {
                        cargaPedido.CargaPedidoFilialEmissora = true;
                        if (cargaPedido.EmitirComplementarFilialEmissora)
                            cargaPedido.TipoContratacaoCarga = TipoContratacaoCarga.Redespacho;
                    }
                    else
                    {
                        cargaPedido.CargaPedidoFilialEmissora = false;
                        if (cargaPedido.EmitirComplementarFilialEmissora && !cargaPedido.PossuiNFSManual)
                            cargaPedido.TipoContratacaoCarga = TipoContratacaoCarga.Normal;
                    }

                    if (cargaPedido.Recebedor != null && (cargaPedido.CargaPedidoProximoTrecho != null || !cargaPedido.CargaPedidoFilialEmissora || cargaPedido.ProximoTrechoComplementaFilialEmissora))
                        cargaPedido.TipoContratacaoCargaSubContratacaoFilialEmissora = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho;
                    else
                        cargaPedido.TipoContratacaoCargaSubContratacaoFilialEmissora = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada;
                }
            }
        }

        public void AdicionarCargaPedidoQuantidades(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> xMLNotaFiscals, List<Dominio.ObjetosDeValor.Embarcador.Carga.TotalizadorModeloDocumento> notasPedidos, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, ref decimal peso, ref decimal pesoLiquido, bool naoAtualizarPesoPedidoPelaNFe, Repositorio.UnitOfWork unitOfWork, List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoSumarizadorQuantidadeNotasFiscais> sumarizadorQuantidadesCargasPedidos = null, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades> cargaPedidoQuantidadesExitentes = null)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Servicos.Embarcador.Pedido.NotaFiscal serCargaNotaFiscal = new Servicos.Embarcador.Pedido.NotaFiscal(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades> cargaPedidoQuantidades = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades>();

            List<Dominio.ObjetosDeValor.Embarcador.Carga.TotalizadorModeloDocumento> notasFiscaisPedido = notasPedidos != null ? notasPedidos.Where(o => o.Codigo == cargaPedido.Pedido.Codigo).ToList() : new List<Dominio.ObjetosDeValor.Embarcador.Carga.TotalizadorModeloDocumento>();
            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> listaXMLNotasFiscaisPedido = notasFiscaisPedido.Count > 0 && xMLNotaFiscals != null ? xMLNotaFiscals.Where(o => notasFiscaisPedido.Select(p => p.Total).ToList().Contains(o.Codigo)).ToList() : new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

            List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoSumarizadorQuantidadeNotasFiscais> listaQuantidadesSumarizadasCargaPedido = sumarizadorQuantidadesCargasPedidos?.Where(o => o.CodigoCargaPedido == cargaPedido.Codigo).ToList();

            IList<int> notasFiscaisDuplicadas = new List<int>();
            if (configuracao.PermiteAdicionarNFeRepetidaParaOutroPedidoCarga)
            {
                notasFiscaisDuplicadas = repPedidoXMLNotaFiscal.BuscarListaCodigosNFeDuplicadaPorCarga(cargaPedido.Carga.Codigo);
                if (notasFiscaisDuplicadas.Count > 0)
                    notasFiscaisDuplicadas = repPedidoXMLNotaFiscal.BuscarNotasFiscaisPorNotasECargaPedido(notasFiscaisDuplicadas, cargaPedido.Codigo);
            }

            if (notasFiscaisDuplicadas.Count > 0 && listaXMLNotasFiscaisPedido.Count > 0)
                listaXMLNotasFiscaisPedido = listaXMLNotasFiscaisPedido.Where(o => !notasFiscaisDuplicadas.Contains(o.Codigo)).ToList();

            if (!naoAtualizarPesoPedidoPelaNFe || (cargaPedido.Peso == 0m && cargaPedido.Pedido.PesoTotal == 0m))
            {
                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> listaXMLNotasFiscaisPedidoPossuiSomentePallets = listaXMLNotasFiscaisPedido.Count > 0 ? listaXMLNotasFiscaisPedido.Where(o => configuracao.NaoUsarPesoNotasPallet ? o.TipoNotaFiscalIntegrada == TipoNotaFiscalIntegrada.RemessaPallet : 1 == 1).ToList() : new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> listaXMLNotasFiscaisPedidoNaoPossuiSomentePallets = listaXMLNotasFiscaisPedido.Count > 0 ? listaXMLNotasFiscaisPedido.Where(o => configuracao.NaoUsarPesoNotasPallet ? o.TipoNotaFiscalIntegrada != TipoNotaFiscalIntegrada.RemessaPallet : 1 == 1).ToList() : new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

                var pesoNotas = 0m;
                var pesoLiquidoNotas = 0m;

                if (listaXMLNotasFiscaisPedidoPossuiSomentePallets?.Count > 0 && listaXMLNotasFiscaisPedidoNaoPossuiSomentePallets?.Count > 0)
                    pesoNotas = listaXMLNotasFiscaisPedidoNaoPossuiSomentePallets.Sum(o => o.Peso);
                else if (listaXMLNotasFiscaisPedidoPossuiSomentePallets?.Count > 0 && listaXMLNotasFiscaisPedidoNaoPossuiSomentePallets?.Count == 0)
                    pesoNotas = listaXMLNotasFiscaisPedidoPossuiSomentePallets.Sum(o => o.Peso);
                else if (listaXMLNotasFiscaisPedidoNaoPossuiSomentePallets?.Count > 0)
                    pesoNotas = listaXMLNotasFiscaisPedidoNaoPossuiSomentePallets.Sum(o => o.Peso);

                if (listaXMLNotasFiscaisPedidoPossuiSomentePallets?.Count > 0 && listaXMLNotasFiscaisPedidoNaoPossuiSomentePallets?.Count > 0)
                    pesoLiquidoNotas = listaXMLNotasFiscaisPedidoNaoPossuiSomentePallets.Sum(o => o.PesoLiquido);
                else if (listaXMLNotasFiscaisPedidoPossuiSomentePallets?.Count > 0 && listaXMLNotasFiscaisPedidoNaoPossuiSomentePallets?.Count == 0)
                    pesoLiquidoNotas = listaXMLNotasFiscaisPedidoPossuiSomentePallets.Sum(o => o.PesoLiquido);
                else if (listaXMLNotasFiscaisPedidoNaoPossuiSomentePallets?.Count > 0)
                    pesoLiquidoNotas = listaXMLNotasFiscaisPedidoNaoPossuiSomentePallets.Sum(o => o.PesoLiquido);

                if (listaQuantidadesSumarizadasCargaPedido?.Count() > 0 && (pesoNotas <= 0m || pesoLiquidoNotas <= 0m))
                {
                    List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoSumarizadorQuantidadeNotasFiscais> listaQuantidadesPeso = listaQuantidadesSumarizadasCargaPedido;

                    if (configuracao.NaoUsarPesoNotasPallet)
                        listaQuantidadesPeso = listaQuantidadesPeso.Where(o => o.TipoNotaFiscalIntegrada != TipoNotaFiscalIntegrada.RemessaPallet).ToList();

                    if (notasFiscaisDuplicadas.Count > 0)
                        listaQuantidadesPeso = listaQuantidadesPeso.Where(o => !notasFiscaisDuplicadas.Contains(o.CodigoXMLNotaFiscal)).ToList();

                    if (cargaPedido.Carga.Internacional)
                    {
                        peso += pesoNotas > 0 ? pesoNotas : listaQuantidadesPeso.Where(o => !o.TipoFatura).Sum(o => o.Peso);
                        pesoLiquido += pesoLiquidoNotas > 0 ? pesoLiquidoNotas : listaQuantidadesPeso.Where(o => !o.TipoFatura).Sum(o => o.PesoLiquido);
                    }
                    else
                    {
                        peso += pesoNotas > 0 ? pesoNotas : listaQuantidadesPeso.Sum(o => o.Peso);
                        pesoLiquido += pesoLiquidoNotas > 0 ? pesoLiquidoNotas : listaQuantidadesPeso.Sum(o => o.PesoLiquido);
                    }
                }
                else
                {
                    peso += pesoNotas > 0 ? pesoNotas : repPedidoXMLNotaFiscal.BuscarPesoPorCargaPedido(cargaPedido.Codigo, notasFiscaisDuplicadas, configuracao.NaoUsarPesoNotasPallet);
                    pesoLiquido += pesoLiquidoNotas > 0 ? pesoLiquidoNotas : repPedidoXMLNotaFiscal.BuscarPesoLiquidoPorCargaPedido(cargaPedido.Codigo, notasFiscaisDuplicadas, configuracao.NaoUsarPesoNotasPallet);
                }

                if (notasFiscaisDuplicadas.Count > 0)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasDuplicadas = repXMLNotaFiscal.BuscarPorCodigos(notasFiscaisDuplicadas);
                    foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaDuplicada in notasDuplicadas)
                    {
                        decimal pesoPedido = configuracao.UtilizarPesoPedidoParaRatearPesoNFeRepetida ? cargaPedido.Pedido.PesoTotal : cargaPedido.Peso;

                        if ((pesoPedido > 0) && (notaDuplicada.Peso > 0))
                        {
                            decimal pesoTotalPedidos;

                            if (configuracao.UtilizarPesoPedidoParaRatearPesoNFeRepetida)
                                pesoTotalPedidos = repPedidoXMLNotaFiscal.BuscarPesoPedidosNotasDuplicas(notaDuplicada.Codigo, cargaPedido.Carga.Codigo);
                            else
                                pesoTotalPedidos = repPedidoXMLNotaFiscal.BuscarPesoCargaPedidosNotasDuplicas(notaDuplicada.Codigo, cargaPedido.Carga.Codigo);

                            if (pesoTotalPedidos > 0m)
                            {
                                decimal percentual = pesoPedido * 100 / pesoTotalPedidos;

                                peso += Math.Round(((notaDuplicada.Peso * percentual) / 100), 3);
                                pesoLiquido += Math.Round(((notaDuplicada.PesoLiquido * percentual) / 100), 3);
                            }
                        }
                    }
                }
            }
            else
            {
                peso = cargaPedido.Peso > 0 ? cargaPedido.Peso : cargaPedido.Pedido.PesoTotal;
                pesoLiquido = cargaPedido.PesoLiquido > 0 ? cargaPedido.PesoLiquido : cargaPedido.Pedido.PesoLiquidoTotal;
            }

            if (peso > 0m)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades cargaPedidoQuantidade = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades
                {
                    CargaPedido = cargaPedido,
                    Quantidade = peso,
                    Unidade = Dominio.Enumeradores.UnidadeMedida.KG
                };

                cargaPedidoQuantidades.Add(cargaPedidoQuantidade);

                if (!configuracao.AtualizarProdutosCarregamentoPorNota)
                {
                    //TODO: PPC - Adicionado log temporário para identificar problema no cargaPedido.Peso.
                    Servicos.Log.TratarErro($"Pedido {cargaPedido.Pedido.NumeroPedidoEmbarcador} - CargaPedido.Codigo = {cargaPedido.Codigo} - Peso Total De.: {cargaPedido.Peso} - Para.: {peso}. CargaPedido.AdicionarCargaPedidoQuantidades", "PesoCargaPedido");
                    Servicos.Log.TratarErro($"Pedido {cargaPedido.Pedido.NumeroPedidoEmbarcador} - CargaPedido.Codigo = {cargaPedido.Codigo} - Peso Liquido De.: {cargaPedido.PesoLiquido} - Para.: {pesoLiquido}. CargaPedido.AdicionarCargaPedidoQuantidades", "PesoCargaPedido");
                    cargaPedido.Peso = peso;
                    cargaPedido.PesoLiquido = pesoLiquido;
                }
            }

            int volumes = listaXMLNotasFiscaisPedido.Count > 0 ? listaXMLNotasFiscaisPedido.Sum(o => o.Volumes) : listaQuantidadesSumarizadasCargaPedido?.Count() > 0 ? listaQuantidadesSumarizadasCargaPedido.Sum(o => o.Volumes) : repPedidoXMLNotaFiscal.BuscarVolumesPorCargaPedido(cargaPedido.Codigo);
            if (volumes > 0)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades cargaPedidoQuantidade = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades
                {
                    CargaPedido = cargaPedido,
                    Quantidade = volumes,
                    Unidade = Dominio.Enumeradores.UnidadeMedida.UN
                };

                cargaPedidoQuantidades.Add(cargaPedidoQuantidade);

                if (!naoAtualizarPesoPedidoPelaNFe)
                    cargaPedido.QtVolumes = volumes;
            }

            decimal metrosCubicos = listaXMLNotasFiscaisPedido.Count > 0 ? listaXMLNotasFiscaisPedido.Sum(o => o.MetrosCubicos) : listaQuantidadesSumarizadasCargaPedido?.Count() > 0 ? listaQuantidadesSumarizadasCargaPedido.Sum(o => o.MetrosCubicos) : repPedidoXMLNotaFiscal.BuscarMetrosCubicosPorCargaPedido(cargaPedido.Codigo);
            if (metrosCubicos == 0 && configuracao.UsarCubagemPedidoSeNFeSemCubagem)
                metrosCubicos = repCargaPedido.BuscarCubagemTotalPorCargaPedido(cargaPedido.Codigo);

            decimal pesoCubado = listaXMLNotasFiscaisPedido.Count > 0 ? listaXMLNotasFiscaisPedido.Sum(o => o.PesoCubado) : listaQuantidadesSumarizadasCargaPedido?.Count() > 0 ? listaQuantidadesSumarizadasCargaPedido.Sum(o => o.PesoCubado) : repPedidoXMLNotaFiscal.BuscarPesoCubadoPorCargaPedido(cargaPedido.Codigo);
            decimal cubagemPedido = cargaPedido.Pedido.CubagemTotal;

            if (metrosCubicos > 0 || pesoCubado > 0 || cubagemPedido > 0)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades cargaPedidoQuantidade = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades
                {
                    CargaPedido = cargaPedido,
                    Quantidade = metrosCubicos > 0 ? metrosCubicos : pesoCubado > 0 ? pesoCubado : cubagemPedido,
                    Unidade = Dominio.Enumeradores.UnidadeMedida.M3
                };

                cargaPedidoQuantidades.Add(cargaPedidoQuantidade);
            }

            Repositorio.Embarcador.Cargas.CargaPedidoQuantidades repCargaPedidoQuantidades = new Repositorio.Embarcador.Cargas.CargaPedidoQuantidades(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades> quantidadesExitentes = cargaPedidoQuantidadesExitentes?.Count() > 0 ? cargaPedidoQuantidadesExitentes.Where(o => o.CargaPedido.Codigo == cargaPedido.Codigo).ToList() : repCargaPedidoQuantidades.BuscarPorCargaPedido(cargaPedido.Codigo);

            if (quantidadesExitentes.Count > 0)
                repCargaPedidoQuantidades.Deletar(quantidadesExitentes, "T_CARGA_PEDIDO_QUANTIDADES");

            if (cargaPedidoQuantidades.Count > 0) repCargaPedidoQuantidades.Inserir(cargaPedidoQuantidades, "T_CARGA_PEDIDO_QUANTIDADES");

            //devemos tambem atualizar os pesos do carga pedido de proximo trecho e recalcular o frete da carga
            if (cargaPedido.Carga.DadosSumarizados?.CargaTrecho == CargaTrechoSumarizada.SubCarga && cargaPedido.CargaPedidoProximoTrecho != null)
            {
                decimal pesoProximoTrecho = 0;
                decimal pesoLiquidoProximoTrecho = 0;

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> PedidoxMLNotaFiscais = repPedidoXMLNotaFiscal.BuscarPorCargaPedido(cargaPedido.Codigo);

                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedxmlNotaFiscal in PedidoxMLNotaFiscais)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoProximoTrecho = repCargaPedido.BuscarPorCodigoComFetch(cargaPedido.CargaPedidoProximoTrecho.Codigo);
                    serCargaNotaFiscal.InformarDadosNotaCarga(pedxmlNotaFiscal.XMLNotaFiscal, cargaPedidoProximoTrecho, tipoServicoMultisoftware, out string msgAlerta);

                    if (cargaPedidoProximoTrecho.NotasFiscais?.Count > 0)//validar se pode ser assim.
                    {
                        cargaPedidoProximoTrecho.SituacaoEmissao = SituacaoNF.NFEnviada;
                        repCargaPedido.Atualizar(cargaPedidoProximoTrecho);
                    }
                }

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades> quantidadesExitentesproximoTrecho = repCargaPedidoQuantidades.BuscarPorCargaPedido(cargaPedido.Codigo);
                List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoSumarizadorQuantidadeNotasFiscais> sumarizadorQuantidadesCargasPedidosProximoTrecho = repPedidoXMLNotaFiscal.ObterSumarizacaoQuantidadesPorCarga(cargaPedido.CargaPedidoProximoTrecho.Carga);

                AdicionarCargaPedidoQuantidades(cargaPedido.CargaPedidoProximoTrecho, xMLNotaFiscals, notasPedidos, tipoServicoMultisoftware, configuracao, ref pesoProximoTrecho, ref pesoLiquidoProximoTrecho, naoAtualizarPesoPedidoPelaNFe, unitOfWork, sumarizadorQuantidadesCargasPedidosProximoTrecho, quantidadesExitentesproximoTrecho);

                cargaPedido.CargaPedidoProximoTrecho.Peso = pesoProximoTrecho;
                cargaPedido.CargaPedidoProximoTrecho.PesoLiquido = pesoLiquidoProximoTrecho;
                repCargaPedido.Atualizar(cargaPedido.CargaPedidoProximoTrecho);

                if (pesoProximoTrecho == 0 && cargaPedido.Peso > 0)
                    cargaPedido.CargaPedidoProximoTrecho.Peso = cargaPedido.Peso;

                if (pesoLiquidoProximoTrecho == 0 && cargaPedido.PesoLiquido > 0)
                    cargaPedido.CargaPedidoProximoTrecho.PesoLiquido = cargaPedido.PesoLiquido;

                Dominio.Entidades.Embarcador.Cargas.Carga cargaProximoTrecho = cargaPedido.CargaPedidoProximoTrecho.Carga;

                if (cargaProximoTrecho != null && (cargaProximoTrecho.SituacaoCarga == SituacaoCarga.AgTransportador || cargaProximoTrecho.SituacaoCarga == SituacaoCarga.AgNFe || cargaProximoTrecho.SituacaoCarga == SituacaoCarga.CalculoFrete))
                {
                    cargaProximoTrecho.SituacaoCarga = SituacaoCarga.CalculoFrete;
                    cargaProximoTrecho.DadosPagamentoInformadosManualmente = false;
                    cargaProximoTrecho.DataInicioCalculoFrete = DateTime.Now;
                    cargaProximoTrecho.CalculandoFrete = true;
                    cargaProximoTrecho.PendenciaEmissaoAutomatica = false;

                    repCarga.Atualizar(cargaProximoTrecho);
                }
            }
        }

        public async Task AdicionarCargaPedidoQuantidadesCarregamentoAsync(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> xMLNotaFiscals, List<Dominio.ObjetosDeValor.Embarcador.Carga.TotalizadorModeloDocumento> notasPedidos, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, decimal peso, decimal pesoLiquido, bool naoAtualizarPesoPedidoPelaNFe, Repositorio.UnitOfWork unitOfWork, List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoSumarizadorQuantidadeNotasFiscais> sumarizadorQuantidadesCargasPedidos = null, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades> cargaPedidoQuantidadesExitentes = null, bool inserir = true, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades> cargaPedidoQuantidadesDeletar = null, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades> cargaPedidoQuantidadesInserir = null)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades> cargaPedidoQuantidades = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades>();

            List<Dominio.ObjetosDeValor.Embarcador.Carga.TotalizadorModeloDocumento> notasFiscaisPedido = notasPedidos != null ? notasPedidos.Where(o => o.Codigo == cargaPedido.Pedido.Codigo).ToList() : new List<Dominio.ObjetosDeValor.Embarcador.Carga.TotalizadorModeloDocumento>();
            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> listaXMLNotasFiscaisPedido = notasFiscaisPedido.Count > 0 && xMLNotaFiscals != null ? xMLNotaFiscals.Where(o => notasFiscaisPedido.Select(p => p.Total).ToList().Contains(o.Codigo)).ToList() : new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

            List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoSumarizadorQuantidadeNotasFiscais> listaQuantidadesSumarizadasCargaPedido = sumarizadorQuantidadesCargasPedidos?.Where(o => o.CodigoCargaPedido == cargaPedido.Codigo).ToList();

            IList<int> notasFiscaisDuplicadas = new List<int>();
            if (configuracao.PermiteAdicionarNFeRepetidaParaOutroPedidoCarga)
            {
                notasFiscaisDuplicadas = repPedidoXMLNotaFiscal.BuscarListaCodigosNFeDuplicadaPorCarga(cargaPedido.Carga.Codigo);
                if (notasFiscaisDuplicadas.Count > 0)
                    notasFiscaisDuplicadas = repPedidoXMLNotaFiscal.BuscarNotasFiscaisPorNotasECargaPedido(notasFiscaisDuplicadas, cargaPedido.Codigo);
            }

            if (notasFiscaisDuplicadas.Count > 0 && listaXMLNotasFiscaisPedido.Count > 0)
                listaXMLNotasFiscaisPedido = listaXMLNotasFiscaisPedido.Where(o => !notasFiscaisDuplicadas.Contains(o.Codigo)).ToList();

            if (!naoAtualizarPesoPedidoPelaNFe || (cargaPedido.Peso == 0m && cargaPedido.Pedido.PesoTotal == 0m))
            {
                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> listaXMLNotasFiscaisPedidoPossuiSomentePallets = listaXMLNotasFiscaisPedido.Count > 0 ? listaXMLNotasFiscaisPedido.Where(o => configuracao.NaoUsarPesoNotasPallet ? o.TipoNotaFiscalIntegrada == TipoNotaFiscalIntegrada.RemessaPallet : 1 == 1).ToList() : new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> listaXMLNotasFiscaisPedidoNaoPossuiSomentePallets = listaXMLNotasFiscaisPedido.Count > 0 ? listaXMLNotasFiscaisPedido.Where(o => configuracao.NaoUsarPesoNotasPallet ? o.TipoNotaFiscalIntegrada != TipoNotaFiscalIntegrada.RemessaPallet : 1 == 1).ToList() : new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

                var pesoNotas = 0m;
                var pesoLiquidoNotas = 0m;

                if (listaXMLNotasFiscaisPedidoPossuiSomentePallets?.Count > 0 && listaXMLNotasFiscaisPedidoNaoPossuiSomentePallets?.Count > 0)
                    pesoNotas = listaXMLNotasFiscaisPedidoNaoPossuiSomentePallets.Sum(o => o.Peso);
                else if (listaXMLNotasFiscaisPedidoPossuiSomentePallets?.Count > 0 && listaXMLNotasFiscaisPedidoNaoPossuiSomentePallets?.Count == 0)
                    pesoNotas = listaXMLNotasFiscaisPedidoPossuiSomentePallets.Sum(o => o.Peso);
                else if (listaXMLNotasFiscaisPedidoNaoPossuiSomentePallets?.Count > 0)
                    pesoNotas = listaXMLNotasFiscaisPedidoNaoPossuiSomentePallets.Sum(o => o.Peso);

                if (listaXMLNotasFiscaisPedidoPossuiSomentePallets?.Count > 0 && listaXMLNotasFiscaisPedidoNaoPossuiSomentePallets?.Count > 0)
                    pesoLiquidoNotas = listaXMLNotasFiscaisPedidoNaoPossuiSomentePallets.Sum(o => o.PesoLiquido);
                else if (listaXMLNotasFiscaisPedidoPossuiSomentePallets?.Count > 0 && listaXMLNotasFiscaisPedidoNaoPossuiSomentePallets?.Count == 0)
                    pesoLiquidoNotas = listaXMLNotasFiscaisPedidoPossuiSomentePallets.Sum(o => o.PesoLiquido);
                else if (listaXMLNotasFiscaisPedidoNaoPossuiSomentePallets?.Count > 0)
                    pesoLiquidoNotas = listaXMLNotasFiscaisPedidoNaoPossuiSomentePallets.Sum(o => o.PesoLiquido);

                if (listaQuantidadesSumarizadasCargaPedido?.Count > 0 && (pesoNotas <= 0m || pesoLiquidoNotas <= 0m))
                {
                    List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoSumarizadorQuantidadeNotasFiscais> listaQuantidadesPeso = listaQuantidadesSumarizadasCargaPedido;

                    if (configuracao.NaoUsarPesoNotasPallet)
                        listaQuantidadesPeso = listaQuantidadesPeso.Where(o => o.TipoNotaFiscalIntegrada != TipoNotaFiscalIntegrada.RemessaPallet).ToList();

                    if (notasFiscaisDuplicadas.Count > 0)
                        listaQuantidadesPeso = listaQuantidadesPeso.Where(o => !notasFiscaisDuplicadas.Contains(o.CodigoXMLNotaFiscal)).ToList();

                    peso += pesoNotas > 0 ? pesoNotas : listaQuantidadesPeso.Sum(o => o.Peso);
                    pesoLiquido += pesoLiquidoNotas > 0 ? pesoLiquidoNotas : listaQuantidadesPeso.Sum(o => o.PesoLiquido);
                }
                else
                {
                    peso += pesoNotas > 0 ? pesoNotas : repPedidoXMLNotaFiscal.BuscarPesoPorCargaPedido(cargaPedido.Codigo, notasFiscaisDuplicadas, configuracao.NaoUsarPesoNotasPallet);
                    pesoLiquido += pesoLiquidoNotas > 0 ? pesoLiquidoNotas : repPedidoXMLNotaFiscal.BuscarPesoLiquidoPorCargaPedido(cargaPedido.Codigo, notasFiscaisDuplicadas, configuracao.NaoUsarPesoNotasPallet);
                }

                if (notasFiscaisDuplicadas.Count > 0)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasDuplicadas = xMLNotaFiscals.Where(x => notasFiscaisDuplicadas.Contains(x.Codigo)).ToList();
                    foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaDuplicada in notasDuplicadas)
                    {
                        decimal pesoPedido = configuracao.UtilizarPesoPedidoParaRatearPesoNFeRepetida ? cargaPedido.Pedido.PesoTotal : cargaPedido.Peso;

                        if ((pesoPedido > 0) && (notaDuplicada.Peso > 0))
                        {
                            decimal pesoTotalPedidos;

                            if (configuracao.UtilizarPesoPedidoParaRatearPesoNFeRepetida)
                                pesoTotalPedidos = repPedidoXMLNotaFiscal.BuscarPesoPedidosNotasDuplicas(notaDuplicada.Codigo, cargaPedido.Carga.Codigo);
                            else
                                pesoTotalPedidos = repPedidoXMLNotaFiscal.BuscarPesoCargaPedidosNotasDuplicas(notaDuplicada.Codigo, cargaPedido.Carga.Codigo);

                            if (pesoTotalPedidos > 0m)
                            {
                                decimal percentual = pesoPedido * 100 / pesoTotalPedidos;

                                peso += Math.Round(((notaDuplicada.Peso * percentual) / 100), 3);
                                pesoLiquido += Math.Round(((notaDuplicada.PesoLiquido * percentual) / 100), 3);
                            }
                        }
                    }
                }
            }
            else
            {
                peso = cargaPedido.Peso > 0 ? cargaPedido.Peso : cargaPedido.Pedido.PesoTotal;
                pesoLiquido = cargaPedido.PesoLiquido > 0 ? cargaPedido.PesoLiquido : cargaPedido.Pedido.PesoLiquidoTotal;
            }

            if (peso > 0m)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades cargaPedidoQuantidade = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades
                {
                    CargaPedido = cargaPedido,
                    Quantidade = peso,
                    Unidade = Dominio.Enumeradores.UnidadeMedida.KG
                };

                cargaPedidoQuantidades.Add(cargaPedidoQuantidade);

                if (!configuracao.AtualizarProdutosCarregamentoPorNota)
                {
                    cargaPedido.Peso = peso;
                    cargaPedido.PesoLiquido = pesoLiquido;
                }
            }

            int volumes = listaXMLNotasFiscaisPedido.Count > 0 ? listaXMLNotasFiscaisPedido.Sum(o => o.Volumes) : listaQuantidadesSumarizadasCargaPedido?.Count > 0 ? listaQuantidadesSumarizadasCargaPedido.Sum(o => o.Volumes) : repPedidoXMLNotaFiscal.BuscarVolumesPorCargaPedido(cargaPedido.Codigo);
            if (volumes > 0)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades cargaPedidoQuantidade = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades
                {
                    CargaPedido = cargaPedido,
                    Quantidade = volumes,
                    Unidade = Dominio.Enumeradores.UnidadeMedida.UN
                };

                cargaPedidoQuantidades.Add(cargaPedidoQuantidade);

                if (!naoAtualizarPesoPedidoPelaNFe)
                    cargaPedido.QtVolumes = volumes;

                AlterarDadosSumarizadosCargaPedido(cargaPedido, 0, volumes);
            }

            decimal metrosCubicos = listaXMLNotasFiscaisPedido.Count > 0 ? listaXMLNotasFiscaisPedido.Sum(o => o.MetrosCubicos) : listaQuantidadesSumarizadasCargaPedido?.Count > 0 ? listaQuantidadesSumarizadasCargaPedido.Sum(o => o.MetrosCubicos) : repPedidoXMLNotaFiscal.BuscarMetrosCubicosPorCargaPedido(cargaPedido.Codigo);
            if (metrosCubicos == 0 && configuracao.UsarCubagemPedidoSeNFeSemCubagem)
                metrosCubicos = repCargaPedido.BuscarCubagemTotalPorCargaPedido(cargaPedido.Codigo);

            decimal pesoCubado = listaXMLNotasFiscaisPedido.Count > 0 ? listaXMLNotasFiscaisPedido.Sum(o => o.PesoCubado) : listaQuantidadesSumarizadasCargaPedido?.Count > 0 ? listaQuantidadesSumarizadasCargaPedido.Sum(o => o.PesoCubado) : repPedidoXMLNotaFiscal.BuscarPesoCubadoPorCargaPedido(cargaPedido.Codigo);
            decimal cubagemPedido = cargaPedido.Pedido.CubagemTotal;

            if (metrosCubicos > 0 || pesoCubado > 0 || cubagemPedido > 0)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades cargaPedidoQuantidade = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades
                {
                    CargaPedido = cargaPedido,
                    Quantidade = metrosCubicos > 0 ? metrosCubicos : pesoCubado > 0 ? pesoCubado : cubagemPedido,
                    Unidade = Dominio.Enumeradores.UnidadeMedida.M3
                };

                cargaPedidoQuantidades.Add(cargaPedidoQuantidade);
            }

            Repositorio.Embarcador.Cargas.CargaPedidoQuantidades repCargaPedidoQuantidades = new Repositorio.Embarcador.Cargas.CargaPedidoQuantidades(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades> quantidadesExitentes = cargaPedidoQuantidadesExitentes?.Count > 0 ? cargaPedidoQuantidadesExitentes.Where(o => o.CargaPedido.Codigo == cargaPedido.Codigo).ToList() : repCargaPedidoQuantidades.BuscarPorCargaPedido(cargaPedido.Codigo);

            if (inserir)
            {
                if (quantidadesExitentes.Count > 0)
                    repCargaPedidoQuantidades.Deletar(quantidadesExitentes, "T_CARGA_PEDIDO_QUANTIDADES");

                if (cargaPedidoQuantidades.Count > 0) repCargaPedidoQuantidades.Inserir(cargaPedidoQuantidades, "T_CARGA_PEDIDO_QUANTIDADES");
            }
            else
            {
                cargaPedidoQuantidadesDeletar.AddRange(quantidadesExitentes);
                cargaPedidoQuantidadesInserir.AddRange(cargaPedidoQuantidades);
            }
        }

        public static bool Duplicar(out string erro, out Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoDuplicado, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, string numeroPedido, Repositorio.UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            if (cargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe && cargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova)
            {
                cargaPedidoDuplicado = null;
                erro = "A situação atual da carga não permite que seja adicionado um novo pedido à mesma.";
                return false;
            }

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoClonado = cargaPedido.Pedido.Clonar();
            cargaPedido.Pedido.ControleNumeracao = cargaPedido.Pedido.Codigo;
            repPedido.Atualizar(cargaPedido.Pedido);
            Utilidades.Object.DefinirListasGenericasComoNulas(pedidoClonado);

            pedidoClonado.SituacaoAcompanhamentoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcompanhamentoPedido.AgColeta;
            pedidoClonado.ValorFreteNegociado = 0m;
            pedidoClonado.ValorFreteToneladaNegociado = 0m;
            pedidoClonado.Numero = repPedido.BuscarProximoNumero();
            pedidoClonado.NumeroPedidoEmbarcador = numeroPedido;
            pedidoClonado.AdicionadaManualmente = true;

            if (pedidoClonado.Recebedor != null)
                pedidoClonado.Destino = pedidoClonado.Recebedor.Localidade;
            else if (pedidoClonado.Destinatario != null)
                pedidoClonado.Destino = pedidoClonado.Destinatario.Localidade;

            if (pedidoClonado.Expedidor != null)
                pedidoClonado.Origem = pedidoClonado.Expedidor.Localidade;
            else if (pedidoClonado.Remetente != null)
                pedidoClonado.Origem = pedidoClonado.Remetente.Localidade;

            repPedido.Inserir(pedidoClonado);

            cargaPedidoDuplicado = CriarCargaPedido(cargaPedido.Carga, pedidoClonado, null, unitOfWork, stringConexao, tipoServicoMultisoftware, configuracao, false, configuracaoGeralCarga);

            cargaPedidoDuplicado.TipoContratacaoCarga = cargaPedido.TipoContratacaoCarga;

            repCargaPedido.Atualizar(cargaPedidoDuplicado);

            erro = string.Empty;
            return true;
        }

        public void AdicionarCargaPedidoQuantidades(List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades> cargaPedidoQuantidades, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoQuantidades repCargaPedidoQuantidades = new Repositorio.Embarcador.Cargas.CargaPedidoQuantidades(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades> cargaPedidoQuantidadesExitentes = repCargaPedidoQuantidades.BuscarPorCargaPedido(cargaPedido.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades cargaPedidoQuantidadeExiste in cargaPedidoQuantidadesExitentes)
                repCargaPedidoQuantidades.Deletar(cargaPedidoQuantidadeExiste);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades quantidade in cargaPedidoQuantidades)
                repCargaPedidoQuantidades.Inserir(quantidade);
        }

        public void CriarUnidadesDeMedidaDaCargaPedido(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, decimal peso, int volumes, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Carga.CargaPedido serCargaPedido = new Servicos.Embarcador.Carga.CargaPedido(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades> cargaPedidoQuantidades = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades>();
            if (peso > 0)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades cargaPedidoQuantidade = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades();
                cargaPedidoQuantidade.CargaPedido = cargaPedido;
                cargaPedidoQuantidade.Quantidade = peso;
                cargaPedidoQuantidade.Unidade = Dominio.Enumeradores.UnidadeMedida.KG;
                cargaPedidoQuantidades.Add(cargaPedidoQuantidade);

            }
            if (volumes > 0)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades cargaPedidoQuantidade = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades();
                cargaPedidoQuantidade.CargaPedido = cargaPedido;
                cargaPedidoQuantidade.Quantidade = volumes;
                cargaPedidoQuantidade.Unidade = Dominio.Enumeradores.UnidadeMedida.UN;
                cargaPedidoQuantidades.Add(cargaPedidoQuantidade);
            }
            serCargaPedido.AdicionarCargaPedidoQuantidades(cargaPedidoQuantidades, cargaPedido, unitOfWork);
        }

        public static bool ValidarNumeroPedidoEmbarcador(out string erro, string numeroPedido, Dominio.Entidades.Cliente clienteTomador, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, bool validacaoCodigoCliente = false)
        {
            if (validacaoCodigoCliente)
            {
                bool validarNumero = false;
                if (tipoOperacao != null && tipoOperacao.UsarConfiguracaoEmissao)
                {
                    validarNumero = tipoOperacao.ExigirNumeroPedido;
                }
                else if (clienteTomador != null)
                {
                    if (clienteTomador.NaoUsarConfiguracaoEmissaoGrupo)
                    {
                        validarNumero = clienteTomador.ExigirNumeroPedido;
                    }
                    else if (clienteTomador.GrupoPessoas != null)
                    {
                        validarNumero = clienteTomador.GrupoPessoas.ExigirNumeroPedido;
                    }
                }
                if (validarNumero && string.IsNullOrWhiteSpace(numeroPedido))
                {
                    erro = "O número do pedido do cliente não foi informado.";
                    return false;
                }
            }
            else
            {
                string regexNumeroPedidoEmbarcador = null;
                bool exigirNumeroPedido = false;

                if (tipoOperacao != null && tipoOperacao.UsarConfiguracaoEmissao)
                {
                    exigirNumeroPedido = tipoOperacao.ExigirNumeroPedido;
                    regexNumeroPedidoEmbarcador = tipoOperacao.RegexValidacaoNumeroPedidoEmbarcador;
                }
                else if (clienteTomador != null)
                {
                    if (clienteTomador.NaoUsarConfiguracaoEmissaoGrupo)
                    {
                        exigirNumeroPedido = clienteTomador.ExigirNumeroPedido;
                        regexNumeroPedidoEmbarcador = clienteTomador.RegexValidacaoNumeroPedidoEmbarcador;
                    }
                    else if (clienteTomador.GrupoPessoas != null)
                    {
                        exigirNumeroPedido = clienteTomador.GrupoPessoas.ExigirNumeroPedido;
                        regexNumeroPedidoEmbarcador = clienteTomador.GrupoPessoas.RegexValidacaoNumeroPedidoEmbarcador;
                    }
                }

                if (!exigirNumeroPedido)
                {
                    erro = string.Empty;
                    return true;
                }

                if (!string.IsNullOrWhiteSpace(regexNumeroPedidoEmbarcador))
                {
                    try
                    {
                        if (!System.Text.RegularExpressions.Regex.IsMatch(numeroPedido, regexNumeroPedidoEmbarcador))
                        {
                            Fare.Xeger xeger = new Fare.Xeger(regexNumeroPedidoEmbarcador);

                            string exemplo = xeger.Generate();

                            erro = "O número do pedido no embarcador não foi informado de acordo com o padrão definido pelo embarcador. Exemplo: " + exemplo + ".";
                            return false;
                        }
                    }
                    catch
                    {
                        erro = "A configuração do padrão para validação do número do pedido do embarcador é inválida: " + regexNumeroPedidoEmbarcador;
                        return false;
                    }
                }
                else if (string.IsNullOrWhiteSpace(numeroPedido))
                {
                    erro = "O número do pedido do embarcador é obrigatório.";
                    return false;
                }
            }

            erro = string.Empty;
            return true;
        }

        public static bool ValidarEmailTomador(out string erro, Dominio.Entidades.Cliente tomador)
        {
            bool valido = false;

            if ((tomador.GrupoPessoas?.EnviarXMLCTePorEmail ?? false) && !string.IsNullOrWhiteSpace(tomador.GrupoPessoas.Email))
                valido = true;

            if (tomador.EmailStatus == "A" && !string.IsNullOrWhiteSpace(tomador.Email))
                valido = true;

            if (!valido)
            {
                erro = $"O tomador {tomador.Descricao} não possui um e-mail configurado para envio da documentação.";
                return false;
            }

            erro = string.Empty;
            return true;
        }

        public TipoEmissaoIntramunicipal ObterTipoEmissaoIntramunicipalCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Localidade origem, int codigoTipoOperacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            bool existeOperacaoIntermunicipalNaCargaDeCobertura = (carga.TipoOperacao?.ConfiguracaoCalculoFrete?.UtilizarCoberturaDeCarga ?? false) && repCargaPedido.ExisteOperacaoIntermunicipalNaCarga(carga.Codigo);

            TipoEmissaoIntramunicipal tipoOperacaoMunicipal = TipoEmissaoIntramunicipal.NaoEspecificado;

            if (carga.Empresa != null && carga.TipoOperacao != null)
            {
                Dominio.Entidades.Embarcador.Transportadores.ConfiguracaoTipoOperacao configuracao = (from obj in carga.Empresa.ConfiguracoesTipoOperacao where obj.TipoOperacao.Codigo == codigoTipoOperacao select obj).FirstOrDefault();
                if (configuracao != null)
                    tipoOperacaoMunicipal = configuracao.TipoEmissaoIntramunicipal;

                if (tipoOperacaoMunicipal == TipoEmissaoIntramunicipal.NaoEspecificado)
                    tipoOperacaoMunicipal = carga.Empresa.TipoEmissaoIntramunicipal;
            }

            if (tipoOperacaoMunicipal == TipoEmissaoIntramunicipal.NaoEspecificado)
            {
                if (carga.TipoOperacao != null && carga.TipoOperacao.UsarConfiguracaoEmissao)
                    tipoOperacaoMunicipal = carga.TipoOperacao.TipoEmissaoIntramunicipal;
            }

            if (tipoOperacaoMunicipal == TipoEmissaoIntramunicipal.NaoEspecificado)
            {
                if (carga.Empresa != null && carga.Empresa.Localidade.Codigo == origem.Codigo)
                    tipoOperacaoMunicipal = origem.TipoEmissaoIntramunicipal;//Essa regra é cadastrada direto na localidade e é valida apenas para multiembarcador.
            }

            if (existeOperacaoIntermunicipalNaCargaDeCobertura)
                tipoOperacaoMunicipal = TipoEmissaoIntramunicipal.SempreCTe;

            return tipoOperacaoMunicipal;
        }

        public void VerificarQuaisDocumentosDeveEmitir(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Localidade origem, Dominio.Entidades.Localidade destino, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, out bool possuiCTe, out bool possuiNFS, out bool possuiNFSManual, out Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscalIntramunicipal, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, out bool sempreDisponibilizarDocumentoNFSManual, bool validarTipoOperacaoMunicipal = false)
        {
            /// validarTipoOperacaoMunicipal :Colocado pra não gerar hora gerar carga com precalculo. Unilever

            possuiNFS = false;
            possuiCTe = false;
            possuiNFSManual = false;
            sempreDisponibilizarDocumentoNFSManual = false;
            modeloDocumentoFiscalIntramunicipal = null;

            bool utilizarOutroModeloIntramunicipal = false;
            Dominio.Entidades.ModeloDocumentoFiscal modeloIntramunicipal = null;

            Dominio.Entidades.Cliente tomador = cargaPedido.ObterTomador();

            if (carga.TipoOperacao != null && carga.TipoOperacao.UsarConfiguracaoEmissao)
            {
                if (carga.TipoOperacao?.ModeloDocumentoFiscal != null && carga.TipoOperacao.DisponibilizarDocumentosParaNFsManual)
                {
                    sempreDisponibilizarDocumentoNFSManual = true;
                    modeloDocumentoFiscalIntramunicipal = carga.TipoOperacao.ModeloDocumentoFiscal;
                }
            }
            else if (tomador != null)
            {
                if (tomador.NaoUsarConfiguracaoEmissaoGrupo)
                {
                    if (tomador.ModeloDocumentoFiscal != null && tomador.DisponibilizarDocumentosParaNFsManual)
                    {
                        sempreDisponibilizarDocumentoNFSManual = true;
                        modeloDocumentoFiscalIntramunicipal = tomador.ModeloDocumentoFiscal;
                    }
                }
                else if (tomador.GrupoPessoas != null)
                {
                    if (tomador.GrupoPessoas.ModeloDocumentoFiscal != null && tomador.GrupoPessoas.DisponibilizarDocumentosParaNFsManual)
                    {
                        sempreDisponibilizarDocumentoNFSManual = true;
                        modeloDocumentoFiscalIntramunicipal = tomador.GrupoPessoas.ModeloDocumentoFiscal;
                    }
                }
            }

            if (sempreDisponibilizarDocumentoNFSManual)
            {
                possuiNFSManual = true;
            }
            else if (destino != null && origem != null && (((origem.Codigo == destino.Codigo || (carga.Empresa?.SempreEmitirNFS ?? false)) && (!carga.EmitirCTeComplementar || cargaPedido.PossuiNFS || cargaPedido.PossuiNFSManual || cargaPedido.ModeloDocumentoFiscal?.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe || cargaPedido.ModeloDocumentoFiscal?.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS)) || (carga.EmitirCTeComplementar && (cargaPedido.PossuiNFS || cargaPedido.PossuiNFSManual))) && (cargaPedido.ModeloDocumentoFiscal == null || cargaPedido.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.Outros))
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal tipoOperacaoMunicipal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal.NaoEspecificado;

                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    if (tomador != null)
                    {
                        if (carga.TipoOperacao != null && carga.TipoOperacao.UsarConfiguracaoEmissao)
                        {
                            tipoOperacaoMunicipal = carga.TipoOperacao.TipoEmissaoIntramunicipal;
                            utilizarOutroModeloIntramunicipal = carga.TipoOperacao.UtilizarOutroModeloDocumentoEmissaoMunicipal;
                            modeloIntramunicipal = carga.TipoOperacao.ModeloDocumentoFiscalEmissaoMunicipal;
                        }
                        else if (tomador.NaoUsarConfiguracaoEmissaoGrupo)
                        {
                            tipoOperacaoMunicipal = tomador.TipoEmissaoIntramunicipal;
                            utilizarOutroModeloIntramunicipal = tomador.UtilizarOutroModeloDocumentoEmissaoMunicipal;
                            modeloIntramunicipal = tomador.ModeloDocumentoFiscalEmissaoMunicipal;
                        }
                        else if (tomador.GrupoPessoas != null)
                        {
                            tipoOperacaoMunicipal = tomador.GrupoPessoas.TipoEmissaoIntramunicipal;
                            utilizarOutroModeloIntramunicipal = tomador.GrupoPessoas.UtilizarOutroModeloDocumentoEmissaoMunicipal;
                            modeloIntramunicipal = tomador.GrupoPessoas.ModeloDocumentoFiscalEmissaoMunicipal;
                        }
                    }
                }
                else
                {
                    tipoOperacaoMunicipal = ObterTipoEmissaoIntramunicipalCarga(carga, origem, cargaPedido.Carga.TipoOperacao?.Codigo ?? 0, unitOfWork);
                }

                if (tipoOperacaoMunicipal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal.NaoEspecificado)
                    tipoOperacaoMunicipal = configuracaoTMS.TipoEmissaoIntramunicipal;

                if (cargaPedido.PedidoPallet)//todo:regra fixa piracanjuba, rever se necessário.
                    tipoOperacaoMunicipal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal.SempreCTe;

                if (cargaPedido.Pedido.TipoPedido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedido.Entrega)
                    tipoOperacaoMunicipal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal.NaoEmiteNenhumDocumento;

                Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe transportadorConfiguracaoNFSe = null;

                switch (tipoOperacaoMunicipal)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal.NaoEspecificado:
                        possuiCTe = true;
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal.SempreNFSe:
                        possuiNFS = true;
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal.SempreCTe:
                        possuiCTe = true;
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal.SempreNFSManual:
                        possuiNFSManual = true;

                        if (utilizarOutroModeloIntramunicipal)
                            modeloDocumentoFiscalIntramunicipal = modeloIntramunicipal;

                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal.NFEsApenasEmpresasAptasDemaisCTe:
                        if (carga.Empresa != null)
                        {
                            if (configuracaoTMS.ConfiguracaoTabelaFrete?.ExigirAliquotaNoMunicipioDePrestacaoParaCalculoDeFreteEmFretesMunicipais ?? false)
                            {
                                transportadorConfiguracaoNFSe = (from obj in carga.Empresa.TransportadorConfiguracoesNFSe where obj.LocalidadePrestacao?.Codigo == origem.Codigo select obj).FirstOrDefault();

                                if ((transportadorConfiguracaoNFSe?.Codigo ?? 0) == 0 && !validarTipoOperacaoMunicipal)
                                    throw new ServicoException("Não foi possível realizar o Cálculo de Frete devido a não localizar configurações De NFS-E no cadastro do Transportador.");
                            }
                            else
                                transportadorConfiguracaoNFSe = (from obj in carga.Empresa.TransportadorConfiguracoesNFSe where obj.LocalidadePrestacao == null || obj.LocalidadePrestacao?.Codigo == origem.Codigo select obj).FirstOrDefault();

                            if (transportadorConfiguracaoNFSe != null && !transportadorConfiguracaoNFSe.ConfiguracaoParaProvisaoDeISS)
                                possuiNFS = true;
                            else
                                possuiCTe = true;
                        }
                        else
                        {
                            possuiNFS = true;
                        }
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal.NFEsApenasEmpresasAptasDemaisNaoEmiteNenhumDocumento:
                        if (carga.Empresa != null)
                        {
                            if (configuracaoTMS.ConfiguracaoTabelaFrete?.ExigirAliquotaNoMunicipioDePrestacaoParaCalculoDeFreteEmFretesMunicipais ?? false)
                            {
                                transportadorConfiguracaoNFSe = (from obj in carga.Empresa.TransportadorConfiguracoesNFSe where obj.LocalidadePrestacao?.Codigo == origem.Codigo select obj).FirstOrDefault();

                                if ((transportadorConfiguracaoNFSe?.Codigo ?? 0) == 0 && !validarTipoOperacaoMunicipal)
                                    throw new ServicoException("Não foi possível realizar o Cálculo de Frete devido a não localizar configurações De NFS-E no cadastro do Transportador.");
                            }
                            else
                                transportadorConfiguracaoNFSe = (from obj in carga.Empresa.TransportadorConfiguracoesNFSe where obj.LocalidadePrestacao == null || obj.LocalidadePrestacao?.Codigo == origem.Codigo select obj).FirstOrDefault();

                            if (transportadorConfiguracaoNFSe != null && !transportadorConfiguracaoNFSe.ConfiguracaoParaProvisaoDeISS)
                                possuiNFS = true;
                        }
                        else
                        {
                            possuiNFS = true;
                        }
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal.NFEsApenasEmpresasAptasDemaisNFsManual:
                        if (carga.Empresa != null)
                        {
                            if (configuracaoTMS.ConfiguracaoTabelaFrete?.ExigirAliquotaNoMunicipioDePrestacaoParaCalculoDeFreteEmFretesMunicipais ?? false)
                            {
                                transportadorConfiguracaoNFSe = (from obj in carga.Empresa.TransportadorConfiguracoesNFSe where obj.LocalidadePrestacao?.Codigo == origem.Codigo select obj).FirstOrDefault();

                                if ((transportadorConfiguracaoNFSe?.Codigo ?? 0) == 0 && !validarTipoOperacaoMunicipal)
                                    throw new ServicoException("Não foi possível realizar o Cálculo de Frete por não localizar uma configuração de NFS-E compatível no cadastro do Transportador");
                            }
                            else
                                transportadorConfiguracaoNFSe = (from obj in carga.Empresa.TransportadorConfiguracoesNFSe where obj.LocalidadePrestacao == null || obj.LocalidadePrestacao?.Codigo == origem.Codigo select obj).FirstOrDefault();

                            if (transportadorConfiguracaoNFSe != null)
                                possuiNFS = true;
                            else
                                possuiNFSManual = true;
                        }
                        else
                        {
                            possuiNFS = true;
                        }
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal.NaoEmiteNenhumDocumento:
                        break;
                    default:
                        possuiCTe = true;
                        break;
                }
            }
            else
            {
                possuiCTe = true;
            }

            if (cargaPedido.ModeloDocumentoEmpresaPropria)
            {
                cargaPedido.ModeloDocumentoFiscal = null;
                cargaPedido.ModeloDocumentoEmpresaPropria = false;
            }

            if ((carga.Empresa != null && (carga.Empresa.EmpresaPropria || carga.Empresa.ModeloDocumentoFiscalCargaPropria != null)) || (carga.TipoOperacao != null && carga.TipoOperacao.CargaPropria))
            {
                if (carga.Empresa?.ModeloDocumentoFiscalCargaPropria != null)
                {
                    possuiCTe = true;
                    cargaPedido.ModeloDocumentoFiscal = carga.Empresa.ModeloDocumentoFiscalCargaPropria;
                    cargaPedido.ModeloDocumentoEmpresaPropria = true;
                }
                else
                    possuiCTe = false;

                possuiNFS = false;
            }
        }

        public async Task<Dominio.ObjetosDeValor.Embarcador.Carga.RetornoVerificacaoDocumentoDeveEmitir> VerificarQuaisDocumentosDeveEmitirPorTomadorCodigoAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Cliente tomador, Dominio.Entidades.Localidade origem, Dominio.Entidades.Localidade destino, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, bool validarTipoOperacaoMunicipal = false)
        {

            Dominio.ObjetosDeValor.Embarcador.Carga.RetornoVerificacaoDocumentoDeveEmitir retorno = new Dominio.ObjetosDeValor.Embarcador.Carga.RetornoVerificacaoDocumentoDeveEmitir();
            Dominio.Entidades.ModeloDocumentoFiscal modeloIntramunicipal = null;

            bool utilizarOutroModeloIntramunicipal = false;

            if (carga.TipoOperacao != null && carga.TipoOperacao.UsarConfiguracaoEmissao)
            {
                if (carga.TipoOperacao?.ModeloDocumentoFiscal != null && carga.TipoOperacao.DisponibilizarDocumentosParaNFsManual)
                {
                    retorno.SempreDisponibilizarDocumentoNFSManual = true;
                    retorno.ModeloDocumentoFiscalIntramunicipal = carga.TipoOperacao.ModeloDocumentoFiscal;
                }
            }
            else if (tomador != null)
            {
                if (tomador.NaoUsarConfiguracaoEmissaoGrupo)
                {
                    if (tomador.ModeloDocumentoFiscal != null && tomador.DisponibilizarDocumentosParaNFsManual)
                    {
                        retorno.SempreDisponibilizarDocumentoNFSManual = true;
                        retorno.ModeloDocumentoFiscalIntramunicipal = tomador.ModeloDocumentoFiscal;
                    }
                }
                else if (tomador.GrupoPessoas != null && tomador.GrupoPessoas.ModeloDocumentoFiscal != null && tomador.GrupoPessoas.DisponibilizarDocumentosParaNFsManual)
                {
                    retorno.SempreDisponibilizarDocumentoNFSManual = true;
                    retorno.ModeloDocumentoFiscalIntramunicipal = tomador.GrupoPessoas.ModeloDocumentoFiscal;
                }
            }

            if (retorno.SempreDisponibilizarDocumentoNFSManual)
            {
                retorno.PossuiNFSManual = true;
            }
            else if (destino != null && origem != null && (((origem.Codigo == destino.Codigo || (carga.Empresa?.SempreEmitirNFS ?? false)) && (!carga.EmitirCTeComplementar || cargaPedido.PossuiNFS || cargaPedido.PossuiNFSManual || cargaPedido.ModeloDocumentoFiscal?.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe || cargaPedido.ModeloDocumentoFiscal?.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS)) || (carga.EmitirCTeComplementar && (cargaPedido.PossuiNFS || cargaPedido.PossuiNFSManual))) && (cargaPedido.ModeloDocumentoFiscal == null || cargaPedido.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.Outros))
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal tipoOperacaoMunicipal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal.NaoEspecificado;

                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    if (tomador != null)
                    {
                        if (carga.TipoOperacao != null && carga.TipoOperacao.UsarConfiguracaoEmissao)
                        {
                            tipoOperacaoMunicipal = carga.TipoOperacao.TipoEmissaoIntramunicipal;
                            utilizarOutroModeloIntramunicipal = carga.TipoOperacao.UtilizarOutroModeloDocumentoEmissaoMunicipal;
                            modeloIntramunicipal = carga.TipoOperacao.ModeloDocumentoFiscalEmissaoMunicipal;
                        }
                        else if (tomador.NaoUsarConfiguracaoEmissaoGrupo)
                        {
                            tipoOperacaoMunicipal = tomador.TipoEmissaoIntramunicipal;
                            utilizarOutroModeloIntramunicipal = tomador.UtilizarOutroModeloDocumentoEmissaoMunicipal;
                            modeloIntramunicipal = tomador.ModeloDocumentoFiscalEmissaoMunicipal;
                        }
                        else if (tomador.GrupoPessoas != null)
                        {
                            tipoOperacaoMunicipal = tomador.GrupoPessoas.TipoEmissaoIntramunicipal;
                            utilizarOutroModeloIntramunicipal = tomador.GrupoPessoas.UtilizarOutroModeloDocumentoEmissaoMunicipal;
                            modeloIntramunicipal = tomador.GrupoPessoas.ModeloDocumentoFiscalEmissaoMunicipal;
                        }
                    }
                }
                else
                {
                    tipoOperacaoMunicipal = ObterTipoEmissaoIntramunicipalCarga(carga, origem, cargaPedido.Carga.TipoOperacao?.Codigo ?? 0, unitOfWork);
                }

                if (tipoOperacaoMunicipal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal.NaoEspecificado)
                    tipoOperacaoMunicipal = configuracaoTMS.TipoEmissaoIntramunicipal;

                if (cargaPedido.PedidoPallet)
                    tipoOperacaoMunicipal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal.SempreCTe;

                if (cargaPedido.Pedido.TipoPedido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedido.Entrega)
                    tipoOperacaoMunicipal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal.NaoEmiteNenhumDocumento;

                Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe transportadorConfiguracaoNFSe = null;

                switch (tipoOperacaoMunicipal)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal.NaoEspecificado:
                        retorno.PossuiCTe = true;
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal.SempreNFSe:
                        retorno.PossuiNFS = true;
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal.SempreCTe:
                        retorno.PossuiCTe = true;
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal.SempreNFSManual:
                        retorno.PossuiNFSManual = true;

                        if (utilizarOutroModeloIntramunicipal)
                            retorno.ModeloDocumentoFiscalIntramunicipal = modeloIntramunicipal;

                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal.NFEsApenasEmpresasAptasDemaisCTe:
                        if (carga.Empresa != null)
                        {
                            if (configuracaoTMS.ConfiguracaoTabelaFrete?.ExigirAliquotaNoMunicipioDePrestacaoParaCalculoDeFreteEmFretesMunicipais ?? false)
                            {
                                transportadorConfiguracaoNFSe = (from obj in carga.Empresa.TransportadorConfiguracoesNFSe where obj.LocalidadePrestacao?.Codigo == origem.Codigo select obj).FirstOrDefault();

                                if ((transportadorConfiguracaoNFSe?.Codigo ?? 0) == 0 && !validarTipoOperacaoMunicipal)
                                    throw new ServicoException("Não foi possível realizar o Cálculo de Frete devido a não localizar configurações De NFS-E no cadastro do Transportador.");
                            }
                            else
                                transportadorConfiguracaoNFSe = (from obj in carga.Empresa.TransportadorConfiguracoesNFSe where obj.LocalidadePrestacao == null || obj.LocalidadePrestacao?.Codigo == origem.Codigo select obj).FirstOrDefault();

                            if (transportadorConfiguracaoNFSe != null && !transportadorConfiguracaoNFSe.ConfiguracaoParaProvisaoDeISS)
                                retorno.PossuiNFS = true;
                            else
                                retorno.PossuiCTe = true;
                        }
                        else
                        {
                            retorno.PossuiNFS = true;
                        }
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal.NFEsApenasEmpresasAptasDemaisNaoEmiteNenhumDocumento:
                        if (carga.Empresa != null)
                        {
                            if (configuracaoTMS.ConfiguracaoTabelaFrete?.ExigirAliquotaNoMunicipioDePrestacaoParaCalculoDeFreteEmFretesMunicipais ?? false)
                            {
                                transportadorConfiguracaoNFSe = (from obj in carga.Empresa.TransportadorConfiguracoesNFSe where obj.LocalidadePrestacao?.Codigo == origem.Codigo select obj).FirstOrDefault();

                                if ((transportadorConfiguracaoNFSe?.Codigo ?? 0) == 0 && !validarTipoOperacaoMunicipal)
                                    throw new ServicoException("Não foi possível realizar o Cálculo de Frete devido a não localizar configurações De NFS-E no cadastro do Transportador.");
                            }
                            else
                                transportadorConfiguracaoNFSe = (from obj in carga.Empresa.TransportadorConfiguracoesNFSe where obj.LocalidadePrestacao == null || obj.LocalidadePrestacao?.Codigo == origem.Codigo select obj).FirstOrDefault();

                            if (transportadorConfiguracaoNFSe != null && !transportadorConfiguracaoNFSe.ConfiguracaoParaProvisaoDeISS)
                                retorno.PossuiNFS = true;
                        }
                        else
                        {
                            retorno.PossuiNFS = true;
                        }
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal.NFEsApenasEmpresasAptasDemaisNFsManual:
                        if (carga.Empresa != null)
                        {
                            if (configuracaoTMS.ConfiguracaoTabelaFrete?.ExigirAliquotaNoMunicipioDePrestacaoParaCalculoDeFreteEmFretesMunicipais ?? false)
                            {
                                transportadorConfiguracaoNFSe = (from obj in carga.Empresa.TransportadorConfiguracoesNFSe where obj.LocalidadePrestacao?.Codigo == origem.Codigo select obj).FirstOrDefault();

                                if ((transportadorConfiguracaoNFSe?.Codigo ?? 0) == 0 && !validarTipoOperacaoMunicipal)
                                    throw new ServicoException("Não foi possível realizar o Cálculo de Frete por não localizar uma configuração de NFS-E compatível no cadastro do Transportador");
                            }
                            else
                                transportadorConfiguracaoNFSe = (from obj in carga.Empresa.TransportadorConfiguracoesNFSe where obj.LocalidadePrestacao == null || obj.LocalidadePrestacao?.Codigo == origem.Codigo select obj).FirstOrDefault();

                            if (transportadorConfiguracaoNFSe != null)
                                retorno.PossuiNFS = true;
                            else
                                retorno.PossuiNFSManual = true;
                        }
                        else
                        {
                            retorno.PossuiNFS = true;
                        }
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal.NaoEmiteNenhumDocumento:
                        break;
                    default:
                        retorno.PossuiCTe = true;
                        break;
                }
            }
            else
            {
                retorno.PossuiCTe = true;
            }

            if (cargaPedido.ModeloDocumentoEmpresaPropria)
            {
                cargaPedido.ModeloDocumentoFiscal = null;
                cargaPedido.ModeloDocumentoEmpresaPropria = false;
            }

            if ((carga.Empresa != null && (carga.Empresa.EmpresaPropria || carga.Empresa.ModeloDocumentoFiscalCargaPropria != null)) || (carga.TipoOperacao != null && carga.TipoOperacao.CargaPropria))
            {
                if (carga.Empresa?.ModeloDocumentoFiscalCargaPropria != null)
                {
                    retorno.PossuiCTe = true;
                    cargaPedido.ModeloDocumentoFiscal = carga.Empresa.ModeloDocumentoFiscalCargaPropria;
                    cargaPedido.ModeloDocumentoEmpresaPropria = true;
                }
                else
                    retorno.PossuiCTe = false;

                retorno.PossuiNFS = false;
            }

            return retorno;
        }

        public static void SetarModeloDocumentoEmitir(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Repositorio.UnitOfWork unitOfWork)
        {
            cargaPedido.ModeloDocumentoFiscal = null;

            if (cargaPedido.Carga.TipoOperacao != null && cargaPedido.Carga.TipoOperacao.UsarConfiguracaoEmissao)
                cargaPedido.ModeloDocumentoFiscal = cargaPedido.Carga.TipoOperacao.ModeloDocumentoFiscal;
            else
            {
                Dominio.Entidades.Cliente tomador = cargaPedido.ObterTomador();
                if (tomador != null)
                {
                    if (tomador.NaoUsarConfiguracaoEmissaoGrupo)
                        cargaPedido.ModeloDocumentoFiscal = tomador.ModeloDocumentoFiscal;
                    else if (tomador.GrupoPessoas != null)
                        cargaPedido.ModeloDocumentoFiscal = tomador.GrupoPessoas.ModeloDocumentoFiscal;
                }
            }
        }

        public void SetarDestinatarioFinalCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido)
        {
            if (cargaPedidos.Count != 1)
                return;

            if (configuracaoPedido.SempreConsiderarDestinatarioInformadoNoPedido)
                return;

            Servicos.Embarcador.Pedido.Pedido serPedido = new Servicos.Embarcador.Pedido.Pedido(unitOfWork);

            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPercurso repCargaPercurso = new Repositorio.Embarcador.Cargas.CargaPercurso(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoEndereco repPedidoEndereco = new Repositorio.Embarcador.Pedidos.PedidoEndereco(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaPercurso cargaPercurso = repCargaPercurso.BuscarUltimaEntrega(carga.Codigo);
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaPedidos.FirstOrDefault();
            Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = null;

            if (cargaPedido.Pedido.Destinatario != null)
            {
                if (cargaPedido.TipoRateio == TipoEmissaoCTeDocumentos.EmitePorNotaFiscalAgrupada || cargaPedido.TipoRateio == TipoEmissaoCTeDocumentos.EmitePorNotaFiscalIndividual)
                    pedidoXMLNotaFiscal = repPedidoXMLNotaFiscal.BuscarNotaPorDestinatario(cargaPedido.Codigo, cargaPedido.Pedido.Destinatario.CPF_CNPJ);
                else if (cargaPercurso != null)
                    pedidoXMLNotaFiscal = repPedidoXMLNotaFiscal.BuscarNotaPorLocalidadeEDestintario(cargaPedido.Codigo, cargaPercurso.Destino.Codigo, cargaPedido.Pedido.Destinatario.CPF_CNPJ);
            }

            if (pedidoXMLNotaFiscal == null && !(carga.TipoOperacao?.GerarCTeComplementarNaCarga ?? false) && cargaPercurso != null)
            {
                pedidoXMLNotaFiscal = repPedidoXMLNotaFiscal.BuscarNotaPorLocalidadeDestino(cargaPedido.Codigo, cargaPercurso.Destino.Codigo);
                if (pedidoXMLNotaFiscal != null)
                {
                    cargaPedido.Pedido.Destinatario = pedidoXMLNotaFiscal.XMLNotaFiscal.Destinatario;
                    cargaPedido.Pedido.Destino = pedidoXMLNotaFiscal.XMLNotaFiscal.Destinatario.Localidade;
                    cargaPedido.Destino = pedidoXMLNotaFiscal.XMLNotaFiscal.Destinatario.Localidade;

                    Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEnderecoDestino = cargaPedido.Pedido.EnderecoDestino;

                    if (cargaPedido.Pedido.PedidoTransbordo || pedidoEnderecoDestino != null)
                    {
                        if (pedidoEnderecoDestino == null)
                            pedidoEnderecoDestino = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco();

                        serPedido.PreecherEnderecoPedido(ref pedidoEnderecoDestino, pedidoXMLNotaFiscal.XMLNotaFiscal.Destinatario);

                        if (pedidoEnderecoDestino.Codigo == 0)
                            repPedidoEndereco.Inserir(pedidoEnderecoDestino);
                        else
                            repPedidoEndereco.Atualizar(pedidoEnderecoDestino);

                        cargaPedido.Pedido.EnderecoDestino = pedidoEnderecoDestino;
                    }

                    repPedido.Atualizar(cargaPedido.Pedido);
                    repCargaPedido.Atualizar(cargaPedido);
                }
            }
        }

        /// <summary>
        /// Procedimento para relacionar os produtos do pedido no na carga.
        /// 07/03/20 -> Adicionado parametros para idenfiticar os produtos do pedido no carregamento.
        /// </summary>
        /// <param name="cargaPedido">CargaPedido</param>
        /// <param name="produtosPedido">Contém todos os produtos do pedido.</param>
        /// <param name="usarPesoProduto">Utilizar o peso do produto no totalizador de peso da carga.</param>
        /// <param name="unitOfWork"></param>
        /// <param name="montagemCargaPorPedidoProduto">Usar os produtos do pedido informados no carregamento</param>
        /// <param name="produtosDoPedidoNoCarregamento">Lista de produtos do pedido contidos no carregamento da carga em questão.</param>
        public void AdicionarProdutosCargaPedido(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> produtosPedido, bool usarPesoProduto, Repositorio.UnitOfWork unitOfWork, bool montagemCargaPorPedidoProduto = false, List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> produtosDoPedidoNoCarregamento = null)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(unitOfWork);
            decimal pesoTotalCargaPedidoProduto = 0m;

            cargaPedido.ValorMercadoriaDescontar = 0;
            cargaPedido.PesoMercadoriaDescontar = 0;

            if (!montagemCargaPorPedidoProduto)
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidosProdutos = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto>();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();
                bool utilizarPesoProdutoParaCalcularPesoCarga = configuracaoGeralCarga?.UtilizarPesoProdutoParaCalcularPesoCarga ?? false;
                decimal pesoProdutos = 0m;
                decimal pesoLiquidoProdutos = 0m;

                if (!cargaPedido.ReentregaSolicitada && !cargaPedido.Pedido.PedidoTotalmenteCarregado && cargaPedido.CargaPedidoTrechoAnterior == null)
                    cargaPedidosProdutos = repCargaPedidoProduto.BuscarPorPedido(cargaPedido.Pedido.Codigo);

                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProduto in produtosPedido)
                {
                    decimal quantidadeJaCarregada = (from obj in cargaPedidosProdutos where obj.Produto.Codigo == pedidoProduto.Produto.Codigo select obj.Quantidade).Sum();

                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto cargaPedidoProduto = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto();
                    cargaPedidoProduto.CargaPedido = cargaPedido;
                    cargaPedidoProduto.Produto = pedidoProduto.Produto;
                    cargaPedidoProduto.PesoUnitario = pedidoProduto.PesoUnitario;
                    cargaPedidoProduto.Quantidade = pedidoProduto.Quantidade - quantidadeJaCarregada;

                    if (cargaPedidoProduto.Quantidade <= 0 && (cargaPedido.Carga.TipoOperacao?.SelecionarRetiradaProduto ?? false))
                        continue;

                    cargaPedidoProduto.QuantidadeCaixa = pedidoProduto.QuantidadeCaixa;
                    cargaPedidoProduto.QuantidadeCaixasVazias = pedidoProduto.QuantidadeCaixasVazias;
                    cargaPedidoProduto.QuantidadeCaixasVaziasPlanejadas = pedidoProduto.QuantidadeCaixasVaziasPlanejadas;
                    cargaPedidoProduto.QuantidadePlanejada = pedidoProduto.QuantidadePlanejada;
                    cargaPedidoProduto.ObservacaoCarga = pedidoProduto.ObservacaoCarga;
                    cargaPedidoProduto.PesoTotalEmbalagem = pedidoProduto.PesoTotalEmbalagem;
                    cargaPedidoProduto.ValorUnitarioProduto = pedidoProduto.ValorProduto > 0 ? pedidoProduto.ValorProduto : pedidoProduto.PrecoUnitario;
                    cargaPedidoProduto.ImunoPlanejado = pedidoProduto.ImunoPlanejado;

                    repCargaPedidoProduto.Inserir(cargaPedidoProduto);

                    if (cargaPedidoProduto.Produto.DescontarPesoProdutoCalculoFrete)
                        cargaPedido.PesoMercadoriaDescontar += cargaPedidoProduto.PesoTotal;

                    if (cargaPedidoProduto.Produto.DescontarValorProdutoCalculoFrete)
                        cargaPedido.ValorMercadoriaDescontar += cargaPedidoProduto.ValorUnitarioProduto * cargaPedidoProduto.Quantidade;

                    pesoTotalCargaPedidoProduto += cargaPedidoProduto.PesoTotal;
                    pesoProdutos += cargaPedidoProduto.PesoProduto;
                    pesoLiquidoProdutos += cargaPedidoProduto.PesoLiquidoProduto;
                }

                if (utilizarPesoProdutoParaCalcularPesoCarga)
                {
                    //TODO: PPC - Adicionado log temporário para identificar problema no cargaPedido.Peso.
                    Servicos.Log.TratarErro($"Pedido {cargaPedido.Pedido.NumeroPedidoEmbarcador} - CargaPedido.Codigo = {cargaPedido.Codigo} - Peso Total De.: {cargaPedido.Peso} - Para.: {pesoProdutos}. CargaPedido.AdicionarProdutosCargaPedido - 1", "PesoCargaPedido");
                    cargaPedido.Peso = pesoProdutos;
                    cargaPedido.PesoLiquido = pesoLiquidoProdutos;
                }
                else if (pesoTotalCargaPedidoProduto > 0m && usarPesoProduto)
                {
                    //TODO: PPC - Adicionado log temporário para identificar problema no cargaPedido.Peso.
                    Servicos.Log.TratarErro($"Pedido {cargaPedido.Pedido.NumeroPedidoEmbarcador} - CargaPedido.Codigo = {cargaPedido.Codigo} - Peso Total De.: {cargaPedido.Peso} - Para.: {(pesoTotalCargaPedidoProduto + cargaPedido.Pedido.PesoTotalPaletes)}. CargaPedido.AdicionarProdutosCargaPedido - 2", "PesoCargaPedido");
                    cargaPedido.Peso = pesoTotalCargaPedidoProduto + cargaPedido.Pedido.PesoTotalPaletes;
                }
                else
                {
                    // Carregamento parcial de pedido...
                    // cargaPedido.Peso = (cargaPedido.Peso == 0 || cargaPedido.Peso >= cargaPedido.Pedido.PesoTotal ? cargaPedido.Pedido.PesoTotal + cargaPedido.Pedido.PesoTotalPaletes : cargaPedido.Peso);
                    //TODO: PPC - Adicionado log temporário para identificar problema no cargaPedido.Peso.
                    Servicos.Log.TratarErro($"Pedido {cargaPedido.Pedido.NumeroPedidoEmbarcador} - CargaPedido.Codigo = {cargaPedido.Codigo} - Peso Total De.: {cargaPedido.Peso} - Para.: {(cargaPedido.Pedido.PedidoTotalmenteCarregado ? (cargaPedido.Pedido.PesoTotal + cargaPedido.Pedido.PesoTotalPaletes) : cargaPedido.Peso)}. CargaPedido.AdicionarProdutosCargaPedido - 3", "PesoCargaPedido");
                    cargaPedido.Peso = cargaPedido.Pedido.PedidoTotalmenteCarregado ? (cargaPedido.Pedido.PesoTotal + cargaPedido.Pedido.PesoTotalPaletes) : cargaPedido.Peso;
                    cargaPedido.PesoLiquido = cargaPedido.Pedido.PesoLiquidoTotal;
                }
            }
            else
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto pedidoProduto in produtosDoPedidoNoCarregamento)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto cargaPedidoProduto = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto();
                    cargaPedidoProduto.CargaPedido = cargaPedido;
                    cargaPedidoProduto.Produto = pedidoProduto.PedidoProduto.Produto;
                    cargaPedidoProduto.PesoUnitario = pedidoProduto.Peso / (pedidoProduto.Quantidade == 0 ? 1 : pedidoProduto.Quantidade);
                    cargaPedidoProduto.Quantidade = pedidoProduto.Quantidade;
                    cargaPedidoProduto.QuantidadeCaixa = pedidoProduto.PedidoProduto.QuantidadeCaixa;
                    cargaPedidoProduto.QuantidadeCaixasVazias = pedidoProduto.PedidoProduto.QuantidadeCaixasVazias;
                    cargaPedidoProduto.QuantidadeCaixasVaziasPlanejadas = pedidoProduto.PedidoProduto.QuantidadeCaixasVaziasPlanejadas;
                    cargaPedidoProduto.PesoTotalEmbalagem = pedidoProduto.PedidoProduto.PesoTotalEmbalagem;
                    cargaPedidoProduto.ValorUnitarioProduto = pedidoProduto.PedidoProduto.ValorProduto > 0 ? pedidoProduto.PedidoProduto.ValorProduto : pedidoProduto.PedidoProduto.PrecoUnitario;
                    repCargaPedidoProduto.Inserir(cargaPedidoProduto);

                    if (cargaPedidoProduto.Produto.DescontarPesoProdutoCalculoFrete)
                        cargaPedido.PesoMercadoriaDescontar += cargaPedidoProduto.PesoTotal;

                    if (cargaPedidoProduto.Produto.DescontarValorProdutoCalculoFrete)
                        cargaPedido.ValorMercadoriaDescontar += cargaPedidoProduto.ValorUnitarioProduto * cargaPedidoProduto.Quantidade;

                    pesoTotalCargaPedidoProduto += pedidoProduto.Peso; // Contém o peso total carregado do produto do pedido no carregamento.
                }

                if (pesoTotalCargaPedidoProduto > 0m && usarPesoProduto)
                {
                    //TODO: PPC - Adicionado log temporário para identificar problema no cargaPedido.Peso.
                    Servicos.Log.TratarErro($"Pedido {cargaPedido.Pedido.NumeroPedidoEmbarcador} - CargaPedido.Codigo = {cargaPedido.Codigo} - Peso Total De.: {cargaPedido.Peso} - Para.: {pesoTotalCargaPedidoProduto + cargaPedido.Pedido.PesoTotalPaletes}. CargaPedido.AdicionarProdutosCargaPedido - 4", "PesoCargaPedido");
                    cargaPedido.Peso = pesoTotalCargaPedidoProduto + cargaPedido.Pedido.PesoTotalPaletes;
                }
                else
                {
                    //TODO: PPC - Adicionado log temporário para identificar problema no cargaPedido.Peso.
                    Servicos.Log.TratarErro($"Pedido {cargaPedido.Pedido.NumeroPedidoEmbarcador} - CargaPedido.Codigo = {cargaPedido.Codigo} - Peso Total De.: {cargaPedido.Peso} - Para.: {cargaPedido.Pedido.PesoTotal + cargaPedido.Pedido.PesoTotalPaletes}. CargaPedido.AdicionarProdutosCargaPedido - 5", "PesoCargaPedido");
                    cargaPedido.Peso = cargaPedido.Pedido.PesoTotal + cargaPedido.Pedido.PesoTotalPaletes;
                    cargaPedido.PesoLiquido = cargaPedido.Pedido.PesoLiquidoTotal;
                }
            }

            repCargaPedido.Atualizar(cargaPedido);
        }

        public void AdicionarProdutosCargaParaProcessamentoPosterior(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> produtosPedido, bool usarPesoProduto, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaSqlPedidoProduto ObjetoMontagemCargaSqlPedidoProduto, bool montagemCargaPorPedidoProduto = false, List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> produtosDoPedidoNoCarregamento = null)
        {
            decimal pesoTotalCargaPedidoProduto = 0;
            cargaPedido.ValorMercadoriaDescontar = 0;
            cargaPedido.PesoMercadoriaDescontar = 0;

            if (!montagemCargaPorPedidoProduto)
            {

                pesoTotalCargaPedidoProduto = produtosPedido.Sum(x => (x.Quantidade * x.PesoUnitario) + x.PesoTotalEmbalagem);
                cargaPedido.PesoMercadoriaDescontar = produtosPedido.Where(x => x.Produto.DescontarPesoProdutoCalculoFrete).Sum(x => x.PesoTotal);
                cargaPedido.ValorMercadoriaDescontar = produtosPedido.Where(x => x.Produto.DescontarValorProdutoCalculoFrete).Sum(x => x.ValorProduto * x.Quantidade);

                CriarObjetoInsertPedidoProduto(cargaPedido, produtosPedido, ref ObjetoMontagemCargaSqlPedidoProduto);

                if (pesoTotalCargaPedidoProduto > 0m && usarPesoProduto)
                {
                    cargaPedido.Peso = pesoTotalCargaPedidoProduto + cargaPedido.Pedido.PesoTotalPaletes;
                }
                else
                {
                    cargaPedido.Peso = (cargaPedido.Peso == 0 || cargaPedido.Peso >= cargaPedido.Pedido.PesoTotal ? cargaPedido.Pedido.PesoTotal + cargaPedido.Pedido.PesoTotalPaletes : cargaPedido.Peso);
                    cargaPedido.PesoLiquido = cargaPedido.Pedido.PesoLiquidoTotal;
                }
            }
            else
            {
                pesoTotalCargaPedidoProduto = produtosDoPedidoNoCarregamento.Sum(x => x.Peso);
                cargaPedido.PesoMercadoriaDescontar = produtosDoPedidoNoCarregamento.Where(x => x.PedidoProduto.Produto.DescontarPesoProdutoCalculoFrete).Sum(x => x.Quantidade * (x.Peso / (x.Quantidade == 0 ? 1 : x.Quantidade)) + x.PedidoProduto.PesoTotalEmbalagem);
                cargaPedido.ValorMercadoriaDescontar = produtosDoPedidoNoCarregamento.Where(x => x.PedidoProduto.Produto.DescontarValorProdutoCalculoFrete).Sum(x => x.PedidoProduto.ValorProduto * x.Quantidade);

                CriarObjetoInsertPedidoProduto(cargaPedido, produtosDoPedidoNoCarregamento, ref ObjetoMontagemCargaSqlPedidoProduto);

                if (pesoTotalCargaPedidoProduto == 0m && usarPesoProduto && (produtosDoPedidoNoCarregamento?.Count ?? 0) > 0 && (produtosPedido?.Count ?? 0) > 0)
                {
                    pesoTotalCargaPedidoProduto = (from ppc in produtosDoPedidoNoCarregamento
                                                   join ppp in produtosPedido on ppc.PedidoProduto.Codigo equals ppp.Codigo
                                                   select ppc.Quantidade * ppp.PesoUnitario)?.Sum() ?? 0;
                }

                if (pesoTotalCargaPedidoProduto > 0m && usarPesoProduto)
                {
                    cargaPedido.Peso = pesoTotalCargaPedidoProduto + cargaPedido.Pedido.PesoTotalPaletes;
                }
                else
                {
                    cargaPedido.Peso = cargaPedido.Pedido.PesoTotal + cargaPedido.Pedido.PesoTotalPaletes;
                    cargaPedido.PesoLiquido = cargaPedido.Pedido.PesoLiquidoTotal;
                }
            }
        }

        public void AdicionarDivisoesCapacidadeCargaPedido(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga, List<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto> produtosIntegracao, ref System.Text.StringBuilder stMensagem, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade repCargaPedidoProdutoDivisaoCapacidade = new Repositorio.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos = repCargaPedidoProduto.BuscarPorCargaPedido(cargaPedido.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto cargaPedidoProduto in cargaPedidoProdutos)
            {
                Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produtoIntegracao = produtosIntegracao.Where(p => p.CodigoProduto == cargaPedidoProduto.Produto.CodigoProdutoEmbarcador).FirstOrDefault();
                if ((produtoIntegracao?.ProdutoDivisoesCapacidade?.Count ?? 0) == 0) continue;

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade> divisoesAdicionadas = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade>();

                foreach (Dominio.ObjetosDeValor.Embarcador.Pedido.ProdutoDivisaoCapacidade produtoIntegracaoDivisoesCapacidade in produtoIntegracao.ProdutoDivisoesCapacidade)
                {
                    Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaDivisaoCapacidade modeloVeicularCargaDivisaoCapacidade = (from o in modeloVeicularCarga.DivisoesCapacidade
                                                                                                                                     where
                                                                                                                                        o.Coluna == produtoIntegracaoDivisoesCapacidade.Coluna
                                                                                                                                        && o.Piso == produtoIntegracaoDivisoesCapacidade.Piso
                                                                                                                                        && o.Descricao == produtoIntegracaoDivisoesCapacidade.Descricao
                                                                                                                                     select o).FirstOrDefault();

                    if (modeloVeicularCargaDivisaoCapacidade == null)
                    {
                        stMensagem.AppendLine($"não foi encontrado a divisão de capacidade {produtoIntegracaoDivisoesCapacidade.Descricao}");
                        if (produtoIntegracaoDivisoesCapacidade.Piso > 0 || produtoIntegracaoDivisoesCapacidade.Coluna > 0)
                            stMensagem.Append($" (Piso: {produtoIntegracaoDivisoesCapacidade.Piso}, Coluna: {produtoIntegracaoDivisoesCapacidade.Coluna})");
                        stMensagem.AppendLine($" do modelo {cargaPedido.Carga.Veiculo?.ModeloVeicularCarga?.Descricao}");
                        return;
                    }

                    bool divisaoJaInseirda = (from o in divisoesAdicionadas where o.ModeloVeicularCargaDivisaoCapacidade.Codigo == modeloVeicularCargaDivisaoCapacidade.Codigo select o).Any();
                    if (divisaoJaInseirda)
                    {
                        stMensagem.AppendLine($"A divisão de capacidade {produtoIntegracaoDivisoesCapacidade.Descricao}");
                        if (produtoIntegracaoDivisoesCapacidade.Piso > 0 || produtoIntegracaoDivisoesCapacidade.Coluna > 0)
                            stMensagem.Append($" (Piso: {produtoIntegracaoDivisoesCapacidade.Piso}, Coluna: {produtoIntegracaoDivisoesCapacidade.Coluna})");
                        stMensagem.AppendLine($" do modelo {cargaPedido.Carga.Veiculo?.ModeloVeicularCarga?.Descricao} esta sendo informada em duplicidade");
                        return;
                    }

                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade cargaPedidoProdutoDivisaoCapacidade = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade()
                    {
                        ModeloVeicularCargaDivisaoCapacidade = modeloVeicularCargaDivisaoCapacidade,
                        CargaPedidoProduto = cargaPedidoProduto,
                        Quantidade = produtoIntegracaoDivisoesCapacidade.Quantidade,
                        QuantidadePlanejada = produtoIntegracaoDivisoesCapacidade.QuantidadePlanejada
                    };

                    repCargaPedidoProdutoDivisaoCapacidade.Inserir(cargaPedidoProdutoDivisaoCapacidade);
                    divisoesAdicionadas.Add(cargaPedidoProdutoDivisaoCapacidade);
                }
            }
        }

        public void ValidarValorDescargaPorCargaPedido(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, bool componenteFilialEmissora, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidoComponentesFretes, List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> cargaComponentesFretes)
        {
            if (!componenteFilialEmissora)
            {
                if (cargaPedido.ValorDescarga > 0 && ((carga.Filial == null || !carga.Filial.NaoAdicionarValorDescarga) && (carga.Filial == null || carga.TipoOperacao == null || !carga.Filial.TipoOperacoesIsencaoValorDescargaCliente.Contains(carga.TipoOperacao)) || cargaPedido.ConfiguracaoDescarga != null))
                {
                    ComponetesFrete svcComponente = new ComponetesFrete(unitOfWork);
                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete cargaPedidoComponentesFrete = (from obj in cargaPedidoComponentesFretes where obj.CargaPedido.Codigo == cargaPedido.Codigo && obj.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.DESCARGA && obj.ComponenteFilialEmissora == componenteFilialEmissora && ((componenteFrete == null && obj.ComponenteFrete == null) || (componenteFrete != null && obj.ComponenteFrete.Codigo == componenteFrete.Codigo)) select obj).FirstOrDefault();

                    if (cargaPedidoComponentesFrete == null)
                        svcComponente.AdicionarCargaPedidoComponente(cargaPedido, cargaPedido.ValorDescarga, 0, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.DESCARGA, componenteFrete, true, false, null, null, "", false, false, componenteFilialEmissora, unitOfWork, false, null, 0, null, false, ref cargaPedidoComponentesFretes, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real, 0m, 0m);

                    svcComponente.AdicionarComponenteFreteCargaUnicoPorTipo(cargaPedido.Carga, componenteFrete, cargaPedido.ValorDescarga, 0m, componenteFilialEmissora, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.DESCARGA, null, true, false, null, tipoServicoMultisoftware, null, unitOfWork, cargaComponentesFretes, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargaComponenteFrete.TabelaFrete, true, false, null, 0, null, cargaPedido.ObterTomador());
                }
            }
        }

        public void ValidarValorPedagioPorCargaPedido(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, bool componenteFilialEmissora, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidoComponentesFretes, List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> cargaComponentesFretes, bool removeuComponentesAnteriormente, Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete)
        {
            if (componenteFilialEmissora || removeuComponentesAnteriormente || (tabelaFrete?.NaoDestacarResultadoConsultaPedagioComoComponente ?? false))
                return;

            if (cargaPedido.ValorPedagio == 0)
                return;

            ComponetesFrete svcComponente = new ComponetesFrete(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete cargaPedidoComponentesFrete = (from obj in cargaPedidoComponentesFretes where obj.CargaPedido.Codigo == cargaPedido.Codigo && obj.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PEDAGIO && obj.ComponenteFilialEmissora == componenteFilialEmissora && ((componenteFrete == null && obj.ComponenteFrete == null) || (componenteFrete != null && obj.ComponenteFrete.Codigo == componenteFrete.Codigo)) select obj).FirstOrDefault();

            if (cargaPedidoComponentesFrete != null)
                return;

            bool destacarComponenteTabelaFrete = Servicos.Embarcador.Carga.Frete.DestacarComponenteTabelaFrete(tabelaFrete, componenteFrete);

            bool naoSomarValorTotalAReceber = (destacarComponenteTabelaFrete ? tabelaFrete?.NaoSomarValorTotalAReceber : componenteFrete?.NaoSomarValorTotalAReceber) ?? false;
            bool naoSomarValorTotalPrestacao = (destacarComponenteTabelaFrete ? tabelaFrete?.NaoSomarValorTotalPrestacao : componenteFrete?.NaoSomarValorTotalPrestacao) ?? false;
            bool IncluirBaseCalculo = !(componenteFrete?.NaoIncluirBaseCalculoImpostos ?? false);

            svcComponente.AdicionarCargaPedidoComponente(cargaPedido, cargaPedido.ValorPedagio, 0, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PEDAGIO, componenteFrete, IncluirBaseCalculo, false, null, null, "", false, false, componenteFilialEmissora, unitOfWork, false, null, 0, null, naoSomarValorTotalAReceber, ref cargaPedidoComponentesFretes, naoSomarValorTotalPrestacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real, 0m, 0m);
            svcComponente.AdicionarComponenteFreteCargaUnicoPorTipo(cargaPedido.Carga, componenteFrete, cargaPedido.ValorPedagio, 0m, componenteFilialEmissora, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PEDAGIO, null, true, false, null, tipoServicoMultisoftware, null, unitOfWork, cargaComponentesFretes, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargaComponenteFrete.TabelaFrete, true, false, null, 0, null, cargaPedido.ObterTomador());
        }

        public void GerarComponentesClientePorCargaPedido(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, bool componenteFilialEmissora, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, List<Dominio.Entidades.Embarcador.Pessoas.ClienteComponente> listaComponentesClienteTotal, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidoComponentesFretes, List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> cargaComponentesFretes)
        {
            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                List<Dominio.Entidades.Embarcador.Pessoas.ClienteComponente> listaComponentesCliente = (from obj in listaComponentesClienteTotal where obj.Cliente.CPF_CNPJ == (cargaPedido.Recebedor != null ? cargaPedido.Recebedor.CPF_CNPJ : cargaPedido.Pedido.Destinatario.CPF_CNPJ) select obj).ToList();
                foreach (Dominio.Entidades.Embarcador.Pessoas.ClienteComponente componenteCliente in listaComponentesCliente)
                {
                    if (componenteCliente.Valor > 0)
                    {
                        ComponetesFrete svcComponente = new ComponetesFrete(unitOfWork);

                        svcComponente.AdicionarCargaPedidoComponente(cargaPedido, componenteCliente.Valor, 0, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.CLIENTE, componenteCliente.ComponenteFrete, true, false, null, null, string.Empty, false, false, componenteFilialEmissora, unitOfWork, false, null, 0, null, false, ref cargaPedidoComponentesFretes, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real, 0m, 0m);
                        svcComponente.AdicionarComponenteFreteCargaUnicoPorTipo(cargaPedido.Carga, componenteCliente.ComponenteFrete, componenteCliente.Valor, 0, componenteFilialEmissora, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.CLIENTE, null, true, false, null, tipoServicoMultisoftware, null, unitOfWork, cargaComponentesFretes, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargaComponenteFrete.TabelaFrete, true, false, null, 0, null, cargaPedido.ObterTomador());
                    }
                }
            }
        }

        public void RemoverValorDescarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, bool componenteFilialEmissora, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return;

            //Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponentesFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unidadeTrabalho);
            Servicos.Embarcador.Carga.ComplementoFrete serComplementoFrete = new ComplementoFrete(unidadeTrabalho);
            //List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);

            List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> cargaComponentesFrete = repCargaComponentesFrete.BuscarPorCargaETipo(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.DESCARGA, true, componenteFilialEmissora);
            for (var i = 0; i < cargaComponentesFrete.Count; i++)
                serComplementoFrete.ExtornarComplementoDeFrete(cargaComponentesFrete[i].CargaComplementoFrete, tipoServicoMultisoftware, unidadeTrabalho);

            repCargaComponentesFrete.DeletarPorCarga(carga.Codigo, componenteFilialEmissora, 0, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.DESCARGA, true);

            if (carga.CargaAgrupada)
                repCargaComponentesFrete.DeletarPorCargaAgrupamento(carga.Codigo, componenteFilialEmissora, 0, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.DESCARGA, true);

        }

        public void RemoverComponentesCliente(Dominio.Entidades.Embarcador.Cargas.Carga carga, bool componenteFilialEmissora, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponentesFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unidadeTrabalho);
            Servicos.Embarcador.Carga.ComplementoFrete serComplementoFrete = new ComplementoFrete(unidadeTrabalho);

            List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> cargaComponentesFrete = repCargaComponentesFrete.BuscarPorCargaETipo(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.CLIENTE, true, componenteFilialEmissora);

            for (var i = 0; i < cargaComponentesFrete.Count; i++)
                serComplementoFrete.ExtornarComplementoDeFrete(cargaComponentesFrete[i].CargaComplementoFrete, tipoServicoMultisoftware, unidadeTrabalho);

            repCargaComponentesFrete.DeletarPorCarga(carga.Codigo, componenteFilialEmissora, 0, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.CLIENTE, true);

            if (carga.CargaAgrupada)
                repCargaComponentesFrete.DeletarPorCargaAgrupamento(carga.Codigo, componenteFilialEmissora, 0, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.CLIENTE, true);

        }

        public List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> BuscarCargaPedidoComponentesFrete(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, bool componenteFilialEmissora, Repositorio.UnitOfWork unitOfWork)
        {
            //todo: Os componentes atualmente são montados por tipo no CT-e ver regra, se necessário para agrupar por componente de Frete (apenas exibição)
            Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponenteFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> cargaPedidoComponentesFretesCliente = new List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidoComponentesFretes = repCargaPedidoComponenteFrete.BuscarPorCargaPedidoComPisCofins(cargaPedido.Codigo, true, cargaPedido.ModeloDocumentoFiscal, componenteFilialEmissora);
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete cargaPedidoComponenteFrete in cargaPedidoComponentesFretes)
                {
                    int index = cargaPedidoComponentesFretesCliente.FindIndex(obj => obj.TipoComponenteFrete == cargaPedidoComponenteFrete.TipoComponenteFrete && cargaPedidoComponenteFrete.ComponenteFrete == obj.ComponenteFrete);

                    if (index != -1)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico componenteFrete = cargaPedidoComponentesFretesCliente[index];

                        componenteFrete.ValorComponenteComICMSIncluso += cargaPedidoComponenteFrete.ValorComponenteComICMSIncluso;
                        componenteFrete.ValorComponente += cargaPedidoComponenteFrete.ValorComponente;
                        componenteFrete.ValorTotalMoeda += cargaPedidoComponenteFrete.ValorTotalMoeda ?? 0m;

                        cargaPedidoComponentesFretesCliente[index] = componenteFrete;
                    }
                    else
                    {
                        cargaPedidoComponentesFretesCliente.Add(cargaPedidoComponenteFrete.ConvertarParaComponenteDinamico());
                    }
                }
            }
            return cargaPedidoComponentesFretesCliente;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> BuscarCargaPedidoComponentesFrete(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Repositorio.UnitOfWork unitOfWork, bool componenteFilialEmissora)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico> cargaPedidoComponentesFretesCliente = new List<Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico>();

            //todo: Os componentes atualmente são montados por tipo no CT-e ver regra, se necessário para agrupar por componente de Frete (apenas exibição)
            Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponenteFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidoComponentesFretes = repCargaPedidoComponenteFrete.BuscarPorCargaPedidoComPisCofins(cargaPedido.Codigo, true, cargaPedido.ModeloDocumentoFiscal, componenteFilialEmissora);
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete cargaPedidoComponenteFrete in cargaPedidoComponentesFretes)
            {
                int index = cargaPedidoComponentesFretesCliente.FindIndex(obj => obj.TipoComponenteFrete == cargaPedidoComponenteFrete.TipoComponenteFrete && cargaPedidoComponenteFrete.ComponenteFrete == obj.ComponenteFrete);

                if (index != -1)
                {
                    Dominio.ObjetosDeValor.Embarcador.Carga.ComponenteFreteDinamico componenteFrete = cargaPedidoComponentesFretesCliente[index];

                    componenteFrete.ValorComponente += cargaPedidoComponenteFrete.ValorComponente;
                    componenteFrete.ValorTotalMoeda += cargaPedidoComponenteFrete.ValorTotalMoeda ?? 0m;

                    cargaPedidoComponentesFretesCliente[index] = componenteFrete;
                }
                else
                {
                    cargaPedidoComponentesFretesCliente.Add(cargaPedidoComponenteFrete.ConvertarParaComponenteDinamico());
                }
            }
            return cargaPedidoComponentesFretesCliente;
        }

        public static void RemoverPedidoVinculadoCarga(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, bool removerPedido)
        {
            if (cargaPedido == null)
                throw new ServicoException("não foi possível encontrar o pedido.");

            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaPedido.Carga;

            if ((carga.SituacaoCarga != SituacaoCarga.AgNFe) && (carga.SituacaoCarga != SituacaoCarga.Nova))
                throw new ServicoException("não é possível remover o pedido na situação atual da carga.");

            if (!cargaPedido.Pedido.AdicionadaManualmente && removerPedido && !configuracao.IncluirCargaCanceladaProcessarDT)
                throw new ServicoException("Somente é possível remover pedidos adicionados manualmente.");

            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repositorioPedidoCTeTerceiro = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);

            if (repositorioPedidoXMLNotaFiscal.VerificarSeExistePorCargaPedido(cargaPedido.Codigo))
                throw new ServicoException("Existem notas fiscais vinculadas à este pedido, não sendo possível excluir o mesmo.");

            if (repositorioPedidoCTeTerceiro.VerificarSeExistePorCargaPedido(cargaPedido.Codigo))
                throw new ServicoException("Existem CT-es de Subcontratação vinculados à este pedido, não sendo possível excluir o mesmo.");

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaPedido.Pedido;
            Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaDadosTransporte repositorioConfiguracaoCargaDadosTransporte = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaDadosTransporte(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repositorioCargaPedidoComponenteFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoQuantidades repositorioCargaPedidoQuantidade = new Repositorio.Embarcador.Cargas.CargaPedidoQuantidades(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoTabelaFreteCliente repositorioCargaPedidoTabelaFrete = new Repositorio.Embarcador.Cargas.CargaPedidoTabelaFreteCliente(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
            Repositorio.Embarcador.Integracao.IntegracaoAVIPED repIntegracaoAVIPED = new Repositorio.Embarcador.Integracao.IntegracaoAVIPED(unitOfWork);
            Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao repApoliceSeguro = new Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoProduto repositorioCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(unitOfWork);
            Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga servicoMontagemCarga = new Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga(unitOfWork);
            Servicos.Embarcador.Pedido.OcorrenciaPedido servOcorrenciaPedido = new Servicos.Embarcador.Pedido.OcorrenciaPedido(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repositorioConfiguracaoPedido.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaDadosTransporte configuracaoDadosTransporte = repositorioConfiguracaoCargaDadosTransporte.BuscarPrimeiroRegistro();
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> pedidosVinculados = repositorioCargaPedido.BuscarPedidosPorPedidoEncaixado(cargaPedido.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> componentesFreteCargaPedido = repositorioCargaPedidoComponenteFrete.BuscarPorCargaPedido(cargaPedido.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades> quantidadesCargaPedido = repositorioCargaPedidoQuantidade.BuscarPorCargaPedido(cargaPedido.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoTabelaFreteCliente> tabelasFreteClienteCargaPedido = repositorioCargaPedidoTabelaFrete.BuscarPorCargaPedido(cargaPedido.Codigo);
            List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotosCargaPedido = repCanhoto.BuscarPorCargaPedido(cargaPedido.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidos = repCargaEntregaPedido.BuscarCargaEntregaPedidoPorCargaPedido(cargaPedido.Codigo);
            List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao> listaApoliceSeguroAverbacao = repApoliceSeguro.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> listacargaPedidoProduto = repositorioCargaPedidoProduto.BuscarPorCargaPedido(cargaPedido.Codigo);

            servicoMontagemCarga.RemoverPedidoTodosCarregamentos(pedido);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoTabelaFreteCliente tabelaFreteClienteCargaPedido in tabelasFreteClienteCargaPedido)
                repositorioCargaPedidoTabelaFrete.Deletar(tabelaFreteClienteCargaPedido);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades quantidadeCargaPedido in quantidadesCargaPedido)
                repositorioCargaPedidoQuantidade.Deletar(quantidadeCargaPedido);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete componenteFreteCargaPedido in componentesFreteCargaPedido)
                repositorioCargaPedidoComponenteFrete.Deletar(componenteFreteCargaPedido);

            repIntegracaoAVIPED.DeletarPorCargaPedido(cargaPedido.Codigo);

            foreach (Dominio.Entidades.Embarcador.Canhotos.Canhoto canhotoCargaPedido in canhotosCargaPedido)
            {
                canhotoCargaPedido.CargaPedido = null;
                repCanhoto.Atualizar(canhotoCargaPedido);
            }

            if (cargaEntregaPedidos != null)
                foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido cargaEntregaPedido in cargaEntregaPedidos)
                    repCargaEntregaPedido.Deletar(cargaEntregaPedido);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido pedidoVinculado in pedidosVinculados)
            {
                pedidoVinculado.CargaPedidoEncaixe = null;
                repositorioCargaPedido.Atualizar(pedidoVinculado);
            }

            foreach (Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao apoliceSeguro in listaApoliceSeguroAverbacao)
                repApoliceSeguro.Deletar(apoliceSeguro);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto cargaPedidoProduto in listacargaPedidoProduto)
                repositorioCargaPedidoProduto.Deletar(cargaPedidoProduto);

            repositorioCargaPedido.Deletar(cargaPedido);

            if (pedido.EnderecoDestino != null)
                pedido.EnderecoDestino = null;

            if (pedido.EnderecoOrigem != null)
                pedido.EnderecoOrigem = null;

            if (removerPedido)
            {
                //pedido.CargasPedido = null;

                Repositorio.Embarcador.Pedidos.PedidoProduto repositorioPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> produtosPedido = repositorioPedidoProduto.BuscarPorPedido(pedido.Codigo);

                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoProduto produtoPedido in produtosPedido)
                    repositorioPedidoProduto.Deletar(produtoPedido);

                repositorioPedido.Deletar(pedido);
            }
            else
            {
                pedido.SituacaoPedido = SituacaoPedido.Cancelado;
                repositorioPedido.Atualizar(pedido);

                servOcorrenciaPedido.ProcessarOcorrenciaPedido(EventoColetaEntrega.PedidoCancelado, pedido, configuracao, clienteMultisoftware);
            }

            if (configuracaoDadosTransporte?.RetornarCargaPendenteConsultaCarregamentoAoSalvarDadosTransporte ?? false)
            {
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                carga.CarregamentoIntegradoERP = false;

                repositorioCarga.Atualizar(carga);
            }
        }

        public static void RemoverPedidoCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga, bool? reentrega = null, bool permitirRemoverTodos = false, bool forcarRemocao = false, bool naoRecalcularFrete = false)
        {
            RemoverPedidosCarga(carga, new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>() { cargaPedido }, configuracaoEmbarcador, tipoServicoMultisoftware, unitOfWork, configuracaoGeralCarga, reentrega, permitirRemoverTodos, forcarRemocao, naoRecalcularFrete);
        }

        public static void RemoverPedidosCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosRemover, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga, bool? reentrega = null, bool permitirRemoverTodos = false, bool forcarRemocao = false, bool naoRecalcularFrete = false)
        {
            if (cargaPedidosRemover.Count == 0)
                return;

            if (cargaPedidosRemover.Any(o => o.Carga.Codigo != carga.Codigo))
                throw new ServicoException("Todos os pedidos devem pertencer a mesma carga para serem removidos");

            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMDFe repositorioCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaDadosTransporte repositorioConfiguracaoCargaDadosTransporte = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaDadosTransporte(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repositorioConfiguracaoPedido.BuscarConfiguracaoPadrao();

            Servicos.Embarcador.Carga.CargaLocaisPrestacao servicoCargaLocaisPrestacao = new Servicos.Embarcador.Carga.CargaLocaisPrestacao(unitOfWork);
            Servicos.Embarcador.Carga.CargaDadosSumarizados servicoCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);
            Servicos.Embarcador.Carga.RateioFrete servicoRateioFrete = new Servicos.Embarcador.Carga.RateioFrete(unitOfWork);

            bool existeIntegracaoTelhaNorte = repositorioTipoIntegracao.ExistePorTipo(TipoIntegracao.TelhaNorte);

            if (existeIntegracaoTelhaNorte && (carga.TipoOperacao?.AdicionarOuRemoverPedidosReplicaAosTrechosAnteriores ?? false))
            {
                IList<int> codigosCargasTelhaNorte = repositorioCarga.BuscarCodigosCargasAgrupadasTelhaNorte(carga.CodigoCargaEmbarcador);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidoTelha = repositorioCargaPedido.BuscarPorCargasEPedido(codigosCargasTelhaNorte, cargaPedidosRemover.FirstOrDefault()?.Pedido.Codigo ?? 0);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoTelha in cargasPedidoTelha)
                    RemoverPedidoCarga(cargaPedidoTelha, configuracaoEmbarcador, tipoServicoMultisoftware, unitOfWork, configuracaoGeralCarga, permitirRemoverTodos: permitirRemoverTodos, reentrega: reentrega, forcarRemocao: forcarRemocao, false, false);
            }
            else
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidosRemover)
                    RemoverPedidoCarga(cargaPedido, configuracaoEmbarcador, tipoServicoMultisoftware, unitOfWork, configuracaoGeralCarga, permitirRemoverTodos: permitirRemoverTodos, reentrega: reentrega, forcarRemocao: forcarRemocao);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaDadosTransporte configuracaoDadosTransporte = repositorioConfiguracaoCargaDadosTransporte.BuscarPrimeiroRegistro();
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedidos = repositorioCargaPedido.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> mdfes = repositorioCargaMDFe.BuscarPorCarga(carga.Codigo);

            mdfes.ForEach(x => x.CargaLocaisPrestacao = null);
            mdfes.ForEach(x => repositorioCargaMDFe.Atualizar(x));

            if (configuracaoEmbarcador.ExigirCargaRoteirizada || (carga.TipoOperacao?.ExigirCargaRoteirizada ?? false))
                carga.SituacaoRoteirizacaoCarga = SituacaoRoteirizacao.Aguardando;

            Servicos.Embarcador.Carga.RotaFrete.SetarRotaFreteCarga(carga, listaCargaPedidos, configuracaoEmbarcador, unitOfWork, tipoServicoMultisoftware);

            if (listaCargaPedidos.Count > 0)
                servicoCargaLocaisPrestacao.VerificarEAjustarLocaisPrestacao(carga, listaCargaPedidos, unitOfWork, tipoServicoMultisoftware, configuracaoPedido);

            servicoCargaDadosSumarizados.AlterarDadosSumarizadosCarga(ref carga, listaCargaPedidos, configuracaoEmbarcador, unitOfWork, tipoServicoMultisoftware);

            if (carga.CargaEmitidaParcialmente)
            {
                servicoRateioFrete.RatearValorFreteCargaEmitidaParcialmente(carga, listaCargaPedidos, configuracaoEmbarcador, tipoServicoMultisoftware, unitOfWork);

                if (repositorioCargaPedido.VerificarSeTodosPedidosEstaoAutorizadosPorCarga(carga.Codigo))
                {
                    if (!carga.DataEnvioUltimaNFe.HasValue)
                    {
                        carga.DataInicioEmissaoDocumentos = DateTime.Now;
                        carga.DataEnvioUltimaNFe = DateTime.Now;
                    }

                    carga.DataInicioConfirmacaoDocumentosFiscais = DateTime.Now;
                    carga.ProcessandoDocumentosFiscais = true;
                }
            }
            else
            {
                if ((carga.TipoFreteEscolhido == TipoFreteEscolhido.Operador) || (carga.TipoFreteEscolhido == TipoFreteEscolhido.Cliente))
                {
                    if (carga.TipoFreteEscolhido == TipoFreteEscolhido.Operador)
                    {
                        servicoRateioFrete.RatearValorDoFrenteEntrePedidos(carga, listaCargaPedidos, configuracaoEmbarcador, false, unitOfWork, tipoServicoMultisoftware);

                        Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicao = (Servicos.Embarcador.Frete.ComposicaoFrete.ObterComposicaoFrete("Valor Informado Pelo Operador", " Valor Informado = " + carga.ValorFrete.ToString("n2"), carga.ValorFrete, TipoParametroBaseTabelaFrete.ValorFreteLiquido, TipoCampoValorTabelaFrete.ValorFixo, "Valor informado pelo Operador", 0, carga.ValorFrete));
                        Servicos.Embarcador.Carga.ComposicaoFrete.ComposicaoFrete.SetarComposicaoFrete(carga, null, null, null, false, composicao, unitOfWork, null);
                    }

                    if (carga.SituacaoCarga != SituacaoCarga.Nova && carga.SituacaoCarga != SituacaoCarga.AgNFe)
                        carga.SituacaoCarga = carga.ExigeNotaFiscalParaCalcularFrete ? SituacaoCarga.CalculoFrete : SituacaoCarga.AgTransportador;

                    if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                        Servicos.Log.TratarErro("Atualizou a situação para calculo frete 39 Carga " + carga.CodigoCargaEmbarcador, "AtualizouSituacaoCalculoFrete");
                }
                else if (carga.ExigeNotaFiscalParaCalcularFrete && !naoRecalcularFrete)
                {
                    if (carga.SituacaoCarga == SituacaoCarga.CalculoFrete)
                    {
                        carga.CalculandoFrete = true;
                        carga.DataInicioCalculoFrete = DateTime.Now;
                    }
                }
                else if (!naoRecalcularFrete && ((carga.SituacaoCarga == SituacaoCarga.AgTransportador) || (carga.SituacaoCarga == SituacaoCarga.CalculoFrete) || (carga.SituacaoCarga == SituacaoCarga.AgNFe)))
                {
                    carga.SituacaoCarga = SituacaoCarga.CalculoFrete;
                    carga.CalculandoFrete = true;
                    carga.DataInicioCalculoFrete = DateTime.Now;

                    if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                        Servicos.Log.TratarErro("Atualizou a situação para calculo frete 38 Carga " + carga.CodigoCargaEmbarcador, "AtualizouSituacaoCalculoFrete");
                }

                if (!carga.DataEnvioUltimaNFe.HasValue && repositorioCargaPedido.VerificarSeTodosPedidosEstaoAutorizadosPorCarga(carga.Codigo) && !carga.ExigeConfirmacaoAntesEmissao)
                {
                    carga.DataInicioEmissaoDocumentos = DateTime.Now;
                    carga.DataEnvioUltimaNFe = DateTime.Now;
                }

                carga.DataInicioConfirmacaoDocumentosFiscais = DateTime.Now;
                carga.ProcessandoDocumentosFiscais = true;
            }

            carga.DataAtualizacaoCarga = DateTime.Now;

            if (configuracaoDadosTransporte?.RetornarCargaPendenteConsultaCarregamentoAoSalvarDadosTransporte ?? false)
                carga.CarregamentoIntegradoERP = false;

            if (carga.Carregamento != null)
            {
                Embarcador.Carga.MontagemCarga.MontagemCarga servicoMontagemCarga = new MontagemCarga.MontagemCarga(unitOfWork);
                Integracao.IntegracaoCarregamento servicoIntegracaoCarregamento = new Integracao.IntegracaoCarregamento(unitOfWork);

                servicoMontagemCarga.AtualizarSituacaoExigeIscaPorCarga(carga);

                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoApolice repositorioCarregamentoApolice = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoApolice(unitOfWork);
                List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro> apolicesSeguro = repositorioCarregamentoApolice.BuscarApolicesPorCarregamento(carga.Carregamento.Codigo);

                servicoIntegracaoCarregamento.AdicionarIntegracaoCarregamento(carga.Carregamento, StatusCarregamentoIntegracao.Atualizar, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.TelhaNorte);

                if (apolicesSeguro.Count > 0 && listaCargaPedidos.Count > 0)
                {
                    Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao repApoliceSeguro = new Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao> listaApoliceSeguroAverbacao = repApoliceSeguro.BuscarPorCarga(carga.Codigo);

                    if (listaApoliceSeguroAverbacao.Count == 0)
                    {
                        foreach (Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apoliceSeguro in apolicesSeguro)
                        {
                            Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao apoliceSeguroAverbacao = new Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao()
                            {
                                CargaPedido = listaCargaPedidos.FirstOrDefault(),
                                ApoliceSeguro = apoliceSeguro
                            };

                            repApoliceSeguro.Inserir(apoliceSeguroAverbacao);
                        }
                    }
                }
            }

            SumarizarDadosZonaTransporte(carga, unitOfWork);
            AtualizarIntegracaoCargaDadosTransporte(carga, unitOfWork);

            repositorioCarga.Atualizar(carga);
        }

        public async static Task RemoverPedidosCargaAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosRemover, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga, bool? reentrega = null, bool permitirRemoverTodos = false, bool forcarRemocao = false, bool naoRecalcularFrete = false)
        {
            if (cargaPedidosRemover.Count == 0)
                return;

            if (cargaPedidosRemover.Any(o => o.Carga.Codigo != carga.Codigo))
                throw new ServicoException("Todos os pedidos devem pertencer a mesma carga para serem removidos");

            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMDFe repositorioCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaDadosTransporte repositorioConfiguracaoCargaDadosTransporte = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaDadosTransporte(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = await repositorioConfiguracaoPedido.BuscarConfiguracaoPadraoAsync();

            Servicos.Embarcador.Carga.CargaLocaisPrestacao servicoCargaLocaisPrestacao = new Servicos.Embarcador.Carga.CargaLocaisPrestacao(unitOfWork);
            Servicos.Embarcador.Carga.CargaDadosSumarizados servicoCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);
            Servicos.Embarcador.Carga.RateioFrete servicoRateioFrete = new Servicos.Embarcador.Carga.RateioFrete(unitOfWork);

            bool existeIntegracaoTelhaNorte = await repositorioTipoIntegracao.ExistePorTipoAsync(TipoIntegracao.TelhaNorte);

            if (existeIntegracaoTelhaNorte && (carga.TipoOperacao?.AdicionarOuRemoverPedidosReplicaAosTrechosAnteriores ?? false))
            {
                List<int> codigosCargasTelhaNorte = await repositorioCarga.BuscarCodigosCargaEmCargasEEmCodigosAgrupadosAsync(carga.CodigoCargaEmbarcador);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidoTelha = repositorioCargaPedido.BuscarPorCargasEPedido(codigosCargasTelhaNorte, cargaPedidosRemover.FirstOrDefault()?.Pedido.Codigo ?? 0);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoTelha in cargasPedidoTelha)
                    RemoverPedidoCarga(cargaPedidoTelha, configuracaoEmbarcador, tipoServicoMultisoftware, unitOfWork, configuracaoGeralCarga, permitirRemoverTodos: permitirRemoverTodos, reentrega: reentrega, forcarRemocao: forcarRemocao, false, false);
            }
            else
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidosRemover)
                    RemoverPedidoCarga(cargaPedido, configuracaoEmbarcador, tipoServicoMultisoftware, unitOfWork, configuracaoGeralCarga, permitirRemoverTodos: permitirRemoverTodos, reentrega: reentrega, forcarRemocao: forcarRemocao);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaDadosTransporte configuracaoDadosTransporte = await repositorioConfiguracaoCargaDadosTransporte.BuscarPrimeiroRegistroAsync();
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedidos = await repositorioCargaPedido.BuscarPorCargaAsync(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> mdfes = await repositorioCargaMDFe.BuscarPorCargaAsync(carga.Codigo);

            mdfes.ForEach(x => x.CargaLocaisPrestacao = null);
            mdfes.ForEach(x => repositorioCargaMDFe.Atualizar(x));

            if (configuracaoEmbarcador.ExigirCargaRoteirizada || (carga.TipoOperacao?.ExigirCargaRoteirizada ?? false))
                carga.SituacaoRoteirizacaoCarga = SituacaoRoteirizacao.Aguardando;

            await new Servicos.Embarcador.Carga.RotaFrete(unitOfWork).SetarRotaFreteCargaAsync(carga, listaCargaPedidos, configuracaoEmbarcador, tipoServicoMultisoftware);

            if (listaCargaPedidos.Count > 0)
                servicoCargaLocaisPrestacao.VerificarEAjustarLocaisPrestacao(carga, listaCargaPedidos, unitOfWork, tipoServicoMultisoftware, configuracaoPedido);

            servicoCargaDadosSumarizados.AlterarDadosSumarizadosCarga(ref carga, listaCargaPedidos, configuracaoEmbarcador, unitOfWork, tipoServicoMultisoftware);

            if (carga.CargaEmitidaParcialmente)
            {
                servicoRateioFrete.RatearValorFreteCargaEmitidaParcialmente(carga, listaCargaPedidos, configuracaoEmbarcador, tipoServicoMultisoftware, unitOfWork);

                if (repositorioCargaPedido.VerificarSeTodosPedidosEstaoAutorizadosPorCarga(carga.Codigo))
                {
                    if (!carga.DataEnvioUltimaNFe.HasValue)
                    {
                        carga.DataInicioEmissaoDocumentos = DateTime.Now;
                        carga.DataEnvioUltimaNFe = DateTime.Now;
                    }

                    carga.DataInicioConfirmacaoDocumentosFiscais = DateTime.Now;
                    carga.ProcessandoDocumentosFiscais = true;
                }
            }
            else
            {
                if ((carga.TipoFreteEscolhido == TipoFreteEscolhido.Operador) || (carga.TipoFreteEscolhido == TipoFreteEscolhido.Cliente))
                {
                    if (carga.TipoFreteEscolhido == TipoFreteEscolhido.Operador)
                    {
                        servicoRateioFrete.RatearValorDoFrenteEntrePedidos(carga, listaCargaPedidos, configuracaoEmbarcador, false, unitOfWork, tipoServicoMultisoftware);

                        Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicao = (Servicos.Embarcador.Frete.ComposicaoFrete.ObterComposicaoFrete("Valor Informado Pelo Operador", " Valor Informado = " + carga.ValorFrete.ToString("n2"), carga.ValorFrete, TipoParametroBaseTabelaFrete.ValorFreteLiquido, TipoCampoValorTabelaFrete.ValorFixo, "Valor informado pelo Operador", 0, carga.ValorFrete));
                        Servicos.Embarcador.Carga.ComposicaoFrete.ComposicaoFrete.SetarComposicaoFrete(carga, null, null, null, false, composicao, unitOfWork, null);
                    }

                    if (carga.SituacaoCarga != SituacaoCarga.Nova && carga.SituacaoCarga != SituacaoCarga.AgNFe)
                        carga.SituacaoCarga = carga.ExigeNotaFiscalParaCalcularFrete ? SituacaoCarga.CalculoFrete : SituacaoCarga.AgTransportador;

                    if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                        Servicos.Log.TratarErro("Atualizou a situação para calculo frete 39 Carga " + carga.CodigoCargaEmbarcador, "AtualizouSituacaoCalculoFrete");
                }
                else if (carga.ExigeNotaFiscalParaCalcularFrete && !naoRecalcularFrete)
                {
                    if (carga.SituacaoCarga == SituacaoCarga.CalculoFrete)
                    {
                        carga.CalculandoFrete = true;
                        carga.DataInicioCalculoFrete = DateTime.Now;
                    }
                }
                else if (!naoRecalcularFrete && ((carga.SituacaoCarga == SituacaoCarga.AgTransportador) || (carga.SituacaoCarga == SituacaoCarga.CalculoFrete) || (carga.SituacaoCarga == SituacaoCarga.AgNFe)))
                {
                    carga.SituacaoCarga = SituacaoCarga.CalculoFrete;
                    carga.CalculandoFrete = true;
                    carga.DataInicioCalculoFrete = DateTime.Now;

                    if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                        Servicos.Log.TratarErro("Atualizou a situação para calculo frete 38 Carga " + carga.CodigoCargaEmbarcador, "AtualizouSituacaoCalculoFrete");
                }

                if (!carga.DataEnvioUltimaNFe.HasValue && repositorioCargaPedido.VerificarSeTodosPedidosEstaoAutorizadosPorCarga(carga.Codigo) && !carga.ExigeConfirmacaoAntesEmissao)
                {
                    carga.DataInicioEmissaoDocumentos = DateTime.Now;
                    carga.DataEnvioUltimaNFe = DateTime.Now;
                }

                carga.DataInicioConfirmacaoDocumentosFiscais = DateTime.Now;
                carga.ProcessandoDocumentosFiscais = true;
            }

            carga.DataAtualizacaoCarga = DateTime.Now;

            if (configuracaoDadosTransporte?.RetornarCargaPendenteConsultaCarregamentoAoSalvarDadosTransporte ?? false)
                carga.CarregamentoIntegradoERP = false;

            if (carga.Carregamento != null)
            {
                Embarcador.Carga.MontagemCarga.MontagemCarga servicoMontagemCarga = new MontagemCarga.MontagemCarga(unitOfWork);
                Integracao.IntegracaoCarregamento servicoIntegracaoCarregamento = new Integracao.IntegracaoCarregamento(unitOfWork);

                servicoMontagemCarga.AtualizarSituacaoExigeIscaPorCarga(carga);

                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoApolice repositorioCarregamentoApolice = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoApolice(unitOfWork);
                List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro> apolicesSeguro = repositorioCarregamentoApolice.BuscarApolicesPorCarregamento(carga.Carregamento.Codigo);

                servicoIntegracaoCarregamento.AdicionarIntegracaoCarregamento(carga.Carregamento, StatusCarregamentoIntegracao.Atualizar, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.TelhaNorte);

                if (apolicesSeguro.Count > 0 && listaCargaPedidos.Count > 0)
                {
                    Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao repApoliceSeguro = new Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao> listaApoliceSeguroAverbacao = repApoliceSeguro.BuscarPorCarga(carga.Codigo);

                    if (listaApoliceSeguroAverbacao.Count == 0)
                    {
                        foreach (Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apoliceSeguro in apolicesSeguro)
                        {
                            Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao apoliceSeguroAverbacao = new Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao()
                            {
                                CargaPedido = listaCargaPedidos.FirstOrDefault(),
                                ApoliceSeguro = apoliceSeguro
                            };

                            await repApoliceSeguro.InserirAsync(apoliceSeguroAverbacao);
                        }
                    }
                }
            }

            SumarizarDadosZonaTransporte(carga, unitOfWork);
            AtualizarIntegracaoCargaDadosTransporte(carga, unitOfWork);

            await repositorioCarga.AtualizarAsync(carga);
        }

        public static void RemoverPedidoCarga(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga, bool permitirRemoverTodos, bool? reentrega, bool forcarRemocao = false, bool replicarTrechoAnterior = true, bool replicarProximoTrecho = true, bool removerPedidoCarregamento = true)
        {
            if (cargaPedido == null)
                throw new ServicoException("Não foi possível encontrar o pedido.");

            if (cargaPedido.CargaPedidoProximoTrecho != null && !(cargaPedido.Carga.TipoOperacao?.AdicionarOuRemoverPedidosReplicaAosTrechosAnteriores ?? false) && !(cargaPedido.Carga.DadosSumarizados?.CargaTrecho == CargaTrechoSumarizada.SubCarga))
                throw new ServicoException("Não é possível remover o pedido " + cargaPedido.Pedido.NumeroPedidoEmbarcador + " pois o mesmo está vinculado a uma carga de próximo trecho (" + cargaPedido.CargaPedidoProximoTrecho.Carga.CodigoCargaEmbarcador + ").");

            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaPedido.Carga;
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaPedido.Pedido;
            int totalPedidosCarga = repositorioCargaPedido.ContarPorCarga(carga.Codigo);

            if (!forcarRemocao)
            {
                if (carga.CargaEmitidaParcialmente && cargaPedido.CTesEmitidos)
                    throw new ServicoException($"Os documentos do pedido {pedido.NumeroPedidoEmbarcador} já foram emitidos, não é possível remover.");

                if (!permitirRemoverTodos && (totalPedidosCarga <= 1))
                    throw new ServicoException($"A carga {carga.CodigoCargaEmbarcador} possui apenas um pedido, não é possível remover.", Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.PedidoVinculadoCarga);
            }

            if ((totalPedidosCarga <= 1) && (carga.DadosSumarizados.CargaTrecho == CargaTrechoSumarizada.SubCarga))
            {
                carga.CargaFechada = false;
                repositorioCarga.Atualizar(carga);
            }

            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repositorioPedidoCTeTerceiro = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalContaContabilContabilizacao repositorioPedidoXMLNotaFiscalContabilizacao = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalContaContabilContabilizacao(unitOfWork);
            Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao repApoliceSeguroAverbacao = new Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Escrituracao.DocumentoProvisao repositorioDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> cargaPedidoXMLNotaFiscalCTes = repCargaPedidoXMLNotaFiscalCTe.BuscarPorCargaPedido(cargaPedido.Codigo);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> listaCargaCTe = (from obj in cargaPedidoXMLNotaFiscalCTes select obj.CargaCTe).Distinct().ToList();
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> notasCte = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe>();

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> cargaXMLNotaFiscalCTes = repCargaPedidoXMLNotaFiscalCTe.BuscarPorCargaCTes((from obj in listaCargaCTe select obj.Codigo).ToList());

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe cargaPedidoXMLNotaFiscalCTe in cargaXMLNotaFiscalCTes)
            {
                if (cargaPedidoXMLNotaFiscalCTe.PedidoXMLNotaFiscal.CargaPedido.Codigo == cargaPedido.Codigo)
                    notasCte.Add(cargaPedidoXMLNotaFiscalCTe);
                else
                {
                    if (listaCargaCTe.Contains(cargaPedidoXMLNotaFiscalCTe.CargaCTe))
                        listaCargaCTe.Remove(cargaPedidoXMLNotaFiscalCTe.CargaCTe);
                }
            }

            if (!pedido.AdicionadaManualmente && !forcarRemocao)
            {
                ValidarPermissaoAdicionarOuRemoverPedido(carga, pedido, configuracaoEmbarcador, unitOfWork, adicionar: false, reentrega: reentrega);

                if (carga.SituacaoCarga == SituacaoCarga.PendeciaDocumentos)
                {
                    if (listaCargaCTe.Any(o => o.CTe != null))
                        throw new ServicoException("Existem CT-es vinculados à este(s) pedido(s), não sendo possível remover.");
                }

                if (repositorioPedidoCTeTerceiro.VerificarSeExistePorCargaPedido(cargaPedido.Codigo))
                    throw new ServicoException("Existem CT-es de Subcontratação vinculados à este(s) pedido(s), não sendo possível remover.");

                if (repositorioCargaPedido.VerificarCargaPedidoTrechoAnterior(cargaPedido.Codigo) && !(carga.TipoOperacao?.AdicionarOuRemoverPedidosReplicaAosTrechosAnteriores ?? false) && !(cargaPedido.Carga.DadosSumarizados?.CargaTrecho == CargaTrechoSumarizada.SubCarga))
                    throw new ServicoException("Um ou mais pedidos estão associados a um trecho anterior, não sendo possível remover.");

                if (repositorioCargaPedido.VerificarCargaPedidoProximoTrecho(cargaPedido.Codigo) && !(carga.TipoOperacao?.AdicionarOuRemoverPedidosReplicaAosTrechosAnteriores ?? false) && !(cargaPedido.Carga.DadosSumarizados?.CargaTrecho == CargaTrechoSumarizada.SubCarga))
                    throw new ServicoException("Um ou mais pedidos estão associados a um proximo trecho, não sendo possível remover.");
            }

            pedido.PedidoDePreCarga = false;

            //#53095 - Problema Braveo, não sei como.. mas Criou uma carga com o pedido... e depois tem log de 2x removeu o pedido da carga.. e com isso duplicou o saldo restante... if (pedido.PesoSaldoRestante + cargaPedido.Peso <= (pedido.PesoTotal + 0.5m))
            if (!cargaPedido.Carga.CargaTransbordo)
            {
                pedido.PesoSaldoRestante += cargaPedido.Peso;
                pedido.PedidoTotalmenteCarregado = false;
                //TODO: PPC - Adicionado log temporário para identificar problema de retorno de saldo de pedido.
                Servicos.Log.TratarErro($"Pedido {pedido.NumeroPedidoEmbarcador} - Liberou saldo pedido {pedido.PesoSaldoRestante} - Peso Total.: {pedido.PesoTotal} - Totalmente carregado.: {pedido.PedidoTotalmenteCarregado} - Carga.: {cargaPedido.Carga.CodigoCargaEmbarcador}. CargaPedido.RemoverPedidoCarga", "SaldoPedido");
            }
            else
                Servicos.Log.TratarErro($"Pedido {pedido.NumeroPedidoEmbarcador} - Liberou saldo pedido {pedido.PesoSaldoRestante} - Peso Total.: {pedido.PesoTotal} - Totalmente carregado.: {pedido.PedidoTotalmenteCarregado} - Carga Transbordo.: {cargaPedido.Carga.CodigoCargaEmbarcador}. CargaPedido.RemoverPedidoCarga", "SaldoPedido");

            pedido.CodigoCargaEmbarcador = null;
            pedido.ReentregaSolicitada = reentrega.HasValue ? reentrega.Value : false;

            if (pedido.ReentregaSolicitada)
                pedido.DataSolicitacaoReentrega = DateTime.Now;


            Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repositorioCargaPedidoComponenteFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoQuantidades repositorioCargaPedidoQuantidade = new Repositorio.Embarcador.Cargas.CargaPedidoQuantidades(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoTabelaFreteCliente repositorioCargaPedidoTabelaFrete = new Repositorio.Embarcador.Cargas.CargaPedidoTabelaFreteCliente(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPercurso repositorioCargaPercurso = new Repositorio.Embarcador.Cargas.CargaPercurso(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaRotaFrete repositorioCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repositorioCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repositorioCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido repSessaoRoteirizadorPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repositorioCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Canhotos.Canhoto repositorioCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
            Repositorio.Embarcador.Canhotos.CanhotoHistorico repositorioCanhotoHistorico = new Repositorio.Embarcador.Canhotos.CanhotoHistorico(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoProduto repositorioCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial repositorioCargaPedidoXMLNotaParcial = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalTemporario repositorioCargaPedidoXMLNotaFiscalTemporario = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalTemporario(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoNotaParcial repositorioPedidoNotaParcial = new Repositorio.Embarcador.Pedidos.PedidoNotaParcial(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete repPedidoXMLNotaFiscalComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete repCargaComposicaoFrete = new Repositorio.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete(unitOfWork);
            Repositorio.Embarcador.Canhotos.CanhotoAvulso repositorioCanhotoAvulso = new Repositorio.Embarcador.Canhotos.CanhotoAvulso(unitOfWork);
            Repositorio.Embarcador.Rateio.RateioCargaPedidoProduto repositorioRateioCargaPedidoProduto = new Repositorio.Embarcador.Rateio.RateioCargaPedidoProduto(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao repositoriocargaPedidoContaCotabilContabilizacao = new Repositorio.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao(unitOfWork);
            Repositorio.Embarcador.NotaFiscal.NaoConformidade repositorioNaoConformidade = new Repositorio.Embarcador.NotaFiscal.NaoConformidade(unitOfWork);
            Repositorio.Embarcador.NotaFiscal.AprovacaoAlcadaNaoConformidade repositorioAprovacaoNaoConformidade = new Repositorio.Embarcador.NotaFiscal.AprovacaoAlcadaNaoConformidade(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracao repositorioCargaCteIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeIntegracao(unitOfWork);
            Repositorio.Embarcador.Frete.ContratoSaldoMesCTe repositorioContratoSaldoMesCte = new Repositorio.Embarcador.Frete.ContratoSaldoMesCTe(unitOfWork);
            Repositorio.Embarcador.Rateio.RateioProdutoComponenteFrete repositorioRateioProdutoComponenteFrete = new Repositorio.Embarcador.Rateio.RateioProdutoComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoIntegracaoPacotes repositorioCargaPedidoIntegracaoPacotes = new Repositorio.Embarcador.Cargas.CargaPedidoIntegracaoPacotes(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoPacote repositorioCargaPedidoPacote = new Repositorio.Embarcador.Cargas.CargaPedidoPacote(unitOfWork);
            Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil repositorioDocumentoContabil = new Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil(unitOfWork);
            Repositorio.Embarcador.Documentos.ControleDocumento repositorioControleDocumento = new Repositorio.Embarcador.Documentos.ControleDocumento(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoRecusaCTE cargaPedidoRecusaCTE = new Repositorio.Embarcador.Cargas.CargaPedidoRecusaCTE(unitOfWork);
            Repositorio.Embarcador.Integracao.IntegracaoAVIPED repIntegracaoAVIPED = new Repositorio.Embarcador.Integracao.IntegracaoAVIPED(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            Servicos.Embarcador.Carga.CargaDadosSumarizados servicoCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);
            Servicos.Embarcador.Carga.CargaLocaisPrestacao servicoCargaLocaisPrestacao = new Servicos.Embarcador.Carga.CargaLocaisPrestacao(unitOfWork);
            Servicos.Embarcador.Pedido.Produto serProduto = new Servicos.Embarcador.Pedido.Produto(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> componentesFreteCargaPedido = repositorioCargaPedidoComponenteFrete.BuscarPorCargaPedido(cargaPedido.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades> quantidadesCargaPedido = repositorioCargaPedidoQuantidade.BuscarPorCargaPedido(cargaPedido.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoTabelaFreteCliente> tabelasFreteClienteCargaPedido = repositorioCargaPedidoTabelaFrete.BuscarPorCargaPedido(cargaPedido.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPercurso> listaCargaPercursos = repositorioCargaPercurso.ConsultarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido> sessoes = repSessaoRoteirizadorPedido.BuscarPorPedido(cargaPedido.Pedido.Codigo);
            List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos = repositorioCanhoto.BuscarPorCargaPedido(cargaPedido.Codigo);
            List<Dominio.Entidades.Embarcador.Canhotos.CanhotoHistorico> historicosCanhoto = repositorioCanhotoHistorico.BuscarPorCanhotos(canhotos.Select(o => o.Codigo).ToList());
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial> notasParciais = repositorioCargaPedidoXMLNotaParcial.BuscarPorCargaPedido(cargaPedido.Codigo);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoNotaParcial> notasParciaisPedido = repositorioPedidoNotaParcial.BuscarPorPedido(cargaPedido.Pedido.Codigo);
            Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = repositorioCargaRotaFrete.BuscarPorCarga(carga.Codigo);

            List<Dominio.Entidades.Embarcador.Canhotos.CanhotoAvulso> canhotosAvulsos = repositorioCanhotoAvulso.BuscarPorCargaPedido(cargaPedido.Codigo);
            List<Dominio.Entidades.Embarcador.Rateio.RateioCargaPedidoProduto> rateiosCargaPedidoProduto = repositorioRateioCargaPedidoProduto.buscarPorCargaPedido(cargaPedido.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao> cargaPedidoContas = repositoriocargaPedidoContaCotabilContabilizacao.BuscarPorCargaPedido(cargaPedido.Codigo);
            List<Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade> naoConformidades = repositorioNaoConformidade.BuscarPorCargaPedido(cargaPedido.Codigo);
            List<int> codigosNaoConformidades = (naoConformidades != null && naoConformidades.Count > 0) ? naoConformidades.Select(s => s.Codigo).ToList() : new List<int>();
            List<Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.AprovacaoAlcadaNaoConformidade> alcadasNaoConformidades = repositorioAprovacaoNaoConformidade.BuscarPorNaoConformidades(codigosNaoConformidades);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoIntegracaoPacotes> cargaPedidoIntegracaoPacotes = repositorioCargaPedidoIntegracaoPacotes.BuscarPorCargaPedido(cargaPedido.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote> cargaPedidoPacotes = repositorioCargaPedidoPacote.BuscarPorCargaPedido(cargaPedido.Codigo);

            repositorioCargaEntregaNotaFiscal.ExcluirCargaEntregaNotaFiscalPorCargaPedido(cargaPedido.Codigo);
            repCargaEntregaPedido.ExcluirPorCargaPedido(cargaPedido.Codigo);

            if (removerPedidoCarregamento)
                new Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga(unitOfWork, configuracaoEmbarcador).RemoverPedido(carga.Carregamento, pedido);

            if (forcarRemocao)
            {
                bool possuiNotaFiscalVinculada = cargaPedido.NotasFiscais.Any(x => x.XMLNotaFiscal != null);

                if (possuiNotaFiscalVinculada)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> xmlsVinculadoNoCargaPedido = cargaPedido?.NotasFiscais?.Where(x => x.XMLNotaFiscal != null).Select(x => x.XMLNotaFiscal).ToList();
                    foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal nota in xmlsVinculadoNoCargaPedido)
                    {
                        if (pedido.NotasFiscais.Where(x => x.Codigo == nota.Codigo).Any())
                            continue;

                        pedido.NotasFiscais.Add(nota);
                    }

                }
            }

            Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repositorioConfiguracaoPedido.BuscarPrimeiroRegistro();

            if (configuracaoPedido.RemoverObservacoesDeEntregaAoRemoverPedidoCarga)
                pedido.ObservacaoEntrega = null;

            repositorioPedido.Atualizar(pedido);

            repCargaComposicaoFrete.DeletarPorCarga(carga.Codigo, false, cargaPedido.Codigo);

            foreach (Dominio.Entidades.Embarcador.Canhotos.CanhotoHistorico canhotoHistorico in historicosCanhoto)
                repositorioCanhotoHistorico.Deletar(canhotoHistorico);

            foreach (Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto in canhotos)
                repositorioCanhoto.Deletar(canhoto);

            foreach (Dominio.Entidades.Embarcador.Canhotos.CanhotoAvulso canhotoAvulso in canhotosAvulsos)
            {
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXmlNotaFiscalRemover = canhotoAvulso.PedidosXMLNotasFiscais.Where(obj => obj.CargaPedido.Codigo == cargaPedido.Codigo).ToList();

                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXmlNotaFiscalRemover in pedidosXmlNotaFiscalRemover)
                    canhotoAvulso.PedidosXMLNotasFiscais.Remove(pedidoXmlNotaFiscalRemover);

                repositorioCanhotoAvulso.Atualizar(canhotoAvulso);
            }

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe notaCTe in notasCte)
                repCargaPedidoXMLNotaFiscalCTe.Deletar(notaCTe);

            unitOfWork.Flush();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoTabelaFreteCliente tabelaFreteClienteCargaPedido in tabelasFreteClienteCargaPedido)
                repositorioCargaPedidoTabelaFrete.Deletar(tabelaFreteClienteCargaPedido);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades quantidadeCargaPedido in quantidadesCargaPedido)
                repositorioCargaPedidoQuantidade.Deletar(quantidadeCargaPedido);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete componenteFreteCargaPedido in componentesFreteCargaPedido)
                repositorioCargaPedidoComponenteFrete.Deletar(componenteFreteCargaPedido);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPercurso cargaPercurso in listaCargaPercursos)
                repositorioCargaPercurso.Deletar(cargaPercurso);


            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in listaCargaCTe)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao> cargaCTeIntegracaos = repositorioCargaCteIntegracao.BuscarPorCargaCTe(cargaCTe.Codigo);
                List<Dominio.Entidades.Embarcador.Frete.ContratoSaldoMesCTe> saldoMesCte = repositorioContratoSaldoMesCte.BuscarPorCargaCte(cargaCTe.Codigo);
                Dominio.Entidades.Embarcador.Documentos.ControleDocumento controleDocumentos = repositorioControleDocumento.BuscarPorCodigoCargaCtE(cargaCTe.Codigo);

                if (controleDocumentos != null)
                {
                    controleDocumentos.CargaCTe = null;
                    repositorioControleDocumento.Atualizar(controleDocumentos);
                }

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao cargaCTeIntegracao in cargaCTeIntegracaos)
                    repositorioCargaCteIntegracao.Deletar(cargaCTeIntegracao);

                foreach (Dominio.Entidades.Embarcador.Frete.ContratoSaldoMesCTe mesCte in saldoMesCte)
                    repositorioContratoSaldoMesCte.Deletar(mesCte);

                if (cargaCTe.CargaCTeTrechoAnterior != null)
                {
                    cargaCTe.CargaCTeTrechoAnterior.CargaCTeProximoTrecho = null;
                    repositorioCargaCTe.Atualizar(cargaCTe.CargaCTeTrechoAnterior);
                }

                repositorioCargaCTe.Deletar(cargaCTe);
            }

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial notaParcial in notasParciais)
            {
                //ao remover esse parcial vamos armazenar em uma tabela temporaria para possivel retorno do pedido a carga.
                Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalTemporario cargaPedidoXMLTemporario = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalTemporario();
                cargaPedidoXMLTemporario.Carga = carga;
                cargaPedidoXMLTemporario.Pedido = pedido;
                cargaPedidoXMLTemporario.NumeroFatura = notaParcial.NumeroFatura;
                cargaPedidoXMLTemporario.Status = notaParcial.Status;
                cargaPedidoXMLTemporario.XMLNotaFiscal = notaParcial.XMLNotaFiscal;
                cargaPedidoXMLTemporario.Chave = notaParcial.Chave;
                cargaPedidoXMLTemporario.TipoNotaFiscalIntegrada = notaParcial.TipoNotaFiscalIntegrada;

                repositorioCargaPedidoXMLNotaFiscalTemporario.Inserir(cargaPedidoXMLTemporario);
                Servicos.Log.TratarErro($"Inseriu Carga Pedido XML temporario carga {carga.Codigo} Pedido {pedido.Protocolo} [{DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")}]");

                repositorioCargaPedidoXMLNotaParcial.Deletar(notaParcial);
            }


            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoIntegracaoPacotes cargaPedidoIntegracaoPacote in cargaPedidoIntegracaoPacotes)
                repositorioCargaPedidoIntegracaoPacotes.Deletar(cargaPedidoIntegracaoPacote);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote cargaPedidoPacote in cargaPedidoPacotes)
                repositorioCargaPedidoPacote.Deletar(cargaPedidoPacote);

            foreach (Dominio.Entidades.Embarcador.Rateio.RateioCargaPedidoProduto rateioCargaPedidoProduto in rateiosCargaPedidoProduto)
            {
                List<Dominio.Entidades.Embarcador.Rateio.RateioProdutoComponenteFrete> componentesRateioCargaPedido = repositorioRateioProdutoComponenteFrete.BuscarPorRateioCargaProduto(rateioCargaPedidoProduto.Codigo);

                foreach (var componente in componentesRateioCargaPedido)
                    repositorioRateioProdutoComponenteFrete.Deletar(componente);

                repositorioRateioCargaPedidoProduto.Deletar(rateioCargaPedidoProduto);
            }

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoContaContabilContabilizacao contaContabilContabilizacao in cargaPedidoContas)
                repositoriocargaPedidoContaCotabilContabilizacao.Deletar(contaContabilContabilizacao);

            foreach (Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.AprovacaoAlcadaNaoConformidade alcadaNaoConformidade in alcadasNaoConformidades)
                repositorioAprovacaoNaoConformidade.Deletar(alcadaNaoConformidade);

            foreach (Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade naoConformidade in naoConformidades)
                repositorioNaoConformidade.Deletar(naoConformidade);

            repositorioPedidoXMLNotaFiscalContabilizacao.DeletarPorCargaPedido(cargaPedido.Codigo);

            repPedidoXMLNotaFiscalComponenteFrete.DeletarPorCargaPedido(cargaPedido.Codigo);

            List<int> numerosNotasFiscais = repositorioPedidoXMLNotaFiscal.BuscarNumeroNotasFiscaisPorCargaPedido(cargaPedido.Codigo);

            foreach (int numeroNotaFiscal in numerosNotasFiscais)
            {
                if (notasParciaisPedido.Any(obj => obj.Numero == numeroNotaFiscal))
                    continue;

                Dominio.Entidades.Embarcador.Pedidos.PedidoNotaParcial pedidoNotaParcialAdicionar = new Dominio.Entidades.Embarcador.Pedidos.PedidoNotaParcial()
                {
                    Numero = numeroNotaFiscal,
                    NumeroPedido = cargaPedido.Pedido.NumeroPedidoEmbarcador,
                    Pedido = cargaPedido.Pedido,
                    DataCriacao = DateTime.Now
                };

                repositorioPedidoNotaParcial.Inserir(pedidoNotaParcialAdicionar);
            }

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotasFiscais = repositorioPedidoXMLNotaFiscal.BuscarPorCargaPedido(cargaPedido.Codigo);

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in pedidoXMLNotasFiscais)
            {
                Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documentosProvisao = repositorioDocumentoProvisao.BuscarPorXMLNotaFiscalECarga(pedidoXMLNotaFiscal.XMLNotaFiscal.Codigo, carga.Codigo);
                if (documentosProvisao != null)
                {
                    if ((documentosProvisao.Provisao != null) && (documentosProvisao.Provisao.Situacao != SituacaoProvisao.Cancelado) && !forcarRemocao)
                        throw new ServicoException($"A nota fiscal está provisionada na provisão de número {documentosProvisao.Provisao.Numero}. Para  excluir a nota é necesário cancelar a provisão antes.");

                    repositorioDocumentoContabil.ExcluirTodosPorDocumentoProvisao(documentosProvisao.Codigo);
                    repositorioDocumentoContabil.ExcluirTodosPorDocumentoProvisaoReferencia(documentosProvisao.Codigo);

                    repositorioDocumentoProvisao.Deletar(documentosProvisao);
                }

                repIntegracaoAVIPED.DeletarPorPedidoXMLNotaFiscal(pedidoXMLNotaFiscal.Codigo);
                repositorioPedidoXMLNotaFiscal.Deletar(pedidoXMLNotaFiscal);
            }

            repIntegracaoAVIPED.DeletarPorCargaPedido(cargaPedido.Codigo);
            repositorioCargaPedidoProduto.DeletarPorCargaPedido(cargaPedido.Codigo);
            repApoliceSeguroAverbacao.DeletarPorCargaPedido(cargaPedido.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedido sessao in sessoes)
            {
                sessao.Situacao = SituacaoSessaoRoteirizadorPedido.RemovidoPedidoSessao;
                repSessaoRoteirizadorPedido.Atualizar(sessao);
            }

            if (cargaRotaFrete != null)
            {
                Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem repositorioCargaRotaFretePontosPassagem = new Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> listaPontosPassagens = repositorioCargaRotaFretePontosPassagem.BuscarPorCargaRotaFrete(cargaRotaFrete.Codigo);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem pontoPassagem in listaPontosPassagens)
                    repositorioCargaRotaFretePontosPassagem.Deletar(pontoPassagem);

                repositorioCargaRotaFrete.Deletar(cargaRotaFrete);
            }

            bool permitirRemoverTrechos = ((carga.TipoOperacao?.AdicionarOuRemoverPedidosReplicaAosTrechosAnteriores ?? false) || (carga.DadosSumarizados.CargaTrecho == CargaTrechoSumarizada.SubCarga));

            if (permitirRemoverTrechos)
            {
                if (replicarTrechoAnterior && (cargaPedido.CargaPedidoTrechoAnterior != null) && (!cargaPedidoRecusaCTE.ExisteRecusaPorPedidoECargaGerada(cargaPedido.CargaPedidoTrechoAnterior.Pedido.Protocolo, cargaPedido.CargaPedidoTrechoAnterior.Carga.Codigo)))
                {
                    if (repositorioCargaPedido.ContarPorTrechosAnteriores(cargaPedido.CargaPedidoTrechoAnterior.Codigo) > 1)
                        throw new ServicoException(Localization.Resources.Cargas.Pedido.NaoEPossivelRemoverEstePedidoPoisETrechoAnteriorVinculadoVariasOutrasCargas);

                    RemoverPedidoCarga(cargaPedido.CargaPedidoTrechoAnterior, configuracaoEmbarcador, tipoServicoMultisoftware, unitOfWork, configuracaoGeralCarga, permitirRemoverTodos, reentrega, forcarRemocao, replicarProximoTrecho: false);
                }

                if (replicarProximoTrecho && (cargaPedido.CargaPedidoProximoTrecho != null) && (!cargaPedidoRecusaCTE.ExisteRecusaPorPedidoECargaGerada(cargaPedido.CargaPedidoProximoTrecho.Pedido.Protocolo, cargaPedido.CargaPedidoProximoTrecho.Carga.Codigo)))
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosProximoTrecho = repositorioCargaPedido.BuscarPorTrechosAnteriores(cargaPedido.Codigo);

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoProximoTrecho in cargaPedidosProximoTrecho)
                        RemoverPedidoCarga(cargaPedidoProximoTrecho, configuracaoEmbarcador, tipoServicoMultisoftware, unitOfWork, configuracaoGeralCarga, permitirRemoverTodos, reentrega, forcarRemocao, replicarTrechoAnterior: false);
                }
            }

            if (cargaPedido.CargaPedidoProximoTrecho != null)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidoTrechosAnteriores = repositorioCargaPedido.BuscarPorTrechosAnteriores(cargaPedido.Codigo);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoTrechoAnterior in cargaPedidoTrechosAnteriores)
                {
                    cargaPedidoTrechoAnterior.CargaPedidoTrechoAnterior = null;
                    repositorioCargaPedido.Atualizar(cargaPedidoTrechoAnterior);
                }
            }

            if (cargaPedido.CargaPedidoTrechoAnterior?.CargaPedidoProximoTrecho != null)
            {
                cargaPedido.CargaPedidoTrechoAnterior.CargaPedidoProximoTrecho = null;
                repositorioCargaPedido.Atualizar(cargaPedido.CargaPedidoTrechoAnterior);
            }

            if (carga.TipoOperacao?.TipoConsolidacao == EnumTipoConsolidacao.PreCheckIn)
            {
                // se estamos removendo um pedido de consolidado, vamos ter q encontrar as cargas filhos desse pedido e remover das cargas filho tambem.
                Repositorio.Embarcador.Pedidos.StageAgrupamento repStageAgrupamento = new Repositorio.Embarcador.Pedidos.StageAgrupamento(unitOfWork);
                Servicos.Embarcador.Carga.Frete serFrete = new Servicos.Embarcador.Carga.Frete(unitOfWork, tipoServicoMultisoftware);

                List<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento> stagesAgrupamentos = repStageAgrupamento.BuscarPorCargaDt(carga.Codigo);

                foreach (var stageAgrupamento in stagesAgrupamentos)
                {
                    if (stageAgrupamento.CargaGerada != null)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoCargafilhoRemover = repositorioCargaPedido.BuscarPorCargaEPedidoFetch(stageAgrupamento.CargaGerada.Codigo, cargaPedido.Pedido.Codigo);
                        if (cargaPedidoCargafilhoRemover != null && !cargaPedidoRecusaCTE.ExisteRecusaPorPedidoECarga(cargaPedidoCargafilhoRemover.Pedido.Protocolo, cargaPedidoCargafilhoRemover.Carga.Codigo))
                        {
                            RemoverPedidoCarga(cargaPedidoCargafilhoRemover, configuracaoEmbarcador, tipoServicoMultisoftware, unitOfWork, configuracaoGeralCarga, permitirRemoverTodos, reentrega, forcarRemocao, true, true);

                            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> todosCargaPedidoCargaGerada = repositorioCargaPedido.BuscarPorCarga(stageAgrupamento.CargaGerada.Codigo);
                            Servicos.Embarcador.Carga.RotaFrete.SetarRotaFreteCarga(stageAgrupamento.CargaGerada, todosCargaPedidoCargaGerada, configuracaoEmbarcador, unitOfWork, tipoServicoMultisoftware);
                            Servicos.Embarcador.Carga.CargaRotaFrete.GerarIntegracoesRoteirizacaoCarga(stageAgrupamento.CargaGerada, unitOfWork, configuracaoEmbarcador, tipoServicoMultisoftware);

                            //Carga de consolidado recalcular o frete da carga gerada apenas se ja tem veiculo, e motoristas e nao esta avançada..
                            bool dadosTransporteInformados = (
                                    (stageAgrupamento.CargaGerada.TipoDeCarga != null) &&
                                    (stageAgrupamento.CargaGerada.ModeloVeicularCarga != null) &&
                                    (stageAgrupamento.CargaGerada.Veiculo != null) &&
                                    (!(stageAgrupamento.CargaGerada.TipoOperacao?.ExigePlacaTracao ?? false) || ((stageAgrupamento.CargaGerada.VeiculosVinculados?.Count ?? 0) == stageAgrupamento.CargaGerada.ModeloVeicularCarga.NumeroReboques)));

                            if (!dadosTransporteInformados && !(stageAgrupamento.CargaGerada.TipoOperacao?.ExigeNotaFiscalParaCalcularFrete ?? false))
                            {
                                stageAgrupamento.CargaGerada.SituacaoCarga = SituacaoCarga.CalculoFrete;
                                stageAgrupamento.CargaGerada.CalculandoFrete = true;
                                stageAgrupamento.CargaGerada.PossuiPendencia = false;
                                stageAgrupamento.CargaGerada.ProblemaIntegracaoValePedagio = false;
                                stageAgrupamento.CargaGerada.MotivoPendencia = "";
                                stageAgrupamento.CargaGerada.MotivoPendenciaFrete = MotivoPendenciaFrete.NenhumPendencia;
                                stageAgrupamento.CargaGerada.DadosPagamentoInformadosManualmente = false;
                                stageAgrupamento.CargaGerada.DataInicioCalculoFrete = DateTime.Now;
                                stageAgrupamento.CargaGerada.PendenciaEmissaoAutomatica = false;
                            }
                            else if (dadosTransporteInformados && todosCargaPedidoCargaGerada.FirstOrDefault()?.NotasFiscais?.Count > 0 && stageAgrupamento.CargaGerada.SituacaoCarga != SituacaoCarga.AgIntegracao && stageAgrupamento.CargaGerada.SituacaoCarga != SituacaoCarga.EmTransporte && stageAgrupamento.CargaGerada.SituacaoCarga != SituacaoCarga.AgImpressaoDocumentos)
                            {
                                stageAgrupamento.CargaGerada.SituacaoCarga = SituacaoCarga.CalculoFrete;
                                stageAgrupamento.CargaGerada.CalculandoFrete = true;
                                stageAgrupamento.CargaGerada.PossuiPendencia = false;
                                stageAgrupamento.CargaGerada.ProblemaIntegracaoValePedagio = false;
                                stageAgrupamento.CargaGerada.MotivoPendencia = "";
                                stageAgrupamento.CargaGerada.MotivoPendenciaFrete = MotivoPendenciaFrete.NenhumPendencia;
                                stageAgrupamento.CargaGerada.DadosPagamentoInformadosManualmente = false;
                                stageAgrupamento.CargaGerada.DataInicioCalculoFrete = DateTime.Now;
                                stageAgrupamento.CargaGerada.PendenciaEmissaoAutomatica = false;
                            }


                            repositorioCarga.Atualizar(stageAgrupamento.CargaGerada);

                        }
                        else
                        {
                            //carga de transferencia ou entrega, precisamos recalcular o frete
                            if (!(stageAgrupamento.CargaGerada.TipoOperacao?.ExigeNotaFiscalParaCalcularFrete ?? false))
                            {
                                stageAgrupamento.CargaGerada.SituacaoCarga = SituacaoCarga.CalculoFrete;
                                stageAgrupamento.CargaGerada.CalculandoFrete = true;
                                stageAgrupamento.CargaGerada.PossuiPendencia = false;
                                stageAgrupamento.CargaGerada.ProblemaIntegracaoValePedagio = false;
                                stageAgrupamento.CargaGerada.MotivoPendencia = "";
                                stageAgrupamento.CargaGerada.MotivoPendenciaFrete = MotivoPendenciaFrete.NenhumPendencia;
                                stageAgrupamento.CargaGerada.DadosPagamentoInformadosManualmente = false;
                                stageAgrupamento.CargaGerada.DataInicioCalculoFrete = DateTime.Now;
                                stageAgrupamento.CargaGerada.PendenciaEmissaoAutomatica = false;

                                repositorioCarga.Atualizar(stageAgrupamento.CargaGerada);
                            }
                        }

                    }
                }
            }

            if (carga.TipoOperacao != null && carga.TipoOperacao.ConfiguracaoCarga != null && carga.TipoOperacao.ConfiguracaoCarga.RetornarSituacaoAoRemoverPedidos)
            {
                carga.SituacaoCarga = carga.TipoOperacao.ConfiguracaoCarga.SituacaoAposRemocaoPedidos;
            }

            unitOfWork.Flush();

            repositorioCargaPedido.Deletar(cargaPedido);

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repositorioTipoIntegracao.BuscarPorTipo(TipoIntegracao.Routeasy);
            if (tipoIntegracao != null)
            {
                new Servicos.Embarcador.Carga.CargaPedidoHistorico(unitOfWork).CriarCargaPedidoHistoricoAsync(CargaPedidoHistoricoTipoAcao.Exclusao, CargaPedidoHistoricoSituacaoIntegracao.Aguardando, pedido, carga).GetAwaiter().GetResult();

                Servicos.Embarcador.Integracao.IntegracaoCarga.AdicionarIntegracaoRouteasy(carga, unitOfWork);
                Servicos.Embarcador.Integracao.IntegracaoCarga.AdicionarCargaDadosTransporteParaIntegracao(carga, tipoIntegracao, false, false, tipoServicoMultisoftware, unitOfWork);
            }

            SumarizarDadosZonaTransporte(carga, unitOfWork);
            AtualizarIntegracaoCargaDadosTransporte(carga, unitOfWork);
            new Servicos.Embarcador.Integracao.IntegracaoCarga(unitOfWork).AdicionarIntegracoesCargaFrete(carga, unitOfWork);
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedido AdicionarPedidoCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool integracaoRouteasy = false)
        {
            NumeroReboque numeroReboque = default;
            TipoCarregamentoPedido tipoCarregamentoPedido = default;

            return AdicionarPedidoCarga(carga, pedido, numeroReboque, tipoCarregamentoPedido, configuracaoEmbarcador, tipoServicoMultisoftware, _unitOfWork, integracaoRouteasy: integracaoRouteasy);
        }

        public static Dominio.Entidades.Embarcador.Cargas.CargaPedido AdicionarPedidoCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, NumeroReboque numeroReboque, TipoCarregamentoPedido tipoCarregamentoPedido, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, bool integracaoRouteasy = false)
        {
            return AdicionarPedidoCarga(carga, pedido, numeroReboque, tipoCarregamentoPedido, configuracaoEmbarcador, tipoServicoMultisoftware, unitOfWork, permitirSomentePedidoMesmaFilial: false, integracaoRouteasy: integracaoRouteasy);
        }

        public static Dominio.Entidades.Embarcador.Cargas.CargaPedido AdicionarPedidoCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, NumeroReboque numeroReboque, TipoCarregamentoPedido tipoCarregamentoPedido, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, bool permitirSomentePedidoMesmaFilial, bool naoAtualizarDadosSumarizados = false, bool replicarProximoTrecho = true, bool replicarTrechoAnterior = true, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoTrecho = null, bool integracaoRouteasy = false)
        {
            if (pedido.SituacaoPedido == SituacaoPedido.Cancelado)
                throw new ServicoException("O pedido está cancelado.");

            ValidarPermissaoAdicionarOuRemoverPedido(carga, pedido, configuracaoEmbarcador, unitOfWork, adicionar: true, integracaoRouteasy: integracaoRouteasy);

            bool exigirDefinicaoReboquePedido = (carga.ModeloVeicularCarga?.ExigirDefinicaoReboquePedido ?? false) && (carga.ModeloVeicularCarga?.NumeroReboques > 1);

            if (exigirDefinicaoReboquePedido && (numeroReboque == NumeroReboque.SemReboque))
                throw new ServicoException($"O reboque não foi definido para o pedido");

            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedidoAnterior = repositorioCargaPedido.BuscarPorPedidoComCargaAtiva(pedido.Codigo);

            if (!pedido.ReentregaSolicitada && pedido.PedidoTotalmenteCarregado && (listaCargaPedidoAnterior?.Count > 0) && !(carga.TipoOperacao?.AdicionarOuRemoverPedidosReplicaAosTrechosAnteriores ?? false))
                throw new ServicoException("Pedido já está em outra carga: " + listaCargaPedidoAnterior.FirstOrDefault().Carga.CodigoCargaEmbarcador);

            if (permitirSomentePedidoMesmaFilial)
            {
                if (carga.Filial == null)
                    throw new ServicoException("A carga deve possuir uma filial.");

                if (pedido.Filial == null)
                    throw new ServicoException("O pedido deve possuir uma filial.");

                if (carga.Filial.Codigo != pedido.Filial.Codigo)
                    throw new ServicoException("O pedido deve pertencer a mesma filial da carga.");
            }

            Dominio.Entidades.Embarcador.Cargas.CargaPedido primeiroCargaPedidoDaCarga = repositorioCargaPedido.BuscarPrimeiroPedidoPorCarga(carga.Codigo);

            pedido.PedidoDePreCarga = carga.CargaDePreCarga;
            pedido.CodigoCargaEmbarcador = carga.CodigoCargaEmbarcador;
            pedido.DataCarregamentoPedido = primeiroCargaPedidoDaCarga?.Pedido.DataCarregamentoPedido;

            if (!(pedido.TipoOperacao?.ConfiguracaoAgendamentoColetaEntrega?.UtilizarDataSaidaGuaritaComoTerminoCarregamento ?? false))
                pedido.DataTerminoCarregamento = primeiroCargaPedidoDaCarga?.Pedido.DataTerminoCarregamento;

            repositorioPedido.Atualizar(pedido);

            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPercurso repositorioCargaPercurso = new Repositorio.Embarcador.Cargas.CargaPercurso(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaRotaFrete repositorioCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaDadosTransporte repositorioConfiguracaoCargaDadosTransporte = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaDadosTransporte(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoProduto repPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto repCarregamentoPedidoProduto = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);
            Servicos.Embarcador.Carga.CargaDadosSumarizados servicoCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);
            Servicos.Embarcador.Carga.CargaLocaisPrestacao servicoCargaLocaisPrestacao = new Servicos.Embarcador.Carga.CargaLocaisPrestacao(unitOfWork);
            Servicos.Embarcador.Pedido.NotaFiscal serCargaNotaFiscal = new Servicos.Embarcador.Pedido.NotaFiscal(unitOfWork);
            Servicos.Embarcador.Carga.RateioFrete servicoRateioFrete = new Servicos.Embarcador.Carga.RateioFrete(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaDadosTransporte configuracaoDadosTransporte = repositorioConfiguracaoCargaDadosTransporte.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork).BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repositorioConfiguracaoPedido.BuscarConfiguracaoPadrao();
            List<Dominio.Entidades.Embarcador.Cargas.CargaPercurso> listaCargaPercursos = repositorioCargaPercurso.ConsultarPorCarga(carga.Codigo);
            Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = repositorioCargaRotaFrete.BuscarPorCarga(carga.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPercurso cargaPercurso in listaCargaPercursos)
                repositorioCargaPercurso.Deletar(cargaPercurso);

            if (cargaRotaFrete != null)
            {
                Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem repositorioCargaRotaFretePontosPassagem = new Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> listaPontosPassagens = repositorioCargaRotaFretePontosPassagem.BuscarPorCargaRotaFrete(cargaRotaFrete.Codigo);

                foreach (var pontoPassagem in listaPontosPassagens)
                    repositorioCargaRotaFretePontosPassagem.Deletar(pontoPassagem);

                repositorioCargaRotaFrete.Deletar(cargaRotaFrete);
            }

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = Servicos.Embarcador.Carga.CargaPedido.CriarCargaPedido(carga, pedido, null, unitOfWork, unitOfWork.StringConexao, tipoServicoMultisoftware, configuracaoEmbarcador, pedidoEncaixe: false, numeroReboque: numeroReboque, tipoCarregamentoPedido: tipoCarregamentoPedido, configuracaoGeralCarga);

            if (cargaPedido != null && (pedido.PedidoNotasParciais?.Count > 0 || !string.IsNullOrWhiteSpace(pedido.NumeroControle)))
            {
                List<int> numerosNotasFiscais = new List<int>();

                if (pedido.PedidoNotasParciais?.Count > 0)
                    numerosNotasFiscais.AddRange(pedido.PedidoNotasParciais.Select(obj => obj.Numero).ToList());

                if (!string.IsNullOrWhiteSpace(pedido.NumeroControle))
                    numerosNotasFiscais.Add(pedido.NumeroControle.ToInt());

                Pedido.NotaFiscal servicoNotaFiscal = new Pedido.NotaFiscal(unitOfWork);
                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial repositorioCargaPedidoNotaParcial = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais = repositorioXmlNotaFiscal.BuscarPorNumeros(numerosNotasFiscais);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial> cargaPedidoNotasParciais = repositorioCargaPedidoNotaParcial.BuscarPorCargaPedido(cargaPedido.Codigo);

                foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal in notasFiscais)
                {
                    if (!cargaPedidoNotasParciais.Any(obj => obj.Numero == notaFiscal.Numero))
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial notaFiscalParcial = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial()
                        {
                            CargaPedido = cargaPedido,
                            Chave = notaFiscal.Chave,
                            Numero = notaFiscal.Numero
                        };

                        repositorioCargaPedidoNotaParcial.Inserir(notaFiscalParcial);
                        Servicos.Log.TratarErro($"2 Adicionando Pedidos Parciais {DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")} Pedido: [{cargaPedido.Pedido.Codigo}]");
                    }

                    servicoNotaFiscal.VincularXMLNotaFiscal(notaFiscal, configuracaoEmbarcador, tipoServicoMultisoftware, null, false, false);
                }
            }

            if (pedido.NotasFiscais?.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal in pedido.NotasFiscais.ToList())
                    serCargaNotaFiscal.InserirNotaCargaPedido(xmlNotaFiscal, cargaPedido, tipoServicoMultisoftware, TipoNotaFiscal.Venda, configuracaoEmbarcador, false, out bool alteradoTipoDeCarga);
            }

            pedido.PedidoTotalmenteCarregado = true;
            pedido.PesoSaldoRestante = 0;
            pedido.SaldoVolumesRestante = 0;
            pedido.TipoOperacao = carga.TipoOperacao;
            pedido.Empresa = carga.Empresa;

            repositorioPedido.Atualizar(pedido);

            //TODO: PPC - Adicionado log temporário para identificar problema de retorno de saldo de pedido.
            Servicos.Log.TratarErro($"Pedido {pedido.NumeroPedidoEmbarcador} - Zerou saldo pedido {pedido.PesoSaldoRestante} - Peso Total.: {pedido.PesoTotal} - Totalmente carregado.: {pedido.PedidoTotalmenteCarregado}. CargaPedido.AdicionarPedidoCarga", "SaldoPedido");

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedidos = repositorioCargaPedido.BuscarPorCarga(carga.Codigo);

            if (!naoAtualizarDadosSumarizados)
            {
                if ((configuracaoEmbarcador.ExigirCargaRoteirizada || (carga.TipoOperacao?.ExigirCargaRoteirizada ?? false)) && carga.SituacaoRoteirizacaoCarga == SituacaoRoteirizacao.Concluido)
                    carga.SituacaoRoteirizacaoCarga = SituacaoRoteirizacao.Aguardando;

                Servicos.Embarcador.Carga.RotaFrete.SetarRotaFreteCarga(carga, listaCargaPedidos, configuracaoEmbarcador, unitOfWork, tipoServicoMultisoftware);
                servicoCargaLocaisPrestacao.VerificarEAjustarLocaisPrestacao(carga, listaCargaPedidos, unitOfWork, tipoServicoMultisoftware, configuracaoPedido);
            }

            if (carga.Carregamento != null)
            {
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido carregamentoPedido = repCarregamentoPedido.BuscarPorCarregamentoEPedido(pedido.Codigo, carga.Carregamento.Codigo);
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> pedidosProduto = repPedidoProduto.BuscarPorPedido(pedido.Codigo);

                Integracao.IntegracaoCarregamento servicoIntegracaoCarregamento = new Integracao.IntegracaoCarregamento(unitOfWork);

                if (carregamentoPedido == null)
                {
                    carregamentoPedido = new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido();
                    carregamentoPedido.Carregamento = carga.Carregamento;
                    carregamentoPedido.Pedido = pedido;
                    carregamentoPedido.Peso = cargaPedido.Peso;
                    carregamentoPedido.Pallet = cargaPedido.Pallet;

                    if ((pedido?.TipoPaleteCliente ?? TipoPaleteCliente.NaoDefinido) != TipoPaleteCliente.NaoDefinido)
                    {
                        Repositorio.Embarcador.Pedidos.TipoDetalhe repositorioTipoDetalhe = new Repositorio.Embarcador.Pedidos.TipoDetalhe(unitOfWork);
                        List<Dominio.Entidades.Embarcador.Pedidos.TipoDetalhe> tiposPallet = repositorioTipoDetalhe.BuscarPorTipo(TipoTipoDetalhe.TipoPallet);

                        Dominio.Entidades.Embarcador.Pedidos.TipoDetalhe tipoDetalhePalete = (from o in tiposPallet where o.TipoPaleteCliente == pedido.TipoPaleteCliente select o).FirstOrDefault();
                        if (tipoDetalhePalete != null)
                            carregamentoPedido.PesoPallet = (carregamentoPedido.Pallet * tipoDetalhePalete?.Valor ?? 0);
                    }

                    repCarregamentoPedido.Inserir(carregamentoPedido);

                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos = repCargaPedidoProduto.BuscarPorCargaPedido(cargaPedido.Codigo);

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto cargaPedidoProduto in cargaPedidoProdutos)
                    {
                        Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto carregamentoPedidoProduto = new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto();

                        carregamentoPedidoProduto.CarregamentoPedido = carregamentoPedido;
                        carregamentoPedidoProduto.MetroCubico = cargaPedidoProduto.Produto.MetroCubito;
                        carregamentoPedidoProduto.PedidoProduto = (from obj in pedidosProduto where obj.Produto.Codigo == cargaPedidoProduto.Produto.Codigo select obj).FirstOrDefault();
                        carregamentoPedidoProduto.Peso = cargaPedidoProduto.PesoTotal;
                        carregamentoPedidoProduto.Quantidade = cargaPedidoProduto.Quantidade;
                        carregamentoPedidoProduto.QuantidadeOriginal = cargaPedidoProduto.Quantidade;
                        carregamentoPedidoProduto.MetroCubicoOriginal = cargaPedidoProduto.Produto.MetroCubito;

                        repCarregamentoPedidoProduto.Inserir(carregamentoPedidoProduto);
                    }
                }

                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao carregamentoIntegracao = servicoIntegracaoCarregamento.AdicionarIntegracaoCarregamento(carga.Carregamento, StatusCarregamentoIntegracao.Atualizar, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.TelhaNorte);

                Embarcador.Carga.MontagemCarga.MontagemCarga servicoMontagemCarga = new MontagemCarga.MontagemCarga(unitOfWork);
                servicoMontagemCarga.AtualizarSituacaoExigeIscaPorCarga(carga);
            }

            if (!naoAtualizarDadosSumarizados)
                servicoCargaDadosSumarizados.AlterarDadosSumarizadosCarga(ref carga, listaCargaPedidos, configuracaoEmbarcador, unitOfWork, tipoServicoMultisoftware);

            if (carga.CargaEmitidaParcialmente)
                servicoRateioFrete.RatearValorFreteCargaEmitidaParcialmente(carga, listaCargaPedidos, configuracaoEmbarcador, tipoServicoMultisoftware, unitOfWork);
            else if ((carga.TipoFreteEscolhido == TipoFreteEscolhido.Operador || carga.TipoFreteEscolhido == TipoFreteEscolhido.Cliente))
            {
                if (carga.TipoFreteEscolhido == TipoFreteEscolhido.Operador)
                {
                    servicoRateioFrete.RatearValorDoFrenteEntrePedidos(carga, listaCargaPedidos, configuracaoEmbarcador, false, unitOfWork, tipoServicoMultisoftware);

                    Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicao = (Servicos.Embarcador.Frete.ComposicaoFrete.ObterComposicaoFrete("Valor Informado Pelo Operador", " Valor Informado = " + carga.ValorFrete.ToString("n2"), carga.ValorFrete, TipoParametroBaseTabelaFrete.ValorFreteLiquido, TipoCampoValorTabelaFrete.ValorFixo, "Valor informado pelo Operador", 0, carga.ValorFrete));
                    Servicos.Embarcador.Carga.ComposicaoFrete.ComposicaoFrete.SetarComposicaoFrete(carga, null, null, null, false, composicao, unitOfWork, null);
                }

                if (carga.ExigeNotaFiscalParaCalcularFrete)
                {
                    if (carga.DataEnvioUltimaNFe.HasValue)
                        carga.SituacaoCarga = SituacaoCarga.CalculoFrete;

                    if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                        Servicos.Log.TratarErro("Atualizou a situação para calculo frete 37 Carga " + carga.CodigoCargaEmbarcador, "AtualizouSituacaoCalculoFrete");
                }
                else
                    carga.SituacaoCarga = SituacaoCarga.AgTransportador;
            }
            else if (carga.ExigeNotaFiscalParaCalcularFrete)
            {
                if (carga.SituacaoCarga == SituacaoCarga.CalculoFrete)
                {
                    carga.CalculandoFrete = true;
                    carga.DataInicioCalculoFrete = DateTime.Now;
                }
            }
            else if (carga.SituacaoCarga == SituacaoCarga.AgTransportador || carga.SituacaoCarga == SituacaoCarga.CalculoFrete || carga.SituacaoCarga == SituacaoCarga.AgNFe)
            {
                carga.SituacaoCarga = SituacaoCarga.CalculoFrete;
                carga.CalculandoFrete = true;
                carga.DataInicioCalculoFrete = DateTime.Now;
                if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                    Servicos.Log.TratarErro("Atualizou a situação para calculo frete 36 Carga " + carga.CodigoCargaEmbarcador, "AtualizouSituacaoCalculoFrete");
            }

            carga.DataAtualizacaoCarga = DateTime.Now;

            if (configuracaoDadosTransporte?.RetornarCargaPendenteConsultaCarregamentoAoSalvarDadosTransporte ?? false)
                carga.CarregamentoIntegradoERP = false;

            SumarizarDadosZonaTransporte(carga, unitOfWork);
            DefinirCanalVendaPorDestinatario(cargaPedido, listaCargaPedidos, unitOfWork);
            AtualizarIntegracaoCargaDadosTransporte(carga, unitOfWork);
            new Servicos.Embarcador.Integracao.IntegracaoCarga(unitOfWork).AdicionarIntegracoesCargaFrete(carga, unitOfWork);

            repositorioCarga.Atualizar(carga);

            if (carga.TipoOperacao?.AdicionarOuRemoverPedidosReplicaAosTrechosAnteriores ?? false)
            {
                if (cargaPedidoTrecho != null && replicarProximoTrecho)
                {
                    cargaPedidoTrecho.CargaPedidoProximoTrecho = cargaPedido;
                    cargaPedido.CargaPedidoTrechoAnterior = cargaPedidoTrecho;
                }

                if (cargaPedidoTrecho != null && replicarTrechoAnterior)
                {
                    cargaPedidoTrecho.CargaPedidoTrechoAnterior = cargaPedido;
                    cargaPedido.CargaPedidoProximoTrecho = cargaPedidoTrecho;
                }

                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasAnteriores = replicarTrechoAnterior ? repositorioCargaPedido.BuscarCargasAnteriores(cargaPedido.Carga.Codigo) : null;
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasProximoTrecho = replicarProximoTrecho ? repositorioCargaPedido.BuscarCargasProximoTrecho(cargaPedido.Carga.Codigo) : null;

                if (replicarTrechoAnterior && cargasAnteriores != null && cargasAnteriores.Count > 0)
                    foreach (var cargaAnterior in cargasAnteriores)
                        AdicionarPedidoCarga(cargaAnterior, cargaPedido.Pedido, numeroReboque, tipoCarregamentoPedido, configuracaoEmbarcador, tipoServicoMultisoftware, unitOfWork, naoAtualizarDadosSumarizados, replicarProximoTrecho: false, cargaPedidoTrecho: cargaPedido);

                if (replicarProximoTrecho && cargasProximoTrecho != null && cargasProximoTrecho.Count > 0)
                    foreach (var cargaProximoTrecho in cargasProximoTrecho)
                        AdicionarPedidoCarga(cargaProximoTrecho, cargaPedido.Pedido, numeroReboque, tipoCarregamentoPedido, configuracaoEmbarcador, tipoServicoMultisoftware, unitOfWork, naoAtualizarDadosSumarizados, replicarTrechoAnterior: false, cargaPedidoTrecho: cargaPedido);

            }

            return cargaPedido;
        }

        public static Dominio.Entidades.Embarcador.Cargas.Carga AdicionarPedidoOutraFilialCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, DateTime dataCarregamento, NumeroReboque numeroReboque, TipoCarregamentoPedido tipoCarregamentoPedido, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente ClienteMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            if (pedido.SituacaoPedido == SituacaoPedido.Cancelado)
                throw new ServicoException("O pedido está cancelado.");

            ValidarPermissaoAdicionarOuRemoverPedido(carga, pedido, configuracaoEmbarcador, unitOfWork, adicionar: true);

            bool exigirDefinicaoReboquePedido = (carga.ModeloVeicularCarga?.ExigirDefinicaoReboquePedido ?? false) && (carga.ModeloVeicularCarga?.NumeroReboques > 1);

            if (exigirDefinicaoReboquePedido && (numeroReboque == NumeroReboque.SemReboque))
                throw new ServicoException($"O reboque não foi definido para o pedido");

            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedidoAnterior = repositorioCargaPedido.BuscarPorPedidoComCargaAtiva(pedido.Codigo);

            if (!pedido.ReentregaSolicitada && (listaCargaPedidoAnterior?.Count > 0))
                throw new ServicoException($"Pedido já está em outra carga: {listaCargaPedidoAnterior.FirstOrDefault().Carga.CodigoCargaEmbarcador}");

            if (carga.Filial == null)
                throw new ServicoException("A carga deve possuir uma filial.");

            if (pedido.Filial == null)
                throw new ServicoException("O pedido deve possuir uma filial.");

            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Servicos.Embarcador.Carga.CargaDadosSumarizados servicoCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);
            bool possuiPedidoReferenciado = false;

            if (carga.Filial.Codigo == pedido.Filial.Codigo)
            {
                if (string.IsNullOrWhiteSpace(pedido.RefEXPTransferencia))
                    throw new ServicoException("O pedido não pode pertencer a mesma filial da carga.");

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoReferenciado = repositorioCargaPedido.BuscarPorCargaENumeroEXP(carga.Codigo, pedido.RefEXPTransferencia);

                if (cargaPedidoReferenciado == null)
                    throw new ServicoException($"A carga não possui nenhum pedido com o número EXP referenciado ({pedido.RefEXPTransferencia}).");

                pedido.Filial = repositorioFilial.BuscarPorCNPJ(pedido.Destinatario?.CPF_CNPJ_SemFormato ?? "") ?? throw new ServicoException("não foi possível encontrar a filial do pedido com número EXP referenciado.");
                Servicos.Log.TratarErro($"1 - Pedido (Código: {pedido.Codigo} - Número: {pedido.NumeroPedidoEmbarcador}) trocou de remetente de {pedido.Remetente.Codigo} para {pedido.Destinatario.Codigo}.", "TrocaRemetentePedido");
                pedido.Remetente = pedido.Destinatario;
                pedido.Expedidor = pedido.Expedidor != null ? pedido.Destinatario : null;
                pedido.Destinatario = cargaPedidoReferenciado.Pedido.Destinatario;
                pedido.Recebedor = cargaPedidoReferenciado.Pedido.Recebedor;

                possuiPedidoReferenciado = true;
            }

            pedido.PedidoDePreCarga = carga.CargaDePreCarga;

            repositorioPedido.Atualizar(pedido);

            Dominio.Entidades.Embarcador.Cargas.Carga cargaNova = servicoCarga.GerarCargaPorCargaEPedidos(carga, new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>() { pedido }, pedido.Filial, configuracaoEmbarcador, tipoServicoMultisoftware, ClienteMultisoftware, unitOfWork, dataCarregamento, adicionarJanelaCarregamento: false, adicionarJanelaDescarregamento: false, numeroReboque: numeroReboque, tipoCarregamentoPedido: tipoCarregamentoPedido);
            Dominio.Entidades.Embarcador.Cargas.Carga cargaAgrupada = new CargaAgrupada(unitOfWork, configuracaoEmbarcador).AgruparCargas(new List<Dominio.Entidades.Embarcador.Cargas.Carga>() { carga, cargaNova }, null, carga.Carregamento?.NumeroCarregamento ?? "", carga.Filial, carga, tipoServicoMultisoftware, ClienteMultisoftware);

            if (!possuiPedidoReferenciado)
                servicoCarga.ValidarCapacidadeModeloVeicularCarga(cargaAgrupada, configuracaoEmbarcador, unitOfWork);

            cargaNova.TipoCondicaoPagamento = carga.TipoCondicaoPagamento;
            cargaNova.ProblemaIntegracaoGrMotoristaVeiculo = carga.ProblemaIntegracaoGrMotoristaVeiculo;
            cargaNova.LiberadoComProblemaIntegracaoGrMotoristaVeiculo = carga.LiberadoComProblemaIntegracaoGrMotoristaVeiculo;
            cargaNova.MensagemProblemaIntegracaoGrMotoristaVeiculo = carga.MensagemProblemaIntegracaoGrMotoristaVeiculo;
            cargaNova.ProtocoloIntegracaoGR = carga.ProtocoloIntegracaoGR;

            cargaAgrupada.TipoCondicaoPagamento = carga.TipoCondicaoPagamento;
            cargaAgrupada.ProblemaIntegracaoGrMotoristaVeiculo = carga.ProblemaIntegracaoGrMotoristaVeiculo;
            cargaAgrupada.LiberadoComProblemaIntegracaoGrMotoristaVeiculo = carga.LiberadoComProblemaIntegracaoGrMotoristaVeiculo;
            cargaAgrupada.MensagemProblemaIntegracaoGrMotoristaVeiculo = carga.MensagemProblemaIntegracaoGrMotoristaVeiculo;
            cargaAgrupada.ProtocoloIntegracaoGR = carga.ProtocoloIntegracaoGR;

            repositorioCarga.Atualizar(cargaNova);
            repositorioCarga.Atualizar(cargaAgrupada);

            if (auditado != null)
                Auditoria.Auditoria.Auditar(auditado, cargaAgrupada, null, $"Criada pelo agrupamento das cargas {string.Join(", ", (from obj in cargaAgrupada.CodigosAgrupados select obj).ToList())}", unitOfWork);

            return cargaAgrupada;
        }

        public static void DesfazerTrocaPedidoCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracao.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();


            if (!configuracaoEmbarcador.PermitirTrocarPedidoCarga)
                throw new ServicoException($"A configuração atual não permite desfazer a troca de pedido da carga.");

            Repositorio.Embarcador.Pedidos.PedidoTroca repositorioPedidoTroca = new Repositorio.Embarcador.Pedidos.PedidoTroca(unitOfWork);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoTroca> pedidosTroca = repositorioPedidoTroca.BuscarPorPedidoDefinitivo(pedido.Codigo);

            if (pedidosTroca.Count() == 0)
                throw new ServicoException($"não foram encontrados pedidos para desfazer a troca.");

            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosProvisorio = pedidosTroca.Select(o => o.PedidoProvisorio).Distinct().ToList();

            if (pedidosProvisorio.Count == 1)
                pedidosTroca = repositorioPedidoTroca.BuscarPorPedidoProvisorio(pedidosProvisorio.FirstOrDefault().Codigo);

            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosDefinitivo = pedidosTroca.Select(o => o.PedidoDefinitivo).Distinct().ToList();
            List<int> codigosPedidosProvisorios = (from o in pedidosProvisorio select o.Codigo).ToList();
            List<int> codigosPedidosDefinitivos = (from o in pedidosDefinitivo select o.Codigo).ToList();
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repositorioCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidosProvisorios = (carga.Carregamento != null) ? repositorioCarregamentoPedido.BuscarPorCarregamentoEPedidos(carga.Carregamento.Codigo, codigosPedidosProvisorios) : new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosDefinitivos = repositorioCargaPedido.BuscarPorCargaEPedidos(carga.Codigo, codigosPedidosDefinitivos);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosAdicionados = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            Dominio.Entidades.Embarcador.Cargas.CargaPedido primeiroCargaPedidoDefinitivos = cargaPedidosDefinitivos.FirstOrDefault();

            foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoProvisorio in pedidosProvisorio)
            {
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido carregamentoPedido = (from o in carregamentoPedidosProvisorios where o.Pedido.Codigo == pedidoProvisorio.Codigo select o).FirstOrDefault();
                NumeroReboque numeroReboque = carregamentoPedido?.NumeroReboque ?? primeiroCargaPedidoDefinitivos?.NumeroReboque ?? NumeroReboque.SemReboque;
                TipoCarregamentoPedido tipoCarregamentoPedido = carregamentoPedido?.TipoCarregamentoPedido ?? TipoCarregamentoPedido.Normal;

                cargaPedidosAdicionados.Add(Servicos.Embarcador.Carga.CargaPedido.AdicionarPedidoCarga(carga, pedidoProvisorio, numeroReboque, tipoCarregamentoPedido, configuracaoEmbarcador, tipoServicoMultisoftware, unitOfWork));
                Servicos.Auditoria.Auditoria.Auditar(auditado, carga, null, $"Adicionou o pedido {pedidoProvisorio.NumeroPedidoEmbarcador}", unitOfWork);
            }

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoDefinitivo in cargaPedidosDefinitivos)
            {
                Servicos.Embarcador.Carga.CargaPedido.RemoverPedidoCarga(carga, cargaPedidoDefinitivo, configuracaoEmbarcador, tipoServicoMultisoftware, unitOfWork, configuracaoGeralCarga);
                Servicos.Auditoria.Auditoria.Auditar(auditado, carga, null, $"Removeu o pedido {cargaPedidoDefinitivo.Pedido.NumeroPedidoEmbarcador}", unitOfWork);
            }

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoTroca pedidoTroca in pedidosTroca)
                repositorioPedidoTroca.Deletar(pedidoTroca);

            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta servicoCargaJanelaCarregamentoConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = servicoCargaJanelaCarregamentoConsulta.ObterCargaJanelaCarregamentoPorCarga(carga.Codigo);

            Servicos.Embarcador.Carga.CargaRotaFrete.GerarIntegracoesRoteirizacaoCarga(carga, unitOfWork, configuracaoEmbarcador, tipoServicoMultisoftware);
            servicoCarga.ValidarCapacidadeModeloVeicularCarga(carga, configuracaoEmbarcador, unitOfWork);
            servicoCarga.AtualizarCargaJanelaCarregamento(carga, cargaJanelaCarregamento, configuracaoEmbarcador, unitOfWork);
            servicoCarga.AtualizarCargaJanelaDescarregamento(carga, cargaJanelaCarregamento, configuracaoEmbarcador, unitOfWork, cargaPedidosAdicionados, cargaPedidosDefinitivos);
        }

        public static List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades> ObterCargaPedidoQuantidades(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoQuantidades repCargaQuantidades = new Repositorio.Embarcador.Cargas.CargaPedidoQuantidades(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades> cargaPedidoQuantidades = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades>();

            for (int i = 0; i < cargaPedidos.Count; i++)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades> quantidadesCargaPedido = repCargaQuantidades.BuscarPorCargaPedido(cargaPedidos[i].Codigo);
                if (quantidadesCargaPedido.Count > 0)
                    cargaPedidoQuantidades.AddRange(quantidadesCargaPedido);
            }

            return cargaPedidoQuantidades;
        }

        public static int ObterQuantidadeDocumentosEmitir(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);

            int totalCTes = 0;

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                if (cargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Normal)
                {
                    switch (cargaPedido.TipoRateio)
                    {
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalAgrupadaEntrePedidos:
                            totalCTes += repPedidoXMLNotaFiscal.ContarAgrupadosPorRemetenteEDestinatario(cargaPedido.Codigo, false, 0);
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalAgrupada:
                            totalCTes += repPedidoXMLNotaFiscal.ContarAgrupadosPorRemetenteEDestinatarioEModalidade(cargaPedido.Codigo, false);
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalIndividual:
                            totalCTes += repPedidoXMLNotaFiscal.ContarPorCargaPedido(cargaPedido.Codigo);
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorPedidoAgrupado:
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorPedidoIndividual:
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.NaoInformado:
                            totalCTes += 1;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    totalCTes += repPedidoCTeParaSubContratacao.ContarPorCargaPedido(cargaPedido.Codigo);
                }
            }

            return totalCTes;
        }

        public static void CriarPreviaDocumentoCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPreviaDocumento repCargaPreviaDocumento = new Repositorio.Embarcador.Cargas.CargaPreviaDocumento(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPreviaDocumentoDocumento repCargaPreviaDocumentoNotaFiscal = new Repositorio.Embarcador.Cargas.CargaPreviaDocumentoDocumento(unitOfWork);
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubcontratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);

            Servicos.Embarcador.Carga.CTe serCargaCte = new CTe(unitOfWork);

            repCargaPreviaDocumento.DeletarPorCarga(carga.Codigo);

            Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscalNFSe = repModeloDocumentoFiscal.BuscarPorModelo("39");
            Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscalCTe = repModeloDocumentoFiscal.BuscarPorModelo("57");

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);

            Servicos.Embarcador.Carga.CargaPedido svcCargaPedido = new CargaPedido(unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                bool emiteOutroModeloDocumento = false;

                if (cargaPedido.ModeloDocumentoFiscal != null && cargaPedido.ModeloDocumentoFiscal != modeloDocumentoFiscalCTe && cargaPedido.ModeloDocumentoFiscal != modeloDocumentoFiscalNFSe)
                    emiteOutroModeloDocumento = true;

                if (cargaPedido.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Normal)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotasFiscais = repPedidoXMLNotaFiscal.BuscarPorCargaPedidoFetch(cargaPedido.Codigo);

                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos tipoEmissaoDocumentos = cargaPedido.TipoRateio;
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes tipoEmissaoCTeParticipantes = cargaPedido.TipoEmissaoCTeParticipantes;

                    bool usarTipoPagamentoNF = cargaPedido.Pedido.UsarTipoPagamentoNF;

                    Dominio.Entidades.Cliente recebedor = null;
                    Dominio.Entidades.Cliente expedidor = null;
                    List<Dominio.ObjetosDeValor.Embarcador.Frete.ParticipantesSumarizadosCalculoFrete> participantes = null;

                    if (tipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor || tipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor)
                        recebedor = cargaPedido.Recebedor;

                    if (tipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor || tipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor)
                        expedidor = cargaPedido.Expedidor;

                    if (tipoEmissaoDocumentos == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalAgrupada
                        || tipoEmissaoDocumentos == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalAgrupadaEntrePedidos)
                    {
                        if (usarTipoPagamentoNF)
                            participantes = pedidoXMLNotasFiscais.Select(o => new Dominio.ObjetosDeValor.Embarcador.Frete.ParticipantesSumarizadosCalculoFrete() { Destinatario = o.XMLNotaFiscal.Destinatario, Remetente = o.XMLNotaFiscal.Emitente, Modalidade = o.XMLNotaFiscal.ModalidadeFrete }).Distinct().ToList();
                        else
                            participantes = pedidoXMLNotasFiscais.Select(o => new Dominio.ObjetosDeValor.Embarcador.Frete.ParticipantesSumarizadosCalculoFrete() { Destinatario = o.XMLNotaFiscal.Destinatario, Remetente = o.XMLNotaFiscal.Emitente }).Distinct().ToList();

                        foreach (Dominio.ObjetosDeValor.Embarcador.Frete.ParticipantesSumarizadosCalculoFrete participante in participantes)
                        {
                            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotasFiscaisAgrupados = null;

                            if (usarTipoPagamentoNF)
                                pedidoXMLNotasFiscaisAgrupados = pedidoXMLNotasFiscais.Where(o => o.XMLNotaFiscal.Emitente.CPF_CNPJ == participante.Remetente.CPF_CNPJ && o.XMLNotaFiscal.Destinatario.CPF_CNPJ == participante.Destinatario.CPF_CNPJ && o.XMLNotaFiscal.ModalidadeFrete == participante.Modalidade).ToList();
                            else
                                pedidoXMLNotasFiscaisAgrupados = pedidoXMLNotasFiscais.Where(o => o.XMLNotaFiscal.Emitente.CPF_CNPJ == participante.Remetente.CPF_CNPJ && o.XMLNotaFiscal.Destinatario.CPF_CNPJ == participante.Destinatario.CPF_CNPJ).ToList();

                            if (emiteOutroModeloDocumento)
                            {
                                AdicionarPreviaDocumento(carga, cargaPedido.ModeloDocumentoFiscal, pedidoXMLNotasFiscaisAgrupados, null, unitOfWork);
                                continue;
                            }

                            bool possuiCTe = false;
                            bool possuiNFS = false;
                            bool possuiNFSManual = false;
                            Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscalIntramunicipal = null;
                            Dominio.Entidades.Localidade localidadeOrigem = expedidor?.Localidade ?? participante.Remetente?.Localidade;
                            Dominio.Entidades.Localidade localidadeDestino = recebedor?.Localidade ?? participante.Destinatario?.Localidade;

                            svcCargaPedido.VerificarQuaisDocumentosDeveEmitir(carga, cargaPedido, localidadeOrigem, localidadeDestino, tipoServicoMultisoftware, unitOfWork, out possuiCTe, out possuiNFS, out possuiNFSManual, out modeloDocumentoFiscalIntramunicipal, configuracao, out bool sempreDisponibilizarDocumentoNFSManual);

                            Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal = null;

                            if (possuiNFS)
                                modeloDocumentoFiscal = modeloDocumentoFiscalNFSe;
                            else if (possuiCTe)
                                modeloDocumentoFiscal = modeloDocumentoFiscalCTe;
                            else if (possuiNFSManual && modeloDocumentoFiscalIntramunicipal != null)
                                modeloDocumentoFiscal = modeloDocumentoFiscalIntramunicipal;

                            if (modeloDocumentoFiscal != null)
                                AdicionarPreviaDocumento(carga, modeloDocumentoFiscal, pedidoXMLNotasFiscaisAgrupados, null, unitOfWork);
                        }
                    }
                    else if (tipoEmissaoDocumentos == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalIndividual)
                    {
                        if (emiteOutroModeloDocumento)
                        {
                            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in pedidoXMLNotasFiscais)
                                AdicionarPreviaDocumento(carga, cargaPedido.ModeloDocumentoFiscal, new List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> { pedidoXMLNotaFiscal }, null, unitOfWork);

                            continue;
                        }

                        if (expedidor != null && recebedor != null)
                        {
                            bool possuiCTe = false;
                            bool possuiNFS = false;
                            bool possuiNFSManual = false;
                            Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscalIntramunicipal = null;

                            svcCargaPedido.VerificarQuaisDocumentosDeveEmitir(carga, cargaPedido, expedidor.Localidade, recebedor.Localidade, tipoServicoMultisoftware, unitOfWork, out possuiCTe, out possuiNFS, out possuiNFSManual, out modeloDocumentoFiscalIntramunicipal, configuracao, out bool sempreDisponibilizarDocumentoNFSManual);

                            Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscalEmitir = null;

                            if (possuiNFS)
                                modeloDocumentoFiscalEmitir = modeloDocumentoFiscalNFSe;
                            else if (possuiCTe)
                                modeloDocumentoFiscalEmitir = modeloDocumentoFiscalCTe;
                            else if (possuiNFSManual && modeloDocumentoFiscalIntramunicipal != null)
                                modeloDocumentoFiscalEmitir = modeloDocumentoFiscalIntramunicipal;

                            if (modeloDocumentoFiscalEmitir != null)
                            {
                                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in pedidoXMLNotasFiscais)
                                    AdicionarPreviaDocumento(carga, modeloDocumentoFiscalEmitir, new List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> { pedidoXMLNotaFiscal }, null, unitOfWork);
                            }
                        }
                        else
                        {
                            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in pedidoXMLNotasFiscais)
                            {
                                bool possuiCTe = false;
                                bool possuiNFS = false;
                                bool possuiNFSManual = false;
                                Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscalIntramunicipal = null;
                                Dominio.Entidades.Localidade localidadeOrigem = expedidor?.Localidade ?? pedidoXMLNotaFiscal.XMLNotaFiscal.Emitente?.Localidade;
                                Dominio.Entidades.Localidade localidadeDestino = recebedor?.Localidade ?? pedidoXMLNotaFiscal.XMLNotaFiscal.Destinatario?.Localidade;

                                svcCargaPedido.VerificarQuaisDocumentosDeveEmitir(carga, cargaPedido, localidadeOrigem, localidadeDestino, tipoServicoMultisoftware, unitOfWork, out possuiCTe, out possuiNFS, out possuiNFSManual, out modeloDocumentoFiscalIntramunicipal, configuracao, out bool sempreDisponibilizarDocumentoNFSManual);

                                Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal = null;

                                if (possuiNFS)
                                    modeloDocumentoFiscal = modeloDocumentoFiscalNFSe;
                                else if (possuiCTe)
                                    modeloDocumentoFiscal = modeloDocumentoFiscalCTe;
                                else if (possuiNFSManual && modeloDocumentoFiscalIntramunicipal != null)
                                    modeloDocumentoFiscal = modeloDocumentoFiscalIntramunicipal;

                                if (modeloDocumentoFiscal != null)
                                    AdicionarPreviaDocumento(carga, modeloDocumentoFiscal, new List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> { pedidoXMLNotaFiscal }, null, unitOfWork);
                            }
                        }
                    }
                    else if (tipoEmissaoDocumentos == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorPedidoIndividual || tipoEmissaoDocumentos == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorPedidoAgrupado)
                    {
                        if (emiteOutroModeloDocumento)
                        {
                            AdicionarPreviaDocumento(carga, cargaPedido.ModeloDocumentoFiscal, pedidoXMLNotasFiscais, null, unitOfWork);
                            continue;
                        }

                        bool possuiCTe = false;
                        bool possuiNFS = false;
                        bool possuiNFSManual = false;
                        Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscalIntramunicipal = null;
                        Dominio.Entidades.Localidade localidadeOrigem = expedidor?.Localidade ?? cargaPedido.Pedido.Origem;
                        Dominio.Entidades.Localidade localidadeDestino = recebedor?.Localidade ?? cargaPedido.Pedido.Destino;

                        svcCargaPedido.VerificarQuaisDocumentosDeveEmitir(carga, cargaPedido, localidadeOrigem, localidadeDestino, tipoServicoMultisoftware, unitOfWork, out possuiCTe, out possuiNFS, out possuiNFSManual, out modeloDocumentoFiscalIntramunicipal, configuracao, out bool sempreDisponibilizarDocumentoNFSManual);

                        Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal = null;

                        if (possuiNFS)
                            modeloDocumentoFiscal = modeloDocumentoFiscalNFSe;
                        else if (possuiCTe)
                            modeloDocumentoFiscal = modeloDocumentoFiscalCTe;
                        else if (possuiNFSManual && modeloDocumentoFiscalIntramunicipal != null)
                            modeloDocumentoFiscal = modeloDocumentoFiscalIntramunicipal;

                        if (modeloDocumentoFiscal != null)
                            AdicionarPreviaDocumento(carga, modeloDocumentoFiscal, pedidoXMLNotasFiscais, null, unitOfWork);
                    }
                }
                else
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidoCTesParaSubcontratacao = repPedidoCTeParaSubcontratacao.BuscarPorCargaPedido(cargaPedido.Codigo);

                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubcontratacao in pedidoCTesParaSubcontratacao)
                    {
                        if (emiteOutroModeloDocumento)
                        {
                            AdicionarPreviaDocumento(carga, cargaPedido.ModeloDocumentoFiscal, null, new List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>() { pedidoCTeParaSubcontratacao }, unitOfWork);
                            continue;
                        }

                        Dominio.Entidades.Localidade inicioPrestacao = pedidoCTeParaSubcontratacao.CTeTerceiro.LocalidadeInicioPrestacao;
                        Dominio.Entidades.Localidade terminoPrestacao = pedidoCTeParaSubcontratacao.CTeTerceiro.LocalidadeTerminoPrestacao;
                        Dominio.Entidades.Cliente remetente = pedidoCTeParaSubcontratacao.CTeTerceiro.Remetente.Cliente;
                        Dominio.Entidades.Cliente destinatario = pedidoCTeParaSubcontratacao.CTeTerceiro.Destinatario.Cliente;

                        if (pedidoCTeParaSubcontratacao.CTeTerceiro.Expedidor != null)
                        {
                            inicioPrestacao = pedidoCTeParaSubcontratacao.CTeTerceiro.Expedidor.Cliente.Localidade;
                            remetente = pedidoCTeParaSubcontratacao.CTeTerceiro.Expedidor.Cliente;
                        }
                        else if (cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor || cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor && cargaPedido.Expedidor != null)
                        {
                            inicioPrestacao = cargaPedido.Expedidor.Localidade;
                            remetente = cargaPedido.Expedidor;
                        }

                        if (pedidoCTeParaSubcontratacao.CTeTerceiro.Recebedor != null)
                        {
                            terminoPrestacao = pedidoCTeParaSubcontratacao.CTeTerceiro.Recebedor.Cliente.Localidade;
                            destinatario = pedidoCTeParaSubcontratacao.CTeTerceiro.Recebedor.Cliente;
                        }
                        else if ((cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor || cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor) && cargaPedido.Recebedor != null && !(cargaPedido.Carga.TipoOperacao?.UtilizarRecebedorApenasComoParticipante ?? false))
                        {
                            terminoPrestacao = cargaPedido.Recebedor.Localidade;
                            destinatario = cargaPedido.Recebedor;
                        }

                        svcCargaPedido.VerificarQuaisDocumentosDeveEmitir(carga, cargaPedido, remetente.Localidade, destinatario.Localidade, tipoServicoMultisoftware, unitOfWork, out bool possuiCTe, out bool possuiNFS, out bool possuiNFSManual, out Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscalIntramunicipal, configuracao, out bool sempreDisponibilizarDocumentoNFSManual);

                        Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal = null;

                        if (possuiNFS)
                            modeloDocumentoFiscal = modeloDocumentoFiscalNFSe;
                        else if (possuiCTe)
                            modeloDocumentoFiscal = modeloDocumentoFiscalCTe;
                        else if (possuiNFSManual && modeloDocumentoFiscalIntramunicipal != null)
                            modeloDocumentoFiscal = modeloDocumentoFiscalIntramunicipal;

                        if (modeloDocumentoFiscal != null)
                            AdicionarPreviaDocumento(carga, modeloDocumentoFiscal, null, new List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>() { pedidoCTeParaSubcontratacao }, unitOfWork);
                    }
                }
            }
        }

        public static Dictionary<Dominio.Entidades.ModeloDocumentoFiscal, int> ObterQuantidadeDocumentosEmitir(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotasFiscais, List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidoCTesParaSubcontratacao, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.CargaPreviaDocumento repCargaPreviaDocumento = new Repositorio.Embarcador.Cargas.CargaPreviaDocumento(unitOfWork);
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);

            Dictionary<Dominio.Entidades.ModeloDocumentoFiscal, int> quantidades = new Dictionary<Dominio.Entidades.ModeloDocumentoFiscal, int>();

            List<Dominio.ObjetosDeValor.Embarcador.Carga.TotalizadorModeloDocumento> totalizadores = new List<Dominio.ObjetosDeValor.Embarcador.Carga.TotalizadorModeloDocumento>();

            if (pedidoXMLNotasFiscais != null)
                totalizadores = repCargaPreviaDocumento.ObterTotalizadorPorNotasFiscais(pedidoXMLNotasFiscais.Select(o => o.Codigo).ToList());
            else if (pedidoCTesParaSubcontratacao != null)
                totalizadores = repCargaPreviaDocumento.ObterTotalizadorPorCTesTerceiro(pedidoCTesParaSubcontratacao.Select(o => o.Codigo).ToList());
            else if (cargaPedidos != null)
                totalizadores = repCargaPreviaDocumento.ObterTotalizadorPorCargaPedido(cargaPedidos.Select(o => o.Codigo).ToList());
            else if (carga != null)
                totalizadores = repCargaPreviaDocumento.ObterTotalizadorPorCarga(carga.Codigo);

            foreach (Dominio.ObjetosDeValor.Embarcador.Carga.TotalizadorModeloDocumento totalizador in totalizadores)
                quantidades.Add(repModeloDocumentoFiscal.BuscarPorCodigo(totalizador.Codigo, false), totalizador.Total);

            return quantidades;
        }

        public static List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> ObterCargaPedidosParaRateio(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            return repCargaPedido.BuscarPorCargaParaRateio(carga.Codigo);
        }

        public void BuscarConfiguracoesMultimodal(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, ref Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab, List<Dominio.Entidades.Cliente> clientesBloquearEmissaoDosDestinatarios, Dominio.Entidades.Cliente tomadorEmCache = null, bool atualizaCargaPedido = true)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            cargaPedido.BloquearEmissaoDosDestinatario = false;
            cargaPedido.BloquearEmissaoDeEntidadeSemCadastro = false;
            cargaPedido.ClientesBloquearEmissaoDosDestinatario = new List<Dominio.Entidades.Cliente>();

            if (carga.CargaRecebidaDeIntegracao)
                return;

            Dominio.Entidades.Cliente tomador = null;

            if (tomadorEmCache != null)
                tomador = tomadorEmCache;
            else
                tomador = cargaPedido.ObterTomador();


            if (tomador != null)
            {
                if (carga.TipoOperacao != null && carga.TipoOperacao.UsarConfiguracaoEmissao)
                {
                    if (carga.TipoOperacao.TipoCobrancaMultimodal != TipoCobrancaMultimodal.Nenhum)
                        cargaPedido.TipoCobrancaMultimodal = carga.TipoOperacao.TipoCobrancaMultimodal;
                    if (cargaPedido != null && cargaPedido.Pedido != null && cargaPedido.Pedido.TipoDeCarga != null && (cargaPedido.Pedido.TipoDeCarga.ModalProposta == ModalPropostaMultimodal.PortaPorta || cargaPedido.Pedido.TipoDeCarga.ModalProposta == ModalPropostaMultimodal.PortaPorto || cargaPedido.Pedido.TipoDeCarga.ModalProposta == ModalPropostaMultimodal.PortoPorta || cargaPedido.Pedido.TipoDeCarga.ModalProposta == ModalPropostaMultimodal.PortoPorto))
                        cargaPedido.ModalPropostaMultimodal = cargaPedido.Pedido.TipoDeCarga.ModalProposta;
                    else if (carga.TipoOperacao.ModalPropostaMultimodal != ModalPropostaMultimodal.Nenhum)
                        cargaPedido.ModalPropostaMultimodal = carga.TipoOperacao.ModalPropostaMultimodal;
                    if (carga.TipoOperacao.TipoServicoMultimodal != TipoServicoMultimodal.Nenhum)
                        cargaPedido.TipoServicoMultimodal = carga.TipoOperacao.TipoServicoMultimodal;
                    if (carga.TipoOperacao.TipoPropostaMultimodal != TipoPropostaMultimodal.Nenhum)
                        cargaPedido.TipoPropostaMultimodal = carga.TipoOperacao.TipoPropostaMultimodal;
                    cargaPedido.BloquearEmissaoDosDestinatario = carga.TipoOperacao.BloquearEmissaoDosDestinatario;
                    cargaPedido.BloquearEmissaoDeEntidadeSemCadastro = carga.TipoOperacao.BloquearEmissaoDeEntidadeSemCadastro;

                    if (clientesBloquearEmissaoDosDestinatarios != null && clientesBloquearEmissaoDosDestinatarios.Any())
                        foreach (Dominio.Entidades.Cliente cliente in clientesBloquearEmissaoDosDestinatarios)
                            cargaPedido.ClientesBloquearEmissaoDosDestinatario.Add(cliente);
                }
                else if (tomador.NaoUsarConfiguracaoEmissaoGrupo)
                {
                    cargaPedido.TipoCobrancaMultimodal = tomador.TipoCobrancaMultimodal;
                    if (cargaPedido != null && cargaPedido.Pedido != null && cargaPedido.Pedido.TipoDeCarga != null && (cargaPedido.Pedido.TipoDeCarga.ModalProposta == ModalPropostaMultimodal.PortaPorta || cargaPedido.Pedido.TipoDeCarga.ModalProposta == ModalPropostaMultimodal.PortaPorto || cargaPedido.Pedido.TipoDeCarga.ModalProposta == ModalPropostaMultimodal.PortoPorta || cargaPedido.Pedido.TipoDeCarga.ModalProposta == ModalPropostaMultimodal.PortoPorto))
                        cargaPedido.ModalPropostaMultimodal = cargaPedido.Pedido.TipoDeCarga.ModalProposta;
                    else
                        cargaPedido.ModalPropostaMultimodal = tomador.ModalPropostaMultimodal;
                    cargaPedido.TipoServicoMultimodal = tomador.TipoServicoMultimodal;
                    cargaPedido.TipoPropostaMultimodal = tomador.TipoPropostaMultimodal;
                    cargaPedido.BloquearEmissaoDosDestinatario = tomador.BloquearEmissaoDosDestinatario;
                    cargaPedido.BloquearEmissaoDeEntidadeSemCadastro = tomador.BloquearEmissaoDeEntidadeSemCadastro;

                    if (tomador.ClientesBloquearEmissaoDosDestinatario != null && tomador.ClientesBloquearEmissaoDosDestinatario.Count > 0)
                        foreach (Dominio.Entidades.Cliente cliente in tomador.ClientesBloquearEmissaoDosDestinatario)
                            cargaPedido.ClientesBloquearEmissaoDosDestinatario.Add(cliente);
                }
                else if (tomador.GrupoPessoas != null)
                {
                    if (cargaPedido.TipoCobrancaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaMultimodal.Nenhum)
                        cargaPedido.TipoCobrancaMultimodal = tomador.GrupoPessoas.TipoCobrancaMultimodal;
                    if (cargaPedido != null && cargaPedido.Pedido != null && cargaPedido.Pedido.TipoDeCarga != null && (cargaPedido.Pedido.TipoDeCarga.ModalProposta == ModalPropostaMultimodal.PortaPorta || cargaPedido.Pedido.TipoDeCarga.ModalProposta == ModalPropostaMultimodal.PortaPorto || cargaPedido.Pedido.TipoDeCarga.ModalProposta == ModalPropostaMultimodal.PortoPorta || cargaPedido.Pedido.TipoDeCarga.ModalProposta == ModalPropostaMultimodal.PortoPorto))
                        cargaPedido.ModalPropostaMultimodal = cargaPedido.Pedido.TipoDeCarga.ModalProposta;
                    else if (cargaPedido.ModalPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalPropostaMultimodal.Nenhum)
                        cargaPedido.ModalPropostaMultimodal = tomador.GrupoPessoas.ModalPropostaMultimodal;
                    if (cargaPedido.TipoServicoMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal.Nenhum)
                        cargaPedido.TipoServicoMultimodal = tomador.GrupoPessoas.TipoServicoMultimodal;
                    if (cargaPedido.TipoPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.Nenhum)
                        cargaPedido.TipoPropostaMultimodal = tomador.GrupoPessoas.TipoPropostaMultimodal;

                    cargaPedido.BloquearEmissaoDosDestinatario = tomador.GrupoPessoas.BloquearEmissaoDosDestinatario;
                    cargaPedido.BloquearEmissaoDeEntidadeSemCadastro = tomador.GrupoPessoas.BloquearEmissaoDeEntidadeSemCadastro;

                    if (tomador.GrupoPessoas.ClientesBloquearEmissaoDosDestinatario != null && tomador.GrupoPessoas.ClientesBloquearEmissaoDosDestinatario.Count > 0)
                        foreach (Dominio.Entidades.Cliente cliente in tomador.GrupoPessoas.ClientesBloquearEmissaoDosDestinatario)
                            cargaPedido.ClientesBloquearEmissaoDosDestinatario.Add(cliente);
                }
            }
            else
            {
                if (carga.TipoOperacao != null && carga.TipoOperacao.UsarConfiguracaoEmissao)
                {
                    if (carga.TipoOperacao.TipoCobrancaMultimodal != TipoCobrancaMultimodal.Nenhum)
                        cargaPedido.TipoCobrancaMultimodal = carga.TipoOperacao.TipoCobrancaMultimodal;
                    if (cargaPedido != null && cargaPedido.Pedido != null && cargaPedido.Pedido.TipoDeCarga != null && (cargaPedido.Pedido.TipoDeCarga.ModalProposta == ModalPropostaMultimodal.PortaPorta || cargaPedido.Pedido.TipoDeCarga.ModalProposta == ModalPropostaMultimodal.PortaPorto || cargaPedido.Pedido.TipoDeCarga.ModalProposta == ModalPropostaMultimodal.PortoPorta || cargaPedido.Pedido.TipoDeCarga.ModalProposta == ModalPropostaMultimodal.PortoPorto))
                        cargaPedido.ModalPropostaMultimodal = cargaPedido.Pedido.TipoDeCarga.ModalProposta;
                    else if (carga.TipoOperacao.ModalPropostaMultimodal != ModalPropostaMultimodal.Nenhum)
                        cargaPedido.ModalPropostaMultimodal = carga.TipoOperacao.ModalPropostaMultimodal;
                    if (carga.TipoOperacao.TipoServicoMultimodal != TipoServicoMultimodal.Nenhum)
                        cargaPedido.TipoServicoMultimodal = carga.TipoOperacao.TipoServicoMultimodal;
                    if (carga.TipoOperacao.TipoPropostaMultimodal != TipoPropostaMultimodal.Nenhum)
                        cargaPedido.TipoPropostaMultimodal = carga.TipoOperacao.TipoPropostaMultimodal;
                    cargaPedido.BloquearEmissaoDosDestinatario = carga.TipoOperacao.BloquearEmissaoDosDestinatario;
                    cargaPedido.BloquearEmissaoDeEntidadeSemCadastro = carga.TipoOperacao.BloquearEmissaoDeEntidadeSemCadastro;

                    if (clientesBloquearEmissaoDosDestinatarios != null && clientesBloquearEmissaoDosDestinatarios.Any())
                        foreach (Dominio.Entidades.Cliente cliente in clientesBloquearEmissaoDosDestinatarios)
                            cargaPedido.ClientesBloquearEmissaoDosDestinatario.Add(cliente);
                }
                else if (cargaPedido.Pedido.GrupoPessoas != null)
                {
                    if (cargaPedido.TipoCobrancaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaMultimodal.Nenhum)
                        cargaPedido.TipoCobrancaMultimodal = cargaPedido.Pedido.GrupoPessoas.TipoCobrancaMultimodal;
                    if (cargaPedido != null && cargaPedido.Pedido != null && cargaPedido.Pedido.TipoDeCarga != null && (cargaPedido.Pedido.TipoDeCarga.ModalProposta == ModalPropostaMultimodal.PortaPorta || cargaPedido.Pedido.TipoDeCarga.ModalProposta == ModalPropostaMultimodal.PortaPorto || cargaPedido.Pedido.TipoDeCarga.ModalProposta == ModalPropostaMultimodal.PortoPorta || cargaPedido.Pedido.TipoDeCarga.ModalProposta == ModalPropostaMultimodal.PortoPorto))
                        cargaPedido.ModalPropostaMultimodal = cargaPedido.Pedido.TipoDeCarga.ModalProposta;
                    else if (cargaPedido.ModalPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalPropostaMultimodal.Nenhum)
                        cargaPedido.ModalPropostaMultimodal = cargaPedido.Pedido.GrupoPessoas.ModalPropostaMultimodal;
                    if (cargaPedido.TipoServicoMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal.Nenhum)
                        cargaPedido.TipoServicoMultimodal = cargaPedido.Pedido.GrupoPessoas.TipoServicoMultimodal;
                    if (cargaPedido.TipoPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.Nenhum)
                        cargaPedido.TipoPropostaMultimodal = cargaPedido.Pedido.GrupoPessoas.TipoPropostaMultimodal;

                    cargaPedido.BloquearEmissaoDosDestinatario = cargaPedido.Pedido.GrupoPessoas.BloquearEmissaoDosDestinatario;
                    cargaPedido.BloquearEmissaoDeEntidadeSemCadastro = cargaPedido.Pedido.GrupoPessoas.BloquearEmissaoDeEntidadeSemCadastro;

                    if (cargaPedido.Pedido.GrupoPessoas.ClientesBloquearEmissaoDosDestinatario != null && cargaPedido.Pedido.GrupoPessoas.ClientesBloquearEmissaoDosDestinatario.Count > 0)
                        foreach (Dominio.Entidades.Cliente cliente in cargaPedido.Pedido.GrupoPessoas.ClientesBloquearEmissaoDosDestinatario)
                            cargaPedido.ClientesBloquearEmissaoDosDestinatario.Add(cliente);
                }
            }

            if ((integracaoIntercab?.BuscarTipoServicoModeloDocumentoVinculadoCarga ?? false))
            {
                if (cargaPedido.TipoContratacaoCarga == TipoContratacaoCarga.Normal)
                    cargaPedido.TipoServicoMultimodal = TipoServicoMultimodal.Normal;
                else if (cargaPedido.TipoContratacaoCarga == TipoContratacaoCarga.NormalESubContratada)
                    cargaPedido.TipoServicoMultimodal = TipoServicoMultimodal.Normal;
                else if (cargaPedido.TipoContratacaoCarga == TipoContratacaoCarga.Redespacho)
                    cargaPedido.TipoServicoMultimodal = TipoServicoMultimodal.Redespacho;
                else if (cargaPedido.TipoContratacaoCarga == TipoContratacaoCarga.RedespachoIntermediario)
                    cargaPedido.TipoServicoMultimodal = TipoServicoMultimodal.RedespachoIntermediario;
                else if (cargaPedido.TipoContratacaoCarga == TipoContratacaoCarga.SubContratada)
                    cargaPedido.TipoServicoMultimodal = TipoServicoMultimodal.Subcontratacao;
                else if (cargaPedido.TipoContratacaoCarga == TipoContratacaoCarga.SVMProprio)
                    cargaPedido.TipoServicoMultimodal = TipoServicoMultimodal.VinculadoMultimodalProprio;
                else if (cargaPedido.TipoContratacaoCarga == TipoContratacaoCarga.SVMTerceiro)
                    cargaPedido.TipoServicoMultimodal = TipoServicoMultimodal.VinculadoMultimodalTerceiro;
            }

            if (integracaoIntercab?.DefinirModalPeloTipoCarga == true)
            {
                if (cargaPedido.ModalPropostaMultimodal == ModalPropostaMultimodal.PortoPorto)
                    cargaPedido.TipoCobrancaMultimodal = TipoCobrancaMultimodal.CTEAquaviario;
                else
                    cargaPedido.TipoCobrancaMultimodal = TipoCobrancaMultimodal.CTeMultimodal;

                if (cargaPedido.TipoPropostaMultimodal == TipoPropostaMultimodal.Nenhum)
                    cargaPedido.TipoPropostaMultimodal = TipoPropostaMultimodal.CargaFechada;
            }

            if (atualizaCargaPedido)
                repCargaPedido.Atualizar(cargaPedido);
        }

        public static void AtualizarLocalidadesCargaPedido(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

            var cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);

            foreach (var cargaPedido in cargaPedidos)
            {
                if (cargaPedido.Expedidor != null)
                {
                    cargaPedido.Origem = cargaPedido.Expedidor.Localidade;
                    if (cargaPedido.Pedido.Expedidor != null)
                        cargaPedido.Pedido.Origem = cargaPedido.Expedidor.Localidade;
                }
                else if (cargaPedido.Pedido.Remetente != null && cargaPedido.Expedidor == null)
                {
                    cargaPedido.Origem = cargaPedido.Pedido.Remetente.Localidade;
                    cargaPedido.Origem = cargaPedido.Pedido.Remetente.Localidade;
                }

                if (cargaPedido.Recebedor != null)
                {
                    cargaPedido.Destino = cargaPedido.Recebedor.Localidade;
                    if (cargaPedido.Pedido.Recebedor != null)
                        cargaPedido.Pedido.Destino = cargaPedido.Pedido.Recebedor.Localidade;
                }
                else if (cargaPedido.Pedido.Destinatario != null)
                {
                    cargaPedido.Destino = cargaPedido.Pedido.Destinatario.Localidade;
                    cargaPedido.Pedido.Destino = cargaPedido.Pedido.Destinatario.Localidade;
                }

                repPedido.Atualizar(cargaPedido.Pedido);
                repCargaPedido.Atualizar(cargaPedido);
            }
        }

        public static void SumarizarDadosZonaTransporte(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.TipoDetalhe repositorioTipoDetalhe = new Repositorio.Embarcador.Pedidos.TipoDetalhe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaZonaTransporte repositorioCargaZonaTransporte = new Repositorio.Embarcador.Cargas.CargaZonaTransporte(unitOfWork);

            IList<Dominio.ObjetosDeValor.Embarcador.Carga.CargaZonaTransporte> cargaZonaTransporte = repositorioCargaZonaTransporte.BuscarCargaZonaTransportePorCarga(carga.Codigo);
            IList<Dominio.ObjetosDeValor.Embarcador.Carga.CargaZonaTransporte> cargaZonaTransporteAgrupado = cargaZonaTransporte
                .GroupBy(obj => new { obj.CodigoZonaTransporte })
                .Select(obj => new Dominio.ObjetosDeValor.Embarcador.Carga.CargaZonaTransporte()
                {
                    CodigoZonaTransporte = obj.Key.CodigoZonaTransporte,
                    PesoTotalPedido = obj.Sum(p => p.PesoTotalPedido),
                    CubagemTotalPedido = obj.Sum(pa => pa.CubagemTotalPedido),
                    ValorMercadoriaPedido = obj.Sum(pb => pb.ValorMercadoriaPedido)
                }).ToList();

            List<Dominio.Entidades.Embarcador.Cargas.CargaZonaTransporte> listaCargaZonaTransporteDeletar = repositorioCargaZonaTransporte.BuscarListaCargaZonaTransportePorCarga(carga.Codigo);
            if (listaCargaZonaTransporteDeletar.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaZonaTransporte cargaZonaTransporteDeletar in listaCargaZonaTransporteDeletar)
                    repositorioCargaZonaTransporte.Deletar(cargaZonaTransporteDeletar);
            }

            int count = 0;
            foreach (Dominio.ObjetosDeValor.Embarcador.Carga.CargaZonaTransporte cargaZonaTransporteInsercao in cargaZonaTransporteAgrupado)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaZonaTransporte cargaZonaTransporteEntidade = new Dominio.Entidades.Embarcador.Cargas.CargaZonaTransporte();
                cargaZonaTransporteEntidade.Carga = carga;
                cargaZonaTransporteEntidade.ZonaTransporte = repositorioTipoDetalhe.BuscarPorCodigo(cargaZonaTransporteInsercao.CodigoZonaTransporte, false);
                cargaZonaTransporteEntidade.Sequencia = ++count;//cargaZonaTransporteInsercao.Sequencia;
                cargaZonaTransporteEntidade.PesoTotalPedido = cargaZonaTransporteInsercao.PesoTotalPedido;
                cargaZonaTransporteEntidade.CubagemTotalPedido = cargaZonaTransporteInsercao.CubagemTotalPedido;
                cargaZonaTransporteEntidade.ValorMercadoriaPedido = cargaZonaTransporteInsercao.ValorMercadoriaPedido;

                repositorioCargaZonaTransporte.Inserir(cargaZonaTransporteEntidade);
            }

            AjustarIntegracoesCargaDadosTransporte(carga, unitOfWork);
        }

        public static void DefinirCanalVendaPorDestinatario(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoAdicionado, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Repositorio.UnitOfWork unitOfWork)
        {
            if (!new Repositorio.Embarcador.Logistica.PeriodoDescarregamentoCanalVenda(unitOfWork).ExisteRegistroCadastrado())
                return;

            double cpfCnpjDestinatario = (cargaPedidoAdicionado.Recebedor ?? cargaPedidoAdicionado.Pedido.Destinatario).CPF_CNPJ;
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosMesmoDestinatario = cargaPedidos.Where(cargaPedido => (cargaPedido.Codigo != cargaPedidoAdicionado.Codigo) && (cargaPedido.Recebedor ?? cargaPedido.Pedido.Destinatario).CPF_CNPJ == cpfCnpjDestinatario).ToList();

            if (cargaPedidosMesmoDestinatario.Count == 0)
                return;

            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.CanalVenda canalVenda = cargaPedidosMesmoDestinatario.Where(cargaPedido => cargaPedido.CanalVenda != null).Select(cargaPedido => cargaPedido.CanalVenda).FirstOrDefault();

            cargaPedidoAdicionado.CanalVenda = canalVenda;

            repositorioCargaPedido.Atualizar(cargaPedidoAdicionado);
        }

        public static void DefinirCanalVendaPorDestinatarios(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Repositorio.UnitOfWork unitOfWork)
        {
            if (!new Repositorio.Embarcador.Logistica.PeriodoDescarregamentoCanalVenda(unitOfWork).ExisteRegistroCadastrado())
                return;

            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            List<double> cpfCnpjDestinatarios = cargaPedidos.Select(cargaPedido => (cargaPedido.Recebedor ?? cargaPedido.Pedido.Destinatario).CPF_CNPJ).Distinct().ToList();

            foreach (double cpfCnpjDestinatario in cpfCnpjDestinatarios)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosPorDestinatario = cargaPedidos.Where(cargaPedido => (cargaPedido.Recebedor ?? cargaPedido.Pedido.Destinatario).CPF_CNPJ == cpfCnpjDestinatario).ToList();
                List<Dominio.Entidades.Embarcador.Pedidos.CanalVenda> canaisVenda = cargaPedidosPorDestinatario.Where(cargaPedido => cargaPedido.CanalVenda != null).Select(cargaPedido => cargaPedido.CanalVenda).Distinct().ToList();
                Dominio.Entidades.Embarcador.Pedidos.CanalVenda canalVendaMaiorPeso = null;

                if (canaisVenda.Count == 1)
                    canalVendaMaiorPeso = canaisVenda.FirstOrDefault();
                else if (canaisVenda.Count > 1)
                {
                    decimal maiorPeso = 0m;

                    foreach (Dominio.Entidades.Embarcador.Pedidos.CanalVenda canalVenda in canaisVenda)
                    {
                        decimal pesoPorCanalVenda = cargaPedidosPorDestinatario.Where(cargaPedido => cargaPedido.CanalVenda != null).Sum(cargaPedido => cargaPedido.Peso);

                        if (pesoPorCanalVenda > maiorPeso)
                        {
                            maiorPeso = pesoPorCanalVenda;
                            canalVendaMaiorPeso = canalVenda;
                        }
                    }

                    if (canalVendaMaiorPeso == null)
                        canalVendaMaiorPeso = canaisVenda.FirstOrDefault();
                }

                if (canalVendaMaiorPeso == null)
                    continue;

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidosPorDestinatario)
                {
                    cargaPedido.CanalVenda = canalVendaMaiorPeso;
                    repositorioCargaPedido.Atualizar(cargaPedido);
                }
            }
        }

        public static async Task DefinirCanalVendaPorDestinatariosAsync(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Repositorio.UnitOfWork unitOfWork)
        {
            if (!await new Repositorio.Embarcador.Logistica.PeriodoDescarregamentoCanalVenda(unitOfWork).ExisteRegistroCadastradoAsync())
                return;

            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            List<double> cpfCnpjDestinatarios = cargaPedidos.Select(cargaPedido => (cargaPedido.Recebedor ?? cargaPedido.Pedido.Destinatario).CPF_CNPJ).Distinct().ToList();

            foreach (double cpfCnpjDestinatario in cpfCnpjDestinatarios)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosPorDestinatario = cargaPedidos.Where(cargaPedido => (cargaPedido.Recebedor ?? cargaPedido.Pedido.Destinatario).CPF_CNPJ == cpfCnpjDestinatario).ToList();
                List<Dominio.Entidades.Embarcador.Pedidos.CanalVenda> canaisVenda = cargaPedidosPorDestinatario.Where(cargaPedido => cargaPedido.CanalVenda != null).Select(cargaPedido => cargaPedido.CanalVenda).Distinct().ToList();
                Dominio.Entidades.Embarcador.Pedidos.CanalVenda canalVendaMaiorPeso = null;

                if (canaisVenda.Count == 1)
                    canalVendaMaiorPeso = canaisVenda.FirstOrDefault();
                else if (canaisVenda.Count > 1)
                {
                    decimal maiorPeso = 0m;

                    foreach (Dominio.Entidades.Embarcador.Pedidos.CanalVenda canalVenda in canaisVenda)
                    {
                        decimal pesoPorCanalVenda = cargaPedidosPorDestinatario.Where(cargaPedido => cargaPedido.CanalVenda != null).Sum(cargaPedido => cargaPedido.Peso);

                        if (pesoPorCanalVenda > maiorPeso)
                        {
                            maiorPeso = pesoPorCanalVenda;
                            canalVendaMaiorPeso = canalVenda;
                        }
                    }

                    if (canalVendaMaiorPeso == null)
                        canalVendaMaiorPeso = canaisVenda.FirstOrDefault();
                }

                if (canalVendaMaiorPeso == null)
                    continue;

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidosPorDestinatario)
                {
                    cargaPedido.CanalVenda = canalVendaMaiorPeso;
                    await repositorioCargaPedido.AtualizarAsync(cargaPedido);
                }
            }
        }

        public bool VerificarSeCargasPossuemMesmaQuantidadeDePedidos(List<int> codigoCargas, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            int quantidadeDePedidosAnterior = repositorioCargaPedido.QuantidadeCargaPedido(codigoCargas[0]);

            for (int i = 1; i < codigoCargas.Count; i++)
            {
                int quantidadeDePedidosAtual = repositorioCargaPedido.QuantidadeCargaPedido(codigoCargas[i]);

                if (quantidadeDePedidosAnterior != quantidadeDePedidosAtual)
                    return false;

                quantidadeDePedidosAnterior = quantidadeDePedidosAtual;
            }

            return true;
        }

        public void AlterarDadosSumarizadosCargaPedido(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, int volumesAnterior, int volumes)
        {
            cargaPedido.QuantidadeVolumesNF = Math.Max(0, cargaPedido.QuantidadeVolumesNF - volumesAnterior + volumes);
        }

        public Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS ObterRetornoImpostoIBSCBS(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, bool filialEmissora = false)
        {
            if (filialEmissora)
            {
                return new Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS()
                {
                    CST = cargaPedido.CSTIBSCBSFilialEmissora,
                    ClassificacaoTributaria = cargaPedido.ClassificacaoTributariaIBSCBSFilialEmissora,

                    AliquotaIBSEstadual = cargaPedido.AliquotaIBSEstadualFilialEmissora,
                    PercentualReducaoIBSEstadual = cargaPedido.PercentualReducaoIBSEstadualFilialEmissora,

                    AliquotaIBSMunicipal = cargaPedido.AliquotaIBSMunicipalFilialEmissora,
                    PercentualReducaoIBSMunicipal = cargaPedido.PercentualReducaoIBSMunicipalFilialEmissora,

                    AliquotaCBS = cargaPedido.AliquotaCBSFilialEmissora,
                    PercentualReducaoCBS = cargaPedido.PercentualReducaoCBSFilialEmissora,
                };
            }

            return new Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS()
            {
                CodigoOutraAliquota = cargaPedido.OutrasAliquotas?.Codigo ?? 0,
                CST = cargaPedido.CSTIBSCBS,
                ClassificacaoTributaria = cargaPedido.ClassificacaoTributariaIBSCBS,

                CodigoIndicadorOperacao = cargaPedido.CodigoIndicadorOperacao,
                NBS = cargaPedido.NBS,

                AliquotaIBSEstadual = cargaPedido.AliquotaIBSEstadual,
                PercentualReducaoIBSEstadual = cargaPedido.PercentualReducaoIBSEstadual,

                AliquotaIBSMunicipal = cargaPedido.AliquotaIBSMunicipal,
                PercentualReducaoIBSMunicipal = cargaPedido.PercentualReducaoIBSMunicipal,

                AliquotaCBS = cargaPedido.AliquotaCBS,
                PercentualReducaoCBS = cargaPedido.PercentualReducaoCBS,
            };
        }

        public void PreencherValoresRetornoIBSCBS(Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS, decimal baseCalculo, decimal valorIBSEstadual, decimal valorIBSMunicipal, decimal valorCBS)
        {
            impostoIBSCBS.BaseCalculo += baseCalculo;
            impostoIBSCBS.ValorIBSEstadual += valorIBSEstadual;
            impostoIBSCBS.ValorIBSMunicipal += valorIBSMunicipal;
            impostoIBSCBS.ValorCBS += valorCBS;
        }

        public void PreencherCamposImpostoIBSCBS(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS, bool semValores = false)
        {
            cargaPedido.SetarRegraOutraAliquota(impostoIBSCBS.CodigoOutraAliquota);
            cargaPedido.CSTIBSCBS = impostoIBSCBS.CST;
            cargaPedido.NBS = impostoIBSCBS.NBS;

            cargaPedido.CodigoIndicadorOperacao = impostoIBSCBS.CodigoIndicadorOperacao;
            cargaPedido.ClassificacaoTributariaIBSCBS = impostoIBSCBS.ClassificacaoTributaria;
            cargaPedido.BaseCalculoIBSCBS = impostoIBSCBS.BaseCalculo;

            cargaPedido.AliquotaIBSEstadual = impostoIBSCBS.AliquotaIBSEstadual;
            cargaPedido.PercentualReducaoIBSEstadual = impostoIBSCBS.PercentualReducaoIBSEstadual;
            if (!semValores)
                cargaPedido.ValorIBSEstadual = Math.Round(impostoIBSCBS.ValorIBSEstadual, 3, MidpointRounding.AwayFromZero);

            cargaPedido.AliquotaIBSMunicipal = impostoIBSCBS.AliquotaIBSMunicipal;
            cargaPedido.PercentualReducaoIBSMunicipal = impostoIBSCBS.PercentualReducaoIBSMunicipal;
            if (!semValores)
                cargaPedido.ValorIBSMunicipal = Math.Round(impostoIBSCBS.ValorIBSMunicipal, 3, MidpointRounding.AwayFromZero);

            cargaPedido.AliquotaCBS = impostoIBSCBS.AliquotaCBS;
            cargaPedido.PercentualReducaoCBS = impostoIBSCBS.PercentualReducaoCBS;
            if (!semValores)
                cargaPedido.ValorCBS = Math.Round(impostoIBSCBS.ValorCBS, 3, MidpointRounding.AwayFromZero);
        }

        public void PreencherCamposImpostoIBSCBSFilialEmissora(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS, bool semValores = false)
        {
            cargaPedido.CSTIBSCBSFilialEmissora = impostoIBSCBS.CST;
            cargaPedido.ClassificacaoTributariaIBSCBSFilialEmissora = impostoIBSCBS.ClassificacaoTributaria;
            cargaPedido.BaseCalculoIBSCBSFilialEmissora = impostoIBSCBS.BaseCalculo;

            cargaPedido.AliquotaIBSEstadualFilialEmissora = impostoIBSCBS.AliquotaIBSEstadual;
            cargaPedido.PercentualReducaoIBSEstadualFilialEmissora = impostoIBSCBS.PercentualReducaoIBSEstadual;
            if (!semValores)
                cargaPedido.ValorIBSEstadualFilialEmissora = Math.Round(impostoIBSCBS.ValorIBSEstadual, 3, MidpointRounding.AwayFromZero);

            cargaPedido.AliquotaIBSMunicipalFilialEmissora = impostoIBSCBS.AliquotaIBSMunicipal;
            cargaPedido.PercentualReducaoIBSMunicipalFilialEmissora = impostoIBSCBS.PercentualReducaoIBSMunicipal;
            if (!semValores)
                cargaPedido.ValorIBSMunicipalFilialEmissora = Math.Round(impostoIBSCBS.ValorIBSMunicipal, 3, MidpointRounding.AwayFromZero);

            cargaPedido.AliquotaCBSFilialEmissora = impostoIBSCBS.AliquotaCBS;
            cargaPedido.PercentualReducaoCBSFilialEmissora = impostoIBSCBS.PercentualReducaoCBS;
            if (!semValores)
                cargaPedido.ValorCBSFilialEmissora = Math.Round(impostoIBSCBS.ValorCBS, 3, MidpointRounding.AwayFromZero);
        }

        public void PreencherValoresImpostoIBSCBS(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, decimal baseCalculo, decimal valorIBSEstadual, decimal valorIBSMunicipal, decimal valorCBS, bool arredondar = false)
        {
            cargaPedido.BaseCalculoIBSCBS = arredondar ? Math.Round(baseCalculo, 3, MidpointRounding.AwayFromZero) : baseCalculo;
            cargaPedido.ValorIBSEstadual = arredondar ? Math.Round(valorIBSEstadual, 3, MidpointRounding.AwayFromZero) : valorIBSEstadual;
            cargaPedido.ValorIBSMunicipal = arredondar ? Math.Round(valorIBSMunicipal, 3, MidpointRounding.AwayFromZero) : valorIBSMunicipal;
            cargaPedido.ValorCBS = arredondar ? Math.Round(valorCBS, 3, MidpointRounding.AwayFromZero) : valorCBS;
        }

        public void PreencherValoresImpostoIBSCBSFilialEmissora(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, decimal baseCalculo, decimal valorIBSEstadual, decimal valorIBSMunicipal, decimal valorCBS, bool arredondar = false)
        {
            cargaPedido.BaseCalculoIBSCBSFilialEmissora = arredondar ? Math.Round(baseCalculo, 3, MidpointRounding.AwayFromZero) : baseCalculo;
            cargaPedido.ValorIBSEstadualFilialEmissora = arredondar ? Math.Round(valorIBSEstadual, 3, MidpointRounding.AwayFromZero) : valorIBSEstadual;
            cargaPedido.ValorIBSMunicipalFilialEmissora = arredondar ? Math.Round(valorIBSMunicipal, 3, MidpointRounding.AwayFromZero) : valorIBSMunicipal;
            cargaPedido.ValorCBSFilialEmissora = arredondar ? Math.Round(valorCBS, 3, MidpointRounding.AwayFromZero) : valorCBS;
        }

        public void ZerarCamposImpostoIBSCBS(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, bool apenasValores = false)
        {
            if (apenasValores)
            {
                cargaPedido.BaseCalculoIBSCBS = 0m;
                cargaPedido.ValorIBSEstadual = 0m;
                cargaPedido.ValorIBSMunicipal = 0m;
                cargaPedido.ValorCBS = 0m;

                return;
            }

            cargaPedido.SetarRegraOutraAliquota(0);
            cargaPedido.CST = string.Empty;
            cargaPedido.CodigoIndicadorOperacao = string.Empty;
            cargaPedido.NBS = string.Empty;
            cargaPedido.ClassificacaoTributariaIBSCBS = string.Empty;
            cargaPedido.BaseCalculoIBSCBS = 0m;

            cargaPedido.AliquotaIBSEstadual = 0m;
            cargaPedido.PercentualReducaoIBSEstadual = 0m;
            cargaPedido.ValorIBSEstadual = 0m;

            cargaPedido.AliquotaIBSMunicipal = 0m;
            cargaPedido.PercentualReducaoIBSMunicipal = 0m;
            cargaPedido.ValorIBSMunicipal = 0m;

            cargaPedido.AliquotaCBS = 0m;
            cargaPedido.PercentualReducaoCBS = 0m;
            cargaPedido.ValorCBS = 0m;
        }

        public void ZerarCamposImpostoIBSCBSFilialEmissora(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, bool apenasValores = false)
        {
            if (apenasValores)
            {
                cargaPedido.BaseCalculoIBSCBSFilialEmissora = 0m;
                cargaPedido.ValorIBSEstadualFilialEmissora = 0m;
                cargaPedido.ValorIBSMunicipalFilialEmissora = 0m;
                cargaPedido.ValorCBSFilialEmissora = 0m;

                return;
            }

            cargaPedido.CSTFilialEmissora = string.Empty;
            cargaPedido.ClassificacaoTributariaIBSCBSFilialEmissora = string.Empty;
            cargaPedido.BaseCalculoIBSCBSFilialEmissora = 0m;

            cargaPedido.AliquotaIBSEstadualFilialEmissora = 0m;
            cargaPedido.PercentualReducaoIBSEstadualFilialEmissora = 0m;
            cargaPedido.ValorIBSEstadualFilialEmissora = 0m;

            cargaPedido.AliquotaIBSMunicipalFilialEmissora = 0m;
            cargaPedido.PercentualReducaoIBSMunicipalFilialEmissora = 0m;
            cargaPedido.ValorIBSMunicipalFilialEmissora = 0m;

            cargaPedido.AliquotaCBSFilialEmissora = 0m;
            cargaPedido.PercentualReducaoCBSFilialEmissora = 0m;
            cargaPedido.ValorCBSFilialEmissora = 0m;
        }

        #endregion

        #region Métodos Privados

        private static void AdicionarPreviaDocumento(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotasFiscais, List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidoCTesParaSubcontratacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPreviaDocumento repCargaPreviaDocumento = new Repositorio.Embarcador.Cargas.CargaPreviaDocumento(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPreviaDocumentoDocumento repCargaPreviaDocumentoNotaFiscal = new Repositorio.Embarcador.Cargas.CargaPreviaDocumentoDocumento(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaPreviaDocumento cargaPreviaDocumento = new Dominio.Entidades.Embarcador.Cargas.CargaPreviaDocumento()
            {
                ModeloDocumentoFiscal = modeloDocumentoFiscal,
                Carga = carga
            };

            repCargaPreviaDocumento.Inserir(cargaPreviaDocumento);

            if (pedidoXMLNotasFiscais != null)
            {
                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in pedidoXMLNotasFiscais)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPreviaDocumentoDocumento cargaPreviaDocumentoNotaFiscal = new Dominio.Entidades.Embarcador.Cargas.CargaPreviaDocumentoDocumento()
                    {
                        CargaPreviaDocumento = cargaPreviaDocumento,
                        PedidoXMLNotaFiscal = pedidoXMLNotaFiscal
                    };

                    repCargaPreviaDocumentoNotaFiscal.Inserir(cargaPreviaDocumentoNotaFiscal);
                }
            }

            if (pedidoCTesParaSubcontratacao != null)
            {
                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao in pedidoCTesParaSubcontratacao)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPreviaDocumentoDocumento cargaPreviaDocumentoCTeTerceiro = new Dominio.Entidades.Embarcador.Cargas.CargaPreviaDocumentoDocumento()
                    {
                        CargaPreviaDocumento = cargaPreviaDocumento,
                        PedidoCTeParaSubContratacao = pedidoCTeParaSubContratacao
                    };

                    repCargaPreviaDocumentoNotaFiscal.Inserir(cargaPreviaDocumentoCTeTerceiro);
                }
            }
        }

        private static void PreecherOutroEnderecoPedido(ref Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEndereco)
        {
            pedidoEndereco.Bairro = pedidoEndereco.ClienteOutroEndereco.Bairro;
            pedidoEndereco.CEP = pedidoEndereco.ClienteOutroEndereco.CEP;
            pedidoEndereco.Localidade = pedidoEndereco.ClienteOutroEndereco.Localidade;

            pedidoEndereco.Complemento = pedidoEndereco.ClienteOutroEndereco.Complemento;
            pedidoEndereco.Endereco = pedidoEndereco.ClienteOutroEndereco.Endereco;
            pedidoEndereco.Numero = pedidoEndereco.ClienteOutroEndereco.Numero;
            pedidoEndereco.Telefone = pedidoEndereco.ClienteOutroEndereco.Telefone;
            pedidoEndereco.IE_RG = pedidoEndereco.ClienteOutroEndereco.IE_RG;
        }

        private static void ValidarPermissaoAdicionarOuRemoverPedido(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork, bool adicionar, bool? reentrega = null, bool integracaoRouteasy = false)
        {
            string descricaoAcao = adicionar ? "adicionar" : "remover";
            bool situacaoCargaPermiteAdicionarOuRemoverPedido;

            if (adicionar && (pedido.TipoOperacao?.GerarCargaFinalizada ?? false))
                return;

            if (Carga.IsCargaBloqueada(carga, unitOfWork))
                throw new ServicoException($"A {carga.DescricaoEntidade} está bloqueada e não permite alteração.");

            if (carga.CargaDePreCargaEmFechamento)
                throw new ServicoException("Aguarde a carga desta pré carga ser fechada.");

            if (carga.ExigeNotaFiscalParaCalcularFrete)
                situacaoCargaPermiteAdicionarOuRemoverPedido = (carga.SituacaoCarga == SituacaoCarga.Nova || carga.SituacaoCarga == SituacaoCarga.AgNFe || (carga.SituacaoCarga == SituacaoCarga.CalculoFrete && !carga.CalculandoFrete));
            else
            {
                if (carga.CalculandoFrete && !integracaoRouteasy)
                    throw new ServicoException($"Aguarde o termino do calculo do frete para {descricaoAcao} o pedido.");

                situacaoCargaPermiteAdicionarOuRemoverPedido = (carga.SituacaoCarga == SituacaoCarga.Nova || carga.SituacaoCarga == SituacaoCarga.CalculoFrete || carga.SituacaoCarga == SituacaoCarga.AgTransportador || carga.SituacaoCarga == SituacaoCarga.AgNFe);
            }

            if (!situacaoCargaPermiteAdicionarOuRemoverPedido)
            {
                bool pedidoReentrega = reentrega ?? false;
                bool permitirRemoverPedidoCargaComPendenciaDocumentos = !adicionar && configuracaoEmbarcador.PermitirRemoverPedidoCargaComPendenciaDocumentos && carga.SituacaoCarga == SituacaoCarga.PendeciaDocumentos && carga.AgImportacaoCTe;
                bool pedidoAbertoEmCargaCancelada = carga.SituacaoCarga == SituacaoCarga.Cancelada && pedido.SituacaoPedido == SituacaoPedido.Aberto;
                bool situacaoPermiteExclusaoPedido = (pedidoReentrega || permitirRemoverPedidoCargaComPendenciaDocumentos || pedidoAbertoEmCargaCancelada || !string.IsNullOrEmpty(pedido.NumeroControle) || carga.CargaEmitidaParcialmente);

                if (!situacaoPermiteExclusaoPedido)
                    throw new ServicoException($"não é possível {descricaoAcao} o pedido na atual situação da carga.");
            }

            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

            if (servicoCarga.RecebeuNumeroCargaEmbarcador(carga, unitOfWork))
                throw new ServicoException($"não é possível {descricaoAcao} o pedido pois a carga já recebeu o número de carga do Embarcador.");
        }

        private void CriarObjetoInsertPedidoProduto(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> produtosPedido, ref Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaSqlPedidoProduto ObjetoMontagemCargaSqlPedidoProduto)
        {
            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProduto in produtosPedido)
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaPedidoProdutoObjeto objetoMontagem = CriarObjetoInsertPedidoProduto(pedidoProduto);
                objetoMontagem.codigoCargaPedido = cargaPedido.Codigo;
                objetoMontagem.codigoPedido = cargaPedido.Pedido.Codigo;

                ObjetoMontagemCargaSqlPedidoProduto.NumeroRegistros++;
                ObjetoMontagemCargaSqlPedidoProduto.ListaObjetosPedidoProduto.Add(objetoMontagem);
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaPedidoProdutoObjeto CriarObjetoInsertPedidoProduto(Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProduto)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaPedidoProdutoObjeto
            {
                codigoProduto = pedidoProduto.Produto.Codigo,
                PesoUnitario = pedidoProduto.PesoUnitario,
                Quantidade = pedidoProduto.Quantidade,
                QuantidadeCaixa = pedidoProduto.QuantidadeCaixa,
                QuantidadeCaixasVazias = pedidoProduto.QuantidadeCaixasVazias,
                QuantidadePlanejada = pedidoProduto.QuantidadePlanejada,
                PesoTotalEmbalagem = pedidoProduto.PesoTotalEmbalagem,
                ValorProduto = pedidoProduto.ValorProduto
            };
        }

        private void CriarObjetoInsertPedidoProduto(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto> carregamentoPedidoProdutos, ref Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaSqlPedidoProduto ObjetoMontagemCargaSqlPedidoProduto)
        {
            foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoProduto carregamentoPedidoProduto in carregamentoPedidoProdutos)
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCargaPedidoProdutoObjeto objetoMontagem = CriarObjetoInsertPedidoProduto(carregamentoPedidoProduto.PedidoProduto);
                objetoMontagem.Quantidade = carregamentoPedidoProduto.Quantidade;
                objetoMontagem.codigoCargaPedido = cargaPedido.Codigo;
                objetoMontagem.codigoPedido = cargaPedido.Pedido.Codigo;

                ObjetoMontagemCargaSqlPedidoProduto.NumeroRegistros++;
                ObjetoMontagemCargaSqlPedidoProduto.ListaObjetosPedidoProduto.Add(objetoMontagem);
            }
        }

        private static void AjustarIntegracoesCargaDadosTransporte(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repositorioCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao = repositorioCargaDadosTransporteIntegracao.BuscarPorCargaETipoIntegracao(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Boticario);

            if (cargaDadosTransporteIntegracao != null)
            {
                cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                repositorioCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracao);
            }
        }

        private static void AtualizarIntegracaoCargaDadosTransporte(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork);

            List<TipoIntegracao> integracoesPermitidasParaAtualizar = new List<TipoIntegracao>() { TipoIntegracao.Cebrace, TipoIntegracao.Vector, TipoIntegracao.SAP_API4 };

            List<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao> cargasDadosTransporteIntegracao = repCargaIntegracao.BuscarPorCargaETipoIntegracao(carga.Codigo, integracoesPermitidasParaAtualizar);

            if (cargasDadosTransporteIntegracao?.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracaoItem in cargasDadosTransporteIntegracao)
                {
                    cargaDadosTransporteIntegracaoItem.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;

                    repCargaIntegracao.Atualizar(cargaDadosTransporteIntegracaoItem);
                }
            }
        }

        private void InformarFilialEmissoraCargaPedido(ref Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            if (cargaPedido.Expedidor != null)
            {
                if ((cargaPedido.CargaOrigem.GrupoPessoaPrincipal != null && cargaPedido.CargaOrigem.GrupoPessoaPrincipal.EmitirSempreComoRedespacho) || cargaPedido.Recebedor == null)
                    cargaPedido.TipoContratacaoCargaSubContratacaoFilialEmissora = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho;
                else
                    cargaPedido.TipoContratacaoCargaSubContratacaoFilialEmissora = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.RedespachoIntermediario;
            }
            else
            {
                if (cargaPedido.Recebedor != null && (cargaPedido.CargaPedidoProximoTrecho != null || !cargaPedido.CargaPedidoFilialEmissora || cargaPedido.ProximoTrechoComplementaFilialEmissora))
                    cargaPedido.TipoContratacaoCargaSubContratacaoFilialEmissora = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Redespacho;
                else
                    cargaPedido.TipoContratacaoCargaSubContratacaoFilialEmissora = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada;
            }

            if (!cargaPedido.PedidoEncaixado && !cargaPedido.PossuiNFSManual && !cargaPedido.ObterTomador().NaoEmitirCTeFilialEmissora)
            {
                cargaPedido.CargaPedidoFilialEmissora = true;
                if (cargaPedido.EmitirComplementarFilialEmissora)
                    cargaPedido.TipoContratacaoCarga = TipoContratacaoCarga.Redespacho;
            }
            else
            {
                cargaPedido.CargaPedidoFilialEmissora = false;
                if (cargaPedido.EmitirComplementarFilialEmissora && !cargaPedido.PossuiNFSManual)
                    cargaPedido.TipoContratacaoCarga = TipoContratacaoCarga.Normal;
            }
        }

        #endregion
    }
}