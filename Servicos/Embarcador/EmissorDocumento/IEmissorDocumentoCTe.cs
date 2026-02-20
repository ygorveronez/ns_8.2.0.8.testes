using Dominio.ObjetosDeValor.Embarcador.EmissorDocumento;
using System;

namespace Servicos.Embarcador.EmissorDocumento
{
    public interface IEmissorDocumentoCTe
    {
        bool EmitirCte(ref Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork, string statusPosEmissao = "E", string wsOracle = "");

        bool ReceberEventoCte(out string mensagemErro, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, RetornoEventoCTe retornoEventoCTe, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork);

        bool ConsultarCte(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, string wsOracle = "");

        bool CancelarCte(int codigoCTe, int codigoEmpresa, string justificativa, Repositorio.UnitOfWork unitOfWork, DateTime? dataCancelamento = null, bool gerarLog = false, Dominio.Entidades.Usuario usuario = null, string cobrarCancelamento = "");

        bool EmitirCCe(Dominio.Entidades.CartaDeCorrecaoEletronica cce, int codigoEmpresa, Repositorio.UnitOfWork unidadeDeTrabalho);

        bool ReceberEventoCCe(out string mensagemErro, out Exception exception, Dominio.ObjetosDeValor.WebService.CTe.CTeOracle cceOracle, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unidadeDeTrabalho);

        Dominio.Entidades.CartaDeCorrecaoEletronica ConsultarCCe(int codigoCCe, Repositorio.UnitOfWork unidadeDeTrabalho);

        byte[] ObterXMLCCe(Dominio.Entidades.CartaDeCorrecaoEletronica cce, Repositorio.UnitOfWork unitOfWork);

        byte[] ObterXMLCteAutorizacao(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork);

        byte[] ObterXMLCteCancelamento(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork);

        bool EnviarEmail(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, string emails, Repositorio.UnitOfWork unitOfWork);
    }
}