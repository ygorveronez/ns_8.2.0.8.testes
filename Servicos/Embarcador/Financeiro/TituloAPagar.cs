using Dominio.Interfaces.Database;
using System.Threading;

namespace Servicos.Embarcador.Financeiro
{
    public class TituloAPagar : ServicoBase
    {        
        public TituloAPagar(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }
        #region Métodos Públicos

        public bool AtualizarTitulos(object documento, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, out string erro, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataCompetenciaDocumentoEntrada dataCompetencia, bool lancarDocumentoEntradaAbertoSeKMEstiverErrado = false)
        {
            if (documento == null)
            {
                erro = "Não é possível gerar os títulos a pagar pois o documento é nulo!";
                return false;
            }

            if (documento.GetType() == typeof(Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS) || documento.GetType().BaseType == typeof(Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS))
            {
                Servicos.Embarcador.Financeiro.DocumentoEntrada svcDocumentoEntrada = new DocumentoEntrada(unidadeTrabalho);
                return svcDocumentoEntrada.AtualizarTitulosAPagar((Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS)documento, unidadeTrabalho, tipoServicoMultisoftware, out erro, tipoAmbiente, Auditado, dataCompetencia, lancarDocumentoEntradaAbertoSeKMEstiverErrado);
            }

            if (documento.GetType() == typeof(Dominio.Entidades.Embarcador.Terceiros.ContratoFrete) || documento.GetType().BaseType == typeof(Dominio.Entidades.Embarcador.Terceiros.ContratoFrete))
            {
                Servicos.Embarcador.Financeiro.DocumentoEntrada svcDocumentoEntrada = new DocumentoEntrada(unidadeTrabalho);
                return svcDocumentoEntrada.AtualizarTitulosAPagar((Dominio.Entidades.Embarcador.Terceiros.ContratoFrete)documento, unidadeTrabalho, tipoServicoMultisoftware, out erro, tipoAmbiente, Auditado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataCompetenciaDocumentoEntrada.DataEntrada);
            }

            erro = "Geração de títulos não implementada para o documento informado.";
            return false;
        }

        public bool AtualizarGuias(object documento, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, out string erro, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataCompetenciaDocumentoEntrada dataCompetencia)
        {
            erro = string.Empty;
            if (documento == null)
            {
                erro = "Não é possível gerar os títulos a pagar pois o documento é nulo!";
                return false;
            }

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                return true;

            Servicos.Embarcador.Financeiro.DocumentoEntrada svcDocumentoEntrada = new DocumentoEntrada(unidadeTrabalho);
            return svcDocumentoEntrada.AtualizarGuiasAPagar((Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS)documento, unidadeTrabalho, tipoServicoMultisoftware, out erro, tipoAmbiente, Auditado, dataCompetencia);

        }

        #endregion
    }
}
