using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy;
using Dominio.ObjetosDeValor.WebService.Rest.Unilever.Carga;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Terceiros
{
    [CustomAuthorize("Terceiros/AutorizacaoPagamentoContratoFrete")]
    public class AutorizacaoPagamentoContratoFreteController : BaseController
    {
		#region Construtores

		public AutorizacaoPagamentoContratoFreteController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                if (!PreencherEntidade(out Dominio.Entidades.Embarcador.Terceiros.AutorizacaoPagamentoContratoFrete autorizacaoPagamentoContratoFrete, out string mensagemErro, unitOfWork))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, mensagemErro);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(new { Codigo = autorizacaoPagamentoContratoFrete.Codigo });
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

                List<Dominio.Entidades.Embarcador.Terceiros.AutorizacaoPagamentoContratoFrete> lista = new List<Dominio.Entidades.Embarcador.Terceiros.AutorizacaoPagamentoContratoFrete>();

                // Variavel com o numero total de resultados
                int totalRegistro = 0;

                // Executa metodo de consutla
                ExecutaPesquisa(ref lista, ref totalRegistro, propOrdena, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Converte os dados recebidos
                var listaProcessada = RetornaDyn(lista, unitOfWork);

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

        public async Task<IActionResult> PesquisaContratoFrete()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisaContratoFrete();

                // Ordenacao da grid
                string propOrdena = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdena);

                List<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete> lista = new List<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete>();

                // Variavel com o numero total de resultados
                int totalRegistro = 0;

                // Executa metodo de consutla
                ExecutaPesquisaContratoFrete(ref lista, ref totalRegistro, propOrdena, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Converte os dados recebidos
                var listaProcessada = RetornaDynContratoFrete(lista, unitOfWork);

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

        public async Task<IActionResult> PesquisaPagamentoCIOT()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisaPagamentoCIOT();

                // Ordenacao da grid
                string propOrdena = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdena);

                List<Dominio.Entidades.Embarcador.Terceiros.PagamentoCIOTIntegracao> lista = new List<Dominio.Entidades.Embarcador.Terceiros.PagamentoCIOTIntegracao>();

                // Variavel com o numero total de resultados
                int totalRegistro = 0;

                // Executa metodo de consutla
                ExecutaPesquisaPagamentoCIOT(ref lista, ref totalRegistro, propOrdena, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Converte os dados recebidos
                var listaProcessada = RetornaDynPagamentoCIOT(lista);

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

        public async Task<IActionResult> PesquisaPagamentoIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisaPagamentoIntegracao();

                // Ordenacao da grid
                string propOrdena = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdena);

                List<Dominio.Entidades.Embarcador.Terceiros.PagamentoContratoIntegracao> lista = new List<Dominio.Entidades.Embarcador.Terceiros.PagamentoContratoIntegracao>();

                // Variavel com o numero total de resultados
                int totalRegistro = 0;

                // Executa metodo de consulta
                ExecutaPesquisaPagamentoIntegracao(ref lista, ref totalRegistro, propOrdena, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Converte os dados recebidos
                var listaProcessada = RetornaDynPagamentoIntegracao(lista);

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

                List<Dominio.Entidades.Embarcador.Terceiros.AutorizacaoPagamentoContratoFrete> lista = new List<Dominio.Entidades.Embarcador.Terceiros.AutorizacaoPagamentoContratoFrete>();

                // Variavel com o numero total de resultados
                int totalRegistro = 0;

                // Executa metodo de consulta
                ExecutaPesquisa(ref lista, ref totalRegistro, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Converte os dados recebidos
                var listaProcessada = RetornaDyn(lista, unitOfWork);

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
        public async Task<IActionResult> ExportarPesquisaContratoFrete()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisaContratoFrete();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                List<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete> lista = new List<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete>();

                // Variavel com o numero total de resultados
                int totalRegistro = 0;

                // Executa metodo de consutla
                ExecutaPesquisaContratoFrete(ref lista, ref totalRegistro, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Converte os dados recebidos
                var listaProcessada = RetornaDynContratoFrete(lista, unitOfWork);

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
        public async Task<IActionResult> ExportarPesquisaPagamentoCIOT()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisaPagamentoCIOT();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                List<Dominio.Entidades.Embarcador.Terceiros.PagamentoCIOTIntegracao> lista = new List<Dominio.Entidades.Embarcador.Terceiros.PagamentoCIOTIntegracao>();

                // Variavel com o numero total de resultados
                int totalRegistro = 0;

                // Executa metodo de consulta
                ExecutaPesquisaPagamentoCIOT(ref lista, ref totalRegistro, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Converte os dados recebidos
                var listaProcessada = RetornaDynPagamentoCIOT(lista);

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
        public async Task<IActionResult> ExportarPesquisaPagamentoIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisaPagamentoIntegracao();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                List<Dominio.Entidades.Embarcador.Terceiros.PagamentoContratoIntegracao> lista = new List<Dominio.Entidades.Embarcador.Terceiros.PagamentoContratoIntegracao>();

                // Variavel com o numero total de resultados
                int totalRegistro = 0;

                // Executa metodo de consutla
                ExecutaPesquisaPagamentoIntegracao(ref lista, ref totalRegistro, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Converte os dados recebidos
                var listaProcessada = RetornaDynPagamentoIntegracao(lista);

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
                Repositorio.Embarcador.Terceiros.AutorizacaoPagamentoContratoFrete repAutorizacaoPagamentoContratoFrete = new Repositorio.Embarcador.Terceiros.AutorizacaoPagamentoContratoFrete(unitOfWork);
                Repositorio.Embarcador.Terceiros.PagamentoCIOTIntegracao repPagamentoCIOTIntegracao = new Repositorio.Embarcador.Terceiros.PagamentoCIOTIntegracao(unitOfWork);
                Repositorio.Embarcador.Terceiros.PagamentoContratoIntegracao repPagamentoContratoIntegracao = new Repositorio.Embarcador.Terceiros.PagamentoContratoIntegracao(unitOfWork);

                // Codigo requisicao
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Entidades
                Dominio.Entidades.Embarcador.Terceiros.AutorizacaoPagamentoContratoFrete autorizacaoPagamentoContratoFrete = repAutorizacaoPagamentoContratoFrete.BuscarPorCodigo(codigo);

                if (autorizacaoPagamentoContratoFrete == null)
                    return new JsonpResult(false, "Autorização de pagamento não encontrada.");

                int qtdPagamentoCIOTIntegracaoPendente = 0;
                int qtdPagamentoContratoIntegracaoPendente = 0;

                var listaPagamentoCIOTIntegracao = repPagamentoCIOTIntegracao.BuscarPorAutorizacaoPagamento(autorizacaoPagamentoContratoFrete.Codigo);
                qtdPagamentoCIOTIntegracaoPendente = listaPagamentoCIOTIntegracao.Where(o => o.SituacaoIntegracao != SituacaoIntegracao.Integrado).Count();

                var listaPagamentoContratoIntegracao = repPagamentoContratoIntegracao.BuscarPorAutorizacaoPagamento(autorizacaoPagamentoContratoFrete.Codigo);
                qtdPagamentoContratoIntegracaoPendente = listaPagamentoContratoIntegracao.Where(o => o.SituacaoIntegracao != SituacaoIntegracao.Integrado).Count();

                var dynDados = new
                {
                    Codigo = autorizacaoPagamentoContratoFrete.Codigo,
                    Numero = autorizacaoPagamentoContratoFrete.Numero,
                    ContratoFretePagamentoCIOTIntegracaoPendente = qtdPagamentoCIOTIntegracaoPendente > 0,
                    ContratoFretePagamentoCONTRATOIntegracaoPendente = qtdPagamentoContratoIntegracaoPendente > 0
                };

                return new JsonpResult(dynDados);
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

        public async Task<IActionResult> IntegrarPagamentoCIOT()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Terceiros.PagamentoCIOTIntegracao repPagamentoCIOTIntegracao = new Repositorio.Embarcador.Terceiros.PagamentoCIOTIntegracao(unitOfWork);
                Servicos.Embarcador.Terceiros.ContratoFrete contratoFrete = new Servicos.Embarcador.Terceiros.ContratoFrete(unitOfWork);

                // Codigo requisicao
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Entidades
                Dominio.Entidades.Embarcador.Terceiros.PagamentoCIOTIntegracao pagamentoCIOTIntegracao = repPagamentoCIOTIntegracao.BuscarPorCodigo(codigo);

                if (pagamentoCIOTIntegracao == null)
                    return new JsonpResult(false, "Pagamento CIOT não encontrada.");

                var situacaoPendente = new List<SituacaoIntegracao>() { SituacaoIntegracao.AgIntegracao, SituacaoIntegracao.ProblemaIntegracao };
                if (!situacaoPendente.Contains(pagamentoCIOTIntegracao.SituacaoIntegracao))
                    return new JsonpResult(false, "Pagamento CIOT encontra em situação onde não é possível processar.");

                var TipoPagamentoAcertoDespesa = pagamentoCIOTIntegracao.AutorizacaoPagamentoContratoFrete.TipoPagamento == EnumTipoPagamentoAutorizacaoPagamento.PagamentoAdiantamento ? TipoAutorizacaoPagamentoCIOTParcela.Adiantamento : TipoAutorizacaoPagamentoCIOTParcela.Saldo;
                contratoFrete.ProcessarPagamentoCIOT(TipoPagamentoAcertoDespesa, pagamentoCIOTIntegracao, TipoServicoMultisoftware, unitOfWork);
                contratoFrete.GerarPagamentoContratoFreteIntegracao(pagamentoCIOTIntegracao.AutorizacaoPagamentoContratoFrete, unitOfWork);

                var dynDados = new
                {
                    Codigo = codigo,
                };

                return new JsonpResult(dynDados);
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

        public async Task<IActionResult> IntegrarPagamentoContrato()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Terceiros.PagamentoContratoIntegracao repPagamentoContratoIntegracao = new Repositorio.Embarcador.Terceiros.PagamentoContratoIntegracao(unitOfWork);
                Servicos.Embarcador.Terceiros.ContratoFrete contratoFrete = new Servicos.Embarcador.Terceiros.ContratoFrete(unitOfWork);

                // Codigo requisicao
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Entidades
                Dominio.Entidades.Embarcador.Terceiros.PagamentoContratoIntegracao pagamentoContratoIntegracao = repPagamentoContratoIntegracao.BuscarPorCodigo(codigo);

                if (pagamentoContratoIntegracao == null)
                    return new JsonpResult(false, "Pagamento contrato não encontrada.");

                var situacaoPendente = new List<SituacaoIntegracao>() { SituacaoIntegracao.AgIntegracao, SituacaoIntegracao.ProblemaIntegracao };
                if (!situacaoPendente.Contains(pagamentoContratoIntegracao.SituacaoIntegracao))
                    return new JsonpResult(false, "Pagamento CIOT encontra em situação onde não é possível processar.");

                contratoFrete.ProcessarPagamentoContratoFreteIntegracao(pagamentoContratoIntegracao.AutorizacaoPagamentoContratoFrete.TipoPagamento, pagamentoContratoIntegracao, TipoServicoMultisoftware, unitOfWork);

                var dynDados = new
                {
                    Codigo = codigo,
                };

                return new JsonpResult(dynDados);
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

        #endregion Métodos Globais

        #region Métodos Privados

        private bool PreencherEntidade(out Dominio.Entidades.Embarcador.Terceiros.AutorizacaoPagamentoContratoFrete autorizacaoPagamentoContratoFrete, out string mensagemErro, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Terceiros.AutorizacaoPagamentoContratoFrete repAutorizacaoPagamentoContratoFrete = new Repositorio.Embarcador.Terceiros.AutorizacaoPagamentoContratoFrete(unitOfWork);

            autorizacaoPagamentoContratoFrete = new Dominio.Entidades.Embarcador.Terceiros.AutorizacaoPagamentoContratoFrete();

            autorizacaoPagamentoContratoFrete.Numero = repAutorizacaoPagamentoContratoFrete.BuscarProximoNumero();
            autorizacaoPagamentoContratoFrete.TipoPagamento = (EnumTipoPagamentoAutorizacaoPagamento)Request.GetIntParam("TipoPagamento");
            autorizacaoPagamentoContratoFrete.DataCriacao = DateTime.Now;
            autorizacaoPagamentoContratoFrete.Usuario = this.Usuario;

            repAutorizacaoPagamentoContratoFrete.Inserir(autorizacaoPagamentoContratoFrete);

            if (!PreencherContratosFrete(out mensagemErro, ref autorizacaoPagamentoContratoFrete, unitOfWork))
                return false;

            repAutorizacaoPagamentoContratoFrete.Atualizar(autorizacaoPagamentoContratoFrete);

            if (!SalvarPagamentoCIOTIntegracao(out mensagemErro, autorizacaoPagamentoContratoFrete, unitOfWork))
                return false;

            if (ExigirComprovante(autorizacaoPagamentoContratoFrete))
                if (PossuiComprovantesPendentes(out mensagemErro, autorizacaoPagamentoContratoFrete, unitOfWork))
                    return false;

            return true;
        }

        private bool PossuiComprovantesPendentes(out string mensagemErro, Dominio.Entidades.Embarcador.Terceiros.AutorizacaoPagamentoContratoFrete autorizacaoPagamentoContratoFrete, Repositorio.UnitOfWork unitOfWork)
        {
            mensagemErro = "";
            List<int> idCargas = autorizacaoPagamentoContratoFrete.ContratoFrete.Select(x => x.Carga.Codigo).ToList();
            Repositorio.Embarcador.Cargas.ComprovanteCarga.ComprovanteCarga repComprovanteCarga = new Repositorio.Embarcador.Cargas.ComprovanteCarga.ComprovanteCarga(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.ComprovanteCarga comprovanteCarga = repComprovanteCarga.ObterComprovantesPendentesPorCarga(idCargas).FirstOrDefault();
            if (comprovanteCarga == null)
                return false;
            else
            {
                mensagemErro = $"Existem comprovantes pendentes.";
                return true;
            }
        }

        private bool ExigirComprovante(Dominio.Entidades.Embarcador.Terceiros.AutorizacaoPagamentoContratoFrete autorizacaoPagamentoContratoFrete)
        {
            foreach (var contratoFrete in autorizacaoPagamentoContratoFrete?.ContratoFrete)
            {
                if (contratoFrete?.Carga?.TipoOperacao?.ConfiguracaoCalculoFrete?.ExigirComprovantesLiberacaoPagamentoContratoFrete ?? false)
                    return true;
                if (contratoFrete?.Carga?.Pedidos?.ElementAt(0)?.Tomador?.ExigirComprovantesLiberacaoPagamentoContratoFrete ?? false)
                    return true;
                if (contratoFrete?.Carga?.GrupoPessoaPrincipal?.ExigirComprovantesLiberacaoPagamentoContratoFrete ?? false)
                    return true;
            }
            return false;
        }

        private bool PreencherContratosFrete(out string mensagemErro, ref Dominio.Entidades.Embarcador.Terceiros.AutorizacaoPagamentoContratoFrete autorizacaoPagamentoContratoFrete, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);

            List<int> ContratosFreteSelecionados = Request.GetListParam<int>("ListaContratosFrete");

            foreach (var codigoContratoFrete in ContratosFreteSelecionados)
            {
                if (autorizacaoPagamentoContratoFrete.ContratoFrete == null)
                    autorizacaoPagamentoContratoFrete.ContratoFrete = new List<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete>();

                autorizacaoPagamentoContratoFrete.ContratoFrete.Add(repContratoFrete.BuscarPorCodigo(codigoContratoFrete));
            }

            mensagemErro = string.Empty;
            return true;
        }

        private bool SalvarPagamentoCIOTIntegracao(out string mensagemErro, Dominio.Entidades.Embarcador.Terceiros.AutorizacaoPagamentoContratoFrete autorizacaoPagamentoContratoFrete, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Terceiros.PagamentoCIOTIntegracao repPagamentoCIOTIntegracao = new Repositorio.Embarcador.Terceiros.PagamentoCIOTIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            var tipoIntegracao = repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada);

            foreach (var contratoFrete in autorizacaoPagamentoContratoFrete.ContratoFrete)
            {
                var pagamentoCIOTIntegracao = new Dominio.Entidades.Embarcador.Terceiros.PagamentoCIOTIntegracao();
                pagamentoCIOTIntegracao.TipoIntegracao = tipoIntegracao;
                pagamentoCIOTIntegracao.ProblemaIntegracao = "";
                pagamentoCIOTIntegracao.NumeroTentativas = 0;
                pagamentoCIOTIntegracao.AutorizacaoPagamentoContratoFrete = autorizacaoPagamentoContratoFrete;
                pagamentoCIOTIntegracao.ContratoFrete = contratoFrete;

                if (contratoFrete.ConfiguracaoCIOT != null)
                {
                    var cargaCIOT = repCargaCIOT.BuscarPorContrato(contratoFrete.Codigo);
                    if (cargaCIOT != null)
                        pagamentoCIOTIntegracao.CIOT = cargaCIOT.CIOT;
                }

                pagamentoCIOTIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                pagamentoCIOTIntegracao.DataIntegracao = DateTime.Now;

                repPagamentoCIOTIntegracao.Inserir(pagamentoCIOTIntegracao);
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
            grid.Prop("Numero").Nome("Nro Autorização Pagamento").Tamanho(9).Align(Models.Grid.Align.center);
            grid.Prop("TipoPagamento").Nome("Tipo Pagamento").Tamanho(9).Align(Models.Grid.Align.center);
            grid.Prop("DataCriacao").Nome("Data Criação").Tamanho(9).Align(Models.Grid.Align.center);
            grid.Prop("Usuario").Nome("Usuário").Tamanho(10).Align(Models.Grid.Align.right);

     
            grid.Prop("DataAberturaCIOT").Nome("Data de abertura CIOT").Tamanho(9).Align(Models.Grid.Align.left);
            grid.Prop("TransportadorTerceiro").Nome("Terceiro").Tamanho(25);
            grid.Prop("Operadora").Nome("Operadora").Tamanho(9).Align(Models.Grid.Align.left);

            return grid;
        }

        private Models.Grid.Grid GridPesquisaContratoFrete()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.Prop("Codigo");
            grid.Prop("NumeroCIOT").Nome("CIOT").Tamanho(9).Align(Models.Grid.Align.center);
            grid.Prop("DataAberturaCIOT").Nome("Data de abertura CIOT").Tamanho(9).Align(Models.Grid.Align.center);
            grid.Prop("Operadora").Nome("Operadora").Tamanho(9).Align(Models.Grid.Align.center);
            grid.Prop("NumeroContrato").Nome("Nº Contrato").Tamanho(9).Align(Models.Grid.Align.center);
            grid.Prop("Carga").Nome("Nº Carga").Tamanho(10).Align(Models.Grid.Align.center);
            grid.Prop("TransportadorTerceiro").Nome("Terceiro").Tamanho(25);
            grid.Prop("ValorFreteSubcontratacao").Nome("Val. Contrato").Tamanho(10).Align(Models.Grid.Align.right);
            grid.Prop("ValorOutrosAdiantamento").Nome("Val. Outros Ad.").Tamanho(10).Align(Models.Grid.Align.right);
            grid.Prop("ValorAdiantamento").Nome("Val. Adiantamento").Tamanho(10).Align(Models.Grid.Align.right);
            grid.Prop("ValorSaldo").Nome("Val. Saldo").Tamanho(10).Align(Models.Grid.Align.right);
            grid.Prop("DescricaoSituacaoContratoFrete").Nome("Situação").Tamanho(11).Align(Models.Grid.Align.center);

            return grid;
        }

        private Models.Grid.Grid GridPesquisaPagamentoCIOT()
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
            grid.AdicionarCabecalho("Operadora", "Operadora", 13, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Veiculos.Veiculo.Tentativas, "NumeroTentativas", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Veiculos.Veiculo.DataDoEnvio, "DataIntegracao", 12, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "SituacaoIntegracao", 12, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Mensagem, "Retorno", 30, Models.Grid.Align.left, false);

            return grid;
        }

        private Models.Grid.Grid GridPesquisaPagamentoIntegracao()
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

        private void ExecutaPesquisa(ref List<Dominio.Entidades.Embarcador.Terceiros.AutorizacaoPagamentoContratoFrete> lista, ref int totalRegistros, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancias
            Repositorio.Embarcador.Terceiros.AutorizacaoPagamentoContratoFrete repAutorizacaoPagamentoContratoFrete = new Repositorio.Embarcador.Terceiros.AutorizacaoPagamentoContratoFrete(unitOfWork);

            // Converte parametros
            int.TryParse(Request.Params("Carga"), out int carga);
            int.TryParse(Request.Params("Usuario"), out int usuario);
            int.TryParse(Request.Params("Numero"), out int numero);
            int.TryParse(Request.Params("Empresa"), out int empresa);

            double.TryParse(Request.Params("TransportadorTerceiro"), out double transportadorTerceiro);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT? operadoraCIOT = null;
            if (Enum.TryParse(Request.Params("Operadora"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT operadoraCIOTAux))
                operadoraCIOT = operadoraCIOTAux;


            DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicial);
            DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFinal);

            DateTime.TryParseExact(Request.Params("DataCiotInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataCiotInicial);
            DateTime.TryParseExact(Request.Params("DataCiotFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataCiotFinal);

            totalRegistros = repAutorizacaoPagamentoContratoFrete.ContarConsulta(usuario, dataInicial, dataFinal, numero, carga, empresa, dataCiotInicial, dataCiotFinal, transportadorTerceiro, operadoraCIOT);
            if (totalRegistros > 0)
                lista = repAutorizacaoPagamentoContratoFrete.Consultar(usuario, dataInicial, dataFinal, numero, carga, empresa, dataCiotInicial, dataCiotFinal, transportadorTerceiro, operadoraCIOT, propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros);
        }

        private void ExecutaPesquisaContratoFrete(ref List<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete> lista, ref int totalRegistros, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancias
            Repositorio.Embarcador.Terceiros.AutorizacaoPagamentoContratoFrete repAutorizacaoPagamentoContratoFrete = new Repositorio.Embarcador.Terceiros.AutorizacaoPagamentoContratoFrete(unitOfWork);

            // Converte parametros
            int.TryParse(Request.Params("TipoOperacao"), out int tipoOperacao);
            int.TryParse(Request.Params("Carga"), out int carga);
            int.TryParse(Request.Params("Usuario"), out int usuario);
            int.TryParse(Request.Params("Numero"), out int numero);
            int.TryParse(Request.Params("Empresa"), out int empresa);
            int.TryParse(Request.Params("Codigo"), out int codigoAutorizacaoPagamento);
            double.TryParse(Request.Params("TransportadorTerceiro"), out double transportadorTerceiro);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete? situacao = null;
            if (Enum.TryParse(Request.Params("Situacao"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete situacaoAux))
                situacao = situacaoAux;

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTipoPagamentoAutorizacaoPagamento? tipoPagamento = null;
            if (Enum.TryParse(Request.Params("TipoPagamento"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTipoPagamentoAutorizacaoPagamento tipoPagamentoAux))
                tipoPagamento = tipoPagamentoAux;
            
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT? operadoraCIOT = null;
            if (Enum.TryParse(Request.Params("Operadora"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT operadoraCIOTAux))
                operadoraCIOT = operadoraCIOTAux;

            DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicial);
            DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFinal);

            totalRegistros = repAutorizacaoPagamentoContratoFrete.ContarConsultaPorContratoFrete(usuario, dataInicial, dataFinal, situacao, numero, carga, empresa, codigoAutorizacaoPagamento, tipoPagamento, operadoraCIOT, transportadorTerceiro);
            if (totalRegistros > 0)
                lista = repAutorizacaoPagamentoContratoFrete.ConsultarPorContratoFrete(usuario, dataInicial, dataFinal, situacao, numero, carga, empresa, codigoAutorizacaoPagamento, tipoPagamento, propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros, operadoraCIOT, transportadorTerceiro);
        }

        private void ExecutaPesquisaPagamentoCIOT(ref List<Dominio.Entidades.Embarcador.Terceiros.PagamentoCIOTIntegracao> lista, ref int totalRegistros, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancias
            Repositorio.Embarcador.Terceiros.PagamentoCIOTIntegracao repPagamentoCIOTIntegracao = new Repositorio.Embarcador.Terceiros.PagamentoCIOTIntegracao(unitOfWork);

            // Converte parametros
            int codigoAutorizacaoPagamento = Request.GetIntParam("Codigo");
            SituacaoIntegracao? situacao = Request.GetNullableEnumParam<SituacaoIntegracao>("Situacao");

            if (codigoAutorizacaoPagamento == 0)
                return;

            totalRegistros = repPagamentoCIOTIntegracao.ContarConsultaPorAutorizacaoPagamento(codigoAutorizacaoPagamento, situacao, null);
            if (totalRegistros > 0)
                lista = repPagamentoCIOTIntegracao.ConsultarPorAutorizacaoPagamento(codigoAutorizacaoPagamento, situacao, null, propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros);
        }

        private void ExecutaPesquisaPagamentoIntegracao(ref List<Dominio.Entidades.Embarcador.Terceiros.PagamentoContratoIntegracao> lista, ref int totalRegistros, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, Repositorio.UnitOfWork unitOfWork)
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

        private dynamic RetornaDyn(List<Dominio.Entidades.Embarcador.Terceiros.AutorizacaoPagamentoContratoFrete> lista, Repositorio.UnitOfWork unitOfWork)
        {

            var listaProcessada = from item in lista
                                  select new
                                  {
                                      item.Codigo,
                                      item.Numero,
                                      TipoPagamento = item.TipoPagamento.ObterDescricao(),
                                      DataCriacao = item.DataCriacao.ToString("dd/MM/yyyy"),
                                      Usuario = item.Usuario.Nome,
                                      TransportadorTerceiro = String.Join(", ", item.ContratoFrete.Where(q => q.TransportadorTerceiro != null).Select(el => el.TransportadorTerceiro.Nome)),
                                      Operadora = String.Join(", ", item.ContratoFrete.Select(el=> ObterOperadoraCIOTContratoFrete(el, unitOfWork)).Where<String>(n=> !string.IsNullOrWhiteSpace(n))),
                                      DataAberturaCIOT = String.Join(", ", item.ContratoFrete.Select(el => ObterDataAberturaCIOTContratoFrete(el, unitOfWork)).Where<String>(n => !string.IsNullOrWhiteSpace(n))),
                                  };

            return listaProcessada.ToList();
        }

        private dynamic RetornaDynContratoFrete(List<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete> lista, Repositorio.UnitOfWork unitOfWork)
        {
            var listaProcessada = from item in lista
                                  select new
                                  {
                                      item.Codigo,
                                      item.NumeroContrato,
                                      Carga = item.Carga.CodigoCargaEmbarcador,
                                      TransportadorTerceiro = item.TransportadorTerceiro.Nome,
                                      ValorFreteSubcontratacao = item.ValorFreteSubcontratacao.ToString("n2"),
                                      ValorOutrosAdiantamento = item.ValorOutrosAdiantamento.ToString("n2"),
                                      ValorAdiantamento = item.ValorAdiantamento.ToString("n2"),
                                      ValorSaldo = item.ValorSaldo.ToString("n2"),
                                      item.DescricaoSituacaoContratoFrete,
                                      NumeroCIOT = ObterCIOTContratoFrete(item, unitOfWork),
                                      Operadora = ObterOperadoraCIOTContratoFrete(item, unitOfWork),
                                      DataAberturaCIOT = ObterDataAberturaCIOTContratoFrete(item, unitOfWork)
                                  };

            return listaProcessada.ToList();
        }

        private dynamic RetornaDynPagamentoCIOT(List<Dominio.Entidades.Embarcador.Terceiros.PagamentoCIOTIntegracao> lista)
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
                    Operadora = integracao?.CIOT?.Operadora.ObterDescricao(),
                    Retorno = integracao.ProblemaIntegracao,
                    integracao.NumeroTentativas,
                    DataIntegracao = integracao.DataIntegracao.ToString("dd/MM/yyyy HH:mm:ss"),
                    DT_RowColor = integracao.SituacaoIntegracao.ObterCorLinha(),
                    DT_FontColor = integracao.SituacaoIntegracao.ObterCorFonte()
                });

            return listaProcessada.ToList();
        }

        private dynamic RetornaDynPagamentoIntegracao(List<Dominio.Entidades.Embarcador.Terceiros.PagamentoContratoIntegracao> lista)
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

        private string ObterCIOTContratoFrete(Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPorContrato(contratoFrete.Codigo);

            return cargaCIOT?.CIOT?.Numero ?? "";
        }

        private string ObterOperadoraCIOTContratoFrete(Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPorContrato(contratoFrete.Codigo);

            return cargaCIOT?.CIOT.Operadora.ToString() ?? "";
        }

        private string ObterDataAberturaCIOTContratoFrete(Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPorContrato(contratoFrete.Codigo);

            return cargaCIOT?.CIOT.DataAbertura.ToString() ?? "";
        }


        #endregion Métodos Privados
    }
}
