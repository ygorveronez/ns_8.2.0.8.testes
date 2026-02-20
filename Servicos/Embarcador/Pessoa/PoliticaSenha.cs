using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Pessoa
{
    public class PoliticaSenha
    {
        public string AplicarPoliticaSenha(ref Dominio.Entidades.Usuario usuario, Dominio.Entidades.Embarcador.Usuarios.PoliticaSenha politicaSenha, Repositorio.UnitOfWork unitOfWork)
        {
            string retorno = "";
            if (politicaSenha != null && politicaSenha.HabilitarPoliticaSenha)
            {
                string senha = usuario.Senha;
                if (politicaSenha.NumeroMinimoCaracteresSenha > 0 && senha.Length < politicaSenha.NumeroMinimoCaracteresSenha)
                    return "A senha deve conter ao mínimo " + politicaSenha.NumeroMinimoCaracteresSenha + " caracteres.";

                if (politicaSenha.ExigirSenhaForte)
                {
                    bool aprovou = true;
                    if (senha.Length == Utilidades.String.RemoveAllSpecialCharacters(senha).Length)
                        aprovou = false;

                    if (Utilidades.String.OnlyNumbers(senha).Length <= 0)
                        aprovou = false;

                    if (!senha.Any(c => char.IsUpper(c)))
                        aprovou = false;

                    if (!aprovou)
                        return "A senha deve conter ao menos 1 (um) caractere especial (Ex: @ # $ !), 1 (um) número e 1 (uma) letra maiúscula (ex: U)";
                }

                string senhaSHA256 = "", senhaMD5 = "";

                if (politicaSenha.HabilitarCriptografia)
                {
                    senhaSHA256 = Servicos.Criptografia.GerarHashSHA256(senha);
                    senhaMD5 = Servicos.Criptografia.GerarHashMD5(senha);
                    usuario.SenhaCriptografada = true;
                }
                else
                    usuario.SenhaCriptografada = false;

                if (politicaSenha.QuantasSenhasAnterioresNaoRepetir > 0)
                {
                    Repositorio.Embarcador.Usuarios.FuncionarioSenhaAnterior repFuncionarioSenhaAnterior = new Repositorio.Embarcador.Usuarios.FuncionarioSenhaAnterior(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Usuarios.FuncionarioSenhaAnterior> senhasAnteriores = repFuncionarioSenhaAnterior.BuscarPorUsuario(usuario.Codigo, 0, politicaSenha.QuantasSenhasAnterioresNaoRepetir);
                    foreach (Dominio.Entidades.Embarcador.Usuarios.FuncionarioSenhaAnterior senhaAnterior in senhasAnteriores)
                    {
                        if ((senha == senhaAnterior.Senha && !senhaAnterior.SenhaCriptografada) || ((senhaSHA256 == senhaAnterior.Senha || senhaMD5 == senhaAnterior.Senha) && senhaAnterior.SenhaCriptografada))
                        {
                            if (politicaSenha.QuantasSenhasAnterioresNaoRepetir > 1)
                                return "A nova senha não pode ser igual as últimas " + politicaSenha.QuantasSenhasAnterioresNaoRepetir + " senhas utilizadas.";
                            else
                                return "A nova senha não pode ser igual a última senha utilizada.";
                        }
                    }
                }

                usuario.Senha = politicaSenha.HabilitarCriptografia ? senhaSHA256 : senha;
            }
            return retorno;
        }

        public bool SenhaEstaDeAcordo(string senha, Dominio.Entidades.Embarcador.Usuarios.PoliticaSenha politicaSenha, out string erro)
        {
            if (politicaSenha == null)
            {
                erro = "";
                return true;
            }

            if (politicaSenha.NumeroMinimoCaracteresSenha > 0)
            {
                if (senha.Length < politicaSenha.NumeroMinimoCaracteresSenha)
                {
                    erro = "A senha deve conter ao mínimo " + politicaSenha.NumeroMinimoCaracteresSenha + " caracteres.";
                    return false;
                }
            }

            if (politicaSenha.ExigirSenhaForte)
            {
                bool aprovou = true;
                if (senha.Length == Utilidades.String.RemoveAllSpecialCharacters(senha).Length)
                    aprovou = false;

                if (Utilidades.String.OnlyNumbers(senha).Length <= 0)
                    aprovou = false;

                if (!senha.Any(c => char.IsUpper(c)))
                    aprovou = false;

                if (!aprovou)
                {
                    erro = "A senha deve conter ao menos 1 (um) caractere especial (Ex: @ # $ !), 1 (um) número e 1 (uma) letra maiúscula (ex: U)";
                    return false;
                }
            }

            erro = "";
            return true;
        }

        public Dominio.Entidades.Embarcador.Usuarios.PoliticaSenha BuscarPoliticaSenha(Repositorio.UnitOfWork unidadeDeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoAcesso)
        {
            Repositorio.Embarcador.Usuarios.PoliticaSenha repPoliticaSenha = new Repositorio.Embarcador.Usuarios.PoliticaSenha(unidadeDeTrabalho);

            Dominio.Entidades.Embarcador.Usuarios.PoliticaSenha politicaSenha = repPoliticaSenha.BuscarPoliticaPadraoPorServicoMultiSoftware(tipoServicoAcesso);

            if (politicaSenha == null && (tipoServicoAcesso == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador || tipoServicoAcesso == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || tipoServicoAcesso == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe || tipoServicoAcesso == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin))
                politicaSenha = repPoliticaSenha.BuscarPoliticaPadraoNull();

            return politicaSenha;
        }
    }
}
