using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
namespace SGT.WebAdmin.Controllers.Cargas.TipoCarga
{
    [CustomAuthorize("Cargas/TipoCargaModeloVeicularAutoConfig")]
    public class TipoCargaModeloVeicularAutoConfigController : BaseController
    {
		#region Construtores

		public TipoCargaModeloVeicularAutoConfigController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarConfig()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.TipoCargaModeloVeicularAutoConfig repTipoCargaModeloVeicularAutoConfig = new Repositorio.Embarcador.Cargas.TipoCargaModeloVeicularAutoConfig(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoCargaPrioridadeCargaAutoConfig repTipoCargaPrioridadeCargaAutoConfig = new Repositorio.Embarcador.Cargas.TipoCargaPrioridadeCargaAutoConfig(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoCargaValorCargaAutoConfig repTipoCargaValorCargaAutoConfig = new Repositorio.Embarcador.Cargas.TipoCargaValorCargaAutoConfig(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicularAutoConfig tipoCargaModeloVeicularAutoConfig = repTipoCargaModeloVeicularAutoConfig.Buscar();
                List<Dominio.Entidades.Embarcador.Cargas.TipoCargaPrioridadeCargaAutoConfig> tiposCargaPrioridade;
                List<Dominio.Entidades.Embarcador.Cargas.TipoCargaValorCargaAutoConfig> tiposCargaValor;

                if (tipoCargaModeloVeicularAutoConfig == null)
                {
                    tipoCargaModeloVeicularAutoConfig = new Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicularAutoConfig();
                    tiposCargaPrioridade = new List<Dominio.Entidades.Embarcador.Cargas.TipoCargaPrioridadeCargaAutoConfig>();
                    tiposCargaValor = new List<Dominio.Entidades.Embarcador.Cargas.TipoCargaValorCargaAutoConfig>();
                }
                else
                {
                    tiposCargaPrioridade = repTipoCargaPrioridadeCargaAutoConfig.BuscarPorTipoCargaModeloAutoConfig(tipoCargaModeloVeicularAutoConfig.Codigo);
                    tiposCargaValor = repTipoCargaValorCargaAutoConfig.BuscarPorTipoCargaModeloAutoConfig(tipoCargaModeloVeicularAutoConfig.Codigo);
                }

                var dynTipoCargaModeloVeicularAutoConfig = new
                {
                    tipoCargaModeloVeicularAutoConfig.AutoModeloVeicularHabilitado,
                    tipoCargaModeloVeicularAutoConfig.TipoAutomatizacaoTipoCarga,
                    tipoCargaModeloVeicularAutoConfig.ConsiderarToleranciaMenorPesoModelo,
                    tipoCargaModeloVeicularAutoConfig.ControlarModeloPorNumeroPaletes,
                    tipoCargaModeloVeicularAutoConfig.ControlarModeloPorPeso,
                    TiposCarga = (from obj in tiposCargaPrioridade
                                  select new
                                  {
                                      Codigo = obj.TipoDeCarga.Codigo,
                                      Descricao = obj.TipoDeCarga.Descricao,
                                      Posicao = obj.Posicao
                                  }).ToList(),
                    TiposCargaValor = (from obj in tiposCargaValor
                                       select new
                                       {
                                           Codigo = obj.TipoCarga.Codigo,
                                           Descricao = obj.TipoCarga.Descricao,
                                           Valor = obj.Valor.ToString("n2"),
                                           TipoValor = obj.TipoValor,
                                           UFDestino = obj.UFDestino?.Nome ?? string.Empty,
                                           CodigoUFDestino = obj.UFDestino?.Sigla ?? string.Empty,
                                       }).ToList()
                };

                return new JsonpResult(dynTipoCargaModeloVeicularAutoConfig);
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

        public async Task<IActionResult> Salvar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAutomatizacaoTipoCarga tipoAutomatizacaoTipoCarga;
                Enum.TryParse(Request.Params("TipoAutomatizacaoTipoCarga"), out tipoAutomatizacaoTipoCarga);

                unitOfWork.Start();

                Repositorio.Embarcador.Cargas.TipoCargaModeloVeicularAutoConfig repTipoCargaModeloVeicularAutoConfig = new Repositorio.Embarcador.Cargas.TipoCargaModeloVeicularAutoConfig(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicularAutoConfig tipoCargaModeloVeicularAutoConfig = repTipoCargaModeloVeicularAutoConfig.Buscar();
                Dominio.Entidades.Auditoria.HistoricoObjeto historico = null;

                if (tipoCargaModeloVeicularAutoConfig == null)
                    tipoCargaModeloVeicularAutoConfig = new Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicularAutoConfig();
                else
                    tipoCargaModeloVeicularAutoConfig.Initialize();

                tipoCargaModeloVeicularAutoConfig.AutoModeloVeicularHabilitado = bool.Parse(Request.Params("AutoModeloVeicularHabilitado"));
                tipoCargaModeloVeicularAutoConfig.TipoAutomatizacaoTipoCarga = tipoAutomatizacaoTipoCarga;
                tipoCargaModeloVeicularAutoConfig.ConsiderarToleranciaMenorPesoModelo = bool.Parse(Request.Params("ConsiderarToleranciaMenorPesoModelo"));
                tipoCargaModeloVeicularAutoConfig.ControlarModeloPorNumeroPaletes = bool.Parse(Request.Params("ControlarModeloPorNumeroPaletes"));
                tipoCargaModeloVeicularAutoConfig.ControlarModeloPorPeso = bool.Parse(Request.Params("ControlarModeloPorPeso"));

                if (tipoCargaModeloVeicularAutoConfig.Codigo <= 0)
                    repTipoCargaModeloVeicularAutoConfig.Inserir(tipoCargaModeloVeicularAutoConfig, Auditado);
                else
                    historico = repTipoCargaModeloVeicularAutoConfig.Atualizar(tipoCargaModeloVeicularAutoConfig, Auditado);

                SalvarTipoCargaPrioridade(tipoCargaModeloVeicularAutoConfig, historico, unitOfWork);
                SalvarTipoCargaValor(tipoCargaModeloVeicularAutoConfig, historico, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoSalvar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void SalvarTipoCargaPrioridade(Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicularAutoConfig tipoCargaModeloVeicularAutoConfig, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TipoCargaPrioridadeCargaAutoConfig repTipoCargaPrioridadeCargaAutoConfig = new Repositorio.Embarcador.Cargas.TipoCargaPrioridadeCargaAutoConfig(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.TipoCargaPrioridadeCargaAutoConfig> tiposCargaPrioridade = repTipoCargaPrioridadeCargaAutoConfig.BuscarPorTipoCargaModeloAutoConfig(tipoCargaModeloVeicularAutoConfig.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.TipoCargaPrioridadeCargaAutoConfig tipoCargaPrioridade in tiposCargaPrioridade)
                repTipoCargaPrioridadeCargaAutoConfig.Deletar(tipoCargaPrioridade, Auditado, historico);

            var jTiposCarga = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("TiposCarga"));

            foreach (var jTipoCarga in jTiposCarga)
            {
                Dominio.Entidades.Embarcador.Cargas.TipoCargaPrioridadeCargaAutoConfig tipoCargaPrioridade = new Dominio.Entidades.Embarcador.Cargas.TipoCargaPrioridadeCargaAutoConfig();
                tipoCargaPrioridade.TipoDeCarga = new Dominio.Entidades.Embarcador.Cargas.TipoDeCarga() { Codigo = (int)jTipoCarga.Codigo };
                tipoCargaPrioridade.TipoCargaModeloVeicularAutoConfig = tipoCargaModeloVeicularAutoConfig;
                tipoCargaPrioridade.Posicao = (int)jTipoCarga.Posicao;

                repTipoCargaPrioridadeCargaAutoConfig.Inserir(tipoCargaPrioridade, Auditado, historico);
            }
        }

        private void SalvarTipoCargaValor(Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicularAutoConfig tipoCargaModeloVeicularAutoConfig, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TipoCargaValorCargaAutoConfig repTipoCargaValorCargaAutoConfig = new Repositorio.Embarcador.Cargas.TipoCargaValorCargaAutoConfig(unitOfWork);
            Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.TipoCargaValorCargaAutoConfig> tiposCargaValor = repTipoCargaValorCargaAutoConfig.BuscarPorTipoCargaModeloAutoConfig(tipoCargaModeloVeicularAutoConfig.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.TipoCargaValorCargaAutoConfig tipoCargaValor in tiposCargaValor)
                repTipoCargaValorCargaAutoConfig.Deletar(tipoCargaValor, Auditado, historico);

            var jTiposCarga = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("TiposCargaValor"));


            foreach (var jTipoCarga in jTiposCarga)
            {
                string ufSigla = jTipoCarga.CodigoUFDestino;
                Dominio.Entidades.Embarcador.Cargas.TipoCargaValorCargaAutoConfig tipoCargaValor = new Dominio.Entidades.Embarcador.Cargas.TipoCargaValorCargaAutoConfig();
                tipoCargaValor.TipoCarga = new Dominio.Entidades.Embarcador.Cargas.TipoDeCarga() { Codigo = (int)jTipoCarga.Codigo };
                tipoCargaValor.TipoCargaModeloVeicularAutoConfig = tipoCargaModeloVeicularAutoConfig;
                tipoCargaValor.Valor = decimal.Parse((string)jTipoCarga.Valor);
                tipoCargaValor.TipoValor = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.ValorAutomatizacaoTipoCargaValor)jTipoCarga.TipoValor;
                tipoCargaValor.UFDestino = !string.IsNullOrWhiteSpace(ufSigla) ? repEstado.BuscarPorSigla(ufSigla) : null;
                repTipoCargaValorCargaAutoConfig.Inserir(tipoCargaValor, Auditado, historico);
            }
        }

        #endregion
    }
}
