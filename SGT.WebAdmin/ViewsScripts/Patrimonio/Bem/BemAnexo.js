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
/// <reference path="Bem.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _bemAnexo, _gridBemAnexo;

var BemAnexoMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: ko.observable(0), def: ko.observable(0), getType: typesKnockout.int });
    this.DescricaoAnexo = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.Arquivo = PropertyEntity({ type: types.map, val: ko.observable("") });
}

var BemAnexo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Grid = PropertyEntity({ type: types.local });

    this.DescricaoAnexo = PropertyEntity({ text: "*Descrição Anexo: ", required: ko.observable(true), maxlength: 500 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "*Anexo:", val: ko.observable(""), required: ko.observable(true), visible: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarBemAnexoClick, type: types.event, text: "Adicionar Anexo", visible: ko.observable(true) });
}

//*******EVENTOS*******

function loadBemAnexo() {
    _bemAnexo = new BemAnexo();
    KoBindings(_bemAnexo, "knockoutAnexoBem");

    var baixarAnexo = { descricao: "Baixar", id: guid(), metodo: BaixarBemAnexoClick, icone: "" };
    var excluirAnexo = { descricao: "Excluir", id: guid(), metodo: ExcluirBemAnexoClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 15, opcoes: [baixarAnexo, excluirAnexo] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "DescricaoAnexo", title: "Descrição", width: "40%" },
        { data: "Arquivo", title: "Caminho / Nome Arquivo", width: "50%" }];

    _gridBemAnexo = new BasicDataTable(_bemAnexo.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    RecarregarGridBemAnexo();
}

//*******MÉTODOS*******

function RecarregarGridBemAnexo() {
    var data = new Array();

    $.each(_bem.ListaAnexos.list, function (i, listaAnexos) {
        var listaAnexosGrid = new Object();

        listaAnexosGrid.Codigo = listaAnexos.Codigo.val;
        listaAnexosGrid.DescricaoAnexo = listaAnexos.DescricaoAnexo.val;
        listaAnexosGrid.Arquivo = listaAnexos.Arquivo.val;

        data.push(listaAnexosGrid);
    });

    _gridBemAnexo.CarregarGrid(data);
}

function BaixarBemAnexoClick(data) {
    if (VerificaNovosBemAnexosLancados(data.Codigo))
        executarDownload("Bem/DownloadAnexo", { CodigoAnexo: data.Codigo });
    else
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Só é possível baixar os Anexos após gravar o Bem");
}

function ExcluirBemAnexoClick(data) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o anexo " + data.DescricaoAnexo + "?", function () {
        $.each(_bem.ListaAnexos.list, function (i, listaAnexos) {
            if (data.Codigo == listaAnexos.Codigo.val) {
                _bem.ListaAnexos.list.splice(i, 1);

                if (VerificaNovosBemAnexosLancados(data.Codigo)) {
                    var listaAnexosExcluidos = new BemAnexoMap();
                    listaAnexosExcluidos.Codigo.val = data.Codigo;
                    _bem.ListaAnexosExcluidos.list.push(listaAnexosExcluidos);
                }

                return false;
            }
        });

        $.each(_bem.ListaAnexosNovos.list, function (i, listaAnexosNovos) {
            if (data.Codigo == listaAnexosNovos.Codigo.val) {
                _bem.ListaAnexosNovos.list.splice(i, 1);

                return false;
            }
        });

        RecarregarGridBemAnexo();
    });
}

function AdicionarBemAnexoClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_bemAnexo);
    var file = document.getElementById(_bemAnexo.Arquivo.id);
    if (file.files[0] == null)
        valido = false;

    if (valido) {
        var codigo = guid();
        var listaAnexosGrid = new BemAnexoMap();

        listaAnexosGrid.Codigo.val = codigo;
        listaAnexosGrid.DescricaoAnexo.val = _bemAnexo.DescricaoAnexo.val();
        listaAnexosGrid.Arquivo.val = _bemAnexo.Arquivo.val();
        _bem.ListaAnexos.list.push(listaAnexosGrid);

        var listaAnexos = new BemAnexoMap();

        listaAnexos.Codigo.val = codigo;
        listaAnexos.DescricaoAnexo.val = _bemAnexo.DescricaoAnexo.val();
        listaAnexos.Arquivo = file.files[0];
        _bem.ListaAnexosNovos.list.push(listaAnexos);

        RecarregarGridBemAnexo();
        LimparCamposBemAnexo();
        $("#" + _bemAnexo.DescricaoAnexo.id).focus();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function VerificaNovosBemAnexosLancados(codigoAnexo) {
    var valido = true;
    $.each(_bem.ListaAnexosNovos.list, function (i, listaAnexos) {
        if (codigoAnexo == listaAnexos.Codigo.val) {
            valido = false;
        }
    });

    return valido;
}

function LimparCamposBemAnexo() {
    LimparCampos(_bemAnexo);
    _bemAnexo.Arquivo.val("");
}