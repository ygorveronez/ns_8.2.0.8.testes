using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Relatorios;
using Dominio.Relatorios.Embarcador.Enumeradores;
using Servicos.Extensions;
using System.Collections.Generic;
using Utilidades.Extensions;

namespace Servicos.Embarcador.Fatura
{
    public class FaturaImpressaoChaveCTe : FaturaImpressao
    {
        #region Construtores

        public FaturaImpressaoChaveCTe(Repositorio.UnitOfWork unitOfWork,
            AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware,
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
            : base(unitOfWork, tipoServicoMultisoftware, configuracaoEmbarcador, "FaturaChaveCTe.rpt",
                CodigoControleRelatorios.R339_FaturaChaveCTe)
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
            byte[] report = ReportRequest.WithType(ReportType.FaturaChaveCTe)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("parametros", parametros.ToJson())
                .AddExtraData("codigofatura", fatura.Codigo)
                .AddExtraData("relatorioControleGeracao", relatorioControleGeracao.Codigo)
                .AddExtraData("relatorioTemporario", servicoRelatorio.ObterConfiguracaoRelatorio(relatorioTemporario).ToJson())
                .CallReport()
                .GetContentFile();
        }

        public override Dominio.Entidades.Embarcador.Relatorios.Relatorio ObterRelatorioTemporario(Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio)
        {
            bool exibirImpostos = _tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador;
            TipoSumarizacao tipoSumarizacao = exibirImpostos ? TipoSumarizacao.sum : TipoSumarizacao.nenhum;
            Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario = ObterRelatorioTemporarioBase(relatorio);

            relatorioTemporario.Colunas = new List<Dominio.Entidades.Embarcador.Relatorios.RelatorioColuna>();

            relatorioTemporario.Colunas.Add(ObterRelatorioTemporarioColuna(relatorioTemporario, "Codigo", relatorioTemporario.Colunas.Count));
            relatorioTemporario.Colunas.Add(ObterRelatorioTemporarioColuna(relatorioTemporario, "NumeroCTe", relatorioTemporario.Colunas.Count, "Nº Documento", Alinhamento.center, 13));
            relatorioTemporario.Colunas.Add(ObterRelatorioTemporarioColuna(relatorioTemporario, "SerieCTE", relatorioTemporario.Colunas.Count, "Série", Alinhamento.center, 6));
            relatorioTemporario.Colunas.Add(ObterRelatorioTemporarioColuna(relatorioTemporario, "ChaveCTe", relatorioTemporario.Colunas.Count, "Chave Autorização Documento", Alinhamento.center, 40));
            relatorioTemporario.Colunas.Add(ObterRelatorioTemporarioColuna(relatorioTemporario, "DataEmissao", relatorioTemporario.Colunas.Count, "Data Emissão", Alinhamento.center, 12));
            relatorioTemporario.Colunas.Add(ObterRelatorioTemporarioColuna(relatorioTemporario, "ICMS", relatorioTemporario.Colunas.Count, "Imposto", Alinhamento.right, (exibirImpostos ? 10 : 0), tipoSumarizacao));
            relatorioTemporario.Colunas.Add(ObterRelatorioTemporarioColuna(relatorioTemporario, "Total", relatorioTemporario.Colunas.Count, "Total", Alinhamento.right, 12, tipoSumarizacao));

            return relatorioTemporario;
        }

        #endregion
    }
}