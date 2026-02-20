using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Escalas
{
    public class GeracaoEscala : RepositorioBase<Dominio.Entidades.Embarcador.Escalas.GeracaoEscala>
    {
        #region Construtores

        public GeracaoEscala(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        public IQueryable<Dominio.Entidades.Embarcador.Escalas.GeracaoEscala> Consultar(Dominio.ObjetosDeValor.Embarcador.Escalas.FiltroPesquisaGeracaoEscala filtrosPesquisa)
        {
            var consultaGeracaoEscala = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escalas.GeracaoEscala>();

            if (filtrosPesquisa.NumeroEscala > 0)
                consultaGeracaoEscala = consultaGeracaoEscala.Where(obj => obj.NumeroEscala == filtrosPesquisa.NumeroEscala);
            else
            {
                if (filtrosPesquisa.SituacaoEscala.HasValue)
                    consultaGeracaoEscala = consultaGeracaoEscala.Where(obj => obj.SituacaoEscala == filtrosPesquisa.SituacaoEscala.Value);

                if (filtrosPesquisa.CodigoProduto > 0)
                    consultaGeracaoEscala = consultaGeracaoEscala.Where(obj => obj.Produtos.Any(pro => pro.Codigo == filtrosPesquisa.CodigoProduto));

                if (filtrosPesquisa.CodigoCentroCarregamento > 0)
                    consultaGeracaoEscala = consultaGeracaoEscala.Where(obj => obj.CentrosCarregamento.Any(cen => cen.Codigo == filtrosPesquisa.CodigoCentroCarregamento));

                if (filtrosPesquisa.DataInicial.HasValue)
                    consultaGeracaoEscala = consultaGeracaoEscala.Where(o => o.DataEscala >= filtrosPesquisa.DataInicial.Value.Date);

                if (filtrosPesquisa.DataLimite.HasValue)
                    consultaGeracaoEscala = consultaGeracaoEscala.Where(o => o.DataEscala < filtrosPesquisa.DataLimite.Value.AddDays(1).Date);
            }

            return consultaGeracaoEscala;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Escalas.GeracaoEscala BuscarPorCodigo(int codigo)
        {
            var consultaGeracaoEscala = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escalas.GeracaoEscala>()
                .Where(o => o.Codigo == codigo);

            return consultaGeracaoEscala.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Escalas.GeracaoEscala BuscarPorData(DateTime data)
        {
            var consultaGeracaoEscala = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escalas.GeracaoEscala>()
                .Where(o => o.DataEscala.Date == data.Date && o.SituacaoEscala != SituacaoEscala.Cancelada);

            return consultaGeracaoEscala.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Escalas.GeracaoEscala> Consultar(Dominio.ObjetosDeValor.Embarcador.Escalas.FiltroPesquisaGeracaoEscala filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaGeracaoEscala = Consultar(filtrosPesquisa);

            return ObterLista(consultaGeracaoEscala, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Escalas.FiltroPesquisaGeracaoEscala filtrosPesquisa)
        {
            var consultaGeracaoEscala = Consultar(filtrosPesquisa);

            return consultaGeracaoEscala.Count();
        }

        public int ObterProximoNumero()
        {
            var consultaGeracaoEscala = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escalas.GeracaoEscala>();

            int? retorno = consultaGeracaoEscala.Max(o => (int?)o.NumeroEscala);

            return retorno.HasValue ? retorno.Value + 1 : 1;
        }

        #endregion
    }
}
