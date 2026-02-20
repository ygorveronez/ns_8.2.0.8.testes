/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

//*******MAPEAMENTO KNOUCKOUT*******
var _acrescimosDescontos;
var _gridAcrescimosDescontos;


var AcrescimosDescontos = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: ko.observable(0) });
    this.Fechamento = PropertyEntity({ type: types.map, val: ko.observable(0) });

    this.Motivo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Motivo:", idBtnSearch: guid(), visible: ko.observable(true), required: true });
    this.Valor = PropertyEntity({ text: "Valor:", getType: typesKnockout.decimal });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string });

    this.Adicionar = PropertyEntity({ text: "Adicionar", getType: typesKnockout.event, eventClick: AdicionarAcrescimosDescontos, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ text: "Atualizar", getType: typesKnockout.event, eventClick: AtualizarAcrescimosDescontos, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ text: "Excluir", getType: typesKnockout.event, eventClick: ExcluirAcrescimosDescontos, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ text: "Cancelar", getType: typesKnockout.event, eventClick: LimparCamposAcrescimosDescontos, visible: ko.observable(true) });
}

//*******EVENTOS*******
function LoadAcrescimosDescontos() {
    _acrescimosDescontos = new AcrescimosDescontos();
    KoBindings(_acrescimosDescontos, "knockoutAcrescimosDescontos");
    
    CarregarGridAcrescimosDescontos();

    new BuscarJustificativas(_acrescimosDescontos.Motivo, null, null, [/*EnumTipoFinalidadeJustificativa.Fatura, */EnumTipoFinalidadeJustificativa.Todas]);
}

function AdicionarAcrescimosDescontos(e, sender) {
    Salvar(_acrescimosDescontos, "FechamentoFreteAcrescimosDescontos/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "sucesso", "Adicionado com Sucesso");
                _gridAcrescimosDescontos.CarregarGrid();
                LimparCamposAcrescimosDescontos();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function AtualizarAcrescimosDescontos(e, sender) {
    Salvar(_acrescimosDescontos, "FechamentoFreteAcrescimosDescontos/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "sucesso", "Atualizado com sucesso");
                _gridAcrescimosDescontos.CarregarGrid();
                LimparCamposAcrescimosDescontos();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}

function ExcluirAcrescimosDescontos(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse Acréscimo/Desconto?", function () {
        ExcluirPorCodigo(_transportadorConfiguracaoNFSe, "FechamentoFreteAcrescimosDescontos/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridAcrescimosDescontos.CarregarGrid();
                LimparCamposAcrescimosDescontos();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}


//*******METODOS*******
function CarregarGridAcrescimosDescontos() {
    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [
            {
                descricao: "Editar",
                id: guid(),
                evento: "onclick",
                tamanho: "10",
                icone: "",
                metodo: EditarAcrescimosDescontos
            }
        ]
    };

    _gridAcrescimosDescontos = new GridView(_acrescimosDescontos.Codigo.idGrid, "", _fechamentoFrete, menuOpcoes);
}

function LimparCamposAcrescimosDescontos() {
    _acrescimosDescontos.Atualizar.visible(false);
    _acrescimosDescontos.Excluir.visible(false);
    _acrescimosDescontos.Adicionar.visible(true);

    LimparCampos(_acrescimosDescontos);

    _acrescimosDescontos.Fechamento.val(_fechamentoFrete.Codigo.val());
}

function EditarAcrescimosDescontos(data) {
    LimparCamposAcrescimosDescontos();
    _acrescimosDescontos.Codigo.val(data.Codigo);

    BuscarPorCodigo(_acrescimosDescontos, "FechamentoFreteAcrescimosDescontos/BuscarPorCodigo", function (arg) {
        _acrescimosDescontos.Atualizar.visible(true);
        _acrescimosDescontos.Excluir.visible(true);
        _acrescimosDescontos.Adicionar.visible(false);
    }, null);
}