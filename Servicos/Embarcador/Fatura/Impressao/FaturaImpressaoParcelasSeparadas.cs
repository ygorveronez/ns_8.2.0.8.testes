using Dominio.Relatorios.Embarcador.Enumeradores;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;
using Utilidades.Extensions;

namespace Servicos.Embarcador.Fatura
{
    public sealed class FaturaImpressaoParcelasSeparadas : FaturaImpressao
    {
        #region Construtores

        public FaturaImpressaoParcelasSeparadas(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
            : base(unitOfWork, tipoServicoMultisoftware, configuracaoEmbarcador, "FaturaParcelasSeparadas.rpt", CodigoControleRelatorios.R243_FaturaParcelasSeparadas)
        {
        }

        #endregion

        #region Métodos Sobrescritos

        public override void GerarRelatorio(Dominio.Entidades.Embarcador.Fatura.Fatura fatura, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImpressaoFatura? tipoImpressaoFatura = null)
        {
            Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(_unitOfWork);

            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = ObterParametros(fatura);
            var report = ReportRequest.WithType(ReportType.FaturaImpressaoParcelasSeparadas)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("parametros", parametros.ToJson())
                .AddExtraData("codigofatura", fatura.Codigo)
                .AddExtraData("relatorioControleGeracao", relatorioControleGeracao.Codigo)
                .AddExtraData("relatorioTemporario", servicoRelatorio.ObterConfiguracaoRelatorio(relatorioTemporario).ToJson())
                .AddExtraData("tipoImpressaoFatura", tipoImpressaoFatura.ToJson())
                .CallReport()
                .GetContentFile();
        }

        public override Dominio.Entidades.Embarcador.Relatorios.Relatorio ObterRelatorioTemporario(Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio)
        {
            Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario = ObterRelatorioTemporarioBase(relatorio);

            relatorioTemporario.Colunas = new List<Dominio.Entidades.Embarcador.Relatorios.RelatorioColuna>();

            return relatorioTemporario;
        }

        #endregion
    }
}
