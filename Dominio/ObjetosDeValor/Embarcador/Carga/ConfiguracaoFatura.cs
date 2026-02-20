using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class ConfiguracaoFatura
    {
        public string CodigoIntegracaoGrupoPessoa { get; set; }
        public string CodigoIntegracaoPessoa { get; set; }
        public List<ObjetosDeValor.Embarcador.Enumeradores.DiaSemana> DiasSemanaFatura { get; set; }
        public List<int> DiasMesFatura { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.TipoAgrupamentoFatura? TipoAgrupamentoFatura { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioFatura? TipoEnvioFatura { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.TipoPrazoFaturamento? TipoPrazoFaturamento { get; set; }
        public int DiasDePrazoFatura { get; set; }
        public bool GerarFaturamentoAVista { get; set; }
        public bool GerarTituloAutomaticamente { get; set; }
        public bool GerarTituloPorDocumentoFiscal { get; set; }
        public bool PermiteFinalDeSemana { get; set; }
        public bool ExigeCanhotoFisico { get; set; }
        public bool ArmazenaCanhotoFisicoCTe { get; set; }
        public bool AgregadoSomenteOcorrenciasFinalizadoras { get; set; }
        public bool FaturarSomenteOcorrenciasFinalizadoras { get; set; }
        public bool GerarBoletoAutomaticamente { get; set; }
        public bool EnviarArquivosDescompactados { get; set; }
        public int NumeroBanco { get; set; }
        public string NumeroAgencia { get; set; }
        public string DigitoBanco { get; set; }
        public string NumeroConta { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco TipoContaBanco { get; set; }
        public string CNPJTomadorFatura { get; set; }
        public string ObservacaoFatura { get; set; }

        public bool AtualizarAssuntoEmailFatura { get; set; }        
        public string AssuntoEmailFatura { get; set; }
        public bool AtualizarCorpoEmailFatura { get; set; }
        public string CorpoEmailFatura { get; set; }
        public bool AtualizarEmailFatura { get; set; }
        public string EmailFatura { get; set; }
        public decimal ValorMaximoEmissaoPendentePagamento { get; set; }

        #region Frete Cabotagem
        public bool CabotagemGerarFaturamentoAVista { get; set; }
        public int CabotagemDiasDePrazoFatura { get; set; }
        public Enumeradores.TipoPrazoFaturamento CabotagemTipoPrazoFaturamento { get; set; }
        public List<Enumeradores.DiaSemana> CabotagemDiasSemanaFatura { get; set; }
        public List<int> CabotagemDiasMesFatura { get; set; }
        public bool AtualizarCabotagemEmail { get; set; }
        public string CabotagemEmail { get; set; }
        #endregion

        #region Frete Longo Curso        
        public bool LongoCursoGerarFaturamentoAVista { get; set; }
        public int LongoCursoDiasDePrazoFatura { get; set; }
        public Enumeradores.TipoPrazoFaturamento LongoCursoTipoPrazoFaturamento { get; set; }
        public List<Enumeradores.DiaSemana> LongoCursoDiasSemanaFatura { get; set; }
        public List<int> LongoCursoDiasMesFatura { get; set; }
        public bool AtualizarLongoCursoEmail { get; set; }
        public string LongoCursoEmail { get; set; }
        #endregion

        #region Custo Extra
        public bool CustoExtraGerarFaturamentoAVista { get; set; }
        public int CustoExtraDiasDePrazoFatura { get; set; }
        public Enumeradores.TipoPrazoFaturamento CustoExtraTipoPrazoFaturamento { get; set; }
        public List<Enumeradores.DiaSemana> CustoExtraDiasSemanaFatura { get; set; }
        public List<int> CustoExtraDiasMesFatura { get; set; }
        public bool AtualizarCustoExtraEmail { get; set; }
        public string CustoExtraEmail { get; set; }
        #endregion
        public bool InativarCadastro { get; set; }
    }
}
