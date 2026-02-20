using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System;

namespace Repositorio.Embarcador.Frete
{
    public class TabelaFreteClienteAlteracao : RepositorioBase<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteAlteracao>
    {
        #region Construtores

        public TabelaFreteClienteAlteracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<int> BuscarCodigosTabelasFreteClientePorAlteracao(int codigoTabelaFreteAlteracao)
        {
            var consultaTabelaFreteClienteAlteracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteAlteracao>()
                .Where(o => o.TabelaFreteAlteracao.Codigo == codigoTabelaFreteAlteracao);

            return consultaTabelaFreteClienteAlteracao.Select(o => o.TabelaFreteCliente.Codigo).ToList();
        }

        public List<int> BuscarCodigosTabelasFreteClientePorAlteracoes(List<int> codigosTabelaFreteAlteracao)
        {
            if (codigosTabelaFreteAlteracao.Count < 2000)
            {
                var consultaTabelaFreteClienteAlteracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteAlteracao>()
                    .Where(o => codigosTabelaFreteAlteracao.Contains(o.TabelaFreteAlteracao.Codigo));

                return consultaTabelaFreteClienteAlteracao.Select(o => o.TabelaFreteCliente.Codigo).ToList();
            }

            List<int> listaRetornar = new List<int>();

            List<int> listaOriginal = codigosTabelaFreteAlteracao;
            int tamanhoLote = 2000;
            int indiceInicial = 0;

            while (indiceInicial < listaOriginal.Count)
            {
                List<int> lote = listaOriginal.GetRange(indiceInicial, Math.Min(tamanhoLote, listaOriginal.Count - indiceInicial));

                var consultaMinima = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteAlteracao>()
                                    .Where(alteracao => lote.Contains(alteracao.Codigo));

                listaRetornar.AddRange(consultaMinima.Select(x => x.TabelaFreteAlteracao.Codigo).ToList());

                indiceInicial += tamanhoLote;
            }

            return listaRetornar;
        }

        public Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteAlteracao BuscarPorAlteracao(int codigoTabelaFreteAlteracao, int codigoTabelaFreteClienteAlteracao)
        {
            var consultaTabelaFreteClienteAlteracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteAlteracao>()
                .Where(o => o.TabelaFreteAlteracao.Codigo == codigoTabelaFreteAlteracao && o.TabelaFreteCliente.Codigo == codigoTabelaFreteClienteAlteracao);

            return consultaTabelaFreteClienteAlteracao.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteAlteracao> BuscarPorAlteracoes(List<int> codigosTabelaFreteAlteracao)
        {
            int take = 1000;
            int start = 0;
            List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteAlteracao> result = new List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteAlteracao>() { };
            while (start < codigosTabelaFreteAlteracao.Count)
            {
                List<int> tmp = codigosTabelaFreteAlteracao.Skip(start).Take(take).ToList();

                var consultaTabelaFreteClienteAlteracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteAlteracao>();

                var filter = from obj in consultaTabelaFreteClienteAlteracao where tmp.Contains(obj.TabelaFreteAlteracao.Codigo) select obj;

                result.AddRange(filter.Fetch(o => o.TabelaFreteCliente).ThenFetch(o => o.Vigencia).ToList());

                start += take;
            }

            return result;
        }

        public void DeletarPorTabelaFreteAlteracao(int codigoTabelaFreteAlteracao)
        {
            if (UnitOfWork.IsActiveTransaction())
            {
                UnitOfWork.Sessao
                    .CreateQuery($"delete TabelaFreteClienteAlteracao tabelaFreteClienteAlteracao where tabelaFreteClienteAlteracao.TabelaFreteAlteracao.Codigo = :codigo ")
                    .SetInt32("codigo", codigoTabelaFreteAlteracao)
                    .ExecuteUpdate();
            }
            else
            {
                try
                {
                    UnitOfWork.Start();

                    UnitOfWork.Sessao
                        .CreateQuery($"delete TabelaFreteClienteAlteracao tabelaFreteClienteAlteracao where tabelaFreteClienteAlteracao.TabelaFreteAlteracao.Codigo = :codigo ")
                        .SetInt32("codigo", codigoTabelaFreteAlteracao)
                        .ExecuteUpdate();

                    UnitOfWork.CommitChanges();
                }
                catch
                {
                    UnitOfWork.Rollback();
                    throw;
                }
            }
        }

        public void DeletarPorTabelaFreteCliente(int codigoTabelaFreteCliente)
        {
            if (UnitOfWork.IsActiveTransaction())
            {
                UnitOfWork.Sessao
                    .CreateQuery(
                        "delete TabelaFreteClienteAlteracao t " +
                        "where t.TabelaFreteCliente.Codigo = :codigo")
                    .SetInt32("codigo", codigoTabelaFreteCliente)
                    .ExecuteUpdate();
            }
            else
            {
                try
                {
                    UnitOfWork.Start();

                    UnitOfWork.Sessao
                        .CreateQuery(
                            "delete TabelaFreteClienteAlteracao t " +
                            "where t.TabelaFreteCliente.Codigo = :codigo")
                        .SetInt32("codigo", codigoTabelaFreteCliente)
                        .ExecuteUpdate();

                    UnitOfWork.CommitChanges();
                }
                catch
                {
                    UnitOfWork.Rollback();
                    throw;
                }
            }
        }

        public void InserirAlteracoes(int codigoTabelaFreteAlteracao, List<int> codigosTabelaFreteCliente)
        {
            if (codigosTabelaFreteCliente.Count <= 0)
                return;

            int limiteRegistros = 10000;

            for (int registroInicial = 0; registroInicial <= codigosTabelaFreteCliente.Count; registroInicial += limiteRegistros)
                UnitOfWork.Sessao
                   .CreateSQLQuery(
                       $@"insert into T_TABELA_FRETE_CLIENTE_ALTERACAO (TFA_CODIGO, TFC_CODIGO, TCA_DATA_ALTERACAO, FUN_CODIGO)
                       select {codigoTabelaFreteAlteracao}, TabelaFreteCliente.TFC_CODIGO, getdate(), null
                         from T_TABELA_FRETE_CLIENTE TabelaFreteCliente
                        where TabelaFreteCliente.TFC_CODIGO in ({string.Join(",", codigosTabelaFreteCliente.Skip(registroInicial).Take(limiteRegistros))})
                          and not exists (
                                  select 1
                                    from T_TABELA_FRETE_CLIENTE_ALTERACAO TabelaFreteClienteAlteracao
                                   where TabelaFreteClienteAlteracao.TFA_CODIGO = {codigoTabelaFreteAlteracao}
                                     and TabelaFreteClienteAlteracao.TFC_CODIGO = TabelaFreteCliente.TFC_CODIGO
                              );"
                   )
                   .SetTimeout(120)
                   .ExecuteUpdate();
        }

        #endregion
    }
}
