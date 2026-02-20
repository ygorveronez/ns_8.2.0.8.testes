/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Fronteira.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="MapRequestApi.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _estadosPesquisa = [
    { text: "Todos", value: "" },
    { text: "Acre", value: "AC" },
    { text: "Alagoas", value: "AL" },
    { text: "Amapá", value: "AP" },
    { text: "Amazonas", value: "AM" },
    { text: "Bahia", value: "BA" },
    { text: "Ceará", value: "CE" },
    { text: "Distrito Federal", value: "DF" },
    { text: "Espírito Santo", value: "ES" },
    { text: "Goiás", value: "GO" },
    { text: "Maranhão", value: "MA" },
    { text: "Mato Grosso", value: "MT" },
    { text: "Mato Grosso do Sul", value: "MS" },
    { text: "Minas Gerais", value: "MG" },
    { text: "Pará", value: "PA" },
    { text: "Paraíba", value: "PB" },
    { text: "Paraná", value: "PR" },
    { text: "Pernambuco", value: "PE" },
    { text: "Piauí", value: "PI" },
    { text: "Rio de Janeiro", value: "RJ" },
    { text: "Rio Grande do Norte", value: "RN" },
    { text: "Rio Grande do Sul", value: "RS" },
    { text: "Rondônia", value: "RO" },
    { text: "Roraima", value: "RR" },
    { text: "Santa Catarina", value: "SC" },
    { text: "São Paulo", value: "SP" },
    { text: "Sergipe", value: "SE" },
    { text: "Tocantins", value: "TO" }
];

var _estados = [
    { text: "Acre", value: "AC", capital: "Rio Branco" },
    { text: "Alagoas", value: "AL", capital: "Maceió" },
    { text: "Amapá", value: "AP", capital: "Macapá" },
    { text: "Amazonas", value: "AM", capital: "Manaus" },
    { text: "Bahia", value: "BA", capital: "Salvador" },
    { text: "Ceará", value: "CE", capital: "Fortaleza" },
    { text: "Distrito Federal", value: "DF", capital: "Brasília" },
    { text: "Espírito Santo", value: "ES", capital: "Vitória" },
    { text: "Goiás", value: "GO", capital: "Goiânia" },
    { text: "Maranhão", value: "MA", capital: "São Luís" },
    { text: "Mato Grosso", value: "MT", capital: "Cuiabá" },
    { text: "Mato Grosso do Sul", value: "MS", capital: "Campo Grande" },
    { text: "Minas Gerais", value: "MG", capital: "Belo Horizonte" },
    { text: "Pará", value: "PA", capital: "Belém" },
    { text: "Paraíba", value: "PB", capital: "João Pessoa" },
    { text: "Paraná", value: "PR", capital: "Curitiba" },
    { text: "Pernambuco", value: "PE", capital: "Recife" },
    { text: "Piauí", value: "PI", capital: "Teresina" },
    { text: "Rio de Janeiro", value: "RJ", capital: "Rio de Janeiro" },
    { text: "Rio Grande do Norte", value: "RN", capital: "Natal" },
    { text: "Rio Grande do Sul", value: "RS", capital: "Porto Alegre" },
    { text: "Rondônia", value: "RO", capital: "Porto Velho" },
    { text: "Roraima", value: "RR", capital: "Boa Vista" },
    { text: "Santa Catarina", value: "SC", capital: "Florianópolis" },
    { text: "São Paulo", value: "SP", capital: "São Paulo" },
    { text: "Sergipe", value: "SE", capital: "Aracaju" },
    { text: "Tocantins", value: "TO", capital: "Palmas" }
];

/*
 * Declaração das Classes
 */

var MapaBrasil = function () {
    this.RS = PropertyEntity({ text: "Rio Grande do Sul", val: ko.observable("RS"), def: "RS", list: ["SC"], cssClass: ko.observable("default"), icon: ko.observable(""), icon: ko.observable("") });
    this.SC = PropertyEntity({ text: "Santa Catarina", val: ko.observable("SC"), def: "SC", list: ["RS", "PR"], cssClass: ko.observable("default"), icon: ko.observable("") });
    this.PR = PropertyEntity({ text: "Paraná", val: ko.observable("PR"), def: "PR", list: ["SC", "MS", "SP"], cssClass: ko.observable("default"), icon: ko.observable("") });
    this.SP = PropertyEntity({ text: "São Paulo", val: ko.observable("SP"), def: "SP", list: ["PR", "MS", "MG", "RJ"], cssClass: ko.observable("default"), icon: ko.observable("") });
    this.RJ = PropertyEntity({ text: "Rio de Janeiro", val: ko.observable("RJ"), def: "RJ", list: ["SP", "MG", "ES"], cssClass: ko.observable("default"), icon: ko.observable("") });
    this.ES = PropertyEntity({ text: "Espirito Santo", val: ko.observable("ES"), def: "ES", list: ["RJ", "MG", "BA"], cssClass: ko.observable("default"), icon: ko.observable("") });
    this.MG = PropertyEntity({ text: "Minas Gerais", val: ko.observable("MG"), def: "MG", list: ["SP", "RJ", "ES", "BA", "GO", "DF", "MS"], cssClass: ko.observable("default"), icon: ko.observable("") });
    this.BA = PropertyEntity({ text: "Bahia", val: ko.observable("BA"), def: "BA", list: ["ES", "MG", "GO", "TO", "PI", "PE", "AL", "SE"], cssClass: ko.observable("default"), icon: ko.observable("") });
    this.SE = PropertyEntity({ text: "Sergipe", val: ko.observable("SE"), def: "SE", list: ["BA", "AL"], cssClass: ko.observable("default"), icon: ko.observable("") });
    this.AL = PropertyEntity({ text: "Alagoas", val: ko.observable("AL"), def: "AL", list: ["SE", "BA", "PE"], cssClass: ko.observable("default"), icon: ko.observable("") });
    this.PE = PropertyEntity({ text: "Pernambuco", val: ko.observable("PE"), def: "PE", list: ["AL", "BA", "PI", "CE", "PB"], cssClass: ko.observable("default"), icon: ko.observable("") });
    this.PB = PropertyEntity({ text: "Paraíba", val: ko.observable("PB"), def: "PB", list: ["PE", "CE", "RN"], cssClass: ko.observable("default"), icon: ko.observable("") });
    this.RN = PropertyEntity({ text: "Rio Grande do Norte", val: ko.observable("RN"), list: ["PB", "CE"], def: "RN", cssClass: ko.observable("default"), icon: ko.observable("") });
    this.CE = PropertyEntity({ text: "Ceará", val: ko.observable("CE"), def: "CE", list: ["RN", "PB", "PE", "PI"], cssClass: ko.observable("default"), icon: ko.observable("") });
    this.PI = PropertyEntity({ text: "Piauí", val: ko.observable("PI"), def: "PI", list: ["CE", "PE", "BA", "TO", "MA"], cssClass: ko.observable("default"), icon: ko.observable("") });
    this.MA = PropertyEntity({ text: "Maranhão", val: ko.observable("MA"), def: "MA", list: ["PI", "TO", "PA"], cssClass: ko.observable("default"), icon: ko.observable("") });
    this.TO = PropertyEntity({ text: "Tocantins", val: ko.observable("TO"), def: "TO", list: ["MA", "PI", "BA", "GO", "BA", "MT", "PA"], cssClass: ko.observable("default"), icon: ko.observable("") });
    this.GO = PropertyEntity({ text: "Goiás", val: ko.observable("GO"), def: "GO", list: ["TO", "BA", "MG", "DF", "MS", "MT"], cssClass: ko.observable("default"), icon: ko.observable("") });
    this.DF = PropertyEntity({ text: "Distrito Federal", val: ko.observable("DF"), def: "DF", list: ["GO", "MG"], cssClass: ko.observable("default"), icon: ko.observable("") });
    this.MS = PropertyEntity({ text: "Mato Grosso do Sul", val: ko.observable("MS"), def: "MS", list: ["PR", "SP", "MG", "GO", "MT"], cssClass: ko.observable("default"), icon: ko.observable("") });
    this.MT = PropertyEntity({ text: "Mato Grosso", val: ko.observable("MT"), def: "MT", list: ["MS", "GO", "TO", "PA", "AM", "RO"], cssClass: ko.observable("default"), icon: ko.observable("") });
    this.PA = PropertyEntity({ text: "Pará", val: ko.observable("PA"), def: "PA", list: ["MA", "TO", "MT", "AM", "RR", "AP"], cssClass: ko.observable("default"), icon: ko.observable("") });
    this.AP = PropertyEntity({ text: "Amapá", val: ko.observable("AP"), def: "AP", list: ["PA"], cssClass: ko.observable("default"), icon: ko.observable("") });
    this.RR = PropertyEntity({ text: "Roraima", val: ko.observable("RR"), def: "RR", list: ["PA", "AM"], cssClass: ko.observable("default"), icon: ko.observable("") });
    this.AM = PropertyEntity({ text: "Amazonas", val: ko.observable("AM"), def: "AM", list: ["RR", "PA", "MT", "RO", "AC"], cssClass: ko.observable("default"), icon: ko.observable("") });
    this.RO = PropertyEntity({ text: "Rondônia", val: ko.observable("RO"), def: "RO", list: ["AM", "AC", "MT"], cssClass: ko.observable("default"), icon: ko.observable("") });
    this.AC = PropertyEntity({ text: "Acre", val: ko.observable("AC"), def: "AC", list: ["AM", "RO"], cssClass: ko.observable("default"), icon: ko.observable("") });

    this.LimparPassagens = PropertyEntity({ eventClick: null, type: types.event, text: Localization.Resources.Logistica.MapaBrasil.LimparPassagens, visible: ko.observable(true) });
    this.BuscarRotaAPI = PropertyEntity({ eventClick: null, type: types.event, text: Localization.Resources.Logistica.MapaBrasil.BuscarSugestaoDeRota, visible: ko.observable(false) });
    this.TextoPassagem = PropertyEntity({ type: types.local, visible: ko.observable(false), idGrid: guid() });
}

var MapaMDFe = function () {
    var mapaKnout;
    var estadoDestino;
    var estadosPassagem = new Array();
    var localidadesBuscaAPI = new Array();
    var origensDestinos = new Array();

    this.GetMapaKnout = function () {
        return mapaKnout;
    }

    this.GetEstadosPassagem = function () {
        return estadosPassagem;
    }

    this.GetOrigensDestinos = function () {
        return origensDestinos;
    }

    this.AddLocalidadesBuscaAPI = function (localidade) {
        localidadesBuscaAPI.push({ Localidade: localidade });
    }

    this.SetEstadoDestino = function (estado) {
        estadoDestino = estado;
    }

    this.LoadMapaMDFe = function (idDivMapa, callback) {

        $.get("Content/Static/Logistica/MapaBrasil.html?dyn=" + guid(), function (data) {
            var idContent = guid();

            LoadLocalizationResources("Logistica.MapaBrasil").then(function () {

                var html = data;
                html = html.replace(/#svg-map/g, idContent);
                $("#" + idDivMapa).html(html);
                mapaKnout = new MapaBrasil();
                mapaKnout.LimparPassagens.eventClick = function () {
                    estadosPassagem = new Array();
                    limparDisplayEstados(mapaKnout);
                    atualizarDisplayMapaMDFe(origensDestinos, estadosPassagem, mapaKnout);
                }

                mapaKnout.BuscarRotaAPI.eventClick = function () {
                    buscarPassagensEntreEstadosViaMapRequestAPI(localidadesBuscaAPI, function (ufsPassagemAPI) {
                        estadosPassagem = new Array();
                        $.each(ufsPassagemAPI, function (i, ufPassagemAPI) {
                            estadosPassagem.push({ Sigla: ufPassagemAPI.Sigla, Posicao: ufPassagemAPI.Posicao });
                        });
                        limparDisplayEstados(mapaKnout);
                        atualizarDisplayMapaMDFe(origensDestinos, estadosPassagem, mapaKnout);
                    });
                }


                KoBindings(mapaKnout, idDivMapa);

                $("#" + idContent + " a").click(function (e) {
                    var id = $(this).attr('id');
                    var estado = retornarKnoutEstadoPorId(id, mapaKnout);
                    estadoClick(estado, mapaKnout, estadosPassagem, origensDestinos);
                    limparDisplayEstados(mapaKnout);
                    atualizarDisplayMapaMDFe(origensDestinos, estadosPassagem, mapaKnout);
                });



                if (callback != null)
                    callback();

            });
            LocalizeCurrentPage();
        });
    }

    this.LimparMapa = function () {
        localidadesBuscaAPI = new Array();
        estadosPassagem = new Array();
        origensDestinos = new Array();
        limparDisplayEstados(mapaKnout);
    }

    this.AddEstadoPassagem = function (sigla, posicao) {
        estadosPassagem.push({ Sigla: sigla, Posicao: posicao });
    }

    this.AddOrigemDestino = function (origem, destino, codigo) {
        origensDestinos.push({ Codigo: codigo, Origem: origem, Destino: destino, Passagens: new Array() });
    }

    this.AtualizarDisplayMapa = function () {
        atualizarDisplayMapaMDFe(origensDestinos, estadosPassagem, mapaKnout);
    };

    this.ValidarPassagens = function () {
        return validarPassagens(estadosPassagem, origensDestinos, estadoDestino, mapaKnout);
    }
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function estadoClick(estadoKnout, mapaKnout, estadosPassagem, origensDestinos) {
    if (origensDestinos.length > 0) {
        var ultimoEstadoPassagem;
        if (estadosPassagem.length > 0) {
            ultimoEstadoPassagem = estadosPassagem[estadosPassagem.length - 1];
        }

        if (ultimoEstadoPassagem != null) {
            if (ultimoEstadoPassagem.Sigla == estadoKnout.val()) {
                estadosPassagem.splice(estadosPassagem.length - 1, 1);
                atualizarDisplayMapaMDFe(origensDestinos, estadosPassagem, mapaKnout);
            } else {
                knoutUltimoEstado = retornarKnoutEstadoSigla(ultimoEstadoPassagem.Sigla, mapaKnout);
                $.each(knoutUltimoEstado.list, function (i, estadoDivisa) {
                    if (estadoDivisa == estadoKnout.val()) {
                        if (!verificarEstadoExistePassagem(estadosPassagem, estadoDivisa) || ValidarSegundaPassagem(mapaKnout, estadosPassagem, estadoKnout.val())) {
                            estadosPassagem.push({ Sigla: estadoKnout.val(), Posicao: estadosPassagem.length + 1 });
                            atualizarDisplayMapaMDFe(origensDestinos, estadosPassagem, mapaKnout);
                            return false;
                        }
                    }
                });
            }
        } else {
            ultimoEstadoPassagem = { Sigla: origensDestinos[0].Origem };
            knoutUltimoEstado = retornarKnoutEstadoSigla(ultimoEstadoPassagem.Sigla, mapaKnout);
            $.each(knoutUltimoEstado.list, function (i, estadoDivisa) {
                if (estadoDivisa == estadoKnout.val()) {
                    estadosPassagem.push({ Sigla: estadoKnout.val(), Posicao: 1 });
                    atualizarDisplayMapaMDFe(origensDestinos, estadosPassagem, mapaKnout);
                    return false;
                }
            });
        }
    }
}

/*
 * Declaração das Funções Públicas
 */

function DescricaoCapitalEstado(sigla) {
    var descricao = "";

    $.each(_estados, function (i, estado) {
        if (estado.value == sigla) {
            descricao = estado.capital + "," + sigla
            return false;
        }
    });

    return descricao;
}

/*
 * Declaração das Funções Privadas
 */

function atualizarDisplayMapaMDFe(origensDestinos, estadosPassagem, mapaKnout) {
    var countPassagens = {};
    var htmlPassagem = "";

    limparDisplayEstados(mapaKnout);

    $.each(origensDestinos, function (i, origemDestino) {
        var knoutOrigem = retornarKnoutEstadoSigla(origemDestino.Origem, mapaKnout);
        var knoutDestino = retornarKnoutEstadoSigla(origemDestino.Destino, mapaKnout);

        if (knoutOrigem != null && knoutDestino != null) {
            var OrigemIsOrigemDestino = verificarEstadoOrigemOuDestino(knoutOrigem.val(), origensDestinos);
            var DestinoIsOrigemDestino = verificarEstadoOrigemOuDestino(knoutDestino.val(), origensDestinos);

            if (OrigemIsOrigemDestino.origem && OrigemIsOrigemDestino.destino)
                knoutOrigem.cssClass("origemDestino");
            else
                knoutOrigem.cssClass("origem");

            if (DestinoIsOrigemDestino.origem && DestinoIsOrigemDestino.destino)
                knoutDestino.cssClass("origemDestino");
            else
                knoutDestino.cssClass("destino");
        }
    });

    $.each(estadosPassagem, function (i, estado) {
        var knoutEstado = retornarKnoutEstadoSigla(estado.Sigla, mapaKnout);

        htmlPassagem += "<p style='padding:1px'>" + estado.Posicao + " - " + DescricaoEstado(estado.Sigla) + "</p>";

        var countPassagem = countPassagens[estado.Sigla] = (countPassagens[estado.Sigla] || 0) + 1;

        if (countPassagem > 1)
            knoutEstado.icon(knoutEstado.icon() + " e " + estado.Posicao);
        else
            knoutEstado.icon(estado.Posicao);

        var origemDestino = verificarEstadoOrigemOuDestino(estado.Sigla, origensDestinos);

        if (!origemDestino.origem && !origemDestino.destino)
            knoutEstado.cssClass("passagemDisable");
    });

    if (estadosPassagem.length > 0) {
        $("#" + mapaKnout.TextoPassagem.idGrid).html(htmlPassagem);
        mapaKnout.TextoPassagem.visible(true);
    }
    else
        mapaKnout.TextoPassagem.visible(false);

    habilitarCaminhos(estadosPassagem, origensDestinos, mapaKnout);
}

function DescricaoEstado(sigla) {
    var descricao = "";
    $.each(_estados, function (i, estado) {
        if (estado.value == sigla) {
            descricao = estado.text;
            return false;
        }
    });
    return descricao;
}

function fazDivisa(knoutEstado, estado) {
    var fazDivisa = false;
    $.each(knoutEstado.list, function (i, estadoDivisa) {
        if (estadoDivisa == estado) {
            fazDivisa = true;
            return false;
        }
    });
    return fazDivisa
}

function habilitarCaminhos(estadosPassagem, origensDestinos, mapaKnout) {
    var ultimoEstado = retornarUltimoEstadoInformado(estadosPassagem, origensDestinos);

    knoutUltimoEstado = ultimoEstado ? retornarKnoutEstadoSigla(ultimoEstado.ultimoEstado.Sigla, mapaKnout) : undefined;

    if (knoutUltimoEstado) {
        $.each(knoutUltimoEstado.list, function (i, estadoDivisa) {
            if (!verificarEstadoExistePassagem(estadosPassagem, estadoDivisa)) {
                var origemDestino = verificarEstadoOrigemOuDestino(estadoDivisa, origensDestinos);

                var knoutEstadoDivisa = retornarKnoutEstadoSigla(estadoDivisa, mapaKnout);

                if (!origemDestino.origem && !origemDestino.destino)
                    knoutEstadoDivisa.cssClass("defaultEnable");
                else {
                    if (origemDestino.origem && origemDestino.destino) {
                        if (estadosPassagem.length > 0)
                            knoutEstadoDivisa.cssClass("origemDestinoEnable");
                        else
                            knoutEstadoDivisa.cssClass("origemDestino");
                    }
                    else if (origemDestino.origem)
                        knoutEstadoDivisa.cssClass("origemEnable");
                    else
                        knoutEstadoDivisa.cssClass("destinoEnable");
                }
            } else if (ValidarSegundaPassagem(mapaKnout, estadosPassagem, estadoDivisa)) {
                var origemDestinoSegundaPassagem = verificarEstadoOrigemOuDestino(estadoDivisa, origensDestinos);
                var knoutEstadoDivisaSegundaPassagem = retornarKnoutEstadoSigla(estadoDivisa, mapaKnout);

                if (!origemDestinoSegundaPassagem.origem && !origemDestinoSegundaPassagem.destino)
                    knoutEstadoDivisaSegundaPassagem.cssClass("passagem");
                else {
                    if (origemDestinoSegundaPassagem.origem && origemDestinoSegundaPassagem.destino)
                        knoutEstadoDivisaSegundaPassagem.cssClass("origemDestinoEnable");
                    else if (origemDestinoSegundaPassagem.origem)
                        knoutEstadoDivisaSegundaPassagem.cssClass("origemEnable");
                    else
                        knoutEstadoDivisaSegundaPassagem.cssClass("destinoEnable");
                }
            }
        });
    }

    if (estadosPassagem.length > 0) {
        var isOrigemDestino = verificarEstadoOrigemOuDestino(knoutUltimoEstado.val(), origensDestinos);

        if (!isOrigemDestino.origem && !isOrigemDestino.destino)
            knoutUltimoEstado.cssClass("passagem");
        else {
            if (isOrigemDestino.origem && isOrigemDestino.destino)
                knoutUltimoEstado.cssClass("origemDestinoEnable");
            else if (isOrigemDestino.origem)
                knoutUltimoEstado.cssClass("origemEnable");
            else
                knoutUltimoEstado.cssClass("destinoEnable");
        }
    }
}

function limparDisplayEstados(mapaKnout) {
    $.each(mapaKnout, function (i, estado) {
        if (estado.type == types.map) {
            estado.cssClass("default");
            estado.icon("");
        }
    });
}

function retornarEstadoExistePassagem(estadosPassagem, estado) {
    var passagem = null;
    $.each(estadosPassagem, function (i, estadoPassagem) {
        if (estado == estadoPassagem.Sigla) {
            passagem = estadoPassagem;
        }
    });
    return passagem;
}

function retornarKnoutEstadoPorId(id, mapaKnout) {
    var knoutEstado;
    $.each(mapaKnout, function (i, estado) {
        if (estado.type == types.map) {
            if (estado.id == id) {
                knoutEstado = estado;
                return false;
            }
        }
    });
    return knoutEstado;
}

function retornarKnoutEstadoSigla(sigla, mapaKnout) {
    var knoutEstado;

    $.each(mapaKnout, function (i, estado) {
        if ((estado.type == types.map) && (estado.val() == sigla)) {
            knoutEstado = estado;
            return false;
        }
    });

    return knoutEstado;
}

function retornarUltimoEstadoInformado(estadosPassagem, origensDestinos) {
    var ultimoEstado;
    var estadoAnterior;

    if (estadosPassagem.length > 0) {
        ultimoEstado = estadosPassagem[estadosPassagem.length - 1];

        if (estadosPassagem.length == 1)
            estadoAnterior = { Sigla: origensDestinos[0].Origem };
        else
            estadoAnterior = estadosPassagem[estadosPassagem.length - 2];
    }
    else if (origensDestinos.length > 0)
        ultimoEstado = { Sigla: origensDestinos[0].Origem };
    else
        return undefined;

    return {
        ultimoEstado: ultimoEstado,
        estadoAnterior: estadoAnterior
    };
}

function validarPassagens(estadosPassagem, origensDestinos, estadoDestino, mapaKnout) {
    var mensagem = "";
    var valido = true;
    var origemDestinoAnterior;
    var estadoOrigem = origensDestinos[0].Origem;
    var origensProximaPosicao = new Array();
    var destinosPassagemPosicaoAnterior = new Array();

    var possuiPassagens = estadosPassagem.length > 0 ? true : false;

    if (possuiPassagens) {
        var ultimoEstadoPassagem = estadosPassagem[estadosPassagem.length - 1];
        var retorno = verificarEstadoOrigemOuDestino(ultimoEstadoPassagem.Sigla, origensDestinos);

        var knoutUltimoEstado = retornarKnoutEstadoSigla(ultimoEstadoPassagem.Sigla, mapaKnout);
        if (retorno.destino && ultimoEstadoPassagem.Sigla == estadoDestino) {
            valido = false;
            mensagem += Localization.Resources.Logistica.MapaBrasil.PorSerUmDestino + knoutUltimoEstado.text + Localization.Resources.Logistica.MapaBrasil.NaoPodeSerUltimoEstadoDePassagem;
        } else {
            var fazDivisaUltimo = false;
            $.each(origensDestinos, function (i, origemDestino) {
                if (fazDivisa(knoutUltimoEstado, origemDestino.Destino)) {
                    fazDivisaUltimo = true;
                    return false;
                }
            });
            if (!fazDivisaUltimo) {
                valido = false;
                mensagem += knoutUltimoEstado.text + Localization.Resources.Logistica.MapaBrasil.NaoFazDivisaComNenhumEstadoDeDestino;
            }
        }
    }

    if (valido) {
        $.each(origensDestinos, function (i, origemDestino) {
            origemDestino.Passagens = new Array();
            var knoutEstadoOrigem = retornarKnoutEstadoSigla(origemDestino.Origem, mapaKnout);
            var knoutEstadoDestino = retornarKnoutEstadoSigla(origemDestino.Destino, mapaKnout);
            if (possuiPassagens) {
                var origemPassagem = retornarEstadoExistePassagem(estadosPassagem, origemDestino.Origem);
                var posicaoProximaPassagem = 0;
                if (origemPassagem != null && i > 0) {
                    posicaoProximaPassagem = verificarOrigemProximaPosicao(origensProximaPosicao, origemDestino.Origem);
                    if (posicaoProximaPassagem == 0) {
                        posicaoProximaPassagem = origemPassagem.Posicao + 1;
                    }
                } else {
                    if (origensDestinos[0].Origem == origemDestino.Origem)
                        posicaoProximaPassagem = 1;
                    else {
                        if (knoutEstadoOrigem.text != knoutEstadoDestino.text)
                            mensagem += Localization.Resources.Logistica.MapaBrasil.EstadoDeOrigem + knoutEstadoOrigem.text + Localization.Resources.Logistica.MapaBrasil.PrecisaSerUmaPassagem;
                        else
                            posicaoProximaPassagem = 1;
                    }
                }

                origensProximaPosicao.push({ Sigla: origemDestino.Origem, ProximaPosicao: posicaoProximaPassagem });
                mensagem = validarProximaPassagemOrigem(posicaoProximaPassagem, estadosPassagem, origemDestino, knoutEstadoOrigem, knoutEstadoDestino, mensagem);
                if (mensagem != "")
                    valido = false;
                if (valido) {
                    var destinoPassagem = retornarEstadoExistePassagem(estadosPassagem, origemDestino.Destino);
                    var posicaoPassagemAnterior = 0;
                    if (destinoPassagem != null) {
                        if (origemDestino.Destino == estadoDestino) {
                            posicaoPassagemAnterior = estadosPassagem.length;
                        } else {
                            posicaoAnterior = verificarDestinoPosicaoAnterior(destinosPassagemPosicaoAnterior, origemDestino.Destino, mensagem);
                            if (posicaoAnterior == null) {

                                if (estadoOrigem == origemDestino.Origem && (origemDestino.Origem == origemDestino.Destino || fazDivisa(knoutEstadoOrigem, origemDestino.Destino)) && origensDestinos.length > 1) {
                                    posicaoPassagemAnterior = 0;
                                } else {
                                    if (estadosPassagem[destinoPassagem.Posicao + 1] != null) {
                                        posicaoPassagemAnterior = destinoPassagem.Posicao - 1;
                                    } else {
                                        if (estadosPassagem[destinoPassagem.Posicao] != null && origensDestinos.length - 1 == i) {
                                            posicaoPassagemAnterior = destinoPassagem.Posicao + 1;
                                        } else {
                                            posicaoPassagemAnterior = destinoPassagem.Posicao - 1;
                                        }
                                    }
                                }
                            } else {
                                posicaoPassagemAnterior = posicaoAnterior.PosicaoAnterior;
                            }

                            destinosPassagemPosicaoAnterior.push({ Sigla: origemDestino.Destino, PosicaoAnterior: posicaoPassagemAnterior });
                        }
                    } else {
                        posicaoPassagemAnterior = estadosPassagem.length;

                        if (estadoDestino != origemDestino.Destino) {
                            var possuiMesmoEstadoProximoDestino = false;
                            $.each(origensDestinos, function (j, origemDestinoProximoEstado) {
                                if (j > i) {
                                    if (origemDestinoProximoEstado.Destino == origemDestino.Destino) {
                                        possuiMesmoEstadoProximoDestino = true;
                                        return false;
                                    }
                                }
                            });
                            if (!possuiMesmoEstadoProximoDestino) {
                                if ((i > 0 && origemDestino.Destino != origemDestino.Origem) || origemDestino.Destino != origemDestino.Origem) {
                                    mensagem += Localization.Resources.Logistica.MapaBrasil.EstadoDeDestino + knoutEstadoDestino.text + Localization.Resources.Logistica.MapaBrasil.PrecisaSerUmaPassagem;
                                } else {
                                    posicaoPassagemAnterior = 0;
                                }
                            }
                        }
                        destinosPassagemPosicaoAnterior.push({ Sigla: origemDestino.Destino, PosicaoAnterior: posicaoPassagemAnterior });
                    }
                    mensagem = validarPassagemDestino(posicaoPassagemAnterior, estadosPassagem, origemDestino, knoutEstadoOrigem, knoutEstadoDestino, mensagem);
                    if (mensagem != "")
                        valido = false;
                }
                if (valido) {
                    for (i = posicaoProximaPassagem; i <= posicaoPassagemAnterior; i++) {
                        if (possuiMesmoEstadoProximoDestino == null || origemDestino.Destino != origemDestino.Origem)
                            origemDestino.Passagens.push(estadosPassagem[i - 1]);
                    }
                };

            } else {

                if (!fazDivisa(knoutEstadoOrigem, origemDestino.Destino) && origemDestino.Origem != origemDestino.Destino) {
                    valido = false;
                    mensagem += Localization.Resources.Logistica.MapaBrasil.EstadoDeOrigem + knoutEstadoOrigem.text + Localization.Resources.Logistica.MapaBrasil.NaoFazDivisaComSeuDestino + knoutEstadoDestino.text + Localization.Resources.Logistica.MapaBrasil.SendoAssimObrigatorioInformarUmPercursoEntreEles;
                }
            }
        });
    }

    if (!valido) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Logistica.MapaBrasil.PercursoInvalido, mensagem, 16000);
    }
    return valido;
}

function validarPassagemDestino(posicaoPassagemAnterior, estadosPassagem, origemDestino, knoutEstadoOrigem, knoutEstadoDestino, msg) {
    var mensagem = msg;
    var ultimoEstadoPassagem = estadosPassagem[posicaoPassagemAnterior - 1];
    if (ultimoEstadoPassagem != null) {
        if (!fazDivisa(knoutEstadoDestino, ultimoEstadoPassagem.Sigla) && origemDestino.Origem != origemDestino.Destino) {
            mensagem += Localization.Resources.Logistica.MapaBrasil.NaoFoiInformadoUmPercursoValidoAteEstadoDeDestino + knoutEstadoDestino.text + Localization.Resources.Logistica.MapaBrasil.PartindoDe + knoutEstadoOrigem.text + ". ";
        }
    } else {
        if (!fazDivisa(knoutEstadoDestino, origemDestino.Origem) && origemDestino.Origem != origemDestino.Destino) {
            mensagem += Localization.Resources.Logistica.MapaBrasil.NaoFoiInformadoUmPercursoValidoAteEstadoDeDestino + knoutEstadoDestino.text + Localization.Resources.Logistica.MapaBrasil.PartindoDe + knoutEstadoOrigem.text + ". ";
        }
    }
    return mensagem;
}

function validarProximaPassagemOrigem(posicaoProximaPassagem, estadosPassagem, origemDestino, knoutEstadoOrigem, knoutEstadoDestino, msg) {
    var proximaPassagem = estadosPassagem[posicaoProximaPassagem - 1];
    var mensagem = msg;
    if (proximaPassagem != null) {
        if (!fazDivisa(knoutEstadoOrigem, proximaPassagem.Sigla) && origemDestino.Origem != origemDestino.Destino) {
            mensagem += Localization.Resources.Logistica.MapaBrasil.NaoFoiInformadoUmPercursoValidoPartindoDoEstadoDeOrigem + knoutEstadoOrigem.text + Localization.Resources.Logistica.MapaBrasil.AteSeuDestino + knoutEstadoDestino.text + ". ";
        }
    } else {
        if (!fazDivisa(knoutEstadoOrigem, origemDestino.Destino) && origemDestino.Origem != origemDestino.Destino) {
            mensagem += Localization.Resources.Logistica.MapaBrasil.NaoFoiInformadoUmPercursoValidoPartindoDoEstadoDeOrigem + knoutEstadoOrigem.text + Localization.Resources.Logistica.MapaBrasil.AteSeuDestino + knoutEstadoDestino.text + " ";
        }
    }
    return mensagem;
}

function ValidarSegundaPassagem(mapaKnout, estadosPassagem, estado) {
    if (estadosPassagem.length > 0 && mapaKnout[estado].list.includes(estadosPassagem[estadosPassagem.length - 1].Sigla))
        return true;

    return false;
}

function verificarDestinoPosicaoAnterior(destinosPassagemPosicaoAnterior, destino) {
    var PassagemAnterior = null;
    $.each(destinosPassagemPosicaoAnterior, function (i, estadoDestino) {
        if (estadoDestino.Sigla == destino) {
            PassagemAnterior = estadoDestino;
            return false;
        }
    });
    return PassagemAnterior;
}

function verificarEstadoExistePassagem(estadosPassagem, estado) {
    var existe = false;
    $.each(estadosPassagem, function (i, estadoPassagem) {
        if (estado == estadoPassagem.Sigla) {
            existe = true;
            return false;
        }
    });
    return existe;
}

function verificarEstadoOrigemOuDestino(estado, origensDestinos) {
    var isOrigem = false;
    var isDestino = false;
    $.each(origensDestinos, function (i, origemDestino) {
        if (estado == origemDestino.Origem)
            isOrigem = true;

        if (estado == origemDestino.Destino)
            isDestino = true;

        if (isDestino && isOrigem)
            return false;
    });

    var retorno = {
        origem: isOrigem,
        destino: isDestino
    }

    return retorno;
}

function verificarOrigemProximaPosicao(origensProximaPosicao, origem) {
    var proximaPassagem = 0;
    $.each(origensProximaPosicao, function (i, estadoOrigem) {
        if (estadoOrigem.Sigla == origem) {
            proximaPassagem = estadoOrigem.ProximaPosicao;
            return false;
        }
    });
    return proximaPassagem;
}
