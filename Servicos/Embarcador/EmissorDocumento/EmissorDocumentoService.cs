using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Servicos.Embarcador.EmissorDocumento.Emissores;
using System;

namespace Servicos.Embarcador.EmissorDocumento
{
    public static class EmissorDocumentoService
    {
        #region Instances

        private static IEmissorDocumentoCTe _currentEmissorDocumentoCTeInstance;
        private static IEmissorDocumentoMDFe _currentEmissorDocumentoMDFeInstance;
        private static readonly Lazy<IEmissorDocumentoCTe> _oracleEmissorDocumentoCTeInstance = new Lazy<IEmissorDocumentoCTe>(() => new OracleEmissorDocumentoCTe());
        private static readonly Lazy<IEmissorDocumentoCTe> _nstechEmissorDocumentoCTeInstance = new Lazy<IEmissorDocumentoCTe>(() => new NSTechEmissorDocumentoCTe(_obterXmlAutomaticamente));
        private static readonly Lazy<IEmissorDocumentoMDFe> _oracleEmissorDocumentoMDFeInstance = new Lazy<IEmissorDocumentoMDFe>(() => new OracleEmissorDocumentoMDFe());
        private static readonly Lazy<IEmissorDocumentoMDFe> _nstechEmissorDocumentoMDFeInstance = new Lazy<IEmissorDocumentoMDFe>(() => new NSTechEmissorDocumentoMDFe());
        private static readonly object _lock = new object();
        private static readonly object _lockConfigureApplicationEmissorDocumento = new object();

        private static bool _obterXmlAutomaticamente = false;

        #endregion

        #region Public Properties

        // Instância dinâmica baseada na configuração
        public static IEmissorDocumentoCTe EmissorDocumentoCTe
        {
            get
            {
                lock (_lock)
                {
                    return _currentEmissorDocumentoCTeInstance ??= CreateInstanceCTe();
                }
            }
        }

        // Instância dinâmica baseada no sistema emissor do documento
        public static IEmissorDocumentoCTe GetEmissorDocumentoCTe(TipoEmissorDocumento? tipoEmissorDocumento)
        {
            switch (tipoEmissorDocumento ?? TipoEmissorDocumento.Integrador)
            {
                case TipoEmissorDocumento.NSTech:
                    return _nstechEmissorDocumentoCTeInstance.Value;

                default:
                    return _oracleEmissorDocumentoCTeInstance.Value;
            }
        }

        // Instância dinâmica baseada na configuração
        public static IEmissorDocumentoMDFe EmissorDocumentoMDFe
        {
            get
            {
                lock (_lock)
                {
                    return _currentEmissorDocumentoMDFeInstance ??= CreateInstanceMDFe();
                }
            }
        }

        // Instância dinâmica baseada no sistema emissor do documento
        public static IEmissorDocumentoMDFe GetEmissorDocumentoMDFe(TipoEmissorDocumento? tipoEmissorDocumento)
        {
            switch (tipoEmissorDocumento ?? TipoEmissorDocumento.Integrador)
            {
                case TipoEmissorDocumento.NSTech:
                    return _nstechEmissorDocumentoMDFeInstance.Value;

                default:
                    return _oracleEmissorDocumentoMDFeInstance.Value;
            }
        }

        #endregion

        #region Private Properties

        private static TipoEmissorDocumento? _emissorDocumentoCTeType;

        private static TipoEmissorDocumento? _emissorDocumentoMDFeType;

        #endregion

        #region Public Properties

        public static TipoEmissorDocumento? EmissorDocumentoCTeType => _emissorDocumentoCTeType;

        public static TipoEmissorDocumento? EmissorDocumentoMDFeType => _emissorDocumentoMDFeType;

        #endregion

        #region Public Methods

        public static void ConfigureApplicationEmissorDocumento(string ConnectionString)
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(ConnectionString))
            {
                ConfigureApplicationEmissorDocumento(unitOfWork);
            }
        }

        public static void ConfigureApplicationEmissorDocumento(Repositorio.UnitOfWork unitOfWork)
        {
            lock (_lockConfigureApplicationEmissorDocumento)
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoIntegracaoEmissorDocumento repConfiguracaoIntegracaoEmissorDocumento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoIntegracaoEmissorDocumento(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoIntegracaoEmissorDocumento configuracaoIntegracaoEmissorDocumento = repConfiguracaoIntegracaoEmissorDocumento.BuscarConfiguracaoPadrao();

                _obterXmlAutomaticamente = configuracaoIntegracaoEmissorDocumento.ObterXMLAutomaticamente;

                switch (configuracaoIntegracaoEmissorDocumento?.TipoEmissorDocumentoCTe ?? TipoEmissorDocumento.Integrador)
                {
                    case TipoEmissorDocumento.Integrador:
                        ConfigureWithOracleCTeDefault();
                        break;
                    case TipoEmissorDocumento.NSTech:
                        ConfigureWithANSTechCTeDefault();
                        break;
                }

                switch (configuracaoIntegracaoEmissorDocumento?.TipoEmissorDocumentoMDFe ?? TipoEmissorDocumento.Integrador)
                {
                    case TipoEmissorDocumento.Integrador:
                        ConfigureWithOracleMDFeDefault();
                        break;
                    case TipoEmissorDocumento.NSTech:
                        ConfigureWithANSTechMDFeDefault();
                        break;
                }
            }
        }

        #endregion

        #region Private Methods

        private static IEmissorDocumentoCTe CreateInstanceCTe()
        {
            return _emissorDocumentoCTeType switch
            {
                TipoEmissorDocumento.NSTech => new NSTechEmissorDocumentoCTe(_obterXmlAutomaticamente),
                _ => _oracleEmissorDocumentoCTeInstance.Value
            };
        }

        private static IEmissorDocumentoMDFe CreateInstanceMDFe()
        {
            return _emissorDocumentoMDFeType switch
            {
                TipoEmissorDocumento.NSTech => new NSTechEmissorDocumentoMDFe(),
                _ => _oracleEmissorDocumentoMDFeInstance.Value
            };
        }

        private static void ConfigureWithANSTechCTeDefault()
        {
            lock (_lock)
            {
                _emissorDocumentoCTeType = TipoEmissorDocumento.NSTech;
                _currentEmissorDocumentoCTeInstance = CreateInstanceCTe();
            }
        }

        private static void ConfigureWithANSTechMDFeDefault()
        {
            lock (_lock)
            {
                _emissorDocumentoMDFeType = TipoEmissorDocumento.NSTech;
                _currentEmissorDocumentoMDFeInstance = CreateInstanceMDFe();
            }
        }

        private static void ConfigureWithOracleCTeDefault()
        {
            lock (_lock)
            {
                _emissorDocumentoCTeType = TipoEmissorDocumento.Integrador;
                _currentEmissorDocumentoCTeInstance = CreateInstanceCTe();
            }
        }

        private static void ConfigureWithOracleMDFeDefault()
        {
            lock (_lock)
            {
                _emissorDocumentoMDFeType = TipoEmissorDocumento.Integrador;
                _currentEmissorDocumentoMDFeInstance = CreateInstanceMDFe();
            }
        }

        #endregion
    }
}