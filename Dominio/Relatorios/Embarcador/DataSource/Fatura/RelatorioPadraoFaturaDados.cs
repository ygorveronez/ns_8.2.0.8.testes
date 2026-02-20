using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Fatura
{
    public class RelatorioPadraoFaturaDados
    {
        #region Propriedades

        public int Codigo { get; set; }
        public int Numero { get; set; }
        public long NumeroOriginal { get; set; }
        public decimal Valor { get; set; }
        public decimal ValorLiquido { get; set; }
        public DateTime DataVencimento { get; set; }
        public int Sequencia { get; set; }
        public decimal ValorTitulo { get; set; }
        public decimal ValorDesconto { get; set; }
        public string MotivoDesconto { get; set; }
        public decimal ValorAcrescimo { get; set; }
        public string MotivoAcrescimo { get; set; }
        public string NomePessoa { get; set; }
        public string EnderecoPessoa { get; set; }
        public string BairroPessoa { get; set; }
        public string CEPPessoa { get; set; }
        public string EmailPessoa { get; set; }
        public string NumeroEnderecoPessoa { get; set; }
        public string CidadePessoa { get; set; }
        public string EstadoPessoa { get; set; }
        public double CNPJPessoa { get; set; }
        public string NumeroCUITRUIT { get; set; }
        public string IEPessoa { get; set; }
        public string NomeGrupo { get; set; }
        public string CNPJRaizGrupo { get; set; }
        public string Observacao { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public string ObservacaoFatura { get; set; }
        public bool ImprimirObservacaoFatura { get; set; }
        public string RazaoEmpresa { get; set; }
        public string FantasiaEmpresa { get; set; }
        public string EnderecoEmpresa { get; set; }
        public string BairroEmpresa { get; set; }
        public string NumeroEnderecoEmpresa { get; set; }
        public string CEPEmpresa { get; set; }
        public string CNPJEmpresa { get; set; }
        public string IEEmpresa { get; set; }
        public string FoneEmpresa { get; set; }
        public string CidadeEmpresa { get; set; }
        public string EstadoEmpresa { get; set; }
        public string ComplementoEmpresa { get; set; }
        public string NomeBanco { get; set; }
        public int NumeroBanco { get; set; }
        public string AgenciaBanco { get; set; }
        public string DigitoAgenciaBanco { get; set; }
        public string NumeroContaBanco { get; set; }
        public int TipoContaBanco { get; set; }
        public string ObservacaoFaturaCliente { get; set; }
        public string ObservacaoFaturaGrupo { get; set; }
        public long NumeroPreFatura { get; set; }
        public DateTime DataEmissao { get; set; }
        public string NumeroTitulos { get; set; }
        public bool ExibirTitulos { get; set; }

        public decimal ValorMoedaCotacao { get; set; }
        public decimal TotalMoedaEstrangeira { get; set; }
        public decimal ParcelaMoedaEstrangeira { get; set; }

        public int TipoProposta { get; set; }
        public DateTime DataBaseCRT { get; set; }
        private MoedaCotacaoBancoCentral MoedaCotacaoBancoCentral { get; set; }
        private TipoImpressaoFatura TipoImpressaoFatura { get; set; }

        public string Prostesto { get; set; }
        public decimal Juros { get; set; }
        public decimal Multa { get; set; }
        private string TipoPessoa { get; set; }
        public string DescricaoServico { get; set; }

        #endregion

        #region Propriedades com Regras

        public string CNPJPessoaFormatado
        {
            get
            {
                if (TipoPessoa == "J")
                    return CNPJPessoa.ToString().ObterCnpjFormatado();
                else if (TipoPessoa == "F")
                    return CNPJPessoa.ToString().ObterCpfFormatado();
                else
                    return "";
            }
        }
        public string ValorPorExtenso
        {
            get
            {
                bool tomadorBrasileiro = TipoPessoa == "J" || TipoPessoa == "F";

                decimal valor = MoedaCotacaoBancoCentral == MoedaCotacaoBancoCentral.Real
                ? (TipoImpressaoFatura == TipoImpressaoFatura.ParcelasSeparadas ? ValorTitulo : ValorLiquido)
                : (TipoImpressaoFatura == TipoImpressaoFatura.ParcelasSeparadas ? ParcelaMoedaEstrangeira : TotalMoedaEstrangeira);

                string moeda = MoedaCotacaoBancoCentral.ObterDescricaoSimplificada();

                if (tomadorBrasileiro)
                {
                    bool traduzirMoeda = MoedaCotacaoBancoCentral != MoedaCotacaoBancoCentral.Real;
                    return Utilidades.Conversor.DecimalToWordsReal(valor, traduzirMoeda);
                }
                else
                    return Utilidades.Conversor.DecimalToWords(valor, moeda);
            }
        }

        public int TipoMoeda
        {
            get { return (int)MoedaCotacaoBancoCentral; }
        }

        public string DescricaoMoedaCotacaoBancoCentral
        {
            get { return MoedaCotacaoBancoCentral.ObterDescricao(); }
        }

        public string SiglaMoeda
        {
            get { return MoedaCotacaoBancoCentral.ObterSigla(); }
        }

        public string DescricaoDataBaseCRT
        {
            get { return DataBaseCRT > DateTime.MinValue ? DataBaseCRT.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string CNPJEmpresaFormatado
        {
            get { return CNPJEmpresa.ObterCnpjFormatado(); }
        }

        #endregion
    }
}
