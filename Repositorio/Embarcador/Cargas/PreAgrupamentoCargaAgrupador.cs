using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.Cargas
{
    public sealed class PreAgrupamentoCargaAgrupador : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaAgrupador>
    {
        #region Construtores

        public PreAgrupamentoCargaAgrupador(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento> ObterConsultaCargasEmCancelamento()
        {
            var consultaCancelamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento>()
                .Where(obj =>
                    obj.Situacao == SituacaoCancelamentoCarga.AgCancelamentoAverbacaoCTe ||
                    obj.Situacao == SituacaoCancelamentoCarga.AgCancelamentoAverbacaoMDFe ||
                    obj.Situacao == SituacaoCancelamentoCarga.AgCancelamentoCTe ||
                    obj.Situacao == SituacaoCancelamentoCarga.AgCancelamentoMDFe ||
                    obj.Situacao == SituacaoCancelamentoCarga.AgIntegracao ||
                    obj.Situacao == SituacaoCancelamentoCarga.EmCancelamento ||
                    obj.Situacao == SituacaoCancelamentoCarga.FinalizandoCancelamento
                );

            return consultaCancelamento;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaAgrupador BuscarPorCodigo(int codigo)
        {
            var consultaAgrupador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaAgrupador>()
                .Where(o => o.Codigo == codigo);

            return consultaAgrupador.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaAgrupador BuscarPorCodigoCargaESituacao(int codigoCarga, SituacaoPreAgrupamentoCarga situacao)
        {
            var consultaAgrupador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaAgrupador>()
                .Where(o => o.Carga.Codigo == codigoCarga && o.Situacao == situacao);

            return consultaAgrupador.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaAgrupador BuscarPorCodigoAgrupamento(int codigoAgrupamento)
        {
            var consultaAgrupador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaAgrupador>()
                .Where(o => o.CodigoAgrupamento == codigoAgrupamento);

            return consultaAgrupador.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaAgrupador> BucarPorSituacaoEncaixePendente(int limiteRegistros)
        {
            var consultaAgrupador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaAgrupador>()
                .Where(o => o.Situacao == SituacaoPreAgrupamentoCarga.AguardandoCargasPararEncaixe && o.PossuiPreCargas == false);

            var consultaCargasEmCancelamento = ObterConsultaCargasEmCancelamento();

            consultaAgrupador = consultaAgrupador.Where(obj =>
                !obj.Agrupamentos.Any(agrupamento =>
                    agrupamento.Carga.CalculandoFrete ||
                    agrupamento.Carga.PendenteGerarCargaDistribuidor ||
                    consultaCargasEmCancelamento.Select(cancelamento => cancelamento.Carga.Codigo).Any(codigoCargaEmCancelamento => codigoCargaEmCancelamento == agrupamento.Carga.Codigo)
                )
            );

            return consultaAgrupador.Take(limiteRegistros).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaAgrupador> BucarPorSituacaoAguardandoCarregamento(int limiteRegistros)
        {
            var consultaAgrupador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaAgrupador>()
                .Where(o => o.Situacao == SituacaoPreAgrupamentoCarga.AguardandoCarregamento);

            var consultaCargasEmCancelamento = ObterConsultaCargasEmCancelamento();

            consultaAgrupador = consultaAgrupador.Where(obj =>
                !obj.Agrupamentos.Any(agrupamento =>
                    agrupamento.Carga.CalculandoFrete ||
                    agrupamento.Carga.PendenteGerarCargaDistribuidor ||
                    consultaCargasEmCancelamento.Select(cancelamento => cancelamento.Carga.Codigo).Any(codigoCargaEmCancelamento => codigoCargaEmCancelamento == agrupamento.Carga.Codigo)
                )
            );

            return consultaAgrupador.Take(limiteRegistros).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaAgrupador> BucarPorSituacaoAguardandoProcessamento(int limiteRegistros)
        {
            var consultaAgrupador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaAgrupador>()
                .Where(o => o.Situacao == SituacaoPreAgrupamentoCarga.AguardandoProcessamento);

            return consultaAgrupador.Take(limiteRegistros).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaAgrupador> BucarPorSituacaoAguardandoRedespacho(int limiteRegistros)
        {
            var consultaAgrupador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaAgrupador>()
                .Where(o => o.Situacao == SituacaoPreAgrupamentoCarga.AguardandoRedespacho);

            var consultaCargasEmCancelamento = ObterConsultaCargasEmCancelamento();

            consultaAgrupador = consultaAgrupador.Where(obj =>
                !obj.Agrupamentos.Any(agrupamento =>
                    agrupamento.CargaRedespacho != null && (
                    agrupamento.CargaRedespacho.CalculandoFrete ||
                    (
                        //agrupamento.CargaRedespacho.SituacaoCarga != SituacaoCarga.AgIntegracao &&
                        agrupamento.CargaRedespacho.SituacaoCarga != SituacaoCarga.AgImpressaoDocumentos &&
                        agrupamento.CargaRedespacho.SituacaoCarga != SituacaoCarga.EmTransporte &&
                        agrupamento.CargaRedespacho.SituacaoCarga != SituacaoCarga.Encerrada
                    ) ||
                    consultaCargasEmCancelamento.Select(cancelamento => cancelamento.Carga.Codigo).Any(codigoCargaEmCancelamento => codigoCargaEmCancelamento == agrupamento.CargaRedespacho.Codigo))
                )
            );

            return consultaAgrupador
              .Take(limiteRegistros)
              .Fetch(obj => obj.Veiculo)
                  .ThenFetch(v => v.VeiculosVinculados) 
              .Fetch(obj => obj.Empresa)
              .ToList();

        }

        #endregion
    }
}
