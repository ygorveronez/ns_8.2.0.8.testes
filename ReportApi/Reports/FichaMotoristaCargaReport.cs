using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;
using Utilidades.Extensions;

namespace ReportApi.Reports;

[UseReportType(ReportType.FichaMotoristaCarga)]
public class FichaMotoristaCargaReport : ReportBase
{
    public FichaMotoristaCargaReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
        Repositorio.Embarcador.Cargas.CargaMotorista repCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(_unitOfWork);
        Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
        Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);

        var carga = repCarga.BuscarPorCodigo(extraData.GetValue<int>("CodigoCarga"));
        var motorista = repCargaMotorista.BuscarPrimeiroMotoristaPorCarga(carga.Codigo);
        var veiculoTracao = repVeiculo.BuscarPorCodigo(extraData.GetValue<int>("CodigoVeiucloTracao"), false);
        List<int> codigosReboques = extraData.GetValue<string>("CodigosReboques").FromJson<List<int>>();

        Dominio.Entidades.Empresa matriz = repEmpresa.BuscarEmpresaMatriz();
        Dominio.Entidades.Embarcador.Usuarios.FuncionarioContato pai =
            motorista.Contatos?.Where(o => o.TipoParentesco == TipoParentesco.Pai)?.FirstOrDefault() ?? null;
        Dominio.Entidades.Embarcador.Usuarios.FuncionarioContato mae =
            motorista.Contatos?.Where(o => o.TipoParentesco == TipoParentesco.Mae)?.FirstOrDefault() ?? null;

        Dominio.Relatorios.Embarcador.DataSource.Cargas.FichaMotorista.FichaMotorista dsFichaMotorista =
            new Dominio.Relatorios.Embarcador.DataSource.Cargas.FichaMotorista.FichaMotorista()
            {
                Data = DateTime.Now.ToString("dd/MM/yyyy"),

                NomeMatriz = matriz?.RazaoSocial ?? "",
                EnderecoMatriz = matriz?.Endereco ?? "",
                LocalidadeMatriz = matriz?.LocalidadeUF ?? "",

                NomeEmpresa = carga.Empresa?.RazaoSocial ?? "",
                PrimeiroTelefone = carga.Empresa?.Telefone ?? "",
                PrimeiroEmail = carga.Empresa?.Email ?? "",

                NomeMotorista = motorista.Nome ?? "",
                RGMotorista = motorista.RG ?? "",
                NrCNHRegistroMotorista = motorista.NumeroHabilitacao ?? "",
                CategoriaCNHMotorista = motorista.Categoria ?? "",
                NrCNHDocumentoMotorista = motorista.RenachHabilitacao ?? "",
                NomePaiMotorista = pai?.Nome ?? string.Empty,
                CidadeNascimentoMotorista = motorista.LocalidadeNascimento?.Descricao ?? "",
                CidadeMotorista = motorista.Localidade?.Descricao ?? "",
                EnderecoMotorista = motorista.Endereco ?? "",
                DataEmissaoRGMotorista = motorista.DataEmissaoRG?.ToDateString() ?? "",
                DataValidadeRGMotorista = motorista.DataVencimentoHabilitacao?.ToDateString() ?? "",
                DataPrimeiraHabilitacaoMotorista = motorista.DataPrimeiraHabilitacao?.ToDateString() ?? "",
                NomeMaeMotorista = mae?.Nome ?? string.Empty,
                EstadoNascimentoMotorista = motorista.LocalidadeNascimento?.Estado?.Descricao ?? "",
                BairroMotorista = motorista.Bairro ?? "",
                TelefoneMotorista = motorista.Telefone ?? "",
                CPFMotorista = motorista.CPF_CNPJ_Formatado ?? "",
                DataNascimentoMotorista = motorista.DataNascimento?.ToDateString() ?? "",
                NrPISMotorista = motorista.PIS ?? "",
                CidadaEmissaoPISMotorista = "",
                EstadoCivilMotorista = motorista.EstadoCivil?.ObterDescricao() ?? string.Empty,
                CEPMotorista = motorista.CEP ?? "",
                CodigoIntegracaoMotorista = motorista.CodigoIntegracao ?? string.Empty,

                PlacaVeiculo = veiculoTracao.Placa ?? "",
                TaraVeiculo = veiculoTracao.Tara.ToString() ?? "",
                MarcaModeloVeiculo = veiculoTracao.DescricaoComMarcaModelo ?? "",
                TipoVeiculo = veiculoTracao.DescricaoTipo ?? "",
                NrRenavamVeiculo = veiculoTracao.Renavam ?? "",
                ComprimentoVeiculo = "",
                NrChassisVeiculo = veiculoTracao.Chassi ?? "",
                LarguraVeiculo = "",
                EspecieTipoVeiculo = veiculoTracao.DescricaoTipoVeiculo ?? "",
                CidadeEstadoVeiculo = veiculoTracao.LocalidadeAtual?.DescricaoCidadeEstado ?? "",
                RNTRCANTTVeiculo = veiculoTracao.RNTRC.ToString() ?? "",
                NrEixosVeiculo = "",
                CorVeiculo = veiculoTracao?.CorVeiculo?.Descricao ?? "",
                AnoFabModeloVeiculo =
                    veiculoTracao.AnoFabricacao.ToString() + "-" + veiculoTracao.DescricaoModelo ?? "",

                NomeProprietarioVeiculo = veiculoTracao.Proprietario?.Nome ?? "",
                EnderecoProprietarioVeiculo = veiculoTracao.Proprietario?.Endereco ?? "",
                BairroProprietarioVeiculo = veiculoTracao.Proprietario?.Bairro ?? "",
                CPFCNPJProprietarioVeiculo = veiculoTracao.Proprietario?.CPF_CNPJ_Formatado ?? "",
                CidadeEstadoProprietarioVeiculo = veiculoTracao.Proprietario?.Localidade?.DescricaoCidadeEstado ?? "",
                INSSProprietarioVeiculo = "",
                CEPProprietarioVeiculo = veiculoTracao.Proprietario?.CEP ?? "",
                EmailProprietarioVeiculo = veiculoTracao.Proprietario?.Email ?? "",
                TelefoneProprietarioVeiculo = veiculoTracao.Proprietario?.Telefone1 ?? ""
            };

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.FichaMotorista.FichaMotorista>()
                    { dsFichaMotorista },
                Parameters = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>()
            };

        List<Dominio.Entidades.Veiculo> veiculoReboques = repVeiculo.BuscarPorCodigo(codigosReboques);

        List<Dominio.Relatorios.Embarcador.DataSource.Cargas.FichaMotoristaVeiculo.FichaMotoristaVeiculo>
            dsFichaMotoristaVeiculo;
        dsFichaMotoristaVeiculo = (from veiculoReboque in veiculoReboques
            where veiculoReboque.TipoVeiculo.Equals("1")
            select new Dominio.Relatorios.Embarcador.DataSource.Cargas.FichaMotoristaVeiculo.FichaMotoristaVeiculo()
            {
                PlacaCarreta = veiculoReboque.Placa ?? "",
                TaraCarreta = veiculoReboque.Tara.ToString() ?? "",
                MarcaCarreta = veiculoReboque.DescricaoComMarcaModelo ?? "",
                TipoCarreta = veiculoReboque.DescricaoTipo ?? "",
                NrRenavamCarreta = veiculoReboque.Renavam ?? "",
                ComprimentoCarreta = "",
                NrChassisCarreta = veiculoReboque.Chassi ?? "",
                LarguraCarreta = "",
                EspecieTipoCarreta = veiculoReboque.DescricaoTipoVeiculo ?? "",
                CidadeEstadoCarreta = veiculoReboque.LocalidadeAtual?.DescricaoCidadeEstado ?? "",
                RNTRCANTTCarreta = veiculoReboque.RNTRC.ToString() ?? "",
                NrEixosCarreta = "",
                CorCarreta = veiculoReboque?.CorVeiculo?.Descricao ?? "",
                AnoFabModeloCarreta =
                    veiculoReboque.AnoFabricacao.ToString() + "-" + veiculoReboque.DescricaoModelo ?? "",

                NomeProprietarioCarreta = veiculoReboque.Proprietario?.Nome ?? "",
                EnderecoProprietarioCarreta = veiculoReboque.Proprietario?.Endereco ?? "",
                BairroProprietarioCarreta = veiculoReboque.Proprietario?.Bairro ?? "",
                CPFCNPJProprietarioCarreta = veiculoReboque.Proprietario?.CPF_CNPJ_Formatado ?? "",
                CidadeEstadoProprietarioCarreta = veiculoReboque.Proprietario?.Localidade?.DescricaoCidadeEstado ?? "",
                INSSProprietarioCarreta = "",
                CEPProprietarioCarreta = veiculoReboque.Proprietario?.CEP ?? "",
                EmailProprietarioCarreta = veiculoReboque.Proprietario?.Email ?? "",
                TelefoneProprietarioCarreta = veiculoReboque.Proprietario?.Telefone1 ?? "",
            }).ToList();

        dataSet.SubReports = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>()
        {
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                Key = "FichaMotoristaVeiculo.rpt",
                DataSet = dsFichaMotoristaVeiculo,
            }
        };

        byte[] pdf = RelatorioSemPadraoReportService.GerarRelatorio(
            @"Areas\Relatorios\Reports\Default\Cargas\FichaMotoristaCarga.rpt",
            Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);

        return PrepareReportResult(FileType.PDF, pdf);
    }
}