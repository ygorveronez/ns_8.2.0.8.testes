using Dominio.ObjetosDeValor.Embarcador.EmissorDocumento;
using System;

namespace Servicos.Embarcador.EmissorDocumento
{
    public interface IEmissorDocumentoMDFe
    {
        bool EmitirMDFe(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, int codigoEmpresa, Repositorio.UnitOfWork unitOfWork);

        bool ReceberEventoMdfe(out string mensagemErro, Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, RetornoEventoCTe retornoEventoMDFe, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork);

        bool CancelarMdfe(int codigoMDFe, int codigoEmpresa, string justificativa, Repositorio.UnitOfWork unitOfWork = null, DateTime? dataCancelamento = null, string cobrarCancelamento = "");

        bool EncerrarMdfe(int codigoMDFe, int codigoEmpresa, DateTime dataEncerramento, Repositorio.UnitOfWork unitOfWork, DateTime? dataEvento = null);

        bool EncerrarMdfeEmissorExterno(Dominio.ObjetosDeValor.MDFe.MDFeEmissorExterno mdfeEmissorExterno, Repositorio.UnitOfWork unitOfWork);

        bool IncluirMotorista(int codigoMDFe, int codigoEmpresa, string cpfMotorista, string nomeMotorista, Repositorio.UnitOfWork unitOfWork, DateTime? dataEvento = null);

        bool IncluirMotorista(int codigoMDFe, int codigoEmpresa, string cpfMotorista, string nomeMotorista, string stringConexao, DateTime? dataEvento = null);

        bool SolicitarEmissaoContingencia(int codigoMDFe, Repositorio.UnitOfWork unitOfWork);

        RetornoStatusMDFePorChave ConsultarMdfeEmissorExterno(string chave, Repositorio.UnitOfWork unitOfWork);
        
        bool ConsultarMdfe(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork);

        Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais ConsultarEventoCancelamento(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork);

        Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais ConsultarEventoEncerramento(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork);

        Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais ConsultarEventoInclusaoMotorista(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork);

        byte[] ObterXMLMdfeAutorizacao(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, int codigoEmpresa, Repositorio.UnitOfWork unitOfWork);

        byte[] ObterXMLMdfeCancelamento(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, int codigoEmpresa, Repositorio.UnitOfWork unitOfWork);
    }
}