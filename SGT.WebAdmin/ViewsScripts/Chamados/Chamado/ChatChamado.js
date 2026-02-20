/*****************************************************************
 * Variáveis globais
 *****************************************************************/
var _knoutChat;          // Instância Knockout do chat
var _codigoCargaAtual;   // Carga atual para filtrar/armazenar chat
var _chatNovoMobile;     // Instância do Bootstrap Modal
var _ListaMensagemChatNovo = new Array(); // Lista global de mensagens

/*****************************************************************
 * Construtor da ViewModel do Chat
 *****************************************************************/
var ChatModal = function () {
    var self = this;

    this.NovaMensagem = PropertyEntity({
        type: types.text,
        val: ko.observable("")
    });

    this.NovaMensagemAnexoNome = PropertyEntity({
        type: types.text,
        val: ko.observable("")
    })

    this.NovaMensagemAnexoFile = PropertyEntity({
        type: types.file,
        val: ko.observable(null)
    });

    this.AdicionarAnexo = PropertyEntity({
        eventClick: adicionarAnexo,
        type: types.event,
        text: "Anexo",
        visible: ko.observable(true),
        idGrid: guid()
    });

    this.MarcarComoNaoLido = PropertyEntity({
        eventClick: marcarNaoLido,
        type: types.event
    });

    this.Atualizar = PropertyEntity({
        eventClick: atualizarChat,
        type: types.event,
        idGrid: guid()
    });

    this.EnviarMensagem = PropertyEntity({
        eventClick: enviarMensagem,
        type: types.event,
        idGrid: guid()
    });

    this.RemoverAnexo = PropertyEntity({
        eventClick: function () {
            self.NovaMensagemAnexoFile.val(null);
            self.NovaMensagemAnexoNome.val("");

            var fileInput = document.getElementById("fileAnexo");
            if (fileInput) {
                fileInput.value = "";
            }
        },
        type: types.event
    });

    this.OnFileChange = function (data, event) {
        var input = event.target;
        if (!input.files || input.files.length === 0) {
            return;
        }

        var file = input.files[0];
        // Armazena apenas o nome do arquivo selecionado
        self.NovaMensagemAnexoNome.val(file.name);
        self.NovaMensagemAnexoFile.val(file);

    };
};

/*****************************************************************
 * Funções associadas
 *****************************************************************/

// Botão "Anexo": apenas abre a caixa de seleção de arquivo
function adicionarAnexo() {
    var fileInput = document.getElementById("fileAnexo");

    if (fileInput) {
        fileInput.click();
    }
}

function marcarNaoLido() {
    MarcarMensagemComoNaoLidoClick()
}

function atualizarChat() {
    if (_codigoCargaAtual > 0) {
        obterHistoricoChatNovo(_codigoCargaAtual);
    }
}

function enviarMensagemComArquivo(dados, formData) {
    enviarArquivo(
        "ControleEntrega/EnviarMensagemChat",
        dados,
        formData,
        function (retorno) {
            if (retorno.Success) {
                var msg = retorno.Data;

                _ListaMensagemChatNovo.push({
                    usuario: msg.remetente,
                    codigoRemetente: msg.codigoRemetente,
                    texto: msg.mensagem,
                    anexo: msg.anexo || "",
                    dataHora: msg.dataMensagem
                });
                renderizarMensagens();

                setTimeout(function () {
                    var c = document.getElementById("chatScrollContainer");
                    if (c) c.scrollTop = c.scrollHeight;
                }, 50);

                _knoutChat.NovaMensagem.val("");
                _knoutChat.NovaMensagemAnexoFile(null);
                _knoutChat.NovaMensagemAnexoNome("");
                var fileInput = document.getElementById("fileAnexo");
                if (fileInput) fileInput.value = "";
            }
            else {
                exibirMensagem(tipoMensagem.falha, "Erro", retorno.Msg);
            }
        },
        function (err) { // callback de erro
            exibirMensagem(tipoMensagem.falha, "Erro",
                "Falha ao enviar mensagem (HTTP status: " + err.status + ")");
        }
    );
}

function enviarMensagemSemArquivo(dados) {
    executarReST("ControleEntrega/EnviarMensagemChat", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                _ListaMensagemChatNovo.push({
                    usuario: arg.Data.remetente,
                    codigoRemetente: arg.Data.codigoRemetente,
                    texto: arg.Data.mensagem,
                    anexo: arg.Data.anexo || "",
                    dataHora: arg.Data.dataMensagem
                });
                renderizarMensagens();

                setTimeout(function () {
                    var c = document.getElementById("chatScrollContainer");
                    if (c) c.scrollTop = c.scrollHeight;
                }, 50);

                _knoutChat.NovaMensagem.val("");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        }
    })
}

/*****************************************************************
 * Envia mensagem com ou sem anexo
 *****************************************************************/
function enviarMensagem(e, sender) {
    var texto = _knoutChat.NovaMensagem.val();
    if (!texto || !_codigoCargaAtual) return;

    var dados = {
        Carga: _codigoCargaAtual,
        Mensagem: texto
    };

    if (this.AdicionarAnexo.visible()) {
        var formData = new FormData();
        var arquivo = _knoutChat.NovaMensagemAnexoFile.val();

        if (arquivo) {
            formData.append("Arquivo", arquivo);
        }

        enviarMensagemComArquivo(dados, formData);
    } else {
        enviarMensagemSemArquivo(dados);
    }
}


/*****************************************************************
 * Buscar histórico no back e carregar no chat
 *****************************************************************/
function obterHistoricoChatNovo(codigoCarga) {
    _ListaMensagemChatNovo = [];
    renderizarMensagens(); 

    executarReST("ControleEntrega/ObterHistoricoMensagemChatMobile", { Carga: codigoCarga },
        function (arg) {
            if (arg.Success) {
                var msgs = arg.Data;

                _ListaMensagemChatNovo = arg.Data.map(m => ({
                    usuario: m.remetente,
                    codigoRemetente: m.codigoRemetente,
                    texto: m.mensagem,
                    anexo: m.anexo || "",
                    dataHora: m.dataMensagem
                }));
                renderizarMensagens();

                setTimeout(function () {
                    var container = document.getElementById("chatScrollContainer");
                    if (container) {
                        container.scrollTop = container.scrollHeight;
                    }
                }, 100);

            } else {
                exibirMensagem(tipoMensagem.falha, "Erro", arg.Msg);
            }
        },
        function (err) {
            exibirMensagem(tipoMensagem.falha, "Erro", "Ocorreu uma falha ao buscar histórico.");
        }
    );
}

/*****************************************************************
 * Exemplo de "abrir" a modal do chat
 *****************************************************************/
function loadChatModal(codigoCarga, permiteAnexo) {
    if (!_knoutChat) {
        _knoutChat = new ChatModal();
    }

    var modalElement = document.getElementById("KnoutChatModal");
    ko.cleanNode(modalElement);
    KoBindings(_knoutChat, "KnoutChatModal");

    _codigoCargaAtual = codigoCarga;
    if (codigoCarga) {
        obterHistoricoChatNovo(codigoCarga);
        MarcarMensagensComoLidoChatNovo(codigoCarga);
    }
    _knoutChat.AdicionarAnexo.visible(permiteAnexo);

    var modalElem = document.getElementById("divModalChatMotorista");
    _chatNovoMobile = new bootstrap.Modal(modalElem, { backdrop: 'static' });
    _chatNovoMobile.show();

}



function MarcarMensagemComoNaoLidoClick(e, sender) {
    var mensagem = { Carga: _codigoCargaAtual }
    executarReST("ControleEntrega/MarcarTodasComoNaoLido", mensagem, function (arg) {
        if (arg.Success) {
            _analise.ChatMotorista.notificationCount(arg.Data);
            _chatNovoMobile.hide()
        }
        else
            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
    });
}

function MarcarMensagensComoLidoChatNovo(codigoCarga) {

    executarReST("ControleEntrega/MarcarTodasComoLido", { Carga: codigoCarga }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
            }
        }
    });

}

function LoadConexaoSignalRChatNovo() {
    SignalRChatNotificarMensagemUsuarioEvent = AtualizarNotificarMensagemUsuario;
}

function AtualizarNotificarMensagemUsuario(retorno) {

    if (!_ListaMensagemChatNovo) return;

    var existe = _ListaMensagemChatNovo.some(m =>
        m.texto === retorno.msg &&
        m.dataHora === retorno.dataHoraEnvio &&
        m.anexo === (retorno.anexo || ""));

    if (!existe) {
        _ListaMensagemChatNovo.push({
            usuario: retorno.quemEnviou,
            codigoRemetente: retorno.codigoRemetente,
            texto: retorno.msg,
            anexo: retorno.anexo || "",
            dataHora: retorno.dataHoraEnvio
        });
    }

    renderizarMensagens();

    let boxChat = document.getElementById("chatScrollContainer");
    if (boxChat) {
        setTimeout(function () {
            boxChat.scrollTop = boxChat.scrollHeight;
        }, 100);
    }

    if (!modalEstaAberta()) {
        var countAtual = _analise.ChatMotorista.notificationCount();
        _analise.ChatMotorista.notificationCount(countAtual + 1);
    }
}

function modalEstaAberta() {
    var modal = document.getElementById("divModalChatMotorista");
    return modal && modal.classList.contains("show");
}

function renderizarMensagens() {
    var container = document.getElementById("chatMessagesContainer");
    container.innerHTML = "";

    _ListaMensagemChatNovo.forEach(m => {
        var li = document.createElement("li");
        li.className = m.codigoRemetente == _IDUsuario ? "balao-me" : "balao-outro";
        li.innerHTML = `
            <div class="balao-container">
                <div><strong>${m.usuario}</strong>:</div>
                <div style="margin:6px 0;">${m.texto}</div>
                ${m.anexo ? `<div><span style="font-size:0.85em; font-weight:bold;">ANEXOS:</span> ${m.anexo}</div>` : ""}
                <div style="margin-top:4px; font-size:0.85em; text-align:right;">${m.dataHora}</div>
            </div>`;
        container.appendChild(li);
    });
    setTimeout(() => { container.scrollTop = container.scrollHeight; }, 100);
}