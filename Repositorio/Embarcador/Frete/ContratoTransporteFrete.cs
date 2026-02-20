using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frete
{
    public class ContratoTransporteFrete : RepositorioBase<Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete>
    {
        public ContratoTransporteFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete> BuscarPorCodigos(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete>();

            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete BuscarClonePorCodigo(int codigoContratoOriginal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete>()
                .Where(obj => obj.CodigoContratoClonado == codigoContratoOriginal);

            return query.FirstOrDefault();
        }

        public int BuscarProximoNumero()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete>();

            int? ultimo = query.Max(o => (int?)o.NumeroContratoSequencial);

            return ultimo.HasValue ? ultimo.Value + 1 : 1;
        }

        public Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete BuscarPorNumeroContrato(int numeroContrato)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete>();

            var result = from obj in query where obj.NumeroContrato == numeroContrato select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete> Consultar(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaContratoTransporteFrete filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var result = Consultar(filtrosPesquisa);

            return ObterLista(result, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaContratoTransporteFrete filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete> Consultar(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaContratoTransporteFrete filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoTransporteFrete>();
            query = from obj in query select obj;

            if (filtrosPesquisa.NumeroContrato > 0)
                query = query.Where(obj => obj.NumeroContrato == filtrosPesquisa.NumeroContrato);

            if (filtrosPesquisa.ContratoExternoId > 0)
                query = query.Where(obj => obj.ContratoExternoID == filtrosPesquisa.ContratoExternoId);

            if (filtrosPesquisa.Categoria.HasValue)
                query = query.Where(obj => obj.Categoria == filtrosPesquisa.Categoria.Value);

            if (filtrosPesquisa.SubCategoria.HasValue)
                query = query.Where(obj => obj.SubCategoria == filtrosPesquisa.SubCategoria.Value);

            if (filtrosPesquisa.CodigoTransportador > 0)
                query = query.Where(obj => obj.Transportador.Codigo.Equals(filtrosPesquisa.CodigoTransportador));

            if (filtrosPesquisa.PessoaJuridica.HasValue)
                query = query.Where(obj => obj.PessoaJuridica == filtrosPesquisa.PessoaJuridica.Value);

            if (filtrosPesquisa.DataInicio != DateTime.MinValue)
                query = query.Where(obj => obj.DataInicio >= filtrosPesquisa.DataInicio);

            if (filtrosPesquisa.DataFim != DateTime.MinValue)
                query = query.Where(obj => obj.DataFim <= filtrosPesquisa.DataFim);

            if (filtrosPesquisa.StatusAprovacaoTransportador.HasValue)
                query = query.Where(obj => obj.StatusAprovacaoTransportador == filtrosPesquisa.StatusAprovacaoTransportador.Value);

            if (filtrosPesquisa.StatusAssinaturaContrato > 0)
                query = query.Where(obj => obj.StatusAssinaturaContrato.Codigo.Equals(filtrosPesquisa.StatusAssinaturaContrato));

            if (filtrosPesquisa.Situacao)
                query = query.Where(obj => obj.Ativo == filtrosPesquisa.Situacao);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NomeContrato))
                query = query.Where(obj => obj.NomeContrato.Contains(filtrosPesquisa.NomeContrato));

            if (filtrosPesquisa.FiltrarPorTransportadorContrato)
            {
                var queryTabelaFrete = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFrete>();
                if (filtrosPesquisa.CodigoTabelaFrete > 0)
                    queryTabelaFrete = from obj in queryTabelaFrete where obj.Codigo == filtrosPesquisa.CodigoTabelaFrete select obj;

                query = query.Where(o => queryTabelaFrete.Where(a => a.ContratosTransporteFrete.Any(x => x.Codigo == o.Codigo)).Any());
            }

            if (filtrosPesquisa.SituacaoIntegracao.HasValue)
            {
                var queryTabelaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoTransporteFreteIntegracao>();

                query = query.Where(o => queryTabelaIntegracao.Where(i => i.ContratoTransporteFrete.Codigo == o.Codigo && i.SituacaoIntegracao == filtrosPesquisa.SituacaoIntegracao.Value).Any());
            }

            query = query.Where(obj => obj.Clonado);

            return query;
        }
    }
}
