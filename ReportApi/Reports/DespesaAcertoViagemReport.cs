using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;
using System;
using System.Collections.Generic;
using System.Linq;
using Utilidades.Extensions;

namespace ReportApi.Reports;

[UseReportType(ReportType.DespesaAcertoViagem)]
public class DespesaAcertoViagemReport : ReportBase
{
    public DespesaAcertoViagemReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {

        var info = extraData.GetInfo();
        var usuario = BuscarUsuario(extraData.GetValue<int>("CodigoUsuario"));
        var relatorio = extraData.GetValue<string>("Relatorio");
        var filtrosPesquisa = extraData.GetValue<string>("FiltrosPesquisa")
            .FromJson<Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioDespesaAcertoViagem>();

        var codigoRelatorioControleGeracao = extraData.GetValue<int>("relatorioControleGeracao");

        Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(_unitOfWork);
        Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(_unitOfWork);

        Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(relatorio);
        Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = repRelatorio.BuscarPorCodigo(dynRelatorio.Codigo);
        Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(_unitOfWork).BuscarPorCodigo(codigoRelatorioControleGeracao);

        ReportApi.Models.Grid.Relatorio gridRelatorio = new ReportApi.Models.Grid.Relatorio();
        Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp = serRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, info.TipoServico);

        gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemp);

        string stringConexao = _unitOfWork.StringConexao;
        GerarRelatorioDespesaAcertoViagem(filtrosPesquisa, relatorioControleGeracao, relatorioTemp, stringConexao);
        return PrepareReportResult(FileType.PDF);
    }
    private void GerarRelatorioDespesaAcertoViagem(Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioDespesaAcertoViagem filtrosPesquisa, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp, string stringConexao)
    {
        Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

        Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
            try
            {
                // TODO (async-ct): Tornar método assíncrono.
                var listaDespesaAcertoViagem = repAcertoViagem.RelatorioDespesaAcertoViagem(filtrosPesquisa, relatorioTemp.PropriedadeAgrupa, relatorioTemp.OrdemAgrupamento, relatorioTemp.PropriedadeOrdena, relatorioTemp.OrdemOrdenacao, 0, 0, CancellationToken.None, false).GetAwaiter().GetResult();

            var lista = (from obj in listaDespesaAcertoViagem
                         select new
                         {
                             Data = obj.Data != null && obj.Data > DateTime.MinValue ? obj.Data.ToString("dd/MM/yyyy") : string.Empty,
                             obj.Fornecedor,
                             obj.NumeroAcerto,
                             obj.Observacao,
                             obj.Quantidade,
                             obj.Valor,
                             obj.Placa,
                             DataAcerto = obj.DataAcerto != null && obj.DataAcerto > DateTime.MinValue ? obj.DataAcerto.ToString("dd/MM/yyyy") : string.Empty,
                             DataInicialAcerto = obj.DataInicialAcerto != null && obj.DataInicialAcerto > DateTime.MinValue ? obj.DataInicialAcerto.ToString("dd/MM/yyyy") : string.Empty,
                             DataFinalAcerto = obj.DataFinalAcerto != null && obj.DataFinalAcerto > DateTime.MinValue ? obj.DataFinalAcerto.ToString("dd/MM/yyyy") : string.Empty,
                             obj.Situacao,
                             obj.Motorista,
                             obj.ModeloVeiculo,
                             obj.Justificativa,
                             Moeda = !obj.MoedaCotacaoBancoCentral.HasValue ? "Reais" : ((Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral)obj.MoedaCotacaoBancoCentral.Value).ObterDescricao(),
                             obj.ValorMoedaCotacao,
                             obj.ValorOriginalMoedaEstrangeira,
                             obj.PaisFornecedor
                         }).ToList();

            Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT identificacaoCamposRPT = new Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT();
            identificacaoCamposRPT.PrefixoCamposSum = "";
            identificacaoCamposRPT.IndiceSumGroup = "3";
            identificacaoCamposRPT.IndiceSumReport = "4";
            CrystalDecisions.CrystalReports.Engine.ReportDocument report = _servicoRelatorioReportService.CriarRelatorio(relatorioControleGeracao, relatorioTemp, lista, unitOfWork, identificacaoCamposRPT);
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Usuario repFuncionario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
            Repositorio.Embarcador.Fatura.Justificativa repJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Pais repPais = new Repositorio.Pais(unitOfWork);

            List<Dominio.Entidades.Pais> paises = filtrosPesquisa.CodigoPais.Count > 0 ? repPais.BuscarPorCodigos(filtrosPesquisa.CodigoPais) : null;

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataAcerto", filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal));

            if ((int)filtrosPesquisa.Situacao > 0)
            {
                if (filtrosPesquisa.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.Cancelado)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", "Cancelado", true));
                else if (filtrosPesquisa.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.EmAntamento)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", "Em Andamento", true));
                if (filtrosPesquisa.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.Fechado)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", "Fechado", true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", "Todos", true));

            if (filtrosPesquisa.CodigoAcertoViagem > 0)
            {
                Dominio.Entidades.Embarcador.Acerto.AcertoViagem acerto = repAcertoViagem.BuscarPorCodigo(filtrosPesquisa.CodigoAcertoViagem);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("AcertoViagem", acerto.Numero.ToString(), true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("AcertoViagem", false));

            if (filtrosPesquisa.CodigoMotorista > 0)
            {
                Dominio.Entidades.Usuario motorista = repFuncionario.BuscarPorCodigo(filtrosPesquisa.CodigoMotorista);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Motorista", "(" + motorista.CPF_Formatado + ") " + motorista.Nome, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Motorista", false));

            if (filtrosPesquisa.CodigoModeloVeicular > 0)
            {
                Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicular = repModeloVeicularCarga.BuscarPorCodigo(filtrosPesquisa.CodigoModeloVeicular);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ModeloVeicular", modeloVeicular.Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ModeloVeicular", false));

            if (filtrosPesquisa.CodigoVeiculoTracao > 0)
            {
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigoVeiculoTracao);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("VeiculoTracao", veiculo.Placa, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("VeiculoTracao", false));

            if (filtrosPesquisa.CodigoVeiculoReboque > 0)
            {
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigoVeiculoReboque);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("VeiculoReboque", veiculo.Placa, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("VeiculoReboque", false));

            if (filtrosPesquisa.CodigoProduto > 0)
            {
                Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Produto", repProduto.BuscarPorCodigo(filtrosPesquisa.CodigoProduto).Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Produto", false));

            if (!string.IsNullOrWhiteSpace(relatorioTemp.PropriedadeAgrupa))
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", relatorioTemp.PropriedadeAgrupa, true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", false));

            if (filtrosPesquisa.CodigoJustificativa > 0)
            {
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Justificativa", repJustificativa.BuscarPorCodigo(filtrosPesquisa.CodigoJustificativa).Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Justificativa", false));

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PaisFornecedor", paises != null && paises.Count > 0 ? string.Join(", ", paises.Select(o => o.Nome).ToList()) : null));

            _servicoRelatorioReportService.PreecherParamentrosFiltro(report, relatorioControleGeracao, relatorioTemp, parametros);

            _servicoRelatorioReportService.GerarRelatorio(report, relatorioControleGeracao, "Relatorios/AcertoViagem/DespesaAcertoViagem", unitOfWork);
        }
        catch (Exception ex)
        {
            serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, ex);
        }
        finally
        {
            unitOfWork.Dispose();
        }
    }
}