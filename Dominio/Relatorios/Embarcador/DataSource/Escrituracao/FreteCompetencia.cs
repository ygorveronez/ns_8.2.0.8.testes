using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Escrituracao
{
    public class FreteCompetencia
    {
        public DateTime DataEmissao { get; set; }

        public string DataEmissaoFormatada
        {
            get { return DataEmissao != DateTime.MinValue ? DataEmissao.ToString("dd/MM/yyyy") : ""; }
        }

        public string Filial { get; set; }

        public string CNPJTransportador { get; set; }

        public string NomeTransportador { get; set; }

        public string Transportador
        {
            get { return $"{String.Format(@"{0:00\.000\.000\/0000\-00}", CNPJTransportador.ToLong())} - {NomeTransportador}"; }
        }

        public double CNPJTomador { get; set; }

        public string TipoTomador { get; set; }

        public string NomeTomador { get; set; }

        public string Tomador
        {
            get { return $"{CNPJTomador.ToString().ObterCpfOuCnpjFormatado(TipoTomador)} - {NomeTomador}"; }
        }

        public string Origem { get; set; }

        public string Destino { get; set; }

        public string TipoOperacao { get; set; }

        public string Carga { get; set; }

        public DateTime DataEmissaoCarga { get; set; }

        public string DataEmissaoCargaFormatada
        {
            get { return DataEmissaoCarga != DateTime.MinValue ? DataEmissaoCarga.ToString("dd/MM/yyyy") : ""; }
        }

        public int _Ocorrencia { get; set; }

        public string Ocorrencia
        {
            get { return _Ocorrencia > 0 ? _Ocorrencia.ToString() : ""; }
        }

        public DateTime DataEmissaoOcorrencia { get; set; }

        public string DataEmissaoOcorrenciaFormatada
        {
            get { return DataEmissaoOcorrencia != DateTime.MinValue ? DataEmissaoOcorrencia.ToString("dd/MM/yyyy") : ""; }
        }

        public int Pagamento { get; set; }

        public int Provisao { get; set; }

        public int CancelamentoProvisao { get; set; }

        public string NumeroValePedagio { get; set; }

        public int _NumeroNFS { get; set; }

        public string NumeroNFS
        {
            get { return _NumeroNFS > 0 ? _NumeroNFS.ToString() : ""; }
        }

        public decimal PesoBruto { get; set; }

        public string Rota { get; set; }

        public DateTime DataEmissaoNFsManual { get; set; }

        public string DataEmissaoNFsManualFormatada
        {
            get { return DataEmissaoNFsManual != DateTime.MinValue ? DataEmissaoNFsManual.ToString("dd/MM/yyyy") : ""; }
        }

        public decimal ValorFrete { get; set; }

        public decimal ValorFreteSemICMS
        {
            get { return ValorFrete - ICMS; }
        }

        public decimal Aliquota { get; set; }

        public decimal AliquotaISS { get; set; }

        public string CST { get; set; }

        public decimal _ICMS { get; set; }

        public decimal ICMS
        {
            get { return CST != "60" ? _ICMS : 0; }
        }

        public decimal ICMSRetido
        {
            get { return CST == "60" ? _ICMS : 0; }
        }

        public decimal _ValorISS { get; set; }

        public decimal ValorISSRetido { get; set; }

        public decimal ValorISS
        {
            get { return _ValorISS - ValorISSRetido; }
        }

        public decimal ValorPIS
        {
            get { return Math.Round(_ValorTotalPrestacao * (_AliquotaPIS / 100), 2, MidpointRounding.AwayFromZero); }
        }

        public decimal ValorCOFINS
        {
            get { return Math.Round(_ValorTotalPrestacao * (_AliquotaCOFINS / 100), 2, MidpointRounding.AwayFromZero); }
        }

        public double CNPJRemetente { get; set; }

        public string TipoRemetente { get; set; }

        public string NomeRemetente { get; set; }

        public string Remetente
        {
            get { return $"{CNPJRemetente.ToString().ObterCpfOuCnpjFormatado(TipoRemetente)} - {NomeRemetente}"; }
        }

        public double CNPJDestinatario { get; set; }

        public string TipoDestinatario { get; set; }

        public string NomeDestinatario { get; set; }

        public string Destinatario
        {
            get { return $"{CNPJDestinatario.ToString().ObterCpfOuCnpjFormatado(TipoDestinatario)} - {NomeDestinatario}"; }
        }

        public string ModeloDocumento { get; set; }

        public int _NumeroCte { get; set; }

        public string NumeroCte
        {
            get { return _NumeroCte > 0 ? _NumeroCte.ToString() : ""; }
        }

        public string TipoCarga { get; set; }

        public string Placa { get; set; }

        public string PlacaFormatada
        {
            get { return string.IsNullOrWhiteSpace(Placa) ? string.Empty : $"{Placa.Substring(0, 3)}-{Placa.Substring(3, 4)}"; }
        }

        public string ModeloVeicular { get; set; }

        public DateTime DataPagamento { get; set; }

        public string DataPagamentoFormatada
        {
            get { return DataPagamento != DateTime.MinValue ? DataPagamento.ToString("dd/MM/yyyy") : ""; }
        }

        public decimal _AliquotaPIS { get; set; }

        public decimal _AliquotaCOFINS { get; set; }

        public bool _ICMSInclusoBC { get; set; }

        public bool _ISSInclusoBC { get; set; }

        public Dominio.Enumeradores.TipoDocumento _TipoDocumentoEmissao { get; set; }

        public decimal _ValorTotalPrestacao
        {
            get
            {
                if ((_TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS) || (_TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe))
                    return _ISSInclusoBC ? (ValorFrete + _ValorISS) : (ValorFrete + ValorISSRetido);

                return _ICMSInclusoBC ? (ValorFrete + ICMS) : ValorFrete;
            }
        }

        private DateTime DataAprovacaoPagamento { get; set; }
        private DateTime DataDigitalizacaoCanhoto { get; set; }

        public int NumeroNotaFiscalServico { get; set; }

        public int NumeroOcorrencia { get; set; }

        public string IDAgrupador { get; set; }

        public double CPFCNPJRemetente { get; set; }

        public string CPFCNPJRemetenteFormatado
        {
            get { return CPFCNPJRemetente.ToString().ObterCpfOuCnpjFormatado(); }
        }

        public DateTime DataEmissaoNotaFiscalServicoManual { get; set; }

        public string DataEmissaoNotaFiscalServicoManualFormatada
        {
            get { return DataEmissaoNotaFiscalServicoManual != DateTime.MinValue ? DataEmissaoNotaFiscalServicoManual.ToString("dd/MM/yyyy") : ""; }
        }

        #region Propriedades com Regras

        public string DataAprovacaoPagamentoFormatada
        {
            get { return DataAprovacaoPagamento != DateTime.MinValue ? DataAprovacaoPagamento.ToDateTimeString() : ""; }
        }

        public string DataDigitalizacaoCanhotoFormatada
        {
            get { return DataDigitalizacaoCanhoto != DateTime.MinValue ? DataDigitalizacaoCanhoto.ToDateTimeString() : ""; }
        }

        #endregion
    }
}
