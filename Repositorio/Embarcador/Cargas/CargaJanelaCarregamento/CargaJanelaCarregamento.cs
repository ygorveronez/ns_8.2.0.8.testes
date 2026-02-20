using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using LinqKit;
using NHibernate.Linq;
using Repositorio.Embarcador.Consulta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaJanelaCarregamento : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>
    {
        #region Construtores

        public CargaJanelaCarregamento(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public CargaJanelaCarregamento(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public List<int> BuscarCodigosPorCotacaoEsgotada(DateTime dataLimite, int limiteRegistros)
        {
            var consultaCargaJanelaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                .Where(o =>
                    o.CargaLiberadaCotacao &&
                    o.Carga.SituacaoCarga != SituacaoCarga.Cancelada &&
                    o.Carga.SituacaoCarga != SituacaoCarga.Anulada &&
                    o.Carga.CargaAgrupamento == null &&
                    o.DataTerminoCotacao <= dataLimite &&
                    (((TipoCargaJanelaCarregamento?)o.Tipo).HasValue == false || o.Tipo == TipoCargaJanelaCarregamento.Carregamento)
                );

            return consultaCargaJanelaCarregamento.Select(o => o.Codigo).Take(limiteRegistros).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> BuscarPorJanelaAgrupamento(int codigoJanelaAgrupamento)
        {
            var consultaCargaJanelaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                .Where(o => o.CargaJanelaCarregamentoAgrupador.Codigo == codigoJanelaAgrupamento && (((TipoCargaJanelaCarregamento?)o.Tipo).HasValue == false || o.Tipo == TipoCargaJanelaCarregamento.Carregamento));

            return consultaCargaJanelaCarregamento.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> BuscarPorCargasOriginais(int codigoCarga)
        {
            var consultaCargaJanelaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                .Where(o => o.Carga.CargaAgrupamento.Codigo == codigoCarga && (((TipoCargaJanelaCarregamento?)o.Tipo).HasValue == false || o.Tipo == TipoCargaJanelaCarregamento.Carregamento));

            return consultaCargaJanelaCarregamento.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento BuscarPorCodigo(int codigoCargaJanelaCarregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>();

            var result = from obj in query where obj.Codigo == codigoCargaJanelaCarregamento select obj;

            return result.
                Fetch(obj => obj.CentroCarregamento).
                Fetch(obj => obj.Carga).
                ThenFetch(obj => obj.DadosSumarizados).
                Fetch(obj => obj.Carga).
                ThenFetch(obj => obj.Carregamento).
                Fetch(obj => obj.Carga).
                ThenFetch(obj => obj.Veiculo).
                Fetch(obj => obj.Carga).
                ThenFetch(obj => obj.TipoDeCarga).
                Fetch(obj => obj.Carga).
                ThenFetch(obj => obj.Empresa).
                Fetch(obj => obj.Carga).
                ThenFetch(obj => obj.TipoOperacao).
                Fetch(obj => obj.Carga).
                ThenFetch(obj => obj.ModeloVeicularCarga).
                FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> BuscarPreCarga(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaJanelaCarregamento filtrosPesquisa)
        {
            if (filtrosPesquisa.SituacaoFaturada)
                return new List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>();

            var consultaCargaJanelaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                .Where(o => (o.Carga == null) && (o.PreCarga != null));

            consultaCargaJanelaCarregamento = ObterConsultaFiltrada(consultaCargaJanelaCarregamento, filtrosPesquisa);

            return consultaCargaJanelaCarregamento
                .Fetch(obj => obj.CentroCarregamento).ThenFetch(o => o.Filial)
                .Fetch(obj => obj.PreCarga).ThenFetch(obj => obj.DadosSumarizados)
                .Fetch(obj => obj.PreCarga).ThenFetch(obj => obj.Filial).ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.PreCarga).ThenFetch(obj => obj.Veiculo)
                .Fetch(obj => obj.PreCarga).ThenFetch(obj => obj.Empresa)
                .Fetch(obj => obj.PreCarga).ThenFetch(obj => obj.TipoDeCarga)
                .Fetch(obj => obj.PreCarga).ThenFetch(obj => obj.ModeloVeicularCarga)
                .Fetch(obj => obj.PreCarga).ThenFetch(obj => obj.LocalCarregamento)
                .OrderBy(o => o.InicioCarregamento)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> BuscarGrupoPessoas(int codigoCentroCarregamento, DateTime dataCarregamento, List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> grupoPessoas, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>();

            var result = from obj in query where obj.CentroCarregamento.Codigo == codigoCentroCarregamento && !obj.Excedente && obj.Carga.SituacaoCarga != SituacaoCarga.Cancelada && obj.Carga.SituacaoCarga != SituacaoCarga.Anulada select obj;

            result = result.Where(obj => obj.Carga.Pedidos.Any(ped => grupoPessoas.Contains(ped.Pedido.Destinatario.GrupoPessoas)));

            if (dataCarregamento != DateTime.MinValue)
                result = result.Where(o => o.InicioCarregamento >= dataCarregamento.Date && o.InicioCarregamento < dataCarregamento.AddDays(1).Date);

            result = result.OrderBy(propOrdena + " " + dirOrdena);

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarGrupoPessoas(int codigoCentroCarregamento, DateTime dataCarregamento, List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> grupoPessoas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>();

            var result = from obj in query where obj.CentroCarregamento.Codigo == codigoCentroCarregamento && !obj.Excedente && obj.Carga.SituacaoCarga != SituacaoCarga.Cancelada && obj.Carga.SituacaoCarga != SituacaoCarga.Anulada select obj;

            result = result.Where(obj => obj.Carga.Pedidos.Any(ped => grupoPessoas.Contains(ped.Pedido.Destinatario.GrupoPessoas)));

            if (dataCarregamento != DateTime.MinValue)
                result = result.Where(o => o.InicioCarregamento >= dataCarregamento.Date && o.InicioCarregamento < dataCarregamento.AddDays(1).Date);

            return result.Count();
        }

        public List<(decimal PesoCarga, bool PossuiVeiculo, int QuantidadeAdicionalVagasOcupadas)> BuscarDadosCargasAlocadasPorDoca(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaJanelaCarregamento filtrosPesquisa)
        {
            var consultaCargaJanelaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                .Where(o => o.Carga.SituacaoCarga != SituacaoCarga.Cancelada && o.Carga.SituacaoCarga != SituacaoCarga.Anulada);

            consultaCargaJanelaCarregamento = ObterConsultaFiltrada(consultaCargaJanelaCarregamento, filtrosPesquisa);

            return consultaCargaJanelaCarregamento
                .Fetch(obj => obj.CargaJanelaCarregamentoAgrupador).ThenFetch(obj => obj.Carga)
                .Select(o => ValueTuple.Create(o.Carga.DadosSumarizados.PesoTotal, o.Carga.Veiculo != null, (int?)o.QuantidadeAdicionalVagasOcupadas ?? 0))
                .ToList();
        }

        public List<(int CodigoTipoOperacao, string TipoOperacao, decimal PesoCarga, bool PossuiVeiculo)> BuscarPesoAlocadoEPercentualAgendado(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaJanelaCarregamento filtrosPesquisa)
        {
            var consultaCargaJanelaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                .Where(o => o.Carga.SituacaoCarga != SituacaoCarga.Cancelada && o.Carga.SituacaoCarga != SituacaoCarga.Anulada);

            consultaCargaJanelaCarregamento = ObterConsultaFiltrada(consultaCargaJanelaCarregamento, filtrosPesquisa);

            return consultaCargaJanelaCarregamento
                .Fetch(obj => obj.CargaJanelaCarregamentoAgrupador).ThenFetch(obj => obj.Carga)
                .Select(o => ValueTuple.Create(o.Carga.TipoOperacao.Codigo, o.Carga.TipoOperacao.Descricao, o.Carga.DadosSumarizados.PesoTotal, o.Carga.Veiculo != null))
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> Buscar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaJanelaCarregamento filtrosPesquisa)
        {
            var consultaCargaJanelaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                .Where(o => o.Carga.SituacaoCarga != SituacaoCarga.Cancelada && o.Carga.SituacaoCarga != SituacaoCarga.Anulada);

            consultaCargaJanelaCarregamento = ObterConsultaFiltrada(consultaCargaJanelaCarregamento, filtrosPesquisa);

            return consultaCargaJanelaCarregamento
                .Fetch(obj => obj.CentroCarregamento).ThenFetch(o => o.Filial)
                .Fetch(obj => obj.Carga).ThenFetch(obj => obj.DadosSumarizados)
                .Fetch(obj => obj.Carga).ThenFetch(obj => obj.Carregamento)
                .Fetch(obj => obj.Carga).ThenFetch(obj => obj.Veiculo)
                .Fetch(obj => obj.Carga).ThenFetch(obj => obj.TipoDeCarga)
                .Fetch(obj => obj.Carga).ThenFetch(obj => obj.Empresa)
                .Fetch(obj => obj.Carga).ThenFetch(obj => obj.TipoOperacao)
                .Fetch(obj => obj.Carga).ThenFetch(obj => obj.ModeloVeicularCarga)
                .Fetch(obj => obj.Carga).ThenFetch(obj => obj.Rota)
                .Fetch(obj => obj.CargaJanelaCarregamentoAgrupador).ThenFetch(obj => obj.Carga)
                .OrderBy(o => o.InicioCarregamento)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> BuscarCargasEmAtrazo()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>();

            var result = from o in query
                         where o.DiasAtraso > 0 && (o.Carga.CargaFechada || o.CargaJanelaCarregamentoAgrupador != null)
                             && o.Carga.SituacaoCarga != SituacaoCarga.AgImpressaoDocumentos
                             && o.Carga.SituacaoCarga != SituacaoCarga.Cancelada
                             && o.Carga.SituacaoCarga != SituacaoCarga.Anulada
                             && o.Carga.SituacaoCarga != SituacaoCarga.Encerrada
                             && o.Carga.SituacaoCarga != SituacaoCarga.LiberadoPagamento
                             && o.Carga.SituacaoCarga != SituacaoCarga.EmTransporte
                             && (((TipoCargaJanelaCarregamento?)o.Tipo).HasValue == false || o.Tipo == TipoCargaJanelaCarregamento.Carregamento)
                         select o;

            return result.OrderByDescending(obj => obj.DiasAtraso).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> BuscarCargasLiberarParaTransportadores(DateTime dataCarregamento)
        {
            var consultaCargaJanelaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                .Where(o =>
                    o.InicioCarregamento >= dataCarregamento.Date &&
                    o.InicioCarregamento <= dataCarregamento.Date.Add(DateTime.MaxValue.TimeOfDay) &&
                    o.Excedente == false &&
                    o.Situacao == SituacaoCargaJanelaCarregamento.AgLiberacaoParaTransportadores &&
                    o.Carga.CargaAgrupamento == null &&
                    (o.Carga.SituacaoCarga != SituacaoCarga.Nova || o.Carga.ExigeNotaFiscalParaCalcularFrete) &&
                    (((TipoCargaJanelaCarregamento?)o.Tipo).HasValue == false || o.Tipo == TipoCargaJanelaCarregamento.Carregamento)
                );

            return consultaCargaJanelaCarregamento.ToList();
        }

        public List<int> BuscarCodigosJanelasCarregamentoLiberarShare()
        {
            var consultaCargaJanelaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                .Where(o =>
                    o.InicioCarregamento >= DateTime.Today.AddDays(-30) &&
                    o.DataLiberacaoShare.Value <= DateTime.Now &&
                    ((bool?)o.ShareLiberado ?? false) == false &&
                    ((TipoCargaJanelaCarregamento?)o.Tipo ?? TipoCargaJanelaCarregamento.Carregamento) == TipoCargaJanelaCarregamento.Carregamento &&
                    o.Carga != null &&
                    o.Carga.Empresa == null &&
                    o.Carga.Rota != null &&
                    o.Carga.SituacaoCarga != SituacaoCarga.Cancelada &&
                    o.Carga.SituacaoCarga != SituacaoCarga.Anulada
                );

            return consultaCargaJanelaCarregamento
                .Select(o => o.Codigo)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> BuscarPorSituacaoEPrazo(SituacaoCargaJanelaCarregamento situacao, DateTime prazoLimite)
        {
            var consultaCargaJanelaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                .Where(o =>
                    o.Excedente == false &&
                    o.Situacao == situacao &&
                    o.InicioCarregamento <= prazoLimite &&
                    o.Carga.CalculandoFrete == false &&
                    o.Carga.problemaAverbacaoCTe == false &&
                    o.Carga.SituacaoCarga == SituacaoCarga.AgTransportador &&
                    (o.Carga.CargaFechada || o.CargaJanelaCarregamentoAgrupador != null) &&
                    (o.Carga.SituacaoAlteracaoFreteCarga == SituacaoAlteracaoFreteCarga.NaoInformada || o.Carga.SituacaoAlteracaoFreteCarga == SituacaoAlteracaoFreteCarga.Aprovada) &&
                    (o.Carga.TipoOperacao == null || o.Carga.TipoOperacao.LiberarCargaAutomaticamenteParaFaturamentoAposPrazoEsgotadoJanelaCarregamento) &&
                    (((TipoCargaJanelaCarregamento?)o.Tipo).HasValue == false || o.Tipo == TipoCargaJanelaCarregamento.Carregamento)
                );

            return consultaCargaJanelaCarregamento.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> BuscarCargaLiberdasPeloTransportador(SituacaoCargaJanelaCarregamento situacao, DateTime prazoLimite)
        {
            var consultaCargaJanelaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                .Where(o =>
                    o.LiberaradaParaFaturamentePeloTransportador &&
                    o.Situacao == situacao &&
                    o.DataProximaSituacao <= prazoLimite &&
                    o.Carga.CalculandoFrete == false &&
                    o.Carga.problemaAverbacaoCTe == false &&
                    (o.Carga.SituacaoAlteracaoFreteCarga == SituacaoAlteracaoFreteCarga.NaoInformada || o.Carga.SituacaoAlteracaoFreteCarga == SituacaoAlteracaoFreteCarga.Aprovada) &&
                    (o.Carga.TipoOperacao == null || o.Carga.TipoOperacao.LiberarCargaAutomaticamenteParaFaturamentoAposPrazoEsgotadoJanelaCarregamento) &&
                    (
                        (o.Carga.SituacaoCarga == SituacaoCarga.AgTransportador && !o.Carga.ExigeNotaFiscalParaCalcularFrete && o.Carga.CargaFechada) ||
                        (o.Carga.SituacaoCarga == SituacaoCarga.CalculoFrete && o.Carga.ExigeNotaFiscalParaCalcularFrete && !o.Carga.PossuiPendencia && !o.Carga.CalculandoFrete && !o.Carga.PendenteGerarCargaDistribuidor && !o.Carga.AguardandoEmissaoDocumentoAnterior && o.Carga.CargaFechada)
                    ) &&
                    (((TipoCargaJanelaCarregamento?)o.Tipo).HasValue == false || o.Tipo == TipoCargaJanelaCarregamento.Carregamento)
                );

            return consultaCargaJanelaCarregamento.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> BuscarPorCodigos(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                .Where(obj => codigos.Contains(obj.Codigo));

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> BuscarParaRemover(int codigoCarga)
        {
            var consultaCargaJanelaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                .Where(o => o.CentroCarregamento != null && (o.Carga.Codigo == codigoCarga || o.Carga.CargaVinculada.Codigo == codigoCarga));

            return consultaCargaJanelaCarregamento.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento BuscarPorPreCarga(int codigoPreCarga)
        {
            var consultaCargaJanelaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                .Where(o => o.PreCarga.Codigo == codigoPreCarga && (((TipoCargaJanelaCarregamento?)o.Tipo).HasValue == false || o.Tipo == TipoCargaJanelaCarregamento.Carregamento));

            return consultaCargaJanelaCarregamento.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento BuscarPorCarga(int codigoCarga)
        {
            var consultaCargaJanelaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                .Where(o => o.Carga.Codigo == codigoCarga && (((TipoCargaJanelaCarregamento?)o.Tipo).HasValue == false || o.Tipo == TipoCargaJanelaCarregamento.Carregamento));

            return consultaCargaJanelaCarregamento.Fetch(obj => obj.CentroCarregamento).FirstOrDefault();
        }
        public Task<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> BuscarPorCargaAsync(int codigoCarga)
        {
            var consultaCargaJanelaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                .Where(o => o.Carga.Codigo == codigoCarga && (((TipoCargaJanelaCarregamento?)o.Tipo).HasValue == false || o.Tipo == TipoCargaJanelaCarregamento.Carregamento));

            return consultaCargaJanelaCarregamento.Fetch(obj => obj.CentroCarregamento).FirstOrDefaultAsync();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento BuscarPorCargaComFetchCargaCentroDescarregamento(int codigoCarga)
        {
            var consultaCargaJanelaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                .Where(o => o.Carga.Codigo == codigoCarga && (((TipoCargaJanelaCarregamento?)o.Tipo).HasValue == false || o.Tipo == TipoCargaJanelaCarregamento.Carregamento));

            return consultaCargaJanelaCarregamento
                .Fetch(obj => obj.CentroCarregamento)
                .Fetch(obj => obj.Carga).ThenFetch(obj => obj.TipoOperacao)
                .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento BuscarPorCargaECentroDescarregamento(int codigoCarga, int codigoCentroDescarregamento)
        {
            var consultaCargaJanelaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                .Where(o => o.Carga.Codigo == codigoCarga && o.CentroCarregamento.Codigo == codigoCentroDescarregamento && o.Tipo == TipoCargaJanelaCarregamento.Descarregamento);

            return consultaCargaJanelaCarregamento.FirstOrDefault();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Logistica.CargaJanelaCarregamentoDados> BuscarDadosDescarregamentosPorCarga(int codigoCarga)
        {
            return BuscarDadosDescarregamentosPorCargas(new List<int>() { codigoCarga });
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Logistica.CargaJanelaCarregamentoDados> BuscarDadosDescarregamentosPorCargas(List<int> codigosCargas)
        {
            var consultaCargaJanelaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                .Where(o =>
                    codigosCargas.Contains(o.Carga.Codigo) &&
                    o.CentroCarregamento != null &&
                    o.Tipo == TipoCargaJanelaCarregamento.Descarregamento
                );

            return consultaCargaJanelaCarregamento
                .Select(o => new Dominio.ObjetosDeValor.Embarcador.Logistica.CargaJanelaCarregamentoDados()
                {
                    CodigoCarga = o.Carga.Codigo,
                    CodigoCentroCarregamento = o.CentroCarregamento.Codigo,
                    DescricaoCentroCarregamento = o.CentroCarregamento.Descricao,
                    Excedente = o.Excedente,
                    InicioCarregamento = o.InicioCarregamento
                })
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> BuscarPorCargas(List<int> codigoCargas)
        {
            //List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> listaRetornar = new List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>();

            //while (codigoCargas.Count > 0)
            //{
            //    var consultaCargaJanelaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
            //        .Where(o => codigoCargas.Take(2100).ToList().Contains(o.Carga.Codigo) && (((TipoCargaJanelaCarregamento?)o.Tipo).HasValue == false || o.Tipo == TipoCargaJanelaCarregamento.Carregamento));

            //    codigoCargas = codigoCargas.Skip(2100).ToList();
            //    listaRetornar.AddRange(consultaCargaJanelaCarregamento.ToList());
            //}

            //return listaRetornar;

            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> result = new List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>();

            int take = 1000;
            int start = 0;
            while (start < codigoCargas?.Count)
            {
                List<int> tmp = codigoCargas.Skip(start).Take(take).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>();
                var filter = from obj in query
                             where tmp.Contains(obj.Carga.Codigo) &&
                                   (((TipoCargaJanelaCarregamento?)obj.Tipo).HasValue == false || obj.Tipo == TipoCargaJanelaCarregamento.Carregamento)
                             select obj;

                result.AddRange(filter.ToList());

                start += take;
            }

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> BuscarPorPreCargas(List<int> codigoPreCargas)
        {
            var consultaCargaJanelaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                .Where(o => codigoPreCargas.Contains(o.PreCarga.Codigo) && (((TipoCargaJanelaCarregamento?)o.Tipo).HasValue == false || o.Tipo == TipoCargaJanelaCarregamento.Carregamento));

            return consultaCargaJanelaCarregamento.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento BuscarPorProtocoloCarga(int protocoloCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> consultaCargaJanelaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                .Where(o => o.Carga.Protocolo == protocoloCarga && (((TipoCargaJanelaCarregamento?)o.Tipo).HasValue == false || o.Tipo == TipoCargaJanelaCarregamento.Carregamento));

            return consultaCargaJanelaCarregamento.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento BuscarPorCodigoEmbarcadorDeCargaEFilial(string codigoCargaEmbarcador, string codigoFilialEmbarcador)
        {
            var consultaCargaJanelaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                .Where(o =>
                    o.Carga.CodigoCargaEmbarcador == codigoCargaEmbarcador &&
                    o.CentroCarregamento.Filial.CodigoFilialEmbarcador == codigoFilialEmbarcador &&
                    o.Carga.CargaFechada == true &&
                    o.Carga.SituacaoCarga != SituacaoCarga.Cancelada &&
                    o.Carga.SituacaoCarga != SituacaoCarga.Anulada
                );

            return consultaCargaJanelaCarregamento.FirstOrDefault();
        }

        public decimal BuscarPesoTotalCarregamentoDia(int codigoCargaJanelaCarregamento, int codigoCentroCarregamento, DateTime dataCarregamento)
        {
            DateTime dataInicioCarregamento = dataCarregamento.Date;
            DateTime dataFinalCarregamento = dataCarregamento.Date.Add(DateTime.MaxValue.TimeOfDay);

            var consultaCargaJanelaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                .Where(o =>
                    o.Codigo != codigoCargaJanelaCarregamento &&
                    o.CentroCarregamento.Codigo == codigoCentroCarregamento &&
                    o.Excedente == false &&
                    o.InicioCarregamento >= dataInicioCarregamento &&
                    o.InicioCarregamento <= dataFinalCarregamento &&
                    (
                        (o.Carga == null && o.PreCarga.SituacaoPreCarga != SituacaoPreCarga.Cancelada) ||
                        (o.Carga != null && o.Carga.SituacaoCarga != SituacaoCarga.Cancelada && o.Carga.SituacaoCarga != SituacaoCarga.Anulada)
                    )
                );

            decimal pesoTotalCargas = consultaCargaJanelaCarregamento.Where(o => o.Carga != null && o.Carga.DadosSumarizados != null).Sum(o => (o.Peso > 0) ? o.Peso : (decimal?)o.Carga.DadosSumarizados.PesoTotal) ?? 0m;
            decimal pesoTotalPreCargas = consultaCargaJanelaCarregamento.Where(o => o.Carga == null && o.PreCarga.DadosSumarizados != null).Sum(o => (o.Peso > 0) ? o.Peso : (decimal?)o.PreCarga.DadosSumarizados.PesoTotal) ?? 0m;

            return (pesoTotalCargas + pesoTotalPreCargas);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> BuscarCarregamentoDia(int codigoCargaJanelaCarregamento, int codigoCentroCarregamento, DateTime dataCarregamento)
        {
            DateTime dataInicioCarregamento = dataCarregamento.Date;
            DateTime dataFinalCarregamento = dataCarregamento.Date.Add(DateTime.MaxValue.TimeOfDay);

            var consultaCargaJanelaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                .Where(o =>
                    o.Codigo != codigoCargaJanelaCarregamento &&
                    o.CentroCarregamento.Codigo == codigoCentroCarregamento &&
                    o.Excedente == false &&
                    o.InicioCarregamento >= dataInicioCarregamento &&
                    o.InicioCarregamento <= dataFinalCarregamento &&
                    (
                        (o.Carga == null && o.PreCarga.SituacaoPreCarga != SituacaoPreCarga.Cancelada) ||
                        (o.Carga != null && o.Carga.SituacaoCarga != SituacaoCarga.Cancelada && o.Carga.SituacaoCarga != SituacaoCarga.Anulada)
                    )
                );

            return consultaCargaJanelaCarregamento.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> BuscarCarregamentoPeriodo(int codigoCargaJanelaCarregamento, int codigoCentroCarregamento, DateTime dataCarregamento, TimeSpan horaInicio, TimeSpan horaTermino)
        {
            DateTime dataInicioCarregamento = dataCarregamento.Date;
            DateTime dataInicioPeriodo = dataInicioCarregamento.Add(horaInicio);
            DateTime dataFinalPeriodo = dataInicioCarregamento.Add(horaTermino);

            var consultaCargaJanelaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                .Where(o =>
                    o.Codigo != codigoCargaJanelaCarregamento &&
                    o.CentroCarregamento.Codigo == codigoCentroCarregamento &&
                    o.Excedente == false &&
                    o.InicioCarregamento >= dataInicioPeriodo &&
                    o.InicioCarregamento <= dataFinalPeriodo &&
                    (
                        (o.Carga == null && o.PreCarga.SituacaoPreCarga != SituacaoPreCarga.Cancelada) ||
                        (o.Carga != null && o.Carga.SituacaoCarga != SituacaoCarga.Cancelada && o.Carga.SituacaoCarga != SituacaoCarga.Anulada)
                    )
                );

            return consultaCargaJanelaCarregamento.ToList();
        }

        public decimal BuscarPesoTotalCarregamentoPeriodo(int codigoCargaJanelaCarregamento, int codigoCentroCarregamento, DateTime dataCarregamento, TimeSpan horaInicio, TimeSpan horaTermino)
        {
            DateTime dataInicioCarregamento = dataCarregamento.Date;
            DateTime dataInicioPeriodo = dataInicioCarregamento.Add(horaInicio);
            DateTime dataFinalPeriodo = dataInicioCarregamento.Add(horaTermino);

            var consultaCargaJanelaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                .Where(o =>
                    o.Codigo != codigoCargaJanelaCarregamento &&
                    o.CentroCarregamento.Codigo == codigoCentroCarregamento &&
                    o.Excedente == false &&
                    o.InicioCarregamento >= dataInicioPeriodo &&
                    o.InicioCarregamento <= dataFinalPeriodo &&
                    (
                        (o.Carga == null && o.PreCarga.SituacaoPreCarga != SituacaoPreCarga.Cancelada) ||
                        (o.Carga != null && o.Carga.SituacaoCarga != SituacaoCarga.Cancelada && o.Carga.SituacaoCarga != SituacaoCarga.Anulada)
                    )
                );

            decimal pesoTotalCargas = consultaCargaJanelaCarregamento.Where(o => o.Carga != null && o.Carga.DadosSumarizados != null).Sum(o => (o.Peso > 0) ? o.Peso : (decimal?)o.Carga.DadosSumarizados.PesoTotal) ?? 0m;
            decimal pesoTotalPreCargas = consultaCargaJanelaCarregamento.Where(o => o.Carga == null && o.PreCarga.DadosSumarizados != null).Sum(o => (o.Peso > 0) ? o.Peso : (decimal?)o.PreCarga.DadosSumarizados.PesoTotal) ?? 0m;

            return (pesoTotalCargas + pesoTotalPreCargas);
        }

        public decimal BuscarVolumeTotalCarregamentoDia(int codigoCargaJanelaCarregamento, int codigoCentroCarregamento, DateTime dataCarregamento)
        {
            DateTime dataInicioCarregamento = dataCarregamento.Date;
            DateTime dataFinalCarregamento = dataCarregamento.Date.Add(DateTime.MaxValue.TimeOfDay);

            var consultaCargaJanelaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                .Where(o =>
                    o.Codigo != codigoCargaJanelaCarregamento &&
                    o.CentroCarregamento.Codigo == codigoCentroCarregamento &&
                    o.Excedente == false &&
                    (
                        (dataInicioCarregamento <= o.InicioCarregamento && dataFinalCarregamento >= o.InicioCarregamento) ||
                        (dataInicioCarregamento <= o.TerminoCarregamento && dataFinalCarregamento >= o.TerminoCarregamento)
                    ) &&
                    (
                        (o.Carga == null && o.PreCarga.SituacaoPreCarga != SituacaoPreCarga.Cancelada) ||
                        (o.Carga != null && o.Carga.SituacaoCarga != SituacaoCarga.Cancelada && o.Carga.SituacaoCarga != SituacaoCarga.Anulada)
                    )
                );

            decimal volumeTotalCargas = consultaCargaJanelaCarregamento.Where(o => o.Carga != null && o.Carga.DadosSumarizados != null).Sum(o => (o.Volume > 0) ? o.Volume : (decimal?)o.Carga.DadosSumarizados.VolumesTotal) ?? 0m;
            decimal volumeTotalPreCargas = consultaCargaJanelaCarregamento.Where(o => o.Carga == null && o.PreCarga.DadosSumarizados != null).Sum(o => (o.Volume > 0) ? o.Volume : (decimal?)o.PreCarga.DadosSumarizados.VolumesTotal) ?? 0m;

            return (volumeTotalCargas + volumeTotalPreCargas);
        }

        public decimal BuscarVolumeTotalCarregamentoPeriodo(int codigoCargaJanelaCarregamento, int codigoCentroCarregamento, DateTime dataCarregamento, TimeSpan horaInicio, TimeSpan horaTermino)
        {
            DateTime dataInicioCarregamento = dataCarregamento.Date;
            DateTime dataInicioPeriodo = dataInicioCarregamento.Add(horaInicio);
            DateTime dataFinalPeriodo = dataInicioCarregamento.Add(horaTermino);

            var consultaCargaJanelaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                .Where(o =>
                    o.Codigo != codigoCargaJanelaCarregamento &&
                    o.CentroCarregamento.Codigo == codigoCentroCarregamento &&
                    o.Excedente == false &&
                    o.InicioCarregamento >= dataInicioPeriodo &&
                    o.InicioCarregamento <= dataFinalPeriodo &&
                    (
                        (o.Carga == null && o.PreCarga.SituacaoPreCarga != SituacaoPreCarga.Cancelada) ||
                        (o.Carga != null && o.Carga.SituacaoCarga != SituacaoCarga.Cancelada && o.Carga.SituacaoCarga != SituacaoCarga.Anulada)
                    )
                );

            decimal volumeTotalCargas = consultaCargaJanelaCarregamento.Where(o => o.Carga != null && o.Carga.DadosSumarizados != null).Sum(o => (o.Volume > 0) ? o.Volume : (decimal?)o.Carga.DadosSumarizados.VolumesTotal) ?? 0m;
            decimal volumeTotalPreCargas = consultaCargaJanelaCarregamento.Where(o => o.Carga == null && o.PreCarga.DadosSumarizados != null).Sum(o => (o.Volume > 0) ? o.Volume : (decimal?)o.PreCarga.DadosSumarizados.VolumesTotal) ?? 0m;

            return (volumeTotalCargas + volumeTotalPreCargas);
        }

        public decimal BuscarCubagemTotalCarregamentoDia(int codigoCargaJanelaCarregamento, int codigoCentroCarregamento, DateTime dataCarregamento)
        {
            DateTime dataInicioCarregamento = dataCarregamento.Date;
            DateTime dataFinalCarregamento = dataCarregamento.Date.Add(DateTime.MaxValue.TimeOfDay);

            var consultaCargaJanelaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                .Where(o =>
                    o.Codigo != codigoCargaJanelaCarregamento &&
                    o.CentroCarregamento.Codigo == codigoCentroCarregamento &&
                    o.Excedente == false &&
                    (
                        (dataInicioCarregamento <= o.InicioCarregamento && dataFinalCarregamento >= o.InicioCarregamento) ||
                        (dataInicioCarregamento <= o.TerminoCarregamento && dataFinalCarregamento >= o.TerminoCarregamento)
                    ) &&
                    (
                        (o.Carga == null && o.PreCarga.SituacaoPreCarga != SituacaoPreCarga.Cancelada) ||
                        (o.Carga != null && o.Carga.SituacaoCarga != SituacaoCarga.Cancelada && o.Carga.SituacaoCarga != SituacaoCarga.Anulada)
                    )
                );

            decimal volumeTotalCargas = consultaCargaJanelaCarregamento.Where(o => o.Carga != null && o.Carga.DadosSumarizados != null).Sum(o => (decimal?)o.Carga.DadosSumarizados.CubagemTotal) ?? 0m;
            decimal volumeTotalPreCargas = consultaCargaJanelaCarregamento.Where(o => o.Carga == null && o.PreCarga.DadosSumarizados != null).Sum(o => (decimal?)o.PreCarga.DadosSumarizados.CubagemTotal) ?? 0m;

            return (volumeTotalCargas + volumeTotalPreCargas);
        }

        public decimal BuscarCubagemTotalCarregamentoPeriodo(int codigoCargaJanelaCarregamento, int codigoCentroCarregamento, DateTime dataCarregamento, TimeSpan horaInicio, TimeSpan horaTermino)
        {
            DateTime dataInicioCarregamento = dataCarregamento.Date;
            DateTime dataInicioPeriodo = dataInicioCarregamento.Add(horaInicio);
            DateTime dataFinalPeriodo = dataInicioCarregamento.Add(horaTermino);

            var consultaCargaJanelaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                .Where(o =>
                    o.Codigo != codigoCargaJanelaCarregamento &&
                    o.CentroCarregamento.Codigo == codigoCentroCarregamento &&
                    o.Excedente == false &&
                    o.InicioCarregamento >= dataInicioPeriodo &&
                    o.InicioCarregamento <= dataFinalPeriodo &&
                    (
                        (o.Carga == null && o.PreCarga.SituacaoPreCarga != SituacaoPreCarga.Cancelada) ||
                        (o.Carga != null && o.Carga.SituacaoCarga != SituacaoCarga.Cancelada && o.Carga.SituacaoCarga != SituacaoCarga.Anulada)
                    )
                );

            decimal volumeTotalCargas = consultaCargaJanelaCarregamento.Where(o => o.Carga != null && o.Carga.DadosSumarizados != null).Sum(o => (decimal?)o.Carga.DadosSumarizados.CubagemTotal) ?? 0m;
            decimal volumeTotalPreCargas = consultaCargaJanelaCarregamento.Where(o => o.Carga == null && o.PreCarga.DadosSumarizados != null).Sum(o => (decimal?)o.PreCarga.DadosSumarizados.CubagemTotal) ?? 0m;

            return (volumeTotalCargas + volumeTotalPreCargas);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> BuscarPorIncidenciaDeHorario(int codigoCargaJanelaCarregamento, int codigoFilial, int codigoCentroCarregamento, DateTime dataInicioCarregamento, DateTime dataFinalCarregamento)
        {
            var consultaCargaJanelaCarregamento = ConsultaPorIncidenciaDeHorario(codigoCargaJanelaCarregamento, codigoFilial, codigoCentroCarregamento, dataInicioCarregamento, dataFinalCarregamento);

            return consultaCargaJanelaCarregamento.ToList();
        }

        public List<(double? Destinatario, int Carga)> BuscarCargasEDestinatarioPorIncidenciaDeHorario(int codigoCargaJanelaCarregamento, int codigoFilial, int codigoCentroCarregamento, DateTime dataInicioCarregamento, DateTime dataFinalCarregamento)
        {
            var consultaCargasDaJanelaCarregamento = ConsultaPorIncidenciaDeHorario(codigoCargaJanelaCarregamento, codigoFilial, codigoCentroCarregamento, dataInicioCarregamento, dataFinalCarregamento)
                                                    .Where(o => o.Carga != null)
                                                    .Select(o => o.Carga);

            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            return consultaCargaPedido
                .Where(o => consultaCargasDaJanelaCarregamento.Contains(o.Carga))
                .Fetch(o => o.Pedido).ThenFetch(o => o.Destinatario)
                .Select(o => ValueTuple.Create((double?)o.Pedido.Destinatario.CPF_CNPJ, o.Carga.Codigo))
                .ToList();
        }

        public string BuscarObservacaoFluxoPatioPorCarga(int codigoCarga)
        {
            var consultaCargaJanelaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                .Where(o => o.Carga.Codigo == codigoCarga && (((TipoCargaJanelaCarregamento?)o.Tipo).HasValue == false || o.Tipo == TipoCargaJanelaCarregamento.Carregamento));

            return consultaCargaJanelaCarregamento.Select(o => o.ObservacaoFluxoPatio).FirstOrDefault();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaPeriodo> BuscarCargaPeriodoPorIncidenciaDeHorario(int codigoCargaJanelaCarregamento, int codigoCentroCarregamento, DateTime dataInicioCarregamento, DateTime dataFinalCarregamento)
        {
            string sql = $@"
                select CargaJanelaCarregamento.CJC_CODIGO Codigo,
                       CargaJanelaCarregamento.CJC_HORARIO_ENCAIXADO Encaixe,
                       CargaJanelaCarregamento.TOP_CODIGO_ENCAIXE TipoOperacaoEncaixe,
                       CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO DataInicio,
                       CargaJanelaCarregamento.CJC_TERMINO_CARREGAMENTO DataFim,
                       isnull(Carga.TCG_CODIGO, Precarga.TCG_CODIGO) TipoCarga,
                       isnull(Carga.TOP_CODIGO, Precarga.TOP_CODIGO) TipoOperacao,
                       isnull(Carga.EMP_CODIGO, Precarga.EMP_CODIGO) Transportador,
                       isnull(Carga.MVC_CODIGO, Precarga.MVC_CODIGO) ModeloVeicularCarga,
                       (
                           select top(1) _pedido.CLI_CODIGO
                             from T_CARGA_PEDIDO _cargaPedido
                             join T_PEDIDO _pedido on _pedido.PED_CODIGO = _cargaPedido.PED_CODIGO
                            where _cargaPedido.CAR_CODIGO = Carga.CAR_CODIGO
                       ) Destinatario
                  from T_CARGA_JANELA_CARREGAMENTO CargaJanelaCarregamento
                  left join T_PRE_CARGA Precarga on Precarga.PCA_CODIGO = CargaJanelaCarregamento.PCA_CODIGO
                  left join T_CARGA Carga on Carga.CAR_CODIGO = CargaJanelaCarregamento.CAR_CODIGO
                 where CargaJanelaCarregamento.CJC_CODIGO <> {codigoCargaJanelaCarregamento}
                   and CargaJanelaCarregamento.CEC_CODIGO = {codigoCentroCarregamento}
                   and CargaJanelaCarregamento.CJC_EXCEDENTE = 0
                   and (
                           (CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO >= '{dataInicioCarregamento.ToString("yyyyMMdd HH:mm:ss")}' and CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO < '{dataFinalCarregamento.ToString("yyyyMMdd HH:mm:ss")}') or
                           (CargaJanelaCarregamento.CJC_TERMINO_CARREGAMENTO > '{dataInicioCarregamento.ToString("yyyyMMdd HH:mm:ss")}' and CargaJanelaCarregamento.CJC_TERMINO_CARREGAMENTO <= '{dataFinalCarregamento.ToString("yyyyMMdd HH:mm:ss")}') or
                           (CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO < '{dataInicioCarregamento.ToString("yyyyMMdd HH:mm:ss")}' and CargaJanelaCarregamento.CJC_TERMINO_CARREGAMENTO > '{dataFinalCarregamento.ToString("yyyyMMdd HH:mm:ss")}')
                       )
                   and (
                           (CargaJanelaCarregamento.CAR_CODIGO is null and Precarga.PCA_SITUACAO <> {(int)SituacaoPreCarga.Cancelada}) or
                           (CargaJanelaCarregamento.CAR_CODIGO is not null and Carga.CAR_SITUACAO <> {(int)SituacaoCarga.Cancelada} and Carga.CAR_SITUACAO <> {(int)SituacaoCarga.Anulada})
                       )";

            var consultaCargaPeriodo = this.SessionNHiBernate.CreateSQLQuery(sql);

            consultaCargaPeriodo.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaPeriodo)));

            return consultaCargaPeriodo.List<Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaPeriodo>();
        }

        public int ContarCargasDiarias(int codigoCargaJanelaCarregamento, int codigoFilial, int codigoCentroCarregamento, int codigoRota, int[] codigosModeloVeiculo, DateTime dataInicioCarregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>();

            var result = from obj in query
                         where obj.Codigo != codigoCargaJanelaCarregamento &&
                               obj.Carga.Filial.Codigo == codigoFilial &&
                               obj.CentroCarregamento.Codigo == codigoCentroCarregamento &&
                               obj.Carga.Rota.Codigo == codigoRota &&
                               obj.InicioCarregamento >= dataInicioCarregamento.Date &&
                               obj.TerminoCarregamento < dataInicioCarregamento.AddDays(1).Date &&
                               codigosModeloVeiculo.Contains(obj.Carga.ModeloVeicularCarga.Codigo) &&
                               obj.Excedente == false &&
                               obj.Carga.SituacaoCarga != SituacaoCarga.Cancelada &&
                               obj.Carga.SituacaoCarga != SituacaoCarga.Anulada
                         select obj;

            return result.Count();
        }

        public int ContarEmReservadasCargasPorRota(int codigoCentroCarregamento, int codigoRota, DateTime dia, int[] modelos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>();

            var result = from obj in query
                         where obj.CentroCarregamento.Codigo == codigoCentroCarregamento &&
                               (obj.Carga == null || obj.Excedente) && obj.PreCarga != null &&
                               (obj.Carga.Rota.Codigo == codigoRota || obj.PreCarga.Rota.Codigo == codigoRota) &&
                               obj.DataReservada >= dia.Date &&
                               obj.DataReservada < dia.AddDays(1) &&
                               (modelos.Contains(obj.PreCarga.ModeloVeicularCarga.Codigo) || modelos.Contains(obj.Carga.ModeloVeicularCarga.Codigo)) &&
                               (obj.CarregamentoReservado)
                         select obj;
            return result.Count();
        }

        public int ContarReservadasDoDia(int codigoCentroCarregamento, int codigoRota, DateTime dia, int[] modelos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>();

            var result = from obj in query
                         where obj.CentroCarregamento.Codigo == codigoCentroCarregamento
                            && obj.PreCarga != null && (obj.Carga == null || obj.Excedente) &&
                            (obj.Carga.Rota.Codigo == codigoRota || obj.PreCarga.Rota.Codigo == codigoRota) &&
                            obj.DataReservada >= dia.Date &&
                            obj.DataReservada < dia.AddDays(1) &&
                            (modelos.Contains(obj.PreCarga.ModeloVeicularCarga.Codigo) || modelos.Contains(obj.Carga.ModeloVeicularCarga.Codigo)) &&
                            (obj.CarregamentoReservado)
                         select obj;

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> BuscarReservadasDoDia(int codigoCentroCarregamento, int codigoRota, DateTime dia, int[] modelos, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>();

            var result = from obj in query
                         where obj.CentroCarregamento.Codigo == codigoCentroCarregamento
                                  && obj.PreCarga != null && (obj.Carga == null || obj.Excedente) &&
                                  (obj.Carga.Rota.Codigo == codigoRota || obj.PreCarga.Rota.Codigo == codigoRota) &&
                                  obj.DataReservada >= dia.Date &&
                                  obj.DataReservada < dia.AddDays(1) &&
                                  (modelos.Contains(obj.PreCarga.ModeloVeicularCarga.Codigo) || modelos.Contains(obj.Carga.ModeloVeicularCarga.Codigo)) &&
                                  (obj.CarregamentoReservado)
                         select obj;

            result = result.OrderBy(propOrdena + " " + dirOrdena);

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result
                .Fetch(obj => obj.PreCarga)
                .Fetch(obj => obj.Carga)
                .ToList();
        }

        public int ContarCargasPorRotaOcupadas(int codigoCentroCarregamento, int codigoRota, DateTime dia, int[] modelos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>();

            var result = from obj in query
                         where obj.CentroCarregamento.Codigo == codigoCentroCarregamento &&
                               obj.Carga.Rota.Codigo == codigoRota &&
                               obj.InicioCarregamento >= dia.Date &&
                               obj.InicioCarregamento < dia.AddDays(1) &&
                               (modelos.Contains(obj.Carga.ModeloVeicularCarga.Codigo) || modelos.Contains(obj.Carga.ModeloVeicularCarga.Codigo)) &&
                               (!obj.Excedente) &&
                               (obj.Carga.SituacaoCarga != SituacaoCarga.Cancelada) &&
                               (obj.Carga.SituacaoCarga != SituacaoCarga.Anulada)
                         select obj.Codigo;

            return result.Count();
        }

        public int ContarCargasPorRota(int codigoCentroCarregamento, int codigoRota, DateTime dia, int[] modelos, int codigoPreCarga, bool somenteReservados, int codigoCargaJanelaCarregamentoDesconsiderar)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>();

            var result = from obj in query
                         where obj.CentroCarregamento.Codigo == codigoCentroCarregamento &&
                               (obj.Carga.Rota.Codigo == codigoRota || obj.PreCarga.Rota.Codigo == codigoRota) &&
                               obj.InicioCarregamento >= dia.Date &&
                               obj.InicioCarregamento < dia.AddDays(1) &&
                               (modelos.Contains(obj.Carga.ModeloVeicularCarga.Codigo) || modelos.Contains(obj.PreCarga.ModeloVeicularCarga.Codigo)) &&
                               (obj.Excedente == false || obj.CarregamentoReservado) &&
                               ((obj.Carga.SituacaoCarga != SituacaoCarga.Cancelada && obj.Carga.SituacaoCarga != SituacaoCarga.Anulada) || obj.Carga == null)
                         select obj;

            if (somenteReservados)
                result = result.Where(obj => obj.CarregamentoReservado);

            if (codigoPreCarga > 0)
                result = result.Where(obj => obj.PreCarga.Codigo != codigoPreCarga);

            if (codigoCargaJanelaCarregamentoDesconsiderar > 0)
                result = result.Where(obj => obj.Codigo != codigoCargaJanelaCarregamentoDesconsiderar);

            return result.Count();
        }

        public int ContarCargasPorCotacaoGanhaAutomaticamente(int codigoTransportador, DateTime dia)
        {
            var consultaCargaJanelaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                .Where(o =>
                    o.CargaCotacaoGanhaAutomaticamente == true &&
                    o.InicioCarregamento >= dia.Date &&
                    o.InicioCarregamento <= dia.Date.Add(DateTime.MaxValue.TimeOfDay) &&
                    o.Carga.Empresa.Codigo == codigoTransportador &&
                    o.Carga.SituacaoCarga != SituacaoCarga.Cancelada &&
                    o.Carga.SituacaoCarga != SituacaoCarga.Anulada &&
                    (((TipoCargaJanelaCarregamento?)o.Tipo).HasValue == false || o.Tipo == TipoCargaJanelaCarregamento.Carregamento)
                );

            return consultaCargaJanelaCarregamento.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> BuscarCargasPorCarregamento(int codigoCarregamento)
        {
            var consultaCargaJanelaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                .Where(o => o.Carga.Carregamento.Codigo == codigoCarregamento);

            return consultaCargaJanelaCarregamento.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento BuscarDataCarga(int codigoCentroCarregamento, string codigoCargaEmbarcador, string codigoPedidoCliente)
        {
            var consultaCargaJanelaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                .Where(o => o.CentroCarregamento.Codigo == codigoCentroCarregamento && !SituacaoCargaHelper.ObterSituacoesCargaCancelada().Contains(o.Carga.SituacaoCarga));

            if (!string.IsNullOrWhiteSpace(codigoCargaEmbarcador))
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.Carga.CodigoCargaEmbarcador == codigoCargaEmbarcador);

            if (!string.IsNullOrWhiteSpace(codigoPedidoCliente))
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.Carga.Pedidos.Any(p => p.Pedido.CodigoPedidoCliente == codigoPedidoCliente));

            return consultaCargaJanelaCarregamento.FirstOrDefault();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Carga.GraficoDirecionamentoOperador> BuscarDirecionamenosPorOperadorGeral(int codigoOperador, int codigoCentroCarregamento, int codigoTransportador, int codigoVeiculo, int codigoFilial, double cpfCnpjDestinatario, DateTime dataInicial, DateTime dataFinal)
        {
            string where = ObterWhereConsultaDirecionamentoPorOperador(codigoOperador, codigoCentroCarregamento, codigoTransportador, codigoVeiculo, codigoFilial, cpfCnpjDestinatario, dataInicial, dataFinal);

            string sqlQuery = @"select ope.FUN_CODIGO Codigo, ope.FUN_NOME Descricao, count(distinct car.CAR_CODIGO) Quantidade from T_CARGA car 
                                inner join T_CARGA_JANELA_CARREGAMENTO cjc on car.CAR_CODIGO = cjc.CAR_CODIGO 
                                inner join T_CARGA_JANELA_CARREGAMENTO_TRANSPORTADOR jct on cjc.CJC_CODIGO = jct.CJC_CODIGO 
                                inner join T_FUNCIONARIO ope on ope.FUN_CODIGO = car.CAR_OPERADOR
                                inner join T_CARGA_PEDIDO cpd on cpd.CAR_CODIGO = car.CAR_CODIGO
                                inner join T_PEDIDO ped on ped.PED_CODIGO = cpd.PED_CODIGO
                                where jct.JCT_SITUACAO in (2,3) and car.CAR_SITUACAO <> 13 " + where +
                              @"group by ope.FUN_CODIGO, ope.FUN_NOME";

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Carga.GraficoDirecionamentoOperador)));

            return query.List<Dominio.ObjetosDeValor.Embarcador.Carga.GraficoDirecionamentoOperador>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Carga.GraficoDirecionamentoOperador> BuscarDirecionamenosPorOperadorTransportador(int codigoOperador, int codigoCentroCarregamento, int codigoTransportador, int codigoVeiculo, int codigoFilial, double cpfCnpjDestinatario, DateTime dataInicial, DateTime dataFinal)
        {
            string where = ObterWhereConsultaDirecionamentoPorOperador(codigoOperador, codigoCentroCarregamento, codigoTransportador, codigoVeiculo, codigoFilial, cpfCnpjDestinatario, dataInicial, dataFinal);

            string sqlQuery = @"select Nome, CNPJ, SUM(QuantidadeDirecionada) QuantidadeDirecionada, SUM(QuantidadeRejeitada) QuantidadeRejeitada from (
                                select emp.EMP_RAZAO Nome, emp.EMP_CNPJ CNPJ, count(distinct car.CAR_CODIGO) QuantidadeDirecionada, 0 QuantidadeRejeitada from T_CARGA car 
                                inner join T_CARGA_JANELA_CARREGAMENTO cjc on car.CAR_CODIGO = cjc.CAR_CODIGO 
                                inner join T_CARGA_JANELA_CARREGAMENTO_TRANSPORTADOR jct on cjc.CJC_CODIGO = jct.CJC_CODIGO 
                                inner join T_EMPRESA emp on emp.EMP_CODIGO = jct.EMP_CODIGO
                                inner join T_CARGA_PEDIDO cpd on cpd.CAR_CODIGO = car.CAR_CODIGO
                                inner join T_PEDIDO ped on ped.PED_CODIGO = cpd.PED_CODIGO
                                where jct.JCT_SITUACAO in (2,3) and car.CAR_SITUACAO <> 13 " + where +
                              @"group by emp.EMP_RAZAO, emp.EMP_CNPJ
                                union
                                select emp.EMP_RAZAO Nome, emp.EMP_CNPJ CNPJ, 0 QuantidadeDirecionada, count(distinct car.CAR_CODIGO) QuantidadeRejeitada from T_CARGA car 
                                inner join T_CARGA_JANELA_CARREGAMENTO cjc on car.CAR_CODIGO = cjc.CAR_CODIGO 
                                inner join T_CARGA_JANELA_CARREGAMENTO_TRANSPORTADOR jct on cjc.CJC_CODIGO = jct.CJC_CODIGO 
                                inner join T_EMPRESA emp on emp.EMP_CODIGO = jct.EMP_CODIGO
                                inner join T_CARGA_PEDIDO cpd on cpd.CAR_CODIGO = car.CAR_CODIGO
                                inner join T_PEDIDO ped on ped.PED_CODIGO = cpd.PED_CODIGO
                                where jct.JCT_SITUACAO in (4) and car.CAR_SITUACAO <> 13 " + where +
                              @"group by emp.EMP_RAZAO, emp.EMP_CNPJ) as a group by Nome, CNPJ order by QuantidadeDirecionada desc, QuantidadeRejeitada desc";


            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Carga.GraficoDirecionamentoOperador)));

            return query.List<Dominio.ObjetosDeValor.Embarcador.Carga.GraficoDirecionamentoOperador>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.DirecionamentoOperador.DirecionamentoOperador> ConsultarRelatorioDirecionamentoOperador(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioDirecionamentoOperador filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var sqlDinamico = ObterSelectConsultaRelatorioDirecionamentoOperador(false, propriedades, filtrosPesquisa, parametroConsulta);

            var query = sqlDinamico.CriarSQLQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Cargas.DirecionamentoOperador.DirecionamentoOperador)));

            return query.List<Dominio.Relatorios.Embarcador.DataSource.Cargas.DirecionamentoOperador.DirecionamentoOperador>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.DirecionamentoOperador.DirecionamentoOperador>> ConsultarRelatorioDirecionamentoOperadorAsync(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioDirecionamentoOperador filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var sqlDinamico = ObterSelectConsultaRelatorioDirecionamentoOperador(false, propriedades, filtrosPesquisa, parametroConsulta);

            var query = sqlDinamico.CriarSQLQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Cargas.DirecionamentoOperador.DirecionamentoOperador)));

            return await query
                .ListAsync<Dominio.Relatorios.Embarcador.DataSource.Cargas.DirecionamentoOperador.DirecionamentoOperador>();
        }

        public int ContarConsultaRelatorioDirecionamentoOperador(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioDirecionamentoOperador filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var sqlDinamico = ObterSelectConsultaRelatorioDirecionamentoOperador(true, propriedades, filtrosPesquisa, null);

            var query = sqlDinamico.CriarSQLQuery(this.SessionNHiBernate);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> BuscarPorCargaDisponibilizarFilaCarregamento(int codigoCentroCarregamento, int codigoModeloVeicularCarga, int prazoAlocarCargaAntesInicioCarregamento, bool transportadorExclusivo)
        {
            var dataHoraAlocacaoInicial = DateTime.Now.AddMinutes(prazoAlocarCargaAntesInicioCarregamento);

            var consultaCargaJanelaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                .Where(o => (
                    (o.Situacao == SituacaoCargaJanelaCarregamento.SemTransportador) &&
                    (o.CentroCarregamento.Codigo == codigoCentroCarregamento) &&
                    (o.Carga.ModeloVeicularCarga.Codigo == codigoModeloVeicularCarga) &&
                    (o.Carga.SituacaoCarga == SituacaoCarga.AgTransportador) &&
                    (o.InicioCarregamento <= dataHoraAlocacaoInicial) &&
                    (o.Excedente == false) &&
                    (((TipoCargaJanelaCarregamento?)o.Tipo).HasValue == false || o.Tipo == TipoCargaJanelaCarregamento.Carregamento)
                ));

            if (transportadorExclusivo)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.TransportadorExclusivo != null);
            else
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.TransportadorExclusivo == null);

            return consultaCargaJanelaCarregamento.OrderBy(o => o.InicioCarregamento).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> ConsultarPorCargaAdicionarFluxoPatio(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaCargaAdicionarFluxoPatio filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaCargaJanelaCarregamento = ConsultarPorCargaAdicionarFluxoPatio(filtrosPesquisa);

            consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento
                .Fetch(o => o.Carga).ThenFetch(o => o.DadosSumarizados);

            return ObterLista(consultaCargaJanelaCarregamento, parametrosConsulta);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> BuscarPorPreCargaDisponibilizarFilaCarregamento(int codigoCentroCarregamento, int codigoModeloVeicularCarga, int prazoAlocarCargaAntesInicioCarregamento)
        {
            var dataHoraAlocacaoInicial = DateTime.Now.AddMinutes(prazoAlocarCargaAntesInicioCarregamento);

            var listaCargaJanelaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                .Where(o => (
                    (o.Situacao == SituacaoCargaJanelaCarregamento.SemTransportador) &&
                    (o.CentroCarregamento.Codigo == codigoCentroCarregamento) &&
                    (o.PreCarga.ModeloVeicularCarga.Codigo == codigoModeloVeicularCarga) &&
                    (o.InicioCarregamento <= dataHoraAlocacaoInicial) &&
                    (o.Excedente == false) &&
                    (o.Carga == null) &&
                    (o.PreCarga.SituacaoPreCarga != SituacaoPreCarga.Cancelada) &&
                    (((TipoCargaJanelaCarregamento?)o.Tipo).HasValue == false || o.Tipo == TipoCargaJanelaCarregamento.Carregamento)
                ))
                .OrderBy(o => o.InicioCarregamento)
                .ToList();

            return listaCargaJanelaCarregamento;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> ConsultarCargaJanelaCarregamento(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaCargaJanelaCarregamento filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaCargaJanelaCarregamento = ConsultarCargaJanelaCarregamento(filtrosPesquisa);

            if (string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeOrdenar))
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.OrderBy("DiasAtraso desc, InicioCarregamento asc");
            else
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.OrderBy(parametrosConsulta.PropriedadeOrdenar);

            if (parametrosConsulta.InicioRegistros > 0)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Skip(parametrosConsulta.InicioRegistros);

            if (parametrosConsulta.LimiteRegistros > 0)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Take(parametrosConsulta.LimiteRegistros);

            return consultaCargaJanelaCarregamento.
                Fetch(obj => obj.CentroCarregamento).
                Fetch(obj => obj.Carga).
                ThenFetch(obj => obj.DadosSumarizados).
                Fetch(obj => obj.Carga).
                ThenFetch(obj => obj.Carregamento).
                Fetch(obj => obj.Carga).
                ThenFetch(obj => obj.Veiculo).
                Fetch(obj => obj.Carga).
                ThenFetch(obj => obj.TipoDeCarga).
                Fetch(obj => obj.Carga).
                ThenFetch(obj => obj.Empresa).
                Fetch(obj => obj.Carga).
                ThenFetch(obj => obj.TipoOperacao).
                Fetch(obj => obj.Carga).
                ThenFetch(obj => obj.ModeloVeicularCarga).
                Fetch(obj => obj.Carga).
                Fetch(obj => obj.CargaJanelaCarregamentoAgrupador).
                ThenFetch(obj => obj.Carga).
                ThenFetch(obj => obj.Rota).ToList();
        }

        public int ContarConsultaCargaJanelaCarregamento(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaCargaJanelaCarregamento filtrosPesquisa)
        {
            var consultaCargaJanelaCarregamento = ConsultarCargaJanelaCarregamento(filtrosPesquisa);

            return consultaCargaJanelaCarregamento.Count();
        }

        public int ContarConsultaPorCargaAdicionarFluxoPatio(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaCargaAdicionarFluxoPatio filtrosPesquisa)
        {
            var consultaCargaJanelaCarregamento = ConsultarPorCargaAdicionarFluxoPatio(filtrosPesquisa);

            return consultaCargaJanelaCarregamento.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> BuscarTodasPorCarga(int codigoCarga)
        {
            var consultaCargaJanelaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                .Where(o => o.Carga.Codigo == codigoCarga);

            return consultaCargaJanelaCarregamento.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento BuscarCarregamentoPorCargaEFilial(int codigoCarga, int codigoFilial)
        {
            var consultaCargaJanelaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                .Where(o => o.Carga.Codigo == codigoCarga && o.CentroCarregamento.Filial.Codigo == codigoFilial && o.Tipo == TipoCargaJanelaCarregamento.Carregamento);

            return consultaCargaJanelaCarregamento.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento BuscarDescarregamentoPorCargaECentro(int codigoCarga, int codigoCentroCarregamento)
        {
            var consultaCargaJanelaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                .Where(o => o.Carga.Codigo == codigoCarga && o.CentroCarregamento.Codigo == codigoCentroCarregamento && o.Tipo == TipoCargaJanelaCarregamento.Descarregamento);

            return consultaCargaJanelaCarregamento.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento BuscarDescarregamentoPorCargaEFilial(int codigoCarga, int codigoFilial)
        {
            var consultaCargaJanelaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                .Where(o => o.Carga.Codigo == codigoCarga && o.CentroCarregamento.Filial.Codigo == codigoFilial && o.Tipo == TipoCargaJanelaCarregamento.Descarregamento);

            return consultaCargaJanelaCarregamento.FirstOrDefault();
        }

        public Task<List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaJanelaCarregamentoTransportador.CargaJanelaCarregamentoTransportadorInteresseCarga>> BuscarCargasJanelaCarregamentoGuaritaAsync(List<int> codigosCarga)
        {
            var consultaCargaJanelaCarregamentoGuarita = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                .Where(o => codigosCarga.Contains(o.Carga.Codigo))
                .Select(x => new Dominio.ObjetosDeValor.Embarcador.Carga.CargaJanelaCarregamentoTransportador.CargaJanelaCarregamentoTransportadorInteresseCarga
                {
                    DataInicioCarregamento = x.InicioCarregamento,
                    CodigoCarga = x.Carga.Codigo,
                });

            return consultaCargaJanelaCarregamentoGuarita.ToListAsync(CancellationToken);
        }

        #endregion Métodos Públicos

        #region Métodos Públicos - Prioridade

        public void AdicionarPrioridade(int codigoCentroCarregamento, DateTime dataCarregamento, int prioridade)
        {
            if (UnitOfWork.IsActiveTransaction())
            {
                UnitOfWork.Sessao
                    .CreateQuery(
                        @"update CargaJanelaCarregamento
                             set Prioridade = (Prioridade + 1)
                           where CentroCarregamento.Codigo = :codigoCentroCarregamento
                             and InicioCarregamento between :dataInicial and :dataFinal
                             and Prioridade >= :prioridade"
                    )
                    .SetParameter("codigoCentroCarregamento", codigoCentroCarregamento)
                    .SetParameter("dataInicial", dataCarregamento.Date)
                    .SetParameter("dataFinal", dataCarregamento.Date.Add(DateTime.MaxValue.TimeOfDay))
                    .SetParameter("prioridade", prioridade)
                    .ExecuteUpdate();
            }
            else
            {
                try
                {
                    UnitOfWork.Start();

                    UnitOfWork.Sessao
                        .CreateQuery(
                            @"update CargaJanelaCarregamento
                                    set Prioridade = (Prioridade + 1)
                                where CentroCarregamento.Codigo = :codigoCentroCarregamento
                                    and InicioCarregamento between :dataInicial and :dataFinal
                                    and Prioridade >= :prioridade"
                        )
                        .SetParameter("codigoCentroCarregamento", codigoCentroCarregamento)
                        .SetParameter("dataInicial", dataCarregamento.Date)
                        .SetParameter("dataFinal", dataCarregamento.Date.Add(DateTime.MaxValue.TimeOfDay))
                        .SetParameter("prioridade", prioridade)
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

        public void AtualizarPrioridadesInferiores(int codigoCentroCarregamento, DateTime dataCarregamento, int prioridadeAtual, int prioridadeAnterior)
        {
            if (UnitOfWork.IsActiveTransaction())
            {
                UnitOfWork.Sessao
                    .CreateQuery(
                        @"update CargaJanelaCarregamento
                             set Prioridade = (Prioridade - 1)
                           where CentroCarregamento.Codigo = :codigoCentroCarregamento
                             and InicioCarregamento between :dataInicial and :dataFinal
                             and Prioridade <= :prioridadeAtual
                             and Prioridade > :prioridadeAnterior"
                    )
                    .SetParameter("codigoCentroCarregamento", codigoCentroCarregamento)
                    .SetParameter("dataInicial", dataCarregamento.Date)
                    .SetParameter("dataFinal", dataCarregamento.Date.Add(DateTime.MaxValue.TimeOfDay))
                    .SetParameter("prioridadeAnterior", prioridadeAnterior)
                    .SetParameter("prioridadeAtual", prioridadeAtual)
                    .ExecuteUpdate();
            }
            else
            {
                try
                {
                    UnitOfWork.Start();

                    UnitOfWork.Sessao
                        .CreateQuery(
                            @"update CargaJanelaCarregamento
                                    set Prioridade = (Prioridade - 1)
                                where CentroCarregamento.Codigo = :codigoCentroCarregamento
                                    and InicioCarregamento between :dataInicial and :dataFinal
                                    and Prioridade <= :prioridadeAtual
                                    and Prioridade > :prioridadeAnterior"
                        )
                        .SetParameter("codigoCentroCarregamento", codigoCentroCarregamento)
                        .SetParameter("dataInicial", dataCarregamento.Date)
                        .SetParameter("dataFinal", dataCarregamento.Date.Add(DateTime.MaxValue.TimeOfDay))
                        .SetParameter("prioridadeAnterior", prioridadeAnterior)
                        .SetParameter("prioridadeAtual", prioridadeAtual)
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

        public void AtualizarPrioridadesSuperiores(int codigoCentroCarregamento, DateTime dataCarregamento, int prioridadeAtual, int prioridadeAnterior)
        {
            if (UnitOfWork.IsActiveTransaction())
            {
                UnitOfWork.Sessao
                    .CreateQuery(
                        @"update CargaJanelaCarregamento
                             set Prioridade = (Prioridade + 1)
                           where CentroCarregamento.Codigo = :codigoCentroCarregamento
                             and InicioCarregamento between :dataInicial and :dataFinal
                             and Prioridade >= :prioridadeAtual
                             and Prioridade < :prioridadeAnterior"
                    )
                    .SetParameter("codigoCentroCarregamento", codigoCentroCarregamento)
                    .SetParameter("dataInicial", dataCarregamento.Date)
                    .SetParameter("dataFinal", dataCarregamento.Date.Add(DateTime.MaxValue.TimeOfDay))
                    .SetParameter("prioridadeAnterior", prioridadeAnterior)
                    .SetParameter("prioridadeAtual", prioridadeAtual)
                    .ExecuteUpdate();
            }
            else
            {
                try
                {
                    UnitOfWork.Start();

                    UnitOfWork.Sessao
                        .CreateQuery(
                            @"update CargaJanelaCarregamento
                                    set Prioridade = (Prioridade + 1)
                                where CentroCarregamento.Codigo = :codigoCentroCarregamento
                                    and InicioCarregamento between :dataInicial and :dataFinal
                                    and Prioridade >= :prioridadeAtual
                                    and Prioridade < :prioridadeAnterior"
                        )
                        .SetParameter("codigoCentroCarregamento", codigoCentroCarregamento)
                        .SetParameter("dataInicial", dataCarregamento.Date)
                        .SetParameter("dataFinal", dataCarregamento.Date.Add(DateTime.MaxValue.TimeOfDay))
                        .SetParameter("prioridadeAnterior", prioridadeAnterior)
                        .SetParameter("prioridadeAtual", prioridadeAtual)
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

        public int BuscarMaiorPrioridade(int codigoCentroCarregamento, DateTime dataCarregamento)
        {
            var consultaCargaJanelaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                .Where(o =>
                    o.CentroCarregamento.Codigo == codigoCentroCarregamento &&
                    o.InicioCarregamento >= dataCarregamento.Date &&
                    o.InicioCarregamento <= dataCarregamento.Date.Add(DateTime.MaxValue.TimeOfDay) &&
                    o.Excedente == false &&
                    (o.Carga == null || (o.Carga.SituacaoCarga != SituacaoCarga.Cancelada && o.Carga.SituacaoCarga != SituacaoCarga.Anulada))
                );

            return consultaCargaJanelaCarregamento.Max(o => (int?)o.Prioridade) ?? 0;
        }

        public int BuscarMenorPrioridadePorCargaOriginal(int codigoCargaAgrupamento, DateTime dataCarregamento)
        {
            var consultaCargaJanelaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                .Where(o =>
                    o.Carga.CargaAgrupamento.Codigo == codigoCargaAgrupamento &&
                    o.InicioCarregamento >= dataCarregamento.Date &&
                    o.InicioCarregamento <= dataCarregamento.Date.Add(DateTime.MaxValue.TimeOfDay)
                );

            return consultaCargaJanelaCarregamento.Min(o => (int?)o.Prioridade) ?? 0;
        }

        public int BuscarPrioridadeAnterior(int codigoCentroCarregamento, DateTime dataCarregamento, int codigoCargaJanelaCarregamentoDesconsiderar)
        {
            var consultaCargaJanelaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                .Where(o =>
                    o.Codigo != codigoCargaJanelaCarregamentoDesconsiderar &&
                    o.CentroCarregamento.Codigo == codigoCentroCarregamento &&
                    o.InicioCarregamento >= dataCarregamento.Date &&
                    o.InicioCarregamento <= dataCarregamento &&
                    o.Excedente == false &&
                    (o.Carga == null || (o.Carga.SituacaoCarga != SituacaoCarga.Cancelada && o.Carga.SituacaoCarga != SituacaoCarga.Anulada))
                );

            return consultaCargaJanelaCarregamento
                .OrderByDescending(o => o.InicioCarregamento)
                .OrderByDescending(o => o.Prioridade)
                .Select(o => o.Prioridade)
                .FirstOrDefault();
        }

        public int BuscarPrioridadeAtual(int codigoCargaJanelaCarregamento)
        {
            var consultaCargaJanelaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                .Where(o => o.Codigo == codigoCargaJanelaCarregamento);

            return consultaCargaJanelaCarregamento
                .Select(o => o.Prioridade)
                .FirstOrDefault();
        }

        public int BuscarProximaPrioridade(int codigoCentroCarregamento, DateTime dataCarregamento)
        {
            var consultaCargaJanelaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                .Where(o =>
                    o.CentroCarregamento.Codigo == codigoCentroCarregamento &&
                    o.InicioCarregamento <= dataCarregamento.Date.Add(DateTime.MaxValue.TimeOfDay) &&
                    o.InicioCarregamento > dataCarregamento &&
                    o.Excedente == false &&
                    (o.Carga == null || (o.Carga.SituacaoCarga != SituacaoCarga.Cancelada && o.Carga.SituacaoCarga != SituacaoCarga.Anulada))
                );

            return consultaCargaJanelaCarregamento
                .OrderBy(o => o.InicioCarregamento)
                .OrderBy(o => o.Prioridade)
                .Select(o => o.Prioridade)
                .FirstOrDefault();
        }

        public void RemoverPrioridade(int codigoCentroCarregamento, DateTime dataCarregamento, int prioridade)
        {
            if (UnitOfWork.IsActiveTransaction())
            {
                UnitOfWork.Sessao
                    .CreateQuery(
                        @"update CargaJanelaCarregamento
                             set Prioridade = (Prioridade - 1)
                           where CentroCarregamento.Codigo = :codigoCentroCarregamento
                             and InicioCarregamento between :dataInicial and :dataFinal
                             and Prioridade > :prioridade"
                    )
                    .SetParameter("codigoCentroCarregamento", codigoCentroCarregamento)
                    .SetParameter("dataInicial", dataCarregamento.Date)
                    .SetParameter("dataFinal", dataCarregamento.Date.Add(DateTime.MaxValue.TimeOfDay))
                    .SetParameter("prioridade", prioridade)
                    .ExecuteUpdate();
            }
            else
            {
                try
                {
                    UnitOfWork.Start();

                    UnitOfWork.Sessao
                        .CreateQuery(
                            @"update CargaJanelaCarregamento
                                    set Prioridade = (Prioridade - 1)
                                where CentroCarregamento.Codigo = :codigoCentroCarregamento
                                    and InicioCarregamento between :dataInicial and :dataFinal
                                    and Prioridade > :prioridade"
                        )
                        .SetParameter("codigoCentroCarregamento", codigoCentroCarregamento)
                        .SetParameter("dataInicial", dataCarregamento.Date)
                        .SetParameter("dataFinal", dataCarregamento.Date.Add(DateTime.MaxValue.TimeOfDay))
                        .SetParameter("prioridade", prioridade)
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

        #endregion Métodos Públicos - Prioridade

        #region Métodos Públicos - Agendamento Entrega Pedido

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> BuscarJanelasNaoConfirmadasPeloTransportadorPorCargas(List<int> codigosCargas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                .Where(cjc => codigosCargas.Contains(cjc.Carga.Codigo))
                .Where(cjc => cjc.Carga.Rota != null)
                .Where(cjc => (((TipoCargaJanelaCarregamento?)cjc.Tipo).HasValue == false || cjc.Tipo == TipoCargaJanelaCarregamento.Carregamento))
                .Where(cjc => cjc.Situacao != SituacaoCargaJanelaCarregamento.ProntaParaCarregamento);

            return query
                .Fetch(cjc => cjc.Carga)
                .ThenFetch(cjc => cjc.Rota)
                .Distinct()
                .ToList();
        }

        #endregion Métodos Públicos - Agendamento Entrega Pedido

        #region Métodos Públicos - Automatização de Não Comparecimento

        public List<int> BuscarCodigosParaMarcarComoNaoComparecimentoPorCargaNaoAgendada(int codigoCentroCarregamento, DateTime dataTolerancia, DateTime dataCriacaoCargaInicial)
        {
            List<TipoMensagemAlerta> tiposMensagemAlertaNaoComparecimento = GatilhoAutomatizacaoNaoComparecimentoHelper.ObterTiposMensagemAlerta();

            var consultaMensagemAlertaNaoComparecimento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MensagemAlertaCarga>()
                .Where(o => o.Confirmada == false && tiposMensagemAlertaNaoComparecimento.Contains(o.Tipo));

            var consultaCargaNFe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaNFe>();

            var consultaCargaJanelaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                .Where(o =>
                    o.CentroCarregamento.Codigo == codigoCentroCarregamento &&
                    o.InicioCarregamento < dataTolerancia &&
                    o.Excedente == true &&
                    o.Carga != null &&
                    o.Carga.SituacaoCarga != SituacaoCarga.Cancelada &&
                    o.Carga.SituacaoCarga != SituacaoCarga.Anulada &&
                    o.Carga.DataCriacaoCarga > dataCriacaoCargaInicial &&
                    o.Carga.Empresa != null &&
                    consultaCargaNFe.Any(cargaNfe => cargaNfe.CargaOrigem.Codigo == o.Carga.Codigo) &&
                    !consultaMensagemAlertaNaoComparecimento.Any(m =>
                        m.Entidade.Codigo == o.Carga.Codigo &&
                        (m.Tipo == TipoMensagemAlerta.CargaNaoAgendada || ((bool?)m.Bloquear ?? false) == true)
                    )
                );

            return consultaCargaJanelaCarregamento.Select(o => o.Codigo).ToList();
        }

        public List<int> BuscarCodigosParaMarcarComoNaoComparecimentoPorVeiculoNaoInformado(int codigoCentroCarregamento, DateTime dataTolerancia, DateTime dataCriacaoCargaInicial)
        {
            List<TipoMensagemAlerta> tiposMensagemAlertaNaoComparecimento = GatilhoAutomatizacaoNaoComparecimentoHelper.ObterTiposMensagemAlerta();

            var consultaMensagemAlertaNaoComparecimento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MensagemAlertaCarga>()
                .Where(o => o.Confirmada == false && tiposMensagemAlertaNaoComparecimento.Contains(o.Tipo));

            var consultaCargaNFe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaNFe>();

            var consultaCargaJanelaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                .Where(o =>
                    o.CentroCarregamento.Codigo == codigoCentroCarregamento &&
                    o.InicioCarregamento < dataTolerancia &&
                    o.Carga != null &&
                    o.Carga.SituacaoCarga != SituacaoCarga.Cancelada &&
                    o.Carga.SituacaoCarga != SituacaoCarga.Anulada &&
                    o.Carga.DataCriacaoCarga > dataCriacaoCargaInicial &&
                    o.Carga.Empresa != null &&
                    o.Carga.Veiculo == null &&
                    consultaCargaNFe.Any(cargaNfe => cargaNfe.CargaOrigem.Codigo == o.Carga.Codigo) &&
                    !consultaMensagemAlertaNaoComparecimento.Any(m =>
                        m.Entidade.Codigo == o.Carga.Codigo &&
                        (m.Tipo == TipoMensagemAlerta.CargaSemVeiculoInformado || ((bool?)m.Bloquear ?? false) == true)
                    )
                );

            return consultaCargaJanelaCarregamento.Select(o => o.Codigo).ToList();
        }

        public List<int> BuscarCodigosParaMarcarComoNaoComparecimentoPorVeiculoSemRegistroChegada(int codigoCentroCarregamento, DateTime dataTolerancia, DateTime dataCriacaoCargaInicial)
        {
            List<TipoMensagemAlerta> tiposMensagemAlertaNaoComparecimento = GatilhoAutomatizacaoNaoComparecimentoHelper.ObterTiposMensagemAlerta();

            var consultaMensagemAlertaNaoComparecimento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MensagemAlertaCarga>()
                .Where(o => o.Confirmada == false && tiposMensagemAlertaNaoComparecimento.Contains(o.Tipo));

            var consultaFluxoGestaoPatioSemRegistroChegada = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio>()
                .Where(o =>
                    o.Tipo == TipoFluxoGestaoPatio.Origem &&
                    o.SituacaoEtapaFluxoGestaoPatio != SituacaoEtapaFluxoGestaoPatio.Cancelado &&
                    o.DataChegadaVeiculoPrevista.HasValue == true &&
                    o.DataChegadaVeiculo.HasValue == false
                );

            var consultaCargaNFe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaNFe>();

            var consultaCargaJanelaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                .Where(o =>
                    o.CentroCarregamento.Codigo == codigoCentroCarregamento &&
                    o.InicioCarregamento < dataTolerancia &&
                    o.Carga != null &&
                    o.Carga.SituacaoCarga != SituacaoCarga.Cancelada &&
                    o.Carga.SituacaoCarga != SituacaoCarga.Anulada &&
                    o.Carga.DataCriacaoCarga > dataCriacaoCargaInicial &&
                    o.Carga.Empresa != null &&
                    o.Carga.Veiculo != null &&
                    consultaCargaNFe.Any(cargaNfe => cargaNfe.CargaOrigem.Codigo == o.Carga.Codigo) &&
                    consultaFluxoGestaoPatioSemRegistroChegada.Any(f => f.Carga.Codigo == o.Carga.Codigo) &&
                    !consultaMensagemAlertaNaoComparecimento.Any(m =>
                        m.Entidade.Codigo == o.Carga.Codigo &&
                        (m.Tipo == TipoMensagemAlerta.VeiculoSemRegistroChegada || ((bool?)m.Bloquear ?? false) == true)
                    )
                );

            return consultaCargaJanelaCarregamento.Select(o => o.Codigo).ToList();
        }

        #endregion Métodos Públicos - Automatização de Não Comparecimento

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> ConsultaPorIncidenciaDeHorario(int codigoCargaJanelaCarregamento, int codigoFilial, int codigoCentroCarregamento, DateTime dataInicioCarregamento, DateTime dataFinalCarregamento)
        {
            var consultaCargaJanelaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                .Where(o =>
                    o.Codigo != codigoCargaJanelaCarregamento &&
                    o.CentroCarregamento.Codigo == codigoCentroCarregamento &&
                    o.Excedente == false &&
                    (
                        (o.InicioCarregamento >= dataInicioCarregamento && o.InicioCarregamento < dataFinalCarregamento) ||
                        (o.TerminoCarregamento > dataInicioCarregamento && o.TerminoCarregamento <= dataFinalCarregamento) ||
                        (o.InicioCarregamento < dataInicioCarregamento && o.TerminoCarregamento > dataFinalCarregamento)

                    ) &&
                    (
                        (o.Carga == null && o.PreCarga.Filial.Codigo == codigoFilial) ||
                        (o.Carga != null && o.Carga.Filial.Codigo == codigoFilial)
                    ) &&
                    (
                        (o.Carga == null && o.PreCarga.SituacaoPreCarga != SituacaoPreCarga.Cancelada) ||
                        (o.Carga != null && o.Carga.SituacaoCarga != SituacaoCarga.Cancelada && o.Carga.SituacaoCarga != SituacaoCarga.Anulada)
                    )
                );

            return consultaCargaJanelaCarregamento;
        }

        private void AdicionarFiltroOperadorLogistica(ref IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> query, Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operador)
        {
            if (operador == null)
                return;

            var predicateAnd = PredicateBuilder.True<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>();

            if (operador.Filiais.Count > 0)
            {
                List<Dominio.Entidades.Embarcador.Filiais.Filial> filiais = (from obj in operador.Filiais select obj.Filial).Distinct().ToList();
                predicateAnd = predicateAnd.And(obj => filiais.Contains(obj.Carga.Filial) || filiais.Contains(obj.PreCarga.Filial));
            }

            if (operador.PossuiFiltroTipoOperacao)
                predicateAnd = predicateAnd.And(obj => ((obj.Carga.TipoOperacao != null && operador.TipoOperacoes.Contains(obj.Carga.TipoOperacao)) || (obj.Carga.TipoOperacao == null && operador.VisualizaCargasSemTipoOperacao)) || obj.Carga == null);

            if (operador.TiposCarga.Count > 0)
            {
                List<Dominio.ObjetosDeValor.Embarcador.Operacional.OperadorTipoCargaModelosVeiculares> operadorTiposCargasModelosVeiculares = (from obj in operador.TiposCarga
                                                                                                                                               select new Dominio.ObjetosDeValor.Embarcador.Operacional.OperadorTipoCargaModelosVeiculares()
                                                                                                                                               {
                                                                                                                                                   ModelosVeicularCarga = (from modelo in obj.ModelosVeiculares select modelo.ModeloVeicularCarga).ToList(),
                                                                                                                                                   TipoCarga = obj.TipoDeCarga
                                                                                                                                               }).ToList();

                // use False quando o predicado for criar para criar OR e use True quando predicado for criar and (veja o sql gerado para entender a regra)
                var predicateOr = PredicateBuilder.False<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>();

                foreach (Dominio.ObjetosDeValor.Embarcador.Operacional.OperadorTipoCargaModelosVeiculares operadorTipoCargaModelo in operadorTiposCargasModelosVeiculares)
                {
                    if (operadorTipoCargaModelo.ModelosVeicularCarga.Count > 0)
                    {
                        predicateOr = predicateOr.Or(obj => ((obj.Carga.TipoDeCarga.Codigo == operadorTipoCargaModelo.TipoCarga.Codigo || obj.PreCarga.TipoDeCarga.Codigo == operadorTipoCargaModelo.TipoCarga.Codigo)
                            && ((operadorTipoCargaModelo.ModelosVeicularCarga.Contains(obj.Carga.ModeloVeicularCarga) || operadorTipoCargaModelo.ModelosVeicularCarga.Contains(obj.PreCarga.ModeloVeicularCarga))
                            || obj.Carga.ModeloVeicularCarga == null)));
                    }
                    else
                    {
                        predicateOr = predicateOr.Or(obj => (obj.Carga.TipoDeCarga.Codigo == operadorTipoCargaModelo.TipoCarga.Codigo || obj.PreCarga.TipoDeCarga.Codigo == operadorTipoCargaModelo.TipoCarga.Codigo));
                    }

                }

                predicateOr = predicateOr.Or(obj => obj.Carga.TipoDeCarga == null);
                predicateAnd = predicateAnd.And(predicateOr);
            }

            if (!operador.SupervisorLogistica)
            {
                var predicateOr = PredicateBuilder.False<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>();
                predicateOr = predicateOr.Or(predicateAnd);
                predicateOr = predicateOr.Or(obj => obj.Carga.Operador.Codigo == operador.Usuario.Codigo || obj.Carga == null);
                query = query.Where(predicateOr);
            }
            else
                query = query.Where(predicateAnd);
        }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> ConsultarCargaJanelaCarregamento(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaCargaJanelaCarregamento filtrosPesquisa)
        {
            if (filtrosPesquisa.RetornarPreCargas)
                return ConsultarCargaJanelaCarregamentoComPreCarga(filtrosPesquisa);

            return ConsultarCargaJanelaCarregamentoSemPreCarga(filtrosPesquisa);
        }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> ConsultarCargaJanelaCarregamentoComPreCarga(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaCargaJanelaCarregamento filtrosPesquisa)
        {
            var consultaCargaJanelaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                .Where(o => (o.Carga.SituacaoCarga != SituacaoCarga.Cancelada && o.Carga.SituacaoCarga != SituacaoCarga.Anulada) || (o.Carga == null));

            if (filtrosPesquisa.CodigosCentroCarregamento?.Count > 0)
            {
                if (filtrosPesquisa.Excedente.HasValue && filtrosPesquisa.Excedente.Value)
                    consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => filtrosPesquisa.CodigosCentroCarregamento.Contains(o.CentroCarregamento.Codigo));
                else
                    consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => filtrosPesquisa.CodigosCentroCarregamento.Contains(o.CentroCarregamento.Codigo) || o.CentroCarregamento == null);
            }

            if (filtrosPesquisa.CpfCnpjDestinatario > 0d)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.Carga.Pedidos.Any(p => p.Pedido.Destinatario.CPF_CNPJ == filtrosPesquisa.CpfCnpjDestinatario) || (o.Carga == null && o.PreCarga.Destinatarios.Any(p => p.CPF_CNPJ == filtrosPesquisa.CpfCnpjDestinatario)));

            if (filtrosPesquisa.CodigoDestino > 0)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.Carga.Pedidos.Any(p => p.Destino.Codigo == filtrosPesquisa.CodigoDestino) || (o.Carga == null && o.PreCarga.Destinatarios.Any(p => p.Localidade.Codigo == filtrosPesquisa.CodigoDestino)));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Ordem))
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.Carga.Pedidos.Any(p => p.Pedido.Ordem == filtrosPesquisa.Ordem) || (o.Carga == null && o.PreCarga.Pedidos.Any(p => p.Ordem == filtrosPesquisa.Ordem)));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroBooking))
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.Carga.Pedidos.Any(p => p.Pedido.NumeroBooking == filtrosPesquisa.NumeroBooking) || (o.Carga == null && o.PreCarga.Pedidos.Any(p => p.NumeroBooking == filtrosPesquisa.NumeroBooking)));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.PortoSaida))
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.Carga.Pedidos.Any(p => p.Pedido.PortoSaida == filtrosPesquisa.PortoSaida) || (o.Carga == null && o.PreCarga.Pedidos.Any(p => p.PortoSaida == filtrosPesquisa.PortoSaida)));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.TipoEmbarque))
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.Carga.Pedidos.Any(p => p.Pedido.TipoEmbarque == filtrosPesquisa.TipoEmbarque) || (o.Carga == null && o.PreCarga.Pedidos.Any(p => p.TipoEmbarque == filtrosPesquisa.TipoEmbarque)));

            if (filtrosPesquisa.CodigoPaisDestino > 0)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.Carga.Pedidos.Any(p => p.Destino.Pais.Codigo == filtrosPesquisa.CodigoPaisDestino) || (o.Carga == null && o.PreCarga.Destinatarios.Any(p => p.Localidade.Pais.Codigo == filtrosPesquisa.CodigoPaisDestino)));

            if (filtrosPesquisa.CodigoMotorista > 0)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.Carga.Motoristas.Any(m => m.Codigo == filtrosPesquisa.CodigoMotorista) || (o.Carga == null && o.PreCarga.Motoristas.Any(m => m.Codigo == filtrosPesquisa.CodigoMotorista)));

            if (filtrosPesquisa.CodigoTransportador > 0)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.Carga.Empresa.Codigo == filtrosPesquisa.CodigoTransportador || (o.Carga == null && o.PreCarga.Empresa.Codigo == filtrosPesquisa.CodigoTransportador));

            if (filtrosPesquisa.CodigoVeiculo > 0)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.Carga.Veiculo.Codigo == filtrosPesquisa.CodigoVeiculo || (o.Carga == null && o.PreCarga.Veiculo.Codigo == filtrosPesquisa.CodigoVeiculo));

            if (filtrosPesquisa.CodigoRota > 0)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.Carga.Rota.Codigo == filtrosPesquisa.CodigoRota || (o.Carga == null && o.PreCarga.Rota.Codigo == filtrosPesquisa.CodigoRota));

            if (filtrosPesquisa.CodigoModeloVeicularCarga > 0)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.Carga.ModeloVeicularCarga.Codigo == filtrosPesquisa.CodigoModeloVeicularCarga || (o.Carga == null && o.PreCarga.ModeloVeicularCarga.Codigo == filtrosPesquisa.CodigoModeloVeicularCarga));

            if (filtrosPesquisa.CodigoOperador > 0)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.Carga.Operador.Codigo == filtrosPesquisa.CodigoOperador || (o.Carga == null && o.PreCarga.Operador.Codigo == filtrosPesquisa.CodigoOperador));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoCargaEmbarcador))
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.Carga.CodigoCargaEmbarcador == filtrosPesquisa.CodigoCargaEmbarcador || (o.Carga == null && o.PreCarga.NumeroPreCarga == filtrosPesquisa.CodigoCargaEmbarcador));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedidoEmbarcador))
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.Carga.Pedidos.Any(p => p.Pedido.NumeroPedidoEmbarcador == filtrosPesquisa.NumeroPedidoEmbarcador) || (o.Carga == null && o.PreCarga.Pedidos.Any(p => p.NumeroPedidoEmbarcador == filtrosPesquisa.NumeroPedidoEmbarcador)));

            if (filtrosPesquisa.CodigosTipoOperacao?.Count > 0)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => filtrosPesquisa.CodigosTipoOperacao.Contains(o.Carga.TipoOperacao.Codigo));

            if (filtrosPesquisa.CodigosFilial?.Count > 0)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => filtrosPesquisa.CodigosFilial.Contains(o.Carga.Filial.Codigo));

            if (filtrosPesquisa.CodigosFilialVenda?.Count > 0)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.Carga.Pedidos.Any(p => filtrosPesquisa.CodigosFilialVenda.Contains(p.Pedido.FilialVenda.Codigo)));

            if (filtrosPesquisa.CodigosTipoCarga?.Count > 0)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => filtrosPesquisa.CodigosTipoCarga.Contains(o.Carga.TipoDeCarga.Codigo));

            if (filtrosPesquisa.ApenasCargasNaoEmitidas)
            {
                List<SituacaoCarga> situacoesCargaNaoEmitida = SituacaoCargaHelper.ObterSituacoesCargaNaoEmitida();

                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => situacoesCargaNaoEmitida.Contains(o.Carga.SituacaoCarga) || o.Carga == null);
            }

            if (filtrosPesquisa.Excedente.HasValue)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.Excedente == filtrosPesquisa.Excedente.Value);

            if (filtrosPesquisa.EmReserva.HasValue)
            {
                if (filtrosPesquisa.EmReserva.Value)
                    consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.CarregamentoReservado == filtrosPesquisa.EmReserva.Value);
                else
                {
                    if (filtrosPesquisa.Excedente.HasValue && filtrosPesquisa.Excedente.Value)
                        consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.Carga != null || (o.Carga == null && !o.CarregamentoReservado));
                    else
                        consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.CarregamentoReservado == filtrosPesquisa.EmReserva.Value);
                }
            }

            if (filtrosPesquisa.ExibirCargaQueNaoEstaoEmInicioViagem)
                consultaCargaJanelaCarregamento = ObterConsultaFiltradaPorInicioViagemNaoInformado(consultaCargaJanelaCarregamento);

            consultaCargaJanelaCarregamento = ObterConsultaFiltradaPorSituacoes(consultaCargaJanelaCarregamento, filtrosPesquisa);

            if (filtrosPesquisa.ApenasCargasPendentes)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.Situacao != SituacaoCargaJanelaCarregamento.ProntaParaCarregamento);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.UFDestino) && !filtrosPesquisa.UFDestino.Contains("0"))
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.Carga.DadosSumarizados.UFDestinos.Contains(filtrosPesquisa.UFDestino));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.UFOrigem) && !filtrosPesquisa.UFOrigem.Contains("0"))
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.Carga.DadosSumarizados.UFOrigens.Contains(filtrosPesquisa.UFOrigem));

            AdicionarFiltroOperadorLogistica(ref consultaCargaJanelaCarregamento, filtrosPesquisa.Operador);

            return consultaCargaJanelaCarregamento;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> ConsultarCargaJanelaCarregamentoSemPreCarga(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaCargaJanelaCarregamento filtrosPesquisa)
        {
            var consultaCargaJanelaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                .Where(o => o.Carga.SituacaoCarga != SituacaoCarga.Cancelada && o.Carga.SituacaoCarga != SituacaoCarga.Anulada);

            if (filtrosPesquisa.CodigosCentroCarregamento?.Count > 0)
            {
                if (filtrosPesquisa.Excedente.HasValue && filtrosPesquisa.Excedente.Value)
                    consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => filtrosPesquisa.CodigosCentroCarregamento.Contains(o.CentroCarregamento.Codigo));
                else
                    consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => filtrosPesquisa.CodigosCentroCarregamento.Contains(o.CentroCarregamento.Codigo) || o.CentroCarregamento == null);
            }

            if (filtrosPesquisa.CpfCnpjDestinatario > 0)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.Carga.Pedidos.Any(p => p.Pedido.Destinatario.CPF_CNPJ == filtrosPesquisa.CpfCnpjDestinatario));

            if (filtrosPesquisa.CodigoDestino > 0)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.Carga.Pedidos.Any(p => p.Destino.Codigo == filtrosPesquisa.CodigoDestino));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Ordem))
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.Carga.Pedidos.Any(p => p.Pedido.Ordem == filtrosPesquisa.Ordem));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroBooking))
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.Carga.Pedidos.Any(p => p.Pedido.NumeroBooking == filtrosPesquisa.NumeroBooking));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.PortoSaida))
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.Carga.Pedidos.Any(p => p.Pedido.PortoSaida == filtrosPesquisa.PortoSaida));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.TipoEmbarque))
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.Carga.Pedidos.Any(p => p.Pedido.TipoEmbarque == filtrosPesquisa.TipoEmbarque));

            if (filtrosPesquisa.CodigoPaisDestino > 0)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.Carga.Pedidos.Any(p => p.Destino.Pais.Codigo == filtrosPesquisa.CodigoPaisDestino));

            if (filtrosPesquisa.CodigoMotorista > 0)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.Carga.Motoristas.Any(m => m.Codigo == filtrosPesquisa.CodigoMotorista));

            if (filtrosPesquisa.CodigoTransportador > 0)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.Carga.Empresa.Codigo == filtrosPesquisa.CodigoTransportador);

            if (filtrosPesquisa.CodigoVeiculo > 0)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.Carga.Veiculo.Codigo == filtrosPesquisa.CodigoVeiculo || o.Carga.VeiculosVinculados.Any(v => v.Codigo == filtrosPesquisa.CodigoVeiculo));

            if (filtrosPesquisa.CodigoRota > 0)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.Carga.Rota.Codigo == filtrosPesquisa.CodigoRota);

            if (filtrosPesquisa.CodigoModeloVeicularCarga > 0)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.Carga.ModeloVeicularCarga.Codigo == filtrosPesquisa.CodigoModeloVeicularCarga);

            if (filtrosPesquisa.CodigoOperador > 0)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.Carga.Operador.Codigo == filtrosPesquisa.CodigoOperador);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoCargaEmbarcador))
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.Carga.CodigoCargaEmbarcador == filtrosPesquisa.CodigoCargaEmbarcador);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedidoEmbarcador))
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.Carga.Pedidos.Any(p => p.Pedido.NumeroPedidoEmbarcador == filtrosPesquisa.NumeroPedidoEmbarcador));

            if (filtrosPesquisa.CodigosTipoOperacao?.Count > 0)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => filtrosPesquisa.CodigosTipoOperacao.Contains(o.Carga.TipoOperacao.Codigo));

            if (filtrosPesquisa.CodigosFilial?.Count > 0)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => filtrosPesquisa.CodigosFilial.Contains(o.Carga.Filial.Codigo));

            if (filtrosPesquisa.CodigosFilialVenda?.Count > 0)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.Carga.Pedidos.Any(p => filtrosPesquisa.CodigosFilialVenda.Contains(p.Pedido.FilialVenda.Codigo)));

            if (filtrosPesquisa.CodigosTipoCarga?.Count > 0)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => filtrosPesquisa.CodigosTipoCarga.Contains(o.Carga.TipoDeCarga.Codigo));

            if (filtrosPesquisa.ApenasCargasNaoEmitidas)
            {
                List<SituacaoCarga> situacoesCargaNaoEmitida = SituacaoCargaHelper.ObterSituacoesCargaNaoEmitida();

                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => situacoesCargaNaoEmitida.Contains(o.Carga.SituacaoCarga));
            }

            if (filtrosPesquisa.Excedente.HasValue)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.Excedente == filtrosPesquisa.Excedente.Value);

            if (filtrosPesquisa.EmReserva.HasValue)
            {
                if (filtrosPesquisa.EmReserva.Value || !(filtrosPesquisa.Excedente ?? false))
                    consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.CarregamentoReservado == filtrosPesquisa.EmReserva.Value);
            }

            if (filtrosPesquisa.ExibirCargaQueNaoEstaoEmInicioViagem)
                consultaCargaJanelaCarregamento = ObterConsultaFiltradaPorInicioViagemNaoInformado(consultaCargaJanelaCarregamento);

            consultaCargaJanelaCarregamento = ObterConsultaFiltradaPorSituacoes(consultaCargaJanelaCarregamento, filtrosPesquisa);

            if (filtrosPesquisa.ApenasCargasPendentes)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento
                    .Where(o =>
                        o.Situacao != SituacaoCargaJanelaCarregamento.ProntaParaCarregamento &&
                        (o.Carga.CargaFechada || o.CargaJanelaCarregamentoAgrupador != null) &&
                        o.Carga.SituacaoCarga != SituacaoCarga.EmTransporte &&
                        o.Carga.SituacaoCarga != SituacaoCarga.Encerrada &&
                        o.Carga.SituacaoCarga != SituacaoCarga.AgImpressaoDocumentos &&
                        o.Carga.SituacaoCarga != SituacaoCarga.PendeciaDocumentos &&
                        o.Carga.SituacaoCarga != SituacaoCarga.AgIntegracao &&
                        o.Carga.SituacaoCarga != SituacaoCarga.LiberadoPagamento
                    );

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.UFDestino) && !filtrosPesquisa.UFDestino.Contains("0"))
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.Carga.DadosSumarizados.UFDestinos.Contains(filtrosPesquisa.UFDestino));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.UFOrigem) && !filtrosPesquisa.UFOrigem.Contains("0"))
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.Carga.DadosSumarizados.UFOrigens.Contains(filtrosPesquisa.UFOrigem));

            AdicionarFiltroOperadorLogistica(ref consultaCargaJanelaCarregamento, filtrosPesquisa.Operador);

            if (filtrosPesquisa.RecomendacaoGR.HasValue)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.RecomendacaoGR == filtrosPesquisa.RecomendacaoGR.Value);

            if (filtrosPesquisa.SituacaoCotacao?.Count > 0)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => filtrosPesquisa.SituacaoCotacao.Contains(o.SituacaoCotacao));

            if (filtrosPesquisa.SituacaoLeilao?.Count > 0)
            {
                Expression<Func<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento, bool>> predicateOr = PredicateBuilder.False<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>();
                int interessados = 0;

                if (filtrosPesquisa.SituacaoLeilao.Contains(SituacaoCotacaoPesquisa.LeilaoAguardandoAprovacaoDoVencedor))
                    predicateOr = predicateOr.Or(obj => obj.SituacaoCotacao == SituacaoCargaJanelaCarregamentoCotacao.AguardandoAprovacao);

                if (filtrosPesquisa.SituacaoLeilao.Contains(SituacaoCotacaoPesquisa.LeilaoSemLances))
                    predicateOr = predicateOr.Or(obj => !(obj.SituacaoCotacao == SituacaoCargaJanelaCarregamentoCotacao.AguardandoAprovacao) && obj.ProcessoCotacaoFinalizada && interessados == 0 && obj.Carga.Empresa == null);

                if (filtrosPesquisa.SituacaoLeilao.Contains(SituacaoCotacaoPesquisa.LeilaoFinalizado))
                    predicateOr = predicateOr.Or(obj => !(obj.SituacaoCotacao == SituacaoCargaJanelaCarregamentoCotacao.AguardandoAprovacao) && obj.ProcessoCotacaoFinalizada && !(interessados == 0 && obj.Carga.Empresa == null));

                if (filtrosPesquisa.SituacaoLeilao.Contains(SituacaoCotacaoPesquisa.LeilaoAbertoParaLances))
                    predicateOr = predicateOr.Or(obj => !(obj.SituacaoCotacao == SituacaoCargaJanelaCarregamentoCotacao.AguardandoAprovacao) && !(obj.ProcessoCotacaoFinalizada && !(interessados == 0 && obj.Carga.Empresa == null)) && obj.CargaLiberadaCotacao);

                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(predicateOr);
            }

            return consultaCargaJanelaCarregamento;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> ConsultarPorCargaAdicionarFluxoPatio(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaCargaAdicionarFluxoPatio filtrosPesquisa)
        {
            var consultaFluxoGestaoPatio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio>();

            IQueryable<int> codigosCargaFluxoPatio = consultaFluxoGestaoPatio.Select(o => o.Carga.Codigo);

            var consultaCargaJanelaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                .Where(o =>
                    (o.Carga != null) &&
                    (o.Carga.CargaFechada == true) &&
                    (
                        (!o.Carga.ExigeNotaFiscalParaCalcularFrete && o.Carga.SituacaoCarga == SituacaoCarga.AgTransportador) ||
                        (o.Carga.ExigeNotaFiscalParaCalcularFrete && o.Carga.SituacaoCarga == SituacaoCarga.Nova)
                    ) &&
                    (((TipoCargaJanelaCarregamento?)o.Tipo).HasValue == false || o.Tipo == TipoCargaJanelaCarregamento.Carregamento)
                    && !(codigosCargaFluxoPatio.Contains(o.Carga.Codigo))
                );

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoCargaEmbarcador))
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.Carga.CodigoCargaEmbarcador == filtrosPesquisa.CodigoCargaEmbarcador);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedidoEmbarcador))
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.Carga.Pedidos.Any(p => p.Pedido.NumeroPedidoEmbarcador == filtrosPesquisa.NumeroPedidoEmbarcador));

            if (filtrosPesquisa.CodigoFilial > 0)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => (o.Carga.Filial.Codigo == filtrosPesquisa.CodigoFilial));

            if (filtrosPesquisa.CodigosTipoOperacao != null && filtrosPesquisa.CodigosTipoOperacao.Count > 0)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => filtrosPesquisa.CodigosTipoOperacao.Contains(o.Carga.TipoOperacao.Codigo));

            if (filtrosPesquisa.CpfCnpjDestinatario?.Count > 0)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.Carga.DadosSumarizados.ClientesDestinatarios.Any(d => filtrosPesquisa.CpfCnpjDestinatario.Contains(d.CPF_CNPJ)));

            if (filtrosPesquisa.CpfCnpjRemetente?.Count > 0)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.Carga.DadosSumarizados.ClientesRemetentes.Any(d => filtrosPesquisa.CpfCnpjRemetente.Contains(d.CPF_CNPJ)));

            if (filtrosPesquisa.CodigosTipoCarga != null && filtrosPesquisa.CodigosTipoCarga.Count > 0)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => filtrosPesquisa.CodigosTipoCarga.Contains(o.Carga.TipoDeCarga.Codigo));

            if (filtrosPesquisa.DataInicial.HasValue)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.InicioCarregamento >= filtrosPesquisa.DataInicial.Value.Date);

            if (filtrosPesquisa.DataFinal.HasValue)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.InicioCarregamento <= filtrosPesquisa.DataFinal.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.CodigoTransportador > 0)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(cargaJanelaCarregamento => cargaJanelaCarregamento.Carga.Empresa.Codigo == filtrosPesquisa.CodigoTransportador);
            else
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(cargaJanelaCarregamento => cargaJanelaCarregamento.Situacao == SituacaoCargaJanelaCarregamento.SemTransportador);

            return consultaCargaJanelaCarregamento;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> ObterConsultaFiltrada(IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> consultaCargaJanelaCarregamento, Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaJanelaCarregamento filtrosPesquisa)
        {
            consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.CentroCarregamento.Codigo == filtrosPesquisa.CodigoCentroCarregamento && !o.Excedente);

            if (filtrosPesquisa.DataCarregamento.HasValue)
            {
                if (filtrosPesquisa.TipoJanelaCarregamento == TipoJanelaCarregamento.Calendario)
                    consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o =>
                        ((o.InicioCarregamento >= filtrosPesquisa.DataCarregamento.Value.Date) && (o.InicioCarregamento < filtrosPesquisa.DataCarregamento.Value.AddDays(1).Date)) ||
                        ((o.TerminoCarregamento >= filtrosPesquisa.DataCarregamento.Value.Date) && (o.TerminoCarregamento < filtrosPesquisa.DataCarregamento.Value.AddDays(1).Date) && (o.InicioCarregamento < filtrosPesquisa.DataCarregamento.Value.Date))
                    );
                else
                    consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o =>
                        (o.InicioCarregamento >= filtrosPesquisa.DataCarregamento.Value.Date) && (o.InicioCarregamento < filtrosPesquisa.DataCarregamento.Value.AddDays(1).Date)
                    );
            }

            if (filtrosPesquisa.DataCarregamento.HasValue && filtrosPesquisa.PeriodoInicial.HasValue)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.InicioCarregamento >= filtrosPesquisa.DataCarregamento.Value.Add(filtrosPesquisa.PeriodoInicial.Value));

            if (filtrosPesquisa.DataCarregamento.HasValue && filtrosPesquisa.PeriodoFinal.HasValue)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.TerminoCarregamento <= filtrosPesquisa.DataCarregamento.Value.Add(filtrosPesquisa.PeriodoFinal.Value));

            if (filtrosPesquisa.DataCarregamento.HasValue && filtrosPesquisa.InicioPeriodoInicial.HasValue && filtrosPesquisa.InicioPeriodoFinal.HasValue)
            {
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => (o.InicioCarregamento >= filtrosPesquisa.DataCarregamento.Value.Add(filtrosPesquisa.InicioPeriodoInicial.Value) || o.TerminoCarregamento >= filtrosPesquisa.DataCarregamento.Value.Add(filtrosPesquisa.InicioPeriodoInicial.Value)) &&
                                                                                            (o.InicioCarregamento <= filtrosPesquisa.DataCarregamento.Value.Add(filtrosPesquisa.InicioPeriodoFinal.Value) || o.TerminoCarregamento <= filtrosPesquisa.DataCarregamento.Value.Add(filtrosPesquisa.InicioPeriodoFinal.Value)));
            }
            else if (filtrosPesquisa.DataCarregamento.HasValue && filtrosPesquisa.InicioPeriodoInicial.HasValue)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.InicioCarregamento <= filtrosPesquisa.DataCarregamento.Value.Add(filtrosPesquisa.InicioPeriodoInicial.Value) && o.TerminoCarregamento >= filtrosPesquisa.DataCarregamento.Value.Add(filtrosPesquisa.InicioPeriodoInicial.Value));
            else if (filtrosPesquisa.DataCarregamento.HasValue && filtrosPesquisa.InicioPeriodoFinal.HasValue)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.InicioCarregamento <= filtrosPesquisa.DataCarregamento.Value.Add(filtrosPesquisa.InicioPeriodoFinal.Value) && o.TerminoCarregamento >= filtrosPesquisa.DataCarregamento.Value.Add(filtrosPesquisa.InicioPeriodoFinal.Value));

            consultaCargaJanelaCarregamento = ObterConsultaFiltradaPorSituacoes(consultaCargaJanelaCarregamento, filtrosPesquisa);

            //Filtros exceto os essenciais
            if (filtrosPesquisa.CodigoModeloVeicularCarga > 0)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o =>
                    (o.Carga.ModeloVeicularCarga.Codigo == filtrosPesquisa.CodigoModeloVeicularCarga) ||
                    (o.PreCarga.ModeloVeicularCarga.Codigo == filtrosPesquisa.CodigoModeloVeicularCarga)
                );

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o =>
                    (o.Carga.TipoOperacao.Codigo == filtrosPesquisa.CodigoTipoOperacao) ||
                    (o.PreCarga.TipoOperacao.Codigo == filtrosPesquisa.CodigoTipoOperacao)
                );

            if (filtrosPesquisa.CodigoOperador > 0)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o =>
                    (o.Carga.Operador.Codigo == filtrosPesquisa.CodigoOperador) ||
                    (o.PreCarga.Operador.Codigo == filtrosPesquisa.CodigoOperador)
                );

            if (filtrosPesquisa.ExibirCargaQueNaoEstaoEmInicioViagem)
                consultaCargaJanelaCarregamento = ObterConsultaFiltradaPorInicioViagemNaoInformado(consultaCargaJanelaCarregamento);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoCargaEmbarcador))
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o =>
                    (o.Carga.CodigoCargaEmbarcador == filtrosPesquisa.CodigoCargaEmbarcador) ||
                    (o.Carga == null && o.PreCarga.NumeroPreCarga == filtrosPesquisa.CodigoCargaEmbarcador)
                );

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedidoEmbarcador))
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o =>
                    o.Carga.Pedidos.Any(cp => cp.Pedido.NumeroPedidoEmbarcador == filtrosPesquisa.NumeroPedidoEmbarcador) ||
                    (o.Carga == null && o.PreCarga.Pedidos.Any(p => p.NumeroPedidoEmbarcador == filtrosPesquisa.NumeroPedidoEmbarcador))
                );


            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroExp))
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o =>
                    o.Carga.Pedidos.Any(cp => cp.Pedido.NumeroEXP == filtrosPesquisa.NumeroExp) ||
                    (o.Carga == null && o.PreCarga.Pedidos.Any(p => p.NumeroEXP == filtrosPesquisa.NumeroExp))
                );

            if (filtrosPesquisa.CodigoTransportador > 0)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o =>
                    (o.Carga.Empresa.Codigo == filtrosPesquisa.CodigoTransportador) ||
                    (o.PreCarga.Empresa.Codigo == filtrosPesquisa.CodigoTransportador)
                );

            if (filtrosPesquisa.CpfCnpjDestinatario > 0d)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o =>
                    o.Carga.Pedidos.Any(cp => cp.Pedido.Destinatario.CPF_CNPJ == filtrosPesquisa.CpfCnpjDestinatario) ||
                    (o.Carga == null && o.PreCarga.Pedidos.Any(p => p.Destinatario.CPF_CNPJ == filtrosPesquisa.CpfCnpjDestinatario))
                );

            if (filtrosPesquisa.CodigoVeiculo > 0)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o =>
                    (o.Carga.Veiculo.Codigo == filtrosPesquisa.CodigoVeiculo || o.Carga.VeiculosVinculados.Any(v => v.Codigo == filtrosPesquisa.CodigoVeiculo)) ||
                    (o.Carga == null && o.PreCarga.Veiculo.Codigo == filtrosPesquisa.CodigoVeiculo || o.PreCarga.VeiculosVinculados.Any(v => v.Codigo == filtrosPesquisa.CodigoVeiculo))
                );

            if (filtrosPesquisa.SituacaoCargaJanelaCarregamento?.Count > 0)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => filtrosPesquisa.SituacaoCargaJanelaCarregamento.Contains(o.Situacao));

            if (filtrosPesquisa.SituacaoCotacao?.Count > 0)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => filtrosPesquisa.SituacaoCotacao.Contains(o.SituacaoCotacao));

            if (filtrosPesquisa.SituacaoLeilao?.Count > 0)
            {
                Expression<Func<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento, bool>> predicateOr = PredicateBuilder.False<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>();
                int interessados = 0;

                if (filtrosPesquisa.SituacaoLeilao.Contains(SituacaoCotacaoPesquisa.LeilaoAguardandoAprovacaoDoVencedor))
                    predicateOr = predicateOr.Or(obj => obj.SituacaoCotacao == SituacaoCargaJanelaCarregamentoCotacao.AguardandoAprovacao);

                if (filtrosPesquisa.SituacaoLeilao.Contains(SituacaoCotacaoPesquisa.LeilaoSemLances))
                    predicateOr = predicateOr.Or(obj => !(obj.SituacaoCotacao == SituacaoCargaJanelaCarregamentoCotacao.AguardandoAprovacao) && obj.ProcessoCotacaoFinalizada && interessados == 0 && obj.Carga.Empresa == null);

                if (filtrosPesquisa.SituacaoLeilao.Contains(SituacaoCotacaoPesquisa.LeilaoFinalizado))
                    predicateOr = predicateOr.Or(obj => !(obj.SituacaoCotacao == SituacaoCargaJanelaCarregamentoCotacao.AguardandoAprovacao) && obj.ProcessoCotacaoFinalizada && !(interessados == 0 && obj.Carga.Empresa == null));

                if (filtrosPesquisa.SituacaoLeilao.Contains(SituacaoCotacaoPesquisa.LeilaoAbertoParaLances))
                    predicateOr = predicateOr.Or(obj => !(obj.SituacaoCotacao == SituacaoCargaJanelaCarregamentoCotacao.AguardandoAprovacao) && !(obj.ProcessoCotacaoFinalizada && !(interessados == 0 && obj.Carga.Empresa == null)) && obj.CargaLiberadaCotacao);

                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(predicateOr);
            }

            if (filtrosPesquisa.RecomendacaoGR.HasValue)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.RecomendacaoGR == filtrosPesquisa.RecomendacaoGR.Value);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.UFDestino) && !filtrosPesquisa.UFDestino.Contains("0"))
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.Carga.DadosSumarizados.UFDestinos.Contains(filtrosPesquisa.UFDestino));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.UFOrigem) && !filtrosPesquisa.UFOrigem.Contains("0"))
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.Carga.DadosSumarizados.UFOrigens.Contains(filtrosPesquisa.UFOrigem));

            if (filtrosPesquisa.CodigoDestino > 0)
                consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o =>
                     o.Carga.Pedidos.Any(cp => cp.Pedido.Destino.Codigo == filtrosPesquisa.CodigoDestino) ||
                     (o.Carga == null && o.PreCarga.Pedidos.Any(p => p.Destino.Codigo == filtrosPesquisa.CodigoDestino))
                 );

            return consultaCargaJanelaCarregamento;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> ObterConsultaFiltradaPorInicioViagemNaoInformado(IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> consultaCargaJanelaCarregamento)
        {
            var consultaFluxoGestaoPatioEtapas = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas>()
                .Where(o =>
                    o.EtapaFluxoGestaoPatio == EtapaFluxoGestaoPatio.InicioViagem &&
                    o.FluxoGestaoPatio.Tipo == TipoFluxoGestaoPatio.Origem &&
                    o.FluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio == SituacaoEtapaFluxoGestaoPatio.Aguardando &&
                    o.FluxoGestaoPatio.DataInicioViagem == null
                );

            consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o =>
                consultaFluxoGestaoPatioEtapas.Any(etapaInicioVigem =>
                    etapaInicioVigem.FluxoGestaoPatio.Carga.Codigo == o.Carga.Codigo
                )
            );

            return consultaCargaJanelaCarregamento;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> ObterConsultaFiltradaPorSituacoes(IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> consultaCargaJanelaCarregamento, Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaSituacaoJanelaCarregamento filtrosPesquisa)
        {
            if (filtrosPesquisa.SituacaoFaturada && filtrosPesquisa.SituacaoNaoFaturada)
                return consultaCargaJanelaCarregamento;

            List<SituacaoCarga> situacoesCargaNaoFaturadas = SituacaoCargaHelper.ObterSituacoesCargaNaoFaturada();

            if (filtrosPesquisa.SituacaoFaturada)
            {
                if (filtrosPesquisa.Situacoes?.Count > 0)
                    return consultaCargaJanelaCarregamento.Where(o => !situacoesCargaNaoFaturadas.Contains(o.Carga.SituacaoCarga) || filtrosPesquisa.Situacoes.Contains(o.Situacao));

                return consultaCargaJanelaCarregamento.Where(o => !situacoesCargaNaoFaturadas.Contains(o.Carga.SituacaoCarga));
            }

            if (filtrosPesquisa.Situacoes?.Count > 0)
                return consultaCargaJanelaCarregamento.Where(o => situacoesCargaNaoFaturadas.Contains(o.Carga.SituacaoCarga) && filtrosPesquisa.Situacoes.Contains(o.Situacao));

            if (filtrosPesquisa.SituacaoNaoFaturada)
                return consultaCargaJanelaCarregamento.Where(o => situacoesCargaNaoFaturadas.Contains(o.Carga.SituacaoCarga));

            return consultaCargaJanelaCarregamento;
        }

        private string ObterWhereConsultaDirecionamentoPorOperador(int codigoOperador, int codigoCentroCarregamento, int codigoTransportador, int codigoVeiculo, int codigoFilial, double cpfCnpjDestinatario, DateTime dataInicial, DateTime dataFinal)
        {
            string where = $"and isnull(cjc.CJC_TIPO, {(int)TipoCargaJanelaCarregamento.Carregamento}) = {(int)TipoCargaJanelaCarregamento.Carregamento} "; // SQL-INJECTION-SAFE

            if (codigoCentroCarregamento > 0)
                where += "and cjc.CEC_CODIGO = " + codigoCentroCarregamento.ToString() + " "; // SQL-INJECTION-SAFE

            if (dataInicial != DateTime.MinValue)
                where += "and cjc.CJC_INICIO_CARREGAMENTO >= '" + dataInicial.ToString("yyyy-MM-dd") + "' "; // SQL-INJECTION-SAFE

            if (dataFinal != DateTime.MinValue)
                where += "and cjc.CJC_INICIO_CARREGAMENTO < '" + dataFinal.AddDays(1).ToString("yyyy-MM-dd") + "' "; // SQL-INJECTION-SAFE

            if (codigoTransportador > 0)
                where += "and car.EMP_CODIGO = " + codigoTransportador.ToString() + " "; // SQL-INJECTION-SAFE

            if (codigoOperador > 0)
                where += "and car.CAR_OPERADOR = " + codigoOperador.ToString() + " "; // SQL-INJECTION-SAFE

            if (cpfCnpjDestinatario > 0d)
                where += "and ped.CLI_CODIGO = " + cpfCnpjDestinatario.ToString("F0") + " ";  // SQL-INJECTION-SAFE

            if (codigoFilial > 0)
                where += "and car.FIL_CODIGO = " + codigoFilial.ToString() + " "; // SQL-INJECTION-SAFE

            if (codigoVeiculo > 0)
                where += "and (car.CAR_VEICULO = " + codigoVeiculo.ToString() + " or car.CAR_CODIGO in (select CAR_CODIGO from T_CARGA_VEICULOS_VINCULADOS where VEI_CODIGO = " + codigoVeiculo.ToString() + ")) "; // SQL-INJECTION-SAFE

            return where;
        }

        private SQLDinamico ObterSelectConsultaRelatorioDirecionamentoOperador(bool count, List<PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioDirecionamentoOperador filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            string select = string.Empty,
                   groupBy = string.Empty,
                   joins = string.Empty,
                   where = string.Empty,
                   orderBy = string.Empty;

            var parametros = new List<ParametroSQL>();  

            for (var i = propriedades.Count - 1; i >= 0; i--)
                SetarSelectRelatorioDirecionamentoOperador(propriedades[i].Propriedade, ref select, ref groupBy, ref joins, count);

            SetarWhereRelatorioDirecionamentoOperador(ref where, ref joins,ref parametros, filtrosPesquisa);

            if (!count)
            {
                if (!string.IsNullOrWhiteSpace(parametroConsulta.PropriedadeAgrupar))
                {
                    SetarSelectRelatorioDirecionamentoOperador(parametroConsulta.PropriedadeAgrupar, ref select, ref groupBy, ref joins, count);

                    if (select.Contains(parametroConsulta.PropriedadeAgrupar))
                        orderBy = parametroConsulta.PropriedadeAgrupar + " " + parametroConsulta.DirecaoAgrupar;
                }

                if (!string.IsNullOrWhiteSpace(parametroConsulta.PropriedadeOrdenar))
                {
                    if (parametroConsulta.PropriedadeOrdenar != parametroConsulta.PropriedadeAgrupar && select.Contains(parametroConsulta.PropriedadeOrdenar))
                        orderBy += (orderBy.Length > 0 ? ", " : string.Empty) + parametroConsulta.PropriedadeOrdenar + " " + parametroConsulta.DirecaoOrdenar;
                }
            }

            return new SQLDinamico((count ? "select distinct(count(0) over ())" : "select " + (select.Length > 0 ? select.Substring(0, select.Length - 2) : string.Empty)) +
                   " from T_CARGA car " +
                   " inner join T_CARGA_JANELA_CARREGAMENTO cjc on car.CAR_CODIGO = cjc.CAR_CODIGO " +
                   " inner join T_CARGA_JANELA_CARREGAMENTO_TRANSPORTADOR jct on cjc.CJC_CODIGO = jct.CJC_CODIGO " + joins +
                   " where jct.JCT_SITUACAO in (2,3,4,5) and car.CAR_SITUACAO <> 13" + where +
                   (groupBy.Length > 0 ? " group by " + groupBy.Substring(0, groupBy.Length - 2) : string.Empty) +
                   (count ? string.Empty : (orderBy.Length > 0 ? " order by " + orderBy : " order by 1 asc ")) +
                   (count || (parametroConsulta.InicioRegistros <= 0 && parametroConsulta.LimiteRegistros <= 0) ? "" : " offset " + parametroConsulta.InicioRegistros.ToString() + " rows fetch next " + parametroConsulta.LimiteRegistros.ToString() + " rows only;"), parametros);
        }

        private void SetarWhereRelatorioDirecionamentoOperador(ref string where, ref string joins, ref List<ParametroSQL> parametros, Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioDirecionamentoOperador filtrosPesquisa)
        {
            where = $"and isnull(cjc.CJC_TIPO, {(int)TipoCargaJanelaCarregamento.Carregamento}) = {(int)TipoCargaJanelaCarregamento.Carregamento} ";

            if (filtrosPesquisa.CodigoOperador > 0)
                where += " and car.CAR_OPERADOR = " + filtrosPesquisa.CodigoOperador.ToString();

            if (filtrosPesquisa.CodigoCentroCarregamento > 0)
                where += " and cjc.CEC_CODIGO = " + filtrosPesquisa.CodigoCentroCarregamento.ToString();

            if (filtrosPesquisa.CodigoTransportador > 0)
                where += " and jct.EMP_CODIGO = " + filtrosPesquisa.CodigoTransportador.ToString();

            if (filtrosPesquisa.CodigoRota > 0)
                where += " and car.ROF_CODIGO = " + filtrosPesquisa.CodigoRota.ToString();

            if (filtrosPesquisa.CodigosTipoCarga.Count > 0)
                where += $" and (car.TCG_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTipoCarga)}){(filtrosPesquisa.CodigosTipoCarga.Contains(-1) ? " or car.TCG_CODIGO is null" : "")})";

            if (filtrosPesquisa.CodigoModeloVeiculo > 0)
                where += " and car.MVC_CODIGO = " + filtrosPesquisa.CodigoModeloVeiculo.ToString();

            if (filtrosPesquisa.CpfCnpjDestinatario > 0f)
            {
                where += " and ped.CLI_CODIGO = " + filtrosPesquisa.CpfCnpjDestinatario.ToString("F0"); // SQL-INJECTION-SAFE

                if (!joins.Contains("T_CARGA_PEDIDO cpd"))
                    joins += "left outer join T_CARGA_PEDIDO cpd on cpd.CAR_CODIGO = car.CAR_CODIGO ";

                if (!joins.Contains("T_PEDIDO ped"))
                    joins += "left outer join T_PEDIDO ped on ped.PED_CODIGO = cpd.PED_CODIGO ";
            }

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                where += " and CAST(cjc.CJC_INICIO_CARREGAMENTO AS DATE) >= '" + filtrosPesquisa.DataInicial.ToString("MM/dd/yyyy") + "'"; // SQL-INJECTION-SAFE

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                where += " and CAST(cjc.CJC_INICIO_CARREGAMENTO AS DATE) <= '" + filtrosPesquisa.DataFinal.ToString("MM/dd/yyyy") + "'"; // SQL-INJECTION-SAFE

            if (filtrosPesquisa.CodigosFilial.Count > 0)
                where += $" and car.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosFilial)})"; // SQL-INJECTION-SAFE

            if (filtrosPesquisa.CodigosTipoOperacao.Count > 0)
                where += $" and (car.TOP_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTipoOperacao)}){(filtrosPesquisa.CodigosTipoOperacao.Contains(-1) ? " or car.TOP_CODIGO is null" : "")})"; // SQL-INJECTION-SAFE

            if (filtrosPesquisa.CodigoVeiculo > 0)
                where += "and (car.CAR_VEICULO = " + filtrosPesquisa.CodigoVeiculo.ToString() + " or car.CAR_CODIGO in (select CAR_CODIGO from T_CARGA_VEICULOS_VINCULADOS where VEI_CODIGO = " + filtrosPesquisa.CodigoVeiculo.ToString() + ")) "; // SQL-INJECTION-SAFE

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarga))
            {
                where += " and car.CAR_CODIGO_CARGA_EMBARCADOR = :CAR_CAR_CODIGO_CARGA_EMBARCADOR";
                parametros.Add(new ParametroSQL("CAR_CAR_CODIGO_CARGA_EMBARCADOR", filtrosPesquisa.NumeroCarga));
            }
        }

        private void SetarSelectRelatorioDirecionamentoOperador(string propriedade, ref string select, ref string groupBy, ref string joins, bool count)
        {
            switch (propriedade)
            {
                case "Operador":
                    if (!select.Contains("Operador"))
                    {
                        select += "ope.FUN_NOME Operador, ";
                        groupBy += "ope.FUN_NOME, ";

                        if (!joins.Contains("T_FUNCIONARIO ope"))
                            joins += "left outer join T_FUNCIONARIO ope on ope.FUN_CODIGO = car.CAR_OPERADOR ";
                    }
                    break;
                case "Situacao":
                    if (!select.Contains(" Situacao,"))
                    {
                        select += "(CASE WHEN jct.JCT_SITUACAO IN (2,3,5) THEN 'Direcionada' ELSE 'Rejeitada' END) Situacao, ";

                        if (!groupBy.Contains("jct.JCT_SITUACAO, "))
                            groupBy += "jct.JCT_SITUACAO, ";
                    }
                    break;
                case "NumeroCarga":
                    if (!select.Contains("NumeroCarga"))
                    {
                        select += "car.CAR_CODIGO_CARGA_EMBARCADOR NumeroCarga, ";
                        groupBy += "car.CAR_CODIGO_CARGA_EMBARCADOR, ";
                    }
                    break;
                case "DiasAtrazo":
                    if (!select.Contains("DiasAtrazo"))
                    {
                        select += "(CASE WHEN DATEDIFF(day, cjc.CJC_DATA_CARREGAMENTO_PROGRAMADA, cjc.CJC_INICIO_CARREGAMENTO) > 0 THEN DATEDIFF(day, cjc.CJC_DATA_CARREGAMENTO_PROGRAMADA, cjc.CJC_INICIO_CARREGAMENTO) ELSE 0 END) DiasAtrazo, ";
                        groupBy += "cjc.CJC_DATA_CARREGAMENTO_PROGRAMADA, cjc.CJC_INICIO_CARREGAMENTO, ";
                    }
                    break;
                case "DataCarregamento":
                case "DataCarregamentoFormatada":
                    if (!select.Contains("DataCarregamento"))
                    {
                        select += "cjc.CJC_INICIO_CARREGAMENTO DataCarregamento, ";

                        if (!groupBy.Contains("cjc.CJC_INICIO_CARREGAMENTO"))
                            groupBy += "cjc.CJC_INICIO_CARREGAMENTO, ";
                    }
                    break;
                case "DataPrimeiroSalvamentoDadosTransporteFormatada":
                    if (!select.Contains("DataPrimeiroSalvamentoDadosTransporte"))
                    {
                        select += "Car.CAR_DATA_PRIMEIRO_SALVAMENTO_DADOS_TRANSPORTE DataPrimeiroSalvamentoDadosTransporte, ";

                        if (!groupBy.Contains("Car.CAR_DATA_PRIMEIRO_SALVAMENTO_DADOS_TRANSPORTE"))
                            groupBy += "Car.CAR_DATA_PRIMEIRO_SALVAMENTO_DADOS_TRANSPORTE, ";
                    }
                    break;
                case "Observacao":
                    if (!select.Contains("Observacao"))
                    {
                        select += "crg.CRG_OBSERVACAO Observacao, ";
                        groupBy += "crg.CRG_OBSERVACAO, ";

                        if (!joins.Contains("T_CARREGAMENTO crg"))
                            joins += "left outer join T_CARREGAMENTO crg on crg.CRG_CODIGO = car.CRG_CODIGO ";
                    }
                    break;
                case "Destinatario":
                    if (!select.Contains("Destinatario"))
                    {
                        select += "cds.CDS_DESTINATARIOS Destinatario, ";
                        groupBy += "cds.CDS_DESTINATARIOS, ";

                        if (!joins.Contains("T_CARGA_DADOS_SUMARIZADOS cds"))
                            joins += "left outer join T_CARGA_DADOS_SUMARIZADOS cds on car.CDS_CODIGO = cds.CDS_CODIGO ";
                    }
                    break;
                case "Destino":
                    if (!select.Contains("Destino"))
                    {
                        select += "cds.CDS_DESTINOS Destino, ";
                        groupBy += "cds.CDS_DESTINOS, ";

                        if (!joins.Contains("T_CARGA_DADOS_SUMARIZADOS cds"))
                            joins += "left outer join T_CARGA_DADOS_SUMARIZADOS cds on car.CDS_CODIGO = cds.CDS_CODIGO ";
                    }
                    break;
                case "NumeroEntregas":
                    if (!select.Contains("NumeroEntregas"))
                    {
                        select += "cds.CDS_NUMERO_ENTREGAS NumeroEntregas, ";
                        groupBy += "cds.CDS_NUMERO_ENTREGAS, ";

                        if (!joins.Contains("T_CARGA_DADOS_SUMARIZADOS cds"))
                            joins += "left outer join T_CARGA_DADOS_SUMARIZADOS cds on car.CDS_CODIGO = cds.CDS_CODIGO ";
                    }
                    break;
                case "Rota":
                    if (!select.Contains("Rota"))
                    {
                        select += "rof.ROF_DESCRICAO Rota, ";
                        groupBy += "rof.ROF_DESCRICAO, ";

                        if (!joins.Contains("T_ROTA_FRETE rof"))
                            joins += "left outer join T_ROTA_FRETE rof on rof.ROF_CODIGO = car.ROF_CODIGO ";
                    }
                    break;
                case "Transportador":
                    if (!select.Contains(" Transportador, "))
                    {
                        select += "emp.EMP_RAZAO Transportador, ";
                        groupBy += "emp.EMP_RAZAO, ";

                        if (!joins.Contains("T_EMPRESA emp"))
                            joins += "left outer join T_EMPRESA emp on emp.EMP_CODIGO = jct.EMP_CODIGO ";
                    }
                    break;
                case "ModeloVeiculo":
                    if (!select.Contains("ModeloVeiculo"))
                    {
                        select += "mvc.MVC_DESCRICAO ModeloVeiculo, ";
                        groupBy += "mvc.MVC_DESCRICAO, ";
                        joins += "left outer join T_MODELO_VEICULAR_CARGA mvc on mvc.MVC_CODIGO = car.MVC_CODIGO ";
                    }
                    break;
                case "TipoCarga":
                    if (!select.Contains("TipoCarga"))
                    {
                        select += "tcg.TCG_DESCRICAO TipoCarga, ";
                        groupBy += "tcg.TCG_DESCRICAO, ";
                        joins += "left outer join T_TIPO_DE_CARGA tcg on tcg.TCG_CODIGO = car.TCG_CODIGO ";
                    }
                    break;
                case "Veiculos":
                    if (!select.Contains("Veiculos"))
                    {
                        select += "(vei.VEI_PLACA + ISNULL((SELECT ', ' + veiculo1.VEI_PLACA FROM T_CARGA_VEICULOS_VINCULADOS veiculoVinculadoCarga1 INNER JOIN T_VEICULO veiculo1 ON veiculoVinculadoCarga1.VEI_CODIGO = veiculo1.VEI_CODIGO WHERE veiculoVinculadoCarga1.CAR_CODIGO = car.CAR_CODIGO FOR XML PATH('')), '')) Veiculos, ";
                        groupBy += "vei.VEI_PLACA, ";

                        if (!groupBy.Contains("car.CAR_CODIGO "))
                            groupBy += "car.CAR_CODIGO, ";

                        if (!joins.Contains("T_VEICULO vei"))
                            joins += "LEFT OUTER JOIN T_VEICULO vei ON car.CAR_VEICULO = vei.VEI_CODIGO and vei.EMP_CODIGO = jct.EMP_CODIGO ";
                    }
                    break;
                case "ValorFrete":
                    if (!select.Contains(" ValorFrete, "))
                    {
                        select += @"CASE 
                                        WHEN (ISNULL(jct.JCT_PENDENTE_CALCULAR_FRETE, 0) = 1) THEN 0.00
                                        WHEN (ISNULL(jct.JCT_POSSUI_FRETE_CALCULADO, 0) = 1 and ISNULL(jct.JCT_FRETE_CALCULADO_COM_PROBLEMAS, 0) = 1) THEN 0.00

                                        WHEN (ISNULL(jct.JCT_POSSUI_FRETE_CALCULADO, 0) = 1 and ISNULL(jct.JCT_FRETE_CALCULADO_COM_PROBLEMAS, 0) = 0) and jct.JCT_SITUACAO IN (2, 3) THEN 
                                            car.CAR_VALOR_FRETE_PAGAR --mostra o valor que está na carga quando for do interesse confirmado

                                        WHEN ((ISNULL(jct.JCT_POSSUI_FRETE_CALCULADO, 0) = 1 or ISNULL(jct.JCT_VALOR_FRETE_TABELA, 0) > 0) and ISNULL(jct.JCT_FRETE_CALCULADO_COM_PROBLEMAS, 0) = 0) THEN 
                                            ISNULL(jct.JCT_VALOR_FRETE_TABELA, 0) +
                                            ISNULL((SELECT SUM(componente.JTC_VALOR)
                                                    from T_COMPONENTE_CARGA_JANELA_CARREGAMENTO_TRANSPORTADOR componente
                                             WHERE componente.JCT_CODIGO = jct.JCT_CODIGO), 0)

                                         ELSE car.CAR_VALOR_FRETE_PAGAR
                                    END ValorFrete, ";
                        groupBy += "car.CAR_VALOR_FRETE_PAGAR, jct.JCT_POSSUI_FRETE_CALCULADO, jct.JCT_FRETE_CALCULADO_COM_PROBLEMAS, jct.JCT_VALOR_FRETE_TABELA, jct.JCT_PENDENTE_CALCULAR_FRETE, ";

                        if (!groupBy.Contains("jct.JCT_CODIGO, "))
                            groupBy += "jct.JCT_CODIGO, ";

                        if (!groupBy.Contains("jct.JCT_SITUACAO, "))
                            groupBy += "jct.JCT_SITUACAO, ";
                    }
                    break;

                case "CNPJTransportadorFormatado":
                    if (!select.Contains(" CNPJTransportador, "))
                    {
                        select += "emp.EMP_CNPJ CNPJTransportador, ";
                        groupBy += "emp.EMP_CNPJ, ";

                        if (!joins.Contains("T_EMPRESA emp"))
                            joins += "left outer join T_EMPRESA emp on emp.EMP_CODIGO = jct.EMP_CODIGO ";
                    }
                    break;

                case "DataCriacaoCargaFormatada":
                    if (!select.Contains(" DataCriacaoCarga, "))
                    {
                        select += "car.CAR_DATA_CRIACAO DataCriacaoCarga, ";
                        groupBy += "car.CAR_DATA_CRIACAO, ";
                    }
                    break;

                case "DataInteresseFormatada":
                    if (!select.Contains(" DataInteresse, "))
                    {
                        select += "jct.JCT_DATA_INTERESSE DataInteresse, ";
                        groupBy += "jct.JCT_DATA_INTERESSE, ";
                    }
                    break;

                case "DataCargaContratadaFormatada":
                    if (!select.Contains(" DataCargaContratada, "))
                    {
                        select += "jct.JCT_DATA_CARGA_CONTRATADA DataCargaContratada, ";
                        groupBy += "jct.JCT_DATA_CARGA_CONTRATADA, ";
                    }
                    break;
                case "NomeMotorista":
                    if (!select.Contains(" NomeMotorista, "))
                    {
                        select += "(select FUN_NOME from t_carga_motorista join T_FUNCIONARIO on T_FUNCIONARIO.FUN_CODIGO = t_carga_motorista.CAR_motorista where car.CAR_CODIGO = t_carga_motorista.CAR_CODIGO) NomeMotorista, ";
                        if (!groupBy.Contains("car.CAR_CODIGO, "))
                            groupBy += "car.CAR_CODIGO, ";
                    }
                    break;
                case "CPFMotoristaFormatado":
                case "CPFMotorista":
                    if (!select.Contains(" CPFMotorista, "))
                    {
                        select += "(select FUN_CPF from t_carga_motorista join T_FUNCIONARIO on T_FUNCIONARIO.FUN_CODIGO = t_carga_motorista.CAR_motorista where car.CAR_CODIGO = t_carga_motorista.CAR_CODIGO) CPFMotorista, ";
                        if (!groupBy.Contains("car.CAR_CODIGO, "))
                            groupBy += "car.CAR_CODIGO, ";
                    }
                    break;
                case "QuantidadeVolumes":
                    if (!select.Contains("QuantidadeVolumes"))
                    {
                        select += @"(select sum(ped.PED_QUANTIDADE_VOLUMES) from T_CARGA_PEDIDO cpd
                                    left outer join T_PEDIDO ped on ped.PED_CODIGO = cpd.PED_CODIGO
                                    where cpd.CAR_CODIGO = car.CAR_CODIGO) QuantidadeVolumes,  ";
                        groupBy += "car.CAR_CODIGO, ";
                    }
                    break;
                case "NumeroPaletes":
                    if (!select.Contains("NumeroPaletes"))
                    {
                        select += @"(select sum(ped.PED_NUMERO_PALETES_FRACIONADO + ped.PED_NUMERO_PALETES) from T_CARGA_PEDIDO cpd
                                    left outer join T_PEDIDO ped on ped.PED_CODIGO = cpd.PED_CODIGO
                                    where cpd.CAR_CODIGO = car.CAR_CODIGO) NumeroPaletes,  ";
                        groupBy += "car.CAR_CODIGO, ";
                    }
                    break;
                case "MotivoRejeicaoCarga":
                    if (!select.Contains("MotivoRejeicaoCarga"))
                    {
                        select += "jct.JCT_MOTIVO_REJEICAO_CARGA MotivoRejeicaoCarga, ";

                        if (!groupBy.Contains("cjc.JCT_MOTIVO_REJEICAO_CARGA"))
                            groupBy += "jct.JCT_MOTIVO_REJEICAO_CARGA, ";
                    }
                    break;
            }
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Logistica.OrigensDoAceite> BuscarOrigensDoAceite(List<int> idsJanelaCarregamentos)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.OrigensDoAceite> returno = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.OrigensDoAceite>();

            if (idsJanelaCarregamentos == null || idsJanelaCarregamentos.Count <= 0)
                return returno;

            string sqlFilaCarregamento = @"    select C.CAR_CODIGO CodigoCarga,0 CodigoJanela,FLV_TIPO Tipo, 1 Origem from T_FILA_CARREGAMENTO_VEICULO F
			                        INNER JOIN T_CARGA C ON C.CAR_CODIGO = F.CAR_CODIGO
			                        INNER JOIN T_CARGA_JANELA_CARREGAMENTO J ON J.CAR_CODIGO = C.CAR_CODIGO
			                        WHERE  J.CJC_CODIGO IN ( " + String.Join(",", idsJanelaCarregamentos) + ")";

            var query01 = this.SessionNHiBernate.CreateSQLQuery(sqlFilaCarregamento);
            query01.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.OrigensDoAceite)));
            returno.AddRange(query01.List<Dominio.ObjetosDeValor.Embarcador.Logistica.OrigensDoAceite>());


            string sqlCargaCarregamentoTransportador = @" select CAR_CODIGO CodigoCarga,J.CJC_CODIGO CodigoJanela,JCT_TIPO Tipo,2 Origem from T_CARGA_JANELA_CARREGAMENTO_TRANSPORTADOR JT
                        INNER JOIN T_CARGA_JANELA_CARREGAMENTO J ON JT.CJC_CODIGO = J.CJC_CODIGO
                        WHERE JT.CJC_CODIGO IN (" + String.Join(",", idsJanelaCarregamentos) + ") AND JCT_TIPO IN (1,2)	";

            var query02 = this.SessionNHiBernate.CreateSQLQuery(sqlCargaCarregamentoTransportador);
            query02.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.OrigensDoAceite)));
            returno.AddRange(query02.List<Dominio.ObjetosDeValor.Embarcador.Logistica.OrigensDoAceite>());


            string sqlPreCarga = @" 
                select C.CAR_CODIGO CodigoCarga,J.CJC_CODIGO CodigoJanela,POT_TIPO TIPO, 3 Origem  from T_CARGA_JANELA_CARREGAMENTO J
                        INNER JOIN T_CARGA C ON J.CAR_CODIGO = C.CAR_CODIGO
                        INNER JOIN T_PRE_CARGA PC ON PC.CAR_CODIGO = C.CAR_CODIGO 
                        INNER JOIN T_PRE_CARGA_OFERTA PCO ON PC.PCA_CODIGO = PCO.PCA_CODIGO 
                        INNER JOIN T_PRE_CARGA_OFERTA_TRANSPORTADOR T ON T.PCO_CODIGO = PCO.PCO_CODIGO
                        WHERE J.CJC_CODIGO IN (" + String.Join(",", idsJanelaCarregamentos) + ") AND POT_TIPO IN (1,2) ";
            var query03 = this.SessionNHiBernate.CreateSQLQuery(sqlCargaCarregamentoTransportador);
            query03.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.OrigensDoAceite)));
            returno.AddRange(query03.List<Dominio.ObjetosDeValor.Embarcador.Logistica.OrigensDoAceite>());

            return returno;
        }
        #endregion Métodos Privados
    }
}
