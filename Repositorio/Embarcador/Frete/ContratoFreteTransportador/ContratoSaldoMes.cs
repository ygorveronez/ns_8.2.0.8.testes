using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.Frete
{
    public class ContratoSaldoMes : RepositorioBase<Dominio.Entidades.Embarcador.Frete.ContratoSaldoMes>
    {
        #region Construtores

        public ContratoSaldoMes(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Frete.ContratoSaldoMes> Consultar(int contratoFreteTransportador, DateTime dataInicio, DateTime dataFim)
        {
            return Consultar(contratoFreteTransportador, dataInicio, dataFim, somenteComCarga: true);
        }

        private IQueryable<Dominio.Entidades.Embarcador.Frete.ContratoSaldoMes> Consultar(int contratoFreteTransportador, DateTime dataInicio, DateTime dataFim, bool somenteComCarga)
        {
            var consultaContratoSaldoMes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoSaldoMes>()
                .Where(o =>
                    o.ContratoFreteTransportador.Codigo == contratoFreteTransportador &&
                    o.DataRegistro >= dataInicio.Date &&
                    o.DataRegistro <= dataFim.Date.Add(DateTime.MaxValue.TimeOfDay)
                );

            if (somenteComCarga)
                consultaContratoSaldoMes = consultaContratoSaldoMes.Where(o =>
                    (o.Carga != null && o.Carga.SituacaoCarga != SituacaoCarga.Cancelada && o.Carga.SituacaoCarga != SituacaoCarga.Anulada)
                );
            else
                consultaContratoSaldoMes = consultaContratoSaldoMes.Where(o =>
                    (o.Carga != null && o.Carga.SituacaoCarga != SituacaoCarga.Cancelada && o.Carga.SituacaoCarga != SituacaoCarga.Anulada) ||
                    (o.FechamentoFrete != null && o.FechamentoFrete.Situacao != SituacaoFechamentoFrete.Cancelado)
                );

            return consultaContratoSaldoMes;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.Carga> ConsultarCargasParaFechamento(int contratoFreteTransportador, int codigoFechamentoFrete, DateTime dataInicio, DateTime dataFim)
        {
            var consultaContratoSaldoMes = Consultar(contratoFreteTransportador, dataInicio, dataFim);
            var consultaFechamentoFreteCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteCarga>();
            var consultaCargasParaFechamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Carga>()
                .Where(o => consultaContratoSaldoMes.Any(s => s.Carga.Codigo == o.Codigo));

            if (codigoFechamentoFrete == 0)
            {
                consultaFechamentoFreteCarga = consultaFechamentoFreteCarga.Where(o => o.Fechamento.Situacao != SituacaoFechamentoFrete.Cancelado);
                consultaCargasParaFechamento = consultaCargasParaFechamento.Where(o => !consultaFechamentoFreteCarga.Any(f => f.Carga.Codigo == o.Codigo));
            }
            else
            {
                consultaFechamentoFreteCarga = consultaFechamentoFreteCarga.Where(o => o.Fechamento.Codigo == codigoFechamentoFrete);
                consultaCargasParaFechamento = consultaCargasParaFechamento.Where(o => consultaFechamentoFreteCarga.Any(f => f.Carga.Codigo == o.Codigo));
            }

            return consultaCargasParaFechamento;
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Frete.ContratoSaldoMes> BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoSaldoMes>()
                 .Where(o => o.Carga.Codigo == codigoCarga);

            return query.ToList();
        }
        public List<Dominio.Entidades.Embarcador.Frete.ContratoSaldoMes> BuscarPorContratoECarga(int contrato,int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoSaldoMes>()
                 .Where(o => o.Carga.Codigo == carga && o.ContratoFreteTransportador.Codigo == contrato);

            return query.ToList();
        }

        public decimal ConsultarDistanciaExcedentePorContratoFrete(int contratoFreteTransportador, DateTime dataInicio, DateTime dataFim)
        {
            var consultaContratoSaldoMes = Consultar(contratoFreteTransportador, dataInicio, dataFim);

            consultaContratoSaldoMes = consultaContratoSaldoMes.Where(o => o.Excedente == true);

            return consultaContratoSaldoMes.Sum(obj => (decimal?)obj.Distancia) ?? 0m;
        }

        public decimal ConsultarDistanciaPorContratoFrete(int contratoFreteTransportador, DateTime dataInicio, DateTime dataFim)
        {
            var consultaContratoSaldoMes = Consultar(contratoFreteTransportador, dataInicio, dataFim);

            consultaContratoSaldoMes = consultaContratoSaldoMes.Where(o => ((bool?)o.Excedente).HasValue == false || o.Excedente == false);

            return consultaContratoSaldoMes.Sum(obj => (decimal?)obj.Distancia) ?? 0m;
        }

        public decimal ConsultarDistanciaTotalPorContratoFrete(int contratoFreteTransportador, DateTime dataInicio, DateTime dataFim)
        {
            var consultaContratoSaldoMes = Consultar(contratoFreteTransportador, dataInicio, dataFim);

            return consultaContratoSaldoMes.Sum(obj => (decimal?)obj.Distancia) ?? 0m;
        }

        public decimal ConsultarValorExcedentePorContratoFrete(int contratoFreteTransportador, DateTime dataInicio, DateTime dataFim)
        {
            var consultaCarga = ConsultarCargasParaFechamento(contratoFreteTransportador, codigoFechamentoFrete: 0, dataInicio: dataInicio, dataFim: dataFim);

            return consultaCarga.Sum(o => (decimal?)o.ValorFreteContratoFreteExcedente) ?? 0m;
        }

        public decimal ConsultarValorTotalPorContratoFrete(int contratoFreteTransportador, DateTime dataInicio, DateTime dataFim, bool somenteComCarga)
        {
            var consultaContratoSaldoMes = Consultar(contratoFreteTransportador, dataInicio, dataFim, somenteComCarga);

            return consultaContratoSaldoMes.Sum(obj => (decimal?)obj.ValorPagar) ?? 0m;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Carga> BuscarCargasParaFechamento(int contrato, DateTime dataInicio, DateTime dataFim)
        {
            var consultaCarga = ConsultarCargasParaFechamento(contrato, codigoFechamentoFrete: 0, dataInicio: dataInicio, dataFim: dataFim);

            return consultaCarga.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Carga> BuscarCargasParaFechamento(int contrato, int fechamento, DateTime dataInicio, DateTime dataFim, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaCarga = ConsultarCargasParaFechamento(contrato, fechamento, dataInicio, dataFim);

            consultaCarga = consultaCarga
                .Fetch(o => o.DadosSumarizados);

            return ObterLista(consultaCarga, parametrosConsulta);
        }

        public int ContarBuscaCargasParaFechamento(int contrato, int fechamento, DateTime dataInicio, DateTime dataFim)
        {
            var result = ConsultarCargasParaFechamento(contrato, fechamento, dataInicio, dataFim);

            return result.Count();
        }

        public void DeletarPorCarga(int codigoCarga)
        {
            if (UnitOfWork.IsActiveTransaction())
            {
                UnitOfWork.Sessao
                    .CreateQuery("delete ContratoSaldoMes where Carga.Codigo = :codigoCarga")
                    .SetInt32("codigoCarga", codigoCarga)
                    .ExecuteUpdate();
            }
            else
            {
                try
                {
                    UnitOfWork.Start();

                    UnitOfWork.Sessao
                        .CreateQuery("delete ContratoSaldoMes where Carga.Codigo = :codigoCarga")
                        .SetInt32("codigoCarga", codigoCarga)
                        .ExecuteUpdate();

                    UnitOfWork.CommitChanges();
                }
                catch
                {
                    UnitOfWork.Rollback();
                    throw;
                }
            }
        }

        public void DeletarPorFechamentoFrete(int codigoFechamentoFrete)
        {
            if (UnitOfWork.IsActiveTransaction())
            {
                UnitOfWork.Sessao
                    .CreateQuery("delete ContratoSaldoMes where FechamentoFrete.Codigo = :codigoFechamentoFrete")
                    .SetInt32("codigoFechamentoFrete", codigoFechamentoFrete)
                    .ExecuteUpdate();
            }
            else
            {
                try
                {
                    UnitOfWork.Start();

                    UnitOfWork.Sessao
                        .CreateQuery("delete ContratoSaldoMes where FechamentoFrete.Codigo = :codigoFechamentoFrete")
                        .SetInt32("codigoFechamentoFrete", codigoFechamentoFrete)
                        .ExecuteUpdate();

                    UnitOfWork.CommitChanges();
                }
                catch
                {
                    UnitOfWork.Rollback();
                    throw;
                }
            }
        }

        #endregion
    }
}
