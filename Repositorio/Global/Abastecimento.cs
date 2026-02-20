using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Criterion;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Threading;
using System.Threading.Tasks;
using NHibernate;

namespace Repositorio
{
    public class Abastecimento : RepositorioBase<Dominio.Entidades.Abastecimento>, Dominio.Interfaces.Repositorios.Abastecimento
    {
        public Abastecimento(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public Abastecimento(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public Dominio.Entidades.Abastecimento BuscarPorCodigoEAcertoDeViagem(int codigo, int codigoAcertoViagem)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();
            var result = from obj in query where obj.Codigo == codigo && obj.AcertoDeViagem.Codigo == codigoAcertoViagem select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Abastecimento BuscarPorCodigoEDocumentoEntrada(int codigo, int codigoDocumentoEntrada)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();
            var result = from obj in query where obj.Codigo == codigo && obj.DocumentoEntrada.Codigo == codigoDocumentoEntrada select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Abastecimento> ConsultarPentendeIntegracao(string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();

            var result = from obj in query where obj.Integrado == false || obj.Integrado == null select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultarPentendeIntegracao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();

            var result = from obj in query where obj.Integrado == false || obj.Integrado == null select obj;

            return result.Count();
        }

        public List<Dominio.Entidades.Abastecimento> BuscarPorAcertoDeViagem(int codigoAcertoViagem)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();
            var result = from obj in query where obj.AcertoDeViagem.Codigo == codigoAcertoViagem select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Abastecimento> BuscarPorDocumentoEntrada(int documentoEntrada)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();
            var result = from obj in query where obj.DocumentoEntrada.Codigo == documentoEntrada select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Abastecimento> BuscarPorAcertos(List<int> codigosAcertoViagem)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();
            var result = from obj in query where codigosAcertoViagem.Contains(obj.AcertoDeViagem.Codigo) select obj;
            return result.ToList();
        }

        public IList<Dominio.Entidades.Abastecimento> Consultar(int codigoEmpresa, string posto, string placaVeiculo, DateTime data, int inicioRegistros, int maximoRegistros)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.Abastecimento>();
            criteria.CreateAlias("Veiculo", "veiculo");
            criteria.Add(Restrictions.Eq("veiculo.Empresa.Codigo", codigoEmpresa));

            if (!string.IsNullOrWhiteSpace(placaVeiculo))
                criteria.Add(Restrictions.InsensitiveLike("veiculo.Placa", placaVeiculo, MatchMode.Anywhere));

            if (data != DateTime.MinValue)
            {
                criteria.Add(Restrictions.Ge("Data", data.Date));
                criteria.Add(Restrictions.Lt("Data", data.AddDays(1).Date));
            }

            if (!string.IsNullOrWhiteSpace(posto))
            {
                criteria.CreateAlias("Posto", "posto", NHibernate.SqlCommand.JoinType.LeftOuterJoin);
                criteria.Add(new OrExpression(Restrictions.InsensitiveLike("posto.Nome", posto, MatchMode.Anywhere), Restrictions.InsensitiveLike("NomePosto", posto, MatchMode.Anywhere)));
            }

            //criteria.Add(Restrictions.IsNull("AcertoDeViagem"));
            criteria.SetMaxResults(maximoRegistros);
            criteria.SetFirstResult(inicioRegistros);
            return criteria.List<Dominio.Entidades.Abastecimento>();
        }

        public int ContarConsulta(int codigoEmpresa, string posto, string placaVeiculo, DateTime data)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.Abastecimento>();
            criteria.CreateAlias("Veiculo", "veiculo");
            criteria.Add(NHibernate.Criterion.Restrictions.Eq("veiculo.Empresa.Codigo", codigoEmpresa));

            if (!string.IsNullOrWhiteSpace(placaVeiculo))
                criteria.Add(Restrictions.InsensitiveLike("veiculo.Placa", placaVeiculo, MatchMode.Anywhere));

            if (data != DateTime.MinValue)
            {
                criteria.Add(Restrictions.Ge("Data", data.Date));
                criteria.Add(Restrictions.Lt("Data", data.AddDays(1).Date));
            }

            if (!string.IsNullOrWhiteSpace(posto))
            {
                criteria.CreateAlias("Posto", "posto", NHibernate.SqlCommand.JoinType.LeftOuterJoin);
                criteria.Add(new OrExpression(Restrictions.InsensitiveLike("posto.Nome", posto, MatchMode.Anywhere), Restrictions.InsensitiveLike("NomePosto", posto, MatchMode.Anywhere)));
            }

            //criteria.Add(NHibernate.Criterion.Restrictions.IsNull("AcertoDeViagem"));
            criteria.SetProjection(NHibernate.Criterion.Projections.RowCount());
            return criteria.UniqueResult<int>();
        }

        public Dominio.Entidades.Abastecimento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public bool ContemAbastecimentoAgrupado(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();
            return query.Any(c => c.Abastecimentos.Any(a => a.Codigo == codigo));
        }

        public List<Dominio.Entidades.Abastecimento> BuscarPorCodigos(List<int> codigosAbastecimentos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();
            var result = from obj in query where codigosAbastecimentos.Contains(obj.Codigo) select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Abastecimento BuscarAbastecimento(int codigoVeiculo, string status, int km, decimal litros, decimal valorUnitario, int codigoProduto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();
            var result = from obj in query where obj.Veiculo != null && obj.Veiculo.Codigo == codigoVeiculo && obj.Status == status && obj.Kilometragem == km && obj.Litros == litros && obj.ValorUnitario == valorUnitario && obj.Produto.Codigo == codigoProduto select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Abastecimento BuscarAbastecimento(int codigoVeiculo, double posto, DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();
            query = query.Where(c => c.Data != null && c.Data.Value.Date >= dataInicial.Date && c.Data.Value.Date <= dataFinal.Date);
            if (codigoVeiculo > 0)
                query = query.Where(c => c.Veiculo.Codigo == codigoVeiculo);
            if (posto > 0)
                query = query.Where(c => c.Posto.CPF_CNPJ == posto);
            return query.FirstOrDefault();
        }

        public decimal BuscarLitrosTotalAbastecimento(int codigoMotorista, int codigoVeiculo, DateTime dataInicialAbastecimento, DateTime dataFinalAbastecimento, TipoAbastecimento tipoAbastecimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();
            var result = from obj in query where obj.Veiculo != null && obj.Veiculo.Codigo == codigoVeiculo && obj.Data.Value.Date >= dataInicialAbastecimento.Date && obj.Data.Value.Date <= dataFinalAbastecimento.Date select obj;
            result = result.Where(o => o.TipoAbastecimento == tipoAbastecimento);

            if (codigoMotorista > 0)
                result = result.Where(obj => obj.Motorista != null && obj.Motorista.Codigo == codigoMotorista);

            return result.Sum(c => (decimal?)c.Litros) ?? 0m;
        }

        public Dominio.Entidades.Abastecimento BuscarPrimeiroAbastecimento(int codigoMotorista, int codigoVeiculo, DateTime dataAbastecimento, int codigoAbastecimento, TipoAbastecimento tipoAbastecimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();
            var result = from obj in query where obj.Veiculo != null && obj.Veiculo.Codigo == codigoVeiculo && obj.Data.Value.Date >= dataAbastecimento.Date select obj;

            result = result.Where(o => o.TipoAbastecimento == tipoAbastecimento);

            if (codigoMotorista > 0)
                result = result.Where(obj => obj.Motorista != null && obj.Motorista.Codigo == codigoMotorista);

            if (codigoAbastecimento > 0)
                result = result.Where(obj => obj.Codigo != codigoAbastecimento);
            if (result.Count() > 0)
                return result.OrderBy("Data asc, Kilometragem asc").FirstOrDefault();
            else
                return null;
        }

        public decimal BuscarPrimeiroKMAbastecimento(int codigoMotorista, int codigoVeiculo, DateTime dataAbastecimento, int codigoAbastecimento, TipoAbastecimento tipoAbastecimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();
            var result = from obj in query where obj.Veiculo != null && obj.Veiculo.Codigo == codigoVeiculo && obj.Data.Value.Date <= dataAbastecimento.Date select obj;

            result = result.Where(o => o.TipoAbastecimento == tipoAbastecimento);

            if (codigoMotorista > 0)
                result = result.Where(obj => obj.Motorista != null && obj.Motorista.Codigo == codigoMotorista);

            if (codigoAbastecimento > 0)
                result = result.Where(obj => obj.Codigo != codigoAbastecimento);
            if (result.Count() > 0)
                return result.OrderBy("Data desc, Kilometragem desc").FirstOrDefault().Kilometragem;
            else
                return 0m;
        }

        public List<Dominio.Entidades.Abastecimento> BuscarAbastecimentos(decimal kmInicial, decimal kmFinal, int codigoMotorista, int codigoVeiculo, TipoAbastecimento tipoAbastecimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();
            var result = from obj in query where obj.Veiculo != null && obj.Veiculo.Codigo == codigoVeiculo select obj;

            result = result.Where(o => o.TipoAbastecimento == tipoAbastecimento);

            if (codigoMotorista > 0)
                result = result.Where(obj => obj.Motorista != null && obj.Motorista.Codigo == codigoMotorista);

            if (kmInicial > 0)
                result = result.Where(obj => obj.Kilometragem >= kmInicial);

            if (kmFinal > 0)
                result = result.Where(obj => obj.Kilometragem <= kmFinal);

            return result.ToList();
        }

        public bool VeiculoPossuiAbastecimentoEmAberto(int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();
            var result = from obj in query where obj.Veiculo != null && obj.Veiculo.Codigo == codigoVeiculo && obj.Status == "A" select obj;

            return result.Any();
        }

        public decimal BuscarUltimoKMAbastecimentoPorData(int codigoVeiculo, DateTime dataAbastecimento, int codigoAbastecimento, TipoAbastecimento tipoAbastecimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();
            var result = from obj in query where obj.Veiculo != null && obj.Veiculo.Codigo == codigoVeiculo && obj.Data.Value.Date <= dataAbastecimento.Date select obj;

            result = result.Where(o => o.TipoAbastecimento == tipoAbastecimento);

            if (codigoAbastecimento > 0)
                result = result.Where(obj => obj.Codigo != codigoAbastecimento);
            if (result.Count() > 0)
                return result.OrderBy("Data desc, Kilometragem desc").FirstOrDefault().Kilometragem;
            else
                return 0m;
        }

        public decimal BuscarUltimoKMAbastecimento(int codigoVeiculo, DateTime dataAbastecimento, int codigoAbastecimento, TipoAbastecimento tipoAbastecimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();
            var result = from obj in query where obj.Veiculo != null && obj.Veiculo.Codigo == codigoVeiculo && obj.Data.Value <= dataAbastecimento select obj;

            result = result.Where(o => o.TipoAbastecimento == tipoAbastecimento);

            if (codigoAbastecimento > 0)
                result = result.Where(obj => obj.Codigo != codigoAbastecimento);
            if (result.Count() > 0)
                return result.OrderBy("Data desc, Kilometragem desc").FirstOrDefault().Kilometragem;
            else
                return 0m;
        }

        public Dominio.Entidades.Abastecimento BuscarUltimoAbastecimentoSemTipo(int codigoVeiculo, DateTime dataAbastecimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();
            var result = from obj in query where obj.Veiculo != null && obj.Veiculo.Codigo == codigoVeiculo && obj.Situacao == "F" select obj;

            if (dataAbastecimento != DateTime.MinValue)
                result = result.Where(o => o.Data.Value <= dataAbastecimento);

            if (result.Count() > 0)
                return result.OrderBy("Data desc, Kilometragem desc").FirstOrDefault();
            else
                return null;
        }

        public Dominio.Entidades.Abastecimento BuscarAbastecimentoPosterior(int codigoVeiculo, DateTime dataAbastecimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();
            var result = from obj in query where obj.Veiculo != null && obj.Veiculo.Codigo == codigoVeiculo && obj.Situacao == "F" select obj;

            if (dataAbastecimento != DateTime.MinValue)
                result = result.Where(o => o.Data.Value >= dataAbastecimento);

            if (result.Count() > 0)
                return result.OrderBy("Data asc, Kilometragem asc").FirstOrDefault();
            else
                return null;
        }

        public int BuscarUltimoHorimetroAbastecimento(int codigoEquipamento, DateTime dataAbastecimento, int codigoAbastecimento, TipoAbastecimento tipoAbastecimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();
            var result = from obj in query where obj.Equipamento.Codigo == codigoEquipamento && obj.Data.Value <= dataAbastecimento select obj;

            result = result.Where(o => o.TipoAbastecimento == tipoAbastecimento);

            if (codigoAbastecimento > 0)
                result = result.Where(obj => obj.Codigo != codigoAbastecimento);
            if (result.Count() > 0)
                return result.OrderBy("Data desc, Horimetro desc").FirstOrDefault().Horimetro;
            else
                return 0;
        }

        public List<Dominio.Entidades.Abastecimento> BuscarPorListaDeAcertosDeViagens(List<int> listaCodigosAcertoViagem)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();
            var result = from obj in query where listaCodigosAcertoViagem.Contains(obj.AcertoDeViagem.Codigo) select obj;
            return result.ToList();
        }

        public IList<Dominio.Entidades.Abastecimento> Relatorio(int codigoEmpresa, int codigoVeiculo, int codigoModeloVeiculo, DateTime dataInicial, DateTime dataFinal, double cpfCnpjCliente, string pagamento, string nomePosto)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.Abastecimento>();
            criteria.CreateAlias("Veiculo", "veiculo");
            criteria.CreateAlias("AcertoDeViagem", "acertoDeViagem", NHibernate.SqlCommand.JoinType.LeftOuterJoin);

            criteria.Add(Restrictions.Eq("veiculo.Empresa.Codigo", codigoEmpresa));
            criteria.Add(Restrictions.Eq("Status", "A"));
            criteria.Add(Restrictions.Or(Restrictions.IsNull("AcertoDeViagem"), Restrictions.And(Restrictions.Eq("acertoDeViagem.Situacao", "F"), Restrictions.Eq("acertoDeViagem.Status", "A"))));

            if (!string.IsNullOrWhiteSpace(nomePosto))
                criteria.Add(Restrictions.Like("NomePosto", string.Concat("%", nomePosto, "%")));

            if (!string.IsNullOrWhiteSpace(pagamento))
                criteria.Add(Restrictions.Eq("Pago", pagamento.Equals("1")));

            if (codigoVeiculo > 0)
                criteria.Add(Restrictions.Eq("veiculo.Codigo", codigoVeiculo));
            if (codigoModeloVeiculo > 0)
                criteria.Add(Restrictions.Eq("veiculo.Modelo.Codigo", codigoModeloVeiculo));
            if (dataInicial != DateTime.MinValue)
                criteria.Add(Restrictions.Ge("Data", dataInicial.Date));
            if (dataFinal != DateTime.MinValue)
                criteria.Add(Restrictions.Lt("Data", dataFinal.AddDays(1).Date));
            if (cpfCnpjCliente > 0)
                criteria.Add(Restrictions.Eq("Posto.CPF_CNPJ", cpfCnpjCliente));
            return criteria.List<Dominio.Entidades.Abastecimento>();
        }

        public bool ContemAbastecimentoPorChave(string chaveNotaFiscal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();

            query = query.Where(c => c.ChaveNotaFiscal != "" && c.ChaveNotaFiscal == chaveNotaFiscal);

            return query.Any();
        }

        public Task<bool> ContemAbastecimentoPorChaveAsync(string chaveNotaFiscal, CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();

            query = query.Where(c => c.ChaveNotaFiscal != "" && c.ChaveNotaFiscal == chaveNotaFiscal);

            return query.AnyAsync(cancellationToken);
        }

        public bool ContemAbastecimentoPorDocumentoDestinado(long codigoDocumentoDestinado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();

            query = query.Where(c => c.DocumentoDestinadoEmpresa.Codigo == codigoDocumentoDestinado);

            return query.Any();
        }

        public List<int> ConsultarSeExisteAbastecimentoPendente(int codigoEmpresa, DateTime dataFechamento, string[] situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();

            query = query.Where(o => (o.Data == null || o.Data.Value.Date <= dataFechamento.Date) && situacao.Contains(o.Situacao));

            if (codigoEmpresa > 0)
                query = query.Where(c => c.Empresa.Codigo == codigoEmpresa);

            return query.Select(o => o.Codigo).ToList();
        }

        public bool VerificarSeJaExisteAbastecimentoImportacaoWS(string numeroDocumento, string codigoProduto, double codigoPosto, int codigoVeiculo, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();

            int codProduto = codigoProduto.ToInt();

            query = query.Where(o => (o.Documento == numeroDocumento &&
                                      o.Produto.Codigo == codProduto &&
                                      o.Posto.CPF_CNPJ == codigoPosto &&
                                      o.Veiculo.Codigo == codigoVeiculo &&
                                      o.Data.Value.Date == data.Date));

            return query.Any();
        }

        public List<int> BuscarCodigosPorDataAlteracao(DateTime dataUltimoProcessamento, DateTime dataProcessamentoAtual)
        {
            IQueryable<Dominio.Entidades.Abastecimento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();

            query = query.Where(o => o.DataAlteracao > dataUltimoProcessamento && o.DataAlteracao <= dataProcessamentoAtual);

            return query.Select(o => o.Codigo).ToList();
        }

        public List<Dominio.ObjetosDeValor.Relatorios.RelatorioAbastecimentoAgrupado> ObterDadosSumarizadosPorPeriodo(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();

            var result = from obj in query
                         where obj.Status.Equals("A") && obj.Veiculo != null && obj.Veiculo.Empresa.Codigo == codigoEmpresa && obj.Data.Value.Date >= dataInicial.Date && obj.Data.Value.Date <= dataFinal.Date
                         select obj;

            if (codigoVeiculo > 0)
                result = result.Where(o => o.Veiculo != null && o.Veiculo.Codigo == codigoVeiculo);

            var agrupamento = result.GroupBy(o => new { o.Data.Value.Month, o.Data.Value.Year }).Select(o => new Dominio.ObjetosDeValor.Relatorios.RelatorioAbastecimentoAgrupado()
            {
                Ano = o.Key.Year,
                Mes = o.Key.Month,
                LitrosGastos = o.Sum(obj => obj.Litros),
                QuilometrosRodados = o.Sum(obj => obj.Kilometragem - obj.KilometragemAnterior),
                ValorMedioPagoPorLitro = o.Average(obj => obj.ValorUnitario)
            });

            return agrupamento.ToList();
        }

        public List<Dominio.Entidades.Abastecimento> Consulta(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaAbastecimento filtrosPesquisa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            IQueryable<Dominio.Entidades.Abastecimento> result = Consulta(filtrosPesquisa);

            result
                .Fetch(o => o.Veiculo)
                .Fetch(o => o.Motorista)
                .Fetch(o => o.Equipamento)
                .Fetch(o => o.Posto)
                .Fetch(o => o.Produto);

            return ObterLista(result, propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros);
        }

        public int ContaConsulta(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaAbastecimento filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Abastecimento> result = Consulta(filtrosPesquisa);

            return result.Count();
        }

        public bool ContemAbastecimentoDataHoraVeiculo(int codigoVeiculo, DateTime? data, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento, int codigoAbastecimento = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();
            var result = from obj in query
                         where
                         obj.Veiculo != null &&
                         obj.Veiculo.Codigo == codigoVeiculo
                         select obj;

            if (codigoAbastecimento > 0)
                result = result.Where(obj => obj.Codigo != codigoAbastecimento);

            if (data.HasValue)
                result = result.Where(obj => obj.Data == data);

            result = result.Where(o => o.TipoAbastecimento == tipoAbastecimento);

            return result.Count() > 0;
        }
 
        public bool ContemAbastecimento(int codigoVeiculo, decimal km, string numeroDocumento, decimal qtdLitros, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento, int codigoAbastecimento = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();
            var result = from obj in query
                         where
                         obj.Veiculo != null &&
                         obj.Veiculo.Codigo == codigoVeiculo &&
                         obj.Kilometragem >= km - 5 && obj.Kilometragem <= km + 5 &&
                         obj.Litros == qtdLitros
                         select obj;

            if (codigoAbastecimento > 0)
                result = result.Where(obj => obj.Codigo != codigoAbastecimento);

            result = result.Where(o => o.TipoAbastecimento == tipoAbastecimento);

            return result.Count() > 0;
        }

        public bool ContemAbastecimentoEquipamento(int codigoEquipamento, decimal km, int horimetro, string numeroDocumento, decimal qtdLitros, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento, int codigoAbastecimento = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();
            var result = from obj in query
                         where
                         obj.Equipamento.Codigo == codigoEquipamento &&
                         obj.Horimetro >= horimetro - 5 && obj.Horimetro <= horimetro + 5 &&
                         obj.Kilometragem >= km - 5 && obj.Kilometragem <= km + 5 &&
                         obj.Litros == qtdLitros
                         select obj;

            if (codigoAbastecimento > 0)
                result = result.Where(obj => obj.Codigo != codigoAbastecimento);

            result = result.Where(o => o.TipoAbastecimento == tipoAbastecimento);

            return result.Count() > 0;
        }

        public bool ContemAbastecimentoKMMaior(int codigoVeiculo, decimal km, DateTime data, int codigoEquipamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento, int codigoAbastecimento = 0)
        {
            if (codigoEquipamento > 0 && km == 0)
                return false;

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();
            var result = from obj in query
                         where
                         obj.Veiculo != null &&
                         obj.Veiculo.Codigo == codigoVeiculo &&
                         obj.Kilometragem > km &&
                         obj.Data < data
                         select obj;

            if (codigoAbastecimento > 0)
                result = result.Where(obj => obj.Codigo != codigoAbastecimento);

            result = result.Where(o => o.TipoAbastecimento == tipoAbastecimento);

            return result.Count() > 0;
        }

        public List<Dominio.Entidades.Abastecimento> BuscarPorDocumentoAgregado(int codigoVeiculo, DateTime dataInicial, DateTime dataFinal, double cnpjAgregado, int codigoPagamentoAgregado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();

            if (codigoVeiculo > 0)
                query = query.Where(obj => obj.Veiculo.Codigo == codigoVeiculo);

            if (cnpjAgregado > 0)
                query = query.Where(obj => obj.Veiculo.Proprietario.CPF_CNPJ == cnpjAgregado);

            if (dataInicial != DateTime.MinValue && dataFinal != DateTime.MinValue)
                query = query.Where(obj => obj.Data.Value.Date >= dataInicial && obj.Data.Value.Date <= dataFinal);

            var queryPagamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAbastecimento>();

            if (codigoPagamentoAgregado > 0)
                queryPagamento = queryPagamento.Where(obj => obj.PagamentoAgregado.Codigo != codigoPagamentoAgregado && obj.PagamentoAgregado.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado.Rejeitada && obj.PagamentoAgregado.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado.Cancelado);

            if (queryPagamento.Count() > 0)
                query = query.Where(obj => !queryPagamento.Any(c => c.Abastecimento == obj));//> !queryPagamento.Select(o => o.Abastecimento).Contains(obj));

            return query.Distinct().ToList();
        }

        public bool ContemAbastecimentoEquipamentoHorimetroMaior(int codigoEquipamento, int horimetro, DateTime data, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento, int codigoAbastecimento = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();
            var result = from obj in query
                         where
                         obj.Equipamento.Codigo == codigoEquipamento &&
                         obj.Horimetro > horimetro &&
                         obj.Data < data
                         select obj;

            if (codigoAbastecimento > 0)
                result = result.Where(obj => obj.Codigo != codigoAbastecimento);

            result = result.Where(o => o.TipoAbastecimento == tipoAbastecimento);

            return result.Count() > 0;
        }

        public bool ContemAbastecimentoPorNota(int codigoVeiculo, decimal km, string numeroDocumento, decimal qtdLitros, string numeroNota, int codigoAbastecimento = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();
            var result = from obj in query
                         where
                         obj.Veiculo != null &&
                         obj.Veiculo.Codigo == codigoVeiculo &&
                         obj.Kilometragem >= km - 5 && obj.Kilometragem <= km + 5 &&
                         obj.Litros == qtdLitros &&
                         obj.Documento == numeroNota
                         select obj;

            if (codigoAbastecimento > 0)
                result = result.Where(obj => obj.Codigo != codigoAbastecimento);

            return result.Count() > 0;
        }

        public Dominio.Entidades.Abastecimento BuscarUltimoAbastecimento(int veiculo, int equipamento, int codigoAbastecimento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento, decimal km, int horimetro)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();

            if (veiculo > 0)
                query = query.Where(o => o.Veiculo.Codigo == veiculo);
            else if (equipamento > 0)
                query = query.Where(o => o.Equipamento.Codigo == equipamento);
            if (codigoAbastecimento > 0)
                query = query.Where(o => o.Codigo != codigoAbastecimento);
            if (km > 0)
                query = query.Where(o => o.Kilometragem <= km && o.Kilometragem > 0);
            if (horimetro > 0)
                query = query.Where(o => o.Horimetro <= horimetro && o.Horimetro > 0);

            query = query.Where(o => o.TipoAbastecimento == tipoAbastecimento);

            return query
                .OrderByDescending(o => o.Data)
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Abastecimento> BuscarPorAbastecimentoAgregado(int codigoVeiculo, DateTime dataInicial, DateTime dataFinal, int codigoMotorista, double cnpjAgregado, int codigoPagamentoAgregado, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();
            var result = from obj in query select obj;

            if (codigoMotorista > 0)
                result = result.Where(obj => obj.Motorista.Codigo == codigoMotorista);
            if (cnpjAgregado > 0)
                result = result.Where(obj => obj.Motorista.ClienteTerceiro.CPF_CNPJ == cnpjAgregado);
            if (dataFinal != DateTime.MinValue && dataInicial == DateTime.MinValue)
                result = result.Where(obj => obj.Data.Value.Date <= dataFinal);
            if (dataFinal == DateTime.MinValue && dataInicial != DateTime.MinValue)
                result = result.Where(obj => obj.Data.Value.Date >= dataInicial);
            if (dataFinal != DateTime.MinValue && dataInicial != DateTime.MinValue)
                result = result.Where(obj => obj.Data.Value.Date >= dataInicial && dataInicial == DateTime.MinValue);

            var queryPagamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAbastecimento>();
            var resultPagamento = from obj in queryPagamento select obj;
            if (codigoPagamentoAgregado > 0)
                resultPagamento = resultPagamento.Where(obj => obj.PagamentoAgregado.Codigo != codigoPagamentoAgregado && obj.PagamentoAgregado.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado.Rejeitada);
            if (resultPagamento.Count() > 0)
                result = result.Where(obj => !resultPagamento.Any(c => c.Abastecimento == obj));// !resultPagamento.Select(o => o.Abastecimento).Contains(obj));

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).Distinct().ToList();
        }

        public int ContarBuscarPorAbastecimentoAgregado(int codigoVeiculo, DateTime dataInicial, DateTime dataFinal, int codigoMotorista, double cnpjAgregado, int codigoPagamentoAgregado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();
            var result = from obj in query select obj;

            if (codigoMotorista > 0)
                result = result.Where(obj => obj.Motorista.Codigo == codigoMotorista);
            if (cnpjAgregado > 0)
                result = result.Where(obj => obj.Motorista.ClienteTerceiro.CPF_CNPJ == cnpjAgregado);
            if (dataFinal != DateTime.MinValue && dataInicial == DateTime.MinValue)
                result = result.Where(obj => obj.Data.Value.Date <= dataFinal);
            if (dataFinal == DateTime.MinValue && dataInicial != DateTime.MinValue)
                result = result.Where(obj => obj.Data.Value.Date >= dataInicial);
            if (dataFinal != DateTime.MinValue && dataInicial != DateTime.MinValue)
                result = result.Where(obj => obj.Data.Value.Date >= dataInicial && dataInicial == DateTime.MinValue);

            var queryPagamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAbastecimento>();
            var resultPagamento = from obj in queryPagamento select obj;
            if (codigoPagamentoAgregado > 0)
                resultPagamento = resultPagamento.Where(obj => obj.PagamentoAgregado.Codigo != codigoPagamentoAgregado && obj.PagamentoAgregado.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado.Rejeitada);
            if (resultPagamento.Count() > 0)
                result = result.Where(obj => !resultPagamento.Any(c => c.Abastecimento == obj));// !resultPagamento.Select(o => o.Abastecimento).Contains(obj));

            return result.Count();
        }

        public List<Dominio.Entidades.Abastecimento> BuscarAbastecimentoPorVeiculoDataSemAcerto(List<int> codigosVeiculos, DateTime dataInicial, DateTime dataFinal)
        {
            var queryAbastecimento = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();
            var resultAbastecimento = from obj in queryAbastecimento where obj.Situacao != "I" && obj.Situacao != "G" select obj;

            var queryAcerto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento>();
            var resultAcerto = from obj in queryAcerto where obj.AcertoViagem.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.EmAntamento || obj.AcertoViagem.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.Fechado select obj;

            if (codigosVeiculos != null && codigosVeiculos.Count > 0)
            {
                resultAbastecimento = resultAbastecimento.Where(obj => obj.Data <= dataFinal);
                resultAbastecimento = resultAbastecimento.Where(obj => codigosVeiculos.Contains(obj.Veiculo.Codigo));// (from p in acertoViagem.Veiculos select p.Veiculo).Contains(obj.Veiculo));                                
            }

            resultAbastecimento = resultAbastecimento.Where(obj => !resultAcerto.Any(c => c.Abastecimento == obj));// !(from p in resultAcerto select p.Abastecimento).Contains(obj));

            return resultAbastecimento.Timeout(300).ToList();
        }

        public List<Dominio.Entidades.Abastecimento> BuscarAbastecimentoPorVeiculoDataSemAcerto(int codigoVeiculo, DateTime dataInicial, DateTime dataFinal)
        {
            var queryAbastecimento = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();
            var resultAbastecimento = from obj in queryAbastecimento where obj.Situacao != "I" && obj.Situacao != "G" select obj;

            var queryAcerto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento>();
            var resultAcerto = from obj in queryAcerto where obj.AcertoViagem.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.EmAntamento || obj.AcertoViagem.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.Fechado select obj;

            resultAbastecimento = resultAbastecimento.Where(obj => obj.Data <= dataFinal);
            resultAbastecimento = resultAbastecimento.Where(obj => obj.Veiculo != null && obj.Veiculo.Codigo == codigoVeiculo);

            resultAbastecimento = resultAbastecimento.Where(obj => !resultAcerto.Any(c => c.Abastecimento == obj));// !(from p in resultAcerto select p.Abastecimento).Contains(obj));

            return resultAbastecimento.ToList();

        }

        public bool ValidaNovoAbastecimento(decimal km, DateTime? data, int codigoVeiculo)
        {
            var queryAbastecimento = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();
            var resultAbastecimento = from obj in queryAbastecimento where (obj.Data >= data || obj.Kilometragem >= km) && obj.Situacao.Equals("F") && obj.Veiculo != null && obj.Veiculo.Codigo == codigoVeiculo select obj;

            return resultAbastecimento.Count() == 0;
        }

        public bool AbastecimentoDuplicado(DateTime dataAbastecimento, string numeroDocumento, double cnpjFornecedor, int codigoProduto, decimal litro, decimal valorUnitario)
        {
            var queryAbastecimento = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();

            queryAbastecimento = queryAbastecimento.Where(obj => obj.Data == dataAbastecimento
                && obj.Documento == numeroDocumento
                && obj.Posto.CPF_CNPJ == cnpjFornecedor
                && obj.Produto.Codigo == codigoProduto
                && obj.Litros == litro
                && obj.ValorUnitario == valorUnitario);

            return queryAbastecimento.Any();
        }

        public Task<bool> AbastecimentoDuplicadoAsync(DateTime dataAbastecimento, string numeroDocumento, double cnpjFornecedor, int codigoProduto, decimal litro, decimal valorUnitario, CancellationToken cancellationToken)
        {
            var queryAbastecimento = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();

            queryAbastecimento = queryAbastecimento.Where(obj => obj.Data == dataAbastecimento
                && obj.Documento == numeroDocumento
                && obj.Posto.CPF_CNPJ == cnpjFornecedor
                && obj.Produto.Codigo == codigoProduto
                && obj.Litros == litro
                && obj.ValorUnitario == valorUnitario);

            return queryAbastecimento.AnyAsync(cancellationToken);
        }

        public bool AbastecimentoDuplicado(DateTime dataAbastecimento, int codigoVeiculo, string numeroDocumento, decimal km)
        {
            var queryAbastecimento = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();
            var resultAbastecimento = from obj in queryAbastecimento
                                      where obj.Veiculo != null
                                        && obj.Kilometragem > 0
                                        && obj.Kilometragem == km
                                        && obj.Veiculo.Codigo == codigoVeiculo
                                        && obj.Data == dataAbastecimento
                                        && obj.Documento == numeroDocumento
                                      select obj;

            return resultAbastecimento.Any();
        }

        public bool AbastecimentoDuplicado(Dominio.Entidades.Abastecimento abastecimento)
        {
            var queryAbastecimento = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();
            var resultAbastecimento = from obj in queryAbastecimento
                                      where obj.Codigo != abastecimento.Codigo
                                        && obj.Veiculo != null
                                        && obj.Kilometragem > 0
                                        && obj.Kilometragem == abastecimento.Kilometragem
                                        && obj.Veiculo.Codigo == abastecimento.Veiculo.Codigo
                                        && obj.Litros == abastecimento.Litros
                                        && obj.TipoAbastecimento == abastecimento.TipoAbastecimento
                                      select obj;

            return resultAbastecimento.Any();
        }

        public bool AbastecimentoDuplicadoPorRequisicao(Dominio.Entidades.Abastecimento abastecimento)
        {
            var queryAbastecimento = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();

            var resultAbastecimento = queryAbastecimento
                .Where(obj => obj.Veiculo != null
                           && obj.Veiculo.Codigo == abastecimento.Veiculo.Codigo
                           && obj.TipoAbastecimento == abastecimento.TipoAbastecimento
                           && obj.Posto.CPF_CNPJ == abastecimento.Posto.CPF_CNPJ
                           && obj.Litros == abastecimento.Litros)
                .ToList();

            if (resultAbastecimento.Count > 0)
                resultAbastecimento = resultAbastecimento.ToList();

            return resultAbastecimento.Any();
        }

        public bool AbastecimentoDuplicadoIntegracao(string codigoIntegracao, DateTime dataAbastecimento, decimal litros)
        {
            var queryAbastecimento = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();

            var resultAbastecimento = queryAbastecimento
                .Where(obj => obj.CodigoIntegracao == codigoIntegracao
                           && obj.Data == dataAbastecimento
                           && obj.Litros == litros)
                .ToList();

            return resultAbastecimento.Any();
        }

        public Dominio.Entidades.Abastecimento BuscarPorCodigoAcerto(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento>();
            var result = from obj in query where obj.Codigo == codigo select obj.Abastecimento;
            return result.FirstOrDefault();
        }

        public bool AbastecimentoTemAcerto(int codigoAbastecimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento>();
            var result = from obj in query where obj.Abastecimento.Codigo == codigoAbastecimento && obj.AcertoViagem.Situacao != SituacaoAcertoViagem.Cancelado select obj;
            return result.Count() > 0;
        }

        public bool AbastecimentoComTituloQuitado(int codigoAbastecimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query where obj.Abastecimento.Codigo == codigoAbastecimento && obj.StatusTitulo == StatusTitulo.Quitada select obj;
            return result.Any();
        }

        public List<Dominio.Entidades.Abastecimento> ConsultarParaFechamento(double codigoPosto, int codigoVeiculo, DateTime dataInicio, DateTime dataFim, string situacao, int codigoEmpresa, int codigoEquipamento, List<int> codigosEmpresa, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            IQueryable<Dominio.Entidades.Abastecimento> result = ConsultarParaFechamento(codigoPosto, codigoVeiculo, dataInicio, dataFim, situacao, codigoEmpresa, codigoEquipamento, codigosEmpresa);

            return result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultarParaFechamento(double codigoPosto, int codigoVeiculo, DateTime dataInicio, DateTime dataFim, string situacao, int codigoEmpresa, int codigoEquipamento, List<int> codigosEmpresa)
        {
            IQueryable<Dominio.Entidades.Abastecimento> result = ConsultarParaFechamento(codigoPosto, codigoVeiculo, dataInicio, dataFim, situacao, codigoEmpresa, codigoEquipamento, codigosEmpresa);

            return result.Count();
        }

        public List<Dominio.Entidades.Abastecimento> ConsultarReprocessarAbastecimento(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaReprocessarAbastecimento filtrosPesquisa, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            IQueryable<Dominio.Entidades.Abastecimento> result = ConsultarReprocessarAbastecimentoFrota(filtrosPesquisa);

            if (maximoRegistros > 0)
                return result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
            else
                return result.ToList();
        }

        public int ContarConsultarReprocessarAbastecimento(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaReprocessarAbastecimento filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Abastecimento> result = ConsultarReprocessarAbastecimentoFrota(filtrosPesquisa);

            return result.Count();
        }

        public void RemoverVinculosPorCodigo(int codigoAbastecimento)
        {
            UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_TMS_DOCUMENTO_ENTRADA_ITEM_ABASTECIMENTO WHERE ABA_CODIGO = :codigoAbastecimento").SetParameter("codigoAbastecimento", codigoAbastecimento).ExecuteUpdate();
            UnitOfWork.Sessao.CreateSQLQuery(@"DELETE A FROM T_ACERTO_ABASTECIMENTO A JOIN T_ACERTO_DE_VIAGEM AA ON AA.ACV_CODIGO = A.ACV_CODIGO WHERE AA.ACV_SITUACAO = 3 AND A.ABA_CODIGO = :codigoAbastecimento").SetParameter("codigoAbastecimento", codigoAbastecimento).ExecuteUpdate();
            UnitOfWork.Sessao.CreateSQLQuery(@"DELETE CA FROM T_ABASTECIMENTO A JOIN T_COMISSAO_FUNCIONARIO_MOTORISTA_ABASTECIMENTO CA ON CA.ABA_CODIGO = A.ABA_CODIGO JOIN T_COMISSAO_FUNCIONARIO_MOTORISTA CM ON CM.CFM_CODIGO = CA.CFM_CODIGO JOIN T_COMISSAO_FUNCIONARIO C ON C.CMF_CODIGO = CM.CMF_CODIGO WHERE A.ABA_CODIGO = :codigoAbastecimento").SetParameter("codigoAbastecimento", codigoAbastecimento).ExecuteUpdate();
            UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_VEICULO_RECEITA_DESPESA WHERE ABA_CODIGO = :codigoAbastecimento").SetParameter("codigoAbastecimento", codigoAbastecimento).ExecuteUpdate();
            UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_RETORNO_CONSULTA_ABASTECIMENTO_ANGELLIRA WHERE ABA_CODIGO = :codigoAbastecimento").SetParameter("codigoAbastecimento", codigoAbastecimento).ExecuteUpdate();
            UnitOfWork.Sessao.CreateSQLQuery("UPDATE T_TITULO SET ABA_CODIGO = NULL WHERE ABA_CODIGO = :codigoAbastecimento").SetParameter("codigoAbastecimento", codigoAbastecimento).ExecuteUpdate();
        }

        public void CancelarAbastecimentosPorPeriodo(int dias)
        {
            UnitOfWork.Sessao.CreateSQLQuery(@"DELETE ANX 
                                               FROM T_ABASTECIMENTO_ANEXO ANX
                                               INNER JOIN T_ABASTECIMENTO ABA ON ANX.ABA_CODIGO = ABA.ABA_CODIGO
                                               WHERE ABA.ABA_SITUACAO = 'A' AND ABA.ABA_DATA < DATEADD(DAY, :dias, GETDATE());
        
                                               DELETE FROM T_ABASTECIMENTO 
                                               WHERE ABA_SITUACAO = 'A' 
                                               AND ABA_DATA < DATEADD(DAY, :dias, GETDATE())")
                .SetParameter("dias", dias)
                .ExecuteUpdate();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Abastecimento> ConsultarReprocessarAbastecimentoFrota(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaReprocessarAbastecimento filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.SituacaoAbastecimento))
                query = query.Where(obj => (obj.Situacao.Equals("I") || obj.Situacao.Equals("A")) && obj.Situacao.Equals(filtrosPesquisa.SituacaoAbastecimento));

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                query = query.Where(obj => obj.Data.Value.Date >= filtrosPesquisa.DataInicial.Date);

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                query = query.Where(obj => obj.Data.Value.Date <= filtrosPesquisa.DataFinal.Date);

            if (filtrosPesquisa.CodigosVeiculos?.Count > 0)
                query = query.Where(obj => obj.Veiculo != null && filtrosPesquisa.CodigosVeiculos.Contains(obj.Veiculo.Codigo));

            if (filtrosPesquisa.CodigosEquipamentos?.Count > 0)
                query = query.Where(obj => obj.Equipamento != null && filtrosPesquisa.CodigosEquipamentos.Contains(obj.Equipamento.Codigo));

            return query;
        }

        private IQueryable<Dominio.Entidades.Abastecimento> Consulta(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaAbastecimento filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();

            var result = from obj in query select obj;

            if (filtrosPesquisa.Produto > 0)
                result = result.Where(obj => obj.Produto.Codigo == filtrosPesquisa.Produto);

            if (filtrosPesquisa.DataInicial.HasValue)
                result = result.Where(obj => obj.Data.Value.Date >= filtrosPesquisa.DataInicial.Value.Date);

            if (filtrosPesquisa.DataFinal.HasValue)
                result = result.Where(obj => obj.Data.Value.Date <= filtrosPesquisa.DataFinal.Value.Date);

            if (filtrosPesquisa.Veiculo > 0)
                result = result.Where(obj => obj.Veiculo != null && obj.Veiculo.Codigo == filtrosPesquisa.Veiculo);

            if (filtrosPesquisa.Equipamento > 0)
                result = result.Where(obj => obj.Equipamento.Codigo == filtrosPesquisa.Equipamento);

            if (filtrosPesquisa.Motorista > 0)
                result = result.Where(obj => obj.Motorista.Codigo == filtrosPesquisa.Motorista);

            if (filtrosPesquisa.ClientePosto > 0)
                result = result.Where(obj => obj.Posto.CPF_CNPJ == filtrosPesquisa.ClientePosto);

            if (filtrosPesquisa.CodigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == filtrosPesquisa.CodigoEmpresa);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Situacao) && !filtrosPesquisa.Situacao.Equals("T"))
                result = result.Where(obj => obj.Situacao.Equals(filtrosPesquisa.Situacao));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Documento))
                result = result.Where(obj => obj.Documento.Contains(filtrosPesquisa.Documento));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Placa))
                result = result.Where(obj => obj.Veiculo != null && obj.Veiculo.Placa.Contains(filtrosPesquisa.Placa));

            if (filtrosPesquisa.TipoAbastecimento > 0)
                result = result.Where(obj => obj.TipoAbastecimento == filtrosPesquisa.TipoAbastecimento);

            if (filtrosPesquisa.Quilometragem > 0)
                result = result.Where(obj => obj.Kilometragem == filtrosPesquisa.Quilometragem);

            if (filtrosPesquisa.NumeroDocumentoInicial > 0)
                result = result.Where(obj => Int32.Parse(obj.Documento) >= filtrosPesquisa.NumeroDocumentoInicial);

            if (filtrosPesquisa.NumeroDocumentoFinal > 0)
                result = result.Where(obj => Int32.Parse(obj.Documento) <= filtrosPesquisa.NumeroDocumentoFinal);
            
            if (filtrosPesquisa.CodigoAbastecimentoIgnorar > 0)
                result = result.Where(obj => obj.Codigo != filtrosPesquisa.CodigoAbastecimentoIgnorar);

            if (filtrosPesquisa.Horimetro > 0)
                result = result.Where(obj => obj.Horimetro == filtrosPesquisa.Horimetro);

            if (filtrosPesquisa.CodigoCentroResultado > 0)
                result = result.Where(obj => obj.CentroResultado.Codigo == filtrosPesquisa.CodigoCentroResultado);  

            if (filtrosPesquisa.CodigosEmpresa != null && filtrosPesquisa.CodigosEmpresa.Count > 0)
                result = result.Where(obj => obj.Veiculo.Empresas.Any(e => filtrosPesquisa.CodigosEmpresa.Contains(e.Codigo)));

            if (filtrosPesquisa.CodigoUsuarioLogado > 0)
            {
                var queryUsuario = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
                double CNPJEmpresaUsuario = queryUsuario.Where(x => x.Codigo == filtrosPesquisa.CodigoUsuarioLogado).FirstOrDefault()?.Cliente?.CPF_CNPJ ?? 0;

                result = result.Where(obj => obj.Posto.CPF_CNPJ == CNPJEmpresaUsuario && obj.Situacao == "R");
            }

            return result;
        }

        private IQueryable<Dominio.Entidades.Abastecimento> ConsultarParaFechamento(double codigoPosto, int codigoVeiculo, DateTime dataInicio, DateTime dataFim, string situacao, int codigoEmpresa, int codigoEquipamento, List<int> codigosEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();

            var result = from obj in query where  obj.FechamentoAbastecimento == null ||
                         (obj.FechamentoAbastecimento != null
                         && obj.FechamentoAbastecimento.Situacao == SituacaoFechamentoAbastecimento.Finalizado)
                         select obj;

            if (codigoPosto > 0)
                result = result.Where(obj => obj.Posto.CPF_CNPJ == codigoPosto);

            if (codigoVeiculo > 0)
                result = result.Where(obj => obj.Veiculo != null && obj.Veiculo.Codigo == codigoVeiculo);

            if (dataInicio != DateTime.MinValue)
                result = result.Where(obj => obj.Data.Value.Date >= dataInicio.Date);

            if (dataFim != DateTime.MinValue)
                result = result.Where(obj => obj.Data.Value.Date <= dataFim.Date);

            if (!string.IsNullOrWhiteSpace(situacao))
                result = result.Where(obj => obj.Situacao.Equals(situacao));

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            if (codigoEquipamento > 0)
                result = result.Where(obj => obj.Equipamento.Codigo == codigoEquipamento);

            if (codigosEmpresa != null && codigosEmpresa.Count > 0)
                result = result.Where(obj => obj.Veiculo.Empresas.Any(e => codigosEmpresa.Contains(e.Codigo)));

            return result;
        }

        #endregion

        #region Relatório de Abastecimento

        public IList<Dominio.Relatorios.Embarcador.DataSource.Frota.Abastecimento> RelatorioAbastecimento(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioAbastecimento filtrosPesquisa, string propGrupo, string dirOrdenacaoGrupo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            string query = @"SELECT A.ABA_CODIGO Codigo, 
                                    A.ABA_DOCUMENTO Documento,
                                    P.PRO_DESCRICAO Produto,
                                    C.CLI_NOME Fornecedor,
                                    V.VEI_PLACA Veiculo,
                                    V.VEI_CAPTANQUE CapacidadeTanque,
                                    A.ORC_CODIGO Requisicao,
                                    CASE
	                                    WHEN V.VEI_TIPOVEICULO = 0 THEN 'Tração'
	                                    ELSE 'Reboque'
                                    END Categoria,
                                    A.ABA_DATA Data,
                                    CASE
										WHEN P.PRO_DESCRICAO LIKE '%ARLA%' THEN
											ISNULL((SELECT TOP(1) ABA_KM FROM T_ABASTECIMENTO AA JOIN T_PRODUTO PP ON PP.PRO_CODIGO = AA.PRO_CODIGO AND PP.PRO_DESCRICAO LIKE '%ARLA%' WHERE AA.ABA_CODIGO <> A.ABA_CODIGO AND A.ABA_KM > AA.ABA_KM AND A.VEI_CODIGO = AA.VEI_CODIGO AND AA.ABA_SITUACAO <> 'G' ORDER BY ABA_KM DESC), 0)
										ELSE
											ISNULL((SELECT TOP(1) ABA_KM FROM T_ABASTECIMENTO AA JOIN T_PRODUTO PP ON PP.PRO_CODIGO = AA.PRO_CODIGO AND NOT PP.PRO_DESCRICAO LIKE '%ARLA%' WHERE AA.ABA_CODIGO <> A.ABA_CODIGO AND A.ABA_KM > AA.ABA_KM AND A.VEI_CODIGO = AA.VEI_CODIGO AND AA.ABA_SITUACAO <> 'G' ORDER BY ABA_KM DESC), 0)
									END KmAnterior,
                                    A.ABA_KM Km,
                                    A.ABA_LITROS Litros,
                                    CASE
	                                    WHEN A.ABA_SITUACAO = 'A' THEN 'Aberto'
	                                    WHEN A.ABA_SITUACAO = 'F' THEN 'Fechado'
                                        WHEN A.ABA_SITUACAO = 'G' THEN 'Agrupado'
	                                    ELSE 'Inconsistente'
                                    END Status, 
                                    ISNULL(ABA_VALOR_UN, 0) Valor, 
                                    (ISNULL(ABA_VALOR_UN, 0) * ISNULL(A.ABA_LITROS, 0)) ValorTotal,
                                    ISNULL((SELECT TOP(1) ISNULL(AV.ACV_NUMERO, 0) 
									    FROM T_ACERTO_ABASTECIMENTO BV 
									    JOIN T_ACERTO_DE_VIAGEM AV ON AV.ACV_CODIGO = BV.ACV_CODIGO AND AV.ACV_SITUACAO <> 3
									    WHERE BV.ABA_CODIGO = A.ABA_CODIGO), 0)  NumeroAcerto,  
                                    CASE
	                                    WHEN V.VEI_TIPO = 'T' THEN 'Terceiro'
	                                    ELSE 'Próprio' 
                                    END TipoPropriedade,
                                    CP.CLI_NOME Proprietario, SEG.VSE_DESCRICAO Segmento,
                                    (A.ABA_KM - CASE
										WHEN P.PRO_DESCRICAO LIKE '%ARLA%' THEN
											ISNULL((SELECT TOP(1) ABA_KM FROM T_ABASTECIMENTO AA JOIN T_PRODUTO PP ON PP.PRO_CODIGO = AA.PRO_CODIGO AND PP.PRO_DESCRICAO LIKE '%ARLA%' WHERE AA.ABA_CODIGO <> A.ABA_CODIGO AND A.ABA_KM > AA.ABA_KM AND A.VEI_CODIGO = AA.VEI_CODIGO AND AA.ABA_SITUACAO <> 'G' ORDER BY ABA_KM DESC), 0)
										ELSE
											ISNULL((SELECT TOP(1) ABA_KM FROM T_ABASTECIMENTO AA JOIN T_PRODUTO PP ON PP.PRO_CODIGO = AA.PRO_CODIGO AND NOT PP.PRO_DESCRICAO LIKE '%ARLA%' WHERE AA.ABA_CODIGO <> A.ABA_CODIGO AND A.ABA_KM > AA.ABA_KM AND A.VEI_CODIGO = AA.VEI_CODIGO AND AA.ABA_SITUACAO <> 'G' ORDER BY ABA_KM DESC), 0)
									END) KMTotal,
                                    ((A.ABA_KM - CASE
										WHEN P.PRO_DESCRICAO LIKE '%ARLA%' THEN
											ISNULL((SELECT TOP(1) ABA_KM FROM T_ABASTECIMENTO AA JOIN T_PRODUTO PP ON PP.PRO_CODIGO = AA.PRO_CODIGO AND PP.PRO_DESCRICAO LIKE '%ARLA%' WHERE AA.ABA_CODIGO <> A.ABA_CODIGO AND A.ABA_KM > AA.ABA_KM AND A.VEI_CODIGO = AA.VEI_CODIGO AND AA.ABA_SITUACAO <> 'G' ORDER BY ABA_KM DESC), 0)
										ELSE
											ISNULL((SELECT TOP(1) ABA_KM FROM T_ABASTECIMENTO AA JOIN T_PRODUTO PP ON PP.PRO_CODIGO = AA.PRO_CODIGO AND NOT PP.PRO_DESCRICAO LIKE '%ARLA%' WHERE AA.ABA_CODIGO <> A.ABA_CODIGO AND A.ABA_KM > AA.ABA_KM AND A.VEI_CODIGO = AA.VEI_CODIGO AND AA.ABA_SITUACAO <> 'G' ORDER BY ABA_KM DESC), 0)
									END) / CASE WHEN A.ABA_LITROS <= 0 THEN 1 ELSE A.ABA_LITROS END) Media, 
                                    ISNULL(V.VEI_NUMERO_FROTA, '') NumeroFrota,
                                    ISNULL(F.FUN_NOME, (SELECT TOP(1) F.FUN_NOME FROM T_ACERTO_ABASTECIMENTO BV 
                                        JOIN T_ACERTO_DE_VIAGEM AV ON AV.ACV_CODIGO = BV.ACV_CODIGO AND AV.ACV_SITUACAO <> 3
                                        JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = AV.FUN_CODIGO_MOTORISTA
                                        WHERE BV.ABA_CODIGO = A.ABA_CODIGO ORDER BY BV.ACB_CODIGO DESC)) Motorista, 
                                   Equipamento.EQP_DESCRICAO Equipamento,
                                   A.ABA_HORIMETRO Horimetro,
                                   ISNULL(ModeloVeiculo.VMO_MEDIA_PADRAO, 0) MediaPadrao,

                                  ISNULL((SELECT TOP(1) AA.ABA_HORIMETRO FROM T_ABASTECIMENTO AA 
                                              WHERE AA.EQP_CODIGO = A.EQP_CODIGO AND AA.ABA_CODIGO < A.ABA_CODIGO AND AA.ABA_SITUACAO <> 'G' ORDER BY AA.ABA_CODIGO DESC), A.ABA_HORIMETRO) HorimetroAnterior,

                                    (A.ABA_HORIMETRO - ISNULL((SELECT TOP(1) ABA_HORIMETRO FROM T_ABASTECIMENTO AA
                                            WHERE A.ABA_HORIMETRO > AA.ABA_HORIMETRO AND A.EQP_CODIGO = AA.EQP_CODIGO AND AA.ABA_SITUACAO <> 'G' ORDER BY ABA_HORIMETRO DESC), A.ABA_HORIMETRO)) HorimetroTotal,

                                    (CASE WHEN (SELECT count(*) FROM T_ABASTECIMENTO AA WHERE A.ABA_HORIMETRO > AA.ABA_HORIMETRO AND A.EQP_CODIGO = AA.EQP_CODIGO AND AA.ABA_SITUACAO <> 'G') > 0 and A.ABA_LITROS > 0 THEN
                                        CASE WHEN (A.ABA_HORIMETRO - ISNULL((SELECT TOP(1) AA.ABA_HORIMETRO FROM T_ABASTECIMENTO AA
                                                                             WHERE A.ABA_HORIMETRO > AA.ABA_HORIMETRO AND A.EQP_CODIGO = AA.EQP_CODIGO AND AA.ABA_SITUACAO <> 'G' ORDER BY AA.ABA_HORIMETRO DESC), 0)) <= 0 THEN 0
                                        ELSE (A.ABA_LITROS / (A.ABA_HORIMETRO - ISNULL((SELECT TOP(1) AA.ABA_HORIMETRO FROM T_ABASTECIMENTO AA
                                                                                        WHERE A.ABA_HORIMETRO > AA.ABA_HORIMETRO AND A.EQP_CODIGO = AA.EQP_CODIGO AND AA.ABA_SITUACAO <> 'G' ORDER BY AA.ABA_HORIMETRO DESC), 0))) END
                                    ELSE 0 END) MediaHorimetro,

                                    A.ABA_MOTIVO_INCONSISTENCIA MotivoInconsistencia,
                                    A.ABA_TIPO_RECEBIMENTO TipoRecebimento,
                                    C.CLI_CGCCPF CpfCnpjFornecedor,
                                    C.CLI_FISJUR TipoFornecedor,
                                    ISNULL(A.ABA_KM_ORIGINAL, 0) KmOriginal,
                                    ISNULL(A.ABA_HORIMETRO_ORIGINAL, 0) HorimetroOriginal,
                                    GrupoPessoa.GRP_DESCRICAO GrupoPessoa,
                                    CentroResultado.CRE_DESCRICAO CentroResultado,
				                    CONVERT(varchar, A.ABA_DATA, 103) DataSeparada,
				                    CONVERT(varchar, A.ABA_DATA, 108) HoraSeparada,
				                    C.CLI_NOMEFANTASIA FantasiaFornecedor,
                                    Localidade.UF_SIGLA UFFornecedor,
                                    ModeloVeicularCarga.MVC_DESCRICAO ModeloVeicularCarga,
                                    MotoristaAnterior.FUN_NOME MotoristaAnterior,
                                    A.ABA_KM_ANTERIOR_ALTERACAO KMAnteriorAlteracao,
                                    A.ABA_DATA_ANTERIOR DataAnterior,
                                    A.ABA_DATA_BASE_CRT DataBaseCRT, 
                                    A.ABA_MOEDA_COTACAO_BANCO_CENTRAL Moeda, 
                                    A.ABA_VALOR_MOEDA_COTACAO ValorMoeda, 
                                    A.ABA_VALOR_ORIGINAL_MOEDA_ESTRANGEIRA ValorOriginalMoeda, 
                                    PaisC.PAI_NOME Pais,
                                    LocalidadeC.LOC_DESCRICAO + ' - ' + LocalidadeC.UF_SIGLA LocalidadeFornecedor,
                                    A.ABA_OBSERVACAO Observacao,
                                    LocalAbastecimento.LAP_DESCRICAO LocalArmazenamento,
									isnull(SUBSTRING((     select ', ' + VeiculoReboque.VEI_PLACA       
										from
											T_VEICULO_CONJUNTO Reboque       
										join
											T_VEICULO VeiculoReboque 
												on VeiculoReboque.VEI_CODIGO = Reboque.VEC_CODIGO_FILHO      
										where
											Reboque.VEC_CODIGO_PAI = V.VEI_CODIGO        for XML PATH('') ),
										3,
										1000),
										'') as PlacaReboque,
									(SELECT VM.MVC_DESCRICAO FROM T_MODELO_VEICULAR_CARGA VM WHERE V.MVC_CODIGO = VM.MVC_CODIGO) ModeloVeicularTracao,
									isnull(      SUBSTRING((    SELECT
											', ' + VM.MVC_DESCRICAO    
										FROM
											T_VEICULO_CONJUNTO Reboque     
										JOIN
											T_VEICULO VeiculoReboque      
												ON VeiculoReboque.VEI_CODIGO = Reboque.VEC_CODIGO_FILHO     
										LEFT JOIN
											T_MODELO_VEICULAR_CARGA VM      
												ON VM.MVC_CODIGO = VeiculoReboque.MVC_CODIGO    
										WHERE
											Reboque.VEC_CODIGO_PAI = V.VEI_CODIGO    for XML PATH('')),
										3,
										1000),
										'') as ModeloVeicularReboque,

									SEG.VSE_DESCRICAO as SegmentoTracao,
									isnull(SUBSTRING((SELECT
											', ' + SegmentoReboque.VSE_DESCRICAO    
										FROM
											T_VEICULO_CONJUNTO Reboque
										JOIN
											T_VEICULO VeiculoReboque      
												ON VeiculoReboque.VEI_CODIGO = Reboque.VEC_CODIGO_FILHO 
										LEFT JOIN
											T_VEICULO_SEGMENTO SegmentoReboque      
												ON SegmentoReboque.VSE_CODIGO = VeiculoReboque.VSE_CODIGO    
										WHERE
											Reboque.VEC_CODIGO_PAI = V.VEI_CODIGO for XML PATH('')),
										3,
										1000), '') as SegmentoReboque
                        FROM T_ABASTECIMENTO A 
                        LEFT OUTER JOIN T_PRODUTO P ON P.PRO_CODIGO = A.PRO_CODIGO
                        LEFT OUTER JOIN T_CLIENTE C ON C.CLI_CGCCPF = A.CLI_CGCCPF
                        LEFT OUTER JOIN T_VEICULO V ON V.VEI_CODIGO = A.VEI_CODIGO 
                        LEFT OUTER JOIN T_CLIENTE CP ON CP.CLI_CGCCPF = V.VEI_PROPRIETARIO
                        LEFT OUTER JOIN T_VEICULO_SEGMENTO SEG ON SEG.VSE_CODIGO = V.VSE_CODIGO
                        LEFT OUTER JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = A.FUN_CODIGO
                        LEFT OUTER JOIN T_EQUIPAMENTO Equipamento ON Equipamento.EQP_CODIGO = A.EQP_CODIGO
                        LEFT OUTER JOIN T_VEICULO_MODELO ModeloVeiculo ON ModeloVeiculo.VMO_CODIGO = V.VMO_CODIGO
                        LEFT OUTER JOIN T_GRUPO_PESSOAS GrupoPessoa ON GrupoPessoa.GRP_CODIGO = C.GRP_CODIGO
                        LEFT OUTER JOIN T_CENTRO_RESULTADO CentroResultado ON CentroResultado.CRE_CODIGO = A.CRE_CODIGO
                        LEFT OUTER JOIN T_LOCALIDADES Localidade ON Localidade.LOC_CODIGO = C.LOC_CODIGO
                        LEFT OUTER JOIN T_MODELO_VEICULAR_CARGA ModeloVeicularCarga ON V.MVC_CODIGO = ModeloVeicularCarga.MVC_CODIGO
                        LEFT OUTER JOIN T_FUNCIONARIO MotoristaAnterior on MotoristaAnterior.FUN_CODIGO = A.FUN_CODIGO_ANTERIOR
                        LEFT OUTER JOIN T_LOCALIDADES LocalidadeC ON LocalidadeC.LOC_CODIGO = C.LOC_CODIGO
                        LEFT JOIN T_PAIS PaisC ON PaisC.PAI_CODIGO = LocalidadeC.PAI_CODIGO
                        LEFT OUTER JOIN T_LOCAL_ARMAZENAMENTO_PRODUTO LocalAbastecimento ON  LocalAbastecimento.LAP_CODIGO = A.LAP_CODIGO
                        WHERE A.ABA_SITUACAO <> 'G' ";

            if (filtrosPesquisa.SituacaoAcerto != Dominio.ObjetosDeValor.Enumerador.SituacaoAbastecimentoAcertoViagem.Todos)
            {
                if (filtrosPesquisa.SituacaoAcerto == Dominio.ObjetosDeValor.Enumerador.SituacaoAbastecimentoAcertoViagem.EmAcerto)
                    query += @" AND A.ABA_CODIGO IN (SELECT DISTINCT ABA_CODIGO FROM T_ACERTO_ABASTECIMENTO BV 
								    JOIN T_ACERTO_DE_VIAGEM AV ON AV.ACV_CODIGO = BV.ACV_CODIGO
								    WHERE AV.ACV_NUMERO IS NOT NULL)";
                else if (filtrosPesquisa.SituacaoAcerto == Dominio.ObjetosDeValor.Enumerador.SituacaoAbastecimentoAcertoViagem.NaoConstaEmAcerto)
                    query += @" AND A.ABA_CODIGO NOT IN (SELECT DISTINCT ABA_CODIGO FROM T_ACERTO_ABASTECIMENTO BV 
									JOIN T_ACERTO_DE_VIAGEM AV ON AV.ACV_CODIGO = BV.ACV_CODIGO
									WHERE AV.ACV_NUMERO IS NOT NULL)";
            }

            if (filtrosPesquisa.TiposRecebimento != null && filtrosPesquisa.TiposRecebimento.Count > 0)
            {
                query += $@" AND (A.ABA_TIPO_RECEBIMENTO IN ({ string.Join(", ", filtrosPesquisa.TiposRecebimento.Select(o => o.ToString("D"))) })";
                if (filtrosPesquisa.TiposRecebimento.Contains(TipoRecebimentoAbastecimento.Sistema))
                    query += $@" OR A.ABA_TIPO_RECEBIMENTO IS NULL) ";
                else
                    query += ") ";
            }

            if (filtrosPesquisa.DataInicial > DateTime.MinValue && filtrosPesquisa.DataFinal > DateTime.MinValue)
                query += " AND A.ABA_DATA >= '" + filtrosPesquisa.DataInicial.ToString("MM/dd/yyyy HH:mm") + "' AND A.ABA_DATA <= '" + filtrosPesquisa.DataFinal.ToString("MM/dd/yyyy HH:mm") + "'";
            else if (filtrosPesquisa.DataInicial > DateTime.MinValue && filtrosPesquisa.DataFinal == DateTime.MinValue)
                query += " AND A.ABA_DATA >= '" + filtrosPesquisa.DataInicial.ToString("MM/dd/yyyy HH:mm") + "' ";
            else if (filtrosPesquisa.DataInicial == DateTime.MinValue && filtrosPesquisa.DataFinal > DateTime.MinValue)
                query += " AND A.ABA_DATA <= '" + filtrosPesquisa.DataFinal.ToString("MM/dd/yyyy HH:mm") + "' ";

            if (filtrosPesquisa.DataBaseCRTInicial > DateTime.MinValue && filtrosPesquisa.DataBaseCRTFinal > DateTime.MinValue)
                query += " AND A.ABA_DATA_BASE_CRT >= '" + filtrosPesquisa.DataBaseCRTInicial.ToString("MM/dd/yyyy HH:mm") + "' AND A.ABA_DATA_BASE_CRT <= '" + filtrosPesquisa.DataBaseCRTFinal.ToString("MM/dd/yyyy HH:mm") + "'";
            else if (filtrosPesquisa.DataBaseCRTInicial > DateTime.MinValue && filtrosPesquisa.DataBaseCRTFinal == DateTime.MinValue)
                query += " AND A.ABA_DATA_BASE_CRT >= '" + filtrosPesquisa.DataBaseCRTInicial.ToString("MM/dd/yyyy HH:mm") + "' ";
            else if (filtrosPesquisa.DataBaseCRTInicial == DateTime.MinValue && filtrosPesquisa.DataBaseCRTFinal > DateTime.MinValue)
                query += " AND A.ABA_DATA_BASE_CRT <= '" + filtrosPesquisa.DataBaseCRTFinal.ToString("MM/dd/yyyy HH:mm") + "' ";

            if (filtrosPesquisa.CodigoSegmento > 0)
                query += " AND V.VSE_CODIGO = " + filtrosPesquisa.CodigoSegmento.ToString();

            if (filtrosPesquisa.CodigoEquipamento > 0)
                query += " AND A.EQP_CODIGO = " + filtrosPesquisa.CodigoEquipamento.ToString();

            if (filtrosPesquisa.TipoPropriedade != "0")
                query += " AND V.VEI_TIPO = '" + filtrosPesquisa.TipoPropriedade + "'";

            if (filtrosPesquisa.CodigoProprietario > 0)
                query += " AND V.VEI_PROPRIETARIO = '" + filtrosPesquisa.CodigoProprietario.ToString() + "'";
            if (filtrosPesquisa.Fornecedor > 0)
                query += " AND A.CLI_CGCCPF = '" + filtrosPesquisa.Fornecedor.ToString() + "'";

            if (filtrosPesquisa.CodigosProdutos != null && filtrosPesquisa.CodigosProdutos.Count > 0)
                query += " AND A.PRO_CODIGO in (" + string.Join(",", filtrosPesquisa.CodigosProdutos) + ")";
            if (filtrosPesquisa.CodigoEmpresa > 0)
                query += " AND A.EMP_CODIGO = '" + filtrosPesquisa.CodigoEmpresa.ToString() + "'";
            if (filtrosPesquisa.CodigoVeiculo > 0)
                query += " AND A.VEI_CODIGO = " + filtrosPesquisa.CodigoVeiculo.ToString();
            if (filtrosPesquisa.CodigoMotorista > 0)
                query += " AND A.FUN_CODIGO = " + filtrosPesquisa.CodigoMotorista.ToString();
            if ((int)filtrosPesquisa.TipoAbastecimento > 0)
                query += " AND A.ABA_TIPO = " + (int)filtrosPesquisa.TipoAbastecimento;

            if (filtrosPesquisa.StatusAbastecimento != "T")
                query += " AND A.ABA_SITUACAO = '" + filtrosPesquisa.StatusAbastecimento + "'";
            else
                query += " AND A.ABA_SITUACAO <> 'G' ";

            if (filtrosPesquisa.TipoAbastecimentoInternoExterno > 0)
            {
                if (filtrosPesquisa.TipoAbastecimentoInternoExterno == 1)//interno
                    query += " AND A.CLI_CGCCPF IN (SELECT CAST(E.EMP_CNPJ AS FLOAT) FROM T_EMPRESA E)";
                else if (filtrosPesquisa.TipoAbastecimentoInternoExterno == 2)//externo
                    query += " AND A.CLI_CGCCPF NOT IN (SELECT CAST(E.EMP_CNPJ AS FLOAT) FROM T_EMPRESA E)";
            }

            if (filtrosPesquisa.CodigoGrupoPessoas > 0)
                query += " AND GrupoPessoa.GRP_CODIGO = " + filtrosPesquisa.CodigoGrupoPessoas;

            if (filtrosPesquisa.CodigoCentroResultado > 0)
                query += " AND CentroResultado.CRE_CODIGO = " + filtrosPesquisa.CodigoCentroResultado;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.UFFornecedor) && filtrosPesquisa.UFFornecedor != "0")
                query += " AND Localidade.UF_SIGLA = '" + filtrosPesquisa.UFFornecedor + "'";

            if (filtrosPesquisa.ModeloVeicularCarga > 0)
                query += " AND ModeloVeicularCarga.MVC_CODIGO = " + filtrosPesquisa.ModeloVeicularCarga;

            if (filtrosPesquisa.Paises?.Count > 0)
                query += $" AND PaisC.PAI_CODIGO IN ({ string.Join(", ", filtrosPesquisa.Paises) }) ";

            if (filtrosPesquisa.Moedas?.Count > 0)
                query += $" AND ( A.ABA_MOEDA_COTACAO_BANCO_CENTRAL IN ({ string.Join(", ", filtrosPesquisa.Moedas.Select(o => o.ToString("D"))) }) {(filtrosPesquisa.Moedas.Contains(MoedaCotacaoBancoCentral.Real) ? " OR A.ABA_MOEDA_COTACAO_BANCO_CENTRAL IS NULL " : string.Empty)} ) ";

            if (filtrosPesquisa.CodigoLocalArmazenamento > 0)
                query += $" AND LocalAbastecimento.LAP_CODIGO = " + filtrosPesquisa.CodigoLocalArmazenamento;

            if (filtrosPesquisa.CodigoOrdemCompra > 0)
                query += $" AND A.ORC_CODIGO = " + filtrosPesquisa.CodigoOrdemCompra;

            var agrup = false;
            if (!string.IsNullOrWhiteSpace(propGrupo))
            {
                agrup = true;
                query += " order by " + propGrupo + " " + dirOrdenacaoGrupo;
            }

            if (!string.IsNullOrWhiteSpace(propOrdenacao) && propGrupo != propOrdenacao)
            {
                if (agrup)
                {
                    query += ", " + propOrdenacao + " " + dirOrdenacao;
                }
                else
                {
                    query += " order by " + propOrdenacao + " " + dirOrdenacao;
                }
            }

            if (maximoRegistros > 0)
                query += " OFFSET " + inicioRegistros + " ROWS FETCH FIRST " + maximoRegistros + " ROWS ONLY";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Frota.Abastecimento)));

            return nhQuery.SetTimeout(6000).List<Dominio.Relatorios.Embarcador.DataSource.Frota.Abastecimento>();
        }


        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Frota.Abastecimento>> RelatorioAbastecimentoAsync(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioAbastecimento filtrosPesquisa, string propGrupo, string dirOrdenacaoGrupo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            string query = @"SELECT A.ABA_CODIGO Codigo, 
                                    A.ABA_DOCUMENTO Documento,
                                    P.PRO_DESCRICAO Produto,
                                    C.CLI_NOME Fornecedor,
                                    V.VEI_PLACA Veiculo,
                                    V.VEI_CAPTANQUE CapacidadeTanque,
                                    A.ORC_CODIGO Requisicao,
                                    CASE
	                                    WHEN V.VEI_TIPOVEICULO = 0 THEN 'Tração'
	                                    ELSE 'Reboque'
                                    END Categoria,
                                    A.ABA_DATA Data,
                                    CASE
										WHEN P.PRO_DESCRICAO LIKE '%ARLA%' THEN
											ISNULL((SELECT TOP(1) ABA_KM FROM T_ABASTECIMENTO AA JOIN T_PRODUTO PP ON PP.PRO_CODIGO = AA.PRO_CODIGO AND PP.PRO_DESCRICAO LIKE '%ARLA%' WHERE AA.ABA_CODIGO <> A.ABA_CODIGO AND A.ABA_KM > AA.ABA_KM AND A.VEI_CODIGO = AA.VEI_CODIGO AND AA.ABA_SITUACAO <> 'G' ORDER BY ABA_KM DESC), 0)
										ELSE
											ISNULL((SELECT TOP(1) ABA_KM FROM T_ABASTECIMENTO AA JOIN T_PRODUTO PP ON PP.PRO_CODIGO = AA.PRO_CODIGO AND NOT PP.PRO_DESCRICAO LIKE '%ARLA%' WHERE AA.ABA_CODIGO <> A.ABA_CODIGO AND A.ABA_KM > AA.ABA_KM AND A.VEI_CODIGO = AA.VEI_CODIGO AND AA.ABA_SITUACAO <> 'G' ORDER BY ABA_KM DESC), 0)
									END KmAnterior,
                                    A.ABA_KM Km,
                                    A.ABA_LITROS Litros,
                                    CASE
	                                    WHEN A.ABA_SITUACAO = 'A' THEN 'Aberto'
	                                    WHEN A.ABA_SITUACAO = 'F' THEN 'Fechado'
                                        WHEN A.ABA_SITUACAO = 'G' THEN 'Agrupado'
	                                    ELSE 'Inconsistente'
                                    END Status, 
                                    ISNULL(ABA_VALOR_UN, 0) Valor, 
                                    (ISNULL(ABA_VALOR_UN, 0) * ISNULL(A.ABA_LITROS, 0)) ValorTotal,
                                    ISNULL((SELECT TOP(1) ISNULL(AV.ACV_NUMERO, 0) 
									    FROM T_ACERTO_ABASTECIMENTO BV 
									    JOIN T_ACERTO_DE_VIAGEM AV ON AV.ACV_CODIGO = BV.ACV_CODIGO AND AV.ACV_SITUACAO <> 3
									    WHERE BV.ABA_CODIGO = A.ABA_CODIGO), 0)  NumeroAcerto,  
                                    CASE
	                                    WHEN V.VEI_TIPO = 'T' THEN 'Terceiro'
	                                    ELSE 'Próprio' 
                                    END TipoPropriedade,
                                    CP.CLI_NOME Proprietario, SEG.VSE_DESCRICAO Segmento,
                                    (A.ABA_KM - CASE
										WHEN P.PRO_DESCRICAO LIKE '%ARLA%' THEN
											ISNULL((SELECT TOP(1) ABA_KM FROM T_ABASTECIMENTO AA JOIN T_PRODUTO PP ON PP.PRO_CODIGO = AA.PRO_CODIGO AND PP.PRO_DESCRICAO LIKE '%ARLA%' WHERE AA.ABA_CODIGO <> A.ABA_CODIGO AND A.ABA_KM > AA.ABA_KM AND A.VEI_CODIGO = AA.VEI_CODIGO AND AA.ABA_SITUACAO <> 'G' ORDER BY ABA_KM DESC), 0)
										ELSE
											ISNULL((SELECT TOP(1) ABA_KM FROM T_ABASTECIMENTO AA JOIN T_PRODUTO PP ON PP.PRO_CODIGO = AA.PRO_CODIGO AND NOT PP.PRO_DESCRICAO LIKE '%ARLA%' WHERE AA.ABA_CODIGO <> A.ABA_CODIGO AND A.ABA_KM > AA.ABA_KM AND A.VEI_CODIGO = AA.VEI_CODIGO AND AA.ABA_SITUACAO <> 'G' ORDER BY ABA_KM DESC), 0)
									END) KMTotal,
                                    ((A.ABA_KM - CASE
										WHEN P.PRO_DESCRICAO LIKE '%ARLA%' THEN
											ISNULL((SELECT TOP(1) ABA_KM FROM T_ABASTECIMENTO AA JOIN T_PRODUTO PP ON PP.PRO_CODIGO = AA.PRO_CODIGO AND PP.PRO_DESCRICAO LIKE '%ARLA%' WHERE AA.ABA_CODIGO <> A.ABA_CODIGO AND A.ABA_KM > AA.ABA_KM AND A.VEI_CODIGO = AA.VEI_CODIGO AND AA.ABA_SITUACAO <> 'G' ORDER BY ABA_KM DESC), 0)
										ELSE
											ISNULL((SELECT TOP(1) ABA_KM FROM T_ABASTECIMENTO AA JOIN T_PRODUTO PP ON PP.PRO_CODIGO = AA.PRO_CODIGO AND NOT PP.PRO_DESCRICAO LIKE '%ARLA%' WHERE AA.ABA_CODIGO <> A.ABA_CODIGO AND A.ABA_KM > AA.ABA_KM AND A.VEI_CODIGO = AA.VEI_CODIGO AND AA.ABA_SITUACAO <> 'G' ORDER BY ABA_KM DESC), 0)
									END) / CASE WHEN A.ABA_LITROS <= 0 THEN 1 ELSE A.ABA_LITROS END) Media, 
                                    ISNULL(V.VEI_NUMERO_FROTA, '') NumeroFrota,
                                    ISNULL(F.FUN_NOME, (SELECT TOP(1) F.FUN_NOME FROM T_ACERTO_ABASTECIMENTO BV 
                                        JOIN T_ACERTO_DE_VIAGEM AV ON AV.ACV_CODIGO = BV.ACV_CODIGO AND AV.ACV_SITUACAO <> 3
                                        JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = AV.FUN_CODIGO_MOTORISTA
                                        WHERE BV.ABA_CODIGO = A.ABA_CODIGO ORDER BY BV.ACB_CODIGO DESC)) Motorista, 
                                   Equipamento.EQP_DESCRICAO Equipamento,
                                   A.ABA_HORIMETRO Horimetro,
                                   ISNULL(ModeloVeiculo.VMO_MEDIA_PADRAO, 0) MediaPadrao,

                                  ISNULL((SELECT TOP(1) AA.ABA_HORIMETRO FROM T_ABASTECIMENTO AA 
                                              WHERE AA.EQP_CODIGO = A.EQP_CODIGO AND AA.ABA_CODIGO < A.ABA_CODIGO AND AA.ABA_SITUACAO <> 'G' ORDER BY AA.ABA_CODIGO DESC), A.ABA_HORIMETRO) HorimetroAnterior,

                                    (A.ABA_HORIMETRO - ISNULL((SELECT TOP(1) ABA_HORIMETRO FROM T_ABASTECIMENTO AA
                                            WHERE A.ABA_HORIMETRO > AA.ABA_HORIMETRO AND A.EQP_CODIGO = AA.EQP_CODIGO AND AA.ABA_SITUACAO <> 'G' ORDER BY ABA_HORIMETRO DESC), A.ABA_HORIMETRO)) HorimetroTotal,

                                    (CASE WHEN (SELECT count(*) FROM T_ABASTECIMENTO AA WHERE A.ABA_HORIMETRO > AA.ABA_HORIMETRO AND A.EQP_CODIGO = AA.EQP_CODIGO AND AA.ABA_SITUACAO <> 'G') > 0 and A.ABA_LITROS > 0 THEN
                                        CASE WHEN (A.ABA_HORIMETRO - ISNULL((SELECT TOP(1) AA.ABA_HORIMETRO FROM T_ABASTECIMENTO AA
                                                                             WHERE A.ABA_HORIMETRO > AA.ABA_HORIMETRO AND A.EQP_CODIGO = AA.EQP_CODIGO AND AA.ABA_SITUACAO <> 'G' ORDER BY AA.ABA_HORIMETRO DESC), 0)) <= 0 THEN 0
                                        ELSE (A.ABA_LITROS / (A.ABA_HORIMETRO - ISNULL((SELECT TOP(1) AA.ABA_HORIMETRO FROM T_ABASTECIMENTO AA
                                                                                        WHERE A.ABA_HORIMETRO > AA.ABA_HORIMETRO AND A.EQP_CODIGO = AA.EQP_CODIGO AND AA.ABA_SITUACAO <> 'G' ORDER BY AA.ABA_HORIMETRO DESC), 0))) END
                                    ELSE 0 END) MediaHorimetro,

                                    A.ABA_MOTIVO_INCONSISTENCIA MotivoInconsistencia,
                                    A.ABA_TIPO_RECEBIMENTO TipoRecebimento,
                                    C.CLI_CGCCPF CpfCnpjFornecedor,
                                    C.CLI_FISJUR TipoFornecedor,
                                    ISNULL(A.ABA_KM_ORIGINAL, 0) KmOriginal,
                                    ISNULL(A.ABA_HORIMETRO_ORIGINAL, 0) HorimetroOriginal,
                                    GrupoPessoa.GRP_DESCRICAO GrupoPessoa,
                                    CentroResultado.CRE_DESCRICAO CentroResultado,
				                    CONVERT(varchar, A.ABA_DATA, 103) DataSeparada,
				                    CONVERT(varchar, A.ABA_DATA, 108) HoraSeparada,
				                    C.CLI_NOMEFANTASIA FantasiaFornecedor,
                                    Localidade.UF_SIGLA UFFornecedor,
                                    ModeloVeicularCarga.MVC_DESCRICAO ModeloVeicularCarga,
                                    MotoristaAnterior.FUN_NOME MotoristaAnterior,
                                    A.ABA_KM_ANTERIOR_ALTERACAO KMAnteriorAlteracao,
                                    A.ABA_DATA_ANTERIOR DataAnterior,
                                    A.ABA_DATA_BASE_CRT DataBaseCRT, 
                                    A.ABA_MOEDA_COTACAO_BANCO_CENTRAL Moeda, 
                                    A.ABA_VALOR_MOEDA_COTACAO ValorMoeda, 
                                    A.ABA_VALOR_ORIGINAL_MOEDA_ESTRANGEIRA ValorOriginalMoeda, 
                                    PaisC.PAI_NOME Pais,
                                    LocalidadeC.LOC_DESCRICAO + ' - ' + LocalidadeC.UF_SIGLA LocalidadeFornecedor,
                                    A.ABA_OBSERVACAO Observacao,
                                    LocalAbastecimento.LAP_DESCRICAO LocalArmazenamento,
									isnull(SUBSTRING((     select ', ' + VeiculoReboque.VEI_PLACA       
										from
											T_VEICULO_CONJUNTO Reboque       
										join
											T_VEICULO VeiculoReboque 
												on VeiculoReboque.VEI_CODIGO = Reboque.VEC_CODIGO_FILHO      
										where
											Reboque.VEC_CODIGO_PAI = V.VEI_CODIGO        for XML PATH('') ),
										3,
										1000),
										'') as PlacaReboque,
									(SELECT VM.MVC_DESCRICAO FROM T_MODELO_VEICULAR_CARGA VM WHERE V.MVC_CODIGO = VM.MVC_CODIGO) ModeloVeicularTracao,
									isnull(      SUBSTRING((    SELECT
											', ' + VM.MVC_DESCRICAO    
										FROM
											T_VEICULO_CONJUNTO Reboque     
										JOIN
											T_VEICULO VeiculoReboque      
												ON VeiculoReboque.VEI_CODIGO = Reboque.VEC_CODIGO_FILHO     
										LEFT JOIN
											T_MODELO_VEICULAR_CARGA VM      
												ON VM.MVC_CODIGO = VeiculoReboque.MVC_CODIGO    
										WHERE
											Reboque.VEC_CODIGO_PAI = V.VEI_CODIGO    for XML PATH('')),
										3,
										1000),
										'') as ModeloVeicularReboque,

									SEG.VSE_DESCRICAO as SegmentoTracao,
									isnull(SUBSTRING((SELECT
											', ' + SegmentoReboque.VSE_DESCRICAO    
										FROM
											T_VEICULO_CONJUNTO Reboque
										JOIN
											T_VEICULO VeiculoReboque      
												ON VeiculoReboque.VEI_CODIGO = Reboque.VEC_CODIGO_FILHO 
										LEFT JOIN
											T_VEICULO_SEGMENTO SegmentoReboque      
												ON SegmentoReboque.VSE_CODIGO = VeiculoReboque.VSE_CODIGO    
										WHERE
											Reboque.VEC_CODIGO_PAI = V.VEI_CODIGO for XML PATH('')),
										3,
										1000), '') as SegmentoReboque
                        FROM T_ABASTECIMENTO A 
                        LEFT OUTER JOIN T_PRODUTO P ON P.PRO_CODIGO = A.PRO_CODIGO
                        LEFT OUTER JOIN T_CLIENTE C ON C.CLI_CGCCPF = A.CLI_CGCCPF
                        LEFT OUTER JOIN T_VEICULO V ON V.VEI_CODIGO = A.VEI_CODIGO 
                        LEFT OUTER JOIN T_CLIENTE CP ON CP.CLI_CGCCPF = V.VEI_PROPRIETARIO
                        LEFT OUTER JOIN T_VEICULO_SEGMENTO SEG ON SEG.VSE_CODIGO = V.VSE_CODIGO
                        LEFT OUTER JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = A.FUN_CODIGO
                        LEFT OUTER JOIN T_EQUIPAMENTO Equipamento ON Equipamento.EQP_CODIGO = A.EQP_CODIGO
                        LEFT OUTER JOIN T_VEICULO_MODELO ModeloVeiculo ON ModeloVeiculo.VMO_CODIGO = V.VMO_CODIGO
                        LEFT OUTER JOIN T_GRUPO_PESSOAS GrupoPessoa ON GrupoPessoa.GRP_CODIGO = C.GRP_CODIGO
                        LEFT OUTER JOIN T_CENTRO_RESULTADO CentroResultado ON CentroResultado.CRE_CODIGO = A.CRE_CODIGO
                        LEFT OUTER JOIN T_LOCALIDADES Localidade ON Localidade.LOC_CODIGO = C.LOC_CODIGO
                        LEFT OUTER JOIN T_MODELO_VEICULAR_CARGA ModeloVeicularCarga ON V.MVC_CODIGO = ModeloVeicularCarga.MVC_CODIGO
                        LEFT OUTER JOIN T_FUNCIONARIO MotoristaAnterior on MotoristaAnterior.FUN_CODIGO = A.FUN_CODIGO_ANTERIOR
                        LEFT OUTER JOIN T_LOCALIDADES LocalidadeC ON LocalidadeC.LOC_CODIGO = C.LOC_CODIGO
                        LEFT JOIN T_PAIS PaisC ON PaisC.PAI_CODIGO = LocalidadeC.PAI_CODIGO
                        LEFT OUTER JOIN T_LOCAL_ARMAZENAMENTO_PRODUTO LocalAbastecimento ON  LocalAbastecimento.LAP_CODIGO = A.LAP_CODIGO
                        WHERE A.ABA_SITUACAO <> 'G' ";

            if (filtrosPesquisa.SituacaoAcerto != Dominio.ObjetosDeValor.Enumerador.SituacaoAbastecimentoAcertoViagem.Todos)
            {
                if (filtrosPesquisa.SituacaoAcerto == Dominio.ObjetosDeValor.Enumerador.SituacaoAbastecimentoAcertoViagem.EmAcerto)
                    query += @" AND A.ABA_CODIGO IN (SELECT DISTINCT ABA_CODIGO FROM T_ACERTO_ABASTECIMENTO BV 
								    JOIN T_ACERTO_DE_VIAGEM AV ON AV.ACV_CODIGO = BV.ACV_CODIGO
								    WHERE AV.ACV_NUMERO IS NOT NULL)";
                else if (filtrosPesquisa.SituacaoAcerto == Dominio.ObjetosDeValor.Enumerador.SituacaoAbastecimentoAcertoViagem.NaoConstaEmAcerto)
                    query += @" AND A.ABA_CODIGO NOT IN (SELECT DISTINCT ABA_CODIGO FROM T_ACERTO_ABASTECIMENTO BV 
									JOIN T_ACERTO_DE_VIAGEM AV ON AV.ACV_CODIGO = BV.ACV_CODIGO
									WHERE AV.ACV_NUMERO IS NOT NULL)";
            }

            if (filtrosPesquisa.TiposRecebimento != null && filtrosPesquisa.TiposRecebimento.Count > 0)
            {
                query += $@" AND (A.ABA_TIPO_RECEBIMENTO IN ({string.Join(", ", filtrosPesquisa.TiposRecebimento.Select(o => o.ToString("D")))})";
                if (filtrosPesquisa.TiposRecebimento.Contains(TipoRecebimentoAbastecimento.Sistema))
                    query += $@" OR A.ABA_TIPO_RECEBIMENTO IS NULL) ";
                else
                    query += ") ";
            }

            if (filtrosPesquisa.DataInicial > DateTime.MinValue && filtrosPesquisa.DataFinal > DateTime.MinValue)
                query += " AND A.ABA_DATA >= '" + filtrosPesquisa.DataInicial.ToString("MM/dd/yyyy HH:mm") + "' AND A.ABA_DATA <= '" + filtrosPesquisa.DataFinal.ToString("MM/dd/yyyy HH:mm") + "'";
            else if (filtrosPesquisa.DataInicial > DateTime.MinValue && filtrosPesquisa.DataFinal == DateTime.MinValue)
                query += " AND A.ABA_DATA >= '" + filtrosPesquisa.DataInicial.ToString("MM/dd/yyyy HH:mm") + "' ";
            else if (filtrosPesquisa.DataInicial == DateTime.MinValue && filtrosPesquisa.DataFinal > DateTime.MinValue)
                query += " AND A.ABA_DATA <= '" + filtrosPesquisa.DataFinal.ToString("MM/dd/yyyy HH:mm") + "' ";

            if (filtrosPesquisa.DataBaseCRTInicial > DateTime.MinValue && filtrosPesquisa.DataBaseCRTFinal > DateTime.MinValue)
                query += " AND A.ABA_DATA_BASE_CRT >= '" + filtrosPesquisa.DataBaseCRTInicial.ToString("MM/dd/yyyy HH:mm") + "' AND A.ABA_DATA_BASE_CRT <= '" + filtrosPesquisa.DataBaseCRTFinal.ToString("MM/dd/yyyy HH:mm") + "'";
            else if (filtrosPesquisa.DataBaseCRTInicial > DateTime.MinValue && filtrosPesquisa.DataBaseCRTFinal == DateTime.MinValue)
                query += " AND A.ABA_DATA_BASE_CRT >= '" + filtrosPesquisa.DataBaseCRTInicial.ToString("MM/dd/yyyy HH:mm") + "' ";
            else if (filtrosPesquisa.DataBaseCRTInicial == DateTime.MinValue && filtrosPesquisa.DataBaseCRTFinal > DateTime.MinValue)
                query += " AND A.ABA_DATA_BASE_CRT <= '" + filtrosPesquisa.DataBaseCRTFinal.ToString("MM/dd/yyyy HH:mm") + "' ";

            if (filtrosPesquisa.CodigoSegmento > 0)
                query += " AND V.VSE_CODIGO = " + filtrosPesquisa.CodigoSegmento.ToString();

            if (filtrosPesquisa.CodigoEquipamento > 0)
                query += " AND A.EQP_CODIGO = " + filtrosPesquisa.CodigoEquipamento.ToString();

            if (filtrosPesquisa.TipoPropriedade != "0")
                query += " AND V.VEI_TIPO = '" + filtrosPesquisa.TipoPropriedade + "'";

            if (filtrosPesquisa.CodigoProprietario > 0)
                query += " AND V.VEI_PROPRIETARIO = '" + filtrosPesquisa.CodigoProprietario.ToString() + "'";
            if (filtrosPesquisa.Fornecedor > 0)
                query += " AND A.CLI_CGCCPF = '" + filtrosPesquisa.Fornecedor.ToString() + "'";

            if (filtrosPesquisa.CodigosProdutos != null && filtrosPesquisa.CodigosProdutos.Count > 0)
                query += " AND A.PRO_CODIGO in (" + string.Join(",", filtrosPesquisa.CodigosProdutos) + ")";
            if (filtrosPesquisa.CodigoEmpresa > 0)
                query += " AND A.EMP_CODIGO = '" + filtrosPesquisa.CodigoEmpresa.ToString() + "'";
            if (filtrosPesquisa.CodigoVeiculo > 0)
                query += " AND A.VEI_CODIGO = " + filtrosPesquisa.CodigoVeiculo.ToString();
            if (filtrosPesquisa.CodigoMotorista > 0)
                query += " AND A.FUN_CODIGO = " + filtrosPesquisa.CodigoMotorista.ToString();
            if ((int)filtrosPesquisa.TipoAbastecimento > 0)
                query += " AND A.ABA_TIPO = " + (int)filtrosPesquisa.TipoAbastecimento;

            if (filtrosPesquisa.StatusAbastecimento != "T")
                query += " AND A.ABA_SITUACAO = '" + filtrosPesquisa.StatusAbastecimento + "'";
            else
                query += " AND A.ABA_SITUACAO <> 'G' ";

            if (filtrosPesquisa.TipoAbastecimentoInternoExterno > 0)
            {
                if (filtrosPesquisa.TipoAbastecimentoInternoExterno == 1)//interno
                    query += " AND A.CLI_CGCCPF IN (SELECT CAST(E.EMP_CNPJ AS FLOAT) FROM T_EMPRESA E)";
                else if (filtrosPesquisa.TipoAbastecimentoInternoExterno == 2)//externo
                    query += " AND A.CLI_CGCCPF NOT IN (SELECT CAST(E.EMP_CNPJ AS FLOAT) FROM T_EMPRESA E)";
            }

            if (filtrosPesquisa.CodigoGrupoPessoas > 0)
                query += " AND GrupoPessoa.GRP_CODIGO = " + filtrosPesquisa.CodigoGrupoPessoas;

            if (filtrosPesquisa.CodigoCentroResultado > 0)
                query += " AND CentroResultado.CRE_CODIGO = " + filtrosPesquisa.CodigoCentroResultado;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.UFFornecedor) && filtrosPesquisa.UFFornecedor != "0")
                query += " AND Localidade.UF_SIGLA = '" + filtrosPesquisa.UFFornecedor + "'";

            if (filtrosPesquisa.ModeloVeicularCarga > 0)
                query += " AND ModeloVeicularCarga.MVC_CODIGO = " + filtrosPesquisa.ModeloVeicularCarga;

            if (filtrosPesquisa.Paises?.Count > 0)
                query += $" AND PaisC.PAI_CODIGO IN ({string.Join(", ", filtrosPesquisa.Paises)}) ";

            if (filtrosPesquisa.Moedas?.Count > 0)
                query += $" AND ( A.ABA_MOEDA_COTACAO_BANCO_CENTRAL IN ({string.Join(", ", filtrosPesquisa.Moedas.Select(o => o.ToString("D")))}) {(filtrosPesquisa.Moedas.Contains(MoedaCotacaoBancoCentral.Real) ? " OR A.ABA_MOEDA_COTACAO_BANCO_CENTRAL IS NULL " : string.Empty)} ) ";

            if (filtrosPesquisa.CodigoLocalArmazenamento > 0)
                query += $" AND LocalAbastecimento.LAP_CODIGO = " + filtrosPesquisa.CodigoLocalArmazenamento;

            if (filtrosPesquisa.CodigoOrdemCompra > 0)
                query += $" AND A.ORC_CODIGO = " + filtrosPesquisa.CodigoOrdemCompra;

            var agrup = false;
            if (!string.IsNullOrWhiteSpace(propGrupo))
            {
                agrup = true;
                query += " order by " + propGrupo + " " + dirOrdenacaoGrupo;
            }

            if (!string.IsNullOrWhiteSpace(propOrdenacao) && propGrupo != propOrdenacao)
            {
                if (agrup)
                {
                    query += ", " + propOrdenacao + " " + dirOrdenacao;
                }
                else
                {
                    query += " order by " + propOrdenacao + " " + dirOrdenacao;
                }
            }

            if (maximoRegistros > 0)
                query += " OFFSET " + inicioRegistros + " ROWS FETCH FIRST " + maximoRegistros + " ROWS ONLY";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Frota.Abastecimento)));

            return await nhQuery.SetTimeout(6000).ListAsync<Dominio.Relatorios.Embarcador.DataSource.Frota.Abastecimento>();
        }


        public int ContarRelatorioAbastecimento(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioAbastecimento filtrosPesquisa)
        {
            string query = @"   SELECT COUNT(0) as CONTADOR 
                                FROM T_ABASTECIMENTO A
                                LEFT OUTER JOIN T_PRODUTO P ON P.PRO_CODIGO = A.PRO_CODIGO
                                LEFT OUTER JOIN T_CLIENTE C ON C.CLI_CGCCPF = A.CLI_CGCCPF
                                LEFT OUTER JOIN T_VEICULO V ON V.VEI_CODIGO = A.VEI_CODIGO
                                LEFT OUTER JOIN T_CLIENTE CP ON CP.CLI_CGCCPF = V.VEI_PROPRIETARIO
                                LEFT OUTER JOIN T_VEICULO_SEGMENTO SEG ON SEG.VSE_CODIGO = V.VSE_CODIGO
                                LEFT OUTER JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = A.FUN_CODIGO
                                LEFT OUTER JOIN T_EQUIPAMENTO Equipamento ON Equipamento.EQP_CODIGO = A.EQP_CODIGO
                                LEFT OUTER JOIN T_VEICULO_MODELO ModeloVeiculo ON ModeloVeiculo.VMO_CODIGO = V.VMO_CODIGO
                                LEFT OUTER JOIN T_GRUPO_PESSOAS GrupoPessoa ON GrupoPessoa.GRP_CODIGO = C.GRP_CODIGO
                                LEFT OUTER JOIN T_CENTRO_RESULTADO CentroResultado ON CentroResultado.CRE_CODIGO = A.CRE_CODIGO
                                LEFT OUTER JOIN T_LOCALIDADES Localidade ON Localidade.LOC_CODIGO = C.LOC_CODIGO
                                LEFT OUTER JOIN T_MODELO_VEICULAR_CARGA ModeloVeicularCarga ON V.MVC_CODIGO = ModeloVeicularCarga.MVC_CODIGO
                                LEFT OUTER JOIN T_LOCALIDADES LocalidadeC ON LocalidadeC.LOC_CODIGO = C.LOC_CODIGO
                                LEFT JOIN T_PAIS PaisC ON PaisC.PAI_CODIGO = LocalidadeC.PAI_CODIGO
                                LEFT OUTER JOIN T_LOCAL_ARMAZENAMENTO_PRODUTO LocalAbastecimento ON  LocalAbastecimento.LAP_CODIGO = A.LAP_CODIGO
                                WHERE 1 = 1 ";

            if (filtrosPesquisa.TiposRecebimento != null && filtrosPesquisa.TiposRecebimento.Count > 0)
            {
                query += $@" AND (A.ABA_TIPO_RECEBIMENTO IN ({ string.Join(", ", filtrosPesquisa.TiposRecebimento.Select(o => o.ToString("D"))) })";
                if (filtrosPesquisa.TiposRecebimento.Contains(TipoRecebimentoAbastecimento.Sistema))
                    query += $@" OR A.ABA_TIPO_RECEBIMENTO IS NULL) ";
                else
                    query += ") ";
            }

            if (filtrosPesquisa.SituacaoAcerto != Dominio.ObjetosDeValor.Enumerador.SituacaoAbastecimentoAcertoViagem.Todos)
            {
                if (filtrosPesquisa.SituacaoAcerto == Dominio.ObjetosDeValor.Enumerador.SituacaoAbastecimentoAcertoViagem.EmAcerto)
                    query += @" AND A.ABA_CODIGO IN (SELECT DISTINCT ABA_CODIGO FROM T_ACERTO_ABASTECIMENTO BV 
								    JOIN T_ACERTO_DE_VIAGEM AV ON AV.ACV_CODIGO = BV.ACV_CODIGO
								    WHERE AV.ACV_NUMERO IS NOT NULL)";
                else if (filtrosPesquisa.SituacaoAcerto == Dominio.ObjetosDeValor.Enumerador.SituacaoAbastecimentoAcertoViagem.NaoConstaEmAcerto)
                    query += @" AND A.ABA_CODIGO NOT IN (SELECT DISTINCT ABA_CODIGO FROM T_ACERTO_ABASTECIMENTO BV 
									JOIN T_ACERTO_DE_VIAGEM AV ON AV.ACV_CODIGO = BV.ACV_CODIGO
									WHERE AV.ACV_NUMERO IS NOT NULL)";
            }

            if (filtrosPesquisa.DataInicial > DateTime.MinValue && filtrosPesquisa.DataFinal > DateTime.MinValue)
                query += " AND A.ABA_DATA >= '" + filtrosPesquisa.DataInicial.ToString("MM/dd/yyyy HH:mm") + "' AND A.ABA_DATA <= '" + filtrosPesquisa.DataFinal.ToString("MM/dd/yyyy HH:mm") + "'";
            else if (filtrosPesquisa.DataInicial > DateTime.MinValue && filtrosPesquisa.DataFinal == DateTime.MinValue)
                query += " AND A.ABA_DATA >= '" + filtrosPesquisa.DataInicial.ToString("MM/dd/yyyy HH:mm") + "' ";
            else if (filtrosPesquisa.DataInicial == DateTime.MinValue && filtrosPesquisa.DataFinal > DateTime.MinValue)
                query += " AND A.ABA_DATA <= '" + filtrosPesquisa.DataFinal.ToString("MM/dd/yyyy HH:mm") + "' ";

            if (filtrosPesquisa.DataBaseCRTInicial > DateTime.MinValue && filtrosPesquisa.DataBaseCRTFinal > DateTime.MinValue)
                query += " AND A.ABA_DATA_BASE_CRT >= '" + filtrosPesquisa.DataBaseCRTInicial.ToString("MM/dd/yyyy HH:mm") + "' AND A.ABA_DATA_BASE_CRT <= '" + filtrosPesquisa.DataBaseCRTFinal.ToString("MM/dd/yyyy HH:mm") + "'";
            else if (filtrosPesquisa.DataBaseCRTInicial > DateTime.MinValue && filtrosPesquisa.DataBaseCRTFinal == DateTime.MinValue)
                query += " AND A.ABA_DATA_BASE_CRT >= '" + filtrosPesquisa.DataBaseCRTInicial.ToString("MM/dd/yyyy HH:mm") + "' ";
            else if (filtrosPesquisa.DataBaseCRTInicial == DateTime.MinValue && filtrosPesquisa.DataBaseCRTFinal > DateTime.MinValue)
                query += " AND A.ABA_DATA_BASE_CRT <= '" + filtrosPesquisa.DataBaseCRTFinal.ToString("MM/dd/yyyy HH:mm") + "' ";

            if (filtrosPesquisa.CodigoSegmento > 0)
                query += " AND V.VSE_CODIGO = " + filtrosPesquisa.CodigoSegmento.ToString();

            if (filtrosPesquisa.CodigoEquipamento > 0)
                query += " AND A.EQP_CODIGO = " + filtrosPesquisa.CodigoEquipamento.ToString();

            if (filtrosPesquisa.TipoPropriedade != "0")
                query += " AND V.VEI_TIPO = '" + filtrosPesquisa.TipoPropriedade + "'";

            if (filtrosPesquisa.CodigoProprietario > 0)
                query += " AND V.VEI_PROPRIETARIO = '" + filtrosPesquisa.CodigoProprietario.ToString() + "'";
            if (filtrosPesquisa.Fornecedor > 0)
                query += " AND A.CLI_CGCCPF = '" + filtrosPesquisa.Fornecedor.ToString() + "'";

            if (filtrosPesquisa.CodigosProdutos != null && filtrosPesquisa.CodigosProdutos.Count > 0)
                query += " AND A.PRO_CODIGO in (" + string.Join(",", filtrosPesquisa.CodigosProdutos) + ")";
            if (filtrosPesquisa.CodigoEmpresa > 0)
                query += " AND A.EMP_CODIGO = '" + filtrosPesquisa.CodigoEmpresa.ToString() + "'";
            if (filtrosPesquisa.CodigoVeiculo > 0)
                query += " AND A.VEI_CODIGO = " + filtrosPesquisa.CodigoVeiculo.ToString();
            if (filtrosPesquisa.CodigoMotorista > 0)
                query += " AND A.FUN_CODIGO = " + filtrosPesquisa.CodigoMotorista.ToString();
            if ((int)filtrosPesquisa.TipoAbastecimento > 0)
                query += " AND A.ABA_TIPO = " + (int)filtrosPesquisa.TipoAbastecimento;

            if (filtrosPesquisa.StatusAbastecimento != "T")
                query += " AND A.ABA_SITUACAO = '" + filtrosPesquisa.StatusAbastecimento + "'";
            else
                query += " AND A.ABA_SITUACAO <> 'G' ";

            if (filtrosPesquisa.TipoAbastecimentoInternoExterno > 0)
            {
                if (filtrosPesquisa.TipoAbastecimentoInternoExterno == 1)//interno
                    query += " AND A.CLI_CGCCPF IN (SELECT CAST(E.EMP_CNPJ AS FLOAT) FROM T_EMPRESA E)";
                else if (filtrosPesquisa.TipoAbastecimentoInternoExterno == 2)//externo
                    query += " AND A.CLI_CGCCPF NOT IN (SELECT CAST(E.EMP_CNPJ AS FLOAT) FROM T_EMPRESA E)";
            }

            if (filtrosPesquisa.CodigoGrupoPessoas > 0)
                query += " AND GrupoPessoa.GRP_CODIGO = " + filtrosPesquisa.CodigoGrupoPessoas;

            if (filtrosPesquisa.CodigoCentroResultado > 0)
                query += " AND CentroResultado.CRE_CODIGO = " + filtrosPesquisa.CodigoCentroResultado;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.UFFornecedor) && filtrosPesquisa.UFFornecedor != "0")
                query += " AND Localidade.UF_SIGLA = '" + filtrosPesquisa.UFFornecedor + "'";

            if (filtrosPesquisa.ModeloVeicularCarga > 0)
                query += " AND ModeloVeicularCarga.MVC_CODIGO = " + filtrosPesquisa.ModeloVeicularCarga;

            if (filtrosPesquisa.Paises?.Count > 0)
                query += $" AND PaisC.PAI_CODIGO IN ({ string.Join(", ", filtrosPesquisa.Paises) }) ";

            if (filtrosPesquisa.Moedas?.Count > 0)
                query += $" AND ( A.ABA_MOEDA_COTACAO_BANCO_CENTRAL IN ({ string.Join(", ", filtrosPesquisa.Moedas.Select(o => o.ToString("D"))) }) {(filtrosPesquisa.Moedas.Contains(MoedaCotacaoBancoCentral.Real) ? " OR A.ABA_MOEDA_COTACAO_BANCO_CENTRAL IS NULL " : string.Empty)} ) ";

            if (filtrosPesquisa.CodigoLocalArmazenamento > 0)
                query += $" AND LocalAbastecimento.LAP_CODIGO = " + filtrosPesquisa.CodigoLocalArmazenamento;

            if (filtrosPesquisa.CodigoOrdemCompra > 0)
                query += $" AND A.ORC_CODIGO = " + filtrosPesquisa.CodigoOrdemCompra;

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.SetTimeout(6000).UniqueResult<int>();
        }

        #endregion

        #region Relatório de Retorno de Abastecimento Angellira

        public IList<Dominio.Relatorios.Embarcador.DataSource.Frota.RetornoAbastecimentoAngellira> RelatorioRetornoAbastecimentoAngellira(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioRetornoAbastecimentoAngellira filtrosPesquisa, string propGrupo, string dirOrdenacaoGrupo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            string query = @"SELECT Retorno.RBA_CODIGO Codigo,
                Consulta.CAA_DATA_CONSULTA DataConsulta,
                Consulta.CAA_DATA_INICIAL DataInicial,
                Consulta.CAA_DATA_FINAL DataFinal,
                Consulta.CAA_MENSAGEM_RETORNO Retorno,
                Veiculo.VEI_PLACA Placa,
                Veiculo.VEI_NUMERO_FROTA Frota,
                Retorno.RBA_PLACA PlacaRetorno,
                Retorno.RBA_CONDUTOR CondutorRetorno,
                Retorno.RBA_DATA_HORA DataRetorno,
                Retorno.RBA_CORDENADA CordenadaRetorno,
                Retorno.RBA_LATITUDE LatitudeRetorno,
                Retorno.RBA_LONGITUDE LontitudeRetorno,
                Retorno.RBA_ODOMETRO Odometro,
                Abastecimento.ABA_CODIGO CodigoAbastecimento,
                CASE
	                WHEN Abastecimento.ABA_SITUACAO = 'A' THEN 'Aberto'
	                WHEN Abastecimento.ABA_SITUACAO = 'F' THEN 'Fechado'
                    WHEN Abastecimento.ABA_SITUACAO = 'G' THEN 'Agrupado'
	                ELSE 'Inconsistente'
                END SituacaoAbastecimento, 
                Motorista.FUN_NOME NomeMotorista,
                Motorista.FUN_CPF CPFMotorista,
                Posto.CLI_FISJUR TipoPosto,
                Posto.CLI_CGCCPF CNPJCPFPosto,
                Posto.CLI_NOME NomePosto,
                Retorno.RBA_SITUACAO_INTEGRACAO SituacaoIntegracao
                FROM T_RETORNO_CONSULTA_ABASTECIMENTO_ANGELLIRA Retorno                
                JOIN T_CONSULTA_ABASTECIMENTO_ANGELLIRA Consulta on Retorno.CAA_CODIGO = Consulta.CAA_CODIGO
				JOIN T_CONSULTA_ABASTECIMENTO_ANGELLIRA_VEICULO ConsultaVeiculo on ConsultaVeiculo.CAA_CODIGO = Consulta.CAA_CODIGO and Retorno.VEI_CODIGO = ConsultaVeiculo.VEI_CODIGO
                JOIN T_VEICULO Veiculo on Veiculo.VEI_CODIGO = ConsultaVeiculo.VEI_CODIGO
                LEFT OUTER JOIN T_ABASTECIMENTO Abastecimento on Abastecimento.ABA_CODIGO = Retorno.ABA_CODIGO
                LEFT OUTER JOIN T_FUNCIONARIO Motorista on Motorista.FUN_CODIGO = Retorno.FUN_CODIGO
                LEFT OUTER JOIN T_CLIENTE Posto on Posto.CLI_CGCCPF = Retorno.CLI_CGCCPF WHERE Retorno.RBA_ODOMETRO > 0 ";

            if (filtrosPesquisa.DataInicial > DateTime.MinValue && filtrosPesquisa.DataFinal > DateTime.MinValue)
                query += " AND Retorno.RBA_DATA_HORA>= '" + filtrosPesquisa.DataInicial.ToString("MM/dd/yyyy HH:mm") + "' AND Retorno.RBA_DATA_HORA <= '" + filtrosPesquisa.DataFinal.ToString("MM/dd/yyyy HH:mm") + "'";
            else if (filtrosPesquisa.DataInicial > DateTime.MinValue && filtrosPesquisa.DataFinal == DateTime.MinValue)
                query += " AND Retorno.RBA_DATA_HORA >= '" + filtrosPesquisa.DataInicial.ToString("MM/dd/yyyy HH:mm") + "' ";
            else if (filtrosPesquisa.DataInicial == DateTime.MinValue && filtrosPesquisa.DataFinal > DateTime.MinValue)
                query += " AND Retorno.RBA_DATA_HORA <= '" + filtrosPesquisa.DataFinal.ToString("MM/dd/yyyy HH:mm") + "' ";

            if (filtrosPesquisa.Fornecedor > 0)
                query += " AND Posto.CLI_CGCCPF = '" + filtrosPesquisa.Fornecedor.ToString() + "'";
            if (filtrosPesquisa.CodigoVeiculo > 0)
                query += " AND ConsultaVeiculo.VEI_CODIGO = " + filtrosPesquisa.CodigoVeiculo.ToString();
            if (filtrosPesquisa.CodigoMotorista > 0)
                query += " AND Retorno.FUN_CODIGO = " + filtrosPesquisa.CodigoMotorista.ToString();

            var agrup = false;
            if (!string.IsNullOrWhiteSpace(propGrupo))
            {
                agrup = true;
                query += " order by " + propGrupo + " " + dirOrdenacaoGrupo;
            }

            if (!string.IsNullOrWhiteSpace(propOrdenacao) && propGrupo != propOrdenacao)
            {
                if (agrup)
                {
                    query += ", " + propOrdenacao + " " + dirOrdenacao;
                }
                else
                {
                    query += " order by " + propOrdenacao + " " + dirOrdenacao;
                }
            }

            if (maximoRegistros > 0)
                query += " OFFSET " + inicioRegistros + " ROWS FETCH FIRST " + maximoRegistros + " ROWS ONLY";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Frota.RetornoAbastecimentoAngellira)));

            return nhQuery.SetTimeout(6000).List<Dominio.Relatorios.Embarcador.DataSource.Frota.RetornoAbastecimentoAngellira>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Frota.RetornoAbastecimentoAngellira>> RelatorioRetornoAbastecimentoAngelliraAsync(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioRetornoAbastecimentoAngellira filtrosPesquisa, string propGrupo, string dirOrdenacaoGrupo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            string query = @"SELECT Retorno.RBA_CODIGO Codigo,
                Consulta.CAA_DATA_CONSULTA DataConsulta,
                Consulta.CAA_DATA_INICIAL DataInicial,
                Consulta.CAA_DATA_FINAL DataFinal,
                Consulta.CAA_MENSAGEM_RETORNO Retorno,
                Veiculo.VEI_PLACA Placa,
                Veiculo.VEI_NUMERO_FROTA Frota,
                Retorno.RBA_PLACA PlacaRetorno,
                Retorno.RBA_CONDUTOR CondutorRetorno,
                Retorno.RBA_DATA_HORA DataRetorno,
                Retorno.RBA_CORDENADA CordenadaRetorno,
                Retorno.RBA_LATITUDE LatitudeRetorno,
                Retorno.RBA_LONGITUDE LontitudeRetorno,
                Retorno.RBA_ODOMETRO Odometro,
                Abastecimento.ABA_CODIGO CodigoAbastecimento,
                CASE
	                WHEN Abastecimento.ABA_SITUACAO = 'A' THEN 'Aberto'
	                WHEN Abastecimento.ABA_SITUACAO = 'F' THEN 'Fechado'
                    WHEN Abastecimento.ABA_SITUACAO = 'G' THEN 'Agrupado'
	                ELSE 'Inconsistente'
                END SituacaoAbastecimento, 
                Motorista.FUN_NOME NomeMotorista,
                Motorista.FUN_CPF CPFMotorista,
                Posto.CLI_FISJUR TipoPosto,
                Posto.CLI_CGCCPF CNPJCPFPosto,
                Posto.CLI_NOME NomePosto,
                Retorno.RBA_SITUACAO_INTEGRACAO SituacaoIntegracao
                FROM T_RETORNO_CONSULTA_ABASTECIMENTO_ANGELLIRA Retorno                
                JOIN T_CONSULTA_ABASTECIMENTO_ANGELLIRA Consulta on Retorno.CAA_CODIGO = Consulta.CAA_CODIGO
				JOIN T_CONSULTA_ABASTECIMENTO_ANGELLIRA_VEICULO ConsultaVeiculo on ConsultaVeiculo.CAA_CODIGO = Consulta.CAA_CODIGO and Retorno.VEI_CODIGO = ConsultaVeiculo.VEI_CODIGO
                JOIN T_VEICULO Veiculo on Veiculo.VEI_CODIGO = ConsultaVeiculo.VEI_CODIGO
                LEFT OUTER JOIN T_ABASTECIMENTO Abastecimento on Abastecimento.ABA_CODIGO = Retorno.ABA_CODIGO
                LEFT OUTER JOIN T_FUNCIONARIO Motorista on Motorista.FUN_CODIGO = Retorno.FUN_CODIGO
                LEFT OUTER JOIN T_CLIENTE Posto on Posto.CLI_CGCCPF = Retorno.CLI_CGCCPF WHERE Retorno.RBA_ODOMETRO > 0 ";

            if (filtrosPesquisa.DataInicial > DateTime.MinValue && filtrosPesquisa.DataFinal > DateTime.MinValue)
                query += " AND Retorno.RBA_DATA_HORA>= '" + filtrosPesquisa.DataInicial.ToString("MM/dd/yyyy HH:mm") + "' AND Retorno.RBA_DATA_HORA <= '" + filtrosPesquisa.DataFinal.ToString("MM/dd/yyyy HH:mm") + "'";
            else if (filtrosPesquisa.DataInicial > DateTime.MinValue && filtrosPesquisa.DataFinal == DateTime.MinValue)
                query += " AND Retorno.RBA_DATA_HORA >= '" + filtrosPesquisa.DataInicial.ToString("MM/dd/yyyy HH:mm") + "' ";
            else if (filtrosPesquisa.DataInicial == DateTime.MinValue && filtrosPesquisa.DataFinal > DateTime.MinValue)
                query += " AND Retorno.RBA_DATA_HORA <= '" + filtrosPesquisa.DataFinal.ToString("MM/dd/yyyy HH:mm") + "' ";

            if (filtrosPesquisa.Fornecedor > 0)
                query += " AND Posto.CLI_CGCCPF = '" + filtrosPesquisa.Fornecedor.ToString() + "'";
            if (filtrosPesquisa.CodigoVeiculo > 0)
                query += " AND ConsultaVeiculo.VEI_CODIGO = " + filtrosPesquisa.CodigoVeiculo.ToString();
            if (filtrosPesquisa.CodigoMotorista > 0)
                query += " AND Retorno.FUN_CODIGO = " + filtrosPesquisa.CodigoMotorista.ToString();

            var agrup = false;
            if (!string.IsNullOrWhiteSpace(propGrupo))
            {
                agrup = true;
                query += " order by " + propGrupo + " " + dirOrdenacaoGrupo;
            }

            if (!string.IsNullOrWhiteSpace(propOrdenacao) && propGrupo != propOrdenacao)
            {
                if (agrup)
                {
                    query += ", " + propOrdenacao + " " + dirOrdenacao;
                }
                else
                {
                    query += " order by " + propOrdenacao + " " + dirOrdenacao;
                }
            }

            if (maximoRegistros > 0)
                query += " OFFSET " + inicioRegistros + " ROWS FETCH FIRST " + maximoRegistros + " ROWS ONLY";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Frota.RetornoAbastecimentoAngellira)));

            return await nhQuery.SetTimeout(6000).ListAsync<Dominio.Relatorios.Embarcador.DataSource.Frota.RetornoAbastecimentoAngellira>();
        }


        public int ContarRelatorioRetornoAbastecimentoAngellira(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioRetornoAbastecimentoAngellira filtrosPesquisa)
        {
            string query = @"   SELECT COUNT(0) as CONTADOR 
                                FROM T_RETORNO_CONSULTA_ABASTECIMENTO_ANGELLIRA Retorno                
                                JOIN T_CONSULTA_ABASTECIMENTO_ANGELLIRA Consulta on Retorno.CAA_CODIGO = Consulta.CAA_CODIGO
				                JOIN T_CONSULTA_ABASTECIMENTO_ANGELLIRA_VEICULO ConsultaVeiculo on ConsultaVeiculo.CAA_CODIGO = Consulta.CAA_CODIGO and Retorno.VEI_CODIGO = ConsultaVeiculo.VEI_CODIGO
                                JOIN T_VEICULO Veiculo on Veiculo.VEI_CODIGO = ConsultaVeiculo.VEI_CODIGO
                                LEFT OUTER JOIN T_ABASTECIMENTO Abastecimento on Abastecimento.ABA_CODIGO = Retorno.ABA_CODIGO
                                LEFT OUTER JOIN T_FUNCIONARIO Motorista on Motorista.FUN_CODIGO = Retorno.FUN_CODIGO
                                LEFT OUTER JOIN T_CLIENTE Posto on Posto.CLI_CGCCPF = Retorno.CLI_CGCCPF
                                WHERE Retorno.RBA_ODOMETRO > 0 ";

            if (filtrosPesquisa.DataInicial > DateTime.MinValue && filtrosPesquisa.DataFinal > DateTime.MinValue)
                query += " AND Retorno.RBA_DATA_HORA>= '" + filtrosPesquisa.DataInicial.ToString("MM/dd/yyyy HH:mm") + "' AND Retorno.RBA_DATA_HORA <= '" + filtrosPesquisa.DataFinal.ToString("MM/dd/yyyy HH:mm") + "'";
            else if (filtrosPesquisa.DataInicial > DateTime.MinValue && filtrosPesquisa.DataFinal == DateTime.MinValue)
                query += " AND Retorno.RBA_DATA_HORA >= '" + filtrosPesquisa.DataInicial.ToString("MM/dd/yyyy HH:mm") + "' ";
            else if (filtrosPesquisa.DataInicial == DateTime.MinValue && filtrosPesquisa.DataFinal > DateTime.MinValue)
                query += " AND Retorno.RBA_DATA_HORA <= '" + filtrosPesquisa.DataFinal.ToString("MM/dd/yyyy HH:mm") + "' ";

            if (filtrosPesquisa.Fornecedor > 0)
                query += " AND Posto.CLI_CGCCPF = '" + filtrosPesquisa.Fornecedor.ToString() + "'";
            if (filtrosPesquisa.CodigoVeiculo > 0)
                query += " AND ConsultaVeiculo.VEI_CODIGO = " + filtrosPesquisa.CodigoVeiculo.ToString();
            if (filtrosPesquisa.CodigoMotorista > 0)
                query += " AND Retorno.FUN_CODIGO = " + filtrosPesquisa.CodigoMotorista.ToString();

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.SetTimeout(6000).UniqueResult<int>();
        }

        #endregion

        #region Relatório de Abastecimento por Nota de Entrada

        public IList<Dominio.Relatorios.Embarcador.DataSource.Frota.AbastecimentoNotaEntrada> ConsultarRelatorioAbastecimentoNotaEntrada(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, DateTime dataInicialEmissaoNota, DateTime dataFinalEmissaoNota, int veiculo, int produto, double fornecedor, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string sql = ObterSelectConsultaRelatorioAbastecimentoNotaEntrada(codigoEmpresa, dataInicial, dataFinal, dataInicialEmissaoNota, dataFinalEmissaoNota, veiculo, produto, fornecedor, false, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Frota.AbastecimentoNotaEntrada)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Frota.AbastecimentoNotaEntrada>();
        }

        public int ContarConsultaRelatorioAbastecimentoNotaEntrada(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, DateTime dataInicialEmissaoNota, DateTime dataFinalEmissaoNota, int veiculo, int produto, double fornecedor, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena)
        {
            string sql = ObterSelectConsultaRelatorioAbastecimentoNotaEntrada(codigoEmpresa, dataInicial, dataFinal, dataInicialEmissaoNota, dataFinalEmissaoNota, veiculo, produto, fornecedor, true, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, 0, 0);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        private string ObterSelectConsultaRelatorioAbastecimentoNotaEntrada(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, DateTime dataInicialEmissaoNota, DateTime dataFinalEmissaoNota, int veiculo, int produto, double fornecedor, bool count, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string select = string.Empty,
                   groupBy = string.Empty,
                   joins = string.Empty,
                   where = string.Empty,
                   orderBy = string.Empty;

            for (var i = propriedades.Count - 1; i >= 0; i--)
                SetarSelectRelatorioConsultaRelatorioAbastecimentoNotaEntrada(propriedades[i].Propriedade, propriedades[i].CodigoDinamico, ref select, ref groupBy, ref joins, count);

            SetarWhereRelatorioConsultaRelatorioAbastecimentoNotaEntrada(ref where, ref groupBy, ref joins, codigoEmpresa, dataInicial, dataFinal, dataInicialEmissaoNota, dataFinalEmissaoNota, veiculo, produto, fornecedor);

            if (!count)
            {
                if (!string.IsNullOrWhiteSpace(propAgrupa))
                {
                    SetarSelectRelatorioConsultaRelatorioAbastecimentoNotaEntrada(propAgrupa, 0, ref select, ref groupBy, ref joins, count);

                    if (select.Contains(propAgrupa))
                        orderBy = propAgrupa + " " + dirAgrupa;
                }

                if (!string.IsNullOrWhiteSpace(propOrdena))
                {
                    if (propOrdena != propAgrupa && select.Contains(propOrdena) && propOrdena != "Codigo")
                        orderBy += (orderBy.Length > 0 ? ", " : string.Empty) + propOrdena + " " + dirOrdena;
                }
            }


            // SELECT
            string query = "SELECT ";

            if (count)
                query += "DISTINCT(COUNT(0) OVER())";
            else if (select.Length > 0)
                query += select.Substring(0, select.Length - 2);

            // FROM
            query += " FROM T_ABASTECIMENTO Abastecimento ";

            // JOIN
            query += joins;

            // WHERE
            query += " WHERE 1 = 1" + where;

            // GROUP BY
            if (groupBy.Length > 0)
                query += " GROUP BY " + groupBy.Substring(0, groupBy.Length - 2);

            // ORDER BY
            if (orderBy.Length > 0)
                query += " ORDER BY " + orderBy;
            else if (!count)
                query += " ORDER BY 1 ASC";

            // LIMIT
            if (!count && limite > 0)
                query += " OFFSET " + inicio.ToString() + " ROWS FETCH NEXT " + limite.ToString() + " ROWS ONLY";

            return query;
        }

        private void SetarSelectRelatorioConsultaRelatorioAbastecimentoNotaEntrada(string propriedade, int codigoDinamico, ref string select, ref string groupBy, ref string joins, bool count)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select += "Abastecimento.ABA_CODIGO Codigo, ";
                        groupBy += "Abastecimento.ABA_CODIGO, ";
                    }
                    break;
                case "Data":
                    if (!select.Contains(" Data, "))
                    {
                        select += "Abastecimento.ABA_DATA Data, ";
                        groupBy += "Abastecimento.ABA_DATA, ";
                    }
                    break;
                case "Veiculo":
                    if (!select.Contains(" Veiculo, "))
                    {
                        if (!joins.Contains(" Veiculo "))
                            joins += " LEFT JOIN T_VEICULO Veiculo ON Veiculo.VEI_CODIGO = Abastecimento.VEI_CODIGO";

                        select += "Veiculo.VEI_PLACA Veiculo, ";
                        groupBy += "Veiculo.VEI_PLACA, ";
                    }
                    break;
                case "Produto":
                    if (!select.Contains(" Produto, "))
                    {
                        if (!joins.Contains(" Produto "))
                            joins += " LEFT JOIN T_PRODUTO Produto ON Produto.PRO_CODIGO = Abastecimento.PRO_CODIGO";

                        select += "Produto.PRO_DESCRICAO Produto, ";
                        groupBy += "Produto.PRO_DESCRICAO, ";
                    }
                    break;
                case "Posto":
                    if (!select.Contains(" Posto, "))
                    {
                        if (!joins.Contains(" Posto "))
                            joins += " LEFT JOIN T_CLIENTE Posto ON Posto.CLI_CGCCPF = Abastecimento.CLI_CGCCPF";

                        select += "Posto.CLI_NOME Posto, ";
                        groupBy += "Posto.CLI_NOME, ";
                    }
                    break;
                case "KM":
                    if (!select.Contains(" KM, "))
                    {
                        select += "CAST(Abastecimento.ABA_KM AS INTEGER) KM, ";
                        groupBy += "Abastecimento.ABA_KM, ";
                    }
                    break;
                case "Litros":
                    if (!select.Contains(" Litros, "))
                    {
                        select += "Abastecimento.ABA_LITROS Litros, ";
                        groupBy += "Abastecimento.ABA_LITROS, ";
                    }
                    break;
                case "ValorUnitario":
                    if (!select.Contains(" ValorUnitario, "))
                    {
                        select += "Abastecimento.ABA_VALOR_UN ValorUnitario, ";
                        groupBy += "Abastecimento.ABA_VALOR_UN, ";
                    }
                    break;
                case "ValorTotal":
                    if (!select.Contains(" ValorTotal, "))
                    {
                        select += "(Abastecimento.ABA_LITROS * Abastecimento.ABA_VALOR_UN) ValorTotal, ";
                        groupBy += "Abastecimento.ABA_VALOR_UN, Abastecimento.ABA_LITROS, ";
                    }
                    break;
                case "NumeroNF":
                    if (!select.Contains(" NumeroNF, "))
                    {
                        if (!joins.Contains(" DocumentoEntradaItem "))
                            joins += " LEFT JOIN T_TMS_DOCUMENTO_ENTRADA_ITEM DocumentoEntradaItem ON DocumentoEntradaItem.PRO_CODIGO = Abastecimento.PRO_CODIGO";

                        if (!joins.Contains(" DocumentoEntrada "))
                            joins += " LEFT JOIN T_TMS_DOCUMENTO_ENTRADA DocumentoEntrada ON DocumentoEntrada.TDE_CODIGO = DocumentoEntradaItem.TDE_CODIGO";

                        select += "DocumentoEntrada.TDE_NUMERO_LONG NumeroNF, ";
                        groupBy += "DocumentoEntrada.TDE_NUMERO_LONG, ";
                    }
                    break;
                case "DataNF":
                    if (!select.Contains(" DataNF, "))
                    {
                        if (!joins.Contains(" DocumentoEntradaItem "))
                            joins += " LEFT JOIN T_TMS_DOCUMENTO_ENTRADA_ITEM DocumentoEntradaItem ON DocumentoEntradaItem.PRO_CODIGO = Abastecimento.PRO_CODIGO";

                        if (!joins.Contains(" DocumentoEntrada "))
                            joins += " LEFT JOIN T_TMS_DOCUMENTO_ENTRADA DocumentoEntrada ON DocumentoEntrada.TDE_CODIGO = DocumentoEntradaItem.TDE_CODIGO";

                        select += "DocumentoEntrada.TDE_DATA_EMISSAO DataNF, ";
                        groupBy += "DocumentoEntrada.TDE_DATA_EMISSAO, ";
                    }
                    break;
                case "QuantidadeNF":
                    if (!select.Contains(" QuantidadeNF, "))
                    {
                        if (!joins.Contains(" DocumentoEntradaItem "))
                            joins += " LEFT JOIN T_TMS_DOCUMENTO_ENTRADA_ITEM DocumentoEntradaItem ON DocumentoEntradaItem.PRO_CODIGO = Abastecimento.PRO_CODIGO";

                        select += "DocumentoEntradaItem.TDI_QUANTIDADE QuantidadeNF, ";
                        groupBy += "DocumentoEntradaItem.TDI_QUANTIDADE, ";
                    }
                    break;
                case "ValorUnitarioNF":
                    if (!select.Contains(" ValorUnitarioNF, "))
                    {
                        if (!joins.Contains(" DocumentoEntradaItem "))
                            joins += " LEFT JOIN T_TMS_DOCUMENTO_ENTRADA_ITEM DocumentoEntradaItem ON DocumentoEntradaItem.PRO_CODIGO = Abastecimento.PRO_CODIGO";

                        select += "DocumentoEntradaItem.TDI_VALOR_UNITARIO ValorUnitarioNF, ";
                        groupBy += "DocumentoEntradaItem.TDI_VALOR_UNITARIO, ";
                    }
                    break;
                case "ValorTotalNF":
                    if (!select.Contains(" ValorTotalNF, "))
                    {
                        if (!joins.Contains(" DocumentoEntradaItem "))
                            joins += " LEFT JOIN T_TMS_DOCUMENTO_ENTRADA_ITEM DocumentoEntradaItem ON DocumentoEntradaItem.PRO_CODIGO = Abastecimento.PRO_CODIGO";

                        select += "DocumentoEntradaItem.TDI_VALOR_TOTAL ValorTotalNF, ";
                        groupBy += "DocumentoEntradaItem.TDI_VALOR_TOTAL, ";
                    }
                    break;

                default:
                    break;
            }
        }

        private void SetarWhereRelatorioConsultaRelatorioAbastecimentoNotaEntrada(ref string where, ref string groupBy, ref string joins, int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, DateTime dataInicialEmissaoNota, DateTime dataFinalEmissaoNota, int veiculo, int produto, double fornecedor)
        {
            string pattern = "yyyy-MM-dd";

            if (!joins.Contains(" DocumentoEntradaItem "))
                joins += " LEFT JOIN T_TMS_DOCUMENTO_ENTRADA_ITEM DocumentoEntradaItem ON DocumentoEntradaItem.PRO_CODIGO = Abastecimento.PRO_CODIGO";

            if (!joins.Contains(" DocumentoEntrada "))
                joins += " LEFT JOIN T_TMS_DOCUMENTO_ENTRADA DocumentoEntrada ON DocumentoEntrada.TDE_CODIGO = DocumentoEntradaItem.TDE_CODIGO";

            where += " AND DocumentoEntrada.CLI_CGCCPF = Abastecimento.CLI_CGCCPF";
            where += " AND DocumentoEntradaItem.PRO_CODIGO = Abastecimento.PRO_CODIGO";
            where += " AND DocumentoEntradaItem.VEI_CODIGO = Abastecimento.VEI_CODIGO";
            where += " AND DocumentoEntradaItem.TDI_KM_ABASTECIMENTO = CAST(Abastecimento.ABA_KM AS INTEGER)";

            if (codigoEmpresa > 0)
                where += " AND Abastecimento.EMP_CODIGO = '" + codigoEmpresa.ToString() + "' ";

            if (dataInicial != DateTime.MinValue)
                where += " AND Abastecimento.ABA_DATA >= '" + dataInicial.ToString(pattern) + "' ";

            if (dataFinal != DateTime.MinValue)
                where += " AND Abastecimento.ABA_DATA <= '" + dataFinal.AddDays(1).ToString(pattern) + "'";

            if (dataInicialEmissaoNota != DateTime.MinValue)
                where += " AND DocumentoEntrada.TDE_DATA_EMISSAO >= '" + dataInicialEmissaoNota.ToString(pattern) + "' ";

            if (dataFinalEmissaoNota != DateTime.MinValue)
                where += " AND DocumentoEntrada.TDE_DATA_EMISSAO <= '" + dataFinalEmissaoNota.AddDays(1).ToString(pattern) + "'";

            if (veiculo > 0)
                where += " AND Abastecimento.VEI_CODIGO = " + veiculo;

            if (produto > 0)
                where += " AND Abastecimento.PRO_CODIGO = " + produto;

            if (fornecedor > 0)
                where += " AND Abastecimento.CLI_CGCCPF = " + fornecedor;
        }

        #endregion
    }
}