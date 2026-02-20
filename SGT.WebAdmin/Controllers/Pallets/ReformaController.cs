using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;


namespace SGT.WebAdmin.Controllers.Pallets
{
    [CustomAuthorize(new string[] { }, "Pallets/Reforma")]
    public class ReformaController : BaseController
    {
        #region Construtores

        public ReformaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> AdicionarEnvio()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var repositorioReforma = new Repositorio.Embarcador.Pallets.Reforma.ReformaPallet(unitOfWork);
                var reforma = new Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPallet();
                List<Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPalletEnvioQuantidade> quantidadesEnviadas;

                try
                {
                    reforma.Numero = repositorioReforma.BuscarProximoNumero();
                    reforma.Situacao = SituacaoReformaPallet.AguardandoNfeSaida;
                    reforma.Envio = ObterEnvioAdicionar(unitOfWork);

                    quantidadesEnviadas = ObterEnvioQuantidadesAdicionar(unitOfWork, reforma.Envio);
                }
                catch (Exception excecao)
                {
                    return new JsonpResult(false, false, excecao.Message);
                }

                unitOfWork.Start();

                var repositorioEnvioQuantidade = new Repositorio.Embarcador.Pallets.Reforma.ReformaPalletEnvioQuantidade(unitOfWork);

                repositorioReforma.Inserir(reforma, Auditado);

                foreach (var quantidadeEnviada in quantidadesEnviadas)
                {
                    repositorioEnvioQuantidade.Inserir(quantidadeEnviada);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, false, "Ocorreu uma falha ao enviar a reforma de pallets.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarNfeRetorno()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var codigo = Request.GetIntParam("Codigo");
                var repositorio = new Repositorio.Embarcador.Pallets.Reforma.ReformaPallet(unitOfWork);
                var reforma = repositorio.BuscarPorCodigo(codigo);

                if (reforma == null)
                    return new JsonpResult(false, false, "Não foi possível encontrar o registro.");

                var repositorioReformaNfeRetorno = new Repositorio.Embarcador.Pallets.Reforma.ReformaPalletNfeRetorno(unitOfWork);
                var numero = Request.GetIntParam("Numero");
                var numeroSerie = Request.GetIntParam("Serie");

                if (repositorioReformaNfeRetorno.BuscarPorReformaNumeroSerie(reforma.Codigo, numero, numeroSerie) != null)
                    return new JsonpResult(false, false, "Já foi cadastrada uma NF-e com esse número e série.");

                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal;

                try
                {
                    var valorTotal = (Request.GetIntParam("Quantidade") * Request.GetDecimalParam("ValorUnitario"));

                    xmlNotaFiscal = new Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal()
                    {
                        CNPJTranposrtador = "",
                        DataEmissao = Request.GetDateTimeParam("DataEmissao"),
                        Destinatario = ObterDestinatario(unitOfWork),
                        Emitente = ObterFornecedor(unitOfWork, Request.GetDoubleParam("Fornecedor")),
                        Numero = numero,
                        Serie = numeroSerie.ToString(),
                        PlacaVeiculoNotaFiscal = "",
                        QuantidadePallets = Request.GetIntParam("Quantidade"),
                        TipoDocumento = TipoDocumento.Outros,
                        TipoOperacaoNotaFiscal = TipoOperacaoNotaFiscal.Entrada,
                        Valor = valorTotal,
                        ValorTotalProdutos = valorTotal,
                        XML = ""
                    };
                }
                catch (Exception excecao)
                {
                    return new JsonpResult(false, false, excecao.Message);
                }

                unitOfWork.Start();

                var repositorioXmlNfe = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

                var nfeRetorno = new Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPalletNfeRetorno()
                {
                    XmlNotaFiscal = xmlNotaFiscal,
                    ReformaPallet = reforma
                };

                repositorioXmlNfe.Inserir(xmlNotaFiscal, Auditado);
                repositorioReformaNfeRetorno.Inserir(nfeRetorno, Auditado);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, reforma, $"Adicionada NF-e de retorno nº {xmlNotaFiscal.Numero}", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    ListaNfeRetorno = ObterNfeRetorno(reforma)
                });
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, false, "Ocorreu uma falha ao adicionar a NF-e de Retorno.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarNfeSaida()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var codigo = Request.GetIntParam("Codigo");
                var repositorio = new Repositorio.Embarcador.Pallets.Reforma.ReformaPallet(unitOfWork);
                var reforma = repositorio.BuscarPorCodigo(codigo);

                if (reforma == null)
                    return new JsonpResult(false, false, "Não foi possível encontrar o registro.");

                var repositorioReformaNfeSaida = new Repositorio.Embarcador.Pallets.Reforma.ReformaPalletNfeSaida(unitOfWork);
                var numero = Request.GetIntParam("Numero");
                var numeroSerie = Request.GetIntParam("Serie");

                if (repositorioReformaNfeSaida.BuscarPorReformaNumeroSerie(reforma.Codigo, numero, numeroSerie) != null)
                    return new JsonpResult(false, false, "Já foi cadastrada uma NF-e com esse número e série.");

                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal;

                try
                {
                    var valorTotal = (Request.GetIntParam("Quantidade") * Request.GetDecimalParam("ValorUnitario"));

                    xmlNotaFiscal = new Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal()
                    {
                        CNPJTranposrtador = "",
                        DataEmissao = Request.GetDateTimeParam("DataEmissao"),
                        Destinatario = ObterDestinatario(unitOfWork),
                        Emitente = ObterFornecedor(unitOfWork, Request.GetDoubleParam("Fornecedor")),
                        Numero = numero,
                        Serie = numeroSerie.ToString(),
                        PlacaVeiculoNotaFiscal = "",
                        QuantidadePallets = Request.GetIntParam("Quantidade"),
                        TipoDocumento = TipoDocumento.Outros,
                        TipoOperacaoNotaFiscal = TipoOperacaoNotaFiscal.Saida,
                        Valor = valorTotal,
                        ValorTotalProdutos = valorTotal,
                        XML = ""
                    };
                }
                catch (Exception excecao)
                {
                    return new JsonpResult(false, false, excecao.Message);
                }

                unitOfWork.Start();

                var repositorioXmlNfe = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

                var nfeSaida = new Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPalletNfeSaida()
                {
                    XmlNotaFiscal = xmlNotaFiscal,
                    ReformaPallet = reforma
                };

                repositorioXmlNfe.Inserir(xmlNotaFiscal, Auditado);
                repositorioReformaNfeSaida.Inserir(nfeSaida, Auditado);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, reforma, $"Adicionada NF-e de saída nº {xmlNotaFiscal.Numero}", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    ListaNfeSaida = ObterNfeSaida(reforma)
                });
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, false, "Ocorreu uma falha ao adicionar a NF-e de Saída.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarNfsRetorno()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var codigo = Request.GetIntParam("Codigo");
                var repositorio = new Repositorio.Embarcador.Pallets.Reforma.ReformaPallet(unitOfWork);
                var reforma = repositorio.BuscarPorCodigo(codigo);

                if (reforma == null)
                    return new JsonpResult(false, false, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                var repositorioReformaNfsRetorno = new Repositorio.Embarcador.Pallets.Reforma.ReformaPalletNfsRetorno(unitOfWork);
                var numero = Request.GetIntParam("Numero");
                var numeroSerie = Request.GetIntParam("Serie");

                if (repositorioReformaNfsRetorno.BuscarPorReformaNumeroSerie(reforma.Codigo, numero, numeroSerie) != null)
                    return new JsonpResult(false, false, "Já foi cadastrada uma NFS com esse número e série.");

                var aliquotaIss = Request.GetDecimalParam("AliquotaISS");

                if (aliquotaIss > 100)
                    return new JsonpResult(false, false, "A aliquota do ISS não pode ser superior a 100%.");

                var percentualRetencao = Request.GetDecimalParam("PercentualRetencao");

                if (percentualRetencao > 100)
                    return new JsonpResult(false, false, "O percentual de retenção não pode ser superior a 100%.");

                var nfsRetorno = new Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPalletNfsRetorno()
                {
                    AliquotaIss = aliquotaIss,
                    DataEmissao = Request.GetDateTimeParam("DataEmissao"),
                    IncluirIssBaseCalculo = Request.GetBoolParam("IncluirValorBC"),
                    Numero = numero,
                    ReformaPallet = reforma,
                    Serie = numeroSerie,
                    Observacoes = Request.Params("Observacao") ?? string.Empty,
                    PercentualRetencao = percentualRetencao,
                    ValorBaseCalculo = Request.GetDecimalParam("BaseCalculo"),
                    ValorIss = Request.GetDecimalParam("ValorISS"),
                    ValorPrestacaoServico = Request.GetDecimalParam("ValorPrestacaoServico"),
                    ValorRetido = Request.GetDecimalParam("ValorRetencao")
                };

                repositorioReformaNfsRetorno.Inserir(nfsRetorno, Auditado);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, reforma, $"Adicionada NFS de retorno nº {nfsRetorno.Numero}", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    ListaNfsRetorno = ObterNfsRetorno(reforma)
                });
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, false, "Ocorreu uma falha ao adicionar a NFS de Retorno.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var codigo = Request.GetIntParam("Codigo");
                var repositorio = new Repositorio.Embarcador.Pallets.Reforma.ReformaPallet(unitOfWork);
                var reforma = repositorio.BuscarPorCodigo(codigo);

                if (reforma == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    reforma.Codigo,
                    reforma.Situacao,
                    Envio = ObterEnvio(reforma),
                    ListaNfeRetorno = ObterNfeRetorno(reforma),
                    ListaNfeSaida = ObterNfeSaida(reforma),
                    ListaNfsRetorno = ObterNfsRetorno(reforma),
                    QuantidadesEnviadas = ObterEnvioQuantidades(reforma),
                    Resumo = ObterResumo(reforma)
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, false, "Ocorreu uma falha ao obter os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarSituacoes()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var repositorio = new Repositorio.Embarcador.Pallets.SituacaoDevolucaoPallet(unitOfWork);
                var situacoes = repositorio.BuscarAtivosPorSituacaoPalletAvariado();

                return new JsonpResult(
                    (
                        from situacao in situacoes
                        select new
                        {
                            situacao.Codigo,
                            situacao.Descricao
                        }
                    ).ToList()
                );
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, false, "Ocorreu uma falha ao obter os dados das situações de devolução de pallets.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> CancelarPorCodigo()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                var codigo = Request.GetIntParam("Codigo");
                var repositorio = new Repositorio.Embarcador.Pallets.Reforma.ReformaPallet(unitOfWork);
                var reforma = repositorio.BuscarPorCodigo(codigo);

                if (reforma == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (!IsSituacaoPermiteCancelamento(reforma))
                    return new JsonpResult(false, true, "A situação da reforma não permite realizar o cancelamento.");

                var listaXmlNotaFiscal = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

                if (reforma.Situacao == SituacaoReformaPallet.AguardandoNfeSaida)
                {
                    reforma.Situacao = SituacaoReformaPallet.CanceladaNfeSaida;

                    var repositorioNfeSaida = new Repositorio.Embarcador.Pallets.Reforma.ReformaPalletNfeSaida(unitOfWork);

                    foreach (var reformaNfeSaida in reforma.NotasFiscaisSaida)
                    {
                        repositorioNfeSaida.Deletar(reformaNfeSaida, Auditado);
                        listaXmlNotaFiscal.Add(reformaNfeSaida.XmlNotaFiscal);
                    }
                }
                else
                {
                    reforma.Situacao = SituacaoReformaPallet.CanceladaRetorno;

                    var repositorioNfeRetorno = new Repositorio.Embarcador.Pallets.Reforma.ReformaPalletNfeRetorno(unitOfWork);

                    foreach (var reformaNfeRetorno in reforma.NotasFiscaisRetorno)
                    {
                        repositorioNfeRetorno.Deletar(reformaNfeRetorno, Auditado);
                        listaXmlNotaFiscal.Add(reformaNfeRetorno.XmlNotaFiscal);
                    }

                    TipoOperacaoMovimentacaoEstoquePallet tipo = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? TipoOperacaoMovimentacaoEstoquePallet.ReformaAvariaPorTransportador : TipoOperacaoMovimentacaoEstoquePallet.ReformaAvaria;

                    InserirMovimentacaoEstoque(unitOfWork, reforma, tipo);
                }

                repositorio.Atualizar(reforma);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, reforma, "Reforma cancelada", unitOfWork);

                unitOfWork.CommitChanges();

                ExcluirTodosXmlNfe(listaXmlNotaFiscal);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, false, "Ocorreu uma falha ao cancelar a reforma de pallet.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DownloadXmlNfeRetorno()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var codigo = Request.GetIntParam("Codigo");
                var repositorio = new Repositorio.Embarcador.Pallets.Reforma.ReformaPalletNfeRetorno(unitOfWork);
                var reformaNfeRetorno = repositorio.BuscarPorCodigo(codigo);

                if (reformaNfeRetorno == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (String.IsNullOrEmpty(reformaNfeRetorno.XmlNotaFiscal.XML))
                    return new JsonpResult(false, true, "Não foi possível encontrar o arquivo XML.");

                var arquivoBinario = System.Text.Encoding.ASCII.GetBytes(reformaNfeRetorno.XmlNotaFiscal.XML);
                var nomeArquivo = $"{reformaNfeRetorno.XmlNotaFiscal.Chave}.xml";
                var tipoArquivo = "text/xml";

                return Arquivo(arquivoBinario, tipoArquivo, nomeArquivo);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, false, "Ocorreu uma falha ao fazer o download do XML.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DownloadXmlNfeSaida()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var codigo = Request.GetIntParam("Codigo");
                var repositorio = new Repositorio.Embarcador.Pallets.Reforma.ReformaPalletNfeSaida(unitOfWork);
                var reformaNfeSaida = repositorio.BuscarPorCodigo(codigo);

                if (reformaNfeSaida == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var arquivoBinario = System.Text.Encoding.ASCII.GetBytes(reformaNfeSaida.XmlNotaFiscal.XML);
                var nomeArquivo = $"{reformaNfeSaida.XmlNotaFiscal.Chave}.xml";
                var tipoArquivo = "text/xml";

                return Arquivo(arquivoBinario, tipoArquivo, nomeArquivo);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, false, "Ocorreu uma falha ao fazer o download do XML.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirNfeRetorno()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var codigo = Request.GetIntParam("Codigo");
                var repositorio = new Repositorio.Embarcador.Pallets.Reforma.ReformaPalletNfeRetorno(unitOfWork);
                var reformaNfeRetorno = repositorio.BuscarPorCodigo(codigo);

                if (reformaNfeRetorno == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (reformaNfeRetorno.ReformaPallet.Situacao != SituacaoReformaPallet.AguardandoRetorno)
                    return new JsonpResult(false, "Situação não permite excluir NF-e de Retorno.");

                var repositorioXmlNfe = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
                var xmlNotaFiscal = reformaNfeRetorno.XmlNotaFiscal;

                unitOfWork.Start();

                repositorio.Deletar(reformaNfeRetorno, Auditado);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, reformaNfeRetorno.ReformaPallet, $"Excluída a NF-e de retorno nº {reformaNfeRetorno.XmlNotaFiscal.Numero}", unitOfWork);

                unitOfWork.CommitChanges();

                ExcluirXmlNfe(reformaNfeRetorno.XmlNotaFiscal);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao deletar a NF-e de Retorno.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirNfeSaida()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var codigo = Request.GetIntParam("Codigo");
                var repositorio = new Repositorio.Embarcador.Pallets.Reforma.ReformaPalletNfeSaida(unitOfWork);
                var reformaNfeSaida = repositorio.BuscarPorCodigo(codigo);

                if (reformaNfeSaida == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (reformaNfeSaida.ReformaPallet.Situacao != SituacaoReformaPallet.AguardandoNfeSaida)
                    return new JsonpResult(false, "Situação não permite excluir NF-e de Saída.");

                var repositorioXmlNfe = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
                var xmlNotaFiscal = reformaNfeSaida.XmlNotaFiscal;

                unitOfWork.Start();

                repositorio.Deletar(reformaNfeSaida, Auditado);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, reformaNfeSaida.ReformaPallet, $"Excluída a NF-e de saída nº {reformaNfeSaida.XmlNotaFiscal.Numero}", unitOfWork);

                unitOfWork.CommitChanges();

                ExcluirXmlNfe(reformaNfeSaida.XmlNotaFiscal);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao deletar a NF-e de Saída.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirNfsRetorno()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var codigo = Request.GetIntParam("Codigo");
                var repositorio = new Repositorio.Embarcador.Pallets.Reforma.ReformaPalletNfsRetorno(unitOfWork);
                var reformaNfsRetorno = repositorio.BuscarPorCodigo(codigo);

                if (reformaNfsRetorno == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (reformaNfsRetorno.ReformaPallet.Situacao != SituacaoReformaPallet.AguardandoRetorno)
                    return new JsonpResult(false, "Situação não permite excluir NFS de Retorno.");

                unitOfWork.Start();

                repositorio.Deletar(reformaNfsRetorno, Auditado);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, reformaNfsRetorno.ReformaPallet, $"Excluída a NFS de saída nº {reformaNfsRetorno.Numero}", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao deletar a NFS de Retorno.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                var grid = ObterGridPesquisa();

                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
        }

        public async Task<IActionResult> Finalizar()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var codigo = Request.GetIntParam("Codigo");
                var repositorio = new Repositorio.Embarcador.Pallets.Reforma.ReformaPallet(unitOfWork);
                var reforma = repositorio.BuscarPorCodigo(codigo);

                if (reforma == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (reforma.Situacao != SituacaoReformaPallet.AguardandoRetorno)
                    return new JsonpResult(false, "Situação não permite finalizar a reforma.");

                if (reforma.NotasFiscaisRetorno.Count == 0)
                    return new JsonpResult(false, "Nenhuma de NF-e de Retorno foi importada ou cadastrada.");

                if (reforma.NotasServicoRetorno.Count == 0)
                    return new JsonpResult(false, "Nenhuma de NFS de Retorno foi cadastrada.");

                unitOfWork.Start();

                reforma.Initialize();

                reforma.DataRetorno = DateTime.Now;
                reforma.Situacao = SituacaoReformaPallet.Finalizada;

                repositorio.Atualizar(reforma);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, reforma, $"Reforma finalizada", unitOfWork);

                TipoOperacaoMovimentacaoEstoquePallet tipo = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? TipoOperacaoMovimentacaoEstoquePallet.ReformaTransportador : TipoOperacaoMovimentacaoEstoquePallet.ReformaFilial;

                InserirMovimentacaoEstoque(unitOfWork, reforma, tipo);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao finalizar a reforma.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> FinalizarNfeSaida()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var codigo = Request.GetIntParam("Codigo");
                var repositorio = new Repositorio.Embarcador.Pallets.Reforma.ReformaPallet(unitOfWork);
                var reforma = repositorio.BuscarPorCodigo(codigo);

                if (reforma == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (reforma.Situacao != SituacaoReformaPallet.AguardandoNfeSaida)
                    return new JsonpResult(false, "Situação não permite finalizar a importação de NF-e de Saída.");

                if (reforma.NotasFiscaisSaida.Count == 0)
                    return new JsonpResult(false, "Nenhuma de NF-e de Saída foi importada.");

                unitOfWork.Start();

                reforma.Initialize();

                reforma.Situacao = SituacaoReformaPallet.AguardandoRetorno;

                repositorio.Atualizar(reforma);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, reforma, $"Importação de NF-e de Saída finalizada", unitOfWork);

                TipoOperacaoMovimentacaoEstoquePallet tipo = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? TipoOperacaoMovimentacaoEstoquePallet.AvariaReformaPorTransportador : TipoOperacaoMovimentacaoEstoquePallet.AvariaReforma;

                InserirMovimentacaoEstoque(unitOfWork, reforma, tipo);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao finalizar a importação de NF-e de Saída.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ImportacaoXmlNfeRetorno()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var arquivos = HttpContext.GetFiles("XML");

                if (arquivos.Count == 0)
                    return new JsonpResult(false, true, "Nenhum arquivo selecionado para importação.");

                var codigo = Request.GetIntParam("Codigo");
                var repositorio = new Repositorio.Embarcador.Pallets.Reforma.ReformaPallet(unitOfWork);
                var reforma = repositorio.BuscarPorCodigo(codigo);

                if (reforma == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                int totalNfeRetornoAdicionadas = 0;

                foreach (var arquivo in arquivos)
                {
                    if (AdicionarNfeRetorno(unitOfWork, reforma, arquivo, TipoServicoMultisoftware))
                        totalNfeRetornoAdicionadas++;
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    TotalXml = arquivos.Count,
                    TotalXmlAdicionados = totalNfeRetornoAdicionadas,
                    ListaNfeRetorno = ObterNfeRetorno(reforma)
                });
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao importar os XMLs.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ImportacaoXmlNfeSaida()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var arquivos = HttpContext.GetFiles("XML");

                if (arquivos.Count == 0)
                    return new JsonpResult(false, true, "Nenhum arquivo selecionado para importação.");

                var codigo = Request.GetIntParam("Codigo");
                var repositorio = new Repositorio.Embarcador.Pallets.Reforma.ReformaPallet(unitOfWork);
                var reforma = repositorio.BuscarPorCodigo(codigo);

                if (reforma == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                int totalNfeSaidaAdicionadas = 0;

                foreach (var arquivo in arquivos)
                {
                    if (AdicionarNfeSaida(unitOfWork, reforma, arquivo))
                        totalNfeSaidaAdicionadas++;
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    TotalXml = arquivos.Count,
                    TotalXmlAdicionados = totalNfeSaidaAdicionadas,
                    ListaNfeSaida = ObterNfeSaida(reforma)
                });
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao importar os XMLs.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        #endregion

        #region Métodos Privados

        private bool AdicionarNfeRetorno(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPallet reforma, Servicos.DTO.CustomFile arquivo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.Embarcador.NFe.NFe servicoNFe = new Servicos.Embarcador.NFe.NFe(unitOfWork);
            System.IO.StreamReader leitorXml = new System.IO.StreamReader(arquivo.InputStream);

            Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebservice = repConfiguracaoWebService.BuscarConfiguracaoPadrao();

            if (!servicoNFe.BuscarDadosNotaFiscal(out string erro, out Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, leitorXml, unitOfWork, null, true, false, false, tipoServicoMultisoftware, false, false, null, null, null, configuracaoWebservice?.CadastroAutomaticoPessoaExterior ?? false))
                return false;

            var repositorioReformaNfeRetorno = new Repositorio.Embarcador.Pallets.Reforma.ReformaPalletNfeRetorno(unitOfWork);

            if (repositorioReformaNfeRetorno.BuscarPorReformaChaveNfe(reforma.Codigo, xmlNotaFiscal.Chave) != null)
                return false;

            var repositorioXmlNfe = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

            var nfeRetorno = new Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPalletNfeRetorno()
            {
                XmlNotaFiscal = xmlNotaFiscal,
                ReformaPallet = reforma
            };

            repositorioXmlNfe.Inserir(xmlNotaFiscal, Auditado);
            repositorioReformaNfeRetorno.Inserir(nfeRetorno, Auditado);

            Servicos.Auditoria.Auditoria.Auditar(Auditado, reforma, $"Adicionada a NF-e de retorno nº {xmlNotaFiscal.Numero}", unitOfWork);

            return true;
        }

        private bool AdicionarNfeSaida(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPallet reforma, Servicos.DTO.CustomFile arquivo)
        {
            Servicos.Embarcador.NFe.NFe servicoNFe = new Servicos.Embarcador.NFe.NFe(unitOfWork);
            System.IO.StreamReader leitorXml = new System.IO.StreamReader(arquivo.InputStream);

            Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebservice = repConfiguracaoWebService.BuscarConfiguracaoPadrao();

            if (!servicoNFe.BuscarDadosNotaFiscal(out string erro, out Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, leitorXml, unitOfWork, null, true, false, false, TipoServicoMultisoftware, false, false, null, null, null, configuracaoWebservice?.CadastroAutomaticoPessoaExterior ?? false))
                return false;

            Repositorio.Embarcador.Pallets.Reforma.ReformaPalletNfeSaida repositorioReformaNfeSaida = new Repositorio.Embarcador.Pallets.Reforma.ReformaPalletNfeSaida(unitOfWork);

            if (repositorioReformaNfeSaida.BuscarPorReformaChaveNfe(reforma.Codigo, xmlNotaFiscal.Chave) != null)
                return false;

            var repositorioXmlNfe = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

            var nfeSaida = new Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPalletNfeSaida()
            {
                XmlNotaFiscal = xmlNotaFiscal,
                ReformaPallet = reforma
            };

            repositorioXmlNfe.Inserir(xmlNotaFiscal, Auditado);
            repositorioReformaNfeSaida.Inserir(nfeSaida, Auditado);

            Servicos.Auditoria.Auditoria.Auditar(Auditado, reforma, $"Adicionada a NF-e de saida nº {xmlNotaFiscal.Numero}", unitOfWork);

            return true;
        }

        private void ExcluirTodosXmlNfe(List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> listaXmlNotaFiscal)
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            var repositorioXmlNfe = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

            foreach (var xmlNotaFiscal in listaXmlNotaFiscal)
            {
                try
                {
                    repositorioXmlNfe.Deletar(xmlNotaFiscal, Auditado);
                }
                catch (Exception excecao)
                {
                    if (!ExcessaoPorPossuirDependeciasNoBanco(excecao))
                        Servicos.Log.TratarErro(excecao);
                }
            }

            unitOfWork.Dispose();
        }

        private void ExcluirXmlNfe(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal)
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            var repositorioXmlNfe = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

            try
            {
                repositorioXmlNfe.Deletar(xmlNotaFiscal, Auditado);
            }
            catch (Exception excecao)
            {
                if (!ExcessaoPorPossuirDependeciasNoBanco(excecao))
                    Servicos.Log.TratarErro(excecao);
            }

            unitOfWork.Dispose();
        }

        private void InserirMovimentacaoEstoque(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPallet reforma, TipoOperacaoMovimentacaoEstoquePallet tipoOperacao)
        {
            var servicoEstoque = new Servicos.Embarcador.Pallets.EstoquePallet(unitOfWork);

            var movimentacao = new Dominio.ObjetosDeValor.Embarcador.Pallets.DadosMovimentacaoEstoquePallet()
            {
                CodigoFilial = reforma.Envio.Filial?.Codigo ?? 0,
                CodigoReforma = reforma.Codigo,
                CodigoTransportador = reforma.Envio.Transportador?.Codigo ?? 0,
                Quantidade = reforma.Envio.QuantidadesReforma.Sum(o => o.Quantidade),
                TipoLancamento = TipoLancamento.Automatico,
                TipoOperacaoMovimentacao = tipoOperacao,
            };

            servicoEstoque.InserirMovimentacao(movimentacao);
        }

        private bool IsSituacaoPermiteCancelamento(Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPallet reforma)
        {
            return (
                (reforma.Situacao == SituacaoReformaPallet.AguardandoNfeSaida) ||
                (reforma.Situacao == SituacaoReformaPallet.AguardandoRetorno)
            );
        }

        private string ObterCaminhoArquivos(Repositorio.UnitOfWork unitOfWork)
        {
            return Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "ReformaPallets", "ImportacaoXML" });
        }

        private Dominio.Entidades.Cliente ObterDestinatario(Repositorio.UnitOfWork unitOfWork)
        {
            double cnpj = 0d;

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                Dominio.Entidades.Empresa transportador = ObterTransportador(unitOfWork, Request.GetIntParam("Transportador"));
                cnpj = transportador.CNPJ.ToDouble();
            }
            else
            {
                Dominio.Entidades.Embarcador.Filiais.Filial filial = ObterFilial(unitOfWork, Request.GetIntParam("Filial"));
                cnpj = filial.CNPJ.ToDouble();
            }

            var repositorio = new Repositorio.Cliente(unitOfWork);
            var fornecedor = repositorio.BuscarPorCPFCNPJ(cnpj);

            if (fornecedor == null)
                throw new Exception("Destinatário não encontrado");

            return fornecedor;
        }

        private dynamic ObterEnvio(Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPallet reforma)
        {
            return new
            {
                reforma.Envio.Codigo,
                Data = reforma.Envio.Data.ToString("dd/MM/yyyy"),
                reforma.Numero,
                Filial = new { Codigo = reforma.Envio.Filial?.Codigo ?? 0, Descricao = reforma.Envio.Filial?.Descricao ?? "" },
                Fornecedor = new { reforma.Envio.Fornecedor.Codigo, reforma.Envio.Fornecedor.Descricao },
                Transportador = new { Codigo = reforma.Envio.Transportador?.Codigo ?? 0, Descricao = reforma.Envio.Transportador?.Descricao ?? "" }
            };
        }

        private Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPalletEnvio ObterEnvioAdicionar(Repositorio.UnitOfWork unitOfWork)
        {
            return new Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPalletEnvio()
            {
                Data = Request.GetNullableDateTimeParam("Data") ?? throw new Exception("Data não informada"),
                Filial = ObterFilial(unitOfWork, Request.GetIntParam("Filial")),
                Fornecedor = ObterFornecedor(unitOfWork, Request.GetIntParam("Fornecedor")),
                Transportador = ObterTransportador(unitOfWork, Request.GetIntParam("Transportador"))
            };
        }

        private dynamic ObterEnvioQuantidades(Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPallet reforma)
        {
            return (
                from quantidadeReforma in reforma.Envio.QuantidadesReforma
                select new
                {
                    quantidadeReforma.SituacaoDevolucaoPallet.Codigo,
                    quantidadeReforma.SituacaoDevolucaoPallet.Descricao,
                    quantidadeReforma.Quantidade
                }
            ).ToList();
        }

        private List<Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPalletEnvioQuantidade> ObterEnvioQuantidadesAdicionar(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPalletEnvio envio)
        {
            var listaQuantidadesEnviadas = new List<Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPalletEnvioQuantidade>();
            var quantidadesEnviadas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("QuantidadesEnviadas"));
            var repositorioSituacaoDevolucao = new Repositorio.Embarcador.Pallets.SituacaoDevolucaoPallet(unitOfWork);

            foreach (var quantidadeEnviada in quantidadesEnviadas)
            {
                var quantidade = ((string)quantidadeEnviada.Quantidade).ToInt();
                var situacaoDevolucao = repositorioSituacaoDevolucao.BuscarPorCodigo(((string)quantidadeEnviada.Codigo).ToInt());

                if (situacaoDevolucao == null)
                    throw new Exception("Situação de devolução de pallet não encontrada");

                var envioQuantidade = new Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPalletEnvioQuantidade()
                {
                    Envio = envio,
                    Quantidade = quantidade,
                    SituacaoDevolucaoPallet = situacaoDevolucao
                };

                listaQuantidadesEnviadas.Add(envioQuantidade);
            }

            if (listaQuantidadesEnviadas.Count == 0)
                throw new Exception("Nenhuma quantidade para reforma informada");

            return listaQuantidadesEnviadas;
        }

        private Dominio.Entidades.Embarcador.Filiais.Filial ObterFilial(Repositorio.UnitOfWork unitOfWork, int codigoFilial)
        {
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return null;

            if (codigoFilial <= 0)
                throw new Exception("Filial não informada");

            var repositorio = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            var filial = repositorio.BuscarPorCodigo(codigoFilial);

            if (filial == null)
                throw new Exception("Filial não encontrada");

            return filial;
        }

        private Dominio.Entidades.Cliente ObterFornecedor(Repositorio.UnitOfWork unitOfWork, double cpfCnpj)
        {
            if (cpfCnpj <= 0d)
                throw new Exception("Fornecedor não informado");

            var repositorio = new Repositorio.Cliente(unitOfWork);
            var fornecedor = repositorio.BuscarPorCPFCNPJ(cpfCnpj);

            if (fornecedor == null)
                throw new Exception("Fornecedor não encontrado");

            return fornecedor;
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho(propriedade: "Codigo", visivel: false);
                grid.AdicionarCabecalho(descricao: "Número", propriedade: "Numero", tamanho: 10, alinhamento: Models.Grid.Align.center, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Data", propriedade: "Data", tamanho: 10, alinhamento: Models.Grid.Align.center, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Situacao", propriedade: "Situacao", tamanho: 14, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    grid.AdicionarCabecalho(descricao: "Empresa/Filial", propriedade: "Transportador", tamanho: 22, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                    grid.AdicionarCabecalho(descricao: "CNPJ Empresa/Filial", propriedade: "TransportadorCnpj", tamanho: 11, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false);
                }
                else
                {
                    grid.AdicionarCabecalho(descricao: "Filial", propriedade: "Filial", tamanho: 22, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                    grid.AdicionarCabecalho(descricao: "CNPJ Filial", propriedade: "FilialCnpj", tamanho: 11, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false);
                }

                grid.AdicionarCabecalho(descricao: "Fornecedor", propriedade: "Fornecedor", tamanho: 22, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "CPF/CNPJ Fornecedor", propriedade: "FornecedorCpfCnpj", tamanho: 11, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false);

                var propriedadeOrdenar = ObterPropriedadeOrdenar(grid);
                int totalRegistros = 0;
                var filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaReforma()
                {
                    Numero = Request.GetIntParam("Numero"),
                    CodigoFilial = Request.GetIntParam("Filial"),
                    CodigoTransportador = Request.GetIntParam("Transportador"),
                    CpfCnpjFornecedor = Request.GetDoubleParam("Fornecedor"),
                    DataInicial = Request.GetNullableDateTimeParam("DataInicio"),
                    DataLimite = Request.GetNullableDateTimeParam("DataLimite"),
                    Situacao = Request.GetEnumParam<SituacaoReformaPallet>("Situacao")
                };

                var lista = Pesquisar(filtrosPesquisa, ref totalRegistros, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private dynamic ObterNfeRetorno(Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPallet reforma)
        {
            return (
                from nfeRetorno in reforma.NotasFiscaisRetorno
                select new
                {
                    nfeRetorno.Codigo,
                    nfeRetorno.XmlNotaFiscal.Numero,
                    nfeRetorno.XmlNotaFiscal.Chave,
                    Quantidade = nfeRetorno.XmlNotaFiscal.QuantidadePallets,
                    nfeRetorno.XmlNotaFiscal.Valor
                }
            ).ToList();
        }

        private dynamic ObterNfeSaida(Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPallet reforma)
        {
            return (
                from nfeSaida in reforma.NotasFiscaisSaida
                select new
                {
                    nfeSaida.Codigo,
                    nfeSaida.XmlNotaFiscal.Numero,
                    nfeSaida.XmlNotaFiscal.Chave,
                    Quantidade = nfeSaida.XmlNotaFiscal.QuantidadePallets,
                    nfeSaida.XmlNotaFiscal.Valor
                }
            ).ToList();
        }

        private dynamic ObterNfsRetorno(Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPallet reforma)
        {
            return (
                from nfeRetorno in reforma.NotasServicoRetorno
                select new
                {
                    nfeRetorno.Codigo,
                    nfeRetorno.Numero,
                    nfeRetorno.Serie
                }
            ).ToList();
        }

        private string ObterPropriedadeOrdenar(Models.Grid.Grid grid)
        {
            if (grid.header[grid.indiceColunaOrdena].data == "Data")
                return "Envio.Data";

            if (grid.header[grid.indiceColunaOrdena].data == "Filial")
                return "Envio.Filial.Descricao";

            if (grid.header[grid.indiceColunaOrdena].data == "Fornecedor")
                return "Envio.Fornecedor.Nome";

            if (grid.header[grid.indiceColunaOrdena].data == "Transportador")
                return "Envio.Transportador.RazaoSocial";

            return grid.header[grid.indiceColunaOrdena].data;
        }

        private dynamic ObterResumo(Dominio.Entidades.Embarcador.Pallets.Reforma.ReformaPallet reforma)
        {
            return new
            {
                Data = reforma.Envio.Data.ToString("dd/MM/yyyy"),
                reforma.Numero,
                Filial = reforma.Envio.Filial?.Descricao,
                Fornecedor = reforma.Envio.Fornecedor.Descricao,
                Transportador = reforma.Envio.Transportador?.Descricao,
                Situacao = reforma.Situacao.ObterDescricao(),
                QuantidadesEnviadas = (
                    from quantidadeReforma in reforma.Envio.QuantidadesReforma
                    select new
                    {
                        quantidadeReforma.SituacaoDevolucaoPallet.Descricao,
                        quantidadeReforma.Quantidade
                    }
                ).ToList()
            };
        }

        private Dominio.Entidades.Empresa ObterTransportador(Repositorio.UnitOfWork unitOfWork, int codigoTransportador)
        {
            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return null;

            if (codigoTransportador <= 0)
                throw new Exception("Empresa/Filial não informada");

            Repositorio.Empresa repositorio = new Repositorio.Empresa(unitOfWork);
            Dominio.Entidades.Empresa empresa = repositorio.BuscarPorCodigo(codigoTransportador);

            if (empresa == null)
                throw new Exception("Empresa/Filial não encontrada");

            return empresa;
        }

        private dynamic Pesquisar(Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaReforma filtrosPesquisa, ref int totalRegistros, string propriedadeOrdenar, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros, Repositorio.UnitOfWork unitOfWork)
        {
            var repositorio = new Repositorio.Embarcador.Pallets.Reforma.ReformaPallet(unitOfWork);
            var listaReformaPallet = repositorio.Consultar(filtrosPesquisa, propriedadeOrdenar, direcaoOrdenacao, inicioRegistros, maximoRegistros);

            totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);

            return (from reforma in listaReformaPallet
                    select new
                    {
                        reforma.Codigo,
                        Data = reforma.Envio.Data.ToString("dd/MM/yyyy"),
                        reforma.Numero,
                        Filial = reforma.Envio.Filial?.Descricao,
                        FilialCnpj = reforma.Envio.Filial?.CNPJ_Formatado,
                        Fornecedor = reforma.Envio.Fornecedor.Nome,
                        FornecedorCpfCnpj = reforma.Envio.Fornecedor.CPF_CNPJ_Formatado,
                        Transportador = reforma.Envio.Transportador?.Descricao,
                        TransportadorCnpj = reforma.Envio.Transportador?.CNPJ_Formatado,
                        Situacao = reforma.Situacao.ObterDescricao()
                    }
            ).ToList();
        }

        #endregion
    }
}
