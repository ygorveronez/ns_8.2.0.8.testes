using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Usuarios
{
    public class Usuario
    {
        #region Propriedades

        public string Nome { get; set; }
        public string CPF { get; set; }
        public string RG { get; set; }
        public string Telefone { get; set; }
        public DateTime DataNascimento { get; set; }
        public DateTime DataAdmissao { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime UltimoAcesso { get; set; }
        public string Email { get; set; }
        public decimal Salario { get; set; }
        public string Cidade { get; set; }
        public string Filial { get; set; }
        public string Endereco { get; set; }
        public string Complemento { get; set; }
        public string Login { get; set; }
        public bool AcessoSistema { get; set; }
        public string Status { get; set; }
        public string PerfilAcesso { get; set; }
        public DateTime DataUltimaAlteracaoSenha { get; set; }
        public string SituacaoSenha { get; set; }
        private Aposentadoria Aposentadoria { get; set; }
        public string Observacao { get; set; }
        private DateTime DataDemissao { get; set; }
        public string HoraInicialAcesso { get; set; }
        public string HoraFinalAcesso { get; set; }
        public bool UsuarioAdministrador { get; set; }
        public string Setor { get; set; }
        public string PermiteAuditarDescricao { get; set; }

        #endregion

        #region Propriedades com Regras

        public string DataNascimentoFormatada
        {
            get { return DataNascimento != DateTime.MinValue ? DataNascimento.ToString("dd/MM/yyyy") : ""; }
        }

        public string AposentadoriaFormatada
        {
            get { return Aposentadoria.ObterDescricao(); }
        }

        public string DataAdmissaoFormatada
        {
            get { return DataAdmissao != DateTime.MinValue ? DataAdmissao.ToString("dd/MM/yyyy") : ""; }
        }

        public string DataCadastroFormatada
        {
            get { return DataCadastro != DateTime.MinValue ? DataCadastro.ToString("dd/MM/yyyy HH:mm") : ""; }
        }

        public string UltimoAcessoFormatada
        {
            get { return UltimoAcesso != DateTime.MinValue ? UltimoAcesso.ToString("dd/MM/yyyy HH:mm") : ""; }
        }

        public string DataUltimaAlteracaoSenhaFormatada
        {
            get { return DataUltimaAlteracaoSenha != DateTime.MinValue ? DataUltimaAlteracaoSenha.ToString("dd/MM/yyyy HH:mm") : ""; }
        }

        public string AcessoSistemaFormatado
        {
            get
            {
                if (!AcessoSistema)
                    return "Liberado";
                else
                    return "Bloqueado";
            }
        }

        public string Situacao
        {
            get
            {
                if (Status == "A")
                    return "Ativo";
                else
                    return "Inativo";
            }
        }

        public string CPFFormatado
        {
            get { return !string.IsNullOrWhiteSpace(CPF) ? CPF.ObterCpfOuCnpjFormatado() : ""; }
        }

        public string Ambiente
        {
            get { return "TMS / Embarcador"; }
        }

        public string DataDemissaoFormatada
        {
            get { return DataDemissao != DateTime.MinValue ? DataDemissao.ToString("dd/MM/yyyy") : ""; }
        }

        public string UsuarioAdministradorFormatado
        {
            get
            {
                if (UsuarioAdministrador)
                    return "Sim";
                else
                    return "Não";
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
