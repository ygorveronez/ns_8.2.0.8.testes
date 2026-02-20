using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using AdminMultisoftware.Dominio.Enumeradores;

namespace ReportApi.Reports;
[UseReportType(ReportType.CargaComposicaoFrete)]
public class CargaComposicaoFreteReport : ReportBase
{
    public CargaComposicaoFreteReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {

    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        int cargaCodigo = extraData.GetValue<int>("codigoCarga");
        bool filialEmissora = extraData.GetValue<bool>("filialEmissora");

        Repositorio.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete repCargaComposicaoFrete = new Repositorio.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete(_unitOfWork);
        Repositorio.Embarcador.Cargas.Carga repCarga= new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
        AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware = extraData.GetInfo().TipoServico;

        List<Dominio.Entidades.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete> cargaComposicaoFretes = repCargaComposicaoFrete.BuscarPorCarga(cargaCodigo, filialEmissora);
        Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(cargaCodigo);

        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Frete.CargaComposicaoFrete> composicoesFrete = new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Frete.CargaComposicaoFrete>()
                {
                    new Dominio.Relatorios.Embarcador.DataSource.Cargas.Frete.CargaComposicaoFrete()
                    {
                         Destinatario = carga.DadosSumarizados.Destinatarios,
                         Remetente = carga.DadosSumarizados.Remetentes,
                         NumeroCarga = carga.CodigoCargaEmbarcador
                    }
                };

        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Frete.CargaComposicaoFreteItem> itensComposicao = ObterItensComposicaoFrete(carga, cargaComposicaoFretes, filialEmissora, tipoServicoMultisoftware, _unitOfWork);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            DataSet = composicoesFrete,
            SubReports = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>()
                    {
                        new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                        {
                             Key = "CargaComposicaoFreteItem",
                             DataSet = itensComposicao
                        }
                    }
        };


        byte[] pdfContent = RelatorioSemPadraoReportService.GerarRelatorio(@"Areas\Relatorios\Reports\Default\Cargas\CargaComposicaoFrete.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);
        return PrepareReportResult(FileType.PDF, pdfContent);
    }

    private List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Frete.CargaComposicaoFreteItem> ObterItensComposicaoFrete(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete> cargaComposicaoFretes, bool filialEmissora, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
    {
        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Frete.CargaComposicaoFreteItem> itensComposicaoFrete = new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Frete.CargaComposicaoFreteItem>();

        itensComposicaoFrete.AddRange(ObterItensComposicaoFretePedido(carga, cargaComposicaoFretes, filialEmissora, tipoServicoMultisoftware, unitOfWork));
        itensComposicaoFrete.AddRange(ObterItensComposicaoFreteNotaFiscal(cargaComposicaoFretes, tipoServicoMultisoftware));
        itensComposicaoFrete.AddRange(ObterItensComposicaoFreteCteParaSubcontratacao(cargaComposicaoFretes, tipoServicoMultisoftware));
        itensComposicaoFrete.AddRange(ObterItensComposicaoFreteCarga(carga, cargaComposicaoFretes, filialEmissora, tipoServicoMultisoftware, unitOfWork));

        return itensComposicaoFrete;
    }

    private List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Frete.CargaComposicaoFreteItem> ObterItensComposicaoFreteCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete> cargaComposicaoFretes, bool filialEmissora, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
    {
        Repositorio.Embarcador.Cargas.CargaTabelaFreteCliente repCargaTabelaFreteCliente = new Repositorio.Embarcador.Cargas.CargaTabelaFreteCliente(unitOfWork);

        Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente = repCargaTabelaFreteCliente.BuscarPorCarga(carga.Codigo, filialEmissora)?.TabelaFreteCliente ?? null;

        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Frete.CargaComposicaoFreteItem> itensComposicaoFrete = (
            from composicao in cargaComposicaoFretes
            where composicao.PedidoCTesParaSubContratacao?.Count == 0 &&
                  composicao.PedidoXMLNotasFiscais?.Count == 0 &&
                  composicao.CargaPedidos?.Count == 0
            select new Dominio.Relatorios.Embarcador.DataSource.Cargas.Frete.CargaComposicaoFreteItem()
            {
                Descricao = composicao.Descricao,
                DescricaoAgrupamento = $"Carga: {composicao.Carga?.CodigoCargaEmbarcador}",
                Placa = $"Placa(s): {composicao.Carga?.PlacasVeiculos}",
                Transportador = $"{(tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? "Empresa/Filial:" : "Transportador")}: {composicao.Carga?.Empresa?.Descricao}",
                Motorista = $"Motorista(s): {composicao.Carga?.NomeMotoristas}",
                Formula = composicao.Formula,
                ValoresFormula = composicao.ValoresFormula,
                TipoCampoValor = composicao.TipoCampoValor.ObterDescricao(),
                Valor = composicao.Valor,
                ValorCalculado = composicao.ValorCalculado,
                TipoParametro = composicao.TipoParametro.ObterDescricao(),
                CodigoTabela = tabelaFreteCliente?.CodigoIntegracao ?? string.Empty,
                Origem = tabelaFreteCliente?.DescricaoOrigem ?? string.Empty,
                Destino = tabelaFreteCliente?.DescricaoDestino ?? string.Empty
            }
        ).ToList();

        return itensComposicaoFrete;
    }

    private List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Frete.CargaComposicaoFreteItem> ObterItensComposicaoFreteCteParaSubcontratacao(List<Dominio.Entidades.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete> cargaComposicaoFretes, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
    {
        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Frete.CargaComposicaoFreteItem> itensComposicaoFrete = (
            from composicao in cargaComposicaoFretes
            where composicao.PedidoCTesParaSubContratacao?.Count > 0
            group composicao by new { Numero = string.Join(", ", composicao.PedidoCTesParaSubContratacao.Select(o => $"{o.CTeTerceiro.Numero}-{o.CTeTerceiro.Serie}")), Codigos = string.Join(", ", composicao.PedidoCTesParaSubContratacao.Select(o => o.Codigo)) }
            into grupo
            select grupo.Select(composicaoAgrupada => new Dominio.Relatorios.Embarcador.DataSource.Cargas.Frete.CargaComposicaoFreteItem()
            {
                Agrupamento = $"{grupo.Key.Numero}-{grupo.Key.Codigos}",
                Descricao = composicaoAgrupada.Descricao,
                DescricaoAgrupamento = $"CT-e para Subcontratação: {grupo.Key.Numero}",
                Placa = $"Placa(s): {grupo.FirstOrDefault()?.Carga?.PlacasVeiculos}",
                Transportador = $"{(tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? "Empresa/Filial:" : "Transportador")}: {grupo.FirstOrDefault()?.Carga?.Empresa?.Descricao}",
                Motorista = $"Motorista(s): {grupo.FirstOrDefault()?.Carga?.NomeMotoristas}",
                Formula = composicaoAgrupada.Formula,
                ValoresFormula = composicaoAgrupada.ValoresFormula,
                TipoCampoValor = composicaoAgrupada.TipoCampoValor.ObterDescricao(),
                Valor = composicaoAgrupada.Valor,
                ValorCalculado = composicaoAgrupada.ValorCalculado,
                TipoParametro = composicaoAgrupada.TipoParametro.ObterDescricao()
            })
        ).SelectMany(composicoesAgrupadas => composicoesAgrupadas).ToList();

        return itensComposicaoFrete;
    }

    private List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Frete.CargaComposicaoFreteItem> ObterItensComposicaoFreteNotaFiscal(List<Dominio.Entidades.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete> cargaComposicaoFretes, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
    {
        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Frete.CargaComposicaoFreteItem> itensComposicaoFrete = (
            from composicao in cargaComposicaoFretes
            where composicao.PedidoXMLNotasFiscais?.Count > 0
            group composicao by new { Numero = string.Join(", ", composicao.PedidoXMLNotasFiscais.Select(o => o.XMLNotaFiscal.Numero)), Codigos = string.Join(", ", composicao.PedidoXMLNotasFiscais.Select(o => o.Codigo)) }
            into grupo
            select grupo.Select(composicaoAgrupada => new Dominio.Relatorios.Embarcador.DataSource.Cargas.Frete.CargaComposicaoFreteItem()
            {
                Agrupamento = $"{grupo.Key.Numero}-{grupo.Key.Codigos}",
                Descricao = composicaoAgrupada.Descricao,
                DescricaoAgrupamento = $"Nota Fiscal: {grupo.Key.Numero}",
                Placa = $"Placa(s): {grupo.FirstOrDefault()?.Carga?.PlacasVeiculos}",
                Transportador = $"{(tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? "Empresa/Filial:" : "Transportador")}: {grupo.FirstOrDefault()?.Carga?.Empresa?.Descricao}",
                Motorista = $"Motorista(s): {grupo.FirstOrDefault()?.Carga?.NomeMotoristas}",
                Formula = composicaoAgrupada.Formula,
                ValoresFormula = composicaoAgrupada.ValoresFormula,
                TipoCampoValor = composicaoAgrupada.TipoCampoValor.ObterDescricao(),
                Valor = composicaoAgrupada.Valor,
                ValorCalculado = composicaoAgrupada.ValorCalculado,
                TipoParametro = composicaoAgrupada.TipoParametro.ObterDescricao()
            })
        ).SelectMany(composicoesAgrupadas => composicoesAgrupadas).ToList();

        return itensComposicaoFrete;
    }

    private List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Frete.CargaComposicaoFreteItem> ObterItensComposicaoFretePedido(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete> cargaComposicaoFretes, bool filialEmissora, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
    {
        Repositorio.Embarcador.Cargas.CargaPedidoTabelaFreteCliente repCargaPedidoTabelaFreteCliente = new Repositorio.Embarcador.Cargas.CargaPedidoTabelaFreteCliente(unitOfWork);

        List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoTabelaFreteCliente> cargaPedidoTabelaFreteCliente = repCargaPedidoTabelaFreteCliente.BuscarPorCarga(carga.Codigo, filialEmissora);

        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Frete.CargaComposicaoFreteItem> itensComposicaoFrete = (
            from composicao in cargaComposicaoFretes
            where composicao.CargaPedidos?.Count > 0
            group composicao by new { Numero = string.Join(", ", composicao.CargaPedidos.Select(cargaPedido => tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? $"{cargaPedido.Pedido.Numero} - {cargaPedido.Pedido.NumeroPedidoEmbarcador}" : cargaPedido.Pedido.NumeroPedidoEmbarcador)), Codigo = composicao.CargaPedidos.FirstOrDefault().Codigo }
            into grupo
            select grupo.Select(composicaoAgrupada => new Dominio.Relatorios.Embarcador.DataSource.Cargas.Frete.CargaComposicaoFreteItem()
            {
                Agrupamento = $"{grupo.Key.Numero}-{grupo.Key.Codigo}",
                Descricao = composicaoAgrupada.Descricao,
                Cliente = $"Cliente: {grupo.FirstOrDefault()?.CargaPedidos?.FirstOrDefault()?.ObterDestinatario()?.Descricao ?? ""}",
                DescricaoAgrupamento = $"Pedido(s): {grupo.Key.Numero}",
                Placa = $"Placa(s): {grupo.FirstOrDefault()?.Carga?.PlacasVeiculos}",
                Transportador = $"{(tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? "Empresa/Filial:" : "Transportador")}: {grupo.FirstOrDefault()?.Carga?.Empresa?.Descricao}",
                Motorista = $"Motorista(s): {grupo.FirstOrDefault()?.Carga?.NomeMotoristas}",
                Formula = composicaoAgrupada.Formula,
                ValoresFormula = composicaoAgrupada.ValoresFormula,
                TipoCampoValor = composicaoAgrupada.TipoCampoValor.ObterDescricao(),
                Valor = composicaoAgrupada.Valor,
                ValorCalculado = composicaoAgrupada.ValorCalculado,
                TipoParametro = composicaoAgrupada.TipoParametro.ObterDescricao(),
                CodigoTabela = cargaPedidoTabelaFreteCliente.Where(c => c.CargaPedido.Codigo == grupo.Key.Codigo)?.FirstOrDefault()?.TabelaFreteCliente?.CodigoIntegracao ?? string.Empty,
                Origem = cargaPedidoTabelaFreteCliente.Where(c => c.CargaPedido.Codigo == grupo.Key.Codigo)?.FirstOrDefault()?.TabelaFreteCliente?.DescricaoOrigem ?? string.Empty,
                Destino = cargaPedidoTabelaFreteCliente.Where(c => c.CargaPedido.Codigo == grupo.Key.Codigo)?.FirstOrDefault()?.CargaPedido?.Destino?.DescricaoCidadeEstado ?? string.Empty,

            })
        ).SelectMany(composicoesAgrupadas => composicoesAgrupadas).ToList();

        return itensComposicaoFrete;
    }

}