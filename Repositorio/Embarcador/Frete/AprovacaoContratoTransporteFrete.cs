using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Frete
{
    public class AprovacaoContratoTransporteFrete : RepositorioBase<Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete>
    {
        public AprovacaoContratoTransporteFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete> Consultar(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaAprovacaoContratoTransporteFrete filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var result = Consultar(filtrosPesquisa);

            return ObterLista(result, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaAprovacaoContratoTransporteFrete filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete> Consultar(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaAprovacaoContratoTransporteFrete filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete>();

            if (filtrosPesquisa.NumeroContrato > 0)
                query = query.Where(obj => obj.NumeroContrato == filtrosPesquisa.NumeroContrato);

            if (filtrosPesquisa.ContratoExternoID > 0)
                query = query.Where(obj => obj.ContratoExternoID == filtrosPesquisa.ContratoExternoID);

            if (filtrosPesquisa.Categoria.HasValue)
                query = query.Where(obj => obj.Categoria == filtrosPesquisa.Categoria.Value);

            if (filtrosPesquisa.SubCategoria.HasValue)
                query = query.Where(obj => obj.SubCategoria == filtrosPesquisa.SubCategoria.Value);

            if (filtrosPesquisa.Transportador > 0)
                query = query.Where(obj => obj.Transportador.Codigo == filtrosPesquisa.Transportador);

            if (filtrosPesquisa.PessoaJuridica.HasValue)
                query = query.Where(obj => obj.PessoaJuridica == filtrosPesquisa.PessoaJuridica.Value);

            if (filtrosPesquisa.DatatInicio.HasValue && filtrosPesquisa.DatatInicio != DateTime.MinValue)
                query = query.Where(obj => obj.DataInicio >= filtrosPesquisa.DatatInicio.Value);

            if (filtrosPesquisa.DataFim.HasValue && filtrosPesquisa.DataFim != DateTime.MinValue)
                query = query.Where(obj => obj.DataFim <= filtrosPesquisa.DataFim.Value);

            if (filtrosPesquisa.StatusAprovacaoTransportador.HasValue)
                query = query.Where(obj => obj.StatusAprovacaoTransportador == filtrosPesquisa.StatusAprovacaoTransportador.Value);

            if (filtrosPesquisa.StatusAssinaturaContrato > 0)
                query = query.Where(obj => obj.StatusAssinaturaContrato.Codigo == filtrosPesquisa.StatusAssinaturaContrato);

            query = query.Where(obj => obj.Clonado && obj.Ativo);

            return query;
        }
    }
}
