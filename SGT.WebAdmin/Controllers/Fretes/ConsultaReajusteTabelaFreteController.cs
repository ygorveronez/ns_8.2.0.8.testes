using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Fretes
{
    [CustomAuthorize("Fretes/ConsultaReajusteTabelaFrete")]
    public class ConsultaReajusteTabelaFreteController : BaseController
    {
		#region Construtores

		public ConsultaReajusteTabelaFreteController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Propriedades

        private int UltimaColunaDinamica = 1;
        private int NumeroMaximoComplementos = 15;
        private decimal TamanhoColunasValores = (decimal)1.75;
        private decimal TamanhoColunasParticipantes = (decimal)5.50;
        private decimal TamanhoColunasEnderecoParticipantes = 3;

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = GridPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosPesquisa = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                List<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete> listaAjustes = new List<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete>();
                int totalRegistro = 0;

                ExecutaPesquisa(ref listaAjustes, ref totalRegistro, parametrosPesquisa, unitOfWork);

                var lista = RetornaDyn(listaAjustes, unitOfWork);

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistro);

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
                Models.Grid.Grid grid = GridPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosPesquisa = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                List<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete> listaAjustes = new List<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete>();
                int totalRegistro = 0;

                ExecutaPesquisa(ref listaAjustes, ref totalRegistro, parametrosPesquisa, unitOfWork);

                var lista = RetornaDyn(listaAjustes, unitOfWork);

                grid.AdicionaRows(lista);

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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            // Busca o item
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Repositorios
                Repositorio.Embarcador.Frete.AjusteTabelaFrete repAjusteTabelaFrete = new Repositorio.Embarcador.Frete.AjusteTabelaFrete(unitOfWork);

                // Codigo requisicao
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Entidades
                Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete ajuste = repAjusteTabelaFrete.BuscarPorCodigo(codigo);

                if (ajuste == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");

                var dynAjuste = new
                {
                    ajuste.Codigo,
                    ajuste.Numero,
                    TabelaFrete = ajuste.TabelaFrete.Codigo,
                    DescricaoTabelaFrete = ajuste.TabelaFrete.Descricao,
                    DataCriacao = ajuste.DataCriacao.ToString("dd/MM/yyyy"),
                    DataAjuste = ajuste.DataAjuste.HasValue ? ajuste.DataAjuste.Value.ToString("dd/MM/yyyy") : string.Empty,
                    Situacao = ajuste.DescricaoSituacao,
                    EnumSituacao = ajuste.Situacao,
                    Etapa = ajuste.DescricaoEtapa
                };

                return new JsonpResult(dynAjuste);
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

        public async Task<IActionResult> RegrasAprovacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Converte parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                int usuario = 0;
                int.TryParse(Request.Params("Usuario"), out usuario);

                // Manipula grids
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Regra", "Regra", 30, Models.Grid.Align.left, false);

                if (usuario > 0)
                    grid.AdicionarCabecalho("Usuario", false);
                else
                    grid.AdicionarCabecalho("Usuário", "Usuario", 15, Models.Grid.Align.left, false);

                grid.AdicionarCabecalho("Etapa", "Etapa", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("PodeAprovar", false);

                // Instancia repositorio
                Repositorio.Embarcador.Frete.AjusteTabelaFreteAutorizacao repAjusteTabelaFreteAutorizacao = new Repositorio.Embarcador.Frete.AjusteTabelaFreteAutorizacao(unitOfWork);

                // Buscas regras do usuario para esso item
                List<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteAutorizacao> regras = repAjusteTabelaFreteAutorizacao.BuscarPorAjusteUsuario(codigo, usuario);

                // Converte as regras em dados apresentaveis
                var lista = (from regra in regras
                             select new
                             {
                                 regra.Codigo,
                                 Regra = regra.RegrasAutorizacaoTabelaFrete.Descricao,
                                 Situacao = regra.DescricaoSituacao,
                                 Usuario = regra.Usuario?.Nome ?? "",
                                 Etapa = regra.DescricaoEtapaAutorizacao,
                                 // Verifica se o usuario ja motificou essa autorizacao
                                 PodeAprovar = repAjusteTabelaFreteAutorizacao.VerificarSePodeAprovar(codigo, regra.Codigo, this.Usuario.Codigo),
                                 // Busca a cor de acordo com a situacao da autorizacao
                                 DT_RowColor = this.CoresRegras(regra.Situacao)
                             }).ToList();

                // Retorna Grid
                grid.setarQuantidadeTotal(lista.Count());
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

        public async Task<IActionResult> AprovarMultiplosAjustes()
        {
            /* Busca todas as itens selecionadas
             * Busca todas as regras das itens selecionadas
             * Aprova todas as regras
             * Atualiza informacoes das itens (verifica se esta aprovada ou rejeitada)
             */
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Frete.AjusteTabelaFrete repAjusteTabelaFrete = new Repositorio.Embarcador.Frete.AjusteTabelaFrete(unitOfWork);
                List<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete> ajustes = new List<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete>();

                try
                {
                    // Busca todas as itens selecionadas
                    ajustes = ObterAjuestesSelecionadas(unitOfWork);
                }
                catch (Exception ex)
                {
                    return new JsonpResult(false, ex.Message);
                }

                // Busca todas as regras das itens selecionadas
                List<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteAutorizacao> ajuestesAutorizacoes = BuscarRegrasPorAjueste(ajustes, this.Usuario.Codigo, unitOfWork);

                if (ajuestesAutorizacoes.Count == 0)
                    return new JsonpResult(false, "Não foram encontradas regras de autorizações para os ajustes.");

                // Inicia transacao
                unitOfWork.Start();

                // Guarda os valores das itens para fazer a checagem geral
                List<int> codigosItemVerificados = new List<int>();

                // Aprova todas as regras
                for (int i = 0; i < ajuestesAutorizacoes.Count(); i++)
                {
                    int codigo = ajuestesAutorizacoes[i].AjusteTabelaFrete.Codigo;

                    if (!codigosItemVerificados.Contains(codigo))
                        codigosItemVerificados.Add(codigo);

                    // Metodo de aprovar o item
                    EfetuarAprovacao(ajuestesAutorizacoes[i], false, unitOfWork);
                }

                // Itera todas as cargas para verificar situacao
                foreach (int cod in codigosItemVerificados)
                {
                    // Atualiza informacoes das itens (verifica se esta aprovada ou rejeitada)
                    bool retorno = this.VerificarSituacaoAjuste(repAjusteTabelaFrete.BuscarPorCodigo(cod), unitOfWork);
                    if (!retorno)
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, "Ocorreu uma falha ao aprovar os reajustes.");
                    }
                }
                // Finaliza transacao
                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao aprovar os reajustes.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AprovarMultiplasRegras()
        {
            /* Busca todas as regras do item
             * Aprova todas as regras
             * Atualiza informacoes das itens (verifica se esta aprovada ou rejeitada)
             */
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia
                Repositorio.Embarcador.Frete.AjusteTabelaFreteAutorizacao repAjusteTabelaFreteAutorizacao = new Repositorio.Embarcador.Frete.AjusteTabelaFreteAutorizacao(unitOfWork);
                Repositorio.Embarcador.Frete.AjusteTabelaFrete repAjusteTabelaFrete = new Repositorio.Embarcador.Frete.AjusteTabelaFrete(unitOfWork);

                // Converte parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca todas as regras das itens selecionadas
                List<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteAutorizacao> ajusteAutorizacoes = repAjusteTabelaFreteAutorizacao.BuscarPorCodigoAjusteEUsuario(codigo, this.Usuario.Codigo);

                // Inicia transacao
                unitOfWork.Start();

                // Aprova todas as regras
                for (int i = 0; i < ajusteAutorizacoes.Count(); i++)
                    EfetuarAprovacao(ajusteAutorizacoes[i], false, unitOfWork);

                // Atualiza informacoes das itens (verifica se esta aprovada ou rejeitada)
                bool retorno = this.VerificarSituacaoAjuste(repAjusteTabelaFrete.BuscarPorCodigo(codigo), unitOfWork);
                if (retorno)
                {
                    // Finaliza transacao
                    unitOfWork.CommitChanges();
                    return new JsonpResult(true);
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, "Ocorreu uma falha ao aprovar");
                }


            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao aprovar os ajustes.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Aprovar()
        {
            // Recebe o codigo da regra especifica aprovada
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Repositorios
                Repositorio.Embarcador.Frete.AjusteTabelaFreteAutorizacao repAjusteTabelaFreteAutorizacao = new Repositorio.Embarcador.Frete.AjusteTabelaFreteAutorizacao(unitOfWork);

                // Codigo requisicao
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Entidades
                Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteAutorizacao ajusteAutorizacao = repAjusteTabelaFreteAutorizacao.BuscarPorCodigo(codigo);

                // Valida se é o usuario da regra
                if (ajusteAutorizacao == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados.");

                if (ajusteAutorizacao.Bloqueada)
                    return new JsonpResult(false, "Não é possível aprovar essa alçada antes que as alçadas de os níveis anteriores sejam aprovadas.");

                // Valida a situacao
                if (ajusteAutorizacao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFreteAutorizacao.Pendente)
                    return new JsonpResult(false, "A situação da aprovação não permite alterações da mesma.");

                // Inicia transacao
                unitOfWork.Start();

                // Chama metodo de aprovacao
                EfetuarAprovacao(ajusteAutorizacao, true, unitOfWork);

                // Finaliza transacao
                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Rejeitar()
        {
            // Recebe o codigo da regra especifica aprovada
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Repositorios
                Repositorio.Embarcador.Frete.AjusteTabelaFreteAutorizacao repAjusteTabelaFreteAutorizacao = new Repositorio.Embarcador.Frete.AjusteTabelaFreteAutorizacao(unitOfWork);
                Repositorio.Embarcador.Frete.MotivoRejeicaoAjuste repMotivoRejeicaoAjuste = new Repositorio.Embarcador.Frete.MotivoRejeicaoAjuste(unitOfWork);

                // Codigo da regra
                int codigo = Request.GetIntParam("Codigo");

                string motivo = !string.IsNullOrWhiteSpace(Request.Params("Motivo")) ? Request.Params("Motivo") : string.Empty;

                // Entidades
                Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteAutorizacao ajusteAutorizacao = repAjusteTabelaFreteAutorizacao.BuscarPorCodigo(codigo);
                Dominio.Entidades.Embarcador.Frete.MotivoRejeicaoAjuste motivoRejeicao = repMotivoRejeicaoAjuste.BuscarPorCodigo(Request.GetIntParam("MotivoRejeicao"));

                // Valida se é o usuario da regra
                if (ajusteAutorizacao == null || ajusteAutorizacao.Usuario?.Codigo != this.Usuario.Codigo)
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados.");

                if (motivoRejeicao == null)
                    return new JsonpResult(false, "Erro ao buscar motivo.");

                //if (string.IsNullOrWhiteSpace(motivo))
                //    return new JsonpResult(false, "Observação é obrigatório.");

                // Valida a situacao
                if (ajusteAutorizacao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFreteAutorizacao.Pendente)
                    return new JsonpResult(false, "A situação da aprovação não permite alterações da mesma.");

                // Inicia transacao
                unitOfWork.Start();

                // Seta com aprovado e coloca informacoes do evento
                ajusteAutorizacao.Data = DateTime.Now;
                ajusteAutorizacao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFreteAutorizacao.Rejeitada;
                ajusteAutorizacao.Motivo = motivo;
                ajusteAutorizacao.MotivoRejeicao = motivoRejeicao;

                // Atualiza banco
                repAjusteTabelaFreteAutorizacao.Atualizar(ajusteAutorizacao);

                // Verifica status gerais
                this.NotificarAlteracao(false, ajusteAutorizacao.AjusteTabelaFrete, unitOfWork);
                this.VerificarSituacaoAjuste(ajusteAutorizacao.AjusteTabelaFrete, unitOfWork);

                // Finaliza transacao
                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> PesquisaAlteracoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoAjusteTabelaFrete = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frete.AjusteTabelaFrete repositorioAjusteTabelaFrete = new Repositorio.Embarcador.Frete.AjusteTabelaFrete(unitOfWork);
                Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = repositorioAjusteTabelaFrete.BuscarPorCodigo(codigoAjusteTabelaFrete).TabelaFrete;
                Models.Grid.Grid grid = GridPadrao(tabelaFrete);

                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteCliente filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteCliente()
                {
                    ParametroBase = tabelaFrete.ParametroBase,
                    CodigoTabelaFrete = tabelaFrete.Codigo,
                    CodigoAjusteTabelaFrete = codigoAjusteTabelaFrete,
                    TipoRegistro = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRegistroAjusteTabelaFrete.Alterados
                };

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades = (
                    from head in grid.header
                    select new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento
                    {
                        Propriedade = head.data,
                        CodigoDinamico = head.dynamicCode
                    }
                ).ToList();

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                int totalRegistros = repositorioTabelaFreteCliente.ContarConsulta(filtrosPesquisa, propriedades);
                IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.ConsultaTabelaFrete> lista = (totalRegistros > 0) ? repositorioTabelaFreteCliente.Consultar(filtrosPesquisa, propriedades, parametrosConsulta) : new List<Dominio.Relatorios.Embarcador.DataSource.Fretes.ConsultaTabelaFrete>();

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

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

        public async Task<IActionResult> PesquisaValoresAlteradosTabelaFreteSelecionadas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<int> codigosTabelaFreteAjustes = Request.GetListParam<int>("Codigos");
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                if (codigosTabelaFreteAjustes.Count == 0)
                {
                    grid.AdicionaRows(new List<Dominio.Relatorios.Embarcador.DataSource.Fretes.ConsultaTabelaFrete>());
                    grid.setarQuantidadeTotal(0);
                    return new JsonpResult(grid);
                }

                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Repositorio.Embarcador.Frete.AjusteTabelaFrete repositorioTabelaFreteAjuste = new Repositorio.Embarcador.Frete.AjusteTabelaFrete(unitOfWork);
                Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unitOfWork);
                List<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete> tabelasFreteAlteracao = repositorioTabelaFreteAjuste.BuscarPorCodigos(codigosTabelaFreteAjustes);
                List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> tabelasFrete = tabelasFreteAlteracao.Select(alteracao => alteracao.TabelaFrete).Distinct().ToList();

                if (tabelasFrete.Count > 1)
                    throw new ControllerException("Não é possível selecionar tabelas de frete diferentes");

                Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete repositorioConfiguracaoTabelaFrete = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTabelaFrete(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTabelaFrete configuracaoTabelaFrete = repositorioConfiguracaoTabelaFrete.BuscarPrimeiroRegistro();
                List<int> codigosTabelasFreteCliente = repositorioTabelaFreteCliente.BuscarCodigosTabelasFreteClientePorAjuste(codigosTabelaFreteAjustes);

                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteCliente filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteCliente()
                {
                    ParametroBase = tabelasFrete.FirstOrDefault().ParametroBase,
                    CodigosTabelasFreteCliente = codigosTabelasFreteCliente,
                    CodigosAjustesTabelaFrete = codigosTabelaFreteAjustes,
                    TipoRegistro = TipoRegistroAjusteTabelaFrete.Alterados
                };

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                int totalRegistros = repositorioTabelaFreteCliente.ContarConsulta(filtrosPesquisa, propriedades);
                IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.ConsultaTabelaFrete> lista = totalRegistros > 0 ? repositorioTabelaFreteCliente.Consultar(filtrosPesquisa, propriedades, parametrosConsulta) : new List<Dominio.Relatorios.Embarcador.DataSource.Fretes.ConsultaTabelaFrete>();

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (BaseException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os valores alterados da tabela de frete.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        
        public async Task<IActionResult> BuscarCodigosTabelaFreteAjustesSelecionados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<int> codigosTabelaFreteAjustes = ObterCodigosAjustesSelecionados(unitOfWork);

                return new JsonpResult(codigosTabelaFreteAjustes);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os registros selecionados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid GridPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Tabela", "Tabela", 15, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Data", "DataCriacao", 6, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Etapa do Ajuste", "Etapa", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Tempo na Etapa", "TempoEtapa", 6, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Criador", "Criador", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Responsável", "Responsavel", 15, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Aprovadores Pendentes", "AprovadoresPendentes", 15, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Aprovações", "Aprovacoes", 6, Models.Grid.Align.center, false);

            return grid;
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Criador")
                return "Criador.Nome";

            return propriedadeOrdenar;
        }

        private void EfetuarAprovacao(Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteAutorizacao ajuesteAutorizacao, bool verificarSeEstaAprovado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.AjusteTabelaFreteAutorizacao repAjusteTabelaFreteAutorizacao = new Repositorio.Embarcador.Frete.AjusteTabelaFreteAutorizacao(unitOfWork);

            // So modifica a autorizacao quando ela for pendente
            if (ajuesteAutorizacao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFreteAutorizacao.Pendente && ajuesteAutorizacao.Usuario?.Codigo == this.Usuario.Codigo)
            {
                // Seta com aprovado e adiciona a hora do evento
                ajuesteAutorizacao.Data = DateTime.Now;
                ajuesteAutorizacao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFreteAutorizacao.Aprovada;

                // Atualiza os dados
                repAjusteTabelaFreteAutorizacao.Atualizar(ajuesteAutorizacao);

                // Faz verificacao se a carga esta aprovada
                if (verificarSeEstaAprovado)
                    this.VerificarSituacaoAjuste(ajuesteAutorizacao.AjusteTabelaFrete, unitOfWork);

                // Notifica usuario que criou o item
                this.NotificarAlteracao(true, ajuesteAutorizacao.AjusteTabelaFrete, unitOfWork);
            }
        }

        private List<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteAutorizacao> BuscarRegrasPorAjueste(List<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete> ajustes, int usuario, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.AjusteTabelaFreteAutorizacao repAjusteTabelaFreteAutorizacao = new Repositorio.Embarcador.Frete.AjusteTabelaFreteAutorizacao(unitOfWork);
            List<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteAutorizacao> ajustesAutorizacao = new List<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteAutorizacao>();

            // Itera todas as itens
            foreach (Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete ajuste in ajustes)
            {
                // Busca as autorizacoes
                List<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteAutorizacao> regras = repAjusteTabelaFreteAutorizacao.BuscarPorCodigoAjusteEUsuario(ajuste.Codigo, usuario);

                // Adiciona na lista
                ajustesAutorizacao.AddRange(regras);
            }

            // Retornas a lista com todas as autorizacao
            return ajustesAutorizacao;
        }

        private List<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete> ObterAjuestesSelecionadas(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.AjusteTabelaFrete repAjusteTabelaFrete = new Repositorio.Embarcador.Frete.AjusteTabelaFrete(unitOfWork);
            List<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete> listaAjuestes = new List<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete>();

            bool todosSelecionados = false;
            bool.TryParse(Request.Params("SelecionarTodos"), out todosSelecionados);

            if (todosSelecionados)
            {
                // Reconsulta com os mesmos dados e remove apenas os desselecionados
                try
                {
                    int totalRegistros = 0;
                    Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
                    {
                        PropriedadeOrdenar = "Codigo"
                    };

                    ExecutaPesquisa(ref listaAjuestes, ref totalRegistros, parametrosPesquisa, unitOfWork);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    new Exception("Erro ao converte dados.");
                }

                // Iterar itens desselecionados e remove da lista
                dynamic listaAjustesNaoSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("AjustesNaoSelecionados"));
                foreach (var dynAjustesNaoSelecionado in listaAjustesNaoSelecionados)
                    listaAjuestes.Remove(new Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete() { Codigo = (int)dynAjustesNaoSelecionado.Codigo });
            }
            else
            {
                // Busca apenas itens selecionados
                dynamic listaAjustesSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("AjustesSelecionados"));
                foreach (var dynAjustesSelecionado in listaAjustesSelecionados)
                    listaAjuestes.Add(repAjusteTabelaFrete.BuscarPorCodigo((int)dynAjustesSelecionado.Codigo));
            }

            // Retorna lista
            return listaAjuestes;
        }

        private void NotificarAlteracao(bool aprovada, Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete ajuste, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(_conexao.StringConexao, null, TipoServicoMultisoftware, string.Empty);

                // Define icone
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao icone;
                if (aprovada)
                    icone = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.confirmado;
                else
                    icone = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.rejeitado;

                // Emite notificação
                string mensagem = string.Format(Localization.Resources.Fretes.AutorizacaoControleReajusteFretePlanilha.UsuarioReajusteFrete, (aprovada ? Localization.Resources.Gerais.Geral.Aprovou : Localization.Resources.Gerais.Geral.Rejeitou));
                if (ajuste.Criador != null)
                    serNotificacao.GerarNotificacao(ajuste.Criador, this.Usuario, ajuste.Codigo, "Fretes/ControleReajusteFretePlanilha", mensagem, icone, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito, TipoServicoMultisoftware, unitOfWork);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        private bool VerificarSituacaoAjuste(Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete ajuste, Repositorio.UnitOfWork unitOfWork)
        {
            bool retorno = true;

            try
            {
                if (ajuste.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFrete.AgAprovacao)
                {
                    Repositorio.Embarcador.Frete.AjusteTabelaFreteAutorizacao repositorioAjusteTabelaFreteAutorizacao = new Repositorio.Embarcador.Frete.AjusteTabelaFreteAutorizacao(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Frete.RegrasAutorizacaoTabelaFrete> regras = repositorioAjusteTabelaFreteAutorizacao.BuscarRegrasDesbloqueadas(ajuste.Codigo);
                    bool rejeitada = false;
                    bool aprovada = true;

                    foreach (Dominio.Entidades.Embarcador.Frete.RegrasAutorizacaoTabelaFrete regra in regras)
                    {
                        int pendentes = repositorioAjusteTabelaFreteAutorizacao.ContarPendentes(ajuste.Codigo, regra.Codigo);
                        int aprovacoes = repositorioAjusteTabelaFreteAutorizacao.ContarAprovacoesAjuste(ajuste.Codigo, regra.Codigo);
                        int rejeitadas = repositorioAjusteTabelaFreteAutorizacao.ContarRejeitadas(ajuste.Codigo, regra.Codigo);
                        int necessariosParaAprovar = regra.NumeroAprovadores;

                        if (rejeitadas > 0)
                            rejeitada = true;
                        else if (aprovacoes < necessariosParaAprovar)
                            aprovada = false;
                    }

                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFrete situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFrete.AgAprovacao;
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFrete? situacaoAposProcessamento = null;

                    if (rejeitada)
                        situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFrete.RejeitadaAutorizacao;
                    else if (aprovada)
                    {
                        situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFrete.EmProcessamento;
                        situacaoAposProcessamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFrete.Finalizado;
                        aprovada = Servicos.Embarcador.Frete.AutorizacaoAjusteTabelaFrete.LiberarProximasHierarquiasDeAprovacao(ajuste, this.Usuario,TipoServicoMultisoftware, _conexao.StringConexao, unitOfWork);
                    }

                    if (rejeitada || aprovada)
                    {
                        Repositorio.Embarcador.Frete.AjusteTabelaFrete repositorioAjusteTabelaFrete = new Repositorio.Embarcador.Frete.AjusteTabelaFrete(unitOfWork);

                        ajuste.Situacao = situacao;
                        ajuste.SituacaoAposProcessamento = situacaoAposProcessamento;
                        ajuste.UsuarioAprovador = this.Usuario;

                        repositorioAjusteTabelaFrete.Atualizar(ajuste);

                        Servicos.Embarcador.Notificacao.Notificacao servicoNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(_conexao.StringConexao, null, TipoServicoMultisoftware, string.Empty);
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao icone = rejeitada ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.rejeitado : Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.confirmado;
                        string mensagem = $"{Localization.Resources.Fretes.AutorizacaoControleReajusteFretePlanilha.ControleFreteFoi} {(rejeitada ? Localization.Resources.Gerais.Geral.Rejeitado : Localization.Resources.Gerais.Geral.Aprovado)}";

                        servicoNotificacao.GerarNotificacao(ajuste.Criador, this.Usuario, ajuste.Codigo, "Fretes/ControleReajusteFretePlanilha", mensagem, icone, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito, TipoServicoMultisoftware, unitOfWork);
                    }
                }
            }
            catch (Exception excecao)
            {
                retorno = false;
                Servicos.Log.TratarErro(excecao);
            }
            return retorno;
        }

        private string CoresRegras(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFreteAutorizacao situacao)
        {
            switch (situacao)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFreteAutorizacao.Aprovada:
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Success;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFreteAutorizacao.Rejeitada:
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Danger;
                default:
                    return "";
            }
        }

        private Models.Grid.Grid GridPadrao(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete)
        {
            UltimaColunaDinamica = 1;

            Models.Grid.Grid grid = new Models.Grid.Grid();
            grid.header = new List<Models.Grid.Head>();


            grid.AdicionarCabecalho("Código", "CodigoIntegracao", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tabela de Frete", "TabelaFrete", TamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, false, false);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                grid.AdicionarCabecalho("CNPJ Transportador", "CNPJTransportador", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
                grid.AdicionarCabecalho("Transportador", "Transportador", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            }

            if (tabelaFrete.ParametroBase.HasValue)
                grid.AdicionarCabecalho("Base (" + tabelaFrete.ParametroBase.Value.ObterDescricao() + ")", "ParametroBase", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);

            grid.AdicionarCabecalho("Inicio Vigência", "DataInicial", TamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Fim Vigência", "DataFinal", TamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, true, false, false, false, false);

            if (tabelaFrete.NumeroEntregas.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.NumeroEntrega))
            {
                grid.AdicionarCabecalho("Entrega", "NumeroEntrega", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Entrega", "DescricaoValorEntrega", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
            }

            if (tabelaFrete.TipoEmbalagens.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.TipoEmbalagem))
            {
                grid.AdicionarCabecalho("Tipo de Embalagem", "TipoEmbalagem", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Tipo de Embalagem", "DescricaoValorTipoEmbalagem", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
            }

            if (tabelaFrete.PesosTransportados.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Peso))
            {
                grid.AdicionarCabecalho("Peso", "DescricaoPeso", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Peso", "DescricaoValorPeso", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
            }

            if (tabelaFrete.Distancias.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Distancia))
            {
                grid.AdicionarCabecalho("Distância", "DescricaoDistancia", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Distância", "DescricaoValorDistancia", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
            }

            if (tabelaFrete.TiposCarga.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.TipoCarga))
            {
                grid.AdicionarCabecalho("Tipo Carga", "TipoCarga", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Tipo Carga", "DescricaoValorTipoCarga", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
            }

            if (tabelaFrete.ModelosReboque.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ModeloReboque))
            {
                grid.AdicionarCabecalho("Reboque", "ModeloReboque", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Reboque", "DescricaoValorModeloReboque", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
            }

            if (tabelaFrete.ModelosTracao.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ModeloTracao))
            {
                grid.AdicionarCabecalho("Tração", "ModeloTracao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Tração", "DescricaoValorModeloTracao", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
            }

            if (tabelaFrete.Ajudantes.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Ajudante))
            {
                grid.AdicionarCabecalho("Ajudante", "NumeroAjudante", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Ajudante", "DescricaoValorAjudante", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
            }

            if (tabelaFrete.Pallets.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Pallets))
            {
                grid.AdicionarCabecalho("Pallet", "NumeroPallets", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Pallet", "DescricaoValorPallets", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
            }

            if (tabelaFrete.Tempos.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Tempo))
            {
                grid.AdicionarCabecalho("Tempo", "HoraTempo", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Tempo", "DescricaoValorTempo", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
            }

            for (int i = 0; i < tabelaFrete.Componentes.Count; i++)
            {
                if (i < NumeroMaximoComplementos)
                {
                    grid.AdicionarCabecalho(tabelaFrete.Componentes[i].ComponenteFrete.Descricao, "DescricaoValorComponente" + UltimaColunaDinamica.ToString(), TamanhoColunasValores, Models.Grid.Align.right, true, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.nenhum, tabelaFrete.Componentes[i].Codigo);
                    UltimaColunaDinamica++;
                }
                else
                {
                    break;
                }
            }

            if (tabelaFrete.PossuiMinimoGarantido)
                grid.AdicionarCabecalho("Valor Mínimo", "ValorMinimo", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);

            if (tabelaFrete.PossuiValorMaximo)
                grid.AdicionarCabecalho("Valor Máximo", "ValorMaximo", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);

            if (tabelaFrete.PossuiValorBase)
                grid.AdicionarCabecalho("Valor Base", "ValorBase", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);

            return grid;
        }

        private void ExecutaPesquisa(ref List<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete> listaAjustes, ref int totalRegistros, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosPesquisa, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaAjusteTabelaFreteAprovacao filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaAjusteTabelaFreteAprovacao()
            {
                CodigoTabelaFrete = Request.GetIntParam("Tabela"),
                CodigoTipoOperacao = Request.GetIntParam("TipoOperacao"),
                CodigoTransportador = Request.GetIntParam("Transportador"),
                CodigoUsuario = Request.GetIntParam("Usuario"),
                DataFim = Request.GetNullableDateTimeParam("DataFim"),
                DataInicio = Request.GetNullableDateTimeParam("DataInicio"),
                EtapaAjuste = Request.GetEnumParam("EtapaAjuste", EtapaAjusteTabelaFrete.Todas),
                EtapaAutorizacao = Request.GetEnumParam<EtapaAutorizacaoTabelaFrete>("EtapaAutorizacao"),
                Situacao = Request.GetEnumParam<SituacaoAjusteTabelaFrete>("Situacao")
            };

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                filtrosPesquisa.CodigoUsuario = Usuario.Codigo;
                filtrosPesquisa.TipoAprovadorRegra = TipoAprovadorRegra.Transportador;
            }
            else
                filtrosPesquisa.TipoAprovadorRegra = TipoAprovadorRegra.Usuario;

            Repositorio.Embarcador.Frete.AjusteTabelaFreteAutorizacao repositorioAjusteTabelaFreteAutorizacao = new Repositorio.Embarcador.Frete.AjusteTabelaFreteAutorizacao(unitOfWork);
            totalRegistros = repositorioAjusteTabelaFreteAutorizacao.ContarConsulta(filtrosPesquisa);
            listaAjustes = totalRegistros > 0 ? repositorioAjusteTabelaFreteAutorizacao.Consultar(filtrosPesquisa, parametrosPesquisa) : new List<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete>();
        }

        private dynamic RetornaDyn(List<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete> listaAjustes, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancias
            Repositorio.Embarcador.Frete.AjusteTabelaFrete repAjusteTabelaFrete = new Repositorio.Embarcador.Frete.AjusteTabelaFrete(unitOfWork);
            Repositorio.Embarcador.Frete.TempoEtapaAjusteTabelaFrete repTempoEtapaAjusteTabelaFrete = new Repositorio.Embarcador.Frete.TempoEtapaAjusteTabelaFrete(unitOfWork);
            Repositorio.Embarcador.Frete.AjusteTabelaFreteAutorizacao repAjusteTabelaFreteAutorizacao = new Repositorio.Embarcador.Frete.AjusteTabelaFreteAutorizacao(unitOfWork);

            var lista = from ajustes in listaAjustes
                        select new
                        {
                            ajustes.Codigo,
                            Tabela = ajustes.TabelaFrete.Descricao,
                            DataCriacao = ajustes.DataCriacao.ToString("dd/MM/yyyy"),
                            Situacao = ajustes.DescricaoSituacao,
                            Etapa = ajustes.DescricaoEtapa,
                            TempoEtapa = repTempoEtapaAjusteTabelaFrete.BuscarTempoAjuste(ajustes.Codigo),
                            Criador = ajustes.Criador?.Nome ?? string.Empty,
                            Responsavel = String.Join(", ", (from o in repAjusteTabelaFrete.Responsavel(ajustes.Codigo) select o.Nome)),
                            AprovadoresPendentes = String.Join(", ", (from o in ProximosResponsaveis(ajustes, unitOfWork) select o.Nome)),
                            Aprovacoes = repAjusteTabelaFreteAutorizacao.ContarAprovacoesPorAjuste(ajustes.Codigo) + " de " + repAjusteTabelaFreteAutorizacao.ContarAutorizacaoPorAjuste(ajustes.Codigo),
                        };

            return lista.ToList();
        }

        private List<Dominio.Entidades.Usuario> ProximosResponsaveis(Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete ajusteTabelaFrete, Repositorio.UnitOfWork unitOfWork)
        {
            if (ajusteTabelaFrete.Situacao != SituacaoAjusteTabelaFrete.AgAprovacao)
                return new List<Dominio.Entidades.Usuario>();

            Repositorio.Embarcador.Frete.AjusteTabelaFreteAutorizacao repositorioAutorizacao = new Repositorio.Embarcador.Frete.AjusteTabelaFreteAutorizacao(unitOfWork);
            List<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteAutorizacao> alcadasAprovacao = repositorioAutorizacao.BuscarPendentesDesbloqueadas(ajusteTabelaFrete.Codigo);

            if (alcadasAprovacao.Count == 0)
                return new List<Dominio.Entidades.Usuario>();
            
            int maiorPrioridadeAprovacaoDesbloqueada = alcadasAprovacao.Select(obj => obj.RegrasAutorizacaoTabelaFrete.PrioridadeAprovacao).Max();

            return alcadasAprovacao.Where(o => o.RegrasAutorizacaoTabelaFrete.PrioridadeAprovacao == maiorPrioridadeAprovacaoDesbloqueada).Select(o => o.Usuario).ToList();
        }

        private List<int> ObterCodigosAjustesSelecionados(Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete> listaTabelaFreteAjustes = new List<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete>();
            var listaItensNaoSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensNaoSelecionados"));
            var selecionarTodos = Request.GetBoolParam("SelecionarTodos");

            if (selecionarTodos)
            {
                int totalRegistros = 0;
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
                {
                    PropriedadeOrdenar = "Codigo"
                };

                ExecutaPesquisa(ref listaTabelaFreteAjustes, ref totalRegistros, parametrosPesquisa, unitOfWork);

                foreach (var itemNaoSelecionado in listaItensNaoSelecionados)
                    listaTabelaFreteAjustes.Remove(new Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete() { Codigo = (int)itemNaoSelecionado.Codigo });
            }
            else
            {
                Repositorio.Embarcador.Frete.AjusteTabelaFrete repositorioTabelaFreteAjuste = new Repositorio.Embarcador.Frete.AjusteTabelaFrete(unitOfWork);
                var listaItensSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensSelecionados"));

                listaTabelaFreteAjustes = new List<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete>();

                foreach (var itemSelecionado in listaItensSelecionados)
                    listaTabelaFreteAjustes.Add(repositorioTabelaFreteAjuste.BuscarPorCodigo((int)itemSelecionado.Codigo, auditavel: false));
            }

            return (from tabelaFreteAljuste in listaTabelaFreteAjustes select tabelaFreteAljuste.Codigo).ToList();
        }
        #endregion
    }
}

