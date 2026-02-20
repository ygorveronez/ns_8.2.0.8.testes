using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Auditoria;
using Dominio.ObjetosDeValor.Embarcador.EmissorDocumento;
using System;

namespace Servicos.Embarcador.EmissorDocumento.Emissores
{
    public class OracleEmissorDocumentoCTe : IEmissorDocumentoCTe
    {
        public bool EmitirCte(ref Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork, string statusPosEmissao = "E", string wsOracle = "")
        {
            Servicos.CTe serCTe = new Servicos.CTe(unitOfWork);
            return serCTe.EmitirCte(ref cte, empresa, unitOfWork, statusPosEmissao, wsOracle);
        }

        public bool ReceberEventoCte(out string mensagemErro, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, RetornoEventoCTe retornoEventoCTe, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            mensagemErro = string.Empty;

            if (retornoEventoCTe.cteOracle == null)
                return true;

            Servicos.CTe serCTe = new Servicos.CTe(unitOfWork);
            serCTe.ProcessarRetornoCTeAutorizado(retornoEventoCTe.cteOracle, cte, Auditado, tipoServicoMultisoftware, unitOfWork);

            return true;
        }

        public bool ConsultarCte(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, string wsOracle = "")
        {
            Servicos.CTe serCTe = new Servicos.CTe(unitOfWork);
            return serCTe.SincronizarDocumentoEmProcessamento(cte, Auditado, tipoServicoMultisoftware, unitOfWork);
        }

        public bool CancelarCte(int codigoCTe, int codigoEmpresa, string justificativa, Repositorio.UnitOfWork unitOfWork, DateTime? dataCancelamento = null, bool gerarLog = false, Dominio.Entidades.Usuario usuario = null, string cobrarCancelamento = "")
        {
            Servicos.CTe serCTe = new Servicos.CTe(unitOfWork);
            return serCTe.Cancelar(codigoCTe, codigoEmpresa, justificativa, unitOfWork, dataCancelamento, gerarLog, usuario, cobrarCancelamento);
        }

        public bool EmitirCCe(Dominio.Entidades.CartaDeCorrecaoEletronica cce, int codigoEmpresa, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.CCe svcCCe = new Servicos.CCe(unitOfWork);
            return svcCCe.Emitir(cce, codigoEmpresa, unitOfWork);
        }

        public bool ReceberEventoCCe(out string mensagemErro, out Exception exception, Dominio.ObjetosDeValor.WebService.CTe.CTeOracle cceOracle, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Servicos.CCe svcCCe = new Servicos.CCe(unidadeDeTrabalho);
            Dominio.Entidades.CartaDeCorrecaoEletronica cce = null;
            return svcCCe.ReceberEventoCCe(out mensagemErro, out exception, cceOracle, auditado, unidadeDeTrabalho, ref cce);
        }

        public Dominio.Entidades.CartaDeCorrecaoEletronica ConsultarCCe(int codigoCCe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Servicos.CCe svcCCe = new Servicos.CCe(unidadeDeTrabalho);
            return svcCCe.Consultar(codigoCCe, unidadeDeTrabalho);
        }

        public byte[] ObterXMLCCe(Dominio.Entidades.CartaDeCorrecaoEletronica cce, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.CCe svcCCe = new Servicos.CCe(unitOfWork);
            return svcCCe.ObterXML(cce, unitOfWork);
        }

        public byte[] ObterXMLCteAutorizacao(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Auditado Auditado, TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            byte[] retorno = null;
            Servicos.CTe serCTe = new Servicos.CTe(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoSGT = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            try
            {
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoSGT = repConfiguracaoSGT.BuscarConfiguracaoPadrao();
                bool armazenarEmArquivo = configuracaoSGT != null ? configuracaoSGT.ArmazenarXMLCTeEmArquivo : false;
                retorno = serCTe.ObterESalvarXMLAutorizacao(cte, armazenarEmArquivo, null, unitOfWork);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }

            return retorno;
        }

        public byte[] ObterXMLCteCancelamento(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Auditado Auditado, TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            byte[] retorno = null;
            Servicos.CTe serCTe = new Servicos.CTe(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoSGT = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            try
            {
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoSGT = repConfiguracaoSGT.BuscarConfiguracaoPadrao();
                bool armazenarEmArquivo = configuracaoSGT != null ? configuracaoSGT.ArmazenarXMLCTeEmArquivo : false;

                unitOfWork.Start();
                retorno = serCTe.ObterESalvarXMLCancelamento(cte, cte.Empresa.Codigo, armazenarEmArquivo, null, unitOfWork);
                unitOfWork.CommitChanges();
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
            }

            return retorno;
        }

        public bool EnviarEmail(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, string emails, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.CTe serCTe = new Servicos.CTe(unitOfWork);
            return serCTe.EnviarEmail(cte, emails, unitOfWork);
        }
    }
}
