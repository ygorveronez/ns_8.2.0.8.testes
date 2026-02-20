using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.Chamados
{
    public class ChamadoTMS : RepositorioBase<Dominio.Entidades.Embarcador.Chamados.ChamadoTMS>
    {
        public ChamadoTMS(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public int BuscarProximoNumero()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoTMS>();
            var result = from obj in query select obj.Numero;
            int prox = result.Count() > 0 ? result.Max() : 0;

            return prox + 1;
        }

        public List<Dominio.Entidades.Embarcador.Chamados.ChamadoTMS> Consultar(Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaChamadoTMS filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaChamado = Consultar(filtrosPesquisa);

            consultaChamado = consultaChamado
                .Fetch(o => o.Carga)
                .Fetch(o => o.Carga).ThenFetch(o => o.Veiculo)
                .Fetch(o => o.Motorista)
                .Fetch(o => o.MotivoChamado);

            return ObterLista(consultaChamado, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaChamadoTMS filtrosPesquisa)
        {
            var consultaChamado = Consultar(filtrosPesquisa);

            return consultaChamado.Count();
        }

        public List<Dominio.Entidades.Embarcador.Chamados.ChamadoTMS> ConsultarChamadosParaControle(Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaControleChamadoTMS filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaChamado = ConsultarChamadosParaControle(filtrosPesquisa);

            consultaChamado = consultaChamado
                .Fetch(o => o.Carga)
                .Fetch(o => o.Carga).ThenFetch(o => o.Veiculo)
                .Fetch(o => o.Motorista)
                .Fetch(o => o.MotivoChamado);

            return ObterLista(consultaChamado, parametrosConsulta);
        }

        public int ContarConsultaChamadosParaControle(Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaControleChamadoTMS filtrosPesquisa)
        {
            var consultaChamado = ConsultarChamadosParaControle(filtrosPesquisa);

            return consultaChamado.Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Chamados.ChamadoTMS> Consultar(Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaChamadoTMS filtrosPesquisa)
        {
            var consultaChamado = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoTMS>();

            if (filtrosPesquisa.NumeroInicial > 0)
                consultaChamado = consultaChamado.Where(o => o.Numero >= filtrosPesquisa.NumeroInicial);

            if (filtrosPesquisa.NumeroFinal > 0)
                consultaChamado = consultaChamado.Where(o => o.Numero <= filtrosPesquisa.NumeroFinal);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoCargaEmbarcador))
                consultaChamado = consultaChamado.Where(o =>
                    o.Carga.CodigoCargaEmbarcador.Equals(filtrosPesquisa.CodigoCargaEmbarcador) ||
                    o.Carga.CodigosAgrupados.Contains(filtrosPesquisa.CodigoCargaEmbarcador)
                );

            if (filtrosPesquisa.CodigoMotorista > 0)
                consultaChamado = consultaChamado.Where(o => o.Motorista.Codigo == filtrosPesquisa.CodigoMotorista);

            if (filtrosPesquisa.CodigoMotivoChamado > 0)
                consultaChamado = consultaChamado.Where(o => o.MotivoChamado.Codigo == filtrosPesquisa.CodigoMotivoChamado);

            if (filtrosPesquisa.SituacaoChamado != SituacaoChamadoTMS.Todos)
                consultaChamado = consultaChamado.Where(o => o.Situacao == filtrosPesquisa.SituacaoChamado);

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                consultaChamado = consultaChamado.Where(o => o.DataCriacao.Date >= filtrosPesquisa.DataInicial.Date);

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                consultaChamado = consultaChamado.Where(o => o.DataCriacao.Date <= filtrosPesquisa.DataFinal.Date);

            if (filtrosPesquisa.CodigoVeiculo > 0)
                consultaChamado = consultaChamado.Where(o =>
                    o.Carga.Veiculo.Codigo == filtrosPesquisa.CodigoVeiculo ||
                    o.Carga.VeiculosVinculados.Any(vei => vei.Codigo == filtrosPesquisa.CodigoVeiculo)
                );

            return consultaChamado;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Chamados.ChamadoTMS> ConsultarChamadosParaControle(Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaControleChamadoTMS filtrosPesquisa)
        {
            var consultaChamado = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoTMS>();
            consultaChamado = consultaChamado.Where(o => o.Situacao == SituacaoChamadoTMS.EmAnalise);

            if (filtrosPesquisa.NumeroInicial > 0)
                consultaChamado = consultaChamado.Where(o => o.Numero >= filtrosPesquisa.NumeroInicial);

            if (filtrosPesquisa.NumeroFinal > 0)
                consultaChamado = consultaChamado.Where(o => o.Numero <= filtrosPesquisa.NumeroFinal);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoCargaEmbarcador))
                consultaChamado = consultaChamado.Where(o =>
                    o.Carga.CodigoCargaEmbarcador.Equals(filtrosPesquisa.CodigoCargaEmbarcador) ||
                    o.Carga.CodigosAgrupados.Contains(filtrosPesquisa.CodigoCargaEmbarcador)
                );

            if (filtrosPesquisa.CodigoMotorista > 0)
                consultaChamado = consultaChamado.Where(o => o.Motorista.Codigo == filtrosPesquisa.CodigoMotorista);

            if (filtrosPesquisa.CodigoMotivoChamado > 0)
                consultaChamado = consultaChamado.Where(o => o.MotivoChamado.Codigo == filtrosPesquisa.CodigoMotivoChamado);

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                consultaChamado = consultaChamado.Where(o => o.DataCriacao.Date >= filtrosPesquisa.DataInicial.Date);

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                consultaChamado = consultaChamado.Where(o => o.DataCriacao.Date <= filtrosPesquisa.DataFinal.Date);

            if (filtrosPesquisa.CodigoVeiculo > 0)
                consultaChamado = consultaChamado.Where(o =>
                    o.Carga.Veiculo.Codigo == filtrosPesquisa.CodigoVeiculo ||
                    o.Carga.VeiculosVinculados.Any(vei => vei.Codigo == filtrosPesquisa.CodigoVeiculo)
                );

            if (filtrosPesquisa.NumeroCTe > 0)
                consultaChamado = consultaChamado.Where(o =>
                    o.CTes.Any(c => c.CTe.Numero == filtrosPesquisa.NumeroCTe)
                );

            return consultaChamado;
        }

        #endregion
    }
}
