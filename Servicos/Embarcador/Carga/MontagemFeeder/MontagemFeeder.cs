using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Servicos.Embarcador.Carga.MontagemFeeder
{
    public class MontagemFeeder : ServicoBase
    {
        #region Construtores
        
        public MontagemFeeder() : base() { }

        #endregion

        #region Métodos Publicos

        public Dominio.ObjetosDeValor.Embarcador.Carga.MontagemFeeder.RetornoImportacaoMontagemFeeder ImportarMontagemFeeder(Dominio.Entidades.Embarcador.Cargas.MontagemFeeder.MontagemFeeder montagemFeeder, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string AdminStringConexao, Repositorio.UnitOfWork unitOfWork, string urlAcessoCliente)
        {
            Dominio.ObjetosDeValor.Embarcador.Carga.MontagemFeeder.RetornoImportacaoMontagemFeeder retorno = new Dominio.ObjetosDeValor.Embarcador.Carga.MontagemFeeder.RetornoImportacaoMontagemFeeder();

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado
            {
                TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Usuario,
                Usuario = usuario,
                Empresa = usuario?.Empresa,
                Texto = ""
            };

            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePay repImportacaoTakePay = new Repositorio.Embarcador.Pedidos.ImportacaoTakePay.ImportacaoTakePay(unitOfWork);
            Repositorio.Embarcador.Operacional.OperadorLogistica repOperadorLogistica = new Repositorio.Embarcador.Operacional.OperadorLogistica(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = usuario != null ? repOperadorLogistica.BuscarPorUsuario(usuario.Codigo) : null;
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            int contador = 0;
            string retornoFinaliza = "";
            StringBuilder erros = new StringBuilder();
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("pt-BR");
            try
            {
                if (!Utilidades.IO.FileStorageService.Storage.Exists(montagemFeeder.CaminhoPlanilha))
                {
                    retorno.TotalPedidos = 0;
                    retorno.TotalCargas = 0;
                    retorno.Sucesso = false;
                    retorno.Mensagem = "Planilha Feeder não localizada."; ;

                    return retorno;
                }

                using ExcelPackage package = new ExcelPackage(Utilidades.IO.FileStorageService.Storage.OpenRead(montagemFeeder.CaminhoPlanilha));
                using ExcelWorksheet worksheet = package.Workbook.Worksheets.First();
                string nomeArquivo = montagemFeeder.Planilha;

                if (montagemFeeder.TipoPlanilhaFeeder == TipoPlanilhaFeeder.Feeder)
                {
                    if (!ImportarPlanilhaCTNRFeeder(worksheet, unitOfWork, ref erros, montagemFeeder.Destinatario, montagemFeeder.CNPJCPFDestinatario, montagemFeeder.EmbarqueAfretamento, montagemFeeder.Expedidor, montagemFeeder.CNPJCPFExpedidor, montagemFeeder.Tomador, montagemFeeder.ImportarMesmoSemCTeAbsorvidoAnteriormente, nomeArquivo, auditado, montagemFeeder.Usuario, tipoServicoMultisoftware, montagemFeeder.Codigo, urlAcessoCliente, montagemFeeder.ImportarMesmoComDocumentacaoDuplicada))
                    {
                        package.Dispose();
                        retornoFinaliza = erros.ToString();
                    }
                    else
                    {
                        package.Dispose();
                    }
                }
                else if (montagemFeeder.TipoPlanilhaFeeder == TipoPlanilhaFeeder.Subcontratacao)
                {
                    if (!ImportarPlanilhaCTNRFeederSubcontratacao(worksheet, unitOfWork, ref erros, montagemFeeder.Destinatario, montagemFeeder.CNPJCPFDestinatario, montagemFeeder.EmbarqueAfretamento, montagemFeeder.Expedidor, montagemFeeder.CNPJCPFExpedidor, montagemFeeder.Tomador, montagemFeeder.ImportarMesmoSemCTeAbsorvidoAnteriormente, nomeArquivo, auditado, montagemFeeder.Usuario, tipoServicoMultisoftware, montagemFeeder.Codigo, urlAcessoCliente, montagemFeeder.ImportarMesmoComDocumentacaoDuplicada))
                    {
                        package.Dispose();
                        retornoFinaliza = erros.ToString();
                    }
                    else
                    {
                        package.Dispose();
                    }
                }
                else if (montagemFeeder.TipoPlanilhaFeeder == TipoPlanilhaFeeder.SubcontratacaoTipoUm)
                {
                    if (!ImportarPlanilhaCTNRFeederSubcontratacaoTipoUm(worksheet, unitOfWork, ref erros, montagemFeeder.Destinatario, montagemFeeder.CNPJCPFDestinatario, montagemFeeder.EmbarqueAfretamento, montagemFeeder.Expedidor, montagemFeeder.CNPJCPFExpedidor, montagemFeeder.Tomador, montagemFeeder.ImportarMesmoSemCTeAbsorvidoAnteriormente, nomeArquivo, auditado, montagemFeeder.Usuario, tipoServicoMultisoftware, montagemFeeder.Codigo, urlAcessoCliente, montagemFeeder.ImportarMesmoComDocumentacaoDuplicada))
                    {
                        package.Dispose();
                        retornoFinaliza = erros.ToString();
                    }
                    else
                    {
                        package.Dispose();
                    }
                }
                else
                    retornoFinaliza = "Não foi encontrada nenhuma configuração para a importação desta planilha, favor verifique o layout do arquivo.";


                retorno.TotalPedidos = 1;
                retorno.TotalCargas = 1;
                retorno.Sucesso = string.IsNullOrWhiteSpace(retornoFinaliza);
                retorno.Mensagem = string.IsNullOrWhiteSpace(retornoFinaliza) ? "Geração de cargas Feeder finalizado com sucesso." : retornoFinaliza;
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(ex);

                retorno.Sucesso = false;
                retorno.Mensagem = ex.Message;
            }

            return retorno;
        }

        public bool ImportarPlanilhaCTNRFeeder(ExcelWorksheet worksheet, Repositorio.UnitOfWork unidadeDeTrabalho, ref StringBuilder erros, Dominio.Entidades.Cliente destinatario, double cnpj_cpf_destinatario, bool embarqueAfretamento, Dominio.Entidades.Cliente expedidor, double cnpj_cpf_expedidor, Dominio.Entidades.Cliente tomador, bool importarMesmoSemCTeAbsorvidoAnteriormente, string nomeArquivo, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, int codigoMontagemFeeder, string urlAcessoCliente, bool importarMesmoComDocumentacaoDuplicada)
        {
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.PedidoProduto repPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.MontagemFeeder.MontagemFeeder repMontagemFeeder = new Repositorio.Embarcador.Cargas.MontagemFeeder.MontagemFeeder(unidadeDeTrabalho);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.PedidoEndereco repPedidoEndereco = new Repositorio.Embarcador.Pedidos.PedidoEndereco(unidadeDeTrabalho);
            Servicos.Embarcador.Pedido.Pedido serPedido = new Servicos.Embarcador.Pedido.Pedido(unidadeDeTrabalho);

            Repositorio.Embarcador.Pedidos.PedidoViagemNavio repViagem = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.Porto repPorto = new Repositorio.Embarcador.Pedidos.Porto(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoMontagemCarga repConfiguracaoMontagemCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMontagemCarga(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMontagemCarga configuracaoMontagem = repConfiguracaoMontagemCarga.BuscarPrimeiroRegistro();
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            string erro = "";
            int linhaInicial = 36, colunaBooking = 2, colunaProposta = 3, colunaManifesto = 4, colunaProtocoloANTAQ = 5, colunaNumeroCE = 6, colunaContainer = 7, colunaTipoContainer = 8, colunaTara = 9, colunaPesoLiquido = 10, colunaPesoPruto = 11, colunaLacre = 12, colunaMercadoria = 13, colunaNCM = 14, colunaTemperatura = 17, colunaBookingReference = 24;

            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

            List<string> numerosBooking = new List<string>();
            List<int> codigosPedidos = new List<int>();
            List<string> numerosContaineres = new List<string>();
            List<Dominio.ObjetosDeValor.Embarcador.Pedido.BookingFeeder> bookingsFeeder = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.BookingFeeder>();

            string viagemNavioDirecao = Utilidades.String.RemoveDiacritics(worksheet.Cells[16, 3].Text);
            string portoOrigem = Utilidades.String.RemoveDiacritics(worksheet.Cells[12, 3].Text);
            string portoDestino = Utilidades.String.RemoveDiacritics(worksheet.Cells[14, 3].Text);

            Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio viagem = repViagem.BuscarPorDescricao(viagemNavioDirecao.ToUpper());
            Dominio.Entidades.Embarcador.Pedidos.Porto portOrigem = repPorto.BuscarPorCodigoIATA(portoOrigem.ToUpper());
            Dominio.Entidades.Embarcador.Cargas.MontagemFeeder.MontagemFeeder montagemFeeder = repMontagemFeeder.BuscarPorCodigo(codigoMontagemFeeder);

            for (var i = linhaInicial; i <= worksheet.Dimension.End.Row; i++)
            {
                unidadeDeTrabalho.FlushAndClear();
                unidadeDeTrabalho.Start();

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = null;
                Dominio.Entidades.Embarcador.Pedidos.Container container = null;
                Dominio.Entidades.Embarcador.Pedidos.ContainerTipo containerTipo = null;
                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador = null;

                string numeroContainer = Utilidades.String.RemoveDiacritics(worksheet.Cells[i, colunaContainer].Text);
                string numeroBooking = Utilidades.String.RemoveDiacritics(worksheet.Cells[i, colunaBooking].Text);

                if (string.IsNullOrWhiteSpace(numeroBooking) && !string.IsNullOrWhiteSpace(numeroContainer))
                {
                    erro = "Número do Booking não informado na planilha";
                    erros.Append("Linha ").Append(i.ToString()).Append(": ").Append(erro).AppendLine();
                    continue;
                }

                if (string.IsNullOrWhiteSpace(numeroContainer) || string.IsNullOrWhiteSpace(numeroBooking))
                    break;

                for (int a = linhaInicial; a <= worksheet.Dimension.End.Row; a++)
                {
                    if (!string.IsNullOrWhiteSpace(worksheet.Cells[a, colunaBooking].Text))
                    {
                        string numBooking = worksheet.Cells[a, colunaBooking].Text;

                        if (!bookingsFeeder.Any(c => c.NumeroBooking == numBooking))
                        {
                            bookingsFeeder.Add(new Dominio.ObjetosDeValor.Embarcador.Pedido.BookingFeeder()
                            {
                                NumeroBooking = numBooking,
                                QtdPlanilha = 0,
                                QtdSistema = repPedido.BuscarQuantidadeBooking(numBooking),
                                Containeres = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.BookingFeederContainer>()
                            });
                        }
                        if (bookingsFeeder.Any(c => c.NumeroBooking == numBooking && !c.Containeres.Any(o => o.NumeroContainer == Utilidades.String.RemoveDiacritics(worksheet.Cells[a, colunaContainer].Text))))
                        {
                            bookingsFeeder.Where(c => c.NumeroBooking == numBooking).FirstOrDefault().QtdPlanilha += 1;
                            bookingsFeeder.Where(c => c.NumeroBooking == numBooking).FirstOrDefault().Containeres.Add(new Dominio.ObjetosDeValor.Embarcador.Pedido.BookingFeederContainer()
                            {
                                NumeroContainer = Utilidades.String.RemoveDiacritics(worksheet.Cells[a, colunaContainer].Text)
                            });
                        }
                    }
                }
                foreach (var bookingFeeder in bookingsFeeder)
                {
                    if (bookingFeeder.QtdSistema != bookingFeeder.QtdPlanilha)
                    {
                        erro = "Quantidade do Booking da planilha divergente da quantidade de pedidos do Booking do sistema. BK: " + bookingFeeder.NumeroBooking + ". Quantidade na Planilha: " + bookingFeeder.QtdPlanilha + " Quantidade no Sistema: " + bookingFeeder.QtdSistema;
                        erros.Append(erro).AppendLine();
                    }
                    if ((configuracaoMontagem?.AtivarTratativaDuplicidadeEmissaoCargasFeeder ?? false) && !importarMesmoComDocumentacaoDuplicada && viagem != null && portOrigem != null && bookingFeeder.Containeres != null && bookingFeeder.Containeres.Count > 0)
                    {
                        foreach (var cont in bookingFeeder.Containeres)
                        {
                            string numeroBookingAberto = repCargaPedido.ContemCargaEmAberto(cont.NumeroContainer, viagem.Codigo, portOrigem.Codigo, TipoPropostaMultimodal.Feeder);
                            if (!string.IsNullOrWhiteSpace(numeroBookingAberto))
                            {
                                erro = "Container já possui documentação para " + cont.NumeroContainer + " + " + viagem.Descricao + " + " + portOrigem.Descricao + " + Tipo Proposta Feeder";
                                erros.Append(erro).AppendLine();
                            }
                        }
                    }
                }
                if (erros.Length > 0)
                {
                    unidadeDeTrabalho.CommitChanges();
                    return false;
                }

                string tipoContainer = Utilidades.String.RemoveDiacritics(worksheet.Cells[i, colunaTipoContainer].Text);
                erro = "";
                if (!ObterTipoContainer(out erro, out containerTipo, tipoContainer, unidadeDeTrabalho))
                {
                    erros.Append("Linha ").Append(i.ToString()).Append(": ").Append(erro).AppendLine();
                    continue;
                }

                erro = "";
                if (!ObterContainer(out erro, out container, numeroContainer, unidadeDeTrabalho, containerTipo, auditado))
                {
                    erros.Append("Linha ").Append(i.ToString()).Append(": ").Append(erro).AppendLine();
                    continue;
                }

                erro = "";
                if (!ObterPedido(out erro, out pedido, numeroBooking, numeroContainer, unidadeDeTrabalho, container, portoOrigem, portoDestino, viagemNavioDirecao, i, nomeArquivo))
                {
                    erros.Append("Linha ").Append(i.ToString()).Append(": ").Append(erro).AppendLine();
                    if (montagemFeeder != null && montagemFeeder.Bookings == null)
                        montagemFeeder.Bookings = new List<string>();

                    if (montagemFeeder != null)
                    {
                        montagemFeeder.Bookings.Add(numeroBooking);
                        repMontagemFeeder.Atualizar(montagemFeeder);
                        unidadeDeTrabalho.CommitChanges();
                    }
                    continue;
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(numeroBooking))
                        numeroBooking = numeroBooking.Trim();
                    if (!numerosBooking.Contains(numeroBooking))
                    {
                        numerosBooking.Add(numeroBooking);
                        if (montagemFeeder != null && montagemFeeder.Bookings == null)
                            montagemFeeder.Bookings = new List<string>();
                        if (montagemFeeder != null)
                        {
                            montagemFeeder.Bookings.Add(numeroBooking);
                            repMontagemFeeder.Atualizar(montagemFeeder);
                        }
                    }

                    codigosPedidos.Add(pedido.Codigo);

                    pedido.EmbarqueAfretamentoFeeder = embarqueAfretamento;
                    pedido.NumeroManifestoFeeder = Utilidades.String.RemoveDiacritics(worksheet.Cells[i, colunaManifesto].Text);
                    pedido.ProtocoloANTAQFeeder = Utilidades.String.RemoveDiacritics(worksheet.Cells[i, colunaProtocoloANTAQ].Text);
                    pedido.NumeroCEFeeder = Utilidades.String.RemoveDiacritics(worksheet.Cells[i, colunaNumeroCE].Text);
                    if (embarqueAfretamento)
                    {
                        if (string.IsNullOrWhiteSpace(pedido.NumeroManifestoFeeder) || string.IsNullOrWhiteSpace(pedido.ProtocoloANTAQFeeder) || string.IsNullOrWhiteSpace(pedido.NumeroCEFeeder))
                        {
                            erro = "Não foi localizado o número do Manifesto e/ou CE e/ou Protocolo ANTAQ";
                            erros.Append("Linha ").Append(i.ToString()).Append(": ").Append(erro).AppendLine();
                            unidadeDeTrabalho.CommitChanges();
                            return false;
                        }
                    }

                    pedido.NumeroProposta = Utilidades.String.RemoveDiacritics(worksheet.Cells[i, colunaProposta].Text);
                    pedido.Destinatario = destinatario;
                    pedido.Container = container;
                    pedido.ContainerTipoReserva = containerTipo;

                    if (tomador != null && !((configuracao?.AtivarNovaDefinicaoDoTomadorParaCargasFeederMontagemCarga ?? false)))
                    {
                        pedido.Tomador = tomador;
                        pedido.UsarTipoTomadorPedido = true;
                        pedido.TipoTomador = Dominio.Enumeradores.TipoTomador.Outros;
                        pedido.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Outros;
                    }

                    Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEnderecoDestino = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco();
                    serPedido.PreecherEnderecoPedido(ref pedidoEnderecoDestino, destinatario);
                    repPedidoEndereco.Inserir(pedidoEnderecoDestino);
                    pedido.EnderecoDestino = pedidoEnderecoDestino;
                    pedido.Destino = pedidoEnderecoDestino?.Localidade;

                    if (expedidor != null && pedido.TipoOperacao != null && (pedido.TipoOperacao.TipoEmissaoCTeParticipantes == TipoEmissaoCTeParticipantes.ComExpedidor || pedido.TipoOperacao.TipoEmissaoCTeParticipantes == TipoEmissaoCTeParticipantes.ComExpedidorERecebedor))
                    {
                        Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEnderecoOrigem = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco();
                        pedido.Expedidor = expedidor;
                        if (pedido.Expedidor != null)
                        {
                            serPedido.PreecherEnderecoPedido(ref pedidoEnderecoOrigem, expedidor);
                            pedido.EnderecoOrigem = pedidoEnderecoOrigem;
                            pedido.Origem = pedidoEnderecoOrigem?.Localidade;
                        }

                        if (pedido.Empresa != null && pedido.Empresa.EstadosFeeder != null && pedido.Empresa.EstadosFeeder.Count > 0 && expedidor.Localidade != null && expedidor.Localidade.Estado != null)
                        {
                            if (!pedido.Empresa.EstadosFeeder.Any(c => c.Sigla == expedidor.Localidade.Estado.Sigla))
                            {
                                erro = "UF de Origem do Expedidor inconsistente com a Filial do Booking " + pedido.NumeroBooking;
                                erros.Append("Linha ").Append(i.ToString()).Append(": ").Append(erro).AppendLine();
                                unidadeDeTrabalho.CommitChanges();
                                return false;
                            }
                        }
                    }
                    if (pedido.TipoOperacao != null && (pedido.TipoOperacao.TipoEmissaoCTeParticipantes == TipoEmissaoCTeParticipantes.ComRecebedor || pedido.TipoOperacao.TipoEmissaoCTeParticipantes == TipoEmissaoCTeParticipantes.ComExpedidorERecebedor))
                        pedido.Recebedor = destinatario;

                    string taraContainer = RemoveExtraText(worksheet.Cells[i, colunaTara].Text);
                    if (string.IsNullOrWhiteSpace(taraContainer))
                        taraContainer = "0,00";

                    pedido.TaraContainer = Utilidades.String.OnlyNumbers(Utilidades.Decimal.Converter(taraContainer).ToString("n0"));

                    decimal pesoLiquidoConvertido = 0;
                    string pesoLiquido = RemoveExtraText(worksheet.Cells[i, colunaPesoLiquido].Text);
                    if (string.IsNullOrWhiteSpace(pesoLiquido))
                        pesoLiquido = "0,00";
                    try
                    {
                        pedido.PesoTotal = Utilidades.Decimal.Converter(pesoLiquido);
                        pesoLiquidoConvertido = Utilidades.Decimal.Converter(pesoLiquido);
                    }
                    catch
                    {
                        pedido.PesoTotal = 0;
                    }
                    decimal pesoBrutoDecimal = 0;
                    string pesoBruto = RemoveExtraText(worksheet.Cells[i, colunaPesoPruto].Text);
                    if (string.IsNullOrWhiteSpace(pesoBruto))
                        pesoBruto = "0,00";
                    try
                    {
                        pesoBrutoDecimal = Utilidades.Decimal.Converter(pesoBruto);
                    }
                    catch
                    {
                        pesoBrutoDecimal = 0;
                    }

                    pedido.PesoTotal = pesoBrutoDecimal;

                    if (pesoBrutoDecimal < (decimal)2000.00 || pesoBrutoDecimal > (decimal)42000.00)
                    {
                        erro = "O peso bruto está inferior a 2 toneladas ou superior a 42 toneladas.";
                        erros.Append("Linha ").Append(i.ToString()).Append(": ").Append(erro).AppendLine();
                        unidadeDeTrabalho.CommitChanges();
                        return false;
                    }

                    pedido.LacreContainerUm = Utilidades.String.RemoveDiacritics(worksheet.Cells[i, colunaLacre].Text);
                    pedido.Temperatura = Utilidades.String.RemoveDiacritics(worksheet.Cells[i, colunaTemperatura].Text);
                    pedido.BookingReference = Utilidades.String.RemoveDiacritics(worksheet.Cells[i, colunaBookingReference].Text);

                    string mercadoria = Utilidades.String.RemoveDiacritics(worksheet.Cells[i, colunaMercadoria].Text);
                    erro = "";
                    if (!ObterProdutoEmbarcador(out erro, out produtoEmbarcador, mercadoria, unidadeDeTrabalho))
                    {
                        erros.Append("Linha ").Append(i.ToString()).Append(": ").Append(erro).AppendLine();
                        continue;
                    }
                    pedido.ProdutoPredominante = mercadoria;
                    if (produtoEmbarcador != null)
                    {
                        if (!repPedidoProduto.ExisteProdutoPedido(pedido.Codigo, produtoEmbarcador.Codigo))
                        {
                            Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProduto = new Dominio.Entidades.Embarcador.Pedidos.PedidoProduto();
                            pedidoProduto.Produto = produtoEmbarcador;
                            pedidoProduto.Pedido = pedido;
                            try
                            {
                                pedidoProduto.PesoUnitario = Utilidades.Decimal.Converter(pesoLiquido);
                                pedidoProduto.PesoUnitario = Utilidades.Decimal.Converter(pesoBruto);
                            }
                            catch
                            {
                                pedidoProduto.PesoUnitario = 0;
                                pedidoProduto.PesoUnitario = 0;
                            }

                            repPedidoProduto.Inserir(pedidoProduto);
                        }
                    }
                    if (pedido.TipoOperacao != null && (pedido.TipoOperacao.TipoServicoMultimodal == TipoServicoMultimodal.Redespacho || pedido.TipoOperacao.TipoServicoMultimodal == TipoServicoMultimodal.RedespachoIntermediario || pedido.TipoOperacao.TipoServicoMultimodal == TipoServicoMultimodal.Subcontratacao || pedido.TipoOperacao.TipoServicoMultimodal == TipoServicoMultimodal.VinculadoMultimodalTerceiro))
                    {
                        if (pedido.TipoOperacao.TipoServicoMultimodal == TipoServicoMultimodal.VinculadoMultimodalTerceiro)
                            pedido.PedidoDeSVMTerceiro = true;
                        else
                            pedido.PedidoSubContratado = true;

                        if (pedido.PedidoDeSVMTerceiro)
                            pedido.PedidoSVM = false;

                        if (expedidor != null)
                        {
                            if (pedido.CTesTerceiro == null)
                                pedido.CTesTerceiro = new List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro>();
                            Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro = null;
                            erro = "";
                            if (!ObterDocumentoAnterior(out erro, out cteTerceiro, Utilidades.String.OnlyNumbers(Utilidades.String.RemoveDiacritics(worksheet.Cells[i, colunaNCM].Text)), unidadeDeTrabalho, pedido, pesoBrutoDecimal, pesoLiquidoConvertido, cnpj_cpf_destinatario, Utilidades.String.OnlyNumbers(Utilidades.String.RemoveDiacritics(worksheet.Cells[i, colunaNumeroCE].Text)), mercadoria, destinatario, expedidor, "", "", Utilidades.String.OnlyNumbers(Utilidades.String.RemoveDiacritics(worksheet.Cells[i, colunaNCM].Text)), tomador, pedido.Recebedor, importarMesmoSemCTeAbsorvidoAnteriormente, importarMesmoComDocumentacaoDuplicada, false))
                            {
                                erros.Append("Linha ").Append(i.ToString()).Append(": ").Append(erro).AppendLine();
                                continue;
                            }
                            if (cteTerceiro != null)
                            {
                                if (!pedido.CTesTerceiro.Any(o => o.Codigo == cteTerceiro.Codigo))
                                    pedido.CTesTerceiro.Add(cteTerceiro);
                            }
                        }
                        else
                            erros.Append("Linha ").Append(i.ToString()).Append(": ").Append("Expedidor não cadastrado no TMS").AppendLine();
                    }
                    else
                    {
                        if (pedido.NotasFiscais == null)
                            pedido.NotasFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
                        Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = null;
                        erro = "";
                        if (!ObterNotaFiscal(out erro, out xmlNotaFiscal, Utilidades.String.OnlyNumbers(Utilidades.String.RemoveDiacritics(worksheet.Cells[i, colunaNCM].Text)), unidadeDeTrabalho, pedido, pesoBrutoDecimal, pesoLiquidoConvertido, cnpj_cpf_destinatario, Utilidades.String.OnlyNumbers(Utilidades.String.RemoveDiacritics(worksheet.Cells[i, colunaNumeroCE].Text))))
                        {
                            erros.Append("Linha ").Append(i.ToString()).Append(": ").Append(erro).AppendLine();
                            continue;
                        }
                        if (xmlNotaFiscal != null)
                        {
                            if (!pedido.NotasFiscais.Any(o => o.Codigo == xmlNotaFiscal.Codigo))
                                pedido.NotasFiscais.Add(xmlNotaFiscal);
                        }
                    }
                    if (string.IsNullOrWhiteSpace(pedido.ObservacaoCTe))
                        pedido.ObservacaoCTe = "";
                    if (!string.IsNullOrWhiteSpace(pedido.NumeroManifestoFeeder))
                        pedido.ObservacaoCTe += " Nº Manifesto " + pedido.NumeroManifestoFeeder;
                    if (!string.IsNullOrWhiteSpace(pedido.ProtocoloANTAQFeeder))
                        pedido.ObservacaoCTe += " Protocolo ANATQ " + pedido.ProtocoloANTAQFeeder;
                    if (!string.IsNullOrWhiteSpace(pedido.NumeroCEFeeder))
                        pedido.ObservacaoCTe += " Nº CE " + pedido.NumeroCEFeeder;
                    if (string.IsNullOrWhiteSpace(pedido.ObservacaoCTe))
                        pedido.ObservacaoCTe = (pedido.ObservacaoCTe ?? string.Empty).Length > 1999 ? pedido.ObservacaoCTe.Substring(0, 2000) : pedido.ObservacaoCTe;

                    pedido.ImprimirObservacaoCTe = true;
                    pedido.TipoServicoCarga = TipoServicoCarga.Feeder;

                    repPedido.Atualizar(pedido);
                    Servicos.Auditoria.Auditoria.Auditar(auditado, pedido, null, "Pedido atualizado pela importação da planilha de Feeder.", unidadeDeTrabalho);
                }

                unidadeDeTrabalho.CommitChanges();

                if (pedido != null)
                    pedidos.Add(pedido);
            }


            unidadeDeTrabalho.FlushAndClear();

            if (erros.Length > 0)
                return false;//new JsonpResult(false, true, erros.ToString());

            usuario = repUsuario.BuscarPorCodigo(usuario?.Codigo ?? 0);
            erro = "";
            if (CriarCarregamentosPedidos(out erro, numerosBooking, codigosPedidos, usuario, tipoServicoMultisoftware, unidadeDeTrabalho, auditado, urlAcessoCliente, montagemFeeder))
                return true;//new JsonpResult(true);
            else
            {
                erros.Append("Não foi possível gerar a carga, favor contate a equipe de suporte: ").Append(erro).AppendLine();
                return false;//new JsonpResult(false, true, erros.ToString());
            }
        }

        public bool ImportarPlanilhaCTNRFeederSubcontratacao(ExcelWorksheet worksheet, Repositorio.UnitOfWork unidadeDeTrabalho, ref StringBuilder erros, Dominio.Entidades.Cliente destinatario, double cnpj_cpf_destinatario, bool embarqueAfretamento, Dominio.Entidades.Cliente expedidor, double cnpj_cpf_expedidor, Dominio.Entidades.Cliente tomador, bool importarMesmoSemCTeAbsorvidoAnteriormente, string nomeArquivo, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, int codigoMontagemFeeder, string urlAcessoCliente, bool importarMesmoComDocumentacaoDuplicada)
        {
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.PedidoProduto repPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.PedidoEndereco repPedidoEndereco = new Repositorio.Embarcador.Pedidos.PedidoEndereco(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.MontagemFeeder.MontagemFeeder repMontagemFeeder = new Repositorio.Embarcador.Cargas.MontagemFeeder.MontagemFeeder(unidadeDeTrabalho);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeDeTrabalho);
            Servicos.Embarcador.Pedido.Pedido serPedido = new Servicos.Embarcador.Pedido.Pedido(unidadeDeTrabalho);

            Repositorio.Embarcador.Pedidos.PedidoViagemNavio repViagem = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.Porto repPorto = new Repositorio.Embarcador.Pedidos.Porto(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoMontagemCarga repConfiguracaoMontagemCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMontagemCarga(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMontagemCarga configuracaoMontagem = repConfiguracaoMontagemCarga.BuscarPrimeiroRegistro();
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            string erro = "";
            int linhaInicial = 36, colunaBooking = 2, colunaProposta = 3, colunaChaveCTe = 5, colunaChaveNFe = 6, colunaContainer = 7, colunaTipoContainer = 8, colunaTara = 9, colunaPesoLiquido = 10, colunaPesoPruto = 11, colunaLacre = 12, colunaNCM = 14, colunaTemperatura = 16, colunaBookingReference = 19;

            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

            List<string> numerosBooking = new List<string>();
            List<string> numerosContaineres = new List<string>();
            List<int> codigosPedidos = new List<int>();
            List<Dominio.ObjetosDeValor.Embarcador.Pedido.BookingFeeder> bookingsFeeder = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.BookingFeeder>();
            string viagemNavioDirecao = Utilidades.String.RemoveDiacritics(worksheet.Cells[16, 3].Text);
            string portoOrigem = Utilidades.String.RemoveDiacritics(worksheet.Cells[12, 3].Text);
            string portoDestino = Utilidades.String.RemoveDiacritics(worksheet.Cells[14, 3].Text);

            Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio viagem = repViagem.BuscarPorDescricao(viagemNavioDirecao.ToUpper());
            Dominio.Entidades.Embarcador.Pedidos.Porto portOrigem = repPorto.BuscarPorCodigoIATA(portoOrigem.ToUpper());
            Dominio.Entidades.Embarcador.Cargas.MontagemFeeder.MontagemFeeder montagemFeeder = repMontagemFeeder.BuscarPorCodigo(codigoMontagemFeeder);

            for (var i = linhaInicial; i <= worksheet.Dimension.End.Row; i++)
            {
                unidadeDeTrabalho.FlushAndClear();
                unidadeDeTrabalho.Start();

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = null;
                Dominio.Entidades.Embarcador.Pedidos.Container container = null;
                Dominio.Entidades.Embarcador.Pedidos.ContainerTipo containerTipo = null;

                string numeroContainer = Utilidades.String.RemoveDiacritics(worksheet.Cells[i, colunaContainer].Text);
                string numeroBooking = Utilidades.String.RemoveDiacritics(worksheet.Cells[i, colunaBooking].Text);
                string chaveNFE = Utilidades.String.RemoveDiacritics(worksheet.Cells[i, colunaChaveNFe].Text);
                string chaveCTe = Utilidades.String.RemoveDiacritics(worksheet.Cells[i, colunaChaveCTe].Text);
                string ncm = Utilidades.String.RemoveDiacritics(worksheet.Cells[i, colunaNCM].Text);

                if (string.IsNullOrWhiteSpace(numeroBooking) && !string.IsNullOrWhiteSpace(numeroContainer))
                {
                    erro = "Número do Booking não informado na planilha";
                    erros.Append("Linha ").Append(i.ToString()).Append(": ").Append(erro).AppendLine();
                    continue;
                }

                if (string.IsNullOrWhiteSpace(numeroBooking) && string.IsNullOrWhiteSpace(numeroContainer))
                    break;

                for (int a = linhaInicial; a <= worksheet.Dimension.End.Row; a++)
                {
                    if (!string.IsNullOrWhiteSpace(worksheet.Cells[a, colunaBooking].Text))
                    {
                        string numBooking = worksheet.Cells[a, colunaBooking].Text;

                        if (!bookingsFeeder.Any(c => c.NumeroBooking == numBooking))
                        {
                            bookingsFeeder.Add(new Dominio.ObjetosDeValor.Embarcador.Pedido.BookingFeeder()
                            {
                                NumeroBooking = numBooking,
                                QtdPlanilha = 0,
                                QtdSistema = repPedido.BuscarQuantidadeBooking(numBooking),
                                Containeres = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.BookingFeederContainer>()
                            });
                        }
                        if (bookingsFeeder.Any(c => c.NumeroBooking == numBooking && !c.Containeres.Any(o => o.NumeroContainer == Utilidades.String.RemoveDiacritics(worksheet.Cells[a, colunaContainer].Text))))
                        {
                            bookingsFeeder.Where(c => c.NumeroBooking == numBooking).FirstOrDefault().QtdPlanilha += 1;
                            bookingsFeeder.Where(c => c.NumeroBooking == numBooking).FirstOrDefault().Containeres.Add(new Dominio.ObjetosDeValor.Embarcador.Pedido.BookingFeederContainer()
                            {
                                NumeroContainer = Utilidades.String.RemoveDiacritics(worksheet.Cells[a, colunaContainer].Text)
                            });
                        }
                    }
                }
                foreach (var bookingFeeder in bookingsFeeder)
                {
                    erro = "";
                    if (bookingFeeder.QtdSistema != bookingFeeder.QtdPlanilha)
                    {
                        erro = "Quantidade do Booking da planilha divergente da quantidade de pedidos do Booking do sistema. BK: " + bookingFeeder.NumeroBooking + ". Quantidade na Planilha: " + bookingFeeder.QtdPlanilha + " Quantidade no Sistema: " + bookingFeeder.QtdSistema;
                        erros.Append(erro).AppendLine();
                        return false;
                    }
                    if ((configuracaoMontagem?.AtivarTratativaDuplicidadeEmissaoCargasFeeder ?? false) && !importarMesmoComDocumentacaoDuplicada && viagem != null && portOrigem != null && bookingFeeder.Containeres != null && bookingFeeder.Containeres.Count > 0)
                    {
                        foreach (var cont in bookingFeeder.Containeres)
                        {
                            string numeroBookingAberto = repCargaPedido.ContemCargaEmAberto(cont.NumeroContainer, viagem.Codigo, portOrigem.Codigo, TipoPropostaMultimodal.Feeder);
                            if (!string.IsNullOrWhiteSpace(numeroBookingAberto))
                            {
                                erro = "Container já possui documentação para " + cont.NumeroContainer + " + " + viagem.Descricao + " + " + portOrigem.Descricao + " + Tipo Proposta Feeder";
                                erros.Append(erro).AppendLine();
                            }
                        }
                    }
                }
                if (erros.Length > 0)
                {
                    unidadeDeTrabalho.CommitChanges();
                    if (!string.IsNullOrWhiteSpace(erro))
                    {
                        erros.Append("Linha ").Append(i.ToString()).Append(": ").Append(erro).AppendLine();
                        return false;
                    }
                }

                string tipoContainer = Utilidades.String.RemoveDiacritics(worksheet.Cells[i, colunaTipoContainer].Text);
                erro = "";
                if (!ObterTipoContainer(out erro, out containerTipo, tipoContainer, unidadeDeTrabalho))
                {
                    erros.Append("Linha ").Append(i.ToString()).Append(": ").Append(erro).AppendLine();
                    continue;
                }

                erro = "";
                if (!ObterContainer(out erro, out container, numeroContainer, unidadeDeTrabalho, containerTipo, auditado))
                {
                    erros.Append("Linha ").Append(i.ToString()).Append(": ").Append(erro).AppendLine();
                    continue;
                }

                erro = "";
                if (!ObterPedido(out erro, out pedido, numeroBooking, numeroContainer, unidadeDeTrabalho, container, portoOrigem, portoDestino, viagemNavioDirecao, i, nomeArquivo))
                {
                    erros.Append("Linha ").Append(i.ToString()).Append(": ").Append(erro).AppendLine();
                    if (montagemFeeder != null && montagemFeeder.Bookings == null)
                        montagemFeeder.Bookings = new List<string>();

                    if (montagemFeeder != null)
                    {
                        montagemFeeder.Bookings.Add(numeroBooking);
                        repMontagemFeeder.Atualizar(montagemFeeder);
                        unidadeDeTrabalho.CommitChanges();
                    }
                    continue;
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(numeroBooking))
                        numeroBooking = numeroBooking.Trim();

                    if (!numerosBooking.Contains(numeroBooking))
                    {
                        numerosBooking.Add(numeroBooking);
                        if (montagemFeeder != null && montagemFeeder.Bookings == null)
                            montagemFeeder.Bookings = new List<string>();
                        if (montagemFeeder != null)
                        {
                            montagemFeeder.Bookings.Add(numeroBooking);
                            repMontagemFeeder.Atualizar(montagemFeeder);
                        }
                    }

                    codigosPedidos.Add(pedido.Codigo);

                    pedido.NumeroProposta = Utilidades.String.RemoveDiacritics(worksheet.Cells[i, colunaProposta].Text);
                    pedido.Destinatario = destinatario;
                    pedido.Container = container;
                    pedido.ContainerTipoReserva = containerTipo;

                    if (tomador != null)
                    {
                        pedido.Tomador = tomador;
                        pedido.UsarTipoTomadorPedido = true;
                        pedido.TipoTomador = Dominio.Enumeradores.TipoTomador.Outros;
                        pedido.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Outros;
                    }

                    Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEnderecoDestino = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco();
                    serPedido.PreecherEnderecoPedido(ref pedidoEnderecoDestino, destinatario);
                    repPedidoEndereco.Inserir(pedidoEnderecoDestino);
                    pedido.EnderecoDestino = pedidoEnderecoDestino;
                    pedido.Destino = pedidoEnderecoDestino?.Localidade;

                    if (expedidor != null && pedido.TipoOperacao != null && (pedido.TipoOperacao.TipoEmissaoCTeParticipantes == TipoEmissaoCTeParticipantes.ComExpedidor || pedido.TipoOperacao.TipoEmissaoCTeParticipantes == TipoEmissaoCTeParticipantes.ComExpedidorERecebedor))
                    {
                        Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEnderecoOrigem = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco();
                        pedido.Expedidor = expedidor;
                        if (pedido.Expedidor != null)
                        {
                            serPedido.PreecherEnderecoPedido(ref pedidoEnderecoOrigem, expedidor);
                            pedido.EnderecoOrigem = pedidoEnderecoOrigem;
                            pedido.Origem = pedidoEnderecoOrigem?.Localidade;
                        }

                        if (pedido.Empresa != null && pedido.Empresa.EstadosFeeder != null && pedido.Empresa.EstadosFeeder.Count > 0 && expedidor.Localidade != null && expedidor.Localidade.Estado != null)
                        {
                            if (!pedido.Empresa.EstadosFeeder.Any(c => c.Sigla == expedidor.Localidade.Estado.Sigla))
                            {
                                erro = "UF de Origem do Expedidor inconsistente com a Filial do Booking " + pedido.NumeroBooking;
                                erros.Append("Linha ").Append(i.ToString()).Append(": ").Append(erro).AppendLine();
                                unidadeDeTrabalho.CommitChanges();
                                continue;
                            }
                        }

                    }
                    if (pedido.TipoOperacao != null && (pedido.TipoOperacao.TipoEmissaoCTeParticipantes == TipoEmissaoCTeParticipantes.ComRecebedor || pedido.TipoOperacao.TipoEmissaoCTeParticipantes == TipoEmissaoCTeParticipantes.ComExpedidorERecebedor))
                        pedido.Recebedor = destinatario;

                    string taraContainer = RemoveExtraText(worksheet.Cells[i, colunaTara].Text);
                    if (string.IsNullOrWhiteSpace(taraContainer))
                        taraContainer = "0,00";

                    pedido.TaraContainer = Utilidades.String.OnlyNumbers(Utilidades.Decimal.Converter(taraContainer).ToString("n0"));

                    string pesoLiquido = RemoveExtraText(worksheet.Cells[i, colunaPesoLiquido].Text);
                    decimal pesoLiquidoConvertido = 0;
                    decimal pesoBrutoConvertio = 0;
                    if (string.IsNullOrWhiteSpace(pesoLiquido))
                        pesoLiquido = "0,00";
                    try
                    {
                        pedido.PesoTotal = Utilidades.Decimal.Converter(pesoLiquido);
                        pesoLiquidoConvertido = Utilidades.Decimal.Converter(pesoLiquido);
                    }
                    catch
                    {
                        pedido.PesoTotal = 0;
                    }

                    string pesoBruto = RemoveExtraText(worksheet.Cells[i, colunaPesoPruto].Text);
                    if (string.IsNullOrWhiteSpace(pesoBruto))
                        pesoBruto = "0,00";
                    try
                    {
                        pedido.PesoTotal = Utilidades.Decimal.Converter(pesoBruto);
                        pesoBrutoConvertio = Utilidades.Decimal.Converter(pesoBruto);
                    }
                    catch
                    {
                        pedido.PesoTotal = 0;
                    }


                    pedido.LacreContainerUm = Utilidades.String.RemoveDiacritics(worksheet.Cells[i, colunaLacre].Text);
                    pedido.Temperatura = Utilidades.String.RemoveDiacritics(worksheet.Cells[i, colunaTemperatura].Text);
                    pedido.BookingReference = Utilidades.String.RemoveDiacritics(worksheet.Cells[i, colunaBookingReference].Text);

                    if (pedido.TipoOperacao != null && (pedido.TipoOperacao.TipoServicoMultimodal == TipoServicoMultimodal.Redespacho || pedido.TipoOperacao.TipoServicoMultimodal == TipoServicoMultimodal.RedespachoIntermediario || pedido.TipoOperacao.TipoServicoMultimodal == TipoServicoMultimodal.Subcontratacao || pedido.TipoOperacao.TipoServicoMultimodal == TipoServicoMultimodal.VinculadoMultimodalTerceiro))
                    {
                        if (pedido.TipoOperacao.TipoServicoMultimodal == TipoServicoMultimodal.VinculadoMultimodalTerceiro)
                            pedido.PedidoDeSVMTerceiro = true;
                        else
                            pedido.PedidoSubContratado = true;

                        if (pedido.PedidoDeSVMTerceiro)
                            pedido.PedidoSVM = false;

                        if (expedidor != null && !string.IsNullOrWhiteSpace(chaveCTe))
                        {
                            if (pedido.CTesTerceiro == null)
                                pedido.CTesTerceiro = new List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro>();
                            Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro = null;
                            erro = "";
                            string numeroCTe = chaveCTe.Substring(25, 9);
                            if (!ObterDocumentoAnterior(out erro, out cteTerceiro, Utilidades.String.OnlyNumbers(numeroCTe), unidadeDeTrabalho, pedido, pesoBrutoConvertio, pesoLiquidoConvertido, cnpj_cpf_destinatario, numeroCTe, "Diversos", destinatario, expedidor, chaveCTe, chaveNFE, ncm, tomador, pedido.Recebedor, importarMesmoSemCTeAbsorvidoAnteriormente, importarMesmoComDocumentacaoDuplicada, configuracao?.AtivarNovaDefinicaoDoTomadorParaCargasFeederMontagemCarga ?? false))
                            {
                                erros.Append("Linha ").Append(i.ToString()).Append(": ").Append(erro).AppendLine();
                                continue;
                            }
                            if (cteTerceiro != null)
                            {
                                if (!pedido.CTesTerceiro.Any(o => o.Codigo == cteTerceiro.Codigo))
                                    pedido.CTesTerceiro.Add(cteTerceiro);
                            }
                        }
                        else
                            erros.Append("Linha ").Append(i.ToString()).Append(": ").Append("Expedidor não cadastrado no TMS").AppendLine();
                    }
                    else
                        pedido.PedidoSubContratado = true;

                    if (string.IsNullOrWhiteSpace(pedido.ObservacaoCTe))
                        pedido.ObservacaoCTe = "";
                    if (!string.IsNullOrWhiteSpace(chaveNFE))
                        pedido.ObservacaoCTe += " Chave NF-e " + chaveNFE;
                    if (string.IsNullOrWhiteSpace(pedido.ObservacaoCTe))
                        pedido.ObservacaoCTe = (pedido.ObservacaoCTe ?? string.Empty).Length > 1999 ? pedido.ObservacaoCTe.Substring(0, 2000) : pedido.ObservacaoCTe;

                    pedido.ImprimirObservacaoCTe = true;
                    pedido.TipoServicoCarga = TipoServicoCarga.Redespacho;

                    repPedido.Atualizar(pedido);
                    Servicos.Auditoria.Auditoria.Auditar(auditado, pedido, null, "Pedido atualizado pela importação da planilha de Feeder de subcontratação.", unidadeDeTrabalho);
                }

                unidadeDeTrabalho.CommitChanges();

                if (pedido != null)
                    pedidos.Add(pedido);
            }


            unidadeDeTrabalho.FlushAndClear();

            if (erros.Length > 0)
                return false;//new JsonpResult(false, true, erros.ToString());

            usuario = repUsuario.BuscarPorCodigo(usuario?.Codigo ?? 0);
            erro = "";
            if (CriarCarregamentosPedidos(out erro, numerosBooking, codigosPedidos, usuario, tipoServicoMultisoftware, unidadeDeTrabalho, auditado, urlAcessoCliente, montagemFeeder))
                return true;//new JsonpResult(true);
            else
            {
                erros.Append("Não foi possível gerar a carga, favor contate a equipe de suporte: ").Append(erro).AppendLine();
                return false;//new JsonpResult(false, true, erros.ToString());
            }
        }

        public bool ImportarPlanilhaCTNRFeederSubcontratacaoTipoUm(ExcelWorksheet worksheet, Repositorio.UnitOfWork unidadeDeTrabalho, ref StringBuilder erros, Dominio.Entidades.Cliente destinatario, double cnpj_cpf_destinatario, bool embarqueAfretamento, Dominio.Entidades.Cliente expedidor, double cnpj_cpf_expedidor, Dominio.Entidades.Cliente tomador, bool importarMesmoSemCTeAbsorvidoAnteriormente, string nomeArquivo, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, int codigoMontagemFeeder, string urlAcessoCliente, bool importarMesmoComDocumentacaoDuplicada)
        {
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.PedidoProduto repPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.PedidoEndereco repPedidoEndereco = new Repositorio.Embarcador.Pedidos.PedidoEndereco(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.MontagemFeeder.MontagemFeeder repMontagemFeeder = new Repositorio.Embarcador.Cargas.MontagemFeeder.MontagemFeeder(unidadeDeTrabalho);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeDeTrabalho);
            Servicos.Embarcador.Pedido.Pedido serPedido = new Servicos.Embarcador.Pedido.Pedido(unidadeDeTrabalho);

            Repositorio.Embarcador.Pedidos.PedidoViagemNavio repViagem = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.Porto repPorto = new Repositorio.Embarcador.Pedidos.Porto(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoMontagemCarga repConfiguracaoMontagemCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMontagemCarga(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMontagemCarga configuracaoMontagem = repConfiguracaoMontagemCarga.BuscarPrimeiroRegistro();
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            string erro = "";
            int linhaInicial = 36, colunaBooking = 1, colunaProposta = 2, colunaChaveCTe = 4, colunaChaveNFe = 5, colunaContainer = 6, colunaTipoContainer = 7, colunaTara = 8, colunaPesoLiquido = 9, colunaPesoPruto = 10, colunaLacre = 11, colunaNCM = 12, colunaTemperatura = 14, colunaBokingReference = 17;

            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

            List<string> numerosBooking = new List<string>();
            List<int> codigosPedidos = new List<int>();
            List<string> numerosContaineres = new List<string>();
            string viagemNavioDirecao = Utilidades.String.RemoveDiacritics(worksheet.Cells[16, 3].Text);
            string portoOrigem = Utilidades.String.RemoveDiacritics(worksheet.Cells[12, 3].Text);
            string portoDestino = Utilidades.String.RemoveDiacritics(worksheet.Cells[14, 3].Text);
            List<Dominio.ObjetosDeValor.Embarcador.Pedido.BookingFeeder> bookingsFeeder = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.BookingFeeder>();

            Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio viagem = repViagem.BuscarPorDescricao(viagemNavioDirecao.ToUpper());
            Dominio.Entidades.Embarcador.Pedidos.Porto portOrigem = repPorto.BuscarPorCodigoIATA(portoOrigem.ToUpper());
            Dominio.Entidades.Embarcador.Cargas.MontagemFeeder.MontagemFeeder montagemFeeder = repMontagemFeeder.BuscarPorCodigo(codigoMontagemFeeder);

            for (var i = linhaInicial; i <= worksheet.Dimension.End.Row; i++)
            {
                unidadeDeTrabalho.FlushAndClear();
                unidadeDeTrabalho.Start();

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = null;
                Dominio.Entidades.Embarcador.Pedidos.Container container = null;
                Dominio.Entidades.Embarcador.Pedidos.ContainerTipo containerTipo = null;

                string numeroContainer = Utilidades.String.RemoveDiacritics(worksheet.Cells[i, colunaContainer].Text);
                string numeroBooking = Utilidades.String.RemoveDiacritics(worksheet.Cells[i, colunaBooking].Text);
                string chaveNFE = Utilidades.String.RemoveDiacritics(worksheet.Cells[i, colunaChaveNFe].Text);
                string chaveCTe = Utilidades.String.RemoveDiacritics(worksheet.Cells[i, colunaChaveCTe].Text);
                string ncm = Utilidades.String.RemoveDiacritics(worksheet.Cells[i, colunaNCM].Text);

                if (string.IsNullOrWhiteSpace(numeroBooking) && !string.IsNullOrWhiteSpace(numeroContainer))
                {
                    erro = "Número do Booking não informado na planilha";
                    erros.Append("Linha ").Append(i.ToString()).Append(": ").Append(erro).AppendLine();
                    continue;
                }

                if (string.IsNullOrWhiteSpace(numeroContainer) || string.IsNullOrWhiteSpace(numeroBooking))
                    break;

                for (int a = linhaInicial; a <= worksheet.Dimension.End.Row; a++)
                {
                    if (!string.IsNullOrWhiteSpace(worksheet.Cells[a, colunaBooking].Text))
                    {
                        string numBooking = worksheet.Cells[a, colunaBooking].Text;

                        if (!bookingsFeeder.Any(c => c.NumeroBooking == numBooking))
                        {
                            bookingsFeeder.Add(new Dominio.ObjetosDeValor.Embarcador.Pedido.BookingFeeder()
                            {
                                NumeroBooking = numBooking,
                                QtdPlanilha = 0,
                                QtdSistema = repPedido.BuscarQuantidadeBooking(numBooking),
                                Containeres = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.BookingFeederContainer>()
                            });
                        }
                        if (bookingsFeeder.Any(c => c.NumeroBooking == numBooking && !c.Containeres.Any(o => o.NumeroContainer == Utilidades.String.RemoveDiacritics(worksheet.Cells[a, colunaContainer].Text))))
                        {
                            bookingsFeeder.Where(c => c.NumeroBooking == numBooking).FirstOrDefault().QtdPlanilha += 1;
                            bookingsFeeder.Where(c => c.NumeroBooking == numBooking).FirstOrDefault().Containeres.Add(new Dominio.ObjetosDeValor.Embarcador.Pedido.BookingFeederContainer()
                            {
                                NumeroContainer = Utilidades.String.RemoveDiacritics(worksheet.Cells[a, colunaContainer].Text)
                            });
                        }
                    }
                }
                foreach (var bookingFeeder in bookingsFeeder)
                {
                    if (bookingFeeder.QtdSistema != bookingFeeder.QtdPlanilha)
                    {
                        erro = "Quantidade do Booking da planilha divergente da quantidade de pedidos do Booking do sistema. BK: " + bookingFeeder.NumeroBooking + ". Quantidade na Planilha: " + bookingFeeder.QtdPlanilha + " Quantidade no Sistema: " + bookingFeeder.QtdSistema;
                        erros.Append(erro).AppendLine();
                    }
                    if ((configuracaoMontagem?.AtivarTratativaDuplicidadeEmissaoCargasFeeder ?? false) && !importarMesmoComDocumentacaoDuplicada && viagem != null && portOrigem != null && bookingFeeder.Containeres != null && bookingFeeder.Containeres.Count > 0)
                    {
                        foreach (var cont in bookingFeeder.Containeres)
                        {
                            string numeroBookingAberto = repCargaPedido.ContemCargaEmAberto(cont.NumeroContainer, viagem.Codigo, portOrigem.Codigo, TipoPropostaMultimodal.Feeder);
                            if (!string.IsNullOrWhiteSpace(numeroBookingAberto))
                            {
                                erro = "Container já possui documentação para " + cont.NumeroContainer + " + " + viagem.Descricao + " + " + portOrigem.Descricao + " + Tipo Proposta Feeder";
                                erros.Append(erro).AppendLine();
                            }
                        }
                    }
                }
                if (erros.Length > 0)
                {
                    unidadeDeTrabalho.CommitChanges();
                    return false;
                }

                string tipoContainer = Utilidades.String.RemoveDiacritics(worksheet.Cells[i, colunaTipoContainer].Text);
                erro = "";
                if (!ObterTipoContainer(out erro, out containerTipo, tipoContainer, unidadeDeTrabalho))
                {
                    erros.Append("Linha ").Append(i.ToString()).Append(": ").Append(erro).AppendLine();
                    continue;
                }

                erro = "";
                if (!ObterContainer(out erro, out container, numeroContainer, unidadeDeTrabalho, containerTipo, auditado))
                {
                    erros.Append("Linha ").Append(i.ToString()).Append(": ").Append(erro).AppendLine();
                    continue;
                }

                erro = "";
                if (!ObterPedido(out erro, out pedido, numeroBooking, numeroContainer, unidadeDeTrabalho, container, portoOrigem, portoDestino, viagemNavioDirecao, i, nomeArquivo))
                {
                    erros.Append("Linha ").Append(i.ToString()).Append(": ").Append(erro).AppendLine();
                    if (montagemFeeder != null && montagemFeeder.Bookings == null)
                        montagemFeeder.Bookings = new List<string>();

                    if (montagemFeeder != null)
                    {
                        montagemFeeder.Bookings.Add(numeroBooking);
                        repMontagemFeeder.Atualizar(montagemFeeder);
                        unidadeDeTrabalho.CommitChanges();
                    }
                    continue;
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(numeroBooking))
                        numeroBooking = numeroBooking.Trim();

                    if (!numerosBooking.Contains(numeroBooking))
                    {
                        numerosBooking.Add(numeroBooking);
                        if (montagemFeeder != null && montagemFeeder.Bookings == null)
                            montagemFeeder.Bookings = new List<string>();
                        if (montagemFeeder != null)
                        {
                            montagemFeeder.Bookings.Add(numeroBooking);
                            repMontagemFeeder.Atualizar(montagemFeeder);
                        }
                    }

                    codigosPedidos.Add(pedido.Codigo);

                    pedido.NumeroProposta = Utilidades.String.RemoveDiacritics(worksheet.Cells[i, colunaProposta].Text);
                    pedido.Destinatario = destinatario;
                    pedido.Container = container;
                    pedido.ContainerTipoReserva = containerTipo;

                    if (tomador != null)
                    {
                        pedido.Tomador = tomador;
                        pedido.UsarTipoTomadorPedido = true;
                        pedido.TipoTomador = Dominio.Enumeradores.TipoTomador.Outros;
                        pedido.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Outros;
                    }

                    Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEnderecoDestino = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco();
                    serPedido.PreecherEnderecoPedido(ref pedidoEnderecoDestino, destinatario);
                    repPedidoEndereco.Inserir(pedidoEnderecoDestino);
                    pedido.EnderecoDestino = pedidoEnderecoDestino;
                    pedido.Destino = pedidoEnderecoDestino?.Localidade;

                    if (expedidor != null && pedido.TipoOperacao != null && (pedido.TipoOperacao.TipoEmissaoCTeParticipantes == TipoEmissaoCTeParticipantes.ComExpedidor || pedido.TipoOperacao.TipoEmissaoCTeParticipantes == TipoEmissaoCTeParticipantes.ComExpedidorERecebedor))
                    {
                        Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEnderecoOrigem = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco();
                        pedido.Expedidor = expedidor;
                        if (pedido.Expedidor != null)
                        {
                            serPedido.PreecherEnderecoPedido(ref pedidoEnderecoOrigem, expedidor);
                            pedido.EnderecoOrigem = pedidoEnderecoOrigem;
                            pedido.Origem = pedidoEnderecoOrigem?.Localidade;
                        }

                        if (pedido.Empresa != null && pedido.Empresa.EstadosFeeder != null && pedido.Empresa.EstadosFeeder.Count > 0 && expedidor.Localidade != null && expedidor.Localidade.Estado != null)
                        {
                            if (!pedido.Empresa.EstadosFeeder.Any(c => c.Sigla == expedidor.Localidade.Estado.Sigla))
                            {
                                erro = "UF de Origem do Expedidor inconsistente com a Filial do Booking " + pedido.NumeroBooking;
                                erros.Append("Linha ").Append(i.ToString()).Append(": ").Append(erro).AppendLine();
                                unidadeDeTrabalho.CommitChanges();
                                return false;
                            }
                        }

                    }
                    if (pedido.TipoOperacao != null && (pedido.TipoOperacao.TipoEmissaoCTeParticipantes == TipoEmissaoCTeParticipantes.ComRecebedor || pedido.TipoOperacao.TipoEmissaoCTeParticipantes == TipoEmissaoCTeParticipantes.ComExpedidorERecebedor))
                        pedido.Recebedor = destinatario;

                    string taraContainer = RemoveExtraText(worksheet.Cells[i, colunaTara].Text);
                    if (string.IsNullOrWhiteSpace(taraContainer))
                        taraContainer = "0,00";

                    pedido.TaraContainer = Utilidades.String.OnlyNumbers(Utilidades.Decimal.Converter(taraContainer).ToString("n0"));

                    string pesoLiquido = RemoveExtraText(worksheet.Cells[i, colunaPesoLiquido].Text);
                    decimal pesoLiquidoConvertido = 0;
                    decimal pesoBrutoConvertido = 0;
                    if (string.IsNullOrWhiteSpace(pesoLiquido))
                        pesoLiquido = "0,00";
                    try
                    {
                        pedido.PesoTotal = Utilidades.Decimal.Converter(pesoLiquido);
                        pesoLiquidoConvertido = Utilidades.Decimal.Converter(pesoLiquido);
                    }
                    catch
                    {
                        pedido.PesoTotal = 0;
                    }

                    string pesoBruto = RemoveExtraText(worksheet.Cells[i, colunaPesoPruto].Text);
                    if (string.IsNullOrWhiteSpace(pesoBruto))
                        pesoBruto = "0,00";
                    try
                    {
                        pedido.PesoTotal = Utilidades.Decimal.Converter(pesoBruto);
                        pesoBrutoConvertido = Utilidades.Decimal.Converter(pesoBruto);
                    }
                    catch
                    {
                        pedido.PesoTotal = 0;
                    }

                    pedido.LacreContainerUm = Utilidades.String.RemoveDiacritics(worksheet.Cells[i, colunaLacre].Text);
                    pedido.Temperatura = Utilidades.String.RemoveDiacritics(worksheet.Cells[i, colunaTemperatura].Text);
                    pedido.BookingReference = Utilidades.String.RemoveDiacritics(worksheet.Cells[i, colunaBokingReference].Text);

                    if (pedido.TipoOperacao != null && (pedido.TipoOperacao.TipoServicoMultimodal == TipoServicoMultimodal.Redespacho || pedido.TipoOperacao.TipoServicoMultimodal == TipoServicoMultimodal.RedespachoIntermediario || pedido.TipoOperacao.TipoServicoMultimodal == TipoServicoMultimodal.Subcontratacao || pedido.TipoOperacao.TipoServicoMultimodal == TipoServicoMultimodal.VinculadoMultimodalTerceiro))
                    {
                        if (pedido.TipoOperacao.TipoServicoMultimodal == TipoServicoMultimodal.VinculadoMultimodalTerceiro)
                            pedido.PedidoDeSVMTerceiro = true;
                        else
                            pedido.PedidoSubContratado = true;

                        if (pedido.PedidoDeSVMTerceiro)
                            pedido.PedidoSVM = false;

                        if (expedidor != null && !string.IsNullOrWhiteSpace(chaveCTe))
                        {
                            if (pedido.CTesTerceiro == null)
                                pedido.CTesTerceiro = new List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro>();
                            Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro = null;
                            erro = "";
                            string numeroCTe = chaveCTe.Substring(25, 9);
                            if (!ObterDocumentoAnterior(out erro, out cteTerceiro, Utilidades.String.OnlyNumbers(numeroCTe), unidadeDeTrabalho, pedido, pesoBrutoConvertido, pesoLiquidoConvertido, cnpj_cpf_destinatario, numeroCTe, "Diversos", destinatario, expedidor, chaveCTe, chaveNFE, ncm, tomador, pedido.Recebedor, importarMesmoSemCTeAbsorvidoAnteriormente, importarMesmoComDocumentacaoDuplicada, configuracao?.AtivarNovaDefinicaoDoTomadorParaCargasFeederMontagemCarga ?? false))
                            {
                                erros.Append("Linha ").Append(i.ToString()).Append(": ").Append(erro).AppendLine();
                                continue;
                            }
                            if (cteTerceiro != null)
                            {
                                if (!pedido.CTesTerceiro.Any(o => o.Codigo == cteTerceiro.Codigo))
                                    pedido.CTesTerceiro.Add(cteTerceiro);
                            }
                        }
                        else
                            erros.Append("Linha ").Append(i.ToString()).Append(": ").Append("Expedidor não cadastrado no TMS").AppendLine();
                    }
                    else
                        pedido.PedidoSubContratado = true;
                    if (string.IsNullOrWhiteSpace(pedido.ObservacaoCTe))
                        pedido.ObservacaoCTe = "";
                    if (!string.IsNullOrWhiteSpace(chaveNFE))
                        pedido.ObservacaoCTe += " Chave NF-e " + chaveNFE;
                    if (string.IsNullOrWhiteSpace(pedido.ObservacaoCTe))
                        pedido.ObservacaoCTe = (pedido.ObservacaoCTe ?? string.Empty).Length > 1999 ? pedido.ObservacaoCTe.Substring(0, 2000) : pedido.ObservacaoCTe;

                    pedido.ImprimirObservacaoCTe = true;
                    pedido.TipoServicoCarga = TipoServicoCarga.Redespacho;

                    repPedido.Atualizar(pedido);
                    Servicos.Auditoria.Auditoria.Auditar(auditado, pedido, null, "Pedido atualizado pela importação da planilha de Feeder de subcontratação tipo um.", unidadeDeTrabalho);
                }

                unidadeDeTrabalho.CommitChanges();

                if (pedido != null)
                    pedidos.Add(pedido);
            }


            unidadeDeTrabalho.FlushAndClear();

            if (erros.Length > 0)
                return false;//new JsonpResult(false, true, erros.ToString());

            usuario = repUsuario.BuscarPorCodigo(usuario?.Codigo ?? 0);
            erro = "";
            if (CriarCarregamentosPedidos(out erro, numerosBooking, codigosPedidos, usuario, tipoServicoMultisoftware, unidadeDeTrabalho, auditado, urlAcessoCliente, montagemFeeder))
                return true;//new JsonpResult(true);
            else
            {
                erros.Append("Não foi possível gerar a carga, favor contate a equipe de suporte: ").Append(erro).AppendLine();
                return false;//new JsonpResult(false, true, erros.ToString());
            }
        }

        #endregion

        #region Métodos Privados

        private bool ObterTipoContainer(out string erro, out Dominio.Entidades.Embarcador.Pedidos.ContainerTipo containerTipo, string tipoContainer, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.ContainerTipo repContainerTipo = new Repositorio.Embarcador.Pedidos.ContainerTipo(unitOfWork);
            containerTipo = repContainerTipo.BuscarPorDescricao(tipoContainer);

            if (containerTipo == null)
            {
                Dominio.Entidades.Embarcador.Pedidos.ContainerTipo novoContainerTipo = new Dominio.Entidades.Embarcador.Pedidos.ContainerTipo()
                {
                    Descricao = tipoContainer,
                    Status = true,
                    Integrado = false
                };
                repContainerTipo.Inserir(novoContainerTipo);

                containerTipo = repContainerTipo.BuscarPorCodigo(novoContainerTipo.Codigo);
                if (containerTipo == null)
                {
                    erro = "Tipo do Container não cadastrado no sistema: " + tipoContainer + ".";
                    return false;
                }
            }

            erro = string.Empty;
            return true;
        }

        private bool ObterNotaFiscal(out string erro, out Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, string ncm, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, decimal pesoBruto, decimal pesoLiquido, double cnpjDestinatario, string numeroCE)
        {
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            //xmlNotaFiscal = repXMLNotaFiscal.BuscarPorDescricao(numeroCE, pesoBruto, pesoLiquido, cnpjDestinatario);
            xmlNotaFiscal = null;

            if (xmlNotaFiscal == null)
            {
                int.TryParse(numeroCE, out int numeroNota);
                if (numeroNota == 0)
                    int.TryParse(ncm, out numeroNota);
                xmlNotaFiscal = new Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal()
                {
                    Chave = numeroCE,
                    NCM = ncm,
                    nfAtiva = true,
                    Numero = numeroNota,
                    Peso = pesoBruto,
                    Valor = (decimal)1.00,
                    DataEmissao = DateTime.Now,
                    Destinatario = pedido.Destinatario,
                    Emitente = pedido.Remetente,
                    BaseCalculoICMS = (decimal)0,
                    ValorICMS = (decimal)0,
                    BaseCalculoST = (decimal)0,
                    ValorST = (decimal)0,
                    ValorTotalProdutos = (decimal)0,
                    ValorSeguro = (decimal)0,
                    ValorDesconto = (decimal)0,
                    ValorImpostoImportacao = (decimal)0,
                    ValorPIS = (decimal)0,
                    PesoLiquido = pesoLiquido,
                    Volumes = 0,
                    ValorCOFINS = (decimal)0,
                    ValorOutros = (decimal)0,
                    ValorIPI = (decimal)0,
                    TipoOperacaoNotaFiscal = TipoOperacaoNotaFiscal.Saida,
                    CNPJTranposrtador = pedido.Empresa?.CNPJ_SemFormato ?? "",
                    ValorFrete = (decimal)0,
                    TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.Outros,
                    Descricao = numeroCE,
                    ModalidadeFrete = ModalidadePagamentoFrete.Pago,
                    RetornoNotaIntegrada = false,
                    Empresa = pedido.Empresa,
                    CanceladaPeloEmitente = false,
                    NumeroPedido = 0,
                    QuantidadePallets = (decimal)0,
                    Altura = (decimal)0,
                    Largura = (decimal)0,
                    Comprimento = (decimal)0,
                    MetrosCubicos = (decimal)0,
                    PesoBaseParaCalculo = pesoBruto,
                    PesoCubado = (decimal)0,
                    PesoPaletizado = (decimal)0,
                    ValorFreteEmbarcador = (decimal)0,
                    SemCarga = false,
                    FatorCubagem = (decimal)0,
                    PesoPorPallet = (decimal)0,
                    KMRota = (decimal)0,
                    XML = "",
                    PlacaVeiculoNotaFiscal = "",
                    DataRecebimento = DateTime.Now
                };
                repXMLNotaFiscal.Inserir(xmlNotaFiscal);
            }

            erro = string.Empty;
            return true;
        }

        private bool ObterPedido(out string erro, out Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, string numeroBooking, string numeroContainer, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Pedidos.Container container, string portoOrigem, string portoDestino, string navioViagemDirecao, int posLinha, string nomeArquivo)
        {
            Servicos.Embarcador.Moedas.Cotacao serCotacao = new Servicos.Embarcador.Moedas.Cotacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete repCargaPedidoComponentesFrete = new Repositorio.Embarcador.Cargas.CargaPedidoComponentesFrete(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoComponenteFrete repPedidoComponenteFrete = new Repositorio.Embarcador.Pedidos.PedidoComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoViagemNavioSchedule repSchedule = new Repositorio.Embarcador.Pedidos.PedidoViagemNavioSchedule(unitOfWork);

            pedido = repPedido.BuscarPorBookingContainer(numeroBooking, numeroContainer);
            erro = "";
            if (pedido == null)
            {
                pedido = repPedido.BuscarPorBookingSemContainer(numeroBooking);
                if (pedido != null && container != null)
                {
                    pedido.Container = container;
                    repPedido.Atualizar(pedido);
                }
                else
                {
                    erro += "Booking não encontrado em nenhum pedido: " + numeroBooking + " com o container " + numeroContainer + " ou sem nenhum container associado. <br/>";
                }
            }
            //if (pedido == null)
            //{
            //    erro += "Booking não encontrado em nenhum pedido pendente de montagem. BK: " + numeroBooking + " com o container " + numeroContainer + " ou sem nenhum container associado. <br/>";
            //}

            if (pedido != null && repCargaPedido.ContemPedidoEmCargaEmAberto(pedido.Codigo))
            {
                erro += "O pedido de número " + pedido.Numero.ToString("D") + " do booking " + numeroBooking + " e do container " + numeroContainer + " já se encontra vinculado a uma carga. <br/>";
            }

            if (pedido != null && pedido.Empresa == null)
            {
                erro += "O pedido de número " + pedido.Numero.ToString("D") + " do booking " + numeroBooking + " e do container " + numeroContainer + " não possui empresa/filial selecionada. <br/>";
            }

            if (pedido != null && pedido.PedidoViagemNavio != null && !string.IsNullOrWhiteSpace(navioViagemDirecao))
            {
                if (!pedido.PedidoViagemNavio.Descricao.Equals(navioViagemDirecao, StringComparison.InvariantCultureIgnoreCase))
                {
                    erro += nomeArquivo + " - Linha " + Utilidades.String.OnlyNumbers(posLinha.ToString("n0")) + ": Navio/Viagem/Direção inconsistente com o Booking " + numeroBooking + " <br/>";
                }
            }
            if (pedido != null && pedido.Porto != null && !string.IsNullOrWhiteSpace(pedido.Porto.CodigoIATA) && !string.IsNullOrWhiteSpace(portoOrigem))
            {
                if (pedido.Porto.CodigoIATA != portoOrigem.ToUpper())
                {
                    erro += nomeArquivo + " - Linha " + Utilidades.String.OnlyNumbers(posLinha.ToString("n0")) + ": Porto de Origem inconsistente com o Booking " + numeroBooking + " <br/>";
                }
            }
            if (pedido != null && pedido.PortoDestino != null && !string.IsNullOrWhiteSpace(pedido.PortoDestino.CodigoIATA) && !string.IsNullOrWhiteSpace(portoDestino))
            {
                if (pedido.PortoDestino.CodigoIATA != portoDestino.ToUpper())
                {
                    erro += nomeArquivo + " - Linha " + Utilidades.String.OnlyNumbers(posLinha.ToString("n0")) + ": Porto de Destino inconsistente com o Booking " + numeroBooking + " <br/>";
                }
            }

            if (!string.IsNullOrWhiteSpace(erro))
                return false;

            Dominio.Entidades.Cliente tomadorPedido = pedido.ObterTomador();
            int codigoGrupoPessoa = 0;
            if (tomadorPedido != null)
                codigoGrupoPessoa = tomadorPedido.GrupoPessoas?.Codigo ?? 0;
            else
                codigoGrupoPessoa = pedido.GrupoPessoas?.Codigo ?? pedido.Remetente?.GrupoPessoas?.Codigo ?? 0;

            if (pedido.ValorTaxaFeeder == 0 && codigoGrupoPessoa > 0)
            {
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorCodigo(codigoGrupoPessoa);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.RegraCotacaoFeeder regraCotacaoFeeder = RegraCotacaoFeeder.TaxaDoDia;
                if (grupoPessoas != null && grupoPessoas.RegraCotacaoFeeder.HasValue)
                    regraCotacaoFeeder = grupoPessoas.RegraCotacaoFeeder.Value;

                DateTime dataBase = DateTime.Now;
                if (regraCotacaoFeeder == RegraCotacaoFeeder.TaxaDoDiaUtilAnterior)
                    dataBase = dataBase.AddDays(-1);
                else if (regraCotacaoFeeder == RegraCotacaoFeeder.TaxaDoDiaUtilDoETS)
                {
                    if (pedido.TerminalOrigem != null && pedido.Porto != null && pedido.PedidoViagemNavio != null)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavioSchedule schedule = repSchedule.BuscarPorViagemPortoTerminal(pedido.PedidoViagemNavio.Codigo, pedido.Porto.Codigo, pedido.TerminalOrigem.Codigo);
                        if (schedule != null && schedule.DataPrevisaoSaidaNavio.HasValue)
                            dataBase = schedule.DataPrevisaoSaidaNavio.Value;
                    }
                }
                dataBase = RetornarProximaDataValida(dataBase, unitOfWork);

                decimal valorCotacao = serCotacao.BuscarValorCotacaoCliente(Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.DolarCompra, unitOfWork, codigoGrupoPessoa, dataBase);
                if (valorCotacao == 0)
                    valorCotacao = serCotacao.BuscarValorCotacaoCliente(Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.DolarCompra, unitOfWork, 0, dataBase);

                if (valorCotacao == 0)
                {
                    erro = "Não foi localizada nenhuma taxa cambial ao grupo " + (grupoPessoas?.Descricao ?? "") + " para a data " + dataBase.ToString("dd/MM/yyyy") + " referente ao pedido: " + numeroBooking;
                    return false;
                }
                else
                {
                    pedido.ValorTaxaFeeder = valorCotacao;
                    pedido.ValorFreteNegociado = valorCotacao > 0 ? (pedido.ValorFreteNegociado * valorCotacao) : pedido.ValorFreteNegociado;
                    if (valorCotacao > 0)
                        pedido.ObservacaoCTe += " Conversão de valor em dólar: " + valorCotacao.ToString("n4") + " //";
                    if (string.IsNullOrWhiteSpace(pedido.ObservacaoCTe))
                        pedido.ObservacaoCTe = (pedido.ObservacaoCTe ?? string.Empty).Length > 1999 ? pedido.ObservacaoCTe.Substring(0, 2000) : pedido.ObservacaoCTe;
                    pedido.ImprimirObservacaoCTe = true;
                    pedido.TipoServicoCarga = TipoServicoCarga.Redespacho;
                    repPedido.Atualizar(pedido);

                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> cargaPedidoComponentesFretes = repCargaPedidoComponentesFrete.BuscarPorPedido(pedido.Codigo);
                    foreach (var componente in cargaPedidoComponentesFretes)
                    {
                        componente.ValorComponente = valorCotacao > 0 ? (componente.ValorComponente * valorCotacao) : componente.ValorComponente;
                        repCargaPedidoComponentesFrete.Atualizar(componente);
                    }
                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoComponenteFrete> pedidoComponenteFretes = repPedidoComponenteFrete.BuscarPorPedido(pedido.Codigo);
                    foreach (var componente in pedidoComponenteFretes)
                    {
                        componente.ValorComponente = valorCotacao > 0 ? (componente.ValorComponente * valorCotacao) : componente.ValorComponente;
                        repPedidoComponenteFrete.Atualizar(componente);
                    }
                }
            }

            erro = string.Empty;
            return true;
        }

        private bool ObterContainer(out string erro, out Dominio.Entidades.Embarcador.Pedidos.Container container, string numeroContainer, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Pedidos.ContainerTipo containerTipo, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repConfiguracaoPedido.BuscarConfiguracaoPadrao();

            Servicos.Embarcador.Pedido.Pedido serPedido = new Servicos.Embarcador.Pedido.Pedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.Container repContainer = new Repositorio.Embarcador.Pedidos.Container(unitOfWork);
            container = repContainer.BuscarPorNumero(numeroContainer.Trim());
            if (!string.IsNullOrWhiteSpace(numeroContainer))
                numeroContainer = numeroContainer.Replace(" ", "").Replace("-", "").Replace(".", "").Replace("/", "").Replace("\\", "").Trim();

            if (container == null)
            {
                Dominio.Entidades.Embarcador.Pedidos.Container novoContainer = new Dominio.Entidades.Embarcador.Pedidos.Container()
                {
                    ContainerTipo = containerTipo,
                    Descricao = numeroContainer,
                    Numero = Utilidades.String.SanitizeString(numeroContainer),
                    Status = true,
                    TipoPropriedade = serPedido.ValidarDigitoContainerNumero(numeroContainer) ? TipoPropriedadeContainer.Proprio : TipoPropriedadeContainer.Soc,
                    DataUltimaAtualizacao = DateTime.Now,
                    Integrado = false
                };
                repContainer.Inserir(novoContainer, auditado);
                container = repContainer.BuscarPorCodigo(novoContainer.Codigo);

                if (containerTipo == null)
                {
                    erro = "Container não cadastrado no sistema: " + numeroContainer + ".";
                    return false;
                }
                if ((configuracaoPedido?.ValidarCadastroContainerPelaFormulaGlobal ?? false) && !string.IsNullOrWhiteSpace(container.Numero))
                {
                    if (!serPedido.ValidarDigitoContainerNumero(container.Numero))
                    {
                        erro = "Número do container " + numeroContainer + " está inválido de acordo com o seu dígito verificado.";
                        return false;
                    }
                }
            }

            erro = string.Empty;
            return true;
        }

        private string RemoveExtraText(string value)
        {
            var allowedChars = "01234567890.,";
            return new string(value.Where(c => allowedChars.Contains(c)).ToArray());
        }

        private bool ObterProdutoEmbarcador(out string erro, out Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador, string descricao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);
            produtoEmbarcador = repProdutoEmbarcador.BuscarPorDescricao(descricao);

            if (produtoEmbarcador == null)
            {
                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcadorNovo = new Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador()
                {
                    CodigoProdutoEmbarcador = descricao,
                    Descricao = descricao,
                    Ativo = true,
                    Integrado = false
                };
                repProdutoEmbarcador.Inserir(produtoEmbarcadorNovo);

                produtoEmbarcador = repProdutoEmbarcador.BuscarPorDescricao(descricao);
                if (produtoEmbarcador == null)
                {
                    erro = "Não foi possível cadastrar Produto/Mercadoria no sistema: " + descricao + ". Favor cadastre o mesmo manualmente antes da importação.";
                    return false;
                }
            }
            erro = string.Empty;
            return true;
        }

        private bool ObterDocumentoAnterior(out string erro, out Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro, string numeroReferenciaNota, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, decimal pesoBruto, decimal pesoLiquido, double cnpjDestinatario, string numeroCE, string produto, Dominio.Entidades.Cliente destinatario, Dominio.Entidades.Cliente expedidor, string chaveCTeAnterior, string chaveNFeImportado, string ncm, Dominio.Entidades.Cliente tomador, Dominio.Entidades.Cliente recebedor, bool importarMesmoSemCTeAbsorvidoAnteriormente, bool importarMesmoComDocumentacaoDuplicada, bool buscarTomadorCTeAnterior)
        {
            Servicos.Cliente servicoCliente = new Servicos.Cliente(unitOfWork.StringConexao);
            Servicos.Embarcador.Carga.CTeSubContratacao servicoCTeSubContratacao = new Servicos.Embarcador.Carga.CTeSubContratacao(unitOfWork);
            Servicos.Embarcador.CTe.CTe serCTe = new Servicos.Embarcador.CTe.CTe(unitOfWork);

            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiro repCTeTerceiro = new Repositorio.Embarcador.CTe.CTeTerceiro(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiroOutrosDocumentos repCTeTerceiroOutrosDocumentos = new Repositorio.Embarcador.CTe.CTeTerceiroOutrosDocumentos(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiroNFe repCTeTerceiroNFe = new Repositorio.Embarcador.CTe.CTeTerceiroNFe(unitOfWork);
            Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiroQuantidade repCTeTerceiroQuantidade = new Repositorio.Embarcador.CTe.CTeTerceiroQuantidade(unitOfWork);

            string[] chavesNFes = null;
            if (!string.IsNullOrWhiteSpace(chaveNFeImportado) && chaveNFeImportado.Contains(","))
                chavesNFes = chaveNFeImportado.Split(',');
            if (!string.IsNullOrWhiteSpace(chaveNFeImportado) && chaveNFeImportado.Contains(";"))
                chavesNFes = chaveNFeImportado.Split(';');

            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            if (destinatario != null && destinatario.CPF_CNPJ > 0)
                destinatario = repCliente.BuscarPorCPFCNPJ(destinatario.CPF_CNPJ);
            if (expedidor != null && expedidor.CPF_CNPJ > 0)
                expedidor = repCliente.BuscarPorCPFCNPJ(expedidor.CPF_CNPJ);
            if (tomador != null && tomador.CPF_CNPJ > 0)
                tomador = repCliente.BuscarPorCPFCNPJ(tomador.CPF_CNPJ);
            if (recebedor != null && recebedor.CPF_CNPJ > 0)
                recebedor = repCliente.BuscarPorCPFCNPJ(recebedor.CPF_CNPJ);

            Dominio.Entidades.Cliente emitenteDocumentoAnterior = null;
            if (!string.IsNullOrWhiteSpace(chaveCTeAnterior) && chaveCTeAnterior.Length == 44)
            {
                string cnpjChaveAnterior = chaveCTeAnterior.Substring(6, 14);
                double.TryParse(cnpjChaveAnterior, out double cnpjEmitenteDocumentoAnterior);
                if (cnpjEmitenteDocumentoAnterior > 0)
                {
                    emitenteDocumentoAnterior = repCliente.BuscarPorCPFCNPJ(cnpjEmitenteDocumentoAnterior);
                    if (emitenteDocumentoAnterior == null)
                    {
                        erro = "O emitente do documento anterior de CNPJ " + cnpjEmitenteDocumentoAnterior + " não se encontra cadastrado no TMS. ";
                        cteTerceiro = null;
                        return false;
                    }
                }
                else
                {
                    erro = "Não foi possível localizar o CNPJ do emitente do documento anterior na chave do CT-e anterior. ";
                    cteTerceiro = null;
                    return false;
                }
            }
            else if (string.IsNullOrWhiteSpace(chaveCTeAnterior))
            {
                emitenteDocumentoAnterior = expedidor;
            }

            int.TryParse(numeroCE, out int numeroNota);
            if (numeroNota == 0)
                int.TryParse(numeroReferenciaNota, out numeroNota);

            string retorno = "";

            if (!string.IsNullOrWhiteSpace(chaveCTeAnterior))
            {
                try
                {
                    cteTerceiro = repCTeTerceiro.BuscarPorChave(chaveCTeAnterior, true);
                    if (cteTerceiro != null)
                    {
                        SalvarNCMDocumentoAnterior(cteTerceiro, ncm, unitOfWork);
                        AlterarTipoNotaDocumentoAnterior(cteTerceiro, ncm, numeroCE, unitOfWork);

                        if (buscarTomadorCTeAnterior)
                        {
                            if (cteTerceiro.Emitente != null)
                            {
                                pedido.Tomador = cteTerceiro.Emitente.Cliente;
                                pedido.UsarTipoTomadorPedido = true;
                                pedido.TipoTomador = Dominio.Enumeradores.TipoTomador.Outros;
                                pedido.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Outros;

                                repPedido.Atualizar(pedido);
                            }
                        }
                        erro = "";
                        return true;

                    }
                    else
                    {
                        cteTerceiro = repCTeTerceiro.BuscarPorChave(chaveCTeAnterior, false);
                        if (cteTerceiro != null)
                        {
                            cteTerceiro.Ativo = true;
                            repCTeTerceiro.Atualizar(cteTerceiro);
                            SalvarNCMDocumentoAnterior(cteTerceiro, ncm, unitOfWork);
                            AlterarTipoNotaDocumentoAnterior(cteTerceiro, ncm, numeroCE, unitOfWork);

                            if (buscarTomadorCTeAnterior)
                            {
                                if (cteTerceiro.Emitente != null)
                                {
                                    pedido.Tomador = cteTerceiro.Emitente.Cliente;
                                    pedido.UsarTipoTomadorPedido = true;
                                    pedido.TipoTomador = Dominio.Enumeradores.TipoTomador.Outros;
                                    pedido.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Outros;

                                    repPedido.Atualizar(pedido);
                                }

                            }
                            erro = "";
                            return true;
                        }
                    }


                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro("Falha ao converter CT-e para CTe Terceiro: " + ex.Message);
                }
                try
                {
                    Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinado = repDocumentoDestinadoEmpresa.BuscarPorChave(chaveCTeAnterior);
                    if (documentoDestinado != null)
                    {
                        string caminho = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;

                        caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "CTe", documentoDestinado.Empresa.CNPJ, documentoDestinado.Chave + ".xml");
                        if (Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                        {
                            using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminho)))
                            {
                                dynamic dynCTe = MultiSoftware.CTe.Servicos.Leitura.Ler(memoryStream);
                                if (dynCTe != null)
                                {
                                    Dominio.ObjetosDeValor.Embarcador.CTe.CTe cteIntegracao = serCTe.ConverterProcCTeParaCTe(dynCTe);
                                    cteTerceiro = servicoCTeSubContratacao.CriarCTeTerceiro(unitOfWork, ref retorno, null, cteIntegracao);
                                    if (cteTerceiro != null)
                                    {
                                        SalvarNCMDocumentoAnterior(cteTerceiro, ncm, unitOfWork);
                                        AlterarTipoNotaDocumentoAnterior(cteTerceiro, ncm, numeroCE, unitOfWork);

                                        if (buscarTomadorCTeAnterior)
                                        {
                                            if (cteTerceiro.Emitente != null)
                                            {
                                                pedido.Tomador = cteTerceiro.Emitente.Cliente;
                                                pedido.UsarTipoTomadorPedido = true;
                                                pedido.TipoTomador = Dominio.Enumeradores.TipoTomador.Outros;
                                                pedido.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Outros;

                                                repPedido.Atualizar(pedido);
                                            }
                                        }

                                        erro = "";
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro("Falha ao converter CT-e para CTe Terceiro: " + ex.Message);
                }
            }

            if (!string.IsNullOrWhiteSpace(chaveCTeAnterior) && chaveCTeAnterior.Length == 44 && !importarMesmoSemCTeAbsorvidoAnteriormente)
            {
                erro = "Não foi encontrado nenhum CT-e importado anterirmente com a chave " + chaveCTeAnterior + ". Favor importe o mesmo por e-mail para importar a planilha.";
                cteTerceiro = null;
                return false;
            }

            if (!string.IsNullOrWhiteSpace(chaveCTeAnterior) && chaveCTeAnterior.Length > 44)
            {
                erro = "A chave do CT-e anterior está inválida: " + chaveCTeAnterior + ".";
                cteTerceiro = null;
                return false;
            }

            cteTerceiro = new Dominio.Entidades.Embarcador.CTe.CTeTerceiro()
            {
                AliquotaICMS = 0,
                Ativo = true,
                BaseCalculoICMS = 0,
                CFOP = null,//repCFOP.BuscarPorNumero(5933),
                ChaveAcesso = !string.IsNullOrWhiteSpace(chaveCTeAnterior) ? chaveCTeAnterior : numeroCE,
                ChaveCTEReferenciado = !string.IsNullOrWhiteSpace(chaveCTeAnterior) ? chaveCTeAnterior : numeroCE,
                CST = "00",
                CTesTerceiroNFes = null,
                CTesTerceiroNotasFiscais = null,
                CTesTerceiroOutrosDocumentos = null,
                CTeTerceiroQuantidades = null,
                CTeTerceiroSeguros = null,
                DataEmissao = DateTime.Now,
                Destinatario = servicoCliente.ConverterClienteParaParticipanteCTe(destinatario),
                Emitente = emitenteDocumentoAnterior != null ? servicoCliente.ConverterClienteParaParticipanteCTe(emitenteDocumentoAnterior) : tomador != null ? servicoCliente.ConverterClienteParaParticipanteCTe(tomador) : servicoCliente.ConverterClienteParaParticipanteCTe(expedidor),
                Expedidor = null,
                InformacaoAdicionalContribuinte = "",
                InformacaoAdicionalFisco = "",
                LocalidadeInicioPrestacao = expedidor != null ? expedidor.Localidade : emitenteDocumentoAnterior != null ? emitenteDocumentoAnterior.Localidade : tomador.Localidade,
                LocalidadeTerminoPrestacao = recebedor != null ? recebedor.Localidade : destinatario.Localidade,
                Lotacao = false,
                Modal = TipoModal.Aquaviario,
                Numero = numeroNota,
                NumeroMinuta = null,
                NumeroOperacionalConhecimentoAereo = null,
                NumeroPedido = numeroCE,
                NumeroRomaneio = numeroCE,
                OutrasCaracteristicasDaCarga = "",
                OutrosTomador = null,
                PercentualReducaoBaseCalculoICMS = 0,
                Peso = pesoBruto,
                ProdutoPredominante = !string.IsNullOrWhiteSpace(produto) && produto.Length > 60 ? produto.Substring(0, 59) : produto,
                ProtocoloCliente = "",
                Recebedor = null,
                Remetente = tomador != null && !string.IsNullOrWhiteSpace(chaveCTeAnterior) ? servicoCliente.ConverterClienteParaParticipanteCTe(tomador) : servicoCliente.ConverterClienteParaParticipanteCTe(expedidor),
                Serie = "1",
                SimplesNacional = false,
                TipoCTE = Dominio.Enumeradores.TipoCTE.Normal,
                TipoPagamento = Dominio.Enumeradores.TipoPagamento.A_Pagar,
                TipoServico = Dominio.Enumeradores.TipoServico.Normal,
                TipoTomador = Dominio.Enumeradores.TipoTomador.Remetente,
                TransportadorTerceiro = emitenteDocumentoAnterior != null ? emitenteDocumentoAnterior : tomador != null ? tomador : expedidor,
                ValorAReceber = pedido.ValorFreteNegociado,
                ValorICMS = 0,
                ValorPrestacaoServico = pedido.ValorFreteNegociado,
                ValorTotalMercadoria = 1,
                Versao = "1.0",
                DescricaoItemPeso = ""
            };

            Servicos.Embarcador.Carga.ICMS serICMS = new Servicos.Embarcador.Carga.ICMS(unitOfWork);
            Dominio.Entidades.Aliquota aliquota = serICMS.ObterAliquota(cteTerceiro.Emitente.Localidade.Estado, cteTerceiro.LocalidadeInicioPrestacao.Estado, cteTerceiro.LocalidadeTerminoPrestacao.Estado, cteTerceiro.Tomador.Atividade, cteTerceiro.Destinatario.Atividade, unitOfWork);
            if (aliquota != null)
                cteTerceiro.CFOP = aliquota.CFOP;

            repCTeTerceiro.Inserir(cteTerceiro);

            Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade cTeTerceiroQuantidade = new Dominio.Entidades.Embarcador.CTe.CTeTerceiroQuantidade()
            {
                CTeTerceiro = cteTerceiro,
                Quantidade = pesoBruto > 0 ? pesoBruto : 1,
                TipoMedida = "PESO BRUTO",
                Unidade = Dominio.Enumeradores.UnidadeMedida.KG
            };
            repCTeTerceiroQuantidade.Inserir(cTeTerceiroQuantidade);

            if (chavesNFes != null)
            {
                foreach (var chaveNFeImportada in chavesNFes)
                {
                    InserirDocumentosAnteriores(chaveNFeImportada, cteTerceiro, pesoBruto, ncm, numeroCE, unitOfWork);
                }
            }
            else
                InserirDocumentosAnteriores(chaveNFeImportado, cteTerceiro, pesoBruto, ncm, numeroCE, unitOfWork);

            erro = string.Empty;
            return true;
        }

        private void AlterarTipoNotaDocumentoAnterior(Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro, string ncm, string numeroCE, Repositorio.UnitOfWork unitOfWork)
        {
            if (cteTerceiro == null || cteTerceiro.Codigo == 0)
                return;

            Repositorio.Embarcador.CTe.CTeTerceiroNFe repCTeTerceiroNFe = new Repositorio.Embarcador.CTe.CTeTerceiroNFe(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiroNotaFiscal repCTeTerceiroNotaFiscal = new Repositorio.Embarcador.CTe.CTeTerceiroNotaFiscal(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiroOutrosDocumentos repCTeTerceiroOutrosDocumentos = new Repositorio.Embarcador.CTe.CTeTerceiroOutrosDocumentos(unitOfWork);
            try
            {
                List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroNFe> notas = repCTeTerceiroNFe.BuscarPorCTeTerceiro(cteTerceiro.Codigo);
                if (notas != null)
                {
                    foreach (var nota in notas)
                    {
                        string chaveNFe = nota.Chave.Trim();
                        decimal valorNota = nota.ValorTotal;

                        if (!string.IsNullOrWhiteSpace(chaveNFe))
                        {
                            Dominio.Entidades.Embarcador.CTe.CTeTerceiroOutrosDocumentos outroDocumento = new Dominio.Entidades.Embarcador.CTe.CTeTerceiroOutrosDocumentos()
                            {
                                CTeTerceiro = cteTerceiro,
                                Descricao = chaveNFe.Length >= 40 ? chaveNFe.Substring(25, 9) : chaveNFe,
                                Numero = chaveNFe.Length >= 40 ? chaveNFe.Substring(25, 9) : chaveNFe.Length > 10 ? chaveNFe.Substring(0, 9) : chaveNFe,
                                Tipo = TipoOutroDocumento.Declaracao,
                                Valor = valorNota,
                                NCM = ncm
                            };
                            repCTeTerceiroOutrosDocumentos.Inserir(outroDocumento);
                            repCTeTerceiroNFe.Deletar(nota);
                        }
                        else if (!string.IsNullOrWhiteSpace(numeroCE))
                        {
                            Dominio.Entidades.Embarcador.CTe.CTeTerceiroOutrosDocumentos outroDocumento = new Dominio.Entidades.Embarcador.CTe.CTeTerceiroOutrosDocumentos()
                            {
                                CTeTerceiro = cteTerceiro,
                                Descricao = numeroCE,
                                Numero = !string.IsNullOrWhiteSpace(numeroCE) && numeroCE.Length > 10 ? numeroCE.Substring(0, 9) : numeroCE,
                                Tipo = TipoOutroDocumento.Declaracao,
                                Valor = valorNota,
                                NCM = ncm
                            };
                            repCTeTerceiroOutrosDocumentos.Inserir(outroDocumento);
                            repCTeTerceiroNFe.Deletar(nota);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Falha ao alterar nfe para outro documento no documento anterior: " + ex.Message);
            }
        }

        private void SalvarNCMDocumentoAnterior(Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro, string ncm, Repositorio.UnitOfWork unitOfWork)
        {
            if (string.IsNullOrWhiteSpace(ncm) || cteTerceiro == null || cteTerceiro.Codigo == 0)
                return;

            Repositorio.Embarcador.CTe.CTeTerceiroNFe repCTeTerceiroNFe = new Repositorio.Embarcador.CTe.CTeTerceiroNFe(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiroNotaFiscal repCTeTerceiroNotaFiscal = new Repositorio.Embarcador.CTe.CTeTerceiroNotaFiscal(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiroOutrosDocumentos repCTeTerceiroOutrosDocumentos = new Repositorio.Embarcador.CTe.CTeTerceiroOutrosDocumentos(unitOfWork);
            try
            {
                List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroNFe> notas = repCTeTerceiroNFe.BuscarPorCTeTerceiro(cteTerceiro.Codigo);
                if (notas != null)
                {
                    foreach (var nota in notas)
                    {
                        nota.NCM = ncm;
                        repCTeTerceiroNFe.Atualizar(nota);
                    }
                }

                List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroOutrosDocumentos> outrosDocumentos = repCTeTerceiroOutrosDocumentos.BuscarPorCTeParaSubContratacao(cteTerceiro.Codigo);
                if (outrosDocumentos != null)
                {
                    foreach (var nota in outrosDocumentos)
                    {
                        nota.NCM = ncm;
                        repCTeTerceiroOutrosDocumentos.Atualizar(nota);
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Falha ao salvar NCM documento anterior: " + ex.Message);
            }
        }

        private void InserirDocumentosAnteriores(string chaveNFeImportada, Dominio.Entidades.Embarcador.CTe.CTeTerceiro cteTerceiro, decimal pesoBruto, string ncm, string numeroCE, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.CTe.CTeTerceiroOutrosDocumentos repCTeTerceiroOutrosDocumentos = new Repositorio.Embarcador.CTe.CTeTerceiroOutrosDocumentos(unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiroNFe repCTeTerceiroNFe = new Repositorio.Embarcador.CTe.CTeTerceiroNFe(unitOfWork);
            string chaveNFe = chaveNFeImportada.Trim();
            if (!string.IsNullOrWhiteSpace(chaveNFe))
            {
                Dominio.Entidades.Embarcador.CTe.CTeTerceiroOutrosDocumentos outroDocumento = new Dominio.Entidades.Embarcador.CTe.CTeTerceiroOutrosDocumentos()
                {
                    CTeTerceiro = cteTerceiro,
                    Descricao = chaveNFe,
                    Numero = chaveNFe.Length > 10 ? chaveNFe.Substring(0, 9) : chaveNFe,
                    Tipo = TipoOutroDocumento.Declaracao,
                    Valor = 0,
                    NCM = ncm
                };
                repCTeTerceiroOutrosDocumentos.Inserir(outroDocumento);
            }
            else
            {
                Dominio.Entidades.Embarcador.CTe.CTeTerceiroOutrosDocumentos outroDocumento = new Dominio.Entidades.Embarcador.CTe.CTeTerceiroOutrosDocumentos()
                {
                    CTeTerceiro = cteTerceiro,
                    Descricao = numeroCE,
                    Numero = !string.IsNullOrWhiteSpace(numeroCE) && numeroCE.Length > 10 ? numeroCE.Substring(0, 9) : numeroCE,
                    Tipo = TipoOutroDocumento.Declaracao,
                    Valor = 0,
                    NCM = ncm
                };
                repCTeTerceiroOutrosDocumentos.Inserir(outroDocumento);
            }
        }

        private DateTime RetornarProximaDataValida(DateTime dataMovimentacao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (dataMovimentacao.DayOfWeek == DayOfWeek.Saturday)
                dataMovimentacao = dataMovimentacao.AddDays(-1);
            else if (dataMovimentacao.DayOfWeek == DayOfWeek.Sunday)
                dataMovimentacao = dataMovimentacao.AddDays(-2);

            bool dataValida = true;
            Configuracoes.Feriado servicoFeriado = new Configuracoes.Feriado(unidadeDeTrabalho);

            while (dataValida)
            {
                if (dataMovimentacao.DayOfWeek == DayOfWeek.Saturday || dataMovimentacao.DayOfWeek == DayOfWeek.Sunday)
                    dataMovimentacao = dataMovimentacao.AddDays(-1);
                else if (servicoFeriado.VerificarSePossuiFeriado(dataMovimentacao))
                    dataMovimentacao = dataMovimentacao.AddDays(-1);
                else
                    dataValida = false;
            }

            return dataMovimentacao;
        }

        private bool CriarCarregamentosPedidos(out string erro, List<string> numerosBooking, List<int> codigosPedidos, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, string urlAcessoCliente, Dominio.Entidades.Embarcador.Cargas.MontagemFeeder.MontagemFeeder montagemFeeder = null)
        {
            erro = "";

            try
            {
                List<int> codigosCarregamento = new List<int>();
                Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoViagemNavio repPedidoViagemNavio = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(unitOfWork);
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao repCarregamentoRoteirizacao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
                Repositorio.Embarcador.PreCargas.PreCarga repositorioPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

                foreach (var numeroBooking in numerosBooking)
                {
                    unitOfWork.FlushAndClear();

                    List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repPedido.BuscarPorBookingCodigos(numeroBooking, codigosPedidos);

                    if (pedidos == null || pedidos.Count == 0)
                    {
                        erro = "Não foi gerado nenhum pedido com o Booking " + numeroBooking;
                        return false;
                    }
                    Dominio.Entidades.Embarcador.Pedidos.Pedido primeiroPedido = pedidos.FirstOrDefault();

                    unitOfWork.Start();
                    int pedidoViagemNavio = primeiroPedido.PedidoViagemNavio?.Codigo ?? 0;
                    int modeloVeicular = primeiroPedido.ModeloVeicularCarga?.Codigo ?? 0;
                    int codigoPreCarga = 0;
                    double recebedor = primeiroPedido.Recebedor?.CPF_CNPJ ?? (double)0;

                    Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento();
                    carregamento.TipoMontagemCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMontagemCarga.NovaCarga;
                    carregamento.DataCriacao = DateTime.Now;
                    if (carregamento.TipoMontagemCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMontagemCarga.NovaCarga)
                    {
                        carregamento.AutoSequenciaNumero = new Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga(unitOfWork).ObterProximoCodigoCarregamento();
                        carregamento.NumeroCarregamento = carregamento.AutoSequenciaNumero.ToString();
                    }
                    carregamento.SituacaoCarregamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.EmMontagem;
                    carregamento.DataCarregamentoCarga = primeiroPedido.DataInicialColeta.HasValue ? primeiroPedido.DataInicialColeta.Value : DateTime.Now.Date;
                    carregamento.DataDescarregamentoCarga = primeiroPedido.DataFinalColeta.HasValue ? primeiroPedido.DataFinalColeta.Value : DateTime.Now.Date;
                    if (recebedor > 0)
                        carregamento.Recebedor = repCliente.BuscarPorCPFCNPJ(recebedor);
                    carregamento.ModeloVeicularCarga = repModeloVeicularCarga.BuscarPorCodigo(modeloVeicular);
                    carregamento.PedidoViagemNavio = repPedidoViagemNavio.BuscarPorCodigo(pedidoViagemNavio);
                    carregamento.CarregamentoColeta = false;
                    if ((codigoPreCarga > 0) && (carregamento.TipoMontagemCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMontagemCarga.NovaCarga))
                        carregamento.PreCarga = repositorioPreCarga.BuscarPorCodigo(codigoPreCarga);
                    else
                        carregamento.PreCarga = null;

                    Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamentoValidacao = repCarregamento.BuscarPorPreCarga(carregamento.PreCarga?.Codigo ?? 0, carregamento.Codigo);
                    int empresa = primeiroPedido.Empresa?.Codigo ?? 0;
                    if (empresa > 0)
                        carregamento.Empresa = repEmpresa.BuscarPorCodigo(empresa);

                    if (carregamento.Empresa == null && configuracaoEmbarcador.TransportadorObrigatorioMontagemCarga)
                    {
                        erro = "Transportador é obrigatório.";
                        return false;
                    }

                    carregamento.Veiculo = null;
                    carregamento.CarregamentoRedespacho = false;
                    int tipoCarga = primeiroPedido.TipoDeCarga?.Codigo ?? 0;
                    if (tipoCarga > 0)
                        carregamento.TipoDeCarga = repTipoDeCarga.BuscarPorCodigo(tipoCarga);
                    else
                        carregamento.TipoDeCarga = null;

                    if (carregamento.TipoDeCarga == null && configuracaoEmbarcador.TipoCargaObrigatorioMontagemCarga)
                    {
                        erro = "Tipo de Carga é obrigatório.";
                        return false;
                    }

                    int tipoOperacao = primeiroPedido.TipoOperacao?.Codigo ?? 0;
                    if (tipoOperacao > 0)
                        carregamento.TipoOperacao = repTipoOperacao.BuscarPorCodigo(tipoOperacao);
                    else
                        carregamento.TipoOperacao = null;

                    if (carregamento.TipoOperacao == null && configuracaoEmbarcador.TipoOperacaoObrigatorioMontagemCarga)
                    {
                        erro = "Tipo de Operação é obrigatório.";
                        return false;
                    }

                    repCarregamento.Inserir(carregamento, auditado);
                    SalvarPedidos(carregamento, pedidos, unitOfWork, auditado);

                    carregamento.PesoCarregamento = pedidos.Sum(o => o.PesoTotal);
                    carregamento.VeiculoBloqueado = false;
                    carregamento.ImportadaComDocumentacaoDuplicadaMontagemFeeder = montagemFeeder?.ImportarMesmoComDocumentacaoDuplicada ?? false;

                    Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao carregamentoRoteirizacao = repCarregamentoRoteirizacao.BuscarPorCarregamento(carregamento.Codigo);

                    if (carregamentoRoteirizacao != null)
                    {
                        Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota repCarregamentoRoteirizacaoClientesRota = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota(unitOfWork);
                        List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota> listaCarregamentoRoteirizacaoClienteRota = repCarregamentoRoteirizacaoClientesRota.BuscarPorCarregamentoRoteirizacao(carregamentoRoteirizacao.Codigo);

                        foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoClientesRota carregamentoRoteirizacaoClienteRota in listaCarregamentoRoteirizacaoClienteRota)
                            repCarregamentoRoteirizacaoClientesRota.Deletar(carregamentoRoteirizacaoClienteRota);

                        Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoPontosPassagem repCarregamentoRoteirizacaoPontosPassagem = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoPontosPassagem(unitOfWork);
                        List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoPontosPassagem> listaCarregamentoRoteirizacaoPontosPassagem = repCarregamentoRoteirizacaoPontosPassagem.BuscarPorCarregamentoRoteirizacao(carregamentoRoteirizacao.Codigo);

                        foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacaoPontosPassagem carregamentoRoteirizacaoPontosPassagem in listaCarregamentoRoteirizacaoPontosPassagem)
                            repCarregamentoRoteirizacaoPontosPassagem.Deletar(carregamentoRoteirizacaoPontosPassagem);

                        repCarregamentoRoteirizacao.Deletar(carregamentoRoteirizacao);
                    }
                    repCarregamento.Atualizar(carregamento);
                    codigosCarregamento.Add(carregamento.Codigo);

                    unitOfWork.CommitChanges();
                }

                GerarCarregamentoCargaEmLote(codigosCarregamento, configuracaoEmbarcador, usuario, tipoServicoMultisoftware, unitOfWork, auditado, urlAcessoCliente);

                return true;
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();

                erro = excecao.Message;
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);

                erro = "Ocorreu uma falha ao gerar os carregamentos por pedidos";
            }

            return false;
        }

        private void SalvarPedidos(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            List<int> codigosPedidos = (from pedido in pedidos select pedido.Codigo).ToList();
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoMontagemCarga repositorioConfiguracaoMontagemCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMontagemCarga(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidosExiste = carregamento.Pedidos != null ? carregamento.Pedidos.ToList() : new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>();

            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido carregamentoPorPedidos = repCarregamentoPedido.BuscarOutroCarregamentoPorPedidos(codigosPedidos, carregamento.CarregamentoRedespacho, carregamento.Codigo, carregamento.CarregamentoColeta);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMontagemCarga configuracaoMontagemCarga = repositorioConfiguracaoMontagemCarga.BuscarPrimeiroRegistro();

            if (carregamentoPorPedidos != null)
            {
                repCarregamentoPedido.DeletarCarregamentoPedidoPendente(carregamentoPorPedidos.Carregamento.Codigo);
                repCarregamentoPedido.DeletarCarregamentoPendente(carregamentoPorPedidos.Carregamento.Codigo);
                //throw new ServicoException($"O pedido {carregamentoPorPedidos.Pedido.NumeroPedidoEmbarcador} já esta sendo utilizado no carregamento {carregamentoPorPedidos.Carregamento.NumeroCarregamento}");
            }

            carregamentoPorPedidos = repCarregamentoPedido.BuscarOutroCarregamentoPorPedidos(codigosPedidos, carregamento.CarregamentoRedespacho, carregamento.Codigo, carregamento.CarregamentoColeta);
            if (carregamentoPorPedidos != null)
                throw new ServicoException($"O pedido {carregamentoPorPedidos.Pedido.Numero} já esta sendo utilizado no carregamento {carregamentoPorPedidos.Carregamento.NumeroCarregamento}, realize o seu fechamento/cancelamento pela tela de montagem manual.");

            List<Dominio.Entidades.Embarcador.Filiais.Filial> filiais = new List<Dominio.Entidades.Embarcador.Filiais.Filial>();
            List<Dominio.Entidades.Cliente> destinatarios = new List<Dominio.Entidades.Cliente>();
            bool exigirDefinicaoReboquePedido = (carregamento.ModeloVeicularCarga?.ExigirDefinicaoReboquePedido ?? false) && (carregamento.ModeloVeicularCarga?.NumeroReboques > 1);

            foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
            {
                int codigoPedido = pedido.Codigo;
                NumeroReboque numeroReboque = NumeroReboque.SemReboque;
                TipoCarregamentoPedido tipoCarregamentoPedido = TipoCarregamentoPedido.Normal;
                DateTime? dataPrevisaoEntrega = pedido.DataPrevisaoChegadaDestinatario;

                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido carregamentoPedido = (from obj in carregamentoPedidosExiste where obj.Pedido.Codigo == codigoPedido select obj).FirstOrDefault();

                if (carregamentoPedido == null)
                {
                    carregamentoPedido = new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido
                    {
                        Carregamento = carregamento,
                        Pedido = repPedido.BuscarPorCodigo(codigoPedido),
                        NumeroReboque = numeroReboque,
                        TipoCarregamentoPedido = tipoCarregamentoPedido
                    };

                    repCarregamentoPedido.Inserir(carregamentoPedido);

                    bool precisaFronteira = (carregamentoPedido.Pedido.Remetente != null && carregamentoPedido.Pedido.Remetente.Tipo == "E")
                            || (carregamentoPedido.Pedido.Destinatario != null && carregamentoPedido.Pedido.Destinatario.Tipo == "E" && carregamentoPedido.Pedido.Recebedor == null);
                    if (
                       precisaFronteira
                       && !configuracaoEmbarcador.UtilizaEmissaoMultimodal
                       && (carregamento.Fronteiras == null
                       || carregamento.Fronteiras.Count == 0)
                       && configuracaoEmbarcador.FronteiraObrigatoriaMontagemCarga
                    )
                    {
                        throw new ServicoException("É obrigatório informar a fronteira no carregamento");
                    }
                }
                else
                {
                    carregamentoPedido.Initialize();
                    carregamentoPedido.NumeroReboque = numeroReboque;
                    carregamentoPedido.TipoCarregamentoPedido = tipoCarregamentoPedido;

                    repCarregamentoPedido.Atualizar(carregamentoPedido);
                }

                if (exigirDefinicaoReboquePedido && (numeroReboque == NumeroReboque.SemReboque))
                    throw new ServicoException($"O reboque não foi definido para o pedido {carregamentoPedido.Pedido.NumeroPedidoEmbarcador}");

                if (dataPrevisaoEntrega.HasValue && carregamentoPedido.Pedido.PrevisaoEntrega.HasValue && (dataPrevisaoEntrega.Value.ToString("dd/MM/yyyy HH:mm") != carregamentoPedido.Pedido.PrevisaoEntrega.Value.ToString("dd/MM/yyyy HH:mm")))
                {
                    carregamentoPedido.Pedido.Initialize();
                    carregamentoPedido.Pedido.PrevisaoEntrega = dataPrevisaoEntrega;

                    repPedido.Atualizar(carregamentoPedido.Pedido, auditado);
                }

                if (carregamentoPedido.Pedido.Filial != null)
                {
                    if (!filiais.Contains(carregamentoPedido.Pedido.Filial))
                        filiais.Add(carregamentoPedido.Pedido.Filial);
                }

                if (carregamentoPedido.Pedido.Destinatario != null)
                {
                    if (carregamentoPedido.Pedido.Recebedor != null && !carregamento.CarregamentoRedespacho)
                    {
                        if (!destinatarios.Contains(carregamentoPedido.Pedido.Recebedor))
                            destinatarios.Add(carregamentoPedido.Pedido.Recebedor);
                    }
                    else
                    {
                        if (!destinatarios.Contains(carregamentoPedido.Pedido.Destinatario))
                            destinatarios.Add(carregamentoPedido.Pedido.Destinatario);
                    }

                }
            }

            if ((carregamento.TipoMontagemCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMontagemCarga.NovaCarga) && (carregamento.PreCarga == null))
            {
                bool exigirPreCargaMontagemCarga = (from filial in filiais where filial.ExigirPreCargaMontagemCarga select filial.ExigirPreCargaMontagemCarga).FirstOrDefault();

                if (exigirPreCargaMontagemCarga)
                    throw new ServicoException("A pré carga deve ser informada.");
            }

            carregamento.Filiais = string.Join(",", (from obj in filiais select obj.Descricao).ToList());
            carregamento.Destinatarios = string.Join(",", (from obj in destinatarios select obj.Descricao).ToList());
            carregamento.Destinos = string.Join(",", (from obj in destinatarios select obj.Localidade.DescricaoCidadeEstado).Distinct().ToList());
            repCarregamento.Atualizar(carregamento);

            for (int i = 0; i < carregamentoPedidosExiste.Count; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido carregamentoPedidoExiste = carregamentoPedidosExiste[i];
                if (!codigosPedidos.Contains(carregamentoPedidoExiste.Pedido.Codigo))
                    repCarregamentoPedido.Deletar(carregamentoPedidoExiste);
            }
        }

        private void GerarCarregamentoCargaEmLote(List<int> codigosCarregamento, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, string urlAcessoCliente)
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamento repMontagemCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamento(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.MontagemCarregamento primeiroCarregamento = repMontagemCarregamento.BuscarPrimerioCarregamento(0);

            if (primeiroCarregamento != null)
                throw new ServicoException("Existem cargas sendo processados. Aguarde alguns instantes.");

            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCarregamentos(codigosCarregamento);

            if (carga != null)
                throw new ServicoException("Não é possível gerar cargas. Já existem cargas geradas.");

            foreach (int codigoCarregamento in codigosCarregamento)
            {
                unitOfWork.FlushAndClear();

                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = repCarregamento.BuscarPorCodigo(codigoCarregamento);
                configuracaoEmbarcador = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga servicoMontagemCarga = new Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga(unitOfWork, configuracaoEmbarcador);

                Dominio.ObjetosDeValor.Embarcador.Carga.PropriedadesGeracaoCarga propriedades = new Dominio.ObjetosDeValor.Embarcador.Carga.PropriedadesGeracaoCarga()
                {
                    MontagemCargaPedidoProduto = Dominio.ObjetosDeValor.Embarcador.Carga.MontagemCarga.MontagemCargaPedidoProduto.Validar,
                    Usuario = usuario
                };

                unitOfWork.Start();

                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidos = repCarregamentoPedido.BuscarPorCarregamento(carregamento.Codigo);
                List<Dominio.Entidades.Embarcador.Filiais.Filial> filiais = (from obj in carregamentoPedidos select obj.Pedido.Filial).Distinct().ToList();

                ValidarCarregamento(carregamento, tipoServicoMultisoftware, configuracaoEmbarcador, unitOfWork);
                servicoMontagemCarga.GerarCarga(carregamento, filiais, carregamentoPedidos, tipoServicoMultisoftware, null, auditado, propriedades, urlAcessoCliente);

                unitOfWork.CommitChanges();
            }
        }

        private void ValidarCarregamento(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            if (carregamento.VeiculoBloqueado)
                throw new ServicoException(string.Format("O veículo selecionado do carregamento {0} não possui licença ativa para o transporte. Favor solicite a liberação da viagem.", carregamento.NumeroCarregamento));

            if ((tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS) && configuracaoEmbarcador.RoteirizacaoObrigatoriaMontagemCarga)
            {
                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao repositorioCarregamentoRoteirizacao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repositorioTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.APIGoogle);
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoRoteirizacao carregamentoRoteirizacao = repositorioCarregamentoRoteirizacao.BuscarPorCarregamento(carregamento.Codigo);

                if ((configuracaoEmbarcador.TipoMontagemCargaPadrao == TipoMontagemCarga.NovaCarga) && (carregamentoRoteirizacao == null) && (tipoIntegracao != null))
                    throw new ServicoException(string.Format("É obrigatório roteirizar o carregamento {0} antes de seguir com a carga.", carregamento.NumeroCarregamento));
            }

            if (configuracaoEmbarcador.ObrigatorioGeracaoBlocosParaCarregamento && !carregamento.CarregamentoRedespacho)
            {
                Repositorio.Embarcador.Cargas.MontagemCarga.BlocoCarregamento repositorioBlocoCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.BlocoCarregamento(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.BlocoCarregamento> blocoCarregamentosExiste = repositorioBlocoCarregamento.BuscarPorCarregamento(carregamento.Codigo);

                if (blocoCarregamentosExiste.Count == 0)
                    throw new ServicoException(string.Format("É obrigatório gerar blocos do carregamento {0} antes de seguir com a carga.", carregamento.NumeroCarregamento));
            }

            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido repositorioCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidos = repositorioCarregamentoPedido.BuscarPorCarregamento(carregamento.Codigo);

            if (carregamentoPedidos.Count > 0)
            {
                var tiposOperacoes = (
                    from o in carregamentoPedidos
                    where o.Pedido?.TipoOperacao != null
                    select o.Pedido?.TipoOperacao?.Descricao
                ).Distinct().ToList();

                if (tiposOperacoes?.Count > 1)
                    throw new ServicoException(string.Format("Não é permitido gerar carga do carregamento {0} com pedidos de \"Tipos de operações\" diferentes [ {1} ].", carregamento.NumeroCarregamento, string.Join(",", tiposOperacoes)));

                var tiposCargas = (
                    from o in carregamentoPedidos
                    where o.Pedido?.TipoDeCarga != null
                    select o.Pedido?.TipoDeCarga?.Descricao
                ).Distinct().ToList();

                if (tiposCargas?.Count > 1 && carregamento.TipoDeCarga == null)
                    throw new ServicoException(string.Format("Não é permitido gerar carga do carregamento {0} com pedidos de \"Tipos de cargas\" diferentes [ {1} ].", carregamento.NumeroCarregamento, string.Join(",", tiposCargas)));

                var pedidosCancelados = (
                    from o in carregamentoPedidos
                    where o.Pedido?.SituacaoPedido == SituacaoPedido.Cancelado
                    select o.Pedido.NumeroPedidoEmbarcador
                ).Distinct().ToList();

                if (pedidosCancelados?.Count > 0)
                    throw new ServicoException(string.Format("Os pedidos do carregamento {0} foram cancelados: {1}.", carregamento.NumeroCarregamento, string.Join(",", pedidosCancelados)));

                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && carregamento.ModeloVeicularCarga != null)
                {
                    //#17664 Validar se os destinatários são fornecedores... e se não possui restrição para o modelo veicular
                    List<double> cnpjsDesnatarios = (from ped in carregamentoPedidos
                                                     where ped.Pedido.Destinatario != null
                                                     select ped.Pedido.Destinatario.CPF_CNPJ).Distinct().ToList();

                    Repositorio.Embarcador.Pessoas.ModalidadePessoas repModalidadePessoas = new Repositorio.Embarcador.Pessoas.ModalidadePessoas(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas> modalidadePessoasFornecedores = repModalidadePessoas.BuscarPorTipo(TipoModalidade.Fornecedor, cnpjsDesnatarios);

                    if (modalidadePessoasFornecedores.Count > 0)
                    {
                        Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoasRestricaoModeloVeicular repModalidadeFornecedorPessoasRestricaoModeloVeicular = new Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoasRestricaoModeloVeicular(unitOfWork);
                        List<int> codigosModalidades = (from modalidade in modalidadePessoasFornecedores select modalidade.Codigo).ToList();
                        List<Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoasRestricaoModeloVeicular> modalidadeFornecedorPessoasRestricaoModeloVeicular = repModalidadeFornecedorPessoasRestricaoModeloVeicular.BuscarPorModalidades(codigosModalidades);

                        if (modalidadeFornecedorPessoasRestricaoModeloVeicular.Count > 0)
                        {
                            foreach (Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoasRestricaoModeloVeicular modalidade in modalidadeFornecedorPessoasRestricaoModeloVeicular)
                            {
                                if (modalidade.ModeloVeicular.Codigo == carregamento.ModeloVeicularCarga.Codigo &&
                                    (modalidade.TipoOperacao == null || (modalidade.TipoOperacao?.Codigo ?? 0) == (carregamento.TipoOperacao?.Codigo ?? 0)))
                                    throw new ServicoException(string.Format("Os fornecedor {0} do carregamento {1} possui restrição de modelo veícular {2}.", modalidade?.ModalidadeFornecedorPessoa?.ModalidadePessoas?.Cliente?.NomeCNPJ ?? "", carregamento.NumeroCarregamento, carregamento.ModeloVeicularCarga.Descricao));
                            }
                        }
                    }

                    //#14515 validar se o veículo possui Tolerância minima para carregamento... e o peso total for inferior.., não permitir gerar..
                    //#23986 Não permitir peso acima da capacidade                    
                    if (configuracaoEmbarcador.ValidarCapacidadeModeloVeicularCargaNaMontagemCarga)
                    {
                        decimal toleranciaMinima = carregamento.ModeloVeicularCarga.ToleranciaPesoMenor;
                        decimal toleranciaMaxima = carregamento.ModeloVeicularCarga.ToleranciaPesoExtra;

                        decimal pesoMaximo = carregamento.ModeloVeicularCarga.CapacidadePesoTransporte + toleranciaMaxima;

                        decimal pesoCarregamento = carregamento.PesoCarregamento;
                        if (pesoCarregamento < toleranciaMinima)
                            throw new ServicoException(string.Format("Não é permitido gerar uma carga do carregamento {0} com peso inferior a tolerância mínima {1} do modelo veícular {2}.", carregamento.NumeroCarregamento, toleranciaMinima.ToString("n4"), carregamento.ModeloVeicularCarga.Descricao));

                        if (pesoCarregamento > pesoMaximo)
                            throw new ServicoException(string.Format("Não é permitido gerar uma carga do carregamento {0} com peso superior a tolerância máxima {1} do modelo veícular {2}.", carregamento.NumeroCarregamento, toleranciaMaxima.ToString("n4"), carregamento.ModeloVeicularCarga.Descricao));
                    }
                }
            }

            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoApolice repositorioCarregamentoApolice = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoApolice(unitOfWork);

            if ((carregamento.Empresa == null) && configuracaoEmbarcador.TransportadorObrigatorioMontagemCarga)
                throw new ServicoException(string.Format("Transportador é obrigatório do carregamento {0}.", carregamento.NumeroCarregamento));

            if (configuracaoEmbarcador.InformaApoliceSeguroMontagemCarga && !repositorioCarregamentoApolice.ExistePorCarregamento(carregamento.Codigo))
                throw new ServicoException(string.Format("Apólice de Seguro é obrigatória do carregamento {0}.", carregamento.NumeroCarregamento));

            if ((carregamento.TipoDeCarga == null) && configuracaoEmbarcador.TipoCargaObrigatorioMontagemCarga)
                throw new ServicoException(string.Format("Tipo de Carga é obrigatório do carregamento {0}.", carregamento.NumeroCarregamento));

            if ((carregamento.TipoOperacao == null) && configuracaoEmbarcador.TipoOperacaoObrigatorioMontagemCarga)
                throw new ServicoException(string.Format("Tipo de Operação é obrigatório do carregamento {0}.", carregamento.NumeroCarregamento));

            if (configuracaoEmbarcador.SimulacaoFreteObrigatorioMontagemCarga)
            {
                Repositorio.Embarcador.Cargas.MontagemCarga.SimulacaoFrete repositorioSimulacaoFrete = new Repositorio.Embarcador.Cargas.MontagemCarga.SimulacaoFrete(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SimulacaoFrete simulacao = repositorioSimulacaoFrete.BuscarPorCarregamento(carregamento.Codigo);

                if (simulacao?.SucessoSimulacao == false)
                    throw new ServicoException(string.Format("É obrigatório gerar a simulação de frete do carregamento {0} antes de gerar a carga.", carregamento.NumeroCarregamento));
            }
        }

        #endregion
    }
}
