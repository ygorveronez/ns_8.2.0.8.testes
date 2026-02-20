/// <reference path="ChamadoTMS.js" />
/// <reference path="Anexos.js" />
/// <reference path="AberturaCustoAdicional.js" />
/// <reference path="AberturaChapa.js" />
/// <reference path="AberturaFormaPagamento.js" />
/// <reference path="../../Enumeradores/EnumSituacaoChamadoTMS.js" />
/// <reference path="../../Enumeradores/EnumFormaCobrancaChamado.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _abertura;
var _CRUDAbertura;
var _gridSelecaoConhecimentos;

var Abertura = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Numero = PropertyEntity({ text: "Número: ", getType: typesKnockout.int, enable: ko.observable(false) });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, val: ko.observable(""), def: "", text: "*Carga:", issue: 195, enable: ko.observable(true), idBtnSearch: guid() });
    this.MotivoChamado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, val: ko.observable(""), def: "", text: "*Motivo Chamado:", issue: 926, enable: ko.observable(true), idBtnSearch: guid() });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, val: ko.observable(""), def: "", text: "*Motorista:", enable: ko.observable(true), idBtnSearch: guid() });
    this.Anexo = PropertyEntity({ eventClick: gerenciarAnexosChamadosClick, type: types.event, text: "Anexos", visible: ko.observable(true), enable: ko.observable(true) });
    this.Observacao = PropertyEntity({ val: ko.observable(""), def: "", text: "Observação:", issue: 593, enable: ko.observable(true) });
    this.NumeroOrdemColeta = PropertyEntity({ val: ko.observable(""), def: "", text: "Nº Ordem Coleta:", enable: ko.observable(true) });
    this.CelularMotorista = PropertyEntity({ getType: typesKnockout.phone, val: ko.observable(""), def: "", text: "Nº Celular Motorista (WhatsApp):", enable: ko.observable(true) });

    this.QuantidadeFormaCobranca = PropertyEntity({ text: ko.observable("*Qtd. Forma Cobrança:"), def: "", val: ko.observable(""), getType: typesKnockout.decimal, maxlength: 10, configDecimal: { precision: 2, allowZero: false, allowNegative: false }, enable: ko.observable(true), required: ko.observable(true), visible: ko.observable(true) });
    this.ValorUnitario = PropertyEntity({ text: ko.observable("*Valor Unitário:"), def: "", val: ko.observable(""), getType: typesKnockout.decimal, maxlength: 10, configDecimal: { precision: 2, allowZero: false, allowNegative: false }, enable: ko.observable(true), required: ko.observable(true), visible: ko.observable(true) });
    this.ValorTotal = PropertyEntity({ text: ko.observable("*Valor Total:"), def: "0,00", val: ko.observable("0,00"), getType: typesKnockout.decimal, maxlength: 10, configDecimal: { precision: 2, allowZero: false, allowNegative: false }, enable: ko.observable(false), required: ko.observable(false), visible: ko.observable(true) });

    this.FormaCobranca = PropertyEntity({ val: ko.observable(EnumFormaCobrancaChamado.Caixa), options: EnumFormaCobrancaChamado.obterOpcoes(), enable: ko.observable(true), text: "*Forma Cobrança: ", def: EnumFormaCobrancaChamado.Caixa, visible: ko.observable(true) });

    this.Carga.codEntity.subscribe(function () {
        if (_abertura.Codigo.val() > 0)
            BuscarCTesDaCarga();
    });

    this.Carga.val.subscribe(function () {
        if (_abertura.Codigo.val() == 0) {
            _abertura.Carga.enable(false);
            BuscarCTesDaCarga();
        }
    });

    this.FormaCobranca.val.subscribe(function (valor) {
        if (valor === EnumFormaCobrancaChamado.ValorFixo) {
            _abertura.QuantidadeFormaCobranca.enable(false);
            _abertura.QuantidadeFormaCobranca.val("1,00");
        } else {
            _abertura.QuantidadeFormaCobranca.enable(true);
            _abertura.QuantidadeFormaCobranca.val("");
        }
    });

    this.QuantidadeFormaCobranca.val.subscribe(function () {
        CalcularTotalFormaCobranca();
    });

    this.ValorUnitario.val.subscribe(function () {
        CalcularTotalFormaCobranca();
    });

    this.GridConhecimentos = PropertyEntity({ idGrid: guid(), enable: ko.observable(true) });
    this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar Todos", visible: ko.observable(true), enable: ko.observable(true) });
    this.ListaFaturamento = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ListaNaoSelecionados = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.TotalizadorValorSelecionado = PropertyEntity({ text: "Total Valor: ", getType: typesKnockout.string, def: "0,00", val: ko.observable("0,00"), enable: ko.observable(true), visible: true });
    this.TotalizadorPesoSelecionado = PropertyEntity({ text: "Total Peso: ", getType: typesKnockout.string, def: "0,00", val: ko.observable("0,00"), enable: ko.observable(true), visible: true });
    this.TotalizadorCaixasSelecionado = PropertyEntity({ text: "Total Caixas: ", getType: typesKnockout.string, def: "0,00", val: ko.observable("0,00"), enable: ko.observable(true), visible: true });

    this.CustosAdicionais = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.Chapas = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });

    this.FormaPagamento = PropertyEntity({ val: ko.observable(new Object), def: ko.observable(new Object), getType: typesKnockout.dynamic });
    this.ListaSelecaoConhecimentos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
};

var CRUDAbertura = function () {
    this.Abrir = PropertyEntity({ eventClick: abrirClick, type: types.event, text: "Abrir", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar Etapa 1", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadAbertura() {
    _abertura = new Abertura();
    KoBindings(_abertura, "tabAbertura");

    HeaderAuditoria("ChamadoTMS", _abertura);

    _CRUDAbertura = new CRUDAbertura();
    KoBindings(_CRUDAbertura, "footerAbertura");

    new BuscarCargas(_abertura.Carga);
    new BuscarMotivoChamado(_abertura.MotivoChamado);
    new BuscarMotoristas(_abertura.Motorista);

    MontarGridConhecimentos();
    loadAnexosChamados();
    loadAberturaCustoAdicional();
    loadAberturaFormaPagamento();
    loadAberturaChapa();
}

function abrirClick(e, sender) {
    if (validaPreencheCampos()) {
        Salvar(_abertura, "ChamadoTMS/Adicionar", function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Chamado aberto com sucesso");
                    _chamadoTMS.Codigo.val(arg.Data);

                    EnviarArquivosAnexadosChamado(function () {
                        BuscarChamadoTMSPorCodigo(arg.Data);
                    });

                    recarregarGridPesquisaChamadosTMS();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender);
    }
}

function atualizarClick(e, sender) {
    if (validaPreencheCampos()) {
        Salvar(_abertura, "ChamadoTMS/Atualizar", function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Chamado atualizado com sucesso");
                    _chamadoTMS.Codigo.val(arg.Data);
                    recarregarGridPesquisaChamadosTMS();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender);
    }
}

function validaPreencheCampos() {
    var valido = ValidarCamposObrigatorios(_abertura);
    if (!valido) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
        return false;
    } else if (!validarCamposObrigatoriosAberturaFormaPagamento()) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por Favor, Informe todos os campos obrigatórios da aba Forma de Pagamento");
        return false;
    }

    _abertura.FormaPagamento.val(JSON.stringify(RetornarObjetoPesquisa(_aberturaFormaPagamento)));

    var ctesSelecionados = _gridSelecaoConhecimentos.ObterMultiplosSelecionados();
    _abertura.ListaSelecaoConhecimentos.val(JSON.stringify(ctesSelecionados));

    return true;
}

//*******MÉTODOS*******

function EditarAbertura(data) {
    _abertura.Codigo.val(data.Codigo);

    PreencherObjetoKnout(_abertura, { Data: data.Abertura });
    PreencherObjetoKnout(_aberturaFormaPagamento, { Data: data.FormaPagamento });

    if (_chamadoTMS.Situacao.val() === EnumSituacaoChamadoTMS.Aberto)
        ControleCamposAbertura(true);
    else
        ControleCamposAbertura(false);

    _abertura.Carga.enable(false);
    if (_abertura.FormaCobranca.val() === EnumFormaCobrancaChamado.ValorFixo)
        _abertura.QuantidadeFormaCobranca.enable(false);

    CarregarAnexosChamados(data);
    recarregarGridCustosAdicionais();
    recarregarGridChapas();
}

function ControleCamposAbertura(status) {
    SetarEnableCamposKnockout(_abertura, status);

    _abertura.Numero.enable(false);
    _abertura.ValorTotal.enable(false);
    _abertura.Anexo.enable(true);

    if (_chamadoTMS.Codigo.val() == 0)
        _CRUDAbertura.Abrir.visible(true);
    else {
        _CRUDAbertura.Abrir.visible(false);
        _CRUDAbertura.Atualizar.visible(status);
    }

    ControleCamposAberturaCustoAdicional(status);
    ControleCamposAberturaFormaPagamento(status);
    ControleCamposAberturaChapa(status);
}

function LimparCamposAbertura() {
    LimparCampos(_abertura);
    _abertura.Anexo.visible(true);
    _CRUDAbertura.Atualizar.visible(false);
    ControleCamposAbertura(true);

    limparOcorrenciaAnexosChamados();
    limparCamposAberturaCustoAdicional();
    limparCamposAberturaFormaPagamento();
    limparCamposAberturaChapa();
}

function CalcularTotalFormaCobranca() {
    var quantidade = Globalize.parseFloat(_abertura.QuantidadeFormaCobranca.val());
    var valorUnitario = Globalize.parseFloat(_abertura.ValorUnitario.val());

    if (quantidade > 0 && valorUnitario > 0) {
        var valorTotal = quantidade * valorUnitario;
        _abertura.ValorTotal.val(Globalize.format(valorTotal, "n2"));
    } else
        _abertura.ValorTotal.val(Globalize.format(0, "n2"));
}

function MontarGridConhecimentos() {
    var downloadDACTE = { descricao: "Download DACTE", id: guid(), metodo: DownloadDACTEClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 15, opcoes: [downloadDACTE] };

    var somenteLeitura = false;
    var multiplaescolha = {
        basicGrid: null,
        callbackSelecionado: function () {
            AtualizarTotalizadorSelecionado();
        },
        callbackNaoSelecionado: function () {
            AtualizarTotalizadorSelecionado();
        },
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _abertura.SelecionarTodos,
        somenteLeitura: somenteLeitura
    }
    var editarColuna = { permite: true, callback: null, atualizarRow: true };

    _gridSelecaoConhecimentos = new GridView(_abertura.GridConhecimentos.idGrid, "ChamadoTMS/PesquisaConhecimentosCarga", _abertura, menuOpcoes, null, 10, null, null, null, multiplaescolha, null, editarColuna);
    BuscarCTesDaCarga();
}

function BuscarCTesDaCarga() {
    if (_abertura.Codigo.val() > 0) {
        _abertura.SelecionarTodos.val(false);
        _gridSelecaoConhecimentos.CarregarGrid(function () { setTimeout(_abertura.SelecionarTodos.eventClick, 100); });
    } else
        _gridSelecaoConhecimentos.CarregarGrid();
}

function AtualizarTotalizadorSelecionado() {

    var ctesSelecionados = _gridSelecaoConhecimentos.ObterMultiplosSelecionados();
    var valorDescargasSelecionado = 0.0;
    var pesoSelecionado = 0.0;
    var caixaSelecionado = 0.0;

    if (ctesSelecionados.length > 0) {
        $.each(ctesSelecionados, function (i, cte) {
            valorDescargasSelecionado = valorDescargasSelecionado + Globalize.parseFloat(cte.ComponenteDescarga.toString()) + Globalize.parseFloat(cte.ValorDescarga.toString());
            pesoSelecionado = pesoSelecionado + Globalize.parseFloat(cte.Peso.toString());
            caixaSelecionado = caixaSelecionado + Globalize.parseFloat(cte.Caixas.toString());

            _abertura.TotalizadorValorSelecionado.val(Globalize.format(valorDescargasSelecionado, "n2"));
            _abertura.TotalizadorPesoSelecionado.val(Globalize.format(pesoSelecionado, "n3"));
            _abertura.TotalizadorCaixasSelecionado.val(Globalize.format(caixaSelecionado, "n2"));
        });

    } else {
        _abertura.TotalizadorValorSelecionado.val("0,00");
        _abertura.TotalizadorPesoSelecionado.val("0,000");
        _abertura.TotalizadorCaixasSelecionado.val("0,00");
    }
}

function DownloadDACTEClick(conhecimentoEletronicoGrid) {
    var data = { CodigoCTe: conhecimentoEletronicoGrid.CodigoCTe, CodigoEmpresa: 0 };
    executarDownload("CargaCTe/DownloadDacte", data);
}