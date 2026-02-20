//*******MAPEAMENTO KNOUCKOUT*******

var _gridFronteiraRotaFrete;
var _fronteiraRotaFrete;
var _adicionarFronteira;

var FronteiraRotaFrete = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Fronteira = PropertyEntity({ type: types.event, text: Localization.Resources.Logistica.RotaFrete.AdicionarPracaPedagio, eventClick: exibirModalFronteiraRotaFreteClick });
};

var AdicionarFronteira = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Latitude = PropertyEntity({ text: Localization.Resources.Logistica.RotaFrete.Latitude.getFieldDescription(), visible: false });
    this.Longitude = PropertyEntity({ text: Localization.Resources.Logistica.RotaFrete.Longitude.getFieldDescription(), visible: false });
    this.Endereco = PropertyEntity({ text: Localization.Resources.Logistica.RotaFrete.Endereco.getFieldDescription(), visible: false });
    this.Numero = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Numero.getFieldDescription(), visible: false });
    this.CEP = PropertyEntity({ text: Localization.Resources.Logistica.RotaFrete.CEP.getFieldDescription(), visible: false });
    this.Nome = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Nome.getFieldDescription(), visible: false });
    this.CodigoIBGE = PropertyEntity({ text: Localization.Resources.Logistica.RotaFrete.CodigoIBGE.getFieldDescription(), type: typesKnockout.int, visible: false });
    this.TempoPermanenciaMinutos = PropertyEntity({ text: Localization.Resources.Logistica.RotaFrete.TempoEstimadoPermanenciaMinutos.getFieldDescription(), type: typesKnockout.int, visible: false });
    this.Ordem = PropertyEntity({ text: Localization.Resources.Logistica.RotaFrete.Ordem.getFieldDescription(), type: typesKnockout.int, visible: false });

    this.Descricao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), idBtnSearch: guid(), required: true, enable: ko.observable(true) });
    this.Localidade = PropertyEntity({ text: Localization.Resources.Logistica.RotaFrete.Localidade.getFieldDescription(), enable: false });
    this.TempoPermanencia = PropertyEntity({ text: ko.observable(Localization.Resources.Logistica.RotaFrete.TempoEstimadoPermanencia.getFieldDescription()), getType: typesKnockout.mask, mask: "000:00", def: "", required: ko.observable(false) });

    this.Adicionar = PropertyEntity({ type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, eventClick: adicionarFronteiraRotaFreteClick, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, eventClick: atualizarFronteiraRotaFreteClick, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, eventClick: cancelarFronteiraRotaFreteClick, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, eventClick: excluirFronteiraRotaFreteClick, visible: ko.observable(false) });
}

//*******EVENTOS*******

function LoadFronteiraRotaFrete() {
    _fronteiraRotaFrete = new FronteiraRotaFrete();
    KoBindings(_fronteiraRotaFrete, "knockoutFronteiraRotaFrete");

    _adicionarFronteira = new AdicionarFronteira();
    KoBindings(_adicionarFronteira, "knockoutAdicionarFronteira");

    new BuscarClientes(_adicionarFronteira.Descricao, retornoCliente, null, null, null, null, null, null, null, null, null, null, null, null, null, null, true);

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarFronteiraRotaFreteClick
        }]
    };
 
    var header = [
        { data: "Codigo", visible: false },
        { data: "Latitude", visible: false },
        { data: "Longitude", visible: false },
        { data: "Endereco", visible: false },
        { data: "Numero", visible: false },
        { data: "CEP", visible: false },
        { data: "Nome", visible: false },
        { data: "CodigoIBGE", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "40%" },
        { data: "Localidade", title: Localization.Resources.Gerais.Geral.Localidade, width: "20%" },
        { data: "TempoMedioPermanenciaFronteira", title: Localization.Resources.Gerais.Geral.TempoEstimadoPermanencia, width: "20%" },
        { data: "TempoMedioPermanenciaFronteiraMinutos", visible: false },
        { data: "Ordem", visible: false }
    ];

    _gridFronteiraRotaFrete = new BasicDataTable(_fronteiraRotaFrete.Grid.id, header, menuOpcoes, { column: 11, dir: orderDir.asc });

    _fronteiraRotaFrete.Fronteira.basicTable = _gridFronteiraRotaFrete;

    RecarregarGridFronteiraRotaFrete();

    if (_ConfiguracaoControleEntrega.ObrigatorioInformarFreetime) {
        _adicionarFronteira.TempoPermanencia.required(true);
        _adicionarFronteira.TempoPermanencia.text("*" + _adicionarFronteira.TempoPermanencia.text());
    }
}

function RecarregarGridFronteiraRotaFrete() {
    _gridFronteiraRotaFrete.CarregarGrid(_rotaFrete.Fronteiras.val() || []);
}

function LimparCamposFronteiraRotaFrete() {
    LimparCampos(_fronteiraRotaFrete);
    _gridFronteiraRotaFrete.CarregarGrid(new Array());
}

function exibirModalFronteiraRotaFreteClick() {
    var habilitado = false;

    limparModalFronteiraRotaFrete();
    controlarBotoesModalFronteira(habilitado);
    _adicionarFronteira.Descricao.enable(true);
    Global.abrirModal('divModalAdicionarFronteira');
}

function editarFronteiraRotaFreteClick(registroSelecionado) {
    var habilitado = true;

    controlarBotoesModalFronteira(habilitado);
    _adicionarFronteira.Descricao.enable(false);
    limparModalFronteiraRotaFrete();
    preencherDadosFronteiraRotaFrete(registroSelecionado);
    Global.abrirModal('divModalAdicionarFronteira');
}

function fecharModalFronteiraRotaFrete() {
    Global.fecharModal('divModalAdicionarFronteira');
}

function adicionarFronteiraRotaFreteClick() {
    if (_adicionarFronteira.TempoPermanencia.val() == "00:00")
        _adicionarFronteira.TempoPermanencia.val("");

    if (ValidarCamposObrigatorios(_adicionarFronteira)) {
        //_adicionarFronteira.Descricao.codEntity(guid());

        var fronteiras = _gridFronteiraRotaFrete.BuscarRegistros();

        if (!existeFronteira(fronteiras)) {
            fronteiras.push(obterFronteiraRotaFreteSalvar());

            _gridFronteiraRotaFrete.CarregarGrid(fronteiras);

            fecharModalFronteiraRotaFrete();
        }
    } else
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Logistica.RotaFrete.PorFavorInformeCamposObrigatorios);
}

function atualizarFronteiraRotaFreteClick() {
    if (_adicionarFronteira.TempoPermanencia.val() == "00:00")
        _adicionarFronteira.TempoPermanencia.val("");

    if (ValidarCamposObrigatorios(_adicionarFronteira)) {
        var fronteiras = _gridFronteiraRotaFrete.BuscarRegistros();

        for (var i = 0; i < fronteiras.length; i++) {
            if (_adicionarFronteira.Codigo.val() == fronteiras[i].Codigo) {
                fronteiras[i] = obterFronteiraRotaFreteSalvar();
                break;
            }
        }

        _gridFronteiraRotaFrete.CarregarGrid(fronteiras);

        fecharModalFronteiraRotaFrete();
    } else
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Logistica.RotaFrete.PorFavorInformeCamposObrigatorios);
}

function cancelarFronteiraRotaFreteClick() {
    fecharModalFronteiraRotaFrete();
}

function excluirFronteiraRotaFreteClick() {

    var fronteirasGrid = _gridFronteiraRotaFrete.BuscarRegistros();

    for (var i = 0; i < fronteirasGrid.length; i++) {
        if (_adicionarFronteira.Codigo.val() == fronteirasGrid[i].Codigo) {
            fronteirasGrid.splice(i, 1);
            break;
        }
    }

    _gridFronteiraRotaFrete.CarregarGrid(fronteirasGrid);
    fecharModalFronteiraRotaFrete();
}

function obterFronteiraRotaFreteSalvar() {
    return {
        Codigo: _adicionarFronteira.Codigo.val(),
        Descricao: _adicionarFronteira.Descricao.val(),
        Localidade: _adicionarFronteira.Localidade.val(),
        TempoMedioPermanenciaFronteira: _adicionarFronteira.TempoPermanencia.val(),
        Latitude: _adicionarFronteira.Latitude.val(),
        Longitude: _adicionarFronteira.Longitude.val(),
        Endereco: _adicionarFronteira.Endereco.val(),
        Numero: _adicionarFronteira.Numero.val(),
        CEP: _adicionarFronteira.CEP.val(),
        Nome: _adicionarFronteira.Nome.val(),
        CodigoIBGE: _adicionarFronteira.CodigoIBGE.val(),
        TempoMedioPermanenciaFronteiraMinutos: _adicionarFronteira.TempoPermanenciaMinutos.val(),
        Ordem: _adicionarFronteira.Ordem.val()
    };
}

function preencherDadosFronteiraRotaFrete(registroSelecionado) {
    _adicionarFronteira.Codigo.val(registroSelecionado.Codigo);
    _adicionarFronteira.Descricao.codEntity(registroSelecionado.Codigo);
    _adicionarFronteira.Descricao.val(registroSelecionado.Descricao);
    _adicionarFronteira.Localidade.val(registroSelecionado.Localidade);
    _adicionarFronteira.TempoPermanencia.val(registroSelecionado.TempoMedioPermanenciaFronteira);

    _adicionarFronteira.Latitude.val(registroSelecionado.Latitude);
    _adicionarFronteira.Longitude.val(registroSelecionado.Longitude);
    _adicionarFronteira.Endereco.val(registroSelecionado.Endereco);
    _adicionarFronteira.Numero.val(registroSelecionado.Numero);
    _adicionarFronteira.CEP.val(registroSelecionado.CEP);
    _adicionarFronteira.Nome.val(registroSelecionado.Nome);
    _adicionarFronteira.CodigoIBGE.val(registroSelecionado.CodigoIBGE);
    _adicionarFronteira.TempoPermanenciaMinutos.val(registroSelecionado.TempoMedioPermanenciaFronteiraMinutos);
    _adicionarFronteira.Ordem.val(registroSelecionado.Ordem);
}

function controlarBotoesModalFronteira(habilitado) {
    _adicionarFronteira.Adicionar.visible(!habilitado);
    _adicionarFronteira.Atualizar.visible(habilitado);
    _adicionarFronteira.Cancelar.visible(habilitado);
    _adicionarFronteira.Excluir.visible(habilitado);
}

function limparModalFronteiraRotaFrete() {
    LimparCampos(_adicionarFronteira);
}

function retornoCliente(dados) {
    _adicionarFronteira.Codigo.val(dados.Codigo);
    _adicionarFronteira.Descricao.codEntity(dados.Codigo);
    _adicionarFronteira.Descricao.val(dados.Descricao);
    _adicionarFronteira.Localidade.val(dados.Localidade);
    _adicionarFronteira.TempoPermanencia.val(dados.TempoMedioPermanenciaFronteira);

    _adicionarFronteira.Latitude.val(dados.Latitude);
    _adicionarFronteira.Longitude.val(dados.Longitude);
    _adicionarFronteira.Endereco.val(dados.Endereco);
    _adicionarFronteira.Numero.val(dados.Numero);
    _adicionarFronteira.CEP.val(dados.CEP);
    _adicionarFronteira.Nome.val(dados.Nome);
    _adicionarFronteira.CodigoIBGE.val(dados.CodigoIBGE);
    _adicionarFronteira.TempoPermanenciaMinutos.val(dados.TempoMedioPermanenciaFronteiraMinutos);
    _adicionarFronteira.Ordem.val(dados.Ordem);
}

function existeFronteira(fronteiras) {
    var existe = false;
    for (var i = 0; i < fronteiras.length; i++) {
        if (_adicionarFronteira.Codigo.val() == fronteiras[i].Codigo) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.RegistroJaExistente, Localization.Resources.Logistica.RotaFrete.JaExisteRegistroParaFronteira);
            existe = true;
            break;
        }
    }

    return existe;
}