using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Anexo
{
    public sealed class ControleArquivo
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public ControleArquivo(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public bool EnviarEmailAlerta(Dominio.Entidades.Embarcador.Anexo.ControleArquivo controleArquivo)
        {
            Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(_unitOfWork);
            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repositorioConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(_unitOfWork);
            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repositorioConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo(0);
            Dominio.Entidades.Usuario usuario = repositorioUsuario.BuscarPorCliente(controleArquivo.Cliente.CPF_CNPJ, controleArquivo.Empresa.Codigo);

            if (email == null)
            {
                Log.TratarErro("Não há um e-mail configurado para realizar o envio.");
                return false;
            }

            try
            {
                string mensagemErro = "Erro ao enviar e-mail";
                string assunto = "Controle de arquivo de " + controleArquivo.Empresa.NomeFantasia + " - " + controleArquivo.Descricao;
                string mensagemEmail = string.Empty;

                if (controleArquivo.DataVencimento.HasValue && controleArquivo.DataVencimento.Value > DateTime.MinValue && controleArquivo.DataVencimento.Value.Date < DateTime.Now.Date)
                    assunto = "ARQUIVO CONTÁBIL/FISCAL VENCIDO! " + controleArquivo.Descricao;

                mensagemEmail = "Prezado(a) " + controleArquivo.Cliente.Nome + " ,<br/>";
                mensagemEmail += "Você recebeu um alerta de gerenciamento de arquivos contábeis/fiscais da empresa " + controleArquivo.Empresa.NomeFantasia + "<br/>";
                mensagemEmail += "<br/>";
                mensagemEmail += "Descrição: " + controleArquivo.Descricao + "<br/>";

                if (controleArquivo.DataVencimento.HasValue && controleArquivo.DataVencimento.Value > DateTime.MinValue)
                    mensagemEmail += "Vencimento: " + controleArquivo.DataVencimento.Value.ToString("dd/MM/yyyy") + "<br/>";

                if (!string.IsNullOrWhiteSpace(controleArquivo.Observacao))
                    mensagemEmail += "Observação: " + controleArquivo.Observacao + "<br/>";

                mensagemEmail += "<br/>";
                mensagemEmail += "Favor acessar o link do portal https://nfe.commerce.inf.br/Login com o seguinte dado de acesso:<br/>";
                mensagemEmail += "<br/>";
                mensagemEmail += "Usuário: " + usuario.Login + "<br/>";
                mensagemEmail += "Senha: " + usuario.Senha + "<br/>";
                mensagemEmail += "<br/>";
                mensagemEmail += "Qualquer dúvida, favor entrar em contato com a contabilidade gerenciadora do(s) arquivo(s):" + "<br/>";
                mensagemEmail += "" + controleArquivo.Empresa.RazaoSocial + "<br/>";
                mensagemEmail += "Telefone: " + controleArquivo.Empresa.Telefone + "<br/>";
                mensagemEmail += "E-mail: " + controleArquivo.Empresa.Email + "<br/>";
                mensagemEmail += "<br/>";
                mensagemEmail += "<br/>";
                mensagemEmail += " * ** E-MAIL ENVIADO AUTOMATICAMENTE PELO NOSSO SISTEMA. FAVOR NÃO RESPONDER ***";

                List<string> emails = new List<string>();

                if (!string.IsNullOrWhiteSpace(controleArquivo.Cliente.Email))
                    emails.AddRange(controleArquivo.Cliente.Email.Split(';').ToList());

                if (!string.IsNullOrWhiteSpace(controleArquivo.Empresa.Email))
                    emails.AddRange(controleArquivo.Empresa.Email.Split(';').ToList());

                if (!string.IsNullOrWhiteSpace(controleArquivo.Empresa.EmailContador))
                    emails.AddRange(controleArquivo.Empresa.EmailContador.Split(';').ToList());

                foreach (var outroEmail in controleArquivo.Cliente.Emails)
                {
                    if (!string.IsNullOrWhiteSpace(outroEmail.Email) && outroEmail.EmailStatus == "A" && outroEmail.TipoEmail != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmail.Administrativo)
                        emails.Add(outroEmail.Email);
                }

                emails = emails.Distinct().ToList();

                if (emails.Count > 0)
                {
                    bool sucesso = Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, null, emails.ToArray(), null, assunto, mensagemEmail, email.Smtp, out mensagemErro, email.DisplayEmail, null, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, _unitOfWork);

                    if (!sucesso)
                    {
                        Log.TratarErro("Problemas ao enviar o cte para o provedor por e-mail: " + mensagemErro);
                        return false;
                    }
                    return true;
                }
                else
                {
                    Log.TratarErro("Nenhum e-mail configurado para enviar o alerta.");
                    return false;
                }

            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                return false;
            }
        }

        #endregion
    }
}
