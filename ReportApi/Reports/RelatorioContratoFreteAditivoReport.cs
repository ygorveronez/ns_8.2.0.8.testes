using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;

namespace ReportApi.Reports;


[UseReportType(ReportType.RelatorioContratoFreteAditivo)]
public class RelatorioContratoFreteAditivoReport : ReportBase
{
    public RelatorioContratoFreteAditivoReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        Repositorio.Embarcador.Frete.ContratoFreteTransportador repContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(_unitOfWork);

        var numeroAditivo = extraData.GetValue<int>("NumeroAditivo");
        var contrato = repContratoFreteTransportador.BuscarPorCodigo(extraData.GetValue<int>("CodigoContrato"));
        
         Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            Dominio.Entidades.Cliente clienteContratoAditivo = configuracao.ClienteContratoAditivo;

            if (clienteContratoAditivo == null) return null;

            string ordinalAditivo = Utilidades.Conversor.Ordinal(numeroAditivo).ToUpper();

            int prazo = ((contrato.DataFinal - contrato.DataInicial).Days / 30);
            string prazoDescricao = prazo.ToString() + " " + (prazo == 1 ? "Mês" : "Meses");
            DateTime? dataAssinatura = ObterContratoOrigemAPartirDoAditivo(contrato, _unitOfWork);

            Dominio.Relatorios.Embarcador.DataSource.Fretes.AditivoContratoFreteTransportador DScontrato = new Dominio.Relatorios.Embarcador.DataSource.Fretes.AditivoContratoFreteTransportador
            {
                OrdinalAditivo = ordinalAditivo,

                ContratanteRazaoSocial = clienteContratoAditivo.Nome,
                ContratanteCPFCNPJ = clienteContratoAditivo.CPF_CNPJ_Formatado,
                ContratanteNaturezaJuridica = "Pessoa Jurídica",
                ContratanteEndereco = clienteContratoAditivo.Endereco,
                ContratanteRepresentantesLegais = "",
                ContratanteCidade = clienteContratoAditivo.Localidade.Descricao,
                ContratanteEstado = clienteContratoAditivo.Localidade.Estado.Sigla,
                ContratanteCEP = clienteContratoAditivo.CEP,
                ContratantePais = clienteContratoAditivo.Localidade.Pais.Nome,

                DataAssinatura = dataAssinatura?.ToString("dd/MM/yyyy") ?? string.Empty,
                LocalAssinatura = clienteContratoAditivo.Localidade.Descricao,

                DataContrato = contrato.DataFinal,
                PrazoContrato = prazoDescricao,
            };

            if (contrato.Transportador != null)
            {
                DScontrato.ContratadoRazaoSocial = contrato.Transportador.RazaoSocial;
                DScontrato.ContratadoCPFCNPJ = contrato.Transportador.CNPJ_Formatado;
                DScontrato.ContratadoNaturezaJuridica = "Pessoa Jurídica";
                DScontrato.ContratadoEndereco = contrato.Transportador.Endereco;
                DScontrato.ContratadoRepresentantesLegais = "";
                DScontrato.ContratadoCidade = contrato.Transportador.Localidade.Descricao;
                DScontrato.ContratadoEstado = contrato.Transportador.Localidade.Estado.Sigla;
                DScontrato.ContratadoCEP = contrato.Transportador.CEP;
                DScontrato.ContratadoPais = contrato.Transportador.Localidade.Pais.Nome;
            }

            Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = new List<Dominio.Relatorios.Embarcador.DataSource.Fretes.AditivoContratoFreteTransportador>() { DScontrato },
            };

            // Gera pdf
            var pdfContent =  RelatorioSemPadraoReportService.GerarRelatorio(@"Areas\Relatorios\Reports\Default\Fretes\AditivoContratoFreteTransportador.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);
            return PrepareReportResult(FileType.PDF, pdfContent);
    }
    
    private static DateTime? ObterContratoOrigemAPartirDoAditivo(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato, Repositorio.UnitOfWork unitOfWork)
    {
        if (contrato.Transportador == null)
            return null;

        Repositorio.Embarcador.Frete.TipoContratoFrete repTipoContratoFrete = new Repositorio.Embarcador.Frete.TipoContratoFrete(unitOfWork);
        Dominio.Entidades.Embarcador.Frete.TipoContratoFrete tipoContrato = repTipoContratoFrete.BuscarPorCodigoAditivo(contrato.TipoContratoFrete.Codigo);

        if (tipoContrato == null)
            return null;

        Repositorio.Embarcador.Frete.ContratoFreteTransportador repContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork);

        return repContratoFreteTransportador.BuscarPorEmpresaETipoContrato(contrato.Transportador.Codigo, tipoContrato.Codigo)?.DataInicial;
    }

}