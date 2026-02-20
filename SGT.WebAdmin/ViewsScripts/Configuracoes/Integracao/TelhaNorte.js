var _integracaoTelhaNorte;
var _gridIntegracaoTelhaNorte;
var _integracaoTelhaNorteParametrosObterToken;

var IntegracaoTelhaNorte = function () {
    this.URLTelhaNorte = PropertyEntity({ text: "URL Carga:", maxlength: 250 });
    this.URLPedidoTelhaNorte = PropertyEntity({ text: "URL Pedido:", maxlength: 250 });
    this.URLObterToken = PropertyEntity({ text: "URL Obter Token:", maxlength: 250 });

    this.Adicionar = PropertyEntity({ type: types.event, eventClick: adicionarChaveValorIntegracaoTelhaNorteClick, idBtn: guid(), text: "Adicionar Parâmetro Obter Token" });
    this.Grid = PropertyEntity({ type: types.local, val: ko.observableArray([]) });

    this.Grid.val.subscribe(function () {
        recarregarGridIntegracaoTelhaNorte();
    });
}

var ParametroIntegracaoTelhaNorte = function () {
    this.Chave = PropertyEntity({ text: "*Chave:", maxlength: 50, required: true });
    this.Valor = PropertyEntity({ text: "*Valor:", maxlength: 300, required: true });

    this.Adicionar = PropertyEntity({ type: types.event, eventClick: adicionarParametroIntegracaoTelhaNorteClick, idBtn: guid(), text: "Adicionar" });
}

function loadIntegracaoTelhaNorte() {
    $("#knockoutConfiguracaoIntegracao").append($("#knockoutIntegracaoTelhaNorte"));

    _integracaoTelhaNorte = new IntegracaoTelhaNorte();
    KoBindings(_integracaoTelhaNorte, "knockoutIntegracaoTelhaNorte");

    _integracaoTelhaNorteParametrosObterToken = new ParametroIntegracaoTelhaNorte();
    KoBindings(_integracaoTelhaNorteParametrosObterToken, "knockoutTelhaNorteObterToken");

    carregarGridConfiguracaoIntegracaoTelhaNorte();
}

function carregarGridConfiguracaoIntegracaoTelhaNorte() {
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Excluir", id: guid(), metodo: excluirConfiguracaoIntegracaoTelhaNorteClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Chave", title: "Chave", width: "50%" },
        { data: "Valor", title: "Valor", width: "50%" }
    ];

    _gridIntegracaoTelhaNorte = new BasicDataTable(_integracaoTelhaNorte.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });
    _gridIntegracaoTelhaNorte.CarregarGrid([]);
}

function excluirConfiguracaoIntegracaoTelhaNorteClick(registroSelecionado) {
    var registros = _gridIntegracaoTelhaNorte.BuscarRegistros();

    for (var i = 0; i < registros.length; i++) {
        if (registros[i].Codigo == registroSelecionado.Codigo) {
            registros.splice(i, 1);
            break;
        }
    }

    _integracaoTelhaNorte.Grid.val(registros);
}

function adicionarChaveValorIntegracaoTelhaNorteClick() {
    $("#divModalTelhaNorteObterToken")
        .modal('show')
        .one('hidden.bs.modal', function () {
            LimparCampos(_integracaoTelhaNorteParametrosObterToken);
        });
}

function adicionarParametroIntegracaoTelhaNorteClick() {
    if (!ValidarCamposObrigatorios(_integracaoTelhaNorteParametrosObterToken)) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Preencha os campos obrigatórios");
        return;
    }

    _integracaoTelhaNorte.Grid.val.push({
        Codigo: guid(),
        Chave: _integracaoTelhaNorteParametrosObterToken.Chave.val(),
        Valor: _integracaoTelhaNorteParametrosObterToken.Valor.val()
    });

    Global.fecharModal('divModalTelhaNorteObterToken');
}

function recarregarGridIntegracaoTelhaNorte() {
    _gridIntegracaoTelhaNorte.CarregarGrid(_integracaoTelhaNorte.Grid.val());
}

function preencherIntegracaoTelhaNorte(dados) {
    PreencherObjetoKnout(_integracaoTelhaNorte, { Data: dados });
    _integracaoTelhaNorte.Grid.val(dados.Chaves);
}

function obterDadosIntegracaoTelhaNorte() {
    return {
        DadosIntegracaoTelhaNorte: RetornarObjetoPesquisa(_integracaoTelhaNorte),
        ParametrosIntegracaoTelhaNorte: _gridIntegracaoTelhaNorte.BuscarRegistros()
    };
}