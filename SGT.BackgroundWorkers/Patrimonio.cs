using SGT.BackgroundWorkers.Utils;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 86400000)]

    public class Patrimonio : LongRunningProcessBase<Patrimonio>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            if (DateTime.Now.Date.Day == 1)
                ProcessarDepreciacaoPatrimonial(unitOfWork, _tipoServicoMultisoftware);
        }

        private void ProcessarDepreciacaoPatrimonial(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            try
            {
                Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento();

                Repositorio.Embarcador.Patrimonio.Bem repBem = new Repositorio.Embarcador.Patrimonio.Bem(unitOfWork);
                Repositorio.Embarcador.Patrimonio.BemDepreciacao repBemDepreciacao = new Repositorio.Embarcador.Patrimonio.BemDepreciacao(unitOfWork);

                List<Dominio.Entidades.Embarcador.Patrimonio.Bem> listaBens = repBem.BuscarTodosAtivos();

                unitOfWork.Start();

                foreach (var bem in listaBens)
                {
                    DateTime dataAtual = DateTime.Now.Date;
                    DateTime dataProcessamento = bem.DataAquisicao;
                    if (dataProcessamento > DateTime.MinValue)
                    {
                        while (dataProcessamento.Month <= dataAtual.Month && dataProcessamento.Year <= dataAtual.Year)
                        {
                            decimal percentualDepreciacao = 0;
                            if (bem.Produto != null && bem.GrupoProdutoTMS != null && bem.GrupoProdutoTMS.PercentualDepreciacao > 0)
                                percentualDepreciacao = bem.GrupoProdutoTMS.PercentualDepreciacao / 12;
                            else if (bem.PercentualDepreciacao > 0)
                                percentualDepreciacao = bem.PercentualDepreciacao / 12;
                            Dominio.Entidades.Embarcador.Patrimonio.BemDepreciacao depreciacao = repBemDepreciacao.BuscarPorBem(bem.Codigo, dataProcessamento.Month, dataProcessamento.Year);
                            if (depreciacao == null && percentualDepreciacao > 0)
                            {
                                depreciacao = new Dominio.Entidades.Embarcador.Patrimonio.BemDepreciacao()
                                {
                                    Ano = dataProcessamento.Year,
                                    Bem = bem,
                                    Data = DateTime.Now.Date,
                                    Empresa = bem.Empresa,
                                    Mes = dataProcessamento.Month,
                                    Percentual = percentualDepreciacao,
                                    Valor = bem.ValorBem * (percentualDepreciacao / 100)
                                };
                                repBemDepreciacao.Inserir(depreciacao);

                                bem.DepreciacaoAcumulada += depreciacao.Valor;
                                bem.ValorDepreciar = bem.ValorBem - bem.DepreciacaoAcumulada;
                                if (bem.ValorDepreciar < 0)
                                    bem.ValorDepreciar = 0;
                                if (bem.ValorDepreciar > 0)
                                {
                                    decimal anosVidaUtil = (bem.ValorDepreciar / depreciacao.Valor) / 12;
                                    anosVidaUtil = anosVidaUtil.RoundUp(0);
                                    if (anosVidaUtil > 0)
                                        bem.VidaUtil = (int)Math.Truncate(anosVidaUtil);
                                }
                                repBem.Atualizar(bem);

                                Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimento = null;
                                if (bem.Produto != null && bem.GrupoProdutoTMS != null && bem.GrupoProdutoTMS.TipoMovimentoDepreciacao != null)
                                    tipoMovimento = bem.GrupoProdutoTMS.TipoMovimentoDepreciacao;
                                if (tipoMovimento != null && depreciacao.Valor > 0)
                                    servProcessoMovimento.GerarMovimentacao(tipoMovimento, Utilidades.Conversor.ExtraiDateTime("01/" + dataProcessamento.Month.ToString("n0").PadLeft(2, '0') + "/" + Utilidades.String.OnlyNumbers(dataProcessamento.Year.ToString("n0"))), depreciacao.Valor, bem.Codigo.ToString(),
                                        "DEPRECIAÇÃO AUTOMATICA DO BEM " + bem.Descricao + " SÉRIE " + bem.NumeroSerie + " NO MÊS " + dataProcessamento.Month.ToString("n0").PadLeft(2, '0') + " E ANO " + Utilidades.String.OnlyNumbers(dataProcessamento.Year.ToString("n0")), unitOfWork,
                                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Outros, tipoServicoMultisoftware);
                            }

                            dataProcessamento = dataProcessamento.AddMonths(1);
                        }
                    }

                }

                unitOfWork.CommitChanges();
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}