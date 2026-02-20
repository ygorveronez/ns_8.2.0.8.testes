using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using LinqKit;
using NHibernate.Linq;
using Repositorio.Embarcador.Cargas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.PreCargas
{
    public class PreCarga : RepositorioBase<Dominio.Entidades.Embarcador.PreCargas.PreCarga>
    {
        #region Atributos

        private readonly int _limiteTentativasVincularFilaCarregamento = 3;

        #endregion Atributos

        #region Construtores

        public PreCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public PreCarga(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.PreCargas.PreCarga> Consultar(Dominio.ObjetosDeValor.Embarcador.PreCarga.FiltroPesquisaPreCarga filtrosPesquisa)
        {
            var consultaPreCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCarga>();

            if (filtrosPesquisa.DataInicial.HasValue)
                consultaPreCarga = consultaPreCarga.Where(o => o.DataPrevisaoEntrega >= filtrosPesquisa.DataInicial.Value.Date);

            if (filtrosPesquisa.DataFinal.HasValue)
                consultaPreCarga = consultaPreCarga.Where(o => o.DataPrevisaoEntrega <= filtrosPesquisa.DataFinal.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.DataViagemInicial.HasValue)
                consultaPreCarga = consultaPreCarga.Where(o => o.DataPrevisaoInicioViagem >= filtrosPesquisa.DataViagemInicial.Value.Date);

            if (filtrosPesquisa.DataViagemFinal.HasValue)
                consultaPreCarga = consultaPreCarga.Where(o => o.DataPrevisaoInicioViagem <= filtrosPesquisa.DataViagemFinal.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.CodigoFilial > 0)
                consultaPreCarga = consultaPreCarga.Where(o => o.Filial.Codigo == filtrosPesquisa.CodigoFilial);

            if (filtrosPesquisa.CodigosConfiguracaoProgramacaoCarga?.Count > 0)
                consultaPreCarga = consultaPreCarga.Where(o => filtrosPesquisa.CodigosConfiguracaoProgramacaoCarga.Contains(o.ConfiguracaoProgramacaoCarga.Codigo));

            if (filtrosPesquisa.CodigosModeloVeicularCarga?.Count > 0)
                consultaPreCarga = consultaPreCarga.Where(o => filtrosPesquisa.CodigosModeloVeicularCarga.Contains(o.ModeloVeicularCarga.Codigo));

            if (filtrosPesquisa.CodigosTipoCarga?.Count > 0)
                consultaPreCarga = consultaPreCarga.Where(o => filtrosPesquisa.CodigosTipoCarga.Contains(o.TipoDeCarga.Codigo));

            if (filtrosPesquisa.CodigosTipoOperacao?.Count > 0)
                consultaPreCarga = consultaPreCarga.Where(o => filtrosPesquisa.CodigosTipoOperacao.Contains(o.TipoOperacao.Codigo));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPreCarga))
                consultaPreCarga = consultaPreCarga.Where(o => o.NumeroPreCarga == filtrosPesquisa.NumeroPreCarga);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarga))
                consultaPreCarga = consultaPreCarga.Where(o => o.Carga.CodigoCargaEmbarcador == filtrosPesquisa.NumeroCarga);

            if (filtrosPesquisa.SemCarga)
                consultaPreCarga = consultaPreCarga.Where(o => o.Carga == null);

            if (filtrosPesquisa.SemCarregamento)
            {
                var consultaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento>()
                    .Where(carregamento => carregamento.PreCarga != null);

                consultaPreCarga = consultaPreCarga.Where(preCarga => !consultaCarregamento.Any(carregamento => carregamento.PreCarga.Codigo == preCarga.Codigo));
            }

            if (filtrosPesquisa.SomenteProgramacaoCarga)
                consultaPreCarga = consultaPreCarga.Where(o => o.ProgramacaoCarga == true);

            if (filtrosPesquisa.SomentePreCargaAtiva)
                consultaPreCarga = consultaPreCarga.Where(preCarga => preCarga.SituacaoPreCarga != SituacaoPreCarga.Cancelada);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedido))
                consultaPreCarga = consultaPreCarga.Where(o => o.Pedidos.Any(p => p.NumeroPedidoEmbarcador == filtrosPesquisa.NumeroPedido));

            if (filtrosPesquisa.CpfCnpjRemetente > 0d)
                consultaPreCarga = consultaPreCarga.Where(o => o.Pedidos.Any(p => p.Remetente.CPF_CNPJ == filtrosPesquisa.CpfCnpjRemetente));

            if (filtrosPesquisa.CpfCnpjDestinatario > 0d)
                consultaPreCarga = consultaPreCarga.Where(o => o.Pedidos.Any(p => p.Destinatario.CPF_CNPJ == filtrosPesquisa.CpfCnpjDestinatario));

            if (filtrosPesquisa.Situacao?.Count > 0)
                consultaPreCarga = consultaPreCarga.Where(preCarga => filtrosPesquisa.Situacao.Contains(preCarga.SituacaoPreCarga));

            if (filtrosPesquisa.Status != FiltroPreCarga.Todos)
            {
                DateTime dataBase = DateTime.Today;

                if (filtrosPesquisa.Status == FiltroPreCarga.ComCarga)
                    consultaPreCarga = consultaPreCarga.Where(o => o.Carga != null);
                else if (filtrosPesquisa.Status == FiltroPreCarga.ComDadosInformados)
                    consultaPreCarga = consultaPreCarga.Where(o => o.Carga == null && (o.Empresa != null || o.Veiculo != null || o.VeiculosVinculados.Count() > 0 || o.Motoristas.Count() > 0));
                else if (filtrosPesquisa.Status == FiltroPreCarga.EmDia)
                    consultaPreCarga = consultaPreCarga.Where(o => o.DataPrevisaoInicioViagem.Value.Date >= dataBase);
                else if (filtrosPesquisa.Status == FiltroPreCarga.EmAtraso)
                    consultaPreCarga = consultaPreCarga.Where(o => o.DataPrevisaoInicioViagem.Value.Date < dataBase);
                else if (filtrosPesquisa.Status == FiltroPreCarga.ProblemaVincularCarga)
                    consultaPreCarga = consultaPreCarga.Where(o => o.ProblemaVincularCarga != null && o.ProblemaVincularCarga != "");
            }

            if (filtrosPesquisa.CodigosCidadesDestino?.Count > 0)
            {
                var consultaCidadesDestino = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCargaDestino>()
                .Where(destino => filtrosPesquisa.CodigosCidadesDestino.Contains(destino.Localidade.Codigo));

                consultaPreCarga = consultaPreCarga.Where(preCarga => consultaCidadesDestino.Any(destino => destino.PreCarga.Codigo == preCarga.Codigo));
            }

            if (filtrosPesquisa.CodigosRotaFrete?.Count > 0)
                consultaPreCarga = consultaPreCarga.Where(o => filtrosPesquisa.CodigosRotaFrete.Contains(o.Rota.Codigo));

            return consultaPreCarga;
        }

        private IQueryable<Dominio.Entidades.Embarcador.PreCargas.PreCarga> ObterConsultaFiltradaPorDestinos(IQueryable<Dominio.Entidades.Embarcador.PreCargas.PreCarga> consultaPreCarga, List<int> codigosDestinos)
        {
            bool possuiDestinosInformados = (codigosDestinos?.Count > 0);

            if (!possuiDestinosInformados)
                return consultaPreCarga;

            List<int> codigosDestinosFiltrar = null;
            List<int> codigosRegioesDestinoFiltrar = null;
            List<string> siglasEstadosDestinoFiltrar = null;

            if (codigosDestinos?.Count > 0)
            {
                codigosDestinosFiltrar = codigosDestinos.ToList();

                codigosRegioesDestinoFiltrar = this.SessionNHiBernate.Query<Dominio.Entidades.Localidade>()
                    .Where(localidade => localidade.Regiao != null && codigosDestinos.Contains(localidade.Codigo))
                    .Select(localidade => localidade.Regiao.Codigo)
                    .Distinct()
                    .ToList();

                siglasEstadosDestinoFiltrar = this.SessionNHiBernate.Query<Dominio.Entidades.Localidade>()
                   .Where(localidade => codigosDestinos.Contains(localidade.Codigo))
                   .Select(localidade => localidade.Estado.Sigla)
                   .Distinct()
                   .ToList();
            }

            var condicoesPorDestinos = PredicateBuilder.True<Dominio.Entidades.Embarcador.PreCargas.PreCarga>();
            var condicaoPreCargaDestino = PredicateBuilder.False<Dominio.Entidades.Embarcador.PreCargas.PreCarga>();
            var consultaPreCargaDestino = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCargaDestino>();

            condicaoPreCargaDestino = condicaoPreCargaDestino.Or(preCarga => consultaPreCargaDestino.Count(preCargaDestino => preCargaDestino.PreCarga.Codigo == preCarga.Codigo) == 0);

            if (codigosDestinosFiltrar?.Count > 0)
            {
                var consultaPreCargaDestinoFiltrada = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCargaDestino>()
                    .Where(preCargaDestino => codigosDestinosFiltrar.Contains(preCargaDestino.Localidade.Codigo));

                condicaoPreCargaDestino = condicaoPreCargaDestino.Or(preCarga => consultaPreCargaDestinoFiltrada.Count(preCargaDestino => preCargaDestino.PreCarga.Codigo == preCarga.Codigo) == codigosDestinosFiltrar.Count);
            }

            var condicaoPreCargaRegiaoDestino = PredicateBuilder.False<Dominio.Entidades.Embarcador.PreCargas.PreCarga>();
            var consultaPreCargaRegiaoDestino = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCargaRegiaoDestino>();

            condicaoPreCargaRegiaoDestino = condicaoPreCargaRegiaoDestino.Or(preCarga => consultaPreCargaRegiaoDestino.Count(preCargaRegiaoDestino => preCargaRegiaoDestino.PreCarga.Codigo == preCarga.Codigo) == 0);

            if (codigosRegioesDestinoFiltrar?.Count > 0)
            {
                var consultaPreCargaRegiaoDestinoFiltrada = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCargaRegiaoDestino>()
                    .Where(preCargaRegiaoDestino => codigosRegioesDestinoFiltrar.Contains(preCargaRegiaoDestino.Regiao.Codigo));

                condicaoPreCargaRegiaoDestino = condicaoPreCargaRegiaoDestino.Or(preCarga => consultaPreCargaRegiaoDestinoFiltrada.Count(preCargaRegiaoDestino => preCargaRegiaoDestino.PreCarga.Codigo == preCarga.Codigo) == codigosRegioesDestinoFiltrar.Count);
            }

            var condicaoPreCargaEstadoDestino = PredicateBuilder.False<Dominio.Entidades.Embarcador.PreCargas.PreCarga>();
            var consultaPreCargaEstadoDestino = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCargaEstadoDestino>();

            condicaoPreCargaEstadoDestino = condicaoPreCargaEstadoDestino.Or(preCarga => consultaPreCargaEstadoDestino.Count(preCargaEstadoDestino => preCargaEstadoDestino.PreCarga.Codigo == preCarga.Codigo) == 0);

            if (siglasEstadosDestinoFiltrar?.Count > 0)
            {
                var consultaPreCargaEstadoDestinoFiltrada = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCargaEstadoDestino>()
                    .Where(preCargaEstadoDestino => siglasEstadosDestinoFiltrar.Contains(preCargaEstadoDestino.Estado.Sigla));

                condicaoPreCargaEstadoDestino = condicaoPreCargaEstadoDestino.Or(preCarga => consultaPreCargaEstadoDestinoFiltrada.Count(preCargaEstadoDestino => preCargaEstadoDestino.PreCarga.Codigo == preCarga.Codigo) == siglasEstadosDestinoFiltrar.Count);
            }

            condicoesPorDestinos = condicoesPorDestinos.And(condicaoPreCargaDestino);
            condicoesPorDestinos = condicoesPorDestinos.And(condicaoPreCargaRegiaoDestino);
            condicoesPorDestinos = condicoesPorDestinos.And(condicaoPreCargaEstadoDestino);

            return consultaPreCarga.Where(condicoesPorDestinos);
        }

        #endregion

        #region Métodos Públicos

        public void AtualizarTentativasVincularFilaCarregamento(int codigoPreCarga)
        {
            UnitOfWork.Sessao
                .CreateQuery("update PreCarga set TentativasVincularFilaCarregamento = isnull(TentativasVincularFilaCarregamento, 0) + 1 where Codigo = :codigoPreCarga")
                .SetInt32("codigoPreCarga", codigoPreCarga)
                .ExecuteUpdate();
        }

        public void ReiniciarTentativasVincularFilaCarregamentoComLimiteAtingido()
        {
            UnitOfWork.Sessao
                .CreateQuery("update PreCarga set TentativasVincularFilaCarregamento = 0 where TentativasVincularFilaCarregamento > :limiteTentativasVincularFilaCarregamento")
                .SetInt32("limiteTentativasVincularFilaCarregamento", _limiteTentativasVincularFilaCarregamento)
                .ExecuteUpdate();
        }

        public Dominio.Entidades.Embarcador.PreCargas.PreCarga BuscarPorCodigo(int codigo)
        {
            var consultaPreCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCarga>()
                .Where(o => o.Codigo == codigo);

            return consultaPreCarga
                .Fetch(o => o.Carga)
                .Fetch(o => o.Empresa)
                .Fetch(o => o.Filial)
                .Fetch(o => o.ModeloVeicularCarga)
                .Fetch(o => o.TipoDeCarga)
                .Fetch(o => o.TipoOperacao)
                .Fetch(o => o.Veiculo)
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.PreCargas.PreCarga> BuscarPorCodigos(List<int> codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCarga>();
            var result = from obj in query where codigo.Contains(obj.Codigo) select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.PreCargas.PreCarga BuscarPorNumeroPreCarga(string numeroPreCarga, List<Dominio.Entidades.Embarcador.PreCargas.PreCarga> lstPreCarga = null)
        {
            if (lstPreCarga != null)
                return lstPreCarga.Where(obj => obj.NumeroPreCarga == numeroPreCarga && obj.SituacaoPreCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPreCarga.Cancelada).FirstOrDefault();

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCarga>();
            var result = from obj in query where obj.NumeroPreCarga == numeroPreCarga && obj.SituacaoPreCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPreCarga.Cancelada select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.PreCargas.PreCarga> BuscarPorNumerosPreCarga(List<string> numerosPreCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCarga>();
            var result = from obj in query where numerosPreCarga.Contains(obj.NumeroPreCarga) && obj.SituacaoPreCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPreCarga.Cancelada select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.PreCargas.PreCarga BuscarPorNumeroFilial(string numeroPreCarga, string filial)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCarga>();
            var result = from obj in query where obj.NumeroPreCarga == numeroPreCarga && obj.SituacaoPreCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPreCarga.Cancelada select obj;

            if (!string.IsNullOrWhiteSpace(filial))
                result = result.Where(obj => obj.Filial.CodigoFilialEmbarcador == filial);

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.PreCargas.PreCarga> BuscarPorNumeroPreCargaFilial(string numeroPreCarga, string filial)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCarga>();
            var result = from obj in query where obj.NumeroPreCarga == numeroPreCarga && obj.SituacaoPreCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPreCarga.Cancelada select obj;

            if (!string.IsNullOrWhiteSpace(filial))
                result = result.Where(obj => obj.Filial.CodigoFilialEmbarcador == filial);

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.PreCargas.PreCarga BuscarPorNumeroPreCargaNaoCancelada(string numeroPreCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCarga>();
            var result = from obj in query
                         where
                            obj.NumeroPreCarga == numeroPreCarga
                            && obj.SituacaoPreCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPreCarga.Cancelada
                         select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.PreCargas.PreCarga BuscarAguardandoCargaPorNumero(string numeroPreCarga, int codigoFilial)
        {
            var consultaPreCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCarga>()
                .Where(preCarga =>
                    preCarga.NumeroPreCarga == numeroPreCarga &&
                    preCarga.SituacaoPreCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPreCarga.AguardandoGeracaoCarga &&
                    ((bool?)preCarga.ProgramacaoCarga ?? false) == false
                );

            if (codigoFilial > 0)
                consultaPreCarga = consultaPreCarga.Where(preCarga => preCarga.Filial.Codigo == codigoFilial);

            return consultaPreCarga.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.PreCargas.PreCarga BuscarPorNumeroPreCarga(string numeroPreCarga, int filial)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCarga>();
            var result = from obj in query where obj.NumeroPreCarga == numeroPreCarga select obj;

            if (filial > 0)
                result = result.Where(obj => obj.Filial.Codigo == filial);

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.PreCargas.PreCarga BuscarPorPedido(int codigoPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.PreCargas.PreCarga> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCarga>();

            query = query.Where(o => o.Pedidos.Any(p => p.Codigo == codigoPedido));

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.PreCargas.PreCarga BuscarPrimeiraAguardandoGeracaoCarga(int codigoFilial, int codigoModeloVeicularCarga, int codigoTipoCarga, DateTime dataProgramada, List<int> codigosDestinos)
        {
            var consultaPreCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCarga>()
                .Where(preCarga =>
                    preCarga.Filial.Codigo == codigoFilial &&
                    preCarga.ModeloVeicularCarga.Codigo == codigoModeloVeicularCarga &&
                    preCarga.DataPrevisaoEntrega <= dataProgramada &&
                    preCarga.SituacaoPreCarga == SituacaoPreCarga.AguardandoGeracaoCarga &&
                    ((bool?)preCarga.ProgramacaoCarga ?? false) == true
                );

            if (codigoTipoCarga > 0)
                consultaPreCarga = consultaPreCarga.Where(preCarga => preCarga.TipoDeCarga.Codigo == codigoTipoCarga);

            consultaPreCarga = ObterConsultaFiltradaPorDestinos(consultaPreCarga, codigosDestinos);

            return consultaPreCarga
                .OrderBy(preCarga => preCarga.DataPrevisaoEntrega).ThenBy(preCarga => preCarga.DataCriacaoPreCarga)
                .FirstOrDefault();
        }

        public int BuscarUltimoNumeroPreCarga()
        {
            IQueryable<Dominio.Entidades.Embarcador.PreCargas.PreCarga> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCarga>();

            return query.Max(o => (int?)o.NumeroPreCargaInterno) ?? 0;
        }

        public List<Dominio.Entidades.Embarcador.PreCargas.PreCarga> BuscarCargasAguardandoCalculoFrete(int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCarga>();

            query = query.Where(o => o.SituacaoPreCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPreCarga.Cancelada && o.CalculandoFrete);

            return query.Take(limite).ToList();
        }

        public Dominio.Entidades.Embarcador.PreCargas.PreCarga BuscarPorEscala(int veiculo, int empresa, int filial, System.DateTime dataEscala)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCarga>();
            var result = from obj in query
                         where
       obj.Veiculo.Codigo == veiculo &&
       obj.Empresa.Codigo == empresa &&
       obj.EscalaVeiculoEscalado != null &&
       obj.Carga == null &&
       obj.EscalaVeiculoEscalado.EscalaOrigemDestino.ExpedicaoEscala.GeracaoEscala.DataEscala == dataEscala
                         select obj;

            if (filial > 0)
                result = result.Where(obj => obj.Filial.Codigo == filial);

            return result.OrderBy(obj => obj.EscalaVeiculoEscalado.HoraCarregamento).FirstOrDefault();
        }

        public int ObterProximoCodigo()
        {
            return ObterProximoCodigo(codigoFilial: 0);
        }

        public int ObterProximoCodigo(int codigoFilial)
        {
            var consultaPreCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCarga>()
                .Where(o => o.AdicionadaManualmente);

            if (codigoFilial > 0)
                consultaPreCarga = consultaPreCarga.Where(o => o.Filial.Codigo == codigoFilial);

            int ultimoNumeroPreCarga = consultaPreCarga.Max(o => (int?)int.Parse(o.NumeroPreCarga)) ?? 0;

            return (ultimoNumeroPreCarga + 1);
        }

        public Dominio.Entidades.Embarcador.PreCargas.PreCarga BuscarPorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCarga>();

            var result = from obj in query where obj.Carga.Codigo == carga select obj;

            return result.FirstOrDefault();
        }
        public List<Dominio.Entidades.Embarcador.PreCargas.PreCarga> BuscarPorCargas(List<int> codigosCarga)
        {
            if (codigosCarga == null || codigosCarga.Count() < 1)
                return new List<Dominio.Entidades.Embarcador.PreCargas.PreCarga>();

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCarga>();

            var result = from obj in query where codigosCarga.Contains(obj.Carga.Codigo) select obj;

            return result.ToList();
        }
        public List<int> BuscarAguardandoGeracaoCarga(int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCarga>()
                .Where(a => a.AguardandoGeracaoCarga);

            return query
                .Select(o => o.Codigo)
                .Take(limite)
                .ToList();
        }

        public Dominio.Entidades.Embarcador.PreCargas.PreCarga BuscarPorCargaVinculadaOutraPreCarga(int codigoPreCarga, int codigoCarga)
        {
            var consultaPreCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCarga>()
                .Where(o =>
                    (o.Codigo != codigoPreCarga) &&
                    (o.Carga.Codigo == codigoCarga) &&
                    (o.SituacaoPreCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPreCarga.Cancelada)
                );

            return consultaPreCarga.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.PreCargas.PreCarga ValidarMotoristaPorHorario(int codigo, int motorista, DateTime dataPrevisaoInicioViagem)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCarga>();

            var result = from obj in query
                         where
                         obj.Codigo != codigo
                         && obj.Motoristas.Any(m => m.Codigo == motorista)
                         && obj.DataPrevisaoInicioViagem.Value == dataPrevisaoInicioViagem
                         select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Veiculos.Equipamento BuscarPrimeiroEquipamentoVinculado(int codigoPreCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCarga>();

            query = query.Where(obj => obj.Codigo == codigoPreCarga);

            return query.SelectMany(obj => obj.VeiculosVinculados).SelectMany(obj => obj.Equipamentos).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.PreCargas.PreCarga> Consultar(Dominio.ObjetosDeValor.Embarcador.PreCarga.FiltroPesquisaPreCarga filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaPreCarga = Consultar(filtrosPesquisa);

            consultaPreCarga = consultaPreCarga
                .Fetch(preCarga => preCarga.Carga)
                .Fetch(preCarga => preCarga.Filial)
                .Fetch(preCarga => preCarga.Empresa).ThenFetch(empresa => empresa.Localidade).ThenFetch(localidade => localidade.Estado)
                .Fetch(preCarga => preCarga.Empresa).ThenFetch(empresa => empresa.Localidade).ThenFetch(localidade => localidade.Pais)
                .Fetch(preCarga => preCarga.ModeloVeicularCarga)
                .Fetch(preCarga => preCarga.Veiculo)
                .Fetch(preCarga => preCarga.TipoOperacao)
                .Fetch(preCarga => preCarga.TipoDeCarga)
                .Fetch(preCarga => preCarga.ConfiguracaoProgramacaoCarga)
                .FetchMany(preCarga => preCarga.VeiculosVinculados)
                .FetchMany(preCarga => preCarga.Motoristas);

            return ObterLista(consultaPreCarga, parametrosConsulta);
        }

        public List<int> ConsultarCodigos(Dominio.ObjetosDeValor.Embarcador.PreCarga.FiltroPesquisaPreCarga filtrosPesquisa)
        {
            var consultaPreCarga = Consultar(filtrosPesquisa);

            return consultaPreCarga
                .Select(preCarga => preCarga.Codigo)
                .ToList();
        }

        public List<int> ConsultarCodigosParaVincularFilaCarregamento(int limiteRegistros)
        {
            var consultaFilaCarregamentoVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>()
                .Where(fila =>
                    fila.Situacao != SituacaoFilaCarregamentoVeiculo.Finalizada &&
                    fila.Situacao != SituacaoFilaCarregamentoVeiculo.Removida &&
                    fila.PreCarga != null
                );

            //            var consultaPreCargaOferta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCargaOferta>()
            //                .Where(oferta => oferta.Situacao == SituacaoPreCargaOferta.Liberada);

            var consultaPreCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCarga>()
                .Where(preCarga =>
                    preCarga.ProgramacaoCarga == true &&
                    preCarga.SituacaoPreCarga == SituacaoPreCarga.Nova &&
                    preCarga.Empresa == null &&
                    preCarga.DataPrevisaoEntrega >= DateTime.Now.AddMinutes(10) &&
                    ((int?)preCarga.TentativasVincularFilaCarregamento ?? 0) <= _limiteTentativasVincularFilaCarregamento
                );

            consultaPreCarga = consultaPreCarga.Where(preCarga => !consultaFilaCarregamentoVeiculo.Any(fila => fila.PreCarga.Codigo == preCarga.Codigo));
            //consultaPreCarga = consultaPreCarga.Where(preCarga => !consultaPreCargaOferta.Any(oferta => oferta.PreCarga.Codigo == preCarga.Codigo));

            return consultaPreCarga
                .OrderBy(preCarga => preCarga.DataPrevisaoEntrega).ThenBy(preCarga => preCarga.DataCriacaoPreCarga).ThenBy(preCarga => preCarga.TentativasVincularFilaCarregamento)
                .Select(preCarga => preCarga.Codigo)
                .Take(limiteRegistros)
                .ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.PreCarga.FiltroPesquisaPreCarga filtrosPesquisa)
        {
            var consultaPreCarga = Consultar(filtrosPesquisa);

            return consultaPreCarga.Count();
        }

        public Dominio.ObjetosDeValor.Embarcador.PreCarga.PreCargaTotalizador BuscarTotalizadorPorSituacao(Dominio.ObjetosDeValor.Embarcador.PreCarga.FiltroPesquisaPreCarga filtrosPesquisa)
        {
            StringBuilder sql = new StringBuilder();

            sql.AppendLine("select isnull([0], 0) as AguardandoGeracaoCarga, ");
            sql.AppendLine("       isnull([1], 0) as CargaGerada, ");
            sql.AppendLine("       isnull([2], 0) as Cancelada, ");
            sql.AppendLine("       isnull([3], 0) as Nova, ");
            sql.AppendLine("       isnull([4], 0) as AguardandoAceite, ");
            sql.AppendLine("       isnull([5], 0) as AguardandoDadosTransporte ");
            sql.AppendLine("  from ( ");
            sql.AppendLine("            select TotalizadoresPorTipo.Tipo, ");
            sql.AppendLine("                   count(TotalizadoresPorTipo.Codigo) TotalRegistros ");
            sql.AppendLine("              from ( ");
            sql.AppendLine("                       select PreCarga.PCA_CODIGO as Codigo, ");
            sql.AppendLine("                              PreCarga.PCA_SITUACAO as Tipo ");
            sql.AppendLine("                         from T_PRE_CARGA PreCarga ");
            sql.AppendLine("                        where 1 = 1 ");

            if (filtrosPesquisa.CodigoFilial > 0)
                sql.AppendLine($"                     and PreCarga.FIL_CODIGO = {filtrosPesquisa.CodigoFilial} ");

            if (filtrosPesquisa.DataInicial.HasValue)
                sql.AppendLine($"                     and PreCarga.CAR_DATA_PREVISAO_ENTREGA >= '{filtrosPesquisa.DataInicial.Value.Date:yyyyMMdd HH:mm:ss}' ");

            if (filtrosPesquisa.DataFinal.HasValue)
                sql.AppendLine($"                     and PreCarga.CAR_DATA_PREVISAO_ENTREGA <= '{filtrosPesquisa.DataFinal.Value.Date.Add(DateTime.MaxValue.TimeOfDay):yyyyMMdd HH:mm:ss}' ");

            if (filtrosPesquisa.SomenteProgramacaoCarga)
                sql.AppendLine($"                     and PreCarga.PCA_PROGRAMACAO_CARGA = 1 ");

            if (filtrosPesquisa.CodigosConfiguracaoProgramacaoCarga?.Count > 0)
                sql.AppendLine($"                     and PreCarga.CPC_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosConfiguracaoProgramacaoCarga)}) ");

            if (filtrosPesquisa.CodigosModeloVeicularCarga?.Count > 0)
                sql.AppendLine($"                     and PreCarga.MVC_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosModeloVeicularCarga)}) ");

            if (filtrosPesquisa.CodigosTipoCarga?.Count > 0)
                sql.AppendLine($"                     and PreCarga.TCG_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTipoCarga)}) ");

            if (filtrosPesquisa.CodigosTipoOperacao?.Count > 0)
                sql.AppendLine($"                     and PreCarga.TOP_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTipoOperacao)}) ");

            sql.AppendLine("                   ) as TotalizadoresPorTipo ");
            sql.AppendLine("             group by TotalizadoresPorTipo.Tipo ");
            sql.AppendLine("       ) as PreCargaTotalizadores ");
            sql.AppendLine(" pivot ( ");
            sql.AppendLine("           sum (PreCargaTotalizadores.TotalRegistros) ");
            sql.AppendLine("           for PreCargaTotalizadores.Tipo in ([0], [1], [2], [3], [4], [5]) ");
            sql.AppendLine("       ) as Totais ");

            var consultaTotalizadores = this.SessionNHiBernate.CreateSQLQuery(sql.ToString());

            consultaTotalizadores.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.PreCarga.PreCargaTotalizador)));

            return consultaTotalizadores.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.PreCarga.PreCargaTotalizador>().FirstOrDefault();
        }

        public int ContarQuantidadePreCargaPorConfiguracaoRota(int codigoConfiguracaoRota, int codigoPreCargaDesconsiderar, DateTime dataInicial, DateTime dataFinal, int codigoModeloVeicularCarga)
        {
            return ContarQuantidadePreCargaPorConfiguracaoRota(codigoConfiguracaoRota, codigoPreCargaDesconsiderar, dataInicial, dataFinal, codigoModeloVeicularCarga, codigoEmpresa: 0);
        }

        public int ContarQuantidadePreCargaPorConfiguracaoRota(int codigoConfiguracaoRota, int codigoPreCargaDesconsiderar, DateTime dataInicial, DateTime dataFinal, int codigoModeloVeicularCarga, int codigoEmpresa)
        {
            System.Text.StringBuilder sqlContarQuantidadePreCarga = new System.Text.StringBuilder();

            sqlContarQuantidadePreCarga.AppendLine("select count(PreCarga.PCA_CODIGO) ");
            sqlContarQuantidadePreCarga.AppendLine("  from T_PRE_CARGA PreCarga ");
            sqlContarQuantidadePreCarga.AppendLine("  join T_FILIAL Filial on Filial.FIL_CODIGO = PreCarga.FIL_CODIGO ");
            sqlContarQuantidadePreCarga.AppendLine($"where PreCarga.PCA_CODIGO <> {codigoPreCargaDesconsiderar} ");
            sqlContarQuantidadePreCarga.AppendLine("   and PreCarga.PCA_PROGRAMACAO_CARGA = 1 ");
            sqlContarQuantidadePreCarga.AppendLine($"  and PreCarga.PCA_SITUACAO <> {(int)SituacaoPreCarga.CargaGerada} ");
            sqlContarQuantidadePreCarga.AppendLine($"  and PreCarga.PCA_SITUACAO <> {(int)SituacaoPreCarga.Cancelada} ");
            sqlContarQuantidadePreCarga.AppendLine("   and PreCarga.EMP_CODIGO is not null ", codigoEmpresa == 0);
            sqlContarQuantidadePreCarga.AppendLine($"  and PreCarga.EMP_CODIGO = {codigoEmpresa} ", codigoEmpresa > 0);
            sqlContarQuantidadePreCarga.AppendLine($"  and PreCarga.MVC_CODIGO = {codigoModeloVeicularCarga} ", codigoModeloVeicularCarga > 0);
            sqlContarQuantidadePreCarga.AppendLine($"  and PreCarga.CAR_DATA_PREVISAO_ENTREGA between '{dataInicial.Date.ToString("yyyyMMdd HH:mm:ss")}' and '{dataFinal.Date.Add(DateTime.MaxValue.TimeOfDay).ToString("yyyyMMdd HH:mm:ss")}' ");
            sqlContarQuantidadePreCarga.AppendLine("   and (PreCarga.TCG_CODIGO is null or exists ( ");
            sqlContarQuantidadePreCarga.AppendLine("           select top(1) Configuracao.CRF_CODIGO ");
            sqlContarQuantidadePreCarga.AppendLine("             from T_CONFIGURACAO_ROTA_FRETE Configuracao ");
            sqlContarQuantidadePreCarga.AppendLine("             left join T_CONFIGURACAO_ROTA_FRETE_TIPO_CARGA ConfiguracaoTipoCarga on ConfiguracaoTipoCarga.CRF_CODIGO = Configuracao.CRF_CODIGO ");
            sqlContarQuantidadePreCarga.AppendLine($"           where Configuracao.CRF_CODIGO = {codigoConfiguracaoRota} ");
            sqlContarQuantidadePreCarga.AppendLine("              and (Configuracao.CRF_POSSUI_TIPOS_CARGA = 0 or ConfiguracaoTipoCarga.TCG_CODIGO = PreCarga.TCG_CODIGO) ");
            sqlContarQuantidadePreCarga.AppendLine("       )) ");
            sqlContarQuantidadePreCarga.AppendLine("   and (PreCarga.MVC_CODIGO is null or exists ( ");
            sqlContarQuantidadePreCarga.AppendLine("           select top(1) Configuracao.CRF_CODIGO ");
            sqlContarQuantidadePreCarga.AppendLine("             from T_CONFIGURACAO_ROTA_FRETE Configuracao ");
            sqlContarQuantidadePreCarga.AppendLine("             left join T_CONFIGURACAO_ROTA_FRETE_MODELO_VEICULAR_CARGA ConfiguracaoModeloVeicular on ConfiguracaoModeloVeicular.CRF_CODIGO = Configuracao.CRF_CODIGO ");
            sqlContarQuantidadePreCarga.AppendLine($"           where Configuracao.CRF_CODIGO = {codigoConfiguracaoRota} ");
            sqlContarQuantidadePreCarga.AppendLine("              and (Configuracao.CRF_POSSUI_MODELOS_VEICULARES_CARGA = 0 or ConfiguracaoModeloVeicular.MVC_CODIGO = PreCarga.MVC_CODIGO) ");
            sqlContarQuantidadePreCarga.AppendLine("       )) ");
            sqlContarQuantidadePreCarga.AppendLine("   and exists ( ");
            sqlContarQuantidadePreCarga.AppendLine("           select top(1) Configuracao.CRF_CODIGO ");
            sqlContarQuantidadePreCarga.AppendLine("             from T_CONFIGURACAO_ROTA_FRETE Configuracao ");
            sqlContarQuantidadePreCarga.AppendLine("             join T_CONFIGURACAO_ROTA_FRETE_LOCALIDADE_ORIGEM ConfiguracaoLocalidadeOrigem on ConfiguracaoLocalidadeOrigem.CRF_CODIGO = Configuracao.CRF_CODIGO ");
            sqlContarQuantidadePreCarga.AppendLine($"           where Configuracao.CRF_CODIGO = {codigoConfiguracaoRota} ");
            sqlContarQuantidadePreCarga.AppendLine("              and ConfiguracaoLocalidadeOrigem.LOC_CODIGO = Filial.LOC_CODIGO ");
            sqlContarQuantidadePreCarga.AppendLine("              and (Configuracao.FIL_CODIGO is null or Configuracao.FIL_CODIGO = Filial.FIL_CODIGO) ");
            sqlContarQuantidadePreCarga.AppendLine("       ) ");
            sqlContarQuantidadePreCarga.AppendLine("   and exists ( ");
            sqlContarQuantidadePreCarga.AppendLine("           select top(1) PreCargaDestino.PCA_CODIGO ");
            sqlContarQuantidadePreCarga.AppendLine("             from T_PRE_CARGA_DESTINO PreCargaDestino ");
            sqlContarQuantidadePreCarga.AppendLine("             join T_LOCALIDADES Localidade on Localidade.LOC_CODIGO = PreCargaDestino.LOC_CODIGO ");
            sqlContarQuantidadePreCarga.AppendLine("            where PreCargaDestino.PCA_CODIGO = PreCarga.PCA_CODIGO ");
            sqlContarQuantidadePreCarga.AppendLine("              and exists ( ");
            sqlContarQuantidadePreCarga.AppendLine("                      select top(1) 1 ");
            sqlContarQuantidadePreCarga.AppendLine("                        from T_CONFIGURACAO_ROTA_FRETE_ESTADO ConfiguracaoEstado ");
            sqlContarQuantidadePreCarga.AppendLine($"                      where ConfiguracaoEstado.CRF_CODIGO = {codigoConfiguracaoRota} ");
            sqlContarQuantidadePreCarga.AppendLine("                         and ConfiguracaoEstado.UF_SIGLA = Localidade.UF_SIGLA ");
            sqlContarQuantidadePreCarga.AppendLine("                  ) ");
            sqlContarQuantidadePreCarga.AppendLine("              and exists ( ");
            sqlContarQuantidadePreCarga.AppendLine("                      select top(1) 1 ");
            sqlContarQuantidadePreCarga.AppendLine("                        from T_CONFIGURACAO_ROTA_FRETE Configuracao ");
            sqlContarQuantidadePreCarga.AppendLine("                        left join T_CONFIGURACAO_ROTA_FRETE_LOCALIDADE_DESTINO ConfiguracaoLocalidadeDestino on ConfiguracaoLocalidadeDestino.CRF_CODIGO = Configuracao.CRF_CODIGO ");
            sqlContarQuantidadePreCarga.AppendLine($"                      where Configuracao.CRF_CODIGO = {codigoConfiguracaoRota} ");
            sqlContarQuantidadePreCarga.AppendLine("                         and (Configuracao.CRF_POSSUI_LOCALIDADES_DESTINO = 0 or ConfiguracaoLocalidadeDestino.LOC_CODIGO = Localidade.LOC_CODIGO) ");
            sqlContarQuantidadePreCarga.AppendLine("                  ) ");
            sqlContarQuantidadePreCarga.AppendLine("            union ");
            sqlContarQuantidadePreCarga.AppendLine("           select top(1) PreCargaEstadoDestino.PCA_CODIGO ");
            sqlContarQuantidadePreCarga.AppendLine("             from T_PRE_CARGA_ESTADO_DESTINO PreCargaEstadoDestino ");
            sqlContarQuantidadePreCarga.AppendLine("            where PreCargaEstadoDestino.PCA_CODIGO = PreCarga.PCA_CODIGO ");
            sqlContarQuantidadePreCarga.AppendLine("              and exists ( ");
            sqlContarQuantidadePreCarga.AppendLine("                      select top(1) 1 ");
            sqlContarQuantidadePreCarga.AppendLine("                        from T_CONFIGURACAO_ROTA_FRETE_ESTADO ConfiguracaoEstado ");
            sqlContarQuantidadePreCarga.AppendLine($"                      where ConfiguracaoEstado.CRF_CODIGO = {codigoConfiguracaoRota} ");
            sqlContarQuantidadePreCarga.AppendLine("                         and ConfiguracaoEstado.UF_SIGLA = PreCargaEstadoDestino.UF_SIGLA ");
            sqlContarQuantidadePreCarga.AppendLine("                  ) ");
            sqlContarQuantidadePreCarga.AppendLine("            union ");
            sqlContarQuantidadePreCarga.AppendLine("           select top(1) PreCargaRegiaoDestino.PCA_CODIGO ");
            sqlContarQuantidadePreCarga.AppendLine("             from T_PRE_CARGA_REGIAO_DESTINO PreCargaRegiaoDestino ");
            sqlContarQuantidadePreCarga.AppendLine("             join T_LOCALIDADES Localidade on Localidade.REG_CODIGO = PreCargaRegiaoDestino.REG_CODIGO ");
            sqlContarQuantidadePreCarga.AppendLine("            where PreCargaRegiaoDestino.PCA_CODIGO = PreCarga.PCA_CODIGO ");
            sqlContarQuantidadePreCarga.AppendLine("              and exists ( ");
            sqlContarQuantidadePreCarga.AppendLine("                      select top(1) 1 ");
            sqlContarQuantidadePreCarga.AppendLine("                        from T_CONFIGURACAO_ROTA_FRETE_ESTADO ConfiguracaoEstado ");
            sqlContarQuantidadePreCarga.AppendLine($"                      where ConfiguracaoEstado.CRF_CODIGO = {codigoConfiguracaoRota} ");
            sqlContarQuantidadePreCarga.AppendLine("                         and ConfiguracaoEstado.UF_SIGLA = Localidade.UF_SIGLA ");
            sqlContarQuantidadePreCarga.AppendLine("                  ) ");
            sqlContarQuantidadePreCarga.AppendLine("              and exists ( ");
            sqlContarQuantidadePreCarga.AppendLine("                      select top(1) 1 ");
            sqlContarQuantidadePreCarga.AppendLine("                        from T_CONFIGURACAO_ROTA_FRETE Configuracao ");
            sqlContarQuantidadePreCarga.AppendLine("                        left join T_CONFIGURACAO_ROTA_FRETE_LOCALIDADE_DESTINO ConfiguracaoLocalidadeDestino on ConfiguracaoLocalidadeDestino.CRF_CODIGO = Configuracao.CRF_CODIGO ");
            sqlContarQuantidadePreCarga.AppendLine($"                      where Configuracao.CRF_CODIGO = {codigoConfiguracaoRota} ");
            sqlContarQuantidadePreCarga.AppendLine("                         and (Configuracao.CRF_POSSUI_LOCALIDADES_DESTINO = 0 or ConfiguracaoLocalidadeDestino.LOC_CODIGO = Localidade.LOC_CODIGO) ");
            sqlContarQuantidadePreCarga.AppendLine("                  ) ");
            sqlContarQuantidadePreCarga.AppendLine("       ) ");

            var consultaContarQuantidadeCarga = this.SessionNHiBernate.CreateSQLQuery(sqlContarQuantidadePreCarga.ToString());

            return consultaContarQuantidadeCarga.UniqueResult<int>();
        }

        #endregion

        #region Relatório

        public IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.PreCarga> ConsultarRelatorioPreCarga(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioPreCarga filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = new ConsultaPreCarga().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.PreCarga)));

            return consulta.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.PreCarga>();
        }
        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.PreCarga>> ConsultarRelatorioPreCargaAsync(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioPreCarga filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = new ConsultaPreCarga().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.PreCarga)));

            return await consulta.SetTimeout(600).ListAsync<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.PreCarga>();
        }

        public async Task<int> ContarConsultaRelatorioPreCargaAsync(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioPreCarga filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, CancellationToken cancellationToken)
        {
            var consulta = new ConsultaPreCarga().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return await consulta.SetTimeout(600).UniqueResultAsync<int>(cancellationToken);
        }

        #endregion
    }
}
