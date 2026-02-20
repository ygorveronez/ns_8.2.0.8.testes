using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System;

namespace Repositorio.Embarcador.Cargas
{
    public sealed class PreAgrupamentoCarga : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga>
    {
        #region Construtores

        public PreAgrupamentoCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaPreAgrupamentoCarga filtrosPesquisa)
        {
            var consultaPreAgrupamentoCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga>();

            if (filtrosPesquisa.CodigoAgrupamento > 0)
                consultaPreAgrupamentoCarga = consultaPreAgrupamentoCarga.Where(o => o.Agrupador.CodigoAgrupamento == filtrosPesquisa.CodigoAgrupamento);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoViagem))
                consultaPreAgrupamentoCarga = consultaPreAgrupamentoCarga.Where(o => o.CodigoViagem.Contains(filtrosPesquisa.CodigoViagem));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroNota))
                consultaPreAgrupamentoCarga = consultaPreAgrupamentoCarga.Where(o => o.NumeroNota.Contains(filtrosPesquisa.NumeroNota));

            if (filtrosPesquisa.Situacao.HasValue)
                consultaPreAgrupamentoCarga = consultaPreAgrupamentoCarga.Where(o => o.Agrupador.Situacao == filtrosPesquisa.Situacao.Value);


            if (filtrosPesquisa.DataInicial.HasValue)
                consultaPreAgrupamentoCarga = consultaPreAgrupamentoCarga.Where(o => o.Agrupador.DataCriacao >= filtrosPesquisa.DataInicial.Value);

            if (filtrosPesquisa.DataFinal.HasValue)
                consultaPreAgrupamentoCarga = consultaPreAgrupamentoCarga.Where(o => o.Agrupador.DataCriacao <= filtrosPesquisa.DataFinal.Value.Add(System.DateTime.MaxValue.TimeOfDay));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Emitente))
                consultaPreAgrupamentoCarga = consultaPreAgrupamentoCarga.Where(o => o.CnpjEmitente.Equals(filtrosPesquisa.Emitente));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Recebedor))
                consultaPreAgrupamentoCarga = consultaPreAgrupamentoCarga.Where(o => o.CnpjRecebedor.Equals(filtrosPesquisa.Recebedor));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Expedidor))
                consultaPreAgrupamentoCarga = consultaPreAgrupamentoCarga.Where(o => o.CnpjExpedidor.Equals(filtrosPesquisa.Expedidor));

            if (filtrosPesquisa.DataPrevisaoEntregaInicial.HasValue)
                consultaPreAgrupamentoCarga = consultaPreAgrupamentoCarga.Where(o => o.DataPrevisaoEntrega >= filtrosPesquisa.DataPrevisaoEntregaInicial.Value);

            if (filtrosPesquisa.DataPrevisaoEntregaFinal.HasValue)
                consultaPreAgrupamentoCarga = consultaPreAgrupamentoCarga.Where(o => o.DataPrevisaoEntrega <= filtrosPesquisa.DataPrevisaoEntregaFinal.Value);

            return consultaPreAgrupamentoCarga;
        }

        #endregion

        #region Métodos Públicos



        public bool VerificarSeExisteCargaDePreCargaPorAgrupamento(int agrupamento)
        {
            var consultaAgrupador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga>()
                .Where(o => o.Agrupador.Codigo == agrupamento);

            return consultaAgrupador.Any(obj => obj.Carga.CargaDePreCarga);
        }
        public List<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga> BuscarCargasPorCodigoAgrupador(int codigo)
        {
            var consultaAgrupador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga>()
                .Where(o => o.Agrupador.Codigo == codigo && o.CargaPedidoEncaixe == null);

            return consultaAgrupador
                .Fetch(obj => obj.Carga).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga> BuscaPorCodigoAgrupador(int codigo)
        {
            var consultaAgrupador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga>()
                .Where(o => o.Agrupador.Codigo == codigo);

            return consultaAgrupador
                .Fetch(obj => obj.Carga)
                .ThenFetch(obj => obj.TipoOperacao)
                .Fetch(obj => obj.Carga)
                .ThenFetch(obj => obj.Carregamento)
                .Fetch(obj => obj.Carga)
                .ThenFetch(obj => obj.Filial)
                .Fetch(obj => obj.Carga)
                .ThenFetch(obj => obj.ModeloVeicularCarga)
                .Fetch(obj => obj.Carga)
                .ThenFetch(obj => obj.TipoDeCarga)
                .Fetch(obj => obj.Carga)
                .ThenFetch(obj => obj.TabelaFrete)
                .Fetch(obj => obj.Carga)
                .ThenFetch(obj => obj.EmpresaFilialEmissora)
                .Fetch(obj => obj.Carga)
                .ThenFetch(obj => obj.DadosSumarizados)
                .Fetch(obj => obj.Carga)
                .ThenFetch(obj => obj.Veiculo)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga> BuscarCargasPedidoEncaixePorCodigoAgrupador(int codigo)
        {
            var consultaAgrupador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga>()
                .Where(o => o.Agrupador.Codigo == codigo && o.PedidoEncaixe);

            return consultaAgrupador
                .Fetch(obj => obj.CargaPedidoEncaixe)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga> BuscarListaPorCarga(int codigoCarga)
        {
            var consultaPreAgrupamentoCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga>()
                .Where(o => o.Carga.Codigo == codigoCarga || o.CargaRedespacho.Codigo == codigoCarga);

            return consultaPreAgrupamentoCarga.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga VerificarSeExistePorCargaPedido(int codigoCargaPedido, List<int> codigosAtual, string expedidor)
        {
            var consultaPreAgrupamentoCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga>()
                .Where(o => o.CargaPedidoEncaixe.Codigo == codigoCargaPedido && !codigosAtual.Contains(o.Codigo) && o.CnpjExpedidor == expedidor && o.Agrupador.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPreAgrupamentoCarga.ProblemaCarregamento);

            return consultaPreAgrupamentoCarga.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga BuscarPorCarga(int codigoCarga)
        {
            var consultaPreAgrupamentoCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga>()
                .Where(o => o.Carga.Codigo == codigoCarga);

            return consultaPreAgrupamentoCarga.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga> BuscarSemCargaPorNumeroCarga(string codigoCargaEmbarcador)
        {
            var consultaPreAgrupamentoCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga>()
                .Where(o =>
                    (o.CodigoViagem == codigoCargaEmbarcador) &&
                    (o.Agrupador.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPreAgrupamentoCarga.SemCarga)
                );

            return consultaPreAgrupamentoCarga.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaPreAgrupamentoCarga filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaPreAgrupamentoCarga = Consultar(filtrosPesquisa);

            consultaPreAgrupamentoCarga =
                consultaPreAgrupamentoCarga
                .Fetch(o => o.Carga)
                .Fetch(o => o.Agrupador)
                .ThenFetch(obj => obj.Veiculo)
                .Fetch(o => o.Agrupador)
                .ThenFetch(obj => obj.Empresa)
                .ThenFetch(obj => obj.Localidade);

            return ObterLista(consultaPreAgrupamentoCarga, parametrosConsulta);
        }

        public int ContarConsultaPreAgrupamentoCarga(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaPreAgrupamentoCarga filtrosPesquisa)
        {
            var consultaPreAgrupamentoCarga = Consultar(filtrosPesquisa);

            return consultaPreAgrupamentoCarga.Count();
        }

        public void DeletarPorAgrupamento(int codigoAgrupamento)
        {
            try
            {

                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE FROM PreAgrupamentoCarga preAgrupamentoCarga WHERE preAgrupamentoCarga.Agrupador.Codigo = :CodigoAgrupamento").SetInt32("CodigoAgrupamento", codigoAgrupamento).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE FROM PreAgrupamentoCarga preAgrupamentoCarga WHERE preAgrupamentoCarga.Agrupador.Codigo = :CodigoAgrupamento").SetInt32("CodigoAgrupamento", codigoAgrupamento).ExecuteUpdate();

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
    }
}
