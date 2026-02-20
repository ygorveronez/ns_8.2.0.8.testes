/// <reference path="../../../wwwroot/js/Global/CRUD.js" />
/// <reference path="../../../wwwroot/js/Global/Grid.js" />
/// <reference path="../../../wwwroot/js/Global/Mensagem.js" />
/// <reference path="../../../wwwroot/js/Global/Rest.js" />
/// <reference path="../../../wwwroot/js/Global/Globais.js" />
/// <reference path="Constantes.js" />


var PesquisaParametrosOfertas = function () {
    this.Descricao = PropertyEntity({ text: "Descrição" });
    this.CodigoIntegracao = PropertyEntity({ text: "Código de Integração", maxlength: 50 });
    this.Situacao = PropertyEntity({ val: ko.observable(0), options: _statusPesquisa, def: "", text: "Situação" });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridParametrosOfertas.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

function LigarModoEdicao(on = true) {
    _crudGeral.Adicionar.visible(!on);
    _crudGeral.Atualizar.visible(on);
    _crudGeral.Cancelar.visible(on);
}

function MontarCaminho(controller, endpoint) {
    return `${controller}/${endpoint}`
}

function AdicionarParametrosOfertasClick() {

    _parametrosOfertas.TiposCarga.val(_tiposCarga.TipoCarga.basicTable.BuscarRegistros());
    _parametrosOfertas.TiposOperacao.val(_tiposOperacao.TipoOperacao.basicTable.BuscarRegistros());
    _parametrosOfertas.Empresas.val(_empresas.Empresa.basicTable.BuscarRegistros());
    _parametrosOfertas.Filiais.val(_filiais.Filial.basicTable.BuscarRegistros());
    _parametrosOfertas.Funcionarios.val(_funcionarios.Funcionario.basicTable.BuscarRegistros());

    const validacao = ValidarTodosCamposParametrosOfertas();

    if (validacao.liberado) {
        executarReST(MontarCaminho(CONTROLLER_PARAMETROS_OFERTAS, ENDPOINT_ADICIONAR_PARAMETROS_OFERTAS), ObterParametrosOfertasAdicionarCompleto(), function (retorno) {

            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                    _gridParametrosOfertas.CarregarGrid();
                    LimparGeral();
                }
                else
                    exibirMensagem(tipoMensagem.atencao, "Atenção", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
    else {
        exibirMensagem(tipoMensagem.atencao, validacao.mensagem.titulo, validacao.mensagem.corpo);
        Global.ResetarAbas();
    }
}

function AtualizarParametrosOfertasClick() {

    _parametrosOfertas.TiposCarga.val(_tiposCarga.TipoCarga.basicTable.BuscarRegistros());
    _parametrosOfertas.TiposOperacao.val(_tiposOperacao.TipoOperacao.basicTable.BuscarRegistros());
    _parametrosOfertas.Empresas.val(_empresas.Empresa.basicTable.BuscarRegistros());
    _parametrosOfertas.Filiais.val(_filiais.Filial.basicTable.BuscarRegistros());
    _parametrosOfertas.Funcionarios.val(_funcionarios.Funcionario.basicTable.BuscarRegistros());

    const validacao = ValidarTodosCamposParametrosOfertas();

    if (validacao.liberado) {
        executarReST(MontarCaminho(CONTROLLER_PARAMETROS_OFERTAS, ENDPOINT_ATUALIZAR_PARAMETROS_OFERTAS), ObterParametrosOfertasAtualizarCompleto(), function (retorno) {

            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso.");
                    _gridParametrosOfertas.CarregarGrid();
                    LimparGeral();
                    LigarModoEdicao(false);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
    else {
        exibirMensagem(tipoMensagem.atencao, validacao.mensagem.titulo, validacao.mensagem.corpo);
        Global.ResetarAbas();
    }
}

function RecuperarParametrosOfertas(codigo) {

    if (codigo != null && codigo > 0) {
        executarReST(MontarCaminho(CONTROLLER_PARAMETROS_OFERTAS, ENDPOINT_RECUPERAR_PARAMETROS_OFERTAS), { Codigo: codigo }, function (retorno) {

            if (retorno.Success) {
                if (retorno.Data) {
                    PreencherParametrosOfertas(retorno.Data.ParametrosOfertas);
                    PreencherEmpresas(retorno.Data.Empresas);
                    PreencherFiliais(retorno.Data.Filiais);
                    PreencherFuncionarios(retorno.Data.Funcionarios);
                    PreencherTiposCarga(retorno.Data.TiposCarga);
                    PreencherTiposOperacao(retorno.Data.TiposOperacao);
                    PreencherTiposIntegracao(retorno.Data.TiposIntegracao);
                    PreencherParametrosDadosOferta(retorno.Data.ParametrosOfertasDadosOfertas);

                    LigarModoEdicao();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    }
    else {
        exibirMensagem(tipoMensagem.atencao, MSG_AVISO_TITULO_CAMPOS_OBRIGATORIOS, "Por favor, informe os campos obrigatórios.");
        Global.ResetarAbas();
    }
}

function ValidarTodosCamposDadosOferta(list) {
    if (list.length < 1) {
        return true;
    }

    let returning = true;

    for (const data of list) {
        if (!data.DiasSemana.length ||
            !data.HoraInicio ||
            !data.HoraTermino
        ) {
            returning = false;
            break;
        }
    }

    return returning;
}

function ValidarTodosCamposTipoIntegracao(list) {
    if (list.length < 1) {
        return true;
    }

    let returning = true;

    for (const data of list) {
        if (!data) {
            returning = false;
            break;
        }
    }

    return returning;
}

function ValidarTodosCamposRelacionamentosEstrangeiros(list) {
    if (list.length < 1) {
        return true;
    }

    let returning = true;

    for (const data of list) {
        if (!data.Descricao ||
            !data.Codigo
        ) {
            returning = false;
            break;
        }
    }

    return returning;
}

function ValidarTodosCamposParametrosOfertas() {

    if (!_parametrosOfertas.Descricao.val() ||
        !ValidarTodosCamposDadosOferta(_parametrosOfertas.ParametrosOfertasDadosOferta.val()) ||
        !ValidarTodosCamposTipoIntegracao(_parametrosOfertas.TiposIntegracao.val()) ||
        !ValidarTodosCamposRelacionamentosEstrangeiros(_parametrosOfertas.TiposOperacao.val()) ||
        !ValidarTodosCamposRelacionamentosEstrangeiros(_parametrosOfertas.Empresas.val()) ||
        !ValidarTodosCamposRelacionamentosEstrangeiros(_parametrosOfertas.Filiais.val()) ||
        !ValidarTodosCamposRelacionamentosEstrangeiros(_parametrosOfertas.Funcionarios.val()) ||
        !ValidarTodosCamposRelacionamentosEstrangeiros(_parametrosOfertas.TiposCarga.val())
    ) {
        return {
            liberado: false,
            mensagem: {
                titulo: MSG_AVISO_TITULO_CAMPOS_OBRIGATORIOS,
                corpo: "Por favor, informe os campos obrigatórios."
            }
        };
    }

    if (
        _parametrosOfertas.ParametrosOfertasDadosOferta.val().length === 0 &&
        _parametrosOfertas.TiposIntegracao.val().length === 0 &&
        _parametrosOfertas.TiposOperacao.val().length === 0 &&
        _parametrosOfertas.Empresas.val().length === 0 &&
        _parametrosOfertas.Filiais.val().length === 0 &&
        _parametrosOfertas.Funcionarios.val().length === 0 &&
        _parametrosOfertas.TiposCarga.val().length === 0
    ) {
        return {
            liberado: false,
            mensagem: {
                titulo: "Relacionamento Necessário",
                corpo: "O preenchimento de ao menos uma das outras abas é necessário para salvar um Parâmetro de Ofertas."
            }
        };
    }


    return {
        liberado: true,
        mensagem: {}
    };
}

function buscarIntegracoes() {
    return new Promise(function (resolve) {
        executarReST("TipoOperacao/BuscarIntegracoes", {}, function (retorno) {
            if (retorno.Success && retorno.Data) {
                var integracoes = retorno.Data.Integracoes.map(function (d) { return { value: d.Tipo, text: d.Descricao } });
                _parametrosOfertasTipoIntegracao.TipoIntegracao.options(integracoes);
            }
            resolve();
        });
    });
}

function LimparGeral() {
    LimparCamposParametrosOfertas();
    LimparCamposParametrosOfertasDadosOferta(true);
    LimparCamposParametrosOfertasTipoIntegracao(true);
    LimparCamposEmpresas();
    LimparCamposFiliais();
    LimparCamposFuncionarios();
    LimparCamposTiposCargas();
    LimparCamposTiposOperacao();
}
