using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Linq;

namespace Repositorio
{
    public class OcorrenciaDeCTe : RepositorioBase<Dominio.Entidades.OcorrenciaDeCTe>, Dominio.Interfaces.Repositorios.OcorrenciaDeCTe
    {
        public OcorrenciaDeCTe(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.OcorrenciaDeCTe BuscarPorCodigo(int codigoEmpresa, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeCTe>();
            var result = from obj in query where obj.Codigo == codigo && obj.CTe.Empresa.Codigo == codigoEmpresa select obj;
            return result.FirstOrDefault();
        }

        public int ContarPorCTeETipoDeOcorrencia(int codigoEmpresa, int codigoCTe, string tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeCTe>();
            var result = from obj in query where obj.CTe.Codigo == codigoCTe && obj.CTe.Empresa.Codigo == codigoEmpresa && obj.Ocorrencia.Tipo.Equals(tipo) select obj.Codigo;
            return result.Count();
        }

        public int ContarOcorrenciaCTe(int codigoCTe, int codigoOcorrencia, DateTime dataOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeCTe>();
            var result = from obj in query where obj.CTe.Codigo == codigoCTe && obj.Ocorrencia.Codigo == codigoOcorrencia && obj.DataDaOcorrencia == dataOcorrencia select obj.Codigo;
            return result.Count();
        }

        public List<Dominio.Entidades.OcorrenciaDeCTe> Consultar(int codigoEmpresa, string descricaoTipoOcorrencia, string observacao, string numeroNF, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeCTe>();

            var result = from obj in query where obj.CTe.Empresa.Codigo == codigoEmpresa select obj;

            if (!string.IsNullOrWhiteSpace(descricaoTipoOcorrencia))
                result = result.Where(o => o.Ocorrencia.Descricao.Contains(descricaoTipoOcorrencia));

            if (!string.IsNullOrWhiteSpace(observacao))
                result = result.Where(o => o.Observacao.Contains(observacao));

            if (!string.IsNullOrWhiteSpace(numeroNF))
                result = result.Where(o => (from obj in o.CTe.Documentos where obj.Numero.Equals(numeroNF) select obj.CTE).Contains(o.CTe));

            return result.OrderByDescending(o => o.DataDeCadastro).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigoEmpresa, string descricaoTipoOcorrencia, string observacao, string numeroNF)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeCTe>();

            var result = from obj in query where obj.CTe.Empresa.Codigo == codigoEmpresa select obj;

            if (!string.IsNullOrWhiteSpace(descricaoTipoOcorrencia))
                result = result.Where(o => o.Ocorrencia.Descricao.Contains(descricaoTipoOcorrencia));

            if (!string.IsNullOrWhiteSpace(observacao))
                result = result.Where(o => o.Observacao.Contains(observacao));

            if (!string.IsNullOrWhiteSpace(numeroNF))
                result = result.Where(o => (from obj in o.CTe.Documentos where obj.Numero.Equals(numeroNF) select obj.CTE).Contains(o.CTe));

            return result.Count();
        }

        public List<Dominio.Entidades.OcorrenciaDeCTe> ConsultarPorCTe(int codigoEmpresa, int codigoCTe, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeCTe>();
            var result = from obj in query where obj.CTe.Codigo == codigoCTe && obj.CTe.Empresa.Codigo == codigoEmpresa orderby obj.DataDaOcorrencia descending select obj;
            return result.Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultaPorCTe(int codigoEmpresa, int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeCTe>();
            var result = from obj in query where obj.CTe.Codigo == codigoCTe && obj.CTe.Empresa.Codigo == codigoEmpresa select obj;
            return result.Count();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarCTesPorRemetente(int codigoEmpresa, string cpfCnpjRemetente, DateTime dataInicial, DateTime dataFinal, string[] status, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, List<int> codigosCTes = null, int codigoVeiculo = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeCTe>();
            var queryCTes = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from cte in queryCTes
                         where (from obj in
                                    query
                                where
                                    obj.CTe.Empresa.Codigo == codigoEmpresa &&
                                    obj.CTe.Remetente.CPF_CNPJ.Contains(cpfCnpjRemetente) &&
                                    obj.CTe.DataEmissao >= dataInicial &&
                                    obj.CTe.DataEmissao < dataFinal.Date.AddDays(1) &&
                                    obj.CTe.TipoAmbiente == tipoAmbiente &&
                                    status.Contains(obj.CTe.Status)
                                select obj.CTe.Codigo).Distinct().Contains(cte.Codigo)
                         select cte;

            if (codigosCTes != null && codigosCTes.Count() > 0)
                result = result.Where(o => codigosCTes.Contains(o.Codigo));

            if (codigoVeiculo > 0)
            {
                var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>();

                result = result.Where(o => (from obj in queryVeiculos where obj.CTE.Codigo == o.Codigo && obj.Veiculo.Codigo == codigoVeiculo select obj.CTE.Codigo).Contains(o.Codigo));
            }

            return result.Fetch(o => o.Remetente)
                         .Fetch(o => o.Destinatario)
                         .Fetch(o => o.Empresa)
                         .Fetch(o => o.Serie)
                         .Fetch(o => o.CFOP)
                         .Fetch(o => o.NaturezaDaOperacao)
                         .Fetch(o => o.ModeloDocumentoFiscal)
                         .ToList();
        }

        public List<string> BuscarRemetentesDosCTes(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, string[] status, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, List<int> codigosCTes = null, int codigoVeiculo = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeCTe>();

            var result = from obj in query
                         where
                             obj.CTe.Empresa.Codigo == codigoEmpresa &&
                             obj.CTe.DataEmissao >= dataInicial.Date &&
                             obj.CTe.DataEmissao < dataFinal.AddDays(1).Date &&
                             status.Contains(obj.CTe.Status) &&
                             obj.CTe.TipoAmbiente == tipoAmbiente
                         select obj.CTe;

            if (codigosCTes != null && codigosCTes.Count() > 0)
                result = result.Where(o => codigosCTes.Contains(o.Codigo));

            if (codigoVeiculo > 0)
            {
                var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.VeiculoCTE>();

                result = result.Where(o => (from obj in queryVeiculos where obj.CTE.Codigo == o.Codigo && obj.Veiculo.Codigo == codigoVeiculo select obj.CTE.Codigo).Contains(o.Codigo));
            }

            return result.Select(o => o.Remetente.CPF_CNPJ).Distinct().ToList();
        }

        public int NumeroOcorrenciasPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeCTe>();

            var retorno = from obj in query where obj.CTe.Codigo == codigoCTe select obj;

            return retorno.Count();
        }

        public List<Dominio.Entidades.OcorrenciaDeCTe> BuscarPorCTe(int[] codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeCTe>();

            var retorno = from obj in query where codigoCTe.Contains(obj.CTe.Codigo) select obj;

            return retorno.ToList();
        }

        public Dominio.Entidades.OcorrenciaDeCTe Buscar(int codigoEmpresa, long numeroCTe, string chaveNFe, string cpfCnpjRemetente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeCTe>();

            var result = from obj in query where obj.CTe.Empresa.Codigo == codigoEmpresa && obj.CTe.Remetente.CPF_CNPJ.Equals(cpfCnpjRemetente) select obj;

            if (numeroCTe > 0)
                result = result.Where(o => o.CTe.Numero == numeroCTe);
            else
                result = result.Where(o => (from obj in this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>() where obj.ChaveNFE.Equals(chaveNFe) select obj.CTE.Codigo).Contains(o.CTe.Codigo));

            return result.OrderByDescending(o => o.DataDaOcorrencia).ThenByDescending(o => o.DataDeCadastro).FirstOrDefault();
        }

        public Dominio.Entidades.OcorrenciaDeCTe BuscarPorChaveCTe(string chaveCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeCTe>();

            var result = from obj in query where obj.CTe.Chave.Equals(chaveCTe) select obj;

            return result.OrderByDescending(o => o.Codigo).FirstOrDefault();
        }

        public void DeletarPorCTe(int codigoCTe)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE OcorrenciaDeCTe obj WHERE obj.CTe.Codigo = :codigoCTe")
                                     .SetInt32("codigoCTe", codigoCTe)
                                     .ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE OcorrenciaDeCTe obj WHERE obj.CTe.Codigo = :codigoCTe")
                                    .SetInt32("codigoCTe", codigoCTe)
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

        public List<Dominio.ObjetosDeValor.Relatorios.RelatorioOcorrenciasCTe> ObterRelatorio(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, string cpfCnpjRemetente, string cpfCnpjExpedidor, string cpfCnpjRecebedor, string cpfCnpjDestinatario, string cpfCnpjTomador, int codigoLocalidadeInicioPrestacao, int codigoLocalidadeTerminoPrestacao, int codigoOcorrencia, string tipoOcorrencia, int numeroInicial, int numeroFinal, string numeroNF)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeCTe>();

            var result = from obj in query where obj.CTe.Empresa.Codigo == codigoEmpresa select obj;

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataDaOcorrencia >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataDaOcorrencia < dataFinal.AddDays(1).Date);

            if (numeroInicial > 0)
                result = result.Where(o => o.CTe.Numero >= numeroInicial);

            if (numeroFinal > 0)
                result = result.Where(o => o.CTe.Numero <= numeroFinal);

            if (codigoOcorrencia > 0)
                result = result.Where(o => o.Ocorrencia.Codigo == codigoOcorrencia);

            if (!string.IsNullOrWhiteSpace(tipoOcorrencia))
                result = result.Where(o => o.Ocorrencia.Tipo.Equals(tipoOcorrencia));

            if (codigoLocalidadeInicioPrestacao > 0)
                result = result.Where(o => o.CTe.LocalidadeInicioPrestacao.Codigo == codigoLocalidadeInicioPrestacao);

            if (codigoLocalidadeTerminoPrestacao > 0)
                result = result.Where(o => o.CTe.LocalidadeTerminoPrestacao.Codigo == codigoLocalidadeTerminoPrestacao);

            if (!string.IsNullOrWhiteSpace(cpfCnpjRemetente))
                result = result.Where(o => o.CTe.Remetente.CPF_CNPJ.Equals(cpfCnpjRemetente));

            if (!string.IsNullOrWhiteSpace(cpfCnpjExpedidor))
                result = result.Where(o => o.CTe.Expedidor.CPF_CNPJ.Equals(cpfCnpjExpedidor));

            if (!string.IsNullOrWhiteSpace(cpfCnpjRecebedor))
                result = result.Where(o => o.CTe.Recebedor.CPF_CNPJ.Equals(cpfCnpjRecebedor));

            if (!string.IsNullOrWhiteSpace(cpfCnpjDestinatario))
                result = result.Where(o => o.CTe.Destinatario.CPF_CNPJ.Equals(cpfCnpjDestinatario));

            if (!string.IsNullOrWhiteSpace(cpfCnpjTomador))
                result = result.Where(o => (o.CTe.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario && o.CTe.Destinatario.CPF_CNPJ == cpfCnpjTomador) ||
                                           (o.CTe.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor && o.CTe.Expedidor.CPF_CNPJ == cpfCnpjTomador) ||
                                           (o.CTe.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros && o.CTe.OutrosTomador.CPF_CNPJ == cpfCnpjTomador) ||
                                           (o.CTe.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor && o.CTe.Recebedor.CPF_CNPJ == cpfCnpjTomador) ||
                                           (o.CTe.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente && o.CTe.Remetente.CPF_CNPJ == cpfCnpjTomador));

            if (!string.IsNullOrWhiteSpace(numeroNF))
                result = result.Where(o => (from obj in o.CTe.Documentos where obj.Numero.Equals(numeroNF) select obj.CTE).Contains(o.CTe));

            return result.Select(o => new Dominio.ObjetosDeValor.Relatorios.RelatorioOcorrenciasCTe()
            {
                Codigo = o.Codigo,
                CodigoCTe = o.CTe.Codigo,
                CPFCNPJDestinatario = o.CTe.Destinatario.CPF_CNPJ,
                CPFCNPJRemetente = o.CTe.Remetente.CPF_CNPJ,
                DataOcorrencia = o.DataDaOcorrencia,
                NomeDestinatario = o.CTe.Destinatario.Nome,
                NomeRemetente = o.CTe.Remetente.Nome,
                NumeroCTe = o.CTe.Numero,
                NumeroNF = (from obj in o.CTe.Documentos select obj.Numero).FirstOrDefault(),
                Ocorrencia = o.Ocorrencia.Descricao,
                SerieCTe = o.CTe.Serie.Numero,
                TipoOcorrencia = o.Ocorrencia.Tipo,
                Observacao = o.Observacao
            }).ToList();
        }

        public IEnumerable<string> ObterNumeroNF(int codigoCTe)
        {
            var query = from doc in this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosCTE>() select doc.Numero;

            return query;
        }

        public Dominio.Entidades.OcorrenciaDeCTe BuscarUltimaDoCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeCTe>();

            var retorno = from obj in query where obj.CTe.Codigo == codigoCTe orderby obj.DataDaOcorrencia descending, obj.Codigo descending select obj;

            return retorno.FirstOrDefault();
        }

        public List<Dominio.Entidades.OcorrenciaDeCTe> BuscarPorEmpresaRemetenteData(int codigoEmpresa, string cnpjRemetente, DateTime dataInicio, DateTime dataFim)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeCTe>();

            var result = from obj in query where obj.CTe.Empresa.Codigo == codigoEmpresa select obj;

            if (!string.IsNullOrWhiteSpace(cnpjRemetente))
                result = result.Where(o => o.CTe.Remetente.CPF_CNPJ.Equals(cnpjRemetente));

            if (dataInicio > DateTime.MinValue && dataFim > DateTime.MinValue)
                result = result.Where(o => o.DataDeCadastro >= dataInicio && o.DataDeCadastro <= dataFim);

            return result.ToList();
        }

        public List<Dominio.Entidades.OcorrenciaDeCTe> BuscarPorEmpresaTomadorData(int codigoEmpresa, string cnpjTomador, DateTime dataInicio, DateTime dataFim)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeCTe>();

            var result = from obj in query where obj.CTe.Empresa.Codigo == codigoEmpresa select obj;

            if (!string.IsNullOrWhiteSpace(cnpjTomador))
                result = result.Where(o => o.CTe.TomadorPagador.CPF_CNPJ.Equals(cnpjTomador));

            if (dataInicio > DateTime.MinValue && dataFim > DateTime.MinValue)
                result = result.Where(o => o.DataDeCadastro >= dataInicio && o.DataDeCadastro <= dataFim);

            return result.ToList();
        }
    }
}
