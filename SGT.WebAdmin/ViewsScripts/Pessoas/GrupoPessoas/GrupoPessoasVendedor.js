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
/// <reference path="GrupoPessoas.js" />

var _grupoPessoasVendedores;
var _gridGrupoPessoasVendedores;

//*******MAPEAMENTO KNOUCKOUT*******

var GrupoPessoasVendedorMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.CodigoFuncionario = PropertyEntity({ type: types.map, val: "" });
    this.Funcionario = PropertyEntity({ type: types.map, val: "" });
    this.PercentualComissao = PropertyEntity({ type: types.map, val: "" });
    this.DataInicioVigencia = PropertyEntity({ type: types.map, val: "" });
    this.DataFimVigencia = PropertyEntity({ type: types.map, val: "" });
}

var GrupoPessoasVendedor = function () {
    this.CodigoVendedor = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Funcionario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.Pessoas.GrupoPessoas.Funcionario.getRequiredFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.PercentualComissao = PropertyEntity({ def: "0,00000", val: ko.observable("0,00000"), getType: typesKnockout.decimal, required: true, text: Localization.Resources.Pessoas.GrupoPessoas.PermissaoComissao.getRequiredFieldDescription(), configDecimal: { precision: 5 }, maxlength: 8, enable: ko.observable(true) });
    this.DataInicioVigencia = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.DataInicioVigencia.getRequiredFieldDescription(), getType: typesKnockout.date, required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.DataFimVigencia = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.DataFimVigencia.getRequiredFieldDescription(), getType: typesKnockout.date, required: true, enable: ko.observable(true), visible: ko.observable(true) });

    this.GridGrupoPessoasVendedores = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, idGrid: guid(), codEntity: ko.observable(0), visible: ko.observable(true), enable: ko.observable(true) });

    this.AdicionarVendedor = PropertyEntity({ eventClick: adicionarGrupoPessoasVendedorClick, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Adicionar, visible: ko.observable(true), enable: !_FormularioSomenteLeitura });
    this.AtualizarVendedor = PropertyEntity({ eventClick: atualizarGrupoPessoasVendedorClick, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Atualizar, visible: ko.observable(false), enable: !_FormularioSomenteLeitura });
    this.ExcluirVendedor = PropertyEntity({ eventClick: excluirGrupoPessoasVendedorClick, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Excluir, visible: ko.observable(false), enable: !_FormularioSomenteLeitura });
    this.CancelarVendedor = PropertyEntity({ eventClick: LimparCamposGrupoPessoasVendedores, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Cancelar, visible: ko.observable(false), enable: !_FormularioSomenteLeitura });
}

//*******EVENTOS*******

function loadGrupoPessoasVendedor() {
    _grupoPessoasVendedores = new GrupoPessoasVendedor();
    KoBindings(_grupoPessoasVendedores, "knockoutVendedor");

    new BuscarFuncionario(_grupoPessoasVendedores.Funcionario);

    var editar = { descricao: Localization.Resources.Pessoas.GrupoPessoas.Editar, id: guid(), evento: "onclick", metodo: editarGrupoPessoasVendedor, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);
    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoFuncionario", visible: false },
        { data: "Funcionario", title: Localization.Resources.Pessoas.GrupoPessoas.Funcionario, width: "70%", className: "text-align-left" },
        { data: "PercentualComissao", title: Localization.Resources.Pessoas.GrupoPessoas.PercentualComissao, width: "20%", className: "text-align-left" },
        { data: "DataInicioVigencia", title: Localization.Resources.Pessoas.GrupoPessoas.DataInicioVigencia, width: "15%", className: "text-align-center" },
        { data: "DataFimVigencia", title: Localization.Resources.Pessoas.GrupoPessoas.DataFimVigencia, width: "15%", className: "text-align-center" }
    ];

    _gridGrupoPessoasVendedores = new BasicDataTable(_grupoPessoasVendedores.GridGrupoPessoasVendedores.idGrid, header, menuOpcoes);
    recarregarGridGrupoPessoasVendedores();
}

function adicionarGrupoPessoasVendedorClick() {
    var valido = ValidarCamposObrigatorios(_grupoPessoasVendedores);

    if (Globalize.parseFloat(_grupoPessoasVendedores.PercentualComissao.val()) <= 0) {
        valido = false;
        _grupoPessoasVendedores.PercentualComissao.requiredClass("form-control");
        _grupoPessoasVendedores.PercentualComissao.requiredClass("form-control is-invalid");
    }
    
    if (valido) {
        var existe = false;
        $.each(_grupoPessoas.ListaVendedores.list, function (i, grupoPessoasVendedor) {
            if (grupoPessoasVendedor.CodigoFuncionario.val == _grupoPessoasVendedores.Funcionario.codEntity()) {
                existe = true;
                return;
            }
        });

        var grupoPessoasVendedor = new GrupoPessoasVendedorMap();
        grupoPessoasVendedor.Codigo.val = guid();
        grupoPessoasVendedor.CodigoFuncionario.val = _grupoPessoasVendedores.Funcionario.codEntity();
        grupoPessoasVendedor.Funcionario.val = _grupoPessoasVendedores.Funcionario.val();
        grupoPessoasVendedor.PercentualComissao.val = _grupoPessoasVendedores.PercentualComissao.val();
        grupoPessoasVendedor.DataInicioVigencia.val = _grupoPessoasVendedores.DataInicioVigencia.val();
        grupoPessoasVendedor.DataFimVigencia.val = _grupoPessoasVendedores.DataFimVigencia.val();

        var listaVendedores = _grupoPessoas.ListaVendedores.list;
        var [novoCadastroVigenciaDataInicio, novoCadastroVigenciaDataFim] = [retornarDataFormatadaPadraoISO(grupoPessoasVendedor.DataInicioVigencia.val), retornarDataFormatadaPadraoISO(grupoPessoasVendedor.DataFimVigencia.val)];

        for (var vendedor of listaVendedores) {
            var [dataInicioVigenciaJaCadastrada, dataFimVigenciaJaCadastrada] = [retornarDataFormatadaPadraoISO(vendedor.DataInicioVigencia.val), retornarDataFormatadaPadraoISO(vendedor.DataFimVigencia.val)];
            var mesmoVendedor = grupoPessoasVendedor.Funcionario.val == vendedor.Funcionario.val;
            var periodoJaCadastrado = checarSuperposicaoDeDatas(novoCadastroVigenciaDataInicio, novoCadastroVigenciaDataFim, dataInicioVigenciaJaCadastrada, dataFimVigenciaJaCadastrada);

            if (mesmoVendedor && periodoJaCadastrado) {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Pessoas.GrupoPessoas.PeriodoJaCadastrado, Localization.Resources.Pessoas.GrupoPessoas.PeriodoJaCadastradoParaVendedor.format(_grupoPessoasVendedores.Funcionario.val()));
                return;
            }
        }

        _grupoPessoas.ListaVendedores.list.push(grupoPessoasVendedor);
        recarregarGridGrupoPessoasVendedores();
        $("#" + _grupoPessoasVendedores.Funcionario.id).focus();
        LimparCamposGrupoPessoasVendedores();
    } else {
        exibirMensagem("atencao", Localization.Resources.Pessoas.GrupoPessoas.CamposObrigatorios, Localization.Resources.Pessoas.GrupoPessoas.InformeCamposObrigatorios);
    }
}

function atualizarGrupoPessoasVendedorClick() {
    var valido = ValidarCamposObrigatorios(_grupoPessoasVendedores);

    if (Globalize.parseFloat(_grupoPessoasVendedores.PercentualComissao.val()) <= 0) {
        valido = false;
        _grupoPessoasVendedores.PercentualComissao.requiredClass("form-control");
        _grupoPessoasVendedores.PercentualComissao.requiredClass("form-control is-invalid");
    }

    if (valido) {
        $.each(_grupoPessoas.ListaVendedores.list, function (i, grupoPessoasVendedor) {
            if (grupoPessoasVendedor.Codigo.val == _grupoPessoasVendedores.CodigoVendedor.val()) {
                grupoPessoasVendedor.PercentualComissao.val = _grupoPessoasVendedores.PercentualComissao.val();
                grupoPessoasVendedor.DataInicioVigencia.val = _grupoPessoasVendedores.DataInicioVigencia.val();
                grupoPessoasVendedor.DataFimVigencia.val = _grupoPessoasVendedores.DataFimVigencia.val();

                return false;
            }
        });
        recarregarGridGrupoPessoasVendedores();
        LimparCamposGrupoPessoasVendedores();
    } else {
        exibirMensagem("atencao", Localization.Resources.Pessoas.GrupoPessoas.CamposObrigatorios, Localization.Resources.Pessoas.GrupoPessoas.InformeCamposObrigatorios);
    }
}

function excluirGrupoPessoasVendedorClick() {
    exibirConfirmacao(Localization.Resources.Pessoas.GrupoPessoas.Confirmacao, Localization.Resources.Pessoas.GrupoPessoas.RealmenteDesejaExcluirVendedorX.format(_grupoPessoasVendedores.Funcionario.val()), function () {
        var listaAtualizada = new Array();
        $.each(_grupoPessoas.ListaVendedores.list, function (i, grupoPessoasVendedor) {
            if (grupoPessoasVendedor.Codigo.val != _grupoPessoasVendedores.CodigoVendedor.val()) {
                listaAtualizada.push(grupoPessoasVendedor);
            }
        });
        _grupoPessoas.ListaVendedores.list = listaAtualizada;
        recarregarGridGrupoPessoasVendedores();
        LimparCamposGrupoPessoasVendedores();
    });
}

//*******MÉTODOS*******

function recarregarGridGrupoPessoasVendedores() {
    var data = new Array();
    $.each(_grupoPessoas.ListaVendedores.list, function (i, grupoPessoas) {
        var grupoPessoasVendedor = new Object();

        grupoPessoasVendedor.Codigo = grupoPessoas.Codigo.val;
        grupoPessoasVendedor.CodigoFuncionario = grupoPessoas.CodigoFuncionario.val;
        grupoPessoasVendedor.Funcionario = grupoPessoas.Funcionario.val;
        grupoPessoasVendedor.PercentualComissao = grupoPessoas.PercentualComissao.val;
        grupoPessoasVendedor.DataInicioVigencia = grupoPessoas.DataInicioVigencia.val;
        grupoPessoasVendedor.DataFimVigencia = grupoPessoas.DataFimVigencia.val;

        data.push(grupoPessoasVendedor);
    });
    _gridGrupoPessoasVendedores.CarregarGrid(data);
}

function editarGrupoPessoasVendedor(data) {
    LimparCamposGrupoPessoasVendedores();
    $.each(_grupoPessoas.ListaVendedores.list, function (i, grupoPessoasVendedor) {
        if (grupoPessoasVendedor.Codigo.val == data.Codigo) {
            _grupoPessoasVendedores.CodigoVendedor.val(grupoPessoasVendedor.Codigo.val);
            _grupoPessoasVendedores.Funcionario.codEntity(grupoPessoasVendedor.CodigoFuncionario.val);
            _grupoPessoasVendedores.Funcionario.val(grupoPessoasVendedor.Funcionario.val);
            _grupoPessoasVendedores.PercentualComissao.val(grupoPessoasVendedor.PercentualComissao.val);
            _grupoPessoasVendedores.DataInicioVigencia.val(grupoPessoasVendedor.DataInicioVigencia.val);
            _grupoPessoasVendedores.DataFimVigencia.val(grupoPessoasVendedor.DataFimVigencia.val);
            _grupoPessoasVendedores.Funcionario.enable(false);

            return false;
        }
    });

    _grupoPessoasVendedores.AdicionarVendedor.visible(false);
    _grupoPessoasVendedores.AtualizarVendedor.visible(true);
    _grupoPessoasVendedores.ExcluirVendedor.visible(true);
    _grupoPessoasVendedores.CancelarVendedor.visible(true);
}

function LimparCamposGrupoPessoasVendedores() {
    LimparCampoEntity(_grupoPessoasVendedores.Funcionario);
    _grupoPessoasVendedores.PercentualComissao.val("0,00000");
    _grupoPessoasVendedores.Funcionario.requiredClass("form-control");
    _grupoPessoasVendedores.PercentualComissao.requiredClass("form-control");
    _grupoPessoasVendedores.DataInicioVigencia.val("");
    _grupoPessoasVendedores.DataFimVigencia.val("");
    _grupoPessoasVendedores.Funcionario.enable(true);

    _grupoPessoasVendedores.AdicionarVendedor.visible(true);
    _grupoPessoasVendedores.AtualizarVendedor.visible(false);
    _grupoPessoasVendedores.ExcluirVendedor.visible(false);
    _grupoPessoasVendedores.CancelarVendedor.visible(false);
}

function retornarDataFormatadaPadraoISO(dataString) {
    var [dia, mes, ano] = dataString.split('/');
    return new Date(`${ano}/${mes}/${dia}`);
}

function checarSuperposicaoDeDatas(novaDataInicial, novaDataFinal, dataInicialJaCadastrada, dataFinalJaCadastrada) {
    return (novaDataInicial <= dataFinalJaCadastrada && novaDataFinal >= dataInicialJaCadastrada);
}