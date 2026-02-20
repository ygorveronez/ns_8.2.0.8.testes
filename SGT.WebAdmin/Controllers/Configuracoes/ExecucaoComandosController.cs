using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Configuracoes
{
    [CustomAuthorize("Configuracoes/ExecucaoComandos")]
    public class ExecucaoComandosController : BaseController
    {
		#region Construtores

		public ExecucaoComandosController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Públicos

        public async Task<IActionResult> AlterarStatusCTeRejeitado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<int> codigosCTes = Request.GetListParam<int>("AlterarStatusCTeRejeitado");

                if (codigosCTes.Count == 0)
                    return new JsonpResult(false, true, "Nenhum registro selecionado.");

                Repositorio.Embarcador.Configuracoes.ExecucaoComandos repExecucaoComandos = new Repositorio.Embarcador.Configuracoes.ExecucaoComandos(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCte = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repCte.BuscarCTesPorCodigos(codigosCTes);

                if (ctes.Any(obj => obj.Status != "E"))
                    return new JsonpResult(false, true, "Há registro(s) em situação não permitida para essa alteração.");

                unitOfWork.Start();

                repExecucaoComandos.AlterarStatusCTes(ctes.Select(obj => obj.Codigo).ToList(), "R");

                foreach (var cte in ctes)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cte, null, "Status do CT-e alterado de Em Emissão para Rejeitado", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar alterar o(s) registro(s).");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarStatusCTeAutorizado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<int> codigosCTes = Request.GetListParam<int>("AlterarStatusCTeAutorizado");

                if (codigosCTes.Count == 0)
                    return new JsonpResult(false, true, "Nenhum registro selecionado.");

                Repositorio.Embarcador.Configuracoes.ExecucaoComandos repExecucaoComandos = new Repositorio.Embarcador.Configuracoes.ExecucaoComandos(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCte = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repCte.BuscarCTesPorCodigos(codigosCTes);

                if (ctes.Any(obj => obj.Status != "K"))
                    return new JsonpResult(false, true, "Há registro(s) em situação não permitida para essa alteração.");

                unitOfWork.Start();

                repExecucaoComandos.AlterarStatusCTes(ctes.Select(obj => obj.Codigo).ToList(), "A");

                foreach (var cte in ctes)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cte, null, "Status do CT-e alterado de Em Cancelamento para Autorizado", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar alterar o(s) registro(s).");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarStatusCTeEmInutilizacaoRejeitado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<int> codigosCTes = Request.GetListParam<int>("AlterarStatusCTeInutilizacaoRejeitado");

                if (codigosCTes.Count == 0)
                    return new JsonpResult(false, true, "Nenhum registro selecionado.");

                Repositorio.Embarcador.Configuracoes.ExecucaoComandos repExecucaoComandos = new Repositorio.Embarcador.Configuracoes.ExecucaoComandos(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCte = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repCte.BuscarCTesPorCodigos(codigosCTes);

                if (ctes.Any(obj => obj.Status != "L"))
                    return new JsonpResult(false, true, "Há registro(s) em situação não permitida para essa alteração.");

                unitOfWork.Start();

                repExecucaoComandos.AlterarStatusCTes(ctes.Select(obj => obj.Codigo).ToList(), "R");

                foreach (var cte in ctes)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cte, null, "Status do CT-e alterado de Em Inutilização para Rejeitado", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar alterar o(s) registro(s).");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReverterAnulacaoGerencialCTe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<int> codigosCTes = Request.GetListParam<int>("ReverterAnulacaoGerencialCTe");

                if (codigosCTes.Count == 0)
                    return new JsonpResult(false, true, "Nenhum registro selecionado.");

                Repositorio.Embarcador.Configuracoes.ExecucaoComandos repExecucaoComandos = new Repositorio.Embarcador.Configuracoes.ExecucaoComandos(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCte = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repCte.BuscarCTesPorCodigos(codigosCTes);

                if (ctes.Any(obj => obj.Status != "Z"))
                    return new JsonpResult(false, true, "Há registro(s) em situação não permitida para essa alteração.");

                unitOfWork.Start();

                repExecucaoComandos.AlterarStatusCTes(ctes.Select(obj => obj.Codigo).ToList(), "A");
                repExecucaoComandos.AutorizarFaturamentosCTe(ctes.Select(obj => obj.Codigo).ToList());

                foreach (var cte in ctes)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cte, null, "Status do CT-e alterado de Anulado para Autorizado", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar alterar o(s) registro(s).");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> CancelarCargaNovaNaoFechada()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<int> codigosCargas = Request.GetListParam<int>("CancelarCargaNovaNaoFechada");

                if (codigosCargas.Count == 0)
                    return new JsonpResult(false, true, "Nenhum registro selecionado.");

                Repositorio.Embarcador.Configuracoes.ExecucaoComandos repExecucaoComandos = new Repositorio.Embarcador.Configuracoes.ExecucaoComandos(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repCarga.BuscarCargasPorCodigos(codigosCargas);

                if (cargas.Any(obj => obj.CargaFechada && obj.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova))
                    return new JsonpResult(false, true, "Há registro(s) em situação não permitida para essa alteração.");

                unitOfWork.Start();

                repExecucaoComandos.AlterarSituacaoCargas(cargas.Select(obj => obj.Codigo).ToList(), Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada);

                foreach (var carga in cargas)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Carga cancelada manualmente pelo suporte", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar alterar o(s) registro(s).");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExecutarScriptPreCadastrado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoScript = Request.GetIntParam("ExecutarScriptPreCadastrado");

                Repositorio.Embarcador.Configuracoes.ExecucaoComandos repExecucaoComandos = new Repositorio.Embarcador.Configuracoes.ExecucaoComandos(unitOfWork);
                Repositorio.Embarcador.Configuracoes.Script repScript = new Repositorio.Embarcador.Configuracoes.Script(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.Script script = repScript.BuscarPorCodigo(codigoScript);

                if (script == null)
                    return new JsonpResult(false, true, "Nenhum registro selecionado.");

                unitOfWork.Start();

                repExecucaoComandos.ExecutarScript(script.ScriptSQL);
                Servicos.Log.TratarErro($"Usuário: {Usuario.Descricao} SQL: {script.ScriptSQL}" , "ExecucaoComando");

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao executar o comando SQL.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
