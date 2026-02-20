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
/// <reference path="../../Enumeradores/EnumTipoLayoutEDI.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/LayoutEDI.js" />
/// <reference path="../../Consultas/Empresa.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridImportarEDI;
var _importarEDI;
var _pesquisaImportarEDI;

var PesquisaImportarEDI = function () {
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo Operação:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoPessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo Pessoa Remetente:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridImportarEDI.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() === true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var ImportarEDI = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo Operação *:", idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(true) });
    this.GrupoPessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo Pessoa Remetente *:", idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(true) });
    this.LayoutEDI = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Layout EDI:", idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(false) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Empresa:", idBtnSearch: guid(), visible: ko.observable(true), required: ko.observable(true) });    
    this.Arquivo = PropertyEntity({ type: types.file, val: ko.observable(""), text: "Anexo:", enable: ko.observable(true), file: null, name: ko.pureComputed(function () { return self.Arquivo.val().replace('C:\\fakepath\\', '') }) });

    this.NomeArquivo = PropertyEntity({ type: types.string, text: "Arquivo:" });
};

var CRUDImportarEDI = function () {
    this.Adicionar = PropertyEntity({ eventClick: ProcessarClick, type: types.event, text: "Processar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.BuscarViaWs = PropertyEntity({ eventClick: ImportarViaWs, type: types.event, text: "Buscar via WS", visible: ko.observable(true) });
};

//*******EVENTOS*******


function loadImportarEDI() {
    _importarEDI = new ImportarEDI();
    KoBindings(_importarEDI, "knockoutImportarEDI");

    _crudImportarEDI = new CRUDImportarEDI();
    KoBindings(_crudImportarEDI, "knockoutCRUDImportarEDI");

    _pesquisaImportarEDI = new PesquisaImportarEDI();
    KoBindings(_pesquisaImportarEDI, "knockoutPesquisaImportarEDI", false, _pesquisaImportarEDI.Pesquisar.id);

    new BuscarTiposOperacao(_importarEDI.TipoOperacao);
    new BuscarTiposOperacao(_pesquisaImportarEDI.TipoOperacao);
    new BuscarGruposPessoas(_importarEDI.GrupoPessoa);
    new BuscarGruposPessoas(_pesquisaImportarEDI.GrupoPessoa);
    new BuscarLayoutsEDI(_importarEDI.LayoutEDI, null, null, null, null, [EnumTipoLayoutEDI.Pedido]);
    new BuscarEmpresa(_importarEDI.Empresa);

    buscarImportacoesEDI();
}
function ImportarViaWs() {

    if (!ValidarCamposObrigatorios(_importarEDI)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
        return;
    }

    exibirConfirmacao("Confirmação", "Realmente deseja importar via WS?", function () {
        executarReST("ImportarEDI/BuscarViaWs", { TipoOperacao: _importarEDI.TipoOperacao.codEntity(), GrupoPessoa: _importarEDI.GrupoPessoa.codEntity(), LayoutEDI: _importarEDI.LayoutEDI.codEntity(), Empresa: _importarEDI.Empresa.codEntity()  }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    limparCampos();
                    exibirMensagem(tipoMensagem.ok)
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        });
    });

}
function ProcessarClick(e, sender) {
    

    if (!ValidarCamposObrigatorios(_importarEDI)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
        return;
    }
    var file = document.getElementById(_importarEDI.Arquivo.id);

    var formData = new FormData();
    formData.append("upload", file.files[0]);


    enviarArquivo("ImportarEDI/Processar?callback=?", { TipoOperacao: _importarEDI.TipoOperacao.codEntity(), GrupoPessoa: _importarEDI.GrupoPessoa.codEntity(), LayoutEDI: _importarEDI.LayoutEDI.codEntity() }, formData, function (arg) {
        if (arg.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso", "Importação realizada com sucesso!");
            limparCampos();
        } else {
            exibirMensagem(tipoMensagem.aviso, "Atenção", arg.Msg);
        }
    })
}

function cancelarClick(e) {
    limparCamposNavio();
}

function limparCampos() {
    LimparCampo(_importarEDI.TipoOperacao);
    LimparCampo(_importarEDI.GrupoPessoa);
    LimparCampo(_importarEDI.LayoutEDI);
    LimparCampo(_importarEDI.Arquivo);
    LimparCampo(_importarEDI.NomeArquivo);
}

//*******MÉTODOS*******

function buscarImportacoesEDI() {
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();

    _gridImportarEDI = new GridView(_pesquisaImportarEDI.Pesquisar.idGrid, "ImportarEDI/Pesquisa", _pesquisaImportarEDI, menuOpcoes, null);
    _gridImportarEDI.CarregarGrid();
}