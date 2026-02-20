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
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="Transportador.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridFilial;
var _filial;

var Filial = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Transportadores.Transportador.TransportadoraFilial, idBtnSearch: guid(), required: true });
    this.Localidade = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.Localidade.getFieldDescription() });
    this.Adicionar = PropertyEntity({ eventClick: adicionarFilialClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
}


//*******EVENTOS*******

function loadFilial() {

    _filial = new Filial();
    KoBindings(_filial, "knockoutFiliais");

    new BuscarTransportadores(_filial.Empresa, callbackEmpresa);

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: excluirFilialTransportadorClick }] };

    var header = [{ data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Transportadores.Transportador.RazaoSocial, width: "50%" },
        { data: "Localidade", title: Localization.Resources.Transportadores.Transportador.Localidade, width: "30%" }
    ];

    _gridFilial = new BasicDataTable(_filial.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    recarregarGridFilial();
}

function callbackEmpresa(dataRow){
    _filial.Empresa.val(dataRow.Descricao);
    _filial.Empresa.codEntity(dataRow.Codigo);
    _filial.Localidade.val(dataRow.Localidade);
}

function recarregarGridFilial() {
    
    var data = new Array();

    $.each(_transportador.Filiais.list, function (i, filial) {
        var filialGrid = new Object();

        filialGrid.Codigo = filial.Empresa.codEntity;
        filialGrid.Descricao = filial.Empresa.val;
        filialGrid.Localidade = filial.Localidade.val;

        data.push(filialGrid);
    });

    _gridFilial.CarregarGrid(data);
}


function excluirFilialTransportadorClick(data) {
    for (var i = 0; i < _transportador.Filiais.list.length; i++) {
        filialExcluir = _transportador.Filiais.list[i];
        if (data.Codigo == filialExcluir.Empresa.codEntity)
            _transportador.Filiais.list.splice(i, 1);
    }
    recarregarGridFilial();
}

function adicionarFilialClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_filial);

    if (valido) {
        var existe = false;
        $.each(_transportador.Filiais.list, function (i, filial) {
            if (filial.Empresa.codEntity == _filial.Empresa.codEntity()) {
                existe = true;
                return;
            }
        });

        if (existe) {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Transportadores.Transportador.TransportadorExistente, Localization.Resources.Transportadores.Transportador.TransportadorCadastradoComoFilial.format(_filial.Empresa.val()));
            return;
        }

        _transportador.Filiais.list.push(SalvarListEntity(_filial));

        recarregarGridFilial();

        $("#" + _filial.Empresa.id).focus();

        limparCamposFilial();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Transportadores.Transportador.CamposObrigatorios, Localization.Resources.Transportadores.Transportador.InformeCamposObrigatorios);
    }
}

function limparCamposFilial() {
    
    LimparCampos(_filial);
}