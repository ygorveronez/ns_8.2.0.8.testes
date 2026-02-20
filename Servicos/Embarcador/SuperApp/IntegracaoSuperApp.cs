using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.WebService.Rest.Webhook.SuperApp;
using ImageMagick;
using Infrastructure.Services.HttpClientFactory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace Servicos.Embarcador.SuperApp
{

    #region Classes auxiliares
    public class DadosEvidencias
    {
        public List<string> observacoes { get; set; } = new List<string>();
        public List<string> imagensCanhoto { get; set; } = new List<string>();
        public List<string> imagensEntrega { get; set; } = new List<string>();
        public List<int> pacotes { get; set; } = new List<int>();
        public List<string> nomes { get; set; } = new List<string>();
        public List<string> documentos { get; set; } = new List<string>();
        public List<string> assinaturas { get; set; } = new List<string>();
        public DateTime? dataRetroativa { get; set; } = null;
        public List<double> coordenadas { get; set; } = new List<double>();
        public string situacaoLocalizacao { get; set; } = string.Empty;
        public List<Dominio.ObjetosDeValor.WebService.Rest.Webhook.SuperApp.StoppingPointDocumentItem> stoppingPointDocumentItems { get; set; } = new List<Dominio.ObjetosDeValor.WebService.Rest.Webhook.SuperApp.StoppingPointDocumentItem>();
    }

    public class RetornoIntegracaoSuperApp
    {
        public bool Sucesso { get; set; } = false;
        public string Mensagem { get; set; } = string.Empty;
    }
    #endregion

    public class IntegracaoSuperApp
    {
        #region Atributos Privados
        private protected int _codigoEmpresaMultisoftware;
        private protected string _cpfMotorista;
        private protected DadosEvidencias _dadosEvidencias;
        private protected Repositorio.UnitOfWork _unitOfWork;
        private protected AdminMultisoftware.Repositorio.UnitOfWork _unitOfWorkAdmin;
        private protected AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente _clienteMultisoftware;
        private protected Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoTMS;
        private protected Dominio.Entidades.Embarcador.SuperApp.IntegracaoSuperApp _integracaoSuperApp;
        #endregion

        #region Construtores
        public IntegracaoSuperApp(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _unitOfWorkAdmin = unitOfWorkAdmin;
            _clienteMultisoftware = clienteMultisoftware;
            _configuracaoTMS = ObterConfiguracaoTMS();
        }
        public IntegracaoSuperApp(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _configuracaoTMS = ObterConfiguracaoTMS();
        }
        #endregion

        #region Métodos Públicos
        public void Iniciar()
        {
            ProcessarEventos();
        }
        #endregion

        #region Métodos Privados
        private void ProcessarEventos()
        {
            Servicos.Embarcador.SuperApp.Eventos.EventSubmit servicoEventSubmit = new Servicos.Embarcador.SuperApp.Eventos.EventSubmit(_unitOfWork, _unitOfWorkAdmin, _clienteMultisoftware);
            Servicos.Embarcador.SuperApp.Eventos.ChatSendMessage servicoChatSendMessage = new Servicos.Embarcador.SuperApp.Eventos.ChatSendMessage(_unitOfWork);

            Servicos.Embarcador.SuperApp.Eventos.OccurrenceCreate servicoOccurrenceCreate = new Servicos.Embarcador.SuperApp.Eventos.OccurrenceCreate(_unitOfWork, _unitOfWorkAdmin, _clienteMultisoftware);

            Repositorio.Embarcador.SuperApp.IntegracaoSuperApp repositorioIntegracaoSuperApp = new Repositorio.Embarcador.SuperApp.IntegracaoSuperApp(_unitOfWork);
            List<int> codEventosPendentes = repositorioIntegracaoSuperApp.BuscarIntegracoesPendentes(new List<TipoEventoApp> { TipoEventoApp.EventsSubmit }, 200);

            Servicos.Log.TratarErro("INICIO PROCESSAR EVENTOS", "IntegracaoSuperAPPEventos");

            foreach (int codEvento in codEventosPendentes)
            {
                RetornoIntegracaoSuperApp retornoIntegracaoSuperApp = new RetornoIntegracaoSuperApp();
                Dominio.Entidades.Embarcador.SuperApp.IntegracaoSuperApp eventoProcessar = repositorioIntegracaoSuperApp.BuscarPorCodigo(codEvento, false);

                eventoProcessar.NumeroTentativas++;
                repositorioIntegracaoSuperApp.Atualizar(eventoProcessar);

                _unitOfWork.Start();
                try
                {
                    switch (eventoProcessar.TipoEvento)
                    {
                        case TipoEventoApp.EventsSubmit:
                            servicoEventSubmit.ProcessarEvento(eventoProcessar, out retornoIntegracaoSuperApp);
                            break;

                    }

                    if (retornoIntegracaoSuperApp.Sucesso)
                        _unitOfWork.CommitChanges();
                    else
                        _unitOfWork.Rollback();

                    _unitOfWork.Start();
                    Dominio.Entidades.Embarcador.SuperApp.IntegracaoSuperApp eventonovo = repositorioIntegracaoSuperApp.BuscarPorCodigo(codEvento, false);

                    if (string.IsNullOrEmpty(retornoIntegracaoSuperApp.Mensagem))
                        eventonovo.DetalhesProcessamento = "Processado";
                    else if (retornoIntegracaoSuperApp.Mensagem.Length > 200)
                        eventonovo.DetalhesProcessamento = retornoIntegracaoSuperApp.Mensagem.Substring(0, 200);
                    else
                        eventonovo.DetalhesProcessamento = retornoIntegracaoSuperApp.Mensagem;

                    eventonovo.SituacaoProcessamento = retornoIntegracaoSuperApp.Sucesso ? SituacaoProcessamentoIntegracao.Processado : SituacaoProcessamentoIntegracao.ErroProcessamento;
                    repositorioIntegracaoSuperApp.Atualizar(eventonovo);

                    _unitOfWork.CommitChanges();
                }
                catch (Exception ex)
                {

                    Servicos.Log.TratarErro(ex, "IntegracaoSuperAPPEventos");

                    _unitOfWork.Rollback();

                    _unitOfWork.Start();
                    Dominio.Entidades.Embarcador.SuperApp.IntegracaoSuperApp eventonovo = repositorioIntegracaoSuperApp.BuscarPorCodigo(codEvento, false);

                    retornoIntegracaoSuperApp.Mensagem = ex.Message;

                    eventonovo.DetalhesProcessamento = "Ocorreu uma falha genérica ao processar o evento.";
                    eventonovo.SituacaoProcessamento = SituacaoProcessamentoIntegracao.ErroProcessamento;

                    repositorioIntegracaoSuperApp.Atualizar(eventonovo);

                    _unitOfWork.CommitChanges();
                }
            }

            Servicos.Log.TratarErro("FIM PROCESSAR EVENTOS", "IntegracaoSuperAPPEventos");
        }

        #endregion

        #region Métodos Privados Protegidos

        private protected string obterJsonRequisicao(Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao arquivoIntegracao)
        {
            return Servicos.Embarcador.Integracao.ArquivoIntegracao.RetornarArquivoTexto(arquivoIntegracao);
        }

        private protected Dominio.Entidades.Usuario ObterUsuario(string cpf)
        {
            Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(_unitOfWork);
            return repositorioUsuario.BuscarPorCPF(cpf);
        }

        private protected Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado ObterAuditado(Dominio.Entidades.Usuario motorista)
        {
            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado
            {
                TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Usuario,
                OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServiceMonitoriamento,
                Usuario = motorista,
            };

            return auditado;
        }

        private protected Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ObterConfiguracaoTMS()
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            return repConfiguracaoTMS.BuscarConfiguracaoPadrao();
        }

        private protected Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint ObterWaypointEvento(Dominio.ObjetosDeValor.WebService.Rest.Webhook.SuperApp.Location location)
        {
            Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint waypoint = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint();
            if (location != null && (location.Coordinates?.Count ?? 0) > 1)
            {
                waypoint.Latitude = location.Coordinates[1];
                waypoint.Longitude = location.Coordinates[0];
            }
            return waypoint;
        }

        private protected void ObterDadosEvidencias(List<Evidence> evidencias)
        {
            _dadosEvidencias = new DadosEvidencias();
            List<string> tiposImagensCanhotos = new() { TipoEvidenciaSuperApp.FotoCanhotoComValidacao.ToString(), TipoEvidenciaSuperApp.FotoCanhotoSemValidacao.ToString() };
            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoSuperApp));

            string enumerador;
            foreach (Evidence evidencia in evidencias)
            {
                switch (evidencia.Type)
                {
                    case "PHOTO":
                        foreach (string url in evidencia.Values)
                        {
                            try
                            {
                                System.Threading.Tasks.Task<byte[]> response = client.GetByteArrayAsync(url);
                                response.Wait(60000);
                                if (response.IsCompleted)
                                {
                                    if (tiposImagensCanhotos.Contains(evidencia.ExternalId))
                                        _dadosEvidencias.imagensCanhoto.Add(Servicos.Imagem.ConverterWebpParaJpg(System.Convert.ToBase64String(response.Result)));
                                    else
                                        _dadosEvidencias.imagensEntrega.Add(Servicos.Imagem.ConverterWebpParaJpg(System.Convert.ToBase64String(response.Result)));
                                }
                            }
                            catch (Exception ex)
                            {
                                Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao processar imagem SuperApp - continuando processamento: {ex.ToString()}", "CatchNoAction");
                            } //Se não conseguiu ler a imagem, vida que segue.
                        }
                        break;

                    case "TEXT":
                        foreach (string observacao in evidencia.Values)
                        {
                            enumerador = evidencia.ExternalId;
                            if (enumerador == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEvidenciaSuperApp.NomeRecebedorConfirmacao.ToString())
                                _dadosEvidencias.nomes.Add(observacao);
                            else
                                _dadosEvidencias.observacoes.Add(observacao);
                        }
                        break;

                    case "NUMBER":
                        foreach (string dado in evidencia.Values)
                        {
                            enumerador = evidencia.ExternalId;
                            if (enumerador == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEvidenciaSuperApp.DocumentoConfirmacao.ToString())
                                _dadosEvidencias.documentos.Add(dado);
                            else
                                _dadosEvidencias.pacotes.Add(dado.ToInt(0));
                        }
                        break;

                    case "SIGNATURE":
                        foreach (string url in evidencia.Values)
                        {
                            try
                            {
                                System.Threading.Tasks.Task<byte[]> response = client.GetByteArrayAsync(url);
                                response.Wait(60000);
                                if (response.IsCompleted)
                                    _dadosEvidencias.assinaturas.Add(System.Convert.ToBase64String(response.Result));
                            }
                            catch (Exception ex)
                            {
                                Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao processar assinatura SuperApp - continuando processamento: {ex.ToString()}", "CatchNoAction");
                            } //Se não conseguiu ler a imagem, vida que segue.
                        }
                        break;
                }
            }
        }

        private protected void ObterDadosChecklistFlow(Dominio.ObjetosDeValor.WebService.Rest.Webhook.SuperApp.Evento.Response evidencias)
        {
            if (evidencias == null) return;

            _dadosEvidencias = new DadosEvidencias();
            List<TipoEvidenciaSuperApp> tiposImagensCanhotos = new() { TipoEvidenciaSuperApp.FotoCanhotoComValidacao, TipoEvidenciaSuperApp.FotoCanhotoSemValidacao };

            foreach (Dominio.ObjetosDeValor.WebService.Rest.Webhook.SuperApp.Evento.Step evidencia in evidencias.Steps)
            {
                TipoEvidenciaSuperApp tipoEvidencia = default;

                if (evidencia.ExternalInfo?.Tags?.Count > 0 &&
                    Enum.TryParse(evidencia.ExternalInfo.Tags[0], out TipoEvidenciaSuperApp temp))
                {
                    tipoEvidencia = temp;
                }

                switch (evidencia.Type)
                {
                    case "IMAGE":
                    case "IMAGE_VALIDATOR":
                        if (!string.IsNullOrEmpty(evidencia.ResponseString))
                        {
                            if (evidencia.ResponseListString == null)
                                evidencia.ResponseListString = new List<string>();
                            evidencia.ResponseListString.Add(evidencia.ResponseString);
                        }
                        try
                        {
                            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoSuperApp));
                            client.Timeout = TimeSpan.FromMinutes(1);
                            // IMAGE e IMAGE_VALIDATOR sempre é um array de strings 
                            foreach (string valor in evidencia.ResponseListString)
                            {
                                byte[] response = client.GetByteArrayAsync(valor).Result;
                                if (response.Length > 0)
                                {
                                    string imagemBase64 = Servicos.Imagem.ConverterWebpParaJpg(System.Convert.ToBase64String(response));
                                    if (tiposImagensCanhotos.Contains(tipoEvidencia))
                                        _dadosEvidencias.imagensCanhoto.Add(imagemBase64);
                                    else
                                        _dadosEvidencias.imagensEntrega.Add(imagemBase64);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao processar evidência de imagem SuperApp - continuando processamento: {ex.ToString()}", "CatchNoAction");
                        } //Se não conseguiu ler a imagem, vida que segue.
                        break;

                    case "TEXT":
                        if (!string.IsNullOrEmpty(evidencia.ResponseString))
                        {
                            if (tipoEvidencia == TipoEvidenciaSuperApp.NomeRecebedorConfirmacao)
                                _dadosEvidencias.nomes.Add(evidencia.ResponseString);
                            else
                                _dadosEvidencias.observacoes.Add(evidencia.ResponseString);
                        }
                        break;

                    case "NUMBER":
                        if (!string.IsNullOrEmpty(evidencia.ResponseString))
                        {
                            if (tipoEvidencia == TipoEvidenciaSuperApp.DocumentoConfirmacao)
                                _dadosEvidencias.documentos.Add(evidencia.ResponseString);
                            else if (tipoEvidencia == TipoEvidenciaSuperApp.SolicitacaoPacotesColeta)
                                _dadosEvidencias.pacotes.Add(evidencia.ResponseString.ToInt(0));
                        }
                        break;
                    case "SIGNATURE":
                        if (!string.IsNullOrEmpty(evidencia.ResponseString))
                            try
                            {
                                HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoSuperApp));
                                client.Timeout = TimeSpan.FromMinutes(1);
                                byte[] response = client.GetByteArrayAsync(evidencia.ResponseString).Result;
                                if (response.Length > 0)
                                    _dadosEvidencias.assinaturas.Add(Servicos.Imagem.ConverterWebpParaJpg(System.Convert.ToBase64String(response)));
                            }
                            catch (Exception ex)
                            {
                                Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao processar assinatura SuperApp ChecklistFlow - continuando processamento: {ex.ToString()}", "CatchNoAction");
                            } //Se não conseguiu ler a imagem, vida que segue.
                        break;
                    case "DATE":
                    case "DATE_TIME":
                        if (tipoEvidencia == TipoEvidenciaSuperApp.DataRetroativa && !string.IsNullOrEmpty(evidencia.ResponseString))
                        {
                            // Converte a string para DateTime (UTC)
                            DateTime dataUtc = DateTime.Parse(evidencia.ResponseString, null, System.Globalization.DateTimeStyles.AdjustToUniversal);
                            // Converte para hora local
                            _dadosEvidencias.dataRetroativa = dataUtc.ToLocalTime();
                        }
                        break;
                    case "LOCATION":
                        if (evidencia.ResponseLocation != null)
                        {
                            if (!string.IsNullOrEmpty(evidencia.ResponseLocation.image))
                            {
                                try
                                {
                                    HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoSuperApp));
                                    client.Timeout = TimeSpan.FromMinutes(1);
                                    byte[] response = client.GetByteArrayAsync(evidencia.ResponseLocation.image).Result;
                                    if (response.Length > 0)
                                        _dadosEvidencias.imagensEntrega.Add(Servicos.Imagem.ConverterWebpParaJpg(System.Convert.ToBase64String(response)));
                                }
                                catch (Exception ex)
                                {
                                    Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao processar imagem de localização SuperApp - continuando processamento: {ex.ToString()}", "CatchNoAction");
                                } //Se não conseguiu ler a imagem, vida que segue.
                            }
                            _dadosEvidencias.situacaoLocalizacao = evidencia.ResponseLocation.situation;
                            if (evidencia.ResponseLocation.location?.Coordinates.Count > 0)
                            {
                                _dadosEvidencias.coordenadas = evidencia.ResponseLocation.location.Coordinates;
                            }
                        }
                        break;
                }
            }
        }
        private protected void AdicionarImagensChamado(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, List<string> imagens, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Chamados.ChamadoAnexo repChamadoAnexo = new Repositorio.Embarcador.Chamados.ChamadoAnexo(unitOfWork);

            string extensao = ".jpg";

            foreach (string imagem in imagens)
            {
                // Salva imagem no disco
                byte[] buffer = System.Convert.FromBase64String(imagem);
                MemoryStream ms = new MemoryStream(buffer);
                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.ArmazenarArquivoFisico(ms, Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "Chamados" }), out string tokenImagem);

                // Salva no banco
                string caminhoChamado = Utilidades.IO.FileStorageService.Storage.Combine(Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "Chamados" }), tokenImagem + extensao);

                if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoChamado))
                {
                    Dominio.Entidades.Embarcador.Chamados.ChamadoAnexo chamadoAnexo = new Dominio.Entidades.Embarcador.Chamados.ChamadoAnexo()
                    {
                        Chamado = chamado,
                        Descricao = string.Empty,
                        GuidArquivo = tokenImagem,
                        NomeArquivo = tokenImagem + extensao,
                    };

                    repChamadoAnexo.Inserir(chamadoAnexo, auditado);
                    if (auditado != null)
                        Servicos.Auditoria.Auditoria.Auditar(auditado, chamado, $"Enviou o anexo {tokenImagem + extensao} na data {DateTime.Now.ToDateTimeString()}", unitOfWork);
                }
            }
        }
        private protected void AtualizarCargaIntegracaoSuperApp(Dominio.Entidades.Embarcador.SuperApp.IntegracaoSuperApp integracaoSuperApp, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega)
        {
            if (carga != null) integracaoSuperApp.Carga = carga;
            if (cargaEntrega != null) integracaoSuperApp.CargaEntrega = cargaEntrega;
        }

        private protected string ProcessarImagemCanhoto(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, Repositorio.Embarcador.Canhotos.Canhoto repCanhoto, object evento, DadosEvidencias dadosEvidencias = null)
        {
            Repositorio.Usuario repositorioMotorista = new Repositorio.Usuario(_unitOfWork);
            Servicos.Embarcador.Canhotos.Canhoto serCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(_unitOfWork);

            if (canhoto.SituacaoDigitalizacaoCanhoto == SituacaoDigitalizacaoCanhoto.Digitalizado || canhoto.SituacaoDigitalizacaoCanhoto == SituacaoDigitalizacaoCanhoto.AgAprovocao)
                return "Canhoto já foi digitalizado.";

            if (_dadosEvidencias.imagensCanhoto.Count == 0) return "Integração de Canhoto sem imagem para liberação.";

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoComprovei configuracaoIntegracaoComprovei = new Repositorio.Embarcador.Configuracoes.IntegracaoComprovei(_unitOfWork).BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto(_unitOfWork).BuscarConfiguracaoPadrao();

            Dominio.Entidades.Usuario motorista = repositorioMotorista.BuscarMotoristaPorCPF(_cpfMotorista);
            canhoto.Observacao = _dadosEvidencias.observacoes.Count > 0 ? _dadosEvidencias.observacoes[0] : string.Empty;
            canhoto.QuantidadeEnvioDigitalizacaoCanhoto = (canhoto.QuantidadeEnvioDigitalizacaoCanhoto > 0 ? canhoto.QuantidadeEnvioDigitalizacaoCanhoto + 1 : 1);
            canhoto.OrigemSituacaoDigitalizacaoCanhoto = null;

            DateTime dataCanhoto;

            if (evento is EventoDriverReceiptCreate)
                dataCanhoto = ((EventoDriverReceiptCreate)evento)?.Data?.Response?.CompletedAt ?? DateTime.Now;
            else
                dataCanhoto = ((EventoDeliveryReceiptCreate)evento)?.Data?.SendAt ?? DateTime.Now;

            canhoto.DataEnvioCanhoto = DateTime.Now;
            canhoto.DataUltimaModificacao = DateTime.Now;
            canhoto.DataDigitalizacao = dataCanhoto;
            canhoto.DataEntregaNotaCliente = dadosEvidencias?.dataRetroativa != null ? dadosEvidencias.dataRetroativa : canhoto.DataDigitalizacao;

            if (!canhoto.DataEntregaNotaCliente.HasValue)
            {
                canhoto.DataEntregaNotaCliente = canhoto.DataEnvioCanhoto;

                if (canhoto.Carga?.TipoOperacao?.ConfiguracaoMobile?.ReplicarDataDigitalizacaoCanhotoDataEntregaCliente ?? false)
                    canhoto.DataEntregaNotaCliente = canhoto.DataDigitalizacao;
            }

            canhoto.UsuarioDigitalizacao = motorista;
            canhoto.GuidNomeArquivo = Guid.NewGuid().ToString().Replace("-", "");

            string caminhoCanhoto = Servicos.Embarcador.Canhotos.Canhoto.CaminhoCanhoto(canhoto, _unitOfWork);
            string extensao;
            string fileLocation;

            if (_dadosEvidencias.imagensCanhoto.Count > 1)
            {
                extensao = ".pdf";
                fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(caminhoCanhoto, canhoto.GuidNomeArquivo + extensao);
                Servicos.PDF.ConvertBase64ImagesToPdf(_dadosEvidencias.imagensCanhoto, fileLocation);
            }
            else
            {
                extensao = ".jpg";
                fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(caminhoCanhoto, canhoto.GuidNomeArquivo + extensao);

                byte[] data = Convert.FromBase64String(_dadosEvidencias.imagensCanhoto[0]);
                using (MagickImage image = new MagickImage(data))
                {
                    image.Format = MagickFormat.Jpeg;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        image.Write(ms);
                        ms.Position = 0;
                        using (System.Drawing.Image sysImage = System.Drawing.Image.FromStream(ms))
                        {
                            using (System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(sysImage))
                            {
                                Utilidades.IO.FileStorageService.Storage.SaveImage(fileLocation, bitmap, System.Drawing.Imaging.ImageFormat.Jpeg);
                            }
                        }
                    }
                }
            }

            canhoto.NomeArquivo = canhoto.Numero.ToString() + extensao;
            bool trizyChecked = false;
            if (evento is EventoDeliveryReceiptCreate)
                trizyChecked = ((EventoDeliveryReceiptCreate)evento).Data.Checked;
            else if (evento is EventoDriverReceiptCreate)
                trizyChecked = ((EventoDriverReceiptCreate)evento).Data.Response.Steps.FirstOrDefault(step => step.Type == "IMAGE_VALIDATOR")?.Checked ?? false;

            canhoto.OrigemDigitalizacao = trizyChecked ? CanhotoOrigemDigitalizacao.MobileSemValidacaoAut : CanhotoOrigemDigitalizacao.Mobile;
            string tipoDigitalizacao = "SuperApp Trizy" + (trizyChecked ? " sem validação automática" : "");

            serCanhoto.GerarHistoricoCanhoto(canhoto, motorista, $"Imagem do Canhoto digitalizada via {tipoDigitalizacao} com o nome {canhoto.GuidNomeArquivo}", _unitOfWork);

            if (!Utilidades.IO.FileStorageService.Storage.Exists(fileLocation))
            {
                serCanhoto.GerarHistoricoCanhoto(canhoto, motorista, $"Falha ao salvar imagem do Canhoto", _unitOfWork);
                Servicos.Log.TratarErro($"Falha ao salvar imagem do Canhoto de id {canhoto.Codigo} e guid {canhoto.GuidNomeArquivo}.", "EnviarCanhoto");
            }

            bool possuiIntegracaoComprovei = (configuracaoIntegracaoComprovei?.PossuiIntegracaoIACanhoto ?? false) &&
                (configuracaoCanhoto.IntegrarCanhotosComValidadorIAComprovei) &&
                !(canhoto.XMLNotaFiscal?.Destinatario?.ClienteDescargas?.FirstOrDefault()?.PossuiCanhotoDeDuasOuMaisPaginas ?? false) &&
                !(configuracaoCanhoto.NaoIntegrarIAComproveiCanhotosDeNotasDevolvidas &&
                canhoto.XMLNotaFiscal?.SituacaoEntregaNotaFiscal == SituacaoNotaFiscal.Devolvida);

            if (!_configuracaoTMS.ExigeAprovacaoDigitalizacaoCanhoto && (possuiIntegracaoComprovei || trizyChecked))
            {
                canhoto.SituacaoDigitalizacaoCanhoto = SituacaoDigitalizacaoCanhoto.Digitalizado;
                canhoto.DigitalizacaoIntegrada = false;
                canhoto.ValidacaoViaOCR = true;

                Servicos.Embarcador.Canhotos.Canhoto.CanhotoLiberado(canhoto, _configuracaoTMS, _unitOfWork, TipoServicoMultisoftware.MultiMobile, _clienteMultisoftware, false);
                Servicos.Embarcador.Canhotos.CanhotoIntegracao.GerarIntegracaoDigitalizacaoCanhoto(canhoto, _configuracaoTMS, TipoServicoMultisoftware.MultiMobile, _clienteMultisoftware, _unitOfWork);
                Servicos.Embarcador.Canhotos.Canhoto.FinalizarDigitalizacaoCanhoto(canhoto, _unitOfWork, TipoServicoMultisoftware.MultiMobile);
            }
            else
            {
                canhoto.SituacaoDigitalizacaoCanhoto = SituacaoDigitalizacaoCanhoto.AgAprovocao;
                Servicos.Embarcador.Canhotos.Canhoto.CanhotoAgAprovacao(canhoto, _configuracaoTMS, _unitOfWork);
            }

            canhoto.SituacaoPgtoCanhoto = SituacaoPgtoCanhoto.Pendente;
            canhoto.UsuarioDigitalizacao = motorista;

            Servicos.Auditoria.Auditoria.Auditar(ObterAuditado(motorista), canhoto, null, "Enviou imagem do canhoto", _unitOfWork);
            repCanhoto.Atualizar(canhoto);

            int codigoNF = canhoto.TipoCanhoto == TipoCanhoto.Avulso ? canhoto.CanhotoAvulso.PedidosXMLNotasFiscais.FirstOrDefault().XMLNotaFiscal.Codigo : canhoto.XMLNotaFiscal.Codigo;
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntregaNotaFiscal.BuscarPorCargaENFe(canhoto.Carga.Codigo, codigoNF)?.CargaEntrega ?? null;

            AtualizarCargaIntegracaoSuperApp(_integracaoSuperApp, canhoto.Carga, cargaEntrega);

            return string.Empty;
        }

        private protected void ProcessarDadosCargaEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, object eventoReceiptCreate, bool processouCanhoto, bool validacaoObrigatoriedades)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.DadosRecebedor repDadosRecebedor = new Repositorio.Embarcador.Cargas.ControleEntrega.DadosRecebedor(_unitOfWork);
            bool atualizarDadosCarga = processouCanhoto;
            string mensagemFalha = string.Empty;
            Location location = new();
            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.DadosRecebedor dadosRecebedor = cargaEntrega.DadosRecebedor ?? new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.DadosRecebedor();

            if (eventoReceiptCreate is EventoDeliveryReceiptCreate)
            {
                ObterDadosEvidencias(((EventoDeliveryReceiptCreate)eventoReceiptCreate).Data.Evidences);
                location = ((EventoDeliveryReceiptCreate)eventoReceiptCreate).Data.Location;
            }
            else if (eventoReceiptCreate is EventoDriverReceiptCreate)
            {
                ObterDadosChecklistFlow(((EventoDriverReceiptCreate)eventoReceiptCreate).Data.Response);
                location = ((EventoDriverReceiptCreate)eventoReceiptCreate).Data.Location;
            }

            if (_dadosEvidencias.imagensEntrega.Count > 0 || (!processouCanhoto && _dadosEvidencias.imagensCanhoto.Count > 0))
            {
                if (!processouCanhoto && _dadosEvidencias.imagensCanhoto.Count > 0)
                    _dadosEvidencias.imagensEntrega.AddRange(_dadosEvidencias.imagensCanhoto);

                Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPoint = ObterWaypointEvento(location);

                try
                {
                    string guidImagem;
                    string extensao;
                    if (_dadosEvidencias.imagensEntrega.Count > 1)
                    {
                        guidImagem = Guid.NewGuid().ToString().Replace("-", "");
                        extensao = ".pdf";
                        string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(_unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "EntregaPedido" }), guidImagem + extensao);
                        Servicos.PDF.ConvertBase64ImagesToPdf(_dadosEvidencias.imagensEntrega, fileLocation);
                    }
                    else
                    {
                        extensao = ".jpg";
                        byte[] data = System.Convert.FromBase64String(_dadosEvidencias.imagensEntrega[0]);
                        MemoryStream ms = new MemoryStream(data);
                        Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SalvarImagem(ms, _unitOfWork, out guidImagem);
                    }

                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SalvarImagemEntrega(_codigoEmpresaMultisoftware, cargaEntrega.Codigo, guidImagem, _unitOfWork, DateTime.Now, wayPoint.Latitude, wayPoint.Longitude, OrigemSituacaoEntrega.App, extensao: extensao);
                    atualizarDadosCarga = true;

                    if (cargaEntrega.Coleta)
                    {
                        int raio = Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.ObterRaioEntrega(cargaEntrega.Cliente, _unitOfWork);
                        bool dentroDoRaio = Servicos.Embarcador.Logistica.Monitoramento.Monitoramento.ValidarEmRaioOuAreaCliente(cargaEntrega.Cliente, wayPoint.Latitude, wayPoint.Longitude, raio);
                        Servicos.Embarcador.SuperApp.IntegracaoNotificacaoApp servicoIntegracaoNotificacaoApp = new(_unitOfWork);

                        //Notificação de foto dentro ou fora do raio do fornecedor/cliente.
                        servicoIntegracaoNotificacaoApp.GerarIntegracaoNotificacao(cargaEntrega.Carga, dentroDoRaio ? TipoNotificacaoApp.MotoristaDentroDoRaioDoFornecedor : TipoNotificacaoApp.MotoristaForaDoRaioDoFornecedor);

                        //Notificação de carregamento em avaliação.
                        servicoIntegracaoNotificacaoApp.GerarIntegracaoNotificacao(cargaEntrega.Carga, TipoNotificacaoApp.CarregamentoEmAvaliacaoPelaLogistica);
                    }
                }
                catch (ServicoException ex)
                {
                    mensagemFalha = ex.Message;
                }
            }

            if (_dadosEvidencias.pacotes.Count > 0)
            {
                cargaEntrega.QuantidadePacotesColetados = _dadosEvidencias.pacotes.Sum();
                repositorioCargaEntrega.Atualizar(cargaEntrega);
                atualizarDadosCarga = true;
            }

            if (_dadosEvidencias.nomes.Count > 0 || _dadosEvidencias.documentos.Count > 0)
            {
                if (_dadosEvidencias.nomes.Count > 0 && !string.IsNullOrEmpty(_dadosEvidencias.nomes[0]))
                {
                    dadosRecebedor.Nome = _dadosEvidencias.nomes[0].PadRight(60).Substring(0, 60);
                    atualizarDadosCarga = true;
                }

                if (_dadosEvidencias.documentos.Count > 0 && !string.IsNullOrEmpty(_dadosEvidencias.documentos[0]))
                {
                    dadosRecebedor.CPF = _dadosEvidencias.documentos[0].PadRight(11).Substring(0, 11);
                    atualizarDadosCarga = true;
                }

                if (atualizarDadosCarga)
                {
                    if (dadosRecebedor.DataEntrega == DateTime.MinValue)
                        dadosRecebedor.DataEntrega = DateTime.Now;

                    if (dadosRecebedor.Codigo > 0)
                        repDadosRecebedor.Atualizar(dadosRecebedor);
                    else
                        repDadosRecebedor.Inserir(dadosRecebedor);

                    cargaEntrega.DadosRecebedor = dadosRecebedor;
                    repositorioCargaEntrega.Atualizar(cargaEntrega);
                }
            }

            if (_dadosEvidencias.assinaturas.Count > 0)
            {
                byte[] buffer = System.Convert.FromBase64String(_dadosEvidencias.assinaturas[0]);
                using (var image = new MagickImage(buffer))
                {
                    image.Format = MagickFormat.Jpeg;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        image.Write(ms);
                        ms.Position = 0;
                        Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SalvarImagemAssinatura(ms, _unitOfWork, out string guidAssinatura);
                        Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SalvarAssinaturaProdutorColetaEntrega(_clienteMultisoftware.Codigo, cargaEntrega.Codigo, guidAssinatura, DateTime.Now, _unitOfWork, out mensagemFalha);
                        atualizarDadosCarga = true;
                    }
                }
            }

            if (!string.IsNullOrEmpty(mensagemFalha))
                throw new ServicoException($"{mensagemFalha} (Qtd Imagens: {_dadosEvidencias.imagensCanhoto.Count + _dadosEvidencias.imagensEntrega.Count} | Qtd Pacotes: {_dadosEvidencias.pacotes.Count} | Qtd Assinatura: {_dadosEvidencias.assinaturas.Count})");
            else if (atualizarDadosCarga)
                AtualizarCargaIntegracaoSuperApp(_integracaoSuperApp, cargaEntrega.Carga, cargaEntrega);
            else if (validacaoObrigatoriedades)
                ValidarObrigatoriedadeDeEvidencias(cargaEntrega, _dadosEvidencias);
        }

        private void ValidarObrigatoriedadeDeEvidencias(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, DadosEvidencias dadosEvidencias)
        {
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais = repNotaFiscal.BuscarXMLNotaFiscalPorCargaPedido(cargaEntrega.Pedidos?.Select(e => e.CargaPedido.Codigo).ToList() ?? new List<int>());

            int quantidadeCanhotos = (cargaEntrega.Cliente?.ClienteDescargas?.FirstOrDefault()?.PossuiCanhotoDeDuasOuMaisPaginas ?? false) ? (cargaEntrega.Cliente?.ClienteDescargas?.FirstOrDefault()?.QuantidadeDePaginasDoCanhoto ?? 1) : 1;
            bool naoEnviarTagValidacao = (cargaEntrega.Carga.TipoOperacao?.ConfiguracaoTrizy?.NaoEnviarTagValidacao ?? false) || (notasFiscais == null || notasFiscais.Count == 0) || quantidadeCanhotos > 1;
            bool exigirInformarNumeroPacotesNaColeta = cargaEntrega.Coleta && (cargaEntrega.Carga.TipoOperacao?.ConfiguracaoControleEntrega?.ExigirInformarNumeroPacotesNaColetaTrizy ?? false);
            bool solicitarComprovanteColetaEntrega = (cargaEntrega.Carga.TipoOperacao?.ConfiguracaoTrizy?.SolicitarComprovanteColetaEntrega ?? false);
            bool solicitarAssinaturaNaConfirmacaoDeColetaEntrega = !cargaEntrega.Coleta && (cargaEntrega.Carga.TipoOperacao?.ConfiguracaoTrizy?.SolicitarAssinaturaNaConfirmacaoDeColetaEntrega ?? false);
            bool SolicitarFotoComoEvidenciaObrigatoria = (cargaEntrega.Carga.TipoOperacao?.ConfiguracaoTrizy?.SolicitarFotoComoEvidenciaObrigatoria ?? false);
            bool enviarNomeRecebedorConfirmacao = cargaEntrega.Carga.TipoOperacao?.ConfiguracaoTrizy?.SolicitarNomeRecebedorNaConfirmacaoDeColetaEntrega ?? false;
            bool enviarDocumentoConfirmacao = cargaEntrega.Carga.TipoOperacao?.ConfiguracaoTrizy?.SolicitarDocumentoNaConfirmacaoDeColetaEntrega ?? false;

            if (((solicitarComprovanteColetaEntrega && !naoEnviarTagValidacao) || SolicitarFotoComoEvidenciaObrigatoria) && dadosEvidencias.imagensCanhoto.Count == 0)
                throw new ServicoException($"Comprovante de {cargaEntrega.TipoCargaEntrega} sem dados.");

            else if (solicitarAssinaturaNaConfirmacaoDeColetaEntrega && dadosEvidencias.assinaturas.Count == 0)
                throw new ServicoException($"Sem dados de Assinatura do Recebedor da {cargaEntrega.TipoCargaEntrega}.");

            else if (exigirInformarNumeroPacotesNaColeta && dadosEvidencias.pacotes.Count == 0)
                throw new ServicoException($"Sem informação de Quantidade de Pacotes Coletados.");

            else if (enviarNomeRecebedorConfirmacao && dadosEvidencias.nomes.Count == 0)
                throw new ServicoException($"Sem informação do Nome do Recebedor da {cargaEntrega.TipoCargaEntrega}.");

            else if (enviarDocumentoConfirmacao && dadosEvidencias.documentos.Count == 0)
                throw new ServicoException($"Sem informação do Documento do Recebedor da {cargaEntrega.TipoCargaEntrega}.");
        }

        #endregion
    }


}
