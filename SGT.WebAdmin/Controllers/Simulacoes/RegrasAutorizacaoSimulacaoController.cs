using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace SGT.WebAdmin.Controllers.Simulacoes
{
    [CustomAuthorize("Simulacoes/RegrasSimulacao")]
    public class RegrasAutorizacaoSimulacaoController : BaseController
    {
		#region Construtores

		public RegrasAutorizacaoSimulacaoController(Conexao conexao) : base(conexao) { }

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
            public Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoSimulacao Condicao { get; set; }
            public Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoSimulacao Juncao { get; set; }
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
                grid.AdicionarCabecalho("Descrição", "Descricao", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Vigência", "Vigencia", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.center, false);

                // Converte parametros
                int codigoAprovador = 0;
                int.TryParse(Request.Params("Aprovador"), out codigoAprovador);

                DateTime dataInicioAux, dataFimAux;
                DateTime? dataInicio = null, dataFim = null;

                if (DateTime.TryParseExact(Request.Params("DataInicio"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicioAux))
                    dataInicio = dataInicioAux;

                if (DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFimAux))
                    dataFim = dataFimAux;



                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = !string.IsNullOrEmpty(Request.Params("Situacao")) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa)int.Parse(Request.Params("Situacao")) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;

                string descricao = !string.IsNullOrWhiteSpace(Request.Params("Descricao")) ? Request.Params("Descricao") : "";

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                // Instancia repositorios
                Repositorio.Embarcador.Simulacoes.RegrasAutorizacaoSimulacao repRegrasAutorizacaoSimulacao = new Repositorio.Embarcador.Simulacoes.RegrasAutorizacaoSimulacao(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                Dominio.Entidades.Usuario aprovador = repUsuario.BuscarPorCodigo(codigoAprovador);

                List<Dominio.Entidades.Embarcador.Simulacoes.RegrasAutorizacaoSimulacao> listaRegras = repRegrasAutorizacaoSimulacao.ConsultarRegras(dataInicio, dataFim, aprovador, descricao, ativo, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repRegrasAutorizacaoSimulacao.ContarConsultaRegras(dataInicio, dataFim, aprovador, descricao,ativo));

                var lista = (from obj in listaRegras
                             select new
                             {
                                 obj.Codigo,
                                 Descricao = !string.IsNullOrWhiteSpace(obj.Descricao) ? obj.Descricao : string.Empty,
                                 Vigencia = obj.Vigencia.HasValue ? obj.Vigencia.Value.ToString("dd/MM/yyyy") : string.Empty,
                                 Situacao = obj.Situacao ? "Ativo" : "Inativo"
                             }).ToList();

                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
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
                Repositorio.Embarcador.Simulacoes.RegrasAutorizacaoSimulacao repRegrasAutorizacaoSimulacao = new Repositorio.Embarcador.Simulacoes.RegrasAutorizacaoSimulacao(unitOfWork);
                Repositorio.Embarcador.Simulacoes.RegrasSimulacaoFilialEmissao repRegrasSimulacaoFilialEmissao = new Repositorio.Embarcador.Simulacoes.RegrasSimulacaoFilialEmissao(unitOfWork);
                Repositorio.Embarcador.Simulacoes.RegrasSimulacaoTipoOperacao repRegrasSimulacaoTipoOperacao = new Repositorio.Embarcador.Simulacoes.RegrasSimulacaoTipoOperacao(unitOfWork);
                Repositorio.Embarcador.Simulacoes.RegrasSimulacaoOrigem repRegrasSimulacaoOrigem = new Repositorio.Embarcador.Simulacoes.RegrasSimulacaoOrigem(unitOfWork);
                Repositorio.Embarcador.Simulacoes.RegrasSimulacaoDestino repRegrasSimulacaoDestino = new Repositorio.Embarcador.Simulacoes.RegrasSimulacaoDestino(unitOfWork);
                Repositorio.Embarcador.Simulacoes.RegrasSimulacaoTransportador repRegrasSimulacaoTransportador = new Repositorio.Embarcador.Simulacoes.RegrasSimulacaoTransportador(unitOfWork);


                // Nova entidade
                Dominio.Entidades.Embarcador.Simulacoes.RegrasAutorizacaoSimulacao regrasSimulacao = new Dominio.Entidades.Embarcador.Simulacoes.RegrasAutorizacaoSimulacao();
                List<Dominio.Entidades.Embarcador.Simulacoes.RegrasSimulacaoFilialEmissao> regraFilialEmissao = new List<Dominio.Entidades.Embarcador.Simulacoes.RegrasSimulacaoFilialEmissao>();
                List<Dominio.Entidades.Embarcador.Simulacoes.RegrasSimulacaoTipoOperacao> regraTipoOperacao = new List<Dominio.Entidades.Embarcador.Simulacoes.RegrasSimulacaoTipoOperacao>();
                List<Dominio.Entidades.Embarcador.Simulacoes.RegrasSimulacaoOrigem> regraOrigem = new List<Dominio.Entidades.Embarcador.Simulacoes.RegrasSimulacaoOrigem>();
                List<Dominio.Entidades.Embarcador.Simulacoes.RegrasSimulacaoDestino> regraDestino = new List<Dominio.Entidades.Embarcador.Simulacoes.RegrasSimulacaoDestino>();
                List<Dominio.Entidades.Embarcador.Simulacoes.RegrasSimulacaoTransportador> regraTransportador = new List<Dominio.Entidades.Embarcador.Simulacoes.RegrasSimulacaoTransportador>();

                // Preenche a entidade
                PreencherEntidade(ref regrasSimulacao, unitOfWork);

                List<string> erros = new List<string>();
                // Validar entidade
                if (!ValidarEntidade(regrasSimulacao, out erros))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, String.Join("<br>", erros));
                }

                #region Regras
                List<string> errosRegras = new List<string>();


                #region FilialEmissao
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasSimulacao.RegraPorFilialEmissao)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("Filial", "RegrasFilialEmissao", false, ref regraFilialEmissao, ref regrasSimulacao, ((codigo) => {
                            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);

                            int codigoInt = 0;
                            int.TryParse(codigo.ToString(), out codigoInt);

                            return repFilial.BuscarPorCodigo(codigoInt);
                        }));
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add("Filial de Emissão");
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra("Filial de Emissão", "Filial", regraFilialEmissao, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion

                #region TipoOperacao
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasSimulacao.RegraPorTipoOperacao)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("TipoOperacao", "RegrasTipoOperacao", false, ref regraTipoOperacao, ref regrasSimulacao, ((codigo) => {
                            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

                            int codigoInt = 0;
                            int.TryParse(codigo.ToString(), out codigoInt);

                            return repTipoOperacao.BuscarPorCodigo(codigoInt);
                        }));
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add("Tipo Operação");
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra("Tipo Operação", "TipoOperacao", regraTipoOperacao, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion

                #region Origem
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasSimulacao.RegraPorOrigem)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("Origem", "RegrasOrigem", false, ref regraOrigem, ref regrasSimulacao, ((codigo) => {
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

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra("Origem", "Origem", regraOrigem, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion

                #region Destino
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasSimulacao.RegraPorDestino)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("Destino", "RegrasDestino", false, ref regraDestino, ref regrasSimulacao, ((codigo) => {
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

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra("Destino", "Destino", regraDestino, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion

                #region Transportador
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasSimulacao.RegraPorTransportador)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("Transportador", "RegrasTransportador", false, ref regraTransportador, ref regrasSimulacao, ((codigo) => {
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


                #endregion

                // Insere Entidade
                repRegrasAutorizacaoSimulacao.Inserir(regrasSimulacao, Auditado);

                // Insere regras
                for (var i = 0; i < regraFilialEmissao.Count(); i++) repRegrasSimulacaoFilialEmissao.Inserir(regraFilialEmissao[i]);
                for (var i = 0; i < regraTipoOperacao.Count(); i++) repRegrasSimulacaoTipoOperacao.Inserir(regraTipoOperacao[i]);
                for (var i = 0; i < regraOrigem.Count(); i++) repRegrasSimulacaoOrigem.Inserir(regraOrigem[i]);
                for (var i = 0; i < regraDestino.Count(); i++) repRegrasSimulacaoDestino.Inserir(regraDestino[i]);
                for (var i = 0; i < regraTransportador.Count(); i++) repRegrasSimulacaoTransportador.Inserir(regraTransportador[i]);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAdicionar);
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
                Repositorio.Embarcador.Simulacoes.RegrasAutorizacaoSimulacao repRegrasAutorizacaoSimulacao = new Repositorio.Embarcador.Simulacoes.RegrasAutorizacaoSimulacao(unitOfWork);
                Repositorio.Embarcador.Simulacoes.RegrasSimulacaoFilialEmissao repRegrasSimulacaoFilialEmissao = new Repositorio.Embarcador.Simulacoes.RegrasSimulacaoFilialEmissao(unitOfWork);
                Repositorio.Embarcador.Simulacoes.RegrasSimulacaoTipoOperacao repRegrasSimulacaoTipoOperacao = new Repositorio.Embarcador.Simulacoes.RegrasSimulacaoTipoOperacao(unitOfWork);
                Repositorio.Embarcador.Simulacoes.RegrasSimulacaoOrigem repRegrasSimulacaoOrigem = new Repositorio.Embarcador.Simulacoes.RegrasSimulacaoOrigem(unitOfWork);
                Repositorio.Embarcador.Simulacoes.RegrasSimulacaoDestino repRegrasSimulacaoDestino = new Repositorio.Embarcador.Simulacoes.RegrasSimulacaoDestino(unitOfWork);
                Repositorio.Embarcador.Simulacoes.RegrasSimulacaoTransportador repRegrasSimulacaoTransportador = new Repositorio.Embarcador.Simulacoes.RegrasSimulacaoTransportador(unitOfWork);

                // Nova entidade
                // Codigo da busca 
                int codigoRegra = 0;
                int.TryParse(Request.Params("Codigo"), out codigoRegra);

                // Busca entidade
                Dominio.Entidades.Embarcador.Simulacoes.RegrasAutorizacaoSimulacao regrasSimulacao = repRegrasAutorizacaoSimulacao.BuscarPorCodigo(codigoRegra, true);

                if (regrasSimulacao == null)
                    return new JsonpResult(false, "Não foi possível buscar a regra.");

                #region BuscaRegras

                List<Dominio.Entidades.Embarcador.Simulacoes.RegrasSimulacaoFilialEmissao> regraFilialEmissao = repRegrasSimulacaoFilialEmissao.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.Simulacoes.RegrasSimulacaoTipoOperacao> regraTipoOperacao = repRegrasSimulacaoTipoOperacao.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.Simulacoes.RegrasSimulacaoOrigem> regraOrigem = repRegrasSimulacaoOrigem.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.Simulacoes.RegrasSimulacaoDestino> regraDestino = repRegrasSimulacaoDestino.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.Simulacoes.RegrasSimulacaoTransportador> regraTransportador = repRegrasSimulacaoTransportador.BuscarPorRegras(codigoRegra);

                #endregion

                #region DeletaRegras

                for (var i = 0; i < regraFilialEmissao.Count(); i++) repRegrasSimulacaoFilialEmissao.Deletar(regraFilialEmissao[i]);
                for (var i = 0; i < regraTipoOperacao.Count(); i++) repRegrasSimulacaoTipoOperacao.Deletar(regraTipoOperacao[i]);
                for (var i = 0; i < regraOrigem.Count(); i++) repRegrasSimulacaoOrigem.Deletar(regraOrigem[i]);
                for (var i = 0; i < regraDestino.Count(); i++) repRegrasSimulacaoDestino.Deletar(regraDestino[i]);
                for (var i = 0; i < regraTransportador.Count(); i++) repRegrasSimulacaoTransportador.Deletar(regraTransportador[i]);

                #endregion

                #region NovasRegras

                regraFilialEmissao = new List<Dominio.Entidades.Embarcador.Simulacoes.RegrasSimulacaoFilialEmissao>();
                regraTipoOperacao = new List<Dominio.Entidades.Embarcador.Simulacoes.RegrasSimulacaoTipoOperacao>();
                regraOrigem = new List<Dominio.Entidades.Embarcador.Simulacoes.RegrasSimulacaoOrigem>();
                regraDestino = new List<Dominio.Entidades.Embarcador.Simulacoes.RegrasSimulacaoDestino>();
                regraTransportador = new List<Dominio.Entidades.Embarcador.Simulacoes.RegrasSimulacaoTransportador>();

                #endregion

                // Preenche a entidade
                PreencherEntidade(ref regrasSimulacao, unitOfWork);

                List<string> erros = new List<string>();
                // Validar entidade
                if (!ValidarEntidade(regrasSimulacao, out erros))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, String.Join("<br>", erros));
                }

                // Atualiza Entidade
                repRegrasAutorizacaoSimulacao.Atualizar(regrasSimulacao, Auditado);

                #region Regras

                List<string> errosRegras = new List<string>();

                #region FilialEmissao
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasSimulacao.RegraPorFilialEmissao)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("Filial", "RegrasFilialEmissao", false, ref regraFilialEmissao, ref regrasSimulacao, ((codigo) => {
                            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);

                            int codigoInt = 0;
                            int.TryParse(codigo.ToString(), out codigoInt);

                            return repFilial.BuscarPorCodigo(codigoInt);
                        }));
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add("Filial de Emissão");
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra("Filial de Emissão", "Filial", regraFilialEmissao, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion

                #region TipoOperacao
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasSimulacao.RegraPorTipoOperacao)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("TipoOperacao", "RegrasTipoOperacao", false, ref regraTipoOperacao, ref regrasSimulacao, ((codigo) => {
                            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

                            int codigoInt = 0;
                            int.TryParse(codigo.ToString(), out codigoInt);

                            return repTipoOperacao.BuscarPorCodigo(codigoInt);
                        }));
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add("Tipo Operação");
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra("Tipo Operação", "TipoOperacao", regraTipoOperacao, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion

                #region Origem
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasSimulacao.RegraPorOrigem)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("Origem", "RegrasOrigem", false, ref regraOrigem, ref regrasSimulacao, ((codigo) => {
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

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra("Origem", "Origem", regraOrigem, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion

                #region Destino
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasSimulacao.RegraPorDestino)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("Destino", "RegrasDestino", false, ref regraDestino, ref regrasSimulacao, ((codigo) => {
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

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra("Destino", "Destino", regraDestino, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion

                #region Transportador
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasSimulacao.RegraPorTransportador)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("Transportador", "RegrasTransportador", false, ref regraTransportador, ref regrasSimulacao, ((codigo) => {
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

                #endregion

                // Insere regras
                for (var i = 0; i < regraFilialEmissao.Count(); i++) repRegrasSimulacaoFilialEmissao.Inserir(regraFilialEmissao[i]);
                for (var i = 0; i < regraTipoOperacao.Count(); i++) repRegrasSimulacaoTipoOperacao.Inserir(regraTipoOperacao[i]);
                for (var i = 0; i < regraOrigem.Count(); i++) repRegrasSimulacaoOrigem.Inserir(regraOrigem[i]);
                for (var i = 0; i < regraDestino.Count(); i++) repRegrasSimulacaoDestino.Inserir(regraDestino[i]);
                for (var i = 0; i < regraTransportador.Count(); i++) repRegrasSimulacaoTransportador.Inserir(regraTransportador[i]);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAtualizar);
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
                Repositorio.Embarcador.Simulacoes.RegrasAutorizacaoSimulacao repRegrasAutorizacaoSimulacao = new Repositorio.Embarcador.Simulacoes.RegrasAutorizacaoSimulacao(unitOfWork);
                Repositorio.Embarcador.Simulacoes.RegrasSimulacaoFilialEmissao repRegrasSimulacaoFilialEmissao = new Repositorio.Embarcador.Simulacoes.RegrasSimulacaoFilialEmissao(unitOfWork);
                Repositorio.Embarcador.Simulacoes.RegrasSimulacaoTipoOperacao repRegrasSimulacaoTipoOperacao = new Repositorio.Embarcador.Simulacoes.RegrasSimulacaoTipoOperacao(unitOfWork);
                Repositorio.Embarcador.Simulacoes.RegrasSimulacaoOrigem repRegrasSimulacaoOrigem = new Repositorio.Embarcador.Simulacoes.RegrasSimulacaoOrigem(unitOfWork);
                Repositorio.Embarcador.Simulacoes.RegrasSimulacaoDestino repRegrasSimulacaoDestino = new Repositorio.Embarcador.Simulacoes.RegrasSimulacaoDestino(unitOfWork);
                Repositorio.Embarcador.Simulacoes.RegrasSimulacaoTransportador repRegrasSimulacaoTransportador = new Repositorio.Embarcador.Simulacoes.RegrasSimulacaoTransportador(unitOfWork);

                // Codigo da busca 
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca entidade
                Dominio.Entidades.Embarcador.Simulacoes.RegrasAutorizacaoSimulacao regrasSimulacao = repRegrasAutorizacaoSimulacao.BuscarPorCodigo(codigo);

                if (regrasSimulacao == null)
                    return new JsonpResult(false, "Não foi possível buscar a regra.");

                #region BuscaRegras

                List<Dominio.Entidades.Embarcador.Simulacoes.RegrasSimulacaoFilialEmissao> regraFilialEmissao = repRegrasSimulacaoFilialEmissao.BuscarPorRegras(codigo);
                List<Dominio.Entidades.Embarcador.Simulacoes.RegrasSimulacaoTipoOperacao> regraTipoOperacao = repRegrasSimulacaoTipoOperacao.BuscarPorRegras(codigo);
                List<Dominio.Entidades.Embarcador.Simulacoes.RegrasSimulacaoOrigem> regraOrigem = repRegrasSimulacaoOrigem.BuscarPorRegras(codigo);
                List<Dominio.Entidades.Embarcador.Simulacoes.RegrasSimulacaoDestino> regraDestino = repRegrasSimulacaoDestino.BuscarPorRegras(codigo);
                List<Dominio.Entidades.Embarcador.Simulacoes.RegrasSimulacaoTransportador> regraTransportador = repRegrasSimulacaoTransportador.BuscarPorRegras(codigo);

                #endregion

                var dynRegra = new
                {
                    regrasSimulacao.Codigo,
                    regrasSimulacao.NumeroAprovadores,
                    regrasSimulacao.NumeroReprovadores,
                    regrasSimulacao.EnviarLinkParaAprovacaoPorEmail,
                    regrasSimulacao.Situacao,
                    Vigencia = regrasSimulacao.Vigencia.HasValue ? regrasSimulacao.Vigencia.Value.ToString("dd/MM/yyyy") : string.Empty,
                    Descricao = !string.IsNullOrWhiteSpace(regrasSimulacao.Descricao) ? regrasSimulacao.Descricao : string.Empty,
                    Observacao = !string.IsNullOrWhiteSpace(regrasSimulacao.Observacoes) ? regrasSimulacao.Observacoes : string.Empty,

                    Aprovadores = (from o in regrasSimulacao.Aprovadores select new { o.Codigo, o.Nome }).ToList(),

                    UsarRegraPorFilialEmissao = regrasSimulacao.RegraPorFilialEmissao,
                    FilialEmissao = (from obj in regraFilialEmissao select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Simulacoes.RegrasSimulacaoFilialEmissao>(obj, "Filial", "Descricao")).ToList(),

                    UsarRegraPorTipoOperacao = regrasSimulacao.RegraPorTipoOperacao,
                    TipoOperacao = (from obj in regraTipoOperacao select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Simulacoes.RegrasSimulacaoTipoOperacao>(obj, "TipoOperacao", "Descricao")).ToList(),

                    UsarRegraPorOrigem = regrasSimulacao.RegraPorOrigem,
                    Origem = (from obj in regraOrigem select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Simulacoes.RegrasSimulacaoOrigem>(obj, "Origem", "Descricao")).ToList(),

                    UsarRegraPorDestino = regrasSimulacao.RegraPorDestino,
                    Destino = (from obj in regraDestino select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Simulacoes.RegrasSimulacaoDestino>(obj, "Destino", "Descricao")).ToList(),

                    UsarRegraPorTransportador = regrasSimulacao.RegraPorTransportador,
                    Transportador = (from obj in regraTransportador select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Simulacoes.RegrasSimulacaoTransportador>(obj, "Transportador", "Descricao")).ToList(),

                };

                return new JsonpResult(dynRegra);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscar);
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
                Repositorio.Embarcador.Simulacoes.RegrasAutorizacaoSimulacao repRegrasAutorizacaoSimulacao = new Repositorio.Embarcador.Simulacoes.RegrasAutorizacaoSimulacao(unitOfWork);

                // Codigo da busca 
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca entidade
                Dominio.Entidades.Embarcador.Simulacoes.RegrasAutorizacaoSimulacao regrasSimulacao = repRegrasAutorizacaoSimulacao.BuscarPorCodigo(codigo);

                if (regrasSimulacao == null)
                    return new JsonpResult(false, "Não foi possível buscar a regra.");

                // Inicia transicao
                unitOfWork.Start();

                regrasSimulacao.Aprovadores.Clear();
                regrasSimulacao.RegrasFilialEmissao.Clear();
                regrasSimulacao.RegrasTipoOperacao.Clear();
                regrasSimulacao.RegrasOrigem.Clear();
                regrasSimulacao.RegrasDestino.Clear();
                regrasSimulacao.RegrasTransportador.Clear();

                repRegrasAutorizacaoSimulacao.Deletar(regrasSimulacao, Auditado);

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

        private void PreencherEntidade(ref Dominio.Entidades.Embarcador.Simulacoes.RegrasAutorizacaoSimulacao regrasSimulacao, Repositorio.UnitOfWork unitOfWork)
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
            
            bool situacao = true;
            bool.TryParse(Request.Params("Situacao"), out situacao);

            bool usarRegraPorFilialEmissao;
            bool.TryParse(Request.Params("UsarRegraPorFilialEmissao"), out usarRegraPorFilialEmissao);
            bool usarRegraPorTipoOperacao;
            bool.TryParse(Request.Params("UsarRegraPorTipoOperacao"), out usarRegraPorTipoOperacao);
            bool usarRegraPorOrigem;
            bool.TryParse(Request.Params("UsarRegraPorOrigem"), out usarRegraPorOrigem);
            bool usarRegraPorDestino;
            bool.TryParse(Request.Params("UsarRegraPorDestino"), out usarRegraPorDestino);
            bool usarRegraPorTransportador;
            bool.TryParse(Request.Params("UsarRegraPorTransportador"), out usarRegraPorTransportador);


            bool usarRegraPorExpedidor = Request.GetBoolParam("UsarRegraPorExpedidor");
            
            bool usarRegraPorTipoCarga = Request.GetBoolParam("UsarRegraPorTipoCarga");
            
            bool usarRegraPorCanalEntrega = Request.GetBoolParam("UsarRegraPorCanalEntrega");

            List<int> codigosUsuarios = new List<int>();
            if (!string.IsNullOrWhiteSpace(Request.Params("Aprovadores")))
            {
                List<ObjetoAprovadores> dynAprovadores = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ObjetoAprovadores>>(Request.Params("Aprovadores"));

                for (var i = 0; i < dynAprovadores.Count(); i++)
                    codigosUsuarios.Add(dynAprovadores[i].Codigo);
            }
            List<Dominio.Entidades.Usuario> listaAprovadores = repUsuario.BuscarUsuariosPorCodigos(codigosUsuarios.ToArray(), null);

            // Seta na entidade
            regrasSimulacao.Descricao = descricao;
            regrasSimulacao.Observacoes = observacao;
            regrasSimulacao.Vigencia = dataVigencia;
            regrasSimulacao.NumeroAprovadores = Request.GetIntParam("NumeroAprovadores");
            regrasSimulacao.NumeroReprovadores = Request.GetIntParam("NumeroReprovadores");
            regrasSimulacao.Situacao = situacao;
            regrasSimulacao.Aprovadores = listaAprovadores;

            regrasSimulacao.RegraPorFilialEmissao = usarRegraPorFilialEmissao;
            regrasSimulacao.RegraPorTipoOperacao = usarRegraPorTipoOperacao;
            regrasSimulacao.RegraPorOrigem = usarRegraPorOrigem;
            regrasSimulacao.RegraPorDestino = usarRegraPorDestino;
            regrasSimulacao.RegraPorTransportador = usarRegraPorTransportador;
            regrasSimulacao.EnviarLinkParaAprovacaoPorEmail = Request.GetBoolParam("EnviarLinkParaAprovacaoPorEmail");
        }

        private void PreencherEntidadeRegra<T>(string nomePropriedade, string parametroJson, bool usarDynamic, ref List<T> regrasProTipo, ref Dominio.Entidades.Embarcador.Simulacoes.RegrasAutorizacaoSimulacao regrasOcorrencia, Func<dynamic, object> lambda = null)
        {
            /* Descricao
             * RegrasAutorizacaoSimulacao é passado com ref, pois é vinculado a regra específica (RegraPorTipo) e após inserir no banco, a referencia permanece com o Codigo válido
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

                prop = regra.GetType().GetProperty("RegrasAutorizacaoSimulacao", BindingFlags.Public | BindingFlags.Instance);
                prop.SetValue(regra, regrasOcorrencia, null);

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

        private bool ValidarEntidade(Dominio.Entidades.Embarcador.Simulacoes.RegrasAutorizacaoSimulacao regrasOcorrencia, out List<string> erros)
        {
            erros = new List<string>();

            if (string.IsNullOrWhiteSpace(regrasOcorrencia.Descricao))
                erros.Add("Descrição é obrigatória.");

            if (regrasOcorrencia.Aprovadores.Count() < regrasOcorrencia.NumeroAprovadores)
                erros.Add($"O número de aprovadores selecionados deve ser maior ou igual ao número de aprovações ( {regrasOcorrencia.NumeroAprovadores} )");

            return erros.Count() == 0;
        }

        private bool ValidarEntidadeRegra<T>(string nomeRegra, string nomePropriedade, List<T> regrasProTipo, out List<string> erros)
        {
            erros = new List<string>();

            if(regrasProTipo.Count() == 0)
                erros.Add($"Nenhuma regra {nomeRegra} cadastrada.");
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
                        erros.Add(string.Format("{0} da regra é obrigatório.", nomeRegra));
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
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoSimulacao juncao = prop.GetValue(obj);

            prop = obj.GetType().GetProperty("Condicao", BindingFlags.Public | BindingFlags.Instance);
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoSimulacao condicao = prop.GetValue(obj);


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

        #endregion
    }
}
