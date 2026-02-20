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
/// <reference path="VendaDireta.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _vendaDiretaAnexo, _gridVendaDiretaAnexo;

var VendaDiretaAnexoMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: ko.observable(0), def: ko.observable(0), getType: typesKnockout.int });
    this.DescricaoAnexo = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.Arquivo = PropertyEntity({ type: types.map, val: ko.observable("") });
}

var VendaDiretaAnexo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Grid = PropertyEntity({ type: types.local });

    this.DescricaoAnexo = PropertyEntity({ text: "*Descrição Anexo: ", enable: ko.observable(true), required: ko.observable(true), maxlength: 500 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "*Anexo:", val: ko.observable(""), enable: ko.observable(true), required: ko.observable(true), visible: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarVendaDiretaAnexoClick, type: types.event, text: "Adicionar Anexo", enable: ko.observable(true), visible: ko.observable(true) });
}

//*******EVENTOS*******

function loadVendaDiretaAnexo() {
    _vendaDiretaAnexo = new VendaDiretaAnexo();
    KoBindings(_vendaDiretaAnexo, "knockoutVendaDiretaAnexo");

    var baixarAnexo = { descricao: "Baixar", id: guid(), metodo: BaixarVendaDiretaAnexoClick, icone: "" };
    var excluirAnexo = { descricao: "Excluir", id: guid(), metodo: ExcluirVendaDiretaAnexoClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 15, opcoes: [baixarAnexo, excluirAnexo] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "DescricaoAnexo", title: "Descrição", width: "40%" },
        { data: "Arquivo", title: "Caminho / Nome Arquivo", width: "50%" }];

    _gridVendaDiretaAnexo = new BasicDataTable(_vendaDiretaAnexo.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    RecarregarGridVendaDiretaAnexo();
}

//*******MÉTODOS*******

function RecarregarGridVendaDiretaAnexo() {
    var data = new Array();

    $.each(_vendaDireta.ListaAnexos.list, function (i, listaAnexos) {
        var listaAnexosGrid = new Object();

        listaAnexosGrid.Codigo = listaAnexos.Codigo.val;
        listaAnexosGrid.DescricaoAnexo = listaAnexos.DescricaoAnexo.val;
        listaAnexosGrid.Arquivo = listaAnexos.Arquivo.val;

        data.push(listaAnexosGrid);
    });

    _gridVendaDiretaAnexo.CarregarGrid(data);
}

function BaixarVendaDiretaAnexoClick(data) {
    if (VerificaNovosVendaDiretaAnexosLancados(data.Codigo))
        executarDownload("VendaDireta/DownloadAnexo", { CodigoAnexo: data.Codigo });
    else
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Só é possível baixar os Anexos após gravar a venda");
}

function ExcluirVendaDiretaAnexoClick(data) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o anexo " + data.DescricaoAnexo + "?", function () {
        $.each(_vendaDireta.ListaAnexos.list, function (i, listaAnexos) {
            if (data.Codigo == listaAnexos.Codigo.val) {
                _vendaDireta.ListaAnexos.list.splice(i, 1);

                if (VerificaNovosVendaDiretaAnexosLancados(data.Codigo)) {
                    var listaAnexosExcluidos = new VendaDiretaAnexoMap();
                    listaAnexosExcluidos.Codigo.val = data.Codigo;
                    _vendaDireta.ListaAnexosExcluidos.list.push(listaAnexosExcluidos);
                }

                return false;
            }
        });

        $.each(_vendaDireta.ListaAnexosNovos.list, function (i, listaAnexosNovos) {
            if (data.Codigo == listaAnexosNovos.Codigo.val) {
                _vendaDireta.ListaAnexosNovos.list.splice(i, 1);

                return false;
            }
        });

        RecarregarGridVendaDiretaAnexo();
    });
}

function AdicionarVendaDiretaAnexoClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_vendaDiretaAnexo);
    var file = document.getElementById(_vendaDiretaAnexo.Arquivo.id);
    if (file.files[0] == null)
        valido = false;

    if (valido) {
        var codigo = guid();
        var listaAnexosGrid = new VendaDiretaAnexoMap();

        listaAnexosGrid.Codigo.val = codigo;
        listaAnexosGrid.DescricaoAnexo.val = _vendaDiretaAnexo.DescricaoAnexo.val();
        listaAnexosGrid.Arquivo.val = _vendaDiretaAnexo.Arquivo.val();
        _vendaDireta.ListaAnexos.list.push(listaAnexosGrid);

        var listaAnexos = new VendaDiretaAnexoMap();

        listaAnexos.Codigo.val = codigo;
        listaAnexos.DescricaoAnexo.val = _vendaDiretaAnexo.DescricaoAnexo.val();
        listaAnexos.Arquivo = file.files[0];
        _vendaDireta.ListaAnexosNovos.list.push(listaAnexos);

        RecarregarGridVendaDiretaAnexo();
        LimparCamposVendaDiretaAnexo();
        $("#" + _vendaDiretaAnexo.DescricaoAnexo.id).focus();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function VerificaNovosVendaDiretaAnexosLancados(codigoAnexo) {
    var valido = true;
    $.each(_vendaDireta.ListaAnexosNovos.list, function (i, listaAnexos) {
        if (codigoAnexo == listaAnexos.Codigo.val) {
            valido = false;
        }
    });

    return valido;
}

function LimparCamposVendaDiretaAnexo() {
    LimparCampos(_vendaDiretaAnexo);
    _vendaDiretaAnexo.Arquivo.val("");
}