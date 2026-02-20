//Variaveis Internas
var idDivBusca = "CTe_Consulta_DivBox";
var idDivContainerGrid = "CTe_Consulta_DivContainerGrid";
var idDivGrid = "CTe_Consulta_DivGrid";
var idDivContainerPaginacao = "CTe_Consuta_DivContainerPaginacao";
var idBtnBusca = "CTe_Consulta_BtnBuscar";
var html = "<div id='#divbusca#' title='#tituloPesquisa#'><div class='form'><div class='fields'><div class='fieldzao'>#divsInputs#<div class='field fieldum'><div class='buttons'><input type='button' id='#idbtnbusca#' value='Buscar' onclick='CTe_Consulta_Buscar()' /></div></div></div></div></div><div id='#iddivcontainergrid#' class='table' style='height: 265px; margin-left: 5px;'></div><div id='#iddivcontainerpaginacao#' class='pagination' style='margin-bottom: 0;'></div></div>";
//Variaveis Globais
var idTxt;
var idBtn;
var campos;
var urlBusca;
var tituloBusca;
var colunasGrid;
var colunasEsconderGrid;
var fnCallback;
var closeSearchBox;
var eventoSearch;

function CTe_Consulta_Carregar(textField, button, camposFiltro, url, titulo, colunas, colunasEsconder, fecharConsulta, usarKeyDownTextField) {
    if (usarKeyDownTextField) {
        $("#" + textField).keydown(function (e) {
            eventoSearch = null;
            CTe_Consulta_Buscar(e, true, textField, button, camposFiltro, url, titulo, colunas, colunasEsconder, fecharConsulta);
        });
    }
    $("#" + button).click(function (e) {
        eventoSearch = e;
        CTe_Consulta_Buscar(undefined, false, textField, button, camposFiltro, url, titulo, colunas, colunasEsconder, fecharConsulta);
    });
}
function CTe_Consulta_Buscar(e, verificarKey, textField, button, camposFiltro, url, titulo, colunas, colunasEsconder, fecharConsulta) {
    if (!e && !verificarKey && !textField) {
        CTe_Consulta_Pesquisar(false, undefined);
    } else {
        idTxt = textField;
        idBtn = button;
        urlBusca = url;
        campos = camposFiltro;
        tituloBusca = titulo;
        colunasGrid = colunas;
        colunasEsconderGrid = colunasEsconder;
        if (!fecharConsulta)
            closeSearchBox = false;
        else
            closeSearchBox = true;
        if (!e)
            e = window.event;
        if (!verificarKey) {
            CTe_Consulta_Pesquisar(true);
        } else if ((e.keyCode == 9) && verificarKey) {
            var dados = {};
            dados["inicioRegistros"] = 0;
            for (var i = 0; i < campos.length; i++) {
                dados[campos[i].DescricaoBusca] = $('#CTe_Consulta_' + campos[i].Id).val();
                if (i == 0) {
                    dados[campos[i].DescricaoBusca] = $('#' + idTxt).val();
                }
                if (dados[campos[i].DescricaoBusca] == undefined) {
                    dados[campos[i].DescricaoBusca] = "";
                }
            }
            executarRest(urlBusca, dados, CTe_Consulta_ResultadoBusca);
        }
    }
}
function CTe_Consulta_ResultadoBusca(dados) {
    if (dados) {
        if (dados.Sucesso) {
            if (dados.TotalRegistros == 1 && colunasGrid.length == 1) {
                FecharDivBusca();
                fnCallback = colunasGrid[0].Evento;
                if (fnCallback != undefined) {
                    fnCallback(dados.Objeto[0]);
                } else {
                    jAlert('Ocorreu uma falha ao retornar os dados da busca. Atualize a página e tente novamente!', 'Falha ao Retornar os Dados');
                }
                return;
            } else {
                CTe_Consulta_Pesquisar(true, dados);
                return;
            }
        }
    }
    jAlert('Ocorreu uma falha ao buscar os dados. Atualize a página e tente novamente!', 'Falha ao Buscar os Dados');
}
function CTe_Consulta_Pesquisar(abrirSearchBox, resultado) {
    if (abrirSearchBox) {
        if ($('#' + idDivBusca + "_" + idTxt).length <= 0) {
            var htmlInputs = "";
            var formatInputs = new Array;
            for (var i = 0; i < campos.length; i++) {
                if (campos.length == 1) {
                    htmlInputs += "<div class='field fieldtres'>";
                } else {
                    htmlInputs += "<div class='field fielddois'>";
                }
                htmlInputs += "<div class='label'><label>" + campos[i].Descricao + ":</label></div><div class='input'><input type='text' id='CTe_Consulta_" + campos[i].Id + "' /></div></div>";
                if (campos[i].Tipo != undefined) {
                    if (campos[i].Tipo != 'text') {
                        formatInputs.push(campos[i]);
                    }
                }
            }
            $('body').append(html.replace("#divsInputs#", htmlInputs).replace("#divbusca#", idDivBusca + "_" + idTxt).replace("#idbtnbusca#", idBtnBusca + "_" + idTxt).replace("#iddivcontainergrid#", idDivContainerGrid + "_" + idTxt).replace("#iddivcontainerpaginacao#", idDivContainerPaginacao + "_" + idTxt).replace("#tituloPesquisa#", tituloBusca));
            for (var i = 0; i < formatInputs.length; i++) {
                switch (formatInputs[i].Tipo) {
                    case 'date':
                        $('#CTe_Consulta_' + formatInputs[i].Id).mask("99/99/9999");
                        $('#CTe_Consulta_' + formatInputs[i].Id).datepicker({
                            dateFormat: "dd/mm/yy",
                            changeMonth: true,
                            changeYear: true
                        });
                        break;
                    case 'phone':
                        $('#CTe_Consulta_' + formatInputs[i].Id).mask("(99)? 999999999");
                        $('#CTe_Consulta_' + formatInputs[i].Id).attr("onfocus", "RemoverMascaraTelefone(this);")
                        $('#CTe_Consulta_' + formatInputs[i].Id).attr("onblur", "AdicionarMascaraTelefone(this);")
                        break;
                    case 'decimal':
                        $('#CTe_Consulta_' + formatInputs[i].Id).priceFormat({ prefix: '' });
                        break;
                    case 'cpf_cnpj':
                        $('#CTe_Consulta_' + formatInputs[i].Id).mask("99999999999?999");
                        break;
                    case 'number':
                        $('#CTe_Consulta_' + formatInputs[i].Id).mask("9?9999999999999999");
                        break;
                }
            }
            for (var i = 0; i < colunasGrid.length; i++) {
                if (i == 0) {
                    var clback_1 = colunasGrid[i].Evento;
                    colunasGrid[i].Evento = function (dados) {
                        CTe_Consulta_Escolheu(dados, clback_1);
                    }
                } else if (i == 1) {
                    var clback_2 = colunasGrid[i].Evento;
                    colunasGrid[i].Evento = function (dados) {
                        CTe_Consulta_Escolheu(dados, clback_2);
                    }
                } else if (i == 2) {
                    var clback_3 = colunasGrid[i].Evento;
                    colunasGrid[i].Evento = function (dados) {
                        CTe_Consulta_Escolheu(dados, clback_3);
                    }
                } else if (i == 3) {
                    var clback_4 = colunasGrid[i].Evento;
                    colunasGrid[i].Evento = function (dados) {
                        CTe_Consulta_Escolheu(dados, clback_4);
                    }
                } else if (i == 4) {
                    var clback_5 = colunasGrid[i].Evento;
                    colunasGrid[i].Evento = function (dados) {
                        CTe_Consulta_Escolheu(dados, clback_5);
                    }
                }
            }
            $('#' + idDivBusca + "_" + idTxt).dialog({
                autoOpen: false,
                height: 400,
                width: 800,
                closeOnEscape: false,
                modal: true,
                draggable: false,
                resizable: false,
                zIndex: 8120,
                close: function () {
                    $('#' + idDivBusca + '_' + idTxt + ' input:text').each(function () {
                        try {
                            $(this).val('');
                        } catch (ex) { }
                    });
                }
            });
        }
        AbrirDivBusca();
    }
    var dados = {};
    dados["inicioRegistros"] = 0;
    for (var i = 0; i < campos.length; i++) {
        dados[campos[i].DescricaoBusca] = $('#CTe_Consulta_' + campos[i].Id).val();
    }
    CriarGridView(urlBusca, dados, idDivGrid + "_" + idTxt, idDivContainerGrid + "_" + idTxt, idDivContainerPaginacao + "_" + idTxt, colunasGrid, colunasEsconderGrid, resultado);
}
function CTe_Consulta_Escolheu(event, callback) {
    if (callback != undefined) {
        if (event != undefined) {
            if (event.data != undefined) {
                callback(event.data, eventoSearch);
            } else {
                callback(event, eventoSearch);
            }
        }
        FecharDivBusca();
    } else {
        jAlert('Ocorreu uma falha ao selecionar os dados. Atualize a página e tente novamente!', 'Atenção');
    }
}
function AbrirDivBusca() {
    try {
        $('#' + idDivBusca + "_" + idTxt).dialog("open");
        if (campos.lengh > 0)
            $('#CTe_Consulta_' + campos[0].Id).focus();
    } catch (e) {
        jAlert("Ocorreu uma falha ao tentar abrir a consulta. Atualize a página e tente novamente!", "Atenção");
    }
}
function FecharDivBusca() {
    try {
        if (closeSearchBox) {
            $('#' + idDivBusca + "_" + idTxt).dialog("close");
        }
    } catch (e) {
        jAlert("Ocorreu uma falha ao tentar fechar a consulta. Atualize a página e tente novamente!", "Atenção");
    }
}

function RemoveConsulta($elements, cb) {
    $elements = $($elements);
    if (typeof cb != "function") cb = function () { };

    var stop = function (evt) {
        if (evt && evt.preventDefault) evt.preventDefault();
    }

    // Keycode pega teclas especiais
    $elements.keydown(function (e) {
        // 8 => Backspace 46 => Del 86 => v 88 => x

        // Chama CB quando limpa input
        if (e.keyCode == 8 || e.keyCode == 46 || e.which == 8 || e.which == 46) return cb($(this));

        // Cancela ação quando o CTRL esta clica (exceto CTRL + C)
        if ((e.keyCode != 67 && e.which != 67) && e.ctrlKey == true) stop(e);
    });

    // Keypress pega as teclas com valores (A-Z 0-9)
    // Quando clicado, mata para nao conseguir digitar nada
    $elements.keypress(function (e) {
        stop(e);
    });

    // Previne a ação de colar
    $elements.on('paste', function (e) {
        stop(e);
    });

    // Previne a ação de recortar
    $elements.on('cut', function (e) {
        stop(e);
    });
}