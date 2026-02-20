/// <reference path="../../../wwwroot/js/Global/CRUD.js" />
/// <reference path="../../../wwwroot/js/Global/Grid.js" />
/// <reference path="../../../wwwroot/js/Global/Mensagem.js" />
/// <reference path="../../../wwwroot/js/Global/Rest.js" />
/// <reference path="Constantes.js" />
/// <reference path="Etapa.js" />


function BuscarGrupoMotoristas() {
    let selecionar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: function (data) { SelecionarGrupoMotoristas(data); }, tamanho: "10", icone: "" };

    let menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", opcoes: [selecionar], tamanho: 10 };

    _gridGrupoMotoristas = new GridView(_pesquisaGrupoMotoristas.Pesquisar.idGrid, `${CONTROLLER_GRUPO_MOTORISTAS}/${ENDPOINT_PESQUISAR_GRUPO_MOTORISTAS}`, _pesquisaGrupoMotoristas, menuOpcoes);
    _gridGrupoMotoristas.CarregarGrid();
}

function AdicionarGrupoMotoristasClick() {

    _grupoMotoristas.Funcionarios.val(_funcionarios.Funcionario.basicTable.BuscarRegistros());

    const validacao = ValidarTodosCamposGrupoMotoristas();

    if (validacao.liberado) {
        exibirConfirmacao("Atenção!", `Deseja realmente criar o grupo de motoristas '${_grupoMotoristas.Descricao.val()}'?`, function () {
            executarReST(`${CONTROLLER_GRUPO_MOTORISTAS}/${ENDPOINT_ADICIONAR_GRUPO_MOTORISTAS}`, ObterGrupoMotoristasAdicionarCompleto(), function (retorno) {

                if (retorno.Success) {
                    if (retorno.Data) {
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                        _gridGrupoMotoristas.CarregarGrid();
                        LimparGeral();
                        Global.ResetarAbas();
                    }
                    else
                        exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
                }
                else
                    exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            });
        });
    }
    else {
        exibirMensagem(tipoMensagem.atencao, validacao.mensagem.titulo, validacao.mensagem.corpo);
        Global.ResetarAbas();
    }
}

function AtualizarGrupoMotoristasClick() {
    _grupoMotoristas.Funcionarios.val(_funcionarios.Funcionario.basicTable.BuscarRegistros());

    const validacao = ValidarTodosCamposGrupoMotoristas();

    if (validacao.liberado) {
        exibirConfirmacao("Atenção!", `Deseja realmente atualizar o grupo de motoristas '${_grupoMotoristas.Descricao.val()}'?`, function () {
            executarReST(`${CONTROLLER_GRUPO_MOTORISTAS}/${ENDPOINT_ATUALIZAR_GRUPO_MOTORISTAS}`, ObterGrupoMotoristasAtualizarCompleto(), function (retorno) {

                if (retorno.Success) {
                    if (retorno.Data) {
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso.");
                        _gridGrupoMotoristas.CarregarGrid();
                        LimparGeral();
                        LigarModoEdicao(false);
                        Global.ResetarAbas();
                    }
                    else
                        exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
                }
                else
                    exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
            });
        });
    }
    else {
        exibirMensagem(tipoMensagem.atencao, validacao.mensagem.titulo, validacao.mensagem.corpo);
        Global.ResetarAbas();
    }
}

function ExcluirGrupoMotoristasClick() {
    let codigoGrupoMotorista = _grupoMotoristas.Codigo.val();
    exibirConfirmacao("Atenção!", `Deseja realmente excluir o grupo de motoristas '${_grupoMotoristas.Descricao.val()}'?`, function () {
        let dados = { Codigo: codigoGrupoMotorista };

        executarReST(`${CONTROLLER_GRUPO_MOTORISTAS}/${ENDPOINT_EXCLUIR_GRUPO_MOTORISTAS}`, dados, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso.");
                    _gridGrupoMotoristas.CarregarGrid();
                    LimparGeral();
                    LigarModoEdicao(false);
                    Global.ResetarAbas();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });

    LimparGeral();
    LigarModoEdicao(false);
}

function RecuperarGrupoMotoristas(codigo) {

    if (codigo != null && codigo > 0) {
        executarReST(`${CONTROLLER_GRUPO_MOTORISTAS}/${ENDPOINT_RECUPERAR_GRUPO_MOTORISTAS}`, { Codigo: codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    PreencherGrupoMotoristas(retorno.Data.GrupoMotoristas);
                    PreencherFuncionarios(retorno.Data.Funcionarios);
                    PreencherTiposIntegracao(retorno.Data.TiposIntegracao);

                    LigarModoEdicao();

                    setarEtapaInicioGrupoMotoristas();
                    setarEtapasGrupoMotoristas();

                    CarregaGrupoMotoristasIntegracao();
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

function buscarIntegracoes() {
    return new Promise(function (resolve) {
        executarReST("GrupoTransportador/BuscarTiposIntegracoes", {}, function (retorno) {
            if (retorno.Success && retorno.Data) {
                var integracoes = retorno.Data.Integracoes.map(function (d) { return { value: d.Tipo, text: d.Descricao } });

                _grupoMotoristasTipoIntegracao.TipoIntegracao.options(integracoes);

                integracoes.unshift({ value: EnumTipoIntegracao.NaoInformada, text: "Todas" });
                _grupoMotoristasIntegracao.TipoConsultaGrupoMotoristas.options(integracoes);
            }
            resolve();
        });
    });
}



