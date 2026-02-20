using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;

namespace Servicos.Embarcador.Compras
{
    public class OrdemCompra
    {
        #region Atributos Privados Somente Leitura

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public OrdemCompra(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public void EnviarEmailOrdemCompra(Dominio.Entidades.Embarcador.Compras.OrdemCompra ordemCompra, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            bool tipoMultiNFe = tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe;

            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(_unitOfWork);
            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo(tipoMultiNFe ? ordemCompra.Empresa.Codigo : 0);

            if (email == null)
                return;

            string assunto = $"Ordem de Compra nº { ordemCompra.Numero}";
            if (tipoMultiNFe)
                assunto += " da " + ordemCompra.Empresa.RazaoSocial;
            string mensagemEmail = "Olá " + ordemCompra.Fornecedor.Nome + "<br/>";
            mensagemEmail += "<br/>Segue a ordem de compra.";

            mensagemEmail += "<br/><br/>E-mail enviado automaticamente. Por favor, não responda.";
            if (!string.IsNullOrWhiteSpace(email.MensagemRodape))
                mensagemEmail += "<br/>" + "<br/>" + "<br/>" + email.MensagemRodape.Replace("#qLinha#", "<br/>");
            string mensagemErro = "Erro ao enviar e-mail";

            List<string> emails = new List<string>();
            if (!string.IsNullOrWhiteSpace(ordemCompra.Fornecedor.Email))
                emails.AddRange(ordemCompra.Fornecedor.Email.Split(';').ToList());

            foreach (Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEmail outroEmail in ordemCompra.Fornecedor.Emails)
            {
                if (!string.IsNullOrWhiteSpace(outroEmail.Email) && outroEmail.EmailStatus == "A")
                    emails.Add(outroEmail.Email);
            }

            bool sucesso = false;
            List<System.Net.Mail.Attachment> attachments = new List<System.Net.Mail.Attachment>();
            var ordemCompraImpressao = GerarImpressaoOrdemCompra(ordemCompra);
            if (ordemCompraImpressao != null)
            {
                attachments.Add(new System.Net.Mail.Attachment(new System.IO.MemoryStream(ordemCompraImpressao), "Ordem de Compra - " + ordemCompra.Numero + ".pdf", "application/pdf"));

                emails = emails.Distinct().ToList();
                if (emails.Count == 0)
                    throw new ServicoException($"Fornecedor {ordemCompra.Fornecedor.Nome} não possui e-mail cadastrado para envio da ordem de compra.");

                sucesso = Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, null, emails.ToArray(), null, assunto, mensagemEmail, email.Smtp, out mensagemErro, email.DisplayEmail, attachments, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, _unitOfWork, tipoMultiNFe ? ordemCompra.Empresa.Codigo : 0);
            }

            if (!sucesso)
            {
                Log.TratarErro($"Problemas ao enviar a ordem de compra por e-mail ao fornecedor { ordemCompra.Fornecedor.Nome }: " + mensagemErro);
                throw new ServicoException($"Problemas ao enviar a ordem de compra por e-mail ao fornecedor { ordemCompra.Fornecedor.Nome }.", Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.FalhaAoEnviarEmail);
            }
        }

        public byte[] GerarImpressaoOrdemCompra(Dominio.Entidades.Embarcador.Compras.OrdemCompra ordemCompra)
        {
            return ReportRequest.WithType(ReportType.OrdemCompra)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("CodigoOrdemCompra", ordemCompra.Codigo.ToString())
                .CallReport()
                .GetContentFile();
        }
        #endregion

        #region Métodos Públicos Estáticos

        public static void OrdemCompraAprovada(Dominio.Entidades.Embarcador.Compras.OrdemCompra ordemCompra, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (ordemCompra.Situacao != SituacaoOrdemCompra.Aprovada)
                return;

            OrdemCompra servicoOrdemCompra = new OrdemCompra(unitOfWork);

            try
            {
                servicoOrdemCompra.EnviarEmailOrdemCompra(ordemCompra, tipoServicoMultisoftware);
            }
            catch (ServicoException excecao)
            {
                if (excecao.ErrorCode != Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.FalhaAoEnviarEmail)
                    Log.TratarErro(excecao.Message);
            }
        }

        public static bool FinalizarOrdemCompra(int codigoDocumentoEntrada, out string mensagemErro, out bool contemQuantidadePendente, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS repDocumentoEntrada = new Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.DocumentoEntradaItem repItemDocumentoEntrada = new Repositorio.Embarcador.Financeiro.DocumentoEntradaItem(unidadeTrabalho);
            Repositorio.Embarcador.Compras.OrdemCompra repOrdemCompra = new Repositorio.Embarcador.Compras.OrdemCompra(unidadeTrabalho);
            Repositorio.Embarcador.Compras.OrdemCompraMercadoria repOrdemCompraMercadoria = new Repositorio.Embarcador.Compras.OrdemCompraMercadoria(unidadeTrabalho);
            Repositorio.Embarcador.Compras.FluxoCompra repFluxoCompra = new Repositorio.Embarcador.Compras.FluxoCompra(unidadeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoEntrada repConfiguracaoDocumentoEntrada = new Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoEntrada(unidadeTrabalho);

            contemQuantidadePendente = false;

            Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada = repDocumentoEntrada.BuscarPorCodigo(codigoDocumentoEntrada);
              
            if (documentoEntrada.Situacao != SituacaoDocumentoEntrada.Finalizado || documentoEntrada.OrdemCompra == null)
            {
                mensagemErro = null;
                return true;
            }

            Dominio.Entidades.Embarcador.Compras.OrdemCompra ordemCompra = repOrdemCompra.BuscarPorCodigo(documentoEntrada.OrdemCompra.Codigo);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentoEntrada configuracaoDocumentoEntrada = repConfiguracaoDocumentoEntrada.BuscarConfiguracaoPadrao();

            if (configuracao.NaoFinalizarDocumentoEntradaOrdemCompraValorDivergente)
            {
                decimal somaDocumentosVinculadosOrdemCompra = repDocumentoEntrada.SomarConsultarDocumentosVinculadosComOrdemCompra(codigoDocumentoEntrada, documentoEntrada.OrdemCompra.Codigo);
                decimal saldoDisponivelOrdemCompra = Math.Round(ordemCompra.ValorTotal, 2) - Math.Round(somaDocumentosVinculadosOrdemCompra, 2);

                if (Math.Round(documentoEntrada.ValorTotal, 2) > Math.Round(saldoDisponivelOrdemCompra, 2))
                {
                    mensagemErro = $"Valor Total do Documento é maior que o saldo disponível da Ordem de Compra. Saldo restante é de R${saldoDisponivelOrdemCompra}.";
                    return false;
                }

                if (Math.Round(documentoEntrada.ValorTotal, 2) > Math.Round(ordemCompra.ValorTotal, 2))
                {
                    mensagemErro = "Valor Total do Documento é maior que o total da Ordem de Compra, não sendo permitido finalizar.";
                    return false;
                }
            }

            if (configuracaoDocumentoEntrada?.BloquearFinalizacaoComFluxoCompraAberto ?? false)
            {
                if (!repFluxoCompra.FluxoCompraDaOrdemFinalizado(ordemCompra.Codigo))
                {
                    mensagemErro = "O fluxo de compra da Ordem de Compra ainda está aberto, não sendo permitido finalizar.";
                    return false;
                }
            }

            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem> itensDocumentoEntrada = repItemDocumentoEntrada.BuscarPorDocumentoEntrada(codigoDocumentoEntrada);
            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem> ordensMercadoria = (from obj in itensDocumentoEntrada
                                                                                                   where obj.OrdemCompraMercadoria != null
                                                                                                        && obj.OrdemCompraMercadoria.OrdemCompra.Situacao != SituacaoOrdemCompra.Finalizada
                                                                                                   select obj).Distinct().ToList();

            if (ordensMercadoria.Count > 0)//Com Mercadoria vinculada aos itens da nota
            {
                foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem item in ordensMercadoria)
                {
                    if (item.Quantidade == item.OrdemCompraMercadoria.Quantidade)
                        item.OrdemCompraMercadoria.QuantidadePendente = 0;
                    else
                        item.OrdemCompraMercadoria.QuantidadePendente = item.OrdemCompraMercadoria.Quantidade - item.Quantidade;

                    ConferirRequisicaoCompra(ordemCompra, documentoEntrada.Destinatario.Codigo, item.Produto.Codigo, item.Quantidade, unidadeTrabalho, tipoServicoMultisoftware, unidadeTrabalho.StringConexao, Auditado);
                    repOrdemCompraMercadoria.Atualizar(item.OrdemCompraMercadoria);
                }

                List<Dominio.Entidades.Embarcador.Compras.OrdemCompraMercadoria> mercadorias = repOrdemCompraMercadoria.BuscarPorOrdem(ordemCompra.Codigo);

                contemQuantidadePendente = mercadorias.Where(o => o.QuantidadePendente != 0).Count() > 0;
                ordemCompra.Situacao = contemQuantidadePendente ? SituacaoOrdemCompra.Incompleta : SituacaoOrdemCompra.Finalizada;
                repOrdemCompra.Atualizar(ordemCompra);
            }
            else
            {
                List<Dominio.Entidades.Embarcador.Compras.OrdemCompraMercadoria> mercadorias = repOrdemCompraMercadoria.BuscarPorOrdem(ordemCompra.Codigo);
                List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem> itensOutrosDocumentoEntrada = repItemDocumentoEntrada.BuscarPorOrdemCompra(ordemCompra.Codigo, codigoDocumentoEntrada);

                IEnumerable<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem> itensTodosDocumentos = itensDocumentoEntrada.ToList();
                itensTodosDocumentos = itensTodosDocumentos.Union(itensOutrosDocumentoEntrada);

                foreach (Dominio.Entidades.Embarcador.Compras.OrdemCompraMercadoria mercadoria in mercadorias)
                {
                    decimal quantidade = itensTodosDocumentos.Where(o => o.Produto.Codigo == mercadoria.Produto.Codigo).Sum(o => o.Quantidade);
                    if (quantidade < mercadoria.Quantidade)
                        mercadoria.QuantidadePendente = mercadoria.Quantidade - quantidade;
                    else
                        mercadoria.QuantidadePendente = 0;

                    repOrdemCompraMercadoria.Atualizar(mercadoria);
                }

                contemQuantidadePendente = mercadorias.Where(o => o.QuantidadePendente != 0).Count() > 0;
                if (!contemQuantidadePendente)
                {
                    ordemCompra.Situacao = SituacaoOrdemCompra.Finalizada;
                    repOrdemCompra.Atualizar(ordemCompra);
                }
            }

            mensagemErro = null;
            return true;
        }
        public static bool FinalizarOrdemCompraPorItem(int codigoDocumentoEntrada,int codigoOrdemCompra, out string mensagemErro, out bool contemQuantidadePendente, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS repDocumentoEntrada = new Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.DocumentoEntradaItem repItemDocumentoEntrada = new Repositorio.Embarcador.Financeiro.DocumentoEntradaItem(unidadeTrabalho);
            Repositorio.Embarcador.Compras.OrdemCompra repOrdemCompra = new Repositorio.Embarcador.Compras.OrdemCompra(unidadeTrabalho);
            Repositorio.Embarcador.Compras.OrdemCompraMercadoria repOrdemCompraMercadoria = new Repositorio.Embarcador.Compras.OrdemCompraMercadoria(unidadeTrabalho);
            Repositorio.Embarcador.Compras.FluxoCompra repFluxoCompra = new Repositorio.Embarcador.Compras.FluxoCompra(unidadeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoEntrada repConfiguracaoDocumentoEntrada = new Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoEntrada(unidadeTrabalho);

            contemQuantidadePendente = false;

            Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada = repDocumentoEntrada.BuscarPorCodigo(codigoDocumentoEntrada);

            if (documentoEntrada.Situacao != SituacaoDocumentoEntrada.Finalizado)
            {
                mensagemErro = null;
                return true;
            }

            Dominio.Entidades.Embarcador.Compras.OrdemCompra ordemCompra = repOrdemCompra.BuscarPorCodigo(codigoOrdemCompra);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentoEntrada configuracaoDocumentoEntrada = repConfiguracaoDocumentoEntrada.BuscarConfiguracaoPadrao();

            if (configuracao.NaoFinalizarDocumentoEntradaOrdemCompraValorDivergente)
            {
                decimal somaDocumentosVinculadosOrdemCompra = repDocumentoEntrada.SomarConsultarDocumentosVinculadosComOrdemCompra(codigoDocumentoEntrada, codigoDocumentoEntrada);
                decimal saldoDisponivelOrdemCompra = Math.Round(ordemCompra.ValorTotal, 2) - Math.Round(somaDocumentosVinculadosOrdemCompra, 2);

                if (Math.Round(documentoEntrada.ValorTotal, 2) > Math.Round(saldoDisponivelOrdemCompra, 2))
                {
                    mensagemErro = $"Valor Total do Documento é maior que o saldo disponível da Ordem de Compra. Saldo restante é de R${saldoDisponivelOrdemCompra}.";
                    return false;
                }

                if (Math.Round(documentoEntrada.ValorTotal, 2) > Math.Round(ordemCompra.ValorTotal, 2))
                {
                    mensagemErro = "Valor Total do Documento é maior que o total da Ordem de Compra, não sendo permitido finalizar.";
                    return false;
                }
            }

            
            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem> itensDocumentoEntrada = repItemDocumentoEntrada.BuscarPorDocumentoEntrada(codigoDocumentoEntrada);
            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem> ordensMercadoria = (from obj in itensDocumentoEntrada
                                                                                                   where obj.OrdemCompraMercadoria != null
                                                                                                        && obj.OrdemCompraMercadoria.OrdemCompra.Situacao != SituacaoOrdemCompra.Finalizada
                                                                                                   select obj).Distinct().ToList();

            if (ordensMercadoria.Count > 0)//Com Mercadoria vinculada aos itens da nota
            {
                foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem item in ordensMercadoria)
                {
                    if (item.Quantidade == item.OrdemCompraMercadoria.Quantidade)
                        item.OrdemCompraMercadoria.QuantidadePendente = 0;
                    else
                        item.OrdemCompraMercadoria.QuantidadePendente = item.OrdemCompraMercadoria.Quantidade - item.Quantidade;

                    ConferirRequisicaoCompra(ordemCompra, documentoEntrada.Destinatario.Codigo, item.Produto.Codigo, item.Quantidade, unidadeTrabalho, tipoServicoMultisoftware, unidadeTrabalho.StringConexao, Auditado);
                    repOrdemCompraMercadoria.Atualizar(item.OrdemCompraMercadoria);
                }

                List<Dominio.Entidades.Embarcador.Compras.OrdemCompraMercadoria> mercadorias = repOrdemCompraMercadoria.BuscarPorOrdem(ordemCompra.Codigo);

                contemQuantidadePendente = mercadorias.Where(o => o.QuantidadePendente != 0).Count() > 0;
                ordemCompra.Situacao = SituacaoOrdemCompra.Finalizada;
                repOrdemCompra.Atualizar(ordemCompra);
            }
            else
            {
                List<Dominio.Entidades.Embarcador.Compras.OrdemCompraMercadoria> mercadorias = repOrdemCompraMercadoria.BuscarPorOrdem(ordemCompra.Codigo);
                List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem> itensOutrosDocumentoEntrada = repItemDocumentoEntrada.BuscarPorOrdemCompra(ordemCompra.Codigo, codigoDocumentoEntrada);

                IEnumerable<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem> itensTodosDocumentos = itensDocumentoEntrada.ToList();
                itensTodosDocumentos = itensTodosDocumentos.Union(itensOutrosDocumentoEntrada);

                foreach (Dominio.Entidades.Embarcador.Compras.OrdemCompraMercadoria mercadoria in mercadorias)
                {
                    decimal quantidade = itensTodosDocumentos.Where(o => o.Produto.Codigo == mercadoria.Produto.Codigo).Sum(o => o.Quantidade);
                    if (quantidade < mercadoria.Quantidade)
                        mercadoria.QuantidadePendente = mercadoria.Quantidade - quantidade;
                    else
                        mercadoria.QuantidadePendente = 0;

                    repOrdemCompraMercadoria.Atualizar(mercadoria);
                }

                contemQuantidadePendente = mercadorias.Where(o => o.QuantidadePendente != 0).Count() > 0;
                if (!contemQuantidadePendente)
                {
                    ordemCompra.Situacao = SituacaoOrdemCompra.Finalizada;
                    repOrdemCompra.Atualizar(ordemCompra);
                }
            }

            mensagemErro = null;
            return true;
        }
        public static void CriarQualificacaoFornecedor(Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Compras.QualificaoFornecedor repQualificaoFornecedor = new Repositorio.Embarcador.Compras.QualificaoFornecedor(unitOfWork);

            if (documentoEntrada == null || documentoEntrada.OrdemCompra == null)
                return;

            if (repQualificaoFornecedor.BuscarPorOrdemCompra(documentoEntrada.OrdemCompra.Codigo) != null)
                return;

            Dominio.Entidades.Embarcador.Compras.QualificaoFornecedor qualificao = new Dominio.Entidades.Embarcador.Compras.QualificaoFornecedor()
            {
                Fornecedor = documentoEntrada.OrdemCompra.Fornecedor,
                DocumentoEntrada = documentoEntrada,
                OrdemCompra = documentoEntrada.OrdemCompra,
            };

            repQualificaoFornecedor.Inserir(qualificao);
        }

        public static bool EstornarOrdemCompra(int codigoDocumentoEntrada, out string mensagemErro, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado)
        {
            Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS repDocumentoEntrada = new Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.DocumentoEntradaItem repItemDocumentoEntrada = new Repositorio.Embarcador.Financeiro.DocumentoEntradaItem(unidadeTrabalho);
            Repositorio.Embarcador.Compras.OrdemCompra repOrdemCompra = new Repositorio.Embarcador.Compras.OrdemCompra(unidadeTrabalho);
            Repositorio.Embarcador.Compras.OrdemCompraMercadoria repOrdemCompraMercadoria = new Repositorio.Embarcador.Compras.OrdemCompraMercadoria(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada = repDocumentoEntrada.BuscarPorCodigo(codigoDocumentoEntrada);
            if (documentoEntrada.OrdemCompra == null)
            {
                mensagemErro = null;
                return true;
            }

            Dominio.Entidades.Embarcador.Compras.OrdemCompra ordemCompra = repOrdemCompra.BuscarPorCodigo(documentoEntrada.OrdemCompra.Codigo);
            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem> itensDocumentoEntrada = repItemDocumentoEntrada.BuscarPorDocumentoEntrada(codigoDocumentoEntrada);
            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem> ordensMercadoria = (from obj in itensDocumentoEntrada
                                                                                                   where obj.OrdemCompraMercadoria != null &&
                                                                                                        (obj.OrdemCompraMercadoria.OrdemCompra.Situacao == SituacaoOrdemCompra.Finalizada ||
                                                                                                         obj.OrdemCompraMercadoria.OrdemCompra.Situacao == SituacaoOrdemCompra.Incompleta)
                                                                                                   select obj).Distinct().ToList();

            if (ordensMercadoria.Count > 0)//Com Mercadoria vinculada aos itens da nota
            {
                foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem item in ordensMercadoria)
                {
                    item.OrdemCompraMercadoria.QuantidadePendente = item.OrdemCompraMercadoria.QuantidadePendente - (item.OrdemCompraMercadoria.Quantidade - item.Quantidade);

                    repOrdemCompraMercadoria.Atualizar(item.OrdemCompraMercadoria);
                }

                List<Dominio.Entidades.Embarcador.Compras.OrdemCompraMercadoria> mercadorias = repOrdemCompraMercadoria.BuscarPorOrdem(ordemCompra.Codigo);
                bool todosProdutosZerados = mercadorias.Where(o => o.QuantidadePendente == 0).Count() == mercadorias.Count();
                ordemCompra.Situacao = todosProdutosZerados ? SituacaoOrdemCompra.Aprovada : SituacaoOrdemCompra.Incompleta;
                repOrdemCompra.Atualizar(ordemCompra);
            }
            else
            {
                List<Dominio.Entidades.Embarcador.Compras.OrdemCompraMercadoria> mercadorias = repOrdemCompraMercadoria.BuscarPorOrdem(ordemCompra.Codigo);

                foreach (Dominio.Entidades.Embarcador.Compras.OrdemCompraMercadoria mercadoria in mercadorias)
                {
                    decimal quantidade = itensDocumentoEntrada.Where(o => o.Produto.Codigo == mercadoria.Produto.Codigo).Sum(o => o.Quantidade);
                    if (quantidade > 0)
                    {
                        mercadoria.QuantidadePendente += quantidade > mercadoria.Quantidade ? mercadoria.Quantidade : quantidade;
                        repOrdemCompraMercadoria.Atualizar(mercadoria);
                    }
                }

                if (ordemCompra.Situacao == SituacaoOrdemCompra.Finalizada)
                {
                    ordemCompra.Situacao = SituacaoOrdemCompra.Aprovada;
                    repOrdemCompra.Atualizar(ordemCompra);
                }
            }

            mensagemErro = null;
            return true;
        }

        public static void EtapaAprovacao(ref Dominio.Entidades.Embarcador.Compras.OrdemCompra ordem, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, int codigoEmpresa)
        {
            List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.RegrasOrdemCompra> regras = VerificarRegrasAutorizacao(ordem, unitOfWork, codigoEmpresa);

            bool possuiRegra = regras.Count() > 0;
            bool agAprovacao = true;

            if (possuiRegra)
            {
                ordem.Situacao = SituacaoOrdemCompra.AgAprovacao;

                agAprovacao = CriarRegrasAutorizacao(regras, ordem, ordem.Usuario, tipoServicoMultisoftware, stringConexao, unitOfWork);

                if (!agAprovacao)
                    ordem.Situacao = SituacaoOrdemCompra.Aprovada;
            }
            else
                ordem.Situacao = SituacaoOrdemCompra.SemRegra;

            OrdemCompraAprovada(ordem, unitOfWork, Auditado, tipoServicoMultisoftware);
        }

        #endregion

        #region Métodos Privados

        private static bool CriarRegrasAutorizacao(List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.RegrasOrdemCompra> listaFiltrada, Dominio.Entidades.Embarcador.Compras.OrdemCompra ordemCompra, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao, Repositorio.UnitOfWork unitOfWork)
        {
            bool possuiRegraPendente = false;

            Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(stringConexao, null, tipoServicoMultisoftware, string.Empty);
            Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra repAprovacaoAlcadaOrdemCompra = new Repositorio.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra(unitOfWork);

            if (listaFiltrada == null || listaFiltrada.Count() == 0)
                throw new ArgumentException("Lista de Regras deve ser maior que 0");

            foreach (Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.RegrasOrdemCompra regra in listaFiltrada)
            {
                if (regra.NumeroAprovadores > 0)
                {
                    possuiRegraPendente = true;
                    foreach (Dominio.Entidades.Usuario aprovador in regra.Aprovadores)
                    {
                        Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra autorizacao = new Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra
                        {
                            OrdemCompra = ordemCompra,
                            Usuario = aprovador,
                            RegraOrdemCompra = regra,
                            DataCriacao = ordemCompra.Data,
                        };
                        repAprovacaoAlcadaOrdemCompra.Inserir(autorizacao);

                        string nota = string.Format(Localization.Resources.Compras.OrdemCompra.UsuarioCriouOrdem,(usuario?.Nome ?? ""), ordemCompra.Numero);
                        if (usuario != null)
                            serNotificacao.GerarNotificacaoEmail(aprovador, usuario, ordemCompra.Codigo, "Compras/AutorizacaoOrdemCompra", Localization.Resources.Compras.OrdemCompra.OdemDeCompra, nota, IconesNotificacao.cifra, TipoNotificacao.credito, tipoServicoMultisoftware, unitOfWork);
                    }
                }
                else
                {
                    Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra autorizacao = new Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra
                    {
                        OrdemCompra = ordemCompra,
                        Usuario = null,
                        RegraOrdemCompra = regra,
                        Situacao = SituacaoAlcadaRegra.Aprovada,
                        Data = DateTime.Now,
                        Motivo = "Alçada aprovada pela Regra " + regra.Descricao,
                        DataCriacao = ordemCompra.Data,
                    };
                    repAprovacaoAlcadaOrdemCompra.Inserir(autorizacao);
                }
            }

            return possuiRegraPendente;
        }

        private static void ConferirRequisicaoCompra(Dominio.Entidades.Embarcador.Compras.OrdemCompra ordem, int codigoEmpresa, int produto, decimal quantidade, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado)
        {
            Repositorio.Embarcador.Compras.Mercadoria repMercadoria = new Repositorio.Embarcador.Compras.Mercadoria(unidadeTrabalho);
            Repositorio.Embarcador.Compras.RequisicaoMercadoria repRequisicaoMercadoria = new Repositorio.Embarcador.Compras.RequisicaoMercadoria(unidadeTrabalho);
            List<Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria> requisicoes = null;

            if (ordem.Requisicoes != null && ordem.Requisicoes.Count > 0)
                requisicoes = ordem.Requisicoes.Where(o => o.Requisicao.Mercadorias.Any(obj => obj.ProdutoEstoque.Produto.Codigo == produto)).Select(o => o.Requisicao).ToList();
            if (requisicoes == null || requisicoes.Count == 0)
            {
                if (ordem.CotacaoCompra != null && ordem.CotacaoCompra.Requisicoes != null && ordem.CotacaoCompra.Requisicoes.Count > 0)
                {
                    requisicoes = ordem.CotacaoCompra.Requisicoes.Where(o => o.RequisicaoMercadoria.Mercadorias.Any(obj => obj.ProdutoEstoque.Produto.Codigo == produto)).Select(o => o.RequisicaoMercadoria).ToList();
                }
            }
            if (requisicoes != null && requisicoes.Count > 0)
            {
                foreach (var req in requisicoes)
                {
                    Dominio.Entidades.Embarcador.Compras.Mercadoria mercadoria = repMercadoria.BuscarPorModoCompra(req.Codigo, produto, codigoEmpresa, quantidade);
                    if (mercadoria != null)
                    {
                        mercadoria.Modo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModoRequisicaoMercadoria.Requisicao;
                        repMercadoria.Atualizar(mercadoria);

                        Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria requisicao = repRequisicaoMercadoria.BuscarPorCodigo(req.Codigo, true);
                        requisicao.Modo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModoRequisicaoMercadoria.Requisicao;
                        requisicao.DataAlteracao = DateTime.Now;
                        Servicos.Embarcador.Compras.RequisicaoMercadoria.EtapaAprovacao(ref requisicao, unidadeTrabalho, tipoServicoMultisoftware, stringConexao, Auditado);
                        repRequisicaoMercadoria.Atualizar(requisicao);
                    }
                }
            }
        }

        private static List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.RegrasOrdemCompra> VerificarRegrasAutorizacao(Dominio.Entidades.Embarcador.Compras.OrdemCompra ordemCompra, Repositorio.UnitOfWork unitOfWork, int codigoEmpresa)
        {
            Repositorio.Embarcador.Compras.OrdemCompraMercadoria repOrdemCompraMercadoria = new Repositorio.Embarcador.Compras.OrdemCompraMercadoria(unitOfWork);
            Repositorio.Embarcador.Compras.AlcadasOrdemCompra.RegrasOrdemCompra repRegrasOrdemCompra = new Repositorio.Embarcador.Compras.AlcadasOrdemCompra.RegrasOrdemCompra(unitOfWork);

            List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.RegrasOrdemCompra> listaRegras = new List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.RegrasOrdemCompra>();
            List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.RegrasOrdemCompra> listaFiltrada = new List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.RegrasOrdemCompra>();
            List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.RegrasOrdemCompra> alcadasCompativeis;

            alcadasCompativeis = repRegrasOrdemCompra.AlcadasPorFornecedor(ordemCompra.Fornecedor.Codigo, DateTime.Today, codigoEmpresa);
            listaRegras.AddRange(alcadasCompativeis);

            alcadasCompativeis = repRegrasOrdemCompra.AlcadasPorOperador(ordemCompra.Usuario?.Codigo ?? 0, DateTime.Today, codigoEmpresa);
            listaRegras.AddRange(alcadasCompativeis);

            if (ordemCompra.Usuario?.Setor != null)
            {
                alcadasCompativeis = repRegrasOrdemCompra.AlcadasPorSetorOperador(ordemCompra.Usuario?.Setor?.Codigo ?? 0, DateTime.Today, codigoEmpresa);
                listaRegras.AddRange(alcadasCompativeis);
            }

            List<Dominio.Entidades.Embarcador.Compras.OrdemCompraMercadoria> mercadorias = repOrdemCompraMercadoria.BuscarPorOrdem(ordemCompra.Codigo);
            foreach (Dominio.Entidades.Embarcador.Compras.OrdemCompraMercadoria mercadoria in mercadorias)
            {
                Dominio.Entidades.Produto produto = mercadoria.Produto;

                alcadasCompativeis = repRegrasOrdemCompra.AlcadasPorProduto(produto.Codigo, DateTime.Today, codigoEmpresa);
                listaRegras.AddRange(alcadasCompativeis);

                if (produto.UltimoCusto > 0)
                {
                    decimal percentual = (mercadoria.ValorUnitario / produto.UltimoCusto) * 100;
                    alcadasCompativeis = repRegrasOrdemCompra.AlcadasPorPercentualDiferencaValorCustoProduto(percentual, DateTime.Today, codigoEmpresa);
                    listaRegras.AddRange(alcadasCompativeis);
                }
            }

            List<int> codigosGrupoProduto = mercadorias.Where(o => o.Produto.GrupoProdutoTMS != null).Select(o => o.Produto.GrupoProdutoTMS.Codigo).Distinct().ToList();
            foreach (int codigoGrupoProduto in codigosGrupoProduto)
            {
                alcadasCompativeis = repRegrasOrdemCompra.AlcadasPorGrupoProduto(codigoGrupoProduto, DateTime.Today, codigoEmpresa);
                listaRegras.AddRange(alcadasCompativeis);
            }

            decimal valorTotalOrdem = mercadorias.Select(o => o.ValorTotal).Sum();
            alcadasCompativeis = repRegrasOrdemCompra.AlcadasPorValor(valorTotalOrdem, DateTime.Today, codigoEmpresa);
            listaRegras.AddRange(alcadasCompativeis);

            listaRegras = listaRegras.Distinct().ToList();
            if (listaRegras.Count() > 0)
            {
                listaFiltrada.AddRange(listaRegras);
                foreach (Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.RegrasOrdemCompra regra in listaRegras)
                {
                    if (regra.RegraPorFornecedor)
                    {
                        bool valido = false;
                        if (regra.AlcadasFornecedor.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && o.Fornecedor.Codigo == ordemCompra.Fornecedor.Codigo))
                            valido = true;
                        else if (regra.AlcadasFornecedor.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && o.Fornecedor.Codigo == ordemCompra.Fornecedor.Codigo))
                            valido = true;
                        else if (regra.AlcadasFornecedor.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && o.Fornecedor.Codigo != ordemCompra.Fornecedor.Codigo))
                            valido = true;
                        else if (regra.AlcadasFornecedor.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && o.Fornecedor.Codigo != ordemCompra.Fornecedor.Codigo))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }

                    if (regra.RegraPorOperador)
                    {
                        bool valido = false;
                        if (regra.AlcadasOperador.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && o.Operador.Codigo == (ordemCompra.Usuario?.Codigo ?? 0)))
                            valido = true;
                        else if (regra.AlcadasOperador.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && o.Operador.Codigo == (ordemCompra.Usuario?.Codigo ?? 0)))
                            valido = true;
                        else if (regra.AlcadasOperador.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && o.Operador.Codigo != (ordemCompra.Usuario?.Codigo ?? 0)))
                            valido = true;
                        else if (regra.AlcadasOperador.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && o.Operador.Codigo != (ordemCompra.Usuario?.Codigo ?? 0)))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }

                    if (regra.RegraPorSetorOperador)
                    {
                        bool valido = false;
                        if (regra.AlcadasSetorOperador.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && o.Setor.Codigo == (ordemCompra.Usuario.Setor?.Codigo ?? 0)))
                            valido = true;
                        else if (regra.AlcadasSetorOperador.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && o.Setor.Codigo == (ordemCompra.Usuario.Setor?.Codigo ?? 0)))
                            valido = true;
                        else if (regra.AlcadasSetorOperador.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && o.Setor.Codigo != (ordemCompra.Usuario.Setor?.Codigo ?? 0)))
                            valido = true;
                        else if (regra.AlcadasSetorOperador.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && o.Setor.Codigo != (ordemCompra.Usuario.Setor?.Codigo ?? 0)))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }

                    if (regra.RegraPorProduto)
                    {
                        bool valido = false;
                        if (regra.AlcadasProduto.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && mercadorias.Any(f => f.Produto.Codigo == o.Produto.Codigo)))
                            valido = true;
                        else if (regra.AlcadasProduto.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && mercadorias.Any(f => f.Produto.Codigo == o.Produto.Codigo)))
                            valido = true;
                        else if (regra.AlcadasProduto.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && mercadorias.Any(f => f.Produto.Codigo != o.Produto.Codigo)))
                            valido = true;
                        else if (regra.AlcadasProduto.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && mercadorias.Any(f => f.Produto.Codigo != o.Produto.Codigo)))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }

                    if (regra.RegraPorValor)
                    {
                        bool valido = false;
                        if (regra.AlcadasValor.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && o.Valor == valorTotalOrdem))
                            valido = true;
                        else if (regra.AlcadasValor.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && o.Valor == valorTotalOrdem))
                            valido = true;
                        else if (regra.AlcadasValor.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && o.Valor != valorTotalOrdem))
                            valido = true;
                        else if (regra.AlcadasValor.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && o.Valor != valorTotalOrdem))
                            valido = true;

                        if (regra.AlcadasValor.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MaiorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && valorTotalOrdem >= o.Valor))
                            valido = true;
                        else if (regra.AlcadasValor.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MaiorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && valorTotalOrdem >= o.Valor))
                            valido = true;

                        if (regra.AlcadasValor.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MenorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && valorTotalOrdem <= o.Valor))
                            valido = true;
                        else if (regra.AlcadasValor.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MenorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && valorTotalOrdem <= o.Valor))
                            valido = true;

                        if (regra.AlcadasValor.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MaiorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && valorTotalOrdem > o.Valor))
                            valido = true;
                        else if (regra.AlcadasValor.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MaiorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && valorTotalOrdem > o.Valor))
                            valido = true;

                        if (regra.AlcadasValor.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MenorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && valorTotalOrdem < o.Valor))
                            valido = true;
                        else if (regra.AlcadasValor.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MenorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && valorTotalOrdem < o.Valor))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }

                    if (regra.RegraPorGrupoProduto)
                    {
                        bool valido = false;
                        if (regra.AlcadasGrupoProduto.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && mercadorias.Any(f => f.Produto.GrupoProdutoTMS != null && f.Produto.GrupoProdutoTMS.Codigo == o.GrupoProdutoTMS.Codigo)))
                            valido = true;
                        else if (regra.AlcadasGrupoProduto.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && mercadorias.Any(f => f.Produto.GrupoProdutoTMS != null && f.Produto.GrupoProdutoTMS.Codigo == o.GrupoProdutoTMS.Codigo)))
                            valido = true;
                        else if (regra.AlcadasGrupoProduto.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && mercadorias.Any(f => f.Produto.GrupoProdutoTMS != null && f.Produto.GrupoProdutoTMS.Codigo != o.GrupoProdutoTMS.Codigo)))
                            valido = true;
                        else if (regra.AlcadasGrupoProduto.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && mercadorias.Any(f => f.Produto.GrupoProdutoTMS != null && f.Produto.GrupoProdutoTMS.Codigo != o.GrupoProdutoTMS.Codigo)))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }

                    if (regra.RegraPorPercentualDiferencaValorCustoProduto)
                    {
                        bool valido = false;
                        if (regra.AlcadasPercentualDiferencaValorCustoProduto.All(o => o.Condicao == CondicaoAutorizao.IgualA && o.Juncao == JuncaoAutorizao.E && mercadorias.Any(m => m.Produto.UltimoCusto > 0 && ((m.ValorUnitario / m.Produto.UltimoCusto) * 100) == o.PercentualDiferencaValorCustoProduto)))
                            valido = true;
                        else if (regra.AlcadasPercentualDiferencaValorCustoProduto.Any(o => o.Condicao == CondicaoAutorizao.IgualA && o.Juncao == JuncaoAutorizao.Ou && mercadorias.Any(m => m.Produto.UltimoCusto > 0 && ((m.ValorUnitario / m.Produto.UltimoCusto) * 100) == o.PercentualDiferencaValorCustoProduto)))
                            valido = true;
                        else if (regra.AlcadasPercentualDiferencaValorCustoProduto.Any(o => o.Condicao == CondicaoAutorizao.DiferenteDe && o.Juncao == JuncaoAutorizao.Ou && mercadorias.Any(m => m.Produto.UltimoCusto > 0 && ((m.ValorUnitario / m.Produto.UltimoCusto) * 100) != o.PercentualDiferencaValorCustoProduto)))
                            valido = true;
                        else if (regra.AlcadasPercentualDiferencaValorCustoProduto.All(o => o.Condicao == CondicaoAutorizao.DiferenteDe && o.Juncao == JuncaoAutorizao.E && mercadorias.Any(m => m.Produto.UltimoCusto > 0 && ((m.ValorUnitario / m.Produto.UltimoCusto) * 100) != o.PercentualDiferencaValorCustoProduto)))
                            valido = true;

                        if (regra.AlcadasPercentualDiferencaValorCustoProduto.All(o => o.Condicao == CondicaoAutorizao.MaiorIgualQue && o.Juncao == JuncaoAutorizao.E && mercadorias.Any(m => m.Produto.UltimoCusto > 0 && ((m.ValorUnitario / m.Produto.UltimoCusto) * 100) >= o.PercentualDiferencaValorCustoProduto)))
                            valido = true;
                        else if (regra.AlcadasPercentualDiferencaValorCustoProduto.Any(o => o.Condicao == CondicaoAutorizao.MaiorIgualQue && o.Juncao == JuncaoAutorizao.Ou && mercadorias.Any(m => m.Produto.UltimoCusto > 0 && ((m.ValorUnitario / m.Produto.UltimoCusto) * 100) >= o.PercentualDiferencaValorCustoProduto)))
                            valido = true;

                        if (regra.AlcadasPercentualDiferencaValorCustoProduto.All(o => o.Condicao == CondicaoAutorizao.MenorIgualQue && o.Juncao == JuncaoAutorizao.E && mercadorias.Any(m => m.Produto.UltimoCusto > 0 && ((m.ValorUnitario / m.Produto.UltimoCusto) * 100) <= o.PercentualDiferencaValorCustoProduto)))
                            valido = true;
                        else if (regra.AlcadasPercentualDiferencaValorCustoProduto.Any(o => o.Condicao == CondicaoAutorizao.MenorIgualQue && o.Juncao == JuncaoAutorizao.Ou && mercadorias.Any(m => m.Produto.UltimoCusto > 0 && ((m.ValorUnitario / m.Produto.UltimoCusto) * 100) <= o.PercentualDiferencaValorCustoProduto)))
                            valido = true;

                        if (regra.AlcadasPercentualDiferencaValorCustoProduto.All(o => o.Condicao == CondicaoAutorizao.MaiorQue && o.Juncao == JuncaoAutorizao.E && mercadorias.Any(m => m.Produto.UltimoCusto > 0 && ((m.ValorUnitario / m.Produto.UltimoCusto) * 100) > o.PercentualDiferencaValorCustoProduto)))
                            valido = true;
                        else if (regra.AlcadasPercentualDiferencaValorCustoProduto.Any(o => o.Condicao == CondicaoAutorizao.MaiorQue && o.Juncao == JuncaoAutorizao.Ou && mercadorias.Any(m => m.Produto.UltimoCusto > 0 && ((m.ValorUnitario / m.Produto.UltimoCusto) * 100) > o.PercentualDiferencaValorCustoProduto)))
                            valido = true;

                        if (regra.AlcadasPercentualDiferencaValorCustoProduto.All(o => o.Condicao == CondicaoAutorizao.MenorQue && o.Juncao == JuncaoAutorizao.E && mercadorias.Any(m => m.Produto.UltimoCusto > 0 && ((m.ValorUnitario / m.Produto.UltimoCusto) * 100) < o.PercentualDiferencaValorCustoProduto)))
                            valido = true;
                        else if (regra.AlcadasPercentualDiferencaValorCustoProduto.Any(o => o.Condicao == CondicaoAutorizao.MenorQue && o.Juncao == JuncaoAutorizao.Ou && mercadorias.Any(m => m.Produto.UltimoCusto > 0 && ((m.ValorUnitario / m.Produto.UltimoCusto) * 100) < o.PercentualDiferencaValorCustoProduto)))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }
                }
            }

            return listaFiltrada;
        }

        #endregion
    }
}
