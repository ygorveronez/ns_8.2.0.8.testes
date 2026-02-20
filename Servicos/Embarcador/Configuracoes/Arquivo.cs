using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;

namespace Servicos.Embarcador.Configuracoes
{
    public class Arquivo
    {
        public void AjustarConfiguracoes(Repositorio.UnitOfWork unitOfWork, bool ambienteHomologacao = false)
        {
            SalvarConfiguracoesArquivos(unitOfWork);
            SalvarConfiguracoesAmbiente(unitOfWork, ambienteHomologacao);

            ConfigurationInstance.GetInstance(unitOfWork).AtualizarConfiguracoes(unitOfWork);
        }

        public void AjustarConfiguracoesMonitoramento(Repositorio.UnitOfWork unitOfWork)
        {
            SalvarConfiguracoesMonitorarThreads(unitOfWork);
            SalvarConfiguracoesMonitorarPosicoes(unitOfWork);
            SalvarConfiguracoesProcessarMonitoramentos(unitOfWork);
            SalvarConfiguracoesProcessarTrocaAlvo(unitOfWork);
            SalvarConfiguracoesProcessarEventos(unitOfWork);
            SalvarConfiguracoesProcessarEventosSinal(unitOfWork);
            SalvarConfiguracoesEnviarNotificacoesAlertas(unitOfWork);
            SalvarConfiguracoesImportarPosicoesPendentesIntegracao(unitOfWork);

            ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).AtualizarConfiguracoes(unitOfWork);
        }

        private void SalvarConfiguracoesMonitorarThreads(Repositorio.UnitOfWork unitOfWork)
        {
            var configSettings = ConfigurationManager.GetSection("MonitorarThreads") as NameValueCollection;

            if (configSettings != null)
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramentoMonitorarThreads repConfiguracaoMonitorarThreads = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramentoMonitorarThreads(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramentoMonitorarThreads configuracaoMonitorarThreads = repConfiguracaoMonitorarThreads.BuscarPrimeiroRegistro();

                if (configuracaoMonitorarThreads == null)
                {
                    configuracaoMonitorarThreads = new Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramentoMonitorarThreads();
                    configuracaoMonitorarThreads.Ativo = bool.Parse(configSettings["Enable"]);
                    configuracaoMonitorarThreads.TempoSleepThread = Int16.Parse(configSettings["TempoSleepThread"]);

                    repConfiguracaoMonitorarThreads.Inserir(configuracaoMonitorarThreads);
                }
            }
        }

        private void SalvarConfiguracoesMonitorarPosicoes(Repositorio.UnitOfWork unitOfWork)
        {
            var configSettings = ConfigurationManager.GetSection("MonitorarPosicoes") as NameValueCollection;

            if (configSettings != null)
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramentoMonitorarPosicoes repConfiguracaoMonitorarPosicoes = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramentoMonitorarPosicoes(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramentoMonitorarPosicoes configuracaoMonitorarPosicoes = repConfiguracaoMonitorarPosicoes.BuscarPrimeiroRegistro();

                if (configuracaoMonitorarPosicoes == null)
                {
                    configuracaoMonitorarPosicoes = new Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramentoMonitorarPosicoes();
                    configuracaoMonitorarPosicoes.Ativo = bool.Parse(configSettings["Enable"]);
                    configuracaoMonitorarPosicoes.TempoSleepThread = Int16.Parse(configSettings["TempoSleepThread"]);
                    configuracaoMonitorarPosicoes.LimiteRegistros = Int16.Parse(configSettings["LimiteRegistros"]);
                    configuracaoMonitorarPosicoes.MinutosClientesCache = Int16.Parse(configSettings["MinutosClientesCache"]);
                    configuracaoMonitorarPosicoes.HorasPosicoesCache = Int16.Parse(configSettings["HorasPosicoesCache"]);
                    configuracaoMonitorarPosicoes.HorasParaExpirarPosicoes = Int16.Parse(configSettings["HorasParaExpirarPosicoes"]);
                    configuracaoMonitorarPosicoes.ClientesSecundarios = configSettings["ClientesSecundarios"] != null && bool.Parse(configSettings["ClientesSecundarios"]);
                    configuracaoMonitorarPosicoes.ConfirmarValidadeDaPosicao = bool.Parse(configSettings["ConfirmarValidadeDaPosicao"]);
                    configuracaoMonitorarPosicoes.RegistrarPosicaoInvalida = bool.Parse(configSettings["RegistrarPosicaoInvalida"]);
                    configuracaoMonitorarPosicoes.ExcluirPosicaoInvalida = bool.Parse(configSettings["ExcluirPosicaoInvalida"]);
                    configuracaoMonitorarPosicoes.GeolocalizacaoApenasJuridico = configSettings["GeolocalizacaoApenasJuridico"] != null && bool.Parse(configSettings["GeolocalizacaoApenasJuridico"]);
                    configuracaoMonitorarPosicoes.DistanciaAlvoMetros = configSettings["DistanciaAlvoMetros"] != null ? int.Parse(configSettings["DistanciaAlvoMetros"]) : 0;
                    configuracaoMonitorarPosicoes.ProcessarPosicoesDemaisPlacas = configSettings["ProcessarPosicoesDemaisPlacas"] != null && bool.Parse(configSettings["ProcessarPosicoesDemaisPlacas"]);
                    configuracaoMonitorarPosicoes.ArquivoNivelLog = configSettings["ArquivoNivelLog"];
                    configuracaoMonitorarPosicoes.EspelharPosicaoReboque = configSettings["EspelharPosicaoReboque"] != null && bool.Parse(configSettings["EspelharPosicaoReboque"]);
                    repConfiguracaoMonitorarPosicoes.Inserir(configuracaoMonitorarPosicoes);
                }
            }
        }

        private void SalvarConfiguracoesProcessarMonitoramentos(Repositorio.UnitOfWork unitOfWork)
        {
            var configSettings = ConfigurationManager.GetSection("ProcessarMonitoramentos") as NameValueCollection;

            if (configSettings != null)
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramentoProcessarMonitoramentos repConfiguracaoProcessarMonitoramentos = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramentoProcessarMonitoramentos(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramentoProcessarMonitoramentos configuracaoProcessarMonitoramentos = repConfiguracaoProcessarMonitoramentos.BuscarPrimeiroRegistro();

                if (configuracaoProcessarMonitoramentos == null)
                {
                    configuracaoProcessarMonitoramentos = new Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramentoProcessarMonitoramentos();
                    configuracaoProcessarMonitoramentos.Ativo = bool.Parse(configSettings["Enable"]);
                    configuracaoProcessarMonitoramentos.TempoSleepThread = Int16.Parse(configSettings["TempoSleepThread"]);
                    configuracaoProcessarMonitoramentos.LimiteRegistros = Int16.Parse(configSettings["LimiteRegistros"]);
                    configuracaoProcessarMonitoramentos.CodigoIntegracaoViaTansporteMaritima = configSettings["CodigoIntegracaoViaTansporteMaritima"].Trim();
                    configuracaoProcessarMonitoramentos.EnviarNotificacao = configSettings["EnviarNotificaoMSMQ"] != null && bool.Parse(configSettings["EnviarNotificaoMSMQ"]);
                    configuracaoProcessarMonitoramentos.RegrasTransito = configSettings["RegrasTransito"] == null || bool.Parse(configSettings["RegrasTransito"]);
                    configuracaoProcessarMonitoramentos.IntervaloCalculoPrevisaoCargaEntregaMinutos = Int16.Parse(configSettings["IntervaloCalculoPrevisaoCargaEntregaMinutos"]);
                    configuracaoProcessarMonitoramentos.PossuiColetaContainer = configSettings["PossuiColetaContainer"] == null || bool.Parse(configSettings["PossuiColetaContainer"]);
                    configuracaoProcessarMonitoramentos.ArquivoNivelLog = configSettings["ArquivoNivelLog"];

                    repConfiguracaoProcessarMonitoramentos.Inserir(configuracaoProcessarMonitoramentos);
                }
            }
        }

        private void SalvarConfiguracoesProcessarTrocaAlvo(Repositorio.UnitOfWork unitOfWork)
        {
            var configSettings = ConfigurationManager.GetSection("ProcessarTrocaAlvo") as NameValueCollection;

            if (configSettings != null)
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramentoProcessarTrocaAlvo repConfiguracaoProcessarTrocaAlvo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramentoProcessarTrocaAlvo(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramentoProcessarTrocaAlvo configuracaoProcessarTrocaAlvo = repConfiguracaoProcessarTrocaAlvo.BuscarPrimeiroRegistro();

                if (configuracaoProcessarTrocaAlvo == null)
                {
                    configuracaoProcessarTrocaAlvo = new Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramentoProcessarTrocaAlvo();
                    configuracaoProcessarTrocaAlvo.Ativo = bool.Parse(configSettings["Enable"]);
                    configuracaoProcessarTrocaAlvo.TempoSleepThread = Int16.Parse(configSettings["TempoSleepThread"]);
                    configuracaoProcessarTrocaAlvo.LimiteRegistros = Int16.Parse(configSettings["LimiteRegistros"]);
                    configuracaoProcessarTrocaAlvo.DiretorioFila = configSettings["DiretorioFila"];
                    configuracaoProcessarTrocaAlvo.ArquivoFilaPrefixo = configSettings["ArquivoFilaPrefixo"];
                    configuracaoProcessarTrocaAlvo.ArquivoNivelLog = configSettings["ArquivoNivelLog"];
                    configuracaoProcessarTrocaAlvo.LimiteFilaArquivos = configSettings["LimiteFilaArquivos"] != null ? Int16.Parse(configSettings["LimiteFilaArquivos"]) : 150;
                    configuracaoProcessarTrocaAlvo.GeolocalizacaoApenasJuridico = configSettings["GeolocalizacaoApenasJuridico"] != null && bool.Parse(configSettings["GeolocalizacaoApenasJuridico"]);
                    configuracaoProcessarTrocaAlvo.GerarPermanenciaLocais = configSettings["GerarPermanenciaLocais"] != null && bool.Parse(configSettings["GerarPermanenciaLocais"]);

                    repConfiguracaoProcessarTrocaAlvo.Inserir(configuracaoProcessarTrocaAlvo);
                }
            }
        }

        private void SalvarConfiguracoesProcessarEventos(Repositorio.UnitOfWork unitOfWork)
        {
            var configSettings = ConfigurationManager.GetSection("ProcessarEventos") as NameValueCollection;

            if (configSettings != null)
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramentoProcessarEventos repConfiguracaoProcessarEventos = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramentoProcessarEventos(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramentoProcessarEventos configuracaoProcessarEventos = repConfiguracaoProcessarEventos.BuscarPrimeiroRegistro();

                if (configuracaoProcessarEventos == null)
                {
                    configuracaoProcessarEventos = new Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramentoProcessarEventos();
                    configuracaoProcessarEventos.Ativo = bool.Parse(configSettings["Enable"]);
                    configuracaoProcessarEventos.TempoSleepThread = Int16.Parse(configSettings["TempoSleepThread"]);
                    configuracaoProcessarEventos.LimiteRegistros = Int16.Parse(configSettings["LimiteRegistros"]);
                    configuracaoProcessarEventos.LimiteDiasRetroativos = configSettings["LimiteDiasRetroativos"] != null ? Int16.Parse(configSettings["LimiteDiasRetroativos"]) : 5;
                    configuracaoProcessarEventos.FiltrarMinutos = configSettings["FiltrarMinutos"] != null ? Int16.Parse(configSettings["FiltrarMinutos"]) : 2;
                    configuracaoProcessarEventos.DiretorioFila = configSettings["DiretorioFila"];
                    configuracaoProcessarEventos.ArquivoFilaPrefixo = configSettings["ArquivoFilaPrefixo"];
                    configuracaoProcessarEventos.ArquivoNivelLog = configSettings["ArquivoNivelLog"];
                    configuracaoProcessarEventos.LimiteFilaArquivos = configSettings["LimiteFilaArquivos"] != null ? Int16.Parse(configSettings["LimiteFilaArquivos"]) : 150;

                    repConfiguracaoProcessarEventos.Inserir(configuracaoProcessarEventos);
                }
            }
        }

        private void SalvarConfiguracoesProcessarEventosSinal(Repositorio.UnitOfWork unitOfWork)
        {
            var configSettings = ConfigurationManager.GetSection("ProcessarEventosSinal") as NameValueCollection;

            if (configSettings != null)
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramentoProcessarEventosSinal repConfiguracaoProcessarEventosSinal = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramentoProcessarEventosSinal(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramentoProcessarEventosSinal configuracaoProcessarEventosSinal = repConfiguracaoProcessarEventosSinal.BuscarPrimeiroRegistro();

                if (configuracaoProcessarEventosSinal == null)
                {
                    configuracaoProcessarEventosSinal = new Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramentoProcessarEventosSinal();
                    configuracaoProcessarEventosSinal.Ativo = bool.Parse(configSettings["Enable"]);
                    configuracaoProcessarEventosSinal.TempoSleepThread = Int16.Parse(configSettings["TempoSleepThread"]);
                    configuracaoProcessarEventosSinal.LimiteRegistros = Int16.Parse(configSettings["LimiteRegistros"]);
                    configuracaoProcessarEventosSinal.ArquivoNivelLog = configSettings["ArquivoNivelLog"];

                    repConfiguracaoProcessarEventosSinal.Inserir(configuracaoProcessarEventosSinal);
                }
            }
        }

        private void SalvarConfiguracoesEnviarNotificacoesAlertas(Repositorio.UnitOfWork unitOfWork)
        {
            var configSettings = ConfigurationManager.GetSection("EnviarNotificacoesAlertas") as NameValueCollection;

            if (configSettings != null)
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramentoEnviarNotificacoesAlerta repConfiguracaoEnviarNotificacoesAlerta = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramentoEnviarNotificacoesAlerta(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramentoEnviarNotificacoesAlerta configuracaoEnviarNotificacoesAlerta = repConfiguracaoEnviarNotificacoesAlerta.BuscarPrimeiroRegistro();

                if (configuracaoEnviarNotificacoesAlerta == null)
                {
                    configuracaoEnviarNotificacoesAlerta = new Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramentoEnviarNotificacoesAlerta();
                    configuracaoEnviarNotificacoesAlerta.Ativo = bool.Parse(configSettings["Enable"]);
                    configuracaoEnviarNotificacoesAlerta.TempoSleepThread = Int16.Parse(configSettings["TempoSleepThread"]);
                    configuracaoEnviarNotificacoesAlerta.LimiteRegistros = Int16.Parse(configSettings["LimiteRegistros"]);
                    configuracaoEnviarNotificacoesAlerta.ArquivoNivelLog = configSettings["ArquivoNivelLog"];

                    repConfiguracaoEnviarNotificacoesAlerta.Inserir(configuracaoEnviarNotificacoesAlerta);
                }
            }
        }

        private void SalvarConfiguracoesImportarPosicoesPendentesIntegracao(Repositorio.UnitOfWork unitOfWork)
        {
            var configSettings = ConfigurationManager.GetSection("ImportarPosicoesPendentesIntegracao") as NameValueCollection;

            if (configSettings != null)
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramentoImportarPosicoesPendenteIntegracao repConfiguracaoImportarPosicoesPendenteIntegracao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramentoImportarPosicoesPendenteIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramentoImportarPosicoesPendenteIntegracao configuracaoImportarPosicoesPendenteIntegracao = repConfiguracaoImportarPosicoesPendenteIntegracao.BuscarPrimeiroRegistro();

                if (configuracaoImportarPosicoesPendenteIntegracao == null)
                {
                    configuracaoImportarPosicoesPendenteIntegracao = new Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramentoImportarPosicoesPendenteIntegracao();
                    configuracaoImportarPosicoesPendenteIntegracao.Ativo = bool.Parse(configSettings["Enable"]);
                    configuracaoImportarPosicoesPendenteIntegracao.TempoSleepThread = Int16.Parse(configSettings["TempoSleepThread"]);
                    configuracaoImportarPosicoesPendenteIntegracao.LimiteRegistros = Int16.Parse(configSettings["LimiteRegistros"]);
                    configuracaoImportarPosicoesPendenteIntegracao.ArquivoNivelLog = configSettings["ArquivoNivelLog"];

                    repConfiguracaoImportarPosicoesPendenteIntegracao.Inserir(configuracaoImportarPosicoesPendenteIntegracao);
                }
            }
        }

        private void SalvarConfiguracoesArquivos(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo repConfiguracaoArquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = repConfiguracaoArquivo.BuscarPrimeiroRegistro();

            if (configuracaoArquivo == null)
                configuracaoArquivo = new Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo();

            if (string.IsNullOrWhiteSpace(configuracaoArquivo.Anexos) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["Anexos"]))
                configuracaoArquivo.Anexos = ConfigurationManager.AppSettings["Anexos"];

            if (string.IsNullOrWhiteSpace(configuracaoArquivo.CaminhoArquivos) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["CaminhoArquivos"]))
                configuracaoArquivo.CaminhoArquivos = ConfigurationManager.AppSettings["CaminhoArquivos"];

            if (string.IsNullOrWhiteSpace(configuracaoArquivo.CaminhoArquivosEmpresas) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["CaminhoArquivosEmpresas"]))
                configuracaoArquivo.CaminhoArquivosEmpresas = ConfigurationManager.AppSettings["CaminhoArquivosEmpresas"];

            if (string.IsNullOrWhiteSpace(configuracaoArquivo.CaminhoArquivosImportacaoBoleto) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["CaminhoArquivosImportacaoBoleto"]))
                configuracaoArquivo.CaminhoArquivosImportacaoBoleto = ConfigurationManager.AppSettings["CaminhoArquivosImportacaoBoleto"];

            if (string.IsNullOrWhiteSpace(configuracaoArquivo.CaminhoArquivosIntegracao) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["CaminhoArquivosIntegracao"]))
                configuracaoArquivo.CaminhoArquivosIntegracao = ConfigurationManager.AppSettings["CaminhoArquivosIntegracao"];

            if (string.IsNullOrWhiteSpace(configuracaoArquivo.CaminhoArquivosIntegracaoEDI) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["CaminhoArquivosIntegracaoEDI"]))
                configuracaoArquivo.CaminhoArquivosIntegracaoEDI = ConfigurationManager.AppSettings["CaminhoArquivosIntegracaoEDI"];

            if (string.IsNullOrWhiteSpace(configuracaoArquivo.CaminhoCanhotos) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["CaminhoCanhotos"]))
                configuracaoArquivo.CaminhoCanhotos = ConfigurationManager.AppSettings["CaminhoCanhotos"];

            if (string.IsNullOrWhiteSpace(configuracaoArquivo.CaminhoCanhotosAvulsos) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["CaminhoCanhotosAvulsos"]))
                configuracaoArquivo.CaminhoCanhotosAvulsos = ConfigurationManager.AppSettings["CaminhoCanhotosAvulsos"];

            if (string.IsNullOrWhiteSpace(configuracaoArquivo.CaminhoDocumentosFiscaisEmbarcador) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["CaminhoDocumentosFiscaisEmbarcador"]))
                configuracaoArquivo.CaminhoDocumentosFiscaisEmbarcador = ConfigurationManager.AppSettings["CaminhoDocumentosFiscaisEmbarcador"];

            if (string.IsNullOrWhiteSpace(configuracaoArquivo.CaminhoGeradorRelatorios) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["CaminhoGeradorRelatorios"]))
                configuracaoArquivo.CaminhoGeradorRelatorios = ConfigurationManager.AppSettings["CaminhoGeradorRelatorios"];

            if (string.IsNullOrWhiteSpace(configuracaoArquivo.CaminhoLogoEmbarcador) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["CaminhoLogoEmbarcador"]))
                configuracaoArquivo.CaminhoLogoEmbarcador = ConfigurationManager.AppSettings["CaminhoLogoEmbarcador"];

            if (string.IsNullOrWhiteSpace(configuracaoArquivo.CaminhoOcorrencias) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["CaminhoOcorrencias"]))
                configuracaoArquivo.CaminhoOcorrencias = ConfigurationManager.AppSettings["CaminhoOcorrencias"];

            if (string.IsNullOrWhiteSpace(configuracaoArquivo.CaminhoOcorrenciasMobiles) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["CaminhoOcorrenciasMobiles"]))
                configuracaoArquivo.CaminhoOcorrenciasMobiles = ConfigurationManager.AppSettings["CaminhoOcorrenciasMobiles"];

            if (string.IsNullOrWhiteSpace(configuracaoArquivo.CaminhoRelatorios) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["CaminhoRelatorios"]))
                configuracaoArquivo.CaminhoRelatorios = ConfigurationManager.AppSettings["CaminhoRelatorios"];

            if (string.IsNullOrWhiteSpace(configuracaoArquivo.CaminhoRelatoriosCrystal) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["CaminhoRelatoriosCrystal"]))
                configuracaoArquivo.CaminhoRelatoriosCrystal = ConfigurationManager.AppSettings["CaminhoRelatoriosCrystal"];

            if (string.IsNullOrWhiteSpace(configuracaoArquivo.CaminhoRelatoriosEmbarcador) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["CaminhoRelatoriosEmbarcador"]))
                configuracaoArquivo.CaminhoRelatoriosEmbarcador = ConfigurationManager.AppSettings["CaminhoRelatoriosEmbarcador"];

            if (string.IsNullOrWhiteSpace(configuracaoArquivo.CaminhoRetornoXMLIntegrador) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["CaminhoRetornoXMLIntegrador"]))
                configuracaoArquivo.CaminhoRetornoXMLIntegrador = ConfigurationManager.AppSettings["CaminhoRetornoXMLIntegrador"];

            if (string.IsNullOrWhiteSpace(configuracaoArquivo.CaminhoTempArquivosImportacao) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["CaminhoTempArquivosImportacao"]))
                configuracaoArquivo.CaminhoTempArquivosImportacao = ConfigurationManager.AppSettings["CaminhoTempArquivosImportacao"];

            if (string.IsNullOrWhiteSpace(configuracaoArquivo.CaminhoXMLNotaFiscalComprovanteEntrega) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["CaminhoXMLNotaFiscalComprovanteEntrega"]))
                configuracaoArquivo.CaminhoXMLNotaFiscalComprovanteEntrega = ConfigurationManager.AppSettings["CaminhoXMLNotaFiscalComprovanteEntrega"];

            if (string.IsNullOrWhiteSpace(configuracaoArquivo.CaminhoDestinoXML) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["CaminhoDestinoXML"]))
                configuracaoArquivo.CaminhoDestinoXML = ConfigurationManager.AppSettings["CaminhoDestinoXML"];

            if (string.IsNullOrWhiteSpace(configuracaoArquivo.CaminhoCanhotosAntigos) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["CaminhoCanhotosAntigos"]))
                configuracaoArquivo.CaminhoCanhotosAntigos = ConfigurationManager.AppSettings["CaminhoCanhotosAntigos"];

            if (string.IsNullOrWhiteSpace(configuracaoArquivo.CaminhoRaiz) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["CaminhoRaiz"]))
                configuracaoArquivo.CaminhoRaiz = ConfigurationManager.AppSettings["CaminhoRaiz"];

            if (string.IsNullOrWhiteSpace(configuracaoArquivo.CaminhoGuia) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["CaminhoGuia"]))
                configuracaoArquivo.CaminhoGuia = ConfigurationManager.AppSettings["CaminhoGuia"];

            if (string.IsNullOrWhiteSpace(configuracaoArquivo.CaminhoDanfeSMS) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["CaminhoDanfeSMS"]))
                configuracaoArquivo.CaminhoGuia = ConfigurationManager.AppSettings["CaminhoDanfeSMS"];

            if (string.IsNullOrWhiteSpace(configuracaoArquivo.CaminhoRaizFTP) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["CaminhoRaizFTP"]))
                configuracaoArquivo.CaminhoRaizFTP = ConfigurationManager.AppSettings["CaminhoRaizFTP"];

            if (string.IsNullOrWhiteSpace(configuracaoArquivo.CaminhoDocumentosINPUT) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["CaminhoDocumentosIMPUT"]))
                configuracaoArquivo.CaminhoDocumentosINPUT = ConfigurationManager.AppSettings["CaminhoDocumentosIMPUT"];

            if (string.IsNullOrWhiteSpace(configuracaoArquivo.CaminhoDocumentosOUTPUT) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["CaminhoDocumentosOUTPUT"]))
                configuracaoArquivo.CaminhoDocumentosOUTPUT = ConfigurationManager.AppSettings["CaminhoDocumentosOUTPUT"];

            if (configuracaoArquivo.Codigo > 0)
                repConfiguracaoArquivo.Atualizar(configuracaoArquivo);
            else
                repConfiguracaoArquivo.Inserir(configuracaoArquivo);

        }

        private void SalvarConfiguracoesAmbiente(Repositorio.UnitOfWork unitOfWork, bool ambienteHomologacao)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoAmbiente repConfiguracaoAmbiente = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAmbiente(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAmbiente configuracaoAmbiente = repConfiguracaoAmbiente.BuscarPrimeiroRegistro();

            if (configuracaoAmbiente == null)
            {
                configuracaoAmbiente = new Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAmbiente();
                configuracaoAmbiente.AmbienteSeguro = true;
                configuracaoAmbiente.AmbienteProducao = !ambienteHomologacao;
            }

            if (string.IsNullOrWhiteSpace(configuracaoAmbiente.IdentificacaoAmbiente) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["IdentificacaoAmbiente"]))
                configuracaoAmbiente.IdentificacaoAmbiente = ConfigurationManager.AppSettings["IdentificacaoAmbiente"];

            if (string.IsNullOrWhiteSpace(configuracaoAmbiente.CodigoLocalidadeNaoCadastrada) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["CodigoLocalidadeNaoCadastrada"]))
                configuracaoAmbiente.CodigoLocalidadeNaoCadastrada = ConfigurationManager.AppSettings["CodigoLocalidadeNaoCadastrada"];

            if (!configuracaoAmbiente.RecalcularICMSNaEmissaoCTe.HasValue)
                configuracaoAmbiente.RecalcularICMSNaEmissaoCTe = ConfigurationManager.AppSettings["RecalcularICMSNaEmissaoCTe"] == "SIM";

            if (!configuracaoAmbiente.AplicarValorICMSNoComplemento.HasValue)
                configuracaoAmbiente.AplicarValorICMSNoComplemento = ConfigurationManager.AppSettings["AplicarValorICMSNoComplemento"] == "SIM";

            if (!configuracaoAmbiente.AdicionarCTesFilaConsulta.HasValue)
                configuracaoAmbiente.AdicionarCTesFilaConsulta = ConfigurationManager.AppSettings["AdicionarCTesFilaConsulta"] == "SIM";

            if (!configuracaoAmbiente.NaoCalcularDIFALParaCSTNaoTributavel.HasValue)
                configuracaoAmbiente.NaoCalcularDIFALParaCSTNaoTributavel = ConfigurationManager.AppSettings["NaoCalcularDIFALParaCSTNaoTributavel"] == "SIM";

            if (!configuracaoAmbiente.NaoUtilizarColetaNaBuscaRotaFrete.HasValue)
                configuracaoAmbiente.NaoUtilizarColetaNaBuscaRotaFrete = ConfigurationManager.AppSettings["NaoUtilizarColetaNaBuscaRotaFrete"] == "SIM";

            if (string.IsNullOrWhiteSpace(configuracaoAmbiente.CodificacaoEDI) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["CodificacaoEDI"]))
                configuracaoAmbiente.CodificacaoEDI = ConfigurationManager.AppSettings["CodificacaoEDI"];

            if (string.IsNullOrWhiteSpace(configuracaoAmbiente.LinkCotacaoCompra) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["LinkCotacaoCompra"]))
                configuracaoAmbiente.LinkCotacaoCompra = ConfigurationManager.AppSettings["LinkCotacaoCompra"];

            if (string.IsNullOrWhiteSpace(configuracaoAmbiente.LogoPersonalizadaFornecedor) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["LogoPersonalizadaFornecedor"]))
                configuracaoAmbiente.LogoPersonalizadaFornecedor = ConfigurationManager.AppSettings["LogoPersonalizadaFornecedor"];

            if (string.IsNullOrWhiteSpace(configuracaoAmbiente.LayoutPersonalizadoFornecedor) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["LayoutPersonalizadoFornecedor"]))
                configuracaoAmbiente.LayoutPersonalizadoFornecedor = ConfigurationManager.AppSettings["LayoutPersonalizadoFornecedor"];

            if (!configuracaoAmbiente.OcultarConteudoColog.HasValue)
                configuracaoAmbiente.OcultarConteudoColog = ConfigurationManager.AppSettings["OcultarConteudoColog"] == "true";

            if (!configuracaoAmbiente.ConsultarPeloCustoDaRota.HasValue)
                configuracaoAmbiente.ConsultarPeloCustoDaRota = ConfigurationManager.AppSettings["ConsultarPeloCustoDaRota"] == "true";

            if (string.IsNullOrWhiteSpace(configuracaoAmbiente.ConcessionariasComDescontos) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["ConcessionariasCoConcessionariasComDescontosmDescontos"]))
                configuracaoAmbiente.ConcessionariasComDescontos = ConfigurationManager.AppSettings["ConcessionariasComDescontos"];

            if (string.IsNullOrWhiteSpace(configuracaoAmbiente.PercentualDescontoConcessionarias) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["PercentualDescontoConcessionarias"]))
                configuracaoAmbiente.PercentualDescontoConcessionarias = ConfigurationManager.AppSettings["PercentualDescontoConcessionarias"];

            if (string.IsNullOrWhiteSpace(configuracaoAmbiente.PlacaPadraoConsultaValorPedagio) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["PlacaPadraoConsultaValorPedagio"]))
                configuracaoAmbiente.PlacaPadraoConsultaValorPedagio = ConfigurationManager.AppSettings["PlacaPadraoConsultaValorPedagio"];

            if (!configuracaoAmbiente.CalcularHorarioDoCarregamento.HasValue && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["CalcularHorarioDoCarregamento"]))
                configuracaoAmbiente.CalcularHorarioDoCarregamento = String.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["CalcularHorarioDoCarregamento"]);

            if (!configuracaoAmbiente.EnviarTodasNotificacoesPorEmail.HasValue)
                configuracaoAmbiente.EnviarTodasNotificacoesPorEmail = System.Configuration.ConfigurationManager.AppSettings["EnviarTodasNotificacoesPorEmail"].ToBool();

            if (string.IsNullOrWhiteSpace(configuracaoAmbiente.APIOCRLink) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["APIOCRLink"]))
                configuracaoAmbiente.APIOCRLink = ConfigurationManager.AppSettings["APIOCRLink"];

            if (string.IsNullOrWhiteSpace(configuracaoAmbiente.APIOCRKey) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["APIOCRKey"]))
                configuracaoAmbiente.APIOCRKey = ConfigurationManager.AppSettings["APIOCRKey"];

            if (string.IsNullOrWhiteSpace(configuracaoAmbiente.QuantidadeSelecaoAgrupamentoCargaAutomatico) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["QuantidadeSelecaoAgrupamentoCargaAutomatico"]))
                configuracaoAmbiente.QuantidadeSelecaoAgrupamentoCargaAutomatico = ConfigurationManager.AppSettings["QuantidadeSelecaoAgrupamentoCargaAutomatico"];

            if (string.IsNullOrWhiteSpace(configuracaoAmbiente.QuantidadeCargasAgrupamentoCargaAutomatico) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["QuantidadeCargasAgrupamentoCargaAutomatico"]))
                configuracaoAmbiente.QuantidadeCargasAgrupamentoCargaAutomatico = ConfigurationManager.AppSettings["QuantidadeCargasAgrupamentoCargaAutomatico"];

            if (string.IsNullOrWhiteSpace(configuracaoAmbiente.HorarioExecucaoThreadDiaria) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["HorarioExecucaoThreadDiaria"]))
                configuracaoAmbiente.HorarioExecucaoThreadDiaria = ConfigurationManager.AppSettings["HorarioExecucaoThreadDiaria"];

            if (!configuracaoAmbiente.CalcularFreteFechamento.HasValue)
                configuracaoAmbiente.CalcularFreteFechamento = ConfigurationManager.AppSettings["CalcularFreteFechamento"] == "SIM";

            if (!configuracaoAmbiente.GerarDocumentoFechamento.HasValue)
                configuracaoAmbiente.GerarDocumentoFechamento = ConfigurationManager.AppSettings["GerarDocumentoFechamento"] == "SIM";

            if (!configuracaoAmbiente.NovoLayoutPortalFornecedor.HasValue)
                configuracaoAmbiente.NovoLayoutPortalFornecedor = ConfigurationManager.AppSettings["NovoLayoutPortalFornecedor"].ToBool();

            if (!configuracaoAmbiente.NovoLayoutCabotagem.HasValue)
                configuracaoAmbiente.NovoLayoutCabotagem = ConfigurationManager.AppSettings["NovoLayoutCabotagem"].ToBool();

            if (string.IsNullOrWhiteSpace(configuracaoAmbiente.FornecedorTMS) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["FornecedorTMS"]))
                configuracaoAmbiente.FornecedorTMS = ConfigurationManager.AppSettings["FornecedorTMS"];

            if (!configuracaoAmbiente.UtilizarIntegracaoSaintGobainNova.HasValue)
                configuracaoAmbiente.UtilizarIntegracaoSaintGobainNova = ConfigurationManager.AppSettings["UtilizarIntegracaoSaintGobainNova"] == "SIM";

            if (!configuracaoAmbiente.FiltrarCargasPorProprietario.HasValue)
                configuracaoAmbiente.FiltrarCargasPorProprietario = ConfigurationManager.AppSettings["FiltrarCargasPorProprietario"] == "SIM";

            if (!configuracaoAmbiente.CargaControleEntrega_Habilitar_ImportacaoCargaFluvial.HasValue)
                configuracaoAmbiente.CargaControleEntrega_Habilitar_ImportacaoCargaFluvial = ConfigurationManager.AppSettings["CargaControleEntrega_Habilitar_ImportacaoCargaFluvial"].ToBool();

            if (string.IsNullOrWhiteSpace(configuracaoAmbiente.TipoArmazenamento) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["TipoArmazemento"]))
                configuracaoAmbiente.TipoArmazenamento = ConfigurationManager.AppSettings["TipoArmazemento"];

            if (string.IsNullOrWhiteSpace(configuracaoAmbiente.EnderecoFTP) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["EnderecoFTP"]))
                configuracaoAmbiente.EnderecoFTP = ConfigurationManager.AppSettings["EnderecoFTP"];

            if (string.IsNullOrWhiteSpace(configuracaoAmbiente.UsuarioFTP) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["UsuarioFTP"]))
                configuracaoAmbiente.UsuarioFTP = ConfigurationManager.AppSettings["UsuarioFTP"];

            if (string.IsNullOrWhiteSpace(configuracaoAmbiente.SenhaFTP) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["SenhaFTP"]))
                configuracaoAmbiente.SenhaFTP = ConfigurationManager.AppSettings["SenhaFTP"];

            if (string.IsNullOrWhiteSpace(configuracaoAmbiente.PortaFTP) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["PortaFTP"]))
                configuracaoAmbiente.PortaFTP = ConfigurationManager.AppSettings["PortaFTP"];

            if (string.IsNullOrWhiteSpace(configuracaoAmbiente.PrefixosFTP) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["PrefixosFTP"]))
                configuracaoAmbiente.PrefixosFTP = ConfigurationManager.AppSettings["PrefixosFTP"];

            if (string.IsNullOrWhiteSpace(configuracaoAmbiente.EmailsFTP) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["Emails"]))
                configuracaoAmbiente.EmailsFTP = ConfigurationManager.AppSettings["Emails"];

            if (string.IsNullOrWhiteSpace(configuracaoAmbiente.CodigoEmpresaMultisoftware) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["CodigoEmpresaMultisoftware"]))
                configuracaoAmbiente.CodigoEmpresaMultisoftware = ConfigurationManager.AppSettings["CodigoEmpresaMultisoftware"];

            if (string.IsNullOrWhiteSpace(configuracaoAmbiente.CodigoEmpresaMultisoftware) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["MinutosParaConsultaNatura"]))
                configuracaoAmbiente.MinutosParaConsultaNatura = ConfigurationManager.AppSettings["MinutosParaConsultaNatura"];

            if (string.IsNullOrWhiteSpace(configuracaoAmbiente.CodigoEmpresaMultisoftware) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["FiliaisNatura"]))
                configuracaoAmbiente.FiliaisNatura = ConfigurationManager.AppSettings["FiliaisNatura"];

            if (!configuracaoAmbiente.FTPPassivo.HasValue && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["FTPPassivo"]))
                configuracaoAmbiente.FTPPassivo = ConfigurationManager.AppSettings["FTPPassivo"].ToBool();

            if (!configuracaoAmbiente.UtilizaSFTP.HasValue && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["UtilizaSFTP"]))
                configuracaoAmbiente.UtilizaSFTP = ConfigurationManager.AppSettings["UtilizaSFTP"].ToBool();

            if (!configuracaoAmbiente.GerarNotFisPorNota.HasValue && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["GerarNotFisPorNota"]))
                configuracaoAmbiente.GerarNotFisPorNota = ConfigurationManager.AppSettings["GerarNotFisPorNota"].ToBool();

            if (!configuracaoAmbiente.UtilizarMetodoImportacaoTabelaFretePorServico.HasValue)
                configuracaoAmbiente.UtilizarMetodoImportacaoTabelaFretePorServico = ConfigurationManager.AppSettings["UtilizarMetodoImportacaoTabelaFretePorServico"] == "SIM";

            if (!configuracaoAmbiente.UtilizarLayoutImportacaoTabelaFreteGPA.HasValue)
                configuracaoAmbiente.UtilizarLayoutImportacaoTabelaFreteGPA = ConfigurationManager.AppSettings["UtilizarLayoutImportacaoTabelaFreteGPA"] == "SIM";

            if (!configuracaoAmbiente.ExibirSituacaoIntegracaoXMLGPA.HasValue)
                configuracaoAmbiente.ExibirSituacaoIntegracaoXMLGPA = ConfigurationManager.AppSettings["ExibirSituacaoIntegracaoXML"] == "SIM";

            if (string.IsNullOrWhiteSpace(configuracaoAmbiente.WebServiceConsultaCTe) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["WebServiceConsultaCTe"]))
                configuracaoAmbiente.WebServiceConsultaCTe = ConfigurationManager.AppSettings["WebServiceConsultaCTe"];

            if (!configuracaoAmbiente.ProcessarCTeMultiCTe.HasValue && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["ProcessarCTeMultiCTe"]))
                configuracaoAmbiente.ProcessarCTeMultiCTe = ConfigurationManager.AppSettings["ProcessarCTeMultiCTe"] == "SIM";

            if (!configuracaoAmbiente.NaoUtilizarCNPJTransportador.HasValue && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["NaoUtilizarCNPJTransportador"]))
                configuracaoAmbiente.NaoUtilizarCNPJTransportador = ConfigurationManager.AppSettings["NaoUtilizarCNPJTransportador"] == "SIM";

            if (!configuracaoAmbiente.BuscarFilialPorCNPJRemetenteDestinatarioGerarCargaCTe.HasValue && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["BuscarFilialPorCNPJRemetenteDestinatarioGerarCargaCTe"]))
                configuracaoAmbiente.BuscarFilialPorCNPJRemetenteDestinatarioGerarCargaCTe = ConfigurationManager.AppSettings["BuscarFilialPorCNPJRemetenteDestinatarioGerarCargaCTe"] == "SIM";

            if (!configuracaoAmbiente.SempreUsarAtividadeCliente.HasValue && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["SempreUsarAtividadeCliente"]))
                configuracaoAmbiente.SempreUsarAtividadeCliente = ConfigurationManager.AppSettings["SempreUsarAtividadeCliente"] == "SIM";

            if (!configuracaoAmbiente.AtualizarFantasiaClienteIntegracaoCTe.HasValue && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["AtualizarFantasiaClienteIntegracaoCTe"]))
                configuracaoAmbiente.AtualizarFantasiaClienteIntegracaoCTe = ConfigurationManager.AppSettings["AtualizarFantasiaClienteIntegracaoCTe"] != "NAO";

            if (!configuracaoAmbiente.CadastrarMotoristaIntegracaoCTe.HasValue && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["CadastrarMotoristaIntegracaoCTe"]))
                configuracaoAmbiente.CadastrarMotoristaIntegracaoCTe = ConfigurationManager.AppSettings["CadastrarMotoristaIntegracaoCTe"] == "SIM";

            if (!configuracaoAmbiente.CTeUtilizaProprietarioCadastro.HasValue && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["CTeUtilizaProprietarioCadastro"]))
                configuracaoAmbiente.CTeUtilizaProprietarioCadastro = ConfigurationManager.AppSettings["CTeUtilizaProprietarioCadastro"] == "SIM";

            if (!configuracaoAmbiente.CTeCarregarVinculosVeiculosCadastro.HasValue && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["CTeCarregarVinculosVeiculosCadastro"]))
                configuracaoAmbiente.CTeCarregarVinculosVeiculosCadastro = ConfigurationManager.AppSettings["CTeCarregarVinculosVeiculosCadastro"] == "SIM";

            if (!configuracaoAmbiente.CTeAtualizaTipoVeiculo.HasValue && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["CTeAtualizaTipoVeiculo"]))
                configuracaoAmbiente.CTeAtualizaTipoVeiculo = ConfigurationManager.AppSettings["CTeAtualizaTipoVeiculo"] == "SIM";

            if (!configuracaoAmbiente.NaoAtualizarCadastroVeiculo.HasValue && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["NaoAtualizarCadastroVeiculo"]))
                configuracaoAmbiente.NaoAtualizarCadastroVeiculo = ConfigurationManager.AppSettings["NaoAtualizarCadastroVeiculo"] == "SIM";

            if (!configuracaoAmbiente.AgruparQuantidadesImportacaoCTe.HasValue && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["AgruparQuantidadesImportacaoCTe"]))
                configuracaoAmbiente.AgruparQuantidadesImportacaoCTe = ConfigurationManager.AppSettings["AgruparQuantidadesImportacaoCTe"] == "SIM";

            if (!configuracaoAmbiente.EncerraMDFeAutomatico.HasValue && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["EncerraMDFeAutomatico"]))
                configuracaoAmbiente.EncerraMDFeAutomatico = ConfigurationManager.AppSettings["EncerraMDFeAutomatico"] == "SIM";

            if (!configuracaoAmbiente.EnviaContingenciaMDFeAutomatico.HasValue && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["EnviaContingenciaMDFeAutomatico"]))
                configuracaoAmbiente.EnviaContingenciaMDFeAutomatico = ConfigurationManager.AppSettings["EnviaContingenciaMDFeAutomatico"] == "SIM";

            if (!configuracaoAmbiente.EnviarCertificadoOracle.HasValue)
                configuracaoAmbiente.EnviarCertificadoOracle = ConfigurationManager.AppSettings["EnviarCertificadoOracle"] == "SIM";

            string seting = ConfigurationManager.AppSettings["APIOCRKey"];

            if (string.IsNullOrWhiteSpace(configuracaoAmbiente.APIOCRKey) && !string.IsNullOrWhiteSpace(seting))
                configuracaoAmbiente.APIOCRKey = seting;

            seting = ConfigurationManager.AppSettings["APIOCRLink"];
            if (string.IsNullOrWhiteSpace(configuracaoAmbiente.APIOCRLink) && !string.IsNullOrWhiteSpace(seting))
                configuracaoAmbiente.APIOCRLink = seting;

            seting = ConfigurationManager.AppSettings["IdentificacaoAmbiente"];
            if (string.IsNullOrWhiteSpace(configuracaoAmbiente.IdentificacaoAmbiente) && !string.IsNullOrWhiteSpace(seting))
                configuracaoAmbiente.IdentificacaoAmbiente = seting;

            seting = ConfigurationManager.AppSettings["WebServiceConsultaCTe"];
            if (string.IsNullOrWhiteSpace(configuracaoAmbiente.WebServiceConsultaCTe) && !string.IsNullOrWhiteSpace(seting))
                configuracaoAmbiente.WebServiceConsultaCTe = seting;

            seting = ConfigurationManager.AppSettings["CodigoLocalidadeNaoCadastrada"];
            if (string.IsNullOrWhiteSpace(configuracaoAmbiente.CodigoLocalidadeNaoCadastrada) && !string.IsNullOrWhiteSpace(seting))
                configuracaoAmbiente.CodigoLocalidadeNaoCadastrada = seting;

            seting = ConfigurationManager.AppSettings["EmpresasUsuariosMultiCTe"];
            if (string.IsNullOrWhiteSpace(configuracaoAmbiente.EmpresasUsuariosMultiCTe) && !string.IsNullOrWhiteSpace(seting))
                configuracaoAmbiente.EmpresasUsuariosMultiCTe = seting;


            if (!configuracaoAmbiente.PesoMaximoIntegracaoCarga.HasValue)
            {
                seting = ConfigurationManager.AppSettings["PesoMaximoIntegracaoCarga"];
                decimal.TryParse(seting, out decimal pesoMaximo);
                configuracaoAmbiente.PesoMaximoIntegracaoCarga = pesoMaximo;
            }

            seting = ConfigurationManager.AppSettings["UtilizaOptanteSimplesNacionalDaIntegracao"];
            if (!configuracaoAmbiente.UtilizaOptanteSimplesNacionalDaIntegracao.HasValue)
                configuracaoAmbiente.UtilizaOptanteSimplesNacionalDaIntegracao = (seting == "SIM" || string.IsNullOrWhiteSpace(seting));

            seting = ConfigurationManager.AppSettings["AtualizarTipoEmpresa"];
            if (!configuracaoAmbiente.AtualizarTipoEmpresa.HasValue)
                configuracaoAmbiente.AtualizarTipoEmpresa = seting == "SIM";

            seting = ConfigurationManager.AppSettings["EncerraMDFeAutomaticoComMesmaData"];
            if (!configuracaoAmbiente.EncerraMDFeAutomaticoComMesmaData.HasValue)
                configuracaoAmbiente.EncerraMDFeAutomaticoComMesmaData = seting == "SIM";

            seting = ConfigurationManager.AppSettings["EncerraMDFeAntesDaEmissao"];
            if (!configuracaoAmbiente.EncerraMDFeAntesDaEmissao.HasValue)
                configuracaoAmbiente.EncerraMDFeAntesDaEmissao = seting == "SIM";

            seting = ConfigurationManager.AppSettings["EncerraMDFeAutomaticoOutrosSistemas"];
            if (!configuracaoAmbiente.EncerraMDFeAutomaticoOutrosSistemas.HasValue)
                configuracaoAmbiente.EncerraMDFeAutomaticoOutrosSistemas = seting == "SIM";

            seting = ConfigurationManager.AppSettings["MDFeUtilizaDadosVeiculoCadastro"];
            if (!configuracaoAmbiente.MDFeUtilizaDadosVeiculoCadastro.HasValue)
                configuracaoAmbiente.MDFeUtilizaDadosVeiculoCadastro = (seting == "SIM" || string.IsNullOrWhiteSpace(seting));

            seting = ConfigurationManager.AppSettings["MDFeUtilizaVeiculoReboqueComoTracao"];
            if (!configuracaoAmbiente.MDFeUtilizaVeiculoReboqueComoTracao.HasValue)
                configuracaoAmbiente.MDFeUtilizaVeiculoReboqueComoTracao = seting == "SIM";

            seting = ConfigurationManager.AppSettings["CTeUtilizaProprietarioCadastro"];
            if (!configuracaoAmbiente.CTeUtilizaProprietarioCadastro.HasValue)
                configuracaoAmbiente.CTeUtilizaProprietarioCadastro = seting == "SIM";

            seting = ConfigurationManager.AppSettings["IncluirISSNFSeLocalidadeTomadorDiferentePrestador"];
            if (!configuracaoAmbiente.IncluirISSNFSeLocalidadeTomadorDiferentePrestador.HasValue)
                configuracaoAmbiente.IncluirISSNFSeLocalidadeTomadorDiferentePrestador = seting == "SIM";

            seting = ConfigurationManager.AppSettings["UtilizarDocaDoComplementoFilial"];
            if (!configuracaoAmbiente.UtilizarDocaDoComplementoFilial.HasValue)
                configuracaoAmbiente.UtilizarDocaDoComplementoFilial = seting == "SIM";

            seting = ConfigurationManager.AppSettings["LimparMotoristaIntegracaoVeiculo"];
            if (!configuracaoAmbiente.LimparMotoristaIntegracaoVeiculo.HasValue)
                configuracaoAmbiente.LimparMotoristaIntegracaoVeiculo = (seting == "SIM" || seting == "");

            seting = ConfigurationManager.AppSettings["RetornarModeloVeiculo"];
            if (!configuracaoAmbiente.RetornarModeloVeiculo.HasValue)
                configuracaoAmbiente.RetornarModeloVeiculo = seting == "SIM";

            seting = ConfigurationManager.AppSettings["LoginAD"];
            if (!configuracaoAmbiente.LoginAD.HasValue)
                configuracaoAmbiente.LoginAD = seting?.ToBool() ?? false;

            seting = ConfigurationManager.AppSettings["AtualizarValorFrete_AtualizarICMS"];
            if (!configuracaoAmbiente.AtualizarValorFrete_AtualizarICMS.HasValue)
                configuracaoAmbiente.AtualizarValorFrete_AtualizarICMS = seting == "SIM";

            seting = ConfigurationManager.AppSettings["ConsultarDuplicidadeOracle"];
            if (!configuracaoAmbiente.ConsultarDuplicidadeOracle.HasValue)
                configuracaoAmbiente.ConsultarDuplicidadeOracle = seting != "NAO";

            seting = ConfigurationManager.AppSettings["EnviarIntegracaoErroMDFeMagalog"];
            if (!configuracaoAmbiente.EnviarIntegracaoErroMDFeMagalog.HasValue)
                configuracaoAmbiente.EnviarIntegracaoErroMDFeMagalog = seting == "SIM";

            seting = ConfigurationManager.AppSettings["EnviarIntegracaoMagalogNoRetorno"];
            if (!configuracaoAmbiente.EnviarIntegracaoMagalogNoRetorno.HasValue)
                configuracaoAmbiente.EnviarIntegracaoMagalogNoRetorno = seting == "SIM";

            seting = ConfigurationManager.AppSettings["RegerarDACTEOracle"];
            if (!configuracaoAmbiente.RegerarDACTEOracle.HasValue)
                configuracaoAmbiente.RegerarDACTEOracle = seting == "SIM";

            seting = ConfigurationManager.AppSettings["ReenviarErroIntegracaoCTe"];
            if (!configuracaoAmbiente.ReenviarErroIntegracaoCTe.HasValue)
                configuracaoAmbiente.ReenviarErroIntegracaoCTe = seting == "SIM";

            seting = ConfigurationManager.AppSettings["ValidarNFeJaImportada"];
            if (!configuracaoAmbiente.ValidarNFeJaImportada.HasValue)
                configuracaoAmbiente.ValidarNFeJaImportada = seting == "SIM";

            seting = ConfigurationManager.AppSettings["RecalcularICMSNaEmissaoCTe"];
            if (!configuracaoAmbiente.RecalcularICMSNaEmissaoCTe.HasValue)
                configuracaoAmbiente.RecalcularICMSNaEmissaoCTe = seting == "SIM";

            seting = ConfigurationManager.AppSettings["ReenviarErroIntegracaoMDFe"];
            if (!configuracaoAmbiente.ReenviarErroIntegracaoMDFe.HasValue)
                configuracaoAmbiente.ReenviarErroIntegracaoMDFe = seting == "SIM";

            seting = ConfigurationManager.AppSettings["EnviarEmailMDFeClientes"];
            if (!configuracaoAmbiente.EnviarEmailMDFeClientes.HasValue)
                configuracaoAmbiente.EnviarEmailMDFeClientes = seting == "SIM";

            seting = ConfigurationManager.AppSettings["GerarCTeDasNFSeAutorizadas"];
            if (!configuracaoAmbiente.GerarCTeDasNFSeAutorizadas.HasValue)
                configuracaoAmbiente.GerarCTeDasNFSeAutorizadas = seting == "SIM";

            seting = ConfigurationManager.AppSettings["IntegracaoNFSeUtilizaAliquotaMultiCTeQuandoTransportadorSimples"];
            if (!configuracaoAmbiente.IntegracaoNFSeUtilizaAliquotaMultiCTeQuandoTransportadorSimples.HasValue)
                configuracaoAmbiente.IntegracaoNFSeUtilizaAliquotaMultiCTeQuandoTransportadorSimples = seting == "SIM";

            if (string.IsNullOrWhiteSpace(configuracaoAmbiente.PrefixoMSMQ) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["PrefixoMSMQ"]))
                configuracaoAmbiente.PrefixoMSMQ = ConfigurationManager.AppSettings["PrefixoMSMQ"];

            if (string.IsNullOrWhiteSpace(configuracaoAmbiente.EnderecoComputadorRemotoFila) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["EnderecoComputadorRemotoFila"]))
                configuracaoAmbiente.EnderecoComputadorRemotoFila = ConfigurationManager.AppSettings["EnderecoComputadorRemotoFila"];

            if (string.IsNullOrWhiteSpace(configuracaoAmbiente.EndpointServiceFila) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["EndpointServiceFila"]))
                configuracaoAmbiente.EndpointServiceFila = ConfigurationManager.AppSettings["EndpointServiceFila"];

            if (string.IsNullOrWhiteSpace(configuracaoAmbiente.UrlReportAPI) && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["REPORT_SERVER_URL"]))
                configuracaoAmbiente.UrlReportAPI = ConfigurationManager.AppSettings["REPORT_SERVER_URL"];

            int.TryParse(System.Configuration.ConfigurationManager.AppSettings.Get("IntervaloDocumentosFiscaisEmbarcador"), out int tempoIntervaloRequisicaoAux);
            if (!configuracaoAmbiente.IntervaloDocumentosFiscaisEmbarcador.HasValue)
                configuracaoAmbiente.IntervaloDocumentosFiscaisEmbarcador = tempoIntervaloRequisicaoAux;

            if (configuracaoAmbiente.Codigo > 0)
                repConfiguracaoAmbiente.Atualizar(configuracaoAmbiente);
            else
                repConfiguracaoAmbiente.Inserir(configuracaoAmbiente);
        }

    }
}
