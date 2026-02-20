using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.NotaFiscal
{
    [CustomAuthorize(new string[] { "RegrasAprovacao" }, "NotasFiscais/AutorizacaoNaoConformidade")]
    public class AutorizacaoNaoConformidadeController : RegraAutorizacao.AutorizacaoController<
        Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.AprovacaoAlcadaNaoConformidade,
        Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.RegraAutorizacaoNaoConformidade,
        Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade
    >
    {
		#region Construtores

		public AutorizacaoNaoConformidadeController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Globais

		public async Task<IActionResult> AjustarNumeroPedido()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.NotaFiscal.NaoConformidade repositorioNaoConformidade = new Repositorio.Embarcador.NotaFiscal.NaoConformidade(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade naoConformidade = repositorioNaoConformidade.BuscarPorCodigo(codigo, auditavel: false);
                Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidade itemNC = new Repositorio.Embarcador.NotaFiscal.ItemNaoConformidade(unitOfWork).BuscarPorCodigo(naoConformidade.ItemNaoConformidade.Codigo);

                if (naoConformidade == null)
                    throw new ServicoException("Não foi possível encontrar o registro.");

                if (!naoConformidade.Situacao.IsPermitirAjustar() || !itemNC.PermiteContingencia)
                    throw new ServicoException("Não é possível ajustar a não conformidade na situação atual.");

                if (naoConformidade.ItemNaoConformidade.TipoRegra != TipoRegraNaoConformidade.NumeroPedido)
                    throw new ServicoException("O tipo da não conformidade não permite ajustar o número do pedido.");

                naoConformidade.Situacao = SituacaoNaoConformidade.AprovadaEmContingencia;
                naoConformidade.CargaPedido.Pedido.NumeroOrdem = Request.GetStringParam("NumeroPedido");

                if (naoConformidade.CargaPedido.Pedido.NumeroOrdem != naoConformidade.XMLNotaFiscal.NumeroOrdemPedidoIntegracaoUnilever)
                    throw new ServicoException($"O número do pedido informado é diferente do número existente na nota fiscal ({naoConformidade.XMLNotaFiscal.NumeroOrdemPedidoIntegracaoUnilever}).");

                repositorioNaoConformidade.Atualizar(naoConformidade);
                repositorioPedido.Atualizar(naoConformidade.CargaPedido.Pedido);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, naoConformidade, $"Alterado o número do pedido para {naoConformidade.CargaPedido.Pedido.NumeroOrdem}", unitOfWork);
                new Servicos.Embarcador.NotaFiscal.NaoConformidade(unitOfWork).NotificarStatusParaTransportador(naoConformidade, true);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao ajustar a não conformidade.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AprovarConformeCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.NotaFiscal.NaoConformidade repositorioNaoConformidade = new Repositorio.Embarcador.NotaFiscal.NaoConformidade(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade naoConformidade = repositorioNaoConformidade.BuscarPorCodigo(codigo, auditavel: false);
                Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidade itemNC = new Repositorio.Embarcador.NotaFiscal.ItemNaoConformidade(unitOfWork).BuscarPorCodigo(naoConformidade.ItemNaoConformidade.Codigo);

                if (naoConformidade == null)
                    throw new ServicoException("Não foi possível encontrar o registro.");

                if (naoConformidade.ItemNaoConformidade.TipoRegra != TipoRegraNaoConformidade.NumeroPedido)
                    throw new ServicoException("O tipo da não conformidade não permite essa tratativa.");

                naoConformidade.Situacao = SituacaoNaoConformidade.AprovadaEmContingencia;

                repositorioNaoConformidade.Atualizar(naoConformidade);
                repositorioPedido.Atualizar(naoConformidade.CargaPedido.Pedido);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, naoConformidade, $"Não conformidade aprovada", unitOfWork);
                new Servicos.Embarcador.NotaFiscal.NaoConformidade(unitOfWork).NotificarStatusParaTransportador(naoConformidade, true);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao aprovar a não conformidade.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AjustarPeso()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.NotaFiscal.NaoConformidade repositorioNaoConformidade = new Repositorio.Embarcador.NotaFiscal.NaoConformidade(unitOfWork);
                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
                Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade naoConformidade = repositorioNaoConformidade.BuscarPorCodigo(codigo, auditavel: false);
                Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidade itemNC = new Repositorio.Embarcador.NotaFiscal.ItemNaoConformidade(unitOfWork).BuscarPorCodigo(naoConformidade.ItemNaoConformidade.Codigo);

                if (naoConformidade == null)
                    throw new ServicoException("Não foi possível encontrar o registro.");

                if (!naoConformidade.Situacao.IsPermitirAjustar() || !itemNC.PermiteContingencia)
                    throw new ServicoException("Não é possível ajustar a não conformidade na situação atual.");

                if (naoConformidade.ItemNaoConformidade.TipoRegra != TipoRegraNaoConformidade.PesoLiquidoTotal)
                    throw new ServicoException("O tipo da não conformidade não permite ajustar o peso.");

                naoConformidade.Situacao = SituacaoNaoConformidade.AprovadaEmContingencia;
                naoConformidade.XMLNotaFiscal.PesoLiquido = Request.GetDecimalParam("Peso");

                if (naoConformidade.XMLNotaFiscal.PesoLiquido <= 0m)
                    throw new ServicoException("O peso informado deve ser maior do que zero.");

                repositorioNaoConformidade.Atualizar(naoConformidade);
                repositorioXMLNotaFiscal.Atualizar(naoConformidade.XMLNotaFiscal);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, naoConformidade, $"Alterado o peso da nota fiscal para {naoConformidade.XMLNotaFiscal.PesoLiquido:n2}", unitOfWork);
                new Servicos.Embarcador.NotaFiscal.NaoConformidade(unitOfWork).NotificarStatusParaTransportador(naoConformidade, true);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao ajustar a não conformidade.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AjustarDeParaProdutos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.NotaFiscal.NaoConformidade repositorioNaoConformidade = new Repositorio.Embarcador.NotaFiscal.NaoConformidade(unitOfWork);
                Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade naoConformidade = repositorioNaoConformidade.BuscarPorCodigo(codigo, auditavel: false);
                Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidade itemNC = new Repositorio.Embarcador.NotaFiscal.ItemNaoConformidade(unitOfWork).BuscarPorCodigo(naoConformidade.ItemNaoConformidade.Codigo);

                if (naoConformidade == null)
                    throw new ServicoException("Não foi possível encontrar o registro.");

                if (!naoConformidade.Situacao.IsPermitirAjustar() || !itemNC.PermiteContingencia)
                    throw new ServicoException("Não é possível ajustar a não conformidade na situação atual.");

                if (naoConformidade.ItemNaoConformidade.TipoRegra != TipoRegraNaoConformidade.ProdutoDePara)
                    throw new ServicoException("O tipo da não conformidade não permite ajustar o de/para dos produtos.");

                naoConformidade.Situacao = SituacaoNaoConformidade.AprovadaEmContingencia;

                // implementar cadastro aqui

                repositorioNaoConformidade.Atualizar(naoConformidade);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, naoConformidade, $"Adicionado o de/para dos produtos", unitOfWork);
                new Servicos.Embarcador.NotaFiscal.NaoConformidade(unitOfWork).NotificarStatusParaTransportador(naoConformidade, true);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao ajustar a não conformidade.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReprocessarNaoConformidade()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.NotaFiscal.NaoConformidade repositorioNaoConformidade = new Repositorio.Embarcador.NotaFiscal.NaoConformidade(unitOfWork);
                Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade naoConformidade = repositorioNaoConformidade.BuscarPorCodigo(codigo, auditavel: false);

                if (naoConformidade == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (naoConformidade.Situacao != SituacaoNaoConformidade.SemRegraAprovacao)
                    return new JsonpResult(new { RegraReprocessada = true });

                unitOfWork.Start();

                Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NaoConformidade dadosNaoConformidade = new Servicos.Embarcador.NotaFiscal.NaoConformidadeAprovacao(unitOfWork).CriarAprovacao(naoConformidade.Codigo, TipoServicoMultisoftware);

                unitOfWork.CommitChanges();

                return new JsonpResult(new { RegraReprocessada = dadosNaoConformidade.Situacao != SituacaoNaoConformidade.SemRegraAprovacao });
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao reprocessar a não conformidade.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReprocessarMultiplasNaoConformidades()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                List<int> codigos = ObterCodigosOrigensSelecionadas(unitOfWork);
                Repositorio.Embarcador.NotaFiscal.AprovacaoAlcadaNaoConformidade repositorioAprovacao = new Repositorio.Embarcador.NotaFiscal.AprovacaoAlcadaNaoConformidade(unitOfWork);
                Servicos.Embarcador.NotaFiscal.NaoConformidadeAprovacao servicoAprovacao = new Servicos.Embarcador.NotaFiscal.NaoConformidadeAprovacao(unitOfWork);
                List<int> codigosNaoConformidadesSemRegraAprovacao = repositorioAprovacao.BuscarCodigosSemRegraAprovacaoPorCodigos(codigos);
                int totalRegrasReprocessadas = 0;

                foreach (int codigoNaoConformidade in codigosNaoConformidadesSemRegraAprovacao)
                {
                    Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NaoConformidade dadosNaoConformidade = servicoAprovacao.CriarAprovacao(codigoNaoConformidade, TipoServicoMultisoftware);

                    if (dadosNaoConformidade.Situacao != SituacaoNaoConformidade.SemRegraAprovacao)
                        totalRegrasReprocessadas++;
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(new { RegrasReprocessadas = totalRegrasReprocessadas });
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao reprocessar as não conformidades.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterDadosProdutos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                dynamic produtos = new { };
                Servicos.NFe servicoNFe = new Servicos.NFe(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
                Repositorio.Embarcador.NotaFiscal.NaoConformidade repNaoConformidade = new Repositorio.Embarcador.NotaFiscal.NaoConformidade(unitOfWork);

                Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade naoConformidade = repNaoConformidade.BuscarPorCodigo(codigo, false);
                if (naoConformidade?.XMLNotaFiscal == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar a nota fiscal.");

                Dominio.ObjetosDeValor.Embarcador.NotaFiscal.DadosNFe dadosNFe = new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.DadosNFe();

                dadosNFe = servicoNFe.ObterDocumentoPorXML(Utilidades.String.ToStream(naoConformidade.XMLNotaFiscal.XML), unitOfWork, true);
                produtos = ObterProdutosCadastrados(dadosNFe, unitOfWork);

                return new JsonpResult(new
                {
                    Fornecedor = new { Codigo = dadosNFe.Remetente?.Codigo ?? 0, Descricao = dadosNFe.Remetente?.Descricao ?? string.Empty },
                    Produtos = produtos
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao carregar o XML da NF-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion Métodos Globais

        #region Métodos Globais Sobrescritos

        public override IActionResult BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.NotaFiscal.NaoConformidade repositorioNaoConformidade = new Repositorio.Embarcador.NotaFiscal.NaoConformidade(unitOfWork);
                Repositorio.Embarcador.NotaFiscal.NaoConformidadeAnexo repositorioNaoConformidadeAnexo = new Repositorio.Embarcador.NotaFiscal.NaoConformidadeAnexo(unitOfWork);

                Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade naoConformidade = repositorioNaoConformidade.BuscarPorCodigo(codigo, auditavel: false);

                if (naoConformidade == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                List<Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidadeAnexo> anexos = repositorioNaoConformidadeAnexo.BuscarAnexosPorNaoConformidade(codigo);

                return new JsonpResult(new
                {
                    naoConformidade.Codigo,
                    naoConformidade.Situacao,
                    naoConformidade.ItemNaoConformidade.TipoRegra,
                    PermitirAjustar = naoConformidade.Situacao.IsPermitirAjustar() && (naoConformidade?.ItemNaoConformidade?.PermiteContingencia ?? false),
                    NumeroNotaFiscal = naoConformidade.XMLNotaFiscal?.Numero.ToString() ?? "",
                    ItemNaoConformidadeDescricao = naoConformidade.ItemNaoConformidade.Descricao,
                    TipoRegraNaoConformidade = naoConformidade.ItemNaoConformidade.TipoRegra,
                    TipoParticipante = naoConformidade.TipoParticipante.HasValue ? naoConformidade.TipoParticipante.Value.ObterDescricao() : "",
                    SituacaoDescricao = naoConformidade.Situacao.ObterDescricao(),
                    Anexos = (
                        from anexo in anexos
                        select new
                        {
                            anexo.Codigo,
                            anexo.Descricao,
                            anexo.NomeArquivo,
                        }
                    ).ToList()
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

        public override IActionResult AprovarMultiplosItens()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var codigosOrigem = ObterCodigosOrigensSelecionadas(unitOfWork);
                List<Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade> ncs = new Repositorio.Embarcador.NotaFiscal.NaoConformidade(unitOfWork).BuscarPorCodigos(codigosOrigem);

                if (ncs == null)
                    throw new ServicoException("Não conformidades não encontradas para realizar aprovação.");

                if (ncs.Any(o => !o.ItemNaoConformidade.PermiteContingencia))
                {
                    throw new ServicoException("Não é possível aprovar as regras selecionadas.");
                }
                return base.AprovarMultiplosItens();
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
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
                return new JsonpResult(false, "Ocorreu uma falha ao aprovar as regras.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaNaoConformidadeAprovacao ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaNaoConformidadeAprovacao()
            {
                CodigoUsuario = Request.GetIntParam("Usuario"),
                NumeroNotaFiscal = Request.GetIntParam("NumeroNotaFiscal"),
                DataInicialGeracaoNC = Request.GetNullableDateTimeParam("DataInicialGeracaoNC"),
                DataFinalGeracaoNC = Request.GetNullableDateTimeParam("DataFinalGeracaoNC"),
                DataInicialEmissaoNotaFiscal = Request.GetNullableDateTimeParam("DataInicialEmissaoNotaFiscal"),
                DataFinalEmissaoNotaFiscal = Request.GetNullableDateTimeParam("DataFinalEmissaoNotaFiscal"),
                NumeroCarga = Request.GetStringParam("NumeroCarga"),
                NumeroPedidoEmbarcador = Request.GetStringParam("NumeroPedidoEmbarcador"),
                Transportador = Request.GetIntParam("Transportador"),
                Filial = Request.GetStringParam("Filial"),
                Destino = Request.GetDoubleParam("Destino"),
                ItemNC = Request.GetStringParam("ItemNC"),
                NumeroNotas = Request.GetListParam<int>("NumeroNotas"),
                Motorista = Request.GetListParam<string>("Motorista"),
                Fornecedor = Request.GetDoubleParam("Fornecedor"),
                NumeroOrdem = Request.GetStringParam("NumeroOrdem"),
                Situacao = Request.GetNullableEnumParam<SituacaoNaoConformidade>("Situacao")
            };
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "NumeroNotaFiscal")
                return "XMLNotaFiscal.Numero";

            if (propriedadeOrdenar == "CodigoCargaEmbarcador")
                return "CargaPedido.Carga.CodigoCargaEmbarcador";

            if (propriedadeOrdenar == "NumeroPedidoEmbarcador")
                return "CargaPedido.Pedido.NumeroPedidoEmbarcador";

            if (propriedadeOrdenar == "ItemNaoConformidade")
                return "ItemNaoConformidade.Descricao";

            return propriedadeOrdenar;
        }

        private string CorPorPrazo(Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade naoConformidade)
        {
            if (naoConformidade.Situacao == SituacaoNaoConformidade.AguardandoTratativa)
                return ClasseCorFundo.Warning(IntensidadeCor._100);

            if (naoConformidade.Situacao == SituacaoNaoConformidade.Concluida)
                return ClasseCorFundo.Sucess(IntensidadeCor._100);

            if (naoConformidade.Situacao == SituacaoNaoConformidade.Reprovada)
                return ClasseCorFundo.Danger(IntensidadeCor._100);

            if (naoConformidade.Situacao == SituacaoNaoConformidade.ConcluidaPorIntegracao || naoConformidade.Situacao == SituacaoNaoConformidade.AprovadaEmContingencia)
                return ClasseCorFundo.Primary(IntensidadeCor._100);

            if (naoConformidade.Situacao == SituacaoNaoConformidade.SemRegraAprovacao)
                return ClasseCorFundo.Secondary(IntensidadeCor._100);

            return "";
        }

        private dynamic ObterProdutosCadastrados(Dominio.ObjetosDeValor.Embarcador.NotaFiscal.DadosNFe dadosNFe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Produtos.ProdutoEmbarcadorFornecedor repositorioProdutoEmbarcadorFornecedor = new Repositorio.Embarcador.Produtos.ProdutoEmbarcadorFornecedor(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFiliais = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);

            List<dynamic> produtos = new List<dynamic>();

            foreach (Dominio.ObjetosDeValor.Embarcador.NFe.Produtos produtoNFe in dadosNFe.Produtos)
            {
                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorFornecedor produtoEmbarcadorFornecedor = repositorioProdutoEmbarcadorFornecedor.BuscarPorCodigoInternoEFornecedor(produtoNFe.Codigo, dadosNFe.Remetente.CPF_CNPJ);
                List<Dominio.Entidades.Embarcador.Filiais.Filial> filiais = repFiliais.BuscarPorCodigoInternoProduto(produtoEmbarcadorFornecedor?.CodigoInterno);

                if (!produtos.Any(p => p.Codigo == produtoNFe.Codigo))
                {
                    var produto = new
                    {
                        produtoNFe.Codigo,
                        CodigoProdutoFornecedor = produtoNFe.Codigo,
                        DescricaoProdutoFornecedor = produtoNFe.Descricao,
                        ProdutoEmbarcadorCodigo = produtoEmbarcadorFornecedor?.ProdutoEmbarcador?.Codigo ?? 0,
                        CodigoProdutoEmbarcador = produtoEmbarcadorFornecedor?.ProdutoEmbarcador?.CodigoProdutoEmbarcador ?? string.Empty,
                        DescricaoProdutoEmbarcador = produtoEmbarcadorFornecedor?.ProdutoEmbarcador?.Descricao ?? string.Empty,
                        Filiais = (from obj in filiais select new { obj?.Codigo, obj?.Descricao }).ToList(),
                        Incluso = produtoEmbarcadorFornecedor != null ? true : false,
                        Status = produtoEmbarcadorFornecedor != null ? "Já cadastrado" : "Não cadastrado",
                        DescricoesFiliais = string.Join(", ", filiais.Select(x => x?.Descricao).ToList()),

                    };

                    produtos.Add(produto);
                }
            }

            return produtos;
        }

        #endregion

        #region Métodos Protegidos SobrescritosS

        protected override bool IsPermitirDelegar(Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade naoConformidade)
        {
            return naoConformidade.Situacao == SituacaoNaoConformidade.AguardandoTratativa;
        }

        protected override List<int> ObterCodigosOrigensSelecionadas(Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade> listaNaoConformidade;
            var selecionarTodos = Request.GetBoolParam("SelecionarTodos");

            if (selecionarTodos)
            {
                var listaItensNaoSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensNaoSelecionados"));
                Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaNaoConformidadeAprovacao filtrosPesquisa = ObterFiltrosPesquisa();
                Repositorio.Embarcador.NotaFiscal.AprovacaoAlcadaNaoConformidade repositorioAprovacaoAlcada = new Repositorio.Embarcador.NotaFiscal.AprovacaoAlcadaNaoConformidade(unitOfWork);

                listaNaoConformidade = repositorioAprovacaoAlcada.Consultar(filtrosPesquisa, new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta() { PropriedadeOrdenar = "Codigo" });

                foreach (var itemNaoSelecionado in listaItensNaoSelecionados)
                    listaNaoConformidade.Remove(new Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade() { Codigo = (int)itemNaoSelecionado.Codigo });
            }
            else
            {
                Repositorio.Embarcador.NotaFiscal.NaoConformidade repositorioNaoConformidade = new Repositorio.Embarcador.NotaFiscal.NaoConformidade(unitOfWork);
                var listaItensSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensSelecionados"));

                listaNaoConformidade = new List<Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade>();

                foreach (var itemSelecionado in listaItensSelecionados)
                    listaNaoConformidade.Add(repositorioNaoConformidade.BuscarPorCodigo((int)itemSelecionado.Codigo, auditavel: false));
            }

            return (from naoConformidade in listaNaoConformidade select naoConformidade.Codigo).ToList();
        }

        protected override Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoCarga", false);
                grid.AdicionarCabecalho("PermitirAjustar", false);
                grid.AdicionarCabecalho("Número da Nota", "NumeroNotaFiscal", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Número da Carga", "CodigoCargaEmbarcador", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Número do Pedido", "NumeroPedidoEmbarcador", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Item de Não Conformidade", "ItemNaoConformidade", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Participante", "TipoParticipante", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Data Emissão NF", "DataEmissaoNF", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Data Geração NC", "DataGeracaoNC", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Número Ordem Pedido", "NumeroOrdemPedido", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Transportador", "Transportador", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Filial", "Filial", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Destino", "Destino", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Fornecedor", "Fornecedor", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Descrição do item de NC", "DescricaoItemNC", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Motorista", "Motorista", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Situação da Aprovação", "Situacao", 15, Models.Grid.Align.center, true);
                

                Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaNaoConformidadeAprovacao filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);

                Repositorio.Embarcador.NotaFiscal.AprovacaoAlcadaNaoConformidade repositorio = new Repositorio.Embarcador.NotaFiscal.AprovacaoAlcadaNaoConformidade(unitOfWork);

                Models.Grid.GridPreferencias gridPreferencias = new Models.Grid.GridPreferencias(unitOfWork, "AutorizacaoNaoConformidade/Pesquisa", "grid-pesquisa-nao-conformidade");
                grid.AplicarPreferenciasGrid(gridPreferencias.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));

                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade> listaNaoConformidade = totalRegistros > 0 ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade>();

                var listaNaoConformidadeRetornar = (
                    from naoConformidade in listaNaoConformidade
                    select new
                    {
                        naoConformidade.Codigo,
                        CodigoCarga = naoConformidade.CargaPedido?.Carga.Codigo.ToString() ?? string.Empty,
                        NumeroNotaFiscal = naoConformidade.XMLNotaFiscal?.Numero.ToString() ?? "",
                        CodigoCargaEmbarcador = naoConformidade.CargaPedido?.Carga.CodigoCargaEmbarcador ?? "",
                        NumeroPedidoEmbarcador = naoConformidade.CargaPedido?.Pedido.NumeroPedidoEmbarcador ?? "",
                        ItemNaoConformidade = naoConformidade.ItemNaoConformidade.Descricao,
                        TipoParticipante = naoConformidade.TipoParticipante.HasValue ? naoConformidade.TipoParticipante.Value.ObterDescricao() : "",
                        DataEmissaoNF = naoConformidade.XMLNotaFiscal?.DataEmissao,
                        DataGeracaoNC = naoConformidade.DataCriacao,
                        NumeroOrdemPedido = naoConformidade.CargaPedido?.Pedido?.NumeroOrdem ?? string.Empty,
                        Transportador = naoConformidade.CargaPedido?.Carga?.Empresa?.RazaoSocial ?? string.Empty,
                        Filial = naoConformidade.CargaPedido?.Carga?.Filial?.Descricao ?? string.Empty,
                        Destino = naoConformidade.XMLNotaFiscal?.Destinatario?.NomeCNPJ ?? string.Empty,
                        Fornecedor = naoConformidade.XMLNotaFiscal?.Emitente?.NomeCNPJ ?? string.Empty,
                        DescricaoItemNC = naoConformidade.ItemNaoConformidade?.Descricao ?? string.Empty,
                        Motorista = naoConformidade.CargaPedido?.Carga?.NomePrimeiroMotorista ?? string.Empty,
                        Situacao = naoConformidade.Situacao.ObterDescricao(),
                        DT_RowClass = this.CorPorPrazo(naoConformidade),
                        PermitirAjustar = naoConformidade.Situacao.IsPermitirAjustar() && (naoConformidade?.ItemNaoConformidade?.PermiteContingencia ?? false)
                    }
                ).ToList();

                grid.AdicionaRows(listaNaoConformidadeRetornar);
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

        protected override void VerificarSituacaoOrigem(Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade naoConformidade, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.NotaFiscal.NaoConformidadeAnexo repositorioNaoConformidadeAnexo = new Repositorio.Embarcador.NotaFiscal.NaoConformidadeAnexo(unitOfWork);
            List<Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidadeAnexo> anexos = repositorioNaoConformidadeAnexo.BuscarAnexosPorNaoConformidade(naoConformidade?.Codigo ?? 0);

            Servicos.Embarcador.NotaFiscal.NaoConformidade svcNaoConformidade = new Servicos.Embarcador.NotaFiscal.NaoConformidade(unitOfWork);

            if ((naoConformidade?.ItemNaoConformidade?.PermiteContingencia ?? false) && anexos.Count == 0)
                throw new ServicoException("Item Não Conformidade está como Permite Contingência, portanto, é obrigatório inserir um anexo.");

            if (naoConformidade.Situacao != SituacaoNaoConformidade.AguardandoTratativa)
                return;

            SituacaoRegrasAutorizacao situacaoRegrasAutorizacao = ObterSituacaoRegrasAutorizacao(naoConformidade.Codigo, unitOfWork);

            if (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aguardando)
                return;

            Repositorio.Embarcador.NotaFiscal.NaoConformidade repositorioNaoConformidade = new Repositorio.Embarcador.NotaFiscal.NaoConformidade(unitOfWork);

            if (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aprovadas)
            {
                if (new Servicos.Embarcador.NotaFiscal.NaoConformidadeAprovacao(unitOfWork).LiberarProximaPrioridadeAprovacao(naoConformidade, TipoServicoMultisoftware))
                {

                    naoConformidade.Situacao = SituacaoNaoConformidade.Concluida;
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, naoConformidade, "Não conformidade aprovada", unitOfWork);
                    svcNaoConformidade.ValidarAutomaticamente(naoConformidade.CargaPedido.Carga);
                }
            }
            else
            {
                naoConformidade.Situacao = SituacaoNaoConformidade.Reprovada;
                Servicos.Auditoria.Auditoria.Auditar(Auditado, naoConformidade, "Não conformidade reprovada", unitOfWork);
            }

            repositorioNaoConformidade.Atualizar(naoConformidade);
            svcNaoConformidade.NotificarStatusParaTransportador(naoConformidade, true);
        }

        #endregion
    }
}