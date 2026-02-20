using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaControleExpedicao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaControleExpedicao>
    {
        #region Construtores

        public CargaControleExpedicao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaControleExpedicao> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaControleExpedicao filtrosPesquisa)
        {
            var consultaCargaControleExpedicao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaControleExpedicao>()
                .Where(o =>
                    o.Carga.CargaFechada &&
                    o.Carga.OcultarNoPatio == false &&
                    (o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada) &&
                    (o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada) &&
                    (o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova) &&
                    (o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete) &&
                    (o.Carga.Empresa != null) &&
                    (o.FluxoGestaoPatio == null || o.FluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEtapaFluxoGestaoPatio.Cancelado)
                );

            consultaCargaControleExpedicao = (from obj in consultaCargaControleExpedicao where (obj.Carga != null && (obj.Carga.CargaFechada == true && !obj.Carga.CargaAgrupada) || (obj.Carga.CargaFechada == false && obj.Carga.CargaAgrupamento != null)) select obj);

            if (filtrosPesquisa.CodigosFilial?.Count > 0)
                consultaCargaControleExpedicao = consultaCargaControleExpedicao.Where(o =>
                    (o.FluxoGestaoPatio != null && filtrosPesquisa.CodigosFilial.Contains(o.FluxoGestaoPatio.Filial.Codigo)) ||
                    (o.FluxoGestaoPatio == null && filtrosPesquisa.CodigosFilial.Contains(o.Carga.Filial.Codigo))
                );

            if (filtrosPesquisa.DataInicial.HasValue)
                consultaCargaControleExpedicao = consultaCargaControleExpedicao.Where(o => o.Carga.DataCarregamentoCarga.Value.Date >= filtrosPesquisa.DataInicial.Value.Date);

            if (filtrosPesquisa.DataLimite.HasValue)
                consultaCargaControleExpedicao = consultaCargaControleExpedicao.Where(o => o.Carga.DataCarregamentoCarga.Value.Date <= filtrosPesquisa.DataLimite.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoCargaEmbarcador))
                consultaCargaControleExpedicao = consultaCargaControleExpedicao.Where(o => o.Carga.CodigoCargaEmbarcador == filtrosPesquisa.CodigoCargaEmbarcador || o.Carga.CodigosAgrupados.Contains(filtrosPesquisa.CodigoCargaEmbarcador));

            if (filtrosPesquisa.CodigoMotorista > 0)
                consultaCargaControleExpedicao = consultaCargaControleExpedicao.Where(o => o.Carga.Motoristas.Any(m => m.Codigo == filtrosPesquisa.CodigoMotorista));

            if (filtrosPesquisa.CodigoVeiculo > 0)
                consultaCargaControleExpedicao = consultaCargaControleExpedicao.Where(o => o.Carga.Veiculo.Codigo == filtrosPesquisa.CodigoVeiculo || o.Carga.VeiculosVinculados.Any(v => v.Codigo == filtrosPesquisa.CodigoVeiculo));

            if (filtrosPesquisa.CodigoTransportador > 0)
                consultaCargaControleExpedicao = consultaCargaControleExpedicao.Where(o => o.Carga.Empresa.Codigo == filtrosPesquisa.CodigoTransportador);

            if (filtrosPesquisa.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaControleExpedicao.Todas)
                consultaCargaControleExpedicao = consultaCargaControleExpedicao.Where(o => o.SituacaoCargaControleExpedicao == filtrosPesquisa.Situacao);

            return consultaCargaControleExpedicao;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.CargaControleExpedicao BuscarPorCarga(int codigoCarga)
        {
            var consultaCargaControleExpedicao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaControleExpedicao>()
                .Where(o =>
                    o.Carga.Codigo == codigoCarga &&
                    (o.FluxoGestaoPatio == null || o.FluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEtapaFluxoGestaoPatio.Cancelado)
                 );
            
            return consultaCargaControleExpedicao.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaControleExpedicao BuscarPorPreCarga(int codigoPrecarga)
        {
            var consultaCargaControleExpedicao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaControleExpedicao>()
                .Where(o =>
                    o.PreCarga.Codigo == codigoPrecarga &&
                    (o.FluxoGestaoPatio == null || o.FluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEtapaFluxoGestaoPatio.Cancelado)
                 );

            return consultaCargaControleExpedicao.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaControleExpedicao BuscarPorCodigo(int codigo)
        {
            var consultaCargaControleExpedicao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaControleExpedicao>()
                .Where(o => o.Codigo == codigo);

            return consultaCargaControleExpedicao.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaControleExpedicao BuscarPorFluxoGestaoPatio(int codigoFluxoGestaoPatio)
        {
            var consultaCargaControleExpedicao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaControleExpedicao>()
                .Where(o => o.FluxoGestaoPatio.Codigo == codigoFluxoGestaoPatio);

            return consultaCargaControleExpedicao.FirstOrDefault();
        }

        public List<Dominio.Entidades.Usuario> BuscarUsuariosNotificacao()
        {
            var consultaUsuario = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>()
                .Where(o => o.NotificadoExpedicao == true);

            return consultaUsuario.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaControleExpedicao> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaControleExpedicao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaCargaControleExpedicao = Consultar(filtrosPesquisa);

            return ObterLista(consultaCargaControleExpedicao, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaControleExpedicao filtrosPesquisa)
        {
            var consultaCargaControleExpedicao = Consultar(filtrosPesquisa);

            return consultaCargaControleExpedicao.Count();
        }

        #endregion
    }
}
