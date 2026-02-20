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
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/Localidade.js" />

var _gridInformarNFSPendentes;
var _pesquisaInformarNFSPendentes;
var _NFSManual;

var PesquisaInformarNFSPendentes = function () {

    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid() });
    this.SituacaoNFSManual = PropertyEntity({ text: "Situação:", required: false, enable: ko.observable(false) });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridInformarNFSPendentes.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

}

var NFSManual = function () {
    this.CodigoCargaNFS = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Numero = PropertyEntity({ text: "*Número: ", required: true, getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: '' } });
    this.ValorFrete = PropertyEntity({ text: "*Valor da Prestação:", getType: typesKnockout.decimal, required: true });
    this.ValorBaseCalculo = PropertyEntity({ text: "*Base de Calculo:", getType: typesKnockout.decimal, required: true });
    this.AliquotaISS = PropertyEntity({ text: "*Aliquota ISS:", getType: typesKnockout.decimal, required: true });
    this.ValorISS = PropertyEntity({ text: "*Valor ISS:", getType: typesKnockout.decimal, required: true });
    this.ValorRetido = PropertyEntity({ text: "Valor Retido:", getType: typesKnockout.decimal, required: false });
    this.ValorLiquido = PropertyEntity({ text: "Valor Retido:", getType: typesKnockout.decimal, required: false, enable: ko.observable(false) });
    this.Tomador = PropertyEntity({ text: "Tomador:",  required: false, enable : ko.observable(false) });
    this.LocalidadePrestacao = PropertyEntity({ text: "Local da Prestação:", required: false, enable: ko.observable(false) });
    this.MotivoRejeicao = PropertyEntity({ text: "Motivo da Rejeição:", required: false, enable: ko.observable(false), visible: ko.observable(false) });
    //public virtual DateTime DataInformacaoManual { get; set; }
    //public virtual string ImagemNFS { get; set; }
    //public virtual string MotivoRejeicao { get; set; }

    this.SalvarNFSManual = PropertyEntity({ eventClick: salvarNFSManualClick, type: types.event, text: "Salvar", visible: ko.observable(true) });
}


function CarregarInformarNFSPendentes() {
    var menuOpcoes = new Object();
    menuOpcoes.opcoes = new Array();
    menuOpcoes.tipo = TypeOptionMenu.list;
    menuOpcoes.opcoes.push({ descricao: "Informar NFS", id: guid(), evento: "onclick", metodo: abrirModalInformarNFSClick, tamanho: "7" });
    
    _gridInformarNFSPendentes = new GridView(_pesquisaInformarNFSPendentes.Pesquisar.idGrid, "CargaNFS/BuscarNFSPendentes", _pesquisaInformarNFSPendentes, menuOpcoes, null, 10, null);
    _gridInformarNFSPendentes.CarregarGrid();
}

function loadInformarNFSPendentes() {
    _pesquisaInformarNFSPendentes = new PesquisaInformarNFSPendentes();
    KoBindings(_pesquisaInformarNFSPendentes, "knockoutPesquisaInformarNFSPendentes", false, _pesquisaInformarNFSPendentes.Pesquisar.id);

    CarregarInformarNFSPendentes();

    new BuscarTransportadores(_pesquisaInformarNFSPendentes.Empresa);
    
}

function abrirModalInformarNFSClick(e) {
    _NFSManual = new NFSManual();
    _NFSManual.CodigoCargaNFS.val(e.Codigo);
    KoBindings(_NFSManual, "knoutNFSManual");
    Global.abrirModal('divModalNFSManual');
}

function salvarNFSManualClick(e) {
    Salvar(_NFSManual, "CargaNFS/EnviarNFSManual", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "cadastrado");
                _gridInformarNFSPendentes.CarregarGrid();
                Global.fecharModal('divModalNFSManual');
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}