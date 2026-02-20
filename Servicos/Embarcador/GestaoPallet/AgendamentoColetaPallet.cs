using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;

namespace Servicos.Embarcador.GestaoPallet
{
    public class AgendamentoColetaPallet
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public AgendamentoColetaPallet(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public byte[] ResumoAgendamentoColetaPallet(Dominio.Entidades.Embarcador.GestaoPallet.AgendamentoColetaPallet agendamento)
        {
            return ReportRequest.WithType(ReportType.ResumoAgendamentoColetaPallet)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("CodigoAgendamento", agendamento.Codigo.ToString())
                .CallReport()
                .GetContentFile();
        }

        #endregion Métodos Públicos        
    }
}
