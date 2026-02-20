using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Veiculos
{
    public class VeiculoReceitaDespesa : RepositorioBase<Dominio.Entidades.Embarcador.Veiculos.VeiculoReceitaDespesa>
    {
        public VeiculoReceitaDespesa(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public VeiculoReceitaDespesa(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public List<Dominio.Entidades.Embarcador.Veiculos.VeiculoReceitaDespesa> BuscarPorCTe(int codigoCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Veiculos.VeiculoReceitaDespesa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoReceitaDespesa>();

            query = query.Where(o => o.CTe.Codigo == codigoCTe);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Veiculos.VeiculoReceitaDespesa> BuscarPorCTeEVeiculo(int codigoCTe, int codigoVeiculo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Veiculos.VeiculoReceitaDespesa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoReceitaDespesa>();

            query = query.Where(o => o.CTe.Codigo == codigoCTe && o.Veiculo.Codigo == codigoVeiculo);

            return query.ToList();
        }

        public void DeletarPorCTe(int codigoCTe)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE FROM VeiculoReceitaDespesa obj WHERE obj.CTe.Codigo = :codigoCTe").SetInt32("codigoCTe", codigoCTe).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE FROM VeiculoReceitaDespesa obj WHERE obj.CTe.Codigo = :codigoCTe").SetInt32("codigoCTe", codigoCTe).ExecuteUpdate();

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

        public void DeletarPorDocumentoEntrada(int codigoDocumentoEntrada)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE FROM VeiculoReceitaDespesa obj WHERE obj.ItemDocumentoEntrada in (select doc from DocumentoEntradaItem doc where doc.DocumentoEntrada.Codigo = :codigoDocumentoEntrada)").SetInt32("codigoDocumentoEntrada", codigoDocumentoEntrada).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE FROM VeiculoReceitaDespesa obj WHERE obj.ItemDocumentoEntrada in (select doc from DocumentoEntradaItem doc where doc.DocumentoEntrada.Codigo = :codigoDocumentoEntrada)").SetInt32("codigoDocumentoEntrada", codigoDocumentoEntrada).ExecuteUpdate();

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

        public void DeletarPorAbastecimento(int codigoAbastecimento)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE FROM VeiculoReceitaDespesa obj WHERE obj.Abastecimento.Codigo = :codigoAbastecimento").SetInt32("codigoAbastecimento", codigoAbastecimento).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE FROM VeiculoReceitaDespesa obj WHERE obj.Abastecimento.Codigo = :codigoAbastecimento").SetInt32("codigoAbastecimento", codigoAbastecimento).ExecuteUpdate();

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

        public void DeletarPorPedagio(int codigoPedagio)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE FROM VeiculoReceitaDespesa obj WHERE obj.Pedagio.Codigo = :codigoPedagio").SetInt32("codigoPedagio", codigoPedagio).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE FROM VeiculoReceitaDespesa obj WHERE obj.Pedagio.Codigo = :codigoPedagio").SetInt32("codigoPedagio", codigoPedagio).ExecuteUpdate();

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

        public void DeletarPorTitulo(int codigoTitulo)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE FROM VeiculoReceitaDespesa obj WHERE obj.Titulo.Codigo = :codigoTitulo").SetInt32("codigoTitulo", codigoTitulo).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE FROM VeiculoReceitaDespesa obj WHERE obj.Titulo.Codigo = :codigoTitulo").SetInt32("codigoTitulo", codigoTitulo).ExecuteUpdate();

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

        public void DeletarPorOrdemServico(int codigoOrdemServico)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE FROM VeiculoReceitaDespesa obj WHERE obj.OrdemServico.Codigo = :codigoOrdemServico").SetInt32("codigoOrdemServico", codigoOrdemServico).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE FROM VeiculoReceitaDespesa obj WHERE obj.OrdemServico.Codigo = :codigoOrdemServico").SetInt32("codigoOrdemServico", codigoOrdemServico).ExecuteUpdate();

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

        public void DeletarPorAcertoViagem(int codigoAcertoViagem)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE FROM VeiculoReceitaDespesa obj WHERE obj.AcertoViagem.Codigo = :codigoAcertoViagem").SetInt32("codigoAcertoViagem", codigoAcertoViagem).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE FROM VeiculoReceitaDespesa obj WHERE obj.AcertoViagem.Codigo = :codigoAcertoViagem").SetInt32("codigoAcertoViagem", codigoAcertoViagem).ExecuteUpdate();

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

        #region Relatório Receitas Despesas 

        public async Task<int> ContarConsultaRelatorio(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaReceitaDespesa filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            NHibernate.ISQLQuery consulta = new Consulta.VeiculoReceitaDespesa().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return await consulta.SetTimeout(600).UniqueResultAsync<int>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Veiculos.ReceitaDespesa>> ConsultarRelatorio(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaReceitaDespesa filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            NHibernate.ISQLQuery consulta = new Consulta.VeiculoReceitaDespesa().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);
            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Veiculos.ReceitaDespesa)));

            return await consulta.SetTimeout(600).ListAsync<Dominio.Relatorios.Embarcador.DataSource.Veiculos.ReceitaDespesa>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Veiculos.ReceitaDespesa>> ConsultarRelatorioAsync(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaReceitaDespesa filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            NHibernate.ISQLQuery consulta = new Consulta.VeiculoReceitaDespesa().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);
            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Veiculos.ReceitaDespesa)));

            return await consulta.SetTimeout(600).ListAsync<Dominio.Relatorios.Embarcador.DataSource.Veiculos.ReceitaDespesa>();
        }

        #endregion
    }
}
