using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cotacoes
{
    [CustomAuthorize("Cotacoes/RegraCotacaoPedido")]
    public class RegraCotacaoPedidoController : BaseController
    {
		#region Construtores

		public RegraCotacaoPedidoController(Conexao conexao) : base(conexao) { }

		#endregion

        private Models.Grid.Grid GridPesquisa()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Descrição", "Descricao", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Vigência", "Vigencia", 15, Models.Grid.Align.center, true);

            return grid;
        }

        #region Métodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                // Retorna Dados
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
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);

                // Gera excel
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
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

                Repositorio.Embarcador.CotacaoPedido.RegrasCotacaoPedido repRegrasCotacaoPedido = new Repositorio.Embarcador.CotacaoPedido.RegrasCotacaoPedido(unitOfWork);
                Dominio.Entidades.Embarcador.CotacaoPedido.RegrasCotacaoPedido regraCotacaoPedido = new Dominio.Entidades.Embarcador.CotacaoPedido.RegrasCotacaoPedido();

                PreencherEntidade(ref regraCotacaoPedido, unitOfWork);

                List<string> erros = new List<string>();
                if (!ValidarEntidade(regraCotacaoPedido, out erros))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, String.Join("<br>", erros));
                }
                repRegrasCotacaoPedido.Inserir(regraCotacaoPedido, Auditado);

                try
                {
                    PreencherTodasRegras(ref regraCotacaoPedido, ref erros, unitOfWork);
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, ex.Message);
                }

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

                Repositorio.Embarcador.CotacaoPedido.RegrasCotacaoPedido repRegrasCotacaoPedido = new Repositorio.Embarcador.CotacaoPedido.RegrasCotacaoPedido(unitOfWork);
                int.TryParse(Request.Params("Codigo"), out int codigoRegra);
                Dominio.Entidades.Embarcador.CotacaoPedido.RegrasCotacaoPedido regraCotacaoPedido = repRegrasCotacaoPedido.BuscarPorCodigo(codigoRegra, true);

                if (regraCotacaoPedido == null)
                    return new JsonpResult(false, "Não foi possível buscar a regra.");

                PreencherEntidade(ref regraCotacaoPedido, unitOfWork);

                List<string> erros = new List<string>();
                if (!ValidarEntidade(regraCotacaoPedido, out erros))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, String.Join("<br>", erros));
                }
                repRegrasCotacaoPedido.Atualizar(regraCotacaoPedido, Auditado);

                try
                {
                    PreencherTodasRegras(ref regraCotacaoPedido, ref erros, unitOfWork);
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, ex.Message);
                }

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
                Repositorio.Embarcador.CotacaoPedido.RegrasCotacaoPedido repRegrasCotacaoPedido = new Repositorio.Embarcador.CotacaoPedido.RegrasCotacaoPedido(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigoRegra);

                Dominio.Entidades.Embarcador.CotacaoPedido.RegrasCotacaoPedido regraCotacaoPedido = repRegrasCotacaoPedido.BuscarPorCodigo(codigoRegra);

                if (regraCotacaoPedido == null)
                    return new JsonpResult(false, "Não foi possível buscar a regra.");

                var dynRegra = new
                {
                    regraCotacaoPedido.Codigo,
                    regraCotacaoPedido.NumeroAprovadores,
                    Vigencia = regraCotacaoPedido.Vigencia.HasValue ? regraCotacaoPedido.Vigencia.Value.ToString("dd/MM/yyyy") : string.Empty,
                    Descricao = !string.IsNullOrWhiteSpace(regraCotacaoPedido.Descricao) ? regraCotacaoPedido.Descricao : string.Empty,
                    Observacao = !string.IsNullOrWhiteSpace(regraCotacaoPedido.Observacoes) ? regraCotacaoPedido.Observacoes : string.Empty,

                    Aprovadores = (from o in regraCotacaoPedido.Aprovadores select new { o.Codigo, o.Nome }).ToList(),

                    UsarRegraPorTipoCarga = regraCotacaoPedido.RegraPorTipoCarga,
                    AlcadasTipoCarga = (from obj in regraCotacaoPedido.RegrasCotacaoPedidoTipoCarga
                                        select new
                                        {
                                            obj.Codigo,
                                            obj.Ordem,
                                            obj.Juncao,
                                            obj.Condicao,
                                            Entidade = obj.TipoDeCarga != null ? new { obj.TipoDeCarga.Codigo, obj.TipoDeCarga.Descricao } : null
                                        }).ToList(),

                    UsarRegraPorTipoOperacao = regraCotacaoPedido.RegraPorTipoOperacao,
                    AlcadasTipoOperacao = (from obj in regraCotacaoPedido.RegrasCotacaoPedidoTipoOperacao
                                           select new
                                           {
                                               obj.Codigo,
                                               obj.Ordem,
                                               obj.Juncao,
                                               obj.Condicao,
                                               Entidade = obj.TipoOperacao != null ? new { obj.TipoOperacao.Codigo, obj.TipoOperacao.Descricao } : null
                                           }).ToList(),

                    UsarRegraPorValor = regraCotacaoPedido.RegraPorValorFrete,
                    AlcadasValor = (from obj in regraCotacaoPedido.RegrasCotacaoPedidoValor
                                    select new
                                    {
                                        obj.Codigo,
                                        obj.Ordem,
                                        obj.Juncao,
                                        obj.Condicao,
                                        Valor = obj.Valor.ToString("n2")
                                    }).ToList()
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
                Repositorio.Embarcador.CotacaoPedido.RegrasCotacaoPedido repRegrasCotacaoPedido = new Repositorio.Embarcador.CotacaoPedido.RegrasCotacaoPedido(unitOfWork);

                // Codigo da busca 
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca entidade
                Dominio.Entidades.Embarcador.CotacaoPedido.RegrasCotacaoPedido regraCotacaoPedido = repRegrasCotacaoPedido.BuscarPorCodigo(codigo);

                if (regraCotacaoPedido == null)
                    return new JsonpResult(false, "Não foi possível buscar a regra.");

                // Inicia transicao
                unitOfWork.Start();

                regraCotacaoPedido.Aprovadores.Clear();
                regraCotacaoPedido.RegrasCotacaoPedidoTipoCarga.Clear();
                regraCotacaoPedido.RegrasCotacaoPedidoTipoOperacao.Clear();
                regraCotacaoPedido.RegrasCotacaoPedidoValor.Clear();

                repRegrasCotacaoPedido.Deletar(regraCotacaoPedido);

                // Comita alteracoes
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Já existem informações vinculadas à regra.");
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
        private void PreencherEntidade(ref Dominio.Entidades.Embarcador.CotacaoPedido.RegrasCotacaoPedido regraCotacaoPedido, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia Repositorios/Entidade
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);


            // Converte parametros
            string descricao = Request.Params("Descricao") ?? string.Empty;
            string observacao = Request.Params("Observacao") ?? string.Empty;

            DateTime? dataVigencia = null;

            if (DateTime.TryParseExact(Request.Params("Vigencia"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataVigenciaAux))
                dataVigencia = dataVigenciaAux;

            int.TryParse(Request.Params("NumeroAprovadores"), out int numeroAprovadores);

            bool.TryParse(Request.Params("UsarRegraPorTipoCarga"), out bool usarRegraPorTipoCarga);
            bool.TryParse(Request.Params("UsarRegraPorTipoOperacao"), out bool usarRegraPorTipoOperacao);
            bool.TryParse(Request.Params("UsarRegraPorValor"), out bool usarRegraPorValor);

            List<int> codigosUsuarios = new List<int>();
            if (!string.IsNullOrWhiteSpace(Request.Params("Aprovadores")))
            {
                List<Dominio.ObjetosDeValor.Embarcador.Alcada.Aprovadores> dynAprovadores = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Alcada.Aprovadores>>(Request.Params("Aprovadores"));

                for (var i = 0; i < dynAprovadores.Count(); i++)
                    codigosUsuarios.Add(dynAprovadores[i].Codigo);
            }
            List<Dominio.Entidades.Usuario> listaAprovadores = repUsuario.BuscarUsuariosPorCodigos(codigosUsuarios.ToArray(), null);

            // Seta na entidade
            regraCotacaoPedido.Descricao = descricao;
            regraCotacaoPedido.Observacoes = observacao;
            regraCotacaoPedido.Vigencia = dataVigencia;
            regraCotacaoPedido.NumeroAprovadores = numeroAprovadores;
            regraCotacaoPedido.Aprovadores = listaAprovadores;

            regraCotacaoPedido.RegraPorTipoCarga = usarRegraPorTipoCarga;
            regraCotacaoPedido.RegraPorTipoOperacao = usarRegraPorTipoOperacao;
            regraCotacaoPedido.RegraPorValorFrete = usarRegraPorValor;
        }

        private bool ValidarEntidade(Dominio.Entidades.Embarcador.CotacaoPedido.RegrasCotacaoPedido regraCotacaoPedido, out List<string> erros)
        {
            erros = new List<string>();

            if (string.IsNullOrWhiteSpace(regraCotacaoPedido.Descricao))
                erros.Add("Descrição é obrigatória.");

            if (regraCotacaoPedido.Aprovadores.Count() < regraCotacaoPedido.NumeroAprovadores)
                erros.Add("O número de aprovadores selecionados deve ser maior ou igual a " + regraCotacaoPedido.NumeroAprovadores.ToString());

            return erros.Count() == 0;
        }

        private bool ValidarEntidadeRegra<T>(string nomeRegra, List<T> regrasPorTipo, out List<string> erros)
        {
            erros = new List<string>();

            if (regrasPorTipo.Count() == 0)
                erros.Add("Nenhuma regra " + nomeRegra + " cadastrada.");

            return erros.Count() == 0;
        }

        private void PreencherTodasRegras(ref Dominio.Entidades.Embarcador.CotacaoPedido.RegrasCotacaoPedido regraCotacaoPedido, ref List<string> errosRegras, Repositorio.UnitOfWork unitOfWork)
        {
            // Erros de validacao
            List<string> erros = new List<string>();

            Repositorio.Embarcador.CotacaoPedido.RegrasCotacaoPedidoTipoCarga repRegrasCotacaoPedidoTipoCarga = new Repositorio.Embarcador.CotacaoPedido.RegrasCotacaoPedidoTipoCarga(unitOfWork);
            Repositorio.Embarcador.CotacaoPedido.RegrasCotacaoPedidoTipoOperacao repRegrasCotacaoPedidoTipoOperacao = new Repositorio.Embarcador.CotacaoPedido.RegrasCotacaoPedidoTipoOperacao(unitOfWork);
            Repositorio.Embarcador.CotacaoPedido.RegrasCotacaoPedidoValor repRegrasCotacaoPedidoValor = new Repositorio.Embarcador.CotacaoPedido.RegrasCotacaoPedidoValor(unitOfWork);

            #region Tipo Carga
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regraCotacaoPedido.RegraPorTipoCarga)
            {
                // Preenche regra
                try
                {
                    Repositorio.Embarcador.Cargas.TipoDeCarga repTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
                    dynamic dynRegras = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(Request.Params("AlcadasTipoCarga"));

                    if (regraCotacaoPedido.RegrasCotacaoPedidoTipoCarga != null && regraCotacaoPedido.RegrasCotacaoPedidoTipoCarga.Count > 0)
                    {
                        List<int> codigos = new List<int>();

                        foreach (var regra in dynRegras)
                        {
                            int codigo = 0;
                            if (regra.Codigo != null && int.TryParse((string)regra.Codigo, out codigo) && codigo > 0)
                                codigos.Add(codigo);
                        }

                        List<Dominio.Entidades.Embarcador.CotacaoPedido.RegrasCotacaoPedidoTipoCarga> regraCotacaoDeletar = (from obj in regraCotacaoPedido.RegrasCotacaoPedidoTipoCarga where !codigos.Contains(obj.Codigo) select obj).ToList();

                        for (var i = 0; i < regraCotacaoDeletar.Count; i++)
                            repRegrasCotacaoPedidoTipoCarga.Deletar(regraCotacaoDeletar[i], Auditado);
                    }
                    else
                        regraCotacaoPedido.RegrasCotacaoPedidoTipoCarga = new List<Dominio.Entidades.Embarcador.CotacaoPedido.RegrasCotacaoPedidoTipoCarga>();

                    foreach (var regra in dynRegras)
                    {
                        int.TryParse((string)regra.Codigo, out int codigoRegra);

                        Dominio.Entidades.Embarcador.CotacaoPedido.RegrasCotacaoPedidoTipoCarga tipoCarga = codigoRegra > 0 ? repRegrasCotacaoPedidoTipoCarga.BuscarPorCodigo(codigoRegra, true) : null;
                        if (tipoCarga == null)
                            tipoCarga = new Dominio.Entidades.Embarcador.CotacaoPedido.RegrasCotacaoPedidoTipoCarga();

                        int.TryParse(regra.Entidade != null ? regra.Entidade.Codigo.ToString() : 0, out int codigoTipoCarga);

                        tipoCarga.Condicao = regra.Condicao;
                        tipoCarga.Ordem = regra.Ordem;
                        tipoCarga.Juncao = regra.Juncao;
                        tipoCarga.TipoDeCarga = repTipoCarga.BuscarPorCodigo(codigoTipoCarga);
                        tipoCarga.RegrasCotacaoPedido = regraCotacaoPedido;

                        if (tipoCarga.Codigo > 0)
                            repRegrasCotacaoPedidoTipoCarga.Atualizar(tipoCarga, Auditado);
                        else
                            repRegrasCotacaoPedidoTipoCarga.Inserir(tipoCarga, Auditado);
                    }
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                    errosRegras.Add("TipoCarga");
                }

                // Valida regra (se for invalida, nao continua o fluxo)
                List<Dominio.Entidades.Embarcador.CotacaoPedido.RegrasCotacaoPedidoTipoCarga> regraTipoCarga = repRegrasCotacaoPedidoTipoCarga.BuscarPorRegra(regraCotacaoPedido.Codigo);
                if (!ValidarEntidadeRegra("TipoCarga", regraTipoCarga, out erros))
                    throw new Exception(String.Join("<br>", erros));
            }
            #endregion

            #region Tipo Operação
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regraCotacaoPedido.RegraPorTipoOperacao)
            {
                // Preenche regra
                try
                {
                    Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                    dynamic dynRegras = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(Request.Params("AlcadasTipoOperacao"));

                    if (regraCotacaoPedido.RegrasCotacaoPedidoTipoOperacao != null && regraCotacaoPedido.RegrasCotacaoPedidoTipoOperacao.Count > 0)
                    {
                        List<int> codigos = new List<int>();

                        foreach (var regra in dynRegras)
                        {
                            int codigo = 0;
                            if (regra.Codigo != null && int.TryParse((string)regra.Codigo, out codigo) && codigo > 0)
                                codigos.Add(codigo);
                        }

                        List<Dominio.Entidades.Embarcador.CotacaoPedido.RegrasCotacaoPedidoTipoOperacao> regraCotacaoDeletar = (from obj in regraCotacaoPedido.RegrasCotacaoPedidoTipoOperacao where !codigos.Contains(obj.Codigo) select obj).ToList();

                        for (var i = 0; i < regraCotacaoDeletar.Count; i++)
                            repRegrasCotacaoPedidoTipoOperacao.Deletar(regraCotacaoDeletar[i], Auditado);
                    }
                    else
                        regraCotacaoPedido.RegrasCotacaoPedidoTipoOperacao = new List<Dominio.Entidades.Embarcador.CotacaoPedido.RegrasCotacaoPedidoTipoOperacao>();

                    foreach (var regra in dynRegras)
                    {
                        int.TryParse((string)regra.Codigo, out int codigoRegra);

                        Dominio.Entidades.Embarcador.CotacaoPedido.RegrasCotacaoPedidoTipoOperacao tipoOperacao = codigoRegra > 0 ? repRegrasCotacaoPedidoTipoOperacao.BuscarPorCodigo(codigoRegra, true) : null;
                        if (tipoOperacao == null)
                            tipoOperacao = new Dominio.Entidades.Embarcador.CotacaoPedido.RegrasCotacaoPedidoTipoOperacao();

                        int.TryParse(regra.Entidade != null ? regra.Entidade.Codigo.ToString() : 0, out int codigoTipoOperacao);

                        tipoOperacao.Condicao = regra.Condicao;
                        tipoOperacao.Ordem = regra.Ordem;
                        tipoOperacao.Juncao = regra.Juncao;
                        tipoOperacao.TipoOperacao = repTipoOperacao.BuscarPorCodigo(codigoTipoOperacao);
                        tipoOperacao.RegrasCotacaoPedido = regraCotacaoPedido;

                        if (tipoOperacao.Codigo > 0)
                            repRegrasCotacaoPedidoTipoOperacao.Atualizar(tipoOperacao, Auditado);
                        else
                            repRegrasCotacaoPedidoTipoOperacao.Inserir(tipoOperacao, Auditado);
                    }
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                    errosRegras.Add("TipoOperacao");
                }

                // Valida regra (se for invalida, nao continua o fluxo)
                List<Dominio.Entidades.Embarcador.CotacaoPedido.RegrasCotacaoPedidoTipoOperacao> regraTipoOperacao = repRegrasCotacaoPedidoTipoOperacao.BuscarPorRegra(regraCotacaoPedido.Codigo);
                if (!ValidarEntidadeRegra("TipoOperacao", regraTipoOperacao, out erros))
                    throw new Exception(String.Join("<br>", erros));
            }
            #endregion

            #region Valor
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regraCotacaoPedido.RegraPorValorFrete)
            {
                // Preenche regra
                try
                {
                    dynamic dynRegras = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(Request.Params("AlcadasValor"));

                    if (regraCotacaoPedido.RegrasCotacaoPedidoValor != null && regraCotacaoPedido.RegrasCotacaoPedidoValor.Count > 0)
                    {
                        List<int> codigos = new List<int>();

                        foreach (var regra in dynRegras)
                        {
                            int codigo = 0;
                            if (regra.Codigo != null && int.TryParse((string)regra.Codigo, out codigo) && codigo > 0)
                                codigos.Add(codigo);
                        }

                        List<Dominio.Entidades.Embarcador.CotacaoPedido.RegrasCotacaoPedidoValor> regraCotacaoDeletar = (from obj in regraCotacaoPedido.RegrasCotacaoPedidoValor where !codigos.Contains(obj.Codigo) select obj).ToList();

                        for (var i = 0; i < regraCotacaoDeletar.Count; i++)
                            repRegrasCotacaoPedidoValor.Deletar(regraCotacaoDeletar[i], Auditado);
                    }
                    else
                        regraCotacaoPedido.RegrasCotacaoPedidoValor = new List<Dominio.Entidades.Embarcador.CotacaoPedido.RegrasCotacaoPedidoValor>();

                    foreach (var regra in dynRegras)
                    {
                        int.TryParse((string)regra.Codigo, out int codigoRegra);

                        Dominio.Entidades.Embarcador.CotacaoPedido.RegrasCotacaoPedidoValor valor = codigoRegra > 0 ? repRegrasCotacaoPedidoValor.BuscarPorCodigo(codigoRegra, true) : null;
                        if (valor == null)
                            valor = new Dominio.Entidades.Embarcador.CotacaoPedido.RegrasCotacaoPedidoValor();

                        decimal.TryParse(regra.Valor.ToString(), out decimal valorDecimal);

                        valor.Condicao = regra.Condicao;
                        valor.Ordem = regra.Ordem;
                        valor.Juncao = regra.Juncao;
                        valor.Valor = valorDecimal;
                        valor.RegrasCotacaoPedido = regraCotacaoPedido;

                        if (valor.Codigo > 0)
                            repRegrasCotacaoPedidoValor.Atualizar(valor, Auditado);
                        else
                            repRegrasCotacaoPedidoValor.Inserir(valor, Auditado);
                    }
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                    errosRegras.Add("Valor");
                }

                // Valida regra (se for invalida, nao continua o fluxo)
                List<Dominio.Entidades.Embarcador.CotacaoPedido.RegrasCotacaoPedidoValor> regraValor = repRegrasCotacaoPedidoValor.BuscarPorRegra(regraCotacaoPedido.Codigo);
                if (!ValidarEntidadeRegra("Valor", regraValor, out erros))
                    throw new Exception(String.Join("<br>", erros));
            }
            #endregion
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.CotacaoPedido.RegrasCotacaoPedido repRegrasCotacaoPedido = new Repositorio.Embarcador.CotacaoPedido.RegrasCotacaoPedido(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            // Converte parametros
            int.TryParse(Request.Params("Aprovador"), out int codigoAprovador);

            DateTime? dataInicio = null, dataFim = null;

            if (DateTime.TryParseExact(Request.Params("DataInicio"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicioAux))
                dataInicio = dataInicioAux;

            if (DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFimAux))
                dataFim = dataFimAux;

            string descricao = !string.IsNullOrWhiteSpace(Request.Params("Descricao")) ? Request.Params("Descricao") : "";

            Dominio.Entidades.Usuario aprovador = repUsuario.BuscarPorCodigo(codigoAprovador);

            // Consulta
            List<Dominio.Entidades.Embarcador.CotacaoPedido.RegrasCotacaoPedido> listaGrid = repRegrasCotacaoPedido.ConsultarRegras(dataInicio, dataFim, aprovador, descricao, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repRegrasCotacaoPedido.ContarConsultaRegras(dataInicio, dataFim, aprovador, descricao);

            var lista = (from obj in listaGrid
                         select new
                         {
                             obj.Codigo,
                             Descricao = !string.IsNullOrWhiteSpace(obj.Descricao) ? obj.Descricao : string.Empty,
                             Vigencia = obj.Vigencia.HasValue ? obj.Vigencia.Value.ToString("dd/MM/yyyy") : string.Empty,
                         }).ToList();

            return lista.ToList();
        }
        #endregion
    }
}
