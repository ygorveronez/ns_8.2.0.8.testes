using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;
using Utilidades.Extensions;
using Zen.Barcode;

namespace SGT.WebAdmin.Controllers.WMS
{
    [CustomAuthorize("WMS/RecebimentoMercadoria")]
    public class RecebimentoMercadoriaController : BaseController
    {
		#region Construtores

		public RecebimentoMercadoriaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string observacao = Request.Params("Observacao");
                string numeroNota = Request.Params("NumeroNota");

                DateTime data;
                DateTime.TryParse(Request.Params("Data"), out data);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRecebimento situacao;
                Enum.TryParse(Request.Params("Situacao"), out situacao);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria tipoRecebimentoMercadoria;
                Enum.TryParse(Request.Params("TipoRecebimento"), out tipoRecebimentoMercadoria);

                int codigoUsuario = 0, codigoCarga = 0, codigoVeiculo = 0, codigoProdutoEmbarcador = 0, codigoMDFe = 0;
                int.TryParse(Request.Params("Usuario"), out codigoUsuario);
                int.TryParse(Request.Params("Carga"), out codigoCarga);
                int.TryParse(Request.Params("MDFe"), out codigoMDFe);
                int.TryParse(Request.Params("Veiculo"), out codigoVeiculo);
                int.TryParse(Request.Params("ProdutoEmbarcador"), out codigoProdutoEmbarcador);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 12, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Situação", "DescricaoSituacaoRecebimento", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Usuário", "Usuario", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Carga", "Carga", 12, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("MDFe", "MDFe", 8, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Veículo", "Veiculo", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Produto", "ProdutoEmbarcador", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo Recebimento", "DescricaoTipoRecebimentoMercadoria", 10,
                    Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo Operação", "TipoOperacao", 10, Models.Grid.Align.left, false);

                Repositorio.Embarcador.WMS.Recebimento repRecebimento =
                    new Repositorio.Embarcador.WMS.Recebimento(unitOfWork);
                List<Dominio.Entidades.Embarcador.WMS.Recebimento> listaRecebimento = repRecebimento.Consulta(
                    codigoMDFe, numeroNota, tipoRecebimentoMercadoria, codigoVeiculo, codigoProdutoEmbarcador, data,
                    situacao, codigoUsuario, codigoCarga, observacao, grid.header[grid.indiceColunaOrdena].data,
                    grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repRecebimento.ContaConsulta(codigoMDFe, numeroNota,
                    tipoRecebimentoMercadoria, codigoVeiculo, codigoProdutoEmbarcador, data, situacao, codigoUsuario,
                    codigoCarga, observacao));

                var lista = (from p in listaRecebimento
                    select new
                    {
                        p.Codigo,
                        Data = p.Data.ToString("dd/MM/yyyy"),
                        p.DescricaoSituacaoRecebimento,
                        Usuario = p.Usuario != null
                            ? "(" + p.Usuario.CPF_Formatado + ") " + p.Usuario.Nome
                            : string.Empty,
                        Carga = p.Carga != null ? p.Carga.CodigoCargaEmbarcador : string.Empty,
                        MDFe = p.MDFe != null ? p.MDFe.Numero.ToString("n0") : string.Empty,
                        Veiculo = p.Veiculo != null ? p.Veiculo.Placa : string.Empty,
                        ProdutoEmbarcador = p.ProdutoEmbarcador != null ? p.ProdutoEmbarcador.Descricao : string.Empty,
                        p.DescricaoTipoRecebimentoMercadoria,
                        TipoOperacao = p.Carga?.TipoOperacao?.Descricao ?? ""
                    }).ToList();
                grid.AdicionaRows(lista);
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

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaMercadoria()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.WMS.RecebimentoMercadoria repRecebimentoMercadoria =
                    new Repositorio.Embarcador.WMS.RecebimentoMercadoria(unidadeDeTrabalho);

                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CNPJCliente", false);
                grid.AdicionarCabecalho("NCM", false);
                grid.AdicionarCabecalho("ChaveNFe", false);
                grid.AdicionarCabecalho("Identificacao", false);
                grid.AdicionarCabecalho("CodigoDepositoPosicao", false);
                grid.AdicionarCabecalho("CodigoRecebimento", false);
                grid.AdicionarCabecalho("CodigoProdutoEmbarcador", false);
                grid.AdicionarCabecalho("DescricaoDepositoPosicao", false);
                grid.AdicionarCabecalho("DescricaoRecebimento", false);
                grid.AdicionarCabecalho("DescricaoProdutoEmbarcador", false);
                grid.AdicionarCabecalho("CodigoProduto", false);
                grid.AdicionarCabecalho("DescricaoProduto", false);
                grid.AdicionarCabecalho("ValorUnitario", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Nº Lote", "NumeroLote", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Cód. Barras", "CodigoBarras", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Vencimento", "DataVencimento", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Quantidade Lote", "QuantidadeLote", 12, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("M³", "MetroCubico", 12, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Peso", "Peso", 12, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Qtd. Palet", "QuantidadePalet", 12, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Armazenamento", "DepositoPosicao", 15, Models.Grid.Align.left, false);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                List<Dominio.Entidades.Embarcador.WMS.RecebimentoMercadoria> listaRecebimentoMercadoria =
                    repRecebimentoMercadoria.Consultar(codigo, propOrdena, grid.dirOrdena, grid.inicio,
                        grid.limite > 0 ? grid.limite : 1);
                grid.setarQuantidadeTotal(repRecebimentoMercadoria.ContarConsulta(codigo));

                var lista = (from obj in listaRecebimentoMercadoria
                    select new
                    {
                        Codigo = obj.Codigo,
                        Descricao = obj.Descricao,
                        NumeroLote = obj.NumeroLote,
                        CNPJCliente = obj.CNPJCliente,
                        NCM = obj.NCM,
                        ChaveNFe = obj.ChaveNFe,
                        Identificacao = obj.Identificacao,
                        CodigoBarras = obj.CodigoBarras,
                        DataVencimento = obj.DataVencimento != null && obj.DataVencimento.HasValue
                            ? obj.DataVencimento.Value.ToString("dd/MM/yyyy")
                            : string.Empty,
                        QuantidadeLote = obj.QuantidadeLote.ToString("n3"),
                        Peso = obj.Peso.ToString("n3"),
                        QuantidadePalet = obj.QuantidadePalet.ToString("n3"),
                        CodigoDepositoPosicao = obj.DepositoPosicao != null ? obj.DepositoPosicao.Codigo : 0,
                        CodigoRecebimento = obj.Recebimento.Codigo,
                        CodigoProdutoEmbarcador = obj.ProdutoEmbarcador != null ? obj.ProdutoEmbarcador.Codigo : 0,
                        DescricaoDepositoPosicao =
                            obj.DepositoPosicao != null ? obj.DepositoPosicao.Abreviacao : string.Empty,
                        DescricaoRecebimento = obj.Recebimento.Codigo.ToString(),
                        DescricaoProdutoEmbarcador = obj.ProdutoEmbarcador != null
                            ? obj.ProdutoEmbarcador.Descricao
                            : string.Empty,
                        DepositoPosicao = obj.DepositoPosicao != null ? obj.DepositoPosicao.Abreviacao : string.Empty,
                        CodigoProduto = obj.Produto != null ? obj.Produto.Codigo : 0,
                        DescricaoProduto = obj.Produto != null ? obj.Produto.Descricao : string.Empty,
                        ValorUnitario = obj.ValorUnitario.ToString("n4"),
                        Altura = obj.Altura,
                        Largura = obj.Largura,
                        Comprimento = obj.Comprimento,
                        MetroCubico = obj.MetroCubico
                    }).ToList();

                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar as mercadorias.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaVolume()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.WMS.RecebimentoMercadoria repRecebimentoMercadoria =
                    new Repositorio.Embarcador.WMS.RecebimentoMercadoria(unidadeDeTrabalho);

                int codigo = Request.GetIntParam("Codigo");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoEmissora", false);
                grid.AdicionarCabecalho("NomeEmissora", false);
                grid.AdicionarCabecalho("CNPJRemetente", false);
                grid.AdicionarCabecalho("CNPJDestinatario", false);
                grid.AdicionarCabecalho("NomeDestinatario", false);
                grid.AdicionarCabecalho("ChaveNFe", false);
                grid.AdicionarCabecalho("CodigoDepositoPosicao", false);
                grid.AdicionarCabecalho("CodigoRecebimento", false);
                grid.AdicionarCabecalho("CodigoBarras", false);
                grid.AdicionarCabecalho("DataEmissaoNF", false);
                grid.AdicionarCabecalho("NomeRemetente", false);
                grid.AdicionarCabecalho("Numero", false);
                grid.AdicionarCabecalho("NS Entrega", "Descricao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Nº Nota", "NumeroLote", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Nº Série", "Serie", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Qtd. Volume", "QuantidadeLote", 10, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Qtd. Conferida", "QuantidadeConferida", 10, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Qtd. Faltante", "QuantidadeFaltante", 10, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Remetente", "Remetente", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Armazenamento", "DescricaoDepositoPosicao", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Peso", "Peso", 8, Models.Grid.Align.right, true);

                grid.AdicionarCabecalho("Altura", "Altura", 5, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Largura", "Largura", 8, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Comprimento", "Comprimento", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("M³", "MetroCubico", 10, Models.Grid.Align.left, false);

                grid.AdicionarCabecalho("PesoBruto", false);
                grid.AdicionarCabecalho("PesoLiquido", false);
                grid.AdicionarCabecalho("ValorNF", false);
                grid.AdicionarCabecalho("ValorMercadoria", false);
                grid.AdicionarCabecalho("TipoUnidade", false);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                List<Dominio.Entidades.Embarcador.WMS.RecebimentoMercadoria> listaRecebimentoMercadoria =
                    repRecebimentoMercadoria.Consultar(codigo, propOrdena, grid.dirOrdena, grid.inicio,
                        grid.limite > 0 ? grid.limite : 1);
                grid.setarQuantidadeTotal(repRecebimentoMercadoria.ContarConsulta(codigo));

                var lista = (from obj in listaRecebimentoMercadoria
                    select new
                    {
                        Codigo = obj.Codigo,
                        CodigoEmissora = obj.EmpresaEmissora != null ? obj.EmpresaEmissora.Codigo : 0,
                        NomeEmissora = obj.EmpresaEmissora != null ? obj.EmpresaEmissora.RazaoSocial : string.Empty,
                        CNPJRemetente = obj.Remetente != null ? obj.Remetente.CPF_CNPJ : 0,
                        CNPJDestinatario = obj.Destinatario != null ? obj.Destinatario.CPF_CNPJ : 0,
                        NomeDestinatario = obj.Destinatario != null ? obj.Destinatario.Nome : string.Empty,
                        ChaveNFe = obj.ChaveNFe,
                        CodigoDepositoPosicao = obj.DepositoPosicao != null ? obj.DepositoPosicao.Codigo : 0,
                        CodigoRecebimento = obj.Recebimento.Codigo,
                        CodigoProdutoEmbarcador = obj.Recebimento.ProdutoEmbarcador != null
                            ? obj.Recebimento.ProdutoEmbarcador.Codigo
                            : 0,
                        CodigoBarras = obj.CodigoBarras,
                        DataEmissaoNF = obj.DataEmissaoNF != null && obj.DataEmissaoNF.HasValue
                            ? obj.DataEmissaoNF.Value.ToString("dd/MM/yyyy")
                            : string.Empty,
                        Descricao = obj.Descricao,
                        Numero = obj.NumeroLote,
                        NumeroLote = obj.NumeroLote,
                        Serie = obj.Serie,
                        QuantidadeLote = obj.QuantidadeLote.ToString("n3"),
                        QuantidadeConferida = obj.QuantidadeConferida.ToString("n3"),
                        QuantidadeFaltante = obj.QuantidadeFaltante.ToString("n3"),
                        NomeRemetente = obj.Remetente != null ? obj.Remetente.Nome : string.Empty,
                        Remetente = obj.Remetente != null ? obj.Remetente.Nome : string.Empty,
                        DescricaoDepositoPosicao =
                            obj.DepositoPosicao != null ? obj.DepositoPosicao.Abreviacao : string.Empty,
                        Peso = obj.Peso,
                        Altura = obj.Altura,
                        Largura = obj.Largura,
                        Comprimento = obj.Comprimento,
                        MetroCubico = obj.MetroCubico,
                        PesoBruto = obj.PesoBruto.ToString("n2"),
                        PesoLiquido = obj.PesoLiquido.ToString("n2"),
                        ValorNF = obj.ValorNF.ToString("n2"),
                        ValorMercadoria = obj.ValorMercadoria.ToString("n2"),
                        obj.TipoUnidade
                    }).ToList();

                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar as mercadorias.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> EnviarArquivo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();
                int codigoRecebimento = 0, codigoProdutoEmbarcador = 0;
                int.TryParse(Request.Params("Recebimento"), out codigoRecebimento);
                int.TryParse(Request.Params("ProdutoEmbarcador"), out codigoProdutoEmbarcador);
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria tipoRecebimento;
                Enum.TryParse(Request.Params("TipoRecebimento"), out tipoRecebimento);
                Repositorio.Embarcador.WMS.Recebimento repRecebimento =
                    new Repositorio.Embarcador.WMS.Recebimento(unitOfWork);
                Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador =
                    new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);
                Dominio.Entidades.Embarcador.WMS.Recebimento recebimento;
                if (codigoRecebimento > 0)
                    recebimento = repRecebimento.BuscarPorCodigo(codigoRecebimento);
                else
                {
                    recebimento = new Dominio.Entidades.Embarcador.WMS.Recebimento();
                    recebimento.Data = DateTime.Now;
                    recebimento.Usuario = this.Usuario;
                    recebimento.SituacaoRecebimento =
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRecebimento.Iniciada;
                    recebimento.ProdutoEmbarcador = repProdutoEmbarcador.BuscarPorCodigo(codigoProdutoEmbarcador);
                    recebimento.TipoRecebimentoMercadoria = tipoRecebimento;

                    repRecebimento.Inserir(recebimento);
                }

                Servicos.Embarcador.Financeiro.DocumentoEntrada svcDocumentoEntrada =
                    new Servicos.Embarcador.Financeiro.DocumentoEntrada(unitOfWork);

                List<RetornoArquivo> retornoArquivos = new List<RetornoArquivo>();

                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

                if (files.Count > 0)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        Servicos.DTO.CustomFile file = files[i];
                        string extensao = System.IO.Path.GetExtension(file.FileName).ToLower();
                        string nomeArquivo = System.IO.Path.GetFileName(file.FileName);

                        RetornoArquivo retornoArquivo = new RetornoArquivo();
                        retornoArquivo.nome = file.FileName;
                        retornoArquivo.processada = true;
                        retornoArquivo.mensagem = "";
                        retornoArquivo.CodigoRecebimento = recebimento.Codigo;
                        retornoArquivo.Mercadorias = new List<Item>();
                        byte[] arquivo = null;
                        using (Stream inputStream = file.InputStream)
                        {
                            MemoryStream memoryStream = inputStream as MemoryStream;
                            if (memoryStream == null)
                            {
                                memoryStream = new MemoryStream();
                                inputStream.CopyTo(memoryStream);
                            }

                            arquivo = memoryStream.ToArray();
                        }

                        if (extensao.Equals(".xml"))
                        {
                            try
                            {
                                Stream stream = new MemoryStream(arquivo);
                                var notaFiscal = MultiSoftware.NFe.Servicos.Leitura.Ler(stream);
                                if (notaFiscal != null)
                                {
                                    //object documento = svcDocumentoEntrada.ObterDetalhesPorNFe(notaFiscal, Empresa, unitOfWork);
                                    MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc nfeProc3 = null;
                                    MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc nfeProc4 = null;

                                    if (notaFiscal.GetType() ==
                                        typeof(MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc))
                                        nfeProc3 = (MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc)notaFiscal;
                                    else if (notaFiscal.GetType() ==
                                             typeof(MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc))
                                        nfeProc4 = (MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc)notaFiscal;
                                    else
                                    {
                                        retornoArquivo.processada = false;
                                        retornoArquivo.mensagem =
                                            "O xml informado não é uma NF-e, por favor verifique.";
                                    }

                                    if (nfeProc3 != null || nfeProc4 != null)
                                    {
                                        if (nfeProc3 != null)
                                            retornoArquivo.Mercadorias = RetornaListaItens(nfeProc3, unitOfWork,
                                                recebimento, tipoRecebimento, codigoProdutoEmbarcador);
                                        else if (nfeProc4 != null)
                                        {
                                            retornoArquivo.Mercadorias = RetornaListaItens(nfeProc4, unitOfWork,
                                                recebimento, tipoRecebimento, codigoProdutoEmbarcador, arquivo);
                                        }
                                    }
                                }
                                else
                                {
                                    retornoArquivo.processada = false;
                                    retornoArquivo.mensagem = "O xml informado não é uma NF-e, por favor verifique.";
                                }
                            }
                            catch (Exception ex)
                            {
                                Servicos.Log.TratarErro(ex);
                                retornoArquivo.processada = false;
                                retornoArquivo.mensagem =
                                    "Ocorreu uma falha ao enviar o xml, verifique se o arquivo é um documento fiscal válido. " +
                                    ex.Message;
                            }
                            finally
                            {
                                file.InputStream.Dispose();
                            }
                        }
                        else
                        {
                            retornoArquivo.processada = false;
                            retornoArquivo.mensagem = "A extensão do arquivo é inválida ou não está cadastrada.";
                        }

                        retornoArquivos.Add(retornoArquivo);
                    }

                    unitOfWork.CommitChanges();
                    var dadosRetorno = new
                    {
                        CodigoRecebimento = recebimento.Codigo,
                        Arquivos = retornoArquivos
                    };
                    return new JsonpResult(dadosRetorno);
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Não foi enviado o arquivo.");
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao enviar o CT-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ValidarMDFeSelecionada()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.WMS.Recebimento repRecebimento =
                    new Repositorio.Embarcador.WMS.Recebimento(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe =
                    new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);
                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe =
                    new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
                Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador =
                    new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                unitOfWork.Start();

                int codigoRecebimento = 0, codigoMDFe = 0, codigoProduto = 0;
                int.TryParse(Request.Params("CodigoRecebimento"), out codigoRecebimento);
                int.TryParse(Request.Params("CodigoMDFe"), out codigoMDFe);
                int.TryParse(Request.Params("CodigoProduto"), out codigoProduto);
                int.TryParse(Request.Params("Usuario"), out int codigoUsuario);
                string observacao = Request.Params("Observacao");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRecebimento situacao;
                Enum.TryParse(Request.Params("Situacao"), out situacao);
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria tipoRecebimentoMercadoria;
                Enum.TryParse(Request.Params("TipoRecebimento"), out tipoRecebimentoMercadoria);

                if (repRecebimento.ContemRecebimentoAbertoMDFe(codigoRecebimento, codigoMDFe))
                    return new JsonpResult(false,
                        "Existe recebimento em aberto para este MDF-e, ou carga vinculada a este MDF-e.");
                else
                {
                    Dominio.Entidades.Embarcador.WMS.Recebimento recebimento;
                    if (codigoRecebimento > 0)
                        recebimento = repRecebimento.BuscarPorCodigo(codigoRecebimento, true);
                    else
                        recebimento = new Dominio.Entidades.Embarcador.WMS.Recebimento();

                    if (codigoMDFe > 0)
                    {
                        recebimento.MDFe = repMDFe.BuscarPorCodigo(codigoMDFe);
                        recebimento.Carga = repCargaMDFe.BuscarCargaPorMDFe(codigoMDFe);
                        if (recebimento.Carga != null && recebimento.Carga.Veiculo != null)
                            recebimento.Veiculo = recebimento.Carga.Veiculo;
                    }

                    if (codigoProduto > 0)
                        recebimento.ProdutoEmbarcador = repProdutoEmbarcador.BuscarPorCodigo(codigoProduto);

                    recebimento.Observacao = observacao;
                    recebimento.Usuario = repUsuario.BuscarPorCodigo(codigoUsuario);
                    recebimento.SituacaoRecebimento = situacao;
                    recebimento.TipoRecebimentoMercadoria = tipoRecebimentoMercadoria;

                    if (codigoRecebimento > 0)
                        repRecebimento.Atualizar(recebimento, Auditado);
                    else
                    {
                        recebimento.Data = DateTime.Now;
                        repRecebimento.Inserir(recebimento);
                    }

                    ControlarFluxoPatio(recebimento, unitOfWork);

                    unitOfWork.CommitChanges();

                    var retorno = new
                    {
                        CodigoCarga = recebimento.Carga?.Codigo,
                        CodigoCargaEmbarcador = recebimento.Carga?.CodigoCargaEmbarcador,
                        CodigoMDFe = recebimento.MDFe?.Codigo,
                        Numero = recebimento.MDFe?.Numero,
                        CodigoVeiculo = recebimento.Veiculo?.Codigo,
                        Placa = recebimento.Veiculo?.Placa,
                        CodigoRecebimento = recebimento.Codigo
                    };

                    return new JsonpResult(retorno, true, "Sucesso");
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os recebimentos para este MDFe.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ValidarCargaSelecionada()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.WMS.Recebimento repRecebimento =
                    new Repositorio.Embarcador.WMS.Recebimento(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe =
                    new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador =
                    new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);
                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe =
                    new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                unitOfWork.Start();

                int codigoRecebimento = 0, codigoCarga = 0, codigoProduto = 0;
                int.TryParse(Request.Params("CodigoRecebimento"), out codigoRecebimento);
                int.TryParse(Request.Params("CodigoCarga"), out codigoCarga);
                int.TryParse(Request.Params("CodigoProduto"), out codigoProduto);
                int.TryParse(Request.Params("Usuario"), out int codigoUsuario);
                string observacao = Request.Params("Observacao");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRecebimento situacao;
                Enum.TryParse(Request.Params("Situacao"), out situacao);
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria tipoRecebimentoMercadoria;
                Enum.TryParse(Request.Params("TipoRecebimento"), out tipoRecebimentoMercadoria);

                List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> mdfes = repCargaMDFe.BuscarPorCarga(codigoCarga);
                if (repRecebimento.ContemRecebimentoAbertoCarga(codigoRecebimento, codigoCarga))
                    return new JsonpResult(false, "Existe recebimento em aberto para esta carga.");
                else
                {
                    if (mdfes != null && mdfes.Count > 0)
                    {
                        foreach (var mdfe in mdfes)
                        {
                            if (repRecebimento.ContemRecebimentoAbertoMDFe(codigoRecebimento, mdfe.MDFe.Codigo))
                                return new JsonpResult(false,
                                    "Existe recebimento em aberto para um MDF-e vinculado a esta carga.");
                        }
                    }

                    Dominio.Entidades.Embarcador.WMS.Recebimento recebimento;
                    if (codigoRecebimento > 0)
                        recebimento = repRecebimento.BuscarPorCodigo(codigoRecebimento, true);
                    else
                        recebimento = new Dominio.Entidades.Embarcador.WMS.Recebimento();

                    if (codigoCarga > 0)
                    {
                        recebimento.MDFe = repCargaMDFe.BuscarMDFePorCarga(codigoCarga);
                        recebimento.Carga = repCarga.BuscarPorCodigo(codigoCarga);
                        if (recebimento.Carga != null && recebimento.Carga.Veiculo != null)
                            recebimento.Veiculo = recebimento.Carga.Veiculo;
                    }

                    if (codigoProduto > 0)
                        recebimento.ProdutoEmbarcador = repProdutoEmbarcador.BuscarPorCodigo(codigoProduto);

                    recebimento.Observacao = observacao;
                    recebimento.Usuario = repUsuario.BuscarPorCodigo(codigoUsuario);
                    recebimento.SituacaoRecebimento = situacao;
                    recebimento.TipoRecebimentoMercadoria = tipoRecebimentoMercadoria;

                    if (codigoRecebimento > 0)
                        repRecebimento.Atualizar(recebimento, Auditado);
                    else
                    {
                        recebimento.Data = DateTime.Now;
                        repRecebimento.Inserir(recebimento);
                    }

                    ControlarFluxoPatio(recebimento, unitOfWork);

                    unitOfWork.CommitChanges();

                    var retorno = new
                    {
                        CodigoCarga = recebimento.Carga?.Codigo,
                        CodigoCargaEmbarcador = recebimento.Carga?.CodigoCargaEmbarcador,
                        CodigoMDFe = recebimento.MDFe?.Codigo,
                        Numero = recebimento.MDFe?.Numero,
                        CodigoVeiculo = recebimento.Veiculo?.Codigo,
                        Placa = recebimento.Veiculo?.Placa,
                        CodigoRecebimento = recebimento.Codigo,
                        CodigoEmpresa = recebimento.Carga?.Empresa?.Codigo ?? 0,
                        Empresa = recebimento.Carga?.Empresa?.Descricao ?? ""
                    };
                    return new JsonpResult(retorno, true, "Sucesso");
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os recebimentos para esta carga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarProdutosCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();
                int codigoRecebimento = 0, codigoCarga = 0;
                int.TryParse(Request.Params("CodigoRecebimento"), out codigoRecebimento);
                int.TryParse(Request.Params("CodigoCarga"), out codigoCarga);

                Repositorio.Embarcador.Produtos.ProdutoEmbarcadorCliente repProdutoEmbarcadorCliente =
                    new Repositorio.Embarcador.Produtos.ProdutoEmbarcadorCliente(unitOfWork);
                Servicos.Embarcador.WMS.LoteProdutoEmbarcador svcLoteProdutoEmbarcadora =
                    new Servicos.Embarcador.WMS.LoteProdutoEmbarcador(unitOfWork);
                Repositorio.Embarcador.WMS.Recebimento repRecebimento =
                    new Repositorio.Embarcador.WMS.Recebimento(unitOfWork);
                Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal =
                    new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

                Dominio.Entidades.Embarcador.WMS.Recebimento recebimento =
                    repRecebimento.BuscarPorCodigo(codigoRecebimento);

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido =
                    new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoProduto repPedidoProduto =
                    new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> pedidos =
                    repCargaPedido.BuscarPorCarga(codigoCarga);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> listaNotas =
                    repPedidoXMLNotaFiscal.BuscarNotasFiscaisPorCarga(codigoCarga);
                List<Item> retornoArquivos = new List<Item>();

                Dominio.Entidades.Embarcador.Pedidos.PedidoProduto produtoPrincipal =
                    repPedidoProduto.BuscarPrimeiroPorCarga(codigoCarga);

                if (TipoServicoMultisoftware ==
                    AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador &&
                    listaNotas != null && listaNotas.Count > 0 && produtoPrincipal != null)
                {
                    for (int i = 0; i < listaNotas.Count; i++)
                    {
                        Item item = new Item();

                        Dominio.Entidades.Embarcador.WMS.DepositoPosicao posicao =
                            svcLoteProdutoEmbarcadora.BuscarPosicaoPadrao(produtoPrincipal.Produto,
                                listaNotas[i].Volumes, listaNotas[i].Peso * listaNotas[i].Volumes,
                                produtoPrincipal.MetroCubico * listaNotas[i].Volumes, produtoPrincipal.QuantidadePalet,
                                unitOfWork);

                        Dominio.Entidades.Produto produto =
                            repProduto.BuscarPorCodigoProduto(produtoPrincipal.Produto.CodigoProdutoEmbarcador);

                        item.Codigo = 0;
                        item.CNPJCliente = 0;
                        item.Descricao = produtoPrincipal.Produto.Descricao;
                        item.NumeroLote = "";
                        item.NCM = "";
                        item.ChaveNFe = "";
                        item.Identificacao = pedidos[i].Carga.CodigoCargaEmbarcador;
                        item.CodigoBarras = produtoPrincipal.Produto.CodigoProdutoEmbarcador;
                        item.DataVencimento = string.Empty;
                        item.QuantidadeLote = listaNotas[i].Volumes.ToString("n3");
                        item.MetroCubico = (listaNotas[i].Volumes * produtoPrincipal.MetroCubico).ToString("n3");
                        item.Peso = (listaNotas[i].Volumes * listaNotas[i].Peso).ToString("n3");
                        item.QuantidadePalet = produtoPrincipal.QuantidadePalet.ToString("n3");
                        item.CodigoDepositoPosicao = posicao != null ? posicao.Codigo : 0;
                        item.CodigoRecebimento = recebimento != null ? recebimento.Codigo : 0;
                        item.CodigoProdutoEmbarcador = produtoPrincipal.Produto.Codigo;
                        item.DescricaoDepositoPosicao = posicao != null ? posicao.Abreviacao : "";
                        item.DescricaoRecebimento = recebimento != null ? recebimento.Codigo.ToString() : "";
                        item.DescricaoProdutoEmbarcador = produtoPrincipal.Produto.Descricao;
                        if (produto != null)
                        {
                            item.ValorUnitario = produto.ValorVenda.ToString("n4");
                            item.CodigoProduto = produto.Codigo;
                            item.DescricaoProduto = produto.Descricao;
                        }

                        item.CodigoMDFe =
                            carga != null && carga.CargaMDFes != null && carga.CargaMDFes.Count > 0 &&
                            carga.CargaMDFes[0] != null && carga.CargaMDFes[0].MDFe != null
                                ? carga.CargaMDFes[0].MDFe.Codigo
                                : 0;
                        item.NumeroMDFe =
                            carga != null && carga.CargaMDFes != null && carga.CargaMDFes.Count > 0 &&
                            carga.CargaMDFes[0] != null && carga.CargaMDFes[0].MDFe != null
                                ? carga.CargaMDFes[0].MDFe.Numero.ToString("n0")
                                : string.Empty;
                        item.CodigoCarga = carga != null ? carga.Codigo : 0;
                        item.NumeroCarga = carga != null ? carga.CodigoCargaEmbarcador : string.Empty;
                        item.CodigoVeiculo = carga != null && carga.Veiculo != null ? carga.Veiculo.Codigo : 0;
                        item.PlacaVeiculo = carga != null && carga.Veiculo != null ? carga.Veiculo.Placa : string.Empty;

                        item.PesoBruto = listaNotas[i].Peso.ToString("n2");
                        item.PesoLiquido = listaNotas[i].PesoLiquido.ToString("n2");
                        item.NumeroNF = listaNotas[i].Numero.ToString("D");
                        item.SerieNF = listaNotas[i].Serie;
                        item.ValorMercadoria = listaNotas[i].ValorTotalProdutos.ToString("n2");
                        item.ValorNF = listaNotas[i].Valor.ToString("n2");
                        item.VolumeNF = listaNotas[i].Volumes.ToString("D");

                        retornoArquivos.Add(item);

                        SalvarItemRecebimento(recebimento, item, unitOfWork,
                            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria.Mercadoria, null);
                    }
                }
                else
                {
                    for (int i = 0; i < pedidos.Count; i++)
                    {
                        for (int k = 0; k < pedidos[i].Pedido.Produtos.Count; k++)
                        {
                            Item item = new Item();

                            Dominio.Entidades.Embarcador.WMS.DepositoPosicao posicao =
                                svcLoteProdutoEmbarcadora.BuscarPosicaoPadrao(pedidos[i].Pedido.Produtos[k].Produto,
                                    pedidos[i].Pedido.Produtos[k].Quantidade,
                                    pedidos[i].Pedido.Produtos[k].PesoUnitario *
                                    pedidos[i].Pedido.Produtos[k].Quantidade,
                                    pedidos[i].Pedido.Produtos[k].MetroCubico *
                                    pedidos[i].Pedido.Produtos[k].Quantidade,
                                    pedidos[i].Pedido.Produtos[k].QuantidadePalet, unitOfWork);

                            Dominio.Entidades.Produto produto =
                                repProduto.BuscarPorCodigoProduto(pedidos[i].Pedido.Produtos[k].Produto
                                    .CodigoProdutoEmbarcador);

                            item.Codigo = 0;
                            item.CNPJCliente = 0;
                            item.Descricao = pedidos[i].Pedido.Produtos[k].Produto.Descricao;
                            item.NumeroLote = "";
                            item.NCM = "";
                            item.ChaveNFe = "";
                            item.Identificacao = pedidos[i].Carga.CodigoCargaEmbarcador;
                            item.CodigoBarras = pedidos[i].Pedido.Produtos[k].Produto.CodigoProdutoEmbarcador;
                            item.DataVencimento = string.Empty;
                            item.QuantidadeLote = pedidos[i].Pedido.Produtos[k].Quantidade.ToString("n3");
                            item.MetroCubico =
                                (pedidos[i].Pedido.Produtos[k].Quantidade * pedidos[i].Pedido.Produtos[k].MetroCubico)
                                .ToString("n3");
                            item.Peso = (pedidos[i].Pedido.Produtos[k].Quantidade *
                                         pedidos[i].Pedido.Produtos[k].PesoUnitario).ToString("n3");
                            item.QuantidadePalet = pedidos[i].Pedido.Produtos[k].QuantidadePalet.ToString("n3");
                            item.CodigoDepositoPosicao = posicao != null ? posicao.Codigo : 0;
                            item.CodigoRecebimento = recebimento != null ? recebimento.Codigo : 0;
                            item.CodigoProdutoEmbarcador = pedidos[i].Pedido.Produtos[k].Produto.Codigo;
                            item.DescricaoDepositoPosicao = posicao != null ? posicao.Abreviacao : "";
                            item.DescricaoRecebimento = recebimento != null ? recebimento.Codigo.ToString() : "";
                            item.DescricaoProdutoEmbarcador = pedidos[i].Pedido.Produtos[k].Produto.Descricao;
                            if (produto != null)
                            {
                                item.ValorUnitario = produto.ValorVenda.ToString("n4");
                                item.CodigoProduto = produto.Codigo;
                                item.DescricaoProduto = produto.Descricao;
                            }

                            item.CodigoMDFe =
                                carga != null && carga.CargaMDFes != null && carga.CargaMDFes.Count > 0 &&
                                carga.CargaMDFes[0] != null && carga.CargaMDFes[0].MDFe != null
                                    ? carga.CargaMDFes[0].MDFe.Codigo
                                    : 0;
                            item.NumeroMDFe =
                                carga != null && carga.CargaMDFes != null && carga.CargaMDFes.Count > 0 &&
                                carga.CargaMDFes[0] != null && carga.CargaMDFes[0].MDFe != null
                                    ? carga.CargaMDFes[0].MDFe.Numero.ToString("n0")
                                    : string.Empty;
                            item.CodigoCarga = carga != null ? carga.Codigo : 0;
                            item.NumeroCarga = carga != null ? carga.CodigoCargaEmbarcador : string.Empty;
                            item.CodigoVeiculo = carga != null && carga.Veiculo != null ? carga.Veiculo.Codigo : 0;
                            item.PlacaVeiculo = carga != null && carga.Veiculo != null
                                ? carga.Veiculo.Placa
                                : string.Empty;

                            item.PesoBruto = string.Empty;
                            item.PesoLiquido = string.Empty;
                            item.NumeroNF = string.Empty;
                            item.SerieNF = string.Empty;
                            item.ValorMercadoria = string.Empty;
                            item.ValorNF = string.Empty;
                            item.VolumeNF = string.Empty;

                            retornoArquivos.Add(item);

                            SalvarItemRecebimento(recebimento, item, unitOfWork,
                                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria.Mercadoria,
                                null);
                        }
                    }
                }

                unitOfWork.CommitChanges();
                var dadosRetorno = new
                {
                    Mercadorias = retornoArquivos,
                    CodigoRecebimento =
                        retornoArquivos != null ? retornoArquivos.FirstOrDefault().CodigoRecebimento : 0,
                    CodigoCarga = retornoArquivos != null ? retornoArquivos.FirstOrDefault().CodigoCarga : 0,
                    CodigoCargaEmbarcador = retornoArquivos != null ? retornoArquivos.FirstOrDefault().NumeroCarga : "",
                    CodigoMDFe = retornoArquivos != null ? retornoArquivos.FirstOrDefault().CodigoMDFe : 0,
                    Numero = retornoArquivos != null ? retornoArquivos.FirstOrDefault().NumeroMDFe : "",
                    CodigoVeiculo = retornoArquivos != null ? retornoArquivos.FirstOrDefault().CodigoVeiculo : 0,
                    Placa = retornoArquivos != null ? retornoArquivos.FirstOrDefault().PlacaVeiculo : "",
                    CodigoEmpresa = carga?.Empresa?.Codigo ?? 0,
                    Empresa = carga?.Empresa?.Descricao ?? ""
                };
                return new JsonpResult(dadosRetorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os produtos da carga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarProdutosMDFe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();
                int codigoRecebimento = 0, codigoMDFe = 0;
                int.TryParse(Request.Params("CodigoRecebimento"), out codigoRecebimento);
                int.TryParse(Request.Params("CodigoMDFe"), out codigoMDFe);

                Repositorio.Embarcador.Produtos.ProdutoEmbarcadorCliente repProdutoEmbarcadorCliente = new Repositorio.Embarcador.Produtos.ProdutoEmbarcadorCliente(unitOfWork);
                Servicos.Embarcador.WMS.LoteProdutoEmbarcador svcLoteProdutoEmbarcadora = new Servicos.Embarcador.WMS.LoteProdutoEmbarcador(unitOfWork);
                Repositorio.Embarcador.WMS.Recebimento repRecebimento = new Repositorio.Embarcador.WMS.Recebimento(unitOfWork);
                Dominio.Entidades.Embarcador.WMS.Recebimento recebimento = repRecebimento.BuscarPorCodigo(codigoRecebimento);
                Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);

                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido =
                    new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>
                    pedidos = repCargaPedido.BuscarPorMDFe(codigoMDFe);

                List<Item> retornoArquivos = new List<Item>();

                for (int i = 0; i < pedidos.Count; i++)
                {
                    for (int k = 0; k < pedidos[i].Pedido.Produtos.Count; k++)
                    {
                        Item item = new Item();

                        Dominio.Entidades.Embarcador.WMS.DepositoPosicao posicao =
                            svcLoteProdutoEmbarcadora.BuscarPosicaoPadrao(pedidos[i].Pedido.Produtos[k].Produto,
                                pedidos[i].Pedido.Produtos[k].Quantidade,
                                pedidos[i].Pedido.Produtos[k].PesoUnitario * pedidos[i].Pedido.Produtos[k].Quantidade,
                                pedidos[i].Pedido.Produtos[k].MetroCubico * pedidos[i].Pedido.Produtos[k].Quantidade,
                                pedidos[i].Pedido.Produtos[k].QuantidadePalet, unitOfWork);

                        Dominio.Entidades.Produto produto =
                            repProduto.BuscarPorCodigoProduto(pedidos[i].Pedido.Produtos[k].Produto
                                .CodigoProdutoEmbarcador);

                        item.Codigo = 0;
                        item.CNPJCliente = 0;
                        item.Descricao = pedidos[i].Pedido.Produtos[k].Produto.Descricao;
                        item.NumeroLote = "";
                        item.NCM = "";
                        item.ChaveNFe = "";
                        item.Identificacao = pedidos[i].Carga.CodigoCargaEmbarcador;
                        item.CodigoBarras = pedidos[i].Pedido.Produtos[k].Produto.CodigoProdutoEmbarcador;
                        item.DataVencimento = string.Empty;
                        item.QuantidadeLote = pedidos[i].Pedido.Produtos[k].Quantidade.ToString("n3");
                        item.MetroCubico =
                            (pedidos[i].Pedido.Produtos[k].Quantidade * pedidos[i].Pedido.Produtos[k].MetroCubico)
                            .ToString("n3");
                        item.Peso = (pedidos[i].Pedido.Produtos[k].Quantidade *
                                     pedidos[i].Pedido.Produtos[k].PesoUnitario).ToString("n3");
                        item.QuantidadePalet = pedidos[i].Pedido.Produtos[k].QuantidadePalet.ToString("n3");
                        item.CodigoDepositoPosicao = posicao != null ? posicao.Codigo : 0;
                        item.CodigoRecebimento = recebimento != null ? recebimento.Codigo : 0;
                        item.CodigoProdutoEmbarcador = pedidos[i].Pedido.Produtos[k].Produto.Codigo;
                        item.DescricaoDepositoPosicao = posicao != null ? posicao.Abreviacao : "";
                        item.DescricaoRecebimento = recebimento != null ? recebimento.Codigo.ToString() : "";
                        item.DescricaoProdutoEmbarcador = pedidos[i].Pedido.Produtos[k].Produto.Descricao;
                        if (produto != null)
                        {
                            item.ValorUnitario = produto.ValorVenda.ToString("n4");
                            item.CodigoProduto = produto.Codigo;
                            item.DescricaoProduto = produto.Descricao;
                        }

                        item.CodigoMDFe = 0;
                        item.NumeroMDFe = string.Empty;
                        item.CodigoCarga = pedidos[i].Carga.Codigo;
                        item.NumeroCarga = pedidos[i].Carga.CodigoCargaEmbarcador;
                        item.CodigoVeiculo = pedidos[i].Carga.Veiculo != null ? pedidos[i].Carga.Veiculo.Codigo : 0;
                        item.PlacaVeiculo = pedidos[i].Carga.Veiculo != null
                            ? pedidos[i].Carga.Veiculo.Placa
                            : string.Empty;

                        item.PesoBruto = string.Empty;
                        item.PesoLiquido = string.Empty;
                        item.NumeroNF = string.Empty;
                        item.SerieNF = string.Empty;
                        item.ValorMercadoria = string.Empty;
                        item.ValorNF = string.Empty;
                        item.VolumeNF = string.Empty;

                        retornoArquivos.Add(item);

                        SalvarItemRecebimento(recebimento, item, unitOfWork,
                            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria.Mercadoria, null);
                    }
                }

                unitOfWork.CommitChanges();
                var dadosRetorno = new
                {
                    Mercadorias = retornoArquivos
                };
                return new JsonpResult(dadosRetorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os produtos do MDFe.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarVolumesCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();
                int codigoRecebimento = 0, codigoCarga = 0, codigoProduto = 0;
                int.TryParse(Request.Params("CodigoRecebimento"), out codigoRecebimento);
                int.TryParse(Request.Params("CodigoProdutoEmbarcador"), out codigoProduto);
                int.TryParse(Request.Params("CodigoCarga"), out codigoCarga);

                System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");
                Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador =
                    new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);
                Servicos.Embarcador.WMS.LoteProdutoEmbarcador svcLoteProdutoEmbarcadora =
                    new Servicos.Embarcador.WMS.LoteProdutoEmbarcador(unitOfWork);
                Repositorio.Embarcador.WMS.Recebimento repRecebimento =
                    new Repositorio.Embarcador.WMS.Recebimento(unitOfWork);
                Repositorio.Embarcador.WMS.RecebimentoMercadoria repRecebimentoMercadoria =
                    new Repositorio.Embarcador.WMS.RecebimentoMercadoria(unitOfWork);
                Dominio.Entidades.Embarcador.WMS.Recebimento recebimento =
                    repRecebimento.BuscarPorCodigo(codigoRecebimento);
                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador =
                    repProdutoEmbarcador.BuscarPorCodigo(codigoProduto);

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal =
                    new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais =
                    repXMLNotaFiscal.BuscarPorCarga(codigoCarga);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                List<Item> retornoArquivos = new List<Item>();
                bool codigoBarrasValido = true;
                for (int i = 0; i < notasFiscais.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal = notasFiscais[i];

                    codigoBarrasValido = true;

                    if (!string.IsNullOrWhiteSpace(notaFiscal.NumeroSolicitacao))
                    {
                        if (repRecebimentoMercadoria.NumeroSolicitacaoJaRecebido(notaFiscal.NumeroSolicitacao))
                        {
                            codigoBarrasValido =
                                repRecebimentoMercadoria.NumeroSolicitacaoPendenteDeConferencia(notaFiscal
                                    .NumeroSolicitacao);
                        }
                    }

                    if (codigoBarrasValido)
                    {
                        Item item = new Item();

                        Dominio.Entidades.Embarcador.WMS.DepositoPosicao posicao =
                            svcLoteProdutoEmbarcadora.BuscarPosicaoPadrao(produtoEmbarcador, notaFiscal.Volumes, 0, 0,
                                0, unitOfWork);

                        item.Codigo = 0;
                        item.CNPJCliente = 0;
                        item.Descricao = notaFiscal.NumeroSolicitacao;
                        item.NumeroLote = notaFiscal.Numero.ToString();
                        item.NCM = "";
                        item.ChaveNFe = notaFiscal.Chave;
                        item.Identificacao = notaFiscal.NumeroSolicitacao;
                        item.CodigoBarras = string.Empty;
                        item.DataVencimento = string.Empty;
                        item.QuantidadeLote = notaFiscal.Volumes.ToString("n3");
                        item.MetroCubico = "0,000";
                        item.Peso = "0,000";
                        item.QuantidadePalet = "0,000";
                        item.CodigoDepositoPosicao = posicao?.Codigo ?? 0;
                        item.CodigoRecebimento = recebimento?.Codigo ?? 0;
                        item.CodigoProdutoEmbarcador = codigoProduto;
                        item.DescricaoDepositoPosicao = posicao?.Abreviacao ?? "";
                        item.DescricaoRecebimento = recebimento?.Codigo.ToString() ?? "";
                        item.DescricaoProdutoEmbarcador = produtoEmbarcador?.Descricao ?? string.Empty;

                        item.CodigoEmissora = carga.Empresa.Codigo;
                        item.NomeEmissora = carga.Empresa.RazaoSocial;

                        if (notaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores
                                .TipoOperacaoNotaFiscal.Entrada)
                        {
                            item.CNPJRemetente =
                                notaFiscal.Recebedor != null &&
                                TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores
                                    .TipoServicoMultisoftware.MultiEmbarcador && notaFiscal.Recebedor.CPF_CNPJ > 0
                                    ? notaFiscal.Recebedor.CPF_CNPJ
                                    : notaFiscal.Destinatario.CPF_CNPJ;
                            item.NomeRemetente =
                                notaFiscal.Recebedor != null &&
                                TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores
                                    .TipoServicoMultisoftware.MultiEmbarcador && notaFiscal.Recebedor.CPF_CNPJ > 0
                                    ? notaFiscal.Recebedor.Nome
                                    : notaFiscal.Destinatario.Nome;
                            item.CNPJDestinatario = notaFiscal.Emitente.CPF_CNPJ;
                            item.NomeDestinatario = notaFiscal.Emitente.Nome;
                        }
                        else
                        {
                            item.CNPJRemetente = notaFiscal.Emitente.CPF_CNPJ;
                            item.NomeRemetente = notaFiscal.Emitente.Nome;
                            item.CNPJDestinatario =
                                notaFiscal.Recebedor != null &&
                                TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores
                                    .TipoServicoMultisoftware.MultiEmbarcador && notaFiscal.Recebedor.CPF_CNPJ > 0
                                    ? notaFiscal.Recebedor.CPF_CNPJ
                                    : notaFiscal.Destinatario.CPF_CNPJ;
                            item.NomeDestinatario =
                                notaFiscal.Recebedor != null &&
                                TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores
                                    .TipoServicoMultisoftware.MultiEmbarcador && notaFiscal.Recebedor.CPF_CNPJ > 0
                                    ? notaFiscal.Recebedor.Nome
                                    : notaFiscal.Destinatario.Nome;
                        }

                        item.DataEmissaoNF = notaFiscal.DataEmissao.ToString("dd/MM/yyyy");
                        item.Numero = Utilidades.String.OnlyNumbers(notaFiscal.Numero.ToString("n0"));
                        item.Serie = Utilidades.String.OnlyNumbers(notaFiscal.Serie);
                        item.QuantidadeConferida = "0,000";
                        item.QuantidadeFaltante = notaFiscal.Volumes.ToString("n3");

                        Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFe = carga?.CargaMDFes?.FirstOrDefault();

                        item.CodigoMDFe = cargaMDFe?.MDFe?.Codigo ?? 0;
                        item.NumeroMDFe = cargaMDFe?.MDFe?.Numero.ToString("n0") ?? string.Empty;
                        item.CodigoCarga = carga?.Codigo ?? 0;
                        item.NumeroCarga = carga?.CodigoCargaEmbarcador ?? string.Empty;
                        item.CodigoVeiculo = carga?.Veiculo?.Codigo ?? 0;
                        item.PlacaVeiculo = carga?.Veiculo?.Placa ?? string.Empty;

                        item.PesoBruto = notaFiscal.Peso.ToString("n2");
                        item.PesoLiquido = notaFiscal.PesoLiquido.ToString("n2");
                        item.NumeroNF = notaFiscal.Numero.ToString("D");
                        item.SerieNF = notaFiscal.Serie;
                        item.ValorMercadoria = notaFiscal.ValorTotalProdutos.ToString("n2");
                        item.ValorNF = notaFiscal.Valor.ToString("n2");
                        item.VolumeNF = notaFiscal.Volumes.ToString("D");

                        retornoArquivos.Add(item);

                        SalvarItemRecebimento(recebimento, item, unitOfWork,
                            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria.Volume, null);
                    }
                }

                unitOfWork.CommitChanges();
                var dadosRetorno = new
                {
                    Mercadorias = retornoArquivos
                };
                return new JsonpResult(dadosRetorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os volumes da carga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarVolumesMDFe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();
                int codigoRecebimento = 0, codigoMDFe = 0, codigoProduto = 0;
                int.TryParse(Request.Params("CodigoRecebimento"), out codigoRecebimento);
                int.TryParse(Request.Params("CodigoProdutoEmbarcador"), out codigoProduto);
                int.TryParse(Request.Params("CodigoMDFe"), out codigoMDFe);

                System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");
                Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador =
                    new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);
                Servicos.Embarcador.WMS.LoteProdutoEmbarcador svcLoteProdutoEmbarcadora =
                    new Servicos.Embarcador.WMS.LoteProdutoEmbarcador(unitOfWork);
                Repositorio.Embarcador.WMS.Recebimento repRecebimento =
                    new Repositorio.Embarcador.WMS.Recebimento(unitOfWork);
                Repositorio.Embarcador.WMS.RecebimentoMercadoria repRecebimentoMercadoria =
                    new Repositorio.Embarcador.WMS.RecebimentoMercadoria(unitOfWork);
                Dominio.Entidades.Embarcador.WMS.Recebimento recebimento =
                    repRecebimento.BuscarPorCodigo(codigoRecebimento);
                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador =
                    repProdutoEmbarcador.BuscarPorCodigo(codigoProduto);

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe =
                    new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);
                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe =
                    new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal =
                    new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais =
                    repXMLNotaFiscal.BuscarPorMDFe(codigoMDFe);
                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe);
                Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFe = repCargaMDFe.BuscarPorMDFe(codigoMDFe);


                List<Item> retornoArquivos = new List<Item>();
                var codigoBarrasValido = true;
                for (int i = 0; i < notasFiscais.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal = notasFiscais[i];

                    codigoBarrasValido = true;

                    if (!string.IsNullOrWhiteSpace(notaFiscal.NumeroSolicitacao))
                    {
                        if (repRecebimentoMercadoria.NumeroSolicitacaoJaRecebido(notaFiscal.NumeroSolicitacao))
                        {
                            codigoBarrasValido =
                                repRecebimentoMercadoria.NumeroSolicitacaoPendenteDeConferencia(notaFiscal
                                    .NumeroSolicitacao);
                        }
                    }

                    if (codigoBarrasValido)
                    {
                        Item item = new Item();

                        Dominio.Entidades.Embarcador.WMS.DepositoPosicao posicao =
                            svcLoteProdutoEmbarcadora.BuscarPosicaoPadrao(produtoEmbarcador, notaFiscal.Volumes, 0, 0,
                                0, unitOfWork);

                        item.Codigo = 0;
                        item.CNPJCliente = 0;
                        item.Descricao = notaFiscal.NumeroSolicitacao;
                        item.NumeroLote = notaFiscal.Numero.ToString();
                        item.NCM = "";
                        item.ChaveNFe = notaFiscal.Chave;
                        item.Identificacao = notaFiscal.NumeroSolicitacao;
                        item.CodigoBarras = string.Empty;
                        item.DataVencimento = string.Empty;
                        item.QuantidadeLote = notaFiscal.Volumes.ToString("n3");
                        item.MetroCubico = "0,000";
                        item.Peso = "0,000";
                        item.QuantidadePalet = "0,000";
                        item.CodigoDepositoPosicao = posicao?.Codigo ?? 0;
                        item.CodigoRecebimento = recebimento?.Codigo ?? 0;
                        item.CodigoProdutoEmbarcador = codigoProduto;
                        item.DescricaoDepositoPosicao = posicao?.Abreviacao ?? "";
                        item.DescricaoRecebimento = recebimento?.Codigo.ToString() ?? "";
                        item.DescricaoProdutoEmbarcador = produtoEmbarcador?.Descricao ?? string.Empty;

                        item.CodigoEmissora = mdfe?.Empresa?.Codigo ?? 0;
                        item.NomeEmissora = mdfe?.Empresa?.RazaoSocial ?? string.Empty;

                        if (notaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores
                                .TipoOperacaoNotaFiscal.Entrada)
                        {
                            item.CNPJRemetente =
                                notaFiscal.Recebedor != null &&
                                TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores
                                    .TipoServicoMultisoftware.MultiEmbarcador && notaFiscal.Recebedor.CPF_CNPJ > 0
                                    ? notaFiscal.Recebedor.CPF_CNPJ
                                    : notaFiscal.Destinatario.CPF_CNPJ;
                            item.NomeRemetente =
                                notaFiscal.Recebedor != null &&
                                TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores
                                    .TipoServicoMultisoftware.MultiEmbarcador && notaFiscal.Recebedor.CPF_CNPJ > 0
                                    ? notaFiscal.Recebedor.Nome
                                    : notaFiscal.Destinatario.Nome;
                            item.CNPJDestinatario = notaFiscal.Emitente.CPF_CNPJ;
                            item.NomeDestinatario = notaFiscal.Emitente.Nome;
                        }
                        else
                        {
                            item.CNPJRemetente = notaFiscal.Emitente.CPF_CNPJ;
                            item.NomeRemetente = notaFiscal.Emitente.Nome;
                            item.CNPJDestinatario =
                                notaFiscal.Recebedor != null &&
                                TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores
                                    .TipoServicoMultisoftware.MultiEmbarcador && notaFiscal.Recebedor.CPF_CNPJ > 0
                                    ? notaFiscal.Recebedor.CPF_CNPJ
                                    : notaFiscal.Destinatario.CPF_CNPJ;
                            item.NomeDestinatario =
                                notaFiscal.Recebedor != null &&
                                TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores
                                    .TipoServicoMultisoftware.MultiEmbarcador && notaFiscal.Recebedor.CPF_CNPJ > 0
                                    ? notaFiscal.Recebedor.Nome
                                    : notaFiscal.Destinatario.Nome;
                        }

                        item.DataEmissaoNF = notaFiscal.DataEmissao.ToString("dd/MM/yyyy");
                        item.Numero = notaFiscal.Numero.ToString();
                        item.Serie = Utilidades.String.OnlyNumbers(notaFiscal.Serie);
                        item.QuantidadeConferida = "0,000";
                        item.QuantidadeFaltante = notaFiscal.Volumes.ToString("n3");

                        item.CodigoMDFe = 0;
                        item.NumeroMDFe = string.Empty;
                        item.CodigoCarga = cargaMDFe?.Carga.Codigo ?? 0;
                        item.NumeroCarga = cargaMDFe?.Carga.CodigoCargaEmbarcador ?? string.Empty;
                        item.CodigoVeiculo = cargaMDFe?.Carga.Veiculo?.Codigo ?? 0;
                        item.PlacaVeiculo = cargaMDFe?.Carga.Veiculo?.Placa ?? string.Empty;

                        item.PesoBruto = string.Empty;
                        item.PesoLiquido = string.Empty;
                        item.NumeroNF = string.Empty;
                        item.SerieNF = string.Empty;
                        item.ValorMercadoria = string.Empty;
                        item.ValorNF = string.Empty;
                        item.VolumeNF = string.Empty;

                        retornoArquivos.Add(item);

                        SalvarItemRecebimento(recebimento, item, unitOfWork,
                            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria.Volume, null);
                    }
                }

                unitOfWork.CommitChanges();
                var dadosRetorno = new
                {
                    Mercadorias = retornoArquivos
                };
                return new JsonpResult(dadosRetorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os volumes do MDFe.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarArmazenamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();
                int codigoProduto = 0;
                int.TryParse(Request.Params("CodigoProduto"), out codigoProduto);

                decimal quantidadePalet = 0, peso = 0, metroCubico = 0, quantidade;
                decimal.TryParse(Request.Params("Quantidade"), out quantidade);
                decimal.TryParse(Request.Params("QuantidadePalet"), out quantidadePalet);
                decimal.TryParse(Request.Params("Peso"), out peso);
                decimal.TryParse(Request.Params("MetroCubico"), out metroCubico);

                Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador =
                    new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);
                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produto =
                    repProdutoEmbarcador.BuscarPorCodigo(codigoProduto);

                Servicos.Embarcador.WMS.LoteProdutoEmbarcador svcLoteProdutoEmbarcadora =
                    new Servicos.Embarcador.WMS.LoteProdutoEmbarcador(unitOfWork);

                Dominio.Entidades.Embarcador.WMS.DepositoPosicao posicao =
                    svcLoteProdutoEmbarcadora.BuscarPosicaoPadrao(produto, quantidade, peso, metroCubico,
                        quantidadePalet, unitOfWork);

                unitOfWork.CommitChanges();
                var dadosRetorno = new
                {
                    Codigo = posicao != null ? posicao.Codigo : 0,
                    Descricao = posicao != null ? posicao.Abreviacao : string.Empty,
                };
                return new JsonpResult(dadosRetorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar o local de armazenamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AutorizarProdutosFaltantes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.WMS.Recebimento repRecebimento =
                    new Repositorio.Embarcador.WMS.Recebimento(unitOfWork);
                int.TryParse(Request.Params("Codigo"), out int codigo);

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas =
                    ObterPermissoesPersonalizadas("WMS/RecebimentoMercadoria");
                if (!(permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada
                        .WMS_Autorizar_Volumes_Faltantes) || this.Usuario.UsuarioAdministrador))
                {
                    return new JsonpResult(false,
                        "O usuário logado não possui permissão para autorizar os volumes faltantes.");
                }
                else if (codigo == 0)
                {
                    return new JsonpResult(false, "Favor selecione um recebimento.");
                }

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.WMS.Recebimento recebimento = repRecebimento.BuscarPorCodigo(codigo, true);
                recebimento.AutorizadoProdutosFaltantes = true;

                unitOfWork.CommitChanges();
                return new JsonpResult(true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal =
                    new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Pedidos.XMLNotaFiscalDimensao repXMLNotaFiscalDimensao =
                    new Repositorio.Embarcador.Pedidos.XMLNotaFiscalDimensao(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega rePedidoOcorrenciaColetaEntrega =
                    new Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega(unitOfWork);
                Repositorio.Embarcador.WMS.Recebimento repRecebimento =
                    new Repositorio.Embarcador.WMS.Recebimento(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido =
                    new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe =
                    new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);
                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe =
                    new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
                Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador =
                    new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);
                int.TryParse(Request.Params("Carga"), out int codigoCarga);
                int.TryParse(Request.Params("MDFe"), out int codigoMDFe);
                int.TryParse(Request.Params("Veiculo"), out int codigoVeiculo);
                int.TryParse(Request.Params("Usuario"), out int codigoUsuario);
                int.TryParse(Request.Params("ProdutoEmbarcador"), out int codigoProdutoEmbarcador);
                int.TryParse(Request.Params("Empresa"), out int codigoEmpresa);
                bool contemVolumesFaltantes = false;

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRecebimento situacao;
                Enum.TryParse(Request.Params("Situacao"), out situacao);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria tipoRecebimentoMercadoria;
                Enum.TryParse(Request.Params("TipoRecebimento"), out tipoRecebimentoMercadoria);

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.WMS.Recebimento recebimento;
                if (codigo > 0)
                    recebimento = repRecebimento.BuscarPorCodigo(codigo, true);
                else
                    recebimento = new Dominio.Entidades.Embarcador.WMS.Recebimento();

                if (codigoCarga > 0)
                    recebimento.Carga = repCarga.BuscarPorCodigo(codigoCarga);
                else
                    recebimento.Carga = null;

                if (codigoMDFe > 0)
                    recebimento.MDFe = repMDFe.BuscarPorCodigo(codigoMDFe);
                else
                    recebimento.MDFe = null;

                if (codigoEmpresa > 0)
                    recebimento.Empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
                else
                    recebimento.Empresa = null;

                recebimento.Observacao = Request.Params("Observacao");
                recebimento.SituacaoRecebimento = situacao;
                recebimento.TipoRecebimentoMercadoria = tipoRecebimentoMercadoria;
                recebimento.Usuario = repUsuario.BuscarPorCodigo(codigoUsuario);
                recebimento.Veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo);
                recebimento.ProdutoEmbarcador = repProdutoEmbarcador.BuscarPorCodigo(codigoProdutoEmbarcador);

                if (codigo == 0)
                {
                    recebimento.Data = DateTime.Now.Date;
                    repRecebimento.Inserir(recebimento, Auditado);
                }
                else
                    repRecebimento.Atualizar(recebimento, Auditado);

                if (tipoRecebimentoMercadoria ==
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria.Volume)
                    contemVolumesFaltantes = recebimento.Mercadorias != null && recebimento.Mercadorias.Count > 0
                        ? recebimento.Mercadorias.Where(o => o.QuantidadeFaltante > 0).Count() > 0
                        : false;

                if (recebimento.SituacaoRecebimento ==
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRecebimento.Finalizada)
                {
                    if (contemVolumesFaltantes && !recebimento.AutorizadoProdutosFaltantes &&
                        tipoRecebimentoMercadoria == Dominio.ObjetosDeValor.Embarcador.Enumeradores
                            .TipoRecebimentoMercadoria.Volume)
                    {
                        List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas =
                            ObterPermissoesPersonalizadas("WMS/RecebimentoMercadoria");
                        var dadosRetorno = new
                        {
                            ContemVolumesFaltantes = contemVolumesFaltantes,
                            LiberarAutorizacao =
                                permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores
                                    .PermissaoPersonalizada.WMS_Autorizar_Volumes_Faltantes) ||
                                this.Usuario.UsuarioAdministrador
                        };
                        unitOfWork.Rollback();
                        return new JsonpResult(dadosRetorno, false,
                            "Existem volumes faltantes para finalizar a conferencia.");
                    }

                    if (!FinalizarRecebimentoMercadoria(recebimento, unitOfWork, out string msgRetorno))
                    {
                        if (!string.IsNullOrWhiteSpace(msgRetorno))
                            throw new ControllerException(msgRetorno);

                        if (tipoRecebimentoMercadoria == Dominio.ObjetosDeValor.Embarcador.Enumeradores
                                .TipoRecebimentoMercadoria.Mercadoria)
                            throw new ControllerException(
                                "Favor informe todos os campos obrigatórios nas mercadorias.");

                        throw new ControllerException("Favor informe todos os campos obrigatórios nos volumes.");
                    }

                    if (ConfiguracaoEmbarcador.TipoDeOcorrenciaRecebimentoMercadoria != null)
                    {
                        List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = null;

                        if (recebimento.Carga != null || recebimento.MDFe != null)
                        {
                            if (recebimento.Carga != null)
                                pedidos = repCargaPedido.BuscarPedidosPorCarga(recebimento.Carga.Codigo);
                            else if (recebimento.MDFe != null)
                                pedidos = repCargaMDFe.BuscarPedidosPorMDFe(recebimento.MDFe.Codigo);
                        }

                        if (pedidos != null && pedidos.Count > 0)
                        {
                            Dominio.Entidades.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente
                                configuracaoPortalCliente =
                                    Servicos.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente.ObterConfiguracao(
                                        unitOfWork);

                            foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
                            {
                                string observacao = recebimento.Empresa != null
                                    ? "Filial: " + ((recebimento.Empresa?.Localidade?.Descricao ?? "") + " - " +
                                                    (recebimento.Empresa?.Localidade?.Estado?.Sigla ?? ""))
                                    : "";
                                Servicos.Embarcador.Carga.ControleEntrega.OcorrenciaEntrega
                                    .GerarPedidoOcorrenciaColetaEntrega(pedido.ObterTomador(), pedido, null,
                                        ConfiguracaoEmbarcador.TipoDeOcorrenciaRecebimentoMercadoria,
                                        configuracaoPortalCliente, observacao, ConfiguracaoEmbarcador, Cliente,
                                        unitOfWork);
                            }
                        }
                    }

                    foreach (var mercadoria in recebimento.Mercadorias)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = null;

                        if (!string.IsNullOrWhiteSpace(mercadoria.ChaveNFe))
                            xmlNotaFiscal = repXMLNotaFiscal.BuscarPorChave(mercadoria.ChaveNFe);

                        if (xmlNotaFiscal == null && mercadoria.NumeroNF > 0 && mercadoria.EmpresaEmissora != null)
                        {
                            double.TryParse(mercadoria.EmpresaEmissora.CNPJ, out double cnpjEmissor);
                            xmlNotaFiscal = repXMLNotaFiscal.BuscarPorNumeroEEmitente(mercadoria.NumeroNF, cnpjEmissor);
                        }

                        if (xmlNotaFiscal != null)
                        {
                            string msg = "";
                            bool valid = ValidarDadosMercadoria(mercadoria, out msg);

                            if (!valid)
                                return new JsonpResult(false, msg);

                            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalDimensao xmlNotaFiscalDimensao =
                                new Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalDimensao()
                                {
                                    Altura = mercadoria.Altura,
                                    Largura = mercadoria.Largura,
                                    Comprimento = mercadoria.Comprimento,
                                    MetrosCubicos = mercadoria.MetroCubico,
                                    Volumes = (int)mercadoria.QuantidadeLote,
                                    XMLNotaFiscal = xmlNotaFiscal
                                };

                            repXMLNotaFiscalDimensao.Inserir(xmlNotaFiscalDimensao);
                        }
                    }
                }

                ControlarFluxoPatio(recebimento, unitOfWork);

                unitOfWork.CommitChanges();

                var retorno = new
                {
                    Codigo = recebimento.Codigo
                };

                return new JsonpResult(retorno, true, "Sucesso");
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
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.WMS.Recebimento repRecebimento =
                    new Repositorio.Embarcador.WMS.Recebimento(unitOfWork);

                Dominio.Entidades.Embarcador.WMS.Recebimento recebimento = repRecebimento.BuscarPorCodigo(codigo);
                var dynPedido = new
                {
                    recebimento.Codigo,
                    recebimento.Observacao,
                    Situacao = recebimento.SituacaoRecebimento,
                    TipoRecebimento = recebimento.TipoRecebimentoMercadoria,
                    MDFe = recebimento.MDFe != null
                        ? new { Codigo = recebimento.MDFe.Codigo, Descricao = recebimento.MDFe.Numero.ToString() }
                        : null,
                    Carga = recebimento.Carga != null
                        ? new { Codigo = recebimento.Carga.Codigo, Descricao = recebimento.Carga.CodigoCargaEmbarcador }
                        : null,
                    Usuario = recebimento.Usuario != null
                        ? new { Codigo = recebimento.Usuario.Codigo, Descricao = recebimento.Usuario.Nome }
                        : null,
                    Veiculo = recebimento.Veiculo != null
                        ? new { Codigo = recebimento.Veiculo.Codigo, Descricao = recebimento.Veiculo.Placa }
                        : null,
                    ProdutoEmbarcador = recebimento.ProdutoEmbarcador != null
                        ? new
                        {
                            Codigo = recebimento.ProdutoEmbarcador.Codigo,
                            Descricao = recebimento.ProdutoEmbarcador.Descricao
                        }
                        : null,
                    Empresa = recebimento.Empresa != null
                        ? new { Codigo = recebimento.Empresa.Codigo, Descricao = recebimento.Empresa.Descricao }
                        : null
                };

                return new JsonpResult(dynPedido);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ImprimirEtiqueta()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio =
                    new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio =
                    repRelatorio.BuscarPadraoPorCodigoControleRelatorio(
                        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R103_EtiquetaVolume,
                        TipoServicoMultisoftware);
                Repositorio.Embarcador.PedidoVenda.PedidoVenda repPedidoVenda =
                    new Repositorio.Embarcador.PedidoVenda.PedidoVenda(unitOfWork);

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio =
                    new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                if (relatorio == null)
                    relatorio = serRelatorio.BuscarConfiguracaoPadrao(
                        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R103_EtiquetaVolume,
                        TipoServicoMultisoftware, "Etiqueta de Volume", "RecebimentoMercadoria", "EtiquetaVolume.rpt",
                        Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "",
                        "", 0, unitOfWork, false, false);

                int codigo = Request.GetIntParam("Codigo");
                int codigoRecebimento = Request.GetIntParam("CodigoRecebimento");

                var etiqueta = Request.Params("Etiqueta");

                string msg = "", nomeEtiqueta = "";
                bool valid = false;
                List<Dominio.Relatorios.Embarcador.DataSource.WMS.IEtiquetaCarga> dadosEtiquetaVolume =
                    new List<Dominio.Relatorios.Embarcador.DataSource.WMS.IEtiquetaCarga>();
                switch (etiqueta)
                {
                    case nameof(Dominio.Relatorios.Embarcador.Enumeradores.Etiqueta.UNICA):
                        valid = ValidarDadosEtiqueta(unitOfWork, out msg);
                        if (valid)
                        {
                            dadosEtiquetaVolume = RetornarDadosEtiqueta(unitOfWork);
                            nomeEtiqueta = ConfiguracaoEmbarcador.UtilizarEtiquetaDetalhadaWMS
                                ? TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores
                                    .TipoServicoMultisoftware.MultiEmbarcador
                                    ? "EtiquetaVolumeReduzida.rpt"
                                    : "EtiquetaVolume.rpt"
                                : "EtiquetaVolumeAntiga.rpt";
                        }

                        break;

                    case nameof(Dominio.Relatorios.Embarcador.Enumeradores.Etiqueta.MULTIPLAS):
                        valid = ValidarDadosEtiquetaMultiplas(unitOfWork, codigoRecebimento, out msg);
                        if (valid)
                        {
                            dadosEtiquetaVolume = RetornarDadosEtiquetaMultiplas(unitOfWork, codigoRecebimento);
                            nomeEtiqueta = ConfiguracaoEmbarcador.UtilizarEtiquetaDetalhadaWMS
                                ? TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores
                                    .TipoServicoMultisoftware.MultiEmbarcador
                                    ? "EtiquetaVolumeReduzida.rpt"
                                    : "EtiquetaVolume.rpt"
                                : "EtiquetaVolumeAntiga.rpt";
                        }

                        break;

                    case nameof(Dominio.Relatorios.Embarcador.Enumeradores.Etiqueta.LRMASTER):
                        valid = ValidarDadosEtiqueta(unitOfWork, out msg);
                        if (valid)
                        {
                            dadosEtiquetaVolume = RetornarDadosEtiqueta(unitOfWork);
                            nomeEtiqueta =
                                TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores
                                    .TipoServicoMultisoftware.MultiEmbarcador
                                    ? "EtiquetaVolumeLRMasterReduzida.rpt"
                                    : "EtiquetaVolumeLRMaster.rpt";
                        }

                        break;

                    case nameof(Dominio.Relatorios.Embarcador.Enumeradores.Etiqueta.ETIQUETAPALLET):
                        valid = ValidarDadosEtiquetaMultiplas(unitOfWork, codigoRecebimento, out msg);
                        if (valid)
                        {
                            dadosEtiquetaVolume = RetornarDadosEtiquetaMultiplas(unitOfWork, codigoRecebimento);
                            nomeEtiqueta =
                                TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores
                                    .TipoServicoMultisoftware.MultiEmbarcador
                                    ? "EtiquetaVolumePalletReduzida.rpt"
                                    : "EtiquetaVolumePallet.rpt";
                        }

                        break;
                }

                if (!valid)
                    return new JsonpResult(false, false,
                        "Existe dado(s) faltante(s) para a geração da etiqueta: " + msg);

                return Etiquetas(dadosEtiquetaVolume, serRelatorio, relatorio, unitOfWork, codigo, nomeEtiqueta);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o relatório.");
            }
        }

        public async Task<IActionResult> ConferirVolume()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoRecebimento = 0;
                int.TryParse(Request.Params("CodigoRecebimento"), out codigoRecebimento);
                string numeroNS = Request.Params("NumeroNS");
                string volume = Request.Params("Volume");
                string cnpjRemetente = Request.Params("CNPJRemetente");
                string numeroNota = Request.Params("NumeroNota");
                string serieNota = Request.Params("SerieNota");
                string codigoBarrasLocalizar = Request.Params("CodigoBarrasLocalizar");
                bool localizou = false;

                Repositorio.Embarcador.WMS.RecebimentoMercadoria repRecebimentoMercadoria =
                    new Repositorio.Embarcador.WMS.RecebimentoMercadoria(unidadeDeTrabalho);

                List<Dominio.Entidades.Embarcador.WMS.RecebimentoMercadoria> listaMercadoria =
                    repRecebimentoMercadoria.BuscarPorRecebimento(codigoRecebimento);

                foreach (var mercadoria in listaMercadoria)
                {
                    if (numeroNS != "")
                    {
                        if (numeroNS == mercadoria.Descricao && localizou == false)
                        {
                            localizou = true;

                            if (mercadoria.QuantidadeFaltante == 0m)
                                return new JsonpResult(false, "Volume já se encontra com suas unidades conferidas.");

                            if (!string.IsNullOrWhiteSpace(mercadoria.CodigoBarras) &&
                                mercadoria.CodigoBarras.Contains(codigoBarrasLocalizar))
                                return new JsonpResult(false, "O código de barras já foi conferido.");

                            unidadeDeTrabalho.Start();

                            mercadoria.QuantidadeConferida++;
                            mercadoria.QuantidadeFaltante--;

                            if (string.IsNullOrWhiteSpace(mercadoria.CodigoBarras))
                                mercadoria.CodigoBarras = codigoBarrasLocalizar;
                            else
                                mercadoria.CodigoBarras = mercadoria.CodigoBarras + "|" + codigoBarrasLocalizar;

                            repRecebimentoMercadoria.Atualizar(mercadoria);

                            unidadeDeTrabalho.CommitChanges();

                            break;
                        }
                    }
                    else if (cnpjRemetente != "")
                    {
                        if (cnpjRemetente.PadLeft(14, '0') ==
                            mercadoria.Remetente.CPF_CNPJ_SemFormato.PadLeft(14, '0') &&
                            numeroNota == mercadoria.NumeroLote &&
                            (serieNota == mercadoria.Serie ||
                             serieNota.TrimStart('0') == mercadoria.Serie.TrimStart('0')) && localizou == false)
                        {
                            localizou = true;

                            if (mercadoria.QuantidadeFaltante == 0m)
                                return new JsonpResult(false, "Volume já se encontra com suas unidades conferidas.");

                            if (!string.IsNullOrWhiteSpace(mercadoria.CodigoBarras) &&
                                mercadoria.CodigoBarras.Contains(codigoBarrasLocalizar))
                                return new JsonpResult(false, "O código de barras já foi conferido.");

                            unidadeDeTrabalho.Start();

                            mercadoria.QuantidadeConferida++;
                            mercadoria.QuantidadeFaltante--;

                            if (string.IsNullOrWhiteSpace(mercadoria.CodigoBarras))
                                mercadoria.CodigoBarras = codigoBarrasLocalizar;
                            else
                                mercadoria.CodigoBarras = mercadoria.CodigoBarras + "|" + codigoBarrasLocalizar;

                            repRecebimentoMercadoria.Atualizar(mercadoria);

                            unidadeDeTrabalho.CommitChanges();

                            break;
                        }
                    }
                }

                if (!localizou)
                    return new JsonpResult(false, "Código de barras não localizado.");
                else
                    return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao conferir.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirMercadoriaVolume()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = int.Parse(Utilidades.String.OnlyNumbers(Request.Params("Codigo")));

                Repositorio.Embarcador.WMS.RecebimentoMercadoria repRecebimentoMercadoria =
                    new Repositorio.Embarcador.WMS.RecebimentoMercadoria(unidadeDeTrabalho);

                unidadeDeTrabalho.Start();

                Dominio.Entidades.Embarcador.WMS.RecebimentoMercadoria mercadoria =
                    repRecebimentoMercadoria.BuscarPorCodigo(codigo);

                repRecebimentoMercadoria.Deletar(mercadoria, Auditado);

                unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true,
                        "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> SalvarMercadoriaVolume()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador =
                    new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unidadeDeTrabalho);
                Repositorio.Embarcador.Produtos.ProdutoEmbarcadorCliente repProdutoEmbarcadorCliente =
                    new Repositorio.Embarcador.Produtos.ProdutoEmbarcadorCliente(unidadeDeTrabalho);
                Repositorio.Embarcador.WMS.RecebimentoMercadoria repRecebimentoMercadoria =
                    new Repositorio.Embarcador.WMS.RecebimentoMercadoria(unidadeDeTrabalho);
                Repositorio.Embarcador.WMS.DepositoPosicao repDepositoPosicao =
                    new Repositorio.Embarcador.WMS.DepositoPosicao(unidadeDeTrabalho);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Repositorio.Produto repProduto = new Repositorio.Produto(unidadeDeTrabalho);
                Repositorio.Embarcador.WMS.Recebimento repRecebimento =
                    new Repositorio.Embarcador.WMS.Recebimento(unidadeDeTrabalho);

                unidadeDeTrabalho.Start();

                string codigo = Request.Params("Codigo");
                int codigoInterno = 0, codigoRecebimento = 0;
                int.TryParse(Request.Params("Codigo"), out codigoInterno);
                int.TryParse(Request.Params("CodigoRecebimento"), out codigoRecebimento);

                Dominio.Entidades.Embarcador.WMS.Recebimento recebimento =
                    repRecebimento.BuscarPorCodigo(codigoRecebimento);

                if (recebimento == null)
                {
                    recebimento = new Dominio.Entidades.Embarcador.WMS.Recebimento()
                    {
                        AutorizadoProdutosFaltantes = false,
                        Carga = null,
                        Data = DateTime.Now,
                        MDFe = null,
                        Observacao = "",
                        ProdutoEmbarcador = null,
                        SituacaoRecebimento =
                            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRecebimento.Iniciada,
                        TipoRecebimentoMercadoria = Dominio.ObjetosDeValor.Embarcador.Enumeradores
                            .TipoRecebimentoMercadoria.Volume,
                        Usuario = this.Usuario,
                        Veiculo = null
                    };
                    repRecebimento.Inserir(recebimento);
                }

                int codigoEmissora = 0, codigoDepositoPosicao = 0, codigoProdutoEmbarcador = 0;
                int.TryParse(Request.Params("CodigoEmissora"), out codigoEmissora);
                int.TryParse(Request.Params("CodigoDepositoPosicao"), out codigoDepositoPosicao);
                int.TryParse(Request.Params("CodigoProdutoEmbarcador"), out codigoProdutoEmbarcador);
                if (codigoProdutoEmbarcador == 0 && recebimento.ProdutoEmbarcador != null)
                    codigoProdutoEmbarcador = recebimento.ProdutoEmbarcador.Codigo;
                int codigoProduto = Request.GetIntParam("CodigoProduto");

                double cnpjRemetente = 0, cnpjDestinatario = 0, cnpjCliente;
                double.TryParse(Request.Params("CNPJRemetente"), out cnpjRemetente);
                double.TryParse(Request.Params("CNPJCliente"), out cnpjCliente);
                double.TryParse(Request.Params("CNPJDestinatario"), out cnpjDestinatario);

                DateTime dataEmissaoNF, dataVencimento;
                DateTime.TryParse(Request.Params("DataEmissaoNF"), out dataEmissaoNF);
                DateTime.TryParse(Request.Params("DataVencimento"), out dataVencimento);

                string codigoBarras = Request.Params("CodigoBarras");
                string descricao = Request.Params("Descricao");
                string numeroLote = Request.Params("Numero");
                string numeroLote2 = Request.Params("NumeroLote");
                string ncm = Request.Params("NCM");
                string serie = Request.Params("Serie");
                string chaveNFe = Request.Params("ChaveNFe");
                string identificacao = Request.Params("Identificacao");

                decimal quantidadeLote = 0,
                    quantidadeConferida = 0,
                    quantidadeFaltante = 0,
                    MetroCubico = 0,
                    peso = 0,
                    quantidadePalet = 0,
                    altura = 0,
                    largura = 0,
                    comprimento = 0;
                decimal.TryParse(Request.Params("QuantidadeLote"), out quantidadeLote);
                decimal.TryParse(Request.Params("QuantidadeConferida"), out quantidadeConferida);
                decimal.TryParse(Request.Params("QuantidadeFaltante"), out quantidadeFaltante);
                decimal.TryParse(Request.Params("Altura"), out altura);
                decimal.TryParse(Request.Params("Largura"), out largura);
                decimal.TryParse(Request.Params("Comprimento"), out comprimento);

                MetroCubico = (altura * largura * comprimento) * (quantidadeLote > 0 ? quantidadeLote : 1);

                decimal.TryParse(Request.Params("Peso"), out peso);
                decimal.TryParse(Request.Params("QuantidadePalet"), out quantidadePalet);

                Dominio.Entidades.Embarcador.WMS.RecebimentoMercadoria recebimentoMercadoria;
                if (codigoInterno > 0)
                    recebimentoMercadoria = repRecebimentoMercadoria.BuscarPorCodigo(codigoInterno, true);
                else
                    recebimentoMercadoria = new Dominio.Entidades.Embarcador.WMS.RecebimentoMercadoria();

                //recebimentoMercadoria.CodigoBarras = codigoBarras;
                if (dataEmissaoNF > DateTime.MinValue)
                    recebimentoMercadoria.DataEmissaoNF = dataEmissaoNF;
                else
                    recebimentoMercadoria.DataEmissaoNF = null;
                if (dataVencimento > DateTime.MinValue)
                    recebimentoMercadoria.DataVencimento = dataVencimento;
                else
                    recebimentoMercadoria.DataVencimento = null;
                recebimentoMercadoria.ValorUnitario = Request.GetDecimalParam("ValorUnitario");
                recebimentoMercadoria.DepositoPosicao = repDepositoPosicao.BuscarPorCodigo(codigoDepositoPosicao);
                recebimentoMercadoria.Descricao = descricao;
                recebimentoMercadoria.NumeroLote = numeroLote;
                recebimentoMercadoria.NCM = ncm;
                recebimentoMercadoria.ChaveNFe = chaveNFe;

                if (!string.IsNullOrWhiteSpace(identificacao))
                    recebimentoMercadoria.Identificacao = identificacao;

                if (codigoProduto > 0)
                    recebimentoMercadoria.Produto = repProduto.BuscarPorCodigo(codigoProduto);
                else
                    recebimentoMercadoria.Produto = null;


                recebimentoMercadoria.Altura = altura;
                recebimentoMercadoria.Largura = largura;
                recebimentoMercadoria.Comprimento = comprimento;
                recebimentoMercadoria.MetroCubico = MetroCubico;

                recebimentoMercadoria.Peso = peso;
                recebimentoMercadoria.QuantidadePalet = quantidadePalet;

                if (!string.IsNullOrEmpty(numeroLote2))
                    recebimentoMercadoria.NumeroLote = numeroLote2;

                recebimentoMercadoria.ProdutoEmbarcador = repProdutoEmbarcador.BuscarPorCodigo(codigoProdutoEmbarcador);
                recebimentoMercadoria.QuantidadeLote = quantidadeLote;
                recebimentoMercadoria.Recebimento = recebimento;
                recebimentoMercadoria.TipoRecebimentoMercadoria = recebimento.TipoRecebimentoMercadoria;
                recebimentoMercadoria.Serie = serie;
                recebimentoMercadoria.QuantidadeConferida = quantidadeConferida;
                recebimentoMercadoria.QuantidadeFaltante = quantidadeFaltante;

                recebimentoMercadoria.PesoBruto = Request.GetDecimalParam("PesoBruto");
                recebimentoMercadoria.PesoLiquido = Request.GetDecimalParam("PesoLiquido");
                recebimentoMercadoria.ValorNF = Request.GetDecimalParam("ValorNF");
                recebimentoMercadoria.ValorMercadoria = Request.GetDecimalParam("ValorMercadoria");
                recebimentoMercadoria.TipoUnidade = Request.GetStringParam("TipoUnidade");

                if (cnpjRemetente > 0)
                    recebimentoMercadoria.Remetente = repCliente.BuscarPorCPFCNPJ(cnpjRemetente);

                if (cnpjDestinatario > 0)
                    recebimentoMercadoria.Destinatario = repCliente.BuscarPorCPFCNPJ(cnpjDestinatario);

                if (cnpjCliente > 0)
                    recebimentoMercadoria.CNPJCliente = cnpjCliente;

                if (codigoEmissora > 0)
                    recebimentoMercadoria.EmpresaEmissora = repEmpresa.BuscarPorCodigo(codigoEmissora);

                if (codigoInterno > 0)
                    repRecebimentoMercadoria.Atualizar(recebimentoMercadoria);
                else
                    repRecebimentoMercadoria.Inserir(recebimentoMercadoria, Auditado);

                unidadeDeTrabalho.CommitChanges();

                var retorno = new { Codigo = recebimentoMercadoria.Codigo, CodigoRecebimento = recebimento.Codigo };


                return new JsonpResult(retorno, true, "Sucesso");
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> VolumesFaltantes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio =
                    new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio =
                    repRelatorio.BuscarPadraoPorCodigoControleRelatorio(
                        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R103_EtiquetaVolume,
                        TipoServicoMultisoftware);
                Repositorio.Embarcador.PedidoVenda.PedidoVenda repPedidoVenda =
                    new Repositorio.Embarcador.PedidoVenda.PedidoVenda(unitOfWork);

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio =
                    new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                if (relatorio == null)
                    relatorio = serRelatorio.BuscarConfiguracaoPadrao(
                        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R175_VolumesFaltantes,
                        TipoServicoMultisoftware, "Volumes Faltantes", "RecebimentoMercadoria", "VolumeFaltante.rpt",
                        Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "",
                        "", 0, unitOfWork, false, false);

                int codigoExpedicao = 0, codigoRecebimento = 0;
                int.TryParse(Request.Params("CodigoExpedicao"), out codigoExpedicao);
                int.TryParse(Request.Params("CodigoRecebimento"), out codigoRecebimento);
                bool recebimentoVolume = false;
                recebimentoVolume = bool.Parse(Request.Params("RecebimentoVolume"));

                List<Dominio.Relatorios.Embarcador.DataSource.WMS.VolumeFaltante> dadosVolumeFaltante = null;
                if (recebimentoVolume && codigoRecebimento > 0)
                    dadosVolumeFaltante = RetornarDadosVolumesFaltantes(unitOfWork, codigoRecebimento);
                else if (codigoExpedicao > 0)
                    dadosVolumeFaltante = RetornarDadosVolumesFaltantesExpedicao(unitOfWork, codigoExpedicao);

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao =
                    serRelatorio.AdicionarRelatorioParaGeracao(relatorio, this.Usuario,
                        Dominio.Enumeradores.TipoArquivoRelatorio.PDF, unitOfWork);

                string stringConexao = _conexao.StringConexao;
                string nomeCliente = Cliente.NomeFantasia;

                if (dadosVolumeFaltante != null && dadosVolumeFaltante.Count > 0)
                {
                    Task.Factory.StartNew(() => GerarVolumeFaltante(recebimentoVolume, nomeCliente, stringConexao,
                        relatorioControleGeracao, dadosVolumeFaltante));
                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, false, "Nenhum registro de volume faltante.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o relatório.");
            }
        }

        #endregion

        #region Métodos Privados

        private bool ValidarDadosMercadoria(Dominio.Entidades.Embarcador.WMS.RecebimentoMercadoria mercadoria,
            out string msg)
        {
            msg = "";
            if (mercadoria.QuantidadeLote == 0)
            {
                msg = "Quantidade lote não definida.";
                return false;
            }

            return true;
        }

        private JsonpResult Etiquetas(
            List<Dominio.Relatorios.Embarcador.DataSource.WMS.IEtiquetaCarga> dadosEtiquetaVolume,
            Servicos.Embarcador.Relatorios.Relatorio serRelatorio,
            Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio, Repositorio.UnitOfWork unitOfWork, int codigo,
            string nomeEtiqueta)
        {
            Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao =
                serRelatorio.AdicionarRelatorioParaGeracao(relatorio, this.Usuario,
                    Dominio.Enumeradores.TipoArquivoRelatorio.PDF, unitOfWork);

            string stringConexao = _conexao.StringConexao;
            string nomeCliente = Cliente.NomeFantasia;


            if (!dadosEtiquetaVolume.Any())
                return new JsonpResult(false, false, "Nenhum registro de etiqueta selecionado.");

            Task.Factory.StartNew(() => GerarEtiquetasVolume(codigo, nomeCliente, stringConexao,
                relatorioControleGeracao, dadosEtiquetaVolume, nomeEtiqueta));
            return new JsonpResult(true);
        }

        private void GerarEtiquetasVolume(int codigo, string nomeEmpresa, string stringConexao,
            Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao,
            List<Dominio.Relatorios.Embarcador.DataSource.WMS.IEtiquetaCarga> dadosEtiquetaVolume, string nomeEtiqueta)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio =
                new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
            Servicos.Embarcador.WMS.LoteProdutoEmbarcador serLoteProdutoEmbarcador =
                new Servicos.Embarcador.WMS.LoteProdutoEmbarcador(unitOfWork);
            try
            {
                ReportRequest.WithType(ReportType.GerarEtiquetasVolume)
                    .WithExecutionType(ExecutionType.Sync)
                    .AddExtraData("codigo", codigo)
                    .AddExtraData("nomeEmpresa", nomeEmpresa)
                    .AddExtraData("dadosEtiquetaVolume", dadosEtiquetaVolume.ToJson())
                    .AddExtraData("nomeEtiqueta", nomeEtiqueta)
                    .AddExtraData("relatorioControleGeracao", relatorioControleGeracao.Codigo)
                    .CallReport()
                    .GetContentFile();
            }
            catch (Exception ex)
            {
                serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, ex);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private bool ValidarDadosEtiqueta(Repositorio.UnitOfWork unitOfWork, out string Msg)
        {
            Msg = "";
            Repositorio.Embarcador.WMS.RecebimentoMercadoria repRecebimentoMercadoria =
                new Repositorio.Embarcador.WMS.RecebimentoMercadoria(unitOfWork);
            decimal quantidadeVolumes = 0;
            quantidadeVolumes = decimal.Parse(Request.Params("QuantidadeLote"));
            double cnpjDestinatario = 0, cnpjJRemetente = 0;
            cnpjJRemetente = double.Parse(Request.Params("CNPJRemetente"));
            cnpjDestinatario = double.Parse(Request.Params("CNPJDestinatario"));

            int codigoEmissora = 0;
            codigoEmissora = int.Parse(Request.Params("CodigoEmissora"));
            string numero = Request.Params("Numero");
            string serie = Request.Params("Serie");

            if (quantidadeVolumes == 0)
            {
                Msg = "Documento sem volume.";
                return false;
            }

            if (cnpjJRemetente == 0)
            {
                Msg = "Sem Remetente selecionado.";
                return false;
            }

            if (cnpjDestinatario == 0)
            {
                Msg = "Sem Destinatário selecionado.";
                return false;
            }

            if (codigoEmissora == 0 && TipoServicoMultisoftware ==
                AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                Msg = "Sem Empresa Emissora selecionada.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(numero))
            {
                Msg = "Número da nota não infomada.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(serie))
            {
                Msg = "Série da nota não informada.";
                return false;
            }

            return true;
        }

        private bool ValidarDadosEtiquetaMultiplas(Repositorio.UnitOfWork unitOfWork, int codigoRecebimento,
            out string Msg)
        {
            Msg = "";
            Repositorio.Embarcador.WMS.RecebimentoMercadoria repRecebimentoMercadoria =
                new Repositorio.Embarcador.WMS.RecebimentoMercadoria(unitOfWork);
            List<Dominio.Entidades.Embarcador.WMS.RecebimentoMercadoria> listaVolumes =
                repRecebimentoMercadoria.BuscarPorRecebimento(codigoRecebimento);

            foreach (var volume in listaVolumes)
            {
                int? codigoEmissora = volume.EmpresaEmissora?.Codigo;
                double? cnpjRemetente = volume.Remetente?.CPF_CNPJ;
                double? cnpjDestinatario = volume.Destinatario?.CPF_CNPJ;
                string numeroLote = volume.NumeroLote;
                string serie = volume.Serie;
                decimal quantidadeLote = volume.QuantidadeLote;

                if (quantidadeLote == 0)
                {
                    Msg = "Documento sem volume";
                    return false;
                }

                if (cnpjRemetente == 0)
                {
                    Msg = "Sem Remetente selecionado.";
                    return false;
                }

                if (cnpjDestinatario == 0)
                {
                    Msg = "Sem Destinatário selecionado.";
                    return false;
                }

                if (codigoEmissora == 0 && TipoServicoMultisoftware ==
                    AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    Msg = "Sem Empresa Emissora selecionada.";
                    return false;
                }

                if (string.IsNullOrWhiteSpace(numeroLote))
                {
                    Msg = "Número da nota não infomada.";
                    return false;
                }

                if (string.IsNullOrWhiteSpace(serie))
                {
                    Msg = "Série da nota não informada.";
                    return false;
                }
            }

            return true;
        }

        private List<Dominio.Relatorios.Embarcador.DataSource.WMS.IEtiquetaCarga> RetornarDadosEtiqueta(
            Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador =
                new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);
            Repositorio.Embarcador.WMS.MontagemContainerNotaFiscal repMontagemContainer =
                new Repositorio.Embarcador.WMS.MontagemContainerNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal =
                new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            List<Dominio.Relatorios.Embarcador.DataSource.WMS.IEtiquetaCarga> listaRetorno =
                new List<Dominio.Relatorios.Embarcador.DataSource.WMS.IEtiquetaCarga>();
            BarcodeMetrics1d metricas = new BarcodeMetrics1d();
            metricas.Scale = 5;

            decimal quantidadeVolumes = Request.GetDecimalParam("QuantidadeLote");
            double cnpjJRemetente = Request.GetDoubleParam("CNPJRemetente");
            double cnpjDestinatario = Request.GetDoubleParam("CNPJDestinatario");
            int codigoEmissora = Request.GetIntParam("CodigoEmissora");

            string chaveNFe = Request.GetStringParam("ChaveNFe");
            string numeroPedido = Request.GetStringParam("Descricao");
            string numero = Request.GetStringParam("Numero");
            string serie = Request.GetStringParam("Serie");
            int codigoCarga = Request.GetIntParam("CodigoCarga");
            int produtoEmbarcadorCodigo = Request.GetIntParam("Item");
            DateTime dataEmissaoNF = Request.GetDateTimeParam("DataEmissaoNF");
            if (dataEmissaoNF == DateTime.MinValue)
                dataEmissaoNF = DateTime.Now;

            decimal pesoCarga = Request.GetDecimalParam("Peso");
            decimal metroCubico = Request.GetDecimalParam("MetroCubico");

            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
            Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador =
                repProdutoEmbarcador.BuscarPorCodigo(produtoEmbarcadorCodigo);
            Dominio.Entidades.Cliente destinatario = repCliente.BuscarPorCPFCNPJ(cnpjDestinatario);
            Dominio.Entidades.Cliente remetente = repCliente.BuscarPorCPFCNPJ(cnpjJRemetente);
            Dominio.Entidades.Empresa empresaEmissora = repEmpresa.BuscarPorCodigo(codigoEmissora);
            Repositorio.Embarcador.Logistica.RotaFreteCEP repRotaCEP =
                new Repositorio.Embarcador.Logistica.RotaFreteCEP(unitOfWork);

            if (empresaEmissora == null)
                empresaEmissora = repEmpresa.BuscarEmpresaPadraoRetirada();

            string cepDestinatario = Utilidades.String.OnlyNumbers(destinatario.CEP);
            if (string.IsNullOrWhiteSpace(cepDestinatario))
                cepDestinatario = "0";

            string cep = Utilidades.String.OnlyNumbers(destinatario.CEP);
            if (string.IsNullOrWhiteSpace(cep))
                cep = "0";
            Dominio.Entidades.Embarcador.Logistica.RotaFreteCEP rotaCEP = repRotaCEP.BuscarPorCEP(int.Parse(cep));

            for (int i = 0; i < quantidadeVolumes; i++)
            {
                Dominio.Relatorios.Embarcador.DataSource.WMS.EtiquetaVolumeLRMaster etiqueta =
                    new Dominio.Relatorios.Embarcador.DataSource.WMS.EtiquetaVolumeLRMaster();

                #region cabeçalho

                etiqueta.NomeTransportadora = empresaEmissora?.NomeFantasia ?? "";
                etiqueta.EnderecoTransportadora = empresaEmissora?.Endereco ?? "";
                etiqueta.Bairro = empresaEmissora?.Bairro ?? "";
                etiqueta.CidadeTransportadora = empresaEmissora?.Localidade?.Descricao ?? "";
                etiqueta.UFTransportadora = empresaEmissora?.Localidade?.Estado?.Sigla ?? "";
                etiqueta.CEPTransportadora = empresaEmissora?.CEP ?? "";
                etiqueta.CNPJTransportadora = empresaEmissora?.CNPJ_Formatado ?? "";
                etiqueta.InscricaoEstadual = empresaEmissora?.InscricaoEstadual ?? "";
                etiqueta.Telefone = empresaEmissora?.Telefone ?? "";
                etiqueta.RNTC = "";
                etiqueta.UFLocalidade = "";
                etiqueta.Localidade = "";

                #endregion

                #region Remetente

                etiqueta.Remetente = remetente.Nome;
                etiqueta.EnderecoRemetente = remetente?.Endereco ?? "";
                etiqueta.CidadeRemetente = remetente.Localidade?.Descricao ?? "";
                etiqueta.UFRemetente = remetente.Localidade?.Estado?.Sigla ?? "";
                etiqueta.CPFCNPJRemetente = remetente?.CPF_CNPJ_Formatado ?? "";
                etiqueta.PaisRemetente = remetente.Localidade?.Pais?.Nome ?? "";
                etiqueta.CEPRemetente = remetente?.CEP ?? "";
                etiqueta.InscricaoRemetente = remetente.InscricaoMunicipal ?? "";
                etiqueta.TelefoneRemetente = remetente?.Telefone1 ?? "";

                #endregion

                #region Destinatário

                etiqueta.Destinatario = destinatario.Nome;
                etiqueta.EnderecoDestinatario =
                    destinatario.Endereco + ", " + destinatario.Bairro + ", " + destinatario.Numero;
                etiqueta.CidadeDestinatario = destinatario.Localidade?.Descricao ?? "";
                etiqueta.UFDestinatario = destinatario.Localidade.Estado.Sigla ?? "";
                etiqueta.CPFCNPJDestinatario = cepDestinatario;
                etiqueta.PaisDestinatario = destinatario.Localidade?.Pais?.Nome ?? "";
                etiqueta.CEPDestinatario = destinatario?.CEP ?? "";
                etiqueta.InscricaoDestinatario = destinatario?.InscricaoMunicipal ?? "";
                etiqueta.TelefoneDestinatario = destinatario?.Telefone1 ?? "";

                #endregion

                #region Carga

                etiqueta.Carga = carga?.CodigoCargaEmbarcador ?? "";
                etiqueta.NumeroPedido = numeroPedido;
                etiqueta.VolumeTotal = quantidadeVolumes;
                etiqueta.Volume = i + 1;
                etiqueta.NumeroNota = Utilidades.String.OnlyNumbers(numero);
                etiqueta.FilialDistribuidora = rotaCEP != null ? rotaCEP.RotaFrete.FilialDistribuidora : string.Empty;
                etiqueta.Rota = rotaCEP != null ? rotaCEP.RotaFrete.Descricao : string.Empty;
                etiqueta.FilialEmissora = empresaEmissora?.CodigoIntegracao ?? "";
                etiqueta.DataEmissaoNF = dataEmissaoNF;
                etiqueta.Endereco = destinatario.Endereco + ", " + destinatario.Bairro + ", " + destinatario.Numero;
                etiqueta.CidadeEstado =
                    destinatario.Localidade.Descricao + " - " + destinatario.Localidade.Estado.Sigla;
                etiqueta.CEP = cep;
                etiqueta.Serie = Utilidades.String.OnlyNumbers(serie);

                string codigoBarrasFormatado = remetente.CPF_CNPJ_SemFormato.PadLeft(14, '0');
                codigoBarrasFormatado += Utilidades.String.OnlyNumbers(numero).PadLeft(9, '0');
                codigoBarrasFormatado += Utilidades.String.OnlyNumbers(etiqueta.Volume.ToString("n0")).PadLeft(4, '0');
                codigoBarrasFormatado += Utilidades.String.OnlyNumbers(serie).PadLeft(2, '0');
                codigoBarrasFormatado += "00";

                byte[] codigoBarras = !ConfiguracaoEmbarcador.UtilizarEtiquetaDetalhadaWMS
                    ? Utilidades.Barcode.Gerar(codigoBarrasFormatado, ZXing.BarcodeFormat.CODE_128,
                        new BarcodeMetrics1d(1, 30), System.Drawing.Imaging.ImageFormat.Png)
                    : Utilidades.QRcode.Gerar(codigoBarrasFormatado);
                etiqueta.CodigoBarras = codigoBarras;

                etiqueta.MetrosCubicos = metroCubico;
                etiqueta.Peso = pesoCarga;
                etiqueta.Sam = destinatario?.NomeFantasia ?? destinatario.Nome;
                etiqueta.Cross = carga?.TipoOperacao?.Descricao ?? "";
                etiqueta.CodigoEtiqueta = codigoBarrasFormatado;
                etiqueta.CodigoBarras = codigoBarras;
                etiqueta.DataEtiqueta = DateTime.Now;

                etiqueta.Transbordo = carga?.CodigoCargaEmbarcador ?? "";
                etiqueta.Item = produtoEmbarcador?.Descricao ?? "";

                #endregion

                if (TipoServicoMultisoftware ==
                    AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                {
                    etiqueta.NumeroNotaFiscal = "";
                    etiqueta.NumeroNotaFiscal = numero;
                    if (!string.IsNullOrWhiteSpace(chaveNFe))
                    {
                        Dominio.Entidades.Embarcador.WMS.MontagemContainer montagemContainer =
                            repMontagemContainer.BuscarPorNotaFiscal(chaveNFe);
                        if (montagemContainer != null && montagemContainer.Container != null)
                            etiqueta.NumeroContainer = montagemContainer.Container.Numero;
                        else
                            etiqueta.NumeroContainer = "";
                    }

                    if (string.IsNullOrWhiteSpace(etiqueta.NumeroContainer) && !string.IsNullOrWhiteSpace(chaveNFe))
                    {
                        Dominio.Entidades.Embarcador.Cargas.Carga cargaNota =
                            repPedidoXMLNotaFiscal.BuscarCargaPorChave(chaveNFe, 0, 0d, 0d);
                        if (cargaNota != null && !string.IsNullOrWhiteSpace(cargaNota.Containeres))
                            etiqueta.NumeroContainer = cargaNota.Containeres;
                        else
                            etiqueta.NumeroContainer = "";
                    }
                }
                else
                {
                    etiqueta.NumeroNotaFiscal = "";
                    etiqueta.NumeroContainer = "";
                }

                listaRetorno.Add(etiqueta);
            }

            return listaRetorno;
        }

        private List<Dominio.Relatorios.Embarcador.DataSource.WMS.IEtiquetaCarga> RetornarDadosEtiquetaMultiplas(
            Repositorio.UnitOfWork unitOfWork, int codigoRecebimento)
        {
            decimal quantidadeVolumes = 0;
            double? cnpjDestinatario = 0, cnpjRemetente = 0;
            int? codigoEmissora = 0;

            int codigoCarga = Request.GetIntParam("CodigoCarga");

            Repositorio.Embarcador.WMS.RecebimentoMercadoria repRecebimentoMercadoria =
                new Repositorio.Embarcador.WMS.RecebimentoMercadoria(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Logistica.RotaFreteCEP repRotaCEP =
                new Repositorio.Embarcador.Logistica.RotaFreteCEP(unitOfWork);
            Repositorio.Embarcador.WMS.MontagemContainerNotaFiscal repMontagemContainer =
                new Repositorio.Embarcador.WMS.MontagemContainerNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal =
                new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            List<Dominio.Relatorios.Embarcador.DataSource.WMS.IEtiquetaCarga> listaRetorno =
                new List<Dominio.Relatorios.Embarcador.DataSource.WMS.IEtiquetaCarga>();
            BarcodeMetrics1d metricas = new BarcodeMetrics1d();
            metricas.Scale = 5;

            if (codigoRecebimento > 0)
            {
                List<Dominio.Entidades.Embarcador.WMS.RecebimentoMercadoria> listaVolumes =
                    repRecebimentoMercadoria.BuscarPorRecebimento(codigoRecebimento);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                foreach (var volume in listaVolumes)
                {
                    codigoEmissora = volume.EmpresaEmissora?.Codigo;
                    cnpjRemetente = volume.Remetente?.CPF_CNPJ;
                    cnpjDestinatario = volume.Destinatario?.CPF_CNPJ;
                    DateTime dataEmissaoNF = volume?.DataEmissaoNF ?? DateTime.Now;

                    string cep = Utilidades.String.OnlyNumbers(volume.Destinatario?.CEP ?? "");
                    if (string.IsNullOrWhiteSpace(cep))
                        cep = "0";
                    Dominio.Entidades.Embarcador.Logistica.RotaFreteCEP rotaCEP =
                        repRotaCEP.BuscarPorCEP(int.Parse(cep));

                    decimal quantidadeLote = volume.QuantidadeLote;
                    decimal quantidadeConferida = volume.QuantidadeConferida;
                    decimal quantidadeFaltante = volume.QuantidadeFaltante;
                    quantidadeVolumes = volume.QuantidadeLote;
                    string numeroPedido = volume.NumeroLote;
                    string numero = volume.NumeroLote;
                    string serie = volume.Serie;
                    string numeroNota = volume.NumeroNF > 0 ? volume.NumeroNF.ToString("D") : volume.NumeroLote;
                    string chaveNotaFiscal = volume.ChaveNFe;

                    Dominio.Entidades.Cliente destinatario = repCliente.BuscarPorCPFCNPJ(cnpjDestinatario.Value);
                    Dominio.Entidades.Cliente remetente = repCliente.BuscarPorCPFCNPJ(cnpjRemetente.Value);
                    Dominio.Entidades.Empresa empresaEmissora = codigoEmissora.HasValue
                        ? repEmpresa.BuscarPorCodigo(codigoEmissora.Value)
                        : null;

                    if (empresaEmissora == null)
                        empresaEmissora = repEmpresa.BuscarEmpresaPadraoRetirada();

                    string cepDestinatario = Utilidades.String.OnlyNumbers(destinatario.CEP);
                    if (string.IsNullOrWhiteSpace(cepDestinatario))
                        cepDestinatario = "0";

                    for (int i = 0; i < quantidadeLote; i++)
                    {
                        Dominio.Relatorios.Embarcador.DataSource.WMS.EtiquetaVolumeLRMaster etiqueta =
                            new Dominio.Relatorios.Embarcador.DataSource.WMS.EtiquetaVolumeLRMaster();

                        #region cabeçalho

                        etiqueta.NomeTransportadora = empresaEmissora?.NomeFantasia ?? "";
                        etiqueta.EnderecoTransportadora = empresaEmissora?.Endereco ?? "";
                        etiqueta.Bairro = empresaEmissora?.Bairro ?? "";
                        etiqueta.CidadeTransportadora = empresaEmissora?.Localidade?.Descricao ?? "";
                        etiqueta.UFTransportadora = empresaEmissora?.Localidade?.Estado?.Sigla ?? "";
                        etiqueta.CEPTransportadora = empresaEmissora?.CEP ?? "";
                        etiqueta.CNPJTransportadora = empresaEmissora?.CNPJ_Formatado ?? "";
                        etiqueta.InscricaoEstadual = empresaEmissora?.InscricaoEstadual ?? "";
                        etiqueta.Telefone = empresaEmissora?.Telefone ?? "";
                        etiqueta.RNTC = "";
                        etiqueta.UFLocalidade = "";
                        etiqueta.Localidade = "";

                        #endregion

                        #region Remetente

                        etiqueta.Remetente = remetente.Nome;
                        etiqueta.EnderecoRemetente = remetente?.Endereco ?? "";
                        etiqueta.CidadeRemetente = remetente.Localidade?.Descricao ?? "";
                        etiqueta.UFRemetente = remetente.Localidade.Estado?.Sigla ?? "";
                        etiqueta.CPFCNPJRemetente = remetente?.CPF_CNPJ_Formatado ?? "";
                        etiqueta.PaisRemetente = remetente.Localidade?.Pais?.Nome ?? "";
                        etiqueta.CEPRemetente = remetente?.CEP ?? "";
                        etiqueta.InscricaoRemetente = remetente.InscricaoMunicipal ?? "";
                        etiqueta.TelefoneRemetente = remetente?.Telefone1 ?? "";

                        #endregion

                        #region Destinatário

                        etiqueta.Destinatario = destinatario.Nome;
                        etiqueta.EnderecoDestinatario = destinatario.Endereco + ", " + destinatario.Bairro + ", " +
                                                        destinatario.Numero;
                        etiqueta.CidadeDestinatario = destinatario.Localidade?.Descricao ?? "";
                        etiqueta.UFDestinatario = destinatario.Localidade.Estado?.Sigla ?? "";
                        etiqueta.CPFCNPJDestinatario = cepDestinatario;
                        etiqueta.PaisDestinatario = destinatario.Localidade?.Pais?.Nome ?? "";
                        etiqueta.CEPDestinatario = destinatario?.CEP ?? "";
                        etiqueta.InscricaoDestinatario = destinatario?.InscricaoMunicipal ?? "";
                        etiqueta.TelefoneDestinatario = destinatario?.Telefone1 ?? "";

                        #endregion

                        #region Carga

                        etiqueta.Carga = carga?.CodigoCargaEmbarcador ?? "";
                        etiqueta.NumeroPedido = numeroPedido;
                        etiqueta.VolumeTotal = quantidadeVolumes;
                        etiqueta.Volume = i + 1;
                        etiqueta.NumeroNota = Utilidades.String.OnlyNumbers(numero);
                        etiqueta.FilialDistribuidora =
                            rotaCEP != null ? rotaCEP.RotaFrete.FilialDistribuidora : string.Empty;
                        etiqueta.Rota = rotaCEP != null ? rotaCEP.RotaFrete.Descricao : string.Empty;
                        etiqueta.FilialEmissora = empresaEmissora?.CodigoIntegracao ?? "";
                        etiqueta.DataEmissaoNF = dataEmissaoNF;
                        etiqueta.Endereco = destinatario.Endereco + ", " + destinatario.Bairro + ", " +
                                            destinatario.Numero;
                        etiqueta.CidadeEstado = destinatario.Localidade.Descricao + " - " +
                                                destinatario.Localidade.Estado.Sigla;
                        etiqueta.CEP = cep;
                        etiqueta.Serie = Utilidades.String.OnlyNumbers(serie);

                        string codigoBarrasFormatado = remetente.CPF_CNPJ_SemFormato.PadLeft(14, '0');
                        codigoBarrasFormatado += volume.NumeroNF > 0
                            ? Utilidades.String.OnlyNumbers(volume.NumeroNF.ToString()).PadLeft(9, '0')
                            : Utilidades.String.OnlyNumbers(volume.NumeroLote.ToString()).PadLeft(9, '0');
                        codigoBarrasFormatado += Utilidades.String.OnlyNumbers(etiqueta.Volume.ToString("n0"))
                            .PadLeft(4, '0');
                        codigoBarrasFormatado += Utilidades.String.OnlyNumbers(volume.Serie).PadLeft(2, '0');
                        codigoBarrasFormatado += "00";

                        byte[] codigoBarras = !ConfiguracaoEmbarcador.UtilizarEtiquetaDetalhadaWMS
                            ? Utilidades.Barcode.Gerar(codigoBarrasFormatado, ZXing.BarcodeFormat.CODE_128,
                                new BarcodeMetrics1d(1, 30), System.Drawing.Imaging.ImageFormat.Png)
                            : Utilidades.QRcode.Gerar(codigoBarrasFormatado);

                        etiqueta.MetrosCubicos = volume.MetroCubico;
                        etiqueta.Peso = volume.Peso;
                        etiqueta.Sam = destinatario?.NomeFantasia ?? destinatario.Nome;
                        etiqueta.Cross = carga?.TipoOperacao?.Descricao ?? "";
                        etiqueta.CodigoEtiqueta = codigoBarrasFormatado;
                        etiqueta.CodigoBarras = codigoBarras;
                        etiqueta.DataEtiqueta = DateTime.Now;

                        etiqueta.Transbordo = carga?.CodigoCargaEmbarcador ?? "";
                        etiqueta.Item = volume?.ProdutoEmbarcador?.Descricao ?? "";

                        #endregion


                        if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware
                                .MultiEmbarcador)
                        {
                            etiqueta.NumeroNotaFiscal = "";
                            etiqueta.NumeroNotaFiscal = numeroNota;
                            if (!string.IsNullOrWhiteSpace(chaveNotaFiscal))
                            {
                                Dominio.Entidades.Embarcador.WMS.MontagemContainer montagemContainer =
                                    repMontagemContainer.BuscarPorNotaFiscal(chaveNotaFiscal);
                                if (montagemContainer != null && montagemContainer.Container != null)
                                    etiqueta.NumeroContainer = montagemContainer.Container.Numero;
                                else
                                    etiqueta.NumeroNotaFiscal = "";
                            }

                            if (string.IsNullOrWhiteSpace(etiqueta.NumeroContainer) &&
                                !string.IsNullOrWhiteSpace(chaveNotaFiscal))
                            {
                                Dominio.Entidades.Embarcador.Cargas.Carga cargaNota =
                                    repPedidoXMLNotaFiscal.BuscarCargaPorChave(chaveNotaFiscal, 0, 0d, 0d);
                                if (cargaNota != null && !string.IsNullOrWhiteSpace(cargaNota.Containeres))
                                    etiqueta.NumeroContainer = cargaNota.Containeres;
                                else
                                    etiqueta.NumeroNotaFiscal = "";
                            }
                        }
                        else
                        {
                            etiqueta.NumeroNotaFiscal = "";
                            etiqueta.NumeroContainer = "";
                        }

                        listaRetorno.Add(etiqueta);
                    }
                }
            }

            return listaRetorno;
        }

        private bool FinalizarRecebimentoMercadoria(Dominio.Entidades.Embarcador.WMS.Recebimento recebimento,
            Repositorio.UnitOfWork unitOfWork, out string msgRetorno)
        {
            msgRetorno = "";
            Repositorio.Embarcador.Produtos.ProdutoEmbarcadorLote repProdutoEmbarcadorLote =
                new Repositorio.Embarcador.Produtos.ProdutoEmbarcadorLote(unitOfWork);
            Repositorio.Embarcador.WMS.RecebimentoMercadoria repRecebimentoMercadoria =
                new Repositorio.Embarcador.WMS.RecebimentoMercadoria(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal =
                new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Servicos.Embarcador.Produto.Estoque servicoEstoque = new Servicos.Embarcador.Produto.Estoque(unitOfWork);

            List<Dominio.Entidades.Embarcador.WMS.RecebimentoMercadoria> mercadorias =
                repRecebimentoMercadoria.BuscarPorRecebimento(recebimento.Codigo);

            if (mercadorias == null || mercadorias.Count <= 0)
                return false;

            if (recebimento.TipoRecebimentoMercadoria ==
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria.Volume)
            {
                for (int i = 0; i < mercadorias.Count; i++)
                {
                    if (!ValidarCamposObrigatoriosVolume(mercadorias[i]))
                        return false;

                    if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware
                            .MultiEmbarcador && string.IsNullOrWhiteSpace(mercadorias[i].CodigoBarras))
                    {
                        msgRetorno = "Volume lançado sem código de barras, favor verifique e refaça o lançamento.";
                        return false;
                    }

                    Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorLote lote =
                        new Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorLote();
                    lote.CodigoBarras = mercadorias[i].CodigoBarras;
                    lote.DataVencimento = null;
                    lote.DepositoPosicao = mercadorias[i].DepositoPosicao;
                    lote.Descricao = mercadorias[i].Descricao;
                    lote.MetroCubico = mercadorias[i].MetroCubico;
                    lote.NCM = string.Empty;
                    lote.Numero = mercadorias[i].NumeroLote;
                    lote.Peso = mercadorias[i].Peso;
                    lote.ProdutoEmbarcador = mercadorias[i].ProdutoEmbarcador;
                    lote.QuantidadeLote = mercadorias[i].QuantidadeLote;
                    lote.QuantidadePalet = 0;
                    lote.QuantidadeAtual = mercadorias[i].QuantidadeConferida;
                    lote.TipoRecebimentoMercadoria =
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria.Volume;

                    lote.Remetente = mercadorias[i].Remetente;
                    lote.Destinatario = mercadorias[i].Destinatario;
                    lote.EmpresaEmissora = mercadorias[i].EmpresaEmissora;

                    repProdutoEmbarcadorLote.Inserir(lote, Auditado);
                    if (mercadorias[i].XMLNotaFiscal != null)
                    {
                        mercadorias[i].XMLNotaFiscal.Comprimento = mercadorias[i].Comprimento;
                        mercadorias[i].XMLNotaFiscal.Largura = mercadorias[i].Largura;
                        mercadorias[i].XMLNotaFiscal.Altura = mercadorias[i].Altura;
                        mercadorias[i].XMLNotaFiscal.MetrosCubicos = mercadorias[i].MetroCubico;

                        if (mercadorias[i].XMLNotaFiscal.Volumes == 0 && mercadorias[i].QuantidadeLote > 0)
                            mercadorias[i].XMLNotaFiscal.Volumes = (int)mercadorias[i].QuantidadeLote;

                        repXMLNotaFiscal.Atualizar(mercadorias[i].XMLNotaFiscal);
                    }
                }
            }
            else
            {
                for (int i = 0; i < mercadorias.Count; i++)
                {
                    if (string.IsNullOrWhiteSpace(mercadorias[i].CodigoBarras))
                        mercadorias[i].CodigoBarras = mercadorias[i].ProdutoEmbarcador.Codigo.ToString("D");

                    if (!ValidarCamposObrigatorios(mercadorias[i]))
                        return false;

                    if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware
                            .MultiEmbarcador && string.IsNullOrWhiteSpace(mercadorias[i].CodigoBarras))
                    {
                        msgRetorno = "Volume lançado sem código de barras, favor verifique e refaça o lançamento.";
                        return false;
                    }

                    Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorLote lote =
                        new Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorLote();
                    lote.CodigoBarras = mercadorias[i].CodigoBarras;
                    lote.DataVencimento = mercadorias[i].DataVencimento;
                    lote.DepositoPosicao = mercadorias[i].DepositoPosicao;
                    lote.Descricao = mercadorias[i].Descricao;
                    lote.MetroCubico = mercadorias[i].MetroCubico;
                    lote.NCM = mercadorias[i].NCM;
                    lote.Numero = mercadorias[i].NumeroLote;
                    lote.Peso = mercadorias[i].Peso;
                    lote.ProdutoEmbarcador = mercadorias[i].ProdutoEmbarcador;
                    lote.QuantidadeLote = mercadorias[i].QuantidadeLote;
                    lote.QuantidadePalet = mercadorias[i].QuantidadePalet;
                    lote.QuantidadeAtual = mercadorias[i].QuantidadeLote;
                    lote.TipoRecebimentoMercadoria = Dominio.ObjetosDeValor.Embarcador.Enumeradores
                        .TipoRecebimentoMercadoria.Mercadoria;

                    repProdutoEmbarcadorLote.Inserir(lote, Auditado);

                    if (mercadorias[i].Produto != null)
                    {
                        if (!servicoEstoque.MovimentarEstoque(out string erro, mercadorias[i].Produto,
                                mercadorias[i].QuantidadeLote, Dominio.Enumeradores.TipoMovimento.Entrada, "ENT",
                                "EMIT " + (mercadorias[i].Remetente?.CPF_CNPJ_SemFormato ?? "") + "-" +
                                mercadorias[i].ChaveNFe, mercadorias[i].ValorUnitario,
                                recebimento.Carga?.Empresa ?? null, DateTime.Now, TipoServicoMultisoftware))
                            return false;
                    }

                    if (mercadorias[i].XMLNotaFiscal != null)
                    {
                        mercadorias[i].XMLNotaFiscal.Comprimento = mercadorias[i].Comprimento;
                        mercadorias[i].XMLNotaFiscal.Largura = mercadorias[i].Largura;
                        mercadorias[i].XMLNotaFiscal.Altura = mercadorias[i].Altura;
                        mercadorias[i].XMLNotaFiscal.MetrosCubicos = mercadorias[i].MetroCubico;

                        if (mercadorias[i].XMLNotaFiscal.Volumes == 0 && mercadorias[i].QuantidadeLote > 0)
                            mercadorias[i].XMLNotaFiscal.Volumes = (int)mercadorias[i].QuantidadeLote;

                        repXMLNotaFiscal.Atualizar(mercadorias[i].XMLNotaFiscal);
                    }
                }
            }

            return true;
        }

        private bool ValidarCamposObrigatorios(Dominio.Entidades.Embarcador.WMS.RecebimentoMercadoria mercadoria)
        {
            return mercadoria.DepositoPosicao != null && !string.IsNullOrWhiteSpace(mercadoria.CodigoBarras) &&
                   !string.IsNullOrWhiteSpace(mercadoria.Descricao) && mercadoria.ProdutoEmbarcador != null &&
                   mercadoria.QuantidadeLote > 0;
        }

        private bool ValidarCamposObrigatoriosVolume(Dominio.Entidades.Embarcador.WMS.RecebimentoMercadoria mercadoria)
        {
            return mercadoria.DepositoPosicao != null && mercadoria.ProdutoEmbarcador != null &&
                   mercadoria.QuantidadeLote > 0;
        }

        private List<Item> RetornaListaItens(MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc nfeProc,
            Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.WMS.Recebimento recebimento,
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria tipoRecebimento,
            int codigoProdutoEmbarcador, byte[] arquivo)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");
            List<Item> listaItem = new List<Item>();
            Servicos.NFe serNFe = new Servicos.NFe(unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcadorCliente repProdutoEmbarcadorCliente =
                new Repositorio.Embarcador.Produtos.ProdutoEmbarcadorCliente(unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador =
                new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);
            Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);

            Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador =
                repProdutoEmbarcador.BuscarPorCodigo(codigoProdutoEmbarcador);
            Servicos.Embarcador.WMS.LoteProdutoEmbarcador svcLoteProdutoEmbarcadora =
                new Servicos.Embarcador.WMS.LoteProdutoEmbarcador(unitOfWork);

            if (tipoRecebimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria.Mercadoria)
            {
                foreach (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeDet det in nfeProc.NFe.infNFe.det)
                {
                    Item item = new Item();

                    Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorCliente produtoCliente =
                        repProdutoEmbarcadorCliente.BuscarPorClienteCodigo(det.prod.cProd,
                            double.Parse(Utilidades.String.OnlyNumbers(nfeProc.NFe.infNFe.emit.Item)));
                    Dominio.Entidades.Embarcador.WMS.DepositoPosicao posicao =
                        svcLoteProdutoEmbarcadora.BuscarPosicaoPadrao(produtoCliente?.ProdutoEmbarcador,
                            decimal.Parse(det.prod.qCom, cultura),
                            produtoCliente != null
                                ? produtoCliente.ProdutoEmbarcador.PesoUnitario * decimal.Parse(det.prod.qCom, cultura)
                                : 0,
                            produtoCliente != null
                                ? produtoCliente.ProdutoEmbarcador.MetroCubito * decimal.Parse(det.prod.qCom, cultura)
                                : 0, produtoCliente != null ? produtoCliente.ProdutoEmbarcador.QtdPalet : 0,
                            unitOfWork);

                    Dominio.Entidades.Produto produto = repProduto.BuscarPorCodigoProduto(det.prod.cProd);
                    if (produto == null)
                    {
                        int codigoProduto =
                            CadastrarProduto(det.prod.xProd, det.prod.cProd, det.prod.NCM, 0, unitOfWork);
                        produto = repProduto.BuscarPorCodigo(codigoProduto);
                    }

                    item.Codigo = 0;
                    item.CNPJCliente = double.Parse(Utilidades.String.OnlyNumbers(nfeProc.NFe.infNFe.emit.Item));
                    item.Descricao = det.prod.xProd;
                    item.NumeroLote = "";
                    item.NCM = det.prod.NCM;
                    item.ChaveNFe = nfeProc.protNFe != null ? nfeProc.protNFe.infProt.chNFe : string.Empty;
                    item.Identificacao = nfeProc.protNFe != null ? nfeProc.protNFe.infProt.chNFe : string.Empty;
                    item.CodigoBarras = det.prod.cProd;
                    item.DataVencimento = string.Empty;
                    item.QuantidadeLote = decimal.Parse(det.prod.qCom, cultura).ToString("n3");
                    item.MetroCubico = produtoCliente != null
                        ? (produtoCliente.ProdutoEmbarcador.MetroCubito * decimal.Parse(det.prod.qCom, cultura))
                        .ToString("n3")
                        : 0.ToString("n3");
                    item.Peso = produtoCliente != null
                        ? (produtoCliente.ProdutoEmbarcador.PesoUnitario * decimal.Parse(det.prod.qCom, cultura))
                        .ToString("n3")
                        : 0.ToString("n3");
                    item.QuantidadePalet = produtoCliente != null
                        ? (produtoCliente.ProdutoEmbarcador.QtdPalet).ToString("n3")
                        : 0.ToString("n3");
                    item.CodigoDepositoPosicao = posicao != null ? posicao.Codigo : 0;
                    item.CodigoRecebimento = recebimento != null ? recebimento.Codigo : 0;
                    item.CodigoProdutoEmbarcador = produtoCliente != null ? produtoCliente.ProdutoEmbarcador.Codigo : 0;
                    item.DescricaoDepositoPosicao = posicao != null ? posicao.Abreviacao : "";
                    item.DescricaoRecebimento = recebimento != null ? recebimento.Codigo.ToString() : "";
                    item.DescricaoProdutoEmbarcador = produtoCliente != null
                        ? produtoCliente.ProdutoEmbarcador.Descricao
                        : string.Empty;
                    item.CodigoProduto = produto.Codigo;
                    item.ValorUnitario = (decimal.Parse(det.prod.vUnCom, cultura)).ToString("n4");
                    item.DescricaoProduto = det.prod.xProd;

                    item.CodigoEmissora = 0;
                    item.NomeEmissora = "";
                    item.CNPJRemetente = double.Parse(Utilidades.String.OnlyNumbers(nfeProc.NFe.infNFe.emit.Item));
                    item.CNPJDestinatario = double.Parse(Utilidades.String.OnlyNumbers(nfeProc.NFe.infNFe.dest.Item));
                    item.NomeDestinatario = nfeProc.NFe.infNFe.dest.xNome;
                    item.DataEmissaoNF = DateTime.Parse(nfeProc.NFe.infNFe.ide.dhEmi, cultura).ToString("dd/MM/yyyy");
                    item.Numero = nfeProc.NFe.infNFe.ide.nNF;
                    item.Serie = nfeProc.NFe.infNFe.ide.serie;
                    item.QuantidadeConferida = decimal.Parse("0", cultura).ToString("n3");
                    item.QuantidadeFaltante = decimal.Parse(det.prod.qCom, cultura).ToString("n3");
                    item.NomeRemetente = nfeProc.NFe.infNFe.emit.xNome;

                    item.CodigoMDFe = 0;
                    item.NumeroMDFe = string.Empty;
                    item.CodigoCarga = 0;
                    item.NumeroCarga = string.Empty;
                    item.CodigoVeiculo = 0;
                    item.PlacaVeiculo = string.Empty;

                    item.PesoBruto = decimal.Parse(nfeProc.NFe.infNFe.transp?.vol[0]?.pesoB ?? "0", cultura)
                        .ToString("n3");
                    item.PesoLiquido = decimal.Parse(nfeProc.NFe.infNFe.transp?.vol[0]?.pesoL ?? "0", cultura)
                        .ToString("n3");
                    item.NumeroNF = nfeProc.NFe.infNFe.ide.nNF;
                    item.SerieNF = nfeProc.NFe.infNFe.ide.serie;
                    item.ValorMercadoria =
                        decimal.Parse(nfeProc.NFe.infNFe.total.ICMSTot.vProd, cultura).ToString("n3");
                    item.ValorNF = decimal.Parse(nfeProc.NFe.infNFe.total.ICMSTot.vNF, cultura).ToString("n3");
                    item.VolumeNF = nfeProc.NFe.infNFe.transp?.vol[0]?.qVol ?? "0";

                    listaItem.Add(item);

                    SalvarItemRecebimento(recebimento, item, unitOfWork,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria.Mercadoria, arquivo);
                }
            }
            else
            {
                foreach (MultiSoftware.NFe.v400.NotaFiscal.TNFeInfNFeTranspVol vol in nfeProc.NFe.infNFe.transp.vol)
                {
                    Dominio.Entidades.Embarcador.WMS.DepositoPosicao posicao =
                        svcLoteProdutoEmbarcadora.BuscarPosicaoPadrao(produtoEmbarcador,
                            decimal.Parse(vol?.qVol ?? "0", cultura), 0, 0, 0, unitOfWork);
                    Item item = new Item();
                    item.Codigo = 0;
                    item.CNPJCliente = double.Parse(Utilidades.String.OnlyNumbers(nfeProc.NFe.infNFe.emit.Item));
                    item.Descricao = nfeProc.NFe.infNFe.ide.nNF;
                    item.NumeroLote = Utilidades.String.OnlyNumbers(nfeProc.NFe.infNFe.ide.nNF);
                    item.NCM = "";
                    item.ChaveNFe = nfeProc.protNFe != null ? nfeProc.protNFe.infProt.chNFe : string.Empty;
                    item.Identificacao = nfeProc.protNFe != null ? nfeProc.protNFe.infProt.chNFe : string.Empty;
                    item.CodigoBarras = Utilidades.String.OnlyNumbers(nfeProc.NFe.infNFe.emit.Item).PadLeft(14, '0') +
                                        nfeProc.NFe.infNFe.ide.nNF.PadLeft(9, '0') +
                                        (vol?.qVol ?? "0").PadLeft(4, '0') +
                                        nfeProc.NFe.infNFe.ide.serie.PadLeft(2, '0') + "00";
                    item.DataVencimento = string.Empty;
                    item.QuantidadeLote = decimal.Parse((vol?.qVol ?? "0"), cultura).ToString("n3");
                    item.MetroCubico = 0.ToString("n3");
                    item.Peso = 0.ToString("n3");
                    item.QuantidadePalet = 0.ToString("n3");
                    item.CodigoDepositoPosicao = posicao != null ? posicao.Codigo : 0;
                    item.CodigoRecebimento = recebimento != null ? recebimento.Codigo : 0;
                    item.CodigoProdutoEmbarcador = produtoEmbarcador != null ? produtoEmbarcador.Codigo : 0;
                    item.DescricaoDepositoPosicao = posicao != null ? posicao.Abreviacao : "";
                    item.DescricaoRecebimento = recebimento != null ? recebimento.Codigo.ToString() : "";
                    item.DescricaoProdutoEmbarcador =
                        produtoEmbarcador != null ? produtoEmbarcador.Descricao : string.Empty;

                    item.CodigoEmissora = 0;
                    item.NomeEmissora = "";
                    item.CNPJRemetente = double.Parse(Utilidades.String.OnlyNumbers(nfeProc.NFe.infNFe.emit.Item));
                    item.CNPJDestinatario = double.Parse(Utilidades.String.OnlyNumbers(nfeProc.NFe.infNFe.dest.Item));
                    item.NomeDestinatario = nfeProc.NFe.infNFe.dest.xNome;
                    item.DataEmissaoNF = DateTime.Parse(nfeProc.NFe.infNFe.ide.dhEmi, cultura).ToString("dd/MM/yyyy");
                    item.Numero = nfeProc.NFe.infNFe.ide.nNF;
                    item.Serie = Utilidades.String.OnlyNumbers(nfeProc.NFe.infNFe.ide.serie);
                    item.QuantidadeConferida = decimal.Parse("0", cultura).ToString("n3");
                    item.QuantidadeFaltante = decimal.Parse((vol?.qVol ?? "0"), cultura).ToString("n3");
                    item.NomeRemetente = nfeProc.NFe.infNFe.emit.xNome;

                    Dominio.Entidades.Cliente emitenteNFe =
                        serNFe.ObterEmitente(nfeProc.NFe.infNFe.emit, this.Empresa.Codigo);
                    Dominio.Entidades.Cliente destinatarioNFe =
                        serNFe.ObterDestinatario(nfeProc.NFe.infNFe.dest, this.Empresa.Codigo);

                    item.CodigoMDFe = 0;
                    item.NumeroMDFe = string.Empty;
                    item.CodigoCarga = 0;
                    item.NumeroCarga = string.Empty;
                    item.CodigoVeiculo = 0;
                    item.PlacaVeiculo = string.Empty;

                    item.PesoBruto = decimal.Parse(nfeProc.NFe.infNFe.transp?.vol[0]?.pesoB ?? "0", cultura)
                        .ToString("n3");
                    item.PesoLiquido = decimal.Parse(nfeProc.NFe.infNFe.transp?.vol[0]?.pesoL ?? "0", cultura)
                        .ToString("n3");
                    item.NumeroNF = nfeProc.NFe.infNFe.ide.nNF;
                    item.SerieNF = nfeProc.NFe.infNFe.ide.serie;
                    item.ValorMercadoria =
                        decimal.Parse(nfeProc.NFe.infNFe.total.ICMSTot.vProd, cultura).ToString("n3");
                    item.ValorNF = decimal.Parse(nfeProc.NFe.infNFe.total.ICMSTot.vNF, cultura).ToString("n3");
                    item.VolumeNF = nfeProc.NFe.infNFe.transp?.vol[0]?.qVol ?? "0";

                    listaItem.Add(item);

                    SalvarItemRecebimento(recebimento, item, unitOfWork,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria.Volume, arquivo);
                }
            }

            return listaItem;
        }

        private List<Item> RetornaListaItens(MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc nfeProc,
            Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.WMS.Recebimento recebimento,
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria tipoRecebimento,
            int codigoProdutoEmbarcador)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");
            List<Item> listaItem = new List<Item>();
            Servicos.NFe serNFe = new Servicos.NFe(unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcadorCliente repProdutoEmbarcadorCliente =
                new Repositorio.Embarcador.Produtos.ProdutoEmbarcadorCliente(unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador =
                new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);
            Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
            Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador =
                repProdutoEmbarcador.BuscarPorCodigo(codigoProdutoEmbarcador);
            Servicos.Embarcador.WMS.LoteProdutoEmbarcador svcLoteProdutoEmbarcadora =
                new Servicos.Embarcador.WMS.LoteProdutoEmbarcador(unitOfWork);
            if (tipoRecebimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria.Mercadoria)
            {
                foreach (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeDet det in nfeProc.NFe.infNFe.det)
                {
                    Item item = new Item();

                    Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorCliente produtoCliente =
                        repProdutoEmbarcadorCliente.BuscarPorClienteCodigo(det.prod.cProd,
                            double.Parse(Utilidades.String.OnlyNumbers(nfeProc.NFe.infNFe.emit.Item)));
                    Dominio.Entidades.Embarcador.WMS.DepositoPosicao posicao =
                        svcLoteProdutoEmbarcadora.BuscarPosicaoPadrao(produtoCliente?.ProdutoEmbarcador,
                            decimal.Parse(det.prod.qCom, cultura),
                            produtoCliente != null
                                ? produtoCliente.ProdutoEmbarcador.PesoUnitario * decimal.Parse(det.prod.qCom, cultura)
                                : 0,
                            produtoCliente != null
                                ? produtoCliente.ProdutoEmbarcador.MetroCubito * decimal.Parse(det.prod.qCom, cultura)
                                : 0, produtoCliente != null ? produtoCliente.ProdutoEmbarcador.QtdPalet : 0,
                            unitOfWork);

                    Dominio.Entidades.Produto produto = repProduto.BuscarPorCodigoProduto(det.prod.cProd);
                    if (produto == null)
                    {
                        int codigoProduto =
                            CadastrarProduto(det.prod.xProd, det.prod.cProd, det.prod.NCM, 0, unitOfWork);
                        produto = repProduto.BuscarPorCodigo(codigoProduto);
                    }

                    item.Codigo = 0;
                    item.CNPJCliente = double.Parse(Utilidades.String.OnlyNumbers(nfeProc.NFe.infNFe.emit.Item));
                    item.Descricao = det.prod.xProd;
                    item.NumeroLote = "";
                    item.NCM = det.prod.NCM;
                    item.ChaveNFe = nfeProc.protNFe != null ? nfeProc.protNFe.infProt.chNFe : string.Empty;
                    item.Identificacao = nfeProc.protNFe != null ? nfeProc.protNFe.infProt.chNFe : string.Empty;
                    item.CodigoBarras = det.prod.cProd;
                    item.DataVencimento = string.Empty;
                    item.QuantidadeLote = decimal.Parse(det.prod.qCom, cultura).ToString("n3");
                    item.MetroCubico = produtoCliente != null
                        ? (produtoCliente.ProdutoEmbarcador.MetroCubito * decimal.Parse(det.prod.qCom, cultura))
                        .ToString("n3")
                        : 0.ToString("n3");
                    item.Peso = produtoCliente != null
                        ? (produtoCliente.ProdutoEmbarcador.PesoUnitario * decimal.Parse(det.prod.qCom, cultura))
                        .ToString("n3")
                        : 0.ToString("n3");
                    item.QuantidadePalet = produtoCliente != null
                        ? (produtoCliente.ProdutoEmbarcador.QtdPalet).ToString("n3")
                        : 0.ToString("n3");
                    item.CodigoDepositoPosicao = posicao != null ? posicao.Codigo : 0;
                    item.CodigoRecebimento = recebimento != null ? recebimento.Codigo : 0;
                    item.CodigoProdutoEmbarcador = produtoCliente != null ? produtoCliente.ProdutoEmbarcador.Codigo : 0;
                    item.DescricaoDepositoPosicao = posicao != null ? posicao.Abreviacao : "";
                    item.DescricaoRecebimento = recebimento != null ? recebimento.Codigo.ToString() : "";
                    item.DescricaoProdutoEmbarcador = produtoCliente != null
                        ? produtoCliente.ProdutoEmbarcador.Descricao
                        : string.Empty;
                    item.CodigoProduto = produto.Codigo;
                    item.ValorUnitario = (decimal.Parse(det.prod.vUnCom, cultura)).ToString("n4");
                    item.DescricaoProduto = det.prod.xProd;

                    item.CodigoEmissora = 0;
                    item.NomeEmissora = "";
                    item.CNPJRemetente = double.Parse(Utilidades.String.OnlyNumbers(nfeProc.NFe.infNFe.emit.Item));
                    item.CNPJDestinatario = double.Parse(Utilidades.String.OnlyNumbers(nfeProc.NFe.infNFe.dest.Item));
                    item.NomeDestinatario = nfeProc.NFe.infNFe.dest.xNome;
                    item.DataEmissaoNF = DateTime.Parse(nfeProc.NFe.infNFe.ide.dhEmi, cultura).ToString("dd/MM/yyyy");
                    item.Numero = nfeProc.NFe.infNFe.ide.nNF;
                    item.Serie = nfeProc.NFe.infNFe.ide.serie;
                    item.QuantidadeConferida = decimal.Parse("0", cultura).ToString("n3");
                    item.QuantidadeFaltante = decimal.Parse(det.prod.qCom, cultura).ToString("n3");
                    item.NomeRemetente = nfeProc.NFe.infNFe.emit.xNome;

                    item.CodigoMDFe = 0;
                    item.NumeroMDFe = string.Empty;
                    item.CodigoCarga = 0;
                    item.NumeroCarga = string.Empty;
                    item.CodigoVeiculo = 0;
                    item.PlacaVeiculo = string.Empty;

                    item.PesoBruto = string.Empty;
                    item.PesoLiquido = string.Empty;
                    item.NumeroNF = string.Empty;
                    item.SerieNF = string.Empty;
                    item.ValorMercadoria = string.Empty;
                    item.ValorNF = string.Empty;
                    item.VolumeNF = string.Empty;

                    listaItem.Add(item);

                    SalvarItemRecebimento(recebimento, item, unitOfWork,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria.Mercadoria, null);
                }
            }
            else
            {
                foreach (MultiSoftware.NFe.v310.NotaFiscal.TNFeInfNFeTranspVol vol in nfeProc.NFe.infNFe.transp.vol)
                {
                    Dominio.Entidades.Embarcador.WMS.DepositoPosicao posicao =
                        svcLoteProdutoEmbarcadora.BuscarPosicaoPadrao(produtoEmbarcador,
                            decimal.Parse(vol.qVol, cultura), 0, 0, 0, unitOfWork);
                    Item item = new Item();
                    item.Codigo = 0;
                    item.CNPJCliente = double.Parse(Utilidades.String.OnlyNumbers(nfeProc.NFe.infNFe.emit.Item));
                    item.Descricao = nfeProc.NFe.infNFe.ide.nNF;
                    item.NumeroLote = Utilidades.String.OnlyNumbers(nfeProc.NFe.infNFe.ide.nNF);
                    item.NCM = "";
                    item.ChaveNFe = nfeProc.protNFe != null ? nfeProc.protNFe.infProt.chNFe : string.Empty;
                    item.Identificacao = nfeProc.protNFe != null ? nfeProc.protNFe.infProt.chNFe : string.Empty;
                    item.CodigoBarras = Utilidades.String.OnlyNumbers(nfeProc.NFe.infNFe.emit.Item).PadLeft(14, '0') +
                                        nfeProc.NFe.infNFe.ide.nNF.PadLeft(9, '0') + vol.qVol.PadLeft(4, '0') +
                                        nfeProc.NFe.infNFe.ide.serie.PadLeft(2, '0') + "00";
                    item.DataVencimento = string.Empty;
                    item.QuantidadeLote = decimal.Parse(vol.qVol, cultura).ToString("n3");
                    item.MetroCubico = 0.ToString("n3");
                    item.Peso = 0.ToString("n3");
                    item.QuantidadePalet = 0.ToString("n3");
                    item.CodigoDepositoPosicao = posicao != null ? posicao.Codigo : 0;
                    item.CodigoRecebimento = recebimento != null ? recebimento.Codigo : 0;
                    item.CodigoProdutoEmbarcador = produtoEmbarcador != null ? produtoEmbarcador.Codigo : 0;
                    item.DescricaoDepositoPosicao = posicao != null ? posicao.Abreviacao : "";
                    item.DescricaoRecebimento = recebimento != null ? recebimento.Codigo.ToString() : "";
                    item.DescricaoProdutoEmbarcador =
                        produtoEmbarcador != null ? produtoEmbarcador.Descricao : string.Empty;

                    item.CodigoEmissora = 0;
                    item.NomeEmissora = "";
                    item.CNPJRemetente = double.Parse(Utilidades.String.OnlyNumbers(nfeProc.NFe.infNFe.emit.Item));
                    item.CNPJDestinatario = double.Parse(Utilidades.String.OnlyNumbers(nfeProc.NFe.infNFe.dest.Item));
                    item.NomeDestinatario = nfeProc.NFe.infNFe.dest.xNome;
                    item.DataEmissaoNF = DateTime.Parse(nfeProc.NFe.infNFe.ide.dhEmi, cultura).ToString("dd/MM/yyyy");
                    item.Numero = nfeProc.NFe.infNFe.ide.nNF;
                    item.Serie = nfeProc.NFe.infNFe.ide.serie;
                    item.QuantidadeConferida = decimal.Parse("0", cultura).ToString("n3");
                    item.QuantidadeFaltante = decimal.Parse(vol.qVol, cultura).ToString("n3");
                    item.PesoBruto = decimal.Parse(nfeProc.NFe.infNFe.transp?.vol[0]?.pesoB ?? "0", cultura)
                        .ToString("n3");
                    item.PesoLiquido = decimal.Parse(nfeProc.NFe.infNFe.transp?.vol[0]?.pesoL ?? "0", cultura)
                        .ToString("n3");
                    item.ValorMercadoria =
                        decimal.Parse(nfeProc.NFe.infNFe.total.ICMSTot.vProd, cultura).ToString("n3");
                    item.ValorNF = decimal.Parse(nfeProc.NFe.infNFe.total.ICMSTot.vNF, cultura).ToString("n3");
                    item.NomeRemetente = nfeProc.NFe.infNFe.emit.xNome;

                    Dominio.Entidades.Cliente emitenteNFe =
                        serNFe.ObterEmitente(nfeProc.NFe.infNFe.emit, this.Empresa.Codigo);
                    Dominio.Entidades.Cliente destinatarioNFe =
                        serNFe.ObterDestinatario(nfeProc.NFe.infNFe.dest, this.Empresa.Codigo);

                    item.CodigoMDFe = 0;
                    item.NumeroMDFe = string.Empty;
                    item.CodigoCarga = 0;
                    item.NumeroCarga = string.Empty;
                    item.CodigoVeiculo = 0;
                    item.PlacaVeiculo = string.Empty;

                    //item.PesoBruto = string.Empty;
                    //item.PesoLiquido = string.Empty;
                    item.NumeroNF = string.Empty;
                    item.SerieNF = string.Empty;
                    //item.ValorMercadoria = string.Empty;
                    //item.ValorNF = string.Empty;
                    item.VolumeNF = string.Empty;

                    listaItem.Add(item);

                    SalvarItemRecebimento(recebimento, item, unitOfWork,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria.Volume, null);
                }
            }

            return listaItem;
        }

        public class RetornoArquivo
        {
            public int CodigoRecebimento { get; set; }
            public string nome { get; set; }
            public bool processada { get; set; }
            public string mensagem { get; set; }
            public List<Item> Mercadorias { get; set; }
        }

        public class Item
        {
            public int Codigo { get; set; }
            public double CNPJCliente { get; set; }
            public string Descricao { get; set; }
            public string NumeroLote { get; set; }
            public string NCM { get; set; }
            public string ChaveNFe { get; set; }
            public string Identificacao { get; set; }
            public string CodigoBarras { get; set; }
            public string DataVencimento { get; set; }
            public string QuantidadeLote { get; set; }
            public string MetroCubico { get; set; }
            public string ValorUnitario { get; set; }
            public string Peso { get; set; }
            public string QuantidadePalet { get; set; }
            public int CodigoDepositoPosicao { get; set; }
            public int CodigoRecebimento { get; set; }
            public int CodigoProdutoEmbarcador { get; set; }
            public string DescricaoDepositoPosicao { get; set; }
            public string DescricaoRecebimento { get; set; }
            public string DescricaoProdutoEmbarcador { get; set; }

            public int CodigoEmissora { get; set; }
            public string NomeEmissora { get; set; }
            public double CNPJRemetente { get; set; }
            public double CNPJDestinatario { get; set; }
            public string NomeDestinatario { get; set; }
            public string DataEmissaoNF { get; set; }
            public string Numero { get; set; }
            public string Serie { get; set; }
            public string QuantidadeConferida { get; set; }
            public string QuantidadeFaltante { get; set; }
            public string NomeRemetente { get; set; }

            public int CodigoMDFe { get; set; }
            public string NumeroMDFe { get; set; }

            public int CodigoCarga { get; set; }
            public string NumeroCarga { get; set; }

            public int CodigoVeiculo { get; set; }
            public string PlacaVeiculo { get; set; }
            public string DescricaoProduto { get; set; }
            public int CodigoProduto { get; set; }

            public string PesoBruto { get; set; }
            public string PesoLiquido { get; set; }
            public string NumeroNF { get; set; }
            public string SerieNF { get; set; }
            public string ValorMercadoria { get; set; }
            public string ValorNF { get; set; }
            public string VolumeNF { get; set; }
        }

        private void SalvarItemRecebimento(Dominio.Entidades.Embarcador.WMS.Recebimento recebimento, Item item,
            Repositorio.UnitOfWork unitOfWork,
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria tipoRecebimento, byte[] arquivo)
        {
            Repositorio.Embarcador.WMS.RecebimentoMercadoria repRecebimentoMercadoria =
                new Repositorio.Embarcador.WMS.RecebimentoMercadoria(unitOfWork);
            Repositorio.Embarcador.WMS.DepositoPosicao repDepositoPosicao =
                new Repositorio.Embarcador.WMS.DepositoPosicao(unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador =
                new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal =
                new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);
            
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebservice = repConfiguracaoWebService.BuscarConfiguracaoPadrao();

            Dominio.Entidades.Embarcador.WMS.RecebimentoMercadoria recebimentoMercadoria;
            if (item.Codigo > 0)
                recebimentoMercadoria = repRecebimentoMercadoria.BuscarPorCodigo(item.Codigo, true);
            else
                recebimentoMercadoria = new Dominio.Entidades.Embarcador.WMS.RecebimentoMercadoria();

            DateTime dataEmissaoNF;
            DateTime.TryParse(item.DataEmissaoNF, out dataEmissaoNF);

            decimal quantidadeLote = 0,
                quantidadeConferida = 0,
                quantidadeFaltante = 0,
                metroCubico = 0,
                peso = 0,
                quantidadePalet = 0,
                valorUnitario = 0,
                pesoBruto = 0,
                pesoLiquido = 0,
                valorMercadoria = 0,
                valorNF = 0;
            decimal.TryParse(item.QuantidadeLote, out quantidadeLote);
            decimal.TryParse(item.QuantidadeConferida, out quantidadeConferida);
            decimal.TryParse(item.QuantidadeFaltante, out quantidadeFaltante);
            decimal.TryParse(item.MetroCubico, out metroCubico);
            decimal.TryParse(item.Peso, out peso);
            decimal.TryParse(item.QuantidadePalet, out quantidadePalet);
            decimal.TryParse(item.ValorUnitario, out valorUnitario);

            decimal.TryParse(item.PesoBruto, out pesoBruto);
            decimal.TryParse(item.PesoLiquido, out pesoLiquido);
            decimal.TryParse(item.ValorMercadoria, out valorMercadoria);
            decimal.TryParse(item.ValorNF, out valorNF);

            int numeroNF = 0, serieNF = 0;
            int.TryParse(item.NumeroNF, out numeroNF);
            int.TryParse(item.SerieNF, out serieNF);

            recebimentoMercadoria.PesoBruto = pesoBruto;
            recebimentoMercadoria.PesoLiquido = pesoLiquido;
            recebimentoMercadoria.NumeroNF = numeroNF;
            recebimentoMercadoria.SerieNF = serieNF;
            recebimentoMercadoria.ValorMercadoria = valorMercadoria;
            recebimentoMercadoria.ValorNF = valorNF;
            recebimentoMercadoria.VolumeNF = item.VolumeNF;

            if (dataEmissaoNF > DateTime.MinValue)
                recebimentoMercadoria.DataEmissaoNF = dataEmissaoNF;
            else
                recebimentoMercadoria.DataEmissaoNF = null;
            //recebimentoMercadoria.CodigoBarras = item.CodigoBarras;
            recebimentoMercadoria.DataVencimento = null;
            recebimentoMercadoria.DepositoPosicao = repDepositoPosicao.BuscarPorCodigo(item.CodigoDepositoPosicao);
            recebimentoMercadoria.Descricao = !string.IsNullOrWhiteSpace(item.Descricao)
                ? item.Descricao
                : Utilidades.String.OnlyNumbers(item.NumeroLote);
            recebimentoMercadoria.NumeroLote = Utilidades.String.OnlyNumbers(item.NumeroLote);
            recebimentoMercadoria.ProdutoEmbarcador =
                repProdutoEmbarcador.BuscarPorCodigo(item.CodigoProdutoEmbarcador);
            recebimentoMercadoria.QuantidadeLote = quantidadeLote;
            recebimentoMercadoria.Recebimento = recebimento;
            recebimentoMercadoria.TipoRecebimentoMercadoria = tipoRecebimento;
            recebimentoMercadoria.Serie = !string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(item.Serie))
                ? Utilidades.String.OnlyNumbers(item.Serie)
                : "1";
            recebimentoMercadoria.QuantidadeConferida = quantidadeConferida;
            recebimentoMercadoria.QuantidadeFaltante = quantidadeFaltante;
            recebimentoMercadoria.CNPJCliente = item.CNPJCliente;
            recebimentoMercadoria.NCM = item.NCM;
            recebimentoMercadoria.ChaveNFe = item.ChaveNFe;
            recebimentoMercadoria.Identificacao = item.Identificacao;
            recebimentoMercadoria.MetroCubico = metroCubico;
            recebimentoMercadoria.Peso = peso > 0 ? peso : pesoBruto;
            recebimentoMercadoria.QuantidadePalet = quantidadePalet;
            recebimentoMercadoria.ValorUnitario = valorUnitario;
            recebimentoMercadoria.Produto = repProduto.BuscarPorCodigo(item.CodigoProduto);

            if (!string.IsNullOrWhiteSpace(recebimentoMercadoria.ChaveNFe))
                recebimentoMercadoria.XMLNotaFiscal = repXMLNotaFiscal.BuscarPorChave(recebimentoMercadoria.ChaveNFe);
            if (arquivo != null && recebimentoMercadoria.XMLNotaFiscal == null)
            {
                using (MemoryStream arquivoEmMemoria = new MemoryStream(arquivo))
                {
                    StreamReader leitorXML = new StreamReader(arquivoEmMemoria);
                    Servicos.Embarcador.NFe.NFe servicoNFe = new Servicos.Embarcador.NFe.NFe(unitOfWork);
                    servicoNFe.BuscarDadosNotaFiscal(out string erro,
                        out Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, leitorXML, unitOfWork,
                        null, true, false, false, null, false, false, null, null, null,  configuracaoWebservice?.CadastroAutomaticoPessoaExterior ?? false);

                    xmlNotaFiscal.Comprimento = recebimentoMercadoria.Comprimento;
                    xmlNotaFiscal.Largura = recebimentoMercadoria.Largura;
                    xmlNotaFiscal.Altura = recebimentoMercadoria.Altura;

                    if (xmlNotaFiscal.Volumes == 0 && recebimentoMercadoria.QuantidadeLote > 0)
                        xmlNotaFiscal.Volumes = (int)recebimentoMercadoria.QuantidadeLote;

                    if (xmlNotaFiscal.Codigo == 0)
                        repXMLNotaFiscal.Inserir(xmlNotaFiscal);
                    else
                        repXMLNotaFiscal.Atualizar(xmlNotaFiscal);

                    recebimentoMercadoria.XMLNotaFiscal = xmlNotaFiscal;
                }
            }

            if (item.CNPJRemetente > 0)
                recebimentoMercadoria.Remetente = repCliente.BuscarPorCPFCNPJ(item.CNPJRemetente);
            if (item.CNPJDestinatario > 0)
                recebimentoMercadoria.Destinatario = repCliente.BuscarPorCPFCNPJ(item.CNPJDestinatario);
            if (item.CodigoEmissora > 0)
                recebimentoMercadoria.EmpresaEmissora = repEmpresa.BuscarPorCodigo(item.CodigoEmissora);

            if (item.Codigo > 0)
                repRecebimentoMercadoria.Atualizar(recebimentoMercadoria);
            else
            {
                if (!repRecebimentoMercadoria.ContemRegistroDupliacdo(recebimento.Codigo,
                        recebimentoMercadoria.CodigoBarras, recebimentoMercadoria.Descricao))
                    repRecebimentoMercadoria.Inserir(recebimentoMercadoria, Auditado);
            }
        }

        private List<Dominio.Relatorios.Embarcador.DataSource.WMS.VolumeFaltante> RetornarDadosVolumesFaltantes(
            Repositorio.UnitOfWork unitOfWork, int codigoRecebimento)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.WMS.RecebimentoMercadoria repRecebimentoMercadoria =
                new Repositorio.Embarcador.WMS.RecebimentoMercadoria(unitOfWork);
            Repositorio.Embarcador.Logistica.RotaFreteCEP repRotaCEP =
                new Repositorio.Embarcador.Logistica.RotaFreteCEP(unitOfWork);
            List<Dominio.Relatorios.Embarcador.DataSource.WMS.VolumeFaltante> listaRetorno =
                new List<Dominio.Relatorios.Embarcador.DataSource.WMS.VolumeFaltante>();

            List<Dominio.Entidades.Embarcador.WMS.RecebimentoMercadoria> listaVolumes =
                repRecebimentoMercadoria.BuscarPorRecebimento(codigoRecebimento);
            if (listaVolumes != null)
            {
                foreach (var volume in listaVolumes)
                {
                    int codigoInterno = volume.Codigo;

                    int? codigoEmissora = volume.EmpresaEmissora?.Codigo;
                    int? codigoDepositoPosicao = volume.DepositoPosicao?.Codigo;
                    int? codigoProdutoEmbarcador = volume.ProdutoEmbarcador?.Codigo;

                    double? cnpjRemetente = volume.Remetente?.CPF_CNPJ;
                    double? cnpjDestinatario = volume.Destinatario?.CPF_CNPJ;

                    DateTime? dataEmissaoNF = volume.DataEmissaoNF;

                    string codigoBarras = volume.CodigoBarras;
                    string descricao = volume.Descricao;
                    string numeroLote = volume.NumeroLote;
                    string serie = volume.Serie;

                    decimal quantidadeLote = volume.QuantidadeLote;
                    decimal quantidadeConferida = volume.QuantidadeConferida;
                    decimal quantidadeFaltante = volume.QuantidadeFaltante;

                    Dominio.Entidades.Cliente destinatario = repCliente.BuscarPorCPFCNPJ(cnpjDestinatario.Value);
                    Dominio.Entidades.Cliente remetente = repCliente.BuscarPorCPFCNPJ(cnpjRemetente.Value);
                    Dominio.Entidades.Empresa empresaEmissora = repEmpresa.BuscarPorCodigo(codigoEmissora.Value);

                    string cep = Utilidades.String.OnlyNumbers(destinatario.CEP);
                    if (string.IsNullOrWhiteSpace(cep))
                        cep = "0";
                    Dominio.Entidades.Embarcador.Logistica.RotaFreteCEP rotaCEP =
                        repRotaCEP.BuscarPorCEP(int.Parse(cep));

                    for (int i = 0; i < quantidadeLote; i++)
                    {
                        string codigoBarrasFormatado = remetente.CPF_CNPJ_SemFormato.PadLeft(14, '0');
                        codigoBarrasFormatado += Utilidades.String.OnlyNumbers(numeroLote).PadLeft(9, '0');
                        codigoBarrasFormatado += Utilidades.String.OnlyNumbers((i + 1).ToString("n0")).PadLeft(4, '0');
                        codigoBarrasFormatado += Utilidades.String.OnlyNumbers(serie).PadLeft(2, '0');
                        codigoBarrasFormatado += "00";

                        if (!volume.CodigoBarras.Contains(codigoBarrasFormatado))
                        {
                            Dominio.Relatorios.Embarcador.DataSource.WMS.VolumeFaltante volumeFaltante =
                                new Dominio.Relatorios.Embarcador.DataSource.WMS.VolumeFaltante();
                            volumeFaltante.CEP = cep;
                            volumeFaltante.CidadeEstado = destinatario.Localidade.Descricao + " - " +
                                                          destinatario.Localidade.Estado.Sigla;
                            volumeFaltante.DataEmissaoNF = dataEmissaoNF.Value;
                            volumeFaltante.Destinatario = destinatario.Nome;
                            volumeFaltante.Endereco = destinatario.Endereco + ", " + destinatario.Bairro + ", " +
                                                      destinatario.Numero;
                            volumeFaltante.FilialDistribuidora =
                                rotaCEP != null ? rotaCEP.RotaFrete.FilialDistribuidora : string.Empty;
                            volumeFaltante.FilialEmissora = empresaEmissora?.CodigoIntegracao ?? "";
                            volumeFaltante.NumeroNota = Utilidades.String.OnlyNumbers(numeroLote);
                            volumeFaltante.NumeroPedido = descricao;
                            volumeFaltante.Remetente = remetente.Nome;
                            volumeFaltante.Rota = rotaCEP != null ? rotaCEP.RotaFrete.Descricao : string.Empty;
                            volumeFaltante.Volume = i + 1;
                            volumeFaltante.VolumeTotal = quantidadeLote;
                            volumeFaltante.Serie = Utilidades.String.OnlyNumbers(serie);
                            volumeFaltante.CodigoBarra = codigoBarrasFormatado;

                            listaRetorno.Add(volumeFaltante);
                        }
                    }
                }
            }


            return listaRetorno;
        }

        private List<Dominio.Relatorios.Embarcador.DataSource.WMS.VolumeFaltante>
            RetornarDadosVolumesFaltantesExpedicao(Repositorio.UnitOfWork unitOfWork, int codigoExpedicao)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe repCargaPedidoDocumentoCTe =
                new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe(unitOfWork);
            Repositorio.Embarcador.WMS.ConferenciaSeparacao repConferenciaSeparacao =
                new Repositorio.Embarcador.WMS.ConferenciaSeparacao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaControleExpedicao repCargaControleExpedicao =
                new Repositorio.Embarcador.Cargas.CargaControleExpedicao(unitOfWork);

            List<Dominio.Relatorios.Embarcador.DataSource.WMS.VolumeFaltante> listaRetorno =
                new List<Dominio.Relatorios.Embarcador.DataSource.WMS.VolumeFaltante>();
            Dominio.Entidades.Embarcador.Cargas.CargaControleExpedicao expedicao =
                repCargaControleExpedicao.BuscarPorCodigo(codigoExpedicao);
            if (expedicao.Carga != null)
            {
                List<Dominio.Entidades.Embarcador.WMS.ConferenciaSeparacao> conferencia =
                    repConferenciaSeparacao.BuscarPorCarga(expedicao.Carga.Codigo,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria.Volume);
                IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoSeparacaoVolume.RelacaoSeparacaoVolume>
                    dadosRelacaoSeparacaoVolume =
                        repCargaPedidoDocumentoCTe.RelatorioRelacaoSeparacaoVolume(expedicao.Carga.Codigo, "", "", "",
                            "");
                foreach (var volume in conferencia)
                {
                    if (volume.QuantidadeFaltante > 0)
                    {
                        foreach (var dadoVolume in dadosRelacaoSeparacaoVolume)
                        {
                            if (dadoVolume.NumeroRemessa == volume.Numero && dadoVolume.Volumes == volume.VolumeCarga)
                            {
                                Dominio.Relatorios.Embarcador.DataSource.WMS.VolumeFaltante volumeFaltante =
                                    new Dominio.Relatorios.Embarcador.DataSource.WMS.VolumeFaltante();
                                volumeFaltante.CEP = "";
                                volumeFaltante.CidadeEstado = "";
                                volumeFaltante.DataEmissaoNF = dadoVolume.DataCarga;
                                volumeFaltante.Destinatario = dadoVolume.Destinatario;
                                volumeFaltante.Endereco = "";
                                volumeFaltante.FilialDistribuidora = "";
                                volumeFaltante.FilialEmissora = "";
                                volumeFaltante.NumeroNota = Utilidades.String.OnlyNumbers(dadoVolume.NumeroNota);
                                volumeFaltante.NumeroPedido = dadoVolume.NumeroPedido;
                                volumeFaltante.Remetente = dadoVolume.Remetente;
                                volumeFaltante.Rota = "";
                                volumeFaltante.Volume = dadoVolume.Volumes;
                                volumeFaltante.VolumeTotal = dadoVolume.Volumes;
                                volumeFaltante.Serie = "";
                                volumeFaltante.CodigoBarra = volume.CodigoBarras;

                                listaRetorno.Add(volumeFaltante);
                            }
                        }
                    }
                }
            }

            return listaRetorno;
        }

        private void GerarVolumeFaltante(bool recebimentoVolume, string nomeEmpresa, string stringConexao,
            Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao,
            List<Dominio.Relatorios.Embarcador.DataSource.WMS.VolumeFaltante> dadosVolumeFaltante)
        {
            ReportRequest.WithType(ReportType.VolumeFaltante)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("recebimentoVolume", recebimentoVolume)
                .AddExtraData("nomeEmpresa", nomeEmpresa)
                .AddExtraData("dadosVolumeFaltante", dadosVolumeFaltante.ToJson())
                .AddExtraData("relatorioControleGeracao", relatorioControleGeracao.Codigo)
                .CallReport()
                .GetContentFile();
        }

        private int CadastrarProduto(string descricao, string codigoProduto, string ncm, int codigoEmpresa,
            Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            Dominio.Entidades.Produto produto = new Dominio.Entidades.Produto();
            produto.Status = "A";
            produto.Descricao = descricao;
            produto.CodigoNCM = ncm;
            produto.CodigoProduto = codigoProduto;
            produto.CodigoBarrasEAN = "";
            produto.UnidadeDeMedida = Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Unidade;
            produto.GrupoImposto = null;
            produto.Empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
            produto.DescricaoNotaFiscal = descricao;
            produto.CodigoCEST = null;
            produto.OrigemMercadoria = Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemMercadoria.Origem0;

            produto.UltimoCusto = 0;
            produto.CustoMedio = 0;
            produto.MargemLucro = 0;
            produto.ValorVenda = 0;
            produto.PesoBruto = 0;
            produto.PesoLiquido = 0;
            produto.ProdutoCombustivel = false;

            repProduto.Inserir(produto);

            Servicos.Embarcador.Produto.Estoque servicoEstoque = new Servicos.Embarcador.Produto.Estoque(unitOfWork);
            servicoEstoque.AdicionarEstoque(produto, produto.Empresa, TipoServicoMultisoftware, ConfiguracaoEmbarcador);

            return produto.Codigo;
        }

        private void ControlarFluxoPatio(Dominio.Entidades.Embarcador.WMS.Recebimento recebimento,
            Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracao servicoFluxoGestaoPatioConfiguracao =
                new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracao(unitOfWork);
            Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoFluxoPatio =
                servicoFluxoGestaoPatioConfiguracao.ObterConfiguracao();

            bool integrarFluxoPatioWMS = configuracaoFluxoPatio?.IntegrarFluxoPatioWMS ?? false;

            if (!integrarFluxoPatioWMS ||
                recebimento.SituacaoRecebimento !=
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRecebimento.Finalizada ||
                recebimento.Carga == null)
                return;

            Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioCargaJanelaDescarga =
                new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioGuarita =
                new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento janelaDescarga =
                repositorioCargaJanelaDescarga.BuscarPorCarga(recebimento.Carga.Codigo);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita =
                repositorioGuarita.BuscarPorCarga(recebimento.Carga.Codigo);

            if (janelaDescarga != null)
            {
                Servicos.Embarcador.Logistica.CargaJanelaDescarregamento servicoJanelaDescarga =
                    new Servicos.Embarcador.Logistica.CargaJanelaDescarregamento(unitOfWork, ConfiguracaoEmbarcador,
                        Auditado);
                servicoJanelaDescarga.AtualizarSituacao(janelaDescarga,
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaJanelaDescarregamento
                        .DescarregamentoFinalizado);
            }

            if (guarita != null)
            {
                guarita.DataSaidaGuarita = DateTime.Now;
                guarita.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaGuarita.SaidaLiberada;

                repositorioGuarita.Atualizar(guarita);
            }
        }

        #endregion
    }
}
