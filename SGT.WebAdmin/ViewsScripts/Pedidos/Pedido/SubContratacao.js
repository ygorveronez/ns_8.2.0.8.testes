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
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/Pais.js" />
/// <reference path="../../Enumeradores/EnumRequisitanteColeta.js" />
/// <reference path="../../Enumeradores/EnumTipoPagamento.js" />
/// <reference path="../../Enumeradores/EnumTipoTomador.js" />
/// <reference path="../../Consultas/Fronteira.js" />
/// <reference path="../../Consultas/TipoDeCarga.js" />
/// <reference path="../../Consultas/ClienteOutroEndereco.js" />
/// <reference path="Pedido.js" />
/// <reference path="OrigemDestino.js" />



//*******MAPEAMENTO KNOUCKOUT*******

var _cteSubContratacao;
var _gridCTeSubcontratacao;
var CTeSubContratacao = function () {
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pedidos.Pedido.Transportadora.getRequiredFieldDescription(), idBtnSearch: guid(), required: true });
    this.Chave = PropertyEntity({ val: ko.observable(""), enable: ko.observable(true), text: Localization.Resources.Pedidos.Pedido.Chave.getRequiredFieldDescription(), def: "", maxlength: 54, required: true, idGrid : guid() });
    this.Adicionar = PropertyEntity({ eventClick: adicionarCTeClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
}

var CTeSubContratacaoMap = function () {
    this.Cliente = PropertyEntity({ val: 0, def: 0 });
    this.RazaoSocial = PropertyEntity({  val: "" });
    this.Chave = PropertyEntity({ val: "", def: "" });
}


//*******EVENTOS*******

function loadSubContratacao() {
    _cteSubContratacao = new CTeSubContratacao();
    KoBindings(_cteSubContratacao, "knockoutSubContratacao");
    new BuscarClientes(_cteSubContratacao.Cliente, null, true);
    
    var remover = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), evento: "onclick", metodo: excluirCTeClick, tamanho: "15", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, descricao: Localization.Resources.Gerais.Geral.Remover, tamanho: 15, opcoes: [remover] };
    var header = [{ data: "Código", visible: false },
        { data: "RazaoSocial", title: Localization.Resources.Pedidos.Pedido.RazaoSocial, width: "40%", className: "text-align-left" },
        { data: "Chave", title: Localization.Resources.Pedidos.Pedido.ChaveCTE, width: "45%", className: "text-align-left" }
    ];
    _gridCTeSubcontratacao = new BasicDataTable(_cteSubContratacao.Chave.idGrid, header, menuOpcoes);
    recarregarSubContratacao();

}

function excluirCTeClick(data) {

    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Pedidos.RealmenteDesejaExcluirOcte.format(data.Chave), function () {

        $.each(_pedido.CTEsSubContratacao.list, function (i, cte) {
            if (cte.Chave.val.replace(/ /g, "") == data.Chave.replace(/ /g, "")) {
                _pedido.CTEsSubContratacao.list.splice(i, 1);
                return false;
            }
        });

        recarregarSubContratacao();
    });

}

function adicionarCTeClick(e) {
    var tudoCerto = ValidarCamposObrigatorios(_cteSubContratacao);
    if (tudoCerto) {
        var chave = _cteSubContratacao.Chave.val().replace(/ /g, '');
        if (chave.length == 44) {
            var existe = false;
            $.each(_pedido.CTEsSubContratacao.list, function (i, cte) {
                if (cte.Chave.val.replace(/ /g, "") == chave) {
                    existe = true;
                    return false;
                }
            });
            if (!existe) {
                var subCTe = new CTeSubContratacaoMap();
                subCTe.Chave.val = chave;
                subCTe.Cliente.val = _cteSubContratacao.Cliente.codEntity();
                subCTe.RazaoSocial.val = _cteSubContratacao.Cliente.val();
                _pedido.CTEsSubContratacao.list.push(subCTe);
                recarregarSubContratacao();
            } else {
                exibirMensagem(Localization.Resources.Gerais.Geral.Aviso, Localization.Resources.Pedidos.Pedido.CteJaInformado, Localization.Resources.Pedidos.CteJaInformadaParaEstePedido.format(_cteSubContratacao.Chave.val()));
            }
            LimparCampos(_cteSubContratacao);
        } else {
            exibirMensagem(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Pedidos.Pedido.ChaveInvalida, Localization.Resources.Pedidos.Pedido.ChaveCTeDeveContarNoMinimo);
        }
 
    } else {
        exibirMensagem(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
}

function recarregarSubContratacao() {
    var data = new Array();
    $.each(_pedido.CTEsSubContratacao.list, function (i, cte) {
        var obj = new Object();
        obj.Codigo = cte.Chave.val;
        obj.RazaoSocial = cte.RazaoSocial.val;
        obj.Chave = cte.Chave.val;
        data.push(obj);
    });
    _gridCTeSubcontratacao.CarregarGrid(data);
}

