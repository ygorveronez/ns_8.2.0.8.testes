using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Relatorios;
using Repositorio;
using Servicos.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Canhotos
{
    public class Canhoto : ServicoBase
    {        
        public Canhoto(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }

        #region Métodos Públicos

        public static void CanhotoLiberado(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, bool confimarControleEntrega = true)
        {
            new Canhoto(unitOfWork).CanhotoLiberadoAsync(canhoto, configuracao, tipoServicoMultisoftware, clienteMultisoftware, confimarControleEntrega).GetAwaiter().GetResult();
        }

        public async Task CanhotoLiberadoAsync(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, bool confimarControleEntrega = true)
        {
            await LiberarPagamentoPorCanhotoAsync(canhoto, configuracao);

            if (configuracao.ConfirmarEntregaDigitilizacaoCanhoto && confimarControleEntrega)
                await ConfirmarEntregaPorCanhotoAsync(canhoto, configuracao, tipoServicoMultisoftware, clienteMultisoftware);

            await TodosCanhotosPorSituacaoAsync(canhoto, new List<SituacaoDigitalizacaoCanhoto>() { SituacaoDigitalizacaoCanhoto.Digitalizado }, TipoRegistroIntegracaoCTeCanhoto.Confirmacao);
        }

        public static void CanhotoAgAprovacao(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            new Canhoto(unitOfWork).CanhotoAgAprovacaoAsync(canhoto, configuracao).GetAwaiter().GetResult();
        }

        public async Task CanhotoAgAprovacaoAsync(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto repositorioConfiguracaoCanhoto = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto(_unitOfWork);
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repositorioDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto = await repositorioConfiguracaoCanhoto.BuscarConfiguracaoPadraoAsync();

            if (!(configuracaoCanhoto?.LiberarParaPagamentoAposDigitalizacaCanhoto ?? false))
                return;

            //buscar o CTE atravez da nota fiscal do canhoho.
            //verificar se todas as notas do CTE tem canhoto com status agAprovacao e/ou digitalizados.
            List<SituacaoDigitalizacaoCanhoto> situacoes = new List<SituacaoDigitalizacaoCanhoto>() { SituacaoDigitalizacaoCanhoto.AgAprovocao, SituacaoDigitalizacaoCanhoto.Digitalizado };
            if (!await TodosCanhotosPorSituacaoAsync(canhoto, situacoes, TipoRegistroIntegracaoCTeCanhoto.Imagem))
                return;

            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> documentosFaturamento = await repositorioDocumentoFaturamento.BuscarPorDocumentosNotaFiscalAsync(canhoto.XMLNotaFiscal?.Codigo ?? 0, canhoto.Empresa?.Codigo ?? 0);
            foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento in documentosFaturamento)
            {
                if (documentoFaturamento.CTe != null)
                {
                    documentoFaturamento.CanhotosDigitalizados = true;
                    await repositorioDocumentoFaturamento.AtualizarAsync(documentoFaturamento);
                }
            }

        }

        private async Task LiberarPagamentoPorCanhotoAsync(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            if (configuracao.GerarPagamentoBloqueado &&
                (canhoto.Emitente?.GrupoPessoas == null || (!canhoto.Emitente.GrupoPessoas.ExigeCanhotoFisico.HasValue || !canhoto.Emitente.GrupoPessoas.ExigeCanhotoFisico.Value)
                || (canhoto.SituacaoCanhoto == SituacaoCanhoto.Justificado || canhoto.SituacaoCanhoto == SituacaoCanhoto.RecebidoFisicamente)
                ))
            {
                Repositorio.Embarcador.Financeiro.DocumentoFaturamento repositorioDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(_unitOfWork, _cancellationToken);
                Repositorio.Embarcador.Canhotos.Canhoto repositorioCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(_unitOfWork, _cancellationToken);
                Repositorio.Embarcador.Financeiro.Titulo repositorioTitulo = new Repositorio.Embarcador.Financeiro.Titulo(_unitOfWork, _cancellationToken);

                if (canhoto.TipoCanhoto == TipoCanhoto.NFe)
                {
                    //se a mesma transportador faz o primeiro e o segundo trecho de uma nota por exemplo, ao digitalizar o canhoto da NF-e liberar o pagamento dos ctes dos dois trechos.
                    List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> documentosFaturamento = await repositorioDocumentoFaturamento.BuscarDocumentoFaturamentoPorNotaFiscalAsync(canhoto.XMLNotaFiscal?.Codigo ?? 0, canhoto.Empresa?.Codigo ?? 0);

                    if (documentosFaturamento.Count == 0)
                        return;

                    List<int> codigosCTes = documentosFaturamento.Select(obj => obj.CTe.Codigo).ToList();

                    List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotosCTes = await repositorioCanhoto.BuscarPorNotasDosCTesECanhotoAsync(codigosCTes, canhoto.Codigo);

                    foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento in documentosFaturamento)
                    {
                        bool canhotos = canhotosCTes.Any(obj => obj.XMLNotaFiscal.CTEs.Any(o => o.Codigo == documentoFaturamento.CTe.Codigo));

                        if (canhotos)
                            continue;

                        documentoFaturamento.PagamentoDocumentoBloqueado = false;
                        documentoFaturamento.DataLiberacaoPagamento = DateTime.Now;

                        await repositorioDocumentoFaturamento.AtualizarAsync(documentoFaturamento);

                        if (documentoFaturamento.CTe.Titulo != null && documentoFaturamento.CTe.Titulo.StatusTitulo != StatusTitulo.Bloqueado)
                        {
                            documentoFaturamento.CTe.Titulo.StatusTitulo = StatusTitulo.EmAberto;
                            await repositorioTitulo.AtualizarAsync(documentoFaturamento.CTe.Titulo);
                        }

                    }
                }
                else if (canhoto.TipoCanhoto == TipoCanhoto.Avulso)
                {
                    List<int> codigosXMLNotaFiscais = canhoto.CanhotoAvulso.PedidosXMLNotasFiscais.Select(x => x.XMLNotaFiscal.Codigo).ToList();

                    List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> documentosFaturamento = await repositorioDocumentoFaturamento.BuscarDocumentoFaturamentoPorNotasFiscaisAsync(codigosXMLNotaFiscais, canhoto.Empresa.Codigo);

                    foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento in documentosFaturamento)
                    {
                        Servicos.Log.TratarErro($"Nota liberada. Codigo: {documentoFaturamento.Codigo} - Numero: {documentoFaturamento.Numero} - Canhoto Número: {canhoto.Numero}", "CanhotoLiberacaoPagamento");

                        documentoFaturamento.PagamentoDocumentoBloqueado = false;
                        documentoFaturamento.DataLiberacaoPagamento = DateTime.Now;

                        await repositorioDocumentoFaturamento.AtualizarAsync(documentoFaturamento);

                        if (documentoFaturamento.CTe.Titulo != null && documentoFaturamento.CTe.Titulo.StatusTitulo != StatusTitulo.Bloqueado)
                        {
                            documentoFaturamento.CTe.Titulo.StatusTitulo = StatusTitulo.EmAberto;
                            await repositorioTitulo.AtualizarAsync(documentoFaturamento.CTe.Titulo);
                        }
                    }
                }
            }
        }

        public async Task ConfirmarEntregaPorCanhotoAsync(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            if (canhoto.SituacaoCanhoto == SituacaoCanhoto.Justificado || canhoto.SituacaoDigitalizacaoCanhoto == SituacaoDigitalizacaoCanhoto.Digitalizado)
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork, _cancellationToken);

                if (canhoto.TipoCanhoto == TipoCanhoto.NFe && canhoto.Carga != null && canhoto.XMLNotaFiscal != null)
                {
                    Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = await repositorioCargaEntrega.BuscarPorCargaENotaFiscalAsync(canhoto.Carga.Codigo, canhoto.XMLNotaFiscal.Codigo);

                    if (cargaEntrega != null)
                    {
                        Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repositorioCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(_unitOfWork, _cancellationToken);

                        bool todosCanhotosJustificadosOuDigitalizados = await repositorioCargaEntregaNotaFiscal.BuscarPorCargaLiberacaoEntregaAsync(canhoto.Carga.Codigo, cargaEntrega.Codigo);

                        if (todosCanhotosJustificadosOuDigitalizados)
                            ConfirmarEntrega(cargaEntrega, canhoto, configuracao, _unitOfWork, tipoServicoMultisoftware, clienteMultisoftware);
                    }
                }
                else if (canhoto.TipoCanhoto == TipoCanhoto.Avulso)
                {
                    Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = await repositorioCargaEntrega.BuscarPorCargaPedidoAsync(canhoto.CanhotoAvulso.PedidosXMLNotasFiscais.FirstOrDefault().CargaPedido.Codigo);

                    if (cargaEntrega != null)
                        ConfirmarEntrega(cargaEntrega, canhoto, configuracao, _unitOfWork, tipoServicoMultisoftware, clienteMultisoftware);
                }
            }
        }

        private static void ConfirmarEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            Servicos.Embarcador.Carga.Carga serCarga = new Carga.Carga(unitOfWork);
            if (serCarga.VerificarSeCargaEstaNaLogistica(cargaEntrega.Carga, tipoServicoMultisoftware) || cargaEntrega.Carga.SituacaoCarga == SituacaoCarga.PendeciaDocumentos)
                return;

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
            auditado.Texto = "Finalizado por liberação dos canhotos da entrega";
            auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema;
            Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPoint = null;

            if (!string.IsNullOrWhiteSpace(canhoto.Latitude))
                wayPoint = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint(canhoto.Latitude, canhoto.Longitude);

            DateTime dataEntrega = canhoto.DataEntregaNotaCliente ?? (canhoto.DataDigitalizacao ?? canhoto.DataEnvioCanhoto);
            if (!canhoto.DataEntregaNotaCliente.HasValue && cargaEntrega.DataPrevista.HasValue) // regra fixa danone talvez tenha que criar alguma configuração para tal.
                dataEntrega = cargaEntrega.DataPrevista.Value;

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork).BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacaoParametro = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork).BuscarPorCodigoFetch(cargaEntrega.Carga.TipoOperacao?.Codigo ?? 0);
            Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.FinalizarEntrega(cargaEntrega, dataEntrega, wayPoint, null, 0, "", configuracao, tipoServicoMultisoftware, auditado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemSituacaoEntrega.LiberacaoCanhoto, clienteMultisoftware, unitOfWork, false, configuracaoControleEntrega, tipoOperacaoParametro);
        }

        public static byte[] GerarCanhotoAvulso(int codigoCanhoto, Repositorio.UnitOfWork unidadeTrabalho, out string mensagemErro)
        {
            ReportResult result = ReportRequest.WithType(ReportType.CanhotoAvulso)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("CodigoCanhoto", codigoCanhoto.ToString())
                .CallReport();

            mensagemErro = result.ErrorMessage;

            return result.GetContentFile();
        }

        public bool CanhotoPossuiCTeNaoAutorizado(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto)
        {
            if (canhoto.Carga.CargaCTes == null ||
                !canhoto.Carga.CargaCTes.Any(obj => obj.CTe != null && obj.CTe.XMLNotaFiscais.Contains(canhoto.XMLNotaFiscal)) ||
                canhoto.Carga.CargaCTes.Any(obj => obj.CTe != null && obj.CTe.XMLNotaFiscais.Contains(canhoto.XMLNotaFiscal) && obj.CTe.Status != "A"))
            {
                return true;
            }

            return false;
        }

        public static void NotificarTransportadoresDeCanhotosPendentes(Repositorio.UnitOfWork unitOfWork, bool buscarPorTipoOperacao, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, string razaoSocial)
        {
            Repositorio.Embarcador.Canhotos.ControleNotificacaoThread repControleNotificacaoThread = new Repositorio.Embarcador.Canhotos.ControleNotificacaoThread(unitOfWork);
            Repositorio.Embarcador.Canhotos.ControleNotificacao repControleNotificacao = new Repositorio.Embarcador.Canhotos.ControleNotificacao(unitOfWork);
            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto> repositorioConfiguracaoCanhoto = new RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto>(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto = repositorioConfiguracaoCanhoto.BuscarPrimeiroRegistro();

            DateTime dataInicial = new DateTime(2020, 5, 1);
            DateTime dataFinal = DateTime.Now.Date.AddDays(-1);

            Dominio.Entidades.Embarcador.Canhotos.ControleNotificacaoThread controleThread = repControleNotificacaoThread.BuscarPadrao();
            List<Dominio.Entidades.Embarcador.Canhotos.ControleNotificacao> notificacoes = repControleNotificacao.BuscarPorControles();
            IList<Dominio.ObjetosDeValor.Embarcador.Canhoto.NotificacaoCanhoto> canhotos = buscarPorTipoOperacao ? repCanhoto.ConsultaCanhotosNotificacaoPorTipoOperacao(dataInicial, dataFinal) : repCanhoto.ConsultaCanhotosNotificacao(dataInicial, dataFinal);

            if (tipoOperacao != null)
                canhotos = canhotos.Where(obj => obj.CodigoTipoOperacao == tipoOperacao.Codigo).ToList();

            int prazoDiasAposDataEmissao = tipoOperacao?.ConfiguracaoCanhoto?.PrazoAposDataEmissaoCanhoto > 0 ? tipoOperacao.ConfiguracaoCanhoto.PrazoAposDataEmissaoCanhoto : configuracaoCanhoto.PrazoDiasAposDataEmissao;

            //StringBuilder strTabela = new StringBuilder();
            ////strTabela.Append("Canhotos pendentes de digitalização da transportadora NOMETRANPS para filial NOMEFILIAL: " + Environment.NewLine + Environment.NewLine);
            //strTabela.Append("Embarcador: {{embarcador}}" + Environment.NewLine);
            //strTabela.Append("Prezado {{nometransportador}}" + Environment.NewLine + "Segue lista dos canhotos pendentes de digitalização para filial {{nomefilial}}" + Environment.NewLine + Environment.NewLine);
            //strTabela.Append("<table cellspacing=\"0\">");
            //strTabela.Append("<thead>");
            //strTabela.Append("<tr>");
            //strTabela.Append("<th style=\"padding:5px;border: 1px solid #ccc;\">Estabelecimento</th>");
            //strTabela.Append("<th style=\"padding:5px;border: 1px solid #ccc;\">Cliente</th>");
            //strTabela.Append("<th style=\"padding:5px;border: 1px solid #ccc;\">Número</th>");
            //strTabela.Append("<th style=\"padding:5px;border: 1px solid #ccc;\">Série</th>");
            //strTabela.Append("<th style=\"padding:5px;border: 1px solid #ccc;\">Data Emissão</th>");
            //strTabela.Append("</tr>");
            //strTabela.Append("</thead>");
            //strTabela.Append("<tbody>{{tabela}}</tbody>");
            //strTabela.Append("</table>" + Environment.NewLine);
            //strTabela.Append("<small>Esse e-mail é gerado automaticamente. Não responda.</small>");

            string template = new Canhoto(unitOfWork).ObterBodyBaseEmailCanhotosPendentes(configuracaoCanhoto.MensagemRodapeEmailCanhotosPendentes);

            List<int> transportadores = (from o in canhotos select o.Transportador).Distinct().ToList();
            List<int> filiais = (from o in canhotos select o.Filial).Distinct().ToList();

            for (int t = 0, st = transportadores.Count; t < st; t++)
            {
                int codigoTransportador = transportadores[t];
                Dominio.Entidades.Empresa transportador = repEmpresa.BuscarPorCodigo(codigoTransportador);

                for (int f = 0, sf = filiais.Count; f < sf; f++)
                {
                    int codigoFilial = filiais[f];
                    Dominio.Entidades.Embarcador.Filiais.Filial filial = repFilial.BuscarPorCodigo(codigoFilial);

                    List<Dominio.ObjetosDeValor.Embarcador.Canhoto.NotificacaoCanhoto> canhotosFiltrados = (from o in canhotos where o.Filial == codigoFilial && o.Transportador == codigoTransportador select o).ToList();
                    Dominio.ObjetosDeValor.Embarcador.Canhoto.NotificacaoCanhoto canhotoRef = canhotosFiltrados.FirstOrDefault();

                    Dominio.Entidades.Embarcador.Canhotos.ControleNotificacao controle = (from o in notificacoes
                                                                                          where o.Filial.Codigo == codigoFilial && o.Transportador.Codigo == codigoTransportador
                                                                                          select o).FirstOrDefault();
                    if (controle != null && controle.Finalizado)
                        continue;
                    if (canhotosFiltrados.Count == 0)
                        continue;
                    if (string.IsNullOrEmpty(canhotoRef.EmailTransportador))
                        continue;

                    string[] cc = canhotoRef.EmailFilial != null ? (from e in canhotoRef.EmailFilial.Split(';') where !string.IsNullOrEmpty(e) select e.Trim()).ToArray() : null;
                    string email = canhotoRef.EmailTransportador;
                    string strCanhotos = "";

                    foreach (Dominio.ObjetosDeValor.Embarcador.Canhoto.NotificacaoCanhoto canhoto in canhotosFiltrados)
                    {
                        //Dominio.ObjetosDeValor.Embarcador.Canhoto.NotificacaoCanhoto canhoto = canhotosFiltrados[c];
                        //strCanhotos.Append("<tr>");
                        //strCanhotos.Append($"<td style=\"padding:5px;border: 1px solid #ccc;\">{canhoto.Estabelecimento_Formatado}</td>");
                        //strCanhotos.Append($"<td style=\"padding:5px;border: 1px solid #ccc;\">{canhoto.Cliente}</td>");
                        //strCanhotos.Append($"<td style=\"padding:5px;border: 1px solid #ccc;\">{canhoto.Numero.ToString()}</td>");
                        //strCanhotos.Append($"<td style=\"padding:5px;border: 1px solid #ccc;\">{canhoto.Serie}</td>");
                        //strCanhotos.Append($"<td style=\"padding:5px;border: 1px solid #ccc;\">{canhoto.DataEmissao?.ToString("dd/MM/yyyy") ?? ""}</td>");
                        //strCanhotos.Append("</tr>");

                        strCanhotos += "<tr>";
                        strCanhotos += $"<td style=\"border: 1px solid #b9b5b5; padding: 10px; text-align: center;\">{canhoto.SituacaoDigitalizacao.ObterDescricao()}</td>";
                        strCanhotos += $"<td style=\"border: 1px solid #b9b5b5; padding: 10px; text-align: center;\">{canhoto.DataEmissao?.AddDays(prazoDiasAposDataEmissao).ToString("dd/MM/yyyy") ?? ""}</td>";
                        strCanhotos += $"<td style=\"border: 1px solid #b9b5b5; padding: 10px; text-align: center;\">{canhoto.Numero}</td>";
                        strCanhotos += $"<td style=\"border: 1px solid #b9b5b5; padding: 10px; text-align: center;\">{canhoto.TipoCanhoto.ObterDescricao()}</td>";
                        strCanhotos += $"<td style=\"border: 1px solid #b9b5b5; padding: 10px; text-align: center;\">{canhoto.DataEmissao?.ToString("dd/MM/yyyy")}</td>";
                        strCanhotos += $"<td style=\"border: 1px solid #b9b5b5; padding: 10px; text-align: center;\">{canhoto.NumeroCarga}</td>";
                        strCanhotos += $"<td style=\"border: 1px solid #b9b5b5; padding: 10px; text-align: center;\">{canhoto.DescricaoTransportador}</td>";
                        strCanhotos += $"<td style=\"border: 1px solid #b9b5b5; padding: 10px; text-align: center;\">{canhoto.TipoCarga}</td>";
                        strCanhotos += $"<td style=\"border: 1px solid #b9b5b5; padding: 10px; text-align: center;\">{canhoto.Destinatario}</td>";
                        strCanhotos += $"<td style=\"border: 1px solid #b9b5b5; padding: 10px; text-align: center;\">{canhoto.Filial}</td>";
                        strCanhotos += $"<td style=\"border: 1px solid #b9b5b5; padding: 10px; text-align: center;\">{canhoto.CTe}</td>";
                        strCanhotos += $"<td style=\"border: 1px solid #b9b5b5; padding: 10px; text-align: center;\">{canhoto.PlacaVeiculoRespEntrega}</td>";
                        strCanhotos += "</tr>";
                    }

                    string body = template
                        .Replace("{{nome-transportador}}", transportador.RazaoSocial)
                        .Replace("{{rows-tabela}}", strCanhotos)
                        .Replace("{{numero-canhotos-pendentes}}", canhotosFiltrados.Count.ToString());
                    //.Replace("{{nome-transportador}}", razaoSocial)
                    //.Replace("{{nomefilial}}", filial.Descricao);

                    if (!Servicos.Email.EnviarEmailAutenticado(email, $"Canhotos pendentes de digitalização ({transportador.NomeFantasia})", body, unitOfWork, out string msg, "", null, null, cc))
                    {
                        Servicos.Log.TratarErro("Falha ao enviar e-mail de acompanhamento de notificação canhoto para " + email + ": " + msg);
                    }
                    else
                    {
                        if (controle == null)
                        {
                            controle = new Dominio.Entidades.Embarcador.Canhotos.ControleNotificacao()
                            {
                                Finalizado = true,
                                Filial = filial,
                                Transportador = transportador
                            };
                            repControleNotificacao.Inserir(controle);
                        }
                        else
                        {
                            controle.Finalizado = true;
                            repControleNotificacao.Atualizar(controle);
                        }
                    }
                }
            }


            //            for (int i = 0, s = canhotos.Count; i < s; i++)
            //            {
            //                Dominio.ObjetosDeValor.Embarcador.Canhoto.NotificacaoCanhoto canhoto = canhotos[i];
            //                Dominio.Entidades.Embarcador.Canhotos.ControleNotificacao controle = (from o in notificacoes
            //                                                                                      where o.Filial.Codigo == canhoto.Filial && o.Transportador.Codigo == canhoto.Transportador
            //                                                                                      select o).FirstOrDefault();
            //                if (controle != null && controle.Finalizado)
            //                    continue;
            //                if (string.IsNullOrWhiteSpace(canhoto.Numero))
            //                    continue;

            //                string[] cc = canhoto.EmailFilial != null ? (from e in canhoto.EmailFilial.Split(';') where !string.IsNullOrEmpty(e) select e.Trim()).ToArray() : null;
            //                string email = canhoto.EmailTransportador;
            //                string strCanhotos = String.Join("", (from o in canhoto.Numero.Split(',') select $"<tr><td>{o}</td></tr>"));
            //                string body = template.Replace("{{canhotos}}", strCanhotos);

            //#if DEBUG
            //                email = "lucassmahle@gmail.com";
            //#endif

            //                if (!Servicos.Email.EnviarEmailAutenticado(email, "Canhotos não recebidos", body, unitOfWork, out string msg, null, null, cc))
            //                {
            //                    Servicos.Log.TratarErro("Falha ao enviar e-mail de acompanhamento de notificação canhoto para " + email + ": " + msg);
            //                }
            //                else
            //                {
            //                    if (controle == null)
            //                    {
            //                        controle = new Dominio.Entidades.Embarcador.Canhotos.ControleNotificacao()
            //                        {
            //                            Finalizado = true,
            //                            Filial = repFilial.BuscarPorCodigo(canhoto.Filial),
            //                            Transportador = repEmpresa.BuscarPorCodigo(canhoto.Transportador)
            //                        };
            //                        repControleNotificacao.Inserir(controle);
            //                    }
            //                    else
            //                    {
            //                        controle.Finalizado = true;
            //                        repControleNotificacao.Atualizar(controle);
            //                    }
            //                }
            //            }

            // Libera para proximo processamento
            repControleNotificacao.SetarNotificacoesFinalizadas();
            controleThread.DataUltimoProcessamento = DateTime.Today;
            repControleNotificacaoThread.Atualizar(controleThread);
        }

        public bool GerarOcorrenciaEntregaCanhoto(Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrencia, Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, Dominio.Entidades.Usuario usuario, string observacao, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, ref string retorno, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
        {
            retorno = string.Empty;

            if (tipoDeOcorrencia == null)
                return true;

            if (canhoto.Carga == null || canhoto.XMLNotaFiscal == null || canhoto.XMLNotaFiscal.CTEs == null || canhoto.XMLNotaFiscal.CTEs.Count == 0)
                return true;

            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTEsOcorrencia = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTEs = repCargaCTe.BuscarPorCarga(canhoto.Carga.Codigo, 0);
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCTEs)
            {
                if (canhoto.XMLNotaFiscal.CTEs.Contains(cargaCTe.CTe))
                {
                    if (!repOcorrencia.ContemOcorrenciaCargaCTeTipoOcorrencia(cargaCTe.Codigo, tipoDeOcorrencia.Codigo))
                        cargaCTEsOcorrencia.Add(cargaCTe);
                }
            }

            if (cargaCTEsOcorrencia == null || cargaCTEsOcorrencia.Count == 0)
                return true;

            Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia()
            {
                NumeroOcorrencia = Servicos.Embarcador.CargaOcorrencia.OcorrenciaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork),
                Carga = canhoto.Carga,
                ComponenteFrete = null,
                DataOcorrencia = DateTime.Now,
                DataFinalizacaoEmissaoOcorrencia = DateTime.Now,
                ObservacaoCTe = observacao,
                Observacao = observacao,
                SituacaoOcorrencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Finalizada,
                TipoOcorrencia = tipoDeOcorrencia,
                ValorOcorrencia = 0,
                ComplementoValorFreteCarga = false,
                DataAlteracao = DateTime.Now,
                OrigemOcorrencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemOcorrencia.PorCarga
            };

            string msgRetorno = string.Empty;

            if (!GerarOcorrencia(ref msgRetorno, ref cargaOcorrencia, unitOfWork, tipoServicoMultisoftware, usuario, auditado, configuracaoEmbarcador, clienteMultisoftware, cargaCTEsOcorrencia))
            {
                retorno = msgRetorno;
                return false;
            }

            return true;
        }

        private bool GerarOcorrencia(ref string MensagemRetorno, ref Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Usuario usuario, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTEs)
        {
            Servicos.Embarcador.CargaOcorrencia.Ocorrencia srvOcorrencia = new Servicos.Embarcador.CargaOcorrencia.Ocorrencia(unitOfWork);

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Frete.ContratoFreteTransportador repContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
            Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe repTransportadorConfiguracaoNFSe = new Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe(unitOfWork);

            MensagemRetorno = string.Empty;

            if (ocorrencia.TipoOcorrencia == null)
            {
                MensagemRetorno = "É obrigatório selecionar o tipo de ocorrência.";
                return false;
            }
            ocorrencia.OrigemOcorrencia = ocorrencia.TipoOcorrencia.OrigemOcorrencia;

            if (cargaCTEs.Count > 50)
            {
                MensagemRetorno = "A quantidade máxima de CT-es permitida para a geração da ocorrência é de 50. Selecione menos CT-es ou faça duas ocorrências caso necessário.";
                return false;
            }

            //Validação de clientes bloqueados para lançamento de ocorrência
            if (ocorrencia.TipoOcorrencia.ClientesBloqueados != null && ocorrencia.TipoOcorrencia.ClientesBloqueados.Count > 0 && cargaCTEs.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.Ocorrencias.ClientesBloqueados clienteBloqueado in ocorrencia.TipoOcorrencia.ClientesBloqueados)
                {
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCTEs)
                    {
                        if (cargaCTe.CTe != null)
                        {
                            if (clienteBloqueado.TipoCliente == Dominio.Enumeradores.TipoTomador.Remetente)
                            {
                                if (clienteBloqueado.Cliente.CPF_CNPJ_SemFormato == cargaCTe.CTe.Remetente.CPF_CNPJ_SemFormato)
                                {
                                    MensagemRetorno = "Cliente origem " + clienteBloqueado.Cliente.CPF_CNPJ_SemFormato + (!string.IsNullOrWhiteSpace(clienteBloqueado.Cliente.CodigoIntegracao) ? " (" + clienteBloqueado.Cliente.CodigoIntegracao + ") " : " ") + clienteBloqueado.Cliente.Nome + " não permitido para lançamento da ocorrência.";
                                    return false;
                                }
                            }
                            else
                            if (clienteBloqueado.TipoCliente == Dominio.Enumeradores.TipoTomador.Destinatario)
                            {
                                if (clienteBloqueado.Cliente.CPF_CNPJ_SemFormato == cargaCTe.CTe.Destinatario.CPF_CNPJ_SemFormato)
                                {
                                    MensagemRetorno = "Cliente destino " + clienteBloqueado.Cliente.CPF_CNPJ_SemFormato + (!string.IsNullOrWhiteSpace(clienteBloqueado.Cliente.CodigoIntegracao) ? " (" + clienteBloqueado.Cliente.CodigoIntegracao + ") " : " ") + clienteBloqueado.Cliente.Nome + " não permitido para lançamento da ocorrência.";
                                    return false;
                                }
                            }
                        }
                    }

                }
            }

            srvOcorrencia.SetaEmitenteOcorrencia(ref ocorrencia, unitOfWork, tipoServicoMultisoftware, usuario);
            srvOcorrencia.SetaEmitenteOcorrencia(ref ocorrencia, unitOfWork, tipoServicoMultisoftware, usuario);

            if (ocorrencia.TipoOcorrencia.BloqueiaOcorrenciaDuplicada)
            {
                if (ocorrencia.OrigemOcorrenciaPorPeriodo)
                {
                    // Validacao de ocorrencia por periodo
                    if (!srvOcorrencia.ValidaSeExisteOcorrenciaPorPeriodo(ocorrencia, out string erro, unitOfWork, usuario))
                    {
                        MensagemRetorno = erro;
                        return false;
                    }
                }
                else
                {
                    // Validacao de ocorrencia por CTe
                    if (!srvOcorrencia.ValidaSeExisteOcorrenciaPorCTe(cargaCTEs, ocorrencia, out string erro, unitOfWork, tipoServicoMultisoftware))
                    {
                        MensagemRetorno = erro;
                        return false;
                    }
                }
            }

            if (!srvOcorrencia.SetaModeloDocumentoFiscal(ref ocorrencia, cargaCTEs, out string erroModeloDocumento, unitOfWork, tipoServicoMultisoftware))
            {
                MensagemRetorno = erroModeloDocumento;
                return false;
            }

            //-- Persistencia dos dados
            //-------------------------
            repOcorrencia.Inserir(ocorrencia, auditado);

            Servicos.Embarcador.Integracao.IntegracaoOcorrencia.AdicionarIntegracoesOcorrencia(ocorrencia, cargaCTEs, unitOfWork);

            if (ocorrencia.OrigemOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemOcorrencia.PorPeriodo)
            {
                // Dados Sumarizados carga ocorrencia
                srvOcorrencia.SalvarCargasSumarizadas(ocorrencia, unitOfWork, tipoServicoMultisoftware, usuario, 0, 0, "");

                // Atuliza valor da ocorrencia
                ocorrencia.ValorOcorrencia = srvOcorrencia.CalcularValorOcorrenciaPorTipoOcorrencia(ocorrencia);
            }

            // Detalhes da ocorrencia
            string mensagemRetorno = string.Empty;
            if (!srvOcorrencia.FluxoGeralOcorrencia(ref ocorrencia, cargaCTEs, null, ref mensagemRetorno, unitOfWork, tipoServicoMultisoftware, usuario, configuracaoEmbarcador, clienteMultisoftware, "", true, false, auditado))
            {
                MensagemRetorno = mensagemRetorno;
                return false;
            }

            repOcorrencia.Atualizar(ocorrencia);

            return true;
        }

        public void GerarHistoricoCanhoto(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, Dominio.Entidades.Usuario usuario, string observacao, Repositorio.UnitOfWork unitOfWork, string observacaoOperador = null)
        {
            Repositorio.Embarcador.Canhotos.CanhotoHistorico repCanhotoHistorico = new Repositorio.Embarcador.Canhotos.CanhotoHistorico(unitOfWork);

            Dominio.Entidades.Embarcador.Canhotos.CanhotoHistorico canhotoHistorico = new Dominio.Entidades.Embarcador.Canhotos.CanhotoHistorico
            {
                Canhoto = canhoto,
                LocalArmazenamentoCanhoto = canhoto.LocalArmazenamentoCanhoto,
                PosicaoNoPacote = canhoto.PosicaoNoPacote,
                PacoteArmazenado = canhoto.PacoteArmazenado,
                DataHistorico = DateTime.Now,
                Observacao = observacao,
                SituacaoCanhoto = canhoto.SituacaoCanhoto,
                SituacaoDigitalizacaoCanhoto = canhoto.SituacaoDigitalizacaoCanhoto,
                Usuario = usuario,
                ObservacaoOperador = observacaoOperador
            };

            repCanhotoHistorico.Inserir(canhotoHistorico);

        }

        public async Task GerarHistoricoCanhotoAsync(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, Dominio.Entidades.Usuario usuario, string observacao, string observacaoOperador = null)
        {
            Repositorio.Embarcador.Canhotos.CanhotoHistorico repCanhotoHistorico = new Repositorio.Embarcador.Canhotos.CanhotoHistorico(_unitOfWork, _cancellationToken);

            Dominio.Entidades.Embarcador.Canhotos.CanhotoHistorico canhotoHistorico = new Dominio.Entidades.Embarcador.Canhotos.CanhotoHistorico
            {
                Canhoto = canhoto,
                LocalArmazenamentoCanhoto = canhoto.LocalArmazenamentoCanhoto,
                PosicaoNoPacote = canhoto.PosicaoNoPacote,
                PacoteArmazenado = canhoto.PacoteArmazenado,
                DataHistorico = DateTime.Now,
                Observacao = observacao,
                SituacaoCanhoto = canhoto.SituacaoCanhoto,
                SituacaoDigitalizacaoCanhoto = canhoto.SituacaoDigitalizacaoCanhoto,
                Usuario = usuario,
                ObservacaoOperador = observacaoOperador
            };

            await repCanhotoHistorico.InserirAsync(canhotoHistorico);

        }

        public Dominio.Entidades.Embarcador.Canhotos.CanhotoHistorico GerarHistoricoCanhotoLote(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, Dominio.Entidades.Usuario usuario, string observacao, string observacaoOperador = null)
        {
            Dominio.Entidades.Embarcador.Canhotos.CanhotoHistorico canhotoHistorico = new Dominio.Entidades.Embarcador.Canhotos.CanhotoHistorico
            {
                Canhoto = canhoto,
                LocalArmazenamentoCanhoto = canhoto.LocalArmazenamentoCanhoto,
                PosicaoNoPacote = canhoto.PosicaoNoPacote,
                PacoteArmazenado = canhoto.PacoteArmazenado,
                DataHistorico = DateTime.Now,
                Observacao = observacao,
                SituacaoCanhoto = canhoto.SituacaoCanhoto,
                SituacaoDigitalizacaoCanhoto = canhoto.SituacaoDigitalizacaoCanhoto,
                Usuario = usuario,
                ObservacaoOperador = observacaoOperador
            };

            return canhotoHistorico;
        }

        public static void GerarHistoricoCanhotos(List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos, Dominio.Entidades.Usuario usuario, string observacao, Repositorio.UnitOfWork unitOfWork, string observacaoOperador = null)
        {
            try
            {
                List<Dominio.Entidades.Embarcador.Canhotos.CanhotoHistorico> canhotoHistoricos = new();
                Repositorio.Embarcador.Canhotos.CanhotoHistorico repCanhotoHistorico = new Repositorio.Embarcador.Canhotos.CanhotoHistorico(unitOfWork);
                foreach (Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto in canhotos)
                {
                    Dominio.Entidades.Embarcador.Canhotos.CanhotoHistorico canhotoHistorico = new Dominio.Entidades.Embarcador.Canhotos.CanhotoHistorico
                    {
                        Canhoto = canhoto,
                        LocalArmazenamentoCanhoto = canhoto.LocalArmazenamentoCanhoto,
                        PosicaoNoPacote = canhoto.PosicaoNoPacote,
                        PacoteArmazenado = canhoto.PacoteArmazenado,
                        DataHistorico = DateTime.Now,
                        Observacao = observacao,
                        SituacaoCanhoto = canhoto.SituacaoCanhoto,
                        SituacaoDigitalizacaoCanhoto = canhoto.SituacaoDigitalizacaoCanhoto,
                        Usuario = usuario,
                        ObservacaoOperador = observacaoOperador
                    };
                    canhotoHistoricos.Add(canhotoHistorico);
                }
                repCanhotoHistorico.Inserir(canhotoHistoricos, "T_CANHOTO_NOTA_FISCAL_HISTORICO");
            }
            catch (Exception ex)
            {
                /* Enterra para seguir o processo */
                Servicos.Log.TratarErro(ex, "ControleEntregaQualidade");
            }

        }

        public bool ValidarSituacaoIntegracaoCanhoto(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, Repositorio.UnitOfWork unitOfWork)
        {
            //buscar o cte do canhoto e atravez do cte validar se possui integracao de canhoto com integracao "confirmacao"
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.CTe.CTeCanhotoIntegracao repCTeIntegracao = new Repositorio.Embarcador.CTe.CTeCanhotoIntegracao(unitOfWork);

            List<TipoIntegracao> tiposIntegracaoGeracaoCteCanhoto = new List<TipoIntegracao>();
            tiposIntegracaoGeracaoCteCanhoto.Add(TipoIntegracao.Unilever);

            if (!repTipoIntegracao.ExistePorTipo(tiposIntegracaoGeracaoCteCanhoto))
                return true;

            string chaveXmlNotaFiscal = "";

            if (canhoto.TipoCanhoto == TipoCanhoto.NFe)
                chaveXmlNotaFiscal = canhoto.XMLNotaFiscal?.Chave;
            else if (canhoto.TipoCanhoto == TipoCanhoto.Avulso && canhoto.CanhotoAvulso.PedidosXMLNotasFiscais.Count > 0)
                chaveXmlNotaFiscal = canhoto.CanhotoAvulso.PedidosXMLNotasFiscais.FirstOrDefault()?.XMLNotaFiscal?.Chave;

            if (string.IsNullOrEmpty(chaveXmlNotaFiscal))
                return true;

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTe = repCTe.BuscarListaPorChaveNFe(Utilidades.String.OnlyNumbers(chaveXmlNotaFiscal));

            foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in listaCTe)
            {
                if (repCTeIntegracao.ExistePorCTeETipoRegistroIntegracao(cte.Codigo, TipoRegistroIntegracaoCTeCanhoto.Confirmacao))
                    return false;
            }

            return true;
        }

        public string ObterDiretorioArquivo(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, Repositorio.UnitOfWork unitOfWork)
        {
            string caminho = CaminhoCanhoto(canhoto, unitOfWork);
            string extensao = Path.GetExtension(canhoto.NomeArquivo).ToLower();
            string nomeArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, canhoto.GuidNomeArquivo + extensao);

            return nomeArquivo;
        }

        public string ObterMiniatura(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, Repositorio.UnitOfWork unitOfWork)
        {
            string caminho = CaminhoCanhoto(canhoto, unitOfWork);
            string extensao = Path.GetExtension(canhoto.NomeArquivo).ToLower();
            string nomeArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, canhoto.GuidNomeArquivo + extensao);

            Servicos.Log.TratarErro($"Canhoto miniatura {canhoto.Codigo} obtendo", "ObterMiniaturaLog");

            return ObterMiniatura(nomeArquivo);
        }

        public string ObterMiniatura(Dominio.Entidades.Embarcador.Canhotos.CanhotoEsperandoVinculo canhotoEsperandoVinculo, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivos = Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo();

            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(configuracaoArquivos.CaminhoCanhotos, "CanhotosEsperandoVinculo");
            string nomeArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, canhotoEsperandoVinculo.NomeArquivo);

            return ObterMiniatura(nomeArquivo);
        }

        public int VerificarQuantidadeCanhotosPendenteAceiteImagem(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);

            Servicos.Embarcador.Carga.Carga serCarga = new Carga.Carga(unitOfWork);
            Servicos.Embarcador.Terceiros.ContratoFrete serContratoFrete = new Terceiros.ContratoFrete(unitOfWork);

            int numeroSemEnvio = repCanhoto.ContarCanhotosNaoEnviadosComEnvioImagemAceitoPorCarga(carga.Codigo);

            if (numeroSemEnvio == 0)
            {
                Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repContratoFrete.BuscarPorCarga(carga.Codigo);

                if (contratoFrete != null && contratoFrete.SituacaoContratoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Aprovado)
                {
                    Repositorio.Embarcador.Terceiros.ContratoFreteCTe repCargaFreteSubContratacaoPreCTe = new Repositorio.Embarcador.Terceiros.ContratoFreteCTe(unitOfWork);

                    int countPendentesEnvio = repCargaFreteSubContratacaoPreCTe.ContarPorCargaFreteSubContratacaoPendentesEnvio(contratoFrete.Codigo);

                    if (countPendentesEnvio <= 0)
                        serContratoFrete.SolicitarFinalizacaoContratoFrete(contratoFrete, tipoServicoMultisoftware, unitOfWork, auditado);
                }
            }

            return numeroSemEnvio;
        }

        public int VerificarQuantidadeCanhotosPendenteEnvioCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);

            Servicos.Embarcador.Carga.Carga serCarga = new Carga.Carga(unitOfWork);
            Servicos.Embarcador.Terceiros.ContratoFrete serContratoFrete = new Terceiros.ContratoFrete(unitOfWork);

            int numeroSemEnvio = repCanhoto.ContarCanhotosNaoEnviadosPorCarga(carga.Codigo);
            if (numeroSemEnvio == 0)
            {
                Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repContratoFrete.BuscarPorCarga(carga.Codigo);
                if (contratoFrete != null && contratoFrete.SituacaoContratoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Aprovado)
                {
                    Repositorio.Embarcador.Terceiros.ContratoFreteCTe repCargaFreteSubContratacaoPreCTe = new Repositorio.Embarcador.Terceiros.ContratoFreteCTe(unitOfWork);

                    int countPendentesEnvio = repCargaFreteSubContratacaoPreCTe.ContarPorCargaFreteSubContratacaoPendentesEnvio(contratoFrete.Codigo);

                    if (countPendentesEnvio <= 0)
                        serContratoFrete.SolicitarFinalizacaoContratoFrete(contratoFrete, tipoServicoMultisoftware, unitOfWork, auditado);
                }
            }

            return numeroSemEnvio;
        }

        public dynamic RetornarDadosLocalArmazenamento(Dominio.Entidades.Embarcador.Canhotos.LocalArmazenamentoCanhoto localArmazenamento)
        {
            var dynLocalArmazenamentoCanhoto = new
            {
                localArmazenamento.Codigo,
                localArmazenamento.CapacidadeArmazenagem,
                localArmazenamento.Descricao,
                localArmazenamento.TipoCanhoto,
                localArmazenamento.Observacao,
                localArmazenamento.PacoteAtual,
                localArmazenamento.DividirEmPacotesDe,
                localArmazenamento.QuantidadeArmazenada,
                localArmazenamento.LocalArmazenagemAtual,
                TotalDePacotes = localArmazenamento.DividirEmPacotesDe > 0 ? localArmazenamento.CapacidadeArmazenagem / localArmazenamento.DividirEmPacotesDe : 0,
                LocalCheio = localArmazenamento.CapacidadeArmazenagem == localArmazenamento.QuantidadeArmazenada ? true : false,
            };
            return dynLocalArmazenamentoCanhoto;
        }

        public void ExcluirCanhotoDaNotaFiscal(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
            Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = repCanhoto.BuscarPorNF(xmlNotaFiscal.Codigo);

            if (canhoto != null)
            {
                Repositorio.Embarcador.Canhotos.CanhotoHistorico repCanhotoHistorico = new Repositorio.Embarcador.Canhotos.CanhotoHistorico(unitOfWork);
                Repositorio.Embarcador.Canhotos.ControleLeituraImagemCanhoto repControleLeituraImagemCanhoto = new Repositorio.Embarcador.Canhotos.ControleLeituraImagemCanhoto(unitOfWork);

                List<Dominio.Entidades.Embarcador.Canhotos.CanhotoHistorico> historicos = repCanhotoHistorico.Consultar(canhoto.Codigo);
                foreach (Dominio.Entidades.Embarcador.Canhotos.CanhotoHistorico historico in historicos)
                    repCanhotoHistorico.Deletar(historico);

                Dominio.Entidades.Embarcador.Canhotos.ControleLeituraImagemCanhoto controle = repControleLeituraImagemCanhoto.BuscarPorCodigoOuCanhoto(0, canhoto.Codigo);
                if (controle != null)
                    repControleLeituraImagemCanhoto.Deletar(controle);

                xmlNotaFiscal.Canhoto.MotoristasResponsaveis.Clear();
                repCanhoto.Deletar(canhoto);
            }
        }

        public void ExcluirCanhotoDoCTe(Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
            Repositorio.Embarcador.Canhotos.CanhotoHistorico repCanhotoHistorico = new Repositorio.Embarcador.Canhotos.CanhotoHistorico(unitOfWork);

            Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = repCanhoto.BuscarPorCTe(cteTerceiro.Codigo);

            if (canhoto != null)
            {
                List<Dominio.Entidades.Embarcador.Canhotos.CanhotoHistorico> historicos = repCanhotoHistorico.Consultar(canhoto.Codigo);

                foreach (Dominio.Entidades.Embarcador.Canhotos.CanhotoHistorico historico in historicos)
                    repCanhotoHistorico.Deletar(historico);

                repCanhoto.Deletar(canhoto);
            }

        }

        public void InativarCanhotosAvulsosCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Canhotos.CanhotoAvulso repCanhotoAvulso = new Repositorio.Embarcador.Canhotos.CanhotoAvulso(unitOfWork);
            List<Dominio.Entidades.Embarcador.Canhotos.CanhotoAvulso> canhotosAvulsos = repCanhotoAvulso.BuscarTodosPorCarga(carga.Codigo);
            foreach (Dominio.Entidades.Embarcador.Canhotos.CanhotoAvulso canhotoAvulso in canhotosAvulsos)
            {
                canhotoAvulso.Ativo = false;
                repCanhotoAvulso.Atualizar(canhotoAvulso);
            }
        }

        //Quando altera o responsavel pelo canhoto na carga (altera o motorista, o terceiro, etc da carga) e as notas já tem canhoto gerado é necessário atualizar.
        public void AtualizarDadosCanhotosDaCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
            List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos = repCanhoto.BuscarPorCarga(carga.Codigo);

            foreach (Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto in canhotos)
            {
                canhoto.TerceiroResponsavel = carga.Veiculo != null && carga.FreteDeTerceiro ? carga.Veiculo.Proprietario : null;
                canhoto.MotoristasResponsaveis = carga.Motoristas?.ToList();
                repCanhoto.Atualizar(canhoto);
            }
        }

        public void CriarCanhotoAvulso(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);

            Repositorio.Embarcador.Canhotos.CanhotoAvulso repCanhotoAvulso = new Repositorio.Embarcador.Canhotos.CanhotoAvulso(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            if (carga.TipoOperacao?.NaoGerarCanhoto ?? false)
                return;

            List<Dominio.Entidades.Cliente> recebedores = (from obj in cargaPedidos where obj.Recebedor != null select obj.Recebedor).Distinct().ToList();

            foreach (Dominio.Entidades.Cliente recebedor in recebedores)
            {
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXMLsNotasFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidoRecebedor = (from obj in cargaPedidos where obj.Recebedor != null && obj.Recebedor.Codigo == recebedor.Codigo select obj).ToList();

                string numeroCarga = "";
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidoRecebedor)
                {
                    pedidosXMLsNotasFiscais.AddRange(repPedidoXMLNotaFiscal.BuscarPorCargaPedido(cargaPedido.Codigo));
                    numeroCarga = cargaPedido.Pedido.CodigoCargaEmbarcador;
                }

                Dominio.Entidades.Embarcador.Canhotos.CanhotoAvulso canhotoAvulso = new Dominio.Entidades.Embarcador.Canhotos.CanhotoAvulso();
                if (configuracaoTMS.UsarNumeroCargaParaNumeroCanhotoAvulso)
                {
                    int numeroCanhoto = 0;
                    int.TryParse(Utilidades.String.OnlyNumbers(numeroCarga), out numeroCanhoto);
                    if (numeroCanhoto > 0)
                        canhotoAvulso.Numero = numeroCanhoto;
                    else
                        canhotoAvulso.Numero = carga.Codigo;
                }
                else
                    canhotoAvulso.Numero = repCanhotoAvulso.BuscarProximoCodigo();

                canhotoAvulso.Recebedor = recebedor;
                canhotoAvulso.Ativo = true;
                canhotoAvulso.QRCode = "AV_" + Guid.NewGuid().ToString().Replace("-", "");

                canhotoAvulso.PedidosXMLNotasFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in pedidosXMLsNotasFiscais)
                    canhotoAvulso.PedidosXMLNotasFiscais.Add(pedidoXMLNotaFiscal);

                Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscalPadrao = pedidosXMLsNotasFiscais.FirstOrDefault();


                if (canhotoAvulso.PedidosXMLNotasFiscais.Count > 0)
                {
                    canhotoAvulso.Codigo = (int)repCanhotoAvulso.Inserir(canhotoAvulso);

                    Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = new Dominio.Entidades.Embarcador.Canhotos.Canhoto();
                    canhoto.DataEnvioCanhoto = DateTime.Now;
                    canhoto.GuidNomeArquivo = "";
                    canhoto.NomeArquivo = "";
                    canhoto.Observacao = "";
                    canhoto.TipoCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.Avulso;
                    canhoto.SituacaoDigitalizacaoCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.PendenteDigitalizacao;
                    canhoto.SituacaoCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Pendente;
                    canhoto.Carga = carga;
                    canhoto.Emitente = pedidoXMLNotaFiscalPadrao.XMLNotaFiscal.Emitente;
                    if (canhoto.EmpresaFilialEmissora == null)
                        canhoto.Empresa = carga.Empresa;
                    canhoto.EmpresaFilialEmissora = carga.EmpresaFilialEmissora;
                    canhoto.DataUltimaModificacao = DateTime.Now;
                    canhoto.CanhotoAvulso = canhotoAvulso;
                    canhoto.DataEmissao = carga.DataFinalizacaoEmissao.HasValue ? carga.DataFinalizacaoEmissao.Value : DateTime.Now;
                    canhoto.Filial = carga.Filial;
                    canhoto.Destinatario = recebedor;
                    canhoto.Numero = canhotoAvulso.Numero;
                    canhoto.Serie = "";
                    canhoto.Peso = pedidosXMLsNotasFiscais.Sum(obj => obj.XMLNotaFiscal.Peso);
                    canhoto.Valor = pedidosXMLsNotasFiscais.Sum(obj => obj.XMLNotaFiscal.Valor);
                    canhoto.ModalidadeFrete = pedidoXMLNotaFiscalPadrao.XMLNotaFiscal.ModalidadeFrete;
                    repCanhoto.Inserir(canhoto);

                    GerarHistoricoCanhoto(canhoto, null, "Canhoto avulso criado e aguardando o recebimento.", unitOfWork);
                }

            }
        }

        public void CriarCanhotoDevolucao(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Cliente recebedor, Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, Dominio.Entidades.Embarcador.Filiais.Filial filial, Dominio.Entidades.Empresa transportador, int numeroCanhoto, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);

            Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = new Dominio.Entidades.Embarcador.Canhotos.Canhoto();
            canhoto.DataEnvioCanhoto = DateTime.Now;
            canhoto.GuidNomeArquivo = "";
            canhoto.NomeArquivo = "";
            canhoto.Observacao = "";
            canhoto.TipoCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.NFe;
            canhoto.SituacaoDigitalizacaoCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.PendenteDigitalizacao;
            canhoto.SituacaoCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Pendente;
            canhoto.Carga = carga;
            canhoto.Emitente = xmlNotaFiscal.Emitente;
            canhoto.Empresa = transportador;
            canhoto.DataUltimaModificacao = DateTime.Now;
            canhoto.DataEmissao = carga.DataFinalizacaoEmissao.HasValue ? carga.DataFinalizacaoEmissao.Value : DateTime.Now;
            canhoto.Filial = filial;
            canhoto.Destinatario = recebedor;
            canhoto.Numero = numeroCanhoto;
            canhoto.Serie = "";
            canhoto.Peso = xmlNotaFiscal.Peso;
            canhoto.Valor = xmlNotaFiscal.Valor;
            canhoto.ModalidadeFrete = xmlNotaFiscal.ModalidadeFrete;
            canhoto.XMLNotaFiscal = xmlNotaFiscal;

            repCanhoto.Inserir(canhoto);

            GerarHistoricoCanhoto(canhoto, null, "Canhoto avulso criado e aguardando o recebimento.", unitOfWork);
        }

        public static string CaminhoCanhoto(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, Repositorio.UnitOfWork unitOfWork = null)
        {
            Dominio.ObjetosDeValor.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivos = Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo();

            string caminho;
            string path = configuracaoArquivos.CaminhoCanhotos;

#if DEBUG
            path = "C:\\Arquivos\\Canhotos";
#endif

            if (string.IsNullOrEmpty(path) && unitOfWork != null)
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo repositorioConfiguracaoArquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = repositorioConfiguracaoArquivo.BuscarPrimeiroRegistro();

                if (configuracaoArquivo != null)
                    path = configuracaoArquivo.CaminhoCanhotos;
            }

            if (canhoto.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.NFe)
                caminho = Utilidades.IO.FileStorageService.Storage.Combine(path, canhoto.Emitente.CPF_CNPJ_SemFormato);
            else if (canhoto.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.Avulso)
                caminho = Utilidades.IO.FileStorageService.Storage.Combine(path, "CanhotosAvulsos");
            else if (canhoto.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.CTe)
                caminho = Utilidades.IO.FileStorageService.Storage.Combine(path, canhoto.Emitente.CPF_CNPJ_SemFormato, "CTe");
            else if (canhoto.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.CTeSubcontratacao)
                caminho = Utilidades.IO.FileStorageService.Storage.Combine(path, canhoto.Emitente.CPF_CNPJ_SemFormato, "CTeSubcontratacao");
            else
                throw new NotImplementedException("Não foram implementados outros modelos de canhotos");

            string caminhoCanhotosAntigos = configuracaoArquivos.CaminhoCanhotosAntigos;

            if (!string.IsNullOrWhiteSpace(caminhoCanhotosAntigos))
            {
                string extensao = System.IO.Path.GetExtension(canhoto.NomeArquivo).ToLower();
                string filepath = Utilidades.IO.FileStorageService.Storage.Combine(caminho, canhoto.GuidNomeArquivo + extensao);

                string caminhoAntigo = "";
                if (!Utilidades.IO.FileStorageService.Storage.Exists(filepath))
                {
                    if (canhoto.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.NFe)
                        caminhoAntigo = Utilidades.IO.FileStorageService.Storage.Combine(caminhoCanhotosAntigos, canhoto.Emitente.CPF_CNPJ_SemFormato);
                    else if (canhoto.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.Avulso)
                        caminhoAntigo = Utilidades.IO.FileStorageService.Storage.Combine(caminhoCanhotosAntigos, "CanhotosAvulsos");
                    else if (canhoto.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.CTe)
                        caminhoAntigo = Utilidades.IO.FileStorageService.Storage.Combine(caminhoCanhotosAntigos, canhoto.Emitente.CPF_CNPJ_SemFormato, "CTe");
                    else if (canhoto.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.CTeSubcontratacao)
                        caminhoAntigo = Utilidades.IO.FileStorageService.Storage.Combine(caminhoCanhotosAntigos, canhoto.Emitente.CPF_CNPJ_SemFormato, "CTeSubcontratacao");

                    extensao = Path.GetExtension(canhoto.NomeArquivo).ToLower();
                    filepath = Utilidades.IO.FileStorageService.Storage.Combine(caminhoAntigo, canhoto.GuidNomeArquivo + extensao);

                    if (Utilidades.IO.FileStorageService.Storage.Exists(filepath))
                        caminho = caminhoAntigo;
                }
            }

            return caminho;
        }

        public void AjustarCanhotosCargaCancelada(List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscaisAjustarCanhoto, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (pedidoXMLNotaFiscaisAjustarCanhoto.Count > 0)
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto repConfiguracaoCanhoto = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto = repConfiguracaoCanhoto.BuscarConfiguracaoPadrao();
                Repositorio.Embarcador.Cargas.CargaMotorista repCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> cargaMotoristas = repCargaMotorista.BuscarPorCargas((from obj in pedidoXMLNotaFiscaisAjustarCanhoto select obj.CargaPedido.Carga.Codigo).Distinct().ToList());

                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> xMLNotaFiscais = (from obj in pedidoXMLNotaFiscaisAjustarCanhoto where obj.XMLNotaFiscal.Canhoto != null select obj.XMLNotaFiscal).Distinct().ToList();
                foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal in xMLNotaFiscais)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = (from obj in pedidoXMLNotaFiscaisAjustarCanhoto where obj.XMLNotaFiscal.Codigo == xmlNotaFiscal.Codigo orderby obj.CargaPedido.Codigo descending select obj).FirstOrDefault();
                    if (pedidoXMLNotaFiscal != null)
                        SalvarCanhotoNota(xmlNotaFiscal, pedidoXMLNotaFiscal.CargaPedido, null, (from obj in cargaMotoristas where obj.Carga.Codigo == pedidoXMLNotaFiscal.CargaPedido.Codigo select obj.Motorista).ToList(), tipoServicoMultisoftware, configuracao, unitOfWork, configuracaoCanhoto, "", true);
                }
            }
        }

        public void AjustarCanhotosCargaTransbordo(Dominio.Entidades.Embarcador.Cargas.Carga cargaCancelada, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Transbordo repTransbordo = new Repositorio.Embarcador.Cargas.Transbordo(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Transbordo transbordo = repTransbordo.BuscarPorCargaGerada(cargaCancelada.Codigo);

            if (transbordo != null)
            {
                Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);

                List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> listaCanhotos = repCanhoto.BuscarPorCarga(cargaCancelada.Codigo);
                foreach (Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto in listaCanhotos)
                {
                    //Não pode duplicar o canhoto, tem que migrar para carga nova
                    canhoto.Carga = transbordo.Carga;
                    repCanhoto.Atualizar(canhoto);
                }
            }

        }

        public bool SalvarCanhotoNota(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Cliente terceiroResponsavel, List<Dominio.Entidades.Usuario> motoristasResponsaveis, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto, string numeroDocumentoOriginario = "", bool cancelamento = false)
        {
            if (cargaPedido?.Carga?.TipoOperacao?.NaoGerarCanhoto ?? false)
                return false;

            if ((cargaPedido?.Carga?.CargaSVM ?? false) || (cargaPedido?.Carga?.CargaTakeOrPay ?? false))
                return false;

            if (xmlNotaFiscal.TipoDocumento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe && configuracao.Pais != TipoPais.Exterior)
                return false;

            if (!DeveGerarCanhotoNotaFiscal(xmlNotaFiscal, cargaPedido, tipoServicoMultisoftware, unitOfWork, configuracaoCanhoto, configuracao))
                return false;

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Canhotos.Canhoto repositorioCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = configuracao ?? repositorioConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = repositorioCanhoto.BuscarPorNF(xmlNotaFiscal.Codigo);
            bool inserir = false;

            if (canhoto == null)
            {
                canhoto = new Dominio.Entidades.Embarcador.Canhotos.Canhoto();

                canhoto.DataEnvioCanhoto = DateTime.Now;
                canhoto.DataUltimaModificacao = DateTime.Now;
                canhoto.GuidNomeArquivo = "";
                canhoto.NomeArquivo = "";
                canhoto.Observacao = "";
                canhoto.TipoCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.NFe;
                canhoto.SituacaoDigitalizacaoCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.PendenteDigitalizacao;
                canhoto.SituacaoCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Pendente;
                canhoto.DataEmissao = xmlNotaFiscal.DataEmissao;

                if (!string.IsNullOrWhiteSpace(numeroDocumentoOriginario))
                    canhoto.NumeroDocumentoOriginario = numeroDocumentoOriginario;

                if (cargaPedido != null)
                {
                    if (canhoto.EmpresaFilialEmissora == null)
                        canhoto.EmpresaFilialEmissora = cargaPedido.CargaOrigem.EmpresaFilialEmissora;

                    canhoto.Empresa = cargaPedido.CargaOrigem.Empresa;
                }

                inserir = true;
            }
            else
            {
                if (configuracaoEmbarcador.RetornarCanhotoParaPendenteAoReceberUmaNotaJaDigitalizada || ((cargaPedido?.ReentregaSolicitada ?? false) && canhoto.SituacaoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Cancelado))
                {
                    canhoto.SituacaoCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Pendente;
                    canhoto.SituacaoDigitalizacaoCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.PendenteDigitalizacao;

                    repositorioCanhoto.Atualizar(canhoto);
                }

                if (cargaPedido != null &&
                    (canhoto.SituacaoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Pendente ||
                        cancelamento ||
                        (canhoto.Carga != null && (canhoto.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada || canhoto.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada))))
                {
                    if (canhoto.Carga != null)
                    {
                        if (canhoto.CargaPedido != null && cargaPedido.Codigo != canhoto.CargaPedido.Codigo && canhoto.Pedido.Codigo == cargaPedido.Pedido.Codigo)
                        {
                            //não faz a troca pois o canhoto em teoria já está no último trecho.
                            if (!cancelamento && canhoto.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && canhoto.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada
                                && canhoto.CargaPedido.Expedidor != null && (cargaPedido.Expedidor?.CPF_CNPJ != canhoto.CargaPedido.Expedidor?.CPF_CNPJ) && canhoto.CargaPedido.Recebedor == null && cargaPedido.Recebedor != null)
                                return false;
                        }

                        //nao faz a troca pois o canhoto ja existe na carga mae
                        if (canhoto.Carga.TipoOperacao?.TipoConsolidacao == EnumTipoConsolidacao.AutorizacaoEmissao)
                            return false;

                        if (cargaPedido.Carga.Codigo != canhoto.Carga.Codigo)
                        {
                            GerarHistoricoCanhoto(canhoto, null, "Nota Fiscal transferida da carga " + canhoto.Carga.CodigoCargaEmbarcador + " para a carga " + cargaPedido.Carga.CodigoCargaEmbarcador + ".", unitOfWork);
                            Servicos.Log.TratarErro($"1 - Atualizando Canhoto {canhoto.Codigo}: Carga {canhoto.Carga.Codigo} para a Carga {cargaPedido.Carga.Codigo}", "AtualizacaoCanhoto");

                            canhoto.Empresa = cargaPedido.Carga.Empresa;

                            if (canhoto.EmpresaFilialEmissora == null)
                                canhoto.EmpresaFilialEmissora = cargaPedido.CargaOrigem.EmpresaFilialEmissora;
                        }
                    }
                    else
                        GerarHistoricoCanhoto(canhoto, null, "Canhoto adicionado e aguardando o seu envio.", unitOfWork);
                }
                else
                    GerarHistoricoCanhoto(canhoto, null, "Canhoto adicionado e aguardando o seu envio.", unitOfWork);
            }

            Servicos.Log.TratarErro($"2 - Atualizando Canhoto {canhoto.Codigo}: Situacao Canhoto {canhoto.SituacaoCanhoto}", "AtualizacaoCanhoto");

            if (canhoto.SituacaoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Pendente ||
                cancelamento ||
                (canhoto.Carga != null && (canhoto.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada || canhoto.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada)))
            {
                //repositorioXMLNotaFiscal.Atualizar(xmlNotaFiscal);

                canhoto.XMLNotaFiscal = xmlNotaFiscal;
                canhoto.Emitente = xmlNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida ? xmlNotaFiscal.Emitente : xmlNotaFiscal.Destinatario;
                canhoto.Destinatario = xmlNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida ? xmlNotaFiscal.Destinatario : xmlNotaFiscal.Emitente;
                canhoto.Filial = cargaPedido?.Carga?.Filial ?? xmlNotaFiscal.Filial;
                canhoto.Numero = xmlNotaFiscal.Numero;
                canhoto.Serie = xmlNotaFiscal.Serie;
                canhoto.Peso = xmlNotaFiscal.Peso;
                canhoto.Valor = xmlNotaFiscal.Valor;
                canhoto.ModalidadeFrete = xmlNotaFiscal.ModalidadeFrete;

                if (canhoto.Empresa == null)
                    canhoto.Empresa = cargaPedido?.Carga?.Empresa ?? xmlNotaFiscal.Empresa;

                if (canhoto.EmpresaFilialEmissora == null)
                    canhoto.EmpresaFilialEmissora = cargaPedido?.CargaOrigem?.EmpresaFilialEmissora;

                Servicos.Log.TratarErro($"3 - Atualizando Canhoto {canhoto.Codigo}: Codigo Carga {canhoto.Carga?.Codigo ?? 0} - Codigo Carga Pedido {cargaPedido?.Codigo}", "AtualizacaoCanhoto");

                if (cargaPedido != null)
                {
                    canhoto.Carga = cargaPedido.Carga;
                    canhoto.Pedido = cargaPedido.Pedido;
                    canhoto.CargaPedido = cargaPedido;
                }

                Servicos.Log.TratarErro($"4 - Atualizando Canhoto {canhoto.Codigo}: Codigo Carga {canhoto.Carga?.Codigo ?? 0} - Codigo Carga Pedido {cargaPedido?.Codigo}", "AtualizacaoCanhoto");

                if (canhoto.MotoristasResponsaveis != null)
                    canhoto.MotoristasResponsaveis.Clear();

                canhoto.MotoristasResponsaveis = motoristasResponsaveis;
                canhoto.TerceiroResponsavel = terceiroResponsavel;

                if (inserir)
                {
                    CanhotoEsperandoVinculo servicoCanhotoEsperandoVinculo = new CanhotoEsperandoVinculo(unitOfWork, configuracaoEmbarcador);

                    repositorioCanhoto.Inserir(canhoto);
                    GerarHistoricoCanhoto(canhoto, null, "Nota fiscal recebida e histórico do canhoto iniciado.", unitOfWork);
                    servicoCanhotoEsperandoVinculo.Vincular(canhoto, tipoServicoMultisoftware, null);
                }
                else
                    repositorioCanhoto.Atualizar(canhoto);
            }

            return true;
        }

        public async Task<bool> SalvarCanhotoNotaLoteAsync(List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> xmlNotaFiscals, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Cliente terceiroResponsavel, List<Dominio.Entidades.Usuario> motoristasResponsaveis, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto, List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos, List<Dominio.ObjetosDeValor.Embarcador.Carga.TotalizadorModeloDocumento> notasPedidos, string numeroDocumentoOriginario = "", bool cancelamento = false)
        {
            Repositorio.Embarcador.Canhotos.Canhoto repositorioCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
            Repositorio.Embarcador.Canhotos.CanhotoHistorico repCanhotoHistorico = new Repositorio.Embarcador.Canhotos.CanhotoHistorico(unitOfWork);
            Repositorio.Embarcador.Canhotos.CanhotoEsperandoVinculo repositorioCanhotoEsperandoVinculo = new Repositorio.Embarcador.Canhotos.CanhotoEsperandoVinculo(unitOfWork);

            var salvarCanhotos = xmlNotaFiscals.Select(async xmlNotaFiscal =>
            {
                int primeiroPedido = (from obj in notasPedidos where obj.Total == xmlNotaFiscal.Codigo select obj.Codigo).FirstOrDefault();
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = (from obj in cargaPedidos where obj.Pedido.Codigo == primeiroPedido select obj).FirstOrDefault();

                if (cargaPedido != null)
                {
                    return await GerarCanhotosNotasAsync(
                        xmlNotaFiscal,
                        cargaPedido,
                        terceiroResponsavel,
                        motoristasResponsaveis,
                        tipoServicoMultisoftware,
                        configuracao,
                        unitOfWork,
                        configuracaoCanhoto,
                        canhotos.Find(x => x.Codigo == xmlNotaFiscal.Codigo)
                    );
                }

                return (false, null, string.Empty);
            });

            (bool inserir, Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, string mensagem)[] retorno = await Task.WhenAll(salvarCanhotos);

            retorno = retorno.Where(x => x.canhoto != null).ToArray();

            await repositorioCanhoto.InserirAsync(retorno.Where(x => x.inserir).Select(x => x.canhoto).ToList(), "T_CANHOTO_NOTA_FISCAL");

            CanhotoEsperandoVinculo servicoCanhotoEsperandoVinculo = new CanhotoEsperandoVinculo(unitOfWork, configuracao);
            List<Dominio.Entidades.Embarcador.Canhotos.CanhotoHistorico> canhotoHistoricosParaInserir = new List<Dominio.Entidades.Embarcador.Canhotos.CanhotoHistorico>();

            bool vincularPendente = await repositorioCanhotoEsperandoVinculo.ExistePendenteAsync();

            foreach (var item in retorno)
            {
                if (!item.inserir)
                    await repositorioCanhoto.AtualizarAsync(item.canhoto);

                if (item.inserir && vincularPendente)
                    await servicoCanhotoEsperandoVinculo.VincularAsync(item.canhoto, tipoServicoMultisoftware, null);

                if (!string.IsNullOrEmpty(item.mensagem))
                    canhotoHistoricosParaInserir.Add(GerarHistoricoCanhotoLote(item.canhoto, null, item.mensagem));
            }

            repCanhotoHistorico.Inserir(canhotoHistoricosParaInserir, "T_CANHOTO_NOTA_FISCAL_HISTORICO");

            return true;
        }

        private async Task<(bool inserir, Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, string mensagem)> GerarCanhotosNotasAsync(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Cliente terceiroResponsavel, List<Dominio.Entidades.Usuario> motoristasResponsaveis, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto, Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, string numeroDocumentoOriginario = "", bool cancelamento = false)
        {
            if (cargaPedido?.Carga?.TipoOperacao?.NaoGerarCanhoto ?? false)
                return (false, null, string.Empty);

            if ((cargaPedido?.Carga?.CargaSVM ?? false) || (cargaPedido?.Carga?.CargaTakeOrPay ?? false))
                return (false, null, string.Empty);

            if (xmlNotaFiscal.TipoDocumento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.NFe && configuracao.Pais != TipoPais.Exterior)
                return (false, null, string.Empty);

            if (!DeveGerarCanhotoNotaFiscal(xmlNotaFiscal, cargaPedido, tipoServicoMultisoftware, unitOfWork, configuracaoCanhoto, configuracao))
                return (false, null, string.Empty);

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = configuracao ?? await repositorioConfiguracaoTMS.BuscarConfiguracaoPadraoAsync();
            bool inserir = false;
            string mensagem = string.Empty;

            if (canhoto == null)
            {
                canhoto = new Dominio.Entidades.Embarcador.Canhotos.Canhoto();

                canhoto.DataEnvioCanhoto = DateTime.Now;
                canhoto.DataUltimaModificacao = DateTime.Now;
                canhoto.GuidNomeArquivo = "";
                canhoto.NomeArquivo = "";
                canhoto.Observacao = "";
                canhoto.TipoCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.NFe;
                canhoto.SituacaoDigitalizacaoCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.PendenteDigitalizacao;
                canhoto.SituacaoCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Pendente;
                canhoto.DataEmissao = xmlNotaFiscal.DataEmissao;

                if (!string.IsNullOrWhiteSpace(numeroDocumentoOriginario))
                    canhoto.NumeroDocumentoOriginario = numeroDocumentoOriginario;

                if (cargaPedido != null)
                {
                    if (canhoto.EmpresaFilialEmissora == null)
                        canhoto.EmpresaFilialEmissora = cargaPedido.CargaOrigem.EmpresaFilialEmissora;

                    canhoto.Empresa = cargaPedido.CargaOrigem.Empresa;
                }

                inserir = true;
            }
            else
            {
                if (configuracaoEmbarcador.RetornarCanhotoParaPendenteAoReceberUmaNotaJaDigitalizada || ((cargaPedido?.ReentregaSolicitada ?? false) && canhoto.SituacaoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Cancelado))
                {
                    canhoto.SituacaoCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Pendente;
                    canhoto.SituacaoDigitalizacaoCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.PendenteDigitalizacao;
                }

                if (cargaPedido != null &&
                    (canhoto.SituacaoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Pendente ||
                        cancelamento ||
                        (canhoto.Carga != null && (canhoto.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada || canhoto.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada))))
                {
                    if (canhoto.Carga != null)
                    {
                        if (canhoto.CargaPedido != null && cargaPedido.Codigo != canhoto.CargaPedido.Codigo && canhoto.Pedido.Codigo == cargaPedido.Pedido.Codigo)
                        {
                            //não faz a troca pois o canhoto em teoria já está no último trecho.
                            if (!cancelamento && canhoto.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && canhoto.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada
                                && canhoto.CargaPedido.Expedidor != null && (cargaPedido.Expedidor?.CPF_CNPJ != canhoto.CargaPedido.Expedidor?.CPF_CNPJ) && canhoto.CargaPedido.Recebedor == null && cargaPedido.Recebedor != null)
                                return (false, null, string.Empty);
                        }

                        //nao faz a troca pois o canhoto ja existe na carga mae
                        if (canhoto.Carga.TipoOperacao?.TipoConsolidacao == EnumTipoConsolidacao.AutorizacaoEmissao)
                            return (false, null, string.Empty);

                        if (cargaPedido.Carga.Codigo != canhoto.Carga.Codigo)
                        {
                            mensagem = "Nota Fiscal transferida da carga ";
                            Servicos.Log.GravarInfo($"1 - Atualizando Canhoto {canhoto.Codigo}: Carga {canhoto.Carga.Codigo} para a Carga {cargaPedido.Carga.Codigo}", "AtualizacaoCanhoto");

                            canhoto.Empresa = cargaPedido.Carga.Empresa;

                            if (canhoto.EmpresaFilialEmissora == null)
                                canhoto.EmpresaFilialEmissora = cargaPedido.CargaOrigem.EmpresaFilialEmissora;
                        }
                    }
                    else
                        mensagem = "Canhoto adicionado e aguardando o seu envio.";
                }
                else
                    mensagem = "Canhoto adicionado e aguardando o seu envio.";
            }

            if (canhoto.SituacaoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Pendente ||
                cancelamento ||
                (canhoto.Carga != null && (canhoto.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada || canhoto.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada)))
            {
                canhoto.XMLNotaFiscal = xmlNotaFiscal;
                canhoto.Emitente = xmlNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida ? xmlNotaFiscal.Emitente : xmlNotaFiscal.Destinatario;
                canhoto.Destinatario = xmlNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida ? xmlNotaFiscal.Destinatario : xmlNotaFiscal.Emitente;
                canhoto.Filial = cargaPedido?.Carga?.Filial ?? xmlNotaFiscal.Filial;
                canhoto.Numero = xmlNotaFiscal.Numero;
                canhoto.Serie = xmlNotaFiscal.Serie;
                canhoto.Peso = xmlNotaFiscal.Peso;
                canhoto.Valor = xmlNotaFiscal.Valor;
                canhoto.ModalidadeFrete = xmlNotaFiscal.ModalidadeFrete;

                canhoto.Empresa ??= cargaPedido?.Carga?.Empresa ?? xmlNotaFiscal.Empresa;

                canhoto.EmpresaFilialEmissora ??= cargaPedido?.CargaOrigem?.EmpresaFilialEmissora;

                Servicos.Log.GravarInfo($"2 - Atualizando Canhoto {canhoto.Codigo}: Codigo Carga {canhoto.Carga?.Codigo ?? 0} - Codigo Carga Pedido {cargaPedido?.Codigo}", "AtualizacaoCanhoto");

                if (cargaPedido != null)
                {
                    canhoto.Carga = cargaPedido.Carga;
                    canhoto.Pedido = cargaPedido.Pedido;
                    canhoto.CargaPedido = cargaPedido;
                }

                canhoto.MotoristasResponsaveis?.Clear();
                canhoto.MotoristasResponsaveis = motoristasResponsaveis;
                canhoto.TerceiroResponsavel = terceiroResponsavel;

                if (inserir)
                    return (true, canhoto, "Nota fiscal recebida e histórico do canhoto iniciado.");

                return (false, canhoto, mensagem);
            }
            return (false, null, string.Empty);
        }

        public void SalvarCanhotoCTe(Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Cliente terceiroResponsavel, List<Dominio.Entidades.Usuario> motoristasResponsaveis, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            if (!cteTerceiro.Ativo)
                return;

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                return;

            if (cargaPedido?.Carga?.TipoOperacao?.NaoGerarCanhoto ?? false)
                return;

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            bool gerarCanhoto = repConfiguracaoTMS.GerarCanhotoSempre();

            if (!gerarCanhoto)
            {
                Dominio.Entidades.Cliente tomador = cargaPedido != null ? cargaPedido.ObterTomador() : null;

                gerarCanhoto = ((cargaPedido != null && cargaPedido.Recebedor == null && cargaPedido.CargaPedidoProximoTrecho == null && cargaPedido.Carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Cliente) || cargaPedido == null) && tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador || ((tomador != null && (tomador.ExigeCanhotoFisico.HasValue && tomador.ExigeCanhotoFisico.Value) || (tomador != null && tomador.GrupoPessoas != null && (tomador.GrupoPessoas.ExigeCanhotoFisico.HasValue && tomador.GrupoPessoas.ExigeCanhotoFisico.Value))) || tomador == null);
            }

            if (gerarCanhoto)
            {
                Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                Repositorio.Embarcador.CTe.CTeTerceiroQuantidade repCTeTerceiroQuantidade = new Repositorio.Embarcador.CTe.CTeTerceiroQuantidade(unitOfWork);

                bool inserir = false;
                Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = repCanhoto.BuscarPorCTe(cteTerceiro.Codigo);

                if (canhoto == null)
                {
                    canhoto = new Dominio.Entidades.Embarcador.Canhotos.Canhoto();
                    canhoto.DataEnvioCanhoto = DateTime.Now;
                    canhoto.GuidNomeArquivo = "";
                    canhoto.NomeArquivo = "";
                    canhoto.Observacao = "";
                    canhoto.DataUltimaModificacao = DateTime.Now;
                    canhoto.TipoCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.CTeSubcontratacao;
                    canhoto.SituacaoDigitalizacaoCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.PendenteDigitalizacao;
                    canhoto.DataEmissao = cteTerceiro.DataEmissao;
                    if (cargaPedido != null)
                    {
                        canhoto.Empresa = cargaPedido.CargaOrigem.Empresa;
                        if (canhoto.EmpresaFilialEmissora == null)
                            canhoto.EmpresaFilialEmissora = cargaPedido.CargaOrigem.EmpresaFilialEmissora;
                    }
                    inserir = true;
                }
                else
                {
                    if (cargaPedido != null)
                    {
                        if (canhoto.Carga != null)
                        {
                            if (cargaPedido.Carga.Codigo != canhoto.Carga.Codigo)
                            {
                                GerarHistoricoCanhoto(canhoto, null, "CT-e de subcontratação transferido da carga " + canhoto.Carga.CodigoCargaEmbarcador + " para a carga " + cargaPedido.Carga.CodigoCargaEmbarcador + ".", unitOfWork);
                                canhoto.Empresa = cargaPedido.CargaOrigem.Empresa;
                                if (canhoto.EmpresaFilialEmissora == null)
                                    canhoto.EmpresaFilialEmissora = cargaPedido.CargaOrigem.EmpresaFilialEmissora;
                            }
                        }
                        else
                        {
                            GerarHistoricoCanhoto(canhoto, null, "Canhoto adicionado e aguardando o seu envio.", unitOfWork);
                        }
                    }
                    else
                    {
                        GerarHistoricoCanhoto(canhoto, null, "Canhoto adicionado e aguardando o seu envio.", unitOfWork);
                    }
                }

                canhoto.CTeSubcontratacao = cteTerceiro;

                canhoto.Emitente = cteTerceiro.Emitente.Cliente;
                canhoto.Destinatario = cteTerceiro.Destinatario.Cliente;

                canhoto.Filial = cargaPedido.Carga.Filial;

                canhoto.Numero = cteTerceiro.Numero;
                canhoto.Serie = cteTerceiro.Serie;
                canhoto.Peso = repCTeTerceiroQuantidade.BuscarPesoPorCTeParaSubContratacao(cteTerceiro.Codigo);
                canhoto.Valor = cteTerceiro.ValorAReceber;

                if (cteTerceiro.TipoPagamento == Dominio.Enumeradores.TipoPagamento.A_Pagar)
                    canhoto.ModalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.A_Pagar;
                else if (cteTerceiro.TipoPagamento == Dominio.Enumeradores.TipoPagamento.Outros)
                    canhoto.ModalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.Outros;
                else if (cteTerceiro.TipoPagamento == Dominio.Enumeradores.TipoPagamento.Pago)
                    canhoto.ModalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.Pago;

                if (cargaPedido != null)
                {
                    canhoto.Carga = cargaPedido.Carga;
                    canhoto.Pedido = cargaPedido.Pedido;
                }

                if (canhoto.MotoristasResponsaveis != null)
                    canhoto.MotoristasResponsaveis.Clear();

                canhoto.MotoristasResponsaveis = motoristasResponsaveis;
                canhoto.TerceiroResponsavel = terceiroResponsavel;

                canhoto.SituacaoCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Pendente;

                if (inserir)
                {
                    repCanhoto.Inserir(canhoto);
                    GerarHistoricoCanhoto(canhoto, null, "CT-e de subcontratação recebido e histórico do canhoto iniciado.", unitOfWork);
                }
                else
                {
                    repCanhoto.Atualizar(canhoto);
                }
            }
        }

        public void SalvarCanhotoCargaCTe(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            if (cargaCTe.Carga?.TipoOperacao?.NaoGerarCanhoto ?? false)
                return;

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            bool gerarCanhoto = repConfiguracaoTMS.GerarCanhotoSempre();

            if (cargaCTe.CTe == null ||
                cargaCTe.CTe.TomadorPagador?.Cliente == null ||
                cargaCTe.CTe.Remetente?.Cliente == null ||
                cargaCTe.CTe.Destinatario?.Cliente == null)
                return;

            if (cargaCTe.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Complemento)
                return;

            if (!gerarCanhoto)
            {
                if (cargaCTe.Carga?.TipoOperacao?.UsarConfiguracaoFaturaPorTipoOperacao ?? false)
                    gerarCanhoto = cargaCTe.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.ArmazenaCanhotoFisicoCTe ?? false;
                else if (cargaCTe.CTe.TomadorPagador.Cliente.NaoUsarConfiguracaoFaturaGrupo)
                    gerarCanhoto = cargaCTe.CTe.TomadorPagador.Cliente.ArmazenaCanhotoFisicoCTe ?? false;
                else if (cargaCTe.CTe.TomadorPagador.Cliente.GrupoPessoas != null)
                    gerarCanhoto = cargaCTe.CTe.TomadorPagador.Cliente.GrupoPessoas.ArmazenaCanhotoFisicoCTe ?? false;

                if (!gerarCanhoto)
                    return;
            }

            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);

            bool inserir = false;

            Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = repCanhoto.BuscarPorCargaCTe(cargaCTe.Codigo);

            if (canhoto == null)
            {
                canhoto = new Dominio.Entidades.Embarcador.Canhotos.Canhoto();
                canhoto.DataEnvioCanhoto = DateTime.Now;
                canhoto.GuidNomeArquivo = "";
                canhoto.NomeArquivo = "";
                canhoto.Observacao = "";
                canhoto.DataUltimaModificacao = DateTime.Now;
                canhoto.TipoCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.CTe;
                canhoto.SituacaoDigitalizacaoCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.PendenteDigitalizacao;
                canhoto.DataEmissao = cargaCTe.CTe.DataEmissao.Value;
                canhoto.Empresa = cargaCTe.CTe.Empresa;

                inserir = true;
            }
            else
            {
                GerarHistoricoCanhoto(canhoto, null, "Canhoto adicionado e aguardando o seu envio.", unitOfWork);
            }

            canhoto.CargaCTe = cargaCTe;

            canhoto.Emitente = cargaCTe.CTe.Remetente.Cliente;
            canhoto.Destinatario = cargaCTe.CTe.Destinatario.Cliente;
            canhoto.Numero = cargaCTe.CTe.Numero;
            canhoto.Serie = cargaCTe.CTe.Serie?.Numero.ToString() ?? "";
            canhoto.Peso = cargaCTe.CTe.QuantidadesCarga.Where(o => o.UnidadeMedida == "01").Sum(o => o.Quantidade);
            canhoto.Valor = cargaCTe.CTe.ValorAReceber;

            if (cargaCTe.CTe.TipoPagamento == Dominio.Enumeradores.TipoPagamento.A_Pagar)
                canhoto.ModalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.A_Pagar;
            else if (cargaCTe.CTe.TipoPagamento == Dominio.Enumeradores.TipoPagamento.Outros)
                canhoto.ModalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.Outros;
            else if (cargaCTe.CTe.TipoPagamento == Dominio.Enumeradores.TipoPagamento.Pago)
                canhoto.ModalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.Pago;

            canhoto.Carga = cargaCTe.Carga;

            if (canhoto.MotoristasResponsaveis != null)
                canhoto.MotoristasResponsaveis.Clear();
            else
                canhoto.MotoristasResponsaveis = new List<Dominio.Entidades.Usuario>();

            foreach (Dominio.Entidades.Usuario motorista in cargaCTe.Carga.Motoristas)
                canhoto.MotoristasResponsaveis.Add(motorista);

            canhoto.TerceiroResponsavel = cargaCTe.Carga.Terceiro;
            canhoto.SituacaoCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Pendente;

            if (inserir)
            {
                repCanhoto.Inserir(canhoto);

                GerarHistoricoCanhoto(canhoto, null, "Histórico do canhoto iniciado.", unitOfWork);
            }
            else
            {
                repCanhoto.Atualizar(canhoto);
            }
        }

        public static bool RemoverCanhotoLocalArmazenamento(out string mensagem, Dominio.Entidades.Usuario usuario, Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, UnitOfWork unitOfWork)
        {
            if (canhoto == null)
            {
                mensagem = "Canhoto não localizado.";
                return false;
            }

            if (canhoto.SituacaoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Pendente)
            {
                mensagem = "A situação do canhoto não permite que o mesmo seja removido do local de armazenamento.";
                return false;
            }

            if (canhoto.LocalArmazenamentoCanhoto == null)
            {
                mensagem = "Canhoto não está armazenado para realizar a remoção.";
                return false;
            }

            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);

            Servicos.Embarcador.Canhotos.Canhoto svcCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);

            canhoto.ObservacaoRecebimentoFisico = string.Empty;
            canhoto.PacoteArmazenado = 0;
            canhoto.PosicaoNoPacote = 0;
            canhoto.LocalArmazenamentoCanhoto = null;
            canhoto.SituacaoCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Pendente;

            repCanhoto.Atualizar(canhoto);

            svcCanhoto.GerarHistoricoCanhoto(canhoto, usuario, "Canhoto removido do local de armazenamento.", unitOfWork);

            mensagem = string.Empty;
            return true;
        }

        public static bool ReverterBaixaCanhoto(out string mensagem, Dominio.Entidades.Usuario usuario, Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, UnitOfWork unitOfWork)
        {
            if (canhoto == null)
            {
                mensagem = "Canhoto não localizado.";
                return false;
            }

            if (canhoto.SituacaoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Pendente)
            {
                mensagem = "A situação do canhoto não permite que o mesmo seja removido do local de armazenamento.";
                return false;
            }

            if (canhoto.LocalArmazenamentoCanhoto != null)
            {
                mensagem = "Não é possível reverter a baixa de um canhoto armazenado. Remova o canhoto do armazenamento.";
                return false;
            }

            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);

            Servicos.Embarcador.Canhotos.Canhoto svcCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);

            canhoto.SituacaoCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Pendente;

            repCanhoto.Atualizar(canhoto);

            svcCanhoto.GerarHistoricoCanhoto(canhoto, usuario, "Realizada a reversão da baixa do canhoto.", unitOfWork);

            mensagem = string.Empty;
            return true;
        }

        public static void GerarCanhotosRetroativosCTe(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS)
        {
            Servicos.Embarcador.Canhotos.Canhoto svcCanhoto = new Canhoto(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaCTe.BuscarCTesSemCanhotoGerado(configuracaoTMS.GerarCanhotoSempre);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCTes)
            {
                unitOfWork.Start();

                svcCanhoto.SalvarCanhotoCargaCTe(cargaCTe, tipoServicoMultisoftware, unitOfWork);

                unitOfWork.CommitChanges();
            }
        }

        public static void GerarCanhotosRetroativosNFe(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS)
        {
            Servicos.Embarcador.Canhotos.Canhoto svcCanhoto = new Canhoto(unitOfWork);

            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

            List<int> pedidoXMLNotaFiscais = repPedidoXMLNotaFiscal.BuscarCodigosNFesSemCanhotoGerado(configuracaoTMS.GerarCanhotoSempre);

            Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto repConfiguracaoCanhoto = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto = repConfiguracaoCanhoto.BuscarConfiguracaoPadrao();

            foreach (int codigoPedidoXMLNotaFiscal in pedidoXMLNotaFiscais)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = repPedidoXMLNotaFiscal.BuscarPorCodigoComFetch(codigoPedidoXMLNotaFiscal);

                unitOfWork.Start();

                svcCanhoto.SalvarCanhotoNota(pedidoXMLNotaFiscal.XMLNotaFiscal, pedidoXMLNotaFiscal.CargaPedido, pedidoXMLNotaFiscal.CargaPedido.Carga.FreteDeTerceiro ? pedidoXMLNotaFiscal.CargaPedido.Carga.Terceiro : null, pedidoXMLNotaFiscal.CargaPedido.Carga.Motoristas.ToList(), tipoServicoMultisoftware, configuracaoTMS, unitOfWork, configuracaoCanhoto);

                unitOfWork.CommitChanges();
                unitOfWork.FlushAndClear();
            }
        }

        public static void GerarCanhotosRetroativosCTeSubcontratacao(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS)
        {
            Servicos.Embarcador.Canhotos.Canhoto svcCanhoto = new Canhoto(unitOfWork);

            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubcontratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);

            List<int> pedidoCTesParaSubcontratacao = repPedidoCTeParaSubcontratacao.BuscarCodigosCTesParaSubcontratacaoSemCanhotoGerado(configuracaoTMS.GerarCanhotoSempre);

            foreach (int codigoPedidoCTeParaSubcontratacao in pedidoCTesParaSubcontratacao)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao = repPedidoCTeParaSubcontratacao.BuscarPorCodigoComFetch(codigoPedidoCTeParaSubcontratacao);

                unitOfWork.Start();

                svcCanhoto.SalvarCanhotoCTe(pedidoCTeParaSubContratacao.CTeTerceiro, pedidoCTeParaSubContratacao.CargaPedido, pedidoCTeParaSubContratacao.CargaPedido.Carga.FreteDeTerceiro ? pedidoCTeParaSubContratacao.CargaPedido.Carga.Terceiro : null, pedidoCTeParaSubContratacao.CargaPedido.Carga.Motoristas.ToList(), tipoServicoMultisoftware, unitOfWork);

                unitOfWork.CommitChanges();
                unitOfWork.FlushAndClear();
            }
        }

        public static void FinalizarDigitalizacaoCanhoto(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware? tipoServicoMultisoftware)
        {
            new Canhoto(unitOfWork).FinalizarDigitalizacaoCanhotoAsync(canhoto, tipoServicoMultisoftware).GetAwaiter().GetResult();
        }

        public async Task FinalizarDigitalizacaoCanhotoAsync(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware? tipoServicoMultisoftware)
        {
            if (canhoto.SituacaoDigitalizacaoCanhoto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.Digitalizado || !tipoServicoMultisoftware.HasValue)
                return;

            Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto repositorioConfiguracaoCanhoto = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto(_unitOfWork, _cancellationToken);
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(_unitOfWork, _cancellationToken);
            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(_unitOfWork, _cancellationToken);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork, _cancellationToken);

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                if (canhoto.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.NFe)
                {
                    //se a mesma transportador faz o primeiro e o segundo trecho de uma nota por exemplo, ao digitalizar o canhoto da NF-e liberar o pagamento dos ctes dos dois trechos.
                    List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> documentosFaturamento = await repDocumentoFaturamento.BuscarPorDocumentosNotaFiscalETransportadorAsync(canhoto.XMLNotaFiscal?.Codigo ?? 0, canhoto.Empresa?.Codigo ?? 0);

                    if (documentosFaturamento.Count > 0)
                    {
                        List<int> codigosCTes = documentosFaturamento.Select(obj => obj.CTe.Codigo).ToList();

                        List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotosCTes = await repCanhoto.BuscarPorNotasDosCTesECanhotoAsync(codigosCTes, canhoto.Codigo);

                        foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento in documentosFaturamento)
                        {
                            bool canhotos = canhotosCTes.Any(obj => obj.XMLNotaFiscal.CTEs.Any(o => o.Codigo == documentoFaturamento.CTe.Codigo));

                            if (canhotos)
                                continue;

                            documentoFaturamento.CanhotosDigitalizados = true;
                            await repDocumentoFaturamento.AtualizarAsync(documentoFaturamento);
                        }
                    }

                    Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado
                    {
                        TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema,
                        OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema
                    };

                    Servicos.Embarcador.GestaoPallet.MovimentacaoPallet servicoMovimentacaoPallet = new Servicos.Embarcador.GestaoPallet.MovimentacaoPallet(_unitOfWork, auditado);
                    servicoMovimentacaoPallet.InformarMudancaResponsavel(canhoto);
                }
                else if (canhoto.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.Avulso)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscals = canhoto.CanhotoAvulso.PedidosXMLNotasFiscais.ToList();
                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXmlNotaFiscal in pedidoXMLNotaFiscals)
                    {
                        Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento = await repDocumentoFaturamento.BuscarPorNotaFiscalCargaAsync(pedidoXmlNotaFiscal.XMLNotaFiscal.Codigo, pedidoXmlNotaFiscal.CargaPedido.Carga?.Codigo ?? 0);
                        if (documentoFaturamento != null)
                        {
                            documentoFaturamento.CanhotosDigitalizados = true;
                            await repDocumentoFaturamento.AtualizarAsync(documentoFaturamento);
                        }
                    }
                }
            }
            else
            {
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto = await repositorioConfiguracaoCanhoto.BuscarConfiguracaoPadraoAsync();

                List<int> ctes;

                if (canhoto.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.NFe)
                {
                    ctes = await repXMLNotaFiscal.BuscarCodigosCTesPorCodigoAsync(canhoto.XMLNotaFiscal.Codigo);
                }
                else if (canhoto.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.CTeSubcontratacao)
                {
                    Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal repPedidoCTeParaSubContratacaoPedidoNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal(_unitOfWork);

                    ctes = await repPedidoCTeParaSubContratacaoPedidoNotaFiscal.BuscarCodigosCTesPorCTeTerceiroAsync(canhoto.CTeSubcontratacao.Codigo);
                }
                else if (canhoto.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.CTe)
                    ctes = new List<int>() { canhoto.CargaCTe.CTe.Codigo };
                else if (canhoto.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.Avulso)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscals = canhoto.CanhotoAvulso.PedidosXMLNotasFiscais.ToList();
                    List<int> codigosNotas = (from obj in pedidoXMLNotaFiscals select obj.XMLNotaFiscal.Codigo).Distinct().ToList();

                    ctes = await repXMLNotaFiscal.BuscarCodigosCTesPorCodigosAsync(codigosNotas);
                }
                else
                    return;

                foreach (int codigoCTe in ctes)
                {
                    if (configuracaoCanhoto.ValidarSituacaoDigitalizacaoCanhotosAoSumarizarDocumentoFaturamento)
                    {
                        if (await repCanhoto.VerificarSeExisteCanhotoCTePendenteDigitalizacaoPorCTeAsync(codigoCTe) ||
                            await repCanhoto.VerificarSeExisteCanhotoNotaFiscalPendenteDigitalizacaoPorCTeAsync(codigoCTe))
                            continue;
                    }
                    else
                    {
                        if (await repCanhoto.VerificarSeExisteCanhotoCTePendentePorCTeAsync(codigoCTe) ||
                            await repCanhoto.VerificarSeExisteCanhotoNotaFiscalPendentePorCTeAsync(codigoCTe))
                            continue;
                    }

                    List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> documentosFaturamento = await repDocumentoFaturamento.BuscarTodosPorCTeAsync(codigoCTe);

                    foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento in documentosFaturamento)
                    {
                        documentoFaturamento.CanhotosDigitalizados = true;

                        await repDocumentoFaturamento.AtualizarAsync(documentoFaturamento);
                    }
                }
            }
        }

        public MemoryStream ObterStremingPDFCanhoto(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, Repositorio.UnitOfWork unitOfWork)
        {
            if (!canhoto.IsPDF())
                return null;

            string caminho = CaminhoCanhoto(canhoto, unitOfWork);
            string nomeCompletoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, canhoto.GuidNomeArquivo + "." + canhoto.NomeArquivo.Split('.').LastOrDefault().ToLower());

            if (!Utilidades.IO.FileStorageService.Storage.Exists(nomeCompletoArquivo))
                return null;

            byte[] pdfArray = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(nomeCompletoArquivo);
            MemoryStream output = new MemoryStream();

            output.Write(pdfArray, 0, pdfArray.Length);
            output.Position = 0;

            return output;
        }

        public async Task GerarIntegracoesCteCanhoto(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, TipoRegistroIntegracaoCTeCanhoto tipoRegistro, List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracao)
        {
            foreach (Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao in tiposIntegracao)
            {
                await GerarRegistroIntegracao(cte, canhoto, tipoIntegracao, tipoRegistro);
            }
        }

        public async Task GerarRegistroIntegracao(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, TipoRegistroIntegracaoCTeCanhoto tipoRegistro)
        {
            Repositorio.Embarcador.CTe.CTeCanhotoIntegracao repositorioCTeIntegracao = new Repositorio.Embarcador.CTe.CTeCanhotoIntegracao(_unitOfWork);
            Dominio.Entidades.Embarcador.CTe.CTeCanhotoIntegracao cteIntegracao = new Dominio.Entidades.Embarcador.CTe.CTeCanhotoIntegracao();

            cteIntegracao.Initialize();

            cteIntegracao.CTe = cte;
            cteIntegracao.TipoRegistro = tipoRegistro;
            cteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
            cteIntegracao.DataIntegracao = DateTime.Now;
            cteIntegracao.NumeroTentativas = 0;
            cteIntegracao.ProblemaIntegracao = "";
            cteIntegracao.TipoIntegracao = tipoIntegracao;

            await repositorioCTeIntegracao.InserirAsync(cteIntegracao);

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado
            {
                TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema,
                OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema
            };

            string msgAuditoria = "Registro de integração gerado na " + (tipoRegistro == TipoRegistroIntegracaoCTeCanhoto.Imagem ? "inclusão" : "aprovação") + " da imagem do canhoto " + canhoto.Numero + " ";
            await Servicos.Auditoria.Auditoria.AuditarAsync(auditado, cteIntegracao, cteIntegracao.GetChanges(), msgAuditoria, _unitOfWork);
        }

        public async Task<bool> TodosCanhotosPorSituacaoAsync(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto> situacoesDigitalizacao, TipoRegistroIntegracaoCTeCanhoto tipoRegistro)
        {
            string chaveNotaFiscal = canhoto.XMLNotaFiscal?.Chave ?? "";

            if (string.IsNullOrEmpty(chaveNotaFiscal))
                return false;

            Repositorio.Embarcador.Canhotos.Canhoto repositorioCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(_unitOfWork, _cancellationToken);
            Repositorio.ConhecimentoDeTransporteEletronico repositorioCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork, _cancellationToken);

            bool ctesOk = true;

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTe = await repositorioCTe.BuscarListaPorChaveNFeAsync(Utilidades.String.OnlyNumbers(chaveNotaFiscal));

            if (listaCTe.Count == 0)
                return false;

            List<int> codigosCTes = listaCTe.Select(obj => obj.Codigo).ToList();

            List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotosDosCTes = await repositorioCanhoto.BuscarPorNotasDosCTesAsync(codigosCTes);

            List<TipoIntegracao> tiposIntegracaoGeracaoCteCanhoto = new List<TipoIntegracao>()
            {
                TipoIntegracao.Unilever
            };

            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork, _cancellationToken);

            List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracao = await repositorioTipoIntegracao.BuscarPorTiposAsync(tiposIntegracaoGeracaoCteCanhoto);

            foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in listaCTe)
            {
                List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotosCtePorNota = canhotosDosCTes.Where(obj => obj.XMLNotaFiscal.CTEs.Any(o => o.Codigo == cte.Codigo)).ToList();

                if (canhotosCtePorNota.Any(obj => !situacoesDigitalizacao.Contains(obj.SituacaoDigitalizacaoCanhoto)))
                    ctesOk = false;
                else
                    await GerarIntegracoesCteCanhoto(cte, canhoto, tipoRegistro, tiposIntegracao);
            }

            if (!ctesOk)
                return false;

            return true;
        }

        #endregion

        #region Métodos Privados

        private bool DeveGerarCanhotoNotaFiscal(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {

            if (!string.IsNullOrEmpty(xmlNotaFiscal.NumeroControlePedido)) // Se tem NumeroControlePedido preenchido, não pode gerar
            {
                return false;
            }

            if (xmlNotaFiscal.TipoNotaFiscalIntegrada == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscalIntegrada.RemessaPallet && !configuracaoCanhoto.GerarCanhotoParaNotasTipoPallet)
                return false;

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            bool gerarCanhoto = configuracao?.GerarCanhotoSempre ?? repConfiguracaoTMS.GerarCanhotoSempre();

            if (!gerarCanhoto)
            {
                Dominio.Entidades.Cliente tomador = cargaPedido != null ? cargaPedido.ObterTomador() : null;

                gerarCanhoto = (
                        cargaPedido == null ||
                        (
                            ((cargaPedido.Carga.TipoOperacao?.ConfiguracaoCanhoto?.NaoGerarCanhotoAvulsoEmCargasComAoMenosUmRecebedor ?? false) || cargaPedido.Recebedor == null || (cargaPedido.Carga.Rota?.FinalizarViagemAutomaticamente ?? false)) &&
                            cargaPedido.CargaPedidoProximoTrecho == null &&
                            (cargaPedido.Carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Cliente || gerarCanhoto)
                        )
                    ) &&
                    tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador ||
                    (
                        tomador == null ||
                        (cargaPedido.Carga.TipoOperacao?.UsarConfiguracaoFaturaPorTipoOperacao ?? false &&
                            cargaPedido.Carga.TipoOperacao?.ConfiguracaoTipoOperacaoFatura?.ExigeCanhotoFisico.HasValue == true &&
                            cargaPedido.Carga.TipoOperacao.ConfiguracaoTipoOperacaoFatura.ExigeCanhotoFisico.Value
                        ) ||
                        (
                            tomador.NaoUsarConfiguracaoFaturaGrupo &&
                            tomador.ExigeCanhotoFisico.HasValue &&
                            tomador.ExigeCanhotoFisico.Value
                        ) ||
                        (
                            tomador.GrupoPessoas != null &&
                            tomador.GrupoPessoas.ExigeCanhotoFisico.HasValue &&
                            tomador.GrupoPessoas.ExigeCanhotoFisico.Value
                        )
                    );
            }

            return gerarCanhoto;
        }

        private string ObterMiniatura(string nomeArquivo)
        {
            if (Path.GetExtension(nomeArquivo).ToLower() == ".tif" || Path.GetExtension(nomeArquivo).ToLower() == ".jpg")
            {
                if (!Utilidades.IO.FileStorageService.Storage.Exists(nomeArquivo) || GetFileSize(nomeArquivo) == 0)
                    return null;

                using (System.Drawing.Image image = System.Drawing.Image.FromStream(Utilidades.IO.FileStorageService.Storage.OpenRead(nomeArquivo)))
                {
                    if (System.Drawing.Imaging.ImageFormat.Tiff.Equals(image.RawFormat))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                            nomeArquivo = Convert.ToBase64String(ms.ToArray());
                        }
                    }
                }
            }

            if (!Utilidades.IO.FileStorageService.Storage.Exists(nomeArquivo))
                return null;

            using (Image imgPhoto = Image.FromStream(Utilidades.IO.FileStorageService.Storage.OpenRead(nomeArquivo)))
            using (Bitmap newImage = ResizeImage(imgPhoto, 600))
            using (MemoryStream ms = new MemoryStream())
            {
                newImage.Save(ms, imgPhoto.RawFormat);

                return Convert.ToBase64String(ms.ToArray());
            }
        }

        static long GetFileSize(string FilePath)
        {
            if (Utilidades.IO.FileStorageService.Storage.Exists(FilePath))
            {
                return Utilidades.IO.FileStorageService.Storage.GetFileInfo(FilePath).Size;
            }
            return 0;
        }

        private string ObterBodyBaseEmailCanhotosPendentes(bool MensagemRodapeEmailCanhotosPendentes)
        {
            string html = "<span style=\"width: 100%; display: inline-block\">Prezado(a) Sr(a) {{nome-transportador}}, ";
            html += "</span><span style=\"width: 100%; display: inline-block\">Até o momento, você possui {{numero-canhotos-pendentes}} pendências no Multiembarcador, conforme relatório abaixo:</span>";
            html += "<table style=\"margin: 30px 0 30px 0; border: 1px solid #b9b5b5; border-collapse: collapse; border-collapse: collapse;\">";
            html += "<thead style=\"background-color: #d9e1f2; color: black;\">";
            html += "<tr>";
            html += "<th style=\"border: 1px solid #b9b5b5; padding: 10px;\"> Digitalização </th>";
            html += "<th style=\"border: 1px solid #b9b5b5; padding: 10px;\"> Data Prazo</th>";
            html += "<th style=\"border: 1px solid #b9b5b5; padding: 10px;\">Número</th>";
            html += "<th style=\"border: 1px solid #b9b5b5; padding: 10px;\">Tipo</th>";
            html += "<th style=\"border: 1px solid #b9b5b5; padding: 10px;\">Data Emissão</th> ";
            html += "<th style=\"border: 1px solid #b9b5b5; padding: 10px;\">Número Carga</th> ";
            html += "<th style=\"border: 1px solid #b9b5b5; padding: 10px;\">Transportador</th>";
            html += "<th style=\"border: 1px solid #b9b5b5; padding: 10px;\">Tipo de Carga</th>";
            html += "<th style=\"border: 1px solid #b9b5b5; padding: 10px;\">Destinatário</th>";
            html += "<th style=\"border: 1px solid #b9b5b5; padding: 10px;\">Filial</th>";
            html += "<th style=\"border: 1px solid #b9b5b5; padding: 10px;\">CT-e</th>";
            html += "<th style=\"border: 1px solid #b9b5b5; padding: 10px;\">Placa Veículo Resp. Entrega</th>";
            html += "</tr>";
            html += "</thead>";
            html += "<tbody>";
            html += "{{rows-tabela}}";
            html += "</tbody>";
            html += "</table>";
            html += "<span style=\"width: 100%; display: inline-block;\">Por favor, entre no sistema para realizar as tratativas.</span>";
            if (MensagemRodapeEmailCanhotosPendentes)
            {
                html += "<div style=\"margin: 30px 0 30px 0; background-color: #f7caac; padding: 8px;\">";
                html += "<span style=\"font-weight: bold; width: 100%; display: inline-block;\">Atenção: o prazo para pagamento é contado a partir da data de envio do canhoto.</span>";
                html += "</div>";
            }
            html += "<small>Esse e-mail é gerado automaticamente. Não responda.</small>";

            return html;
        }

        private Bitmap ResizeImage(Image image, int newWidth)
        {
            int newHeight = (image.Height * newWidth) / image.Width;
            Rectangle destRect = new Rectangle(0, 0, newWidth, newHeight);
            Bitmap destImage = new Bitmap(newWidth, newHeight);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (Graphics graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (ImageAttributes wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }



        #endregion
    }
}
