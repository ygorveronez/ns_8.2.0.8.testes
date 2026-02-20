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
/// <reference path="../../Consultas/CentroResultado.js" />
/// <reference path="../../Consultas/TipoDespesaFinanceira.js" />
/// <reference path="TituloFinanceiro.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _centroResultadoTipoDespesa, _gridCentroResultadoTipoDespesa;

var CentroResultadoTipoDespesa = function () {
    this.Grid = PropertyEntity({ type: types.local });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CentroResultado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Centro de Resultado:", idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.TipoDespesa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tipo de Despesa:", idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.Percentual = PropertyEntity({ text: "*Percentual:", getType: typesKnockout.decimal, maxlength: 6, required: true, enable: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarCentroResultadoTipoDespesaClick, type: types.event, text: "Adicionar", visible: ko.observable(true), enable: ko.observable(true) });
};

//*******EVENTOS*******

function LoadCentroResultadoTipoDespesa() {

    _centroResultadoTipoDespesa = new CentroResultadoTipoDespesa();
    KoBindings(_centroResultadoTipoDespesa, "knockoutCentroResultadoTipoDespesa");

    new BuscarCentroResultado(_centroResultadoTipoDespesa.CentroResultado);
    new BuscarTipoDespesaFinanceira(_centroResultadoTipoDespesa.TipoDespesa);

    LoadGridCentroResultadoTipoDespesa();

    if (_CONFIGURACAO_TMS.AtivarControleDespesas) {
        $("#liTabCentroResultado").hide();
        $("#liTabCentroResultadoTipoDespesa").show();
    }
}

function AdicionarCentroResultadoTipoDespesaClick() {
    if (!ValidarCamposObrigatorios(_centroResultadoTipoDespesa)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");
        return;
    }

    if (Globalize.parseFloat(_centroResultadoTipoDespesa.Percentual.val()) > 100) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Informe um percentual menor ou igual a 100%.");
        return;
    }

    _centroResultadoTipoDespesa.Codigo.val(guid());
    _tituloFinanceiro.CentrosResultadoTiposDespesa.list.push(SalvarListEntity(_centroResultadoTipoDespesa));

    LimparCamposCentroResultadoTipoDespesa();
}

function ExcluirCentroResultadoTipoDespesaClick(data) {
    exibirConfirmacao("Atenção!", "Deseja realmente excluir o registro?", function () {
        for (var i = 0; i < _tituloFinanceiro.CentrosResultadoTiposDespesa.list.length; i++) {
            if (data.Codigo == _tituloFinanceiro.CentrosResultadoTiposDespesa.list[i].Codigo.val) {
                _tituloFinanceiro.CentrosResultadoTiposDespesa.list.splice(i, 1);
                break;
            }
        }

        LimparCamposCentroResultadoTipoDespesa();
    });
}

function PreencherCentroResultadoTipoDespesa(data) {
    if (_CONFIGURACAO_TMS.AtivarControleDespesas) {
        executarReST("TipoMovimento/ObterDetalhes", { Codigo: data.Codigo }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    var data = r.Data;
                    if (data.CodigoTipoDespesa > 0) {
                        _centroResultadoTipoDespesa.TipoDespesa.codEntity(data.CodigoTipoDespesa);
                        _centroResultadoTipoDespesa.TipoDespesa.val(data.TipoDespesa);
                    }

                    AplicarCentroResultadoTipoDespesaNaLista();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    }
}

function AplicarCentroResultadoTipoDespesaNaLista() {
    if (_centroResultadoTipoDespesa.TipoDespesa.codEntity() > 0 && _centroResultadoTipoDespesa.CentroResultado.codEntity() > 0) {
        if (_tituloFinanceiro.CentrosResultadoTiposDespesa.list.length > 0)
            _tituloFinanceiro.CentrosResultadoTiposDespesa.list = new Array();
        _centroResultadoTipoDespesa.Percentual.val("100,00");

        AdicionarCentroResultadoTipoDespesaClick();
    }
}

////*******MÉTODOS*******

function LoadGridCentroResultadoTipoDespesa() {
    var excluir = { descricao: "Excluir", id: guid(), evento: "onclick", metodo: function (data) { ExcluirCentroResultadoTipoDespesaClick(data); }, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [excluir] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "CentroResultado", title: "Centro de Resultado", width: "40%" },
        { data: "TipoDespesa", title: "Tipo de Despesa", width: "40%" },
        { data: "Percentual", title: "Percentual", width: "20%" }
    ];

    _gridCentroResultadoTipoDespesa = new BasicDataTable(_centroResultadoTipoDespesa.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    RecarregarGridCentroResultadoTipoDespesa();
}

function RecarregarGridCentroResultadoTipoDespesa() {
    var data = new Array();

    $.each(_tituloFinanceiro.CentrosResultadoTiposDespesa.list, function (i, item) {
        var itemGrid = new Object();

        itemGrid.Codigo = item.Codigo.val;
        itemGrid.CentroResultado = item.CentroResultado.val;
        itemGrid.TipoDespesa = item.TipoDespesa.val;
        itemGrid.Percentual = item.Percentual.val;

        data.push(itemGrid);
    });

    _gridCentroResultadoTipoDespesa.CarregarGrid(data);
}

function LimparCamposCentroResultadoTipoDespesa() {
    LimparCampos(_centroResultadoTipoDespesa);
    RecarregarGridCentroResultadoTipoDespesa();
}