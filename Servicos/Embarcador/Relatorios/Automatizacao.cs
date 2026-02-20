using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Servicos.Embarcador.Relatorios
{
    public class Automatizacao
    {
        #region Atributos

        protected readonly Repositorio.UnitOfWork _unitOfWork;
        protected readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        protected readonly AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente _clienteMultisoftware;

        #endregion

        #region Construtores 

        public Automatizacao(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _clienteMultisoftware = clienteMultisoftware;
        }

        #endregion

        #region Métodos Públicos

        public void AtualizarFiltrosPesquisaAutomatizacao(object filtroPesquisa, Dominio.Entidades.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorio automatizacaoGeracaoRelatorio)
        {
            Type[] tiposPropriedadesFiltrarPorData = new[] { typeof(DateTime), typeof(DateTime?) };
            PropertyInfo[] propriedadesFiltrarPorData = filtroPesquisa.GetType().GetProperties().Where(propriedade => tiposPropriedadesFiltrarPorData.Contains(propriedade.PropertyType)).ToArray();

            foreach (PropertyInfo propriedade in propriedadesFiltrarPorData)
            {
                object valorPropriedade = propriedade.GetValue(filtroPesquisa);

                if (valorPropriedade == null)
                    continue;

                DateTime dataFiltroOriginal = (DateTime)valorPropriedade;

                if (dataFiltroOriginal == DateTime.MinValue)
                    continue;

                switch (automatizacaoGeracaoRelatorio.OcorrenciaGeracao)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OcorrenciaGeracaoRelatorio.Diario:
                        propriedade.SetValue(filtroPesquisa, ObterDataFiltroAutomatizacaoDiariaAjustada(automatizacaoGeracaoRelatorio, dataFiltroOriginal));
                        break;

                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OcorrenciaGeracaoRelatorio.Semanal:
                        propriedade.SetValue(filtroPesquisa, ObterDataFiltroAutomatizacaoSemanalAjustada(automatizacaoGeracaoRelatorio, dataFiltroOriginal));
                        break;

                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OcorrenciaGeracaoRelatorio.Mensal:
                        propriedade.SetValue(filtroPesquisa, ObterDataFiltroAutomatizacaoMensalAjustada(automatizacaoGeracaoRelatorio, dataFiltroOriginal));
                        break;
                }
            }
        }

        public void GerarRelatorio(int codigoAutomatizacao)
        {
            Repositorio.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorio repositorioAutomatizacaoGeracaoRelatorio = new Repositorio.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorio(_unitOfWork);
            Dominio.Entidades.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorio automatizacaoGeracaoRelatorio = repositorioAutomatizacaoGeracaoRelatorio.BuscarPorCodigo(codigoAutomatizacao, false);

            if (automatizacaoGeracaoRelatorio == null)
                return;

            _unitOfWork.Start();

            switch (automatizacaoGeracaoRelatorio.TipoArquivo)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoGeracaoRelatorio.Excel:
                    GerarRegistroGeracaoRelatorioPorAutomatizacao(automatizacaoGeracaoRelatorio, Dominio.Enumeradores.TipoArquivoRelatorio.CSV);
                    break;

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoGeracaoRelatorio.Pdf:
                    GerarRegistroGeracaoRelatorioPorAutomatizacao(automatizacaoGeracaoRelatorio, Dominio.Enumeradores.TipoArquivoRelatorio.PDF);
                    break;

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoGeracaoRelatorio.PdfEExcel:
                    GerarRegistroGeracaoRelatorioPorAutomatizacao(automatizacaoGeracaoRelatorio, Dominio.Enumeradores.TipoArquivoRelatorio.PDF);
                    GerarRegistroGeracaoRelatorioPorAutomatizacao(automatizacaoGeracaoRelatorio, Dominio.Enumeradores.TipoArquivoRelatorio.CSV);
                    break;

                default:
                    throw new Exception("Arquivo não implementado para a geração do relatório por automatização.");
            }

            automatizacaoGeracaoRelatorio.DataProximaGeracao = ObterDataProximaGeracao(automatizacaoGeracaoRelatorio);

            repositorioAutomatizacaoGeracaoRelatorio.Atualizar(automatizacaoGeracaoRelatorio);

            _unitOfWork.CommitChanges();
        }

        public DateTime ObterDataProximaGeracao(Dominio.Entidades.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorio automatizacaoGeracaoRelatorio)
        {
            switch (automatizacaoGeracaoRelatorio.OcorrenciaGeracao)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OcorrenciaGeracaoRelatorio.Semanal:
                    return ObterDataProximaGeracaoSemanal(automatizacaoGeracaoRelatorio);

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OcorrenciaGeracaoRelatorio.Mensal:
                    if (automatizacaoGeracaoRelatorio.GerarSomenteEmDiaUtil)
                        return ObterDataProximaGeracaoMensalPorDiaUtil(automatizacaoGeracaoRelatorio, automatizacaoGeracaoRelatorio.DataProximaGeracao);

                    return ObterDataProximaGeracaoMensal(automatizacaoGeracaoRelatorio, automatizacaoGeracaoRelatorio.DataProximaGeracao);

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OcorrenciaGeracaoRelatorio.Diario:
                    return ObterDataProximaGeracaoDiaria(automatizacaoGeracaoRelatorio);

                default:
                    return DateTime.Now.AddYears(100); //vai gerar daqui 100 anos... eu é que não vou estar aqui pra ver esse relatório gerar.
            }
        }

        #endregion

        #region Métodos Privados 

        private void GerarRegistroGeracaoRelatorioPorAutomatizacao(Dominio.Entidades.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorio automatizacaoGeracaoRelatorio, Dominio.Enumeradores.TipoArquivoRelatorio tipoArquivoRelatorio)
        {
            Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repositorioRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = new Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao
            {
                AutomatizacaoGeracaoRelatorio = automatizacaoGeracaoRelatorio,
                Relatorio = automatizacaoGeracaoRelatorio.Relatorio,
                DataInicioGeracao = DateTime.Now,
                DataFinalGeracao = DateTime.Now,
                GuidArquivo = Guid.NewGuid().ToString().Replace("-", ""),
                Titulo = automatizacaoGeracaoRelatorio.Relatorio?.Titulo ?? "",
                Usuario = automatizacaoGeracaoRelatorio.Usuario,
                TipoArquivoRelatorio = tipoArquivoRelatorio,
                SituacaoGeracaoRelatorio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGeracaoRelatorio.EmExecucao,
                CodigoEntidade = 0,
                Empresa = automatizacaoGeracaoRelatorio.Usuario?.Empresa,
                GerarPorServico = true,
                DadosConsulta = null,
                TipoServicoMultisoftware = _tipoServicoMultisoftware
            };

            repositorioRelatorioControleGeracao.Inserir(relatorioControleGeracao);
        }

        private DateTime ObterDataFiltroAutomatizacaoDiariaAjustada(Dominio.Entidades.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorio automatizacaoGeracaoRelatorio, DateTime dataFiltro)
        {
            int diasDiferenca = (DateTime.Now.Date - automatizacaoGeracaoRelatorio.DataBase.Date).Days;

            return dataFiltro.AddDays(diasDiferenca);
        }

        private DateTime ObterDataFiltroAutomatizacaoMensalAjustada(Dominio.Entidades.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorio automatizacaoGeracaoRelatorio, DateTime dataFiltro)
        {
            int mesesDiferenca = automatizacaoGeracaoRelatorio.DataBase.DifferenceOfMonthsBetween(DateTime.Now);
            DateTime dataFiltroAjustada = dataFiltro.AddMonths(mesesDiferenca - 1);

            if (dataFiltro.IsLastDayOfMonth() && !dataFiltroAjustada.IsLastDayOfMonth())
            {
                int diasDiferenca = dataFiltroAjustada.LastDayOfMonth().Day - dataFiltroAjustada.Day;

                dataFiltroAjustada.AddDays(diasDiferenca);
            }

            return dataFiltroAjustada;
        }

        private DateTime ObterDataFiltroAutomatizacaoSemanalAjustada(Dominio.Entidades.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorio automatizacaoGeracaoRelatorio, DateTime dataFiltro)
        {
            int diasDiferenca = (DateTime.Now.Date - automatizacaoGeracaoRelatorio.DataBase.Date).Days;

            return dataFiltro.AddDays(diasDiferenca);
        }

        private DateTime ObterDataProximaGeracaoDiaria(Dominio.Entidades.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorio automatizacaoGeracaoRelatorio)
        {
            DateTime dataAtual = DateTime.Now;
            DateTime dataProximaGeracao = dataAtual.Date.Add(automatizacaoGeracaoRelatorio.HoraGeracao);

            if (dataProximaGeracao < dataAtual.AddMinutes(5))
                dataProximaGeracao = dataProximaGeracao.AddDays(1);

            return dataProximaGeracao;
        }

        private DateTime ObterDataProximaGeracaoMensal(Dominio.Entidades.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorio automatizacaoGeracaoRelatorio, DateTime dataGeracaoAtual)
        {
            DateTime dataAtual = DateTime.Now.Date;
            DateTime dataInicialMesAtual = dataAtual.FirstDayOfMonth();
            DateTime dataInicialMesGeracaoAtual = dataGeracaoAtual.Date.FirstDayOfMonth();
            DateTime dataInicialMesProximaGeracao = (dataInicialMesGeracaoAtual > dataInicialMesAtual) ? dataInicialMesGeracaoAtual : dataInicialMesAtual;
            int ultimoDiaMesProximaGeracao = dataInicialMesProximaGeracao.LastDayOfMonth().Day;
            int diaProximaGeracao = Math.Min(automatizacaoGeracaoRelatorio.DiaGeracao, ultimoDiaMesProximaGeracao);
            DateTime dataProximaGeracao = new DateTime(dataInicialMesProximaGeracao.Year, dataInicialMesProximaGeracao.Month, diaProximaGeracao, automatizacaoGeracaoRelatorio.HoraGeracao.Hours, automatizacaoGeracaoRelatorio.HoraGeracao.Minutes, 0);

            if (dataProximaGeracao.Date <= dataAtual)
                return ObterDataProximaGeracaoMensal(automatizacaoGeracaoRelatorio, dataInicialMesAtual.AddMonths(1));

            return dataProximaGeracao;
        }

        private DateTime ObterDataProximaGeracaoMensalPorDiaUtil(Dominio.Entidades.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorio automatizacaoGeracaoRelatorio, DateTime dataGeracaoAtual)
        {
            DateTime dataAtual = DateTime.Now.Date;
            DateTime dataInicialMesAtual = dataAtual.FirstDayOfMonth();
            DateTime dataInicialMesGeracaoAtual = dataGeracaoAtual.Date.FirstDayOfMonth();
            DateTime dataInicialMesProximaGeracao = (dataInicialMesGeracaoAtual > dataInicialMesAtual) ? dataInicialMesGeracaoAtual : dataInicialMesAtual;
            DateTime dataFinalMesProximaGeracao = dataInicialMesProximaGeracao.LastDayOfMonth();
            List<DateTime> datasComFeriado = new Configuracoes.Feriado(_unitOfWork).ObterDatasComFeriado(dataInicialMesProximaGeracao, dataFinalMesProximaGeracao, automatizacaoGeracaoRelatorio.Usuario.Localidade?.Codigo ?? 0, automatizacaoGeracaoRelatorio.Usuario.Localidade?.Estado?.Sigla ?? "");
            int ultimoDiaMesProximaGeracao = dataFinalMesProximaGeracao.Day;
            int diaProximaGeracao = Math.Min(automatizacaoGeracaoRelatorio.DiaGeracao, ultimoDiaMesProximaGeracao);
            int diasUteisAdicionados = 0;
            DateTime? dataProximaGeracao = null;

            for (int diasAdicionar = 0; diasAdicionar < ultimoDiaMesProximaGeracao; diasAdicionar++)
            {
                DateTime data = dataInicialMesProximaGeracao.AddDays(diasAdicionar);

                if (datasComFeriado.Contains(data))
                    continue;

                if ((data.DayOfWeek == DayOfWeek.Saturday) || (data.DayOfWeek == DayOfWeek.Sunday))
                    continue;

                diasUteisAdicionados++;

                if (diasUteisAdicionados == diaProximaGeracao)
                {
                    dataProximaGeracao = data;
                    break;
                }
            }

            if (!dataProximaGeracao.HasValue)
                dataProximaGeracao = dataFinalMesProximaGeracao;

            if (dataProximaGeracao.Value <= dataAtual)
                return ObterDataProximaGeracaoMensalPorDiaUtil(automatizacaoGeracaoRelatorio, dataInicialMesAtual.AddMonths(1));

            return dataProximaGeracao.Value.Add(automatizacaoGeracaoRelatorio.HoraGeracao);
        }

        private DateTime ObterDataProximaGeracaoSemanal(Dominio.Entidades.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorio automatizacaoGeracaoRelatorio)
        {
            List<DayOfWeek> diasSemanaParaGeracao = ObterDiasSemanaParaGeracao(automatizacaoGeracaoRelatorio);
            DateTime dataAtual = DateTime.Now;
            DateTime dataInicialProximaGeracao = dataAtual.Date.Add(automatizacaoGeracaoRelatorio.HoraGeracao);
            DateTime dataProximaGeracao = dataInicialProximaGeracao;

            for (int diasAdicionar = 1; diasAdicionar <= 7; diasAdicionar++)
            {
                if (diasSemanaParaGeracao.Contains(dataProximaGeracao.DayOfWeek) && (dataProximaGeracao > dataAtual.AddMinutes(5)))
                    break;

                dataProximaGeracao = dataInicialProximaGeracao.AddDays(diasAdicionar);
            }

            return dataProximaGeracao;
        }

        private List<DayOfWeek> ObterDiasSemanaParaGeracao(Dominio.Entidades.Embarcador.Relatorios.AutomatizacaoGeracaoRelatorio automatizacaoGeracaoRelatorio)
        {
            List<DayOfWeek> diasSemanaGerarAutomatizacao = new List<DayOfWeek>();

            if (automatizacaoGeracaoRelatorio.GerarDomingo)
                diasSemanaGerarAutomatizacao.Add(DayOfWeek.Sunday);

            if (automatizacaoGeracaoRelatorio.GerarSegunda)
                diasSemanaGerarAutomatizacao.Add(DayOfWeek.Monday);

            if (automatizacaoGeracaoRelatorio.GerarTerca)
                diasSemanaGerarAutomatizacao.Add(DayOfWeek.Tuesday);

            if (automatizacaoGeracaoRelatorio.GerarQuarta)
                diasSemanaGerarAutomatizacao.Add(DayOfWeek.Wednesday);

            if (automatizacaoGeracaoRelatorio.GerarQuinta)
                diasSemanaGerarAutomatizacao.Add(DayOfWeek.Thursday);

            if (automatizacaoGeracaoRelatorio.GerarSexta)
                diasSemanaGerarAutomatizacao.Add(DayOfWeek.Friday);

            if (automatizacaoGeracaoRelatorio.GerarSabado)
                diasSemanaGerarAutomatizacao.Add(DayOfWeek.Saturday);

            return diasSemanaGerarAutomatizacao;
        }

        #endregion
    }
}
