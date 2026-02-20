using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;

namespace Servicos.Embarcador.Frotas
{
    public class MovimentacaoDePlacas
    {
        private readonly Repositorio.UnitOfWork _unitOfWork;

        public MovimentacaoDePlacas(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public byte[] RelatorioMovimentacaoDePlacas(Dominio.Entidades.Veiculo veiculo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware, int TipoArquivo )
        {
            return ReportRequest.WithType(ReportType.MovimentacaoDePlacas)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("CodigoVeiculo", veiculo.Codigo)
                .AddExtraData("TipoArquivo", TipoArquivo)
                .CallReport()
                .GetContentFile();
        }
    }
}
