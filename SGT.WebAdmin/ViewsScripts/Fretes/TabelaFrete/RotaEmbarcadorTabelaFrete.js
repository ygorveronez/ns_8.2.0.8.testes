/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/RotaEmbarcadorTabelaFrete.js" />
/// <reference path="TabelaFrete.js" />
/// <reference path="../../Consultas/RotaFrete.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridRotaEmbarcadorTabelaFrete;
var _rotaEmbarcadorTabelaFrete;

var RotaEmbarcadorTabelaFrete = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.RotaFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Fretes.TabelaFrete.Rota.getRequiredFieldDescription(), idBtnSearch: guid(), required: true, titulo: _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS ? "Adicionais por Rota" : "Restrição de Rotas" });
    this.Componente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Fretes.TabelaFrete.UsarComoValorComponente.getFieldDescription(), idBtnSearch: guid(), required: false, issue: 85, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) });
    this.ValorAdicionalFixoPorRota = PropertyEntity({ text: Localization.Resources.Fretes.TabelaFrete.AdicionarValorFixoFreteParaRota, issue: 716, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) });
    this.ValorFixoRota = PropertyEntity({ type: types.map, text: Localization.Resources.Fretes.TabelaFrete.ValorFixoParaRota.getRequiredFieldDescription(), val: ko.observable("0,00"), def: "0,00", getType: typesKnockout.decimal, visible: ko.observable(false), maxlength: 13, configDecimal: { precision: 2, allowZero: true }, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRotaEmbarcadorTabelaFreteClick, type: types.event, text: Localization.Resources.Fretes.TabelaFrete.Adicionar, visible: ko.observable(true) });
}


//*******EVENTOS*******

function loadRotaEmbarcadorTabelaFrete() {

    _rotaEmbarcadorTabelaFrete = new RotaEmbarcadorTabelaFrete();
    KoBindings(_rotaEmbarcadorTabelaFrete, "knockoutRotas");

    new BuscarComponentesDeFrete(_rotaEmbarcadorTabelaFrete.Componente);
    new BuscarRotasFrete(_rotaEmbarcadorTabelaFrete.RotaFrete, null, null, _tabelaFrete.GrupoPessoas);

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 5, opcoes: [{ descricao: Localization.Resources.Fretes.TabelaFrete.Excluir, id: guid(), metodo: excluirRotaEmbarcadorTabelaFreteClick }] };

    var visibleTMS = _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS;

    var header = [{ data: "Codigo", visible: false },
    { data: "CodigoComponente", visible: false },
    { data: "DescricaoRota", title: Localization.Resources.Fretes.TabelaFrete.Rota, width: visibleTMS ? "40%" : "90%" },
    { data: "DescricaoComponente", title: Localization.Resources.Fretes.TabelaFrete.Componente, width: "35%", visible: visibleTMS },
    { data: "ValorAdicionalFixoPorRota", title: Localization.Resources.Fretes.TabelaFrete.ValorFixoPorRota, width: "10%", visible: visibleTMS },
    { data: "ValorFixoRota", title: Localization.Resources.Fretes.TabelaFrete.Valor, width: "10%", visible: visibleTMS }
    ];


    _gridRotaEmbarcadorTabelaFrete = new BasicDataTable(_rotaEmbarcadorTabelaFrete.Grid.id, header, menuOpcoes, { column: 2, dir: orderDir.asc });

    recarregarGridRotaEmbarcadorTabelaFrete();
}

function recarregarGridRotaEmbarcadorTabelaFrete() {

    var data = new Array();

    $.each(_tabelaFrete.RotasFreteEmbarcador.list, function (i, rotaEmbarcadorTabelaFrete) {
        var rotaEmbarcadorTabelaFreteGrid = new Object();

        rotaEmbarcadorTabelaFreteGrid.Codigo = rotaEmbarcadorTabelaFrete.Codigo.val;
        rotaEmbarcadorTabelaFreteGrid.CodigoComponente = rotaEmbarcadorTabelaFrete.Componente.codEntity;
        rotaEmbarcadorTabelaFreteGrid.CodigoRota = rotaEmbarcadorTabelaFrete.RotaFrete.codEntity;
        rotaEmbarcadorTabelaFreteGrid.DescricaoComponente = rotaEmbarcadorTabelaFrete.Componente.val;
        rotaEmbarcadorTabelaFreteGrid.DescricaoRota = rotaEmbarcadorTabelaFrete.RotaFrete.val;
        rotaEmbarcadorTabelaFreteGrid.ValorFixoRota = rotaEmbarcadorTabelaFrete.ValorFixoRota.val;
        rotaEmbarcadorTabelaFreteGrid.ValorAdicionalFixoPorRota = rotaEmbarcadorTabelaFrete.ValorAdicionalFixoPorRota.val ? Localization.Resources.Fretes.TabelaFrete.Sim : Localization.Resources.Fretes.TabelaFrete.Nao;

        data.push(rotaEmbarcadorTabelaFreteGrid);
    });

    _gridRotaEmbarcadorTabelaFrete.CarregarGrid(data);
}


function excluirRotaEmbarcadorTabelaFreteClick(data) {
    for (var i = 0; i < _tabelaFrete.RotasFreteEmbarcador.list.length; i++) {
        if (data.CodigoRota == _tabelaFrete.RotasFreteEmbarcador.list[i].RotaFrete.codEntity) {
            _tabelaFrete.RotasFreteEmbarcador.list.splice(i, 1);
            break;
        }
    }

    recarregarGridRotaEmbarcadorTabelaFrete();
}

function adicionarRotaEmbarcadorTabelaFreteClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_rotaEmbarcadorTabelaFrete);

    if (valido) {
        var existe = false;

        $.each(_tabelaFrete.RotasFreteEmbarcador.list, function (i, rotaEmbarcadorTabelaFrete) {
            if (rotaEmbarcadorTabelaFrete.RotaFrete.codEntity == _rotaEmbarcadorTabelaFrete.RotaFrete.codEntity()) {
                existe = true;
                return;
            }
        });

        if (existe) {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Fretes.TabelaFrete.ComponenteJaExistente, RotaXJaEstaCadastrada.format(_rotaEmbarcadorTabelaFrete.RotaFrete.val()));
            return;
        }

        _tabelaFrete.RotasFreteEmbarcador.list.push(SalvarListEntity(_rotaEmbarcadorTabelaFrete));

        recarregarGridRotaEmbarcadorTabelaFrete();

        $("#" + _rotaEmbarcadorTabelaFrete.RotaFrete.id).focus();

        limparCamposRotaEmbarcadorTabelaFrete();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Fretes.TabelaFrete.CamposObrigatorios, Localization.Resources.Fretes.TabelaFrete.InformeOsCamposObrigatorios);
    }
}

function limparCamposRotaEmbarcadorTabelaFrete() {
    LimparCampos(_rotaEmbarcadorTabelaFrete);
}