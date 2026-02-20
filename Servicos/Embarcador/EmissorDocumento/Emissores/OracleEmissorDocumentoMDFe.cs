using Dominio.ObjetosDeValor.Embarcador.EmissorDocumento;
using System;

namespace Servicos.Embarcador.EmissorDocumento.Emissores
{
    public class OracleEmissorDocumentoMDFe : IEmissorDocumentoMDFe
    {
        #region Métodos Globais

        public bool EmitirMDFe(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, int codigoEmpresa, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.MDFe svcMDFe = new Servicos.MDFe(unitOfWork);
            return svcMDFe.EmitirMDFe(mdfe, codigoEmpresa, unitOfWork);
        }

        public bool ReceberEventoMdfe(out string mensagemErro, Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, RetornoEventoCTe retornoEventoMDFe, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            mensagemErro = string.Empty;

            if (retornoEventoMDFe.mdfeOracle == null)
                return true;

            Servicos.MDFe serMDFe = new Servicos.MDFe(unitOfWork);
            if (!serMDFe.ProcessarRetornoMDFeAutorizado(out mensagemErro, retornoEventoMDFe.mdfeOracle, mdfe, Auditado, tipoServicoMultisoftware, unitOfWork))
                return false;

            return true;
        }

        public bool CancelarMdfe(int codigoMDFe, int codigoEmpresa, string justificativa, Repositorio.UnitOfWork unitOfWork, DateTime? dataCancelamento = null, string cobrarCancelamento = "")
        {
            Servicos.MDFe serMDFe = new Servicos.MDFe(unitOfWork);
            return serMDFe.Cancelar(codigoMDFe, codigoEmpresa, justificativa, unitOfWork, dataCancelamento, cobrarCancelamento);
        }

        public bool EncerrarMdfe(int codigoMDFe, int codigoEmpresa, DateTime dataEncerramento, Repositorio.UnitOfWork unitOfWork, DateTime? dataEvento = null)
        {
            Servicos.MDFe serMDFe = new Servicos.MDFe(unitOfWork);
            return serMDFe.Encerrar(codigoMDFe, codigoEmpresa, dataEncerramento, unitOfWork, dataEvento);
        }

        public bool EncerrarMdfeEmissorExterno(Dominio.ObjetosDeValor.MDFe.MDFeEmissorExterno mdfeEmissorExterno, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.MDFe serMDFe = new Servicos.MDFe(unitOfWork);
            return serMDFe.EncerrarMDFeEmissorExterno(mdfeEmissorExterno, unitOfWork);
        }

        public bool IncluirMotorista(int codigoMDFe, int codigoEmpresa, string cpfMotorista, string nomeMotorista, Repositorio.UnitOfWork unitOfWork, DateTime? dataEvento = null)
        {
            Servicos.MDFe serMDFe = new Servicos.MDFe(unitOfWork);
            return serMDFe.IncluirMotorista(codigoMDFe, codigoEmpresa, cpfMotorista, nomeMotorista, unitOfWork, dataEvento);
        }

        public bool IncluirMotorista(int codigoMDFe, int codigoEmpresa, string cpfMotorista, string nomeMotorista, string stringConexao, DateTime? dataEvento = null)
        {
            Servicos.MDFe serMDFe = new Servicos.MDFe();
            return serMDFe.IncluirMotorista(codigoMDFe, codigoEmpresa, cpfMotorista, nomeMotorista, null, dataEvento);
        }

        public bool SolicitarEmissaoContingencia(int codigoMDFe, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.MDFe serMDFe = new Servicos.MDFe(unitOfWork);
            return serMDFe.SolicitarEmissaoContingencia(codigoMDFe, unitOfWork);
        }

        public bool ConsultarMdfe(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.MDFe serMDFe = new Servicos.MDFe(unitOfWork);
            return serMDFe.sincronizarDocumentoEmProcessamento(mdfe, Auditado, tipoServicoMultisoftware, unitOfWork);
        }

        public Dominio.ObjetosDeValor.Embarcador.EmissorDocumento.RetornoStatusMDFePorChave ConsultarMdfeEmissorExterno(string chave, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.MDFe serMDFe = new Servicos.MDFe(unitOfWork);
            return serMDFe.ConsultarStatusMDFePorChave(chave);
        }

        public Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais ConsultarEventoCancelamento(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.MDFe serMDFe = new Servicos.MDFe(unitOfWork);
            return serMDFe.ConsultarCancelamento(mdfe.Codigo, unitOfWork);
        }

        public Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais ConsultarEventoEncerramento(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.MDFe serMDFe = new Servicos.MDFe(unitOfWork);
            return serMDFe.ConsultarEncerramento(mdfe, unitOfWork);
        }

        public Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais ConsultarEventoInclusaoMotorista(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.MDFe serMDFe = new Servicos.MDFe(unitOfWork);
            return serMDFe.ConsultarEventoInclusaoMotorista(mdfe, unitOfWork);
        }

        public byte[] ObterXMLMdfeAutorizacao(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, int codigoEmpresa, Repositorio.UnitOfWork unitOfWork)
        {
            byte[] retorno = null;
            Servicos.MDFe serMDFe = new Servicos.MDFe(unitOfWork);

            try
            {
                retorno = serMDFe.ObterESalvarXMLAutorizacao(mdfe, codigoEmpresa, unitOfWork);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }

            return retorno;
        }

        public byte[] ObterXMLMdfeCancelamento(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, int codigoEmpresa, Repositorio.UnitOfWork unitOfWork)
        {
            byte[] retorno = null;
            Servicos.MDFe serMDFe = new Servicos.MDFe(unitOfWork);

            try
            {
                retorno = serMDFe.ObterESalvarXMLCancelamento(mdfe.Codigo, codigoEmpresa, null, unitOfWork);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
            }

            return retorno;
        }

        #endregion Métodos Globais

        #region Métodos Privados

        #endregion Métodos Privados
    }
}