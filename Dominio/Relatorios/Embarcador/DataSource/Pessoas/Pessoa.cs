using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Relatorios.Embarcador.DataSource.Pessoas
{
    public class Pessoa
    {
        #region Propriedades

        public double CPFCNPJ { get; set; }
        private string TipoPessoa { get; set; }
        public string Categoria { get; set; }
        public string CodigoIntegracao { get; set; }
        public string IE { get; set; }
        public string RazaoSocial { get; set; }
        public string NomeFantasia { get; set; }
        public string Atividade { get; set; }
        public string GrupoPessoas { get; set; }
        public string TelefonePrincipal { get; set; }
        public string TelefoneSecundario { get; set; }
        public string CEP { get; set; }
        public string Endereco { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string UF { get; set; }
        public string Complemento { get; set; }
        public string Numero { get; set; }
        public string Email { get; set; }
        public string Observacao { get; set; }
        public string PortadorConta { get; set; }
        public string Banco { get; set; }
        public string Agencia { get; set; }
        public string Digito { get; set; }
        public string NumeroConta { get; set; }
        private int TipoConta { get; set; }
        private string Latitude { get; set; }
        private string Longitude { get; set; }
        public string PISPASEP { get; set; }
        private DateTime DataNascimento { get; set; }
        private int IndicadorIE { get; set; }
        public string CodigoDocumento { get; set; }
        public string Bloqueado { get; set; }
        private DateTime DataBloqueio { get; set; }
        public string AguardandoConferenciaInformacao { get; set; }
        public TipoPrazoFaturamento tipoPrazoFaturamento { get; set; }
        public FormaGeracaoTituloFatura formaGeracaoTituloFatura { get; set; }
        public TipoEnvioFatura tipoEnvioFatura { get; set; }
        public string DiasPrazoFaturamento { get; set; }
        public string DiaSemana { get; set; }
        public string DiaMes { get; set; }
        public string ContaContabil { get; set; }

        public EstadoCivil EstadoCivil { get; set; }
        public Dominio.ObjetosDeValor.Enumerador.Sexo Sexo { get; set; }
        public string TipoFornecedor { get; set; }
        public string CodigoCategoriaTrabalhador { get; set; }
        public string Funcao { get; set; }
        public string PagamentoEmBanco { get; set; }
        public string FormaPagamentoeSocial { get; set; }
        public string BancoDOC { get; set; }
        public string TipoAutonomo { get; set; }
        public string CodigoReceita { get; set; }
        public string TipoPagamentoBancario { get; set; }
        public string NaoDescontaIRRF { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.RegimeTributario RegimeTributario { get; set; }
        public string RNTRC { get; set; }
        public int Raio { get; set; }
        private int ExigeAgendamento { get; set; }
        private TipoArea TipoArea { get; set; }
        public string Modalidade { get; set; }
        public string Filiais { get; set; }
        private TipoModalidade TipoClienteIntegracao { get; set; }
        private DateTime DataIntegracao { get; set; }
        public string Pais { get; set; }
        public string TipoTerceiro { get; set; }

        public string Vendedor { get; set; }

        #endregion

        #region Propriedades com Regras

        public virtual string SexoFormatado
        {
            get { return this.Sexo == Dominio.ObjetosDeValor.Enumerador.Sexo.Feminino ? "Feminino" : this.Sexo == Dominio.ObjetosDeValor.Enumerador.Sexo.Masculino ? "Masculino" : "Não informado"; }
        }
        public virtual string EstadoCivilFormatado
        {
            get { return this.EstadoCivil.ObterDescricao(); }
        }
        public virtual string TipoPrazoFaturamentoFormatado
        {
            get { return tipoPrazoFaturamento.ObterDescricao(); }
        }
        public virtual string FormaGeracaoTituloFaturaFormatado
        {
            get { return formaGeracaoTituloFatura.ObterDescricao(); }
        }
        public virtual string TipoEnvioFaturaFormatado
        {
            get { return tipoEnvioFatura.ObterDescricao(); }
        }

        public virtual string CPFCNPJFormatado
        {
            get
            {
                if (TipoPessoa == "E")
                    return "00.000.000/0000-00";
                else
                    return TipoPessoa == "J" ? string.Format(@"{0:00\.000\.000\/0000\-00}", CPFCNPJ) : string.Format(@"{0:000\.000\.000\-00}", CPFCNPJ);
            }
        }

        public virtual string DataBloqueioFormatada
        {
            get { return DataBloqueio != DateTime.MinValue ? DataBloqueio.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public virtual string LongitudeFormatada
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Longitude) || Longitude == "NaN")
                    return "";
                else
                    return Utilidades.Decimal.Converter(this.Longitude).ToString("n7");
            }
        }

        public virtual string LatitudeFormatada
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Latitude) || Latitude == "NaN")
                    return "";
                else
                    return Utilidades.Decimal.Converter(this.Latitude).ToString("n7");
            }
        }

        public virtual string TipoContaFormatada
        {
            get { return TipoConta.ToString().ToNullableEnum<TipoContaBanco>().ObterDescricaoAbreviada(); }
        }

        public string DataNascimentoFormatada
        {
            get { return DataNascimento != DateTime.MinValue ? DataNascimento.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public virtual string IndicadorIEFormatado
        {
            get { return IndicadorIE.ToString().ToNullableEnum<IndicadorIE>().ObterDescricaoAbreviada(); }
        }

        public virtual string RegimeTributarioFormatado
        {
            get { return RegimeTributario.ObterDescricao(); }
        }

        public string TipoAreaDescricao
        {
            get { return TipoArea.ObterDescricao(); }
        }

        public string ExigeAgendamentoDescricao
        {
            get { return ExigeAgendamento > 0 ? "Sim" : "Não"; }
        }

        public string TipoClienteIntegracaoFormatado
        {
            get { return this.TipoClienteIntegracao.ObterDescricao(); }
        }

        public virtual string DataIntegracaoFormatada
        {
            get { return DataIntegracao != DateTime.MinValue ? DataIntegracao.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        #endregion
    }
}
