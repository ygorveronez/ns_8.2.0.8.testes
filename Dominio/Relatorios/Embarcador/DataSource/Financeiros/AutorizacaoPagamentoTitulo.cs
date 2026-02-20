using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Financeiros
{
    public class AutorizacaoPagamentoTitulo
    {
        public int Codigo { get; set; }
        public DateTime DataEmissao { get; set; }
        public DateTime DataVencimento { get; set; }
        public DateTime DataLiquidacao { get; set; }

        public string FantasiaEmpresa { get; set; }
        public string CNPJEmpresa { get; set; }
        public string TipoEmpresa { get; set; }
        public string CodigoIntegracaoEmpresa { get; set; }

        public string NomePessoa { get; set; }
        public double CNPJPessoa { get; set; }
        public string TipoPessoa { get; set; }
        public string CodigoIntegracaoPessoa { get; set; }

        public int Sequencia { get; set; }
        public decimal Valor { get; set; }
        public decimal ValorPago { get; set; }
        public decimal Desconto { get; set; }
        public decimal Acrescimo { get; set; }
        public string Observacao { get; set; }
        public string TipoDocumentoTituloOriginal { get; set; }
        public string NumeroDocumentoTituloOriginal { get; set; }
        public string TipoMovimento { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral Moeda { get; set; }
        public decimal ValorMoeda { get; set; }
        public decimal ValorOriginalMoeda { get; set; }


        public string Usuario { get; set; }
        public string NomePortador { get; set; }
        public double CNPJPortador { get; set; }
        public string TipoPortador { get; set; }
        public string RuaPessoa { get; set; }
        public string NumeroPessoa { get; set; }
        public string BairroPessoa { get; set; }
        public string ComplementoPessoa { get; set; }
        public string CidadePessoa { get; set; }
        public string UFPessoa { get; set; }
        public string Banco { get; set; }
        public string Agencia { get; set; }
        public string DigitoAgencia { get; set; }
        public string NumeroConta { get; set; }


        public virtual string DescricaoMoeda
        {
            get
            {
                switch (this.Moeda)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.DolarCompra:
                        return "Dolar Compra";
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.DolarVenda:
                        return "Dolar Venda";
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Guarani:
                        return "Guarani";
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.PesoArgentino:
                        return "Peso Argentino";
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.PesoChileno:
                        return "peso Chileno";
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.PesoUruguaio:
                        return "Peso Uruguaio";
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real:
                        return "Real";
                    default:
                        return "Real";
                }
            }
        }

        public string DataEmissaoFormatada
        {
            get { return DataEmissao != DateTime.MinValue ? DataEmissao.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DataVencimentoFormatada
        {
            get { return DataVencimento != DateTime.MinValue ? DataVencimento.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DataLiquidacaoFormatada
        {
            get { return DataLiquidacao != DateTime.MinValue ? DataLiquidacao.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string Empresa
        {
            get
            {
                return !string.IsNullOrWhiteSpace(FantasiaEmpresa) ? (!string.IsNullOrWhiteSpace(CodigoIntegracaoEmpresa) ? CodigoIntegracaoEmpresa + " - " : string.Empty) +
                  FantasiaEmpresa + " - " + CNPJEmpresa.ObterCpfOuCnpjFormatado(TipoEmpresa) : string.Empty;
            }
        }

        public string Pessoa
        {
            get
            {
                return (!string.IsNullOrWhiteSpace(CodigoIntegracaoPessoa) ? CodigoIntegracaoPessoa + " - " : string.Empty) +
                  NomePessoa + " - " + CNPJPessoa.ToString().ObterCpfOuCnpjFormatado(TipoPessoa);
            }
        }

        public string Portador
        {
            get
            {
                return NomePortador + " - " + CNPJPortador.ToString().ObterCpfOuCnpjFormatado(TipoPortador);
            }
        }
    }
}
