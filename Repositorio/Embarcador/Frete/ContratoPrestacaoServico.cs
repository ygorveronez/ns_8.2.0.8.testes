using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System;

namespace Repositorio.Embarcador.Frete
{
    public sealed class ContratoPrestacaoServico : RepositorioBase<Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServico>
    {
        #region Construtores

        public ContratoPrestacaoServico(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServico> Consultar(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaContratoPrestacaoServico filtrosPesquisa)
        {
            var consultaContratoPrestacaoServico = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServico>();

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                consultaContratoPrestacaoServico = consultaContratoPrestacaoServico.Where(o => o.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.Situacao.HasValue)
                consultaContratoPrestacaoServico = consultaContratoPrestacaoServico.Where(o => o.Situacao == filtrosPesquisa.Situacao.Value);

            if (filtrosPesquisa.SituacaoAtivo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                consultaContratoPrestacaoServico = consultaContratoPrestacaoServico.Where(o => o.Ativo);
            else if (filtrosPesquisa.SituacaoAtivo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                consultaContratoPrestacaoServico = consultaContratoPrestacaoServico.Where(o => !o.Ativo);

            return consultaContratoPrestacaoServico;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServico> ConsultarAtivo(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaContratoPrestacaoServico filtrosPesquisa)
        {
            DateTime dataBase = DateTime.Now.Date;

            var consultaContratoPrestacaoServico = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServico>()
                .Where(o => 
                    o.Ativo &&
                    (o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoPrestacaoServico.Aprovado) &&
                    (o.DataInicial <= dataBase) &&
                    (o.DataFinal >= dataBase.Add(DateTime.MaxValue.TimeOfDay))
                );

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                consultaContratoPrestacaoServico = consultaContratoPrestacaoServico.Where(o => o.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.Situacao.HasValue)
                consultaContratoPrestacaoServico = consultaContratoPrestacaoServico.Where(o => o.Situacao == filtrosPesquisa.Situacao.Value);

            return consultaContratoPrestacaoServico;
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServico> BuscarAtivoPorTransportador(int codigoTransportador)
        {
            DateTime dataBase = DateTime.Now.Date;

            var consultaContratoPrestacaoServico = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServico>()
                .Where(o =>
                    o.Ativo &&
                    (o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoPrestacaoServico.Aprovado) &&
                    (o.DataInicial <= dataBase) &&
                    (o.DataFinal >= dataBase.Add(DateTime.MaxValue.TimeOfDay)) &&
                    o.Transportadores.Any(t => t.Codigo == codigoTransportador)
                );

            return consultaContratoPrestacaoServico.ToList();
        }

        public Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServico BuscarAtivoPorTransportadorEFilial(int codigoTransportador, int codigoFilial)
        {
            DateTime dataBase = DateTime.Now.Date;

            var consultaContratoPrestacaoServico = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServico>()
                .Where(o => 
                    o.Ativo &&
                    (o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoPrestacaoServico.Aprovado) &&
                    (o.DataInicial <= dataBase) &&
                    (o.DataFinal >= dataBase.Add(DateTime.MaxValue.TimeOfDay)) &&
                    o.Transportadores.Any(t => t.Codigo == codigoTransportador) &&
                    ((o.Filiais.Count == 0) || o.Filiais.Any(f => f.Codigo == codigoFilial))
                );

            return consultaContratoPrestacaoServico.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServico BuscarPorCodigo(int codigo)
        {
            var consultaContratoPrestacaoServico = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServico>()
                .Where(o => o.Codigo == codigo);

            return consultaContratoPrestacaoServico.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServico> Consultar(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaContratoPrestacaoServico filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaCarga = Consultar(filtrosPesquisa);

            return ObterLista(consultaCarga, parametrosConsulta);
        }

        public List<Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServico> ConsultarAtivo(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaContratoPrestacaoServico filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaCarga = ConsultarAtivo(filtrosPesquisa);

            return ObterLista(consultaCarga, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaContratoPrestacaoServico filtrosPesquisa)
        {
            var consultaCarga = Consultar(filtrosPesquisa);

            return consultaCarga.Count();
        }

        public int ContarConsultaAtivo(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaContratoPrestacaoServico filtrosPesquisa)
        {
            var consultaCarga = ConsultarAtivo(filtrosPesquisa);

            return consultaCarga.Count();
        }

        #endregion
    }
}
