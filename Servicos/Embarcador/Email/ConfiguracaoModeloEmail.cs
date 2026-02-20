using Dominio.Entidades.Embarcador.Logistica;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;

namespace Servicos.Embarcador.Email
{
    public class ConfiguracaoModeloEmail
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly Dominio.Entidades.Usuario _usuario;
        private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;

        #endregion

        #region Construtores

        public ConfiguracaoModeloEmail(string stringConexao, Dominio.Entidades.Usuario usuario, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            _usuario = usuario;
            _auditado = auditado;
            _unitOfWork = new Repositorio.UnitOfWork(stringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.AtualizarAtual);
        }

        #endregion

        #region Métodos Públicos

        public void EnviarEmails(AgendamentoColeta agendamentoColeta, TipoGatilhoNotificacao tipoGatilhoNotificacao)
        {
            if (agendamentoColeta == null)
                return;

            try
            {
                //Busca movamente para ficar na mesma sessão
                agendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(_unitOfWork).BuscarPorCodigo(agendamentoColeta.Codigo);

                List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoModeloEmail> configuracoeModelosEmails = ObterConfiguracoesModelosEmails(tipoGatilhoNotificacao);

                if (configuracoeModelosEmails.Count == 0)
                    return;

                foreach (Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoModeloEmail configuracaoModeloEmail in configuracoeModelosEmails)
                    ConfiguraEmails(agendamentoColeta, configuracaoModeloEmail);
            }
            catch (Exception excecao)
            {
                Log.TratarErro($"Erro ao enviar e-mails tipo {tipoGatilhoNotificacao.ObterDescricao()}: {excecao.Message}", "ConfiguracaoModeloEmail");
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private List<Attachment> ObterZipAnexos(Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoModeloEmailAnexo anexo)
        {
            if (anexo == null || !Utilidades.IO.FileStorageService.Storage.Exists(anexo.CaminhoArquivo))
                return null;

            Dictionary<string, byte[]> listaAnexos = new Dictionary<string, byte[]>();

            byte[] buffer = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(anexo.CaminhoArquivo);

            listaAnexos.Add($"{anexo.NomeArquivo}", buffer);

            return new List<System.Net.Mail.Attachment>() { new Attachment(Utilidades.File.GerarArquivoCompactado(listaAnexos), "Anexos.zip") };
        }

        private List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoModeloEmail> ObterConfiguracoesModelosEmails(TipoGatilhoNotificacao tipoGatilhoNotificacao)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoModeloEmail repositorioConfiguracaoModeloEmail = new Repositorio.Embarcador.Configuracoes.ConfiguracaoModeloEmail(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoModeloEmail> configuracoeModelosEmails = repositorioConfiguracaoModeloEmail.BuscarPorTipoGatilhoNotificacao(tipoGatilhoNotificacao);

            return configuracoeModelosEmails;
        }

        private void ConfiguraEmails(AgendamentoColeta agendamentoColeta, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoModeloEmail configuracaoModeloEmail)
        {
            try
            {
                string email = ObterEmail(agendamentoColeta, configuracaoModeloEmail);

                if (string.IsNullOrWhiteSpace(email))
                    return;

                EnviarEmail(email, agendamentoColeta, configuracaoModeloEmail);
            }
            catch (Exception ex)
            {
                Log.TratarErro($"Erro ao enviar e-mails tipo: {configuracaoModeloEmail.TipoGatilhoNotificacao.ObterDescricao()}, para {configuracaoModeloEmail.TipoEnviarPara.ObterDescricao()}", "ConfiguracaoModeloEmail");
                Log.TratarErro(ex.Message, "ConfiguracaoModeloEmail");
            }

        }

        private void EnviarEmail(string email, AgendamentoColeta agendamentoColeta, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoModeloEmail configuracaoModeloEmail)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoModeloEmailAnexo repositorioConfiguracaoModeloEmailAnexo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoModeloEmailAnexo(_unitOfWork);

            Servicos.Email svcEmail = new Servicos.Email(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoModeloEmailAnexo anexos = repositorioConfiguracaoModeloEmailAnexo.BuscarPorModeloEmail(configuracaoModeloEmail.Codigo);

            List<Attachment> attachments = ObterZipAnexos(anexos);

            string corpo = BuscarCorpoEmail(agendamentoColeta, configuracaoModeloEmail);

            if (!svcEmail.EnviarEmail(string.Empty, string.Empty, string.Empty, email, string.Empty, string.Empty, configuracaoModeloEmail.Assunto, corpo, string.Empty, attachments, string.Empty, false, string.Empty, 0, _unitOfWork))
                Log.TratarErro($"Falha ao enviar o e-mail - Servico ConfiguracaoModeloEmail", "ConfiguracaoModeloEmail");
        }

        private string BuscarCorpoEmail(AgendamentoColeta agendamentoColeta, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoModeloEmail configuracaoModeloEmail)
        {
            List<Dominio.ObjetosDeValor.Email.TagValorAgendamento> tagValor = ObterTags(configuracaoModeloEmail.Corpo);

            Servicos.Embarcador.Logistica.AgendamentoEntregaPedido servicoAgendamentoEntregaPedido = new Servicos.Embarcador.Logistica.AgendamentoEntregaPedido(_unitOfWork, _auditado);

            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repositorioCargaPedido.BuscarPedidosPorCarga(agendamentoColeta.Carga.Codigo);

            string corpoEmail = servicoAgendamentoEntregaPedido.MontarEmail(tagValor, pedidos, new List<Dominio.Entidades.Embarcador.Cargas.Carga> { agendamentoColeta.Carga }, configuracaoModeloEmail, _unitOfWork);

            return corpoEmail;
        }

        private List<Dominio.ObjetosDeValor.Email.TagValorAgendamento> ObterTags(string tamplate)
        {
            List<Dominio.ObjetosDeValor.Email.TagValorAgendamento> tagValor = new List<Dominio.ObjetosDeValor.Email.TagValorAgendamento>
            {
                new Dominio.ObjetosDeValor.Email.TagValorAgendamento(tag: "#TagStatus"),
                new Dominio.ObjetosDeValor.Email.TagValorAgendamento(tag: "#TagDataColeta"),
                new Dominio.ObjetosDeValor.Email.TagValorAgendamento(tag: "#TagRazaoSocialRecebedor"),
                new Dominio.ObjetosDeValor.Email.TagValorAgendamento(tag: "#TagCNJPRecebedor"),
                new Dominio.ObjetosDeValor.Email.TagValorAgendamento(tag: "#TagEnderecoRecebedor"),
                new Dominio.ObjetosDeValor.Email.TagValorAgendamento(tag: "#TagNumeroPedido"),
                new Dominio.ObjetosDeValor.Email.TagValorAgendamento(tag: "#TagNumeroPedidoCliente"),
                new Dominio.ObjetosDeValor.Email.TagValorAgendamento(tag: "#TagNumeroNotaFiscal"),
                new Dominio.ObjetosDeValor.Email.TagValorAgendamento(tag: "#TagCarga"),
                new Dominio.ObjetosDeValor.Email.TagValorAgendamento(tag: "#TagDataAgendamento"),
                new Dominio.ObjetosDeValor.Email.TagValorAgendamento(tag: "#TagRazaoSocialRemetente"),
                new Dominio.ObjetosDeValor.Email.TagValorAgendamento(tag: "#TagCNPJRemetente"),
                new Dominio.ObjetosDeValor.Email.TagValorAgendamento(tag: "#TagEnderecoRemetente"),
                new Dominio.ObjetosDeValor.Email.TagValorAgendamento(tag: "#TagComplementoRemetente"),
                new Dominio.ObjetosDeValor.Email.TagValorAgendamento(tag: "#TagBairroRemetente"),
                new Dominio.ObjetosDeValor.Email.TagValorAgendamento(tag: "#TagCidadeUFRemetente"),
                new Dominio.ObjetosDeValor.Email.TagValorAgendamento(tag: "#TagTelefoneRemetente"),
                new Dominio.ObjetosDeValor.Email.TagValorAgendamento(tag: "#TagRazaoSocialDestinatario"),
                new Dominio.ObjetosDeValor.Email.TagValorAgendamento(tag: "#TagCNPJDestinatario"),
                new Dominio.ObjetosDeValor.Email.TagValorAgendamento(tag: "#TagEnderecoDestinatario"),
                new Dominio.ObjetosDeValor.Email.TagValorAgendamento(tag: "#TagComplementoDestinatario"),
                new Dominio.ObjetosDeValor.Email.TagValorAgendamento(tag: "#TagBairroDestinatario"),
                new Dominio.ObjetosDeValor.Email.TagValorAgendamento(tag: "#TagCidadeUFDestinatario"),
                new Dominio.ObjetosDeValor.Email.TagValorAgendamento(tag: "#TagTelefoneDestinatario"),
                new Dominio.ObjetosDeValor.Email.TagValorAgendamento(tag: "#TagRazaoSocialTransportador"),
                new Dominio.ObjetosDeValor.Email.TagValorAgendamento(tag: "#TagCNPJTransportador"),
                new Dominio.ObjetosDeValor.Email.TagValorAgendamento(tag: "#TagEnderecoTransportador"),
                new Dominio.ObjetosDeValor.Email.TagValorAgendamento(tag: "#TagComplementoTransportador"),
                new Dominio.ObjetosDeValor.Email.TagValorAgendamento(tag: "#TagBairroTransportador"),
                new Dominio.ObjetosDeValor.Email.TagValorAgendamento(tag: "#TagCidadeUFTransportador"),
                new Dominio.ObjetosDeValor.Email.TagValorAgendamento(tag: "#TagTelefoneTransportador"),
                new Dominio.ObjetosDeValor.Email.TagValorAgendamento(tag: "#TagDataSugestaoEntrega"),
                new Dominio.ObjetosDeValor.Email.TagValorAgendamento(tag: "#TagCodigoIntegracaoFilial"),
                new Dominio.ObjetosDeValor.Email.TagValorAgendamento(tag: "#TagTipoCarga"),
                new Dominio.ObjetosDeValor.Email.TagValorAgendamento(tag: "#TagCodigoIntegracaoDestinatarioPedido"),
                new Dominio.ObjetosDeValor.Email.TagValorAgendamento(tag: "#TagCanalClientes"),
                new Dominio.ObjetosDeValor.Email.TagValorAgendamento(tag: "#TagQtdVolumesCarga"),
                new Dominio.ObjetosDeValor.Email.TagValorAgendamento(tag: "#TagSenhaEntregaAgendamento"),
                new Dominio.ObjetosDeValor.Email.TagValorAgendamento(tag: "#TagDataHoraAgendamentoColeta"),
                new Dominio.ObjetosDeValor.Email.TagValorAgendamento(tag: "#TagSenhaAgendamentoColeta")
            };

            return tagValor.Where(tag => tamplate.Contains(tag.Tag)).ToList();
        }

        private string ObterEmail(AgendamentoColeta agendamentoColeta, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoModeloEmail configuracaoModeloEmail)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            return configuracaoModeloEmail.TipoEnviarPara switch
            {
                TipoEnviarPara.Transportador => agendamentoColeta.Carga.Empresa?.Email ?? string.Empty,
                TipoEnviarPara.Fornecedor => repositorioCargaPedido.BuscarEmailPrimeiroClientePorCarga(agendamentoColeta.Carga.Codigo, destinatariosDaCarga: false),
                TipoEnviarPara.OperadorME => _usuario?.Email ?? string.Empty,
                _ => string.Empty,
            };
        }

        #endregion
    }
}
