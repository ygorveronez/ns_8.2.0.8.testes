using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
namespace SGT.WebAdmin.Controllers.Cotacoes
{
    [CustomAuthorize("Cotacoes/RegraCotacao")]
    public class RegraCotacaoController : BaseController
    {
		#region Construtores

		public RegraCotacaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region ObjetosJson

        private class ObjetoEntidade
        {
            public dynamic Codigo { get; set; } // dynamic pois o codigo pode ser também um cpf/cnpj
            public string Descricao { get; set; }
        }

        private class ObjetoTransportador
        {
            public int Codigo { get; set; }
            public string Descricao { get; set; }
        }

        private class RegrasPorTipo
        {
            public dynamic Codigo { get; set; }
            public int Ordem { get; set; }
            public Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao Condicao { get; set; }
            public Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao Juncao { get; set; }
            public ObjetoEntidade Entidade { get; set; }
            public dynamic Valor { get; set; }
        }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Prioridade", "PrioridadeRegra", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Descrição", "Descricao", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo da Aplicação", "TipoAplicacao", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Vigência", "Vigencia", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Situação", "DescricaoAtivo", 10, Models.Grid.Align.center, false);


                DateTime dataInicioAux, dataFimAux;
                DateTime? dataInicio = null, dataFim = null;

                if (DateTime.TryParseExact(Request.Params("DataInicio"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicioAux))
                    dataInicio = dataInicioAux;

                if (DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFimAux))
                    dataFim = dataFimAux;


                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = !string.IsNullOrEmpty(Request.Params("Ativo")) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa)int.Parse(Request.Params("Ativo")) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;

                string descricao = !string.IsNullOrWhiteSpace(Request.Params("Descricao")) ? Request.Params("Descricao") : "";

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Cotacao.RegraCotacao repRegrasCotacao = new Repositorio.Embarcador.Cotacao.RegraCotacao(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cotacao.RegraCotacao> listaRegras = repRegrasCotacao.ConsultarRegras(dataInicio, dataFim, descricao, ativo, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repRegrasCotacao.ContarConsultaRegras(dataInicio, dataFim, descricao, ativo));

                var lista = (from obj in listaRegras
                             select new
                             {
                                 obj.Codigo,
                                 PrioridadeRegra = obj.PrioridadeRegra.HasValue ? obj.PrioridadeRegra.Value.ToString() : "",
                                 TipoAplicacao = obj.TipoAplicacao.ObterDescricao(),
                                 Descricao = !string.IsNullOrWhiteSpace(obj.Descricao) ? obj.Descricao : string.Empty,
                                 Vigencia = obj.Vigencia.HasValue ? obj.Vigencia.Value.ToString("dd/MM/yyyy") : string.Empty,
                                 obj.DescricaoAtivo
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                // Instancia Repositorios/Entidade
                Repositorio.Embarcador.Cotacao.RegraCotacao repRegrasCotacao = new Repositorio.Embarcador.Cotacao.RegraCotacao(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoCepDestino repRegraCotacaoCepDestino = new Repositorio.Embarcador.Cotacao.RegraCotacaoCepDestino(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoCubagem repCotacaoCubagem = new Repositorio.Embarcador.Cotacao.RegraCotacaoCubagem(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoDistancia repRegraCotacaoDistancia = new Repositorio.Embarcador.Cotacao.RegraCotacaoDistancia(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoEstadoDestino repRegrasCotacaoEstadoDestino = new Repositorio.Embarcador.Cotacao.RegraCotacaoEstadoDestino(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoExpedidor repRegrasCotacaoExpedidor = new Repositorio.Embarcador.Cotacao.RegraCotacaoExpedidor(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoDestinatario repRegrasCotacaoDestinatario = new Repositorio.Embarcador.Cotacao.RegraCotacaoDestinatario(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoGrupoProduto repRegrasCotacaoGrupoProduto = new Repositorio.Embarcador.Cotacao.RegraCotacaoGrupoProduto(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoLinhaSeparacao repRegrasCotacaoLinhaSeparacao = new Repositorio.Embarcador.Cotacao.RegraCotacaoLinhaSeparacao(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoMarcaProduto repRegrasCotacaoMarcaProduto = new Repositorio.Embarcador.Cotacao.RegraCotacaoMarcaProduto(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoPeso repRegrasCotacaoPeso = new Repositorio.Embarcador.Cotacao.RegraCotacaoPeso(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoProduto repRegrasCotacaoProduto = new Repositorio.Embarcador.Cotacao.RegraCotacaoProduto(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoTransportador repRegrasCotacaoTransportador = new Repositorio.Embarcador.Cotacao.RegraCotacaoTransportador(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoValorMercadoria repRegrasCotacaoValorMercadoria = new Repositorio.Embarcador.Cotacao.RegraCotacaoValorMercadoria(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoVolume repRegrasCotacaoVolume = new Repositorio.Embarcador.Cotacao.RegraCotacaoVolume(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoArestaProduto repRegrasCotacaoArestaProduto = new Repositorio.Embarcador.Cotacao.RegraCotacaoArestaProduto(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoValorCotacao repRegraCotacaoValorCotacao = new Repositorio.Embarcador.Cotacao.RegraCotacaoValorCotacao(unitOfWork);


                // Nova entidade
                Dominio.Entidades.Embarcador.Cotacao.RegraCotacao regrasCotacao = new Dominio.Entidades.Embarcador.Cotacao.RegraCotacao();
                List<Dominio.Entidades.Embarcador.Cotacao.RegrasCepDestino> regraCepDestino = new List<Dominio.Entidades.Embarcador.Cotacao.RegrasCepDestino>();
                List<Dominio.Entidades.Embarcador.Cotacao.RegrasCubagem> regraCubagem = new List<Dominio.Entidades.Embarcador.Cotacao.RegrasCubagem>();
                List<Dominio.Entidades.Embarcador.Cotacao.RegrasDistancia> regraDistancia = new List<Dominio.Entidades.Embarcador.Cotacao.RegrasDistancia>();
                List<Dominio.Entidades.Embarcador.Cotacao.RegrasEstadoDestino> regraEstadoDestino = new List<Dominio.Entidades.Embarcador.Cotacao.RegrasEstadoDestino>();
                List<Dominio.Entidades.Embarcador.Cotacao.RegrasExpedidor> regraExpedidor = new List<Dominio.Entidades.Embarcador.Cotacao.RegrasExpedidor>();
                List<Dominio.Entidades.Embarcador.Cotacao.RegrasDestinatario> regraDestinatario = new List<Dominio.Entidades.Embarcador.Cotacao.RegrasDestinatario>();
                List<Dominio.Entidades.Embarcador.Cotacao.RegrasGrupoProduto> regraGrupoProduto = new List<Dominio.Entidades.Embarcador.Cotacao.RegrasGrupoProduto>();
                List<Dominio.Entidades.Embarcador.Cotacao.RegrasLinhaSeparacao> regraLinhaSeparacao = new List<Dominio.Entidades.Embarcador.Cotacao.RegrasLinhaSeparacao>();
                List<Dominio.Entidades.Embarcador.Cotacao.RegrasMarcaProduto> regraMarcaProduto = new List<Dominio.Entidades.Embarcador.Cotacao.RegrasMarcaProduto>();
                List<Dominio.Entidades.Embarcador.Cotacao.RegrasPeso> regraPeso = new List<Dominio.Entidades.Embarcador.Cotacao.RegrasPeso>();
                List<Dominio.Entidades.Embarcador.Cotacao.RegrasProduto> regraProduto = new List<Dominio.Entidades.Embarcador.Cotacao.RegrasProduto>();
                List<Dominio.Entidades.Embarcador.Cotacao.RegrasCotacaoTransportador> regraTransportador = new List<Dominio.Entidades.Embarcador.Cotacao.RegrasCotacaoTransportador>();
                List<Dominio.Entidades.Embarcador.Cotacao.RegrasValorMercadoria> regraValorMercadoria = new List<Dominio.Entidades.Embarcador.Cotacao.RegrasValorMercadoria>();
                List<Dominio.Entidades.Embarcador.Cotacao.RegrasVolume> regraVolume = new List<Dominio.Entidades.Embarcador.Cotacao.RegrasVolume>();
                List<Dominio.Entidades.Embarcador.Cotacao.RegrasArestaProduto> regraArestaProduto = new List<Dominio.Entidades.Embarcador.Cotacao.RegrasArestaProduto>();
                List<Dominio.Entidades.Embarcador.Cotacao.RegrasValorCotacao> regraValorCotacao = new List<Dominio.Entidades.Embarcador.Cotacao.RegrasValorCotacao>();

                // Preenche a entidade
                PreencherEntidade(ref regrasCotacao, unitOfWork);

                List<string> erros = new List<string>();
                // Validar entidade
                if (!ValidarEntidade(regrasCotacao, out erros))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, String.Join("<br>", erros));
                }

                #region Regras
                List<string> errosRegras = new List<string>();

                #region CepDestino

                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasCotacao.RegraPorCEPDestino)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("CepDestino", "RegrasCepDestino", true, ref regraCepDestino, ref regrasCotacao);
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add("Cep de Destino");
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra("Cep de Destino", "CepDestino", regraCepDestino, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion

                #region Cubagem
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasCotacao.RegraPorCubagem)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("Cubagem", "RegrasCubagem", true, ref regraCubagem, ref regrasCotacao);
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add("Cubagem");
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra("Cubagem", "Cubagem", regraCubagem, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }

                #endregion

                #region Distancia
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasCotacao.RegraPorDistancia)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("Distancia", "RegrasDistancia", true, ref regraDistancia, ref regrasCotacao);
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add("Distancia");
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra("Distancia", "Distancia", regraDistancia, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }

                #endregion 

                #region Estado Destino
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasCotacao.RegraPorEstadoDestino)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("Estado", "RegrasEstadoDestino", false, ref regraEstadoDestino, ref regrasCotacao, ((codigo) =>
                        {
                            Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);

                            return repEstado.BuscarPorSigla(codigo.ToString());
                        }));
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add("Estado de Destino");
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra("Estado de Destino", "Estado", regraEstadoDestino, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion

                #region Expedidor
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasCotacao.RegraPorExpedidor)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("Expedidor", "RegrasExpedidor", false, ref regraExpedidor, ref regrasCotacao, ((codigo) =>
                        {
                            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                            double codigoDouble = 0;
                            double.TryParse(codigo.ToString(), out codigoDouble);

                            return repCliente.BuscarPorCPFCNPJ(codigoDouble);
                        }));
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add("Expedidor");
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra("Expedidor", "Expedidor", regraExpedidor, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion

                #region Destinatario
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasCotacao.RegraPorDestinatario)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("Destinatario", "RegrasDestinatario", false, ref regraDestinatario, ref regrasCotacao, ((codigo) =>
                        {
                            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                            double codigoDouble = 0;
                            double.TryParse(codigo.ToString(), out codigoDouble);

                            return repCliente.BuscarPorCPFCNPJ(codigoDouble);
                        }));
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add("Destinatario");
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra("Destinatario", "Destinatario", regraDestinatario, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion

                #region GrupoProduto
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasCotacao.RegraPorGrupoProduto)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("GrupoProduto", "RegrasGrupoProduto", false, ref regraGrupoProduto, ref regrasCotacao, ((codigo) =>
                        {
                            Repositorio.Embarcador.Produtos.GrupoProduto repgrupoProduto = new Repositorio.Embarcador.Produtos.GrupoProduto(unitOfWork);

                            int codigoInt = 0;
                            int.TryParse(codigo.ToString(), out codigoInt);

                            return repgrupoProduto.BuscarPorCodigo(codigoInt, false);
                        }));
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add("Grupo Produto");
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra("Grupo Produto", "GrupoProduto", regraGrupoProduto, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion

                #region Linha Separacao
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasCotacao.RegraPorLinhaSeparacao)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("LinhaSeparacao", "RegrasLinhaSeparacao", false, ref regraLinhaSeparacao, ref regrasCotacao, ((codigo) =>
                        {
                            Repositorio.Embarcador.Pedidos.LinhaSeparacao repLinhaSeparacao = new Repositorio.Embarcador.Pedidos.LinhaSeparacao(unitOfWork);

                            int codigoInt = 0;
                            int.TryParse(codigo.ToString(), out codigoInt);

                            return repLinhaSeparacao.BuscarPorCodigo(codigoInt);
                        }));
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add("Linha de Separacao");
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra("Linha de Separacao", "LinhaSeparacao", regraLinhaSeparacao, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion

                #region MarcaProduto
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasCotacao.RegraPorMarcaProduto)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("MarcaProduto", "RegrasMarcaProduto", false, ref regraMarcaProduto, ref regrasCotacao, ((codigo) =>
                        {
                            Repositorio.Embarcador.Produtos.MarcaProduto repMarcaProduto = new Repositorio.Embarcador.Produtos.MarcaProduto(unitOfWork);

                            int codigoInt = 0;
                            int.TryParse(codigo.ToString(), out codigoInt);

                            return repMarcaProduto.BuscarPorCodigo(codigoInt, false);
                        }));
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add("Marca Produto");
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra("Marca Produto", "MarcaProduto", regraMarcaProduto, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion

                #region Peso
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasCotacao.RegraPorPeso)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("Peso", "RegrasPeso", true, ref regraPeso, ref regrasCotacao);
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add("Peso");
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra("Peso", "Peso", regraPeso, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion

                #region Produto
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasCotacao.RegraPorProduto)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("Produto", "RegrasProduto", false, ref regraProduto, ref regrasCotacao, ((codigo) =>
                        {
                            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProduto = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);

                            int codigoInt = 0;
                            int.TryParse(codigo.ToString(), out codigoInt);

                            return repProduto.BuscarPorCodigo(codigoInt);
                        }));
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add("Produto");
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra("Produto", "Produto", regraProduto, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion

                #region Transportador
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasCotacao.RegraPorTransportador)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("Transportador", "RegrasTransportador", false, ref regraTransportador, ref regrasCotacao, ((codigo) =>
                        {
                            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                            int codigoInt = 0;
                            int.TryParse(codigo.ToString(), out codigoInt);

                            return repEmpresa.BuscarPorCodigo(codigoInt);
                        }));
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add("Transportador");
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra("Transportador", "Transportador", regraTransportador, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion

                #region ValorMercadoria
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasCotacao.RegraPorValorMercadoria)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("Valor", "RegrasValorMercadoria", true, ref regraValorMercadoria, ref regrasCotacao);
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add("Valor Mercadoria");
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra("Valor Mercadoria", "Valor", regraValorMercadoria, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion

                #region Volume
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasCotacao.RegraPorVolume)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("Volume", "RegrasVolume", true, ref regraVolume, ref regrasCotacao);
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add("Volume");
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra("Volume", "Volume", regraVolume, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion

                #region ArestaProduto
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasCotacao.RegraPorArestaProduto)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("ArestaProduto", "RegrasArestaProduto", true, ref regraArestaProduto, ref regrasCotacao);
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add("Aresta Produto");
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra("Aresta Produto", "ArestaProduto", regraArestaProduto, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }

                #endregion

                #region Valor da Cotação
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasCotacao.RegraPorValorCotacao)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("Valor", "RegrasValorCotacao", true, ref regraValorCotacao, ref regrasCotacao);
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add("Valor da Cotação");
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra("Valor da Cotação", "Valor", regraValorCotacao, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }

                #endregion

                #endregion

                //Insere Entidade
                repRegrasCotacao.Inserir(regrasCotacao, Auditado);

                //Insere regras
                for (var i = 0; i < regraCepDestino.Count(); i++) repRegraCotacaoCepDestino.Inserir(regraCepDestino[i]);
                for (var i = 0; i < regraCubagem.Count(); i++) repCotacaoCubagem.Inserir(regraCubagem[i]);
                for (var i = 0; i < regraDistancia.Count(); i++) repRegraCotacaoDistancia.Inserir(regraDistancia[i]);
                for (var i = 0; i < regraEstadoDestino.Count(); i++) repRegrasCotacaoEstadoDestino.Inserir(regraEstadoDestino[i]);
                for (var i = 0; i < regraExpedidor.Count(); i++) repRegrasCotacaoExpedidor.Inserir(regraExpedidor[i]);
                for (var i = 0; i < regraDestinatario.Count(); i++) repRegrasCotacaoDestinatario.Inserir(regraDestinatario[i]);
                for (var i = 0; i < regraGrupoProduto.Count(); i++) repRegrasCotacaoGrupoProduto.Inserir(regraGrupoProduto[i]);
                for (var i = 0; i < regraLinhaSeparacao.Count(); i++) repRegrasCotacaoLinhaSeparacao.Inserir(regraLinhaSeparacao[i]);
                for (var i = 0; i < regraMarcaProduto.Count(); i++) repRegrasCotacaoMarcaProduto.Inserir(regraMarcaProduto[i]);
                for (var i = 0; i < regraPeso.Count(); i++) repRegrasCotacaoPeso.Inserir(regraPeso[i]);
                for (var i = 0; i < regraProduto.Count(); i++) repRegrasCotacaoProduto.Inserir(regraProduto[i]);
                for (var i = 0; i < regraTransportador.Count(); i++) repRegrasCotacaoTransportador.Inserir(regraTransportador[i]);
                for (var i = 0; i < regraValorMercadoria.Count(); i++) repRegrasCotacaoValorMercadoria.Inserir(regraValorMercadoria[i]);
                for (var i = 0; i < regraVolume.Count(); i++) repRegrasCotacaoVolume.Inserir(regraVolume[i]);
                for (var i = 0; i < regraArestaProduto.Count(); i++) repRegrasCotacaoArestaProduto.Inserir(regraArestaProduto[i]);
                for (var i = 0; i < regraValorCotacao.Count(); i++) repRegraCotacaoValorCotacao.Inserir(regraValorCotacao[i]);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
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

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                // Instancia Repositorios/Entidade
                Repositorio.Embarcador.Cotacao.RegraCotacao repRegrasCotacao = new Repositorio.Embarcador.Cotacao.RegraCotacao(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoCepDestino repRegraCotacaoCepDestino = new Repositorio.Embarcador.Cotacao.RegraCotacaoCepDestino(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoCubagem repRegrasCotacaoCubagem = new Repositorio.Embarcador.Cotacao.RegraCotacaoCubagem(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoDistancia repRegraCotacaoDistancia = new Repositorio.Embarcador.Cotacao.RegraCotacaoDistancia(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoEstadoDestino repRegrasCotacaoEstadoDestino = new Repositorio.Embarcador.Cotacao.RegraCotacaoEstadoDestino(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoExpedidor repRegrasCotacaoExpedidor = new Repositorio.Embarcador.Cotacao.RegraCotacaoExpedidor(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoDestinatario repRegrasCotacaoDestinatario = new Repositorio.Embarcador.Cotacao.RegraCotacaoDestinatario(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoGrupoProduto repRegrasCotacaoGrupoProduto = new Repositorio.Embarcador.Cotacao.RegraCotacaoGrupoProduto(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoLinhaSeparacao repRegrasCotacaoLinhaSeparacao = new Repositorio.Embarcador.Cotacao.RegraCotacaoLinhaSeparacao(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoMarcaProduto repRegrasCotacaoMarcaProduto = new Repositorio.Embarcador.Cotacao.RegraCotacaoMarcaProduto(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoPeso repRegrasCotacaoPeso = new Repositorio.Embarcador.Cotacao.RegraCotacaoPeso(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoProduto repRegrasCotacaoProduto = new Repositorio.Embarcador.Cotacao.RegraCotacaoProduto(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoTransportador repRegrasCotacaoTransportador = new Repositorio.Embarcador.Cotacao.RegraCotacaoTransportador(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoValorMercadoria repRegrasCotacaoValorMercadoria = new Repositorio.Embarcador.Cotacao.RegraCotacaoValorMercadoria(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoVolume repRegrasCotacaoVolume = new Repositorio.Embarcador.Cotacao.RegraCotacaoVolume(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoArestaProduto repRegrasCotacaoArestaProduto = new Repositorio.Embarcador.Cotacao.RegraCotacaoArestaProduto(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoValorCotacao repRegrasCotacaoValorCotacao = new Repositorio.Embarcador.Cotacao.RegraCotacaoValorCotacao(unitOfWork);


                // Nova entidade
                // Codigo da busca 
                int codigoRegra = 0;
                int.TryParse(Request.Params("Codigo"), out codigoRegra);

                // Busca entidade
                Dominio.Entidades.Embarcador.Cotacao.RegraCotacao regrasCotacao = repRegrasCotacao.BuscarPorCodigo(codigoRegra, true);

                if (regrasCotacao == null)
                    return new JsonpResult(false, "Não foi possível buscar a regra.");

                #region BuscaRegras

                List<Dominio.Entidades.Embarcador.Cotacao.RegrasCepDestino> regraCepDestino = repRegraCotacaoCepDestino.BuscarPorRegras(regrasCotacao.Codigo);
                List<Dominio.Entidades.Embarcador.Cotacao.RegrasCubagem> regraCubagem = repRegrasCotacaoCubagem.BuscarPorRegras(regrasCotacao.Codigo);
                List<Dominio.Entidades.Embarcador.Cotacao.RegrasDistancia> regraDistancia = repRegraCotacaoDistancia.BuscarPorRegras(regrasCotacao.Codigo);
                List<Dominio.Entidades.Embarcador.Cotacao.RegrasEstadoDestino> regraEstadoDestino = repRegrasCotacaoEstadoDestino.BuscarPorRegras(regrasCotacao.Codigo);
                List<Dominio.Entidades.Embarcador.Cotacao.RegrasExpedidor> regraExpedidor = repRegrasCotacaoExpedidor.BuscarPorRegras(regrasCotacao.Codigo);
                List<Dominio.Entidades.Embarcador.Cotacao.RegrasDestinatario> regraDestinatario = repRegrasCotacaoDestinatario.BuscarPorRegras(regrasCotacao.Codigo);
                List<Dominio.Entidades.Embarcador.Cotacao.RegrasGrupoProduto> regraGrupoProduto = repRegrasCotacaoGrupoProduto.BuscarPorRegras(regrasCotacao.Codigo);
                List<Dominio.Entidades.Embarcador.Cotacao.RegrasLinhaSeparacao> regraLinhaSeparacao = repRegrasCotacaoLinhaSeparacao.BuscarPorRegras(regrasCotacao.Codigo);
                List<Dominio.Entidades.Embarcador.Cotacao.RegrasMarcaProduto> regraMarcaProduto = repRegrasCotacaoMarcaProduto.BuscarPorRegras(regrasCotacao.Codigo);
                List<Dominio.Entidades.Embarcador.Cotacao.RegrasPeso> regraPeso = repRegrasCotacaoPeso.BuscarPorRegras(regrasCotacao.Codigo);
                List<Dominio.Entidades.Embarcador.Cotacao.RegrasProduto> regraProduto = repRegrasCotacaoProduto.BuscarPorRegras(regrasCotacao.Codigo);
                List<Dominio.Entidades.Embarcador.Cotacao.RegrasCotacaoTransportador> regraTransportador = repRegrasCotacaoTransportador.BuscarPorRegras(regrasCotacao.Codigo);
                List<Dominio.Entidades.Embarcador.Cotacao.RegrasValorMercadoria> regraValorMercadoria = repRegrasCotacaoValorMercadoria.BuscarPorRegras(regrasCotacao.Codigo);
                List<Dominio.Entidades.Embarcador.Cotacao.RegrasVolume> regraVolume = repRegrasCotacaoVolume.BuscarPorRegras(regrasCotacao.Codigo);
                List<Dominio.Entidades.Embarcador.Cotacao.RegrasArestaProduto> regraArestaProduto = repRegrasCotacaoArestaProduto.BuscarPorRegras(regrasCotacao.Codigo);
                List<Dominio.Entidades.Embarcador.Cotacao.RegrasValorCotacao> regraValorCotacao = repRegrasCotacaoValorCotacao.BuscarPorRegras(regrasCotacao.Codigo);

                #endregion

                #region DeletaRegras

                //Insere regras
                for (var i = 0; i < regraCepDestino.Count(); i++) repRegraCotacaoCepDestino.Deletar(regraCepDestino[i]);
                for (var i = 0; i < regraCubagem.Count(); i++) repRegrasCotacaoCubagem.Deletar(regraCubagem[i]);
                for (var i = 0; i < regraDistancia.Count(); i++) repRegraCotacaoDistancia.Deletar(regraDistancia[i]);
                for (var i = 0; i < regraEstadoDestino.Count(); i++) repRegrasCotacaoEstadoDestino.Deletar(regraEstadoDestino[i]);
                for (var i = 0; i < regraExpedidor.Count(); i++) repRegrasCotacaoExpedidor.Deletar(regraExpedidor[i]);
                for (var i = 0; i < regraDestinatario.Count(); i++) repRegrasCotacaoDestinatario.Deletar(regraDestinatario[i]);
                for (var i = 0; i < regraGrupoProduto.Count(); i++) repRegrasCotacaoGrupoProduto.Deletar(regraGrupoProduto[i]);
                for (var i = 0; i < regraLinhaSeparacao.Count(); i++) repRegrasCotacaoLinhaSeparacao.Deletar(regraLinhaSeparacao[i]);
                for (var i = 0; i < regraMarcaProduto.Count(); i++) repRegrasCotacaoMarcaProduto.Deletar(regraMarcaProduto[i]);
                for (var i = 0; i < regraPeso.Count(); i++) repRegrasCotacaoPeso.Deletar(regraPeso[i]);
                for (var i = 0; i < regraProduto.Count(); i++) repRegrasCotacaoProduto.Deletar(regraProduto[i]);
                for (var i = 0; i < regraTransportador.Count(); i++) repRegrasCotacaoTransportador.Deletar(regraTransportador[i]);
                for (var i = 0; i < regraValorMercadoria.Count(); i++) repRegrasCotacaoValorMercadoria.Deletar(regraValorMercadoria[i]);
                for (var i = 0; i < regraVolume.Count(); i++) repRegrasCotacaoVolume.Deletar(regraVolume[i]);
                for (var i = 0; i < regraArestaProduto.Count(); i++) repRegrasCotacaoArestaProduto.Deletar(regraArestaProduto[i]);
                for (var i = 0; i < regraValorCotacao.Count(); i++) repRegrasCotacaoValorCotacao.Deletar(regraValorCotacao[i]);

                #endregion

                #region NovasRegras

                regraCepDestino = new List<Dominio.Entidades.Embarcador.Cotacao.RegrasCepDestino>();
                regraCubagem = new List<Dominio.Entidades.Embarcador.Cotacao.RegrasCubagem>();
                regraDistancia = new List<Dominio.Entidades.Embarcador.Cotacao.RegrasDistancia>();
                regraEstadoDestino = new List<Dominio.Entidades.Embarcador.Cotacao.RegrasEstadoDestino>();
                regraExpedidor = new List<Dominio.Entidades.Embarcador.Cotacao.RegrasExpedidor>();
                regraDestinatario = new List<Dominio.Entidades.Embarcador.Cotacao.RegrasDestinatario>();
                regraGrupoProduto = new List<Dominio.Entidades.Embarcador.Cotacao.RegrasGrupoProduto>();
                regraLinhaSeparacao = new List<Dominio.Entidades.Embarcador.Cotacao.RegrasLinhaSeparacao>();
                regraMarcaProduto = new List<Dominio.Entidades.Embarcador.Cotacao.RegrasMarcaProduto>();
                regraPeso = new List<Dominio.Entidades.Embarcador.Cotacao.RegrasPeso>();
                regraProduto = new List<Dominio.Entidades.Embarcador.Cotacao.RegrasProduto>();
                regraTransportador = new List<Dominio.Entidades.Embarcador.Cotacao.RegrasCotacaoTransportador>();
                regraValorMercadoria = new List<Dominio.Entidades.Embarcador.Cotacao.RegrasValorMercadoria>();
                regraVolume = new List<Dominio.Entidades.Embarcador.Cotacao.RegrasVolume>();
                regraArestaProduto = new List<Dominio.Entidades.Embarcador.Cotacao.RegrasArestaProduto>();
                regraValorCotacao = new List<Dominio.Entidades.Embarcador.Cotacao.RegrasValorCotacao>();

                #endregion

                // Preenche a entidade
                PreencherEntidade(ref regrasCotacao, unitOfWork);

                List<string> erros = new List<string>();
                // Validar entidade
                if (!ValidarEntidade(regrasCotacao, out erros))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, String.Join("<br>", erros));
                }

                // Atualiza Entidade
                repRegrasCotacao.Atualizar(regrasCotacao, Auditado);

                #region Regras

                List<string> errosRegras = new List<string>();

                #region CepDestino

                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasCotacao.RegraPorCEPDestino)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("CepDestino", "RegrasCepDestino", true, ref regraCepDestino, ref regrasCotacao);
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add("Cep de Destino");
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra("Cep de Destino", "CepDestino", regraCepDestino, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion

                #region Cubagem
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasCotacao.RegraPorCubagem)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("Cubagem", "RegrasCubagem", true, ref regraCubagem, ref regrasCotacao);
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add("Cubagem");
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra("Cubagem", "Cubagem", regraCubagem, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }

                #endregion

                #region Distancia
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasCotacao.RegraPorDistancia)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("Distancia", "RegrasDistancia", true, ref regraDistancia, ref regrasCotacao);
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add("Distancia");
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra("Distancia", "Distancia", regraDistancia, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }

                #endregion 

                #region Estado Destino
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasCotacao.RegraPorEstadoDestino)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("Estado", "RegrasEstadoDestino", false, ref regraEstadoDestino, ref regrasCotacao, ((codigo) =>
                        {
                            Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);

                            return repEstado.BuscarPorSigla(codigo.ToString());
                        }));
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add("Estado de Destino");
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra("Estado de Destino", "Estado", regraEstadoDestino, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion

                #region Expedidor
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasCotacao.RegraPorExpedidor)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("Expedidor", "RegrasExpedidor", false, ref regraExpedidor, ref regrasCotacao, ((codigo) =>
                        {
                            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                            double codigoDouble = 0;
                            double.TryParse(codigo.ToString(), out codigoDouble);

                            return repCliente.BuscarPorCPFCNPJ(codigoDouble);
                        }));
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add("Expedidor");
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra("Expedidor", "Expedidor", regraExpedidor, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion


                #region Destinatario
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasCotacao.RegraPorDestinatario)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("Destinatario", "RegrasDestinatario", false, ref regraDestinatario, ref regrasCotacao, ((codigo) =>
                        {
                            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                            double codigoDouble = 0;
                            double.TryParse(codigo.ToString(), out codigoDouble);

                            return repCliente.BuscarPorCPFCNPJ(codigoDouble);
                        }));
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add("Destinatario");
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra("Destinatario", "Destinatario", regraDestinatario, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion

                #region GrupoProduto
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasCotacao.RegraPorGrupoProduto)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("GrupoProduto", "RegrasGrupoProduto", false, ref regraGrupoProduto, ref regrasCotacao, ((codigo) =>
                        {
                            Repositorio.Embarcador.Produtos.GrupoProduto repgrupoProduto = new Repositorio.Embarcador.Produtos.GrupoProduto(unitOfWork);

                            int codigoInt = 0;
                            int.TryParse(codigo.ToString(), out codigoInt);

                            return repgrupoProduto.BuscarPorCodigo(codigoInt, false);
                        }));
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add("Grupo Produto");
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra("Grupo Produto", "GrupoProduto", regraGrupoProduto, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion

                #region Linha Separacao
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasCotacao.RegraPorLinhaSeparacao)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("LinhaSeparacao", "RegrasLinhaSeparacao", false, ref regraLinhaSeparacao, ref regrasCotacao, ((codigo) =>
                        {
                            Repositorio.Embarcador.Pedidos.LinhaSeparacao repLinhaSeparacao = new Repositorio.Embarcador.Pedidos.LinhaSeparacao(unitOfWork);

                            int codigoInt = 0;
                            int.TryParse(codigo.ToString(), out codigoInt);

                            return repLinhaSeparacao.BuscarPorCodigo(codigoInt);
                        }));
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add("Linha de Separacao");
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra("Linha de Separacao", "LinhaSeparacao", regraLinhaSeparacao, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion

                #region MarcaProduto
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasCotacao.RegraPorMarcaProduto)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("MarcaProduto", "RegrasMarcaProduto", false, ref regraMarcaProduto, ref regrasCotacao, ((codigo) =>
                        {
                            Repositorio.Embarcador.Produtos.MarcaProduto repMarcaProduto = new Repositorio.Embarcador.Produtos.MarcaProduto(unitOfWork);

                            int codigoInt = 0;
                            int.TryParse(codigo.ToString(), out codigoInt);

                            return repMarcaProduto.BuscarPorCodigo(codigoInt, false);
                        }));
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add("Marca Produto");
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra("Marca Produto", "MarcaProduto", regraMarcaProduto, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion

                #region Peso
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasCotacao.RegraPorPeso)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("Peso", "RegrasPeso", true, ref regraPeso, ref regrasCotacao);
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add("Peso");
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra("Peso", "Peso", regraPeso, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion

                #region Produto
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasCotacao.RegraPorProduto)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("Produto", "RegrasProduto", false, ref regraProduto, ref regrasCotacao, ((codigo) =>
                        {
                            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProduto = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);

                            int codigoInt = 0;
                            int.TryParse(codigo.ToString(), out codigoInt);

                            return repProduto.BuscarPorCodigo(codigoInt);
                        }));
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add("Produto");
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra("Produto", "Produto", regraProduto, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion

                #region Transportador
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasCotacao.RegraPorTransportador)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("Transportador", "RegrasTransportador", false, ref regraTransportador, ref regrasCotacao, ((codigo) =>
                        {
                            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                            int codigoInt = 0;
                            int.TryParse(codigo.ToString(), out codigoInt);

                            return repEmpresa.BuscarPorCodigo(codigoInt);
                        }));
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add("Transportador");
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra("Transportador", "Transportador", regraTransportador, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion

                #region ValorMercadoria
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasCotacao.RegraPorValorMercadoria)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("Valor", "RegrasValorMercadoria", true, ref regraValorMercadoria, ref regrasCotacao);
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add("Valor Mercadoria");
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra("Valor Mercadoria", "Valor", regraValorMercadoria, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion

                #region Volume
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasCotacao.RegraPorVolume)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("Volume", "RegrasVolume", true, ref regraVolume, ref regrasCotacao);
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add("Volume");
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra("Volume", "Volume", regraVolume, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion

                #region ArestaProduto
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasCotacao.RegraPorArestaProduto)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("ArestaProduto", "RegrasArestaProduto", true, ref regraArestaProduto, ref regrasCotacao);
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add("Aresta Produto");
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra("Aresta Produto", "ArestaProduto", regraArestaProduto, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }

                #endregion

                #region Valor da Cotação
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasCotacao.RegraPorValorCotacao)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("Valor", "RegrasValorCotacao", true, ref regraValorCotacao, ref regrasCotacao);
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add("Valor da Cotação");
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra("Valor da Cotação", "Valor", regraValorCotacao, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }

                #endregion

                #endregion

                // Insere regras
                for (var i = 0; i < regraCepDestino.Count(); i++) repRegraCotacaoCepDestino.Inserir(regraCepDestino[i]);
                for (var i = 0; i < regraCubagem.Count(); i++) repRegrasCotacaoCubagem.Inserir(regraCubagem[i]);
                for (var i = 0; i < regraDistancia.Count(); i++) repRegraCotacaoDistancia.Inserir(regraDistancia[i]);
                for (var i = 0; i < regraEstadoDestino.Count(); i++) repRegrasCotacaoEstadoDestino.Inserir(regraEstadoDestino[i]);
                for (var i = 0; i < regraExpedidor.Count(); i++) repRegrasCotacaoExpedidor.Inserir(regraExpedidor[i]);
                for (var i = 0; i < regraDestinatario.Count(); i++) repRegrasCotacaoDestinatario.Inserir(regraDestinatario[i]);
                for (var i = 0; i < regraGrupoProduto.Count(); i++) repRegrasCotacaoGrupoProduto.Inserir(regraGrupoProduto[i]);
                for (var i = 0; i < regraLinhaSeparacao.Count(); i++) repRegrasCotacaoLinhaSeparacao.Inserir(regraLinhaSeparacao[i]);
                for (var i = 0; i < regraMarcaProduto.Count(); i++) repRegrasCotacaoMarcaProduto.Inserir(regraMarcaProduto[i]);
                for (var i = 0; i < regraPeso.Count(); i++) repRegrasCotacaoPeso.Inserir(regraPeso[i]);
                for (var i = 0; i < regraProduto.Count(); i++) repRegrasCotacaoProduto.Inserir(regraProduto[i]);
                for (var i = 0; i < regraTransportador.Count(); i++) repRegrasCotacaoTransportador.Inserir(regraTransportador[i]);
                for (var i = 0; i < regraValorMercadoria.Count(); i++) repRegrasCotacaoValorMercadoria.Inserir(regraValorMercadoria[i]);
                for (var i = 0; i < regraVolume.Count(); i++) repRegrasCotacaoVolume.Inserir(regraVolume[i]);
                for (var i = 0; i < regraArestaProduto.Count(); i++) repRegrasCotacaoArestaProduto.Inserir(regraArestaProduto[i]);
                for (var i = 0; i < regraValorCotacao.Count(); i++) repRegrasCotacaoValorCotacao.Inserir(regraValorCotacao[i]);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
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

                // Instancia Repositorios/Entidade
                Repositorio.Embarcador.Cotacao.RegraCotacao repRegrasCotacao = new Repositorio.Embarcador.Cotacao.RegraCotacao(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoCepDestino repRegraCotacaoCepDestino = new Repositorio.Embarcador.Cotacao.RegraCotacaoCepDestino(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoCubagem repRegrasCotacaoCubagem = new Repositorio.Embarcador.Cotacao.RegraCotacaoCubagem(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoDistancia repRegraCotacaoDistancia = new Repositorio.Embarcador.Cotacao.RegraCotacaoDistancia(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoEstadoDestino repRegrasCotacaoEstadoDestino = new Repositorio.Embarcador.Cotacao.RegraCotacaoEstadoDestino(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoExpedidor repRegrasCotacaoExpedidor = new Repositorio.Embarcador.Cotacao.RegraCotacaoExpedidor(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoDestinatario repRegrasCotacaoDestinatario = new Repositorio.Embarcador.Cotacao.RegraCotacaoDestinatario(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoGrupoProduto repRegrasCotacaoGrupoProduto = new Repositorio.Embarcador.Cotacao.RegraCotacaoGrupoProduto(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoLinhaSeparacao repRegrasCotacaoLinhaSeparacao = new Repositorio.Embarcador.Cotacao.RegraCotacaoLinhaSeparacao(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoMarcaProduto repRegrasCotacaoMarcaProduto = new Repositorio.Embarcador.Cotacao.RegraCotacaoMarcaProduto(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoPeso repRegrasCotacaoPeso = new Repositorio.Embarcador.Cotacao.RegraCotacaoPeso(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoProduto repRegrasCotacaoProduto = new Repositorio.Embarcador.Cotacao.RegraCotacaoProduto(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoTransportador repRegrasCotacaoTransportador = new Repositorio.Embarcador.Cotacao.RegraCotacaoTransportador(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoValorMercadoria repRegrasCotacaoValorMercadoria = new Repositorio.Embarcador.Cotacao.RegraCotacaoValorMercadoria(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoVolume repRegrasCotacaoVolume = new Repositorio.Embarcador.Cotacao.RegraCotacaoVolume(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoArestaProduto repRegrasArestaProduto = new Repositorio.Embarcador.Cotacao.RegraCotacaoArestaProduto(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoValorCotacao repRegrasCotacaoValorCotacao = new Repositorio.Embarcador.Cotacao.RegraCotacaoValorCotacao(unitOfWork);


                // Codigo da busca 
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca entidade
                Dominio.Entidades.Embarcador.Cotacao.RegraCotacao regrasCotacao = repRegrasCotacao.BuscarPorCodigo(codigo, false);

                if (regrasCotacao == null)
                    return new JsonpResult(false, "Não foi possível buscar a regra.");

                #region BuscaRegras

                List<Dominio.Entidades.Embarcador.Cotacao.RegrasCepDestino> regraCepDestino = repRegraCotacaoCepDestino.BuscarPorRegras(regrasCotacao.Codigo);
                List<Dominio.Entidades.Embarcador.Cotacao.RegrasCubagem> regraCubagem = repRegrasCotacaoCubagem.BuscarPorRegras(regrasCotacao.Codigo);
                List<Dominio.Entidades.Embarcador.Cotacao.RegrasDistancia> regraDistancia = repRegraCotacaoDistancia.BuscarPorRegras(regrasCotacao.Codigo);
                List<Dominio.Entidades.Embarcador.Cotacao.RegrasEstadoDestino> regraEstadoDestino = repRegrasCotacaoEstadoDestino.BuscarPorRegras(regrasCotacao.Codigo);
                List<Dominio.Entidades.Embarcador.Cotacao.RegrasExpedidor> regraExpedidor = repRegrasCotacaoExpedidor.BuscarPorRegras(regrasCotacao.Codigo);
                List<Dominio.Entidades.Embarcador.Cotacao.RegrasDestinatario> regraDestinatario = repRegrasCotacaoDestinatario.BuscarPorRegras(regrasCotacao.Codigo);
                List<Dominio.Entidades.Embarcador.Cotacao.RegrasGrupoProduto> regraGrupoProduto = repRegrasCotacaoGrupoProduto.BuscarPorRegras(regrasCotacao.Codigo);
                List<Dominio.Entidades.Embarcador.Cotacao.RegrasLinhaSeparacao> regraLinhaSeparacao = repRegrasCotacaoLinhaSeparacao.BuscarPorRegras(regrasCotacao.Codigo);
                List<Dominio.Entidades.Embarcador.Cotacao.RegrasMarcaProduto> regraMarcaProduto = repRegrasCotacaoMarcaProduto.BuscarPorRegras(regrasCotacao.Codigo);
                List<Dominio.Entidades.Embarcador.Cotacao.RegrasPeso> regraPeso = repRegrasCotacaoPeso.BuscarPorRegras(regrasCotacao.Codigo);
                List<Dominio.Entidades.Embarcador.Cotacao.RegrasProduto> regraProduto = repRegrasCotacaoProduto.BuscarPorRegras(regrasCotacao.Codigo);
                List<Dominio.Entidades.Embarcador.Cotacao.RegrasCotacaoTransportador> regraTransportador = repRegrasCotacaoTransportador.BuscarPorRegras(regrasCotacao.Codigo);
                List<Dominio.Entidades.Embarcador.Cotacao.RegrasValorMercadoria> regraValorMercadoria = repRegrasCotacaoValorMercadoria.BuscarPorRegras(regrasCotacao.Codigo);
                List<Dominio.Entidades.Embarcador.Cotacao.RegrasVolume> regraVolume = repRegrasCotacaoVolume.BuscarPorRegras(regrasCotacao.Codigo);
                List<Dominio.Entidades.Embarcador.Cotacao.RegrasArestaProduto> regraArestaProduto = repRegrasArestaProduto.BuscarPorRegras(regrasCotacao.Codigo);
                List<Dominio.Entidades.Embarcador.Cotacao.RegrasValorCotacao> regraValorCotacao = repRegrasCotacaoValorCotacao.BuscarPorRegras(regrasCotacao.Codigo);

                #endregion

                var dynRegra = new
                {
                    regrasCotacao.Codigo,
                    regrasCotacao.NumeroDiasFrete,
                    regrasCotacao.Ativo,
                    Vigencia = regrasCotacao.Vigencia.HasValue ? regrasCotacao.Vigencia.Value.ToString("dd/MM/yyyy") : string.Empty,
                    Descricao = !string.IsNullOrWhiteSpace(regrasCotacao.Descricao) ? regrasCotacao.Descricao : string.Empty,
                    Observacao = !string.IsNullOrWhiteSpace(regrasCotacao.Observacoes) ? regrasCotacao.Observacoes : string.Empty,
                    Transportadores = (from o in regrasCotacao.Transportadores select new { o.Codigo, Descricao = o.NomeCNPJ }).ToList(),
                    OpcaoAplicacao = regrasCotacao.TipoAplicacao,
                    PercentualCobranca = regrasCotacao.PercentualCobranca,
                    PrioridadeRegra = regrasCotacao.PrioridadeRegra,
                    ValorFixoCotacaoFrete = regrasCotacao.ValorFixoCotacaoFrete,
                    ValorCobranca = regrasCotacao.ValorCobranca,                    
                    ModeloVeicularCarga = new { Codigo = regrasCotacao.ModeloVeicularCarga?.Codigo ?? 0, Descricao = regrasCotacao.ModeloVeicularCarga?.Descricao ?? string.Empty },

                    UsarRegraPorPeso = regrasCotacao.RegraPorPeso,
                    Peso = (from obj in regraPeso select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Cotacao.RegrasPeso>(obj, "Peso", "Peso", true)).ToList(),

                    UsarRegraPorDistancia = regrasCotacao.RegraPorDistancia,
                    Distancia = (from obj in regraDistancia select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Cotacao.RegrasDistancia>(obj, "Distancia", "Distancia", true)).ToList(),

                    UsarRegraPorValorMercadoria = regrasCotacao.RegraPorValorMercadoria,
                    ValorMercadoria = (from obj in regraValorMercadoria select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Cotacao.RegrasValorMercadoria>(obj, "ValorMercadoria", "Valor", true)).ToList(),

                    UsarRegraPorGrupoProduto = regrasCotacao.RegraPorGrupoProduto,
                    GrupoProduto = (from obj in regraGrupoProduto select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Cotacao.RegrasGrupoProduto>(obj, "GrupoProduto", "Descricao")).ToList(),

                    UsarRegraPorEstadoDestino = regrasCotacao.RegraPorEstadoDestino,
                    EstadoDestino = (from obj in regraEstadoDestino select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Cotacao.RegrasEstadoDestino>(obj, "Estado", "Sigla")).ToList(),

                    UsarRegraPorExpedidor = regrasCotacao.RegraPorExpedidor,
                    Expedidor = (from obj in regraExpedidor select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Cotacao.RegrasExpedidor>(obj, "Expedidor", "Descricao")).ToList(),

                    UsarRegraPorDestinatario = regrasCotacao.RegraPorDestinatario,
                    Destinatario = (from obj in regraDestinatario select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Cotacao.RegrasDestinatario>(obj, "Destinatario", "Descricao")).ToList(),

                    UsarRegraPorTransportador = regrasCotacao.RegraPorTransportador,
                    Transportador = (from obj in regraTransportador select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Cotacao.RegrasCotacaoTransportador>(obj, "Transportador", "NomeCNPJ")).ToList(),

                    UsarRegraPorProduto = regrasCotacao.RegraPorProduto,
                    Produto = (from obj in regraProduto select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Cotacao.RegrasProduto>(obj, "Produto", "Descricao")).ToList(),

                    UsarRegraPorCepDestino = regrasCotacao.RegraPorCEPDestino,
                    CepDestino = (from obj in regraCepDestino select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Cotacao.RegrasCepDestino>(obj, "CepDestino", "CepDestino", true)).ToList(),

                    UsarRegraPorMarcaProduto = regrasCotacao.RegraPorMarcaProduto,
                    MarcaProduto = (from obj in regraMarcaProduto select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Cotacao.RegrasMarcaProduto>(obj, "MarcaProduto", "Descricao")).ToList(),

                    UsarRegraPorVolume = regrasCotacao.RegraPorVolume,
                    Volume = (from obj in regraVolume select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Cotacao.RegrasVolume>(obj, "Volume", "Volume", true)).ToList(),

                    UsarRegraPorLinhaSeparacao = regrasCotacao.RegraPorLinhaSeparacao,
                    LinhaSeparacao = (from obj in regraLinhaSeparacao select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Cotacao.RegrasLinhaSeparacao>(obj, "LinhaSeparacao", "Descricao")).ToList(),

                    UsarRegraPorCubagem = regrasCotacao.RegraPorCubagem,
                    Cubagem = (from obj in regraCubagem select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Cotacao.RegrasCubagem>(obj, "Cubagem", "Cubagem", true)).ToList(),

                    UsarRegraPorArestaProduto = regrasCotacao.RegraPorArestaProduto,
                    ArestaProduto = (from obj in regraArestaProduto select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Cotacao.RegrasArestaProduto>(obj, "ArestaProduto", "ArestaProduto", true)).ToList(),

                    UsarRegraPorValorCotacao = regrasCotacao.RegraPorValorCotacao,
                    ValorCotacao = (from obj in regraValorCotacao select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Cotacao.RegrasValorCotacao>(obj, "ValorCotacao", "Valor", true)).ToList(),
                };

                return new JsonpResult(dynRegra);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia Repositorios/Entidade
                Repositorio.Embarcador.Cotacao.RegraCotacao repRegrasCotacao = new Repositorio.Embarcador.Cotacao.RegraCotacao(unitOfWork);

                Repositorio.Embarcador.Cotacao.RegraCotacaoCepDestino repositorioRegraCotacaoCepDestino = new Repositorio.Embarcador.Cotacao.RegraCotacaoCepDestino(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoCubagem repositorioRegrasCotacaoCubagem = new Repositorio.Embarcador.Cotacao.RegraCotacaoCubagem(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoDistancia repositorioRegraCotacaoDistancia = new Repositorio.Embarcador.Cotacao.RegraCotacaoDistancia(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoEstadoDestino repositorioRegrasCotacaoEstadoDestino = new Repositorio.Embarcador.Cotacao.RegraCotacaoEstadoDestino(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoExpedidor repositorioRegrasCotacaoExpedidor = new Repositorio.Embarcador.Cotacao.RegraCotacaoExpedidor(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoDestinatario repositorioRegrasCotacaoDestinatario = new Repositorio.Embarcador.Cotacao.RegraCotacaoDestinatario(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoGrupoProduto repositorioRegrasCotacaoGrupoProduto = new Repositorio.Embarcador.Cotacao.RegraCotacaoGrupoProduto(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoLinhaSeparacao repositorioRegrasCotacaoLinhaSeparacao = new Repositorio.Embarcador.Cotacao.RegraCotacaoLinhaSeparacao(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoMarcaProduto repositorioRegrasCotacaoMarcaProduto = new Repositorio.Embarcador.Cotacao.RegraCotacaoMarcaProduto(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoPeso repositorioRegrasCotacaoPeso = new Repositorio.Embarcador.Cotacao.RegraCotacaoPeso(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoProduto repositorioRegrasCotacaoProduto = new Repositorio.Embarcador.Cotacao.RegraCotacaoProduto(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoTransportador repositorioRegrasCotacaoTransportador = new Repositorio.Embarcador.Cotacao.RegraCotacaoTransportador(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoValorMercadoria repositorioRegrasCotacaoValorMercadoria = new Repositorio.Embarcador.Cotacao.RegraCotacaoValorMercadoria(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoVolume repositorioRegrasCotacaoVolume = new Repositorio.Embarcador.Cotacao.RegraCotacaoVolume(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoArestaProduto repositorioRegrasArestaProduto = new Repositorio.Embarcador.Cotacao.RegraCotacaoArestaProduto(unitOfWork);
                Repositorio.Embarcador.Cotacao.RegraCotacaoValorCotacao repositorioRegrasCotacaoValorCotacao = new Repositorio.Embarcador.Cotacao.RegraCotacaoValorCotacao(unitOfWork);

                // Codigo da busca 
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca entidade
                Dominio.Entidades.Embarcador.Cotacao.RegraCotacao regrasCotacao = repRegrasCotacao.BuscarPorCodigo(codigo, false);

                if (regrasCotacao == null)
                    return new JsonpResult(false, "Não foi possível buscar a regra.");

                // Inicia transicao
                unitOfWork.Start();

                regrasCotacao.Transportadores.Clear();
                repositorioRegraCotacaoCepDestino.DeletarTodosPorRegra(codigo);
                repositorioRegrasCotacaoCubagem.DeletarTodosPorRegra(codigo);
                repositorioRegraCotacaoDistancia.DeletarTodosPorRegra(codigo);
                repositorioRegrasCotacaoEstadoDestino.DeletarTodosPorRegra(codigo);
                repositorioRegrasCotacaoExpedidor.DeletarTodosPorRegra(codigo);
                repositorioRegrasCotacaoDestinatario.DeletarTodosPorRegra(codigo);
                repositorioRegrasCotacaoGrupoProduto.DeletarTodosPorRegra(codigo);
                repositorioRegrasCotacaoLinhaSeparacao.DeletarTodosPorRegra(codigo);
                repositorioRegrasCotacaoMarcaProduto.DeletarTodosPorRegra(codigo);
                repositorioRegrasCotacaoPeso.DeletarTodosPorRegra(codigo);
                repositorioRegrasCotacaoProduto.DeletarTodosPorRegra(codigo);
                repositorioRegrasCotacaoTransportador.DeletarTodosPorRegra(codigo);
                repositorioRegrasCotacaoValorMercadoria.DeletarTodosPorRegra(codigo);
                repositorioRegrasCotacaoVolume.DeletarTodosPorRegra(codigo);
                repositorioRegrasArestaProduto.DeletarTodosPorRegra(codigo);
                repositorioRegrasCotacaoValorCotacao.DeletarTodosPorRegra(codigo);

                repRegrasCotacao.Deletar(regrasCotacao, Auditado);

                // Comita alteracoes
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Já existem solicitações vinculadas à regra.");
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherEntidade(ref Dominio.Entidades.Embarcador.Cotacao.RegraCotacao regrasCotacao, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia Repositorios/Entidade
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloveicular = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);

            // Converte parametros
            string descricao = !string.IsNullOrWhiteSpace(Request.Params("Descricao")) ? Request.Params("Descricao") : string.Empty;
            string observacao = !string.IsNullOrWhiteSpace(Request.Params("Observacao")) ? Request.Params("Observacao") : string.Empty;

            DateTime dataVigenciaAux;
            DateTime? dataVigencia = null;

            if (DateTime.TryParseExact(Request.Params("Vigencia"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataVigenciaAux))
                dataVigencia = dataVigenciaAux;

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoCotacao tipoAplicacao;
            Enum.TryParse(Request.Params("OpcaoAplicacao"), out tipoAplicacao);

            bool ativo = true;
            bool.TryParse(Request.Params("Ativo"), out ativo);            
            bool UsarRegraPorPeso;
            bool.TryParse(Request.Params("UsarRegraPorPeso"), out UsarRegraPorPeso);
            bool UsarRegraPorDistancia;
            bool.TryParse(Request.Params("UsarRegraPorDistancia"), out UsarRegraPorDistancia);
            bool UsarRegraPorValorMercadoria;
            bool.TryParse(Request.Params("UsarRegraPorValorMercadoria"), out UsarRegraPorValorMercadoria);
            bool UsarRegraPorGrupoProduto;
            bool.TryParse(Request.Params("UsarRegraPorGrupoProduto"), out UsarRegraPorGrupoProduto);
            bool UsarRegraPorEstadoDestino;
            bool.TryParse(Request.Params("UsarRegraPorEstadoDestino"), out UsarRegraPorEstadoDestino);
            bool UsarRegraPorExpedidor;
            bool.TryParse(Request.Params("UsarRegraPorExpedidor"), out UsarRegraPorExpedidor);
            bool UsarRegraPorDestinatario;
            bool.TryParse(Request.Params("UsarRegraPorDestinatario"), out UsarRegraPorDestinatario);
            bool UsarRegraPorTransportador;
            bool.TryParse(Request.Params("UsarRegraPorTransportador"), out UsarRegraPorTransportador);
            bool UsarRegraPorProduto;
            bool.TryParse(Request.Params("UsarRegraPorProduto"), out UsarRegraPorProduto);
            bool UsarRegraPorCubagem;
            bool.TryParse(Request.Params("UsarRegraPorCubagem"), out UsarRegraPorCubagem);
            bool UsarRegraPorCepDestino;
            bool.TryParse(Request.Params("UsarRegraPorCepDestino"), out UsarRegraPorCepDestino);
            bool UsarRegraPorMarcaProduto;
            bool.TryParse(Request.Params("UsarRegraPorMarcaProduto"), out UsarRegraPorMarcaProduto);
            bool UsarRegraPorLinhaSeparacao;
            bool.TryParse(Request.Params("UsarRegraPorLinhaSeparacao"), out UsarRegraPorLinhaSeparacao);
            bool UsarRegraPorVolume;
            bool.TryParse(Request.Params("UsarRegraPorVolume"), out UsarRegraPorVolume);
            bool UsarRegraPorArestaProduto;
            bool.TryParse(Request.Params("UsarRegraPorArestaProduto"), out UsarRegraPorArestaProduto);

            List<int> codigosTransportadores = new List<int>();
            if (!string.IsNullOrWhiteSpace(Request.Params("Transportadores")))
            {
                List<ObjetoTransportador> dynAprovadores = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ObjetoTransportador>>(Request.Params("Transportadores"));

                for (var i = 0; i < dynAprovadores.Count(); i++)
                    codigosTransportadores.Add(dynAprovadores[i].Codigo);
            }
            List<Dominio.Entidades.Empresa> listaTransportadores = repEmpresa.BuscarPorCodigos(codigosTransportadores);

            if (tipoAplicacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoCotacao.UsarTransportador && tipoAplicacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoCotacao.ExcluirTransportador)
                listaTransportadores = new List<Dominio.Entidades.Empresa>();

            // Seta na entidade
            regrasCotacao.Descricao = descricao;
            regrasCotacao.Observacoes = observacao;
            regrasCotacao.Vigencia = dataVigencia;
            regrasCotacao.TipoAplicacao = tipoAplicacao;
            regrasCotacao.Ativo = ativo;
            regrasCotacao.NumeroDiasFrete = Request.GetNullableIntParam("NumeroDiasFrete");
            regrasCotacao.PercentualCobranca = Request.GetNullableDecimalParam("PercentualCobranca");
            regrasCotacao.ValorFixoCotacaoFrete = Request.GetDecimalParam("ValorFixoCotacaoFrete");
            regrasCotacao.ValorCobranca = Request.GetDecimalParam("ValorCobranca");
            regrasCotacao.PrioridadeRegra = Request.GetNullableIntParam("PrioridadeRegra");
            regrasCotacao.Transportadores = listaTransportadores;            
            regrasCotacao.ModeloVeicularCarga = repModeloveicular.BuscarPorCodigo(Request.GetIntParam("ModeloVeicularCarga"));
            regrasCotacao.RegraPorCEPDestino = UsarRegraPorCepDestino;
            regrasCotacao.RegraPorCubagem = UsarRegraPorCubagem;
            regrasCotacao.RegraPorDistancia = UsarRegraPorDistancia;
            regrasCotacao.RegraPorEstadoDestino = UsarRegraPorEstadoDestino;
            regrasCotacao.RegraPorExpedidor = UsarRegraPorExpedidor;
            regrasCotacao.RegraPorDestinatario = UsarRegraPorDestinatario;
            regrasCotacao.RegraPorGrupoProduto = UsarRegraPorGrupoProduto;
            regrasCotacao.RegraPorLinhaSeparacao = UsarRegraPorLinhaSeparacao;
            regrasCotacao.RegraPorMarcaProduto = UsarRegraPorMarcaProduto;
            regrasCotacao.RegraPorPeso = UsarRegraPorPeso;
            regrasCotacao.RegraPorProduto = UsarRegraPorProduto;
            regrasCotacao.RegraPorTransportador = UsarRegraPorTransportador;
            regrasCotacao.RegraPorValorMercadoria = UsarRegraPorValorMercadoria;
            regrasCotacao.RegraPorVolume = UsarRegraPorVolume;
            regrasCotacao.RegraPorArestaProduto = UsarRegraPorArestaProduto;
            regrasCotacao.RegraPorValorCotacao = Request.GetBoolParam("UsarRegraPorValorCotacao");

        }

        private void PreencherEntidadeRegra<T>(string nomePropriedade, string parametroJson, bool usarDynamic, ref List<T> regrasProTipo, ref Dominio.Entidades.Embarcador.Cotacao.RegraCotacao regrasCotacao, Func<dynamic, object> lambda = null)
        {
            /* Descricao
             * RegrasAutorizacaoOcorrencia é passado com ref, pois é vinculado a regra específica (RegraPorTipo) e após inserir no banco, a referencia permanece com o Codigo válido
             * 
             * Esse método facilita a instancia de novas regras, já que todas possuem o mesmo padra
             * - RegraOcorrencia (Entidade Pai)
             * - Ordem
             * - Codicao
             * - Juncao
             * - TIPO
             * 
             * Esse último, é instanciado com o retorno do callback, já que é o único parametro que é modificado
             * Mas quando não for uma enteidade, mas um valor simples, basta usar a flag usarDynamic = true,
             * Fazendo isso é setado o valor que vem no RegrasPorTipo.Valor
             */

            // Converte json (com o parametro get)
            List<RegrasPorTipo> dynRegras = Newtonsoft.Json.JsonConvert.DeserializeObject<List<RegrasPorTipo>>(Request.Params(parametroJson));

            if (dynRegras == null)
                throw new Exception("Erro ao converter os dados recebidos.");

            // Variavel auxiliar
            PropertyInfo prop;

            // Itera retornos
            for (var i = 0; i < dynRegras.Count(); i++)
            {
                // Instancia o objeto T (T não possui construor new)
                T regra = default(T);
                regra = Activator.CreateInstance<T>();

                // Seta as propriedas da entidade
                int codigoRegra = 0;
                int.TryParse(dynRegras[i].Codigo.ToString(), out codigoRegra);
                prop = regra.GetType().GetProperty("Codigo", BindingFlags.Public | BindingFlags.Instance);
                prop.SetValue(regra, codigoRegra, null);

                prop = regra.GetType().GetProperty("RegrasCotacao", BindingFlags.Public | BindingFlags.Instance);
                prop.SetValue(regra, regrasCotacao, null);

                prop = regra.GetType().GetProperty("Ordem", BindingFlags.Public | BindingFlags.Instance);
                prop.SetValue(regra, dynRegras[i].Ordem, null);

                prop = regra.GetType().GetProperty("Condicao", BindingFlags.Public | BindingFlags.Instance);
                prop.SetValue(regra, dynRegras[i].Condicao, null);

                prop = regra.GetType().GetProperty("Juncao", BindingFlags.Public | BindingFlags.Instance);
                prop.SetValue(regra, dynRegras[i].Juncao, null);

                if (!usarDynamic)
                {
                    // Executa lambda
                    var result = dynRegras[i].Entidade != null ? lambda(dynRegras[i].Entidade.Codigo) : null;

                    prop = regra.GetType().GetProperty(nomePropriedade, BindingFlags.Public | BindingFlags.Instance);
                    prop.SetValue(regra, result, null);
                }
                else
                {
                    prop = regra.GetType().GetProperty(nomePropriedade, BindingFlags.Public | BindingFlags.Instance);

                    if (prop.PropertyType.Name.Equals("Decimal"))
                        prop.SetValue(regra, ((string)dynRegras[i].Valor.ToString()).ToDecimal(), null);
                    else if (prop.PropertyType.Name.Equals("Int32"))
                        prop.SetValue(regra, ((string)dynRegras[i].Valor.ToString()).ToInt(), null);
                    else
                        prop.SetValue(regra, dynRegras[i].Valor, null);
                }

                // Adiciona lista de retorno
                regrasProTipo.Add(regra);
            }

        }

        private bool ValidarEntidade(Dominio.Entidades.Embarcador.Cotacao.RegraCotacao regrasCotacao, out List<string> erros)
        {
            erros = new List<string>();

            if (string.IsNullOrWhiteSpace(regrasCotacao.Descricao))
                erros.Add("Descrição é obrigatória.");

            if (regrasCotacao.TipoAplicacao == TipoAplicacaoCotacao.UtilizarModeloVeicular && regrasCotacao.ModeloVeicularCarga == null)
                erros.Add("Ao utilizar o tipo de aplicação modelo veícular é obrigatório informar o mesmo!");

            return erros.Count() == 0;
        }

        private bool ValidarEntidadeRegra<T>(string nomeRegra, string nomePropriedade, List<T> regrasProTipo, out List<string> erros)
        {
            erros = new List<string>();

            if (regrasProTipo.Count() == 0)
                erros.Add("Nenhuma regra " + nomeRegra + " cadastrada.");
            else
            {
                // Variavel auxiliar
                PropertyInfo prop;

                // Itera validacao
                for (var i = 0; i < regrasProTipo.Count(); i++)
                {
                    var regra = regrasProTipo[i];
                    prop = regra.GetType().GetProperty(nomePropriedade, BindingFlags.Public | BindingFlags.Instance);

                    if (prop.GetValue(regra) == null)
                        erros.Add(nomeRegra + " da regra é obrigatório.");
                }
            }

            return erros.Count() == 0;
        }

        private RegrasPorTipo RetornaRegraPorTipoDyn<T>(dynamic obj, string paramentro, string paramentroDescricaoValor, bool usarValor = false)
        {
            // Variavel auxiliar
            PropertyInfo prop;


            prop = obj.GetType().GetProperty("Codigo", BindingFlags.Public | BindingFlags.Instance);
            int codigo = prop.GetValue(obj);

            prop = obj.GetType().GetProperty("Ordem", BindingFlags.Public | BindingFlags.Instance);
            int ordem = prop.GetValue(obj);

            prop = obj.GetType().GetProperty("Juncao", BindingFlags.Public | BindingFlags.Instance);
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao juncao = prop.GetValue(obj);

            prop = obj.GetType().GetProperty("Condicao", BindingFlags.Public | BindingFlags.Instance);
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao condicao = prop.GetValue(obj);


            ObjetoEntidade objetoEntidade = new ObjetoEntidade();
            dynamic valor = null;
            if (!usarValor)
            {
                prop = obj.GetType().GetProperty(paramentro, BindingFlags.Public | BindingFlags.Instance);
                dynamic entidade = prop.GetValue(obj);

                dynamic codigoEntidade = null;
                string descricaoEntidade = "";
                if (paramentro == "Estado")
                {
                    prop = entidade.GetType().GetProperty("Sigla", BindingFlags.Public | BindingFlags.Instance);
                    codigoEntidade = prop.GetValue(entidade);

                    prop = entidade.GetType().GetProperty(paramentroDescricaoValor, BindingFlags.Public | BindingFlags.Instance);
                    descricaoEntidade = prop.GetValue(entidade);
                }
                else
                {
                    prop = entidade.GetType().GetProperty("Codigo", BindingFlags.Public | BindingFlags.Instance);
                    codigoEntidade = prop.GetValue(entidade);

                    prop = entidade.GetType().GetProperty(paramentroDescricaoValor, BindingFlags.Public | BindingFlags.Instance);
                    descricaoEntidade = prop.GetValue(entidade);
                }

                objetoEntidade.Codigo = codigoEntidade;
                objetoEntidade.Descricao = descricaoEntidade;
            }
            else
            {
                prop = obj.GetType().GetProperty(paramentroDescricaoValor, BindingFlags.Public | BindingFlags.Instance);
                valor = prop.GetValue(obj);
            }

            RegrasPorTipo restorno = new RegrasPorTipo()
            {
                Codigo = codigo,
                Ordem = ordem,
                Juncao = juncao,
                Condicao = condicao,
                Entidade = objetoEntidade,
                Valor = valor,
            };
            return restorno;
        }

        #endregion
    }
}

