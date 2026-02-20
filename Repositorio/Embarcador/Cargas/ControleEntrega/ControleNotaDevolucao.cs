using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas.ControleEntrega
{
    public class ControleNotaDevolucao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ControleNotaDevolucao>
    {
        public ControleNotaDevolucao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ControleNotaDevolucao BuscarPorCodigo(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ControleNotaDevolucao>();
            var result = query.Where(obj => obj.Codigo == codigo);
            return result
                .Fetch(obj => obj.XMLNotaFiscal)
                .ThenFetch(obj => obj.Canhoto)
                .Fetch(obj => obj.XMLNotaFiscal)
                .ThenFetch(obj => obj.Destinatario)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CargaEntregaNFeDevolucao)
                .ThenFetch(obj => obj.XMLNotaFiscal)
                .ThenFetch(obj => obj.Canhoto)
                .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ControleNotaDevolucao BuscarPorChave(string chave)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ControleNotaDevolucao>();
            var result = query.Where(obj => obj.ChaveNFe == chave);
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ControleNotaDevolucao> BuscarPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ControleNotaDevolucao> consultaControleNotaDevolucao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ControleNotaDevolucao>();
            consultaControleNotaDevolucao = consultaControleNotaDevolucao.Where(controleDevolucao => controleDevolucao.CargaEntregaNFeDevolucao.CargaEntrega.Carga.Codigo == codigoCarga);

            return consultaControleNotaDevolucao.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ControleNotaDevolucao> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaControleNotaDevolucao filtrosPesquisa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = Consultar(filtrosPesquisa);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros)
                .Fetch(obj => obj.XMLNotaFiscal)
                .ThenFetch(obj => obj.Canhoto)
                .Fetch(obj => obj.XMLNotaFiscal)
                .ThenFetch(obj => obj.Destinatario)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CargaEntregaNFeDevolucao)
                .ThenFetch(obj => obj.XMLNotaFiscal)
                .ThenFetch(obj => obj.Canhoto)
                .ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaControleNotaDevolucao filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.Count();
        }


        public void DeletarPorCargaEntregaNFeDevolucao(int codigoCargaEntregaNFeDevolucao)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE FROM ControleNotaDevolucao obj WHERE obj.CargaEntregaNFeDevolucao.Codigo = :codigoCargaEntregaNFeDevolucao").SetInt32("codigoCargaEntregaNFeDevolucao", codigoCargaEntregaNFeDevolucao).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE FROM ControleNotaDevolucao obj WHERE obj.CargaEntregaNFeDevolucao.Codigo = :codigoCargaEntregaNFeDevolucao").SetInt32("codigoCargaEntregaNFeDevolucao", codigoCargaEntregaNFeDevolucao).ExecuteUpdate();

                        UnitOfWork.CommitChanges();
                    }
                    catch
                    {
                        UnitOfWork.Rollback();
                        throw;
                    }
                }
            }
            catch (NHibernate.Exceptions.GenericADOException ex)
            {
                if (ex.InnerException != null && object.ReferenceEquals(ex.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
                {
                    System.Data.SqlClient.SqlException excecao = (System.Data.SqlClient.SqlException)ex.InnerException;
                    if (excecao.Number == 547)
                    {
                        throw new Exception("O registro possui dependências e não pode ser excluido.", ex);
                    }
                }
                throw;
            }
        }


        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ControleNotaDevolucao> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaControleNotaDevolucao filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ControleNotaDevolucao>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Chave))
                result = result.Where(obj => obj.ChaveNFe.Contains(filtrosPesquisa.Chave));

            if (filtrosPesquisa.Status != Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusControleNotaDevolucao.Todos)
                result = result.Where(o => o.Status == filtrosPesquisa.Status);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarga))
                result = result.Where(o => o.Chamado.Carga.CodigoCargaEmbarcador == filtrosPesquisa.NumeroCarga);

            if (filtrosPesquisa.NumeroChamado > 0)
                result = result.Where(o => o.Chamado.Numero == filtrosPesquisa.NumeroChamado);

            if (filtrosPesquisa.DataEmissaoInicial != DateTime.MinValue)
                result = result.Where(o => o.XMLNotaFiscal.DataEmissao.Date >= filtrosPesquisa.DataEmissaoInicial);

            if (filtrosPesquisa.DataEmissaoFinal != DateTime.MinValue)
                result = result.Where(o => o.XMLNotaFiscal.DataEmissao.Date <= filtrosPesquisa.DataEmissaoFinal);

            if (filtrosPesquisa.CnpjCpfDestinatario > 0)
                result = result.Where(o => o.XMLNotaFiscal.Destinatario.CPF_CNPJ == filtrosPesquisa.CnpjCpfDestinatario);

            return result;
        }

        #endregion
    }
}
