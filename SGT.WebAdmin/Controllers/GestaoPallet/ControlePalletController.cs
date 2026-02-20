using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.GestaoPallet;
using Dominio.ObjetosDeValor.Embarcador.GestaoPallet.DetalhesGestaoPallet;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;
using ConsultaMovimentacaoPallet = Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControlePallet.ConsultaMovimentacaoPallet;

namespace SGT.WebAdmin.Controllers.GestaoPallet
{
    [CustomAuthorize("GestaoPallet/ControlePallet")]
    public class ControlePalletController : BaseController
    {
        #region Construtores

        public ControlePalletController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                return new JsonpResult(await ObterGridPesquisa(unitOfWork, cancellationToken));
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaSaldo(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                return new JsonpResult(await ObterGridPesquisaSaldo(unitOfWork, cancellationToken));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterTotais(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.GestaoPallet.MovimentacaoPallet repositorioMovimentacaoPallet = new Repositorio.Embarcador.GestaoPallet.MovimentacaoPallet(unitOfWork, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControlePallet.FiltroPesquisaControlePallet filtroPesquisa = ObterFiltrosPesquisaControlePallet();
                Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControlePallet.TotalizadoresEnvioDevolucaoPallets totalizadores = await repositorioMovimentacaoPallet.ObterTotalizadoresControlePalletAsync(filtroPesquisa);

                return new JsonpResult(totalizadores);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = await ObterGridPesquisa(unitOfWork, cancellationToken);
                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario == null)
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoGerarArquivo);

                return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoGerarArquivo);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorCodigo(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.GestaoPallet.MovimentacaoPallet repositorioMovimentacaoPallet = new Repositorio.Embarcador.GestaoPallet.MovimentacaoPallet(unitOfWork, cancellationToken);

                DetalhesGestaoPallet dados = await repositorioMovimentacaoPallet.BuscarPorMovimentacaoPalletAsync(codigo);

                return new JsonpResult(dados);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Servicos.Embarcador.GestaoPallet.MovimentacaoPallet servicoMovimentacaoPallet = new Servicos.Embarcador.GestaoPallet.MovimentacaoPallet(unitOfWork, Auditado);
                Servicos.Embarcador.GestaoPallet.ControleEstoquePallet servicoControleEstoquePallet = new Servicos.Embarcador.GestaoPallet.ControleEstoquePallet(unitOfWork);

                int codigoNotaFiscal = Request.GetIntParam("NotaFiscal");
                int quantidadePallets = Request.GetIntParam("QuantidadePallets");
                int codigoCarga = Request.GetIntParam("Carga");
                string observacao = Request.GetStringParam("Observacao");
                int codigoFilial = Request.GetIntParam("Filial");
                int codigoTransportador = Request.GetIntParam("Transportador");
                long codigoCliente = Request.GetLongParam("Cliente");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelPallet responsavelPallet = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelPallet>("ResponsavelPallet");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida tipoEntradaSaida = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida>("TipoEntradaSaida");

                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = repositorioXmlNotaFiscal.BuscarPorCodigo(codigoNotaFiscal);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = null;

                if (quantidadePallets <= 0)
                    throw new ControllerException("Quantidade de pallets não pode estar zerada");

                if (codigoCarga > 0)
                {
                    carga = repositorioCarga.BuscarPorCodigo(codigoCarga);
                    if (carga == null)
                        throw new ControllerException("Carga não encontrada");
                }

                unitOfWork.Start();

                DadosControlePallet dadosControlePallet = new DadosControlePallet(codigoCliente, codigoTransportador, codigoFilial)
                {
                    ResponsavelPallet = responsavelPallet,
                    TipoEstoquePallet = TipoEstoquePallet.Movimentacao,
                };

                Dominio.Entidades.Embarcador.GestaoPallet.ControleEstoquePallet controleEstoquePallet = servicoControleEstoquePallet.AdicionarEstoque(dadosControlePallet);

                Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet movimentacaoEstoquePallet = servicoMovimentacaoPallet.AdicionarMovimentacaoPalletManual(xmlNotaFiscal, carga, controleEstoquePallet, quantidadePallets, observacao, tipoEntradaSaida);

                unitOfWork.CommitChanges();

                return new JsonpResult(new { movimentacaoEstoquePallet.Codigo });
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAdicionar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Reverter()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.GestaoPallet.MovimentacaoPallet repositorioMovimentacaoPallet = new Repositorio.Embarcador.GestaoPallet.MovimentacaoPallet(unitOfWork);

                Servicos.Embarcador.GestaoPallet.MovimentacaoPallet servicoMovimentacaoPallet = new Servicos.Embarcador.GestaoPallet.MovimentacaoPallet(unitOfWork, Auditado);

                int codigoMovimentacaoPallet = Request.GetIntParam("Codigo");

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet movimentacaoPallet = repositorioMovimentacaoPallet.BuscarPorCodigo(codigoMovimentacaoPallet, false);

                if (movimentacaoPallet == null)
                    throw new ControllerException("Movimentação de pallet não encontrada");

                if (movimentacaoPallet.TipoLancamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLancamento.Automatico)
                    throw new ControllerException("Não é possível reverter movimentação de pallet automática");

                if (movimentacaoPallet.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGestaoPallet.Cancelada)
                    throw new ControllerException("Não é possível reverter movimentação de pallet cancelada");

                Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet ultimaMovimentacaoPallet = repositorioMovimentacaoPallet.BuscarUltimaMovimentacaoPorControlePallet(movimentacaoPallet.ControleEstoquePallet.Codigo);

                if (ultimaMovimentacaoPallet.Codigo != movimentacaoPallet.Codigo)
                    throw new ControllerException("Não é possível reverter movimentações anteriores a última movimentação");

                servicoMovimentacaoPallet.ReverterMovimentacaoPallet(movimentacaoPallet);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, movimentacaoPallet, "Movimentação revertida", unitOfWork);

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
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAtualizar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarDadosLaudo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoMovimentacaoPallet = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.GestaoPallet.MovimentacaoPallet repositorioMovimentacaoPallet = new Repositorio.Embarcador.GestaoPallet.MovimentacaoPallet(unitOfWork);

                Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet movimentacaoPallet = repositorioMovimentacaoPallet.BuscarPorCodigo(codigoMovimentacaoPallet, false);

                if (movimentacaoPallet == null)
                    throw new ControllerException("Movimentação de pallet não encontrada");

                Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoLaudo gestaoDevolucaoLaudo = new Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoLaudo();
                List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoLaudoProduto> gestaoDevolucaoLaudoProdutos = new List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoLaudoProduto>();

                if (movimentacaoPallet.Laudo != null)
                {
                    gestaoDevolucaoLaudo = movimentacaoPallet.Laudo;
                    gestaoDevolucaoLaudoProdutos = movimentacaoPallet.Laudo.Produtos.ToList();
                }

                if (gestaoDevolucaoLaudoProdutos.Count <= 0)
                {
                    Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto repositorioXmlNotaFiscalProduto = new Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto(unitOfWork);
                    List<Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ProdutoQuantidadePallet> produtoQuantidadePallet = repositorioXmlNotaFiscalProduto.BuscarProdutoEQuantidadePorNotaFiscal(movimentacaoPallet.XMLNotaFiscal.Codigo);

                    gestaoDevolucaoLaudoProdutos.AddRange(produtoQuantidadePallet.Select(o => new Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoLaudoProduto()
                    {
                        Produto = o.Produto,
                        QuantidadeOrigem = o.Quantidade
                    }));
                }

                dynamic retorno = new
                {
                    Codigo = gestaoDevolucaoLaudo.Codigo,
                    DataCriacao = gestaoDevolucaoLaudo.DataCriacao.ToString(),
                    NumeroCompensacao = gestaoDevolucaoLaudo.NumeroCompensacao,
                    DataCompensacao = gestaoDevolucaoLaudo.DataCompensacao.ToString(),
                    Valor = gestaoDevolucaoLaudo.Valor.ToString(),
                    Responsavel = new
                    {
                        Codigo = gestaoDevolucaoLaudo.Responsavel?.Codigo.ToString() ?? string.Empty,
                        Descricao = gestaoDevolucaoLaudo.Responsavel?.Descricao ?? string.Empty
                    },
                    Transportador = new
                    {
                        Codigo = gestaoDevolucaoLaudo.Transportador?.Codigo.ToString() ?? movimentacaoPallet.Transportador?.Codigo.ToString() ?? string.Empty,
                        Descricao = gestaoDevolucaoLaudo.Transportador?.Descricao ?? movimentacaoPallet.Transportador?.Descricao ?? string.Empty
                    },
                    Veiculo = new
                    {
                        Codigo = gestaoDevolucaoLaudo.Veiculo?.Codigo.ToString() ?? movimentacaoPallet.Carga?.Veiculo?.Codigo.ToString() ?? string.Empty,
                        Descricao = gestaoDevolucaoLaudo.Veiculo?.Descricao ?? movimentacaoPallet.Carga?.Veiculo?.Descricao ?? string.Empty
                    },
                    Produtos = gestaoDevolucaoLaudoProdutos.Select(p => new
                    {
                        p.Codigo,
                        Produto = new
                        {
                            Codigo = p.Produto?.Codigo ?? 0,
                            Descricao = p.Produto?.Descricao ?? string.Empty,
                            CodigoProdutoEmbarcador = p.Produto?.CodigoProdutoEmbarcador ?? string.Empty
                        },
                        p.QuantidadeOrigem,
                        p.QuantidadeDevolvida,
                        p.QuantidadeAvariada,
                        p.ValorAvariado,
                        p.QuantidadeSobras,
                        p.ValorSobras,
                        p.QuantidadeSemCondicao,
                        p.ValorSemCondicao,
                        p.QuantidadeFalta,
                        p.ValorFalta,
                        p.QuantidadeDescarte,
                        p.QuantidadeManutencao,
                    }).ToList()
                };

                return new JsonpResult(retorno);
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
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> GerarLaudoPallet()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Servicos.Embarcador.GestaoPallet.MovimentacaoPallet servicoMovimentacaoPallet = new Servicos.Embarcador.GestaoPallet.MovimentacaoPallet(unitOfWork, Auditado);

                Repositorio.Embarcador.Devolucao.GestaoDevolucaoLaudo repositorioGestaoDevolucaoLaudo = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoLaudo(unitOfWork);
                Repositorio.Embarcador.GestaoPallet.MovimentacaoPallet repositorioMovimentacaoPallet = new Repositorio.Embarcador.GestaoPallet.MovimentacaoPallet(unitOfWork);

                int codigoMovimentacaoPallet = Request.GetIntParam("CodigoMovimentacao");
                int codigoResponsavel = Request.GetIntParam("Responsavel");
                int codigoTransportador = Request.GetIntParam("Transportador");
                int codigoVeiculo = Request.GetIntParam("Veiculo");
                List<dynamic> produtos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(Request.GetStringParam("Produtos"));

                Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet movimentacaoPallet = repositorioMovimentacaoPallet.BuscarPorCodigo(codigoMovimentacaoPallet, false);

                if (movimentacaoPallet == null)
                    return new JsonpResult(false, "Não foi possível encontrar o registro de movimentação");

                if (movimentacaoPallet.Laudo != null)
                    return new JsonpResult(false, "Já existe um laudo gerado para este registro.");

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoLaudo gestaoDevolucaoLaudo = new Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoLaudo();

                gestaoDevolucaoLaudo.Responsavel = new Repositorio.Usuario(unitOfWork).BuscarPorCodigo(codigoResponsavel);
                gestaoDevolucaoLaudo.Transportador = new Repositorio.Empresa(unitOfWork).BuscarPorCodigo(codigoTransportador);
                gestaoDevolucaoLaudo.Veiculo = new Repositorio.Veiculo(unitOfWork).BuscarPorCodigo(codigoVeiculo);

                int quantidadeEntregue = 0;
                int quantidadeManutencao = 0;
                int quantidadeDescarte = 0;
                gestaoDevolucaoLaudo.Produtos = new List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoLaudoProduto>();
                foreach (dynamic produto in produtos)
                {
                    Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoLaudoProduto produtoLaudo = new Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoLaudoProduto();

                    produtoLaudo.Laudo = gestaoDevolucaoLaudo;
                    produtoLaudo.Produto = new Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador() { Codigo = produto.Produto };
                    produtoLaudo.QuantidadeOrigem = ((string)produto.QuantidadeOrigem).ToDecimal();
                    produtoLaudo.QuantidadeDevolvida = ((string)produto.QuantidadeDevolvida).ToDecimal();
                    produtoLaudo.QuantidadeAvariada = ((string)produto.QuantidadeAvariada).ToDecimal();
                    produtoLaudo.ValorAvariado = ((string)produto.ValorAvariado).ToDecimal();
                    produtoLaudo.QuantidadeSobras = ((string)produto.QuantidadeSobras).ToDecimal();
                    produtoLaudo.ValorSobras = ((string)produto.ValorSobras).ToDecimal();
                    produtoLaudo.QuantidadeSemCondicao = ((string)produto.QuantidadeSemCondicao).ToDecimal();
                    produtoLaudo.ValorSemCondicao = ((string)produto.ValorSemCondicao).ToDecimal();
                    produtoLaudo.QuantidadeFalta = ((string)produto.QuantidadeFalta).ToDecimal();
                    produtoLaudo.ValorFalta = ((string)produto.ValorFalta).ToDecimal();
                    produtoLaudo.QuantidadeDescarte = ((string)produto.QuantidadeDescarte).ToDecimal();
                    produtoLaudo.QuantidadeManutencao = ((string)produto.QuantidadeManutencao).ToDecimal();

                    quantidadeEntregue += (int)produtoLaudo.QuantidadeDevolvida;
                    quantidadeManutencao += (int)produtoLaudo.QuantidadeManutencao;
                    quantidadeDescarte += (int)produtoLaudo.QuantidadeDescarte;

                    gestaoDevolucaoLaudo.Produtos.Add(produtoLaudo);
                }

                repositorioGestaoDevolucaoLaudo.Inserir(gestaoDevolucaoLaudo);

                movimentacaoPallet.Laudo = gestaoDevolucaoLaudo;

                servicoMovimentacaoPallet.AlterarSituacao(movimentacaoPallet, SituacaoGestaoPallet.Concluido);

                DadosFinalizarMovimentacaoPallet dadosFinalizarMovimentacaoPallet = new DadosFinalizarMovimentacaoPallet()
                {
                    Filial = movimentacaoPallet.Filial,
                    XMLNotaFiscal = movimentacaoPallet.XMLNotaFiscal,
                    Carga = movimentacaoPallet.Carga,
                    QuantidadeDevolvida = quantidadeEntregue,
                    QuantidadeManutencao = quantidadeManutencao,
                    QuantidadeDescarte = quantidadeDescarte,
                    Observacao = "Adicionado ao concluir a Informar recebimento de Transferência"
                };

                servicoMovimentacaoPallet.FinalizarMovimentacaoPallet(dadosFinalizarMovimentacaoPallet);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao finalizar o recebimento da Transferência.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarResponsavelManual()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.GestaoPallet.MovimentacaoPallet repositorioMovimentacaoPallet = new Repositorio.Embarcador.GestaoPallet.MovimentacaoPallet(unitOfWork);
                Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);

                Servicos.Embarcador.GestaoPallet.MovimentacaoPallet servicoMovimentacaoPallet = new Servicos.Embarcador.GestaoPallet.MovimentacaoPallet(unitOfWork, Auditado);

                int codigoMovimentacaoPallet = Request.GetIntParam("Codigo");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelPallet responsavelMovimentacaoPallet = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelPallet>("ResponsavelPallet");

                int codigoFilial = Request.GetIntParam("Filial");
                long codigoCliente = Request.GetLongParam("Cliente");

                Dominio.Entidades.Embarcador.Filiais.Filial filial = null;
                Dominio.Entidades.Cliente cliente = null;

                if (responsavelMovimentacaoPallet == ResponsavelPallet.Cliente)
                {
                    cliente = repositorioCliente.BuscarPorCPFCNPJ(codigoCliente);

                    if (cliente == null)
                        throw new ControllerException("Cliente não encontrado!");
                }
                else if (responsavelMovimentacaoPallet == ResponsavelPallet.Filial)
                {
                    filial = repositorioFilial.BuscarPorCodigo(codigoFilial);

                    if (filial == null)
                        throw new ControllerException("Filial não encontrada!");
                }

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet movimentacaoPallet = repositorioMovimentacaoPallet.BuscarPorCodigo(codigoMovimentacaoPallet, false);

                if (movimentacaoPallet == null)
                    throw new ControllerException("Movimentação de pallet não encontrada");

                servicoMovimentacaoPallet.InformarMudancaResponsavelManual(movimentacaoPallet, responsavelMovimentacaoPallet, filial, cliente);

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
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAtualizar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarValePallet(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoMovimentacaoPallet = Request.GetIntParam("CodigoMovimentacao");

                Repositorio.Embarcador.GestaoPallet.MovimentacaoPallet repositorioMovimentacaoPallet = new Repositorio.Embarcador.GestaoPallet.MovimentacaoPallet(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet movimentacaoPallet = await repositorioMovimentacaoPallet.BuscarPorCodigoAsync(codigoMovimentacaoPallet, false);

                var detalhesValePallet = new
                {
                    Codigo = movimentacaoPallet.ValePallet?.Codigo ?? 0,
                    NumeroNotaFiscalOrigem = movimentacaoPallet.XMLNotaFiscal.Numero,
                    SerieNotaFiscalOrigem = movimentacaoPallet.XMLNotaFiscal.Serie,
                    Cliente = movimentacaoPallet.Cliente.Descricao,
                    NumeroValePallet = movimentacaoPallet.ValePallet?.NumeroValePallet ?? 0,
                    DataVencimento = movimentacaoPallet.ValePallet?.DataVencimento.ToDateTimeString() ?? string.Empty,
                };

                return new JsonpResult(detalhesValePallet);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ConfirmarValePallet(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoValePallet = Request.GetIntParam("Codigo");
                int codigoMovimentacaoPallet = Request.GetIntParam("CodigoMovimentacao");

                Repositorio.Embarcador.GestaoPallet.RecebimentoValePallet repositorioRecebimentoValePallet = new Repositorio.Embarcador.GestaoPallet.RecebimentoValePallet(unitOfWork, cancellationToken);
                Repositorio.Embarcador.GestaoPallet.MovimentacaoPallet repositorioMovimentacaoPallet = new Repositorio.Embarcador.GestaoPallet.MovimentacaoPallet(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.GestaoPallet.RecebimentoValePallet recebimentoValePallet = await repositorioRecebimentoValePallet.BuscarPorCodigoAsync(codigoValePallet, false);
                Dominio.Entidades.Embarcador.GestaoPallet.MovimentacaoPallet movimentacaoPallet = await repositorioMovimentacaoPallet.BuscarPorCodigoAsync(codigoMovimentacaoPallet, false);

                if (movimentacaoPallet == null)
                    return new JsonpResult(false, "Não foi possível encontrar o registro de movimentação");

                recebimentoValePallet ??= new Dominio.Entidades.Embarcador.GestaoPallet.RecebimentoValePallet();

                await unitOfWork.StartAsync(cancellationToken);

                recebimentoValePallet.NumeroValePallet = Request.GetIntParam("NumeroValePallet");
                recebimentoValePallet.DataVencimento = Request.GetDateTimeParam("DataVencimento");

                if (codigoValePallet > 0)
                    await repositorioRecebimentoValePallet.AtualizarAsync(recebimentoValePallet);
                else
                    await repositorioRecebimentoValePallet.InserirAsync(recebimentoValePallet);

                movimentacaoPallet.ValePallet = recebimentoValePallet;

                await repositorioMovimentacaoPallet.AtualizarAsync(movimentacaoPallet);

                if (codigoValePallet == 0)
                    new Servicos.Embarcador.GestaoPallet.MovimentacaoPallet(unitOfWork, Auditado).InformarMudancaResponsavel(movimentacaoPallet);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoAdicionarDados);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private async Task<Models.Grid.Grid> ObterGridPesquisa(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.GestaoPallet.MovimentacaoPallet repositorioMovimentacaoPallet = new(unitOfWork, cancellationToken);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPaletes configuracaoPaletes = await new Repositorio.Embarcador.Configuracoes.ConfiguracaoPaletes(unitOfWork, cancellationToken).BuscarConfiguracaoPadraoAsync();

            Models.Grid.Grid grid = new(Request)
            {
                header = []
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("RegraPallet", false);
            grid.AdicionarCabecalho("Situacao", false);
            grid.AdicionarCabecalho("TipoLancamento", false);
            grid.AdicionarCabecalho("TipoMovimentacao", false);
            grid.AdicionarCabecalho("CodigoCargaPedido", false);
            grid.AdicionarCabecalho("Número Nota Fiscal", "NumeroNotaFiscal", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Carga", "Carga", 7, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Quantidade Pallets", "QuantidadePallets", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Data Recebimento NF", "DataRecebimentoNotaFiscal", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Data Recebimento", "DataRecebimento", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Regra Pallet", "RegraPalletDescricao", 5, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Situação", "SituacaoDescricao", 5, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("ResponsavelMovimentacaoPallet", false);
            grid.AdicionarCabecalho("Responsável Pendência", nameof(ConsultaMovimentacaoPallet.ResponsavelPendencia), 5, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Origem", "Origem", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Destino", "Destino", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Filial", "DescricaoFilial", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Transportador", "DescricaoTransportador", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Cliente", "DescricaoCliente", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Observação", "Observacao", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Quebra Regra", "DescricaoQuebraRegra", 5, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Tipo Lançamento", "DescricaoTipoLancamento", 5, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Tipo Movimentação", "DescricaoTipoMovimentacao", 5, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Tipo Devolução", "DescricaoTipoGestaoDevolucao", 5, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("LeadTime da Devolução", "LeadTimeDevolucaoFormatado", 5, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Situação da Devolução", "SituacaoDevolucao", 5, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Série NFe", nameof(ConsultaMovimentacaoPallet.SerieNfe), 5, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Tipos Tomador", nameof(ConsultaMovimentacaoPallet.TiposTomadorDescricao), 5, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Tipos Modal", nameof(ConsultaMovimentacaoPallet.TiposModalDescricao), 5, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("CFOP NFe", nameof(ConsultaMovimentacaoPallet.CfopNfe), 5, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Data Emissão NFe", nameof(ConsultaMovimentacaoPallet.DataEmissaoNfe), 5, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Data Vencimento Vale Pallet", nameof(ConsultaMovimentacaoPallet.DataVencimenoValePallet), 5, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Números Pedido Cliente", nameof(ConsultaMovimentacaoPallet.NumerosPedidoCliente), 5, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Canais Venda", nameof(ConsultaMovimentacaoPallet.CanaisVenda), 5, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Placa Tração", nameof(ConsultaMovimentacaoPallet.PlacaTracao), 5, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Placas Reboque", nameof(ConsultaMovimentacaoPallet.PlacasReboque), 5, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Situação do Canhoto", nameof(ConsultaMovimentacaoPallet.SituacaoCanhotoDescricao), 5, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Situação Vale Pallet", nameof(ConsultaMovimentacaoPallet.SituacaoValePallet), 5, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Data Recebimento Canhoto", nameof(ConsultaMovimentacaoPallet.DataRecebimentoCanhoto), 5, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Data Digitalização Canhoto", nameof(ConsultaMovimentacaoPallet.DataDigitalizacaoCanhoto), 5, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Data Recebimeto Vale Pallet", nameof(ConsultaMovimentacaoPallet.DataRecebimentoValePallet), 5, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Número Atendimento", nameof(ConsultaMovimentacaoPallet.NumeroAtendimento), 5, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Situação Atendimento", nameof(ConsultaMovimentacaoPallet.SituacaoAtendimentoDescricao), 5, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Escritório de Vendas", nameof(ConsultaMovimentacaoPallet.EscritorioVendas), 5, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Dias para Vencer", nameof(ConsultaMovimentacaoPallet.DiasVencer), 5, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Data Laudo", nameof(ConsultaMovimentacaoPallet.DataLaudo), 5, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Número do Laudo", nameof(ConsultaMovimentacaoPallet.NumeroLaudo), 5, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Número NF Permuta", nameof(ConsultaMovimentacaoPallet.NumeroNotaFiscalPermuta), 5, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Série NF Permuta", nameof(ConsultaMovimentacaoPallet.SerieNotaFiscalPermuta), 5, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Número NFD", nameof(ConsultaMovimentacaoPallet.NumeroNotaFiscalDevolucao), 5, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Série NFD", nameof(ConsultaMovimentacaoPallet.SerieNotaFiscalDevolucao), 5, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Origem NFD", nameof(ConsultaMovimentacaoPallet.OrigemNotaFiscalDevolucaoDescricao), 5, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Responsável Devolução", nameof(ConsultaMovimentacaoPallet.ResponsavelDevolucao), 5, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Data Coleta", nameof(ConsultaMovimentacaoPallet.DataColeta), 5, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Data Previsão Chegada", nameof(ConsultaMovimentacaoPallet.DataPrevisaoChegada), 5, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Endereço Coleta", nameof(ConsultaMovimentacaoPallet.EnderecoColeta), 5, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Cidade Coleta", nameof(ConsultaMovimentacaoPallet.CidadeColeta), 5, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Centro de Descarregamento", nameof(ConsultaMovimentacaoPallet.CentroDescarregamento), 5, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Data Recebimento NFD", nameof(ConsultaMovimentacaoPallet.DataRecebimentoNFD), 5, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Código Devolução", nameof(ConsultaMovimentacaoPallet.CodigoDevolucao), 5, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Data Agendamento Devolução", nameof(ConsultaMovimentacaoPallet.DataAgendamentoDevolucao), 5, Models.Grid.Align.center, false, false);

            Models.Grid.GridPreferencias gridPreferencias = new(unitOfWork, "ControlePallet/Pesquisa", "grid-gestao-pallet-controle-pallet");
            grid.AplicarPreferenciasGrid(gridPreferencias.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));

            Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControlePallet.FiltroPesquisaControlePallet filtroPesquisa = ObterFiltrosPesquisaControlePallet(configuracaoPaletes);

            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);

            int totalRegistros = await repositorioMovimentacaoPallet.ContarControlePalletAsync(filtroPesquisa, parametrosConsulta);
            IList<ConsultaMovimentacaoPallet> listaConsultaControlePallet = totalRegistros > 0 ? await repositorioMovimentacaoPallet.ObterControlePalletAsync(filtroPesquisa, parametrosConsulta) : [];

            grid.setarQuantidadeTotal(totalRegistros);
            grid.AdicionaRows(listaConsultaControlePallet);

            return grid;
        }

        private async Task<Models.Grid.Grid> ObterGridPesquisaSaldo(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.GestaoPallet.MovimentacaoPallet repositorioMovimentacaoPallet = new Repositorio.Embarcador.GestaoPallet.MovimentacaoPallet(unitOfWork, cancellationToken);

            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Responsável", "DescricaoResponsavel", 40, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Pallets Pendentes", "PalettsPendente", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Pallets Disponível", "QuantidadeTotalPallets", 20, Models.Grid.Align.left, true);

            Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControlePallet.FiltroPesquisaSaldoControlePallet filtroPesquisa = ObterFiltrosPesquisaSaldoControlePallet();

            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
            int totalRegistros = await repositorioMovimentacaoPallet.ContarSaldoControlePalletAsync(filtroPesquisa, parametrosConsulta);
            IList<Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControlePallet.ConsultaSaldoControlePallet> listaConsultaSaldoControlePallet = (totalRegistros > 0) ? await repositorioMovimentacaoPallet.ObterSaldoControlePalletAsync(filtroPesquisa, parametrosConsulta) : new List<Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControlePallet.ConsultaSaldoControlePallet>();

            grid.setarQuantidadeTotal(totalRegistros);
            grid.AdicionaRows(listaConsultaSaldoControlePallet);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControlePallet.FiltroPesquisaControlePallet ObterFiltrosPesquisaControlePallet(Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPaletes configuracaoPaletes = null)
        {
            Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControlePallet.FiltroPesquisaControlePallet filtroPesquisa = new Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControlePallet.FiltroPesquisaControlePallet
            {
                NotaFiscal = Request.GetIntParam("NotaFiscal"),
                Carga = Request.GetStringParam("Carga"),
                Filial = Request.GetIntParam("Filial"),
                DataInicialCriacaoCarga = Request.GetDateTimeParam("DataInicialCriacaoCarga"),
                DataFinalCriacaoCarga = Request.GetDateTimeParam("DataFinalCriacaoCarga"),
                DataInicialNotaFiscal = Request.GetDateTimeParam("DataInicialNotaFiscal"),
                DataFinalNotaFiscal = Request.GetDateTimeParam("DataFinalNotaFiscal"),
                Transportador = Request.GetIntParam("Transportador"),
                Cliente = Request.GetLongParam("Cliente"),
                Situacao = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGestaoPallet>("Situacao"),
                RegraPallet = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.RegraPallet>("RegraPallet"),
                ResponsavelPallet = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelPallet>("ResponsavelPallet"),
                UFOrigem = Request.GetListParam<string>("UFOrigem"),
                UFDestino = Request.GetListParam<string>("UFDestino"),
                EscritorioVendas = Request.GetStringParam("EscritorioVendas")
            };

            if (configuracaoPaletes != null)
            {
                filtroPesquisa.DiasLimiteParaDevolucao = configuracaoPaletes.LimiteDiasParaDevolucaoDePallet;
                filtroPesquisa.DataLimiteGeracaoDevolucao = DateTime.Now.AddDays(-configuracaoPaletes.LimiteDiasParaDevolucaoDePallet);
            }

            return filtroPesquisa;
        }

        private Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControlePallet.FiltroPesquisaSaldoControlePallet ObterFiltrosPesquisaSaldoControlePallet()
        {
            Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControlePallet.FiltroPesquisaSaldoControlePallet filtroPesquisa = new Dominio.ObjetosDeValor.Embarcador.GestaoPallet.ControlePallet.FiltroPesquisaSaldoControlePallet
            {
                Filial = Request.GetIntParam("Filial"),
                Transportador = Request.GetIntParam("Transportador"),
                Cliente = Request.GetLongParam("Cliente"),
                ResponsavelMovimentacaoPallet = Request.GetNullableEnumParam<ResponsavelPallet>("ResponsavelPallet")
            };

            return filtroPesquisa;
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Carga")
                return "Carga.CAR_CODIGO_CARGA_EMBARCADOR";

            if (propriedadeOrdenar == "Cliente")
                return "Cliente.CLI_NOME";

            if (propriedadeOrdenar == "QuantidadePallets")
                return "MovimentacaoPallet.MPT_QUANTIDADE_PALLETS";

            if (propriedadeOrdenar == "DataRecebimentoNotaFiscal")
                return "XmlNotaFiscal.NF_DATA_RECEBIMENTO";

            if (propriedadeOrdenar == "DescricaoResponsavel")
                return "Responsavel";

            return propriedadeOrdenar;
        }

        #endregion
    }
}