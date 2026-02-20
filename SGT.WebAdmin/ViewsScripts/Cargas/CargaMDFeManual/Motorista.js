
var _gridMotorista = null;

function LoadMotoristas() {
    var menuOpcoes;
    var header;
    if (_CONFIGURACAO_TMS.UtilizaControlePercentualExecucao) {
        let excluir = {
            descricao: "Excluir", id: guid(), metodo: function (data) {
                ExcluirMotorista(data.Codigo, false);
            }, icone: "", visibilidade: true
        };
        let editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: editarMotorista, tamanho: "15", icone: "" };
        menuOpcoes = new Object();

        menuOpcoes.tipo = TypeOptionMenu.list;
        menuOpcoes.opcoes = new Array();
        menuOpcoes.opcoes.push(editar, excluir);

        header = [
            { data: "Codigo", visible: false },
            { data: "Descricao", title: "Descrição", width: "60%" },
            { data: "PercentualExecucao", title: "% Execucão", width: "20%", className: "text-align-right" }
        ];
    } else {
        menuOpcoes = {
            tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
                descricao: "Excluir", id: guid(), metodo: function (data) {
                    ExcluirMotorista(data.Codigo, false);
                }
            }]
        };
        header = [
            { data: "Codigo", visible: false },
            { data: "Descricao", title: "Descrição", width: "60%" },            
        ];
    }

    _gridMotorista = new BasicDataTable(_cargaMDFeManual.Motorista.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    if (_CONFIGURACAO_TMS.UtilizaControlePercentualExecucao) {
        new BuscarMotoristas(_cargaMDFeManual.NomeMotorista, null, _cargaMDFeManual.Empresa, null, true);
    } else {
        new BuscarMotoristas(_cargaMDFeManual.AdicionarMotorista, null, _cargaMDFeManual.Empresa, _gridMotorista, true);
    } 

    _cargaMDFeManual.Motorista.basicTable = _gridMotorista;
    _cargaMDFeManual.Motorista.basicTable.CarregarGrid(new Array());
}

function editarMotorista(data) {
    _cargaMDFeManual.NomeMotorista.enable(false);
    _cargaMDFeManual.NomeMotorista.codEntity(data.Codigo);
    _cargaMDFeManual.NomeMotorista.val(data.Descricao);
    _cargaMDFeManual.PercentualExecucao.val(data.PercentualExecucao);

    _cargaMDFeManual.AdicionarMotoristaComissao.visible(false);
    _cargaMDFeManual.AtualizarMotoristaComissao.visible(true);
    _cargaMDFeManual.ExcluirMotoristaComissao.visible(true);
    _cargaMDFeManual.CancelarMotoristaComissao.visible(true);
}

function excluirMotoristaClick() {
    ExcluirMotorista(_cargaMDFeManual.NomeMotorista.codEntity(), true);   
}

function isValidar(edit) {
    _cargaMDFeManual.NomeMotorista.required(true);

    var validaMotorista = ValidarCampoObrigatorioEntity(_cargaMDFeManual.NomeMotorista);
    var tudoCerto = validaMotorista;

    _cargaMDFeManual.NomeMotorista.required(false);
    _cargaMDFeManual.PercentualExecucao.required(false);

    if (!tudoCerto) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return false;
    }

    var motoristasGrid = _gridMotorista.BuscarRegistros();

    var existe = false;
    $.each(motoristasGrid, function (i, motorista) {
        if (motorista.Codigo == _cargaMDFeManual.NomeMotorista.codEntity() && !edit) {
            existe = true;
            return false;
        }
    });

    if (existe) {
        exibirMensagem(tipoMensagem.aviso, "Motorista já informado", "Esse motorista já foi informado");
        return false;
    }

    if (_cargaMDFeManual.PercentualExecucao.val() > 100) {
        exibirMensagem(tipoMensagem.aviso, "Percentual", "O percentual não pode ser maior que 100%");
        return false;
    }

    var percentualTotal = Globalize.parseFloat(_cargaMDFeManual.PercentualExecucao.val());
    $.each(motoristasGrid, function (i, motorista) {
        if (motorista.Codigo != _cargaMDFeManual.NomeMotorista.codEntity()) {
            percentualTotal = percentualTotal + (isNaN(motorista.PercentualExecucao) ? Globalize.parseFloat(motorista.PercentualExecucao) : motorista.PercentualExecucao);
        }
    });

    if (percentualTotal > 100) {
        exibirMensagem(tipoMensagem.aviso, "Percentual", "A somas dos percentuais não pode ser maior que 100%");
        return false;
    }
    return true;
}

function atualizarMotoristaClick() {
    if (isValidar(true)) {
        var motoristasGrid = _gridMotorista.BuscarRegistros();
        const motorista = motoristasGrid.find(el => el.Codigo == _cargaMDFeManual.NomeMotorista.codEntity());
        motorista.PercentualExecucao = _cargaMDFeManual.PercentualExecucao.val();
        _gridMotorista.CarregarGrid(motoristasGrid);
        limparMotorista();
    }
}



function ExcluirMotorista(codigo, limpar) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, "Realmente deseja excluir o motorista selecionado?",
        function () {
            var motoristaGrid = _gridMotorista.BuscarRegistros();

            for (var i = 0; i < motoristaGrid.length; i++) {
                if (codigo == motoristaGrid[i].Codigo) {
                    motoristaGrid.splice(i, 1);
                    break;
                }
            }

            _gridMotorista.CarregarGrid(motoristaGrid);

            if (limpar) {
                limparMotorista();
            }
        }
    );

}

function limparMotorista(){
    LimparCampoEntity(_cargaMDFeManual.NomeMotorista);
    LimparCampo(_cargaMDFeManual.PercentualExecucao);
    _cargaMDFeManual.NomeMotorista.enable(true);
    _cargaMDFeManual.AdicionarMotoristaComissao.visible(true);
    _cargaMDFeManual.AtualizarMotoristaComissao.visible(false);
    _cargaMDFeManual.ExcluirMotoristaComissao.visible(false);
    _cargaMDFeManual.CancelarMotoristaComissao.visible(false);
}

function adicionarMotoristaClick() { 
    if (isValidar(false)) {
        var motoristasGrid = _gridMotorista.BuscarRegistros();    
        var motorista = {          
            Codigo: _cargaMDFeManual.NomeMotorista.codEntity(),
            Descricao: _cargaMDFeManual.NomeMotorista.val(),
            PercentualExecucao: _cargaMDFeManual.PercentualExecucao.val()
        };
        motoristasGrid.push(motorista);
        _gridMotorista.CarregarGrid(motoristasGrid);
        limparMotorista();
    }    
}
