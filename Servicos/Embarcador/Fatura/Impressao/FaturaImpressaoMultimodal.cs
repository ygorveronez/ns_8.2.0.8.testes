using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.Enumeradores;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;
using Utilidades.Extensions;

namespace Servicos.Embarcador.Fatura
{
    public sealed class FaturaImpressaoMultimodal : FaturaImpressao
    {
        #region Construtores

        public FaturaImpressaoMultimodal(Repositorio.UnitOfWork unitOfWork,
            AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware,
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador,
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImpressaoFatura tipoImpressaoFatura)
            : base(unitOfWork, tipoServicoMultisoftware, configuracaoEmbarcador, "FaturaPadraoMultimodal.rpt",
                CodigoControleRelatorios.R200_FaturaMultimodal)
        {
        }

        #endregion

        #region Métodos Sobrescritos

        public override void GerarRelatorio(Dominio.Entidades.Embarcador.Fatura.Fatura fatura,
            Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao,
            Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario,
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImpressaoFatura? tipoImpressaoFatura = null)
        {
            Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(_unitOfWork);

            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = ObterParametros(fatura);
            var report = ReportRequest.WithType(ReportType.FaturaImpressaoMultimodal)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("parametros", parametros.ToJson())
                .AddExtraData("codigofatura", fatura.Codigo)
                .AddExtraData("relatorioControleGeracao", relatorioControleGeracao.Codigo)
                .AddExtraData("relatorioTemporario", servicoRelatorio.ObterConfiguracaoRelatorio(relatorioTemporario).ToJson())
                .AddExtraData("tipoImpressaoFatura", tipoImpressaoFatura.ToJson())
                .CallReport()
                .GetContentFile();
        }

        public override Dominio.Entidades.Embarcador.Relatorios.Relatorio ObterRelatorioTemporario(
            Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio)
        {
            bool exibirImpostos = _tipoServicoMultisoftware ==
                                  AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador;
            TipoSumarizacao tipoSumarizacao = exibirImpostos ? TipoSumarizacao.sum : TipoSumarizacao.nenhum;
            Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario =
                ObterRelatorioTemporarioBase(relatorio);

            relatorioTemporario.Colunas = new List<Dominio.Entidades.Embarcador.Relatorios.RelatorioColuna>();

            relatorioTemporario.Colunas.Add(ObterRelatorioTemporarioColuna(relatorioTemporario, "Codigo",
                relatorioTemporario.Colunas.Count));
            relatorioTemporario.Colunas.Add(ObterRelatorioTemporarioColuna(relatorioTemporario, "NumeroCTe",
                relatorioTemporario.Colunas.Count));
            relatorioTemporario.Colunas.Add(ObterRelatorioTemporarioColuna(relatorioTemporario, "SerieCTE",
                relatorioTemporario.Colunas.Count));

            if (_tipoServicoMultisoftware ==
                AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                relatorioTemporario.Colunas.Add(ObterRelatorioTemporarioColuna(relatorioTemporario, "Peso",
                    relatorioTemporario.Colunas.Count, "Peso", Alinhamento.right, 7));

            relatorioTemporario.Colunas.Add(ObterRelatorioTemporarioColuna(relatorioTemporario, "Notas",
                relatorioTemporario.Colunas.Count));
            relatorioTemporario.Colunas.Add(ObterRelatorioTemporarioColuna(relatorioTemporario, "DataEmissao",
                relatorioTemporario.Colunas.Count, "Data Emissão", Alinhamento.center, 13));
            relatorioTemporario.Colunas.Add(ObterRelatorioTemporarioColuna(relatorioTemporario, "ValorSemICMS",
                relatorioTemporario.Colunas.Count));
            relatorioTemporario.Colunas.Add(ObterRelatorioTemporarioColuna(relatorioTemporario, "ICMS",
                relatorioTemporario.Colunas.Count));
            relatorioTemporario.Colunas.Add(ObterRelatorioTemporarioColuna(relatorioTemporario, "TotalCobrado",
                relatorioTemporario.Colunas.Count));
            relatorioTemporario.Colunas.Add(ObterRelatorioTemporarioColuna(relatorioTemporario, "PedidoNavioDirecao",
                relatorioTemporario.Colunas.Count, "Navio/Viagem", Alinhamento.left, 30));
            relatorioTemporario.Colunas.Add(ObterRelatorioTemporarioColuna(relatorioTemporario, "NumeroControleCTe",
                relatorioTemporario.Colunas.Count, "Documentos", Alinhamento.center, 15));
            relatorioTemporario.Colunas.Add(ObterRelatorioTemporarioColuna(relatorioTemporario, "Trecho",
                relatorioTemporario.Colunas.Count, "Trecho", Alinhamento.center, 20));
            relatorioTemporario.Colunas.Add(ObterRelatorioTemporarioColuna(relatorioTemporario, "Total",
                relatorioTemporario.Colunas.Count, "Total", Alinhamento.right, 12, tipoSumarizacao));

            return relatorioTemporario;
        }

        #endregion
    }
}