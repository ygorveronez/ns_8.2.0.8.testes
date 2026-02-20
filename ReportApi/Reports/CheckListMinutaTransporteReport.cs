using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ReportApi.Reports;

[UseReportType(ReportType.CheckListMinutaTransporte)]
public class CheckListMinutaTransporteReport : ReportBase
{
    public CheckListMinutaTransporteReport(UnitOfWork unitOfWork, IStorage storage, RelatorioReportService servicoRelatorioReportService) : base(unitOfWork, servicoRelatorioReportService, storage)
    {

    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(_unitOfWork);
        Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
        Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);
        Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto repositorioCargaEntregaProduto = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto(_unitOfWork);
        Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
        Repositorio.Embarcador.Cargas.CargaMotorista repositorioCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(_unitOfWork);

        int codigoCarga = extraData.GetValue<int>("CodigoCarga");

        Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);
        List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargasEntrega = repositorioCargaEntrega.BuscarPorCarga(codigoCarga);
        Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorUnicaCarga(codigoCarga);
        Dominio.Entidades.Usuario motorista = repositorioCargaMotorista.BuscarPrimeiroMotoristaPorCarga(codigoCarga);
        Dominio.Entidades.Cliente filial = repositorioCliente.BuscarPorCPFCNPJ(carga.Filial.CNPJ.ToLong());

        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.CheckListMinutaTransporte> listaCheckListMinutaTransporte = new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.CheckListMinutaTransporte>();

        foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega in cargasEntrega)
        {
            if (!cargaEntrega.Coleta)
                continue;

            List<(string Descricao, decimal Quantidade)> cargasEntregaProdutos = repositorioCargaEntregaProduto.BuscarProdutosPorCargaEntrega(cargaEntrega.Codigo);
            Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.CheckListMinutaTransporte minutaTransporte = new Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.CheckListMinutaTransporte();

            string telefone = (filial?.Telefone1?.ObterTelefoneFormatado() ?? filial?.Telefone2?.ObterTelefoneFormatado()) ?? string.Empty;

            minutaTransporte.Empresa = filial?.Descricao ?? string.Empty;
            minutaTransporte.Endereco = filial?.EnderecoCompletoCidadeeEstado ?? string.Empty;
            minutaTransporte.Telefone = telefone != "0" ? telefone : string.Empty;
            minutaTransporte.DataEmbarque = cargaEntrega.DataPrevista.ToDateString();
            minutaTransporte.DataPrevistaAbate = pedido.DataETA.HasValue ? pedido.DataETA.ToDateString() : string.Empty;
            minutaTransporte.NumeroMinutaEmbarque = $"{carga.CodigoCargaEmbarcador}-{pedido.NumeroPedidoEmbarcador}";

            minutaTransporte.Motorista = motorista?.Nome ?? string.Empty;
            minutaTransporte.CPFCNPJMotorista = motorista?.CPF_CNPJ_Formatado ?? string.Empty;
            minutaTransporte.Placa = carga.DadosSumarizados?.Veiculos ?? string.Empty;
            minutaTransporte.Transportador = carga.Empresa?.NomeFantasia ?? string.Empty;
            minutaTransporte.TipoVeiculo = carga.ModeloVeicularCarga?.Descricao ?? string.Empty;
            minutaTransporte.AnimaisEmbarcados = AgruparAnimaisEmbarcados(cargasEntregaProdutos);
            minutaTransporte.Pecuarista = cargaEntrega.Cliente?.Nome ?? string.Empty;
            minutaTransporte.CPFCNPJProprietario = cargaEntrega.Cliente?.CPF_CNPJ_Formatado ?? string.Empty;
            minutaTransporte.Fazenda = pedido?.Remetente?.EnderecoCompletoCidadeeEstado ?? string.Empty;
            minutaTransporte.Cidade = cargaEntrega.Cliente?.Localidade?.Descricao ?? string.Empty;
            minutaTransporte.UF = cargaEntrega.Cliente?.Localidade?.Estado?.Sigla ?? string.Empty;
            minutaTransporte.Roteiro = pedido?.Observacao ?? string.Empty;
            minutaTransporte.InscricaoEstadual = carga.Empresa?.InscricaoEstadual ?? string.Empty;

            listaCheckListMinutaTransporte.Add(minutaTransporte);
        }

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            DataSet = listaCheckListMinutaTransporte,
            Parameters = ObterParametros(cargasEntrega),
        };

        byte[] pdf = RelatorioSemPadraoReportService.GerarRelatorio(
            @"Areas\Relatorios\Reports\Default\Cargas\CheckListMinutaTransporte.rpt",
            Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);

        if (pdf == null)
            throw new ServicoException("Não foi possível gerar o relatório de minuta.");

        ReportResult result = PrepareReportResult(FileType.PDF, pdf);

        return result;
    }

    private List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargasEntrega)
    {
        Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaAssinaturaResponsavel repositorioCargaEntregaAssinaturaResponsavel = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaAssinaturaResponsavel(_unitOfWork);

        List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

        Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaAssinaturaResponsavel cargaEntregaAssinaturaResponsavel = repositorioCargaEntregaAssinaturaResponsavel.BuscarPorCargaEntrega(cargasEntrega.Where(o => o.Coleta).FirstOrDefault()?.Codigo ?? 0);
        Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaAssinaturaResponsavel cargaEntregaAssinaturaRecebedor = repositorioCargaEntregaAssinaturaResponsavel.BuscarPorCargaEntrega(cargasEntrega.Where(o => !o.Coleta).FirstOrDefault()?.Codigo ?? 0);
        Dominio.ObjetosDeValor.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(_unitOfWork)?.ObterConfiguracaoArquivo();

        string caminhoAssinatura = Utilidades.Directory.CriarCaminhoArquivos(new string[] { configuracaoArquivo.CaminhoArquivos, "Anexos", "CargaColetaEntrega", "Assinatura" });
        string caminhoPadrao = Utilidades.IO.FileStorageService.Storage.Combine(configuracaoArquivo.CaminhoLogoEmbarcador, "crystal.png");
        string caminhoLogo = string.Empty;

        if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoPadrao))
            caminhoLogo = caminhoPadrao;

        Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro assinaturaResponsavel = new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();

        assinaturaResponsavel.NomeParametro = "CaminhoAssinaturaResponsavel";
        assinaturaResponsavel.ValorParametro = cargaEntregaAssinaturaResponsavel != null
            ? Utilidades.IO.FileStorageService.Storage.Combine(caminhoAssinatura,
                cargaEntregaAssinaturaResponsavel.GuidArquivo + "-miniatura" +
                Path.GetExtension(cargaEntregaAssinaturaResponsavel.NomeArquivo))
            : string.Empty;

        parametros.Add(assinaturaResponsavel);

        Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro assinaturaRecebedor = new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro();
        assinaturaRecebedor.NomeParametro = "CaminhoAssinaturaRecebedor";
        assinaturaRecebedor.ValorParametro = cargaEntregaAssinaturaRecebedor != null
            ? Utilidades.IO.FileStorageService.Storage.Combine(caminhoAssinatura,
                cargaEntregaAssinaturaRecebedor.GuidArquivo + "-miniatura" +
                Path.GetExtension(cargaEntregaAssinaturaRecebedor.NomeArquivo))
            : caminhoLogo;

        parametros.Add(assinaturaRecebedor);

        return parametros;
    }

    private string AgruparAnimaisEmbarcados(IEnumerable<(string Descricao, decimal Quantidade)> itens)
    {
        var animaisAgrupados = itens
            .GroupBy(item => (item.Descricao ?? string.Empty).Trim())
            .Select(grupo => new
            {
                Descricao = grupo.Key,
                Quantidade = grupo.Sum(item => (int)item.Quantidade)
            })
            .ToList();

        int totalAnimais = animaisAgrupados.Sum(a => a.Quantidade);

        string resultado = string.Join(", ", animaisAgrupados.Select(a => $"{a.Descricao} - {a.Quantidade}"));
        resultado += $", Total: {totalAnimais}";

        return resultado;
    }
}
