/// <reference path="../../../wwwroot/js/Global/CRUD.js" />
/// <reference path="../../../wwwroot/js/Global/Grid.js" />
/// <reference path="../../../wwwroot/js/Global/Mensagem.js" />
/// <reference path="../../../wwwroot/js/Global/Rest.js" />
/// <reference path="./GrupoMotoristas.js" />
/// <reference path="./Etapa.js" />

function LimparGeral() {
    LimparCamposGrupoMotoristas();
    LimparCamposFuncionarios();
    LimparCamposGrupoMotoristasTipoIntegracao(true);

    setarEtapaInicioGrupoMotoristas();
}

function LimparCamposGrupoMotoristas() {
    LimparCampos(_grupoMotoristas);

    _grupoMotoristas.TiposIntegracao.val([]);
}

function LimparCamposFuncionarios() {
    LimparCampos(_funcionarios);
    _gridFuncionarios.CarregarGrid(new Array());
}

function LimparCamposGrupoMotoristasTipoIntegracao(comGrid) {
    LimparCampos(_grupoMotoristasTipoIntegracao);

    if (comGrid) {
        _gridGrupoMotoristasTipoIntegracao.CarregarGrid(new Array());
    }
}

function LigarModoEdicao(on = true) {
    _crudGeral.Adicionar.visible(!on);
    _crudGeral.Atualizar.visible(on);
    _crudGeral.Cancelar.visible(on);
    _crudGeral.Excluir.visible(on);

    _grupoMotoristas.Ativo.visible(on);

    if (on) {
        _grupoMotoristas.Descricao.cssClass("col col-xs-12 col-sm-12 col-md-4 col-lg-4");
        _grupoMotoristas.CodigoIntegracao.cssClass("col col-xs-12 col-sm-12 col-md-4 col-lg-4");
    } else {
        _grupoMotoristas.Descricao.cssClass("col col-xs-12 col-sm-12 col-md-6 col-lg-6");
        _grupoMotoristas.CodigoIntegracao.cssClass("col col-xs-12 col-sm-12 col-md-6 col-lg-6");
    }
}

function ValidarTodosCamposGrupoMotoristas() {
    if (!_grupoMotoristas.Descricao.val() ||
        !ValidarTodosCamposRelacionamentosEstrangeiros(_grupoMotoristas.Funcionarios.val())
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
        _grupoMotoristas.Funcionarios.val().length === 0
    ) {
        return {
            liberado: false,
            mensagem: {
                titulo: "Relacionamento Necessário",
                corpo: "O preenchimento de ao menos um Motorista para salvar um Grupo de Motoristas."
            }
        };
    }


    return {
        liberado: true,
        mensagem: {}
    };
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