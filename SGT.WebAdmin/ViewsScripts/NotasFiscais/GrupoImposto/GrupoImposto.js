/// <reference path="../../Enumeradores/EnumCamposGrupoImposto.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../Consultas/GrupoImposto.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridGrupoImposto;
var _grupoImposto;
var _pesquisaGrupoImposto;
var _gridItensImposto;

var _tipoCampoAlterar = [
    { text: "Selecione", value: 0 },
    { text: "Aliq. Interna Venda", value: EnumCamposGrupoImposto.AliquotaICMSInternaVenda },
    { text: "Aliq. Interestadual Venda", value: EnumCamposGrupoImposto.AliquotaICMSInterestadualVenda },
    { text: "MVA Venda", value: EnumCamposGrupoImposto.MVAVenda },
    { text: "CST/CSOSN Venda", value: EnumCamposGrupoImposto.CSTICMSVenda },
    { text: "CFOP Venda", value: EnumCamposGrupoImposto.CFOPVenda },
    { text: "Red. BC MVA Venda", value: EnumCamposGrupoImposto.ReducaoMVAVenda },
    { text: "Dif. Alíquota", value: EnumCamposGrupoImposto.DiferencialAliquotaVenda },
    { text: "Red. BC ICMS Venda", value: EnumCamposGrupoImposto.ReducaoBCICMSVenda },
    { text: "CST PIS Venda", value: EnumCamposGrupoImposto.CSTPISVenda },
    { text: "Red. BC PIS Venda", value: EnumCamposGrupoImposto.ReducaoBCPISVenda },
    { text: "Aliq. PIS Venda", value: EnumCamposGrupoImposto.AliquotaPISVenda },
    { text: "CST COFINS Venda", value: EnumCamposGrupoImposto.CSTCOFINSVenda },
    { text: "Red. BC COFINS Venda", value: EnumCamposGrupoImposto.ReducaoBCCOFINSVenda },
    { text: "Aliq. COFINS Venda", value: EnumCamposGrupoImposto.AliquotaCOFINSVenda },
    { text: "FCP", value: EnumCamposGrupoImposto.AliquotaFCPVenda },
    { text: "DIFAL", value: EnumCamposGrupoImposto.AliquotaDifalVenda },
    { text: "Aliq. ICMS Compra", value: EnumCamposGrupoImposto.AliquotaICMSCompra },
    { text: "MVA Compra", value: EnumCamposGrupoImposto.MVACompra },
    { text: "CST/CSOSN ICMS Compra", value: EnumCamposGrupoImposto.CSTICMSCompra },
    { text: "CFOP Compra", value: EnumCamposGrupoImposto.CFOPCompra },
    { text: "Red. BC ICMS Compra", value: EnumCamposGrupoImposto.ReducaoBCICMSCompra },
    { text: "Red. MVA Compra", value: EnumCamposGrupoImposto.ReducaoMVACompra },
    { text: "CST PIS Compra", value: EnumCamposGrupoImposto.CSTPISCompra },
    { text: "Red. BC PIS Compra", value: EnumCamposGrupoImposto.ReducaoBCPISCompra },
    { text: "Aliq. PIS Compra", value: EnumCamposGrupoImposto.AliquotaPISCompra },
    { text: "CST COFINS Compra", value: EnumCamposGrupoImposto.CSTCOFINSCompra },
    { text: "Red. BC COFINS Compra", value: EnumCamposGrupoImposto.ReducaoBCCOFINSCompra },
    { text: "Aliq. COFINS Compra", value: EnumCamposGrupoImposto.AliquotaCOFINSCompra },
    { text: "Observação", value: EnumCamposGrupoImposto.ObservacaoFiscal }
]

var _estadosOrigem = [
    { text: "Selecione", value: "" },
    { text: "ACRE", value: "AC" },
    { text: "ALAGOAS", value: "AL" },
    { text: "AMAZONAS", value: "AM" },
    { text: "AMAPA", value: "AP" },
    { text: "BAHIA", value: "BA" },
    { text: "CEARA", value: "CE" },
    { text: "DISTRITO FEDERAL", value: "DF" },
    { text: "ESPIRITO SANTO", value: "ES" },
    { text: "EXPORTACAO", value: "EX" },
    { text: "GOIAS", value: "GO" },
    { text: "MARANHAO", value: "MA" },
    { text: "MINAS GERAIS", value: "MG" },
    { text: "MATO GROSSO DO SUL", value: "MS" },
    { text: "MATO GROSSO", value: "MT" },
    { text: "PARA", value: "PA" },
    { text: "PARAIBA", value: "PB" },
    { text: "PERNAMBUCO", value: "PE" },
    { text: "PIAUI", value: "PI" },
    { text: "PARANA", value: "PR" },
    { text: "RIO DE JANEIRO", value: "RJ" },
    { text: "RIO GRANDE DO NORTE", value: "RN" },
    { text: "RONDONIA", value: "RO" },
    { text: "RORAIMA", value: "RR" },
    { text: "RIO GRANDE DO SUL", value: "RS" },
    { text: "SANTA CATARINA", value: "SC" },
    { text: "SERGIPE", value: "SE" },
    { text: "SAO PAULO", value: "SP" },
    { text: "TOCANTINS", value: "TO" }]


var PesquisaGrupoImposto = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Status: " });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridGrupoImposto.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var GrupoImposto = function () {
    this.Visible = PropertyEntity({ visible: false, idFade: guid(), visibleFade: ko.observable(false) });
    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.Visible.visibleFade() == true) {
                e.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Clique para visualizar as opções de importações", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true });
    this.NCM = PropertyEntity({ text: "NCM: ", required: false, maxlength: 8 });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: " });

    this.TipoCampo = PropertyEntity({ val: ko.observable(0), options: _tipoCampoAlterar, def: 0, text: "Selecione o campo que deseja alterar: ", eventChange: TipoCampoChange });
    this.ValorCampoCFOP = PropertyEntity({ text: "Informe o valor o CFOP alterado: ", required: false, maxlength: 4, getType: typesKnockout.string, visible: ko.observable(true) });
    this.ValorCampoCSTICMS = PropertyEntity({ text: "Informe o valor o CST/CSOSN alterado: ", required: false, maxlength: 3, getType: typesKnockout.string, visible: ko.observable(false) });
    this.ValorCampoDecimal = PropertyEntity({ text: "Informe o valor a ser alterado: ", required: false, maxlength: ko.observable(5), getType: typesKnockout.decimal, visible: ko.observable(false) });
    this.ValorCampoCST = PropertyEntity({ text: "Informe o valor o CST alterado: ", required: false, maxlength: 2, getType: typesKnockout.string, visible: ko.observable(false) });
    this.ValorCampoString = PropertyEntity({ text: "Informe o valor a ser alterado: ", required: false, maxlength: 1, getType: typesKnockout.string, visible: ko.observable(false) });
    this.AlterarValorCampo = PropertyEntity({ eventClick: AlterarValorCampoClick, type: types.event, text: "Clique para alterar o campo selecionado", visible: ko.observable(true) });

    this.EstadoOrigem = PropertyEntity({ val: ko.observable(""), options: _estadosOrigem, def: "", text: "Selecione o estado de Origem: ", eventChange: EstadoOrigemChange });
    this.EstadoDestino = PropertyEntity({ val: ko.observable(""), options: _estadosOrigem, def: "", text: "Selecione o estado de Destino: ", eventChange: EstadoOrigemChange });

    this.ListaItensImposto = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ItensImposto = PropertyEntity({ type: types.map, required: false, text: "", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), enable: ko.observable(true) });

    this.CopiarTabelaImposto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Selecione uma tabela de imposto para copiar os dados:", idBtnSearch: guid(), required: false, visible: true });
    this.LancarNovosRegistros = PropertyEntity({ eventClick: LancarNovosRegistrosClick, type: types.event, text: "Clique para lançar uma nova tabela de imposto zerada", visible: ko.observable(true) });

    this.TipoCampo.val.subscribe(function (novoValor) {
        _grupoImposto.ValorCampoDecimal.maxlength(5);

        if (novoValor == EnumCamposGrupoImposto.MVAVenda)
            _grupoImposto.ValorCampoDecimal.maxlength(6);
    });

    //CRUD
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******


function loadGrupoImposto() {

    _grupoImposto = new GrupoImposto();
    KoBindings(_grupoImposto, "knockoutCadastroGrupoImposto");

    _pesquisaGrupoImposto = new PesquisaGrupoImposto();
    KoBindings(_pesquisaGrupoImposto, "knockoutPesquisaGrupoImposto", false, _pesquisaGrupoImposto.Pesquisar.id);

    HeaderAuditoria("GrupoImposto", _grupoImposto);

    new BuscarGrupoImposto(_grupoImposto.CopiarTabelaImposto, RetornoCopiarTabela);

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoGrupoImposto", visible: false },
        { data: "UFOrigem", title: "Origem", width: "3%", className: "text-align-center", visible: true },
        { data: "UFDestino", title: "Destino", width: "3%", className: "text-align-center" },
        { data: "AliquotaICMSInternaVenda", title: "Aliq. Interna", width: "4%", className: "text-align-right" },
        { data: "AliquotaICMSInterestadualVenda", title: "Aliq. Interestadual", width: "6%", className: "text-align-right" },
        { data: "MVAVenda", title: "MVA Venda", width: "4%", className: "text-align-right" },
        { data: "CSTICMSVenda", title: "CST/CSOSN", width: "4%", className: "text-align-left" },
        { data: "CFOPVenda", title: "CFOP", width: "3%", className: "text-align-left" },
        { data: "ReducaoMVAVenda", title: "Red. MVA Venda", width: "5%", className: "text-align-right" },
        { data: "DiferencialAliquotaVenda", title: "Aliq. Diferencial", width: "5%", className: "text-align-center" },
        { data: "Atividade", title: "Atividade", width: "9%", className: "text-align-left" },
        { data: "ReducaoBCICMSVenda", title: "Red. BC ICMS", width: "5%", className: "text-align-right" },
        { data: "CSTPISVenda", title: "CST PIS", width: "3%", className: "text-align-left" },
        { data: "ReducaoBCPISVenda", title: "Red. BC PIS", width: "4%", className: "text-align-right" },
        { data: "AliquotaPISVenda", title: "Aliq. PIS", width: "3%", className: "text-align-right" },
        { data: "CSTCOFINSVenda", title: "CST COFINS", width: "5%", className: "text-align-left" },
        { data: "ReducaoBCCOFINSVenda", title: "Red. BC COFINS", width: "6%", className: "text-align-right" },
        { data: "AliquotaCOFINSVenda", title: "Aliq. COFINS", width: "4%", className: "text-align-right" },
        { data: "AliquotaFCPVenda", title: "FCP", width: "3%", className: "text-align-right" },
        { data: "AliquotaDifalVenda", title: "Aliq. DIFAL", width: "4%", className: "text-align-right" },
        { data: "AliquotaICMSCompra", title: "Aliq. ICMS Comp", width: "5%", className: "text-align-right" },
        { data: "MVACompra", title: "MVA Comp", width: "4%", className: "text-align-right" },
        { data: "CSTICMSCompra", title: "CST/CSON Comp", width: "5%", className: "text-align-left" },
        { data: "CFOPCompra", title: "CFOP Comp", width: "4%", className: "text-align-left" },
        { data: "ReducaoBCICMSCompra", title: "Red. BC ICMS Comp", width: "6%", className: "text-align-right" },
        { data: "ReducaoMVACompra", title: "Red. MVA Comp", width: "5%", className: "text-align-right" },
        { data: "CSTPISCompra", title: "CST PIS Comp", width: "5%", className: "text-align-left" },
        { data: "ReducaoBCPISCompra", title: "Red. BC PIS Comp", width: "6%", className: "text-align-right" },
        { data: "AliquotaPISCompra", title: "Aliq. PIS Comp", width: "5%", className: "text-align-right" },
        { data: "CSTCOFINSCompra", title: "CST COFINS Comp", width: "6%", className: "text-align-left" },
        { data: "ReducaoBCCOFINSCompra", title: "Red BC COFINS Comp", width: "6%", className: "text-align-right" },
        { data: "AliquotaCOFINSCompra", title: "Aliq. COFINS Comp", width: "6%", className: "text-align-right" },
        { data: "ObservacaoFiscal", title: "OBS", width: "3%", className: "text-align-center" }
    ];

    _gridItensImposto = new BasicDataTable(_grupoImposto.ItensImposto.idGrid, header, null, null, null, 500, false, false, null, null, null, null, null);
    _grupoImposto.ItensImposto.basicTable = _gridItensImposto;

    $('#' + _grupoImposto.ItensImposto.idGrid).on('click', 'tbody td', function (e) {
        e.stopPropagation();
        editCell(this, e);
    });

    buscarGrupoImpostos();
}

var valorItemEdicao = null;
var htmlItemEdicao = null;
var codigoItemEdicao = null;
var permiteAlterar = true;

function editCell(cell, data) {
    var span = $(cell).find("span")[0];

    if ($(cell).index() == 5 || $(cell).index() == 8 || $(cell).index() == 2
        || $(cell).index() == 3 || $(cell).index() == 4 || $(cell).index() == 5
        || $(cell).index() == 6 || $(cell).index() == 7 || $(cell).index() == 8
        || $(cell).index() == 10 || $(cell).index() == 11 || $(cell).index() == 12
        || $(cell).index() == 13 || $(cell).index() == 14 || $(cell).index() == 15
        || $(cell).index() == 16 || $(cell).index() == 17 || $(cell).index() == 18
        || $(cell).index() == 19 || $(cell).index() == 20 || $(cell).index() == 21
        || $(cell).index() == 22 || $(cell).index() == 23 || $(cell).index() == 24
        || $(cell).index() == 25 || $(cell).index() == 26 || $(cell).index() == 27
        || $(cell).index() == 28 || $(cell).index() == 29 || $(cell).index() == 30
        || $(cell).index() == 31 && $(span).data("codigoItem") != null) {
        codigoItemEdicao = $(span).data("codigoItem");

        var idTxt = guid();

        htmlItemEdicao = cell.innerHTML;
        if ($(cell).index() == 5 || $(cell).index() == 6 || $(cell).index() == 8 || $(cell).index() == 11 || $(cell).index() == 14 || $(cell).index() == 21 || $(cell).index() == 22 || $(cell).index() == 25 || $(cell).index() == 28 || $(cell).index() == 31) {
            if (cell.innerHTML.split("</span>")[1] == undefined)
                valorItemEdicao = "";
            else
                valorItemEdicao = cell.innerHTML.split("</span>")[1].trim();
            cell.innerHTML = '<input id="' + idTxt + '" type="text" value="' + valorItemEdicao + '" style="width: 100%; height: 100%;" />';
        } else {
            if (cell.innerHTML.split("</span>")[1] == undefined)
                valorItemEdicao = Globalize.parseFloat("0,00");
            else
                valorItemEdicao = Globalize.parseFloat(cell.innerHTML.split("</span>")[1].trim());
            cell.innerHTML = '<input id="' + idTxt + '" type="text" value="' + Globalize.format(valorItemEdicao, "n2") + '" style="width: 100%; height: 100%;" />';
            $("#" + idTxt).maskMoney();
        }

        switch ($(cell).index()) {
            case 2:
                $("#" + idTxt).attr("maxlength", "5");
                break;
            case 3:
                $("#" + idTxt).attr("maxlength", "5");
            case 4:
                $("#" + idTxt).attr("maxlength", "6");
                break;
            case 5:
                $("#" + idTxt).attr("maxlength", "3");
                break;
            case 6:
                $("#" + idTxt).attr("maxlength", "4");
                break;
            case 7:
                $("#" + idTxt).attr("maxlength", "5");
                break;
            case 8:
                $("#" + idTxt).attr("maxlength", "1");
                break;
            case 10:
                $("#" + idTxt).attr("maxlength", "5");
                break;
            case 11:
                $("#" + idTxt).attr("maxlength", "2");
                break;
            case 12:
                $("#" + idTxt).attr("maxlength", "5");
                break;
            case 13:
                $("#" + idTxt).attr("maxlength", "5");
                break;
            case 14:
                $("#" + idTxt).attr("maxlength", "2");
                break;
            case 15:
                $("#" + idTxt).attr("maxlength", "5");
                break;
            case 16:
                $("#" + idTxt).attr("maxlength", "5");
                break;
            case 17:
                $("#" + idTxt).attr("maxlength", "5");
                break;
            case 18:
                $("#" + idTxt).attr("maxlength", "5");
                break;
            case 19:
                $("#" + idTxt).attr("maxlength", "5");
                break;
            case 20:
                $("#" + idTxt).attr("maxlength", "5");
                break;
            case 21:
                $("#" + idTxt).attr("maxlength", "3");
                break;
            case 22:
                $("#" + idTxt).attr("maxlength", "4");
                break;
            case 23:
                $("#" + idTxt).attr("maxlength", "5");
                break;
            case 24:
                $("#" + idTxt).attr("maxlength", "5");
                break;
            case 25:
                $("#" + idTxt).attr("maxlength", "2");
                break;
            case 26:
                $("#" + idTxt).attr("maxlength", "5");
                break;
            case 27:
                $("#" + idTxt).attr("maxlength", "5");
                break;
            case 28:
                $("#" + idTxt).attr("maxlength", "2");
                break;
            case 29:
                $("#" + idTxt).attr("maxlength", "5");
                break;
            case 30:
                $("#" + idTxt).attr("maxlength", "5");
                break;
            case 31:
                $("#" + idTxt).attr("maxlength", "1");
                break;
            default:
                break;
        }

        $("#" + idTxt).focus();

        $("#" + idTxt).focusout(function () {
            if ($(cell).index() == 5 || $(cell).index() == 6 || $(cell).index() == 8 || $(cell).index() == 11 || $(cell).index() == 14 || $(cell).index() == 21 || $(cell).index() == 22 || $(cell).index() == 25 || $(cell).index() == 28 || $(cell).index() == 31)
                var valor = $("#" + idTxt).val();
            else
                var valor = Globalize.parseFloat($("#" + idTxt).val());

            if (valor == NaN || valor == valorItemEdicao) {
                $(this).closest("td").html(htmlItemEdicao);
                htmlItemEdicao = null;
                valorItemEdicao = null;
            } else {
                var itensImpostoGrid = _gridItensImposto.BuscarRegistros();

                for (var i = 0; i < itensImpostoGrid.length; i++) {
                    if (codigoItemEdicao == itensImpostoGrid[i].Codigo) {
                        if ($(cell).index() == 2)
                            itensImpostoGrid[i].AliquotaICMSInternaVenda = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + Globalize.format(valor, "n2");
                        else if ($(cell).index() == 3)
                            itensImpostoGrid[i].AliquotaICMSInterestadualVenda = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + Globalize.format(valor, "n2");
                        else if ($(cell).index() == 4)
                            itensImpostoGrid[i].MVAVenda = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + Globalize.format(valor, "n2");
                        else if ($(cell).index() == 5)
                            itensImpostoGrid[i].CSTICMSVenda = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + valor;
                        else if ($(cell).index() == 6)
                            itensImpostoGrid[i].CFOPVenda = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + valor;
                        else if ($(cell).index() == 7)
                            itensImpostoGrid[i].ReducaoMVAVenda = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + Globalize.format(valor, "n2");
                        else if ($(cell).index() == 8)
                            itensImpostoGrid[i].DiferencialAliquotaVenda = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + valor;
                        else if ($(cell).index() == 10)
                            itensImpostoGrid[i].ReducaoBCICMSVenda = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + Globalize.format(valor, "n2");
                        else if ($(cell).index() == 11)
                            itensImpostoGrid[i].CSTPISVenda = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + valor;
                        else if ($(cell).index() == 12)
                            itensImpostoGrid[i].ReducaoBCPISVenda = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + Globalize.format(valor, "n2");
                        else if ($(cell).index() == 13)
                            itensImpostoGrid[i].AliquotaPISVenda = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + Globalize.format(valor, "n2");
                        else if ($(cell).index() == 14)
                            itensImpostoGrid[i].CSTCOFINSVenda = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + valor;
                        else if ($(cell).index() == 15)
                            itensImpostoGrid[i].ReducaoBCCOFINSVenda = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + Globalize.format(valor, "n2");
                        else if ($(cell).index() == 16)
                            itensImpostoGrid[i].AliquotaCOFINSVenda = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + Globalize.format(valor, "n2");
                        else if ($(cell).index() == 17)
                            itensImpostoGrid[i].AliquotaFCPVenda = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + Globalize.format(valor, "n2");
                        else if ($(cell).index() == 18)
                            itensImpostoGrid[i].AliquotaDifalVenda = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + Globalize.format(valor, "n2");
                        else if ($(cell).index() == 19)
                            itensImpostoGrid[i].AliquotaICMSCompra = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + Globalize.format(valor, "n2");
                        else if ($(cell).index() == 20)
                            itensImpostoGrid[i].MVACompra = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + Globalize.format(valor, "n2");
                        else if ($(cell).index() == 21)
                            itensImpostoGrid[i].CSTICMSCompra = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + valor;
                        else if ($(cell).index() == 22)
                            itensImpostoGrid[i].CFOPCompra = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + valor;
                        else if ($(cell).index() == 23)
                            itensImpostoGrid[i].ReducaoBCICMSCompra = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + Globalize.format(valor, "n2");
                        else if ($(cell).index() == 24)
                            itensImpostoGrid[i].ReducaoMVACompra = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + Globalize.format(valor, "n2");
                        else if ($(cell).index() == 25)
                            itensImpostoGrid[i].CSTPISCompra = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + valor;
                        else if ($(cell).index() == 26)
                            itensImpostoGrid[i].ReducaoBCPISCompra = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + Globalize.format(valor, "n2");
                        else if ($(cell).index() == 27)
                            itensImpostoGrid[i].AliquotaPISCompra = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + Globalize.format(valor, "n2");
                        else if ($(cell).index() == 28)
                            itensImpostoGrid[i].CSTCOFINSCompra = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + valor;
                        else if ($(cell).index() == 29)
                            itensImpostoGrid[i].ReducaoBCCOFINSCompra = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + Globalize.format(valor, "n2");
                        else if ($(cell).index() == 30)
                            itensImpostoGrid[i].AliquotaCOFINSCompra = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + Globalize.format(valor, "n2");
                        else if ($(cell).index() == 31)
                            itensImpostoGrid[i].ObservacaoFiscal = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + valor;
                        break;
                    }
                }
                _gridItensImposto.CarregarGrid(itensImpostoGrid);
            }
        });
    }
}

function LancarNovosRegistrosClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja lançar novos dados para a tabela de imposto e digita-los manualmente?", function () {
        var data = {
            Codigo: _grupoImposto.Codigo.val(),
            Descricao: _grupoImposto.Descricao.val(),
            NCM: _grupoImposto.NCM.val(),
            Ativo: _grupoImposto.Ativo.val(),
            EstadoOrigem: _grupoImposto.EstadoOrigem.val(),
            EstadoDestino: _grupoImposto.EstadoDestino.val()
        };

        _grupoImposto.Descricao.requiredClass("form-control");
        if (data.Codigo == 0 && (data.Descricao == "" || data.Descricao == undefined)) {
            _grupoImposto.Descricao.requiredClass("form-control is-invalid");
            exibirMensagem(tipoMensagem.aviso, "Campos obrigatórios", "Favor informe a descrição do grupo de imposto antes de lançar os campos");
            return;
        }

        executarReST("GrupoImposto/LancarNovosDadosTabelaDeImposto", data, function (arg) {
            if (arg.Success) {

                var data = {
                    Codigo: arg.Data.Codigo,
                    Descricao: _grupoImposto.Descricao.val(),
                    NCM: _grupoImposto.NCM.val(),
                    Ativo: _grupoImposto.Ativo.val(),
                    EstadoOrigem: _grupoImposto.EstadoOrigem.val(),
                    EstadoDestino: _grupoImposto.EstadoDestino.val()
                };

                executarReST("GrupoImposto/BuscarPorCodigo", data, function (arg) {
                    if (arg.Success) {
                        var dataResultado = { Data: arg.Data };
                        PreencherObjetoKnout(_grupoImposto, dataResultado);
                        _pesquisaGrupoImposto.ExibirFiltros.visibleFade(false);
                        _grupoImposto.Atualizar.visible(true);
                        _grupoImposto.Cancelar.visible(true);
                        _grupoImposto.Excluir.visible(true);
                        _grupoImposto.Adicionar.visible(false);
                        recarregarGridItensImposto();
                        $("#divTabelaImposto").show();
                    } else {
                        exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                    }
                });
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function TipoCampoChange(e, sender) {
    if (_grupoImposto.TipoCampo.val() == EnumCamposGrupoImposto.AliquotaICMSInternaVenda || _grupoImposto.TipoCampo.val() == EnumCamposGrupoImposto.AliquotaICMSInterestadualVenda ||
        _grupoImposto.TipoCampo.val() == EnumCamposGrupoImposto.MVAVenda || _grupoImposto.TipoCampo.val() == EnumCamposGrupoImposto.ReducaoMVAVenda ||
        _grupoImposto.TipoCampo.val() == EnumCamposGrupoImposto.ReducaoBCICMSVenda || _grupoImposto.TipoCampo.val() == EnumCamposGrupoImposto.ReducaoBCPISVenda ||
        _grupoImposto.TipoCampo.val() == EnumCamposGrupoImposto.AliquotaPISVenda || _grupoImposto.TipoCampo.val() == EnumCamposGrupoImposto.ReducaoBCCOFINSVenda ||
        _grupoImposto.TipoCampo.val() == EnumCamposGrupoImposto.AliquotaCOFINSVenda || _grupoImposto.TipoCampo.val() == EnumCamposGrupoImposto.AliquotaFCPVenda ||
        _grupoImposto.TipoCampo.val() == EnumCamposGrupoImposto.AliquotaDifalVenda || _grupoImposto.TipoCampo.val() == EnumCamposGrupoImposto.AliquotaICMSCompra ||
        _grupoImposto.TipoCampo.val() == EnumCamposGrupoImposto.MVACompra || _grupoImposto.TipoCampo.val() == EnumCamposGrupoImposto.ReducaoBCICMSCompra ||
        _grupoImposto.TipoCampo.val() == EnumCamposGrupoImposto.ReducaoMVACompra || _grupoImposto.TipoCampo.val() == EnumCamposGrupoImposto.ReducaoBCPISCompra ||
        _grupoImposto.TipoCampo.val() == EnumCamposGrupoImposto.AliquotaPISCompra || _grupoImposto.TipoCampo.val() == EnumCamposGrupoImposto.ReducaoBCCOFINSCompra || _grupoImposto.TipoCampo.val() == EnumCamposGrupoImposto.AliquotaCOFINSCompra) {

        _grupoImposto.ValorCampoDecimal.val(0.00);
        _grupoImposto.ValorCampoCFOP.visible(false);
        _grupoImposto.ValorCampoCST.visible(false);
        _grupoImposto.ValorCampoCSTICMS.visible(false);
        _grupoImposto.ValorCampoDecimal.visible(true);
        _grupoImposto.ValorCampoString.visible(false);

    } else if (_grupoImposto.TipoCampo.val() == EnumCamposGrupoImposto.CFOPVenda || _grupoImposto.TipoCampo.val() == EnumCamposGrupoImposto.CFOPCompra || _grupoImposto.TipoCampo.val() == 0) {

        _grupoImposto.ValorCampoCFOP.val("");
        _grupoImposto.ValorCampoCFOP.visible(true);
        _grupoImposto.ValorCampoCST.visible(false);
        _grupoImposto.ValorCampoCSTICMS.visible(false);
        _grupoImposto.ValorCampoDecimal.visible(false);
        _grupoImposto.ValorCampoString.visible(false);

    } else if (_grupoImposto.TipoCampo.val() == EnumCamposGrupoImposto.CSTICMSVenda || _grupoImposto.TipoCampo.val() == EnumCamposGrupoImposto.CSTICMSCompra) {

        _grupoImposto.ValorCampoCSTICMS.val("");
        _grupoImposto.ValorCampoCFOP.visible(false);
        _grupoImposto.ValorCampoCST.visible(false);
        _grupoImposto.ValorCampoCSTICMS.visible(true);
        _grupoImposto.ValorCampoDecimal.visible(false);
        _grupoImposto.ValorCampoString.visible(false);

    } else if (_grupoImposto.TipoCampo.val() == EnumCamposGrupoImposto.CSTPISVenda || _grupoImposto.TipoCampo.val() == EnumCamposGrupoImposto.CSTCOFINSVenda ||
        _grupoImposto.TipoCampo.val() == EnumCamposGrupoImposto.CSTPISCompra || _grupoImposto.TipoCampo.val() == EnumCamposGrupoImposto.CSTCOFINSCompra) {

        _grupoImposto.ValorCampoCST.val("");
        _grupoImposto.ValorCampoCFOP.visible(false);
        _grupoImposto.ValorCampoCST.visible(true);
        _grupoImposto.ValorCampoCSTICMS.visible(false);
        _grupoImposto.ValorCampoDecimal.visible(false);
        _grupoImposto.ValorCampoString.visible(false);

    } else if (_grupoImposto.TipoCampo.val() == EnumCamposGrupoImposto.DiferencialAliquotaVenda || _grupoImposto.TipoCampo.val() == EnumCamposGrupoImposto.ObservacaoFiscal) {

        _grupoImposto.ValorCampoString.val("");
        _grupoImposto.ValorCampoCFOP.visible(false);
        _grupoImposto.ValorCampoCST.visible(false);
        _grupoImposto.ValorCampoCSTICMS.visible(false);
        _grupoImposto.ValorCampoDecimal.visible(false);
        _grupoImposto.ValorCampoString.visible(true);
    }
}

function AlterarValorCampoClick(e, sender) {
    if (_grupoImposto.Codigo.val() > 0 && _grupoImposto.ListaItensImposto.val().length > 0) {
        if (_grupoImposto.TipoCampo.val() > 0) {
            if (_grupoImposto.TipoCampo.val() == EnumCamposGrupoImposto.CFOPVenda || _grupoImposto.TipoCampo.val() == EnumCamposGrupoImposto.CFOPCompra) {
                if (_grupoImposto.ValorCampoCFOP.val().length == 4) {
                    AlteraCampoGridItensImposto(_grupoImposto.ValorCampoCFOP.val(), _grupoImposto.TipoCampo.val());
                } else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", "Campo CFOP inválido!");
            } else if (_grupoImposto.TipoCampo.val() == EnumCamposGrupoImposto.CSTPISVenda || _grupoImposto.TipoCampo.val() == EnumCamposGrupoImposto.CSTCOFINSVenda ||
                _grupoImposto.TipoCampo.val() == EnumCamposGrupoImposto.CSTPISCompra || _grupoImposto.TipoCampo.val() == EnumCamposGrupoImposto.CSTCOFINSCompra) {
                if (_grupoImposto.ValorCampoCST.val().length == 2) {
                    AlteraCampoGridItensImposto(_grupoImposto.ValorCampoCST.val(), _grupoImposto.TipoCampo.val());
                } else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", "Campo CST inválido!");
            } else if (_grupoImposto.TipoCampo.val() == EnumCamposGrupoImposto.CSTICMSVenda || _grupoImposto.TipoCampo.val() == EnumCamposGrupoImposto.CSTICMSCompra) {
                if (_grupoImposto.ValorCampoCSTICMS.val().length == 2 || _grupoImposto.ValorCampoCSTICMS.val().length == 3) {
                    AlteraCampoGridItensImposto(_grupoImposto.ValorCampoCSTICMS.val(), _grupoImposto.TipoCampo.val());
                } else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", "Campo CST/CSOSN inválido!");
            } else if (_grupoImposto.TipoCampo.val() == EnumCamposGrupoImposto.DiferencialAliquotaVenda || _grupoImposto.TipoCampo.val() == EnumCamposGrupoImposto.ObservacaoFiscal) {
                if (_grupoImposto.ValorCampoString.val().length == 1) {
                    AlteraCampoGridItensImposto(_grupoImposto.ValorCampoString.val(), _grupoImposto.TipoCampo.val());
                } else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", "Campo inválido!");
            } else if (_grupoImposto.TipoCampo.val() > 0) {
                AlteraCampoGridItensImposto(_grupoImposto.ValorCampoDecimal.val(), _grupoImposto.TipoCampo.val());
            }
        } else
            exibirMensagem(tipoMensagem.aviso, "Aviso", "Favor selecione um tipo de campo antes!");
    } else
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Favor selecione um grupo de imposto antes!");
}

function RetornoCopiarTabela(data) {
    _grupoImposto.CopiarTabelaImposto.codEntity(data.Codigo);
    _grupoImposto.CopiarTabelaImposto.val(data.Descricao);
    var codigoGrupoImposto = data.Codigo;
    exibirConfirmacao("Confirmação", "Realmente deseja duplicar a tabela de imposto selecionada " + data.Descricao + "?", function () {
        var dataEnvio = {
            Codigo: _grupoImposto.Codigo.val(),
            Descricao: _grupoImposto.Descricao.val(),
            NCM: _grupoImposto.NCM.val(),
            Ativo: _grupoImposto.Ativo.val(),
            EstadoOrigem: _grupoImposto.EstadoOrigem.val(),
            EstadoDestino: _grupoImposto.EstadoDestino.val(),
            CodigoGrupoImposto: codigoGrupoImposto
        };

        _grupoImposto.Descricao.requiredClass("form-control");
        if (dataEnvio.Codigo == 0 && (dataEnvio.Descricao == "" || dataEnvio.Descricao == undefined)) {
            _grupoImposto.Descricao.requiredClass("form-control is-invalid");
            exibirMensagem(tipoMensagem.aviso, "Campos obrigatórios", "Favor informe a descrição do grupo de imposto antes de lançar os campos");
            return;
        }

        executarReST("GrupoImposto/DuplicarTabelaDeImposto", dataEnvio, function (arg) {
            if (arg.Success) {

                var dataEnvio = {
                    Codigo: arg.Data.Codigo,
                    Descricao: _grupoImposto.Descricao.val(),
                    NCM: _grupoImposto.NCM.val(),
                    Ativo: _grupoImposto.Ativo.val(),
                    EstadoOrigem: _grupoImposto.EstadoOrigem.val(),
                    EstadoDestino: _grupoImposto.EstadoDestino.val()
                };

                executarReST("GrupoImposto/BuscarPorCodigo", dataEnvio, function (arg) {
                    if (arg.Success) {
                        var dataResultado = { Data: arg.Data };
                        PreencherObjetoKnout(_grupoImposto, dataResultado);
                        _pesquisaGrupoImposto.ExibirFiltros.visibleFade(false);
                        _grupoImposto.Atualizar.visible(true);
                        _grupoImposto.Cancelar.visible(true);
                        _grupoImposto.Excluir.visible(true);
                        _grupoImposto.Adicionar.visible(false);
                        recarregarGridItensImposto();
                        $("#divTabelaImposto").show();
                    } else {
                        exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                    }
                });
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
    LimparCampoEntity(_grupoImposto.CopiarTabelaImposto);
}

function EstadoOrigemChange(e, data) {
    if ((_grupoImposto.EstadoOrigem.val() != "" || _grupoImposto.EstadoDestino.val() != "") && _grupoImposto.Codigo.val() > 0) {
        preencherListasSelecao();
        var data = {
            Codigo: _grupoImposto.Codigo.val(),
            Descricao: _grupoImposto.Descricao.val(),
            NCM: _grupoImposto.NCM.val(),
            Ativo: _grupoImposto.Ativo.val(),
            EstadoOrigem: _grupoImposto.EstadoOrigem.val(),
            EstadoDestino: _grupoImposto.EstadoDestino.val(),
            ListaItensImposto: _grupoImposto.ListaItensImposto.val()
        };

        preencherListasSelecao();

        executarReST("GrupoImposto/BuscarPorCodigo", data, function (arg) {
            if (arg.Success) {
                var dataResultado = { Data: arg.Data };
                PreencherObjetoKnout(_grupoImposto, dataResultado);
                _pesquisaGrupoImposto.ExibirFiltros.visibleFade(false);
                _grupoImposto.Atualizar.visible(true);
                _grupoImposto.Cancelar.visible(true);
                _grupoImposto.Excluir.visible(true);
                _grupoImposto.Adicionar.visible(false);
                recarregarGridItensImposto();
                $("#divTabelaImposto").show();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    } else
        $("#divTabelaImposto").hide();
}

function adicionarClick(e, sender) {
    preencherListasSelecao();
    Salvar(e, "GrupoImposto/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridGrupoImposto.CarregarGrid();
                limparCamposGrupoImposto();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    preencherListasSelecao();
    Salvar(e, "GrupoImposto/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridGrupoImposto.CarregarGrid();
                limparCamposGrupoImposto();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o grupo de imposto " + _grupoImposto.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_grupoImposto, "GrupoImposto/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridGrupoImposto.CarregarGrid();
                limparCamposGrupoImposto();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposGrupoImposto();
}

//*******MÉTODOS*******

function AlteraCampoGridItensImposto(valor, tipoCampo) {

    var itensImpostoGrid = _gridItensImposto.BuscarRegistros();

    iniciarRequisicao();

    setTimeout(function () {

        var codigoItemEdicao;
        for (var i = 0; i < itensImpostoGrid.length; i++) {
            codigoItemEdicao = itensImpostoGrid[i].Codigo;
            if (tipoCampo == EnumCamposGrupoImposto.AliquotaICMSInternaVenda)
                itensImpostoGrid[i].AliquotaICMSInternaVenda = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + Globalize.format(valor, "n2");
            else if (tipoCampo == EnumCamposGrupoImposto.AliquotaICMSInterestadualVenda)
                itensImpostoGrid[i].AliquotaICMSInterestadualVenda = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + Globalize.format(valor, "n2");
            else if (tipoCampo == EnumCamposGrupoImposto.MVAVenda)
                itensImpostoGrid[i].MVAVenda = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + Globalize.format(valor, "n2");
            else if (tipoCampo == EnumCamposGrupoImposto.CSTICMSVenda)
                itensImpostoGrid[i].CSTICMSVenda = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + valor;
            else if (tipoCampo == EnumCamposGrupoImposto.CFOPVenda)
                itensImpostoGrid[i].CFOPVenda = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + valor;
            else if (tipoCampo == EnumCamposGrupoImposto.ReducaoMVAVenda)
                itensImpostoGrid[i].ReducaoMVAVenda = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + Globalize.format(valor, "n2");
            else if (tipoCampo == EnumCamposGrupoImposto.DiferencialAliquotaVenda)
                itensImpostoGrid[i].DiferencialAliquotaVenda = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + valor;
            else if (tipoCampo == EnumCamposGrupoImposto.ReducaoBCICMSVenda)
                itensImpostoGrid[i].ReducaoBCICMSVenda = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + Globalize.format(valor, "n2");
            else if (tipoCampo == EnumCamposGrupoImposto.CSTPISVenda)
                itensImpostoGrid[i].CSTPISVenda = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + valor;
            else if (tipoCampo == EnumCamposGrupoImposto.ReducaoBCPISVenda)
                itensImpostoGrid[i].ReducaoBCPISVenda = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + Globalize.format(valor, "n2");
            else if (tipoCampo == EnumCamposGrupoImposto.AliquotaPISVenda)
                itensImpostoGrid[i].AliquotaPISVenda = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + Globalize.format(valor, "n2");
            else if (tipoCampo == EnumCamposGrupoImposto.CSTCOFINSVenda)
                itensImpostoGrid[i].CSTCOFINSVenda = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + valor;
            else if (tipoCampo == EnumCamposGrupoImposto.ReducaoBCCOFINSVenda)
                itensImpostoGrid[i].ReducaoBCCOFINSVenda = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + Globalize.format(valor, "n2");
            else if (tipoCampo == EnumCamposGrupoImposto.AliquotaCOFINSVenda)
                itensImpostoGrid[i].AliquotaCOFINSVenda = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + Globalize.format(valor, "n2");
            else if (tipoCampo == EnumCamposGrupoImposto.AliquotaFCPVenda)
                itensImpostoGrid[i].AliquotaFCPVenda = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + Globalize.format(valor, "n2");
            else if (tipoCampo == EnumCamposGrupoImposto.AliquotaDifalVenda)
                itensImpostoGrid[i].AliquotaDifalVenda = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + Globalize.format(valor, "n2");
            else if (tipoCampo == EnumCamposGrupoImposto.AliquotaICMSCompra)
                itensImpostoGrid[i].AliquotaICMSCompra = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + Globalize.format(valor, "n2");
            else if (tipoCampo == EnumCamposGrupoImposto.MVACompra)
                itensImpostoGrid[i].MVACompra = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + Globalize.format(valor, "n2");
            else if (tipoCampo == EnumCamposGrupoImposto.CSTICMSCompra)
                itensImpostoGrid[i].CSTICMSCompra = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + valor;
            else if (tipoCampo == EnumCamposGrupoImposto.CFOPCompra)
                itensImpostoGrid[i].CFOPCompra = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + valor;
            else if (tipoCampo == EnumCamposGrupoImposto.ReducaoBCICMSCompra)
                itensImpostoGrid[i].ReducaoBCICMSCompra = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + Globalize.format(valor, "n2");
            else if (tipoCampo == EnumCamposGrupoImposto.ReducaoMVACompra)
                itensImpostoGrid[i].ReducaoMVACompra = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + Globalize.format(valor, "n2");
            else if (tipoCampo == EnumCamposGrupoImposto.CSTPISCompra)
                itensImpostoGrid[i].CSTPISCompra = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + valor;
            else if (tipoCampo == EnumCamposGrupoImposto.ReducaoBCPISCompra)
                itensImpostoGrid[i].ReducaoBCPISCompra = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + Globalize.format(valor, "n2");
            else if (tipoCampo == EnumCamposGrupoImposto.AliquotaPISCompra)
                itensImpostoGrid[i].AliquotaPISCompra = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + Globalize.format(valor, "n2");
            else if (tipoCampo == EnumCamposGrupoImposto.CSTCOFINSCompra)
                itensImpostoGrid[i].CSTCOFINSCompra = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + valor;
            else if (tipoCampo == EnumCamposGrupoImposto.ReducaoBCCOFINSCompra)
                itensImpostoGrid[i].ReducaoBCCOFINSCompra = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + Globalize.format(valor, "n2");
            else if (tipoCampo == EnumCamposGrupoImposto.AliquotaCOFINSCompra)
                itensImpostoGrid[i].AliquotaCOFINSCompra = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + Globalize.format(valor, "n2");
            else if (tipoCampo == EnumCamposGrupoImposto.ObservacaoFiscal)
                itensImpostoGrid[i].ObservacaoFiscal = "<span class='spnTipoValorItem' data-codigo-item='" + codigoItemEdicao + "'></span> " + valor;
        }

        _gridItensImposto.CarregarGrid(itensImpostoGrid);
        finalizarRequisicao();
    }, 200);
}

function recarregarGridItensImposto() {
    var data = new Array();

    $.each(_grupoImposto.ListaItensImposto.val(), function (i, item) {
        var obj = new Object();

        obj.Codigo = item.Codigo;
        obj.CodigoGrupoImposto = item.CodigoGrupoImposto;
        obj.UFOrigem = item.UFOrigem;
        obj.UFDestino = item.UFDestino;
        obj.AliquotaICMSInternaVenda = item.AliquotaICMSInternaVenda;
        obj.AliquotaICMSInterestadualVenda = item.AliquotaICMSInterestadualVenda;
        obj.MVAVenda = item.MVAVenda;
        obj.CSTICMSVenda = item.CSTICMSVenda;
        obj.CFOPVenda = item.CFOPVenda;
        obj.ReducaoMVAVenda = item.ReducaoMVAVenda;
        obj.DiferencialAliquotaVenda = item.DiferencialAliquotaVenda;
        obj.Atividade = item.Atividade;
        obj.ReducaoBCICMSVenda = item.ReducaoBCICMSVenda;
        obj.CSTPISVenda = item.CSTPISVenda;
        obj.ReducaoBCPISVenda = item.ReducaoBCPISVenda;
        obj.AliquotaPISVenda = item.AliquotaPISVenda;
        obj.CSTCOFINSVenda = item.CSTCOFINSVenda;
        obj.ReducaoBCCOFINSVenda = item.ReducaoBCCOFINSVenda;
        obj.AliquotaCOFINSVenda = item.AliquotaCOFINSVenda;
        obj.AliquotaFCPVenda = item.AliquotaFCPVenda;
        obj.AliquotaDifalVenda = item.AliquotaDifalVenda;
        obj.AliquotaICMSCompra = item.AliquotaICMSCompra;
        obj.MVACompra = item.MVACompra;
        obj.CSTICMSCompra = item.CSTICMSCompra;
        obj.CFOPCompra = item.CFOPCompra;
        obj.ReducaoBCICMSCompra = item.ReducaoBCICMSCompra;
        obj.ReducaoMVACompra = item.ReducaoMVACompra;
        obj.CSTPISCompra = item.CSTPISCompra;
        obj.ReducaoBCPISCompra = item.ReducaoBCPISCompra;
        obj.AliquotaPISCompra = item.AliquotaPISCompra;
        obj.CSTCOFINSCompra = item.CSTCOFINSCompra;
        obj.ReducaoBCCOFINSCompra = item.ReducaoBCCOFINSCompra;
        obj.AliquotaCOFINSCompra = item.AliquotaCOFINSCompra;
        obj.ObservacaoFiscal = item.ObservacaoFiscal;

        data.push(obj);
    });

    _gridItensImposto.CarregarGrid(data);
}

function buscarGrupoImpostos() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarGrupoImposto, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridGrupoImposto = new GridView(_pesquisaGrupoImposto.Pesquisar.idGrid, "GrupoImposto/Pesquisa", _pesquisaGrupoImposto, menuOpcoes, null);
    _gridGrupoImposto.CarregarGrid();
}

function editarGrupoImposto(grupoImpostoGrid) {
    limparCamposGrupoImposto();
    _grupoImposto.Codigo.val(grupoImpostoGrid.Codigo);
    _grupoImposto.EstadoOrigem.val("");
    _grupoImposto.EstadoDestino.val("");
    BuscarPorCodigo(_grupoImposto, "GrupoImposto/BuscarPorCodigo", function (arg) {
        _pesquisaGrupoImposto.ExibirFiltros.visibleFade(false);
        _grupoImposto.Atualizar.visible(true);
        _grupoImposto.Cancelar.visible(true);
        _grupoImposto.Excluir.visible(true);
        _grupoImposto.Adicionar.visible(false);
        recarregarGridItensImposto();
        if (_grupoImposto.ListaItensImposto.val().length > 0)
            $("#divTabelaImposto").show();
        else
            $("#divTabelaImposto").hide();
    }, null);
}

function limparCamposGrupoImposto() {
    _grupoImposto.Atualizar.visible(false);
    _grupoImposto.Cancelar.visible(false);
    _grupoImposto.Excluir.visible(false);
    _grupoImposto.Adicionar.visible(true);
    LimparCampos(_grupoImposto);
    _grupoImposto.EstadoOrigem.val("");
    _grupoImposto.EstadoDestino.val("");
    $("#divTabelaImposto").hide();
}

function preencherListasSelecao() {
    _grupoImposto.ListaItensImposto.list = new Array();

    var itens = new Array();

    //$.each(_grupoImposto.ItensImposto.basicTable.BuscarRegistros(), function (i, item) {
    $.each(_gridItensImposto.BuscarRegistros(), function (i, item) {
        if (item.AliquotaICMSInternaVenda.toString().toLowerCase().indexOf("span") >= 0) {

            item.AliquotaICMSInternaVenda = Globalize.parseFloat(item.AliquotaICMSInternaVenda.split("</span>")[1].trim());
            item.AliquotaICMSInterestadualVenda = Globalize.parseFloat(item.AliquotaICMSInterestadualVenda.split("</span>")[1].trim());
            item.MVAVenda = Globalize.parseFloat(item.MVAVenda.split("</span>")[1].trim());
            item.CSTICMSVenda = item.CSTICMSVenda.split("</span>")[1].trim();
            item.CFOPVenda = item.CFOPVenda.split("</span>")[1].trim();
            item.ReducaoMVAVenda = Globalize.parseFloat(item.ReducaoMVAVenda.split("</span>")[1].trim());
            item.DiferencialAliquotaVenda = item.DiferencialAliquotaVenda.split("</span>")[1].trim();
            item.ReducaoBCICMSVenda = Globalize.parseFloat(item.ReducaoBCICMSVenda.split("</span>")[1].trim());
            item.CSTPISVenda = item.CSTPISVenda.split("</span>")[1].trim();
            item.ReducaoBCPISVenda = Globalize.parseFloat(item.ReducaoBCPISVenda.split("</span>")[1].trim());
            item.AliquotaPISVenda = Globalize.parseFloat(item.AliquotaPISVenda.split("</span>")[1].trim());
            item.CSTCOFINSVenda = item.CSTCOFINSVenda.split("</span>")[1].trim();
            item.ReducaoBCCOFINSVenda = Globalize.parseFloat(item.ReducaoBCCOFINSVenda.split("</span>")[1].trim());
            item.AliquotaCOFINSVenda = Globalize.parseFloat(item.AliquotaCOFINSVenda.split("</span>")[1].trim());
            item.AliquotaFCPVenda = Globalize.parseFloat(item.AliquotaFCPVenda.split("</span>")[1].trim());
            item.AliquotaDifalVenda = Globalize.parseFloat(item.AliquotaDifalVenda.split("</span>")[1].trim());
            item.AliquotaICMSCompra = Globalize.parseFloat(item.AliquotaICMSCompra.split("</span>")[1].trim());
            item.MVACompra = Globalize.parseFloat(item.MVACompra.split("</span>")[1].trim());
            item.CSTICMSCompra = item.CSTICMSCompra.split("</span>")[1].trim();
            item.CFOPCompra = item.CFOPCompra.split("</span>")[1].trim();
            item.ReducaoBCICMSCompra = Globalize.parseFloat(item.ReducaoBCICMSCompra.split("</span>")[1].trim());
            item.ReducaoMVACompra = Globalize.parseFloat(item.ReducaoMVACompra.split("</span>")[1].trim());
            item.CSTPISCompra = item.CSTPISCompra.split("</span>")[1].trim();
            item.ReducaoBCPISCompra = Globalize.parseFloat(item.ReducaoBCPISCompra.split("</span>")[1].trim());
            item.AliquotaPISCompra = Globalize.parseFloat(item.AliquotaPISCompra.split("</span>")[1].trim());
            item.CSTCOFINSCompra = item.CSTCOFINSCompra.split("</span>")[1].trim();
            item.ReducaoBCCOFINSCompra = Globalize.parseFloat(item.ReducaoBCCOFINSCompra.split("</span>")[1].trim());
            item.AliquotaCOFINSCompra = Globalize.parseFloat(item.AliquotaCOFINSCompra.split("</span>")[1].trim());
            item.ObservacaoFiscal = item.ObservacaoFiscal.split("</span>")[1].trim();

        }
        itens.push({ ItemImposto: item });
    });

    _grupoImposto.ListaItensImposto.val(JSON.stringify(itens))
}