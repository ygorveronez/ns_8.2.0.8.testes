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
/// <reference path="GrupoProduto.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridReorder;

var GrupoProdutoTipoCarga = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Posicao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.Gerais.Geral.TipoCarga.getRequiredFieldDescription(), issue: 551, idBtnSearch: guid() });
    this.Adicionar = PropertyEntity({ eventClick: adicionarGrupoProdutoTipoCargaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
}


//*******EVENTOS*******

function loadGrupoProdutoTipoCarga() {
    var grupoProdutoTipoCarga = new GrupoProdutoTipoCarga();
    KoBindings(grupoProdutoTipoCarga, "knockoutGrupoProdutoTipoCarga");
    new BuscarTiposdeCarga(grupoProdutoTipoCarga.TipoCarga);

    var headHtml = '<tr><th width="5%"></th><th width="80%">'+ Localization.Resources.Gerais.Geral.Descricao +'</th><th class="text-align-center" width="15%">'+ Localization.Resources.Gerais.Geral.Remover +'</th></tr>';
    _gridReorder = new GridReordering(Localization.Resources.Produtos.GrupoProduto.MovaLinhasConformePreferenciaDePrioridade, "reorderSelector", headHtml, "");
    _gridReorder.CarregarGrid();
}

function adicionarGrupoProdutoTipoCargaClick(e, sender) {
    var tudoCerto = ValidarCampoObrigatorioEntity(_grupoProduto.TiposCarga);
    if (tudoCerto) {
        var existe = false;
        $.each(_grupoProduto.TiposCarga.list, function (i, grupoProdutoTipoCarga) {
            if (grupoProdutoTipoCarga.TipoCarga.codEntity == e.TipoCarga.codEntity()) {
                existe = true;
                return false;
            }
        });
        if (!existe) {
            e.Posicao.val(_grupoProduto.TiposCarga.list.length + 1)
            _grupoProduto.TiposCarga.list.push(SalvarListEntity(e));

            reordenarPosicoesTiposCarga();
            recarregarGridReorder();
            LimparCampos(e);
            $("#" + e.TipoCarga.id).focus();
        } else {
            exibirMensagem("aviso", Localization.Resources.Produtos.GrupoProduto.TipoCargaJaInformado, Localization.Resources.Produtos.GrupoProduto.TipoCargaJaVinculadoAoGrupoProduto.format(e.TipoCarga.val(), _grupoProduto.Descricao.val()));
        }
        LimparCampoEntity(_grupoProduto.TiposCarga);
    } else {
        exibirMensagem("atencao", Localization.Resources.Gerais.Geral.CampoObrigatorio, Localization.Resources.Produtos.GrupoProduto.InformeTipoCarga);
    }
}

function removerTipoCargaClick(codigoTipoCarga, descricao) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Produtos.GrupoProduto.RealmenteDesejaRemoverTipoDeCargaGrupoProduto.format(descricao, _grupoProduto.Descricao.val()), function () {
        reordenarPosicoesTiposCarga();
        var novaLista = new Array();
        var novaPosicao = 1;
        $.each(_grupoProduto.TiposCarga.list, function (i, grupoProdutoTipoCarga) {
            if (codigoTipoCarga != grupoProdutoTipoCarga.TipoCarga.codEntity) {
                grupoProdutoTipoCarga.Posicao.val = novaPosicao;
                novaPosicao++;
                novaLista.push(grupoProdutoTipoCarga);
            }
        });
        _grupoProduto.TiposCarga.list = novaLista;
        recarregarGridReorder();
    });
}

//*******MÉTODOS*******


function recarregarGridReorder() {

    var html = "";
    _grupoProduto.TiposCarga.list.sort(function (a, b) { return a.Posicao.val < b.Posicao.val });
    $.each(_grupoProduto.TiposCarga.list, function (i, grupoProdutoTipoCarga) {
        html += '<tr data-position="' + grupoProdutoTipoCarga.Posicao.val + '" id="sort_' + grupoProdutoTipoCarga.TipoCarga.codEntity + '"><td>' + grupoProdutoTipoCarga.Posicao.val + '</td>';
        html += '<td>' + grupoProdutoTipoCarga.TipoCarga.val + '</td>';
        html += '<td class="text-align-center"><a href="javascript:;" onclick="removerTipoCargaClick(' + grupoProdutoTipoCarga.TipoCarga.codEntity + ', \'' + grupoProdutoTipoCarga.TipoCarga.val + '\')">'+ Localization.Resources.Gerais.Geral.Remover +'</a></td></tr>';
    });
    _gridReorder.RecarregarGrid(html);
}

function reordenarPosicoesTiposCarga() {
    var ListaOrdenada = _gridReorder.ObterOrdencao();
    $.each(_grupoProduto.TiposCarga.list, function (i, grupoProdutoTipoCarga) {
        $.each(ListaOrdenada, function (i, ordem) {
            if (ordem.id.split("_")[1] == grupoProdutoTipoCarga.TipoCarga.codEntity) {
                grupoProdutoTipoCarga.Posicao.val = ordem.posicao;
            }
        });
    });
}
