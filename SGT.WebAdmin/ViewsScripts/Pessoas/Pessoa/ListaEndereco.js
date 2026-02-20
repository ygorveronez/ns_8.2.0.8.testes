/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/Endereco.js" />
/// <reference path="../../Consultas/ListaEndereco.js" />
/// <reference path="Pessoa.js" />
/// <reference path="Transportador.js" />
/// <reference path="MapaEnderecoSecundario.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var PesquisaEnderecoEnderecoSecundario = function () {
    this.Logradouro = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.Endereco.getFieldDescription()), required: false, maxlength: 80, enable: ko.observable(true) });
    this.Bairro = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.Bairro.getFieldDescription()), required: false, maxlength: 80, enable: ko.observable(true) });
    this.CEP = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.CEP.getFieldDescription()), required: false });
    this.Localidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Pessoas.Pessoa.Cidade.getFieldDescription()), idBtnSearch: guid(), enable: ko.observable(true) });
    this.CodigoIBGE = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.CodigoIBGE.getFieldDescription()), required: false });

    this.Pesquisar = PropertyEntity({ eventClick: pesquisarEnderecosEnderecoSecundarioClick, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, visible: ko.observable(true) });

    this.Enderecos = PropertyEntity({ type: types.local, idGrid: guid() });
}

var _gridListaEndereco;
var _listaEndereco;
var _pesquisaEnderecoSecundario;
var _mapaListaEndereco;

var ListaEndereco = function () {
    this.Grid = PropertyEntity({ type: types.local });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoLocalidade = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DescricaoLocalidade = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });

    this.Endereco = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.EnderecoPrincipal.getRequiredFieldDescription()), required: false, maxlength: 80, enable: ko.observable(true) });
    this.Numero = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.Numero.getRequiredFieldDescription()), required: false, maxlength: 60, enable: ko.observable(true) });
    this.Telefone = PropertyEntity({ text: ko.observable(Localization.Resources.Gerais.Geral.Telefone.getRequiredFieldDescription()), required: false, getType: typesKnockout.phone, enable: ko.observable(true) });
    this.Bairro = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.Bairro.getRequiredFieldDescription()), required: false, maxlength: 40, enable: ko.observable(true) });
    this.Complemento = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.Complemento.getFieldDescription()), required: false, maxlength: 60 });
    this.CodigoDocumento = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.CodigoDocumento.getFieldDescription()), required: false, maxlength: 50 });
    this.CEP = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.CEP.getRequiredFieldDescription()), required: false, getType: typesKnockout.string });
    this.Localidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Pessoas.Pessoa.Cidade.getRequiredFieldDescription()), idBtnSearch: guid(), enable: ko.observable(true) });
    this.IE = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.IE.getFieldDescription()), required: false, maxlength: 60 });
    this.CodigoIntegracao = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.CodigoIntegracao.getFieldDescription()), visible: ko.observable(true), maxlength: 40 });

    this.TipoLogradouro = PropertyEntity({ val: ko.observable(EnumTipoLogradouro.Rua), options: EnumTipoLogradouro.obterOpcoes(), def: EnumTipoLogradouro.Rua, text: Localization.Resources.Pessoas.Pessoa.TipoLogistica.getRequiredFieldDescription(), required: false });
    this.TipoEndereco = PropertyEntity({ val: ko.observable(EnumTipoEndereco.Comercial), options: EnumTipoEndereco.obterOpcoes(), def: EnumTipoEndereco.Comercial, text: Localization.Resources.Pessoas.Pessoa.TipoEndereco.getRequiredFieldDescription(), required: false });

    this.Latitude = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.Latitude.getFieldDescription()), required: false, visible: ko.observable(true), maxlength: 20 });
    this.Longitude = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.Longitude.getFieldDescription()), required: false, visible: ko.observable(true), maxlength: 20 });
    this.TipoAreaEnderecoSecundario = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.TipoArea.getFieldDescription(), val: ko.observable(true), options: EnumTipoArea.obterPontoPoligono(), def: EnumTipoArea.Raio, enable: ko.observable(true) });
    this.TipoAreaEnderecoSecundario.val.subscribe(function () { setarTipoAreaEnderecoSecundario(); });
    this.RaioEmMetrosSecundario = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.RaioMetros), required: false, visible: ko.observable(true), maxlength: 10, enable: ko.observable(true) });
    this.RaioEmMetrosSecundario.val.subscribe(function () { setarRaioEmMetrosSecundario(); });
    this.AreaSecundario = PropertyEntity();

    this.SN_Numero = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Pessoas.Pessoa.SemNumero, def: ko.observable(false) });

    this.EnderecoDigitado = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Pessoas.Pessoa.DigitarEndereco, def: ko.observable(false) });
    this.ConsultarCEP = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable(Localization.Resources.Pessoas.Pessoa.NaoSeiCEP.getFieldDescription()), idBtnSearch: guid(), enable: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: adicionarListaEnderecoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarListaEnderecoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: limparCamposListaEndereco, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirListaEnderecoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });

    this.BuscarCoordenadas = PropertyEntity({ eventClick: BuscarCoordenadasListaEnderecoClick, type: types.event, text: Localization.Resources.Pessoas.Pessoa.BuscarCoordenadasDoEndereco, visible: ko.observable(true) });

    this.BuscarLatitudeLongitude = PropertyEntity({ eventClick: BuscarLatitudeLongitudeEndereco, type: types.event, text: Localization.Resources.Pessoas.Pessoa.BuscarLatitudeLongitude, visible: ko.observable(true), enable: ko.observable(true) });
}

//*******EVENTOS*******


function loadListaEndereco() {

    _listaEndereco = new ListaEndereco();
    KoBindings(_listaEndereco, "knockoutListaEnderecos");
    $("#liTabListaEndereco").show();
    $("#" + _listaEndereco.CEP.id).mask("00.000-000", { selectOnFocus: true, clearIfNotMatch: true });
    new BuscarLocalidades(_listaEndereco.Localidade);
    new BuscarEnderecos(_listaEndereco.ConsultarCEP, null, null, selecionarEnderecoClickEnderecoSecundario);

    _listaEndereco.Numero.val("S/N");
    _listaEndereco.Numero.enable(false);
    _listaEndereco.EnderecoDigitado.val(false);
    desabilitaCamposEnderecoEnderecoSecundario();

    $("#" + _listaEndereco.SN_Numero.id).click(verificarSNNumeroEnderecoSecundario);
    $("#" + _listaEndereco.EnderecoDigitado.id).click(digitarEnderecoEnderecoSecundario);

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarListaEnderecoClick }] };

    var header = [{ data: "Codigo", visible: false },
    { data: "Endereco", title: Localization.Resources.Pessoas.Pessoa.Endereco, width: "30%" },
    { data: "CodigoIntegracao", title: Localization.Resources.Pessoas.Pessoa.CodigoIntegracao, width: "20%" },
    { data: "Bairro", title: Localization.Resources.Pessoas.Pessoa.Bairro, width: "20%" },
    { data: "Localidade", title: Localization.Resources.Pessoas.Pessoa.Cidade, width: "20%" },
    { data: "CodigoLocalidade", visible: false },
    { data: "Numero", title: Localization.Resources.Pessoas.Pessoa.Numero, width: "10%" },
    { data: "AreaSecundario", visible: false }];


    _gridListaEndereco = new BasicDataTable(_listaEndereco.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    recarregarGridListaEndereco();
}

function recarregarGridListaEndereco() {

    var data = new Array();

    $.each(_pessoa.ListaEndereco.list, function (i, listaEndereco) {
        var listaEnderecoGrid = new Object();

        listaEnderecoGrid.Codigo = listaEndereco.Codigo.val;
        listaEnderecoGrid.Bairro = listaEndereco.Bairro.val;
        listaEnderecoGrid.CEP = listaEndereco.CEP.val;
        listaEnderecoGrid.Complemento = listaEndereco.Complemento.val;
        if (listaEndereco.CodigoDocumento != undefined && listaEndereco.CodigoDocumento != null)
            listaEnderecoGrid.CodigoDocumento = listaEndereco.CodigoDocumento.val;
        else
            listaEnderecoGrid.CodigoDocumento = "";
        listaEnderecoGrid.Endereco = listaEndereco.Endereco.val;
        listaEnderecoGrid.EnderecoDigitado = listaEndereco.EnderecoDigitado.val;
        listaEnderecoGrid.Latitude = 0;
        listaEnderecoGrid.Longitude = 0;
        listaEnderecoGrid.Numero = listaEndereco.Numero.val;
        listaEnderecoGrid.Localidade = listaEndereco.DescricaoLocalidade.val;
        listaEnderecoGrid.CodigoLocalidade = listaEndereco.CodigoLocalidade.val;
        listaEnderecoGrid.DescricaoLocalidade = listaEndereco.DescricaoLocalidade.val;
        listaEnderecoGrid.TipoEndereco = listaEndereco.TipoEndereco.val;
        listaEnderecoGrid.TipoLogradouro = listaEndereco.TipoLogradouro.val;
        listaEnderecoGrid.IE = listaEndereco.IE.val;
        listaEnderecoGrid.AreaSecundario = listaEndereco.AreaSecundario.val;
        listaEnderecoGrid.TipoAreaEnderecoSecundario = listaEndereco.TipoAreaEnderecoSecundario.val;
        listaEnderecoGrid.RaioEmMetrosSecundario = listaEndereco.RaioEmMetrosSecundario.val;
        if (listaEndereco.CodigoIntegracao != undefined && listaEndereco.CodigoIntegracao != null)
            listaEnderecoGrid.CodigoIntegracao = listaEndereco.CodigoIntegracao.val;
        else
            listaEnderecoGrid.CodigoIntegracao = "";
        listaEnderecoGrid.Telefone = listaEndereco.Telefone.val;

        data.push(listaEnderecoGrid);
    });

    _gridListaEndereco.CarregarGrid(data);
}


function excluirListaEnderecoClick(data) {
    var codigo = _listaEndereco.Codigo.val();

    $.each(_pessoa.ListaEndereco.list, function (i, listaEndereco) {
        if (codigo == listaEndereco.Codigo.val) {
            _pessoa.ListaEndereco.list.splice(i, 1);
            return false;
        }
    });

    limparCamposListaEndereco();
    recarregarGridListaEndereco();
}

function editarListaEnderecoClick(data) {
    var endereco = null;

    $.each(_pessoa.ListaEndereco.list, function (i, listaEndereco) {
        if (data.Codigo == listaEndereco.Codigo.val) {
            endereco = _pessoa.ListaEndereco.list[i];
            return;
        }
    });

    if (endereco != null) {
        _listaEndereco.Codigo.val(endereco.Codigo.val);
        _listaEndereco.Bairro.val(endereco.Bairro.val);
        _listaEndereco.CEP.val(endereco.CEP.val);
        _listaEndereco.Complemento.val(endereco.Complemento.val);
        _listaEndereco.CodigoDocumento.val(endereco.CodigoDocumento.val);
        _listaEndereco.Endereco.val(endereco.Endereco.val);
        _listaEndereco.Endereco.enable(endereco.EnderecoDigitado.val);
        _listaEndereco.EnderecoDigitado.val(endereco.EnderecoDigitado.val);
        _listaEndereco.CodigoIntegracao.val(endereco.CodigoIntegracao.val);

        if (endereco.Latitude) {
            _listaEndereco.Latitude.val(endereco.Latitude.val);
            _listaEndereco.Longitude.val(endereco.Longitude.val);
        }
        _listaEndereco.SN_Numero.val(endereco.Numero.val != "S/N");
        _listaEndereco.Numero.enable(_listaEndereco.SN_Numero.val());
        _listaEndereco.Numero.val(endereco.Numero.val);
        _listaEndereco.Localidade.val(endereco.DescricaoLocalidade.val);//.DescricaoLocalidade.val);
        _listaEndereco.Localidade.codEntity(endereco.CodigoLocalidade.val);

        _listaEndereco.TipoEndereco.val(endereco.TipoEndereco.val);
        _listaEndereco.TipoLogradouro.val(endereco.TipoLogradouro.val);
        _listaEndereco.IE.val(endereco.IE.val);
        _listaEndereco.TipoAreaEnderecoSecundario.val(endereco.TipoAreaEnderecoSecundario.val);
        _listaEndereco.AreaSecundario.val(endereco.AreaSecundario.val);
        _listaEndereco.RaioEmMetrosSecundario.val(endereco.RaioEmMetrosSecundario.val)
        _listaEndereco.Telefone.val(endereco.Telefone.val)

        changePositionMarkerEndereco();
        setarTipoAreaEnderecoSecundario();
        setarRaioEmMetrosSecundario();
        SetarAreaGeoLocalizacaoEnderecoSecundario();
        BuscarLatitudeLongitudeEndereco();

        _listaEndereco.Adicionar.visible(false);
        _listaEndereco.Cancelar.visible(true);
        _listaEndereco.Excluir.visible(true);
        _listaEndereco.Atualizar.visible(true);
    }
}

function atualizarListaEnderecoClick(data) {
    var valido = ValidaEndereco();

    if (valido) {
        var endereco = ObjetoEndereco();
        var codigo = _listaEndereco.Codigo.val();

        $.each(_pessoa.ListaEndereco.list, function (i, listaEndereco) {
            if (codigo == listaEndereco.Codigo.val) {
                _pessoa.ListaEndereco.list[i] = endereco;
                return false;
            }
        });

        limparCamposListaEndereco();
        recarregarGridListaEndereco();
    } else {
        ExibeErroEndereco();
    }
}

function adicionarListaEnderecoClick(e, sender) {

    var valido = ValidaEndereco();

    if (valido) {
        _listaEndereco.Codigo.val(guid());

        var endereco = ObjetoEndereco();
        _pessoa.ListaEndereco.list.push(endereco);

        recarregarGridListaEndereco();
        limparCamposListaEndereco();
    } else {
        ExibeErroEndereco();
    }
}

function limparCamposListaEndereco() {
    LimparCampos(_listaEndereco);
    _listaEndereco.CEP.get$().val("");

    $("#" + _listaEndereco.EnderecoDigitado.id).prop("checked", false);
    $("#" + _listaEndereco.SN_Numero.id).prop("checked", false);

    _listaEndereco.Numero.val("S/N");
    _listaEndereco.Numero.enable(false);
    _listaEndereco.EnderecoDigitado.val(false);
    desabilitaCamposEnderecoEnderecoSecundario();

    _listaEndereco.Endereco.requiredClass("form-control");
    _listaEndereco.Numero.requiredClass("form-control");
    _listaEndereco.Bairro.requiredClass("form-control");
    _listaEndereco.CEP.requiredClass("form-control");
    _listaEndereco.Localidade.requiredClass("form-control");

    _listaEndereco.Adicionar.visible(true);
    _listaEndereco.Cancelar.visible(false);
    _listaEndereco.Excluir.visible(false);
    _listaEndereco.Atualizar.visible(false);

    if (_mapaListaEndereco)
        _mapaListaEndereco.clear();
}

function abrirModaConsultarCEPEnderecoSecundarioClick(e, sender) {
    Global.abrirModal('divModalConsultaEndereco');
}

function consultaEnderecoCEPEnderecoSecundario(e) {
    if ($("#" + _listaEndereco.CEP.id).val().match(/\d/g) != null && $("#" + _listaEndereco.CEP.id).val().match(/\d/g).join("").length == 8) {
        var data = { CEP: $("#" + _listaEndereco.CEP.id).val() };
        executarReST("Localidade/BuscarEnderecoPorCEP", data, function (arg) {
            if (arg.Success) {
                if (arg.Data != null && arg.Data.DescricaoCidadeEstado != null && arg.Data.DescricaoCidadeEstado != "" && arg.Data.CodigoCidade > 0) {
                    _listaEndereco.Bairro.val(arg.Data.Bairro);
                    _listaEndereco.Endereco.val(arg.Data.Logradouro);
                    _listaEndereco.Localidade.codEntity(arg.Data.CodigoCidade);
                    _listaEndereco.Localidade.val(arg.Data.DescricaoCidadeEstado);
                    _listaEndereco.Latitude.val(arg.Data.Latitude);
                    _listaEndereco.Longitude.val(arg.Data.Longitude);
                    if (arg.Data.TipoLogradouro != null && arg.Data.TipoLogradouro != "") {
                        if (arg.Data.TipoLogradouro == "Rua")
                            _listaEndereco.TipoLogradouro.val(1);
                        else if (arg.Data.TipoLogradouro == "Avenida")
                            _listaEndereco.TipoLogradouro.val(2);
                        else if (arg.Data.TipoLogradouro == "Rodovia")
                            _listaEndereco.TipoLogradouro.val(3);
                        else if (arg.Data.TipoLogradouro == "Estrada")
                            _listaEndereco.TipoLogradouro.val(4);
                        else if (arg.Data.TipoLogradouro == "Praca")
                            _listaEndereco.TipoLogradouro.val(5);
                        else if (arg.Data.TipoLogradouro == "Praça")
                            _listaEndereco.TipoLogradouro.val(5);
                        else if (arg.Data.TipoLogradouro == "Travessa")
                            _listaEndereco.TipoLogradouro.val(6);
                        else
                            _listaEndereco.TipoLogradouro.val(99);
                    }
                    $("#" + _listaEndereco.Complemento.id).focus();
                    verificaEnderecoUnico(arg.Data.DescricaoCidade);
                } else if (_listaEndereco.EnderecoDigitado.val() == false) {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Pessoas.Pessoa.ConsultaDeCEP, Localization.Resources.Pessoas.Pessoa.CEPInformadoNaoExisteNaBaseDeDados);
                    _listaEndereco.Bairro.val("");
                    _listaEndereco.Endereco.val("");
                    LimparCampoEntity(_listaEndereco.Localidade);
                    _listaEndereco.EnderecoDigitado.val(true);
                    habilitaCamposEnderecoEnderecoSecundario();
                } else if (_listaEndereco.EnderecoDigitado.val() == true) {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Pessoas.Pessoa.ConsultaDeCEP, Localization.Resources.Pessoas.Pessoa.CEPInformadoNaoExisteNaBaseDeDados);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    }
}

function verificarSNNumeroEnderecoSecundario(e, sender) {
    if (_listaEndereco.SN_Numero.val() == false) {
        _listaEndereco.Numero.val("S/N");
        _listaEndereco.Numero.enable(false);
    } else {
        if (_listaEndereco.Numero.val("S/N"))
            _listaEndereco.Numero.val("");
        _listaEndereco.Numero.enable(true);
    }
}

function digitarEnderecoEnderecoSecundario(e, sender) {
    verificaDigitarEnderecoEnderecoSecundario();
}

function selecionarEnderecoClickEnderecoSecundario(enderecoSelecionado) {
    if (enderecoSelecionado != null) {
        Global.fecharModal('divModalConsultaEndereco');

        _listaEndereco.CEP.val(enderecoSelecionado.CEP);
        _listaEndereco.Bairro.val(enderecoSelecionado.Bairro);
        _listaEndereco.Endereco.val(enderecoSelecionado.Logradouro);
        _listaEndereco.Localidade.codEntity(enderecoSelecionado.CodigoCidade);
        _listaEndereco.Localidade.val(enderecoSelecionado.Descricao);
        _listaEndereco.Latitude.val(enderecoSelecionado.Latitude);
        _listaEndereco.Longitude.val(enderecoSelecionado.Longitude);
        if (enderecoSelecionado.TipoLogradouro != null && enderecoSelecionado.TipoLogradouro != "") {
            if (enderecoSelecionado.TipoLogradouro == "Rua")
                _listaEndereco.TipoLogradouro.val(1);
            else if (enderecoSelecionado.TipoLogradouro == "Avenida")
                _listaEndereco.TipoLogradouro.val(2);
            else if (enderecoSelecionado.TipoLogradouro == "Rodovia")
                _listaEndereco.TipoLogradouro.val(3);
            else if (enderecoSelecionado.TipoLogradouro == "Estrada")
                _listaEndereco.TipoLogradouro.val(4);
            else if (enderecoSelecionado.TipoLogradouro == "Praca")
                _listaEndereco.TipoLogradouro.val(5);
            else if (enderecoSelecionado.TipoLogradouro == "Praça")
                _listaEndereco.TipoLogradouro.val(5);
            else if (enderecoSelecionado.TipoLogradouro == "Travessa")
                _listaEndereco.TipoLogradouro.val(6);
            else
                _listaEndereco.TipoLogradouro.val(99);
        }
        _listaEndereco.EnderecoDigitado.val(false);
        desabilitaCamposEnderecoEnderecoSecundario();
        changePositionMarkerEndereco();

        verificaEnderecoUnicoEnderecoSecundario(enderecoSelecionado.DescricaoCidade);

        $("#" + _listaEndereco.Complemento.id).focus();
    }
}

function verificaEnderecoUnicoEnderecoSecundario(nomeCidade) {
    if (_listaEndereco.Endereco.val() == null || _listaEndereco.Endereco.val() == "" || removeAcento(_listaEndereco.Endereco.val().toUpperCase()) == removeAcento(nomeCidade.toUpperCase())) {
        _listaEndereco.Bairro.enable(true);
        _listaEndereco.Endereco.enable(true);
        $("#" + _listaEndereco.Endereco.id).focus();
    } else {
        _listaEndereco.Bairro.enable(false);
        _listaEndereco.Endereco.enable(false);
    }
}

function pesquisarEnderecosEnderecoSecundarioClick(e, sender) {
    var data = {
        Logradouro: _pesquisaEnderecoSecundario.Logradouro.val(),
        CEP: _pesquisaEnderecoSecundario.CEP.val(),
        Bairro: _pesquisaEnderecoSecundario.Bairro.val(),
        Descricao: _pesquisaEnderecoSecundario.Localidade.val(),
        CodigoIBGE: _pesquisaEnderecoSecundario.CodigoIBGE.val(),
        CodigoCidade: _pesquisaEnderecoSecundario.Localidade.codEntity(),
        NomeCidade: _pesquisaEnderecoSecundario.Localidade.val()
    };
    var realizaConsulta = false;

    realizaConsulta = data.Logradouro != "" || data.CEP != "" || data.Bairro != "" || data.Descricao != "" || data.CodigoIBGE != "" || data.CodigoCidade != "" || data.NomeCidade != "";

    if (realizaConsulta) {
        executarReST("Localidade/BuscarEnderecosCorreio", data, function (arg) {
            if (arg.Success) {
                $.each(arg.Data, function (i, endereco) {

                    var selecionar = { descricao: Localization.Resources.Gerais.Geral.Selecionar, id: guid(), evento: "onclick", metodo: selecionarEnderecoClickEnderecoSecundario, tamanho: "15", icone: "" };
                    var menuOpcoes = new Object();
                    menuOpcoes.tipo = TypeOptionMenu.link;
                    menuOpcoes.opcoes = new Array();
                    menuOpcoes.opcoes.push(selecionar);

                    var header = [
                        { data: "Descricao", title: Localization.Resources.Pessoas.Pessoa.Cidade, width: "20%", className: "text-align-left" },
                        { data: "Logradouro", title: Localization.Pessoas.Pessoa.Endereco, width: "20%", className: "text-align-left" },
                        { data: "Bairro", title: Localization.Resources.Pessoas.Pessoa.Bairro, width: "15%" },
                        { data: "CEP", title: Localization.Resources.Pessoas.Pessoa.CEP, width: "10%", className: "text-align-right" },
                        { data: "CodigoIBGE", title: Localization.Resources.Pessoas.Pessoa.CodigoIBGE, width: "10%", className: "text-align-right" },
                        { data: "TipoLogradouro", title: Localization.Resources.Pessoas.Pessoa.Tipo, width: "10%", className: "text-align-left" },
                        { data: "CodigoCidade", visible: false },
                        { data: "Latitude", visible: false },
                        { data: "Longitude", visible: false },
                        { data: "DescricaoCidade", visible: false }

                    ];
                    var gridEnderecos = new BasicDataTable(_pesquisaEnderecoSecundario.Enderecos.idGrid, header, menuOpcoes);
                    gridEnderecos.CarregarGrid(endereco);
                });
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    } else
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Pessoas.Pessoa.PorFavorInformeAoMenosUmDosCamposParaRealizarConsulta);
}

function carregarConteudosHTMLEnderecoSecundario(callback) {
    $.get("Content/Static/Localidade/Localidade.html?dyn=" + guid(), function (data) {
        $("#ConsultaEnderecoSecundario").html(data);
        _pesquisaEnderecoSecundario = new PesquisaEnderecoEnderecoSecundario();
        KoBindings(_pesquisaEnderecoSecundario, "knoutConsultaEndereco");
        new BuscarLocalidadesBrasil(_pesquisaEnderecoSecundario.Localidade);
        $("#" + _pesquisaEnderecoSecundario.CEP.id).mask("00.000-000", { selectOnFocus: true, clearIfNotMatch: true });
    });
}

function verificaDigitarEnderecoEnderecoSecundario() {
    if (_listaEndereco.EnderecoDigitado.val() == true) {
        habilitaCamposEnderecoEnderecoSecundario();
    } else {
        desabilitaCamposEnderecoEnderecoSecundario();
    }
}

function habilitaCamposEnderecoEnderecoSecundario() {
    _listaEndereco.Bairro.enable(true);
    _listaEndereco.Endereco.enable(true);
    _listaEndereco.Localidade.enable(true);
}

function desabilitaCamposEnderecoEnderecoSecundario() {
    _listaEndereco.Bairro.enable(false);
    _listaEndereco.Endereco.enable(false);
    _listaEndereco.Localidade.enable(false);
}
function ValidaEndereco() {
    var valido = true;
    valido = _listaEndereco.Endereco.val() != "";
    valido = _listaEndereco.Numero.val() != "" && valido;
    valido = _listaEndereco.Bairro.val() != "" && valido;
    valido = $("#" + _listaEndereco.CEP.id).val().match(/\d/g) != null && $("#" + _listaEndereco.CEP.id).val().match(/\d/g).join("").length == 8 && valido;
    valido = _listaEndereco.Localidade.val() != "" && valido;

    _listaEndereco.Endereco.requiredClass("form-control");
    _listaEndereco.Numero.requiredClass("form-control");
    _listaEndereco.Bairro.requiredClass("form-control");
    _listaEndereco.CEP.requiredClass("form-control");
    _listaEndereco.Localidade.requiredClass("form-control");

    return valido;
}

function ExibeErroEndereco() {
    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    if (_listaEndereco.Endereco.val() == "")
        _listaEndereco.Endereco.requiredClass("form-control is-invalid");

    if (_listaEndereco.Numero.val() == "")
        _listaEndereco.Numero.requiredClass("form-control is-invalid");

    if (_listaEndereco.Bairro.val() == "")
        _listaEndereco.Bairro.requiredClass("form-control is-invalid");

    if ($("#" + _listaEndereco.CEP.id).val().match(/\d/g) == null || $("#" + _listaEndereco.CEP.id).val().match(/\d/g).join("").length < 8)
        _listaEndereco.CEP.requiredClass("form-control is-invalid");

    if (_listaEndereco.Localidade.val() == "")
        _listaEndereco.Localidade.requiredClass("form-control is-invalid");
}

function ObjetoEndereco() {
    _listaEndereco.CodigoLocalidade.val(_listaEndereco.Localidade.codEntity());
    _listaEndereco.DescricaoLocalidade.val(_listaEndereco.Localidade.val());
    _listaEndereco.AreaSecundario.val(obterJsonPoligonoGeoLocalizacaoSecondario());

    var endereco = SalvarListEntity(_listaEndereco);

    delete endereco.ConsultarCEP;
    delete endereco.Localidade;

    return endereco;
}

function abaPessoaEnderecoGeoLocalizacaoClick() {
    var opcoes = new OpcoesMapa(false, false);

    if (_mapaListaEndereco == null && _mapDrawSecundario == null) {
        _mapaListaEndereco = new MapaGoogle("mapaListaEndereco", false, opcoes);
        _mapDrawSecundario = _mapaListaEndereco.draw;
    }

    var info = { latitude: _listaEndereco.Latitude.val(), longitude: _listaEndereco.Longitude.val() };
    criarMarkerEndereco(info);

    setarTipoAreaEnderecoSecundario();
    setarRaioEmMetrosSecundario();
    SetarAreaGeoLocalizacaoEnderecoSecundario();
}

var _newShape;

function criarMarkerEndereco(info) {

    if (_mapaListaEndereco)
        _mapaListaEndereco.clear();

    if (_newShape)
        google.maps.event.clearInstanceListeners(_newShape);

    if ((info.latitude) && (info.longitude) && (_mapaListaEndereco)) {

        var marker = new ShapeMarker();

        marker.setPosition(info.latitude, info.longitude);

        marker.title = '<div>' + ' (' + info.latitude + ',' + info.longitude + ')' + '<div>';

        _mapaListaEndereco.draw.setShapeDraggable(true);

        _newShape = _mapaListaEndereco.draw.addShape(marker);
        _mapaListaEndereco.direction.setZoom(17);
        _mapaListaEndereco.direction.centralizar(info.latitude, info.longitude);

        _newShape.addListener("dragend", dragendEventEndereco)
    }
}


function BuscarCoordenadasListaEnderecoClick() {
    dadosEndereco = new DadosEndereco(_listaEndereco.Endereco.val(), _listaEndereco.Numero.val(), _listaEndereco.Localidade.val(), "", _listaEndereco.CEP.val(), _listaEndereco.Bairro.val(), _listaEndereco.Complemento.val(), _listaEndereco.TipoLogradouro.val());
    
    _mapaListaEndereco.geo.buscarCoordenadas(dadosEndereco, function (resposta) {

        if (resposta.status === "OK") {
            _listaEndereco.Latitude.val(resposta.latitude);
            _listaEndereco.Longitude.val(resposta.longitude);
        } else if (resposta.status === "NotFound") {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Pessoas.Pessoa.Atencao, Localization.Resources.Pessoas.Pessoa.GeolocalizacaoNotFound);
        } else if (resposta.status === "ErroNominatim") {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Pessoas.Pessoa.Atencao, Localization.Resources.Pessoas.Pessoa.GeolocalizacaoErroNominatim);
        }

        changePositionMarkerEndereco();
    });
}

function changePositionMarkerEndereco() {
    var info = { latitude: _listaEndereco.Latitude.val(), longitude: _listaEndereco.Longitude.val() };
    criarMarkerEndereco(info);
}

function dragendEventEndereco(event) {
    var latLng = event.latLng;
    //var latLng = _marker.getPosition();
    _listaEndereco.Latitude.val(latLng.lat().toFixed(6).toString());
    _listaEndereco.Longitude.val(latLng.lng().toFixed(6).toString());
}

function BuscarLatitudeLongitudeEndereco() {
    var lat = parseFloat(String(_listaEndereco.Latitude.val()).replace(',', '.'));
    var long = parseFloat(String(_listaEndereco.Longitude.val()).replace(',', '.'));
    if (!isNaN(lat) != 0 && !isNaN(long) != 0) {
        _listaEndereco.Latitude.val(lat);
        _listaEndereco.Longitude.val(long);
        changePositionMarkerEndereco();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Pessoas.Pessoa.Atencao, Localization.Resources.Pessoas.Pessoa.LatitudeOuLongitudeInformadaEstaInvalida);
    }
}