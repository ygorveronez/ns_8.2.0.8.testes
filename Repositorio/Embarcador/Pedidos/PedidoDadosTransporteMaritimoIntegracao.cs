using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pedidos
{
    public class PedidoDadosTransporteMaritimoIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimoIntegracao>
    {
        #region Construtores

        public PedidoDadosTransporteMaritimoIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        public IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimoIntegracao> Consultar(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedidoDadosTransporteMaritimoIntegracao filtrosPesquisa)
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimoIntegracao>();

            if (filtrosPesquisa.SituacaoIntegracao.HasValue)
                consultaIntegracao = consultaIntegracao.Where(o => o.SituacaoIntegracao == filtrosPesquisa.SituacaoIntegracao.Value);

            if (filtrosPesquisa.DataInicial.HasValue || filtrosPesquisa.DataLimite.HasValue)
            {
                if (filtrosPesquisa.DataInicial.HasValue)
                    consultaIntegracao = consultaIntegracao.Where(o => o.DataIntegracao >= filtrosPesquisa.DataInicial.Value.Date);

                if (filtrosPesquisa.DataLimite.HasValue)
                    consultaIntegracao = consultaIntegracao.Where(o => o.DataIntegracao <= filtrosPesquisa.DataLimite.Value.Date.Add(DateTime.MaxValue.TimeOfDay));
            }


            if (!string.IsNullOrEmpty(filtrosPesquisa.NumeroPedidoEmbarcador))
                consultaIntegracao = consultaIntegracao.Where(o => o.PedidoDadosTransporteMaritimo.Pedido.NumeroPedidoEmbarcador == filtrosPesquisa.NumeroPedidoEmbarcador);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroEXP))
                consultaIntegracao = consultaIntegracao.Where(o => o.PedidoDadosTransporteMaritimo.NumeroEXP == filtrosPesquisa.NumeroEXP);

            return consultaIntegracao;
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimoIntegracao> BuscarIntegracoesPendentes(int numeroTentativas, double tempoProximaTentativaEmMinutos, int limiteRegistros)
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimoIntegracao>()
                .Where(o =>
                    o.SituacaoIntegracao == SituacaoIntegracao.AgIntegracao ||
                    (o.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao && o.NumeroTentativas < numeroTentativas && o.DataIntegracao <= DateTime.Now.AddMinutes(-tempoProximaTentativaEmMinutos))
                );

            return consultaIntegracao
                .Fetch(o => o.PedidoDadosTransporteMaritimo)
                .OrderBy(o => o.Codigo)
                .Skip(0)
                .Take(limiteRegistros)
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimoIntegracao BuscarPorCodigoArquivo(int codigoArquivo)
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimoIntegracao>()
                .Where(o => o.ArquivosTransacao.Any(a => a.Codigo == codigoArquivo));

            return consultaIntegracao.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimoIntegracao BuscarPorCodigoBooking(int codigoPedidoDadosTransporteMaritimo)
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimoIntegracao>()
                .Where(o => o.PedidoDadosTransporteMaritimo.Codigo == codigoPedidoDadosTransporteMaritimo);

            return consultaIntegracao.FirstOrDefault();
        }

        public bool ExisteDadosTransporteMaritimoCodigoOriginalAguardandoRetorno(int codigoDadosTrasporteMaritimoOriginal)
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimoIntegracao>()
                .Any(o => o.PedidoDadosTransporteMaritimo.CodigoOriginal == codigoDadosTrasporteMaritimoOriginal && o.SituacaoIntegracao == SituacaoIntegracao.AgRetorno);

            return consultaIntegracao;
        }


        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimoIntegracao> Consultar(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedidoDadosTransporteMaritimoIntegracao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaIntegracao = Consultar(filtrosPesquisa);

            consultaIntegracao = consultaIntegracao.Fetch(o => o.PedidoDadosTransporteMaritimo).ThenFetch(o => o.Pedido);

            return ObterLista(consultaIntegracao, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedidoDadosTransporteMaritimoIntegracao filtrosPesquisa)
        {
            var consultaIntegracao = Consultar(filtrosPesquisa);

            return consultaIntegracao.Count();
        }

        #endregion Métodos Públicos
    }
}
