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
/// <reference path="PagamentoAgregado.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _pagamentoAnexo, _gridPagamentoAgregadoAnexo;

var PagamentoAgregadoAnexoMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: ko.observable(0), def: ko.observable(0), getType: typesKnockout.int });
    this.DescricaoAnexo = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.Arquivo = PropertyEntity({ type: types.map, val: ko.observable("") });
}

var PagamentoAgregadoAnexo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Grid = PropertyEntity({ type: types.local });

    this.DescricaoAnexo = PropertyEntity({ text: "*Descrição Anexo: ", enable: ko.observable(true), required: ko.observable(true), maxlength: 500 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "*Anexo:", val: ko.observable(""), enable: ko.observable(true), required: ko.observable(true), visible: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarPagamentoAgregadoAnexoClick, type: types.event, text: "Adicionar Anexo", enable: ko.observable(true), visible: ko.observable(true) });
}

//*******EVENTOS*******

function loadPagamentoAgregadoAnexo() {
    _pagamentoAnexo = new PagamentoAgregadoAnexo();
    KoBindings(_pagamentoAnexo, "knockoutAnexo");

    var baixarAnexo = { descricao: "Baixar", id: guid(), metodo: BaixarPagamentoAgregadoAnexoClick, icone: "" };
    var excluirAnexo = { descricao: "Excluir", id: guid(), metodo: ExcluirPagamentoAgregadoAnexoClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 15, opcoes: [baixarAnexo, excluirAnexo] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "DescricaoAnexo", title: "Descrição", width: "40%" },
        { data: "Arquivo", title: "Caminho / Nome Arquivo", width: "50%" }];

    _gridPagamentoAgregadoAnexo = new BasicDataTable(_pagamentoAnexo.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    RecarregarGridPagamentoAgregadoAnexo();
}

//*******MÉTODOS*******

function RecarregarGridPagamentoAgregadoAnexo() {
    var data = new Array();

    $.each(_pagamento.ListaAnexos.list, function (i, listaAnexos) {
        var listaAnexosGrid = new Object();

        listaAnexosGrid.Codigo = listaAnexos.Codigo.val;
        listaAnexosGrid.DescricaoAnexo = listaAnexos.DescricaoAnexo.val;
        listaAnexosGrid.Arquivo = listaAnexos.Arquivo.val;

        data.push(listaAnexosGrid);
    });

    _gridPagamentoAgregadoAnexo.CarregarGrid(data);
}

function BaixarPagamentoAgregadoAnexoClick(data) {
    if (VerificaNovosPagamentoAgregadoAnexosLancados(data.Codigo))
        executarDownload("PagamentoAgregado/DownloadAnexo", { CodigoAnexo: data.Codigo });
    else
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Só é possível baixar os Anexos após gravar a venda");
}

function ExcluirPagamentoAgregadoAnexoClick(data) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o anexo " + data.DescricaoAnexo + "?", function () {
        $.each(_pagamento.ListaAnexos.list, function (i, listaAnexos) {
            if (data.Codigo == listaAnexos.Codigo.val) {
                _pagamento.ListaAnexos.list.splice(i, 1);

                if (VerificaNovosPagamentoAgregadoAnexosLancados(data.Codigo)) {
                    var listaAnexosExcluidos = new PagamentoAgregadoAnexoMap();
                    listaAnexosExcluidos.Codigo.val = data.Codigo;
                    _pagamento.ListaAnexosExcluidos.list.push(listaAnexosExcluidos);
                }

                return false;
            }
        });

        $.each(_pagamento.ListaAnexosNovos.list, function (i, listaAnexosNovos) {
            if (data.Codigo == listaAnexosNovos.Codigo.val) {
                _pagamento.ListaAnexosNovos.list.splice(i, 1);

                return false;
            }
        });

        RecarregarGridPagamentoAgregadoAnexo();
    });
}

function AdicionarPagamentoAgregadoAnexoClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_pagamentoAnexo);
    var file = document.getElementById(_pagamentoAnexo.Arquivo.id);
    if (file.files[0] == null)
        valido = false;

    if (valido) {
        var codigo = guid();
        var listaAnexosGrid = new PagamentoAgregadoAnexoMap();

        listaAnexosGrid.Codigo.val = codigo;
        listaAnexosGrid.DescricaoAnexo.val = _pagamentoAnexo.DescricaoAnexo.val();
        listaAnexosGrid.Arquivo.val = _pagamentoAnexo.Arquivo.val();
        _pagamento.ListaAnexos.list.push(listaAnexosGrid);

        var listaAnexos = new PagamentoAgregadoAnexoMap();

        listaAnexos.Codigo.val = codigo;
        listaAnexos.DescricaoAnexo.val = _pagamentoAnexo.DescricaoAnexo.val();
        listaAnexos.Arquivo = file.files[0];
        _pagamento.ListaAnexosNovos.list.push(listaAnexos);

        RecarregarGridPagamentoAgregadoAnexo();
        LimparCamposPagamentoAgregadoAnexo();
        $("#" + _pagamentoAnexo.DescricaoAnexo.id).focus();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function VerificaNovosPagamentoAgregadoAnexosLancados(codigoAnexo) {
    var valido = true;
    $.each(_pagamento.ListaAnexosNovos.list, function (i, listaAnexos) {
        if (codigoAnexo == listaAnexos.Codigo.val) {
            valido = false;
        }
    });

    return valido;
}

function LimparCamposPagamentoAgregadoAnexo() {
    LimparCampos(_pagamentoAnexo);
    _pagamentoAnexo.Arquivo.val("");
}