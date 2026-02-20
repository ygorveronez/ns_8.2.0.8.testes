using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Enumerador;

namespace Dominio.Relatorios.Embarcador.DataSource.Frota
{
    public class Motorista
    {
        #region Propriedades

        public int Codigo { get; set; }
        public string Nome { get; set; }
        public string UsuarioMobile { get; set; }
        public string NaoBloquearAcessoSimultaneo { get; set; }
        public string CodigoIntegracao { get; set; }
        public string CPF { get; set; }
        public string RG { get; set; }
        public DateTime DataNascimento { get; set; }
        public string CNH { get; set; }
        public DateTime ValidadeCNH { get; set; }
        public DateTime ValidadeSeguradora { get; set; }
        public string Telefone { get; set; }
        public string Endereco { get; set; }
        public string CEP { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string TipoMotorista { get; set; }
        public string Situacao { get; set; }
        public string FichaAtiva { get; set; }
        public string GerarComissao { get; set; }
        public string Banco { get; set; }
        public string Agencia { get; set; }
        public string Digito { get; set; }
        public string NumeroConta { get; set; }
        public string NumeroCartao { get; set; }
        public string Filial { get; set; }
        public string Transportadora { get; set; }
        public int TipoConta { get; set; }
        public string CNPJTransportador { get; set; }
        public string Veiculo { get; set; }
        public decimal SaldoDiaria { get; set; }
        public decimal SaldoAdiantamento { get; set; }
        public DateTime DataEmissaoCNH { get; set; }
        public string NumeroProntuario { get; set; }
        public DateTime DataEmissaoRG { get; set; }
        public OrgaoEmissorRG EmissorRG { get; set; }
        public string CategoriaCNH { get; set; }
        public string Celular { get; set; }
        public string PISPASEP { get; set; }
        public DateTime DataAdmissao { get; set; }
        public string RenachCNH { get; set; }
        public int DiasTrabalhado { get; set; }
        public int DiasFolgaRetirado { get; set; }
        public string CentroResultado { get; set; }
        public string Cargo { get; set; }
        public string NumeroFrota { get; set; }
        private Aposentadoria Aposentado { get; set; }
        public string CargoMotorista { get; set; }
        public string MotivoBloqueio { get; set; }
        public bool Bloqueado { get; set; }
        public Escolaridade Escolaridade { get; set; }
        public CorRaca CorRaca { get; set; }
        public EstadoCivil EstadoCivil { get; set; }
        public string EstadoRG { get; set; }
        public string NumRegistroCNH { get; set; }
        public DateTime DataPrimeiraCNH { get; set; }
        public string Email { get; set; }
        public string Bairro { get; set; }
        public string Numero { get; set; }
        public TipoLogradouro TipoLogradouro { get; set; }
        public string TituloEleitoral { get; set; }
        public string ZonaEleitoral { get; set; }
        public string SecaoEleitoral { get; set; }
        public DateTime DataExpedicaoCTPS { get; set; }
        public string EstadoExpedicaoCTPS { get; set; }
        public string LocalidadeNascimento { get; set; }
        public TipoParentesco TipoParentesco { get; set; }
        public string PessoaAgregado { get; set; }
        public string ComplementoEndereco { get; set; }
        public string NumeroCTPS { get; set; }
        public string SerieCTPS { get; set; }
        public string DadosBancarios { get; set; }
        public string ContatoNenhum { get; set; }
        public string ContatoOutro { get; set; }
        public string ContatoPai { get; set; }
        public string ContatoMae { get; set; }
        public string ContatoFilhos { get; set; }
        public string ContatoIrmao { get; set; }
        public string ContatoAvo { get; set; }
        public string ContatoNeto { get; set; }
        public string ContatoTio { get; set; }
        public string ContatoSobrinho { get; set; }
        public string ContatoBisavo { get; set; }
        public string ContatoBisneto { get; set; }
        public string ContatoPrimo { get; set; }
        public string ContatoTrisavo { get; set; }
        public string ContatoTrineto { get; set; }
        public string ContatoTipoAvo { get; set; }
        public string ContatoSobrinhoNeto { get; set; }
        public string ContatoEsposo { get; set; }
        private string LocalidadeMunicipioCNH { get; set; }
        private string LocalidadeEstadoCNH { get; set; }
        public string CodigoSegurancaCNH { get; set; }
        public string Gestor { get; set; }
        public TipoResidencia TipoResidencia { get; set; }

        #endregion

        #region Propriedades com Regras

        public string BloqueadoDescricao
        {
            get { return Bloqueado ? "Sim" : "Não"; }
        }

        public string LocalidadeMunicipioEstadoCNH {

            get
            {
                return LocalidadeMunicipioCNH != null ? LocalidadeMunicipioCNH + "/" + LocalidadeEstadoCNH : null;
            }
        }

        public string AposentadoriaFormatada
        {
            get { return Aposentado.ObterDescricao(); }
        }

        public string EscolaridadeFormatada
        {
            get { return Escolaridade.ObterDescricao(); }
        }

        public string CorRacaFormatada
        {
            get { return CorRaca.ObterDescricao(); }
        }
        public string EstadoCivilFormatada
        {
            get { return EstadoCivil.ObterDescricao(); }
        }

        public string TipoLogradouroFormatada
        {
            get { return TipoLogradouro.ObterDescricao(); }
        }

        public string TipoResidenciaFormatada
        {
            get { return TipoResidencia.ObterDescricao(); }
        }

        public string EmissorRGDescricao
        {
            get { return EmissorRG.ObterDescricao(); }
        }

        public string TipoParentescoFormatada
        {
            get { return TipoParentesco.ObterDescricao(); }
        }

        public string DataEmissaoRGFormatada
        {
            get { return DataEmissaoRG != DateTime.MinValue ? DataEmissaoRG.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DataAdmissaoFormatada
        {
            get { return DataAdmissao != DateTime.MinValue ? DataAdmissao.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DataNascimentoFormatada
        {
            get { return DataNascimento != DateTime.MinValue ? DataNascimento.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DataValidadeCNHFormatada
        {
            get { return ValidadeCNH != DateTime.MinValue ? ValidadeCNH.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DataValidadeSeguradoraFormatada
        {
            get { return ValidadeSeguradora != DateTime.MinValue ? ValidadeSeguradora.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DataEmissaoCNHFormatada
        {
            get { return DataEmissaoCNH != DateTime.MinValue ? DataEmissaoCNH.ToString("dd/MM/yyyy") : string.Empty; }
        }
        public string DataPrimeiraCNHFormatada
        {
            get { return DataPrimeiraCNH != DateTime.MinValue ? DataPrimeiraCNH.ToString("dd/MM/yyyy") : string.Empty; }
        }
        public string DataExpedicaoCTPSFormatada
        {
            get { return DataExpedicaoCTPS != DateTime.MinValue ? DataExpedicaoCTPS.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public virtual string TipoContaFormatada
        {
            get { return TipoConta.ToString().ToNullableEnum<TipoContaBanco>().ObterDescricaoAbreviada(); }
        }
        public string CNPJTransportadorFormatado
        {
            get
            {
                return this.CNPJTransportador.ObterCnpjFormatado();
            }
        }


        #endregion

        #region Colunas Dinâmicas

        public int QuantidadeEPI1 { get; set; }
        public int QuantidadeEPI2 { get; set; }
        public int QuantidadeEPI3 { get; set; }
        public int QuantidadeEPI4 { get; set; }
        public int QuantidadeEPI5 { get; set; }
        public int QuantidadeEPI6 { get; set; }
        public int QuantidadeEPI7 { get; set; }
        public int QuantidadeEPI8 { get; set; }
        public int QuantidadeEPI9 { get; set; }
        public int QuantidadeEPI10 { get; set; }
        public int QuantidadeEPI11 { get; set; }
        public int QuantidadeEPI12 { get; set; }
        public int QuantidadeEPI13 { get; set; }
        public int QuantidadeEPI14 { get; set; }
        public int QuantidadeEPI15 { get; set; }
        public int QuantidadeEPI16 { get; set; }
        public int QuantidadeEPI17 { get; set; }
        public int QuantidadeEPI18 { get; set; }
        public int QuantidadeEPI19 { get; set; }
        public int QuantidadeEPI20 { get; set; }

        #endregion
    }
}
