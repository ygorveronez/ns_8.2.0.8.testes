var _acrescimoDescontoDocumentoNegociacaoBaixaTituloReceber, _gridAcrescimoDescontoDocumentoNegociacaoBaixaTituloReceber;

var AcrescimoDescontoDocumentoNegociacaoBaixaTituloReceber = function () {
    this.Grid = PropertyEntity({ type: types.local, idGrid: guid() });
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Documento = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Valor = PropertyEntity({ getType: typesKnockout.decimal, text: "*Valor:", val: ko.observable(""), def: "", required: true, enable: ko.observable(true), cssClass: ko.observable("col-12 col-md-4") });
    this.ValorMoeda = PropertyEntity({ getType: typesKnockout.decimal, text: "*Valor em Moeda:", val: ko.observable(""), def: "", required: false, visible: ko.observable(false) });
    this.Justificativa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Justificativa:", idBtnSearch: guid(), required: true, issue: 382, enable: ko.observable(true), cssClass: ko.observable("col-12 col-md-8") });
    this.Observacao = PropertyEntity({ text: "Observação:", val: ko.observable(""), def: "", required: false, maxlength: 500 });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarAcrescimoDescontoDocumentoNegociacaoBaixaTituloReceberClick, type: types.event, text: "Adicionar", icon: "fal fa-plus", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarAcrescimoDescontoDocumentoNegociacaoBaixaTituloReceberClick, type: types.event, text: "Atualizar", icon: "fal fa-save", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirAcrescimoDescontoDocumentoNegociacaoBaixaTituloReceberClick, type: types.event, text: "Excluir", icon: "fal fa-trash", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarAcrescimoDescontoDocumentoNegociacaoBaixaTituloReceberClick, type: types.event, text: "Cancelar", icon: "fal fa-undo", visible: ko.observable(true) });
    this.Fechar = PropertyEntity({ eventClick: FecharAcrescimoDescontoDocumentoNegociacaoBaixaTituloReceberFatura, type: types.event, text: "Fechar", icon: "fal fa-window-close", visible: ko.observable(true) });

    this.ValorMoeda.val.subscribe(function (novoValor) {
        _acrescimoDescontoDocumentoNegociacaoBaixaTituloReceber.Valor.val(Globalize.format(ObterValorEmRealBaixaTituloReceber(novoValor), "n2"));
    });
}

////*******EVENTOS*******

function LoadAcrescimoDescontoDocumentoNegociacaoBaixaTituloReceber() {

    _acrescimoDescontoDocumentoNegociacaoBaixaTituloReceber = new AcrescimoDescontoDocumentoNegociacaoBaixaTituloReceber();
    KoBindings(_acrescimoDescontoDocumentoNegociacaoBaixaTituloReceber, "knockoutAcrescimoDescontoDocumentoNegociacaoBaixaTituloReceber");

    new BuscarJustificativas(_acrescimoDescontoDocumentoNegociacaoBaixaTituloReceber.Justificativa, null, null, [EnumTipoFinalidadeJustificativa.TitulosReceber, EnumTipoFinalidadeJustificativa.Todas]);
}

function AdicionarAcrescimoDescontoDocumentoNegociacaoBaixaTituloReceberClick(e, sender) {
    Salvar(_acrescimoDescontoDocumentoNegociacaoBaixaTituloReceber, "BaixaTituloReceberNovoAgrupadoDocumentoAcrescimoDesconto/Adicionar", function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "Valor adicionado com sucesso!");

                _gridAcrescimoDescontoDocumentoNegociacaoBaixaTituloReceber.CarregarGrid();
                _gridDocumentosNegociacaoBaixaTituloReceber.CarregarGrid();

                LimparCamposAcrescimoDescontoDocumentoNegociacaoBaixaTituloReceber();

                PreencherObjetoKnout(_negociacaoBaixa, { Data: r.Data.Negociacao });
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function AtualizarAcrescimoDescontoDocumentoNegociacaoBaixaTituloReceberClick(e, sender) {
    Salvar(_acrescimoDescontoDocumentoNegociacaoBaixaTituloReceber, "BaixaTituloReceberNovoAgrupadoDocumentoAcrescimoDesconto/Atualizar", function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "Valor atualizado com sucesso!");

                _gridAcrescimoDescontoDocumentoNegociacaoBaixaTituloReceber.CarregarGrid();
                _gridDocumentosNegociacaoBaixaTituloReceber.CarregarGrid();

                LimparCamposAcrescimoDescontoDocumentoNegociacaoBaixaTituloReceber();

                PreencherObjetoKnout(_negociacaoBaixa, { Data: r.Data.Negociacao });
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function ExcluirAcrescimoDescontoDocumentoNegociacaoBaixaTituloReceberClick(e, sender) {
    exibirConfirmacao("Atenção!", "Deseja realmente excluir o valor de " + _acrescimoDescontoDocumentoNegociacaoBaixaTituloReceber.Valor.val() + "?", function () {
        ExcluirPorCodigo(_acrescimoDescontoDocumentoNegociacaoBaixaTituloReceber, "BaixaTituloReceberNovoAgrupadoDocumentoAcrescimoDesconto/Excluir", function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "Valor excluído com sucesso!");

                    _gridAcrescimoDescontoDocumentoNegociacaoBaixaTituloReceber.CarregarGrid();
                    _gridDocumentosNegociacaoBaixaTituloReceber.CarregarGrid();

                    LimparCamposAcrescimoDescontoDocumentoNegociacaoBaixaTituloReceber();

                    PreencherObjetoKnout(_negociacaoBaixa, { Data: r.Data.Negociacao });
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

function CancelarAcrescimoDescontoDocumentoNegociacaoBaixaTituloReceberClick(e, sender) {
    LimparCamposAcrescimoDescontoDocumentoNegociacaoBaixaTituloReceber();
}

function EditarAcrescimoDescontoDocumentoNegociacaoBaixaTituloReceberClick(dadosGrid) {
    LimparCamposAcrescimoDescontoDocumentoNegociacaoBaixaTituloReceber();
    _acrescimoDescontoDocumentoNegociacaoBaixaTituloReceber.Codigo.val(dadosGrid.Codigo);
    BuscarPorCodigo(_acrescimoDescontoDocumentoNegociacaoBaixaTituloReceber, "BaixaTituloReceberNovoAgrupadoDocumentoAcrescimoDesconto/BuscarPorCodigo", function (r) {
        if (r.Success) {
            if (r.Data) {
                _acrescimoDescontoDocumentoNegociacaoBaixaTituloReceber.Justificativa.enable(false);
                _acrescimoDescontoDocumentoNegociacaoBaixaTituloReceber.Adicionar.visible(false);
                _acrescimoDescontoDocumentoNegociacaoBaixaTituloReceber.Atualizar.visible(true);
                _acrescimoDescontoDocumentoNegociacaoBaixaTituloReceber.Excluir.visible(true);
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

////*******METODOS*******

function ObterValorEmRealBaixaTituloReceber(valorEmMoeda) {
    var valor = 0;
    var cotacao = 0;

    if (_negociacaoBaixa.Moeda.val() != EnumMoedaCotacaoBancoCentral.Real) {
        valor = Globalize.parseFloat(valorEmMoeda);
        cotacao = Globalize.parseFloat(_negociacaoBaixa.ValorCotacaoMoeda.val());

        if (isNaN(valor))
            valor = 0;
        if (isNaN(cotacao))
            cotacao = 0;
    }

    return valor * cotacao;
}

function CarregarGridAcrescimoDescontoDocumentoNegociacaoBaixaTituloReceber() {
    var permiteAlterar = false;

    if (_baixaTituloReceber.Etapa.val() == EnumEtapasBaixaTituloReceber.EmNegociacao || _baixaTituloReceber.Etapa.val() == EnumEtapasBaixaTituloReceber.Iniciada)
        permiteAlterar = true;

    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarAcrescimoDescontoDocumentoNegociacaoBaixaTituloReceberClick, tamanho: "15", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [editar] };

    if (!permiteAlterar)
        menuOpcoes = null;

    if (_gridAcrescimoDescontoDocumentoNegociacaoBaixaTituloReceber != null)
        _gridAcrescimoDescontoDocumentoNegociacaoBaixaTituloReceber.Destroy();

    _gridAcrescimoDescontoDocumentoNegociacaoBaixaTituloReceber = new GridView(_acrescimoDescontoDocumentoNegociacaoBaixaTituloReceber.Grid.idGrid, "BaixaTituloReceberNovoAgrupadoDocumentoAcrescimoDesconto/Pesquisa", _acrescimoDescontoDocumentoNegociacaoBaixaTituloReceber, menuOpcoes, { column: 0, dir: orderDir.asc }, 5, null, false);
    _gridAcrescimoDescontoDocumentoNegociacaoBaixaTituloReceber.CarregarGrid();
}

function LimparCamposAcrescimoDescontoDocumentoNegociacaoBaixaTituloReceber() {
    _acrescimoDescontoDocumentoNegociacaoBaixaTituloReceber.Justificativa.enable(true);
    _acrescimoDescontoDocumentoNegociacaoBaixaTituloReceber.Justificativa.val("");
    _acrescimoDescontoDocumentoNegociacaoBaixaTituloReceber.Justificativa.codEntity(0);
    _acrescimoDescontoDocumentoNegociacaoBaixaTituloReceber.Valor.val("");
    _acrescimoDescontoDocumentoNegociacaoBaixaTituloReceber.ValorMoeda.val("");
    _acrescimoDescontoDocumentoNegociacaoBaixaTituloReceber.Observacao.val("");

    _acrescimoDescontoDocumentoNegociacaoBaixaTituloReceber.Adicionar.visible(true);
    _acrescimoDescontoDocumentoNegociacaoBaixaTituloReceber.Atualizar.visible(false);
    _acrescimoDescontoDocumentoNegociacaoBaixaTituloReceber.Excluir.visible(false);
}

function AbrirTelaAcrescimoDescontoDocumentoNegociacaoBaixaTituloReceber(dadosGrid) {
    LimparCamposAcrescimoDescontoDocumentoNegociacaoBaixaTituloReceber();

    _acrescimoDescontoDocumentoNegociacaoBaixaTituloReceber.Documento.val(dadosGrid.Codigo);

    if (_negociacaoBaixa.Moeda.val() != EnumMoedaCotacaoBancoCentral.Real) {
        _acrescimoDescontoDocumentoNegociacaoBaixaTituloReceber.ValorMoeda.visible(true);
        _acrescimoDescontoDocumentoNegociacaoBaixaTituloReceber.ValorMoeda.required = true;
        _acrescimoDescontoDocumentoNegociacaoBaixaTituloReceber.Valor.enable(false);

        _acrescimoDescontoDocumentoNegociacaoBaixaTituloReceber.Justificativa.cssClass("col-12 col-md-6");
        _acrescimoDescontoDocumentoNegociacaoBaixaTituloReceber.Valor.cssClass("col-12 col-md-3");
    } else {
        _acrescimoDescontoDocumentoNegociacaoBaixaTituloReceber.ValorMoeda.visible(false);
        _acrescimoDescontoDocumentoNegociacaoBaixaTituloReceber.ValorMoeda.required = false;
        _acrescimoDescontoDocumentoNegociacaoBaixaTituloReceber.Valor.enable(true);

        _acrescimoDescontoDocumentoNegociacaoBaixaTituloReceber.Justificativa.cssClass("col-12 col-md-8");
        _acrescimoDescontoDocumentoNegociacaoBaixaTituloReceber.Valor.cssClass("col-12 col-md-4");
    }

    CarregarGridAcrescimoDescontoDocumentoNegociacaoBaixaTituloReceber();
    Global.abrirModal("knockoutAcrescimoDescontoDocumentoNegociacaoBaixaTituloReceber");
}

function FecharAcrescimoDescontoDocumentoNegociacaoBaixaTituloReceberFatura() {
    Global.fecharModal("knockoutAcrescimoDescontoDocumentoNegociacaoBaixaTituloReceber");
    LimparCamposAcrescimoDescontoDocumentoNegociacaoBaixaTituloReceber();
}