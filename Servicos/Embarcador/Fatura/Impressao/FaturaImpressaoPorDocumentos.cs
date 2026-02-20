using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.Enumeradores;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;
using Utilidades.Extensions;

namespace Servicos.Embarcador.Fatura
{
    public class FaturaImpressaoPorDocumentos : FaturaImpressao
    {
        #region Construtores

        public FaturaImpressaoPorDocumentos(Repositorio.UnitOfWork unitOfWork,
            AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware,
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
            : base(unitOfWork, tipoServicoMultisoftware, configuracaoEmbarcador, "FaturaPorDocumentos.rpt",
                CodigoControleRelatorios.R220_FaturaPorDocumentos)
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
            var report = ReportRequest.WithType(ReportType.FaturaImpressaoPorDocumentos)
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
            Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario =
                ObterRelatorioTemporarioBase(relatorio);
            relatorioTemporario.TamanhoPadraoFonte = 7;
            relatorioTemporario.CortarLinhas = false;

            relatorioTemporario.Colunas = new List<Dominio.Entidades.Embarcador.Relatorios.RelatorioColuna>();

            relatorioTemporario.Colunas.Add(ObterRelatorioTemporarioColuna(relatorioTemporario, "Codigo",
                relatorioTemporario.Colunas.Count));
            relatorioTemporario.Colunas.Add(ObterRelatorioTemporarioColuna(relatorioTemporario, "NumeroCTe",
                relatorioTemporario.Colunas.Count, "Nº Documentos", Alinhamento.left, 10));
            relatorioTemporario.Colunas.Add(ObterRelatorioTemporarioColuna(relatorioTemporario, "DataEmissao",
                relatorioTemporario.Colunas.Count, "Data Emissão", Alinhamento.center, 12));
            relatorioTemporario.Colunas.Add(ObterRelatorioTemporarioColuna(relatorioTemporario, "Destinatario",
                relatorioTemporario.Colunas.Count, "Destinatário", Alinhamento.left, 25));
            relatorioTemporario.Colunas.Add(ObterRelatorioTemporarioColuna(relatorioTemporario, "Notas",
                relatorioTemporario.Colunas.Count, "Notas", Alinhamento.left, 10));
            relatorioTemporario.Colunas.Add(ObterRelatorioTemporarioColuna(relatorioTemporario, "ValorMercadoria",
                relatorioTemporario.Colunas.Count, "Vlr. Mercadoria", Alinhamento.right, 10, TipoSumarizacao.sum));
            relatorioTemporario.Colunas.Add(ObterRelatorioTemporarioColuna(relatorioTemporario, "Peso",
                relatorioTemporario.Colunas.Count, "Peso", Alinhamento.right, 8, TipoSumarizacao.sum));
            relatorioTemporario.Colunas.Add(ObterRelatorioTemporarioColuna(relatorioTemporario, "BaseCalculoICMS",
                relatorioTemporario.Colunas.Count, "BC. ICMS", Alinhamento.right, 8, TipoSumarizacao.sum));
            relatorioTemporario.Colunas.Add(ObterRelatorioTemporarioColuna(relatorioTemporario, "ICMS",
                relatorioTemporario.Colunas.Count, "ICMS", Alinhamento.right, 8, TipoSumarizacao.sum));
            relatorioTemporario.Colunas.Add(ObterRelatorioTemporarioColuna(relatorioTemporario, "Total",
                relatorioTemporario.Colunas.Count, "Total", Alinhamento.right, 8, TipoSumarizacao.sum));

            return relatorioTemporario;
        }

        #endregion
    }
}