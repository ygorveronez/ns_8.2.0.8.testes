using SGT.BackgroundWorkers.Utils;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 300000)]

    public class ValidarIrregularidadesControleDocumento : LongRunningProcessBase<ValidarIrregularidadesControleDocumento>
    {
        #region Métodos protegidos

        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            Servicos.Embarcador.Documentos.ControleDocumento serControleDoc = new Servicos.Embarcador.Documentos.ControleDocumento(unitOfWork);
            serControleDoc.GeracaoThreadControleDocumentoPorCte();
            serControleDoc.ProcessarDocumentosPendentesVerificacao();
            new Servicos.Embarcador.Integracao.Unilever.DocumentoDestinado(unitOfWork).VincularCTeComPreCte();
            new Servicos.Embarcador.Email.EnvioEmailDocumentacao(unitOfWork, _tipoServicoMultisoftware, _auditado, null).EnviarEmailIrregularidades();

        }

        public override bool CanRun()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_stringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
            {
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento configuracao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento(unitOfWork).BuscarConfiguracaoPadrao();
                return (configuracao?.CriarControleDeEmissaoDeDocumentos ?? false) && _tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador;
            }
        }
        #endregion
    }
}