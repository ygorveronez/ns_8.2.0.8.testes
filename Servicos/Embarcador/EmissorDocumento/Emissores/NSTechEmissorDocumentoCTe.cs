using Dominio.ObjetosDeValor.Embarcador.EmissorDocumento;
using System;

namespace Servicos.Embarcador.EmissorDocumento.Emissores
{
    public class NSTechEmissorDocumentoCTe : IEmissorDocumentoCTe
    {
        private readonly bool _obterXmlAutomaticamente;

        #region Métodos Globais

        public NSTechEmissorDocumentoCTe(bool obterXmlAutomaticamente)
        {
            _obterXmlAutomaticamente = obterXmlAutomaticamente;
        }

        #endregion Métodos Globais

        #region Métodos Globais

        public bool EmitirCte(ref Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork, string statusPosEmissao = "E", string wsOracle = "")
        {
            Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech serIntegracaoNSTech = new Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech(unitOfWork);
            return serIntegracaoNSTech.EnviarCteEmissor(ref cte, empresa, unitOfWork);
        }

        public bool ReceberEventoCte(out string mensagemErro, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, RetornoEventoCTe retornoEventoCTe, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            bool sucesso = true;
            string tipoEvento = string.Empty;
            mensagemErro = string.Empty;

            try
            {
                Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech serIntegracaoNSTech = new Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech(unitOfWork);

                dynamic objetoRetorno = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(retornoEventoCTe.objeto.ToString());
                tipoEvento = (string)objetoRetorno.type;

                switch (tipoEvento)
                {
                    case "com.nstech.issuance-engine.cte-authorized":
                    case "com.nstech.issuance-engine.cte-rejected":
                    case "com.nstech.issuance-engine.cte-error":
                        if (!serIntegracaoNSTech.ProcessarCTe(out mensagemErro, tipoEvento, cte, objetoRetorno, retornoEventoCTe, Auditado, tipoServicoMultisoftware, unitOfWork, _obterXmlAutomaticamente))
                            sucesso = false;
                        break;

                    case "com.nstech.issuance-engine.cte-event-authorized":
                    case "com.nstech.issuance-engine.cte-event-rejected":
                    case "com.nstech.issuance-engine.cte-event-error":
                        Dominio.Entidades.CartaDeCorrecaoEletronica cce = null;
                        if (!serIntegracaoNSTech.ProcessarEventoCTe(out mensagemErro, tipoEvento, cte, ref cce, objetoRetorno, retornoEventoCTe, Auditado, tipoServicoMultisoftware, unitOfWork))
                            sucesso = false;
                        break;

                    case "com.nstech.issuance-engine.cte-dacte-generated":
                        if (!serIntegracaoNSTech.ProcessarRecebimentoDacte(out mensagemErro, tipoEvento, cte, objetoRetorno, retornoEventoCTe, Auditado, tipoServicoMultisoftware, unitOfWork))
                            sucesso = false;
                        break;

                    case "com.nstech.issuance-engine.mdfe-authorized":
                    case "com.nstech.issuance-engine.mdfe-rejected":
                    case "com.nstech.issuance-engine.mdfe-error":
                    case "com.nstech.issuance-engine.mdfe-event-authorized":
                    case "com.nstech.issuance-engine.mdfe-event-error":
                    case "com.nstech.issuance-engine.mdfe-event-rejected":
                    case "com.nstech.issuance-engine.mdfe-damdfe-generated":
                    case "com.nstech.issuance-engine.mdfe-damdfe-error":
                        return Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoMDFe(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.NSTech).ReceberEventoMdfe(out mensagemErro, null, retornoEventoCTe, Auditado, tipoServicoMultisoftware, unitOfWork);

                    default:
                        Log.TratarErro($"ReceberEventoCTe - Mensagem: Evento não homologado, Tipo Evento: {tipoEvento}, Data: {retornoEventoCTe.objeto.ToString()}");
                        mensagemErro = "Evento não homologado";
                        sucesso = false;
                        break;
                }

            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                Log.TratarErro($"ReceberEventoCTe - Mensagem: Ocorreu uma falha ao receber o evento, Tipo Evento: {tipoEvento}, Data: {retornoEventoCTe.objeto.ToString()}");
                mensagemErro = "Ocorreu uma falha ao receber o evento.";
                sucesso = false;
            }

            return sucesso;
        }

        public bool ConsultarCte(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, string wsOracle = "")
        {
            Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech serIntegracaoNSTech = new Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech(unitOfWork);
            return serIntegracaoNSTech.ConsultarCte(cte, Auditado, tipoServicoMultisoftware, unitOfWork);
        }

        public byte[] ObterXMLCteAutorizacao(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech serIntegracaoNSTech = new Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech(unitOfWork);
            return serIntegracaoNSTech.ObterXMLCte(cte, Auditado, tipoServicoMultisoftware, unitOfWork);
        }

        public byte[] ObterXMLCteCancelamento(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech serIntegracaoNSTech = new Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech(unitOfWork);
            return serIntegracaoNSTech.ObterXMLCte(cte, Auditado, tipoServicoMultisoftware, unitOfWork, true);
        }

        public bool CancelarCte(int codigoCTe, int codigoEmpresa, string justificativa, Repositorio.UnitOfWork unitOfWork, DateTime? dataCancelamento = null, bool gerarLog = false, Dominio.Entidades.Usuario usuario = null, string cobrarCancelamento = "")
        {
            Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech serIntegracaoNSTech = new Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech(unitOfWork);
            return serIntegracaoNSTech.CancelarCte(codigoCTe, codigoEmpresa, justificativa, unitOfWork, null, dataCancelamento, gerarLog, usuario, cobrarCancelamento);
        }

        //public bool CancelarCte(int codigoCTe, int codigoEmpresa, string justificativa, string stringConexao, DateTime? dataCancelamento = null, bool gerarLog = false, Dominio.Entidades.Usuario usuario = null, string cobrarCancelamento = "")
        //{
        //    Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech serIntegracaoNSTech = new Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech(stringConexao);
        //    return serIntegracaoNSTech.CancelarCte(codigoCTe, codigoEmpresa, justificativa, null, stringConexao, dataCancelamento, gerarLog, usuario, cobrarCancelamento);
        //}

        public bool EmitirCCe(Dominio.Entidades.CartaDeCorrecaoEletronica cce, int codigoEmpresa, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech serIntegracaoNSTech = new Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech(unitOfWork);
            return serIntegracaoNSTech.EmitirCCe(cce, codigoEmpresa, unitOfWork);
        }

        public bool ReceberEventoCCe(out string mensagemErro, out Exception exception, Dominio.ObjetosDeValor.WebService.CTe.CTeOracle cceOracle, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            mensagemErro = "metodo não homologado";
            exception = null;
            return false;
        }

        public Dominio.Entidades.CartaDeCorrecaoEletronica ConsultarCCe(int codigoCCe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.CartaDeCorrecaoEletronica repCCe = new Repositorio.CartaDeCorrecaoEletronica(unidadeDeTrabalho);
            Dominio.Entidades.CartaDeCorrecaoEletronica cce = repCCe.BuscarPorCodigo(codigoCCe);

            if (cce != null)
            {
                Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech serIntegracaoNSTech = new Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech(unidadeDeTrabalho);
                serIntegracaoNSTech.ConsultarCCe(ref cce, unidadeDeTrabalho);
            }

            return cce;
        }

        public byte[] ObterXMLCCe(Dominio.Entidades.CartaDeCorrecaoEletronica cce, Repositorio.UnitOfWork unitOfWork)
        {
            return null;
        }

        public bool EnviarEmail(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, string emails, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech serIntegracaoNSTech = new Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech(unitOfWork);
            return serIntegracaoNSTech.SolicitarDacte(cte, emails);
        }

        #endregion Métodos Globais

        #region Métodos Privados

        #endregion Métodos Privados
    }
}