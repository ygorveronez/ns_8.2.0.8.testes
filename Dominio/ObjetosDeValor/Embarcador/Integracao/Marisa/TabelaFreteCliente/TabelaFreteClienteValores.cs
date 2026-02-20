namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Marisa.TabelaFreteCliente
{
    public sealed class TabelaFreteClienteValores
    {
        #region Propriedades

        public string CepDestinoFinal { get; set; }

        public string CepDestinoInicial { get; set; }

        public int CepDestinoPrazoDiasUteis { get; set; }

        public string CodigoIntegracaoTabelaFrete { get; set; }

        public decimal FatorCubagem { get; set; }

        public decimal PesoFinal { get; set; }

        public decimal PesoInicial { get; set; }

        public decimal ValorAdValorem { get; set; }

        public decimal ValorGris { get; set; }

        public decimal ValorPeso { get; set; }

        public decimal ValorPesoExcedente { get; set; }

        public Enumeradores.TipoCampoValorTabelaFrete TipoValorAdValorem { get; set; }

        public Enumeradores.TipoCampoValorTabelaFrete TipoValorGris { get; set; }

        #endregion Propriedades

        #region Propriedades com Regras

        public string Ativo
        {
            get
            {
                return "1";
            }
        }

        public string FatorCubagemDescricao
        {
            get
            {
                return FatorCubagem.ToString("n3");
            }
        }

        public string CepDestinoPrazoDiasUteisDescricao
        {
            get
            {
                return CepDestinoPrazoDiasUteis.ToString();
            }
        }

        public string ParametroTabelaFrete
        {
            get
            {
                return "Rodoviario";
            }
        }

        public string PercentualIcmsDescricao
        {
            get
            {
                return "12";
            }
        }

        public string PercentualAdValoremDescricao
        {
            get
            {
                return (TipoValorAdValorem == Enumeradores.TipoCampoValorTabelaFrete.AumentoPercentual ? ValorAdValorem : 0m).ToString("n3");
            }
        }

        public string PercentualGrisDescricao
        {
            get
            {
                return (TipoValorGris == Enumeradores.TipoCampoValorTabelaFrete.AumentoPercentual ? ValorGris : 0m).ToString("n3");
            }
        }

        public string PesoFinalDescricao
        {
            get
            {
                return PesoFinal.ToString("n3");
            }
        }

        public string PesoInicialDescricao
        {
            get
            {
                return PesoInicial.ToString("n3");
            }
        }

        public string ValorAdValoremDescricao
        {
            get
            {
                return ((TipoValorAdValorem == Enumeradores.TipoCampoValorTabelaFrete.AumentoValor) || (TipoValorAdValorem == Enumeradores.TipoCampoValorTabelaFrete.ValorFixo) || (TipoValorAdValorem == Enumeradores.TipoCampoValorTabelaFrete.ValorFixoArredondadoParaCima) ? ValorAdValorem : 0m).ToString("n3");
            }
        }

        public string ValorGrisDescricao
        {
            get
            {
                return ((TipoValorGris == Enumeradores.TipoCampoValorTabelaFrete.AumentoValor) || (TipoValorGris == Enumeradores.TipoCampoValorTabelaFrete.ValorFixo) || (TipoValorGris == Enumeradores.TipoCampoValorTabelaFrete.ValorFixoArredondadoParaCima) ? ValorGris: 0m).ToString("n3");
            }
        }

        public string ValorPesoDescricao
        {
            get
            {
                return ValorPeso.ToString("n3");
            }
        }

        public string ValorPesoExcedenteDescricao
        {
            get
            {
                return ValorPesoExcedente.ToString("n3");
            }
        }

        #endregion Propriedades com Regras
    }
}
