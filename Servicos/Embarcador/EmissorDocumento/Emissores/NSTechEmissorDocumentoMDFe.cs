using Dominio.ObjetosDeValor.Embarcador.EmissorDocumento;
using System;

namespace Servicos.Embarcador.EmissorDocumento.Emissores
{
    public class NSTechEmissorDocumentoMDFe : IEmissorDocumentoMDFe
    {
        #region Métodos Globais

        public bool EmitirMDFe(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, int codigoEmpresa, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech serIntegracaoNSTech = new Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech(unitOfWork);
            return serIntegracaoNSTech.EnviarMDFeEmissor(mdfe, codigoEmpresa, unitOfWork);
        }

        public bool ReceberEventoMdfe(out string mensagemErro, Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, RetornoEventoCTe retornoEventoMDFe, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            bool sucesso = true;
            string tipoEvento = string.Empty;
            mensagemErro = string.Empty;

            try
            {
                Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech serIntegracaoNSTech = new Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech(unitOfWork);

                dynamic objetoRetorno = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(retornoEventoMDFe.objeto.ToString());
                tipoEvento = (string)objetoRetorno.type;

                switch (tipoEvento)
                {
                    case "com.nstech.issuance-engine.mdfe-authorized":
                    case "com.nstech.issuance-engine.mdfe-rejected":
                    case "com.nstech.issuance-engine.mdfe-error":
                        if (!serIntegracaoNSTech.ProcessarMDFe(out mensagemErro, tipoEvento, mdfe, objetoRetorno, retornoEventoMDFe, Auditado, tipoServicoMultisoftware, unitOfWork))
                            sucesso = false;
                        break;

                    case "com.nstech.issuance-engine.mdfe-event-authorized":
                    case "com.nstech.issuance-engine.mdfe-event-error":
                    case "com.nstech.issuance-engine.mdfe-event-rejected":
                        if (!serIntegracaoNSTech.ProcessarEventoMDFe(out mensagemErro, tipoEvento, mdfe, objetoRetorno, retornoEventoMDFe, Auditado, tipoServicoMultisoftware, unitOfWork))
                            sucesso = false;
                        break;

                    case "com.nstech.issuance-engine.mdfe-damdfe-generated":
                        if (!serIntegracaoNSTech.ProcessarRecebimentoDamdfe(out mensagemErro, tipoEvento, mdfe, objetoRetorno, retornoEventoMDFe, Auditado, tipoServicoMultisoftware, unitOfWork))
                            sucesso = false;
                        break;

                    default:
                        Log.TratarErro($"ReceberEventoMDFe - Mensagem: Evento não homologado, Tipo Evento: {tipoEvento}, Data: {retornoEventoMDFe.objeto.ToString()}");
                        mensagemErro = "Evento não homologado";
                        sucesso = false;
                        break;
                }

            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                Log.TratarErro($"ReceberEventoCTe - Mensagem: Ocorreu uma falha ao receber o evento, Tipo Evento: {tipoEvento}, Data: {retornoEventoMDFe.objeto.ToString()}");
                mensagemErro = "Ocorreu uma falha ao receber o evento.";
                sucesso = false;
            }

            return sucesso;
        }

        public bool CancelarMdfe(int codigoMDFe, int codigoEmpresa, string justificativa, Repositorio.UnitOfWork unitOfWork = null, DateTime? dataCancelamento = null, string cobrarCancelamento = "")
        {
            Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech serIntegracaoNSTech = new Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech(unitOfWork);
            return serIntegracaoNSTech.CancelarMdfe(codigoMDFe, codigoEmpresa, justificativa, unitOfWork, null, dataCancelamento, cobrarCancelamento);
        }

        public bool EncerrarMdfe(int codigoMDFe, int codigoEmpresa, DateTime dataEncerramento, Repositorio.UnitOfWork unitOfWork, DateTime? dataEvento = null)
        {
            Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech serIntegracaoNSTech = new Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech(unitOfWork);
            return serIntegracaoNSTech.EncerrarMdfe(codigoMDFe, codigoEmpresa, dataEncerramento, unitOfWork, null, dataEvento);
        }

        public bool EncerrarMdfeEmissorExterno(Dominio.ObjetosDeValor.MDFe.MDFeEmissorExterno mdfeEmissorExterno, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech serIntegracaoNSTech = new Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech(unitOfWork);
            return serIntegracaoNSTech.EncerrarMDFeEmissorExterno(mdfeEmissorExterno, unitOfWork);
        }

        public bool IncluirMotorista(int codigoMDFe, int codigoEmpresa, string cpfMotorista, string nomeMotorista, Repositorio.UnitOfWork unitOfWork, DateTime? dataEvento = null)
        {
            Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech serIntegracaoNSTech = new Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech(unitOfWork);
            return serIntegracaoNSTech.IncluirMotorista(codigoMDFe, codigoEmpresa, cpfMotorista, nomeMotorista, unitOfWork, null, dataEvento);
        }

        public bool IncluirMotorista(int codigoMDFe, int codigoEmpresa, string cpfMotorista, string nomeMotorista, string stringConexao, DateTime? dataEvento = null)
        {
            Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech serIntegracaoNSTech = new Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech(stringConexao);
            return serIntegracaoNSTech.IncluirMotorista(codigoMDFe, codigoEmpresa, cpfMotorista, nomeMotorista, null, stringConexao, dataEvento);
        }

        public bool SolicitarEmissaoContingencia(int codigoMDFe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
            Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe);

            Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech serIntegracaoNSTech = new Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech(unitOfWork);
            return serIntegracaoNSTech.EnviarMDFeEmissor(mdfe, mdfe.Empresa.Codigo, unitOfWork, true);
        }

        public bool ConsultarMdfe(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech serIntegracaoNSTech = new Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech(unitOfWork);
            return serIntegracaoNSTech.ConsultarMdfe(mdfe, Auditado, tipoServicoMultisoftware, unitOfWork);
        }

        public Dominio.ObjetosDeValor.Embarcador.EmissorDocumento.RetornoStatusMDFePorChave ConsultarMdfeEmissorExterno(string chave, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech serIntegracaoNSTech = new Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech(unidadeDeTrabalho);
            return serIntegracaoNSTech.ConsultarMdfeEmissorExterno(chave);
        }

        public Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais ConsultarEventoCancelamento(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMdfe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);

            Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech serIntegracaoNSTech = new Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech(unitOfWork);
            serIntegracaoNSTech.ConsultarMdfe(mdfe, Auditado, tipoServicoMultisoftware, unitOfWork);

            return repMdfe.BuscarPorCodigo(mdfe.Codigo);
        }

        public Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais ConsultarEventoEncerramento(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMdfe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);

            Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech serIntegracaoNSTech = new Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech(unitOfWork);
            serIntegracaoNSTech.ConsultarMdfe(mdfe, Auditado, tipoServicoMultisoftware, unitOfWork);

            return repMdfe.BuscarPorCodigo(mdfe.Codigo);
        }

        public Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais ConsultarEventoInclusaoMotorista(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMdfe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);

            Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech serIntegracaoNSTech = new Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech(unitOfWork);
            serIntegracaoNSTech.ConsultarMdfe(mdfe, Auditado, tipoServicoMultisoftware, unitOfWork);

            return repMdfe.BuscarPorCodigo(mdfe.Codigo);
        }

        public byte[] ObterXMLMdfeAutorizacao(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, int codigoEmpresa, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech serIntegracaoNSTech = new Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech(unitOfWork);
            return serIntegracaoNSTech.ObterXMLMdfe(mdfe, codigoEmpresa, unitOfWork);
        }

        public byte[] ObterXMLMdfeCancelamento(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, int codigoEmpresa, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech serIntegracaoNSTech = new Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech(unitOfWork);
            return serIntegracaoNSTech.ObterXMLMdfe(mdfe, codigoEmpresa, unitOfWork, true);
        }

        #endregion Métodos Globais

        #region Métodos Privados

        #endregion Métodos Privados
    }
}