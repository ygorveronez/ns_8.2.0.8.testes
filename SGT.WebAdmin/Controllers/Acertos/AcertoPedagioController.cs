using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Acertos
{
    [CustomAuthorize(new string[] { "VerificaPedagioDuplicado", "ContemPedagioPendenteAutorizacao" }, "Acertos/AcertoViagem")]
    public class AcertoPedagioController : BaseController
    {
		#region Construtores

		public AcertoPedagioController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> AtualizarPedagios()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller AtualizarPedagios " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                Repositorio.Embarcador.Acerto.AcertoPedagio repAcertoPedagio = new Repositorio.Embarcador.Acerto.AcertoPedagio(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
                Servicos.Embarcador.Acerto.AcertoViagem servAcertoViagem = new Servicos.Embarcador.Acerto.AcertoViagem(unitOfWork);

                //unitOfWork.Start(IsolationLevel.ReadUncommitted);

                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                bool aprovacaoPedagio;
                bool.TryParse(Request.Params("AprovacaoPedagio"), out aprovacaoPedagio);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapasAcertoViagem etapa;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem situacao;
                Enum.TryParse(Request.Params("Etapa"), out etapa);
                Enum.TryParse(Request.Params("Situacao"), out situacao);

                Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem;
                if (codigo > 0)
                    acertoViagem = repAcertoViagem.BuscarPorCodigo(codigo, true);
                else
                    return new JsonpResult(false, "Por favor inicie o acerto de viagem antes.");

                acertoViagem.Etapa = etapa;
                acertoViagem.Situacao = situacao;
                acertoViagem.DataAlteracao = DateTime.Now;
                acertoViagem.PedagioSalvo = true;        
                
                servAcertoViagem.AtualizarPedagiosAcerto(acertoViagem, unitOfWork, Request.Params("ListaPedagios"), Request.Params("ListaPedagiosCredito"), Auditado);
                servAcertoViagem.InserirLogAcerto(acertoViagem, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoLogAcertoViagem.Pedagios, this.Usuario);

                if (servAcertoViagem.ContemPedagioPendenteAutorizacao(acertoViagem.Codigo, unitOfWork))
                    acertoViagem.AprovacaoPedagio = false;
                else
                    acertoViagem.AprovacaoPedagio = true;

                repAcertoViagem.Atualizar(acertoViagem, Auditado);

                //unitOfWork.CommitChanges();

                var dynRetorno = new { Codigo = acertoViagem.Codigo };

                return new JsonpResult(dynRetorno, true, "Sucesso");
            }
            catch (Exception ex)
            {
                //unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar o pedágio.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller AtualizarPedagios " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }
        
        public async Task<IActionResult> AutorizarPedagios()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller AutorizarPedagios " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Acertos/AcertoViagem");

                if (!(this.Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains( AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Acerto_PermiteLiberarPedagioAcerto)))
                    return new JsonpResult(false, "Seu usuário não possui permissão para autorizar os pedágios duplicados.");

                Repositorio.Embarcador.Acerto.AcertoPedagio repAcertoPedagio = new Repositorio.Embarcador.Acerto.AcertoPedagio(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
                Servicos.Embarcador.Acerto.AcertoViagem servAcertoViagem = new Servicos.Embarcador.Acerto.AcertoViagem(unitOfWork);

                //unitOfWork.Start(IsolationLevel.ReadUncommitted);

                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                bool aprovacaoPedagio;
                bool.TryParse(Request.Params("AprovacaoPedagio"), out aprovacaoPedagio);                
                

                Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem;
                if (codigo > 0)
                    acertoViagem = repAcertoViagem.BuscarPorCodigo(codigo, true);
                else
                    return new JsonpResult(false, "Por favor inicie o acerto de viagem antes.");
                
                acertoViagem.AprovacaoPedagio = aprovacaoPedagio;

                repAcertoViagem.Atualizar(acertoViagem, Auditado);

                servAcertoViagem.AtualizarPedagiosAcerto(acertoViagem, unitOfWork, Request.Params("ListaPedagios"), Request.Params("ListaPedagiosCredito"), Auditado);
                servAcertoViagem.InserirLogAcerto(acertoViagem, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoLogAcertoViagem.Pedagios, this.Usuario);

                //unitOfWork.CommitChanges();

                var dynRetorno = new { Codigo = acertoViagem.Codigo }; //servAcertoViagem.RetornaObjetoCompletoAcertoViagem(acertoViagem.Codigo, unitOfWork);

                return new JsonpResult(dynRetorno, true, "Sucesso");
            }
            catch (Exception ex)
            {
                //unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualiar o pedágio.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller AutorizarPedagios " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }
        
        public async Task<IActionResult> VerificaPedagioDuplicado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller VerificaPedagioDuplicado " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                Repositorio.Embarcador.Acerto.AcertoPedagio repAcertoPedagio = new Repositorio.Embarcador.Acerto.AcertoPedagio(unitOfWork);                

                unitOfWork.Start(IsolationLevel.ReadUncommitted);

                int codigoVeiculo;
                int.TryParse(Request.Params("CodigoVeiculo"), out codigoVeiculo);

                string praca = Request.Params("Praca");
                string rodovia = Request.Params("Rodovia");

                DateTime data;
                DateTime.TryParse(Request.Params("DataHota"), out data);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedagio tipoPedagio;
                Enum.TryParse(Request.Params("TipoPedagio"), out tipoPedagio);

                var pedagioDuplicado = repAcertoPedagio.PedagioDuplicado(tipoPedagio, praca, rodovia, data, codigoVeiculo);

                var dynRetorno = new
                {
                    PedagioDuplicado = pedagioDuplicado
                };

                unitOfWork.CommitChanges();

                return new JsonpResult(dynRetorno, true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar o pedágio duplicado.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Fim Controller VerificaPedagioDuplicado " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ContemPedagioPendenteAutorizacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Log.TratarErro(" Inicio Controller ContemPedagioPendenteAutorizacao " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            try
            {
                Repositorio.Embarcador.Acerto.AcertoAbastecimento repAcertoAbastecimento = new Repositorio.Embarcador.Acerto.AcertoAbastecimento(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
                Servicos.Embarcador.Acerto.AcertoViagem servAcertoViagem = new Servicos.Embarcador.Acerto.AcertoViagem(unitOfWork);

                unitOfWork.Start(IsolationLevel.ReadUncommitted);

                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem;
                if (codigo > 0)
                {
                    acertoViagem = repAcertoViagem.BuscarPorCodigo(codigo);
                    var contemResumoPendente = false;
                    if (!acertoViagem.AprovacaoPedagio)
                        contemResumoPendente = servAcertoViagem.ContemPedagioPendenteAutorizacao(codigo, unitOfWork);

                    unitOfWork.CommitChanges();

                    var dynRetorno = new
                    {
                        ContemPedagioPendente = contemResumoPendente
                    };

                    return new JsonpResult(dynRetorno, true, "Sucesso");
                }
                else {
                    var dynRetorno = new
                    {
                        ContemPedagioPendente = false
                    };

                    return new JsonpResult(dynRetorno, true, "Sucesso");                    
                }
                
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os pedagios pendentes.");
            }
            finally
            {
                Servicos.Log.TratarErro(" Inicio Controller ContemPedagioPendenteAutorizacao " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                unitOfWork.Dispose();
            }
        }
    }
}


