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
/// <reference path="TituloFinanceiro.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _TituloFinanceiroAnexo, _gridTituloFinanceiroAnexo;

var TituloFinanceiroAnexoMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: ko.observable(0), def: ko.observable(0), getType: typesKnockout.int });
    this.DescricaoAnexo = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.Arquivo = PropertyEntity({ type: types.map, val: ko.observable("") });
};

var TituloFinanceiroAnexo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Grid = PropertyEntity({ type: types.local });

    this.DescricaoAnexo = PropertyEntity({ text: "*Descrição Anexo: ", enable: ko.observable(true), required: ko.observable(true), maxlength: 500 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "*Anexo:", val: ko.observable(""), enable: ko.observable(true), required: ko.observable(true), visible: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarTituloFinanceiroAnexoClick, type: types.event, text: "Adicionar Anexo", enable: ko.observable(true), visible: ko.observable(true) });
};

//*******EVENTOS*******

function loadTituloFinanceiroAnexo() {
    _TituloFinanceiroAnexo = new TituloFinanceiroAnexo();
    KoBindings(_TituloFinanceiroAnexo, "knockoutAnexoTituloFinanceiro");

    var baixarAnexo = { descricao: "Baixar", id: guid(), metodo: BaixarTituloFinanceiroAnexoClick, icone: "" };
    var excluirAnexo = { descricao: "Excluir", id: guid(), metodo: ExcluirTituloFinanceiroAnexoClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 15, opcoes: [baixarAnexo, excluirAnexo] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "DescricaoAnexo", title: "Descrição", width: "40%" },
        { data: "Arquivo", title: "Caminho / Nome Arquivo", width: "50%" }];

    _gridTituloFinanceiroAnexo = new BasicDataTable(_TituloFinanceiroAnexo.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    RecarregarGridTituloFinanceiroAnexo();
}

//*******MÉTODOS*******

function RecarregarGridTituloFinanceiroAnexo() {
    var data = new Array();

    $.each(_tituloFinanceiro.ListaAnexos.list, function (i, listaAnexos) {
        var listaAnexosGrid = new Object();

        listaAnexosGrid.Codigo = listaAnexos.Codigo.val;
        listaAnexosGrid.DescricaoAnexo = listaAnexos.DescricaoAnexo.val;
        listaAnexosGrid.Arquivo = listaAnexos.Arquivo.val;

        data.push(listaAnexosGrid);
    });

    _gridTituloFinanceiroAnexo.CarregarGrid(data);
}

function BaixarTituloFinanceiroAnexoClick(data) {
    if (VerificaNovosTituloFinanceiroAnexosLancados(data.Codigo))
        executarDownload("TituloFinanceiro/DownloadAnexo", { CodigoAnexo: data.Codigo });
    else
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Só é possível baixar os Anexos após gravar");
}

function ExcluirTituloFinanceiroAnexoClick(data) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o anexo " + data.DescricaoAnexo + "?", function () {
        $.each(_tituloFinanceiro.ListaAnexos.list, function (i, listaAnexos) {
            if (data.Codigo == listaAnexos.Codigo.val) {
                _tituloFinanceiro.ListaAnexos.list.splice(i, 1);

                if (VerificaNovosTituloFinanceiroAnexosLancados(data.Codigo)) {
                    var listaAnexosExcluidos = new TituloFinanceiroAnexoMap();
                    listaAnexosExcluidos.Codigo.val = data.Codigo;
                    _tituloFinanceiro.ListaAnexosExcluidos.list.push(listaAnexosExcluidos);
                }

                return false;
            }
        });

        $.each(_tituloFinanceiro.ListaAnexosNovos.list, function (i, listaAnexosNovos) {
            if (data.Codigo == listaAnexosNovos.Codigo.val) {
                _tituloFinanceiro.ListaAnexosNovos.list.splice(i, 1);

                return false;
            }
        });

        RecarregarGridTituloFinanceiroAnexo();
    });
}

function AdicionarTituloFinanceiroAnexoClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_TituloFinanceiroAnexo);
    var file = document.getElementById(_TituloFinanceiroAnexo.Arquivo.id);
    if (file.files[0] == null)
        valido = false;

    if (valido) {
        var codigo = guid();
        var listaAnexosGrid = new TituloFinanceiroAnexoMap();

        listaAnexosGrid.Codigo.val = codigo;
        listaAnexosGrid.DescricaoAnexo.val = _TituloFinanceiroAnexo.DescricaoAnexo.val();
        listaAnexosGrid.Arquivo.val = _TituloFinanceiroAnexo.Arquivo.val();
        _tituloFinanceiro.ListaAnexos.list.push(listaAnexosGrid);

        var listaAnexos = new TituloFinanceiroAnexoMap();

        listaAnexos.Codigo.val = codigo;
        listaAnexos.DescricaoAnexo.val = _TituloFinanceiroAnexo.DescricaoAnexo.val();
        listaAnexos.Arquivo = file.files[0];
        _tituloFinanceiro.ListaAnexosNovos.list.push(listaAnexos);

        RecarregarGridTituloFinanceiroAnexo();
        LimparCamposTituloFinanceiroAnexo();
        $("#" + _TituloFinanceiroAnexo.DescricaoAnexo.id).focus();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function obterFormDataTituloFinanceiroAnexo() {
    var anexos = _tituloFinanceiro.ListaAnexosNovos.list;

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

function VerificaNovosTituloFinanceiroAnexosLancados(codigoAnexo) {
    var valido = true;
    $.each(_tituloFinanceiro.ListaAnexosNovos.list, function (i, listaAnexos) {
        if (codigoAnexo == listaAnexos.Codigo.val) {
            valido = false;
        }
    });

    return valido;
}

function LimparCamposTituloFinanceiroAnexo() {
    LimparCampos(_TituloFinanceiroAnexo);
    _TituloFinanceiroAnexo.Arquivo.val("");

    _tituloFinanceiro.ListaAnexosNovos.val(new Array());
    _tituloFinanceiro.ListaAnexos.val(new Array());
    _tituloFinanceiro.ListaAnexosExcluidos.val(new Array());

    RecarregarGridTituloFinanceiroAnexo();

}