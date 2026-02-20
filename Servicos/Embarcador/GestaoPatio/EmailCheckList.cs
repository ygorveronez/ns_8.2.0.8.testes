using Dominio.Entidades.Embarcador.Cargas.ControleEntrega;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using Servicos.Extensions;
using Dominio.ObjetosDeValor.Relatorios;

namespace Servicos.Embarcador.GestaoPatio
{
    /// <summary>
    /// Envia emails com o conteúdo de uma Checklist para Cliente e Motorista
    /// </summary>
    public sealed class EmailCheckList
    {
        private Repositorio.UnitOfWork unitOfWork;
        private Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado;

        #region Construtores

        public EmailCheckList(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            this.unitOfWork = unitOfWork;
            this.auditado = auditado;
        }

        #endregion
        #region Métodos públicos

        public void EnviarEmailCheckListSetoresTipoOperacao(int codigoCarga, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckList repCargaEntregaCheckList = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckList(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckList cargaEntregaCheckList = repCargaEntregaCheckList.BuscarPrimeiroPorCarga(codigoCarga, TipoCheckList.Desembarque);

            if (!(tipoOperacao?.ConfiguracaoControleEntrega?.EnviarBoletimViagemAoFinalizarViagem ?? false) || cargaEntregaCheckList == null)
                return;

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargasEntrega = repCargaEntrega.BuscarPorCarga(codigoCarga);

            Servicos.Embarcador.Carga.BoletimViagem servicoBoletimViagem = new Servicos.Embarcador.Carga.BoletimViagem(unitOfWork);

            Repositorio.Embarcador.Pedidos.TipoOperacao repConfiguracaoTipoOperacaoControleEntrega = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacaoControleEntregaSetor repTipoOperacaoControleEntregaSetor = new Repositorio.Embarcador.Pedidos.TipoOperacaoControleEntregaSetor(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoControleEntregaSetor> listaSetores = repTipoOperacaoControleEntregaSetor.BuscarPorConfiguracaoTipoOperacaoControleEntrega(tipoOperacao.ConfiguracaoControleEntrega?.Codigo ?? 0);

            byte[] buffer = ReportRequest.WithType(ReportType.BoletimViagemEmbarque)
            .WithExecutionType(ExecutionType.Sync)
            .AddExtraData("CodigoCarga", codigoCarga.ToString())
            .CallReport()
            .GetContentFile();

            MemoryStream stream = new MemoryStream(buffer);
            Attachment anexo = new Attachment(stream, "BoletimViagemEmbarque.pdf", "application/pdf");

            string nomeFilial = cargaEntregaCheckList.CargaEntrega?.Carga?.Filial?.Descricao;
            string descricaoCheckList = cargaEntregaCheckList.Descricao;
            string dataAtual = DateTime.Now.ToString("dd/MM/yyyy");
            string emailFilial = cargaEntregaCheckList.CargaEntrega?.Carga?.Filial?.Email;
            string listaPerguntasERespostas = ObterPerguntasERespostas(cargaEntregaCheckList);

            string assuntoEmail = $"{nomeFilial} - {descricaoCheckList} - {dataAtual}";

            string corpoEmail = "";
            corpoEmail += $"<h1>{assuntoEmail}</h1>";
            corpoEmail += "<h2>Perguntas e respostas:</h2>";
            corpoEmail += listaPerguntasERespostas;
            corpoEmail += $"<p>Qualquer divergência nas informações, retornar contato com a filial através do email {emailFilial}</p>";

            if ((tipoOperacao?.ConfiguracaoControleEntrega?.EnviarBoletimViagemAoFinalizarViagemParaTransportador ?? false) || (tipoOperacao?.ConfiguracaoControleEntrega?.EnviarBoletimViagemAoFinalizarViagemParaRemetente ?? false))
            {
                foreach (var cargaEntrega in cargasEntrega)
                {
                    if (!string.IsNullOrEmpty(cargaEntrega.Cliente?.Email))
                    {
                        Servicos.Email.EnviarEmailAutenticado(cargaEntrega.Cliente.Email, assuntoEmail, corpoEmail, unitOfWork, out string msgErro, null, new List<Attachment>() { anexo });

                        if (msgErro != "")
                        {
                            Servicos.Auditoria.Auditoria.Auditar(auditado, cargaEntregaCheckList, $"Erro ao enviar e-mail para ({cargaEntrega.Cliente.Email})", unitOfWork);
                        }
                        else
                        {
                            Servicos.Auditoria.Auditoria.Auditar(auditado, cargaEntregaCheckList, $"E-mail enviado com sucesso para ({cargaEntrega.Cliente.Email})", unitOfWork);
                        }
                    }

                }
            }

            foreach (var setor in listaSetores.Select(o => o.Setor))
            {
                List<Dominio.Entidades.Usuario> listaUsuarios = repUsuario.BuscarPorSetor(setor.Codigo);

                foreach (var usuario in listaUsuarios)
                {
                    if (!string.IsNullOrWhiteSpace(usuario.Email))
                    {
                        Servicos.Email.EnviarEmailAutenticado(usuario.Email, assuntoEmail, corpoEmail, unitOfWork, out string msgErro, null, new List<Attachment>() { anexo });

                        if (msgErro != "")
                        {
                            Servicos.Auditoria.Auditoria.Auditar(auditado, cargaEntregaCheckList, $"Erro ao enviar e-mail para o funcionário ({usuario.Email})", unitOfWork);
                        }
                        else
                        {
                            Servicos.Auditoria.Auditoria.Auditar(auditado, cargaEntregaCheckList, $"E-mail enviado com sucesso para o funcionário ({usuario.Email})", unitOfWork);
                        }
                    }
                }

            }
        }

        public void EnviarEmailCheckList(CargaEntregaCheckList cargaEntregaCheckList)
        {
            string nomeFilial = cargaEntregaCheckList.CargaEntrega?.Carga?.Filial?.Descricao;
            string descricaoCheckList = cargaEntregaCheckList.Descricao;
            string dataAtual = DateTime.Now.ToString("dd/MM/yyyy");
            string emailFilial = cargaEntregaCheckList.CargaEntrega?.Carga?.Filial?.Email;
            string listaPerguntasERespostas = ObterPerguntasERespostas(cargaEntregaCheckList);

            string assuntoEmail = $"{nomeFilial} - {descricaoCheckList} - {dataAtual}";

            string corpoEmail = "";
            corpoEmail += $"<h1>{assuntoEmail}</h1>";
            corpoEmail += "<h2>Perguntas e respostas:</h2>";
            corpoEmail += listaPerguntasERespostas;
            corpoEmail += $"<p>Qualquer divergência nas informações, retornar contato com a filial através do email {emailFilial}</p>";

            if (cargaEntregaCheckList.CheckListTipo?.EnviarEmailParaCliente == true || true)
            {
                string email = cargaEntregaCheckList.CargaEntrega?.Cliente?.Email;

                if (email != null && email != "")
                {
                    Servicos.Email.EnviarEmailAutenticado(email, assuntoEmail, corpoEmail, unitOfWork, out string msgErro, null);

                    if (msgErro != "")
                    {
                        Servicos.Auditoria.Auditoria.Auditar(auditado, cargaEntregaCheckList, $"Erro ao enviar e-mail para o cliente ({email})", unitOfWork);
                    }
                    else
                    {
                        Servicos.Auditoria.Auditoria.Auditar(auditado, cargaEntregaCheckList, $"E-mail enviado com sucesso para o cliente ({email})", unitOfWork);
                    }
                }
            }

            if (cargaEntregaCheckList.CheckListTipo?.EnviarEmailParaMotorista == true)
            {
                foreach (var motorista in cargaEntregaCheckList.CargaEntrega.Carga.Motoristas)
                {
                    if (motorista.Email != null && motorista.Email != "")
                    {
                        Servicos.Email.EnviarEmailAutenticado(motorista.Email, assuntoEmail, corpoEmail, unitOfWork, out string msgErro, null);

                        if (msgErro != "")
                        {
                            Servicos.Auditoria.Auditoria.Auditar(auditado, cargaEntregaCheckList, $"Erro ao enviar e-mail para o motorista ({motorista.Email})", unitOfWork);
                        }
                        else
                        {
                            Servicos.Auditoria.Auditoria.Auditar(auditado, cargaEntregaCheckList, $"E-mail enviado com sucesso para o motorista ({motorista.Email})", unitOfWork);
                        }
                    }
                }
            }

        }

        private string ObterPerguntasERespostas(CargaEntregaCheckList cargaEntregaCheckList)
        {
            string perguntasERespostas = "";

            var repCargaEntregaCheckListPergunta = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta(unitOfWork);
            List<CargaEntregaCheckListPergunta> perguntas = repCargaEntregaCheckListPergunta.BuscarPerguntasOrdenadasPorCargaEntregaCheckList(cargaEntregaCheckList.Codigo);

            Carga.ControleEntrega.CargaEntregaCheckList servicoCargaEntregaCheckList = new Carga.ControleEntrega.CargaEntregaCheckList(unitOfWork);

            foreach (var pergunta in perguntas)
            {
                string descricaoPergunta = pergunta.Descricao;
                string respostaPergunta = servicoCargaEntregaCheckList.ObterRespostaDescricaoPergunta(pergunta);

                perguntasERespostas += $"<p><b>{descricaoPergunta}</b><br>{respostaPergunta}</p>";
            }

            return perguntasERespostas;
        }
        #endregion
    }
}
