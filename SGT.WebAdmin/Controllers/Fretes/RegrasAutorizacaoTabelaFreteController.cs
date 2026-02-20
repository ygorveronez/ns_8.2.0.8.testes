using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.Fretes
{
    [CustomAuthorize("Fretes/RegrasAutorizacaoValorFrete")]
    public class RegrasAutorizacaoTabelaFreteController : BaseController
    {
		#region Construtores

		public RegrasAutorizacaoTabelaFreteController(Conexao conexao) : base(conexao) { }

		#endregion

        #region ObjetosJson

        private class ObjetoEntidade
        {
            public dynamic Codigo { get; set; } // dynamic pois o codigo pode ser também um cpf/cnpj
            public string Descricao { get; set; }
        }

        private class ObjetoAprovadores
        {
            public int Codigo { get; set; }
            public string Nome { get; set; }
        }

        private class RegrasPorTipo
        {
            public dynamic Codigo { get; set; }
            public int Ordem { get; set; }
            public CondicaoAutorizaoValorFrete Condicao { get; set; }
            public JuncaoAutorizaoValorFrete Juncao { get; set; }
            public TipoValorAutorizaoValorFrete TipoRegra { get; set; }
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
                Models.Grid.Grid grid = GridPesquisa();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "DescricaoEtapaAutorizacaoTabelaFrete") propOrdenar = "Etapa";

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
                if (propOrdenar == "DescricaoEtapaAutorizacaoTabelaFrete") propOrdenar = "Etapa";

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

                // Instancia Repositorios/Entidade
                Repositorio.Embarcador.Frete.RegrasAutorizacaoTabelaFrete repRegrasAutorizacaoValorFrete = new Repositorio.Embarcador.Frete.RegrasAutorizacaoTabelaFrete(unitOfWork);
                Repositorio.Embarcador.Frete.RegrasMotivoReajuste repRegrasMotivoReajuste = new Repositorio.Embarcador.Frete.RegrasMotivoReajuste(unitOfWork);
                Repositorio.Embarcador.Frete.RegrasOrigemFrete repRegrasOrigemFrete = new Repositorio.Embarcador.Frete.RegrasOrigemFrete(unitOfWork);
                Repositorio.Embarcador.Frete.RegrasDestinoFrete repRegrasDestinoFrete = new Repositorio.Embarcador.Frete.RegrasDestinoFrete(unitOfWork);
                Repositorio.Embarcador.Frete.RegrasTransportador repRegrasTransportador = new Repositorio.Embarcador.Frete.RegrasTransportador(unitOfWork);
                Repositorio.Embarcador.Frete.RegrasTipoOperacao repRegrasTipoOperacao = new Repositorio.Embarcador.Frete.RegrasTipoOperacao(unitOfWork);
                Repositorio.Embarcador.Frete.RegrasValorFrete repRegrasValorFrete = new Repositorio.Embarcador.Frete.RegrasValorFrete(unitOfWork);
                Repositorio.Embarcador.Frete.RegrasValorPedagio repRegrasValorPedagio = new Repositorio.Embarcador.Frete.RegrasValorPedagio(unitOfWork);
                Repositorio.Embarcador.Frete.RegrasAdValorem repRegrasAdValorem = new Repositorio.Embarcador.Frete.RegrasAdValorem(unitOfWork);
                Repositorio.Embarcador.Frete.RegrasFilial repRegrasFilial = new Repositorio.Embarcador.Frete.RegrasFilial(unitOfWork);

                // Nova entidade
                Dominio.Entidades.Embarcador.Frete.RegrasAutorizacaoTabelaFrete regrasTabelaFrete = new Dominio.Entidades.Embarcador.Frete.RegrasAutorizacaoTabelaFrete();
                List<Dominio.Entidades.Embarcador.Frete.RegrasMotivoReajuste> regrasMotivoReajuste = new List<Dominio.Entidades.Embarcador.Frete.RegrasMotivoReajuste>();
                List<Dominio.Entidades.Embarcador.Frete.RegrasOrigemFrete> regrasOrigemFrete = new List<Dominio.Entidades.Embarcador.Frete.RegrasOrigemFrete>();
                List<Dominio.Entidades.Embarcador.Frete.RegrasDestinoFrete> regrasDestinoFrete = new List<Dominio.Entidades.Embarcador.Frete.RegrasDestinoFrete>();
                List<Dominio.Entidades.Embarcador.Frete.RegrasTransportador> regrasTransportador = new List<Dominio.Entidades.Embarcador.Frete.RegrasTransportador>();
                List<Dominio.Entidades.Embarcador.Frete.RegrasTipoOperacao> regrasTipoOperacao = new List<Dominio.Entidades.Embarcador.Frete.RegrasTipoOperacao>();
                List<Dominio.Entidades.Embarcador.Frete.RegrasValorFrete> regrasValorFrete = new List<Dominio.Entidades.Embarcador.Frete.RegrasValorFrete>();
                List<Dominio.Entidades.Embarcador.Frete.RegrasValorPedagio> regrasValorPedagio = new List<Dominio.Entidades.Embarcador.Frete.RegrasValorPedagio>();
                List<Dominio.Entidades.Embarcador.Frete.RegrasAdValorem> regrasAdValorem = new List<Dominio.Entidades.Embarcador.Frete.RegrasAdValorem>();
                List<Dominio.Entidades.Embarcador.Frete.RegrasFilial> regrasFilial = new List<Dominio.Entidades.Embarcador.Frete.RegrasFilial>();

                // Preenche a entidade
                PreencherEntidade(ref regrasTabelaFrete, unitOfWork);

                string erro = string.Empty;
                // Validar entidade
                if (!ValidarEntidade(regrasTabelaFrete, out erro))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, erro);
                }

                try
                {
                    List<string> erros = new List<string>();
                    PreencherTodasRegras(ref regrasTabelaFrete, ref regrasMotivoReajuste, ref regrasOrigemFrete, ref regrasDestinoFrete, ref regrasTransportador, ref regrasTipoOperacao, ref regrasValorFrete, ref regrasValorPedagio, ref regrasAdValorem, ref regrasFilial, ref erros, unitOfWork);
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, ex.Message);
                }

                // Insere Entidade
                repRegrasAutorizacaoValorFrete.Inserir(regrasTabelaFrete);

                // Insere regras
                for (var i = 0; i < regrasMotivoReajuste.Count; i++) repRegrasMotivoReajuste.Inserir(regrasMotivoReajuste[i]);
                for (var i = 0; i < regrasOrigemFrete.Count; i++) repRegrasOrigemFrete.Inserir(regrasOrigemFrete[i]);
                for (var i = 0; i < regrasDestinoFrete.Count; i++) repRegrasDestinoFrete.Inserir(regrasDestinoFrete[i]);
                for (var i = 0; i < regrasTransportador.Count; i++) repRegrasTransportador.Inserir(regrasTransportador[i]);
                for (var i = 0; i < regrasTipoOperacao.Count; i++) repRegrasTipoOperacao.Inserir(regrasTipoOperacao[i]);
                for (var i = 0; i < regrasValorFrete.Count; i++) repRegrasValorFrete.Inserir(regrasValorFrete[i]);
                for (var i = 0; i < regrasValorPedagio.Count; i++) repRegrasValorPedagio.Inserir(regrasValorPedagio[i]);
                for (var i = 0; i < regrasAdValorem.Count; i++) repRegrasAdValorem.Inserir(regrasAdValorem[i]);
                for (var i = 0; i < regrasFilial.Count; i++) repRegrasFilial.Inserir(regrasFilial[i]);

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
                Repositorio.Embarcador.Frete.RegrasAutorizacaoTabelaFrete repRegrasAutorizacaoTabelaFrete = new Repositorio.Embarcador.Frete.RegrasAutorizacaoTabelaFrete(unitOfWork);
                Repositorio.Embarcador.Frete.RegrasMotivoReajuste repRegrasMotivoReajuste = new Repositorio.Embarcador.Frete.RegrasMotivoReajuste(unitOfWork);
                Repositorio.Embarcador.Frete.RegrasOrigemFrete repRegrasOrigemFrete = new Repositorio.Embarcador.Frete.RegrasOrigemFrete(unitOfWork);
                Repositorio.Embarcador.Frete.RegrasDestinoFrete repRegrasDestinoFrete = new Repositorio.Embarcador.Frete.RegrasDestinoFrete(unitOfWork);
                Repositorio.Embarcador.Frete.RegrasTransportador repRegrasTransportador = new Repositorio.Embarcador.Frete.RegrasTransportador(unitOfWork);
                Repositorio.Embarcador.Frete.RegrasTipoOperacao repRegrasTipoOperacao = new Repositorio.Embarcador.Frete.RegrasTipoOperacao(unitOfWork);
                Repositorio.Embarcador.Frete.RegrasValorFrete repRegrasValorFrete = new Repositorio.Embarcador.Frete.RegrasValorFrete(unitOfWork);
                Repositorio.Embarcador.Frete.RegrasValorPedagio repRegrasValorPedagio = new Repositorio.Embarcador.Frete.RegrasValorPedagio(unitOfWork);
                Repositorio.Embarcador.Frete.RegrasAdValorem repRegrasAdValorem = new Repositorio.Embarcador.Frete.RegrasAdValorem(unitOfWork);
                Repositorio.Embarcador.Frete.RegrasFilial repRegrasFilial = new Repositorio.Embarcador.Frete.RegrasFilial(unitOfWork);

                // Nova entidade
                // Codigo da busca 
                int codigoRegra = 0;
                int.TryParse(Request.Params("Codigo"), out codigoRegra);

                // Busca entidade
                Dominio.Entidades.Embarcador.Frete.RegrasAutorizacaoTabelaFrete regrasTabelaFrete = repRegrasAutorizacaoTabelaFrete.BuscarPorCodigo(codigoRegra);

                if (regrasTabelaFrete == null)
                    return new JsonpResult(false, "Não foi possível buscar a regra.");

                #region BuscaRegras
                List<Dominio.Entidades.Embarcador.Frete.RegrasMotivoReajuste> regrasMotivoReajuste = repRegrasMotivoReajuste.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.Frete.RegrasOrigemFrete> regrasOrigemFrete = repRegrasOrigemFrete.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.Frete.RegrasDestinoFrete> regrasDestinoFrete = repRegrasDestinoFrete.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.Frete.RegrasTransportador> regrasTransportador = repRegrasTransportador.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.Frete.RegrasTipoOperacao> regrasTipoOperacao = repRegrasTipoOperacao.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.Frete.RegrasValorFrete> regrasValorFrete = repRegrasValorFrete.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.Frete.RegrasValorPedagio> regrasValorPedagio = repRegrasValorPedagio.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.Frete.RegrasAdValorem> regrasAdValorem = repRegrasAdValorem.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.Frete.RegrasFilial> regrasFilial = repRegrasFilial.BuscarPorRegras(codigoRegra);
                #endregion


                #region Deleta Regras
                for (var i = 0; i < regrasMotivoReajuste.Count(); i++) repRegrasMotivoReajuste.Deletar(regrasMotivoReajuste[i]);
                for (var i = 0; i < regrasOrigemFrete.Count(); i++) repRegrasOrigemFrete.Deletar(regrasOrigemFrete[i]);
                for (var i = 0; i < regrasDestinoFrete.Count(); i++) repRegrasDestinoFrete.Deletar(regrasDestinoFrete[i]);
                for (var i = 0; i < regrasTransportador.Count(); i++) repRegrasTransportador.Deletar(regrasTransportador[i]);
                for (var i = 0; i < regrasTipoOperacao.Count(); i++) repRegrasTipoOperacao.Deletar(regrasTipoOperacao[i]);
                for (var i = 0; i < regrasValorFrete.Count(); i++) repRegrasValorFrete.Deletar(regrasValorFrete[i]);
                for (var i = 0; i < regrasValorPedagio.Count(); i++) repRegrasValorPedagio.Deletar(regrasValorPedagio[i]);
                for (var i = 0; i < regrasAdValorem.Count(); i++) repRegrasAdValorem.Deletar(regrasAdValorem[i]);
                for (var i = 0; i < regrasFilial.Count; i++) repRegrasFilial.Deletar(regrasFilial[i]);
                #endregion


                #region Novas Regras
                regrasMotivoReajuste = new List<Dominio.Entidades.Embarcador.Frete.RegrasMotivoReajuste>();
                regrasOrigemFrete = new List<Dominio.Entidades.Embarcador.Frete.RegrasOrigemFrete>();
                regrasDestinoFrete = new List<Dominio.Entidades.Embarcador.Frete.RegrasDestinoFrete>();
                regrasTransportador = new List<Dominio.Entidades.Embarcador.Frete.RegrasTransportador>();
                regrasTipoOperacao = new List<Dominio.Entidades.Embarcador.Frete.RegrasTipoOperacao>();
                regrasValorFrete = new List<Dominio.Entidades.Embarcador.Frete.RegrasValorFrete>();
                regrasValorPedagio = new List<Dominio.Entidades.Embarcador.Frete.RegrasValorPedagio>();
                regrasAdValorem = new List<Dominio.Entidades.Embarcador.Frete.RegrasAdValorem>();
                regrasFilial = new List<Dominio.Entidades.Embarcador.Frete.RegrasFilial>();
                #endregion


                // Preenche a entidade
                PreencherEntidade(ref regrasTabelaFrete, unitOfWork);

                string erro = string.Empty;
                // Validar entidade
                if (!ValidarEntidade(regrasTabelaFrete, out erro))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, erro);
                }

                // Atualiza Entidade
                repRegrasAutorizacaoTabelaFrete.Atualizar(regrasTabelaFrete);

                try
                {
                    List<string> erros = new List<string>();
                    PreencherTodasRegras(ref regrasTabelaFrete, ref regrasMotivoReajuste, ref regrasOrigemFrete, ref regrasDestinoFrete, ref regrasTransportador, ref regrasTipoOperacao, ref regrasValorFrete, ref regrasValorPedagio, ref regrasAdValorem, ref regrasFilial, ref erros, unitOfWork);
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, ex.Message);
                }

                // Insere regras
                for (var i = 0; i < regrasMotivoReajuste.Count; i++) repRegrasMotivoReajuste.Inserir(regrasMotivoReajuste[i]);
                for (var i = 0; i < regrasOrigemFrete.Count; i++) repRegrasOrigemFrete.Inserir(regrasOrigemFrete[i]);
                for (var i = 0; i < regrasDestinoFrete.Count; i++) repRegrasDestinoFrete.Inserir(regrasDestinoFrete[i]);
                for (var i = 0; i < regrasTransportador.Count; i++) repRegrasTransportador.Inserir(regrasTransportador[i]);
                for (var i = 0; i < regrasTipoOperacao.Count; i++) repRegrasTipoOperacao.Inserir(regrasTipoOperacao[i]);
                for (var i = 0; i < regrasValorPedagio.Count; i++) repRegrasValorPedagio.Inserir(regrasValorPedagio[i]);
                for (var i = 0; i < regrasAdValorem.Count; i++) repRegrasAdValorem.Inserir(regrasAdValorem[i]);
                for (var i = 0; i < regrasValorFrete.Count; i++) repRegrasValorFrete.Inserir(regrasValorFrete[i]);
                for (var i = 0; i < regrasFilial.Count; i++) repRegrasFilial.Inserir(regrasFilial[i]);

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
                Repositorio.Embarcador.Frete.RegrasAutorizacaoTabelaFrete repRegrasAutorizacaoTabelaFrete = new Repositorio.Embarcador.Frete.RegrasAutorizacaoTabelaFrete(unitOfWork);
                Repositorio.Embarcador.Frete.RegrasMotivoReajuste repRegrasMotivoReajuste = new Repositorio.Embarcador.Frete.RegrasMotivoReajuste(unitOfWork);
                Repositorio.Embarcador.Frete.RegrasOrigemFrete repRegrasOrigemFrete = new Repositorio.Embarcador.Frete.RegrasOrigemFrete(unitOfWork);
                Repositorio.Embarcador.Frete.RegrasDestinoFrete repRegrasDestinoFrete = new Repositorio.Embarcador.Frete.RegrasDestinoFrete(unitOfWork);
                Repositorio.Embarcador.Frete.RegrasTransportador repRegrasTransportador = new Repositorio.Embarcador.Frete.RegrasTransportador(unitOfWork);
                Repositorio.Embarcador.Frete.RegrasTipoOperacao repRegrasTipoOperacao = new Repositorio.Embarcador.Frete.RegrasTipoOperacao(unitOfWork);
                Repositorio.Embarcador.Frete.RegrasValorFrete repRegrasValorFrete = new Repositorio.Embarcador.Frete.RegrasValorFrete(unitOfWork);
                Repositorio.Embarcador.Frete.RegrasValorPedagio repRegrasValorPedagio = new Repositorio.Embarcador.Frete.RegrasValorPedagio(unitOfWork);
                Repositorio.Embarcador.Frete.RegrasAdValorem repRegrasAdValorem = new Repositorio.Embarcador.Frete.RegrasAdValorem(unitOfWork);
                Repositorio.Embarcador.Frete.RegrasFilial repRegrasFilial = new Repositorio.Embarcador.Frete.RegrasFilial(unitOfWork);

                // Codigo da busca 
                int codigoRegra = 0;
                int.TryParse(Request.Params("Codigo"), out codigoRegra);

                // Busca entidade
                Dominio.Entidades.Embarcador.Frete.RegrasAutorizacaoTabelaFrete regrasTabelaFrete = repRegrasAutorizacaoTabelaFrete.BuscarPorCodigo(codigoRegra);

                if (regrasTabelaFrete == null)
                    return new JsonpResult(false, "Não foi possível buscar a regra.");

                #region BuscaRegras
                List<Dominio.Entidades.Embarcador.Frete.RegrasMotivoReajuste> regrasMotivoReajuste = repRegrasMotivoReajuste.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.Frete.RegrasOrigemFrete> regrasOrigemFrete = repRegrasOrigemFrete.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.Frete.RegrasDestinoFrete> regrasDestinoFrete = repRegrasDestinoFrete.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.Frete.RegrasTransportador> regrasTransportador = repRegrasTransportador.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.Frete.RegrasTipoOperacao> regrasTipoOperacao = repRegrasTipoOperacao.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.Frete.RegrasValorFrete> regrasValorFrete = repRegrasValorFrete.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.Frete.RegrasValorPedagio> regrasValorPedagio = repRegrasValorPedagio.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.Frete.RegrasAdValorem> regrasAdValorem = repRegrasAdValorem.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.Frete.RegrasFilial> regrasFilial = repRegrasFilial.BuscarPorRegras(codigoRegra);
                #endregion

                List<Dominio.Entidades.Usuario> aprovadores = (regrasTabelaFrete.TipoAprovadorRegra == TipoAprovadorRegra.Usuario) ? regrasTabelaFrete.Aprovadores.ToList() : new List<Dominio.Entidades.Usuario>();

                var dynRegra = new
                {
                    regrasTabelaFrete.Codigo,
                    regrasTabelaFrete.NumeroAprovadores,
                    regrasTabelaFrete.PrioridadeAprovacao,
                    Status = regrasTabelaFrete.Ativo,
                    Vigencia = regrasTabelaFrete.Vigencia.HasValue ? regrasTabelaFrete.Vigencia.Value.ToString("dd/MM/yyyy") : string.Empty,
                    Descricao = !string.IsNullOrWhiteSpace(regrasTabelaFrete.Descricao) ? regrasTabelaFrete.Descricao : string.Empty,
                    Observacao = !string.IsNullOrWhiteSpace(regrasTabelaFrete.Observacoes) ? regrasTabelaFrete.Observacoes : string.Empty,
                    TabelaFrete = new { Codigo = regrasTabelaFrete.TabelaFrete?.Codigo ?? 0, Descricao = regrasTabelaFrete.TabelaFrete?.Descricao ?? "" },
                    EtapaAutorizacao = regrasTabelaFrete.EtapaAutorizacaoTabelaFrete,
                    regrasTabelaFrete.TipoAprovadorRegra,

                    Aprovadores = (from o in aprovadores select new { o.Codigo, o.Nome }).ToList(),

                    UsarRegraPorMotivoReajuste = regrasTabelaFrete.RegraPorMotivoReajuste,
                    MotivoReajuste = (from obj in regrasMotivoReajuste select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Frete.RegrasMotivoReajuste>(obj, "MotivoReajuste", "Descricao")).ToList(),

                    UsarRegraPorOrigemFrete = regrasTabelaFrete.RegraPorOrigemFrete,
                    OrigemFrete = (from obj in regrasOrigemFrete select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Frete.RegrasOrigemFrete>(obj, "OrigemFrete", "Descricao")).ToList(),

                    UsarRegraPorDestinoFrete = regrasTabelaFrete.RegraPorDestinoFrete,
                    DestinoFrete = (from obj in regrasDestinoFrete select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Frete.RegrasDestinoFrete>(obj, "DestinoFrete", "Descricao")).ToList(),

                    UsarRegraPorTransportador = regrasTabelaFrete.RegraPorTransportador,
                    Transportador = (from obj in regrasTransportador select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Frete.RegrasTransportador>(obj, "Transportador", "RazaoSocial")).ToList(),

                    UsarRegraPorTipoOperacao = regrasTabelaFrete.RegraPorTipoOperacao,
                    TipoOperacao = (from obj in regrasTipoOperacao select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Frete.RegrasTipoOperacao>(obj, "TipoOperacao", "Descricao")).ToList(),

                    UsarRegraPorValorFrete = regrasTabelaFrete.RegraPorValorFrete,
                    ValorFrete = (from obj in regrasValorFrete select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Frete.RegrasValorFrete>(obj, "Valor", "Valor", true)).ToList(),

                    UsarRegraPorValorPedagio = regrasTabelaFrete.RegraPorValorPedagio,
                    ValorPedagio = (from obj in regrasValorPedagio select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Frete.RegrasValorPedagio>(obj, "Valor", "Valor", true)).ToList(),

                    UsarRegraPorAdValorem = regrasTabelaFrete.RegraPorAdValorem,
                    AdValorem = (from obj in regrasAdValorem select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Frete.RegrasAdValorem>(obj, "Valor", "Valor", true)).ToList(),

                    UsarRegraPorFilial = regrasTabelaFrete.RegraPorFilial,
                    Filial = (from obj in regrasFilial select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Frete.RegrasFilial>(obj, "Filial", "Descricao")).ToList(),

                    EnviarLinkParaAprovacaoPorEmail = regrasTabelaFrete.EnviarLinkParaAprovacaoPorEmail
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
                Repositorio.Embarcador.Frete.RegrasAutorizacaoTabelaFrete repRegrasAutorizacaoTabelaFrete = new Repositorio.Embarcador.Frete.RegrasAutorizacaoTabelaFrete(unitOfWork);

                // Codigo da busca 
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca entidade
                Dominio.Entidades.Embarcador.Frete.RegrasAutorizacaoTabelaFrete regrasTabelaFrete = repRegrasAutorizacaoTabelaFrete.BuscarPorCodigo(codigo);

                if (regrasTabelaFrete == null)
                    return new JsonpResult(false, "Não foi possível buscar a regra.");

                // Inicia transicao
                unitOfWork.Start();

                regrasTabelaFrete.Aprovadores.Clear();
                regrasTabelaFrete.RegrasMotivoReajuste.Clear();
                regrasTabelaFrete.RegrasOrigemFrete.Clear();
                regrasTabelaFrete.RegrasDestinoFrete.Clear();
                regrasTabelaFrete.RegrasTransportador.Clear();
                regrasTabelaFrete.RegrasTipoOperacao.Clear();
                regrasTabelaFrete.RegrasValorFrete.Clear();
                regrasTabelaFrete.RegrasValorPedagio.Clear();
                regrasTabelaFrete.RegrasAdValorem.Clear();
                regrasTabelaFrete.RegrasFilial.Clear();

                repRegrasAutorizacaoTabelaFrete.Deletar(regrasTabelaFrete);

                // Comita alteracoes
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid GridPesquisa()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Descrição", "Descricao", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Tabela de Frete", "TabelaFrete", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Etapa", "DescricaoEtapaAutorizacaoTabelaFrete", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Vigência", "Vigencia", 15, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Status", "DescricaoAtivo", 15, Models.Grid.Align.left, true);

            return grid;
        }

        private void PreencherEntidade(ref Dominio.Entidades.Embarcador.Frete.RegrasAutorizacaoTabelaFrete regrasTabelaFrete, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia Repositorios/Entidade
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Embarcador.Frete.TabelaFrete repTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unitOfWork);


            // Converte parametros
            string descricao = !string.IsNullOrWhiteSpace(Request.Params("Descricao")) ? Request.Params("Descricao") : string.Empty;
            string observacao = !string.IsNullOrWhiteSpace(Request.Params("Observacao")) ? Request.Params("Observacao") : string.Empty;

            DateTime dataVigenciaAux;
            DateTime? dataVigencia = null;

            if (DateTime.TryParseExact(Request.Params("Vigencia"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataVigenciaAux))
                dataVigencia = dataVigenciaAux;

            EtapaAutorizacaoTabelaFrete etapaAutorizacaoTabelaFrete;
            Enum.TryParse(Request.Params("EtapaAutorizacao"), out etapaAutorizacaoTabelaFrete);

            int numeroAprovadores = 0;
            int.TryParse(Request.Params("NumeroAprovadores"), out numeroAprovadores);

            int prioridadeAprovacao = 0;
            int.TryParse(Request.Params("PrioridadeAprovacao"), out prioridadeAprovacao);


            int codigoTabela = 0;
            int.TryParse(Request.Params("TabelaFrete"), out codigoTabela);
            Dominio.Entidades.Embarcador.Frete.TabelaFrete tabela = repTabelaFrete.BuscarPorCodigo(codigoTabela);

            bool.TryParse(Request.Params("UsarRegraPorMotivoReajuste"), out bool usarRegraPorMotivoReajuste);
            bool.TryParse(Request.Params("UsarRegraPorOrigemFrete"), out bool usarRegraPorOrigemFrete);
            bool.TryParse(Request.Params("UsarRegraPorDestinoFrete"), out bool usarRegraPorDestinoFrete);
            bool.TryParse(Request.Params("UsarRegraPorTransportador"), out bool usarRegraPorTransportador);
            bool.TryParse(Request.Params("UsarRegraPorTipoOperacao"), out bool usarRegraPorTipoOperacao);
            bool.TryParse(Request.Params("UsarRegraPorValorFrete"), out bool usarRegraPorValorFrete);
            bool.TryParse(Request.Params("UsarRegraPorValorPedagio"), out bool usarRegraPorValorPedagio);
            bool.TryParse(Request.Params("UsarRegraPorAdValorem"), out bool usarRegraPorAdValorem);
            bool.TryParse(Request.Params("UsarRegraPorFilial"), out bool usarRegraPorFilial);

            // Seta na entidade
            regrasTabelaFrete.Ativo = Request.GetBoolParam("Status");
            regrasTabelaFrete.Descricao = descricao;
            regrasTabelaFrete.Observacoes = observacao;
            regrasTabelaFrete.Vigencia = dataVigencia;
            regrasTabelaFrete.NumeroAprovadores = numeroAprovadores;
            regrasTabelaFrete.PrioridadeAprovacao = prioridadeAprovacao;
            regrasTabelaFrete.EtapaAutorizacaoTabelaFrete = etapaAutorizacaoTabelaFrete;
            regrasTabelaFrete.TipoAprovadorRegra = Request.GetEnumParam<TipoAprovadorRegra>("TipoAprovadorRegra");
            regrasTabelaFrete.TabelaFrete = tabela;

            regrasTabelaFrete.RegraPorMotivoReajuste = usarRegraPorMotivoReajuste;
            regrasTabelaFrete.RegraPorOrigemFrete = usarRegraPorOrigemFrete;
            regrasTabelaFrete.RegraPorDestinoFrete = usarRegraPorDestinoFrete;
            regrasTabelaFrete.RegraPorTransportador = usarRegraPorTransportador;
            regrasTabelaFrete.RegraPorTipoOperacao = usarRegraPorTipoOperacao;
            regrasTabelaFrete.RegraPorValorFrete = usarRegraPorValorFrete;
            regrasTabelaFrete.RegraPorValorPedagio = usarRegraPorValorPedagio;
            regrasTabelaFrete.RegraPorAdValorem = usarRegraPorAdValorem;
            regrasTabelaFrete.RegraPorFilial = usarRegraPorFilial;
            regrasTabelaFrete.EnviarLinkParaAprovacaoPorEmail = Request.GetBoolParam("EnviarLinkParaAprovacaoPorEmail");

            if (regrasTabelaFrete.TipoAprovadorRegra == TipoAprovadorRegra.Usuario)
            {
                List<int> codigosUsuarios = new List<int>();
                if (!string.IsNullOrWhiteSpace(Request.Params("Aprovadores")))
                {
                    List<ObjetoAprovadores> dynAprovadores = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ObjetoAprovadores>>(Request.Params("Aprovadores"));

                    for (var i = 0; i < dynAprovadores.Count(); i++)
                        codigosUsuarios.Add(dynAprovadores[i].Codigo);
                }

                regrasTabelaFrete.Aprovadores = repUsuario.BuscarUsuariosPorCodigos(codigosUsuarios.ToArray(), null);
            }
            else
                regrasTabelaFrete.Aprovadores = new List<Dominio.Entidades.Usuario>();
        }

        private void PreencherEntidadeRegra<T>(string nomePropriedade, string parametroJson, bool usarDynamic, ref List<T> regrasPorTipo, ref Dominio.Entidades.Embarcador.Frete.RegrasAutorizacaoTabelaFrete regrasTabelaFrete, Func<dynamic, object> lambda = null)
        {
            /* Descricao
             * regrasTabelaFrete é passado com ref, pois é vinculado a regra específica (RegraPorTipo) e após inserir no banco, a referencia permanece com o Codigo válido
             * 
             * Esse método facilita a instancia de novas regras, já que todas possuem o mesmo padra
             * - RegrasTabelaFrete (Entidade Pai)
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

                prop = regra.GetType().GetProperty("RegrasAutorizacaoTabelaFrete", BindingFlags.Public | BindingFlags.Instance);
                prop.SetValue(regra, regrasTabelaFrete, null);

                prop = regra.GetType().GetProperty("Ordem", BindingFlags.Public | BindingFlags.Instance);
                prop.SetValue(regra, dynRegras[i].Ordem, null);

                prop = regra.GetType().GetProperty("Condicao", BindingFlags.Public | BindingFlags.Instance);
                prop.SetValue(regra, dynRegras[i].Condicao, null);

                prop = regra.GetType().GetProperty("Juncao", BindingFlags.Public | BindingFlags.Instance);
                prop.SetValue(regra, dynRegras[i].Juncao, null);

                try
                {
                    // Englobado num try catch por o parametro Tipo é opcional
                    prop = regra.GetType().GetProperty("TipoRegra", BindingFlags.Public | BindingFlags.Instance);
                    prop.SetValue(regra, dynRegras[i].TipoRegra, null);
                } 
                catch (Exception ex) 
                {
                    Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao definir TipoRegra na regra de autorização de tabela de frete: {ex.ToString()}", "CatchNoAction");
                }
                

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
                    {
                        decimal valorDecimal = 0;
                        decimal.TryParse(dynRegras[i].Valor.ToString(), out valorDecimal);

                        prop.SetValue(regra, valorDecimal, null);
                    }
                    else
                    {
                        prop.SetValue(regra, dynRegras[i].Valor, null);
                    }
                }

                // Adiciona lista de retorno
                regrasPorTipo.Add(regra);
            }

        }

        private bool ValidarEntidade(Dominio.Entidades.Embarcador.Frete.RegrasAutorizacaoTabelaFrete regrasTabelaFrete, out string erro)
        {
            erro = string.Empty;

            if (string.IsNullOrWhiteSpace(regrasTabelaFrete.Descricao))
            {
                erro = "Descrição é obrigatória.";
                return false;
            }

            if ((regrasTabelaFrete.TipoAprovadorRegra == TipoAprovadorRegra.Usuario) && (regrasTabelaFrete.Aprovadores.Count() < regrasTabelaFrete.NumeroAprovadores))
            {
                erro = "O número de aprovadores selecionados deve ser maior ou igual a " + regrasTabelaFrete.NumeroAprovadores.ToString();
                return false;
            }

            //if (regrasTabelaFrete.TabelaFrete == null)
            //{
            //    erro = "Tabela de Frete é obrigatória.";
            //    return false;
            //}

            return true;
        }

        private bool ValidarEntidadeRegra<T>(string nomeRegra, string nomePropriedade, List<T> regrasPorTipo, out List<string> erros)
        {
            erros = new List<string>();

            if (regrasPorTipo.Count() == 0)
                erros.Add("Nenhuma regra " + nomeRegra + " cadastrada.");
            else
            {
                // Variavel auxiliar
                PropertyInfo prop;

                // Itera validacao
                for (var i = 0; i < regrasPorTipo.Count(); i++)
                {
                    var regra = regrasPorTipo[i];
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
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoValorFrete juncao = prop.GetValue(obj);

            prop = obj.GetType().GetProperty("Condicao", BindingFlags.Public | BindingFlags.Instance);
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoValorFrete condicao = prop.GetValue(obj);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoValorAutorizaoValorFrete tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoValorAutorizaoValorFrete.ValorFixo;
            try
            {
                prop = obj.GetType().GetProperty("TipoRegra", BindingFlags.Public | BindingFlags.Instance);
                tipo = prop.GetValue(obj);
            }
            catch (Exception ex) 
            {
                Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao obter TipoRegra da regra de autorização de tabela de frete: {ex.ToString()}", "CatchNoAction");
            }


            ObjetoEntidade objetoEntidade = new ObjetoEntidade();
            dynamic valor = null;
            if (!usarValor)
            {
                prop = obj.GetType().GetProperty(paramentro, BindingFlags.Public | BindingFlags.Instance);
                dynamic entidade = prop.GetValue(obj);

                prop = entidade.GetType().GetProperty("Codigo", BindingFlags.Public | BindingFlags.Instance);
                dynamic codigoEntidade = prop.GetValue(entidade);

                prop = entidade.GetType().GetProperty(paramentroDescricaoValor, BindingFlags.Public | BindingFlags.Instance);
                string descricaoEntidade = prop.GetValue(entidade);

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
                TipoRegra = tipo,
                Condicao = condicao,
                Entidade = objetoEntidade,
                Valor = valor,
            };
            return restorno;
        }

        private void PreencherTodasRegras(ref Dominio.Entidades.Embarcador.Frete.RegrasAutorizacaoTabelaFrete regrasTabelaFrete, ref List<Dominio.Entidades.Embarcador.Frete.RegrasMotivoReajuste> regrasMotivoReajuste, ref List<Dominio.Entidades.Embarcador.Frete.RegrasOrigemFrete> regrasOrigemFrete, ref List<Dominio.Entidades.Embarcador.Frete.RegrasDestinoFrete> regrasDestinoFrete, ref List<Dominio.Entidades.Embarcador.Frete.RegrasTransportador> regrasTransportador, ref List<Dominio.Entidades.Embarcador.Frete.RegrasTipoOperacao> regrasTipoOperacao, ref List<Dominio.Entidades.Embarcador.Frete.RegrasValorFrete> regrasValorFrete, ref List<Dominio.Entidades.Embarcador.Frete.RegrasValorPedagio> regrasValorPedagio, ref List<Dominio.Entidades.Embarcador.Frete.RegrasAdValorem> regrasAdValorem, ref List<Dominio.Entidades.Embarcador.Frete.RegrasFilial> regrasFilial, ref List<string> errosRegras, Repositorio.UnitOfWork unitOfWork)
        {
            // Erros de validacao
            List<string> erros = new List<string>();

            #region MotivoReajuste
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regrasTabelaFrete.RegraPorMotivoReajuste)
            {
                // Preenche regra
                try
                {
                    PreencherEntidadeRegra("MotivoReajuste", "RegrasMotivoReajuste", false, ref regrasMotivoReajuste, ref regrasTabelaFrete, ((codigo) => {
                        Repositorio.Embarcador.Frete.MotivoReajuste repMotivoReajuste = new Repositorio.Embarcador.Frete.MotivoReajuste(unitOfWork);

                        int codigoInt = 0;
                        int.TryParse(codigo.ToString(), out codigoInt);

                        return repMotivoReajuste.BuscarPorCodigo(codigoInt);
                    }));
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                    errosRegras.Add("Motivo da Reajuste");
                }

                // Valida regra (se for invalida, nao continua o fluxo)
                if (!ValidarEntidadeRegra("Motivo do Reajuste", "MotivoReajuste", regrasMotivoReajuste, out erros))
                    throw new Exception(String.Join("<br>", erros));
            }
            #endregion

            #region OrigemFrete
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regrasTabelaFrete.RegraPorOrigemFrete)
            {
                // Preenche regra
                try
                {
                    PreencherEntidadeRegra("OrigemFrete", "RegrasOrigemFrete", false, ref regrasOrigemFrete, ref regrasTabelaFrete, ((codigo) => {
                        Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

                        int codigoInt = 0;
                        int.TryParse(codigo.ToString(), out codigoInt);

                        return repLocalidade.BuscarPorCodigo(codigoInt);
                    }));
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                    errosRegras.Add("Origem");
                }

                // Valida regra (se for invalida, nao continua o fluxo)
                if (!ValidarEntidadeRegra("Origem", "OrigemFrete", regrasOrigemFrete, out erros))
                    throw new Exception(String.Join("<br>", erros));
            }
            #endregion

            #region DestinoFrete
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regrasTabelaFrete.RegraPorDestinoFrete)
            {
                // Preenche regra
                try
                {
                    PreencherEntidadeRegra("DestinoFrete", "RegrasDestinoFrete", false, ref regrasDestinoFrete, ref regrasTabelaFrete, ((codigo) => {
                        Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

                        int codigoInt = 0;
                        int.TryParse(codigo.ToString(), out codigoInt);

                        return repLocalidade.BuscarPorCodigo(codigoInt);
                    }));
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                    errosRegras.Add("Destino");
                }

                // Valida regra (se for invalida, nao continua o fluxo)
                if (!ValidarEntidadeRegra("Destino", "DestinoFrete", regrasDestinoFrete, out erros))
                    throw new Exception(String.Join("<br>", erros));
            }
            #endregion

            #region Transportador
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regrasTabelaFrete.RegraPorTransportador)
            {
                // Preenche regra
                try
                {
                    PreencherEntidadeRegra("Transportador", "RegrasTransportador", false, ref regrasTransportador, ref regrasTabelaFrete, ((codigo) => {
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

                // Valida regra (se for invalida, nao continua o fluxo)
                if (!ValidarEntidadeRegra("Transportador", "Transportador", regrasTransportador, out erros))
                    throw new Exception(String.Join("<br>", erros));
            }
            #endregion

            #region TipoOperacao
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regrasTabelaFrete.RegraPorTipoOperacao)
            {
                // Preenche regra
                try
                {
                    PreencherEntidadeRegra("TipoOperacao", "RegrasTipoOperacao", false, ref regrasTipoOperacao, ref regrasTabelaFrete, ((codigo) => {
                        Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

                        int codigoInt = 0;
                        int.TryParse(codigo.ToString(), out codigoInt);

                        return repTipoOperacao.BuscarPorCodigo(codigoInt);
                    }));
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                    errosRegras.Add("Tipo de Operação");
                }

                // Valida regra (se for invalida, nao continua o fluxo)
                if (!ValidarEntidadeRegra("Tipo de Operação", "TipoOperacao", regrasTipoOperacao, out erros))
                    throw new Exception(String.Join("<br>", erros));
            }
            #endregion

            #region RegrasValorFrete
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regrasTabelaFrete.RegraPorValorFrete)
            {
                // Preenche regra
                try
                {
                    PreencherEntidadeRegra("Valor", "RegrasValorFrete", true, ref regrasValorFrete, ref regrasTabelaFrete);
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                    errosRegras.Add("Valor do Frete");
                }

                // Valida regra (se for invalida, nao continua o fluxo)
                if (!ValidarEntidadeRegra("Valor do Frete", "Valor", regrasValorFrete, out erros))
                    throw new Exception(String.Join("<br>", erros));
            }
            #endregion

            #region RegrasValorPedagio
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regrasTabelaFrete.RegraPorValorPedagio)
            {
                // Preenche regra
                try
                {
                    PreencherEntidadeRegra("Valor", "RegrasValorPedagio", true, ref regrasValorPedagio, ref regrasTabelaFrete);
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                    errosRegras.Add("Valor do Pedágio");
                }

                // Valida regra (se for invalida, nao continua o fluxo)
                if (!ValidarEntidadeRegra("Valor do Pedágio", "Valor", regrasValorPedagio, out erros))
                    throw new Exception(String.Join("<br>", erros));
            }
            #endregion

            #region RegraPorAdValorem
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regrasTabelaFrete.RegraPorAdValorem)
            {
                // Preenche regra
                try
                {
                    PreencherEntidadeRegra("Valor", "RegrasAdValorem", true, ref regrasAdValorem, ref regrasTabelaFrete);
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                    errosRegras.Add("AdValorem");
                }

                // Valida regra (se for invalida, nao continua o fluxo)
                if (!ValidarEntidadeRegra("AdValorem", "Valor", regrasAdValorem, out erros))
                    throw new Exception(String.Join("<br>", erros));
            }
            #endregion

            #region RegraPorFilial
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regrasTabelaFrete.RegraPorFilial)
            {
                // Preenche regra
                try
                {
                    PreencherEntidadeRegra("Filial", "RegrasFilial", false, ref regrasFilial, ref regrasTabelaFrete, ((codigo) => {
                        Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);

                        int codigoInt = 0;
                        int.TryParse(codigo.ToString(), out codigoInt);

                        return repFilial.BuscarPorCodigo(codigoInt);
                    }));
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                    errosRegras.Add("Filial");
                }

                // Valida regra (se for invalida, nao continua o fluxo)
                if (!ValidarEntidadeRegra("Filial", "Filial", regrasFilial, out erros))
                    throw new Exception(String.Join("<br>", erros));
            }
            #endregion
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Frete.RegrasAutorizacaoTabelaFrete repRegrasAutorizacaoTabelaFrete = new Repositorio.Embarcador.Frete.RegrasAutorizacaoTabelaFrete(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            // Converte parametros
            int codigoAprovador = 0;
            int.TryParse(Request.Params("Aprovador"), out codigoAprovador);

            int codigoTabela = 0;
            int.TryParse(Request.Params("TabelaFrete"), out codigoTabela);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoTabelaFrete etapaAutorizacaoTabelaFrete;
            Enum.TryParse(Request.Params("EtapaAutorizacao"), out etapaAutorizacaoTabelaFrete);

            DateTime dataInicioAux, dataFimAux;
            DateTime? dataInicio = null, dataFim = null;

            if (DateTime.TryParseExact(Request.Params("DataInicio"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicioAux))
                dataInicio = dataInicioAux;

            if (DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFimAux))
                dataFim = dataFimAux;

            string descricao = !string.IsNullOrWhiteSpace(Request.Params("Descricao")) ? Request.Params("Descricao") : "";
            Dominio.Entidades.Usuario aprovador = repUsuario.BuscarPorCodigo(codigoAprovador);
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacao = Request.GetEnumParam("Status", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo);

            // Consulta
            List<Dominio.Entidades.Embarcador.Frete.RegrasAutorizacaoTabelaFrete> listaGrid = repRegrasAutorizacaoTabelaFrete.ConsultarRegras(dataInicio, dataFim, aprovador, codigoTabela, descricao, etapaAutorizacaoTabelaFrete, situacao, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repRegrasAutorizacaoTabelaFrete.ContarConsultaRegras(dataInicio, dataFim, aprovador, codigoTabela, descricao, etapaAutorizacaoTabelaFrete, situacao);


            var lista = (from obj in listaGrid
                         select new
                         {
                             obj.Codigo,
                             Descricao = !string.IsNullOrWhiteSpace(obj.Descricao) ? obj.Descricao : string.Empty,
                             obj.DescricaoEtapaAutorizacaoTabelaFrete,
                             obj.DescricaoAtivo,
                             TabelaFrete = obj.TabelaFrete?.Descricao ?? string.Empty,
                             Vigencia = obj.Vigencia.HasValue ? obj.Vigencia.Value.ToString("dd/MM/yyyy") : string.Empty,
                         }).ToList();

            return lista.ToList();
        }
        
        #endregion
    }
}

