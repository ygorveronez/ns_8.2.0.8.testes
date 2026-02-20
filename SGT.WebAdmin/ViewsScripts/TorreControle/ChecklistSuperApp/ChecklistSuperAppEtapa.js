/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />

/*
* Declaração de Objetos Globais do Arquivo
*/
var _gridEtapasChecklist;

/*
 * Declaração das Funções de Inicialização
 */
function loadGridEtapasChecklist(etapas) {
    var linhasPorPaginas = 5;
    var opcaoEditar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarEtapaChecklist, icone: "" };
    var opcaoRemover = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: removerEtapaChecklist, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 21, opcoes: [opcaoEditar, opcaoRemover] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Tipo", visible: false },
        { data: "TipoDescricao", title: "Tipo", width: "20%", className: "text-align-left" },
        { data: "Titulo", title: "Título", width: "20%", className: "text-align-left" },
        { data: "Descricao", title: "Descrição", width: "30%", className: "text-align-left" },
        { data: "Ordem", title: "Ordem", width: "10%", className: "text-align-left" },
        { data: "Obrigatorio", visible: false },
        { data: "ObrigatorioDescricao", title: "Obrigatório", width: "20%", className: "text-align-left" },
        { data: "IdSuperApp", title: "Id Super App", width: "20%", className: "text-align-left" },
        { data: "Configuracoes", visible: false},
        { data: "TipoEvidencia", visible: false},
    ];
    _gridEtapasChecklist = new BasicDataTable(_checklistSuperApp.GridEtapasChecklist.id, header, menuOpcoes, [{ column: 5, dir: orderDir.asc }], null, linhasPorPaginas);
    _gridEtapasChecklist.CarregarGrid(etapas);
}

/*
 * Declaração das Funções Públicas
 */
function limparCamposCadastroEtapaChecklistClick() {
    limparCamposCadastroEtapaChecklist();
}
function obterListaEtapasChecklistSalvar(checklistSuperApp) {
    let etapas = obterListaEtapasChecklist();
    for (let i = 0; i < etapas.length; i++) {
        if (typeof etapas[i].Configuracoes !== "string") etapas[i].Configuracoes = JSON.stringify(etapas[i].Configuracoes);
    };
    checklistSuperApp["ListaEtapasChecklist"] = JSON.stringify(etapas);
}

/*
 * Declaração das Funções
 */
function obterListaEtapasChecklist() {
    return _gridEtapasChecklist.BuscarRegistros();
}

function removerEtapaChecklist(registroSelecionado) {
    var listaEtapaChecklist = obterListaEtapasChecklist();

    for (var i = 0; i < listaEtapaChecklist.length; i++) {
        if (registroSelecionado.Codigo == listaEtapaChecklist[i].Codigo) {
            listaEtapaChecklist.splice(i, 1);
            break;
        }
    }
    _gridEtapasChecklist.CarregarGrid(listaEtapaChecklist);
}
function editarEtapaChecklist(etapa) {
    var listaEtapaChecklist = obterListaEtapasChecklist();
    var cadastroEtapaChecklist;

    $.each(listaEtapaChecklist, function (i, item) {
        if (etapa.Codigo == item.Codigo) {
            cadastroEtapaChecklist = item;
            return false;
        }
    });
    if (cadastroEtapaChecklist != null) {
        _checklistSuperApp.CodigoEtapaChecklist.val(cadastroEtapaChecklist.Codigo);
        _checklistSuperApp.TipoEtapaChecklist.val(cadastroEtapaChecklist.Tipo);
        _checklistSuperApp.TituloEtapaChecklist.val(cadastroEtapaChecklist.Titulo);
        _checklistSuperApp.DescricaoEtapaChecklist.val(cadastroEtapaChecklist.Descricao);
        _checklistSuperApp.OrdemEtapaChecklist.val(cadastroEtapaChecklist.Ordem);
        _checklistSuperApp.ObrigatorioEtapaChecklist.val(cadastroEtapaChecklist.Obrigatorio);
        _checklistSuperApp.TipoEvidencia.val(cadastroEtapaChecklist.TipoEvidencia);

        const configuracoes = typeof cadastroEtapaChecklist.Configuracoes === "string" ? JSON.parse(cadastroEtapaChecklist.Configuracoes) : cadastroEtapaChecklist.Configuracoes;

        for (const key in configuracoes) {
            if (configuracoes.hasOwnProperty(key)) {
                _checklistSuperApp[key].val(configuracoes[key]);
            }
        }
        
        _checklistSuperApp.AdicionarCadastroEtapaChecklist.visible(false);
        _checklistSuperApp.AtualizarCadastroEtapaChecklist.visible(true);
        _checklistSuperApp.CancelarCadastroEtapaChecklist.visible(true);
    }
}
function adicionarListaCadastroEtapaChecklistClick(dados) {
    var etapa = obterObjetoEtapaChecklist(dados);
    var listaEtapaChecklist = obterListaEtapasChecklist();
    if (validarEtapa(etapa) && validarSequencia(listaEtapaChecklist, etapa, false)) {
        listaEtapaChecklist.push(etapa);
        _gridEtapasChecklist.CarregarGrid(listaEtapaChecklist);
        limparCamposCadastroEtapaChecklist();
    }
}
function atualizarListaCadastroEtapaChecklistClick(dados) {
    var etapa = obterObjetoEtapaChecklist(dados);
    var listaEtapaChecklist = obterListaEtapasChecklist();
    if (validarEtapa(etapa) && validarSequencia(listaEtapaChecklist, etapa, true)) {
        for (var i = 0; i < listaEtapaChecklist.length; i++) {
            if (etapa.Codigo == listaEtapaChecklist[i].Codigo) {
                listaEtapaChecklist[i] = etapa;
                break;
            }
        }
        _gridEtapasChecklist.CarregarGrid(listaEtapaChecklist);
        limparCamposCadastroEtapaChecklist();
    }
}

function obterObjetoEtapaChecklist(dados) {
    var codigo = dados.CodigoEtapaChecklist.val() ? dados.CodigoEtapaChecklist.val() : guid();

    const copiaObjeto = { ...dados, Codigo: undefined, GridEtapasChecklist: undefined };

    const removerDasConfiguracoes = ["Titulo", "TipoFluxo", "IdSuperApp", "CodigoEtapaChecklist"];

    const configuracoes = {}
    for (const key in copiaObjeto) {
        if (copiaObjeto.hasOwnProperty(key) && !removerDasConfiguracoes.includes(key)) {
            configuracoes[key] = copiaObjeto[key]?.val() ? copiaObjeto[key].val() : undefined;
        }
    }

    return {
        ...configuracoes,
        Codigo: codigo,
        Tipo: dados.TipoEtapaChecklist.val(),
        TipoDescricao: EnumTipoEtapaChecklistSuperApp.obterDescricao(dados.TipoEtapaChecklist.val()),
        Titulo: dados.TituloEtapaChecklist.val(),
        Descricao: dados.DescricaoEtapaChecklist.val(),
        Obrigatorio: dados.ObrigatorioEtapaChecklist.val(),
        ObrigatorioDescricao: dados.ObrigatorioEtapaChecklist.val() ? 'Sim' : 'Não',
        Ordem: dados.OrdemEtapaChecklist.val(),
        IdSuperApp: null,
        Configuracoes: JSON.stringify(configuracoes),
        TipoEvidencia: dados.TipoEvidencia.val() || null
    };
}

function limparCamposCadastroEtapaChecklist() {
    _checklistSuperApp.CodigoEtapaChecklist.val(_checklistSuperApp.CodigoEtapaChecklist.def());
    _checklistSuperApp.TipoEtapaChecklist.val(_checklistSuperApp.TipoEtapaChecklist.def());
    _checklistSuperApp.TipoEvidencia.val(_checklistSuperApp.TipoEvidencia.def());
    _checklistSuperApp.TituloEtapaChecklist.val(_checklistSuperApp.TituloEtapaChecklist.def());
    _checklistSuperApp.DescricaoEtapaChecklist.val(_checklistSuperApp.DescricaoEtapaChecklist.def());
    _checklistSuperApp.ObrigatorioEtapaChecklist.val(_checklistSuperApp.ObrigatorioEtapaChecklist.def());
    _checklistSuperApp.OrdemEtapaChecklist.val(_checklistSuperApp.OrdemEtapaChecklist.def());
    _checklistSuperApp.HelperTextEtapaChecklist.val(_checklistSuperApp.HelperTextEtapaChecklist.def());
    _checklistSuperApp.TipoAlertaEtapaChecklist.val(_checklistSuperApp.TipoAlertaEtapaChecklist.def());
    _checklistSuperApp.TituloAlertaEtapaChecklist.val(_checklistSuperApp.TituloAlertaEtapaChecklist.def());
    _checklistSuperApp.DescricaoAlertaEtapaChecklist.val(_checklistSuperApp.DescricaoAlertaEtapaChecklist.def());
    _checklistSuperApp.PlaceHolderEtapaChecklist.val(_checklistSuperApp.PlaceHolderEtapaChecklist.def());
    _checklistSuperApp.ValorMinimoEtapaChecklist.val(_checklistSuperApp.ValorMinimoEtapaChecklist.def());
    _checklistSuperApp.ValorMaximoEtapaChecklist.val(_checklistSuperApp.ValorMaximoEtapaChecklist.def());
    _checklistSuperApp.QuantidadeMinimaEtapaChecklist.val(_checklistSuperApp.QuantidadeMinimaEtapaChecklist.def());
    _checklistSuperApp.QuantidadeMaximaEtapaChecklist.val(_checklistSuperApp.QuantidadeMaximaEtapaChecklist.def());
    _checklistSuperApp.GaleriaHabilitadaEtapaChecklist.val(_checklistSuperApp.GaleriaHabilitadaEtapaChecklist.def());
    _checklistSuperApp.PermitirPausarEtapaChecklist.val(_checklistSuperApp.PermitirPausarEtapaChecklist.def());
    _checklistSuperApp.TempoEsperaEtapaChecklist.val(_checklistSuperApp.TempoEsperaEtapaChecklist.def());
    _checklistSuperApp.TipoProcessamentoImagemEtapaChecklist.val(_checklistSuperApp.TipoProcessamentoImagemEtapaChecklist.def());
    _checklistSuperApp.ThresholdEtapaChecklist.val(_checklistSuperApp.ThresholdEtapaChecklist.def());
    _checklistSuperApp.ModoValidacaoImagemEtapaChecklist.val(_checklistSuperApp.ModoValidacaoImagemEtapaChecklist.def());
    _checklistSuperApp.LocalizacaoBloquearAvancoEtapa.val(_checklistSuperApp.LocalizacaoBloquearAvancoEtapa.def());
    _checklistSuperApp.LocalizacaoPodeAvancarForaRaio.val(_checklistSuperApp.LocalizacaoPodeAvancarForaRaio.def());
    _checklistSuperApp.LocalizacaoObrigarImagemComprovacao.val(_checklistSuperApp.LocalizacaoObrigarImagemComprovacao.def());
    _checklistSuperApp.UsarDataAtualComoInicial.val(_checklistSuperApp.UsarDataAtualComoInicial.def());
    _checklistSuperApp.MetadadosImagemMostrarLogo.val(_checklistSuperApp.MetadadosImagemMostrarLogo.def());
    _checklistSuperApp.MetadadosImagemMostrarData.val(_checklistSuperApp.MetadadosImagemMostrarData.def());
    _checklistSuperApp.MetadadosImagemMostrarHora.val(_checklistSuperApp.MetadadosImagemMostrarHora.def());
    _checklistSuperApp.MetadadosImagemMostrarLocalizacao.val(_checklistSuperApp.MetadadosImagemMostrarLocalizacao.def());
    _checklistSuperApp.MetadadosImagemMostrarNomeMotorista.val(_checklistSuperApp.MetadadosImagemMostrarNomeMotorista.def());
    _checklistSuperApp.ValidacaoAdicionalTexto.val(_checklistSuperApp.ValidacaoAdicionalTexto.def());
    _checklistSuperApp.UtilizarNumerosDecimais.val(_checklistSuperApp.UtilizarNumerosDecimais.def());

    _checklistSuperApp.AdicionarCadastroEtapaChecklist.visible(true);
    _checklistSuperApp.AtualizarCadastroEtapaChecklist.visible(false);
    _checklistSuperApp.CancelarCadastroEtapaChecklist.visible(false);
}

function validarEtapaNumber(etapa) {
    if (etapa.MinimoDigitosDecimaisEtapaChecklist < 0) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, "A quantitade mínima de digitos decimais deve ser maior ou igual a 0.");
        return false;
    }
    if (etapa.MaximoDigitosDecimaisEtapaChecklist < 0) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, "A quantitade máxima de digitos decimais deve ser maior ou igual a 0.");
        return false;
    }
    if ((etapa.MinimoDigitosDecimaisEtapaChecklist && !etapa.MaximoDigitosDecimaisEtapaChecklist) || (!etapa.MinimoDigitosDecimaisEtapaChecklist && etapa.MaximoDigitosDecimaisEtapaChecklist)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, "Para definir o número de digitos decimais é necessário informar a quantidade minima e máxima");
        return false;
    }
    if (etapa.MaximoDigitosDecimaisEtapaChecklist < etapa.MinimoDigitosDecimaisEtapaChecklist) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, "A quantitade máxima de digitos decimais deve ser maior que a mínima.");
        return false;
    }
    if (etapa.ValorMaximoEtapaChecklist < etapa.ValorMinimoEtapaChecklist) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, "O valor máximo deve ser maior que o valor mínimo.");
        return false;
    }
    if ((etapa.ValorMinimoEtapaChecklist && !etapa.ValorMaximoEtapaChecklist) || (!etapa.ValorMinimoEtapaChecklist && etapa.ValorMaximoEtapaChecklist)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, "Os valores mínimo e máximo devem ser informados juntos ou não informados.");
        return false;
    }
    return true;
}

function validarEtapaImage(etapa) {
    if (etapa.QuantidadeMinimaEtapaChecklist < 1) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, "A quantitade mínima de imagens deve ser maior ou igual a 1.");
        return false;
    }
    if (etapa.QuantidadeMaximaEtapaChecklist < 1) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, "A quantitade máxima de imagens deve ser maior ou igual a 1.");
        return false;
    }
    if ((etapa.QuantidadeMinimaEtapaChecklist && !etapa.QuantidadeMaximaEtapaChecklist) || (!etapa.QuantidadeMinimaEtapaChecklist && etapa.QuantidadeMaximaEtapaChecklist)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, "Os valores mínimo e máximo devem ser informados juntos ou não informados.");
        return false;
    }
    if (parseInt(etapa.QuantidadeMaximaEtapaChecklist) < parseInt(etapa.QuantidadeMinimaEtapaChecklist)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, "A quantitade máxima de imagens deve ser maior que a mínima.");
        return false;
    }
    return true;
}

function validarEtapaTimer(etapa) {
    if (etapa.TempoEsperaEtapaChecklist < 0) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, "O tempo de espera deve ser no mínimo 0.");
        return false;
    }
    return true;
}

function validarEtapaImageValidator(etapa) {
    if (!validarEtapaImage(etapa)) {
        return false;
    }
    if (!etapa.ModoValidacaoImagemEtapaChecklist) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, "Modo de validação da imagem é obrigatório.");
        return false;
    }
    if (etapa.TipoProcessamentoImagemEtapaChecklist === EnumTipoProcessamentoImagemEtapaChecklistSuperApp.BlackWhite && !etapa.ThresholdEtapaChecklist) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, "É necessário definir um thresold para o tipo de processamento Preto e Branco.");
        return false;
    }
    return true;
}

function validarEtapa(etapa) {
    if (typeof etapa.Tipo != "number") {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, "Tipo da etapa é obrigatório.");
        return false;
    }
    if (!etapa.Titulo) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, "Título da etapa é obrigatório.");
        return false;
    }
    if (!etapa.Descricao) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, "Descrição da etapa é obrigatória.");
        return false;
    }
    if (!etapa.Ordem) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, "Ordem da etapa é obrigatória.");
        return false;
    }

    let validacaoTipoEtapa = true

    switch (etapa.Tipo) {
        case EnumTipoEtapaChecklistSuperApp.Number:
            validacaoTipoEtapa = validarEtapaNumber(etapa);
            break;
        case EnumTipoEtapaChecklistSuperApp.Image:
            validacaoTipoEtapa = validarEtapaImage(etapa);
            break;
        case EnumTipoEtapaChecklistSuperApp.Timer:
            validacaoTipoEtapa = validarEtapaTimer(etapa);
            break;
        case EnumTipoEtapaChecklistSuperApp.ImageValidator:
            validacaoTipoEtapa = validarEtapaImageValidator(etapa);
            break;
    }
    
    return validacaoTipoEtapa;
}

function preencherGridEtapasChecklist(etapas, duplicar) {
    // Caso seja duplicação, deve remover o id do que esta sendo duplicado
    if (duplicar) {
        etapas.forEach(etapa => {
            etapa.Codigo = guid();
        })
    }
    _gridEtapasChecklist.CarregarGrid(etapas);
}

function validarSequencia(listaEtapaChecklist, etapa, estaAtualizando) {
    var listaEtapaChecklistCopia = [...listaEtapaChecklist];

    if (estaAtualizando) {
        for (var i = 0; i < listaEtapaChecklistCopia.length; i++) {
            if (etapa.Codigo == listaEtapaChecklistCopia[i].Codigo) {
                listaEtapaChecklistCopia[i] = etapa;
                break;
            }
        }
    } else {
        listaEtapaChecklistCopia.push(etapa);
    }

    const ordens = listaEtapaChecklistCopia.map(etapaLista => etapaLista.Ordem);

    const duplicados = ordens.filter((item, index) =>
        ordens.indexOf(item) !== index
    );

    if (duplicados.length) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, `Já existe uma etapa de ordem ${duplicados[0]}`);
        return false;
    }

    return true
}