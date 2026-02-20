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

[UseReportType(ReportType.MovimentacaoDePlacas)]
public class MovimentacaoDePlacasReport : ReportBase
{
    public MovimentacaoDePlacasReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);

        var veiculo = repVeiculo.BuscarPorCodigo(extraData.GetValue<int>("CodigoVeiculo"));
        bool tipoPDF = extraData.GetValue<int>("TipoArquivo") == 1;

        Repositorio.Embarcador.Veiculos.VeiculoMotorista repositorioVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(_unitOfWork);
        Repositorio.Embarcador.Veiculos.TecnologiaRastreador repositorioRastreador = new Repositorio.Embarcador.Veiculos.TecnologiaRastreador(_unitOfWork);

        //Veículo
        Dominio.Relatorios.Embarcador.DataSource.Frotas.MovimentacaoDePlacas movimentacaoDePlacas = new Dominio.Relatorios.Embarcador.DataSource.Frotas.MovimentacaoDePlacas()
        {
            Codigo = veiculo.Codigo,
            Placa = veiculo.Placa,
            NumeroRastreador = veiculo.NumeroEquipamentoRastreador != null ? veiculo.NumeroEquipamentoRastreador : string.Empty,
            TenologiaRastreador = veiculo.TecnologiaRastreador != null ? repositorioRastreador.BuscarPorCodigo(veiculo.TecnologiaRastreador.Codigo, false).Descricao : string.Empty,
            Renavam = veiculo.Renavam,
            Modelo = veiculo.Modelo?.Descricao ?? string.Empty,
            Marca = veiculo.Marca?.Descricao ?? string.Empty,
            AnoFabricacao = veiculo.AnoFabricacao.ToString(),
            AnoModelo = veiculo.AnoModelo.ToString(),
            Chassi = veiculo.Chassi,
            Tara = veiculo.Tara,
            TotalTara = veiculo.Tara,
            Gestor = veiculo.FuncionarioResponsavel?.Nome ?? string.Empty,
            GestorEmail = veiculo.FuncionarioResponsavel?.Email ?? string.Empty,
            GestorTelefone = veiculo.FuncionarioResponsavel?.Telefone_Formatado ?? string.Empty
        };

        //Reboques
        List<Dominio.Relatorios.Embarcador.DataSource.Frotas.MovimentacaoDePlacas> movimentacaoDePlacasReboques = new List<Dominio.Relatorios.Embarcador.DataSource.Frotas.MovimentacaoDePlacas>();
        foreach (Dominio.Entidades.Veiculo veiculoVinculado in veiculo.VeiculosVinculados)
        {
            Dominio.Relatorios.Embarcador.DataSource.Frotas.MovimentacaoDePlacas reboque = new Dominio.Relatorios.Embarcador.DataSource.Frotas.MovimentacaoDePlacas()
            {
                Codigo = veiculoVinculado.Codigo,
                Placa = veiculoVinculado.Placa,
                Renavam = veiculoVinculado.Renavam,
                Modelo = veiculoVinculado.Modelo?.Descricao ?? string.Empty,
                Marca = veiculoVinculado.Marca?.Descricao ?? string.Empty,
                AnoFabricacao = veiculoVinculado.AnoFabricacao.ToString(),
                AnoModelo = veiculoVinculado.AnoModelo.ToString(),
                Chassi = veiculoVinculado?.Chassi ?? string.Empty,
                Tara = veiculoVinculado.Tara,
                Gestor = veiculoVinculado.FuncionarioResponsavel?.Nome?? string.Empty,
                GestorEmail = veiculoVinculado.FuncionarioResponsavel?.Email ?? string.Empty,
                GestorTelefone = veiculoVinculado.FuncionarioResponsavel?.Telefone_Formatado ?? string.Empty
            };

            movimentacaoDePlacas.TotalTara += veiculoVinculado.Tara;
            movimentacaoDePlacas.CountReboques += 1;

            movimentacaoDePlacasReboques.Add(reboque);
        }

        //Motoristas
        Dominio.Entidades.Usuario veiculoMotoristaPrincipal = repositorioVeiculoMotorista.BuscarMotoristaPrincipal(veiculo.Codigo);
        List<Dominio.Entidades.Usuario> veiculoMotoristaSecundarios = repositorioVeiculoMotorista.BuscarMotoristasSecundarios(veiculo.Codigo);

        //Motorista Principal
        List<Dominio.Relatorios.Embarcador.DataSource.Frotas.MovimentacaoDePlacasMotorista> movimentacaoDePlacasMotoristas = new List<Dominio.Relatorios.Embarcador.DataSource.Frotas.MovimentacaoDePlacasMotorista>();
        if (veiculoMotoristaPrincipal != null)
        {
            var motoristaPrincial = new Dominio.Relatorios.Embarcador.DataSource.Frotas.MovimentacaoDePlacasMotorista()
            {
                NomeMotorista = veiculoMotoristaPrincipal.Nome,
                CPF = veiculoMotoristaPrincipal.CPF_CNPJ_Formatado,
                RG = veiculoMotoristaPrincipal.RG,
                DataRG = veiculoMotoristaPrincipal.DataEmissaoRG?.ToString("dd/MM/yyyy") ?? string.Empty,
                Telefone = veiculoMotoristaPrincipal.Telefone_Formatado,
                Celular = veiculoMotoristaPrincipal.Celular_Formatado,
                DataNascimento = veiculoMotoristaPrincipal.DataNascimento?.ToString("dd/MM/yyyy") ?? string.Empty,
                NumeroHabilitacao = veiculoMotoristaPrincipal.NumeroHabilitacao,
                VencimentoCNH = veiculoMotoristaPrincipal.DataVencimentoHabilitacao.HasValue ? veiculoMotoristaPrincipal.DataVencimentoHabilitacao?.ToString("dd/MM/yyyy") : string.Empty,
                Gestor = veiculoMotoristaPrincipal.Gestor?.Nome ?? string.Empty,
                GestorEmail = veiculoMotoristaPrincipal.Gestor?.Email ?? string.Empty,
                GestorTelefone = veiculoMotoristaPrincipal.Gestor?.Telefone ?? string.Empty
            };

            movimentacaoDePlacasMotoristas.Add(motoristaPrincial);
            movimentacaoDePlacas.CountMotoristas += 1;
        }

        //Motoristas Secundarios
        foreach (Dominio.Entidades.Usuario motoristaSecundario in veiculoMotoristaSecundarios)
        {
            var motoristaSec = new Dominio.Relatorios.Embarcador.DataSource.Frotas.MovimentacaoDePlacasMotorista()
            {
                NomeMotorista = motoristaSecundario.Nome,
                CPF = motoristaSecundario.CPF_CNPJ_Formatado,
                RG = motoristaSecundario.RG,
                DataRG = motoristaSecundario.DataEmissaoRG?.ToString("dd/MM/yyyy") ?? string.Empty,
                Telefone = motoristaSecundario.Telefone_Formatado,
                Celular = motoristaSecundario.Celular_Formatado,
                DataNascimento = motoristaSecundario.DataNascimento?.ToString("dd/MM/yyyy") ?? string.Empty,
                NumeroHabilitacao = veiculoMotoristaPrincipal.NumeroHabilitacao,
                VencimentoCNH = veiculoMotoristaPrincipal.DataVencimentoHabilitacao.HasValue ? veiculoMotoristaPrincipal.DataVencimentoHabilitacao?.ToString("dd/MM/yyyy") : string.Empty,
                Gestor = veiculoMotoristaPrincipal.Gestor?.Nome ?? string.Empty,
                GestorEmail = veiculoMotoristaPrincipal.Gestor?.Email ?? string.Empty,
                GestorTelefone = veiculoMotoristaPrincipal.Gestor?.Telefone ?? string.Empty
            };

            movimentacaoDePlacasMotoristas.Add(motoristaSec);
            movimentacaoDePlacas.CountMotoristas += 1;
        }

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet reboques = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet
        {
            Key = "MovimentacaoDePlacasReboque",
            DataSet = movimentacaoDePlacasReboques
        };

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet motoristas = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet
        {
            Key = "MovimentacaoDePlacasMotorista",
            DataSet = movimentacaoDePlacasMotoristas
        };

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
        {
            DataSet = new List<Dominio.Relatorios.Embarcador.DataSource.Frotas.MovimentacaoDePlacas>() { movimentacaoDePlacas },
            SubReports = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>() { reboques, motoristas }
        };

        Dominio.Enumeradores.TipoArquivoRelatorio tipoArquivo = tipoPDF ? Dominio.Enumeradores.TipoArquivoRelatorio.PDF : Dominio.Enumeradores.TipoArquivoRelatorio.XLS;

        byte[] report = RelatorioSemPadraoReportService.GerarRelatorio(@"Areas\Relatorios\Reports\Default\Frotas\MovimentacaoDePlacas.rpt", tipoArquivo, dataSet, true);

        return PrepareReportResult(tipoPDF ? FileType.PDF : FileType.EXCEL, report);
    }
}