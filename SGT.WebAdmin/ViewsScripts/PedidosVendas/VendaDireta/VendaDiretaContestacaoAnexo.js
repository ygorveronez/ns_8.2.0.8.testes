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

var _vendaDiretaContestacaoAnexo, _gridVendaDiretaContestacaoAnexo;

var VendaDiretaContestacaoAnexoMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: ko.observable(0), def: ko.observable(0), getType: typesKnockout.int });
    this.DescricaoAnexo = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.Arquivo = PropertyEntity({ type: types.map, val: ko.observable("") });
}

var VendaDiretaContestacaoAnexo = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Grid = PropertyEntity({ type: types.local });

    this.FuncionarioContestacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Contestador:", idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.DataContestacao = PropertyEntity({ text: "Data Contestação: ", getType: typesKnockout.dateTime, enable: ko.observable(true), visible: ko.observable(true) });
    this.ObservacaoContestacao = PropertyEntity({ text: "Observação:", maxlength: 2000, val: ko.observable(""), enable: ko.observable(true) });

    this.DescricaoAnexo = PropertyEntity({ text: "*Descrição Anexo: ", enable: ko.observable(true), required: ko.observable(true), maxlength: 500 });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: "*Anexo:", val: ko.observable(""), enable: ko.observable(true), required: ko.observable(true), visible: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarVendaDiretaContestacaoAnexoClick, type: types.event, text: "Adicionar Anexo", enable: ko.observable(true), visible: ko.observable(true) });
    this.Salvar = PropertyEntity({ eventClick: SalvarVendaDiretaContestacaoAnexoClick, type: types.event, text: "Salvar", enable: ko.observable(true), visible: ko.observable(true) });
}

//*******EVENTOS*******

function loadVendaDiretaContestacaoAnexo() {
    _vendaDiretaContestacaoAnexo = new VendaDiretaContestacaoAnexo();
    KoBindings(_vendaDiretaContestacaoAnexo, "knockoutContestacaoAnexo");

    var baixarAnexo = { descricao: "Baixar", id: guid(), metodo: BaixarVendaDiretaContestacaoAnexoClick, icone: "" };
    var excluirAnexo = { descricao: "Excluir", id: guid(), metodo: ExcluirVendaDiretaContestacaoAnexoClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 15, opcoes: [baixarAnexo, excluirAnexo] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "DescricaoAnexo", title: "Descrição", width: "40%" },
        { data: "Arquivo", title: "Caminho / Nome Arquivo", width: "50%" }];

    _gridVendaDiretaContestacaoAnexo = new BasicDataTable(_vendaDiretaContestacaoAnexo.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    RecarregarGridVendaDiretaContestacaoAnexo();
}

//*******MÉTODOS*******

function RecarregarGridVendaDiretaContestacaoAnexo() {
    var data = new Array();

    $.each(_vendaDireta.ListaAnexos.list, function (i, listaAnexos) {
        var listaAnexosGrid = new Object();

        listaAnexosGrid.Codigo = listaAnexos.Codigo.val;
        listaAnexosGrid.DescricaoAnexo = listaAnexos.DescricaoAnexo.val;
        listaAnexosGrid.Arquivo = listaAnexos.Arquivo.val;

        data.push(listaAnexosGrid);
    });

    _gridVendaDiretaContestacaoAnexo.CarregarGrid(data);
}

function BaixarVendaDiretaContestacaoAnexoClick(data) {
    if (VerificaNovosVendaDiretaContestacaoAnexosLancados(data.Codigo))
        executarDownload("VendaDireta/DownloadAnexo", { CodigoAnexo: data.Codigo });
    else
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Só é possível baixar os Anexos após gravar a venda");
}

function ExcluirVendaDiretaContestacaoAnexoClick(data) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o anexo " + data.DescricaoAnexo + "?", function () {
        $.each(_vendaDireta.ListaAnexos.list, function (i, listaAnexos) {
            if (data.Codigo == listaAnexos.Codigo.val) {
                _vendaDireta.ListaAnexos.list.splice(i, 1);

                if (VerificaNovosVendaDiretaContestacaoAnexosLancados(data.Codigo)) {
                    var listaAnexosExcluidos = new VendaDiretaContestacaoAnexoMap();
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

        RecarregarGridVendaDiretaContestacaoAnexo();
    });
}

function SalvarVendaDiretaContestacaoAnexoClick(e, sender) {
    Global.fecharModal('divContestacaoAnexo');
    
}

function AdicionarVendaDiretaContestacaoAnexoClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_vendaDiretaContestacaoAnexo);
    var file = document.getElementById(_vendaDiretaContestacaoAnexo.Arquivo.id);
    if (file.files[0] == null)
        valido = false;

    if (valido) {
        var codigo = guid();
        var listaAnexosGrid = new VendaDiretaContestacaoAnexoMap();

        listaAnexosGrid.Codigo.val = codigo;
        listaAnexosGrid.DescricaoAnexo.val = _vendaDiretaContestacaoAnexo.DescricaoAnexo.val();
        listaAnexosGrid.Arquivo.val = _vendaDiretaContestacaoAnexo.Arquivo.val();
        _vendaDireta.ListaAnexos.list.push(listaAnexosGrid);

        var listaAnexos = new VendaDiretaContestacaoAnexoMap();

        listaAnexos.Codigo.val = codigo;
        listaAnexos.DescricaoAnexo.val = _vendaDiretaContestacaoAnexo.DescricaoAnexo.val();
        listaAnexos.Arquivo = file.files[0];
        _vendaDireta.ListaAnexosNovos.list.push(listaAnexos);

        RecarregarGridVendaDiretaContestacaoAnexo();
        LimparCamposVendaDiretaContestacaoAnexo();
        $("#" + _vendaDiretaContestacaoAnexo.DescricaoAnexo.id).focus();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function VerificaNovosVendaDiretaContestacaoAnexosLancados(codigoAnexo) {
    var valido = true;
    $.each(_vendaDireta.ListaAnexosNovos.list, function (i, listaAnexos) {
        if (codigoAnexo == listaAnexos.Codigo.val) {
            valido = false;
        }
    });

    return valido;
}

function LimparCamposVendaDiretaContestacaoAnexo() {
    LimparCampos(_vendaDiretaContestacaoAnexo);
    _vendaDiretaContestacaoAnexo.Arquivo.val("");
}