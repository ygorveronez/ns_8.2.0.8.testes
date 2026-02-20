using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace SGT.WebAdmin.Controllers.Ocorrencias
{
    [CustomAuthorize("Ocorrencias/RegrasAutorizacaoOcorrencia")]
    public class RegrasAutorizacaoOcorrenciaController : BaseController
    {
		#region Construtores

		public RegrasAutorizacaoOcorrenciaController(Conexao conexao) : base(conexao) { }

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
            public Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia Condicao { get; set; }
            public Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia Juncao { get; set; }
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
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Descricao, "Descricao", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Etapa, "DescricaoEtapaAutorizacaoOcorrencia", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Vigencia, "Vigencia", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Situacao, "DescricaoAtivo", 10, Models.Grid.Align.center, false);

                // Converte parametros
                int codigoAprovador = 0;
                int.TryParse(Request.Params("Aprovador"), out codigoAprovador);
                int codigoOcorrencia = 0;
                int.TryParse(Request.Params("TipoOcorrencia"), out codigoOcorrencia);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia? etapaAutorizacaoOcorrencia = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia>("EtapaAutorizacao");

                DateTime dataInicioAux, dataFimAux;
                DateTime? dataInicio = null, dataFim = null;

                if (DateTime.TryParseExact(Request.Params("DataInicio"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicioAux))
                    dataInicio = dataInicioAux;

                if (DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFimAux))
                    dataFim = dataFimAux;



                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = !string.IsNullOrEmpty(Request.Params("Ativo")) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa)int.Parse(Request.Params("Ativo")) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;

                string descricao = !string.IsNullOrWhiteSpace(Request.Params("Descricao")) ? Request.Params("Descricao") : "";

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                // Instancia repositorios
                Repositorio.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia repRegrasAutorizacaoOcorrencia = new Repositorio.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                Dominio.Entidades.Usuario aprovador = repUsuario.BuscarPorCodigo(codigoAprovador);

                List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia> listaRegras = repRegrasAutorizacaoOcorrencia.ConsultarRegras(codigoOcorrencia, dataInicio, dataFim, aprovador, descricao, ativo, etapaAutorizacaoOcorrencia, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repRegrasAutorizacaoOcorrencia.ContarConsultaRegras(codigoOcorrencia, dataInicio, dataFim, aprovador, descricao,ativo, etapaAutorizacaoOcorrencia));

                var lista = (from obj in listaRegras
                             select new
                             {
                                 obj.Codigo,
                                 Descricao = !string.IsNullOrWhiteSpace(obj.Descricao) ? obj.Descricao : string.Empty,
                                 DescricaoEtapaAutorizacaoOcorrencia = obj.DescricaoEtapaAutorizacaoOcorrencia,
                                 Vigencia = obj.Vigencia.HasValue ? obj.Vigencia.Value.ToString("dd/MM/yyyy") : string.Empty,
                                 obj.DescricaoAtivo
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
                Repositorio.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia repRegrasAutorizacaoOcorrencia = new Repositorio.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaTipoOcorrencia repRegrasOcorrenciaTipoOcorrencia = new Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaTipoOcorrencia(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaComponenteFrete repRegrasOcorrenciaComponenteFrete = new Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaComponenteFrete(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaFilialEmissao repRegrasOcorrenciaFilialEmissao = new Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaFilialEmissao(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaTomadorOcorrencia repRegrasOcorrenciaTomadorOcorrencia = new Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaTomadorOcorrencia(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaValorOcorrencia repRegrasOcorrenciaValorOcorrencia = new Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaValorOcorrencia(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaTipoOperacao repRegrasOcorrenciaTipoOperacao = new Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaTipoOperacao(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaDiasAbertura repRegrasOcorrenciaDiasAbertura = new Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaDiasAbertura(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaExpedidor repRegrasOcorrenciaExpedidor = new Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaExpedidor(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaTipoCarga repRegrasOcorrenciaTipoCarga = new Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaTipoCarga(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaCanalEntrega repRegrasOcorrenciaCanalEntrega = new Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaCanalEntrega(unitOfWork);

                // Nova entidade
                Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia regrasOcorrencia = new Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia();
                List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaTipoOcorrencia> regraTipoOcorrencia = new List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaTipoOcorrencia>();
                List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaComponenteFrete> regraComponenteFrete = new List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaComponenteFrete>();
                List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaFilialEmissao> regraFilialEmissao = new List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaFilialEmissao>();
                List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaTomadorOcorrencia> regraTomadorOcorrencia = new List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaTomadorOcorrencia>();
                List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaValorOcorrencia> regraValorOcorrencia = new List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaValorOcorrencia>();
                List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaTipoOperacao> regraTipoOperacao = new List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaTipoOperacao>();
                List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaDiasAbertura> regraDiasAbertura = new List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaDiasAbertura>();
                List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaExpedidor> regraExpedidor = new List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaExpedidor>();
                List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaTipoCarga> regraTipoCarga = new List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaTipoCarga>();
                List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaCanalEntrega> regraCanalEntrega = new List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaCanalEntrega>();

                // Preenche a entidade
                PreencherEntidade(ref regrasOcorrencia, unitOfWork);

                List<string> erros = new List<string>();
                // Validar entidade
                if (!ValidarEntidade(regrasOcorrencia, out erros))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, String.Join("<br>", erros));
                }

                #region Regras
                List<string> errosRegras = new List<string>();

                #region TipoOcorrencia
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasOcorrencia.RegraPorTipoOcorrencia)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("TipoDeOcorrencia", "RegrasTipoOcorrencia", false, ref regraTipoOcorrencia, ref regrasOcorrencia, ((codigo) => {
                            Repositorio.TipoDeOcorrenciaDeCTe repTipoDeOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);

                            int codigoInt = 0;
                            int.TryParse(codigo.ToString(), out codigoInt);

                            return repTipoDeOcorrenciaDeCTe.BuscarPorCodigo(codigoInt);
                        }));
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.TipoOcorrencia);
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.TipoOcorrencia, "TipoDeOcorrencia", regraTipoOcorrencia, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion

                #region ComponenteFrete
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasOcorrencia.RegraPorComponenteFrete)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("ComponenteFrete", "RegrasComponenteFrete", false, ref regraComponenteFrete, ref regrasOcorrencia, ((codigo) => {
                            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);

                            int codigoInt = 0;
                            int.TryParse(codigo.ToString(), out codigoInt);

                            return repComponenteFrete.BuscarPorCodigo(codigoInt);
                        }));
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.ComponenteFrete);
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.ComponenteFrete, "ComponenteFrete", regraComponenteFrete, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion

                #region FilialEmissao
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasOcorrencia.RegraPorFilialEmissao)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("Filial", "RegrasFilialEmissao", false, ref regraFilialEmissao, ref regrasOcorrencia, ((codigo) => {
                            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);

                            int codigoInt = 0;
                            int.TryParse(codigo.ToString(), out codigoInt);

                            return repFilial.BuscarPorCodigo(codigoInt);
                        }));
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.FilialEmissao);
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.FilialEmissao, "Filial", regraFilialEmissao, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion

                #region TomadorOcorrencia
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasOcorrencia.RegraPorTomadorOcorrencia)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("Tomador", "RegrasTomadorOcorrencia", false, ref regraTomadorOcorrencia, ref regrasOcorrencia, ((codigo) => {
                            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                            double CPFCNPJ = 0;
                            double.TryParse(codigo.ToString(), out CPFCNPJ);

                            return repCliente.BuscarPorCPFCNPJ(CPFCNPJ);
                        }));
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.TomadorOcorrencia);
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.TomadorOcorrencia, "Tomador", regraTomadorOcorrencia, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion

                #region ValorOcorrencia
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasOcorrencia.RegraPorValorOcorrencia)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("Valor", "RegrasValorOcorrencia", true, ref regraValorOcorrencia, ref regrasOcorrencia);
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.ValorOcorrencia);
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.ValorOcorrencia, "Valor", regraValorOcorrencia, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion

                #region TipoOperacao
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasOcorrencia.RegraPorTipoOperacao)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("TipoOperacao", "RegrasTipoOperacao", false, ref regraTipoOperacao, ref regrasOcorrencia, ((codigo) => {
                            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

                            int codigoInt = 0;
                            int.TryParse(codigo.ToString(), out codigoInt);

                            return repTipoOperacao.BuscarPorCodigo(codigoInt);
                        }));
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.TipoOperacao);
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.TipoOperacao, "TipoOperacao", regraTipoOperacao, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion

                #region DiasAbertura
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasOcorrencia.RegraPorDiasAbertura)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("DiasAbertura", "RegrasDiasAbertura", true, ref regraDiasAbertura, ref regrasOcorrencia);
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.DiasAbertura);
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.DiasAbertura, "DiasAbertura", regraDiasAbertura, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion
                #region Expedidor
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasOcorrencia.RegraPorExpedidor)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("Expedidor", "RegrasExpedidor", false, ref regraExpedidor, ref regrasOcorrencia, ((codigo) => {
                            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                            double codigoDouble = 0;
                            double.TryParse(codigo.ToString(), out codigoDouble);

                            return repCliente.BuscarPorCPFCNPJ(codigoDouble);
                        }));
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Expedidor);
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Expedidor, "Expedidor", regraExpedidor, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion

                #region TipoCarga
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasOcorrencia.RegraPorTipoCarga)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("TipoDeCarga", "RegrasTipoCarga", false, ref regraTipoCarga, ref regrasOcorrencia, ((codigo) => {
                            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoCarga= new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);

                            int codigoInt = 0;
                            int.TryParse(codigo.ToString(), out codigoInt);

                            return repTipoCarga.BuscarPorCodigo(codigoInt);
                        }));
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.TipoCarga);
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.TipoCarga, "TipoDeCarga", regraTipoCarga, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion


                #region CanalEntrega
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasOcorrencia.RegraPorCanalEntrega)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("CanalEntrega", "RegrasCanalEntrega", false, ref regraCanalEntrega, ref regrasOcorrencia, ((codigo) => {
                            Repositorio.Embarcador.Pedidos.CanalEntrega repCanalEntrega = new Repositorio.Embarcador.Pedidos.CanalEntrega(unitOfWork);

                            int codigoInt = 0;
                            int.TryParse(codigo.ToString(), out codigoInt);

                            return repCanalEntrega.BuscarPorCodigo(codigoInt);
                        }));
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.CanalEntrega);
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.CanalEntrega, "CanalEntrega", regraCanalEntrega, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion

                #endregion

                // Insere Entidade
                repRegrasAutorizacaoOcorrencia.Inserir(regrasOcorrencia, Auditado);

                // Insere regras
                for (var i = 0; i < regraTipoOcorrencia.Count(); i++) repRegrasOcorrenciaTipoOcorrencia.Inserir(regraTipoOcorrencia[i]);
                for (var i = 0; i < regraComponenteFrete.Count(); i++) repRegrasOcorrenciaComponenteFrete.Inserir(regraComponenteFrete[i]);
                for (var i = 0; i < regraFilialEmissao.Count(); i++) repRegrasOcorrenciaFilialEmissao.Inserir(regraFilialEmissao[i]);
                for (var i = 0; i < regraTomadorOcorrencia.Count(); i++) repRegrasOcorrenciaTomadorOcorrencia.Inserir(regraTomadorOcorrencia[i]);
                for (var i = 0; i < regraValorOcorrencia.Count(); i++) repRegrasOcorrenciaValorOcorrencia.Inserir(regraValorOcorrencia[i]);
                for (var i = 0; i < regraTipoOperacao.Count(); i++) repRegrasOcorrenciaTipoOperacao.Inserir(regraTipoOperacao[i]);
                for (var i = 0; i < regraDiasAbertura.Count(); i++) repRegrasOcorrenciaDiasAbertura.Inserir(regraDiasAbertura[i]);
                for (var i = 0; i < regraExpedidor.Count(); i++) repRegrasOcorrenciaExpedidor.Inserir(regraExpedidor[i]);
                for (var i = 0; i < regraTipoCarga.Count(); i++) repRegrasOcorrenciaTipoCarga.Inserir(regraTipoCarga[i]);
                for (var i = 0; i < regraCanalEntrega.Count(); i++) repRegrasOcorrenciaCanalEntrega.Inserir(regraCanalEntrega[i]);

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
                Repositorio.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia repRegrasAutorizacaoOcorrencia = new Repositorio.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaTipoOcorrencia repRegrasOcorrenciaTipoOcorrencia = new Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaTipoOcorrencia(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaComponenteFrete repRegrasOcorrenciaComponenteFrete = new Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaComponenteFrete(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaFilialEmissao repRegrasOcorrenciaFilialEmissao = new Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaFilialEmissao(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaTomadorOcorrencia repRegrasOcorrenciaTomadorOcorrencia = new Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaTomadorOcorrencia(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaValorOcorrencia repRegrasOcorrenciaValorOcorrencia = new Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaValorOcorrencia(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaTipoOperacao repRegrasOcorrenciaTipoOperacao = new Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaTipoOperacao(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaDiasAbertura repRegrasOcorrenciaDiasAbertura = new Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaDiasAbertura(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaExpedidor repRegrasOcorrenciaExpedidor = new Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaExpedidor(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaTipoCarga repRegrasOcorrenciaTipoCarga = new Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaTipoCarga(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaCanalEntrega repRegrasOcorrenciaCanalEntrega = new Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaCanalEntrega(unitOfWork);

                // Nova entidade
                // Codigo da busca 
                int codigoRegra = 0;
                int.TryParse(Request.Params("Codigo"), out codigoRegra);

                // Busca entidade
                Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia regrasOcorrencia = repRegrasAutorizacaoOcorrencia.BuscarPorCodigo(codigoRegra, true);

                if (regrasOcorrencia == null)
                    return new JsonpResult(false, Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.NaoFoiPossivelBuscarRegra);

                #region BuscaRegras

                List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaTipoOcorrencia> regraTipoOcorrencia = repRegrasOcorrenciaTipoOcorrencia.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaComponenteFrete> regraComponenteFrete = repRegrasOcorrenciaComponenteFrete.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaFilialEmissao> regraFilialEmissao = repRegrasOcorrenciaFilialEmissao.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaTomadorOcorrencia> regraTomadorOcorrencia = repRegrasOcorrenciaTomadorOcorrencia.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaValorOcorrencia> regraValorOcorrencia = repRegrasOcorrenciaValorOcorrencia.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaTipoOperacao> regraTipoOperacao = repRegrasOcorrenciaTipoOperacao.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaDiasAbertura> regraDiasAbertura = repRegrasOcorrenciaDiasAbertura.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaExpedidor> regraExpedidor = repRegrasOcorrenciaExpedidor.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaTipoCarga> regraTipoCarga = new List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaTipoCarga>();
                List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaCanalEntrega> regraCanalEntrega = new List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaCanalEntrega>();

                #endregion

                #region DeletaRegras

                for (var i = 0; i < regraTipoOcorrencia.Count(); i++) repRegrasOcorrenciaTipoOcorrencia.Deletar(regraTipoOcorrencia[i]);
                for (var i = 0; i < regraComponenteFrete.Count(); i++) repRegrasOcorrenciaComponenteFrete.Deletar(regraComponenteFrete[i]);
                for (var i = 0; i < regraFilialEmissao.Count(); i++) repRegrasOcorrenciaFilialEmissao.Deletar(regraFilialEmissao[i]);
                for (var i = 0; i < regraTomadorOcorrencia.Count(); i++) repRegrasOcorrenciaTomadorOcorrencia.Deletar(regraTomadorOcorrencia[i]);
                for (var i = 0; i < regraValorOcorrencia.Count(); i++) repRegrasOcorrenciaValorOcorrencia.Deletar(regraValorOcorrencia[i]);
                for (var i = 0; i < regraTipoOperacao.Count(); i++) repRegrasOcorrenciaTipoOperacao.Deletar(regraTipoOperacao[i]);
                for (var i = 0; i < regraDiasAbertura.Count(); i++) repRegrasOcorrenciaDiasAbertura.Deletar(regraDiasAbertura[i]);
                for (var i = 0; i < regraExpedidor.Count(); i++) repRegrasOcorrenciaExpedidor.Deletar(regraExpedidor[i]);
                for (var i = 0; i < regraTipoCarga.Count(); i++) repRegrasOcorrenciaTipoCarga.Deletar(regraTipoCarga[i]);
                for (var i = 0; i < regraCanalEntrega.Count(); i++) repRegrasOcorrenciaCanalEntrega.Deletar(regraCanalEntrega[i]);

                #endregion

                #region NovasRegras

                regraTipoOcorrencia = new List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaTipoOcorrencia>();
                regraComponenteFrete = new List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaComponenteFrete>();
                regraFilialEmissao = new List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaFilialEmissao>();
                regraTomadorOcorrencia = new List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaTomadorOcorrencia>();
                regraValorOcorrencia = new List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaValorOcorrencia>();
                regraTipoOperacao = new List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaTipoOperacao>();
                regraDiasAbertura = new List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaDiasAbertura>();
                regraExpedidor = new List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaExpedidor>();
                regraTipoCarga = new List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaTipoCarga>();
                regraCanalEntrega = new List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaCanalEntrega>();

                #endregion

                // Preenche a entidade
                PreencherEntidade(ref regrasOcorrencia, unitOfWork);

                List<string> erros = new List<string>();
                // Validar entidade
                if (!ValidarEntidade(regrasOcorrencia, out erros))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, String.Join("<br>", erros));
                }

                // Atualiza Entidade
                repRegrasAutorizacaoOcorrencia.Atualizar(regrasOcorrencia, Auditado);

                #region Regras

                List<string> errosRegras = new List<string>();

                #region TipoOcorrencia
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasOcorrencia.RegraPorTipoOcorrencia)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("TipoDeOcorrencia", "RegrasTipoOcorrencia", false, ref regraTipoOcorrencia, ref regrasOcorrencia, ((codigo) => {
                            Repositorio.TipoDeOcorrenciaDeCTe repTipoDeOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);

                            int codigoInt = 0;
                            int.TryParse(codigo.ToString(), out codigoInt);

                            return repTipoDeOcorrenciaDeCTe.BuscarPorCodigo(codigoInt);
                        }));
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.TipoOcorrencia);
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.TipoOcorrencia, "TipoDeOcorrencia", regraTipoOcorrencia, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion

                #region ComponenteFrete
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasOcorrencia.RegraPorComponenteFrete)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("ComponenteFrete", "RegrasComponenteFrete", false, ref regraComponenteFrete, ref regrasOcorrencia, ((codigo) => {
                            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);

                            int codigoInt = 0;
                            int.TryParse(codigo.ToString(), out codigoInt);

                            return repComponenteFrete.BuscarPorCodigo(codigoInt);
                        }));
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.ComponenteFrete);
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.ComponenteFrete, "ComponenteFrete", regraComponenteFrete, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion

                #region FilialEmissao
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasOcorrencia.RegraPorFilialEmissao)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("Filial", "RegrasFilialEmissao", false, ref regraFilialEmissao, ref regrasOcorrencia, ((codigo) => {
                            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);

                            int codigoInt = 0;
                            int.TryParse(codigo.ToString(), out codigoInt);

                            return repFilial.BuscarPorCodigo(codigoInt);
                        }));
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.FilialEmissao);
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.FilialEmissao, "Filial", regraFilialEmissao, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion

                #region TomadorOcorrencia
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasOcorrencia.RegraPorTomadorOcorrencia)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("Tomador", "RegrasTomadorOcorrencia", false, ref regraTomadorOcorrencia, ref regrasOcorrencia, ((codigo) => {
                            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                            double CPFCNPJ = 0;
                            double.TryParse(codigo.ToString(), out CPFCNPJ);

                            return repCliente.BuscarPorCPFCNPJ(CPFCNPJ);
                        }));
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.TomadorOcorrencia);
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.TomadorOcorrencia, "Tomador", regraTomadorOcorrencia, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion

                #region ValorOcorrencia
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasOcorrencia.RegraPorValorOcorrencia)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("Valor", "RegrasValorOcorrencia", true, ref regraValorOcorrencia, ref regrasOcorrencia);
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.ValorOcorrencia);
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.ValorOcorrencia, "Valor", regraValorOcorrencia, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion

                #region TipoOperacao
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasOcorrencia.RegraPorTipoOperacao)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("TipoOperacao", "RegrasTipoOperacao", false, ref regraTipoOperacao, ref regrasOcorrencia, ((codigo) => {
                            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

                            int codigoInt = 0;
                            int.TryParse(codigo.ToString(), out codigoInt);

                            return repTipoOperacao.BuscarPorCodigo(codigoInt);
                        }));
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.TipoOperacao);
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.TipoOperacao, "TipoOperacao", regraTipoOperacao, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion

                #region DiasAbertura
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasOcorrencia.RegraPorDiasAbertura)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("DiasAbertura", "RegrasDiasAbertura", true, ref regraDiasAbertura, ref regrasOcorrencia);
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.DiasAbertura);
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.DiasAbertura, "DiasAbertura", regraDiasAbertura, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion
                #region Expedidor
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasOcorrencia.RegraPorExpedidor)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("Expedidor", "RegrasExpedidor", false, ref regraExpedidor, ref regrasOcorrencia, ((codigo) => {
                            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                            double codigoDouble = 0;
                            double.TryParse(codigo.ToString(), out codigoDouble);

                            return repCliente.BuscarPorCPFCNPJ(codigoDouble);
                        }));
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Expedidor);
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.Expedidor, "Expedidor", regraExpedidor, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion

                #region CanalEntrega
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasOcorrencia.RegraPorCanalEntrega)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("CanalEntrega", "RegrasCanalEntrega", false, ref regraCanalEntrega, ref regrasOcorrencia, ((codigo) => {
                            Repositorio.Embarcador.Pedidos.CanalEntrega repositorioCanalEntrega = new Repositorio.Embarcador.Pedidos.CanalEntrega(unitOfWork);

                            int codigoInt = 0;
                            int.TryParse(codigo.ToString(), out codigoInt);

                            return repositorioCanalEntrega.BuscarPorCodigo(codigoInt);
                        }));
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.CanalEntrega);
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.CanalEntrega, "CanalEntrega", regraCanalEntrega, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion

                #region TipoCarga
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasOcorrencia.RegraPorTipoCarga)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("TipoDeCarga", "RegrasTipoCarga", false, ref regraTipoCarga, ref regrasOcorrencia, ((codigo) => {
                            Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);

                            int codigoInt = 0;
                            int.TryParse(codigo.ToString(), out codigoInt);

                            return repositorioTipoCarga.BuscarPorCodigo(codigoInt);
                        }));
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.TipoCarga);
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.TipoCarga, "TipoDeCarga", regraTipoCarga, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion

                #endregion

                // Insere regras
                for (var i = 0; i < regraTipoOcorrencia.Count(); i++) repRegrasOcorrenciaTipoOcorrencia.Inserir(regraTipoOcorrencia[i]);
                for (var i = 0; i < regraComponenteFrete.Count(); i++) repRegrasOcorrenciaComponenteFrete.Inserir(regraComponenteFrete[i]);
                for (var i = 0; i < regraFilialEmissao.Count(); i++) repRegrasOcorrenciaFilialEmissao.Inserir(regraFilialEmissao[i]);
                for (var i = 0; i < regraTomadorOcorrencia.Count(); i++) repRegrasOcorrenciaTomadorOcorrencia.Inserir(regraTomadorOcorrencia[i]);
                for (var i = 0; i < regraValorOcorrencia.Count(); i++) repRegrasOcorrenciaValorOcorrencia.Inserir(regraValorOcorrencia[i]);
                for (var i = 0; i < regraTipoOperacao.Count(); i++) repRegrasOcorrenciaTipoOperacao.Inserir(regraTipoOperacao[i]);
                for (var i = 0; i < regraDiasAbertura.Count(); i++) repRegrasOcorrenciaDiasAbertura.Inserir(regraDiasAbertura[i]);
                for (var i = 0; i < regraExpedidor.Count(); i++) repRegrasOcorrenciaExpedidor.Inserir(regraExpedidor[i]);
                for (var i = 0; i < regraTipoCarga.Count(); i++) repRegrasOcorrenciaTipoCarga.Inserir(regraTipoCarga[i]);
                for (var i = 0; i < regraCanalEntrega.Count(); i++) repRegrasOcorrenciaCanalEntrega.Inserir(regraCanalEntrega[i]);

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
                Repositorio.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia repRegrasAutorizacaoOcorrencia = new Repositorio.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaTipoOcorrencia repRegrasOcorrenciaTipoOcorrencia = new Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaTipoOcorrencia(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaComponenteFrete repRegrasOcorrenciaComponenteFrete = new Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaComponenteFrete(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaFilialEmissao repRegrasOcorrenciaFilialEmissao = new Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaFilialEmissao(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaTomadorOcorrencia repRegrasOcorrenciaTomadorOcorrencia = new Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaTomadorOcorrencia(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaValorOcorrencia repRegrasOcorrenciaValorOcorrencia = new Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaValorOcorrencia(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaTipoOperacao repRegrasOcorrenciaTipoOperacao = new Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaTipoOperacao(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaDiasAbertura repRegrasOcorrenciaDiasAbertura = new Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaDiasAbertura(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaExpedidor repRegrasOcorrenciaExpedidor = new Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaExpedidor(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaTipoCarga repRegrasOcorrenciaTipoCarga = new Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaTipoCarga(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaCanalEntrega repRegrasOcorrenciaCanalEntrega = new Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaCanalEntrega(unitOfWork);

                // Codigo da busca 
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca entidade
                Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia regrasOcorrencia = repRegrasAutorizacaoOcorrencia.BuscarPorCodigo(codigo);

                if (regrasOcorrencia == null)
                    return new JsonpResult(false, Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.NaoFoiPossivelBuscarRegra);

                #region BuscaRegras

                List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaTipoOcorrencia> regraTipoOcorrencia = repRegrasOcorrenciaTipoOcorrencia.BuscarPorRegras(codigo);
                List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaComponenteFrete> regraComponenteFrete = repRegrasOcorrenciaComponenteFrete.BuscarPorRegras(codigo);
                List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaFilialEmissao> regraFilialEmissao = repRegrasOcorrenciaFilialEmissao.BuscarPorRegras(codigo);
                List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaTomadorOcorrencia> regraTomadorOcorrencia = repRegrasOcorrenciaTomadorOcorrencia.BuscarPorRegras(codigo);
                List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaValorOcorrencia> regraValorOcorrencia = repRegrasOcorrenciaValorOcorrencia.BuscarPorRegras(codigo);
                List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaTipoOperacao> regraTipoOperacao = repRegrasOcorrenciaTipoOperacao.BuscarPorRegras(codigo);
                List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaDiasAbertura> regraDiasAbertura = repRegrasOcorrenciaDiasAbertura.BuscarPorRegras(codigo);
                List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaExpedidor> regraExpedidor = repRegrasOcorrenciaExpedidor.BuscarPorRegras(codigo);
                List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaTipoCarga> regraTipoCarga = repRegrasOcorrenciaTipoCarga.BuscarPorRegras(codigo);
                List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaCanalEntrega> regraCanalEntrega = repRegrasOcorrenciaCanalEntrega.BuscarPorRegras(codigo);

                #endregion

                var dynRegra = new
                {
                    regrasOcorrencia.Codigo,
                    regrasOcorrencia.NumeroAprovadores,
                    regrasOcorrencia.NumeroReprovadores,
                    regrasOcorrencia.DiasPrazoAprovacao,
                    regrasOcorrencia.PrioridadeAprovacao,
                    regrasOcorrencia.AprovacaoAutomaticaAposDias,
                    regrasOcorrencia.TipoDiasAprovacao,
                    regrasOcorrencia.EnviarLinkParaAprovacaoPorEmail,
                    regrasOcorrencia.Ativo,
                    Vigencia = regrasOcorrencia.Vigencia.HasValue ? regrasOcorrencia.Vigencia.Value.ToString("dd/MM/yyyy") : string.Empty,
                    Descricao = !string.IsNullOrWhiteSpace(regrasOcorrencia.Descricao) ? regrasOcorrencia.Descricao : string.Empty,
                    Observacao = !string.IsNullOrWhiteSpace(regrasOcorrencia.Observacoes) ? regrasOcorrencia.Observacoes : string.Empty,
                    EtapaAutorizacao = regrasOcorrencia.EtapaAutorizacaoOcorrencia,

                    Aprovadores = (from o in regrasOcorrencia.Aprovadores select new { o.Codigo, o.Nome }).ToList(),
                    UsarRegraPorExpedidor = regrasOcorrencia.RegraPorExpedidor,
                    Expedidor = (from obj in regraExpedidor select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaExpedidor>(obj, "Expedidor", "Descricao")).ToList(),
                    
                    UsarRegraPorTipoCarga = regrasOcorrencia.RegraPorTipoCarga,
                    TipoCarga = (from obj in regraTipoCarga select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaTipoCarga>(obj, "TipoDeCarga", "Descricao")).ToList(),
                    
                    UsarRegraPorCanalEntrega = regrasOcorrencia.RegraPorCanalEntrega,
                    CanalEntrega = (from obj in regraCanalEntrega select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaCanalEntrega>(obj, "CanalEntrega", "Descricao")).ToList(),

                    UsarRegraPorTipoOcorrencia = regrasOcorrencia.RegraPorTipoOcorrencia,
                    TipoOcorrencia = (from obj in regraTipoOcorrencia select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaTipoOcorrencia>(obj, "TipoDeOcorrencia", "Descricao")).ToList(),

                    UsarRegraPorComponenteFrete = regrasOcorrencia.RegraPorComponenteFrete,
                    ComponenteFrete = (from obj in regraComponenteFrete select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaComponenteFrete>(obj, "ComponenteFrete", "Descricao")).ToList(),

                    UsarRegraPorFilialEmissao = regrasOcorrencia.RegraPorFilialEmissao,
                    FilialEmissao = (from obj in regraFilialEmissao select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaFilialEmissao>(obj, "Filial", "Descricao")).ToList(),

                    UsarRegraPorTomadorOcorrencia = regrasOcorrencia.RegraPorTomadorOcorrencia,
                    TomadorOcorrencia = (from obj in regraTomadorOcorrencia select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaTomadorOcorrencia>(obj, "Tomador", "Nome")).ToList(),

                    UsarRegraPorValorOcorrencia = regrasOcorrencia.RegraPorValorOcorrencia,
                    ValorOcorrencia = (from obj in regraValorOcorrencia select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaValorOcorrencia>(obj, "ValorOcorrencia", "Valor", true)).ToList(),

                    UsarRegraPorTipoOperacao = regrasOcorrencia.RegraPorTipoOperacao,
                    TipoOperacao = (from obj in regraTipoOperacao select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaTipoOperacao>(obj, "TipoOperacao", "Descricao")).ToList(),

                    UsarRegraPorDiasAbertura = regrasOcorrencia.RegraPorDiasAbertura,
                    DiasAbertura = (from obj in regraDiasAbertura select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaDiasAbertura>(obj, "DiasAbertura", "DiasAbertura", true)).ToList(),

                   
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
                Repositorio.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia repRegrasAutorizacaoOcorrencia = new Repositorio.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia(unitOfWork);

                // Codigo da busca 
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca entidade
                Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia regrasOcorrencia = repRegrasAutorizacaoOcorrencia.BuscarPorCodigo(codigo);

                if (regrasOcorrencia == null)
                    return new JsonpResult(false, Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.NaoFoiPossivelBuscarRegra);

                // Inicia transicao
                unitOfWork.Start();

                regrasOcorrencia.Aprovadores.Clear();
                regrasOcorrencia.RegrasTipoOcorrencia.Clear();
                regrasOcorrencia.RegrasComponenteFrete.Clear();
                regrasOcorrencia.RegrasFilialEmissao.Clear();
                regrasOcorrencia.RegrasTomadorOcorrencia.Clear();
                regrasOcorrencia.RegrasValorOcorrencia.Clear();
                regrasOcorrencia.RegrasTipoOperacao.Clear();
                regrasOcorrencia.RegrasDiasAbertura.Clear();

                repRegrasAutorizacaoOcorrencia.Deletar(regrasOcorrencia, Auditado);

                // Comita alteracoes
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.JaExistemSolicitacoesVinculadasRegra);
                else
                    return new JsonpResult(false, Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.OcorreUmaFalhaAoExcluir);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherEntidade(ref Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia regrasOcorrencia, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia Repositorios/Entidade
            Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaTipoOcorrencia repRegrasOcorrenciaTipoOcorrencia = new Repositorio.Embarcador.Ocorrencias.RegrasOcorrenciaTipoOcorrencia(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);


            // Converte parametros
            string descricao = !string.IsNullOrWhiteSpace(Request.Params("Descricao")) ? Request.Params("Descricao") : string.Empty;
            string observacao = !string.IsNullOrWhiteSpace(Request.Params("Observacao")) ? Request.Params("Observacao") : string.Empty;

            DateTime dataVigenciaAux;
            DateTime? dataVigencia = null;

            if (DateTime.TryParseExact(Request.Params("Vigencia"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataVigenciaAux))
                dataVigencia = dataVigenciaAux;

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia etapaAutorizacaoOcorrencia;
            Enum.TryParse(Request.Params("EtapaAutorizacao"), out etapaAutorizacaoOcorrencia);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDiasAprovacao tipoDiasAprovacao;
            Enum.TryParse(Request.Params("TipoDiasAprovacao"), out tipoDiasAprovacao);

            int prioridadeAprovacao = 0;
            int.TryParse(Request.Params("PrioridadeAprovacao"), out prioridadeAprovacao);
            
            bool ativo = true;
            bool.TryParse(Request.Params("Ativo"), out ativo);

            bool usarRegraPorTipoOcorrencia;
            bool.TryParse(Request.Params("UsarRegraPorTipoOcorrencia"), out usarRegraPorTipoOcorrencia);
            bool usarRegraPorComponenteFrete;
            bool.TryParse(Request.Params("UsarRegraPorComponenteFrete"), out usarRegraPorComponenteFrete);
            bool usarRegraPorFilialEmissao;
            bool.TryParse(Request.Params("UsarRegraPorFilialEmissao"), out usarRegraPorFilialEmissao);
            bool usarRegraPorTomadorOcorrencia;
            bool.TryParse(Request.Params("UsarRegraPorTomadorOcorrencia"), out usarRegraPorTomadorOcorrencia);
            bool usarRegraPorValorOcorrencia;
            bool.TryParse(Request.Params("UsarRegraPorValorOcorrencia"), out usarRegraPorValorOcorrencia);
            bool usarRegraPorTipoOperacao;
            bool.TryParse(Request.Params("UsarRegraPorTipoOperacao"), out usarRegraPorTipoOperacao);
            bool usarRegraPorDiasAbertura;
            bool.TryParse(Request.Params("UsarRegraPorDiasAbertura"), out usarRegraPorDiasAbertura);

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
            regrasOcorrencia.Descricao = descricao;
            regrasOcorrencia.Observacoes = observacao;
            regrasOcorrencia.Vigencia = dataVigencia;
            regrasOcorrencia.NumeroAprovadores = Request.GetIntParam("NumeroAprovadores");
            regrasOcorrencia.NumeroReprovadores = Request.GetIntParam("NumeroReprovadores");
            regrasOcorrencia.PrioridadeAprovacao = prioridadeAprovacao;
            regrasOcorrencia.DiasPrazoAprovacao = Request.GetNullableIntParam("DiasPrazoAprovacao");
            regrasOcorrencia.AprovacaoAutomaticaAposDias = Request.GetIntParam("AprovacaoAutomaticaAposDias");
            regrasOcorrencia.Ativo = ativo;
            regrasOcorrencia.Aprovadores = listaAprovadores;
            regrasOcorrencia.EtapaAutorizacaoOcorrencia = etapaAutorizacaoOcorrencia;
            regrasOcorrencia.TipoDiasAprovacao = tipoDiasAprovacao;

            regrasOcorrencia.RegraPorTipoOcorrencia = usarRegraPorTipoOcorrencia;
            regrasOcorrencia.RegraPorComponenteFrete = usarRegraPorComponenteFrete;
            regrasOcorrencia.RegraPorFilialEmissao = usarRegraPorFilialEmissao;
            regrasOcorrencia.RegraPorTomadorOcorrencia = usarRegraPorTomadorOcorrencia;
            regrasOcorrencia.RegraPorValorOcorrencia = usarRegraPorValorOcorrencia;
            regrasOcorrencia.RegraPorTipoOperacao = usarRegraPorTipoOperacao;
            regrasOcorrencia.RegraPorDiasAbertura = usarRegraPorDiasAbertura;
            regrasOcorrencia.RegraPorExpedidor = usarRegraPorExpedidor;
            regrasOcorrencia.RegraPorExpedidor = usarRegraPorExpedidor;
            regrasOcorrencia.RegraPorCanalEntrega = usarRegraPorCanalEntrega;
            regrasOcorrencia.RegraPorTipoCarga = usarRegraPorTipoCarga;
            regrasOcorrencia.EnviarLinkParaAprovacaoPorEmail = Request.GetBoolParam("EnviarLinkParaAprovacaoPorEmail");
        }

        private void PreencherEntidadeRegra<T>(string nomePropriedade, string parametroJson, bool usarDynamic, ref List<T> regrasProTipo, ref Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia regrasOcorrencia, Func<dynamic, object> lambda = null)
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
                throw new Exception(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.ErroAoConverterDadosRecebidor);

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

                prop = regra.GetType().GetProperty("RegrasAutorizacaoOcorrencia", BindingFlags.Public | BindingFlags.Instance);
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

        private bool ValidarEntidade(Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia regrasOcorrencia, out List<string> erros)
        {
            erros = new List<string>();

            if (string.IsNullOrWhiteSpace(regrasOcorrencia.Descricao))
                erros.Add(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.DescricaoEObrigatoria);

            if (regrasOcorrencia.Aprovadores.Count() < regrasOcorrencia.NumeroAprovadores)
                erros.Add(string.Format(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.NumeroAprovadoresSelecionadosDevesSerMaiorIgualCondicao, regrasOcorrencia.NumeroAprovadores));

            return erros.Count() == 0;
        }

        private bool ValidarEntidadeRegra<T>(string nomeRegra, string nomePropriedade, List<T> regrasProTipo, out List<string> erros)
        {
            erros = new List<string>();

            if(regrasProTipo.Count() == 0)
                erros.Add(string.Format(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.NenhumaRegraCadastrada, nomeRegra));
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
                        erros.Add(string.Format(Localization.Resources.Ocorrencias.RegrasAutorizacaoOcorrencia.RegraObrigatorio, nomeRegra));
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
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia juncao = prop.GetValue(obj);

            prop = obj.GetType().GetProperty("Condicao", BindingFlags.Public | BindingFlags.Instance);
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia condicao = prop.GetValue(obj);


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
