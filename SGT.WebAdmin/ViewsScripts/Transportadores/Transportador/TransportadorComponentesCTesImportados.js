/// <reference path="Transportador.js" />
/// <reference path="../../Consultas/ComponenteFrete.js" />

var _gridComponentesCTesImportados;
var _componenteCTeImportado;

var ComponenteCTeImportado = function () {
    this.ComponentesCTesImportados = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.ComponentesCTesImportados, type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });
    this.ComponenteFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.Transportadores.Transportador.ComponenteFrete.getRequiredFieldDescription(), idBtnSearch: guid() });
    this.Descricao = PropertyEntity({ val: ko.observable(""), text: Localization.Resources.Transportadores.Transportador.Descricao.getRequiredFieldDescription(), required: true, maxlength: 150 });

    this.ComponentesCTesImportados.val.subscribe(recarregarGridComponentesCTesImportados);

    this.Adicionar = PropertyEntity({ eventClick: adicionarComponenteCTeImportadoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
};

function loadComponentesCTeImportados() {
    _componenteCTeImportado = new ComponenteCTeImportado();
    KoBindings(_componenteCTeImportado, "knockoutTransportadorComponentesCTesImportados");

    new BuscarComponentesDeFrete(_componenteCTeImportado.ComponenteFrete);

    loadGridComponentesCTeImportados();
}

function loadGridComponentesCTeImportados() {
    var linhasPorPaginas = 5;
    var opcaoRemover = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: removerComponenteCTeImportadoClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoRemover] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoComponenteFrete", visible: false },
        { data: "Descricao", title: Localization.Resources.Transportadores.Transportador.Descricao, width: "40%", className: "text-align-left" },
        { data: "DescricaoComponenteFrete", title: Localization.Resources.Transportadores.Transportador.ComponenteFrete, width: "25%", className: "text-align-left" }
    ];

    _gridComponentesCTesImportados = new BasicDataTable(_componenteCTeImportado.ComponentesCTesImportados.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridComponentesCTesImportados.CarregarGrid([]);
}

function visibilidadeAbaComponentesCTesImportados(valor) {
    var idLi = "#liTabComponentesCTesImportados";
    
    if (valor && _CONFIGURACAO_TMS.ControlarValoresComponentesCTe)
        $(idLi).show();
    else
        $(idLi).hide();
}

function adicionarComponenteCTeImportadoClick() {
    if (!ValidarCamposObrigatorios(_componenteCTeImportado)) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Preencha os campos obrigatórios.");
        return;
    }

    var descricao = _componenteCTeImportado.Descricao.val();
    var componenteFrete = {
        Codigo: _componenteCTeImportado.ComponenteFrete.codEntity(),
        Descricao: _componenteCTeImportado.ComponenteFrete.val()
    };

    if (!validarDescricao(descricao)) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Já existe um registro cadastrado com essa descrição.");
        return;
    }

    var novoRegistro = {
        Codigo: guid(),
        Descricao: descricao,
        DescricaoComponenteFrete: componenteFrete.Descricao,
        CodigoComponenteFrete: componenteFrete.Codigo
    }

    var registros = _componenteCTeImportado.ComponentesCTesImportados.val();
    registros.push(novoRegistro);

    _componenteCTeImportado.ComponentesCTesImportados.val(registros);

    LimparCampo(_componenteCTeImportado.Descricao);
    LimparCampoEntity(_componenteCTeImportado.ComponenteFrete);
}

function validarDescricao(descricao) {
    var registros = _gridComponentesCTesImportados.BuscarRegistros();

    for (var i = 0; i < registros.length; i++) {
        if (registros[i].Descricao == descricao)
            return false;
    }

    return true;
}

function removerComponenteCTeImportadoClick(registroSelecionado) {
    var registros = _gridComponentesCTesImportados.BuscarRegistros();

    for (var i = 0; i < registros.length; i++) {
        if (registros[i].Codigo == registroSelecionado.Codigo) {
            registros.splice(i, 1);
            _componenteCTeImportado.ComponentesCTesImportados.val(registros);
            return;
        }
    }
}

function recarregarGridComponentesCTesImportados() {
    var registros = _componenteCTeImportado.ComponentesCTesImportados.val();
    _gridComponentesCTesImportados.CarregarGrid(registros);
}

function limparCamposTransportadorComponenteCTeImportado() {
    _componenteCTeImportado.ComponentesCTesImportados.val([]);
    LimparCampos(_componenteCTeImportado);
}

function preencherGridComponentesCTeImportados(data) {
    _componenteCTeImportado.ComponentesCTesImportados.val(data);
}