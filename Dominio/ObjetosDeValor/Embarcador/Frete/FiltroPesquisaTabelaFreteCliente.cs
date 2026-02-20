using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    public sealed class FiltroPesquisaTabelaFreteCliente
    {
        #region Atributos

        private bool _codigoEmpresaInformado;
        private bool _codigoEstadoDestinoInformado;
        private bool _codigoEstadoOrigemInformado;
        private bool _codigoLocalidadeDestinoInformado;
        private bool _codigoLocalidadeOrigemInformado;
        private bool _codigoModeloTracaoInformado;
        private bool _codigoRegiaoDestinoInformado;
        private bool _codigoRotaFreteDestinoInformado;
        private bool _codigoRotaFreteOrigemInformado;
        private bool _codigoTipoCargaInformado;
        private bool _codigoTipoOperacaoInformado;
        private bool _cpfCnpjDestinatarioInformado;
        private bool _cpfCnpjRemetenteInformado;
        private bool _cpfCnpjTomadorInformado;

        #endregion

        #region Propriedades

        public TipoParametroBaseTabelaFrete? ParametroBase { get; set; }

        public int CodigoTabelaFrete { get; set; }

        public List<int> CodigosTabelasFreteCliente { get; set; }

        public int CodigoAjusteTabelaFrete { get; set; }
        public List<int> CodigosAjustesTabelaFrete { get; set; }

        public int CodigoContratoTransporteFrete { get; set; }

        public int CodigoGrupoPessoas { get; set; }

        public int CodigoLicitacaoParticipacao { get; set; }

        public List<int> CodigosContratoTransportador { get; set; }

        public int CodigoTabelaFreteCliente { get; set; }

        public int CodigoVigencia { get; set; }

        public List<int> CodigosComplemento { get; set; }

        public List<int> CodigosEmpresa { get; set; }

        public List<string> CodigosEstadoDestino { get; set; }

        public List<string> CodigosEstadoOrigem { get; set; }

        public List<int> CodigosLocalidadeDestino { get; set; }

        public List<int> CodigosLocalidadeOrigem { get; set; }

        public List<int> CodigosModeloReboque { get; set; }

        public List<int> CodigosModeloTracao { get; set; }

        public List<int> CodigosRegiaoDestino { get; set; }

        public List<int> CodigosRotaFreteDestino { get; set; }

        public List<int> CodigosRotaFreteOrigem { get; set; }

        public List<int> CodigosTipoCarga { get; set; }

        public List<int> CodigosTipoOperacao { get; set; }

        public double CpfCnpjTransportadorTerceiro { get; set; }

        public List<double> CpfCnpjDestinatarios { get; set; }

        public List<double> CpfCnpjRemetentes { get; set; }

        public List<double> CpfCnpjTomadores { get; set; }

        public DateTime? DataInicialAlteracao { get; set; }

        public DateTime? DataInicialVigencia { get; set; }

        public DateTime? DataFinalAlteracao { get; set; }

        public DateTime? DataFinalVigencia { get; set; }

        public bool ExibirHistoricoAlteracao { get; set; }

        public bool ExibirSomenteAguardandoAprovacao { get; set; }

        public bool IsRelatorio { get; set; }

        public bool IsCSV { get; set; }

        public bool TabelaComCargaRealizada { get; set; }

        public Enumeradores.TipoPagamentoEmissao? TipoPagamentoEmissao { get; set; }

        public Enumeradores.TipoRegistroAjusteTabelaFrete? TipoRegistro { get; set; }

        public bool UtilizarBuscaNasLocalidadesPorEstadoDestino { get; set; }

        public bool UtilizarBuscaNasLocalidadesPorEstadoOrigem { get; set; }

        public bool AjustarPedagiosComSemParar { get; set; }

        public int Vigencia { get; set; }

        public bool SomenteEmVigencia { get; set; }

        public SituacaoAlteracaoTabelaFrete? SituacaoAlteracao { get; set; }

        public int RotaFrete { get; set; }

        public int CEPOrigem { get; set; }

        public int CEPDestino { get; set; }

        public Enumeradores.PossuiRota PossuiRota { get; set; }

        public Enumeradores.SituacaoAtivoPesquisa? Ativo { get; set; }
        public Enumeradores.SituacaoAtivoPesquisa? SituacaoTabelaFrete { get; set; }

        public Enumeradores.TipoPagamentoEmissao? TipoPagamento { get; set; }

        public string CodigoIntegracao { get; set; }

        public Enumeradores.SituacaoAlteracaoTabelaFrete? SituacaoAlteracaoTabelaFrete { get; set; }

        public int CodigoCanalEntrega { get; set; }

        public int CodigoLocalidadeOrigemFiltro { get; set; }

        public int CodigoLocalidadeDestinoFiltro { get; set; }

        public int CodigoRegiaoOrigemFiltro { get; set; }
        public int CodigoRegiaoDestinoFiltro { get; set; }

        public int CodigoTipoOperacaoFiltro { get; set; }

        public int CodigoEmpresaFiltro { get; set; }

        public double CpfCnpjRemetenteFiltro { get; set; }

        public double CpfCnpjDestinatarioFiltro { get; set; }

        public double CpfCnpjTomadorFiltro { get; set; }

        public string EstadoDestino { get; set; }

        public string EstadoOrigem { get; set; }

        public int ContratoTransporteFrete { get; set; }

        public SituacaoIntegracaoTabelaFreteCliente? SituacaoIntegracaoTabelaFreteCliente { get; set; }

        public bool SomenteRegistrosComValores { get; set; }
        public bool ObterItemSomenteComValoresInformado { get; set; }

        public int CodigoStatusAceiteValor { get; set; }

        public List<int> CodigosRegiaoOrigem { get; set; }

        public List<int> CodigosCanaisEntrega { get; set; }

        public List<int> CodigosCanaisVenda { get; set; }

        public string CodigoIntegracaoTabelaFreteCliente { get; set; }

        #endregion

        #region Propriedades com Regras

        public int CodigoEmpresa
        {
            get
            {
                if (_codigoEmpresaInformado)
                    return CodigosEmpresa.FirstOrDefault();

                return 0;
            }
            set
            {
                if (value > 0)
                {
                    _codigoEmpresaInformado = true;
                    CodigosEmpresa = new List<int>() { value };
                }
            }
        }

        public string CodigoEstadoDestino
        {
            get
            {
                if (_codigoEstadoDestinoInformado)
                    return CodigosEstadoDestino.FirstOrDefault();

                return string.Empty;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && (value != "0"))
                {
                    _codigoEstadoDestinoInformado = true;
                    CodigosEstadoDestino = new List<string>() { value };
                }
            }
        }

        public string CodigoEstadoOrigem
        {
            get
            {
                if (_codigoEstadoOrigemInformado)
                    return CodigosEstadoOrigem.FirstOrDefault();

                return string.Empty;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && (value != "0"))
                {
                    _codigoEstadoOrigemInformado = true;
                    CodigosEstadoOrigem = new List<string>() { value };
                }
            }
        }

        public int CodigoLocalidadeDestino
        {
            get
            {
                if (_codigoLocalidadeDestinoInformado)
                    return CodigosLocalidadeDestino.FirstOrDefault();

                return 0;
            }
            set
            {
                if (value > 0)
                {
                    _codigoLocalidadeDestinoInformado = true;
                    CodigosLocalidadeDestino = new List<int>() { value };
                }
            }
        }

        public int CodigoLocalidadeOrigem
        {
            get
            {
                if (_codigoLocalidadeOrigemInformado)
                    return CodigosLocalidadeOrigem.FirstOrDefault();

                return 0;
            }
            set
            {
                if (value > 0)
                {
                    _codigoLocalidadeOrigemInformado = true;
                    CodigosLocalidadeOrigem = new List<int>() { value };
                }
            }
        }

        public int CodigoModeloTracao
        {
            get
            {
                if (_codigoModeloTracaoInformado)
                    return CodigosModeloTracao.FirstOrDefault();

                return 0;
            }
            set
            {
                if (value > 0)
                {
                    _codigoModeloTracaoInformado = true;
                    CodigosModeloTracao = new List<int>() { value };
                }
            }
        }

        public int CodigoRegiaoDestino
        {
            get
            {
                if (_codigoRegiaoDestinoInformado)
                    return CodigosRegiaoDestino.FirstOrDefault();

                return 0;
            }
            set
            {
                if (value > 0)
                {
                    _codigoRegiaoDestinoInformado = true;
                    CodigosRegiaoDestino = new List<int>() { value };
                }
            }
        }

        public int CodigoRotaFreteDestino
        {
            get
            {
                if (_codigoRotaFreteDestinoInformado)
                    return CodigosRotaFreteDestino.FirstOrDefault();

                return 0;
            }
            set
            {
                if (value > 0)
                {
                    _codigoRotaFreteDestinoInformado = true;
                    CodigosRotaFreteDestino = new List<int>() { value };
                }
            }
        }

        public int CodigoRotaFreteOrigem
        {
            get
            {
                if (_codigoRotaFreteOrigemInformado)
                    return CodigosRotaFreteOrigem.FirstOrDefault();

                return 0;
            }
            set
            {
                if (value > 0)
                {
                    _codigoRotaFreteOrigemInformado = true;
                    CodigosRotaFreteOrigem = new List<int>() { value };
                }
            }
        }

        public int CodigoTipoCarga
        {
            get
            {
                if (_codigoTipoCargaInformado)
                    return CodigosTipoCarga.FirstOrDefault();

                return 0;
            }
            set
            {
                if (value > 0)
                {
                    _codigoTipoCargaInformado = true;
                    CodigosTipoCarga = new List<int>() { value };
                }
            }
        }

        public int CodigoTipoOperacao
        {
            get
            {
                if (_codigoTipoOperacaoInformado)
                    return CodigosTipoOperacao.FirstOrDefault();

                return 0;
            }
            set
            {
                if (value > 0)
                {
                    _codigoTipoOperacaoInformado = true;
                    CodigosTipoOperacao = new List<int>() { value };
                }
            }
        }

        public double CpfCnpjDestinatario
        {
            get
            {
                if (_cpfCnpjDestinatarioInformado)
                    return CpfCnpjDestinatarios.FirstOrDefault();

                return 0d;
            }
            set
            {
                if (value > 0d)
                {
                    _cpfCnpjDestinatarioInformado = true;
                    CpfCnpjDestinatarios = new List<double>() { value };
                }
            }
        }

        public double CpfCnpjRemetente
        {
            get
            {
                if (_cpfCnpjRemetenteInformado)
                    return CpfCnpjRemetentes.FirstOrDefault();

                return 0d;
            }
            set
            {
                if (value > 0d)
                {
                    _cpfCnpjRemetenteInformado = true;
                    CpfCnpjRemetentes = new List<double>() { value };
                }
            }
        }

        public double CpfCnpjTomador
        {
            get
            {
                if (_cpfCnpjTomadorInformado)
                    return CpfCnpjTomadores.FirstOrDefault();

                return 0d;
            }
            set
            {
                if (value > 0d)
                {
                    _cpfCnpjTomadorInformado = true;
                    CpfCnpjTomadores = new List<double>() { value };
                }
            }
        }

        #endregion
    }
}
