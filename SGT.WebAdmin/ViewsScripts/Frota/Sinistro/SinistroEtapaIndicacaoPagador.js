/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../Enumeradores/EnumIndicadorPagador.js" />
/// <reference path="../../Enumeradores/EnumFormaTitulo.js" />
/// <reference path="../../Consultas/TipoMovimento.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/FolhaInformacao.js" />
/// <reference path="SinistroEtapaIndicacaoPagadorAnexo.js" />
/// <reference path="SinistroEtapaIndicacaoPagadorParcelamento.js" />
/// <reference path="SinistroEtapaIndicacaoPagadorNota.js" />
/// <reference path="SinistroEtapaDados.js" />

var _etapaIndicadorPagadorSinistro;
var _CRUDIndicacaoPagadorSinistro;

var IndicacaoPagadorSinistro = function () {
    this.Sinistro = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.BloqueiaEdicaoAoVoltarEtapa = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.IndicadorPagador = PropertyEntity({ text: "*Indicação do Pagador:", options: EnumIndicadorPagador.obterOpcoes(), val: ko.observable(EnumIndicadorPagador.SeguroProprio), def: EnumIndicadorPagador.SeguroProprio, required: true, enable: ko.observable(true) });
    this.IndicadorPagador.val.subscribe((valor) => {
        _etapaIndicadorPagadorSinistro.LinhaDigitavel.visible(valor === EnumIndicadorPagador.Empresa || valor === EnumIndicadorPagador.EmpresaMotorista);
        _etapaIndicadorPagadorSinistro.CodigoDeBarras.visible(valor === EnumIndicadorPagador.Empresa || valor === EnumIndicadorPagador.EmpresaMotorista);

        $("#liIndicacaoPagadorGeracaoTitulo").hide();
        $("#liIndicacaoPagadorFolhaLancamento").hide();
        $("#knockoutFluxoSinistroIndicacaoPagadorParcelamento").hide();
        $("#knockoutOperadorIndicacaoPagadorSinistroNota").hide();

        if (valor === EnumIndicadorPagador.Empresa || valor === EnumIndicadorPagador.EmpresaMotorista || valor === EnumIndicadorPagador.Terceiro || valor === EnumIndicadorPagador.SeguroTerceiroReembolso) {
            $("#liIndicacaoPagadorGeracaoTitulo").show();
            $("#knockoutFluxoSinistroIndicacaoPagadorParcelamento").show();
        } else if (valor === EnumIndicadorPagador.MotoristaFolha)
            $("#liIndicacaoPagadorFolhaLancamento").show();
        else if (valor === EnumIndicadorPagador.EmpresaNota)
            $("#knockoutOperadorIndicacaoPagadorSinistroNota").show();
    });

    //Geração de Título
    this.TipoMovimento = PropertyEntity({ text: "*Tipo de Movimento:", type: types.entity, codEntity: ko.observable(0), val: ko.observable(""), required: ko.observable(false), idBtnSearch: guid(), enable: ko.observable(true) });
    this.Pessoa = PropertyEntity({ text: "*Pessoa:", type: types.entity, codEntity: ko.observable(0), val: ko.observable(""), required: ko.observable(false), idBtnSearch: guid(), enable: ko.observable(true) });
    this.DataEmissao = PropertyEntity({ getType: typesKnockout.date, val: ko.observable(""), text: "*Data Emissão:", required: ko.observable(false), enable: ko.observable(true) });
    this.DataVencimento = PropertyEntity({ getType: typesKnockout.date, val: ko.observable(""), text: "*Data Vencimento:", required: ko.observable(false), enable: ko.observable(true) });
    this.ValorOriginal = PropertyEntity({ getType: typesKnockout.decimal, val: ko.observable(""), def: "", text: "*Valor Original:", required: ko.observable(false), enable: ko.observable(true) });
    this.TipoDocumento = PropertyEntity({ val: ko.observable(""), text: "Tipo do Documento:", visible: ko.observable(true), maxlength: 100, enable: ko.observable(true) });
    this.NumeroDocumento = PropertyEntity({ val: ko.observable(""), text: "Número do Documento:", visible: ko.observable(true), maxlength: 100, enable: ko.observable(true) });
    this.LinhaDigitavel = PropertyEntity({ val: ko.observable(""), text: "Linha Digitável:", visible: ko.observable(false), maxlength: 100, enable: ko.observable(true) });
    this.CodigoDeBarras = PropertyEntity({ val: ko.observable(""), text: "Código de Barras:", visible: ko.observable(false), maxlength: 100, enable: ko.observable(true) });
    this.FormaTitulo = PropertyEntity({ text: "*Forma do Título:", options: EnumFormaTitulo.obterOpcoes(), val: ko.observable(EnumFormaTitulo.Outros), def: EnumFormaTitulo.Outros, required: false, enable: ko.observable(true) });
    this.Observacao = PropertyEntity({ val: ko.observable(""), text: "Observação:", maxlength: 300, enable: ko.observable(true) });

    //Folha de Lançamento
    this.Descricao = PropertyEntity({ text: "*Descrição: ", maxlength: 1000, required: ko.observable(false), enable: ko.observable(true) });
    this.NumeroEvento = PropertyEntity({ text: "*Número Evento: ", getType: typesKnockout.int, required: ko.observable(false), enable: ko.observable(true) });
    this.NumeroContrato = PropertyEntity({ text: "Número Contrato: ", getType: typesKnockout.int, required: ko.observable(false), enable: ko.observable(true) });
    this.DataInicial = PropertyEntity({ text: "*Data Inicial:", getType: typesKnockout.date, required: ko.observable(false), enable: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: "*Data Final:", getType: typesKnockout.date, required: ko.observable(false), enable: ko.observable(true) });
    this.Base = PropertyEntity({ text: "*Valor Base:", getType: typesKnockout.decimal, maxlength: 18, val: ko.observable(""), def: "", configDecimal: { precision: 2, allowZero: false, allowNegative: false }, required: ko.observable(false), enable: ko.observable(true) });
    this.Referencia = PropertyEntity({ text: "Valor Referência:", getType: typesKnockout.decimal, maxlength: 18, val: ko.observable(""), def: "", configDecimal: { precision: 2, allowZero: false, allowNegative: false }, required: ko.observable(false), enable: ko.observable(true) });
    this.Valor = PropertyEntity({ text: "*Valor:", getType: typesKnockout.decimal, maxlength: 18, val: ko.observable(""), def: "", configDecimal: { precision: 2, allowZero: false, allowNegative: false }, required: ko.observable(false), enable: ko.observable(true) });
    this.DataCompetencia = PropertyEntity({ text: ko.observable("Data Competência:"), getType: typesKnockout.date, required: ko.observable(false), enable: ko.observable(true) });
    this.Funcionario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Funcionário:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true) });
    this.FolhaInformacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Informação da Folha:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(true) });

    //Listas
    this.Parcelas = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.Notas = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
};

var CRUDIndicacaoPagadorSinistro = function () {
    this.EnviarParaAcompanhamento = PropertyEntity({ eventClick: enviarParaAcompanhamentoClick, type: types.event, text: "Enviar para Acompanhamento", visible: ko.observable(true) });
    this.AvancarEtapa = PropertyEntity({ eventClick: avancarEtapaIndicacaoPagadorClick, type: types.event, text: "Avançar", visible: ko.observable(false) });
    this.VoltarEtapa = PropertyEntity({ eventClick: voltarEtapaIndicacaoPagadorClick, type: types.event, text: "Voltar Etapa", visible: ko.observable(false) });
};

function loadEtapaIndicacaoPagadorSinistro() {
    _etapaIndicadorPagadorSinistro = new IndicacaoPagadorSinistro();
    KoBindings(_etapaIndicadorPagadorSinistro, "knockoutFluxoSinistroIndicacaoPagador");

    _CRUDIndicacaoPagadorSinistro = new CRUDIndicacaoPagadorSinistro();
    KoBindings(_CRUDIndicacaoPagadorSinistro, "knockoutCRUDFluxoSinistroIndicacaoPagador");

    new BuscarTipoMovimento(_etapaIndicadorPagadorSinistro.TipoMovimento);
    new BuscarClientes(_etapaIndicadorPagadorSinistro.Pessoa);
    new BuscarFuncionario(_etapaIndicadorPagadorSinistro.Funcionario);
    new BuscarFolhaInformacao(_etapaIndicadorPagadorSinistro.FolhaInformacao, retornoFolhaInformacao);

    if (_CONFIGURACAO_TMS.GerarTituloFolhaPagamento)
        _etapaIndicadorPagadorSinistro.DataCompetencia.text("*Data Competência:");

    loadEtapaIndicacaoPagadorParcelamento();
    loadIndicacaoPagadorSinistroNota();
    loadEtapaIndicacaoPagadorAnexo();
}

function retornoFolhaInformacao(data) {
    _etapaIndicadorPagadorSinistro.FolhaInformacao.val(data.Descricao);
    _etapaIndicadorPagadorSinistro.FolhaInformacao.codEntity(data.Codigo);
    _etapaIndicadorPagadorSinistro.NumeroEvento.val(data.CodigoIntegracao);
}

function enviarParaAcompanhamentoClick() {
    if (!validarRegrasSalvarIndicacaoPagador())
        return;

    _etapaIndicadorPagadorSinistro.Notas.val(JSON.stringify(_etapaIndicadorPagadorNota.DocumentoEntrada.basicTable.BuscarRegistros()));

    exibirConfirmacao("Atenção!", "Deseja realmente avançar a etapa do indicador?", function () {
        _etapaIndicadorPagadorSinistro.Sinistro.val(_etapaDadosSinistro.Codigo.val());
        Salvar(_etapaIndicadorPagadorSinistro, "SinistroIndicador/EnviarParaAcompanhamento", function (r) {
            if (r.Success) {
                if (r.Data) {
                    limparFluxoSinistro();
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Etapa avançada com sucesso!");                                        
                    CarregarDadosSinistro(r.Data.Codigo);
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

function avancarEtapaIndicacaoPagadorClick() {
    exibirConfirmacao("Atenção!", "Deseja realmente avançar a etapa de Indicação Pagamento?", function () {
        executarReST("Sinistro/AvancarEtapa", { Codigo: _etapaDadosSinistro.Codigo.val(), Etapa: EnumEtapaSinistro.Acompanhamento }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    limparFluxoSinistro();                    
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Fluxo avançado com sucesso.");
                    CarregarDadosSinistro(retorno.Data.Codigo);
                } else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

function voltarEtapaIndicacaoPagadorClick() {
    exibirConfirmacao("Atenção!", "Deseja realmente voltar para a etapa Manutenção?", function () {
        executarReST("Sinistro/VoltarEtapa", { Codigo: _etapaDadosSinistro.Codigo.val(), Etapa: EnumEtapaSinistro.Manutencao }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Fluxo voltado com sucesso.");

                    recarregarGridSinistro();
                    editarSinistroClick({ Codigo: _etapaDadosSinistro.Codigo.val() });
                } else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}

function validarRegrasSalvarIndicacaoPagador() {
    var indicadorPagador = _etapaIndicadorPagadorSinistro.IndicadorPagador.val();

    if (indicadorPagador === EnumIndicadorPagador.SeguroProprio || indicadorPagador === EnumIndicadorPagador.SeguroTerceiro || indicadorPagador === EnumIndicadorPagador.EmpresaNota)
        return true;

    var geracaoTitulo = indicadorPagador === EnumIndicadorPagador.Empresa || indicadorPagador === EnumIndicadorPagador.EmpresaMotorista || indicadorPagador === EnumIndicadorPagador.Terceiro || indicadorPagador === EnumIndicadorPagador.SeguroTerceiroReembolso;

    _etapaIndicadorPagadorSinistro.TipoMovimento.required(geracaoTitulo);
    _etapaIndicadorPagadorSinistro.Pessoa.required(geracaoTitulo);
    _etapaIndicadorPagadorSinistro.DataEmissao.required(geracaoTitulo);
    _etapaIndicadorPagadorSinistro.DataVencimento.required(geracaoTitulo);
    _etapaIndicadorPagadorSinistro.ValorOriginal.required(geracaoTitulo);

    var folha = indicadorPagador === EnumIndicadorPagador.MotoristaFolha;

    _etapaIndicadorPagadorSinistro.Descricao.required(folha);
    _etapaIndicadorPagadorSinistro.NumeroEvento.required(folha);
    _etapaIndicadorPagadorSinistro.DataInicial.required(folha);
    _etapaIndicadorPagadorSinistro.DataFinal.required(folha);
    _etapaIndicadorPagadorSinistro.Base.required(folha);
    _etapaIndicadorPagadorSinistro.Valor.required(folha);
    _etapaIndicadorPagadorSinistro.Funcionario.required(folha);
    _etapaIndicadorPagadorSinistro.FolhaInformacao.required(folha);
    _etapaIndicadorPagadorSinistro.DataCompetencia.required(folha && _CONFIGURACAO_TMS.GerarTituloFolhaPagamento);

    if (!ValidarCamposObrigatorios(_etapaIndicadorPagadorSinistro)) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Preencha os campos obrigatórios.");
        return false;
    }

    return true;
}

function preencherEtapaIndicacaoPagador(dados) {
    PreencherObjetoKnout(_etapaIndicadorPagadorSinistro, { Data: dados });

    recarregarGridIndicacaoPagadorParcela();
    recarregarGridIndicacaoPagadorNota();
    setarEtapaDesabilitadaIndicacaoPagadorAnexo(true);
    _indicacaoPagadorSinistroAnexo.Anexos.val(dados.Anexos);

    if (_etapaDadosSinistro.Etapa.val() === EnumEtapaSinistro.IndicacaoPagador && !_etapaIndicadorPagadorSinistro.BloqueiaEdicaoAoVoltarEtapa.val())
        return;

    SetarEnableCamposKnockout(_etapaIndicadorPagadorSinistro, false);
    setarEtapaDesabilitadaIndicacaoPagadorAnexo(false);
    _CRUDIndicacaoPagadorSinistro.EnviarParaAcompanhamento.visible(false);
    _CRUDIndicacaoPagadorSinistro.VoltarEtapa.visible(false);

    if (_etapaDadosSinistro.Etapa.val() === EnumEtapaSinistro.IndicacaoPagador && _etapaIndicadorPagadorSinistro.BloqueiaEdicaoAoVoltarEtapa.val()) {
        _CRUDIndicacaoPagadorSinistro.AvancarEtapa.visible(true);
        _CRUDIndicacaoPagadorSinistro.VoltarEtapa.visible(true);
    }

    bloquearCamposIndicadorPagadorParcela();
    bloquearCamposIndicadorPagadorNota();
}

function limparCamposSinistroEtapaIndicacaoPagador() {
    LimparCampos(_etapaIndicadorPagadorSinistro);

    limparCamposIndicacaoPagadorParcelamento();
    limparCamposIndicacaoPagadorSinistroNota();

    SetarEnableCamposKnockout(_etapaIndicadorPagadorSinistro, true);
    _CRUDIndicacaoPagadorSinistro.EnviarParaAcompanhamento.visible(true);
    _CRUDIndicacaoPagadorSinistro.AvancarEtapa.visible(false);
    _CRUDIndicacaoPagadorSinistro.VoltarEtapa.visible(true);

    $("#knockoutFluxoSinistroIndicacaoPagadorParcelamento").hide();
}