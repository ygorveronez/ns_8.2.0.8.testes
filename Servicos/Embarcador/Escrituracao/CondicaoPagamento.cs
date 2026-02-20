using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Escrituracao
{
    public class CondicaoPagamento
    {
        #region Métodos Públicos

        public static DateTime CalculaDataPagamento(Dominio.ObjetosDeValor.Embarcador.Escrituracao.CondicaoPagamento condicao, DateTime database, Repositorio.UnitOfWork unitOfWork)
        {
            if (database == null || database == DateTime.MinValue)
                throw new Dominio.Excecoes.Embarcador.ServicoException("Data base para calculo do pagamento não informada.");
            //Manter a Logica de definição da DataBase fora desse método

            // 1 - Se pagamento for fora do mês, data base passa a ser o primeiro dia do próximo mês
            if (condicao.VencimentoForaMes)
                database = database.AddMonths(1).AddDays(-database.Day + 1);

            // 2 - Adiciona o tempo do prazo para pagamento
            database = database.AddDays(condicao.DiasDePrazoPagamento ?? 0);

            // 3 - Define pagamento como dia de semana ou dia mes (na prática, somente uma dessas vai ser definida)
            int diaSemanaCondicao = condicao.DiaSemana.HasValue ? (int)condicao.DiaSemana.Value : 0;
            int diaSemanaPagamento = (int)database.DayOfWeek + 1; //DayOfWeek começa em 0 e DiaSemana em 1
            if (condicao.DiaSemana.HasValue && diaSemanaCondicao != diaSemanaPagamento)
            {
                int diff = 0;
                if (diaSemanaCondicao > diaSemanaPagamento)
                    diff = diaSemanaCondicao - diaSemanaPagamento;
                else
                    diff = 7 - (diaSemanaPagamento - diaSemanaCondicao);

                database = database.AddDays(diff);
            }

            // 3 - Define pagamento como dia de semana ou dia mes (na prática, somente uma dessas vai ser definida)
            int diaMesCondicao = condicao.DiaMes ?? 0;
            if (diaMesCondicao > 0 && diaMesCondicao != database.Day)
            {
                if (diaMesCondicao < database.Day)
                    database = database.AddDays(diaMesCondicao - database.Day).AddMonths(1);
                else
                    database = database.AddDays(diaMesCondicao - database.Day);
            }

            if (condicao.ConsiderarDiaUtilVencimento ?? false)
            {
                Configuracoes.Feriado servicoFeriado = new Configuracoes.Feriado(unitOfWork);

                while (true)
                {
                    if (database.DayOfWeek == DayOfWeek.Saturday || database.DayOfWeek == DayOfWeek.Sunday)
                        database = database.AddDays(1);
                    else if (servicoFeriado.VerificarSePossuiFeriado(database))
                        database = database.AddDays(1);
                    else
                        break;
                }
            }

            return database;
        }

        public static Dominio.ObjetosDeValor.Embarcador.Escrituracao.CondicaoPagamento BuscarCondicaoFiltrada(int codigoEmpresa, Dominio.ObjetosDeValor.Embarcador.Escrituracao.CondicaoPagamento condicaoComparacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Transportadores.CondicaoPagamentoTransportador repositorioCondicaoPagamentoTransportador = new Repositorio.Embarcador.Transportadores.CondicaoPagamentoTransportador(unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.CondicaoPagamento> condicoesPagamento = repositorioCondicaoPagamentoTransportador.BuscarObjetoPorEmpresa(codigoEmpresa);
            Dominio.ObjetosDeValor.Embarcador.Escrituracao.CondicaoPagamento condicaoPagamento = null;
            int maximoParametrosCompativel = 0;

            foreach (Dominio.ObjetosDeValor.Embarcador.Escrituracao.CondicaoPagamento condicao in condicoesPagamento)
            {
                int parametrosCompativeis = 0;
                if (condicao.CodigoTipoOperacao.HasValue)
                {
                    if (condicao.CodigoTipoOperacao != condicaoComparacao.CodigoTipoOperacao)
                        continue;
                    parametrosCompativeis++;
                }

                if (condicao.CodigoTipoCarga.HasValue)
                {
                    if (condicao.CodigoTipoCarga != condicaoComparacao.CodigoTipoCarga)
                        continue;
                    parametrosCompativeis++;
                }

                if (condicao.TipoPrazoPagamento.HasValue)
                {
                    if (condicao.TipoPrazoPagamento != condicaoComparacao.TipoPrazoPagamento)
                        continue;
                    parametrosCompativeis++;
                }

                if (parametrosCompativeis > maximoParametrosCompativel)
                {
                    maximoParametrosCompativel = parametrosCompativeis;
                    condicaoPagamento = condicao;
                }
                else if (parametrosCompativeis == 0 && maximoParametrosCompativel == 0)
                    condicaoPagamento = condicao;
            }
            return condicaoPagamento;
        }

        #endregion
    }
}
