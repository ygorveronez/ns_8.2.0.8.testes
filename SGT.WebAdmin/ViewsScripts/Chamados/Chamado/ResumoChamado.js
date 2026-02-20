/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumTipoChamado.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _resumoChamado;

var _notasFiscaisCompletas = "";
var _notasFiscaisResumidas = "";
var _expandirNotasFiscais = false;

var ResumoChamado = function () {
    this.Carga = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.NumeroCarga, visible: ko.observable(false) });
    this.CodigosAgrupadosCarga = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.CargasAgrupadas });
    this.Empresa = PropertyEntity({ text: ko.observable(Localization.Resources.Chamado.ChamadoOcorrencia.Transportador), visible: ko.observable(true) });
    this.Veiculo = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.Veiculo });
    this.TipoVeiculo = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.TipoVeiculo });
    this.CodigoCliente = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.CodigoCliente, visible: ko.observable(true) });
    this.Cliente = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.Cliente, visible: ko.observable(true) });
    this.ClienteFantasia = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.Fantasia, visible: ko.observable(true) });
    this.ClienteEndereco = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.Endereco, visible: ko.observable(true) });
    this.ClienteBairro = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.ClienteBairro, visible: ko.observable(true) });
    this.ClienteCEP = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.ClienteCEP, visible: ko.observable(true) });
    this.ClienteCidade = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.CidadeCliente, visible: ko.observable(true) });
    this.Motorista = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.Motorista });
    this.MotoristaTelefone = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.TelefoneMotorista });
    this.Origem = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.Origem });
    this.Destino = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.Destino });
    this.Recebedor = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.Recebedor });
    this.Expedidor = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.Expedidor});
    this.Tomador = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.Tomador, visible: ko.observable(true) });
    this.Destinatario = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.Destinatario });
    this.DestinatarioFantasia = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.Fantasia });
    this.DataChamado = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.DataAberturaAtendimento });
    this.DataRegistroMotorista = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.DataRegistroMotorista, visible: ko.observable(false) });
    this.NumeroPedidoEmbarcador = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.NumeroPedidoEmbarcador });
    this.Situacao = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.Situacao });
    this.ObservacoesCarga = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.ObservacoesCarga });
    this.DataHoraFaturamento = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.DataFaturamento, visible: ko.observable(true) });
    this.Atraso = PropertyEntity({ text: ko.observable(Localization.Resources.Chamado.ChamadoOcorrencia.DiasAtraso), visible: ko.observable(true) });
    this.Numero = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.Numero, visible: ko.observable(true) });
    this.NotasFiscais = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.NotasFiscaisCarga, visible: ko.observable(true) });
    this.DataHoraPrevisaoEntrega = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.PrevisaoEntrega});
    this.VeiculoCarregado = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.VeiculoCarregado, visible: ko.observable(true) });
    this.Notificado = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.ClienteNotificadoPorEmail, visible: ko.observable(true)});
    this.FilialVenda = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.FilialVenda });
    this.FuncionarioVendedor = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.Vendedor });
    this.FuncionarioVendedorEmail = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.VendedorEmail });
    this.FuncionarioVendedorTelefone = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.VendedorTelefone });
    this.DataInicioViagemCarga = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.DataInicioViagemCarga });
    this.DataEmissaoDocumentoFreteCarga = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.DataEmissaoDocumentoFreteCarga });
    this.DataEntradaRaio = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.DataChegada, visible: ko.observable(false) });
    this.DataSaidaRaio = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.DataSaida, visible: ko.observable(false) });
    this.DataRetencaoInicio = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.DataRetencaoInicio });
    this.DataRetencaoFim = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.DataRetencaoFim });
    this.PeriodoJanelaDescarga = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.PeriodoJanelaDescarga });
    this.FilialCarga = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.FilialCarregamento });
    this.ModeloVeiculo = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.ModeloVeiculo });
    this.NumeroOrdem = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.NumeroOrdem});
    this.CustoFrete = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.CustoFrete });
    this.NotasFiscaisSelecionadasAtendimento = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.NotasFiscaisSelecionadasAtendimento });
    this.TipoOperacaoCarga = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.TipoOperacao });
    this.Genero = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.Genero });
    this.AreaEnvolvida = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.AreaEnvolvida });
    this.ValorNFe = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.ValorNFe });
    this.DataEntradaRaioEntrega = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.DataEntradaRaio });
    this.DataSaidaRaioEntrega = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.DataSaidaRaio });
    this.Volume = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.VolumesTotalCarga });
    this.VolumeNotasSelecionadas = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.VolumesNotaAtendimento });
    this.LeadTimeTransportador = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.LeadTimeTransportador });
    this.Parqueada = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.Parqueada });

    // Dados da diária automática
    this.DiariaAutomaticaTempoCobrado = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.TempoCobradoMinutos });
    this.DiariaAutomaticaLocalFreeTime = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.TipoFreeTime });
    this.DiariaAutomaticaTempoFreeTime = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.TempoFreeTimeMinutos });
    this.DiariaAutomaticaValorTotal = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.ValorTotal });
    this.DiariaAutomaticaValorPorHora = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.ValorTotalHora });
    this.DiariaAutomaticaEntradasESaidas = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.ListaParadasDiariaAutomatica });

    this.ExpandirNotasFiscais = PropertyEntity({ eventClick: AlterarValorNotasFiscais, type: types.event });
    this.VerMais = PropertyEntity({ val: ko.observable(" ...Ver Todas") });
    this.OperacaoCargaPedido = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.OperacaoCargaPedido });
    this.TransporteEVeiculos = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.TransporteEVeiculos });
    this.Atendimento = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.Atendimento });
    this.ContatoEndereco = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.ContatoEndereco });
    this.LocalizacaoOperacional = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.LocalizacaoOperacional });
    this.PrazosDatas = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.PrazosDatas });
    this.RecebimentoHorarios = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.RecebimentoHorarios });
    this.Descarga = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.Descarga });
    this.RestricoesEspecificacoes = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.RestricoesEspecificacoes });
    this.Observacao = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.Observacao });
    this.Outros = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.Outros });

    this.ClienteUF = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.ClienteUF, visible: ko.observable(true) });
    this.ClientePais = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.ClientePais, visible: ko.observable(true) });
    this.ClienteTelefone = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.Telefone, visible: ko.observable(true) });
    this.ClienteEmail = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.ClienteEmail, visible: ko.observable(true) });
    this.ClienteEmailNFe = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.ClienteEmailNFe, visible: ko.observable(true) });
    this.CNPJCliente = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.CNPJCliente, visible: ko.observable(true) });
    this.ClienteMatriz = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.Matriz, visible: ko.observable(true) });
    this.CodigoClienteMatriz = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.CodigoClienteMatriz, visible: ko.observable(true) });
    this.MatrizReferencia = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.MatrizReferencia, visible: ko.observable(true) });
    this.GrupoCliente = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.GrupoCliente, visible: ko.observable(true) });
    this.DescricaoTipoVeiculo = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.DescricaoTipoVeiculo, visible: ko.observable(true) });
    this.ParticionamentoVeiculo = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.ParticionamentoVeiculo, visible: ko.observable(true) });
    this.DescricaoParticionamentoVeiculo = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.DescricaoParticionamentoVeiculo, visible: ko.observable(true) });
    this.AlturaRecebimento = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.AlturaRecebimento, visible: ko.observable(true) });
    this.DescricaoAlturaRecebimento = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.DescricaoAlturaRecebimento, visible: ko.observable(true) });
    this.RestricaoCarregamento = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.RestricaoCarregamento, visible: ko.observable(true) });
    this.DescricaoRestricaoCarregamento = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.DescricaoRestricaoCarregamento, visible: ko.observable(true) });
    this.ComposicaoPalete = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.ComposicaoPalete, visible: ko.observable(true) });
    this.DescricaoComposicaoPalete = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.DescricaoComposicaoPalete, visible: ko.observable(true) });
    this.CustoDescarga = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.CustoDescarga, visible: ko.observable(true) });
    this.TipoCusto = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.TipoCusto, visible: ko.observable(true) });
    this.Ajudantes = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.Ajudantes, visible: ko.observable(true) });
    this.PagamentoDescarga = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.PagamentoDescarga, visible: ko.observable(true) });
    this.DescricaoPagamentoDescarga = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.DescricaoPagamentoDescarga, visible: ko.observable(true) });
    this.SegundaRemessa = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.SegundaRemessa, visible: ko.observable(true) });
    this.ExclusividadeEntrega = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.ExclusividadeEntrega, visible: ko.observable(true) });
    this.Paletizacao = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.Paletizacao, visible: ko.observable(true) });
    this.ClienteStrechado = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.ClienteStrechado, visible: ko.observable(true) });
    this.Agendamento = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.Agendamento, visible: ko.observable(true) });
    this.ClienteComMulta = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.ClienteComMulta, visible: ko.observable(true) });
    this.CapacidadeRecebimento = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.CapacidadeRecebimento, visible: ko.observable(true) });

    this.RecebimentoSegundaFeira = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.SegundaFeira });
    this.RecebimentoTercaFeira = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.TercaFeira });
    this.RecebimentoQuartaFeira = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.QuartaFeira });
    this.RecebimentoQuintaFeira = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.QuintaFeira });
    this.RecebimentoSextaFeira = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.SextaFeira });
    this.RecebimentoSabado = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.Sabado });
    this.RecebimentoDomingo = PropertyEntity({ text: Localization.Resources.Chamado.ChamadoOcorrencia.Domingo });
};

//*******EVENTOS*******

function LoadResumoChamado() {
    _resumoChamado = new ResumoChamado();
    KoBindings(_resumoChamado, "knockoutResumoChamado");

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        _resumoChamado.Empresa.text(Localization.Resources.Chamado.ChamadoOcorrencia.EmpresaFilial);
        _resumoChamado.DataHoraFaturamento.visible(false);
        _resumoChamado.Atraso.visible(false);
        _resumoChamado.VeiculoCarregado.visible(false);
    } else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe) {
        _resumoChamado.Empresa.visible(false);
    }

    if (_CONFIGURACAO_TMS.NaoExibirInfosAdicionaisGridPatio) {
        _resumoChamado.Tomador.visible(false);
        _resumoChamado.VeiculoCarregado.visible(false);
    }
    if (_CONFIGURACAO_TMS.VisualizarDatasRaioNoAtendimento) {
        _resumoChamado.DataEntradaRaio.visible(true);
        _resumoChamado.DataSaidaRaio.visible(true);
    }
}

//*******MÉTODOS*******

function PreecherResumoChamado(dados) {
    PreencherObjetoKnout(_resumoChamado, { Data: dados });
    _resumoChamado.Carga.visible(true);

    if (dados.Atrazo == 0)
        _resumoChamado.Atraso.val(Localization.Resources.Chamado.ChamadoOcorrencia.CargaFaturadaDia);
    else if (dados.Atrazo < 0) {
        _resumoChamado.Atraso.text(Localization.Resources.Chamado.ChamadoOcorrencia.CargaAntecipadaEm);
        dados.Atrazo = dados.Atrazo * -1;
        _resumoChamado.Atraso.val(dados.Atrazo + Localization.Resources.Chamado.ChamadoOcorrencia.Dia  + (dados.Atrazo > 1 ? "s" : ""));
    }
    else {
        _resumoChamado.Atraso.text(Localization.Resources.Chamado.ChamadoOcorrencia.DiasAtraso);
        _resumoChamado.Atraso.val(Localization.Resources.Chamado.ChamadoOcorrencia.CargaFaturadaCom + dados.Atrazo + Localization.Resources.Chamado.ChamadoOcorrencia.DiasAtraso + (dados.Atrazo > 1 ? "s" : "") + Localization.Resources.Chamado.ChamadoOcorrencia.DeAtraso);
    }

    if (_CONFIGURACAO_TMS.TipoChamado == EnumTipoChamado.PadraoEmbarcador) {
        _resumoChamado.Notificado.visible(true);
        if (_chamado.Notificado.val()) {
            _resumoChamado.Notificado.val(Localization.Resources.Chamado.ChamadoOcorrencia.Sim);
        } else {
            _resumoChamado.Notificado.val(Localization.Resources.Chamado.ChamadoOcorrencia.Nao);
        }
    }

    if (dados.DataRegistroMotorista != "")
        _resumoChamado.DataRegistroMotorista.visible(true);

    if (_chamado.VeiculoCarregado.val()) {
        _resumoChamado.VeiculoCarregado.val(Localization.Resources.Chamado.ChamadoOcorrencia.Sim);
    } else {
        _resumoChamado.VeiculoCarregado.val(Localization.Resources.Chamado.ChamadoOcorrencia.Nao);
    }

    if (dados.Numero == 0)
        _resumoChamado.Numero.visible(false);

    _notasFiscaisCompletas = _resumoChamado.NotasFiscais.val();
    _notasFiscaisResumidas = _resumoChamado.NotasFiscais.val().split(",", 120);
    AlterarValorNotasFiscaisResumoChamado();
}

function LimparResumoChamado() {
    LimparCampos(_resumoChamado);
    _resumoChamado.Carga.visible(false);
    _resumoChamado.Numero.visible(true);
    _resumoChamado.Atraso.text(Localization.Resources.Chamado.ChamadoOcorrencia.DiasAtraso);
}

function AlterarValorNotasFiscaisResumoChamado() {
    if (_notasFiscaisCompletas.split(",").length <= 120) {
        _resumoChamado.VerMais.val("");
        return;
    }
    if (_expandirNotasFiscais) {
        _resumoChamado.NotasFiscais.val(_notasFiscaisCompletas);
        _resumoChamado.VerMais.val(" - Ver Menos");
        _expandirNotasFiscais = false;
    } else {
        _resumoChamado.NotasFiscais.val(_notasFiscaisResumidas);
        _resumoChamado.VerMais.val(" ...Ver Todas");
        _expandirNotasFiscais = true;
    }
}