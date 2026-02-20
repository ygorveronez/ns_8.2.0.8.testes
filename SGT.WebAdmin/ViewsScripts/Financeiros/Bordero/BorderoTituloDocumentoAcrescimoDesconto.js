var _acrescimoDescontoBorderoTituloDocumento, _gridAcrescimoDescontoBorderoTituloDocumento;

var AcrescimoDescontoBorderoTituloDocumento = function () {
    this.Grid = PropertyEntity({ type: types.local, idGrid: guid() });
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Documento = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Valor = PropertyEntity({ getType: typesKnockout.decimal, text: "*Valor:", val: ko.observable(""), def: "", required: true });
    this.Justificativa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Justificativa:", idBtnSearch: guid(), required: true, issue: 382, enable: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação:", val: ko.observable(""), def: "", required: false, maxlength: 500 })

    this.Adicionar = PropertyEntity({ eventClick: AdicionarAcrescimoDescontoBorderoTituloDocumentoClick, type: types.event, text: "Adicionar", icon: "fal fa-plus", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarAcrescimoDescontoBorderoTituloDocumentoClick, type: types.event, text: "Atualizar", icon: "fa fa-save", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirAcrescimoDescontoBorderoTituloDocumentoClick, type: types.event, text: "Excluir", icon: "fa fa-close", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarAcrescimoDescontoBorderoTituloDocumentoClick, type: types.event, text: "Cancelar", icon: "fa fa-rotate-left", visible: ko.observable(true) });
    this.Fechar = PropertyEntity({ eventClick: FecharTelaAcrescimoDescontoBorderoTituloDocumento, type: types.event, text: "Fechar", icon: "fa fa-window-close", visible: ko.observable(true) });
}

////*******EVENTOS*******

function LoadAcrescimoDescontoBorderoTituloDocumento() {

    _acrescimoDescontoBorderoTituloDocumento = new AcrescimoDescontoBorderoTituloDocumento();
    KoBindings(_acrescimoDescontoBorderoTituloDocumento, "knockoutAcrescimoDescontoBorderoTituloDocumento");

    new BuscarJustificativas(_acrescimoDescontoBorderoTituloDocumento.Justificativa, null, null, [EnumTipoFinalidadeJustificativa.TitulosReceber, EnumTipoFinalidadeJustificativa.Todas]);

    CarregarGridAcrescimoDescontoBorderoTituloDocumento();
}

function AdicionarAcrescimoDescontoBorderoTituloDocumentoClick(e, sender) {
    Salvar(_acrescimoDescontoBorderoTituloDocumento, "BorderoTituloDocumentoAcrescimoDesconto/Adicionar", function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "Valor adicionado com sucesso!");
                PreecherCamposEdicaoBordero(r.Data);
                _gridAcrescimoDescontoBorderoTituloDocumento.CarregarGrid();
                _gridBorderoTituloDocumento.CarregarGrid();
                _gridBorderoTitulo.CarregarGrid();
                LimparCamposAcrescimoDescontoBorderoTituloDocumento();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function AtualizarAcrescimoDescontoBorderoTituloDocumentoClick(e, sender) {
    Salvar(_acrescimoDescontoBorderoTituloDocumento, "BorderoTituloDocumentoAcrescimoDesconto/Atualizar", function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "Valor atualizado com sucesso!");
                PreecherCamposEdicaoBordero(r.Data);
                _gridAcrescimoDescontoBorderoTituloDocumento.CarregarGrid();
                _gridBorderoTituloDocumento.CarregarGrid();
                _gridBorderoTitulo.CarregarGrid();
                LimparCamposAcrescimoDescontoBorderoTituloDocumento();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function ExcluirAcrescimoDescontoBorderoTituloDocumentoClick(e, sender) {
    exibirConfirmacao("Atenção!", "Deseja realmente excluir o valor de " + _acrescimoDescontoBorderoTituloDocumento.Valor.val() + "?", function () {
        ExcluirPorCodigo(_acrescimoDescontoBorderoTituloDocumento, "BorderoTituloDocumentoAcrescimoDesconto/Excluir", function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "Valor excluído com sucesso!");
                    PreecherCamposEdicaoBordero(r.Data);
                    _gridAcrescimoDescontoBorderoTituloDocumento.CarregarGrid();
                    _gridBorderoTituloDocumento.CarregarGrid();
                    _gridBorderoTitulo.CarregarGrid();
                    LimparCamposAcrescimoDescontoBorderoTituloDocumento();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

function CancelarAcrescimoDescontoBorderoTituloDocumentoClick(e, sender) {
    LimparCamposAcrescimoDescontoBorderoTituloDocumento();
}

function EditarAcrescimoDescontoBorderoTituloDocumentoClick(dadosGrid) {
    LimparCamposAcrescimoDescontoBorderoTituloDocumento();
    _acrescimoDescontoBorderoTituloDocumento.Codigo.val(dadosGrid.Codigo);
    BuscarPorCodigo(_acrescimoDescontoBorderoTituloDocumento, "BorderoTituloDocumentoAcrescimoDesconto/BuscarPorCodigo", function (r) {
        if (r.Success) {
            if (r.Data) {
                _acrescimoDescontoBorderoTituloDocumento.Justificativa.enable(false);
                _acrescimoDescontoBorderoTituloDocumento.Adicionar.visible(false);
                _acrescimoDescontoBorderoTituloDocumento.Atualizar.visible(true);
                _acrescimoDescontoBorderoTituloDocumento.Excluir.visible(true);
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

////*******METODOS*******

function CarregarGridAcrescimoDescontoBorderoTituloDocumento() {
    var permiteAlterar = false;

    if (_bordero.Situacao.val() == EnumSituacaoBordero.EmAndamento)
        permiteAlterar = true;

    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarAcrescimoDescontoBorderoTituloDocumentoClick, tamanho: "15", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [editar] };

    if (!permiteAlterar)
        menuOpcoes = null;

    if (_gridAcrescimoDescontoBorderoTituloDocumento != null)
        _gridAcrescimoDescontoBorderoTituloDocumento.Destroy();

    _gridAcrescimoDescontoBorderoTituloDocumento = new GridView(_acrescimoDescontoBorderoTituloDocumento.Grid.idGrid, "BorderoTituloDocumentoAcrescimoDesconto/Pesquisa", _acrescimoDescontoBorderoTituloDocumento, menuOpcoes, { column: 0, dir: orderDir.asc }, 5, null, false);
}

function LimparCamposAcrescimoDescontoBorderoTituloDocumento() {
    _acrescimoDescontoBorderoTituloDocumento.Justificativa.enable(true);
    _acrescimoDescontoBorderoTituloDocumento.Justificativa.val("");
    _acrescimoDescontoBorderoTituloDocumento.Justificativa.codEntity(0);
    _acrescimoDescontoBorderoTituloDocumento.Valor.val("");
    _acrescimoDescontoBorderoTituloDocumento.Observacao.val("");
    _acrescimoDescontoBorderoTituloDocumento.Adicionar.visible(true);
    _acrescimoDescontoBorderoTituloDocumento.Atualizar.visible(false);
    _acrescimoDescontoBorderoTituloDocumento.Excluir.visible(false);
}

function AbrirTelaAcrescimoDescontoBorderoTituloDocumento(dadosGrid) {
    LimparCamposAcrescimoDescontoBorderoTituloDocumento();
    _acrescimoDescontoBorderoTituloDocumento.Documento.val(dadosGrid.Codigo);
    _gridAcrescimoDescontoBorderoTituloDocumento.CarregarGrid();
    Global.abrirModal('knockoutAcrescimoDescontoBorderoTituloDocumento');
}

function FecharTelaAcrescimoDescontoBorderoTituloDocumento() {
    Global.fecharModal('knockoutAcrescimoDescontoBorderoTituloDocumento');
    LimparCamposAcrescimoDescontoBorderoTituloDocumento();
}