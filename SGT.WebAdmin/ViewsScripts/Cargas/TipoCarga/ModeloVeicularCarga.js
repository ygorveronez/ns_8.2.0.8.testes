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
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="TipoCarga.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _modeloVeicularCarga;
var _gridReorder;
var ModeloVeicularCargaMap = function () {
    this.Posicao = PropertyEntity({ val: 0, def: 0 });
    this.Codigo = PropertyEntity({ val: 0, def: 0 });
    this.Descricao = PropertyEntity({ val: "" });
    this.TempoDescarga = PropertyEntity({ val: 0, def: 0 });
};

var ModeloVeicularCarga = function () {
    this.ModelosVeicularesCargas = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.TipoCarga.ModeloVeicularDeCarga.getRequiredFieldDescription(), issue: 585, required: true, idBtnSearch: guid() });

    this.TempoDescargaModeloVeicular = PropertyEntity({ text: Localization.Resources.Cargas.TipoCarga.TempoDeDescargaMinutos, visible: ko.observable(true), getType: typesKnockout.int, configInt: { precision: 0, allowZero: true }, maxlength: 5, val: ko.observable("0"), def: "0" });
    this.InserirModeloVeicularCarga = PropertyEntity({ eventClick: adicionarModeloVeicularCargaClick, type: types.event, text: Localization.Resources.Cargas.TipoCarga.AdicionarModeloVeicularDeCarga, visible: ko.observable(true) });

};

//*******EVENTOS*******

function loadTipoModeloVeicularCarga() {
    _modeloVeicularCarga = new ModeloVeicularCarga();
    KoBindings(_modeloVeicularCarga, "knockoutModeloVeicularCarga");

    new BuscarModelosVeicularesCarga(_modeloVeicularCarga.ModelosVeicularesCargas);

    var headHtml = '<tr><th width="5%"></th><th width="70%">' + Localization.Resources.Gerais.Geral.Descricao + '</th><th width="10%">' + Localization.Resources.Cargas.TipoCarga.TempoDeDescarga + '</th><th class="text-align-center" width="15%">' + Localization.Resources.Cargas.TipoCarga.Remover + '</th></tr>';
    _gridReorder = new GridReordering(Localization.Resources.Cargas.TipoCarga.MovaAsLinhasConformePreferenciaDePrioridades, "reorderSelector", headHtml, "");
    _gridReorder.CarregarGrid();
}

function adicionarModeloVeicularCargaClick(e, sender) {
    var tudoCerto = ValidarCampoObrigatorioEntity(_modeloVeicularCarga.ModelosVeicularesCargas);

    posicao = _tipoCarga.ModelosVeicularesCargas.list.length;

    if (tudoCerto) {
        $.each(_modeloVeicularCarga.ModelosVeicularesCargas.multiplesEntities(), function (x, modelo) {

            var existe = false;
            $.each(_tipoCarga.ModelosVeicularesCargas.list, function (i, modeloVeicularCarga) {
                if (modeloVeicularCarga.Codigo.val == modelo.Codigo) {
                    existe = true;
                    return false;
                }
            });

            if (!existe) {
                posicao++;
                var modeloMap = new ModeloVeicularCargaMap();
                modeloMap.Codigo.val = parseInt(modelo.Codigo);
                modeloMap.Posicao.val = posicao;
                modeloMap.Descricao.val = modelo.Descricao;
                modeloMap.TempoDescarga.val = _modeloVeicularCarga.TempoDescargaModeloVeicular.val();
                _tipoCarga.ModelosVeicularesCargas.list.push(modeloMap);
                reordenarPosicoesModelos();
                recarregarGridReorder();
                $("#" + _modeloVeicularCarga.ModelosVeicularesCargas.id).focus();

            } else {
                exibirMensagem("aviso", Localization.Resources.Cargas.TipoCarga.ModeloVeicularDeCargaJaInformado, Localization.Resources.Cargas.TipoCarga.ModeloVeicularDeCargaInformadoJaFoiAdicionadoParaEsseTipoDeCarga);
            }
        });
        LimparCampoEntity(_modeloVeicularCarga.ModelosVeicularesCargas);
    } else {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Cargas.TipoCarga.InformeModeloVeicularDeCarga);
    }
}

function removerModeloVeicularDeCargaClick(codigoModeloVeicularDeCarga, descricao) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.TipoCarga.RealmenteDesejaRemoverModeloVeicularDeCarga + descricao + Localization.Resources.Cargas.TipoCarga.DoTipoDeCarga.format(_tipoCarga.Descricao.val()), function () {
        reordenarPosicoesModelos();
        var novaLista = new Array();
        var novaPosicao = 1;
        $.each(_tipoCarga.ModelosVeicularesCargas.list, function (i, modeloVeicularCarga) {
            if (codigoModeloVeicularDeCarga != modeloVeicularCarga.Codigo.val) {
                modeloVeicularCarga.Posicao.val = novaPosicao;
                novaPosicao++;
                novaLista.push(modeloVeicularCarga);
            }
        });
        _tipoCarga.ModelosVeicularesCargas.list = novaLista;
        recarregarGridReorder();
    });
}

//*******MÉTODOS*******

function recarregarGridReorder() {
    var html = "";
    _tipoCarga.ModelosVeicularesCargas.list.sort(function (a, b) { return a.Posicao.val < b.Posicao.val });
    $.each(_tipoCarga.ModelosVeicularesCargas.list, function (i, modeloVeicularCarga) {
        html += '<tr data-position="' + modeloVeicularCarga.Posicao.val + '" id="sort_' + modeloVeicularCarga.Codigo.val + '"><td>' + modeloVeicularCarga.Posicao.val + '</td>';
        html += '<td>' + modeloVeicularCarga.Descricao.val + '</td>';
        html += '<td>' + modeloVeicularCarga.TempoDescarga.val + '</td>';
        html += '<td class="text-align-center"><a href="javascript:;" onclick="removerModeloVeicularDeCargaClick(' + modeloVeicularCarga.Codigo.val + ', \'' + modeloVeicularCarga.Descricao.val + '\')">Remover</a></td></tr>';
    });
    _gridReorder.RecarregarGrid(html);
}

function reordenarPosicoesModelos() {
    var ListaOrdenada = _gridReorder.ObterOrdencao();
    $.each(_tipoCarga.ModelosVeicularesCargas.list, function (i, modeloVeicularCarga) {
        $.each(ListaOrdenada, function (i, ordem) {
            if (ordem.id.split("_")[1] == modeloVeicularCarga.Codigo.val) {
                modeloVeicularCarga.Posicao.val = ordem.posicao;
            }
        });
    });
}
