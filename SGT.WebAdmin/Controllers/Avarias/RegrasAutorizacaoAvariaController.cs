using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;


namespace SGT.WebAdmin.Controllers.Avarias
{
    [CustomAuthorize("Avarias/RegrasAutorizacaoAvaria")]
    public class RegrasAutorizacaoAvariaController : BaseController
    {
		#region Construtores

		public RegrasAutorizacaoAvariaController(Conexao conexao) : base(conexao) { }

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
            public Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria Condicao { get; set; }
            public Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria Juncao { get; set; }
            public ObjetoEntidade Entidade { get; set; }
            public dynamic Valor { get; set; }
        }
        #endregion

        private Models.Grid.Grid GridPesquisa()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Descrição", "Descricao", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Etapa", "DescricaoEtapaAutorizacaoAvaria", 15, Models.Grid.Align.left, true);
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

                // Instancia Repositorios/Entidade
                Repositorio.Embarcador.Avarias.RegrasAutorizacaoAvaria repRegrasAutorizacaoOcorrencia = new Repositorio.Embarcador.Avarias.RegrasAutorizacaoAvaria(unitOfWork);
                Repositorio.Embarcador.Avarias.RegrasAvariaMotivoAvaria repRegrasAvariaMotivoAvaria = new Repositorio.Embarcador.Avarias.RegrasAvariaMotivoAvaria(unitOfWork);
                Repositorio.Embarcador.Avarias.RegrasAvariaOrigem repRegrasAvariaOrigem = new Repositorio.Embarcador.Avarias.RegrasAvariaOrigem(unitOfWork);
                Repositorio.Embarcador.Avarias.RegrasAvariaDestino repRegrasAvariaDestino = new Repositorio.Embarcador.Avarias.RegrasAvariaDestino(unitOfWork);
                Repositorio.Embarcador.Avarias.RegrasAvariaFilial repRegrasAvariaFilial = new Repositorio.Embarcador.Avarias.RegrasAvariaFilial(unitOfWork);
                Repositorio.Embarcador.Avarias.RegrasAvariaTransportadora repRegrasAvariaTransportadora = new Repositorio.Embarcador.Avarias.RegrasAvariaTransportadora(unitOfWork);
                Repositorio.Embarcador.Avarias.RegrasAvariaTipoOperacao repRegrasTipoOperacao = new Repositorio.Embarcador.Avarias.RegrasAvariaTipoOperacao(unitOfWork);
                Repositorio.Embarcador.Avarias.RegrasAvariaValorAvaria repRegrasAvariaValorAvaria = new Repositorio.Embarcador.Avarias.RegrasAvariaValorAvaria(unitOfWork);

                // Nova entidade
                Dominio.Entidades.Embarcador.Avarias.RegrasAutorizacaoAvaria regrasAvaria = new Dominio.Entidades.Embarcador.Avarias.RegrasAutorizacaoAvaria();
                List<Dominio.Entidades.Embarcador.Avarias.RegrasMotivoAvaria> regraMotivoAvaria = new List<Dominio.Entidades.Embarcador.Avarias.RegrasMotivoAvaria>();
                List<Dominio.Entidades.Embarcador.Avarias.RegrasOrigem> regraOrigem = new List<Dominio.Entidades.Embarcador.Avarias.RegrasOrigem>();
                List<Dominio.Entidades.Embarcador.Avarias.RegrasDestino> regraDestino = new List<Dominio.Entidades.Embarcador.Avarias.RegrasDestino>();
                List<Dominio.Entidades.Embarcador.Avarias.RegrasFilial> regraFilial = new List<Dominio.Entidades.Embarcador.Avarias.RegrasFilial>();
                List<Dominio.Entidades.Embarcador.Avarias.RegrasTransportadora> regraTransportadora = new List<Dominio.Entidades.Embarcador.Avarias.RegrasTransportadora>();
                List<Dominio.Entidades.Embarcador.Avarias.RegrasTipoOperacao> regraTipoOperacao = new List<Dominio.Entidades.Embarcador.Avarias.RegrasTipoOperacao>();
                List<Dominio.Entidades.Embarcador.Avarias.RegrasValorAvaria> regraValorAvaria = new List<Dominio.Entidades.Embarcador.Avarias.RegrasValorAvaria>();

                // Preenche a entidade
                PreencherEntidade(ref regrasAvaria, unitOfWork);

                List<string> erros = new List<string>();
                // Validar entidade
                if (!ValidarEntidade(regrasAvaria, out erros))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, String.Join("<br>", erros));
                }

                try
                {
                    PreencherTodasRegras(ref regrasAvaria, ref regraMotivoAvaria, ref regraOrigem, ref regraDestino, ref regraFilial, ref regraTransportadora, ref regraTipoOperacao, ref regraValorAvaria, ref erros, unitOfWork);
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, ex.Message);
                }

                // Insere Entidade
                repRegrasAutorizacaoOcorrencia.Inserir(regrasAvaria, Auditado);

                // Insere regras
                for (var i = 0; i < regraMotivoAvaria.Count(); i++) repRegrasAvariaMotivoAvaria.Inserir(regraMotivoAvaria[i], Auditado);
                for (var i = 0; i < regraOrigem.Count(); i++) repRegrasAvariaOrigem.Inserir(regraOrigem[i], Auditado);
                for (var i = 0; i < regraDestino.Count(); i++) repRegrasAvariaDestino.Inserir(regraDestino[i], Auditado);
                for (var i = 0; i < regraFilial.Count(); i++) repRegrasAvariaFilial.Inserir(regraFilial[i], Auditado);
                for (var i = 0; i < regraTransportadora.Count(); i++) repRegrasAvariaTransportadora.Inserir(regraTransportadora[i], Auditado);
                for (var i = 0; i < regraTipoOperacao.Count(); i++) repRegrasTipoOperacao.Inserir(regraTipoOperacao[i], Auditado);
                for (var i = 0; i < regraValorAvaria.Count(); i++) repRegrasAvariaValorAvaria.Inserir(regraValorAvaria[i], Auditado);

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
                Repositorio.Embarcador.Avarias.RegrasAutorizacaoAvaria repRegrasAutorizacaoOcorrencia = new Repositorio.Embarcador.Avarias.RegrasAutorizacaoAvaria(unitOfWork);
                Repositorio.Embarcador.Avarias.RegrasAvariaMotivoAvaria repRegrasAvariaMotivoAvaria = new Repositorio.Embarcador.Avarias.RegrasAvariaMotivoAvaria(unitOfWork);
                Repositorio.Embarcador.Avarias.RegrasAvariaOrigem repRegrasAvariaOrigem = new Repositorio.Embarcador.Avarias.RegrasAvariaOrigem(unitOfWork);
                Repositorio.Embarcador.Avarias.RegrasAvariaDestino repRegrasAvariaDestino = new Repositorio.Embarcador.Avarias.RegrasAvariaDestino(unitOfWork);
                Repositorio.Embarcador.Avarias.RegrasAvariaFilial repRegrasAvariaFilial = new Repositorio.Embarcador.Avarias.RegrasAvariaFilial(unitOfWork);
                Repositorio.Embarcador.Avarias.RegrasAvariaTransportadora repRegrasAvariaTransportadora = new Repositorio.Embarcador.Avarias.RegrasAvariaTransportadora(unitOfWork);
                Repositorio.Embarcador.Avarias.RegrasAvariaTipoOperacao repRegrasTipoOperacao = new Repositorio.Embarcador.Avarias.RegrasAvariaTipoOperacao(unitOfWork);
                Repositorio.Embarcador.Avarias.RegrasAvariaValorAvaria repRegrasAvariaValorAvaria = new Repositorio.Embarcador.Avarias.RegrasAvariaValorAvaria(unitOfWork);

                // Nova entidade
                // Codigo da busca 
                int codigoRegra = 0;
                int.TryParse(Request.Params("Codigo"), out codigoRegra);

                // Busca entidade
                Dominio.Entidades.Embarcador.Avarias.RegrasAutorizacaoAvaria regrasAvaria = repRegrasAutorizacaoOcorrencia.BuscarPorCodigo(codigoRegra, true);

                if (regrasAvaria == null)
                    return new JsonpResult(false, "Não foi possível buscar a regra.");

                #region BuscaRegras
                List<Dominio.Entidades.Embarcador.Avarias.RegrasMotivoAvaria> regraMotivoAvaria = repRegrasAvariaMotivoAvaria.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.Avarias.RegrasOrigem> regraOrigem = repRegrasAvariaOrigem.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.Avarias.RegrasDestino> regraDestino = repRegrasAvariaDestino.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.Avarias.RegrasFilial> regraFilial = repRegrasAvariaFilial.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.Avarias.RegrasTransportadora> regraTransportadora = repRegrasAvariaTransportadora.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.Avarias.RegrasTipoOperacao> regraTipoOperacao = repRegrasTipoOperacao.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.Avarias.RegrasValorAvaria> regraValorAvaria = repRegrasAvariaValorAvaria.BuscarPorRegras(codigoRegra);
                #endregion


                #region Deleta Regras
                //for (var i = 0; i < regraMotivoAvaria.Count(); i++) repRegrasAvariaMotivoAvaria.Deletar(regraMotivoAvaria[i]);
                //for (var i = 0; i < regraOrigem.Count(); i++) repRegrasAvariaOrigem.Deletar(regraOrigem[i]);
                //for (var i = 0; i < regraDestino.Count(); i++) repRegrasAvariaDestino.Deletar(regraDestino[i]);
                //for (var i = 0; i < regraFilial.Count(); i++) repRegrasAvariaFilial.Deletar(regraFilial[i]);
                //for (var i = 0; i < regraTransportadora.Count(); i++) repRegrasAvariaTransportadora.Deletar(regraTransportadora[i]);
                //for (var i = 0; i < regraTipoOperacao.Count(); i++) repRegrasTipoOperacao.Deletar(regraTipoOperacao[i]);
                //for (var i = 0; i < regraValorAvaria.Count(); i++) repRegrasAvariaValorAvaria.Deletar(regraValorAvaria[i]);
                #endregion


                #region Novas Regras
                //regraMotivoAvaria = new List<Dominio.Entidades.Embarcador.Avarias.RegrasMotivoAvaria>();
                //regraOrigem = new List<Dominio.Entidades.Embarcador.Avarias.RegrasOrigem>();
                //regraDestino = new List<Dominio.Entidades.Embarcador.Avarias.RegrasDestino>();
                //regraFilial = new List<Dominio.Entidades.Embarcador.Avarias.RegrasFilial>();
                //regraTransportadora = new List<Dominio.Entidades.Embarcador.Avarias.RegrasTransportadora>();
                //regraTipoOperacao = new List<Dominio.Entidades.Embarcador.Avarias.RegrasTipoOperacao>();
                //regraValorAvaria = new List<Dominio.Entidades.Embarcador.Avarias.RegrasValorAvaria>();
                #endregion


                // Preenche a entidade
                PreencherEntidade(ref regrasAvaria, unitOfWork);

                List<string> erros = new List<string>();
                // Validar entidade
                if (!ValidarEntidade(regrasAvaria, out erros))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, String.Join("<br>", erros));
                }

                try
                {
                    PreencherTodasRegras(ref regrasAvaria, ref regraMotivoAvaria, ref regraOrigem, ref regraDestino, ref regraFilial, ref regraTransportadora, ref regraTipoOperacao, ref regraValorAvaria, ref erros, unitOfWork);
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, ex.Message);
                }

                // Insere regras
                #region Insere Regras
                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = new List<Dominio.Entidades.Auditoria.HistoricoPropriedade>();

                bool inseriuCriterio = false;
                for (var i = 0; i < regraMotivoAvaria.Count(); i++)
                {
                    if (regraMotivoAvaria[i].Codigo == 0)
                    {
                        repRegrasAvariaMotivoAvaria.Inserir(regraMotivoAvaria[i]);
                        inseriuCriterio = true;
                    }
                    else
                    {
                        alteracoes.AddRange(regraMotivoAvaria[i].GetChanges());
                        repRegrasAvariaMotivoAvaria.Atualizar(regraMotivoAvaria[i]);
                    }
                }
                if (inseriuCriterio)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, regrasAvaria, null, "Adicionou um critério de Motivo na regra.", unitOfWork);

                inseriuCriterio = false;
                for (var i = 0; i < regraOrigem.Count(); i++)
                {
                    if (regraOrigem[i].Codigo == 0)
                    {
                        repRegrasAvariaOrigem.Inserir(regraOrigem[i]);
                        inseriuCriterio = true;
                    }
                    else
                    {
                        alteracoes.AddRange(regraOrigem[i].GetChanges());
                        repRegrasAvariaOrigem.Atualizar(regraOrigem[i]);
                    }
                }
                if (inseriuCriterio)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, regrasAvaria, null, "Adicionou um critério de Origem na regra.", unitOfWork);

                inseriuCriterio = false;
                for (var i = 0; i < regraDestino.Count(); i++)
                {
                    if (regraDestino[i].Codigo == 0)
                    {
                        repRegrasAvariaDestino.Inserir(regraDestino[i]);
                        inseriuCriterio = true;
                    }
                    else
                    {
                        alteracoes.AddRange(regraDestino[i].GetChanges());
                        repRegrasAvariaDestino.Atualizar(regraDestino[i]);
                    }
                }
                if (inseriuCriterio)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, regrasAvaria, null, "Adicionou um critério de Destino na regra.", unitOfWork);

                inseriuCriterio = false;
                for (var i = 0; i < regraFilial.Count(); i++)
                {
                    if (regraFilial[i].Codigo == 0)
                    {
                        repRegrasAvariaFilial.Inserir(regraFilial[i]);
                        inseriuCriterio = true;
                    }
                    else
                    {
                        alteracoes.AddRange(regraFilial[i].GetChanges());
                        repRegrasAvariaFilial.Atualizar(regraFilial[i]);
                    }
                }
                if (inseriuCriterio)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, regrasAvaria, null, "Adicionou um critério de Filial na regra.", unitOfWork);

                inseriuCriterio = false;
                for (var i = 0; i < regraTransportadora.Count(); i++)
                {
                    if (regraTransportadora[i].Codigo == 0)
                    {
                        repRegrasAvariaTransportadora.Inserir(regraTransportadora[i]);
                        inseriuCriterio = true;
                    }
                    else
                    {
                        alteracoes.AddRange(regraTransportadora[i].GetChanges());
                        repRegrasAvariaTransportadora.Atualizar(regraTransportadora[i]);
                    }
                }
                if (inseriuCriterio)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, regrasAvaria, null, "Adicionou um critério de Transportadora na regra.", unitOfWork);

                inseriuCriterio = false;
                for (var i = 0; i < regraTipoOperacao.Count(); i++)
                {
                    if (regraTipoOperacao[i].Codigo == 0)
                    {
                        repRegrasTipoOperacao.Inserir(regraTipoOperacao[i]);
                        inseriuCriterio = true;
                    }
                    else
                    {
                        alteracoes.AddRange(regraTipoOperacao[i].GetChanges());
                        repRegrasTipoOperacao.Atualizar(regraTipoOperacao[i]);
                    }
                }
                if (inseriuCriterio)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, regrasAvaria, null, "Adicionou um critério de Tipo de Operação na regra.", unitOfWork);

                inseriuCriterio = false;
                for (var i = 0; i < regraValorAvaria.Count(); i++)
                {
                    if (regraValorAvaria[i].Codigo == 0)
                    {
                        repRegrasAvariaValorAvaria.Inserir(regraValorAvaria[i]);
                        inseriuCriterio = true;
                    }
                    else
                    {
                        alteracoes.AddRange(regraValorAvaria[i].GetChanges());
                        repRegrasAvariaValorAvaria.Atualizar(regraValorAvaria[i]);
                    }
                }
                if (inseriuCriterio)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, regrasAvaria, null, "Adicionou um critério de Valor na regra.", unitOfWork);


                // Atualiza Entidade
                repRegrasAutorizacaoOcorrencia.Atualizar(regrasAvaria, Auditado);
                if (alteracoes.Count > 0)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, regrasAvaria, alteracoes, "Alterou os critérios da regra.", unitOfWork);
                #endregion

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
                Repositorio.Embarcador.Avarias.RegrasAutorizacaoAvaria repRegrasAutorizacaoOcorrencia = new Repositorio.Embarcador.Avarias.RegrasAutorizacaoAvaria(unitOfWork);
                Repositorio.Embarcador.Avarias.RegrasAvariaMotivoAvaria repRegrasAvariaMotivoAvaria = new Repositorio.Embarcador.Avarias.RegrasAvariaMotivoAvaria(unitOfWork);
                Repositorio.Embarcador.Avarias.RegrasAvariaOrigem repRegrasAvariaOrigem = new Repositorio.Embarcador.Avarias.RegrasAvariaOrigem(unitOfWork);
                Repositorio.Embarcador.Avarias.RegrasAvariaDestino repRegrasAvariaDestino = new Repositorio.Embarcador.Avarias.RegrasAvariaDestino(unitOfWork);
                Repositorio.Embarcador.Avarias.RegrasAvariaFilial repRegrasAvariaFilial = new Repositorio.Embarcador.Avarias.RegrasAvariaFilial(unitOfWork);
                Repositorio.Embarcador.Avarias.RegrasAvariaTransportadora repRegrasAvariaTransportadora = new Repositorio.Embarcador.Avarias.RegrasAvariaTransportadora(unitOfWork);
                Repositorio.Embarcador.Avarias.RegrasAvariaTipoOperacao repRegrasAvariaTipoOperacao = new Repositorio.Embarcador.Avarias.RegrasAvariaTipoOperacao(unitOfWork);
                Repositorio.Embarcador.Avarias.RegrasAvariaValorAvaria repRegrasAvariaValorAvaria = new Repositorio.Embarcador.Avarias.RegrasAvariaValorAvaria(unitOfWork);

                // Codigo da busca 
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca entidade
                Dominio.Entidades.Embarcador.Avarias.RegrasAutorizacaoAvaria regrasAvaria = repRegrasAutorizacaoOcorrencia.BuscarPorCodigo(codigo);

                if (regrasAvaria == null)
                    return new JsonpResult(false, "Não foi possível buscar a regra.");

                #region BuscaRegras
                List<Dominio.Entidades.Embarcador.Avarias.RegrasMotivoAvaria> regraMotivoAvaria = repRegrasAvariaMotivoAvaria.BuscarPorRegras(codigo);
                List<Dominio.Entidades.Embarcador.Avarias.RegrasOrigem> regraOrigem = repRegrasAvariaOrigem.BuscarPorRegras(codigo);
                List<Dominio.Entidades.Embarcador.Avarias.RegrasDestino> regraDestino = repRegrasAvariaDestino.BuscarPorRegras(codigo);
                List<Dominio.Entidades.Embarcador.Avarias.RegrasFilial> regraFilial = repRegrasAvariaFilial.BuscarPorRegras(codigo);
                List<Dominio.Entidades.Embarcador.Avarias.RegrasTransportadora> regraTransportadora = repRegrasAvariaTransportadora.BuscarPorRegras(codigo);
                List<Dominio.Entidades.Embarcador.Avarias.RegrasTipoOperacao> regraTipoOperacao = repRegrasAvariaTipoOperacao.BuscarPorRegras(codigo);
                List<Dominio.Entidades.Embarcador.Avarias.RegrasValorAvaria> regraValorAvaria = repRegrasAvariaValorAvaria.BuscarPorRegras(codigo);
                #endregion


                var dynRegra = new
                {
                    regrasAvaria.Codigo,
                    regrasAvaria.NumeroAprovadores,
                    regrasAvaria.PrioridadeAprovacao,
                    Vigencia = regrasAvaria.Vigencia.HasValue ? regrasAvaria.Vigencia.Value.ToString("dd/MM/yyyy") : string.Empty,
                    Descricao = !string.IsNullOrWhiteSpace(regrasAvaria.Descricao) ? regrasAvaria.Descricao : string.Empty,
                    Observacao = !string.IsNullOrWhiteSpace(regrasAvaria.Observacoes) ? regrasAvaria.Observacoes : string.Empty,
                    EtapaAutorizacao = regrasAvaria.EtapaAutorizacaoAvaria,

                    Aprovadores = (from o in regrasAvaria.Aprovadores select new { o.Codigo, o.Nome }).ToList(),

                    UsarRegraPorMotivoAvaria = regrasAvaria.RegraPorMotivoAvaria,
                    MotivoAvaria = (from obj in regraMotivoAvaria select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Avarias.RegrasMotivoAvaria>(obj, "MotivoAvaria", "Descricao")).ToList(),

                    UsarRegraPorOrigem = regrasAvaria.RegraPorOrigem,
                    Origem = (from obj in regraOrigem select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Avarias.RegrasOrigem>(obj, "Origem", "Descricao")).ToList(),

                    UsarRegraPorDestino = regrasAvaria.RegraPorDestino,
                    Destino = (from obj in regraDestino select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Avarias.RegrasDestino>(obj, "Destino", "Descricao")).ToList(),

                    UsarRegraPorFilial = regrasAvaria.RegraPorFilial,
                    Filial = (from obj in regraFilial select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Avarias.RegrasFilial>(obj, "Filial", "Descricao")).ToList(),

                    UsarRegraPorTransportador = regrasAvaria.RegraPorTransportadora,
                    Transportador = (from obj in regraTransportadora select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Avarias.RegrasTransportadora>(obj, "Transportadora", "RazaoSocial")).ToList(),

                    UsarRegraPorTipoOperacao = regrasAvaria.RegraPorTipoOperacao,
                    TipoOperacao = (from obj in regraTipoOperacao select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Avarias.RegrasTipoOperacao>(obj, "TipoOperacao", "Descricao")).ToList(),

                    UsarRegraPorValorAvaria = regrasAvaria.RegraPorValor,
                    ValorAvaria = (from obj in regraValorAvaria select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Avarias.RegrasValorAvaria>(obj, "Valor", "Valor", true)).ToList()
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
                Repositorio.Embarcador.Avarias.RegrasAutorizacaoAvaria repRegrasAutorizacaoAvaria = new Repositorio.Embarcador.Avarias.RegrasAutorizacaoAvaria(unitOfWork);

                // Codigo da busca 
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca entidade
                Dominio.Entidades.Embarcador.Avarias.RegrasAutorizacaoAvaria regrasAvaria = repRegrasAutorizacaoAvaria.BuscarPorCodigo(codigo);

                if (regrasAvaria == null)
                    return new JsonpResult(false, "Não foi possível buscar a regra.");

                // Inicia transicao
                unitOfWork.Start();

                regrasAvaria.Aprovadores.Clear();
                regrasAvaria.RegrasMotivoAvaria.Clear();
                regrasAvaria.RegrasOrigem.Clear();
                regrasAvaria.RegrasDestino.Clear();
                regrasAvaria.RegrasFilial.Clear();
                regrasAvaria.RegrasTipoOperacao.Clear();
                regrasAvaria.RegrasTransportadora.Clear();
                regrasAvaria.RegrasValorAvaria.Clear();

                repRegrasAutorizacaoAvaria.Deletar(regrasAvaria);

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
        private void PreencherEntidade(ref Dominio.Entidades.Embarcador.Avarias.RegrasAutorizacaoAvaria regrasAvaria, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia Repositorios/Entidade
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);


            // Converte parametros
            string descricao = !string.IsNullOrWhiteSpace(Request.Params("Descricao")) ? Request.Params("Descricao") : string.Empty;
            string observacao = !string.IsNullOrWhiteSpace(Request.Params("Observacao")) ? Request.Params("Observacao") : string.Empty;

            DateTime dataVigenciaAux;
            DateTime? dataVigencia = null;

            if (DateTime.TryParseExact(Request.Params("Vigencia"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataVigenciaAux))
                dataVigencia = dataVigenciaAux;

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoAvaria etapaAutorizacaoAvaria;
            Enum.TryParse(Request.Params("EtapaAutorizacao"), out etapaAutorizacaoAvaria);

            int numeroAprovadores = 0;
            int.TryParse(Request.Params("NumeroAprovadores"), out numeroAprovadores);

            int prioridadeAprovacao = 0;
            int.TryParse(Request.Params("PrioridadeAprovacao"), out prioridadeAprovacao);


            bool usarRegraPorMotivoAvaria;
            bool.TryParse(Request.Params("UsarRegraPorMotivoAvaria"), out usarRegraPorMotivoAvaria);
            bool usarRegraPorOrigem;
            bool.TryParse(Request.Params("UsarRegraPorOrigem"), out usarRegraPorOrigem);
            bool usarRegraPorDestino;
            bool.TryParse(Request.Params("UsarRegraPorDestino"), out usarRegraPorDestino);
            bool usarRegraPorFilial;
            bool.TryParse(Request.Params("UsarRegraPorFilial"), out usarRegraPorFilial);
            bool usarRegraPorTransportador;
            bool.TryParse(Request.Params("UsarRegraPorTransportador"), out usarRegraPorTransportador);
            bool usarRegraPorTipoOperacao;
            bool.TryParse(Request.Params("UsarRegraPorTipoOperacao"), out usarRegraPorTipoOperacao);
            bool usarRegraPorValor;
            bool.TryParse(Request.Params("UsarRegraPorValorAvaria"), out usarRegraPorValor);

            List<int> codigosUsuarios = new List<int>();
            if (!string.IsNullOrWhiteSpace(Request.Params("Aprovadores")))
            {
                List<ObjetoAprovadores> dynAprovadores = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ObjetoAprovadores>>(Request.Params("Aprovadores"));

                for (var i = 0; i < dynAprovadores.Count(); i++)
                    codigosUsuarios.Add(dynAprovadores[i].Codigo);
            }
            List<Dominio.Entidades.Usuario> listaAprovadores = repUsuario.BuscarUsuariosPorCodigos(codigosUsuarios.ToArray(), null);

            // Seta na entidade
            regrasAvaria.Descricao = descricao;
            regrasAvaria.Observacoes = observacao;
            regrasAvaria.Vigencia = dataVigencia;
            regrasAvaria.NumeroAprovadores = numeroAprovadores;
            regrasAvaria.PrioridadeAprovacao = prioridadeAprovacao;
            regrasAvaria.Aprovadores = listaAprovadores;
            regrasAvaria.EtapaAutorizacaoAvaria = etapaAutorizacaoAvaria;

            regrasAvaria.RegraPorMotivoAvaria = usarRegraPorMotivoAvaria;
            regrasAvaria.RegraPorOrigem = usarRegraPorOrigem;
            regrasAvaria.RegraPorDestino = usarRegraPorDestino;
            regrasAvaria.RegraPorFilial = usarRegraPorFilial;
            regrasAvaria.RegraPorTransportadora = usarRegraPorTransportador;
            regrasAvaria.RegraPorTipoOperacao = usarRegraPorTipoOperacao;
            regrasAvaria.RegraPorValor = usarRegraPorValor;
        }

        private void PreencherEntidadeRegra<T>(string nomePropriedade, string parametroJson, bool usarDynamic, ref List<T> regrasPorTipo, ref Dominio.Entidades.Embarcador.Avarias.RegrasAutorizacaoAvaria regrasAvaria, Func<dynamic, object> lambda = null) where T : Dominio.Entidades.EntidadeBase
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
                int.TryParse(dynRegras[i].Codigo.ToString(), out int codigoRegra);
                int indexRegraNaLista = -1;

                // Instancia o objeto T (T não possui construor new)
                T regra = default(T);
                if (codigoRegra > 0)
                {
                    for (int j = 0; j < regrasPorTipo.Count; j++)
                        if ((int)((dynamic)regrasPorTipo[j]).Codigo == codigoRegra)
                        {
                            indexRegraNaLista = j;
                            break;
                        }
                }

                if (indexRegraNaLista >= 0)
                {
                    regra = regrasPorTipo[indexRegraNaLista];
                    regra.Initialize();
                }
                else
                    regra = Activator.CreateInstance<T>();

                // Seta as propriedas da entidade
                //prop = regra.GetType().GetProperty("Codigo", BindingFlags.Public | BindingFlags.Instance);
                //prop.SetValue(regra, codigoRegra, null);

                prop = regra.GetType().GetProperty("RegrasAutorizacaoAvaria", BindingFlags.Public | BindingFlags.Instance);
                prop.SetValue(regra, regrasAvaria, null);

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
                if (indexRegraNaLista >= 0)
                    regrasPorTipo[indexRegraNaLista] = regra;
                else
                    regrasPorTipo.Add(regra);
            }

        }

        private bool ValidarEntidade(Dominio.Entidades.Embarcador.Avarias.RegrasAutorizacaoAvaria regrasOcorrencia, out List<string> erros)
        {
            erros = new List<string>();

            if (string.IsNullOrWhiteSpace(regrasOcorrencia.Descricao))
                erros.Add("Descrição é obrigatória.");

            if (regrasOcorrencia.NumeroAprovadores <= 0)
                erros.Add("Número de Aprovadores é obrigatória.");

            if (regrasOcorrencia.Aprovadores.Count() < regrasOcorrencia.NumeroAprovadores)
                erros.Add("O número de aprovadores selecionados deve ser maior ou igual a " + regrasOcorrencia.NumeroAprovadores.ToString());

            return erros.Count() == 0;
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
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria juncao = prop.GetValue(obj);

            prop = obj.GetType().GetProperty("Condicao", BindingFlags.Public | BindingFlags.Instance);
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria condicao = prop.GetValue(obj);


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
                Condicao = condicao,
                Entidade = objetoEntidade,
                Valor = valor,
            };
            return restorno;
        }

        private void PreencherTodasRegras(ref Dominio.Entidades.Embarcador.Avarias.RegrasAutorizacaoAvaria regrasAvaria, ref List<Dominio.Entidades.Embarcador.Avarias.RegrasMotivoAvaria> regraMotivoAvaria, ref List<Dominio.Entidades.Embarcador.Avarias.RegrasOrigem> regraOrigem, ref List<Dominio.Entidades.Embarcador.Avarias.RegrasDestino> regraDestino, ref List<Dominio.Entidades.Embarcador.Avarias.RegrasFilial> regraFilial, ref List<Dominio.Entidades.Embarcador.Avarias.RegrasTransportadora> regraTransportadora, ref List<Dominio.Entidades.Embarcador.Avarias.RegrasTipoOperacao> regraTipoOperacao, ref List<Dominio.Entidades.Embarcador.Avarias.RegrasValorAvaria> regraValorAvaria, ref List<string> errosRegras, Repositorio.UnitOfWork unitOfWork)
        {
            // Erros de validacao
            List<string> erros = new List<string>();

            #region MotivoAvaria
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regrasAvaria.RegraPorMotivoAvaria)
            {
                // Preenche regra
                try
                {
                    PreencherEntidadeRegra("MotivoAvaria", "RegrasMotivoAvaria", false, ref regraMotivoAvaria, ref regrasAvaria, ((codigo) =>
                    {
                        Repositorio.Embarcador.Avarias.MotivoAvaria repMotivoAvaria = new Repositorio.Embarcador.Avarias.MotivoAvaria(unitOfWork);

                        int codigoInt = 0;
                        int.TryParse(codigo.ToString(), out codigoInt);

                        return repMotivoAvaria.BuscarPorCodigo(codigoInt);
                    }));
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                    errosRegras.Add("Motivo da Avaria");
                }

                // Valida regra (se for invalida, nao continua o fluxo)
                if (!ValidarEntidadeRegra("Motivo da Avaria", "MotivoAvaria", regraMotivoAvaria, out erros))
                    throw new Exception(String.Join("<br>", erros));
            }
            else
            {
                regraMotivoAvaria = new List<Dominio.Entidades.Embarcador.Avarias.RegrasMotivoAvaria>();
            }
            #endregion

            #region Origem
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regrasAvaria.RegraPorOrigem)
            {
                // Preenche regra
                try
                {
                    PreencherEntidadeRegra("Origem", "RegrasOrigem", false, ref regraOrigem, ref regrasAvaria, ((codigo) =>
                    {
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
                if (!ValidarEntidadeRegra("Origem", "Origem", regraOrigem, out erros))
                    throw new Exception(String.Join("<br>", erros));
            }
            else
            {
                regraOrigem = new List<Dominio.Entidades.Embarcador.Avarias.RegrasOrigem>();
            }
            #endregion

            #region Destino
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regrasAvaria.RegraPorDestino)
            {
                // Preenche regra
                try
                {
                    PreencherEntidadeRegra("Destino", "RegrasDestino", false, ref regraDestino, ref regrasAvaria, ((codigo) =>
                    {
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
                if (!ValidarEntidadeRegra("Destino", "Destino", regraDestino, out erros))
                    throw new Exception(String.Join("<br>", erros));
            }
            else
            {
                regraDestino = new List<Dominio.Entidades.Embarcador.Avarias.RegrasDestino>();
            }
            #endregion

            #region Filial
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regrasAvaria.RegraPorFilial)
            {
                // Preenche regra
                try
                {
                    PreencherEntidadeRegra("Filial", "RegrasFilial", false, ref regraFilial, ref regrasAvaria, ((codigo) =>
                    {
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
                if (!ValidarEntidadeRegra("Filial", "Filial", regraFilial, out erros))
                    throw new Exception(String.Join("<br>", erros));
            }
            else
            {
                regraFilial = new List<Dominio.Entidades.Embarcador.Avarias.RegrasFilial>();
            }
            #endregion

            #region Transportadora
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regrasAvaria.RegraPorTransportadora)
            {
                // Preenche regra
                try
                {
                    PreencherEntidadeRegra("Transportadora", "RegrasTransportador", false, ref regraTransportadora, ref regrasAvaria, ((codigo) =>
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
                    errosRegras.Add("Transportadora");
                }

                // Valida regra (se for invalida, nao continua o fluxo)
                if (!ValidarEntidadeRegra("Transportador", "Transportadora", regraTransportadora, out erros))
                    throw new Exception(String.Join("<br>", erros));
            }
            else
            {
                regraTransportadora = new List<Dominio.Entidades.Embarcador.Avarias.RegrasTransportadora>();
            }
            #endregion

            #region TipoOperacao
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regrasAvaria.RegraPorTipoOperacao)
            {
                // Preenche regra
                try
                {
                    PreencherEntidadeRegra("TipoOperacao", "RegrasTipoOperacao", false, ref regraTipoOperacao, ref regrasAvaria, ((codigo) =>
                    {
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
                if (!ValidarEntidadeRegra("Tipo de Operação", "TipoOperacao", regraTipoOperacao, out erros))
                    throw new Exception(String.Join("<br>", erros));
            }
            else
            {
                regraTipoOperacao = new List<Dominio.Entidades.Embarcador.Avarias.RegrasTipoOperacao>();
            }
            #endregion

            #region Valor
            // Preenche as regras apenas se a flag de uso for verdadeira
            if (regrasAvaria.RegraPorValor)
            {
                // Preenche regra
                try
                {
                    PreencherEntidadeRegra("Valor", "RegrasValorAvaria", true, ref regraValorAvaria, ref regrasAvaria);
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e);
                    errosRegras.Add("Valor da Avaria");
                }

                // Valida regra (se for invalida, nao continua o fluxo)
                if (!ValidarEntidadeRegra("Valor da Avaria", "Valor", regraValorAvaria, out erros))
                    throw new Exception(String.Join("<br>", erros));
            }
            else
            {
                regraValorAvaria = new List<Dominio.Entidades.Embarcador.Avarias.RegrasValorAvaria>();
            }
            #endregion
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Avarias.RegrasAutorizacaoAvaria repRegrasAutorizacaoOcorrencia = new Repositorio.Embarcador.Avarias.RegrasAutorizacaoAvaria(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            // Converte parametros
            int codigoAprovador = 0;
            int.TryParse(Request.Params("Aprovador"), out codigoAprovador);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoAvaria etapaAutorizacaoAvaria;
            Enum.TryParse(Request.Params("EtapaAutorizacao"), out etapaAutorizacaoAvaria);

            DateTime dataInicioAux, dataFimAux;
            DateTime? dataInicio = null, dataFim = null;

            if (DateTime.TryParseExact(Request.Params("DataInicio"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicioAux))
                dataInicio = dataInicioAux;

            if (DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFimAux))
                dataFim = dataFimAux;

            string descricao = !string.IsNullOrWhiteSpace(Request.Params("Descricao")) ? Request.Params("Descricao") : "";

            Dominio.Entidades.Usuario aprovador = repUsuario.BuscarPorCodigo(codigoAprovador);

            // Consulta
            List<Dominio.Entidades.Embarcador.Avarias.RegrasAutorizacaoAvaria> listaGrid = repRegrasAutorizacaoOcorrencia.ConsultarRegras(dataInicio, dataFim, aprovador, descricao, etapaAutorizacaoAvaria, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repRegrasAutorizacaoOcorrencia.ContarConsultaRegras(dataInicio, dataFim, aprovador, descricao, etapaAutorizacaoAvaria);


            var lista = (from obj in listaGrid
                         select new
                         {
                             obj.Codigo,
                             Descricao = !string.IsNullOrWhiteSpace(obj.Descricao) ? obj.Descricao : string.Empty,
                             DescricaoEtapaAutorizacaoAvaria = obj.DescricaoEtapaAutorizacaoAvaria,
                             Vigencia = obj.Vigencia.HasValue ? obj.Vigencia.Value.ToString("dd/MM/yyyy") : string.Empty,
                         }).ToList();

            return lista.ToList();
        }
        #endregion
    }
}

