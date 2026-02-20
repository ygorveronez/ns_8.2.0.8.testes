using Dominio.ObjetosDeValor.Relatorios;
using Newtonsoft.Json;
using Servicos.Extensions;
using Utilidades.Extensions;

namespace Servicos.Embarcador.Pedido
{
    public sealed class ImpressaoPedido
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public ImpressaoPedido(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public bool GerarRelatorioTMS(bool planejamentoPedido, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, bool carregamento, out string msg, bool impressaoCarga, Dominio.Entidades.Embarcador.Cargas.Carga carga, int codigo, bool ordemColeta, bool planoViagem, string stringConexaoPar, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string nomeFantasia, Dominio.Entidades.Usuario usuario, bool gerarPorThread, out string guidRelatorio, out string fileName)
        {

            var report = ReportRequest.WithType(ReportType.ImpressaoPedido)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("planejamentoPedido", planejamentoPedido)
                .AddExtraData("codigoPedido", pedido?.Codigo ?? 0)
                .AddExtraData("carregamento", carregamento)
                .AddExtraData("impressaoCarga", impressaoCarga)
                .AddExtraData("codigoCarga", carga?.Codigo ?? 0)
                .AddExtraData("codigo", codigo)
                .AddExtraData("ordemColeta", ordemColeta)
                .AddExtraData("planoViagem", planoViagem)
                .AddExtraData("stringConexaoPar", stringConexaoPar)
                .AddExtraData("nomeFantasia", nomeFantasia)
                .AddExtraData("usuario", new {Codigo = usuario?.Codigo ?? 0,CPF = usuario?.CPF ?? string.Empty})
                .AddExtraData("gerarPorThread", gerarPorThread)
                .AddExtraData("CodigoUsuario", usuario?.Codigo ?? 0)
                .CallReport();

            msg = report.ErrorMessage;
            guidRelatorio = report.Id.ToString();
            fileName = report.FileName;
            if (string.IsNullOrEmpty(report.ErrorMessage))
                return true;
            else
                return false;

        }

        #endregion

    }
}
