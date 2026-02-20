using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using Dominio.Enumeradores;

namespace Repositorio.Embarcador.Fatura
{
    public class FaturaCargaDocumento : RepositorioBase<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento>
    {
        public FaturaCargaDocumento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public void DeletarPorFatura(int codigoFatura)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE FaturaCargaDocumento obj WHERE obj.Fatura.Codigo = :codigoFatura")
                                     .SetInt32("codigoFatura", codigoFatura)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE FaturaCargaDocumento obj WHERE obj.Fatura.Codigo = :codigoFatura")
                                .SetInt32("codigoFatura", codigoFatura)
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

        public void LimpaConhecimentoPorFatura(int codigoFatura)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("UPDATE ConhecimentoDeTransporteEletronico obj SET obj.Fatura = null WHERE obj.Fatura.Codigo = :codigoFatura")
                                     .SetInt32("codigoFatura", codigoFatura)
                                     .SetTimeout(3000)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("UPDATE ConhecimentoDeTransporteEletronico obj SET obj.Fatura = null WHERE obj.Fatura.Codigo = :codigoFatura")
                                .SetInt32("codigoFatura", codigoFatura)
                                .SetTimeout(3000)
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

        public List<int> BuscarCodigosCTesCSTs(int codigoFatura, List<string> CSTs, bool comCSTs, bool modeloCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura && o.StatusDocumentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDocumentoFatura.Normal && o.ConhecimentoDeTransporteEletronico.Status == "A");

            if (modeloCTe)
            {
                query = query.Where(o => o.ConhecimentoDeTransporteEletronico.ModeloDocumentoFiscal.TipoDocumentoEmissao == TipoDocumento.CTe);
                if (comCSTs)
                    query = query.Where(o => CSTs.Contains(o.ConhecimentoDeTransporteEletronico.CST));
                else
                    query = query.Where(o => !CSTs.Contains(o.ConhecimentoDeTransporteEletronico.CST));
            }
            else
            {
                query = query.Where(o => o.ConhecimentoDeTransporteEletronico.ModeloDocumentoFiscal.TipoDocumentoEmissao != TipoDocumento.CTe);
            }

            return query.Select(o => o.ConhecimentoDeTransporteEletronico.Codigo).Distinct().ToList();
        }

        public decimal BuscarTotalReceberCSTs(int codigoFatura, List<string> CSTs, bool comCSTs, bool modeloCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura && o.StatusDocumentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDocumentoFatura.Normal && o.ConhecimentoDeTransporteEletronico.Status == "A");

            if (modeloCTe)
            {
                query = query.Where(o => o.ConhecimentoDeTransporteEletronico.ModeloDocumentoFiscal.TipoDocumentoEmissao == TipoDocumento.CTe);
                if (comCSTs)
                    query = query.Where(o => CSTs.Contains(o.ConhecimentoDeTransporteEletronico.CST));
                else
                    query = query.Where(o => !CSTs.Contains(o.ConhecimentoDeTransporteEletronico.CST));
            }
            else
            {
                query = query.Where(o => o.ConhecimentoDeTransporteEletronico.ModeloDocumentoFiscal.TipoDocumentoEmissao != TipoDocumento.CTe);
            }

            return query.Sum(o => (decimal?)o.ConhecimentoDeTransporteEletronico.ValorAReceber) ?? 0m;
        }

        public decimal BuscarTotalICMS(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura && o.ConhecimentoDeTransporteEletronico.CST != "60" && o.StatusDocumentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDocumentoFatura.Normal && o.ConhecimentoDeTransporteEletronico.Status == "A");

            return query.Sum(o => (decimal?)o.ConhecimentoDeTransporteEletronico.ValorICMS) ?? 0m;
        }

        public decimal BuscarTotalICMSSST(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura && o.ConhecimentoDeTransporteEletronico.CST == "60" && o.StatusDocumentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDocumentoFatura.Normal && o.ConhecimentoDeTransporteEletronico.Status == "A");

            return query.Sum(o => (decimal?)o.ConhecimentoDeTransporteEletronico.ValorICMS) ?? 0m;
        }

        public decimal BuscarTotalISS(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura && o.ConhecimentoDeTransporteEletronico.ModeloDocumentoFiscal.TipoDocumentoEmissao != TipoDocumento.CTe && o.StatusDocumentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDocumentoFatura.Normal && o.ConhecimentoDeTransporteEletronico.Status == "A");

            return query.Sum(o => (decimal?)o.ConhecimentoDeTransporteEletronico.ValorISS) ?? 0m;
        }

        public decimal BuscarTotalISSRetido(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura && o.ConhecimentoDeTransporteEletronico.ModeloDocumentoFiscal.TipoDocumentoEmissao != TipoDocumento.CTe && o.StatusDocumentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDocumentoFatura.Normal && o.ConhecimentoDeTransporteEletronico.Status == "A");

            return query.Sum(o => (decimal?)o.ConhecimentoDeTransporteEletronico.ValorISSRetido) ?? 0m;
        }



        public decimal BuscarTotalDaPrestacaoComCST(int codigoFatura, List<string> CSTs, bool comCSTs, bool modeloCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura && o.StatusDocumentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDocumentoFatura.Normal && o.ConhecimentoDeTransporteEletronico.Status == "A");

            if (modeloCTe)
            {
                query = query.Where(o => o.ConhecimentoDeTransporteEletronico.ModeloDocumentoFiscal.TipoDocumentoEmissao == TipoDocumento.CTe);
                if (comCSTs)
                    query = query.Where(o => CSTs.Contains(o.ConhecimentoDeTransporteEletronico.CST));
                else
                    query = query.Where(o => !CSTs.Contains(o.ConhecimentoDeTransporteEletronico.CST));
            }
            else
            {
                query = query.Where(o => o.ConhecimentoDeTransporteEletronico.ModeloDocumentoFiscal.TipoDocumentoEmissao != TipoDocumento.CTe);
            }

            return query.Sum(o => (decimal?)o.ConhecimentoDeTransporteEletronico.ValorPrestacaoServico) ?? 0m;
        }

        public Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.Fetch(o => o.ConhecimentoDeTransporteEletronico).Fetch(o => o.Fatura).FirstOrDefault();
        }

        public bool ContemCanhotoPendente(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento>();
            var result = from obj in query where obj.Fatura.Codigo == codigo && obj.TipoDocumentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFatura.Conhecimento && obj.StatusDocumentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDocumentoFatura.Normal select obj;
            return result.Any(obj => obj.ConhecimentoDeTransporteEletronico.XMLNotaFiscais.Any(nfe => nfe.Canhoto.SituacaoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Pendente));
        }

        public List<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento> ListaCanhotosPendente(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento>();
            var result = from obj in query where obj.Fatura.Codigo == codigo && obj.TipoDocumentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFatura.Conhecimento && obj.StatusDocumentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDocumentoFatura.Normal select obj;
            result = result.Where(obj => obj.ConhecimentoDeTransporteEletronico.XMLNotaFiscais.Any(nfe => nfe.Canhoto.SituacaoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Pendente));
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento> ConhecimentosTitulo(int codigoTitulo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento>();
            var result = from obj in query where obj.TipoDocumentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFatura.Conhecimento && obj.StatusDocumentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDocumentoFatura.Normal select obj;

            var queryTitulo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var resultoTitulo = from obj in queryTitulo where obj.Codigo == codigoTitulo && obj.FaturaParcela != null select obj;

            result = result.Where(obj => resultoTitulo.Select(a => a.FaturaParcela.Fatura).Contains(obj.Fatura));

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConhecimentosTitulo(int codigoTitulo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento>();
            var result = from obj in query where obj.TipoDocumentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFatura.Conhecimento && obj.StatusDocumentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDocumentoFatura.Normal select obj;

            var queryTitulo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var resultoTitulo = from obj in queryTitulo where obj.Codigo == codigoTitulo && obj.FaturaParcela != null select obj;

            result = result.Where(obj => resultoTitulo.Select(a => a.FaturaParcela.Fatura).Contains(obj.Fatura));

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento> BuscarPorFatura(int codigoFatura, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDocumentoFatura situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento>();

            var result = from obj in query where obj.Fatura.Codigo == codigoFatura && obj.StatusDocumentoFatura == situacao select obj;
            
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento BuscarPorFatura(int codigoFatura, int codigoDocumento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFatura tipoDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento>();
            var result = from obj in query where obj.Fatura.Codigo == codigoFatura select obj;
            if (tipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFatura.Conhecimento)
                result = result.Where(obj => obj.ConhecimentoDeTransporteEletronico.Codigo == codigoDocumento);
            else
                result = result.Where(obj => obj.NFSe.Codigo == codigoDocumento);
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento> BuscarPorCarga(int codigoFatura, int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento>();
            var result = from obj in query where obj.Fatura.Codigo == codigoFatura && obj.Carga.Codigo == codigoCarga select obj;
            return result.Fetch(o => o.ConhecimentoDeTransporteEletronico).ToList();
        }

        public List<int> BuscarCodigosPorCarga(int codigoFatura, int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento>();
            var result = from obj in query where obj.Fatura.Codigo == codigoFatura && obj.Carga.Codigo == codigoCarga select obj;
            return result.Select(o => o.Codigo).ToList();
        }

        public bool ContemDocumentoFatura(int codigoFatura, int codigoDocumento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFatura tipoDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento>();

            var result = from obj in query where obj.Fatura.Codigo == codigoFatura select obj;

            if (tipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFatura.Conhecimento)
                result = result.Where(obj => obj.ConhecimentoDeTransporteEletronico.Codigo == codigoDocumento);
            else
                result = result.Where(obj => obj.NFSe.Codigo == codigoDocumento);

            return result.Select(o => o.Codigo).Any();
        }

        public bool ContemDocumentoEmOutraFatura(int codigoFatura, int codigoDocumento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFatura tipoDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento>();
            var result = from obj in query where obj.Fatura.Codigo != codigoFatura && obj.StatusDocumentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDocumentoFatura.Normal && obj.Fatura.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.Cancelado select obj;
            if (tipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFatura.Conhecimento)
                result = result.Where(obj => obj.ConhecimentoDeTransporteEletronico.Codigo == codigoDocumento);
            else
                result = result.Where(obj => obj.NFSe.Codigo == codigoDocumento);

            return result.Count() > 0;
        }

        public Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento BuscarPorConhecimento(int codigo, Dominio.Entidades.Embarcador.Fatura.Fatura fatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento>();
            var result = from obj in query where obj.ConhecimentoDeTransporteEletronico.Codigo == codigo select obj;

            if (fatura.TipoPessoa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.Pessoa && fatura.Cliente != null)
            {
                string cnpjPessoa = Convert.ToString(fatura.Cliente.CPF_CNPJ);
                result = result.Where(obj => obj.ConhecimentoDeTransporteEletronico.DataEmissao >= fatura.DataInicial && obj.ConhecimentoDeTransporteEletronico.DataEmissao <= fatura.DataFinal.Value.AddDays(1)
                                         && (obj.ConhecimentoDeTransporteEletronico.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario ? obj.ConhecimentoDeTransporteEletronico.Destinatario.CPF_CNPJ.Equals(cnpjPessoa) :
                                         obj.ConhecimentoDeTransporteEletronico.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor ? obj.ConhecimentoDeTransporteEletronico.Expedidor.CPF_CNPJ.Equals(cnpjPessoa) :
                                         obj.ConhecimentoDeTransporteEletronico.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros ? obj.ConhecimentoDeTransporteEletronico.OutrosTomador.CPF_CNPJ.Equals(cnpjPessoa) :
                                         obj.ConhecimentoDeTransporteEletronico.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor ? obj.ConhecimentoDeTransporteEletronico.Recebedor.CPF_CNPJ.Equals(cnpjPessoa) :
                                         obj.ConhecimentoDeTransporteEletronico.Remetente.CPF_CNPJ.Equals(cnpjPessoa))
                                         && obj.ConhecimentoDeTransporteEletronico.Status == "A");

            }
            else if (fatura.TipoPessoa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.GrupoPessoa && fatura.GrupoPessoas != null && fatura.GrupoPessoas.Clientes != null)
            {
                for (int i = 0; i < fatura.GrupoPessoas.Clientes.Count; i++)
                {
                    string cnpjPessoa = Convert.ToString(fatura.GrupoPessoas.Clientes[i].CPF_CNPJ);
                    result = result.Where(obj => obj.ConhecimentoDeTransporteEletronico.DataEmissao >= fatura.DataInicial && obj.ConhecimentoDeTransporteEletronico.DataEmissao <= fatura.DataFinal.Value.AddDays(1)
                                         && (obj.ConhecimentoDeTransporteEletronico.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario ? obj.ConhecimentoDeTransporteEletronico.Destinatario.CPF_CNPJ.Equals(cnpjPessoa) :
                                         obj.ConhecimentoDeTransporteEletronico.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor ? obj.ConhecimentoDeTransporteEletronico.Expedidor.CPF_CNPJ.Equals(cnpjPessoa) :
                                         obj.ConhecimentoDeTransporteEletronico.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros ? obj.ConhecimentoDeTransporteEletronico.OutrosTomador.CPF_CNPJ.Equals(cnpjPessoa) :
                                         obj.ConhecimentoDeTransporteEletronico.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor ? obj.ConhecimentoDeTransporteEletronico.Recebedor.CPF_CNPJ.Equals(cnpjPessoa) :
                                         obj.ConhecimentoDeTransporteEletronico.Remetente.CPF_CNPJ.Equals(cnpjPessoa))
                                         && obj.ConhecimentoDeTransporteEletronico.Status == "A");
                }
            }

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento> ConsultaDocumentosCargaFatura(int codigoModeloDocumento, string numeroCarga, string numeroPedido, string numeroOcorrencia, decimal aliquotaICMS, Dominio.Enumeradores.TipoCTE tipoCTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento tipoDocumento, decimal valorDocumento, int numeroDocumento, int codigoCarga, Dominio.Entidades.Embarcador.Fatura.Fatura fatura, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, int numeroDocumentoFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento>();
            var resultCTe = from obj in query where obj.Fatura.Codigo == fatura.Codigo && obj.Carga != null select obj;

            if (codigoCarga > 0)
                resultCTe = resultCTe.Where(obj => obj.Carga.Codigo == codigoCarga);

            if (!string.IsNullOrWhiteSpace(numeroCarga))
                resultCTe = resultCTe.Where(obj => obj.Carga.CodigoCargaEmbarcador == numeroCarga);

            if (valorDocumento > 0)
                resultCTe = resultCTe.Where(obj => obj.ConhecimentoDeTransporteEletronico.ValorAReceber.Equals(valorDocumento));
            if (numeroDocumento > 0 && numeroDocumentoFinal == 0)
                resultCTe = resultCTe.Where(obj => obj.ConhecimentoDeTransporteEletronico.Numero == numeroDocumento);
            else if (numeroDocumento > 0 && numeroDocumentoFinal > 0)
                resultCTe = resultCTe.Where(obj => obj.ConhecimentoDeTransporteEletronico.Numero >= numeroDocumento && obj.ConhecimentoDeTransporteEletronico.Numero <= numeroDocumentoFinal);
            else if (numeroDocumento == 0 && numeroDocumentoFinal > 0)
                resultCTe = resultCTe.Where(obj => obj.ConhecimentoDeTransporteEletronico.Numero == numeroDocumentoFinal);

            if ((int)tipoCTe >= 0)
                resultCTe = resultCTe.Where(obj => obj.ConhecimentoDeTransporteEletronico.TipoCTE == tipoCTe);

            if (aliquotaICMS > 0)
                resultCTe = resultCTe.Where(obj => obj.ConhecimentoDeTransporteEletronico.AliquotaICMS.Equals(aliquotaICMS));

            if (codigoModeloDocumento > 0)
                resultCTe = resultCTe.Where(obj => obj.ConhecimentoDeTransporteEletronico.ModeloDocumentoFiscal.Codigo.Equals(codigoModeloDocumento));

            long vNumeroPedido = 0;
            long.TryParse(numeroPedido, out vNumeroPedido);
            if (vNumeroPedido > 0)
            {
                var queryAvon = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoAvon>();
                var queryNatura = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoNatura>();
                var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

                var resultAvon = from obj in queryAvon where obj.NumeroMinuta.Equals(vNumeroPedido) select obj;
                var resultNatura = from obj in queryNatura where obj.DocumentoTransporte.Numero.Equals(vNumeroPedido) select obj;
                var resultCargaPedido = from obj in queryCargaPedido where obj.Pedido.NumeroPedidoEmbarcador.Contains(numeroPedido) select obj;

                resultCTe = resultCTe.Where(obj =>
                    obj.Fatura.Cargas.Select(a => a.Carga.Codigo).Contains(resultAvon.Select(b => b.Carga.Codigo).FirstOrDefault()) ||
                    obj.Fatura.Cargas.Select(a => a.Carga.Codigo).Contains(resultNatura.Select(b => b.Carga.Codigo).FirstOrDefault()) ||
                    obj.Fatura.Cargas.Select(a => a.Carga.Codigo).Contains(resultCargaPedido.Select(b => b.Carga.Codigo).FirstOrDefault()));
            }
            else if (!string.IsNullOrWhiteSpace(numeroPedido))
            {
                var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
                var resultCargaPedido = from obj in queryCargaPedido where obj.Pedido.NumeroPedidoEmbarcador.Contains(numeroPedido) select obj;

                resultCTe = resultCTe.Where(obj => obj.Fatura.Cargas.Select(a => a.Carga.Codigo).Contains(resultCargaPedido.Select(b => b.Carga.Codigo).FirstOrDefault()));
            }

            if (!string.IsNullOrWhiteSpace(numeroOcorrencia))
                resultCTe = resultCTe.Where(o => o.Carga.Ocorrencias.Any(oco => oco.NumeroOcorrenciaCliente.Contains(numeroOcorrencia)));

            return resultCTe.Fetch(o => o.ConhecimentoDeTransporteEletronico).OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultaDocumentosCargaFatura(int codigoModeloDocumento, string numeroCarga, string numeroPedido, string numeroOcorrencia, decimal aliquotaICMS, Dominio.Enumeradores.TipoCTE tipoCTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento tipoDocumento, decimal valorDocumento, int numeroDocumento, int codigoCarga, Dominio.Entidades.Embarcador.Fatura.Fatura fatura, int numeroDocumentoFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento>();
            var resultCTe = from obj in query where obj.Fatura.Codigo == fatura.Codigo && obj.Carga != null select obj;

            if (codigoCarga > 0)
                resultCTe = resultCTe.Where(obj => obj.Carga.Codigo == codigoCarga);

            if (!string.IsNullOrWhiteSpace(numeroCarga))
                resultCTe = resultCTe.Where(obj => obj.Carga.CodigoCargaEmbarcador == numeroCarga);

            if (valorDocumento > 0)
                resultCTe = resultCTe.Where(obj => obj.ConhecimentoDeTransporteEletronico.ValorAReceber.Equals(valorDocumento));
            if (numeroDocumento > 0 && numeroDocumentoFinal == 0)
                resultCTe = resultCTe.Where(obj => obj.ConhecimentoDeTransporteEletronico.Numero == numeroDocumento);
            else if (numeroDocumento > 0 && numeroDocumentoFinal > 0)
                resultCTe = resultCTe.Where(obj => obj.ConhecimentoDeTransporteEletronico.Numero >= numeroDocumento && obj.ConhecimentoDeTransporteEletronico.Numero <= numeroDocumentoFinal);
            else if (numeroDocumento == 0 && numeroDocumentoFinal > 0)
                resultCTe = resultCTe.Where(obj => obj.ConhecimentoDeTransporteEletronico.Numero == numeroDocumentoFinal);

            if ((int)tipoCTe >= 0)
                resultCTe = resultCTe.Where(obj => obj.ConhecimentoDeTransporteEletronico.TipoCTE == tipoCTe);

            if (aliquotaICMS > 0)
                resultCTe = resultCTe.Where(obj => obj.ConhecimentoDeTransporteEletronico.AliquotaICMS.Equals(aliquotaICMS));

            if (codigoModeloDocumento > 0)
                resultCTe = resultCTe.Where(obj => obj.ConhecimentoDeTransporteEletronico.ModeloDocumentoFiscal.Codigo.Equals(codigoModeloDocumento));

            long vNumeroPedido = 0;
            long.TryParse(numeroPedido, out vNumeroPedido);
            if (vNumeroPedido > 0)
            {
                var queryAvon = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoAvon>();
                var queryNatura = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoNatura>();
                var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

                var resultAvon = from obj in queryAvon where obj.NumeroMinuta.Equals(vNumeroPedido) select obj;
                var resultNatura = from obj in queryNatura where obj.DocumentoTransporte.Numero.Equals(vNumeroPedido) select obj;
                var resultCargaPedido = from obj in queryCargaPedido where obj.Pedido.NumeroPedidoEmbarcador.Contains(numeroPedido) select obj;

                resultCTe = resultCTe.Where(obj =>
                    obj.Fatura.Cargas.Select(a => a.Carga.Codigo).Contains(resultAvon.Select(b => b.Carga.Codigo).FirstOrDefault()) ||
                    obj.Fatura.Cargas.Select(a => a.Carga.Codigo).Contains(resultNatura.Select(b => b.Carga.Codigo).FirstOrDefault()) ||
                    obj.Fatura.Cargas.Select(a => a.Carga.Codigo).Contains(resultCargaPedido.Select(b => b.Carga.Codigo).FirstOrDefault()));
            }
            else if (!string.IsNullOrWhiteSpace(numeroPedido))
            {
                var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
                var resultCargaPedido = from obj in queryCargaPedido where obj.Pedido.NumeroPedidoEmbarcador.Contains(numeroPedido) select obj;

                resultCTe = resultCTe.Where(obj => obj.Fatura.Cargas.Select(a => a.Carga.Codigo).Contains(resultCargaPedido.Select(b => b.Carga.Codigo).FirstOrDefault()));
            }

            if (!string.IsNullOrWhiteSpace(numeroOcorrencia))
                resultCTe = resultCTe.Where(o => o.Carga.Ocorrencias.Any(oco => oco.NumeroOcorrenciaCliente.Contains(numeroOcorrencia)));

            return resultCTe.Distinct().Count();
        }

        public int NumeroFaturaDocumento(int codigoConhecimento, int codigoCarga, int codigoFatura)
        {
            var queryFatura = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCarga>();
            var resultFatura = from obj in queryFatura
                               where obj.StatusFaturaCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusFaturaCarga.NaoFaturada
                               && obj.Carga.Codigo == codigoCarga
                               select obj;

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var resultConhecimento = from obj in query where obj.Carga.Codigo == codigoCarga && obj.CTe.Codigo == codigoConhecimento select obj;
            resultConhecimento = resultConhecimento.Where(obj => (from p in resultFatura select p.Carga).Contains(obj.Carga));

            var queryDocumentosExcluidos = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumentoExcluido>();
            var resultDocumentosExcluidos = from obj in queryDocumentosExcluidos
                                            where obj.FaturaCarga.Carga.Codigo == codigoCarga
                                            && obj.ConhecimentoDeTransporteEletronico.Codigo == codigoConhecimento
                                            && obj.FaturaCarga.Fatura.Codigo != codigoFatura
                                            select obj;
            if (resultDocumentosExcluidos.Count() > 0)
            {
                var resultFaturaCargaConhecimento = resultFatura.Where(obj => !(from p in resultDocumentosExcluidos select p.FaturaCarga.Fatura).Contains(obj.Fatura));
                if (resultFaturaCargaConhecimento.Count() > 0)
                    return resultFaturaCargaConhecimento.FirstOrDefault().Fatura.Numero;
                else
                {
                    if (resultFatura.Count() > 0)
                        return resultFatura.FirstOrDefault().Fatura.Numero;
                    else
                        return 0;
                }

            }
            else
            {
                if (resultFatura.Count() > 0)
                    return resultFatura.FirstOrDefault().Fatura.Numero;
                else
                    return 0;
            }
        }

        public List<int> BuscarListaCodigosCTes(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento>();

            if (codigoFatura > 0)
                query = query.Where(o => o.Fatura.Codigo == codigoFatura && o.TipoDocumentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFatura.Conhecimento && o.StatusDocumentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDocumentoFatura.Normal);

            return query.Select(o => o.ConhecimentoDeTransporteEletronico.Codigo).ToList();
        }

        public List<int> BuscarListaCodigosDocumentos(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento>();

            if (codigoFatura > 0)
                query = query.Where(o => o.Fatura.Codigo == codigoFatura && o.TipoDocumentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFatura.Conhecimento && o.StatusDocumentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDocumentoFatura.Normal);

            return query.Select(o => o.Codigo).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento> BuscarDocumentosFatura(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento>();

            if (codigoFatura > 0)
                query = query.Where(o => o.Fatura.Codigo == codigoFatura && o.TipoDocumentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFatura.Conhecimento && o.StatusDocumentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDocumentoFatura.Normal);

            return query.ToList();
        }

        public List<string> BuscarChavesCTesAutorizadosFatura(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento>();

            if (codigoFatura > 0)
                query = query.Where(o => o.Fatura.Codigo == codigoFatura && o.TipoDocumentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFatura.Conhecimento && o.StatusDocumentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDocumentoFatura.Normal && o.ConhecimentoDeTransporteEletronico.Status == "A");

            return query.Select(o => o.ConhecimentoDeTransporteEletronico.Chave).ToList();
        }

        public Dominio.Enumeradores.TipoAmbiente BuscarTipoAmbienteCTeFatura(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento>();

            if (codigoFatura > 0)
                query = query.Where(o => o.Fatura.Codigo == codigoFatura && o.TipoDocumentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFatura.Conhecimento && o.StatusDocumentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDocumentoFatura.Normal && o.ConhecimentoDeTransporteEletronico.Status == "A");

            return query.Select(o => o.ConhecimentoDeTransporteEletronico.TipoAmbiente).FirstOrDefault();
        }

        public string BuscarCNPJRemetenteCTeFatura(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento>();

            if (codigoFatura > 0)
                query = query.Where(o => o.Fatura.Codigo == codigoFatura && o.TipoDocumentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFatura.Conhecimento && o.StatusDocumentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDocumentoFatura.Normal && o.ConhecimentoDeTransporteEletronico.Status == "A");

            return query.Select(o => o.ConhecimentoDeTransporteEletronico.Remetente.CPF_CNPJ).FirstOrDefault();
        }

        public DateTime? BuscarPrimeiraDataEmissaoCTe(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento>();

            if (codigoFatura > 0)
                query = query.Where(o => o.Fatura.Codigo == codigoFatura && o.TipoDocumentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFatura.Conhecimento && o.StatusDocumentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDocumentoFatura.Normal);

            query = query.Fetch(o => o.ConhecimentoDeTransporteEletronico);
            query = query.OrderBy("ConhecimentoDeTransporteEletronico.DataEmissao ascending");

            return query.FirstOrDefault().ConhecimentoDeTransporteEletronico.DataEmissao;
        }

        public DateTime? BuscarPrimeiraDataPrevisaoEncerramentoCTe(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento>();

            if (codigoFatura > 0)
                query = query.Where(o => o.Fatura.Codigo == codigoFatura && o.TipoDocumentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFatura.Conhecimento && o.StatusDocumentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDocumentoFatura.Normal);

            query = query.Fetch(o => o.ConhecimentoDeTransporteEletronico);
            query = query.OrderBy("ConhecimentoDeTransporteEletronico.DataPrevistaEntrega ascending");

            return query.FirstOrDefault().ConhecimentoDeTransporteEletronico.DataPrevistaEntrega;
        }

        public List<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento> ConsultaDocumentosCargaFatura(string numeroCarga, long numeroPedido, decimal aliquotaICMS, Dominio.Enumeradores.TipoCTE tipoCTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento tipoDocumento, decimal valorDocumento, int numeroDocumento, int codigoCarga, Dominio.Entidades.Embarcador.Fatura.Fatura fatura, int numeroDocumentoFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento>();
            var resultCTe = from obj in query where obj.Fatura.Codigo == fatura.Codigo && obj.Carga != null select obj;

            if (codigoCarga > 0)
                resultCTe = resultCTe.Where(obj => obj.Carga.Codigo == codigoCarga);

            if (!string.IsNullOrWhiteSpace(numeroCarga))
                resultCTe = resultCTe.Where(obj => obj.Carga.CodigoCargaEmbarcador == numeroCarga);

            if (valorDocumento > 0)
                resultCTe = resultCTe.Where(obj => obj.ConhecimentoDeTransporteEletronico.ValorAReceber.Equals(valorDocumento));
            if (numeroDocumento > 0 && numeroDocumentoFinal == 0)
                resultCTe = resultCTe.Where(obj => obj.ConhecimentoDeTransporteEletronico.Numero == numeroDocumento);
            else if (numeroDocumento > 0 && numeroDocumentoFinal > 0)
                resultCTe = resultCTe.Where(obj => obj.ConhecimentoDeTransporteEletronico.Numero >= numeroDocumento && obj.ConhecimentoDeTransporteEletronico.Numero <= numeroDocumentoFinal);
            else if (numeroDocumento == 0 && numeroDocumentoFinal > 0)
                resultCTe = resultCTe.Where(obj => obj.ConhecimentoDeTransporteEletronico.Numero == numeroDocumentoFinal);

            if ((int)tipoCTe >= 0)
                resultCTe = resultCTe.Where(obj => obj.ConhecimentoDeTransporteEletronico.TipoCTE == tipoCTe);

            if (aliquotaICMS > 0)
                resultCTe = resultCTe.Where(obj => obj.ConhecimentoDeTransporteEletronico.AliquotaICMS.Equals(aliquotaICMS));

            if (numeroPedido > 0)
            {
                var queryAvon = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoAvon>();
                var queryNatura = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoNatura>();

                var resultAvon = from obj in queryAvon where obj.NumeroMinuta.Equals(numeroPedido) select obj;
                var resultNatura = from obj in queryNatura where obj.DocumentoTransporte.Numero.Equals(numeroPedido) select obj;

                resultCTe = resultCTe.Where(obj => resultAvon.Select(a => a.Carga).Contains(obj.Carga) || resultNatura.Select(a => a.Carga).Contains(obj.Carga));
            }

            return resultCTe.Fetch(o => o.ConhecimentoDeTransporteEletronico).Fetch(o => o.Carga).ToList();
        }
    }
}
