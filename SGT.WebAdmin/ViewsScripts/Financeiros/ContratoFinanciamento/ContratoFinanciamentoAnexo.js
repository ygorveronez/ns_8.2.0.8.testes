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
/// <reference path="ContratoFinanciamento.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _contratoFinanciamentoAnexo, _gridContratoFinanciamentoAnexo;

var ContratoFinanciamentoAnexoMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: ko.observable(0), def: ko.observable(0), getType: typesKnockout.int });
    this.DescricaoAnexo = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.Arquivo = PropertyEntity({ type: types.map, val: ko.observable("") });
};

var ContratoFinanciamentoAnexo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Grid = PropertyEntity({ type: types.local });

    this.DescricaoAnexo = PropertyEntity({ text: "*Descrição Anexo: ", enable: ko.observable(true), required: ko.observable(true), maxlength: 500 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "*Anexo:", val: ko.observable(""), enable: ko.observable(true), required: ko.observable(true), visible: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarContratoFinanciamentoAnexoClick, type: types.event, text: "Adicionar Anexo", enable: ko.observable(true), visible: ko.observable(true) });
};

//*******EVENTOS*******

function loadContratoFinanciamentoAnexo() {
    _contratoFinanciamentoAnexo = new ContratoFinanciamentoAnexo();
    KoBindings(_contratoFinanciamentoAnexo, "knockoutAnexoContratoFinanciamento");

    var baixarAnexo = { descricao: "Baixar", id: guid(), metodo: BaixarContratoFinanciamentoAnexoClick, icone: "" };
    var excluirAnexo = { descricao: "Excluir", id: guid(), metodo: ExcluirContratoFinanciamentoAnexoClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 15, opcoes: [baixarAnexo, excluirAnexo] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "DescricaoAnexo", title: "Descrição", width: "40%" },
        { data: "Arquivo", title: "Caminho / Nome Arquivo", width: "50%" }];

    _gridContratoFinanciamentoAnexo = new BasicDataTable(_contratoFinanciamentoAnexo.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    RecarregarGridContratoFinanciamentoAnexo();
}

//*******MÉTODOS*******

function RecarregarGridContratoFinanciamentoAnexo() {
    var data = new Array();

    $.each(_contratoFinanciamento.ListaAnexos.list, function (i, listaAnexos) {
        var listaAnexosGrid = new Object();

        listaAnexosGrid.Codigo = listaAnexos.Codigo.val;
        listaAnexosGrid.DescricaoAnexo = listaAnexos.DescricaoAnexo.val;
        listaAnexosGrid.Arquivo = listaAnexos.Arquivo.val;

        data.push(listaAnexosGrid);
    });

    _gridContratoFinanciamentoAnexo.CarregarGrid(data);
}

function BaixarContratoFinanciamentoAnexoClick(data) {
    if (VerificaNovosContratoFinanciamentoAnexosLancados(data.Codigo))
        executarDownload("ContratoFinanciamento/DownloadAnexo", { CodigoAnexo: data.Codigo });
    else
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Só é possível baixar os Anexos após gravar");
}

function ExcluirContratoFinanciamentoAnexoClick(data) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o anexo " + data.DescricaoAnexo + "?", function () {
        $.each(_contratoFinanciamento.ListaAnexos.list, function (i, listaAnexos) {
            if (data.Codigo == listaAnexos.Codigo.val) {
                _contratoFinanciamento.ListaAnexos.list.splice(i, 1);

                if (VerificaNovosContratoFinanciamentoAnexosLancados(data.Codigo)) {
                    var listaAnexosExcluidos = new ContratoFinanciamentoAnexoMap();
                    listaAnexosExcluidos.Codigo.val = data.Codigo;
                    _contratoFinanciamento.ListaAnexosExcluidos.list.push(listaAnexosExcluidos);
                }

                return false;
            }
        });

        $.each(_contratoFinanciamento.ListaAnexosNovos.list, function (i, listaAnexosNovos) {
            if (data.Codigo == listaAnexosNovos.Codigo.val) {
                _contratoFinanciamento.ListaAnexosNovos.list.splice(i, 1);

                return false;
            }
        });

        RecarregarGridContratoFinanciamentoAnexo();
    });
}

function AdicionarContratoFinanciamentoAnexoClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_contratoFinanciamentoAnexo);
    var file = document.getElementById(_contratoFinanciamentoAnexo.Arquivo.id);
    if (file.files[0] == null)
        valido = false;

    if (valido) {
        var codigo = guid();
        var listaAnexosGrid = new ContratoFinanciamentoAnexoMap();

        listaAnexosGrid.Codigo.val = codigo;
        listaAnexosGrid.DescricaoAnexo.val = _contratoFinanciamentoAnexo.DescricaoAnexo.val();
        listaAnexosGrid.Arquivo.val = _contratoFinanciamentoAnexo.Arquivo.val();
        _contratoFinanciamento.ListaAnexos.list.push(listaAnexosGrid);

        var listaAnexos = new ContratoFinanciamentoAnexoMap();

        listaAnexos.Codigo.val = codigo;
        listaAnexos.DescricaoAnexo.val = _contratoFinanciamentoAnexo.DescricaoAnexo.val();
        listaAnexos.Arquivo = file.files[0];
        _contratoFinanciamento.ListaAnexosNovos.list.push(listaAnexos);

        RecarregarGridContratoFinanciamentoAnexo();
        LimparCamposContratoFinanciamentoAnexo();
        $("#" + _contratoFinanciamentoAnexo.DescricaoAnexo.id).focus();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function obterFormDataContratoFinanciamentoAnexo() {
    var anexos = _contratoFinanciamento.ListaAnexosNovos.list;

    if (anexos.length > 0) {
        var formData = new FormData();

        anexos.forEach(function (anexo) {
            formData.append("Arquivo", anexo.Arquivo);
            formData.append("Descricao", anexo.DescricaoAnexo.val);
        });

        return formData;
    }

    return undefined;
}

function VerificaNovosContratoFinanciamentoAnexosLancados(codigoAnexo) {
    var valido = true;
    $.each(_contratoFinanciamento.ListaAnexosNovos.list, function (i, listaAnexos) {
        if (codigoAnexo == listaAnexos.Codigo.val) {
            valido = false;
        }
    });

    return valido;
}

function LimparCamposContratoFinanciamentoAnexo() {
    LimparCampos(_contratoFinanciamentoAnexo);
    _contratoFinanciamentoAnexo.Arquivo.val("");
}