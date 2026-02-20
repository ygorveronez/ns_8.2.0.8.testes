using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System;

namespace Repositorio.Embarcador.Logistica
{
    public sealed class ControleCarregamento : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.ControleCarregamento>
    {
        #region Construtores

        public ControleCarregamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Logistica.ControleCarregamento> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaControleCarregamento filtrosPesquisa)
        {
            var consultaControleCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ControleCarregamento>();

            if (filtrosPesquisa.CodigoCentroCarregamento > 0)
                consultaControleCarregamento = consultaControleCarregamento.Where(o => o.JanelaCarregamento.CentroCarregamento.Codigo == filtrosPesquisa.CodigoCentroCarregamento);

            if (filtrosPesquisa.Situacoes?.Count > 0)
                consultaControleCarregamento = consultaControleCarregamento.Where(o => filtrosPesquisa.Situacoes.Contains(o.Situacao));

            if (filtrosPesquisa.DataInicial.HasValue)
                consultaControleCarregamento = consultaControleCarregamento.Where(o => o.DataCriacao >= filtrosPesquisa.DataInicial.Value.Date);

            if (filtrosPesquisa.DataLimite.HasValue)
                consultaControleCarregamento = consultaControleCarregamento.Where(o => o.DataCriacao <= filtrosPesquisa.DataLimite.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            return consultaControleCarregamento;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Logistica.ControleCarregamento BuscarPorCodigo(int codigo)
        {
            var consultaControleCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ControleCarregamento>()
                .Where(o => o.Codigo == codigo);

            return consultaControleCarregamento.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.ControleCarregamento BuscarPorJanelaCarregamento(int codigoJanelaCarregamento)
        {
            var consultaControleCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ControleCarregamento>()
                .Where(o => o.JanelaCarregamento.Codigo == codigoJanelaCarregamento);

            return consultaControleCarregamento.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.ControleCarregamento BuscarAguardandoPorVeiculo(int codigoVeiculo)
        {
            Dominio.Entidades.Veiculo veiculo = new Dominio.Entidades.Veiculo() { Codigo = codigoVeiculo };

            var consultaControleCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ControleCarregamento>()
                .Where(o =>
                    (o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoControleCarregamento.Aguardando) &&
                    (
                        (o.JanelaCarregamento.Carga != null && (o.JanelaCarregamento.Carga.Veiculo.Codigo == codigoVeiculo || o.JanelaCarregamento.Carga.VeiculosVinculados.Contains(veiculo))) ||
                        (o.JanelaCarregamento.Carga == null && (o.JanelaCarregamento.PreCarga.Veiculo.Codigo == codigoVeiculo || o.JanelaCarregamento.PreCarga.VeiculosVinculados.Contains(veiculo)))
                    )
                )
                .OrderByDescending(o => o.DataCriacao);

            return consultaControleCarregamento.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.ControleCarregamento> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaControleCarregamento filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaControleCarregamento = Consultar(filtrosPesquisa);

            return ObterLista(consultaControleCarregamento, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaControleCarregamento filtrosPesquisa)
        {
            var consultaControleCarregamento = Consultar(filtrosPesquisa);

            return consultaControleCarregamento.Count();
        }

        #endregion
    }
}
