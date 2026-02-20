using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas.MontagemCarga
{
    public sealed class CarregamentoFilial : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoFilial>
    {
        #region Construtores

        public CarregamentoFilial(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public CarregamentoFilial(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Públicos

        public Task<List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoFilial>> BuscarPorCarregamentoAsync(int codigoCarregamento)
        {
            var consultaCarregamentoFilial = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoFilial>()
                .Where(o => o.Carregamento.Codigo == codigoCarregamento);

            return consultaCarregamentoFilial
                .Fetch(o => o.Empresa)
                .Fetch(o => o.Filial)
                .ToListAsync(CancellationToken);
        }

        public Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoFilial BuscarPorCarregamentoEFilial(int codigoCarregamento, int codigoFilial)
        {
            var consultaCarregamentoFilial = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoFilial>()
                .Where(o => o.Carregamento.Codigo == codigoCarregamento && o.Filial.Codigo == codigoFilial);

            return consultaCarregamentoFilial
                .Fetch(o => o.Empresa)
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoFilial> BuscarPorCarregamentos(List<int> codigosCarregamentos)
        {
            var consultaCarregamentoFilial = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoFilial>()
                .Where(o => codigosCarregamentos.Contains(o.Carregamento.Codigo));

            return consultaCarregamentoFilial
                .Fetch(o => o.Empresa)
                .Fetch(o => o.Filial)
                .ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoFilial>> BuscarPorCarregamentosAsync(List<int> codigosCarregamentos)
        {
            var consultaCarregamentoFilial = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoFilial>()
                .Where(o => codigosCarregamentos.Contains(o.Carregamento.Codigo));

            return consultaCarregamentoFilial
                .Fetch(o => o.Empresa)
                .Fetch(o => o.Filial)
                .ToListAsync(CancellationToken);
        }

        public void DeletarPorCarregamento(int codigoCarregamento)
        {
            try
            {
                UnitOfWork.Sessao
                    .CreateQuery("delete CarregamentoFilial carregamentoFilial where carregamentoFilial.Carregamento.Codigo = :codigoCarregamento ")
                    .SetInt32("codigoCarregamento", codigoCarregamento)
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

        #endregion
    }
}
