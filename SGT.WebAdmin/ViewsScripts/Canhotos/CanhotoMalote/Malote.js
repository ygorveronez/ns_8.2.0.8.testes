
//*******MAPEAMENTO KNOUCKOUT*******
var _malote;
var _usuarioLogado = {
    Codigo: 0,
    Nome: ""
};

var Malote = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.QuantidadeCanhotos = PropertyEntity({ text: "Quantidade Canhotos:", getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.ProtocoloMalote = PropertyEntity({ text: "Número Protocolo:", val: ko.observable(""), def: "" });
    this.DataMalote = PropertyEntity({ text: "* Data Envio: ", getType: typesKnockout.dateTime, required: true });
    this.OperadorMalote = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.OperadorManualMalote = PropertyEntity({ eventChange: OperadorManualChange, text: "* Operador (Editável):", required: true });
    this.OrigemMalote = PropertyEntity({ text: "* Origem:" });
    this.DestinoMalote = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "* Destino:", idBtnSearch: guid(), required: true });

    this.GerarMalote = PropertyEntity({ type: types.event, eventClick: gerarMaloteClick, text: "Salvar" });
}

//*******EVENTOS*******
function loadMalotes() {
    _malote = new Malote();
    KoBindings(_malote, "knockoutMalote");

    new BuscarFuncionario(_malote.OperadorMalote, PreencheCampoOperador);
    new BuscarFilial(_malote.DestinoMalote);

    CarregaUsuarioLogado();
}

function gerarMaloteClick(e, sender) {
    var dadosPesquisa = RetornarObjetoPesquisa(_knoutPesquisar);
    var dadosMalote = RetornarObjetoPesquisa(_malote);
    var dadosGrid = {
        SelecionarTodos: _knoutPesquisar.SelecionarTodos.val(),
        Selecionados: JSON.stringify(_gridCanhotos.ObterMultiplosSelecionados()),
        NaoSelecionados: JSON.stringify(_gridCanhotos.ObterMultiplosNaoSelecionados())
    };

    var dados = $.extend({}, dadosPesquisa, dadosGrid, dadosMalote);
    
    executarReST("CanhotoMalote/Adicionar", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                LimparCamposMalote();
                Global.fecharModal('divModalMalote');
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function OperadorManualChange() {
    if (_malote.OperadorMalote.codEntity() > 0)
        PreencheCampoOperador({ Nome: _malote.OperadorManualMalote.val(), Codigo: 0 });
}


//*******METODOS******* 
function SalvarMalote() {
    LimparCampos(_malote);
    BuscarProximoProtocolo(function (data) {
        PreencherObjetoKnout(_malote, { Data: data });
        var totalSelecionado = QuantidadeCanhotosSelecionados();
        _malote.QuantidadeCanhotos.val(totalSelecionado);
        PreencheCampoOperador(_usuarioLogado);
                
        Global.abrirModal('divModalMalote');
    });
}

function PreencheCampoOperador(usuario) {
    _malote.OperadorManualMalote.val(usuario.Nome);
    _malote.OperadorMalote.val(usuario.Nome);
    _malote.OperadorMalote.entityDescription(usuario.Nome);
    _malote.OperadorMalote.codEntity(usuario.Codigo);
}

function CarregaUsuarioLogado() {
    executarReST("Usuario/DadosUsuarioLogado", {}, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false && arg.Data != null) {
                _usuarioLogado.Nome = arg.Data.Nome;
                _usuarioLogado.Codigo = arg.Data.Codigo;
            }
        }
    });
}


function BuscarProximoProtocolo(callback) {
    var dados = {
        Transportdor: _knoutPesquisar.Transportador.codEntity(),
        Filial: _knoutPesquisar.Filial.codEntity(),
    };
    executarReST("CanhotoMalote/ObterProximoProtocolo", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                callback(arg.Data);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function QuantidadeCanhotosSelecionados() {
    var quantidade = 0;
    if (_knoutPesquisar.SelecionarTodos.val()) {
        var todos = _gridCanhotos.NumeroRegistros();
        var naoSelecionados = _gridCanhotos.ObterMultiplosNaoSelecionados().length;

        quantidade = todos - naoSelecionados;
    } else {
        quantidade = _gridCanhotos.ObterMultiplosSelecionados().length;
    }

    return quantidade;
}