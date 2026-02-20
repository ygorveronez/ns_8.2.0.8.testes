using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Terceiros
{
    [CustomAuthorize("Terceiros/FechamentoAgregado")]
    public class FechamentoAgregadoController : BaseController
    {
		#region Construtores

		public FechamentoAgregadoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                if (!PreencherEntidade(out Dominio.Entidades.Embarcador.Terceiros.FechamentoAgregado fechamentoAgregado, out string mensagemErro, unitOfWork))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, mensagemErro);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(RetornaDynAdicionar(fechamentoAgregado, false, SituacaoCIOT.Aberto, unitOfWork));
            }
            catch (ServicoException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa();

                // Ordenacao da grid
                string propOrdena = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdena);

                List<Dominio.Entidades.Embarcador.Terceiros.FechamentoAgregado> lista = new List<Dominio.Entidades.Embarcador.Terceiros.FechamentoAgregado>();

                // Variavel com o numero total de resultados
                int totalRegistro = 0;

                // Executa metodo de consutla
                ExecutaPesquisa(ref lista, ref totalRegistro, propOrdena, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Converte os dados recebidos
                var listaProcessada = RetornaDyn(lista);

                // Retorna Grid
                grid.AdicionaRows(listaProcessada);
                grid.setarQuantidadeTotal(totalRegistro);

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

        public async Task<IActionResult> PesquisaCIOT()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisaCIOT();

                // Ordenacao da grid
                string propOrdena = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdena);

                List<Dominio.Entidades.Embarcador.Documentos.CIOT> lista = new List<Dominio.Entidades.Embarcador.Documentos.CIOT>();

                // Variavel com o numero total de resultados
                int totalRegistro = 0;

                // Executa metodo de consutla
                ExecutaPesquisaCIOT(ref lista, ref totalRegistro, propOrdena, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Converte os dados recebidos
                var listaProcessada = RetornaDynCIOT(lista);

                // Retorna Grid
                grid.AdicionaRows(listaProcessada);
                grid.setarQuantidadeTotal(totalRegistro);

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

        public async Task<IActionResult> PesquisaCIOTConsolidacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisaCIOTConsolidacao();

                // Ordenacao da grid
                string propOrdena = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdena);

                List<Dominio.Entidades.Embarcador.Terceiros.FechamentoAgregadoCIOT> lista = new List<Dominio.Entidades.Embarcador.Terceiros.FechamentoAgregadoCIOT>();

                // Variavel com o numero total de resultados
                int totalRegistro = 0;

                // Executa metodo de consutla
                ExecutaPesquisaCIOTConsolidacao(ref lista, ref totalRegistro, propOrdena, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Converte os dados recebidos
                var listaProcessada = RetornaDynCIOTConsolidacao(lista, unitOfWork);

                // Retorna Grid
                grid.AdicionaRows(listaProcessada);
                grid.setarQuantidadeTotal(totalRegistro);

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

        public async Task<IActionResult> PesquisaDetalhes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisaDetalhes();

                // Ordenacao da grid
                string propOrdena = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdena);

                List<Dominio.Entidades.Embarcador.Cargas.Carga> lista = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();

                // Variavel com o numero total de resultados
                int totalRegistro = 0;

                // Executa metodo de consutla
                ExecutaPesquisaDetalhes(ref lista, ref totalRegistro, propOrdena, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Converte os dados recebidos
                var listaProcessada = RetornaDynDetalhes(lista, unitOfWork);

                // Retorna Grid
                grid.AdicionaRows(listaProcessada);
                grid.setarQuantidadeTotal(totalRegistro);

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

        public async Task<IActionResult> PesquisaAcrescimo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisaAcrescimoDesconto();

                // Ordenacao da grid
                string propOrdena = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdena);

                List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto> lista = new List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto>();

                // Variavel com o numero total de resultados
                int totalRegistro = 0;

                // Executa metodo de consutla
                ExecutaPesquisaAcrescimoDesconto(ref lista, TipoJustificativa.Acrescimo, ref totalRegistro, propOrdena, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Converte os dados recebidos
                var listaProcessada = RetornaDynAcrescimoDesconto(lista);

                // Retorna Grid
                grid.AdicionaRows(listaProcessada);
                grid.setarQuantidadeTotal(totalRegistro);

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

        public async Task<IActionResult> PesquisaDesconto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisaAcrescimoDesconto();

                // Ordenacao da grid
                string propOrdena = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdena);

                List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto> lista = new List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto>();

                // Variavel com o numero total de resultados
                int totalRegistro = 0;

                // Executa metodo de consutla
                ExecutaPesquisaAcrescimoDesconto(ref lista, TipoJustificativa.Desconto, ref totalRegistro, propOrdena, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Converte os dados recebidos
                var listaProcessada = RetornaDynAcrescimoDesconto(lista);

                // Retorna Grid
                grid.AdicionaRows(listaProcessada);
                grid.setarQuantidadeTotal(totalRegistro);

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

        public async Task<IActionResult> PesquisaIntegracaoCIOT()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisaIntegracaoCIOT();

                // Ordenacao da grid
                string propOrdena = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdena);

                List<Dominio.Entidades.Embarcador.Terceiros.PagamentoContratoIntegracao> lista = new List<Dominio.Entidades.Embarcador.Terceiros.PagamentoContratoIntegracao>();

                // Variavel com o numero total de resultados
                int totalRegistro = 0;

                // Executa metodo de consulta
                ExecutaPesquisaIntegracaoCIOT(ref lista, ref totalRegistro, propOrdena, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Converte os dados recebidos
                var listaProcessada = RetornaDynIntegracaoCIOT(lista);

                // Retorna Grid
                grid.AdicionaRows(listaProcessada);
                grid.setarQuantidadeTotal(totalRegistro);

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
                PropOrdena(ref propOrdenar);

                List<Dominio.Entidades.Embarcador.Terceiros.FechamentoAgregado> lista = new List<Dominio.Entidades.Embarcador.Terceiros.FechamentoAgregado>();

                // Variavel com o numero total de resultados
                int totalRegistro = 0;

                // Executa metodo de consutla
                ExecutaPesquisa(ref lista, ref totalRegistro, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Converte os dados recebidos
                var listaProcessada = RetornaDyn(lista);

                // Seta valores na grid
                grid.AdicionaRows(listaProcessada);

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

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisaCIOT()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisaCIOT();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                List<Dominio.Entidades.Embarcador.Documentos.CIOT> lista = new List<Dominio.Entidades.Embarcador.Documentos.CIOT>();

                // Variavel com o numero total de resultados
                int totalRegistro = 0;

                // Executa metodo de consutla
                ExecutaPesquisaCIOT(ref lista, ref totalRegistro, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Converte os dados recebidos
                var listaProcessada = RetornaDynCIOT(lista);

                // Seta valores na grid
                grid.AdicionaRows(listaProcessada);

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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Repositorios
                Repositorio.Embarcador.Terceiros.FechamentoAgregado repFechamentoAgregado = new Repositorio.Embarcador.Terceiros.FechamentoAgregado(unitOfWork);
                Repositorio.Embarcador.Terceiros.FechamentoAgregadoCIOT repFechamentoAgregadoCIOT = new Repositorio.Embarcador.Terceiros.FechamentoAgregadoCIOT(unitOfWork);

                // Codigo requisicao
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Entidades
                Dominio.Entidades.Embarcador.Terceiros.FechamentoAgregado fechamentoAgregado = repFechamentoAgregado.BuscarPorCodigo(codigo);

                if (fechamentoAgregado == null)
                    return new JsonpResult(false, "Fechamento de agregado não encontrado.");

                bool consolidado = true;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT situacaoCIOT = SituacaoCIOT.Encerrado;

                if (fechamentoAgregado.CIOT != null)
                {
                    consolidado = fechamentoAgregado.Consolidado;
                    situacaoCIOT = fechamentoAgregado.CIOT.Situacao;
                }
                else
                {
                    List<Dominio.Entidades.Embarcador.Terceiros.FechamentoAgregadoCIOT> fechamentoAgregadoCIOTs = repFechamentoAgregadoCIOT.BuscarPorCodigoFechamentoAgregado(fechamentoAgregado.Codigo);

                    if (fechamentoAgregadoCIOTs.FirstOrDefault(o => !o.Consolidado) != null)
                        consolidado = false;

                    if (fechamentoAgregadoCIOTs.FirstOrDefault(o => o.CIOT.Situacao != SituacaoCIOT.Encerrado) != null)
                        situacaoCIOT = SituacaoCIOT.Aberto;
                }

                return new JsonpResult(RetornaDynAdicionar(fechamentoAgregado, consolidado, situacaoCIOT, unitOfWork));
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

        public async Task<IActionResult> EncerrarCIOTAgregado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Repositorios
                Repositorio.Embarcador.Terceiros.FechamentoAgregado repFechamentoAgregado = new Repositorio.Embarcador.Terceiros.FechamentoAgregado(unitOfWork);
                Repositorio.Embarcador.Terceiros.FechamentoAgregadoCIOT repFechamentoAgregadoCIOT = new Repositorio.Embarcador.Terceiros.FechamentoAgregadoCIOT(unitOfWork);
                Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto repContratoFreteAcrescimoDesconto = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto(unitOfWork);
                Servicos.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto serContratoFreteAcrescimoDesconto = new Servicos.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto(unitOfWork);
                Servicos.Embarcador.CIOT.CIOT svcCIOT = new Servicos.Embarcador.CIOT.CIOT();
                Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);

                // Codigo requisicao
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Entidades
                Dominio.Entidades.Embarcador.Terceiros.FechamentoAgregado fechamentoAgregado = repFechamentoAgregado.BuscarPorCodigo(codigo);

                if (fechamentoAgregado == null)
                    return new JsonpResult(false, "Fechamento de agregado não encontrado.");

                if (fechamentoAgregado.CIOT != null)
                {
                    #region Processo descontinuado mantido para compatibilidade dos registros legados

                    if (fechamentoAgregado.CIOT.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto)
                        return new JsonpResult(false, true, "O CIOT não pode ser encerrado na situação atual.");

                    #region Aplicar Valores de Acrescimo e Descrescimo nos contratos de frete.

                    if (!fechamentoAgregado.Consolidado)
                    {
                        unitOfWork.Start();

                        var listaContratoFreteAcrescimoDesconto = repContratoFreteAcrescimoDesconto.BuscarPorCIOT(fechamentoAgregado.CIOT.Codigo);

                        foreach (var contratoFreteAcrescimoDesconto in listaContratoFreteAcrescimoDesconto)
                        {
                            if (contratoFreteAcrescimoDesconto.ValorAplicado)
                                continue;

                            if (contratoFreteAcrescimoDesconto.Situacao != SituacaoContratoFreteAcrescimoDesconto.Aprovado && 
                                contratoFreteAcrescimoDesconto.Situacao != SituacaoContratoFreteAcrescimoDesconto.Finalizado)
                                throw new ServicoException("Existe acréscimo/desconto não aprovado, favor verificar os acréscimos/desconto.");

                            serContratoFreteAcrescimoDesconto.AplicarValorNoContratoFrete(contratoFreteAcrescimoDesconto, TipoServicoMultisoftware, Auditado, false);

                            contratoFreteAcrescimoDesconto.Situacao = SituacaoContratoFreteAcrescimoDesconto.Finalizado;
                            repContratoFreteAcrescimoDesconto.Atualizar(contratoFreteAcrescimoDesconto);
                        }

                        fechamentoAgregado.Consolidado = true;
                        repFechamentoAgregado.Atualizar(fechamentoAgregado);

                        unitOfWork.CommitChanges();
                    }

                    #endregion

                    #region Encerrar CIOT Agregado

                    string mensagemErro = "";

                    if (!svcCIOT.EncerrarCIOT(fechamentoAgregado.CIOT, unitOfWork, TipoServicoMultisoftware, out mensagemErro))
                        return new JsonpResult(false, true, mensagemErro);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, fechamentoAgregado.CIOT, null, "Encerrou o CIOT Fechamento Agregado.", unitOfWork);

                    #endregion

                    #endregion
                }
                else
                {
                    List<Dominio.Entidades.Embarcador.Terceiros.FechamentoAgregadoCIOT> fechamentoAgregadoCIOTs = repFechamentoAgregadoCIOT.BuscarPorCodigoFechamentoAgregado(fechamentoAgregado.Codigo);

                    if (fechamentoAgregadoCIOTs.Count() == 0)
                        return new JsonpResult(false, true, "CIOTs para encerramentos não encontrados.");

                    foreach (var fechamentoAgregadoCIOT in fechamentoAgregadoCIOTs)
                    {
                        fechamentoAgregado.CIOT = repFechamentoAgregadoCIOT.BuscarCIOTPorCodigoFechamentoAgregado(fechamentoAgregadoCIOT.Codigo);

                        if (fechamentoAgregado.CIOT.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto)
                            continue;

                        #region Aplicar Valores de Acrescimo e Descrescimo nos contratos de frete.

                        if (!fechamentoAgregadoCIOT.Consolidado)
                        {
                            unitOfWork.Start();

                            var listaContratoFreteAcrescimoDesconto = repContratoFreteAcrescimoDesconto.BuscarPorCIOT(fechamentoAgregadoCIOT.CIOT.Codigo);

                            foreach (var contratoFreteAcrescimoDesconto in listaContratoFreteAcrescimoDesconto)
                            {
                                if (contratoFreteAcrescimoDesconto.ValorAplicado)
                                    continue;

                                if (contratoFreteAcrescimoDesconto.Situacao != SituacaoContratoFreteAcrescimoDesconto.Aprovado &&
                                contratoFreteAcrescimoDesconto.Situacao != SituacaoContratoFreteAcrescimoDesconto.Finalizado)
                                    throw new ServicoException("Existe acréscimo/desconto não aprovado, favor verificar os acréscimos/desconto.");

                                serContratoFreteAcrescimoDesconto.AplicarValorNoContratoFrete(contratoFreteAcrescimoDesconto, TipoServicoMultisoftware, Auditado, false);

                                contratoFreteAcrescimoDesconto.Situacao = SituacaoContratoFreteAcrescimoDesconto.Finalizado;
                                repContratoFreteAcrescimoDesconto.Atualizar(contratoFreteAcrescimoDesconto);
                            }

                            fechamentoAgregadoCIOT.Consolidado = true;
                            repFechamentoAgregadoCIOT.Atualizar(fechamentoAgregadoCIOT);

                            unitOfWork.CommitChanges();
                        }

                        #endregion

                        #region Encerrar CIOT Agregado

                        string mensagemErro = "";

                        if (!svcCIOT.EncerrarCIOT(fechamentoAgregadoCIOT.CIOT, unitOfWork, TipoServicoMultisoftware, out mensagemErro))
                            return new JsonpResult(false, true, mensagemErro);

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, fechamentoAgregadoCIOT.CIOT, null, "Encerrou o CIOT Fechamento Agregado.", unitOfWork);

                        #endregion
                    }
                }

                return new JsonpResult(true);
            }
            catch (ServicoException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao encerrar CIOT Agregado.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RemoverVinculoContratoFreteAcrescimoDesconto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Repositorios
                Repositorio.Embarcador.Terceiros.FechamentoAgregado repFechamentoAgregado = new Repositorio.Embarcador.Terceiros.FechamentoAgregado(unitOfWork);
                Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto repContratoFreteAcrescimoDesconto = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto(unitOfWork);
                Servicos.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto serContratoFreteAcrescimoDesconto = new Servicos.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto(unitOfWork);

                // Codigo requisicao
                int.TryParse(Request.Params("Codigo"), out int codigo);
                int.TryParse(Request.Params("CodigoFechamentoAgregado"), out int codigoFechamentoAgregado);

                // Entidades
                Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto contratoFreteAcrescimoDesconto = repContratoFreteAcrescimoDesconto.BuscarPorCodigo(codigo);
                Dominio.Entidades.Embarcador.Terceiros.FechamentoAgregado fechamentoAgregado = repFechamentoAgregado.BuscarPorCodigo(codigoFechamentoAgregado);

                if (fechamentoAgregado == null)
                    return new JsonpResult(false, "Fechamento de agregado não encontrado.");

                if (contratoFreteAcrescimoDesconto == null)
                    return new JsonpResult(false, "Acréscimo/desconto não encontrado.");

                if (fechamentoAgregado.Consolidado)
                    return new JsonpResult(false, "Fechamento de agregado já foi consolidado, não é possível remover o vinculo do acréscimo/desconto");

                unitOfWork.Start();

                if (contratoFreteAcrescimoDesconto.ValorAplicado)
                    serContratoFreteAcrescimoDesconto.ReverterValorNoContratoFrete(contratoFreteAcrescimoDesconto, TipoServicoMultisoftware, Auditado, false);

                if (contratoFreteAcrescimoDesconto.ContratoFreteOriginal == null)
                    contratoFreteAcrescimoDesconto.ContratoFreteOriginal = contratoFreteAcrescimoDesconto.ContratoFrete;

                contratoFreteAcrescimoDesconto.ContratoFrete = null;
                contratoFreteAcrescimoDesconto.CIOT = null;

                repContratoFreteAcrescimoDesconto.Atualizar(contratoFreteAcrescimoDesconto);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao remover o acréscimo/desconto.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> VincularContratoFreteAcrescimoDesconto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto repContratoFreteAcrescimoDesconto = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto(unitOfWork);
                Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);

                int codigoContratoFrete = Request.GetIntParam("ContratoFrete");
                var TipoJustificativa = Request.GetEnumParam<TipoJustificativa>("TipoJustificativa");
                dynamic AcrescimosDescontos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("SalvarAcrescimosDescontos"));

                Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repContratoFrete.BuscarPorCodigo(codigoContratoFrete);

                if (contratoFrete == null)
                    throw new ServicoException("Contrato de frete não encontrado");

                if (AcrescimosDescontos.Count == 0)
                    throw new ServicoException("Acréscimo/desconto não informado.");

                unitOfWork.Start();

                foreach (var acrescimoDesconto in AcrescimosDescontos)
                {
                    Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto contratoFreteAcrescimoDesconto = repContratoFreteAcrescimoDesconto.BuscarPorCodigo((int)acrescimoDesconto.Codigo);
                    contratoFreteAcrescimoDesconto.ContratoFrete = contratoFrete;
                    
                    var cargaCIOT = repCargaCIOT.BuscarPorContrato(contratoFrete.Codigo);

                    if (cargaCIOT?.CIOT == null)
                        throw new ControllerException("Contrato de frete não está vinculado a CIOT");

                    if (cargaCIOT.CIOT.Situacao != SituacaoCIOT.Aberto)
                        throw new ControllerException("CIOT deve estar na situação aberta para inclusão de acréscimo/desconto.");

                    contratoFreteAcrescimoDesconto.CIOT = cargaCIOT.CIOT;

                    repContratoFreteAcrescimoDesconto.Atualizar(contratoFreteAcrescimoDesconto);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao vincular acréscimo/desconto.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion Métodos Globais

        #region Métodos Privados

        private bool PreencherEntidade(out Dominio.Entidades.Embarcador.Terceiros.FechamentoAgregado fechamentoAgregado, out string mensagemErro, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Terceiros.FechamentoAgregado repFechamentoAgregado = new Repositorio.Embarcador.Terceiros.FechamentoAgregado(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);

            fechamentoAgregado = new Dominio.Entidades.Embarcador.Terceiros.FechamentoAgregado();
            fechamentoAgregado.Numero = repFechamentoAgregado.BuscarProximoNumero();
            fechamentoAgregado.CIOT = null;
            fechamentoAgregado.Consolidado = false;
            fechamentoAgregado.DataCriacao = DateTime.Now;
            fechamentoAgregado.Usuario = this.Usuario;

            repFechamentoAgregado.Inserir(fechamentoAgregado);

            PreencherEntidadeCIOT(out mensagemErro, fechamentoAgregado, unitOfWork);

            mensagemErro = null;
            return true;
        }

        private bool PreencherEntidadeCIOT(out string mensagemErro, Dominio.Entidades.Embarcador.Terceiros.FechamentoAgregado fechamentoAgregado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Terceiros.FechamentoAgregadoCIOT repFechamentoAgregadoCIOT = new Repositorio.Embarcador.Terceiros.FechamentoAgregadoCIOT(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);

            List<int> ContratosCIOTSelecionados = Request.GetListParam<int>("ListaCIOT");

            if (ContratosCIOTSelecionados == null || ContratosCIOTSelecionados.Count() == 0)
                throw new ServicoException("CIOT não selecionado.");

            foreach (int codigoCIOT in ContratosCIOTSelecionados)
            {
                Dominio.Entidades.Embarcador.Documentos.CIOT ciot = repCIOT.BuscarPorCodigo(codigoCIOT);

                if (ciot == null)
                    throw new ServicoException($"CIOT código {codigoCIOT} não encontrado.");

                if (repFechamentoAgregadoCIOT.BuscarPorCIOT(codigoCIOT) != null)
                    throw new ServicoException($"Já existe fechamento de agregado para o CIOT {ciot.Numero} selecionado.");

                if (ciot.Operadora != OperadoraCIOT.Pamcard && ciot.Operadora != OperadoraCIOT.RepomFrete)
                    throw new ServicoException($"Processo não disponível para operadora {ciot.Operadora.ObterDescricao()}.");

                Dominio.Entidades.Embarcador.Terceiros.FechamentoAgregadoCIOT fechamentoAgregadoCIOT = new Dominio.Entidades.Embarcador.Terceiros.FechamentoAgregadoCIOT();

                fechamentoAgregadoCIOT.FechamentoAgregado = fechamentoAgregado;
                fechamentoAgregadoCIOT.CIOT = ciot;
                fechamentoAgregadoCIOT.Consolidado = false;

                repFechamentoAgregadoCIOT.Inserir(fechamentoAgregadoCIOT);
            }

            mensagemErro = string.Empty;
            return true;
        }

        /* PropOrdena
         * Recebe o campo ordenado na grid
         * Retorna o elemento especifico da entidade para ordenacao
         */

        private void PropOrdena(ref string propOrdena)
        {
            if (propOrdena == "Codigo")
                propOrdena = "Codigo";
        }

        private Models.Grid.Grid GridPesquisa()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.Prop("Codigo");
            grid.Prop("Numero").Nome("Nro Fechamento Agregado").Tamanho(9).Align(Models.Grid.Align.center);
            grid.Prop("NumeroCIOT").Nome("Nro CIOT").Tamanho(9).Align(Models.Grid.Align.center);
            grid.Prop("DataCriacao").Nome("Data Criação").Tamanho(9).Align(Models.Grid.Align.center);
            grid.Prop("Usuario").Nome("Usuário").Tamanho(10).Align(Models.Grid.Align.right);

            return grid;
        }

        private Models.Grid.Grid GridPesquisaCIOT()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Descricao", false);
            grid.AdicionarCabecalho("Número", "Numero", 12, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Cód. Verificador", "CodigoVerificador", 8, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Protocolo", "ProtocoloAutorizacao", 8, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Cargas", "Cargas", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Operadora", "Operadora", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Data Final da Viagem", "DataFinalViagem", 8, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Transportador", "Transportador", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Situação", "Situacao", 8, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Mensagem", "Mensagem", 20, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Filial", "Filial", 20, Models.Grid.Align.left, false);

            return grid;
        }

        private Models.Grid.Grid GridPesquisaCIOTConsolidacao()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Número", "NumeroCIOT", 12, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Cargas", "Cargas", 10, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Valor a Receber", "ValorAReceber", 15, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Adiantamento", "ValorAdiantamento", 15, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Saldo", "ValorSaldo", 15, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Abastecimento", "ValorAbastecimento", 15, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("IRRF", "ValorIRRF", 15, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("INSS", "ValorINSS", 15, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("SESTSENAT", "ValorSESTSENAT", 15, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Frete Líquido", "ValorFreteLiquido", 15, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Frete Bruto", "ValorFreteBruto", 15, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Acréscimo", "ValorAcrescimo", 15, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Desconto", "ValorDesconto", 15, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Tarifa Saque", "ValorTarifaSaque", 15, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Tarifa Transferencia", "ValorTarifaTransferencia", 15, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Consolidado", false);
            grid.AdicionarCabecalho("CodigoTransportadorContratoFreteOrigem", false);
            grid.AdicionarCabecalho("CodigoCIOT", false);

            return grid;
        }

        private Models.Grid.Grid GridPesquisaDetalhes()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Carga", "Carga", 12, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Contratos Frete", "ContratosFrete", 10, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho("Valor a Receber", "ValorAReceber", 15, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Adiantamento", "ValorAdiantamento", 15, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Saldo", "ValorSaldo", 15, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Abastecimento", "ValorAbastecimento", 15, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("IRRF", "ValorIRRF", 15, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("INSS", "ValorINSS", 15, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("SESTSENAT", "ValorSESTSENAT", 15, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Frete Líquido", "ValorFreteLiquido", 15, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Frete Bruto", "ValorFreteBruto", 15, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Acréscimo", "ValorAcrescimo", 15, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Desconto", "ValorDesconto", 15, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Tarifa Saque", "ValorTarifaSaque", 15, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Tarifa Transferencia", "ValorTarifaTransferencia", 15, Models.Grid.Align.right, true, false);

            return grid;
        }

        private Models.Grid.Grid GridPesquisaAcrescimoDesconto()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.Prop("Codigo");
            grid.Prop("NumeroContrato").Nome("Nº Contrato").Tamanho(30).Align(Models.Grid.Align.left);
            grid.Prop("Justificativa").Nome("Justificativa").Tamanho(30).Align(Models.Grid.Align.left);
            grid.Prop("Valor").Nome("Valor").Tamanho(15).Align(Models.Grid.Align.right);
            grid.Prop("NumeroCarga").Nome("Número Carga").Tamanho(15).Align(Models.Grid.Align.right);
            grid.Prop("NomeSubcontratado").Nome("Subcontratado").Tamanho(15).Align(Models.Grid.Align.right);
            grid.Prop("NomeMotorista").Nome("Motorista").Tamanho(15).Align(Models.Grid.Align.right);
            grid.Prop("Situacao").Nome("Situação").Tamanho(15).Align(Models.Grid.Align.center);

            return grid;
        }

        private Models.Grid.Grid GridPesquisaIntegracaoCIOT()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Situacao", false);
            grid.AdicionarCabecalho("Nro Contrato Frete", "NumeroContrato", 13, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.Integracao, "TipoIntegracao", 13, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Veiculos.Veiculo.Tentativas, "NumeroTentativas", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Veiculos.Veiculo.DataDoEnvio, "DataIntegracao", 12, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "SituacaoIntegracao", 12, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Mensagem, "Retorno", 30, Models.Grid.Align.left, false);

            return grid;
        }

        private void ExecutaPesquisa(ref List<Dominio.Entidades.Embarcador.Terceiros.FechamentoAgregado> lista, ref int totalRegistros, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancias
            Repositorio.Embarcador.Terceiros.FechamentoAgregado repFechamentoAgregado = new Repositorio.Embarcador.Terceiros.FechamentoAgregado(unitOfWork);

            // Converte parametros
            int.TryParse(Request.Params("Usuario"), out int usuario);
            int.TryParse(Request.Params("CIOT"), out int codigoCIOT);

            DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicial);
            DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFinal);

            totalRegistros = repFechamentoAgregado.ContarConsulta(usuario, dataInicial, dataFinal, codigoCIOT);
            if (totalRegistros > 0)
                lista = repFechamentoAgregado.Consultar(usuario, dataInicial, dataFinal, codigoCIOT, propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros);
        }

        private void ExecutaPesquisaCIOT(ref List<Dominio.Entidades.Embarcador.Documentos.CIOT> lista, ref int totalRegistros, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancias
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
            Repositorio.Embarcador.Terceiros.FechamentoAgregado repFechamentoAgregado = new Repositorio.Embarcador.Terceiros.FechamentoAgregado(unitOfWork);
            Repositorio.Embarcador.Terceiros.FechamentoAgregadoCIOT repFechamentoAgregadoCIOT = new Repositorio.Embarcador.Terceiros.FechamentoAgregadoCIOT(unitOfWork);

            // Converte parametros
            int.TryParse(Request.Params("Codigo"), out int codigoFechamentoAgregado);
            double.TryParse(Request.Params("Transportador"), out double transportador);

            Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaCIOT filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaCIOT();

            if (codigoFechamentoAgregado > 0)
            {
                Dominio.Entidades.Embarcador.Terceiros.FechamentoAgregado fechamentoAgregado = repFechamentoAgregado.BuscarPorCodigo(codigoFechamentoAgregado);

                if (fechamentoAgregado == null)
                {
                    filtrosPesquisa.CpfCnpjTransportador = 999999;
                }
                else
                {
                    filtrosPesquisa.CodigosCIOTs = new List<int>();
                    if (fechamentoAgregado.CIOT != null)
                        filtrosPesquisa.CodigosCIOTs.Add(fechamentoAgregado.CIOT.Codigo);
                    else
                        filtrosPesquisa.CodigosCIOTs = repFechamentoAgregadoCIOT.BuscarCodigoCIOTPorCodigoFechamentoAgregado(fechamentoAgregado.Codigo);
                }
            }
            else
            {
                filtrosPesquisa.Situacao = SituacaoCIOT.Aberto;
                filtrosPesquisa.TiposTransportador = new List<TipoProprietarioVeiculo>() { TipoProprietarioVeiculo.TACAgregado };

                if (transportador != 0)
                    filtrosPesquisa.CpfCnpjTransportador = transportador;
                else
                    filtrosPesquisa.CpfCnpjTransportador = 999999;
            }

            totalRegistros = repCIOT.ContarConsulta(filtrosPesquisa);
            if (totalRegistros > 0)
                lista = repCIOT.Consultar(filtrosPesquisa, propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros);
        }

        private void ExecutaPesquisaCIOTConsolidacao(ref List<Dominio.Entidades.Embarcador.Terceiros.FechamentoAgregadoCIOT> lista, ref int totalRegistros, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancias
            Repositorio.Embarcador.Terceiros.FechamentoAgregado repFechamentoAgregado = new Repositorio.Embarcador.Terceiros.FechamentoAgregado(unitOfWork);
            Repositorio.Embarcador.Terceiros.FechamentoAgregadoCIOT repFechamentoAgregadoCIOT = new Repositorio.Embarcador.Terceiros.FechamentoAgregadoCIOT(unitOfWork);

            // Converte parametros
            int.TryParse(Request.Params("Codigo"), out int codigoFechamentoAgregado);

            if (codigoFechamentoAgregado > 0)
            {
                Dominio.Entidades.Embarcador.Terceiros.FechamentoAgregado fechamentoAgregado = repFechamentoAgregado.BuscarPorCodigo(codigoFechamentoAgregado);

                if (fechamentoAgregado.CIOT != null)
                {
                    lista = new List<Dominio.Entidades.Embarcador.Terceiros.FechamentoAgregadoCIOT>();
                    lista.Add(new Dominio.Entidades.Embarcador.Terceiros.FechamentoAgregadoCIOT() { CIOT = fechamentoAgregado.CIOT, Consolidado = fechamentoAgregado.Consolidado, FechamentoAgregado = fechamentoAgregado });
                    totalRegistros = 1;

                    return;
                }
            }

            totalRegistros = repFechamentoAgregadoCIOT.ContarConsulta(codigoFechamentoAgregado);
            if (totalRegistros > 0)
                lista = repFechamentoAgregadoCIOT.Consultar(codigoFechamentoAgregado, 0, propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros);
        }

        private void ExecutaPesquisaDetalhes(ref List<Dominio.Entidades.Embarcador.Cargas.Carga> lista, ref int totalRegistros, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancias
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);

            // Converte parametros
            int.TryParse(Request.Params("Codigo"), out int codigoFechamentoAgregado);
            int.TryParse(Request.Params("CIOT"), out int codigoCIOT);

            totalRegistros = repCargaCIOT.ContarConsultaCargasCIOT(codigoCIOT);
            if (totalRegistros > 0)
                lista = repCargaCIOT.ConsultarCargasCIOT(codigoCIOT, propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros);
        }

        private void ExecutaPesquisaAcrescimoDesconto(ref List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto> lista, TipoJustificativa? tipoJustificativa, ref int totalRegistros, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancias
            Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto repContratoFreteAcrescimoDesconto = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto(unitOfWork);

            // Converte parametros
            int.TryParse(Request.Params("CIOT"), out int codigoCIOT);

            totalRegistros = repContratoFreteAcrescimoDesconto.ContarParaFechamentoAgregado(codigoCIOT, tipoJustificativa);
            if (totalRegistros > 0)
                lista = repContratoFreteAcrescimoDesconto.ConsultarParaFechamentoAgregado(codigoCIOT, tipoJustificativa, propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros);
        }

        private void ExecutaPesquisaIntegracaoCIOT(ref List<Dominio.Entidades.Embarcador.Terceiros.PagamentoContratoIntegracao> lista, ref int totalRegistros, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancias
            Repositorio.Embarcador.Terceiros.PagamentoContratoIntegracao repPagamentoContratoIntegracao = new Repositorio.Embarcador.Terceiros.PagamentoContratoIntegracao(unitOfWork);

            // Converte parametros
            int codigoAutorizacaoPagamento = Request.GetIntParam("Codigo");
            SituacaoIntegracao? situacao = Request.GetNullableEnumParam<SituacaoIntegracao>("Situacao");

            if (codigoAutorizacaoPagamento == 0)
                return;

            totalRegistros = repPagamentoContratoIntegracao.ContarConsultaPorAutorizacaoPagamento(codigoAutorizacaoPagamento, situacao, null);
            if (totalRegistros > 0)
                lista = repPagamentoContratoIntegracao.ConsultarPorAutorizacaoPagamento(codigoAutorizacaoPagamento, situacao, null, propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros);
        }

        private dynamic RetornaDyn(List<Dominio.Entidades.Embarcador.Terceiros.FechamentoAgregado> lista)
        {
            var listaProcessada = from item in lista
                                  select new
                                  {
                                      item.Codigo,
                                      item.Numero,
                                      NumeroCIOT = (item.CIOT != null ? item.CIOT?.Numero : string.Join(", ", item.FechamentoAgregadoCIOTs.Select(o => o.CIOT.Numero))) ?? string.Empty,
                                      DataCriacao = item.DataCriacao.ToString("dd/MM/yyyy"),
                                      Usuario = item.Usuario.Nome
                                  };

            return listaProcessada.ToList();
        }

        private dynamic RetornaDynCIOT(List<Dominio.Entidades.Embarcador.Documentos.CIOT> lista)
        {
            var listaProcessada = from obj in lista
                                  select new
                                  {
                                      obj.Codigo,
                                      Descricao = obj.Numero,
                                      obj.Numero,
                                      Cargas = string.Join(", ", obj.CargaCIOT.Select(o => o.Carga.CodigoCargaEmbarcador)),
                                      obj.CodigoVerificador,
                                      obj.ProtocoloAutorizacao,
                                      DataFinalViagem = obj.DataFinalViagem.ToString("dd/MM/yyyy"),
                                      Transportador = obj.Transportador?.Nome,
                                      Situacao = obj.DescricaoSituacao,
                                      obj.Mensagem,
                                      Operadora = Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOTHelper.ObterDescricao(obj.Operadora),
                                      Filial = string.Join(", ", obj.CargaCIOT.Select(o => o.Carga?.Filial?.Descricao ?? string.Empty))
                                  };

            return listaProcessada.ToList();
        }

        private dynamic RetornaDynCIOTConsolidacao(List<Dominio.Entidades.Embarcador.Terceiros.FechamentoAgregadoCIOT> lista, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto repContratoFreteAcrescimoDesconto = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);

            var listaProcessada = from obj in lista
                                  select new
                                  {
                                      obj.Codigo,
                                      NumeroCIOT = obj.CIOT.Numero,
                                      Cargas = string.Join(", ", obj.CIOT.CargaCIOT.Select(o => o.Carga.CodigoCargaEmbarcador)),
                                      ValorAReceber = repCIOT.BuscarValorTotalReceberPorCIOTeCarga(obj.CIOT.Codigo, 0, "A"),
                                      ValorAdiantamento = (obj.CIOT.CargaCIOT.Sum(o => o.ContratoFrete.ValorAdiantamento)).ToString("n2"),
                                      ValorSaldo = (((obj.CIOT.CargaCIOT.Sum(o => o.ContratoFrete.ValorBruto)) -
                                                     (obj.CIOT.CargaCIOT.Where(o => !o.ContratoFrete.ReterImpostosContratoFrete || o.ContratoFrete.TransportadorTerceiro?.Tipo != "F")
                                                     .Sum(o => o.ContratoFrete.ValorIRRF + o.ContratoFrete.ValorINSS + o.ContratoFrete.ValorSENAT))) // ValorFreteLiquido
                                                     + (repContratoFreteAcrescimoDesconto.SomarValorParaFechamentoAgregado(obj.CIOT.Codigo, TipoJustificativa.Acrescimo)) // ValorAcrescimo
                                                     - (repContratoFreteAcrescimoDesconto.SomarValorParaFechamentoAgregado(obj.CIOT.Codigo, TipoJustificativa.Desconto)) // ValorDesconto
                                                   ).ToString("n2"),
                                      ValorAbastecimento = (obj.CIOT.CargaCIOT.Sum(o => o.ContratoFrete.ValorAbastecimento)).ToString("n2"),
                                      ValorIRRF = (obj.CIOT.CargaCIOT.Where(o => !o.ContratoFrete.ReterImpostosContratoFrete || o.ContratoFrete.TransportadorTerceiro?.Tipo != "F").Sum(o => o.ContratoFrete.ValorIRRF)).ToString("n2"),
                                      ValorINSS = (obj.CIOT.CargaCIOT.Where(o => !o.ContratoFrete.ReterImpostosContratoFrete || o.ContratoFrete.TransportadorTerceiro?.Tipo != "F").Sum(o => o.ContratoFrete.ValorINSS)).ToString("n2"),
                                      ValorSESTSENAT = (obj.CIOT.CargaCIOT.Where(o => !o.ContratoFrete.ReterImpostosContratoFrete || o.ContratoFrete.TransportadorTerceiro?.Tipo != "F").Sum(o => o.ContratoFrete.ValorSEST + o.ContratoFrete.ValorSENAT)).ToString("n2"),
                                      ValorFreteLiquido = ((obj.CIOT.CargaCIOT.Sum(o => o.ContratoFrete.ValorBruto)) - (obj.CIOT.CargaCIOT.Where(o => !o.ContratoFrete.ReterImpostosContratoFrete || o.ContratoFrete.TransportadorTerceiro?.Tipo != "F").Sum(o => o.ContratoFrete.ValorIRRF + o.ContratoFrete.ValorINSS + o.ContratoFrete.ValorSENAT))).ToString("n2"),
                                      ValorFreteBruto = (obj.CIOT.CargaCIOT.Sum(o => o.ContratoFrete.ValorBruto)).ToString("n2"),
                                      ValorAcrescimo = (repContratoFreteAcrescimoDesconto.SomarValorParaFechamentoAgregado(obj.CIOT.Codigo, TipoJustificativa.Acrescimo)).ToString("n2"),
                                      ValorDesconto = (repContratoFreteAcrescimoDesconto.SomarValorParaFechamentoAgregado(obj.CIOT.Codigo, TipoJustificativa.Desconto)).ToString("n2"),
                                      ValorTarifaSaque = (obj.CIOT.CargaCIOT.Sum(o => o.ContratoFrete.TarifaSaque)).ToString("n2"),
                                      ValorTarifaTransferencia = (obj.CIOT.CargaCIOT.Sum(o => o.ContratoFrete.TarifaTransferencia)).ToString("n2"),
                                      obj.Consolidado,
                                      CodigoTransportadorContratoFreteOrigem = obj.CIOT.Transportador.CPF_CNPJ,
                                      CodigoCIOT = obj.CIOT.Codigo,
                                  };

            return listaProcessada.ToList();
        }

        private dynamic RetornaDynDetalhes(List<Dominio.Entidades.Embarcador.Cargas.Carga> lista, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto repContratoFreteAcrescimoDesconto = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            var listaProcessada = from obj in lista
                                  select new
                                  {
                                      obj.Codigo,
                                      Carga = obj.CodigoCargaEmbarcador,
                                      ContratosFrete = string.Join(", ", obj.CargaCIOTs.Select(o => o.ContratoFrete.NumeroContrato)),
                                      ValorAReceber = repCarga.BuscarValorFreteAReceberConhecimentos(obj.Codigo),
                                      ValorAdiantamento = (obj.CargaCIOTs.Sum(o => o.ContratoFrete.ValorAdiantamento)).ToString("n2"),
                                      ValorSaldo = (obj.CargaCIOTs.Sum(o => o.ContratoFrete.SaldoAReceber)).ToString("n2"),
                                      ValorAbastecimento = (obj.CargaCIOTs.Sum(o => o.ContratoFrete.ValorAbastecimento)).ToString("n2"),
                                      ValorIRRF = (obj.CargaCIOTs.Where(o => !o.ContratoFrete.ReterImpostosContratoFrete || o.ContratoFrete.TransportadorTerceiro?.Tipo != "F").Sum(o => o.ContratoFrete.ValorIRRF)).ToString("n2"),
                                      ValorINSS = (obj.CargaCIOTs.Where(o => !o.ContratoFrete.ReterImpostosContratoFrete || o.ContratoFrete.TransportadorTerceiro?.Tipo != "F").Sum(o => o.ContratoFrete.ValorINSS)).ToString("n2"),
                                      ValorSESTSENAT = (obj.CargaCIOTs.Where(o => !o.ContratoFrete.ReterImpostosContratoFrete || o.ContratoFrete.TransportadorTerceiro?.Tipo != "F").Sum(o => o.ContratoFrete.ValorSEST + o.ContratoFrete.ValorSENAT)).ToString("n2"),
                                      ValorFreteLiquido = ((obj.CargaCIOTs.Sum(o => o.ContratoFrete.ValorBruto)) - (obj.CargaCIOTs.Where(o => !o.ContratoFrete.ReterImpostosContratoFrete || o.ContratoFrete.TransportadorTerceiro?.Tipo != "F").Sum(o => o.ContratoFrete.ValorIRRF + o.ContratoFrete.ValorINSS + o.ContratoFrete.ValorSENAT))).ToString("n2"),
                                      ValorFreteBruto = (obj.CargaCIOTs.Sum(o => o.ContratoFrete.ValorBruto)).ToString("n2"),
                                      ValorAcrescimo = (repContratoFreteAcrescimoDesconto.SomarValorParaFechamentoAgregadoPorCarga(obj.Codigo, TipoJustificativa.Acrescimo)).ToString("n2"),
                                      ValorDesconto = (repContratoFreteAcrescimoDesconto.SomarValorParaFechamentoAgregadoPorCarga(obj.Codigo, TipoJustificativa.Desconto)).ToString("n2"),
                                      ValorTarifaSaque = (obj.CargaCIOTs.Sum(o => o.ContratoFrete.TarifaSaque)).ToString("n2"),
                                      ValorTarifaTransferencia = (obj.CargaCIOTs.Sum(o => o.ContratoFrete.TarifaTransferencia)).ToString("n2"),
                                  };

            return listaProcessada.ToList();
        }

        private dynamic RetornaDynAcrescimoDesconto(List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto> lista)
        {
            var listaProcessada = from obj in lista
                                  select new
                                  {
                                      obj.Codigo,
                                      obj?.ContratoFrete?.NumeroContrato,
                                      Justificativa = obj.Justificativa.Descricao,
                                      Valor = obj.Valor.ToString("n2"),
                                      Situacao = obj.Situacao.ObterDescricao(),
                                      NumeroCarga = obj?.ContratoFrete?.Carga?.CodigoCargaEmbarcador,
                                      NomeMotorista = obj?.ContratoFrete?.Carga?.NomeMotoristas,
                                      NomeSubcontratado = obj?.ContratoFrete?.TransportadorTerceiro?.Nome,
                                  };

            return listaProcessada.ToList();
        }

        private dynamic RetornaDynIntegracaoCIOT(List<Dominio.Entidades.Embarcador.Terceiros.PagamentoContratoIntegracao> lista)
        {
            var listaProcessada = (
                from integracao in lista
                select new
                {
                    integracao.Codigo,
                    Situacao = integracao.SituacaoIntegracao,
                    NumeroContrato = integracao.ContratoFrete.NumeroContrato,
                    SituacaoIntegracao = integracao.DescricaoSituacaoIntegracao,
                    TipoIntegracao = integracao.TipoIntegracao.DescricaoTipo,
                    Retorno = integracao.ProblemaIntegracao,
                    integracao.NumeroTentativas,
                    DataIntegracao = integracao.DataIntegracao.ToString("dd/MM/yyyy HH:mm:ss"),
                    DT_RowColor = integracao.SituacaoIntegracao.ObterCorLinha(),
                    DT_FontColor = integracao.SituacaoIntegracao.ObterCorFonte()
                });

            return listaProcessada.ToList();
        }

        private dynamic RetornaDynAdicionar(Dominio.Entidades.Embarcador.Terceiros.FechamentoAgregado fechamentoAgregado, bool consolidado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT situacaoCIOT, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto repContratoFreteAcrescimoDesconto = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto(unitOfWork);
            List<Dominio.Entidades.Embarcador.Terceiros.FechamentoAgregadoCIOT> lista = new List<Dominio.Entidades.Embarcador.Terceiros.FechamentoAgregadoCIOT>();

            // Variavel com o numero total de resultados
            int totalRegistro = 0;

            // Executa metodo de consutla
            ExecutaPesquisaCIOTConsolidacao(ref lista, ref totalRegistro, "Codigo", null, 0, 0, unitOfWork);

            decimal valorAReceber = 0;
            decimal valorAdiantamento = 0;
            decimal valorSaldo = 0;
            decimal valorAbastecimento = 0;
            decimal valorBruto = 0;
            decimal valorIRRF = 0;
            decimal valorINSS = 0;
            decimal valorSESTSENAT = 0;
            decimal valorFreteLiquido = 0;
            decimal valorAcrescimo = 0;
            decimal valorDesconto = 0;

            foreach (var item in lista)
            {
                valorAReceber = repCIOT.BuscarValorTotalReceberPorCIOTeCarga(item.CIOT.Codigo, 0, "A");
                valorAdiantamento += item.CIOT.CargaCIOT.Sum(o => o.ContratoFrete.ValorAdiantamento);
                valorAbastecimento += item.CIOT.CargaCIOT.Sum(o => o.ContratoFrete.ValorAbastecimento);
                valorBruto += item.CIOT.CargaCIOT.Sum(o => o.ContratoFrete.ValorBruto);
                valorIRRF += item.CIOT.CargaCIOT.Where(o => !o.ContratoFrete.ReterImpostosContratoFrete || o.ContratoFrete.TransportadorTerceiro?.Tipo != "F").Sum(o => o.ContratoFrete.ValorIRRF);
                valorINSS += item.CIOT.CargaCIOT.Where(o => !o.ContratoFrete.ReterImpostosContratoFrete || o.ContratoFrete.TransportadorTerceiro?.Tipo != "F").Sum(o => o.ContratoFrete.ValorINSS);
                valorSESTSENAT += item.CIOT.CargaCIOT.Where(o => !o.ContratoFrete.ReterImpostosContratoFrete || o.ContratoFrete.TransportadorTerceiro?.Tipo != "F").Sum(o => o.ContratoFrete.ValorSEST + o.ContratoFrete.ValorSENAT);
                valorFreteLiquido += valorBruto - valorIRRF - valorINSS - valorSESTSENAT;
                valorAcrescimo += repContratoFreteAcrescimoDesconto.SomarValorParaFechamentoAgregado(item.CIOT.Codigo, TipoJustificativa.Acrescimo);
                valorDesconto += repContratoFreteAcrescimoDesconto.SomarValorParaFechamentoAgregado(item.CIOT.Codigo, TipoJustificativa.Desconto);
            }

            var dynDados = new
            {
                Codigo = fechamentoAgregado.Codigo,
                Numero = fechamentoAgregado.Numero,
                Consolidado = consolidado,
                SituacaoCIOT = situacaoCIOT,
                ValorAdiantamento = valorAdiantamento.ToString("n2"),
                ValorSaldo = (valorFreteLiquido + valorAcrescimo - valorDesconto).ToString("n2"),
                ValorAcrescimo = valorAcrescimo.ToString("n2"),
                ValorFreteBruto = valorBruto.ToString("n2"),
                ValorIRRF = valorIRRF.ToString("n2"),
                ValorINSS = valorINSS.ToString("n2"),
                ValorSESTSENAT = valorSESTSENAT.ToString("n2"),
                ValorDesconto = valorDesconto.ToString("n2"),
                ValorFreteLiquido = valorFreteLiquido.ToString("n2"),
                ValorAbastecimento = valorAbastecimento.ToString("n2"),
            };

            return dynDados;
        }

        #endregion Métodos Privados
    }
}
