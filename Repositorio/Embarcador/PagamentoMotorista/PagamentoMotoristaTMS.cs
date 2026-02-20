using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.PagamentoMotorista
{
    public class PagamentoMotoristaTMS : RepositorioBase<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS>
    {
        public PagamentoMotoristaTMS(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public bool ContemPagamentoFinalizadoPorChamado(int codigoChamado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS>();
            var result = from obj in query where obj.Chamado.Codigo == codigoChamado && obj.SituacaoPagamentoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.FinalizadoPagamento select obj;
            return result.Any();
        }

        public bool ContemPagamentoAutorizacaoPendente(int codigoChamado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS>()
                .Where(obj => obj.Chamado.Codigo == codigoChamado && obj.SituacaoPagamentoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.AutorizacaoPendente);

            return query.Any();
        }

        public bool ContemPagamentoIdentico(DateTime data, int codigoMotorista, int codigosTipoPagamento, decimal valor)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS>();
            var result = from obj in query
                         where obj.DataPagamento.Date == data.Date &&
                               obj.Motorista.Codigo == codigoMotorista &&
                               obj.PagamentoMotoristaTipo.Codigo == codigosTipoPagamento &&
                               obj.Valor == valor &&
                               obj.SituacaoPagamentoMotorista != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.Cancelada &&
                                    obj.SituacaoPagamentoMotorista != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.Rejeitada
                         select obj;

            return result.Any();
        }

        public bool ContemPagamentoEmAberto(int codigoMotorista)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS>();
            var result = from obj in query
                         where obj.Motorista.Codigo == codigoMotorista &&
                               obj.SituacaoPagamentoMotorista != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.FinalizadoPagamento
                               && obj.SituacaoPagamentoMotorista != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.Cancelada
                               && obj.SituacaoPagamentoMotorista != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.Rejeitada
                         select obj;
            return result.Any();
        }

        public bool ContemPagamentoEmAcerti(int codigoPagamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoAdiantamento>();
            var result = from obj in query
                         where obj.PagamentoMotoristaTMS.Codigo == codigoPagamento
                         select obj;
            return result.Any();
        }

        public int BuscarProximoNumero()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS>();
            var result = from obj in query select obj;

            if (result.Count() > 0)
                return result.Max(obj => obj.Numero) + 1;
            else
                return 1;
        }

        public decimal BuscarValorPorAcerto(int codigoAcerto, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoMotorista tipoPagamentoMotorista)
        {
            var queryCargas = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoCarga>();
            var resultCargas = from obj in queryCargas where obj.AcertoViagem.Codigo == codigoAcerto select obj.Carga;

            var queryAcertoAdiantamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoAdiantamento>();
            var resultAcertoAdiantamento = from obj in queryAcertoAdiantamento where obj.AcertoViagem.Codigo == codigoAcerto select obj.PagamentoMotoristaTMS;

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS>();
            var result = from obj in query where !resultAcertoAdiantamento.Any(p => p == obj) && resultCargas.Any(p => p == obj.Carga) && obj.PagamentoMotoristaTipo.TipoPagamentoMotorista == tipoPagamentoMotorista && obj.SituacaoPagamentoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.FinalizadoPagamento select obj;

            return result.Sum(o => ((decimal?)o.Valor - (decimal?)o.SaldoDescontado)) ?? 0m;
        }

        public decimal BuscarValorTotalPorAcerto(int codigoAcerto, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoMotorista tipoPagamentoMotorista)
        {
            var queryCargas = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoCarga>();
            var resultCargas = from obj in queryCargas where obj.AcertoViagem.Codigo == codigoAcerto select obj.Carga;

            var queryAcertoAdiantamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoAdiantamento>();
            var resultAcertoAdiantamento = from obj in queryAcertoAdiantamento where obj.AcertoViagem.Codigo == codigoAcerto select obj.PagamentoMotoristaTMS;

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS>();
            var result = from obj in query where !resultAcertoAdiantamento.Any(p => p == obj) && resultCargas.Any(p => p == obj.Carga) && obj.PagamentoMotoristaTipo.TipoPagamentoMotorista == tipoPagamentoMotorista && obj.SituacaoPagamentoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.FinalizadoPagamento select obj;

            return result.Sum(o => ((decimal?)o.Valor)) ?? 0m;
        }

        public List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS> BuscarAdiantamentosMotorista(bool somenteParaAcertoDeViagem, DateTime dataInicial, DateTime? dataFinal, int codigoFuncionario, int codigoAcerto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS>();
            var result = from obj in query select obj;

            if (dataInicial != DateTime.MinValue)
                result = result.Where(obj => obj.DataPagamento >= dataInicial);

            if (dataFinal.HasValue && dataFinal.Value != DateTime.MinValue)
                result = result.Where(obj => obj.DataPagamento <= dataFinal.Value);

            if (codigoFuncionario > 0)
                result = result.Where(obj => obj.Motorista.Codigo == codigoFuncionario);

            if (somenteParaAcertoDeViagem)
            {
                result = result.Where(obj => obj.PagamentoMotoristaTipo.NaoAssociarTipoPagamentoNoAcertoDeViagem == false);
            }

            result = result.Where(obj => obj.SituacaoPagamentoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.FinalizadoPagamento);
            result = result.Where(obj => obj.PagamentoMotoristaTipo.TipoPagamentoMotorista != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoMotorista.Diaria && obj.PagamentoMotoristaTipo.TipoPagamentoMotorista != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoMotorista.Terceiro);

            var queryAcerto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoAdiantamento>();
            var resultAcerto = from obj in queryAcerto where (obj.AcertoViagem.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.EmAntamento || obj.AcertoViagem.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.Fechado) select obj;

            result = result.Where(obj => !(from p in resultAcerto select p.PagamentoMotoristaTMS).Contains(obj));

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS> BuscarDiariasMotorista(DateTime dataInicial, DateTime? dataFinal, int codigoFuncionario, int codigoAcerto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS>();
            var result = from obj in query select obj;

            var queryCargas = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoCarga>();
            var resultCargas = from obj in queryCargas where obj.AcertoViagem.Codigo == codigoAcerto select obj.Carga;

            result = result.Where(obj => resultCargas.Any(p => p == obj.Carga));

            //if (dataInicial != DateTime.MinValue)
            //    result = result.Where(obj => obj.DataPagamento.Date >= dataInicial.Date);

            //if (dataFinal.HasValue && dataFinal.Value != DateTime.MinValue)
            //    result = result.Where(obj => obj.DataPagamento.Date <= dataFinal.Value.Date);

            if (codigoFuncionario > 0)
                result = result.Where(obj => obj.Motorista.Codigo == codigoFuncionario);

            result = result.Where(obj => obj.SituacaoPagamentoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.FinalizadoPagamento);
            result = result.Where(obj => obj.PagamentoMotoristaTipo.TipoPagamentoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoMotorista.Diaria);

            var queryAcerto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoAdiantamento>();
            var resultAcerto = from obj in queryAcerto where (obj.AcertoViagem.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.EmAntamento || obj.AcertoViagem.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.Fechado) select obj;

            result = result.Where(obj => !(from p in resultAcerto select p.PagamentoMotoristaTMS).Contains(obj));

            return result.ToList();
        }

        public decimal BuscarAdiantamentoMotorista(DateTime dataInicial, DateTime dataFinal, int codigoFuncionario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS>();
            var result = from obj in query select obj;

            if (dataInicial != DateTime.MinValue)
                result = result.Where(obj => obj.DataPagamento >= dataInicial);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(obj => obj.DataPagamento <= dataFinal);

            if (codigoFuncionario > 0)
                result = result.Where(obj => obj.Motorista.Codigo == codigoFuncionario);

            result = result.Where(obj => obj.SituacaoPagamentoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.FinalizadoPagamento);

            if (result.Count() > 0)
                return result.Sum(obj => (obj.Valor - obj.SaldoDescontado));
            else
                return 0;
        }

        public List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS> Consultar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoMotorista tipoPagamentoMotorista, bool buscarAdiantamentosSemDataInicialAcertoViagem, bool pagamentosParaAcertoViagem, int codigo, string numeroCarga, List<int> codigosTipoPagamento, int codigoMotorista, int numeroPagamento, DateTime dataInicio, DateTime dataFim, int codigoUsuario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista situacaoPagamentoMotorista, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaPagamentoMotorista etapaPagamentoMotorista, int codigoAcertoViagem, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, bool? pendente = null, List<int> empresa = null)
        {
            var result = _Consultar(tipoPagamentoMotorista, buscarAdiantamentosSemDataInicialAcertoViagem, pagamentosParaAcertoViagem, codigo, numeroCarga, codigosTipoPagamento, codigoMotorista, numeroPagamento, dataInicio, dataFim, codigoUsuario, situacaoPagamentoMotorista, etapaPagamentoMotorista, codigoAcertoViagem, pendente, empresa);

            result = result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);
            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.Timeout(7000).ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoMotorista tipoPagamentoMotorista, bool buscarAdiantamentosSemDataInicialAcertoViagem, bool pagamentosParaAcertoViagem, int codigo, string numeroCarga, List<int> codigosTipoPagamento, int codigoMotorista, int numeroPagamento, DateTime dataInicio, DateTime dataFim, int codigoUsuario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista situacaoPagamentoMotorista, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaPagamentoMotorista etapaPagamentoMotorista, int codigoAcertoViagem, bool? pendente = null, List<int> empresa = null)
        {
            var result = _Consultar(tipoPagamentoMotorista, buscarAdiantamentosSemDataInicialAcertoViagem, pagamentosParaAcertoViagem, codigo, numeroCarga, codigosTipoPagamento, codigoMotorista, numeroPagamento, dataInicio, dataFim, codigoUsuario, situacaoPagamentoMotorista, etapaPagamentoMotorista, codigoAcertoViagem, pendente, empresa);                         
            
            return result.Timeout(7000).ToList().Count();
        }

        public List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS> BuscarPorAdiantamentoAgregado(int numero, DateTime dataPagamento, DateTime dataFinal, int codigoMotorista, double cnpjAgregado, int codigoPagamentoAgregado, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS>();
            var result = from obj in query select obj;
            if (numero > 0)
                result = result.Where(obj => obj.Numero == numero);
            if (codigoMotorista > 0)
                result = result.Where(obj => obj.Motorista.Codigo == codigoMotorista);
            if (cnpjAgregado > 0)
                result = result.Where(obj => obj.Motorista.ClienteTerceiro.CPF_CNPJ == cnpjAgregado);
            if (dataPagamento != DateTime.MinValue)
                result = result.Where(obj => obj.DataPagamento.Date == dataPagamento);
            if (dataFinal != DateTime.MinValue)
                result = result.Where(obj => obj.DataPagamento.Date <= dataFinal);

            var queryPagamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAdiantamento>();
            var resultPagamento = from obj in queryPagamento select obj;
            if (codigoPagamentoAgregado > 0)
                resultPagamento = resultPagamento.Where(obj => obj.PagamentoAgregado.Codigo != codigoPagamentoAgregado && obj.PagamentoAgregado.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado.Rejeitada);
            if (resultPagamento.Count() > 0)
                result = result.Where(obj => !resultPagamento.Select(o => o.PagamentoMotoristaTMS).Contains(obj));

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS> BuscarPagamentoPendenciaMotorista(int numero, DateTime dataPagamento, DateTime dataFinal, int codigoMotorista, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, bool? pendente = null, bool? gerarPendenciaAtivo = null, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista? situacao = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS>();
            var result = from obj in query select obj;
            if (numero > 0)
                result = result.Where(obj => obj.Numero == numero);
            if (codigoMotorista > 0)
                result = result.Where(obj => obj.Motorista.Codigo == codigoMotorista);

            if (dataPagamento != DateTime.MinValue)
                result = result.Where(obj => obj.DataPagamento.Date == dataPagamento);
            if (dataFinal != DateTime.MinValue)
                result = result.Where(obj => obj.DataPagamento.Date <= dataFinal);

            if (pendente != null)
                result = result.Where(obj => obj.Pendente == pendente || obj.Pendente == null);

            if (gerarPendenciaAtivo != null)
                result = result.Where(obj => obj.PagamentoMotoristaTipo.GerarPendenciaAoMotorista == gerarPendenciaAtivo);

            var queryPagamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAdiantamento>();
            var resultPagamento = from obj in queryPagamento select obj;

            if (resultPagamento.Count() > 0)
                result = result.Where(obj => !resultPagamento.Select(o => o.PagamentoMotoristaTMS).Contains(obj));

            if (situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.Todas)
                result = result.Where(obj => obj.SituacaoPagamentoMotorista == situacao);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).Distinct().ToList();
        }
        public List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS> BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS>()
                .Where(o => o.Carga.Codigo == codigoCarga);

            return query.ToList();
        }
        public Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS BuscarPorCargaETipoPagamento(int codigoCarga, int codigoTipoPagamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS>()
                .Where(o => o.Carga.Codigo == codigoCarga && o.PagamentoMotoristaTipo.Codigo == codigoTipoPagamento);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS BuscarFirstOrDefaultPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS>()
                .Where(o => o.Carga.Codigo == codigoCarga);

            return query.FirstOrDefault();
        }

        public bool PagamentoGerado(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS>()
                .Where(o => o.Carga.Codigo == codigoCarga && o.Terceiro != null && o.SituacaoPagamentoMotorista != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.Cancelada
                && o.SituacaoPagamentoMotorista != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.Rejeitada);

            return query.Any();
        }

        public int ContarBuscarPorAdiantamentoAgregado(int numero, DateTime dataPagamento, DateTime dataFinal, int codigoMotorista, double cnpjAgregado, int codigoPagamentoAgregado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS>();
            var result = from obj in query select obj;
            if (numero > 0)
                result = result.Where(obj => obj.Numero == numero);
            if (codigoMotorista > 0)
                result = result.Where(obj => obj.Motorista.Codigo == codigoMotorista);
            if (cnpjAgregado > 0)
                result = result.Where(obj => obj.Motorista.ClienteTerceiro.CPF_CNPJ == cnpjAgregado);
            if (dataPagamento != DateTime.MinValue)
                result = result.Where(obj => obj.DataPagamento.Date == dataPagamento);
            if (dataFinal != DateTime.MinValue)
                result = result.Where(obj => obj.DataPagamento.Date <= dataFinal);

            var queryPagamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAdiantamento>();
            var resultPagamento = from obj in queryPagamento select obj;
            if (codigoPagamentoAgregado > 0)
                resultPagamento = resultPagamento.Where(obj => obj.PagamentoAgregado.Codigo != codigoPagamentoAgregado && obj.PagamentoAgregado.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado.Rejeitada);
            if (resultPagamento.Count() > 0)
                result = result.Where(obj => !resultPagamento.Select(o => o.PagamentoMotoristaTMS).Contains(obj));

            return result.Count();
        }

        public int ContarBuscarPagamentoPendenciaMotorista(int numero, DateTime dataPagamento, DateTime dataFinal, int codigoMotorista, bool? pendente = null, bool? gerarPendenciaAtivo = null, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista? situacao = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS>();
            var result = from obj in query select obj;

            if (numero > 0)
                result = result.Where(obj => obj.Numero == numero);

            if (codigoMotorista > 0)
                result = result.Where(obj => obj.Motorista.Codigo == codigoMotorista);

            if (dataPagamento != DateTime.MinValue)
                result = result.Where(obj => obj.DataPagamento.Date == dataPagamento);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(obj => obj.DataPagamento.Date <= dataFinal);

            if (pendente != null)
                result = result.Where(obj => obj.Pendente == pendente || obj.Pendente == null);

            if (gerarPendenciaAtivo != null)
                result = result.Where(obj => obj.PagamentoMotoristaTipo.GerarPendenciaAoMotorista == gerarPendenciaAtivo);

            var queryPagamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAdiantamento>();
            var resultPagamento = from obj in queryPagamento select obj;

            if (resultPagamento.Count() > 0)
                result = result.Where(obj => !resultPagamento.Select(o => o.PagamentoMotoristaTMS).Contains(obj));

            if (situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.Todas)
                result = result.Where(obj => obj.SituacaoPagamentoMotorista == situacao);

            return result.Count();
        }

        public int ContarConsultaRelatorio(Dominio.ObjetosDeValor.Embarcador.PagamentosMotoristas.FiltroPesquisaPagamentoMotoristaTMS filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consultaPagamentoMotoristaTMS = new ConsultaPagamentoMotoristaTMS().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaPagamentoMotoristaTMS.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.PagamentoMotorista.PagamentoMotorista> ConsultarRelatorio(Dominio.ObjetosDeValor.Embarcador.PagamentosMotoristas.FiltroPesquisaPagamentoMotoristaTMS filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaPagamentoMotoristaTMS = new ConsultaPagamentoMotoristaTMS().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaPagamentoMotoristaTMS.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.PagamentoMotorista.PagamentoMotorista)));

            return consultaPagamentoMotoristaTMS.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.PagamentoMotorista.PagamentoMotorista>();
        }

        public List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS> Consultar(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS>();

            if (codigoCarga > 0)
                query = query.Where(o => o.Carga.Codigo == codigoCarga);

            return ObterLista(query, parametrosConsulta);
        }

        public int ContarConsulta(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga);

            return query.Count();
        }

        public bool ContemPorChamado(int codigoChamado, int codigoMotorista, decimal valor)
        {
            IQueryable<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS>();

            query = query.Where(o => o.Chamado.Codigo == codigoChamado && o.Motorista.Codigo == codigoMotorista && o.Valor == valor);

            return query.Any();
        }

        public bool ContemPorAcerto(int codigoAcerto, int codigoMotorista)
        {
            IQueryable<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS>();

            query = query.Where(o => o.AcertoViagem.Codigo == codigoAcerto && o.Motorista.Codigo == codigoMotorista && o.SituacaoPagamentoMotorista != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.Cancelada && o.SituacaoPagamentoMotorista != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.Rejeitada);

            return query.Any();
        }

        public Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS BuscarPorChamado(int codigoChamado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS>();
            var result = from obj in query where obj.Chamado.Codigo == codigoChamado select obj;
            return result.FirstOrDefault();
        }

        public decimal BuscarIRRFPorRaizCNPJPorPeriodo(string empresaCNPJ, double transportadorTerceiro, DateTime dataInicial, DateTime dataFinal, int codigoPagamentoMotoristaTMS)
        {
            IQueryable<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS>();

            query = query.Where(o => o.SituacaoPagamentoMotorista != SituacaoPagamentoMotorista.Cancelada && o.Carga.Empresa.CNPJ.Substring(0, 8) == empresaCNPJ.Substring(0, 8) && o.Terceiro.CPF_CNPJ == transportadorTerceiro && o.Data >= dataInicial && o.Data < dataFinal && o.Codigo != codigoPagamentoMotoristaTMS);

            return query.Sum(o => (decimal?)o.ValorIRRF) ?? 0m;
        }

        public decimal BuscarValorPorRaizCNPJPorPeriodo(string empresaCNPJ, double transportadorTerceiro, DateTime dataInicial, DateTime dataFinal)
        {
            IQueryable<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS>();

            query = query.Where(o => o.SituacaoPagamentoMotorista != SituacaoPagamentoMotorista.Cancelada && o.Carga.Empresa.CNPJ.Substring(0, 8) == empresaCNPJ.Substring(0, 8) && o.Terceiro.CPF_CNPJ == transportadorTerceiro && o.Data >= dataInicial && o.Data < dataFinal);

            return query.Sum(o => (decimal?)(o.Valor)) ?? 0m;
        }

        public decimal BuscarIRRFPorTerceiroPorPeriodo(double transportadorTerceiro, DateTime dataInicial, DateTime dataFinal, int codigoPagamentoMotoristaTMS)
        {
            IQueryable<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS>();

            query = query.Where(o => o.SituacaoPagamentoMotorista != SituacaoPagamentoMotorista.Cancelada && o.Terceiro.CPF_CNPJ == transportadorTerceiro && o.Data >= dataInicial && o.Data < dataFinal && o.Codigo != codigoPagamentoMotoristaTMS);

            return query.Sum(o => (decimal?)o.ValorIRRF) ?? 0m;
        }

        public decimal BuscarValorPorTerceiroPorPeriodo(double transportadorTerceiro, DateTime dataInicial, DateTime dataFinal)
        {
            IQueryable<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS > ();

            query = query.Where(o => o.SituacaoPagamentoMotorista != SituacaoPagamentoMotorista.Cancelada && o.Terceiro.CPF_CNPJ == transportadorTerceiro && o.Data >= dataInicial && o.Data < dataFinal);

            return query.Sum(o => (decimal?)(o.Valor)) ?? 0m;
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS> _Consultar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoMotorista tipoPagamentoMotorista, bool buscarAdiantamentosSemDataInicialAcertoViagem, bool pagamentosParaAcertoViagem, int codigo, string numeroCarga, List<int> codigosTipoPagamento, int codigoMotorista, int numeroPagamento, DateTime dataInicio, DateTime dataFim, int codigoUsuario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista situacaoPagamentoMotorista, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaPagamentoMotorista etapaPagamentoMotorista, int codigoAcertoViagem, bool? pendente = null, List<int> empresa = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS>();
            var result = from obj in query select obj;


            if (codigo > 0)
                result = result.Where(o => o.Codigo == codigo);

            if (tipoPagamentoMotorista != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoMotorista.Todos)
                result = result.Where(o => o.PagamentoMotoristaTipo.TipoPagamentoMotorista == tipoPagamentoMotorista);

            if (!string.IsNullOrWhiteSpace(numeroCarga))
                result = result.Where(o => o.Carga.CodigoCargaEmbarcador == numeroCarga);

            if (codigosTipoPagamento.Count > 0)
                result = result.Where(o => codigosTipoPagamento.Contains(o.PagamentoMotoristaTipo.Codigo));

            if (codigoMotorista > 0)
                result = result.Where(o => o.Motorista.Codigo == codigoMotorista);

            if (numeroPagamento > 0)
                result = result.Where(o => o.Numero == numeroPagamento);

            if (dataInicio != DateTime.MinValue)
                result = result.Where(obj => obj.DataPagamento.Date >= dataInicio);

            if (dataFim != DateTime.MinValue)
                result = result.Where(obj => obj.DataPagamento.Date <= dataFim);

            if (codigoUsuario > 0)
                result = result.Where(o => o.Usuario.Codigo == codigoUsuario);

            if (situacaoPagamentoMotorista != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.Todas)
                result = result.Where(obj => obj.SituacaoPagamentoMotorista == situacaoPagamentoMotorista);

            if ((int)etapaPagamentoMotorista != 0)
                result = result.Where(obj => obj.EtapaPagamentoMotorista == etapaPagamentoMotorista);

            if (pendente != null)
                result = result.Where(obj => obj.Pendente == pendente || obj.Pendente == null);

            if (empresa != null && empresa.Count > 0)
                result = result.Where(obj => empresa.Contains(obj.Carga.Empresa.Codigo));


            result = result.Where(obj => obj.PagamentoLiberado == true);

            if (pagamentosParaAcertoViagem)
            {
                result = result.Where(obj => obj.PagamentoMotoristaTipo.NaoAssociarTipoPagamentoNoAcertoDeViagem == false);
                result = result.Where(obj => obj.SituacaoPagamentoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.FinalizadoPagamento);
            }

            if (codigoAcertoViagem > 0)
            {
                var queryAcerto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoAdiantamento>();
                var resultAcerto = from obj in queryAcerto where (obj.AcertoViagem.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.EmAntamento || obj.AcertoViagem.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.Fechado) select obj;

                result = result.Where(obj => !(from p in resultAcerto select p.PagamentoMotoristaTMS).Contains(obj));
            }

            return result;
        }

        #endregion
    }
}
