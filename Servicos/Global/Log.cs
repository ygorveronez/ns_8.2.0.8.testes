using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Serilog;
using Serilog.Context;
using Serilog.Sinks.Graylog;
using Serilog.Sinks.Graylog.Core.Transport;
using Serilog.Sinks.Network;
using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Servicos
{
    public static class Log
    {

        #region Propriedades Privadas

        private static Dominio.ObjetosDeValor.Embarcador.Configuracoes.ConfiguracaoLog _configuracaoLog;
        private static string _stringConexao;
        private static string _cliente;
        private static int _minutosAtualizarConfiguracaoLog = 0;
        private static DateTime? _atualizouConfiguracaoLog = null;
        private static readonly ConcurrentDictionary<string, ILogger> _loggers = new ConcurrentDictionary<string, ILogger>();

        #endregion Propriedades Privadas

        #region Metodos Públicas

        public static void SetStringConexao(string conexao, int minutosAtualizarConfiguracaoLog = 0)
        {
            if (string.IsNullOrEmpty(_stringConexao))
            {
                _stringConexao = conexao;
                _minutosAtualizarConfiguracaoLog = minutosAtualizarConfiguracaoLog;
            }
        }

        public static void SetCliente(string cliente)
        {
            if (string.IsNullOrEmpty(_cliente))
                _cliente = cliente;
        }

        /// <summary>
        /// Grava exceção no arquivo de Log
        /// </summary>
        /// <param name="ex">Exceção capturada pelo catch</param>
        public static void TratarErro(Exception ex, [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string callerMember = null, [CallerFilePath] string filePath = null)
        {
            try
            {
                GravarLog(ex.ToString(), "", TipoLogSistema.Error, callerMember, lineNumber, filePath);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Grava exceção no arquivo de Log com prefixo específico
        /// </summary>
        /// <param name="ex">Exceção capturada pelo catch</param>
        /// <param name="arquivo">Prefixo específico (PREFIXO-dd-mm-aaaa.txt)</param>
        public static void TratarErro(Exception ex, string arquivo, [CallerMemberName] string callerMember = null, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = null)
        {
            try
            {
                GravarLog(ex.ToString(), arquivo, TipoLogSistema.Error, callerMember, lineNumber, filePath);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Cria um log para o ElasticSearch
        /// </summary>
        /// <param name="mensagem">Conteúdo da mensagem</param>
        public static void TratarErro(object obj, TipoLogSistema tipoLogSistema = TipoLogSistema.Error, [CallerMemberName] string callerMember = null, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = null)
        {
            try
            {
                var serialize = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
                GravarLog(serialize, "", tipoLogSistema, callerMember, lineNumber, filePath);
            }
            catch (Exception ex)
            {
                GravarLog($"Invalid value to serialize: {ex}", "", TipoLogSistema.Error, callerMember, lineNumber, filePath);
            }
        }

        /// <summary>
        /// Grava uma mensagem qualquer no arquivo de Log
        /// </summary>
        /// <param name="mensagem">Conteúdo da mensagem</param>
        public static void TratarErro(string mensagem, [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string callerMember = null, [CallerFilePath] string filePath = null)
        {
            GravarLog(mensagem, "", TipoLogSistema.Error, callerMember, lineNumber, filePath);
        }

        /// <summary>
        /// Grava uma mensagem qualquer no arquivo de Log com prefixo específico
        /// </summary>
        /// <param name="mensagem">Conteúdo da mensagem</param>
        /// <param name="arquivo">Prefixo específico (PREFIXO-dd-mm-aaaa.txt)</param>
        public static void TratarErro(string mensagem, string arquivo, TipoLogSistema tipoLogSistema = TipoLogSistema.Error, [CallerMemberName] string callerMember = null, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = null)
        {
            GravarLog(mensagem, arquivo, tipoLogSistema, callerMember, lineNumber, filePath);
        }

        /// <summary>
        /// Grava uma mensagem qualquer no arquivo de Log
        /// </summary>
        /// <param name="mensagem">Conteúdo da mensagem</param>
        public static void GravarInfo(string mensagem, [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string callerMember = null, [CallerFilePath] string filePath = null)
        {
            GravarLog(mensagem, "", TipoLogSistema.Info, callerMember, lineNumber, filePath);
        }

        /// <summary>
        /// Grava uma mensagem qualquer no arquivo de Log com prefixo específico
        /// </summary>
        /// <param name="mensagem">Conteúdo da mensagem</param>
        /// <param name="arquivo">Prefixo específico (PREFIXO-dd-mm-aaaa.txt)</param>
        public static void GravarInfo(string mensagem, string arquivo, [CallerMemberName] string callerMember = null, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = null)
        {
            GravarLog(mensagem, arquivo, TipoLogSistema.Info, callerMember, lineNumber, filePath);
        }

        /// <summary>
        /// Grava uma mensagem qualquer no arquivo de Log
        /// </summary>
        /// <param name="mensagem">Conteúdo da mensagem</param>
        public static void GravarAdvertencia(string mensagem, [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string callerMember = null, [CallerFilePath] string filePath = null)
        {
            GravarLog(mensagem, "", TipoLogSistema.Advertencia, callerMember, lineNumber, filePath);
        }

        /// <summary>
        /// Grava uma mensagem qualquer no arquivo de Log com prefixo específico
        /// </summary>
        /// <param name="mensagem">Conteúdo da mensagem</param>
        /// <param name="arquivo">Prefixo específico (PREFIXO-dd-mm-aaaa.txt)</param>
        public static void GravarAdvertencia(string mensagem, string arquivo, [CallerMemberName] string callerMember = null, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = null)
        {
            GravarLog(mensagem, arquivo, TipoLogSistema.Advertencia, callerMember, lineNumber, filePath);
        }

        /// <summary>
        /// Grava uma mensagem qualquer no arquivo de Log
        /// </summary>
        /// <param name="mensagem">Conteúdo da mensagem</param>
        public static void GravarDebug(string mensagem, [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string callerMember = null, [CallerFilePath] string filePath = null)
        {
            GravarLog(mensagem, "", TipoLogSistema.Debug, callerMember, lineNumber, filePath);
        }

        /// <summary>
        /// Grava uma mensagem qualquer no arquivo de Log com prefixo específico
        /// </summary>
        /// <param name="mensagem">Conteúdo da mensagem</param>
        /// <param name="arquivo">Prefixo específico (PREFIXO-dd-mm-aaaa.txt)</param>
        public static void GravarDebug(string mensagem, string arquivo, [CallerMemberName] string callerMember = null, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = null)
        {
            GravarLog(mensagem, arquivo, TipoLogSistema.Debug, callerMember, lineNumber, filePath);
        }

        public static void RecarregarConfiguracao()
        {
            ObterConfiguracaoLog(true);
        }


        #endregion  Propriedades Públicas

        #region Metodos Privadas

        private static void ObterConfiguracaoLog(bool recarregarConfiguracao = false)
        {
            try
            {
                if (_minutosAtualizarConfiguracaoLog != 0 && _atualizouConfiguracaoLog != null)
                {
                    if (System.DateTime.Now.AddMinutes(-_minutosAtualizarConfiguracaoLog) > _atualizouConfiguracaoLog.Value)
                        recarregarConfiguracao = true;
                }

                if ((_configuracaoLog == null || recarregarConfiguracao) && !string.IsNullOrEmpty(_stringConexao))
                {
                    _atualizouConfiguracaoLog = System.DateTime.Now;
                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoLog configuracao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoLog(new Repositorio.UnitOfWork(_stringConexao)).BuscarConfiguracaoPadrao();

#if DEBUG
                    bool? logArquivo = true;
                    bool logWeb = false;
                    bool graylog = false;
#else
                    bool? logArquivo = configuracao.UtilizaLogArquivo;
                    bool logWeb = configuracao.UtilizaLogWeb;
                    bool graylog = configuracao.UtilizaGraylog;
#endif

                    _configuracaoLog = new Dominio.ObjetosDeValor.Embarcador.Configuracoes.ConfiguracaoLog()
                    {
                        UtilizaLogArquivo = logArquivo,
                        UtilizaLogWeb = logWeb,
                        ProtocoloLogWeb = configuracao.ProtocoloLogWeb ?? ProtocoloLogWeb.UDP,
                        GravarLogError = configuracao.GravarLogError,
                        GravarLogInfo = configuracao.GravarLogInfo,
                        GravarLogAdvertencia = configuracao.GravarLogAdvertencia,
                        GravarLogDebug = configuracao.GravarLogDebug,
                        Porta = configuracao.Porta,
                        Url = configuracao.Url,
                        UtilizaGraylog = graylog,
                        ProtocoloLogGraylog = configuracao.ProtocoloLogGraylog ?? ProtocoloLogWeb.TCP,
                        UrlGraylog = configuracao.UrlGraylog,
                        PortaGraylog = configuracao.PortaGraylog
                    };
                }
            }
            catch
            {

            }
        }

        private static void GravarLog(string mensagem, string prefixo = "", TipoLogSistema tipoLogSistema = TipoLogSistema.Error, string callerMember = null, int lineNumber = 0, string filePath = null)
        {
            GravarSerilog(mensagem, prefixo, tipoLogSistema, callerMember, lineNumber, filePath);
        }

        private static void GravarSerilog(string mensagem, string prefixo, TipoLogSistema tipoLogSistema = TipoLogSistema.Error, string callerMember = null, int lineNumber = 0, string filePath = null)
        {
            try
            {
                ObterConfiguracaoLog();

                if (!verificarGravarLog(tipoLogSistema))
                    return;

                DateTime dateTime = DateTime.Now;

                string arquivo = (string.IsNullOrWhiteSpace(prefixo) ? "" : prefixo + "-") + dateTime.Day + "-" + dateTime.Month + "-" + dateTime.Year + ".txt";
                string path = Utilidades.IO.FileStorageService.LocalStorage.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log");
                string file = Utilidades.IO.FileStorageService.LocalStorage.Combine(path, arquivo);

                if (_configuracaoLog?.UtilizaLogArquivo ?? true && Utilidades.IO.FileStorageService.Storage.GetStorageType() == Utilidades.IO.StorageType.Local)
                {
                    string logPrefix = string.IsNullOrWhiteSpace(prefixo) ? "" : prefixo + "-";
                    string messageTemplate = "{LogPrefix} {Timestamp} {Message}";
                    WriteSerilog(file, false, tipoLogSistema, logPrefix, mensagem, messageTemplate, dateTime, false, callerMember, lineNumber, filePath);
                }

                if (_configuracaoLog?.UtilizaLogWeb ?? false)
                {
                    string logPrefix = string.IsNullOrWhiteSpace(prefixo) ? "Geral" : prefixo;
                    string messageTemplate = "{LogPrefix} {Timestamp} {Message} {Cliente}";
                    WriteSerilog(file, true, tipoLogSistema, logPrefix, mensagem, messageTemplate, dateTime, false, callerMember, lineNumber, filePath);
                }

                if (_configuracaoLog?.UtilizaGraylog ?? false)
                {
                    string logPrefix = string.IsNullOrWhiteSpace(prefixo) ? "Geral" : prefixo;
                    string messageTemplate = "{LogPrefix} {Timestamp} {Message} {Cliente}";
                    WriteSerilog("", false, tipoLogSistema, logPrefix, mensagem, messageTemplate, dateTime, true, callerMember, lineNumber, filePath);
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.ToString());
            }
        }

        private static void WriteSerilog(string file, bool logWeb, TipoLogSistema tipoLogSistema, string logPrefix, string mensagem, string messageTemplate, DateTime dateTime, bool graylog = false, string callerMember = "", int lineNumber = 0, string filePath = "")
        {
            var loggerName = GetLoggerName(file, logWeb, graylog);
            var logger = _loggers.GetOrAdd(loggerName, (name) => CreateLogger(name, logWeb, graylog));

            using (LogContext.PushProperty("MemberName", callerMember))
            using (LogContext.PushProperty("FilePath", filePath))
            using (LogContext.PushProperty("LineNumber", lineNumber))
            {
                string timestamp = dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");

                switch (tipoLogSistema)
                {
                    case TipoLogSistema.Error:
                        logger.Error(messageTemplate, logPrefix, timestamp, mensagem, _cliente);
                        break;
                    case TipoLogSistema.Info:
                        logger.Information(messageTemplate, logPrefix, timestamp, mensagem, _cliente);
                        break;
                    case TipoLogSistema.Advertencia:
                        logger.Warning(messageTemplate, logPrefix, timestamp, mensagem, _cliente);
                        break;
                    case TipoLogSistema.Debug:
                        logger.Debug(messageTemplate, logPrefix, timestamp, mensagem, _cliente);
                        break;
                    default:
                        break;
                }
            }
        }

        private static string GetLoggerName(string file, bool logWeb, bool graylog)
        {
            if (graylog)
                return "default-graylog";

            return logWeb ? "default-logweb" : file;
        }

        private static ILogger CreateLogger(string filePath, bool logWeb, bool graylog = false)
        {
            string applicationName = Assembly.GetEntryAssembly()?.GetName().Name ?? "NAO.DEFINIDO";

            var loggerConfig = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .Enrich.WithProperty("Application", applicationName)
                    .Enrich.FromLogContext();

            if (graylog)
            {
                var host = _configuracaoLog?.UrlGraylog ?? Environment.GetEnvironmentVariable("GRAYLOG_HOST");

                int port;
                var portEnv = Environment.GetEnvironmentVariable("GRAYLOG_PORT");
                if (_configuracaoLog?.PortaGraylog != null)
                {
                    port = _configuracaoLog.PortaGraylog;
                }
                else if (!int.TryParse(portEnv, out port))
                {
                    port = 12201; // Default Graylog port
                }

                TransportType transportType = TransportType.Tcp;

                if (_configuracaoLog != null && _configuracaoLog.ProtocoloLogGraylog == ProtocoloLogWeb.UDP)
                {
                    transportType = TransportType.Udp;

                }

                var logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("Application", applicationName)
                    .Enrich.WithProperty("Cliente", _cliente ?? "Cliente não definido")
                    .WriteTo.Graylog(new GraylogSinkOptions
                    {
                        HostnameOrAddress = host,
                        Port = port,
                        TransportType = transportType,
                        Facility = applicationName
                    })
                    .CreateLogger();

                return logger;
            }
            else if (logWeb)
            {
                if ((_configuracaoLog?.ProtocoloLogWeb ?? ProtocoloLogWeb.UDP) == ProtocoloLogWeb.TCP)
                {
                    loggerConfig
                        .Enrich.WithProperty("Cliente", _cliente ?? "Cliente não definido")
                        .WriteTo.Async(x => x.TCPSink(
                            $"tcp://{_configuracaoLog.Url}:{_configuracaoLog.Porta}",
                            new Serilog.Formatting.Json.JsonFormatter()
                        ));
                }
                else
                {
                    loggerConfig
                        .Enrich.WithProperty("Cliente", _cliente ?? "Cliente não definido")
                        .WriteTo.Async(x => x.Udp(
                            _configuracaoLog.Url,
                            _configuracaoLog.Porta,
                            AddressFamily.InterNetwork,
                            new Serilog.Formatting.Json.JsonFormatter()
                        ));
                }
            }
            else
            {
#if DEBUG
                loggerConfig
                    .WriteTo.Async(x => x.Console())
                    .WriteTo.Async(x => x.Debug())
                    .WriteTo.Async(x => x.File(filePath, rollingInterval: RollingInterval.Infinite, shared: true, fileSizeLimitBytes: null));
#else
                loggerConfig
                    .WriteTo.Async(x => x.Console())
                    .WriteTo.Async(x => x.File(filePath, rollingInterval: RollingInterval.Infinite, shared: true, fileSizeLimitBytes: null));
#endif
            }

            return loggerConfig.CreateLogger();
        }

        private static bool verificarGravarLog(TipoLogSistema tipoLogSistema)
        {
            if (tipoLogSistema == TipoLogSistema.Error && !(_configuracaoLog?.GravarLogError ?? true))
                return false;
            else if (tipoLogSistema == TipoLogSistema.Info && !(_configuracaoLog?.GravarLogInfo ?? false))
                return false;
            else if (tipoLogSistema == TipoLogSistema.Advertencia && !(_configuracaoLog?.GravarLogAdvertencia ?? true))
                return false;
            else if (tipoLogSistema == TipoLogSistema.Debug && !(_configuracaoLog?.GravarLogDebug ?? false))
                return false;

            if (!(_configuracaoLog?.UtilizaLogArquivo ?? true) && !(_configuracaoLog?.UtilizaLogWeb ?? false) && !(_configuracaoLog?.UtilizaGraylog ?? false))
                return false;

            return true;
        }

        #endregion Metodos Privadas
    }
}