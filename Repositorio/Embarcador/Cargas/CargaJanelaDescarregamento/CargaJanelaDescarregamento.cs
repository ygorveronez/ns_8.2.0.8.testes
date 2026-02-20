using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaJanelaDescarregamento : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento>
    {
        #region Construtores

        public CargaJanelaDescarregamento(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public CargaJanelaDescarregamento(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Privados

        private bool FiltrarPorCanaisVenda(int codigoCentroDescarregamento)
        {
            return new Logistica.PeriodoDescarregamentoCanalVenda(UnitOfWork).ExistePorCentroDescarregamento(codigoCentroDescarregamento);
        }

        private string ObterRaizCpfCnpj(Dominio.Entidades.Cliente remetente)
        {
            string cpfCnpjRemetenteSemFormatacao;

            if ((remetente.Tipo != null) && remetente.Tipo.Equals("E"))
                cpfCnpjRemetenteSemFormatacao = "00000000000000";
            else
                cpfCnpjRemetenteSemFormatacao = ((remetente.Tipo != null) && remetente.Tipo.Equals("J")) ? String.Format(@"{0:00000000000000}", remetente.CPF_CNPJ) : String.Format(@"{0:00000000000}", remetente.CPF_CNPJ);

            return cpfCnpjRemetenteSemFormatacao.Substring(0, 8);
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public int BuscarQuantidadePorDiaRaizCnpj(DateTime data, int codigoGrupoPessoa, int codigoCentroDescarregamento, int codigoCargaDesconsiderar)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento>();
            var queryAgendamentoColeta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta>();

            List<int> cargasNoDia = query
                .Where(o =>
                    o.Carga.Codigo != codigoCargaDesconsiderar &&
                    o.InicioDescarregamento.Date == data.Date &&
                    ((bool?)o.Cancelada ?? false) == false &&
                    o.CentroDescarregamento.Codigo == codigoCentroDescarregamento &&
                    o.Carga.SituacaoCarga != SituacaoCarga.Anulada &&
                    o.Carga.SituacaoCarga != SituacaoCarga.Cancelada &&
                    ((bool?)o.Carga.AgendaExtra ?? false) == false
                )
                .Select(obj => obj.Carga.Codigo)
                .ToList();

            queryAgendamentoColeta = queryAgendamentoColeta.Where(obj => cargasNoDia.Contains(obj.Carga.Codigo) && obj.Remetente.GrupoPessoas.Codigo == codigoGrupoPessoa);

            return queryAgendamentoColeta.Count();
        }

        public int BuscarPorTipoDeCargaDia(int codigoTipoCarga, DateTime dataDescarregamento, int codigoCargaDesconsiderar, int codigoCentroDescarregamento)
        {
            DateTime dataInicioDescarregamento = dataDescarregamento.Date;
            DateTime dataFinalDescarregamento = dataDescarregamento.Date.Add(DateTime.MaxValue.TimeOfDay);

            var consultaCargaJanelaDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento>()
                .Where(o =>
                    o.CentroDescarregamento.Codigo == codigoCentroDescarregamento &&
                    o.Excedente == false &&
                    ((bool?)o.Cancelada ?? false) == false &&
                    o.Carga.Codigo != codigoCargaDesconsiderar &&
                    o.Carga.TipoDeCarga.Codigo == codigoTipoCarga &&
                    o.Carga.SituacaoCarga != SituacaoCarga.Cancelada &&
                    o.Carga.SituacaoCarga != SituacaoCarga.Anulada &&
                    (
                        (dataInicioDescarregamento <= o.InicioDescarregamento && dataFinalDescarregamento >= o.InicioDescarregamento) ||
                        (dataInicioDescarregamento <= o.TerminoDescarregamento && dataFinalDescarregamento >= o.TerminoDescarregamento)
                    )
                );

            var consultaAgendamentoColetaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido>()
                .Where(o =>
                    consultaCargaJanelaDescarregamento.Any(j => j.Carga.Codigo == o.AgendamentoColeta.Carga.Codigo && j.CentroDescarregamento.Destinatario.CPF_CNPJ == o.Pedido.Destinatario.CPF_CNPJ)
                );

            return consultaAgendamentoColetaPedido.Sum(o => (int?)o.VolumesEnviar) ?? 0;
        }

        public List<(int TipoDeCarga, int QuantidadeVolumesNoDia)> BuscarPorTipoDeCargaDia(List<int> codigosTipoCarga, DateTime dataDescarregamento, int codigoCargaDesconsiderar, int codigoCentroDescarregamento)
        {
            DateTime dataInicioDescarregamento = dataDescarregamento.Date;
            DateTime dataFinalDescarregamento = dataDescarregamento.Date.Add(DateTime.MaxValue.TimeOfDay);

            var consultaCargaJanelaDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento>()
                .Where(o =>
                    o.CentroDescarregamento.Codigo == codigoCentroDescarregamento &&
                    o.Excedente == false &&
                    ((bool?)o.Cancelada ?? false) == false &&
                    o.Carga.Codigo != codigoCargaDesconsiderar &&
                    !((bool?)o.Carga.AgendaExtra ?? false) &&
                    o.Carga.SituacaoCarga != SituacaoCarga.Cancelada &&
                    o.Carga.SituacaoCarga != SituacaoCarga.Anulada &&
                    (
                        (dataInicioDescarregamento <= o.InicioDescarregamento && dataFinalDescarregamento >= o.InicioDescarregamento) ||
                        (dataInicioDescarregamento <= o.TerminoDescarregamento && dataFinalDescarregamento >= o.TerminoDescarregamento)
                    )
                );

            var consultaAgendamentoColetaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido>()
                .Where(o =>
                    consultaCargaJanelaDescarregamento.Any(j => j.Carga.Codigo == o.AgendamentoColeta.Carga.Codigo
                    && j.CentroDescarregamento.Destinatario.CPF_CNPJ == o.Pedido.Destinatario.CPF_CNPJ
                    && codigosTipoCarga.Contains(o.Pedido.TipoDeCarga.Codigo))
                );

            return consultaAgendamentoColetaPedido
                .Select(o => ValueTuple.Create(o.Pedido.TipoDeCarga.Codigo, o.VolumesEnviar))
                .ToList();
        }

        public List<(int TipoDeCarga, int QuantidadeVolumesNoDia)> BuscarPorTipoDeCargaAgendamentoDia(int codigoTipoCarga, DateTime dataDescarregamento, int codigoCargaDesconsiderar, int codigoCentroDescarregamento)
        {
            DateTime dataInicioDescarregamento = dataDescarregamento.Date;
            DateTime dataFinalDescarregamento = dataDescarregamento.Date.Add(DateTime.MaxValue.TimeOfDay);

            var consultaCargaJanelaDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento>()
                .Where(o =>
                    o.CentroDescarregamento.Codigo == codigoCentroDescarregamento &&
                    o.Excedente == false &&
                    ((bool?)o.Cancelada ?? false) == false &&
                    o.Carga.Codigo != codigoCargaDesconsiderar &&
                    !((bool?)o.Carga.AgendaExtra ?? false) &&
                    o.Carga.SituacaoCarga != SituacaoCarga.Cancelada &&
                    o.Carga.SituacaoCarga != SituacaoCarga.Anulada &&
                    (
                        (dataInicioDescarregamento <= o.InicioDescarregamento && dataFinalDescarregamento >= o.InicioDescarregamento) ||
                        (dataInicioDescarregamento <= o.TerminoDescarregamento && dataFinalDescarregamento >= o.TerminoDescarregamento)
                    )
                );

            var consultaAgendamentoColeta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta>()
                .Where(o => consultaCargaJanelaDescarregamento.Any(x => x.Carga.Codigo == o.Carga.Codigo) &&
                       o.TipoCarga.Codigo == codigoTipoCarga);

            var consultaAgendamentoColetaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido>()
                .Where(o => consultaAgendamentoColeta.Any(h => h.Codigo == o.AgendamentoColeta.Codigo));

            return consultaAgendamentoColetaPedido
                .Select(o => ValueTuple.Create(o.AgendamentoColeta.TipoCarga.Codigo, o.VolumesEnviar))
                .ToList();
        }

        public decimal BuscarPesoTotalDescarregamentoDia(int codigoCargaDesconsiderar, int codigoCentroDescarregamento, DateTime dataDescarregamento, bool utilizaPesoLiquido)
        {
            DateTime dataInicioDescarregamento = dataDescarregamento.Date;
            DateTime dataFinalDescarregamento = dataDescarregamento.Date.Add(DateTime.MaxValue.TimeOfDay);

            var consultaCargaJanelaDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento>()
                .Where(o =>
                    o.CentroDescarregamento.Codigo == codigoCentroDescarregamento &&
                    o.Excedente == false &&
                    ((bool?)o.Cancelada ?? false) == false &&
                    o.Carga.Codigo != codigoCargaDesconsiderar &&
                    o.Carga.SituacaoCarga != SituacaoCarga.Cancelada &&
                    o.Carga.SituacaoCarga != SituacaoCarga.Anulada &&
                    (
                        (dataInicioDescarregamento <= o.InicioDescarregamento && dataFinalDescarregamento >= o.InicioDescarregamento) ||
                        (dataInicioDescarregamento <= o.TerminoDescarregamento && dataFinalDescarregamento >= o.TerminoDescarregamento)
                    )
                );

            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o =>
                    o.TipoCarregamentoPedido != TipoCarregamentoPedido.TrocaNota &&
                    consultaCargaJanelaDescarregamento.Any(j => j.Carga.Codigo == o.Carga.Codigo && j.CentroDescarregamento.Destinatario.CPF_CNPJ == ((double?)o.Recebedor.CPF_CNPJ ?? o.Pedido.Destinatario.CPF_CNPJ))
                );

            if (utilizaPesoLiquido)
                return consultaCargaPedido.Sum(o => o.PesoLiquido > 0 ? (decimal?)o.PesoLiquido : (decimal?)o.Peso) ?? 0m;
            else
                return consultaCargaPedido.Sum(o => (decimal?)o.Peso) ?? 0m;

        }

        public decimal BuscarPesoTotalDescarregamentoPeriodo(int codigoCargaDesconsiderar, int codigoCentroDescarregamento, DateTime dataDescarregamento, TimeSpan horaInicio, TimeSpan horaTermino, bool utilizarPesoLiquido, List<int> codigosCanaisVenda)
        {
            DateTime dataInicioDescarregamento = dataDescarregamento.Date;
            DateTime dataInicioPeriodo = dataInicioDescarregamento.Add(horaInicio);
            DateTime dataFinalPeriodo = dataInicioDescarregamento.Add(horaTermino);

            var consultaCargaJanelaDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento>()
                .Where(janelaDescarregamento =>
                    janelaDescarregamento.CentroDescarregamento.Codigo == codigoCentroDescarregamento &&
                    janelaDescarregamento.Excedente == false &&
                    ((bool?)janelaDescarregamento.Cancelada ?? false) == false &&
                    janelaDescarregamento.InicioDescarregamento >= dataInicioPeriodo &&
                    janelaDescarregamento.InicioDescarregamento <= dataFinalPeriodo &&
                    janelaDescarregamento.Carga.Codigo != codigoCargaDesconsiderar &&
                    janelaDescarregamento.Carga.SituacaoCarga != SituacaoCarga.Cancelada &&
                    janelaDescarregamento.Carga.SituacaoCarga != SituacaoCarga.Anulada
                );

            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(cargaPedido =>
                    cargaPedido.TipoCarregamentoPedido != TipoCarregamentoPedido.TrocaNota &&
                    consultaCargaJanelaDescarregamento.Any(janelaDescarregamento => janelaDescarregamento.Carga.Codigo == cargaPedido.Carga.Codigo && janelaDescarregamento.CentroDescarregamento.Destinatario.CPF_CNPJ == ((double?)cargaPedido.Recebedor.CPF_CNPJ ?? cargaPedido.Pedido.Destinatario.CPF_CNPJ))
                );

            if (FiltrarPorCanaisVenda(codigoCentroDescarregamento))
            {
                if (codigosCanaisVenda?.Count > 0)
                    consultaCargaPedido = consultaCargaPedido.Where(cargaPedido => codigosCanaisVenda.Contains(cargaPedido.CanalVenda.Codigo));
                else
                    consultaCargaPedido = consultaCargaPedido.Where(cargaPedido => cargaPedido.CanalVenda == null);
            }

            if (utilizarPesoLiquido)
                return consultaCargaPedido.Sum(cargaPedido => cargaPedido.PesoLiquido > 0 ? (decimal?)cargaPedido.PesoLiquido : (decimal?)cargaPedido.Peso) ?? 0m;
            else
                return consultaCargaPedido.Sum(cargaPedido => (decimal?)cargaPedido.Peso) ?? 0m;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> BuscarParaRemover(int codigoCarga)
        {
            var consultaCargaJanelaDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento>()
                .Where(o => o.CentroDescarregamento != null && o.Carga.Codigo == codigoCarga);

            return consultaCargaJanelaDescarregamento.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento BuscarPorCarga(int codigoCarga)
        {
            var consultaCargaJanelaDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento>()
                .Where(o => o.Carga.Codigo == codigoCarga && ((bool?)o.Cancelada ?? false) == false);

            return consultaCargaJanelaDescarregamento
                .Fetch(o => o.Carga)
                .Fetch(o => o.CentroDescarregamento)
                .FirstOrDefault();
        }
        
        public Task<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> BuscarPorCargaAsync(int codigoCarga)
        {
            var consultaCargaJanelaDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento>()
                .Where(o => o.Carga.Codigo == codigoCarga && ((bool?)o.Cancelada ?? false) == false);

            return consultaCargaJanelaDescarregamento
                .Fetch(o => o.Carga)
                .Fetch(o => o.CentroDescarregamento)
                .FirstOrDefaultAsync();
        }

        public bool ExisteJanelaDescarregamentoCanceladaPorCarga(int codigoCarga)
        {
            var consultaCargaJanelaDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento>()
                .Where(o => o.Carga.Codigo == codigoCarga && ((bool?)o.Cancelada ?? false));

            return consultaCargaJanelaDescarregamento.Any();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> BuscarPorCodigos(List<int> codigos)
        {
            var consultaCargaJanelaDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento>()
                .Where(o => codigos.Contains(o.Codigo));

            return consultaCargaJanelaDescarregamento.ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento>> BuscarPorCodigosAsync(List<int> codigos)
        {
            var consultaCargaJanelaDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento>()
                .Where(o => codigos.Contains(o.Codigo));

            return consultaCargaJanelaDescarregamento.ToListAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> BuscarParaNaoComparecimento()
        {
            var consultaCargaJanelaDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento>();

            consultaCargaJanelaDescarregamento = consultaCargaJanelaDescarregamento.Where(obj => obj.InicioDescarregamento < DateTime.Today);
            consultaCargaJanelaDescarregamento = consultaCargaJanelaDescarregamento.Where(obj => obj.Situacao == SituacaoCargaJanelaDescarregamento.AguardandoDescarregamento);
            consultaCargaJanelaDescarregamento = consultaCargaJanelaDescarregamento.Where(obj => obj.CentroDescarregamento.DefinirNaoComparecimentoAutomaticoAposPrazoDataAgendada);

            return consultaCargaJanelaDescarregamento.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> BuscarNaoComparecimentosParaCancelamento(int tempoPermitirReagendamentoHoras, int limiteRegistros)
        {
            var consultaCargaJanelaDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento>()
                .Where(obj =>
                    obj.Situacao == SituacaoCargaJanelaDescarregamento.NaoComparecimento &&
                    obj.InicioDescarregamento.AddHours(-tempoPermitirReagendamentoHoras) <= DateTime.Now &&
                    !obj.Cancelada &&
                    obj.Carga.SituacaoCarga != SituacaoCarga.Cancelada &&
                    obj.Carga.SituacaoCarga != SituacaoCarga.Anulada
                );

            return consultaCargaJanelaDescarregamento.Take(limiteRegistros).ToList();
        }
        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> BuscarPorCarga(List<int> codigosCargas)
        {
            //var consultaCargaJanelaDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento>()
            //    .Where(o => codigosCargas.Contains(o.Carga.Codigo) && ((bool?)o.Cancelada ?? false) == false);

            //return consultaCargaJanelaDescarregamento
            //    .Fetch(o => o.Carga)
            //    .Fetch(o => o.CentroDescarregamento)
            //    .ToList();

            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> result = new List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento>();

            int take = 1000;
            int start = 0;
            while (start < codigosCargas?.Count)
            {
                List<int> tmp = codigosCargas.Skip(start).Take(take).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento>();
                var filter = from obj in query
                             where tmp.Contains(obj.Carga.Codigo) && ((bool?)obj.Cancelada ?? false) == false
                             select obj;

                result.AddRange(filter.Fetch(o => o.Carga)
                                      .Fetch(o => o.CentroDescarregamento)
                                      .ToList());

                start += take;
            }

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> BuscarPorPeriodo(DateTime dataInicial, DateTime dataFinal, int codigoCentroDescarregamento)
        {
            var consultaCargaJanelaDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento>()
                .Where(o =>
                    o.Carga.SituacaoCarga != SituacaoCarga.Cancelada &&
                    o.Carga.SituacaoCarga != SituacaoCarga.Anulada &&
                    o.CentroDescarregamento.Codigo == codigoCentroDescarregamento &&
                    o.Excedente == false &&
                    ((bool?)o.Cancelada ?? false) == false &&
                    o.InicioDescarregamento.Date >= dataInicial.Date &&
                    o.InicioDescarregamento.Date <= dataFinal.Date
                );

            return consultaCargaJanelaDescarregamento
                .Fetch(o => o.Carga).ThenFetch(o => o.DadosSumarizados)
                .Fetch(o => o.CentroDescarregamento).ThenFetch(o => o.Destinatario)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> BuscarTodasPorCarga(int codigoCarga)
        {
            return BuscarTodasPorCarga(codigoCarga, retornarCanceladas: false);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> BuscarTodasPorCarga(int codigoCarga, bool retornarCanceladas)
        {
            var consultaCargaJanelaDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento>()
                .Where(o => o.Carga.Codigo == codigoCarga);

            if (!retornarCanceladas)
                consultaCargaJanelaDescarregamento = consultaCargaJanelaDescarregamento.Where(o => ((bool?)o.Cancelada ?? false) == false);

            return consultaCargaJanelaDescarregamento
                .Fetch(o => o.Carga)
                .Fetch(o => o.CentroDescarregamento).ThenFetch(o => o.Destinatario)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> BuscarTodasPorCargas(List<int> codigosCarga, bool retornarCanceladas)
        {
            var consultaCargaJanelaDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento>()
                .Where(o => codigosCarga.Contains(o.Carga.Codigo));

            if (!retornarCanceladas)
                consultaCargaJanelaDescarregamento = consultaCargaJanelaDescarregamento.Where(o => ((bool?)o.Cancelada ?? false) == false);

            return consultaCargaJanelaDescarregamento
                .Fetch(o => o.Carga)
                .Fetch(o => o.CentroDescarregamento).ThenFetch(o => o.Destinatario)
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento BuscarPorCargaECentroDescarregamento(int codigoCarga, int codigoCentroDescarregamento)
        {
            var consultaCargaJanelaDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento>()
                .Where(o =>
                    o.Carga.Codigo == codigoCarga &&
                    o.CentroDescarregamento.Codigo == codigoCentroDescarregamento
                );

            return consultaCargaJanelaDescarregamento
                .Fetch(o => o.Carga)
                .FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> BuscarPorCargaECentroDescarregamentoAsync(int codigoCarga, int codigoCentroDescarregamento)
        {
            var consultaCargaJanelaDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento>()
                .Where(o =>
                    o.Carga.Codigo == codigoCarga &&
                    o.CentroDescarregamento.Codigo == codigoCentroDescarregamento
                );

            return consultaCargaJanelaDescarregamento
                .Fetch(o => o.Carga)
                .FirstOrDefaultAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> BuscarPorCargasOriginais(int codigoCarga)
        {
            var consultaCargaJanelaDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento>()
                .Where(o => o.Carga.CargaAgrupamento.Codigo == codigoCarga);

            return consultaCargaJanelaDescarregamento.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento BuscarAtivaPorCargaEDestinatario(int codigoCarga, double cpfCnpjDestinatario)
        {
            var consultaCargaJanelaDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento>()
                .Where(o =>
                    o.Carga.Codigo == codigoCarga &&
                    o.CentroDescarregamento.Destinatario.CPF_CNPJ == cpfCnpjDestinatario &&
                    ((bool?)o.Cancelada ?? false) == false
                );

            return consultaCargaJanelaDescarregamento
                .Fetch(o => o.Carga)
                .FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> BuscarAtivaPorCargaEDestinatarioAsync(int codigoCarga, double cpfCnpjDestinatario)
        {
            var consultaCargaJanelaDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento>()
                .Where(o =>
                    o.Carga.Codigo == codigoCarga &&
                    o.CentroDescarregamento.Destinatario.CPF_CNPJ == cpfCnpjDestinatario &&
                    ((bool?)o.Cancelada ?? false) == false
                );

            return consultaCargaJanelaDescarregamento
                .Fetch(o => o.Carga)
                .FirstOrDefaultAsync();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaPeriodo> BuscarCargaPeriodoPorIncidenciaDeHorario(int codigoCargaDesconsiderar, int codigoCentroDescarregamento, DateTime dataInicioDescarregamento, DateTime dataTerminoDescarregamento, List<int> codigosCanaisVenda)
        {
            var consultaCargaJanelaDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento>()
                .Where(o =>
                    o.Carga.Codigo != codigoCargaDesconsiderar &&
                    o.Carga.SituacaoCarga != SituacaoCarga.Cancelada &&
                    o.Carga.SituacaoCarga != SituacaoCarga.Anulada &&
                    !((bool?)o.Carga.AgendaExtra ?? false) &&
                    o.Excedente == false &&
                    ((bool?)o.Cancelada ?? false) == false &&
                    o.CentroDescarregamento.Codigo == codigoCentroDescarregamento &&
                    (
                        (o.InicioDescarregamento >= dataInicioDescarregamento && o.InicioDescarregamento < dataTerminoDescarregamento) ||
                        (o.TerminoDescarregamento > dataInicioDescarregamento && o.TerminoDescarregamento <= dataTerminoDescarregamento)
                    )
                );

            if (FiltrarPorCanaisVenda(codigoCentroDescarregamento))
            {
                var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                    .Where(cargaPedido => cargaPedido.TipoCarregamentoPedido != TipoCarregamentoPedido.TrocaNota);

                if (codigosCanaisVenda?.Count > 0)
                    consultaCargaPedido = consultaCargaPedido.Where(cargaPedido => codigosCanaisVenda.Contains(cargaPedido.CanalVenda.Codigo));
                else
                    consultaCargaPedido = consultaCargaPedido.Where(cargaPedido => cargaPedido.CanalVenda == null);

                consultaCargaJanelaDescarregamento = consultaCargaJanelaDescarregamento
                    .Where(janelaDescarregamento =>
                        consultaCargaPedido.Any(cargaPedido =>
                            cargaPedido.Carga.Codigo == janelaDescarregamento.Carga.Codigo &&
                            janelaDescarregamento.CentroDescarregamento.Destinatario.CPF_CNPJ == ((double?)cargaPedido.Recebedor.CPF_CNPJ ?? cargaPedido.Pedido.Destinatario.CPF_CNPJ)
                        )
                    );
            }

            List<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaPeriodo> cargasPeriodo = consultaCargaJanelaDescarregamento
                .Fetch(x => x.CentroDescarregamento)
                .Select(o => new Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaPeriodo()
                {
                    Codigo = o.Codigo,
                    CodigoCarga = o.Carga.Codigo,
                    TipoCarga = o.Carga.TipoDeCarga.Codigo,
                    DataInicio = o.InicioDescarregamento,
                    DataFim = o.TerminoDescarregamento
                })
                .ToList();

            List<int> codigosCargas = cargasPeriodo.Select(cargaPeriodo => cargaPeriodo.CodigoCarga.Value).ToList();
            List<(double CpfCnpj, string RaizCpfCnpj, int CodigoCarga)> informacoesRemetentes = new List<(double CpfCnpj, string RaizCpfCnpj, int CodigoCarga)>();
            List<(int CodigoCarga, int QuantidadeSku)> agendamentoPedidos = new List<(int CodigoCarga, int QuantidadeSku)>();
            IList<(int CodigoCarga, int CodigoGrupoProduto)> gruposProdutosDominante = new List<(int CodigoCarga, int CodigoGrupoProduto)>();
            List<(int CodigoGrupoPessoas, string RaizCpfCnpj)> gruposPessoas = new List<(int CodigoGrupoPessoas, string RaizCpfCnpj)>();

            if (codigosCargas.Count > 0)
            {
                informacoesRemetentes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta>()
                    .Where(agendamentoColeta => codigosCargas.Contains(agendamentoColeta.Carga.Codigo))
                    .Select(agendamentoColeta => ValueTuple.Create(
                        agendamentoColeta.Remetente != null ? agendamentoColeta.Remetente.CPF_CNPJ : 0,
                        agendamentoColeta.Remetente != null ? ObterRaizCpfCnpj(agendamentoColeta.Remetente) : "",
                        agendamentoColeta.Carga != null ? agendamentoColeta.Carga.Codigo : 0
                    )).ToList();

                agendamentoPedidos = (
                    from o in this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido>()
                    where codigosCargas.Contains(o.AgendamentoColeta.Carga.Codigo)
                    group o by o.AgendamentoColeta.Carga.Codigo into g
                    select new ValueTuple<int, int>(
                        g.Key,
                        g.Sum(t => t.SKU)
                    )
                ).ToList();

                List<string> raizesCpfCnpj = informacoesRemetentes.Select(remetente => remetente.RaizCpfCnpj).ToList();

                gruposPessoas = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasRaizCNPJ>()
                    .Where(o => raizesCpfCnpj.Contains(o.RaizCNPJ))
                    .Select(o => ValueTuple.Create(
                        o.GrupoPessoas.Codigo,
                        o.RaizCNPJ)
                    )
                    .ToList();

                gruposProdutosDominante = BuscarDadosGruposProdutosDominantesPorCargas(cargasPeriodo.Select(x => x.Codigo).ToList());
            }

            foreach (Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaPeriodo cargaPeriodo in cargasPeriodo)
            {
                string raizCpfCnpj = (from remetente in informacoesRemetentes where remetente.CodigoCarga == cargaPeriodo.CodigoCarga select remetente.RaizCpfCnpj).FirstOrDefault();

                cargaPeriodo.QuantidadeSku = (from agendamentoPedido in agendamentoPedidos where agendamentoPedido.CodigoCarga == cargaPeriodo.CodigoCarga select agendamentoPedido.QuantidadeSku).FirstOrDefault();
                cargaPeriodo.CPFCNPJRemetente = (from remetente in informacoesRemetentes where remetente.CodigoCarga == cargaPeriodo.CodigoCarga select remetente.CpfCnpj).FirstOrDefault();
                cargaPeriodo.CodigoGrupoPessoaRemetente = (from grupoPessoas in gruposPessoas where grupoPessoas.RaizCpfCnpj == raizCpfCnpj select grupoPessoas.CodigoGrupoPessoas).FirstOrDefault();
                cargaPeriodo.CodigoGrupoProdutoDominante = (from grupoProdutoDominante in gruposProdutosDominante where grupoProdutoDominante.CodigoCarga == cargaPeriodo.CodigoCarga select grupoProdutoDominante.CodigoGrupoProduto).FirstOrDefault();
            }

            return cargasPeriodo;
        }

        public bool ExisteCargasPorPeriodo(int codigoCentroDescarregamento, Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodo, DateTime data)
        {
            var consultaCargaJanelaDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento>()
                .Where(o =>
                    o.Carga.SituacaoCarga != SituacaoCarga.Cancelada &&
                    o.Carga.SituacaoCarga != SituacaoCarga.Anulada &&
                    !((bool?)o.Carga.AgendaExtra ?? false) &&
                    o.Excedente == false &&
                    ((bool?)o.Cancelada ?? false) == false &&
                    o.InicioDescarregamento.Date == data.Date &&
                    o.CentroDescarregamento.Codigo == codigoCentroDescarregamento
                )
                .ToList()
                .Where(o =>
                    (o.InicioDescarregamento.TimeOfDay >= periodo.HoraInicio && o.InicioDescarregamento.TimeOfDay < periodo.HoraTermino) ||
                    (o.TerminoDescarregamento.TimeOfDay > periodo.HoraInicio && o.TerminoDescarregamento.TimeOfDay <= periodo.HoraTermino)
                );

            int cargasPeriodoCount = consultaCargaJanelaDescarregamento.Count();

            return cargasPeriodoCount < periodo.CapacidadeDescarregamentoSimultaneoTotal;
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Logistica.RetornoJanelaDescarga> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaCargaJanelaDescarregamento filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaJanelaDescarregamento = new ConsultaCargaJanelaDescarregamento().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaJanelaDescarregamento.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.RetornoJanelaDescarga)));

            return consultaJanelaDescarregamento.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.Logistica.RetornoJanelaDescarga>();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> ConsultarPorCargas(List<int> codigosCargas)
        {
            var consultaCargaJanelaDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento>()
                .Where(o => codigosCargas.Contains(o.Carga.Codigo) && ((bool?)o.Cancelada ?? false) == false);

            return consultaCargaJanelaDescarregamento
                .Fetch(o => o.Carga).ThenFetch(o => o.Filial)
                .Fetch(o => o.Carga).ThenFetch(o => o.Empresa)
                .Fetch(o => o.Carga).ThenFetch(o => o.TipoDeCarga)
                .Fetch(o => o.Carga).ThenFetch(o => o.Veiculo)
                .ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaCargaJanelaDescarregamento filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consultaJanelaDescarregamento = new ConsultaCargaJanelaDescarregamento().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaJanelaDescarregamento.SetTimeout(600).UniqueResult<int>();
        }

        public void DeletarPorCodigo(int codigo)
        {
            UnitOfWork.Sessao
                .CreateSQLQuery("delete from T_CARGA_JANELA_DESCARREGAMENTO_COMPOSICAO_HORARIO_DETALHE where DCH_CODIGO in (select ComposicaoHorario.DCH_CODIGO from T_CARGA_JANELA_DESCARREGAMENTO_COMPOSICAO_HORARIO ComposicaoHorario where ComposicaoHorario.CJD_CODIGO = :codigoCargaJanelaDescarregamento)")
                .SetInt32("codigoCargaJanelaDescarregamento", codigo)
                .ExecuteUpdate();

            UnitOfWork.Sessao
                .CreateSQLQuery("delete from T_CARGA_JANELA_DESCARREGAMENTO_COMPOSICAO_HORARIO where CJD_CODIGO = :codigoCargaJanelaDescarregamento")
                .SetInt32("codigoCargaJanelaDescarregamento", codigo)
                .ExecuteUpdate();

            UnitOfWork.Sessao
                .CreateSQLQuery("delete from T_CARGA_JANELA_DESCARREGAMENTO where CJD_CODIGO = :codigoCargaJanelaDescarregamento")
                .SetInt32("codigoCargaJanelaDescarregamento", codigo)
                .ExecuteUpdate();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> BuscarPorDataEFilial(Dominio.ObjetosDeValor.WebService.Pedido.ObterAgendamentos obterAgendamentos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento>();

            var result = query.Where(o => o.Carga.Filial.CodigoFilialEmbarcador == obterAgendamentos.Filial.CodigoIntegracao);

            if (obterAgendamentos.DataInicial != System.DateTime.MinValue)
                result = result.Where(o => o.InicioDescarregamento >= obterAgendamentos.DataInicial);

            if (obterAgendamentos.DataFinal != System.DateTime.MinValue)
            {
                if (obterAgendamentos.DataFinal.TimeOfDay.ToString() == "00:00:00")
                    obterAgendamentos.DataFinal = obterAgendamentos.DataFinal.AddDays(1).AddTicks(-1);
                result = result.Where(o => o.InicioDescarregamento <= obterAgendamentos.DataFinal);
            }

            return result.ToList();
        }

        public string ObterSetoresAgendamentoColeta(Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta)
        {
            string query = $@"select distinct GrupoProduto.GRP_DESCRICAO Setor from T_AGENDAMENTO_COLETA Agendamento
                                join T_AGENDAMENTO_COLETA_PEDIDO AgendamentoPedido on Agendamento.ACO_CODIGO = AgendamentoPedido.ACO_CODIGO
                                join T_PEDIDO_PRODUTO PedidoProduto on AgendamentoPedido.PED_CODIGO = PedidoProduto.PED_CODIGO
                                join T_PRODUTO_EMBARCADOR ProdutoEmbarcador on PedidoProduto.PRO_CODIGO = ProdutoEmbarcador.PRO_CODIGO
                                join T_GRUPO_PRODUTO GrupoProduto on GrupoProduto.GPR_CODIGO = ProdutoEmbarcador.GRP_CODIGO
                             where Agendamento.CAR_CODIGO = {agendamentoColeta.Carga.Codigo}
                             group by GrupoProduto.GRP_DESCRICAO;";

            var nhQuery = this.UnitOfWork.Sessao.CreateSQLQuery(query);
            var resultados = nhQuery.List<string>();

            return resultados.Any() ? string.Join(", ", resultados) : null;
        }

        public IList<(int CodigoCarga, int CodigoGrupoProduto)> BuscarDadosGruposProdutosDominantesPorCargas(List<int> codigosCargas)
        {
            var sql = $@"
                select CodigoCarga, CodigoGrupoProduto
                    from (
                        select
                            CargaPedido.CAR_CODIGO as CodigoCarga,
                            isnull(ProdutoEmbarcador.GRP_CODIGO, 0) as CodigoGrupoProduto,
                            row_number() OVER (PARTITION by CargaPedido.CAR_CODIGO ORDER by sum(PedidoProduto.PRP_QUANTIDADE) desc) AS Ordem
                        from
                            T_CARGA_PEDIDO CargaPedido
                        join
                            T_PEDIDO_PRODUTO PedidoProduto on PedidoProduto.PED_CODIGO = CargaPedido.PED_CODIGO
                        join
                            T_PRODUTO_EMBARCADOR ProdutoEmbarcador on ProdutoEmbarcador.PRO_CODIGO = PedidoProduto.PRO_CODIGO
                        where
                            CargaPedido.CAR_CODIGO in ({string.Join(", ", codigosCargas)})
                        group by
                            CargaPedido.CAR_CODIGO,
                            ProdutoEmbarcador.GRP_CODIGO
                    ) AS Subquery
                    where
                        Subquery.Ordem = 1;";

            var consultaGrupoProdutoDominante = this.SessionNHiBernate.CreateSQLQuery(sql);

            consultaGrupoProdutoDominante.SetResultTransformer(new NHibernate.Transform.AliasToBeanConstructorResultTransformer(typeof((int CodigoCarga, int CodigoGrupoProduto)).GetConstructors().FirstOrDefault()));

            return consultaGrupoProdutoDominante.SetTimeout(600).List<(int CodigoCarga, int CodigoGrupoProduto)>();
        }

        #endregion Métodos Públicos
    }
}
