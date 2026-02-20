/// <reference path="../../Enumeradores/EnumTipoParametroBaseTabelaFrete.js" />
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
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/TabelaFrete.js" />
/// <reference path="../../Enumeradores/EnumTipoCampoValorTabelaFrete.js" />
/// <reference path="../../Enumeradores/EnumTipoComponenteFrete.js" />
/// <reference path="TabelaFrete.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridParametros;
var _parametro;

var _tipoValor = [
    { text: 'Valor Fixo', value: EnumTipoCampoValorTabelaFrete.ValorFixo },
    { text: 'Aumento de Valor', value: EnumTipoCampoValorTabelaFrete.AumentoValor },
    { text: 'Aumento Percentual', value: EnumTipoCampoValorTabelaFrete.AumentoPercentual },
    { text: 'Percentual sobre o valor da nota fiscal', value: EnumTipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal }
];

var ParametroMap = function () {
    this.ParametroBase = PropertyEntity({ type: types.map, val: ko.observable(""), def: "" });
    this.ItemParametroBase = PropertyEntity({ type: types.map, val: ko.observable(""), def: "" });
    this.TipoValor = PropertyEntity({ type: types.map, val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Coluna = PropertyEntity({ type: types.map, val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

var Parametro = function () {
    this.Grid = PropertyEntity({ type: types.local });

    this.ParametroBase = PropertyEntity({ val: ko.observable(""), options: ko.observable(new Array()), text: "*Parâmetro:", def: "", visible: ko.observable(true), required: ko.observable(true) });
    this.ItemParametroBase = PropertyEntity({ val: ko.observable(""), options: ko.observable(new Array()), text: "*Item:", def: "", visible: ko.observable(true), required: true });
    this.TipoValor = PropertyEntity({ val: ko.observable(""), options: _tipoValor, text: "*Tipo de Valor:", def: "", visible: ko.observable(true), required: true });
    this.Coluna = PropertyEntity({ val: ko.observable(1), def: 1, getType: typesKnockout.int, text: "*Coluna:", visible: ko.observable(true), required: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarParametroClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.ExcluirTodos = PropertyEntity({ eventClick: excluirTodosParametroClick, type: types.event, text: "Excluir Todos", visible: ko.observable(true) });

    this.ItemParametroBase.val.subscribe(function (novoValor) {
        changeItemParametroBase(novoValor);
    });
};

//*******EVENTOS*******

function changeItemParametroBase(valor) {
    if (valor == null)
        return;

    var tipoObjeto = valor.split('_')[0];
    var tipo = Globalize.parseInt(tipoObjeto);

    if (tipo != EnumTipoParametroBaseTabelaFrete.ComponenteFrete) {
        if ((tipoObjeto == "ValorBase") || (tipoObjeto == "ValorMinimoGarantido")) {
            $("#" + _parametro.TipoValor.id + " option").prop("disabled", true);
            $("#" + _parametro.TipoValor.id + " option[value=" + EnumTipoCampoValorTabelaFrete.ValorFixo + "]").prop("disabled", false);

            _parametro.TipoValor.val(EnumTipoCampoValorTabelaFrete.ValorFixo);
        }
        else if ((tipoObjeto == "EntregaExcedente") || (tipoObjeto == "PesoExcedente")) {
            $("#" + _parametro.TipoValor.id + " option").prop("disabled", true);
            $("#" + _parametro.TipoValor.id + " option[value=" + EnumTipoCampoValorTabelaFrete.AumentoValor + "]").prop("disabled", false);

            _parametro.TipoValor.val(EnumTipoCampoValorTabelaFrete.AumentoValor);
        }
        else {
            $("#" + _parametro.TipoValor.id + " option").prop("disabled", false);

            _parametro.TipoValor.val(EnumTipoCampoValorTabelaFrete.ValorFixo);
        }
    }
    else {
        $("#" + _parametro.TipoValor.id + " option").prop("disabled", true);

        var tipoComponente = Globalize.parseInt(valor.split('_')[2]);

        if (
            tipoComponente == EnumTipoComponenteFrete.PEDAGIO ||
            tipoComponente == EnumTipoComponenteFrete.DESCARGA ||
            tipoComponente == EnumTipoComponenteFrete.FRETE ||
            tipoComponente == EnumTipoComponenteFrete.OUTROS
        ) {
            $("#" + _parametro.TipoValor.id + " option[value=" + EnumTipoCampoValorTabelaFrete.AumentoValor + "]").prop("disabled", false);
            _parametro.TipoValor.val(EnumTipoCampoValorTabelaFrete.AumentoValor);
        }

        if (
            tipoComponente == EnumTipoComponenteFrete.OUTROS ||
            tipoComponente == EnumTipoComponenteFrete.FRETE ||
            tipoComponente == EnumTipoComponenteFrete.DESCARGA
        ) {
            $("#" + _parametro.TipoValor.id + " option[value=" + EnumTipoCampoValorTabelaFrete.AumentoPercentual + "]").prop("disabled", false);
        }

        if (
            tipoComponente == EnumTipoComponenteFrete.OUTROS ||
            tipoComponente == EnumTipoComponenteFrete.ADVALOREM
        ) {
            $("#" + _parametro.TipoValor.id + " option[value=" + EnumTipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal + "]").prop("disabled", false);

            if (tipoComponente == EnumTipoComponenteFrete.ADVALOREM)
                _parametro.TipoValor.val(EnumTipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal);
        }
    }
}

function loadParametros() {

    _parametro = new Parametro();
    KoBindings(_parametro, "knockoutParametros");

    var configuracaoEdicaoColuna = {
        permite: true,
        callback: AtualizarGridParametros,
        atualizarRow: true
    };

    var configuracaoEdicaoCelula = {
        editable: true,
        type: EnumTipoColunaEditavelGrid.int,
        numberMask: ConfigInt()
    };

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Excluir", id: guid(), metodo: excluirParametroClick }] };

    var header = [
        { data: "CodigoParametro", visible: false },
        { data: "CodigoItem", visible: false },
        { data: "Parametro", title: "Parâmetro", width: "28%" },
        { data: "Item", title: "Item", width: "28%" },
        { data: "TipoValor", title: "Tipo Valor", width: "15%" },
        { data: "Coluna", title: "Coluna", width: "15%", editableCell: configuracaoEdicaoCelula }
    ];

    _gridParametros = new BasicDataTable(_parametro.Grid.id, header, menuOpcoes, { column: 0, dir: orderDir.asc }, null, 20, null, null, configuracaoEdicaoColuna);

    recarregarGridParametros();
}

function AtualizarGridParametros(dataRow, row, head, callbackTabPress, table) {
    var data = GetParametros();

    for (var i in data) {
        if (dataRow.CodigoParametro == data[i].ParametroBase.val && dataRow.CodigoItem == data[i].ItemParametroBase.val) {
            var coluna = Globalize.parseInt(dataRow.Coluna);
            if (coluna > 0)
                data[i].Coluna.val = coluna;
            else {
                dataRow.Coluna = data[i].Coluna.val;
                AtualizarDataRow(table, row, dataRow, callbackTabPress);
            }
            break;
        }
    }

    SetParametros(data);
}

function GetParametros() {
    return _importacaoTabelaFrete.Parametros.list.slice();
}

function SetParametros(data) {
    return _importacaoTabelaFrete.Parametros.list = data.slice();
}

function recarregarGridParametros() {

    var data = new Array();

    $.each(_importacaoTabelaFrete.Parametros.list, function (i, parametro) {
        var parametroGrid = new Object();

        parametroGrid.DT_Enable = true;
        parametroGrid.CodigoParametro = parametro.ParametroBase.val;
        parametroGrid.CodigoItem = parametro.ItemParametroBase.val;
        parametroGrid.Coluna = parametro.Coluna.val;
        parametroGrid.Parametro = $("#" + _parametro.ParametroBase.id + " option[value='" + parametro.ParametroBase.val + "']").text();
        parametroGrid.Item = $("#" + _parametro.ItemParametroBase.id + " option[value='" + parametro.ItemParametroBase.val + "']").text();
        parametroGrid.TipoValor = $("#" + _parametro.TipoValor.id + " option[value='" + parametro.TipoValor.val + "']").text();

        data.push(parametroGrid);
    });

    _gridParametros.CarregarGrid(data);
}

function excluirParametroClick(data) {
    for (var i = 0; i < _importacaoTabelaFrete.Parametros.list.length; i++) {
        if (data.CodigoParametro == _importacaoTabelaFrete.Parametros.list[i].ParametroBase.val && data.CodigoItem == _importacaoTabelaFrete.Parametros.list[i].ItemParametroBase.val) {
            _importacaoTabelaFrete.Parametros.list.splice(i, 1);
            break;
        }
    }

    recarregarGridParametros();
}

function adicionarParametroClick(e, sender) {
    if (_importacaoTabelaFrete.TabelaFrete.codEntity() <= 0) {
        exibirMensagem(tipoMensagem.atencao, "Atenção!", "Selecione a tabela de frete antes de adicionar um parâmetro!");
        return;
    }

    var valido = ValidarCamposObrigatorios(_parametro);

    if (_parametro.ParametroBase.val() == 0) {
        if (_importacaoTabelaFrete.ColunaParametrosBase.val() == "" || _importacaoTabelaFrete.ColunaParametrosBase.val() == "0") {
            exibirMensagem(tipoMensagem.aviso, "Parâmetro", "Parâmetro só permitido quando informado a coluna Parâmetros Base.");
            return;
        }
    }

    if (valido) {
        for (var i = 0; i < _importacaoTabelaFrete.Parametros.list.length; i++) {
            
            if (_parametro.ParametroBase.val() != "0" && _importacaoTabelaFrete.Parametros.list[i].ItemParametroBase.val == _parametro.ItemParametroBase.val()
                && (_importacaoTabelaFrete.Parametros.list[i].ParametroBase.val == null
                    || _importacaoTabelaFrete.Parametros.list[i].ParametroBase.val == ""
                    || (Boolean(_importacaoTabelaFrete.Parametros.list[i].ParametroBase.val) && _importacaoTabelaFrete.Parametros.list[i].ParametroBase.val == _parametro.ParametroBase.val())
                    || _importacaoTabelaFrete.Parametros.list[i].ParametroBase.val == _parametro.ParametroBase.val())) {
                exibirMensagem(tipoMensagem.aviso, "Item já existente", "O item já foi cadastrado.");
                return;
            }
        }

        _importacaoTabelaFrete.Parametros.list.push(SalvarListEntity(_parametro));

        recarregarGridParametros();

        limparCamposParametro();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function excluirTodosParametroClick() {
    exibirConfirmacao("Confirmação", "Deseja realmente excluir todos os parâmetros adicionados?", function () {
        _importacaoTabelaFrete.Parametros.list = new Array();
        recarregarGridParametros();
    });
}

function limparCamposParametro() {
    LimparCampos(_parametro);

    var tipoValor = $("#" + _parametro.TipoValor.id + " option:not(:disabled):first").eq(0).val();
    _parametro.TipoValor.val(tipoValor);
}

function montarParametros() {
    if (_tabelaFrete.ParametroBase != 0) {

        _parametro.ParametroBase.visible(true);
        _parametro.ParametroBase.required(true);

        switch (_tabelaFrete.ParametroBase) {
            case EnumTipoParametroBaseTabelaFrete.Distancia:
                setarParametroBase(_tabelaFrete.Distancias, "Descricao", "Codigo", EnumTipoParametroBaseTabelaFrete.Distancia);
                break;
            case EnumTipoParametroBaseTabelaFrete.ModeloReboque:
                setarParametroBase(_tabelaFrete.ModelosReboque, "Modelo.Descricao", "Modelo.Codigo", EnumTipoParametroBaseTabelaFrete.ModeloReboque);
                break;
            case EnumTipoParametroBaseTabelaFrete.ModeloTracao:
                setarParametroBase(_tabelaFrete.ModelosTracao, "Modelo.Descricao", "Modelo.Codigo", EnumTipoParametroBaseTabelaFrete.ModeloTracao);
                break;
            case EnumTipoParametroBaseTabelaFrete.NumeroEntrega:
                setarParametroBase(_tabelaFrete.NumeroEntregas, "Descricao", "Codigo", EnumTipoParametroBaseTabelaFrete.NumeroEntrega);
                break;
            case EnumTipoParametroBaseTabelaFrete.Peso:
                setarParametroBase(_tabelaFrete.PesosTransportados, "Descricao", "Codigo", EnumTipoParametroBaseTabelaFrete.Peso);
                break;
            case EnumTipoParametroBaseTabelaFrete.TipoCarga:
                setarParametroBase(_tabelaFrete.TiposCargas, "Tipo.Descricao", "Tipo.Codigo", EnumTipoParametroBaseTabelaFrete.TipoCarga);
                break;
            default:
                break;
        }
    }
    else {
        _parametro.ParametroBase.visible(false);
        _parametro.ParametroBase.required(false);

        _parametro.ParametroBase.options(new Array());
    }

    setarItensParametroBase();
}

function setarParametroBase(parametros, paramDescricao, paramCodigo) {
    var parametrosAdd = new Array();

    for (var i = 0; i < parametros.length; i++)
        parametrosAdd.push({ text: getProperty(parametros[i], paramDescricao), value: getProperty(parametros[i], paramCodigo) });

    parametrosAdd.push({ text: "Parâmetro na Coluna", value: "0" });

    _parametro.ParametroBase.options(parametrosAdd);
}

function setarItensParametroBase() {
    var itens = new Array();

    if (_tabelaFrete.ParametroBase != EnumTipoParametroBaseTabelaFrete.Distancia)
        itens = itens.concat(obterItensParametroBase(_tabelaFrete.Distancias, "Descricao", "Codigo", EnumTipoParametroBaseTabelaFrete.Distancia));

    if (_tabelaFrete.ParametroBase != EnumTipoParametroBaseTabelaFrete.ComponenteFrete)
        itens = itens.concat(obterItensParametroBase(_tabelaFrete.ComponentesFrete, "Componente.Descricao", "Codigo", EnumTipoParametroBaseTabelaFrete.ComponenteFrete, "Componente.Tipo"));

    if (_tabelaFrete.ParametroBase != EnumTipoParametroBaseTabelaFrete.ModeloReboque)
        itens = itens.concat(obterItensParametroBase(_tabelaFrete.ModelosReboque, "Modelo.Descricao", "Modelo.Codigo", EnumTipoParametroBaseTabelaFrete.ModeloReboque));

    if (_tabelaFrete.ParametroBase != EnumTipoParametroBaseTabelaFrete.ModeloTracao)
        itens = itens.concat(obterItensParametroBase(_tabelaFrete.ModelosTracao, "Modelo.Descricao", "Modelo.Codigo", EnumTipoParametroBaseTabelaFrete.ModeloTracao));

    if (_tabelaFrete.ParametroBase != EnumTipoParametroBaseTabelaFrete.NumeroEntrega)
        itens = itens.concat(obterItensParametroBase(_tabelaFrete.NumeroEntregas, "Descricao", "Codigo", EnumTipoParametroBaseTabelaFrete.NumeroEntrega));

    if (_tabelaFrete.ParametroBase != EnumTipoParametroBaseTabelaFrete.Peso)
        itens = itens.concat(obterItensParametroBase(_tabelaFrete.PesosTransportados, "Descricao", "Codigo", EnumTipoParametroBaseTabelaFrete.Peso));

    if (_tabelaFrete.ParametroBase != EnumTipoParametroBaseTabelaFrete.TipoCarga && !_tabelaFrete.NaoPermitirLancarValorPorTipoDeCarga)
        itens = itens.concat(obterItensParametroBase(_tabelaFrete.TiposCargas, "Tipo.Descricao", "Tipo.Codigo", EnumTipoParametroBaseTabelaFrete.TipoCarga));

    if (_tabelaFrete.ParametroBase != EnumTipoParametroBaseTabelaFrete.TipoEmbalagem)
        itens = itens.concat(obterItensParametroBase(_tabelaFrete.TipoEmbalagens, "TipoEmbalagem.Descricao", "TipoEmbalagem.Codigo", EnumTipoParametroBaseTabelaFrete.TipoEmbalagem));

    if (_tabelaFrete.PossuiValorBase)
        itens.push({ text: "Valor Base", value: "ValorBase_0" });

    if (_tabelaFrete.PossuiMinimoGarantido)
        itens.push({ text: "Valor Mínimo Garantido", value: "ValorMinimoGarantido_0" });

    if (_tabelaFrete.PermiteValorAdicionalEntregaExcedente === true && _tabelaFrete.NumeroEntregas.length > 0)
        itens.push({ text: "Por entrega excedente", value: "EntregaExcedente_0" });

    if (_tabelaFrete.PermiteValorAdicionalPesoExcedente === true && Globalize.parseFloat(_tabelaFrete.PesoExcedente) > 0 && _tabelaFrete.PesosTransportados.length > 0)
        itens.push({ text: "A cada " + _tabelaFrete.PesoExcedente + " " + _tabelaFrete.PesosTransportados[0].UnidadeMedida.Sigla + " excedente", value: "PesoExcedente_0" });

    _parametro.ItemParametroBase.options(itens);

    adicionarAutomaticamenteCombinacoesParametroItem();
    adicionarAutomaticamenteCombinacoesItemSemParametro();
}

function obterItensParametroBase(parametros, paramDescricao, paramCodigo, tipo, param3) {
    var itensAdd = new Array();

    if (parametros != null && parametros.length > 0)
        for (var i = 0; i < parametros.length; i++)
            itensAdd.push({ text: getProperty(parametros[i], paramDescricao), value: tipo + "_" + getProperty(parametros[i], paramCodigo) + (param3 != null && param3 != "" ? "_" + getProperty(parametros[i], param3) : "") });

    return itensAdd;
}

function adicionarAutomaticamenteCombinacoesParametroItem() {
    if (_parametro.ParametroBase.options().length == 0 || _parametro.ItemParametroBase.options().length == 0)
        return;

    var count = 1;
    $.each(_parametro.ParametroBase.options(), function (i, parametro) {
        $.each(_parametro.ItemParametroBase.options(), function (i, itemParametro) {
            var parametroMap = new ParametroMap();

            parametroMap.ParametroBase.val(parametro.value);
            parametroMap.ItemParametroBase.val(itemParametro.value);
            parametroMap.TipoValor.val(EnumTipoCampoValorTabelaFrete.ValorFixo);
            parametroMap.Coluna.val(count);

            if (parametroMap.ParametroBase.val() != 0 && (!Boolean(_importacaoTabelaFrete.ColunaParametrosBase.val()) && _importacaoTabelaFrete.ColunaParametrosBase.val() != "0")) {
                _importacaoTabelaFrete.Parametros.list.push(SalvarListEntity(parametroMap));
                count++;
            }
        });
    });

    recarregarGridParametros();
}

function adicionarAutomaticamenteCombinacoesItemSemParametro() {
    if (!(_parametro.ParametroBase.options().length == 0 && _parametro.ItemParametroBase.options().length > 0))
        return;

    var count = 1;
    $.each(_parametro.ItemParametroBase.options(), function (i, itemParametro) {
        var parametroMap = new ParametroMap();

        parametroMap.ItemParametroBase.val(itemParametro.value);
        changeItemParametroBase(itemParametro.value);
        parametroMap.TipoValor.val(_parametro.TipoValor.val());
        parametroMap.Coluna.val(count);

        _importacaoTabelaFrete.Parametros.list.push(SalvarListEntity(parametroMap));
        count++;
    });

    changeItemParametroBase(_parametro.ItemParametroBase.val());
    recarregarGridParametros();
}

function getProperty(obj, prop) {

    if (typeof obj === 'undefined')
        return null;

    if (typeof prop === 'undefined')
        return null;

    var _index = prop.indexOf('.');

    if (_index > -1)
        return getProperty(obj[prop.substring(0, _index)], prop.substr(_index + 1));

    return obj[prop];
}
