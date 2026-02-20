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
/// <reference path="../../Consultas/TipoCarga.js" />

/// <reference path="TipoCargaModeloVeicularAutoConfig.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridReorder;

var TipoCargaMap = function () {
    this.Posicao = PropertyEntity({ val: 0, def: 0 });
    this.Codigo = PropertyEntity({ val: 0, def: 0 });
    this.Descricao = PropertyEntity({ val: "" });
}


//*******EVENTOS*******

function loadTipoCarga() {

    new BuscarTiposdeCarga(_tipoCargaModeloVeicularAutoConfig.TiposCarga);

    var headHtml = '<tr><th width="5%"></th><th width="80%">Descrição</th><th class="text-align-center" width="15%">Remover</th></tr>';
    _gridReorder = new GridReordering(Localization.Resources.Cargas.TipoCargaModeloVeicularAutoConfig.MovaLinhasConformePreferenciaPrioridades, "reorderSelector", headHtml, "");
    _gridReorder.CarregarGrid();
}

function adicionarTipoCargaClick(e, sender) {
    var tudoCerto = ValidarCampoObrigatorioEntity(_tipoCargaModeloVeicularAutoConfig.TiposCarga);
    if (tudoCerto) {
        var existe = false;
        $.each(_tipoCargaModeloVeicularAutoConfig.TiposCarga.list, function (i, tipoCarga) {
            if (tipoCarga.Codigo.val == _tipoCargaModeloVeicularAutoConfig.TiposCarga.codEntity()) {
                existe = true;
                return false;
            }
        });
        if (!existe) {
            var tipoCarga = new TipoCargaMap();
            tipoCarga.Codigo.val = parseInt(_tipoCargaModeloVeicularAutoConfig.TiposCarga.codEntity());
            tipoCarga.Posicao.val = _tipoCargaModeloVeicularAutoConfig.TiposCarga.list.length + 1;
            tipoCarga.Descricao.val = _tipoCargaModeloVeicularAutoConfig.TiposCarga.val();
            _tipoCargaModeloVeicularAutoConfig.TiposCarga.list.push(tipoCarga);
            reordenarPosicoesTipoCarga();
            recarregarGridReorder();
            $("#" + _tipoCargaModeloVeicularAutoConfig.TiposCarga.id).focus();
        } else {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Cargas.TipoCargaModeloVeicularAutoConfig.TipoCargaJaInformado, Localization.Resources.Cargas.TipoCargaModeloVeicularAutoConfig.TipoCargaInformadoFoiAdicionado);
        }
        LimparCampoEntity(_tipoCargaModeloVeicularAutoConfig.TiposCarga);
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CampoObrigatorio, Localization.Resources.Cargas.TipoCargaModeloVeicularAutoConfig.InformeTipoCarga);
    }
}

function removerTipoDeCargaClick(codigoModeloVeicularDeCarga, descricao) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.TipoCargaModeloVeicularAutoConfig.RealmenteDesejaRemoverTipoCarga + descricao + " ?", function () {
        reordenarPosicoesTipoCarga();
        var novaLista = new Array();
        var novaPosicao = 1;
        $.each(_tipoCargaModeloVeicularAutoConfig.TiposCarga.list, function (i, tipoCarga) {
            if (codigoModeloVeicularDeCarga != tipoCarga.Codigo.val) {
                tipoCarga.Posicao.val = novaPosicao;
                novaPosicao++;
                novaLista.push(tipoCarga);
            }
        });
        _tipoCargaModeloVeicularAutoConfig.TiposCarga.list = novaLista;
        recarregarGridReorder();
    });
}

//*******MÉTODOS*******


function recarregarGridReorder() {

    var html = "";
    _tipoCargaModeloVeicularAutoConfig.TiposCarga.list.sort(function (a, b) { return a.Posicao.val < b.Posicao.val });
    $.each(_tipoCargaModeloVeicularAutoConfig.TiposCarga.list, function (i, tipoCarga) {
        html += '<tr data-position="' + tipoCarga.Posicao.val + '" id="sort_' + tipoCarga.Codigo.val + '"><td>' + tipoCarga.Posicao.val + '</td>';
        html += '<td>' + tipoCarga.Descricao.val + '</td>';
        html += '<td class="text-align-center"><a href="javascript:;" onclick="removerTipoDeCargaClick(' + tipoCarga.Codigo.val + ', \'' + tipoCarga.Descricao.val + '\')">Remover</a></td></tr>';
    });
    _gridReorder.RecarregarGrid(html);
}

function reordenarPosicoesTipoCarga() {
    var ListaOrdenada = _gridReorder.ObterOrdencao();
    $.each(_tipoCargaModeloVeicularAutoConfig.TiposCarga.list, function (i, tipoCarga) {
        $.each(ListaOrdenada, function (i, ordem) {
            if (ordem.id.split("_")[1] == tipoCarga.Codigo.val) {
                tipoCarga.Posicao.val = ordem.posicao;
            }
        });
    });
}
