using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using Servicos.Embarcador.Pedido;
using SGT.WebAdmin.Models.Grid;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.GestaoPallet
{
    [CustomAuthorize("GestaoPallet/AgendamentoPallet")]
    public class AgendamentoPalletController : BaseController
    {

        #region Contrutores

        public AgendamentoPalletController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                return new JsonpResult(ObterGridPesquisa(unidadeTrabalho));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar agendamento pallet.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaNFe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.GestaoPallet.AgendamentoPallet repositorioAgendamentoPallet = new Repositorio.Embarcador.GestaoPallet.AgendamentoPallet(unitOfWork);
                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal repCarregamentoPedidoNotaFiscal = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");
                Dominio.Entidades.Embarcador.GestaoPallet.AgendamentoPallet agendamento = repositorioAgendamentoPallet.BuscarPorCodigo(codigo, false);

                if (codigo > 0 && agendamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("ExigeChaveVenda", false);
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Volumes", false);
                grid.AdicionarCabecalho("Pallets", false);
                grid.AdicionarCabecalho("MetrosCubicos", false);
                grid.AdicionarCabecalho("ChaveVenda", false);
                grid.AdicionarCabecalho("Número", "Numero", 6, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Chave", "Chave", 15, Models.Grid.Align.left, false, true, true, false, true);
                grid.AdicionarCabecalho("Emitente", "Emitente", 14, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Origem", "Origem", 9, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destinatário", "Destinatario", 14, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destino", "Destino", 9, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Modalidade Pgto.", "DescricaoModalidadeFrete", 5, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Peso", "Peso", 6, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor", "Valor", 6, Models.Grid.Align.left, true);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenar == "Emitente" || propOrdenar == "Destinatario")
                    propOrdenar += ".Nome";
                else if (propOrdenar == "Destino")
                    propOrdenar = "Destinatario.Localidade.Descricao";
                else if (propOrdenar == "DescricaoModalidadeFrete")
                    propOrdenar = "ModalidadeFrete";
                propOrdenar = "XMLNotaFiscal." + propOrdenar;

                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> listaNotasFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

                if (agendamento != null)
                {
                    if (agendamento.Pedido != null)
                    {
                        listaNotasFiscais = agendamento.Pedido.NotasFiscais.ToList();
                    }

                    if (!ConfiguracaoEmbarcador.ControlarAgendamentoSKU && agendamento.Carga?.Carregamento != null)
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal> listaNotasEnviadas = repCarregamentoPedidoNotaFiscal.BuscarPorCarregamento(agendamento.Carga.Carregamento.Codigo);

                        listaNotasFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();
                        foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal notaFiscal in listaNotasEnviadas)
                            listaNotasFiscais.AddRange(notaFiscal.NotasFiscais.ToList());
                    }

                    if (listaNotasFiscais.Count == 0 && agendamento.Carga != null)
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(agendamento.Carga.Codigo);
                        foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                        {
                            listaNotasFiscais.AddRange(cargaPedido.NotasFiscais.Select(nf => nf.XMLNotaFiscal).ToList());
                        }
                    }

                    if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
                        listaNotasFiscais = listaNotasFiscais.Where(obj => obj.Emitente.Codigo == Usuario.ClienteFornecedor.CPF_CNPJ).ToList();
                }

                var dynXmlNotaFiscal = (from obj in listaNotasFiscais
                                        where obj.nfAtiva == true
                                        select new
                                        {
                                            ExigeChaveVenda = agendamento.Carga?.TipoOperacao?.ExigeChaveVendaAntesConfirmarNotas ?? false,
                                            obj.Codigo,
                                            obj.Numero,
                                            obj.Chave,
                                            Emitente = obj.Emitente != null ? obj.Emitente.Descricao : "",
                                            Origem = obj.TipoOperacaoNotaFiscal == TipoOperacaoNotaFiscal.Saida ? obj.Emitente != null ? obj.Emitente.Localidade.DescricaoCidadeEstado : "" : obj.Destinatario != null ? obj.Destinatario.Localidade.DescricaoCidadeEstado : "",
                                            Destinatario = obj.Destinatario != null ? obj.Destinatario.Descricao : "",
                                            Destino = obj.TipoOperacaoNotaFiscal == TipoOperacaoNotaFiscal.Saida ? obj.Destinatario != null ? obj.Destinatario.Localidade.DescricaoCidadeEstado : "" : obj.Emitente != null ? obj.Emitente.Localidade.DescricaoCidadeEstado : "",
                                            obj.DescricaoModalidadeFrete,
                                            obj.Valor,
                                            Peso = obj.Peso.ToString("n3"),
                                            ChaveVenda = obj.ChaveVenda,
                                            obj.Volumes,
                                            obj.MetrosCubicos,
                                            Pallets = obj.QuantidadePallets
                                        }).ToList();

                grid.setarQuantidadeTotal(dynXmlNotaFiscal.Count);
                grid.AdicionaRows(dynXmlNotaFiscal);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirNFe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoAgendamento = Request.GetIntParam("Agendamento");
                int codigoNFe = Request.GetIntParam("NFe");

                Repositorio.Embarcador.GestaoPallet.AgendamentoPallet repositorioAgendamentoPallet = new Repositorio.Embarcador.GestaoPallet.AgendamentoPallet(unitOfWork);
                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal repCarregamentoPedidoNotaFiscal = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Integracao.IntegracaoAVIPED repIntegracaoAVIPED = new Repositorio.Embarcador.Integracao.IntegracaoAVIPED(unitOfWork);

                Dominio.Entidades.Embarcador.GestaoPallet.AgendamentoPallet agendamentoPallet = repositorioAgendamentoPallet.BuscarPorCodigo(codigoAgendamento, false);

                if (agendamentoPallet == null)
                    return new JsonpResult(false, true, "Agendamento não encontrado.");

                if (agendamentoPallet.Situacao == SituacaoAgendamentoPallet.Cancelado)
                    return new JsonpResult(false, true, "Não é possível modificar as notas de um agendamento cancelado");

                if (agendamentoPallet.EtapaAgendamentoPallet != EtapaAgendamentoPallet.NFe)
                    return new JsonpResult(false, true, "Não é possível modificar as notas na atual esapa do agendamento");

                unitOfWork.Start();

                if (agendamentoPallet.Pedido != null)
                {
                    foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xml in agendamentoPallet.Pedido.NotasFiscais)
                    {
                        if (xml.Codigo == codigoNFe)
                        {
                            agendamentoPallet.Pedido.NotasFiscais.Remove(xml);
                            xml.nfAtiva = false;
                            repXMLNotaFiscal.Atualizar(xml);
                            break;
                        }
                    }

                    repPedido.Atualizar(agendamentoPallet.Pedido);
                }
                else if (agendamentoPallet.Carga?.Carregamento != null)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal> listaNotasEnviadas = repCarregamentoPedidoNotaFiscal.BuscarPorCarregamento(agendamentoPallet.Carga.Carregamento.Codigo);

                    foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal notaFiscal in listaNotasEnviadas)
                    {
                        if (!notaFiscal.NotasFiscais.Any(nf => nf.Codigo == codigoNFe))
                            continue;

                        foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xml in notaFiscal.NotasFiscais)
                        {
                            if (xml.Codigo == codigoNFe)
                            {
                                notaFiscal.NotasFiscais.Remove(xml);
                                xml.nfAtiva = false;
                                repXMLNotaFiscal.Atualizar(xml);
                                break;
                            }
                        }

                        repCarregamentoPedidoNotaFiscal.Atualizar(notaFiscal);
                        break;
                    }
                }
                else if (agendamentoPallet.Carga != null)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotasFiscais = repositorioPedidoXMLNotaFiscal.BuscarPorCarga(agendamentoPallet.Carga.Codigo);

                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal notaFiscal in pedidoXMLNotasFiscais)
                    {
                        if (notaFiscal.XMLNotaFiscal.Codigo == codigoNFe)
                        {
                            notaFiscal.XMLNotaFiscal.nfAtiva = false;
                            repXMLNotaFiscal.Atualizar(notaFiscal.XMLNotaFiscal);
                            repIntegracaoAVIPED.DeletarPorPedidoXMLNotaFiscal(notaFiscal.Codigo);
                            repositorioPedidoXMLNotaFiscal.Deletar(notaFiscal);
                            break;
                        }
                    }
                }

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao excluir NFe");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EnviarNFe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.GestaoPallet.AgendamentoPallet repositorioAgendamentoPallet = new Repositorio.Embarcador.GestaoPallet.AgendamentoPallet(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPallet.AgendamentoPallet agendamentoPallet = repositorioAgendamentoPallet.BuscarPorCodigo(codigo, false);

                if (agendamentoPallet == null)
                    return new JsonpResult(false, true, "Agendamento não encontrado.");

                if (agendamentoPallet.Situacao == SituacaoAgendamentoPallet.Cancelado)
                    return new JsonpResult(false, true, "Não é possível modificar as notas de um agendamento cancelado");

                if (agendamentoPallet.EtapaAgendamentoPallet != EtapaAgendamentoPallet.NFe)
                    return new JsonpResult(false, true, "Não é possível modificar as notas na atual etapa do agendamento");

                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

                if (files.Count == 0)
                    return new JsonpResult(false, true, "Não foi enviado o arquivo.");

                unitOfWork.Start();

                string mensagemRetorno = string.Empty;
                List<RetornoArquivo> retornoArquivos = new List<RetornoArquivo>();

                for (int i = 0; i < files.Count; i++)
                {
                    Servicos.DTO.CustomFile file = files[i];
                    string extensao = System.IO.Path.GetExtension(file.FileName).ToLower();

                    RetornoArquivo retornoArquivo = new RetornoArquivo
                    {
                        nome = file.FileName,
                        processada = true,
                        mensagem = "",
                        codigo = 0
                    };

                    if (extensao != ".xml")
                    {
                        retornoArquivo.processada = false;
                        retornoArquivo.mensagem = "A extensão do arquivo é inválida.";
                        mensagemRetorno = RetornarMensagem(mensagemRetorno, "Há arquivos com extenção inválida.");
                        retornoArquivos.Add(retornoArquivo);
                        continue;
                    }

                    try
                    {
                        var objetoNFe = MultiSoftware.NFe.Servicos.Leitura.Ler(file.InputStream);
                        if (objetoNFe == null)
                        {
                            retornoArquivo.processada = false;
                            retornoArquivo.mensagem = "O xml informado não é uma NF-e, por favor verifique.";
                            mensagemRetorno = RetornarMensagem(mensagemRetorno, "Há arquivos que não são nota fiscal.");
                            retornoArquivos.Add(retornoArquivo);
                            continue;
                        }

                        string retorno = ProcessarXMLNFe(file.InputStream, agendamentoPallet, unitOfWork, out bool msgAlertaObservacao);

                        if (!string.IsNullOrEmpty(retorno) && !msgAlertaObservacao)
                        {
                            retornoArquivo.processada = false;
                            retornoArquivo.mensagem = retorno;
                        }
                        else if (msgAlertaObservacao)
                        {
                            retornoArquivo.processada = true;
                            retornoArquivo.mensagem = retorno;
                        }
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex);
                        retornoArquivo.processada = false;
                        retornoArquivo.mensagem = "Ocorreu uma falha ao enviar o xml, verifique se o arquivo é um documento fiscal válido. " + ex.Message;
                        mensagemRetorno = RetornarMensagem(mensagemRetorno, "Ocorreu falhas ao enviar os xmls, verifique se os arquivos são documentos fiscais válidos.");
                    }
                    finally
                    {
                        file.InputStream.Dispose();
                    }

                    Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = null;

                    if (agendamentoPallet.Pedido != null)
                        pedido = agendamentoPallet.Pedido;
                    else if (agendamentoPallet.Carga != null && agendamentoPallet.Carga.Pedidos?.Count > 0)
                        pedido = agendamentoPallet.Carga.Pedidos.Select(o => o.Pedido).FirstOrDefault();
                    else
                        mensagemRetorno = RetornarMensagem(mensagemRetorno, "Ocorreu falha ao vincular NF-e ao pedido. Pedido não econtrado.");

                    repPedido.Atualizar(pedido);
                    retornoArquivos.Add(retornoArquivo);
                }

                unitOfWork.CommitChanges();
                return new JsonpResult(new
                {
                    Arquivos = retornoArquivos,
                    MensagemRetorno = mensagemRetorno
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao enviar o NF-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Grid grid = ObterGridPesquisa(unidadeTrabalho);
                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario == null)
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoGerarArquivo);

                return Arquivo(arquivoBinario, grid.extensaoCSV, $"{grid.tituloExportacao}.{grid.extensaoCSV}");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoGerarArquivo);
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.GestaoPallet.AgendamentoPallet repositorioAgendamento = new Repositorio.Embarcador.GestaoPallet.AgendamentoPallet(unidadeTrabalho);

                Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDescarregamento configuracaoDescarregamento = new Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDescarregamento
                {
                    NaoPermitirBuscarOutroPeriodo = true
                };

                Servicos.Embarcador.Logistica.CargaJanelaDescarregamento servicoJanelaDescarregamento = new Servicos.Embarcador.Logistica.CargaJanelaDescarregamento(unidadeTrabalho, ConfiguracaoEmbarcador, configuracaoDescarregamento);
                Servicos.Embarcador.GestaoPallet.AgendamentoPallet servicoAgendamento = new Servicos.Embarcador.GestaoPallet.AgendamentoPallet(unidadeTrabalho, TipoServicoMultisoftware, ConfiguracaoEmbarcador);

                Dominio.Entidades.Embarcador.GestaoPallet.AgendamentoPallet agendamento = new Dominio.Entidades.Embarcador.GestaoPallet.AgendamentoPallet();

                unidadeTrabalho.Start();

                PreencherEntidadeAgendamento(agendamento, unidadeTrabalho);

                agendamento.Carga = servicoAgendamento.AdicionarCarga(agendamento, Cliente);

                repositorioAgendamento.Inserir(agendamento, Auditado);

                Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodo = ObterPeriodo(unidadeTrabalho);

                servicoJanelaDescarregamento.Adicionar(agendamento.Carga, agendamento.DataEntrega ?? DateTime.Now, TipoServicoMultisoftware, periodo);

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(new
                {
                    agendamento.Codigo,
                    agendamento.EtapaAgendamentoPallet,
                    agendamento.Situacao
                });
            }
            catch (BaseException excecao)
            {
                unidadeTrabalho.Rollback();

                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar agendamento pallet.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.GestaoPallet.AgendamentoPallet repositorioAgendamentoPallet = new Repositorio.Embarcador.GestaoPallet.AgendamentoPallet(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(unidadeTrabalho);

                Dominio.Entidades.Embarcador.GestaoPallet.AgendamentoPallet agendamentoPallet = repositorioAgendamentoPallet.BuscarPorCodigo(codigo, false);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento = repositorioCargaJanelaDescarregamento.BuscarPorCarga(agendamentoPallet?.Carga?.Codigo ?? 0);

                if (agendamentoPallet == null)
                    return new JsonpResult(false, "Agendamento não encontrado.");

                return new JsonpResult(new
                {
                    AgendamentoPallet = new
                    {
                        agendamentoPallet.Codigo,
                        agendamentoPallet.QuantidadePallets,
                        agendamentoPallet.EtapaAgendamentoPallet,
                        agendamentoPallet.Situacao,
                        agendamentoPallet.Observacao,
                        agendamentoPallet.ResponsavelPallet,
                        DataEntrega = agendamentoPallet.DataEntrega?.ToString("dd/MM/yyyy") ?? string.Empty,
                        Remetente = agendamentoPallet.Remetente == null ? null : new
                        {
                            agendamentoPallet.Remetente.CPF_CNPJ,
                            agendamentoPallet.Remetente.Descricao
                        },
                        ModeloVeicular = agendamentoPallet.ModeloVeicular == null ? null : new
                        {
                            agendamentoPallet.ModeloVeicular.Codigo,
                            agendamentoPallet.ModeloVeicular.Descricao
                        },
                        Veiculo = agendamentoPallet.VeiculoSelecionado == null ? null : new
                        {
                            agendamentoPallet.VeiculoSelecionado.Codigo,
                            agendamentoPallet.VeiculoSelecionado.Descricao
                        },
                        Motorista = agendamentoPallet.MotoristaSelecionado == null ? null : new
                        {
                            agendamentoPallet.MotoristaSelecionado.Codigo,
                            agendamentoPallet.MotoristaSelecionado.Descricao
                        },
                        TipoCarga = agendamentoPallet.TipoCarga == null ? null : new
                        {
                            agendamentoPallet.TipoCarga.Codigo,
                            agendamentoPallet.TipoCarga.Descricao
                        },
                        Destinatario = new
                        {
                            agendamentoPallet.Destinatario.CPF_CNPJ,
                            agendamentoPallet.Destinatario.Descricao
                        },
                        Transportador = new
                        {
                            Codigo = agendamentoPallet.Transportador?.Codigo ?? 0,
                            Descricao = agendamentoPallet.Transportador?.Descricao ?? string.Empty
                        }
                    },
                    RetornoAcompanhamentoPallet = new
                    {
                        SenhaAgendamento = agendamentoPallet.Senha,
                        SituacaoCodigo = agendamentoPallet.Situacao,
                        QuantidadePallet = agendamentoPallet.QuantidadePallets,
                        Situacao = agendamentoPallet.Situacao?.ObterDescricao(),
                        DataSolicitada = agendamentoPallet.DataAgendamento?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                        DataProgramada = cargaJanelaDescarregamento?.InicioDescarregamento.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                        DataCancelamento = agendamentoPallet.DataCancelamento?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                        NumeroCarga = agendamentoPallet.Carga?.Numero,
                        NumeroCarregamento = agendamentoPallet.Carga?.Carregamento?.NumeroCarregamento ?? string.Empty,
                        OperadorAgendamento = agendamentoPallet.Solicitante?.Nome ?? agendamentoPallet.Carga?.Operador?.Nome ?? string.Empty
                    }
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar registro.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Cancelar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.GestaoPallet.AgendamentoPallet repositorioAgendamentoPallet = new Repositorio.Embarcador.GestaoPallet.AgendamentoPallet(unitOfWork);

                Dominio.Entidades.Embarcador.GestaoPallet.AgendamentoPallet agendamentoPallet = repositorioAgendamentoPallet.BuscarPorCodigo(codigo, false);

                if (agendamentoPallet == null)
                    return new JsonpResult(false, "Agendamento não encontrado.");

                if (agendamentoPallet.Situacao == SituacaoAgendamentoPallet.Cancelado)
                    return new JsonpResult(false, "O Agendamento já foi cancelado.");

                if (agendamentoPallet.Situacao == SituacaoAgendamentoPallet.Finalizado)
                    return new JsonpResult(false, "O Agendamento não pode ser cancelado pois já foi finalizado.");

                Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(unitOfWork);
                Servicos.Embarcador.Logistica.CargaJanelaDescarregamento servicoCargaJanelaDescarregamento = new Servicos.Embarcador.Logistica.CargaJanelaDescarregamento(unitOfWork, ConfiguracaoEmbarcador, Auditado);

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento = repositorioCargaJanelaDescarregamento.BuscarPorCarga(agendamentoPallet.Carga.Codigo);

                if (DateTime.Now > cargaJanelaDescarregamento.InicioDescarregamento)
                    throw new ControllerException($"Não é possível cancelar a agenda pois a data atual é maior do que a data agendada.");

                agendamentoPallet.Initialize();
                cargaJanelaDescarregamento.Initialize();

                Dominio.ObjetosDeValor.Embarcador.Carga.CargaCancelamentoAdicionar cargaCancelamentoAdicionar = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaCancelamentoAdicionar()
                {
                    Carga = agendamentoPallet.Carga,
                    GerarIntegracoes = false,
                    MotivoCancelamento = "Cancelamento por Agendamento de Pallet",
                    TipoServicoMultisoftware = TipoServicoMultisoftware,
                    Usuario = this.Usuario
                };

                Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = Servicos.Embarcador.Carga.Cancelamento.GerarCargaCancelamento(cargaCancelamentoAdicionar, ConfiguracaoEmbarcador, unitOfWork);
                Servicos.Embarcador.Carga.Cancelamento.SolicitarCancelamentoCarga(ref cargaCancelamento, unitOfWork, _conexao.StringConexao, TipoServicoMultisoftware);

                if (cargaCancelamento.Situacao == SituacaoCancelamentoCarga.RejeicaoCancelamento)
                {
                    unitOfWork.CommitChanges();
                    return new JsonpResult(false, cargaCancelamento.MensagemRejeicaoCancelamento);
                }

                agendamentoPallet.DataCancelamento = DateTime.Now;
                agendamentoPallet.Situacao = SituacaoAgendamentoPallet.Cancelado;

                servicoCargaJanelaDescarregamento.AtualizarSituacao(cargaJanelaDescarregamento, SituacaoCargaJanelaDescarregamento.Cancelado);
                repositorioCargaJanelaDescarregamento.Atualizar(cargaJanelaDescarregamento, Auditado);
                agendamentoPallet.SetExternalChanges(cargaJanelaDescarregamento.GetCurrentChanges());

                repositorioAgendamentoPallet.Atualizar(agendamentoPallet, Auditado, null, "Agendamento Coleta Cancelado pela tela de Agendamento.");

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();

                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao solicitar o cancelamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Imprimir()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.GestaoPallet.AgendamentoPallet repAgendamentoAgendamentoPallet = new Repositorio.Embarcador.GestaoPallet.AgendamentoPallet(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.GestaoPallet.AgendamentoPallet agendamento = repAgendamentoAgendamentoPallet.BuscarPorCodigo(codigo, false);

                if (agendamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                byte[] arquivo = new Servicos.Embarcador.GestaoPallet.AgendamentoPallet(unitOfWork).ResumoAgendamentoPallet(agendamento.Codigo);

                return Arquivo(arquivo, "application/pdf", "Resumo Agendamento - " + agendamento.Carga.CodigoCargaEmbarcador + ".pdf");
            }
            catch (ServicoException ex)
            {
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion Métodos Globais

        #region Métodos Privados

        private Grid ObterGridPesquisa(Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.GestaoPallet.AgendamentoPallet repositorioAgendamentoPallet = new Repositorio.Embarcador.GestaoPallet.AgendamentoPallet(unidadeTrabalho);

            Grid grid = ObterCabecalhosGridPesquisa();
            Dominio.ObjetosDeValor.Embarcador.GestaoPallet.AgendamentoPallet.FiltroPesquisaAgendamentoPallet filtrosPesquisa = ObterFiltrosPesquisa();
            int totalLinhas = repositorioAgendamentoPallet.ContarConsultaAgendamentoPallet(filtrosPesquisa);

            if (totalLinhas == 0)
            {
                grid.AdicionaRows(new List<dynamic>() { });
                return grid;
            }

            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
            parametrosConsulta.PropriedadeOrdenar = ObterPropriedadeOrdenarOuAgrupar(parametrosConsulta.PropriedadeOrdenar);

            IList<Dominio.ObjetosDeValor.Embarcador.GestaoPallet.AgendamentoPallet.DadosPesquisaAgendamentoPallet> dados = repositorioAgendamentoPallet.ConsultaAgendamentoPallet(filtrosPesquisa, parametrosConsulta);

            grid.AdicionaRows(dados);
            grid.setarQuantidadeTotal(totalLinhas);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.GestaoPallet.AgendamentoPallet.FiltroPesquisaAgendamentoPallet ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.GestaoPallet.AgendamentoPallet.FiltroPesquisaAgendamentoPallet filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.GestaoPallet.AgendamentoPallet.FiltroPesquisaAgendamentoPallet()
            {
                NumeroCarga = Request.GetStringParam("Carga"),
                DataAgendamento = Request.GetDateTimeParam("DataAgendamento"),
                CodigoDestinatario = Request.GetLongParam("Destinatario"),
                Senha = Request.GetStringParam("Senha"),
                SituacaoCargaJanelaCarregamento = Request.GetNullableEnumParam<SituacaoCargaJanelaCarregamento>("Situacao"),
                CodigoCliente = Request.GetLongParam("Cliente"),
                CodigoTransportador = Request.GetIntParam("Transportador")
            };

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                filtrosPesquisa.CodigoTransportador = Usuario.Empresa.Codigo;

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
                filtrosPesquisa.CodigoCliente = Usuario.ClienteFornecedor.Codigo;

            return filtrosPesquisa;
        }

        private Grid ObterCabecalhosGridPesquisa()
        {
            Grid grid = new Grid(Request)
            {
                header = new List<Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Carga", "Carga", 15, Align.left, true);
            grid.AdicionarCabecalho("Data Solicitada", "DataAgendamentoFormatada", 15, Align.center, true);
            grid.AdicionarCabecalho("Data Criação", "DataCriacaoFormatada", 15, Align.center, true);
            grid.AdicionarCabecalho("Data Agendada", "DataConfirmadaFormatada", 15, Align.center, false);
            grid.AdicionarCabecalho("Remetente", "Remetente", 35, Align.left, true);
            grid.AdicionarCabecalho("Destinatário", "Destinatario", 35, Align.left, true);
            grid.AdicionarCabecalho("Tipo de Carga", "TipoCarga", 20, Align.left, true);
            grid.AdicionarCabecalho("Etapa Agendamento", "EtapaAgendamentoPalletFormatada", 20, Align.left, false);
            grid.AdicionarCabecalho("Etapa Carga", "SituacaoCargaFormatada", 20, Align.left, false);
            grid.AdicionarCabecalho("Situação Agendamento", "SituacaoFormatada", 20, Align.left, true);
            grid.AdicionarCabecalho("Senha", "Senha", 20, Align.left, false);

            return grid;
        }

        private string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar.EndsWith("Formatada"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatada", "");

            return propriedadeOrdenarOuAgrupar;
        }

        private void PreencherEntidadeAgendamento(Dominio.Entidades.Embarcador.GestaoPallet.AgendamentoPallet agendamentoPallet, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unidadeTrabalho);
            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unidadeTrabalho);
            Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unidadeTrabalho);
            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unidadeTrabalho);

            Servicos.Embarcador.GestaoPallet.AgendamentoPallet servicoAgendamentoPallet = new Servicos.Embarcador.GestaoPallet.AgendamentoPallet(unidadeTrabalho);

            int codigoModeloVeicular = Request.GetIntParam("ModeloVeicular");
            int codigoTipoCarga = Request.GetIntParam("TipoCarga");
            int codigoMotorista = Request.GetIntParam("Motorista");
            int codigoVeiculo = Request.GetIntParam("Veiculo");

            Dominio.Entidades.Cliente destinatario = repositorioCliente.BuscarPorCPFCNPJ(Request.GetDoubleParam("Destinatario"));

            agendamentoPallet.DataEntrega = Request.GetDateTimeParam("DataEntrega", DateTime.Now);
            agendamentoPallet.DataAgendamento = Request.GetDateTimeParam("DataAgendamento", DateTime.Now);
            agendamentoPallet.Observacao = Request.GetStringParam("Observacao");
            agendamentoPallet.Destinatario = destinatario;
            agendamentoPallet.ModeloVeicular = (codigoModeloVeicular > 0) ? repositorioModeloVeicularCarga.BuscarPorCodigo(codigoModeloVeicular) : null;
            agendamentoPallet.TipoCarga = (codigoTipoCarga > 0) ? repositorioTipoCarga.BuscarPorCodigo(codigoTipoCarga) : null;
            agendamentoPallet.Transportador = ObterTransportador(unidadeTrabalho);
            agendamentoPallet.ResponsavelPallet = ObterResponsavelPallet();
            agendamentoPallet.Remetente = ObterRemetente(unidadeTrabalho);
            agendamentoPallet.Carga = null;
            agendamentoPallet.ResponsavelConfirmacao = null;
            agendamentoPallet.Situacao = SituacaoAgendamentoPallet.Agendamento;
            agendamentoPallet.EtapaAgendamentoPallet = EtapaAgendamentoPallet.NFe;
            agendamentoPallet.MotoristaSelecionado = (codigoMotorista > 0) ? repositorioUsuario.BuscarPorCodigo(codigoMotorista) : null;
            agendamentoPallet.VeiculoSelecionado = (codigoVeiculo > 0) ? repositorioVeiculo.BuscarPorCodigo(codigoVeiculo) : null;
            agendamentoPallet.Pedido = null;
            agendamentoPallet.SenhaSequencial = 0;
            agendamentoPallet.Solicitante = this.Usuario;
            agendamentoPallet.QuantidadePallets = Request.GetIntParam("QuantidadePallets");
            agendamentoPallet.TipoOperacao = repositorioTipoOperacao.BuscarTipoOperacaoPorTipoDeCarga(codigoTipoCarga);

            if (!ConfiguracaoEmbarcador.NaoGerarSenhaAgendamento)
                agendamentoPallet.Senha = servicoAgendamentoPallet.ObterSenhaAgendamentoPallet(agendamentoPallet);

            agendamentoPallet.Sequencia = Servicos.Embarcador.GestaoPallet.AgendamentoPalletSequencial.GetInstance().ObterProximoNumeroSequencial(unidadeTrabalho);
        }

        private ResponsavelPallet ObterResponsavelPallet()
        {
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
                return ResponsavelPallet.Cliente;

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                return ResponsavelPallet.Transportador;

            return Request.GetEnumParam<ResponsavelPallet>("ResponsavelPallet");
        }

        private Dominio.Entidades.Cliente ObterRemetente(Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeTrabalho);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
            {
                bool compartilharAcessoEntreGrupoPessoas = IsCompartilharAcessoEntreGrupoPessoas();

                if (compartilharAcessoEntreGrupoPessoas)
                {
                    Dominio.Entidades.Cliente remetente = repCliente.BuscarPorCPFCNPJ(Request.GetDoubleParam("Remetente"));
                    return (Usuario.ClienteFornecedor.GrupoPessoas?.Clientes?.Contains(remetente) ?? false) ? remetente : Usuario.ClienteFornecedor;
                }

                return Usuario.ClienteFornecedor;
            }

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                Dominio.Entidades.Cliente remetente = repCliente.BuscarPorCPFCNPJ(Usuario.Empresa.CNPJ.ObterSomenteNumeros().ToDouble());
                return remetente;
            }

            return repCliente.BuscarPorCPFCNPJ(Request.GetDoubleParam("Remetente"));
        }

        private Dominio.Entidades.Empresa ObterTransportador(Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unidadeTrabalho);

            int codigoTransportador = Request.GetIntParam("Transportador");

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                codigoTransportador = Empresa.Codigo;

                if (codigoTransportador == 0)
                    throw new ControllerException("Não foi possível encontrar o transportador logado.");
            }

            return repositorioEmpresa.BuscarPorCodigo(codigoTransportador);
        }

        private Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento ObterPeriodo(Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Logistica.PeriodoDescarregamento repositorioPeriodo = new Repositorio.Embarcador.Logistica.PeriodoDescarregamento(unidadeTrabalho);

            return repositorioPeriodo.BuscarPorCodigo(Request.GetIntParam("PeriodoAgendamento"));
        }

        private string RetornarMensagem(string mensagem, string mensagemNova)
        {
            if (mensagem.Contains(mensagemNova))
                return mensagem;

            return string.IsNullOrWhiteSpace(mensagem) ? mensagemNova : mensagem += $"; {mensagemNova}";
        }

        private string ProcessarXMLNFe(System.IO.Stream xml, Dominio.Entidades.Embarcador.GestaoPallet.AgendamentoPallet agendamentoPallet, Repositorio.UnitOfWork unitOfWork, out bool msgAlertaObservacao)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoNF = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);
          
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebservice = repConfiguracaoWebService.BuscarConfiguracaoPadrao();
           
            msgAlertaObservacao = false;
            string retorno = "";

            xml.Position = 0;
            Servicos.Embarcador.NFe.NFe serNFe = new Servicos.Embarcador.NFe.NFe(unitOfWork);
            System.IO.StreamReader stReaderXML = new System.IO.StreamReader(xml);

            if (!serNFe.BuscarDadosNotaFiscal(out string erro, out Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, stReaderXML, unitOfWork, null, true, false, false, TipoServicoMultisoftware, false, ConfiguracaoEmbarcador?.UtilizarValorFreteNota ?? false, null, null, null, configuracaoWebservice?.CadastroAutomaticoPessoaExterior ?? false))
                return erro;

            if (this.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor && agendamentoPallet.Pedido != null)
            {
                if (xmlNotaFiscal.Destinatario?.Codigo != agendamentoPallet.Pedido?.Destinatario?.Codigo)
                    return $"O destinatário da nota ({xmlNotaFiscal.Destinatario.Descricao}) não é o mesmo do pedido ({agendamentoPallet.Pedido.Destinatario.Descricao}).";

                if (xmlNotaFiscal.Emitente?.Codigo != agendamentoPallet.Pedido?.Remetente?.Codigo)
                    return $"O remetente da nota ({xmlNotaFiscal.Emitente.Descricao}) não é o mesmo do pedido ({agendamentoPallet.Pedido.Remetente.Descricao}).";
            }

            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

            if (xmlNotaFiscal.Codigo == 0)
            {
                xmlNotaFiscal.DataRecebimento = DateTime.Now;
                repXmlNotaFiscal.Inserir(xmlNotaFiscal);
            }
            else
                repXmlNotaFiscal.Atualizar(xmlNotaFiscal);

            if (string.IsNullOrEmpty(retorno) || msgAlertaObservacao)
            {
                PedidoXMLNotaFiscal servicoPedidoXMLNotaFiscal = new PedidoXMLNotaFiscal(unitOfWork, ConfiguracaoEmbarcador);

                xmlNotaFiscal.SemCarga = false;

                servicoPedidoXMLNotaFiscal.ArmazenarProdutosXML(xmlNotaFiscal.XML, xmlNotaFiscal, Auditado, TipoServicoMultisoftware, agendamentoPallet.Pedido);

                if (agendamentoPallet.Pedido != null)
                    agendamentoPallet.Pedido.NotasFiscais.Add(xmlNotaFiscal);
                else if (agendamentoPallet.Carga != null)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(agendamentoPallet.Carga.Codigo);
                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaPedidos != null && cargaPedidos.Count > 0 ? cargaPedidos.FirstOrDefault() : null;

                    if (cargaPedido != null)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = repositorioPedidoNF.BuscarPorCargaPedidoEXMLNotaFiscal(cargaPedido.Codigo, xmlNotaFiscal.Codigo);

                        if (pedidoXMLNotaFiscal == null)
                        {
                            pedidoXMLNotaFiscal = new Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal()
                            {
                                XMLNotaFiscal = xmlNotaFiscal,
                                CargaPedido = cargaPedido
                            };

                            repositorioPedidoNF.Inserir(pedidoXMLNotaFiscal);
                        }
                    }
                }

                msgAlertaObservacao = true;
            }

            return retorno;
        }

        #endregion Métodos Privados

        #region Classes Privadas

        private class RetornoArquivo
        {
            public int codigo { get; set; }

            public string nome { get; set; }

            public bool processada { get; set; }

            public string mensagem { get; set; }
        }

        #endregion Classes Privadas
    }
}