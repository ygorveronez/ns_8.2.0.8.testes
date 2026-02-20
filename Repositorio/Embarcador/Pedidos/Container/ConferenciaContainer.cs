using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pedidos
{
    public class ConferenciaContainer : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.ConferenciaContainer>
    {
        #region Construtores

        public ConferenciaContainer(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Privados

        public IQueryable<Dominio.Entidades.Embarcador.Pedidos.ConferenciaContainer> Consultar(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaConferenciaContainer filtrosPesquisa)
        {
            var consultaConferenciaContainer = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ConferenciaContainer>();

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoCargaEmbarcador))
                consultaConferenciaContainer = consultaConferenciaContainer.Where(o => o.Carga.CodigoCargaEmbarcador == filtrosPesquisa.CodigoCargaEmbarcador);

            if (filtrosPesquisa.DataInicial.HasValue)
                consultaConferenciaContainer = consultaConferenciaContainer.Where(o => o.DataCriacao >= filtrosPesquisa.DataInicial.Value.Date);

            if (filtrosPesquisa.DataFinal.HasValue)
                consultaConferenciaContainer = consultaConferenciaContainer.Where(o => o.DataCriacao <= filtrosPesquisa.DataFinal.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.Situacao.HasValue)
                consultaConferenciaContainer = consultaConferenciaContainer.Where(o => o.Situacao == filtrosPesquisa.Situacao.Value);

            return consultaConferenciaContainer;
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Pedidos.ConferenciaContainer BuscarPorCarga(int codigoCarga)
        {
            var consultaConferenciaContainer = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ConferenciaContainer>()
                .Where(o => o.Carga.Codigo == codigoCarga);

            return consultaConferenciaContainer.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.ConferenciaContainer> Consultar(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaConferenciaContainer filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaConferenciaContainer = Consultar(filtrosPesquisa);

            return ObterLista(consultaConferenciaContainer, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaConferenciaContainer filtrosPesquisa)
        {
            var consultaConferenciaContainer = Consultar(filtrosPesquisa);

            return consultaConferenciaContainer.Count();
        }

        public void DeletarPorCarga(int codigoCarga)
        {
            try
            {
                UnitOfWork.Sessao
                    .CreateQuery($"delete ConferenciaContainer conferencia where conferencia.Carga.Codigo = :codigoCarga")
                    .SetInt32("codigoCarga", codigoCarga)
                    .ExecuteUpdate();
            }
            catch (NHibernate.Exceptions.GenericADOException excecao)
            {
                if (excecao.InnerException != null && object.ReferenceEquals(excecao.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
                {
                    System.Data.SqlClient.SqlException excecaoSql = (System.Data.SqlClient.SqlException)excecao.InnerException;

                    if (excecaoSql.Number == 547)
                        throw new Exception("O registro possui dependências e não pode ser excluido.", excecao);
                }

                throw;
            }
        }

        public bool ExistePorCarga(int codigoCarga)
        {
            var consultaConferenciaContainer = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ConferenciaContainer>()
                .Where(o => o.Carga.Codigo == codigoCarga);

            return consultaConferenciaContainer.Count() > 0;
        }

        #endregion Métodos Públicos
    }
}
