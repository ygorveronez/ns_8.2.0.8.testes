using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using LinqKit;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;

namespace Repositorio.Embarcador.Logistica
{
    public sealed class FilaCarregamentoVeiculo : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>
    {
        #region Construtores

        public FilaCarregamentoVeiculo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> BuscarPorFilaPosicaoAjustarInferiorAtual(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculo filtrosPesquisa, int posicaoAtual, int novaPosicao)
        {
            var consultaFilaCarregamentoVeiculo = ConsultarFilaCarregamentoVeiculo(filtrosPesquisa);

            consultaFilaCarregamentoVeiculo = consultaFilaCarregamentoVeiculo.Where(o => (o.Posicao < posicaoAtual) && (o.Posicao >= novaPosicao));

            return consultaFilaCarregamentoVeiculo.ToList();
        }

        private List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> BuscarPorPosicaoAjustarSuperiorAtual(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculo filtrosPesquisa, int posicaoAtual, int novaPosicao)
        {
            var consultaFilaCarregamentoVeiculo = ConsultarFilaCarregamentoVeiculo(filtrosPesquisa);

            consultaFilaCarregamentoVeiculo = consultaFilaCarregamentoVeiculo.Where(o => (o.Posicao > posicaoAtual) && (o.Posicao <= novaPosicao));

            return consultaFilaCarregamentoVeiculo.ToList();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> ConsultarFilaCarregamentoVeiculo(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculo filtrosPesquisa)
        {
            var consultaFilaCarregamentoVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>();

            if (filtrosPesquisa.CodigoFilaCarregamento > 0)
                consultaFilaCarregamentoVeiculo = consultaFilaCarregamentoVeiculo.Where(o => o.Codigo == filtrosPesquisa.CodigoFilaCarregamento);

            if (filtrosPesquisa.CodigoCentroCarregamento > 0)
                consultaFilaCarregamentoVeiculo = consultaFilaCarregamentoVeiculo.Where(o => o.CentroCarregamento.Codigo == filtrosPesquisa.CodigoCentroCarregamento);
            else if (filtrosPesquisa.CodigoFilial > 0)
                consultaFilaCarregamentoVeiculo = consultaFilaCarregamentoVeiculo.Where(o => o.Filial.Codigo == filtrosPesquisa.CodigoFilial);

            if (filtrosPesquisa.CodigoGrupoModeloVeicularCarga > 0)
                consultaFilaCarregamentoVeiculo = consultaFilaCarregamentoVeiculo.Where(o => o.ConjuntoVeiculo.ModeloVeicularCarga.GrupoModeloVeicular.Codigo == filtrosPesquisa.CodigoGrupoModeloVeicularCarga);

            if (filtrosPesquisa.CodigosModeloVeicularCarga?.Count > 0)
                consultaFilaCarregamentoVeiculo = consultaFilaCarregamentoVeiculo.Where(o => filtrosPesquisa.CodigosModeloVeicularCarga.Contains(o.ConjuntoVeiculo.ModeloVeicularCarga.Codigo));

            if (filtrosPesquisa.CodigosCarga?.Count > 0)
                consultaFilaCarregamentoVeiculo = consultaFilaCarregamentoVeiculo.Where(o => filtrosPesquisa.CodigosCarga.Contains(o.Carga.Codigo));

            if (filtrosPesquisa.CodigosConfiguracaoProgramacaoCarga?.Count > 0)
                consultaFilaCarregamentoVeiculo = consultaFilaCarregamentoVeiculo.Where(o => filtrosPesquisa.CodigosConfiguracaoProgramacaoCarga.Contains(o.PreCarga.ConfiguracaoProgramacaoCarga.Codigo));

            if (filtrosPesquisa.CodigosTipoOperacao?.Count > 0)
                consultaFilaCarregamentoVeiculo = consultaFilaCarregamentoVeiculo.Where(fila =>
                    (fila.Carga != null && filtrosPesquisa.CodigosTipoOperacao.Contains(fila.Carga.TipoOperacao.Codigo)) ||
                    (fila.Carga == null && filtrosPesquisa.CodigosTipoOperacao.Contains(fila.PreCarga.TipoOperacao.Codigo))
                );

            if (filtrosPesquisa.DataProgramadaInicial.HasValue)
                consultaFilaCarregamentoVeiculo = consultaFilaCarregamentoVeiculo.Where(o => o.DataProgramada >= filtrosPesquisa.DataProgramadaInicial.Value.Date);

            if (filtrosPesquisa.DataProgramadaFinal.HasValue)
                consultaFilaCarregamentoVeiculo = consultaFilaCarregamentoVeiculo.Where(o => o.DataProgramada <= filtrosPesquisa.DataProgramadaFinal.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.CodigosDestino?.Count > 0)
            {
                var consultaFilaCarregamentoVeiculoDestino = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoDestino>()
                    .Where(destino => filtrosPesquisa.CodigosDestino.Contains(destino.Localidade.Codigo));

                consultaFilaCarregamentoVeiculo = consultaFilaCarregamentoVeiculo.Where(fila => consultaFilaCarregamentoVeiculoDestino.Any(destino => destino.FilaCarregamentoVeiculo.Codigo == fila.Codigo));
            }

            if (filtrosPesquisa.SiglasEstadoDestino?.Count > 0)
            {
                var consultaFilaCarregamentoVeiculoEstadoDestino = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoEstadoDestino>()
                    .Where(estadoDestino => filtrosPesquisa.SiglasEstadoDestino.Contains(estadoDestino.Estado.Sigla));

                consultaFilaCarregamentoVeiculo = consultaFilaCarregamentoVeiculo.Where(fila => consultaFilaCarregamentoVeiculoEstadoDestino.Any(estadoDestino => estadoDestino.FilaCarregamentoVeiculo.Codigo == fila.Codigo));
            }

            if (filtrosPesquisa.CodigosRegiaoDestino?.Count > 0)
            {
                var consultaFilaCarregamentoVeiculoRegiaoDestino = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoRegiaoDestino>()
                    .Where(regiaoDestino => filtrosPesquisa.CodigosRegiaoDestino.Contains(regiaoDestino.Regiao.Codigo));

                consultaFilaCarregamentoVeiculo = consultaFilaCarregamentoVeiculo.Where(fila => consultaFilaCarregamentoVeiculoRegiaoDestino.Any(regiaoDestino => regiaoDestino.FilaCarregamentoVeiculo.Codigo == fila.Codigo));
            }

            if (filtrosPesquisa.CodigosTipoCarga?.Count > 0)
            {
                var consultaFilaCarregamentoVeiculoTipoCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoTipoCarga>()
                    .Where(tipoCarga => filtrosPesquisa.CodigosTipoCarga.Contains(tipoCarga.TipoCarga.Codigo));

                consultaFilaCarregamentoVeiculo = consultaFilaCarregamentoVeiculo.Where(fila => consultaFilaCarregamentoVeiculoTipoCarga.Any(tipoCarga => tipoCarga.FilaCarregamentoVeiculo.Codigo == fila.Codigo));
            }

            if (filtrosPesquisa.CodigosTransportador?.Count > 0)
                consultaFilaCarregamentoVeiculo = consultaFilaCarregamentoVeiculo.Where(o =>
                    (filtrosPesquisa.CodigosTransportador.Contains(o.ConjuntoVeiculo.Tracao.Empresa.Codigo)) ||
                    o.ConjuntoVeiculo.Reboques.Any(r => filtrosPesquisa.CodigosTransportador.Contains(r.Empresa.Codigo))
                );

            if (filtrosPesquisa.CodigosVeiculo?.Count > 0)
                consultaFilaCarregamentoVeiculo = consultaFilaCarregamentoVeiculo.Where(o =>
                    (filtrosPesquisa.CodigosVeiculo.Contains(o.ConjuntoVeiculo.Tracao.Codigo)) ||
                    o.ConjuntoVeiculo.Reboques.Any(r => filtrosPesquisa.CodigosVeiculo.Contains(r.Codigo))
                );

            if (filtrosPesquisa.CodigoProprietarioVeiculo != 0)
                consultaFilaCarregamentoVeiculo = consultaFilaCarregamentoVeiculo.Where(o =>
                    o.ConjuntoVeiculo.Tracao.Proprietario.CPF_CNPJ == filtrosPesquisa.CodigoProprietarioVeiculo
                );

            if (filtrosPesquisa.Tipo.HasValue)
                consultaFilaCarregamentoVeiculo = consultaFilaCarregamentoVeiculo.Where(o => o.Tipo == filtrosPesquisa.Tipo.Value);

            if (filtrosPesquisa.Situacoes?.Count > 0)
                consultaFilaCarregamentoVeiculo = consultaFilaCarregamentoVeiculo.Where(o => filtrosPesquisa.Situacoes.Contains(o.Situacao));

            if (filtrosPesquisa.SituacaoPesquisa?.Count > 0)
            {
                var consultaFilaCarregamentoVeiculoSituacao = PredicateBuilder.False<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>();

                if (filtrosPesquisa.SituacaoPesquisa.Contains(SituacaoFilaCarregamentoVeiculoPesquisa.AguardandoAceite))
                    consultaFilaCarregamentoVeiculoSituacao = consultaFilaCarregamentoVeiculoSituacao.Or(o => o.Situacao == SituacaoFilaCarregamentoVeiculo.AguardandoAceiteCarga);

                if (filtrosPesquisa.SituacaoPesquisa.Contains(SituacaoFilaCarregamentoVeiculoPesquisa.AguardandoAceitePreCarga))
                    consultaFilaCarregamentoVeiculoSituacao = consultaFilaCarregamentoVeiculoSituacao.Or(o => o.Situacao == SituacaoFilaCarregamentoVeiculo.AguardandoAceitePreCarga);

                if (filtrosPesquisa.SituacaoPesquisa.Contains(SituacaoFilaCarregamentoVeiculoPesquisa.AguardandoCarga))
                    consultaFilaCarregamentoVeiculoSituacao = consultaFilaCarregamentoVeiculoSituacao.Or(o => o.Situacao == SituacaoFilaCarregamentoVeiculo.AguardandoCarga);

                if (filtrosPesquisa.SituacaoPesquisa.Contains(SituacaoFilaCarregamentoVeiculoPesquisa.AguardandoConfirmacao))
                    consultaFilaCarregamentoVeiculoSituacao = consultaFilaCarregamentoVeiculoSituacao.Or(o => o.Situacao == SituacaoFilaCarregamentoVeiculo.AguardandoConfirmacao);

                if (filtrosPesquisa.SituacaoPesquisa.Contains(SituacaoFilaCarregamentoVeiculoPesquisa.AguardandoConjuntos))
                    consultaFilaCarregamentoVeiculoSituacao = consultaFilaCarregamentoVeiculoSituacao.Or(o => o.Situacao == SituacaoFilaCarregamentoVeiculo.AguardandoConjuntos);

                if (filtrosPesquisa.SituacaoPesquisa.Contains(SituacaoFilaCarregamentoVeiculoPesquisa.AguardandoDesatrelar))
                    consultaFilaCarregamentoVeiculoSituacao = consultaFilaCarregamentoVeiculoSituacao.Or(o => o.Situacao == SituacaoFilaCarregamentoVeiculo.ReboqueAtrelado);

                if (filtrosPesquisa.SituacaoPesquisa.Contains(SituacaoFilaCarregamentoVeiculoPesquisa.CargaCancelada))
                    consultaFilaCarregamentoVeiculoSituacao = consultaFilaCarregamentoVeiculoSituacao.Or(o => o.Situacao == SituacaoFilaCarregamentoVeiculo.CargaCancelada);

                if (filtrosPesquisa.SituacaoPesquisa.Contains(SituacaoFilaCarregamentoVeiculoPesquisa.EmChecklist))
                    consultaFilaCarregamentoVeiculoSituacao = consultaFilaCarregamentoVeiculoSituacao.Or(o => o.Situacao == SituacaoFilaCarregamentoVeiculo.EmChecklist);

                if (filtrosPesquisa.SituacaoPesquisa.Contains(SituacaoFilaCarregamentoVeiculoPesquisa.EmRemocao))
                    consultaFilaCarregamentoVeiculoSituacao = consultaFilaCarregamentoVeiculoSituacao.Or(o => o.Situacao == SituacaoFilaCarregamentoVeiculo.EmRemocao);

                if (filtrosPesquisa.SituacaoPesquisa.Contains(SituacaoFilaCarregamentoVeiculoPesquisa.EmViagem))
                    consultaFilaCarregamentoVeiculoSituacao = consultaFilaCarregamentoVeiculoSituacao.Or(o => o.Situacao == SituacaoFilaCarregamentoVeiculo.EmViagem);

                if (filtrosPesquisa.SituacaoPesquisa.Contains(SituacaoFilaCarregamentoVeiculoPesquisa.Vazio))
                    consultaFilaCarregamentoVeiculoSituacao = consultaFilaCarregamentoVeiculoSituacao.Or(o =>
                        o.Situacao == SituacaoFilaCarregamentoVeiculo.Disponivel &&
                        o.Tipo == TipoFilaCarregamentoVeiculo.Vazio &&
                        (o.ConjuntoMotorista.FilaCarregamentoMotorista == null || o.ConjuntoMotorista.FilaCarregamentoMotorista.Situacao == SituacaoFilaCarregamentoMotorista.Disponivel)
                    );

                if (filtrosPesquisa.SituacaoPesquisa.Contains(SituacaoFilaCarregamentoVeiculoPesquisa.EmReversa))
                    consultaFilaCarregamentoVeiculoSituacao = consultaFilaCarregamentoVeiculoSituacao.Or(o =>
                        o.Situacao == SituacaoFilaCarregamentoVeiculo.Disponivel &&
                        o.Tipo == TipoFilaCarregamentoVeiculo.Reversa &&
                        (o.ConjuntoMotorista.FilaCarregamentoMotorista == null || o.ConjuntoMotorista.FilaCarregamentoMotorista.Situacao == SituacaoFilaCarregamentoMotorista.Disponivel)
                    );

                if (filtrosPesquisa.SituacaoPesquisa.Contains(SituacaoFilaCarregamentoVeiculoPesquisa.PerdeuSenha))
                    consultaFilaCarregamentoVeiculoSituacao = consultaFilaCarregamentoVeiculoSituacao.Or(o =>
                        o.Situacao == SituacaoFilaCarregamentoVeiculo.Disponivel &&
                        (o.ConjuntoMotorista.FilaCarregamentoMotorista.Situacao == SituacaoFilaCarregamentoMotorista.SenhaPerdida)
                    );

                if (filtrosPesquisa.SituacaoPesquisa.Contains(SituacaoFilaCarregamentoVeiculoPesquisa.CargaRecusada))
                    consultaFilaCarregamentoVeiculoSituacao = consultaFilaCarregamentoVeiculoSituacao.Or(o =>
                        o.Situacao == SituacaoFilaCarregamentoVeiculo.Disponivel &&
                        (o.ConjuntoMotorista.FilaCarregamentoMotorista.Situacao == SituacaoFilaCarregamentoMotorista.CargaRecusada)
                    );

                consultaFilaCarregamentoVeiculo = consultaFilaCarregamentoVeiculo.Where(consultaFilaCarregamentoVeiculoSituacao);
            }

            return consultaFilaCarregamentoVeiculo;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> ConsultarFilaCarregamentoVeiculoAcompanhamentoReversa(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAcompanhamentoFilaCarregamentoReversa filtrosPesquisa, TipoFilaCarregamentoVeiculo tipo)
        {
            var consultaFilaCarregamentoVeiculoReversa = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>();
            List<SituacaoFilaCarregamentoVeiculo> situacoesNaFila = SituacaoFilaCarregamentoVeiculoHelper.ObterSituacoesNaFila();

            if (filtrosPesquisa.CodigosCentroCarregamento?.Count > 0)
                consultaFilaCarregamentoVeiculoReversa = consultaFilaCarregamentoVeiculoReversa.Where(o => filtrosPesquisa.CodigosCentroCarregamento.Contains(o.CentroCarregamento.Codigo));

            consultaFilaCarregamentoVeiculoReversa = consultaFilaCarregamentoVeiculoReversa.Where(o => situacoesNaFila.Contains(o.Situacao));
            consultaFilaCarregamentoVeiculoReversa = consultaFilaCarregamentoVeiculoReversa.Where(o => o.Tipo == tipo);

            return consultaFilaCarregamentoVeiculoReversa;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> ConsultarFilaCarregamentoVeiculoDisponivelParaPreCarga(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculoPreCarga filtrosPesquisa)
        {
            var consultaFilaCarregamentoVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>()
                .Where(o => o.PreCarga.Codigo == filtrosPesquisa.CodigoPreCarga ||
                        (
                            o.Situacao == SituacaoFilaCarregamentoVeiculo.Disponivel && o.Tipo == TipoFilaCarregamentoVeiculo.Vazio &&
                            (
                                o.ConjuntoMotorista.FilaCarregamentoMotorista == null ||
                                o.ConjuntoMotorista.FilaCarregamentoMotorista.Situacao == SituacaoFilaCarregamentoMotorista.Disponivel
                            )
                        )
                );

            if (filtrosPesquisa.CodigoModeloVeicularCarga > 0)
                consultaFilaCarregamentoVeiculo = consultaFilaCarregamentoVeiculo.Where(o => o.ConjuntoVeiculo.ModeloVeicularCarga.Codigo == filtrosPesquisa.CodigoModeloVeicularCarga);

            if (filtrosPesquisa.CodigoCentroCarregamento > 0)
                consultaFilaCarregamentoVeiculo = consultaFilaCarregamentoVeiculo.Where(o => o.CentroCarregamento.Codigo == filtrosPesquisa.CodigoCentroCarregamento);
            else
                consultaFilaCarregamentoVeiculo = consultaFilaCarregamentoVeiculo.Where(o => o.Filial.Codigo == filtrosPesquisa.CodigoFilial);

            if (filtrosPesquisa.DataProgramada.HasValue)
                consultaFilaCarregamentoVeiculo = consultaFilaCarregamentoVeiculo.Where(o => (o.DataProgramada != null) && (o.DataProgramada.Value.Date <= filtrosPesquisa.DataProgramada.Value.Date));

            consultaFilaCarregamentoVeiculo = ObterConsultaFiltradaPorDestinos(consultaFilaCarregamentoVeiculo, filtrosPesquisa.CodigosDestinos, filtrosPesquisa.CodigosRegioesDestino, filtrosPesquisa.SiglasEstadosDestino);

            return consultaFilaCarregamentoVeiculo;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> ConsultarFilaCarregamentoVeiculoDisponivel(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculoDisponivel filtrosPesquisa)
        {
            var consultaFilaCarregamentoVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>()
                .Where(o =>
                    (o.Situacao == SituacaoFilaCarregamentoVeiculo.Disponivel) &&
                    (o.Tipo == TipoFilaCarregamentoVeiculo.Vazio) &&
                    (
                        (o.ConjuntoMotorista.FilaCarregamentoMotorista == null) ||
                        (o.ConjuntoMotorista.FilaCarregamentoMotorista.Situacao == SituacaoFilaCarregamentoMotorista.Disponivel)
                    )
                );

            if (filtrosPesquisa.CodigoCentroCarregamento > 0)
                consultaFilaCarregamentoVeiculo = consultaFilaCarregamentoVeiculo.Where(o => o.CentroCarregamento.Codigo == filtrosPesquisa.CodigoCentroCarregamento);
            else
                consultaFilaCarregamentoVeiculo = consultaFilaCarregamentoVeiculo.Where(o => o.Filial.Codigo == filtrosPesquisa.CodigoFilial);

            if (filtrosPesquisa.CodigoModeloVeicularCarga > 0)
                consultaFilaCarregamentoVeiculo = consultaFilaCarregamentoVeiculo.Where(o => o.ConjuntoVeiculo.ModeloVeicularCarga.Codigo == filtrosPesquisa.CodigoModeloVeicularCarga);

            if (filtrosPesquisa.CodigoTransportador > 0)
                consultaFilaCarregamentoVeiculo = consultaFilaCarregamentoVeiculo
                    .Where(o => o.ConjuntoMotorista.Motorista.Empresa.Codigo == filtrosPesquisa.CodigoTransportador ||
                                o.ConjuntoVeiculo.Tracao.Empresa.Codigo == filtrosPesquisa.CodigoTransportador ||
                                o.ConjuntoVeiculo.Reboques.Any(reboque => reboque.Empresa.Codigo == filtrosPesquisa.CodigoTransportador)
                    );

            return consultaFilaCarregamentoVeiculo;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> ObterConsultaFiltradaPorDestinos(IQueryable<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> consultaFilaCarregamentoVeiculo, List<int> codigosDestinos, List<int> codigosRegioesDestino, List<string> siglasEstadosDestino)
        {
            bool possuiDestinosInformados = (codigosDestinos?.Count > 0) || (codigosRegioesDestino?.Count > 0) || (siglasEstadosDestino?.Count > 0);

            if (!possuiDestinosInformados)
                return consultaFilaCarregamentoVeiculo;

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
            else if (codigosRegioesDestino?.Count > 0)
            {
                codigosRegioesDestinoFiltrar = codigosRegioesDestino.ToList();

                codigosDestinosFiltrar = this.SessionNHiBernate.Query<Dominio.Entidades.Localidade>()
                   .Where(localidade => codigosRegioesDestino.Contains(localidade.Regiao.Codigo))
                   .Select(localidade => localidade.Codigo)
                   .Distinct()
                   .ToList();

                siglasEstadosDestinoFiltrar = this.SessionNHiBernate.Query<Dominio.Entidades.Localidade>()
                   .Where(localidade => codigosRegioesDestino.Contains(localidade.Regiao.Codigo))
                   .Select(localidade => localidade.Estado.Sigla)
                   .Distinct()
                   .ToList();
            }
            else if (siglasEstadosDestino?.Count > 0)
            {
                codigosDestinosFiltrar = new List<int>();
                codigosRegioesDestinoFiltrar = new List<int>();
                siglasEstadosDestinoFiltrar = siglasEstadosDestino.ToList();
            }

            var condicoesPorDestinos = PredicateBuilder.True<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>();
            var condicaoFilaDestino = PredicateBuilder.False<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>();
            var consultaFilaDestino = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoDestino>();

            condicaoFilaDestino = condicaoFilaDestino.Or(fila => consultaFilaDestino.Count(filaDestino => filaDestino.FilaCarregamentoVeiculo.Codigo == fila.Codigo) == 0);

            if (codigosDestinosFiltrar?.Count > 0)
            {
                var consultaFilaDestinoFiltrada = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoDestino>()
                    .Where(filaDestino => codigosDestinosFiltrar.Contains(filaDestino.Localidade.Codigo));

                condicaoFilaDestino = condicaoFilaDestino.Or(fila => consultaFilaDestinoFiltrada.Count(filaDestino => filaDestino.FilaCarregamentoVeiculo.Codigo == fila.Codigo) == codigosDestinosFiltrar.Count);
            }

            var condicaoFilaRegiaoDestino = PredicateBuilder.False<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>();
            var consultaFilaRegiaoDestino = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoRegiaoDestino>();

            condicaoFilaRegiaoDestino = condicaoFilaRegiaoDestino.Or(fila => consultaFilaRegiaoDestino.Count(filaRegiaoDestino => filaRegiaoDestino.FilaCarregamentoVeiculo.Codigo == fila.Codigo) == 0);

            if (codigosRegioesDestinoFiltrar?.Count > 0)
            {
                var consultaFilaRegiaoDestinoFiltrada = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoRegiaoDestino>()
                    .Where(filaRegiaoDestino => codigosRegioesDestinoFiltrar.Contains(filaRegiaoDestino.Regiao.Codigo));

                condicaoFilaRegiaoDestino = condicaoFilaRegiaoDestino.Or(fila => consultaFilaRegiaoDestinoFiltrada.Count(filaRegiaoDestino => filaRegiaoDestino.FilaCarregamentoVeiculo.Codigo == fila.Codigo) == codigosRegioesDestinoFiltrar.Count);
            }

            var condicaoFilaEstadoDestino = PredicateBuilder.False<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>();
            var consultaFilaEstadoDestino = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoEstadoDestino>();

            condicaoFilaEstadoDestino = condicaoFilaEstadoDestino.Or(fila => consultaFilaEstadoDestino.Count(filaEstadoDestino => filaEstadoDestino.FilaCarregamentoVeiculo.Codigo == fila.Codigo) == 0);

            if (siglasEstadosDestinoFiltrar?.Count > 0)
            {
                var consultaFilaEstadoDestinoFiltrada = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoEstadoDestino>()
                    .Where(filaEstadoDestino => siglasEstadosDestinoFiltrar.Contains(filaEstadoDestino.Estado.Sigla));

                condicaoFilaEstadoDestino = condicaoFilaEstadoDestino.Or(fila => consultaFilaEstadoDestinoFiltrada.Count(filaEstadoDestino => filaEstadoDestino.FilaCarregamentoVeiculo.Codigo == fila.Codigo) == siglasEstadosDestinoFiltrar.Count);
            }

            condicoesPorDestinos = condicoesPorDestinos.And(condicaoFilaDestino);
            condicoesPorDestinos = condicoesPorDestinos.And(condicaoFilaRegiaoDestino);
            condicoesPorDestinos = condicoesPorDestinos.And(condicaoFilaEstadoDestino);

            return consultaFilaCarregamentoVeiculo.Where(condicoesPorDestinos);
        }

        private string ObterSituacoesNaFila()
        {
            return string.Join(", ", (from situacao in SituacaoFilaCarregamentoVeiculoHelper.ObterSituacoesNaFila() select (int)situacao).ToList());
        }

        private string ObterSqlConsultaResumoPorGrupoModeloVeicularCarga(int codigoCentroCarregamento, int codigoFilial)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append("select count(T_FILA_CARREGAMENTO_VEICULO.FLV_CODIGO) TotalRegistros, ");
            sql.Append("       T_MODELO_VEICULAR_CARGA_GRUPO.MVG_DESCRICAO DescricaoGrupoOuModeloVeicular ");
            sql.Append("  from T_FILA_CARREGAMENTO_VEICULO ");
            sql.Append("  join T_FILA_CARREGAMENTO_CONJUNTO_VEICULO ");
            sql.Append("    on T_FILA_CARREGAMENTO_CONJUNTO_VEICULO.FCV_CODIGO = T_FILA_CARREGAMENTO_VEICULO.FCV_CODIGO ");
            sql.Append("  join T_MODELO_VEICULAR_CARGA ");
            sql.Append("    on T_FILA_CARREGAMENTO_CONJUNTO_VEICULO.MVC_CODIGO = T_MODELO_VEICULAR_CARGA.MVC_CODIGO ");
            sql.Append("  join T_MODELO_VEICULAR_CARGA_GRUPO ");
            sql.Append("    on T_MODELO_VEICULAR_CARGA.MVG_CODIGO = T_MODELO_VEICULAR_CARGA_GRUPO.MVG_CODIGO ");

            if (codigoCentroCarregamento > 0)
                sql.Append($"where T_FILA_CARREGAMENTO_VEICULO.CEC_CODIGO = {codigoCentroCarregamento} ");
            else
                sql.Append($"where T_FILA_CARREGAMENTO_VEICULO.FIL_CODIGO = {codigoFilial} ");

            sql.Append($"  and T_FILA_CARREGAMENTO_VEICULO.FLV_SITUACAO in ({ObterSituacoesNaFila()}) ");
            sql.Append(" group by T_MODELO_VEICULAR_CARGA_GRUPO.MVG_CODIGO, ");
            sql.Append("       T_MODELO_VEICULAR_CARGA_GRUPO.MVG_DESCRICAO ");

            return sql.ToString();
        }

        private string ObterSqlConsultaResumoPorModeloVeicularCarga(int codigoCentroCarregamento, int codigoFilial, int codigoGrupoModeloVeicularCarga)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append("select count(T_FILA_CARREGAMENTO_VEICULO.FLV_CODIGO) TotalRegistros, ");
            sql.Append("       T_MODELO_VEICULAR_CARGA.MVC_DESCRICAO DescricaoGrupoOuModeloVeicular ");
            sql.Append("  from T_FILA_CARREGAMENTO_VEICULO ");
            sql.Append("  join T_FILA_CARREGAMENTO_CONJUNTO_VEICULO ");
            sql.Append("    on T_FILA_CARREGAMENTO_CONJUNTO_VEICULO.FCV_CODIGO = T_FILA_CARREGAMENTO_VEICULO.FCV_CODIGO ");
            sql.Append("  join T_MODELO_VEICULAR_CARGA ");
            sql.Append("    on T_FILA_CARREGAMENTO_CONJUNTO_VEICULO.MVC_CODIGO = T_MODELO_VEICULAR_CARGA.MVC_CODIGO ");

            if (codigoCentroCarregamento > 0)
                sql.Append($"where T_FILA_CARREGAMENTO_VEICULO.CEC_CODIGO = {codigoCentroCarregamento} ");
            else
                sql.Append($"where T_FILA_CARREGAMENTO_VEICULO.FIL_CODIGO = {codigoFilial} ");

            sql.Append($"  and T_FILA_CARREGAMENTO_VEICULO.FLV_SITUACAO in ({ObterSituacoesNaFila()}) ");
            sql.Append($"  and T_MODELO_VEICULAR_CARGA.MVG_CODIGO = {codigoGrupoModeloVeicularCarga} ", (codigoGrupoModeloVeicularCarga > 0));
            sql.Append(" group by T_MODELO_VEICULAR_CARGA.MVC_CODIGO, ");
            sql.Append("       T_MODELO_VEICULAR_CARGA.MVC_DESCRICAO ");

            return sql.ToString();
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> BuscarAguardandoChegadaveiculo(List<int> listaCodigoCentroCarregamento)
        {
            var consultaFilaCarregamentoVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>()
                .Where(o => o.Situacao == SituacaoFilaCarregamentoVeiculo.AguardandoChegadaVeiculo);

            if (listaCodigoCentroCarregamento?.Count > 0)
                consultaFilaCarregamentoVeiculo = consultaFilaCarregamentoVeiculo.Where(o => listaCodigoCentroCarregamento.Contains(o.CentroCarregamento.Codigo));

            return consultaFilaCarregamentoVeiculo.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> BuscarAguardandoChegadaVeiculoPorVeiculo(int codigo)
        {
            var consultaFilaCarregamentoVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>()
                .Where(o => (o.Situacao == SituacaoFilaCarregamentoVeiculo.AguardandoChegadaVeiculo) && ((o.ConjuntoVeiculo.Tracao.Codigo == codigo) || (o.ConjuntoVeiculo.Reboques.Any(r => r.Codigo == codigo))));

            return consultaFilaCarregamentoVeiculo.ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoAgrupamento> BuscarAgrupamentoPorCargaAguardandoConfirmacao()
        {
            var filaCarregamentoAgrupamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>()
                .Where(o =>
                    (o.Situacao == SituacaoFilaCarregamentoVeiculo.AguardandoConfirmacao) &&
                    (o.ConjuntoMotorista.FilaCarregamentoMotorista.Situacao == SituacaoFilaCarregamentoMotorista.CargaAlocada)
                )
                .GroupBy(o =>
                    new
                    {
                        codigoCentroCarregamento = o.CentroCarregamento.Codigo,
                        codigoModeloVeicular = o.ConjuntoVeiculo.ModeloVeicularCarga.Codigo
                    },
                    (key, value) => new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoAgrupamento()
                    {
                        CodigoCentroCarregamento = key.codigoCentroCarregamento,
                        CodigoModeloVeicularCarga = key.codigoModeloVeicular
                    }
                )
                .Distinct()
                .ToList();

            return filaCarregamentoAgrupamento;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoAgrupamento> BuscarAgrupamentoPorDisponivel()
        {
            var filaCarregamentoAgrupamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>()
                .Where(o =>
                    (o.Tipo == TipoFilaCarregamentoVeiculo.Vazio) &&
                    (o.Situacao == SituacaoFilaCarregamentoVeiculo.Disponivel) &&
                    (
                        (o.ConjuntoMotorista.FilaCarregamentoMotorista == null) ||
                        (o.ConjuntoMotorista.FilaCarregamentoMotorista.Situacao == SituacaoFilaCarregamentoMotorista.Disponivel)
                    )
                )
                .GroupBy(o =>
                    new
                    {
                        codigoCentroCarregamento = o.CentroCarregamento.Codigo,
                        codigoModeloVeicular = o.ConjuntoVeiculo.ModeloVeicularCarga.Codigo
                    },
                    (key, value) => new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoAgrupamento()
                    {
                        CodigoCentroCarregamento = key.codigoCentroCarregamento,
                        CodigoModeloVeicularCarga = key.codigoModeloVeicular
                    }
                )
                .Distinct()
                .ToList();

            return filaCarregamentoAgrupamento;
        }

        public Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo BuscarAtivaPorMotoristaNaFila(int codigo)
        {
            List<SituacaoFilaCarregamentoVeiculo> situacoesAtiva = SituacaoFilaCarregamentoVeiculoHelper.ObterSituacoesAtiva();

            var filaCarregamentoVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>()
                .Where(o => situacoesAtiva.Contains(o.Situacao) && (o.ConjuntoMotorista.Motorista.Codigo == codigo) && o.ConjuntoMotorista.FilaCarregamentoMotorista != null)
                .OrderByDescending(o => o.Codigo)
                .FirstOrDefault();

            return filaCarregamentoVeiculo;
        }

        public Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo BuscarAtivaPorVeiculo(int codigoVeiculo, int codigoCentroCarregamento, int codigoFilial, bool somenteSemCargaOuPreCargaAlocada)
        {
            List<SituacaoFilaCarregamentoVeiculo> situacoesAtiva = SituacaoFilaCarregamentoVeiculoHelper.ObterSituacoesAtivaSemEmTrasicao();

            var filaCarregamentoVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>()
                .Where(o =>
                    situacoesAtiva.Contains(o.Situacao) &&
                    ((o.ConjuntoVeiculo.Tracao.Codigo == codigoVeiculo) || (o.ConjuntoVeiculo.Reboques.Any(r => r.Codigo == codigoVeiculo))) &&
                    ((o.CentroCarregamento.Codigo == codigoCentroCarregamento) || (o.Filial.Codigo == codigoFilial))
                );

            if (somenteSemCargaOuPreCargaAlocada)
                filaCarregamentoVeiculo = filaCarregamentoVeiculo
                    .Where(o =>
                        (o.Carga == null && o.PreCarga == null) ||
                        (o.Situacao == SituacaoFilaCarregamentoVeiculo.AguardandoAceiteCarga || o.Situacao == SituacaoFilaCarregamentoVeiculo.AguardandoAceitePreCarga)
                    );

            return filaCarregamentoVeiculo
                .OrderByDescending(o => o.Codigo)
                .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo BuscarAtivaPorVeiculo(int codigoVeiculo)
        {
            List<SituacaoFilaCarregamentoVeiculo> situacoesAtiva = SituacaoFilaCarregamentoVeiculoHelper.ObterSituacoesAtivaSemEmTrasicao();

            var filaCarregamentoVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>()
                .Where(o =>
                    situacoesAtiva.Contains(o.Situacao) &&
                    (o.ConjuntoVeiculo.Tracao.Codigo == codigoVeiculo) || (o.ConjuntoVeiculo.Reboques.Any(r => r.Codigo == codigoVeiculo))
                );

            return filaCarregamentoVeiculo
                .OrderByDescending(o => o.Codigo)
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> BuscarCargaAguardandoConfirmacaoPorAgrupamento(Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoAgrupamento agrupamento)
        {
            var consultaFilaCarregamentoVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>()
                .Where(o =>
                    (o.CentroCarregamento.Codigo == agrupamento.CodigoCentroCarregamento) &&
                    (o.ConjuntoVeiculo.ModeloVeicularCarga.Codigo == agrupamento.CodigoModeloVeicularCarga) &&
                    (o.Situacao == SituacaoFilaCarregamentoVeiculo.AguardandoConfirmacao) &&
                    (o.ConjuntoMotorista.FilaCarregamentoMotorista.Situacao == SituacaoFilaCarregamentoMotorista.CargaAlocada)
                )
                .OrderBy(o => o.Posicao)
                .Take(5);

            return consultaFilaCarregamentoVeiculo.ToList();
        }

        public List<int> BuscarCodigosPorDataProgramadaAlterarAutomaticamente(DateTime? dataProgramadaInicial)
        {
            DateTime dataProgramada = DateTime.Today.AddDays(-1);

            var filaCarregamentoVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>()
                .Where(fila => (fila.Situacao == SituacaoFilaCarregamentoVeiculo.Disponivel) && (fila.DataProgramada != null) && (fila.DataProgramada.Value.Date == dataProgramada));

            if (dataProgramadaInicial.HasValue)
                filaCarregamentoVeiculo = filaCarregamentoVeiculo.Where(fila => fila.DataProgramadaInicial >= dataProgramadaInicial.Value.Date);

            return filaCarregamentoVeiculo
                .OrderBy(o => o.Posicao)
                .Select(o => o.Codigo)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> BuscarDisponivelPorAgrupamento(Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoAgrupamento agrupamento)
        {
            var consultaFilaCarregamentoVeiculo = ConsultarFilaCarregamentoVeiculoDisponivel(new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculoDisponivel()
            {
                CodigoCentroCarregamento = agrupamento.CodigoCentroCarregamento,
                CodigoModeloVeicularCarga = agrupamento.CodigoModeloVeicularCarga
            });

            consultaFilaCarregamentoVeiculo = consultaFilaCarregamentoVeiculo
                .OrderBy(o => o.Posicao)
                .Take(10);

            return consultaFilaCarregamentoVeiculo.ToList();
        }

        public Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo BuscarDisponivelPorMotoristaVincularFilaCarregamentoMotorista(int codigo)
        {
            List<SituacaoFilaCarregamentoVeiculo> situacoesVincularFilaCarregamentoMotorista = SituacaoFilaCarregamentoVeiculoHelper.ObterSituacoesVincularFilaCarregamentoMotorista();

            var filaCarregamentoVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>()
                .Where(o => situacoesVincularFilaCarregamentoMotorista.Contains(o.Situacao) && (o.ConjuntoMotorista.Motorista.Codigo == codigo) && (o.ConjuntoMotorista.FilaCarregamentoMotorista == null))
                .OrderByDescending(o => o.Codigo)
                .FirstOrDefault();

            return filaCarregamentoVeiculo;
        }

        public Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo BuscarEmTransicaoPorVeiculo(int codigo)
        {
            var filaCarregamentoVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>()
                .Where(o =>
                    (o.Situacao == SituacaoFilaCarregamentoVeiculo.EmTransicao) &&
                    ((o.ConjuntoVeiculo.Tracao.Codigo == codigo) || (o.ConjuntoVeiculo.Reboques.Any(r => r.Codigo == codigo)))
                )
                .OrderByDescending(o => o.Codigo)
                .FirstOrDefault();

            return filaCarregamentoVeiculo;
        }

        public List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> BuscarEmViagemPorMotorista(int codigo)
        {
            var consultaFilaCarregamentoVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>()
                .Where(o => (o.Situacao == SituacaoFilaCarregamentoVeiculo.EmViagem) && (o.ConjuntoMotorista.FilaCarregamentoMotorista.Motorista.Codigo == codigo))
                .OrderBy(o => o.Codigo);

            return consultaFilaCarregamentoVeiculo.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> BuscarEmViagemPorVeiculo(int codigo)
        {
            var consultaFilaCarregamentoVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>()
                .Where(o =>
                    (o.Situacao == SituacaoFilaCarregamentoVeiculo.EmViagem) &&
                    ((o.ConjuntoVeiculo.Tracao.Codigo == codigo) || o.ConjuntoVeiculo.Reboques.Any(r => r.Codigo == codigo))
                )
                .OrderBy(o => o.Codigo);

            return consultaFilaCarregamentoVeiculo.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> BuscarEmViagemPorVeiculosECentroCarregamento(List<int> codigosVeiculo, int codigoCentroCarregamento)
        {
            var consultaFilaCarregamentoVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>()
                .Where(o =>
                    (o.Situacao == SituacaoFilaCarregamentoVeiculo.EmViagem) &&
                    (o.CentroCarregamento.Codigo == codigoCentroCarregamento) &&
                    ((codigosVeiculo.Contains(o.ConjuntoVeiculo.Tracao.Codigo)) || o.ConjuntoVeiculo.Reboques.Any(r => codigosVeiculo.Contains(r.Codigo)))
                )
                .OrderBy(o => o.Codigo);

            return consultaFilaCarregamentoVeiculo.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> BuscarPorCarga(int codigo)
        {
            var consultaFilaCarregamentoVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>()
                .Where(o => o.Carga.Codigo == codigo);

            return consultaFilaCarregamentoVeiculo.ToList();
        }
        public Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo BuscarPrimeiroPorCarga(int codigo)
        {
            var consultaFilaCarregamentoVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>()
                .Where(o => o.Carga.Codigo == codigo);

            return consultaFilaCarregamentoVeiculo.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> BuscarPorCargaAtiva(int codigo)
        {
            List<SituacaoFilaCarregamentoVeiculo> situacoesAtiva = SituacaoFilaCarregamentoVeiculoHelper.ObterSituacoesCargaAtiva();

            var consultaFilaCarregamentoVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>()
                .Where(o => situacoesAtiva.Contains(o.Situacao) && (o.Carga.Codigo == codigo));

            return consultaFilaCarregamentoVeiculo.ToList();
        }

        public Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo BuscarPorCodigo(int codigo)
        {
            var filaCarregamentoVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>()
                .Where(o => o.Codigo == codigo)
                .FirstOrDefault();

            return filaCarregamentoVeiculo;
        }

        public List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> BuscarPorPosicaoAjustar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculo filtrosPesquisa, int posicaoAtual, int novaPosicao)
        {
            if (posicaoAtual < novaPosicao)
                return BuscarPorPosicaoAjustarSuperiorAtual(filtrosPesquisa, posicaoAtual, novaPosicao);

            return BuscarPorFilaPosicaoAjustarInferiorAtual(filtrosPesquisa, posicaoAtual, novaPosicao);
        }

        public List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> BuscarPorPosicaoSuperior(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculo filtrosPesquisa, int posicao)
        {
            var consultaFilaCarregamentoVeiculo = ConsultarFilaCarregamentoVeiculo(filtrosPesquisa);

            consultaFilaCarregamentoVeiculo = consultaFilaCarregamentoVeiculo.Where(o => o.Posicao > posicao);

            return consultaFilaCarregamentoVeiculo.ToList();
        }

        public Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo BuscarPorPreCarga(int codigo)
        {
            var filaCarregamentoVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>()
                .Where(o => o.PreCarga.Codigo == codigo)
                .FirstOrDefault();

            return filaCarregamentoVeiculo;
        }

        public Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo BuscarPorPreCargaAtiva(int codigo)
        {
            List<SituacaoFilaCarregamentoVeiculo> situacoesAtiva = SituacaoFilaCarregamentoVeiculoHelper.ObterSituacoesPreCargaAtiva();

            var filaCarregamentoVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>()
                .Where(o => situacoesAtiva.Contains(o.Situacao) && (o.PreCarga.Codigo == codigo))
                .FirstOrDefault();

            return filaCarregamentoVeiculo;
        }

        public Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo BuscarPrimeiraAtiva(int codigoVeiculo, int codigoMotoristaDesconsiderar)
        {
            List<SituacaoFilaCarregamentoVeiculo> situacoesAtiva = SituacaoFilaCarregamentoVeiculoHelper.ObterSituacoesAtiva();

            var filaCarregamentoVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>()
                .Where(o => situacoesAtiva.Contains(o.Situacao) && ((o.ConjuntoVeiculo.Tracao.Codigo == codigoVeiculo) || (o.ConjuntoVeiculo.Reboques.Any(r => r.Codigo == codigoVeiculo))));

            if (codigoMotoristaDesconsiderar > 0)
                filaCarregamentoVeiculo = filaCarregamentoVeiculo.Where(o => o.ConjuntoMotorista.Motorista.Codigo != codigoMotoristaDesconsiderar);

            return filaCarregamentoVeiculo
                .OrderByDescending(o => o.Codigo)
                .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo BuscarPrimeiraComCargaCancelada(int codigoVeiculo, int codigoCentroCarregamento, int codigoFilial)
        {
            var filaCarregamentoVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>()
                .Where(o =>
                    o.Situacao == SituacaoFilaCarregamentoVeiculo.CargaCancelada &&
                    ((o.ConjuntoVeiculo.Tracao.Codigo == codigoVeiculo) || (o.ConjuntoVeiculo.Reboques.Any(r => r.Codigo == codigoVeiculo))) &&
                    ((o.CentroCarregamento.Codigo == codigoCentroCarregamento) || (o.Filial.Codigo == codigoFilial))
                )
                .OrderByDescending(o => o.Codigo)
                .FirstOrDefault();

            return filaCarregamentoVeiculo;
        }

        public Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo BuscarPrimeiroDisponivelNaFila(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculoPrimeiroDisponivel filtrosPesquisa)
        {
            var consultaFilaCarregamentoVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>()
                .Where(o => (
                    (o.ConjuntoVeiculo.ModeloVeicularCarga.Codigo == filtrosPesquisa.CodigoModeloVeicularCarga) &&
                    (o.Situacao == SituacaoFilaCarregamentoVeiculo.Disponivel) &&
                    (o.Tipo == TipoFilaCarregamentoVeiculo.Vazio) &&
                    (
                        (o.ConjuntoMotorista.FilaCarregamentoMotorista == null) ||
                        (o.ConjuntoMotorista.FilaCarregamentoMotorista.Situacao == SituacaoFilaCarregamentoMotorista.Disponivel)
                    )
                ));

            if (filtrosPesquisa.CodigoCentroCarregamento > 0)
                consultaFilaCarregamentoVeiculo = consultaFilaCarregamentoVeiculo.Where(o => o.CentroCarregamento.Codigo == filtrosPesquisa.CodigoCentroCarregamento);
            else
                consultaFilaCarregamentoVeiculo = consultaFilaCarregamentoVeiculo.Where(o => o.Filial.Codigo == filtrosPesquisa.CodigoFilial);

            if (filtrosPesquisa.DataProgramada.HasValue)
                consultaFilaCarregamentoVeiculo = consultaFilaCarregamentoVeiculo.Where(o => (o.DataProgramada != null) && (o.DataProgramada.Value.Date <= filtrosPesquisa.DataProgramada.Value.Date));

            if (filtrosPesquisa.CodigoPreCargaDesconsiderar > 0)
            {
                var consultaHistorico = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoHistorico>()
                    .Where(historico => historico.PreCarga.Codigo == filtrosPesquisa.CodigoPreCargaDesconsiderar && historico.Tipo == TipoFilaCarregamentoVeiculoHistorico.PreCargaRecusada);

                consultaFilaCarregamentoVeiculo = consultaFilaCarregamentoVeiculo.Where(o => !consultaHistorico.Any(historico => historico.FilaCarregamentoVeiculo.Codigo == o.Codigo));
            }

            consultaFilaCarregamentoVeiculo = ObterConsultaFiltradaPorDestinos(consultaFilaCarregamentoVeiculo, filtrosPesquisa.CodigosDestinos, filtrosPesquisa.CodigosRegioesDestino, filtrosPesquisa.SiglasEstadosDestino);

            return consultaFilaCarregamentoVeiculo
                .OrderBy(o => o.Posicao)
                .FirstOrDefault();
        }

        public int BuscarProximaPosicao(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculoPosicao filtrosPesquisa)
        {
            return BuscarUltimaPosicao(filtrosPesquisa) + 1;
        }

        public Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo BuscarFilaVeiculoPorCodigoConjuntoVeiculo(int codigoConjuntoVeiculo)
        {
            var consultaFilaCarregamentoVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>()
                 .Where(o => o.ConjuntoVeiculo.Codigo == codigoConjuntoVeiculo);

            return consultaFilaCarregamentoVeiculo.FirstOrDefault();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Logistica.GraficoFilaCarregamentoResumo> BuscarResumo(int codigoCentroCarregamento, int codigoFilial, int codigoGrupoModeloVeicularCarga, bool exibirResumoSomentePorModeloVeicularCarga)
        {
            var sql = ((codigoGrupoModeloVeicularCarga > 0) || exibirResumoSomentePorModeloVeicularCarga) ? ObterSqlConsultaResumoPorModeloVeicularCarga(codigoCentroCarregamento, codigoFilial, codigoGrupoModeloVeicularCarga) : ObterSqlConsultaResumoPorGrupoModeloVeicularCarga(codigoCentroCarregamento, codigoFilial);
            var consultaResumo = this.SessionNHiBernate.CreateSQLQuery(sql);

            consultaResumo.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.GraficoFilaCarregamentoResumo)));

            return consultaResumo.List<Dominio.ObjetosDeValor.Embarcador.Logistica.GraficoFilaCarregamentoResumo>();
        }

        public int BuscarUltimaPosicao(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculoPosicao filtrosPesquisa)
        {
            List<SituacaoFilaCarregamentoVeiculo> situacoesNaFila = SituacaoFilaCarregamentoVeiculoHelper.ObterSituacoesNaFila();

            var consultaFilaCarregamentoVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>()
                .Where(o => situacoesNaFila.Contains(o.Situacao));

            if (filtrosPesquisa.CodigoModeloVeicularCarga > 0)
                consultaFilaCarregamentoVeiculo = consultaFilaCarregamentoVeiculo.Where(o => o.ConjuntoVeiculo.ModeloVeicularCarga.Codigo == filtrosPesquisa.CodigoModeloVeicularCarga);

            if (filtrosPesquisa.CodigoCentroCarregamento > 0)
                consultaFilaCarregamentoVeiculo = consultaFilaCarregamentoVeiculo.Where(o => o.CentroCarregamento.Codigo == filtrosPesquisa.CodigoCentroCarregamento);
            else
                consultaFilaCarregamentoVeiculo = consultaFilaCarregamentoVeiculo.Where(o => o.Filial.Codigo == filtrosPesquisa.CodigoFilial);

            if (filtrosPesquisa.CodigoFilaCarregamentoVeiculoDesconsiderar > 0)
                consultaFilaCarregamentoVeiculo = consultaFilaCarregamentoVeiculo.Where(o => o.Codigo != filtrosPesquisa.CodigoFilaCarregamentoVeiculoDesconsiderar);

            if (filtrosPesquisa.DataProgramada.HasValue)
            {
                DateTime dataEntrada = filtrosPesquisa.DataEntrada ?? DateTime.Now;

                consultaFilaCarregamentoVeiculo = consultaFilaCarregamentoVeiculo.Where(o =>
                    ((o.DataProgramada < filtrosPesquisa.DataProgramada.Value) || (o.DataProgramada == filtrosPesquisa.DataProgramada.Value && o.DataEntrada <= dataEntrada)) &&
                    ((bool?)o.ConjuntoVeiculoDedicado ?? false) == filtrosPesquisa.ConjuntoVeiculoDedicado
                );
            }

            return consultaFilaCarregamentoVeiculo.Count();
        }

        public List<dynamic> ConsultaGraficoProximidadeAcompanhamentoFilaCarregamentoReversa(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAcompanhamentoFilaCarregamentoReversa filtrosPesquisa)
        {
            List<dynamic> listaDadosGraficoProximidade = new List<dynamic>();
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento>()
            {
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento()
                {
                    CodigoDinamico = 0,
                    Propriedade = "Codigo"
                }
            };

            filtrosPesquisa.LojaProximidade = true;
            int proximidadeSim = ContarConsultaAcompanhamentoReversa(filtrosPesquisa, propriedades);

            if (proximidadeSim > 0)
                listaDadosGraficoProximidade.Add(new { label = "Sim", value = proximidadeSim, color = "#1cb25b" });

            filtrosPesquisa.LojaProximidade = false;
            int proximidadeNao = ContarConsultaAcompanhamentoReversa(filtrosPesquisa, propriedades);

            if (proximidadeNao > 0)
                listaDadosGraficoProximidade.Add(new { label = "Não", value = proximidadeNao, color = "#cc8118" });

            return listaDadosGraficoProximidade;
        }

        public List<dynamic> ConsultaGraficoReversaAcompanhamentoFilaCarregamentoReversa(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAcompanhamentoFilaCarregamentoReversa filtrosPesquisa)
        {
            List<dynamic> listaDadosGraficoReversa = new List<dynamic>();

            int reversaSim = ConsultarFilaCarregamentoVeiculoAcompanhamentoReversa(filtrosPesquisa, TipoFilaCarregamentoVeiculo.Reversa).Count();

            if (reversaSim > 0)
                listaDadosGraficoReversa.Add(new { label = "Sim", value = reversaSim, color = "#1cb25b" });

            int reversaNao = ConsultarFilaCarregamentoVeiculoAcompanhamentoReversa(filtrosPesquisa, TipoFilaCarregamentoVeiculo.Vazio).Count();

            if (reversaNao > 0)
                listaDadosGraficoReversa.Add(new { label = "Não", value = reversaNao, color = "#cc8118" });

            return listaDadosGraficoReversa;
        }

        public List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculo filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaFilaCarregamentoVeiculo = ConsultarFilaCarregamentoVeiculo(filtrosPesquisa);

            consultaFilaCarregamentoVeiculo = consultaFilaCarregamentoVeiculo
                .Fetch(fila => fila.ConjuntoMotorista).ThenFetch(conjuntoMotorista => conjuntoMotorista.Motorista)
                .Fetch(fila => fila.ConjuntoVeiculo).ThenFetch(conjuntoVeiculo => conjuntoVeiculo.ModeloVeicularCarga)
                .Fetch(fila => fila.ConjuntoVeiculo).ThenFetch(conjuntoVeiculo => conjuntoVeiculo.Tracao).ThenFetch(tracao => tracao.Empresa).ThenFetch(empresa => empresa.Localidade)
                .Fetch(fila => fila.CentroCarregamento).ThenFetch(centro => centro.Filial)
                .Fetch(fila => fila.Filial)
                .Fetch(fila => fila.PreCarga)
                .Fetch(fila => fila.Carga);

            return ObterLista(consultaFilaCarregamentoVeiculo, parametrosConsulta);
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Logistica.AcompanhamentoFilaCarregamentoReversa> ConsultarAcompanhamentoReversa(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAcompanhamentoFilaCarregamentoReversa filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaAcompanhamentoReversa = new Repositorio.Embarcador.Logistica.Consulta.ConsultaAcompanhamentoFilaCarregamentoReversa().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaAcompanhamentoReversa.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Logistica.AcompanhamentoFilaCarregamentoReversa)));

            return consultaAcompanhamentoReversa.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Logistica.AcompanhamentoFilaCarregamentoReversa>();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> ConsultarDisponivelParaPreCarga(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculoPreCarga filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaFilaCarregamentoVeiculo = ConsultarFilaCarregamentoVeiculoDisponivelParaPreCarga(filtrosPesquisa);

            consultaFilaCarregamentoVeiculo = consultaFilaCarregamentoVeiculo
                .Fetch(fila => fila.ConjuntoMotorista).ThenFetch(conjuntoMotorista => conjuntoMotorista.Motorista)
                .Fetch(fila => fila.ConjuntoVeiculo).ThenFetch(conjuntoVeiculo => conjuntoVeiculo.ModeloVeicularCarga)
                .Fetch(fila => fila.ConjuntoVeiculo).ThenFetch(conjuntoVeiculo => conjuntoVeiculo.Tracao).ThenFetch(tracao => tracao.Empresa).ThenFetch(empresa => empresa.Localidade);

            return ObterLista(consultaFilaCarregamentoVeiculo, parametrosConsulta);
        }

        public List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> ConsultarDisponivelPorAgrupamento(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculoDisponivel filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaFilaCarregamentoVeiculo = ConsultarFilaCarregamentoVeiculoDisponivel(filtrosPesquisa);

            return ObterLista(consultaFilaCarregamentoVeiculo, parametrosConsulta);
        }

        public List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> ConsultarPorVinculoExterno(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculoVinculoExterno filtrosPesquisa)
        {
            var consultaFilaCarregamentoVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>()
                .Where(o => o.Situacao == filtrosPesquisa.Situacao);

            if (filtrosPesquisa.CentroCarregamento != null)
                consultaFilaCarregamentoVeiculo = consultaFilaCarregamentoVeiculo.Where(o => o.CentroCarregamento.Codigo == filtrosPesquisa.CentroCarregamento.Codigo);

            if (filtrosPesquisa.Tracao != null)
                consultaFilaCarregamentoVeiculo = consultaFilaCarregamentoVeiculo.Where(o => o.ConjuntoVeiculo.Tracao.Codigo == filtrosPesquisa.Tracao.Codigo);

            if (filtrosPesquisa.Reboques?.Count > 0)
            {
                var consultaFilaCarregamentoConjuntoVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo>()
                    .Where(o => o.Reboques.Count == filtrosPesquisa.Reboques.Count && o.Reboques.All(r => filtrosPesquisa.Reboques.Contains(r)));

                consultaFilaCarregamentoVeiculo = consultaFilaCarregamentoVeiculo.Where(o => consultaFilaCarregamentoConjuntoVeiculo.Any(c => c.Codigo == o.ConjuntoVeiculo.Codigo));
            }

            return consultaFilaCarregamentoVeiculo.ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculo filtrosPesquisa)
        {
            var consultaFilaCarregamentoVeiculo = ConsultarFilaCarregamentoVeiculo(filtrosPesquisa);

            return consultaFilaCarregamentoVeiculo.Count();
        }

        public int ContarConsultaAcompanhamentoReversa(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAcompanhamentoFilaCarregamentoReversa filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consultaAcompanhamentoReversa = new Repositorio.Embarcador.Logistica.Consulta.ConsultaAcompanhamentoFilaCarregamentoReversa().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaAcompanhamentoReversa.SetTimeout(600).UniqueResult<int>();
        }

        public int ContarConsultaDisponivelParaPreCarga(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculoPreCarga filtrosPesquisa)
        {
            var consultaFilaCarregamentoVeiculo = ConsultarFilaCarregamentoVeiculoDisponivelParaPreCarga(filtrosPesquisa);

            return consultaFilaCarregamentoVeiculo.Count();
        }

        public int ContarConsultaDisponivelPorAgrupamento(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculoDisponivel filtrosPesquisa)
        {
            var consultaFilaCarregamentoVeiculo = ConsultarFilaCarregamentoVeiculoDisponivel(filtrosPesquisa);

            return consultaFilaCarregamentoVeiculo.Count();
        }

        public Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoTotalizador BuscarTotalizadorPorSituacao(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculo filtrosPesquisa)
        {
            StringBuilder sql = new StringBuilder();

            sql.AppendLine("select isnull([1], 0) as AguardandoConfirmacao,    ");
            sql.AppendLine("       isnull([2], 0) as AguardandoConjuntos,      ");
            sql.AppendLine("       isnull([3], 0) as CargaCancelada,           ");
            sql.AppendLine("       isnull([4], 0) as EmRemocao,                ");
            sql.AppendLine("       isnull([5], 0) as AguardandoAceite,         ");
            sql.AppendLine("       isnull([6], 0) as EmChecklist,              ");
            sql.AppendLine("       isnull([7], 0) as AguardandoCarga,          ");
            sql.AppendLine("       isnull([8], 0) as AguardandoDesatrelar,     ");
            sql.AppendLine("       isnull([9], 0) as EmReversa,                ");
            sql.AppendLine("       isnull([10], 0) as Vazio,                   ");
            sql.AppendLine("       isnull([11], 0) as CargaRecusada,           ");
            sql.AppendLine("       isnull([12], 0) as PerdeuSenha,             ");
            sql.AppendLine("       isnull([13], 0) as EmViagem,                ");
            sql.AppendLine("       isnull([14], 0) as AguardandoAceitePreCarga ");
            sql.AppendLine("  from (                                           ");
            sql.AppendLine("            select TotalizadoresPorTipo.Tipo,      ");
            sql.AppendLine("                   count(TotalizadoresPorTipo.Codigo) TotalRegistros ");
            sql.AppendLine("              from ( ");
            sql.AppendLine("                       select FilaCarregamento.FLV_CODIGO as Codigo, ");
            sql.AppendLine("                              (case ");
            sql.AppendLine($"                                 when FilaCarregamento.FLV_SITUACAO = {(int)SituacaoFilaCarregamentoVeiculo.AguardandoConfirmacao} then 1 ");
            sql.AppendLine($"                                 when FilaCarregamento.FLV_SITUACAO = {(int)SituacaoFilaCarregamentoVeiculo.AguardandoConjuntos} then 2 ");
            sql.AppendLine($"                                 when FilaCarregamento.FLV_SITUACAO = {(int)SituacaoFilaCarregamentoVeiculo.CargaCancelada} then 3 ");
            sql.AppendLine($"                                 when FilaCarregamento.FLV_SITUACAO = {(int)SituacaoFilaCarregamentoVeiculo.EmRemocao} then 4 ");
            sql.AppendLine($"                                 when FilaCarregamento.FLV_SITUACAO = {(int)SituacaoFilaCarregamentoVeiculo.AguardandoAceiteCarga} then 5 ");
            sql.AppendLine($"                                 when FilaCarregamento.FLV_SITUACAO = {(int)SituacaoFilaCarregamentoVeiculo.EmChecklist} then 6 ");
            sql.AppendLine($"                                 when FilaCarregamento.FLV_SITUACAO = {(int)SituacaoFilaCarregamentoVeiculo.AguardandoCarga} then 7 ");
            sql.AppendLine($"                                 when FilaCarregamento.FLV_SITUACAO = {(int)SituacaoFilaCarregamentoVeiculo.ReboqueAtrelado} then 8 ");
            sql.AppendLine($"                                 when (FilaCarregamento.FLV_SITUACAO = {(int)SituacaoFilaCarregamentoVeiculo.Disponivel} and isnull(FilaMotorista.FLM_SITUACAO, {(int)SituacaoFilaCarregamentoMotorista.Disponivel}) = {(int)SituacaoFilaCarregamentoMotorista.Disponivel} and FilaCarregamento.FLV_TIPO = {(int)TipoFilaCarregamentoVeiculo.Reversa}) then 9 ");
            sql.AppendLine($"                                 when (FilaCarregamento.FLV_SITUACAO = {(int)SituacaoFilaCarregamentoVeiculo.Disponivel} and isnull(FilaMotorista.FLM_SITUACAO, {(int)SituacaoFilaCarregamentoMotorista.Disponivel}) = {(int)SituacaoFilaCarregamentoMotorista.Disponivel} and FilaCarregamento.FLV_TIPO = {(int)TipoFilaCarregamentoVeiculo.Vazio}) then 10 ");
            sql.AppendLine($"                                 when (FilaCarregamento.FLV_SITUACAO = {(int)SituacaoFilaCarregamentoVeiculo.Disponivel} and FilaMotorista.FLM_SITUACAO = {(int)SituacaoFilaCarregamentoMotorista.CargaRecusada}) then 11 ");
            sql.AppendLine($"                                 when (FilaCarregamento.FLV_SITUACAO = {(int)SituacaoFilaCarregamentoVeiculo.Disponivel} and FilaMotorista.FLM_SITUACAO = {(int)SituacaoFilaCarregamentoMotorista.SenhaPerdida}) then 12 ");
            sql.AppendLine($"                                 when FilaCarregamento.FLV_SITUACAO = {(int)SituacaoFilaCarregamentoVeiculo.EmViagem} then 13 ");
            sql.AppendLine($"                                 when FilaCarregamento.FLV_SITUACAO = {(int)SituacaoFilaCarregamentoVeiculo.AguardandoAceitePreCarga} then 14 ");
            sql.AppendLine("                                  else 0 ");
            sql.AppendLine("                              end) as Tipo ");
            sql.AppendLine("                         from T_FILA_CARREGAMENTO_VEICULO FilaCarregamento ");
            sql.AppendLine("                         join T_FILA_CARREGAMENTO_CONJUNTO_VEICULO ConjuntoVeiculo on ConjuntoVeiculo.FCV_CODIGO = FilaCarregamento.FCV_CODIGO ");
            sql.AppendLine("                         join T_FILA_CARREGAMENTO_CONJUNTO_MOTORISTA ConjuntoMotorista on ConjuntoMotorista.FCM_CODIGO = FilaCarregamento.FCM_CODIGO ");
            sql.AppendLine("                         left join T_FILA_CARREGAMENTO_MOTORISTA FilaMotorista on FilaMotorista.FLM_CODIGO = ConjuntoMotorista.FLM_CODIGO ");
            sql.AppendLine("                         left join T_MODELO_VEICULAR_CARGA ModeloVeicularCarga on ModeloVeicularCarga.MVC_CODIGO = ConjuntoVeiculo.MVC_CODIGO ");
            sql.AppendLine("                         left join T_VEICULO Tracao on Tracao.VEI_CODIGO = ConjuntoVeiculo.FCV_CODIGO_TRACAO ");
            sql.AppendLine("                         left join T_PRE_CARGA PreCarga on PreCarga.PCA_CODIGO = FilaCarregamento.PCA_CODIGO ");
            sql.AppendLine("                         left join T_CARGA Carga on Carga.CAR_CODIGO = FilaCarregamento.CAR_CODIGO ");
            sql.AppendLine("                        where 1 = 1 ");

            if (filtrosPesquisa.CodigoCentroCarregamento > 0)
                sql.AppendLine($"                     and FilaCarregamento.CEC_CODIGO = {filtrosPesquisa.CodigoCentroCarregamento} ");
            else if (filtrosPesquisa.CodigoFilial > 0)
                sql.AppendLine($"                     and FilaCarregamento.FIL_CODIGO = {filtrosPesquisa.CodigoFilial} ");

            if (filtrosPesquisa.CodigoGrupoModeloVeicularCarga > 0)
                sql.AppendLine($"                     and ModeloVeicularCarga.MVG_CODIGO = {filtrosPesquisa.CodigoGrupoModeloVeicularCarga} ");

            if (filtrosPesquisa.CodigosModeloVeicularCarga?.Count > 0)
                sql.AppendLine($"                     and ConjuntoVeiculo.MVC_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosModeloVeicularCarga)}) ");

            if (filtrosPesquisa.CodigosCarga?.Count > 0)
                sql.AppendLine($"                     and FilaCarregamento.CAR_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosCarga)}) ");

            if (filtrosPesquisa.CodigosConfiguracaoProgramacaoCarga?.Count > 0)
                sql.AppendLine($"                     and PreCarga.CPC_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosConfiguracaoProgramacaoCarga)}) ");

            if (filtrosPesquisa.CodigosTipoOperacao?.Count > 0)
                sql.AppendLine($"                     and isnull(Carga.TOP_CODIGO, PreCarga.TOP_CODIGO) in ({string.Join(", ", filtrosPesquisa.CodigosTipoOperacao)}) ");

            if (filtrosPesquisa.DataProgramadaInicial.HasValue)
                sql.AppendLine($"                     and FilaCarregamento.FLV_DATA_PROGRAMADA >= '{filtrosPesquisa.DataProgramadaInicial.Value.Date:yyyyMMdd HH:mm:ss}' ");

            if (filtrosPesquisa.DataProgramadaFinal.HasValue)
                sql.AppendLine($"                     and FilaCarregamento.FLV_DATA_PROGRAMADA <= '{filtrosPesquisa.DataProgramadaFinal.Value.Date.Add(DateTime.MaxValue.TimeOfDay):yyyyMMdd HH:mm:ss}' ");

            if (filtrosPesquisa.CodigosDestino?.Count > 0)
            {
                sql.AppendLine("                     and exists ( ");
                sql.AppendLine("                             select 1 ");
                sql.AppendLine("                               from T_FILA_CARREGAMENTO_VEICULO_DESTINO Destino ");
                sql.AppendLine("                              where Destino.FLV_CODIGO = FilaCarregamento.FLV_CODIGO ");
                sql.AppendLine($"                               and Destino.LOC_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosDestino)}) ");
                sql.AppendLine("                         ) ");
            }

            if (filtrosPesquisa.SiglasEstadoDestino?.Count > 0)
            {
                sql.AppendLine("                     and exists ( ");
                sql.AppendLine("                             select 1 ");
                sql.AppendLine("                               from T_FILA_CARREGAMENTO_VEICULO_ESTADO_DESTINO EstadoDestino ");
                sql.AppendLine("                              where EstadoDestino.FLV_CODIGO = FilaCarregamento.FLV_CODIGO ");
                sql.AppendLine($"                               and EstadoDestino.UF_SIGLA in ({string.Join(", ", filtrosPesquisa.SiglasEstadoDestino.Select(sigla => $"'{sigla}'"))}) ");
                sql.AppendLine("                         ) ");
            }

            if (filtrosPesquisa.CodigosRegiaoDestino?.Count > 0)
            {
                sql.AppendLine("                     and exists ( ");
                sql.AppendLine("                             select 1 ");
                sql.AppendLine("                               from T_FILA_CARREGAMENTO_VEICULO_REGIAO_DESTINO RegiaoDestino ");
                sql.AppendLine("                              where RegiaoDestino.FLV_CODIGO = FilaCarregamento.FLV_CODIGO ");
                sql.AppendLine($"                               and RegiaoDestino.REG_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosRegiaoDestino)}) ");
                sql.AppendLine("                         ) ");
            }

            if (filtrosPesquisa.CodigosTipoCarga?.Count > 0)
            {
                sql.AppendLine("                     and exists ( ");
                sql.AppendLine("                             select 1 ");
                sql.AppendLine("                               from T_FILA_CARREGAMENTO_VEICULO_TIPO_CARGA TipoCarga ");
                sql.AppendLine("                              where TipoCarga.FLV_CODIGO = FilaCarregamento.FLV_CODIGO ");
                sql.AppendLine($"                               and TipoCarga.TCG_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTipoCarga)}) ");
                sql.AppendLine("                         ) ");
            }

            if (filtrosPesquisa.CodigosTransportador?.Count > 0)
            {
                string codigosTransportador = string.Join(", ", filtrosPesquisa.CodigosTransportador);

                sql.AppendLine("                      and ( ");
                sql.AppendLine($"                             Tracao.EMP_CODIGO in ({codigosTransportador}) or exists ( ");
                sql.AppendLine("                                  select 1 ");
                sql.AppendLine("                                    from T_FILA_CARREGAMENTO_CONJUNTO_VEICULO_REBOQUE ConjuntoReboque ");
                sql.AppendLine("                                    join T_VEICULO Reboque on Reboque.VEI_CODIGO = ConjuntoReboque.VEI_CODIGO ");
                sql.AppendLine("                                   where ConjuntoReboque.FCV_CODIGO_REBOQUE = ConjuntoVeiculo.FCV_CODIGO  ");
                sql.AppendLine($"                                    and Reboque.EMP_CODIGO in ({codigosTransportador}) ");
                sql.AppendLine("                              ) ");
                sql.AppendLine("                          ) ");
            }

            if (filtrosPesquisa.CodigosVeiculo?.Count > 0)
            {
                string codigosVeiculo = string.Join(", ", filtrosPesquisa.CodigosVeiculo);

                sql.AppendLine("                      and ( ");
                sql.AppendLine($"                             ConjuntoVeiculo.FCV_CODIGO_TRACAO in ({codigosVeiculo}) or exists ( ");
                sql.AppendLine("                                  select 1 ");
                sql.AppendLine("                                    from T_FILA_CARREGAMENTO_CONJUNTO_VEICULO_REBOQUE ConjuntoReboque ");
                sql.AppendLine("                                   where ConjuntoReboque.FCV_CODIGO_REBOQUE = ConjuntoVeiculo.FCV_CODIGO ");
                sql.AppendLine($"                                    and ConjuntoReboque.VEI_CODIGO in ({codigosVeiculo}) ");
                sql.AppendLine("                              ) ");
                sql.AppendLine("                          ) ");
            }

            sql.AppendLine("                   ) as TotalizadoresPorTipo ");
            sql.AppendLine("             where TotalizadoresPorTipo.Tipo in (1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14) ");
            sql.AppendLine("             group by TotalizadoresPorTipo.Tipo ");
            sql.AppendLine("       ) as FilaCarregamentoTotalizadores ");
            sql.AppendLine(" pivot ( ");
            sql.AppendLine("           sum (FilaCarregamentoTotalizadores.TotalRegistros) ");
            sql.AppendLine("           for FilaCarregamentoTotalizadores.Tipo in ([1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12], [13], [14]) ");
            sql.AppendLine("       ) as Totais ");

            var consultaTotalizadores = this.SessionNHiBernate.CreateSQLQuery(sql.ToString());

            consultaTotalizadores.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoTotalizador)));

            return consultaTotalizadores.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoTotalizador>().FirstOrDefault();
        }

        public bool ExistePorCarga(int codigo)
        {
            var consultaFilaCarregamentoVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>()
                .Where(o => o.Carga.Codigo == codigo);

            return consultaFilaCarregamentoVeiculo.Count() > 0;
        }

        public bool ExisteEquipamentoEmUsoFilaCarregamento(int codigoEquipamento)
        {
            var consultaFilaCarregamentoVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>()
                .Where(o => o.Equipamento.Codigo == codigoEquipamento && SituacaoFilaCarregamentoVeiculoHelper.ObterSituacoesNaFila().Contains(o.Situacao));

            return consultaFilaCarregamentoVeiculo.Any();
        }

        public Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo ExisteFilaCarregamentoVeiculoSituacao(int codigoVeiculo, SituacaoFilaCarregamentoVeiculo[] situacoes)
        {
            var consultaFilaCarregamentoVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>()
                .Where(o => ((o.ConjuntoVeiculo.Tracao.Codigo == codigoVeiculo) || (o.ConjuntoVeiculo.Reboques.Any(r => r.Codigo == codigoVeiculo))) && !situacoes.Contains(o.Situacao));

            return consultaFilaCarregamentoVeiculo
                .OrderByDescending(o => o.Codigo)
                .FirstOrDefault();
        }
        public bool ExistePorCargaESituacao(int codigoCarga, SituacaoFilaCarregamentoVeiculo situacao)
        {
            var consultaFilaCarregamentoVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>()
                .Where(o => o.Carga.Codigo == codigoCarga && o.Situacao == situacao);

            return consultaFilaCarregamentoVeiculo.Count() > 0;
        }

        public bool ExistePorDataProgramada(DateTime dataProgramada, int codigoCentroCarregamento, int codigoFilial, List<int> codigosVeiculos, int codigoFilaCarregamentoVeiculoDesconsiderar)
        {
            var filaCarregamentoVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>()
                .Where(o =>
                    o.Codigo != codigoFilaCarregamentoVeiculoDesconsiderar &&
                    o.Situacao == SituacaoFilaCarregamentoVeiculo.Disponivel &&
                    ((o.CentroCarregamento.Codigo == codigoCentroCarregamento) || (o.Filial.Codigo == codigoFilial)) &&
                    (codigosVeiculos.Contains(o.ConjuntoVeiculo.Tracao.Codigo) || o.ConjuntoVeiculo.Reboques.Any(r => codigosVeiculos.Contains(r.Codigo))) &&
                    (o.DataProgramada != null && o.DataProgramada.Value.Date == dataProgramada.Date)
                );

            return filaCarregamentoVeiculo.Count() > 0;
        }

        public Dominio.Entidades.Embarcador.Veiculos.Equipamento BuscarEquipamentoVinculado(int codigoCargaBase, bool isCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>();

            if (isCarga)
                query = query.Where(obj => obj.Carga.Codigo == codigoCargaBase);
            else
                query = query.Where(obj => obj.PreCarga.Codigo == codigoCargaBase);

            return query.Select(obj => obj.Equipamento).FirstOrDefault();
        }

        #endregion
    }
}
