using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Enumerador;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Carga;

public class FecharCarga : ServicoBase
{
    public FecharCarga(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken)
    {
    }

    public async Task FecharCargaAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente ClienteMultisoftware, bool recriarRotas = false, bool adicionarJanelaDescarregamento = true, bool adicionarJanelaCarregamento = true, bool validarDados = false, bool gerarAgendamentoColeta = true, Dominio.ObjetosDeValor.Embarcador.Carga.PropriedadesGeracaoCarga propriedades = null, bool notificarAcompanhamento = true, bool viaWSAtualizarCarga = false, string ip = "", bool viaWSRest = false, CancellationToken cancellationToken = default)
    {
        Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
        {
            OrigemAuditado = viaWSAtualizarCarga ? Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServiceCargas : Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema,
            TipoAuditado = viaWSAtualizarCarga ? Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Integradoras : Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema,
            IP = ip
        };
        
        Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = await new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarConfiguracaoPadraoAsync();
        Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = await new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(_unitOfWork).BuscarPrimeiroRegistroAsync();
        Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento = await new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento(_unitOfWork).BuscarPrimeiroRegistroAsync();
        Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = await new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(_unitOfWork).BuscarConfiguracaoPadraoAsync();
        Servicos.Embarcador.CTe.CTEsImportados serCTEsImportados = new Servicos.Embarcador.CTe.CTEsImportados(_unitOfWork, tipoServicoMultisoftware, cancellationToken);
        Servicos.Embarcador.Transportadores.Empresa servicoEmpresa = new Servicos.Embarcador.Transportadores.Empresa(_unitOfWork, cancellationToken);
        Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(_unitOfWork, cancellationToken);
        Servicos.Embarcador.Carga.CargaPallets serCargaPallets = new CargaPallets(_unitOfWork, configuracao);
        Servicos.Embarcador.Carga.CargaLocaisPrestacao servicoCargaLocaisPrestacao = new Servicos.Embarcador.Carga.CargaLocaisPrestacao(_unitOfWork, cancellationToken);
        Servicos.Embarcador.Veiculo.LicencaVeiculo servicoLicencaVeiculo = new Veiculo.LicencaVeiculo(_unitOfWork, tipoServicoMultisoftware);
        Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga servAlertaAcompanhamentoCarga = new Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga(_unitOfWork);
        Servicos.Embarcador.Pedido.Pedido servicoPedido = new Servicos.Embarcador.Pedido.Pedido(_unitOfWork, cancellationToken);
        Servicos.Embarcador.PreCarga.PreCarga servicoPreCarga = new Servicos.Embarcador.PreCarga.PreCarga(_unitOfWork);
        Servicos.Embarcador.Carga.MensagemAlertaCarga servicoMensagemAlerta = new Servicos.Embarcador.Carga.MensagemAlertaCarga(_unitOfWork);
        Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork, cancellationToken);
        Servicos.Embarcador.Hubs.Carga servicoHubCarga = new Servicos.Embarcador.Hubs.Carga();
        Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork, cancellationToken);
        Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork, cancellationToken);
        Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork, cancellationToken);
        Repositorio.Embarcador.Cargas.CargaMotorista repCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(_unitOfWork, cancellationToken);
        Repositorio.Embarcador.Cargas.CargaPedidoDistribuicao repCargaPedidoDistribuicao = new Repositorio.Embarcador.Cargas.CargaPedidoDistribuicao(_unitOfWork, cancellationToken);
        Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork, cancellationToken);
        Repositorio.Embarcador.TorreControle.ConfiguracaoWidgetAcompanhamentoCargaUsuario repConfigWidget = new Repositorio.Embarcador.TorreControle.ConfiguracaoWidgetAcompanhamentoCargaUsuario(_unitOfWork, cancellationToken);
        Repositorio.Embarcador.Configuracoes.ConfiguracaoEmissaoDocumentoEmbarcador repConfiguracaoEmissaoDocumentoEmbarcador = new Repositorio.Embarcador.Configuracoes.ConfiguracaoEmissaoDocumentoEmbarcador(_unitOfWork, cancellationToken);
        Repositorio.Embarcador.Pedidos.PedidoLocaisPrestacaoPassagens repositorioPassagemPercursoEstado = new Repositorio.Embarcador.Pedidos.PedidoLocaisPrestacaoPassagens(_unitOfWork, cancellationToken);

        bool isUnilever = await repTipoIntegracao.ExistePorTipoAsync(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Unilever);

        bool validarRegraTipoTomador = !isUnilever;
        bool cargaDePreCargaEmFechamento = carga.CargaDePreCargaEmFechamento;
        notificarAcompanhamento = notificarAcompanhamento && !repConfigWidget.DesativarAtualizacaoNovasCargas();

        if (configuracaoGeralCarga?.TrocarFilialQuandoExpedidorForUmaFilial ?? false)
        {
            Dominio.Entidades.Embarcador.Filiais.Filial filialPorExpedidores = await repCargaPedido.BuscarPrimeiraFilialPorExpedidoresDaCargaAsync(carga.Codigo);

            if (filialPorExpedidores != null)
                carga.Filial = filialPorExpedidores;
        }

        if (configuracao.GerarCargaTrajeto)
            await VerificarCargaTrajetoAsync(carga);

        if (!configuracao.IncluirCargaCanceladaProcessarDT)
            await servicoLicencaVeiculo.GerarCargaLicencaAsync(carga);

        if (cargaDePreCargaEmFechamento)
        {
            carga.CargaDePreCarga = false;
            carga.CargaDePreCargaEmFechamento = false;
            carga.CargaDePreCargaFechada = true;

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosPreCarga = await repCargaPedido.BuscarPedidosPreCargaPorCargaAsync(carga.Codigo);
            await Servicos.Embarcador.Carga.CargaPedido.RemoverPedidosCargaAsync(carga, cargaPedidosPreCarga, configuracao, tipoServicoMultisoftware, _unitOfWork, configuracaoGeralCarga);
        }
        else
            carga.BloquearAlteracaoJanelaDescarregamento = carga.TipoOperacao?.BloquearAdicaoNaJanelaDescarregamentoAutomaticamente ?? false;

        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = await repCargaPedido.BuscarPorCargaAsync(carga.Codigo);

        if (!configuracao.IncluirCargaCanceladaProcessarDT)
        {
            if (cargaDePreCargaEmFechamento || (configuracaoGeralCarga?.ForcarRoteirizacaoFecharCarga ?? false))
            {
                await new Servicos.Embarcador.Carga.RotaFrete(_unitOfWork).SetarRotaFreteCargaAsync(carga, cargaPedidos, configuracao, tipoServicoMultisoftware);
                await servicoCargaLocaisPrestacao.VerificarEAjustarLocaisPrestacaoAsync(carga, cargaPedidos, _unitOfWork, tipoServicoMultisoftware, configuracaoPedido);
                await Servicos.Embarcador.Carga.CargaRotaFrete.GerarIntegracoesRoteirizacaoCargaAsync(carga, _unitOfWork, configuracao, tipoServicoMultisoftware);
            }
            else if (repositorioPassagemPercursoEstado.ExistePorPedidos(cargaPedidos.Take(1500).Select(x => x.Pedido.Codigo).ToList()))
            {
                await servicoCargaLocaisPrestacao.VerificarEAjustarLocaisPrestacaoAsync(carga, cargaPedidos, _unitOfWork, tipoServicoMultisoftware, configuracaoPedido);
            }
        }

        await Servicos.Embarcador.Carga.CargaPedido.DefinirCanalVendaPorDestinatariosAsync(cargaPedidos, _unitOfWork);

        if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
        {
            Dominio.Entidades.Empresa empresaAnterior = null;
            if (carga.Empresa != null)
                empresaAnterior = carga.Empresa;

            carga.Empresa = servicoEmpresa.BuscarEmpresaPadraoCarga(cargaPedidos);

            if (carga.Empresa == null && empresaAnterior != null)
                carga.Empresa = empresaAnterior;

            if (carga.Veiculo != null)
            {
                if (carga.Veiculo.Tipo == "T")
                    carga.FreteDeTerceiro = true;
                else
                    carga.FreteDeTerceiro = false;
            }
            else if (carga.ProvedorOS != null)
                carga.FreteDeTerceiro = true;

            List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoEmissaoDocumentoEmbarcador> cacheConfiguracaoEmissaoDocumentoEmbarcador = repConfiguracaoEmissaoDocumentoEmbarcador.BuscaConfiguracaoEmissaoDocumentoEmbarcadorPorClientesDaCarga(cargaPedidos);
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                if (!cargaPedido.Pedido.AdicionadaManualmente && validarRegraTipoTomador && cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.Normal && cargaPedido.Pedido.Remetente?.GrupoPessoas == null && cargaPedido.Pedido.GrupoPessoas == null && cargaPedido.Pedido?.Destinatario?.GrupoPessoas != null)
                {
                    cargaPedido.TipoTomador = Dominio.Enumeradores.TipoTomador.Destinatario;
                    cargaPedido.Pedido.TipoPagamento = Dominio.Enumeradores.TipoPagamento.A_Pagar;

                    await repPedido.AtualizarAsync(cargaPedido.Pedido);
                }

                bool cteEmitidoNoEmbarcador = serCarga.VerificarSeCTeEmitidoNoEmbarcador(cargaPedido, _unitOfWork, cacheConfiguracaoEmissaoDocumentoEmbarcador);

                if (validarDados && cteEmitidoNoEmbarcador != cargaPedido.CTeEmitidoNoEmbarcador && serCarga.VerificarSePossuiDocumentoVinculado(carga, _unitOfWork))
                    throw new ServicoException("Não é possível alterar o tipo de operação desta carga pois existem documentos/notas fiscais vinculados à mesma.");

                cargaPedido.CTeEmitidoNoEmbarcador = cteEmitidoNoEmbarcador;

                await repCargaPedido.AtualizarAsync(cargaPedido);

                if (cargaPedido.CTeEmitidoNoEmbarcador && !configuracaoGeralCarga.NaoVincularAutomaticamenteDocumentosEmitidosEmbarcador)
                {
                    Servicos.Embarcador.MDFe.MDFeImportado.VerificarSeCargaPossuiAlgumMDFeCompativel(out string erro, cargaPedido.Codigo, _unitOfWork, auditado, configuracao);
                    serCTEsImportados.VerificarSeCargaPossuiAlgumCTe(cargaPedido, _unitOfWork, auditado);
                }
                else
                {
                    serCTEsImportados.VerificarSeCargaPossuiAlgumCTeTerceiro(cargaPedido, _unitOfWork, auditado, tipoServicoMultisoftware);
                }
            }

            await servicoPedido.AtualizarLocalParqueamentoPedidoAsync(carga, cargaPedidos, tipoServicoMultisoftware, _unitOfWork);
        }
        else
        {
            if (configuracao.AgruparNotasPedidosValoresZeradosFechamentoCarga)
                await AgruparNotasPedidosValoresZeradosFechamentoCargaAsync(cargaPedidos);

            if (carga.Veiculo != null && carga.Veiculo.Tipo == "T")
                carga.FreteDeTerceiro = true;
            else if (carga.ProvedorOS != null)
                carga.FreteDeTerceiro = true;

            await ObterTipoCargaModeloAutoAsync(carga, cargaPedidos);
            await ObterTipoOperacaoPadraoAsync(carga, cargaPedidos);
            await ObterDadosVeiculosEMotoristasAsync(carga);

            await Servicos.Embarcador.Carga.CargaIntegracaoEDI.VerificarCargaIntegracaoEDINotifisAsync(carga, tipoServicoMultisoftware, _unitOfWork);

            if (!configuracao.IncluirCargaCanceladaProcessarDT)
            {
                List<string> log = new List<string>();
                await serCarga.ObterTipoTrechoAsync(carga, cargaPedidos, _unitOfWork, false, log);
            }
        }

        await AlterarNumeroCargaPorPadraoGeracaoConfiguradoAsync(carga);

        Repositorio.Embarcador.Filiais.EstadoDestinoEmpresaEmissora repositorioEstadoDestinoEmpresaEmissora = new Repositorio.Embarcador.Filiais.EstadoDestinoEmpresaEmissora(_unitOfWork);
        List<Dominio.Entidades.Embarcador.Filiais.EstadoDestinoEmpresaEmissora> estadosDestinoEmpresaEmissora = carga.Filial != null ? await repositorioEstadoDestinoEmpresaEmissora.BuscarPorFilialAsync(carga.Filial.Codigo) : new List<Dominio.Entidades.Embarcador.Filiais.EstadoDestinoEmpresaEmissora>();

        await servicoPreCarga.TrocarPreCargaCompletoAsync(carga, tipoServicoMultisoftware, configuracao, ClienteMultisoftware);

        if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && carga.Filial != null && carga.TipoOperacao != null && carga.Filial.EmpresaEmissora != null && carga.TipoOperacao.EmiteCTeFilialEmissora && carga.Filial.EmpresaEmissora.CNPJ != (carga.Empresa?.CNPJ ?? string.Empty))
        {
            if (await repCargaPedido.VerificarSeOperacaoTeraEmissaoFilialEmissoraPorCargaAsync(carga.Codigo))
            {
                Dominio.Entidades.Embarcador.Filiais.EstadoDestinoEmpresaEmissora estadoDestino = estadosDestinoEmpresaEmissora.Find(e => e.Estado.Codigo == cargaPedidos.FirstOrDefault()?.Destino?.Estado.Codigo);

                if (carga.EmpresaFilialEmissora == null)
                {

                    if ((estadosDestinoEmpresaEmissora.Count > 0) && estadoDestino != null)
                        carga.EmpresaFilialEmissora = estadoDestino.Empresa;
                    else
                        carga.EmpresaFilialEmissora = carga.Filial.EmpresaEmissora;
                }
                if ((estadosDestinoEmpresaEmissora.Count > 0) && estadoDestino != null)
                    carga.EmiteMDFeFilialEmissora = carga.Filial.EmiteMDFeFilialEmissoraPorEstadoDestino;
                else
                    carga.EmiteMDFeFilialEmissora = carga.Filial.EmiteMDFeFilialEmissora;

                if (carga.Filial.UtilizarCtesAnterioresComoCteFilialEmissora)
                    carga.UtilizarCTesAnterioresComoCTeFilialEmissora = true;
            }
            else
            {
                carga.EmpresaFilialEmissora = null;
                carga.EmiteMDFeFilialEmissora = false;
            }
        }

        await serCarga.SetarTipoContratacaoCargaAsync(carga, _unitOfWork);

        carga.CalcularFreteCliente = configuracao.CalcularFreteCliente;

        if (carga.TipoOperacao?.ConfiguracaoCarga?.ConsiderarKMDaRotaFrete ?? false)
            carga.Distancia = 0;

        if (carga.TipoOperacao != null && carga.TipoOperacao.FretePorContadoCliente)
            carga.TipoFreteEscolhido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Cliente;

        if (carga.ModeloVeicularCarga == null)
        {
            if (carga.Veiculo?.ModeloVeicularCarga != null)
                carga.ModeloVeicularCarga = carga.Veiculo.ModeloVeicularCarga;
            else
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repositorioConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(_unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService = await repositorioConfiguracaoWebService.BuscarPrimeiroRegistroAsync();

                if (configuracaoWebService?.AdicionarVeiculoTipoReboqueComoReboqueAoAdicionarCarga ?? false)
                    carga.ModeloVeicularCarga = carga.VeiculosVinculados?.FirstOrDefault()?.ModeloVeicularCarga;
            }
        }

        Servicos.Embarcador.Integracao.IntegracaoCarga.AdicionarIntegracoesCarga(carga, cargaPedidos, tipoServicoMultisoftware, _unitOfWork, true); // refatorar

        if ((carga.ModeloVeicularCarga != null || carga.NaoExigeVeiculoParaEmissao) && carga.TipoDeCarga != null)
        {
            bool podeCalcular = true;

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.RetornoCargaPossuiOperacao retornoCargaPossuiOperacao = serCarga.VerificarSeCargaPossuiOperacao(carga, _unitOfWork);

                if (retornoCargaPossuiOperacao.TabelaQuePossuiOperacao && carga.TipoOperacao == null)
                {
                    podeCalcular = false;
                }
            }

            List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> cargaMotoristas = await repCargaMotorista.BuscarPorCargaAsync(carga.Codigo);

            if (carga.ExigeNotaFiscalParaCalcularFrete && (((carga.Veiculo == null || cargaMotoristas == null || cargaMotoristas.Count == 0) && !carga.NaoExigeVeiculoParaEmissao) || carga.Empresa == null || (carga.TipoOperacao?.ConfiguracaoCarga?.InformarTransportadorSubcontratadoEtapaUm ?? false)))
                podeCalcular = false;

            await serCargaPallets.RatearPaletesModeloVeicularCargaEntreOsPedidosAsync(carga, cargaPedidos);

            Servicos.Embarcador.Integracao.IntegracaoCarga.AdicionarIntegracoesCargaDadosTransporte(carga, cargaPedidos, cargaMotoristas, configuracao, tipoServicoMultisoftware, _unitOfWork);

            if (podeCalcular)
            {
                if (recriarRotas)
                {
                    Servicos.Embarcador.Carga.Rota serRota = new Servicos.Embarcador.Carga.Rota(_unitOfWork);
                    await serRota.DeletarPercursoDestinosCargaAsync(carga, _unitOfWork);
                }

                bool naoTemNenhumaPendenciaDeProdutos = !Servicos.Embarcador.Carga.Carga.IsCargaComPedidosSemProdutos(carga, _unitOfWork);
                bool naoTemClienteSemLocalidade = !Servicos.Embarcador.Carga.Carga.IsCargaComClienteSemLocalidade(carga, _unitOfWork);

                if (naoTemNenhumaPendenciaDeProdutos && naoTemClienteSemLocalidade)
                {
                    Servicos.Embarcador.Carga.Frete serFrete = new Frete(_unitOfWork, tipoServicoMultisoftware);
                    await serFrete.DefinirEtapaEFreteCargasAsync(carga, cargaPedidos, _unitOfWork);

                    if ((carga.SituacaoCarga == SituacaoCarga.AgNFe || (carga.SituacaoCarga == SituacaoCarga.CalculoFrete && !carga.ExigeNotaFiscalParaCalcularFrete)) && (carga.TipoOperacao?.ConfiguracaoCarga?.TempoParaRecebimentoDosPacotes ?? 0) > 0)
                        carga.DataSalvouDadosCarga = DateTime.Now;
                }
            }
            else if (!carga.NaoExigeVeiculoParaEmissao || (carga.TipoOperacao?.ExigirConfirmacaoDadosTransportadorAvancarCarga ?? false) || (carga.TipoOperacao?.ConfiguracaoCarga?.InformarTransportadorSubcontratadoEtapaUm ?? false))
                carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova;
        }
        else if (!carga.NaoExigeVeiculoParaEmissao || (carga.TipoOperacao?.ExigirConfirmacaoDadosTransportadorAvancarCarga ?? false) || (carga.TipoOperacao?.ConfiguracaoCarga?.InformarTransportadorSubcontratadoEtapaUm ?? false))
            carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova;

        if (carga.TipoFreteEscolhido == TipoFreteEscolhido.Embarcador &&
            ((carga.ExigeNotaFiscalParaCalcularFrete && !carga.CargaFechada) || (!carga.ExigeNotaFiscalParaCalcularFrete))
            && serCarga.VerificarSeCargaEstaNaLogistica(carga, tipoServicoMultisoftware))
        {
            Servicos.Embarcador.Carga.ComposicaoFrete.ComposicaoFrete.ExcluirComposicoesFrete(carga, _unitOfWork);
            new Servicos.Embarcador.Carga.Frete(_unitOfWork, tipoServicoMultisoftware).AtualizarFreteEmbarcadorCargaComponentes(carga, false, _unitOfWork);
        }

        serCarga.VerificarCalculoFretePeloBIDPedidoOrigem(carga, cargaPedidos, _unitOfWork);

        if (carga.TipoOperacao != null)
        {
            if (carga.TipoOperacao.HabilitarTipoPagamentoValePedagio)
                carga.TipoPagamentoValePedagio = carga.TipoOperacao.TipoPagamentoValePedagio;

            if (carga.TipoOperacao.GerarCTeComplementarNaCarga)
            {
                carga.EmitirCTeComplementar = true;
                carga.NaoGerarMDFe = true;
            }
        }

        if (cargaPedidos.Any(o => o.Pedido.PedidoTrocaNota))
            carga.CargaTrocaDeNota = true;

        foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
        {
            if ((carga.TipoOperacao?.GerarRedespachoAutomaticamente ?? false) && !cargaPedido.PendenteGerarCargaDistribuidor && cargaPedido.Recebedor != null)
            {
                cargaPedido.PendenteGerarCargaDistribuidor = true;
                carga.PendenteGerarCargaDistribuidor = true;
                await repCargaPedido.AtualizarAsync(cargaPedido);
            }
        }

        if (carga.Rota != null && carga.Rota.GerarRedespachoAutomaticamente && carga.Rota.Distribuidor != null && !carga.CargaAgrupada)
        {
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasOrigem = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                cargaPedido.PendenteGerarCargaDistribuidor = true;
                await repCargaPedido.AtualizarAsync(cargaPedido);

                if (!cargasOrigem.Contains(cargaPedido.CargaOrigem) && carga.CargaAgrupada)
                    cargasOrigem.Add(cargaPedido.CargaOrigem);

                Dominio.Entidades.Embarcador.Cargas.CargaPedidoDistribuicao cargaPedidoDistribuicao = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoDistribuicao();
                cargaPedidoDistribuicao.CargaPedido = cargaPedido;
                cargaPedidoDistribuicao.Empresa = carga.Empresa;
                if (carga.TipoOperacao != null)
                    cargaPedidoDistribuicao.TipoOperacao = carga.TipoOperacao;
                if (carga.ModeloVeicularCarga != null)
                    cargaPedidoDistribuicao.ModeloVeicularCarga = carga.ModeloVeicularCarga;

                await repCargaPedidoDistribuicao.InserirAsync(cargaPedidoDistribuicao);
            }

            carga.PendenteGerarCargaDistribuidor = true;
            if (carga.CargaAgrupada)
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem in cargasOrigem)
                {
                    carga.PendenteGerarCargaDistribuidor = true;
                    await repCarga.AtualizarAsync(cargaOrigem);
                }
            }
        }

        if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            await Servicos.Embarcador.Seguro.Seguro.SetarDadosSeguroCargaAsync(carga, cargaPedidos, configuracao, tipoServicoMultisoftware, _unitOfWork);

        await serCarga.SetarValePedagioPedidoCargaAsync(carga, _unitOfWork);
        await serCarga.AtualizarIntegracaoConsultaCalculoValePedagioAsync(carga, _unitOfWork, tipoServicoMultisoftware);

        if (configuracao.QuantidadeCargaPedidoProcessamentoLote > 0 && cargaPedidos.Count >= configuracao.QuantidadeCargaPedidoProcessamentoLote)
            carga.CalcularFreteLote = Dominio.Enumeradores.LoteCalculoFrete.Integracao;

        if (carga.TipoOperacao?.ProcessarDocumentoEmLote ?? false)
            carga.CalcularFreteLote = Dominio.Enumeradores.LoteCalculoFrete.Integracao;

        carga.Mercosul = carga.TipoOperacao?.TipoOperacaoMercosul ?? false;
        carga.EmitindoCRT = carga.TipoOperacao?.TipoOperacaoMercosul ?? false;
        carga.Internacional = carga.TipoOperacao?.ConfiguracaoCarga?.TipoOperacaoInternacional ?? false;

        await new Servicos.Embarcador.Carga.RotaFrete(_unitOfWork).SetarRotaFreteCargaAsync(carga, cargaPedidos, configuracao, tipoServicoMultisoftware);
        await Servicos.Embarcador.Carga.ColetaEntrega.FluxoColetaEntrega.AjustarFluxoGestaoColetaEntregaAsync(carga, _unitOfWork);
        await Servicos.Embarcador.Logistica.ProcedimentoEmbarque.SetarProcedimentoEmbarqueCargaAsync(carga, _unitOfWork);

        GerarRegistroPlanilhaImportada(carga, cargaPedidos);
        serCarga.AlterarSegmentoCarga(ref carga);

        if (tipoServicoMultisoftware == TipoServicoMultisoftware.MultiTMS)
            serCarga.AtualizarVeiculoEMotoristasPedidos(carga, auditado, _unitOfWork);

        if ((tipoServicoMultisoftware == TipoServicoMultisoftware.MultiTMS || tipoServicoMultisoftware == TipoServicoMultisoftware.MultiEmbarcador) && (carga.TipoOperacao?.DeslocamentoVazio ?? false))
        {
            carga.ProcessandoDocumentosFiscais = true;
            carga.DataInicioConfirmacaoDocumentosFiscais = DateTime.Now;
        }

        if ((carga.TipoOperacao?.ObrigatorioVincularContainerCarga ?? false) && tipoServicoMultisoftware != TipoServicoMultisoftware.MultiTMS && !servicoMensagemAlerta.IsMensagemSemConfirmacao(carga, TipoMensagemAlerta.CargaSemInformacaoContainer))
        {
            servicoMensagemAlerta.Adicionar(carga, TipoMensagemAlerta.CargaSemInformacaoContainer, "Para esse tipo de operação é necessário vincular container na carga");
        }

        carga.CargaPerigosaIntegracaoLeilao = cargaPedidos.Any(obj => obj.Pedido.PossuiCargaPerigosa);
        await AtualizarInformacoesPedidosPorCargaAsync(carga, cargaPedidos);

        Repositorio.Embarcador.Configuracoes.IntegracaoCorreios repositorioConfiguracaoIntegracaoCorreios = new Repositorio.Embarcador.Configuracoes.IntegracaoCorreios(_unitOfWork);
        Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCorreios configuracaoIntegracaoCorreios = repositorioConfiguracaoIntegracaoCorreios.Buscar();
        Repositorio.Embarcador.Pedidos.PedidoIntegracao repPedidoIntegracao = new Repositorio.Embarcador.Pedidos.PedidoIntegracao(_unitOfWork);

        await serCargaDadosSumarizados.AlterarDadosSumarizadosCargaAsync(carga, cargaPedidos, configuracao, _unitOfWork, tipoServicoMultisoftware);

        if (configuracaoIntegracaoCorreios != null && !string.IsNullOrWhiteSpace(configuracaoIntegracaoCorreios.URLPLP))
        {
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao> integracaoesPendente = await repPedidoIntegracao.BuscarPendentesIntegracaoPorPedidoAsync(cargaPedidos.Select(x => x.Pedido.Codigo).ToList());

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                if (integracaoesPendente.Any(o => o.Pedido.Codigo == cargaPedido.Pedido.Codigo))
                    return;

                if (!string.IsNullOrWhiteSpace(cargaPedido.Pedido?.NumeroRastreioCorreios ?? string.Empty))
                    Servicos.Embarcador.Integracao.Correios.IntegracaoCorreios.GerarIntegracaoPendente(cargaPedido.Pedido, true, _unitOfWork);
            }
        }

        if (configuracao.UtilizaEmissaoMultimodal)
            serCargaDadosSumarizados.ConsultarMDFeAquaviarioJaGerado(carga.Codigo, _unitOfWork);

        Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = null;

        if (adicionarJanelaCarregamento)
            cargaJanelaCarregamento = serCarga.AdicionarCargaJanelaCarregamento(carga, configuracao, tipoServicoMultisoftware, _unitOfWork, propriedades);

        if (adicionarJanelaDescarregamento)
        {
            Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDescarregamento configuracoesDescarregamento = new Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDescarregamento()
            {
                PermitirHorarioDescarregamentoComLimiteAtingido = propriedades?.PermitirHorarioDescarregamentoComLimiteAtingido ?? false,
                PermitirHorarioDescarregamentoInferiorAoAtual = propriedades?.PermitirHorarioDescarregamentoInferiorAoAtual ?? false
            };

            serCarga.AdicionarCargaJanelaDescarregamento(carga, cargaJanelaCarregamento, configuracao, _unitOfWork, tipoServicoMultisoftware, configuracoesDescarregamento);
        }

        if (configuracao.GerarFluxoPatioAoFecharCarga)
            serCarga.AdicionarFluxoGestaoPatio(carga, cargaJanelaCarregamento, _unitOfWork, tipoServicoMultisoftware, validarDadosTransporteInformados: false);

        if (carga.PreCarga != null)
        {
            servicoPreCarga.CriarAtualizarGestaoPatio(carga.PreCarga, tipoServicoMultisoftware);
            servicoPreCarga.AtualizarJanelaCarregamento(carga.PreCarga);
        }

        if (tipoServicoMultisoftware != TipoServicoMultisoftware.MultiTMS && gerarAgendamentoColeta)
            await AdicionarAgendamentoColetaAsync(carga, cargaPedidos, configuracao, tipoServicoMultisoftware);

        if (ClienteMultisoftware != null && notificarAcompanhamento && !viaWSRest)
            servAlertaAcompanhamentoCarga.informarNovoCardCargasAcompanamentoMSMQ(carga, ClienteMultisoftware.Codigo);

        if (cargaDePreCargaEmFechamento || tipoServicoMultisoftware == TipoServicoMultisoftware.MultiTMS || (configuracaoMonitoramento?.GerarMonitoramentoAoFecharCarga ?? false))
            Servicos.Embarcador.Monitoramento.Monitoramento.GerarMonitoriaPorCarga(carga, configuracao, auditado, "Fechamento de carga", _unitOfWork);

        if (carga.TipoOperacao?.GerarCargaFinalizada ?? false)
        {
            carga.SituacaoCarga = SituacaoCarga.Encerrada;
            carga = serCarga.AtualizarStatusCustoExtra(carga, servicoHubCarga, repCarga);
            carga.CalculandoFrete = false;
            Servicos.Auditoria.Auditoria.Auditar(auditado, carga, $"Alterou carga para situação {Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada.ObterDescricao()}", _unitOfWork);
        }

        serCarga.EnviarEmailNotificacaoAutomaticamenteTransportadorDaCarga(carga, _unitOfWork, auditado);
        serCarga.CriarCargaOferta(_unitOfWork, carga);

        //Devops 7945: Executar SalvarDadosTransporteCarga de forma assíncrona.
        carga.AguardandoSalvarDadosTransporteCarga = configuracaoGeralCarga?.ProcessarDadosTransporteAoFecharCarga ?? false;

        if (configuracao.CalcularFreteInicioCarga && !carga.CalculandoFrete && (carga.SituacaoCarga == SituacaoCarga.Nova || carga.SituacaoCarga == SituacaoCarga.AgNFe))
        {
            carga.CalculandoFrete = true;
            carga.CalcularFreteLote = LoteCalculoFrete.Integracao;
            await repCarga.AtualizarAsync(carga);
        }

        new Servicos.Embarcador.Integracao.IntegracaoCarga(_unitOfWork).AdicionarIntegracoesCargaFrete(carga, _unitOfWork);
        Servicos.Embarcador.Carga.CargaPedido.SumarizarDadosZonaTransporte(carga, _unitOfWork);

        serCarga.AdicionarMensagemAlertaCargaBloqueadaEnquantoNaoReceberDesbloqueioViaIntegracaoOuManual(carga, configuracaoGeralCarga, _unitOfWork);

        if (carga.SituacaoCarga == SituacaoCarga.AgNFe)
        {
            Servicos.Embarcador.Integracao.MercadoLivre.IntegracaoMercadoLivre svcIntegracaoMercadoLivre = new Integracao.MercadoLivre.IntegracaoMercadoLivre(_unitOfWork);
            svcIntegracaoMercadoLivre.VerificarConsultaRotaEFacilityAutomatizada(carga, cargaPedidos, tipoServicoMultisoftware, _unitOfWork);
        }

        try
        {
            new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(_unitOfWork, OrigemAlteracaoFilaCarregamento.Sistema).VincularCargaFilaCarregamentoCargaCancelada(carga, tipoServicoMultisoftware);
        }
        catch (Exception excecaoVincularFilaCarregamento)
        {
            Servicos.Log.TratarErro("Falha ao vincular a carga a uma fila de carregamento:");
            Servicos.Log.TratarErro(excecaoVincularFilaCarregamento);
        }
    }

    #region Métodos Privados

    private async Task AtualizarInformacoesPedidosPorCargaAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos)
    {
        if (carga.Carregamento == null)
            return;

        if ((cargaPedidos?.Count ?? 0) == 0)
            return;

        Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMontagemCarga configuracaoMontagemCarga = await new Repositorio.Embarcador.Configuracoes.ConfiguracaoMontagemCarga(_unitOfWork).BuscarPrimeiroRegistroAsync();

        if (!configuracaoMontagemCarga.AtualizarInformacoesPedidosPorCarga)
            return;

        Repositorio.RotaFrete repositorioRotaFrete = new Repositorio.RotaFrete(_unitOfWork);
        Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);

        List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = cargaPedidos.Select(o => o.Pedido).ToList();

        foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
        {
            pedido.Empresa = carga.Empresa;
            pedido.TipoDeCarga = carga.TipoDeCarga;
            pedido.TipoOperacao = carga.TipoOperacao;

            if ((pedido.Origem != null) && (pedido.Destino != null))
                pedido.RotaFrete = await repositorioRotaFrete.BuscarPorOrigemDestinoTipoOperacaoTransportadorAsync(pedido.Origem.Codigo, pedido.Destino.Codigo, pedido.TipoOperacao?.Codigo ?? 0, pedido.Empresa?.Codigo ?? 0);

            await repositorioPedido.AtualizarAsync(pedido);
        }
    }

    private async Task ObterTipoOperacaoPadraoAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos)
    {
        Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);
        Carga servicoCarga = new Carga(_unitOfWork);

        if (carga.TipoOperacao == null)
        {
            if (!string.IsNullOrWhiteSpace(carga.ObservacaoLocalEntrega))
            {
                if (carga.Filial != null)
                    carga.TipoOperacao = await repTipoOperacao.BuscarTipoOperacaoPadraoQuandoObservacaoEntregaInformadoNaIntegracaoAsync(carga.Filial.Codigo);

                if (carga.TipoOperacao == null)
                    carga.TipoOperacao = await repTipoOperacao.BuscarTipoOperacaoPadraoQuandoObservacaoEntregaInformadoNaIntegracaoAsync();
            }


            if (carga.TipoOperacao == null && carga.TipoDeCarga != null)
                carga.TipoOperacao = await repTipoOperacao.BuscarTipoOperacaoPorTipoDeCargaAsync(carga.TipoDeCarga.Codigo);

            if (carga.TipoOperacao == null)
            {
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacaoCargaEntreFiliais = await repTipoOperacao.BuscarTipoOperacaoPadraoCargaEntreFiliaisAsync();
                if (tipoOperacaoCargaEntreFiliais != null)
                {
                    Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
                    List<string> cnpjsFiliais = await repFilial.BuscarTodosCPNJsAsync();

                    bool entreFilial = true;

                    entreFilial = !cargaPedidos.Any(cargaPedido =>
                        (cargaPedido.Recebedor != null && !cnpjsFiliais.Contains(cargaPedido.Recebedor.CPF_CNPJ_SemFormato)) ||
                        (!cnpjsFiliais.Contains(cargaPedido.Pedido.Destinatario?.CPF_CNPJ_SemFormato)) ||
                        (cargaPedido.Expedidor != null && !cnpjsFiliais.Contains(cargaPedido.Expedidor.CPF_CNPJ_SemFormato)) ||
                        (!cnpjsFiliais.Contains(cargaPedido.Pedido.Remetente?.CPF_CNPJ_SemFormato))
                    );

                    if (entreFilial)
                        carga.TipoOperacao = tipoOperacaoCargaEntreFiliais;
                }
            }

            if (carga.TipoOperacao == null)
            {
                if (carga.Filial != null)
                    carga.TipoOperacao = await repTipoOperacao.BuscarTipoOperacaoPadraoQuandoNaoInformadaNaIntegracaoAsync(carga.Filial.Codigo);

                if (carga.TipoOperacao == null)
                {
                    Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new CargaDadosSumarizados(_unitOfWork);
                    Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = serCargaDadosSumarizados.ObterGrupPesssoasPrincipal(cargaPedidos, carga, _unitOfWork);
                    if (grupoPessoas != null)
                        carga.TipoOperacao = await repTipoOperacao.BuscarTipoOperacaoPadraoQuandoNaoInformadaNaIntegracaoGrupoPessoaAsync(grupoPessoas.Codigo);
                }

                if (carga.TipoOperacao == null)
                    carga.TipoOperacao = await repTipoOperacao.BuscarTipoOperacaoPadraoQuandoNaoInformadaNaIntegracaoAsync();
            }


            if (carga.TipoDeCarga == null)
                carga.TipoDeCarga = carga.TipoOperacao?.TipoDeCargaPadraoOperacao;

            if (carga.TipoOperacao != null && carga.PossuiPendencia)
            {
                carga.PossuiPendencia = false;
                carga.MotivoPendencia = "";

                if (carga.TipoDeCarga == null)
                    await ObterTipoCargaModeloAutoAsync(carga, cargaPedidos);
            }

            if (carga.TipoOperacao == null)
            {
                bool possuiForaPais = cargaPedidos.Any(x => x.Pedido.Remetente?.Localidade.Pais?.Codigo != x.Pedido.Destinatario?.Localidade.Pais?.Codigo);
                if (possuiForaPais)
                    carga.TipoOperacao = await repTipoOperacao.BuscarTipoOperacaoPadraoQuandoForadoPaisAsync();
                if (!possuiForaPais)
                    carga.TipoOperacao = await repTipoOperacao.BuscarTipoOperacaoPadraoQuandoDentrodoPaisAsync();
            }

            if (carga.TipoOperacao == null)
                carga.TipoOperacao = await servicoCarga.ProcessarRegrasTipoOperacaoAsync(carga, cargaPedidos, _unitOfWork);

            if (carga.TipoOperacao != null && carga.TipoOperacao.ExigeNotaFiscalParaCalcularFrete && !carga.ExigeNotaFiscalParaCalcularFrete)
                carga.ExigeNotaFiscalParaCalcularFrete = true;

        }
    }

    private async Task ObterTipoCargaModeloAutoAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos)
    {

        Repositorio.Embarcador.Cargas.TipoCargaModeloVeicularAutoConfig repTipoCargaModeloVeicularAutoConfig = new Repositorio.Embarcador.Cargas.TipoCargaModeloVeicularAutoConfig(_unitOfWork);
        Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicularAutoConfig tipoCargaModeloVeicularAutoConfig = await repTipoCargaModeloVeicularAutoConfig.BuscarPrimeiroRegistroAsync();

        if (tipoCargaModeloVeicularAutoConfig != null)
        {

            Repositorio.Embarcador.Produtos.GrupoProdutoTipoCarga repGrupoProdutoTipoCarga = new Repositorio.Embarcador.Produtos.GrupoProdutoTipoCarga(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTipoCarga> grupoProdutoTipoCargaSumarizado = new List<Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTipoCarga>();

            if (tipoCargaModeloVeicularAutoConfig.TipoAutomatizacaoTipoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAutomatizacaoTipoCarga.Desabilitado)
            {
                List<Dominio.Entidades.Embarcador.Produtos.GrupoProduto> gruposProdutoCarga = await repCargaPedidoProduto.BuscarGruposDeProdutosDaCargaAsync(carga.Codigo);

                Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCargaPadrao = null;

                if ((carga.TipoOperacao == null || carga.TipoOperacao.TipoDeCargaPadraoOperacao == null) && carga.TipoDeCarga == null)
                {
                    foreach (Dominio.Entidades.Embarcador.Produtos.GrupoProduto grupoProduto in gruposProdutoCarga)
                    {
                        if (grupoProduto == null)
                            continue;

                        List<Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTipoCarga> grupoProdutoTiposCarga = await repGrupoProdutoTipoCarga.ConsultarPorGrupoProdutoAsync(grupoProduto.Codigo);

                        if (grupoProdutoTiposCarga.Count <= 0)
                        {
                            carga.PossuiPendencia = true;
                            carga.MotivoPendencia = "O grupo de produto " + grupoProduto.Descricao + " não possui nenhum tipo de carga vinculado a ele.";
                            break;
                        }

                        foreach (Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTipoCarga grupoProdutoTipoCarga in grupoProdutoTiposCarga)
                        {
                            if (!grupoProdutoTipoCargaSumarizado.Exists(obj => obj.TipoDeCarga.Codigo == grupoProdutoTipoCarga.TipoDeCarga.Codigo && obj.Posicao == grupoProdutoTipoCarga.Posicao))
                                grupoProdutoTipoCargaSumarizado.Add(grupoProdutoTipoCarga);
                        }
                    }
                }

                List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> tiposCargasPrioritarias = null;

                if (tipoCargaModeloVeicularAutoConfig.TipoAutomatizacaoTipoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAutomatizacaoTipoCarga.PorPrioridade)
                    tiposCargasPrioritarias = (from obj in grupoProdutoTipoCargaSumarizado where obj.Posicao == 1 select obj.TipoDeCarga).Distinct().ToList();
                else
                    tiposCargasPrioritarias = (from obj in grupoProdutoTipoCargaSumarizado select obj.TipoDeCarga).Distinct().ToList();

                if (tiposCargasPrioritarias.Count > 1)
                {
                    if (tipoCargaModeloVeicularAutoConfig.TipoAutomatizacaoTipoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAutomatizacaoTipoCarga.PorPrioridade)
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.TipoCargaPrioridadeCargaAutoConfig> ordemPrioridades = new List<Dominio.Entidades.Embarcador.Cargas.TipoCargaPrioridadeCargaAutoConfig>();
                        Repositorio.Embarcador.Cargas.TipoCargaPrioridadeCargaAutoConfig repTipoCargaPrioridadeCargaAutoConfig = new Repositorio.Embarcador.Cargas.TipoCargaPrioridadeCargaAutoConfig(_unitOfWork);

                        List<Dominio.Entidades.Embarcador.Cargas.TipoCargaPrioridadeCargaAutoConfig> tipoCargaPrioridades = await repTipoCargaPrioridadeCargaAutoConfig.BuscarPorTipoCargaModeloAutoConfigAsync(tipoCargaModeloVeicularAutoConfig.Codigo);

                        foreach (Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga in tiposCargasPrioritarias)
                        {
                            Dominio.Entidades.Embarcador.Cargas.TipoCargaPrioridadeCargaAutoConfig tipoCargaPrioridadeCargaAutoConfig = (from obj in tipoCargaPrioridades where obj.TipoDeCarga.Codigo == tipoCarga.Codigo select obj).FirstOrDefault();
                            if (tipoCargaPrioridadeCargaAutoConfig != null)
                                ordemPrioridades.Add(tipoCargaPrioridadeCargaAutoConfig);
                        }

                        if (ordemPrioridades.Count > 0)
                        {
                            tipoCargaPadrao = (from obj in ordemPrioridades orderby obj.Posicao select obj.TipoDeCarga).FirstOrDefault();
                            foreach (Dominio.Entidades.Embarcador.Produtos.GrupoProduto grupoProduto in gruposProdutoCarga)
                            {
                                if (!await repGrupoProdutoTipoCarga.ExisteConsultarPorGrupoProdutoTipoCargaAsync(grupoProduto.Codigo, tipoCargaPadrao.Codigo))
                                {
                                    carga.PossuiPendencia = true;
                                    carga.MotivoPendencia = "O tipo de carga prioritário " + tipoCargaPadrao.Descricao + " não está vinculado ao grupo de produto " + grupoProduto.Descricao + ".";
                                    tipoCargaPadrao = null;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            string descricaoTipoSemPrioridade = "";
                            foreach (Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga in tiposCargasPrioritarias)
                                descricaoTipoSemPrioridade += ", " + tipoCarga.Descricao;

                            carga.PossuiPendencia = true;
                            carga.MotivoPendencia = "Não existe um tipo de carga prioritário entre os tipos" + descricaoTipoSemPrioridade + ".";
                        }
                    }
                    else if (tipoCargaModeloVeicularAutoConfig.TipoAutomatizacaoTipoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAutomatizacaoTipoCarga.PorValor)
                    {
                        decimal valorTotalPedidos = await repCargaPedidoProduto.ObterValorTotalPorCargaPedidosAsync(cargaPedidos.Select(x => x.Codigo).ToList());

                        List<Dominio.Entidades.Embarcador.Cargas.TipoCargaValorCargaAutoConfig> ordemPrioridades = new List<Dominio.Entidades.Embarcador.Cargas.TipoCargaValorCargaAutoConfig>();
                        Repositorio.Embarcador.Cargas.TipoCargaValorCargaAutoConfig repTipoCargaValorCargaAutoConfig = new Repositorio.Embarcador.Cargas.TipoCargaValorCargaAutoConfig(_unitOfWork);
                        List<Dominio.Entidades.Embarcador.Cargas.TipoCargaValorCargaAutoConfig> tiposCargaValor = await repTipoCargaValorCargaAutoConfig.BuscarPorTipoCargaModeloAutoConfigAsync(tipoCargaModeloVeicularAutoConfig.Codigo);

                        foreach (Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga in tiposCargasPrioritarias)
                        {

                            Dominio.Entidades.Embarcador.Cargas.TipoCargaValorCargaAutoConfig tipoCargaValorCargaAutoConfig = (from obj in tiposCargaValor
                                                                                                                               where obj.TipoCarga.Codigo == tipoCarga.Codigo && (obj.UFDestino == null || cargaPedidos.Any(ped => ped.Pedido.Destino?.Estado.Sigla == obj.UFDestino.Sigla)) &&
                                                                                                                                     ((obj.TipoValor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ValorAutomatizacaoTipoCargaValor.Ate && valorTotalPedidos <= obj.Valor) ||
                                                                                                                                      (obj.TipoValor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ValorAutomatizacaoTipoCargaValor.AcimaDe && valorTotalPedidos > obj.Valor))
                                                                                                                               select obj).FirstOrDefault();

                            if (tipoCargaValorCargaAutoConfig != null)
                                ordemPrioridades.Add(tipoCargaValorCargaAutoConfig);
                        }

                        if (ordemPrioridades.Count > 0)
                        {
                            tipoCargaPadrao = (from obj in ordemPrioridades orderby obj.Valor select obj.TipoCarga).FirstOrDefault();
                            foreach (Dominio.Entidades.Embarcador.Produtos.GrupoProduto grupoProduto in gruposProdutoCarga)
                            {
                                if (!await repGrupoProdutoTipoCarga.ExisteConsultarPorGrupoProdutoTipoCargaAsync(grupoProduto.Codigo, tipoCargaPadrao.Codigo))
                                {
                                    carga.PossuiPendencia = true;
                                    carga.MotivoPendencia = "O tipo de carga prioritário " + tipoCargaPadrao.Descricao + " não está vinculado ao grupo de produto " + grupoProduto.Descricao + ".";
                                    tipoCargaPadrao = null;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            string descricaoTipoSemPrioridade = "";
                            foreach (Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga in tiposCargasPrioritarias)
                                descricaoTipoSemPrioridade += ", " + tipoCarga.Descricao;

                            carga.PossuiPendencia = true;
                            carga.MotivoPendencia = "Não existe um tipo de carga prioritário entre os tipos" + descricaoTipoSemPrioridade + ".";
                        }
                    }
                }
                else
                {
                    if (tiposCargasPrioritarias.Count > 0)
                        tipoCargaPadrao = tiposCargasPrioritarias.FirstOrDefault();
                }

                if (tipoCargaPadrao != null)
                    carga.TipoDeCarga = tipoCargaPadrao;
            }

        }

        if (carga.TipoDeCarga != null && (tipoCargaModeloVeicularAutoConfig?.AutoModeloVeicularHabilitado ?? false))
        {
            Repositorio.Embarcador.Cargas.TipoCargaModeloVeicular repTipoCargaModeloVeicular = new Repositorio.Embarcador.Cargas.TipoCargaModeloVeicular(_unitOfWork);

            if (tipoCargaModeloVeicularAutoConfig.ControlarModeloPorNumeroPaletes)
            {
                int numeroPaletesfracionado = Convert.ToInt32((from obj in cargaPedidos select obj.Pedido.NumeroPaletesFracionado).Sum());
                int numeroPaletes = (from obj in cargaPedidos select obj.Pedido.NumeroPaletes).Sum() + numeroPaletesfracionado;

                List<Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular> tipoCargaModelosVeiculares = await repTipoCargaModeloVeicular.ConsultarPorTipoCargaeModeloVeicularPaletizadoAtivoAsync(carga.TipoDeCarga.Codigo);

                List<Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular> modelosComNumeroPaletesCompativel = (from obj in tipoCargaModelosVeiculares where (obj.ModeloVeicularCarga.NumeroPaletes != null && obj.ModeloVeicularCarga.NumeroPaletes.Value == numeroPaletes) select obj).ToList();

                int numeroModeloComMenosPaletes = 0;

                if (modelosComNumeroPaletesCompativel.Count == 0)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular> modelosSuportamNumeroPaletes = new List<Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular>();

                    foreach (Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular tipoCargaModeloVeicular in tipoCargaModelosVeiculares)
                    {
                        if (tipoCargaModeloVeicular.ModeloVeicularCarga.NumeroPaletes != null && tipoCargaModeloVeicular.ModeloVeicularCarga.NumeroPaletes.Value >= numeroPaletes)
                        {
                            modelosSuportamNumeroPaletes.Add(tipoCargaModeloVeicular);

                            if (numeroModeloComMenosPaletes == 0 || numeroModeloComMenosPaletes > tipoCargaModeloVeicular.ModeloVeicularCarga.NumeroPaletes.Value)
                                numeroModeloComMenosPaletes = tipoCargaModeloVeicular.ModeloVeicularCarga.NumeroPaletes.Value;
                        }
                    }

                    if (carga.ModeloVeicularCarga == null)
                    {
                        if (modelosSuportamNumeroPaletes.Count > 0)
                        {
                            if (!tipoCargaModeloVeicularAutoConfig.ControlarModeloPorPeso)
                            {
                                if (numeroModeloComMenosPaletes > 0)
                                {
                                    modelosComNumeroPaletesCompativel = (from obj in modelosSuportamNumeroPaletes where obj.ModeloVeicularCarga.NumeroPaletes.Value == numeroModeloComMenosPaletes select obj).ToList();
                                    carga.ModeloVeicularCarga = (from obj in modelosComNumeroPaletesCompativel orderby obj.Posicao select obj.ModeloVeicularCarga).FirstOrDefault();
                                }
                                else
                                {
                                    if (!carga.NaoExigeVeiculoParaEmissao)
                                    {
                                        carga.PossuiPendencia = true;
                                        carga.MotivoPendencia = "Não existe um modelo veicular cadastrado que suporte " + numeroPaletes + " paletes para o tipo de carga " + carga.TipoDeCarga.Descricao + ".";
                                    }
                                }
                            }
                            else
                            {
                                await ControlarModeloPorPesoAsync(carga, tipoCargaModeloVeicularAutoConfig, cargaPedidos, modelosSuportamNumeroPaletes);
                            }
                        }
                        else
                        {
                            if (tipoCargaModeloVeicularAutoConfig.ControlarModeloPorPeso)
                            {
                                List<Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular> modelosNaoPaletizados = (from obj in tipoCargaModelosVeiculares where !obj.ModeloVeicularCarga.VeiculoPaletizado select obj).ToList();
                                await ControlarModeloPorPesoAsync(carga, tipoCargaModeloVeicularAutoConfig, cargaPedidos, modelosNaoPaletizados);
                            }
                            else
                            {
                                if (!carga.NaoExigeVeiculoParaEmissao)
                                {
                                    carga.PossuiPendencia = true;
                                    carga.MotivoPendencia = "Não existe um modelo veicular cadastrado que suporte " + numeroPaletes + " paletes para o tipo de carga " + carga.TipoDeCarga.Descricao + ".";
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (carga.ModeloVeicularCarga == null)
                    {
                        if (tipoCargaModeloVeicularAutoConfig.ControlarModeloPorPeso)
                        {
                            await ControlarModeloPorPesoAsync(carga, tipoCargaModeloVeicularAutoConfig, cargaPedidos, modelosComNumeroPaletesCompativel);
                        }
                        else
                        {
                            carga.ModeloVeicularCarga = (from obj in modelosComNumeroPaletesCompativel orderby obj.Posicao select obj.ModeloVeicularCarga).FirstOrDefault();
                        }
                    }
                }
            }
            else
            {
                if (carga.ModeloVeicularCarga == null && tipoCargaModeloVeicularAutoConfig.ControlarModeloPorPeso)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular> tipoCargaModelosVeiculares = repTipoCargaModeloVeicular.ConsultarPorTipoCargaeModeloVeicularAtivo(carga.TipoDeCarga.Codigo);
                    await ControlarModeloPorPesoAsync(carga, tipoCargaModeloVeicularAutoConfig, cargaPedidos, tipoCargaModelosVeiculares);
                }
            }
        }

        if (carga.TipoDeCarga == null)
            carga.TipoDeCarga = carga.Filial?.TipoDeCarga;

    }

    private async Task ControlarModeloPorPesoAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicularAutoConfig tipoCargaModeloVeicularAutoConfig, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, List<Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular> tipoCargaModelosVeiculares)
    {
        decimal pesoCarga = (from obj in cargaPedidos select obj.Peso).Sum();
        decimal quantidadeCarga = 0;

        if (tipoCargaModelosVeiculares.Any(obj => obj.ModeloVeicularCarga.UnidadeCapacidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeCapacidade.Unidade))
        {
            Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(_unitOfWork);
            quantidadeCarga = await repCargaPedidoProduto.ObterQuantidadeTotalPorCargaAsync(carga.Codigo);
        }

        decimal menorPesoSuportado = 0;

        List<Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular> ModelosCapacidadeCarga = new List<Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular>();
        foreach (Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular tipoCargaModeloVeicular in tipoCargaModelosVeiculares)
        {
            decimal quantidadeComparativa = pesoCarga;
            if (tipoCargaModeloVeicular.ModeloVeicularCarga.UnidadeCapacidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeCapacidade.Unidade)
                quantidadeComparativa = quantidadeCarga;

            decimal pesoSuportado = tipoCargaModeloVeicular.ModeloVeicularCarga.CapacidadePesoTransporte + tipoCargaModeloVeicular.ModeloVeicularCarga.ToleranciaPesoExtra;
            if (pesoSuportado >= quantidadeComparativa && (!tipoCargaModeloVeicularAutoConfig.ConsiderarToleranciaMenorPesoModelo ||
                    quantidadeComparativa >= (tipoCargaModeloVeicular.ModeloVeicularCarga.CapacidadePesoTransporte - tipoCargaModeloVeicular.ModeloVeicularCarga.ToleranciaPesoMenor)))
            {
                ModelosCapacidadeCarga.Add(tipoCargaModeloVeicular);

                if (menorPesoSuportado == 0 || pesoSuportado < menorPesoSuportado)
                    menorPesoSuportado = pesoSuportado;
            }
        }
        if (ModelosCapacidadeCarga.Count > 0)
        {
            carga.ModeloVeicularCarga = (from obj in ModelosCapacidadeCarga orderby obj.Posicao select obj.ModeloVeicularCarga).FirstOrDefault();
        }
        else
        {
            if (!carga.NaoExigeVeiculoParaEmissao)
            {
                carga.PossuiPendencia = true;
                string mensagemPalete = "";
                if (tipoCargaModeloVeicularAutoConfig.ControlarModeloPorNumeroPaletes)
                {
                    int numeroPaletes = (from obj in cargaPedidos select obj.Pedido.NumeroPaletes).Sum();
                    mensagemPalete = " com capacidade de transportar " + numeroPaletes.ToString() + " paletes, ";
                }
                carga.MotivoPendencia = "Não foi possível encontar um veículo " + mensagemPalete + " que suporte " + pesoCarga.ToString("n2") + " quilos para o tipo de carga " + carga.TipoDeCarga.Descricao + ".";
            }
        }
    }

    private async Task VerificarCargaTrajetoAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga)
    {
        Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
        Repositorio.Embarcador.Cargas.CargaTrajeto repositorioCargaTrajeto = new Repositorio.Embarcador.Cargas.CargaTrajeto(_unitOfWork);

        Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = await repositorioCargaPedido.BuscarPrimeiroPedidoPorCargaAsync(carga.Codigo);
        Dominio.Entidades.Embarcador.Cargas.CargaTrajeto cargaTrajeto = null;

        if (cargaPedido == null)
            return;

        if (cargaPedido.CargaPedidoTrechoAnterior == null)
            cargaTrajeto = await GerarCargaTrajetoAsync(cargaPedido);
        else
            cargaTrajeto = await repositorioCargaTrajeto.BuscarPorCargaAsync(cargaPedido.CargaPedidoTrechoAnterior.Carga.Codigo);

        if (cargaTrajeto == null)
            return;

        await AdicionarCargaAoTrajetoAsync(carga, cargaTrajeto);
    }

    private async Task<Dominio.Entidades.Embarcador.Cargas.CargaTrajeto> GerarCargaTrajetoAsync(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
    {
        Repositorio.Embarcador.Cargas.CargaTrajeto repositorioCargaTrajeto = new Repositorio.Embarcador.Cargas.CargaTrajeto(_unitOfWork);

        Dominio.Entidades.Embarcador.Cargas.CargaTrajeto cargaTrajeto = new Dominio.Entidades.Embarcador.Cargas.CargaTrajeto()
        {
            SituacaoTrajeto = SituacaoTrajeto.Origem,
            QuilometragemTotal = 0,
            PercentualConcluido = 0,
            Destinatario = cargaPedido.Recebedor ?? cargaPedido.Pedido.Destinatario
        };

        await repositorioCargaTrajeto.InserirAsync(cargaTrajeto);

        return cargaTrajeto;
    }

    private async Task AdicionarCargaAoTrajetoAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaTrajeto cargaTrajeto)
    {
        Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
        Repositorio.Embarcador.Cargas.CargaTrajeto repositorioCargaTrajeto = new Repositorio.Embarcador.Cargas.CargaTrajeto(_unitOfWork);
        Repositorio.Embarcador.Cargas.CargaTrajetoCarga repositorioCargaTrajetoCarga = new Repositorio.Embarcador.Cargas.CargaTrajetoCarga(_unitOfWork);

        Carga servicoCarga = new Carga(_unitOfWork);

        Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = await repositorioCargaPedido.BuscarPrimeiroPedidoPorCargaAsync(carga.Codigo);
        Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = await new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarConfiguracaoPadraoAsync();
        
        cargaTrajeto.QuilometragemTotal += servicoCarga.ObterDistancia(carga, configuracaoTMS, _unitOfWork);
        cargaTrajeto.Destinatario = cargaPedido.Recebedor ?? cargaPedido.Pedido?.Destinatario ?? null;

        await repositorioCargaTrajeto.AtualizarAsync(cargaTrajeto);

        int novaOrdem = await repositorioCargaTrajetoCarga.BuscarOrdemPorCargaTrajetoAsync(cargaTrajeto.Codigo);

        Dominio.Entidades.Embarcador.Cargas.CargaTrajetoCarga cargaTrajetoCarga = new Dominio.Entidades.Embarcador.Cargas.CargaTrajetoCarga()
        {
            Carga = carga,
            CargaTrajeto = cargaTrajeto,
            SituacaoTrajetoCarga = SituacaoTrajetoCarga.AguardandoInicio,
            Data = DateTime.Now,
            Ordem = novaOrdem + 1
        };

        await repositorioCargaTrajetoCarga.InserirAsync(cargaTrajetoCarga);
    }

    private async Task AlterarNumeroCargaPorPadraoGeracaoConfiguradoAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga)
    {
        Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = await new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(_unitOfWork).BuscarPrimeiroRegistroAsync();

        if (string.IsNullOrWhiteSpace(configuracaoGeralCarga?.PadraoGeracaoNumeroCarga))
            return;

        if (!string.IsNullOrWhiteSpace(carga.CodigoAlfanumericoEmpresa))
            return;

        if (configuracaoGeralCarga.PadraoGeracaoNumeroCarga.Contains("#CodigoAlfanumericoEmpresa"))
        {
            await new Servicos.Embarcador.Transportadores.Empresa(_unitOfWork).AtualizarCodigoAlfanumericoAsync(carga.Empresa);

            carga.CodigoAlfanumericoEmpresa = carga.Empresa.CodigoAlfanumerico;
        }

        carga.CodigoCargaEmbarcador = configuracaoGeralCarga.PadraoGeracaoNumeroCarga
            .Replace("#Ano", carga.DataCriacaoCarga.ToString("yyyy"))
            .Replace("#Mes", carga.DataCriacaoCarga.ToString("MM"))
            .Replace("#CodigoIntegracaoEmpresa", carga.Empresa.CodigoIntegracao)
            .Replace("#CodigoAlfanumericoEmpresa", carga.CodigoAlfanumericoEmpresa);
    }

    private void GerarRegistroPlanilhaImportada(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos)
    {
        Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI repControleIntegracaoCargaEDI = new Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI(_unitOfWork);
        List<(string Nome, string Guid)> arquivosGeracao = (from obj in cargaPedidos
                                                            where !string.IsNullOrEmpty(obj.Pedido.GuidArquivoGerador)
                                                            select ValueTuple.Create(obj.Pedido.NomeArquivoGerador, obj.Pedido.GuidArquivoGerador)).Distinct().ToList();

        List<Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI> controlesIntegracaoCargaEDI = new List<Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI>();

        foreach ((string Nome, string Guid) arquivo in arquivosGeracao)
        {
            Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI arquivoGeracao = new Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI()
            {
                Data = DateTime.Now,
                MensagemRetorno = "",
                NumeroDT = "",
                SituacaoIntegracaoCargaEDI = SituacaoIntegracaoCargaEDI.Integrado,
                ArquivoImportacaoPedido = true,
                GuidArquivo = arquivo.Guid,
                NomeArquivo = arquivo.Nome,
                Cargas = new List<Dominio.Entidades.Embarcador.Cargas.Carga>() { carga }
            };

            controlesIntegracaoCargaEDI.Add(arquivoGeracao);
        }

        repControleIntegracaoCargaEDI.Inserir(controlesIntegracaoCargaEDI, "T_CONTROLE_INTEGRACAO_CARGA_EDI");
    }

    private async Task ObterDadosVeiculosEMotoristasAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga)
    {
        if (string.IsNullOrEmpty(carga.ExternalID2) || carga.TipoOperacao == null || !(carga.TipoOperacao?.ConfiguracaoCarga?.HerdarDadosDeTransporteCargaPrimeiroTrecho ?? false))
            return;

        Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
        Dominio.Entidades.Embarcador.Cargas.Carga existeCargaExternalIgual = await repositorioCarga.BuscarPrimeiraCargaPorExternalID2Async(carga.ExternalID2);
        Servicos.Embarcador.Carga.MensagemAlertaCarga servicoMensagemAlerta = new Servicos.Embarcador.Carga.MensagemAlertaCarga(_unitOfWork);

        List<Dominio.ObjetosDeValor.Embarcador.Alertas.MensagemAlerta> existeMensagems = servicoMensagemAlerta.ObterMensagensPorEntidades(new List<int> { carga.Codigo });

        if (existeCargaExternalIgual == null)
        {
            if ((existeMensagems == null || existeMensagems.Count == 0) || !existeMensagems.Any(m => m.Tipo == TipoMensagemAlerta.NaoPodeHerdarDadosTransporte))
                servicoMensagemAlerta.Adicionar(carga, TipoMensagemAlerta.NaoPodeHerdarDadosTransporte, $"Por que não foi encontrada carga com ExternalID1 = {carga.ExternalID2}");
            return;
        }

        List<Dominio.Entidades.Usuario> motoristas = existeCargaExternalIgual.Motoristas.ToList();
        Dominio.Entidades.Veiculo veiculo = existeCargaExternalIgual.Veiculo;
        List<Dominio.Entidades.Veiculo> veiculosVinculados = existeCargaExternalIgual.VeiculosVinculados != null && existeCargaExternalIgual.VeiculosVinculados.Any() ? existeCargaExternalIgual.VeiculosVinculados.ToList() : null;

        if (veiculo == null || motoristas == null || motoristas.Count == 0 || veiculosVinculados == null || veiculosVinculados.Count == 0)
        {
            if ((existeMensagems == null || existeMensagems.Count == 0) || !existeMensagems.Any(m => m.Tipo == TipoMensagemAlerta.NaoPodeHerdarDadosTransporte))
                servicoMensagemAlerta.Adicionar(carga, TipoMensagemAlerta.NaoPodeHerdarDadosTransporte, "Por que a carga de primeiro trecho não possui dados de transporte");

            return;
        }

        carga.VeiculosVinculados = veiculosVinculados;
        carga.Motoristas = motoristas;
        carga.Veiculo = veiculo;
        servicoMensagemAlerta.Confirmar(carga, TipoMensagemAlerta.NaoPodeHerdarDadosTransporte);
    }

    private async Task AgruparNotasPedidosValoresZeradosFechamentoCargaAsync(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedido)
    {
        if (listaCargaPedido != null && listaCargaPedido.Count > 0 && (listaCargaPedido?.FirstOrDefault()?.Pedido?.NumeroPedidoEmbarcador?.Contains("_") ?? false))
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);

            List<string> listaNumeroPedidosPedidos = (from o in listaCargaPedido where o.ValorFrete > 0 select o.Pedido.NumeroPedidoEmbarcador.Split('_').FirstOrDefault()).ToList();
            foreach (string numeroPerido in listaNumeroPedidosPedidos)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoComValor = (from o in listaCargaPedido where o.Pedido.NumeroPedidoEmbarcador.Contains(numeroPerido) && o.ValorFrete > 0 select o).FirstOrDefault();
                if (cargaPedidoComValor != null)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaPedidosSemValor = (from o in listaCargaPedido where o.Pedido.NumeroPedidoEmbarcador.Contains(numeroPerido) && o.ValorFrete <= 0 select o).ToList();
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoSemValor in listaPedidosSemValor)
                    {
                        List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> listaPedidoXMLNotaFiscal = await repPedidoXMLNotaFiscal.BuscarPorCargaPedidoSemFetchAsync(cargaPedidoSemValor.Codigo);
                        foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in listaPedidoXMLNotaFiscal)
                        {
                            pedidoXMLNotaFiscal.CargaPedido = cargaPedidoComValor;
                            await repPedidoXMLNotaFiscal.AtualizarAsync(pedidoXMLNotaFiscal);
                        }

                        cargaPedidoSemValor.Ativo = false;
                        cargaPedidoSemValor.PedidoSemNFe = true;
                        await repCargaPedido.AtualizarAsync(cargaPedidoSemValor);
                    }
                }
            }

        }

    }

    private async Task AdicionarAgendamentoColetaAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
    {
        if (cargaPedidos == null || cargaPedidos.Count == 0)
            return;

        Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaPedidos.FirstOrDefault();
        Servicos.Embarcador.Logistica.CargaJanelaDescarregamento servicoJanelaDescarregamento = new Servicos.Embarcador.Logistica.CargaJanelaDescarregamento(_unitOfWork);
        Servicos.Embarcador.Logistica.AgendamentoColeta servicoAgendamentoColeta = new Servicos.Embarcador.Logistica.AgendamentoColeta(_unitOfWork);

        Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
        Repositorio.Embarcador.Logistica.AgendamentoColeta repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(_unitOfWork);
        Repositorio.Embarcador.Configuracoes.ConfiguracaoAgendamentoColeta repositorioConfiguracaoAgendamentoColeta = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAgendamentoColeta(_unitOfWork);
        Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(_unitOfWork);
        Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);

        bool naoGerarAgendamentoColeta = await repositorioTipoIntegracao.ExistePorTipoAsync(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Unilever);

        if (naoGerarAgendamentoColeta)
            return;

        Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAgendamentoColeta configuracaoAgendamentoColeta = await repositorioConfiguracaoAgendamentoColeta.BuscarPrimeiroRegistroAsync();
        List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta> listaAgendamentosColeta = await repositorioAgendamentoColeta.BuscarPorPedidosAsync(cargaPedidos.Select(x => x.Pedido.Codigo).ToList());
        Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoPorColeta = await repositorioAgendamentoColeta.BuscarPorCargaAsync(carga.Codigo);
        Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaCarregamento = await repositorioCargaJanelaDescarregamento.BuscarPorCargaAsync(carga.Codigo);

        if (agendamentoPorColeta != null)
            listaAgendamentosColeta.Add(agendamentoPorColeta);

        Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColetaAdicionar;

        if (!configuracao.ControlarAgendamentoSKU && listaAgendamentosColeta.Count > 0)
        {
            Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta = listaAgendamentosColeta.FirstOrDefault(obj => obj.Carga == null);
            if (agendamentoColeta != null)
            {
                agendamentoColetaAdicionar = agendamentoColeta.Clonar<Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta>();
                agendamentoColetaAdicionar.Carga = carga;
                agendamentoColetaAdicionar.AgendamentoPai = false;
                agendamentoColetaAdicionar.CodigoControle = carga.Codigo;
                agendamentoColetaAdicionar.Pedidos = new List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido>();
                agendamentoColeta.AgendamentoPai = true;
                await repositorioAgendamentoColeta.InserirAsync(agendamentoColetaAdicionar);
                await repositorioAgendamentoColeta.AtualizarAsync(agendamentoColeta);
            }

        }
        else if (cargaPedido.Expedidor == null && cargaPedido.Pedido.Remetente.Modalidades?.Count > 0 && cargaPedido.Pedido.Remetente.Modalidades.Any(obj => obj.TipoModalidade == TipoModalidade.Fornecedor) && listaAgendamentosColeta.Count <= 0)
        {

            agendamentoColetaAdicionar = new Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta()
            {
                Situacao = SituacaoAgendamentoColeta.AguardandoConfirmacao,
                EtapaAgendamentoColeta = configuracao.ControlarAgendamentoSKU ? EtapaAgendamentoColeta.AguardandoAceite : configuracaoAgendamentoColeta.RemoverEtapaAgendamentoAgendamentoColeta ? EtapaAgendamentoColeta.NFe : EtapaAgendamentoColeta.DadosTransporte
            };

            agendamentoColetaAdicionar.Recebedor = cargaPedido.Recebedor;
            agendamentoColetaAdicionar.DataAgendamento = carga.DataCarregamentoCarga.HasValue ? carga.DataCarregamentoCarga.Value : carga.DataCriacaoCarga;
            agendamentoColetaAdicionar.DataEntrega = cargaPedido.Pedido.PrevisaoEntrega.HasValue ? cargaPedido.Pedido.PrevisaoEntrega.Value : DateTime.Now;
            agendamentoColetaAdicionar.DataColeta = cargaPedido.Pedido.DataCarregamentoCarga.HasValue ? cargaPedido.Pedido.DataCarregamentoCarga.Value : DateTime.Now;
            agendamentoColetaAdicionar.Volumes = carga.DadosSumarizados.VolumesTotal;
            agendamentoColetaAdicionar.ValorTotalVolumes = carga.DadosSumarizados.ValorTotalMercadoriaPedidos;
            agendamentoColetaAdicionar.Peso = carga.DadosSumarizados.PesoTotal;
            agendamentoColetaAdicionar.Observacao = "";
            agendamentoColetaAdicionar.CodigoControle = carga.Codigo;
            agendamentoColetaAdicionar.Remetente = cargaPedido.Pedido.Remetente;
            agendamentoColetaAdicionar.AgendamentoPai = false;

            agendamentoColetaAdicionar.Destinatario = cargaPedido.Pedido.Destinatario;
            agendamentoColetaAdicionar.ModeloVeicular = carga.ModeloVeicularCarga;
            agendamentoColetaAdicionar.TipoCarga = carga.TipoDeCarga;
            agendamentoColetaAdicionar.Transportador = carga.Empresa;

            if (agendamentoColetaAdicionar.Codigo == 0 && !configuracao.NaoGerarSenhaAgendamento)
                agendamentoColetaAdicionar.Senha = servicoAgendamentoColeta.ObterSenhaAgendamentoColeta(cargaJanelaCarregamento, agendamentoColetaAdicionar, configuracaoAgendamentoColeta);

            agendamentoColetaAdicionar.Carga = carga;


            if (carga.Codigo > 0)
                await repositorioCarga.AtualizarAsync(carga);

            await repositorioAgendamentoColeta.InserirAsync(agendamentoColetaAdicionar);

            if (servicoJanelaDescarregamento.IsPermitirAdicionar(carga))
                servicoJanelaDescarregamento.Adicionar(agendamentoColetaAdicionar, tipoServicoMultisoftware);
        }
    }

    #endregion

}
