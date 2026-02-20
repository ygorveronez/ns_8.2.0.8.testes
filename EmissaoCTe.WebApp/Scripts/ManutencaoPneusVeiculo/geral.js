var padraoDraggable = {
    revert: 'invalid',
    addClasses: false,
    opacity: 0.5,
    revertDuration: 200,
    containment: "document"
};

var padraoDroppableSaida = {
    accept: ".pneu, .pneuManutencao",
    activeClass: "droppableActive",
    hoverClass: "droppableHover",
    addClasses: false,
    drop: function (event, ui) {

        $("body").data("pneuDrop", $("#" + ui.draggable[0].id).data("pneu"));
        $("body").data("eixoDrop", $(this).data("eixo"));
        $("body").data("pneuUI", ui);

        AbrirTelaSaidaPneu();
    }
};

var padraoDroppableEntrada = {
    accept: ".pneuEixo",
    activeClass: "droppableActive",
    hoverClass: "droppableHover",
    addClasses: false,
    drop: function (event, ui) {

        $("body").data("pneuDrop", $("#" + ui.draggable[0].id).data("pneu"));
        $("body").data("eixoDrop", $(ui.draggable).parent(".eixo").data("eixo"));
        $("body").data("entradaDrop", $(this)[0].id);
        $("body").data("pneuUI", ui);

        AbrirTelaEntradaPneu();
    }
};

$(function () {
    CarregarConsultaDeVeiculos("btnBuscarVeiculo", "btnBuscarVeiculo", RetornoConsultaVeiculo, true, false);

    $("#txtVeiculo").keydown(function (e) {
        if (e.which != 9 && e.which != 16) {
            if (e.which == 8 || e.which == 46) {
                $("body").data("codigoVeiculo", null);
                $(this).val("");
            } else {
                e.preventDefault();
            }
        }
    });

    $("#divContainerEstoquePneu").droppable(padraoDroppableEntrada);

    $("#divContainerManutencaoPneu").droppable(padraoDroppableEntrada);

    $("#divDescartePneu").droppable(padraoDroppableEntrada);

    AtualizarEstoquePneus(0, true);
    AtualizarManutencaoPneus(0, true);
});

function RetornoConsultaVeiculo(veiculo) {
    $("body").data("codigoVeiculo", veiculo.Codigo);
    $("#txtVeiculo").val(veiculo.Placa);

    BuscarEixosVeiculo(veiculo.Codigo);
    BuscarKilometragemVeiculo(veiculo);
}

function BuscarKilometragemVeiculo(veiculo) {
    executarRest("/Veiculo/BuscarKilometragemPorPlaca?callback=?", { Placa: veiculo.Placa }, function (r) {
        if (r.Sucesso) {
            $("#txtKMSaida").val(Globalize.format(r.Objeto.KilometragemAtual, "n0"));
            $("#txtKMEntrada").val(Globalize.format(r.Objeto.KilometragemAtual, "n0"));
        } else {
            ExibirMensagemErro(r.Erro, "Atenção");
        }
    });
}

function BuscarEixosVeiculo(codigoVeiculo) {
    $("#veiculo").hide();

    executarRest("/Veiculo/BuscarEixosVeiculo?callback=?", { CodigoVeiculo: codigoVeiculo }, function (r) {
        if (r.Sucesso) {

            if (r.Objeto.length > 0) {

                $("#veiculo .eixo").droppable("destroy");

                var eixos = new Array();
                var estepes = new Array();

                for (var i = 0; i < r.Objeto.length; i++) {
                    if (r.Objeto[i].Tipo == "E") {
                        estepes.push(r.Objeto[i]);
                    } else {
                        var existe = false;

                        for (var x = 0; x <= eixos.length; x++) {
                            if (eixos[r.Objeto[i].OrdemEixo] != null) {
                                eixos[r.Objeto[i].OrdemEixo].push(i);
                                existe = true;
                                break;
                            }
                        }

                        if (!existe)
                            eixos[r.Objeto[i].OrdemEixo] = [i];
                    }
                }

                var divEixosEsquerda = document.getElementById("eixosEsquerda");
                var divEixosDireita = document.getElementById("eixosDireita");
                var divEstepes = document.getElementById("estepes");

                divEixosDireita.innerHTML = "";
                divEixosEsquerda.innerHTML = "";
                divEstepes.innerHTML = "";

                eixos.forEach(function (indicesEixo) {
                    var esquerdoExterno, esquerdoInterno, direitoInterno, direitoExterno;

                    for (var i = 0; i < indicesEixo.length; i++) {
                        var eixo = r.Objeto[indicesEixo[i]];

                        if (eixo.Posicao == "D" && eixo.Interno_Externo == "I")
                            direitoInterno = eixo;
                        else if (eixo.Posicao == "D" && eixo.Interno_Externo == "E")
                            direitoExterno = eixo;
                        else if (eixo.Posicao == "E" && eixo.Interno_Externo == "I")
                            esquerdoInterno = eixo;
                        else if (eixo.Posicao == "E" && eixo.Interno_Externo == "E")
                            esquerdoExterno = eixo;
                    }

                    CriarEixos(divEixosEsquerda, esquerdoExterno, esquerdoInterno);
                    CriarEixos(divEixosDireita, direitoInterno, direitoExterno);
                });

                estepes.forEach(function (estepe) {
                    CriarEstepe(divEstepes, estepe);
                });

                $("#veiculo").show();

            }
        } else {
            ExibirMensagemErro(r.Erro, "Atenção!");
        }
    });
}

function CriarEstepe(divEstepeContainer, eixo) {
    divEstepeContainer.appendChild(ObterElementoEixo(eixo, "3"));

    VincularEventosAoEixo(eixo);
}

function CriarEixos(divEixoContainer, eixoEsquerda, eixoDireita) {
    var divEixo = document.createElement("div");
    divEixo.classList.add("col-12");

    divEixoContainer.appendChild(divEixo);

    var divRowEixo = document.createElement("div");
    divRowEixo.classList.add("row");

    divEixo.appendChild(divRowEixo);

    divRowEixo.appendChild(ObterElementoEixo(eixoEsquerda, "6"));
    divRowEixo.appendChild(ObterElementoEixo(eixoDireita, "6"));

    VincularEventosAoEixo(eixoEsquerda);
    VincularEventosAoEixo(eixoDireita);
}

function VincularEventosAoEixo(eixo) {
    if (eixo != null) {
        $("#eixo_" + eixo.Codigo).data("eixo", eixo);

        $("#eixo_" + eixo.Codigo).droppable(padraoDroppableSaida);

        if (eixo.Pneu != null) {
            $("#pneu_" + eixo.Pneu.Codigo).data("pneu", eixo.Pneu);

            $("#pneu_" + eixo.Pneu.Codigo).draggable(padraoDraggable);
            $("#eixo_" + eixo.Codigo).droppable("option", "disabled", true);
        }
    }
}

function ObterElementoEixo(eixo, tamanho) {
    var divEixo = document.createElement("div");
    divEixo.className = (eixo == null ? "hidden-xs" : "col-xs-12") + " col-sm-" + tamanho + " col-md-" + tamanho + " col-lg-" + tamanho;

    if (eixo != null) {
        var divPanelEixoEsquerdoExterno = document.createElement("div");
        divPanelEixoEsquerdoExterno.className = "panel panel-default";

        var divHeaderPanelEixoEsquerdoExterno = document.createElement("div");
        divHeaderPanelEixoEsquerdoExterno.className = "panel-heading bold";
        divHeaderPanelEixoEsquerdoExterno.innerText = eixo.Descricao;

        var divBodyPanelEixoEsquerdoExterno = document.createElement("div");
        divBodyPanelEixoEsquerdoExterno.className = "panel-body eixo";
        divBodyPanelEixoEsquerdoExterno.id = "eixo_" + eixo.Codigo;

        if (eixo.Pneu != null)
            divBodyPanelEixoEsquerdoExterno.appendChild(ObterElementoPneu(eixo.Pneu, "pneuEixo"));
        else
            divBodyPanelEixoEsquerdoExterno.innerText = "Arraste um pneu para vincular ao eixo.";

        divPanelEixoEsquerdoExterno.appendChild(divHeaderPanelEixoEsquerdoExterno);
        divPanelEixoEsquerdoExterno.appendChild(divBodyPanelEixoEsquerdoExterno);
        divEixo.appendChild(divPanelEixoEsquerdoExterno);
    }

    return divEixo;
}

function ObterElementoPneu(pneu, classe) {
    var divPneu = document.createElement("div");
    divPneu.className = classe;
    divPneu.id = "pneu_" + pneu.Codigo;

    var headerPneu = document.createElement("header");
    headerPneu.innerText = pneu.Serie + " - " + pneu.Descricao;

    divPneu.appendChild(headerPneu);

    return divPneu;
}

function AtualizarEstoquePneus(pagina, paginacao) {
    executarRest("/Pneu/ConsultarParaManutencao?callback=?", { inicioRegistros: pagina * 12, Serie: $("#txtSeriePneuEstoque").val(), Status: "A" }, function (r) {
        if (r.Sucesso) {
            if (paginacao) {
                $("#paginationEstoquePneu").twbsPagination({
                    totalPages: r.Objeto.Total <= 0 ? 1 : Math.ceil(r.Objeto.Total / 12),
                    visiblePages: 5,
                    first: '<<',
                    prev: "<",
                    next: ">",
                    last: ">>",
                    onPageClick: function (event, page) {
                        AtualizarEstoquePneus(page - 1, false);
                    }
                });
            }

            var divContainerEstoquePneu = document.getElementById("divContainerEstoquePneu");

            divContainerEstoquePneu.innerHTML = "";

            if (r.Objeto.Pneus.length > 0) {
                for (var i = 0; i < r.Objeto.Pneus.length; i++) {
                    var div = document.createElement("div");
                    div.classList.add("pneu");
                    div.id = "pneu_" + r.Objeto.Pneus[i].Codigo;

                    var header = document.createElement("header");
                    header.innerText = r.Objeto.Pneus[i].Serie + " - " + r.Objeto.Pneus[i].Descricao;

                    div.appendChild(header);

                    divContainerEstoquePneu.appendChild(div);

                    $("#pneu_" + r.Objeto.Pneus[i].Codigo).data("pneu", r.Objeto.Pneus[i]);
                }

                $("#divContainerEstoquePneu .pneu").draggable(padraoDraggable);
            } else {
                divContainerEstoquePneu.innerHTML = "Arraste um pneu aqui para adicionar à manutenção.";
            }
        } else {
            ExibirMensagemErro(r.Erro, "Atenção!");
        }
    });
}

function AtualizarManutencaoPneus(pagina, paginacao) {
    executarRest("/Pneu/ConsultarParaManutencao?callback=?", { inicioRegistros: pagina * 12, Serie: $("#txtSeriePneuManutencao").val(), Status: "S" }, function (r) {
        if (r.Sucesso) {
            if (paginacao) {
                $("#paginationManutencaoPneu").twbsPagination({
                    totalPages: r.Objeto.Total <= 0 ? 1 : Math.ceil(r.Objeto.Total / 12),
                    visiblePages: 5,
                    first: '<<',
                    prev: "<",
                    next: ">",
                    last: ">>",
                    onPageClick: function (event, page) {
                        AtualizarEstoquePneus(pagina, false);
                    }
                });
            }

            var divContainerEstoquePneu = document.getElementById("divContainerManutencaoPneu");

            divContainerEstoquePneu.innerHTML = "";

            if (r.Objeto.Pneus.length > 0) {
                for (var i = 0; i < r.Objeto.Pneus.length; i++) {
                    var div = document.createElement("div");
                    div.classList.add("pneuManutencao");
                    div.id = "pneu_" + r.Objeto.Pneus[i].Codigo;

                    var header = document.createElement("header");
                    header.innerText = r.Objeto.Pneus[i].Serie + " - " + r.Objeto.Pneus[i].Descricao;

                    div.appendChild(header);

                    divContainerEstoquePneu.appendChild(div);

                    $("#pneu_" + r.Objeto.Pneus[i].Codigo).data("pneu", r.Objeto.Pneus[i]);
                }

                $("#divContainerManutencaoPneu .pneuManutencao").draggable(padraoDraggable);
            } else {
                divContainerEstoquePneu.innerHTML = "Arraste um pneu aqui para adicionar à manutenção.";
            }
        } else {
            ExibirMensagemErro(r.Erro, "Atenção!");
        }
    });
}