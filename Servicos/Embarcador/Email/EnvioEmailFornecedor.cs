using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace Servicos.Embarcador.Email
{
    public class EnvioEmailFornecedor
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public EnvioEmailFornecedor(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public void EnviarEmailFornecedores(Dominio.Entidades.Embarcador.Email.EmailGlobalizadoFornecedor emailFornecedor)
        {
            try
            {
                Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
                Repositorio.Embarcador.Email.EmailGlobalizadoFornecedor repEmailGlobalizadoFornecedor = new Repositorio.Embarcador.Email.EmailGlobalizadoFornecedor(_unitOfWork);
                Repositorio.Embarcador.Email.ConfigEmailDocTransporte repositorioConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(_unitOfWork);
                Repositorio.Embarcador.Email.EmailGlobalizadoFornecedorCliente repEmailGlobalizadoFornecedorCliente = new Repositorio.Embarcador.Email.EmailGlobalizadoFornecedorCliente(_unitOfWork);

                Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configuracaoEmail = repositorioConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo(0);
                string assunto = emailFornecedor.Descricao;
                string mensagem = emailFornecedor.CorpoEmail;

                List<Attachment> attachments = ObterZipAnexos(emailFornecedor.Anexos.ToList(), _unitOfWork);


                List<Dominio.Entidades.Cliente> clientes = new List<Dominio.Entidades.Cliente>();
                if (emailFornecedor.EnviarTodosFornecedores)
                    clientes = repCliente.BuscarTodosFornecedoresAtivos();
                else
                    clientes = repEmailGlobalizadoFornecedorCliente.BuscarClientePorEmailGlobalizado(emailFornecedor.Codigo);

                List<string> emailsFornecedoresAgrupados = new List<string>();
                StringBuilder sb = new StringBuilder();
                string mensagemErro;

                foreach (Dominio.Entidades.Cliente cliente in clientes)
                {
                    List<string> emails = new List<string>();

                    emails = cliente.Email.Split(';').ToList();

                    Servicos.Email.EnviarEmail(configuracaoEmail.Email, configuracaoEmail.Email, configuracaoEmail.Senha, null, emails.ToArray(), null, assunto, mensagem, configuracaoEmail.Smtp, out mensagemErro, configuracaoEmail.DisplayEmail, attachments, "", configuracaoEmail.RequerAutenticacaoSmtp, "", configuracaoEmail.PortaSmtp, _unitOfWork);
                }

                emailFornecedor.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioEmail.Enviado;
                repEmailGlobalizadoFornecedor.Atualizar(emailFornecedor);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        #endregion

        #region Métodos Privados

        private byte[] ObterArquivo(string guidArquivo, string extensao, Repositorio.UnitOfWork unitOfWork)
        {
            string caminho = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "EmailGlobalizadoFornecedor" });
            string nomeArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, $"{guidArquivo}.{extensao}");
            byte[] arquivoBinario = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(nomeArquivo);

            return arquivoBinario;
        }

        private List<Attachment> ObterZipAnexos(List<Dominio.Entidades.Embarcador.Email.EmailGlobalizadoFornecedorAnexo> anexos, Repositorio.UnitOfWork unitOfWork)
        {
            if (anexos.Count <= 0)
                return null;

            Dictionary<string, byte[]> listaAnexos = new Dictionary<string, byte[]>();

            foreach (Dominio.Entidades.Embarcador.Email.EmailGlobalizadoFornecedorAnexo anexo in anexos)
            {
                byte[] buffer = ObterArquivo(anexo.GuidArquivo, anexo.ExtensaoArquivo, unitOfWork);
                listaAnexos.Add($"{anexo.Descricao}.{anexo.ExtensaoArquivo}", buffer);
            }

            return new List<System.Net.Mail.Attachment>() { new System.Net.Mail.Attachment(Utilidades.File.GerarArquivoCompactado(listaAnexos), "Anexos.zip") };
        }

        #endregion
    }
}
