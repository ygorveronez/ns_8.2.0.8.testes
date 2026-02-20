var _layoutImportacao
var _gridLayout;

var LayoutImportacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoTabelaFrete = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "Descrição:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 500 });

    this.Grid = PropertyEntity({ type: types.local, idGrid: guid() });

    this.Salvar = PropertyEntity({ eventClick: salvarLayoutAtualClick, type: types.event, text: "Salvar Layout Atual", visible: ko.observable(true) });
};

function loadLayoutImportacao() {
    _layoutImportacao = new LayoutImportacao();
    KoBindings(_layoutImportacao, "knockoutLayoutImportacao");

    _layoutImportacao.CodigoTabelaFrete.val(_importacaoTabelaFrete.TabelaFrete.codEntity());

    Global.abrirModal("divModalLayoutImportacao");

    var aplicar = { descricao: "Aplicar", id: guid(), metodo: aplicarClick, icone: "" };
    var excluir = { descricao: "Excluir", id: guid(), metodo: excluirClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [aplicar, excluir] };

    _gridLayout = new GridView(_layoutImportacao.Grid.idGrid, "ImportacaoTabelaFrete/ConsultarLayoutImportacao", _layoutImportacao, menuOpcoes);
    _gridLayout.CarregarGrid();
}

function aplicarClick(registro) {
    executarReST("ImportacaoTabelaFrete/BuscarLayoutPorCodigo", { CodigoTabelaFrete: _importacaoTabelaFrete.TabelaFrete.codEntity(), Codigo: registro.Codigo }, function (retorno) {
        if (retorno.Success) {
            PreencherColunas(JSON.parse(retorno.Data));
            LimparCamposLayoutImportacao();
            exibirMensagem(tipoMensagem.ok, "Sucesso", "Layout de importação aplicado com sucesso!");
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }
    }, null);
}

function excluirClick(registro) {
    executarReST("ImportacaoTabelaFrete/ExcluirLayoutPorCodigo", { CodigoTabelaFrete: _importacaoTabelaFrete.TabelaFrete.codEntity(), Codigo: registro.Codigo }, function (retorno) {
        if (retorno.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso", "Layout de importação excluído com sucesso!");
            _gridLayout.CarregarGrid();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }
    }, null);
}

function salvarLayoutAtualClick() {
    var dados = {
        CodigoTabelaFrete: _layoutImportacao.CodigoTabelaFrete.val(),
        Descricao: _layoutImportacao.Descricao.val(),
        JSONColunas: JSON.stringify(ObterJsonColunas())
    };

    executarReST("ImportacaoTabelaFrete/SalvarLayoutAtual", dados, function (retorno) {
        if (retorno.Success) {
            exibirMensagem(tipoMensagem.ok, "Sucesso", "Layout de importação salvo com sucesso!");
            _gridLayout.CarregarGrid();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }
    }, null);
}

function ObterJsonColunas() {
    var json = {};

    for (const key in _importacaoTabelaFrete) {
        if (key.startsWith("Coluna")) {
            var chave = key;
            json = { ...json, [chave]: _importacaoTabelaFrete[key].val() };
        }

        if (key == "LinhaInicioDados" || key == "FreteValidoParaQualquerOrigem" || key == "FreteValidoParaQualquerDestino") {
            var chave = key;
            json = { ...json, [chave]: _importacaoTabelaFrete[key].val() };
        }
    };

    json = { ...json, Parametros: JSON.stringify(recursiveListEntity(_importacaoTabelaFrete.Parametros)) };

    return json;
}

function PreencherColunas(json) {
    for (const key in json) {
        if (key.startsWith("Coluna")) {
            var chave = key;
            if (_importacaoTabelaFrete.hasOwnProperty(chave))
                _importacaoTabelaFrete[chave].val(json[key]);
            continue;
        }

        if (key == "LinhaInicioDados" || key == "FreteValidoParaQualquerOrigem" || key == "FreteValidoParaQualquerDestino") {
            var chave = key;
            if (_importacaoTabelaFrete.hasOwnProperty(chave))
                _importacaoTabelaFrete[chave].val(json[key]);
            continue;
        }

        if (key == "Parametros") {
            adicionarParametrosJSON(JSON.parse(json['Parametros']));
        }
    };
}

function adicionarParametrosJSON(parametros) {
    _importacaoTabelaFrete.Parametros.list = [];
    recarregarGridParametros();

    for (var i = 0; i < parametros.length; i++) {
        _parametro.ParametroBase.val(parametros[i].ParametroBase);
        _parametro.ItemParametroBase.val(parametros[i].ItemParametroBase);
        _parametro.TipoValor.val(parametros[i].TipoValor);
        _parametro.Coluna.val(parametros[i].Coluna);

        _importacaoTabelaFrete.Parametros.list.push(SalvarListEntity(_parametro));
    }

    recarregarGridParametros();
    limparCamposParametro();
}

function LimparCamposLayoutImportacao() {
    Global.fecharModal("divModalLayoutImportacao");
    LimparCampo(_layoutImportacao.Descricao);
}