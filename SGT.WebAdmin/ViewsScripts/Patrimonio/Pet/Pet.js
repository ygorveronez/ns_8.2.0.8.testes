/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Especie.js" />
/// <reference path="../../Consultas/EspecieRaca.js" />
/// <reference path="../../Consultas/CorAnimal.js" />
/// <reference path="../../Consultas/PlanoServico.js" />
/// <reference path="Anexo.js" />
//*******MAPEAMENTO KNOUCKOUT*******

var _formulario, _pesquisaPet, _gridPesquisa;

var PesquisaPet = function () {
    this.Tutor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), text: Localization.Resources.Patrimonio.Pet.Tutor.getFieldDescription()});
    this.Nome = PropertyEntity({ text: Localization.Resources.Patrimonio.Pet.Nome.getFieldDescription()});
    this.Especie = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), text: Localization.Resources.Configuracoes.Especie.DescricaoEspecie.getFieldDescription() });
    this.Raca = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), text: Localization.Resources.Configuracoes.EspecieRaca.Raca.getFieldDescription() });
    this.Ativo = PropertyEntity({ val: ko.observable(_statusPesquisa.Todos), options: _statusPesquisa, def: _statusPesquisa.Todos, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription() });
    this.Sexo = PropertyEntity({ val: ko.observable(EnumPetSexo.Todos), options: EnumPetSexo.obterOpcoesPesquisa(), def: EnumPetSexo.Todos, text: Localization.Resources.Patrimonio.Pet.Sexo.getFieldDescription() });
    this.Porte = PropertyEntity({ val: ko.observable(EnumPorte.Todos), options: EnumPorte.obterOpcoesPesquisa(), def: EnumPorte.Todos, text: Localization.Resources.Patrimonio.Pet.Porte.getFieldDescription() });
    this.Pelagem = PropertyEntity({ val: ko.observable(EnumPelagem.Todas), options: EnumPelagem.obterOpcoesPesquisa(), def: EnumPelagem.Todas, text: Localization.Resources.Patrimonio.Pet.Pelagem.getFieldDescription() });
    this.Cor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), text: Localization.Resources.Patrimonio.Pet.Cor.getFieldDescription() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridPesquisa.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade()) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var Formulario = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Tutor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), text: "*" + Localization.Resources.Patrimonio.Pet.Tutor.getFieldDescription(), required: true });
    this.Nome = PropertyEntity({ text: "*" + Localization.Resources.Patrimonio.Pet.Nome.getFieldDescription(), required: true });
    this.Especie = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), text: "*" + Localization.Resources.Configuracoes.Especie.DescricaoEspecie.getFieldDescription(), required: true });
    this.Raca = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), text: "*" + Localization.Resources.Configuracoes.EspecieRaca.Raca.getFieldDescription(), required: true });
    this.Ativo = PropertyEntity({ val: ko.observable(_status.Ativo), options: _status, def: true, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription() });
    this.Porte = PropertyEntity({ val: ko.observable(EnumPorte.Pequeno), options: EnumPorte.obterOpcoes(), def: EnumPorte.Pequeno, text: "*" + Localization.Resources.Patrimonio.Pet.Porte.getFieldDescription(), required: true });
    this.Sexo = PropertyEntity({ val: ko.observable(EnumPetSexo.NaoInformado), options: EnumPetSexo.obterOpcoes(), def: EnumPetSexo.NaoInformado, text: "*" + Localization.Resources.Patrimonio.Pet.Sexo.getFieldDescription(), required: true });
    this.Pelagem = PropertyEntity({ val: ko.observable(EnumPelagem.Curta), options: EnumPelagem.obterOpcoes(), def: EnumPelagem.Curta, text: Localization.Resources.Patrimonio.Pet.Pelagem.getFieldDescription() });
    this.Cor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), text: Localization.Resources.Patrimonio.Pet.Cor.getFieldDescription() });
    this.Peso = PropertyEntity({ maxlength: 12, getType: typesKnockout.decimal, text: Localization.Resources.Patrimonio.Pet.Peso.getFieldDescription() });
    this.DataNascimento = PropertyEntity({ val: ko.observable(""), def: ko.observable(""), getType: typesKnockout.date, text: Localization.Resources.Consultas.Cliente.DataNascimento.getFieldDescription() });
    this.Comportamento = PropertyEntity({ val: ko.observable(EnumComportamento.Docil), options: EnumComportamento.obterOpcoes(), def: EnumComportamento.Docil, text: Localization.Resources.Patrimonio.Pet.Comportamento.getFieldDescription() });
    this.PlanoServico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), val: ko.observable(""), def: "", text: Localization.Resources.Patrimonio.Pet.PlanoServico.getFieldDescription() });
    this.Castrado = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Patrimonio.Pet.Castrado.getFieldDescription() });
    this.Microchip = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: Localization.Resources.Patrimonio.Pet.Microchip.getFieldDescription() });
    this.UltimaVisita = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.date, text: Localization.Resources.Patrimonio.Pet.UltimaVisita.getFieldDescription() });
    this.Observacao = PropertyEntity({ text: Localization.Resources.Patrimonio.Pet.Observacao.getFieldDescription() });

    //Foto
    this.AdicionarFoto = PropertyEntity({ type: types.file, codEntity: ko.observable(0), val: ko.observable(""), enable: ko.observable(true), text: ko.observable(Localization.Resources.Patrimonio.Pet.Selecionar.getFieldDescription().replace(':', '')), visible: ko.observable(true) });
    this.RemoverFoto = PropertyEntity({ eventClick: removerPreVisualizacao, type: types.event, text: ko.observable(Localization.Resources.Patrimonio.Pet.Remover.getFieldDescription().replace(':', '')), visible: ko.observable(true) });
    this.FotoPet = PropertyEntity({});

    this.AdicionarFoto.val.subscribe(function (nomeArquivoFotoSelecionado) {
        if (nomeArquivoFotoSelecionado)
            preVisualizarFoto();
    });

    //CRUD
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Limpar/Cancelar", visible: ko.observable(true) });

};

function loadPet() {
    _formulario = new Formulario();
    KoBindings(_formulario, "knockoutCadastro");

    HeaderAuditoria("Pet", _formulario);

    _pesquisaPet = new PesquisaPet();
    KoBindings(_pesquisaPet, "knockoutPesquisaPet", false, _pesquisaPet.Pesquisar.id);

    buscarPet();

    new BuscarClientes(_formulario.Tutor);
    new BuscarEspecie(_formulario.Especie, retornoEspecieSelecionada);
    new BuscarRaca(_formulario.Raca, retornoRacaSelecionada, _formulario.Especie);
    new BuscarCorAnimal(_formulario.Cor);
    new BuscarPlanoServico(_formulario.PlanoServico);

    new BuscarClientes(_pesquisaPet.Tutor);
    new BuscarEspecie(_pesquisaPet.Especie, retornoEspeciePesquisaSelecionada);
    new BuscarRaca(_pesquisaPet.Raca, retornoRacaPesquisaSelecionada, _pesquisaPet.Especie);
    new BuscarCorAnimal(_pesquisaPet.Cor); 

    loadAnexos();
}

function buscarPet() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarPet, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridPesquisa = new GridView(_pesquisaPet.Pesquisar.idGrid, "Pet/Pesquisar", _pesquisaPet, menuOpcoes, null);
    _gridPesquisa.CarregarGrid();
}

function editarPet(arquivoGrid) {
    limparCamposPet();
    _formulario.Codigo.val(arquivoGrid.Codigo);

    BuscarPorCodigo(_formulario, "Pet/BuscarPorCodigo", function (arg) {
        _pesquisaPet.ExibirFiltros.visibleFade(false);
        _formulario.Atualizar.visible(true);
        _formulario.Cancelar.visible(true);
        _formulario.Excluir.visible(true);
        _formulario.Adicionar.visible(false);

        EditarListarAnexos(arg);

    }, null);
}

function adicionarClick(e, sender) {

    Salvar(_formulario, "Pet/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _formulario.Codigo.val(arg.Data)
                salvarImagem();
                EnviarArquivosAnexados();
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Pet cadastrado com sucesso");
                _gridPesquisa.CarregarGrid();
                limparCamposPet();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_formulario, "Pet/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (_formulario.FotoPet.val() == '')
                    removerImagem();
                else
                    salvarImagem();
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridPesquisa.CarregarGrid();
                limparCamposPet();
                limparAnexosTela();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o pet " + _formulario.Nome.val() + "?", function () {

        removerImagem();
        ExcluirPorCodigo(_formulario, "Pet/Excluir", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridPesquisa.CarregarGrid();
                    limparCamposPet();
                    limparAnexosTela();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposPet();
    limparAnexosTela();
}

function limparCamposPet() {
    _formulario.Atualizar.visible(false);
    _formulario.Cancelar.visible(false);
    _formulario.Excluir.visible(false);
    _formulario.Adicionar.visible(true);

    _formulario.FotoPet.val('');
    _formulario.AdicionarFoto.val('');

    LimparCampos(_formulario);
}


//#region Callbacks

function retornoEspecieSelecionada(data) {
    if (!data)
        return;
   
    if (data.Codigo > 0) {
        _formulario.Especie.codEntity(data.Codigo)
        _formulario.Especie.val(data.Descricao)
    }

    _formulario.Raca.codEntity(0)
    _formulario.Raca.val('')

    if (data.CodigoRaca > 0) {
        _formulario.Raca.codEntity(data.CodigoRaca)
        _formulario.Raca.val(data.DescricaoRaca)
    }    
}

function retornoRacaSelecionada(data) {

    if (!data)
        return;    

    if (data.Codigo > 0) {
        _formulario.Raca.codEntity(data.Codigo)
        _formulario.Raca.val(data.Descricao)
    }

    if (data.CodigoEspecie > 0) {
        _formulario.Especie.codEntity(data.CodigoEspecie)
        _formulario.Especie.val(data.DescricaoEspecie)
    }
}

function retornoEspeciePesquisaSelecionada(data) {

    if (!data)
        return;

    _formulario.Raca.codEntity(0)
    _formulario.Raca.val('')

    if (data.Codigo > 0) {
        _pesquisaPet.Especie.codEntity(data.Codigo)
        _pesquisaPet.Especie.val(data.Descricao)
    }

    if (data.CodigoRaca > 0) {
        _pesquisaPet.Raca.codEntity(data.CodigoRaca)
        _pesquisaPet.Raca.val(data.DescricaoRaca)
    }
}

function retornoRacaPesquisaSelecionada(data) {

    if (!data)
        return;

    if (data.Codigo > 0) {
        _pesquisaPet.Raca.codEntity(data.Codigo)
        _pesquisaPet.Raca.val(data.Descricao)
    }

    if (data.CodigoEspecie > 0) {
        _pesquisaPet.Especie.codEntity(data.CodigoEspecie)
        _pesquisaPet.Especie.val(data.DescricaoEspecie)
    }
}

//#endregion

//#region Foto

function preVisualizarFoto() {
    // Obtenho o elemento da imagem
    var elemento = document.getElementById(_formulario.AdicionarFoto.id);

    // Obtenho o arquivo da imagem a partir do elemento
    var arquivo = elemento.files[0];

    // Instancio um FileReader
    var reader = new FileReader();

    // Declaro um evento ao terminar de ler o arquivo
    reader.onloadend = function () {
        _formulario.FotoPet.val(reader.result)
    }

    // Faço o file reader ler o arquivo da imagem
    reader.readAsDataURL(arquivo);
}

function removerPreVisualizacao() {
    _formulario.FotoPet.val('');
    _formulario.AdicionarFoto.val('');
}

function obterFormDataFotoPet() {
    var arquivo = document.getElementById(_formulario.AdicionarFoto.id);

    if (arquivo.files.length > 0) {
        var formData = new FormData();

        formData.append("ArquivoFoto", arquivo.files[0]);

        return formData;
    }

    return undefined;
}

function salvarImagem() {
    var formData = obterFormDataFotoPet();

    if (!formData)
        return;
    enviarArquivo("Pet/AdicionarFoto?callback=?", { Codigo: _formulario.Codigo.val() }, formData, function (retorno) {
        if (retorno.Success) {
            if (!retorno.Data)
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function removerImagem() {

    executarReST("Pet/RemoverFoto", { Codigo: _formulario.Codigo.val() }, function (retorno) {
        if (!retorno.Success)
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

//#endregion