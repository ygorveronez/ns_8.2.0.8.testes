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
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="Chamado.js" />
/// <reference path="ChamadoEtapa.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _chamadoAnexos;
var _gridAnexos;

var AnexoMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: ko.observable(0), def: ko.observable(0), getType: typesKnockout.int });
    this.DescricaoAnexo = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.Arquivo = PropertyEntity({ type: types.map, val: ko.observable("") });
}

var ChamadoAnexos = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.DescricaoAnexo = PropertyEntity({ text: "*Descrição Anexo:", getType: typesKnockout.string, maxlength: 500, required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "*Anexo:", val: ko.observable(""), required: ko.observable(true), visible: ko.observable(true) });

    this.SalvarAnexo = PropertyEntity({ eventClick: SalvarAnexoClick, type: types.event, text: "Salvar Anexo", visible: ko.observable(true), enable: ko.observable(true) });

    this.AnexosChamado = PropertyEntity({ type: types.local, id: guid() });
}

//*******EVENTOS*******

function loadChamadoAnexos() {
    _chamadoAnexos = new ChamadoAnexos();
    KoBindings(_chamadoAnexos, "knockoutAnexosChamado");

    var baixarAnexo = { descricao: "Baixar", id: guid(), metodo: BaixarAnexoClick, icone: "" };
    var excluirAnexo = { descricao: "Excluir", id: guid(), metodo: ExcluirAnexoClick, icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 15, opcoes: [baixarAnexo, excluirAnexo] };

    var header = [{ data: "Codigo", visible: false },
                  { data: "CodigoChamado", visible: false },
                  { data: "DescricaoAnexo", title: "Descrição", width: "40%" },
                  { data: "Arquivo", title: "Caminho / Nome Arquivo", width: "50%" }];

    _gridAnexos = new BasicDataTable(_chamadoAnexos.AnexosChamado.id, header, menuOpcoes);

    recarregarGridListaAnexos();
}

function BaixarAnexoClick(data) {
    if (VerificaNovosAnexosLancados(data.Codigo))
        executarDownload("Chamado/DownloadAnexo", { CodigoAnexo: data.Codigo });
    else
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Só é possível baixar os Anexos após gravar o Chamado");
}

function ExcluirAnexoClick(data) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o anexo " + data.DescricaoAnexo + "?", function () {
        $.each(_chamado.ListaAnexos.list, function (i, listaAnexos) {
            if (data.Codigo == listaAnexos.Codigo.val) {
                _chamado.ListaAnexos.list.splice(i, 1);

                if (VerificaNovosAnexosLancados(data.Codigo)) {
                    var listaAnexosExcluidos = new AnexoMap();
                    listaAnexosExcluidos.Codigo.val = data.Codigo;
                    _chamado.ListaAnexosExcluidos.list.push(listaAnexosExcluidos);
                }

                return false;
            }
        });

        $.each(_chamado.ListaAnexosNovos.list, function (i, listaAnexosNovos) {
            if (data.Codigo == listaAnexosNovos.Codigo.val) {
                _chamado.ListaAnexosNovos.list.splice(i, 1);

                return false;
            }
        });

        recarregarGridListaAnexos();
    });
}

function SalvarAnexoClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_chamadoAnexos);
    _chamadoAnexos.Arquivo.requiredClass("form-control");

    if (_chamadoAnexos.Arquivo.val() == "") {
        valido = false;
        _chamadoAnexos.Arquivo.requiredClass("form-control is-invalid");
    }

    if (valido) {
        var codigo = guid();
        var listaAnexosGrid = new AnexoMap();

        listaAnexosGrid.Codigo.val = codigo;
        listaAnexosGrid.DescricaoAnexo.val = _chamadoAnexos.DescricaoAnexo.val();
        listaAnexosGrid.Arquivo.val = _chamadoAnexos.Arquivo.val();
        _chamado.ListaAnexos.list.push(listaAnexosGrid);

        var listaAnexos = new AnexoMap();

        listaAnexos.Codigo.val = codigo;
        listaAnexos.DescricaoAnexo.val = _chamadoAnexos.DescricaoAnexo.val();
        var file = document.getElementById(_chamadoAnexos.Arquivo.id);
        listaAnexos.Arquivo = file.files[0];
        _chamado.ListaAnexosNovos.list.push(listaAnexos);

        recarregarGridListaAnexos();
        limparCamposChamadoAnexos();
        $("#" + _chamadoAnexos.DescricaoAnexo.id).focus();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

//*******MÉTODOS*******

function VerificaNovosAnexosLancados(codigoAnexo) {
    var valido = true;
    $.each(_chamado.ListaAnexosNovos.list, function (i, listaAnexos) {
        if (codigoAnexo == listaAnexos.Codigo.val) {
            valido = false;
        }
    });

    return valido;
}

function recarregarGridListaAnexos() {

    var data = new Array();

    $.each(_chamado.ListaAnexos.list, function (i, listaAnexos) {
        var listaAnexosGrid = new Object();

        listaAnexosGrid.Codigo = listaAnexos.Codigo.val;
        listaAnexosGrid.CodigoChamado = _chamado.Codigo.val();
        listaAnexosGrid.DescricaoAnexo = listaAnexos.DescricaoAnexo.val;
        listaAnexosGrid.Arquivo = listaAnexos.Arquivo.val;

        data.push(listaAnexosGrid);
    });

    _gridAnexos.CarregarGrid(data);
}

function limparCamposChamadoAnexos() {
    LimparCampos(_chamadoAnexos);
    _chamadoAnexos.Arquivo.val("");
}

function exibirCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}