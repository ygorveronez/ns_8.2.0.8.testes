using NHibernate.Criterion;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio
{
    public class Empresa : RepositorioBase<Dominio.Entidades.Empresa>, Dominio.Interfaces.Repositorios.Empresa
    {
        #region Construtores

        public Empresa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Empresa(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Empresa BuscarPorCodigo(int codigo)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>().Where(obj => obj.Codigo == codigo).FirstOrDefault();
        }

        //TODO: ct default
        public async Task<Dominio.Entidades.Empresa> BuscarPorCodigoAsync(int codigo, CancellationToken cancellationToken = default)
        {
            return await this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>().Where(obj => obj.Codigo == codigo).FirstOrDefaultAsync(cancellationToken);
        }
        public Dominio.Entidades.Empresa BuscarPorCodigo(int codigo, List<Dominio.Entidades.Empresa> lstEmpresas = null)
        {
            if (lstEmpresas != null)
                return lstEmpresas.Where(obj => obj.Codigo == codigo).FirstOrDefault();
            return this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>().Where(obj => obj.Codigo == codigo).FirstOrDefault();
        }

        public Dominio.Entidades.Empresa BuscarPorCodigoFetch(int codigo)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            IQueryable<Dominio.Entidades.Empresa> result = from obj in query where obj.Codigo == codigo select obj;
            return result.Fetch(obj => obj.Filiais).FirstOrDefault();
        }

        public Dominio.Entidades.Empresa BuscarPorCNPJEInscricaoEstadual(string cnpj, string inscricaoEstadual)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            IQueryable<Dominio.Entidades.Empresa> result = from obj in query
                                                           where obj.CNPJ == cnpj &&
                                                                 obj.InscricaoEstadual == inscricaoEstadual
                                                           select obj;

            return result.FirstOrDefault();
        }


        public Dominio.Entidades.Empresa BuscarPorTokenIntegracao(string token)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            IQueryable<Dominio.Entidades.Empresa> result = from obj in query where obj.Configuracao.TokenIntegracaoCTe != string.Empty && token.Contains(obj.Configuracao.TokenIntegracaoCTe) select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Empresa> BuscarPorCodigos(List<int> codigos)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            IQueryable<Dominio.Entidades.Empresa> result = from obj in query where codigos.Contains(obj.Codigo) select obj;
            return result
                .Fetch(obj => obj.Localidade)
                .ThenFetch(obj => obj.Estado)
                .ThenFetch(obj => obj.Pais)
                .Fetch(obj => obj.Filiais)
                .ToList();
        }
        public async Task<List<Dominio.Entidades.Empresa>> BuscarPorCodigosAsync(List<int> codigos)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            IQueryable<Dominio.Entidades.Empresa> result = from obj in query where codigos.Contains(obj.Codigo) select obj;
            return await result
                .Fetch(obj => obj.Localidade)
                .ThenFetch(obj => obj.Estado)
                .ThenFetch(obj => obj.Pais)
                .Fetch(obj => obj.Filiais)
                .ToListAsync();
        }

        public bool EmpresaAtivaDocumentosDestinados(int codigo)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            return query.Any(obj => obj.Codigo == codigo && obj.Status.Equals("A") && obj.HabilitaSincronismoDocumentosDestinados);
        }

        public List<int> BuscarCodigosEmpresasAtivasDocumentosDestinados()
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            IQueryable<int> result = from obj in query where obj.Status.Equals("A") && obj.HabilitaSincronismoDocumentosDestinados select obj.Codigo;
            return result.ToList();
        }

        public List<int> BuscarCodigosEmpresasCTFAtivas()
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            IQueryable<int> result = from obj in query where obj.Status.Equals("A") && obj.Configuracao != null && obj.Configuracao.LoginCTF != "" select obj.Codigo;
            return result.ToList();
        }

        public List<int> BuscarCodigosEmpresasAtivas(List<string> cnpjsNaoImportar)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();

            query = query.Where(obj => obj.Status.Equals("A") && !cnpjsNaoImportar.Contains(obj.CNPJ) && obj.HabilitaSincronismoDocumentosDestinados);

            return query.Select(o => o.Codigo).ToList();
        }

        public List<Dominio.Entidades.Empresa> BuscarEmpresasEmissaoNaturaAutomatico()
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            IQueryable<Dominio.Entidades.Empresa> result = from obj in query where obj.Status.Equals("A") && obj.Configuracao.EmitirNaturaAutomatico select obj;
            return result.ToList();
        }

        public List<int> BuscarCodigosEmpresasAtivasSincronismoDocumentosDestinados()
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            IQueryable<int> result = from obj in query where obj.Status.Equals("A") && obj.HabilitaSincronismoDocumentosDestinados == true && obj.DataFinalCertificado.Value > DateTime.Now select obj.Codigo;
            return result.ToList();
        }

        public List<Dominio.Entidades.Empresa> BuscarPontuacaoMaiorQueZero()
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            IQueryable<Dominio.Entidades.Empresa> result = from obj in query where obj.PontuacaoFixa > 0 select obj;

            return result.ToList();
        }

        public List<int> BuscarCodigosEmpresasAtivasIntegracaoDocumentosDestinados()
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            IQueryable<int> result = from obj in query where obj.Status.Equals("A") && obj.HabilitaSincronismoDocumentosDestinados == true && obj.UtilizaIntegracaoDocumentosDestinado == true && obj.DataFinalCertificado.Value > DateTime.Now select obj.Codigo;
            return result.ToList();
        }

        public List<int> BuscarCodigosEmpresasAtivasIntegracaoEmail()
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            IQueryable<int> result = from obj in query where obj.Status.Equals("A") && obj.HabilitaSincronismoDocumentosDestinados == true && obj.UtilizaIntegracaoDocumentosDestinado == true select obj.Codigo;
            return result.ToList();
        }

        public Dominio.Entidades.Empresa BuscarEmpresaPadraoEmissao()
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            IQueryable<Dominio.Entidades.Empresa> result = from obj in query where obj.Status.Equals("A") && obj.NomeCertificado != null && obj.NomeCertificado != "" select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Empresa> BuscarAtivasProducao(int codigoEmpresaPai)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            IQueryable<Dominio.Entidades.Empresa> result = from obj in query where obj.Status.Equals("A") && obj.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao select obj;

            if (codigoEmpresaPai > 0)
                result = result.Where(o => o.EmpresaPai.Codigo == codigoEmpresaPai);

            return result.ToList();
        }

        public List<int> BuscarEmpresasConfiguradasEmissaoNFSAutomatica()
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            IQueryable<Dominio.Entidades.Empresa> result = from obj in query where obj.Status.Equals("A") select obj;
            result = result.Where(c => c.TipoEmissaoIntramunicipal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoIntramunicipal.SempreNFSManual);
            result = result.Where(c => c.PeriodicidadeEmissaoNFSManual == Dominio.ObjetosDeValor.Embarcador.Enumeradores.Periodicidade.Diario
                || c.PeriodicidadeEmissaoNFSManual == Dominio.ObjetosDeValor.Embarcador.Enumeradores.Periodicidade.Semanal
                || c.PeriodicidadeEmissaoNFSManual == Dominio.ObjetosDeValor.Embarcador.Enumeradores.Periodicidade.Mensal);

            return result.Select(c => c.Codigo).ToList();
        }

        public List<Dominio.Entidades.Empresa> BuscarEmpresasComANTTInvalida(int codigoEmpresaPai)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            IQueryable<Dominio.Entidades.Empresa> result = from obj in query
                                                           where obj.Status.Equals("A")
                                         && (obj.RegistroANTT == "12345678" || obj.RegistroANTT == "88888888" || obj.RegistroANTT == "99999999" || obj.RegistroANTT == "11111111")
                                                           select obj;

            if (codigoEmpresaPai > 0)
                result = result.Where(o => o.EmpresaPai.Codigo == codigoEmpresaPai);

            return result.ToList();
        }

        public List<Dominio.Entidades.Empresa> BuscarComPlano(int codigoEmpresaPai, int codigoEmpresa, Dominio.Enumeradores.TipoAmbiente ambiente)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            IQueryable<Dominio.Entidades.Empresa> result = from obj in query where obj.Status.Equals("A") && obj.TipoAmbiente == ambiente && obj.PlanoEmissaoCTe != null select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Codigo == codigoEmpresa);

            if (codigoEmpresaPai > 0)
                result = result.Where(o => o.EmpresaPai.Codigo == codigoEmpresaPai);

            return result.OrderBy(o => o.RazaoSocial).ToList();
        }

        public List<Dominio.Entidades.Empresa> BuscarAtivas()
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            IQueryable<Dominio.Entidades.Empresa> result = from obj in query where obj.Status.Equals("A") select obj;
            return result.ToList();
        }


        public Task<List<Dominio.Entidades.Empresa>> BuscarAtivasAsync()
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            IQueryable<Dominio.Entidades.Empresa> result = from obj in query where obj.Status.Equals("A") select obj;
            return result.ToListAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Empresa> BuscarTransportadoresSemJanelaCarregamentoTransportadorPorCodigoJanelaCarregamento(int codigoJanelaCarregamento)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            IQueryable<Dominio.Entidades.Empresa> queryJanelaTransp = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador>()
                                        .Where(o => o.CargaJanelaCarregamento.Codigo == codigoJanelaCarregamento)
                                        .Select(o => o.Transportador);

            IQueryable<Dominio.Entidades.Empresa> result = from obj in query
                                                           where
                                                              obj.Status.Equals("A")
                                                              && !queryJanelaTransp.Any(o => o.Codigo == obj.Codigo)
                                                           select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Empresa> BuscarEmpresasParaCobranca(Dominio.Enumeradores.TipoAmbiente tipoAmbiente)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            IQueryable<Dominio.Entidades.Empresa> result = from obj in query where obj.Status.Equals("A") && obj.TipoAmbiente == tipoAmbiente select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Empresa> BuscarPorDataAtualizacao(DateTime dataInicial, DateTime dataFinal, int codigoEmpresa, int codigoEmpresaPai)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            IQueryable<Dominio.Entidades.Empresa> result = from obj in query where obj.Status.Equals("A") select obj;

            if (codigoEmpresaPai > 0)
                result = result.Where(o => o.EmpresaPai.Codigo == codigoEmpresaPai);

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Codigo == codigoEmpresa);
            else
            {
                if (dataInicial > DateTime.MinValue)
                    result = result.Where(o => o.DataAtualizacao >= dataInicial);

                if (dataFinal > DateTime.MinValue)
                    result = result.Where(o => o.DataAtualizacao <= dataFinal.AddDays(1));
            }

            return result.ToList();

        }

        public List<Dominio.Entidades.Empresa> BuscarPorDataCadastro(DateTime dataInicial, DateTime dataFinal, int codigoEmpresa, int codigoEmpresaPai)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            IQueryable<Dominio.Entidades.Empresa> result = from obj in query where obj.Status.Equals("A") select obj;

            if (codigoEmpresaPai > 0)
                result = result.Where(o => o.EmpresaPai.Codigo == codigoEmpresaPai);

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Codigo == codigoEmpresa);
            else
            {
                if (dataInicial > DateTime.MinValue)
                    result = result.Where(o => o.DataCadastro >= dataInicial);

                if (dataFinal > DateTime.MinValue)
                    result = result.Where(o => o.DataCadastro <= dataFinal.AddDays(1));
            }

            return result.ToList();
        }

        public Dominio.Entidades.Empresa BuscarPorConfiguracao(int codigoConfiguracao)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            IQueryable<Dominio.Entidades.Empresa> result = from obj in query where obj.Status.Equals("A") && obj.Configuracao.Codigo == codigoConfiguracao select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Empresa BuscarPorCodigoTransportadorNatura(string codigoTranspFilialNatura)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            IQueryable<Dominio.Entidades.Empresa> result = from obj in query where obj.Configuracao.CodigoFilialNatura == codigoTranspFilialNatura select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Empresa BuscarPorCodigoEEmpresaPai(int codigo, int codigoEmpresaPai)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            IQueryable<Dominio.Entidades.Empresa> result = from obj in query where obj.Codigo == codigo && obj.EmpresaPai.Codigo == codigoEmpresaPai select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Empresa> BuscarEmpresasAvon()
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();

            IQueryable<Dominio.Entidades.Empresa> result = from obj in query where obj.Configuracao.UtilizaIntegracaoAvon select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Empresa> ConsultarTransportadoresPendentesIntegracao(string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();

            query = query.Where(obj => obj.Integrado == false);

            return query.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultaTransportadoresPendentesIntegracao()
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();

            query = query.Where(obj => obj.Integrado == false);

            return query.Count();
        }

        public List<Dominio.Entidades.Empresa> BuscarPorEmpresaPai(int codigoEmpresaPai)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();

            IQueryable<Dominio.Entidades.Empresa> result = from obj in query where obj.Codigo == codigoEmpresaPai || obj.EmpresaPai.Codigo == codigoEmpresaPai select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Empresa> BuscarPorEmpresaPaiEStatus(int codigoEmpresaPai, string status)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            IQueryable<Dominio.Entidades.Empresa> result = from obj in query where obj.Codigo == codigoEmpresaPai || obj.EmpresaPai.Codigo == codigoEmpresaPai select obj;

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            return result.OrderBy(o => o.NomeFantasia).ToList();
        }

        public List<Dominio.Entidades.Empresa> BuscarTodos(int codigoEmpresa, int maximoRegistros, int inicioRegistros)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            IQueryable<Dominio.Entidades.Empresa> result = (from obj in query where (obj.Codigo == codigoEmpresa || obj.EmpresaPai.Codigo == codigoEmpresa) && obj.Status == "A" select obj).Skip(inicioRegistros).Take(maximoRegistros);
            return result.ToList();
        }

        public int ContarTodos(int codigoEmpresa)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            IQueryable<int> result = from obj in query where (obj.Codigo == codigoEmpresa || obj.EmpresaPai.Codigo == codigoEmpresa) && obj.Status == "A" select obj.Codigo;
            return result.Count();
        }

        public List<int> BuscarCodigoMatrizEFiliais(string filtroCNPJ)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            IQueryable<int> result = from obj in query where obj.CNPJ == filtroCNPJ || obj.Matriz.Any(m => m.CNPJ == filtroCNPJ) select obj.Codigo;
            return result.ToList();
        }

        public async Task<List<int>> BuscarCodigoMatrizEFiliaisAsync(string filtroCNPJ)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            IQueryable<int> result = from obj in query where obj.CNPJ == filtroCNPJ || obj.Matriz.Any(m => m.CNPJ == filtroCNPJ) select obj.Codigo;
            return await result.ToListAsync();
        }

        public List<int> BuscarCodigoMatrizEFiliaisPorRaizCNPJ(string raizCNPJ)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            IQueryable<int> result = from obj in query where obj.CNPJ.StartsWith(raizCNPJ) || obj.Matriz.Any(m => m.CNPJ.StartsWith(raizCNPJ)) select obj.Codigo;
            return result.ToList();
        }

        public List<Dominio.Entidades.Empresa> BuscarComAnexosPorCodigo(int codigo)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            IQueryable<Dominio.Entidades.Empresa> result = from obj in query where obj.Codigo == codigo select obj;
            return result.Fetch(o => o.Anexos).ToList();
        }

        public List<string> BuscarDescricoesPorCodigos(List<int> codigos)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            IQueryable<string> result = from obj in query where codigos.Contains(obj.Codigo) select obj.NomeFantasia;
            return result.ToList();
        }

        public List<Dominio.Entidades.Empresa> Consultar(int codigoEmpresa, string nome, string cnpj, string placaVeiculo, string status, int inicioRegistros, int maximoRegistros, string propOrdenacao = "Codigo", string dirOrdenacao = "desc", bool somenteProducao = false, Dominio.Enumeradores.TipoAmbiente tipoAmbiente = Dominio.Enumeradores.TipoAmbiente.Producao, bool semEmpresaPai = false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor sistemaEmissor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.Todos, int filial = 0, string nomeFantasia = "", List<int> codigos = null, string ufFilialTransportador = "")
        {
            Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaTransportador filtro = new Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaTransportador()
            {
                Codigos = codigos,
                CodigoEmpresa = codigoEmpresa,
                RazaoSocial = nome,
                NomeFantasia = nomeFantasia,
                CNPJ = cnpj,
                PlacaVeiculo = placaVeiculo,
                SituacaoPesquisa = status == "A" ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo : status == "I" ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos,
                SomenteProducao = somenteProducao,
                TipoAmbiente = tipoAmbiente,
                SemEmpresaPai = semEmpresaPai,
                SistemaEmissor = sistemaEmissor,
                CodigoFilial = filial,
                UFFilialTransportador = ufFilialTransportador
            };
            IQueryable<Dominio.Entidades.Empresa> result = _Consultar(filtro);

            if (!string.IsNullOrWhiteSpace(propOrdenacao))
                result = result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result
                .Fetch(obj => obj.Localidade)
                .ThenFetch(obj => obj.Pais)
                .Fetch(obj => obj.Configuracao)
                 .Fetch(obj => obj.EmpresaPai)
                .ToList();
        }

        public int ContarConsulta(int codigoEmpresa, string nome, string cnpj, string placaVeiculo, string status, bool somenteProducao = false, Dominio.Enumeradores.TipoAmbiente tipoAmbiente = Dominio.Enumeradores.TipoAmbiente.Producao, bool semEmpresaPai = false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor sistemaEmissor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.Todos, int filial = 0, string nomeFantasia = "", List<int> codigos = null, string ufFilialTransportador = "")
        {
            Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaTransportador filtro = new Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaTransportador()
            {
                Codigos = codigos,
                CodigoEmpresa = codigoEmpresa,
                RazaoSocial = nome,
                NomeFantasia = nomeFantasia,
                CNPJ = cnpj,
                PlacaVeiculo = placaVeiculo,
                SituacaoPesquisa = status == "A" ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo : status == "I" ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos,
                SomenteProducao = somenteProducao,
                TipoAmbiente = tipoAmbiente,
                SemEmpresaPai = semEmpresaPai,
                SistemaEmissor = sistemaEmissor,
                CodigoFilial = filial,
                UFFilialTransportador = ufFilialTransportador
            };
            IQueryable<Dominio.Entidades.Empresa> result = _Consultar(filtro);

            return result.Count();
        }

        public List<Dominio.Entidades.Empresa> ConsultarAdmin(int codigoEmpresa, string nome, string cnpj, string placaVeiculo, string status, int inicioRegistros, int maximoRegistros, string propOrdenacao = "Codigo", string dirOrdenacao = "desc", bool somenteProducao = false, Dominio.Enumeradores.TipoAmbiente tipoAmbiente = Dominio.Enumeradores.TipoAmbiente.Producao, bool semEmpresaPai = false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor sistemaEmissor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.Todos, int filial = 0, string nomeFantasia = "", List<int> codigos = null, string ufFilialTransportador = "")
        {
            Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaTransportador filtro = new Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaTransportador()
            {
                Codigos = codigos,
                CodigoEmpresa = codigoEmpresa,
                RazaoSocial = nome,
                CNPJ = cnpj,
                PlacaVeiculo = placaVeiculo,
                SituacaoPesquisa = status == "A" || status == "Z" ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo : status == "I" ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos,
                SomenteProducao = somenteProducao,
                TipoAmbiente = tipoAmbiente,
                SemEmpresaPai = semEmpresaPai,
                SistemaEmissor = sistemaEmissor,
                CodigoFilial = filial,
                UFFilialTransportador = ufFilialTransportador
            };
            IQueryable<Dominio.Entidades.Empresa> result = _ConsultarAdmin(filtro);

            if (status == "Z")
                result = result.Where(o => o.EmpresaPai == null);

            if (!string.IsNullOrWhiteSpace(propOrdenacao))
                result = result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result
                .Fetch(obj => obj.Localidade)
                .ThenFetch(obj => obj.Pais)
                .Fetch(obj => obj.Configuracao)
                 .Fetch(obj => obj.EmpresaPai)
                .ToList();
        }

        public int ContarConsultaAdmin(int codigoEmpresa, string nome, string cnpj, string placaVeiculo, string status, bool somenteProducao = false, Dominio.Enumeradores.TipoAmbiente tipoAmbiente = Dominio.Enumeradores.TipoAmbiente.Producao, bool semEmpresaPai = false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor sistemaEmissor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.Todos, int filial = 0, string nomeFantasia = "", List<int> codigos = null, string ufFilialTransportador = "")
        {
            Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaTransportador filtro = new Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaTransportador()
            {
                Codigos = codigos,
                CodigoEmpresa = codigoEmpresa,
                RazaoSocial = nome,
                NomeFantasia = nomeFantasia,
                CNPJ = cnpj,
                PlacaVeiculo = placaVeiculo,
                SituacaoPesquisa = status == "A" || status == "Z" ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo : status == "I" ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos,
                SomenteProducao = somenteProducao,
                TipoAmbiente = tipoAmbiente,
                SemEmpresaPai = semEmpresaPai,
                SistemaEmissor = sistemaEmissor,
                CodigoFilial = filial,
                UFFilialTransportador = ufFilialTransportador
            };
            IQueryable<Dominio.Entidades.Empresa> result = _ConsultarAdmin(filtro);

            if (status == "Z")
                result = result.Where(o => o.EmpresaPai == null);

            return result.Count();
        }

        public List<Dominio.Entidades.Empresa> Consultar(Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaTransportador filtro, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            IQueryable<Dominio.Entidades.Empresa> result = _Consultar(filtro);

            result = result.OrderBy($"{parametrosConsulta.PropriedadeOrdenar} {(parametrosConsulta.DirecaoOrdenar == "asc" ? "ascending" : "descending")}");

            if (parametrosConsulta.LimiteRegistros > 0)
                result = result.Skip(parametrosConsulta.InicioRegistros).Take(parametrosConsulta.LimiteRegistros);

            return result
                .Fetch(obj => obj.Localidade)
                .ThenFetch(obj => obj.Pais)
                .Fetch(obj => obj.Configuracao)
                 .Fetch(obj => obj.EmpresaPai)
                .ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaTransportador filtro)
        {
            IQueryable<Dominio.Entidades.Empresa> result = _Consultar(filtro);

            return result.Count();
        }

        public IList<Dominio.Entidades.Empresa> Consultar(string nome, string cnpj, string placaVeiculo, string status, int inicioRegistros, int maximoRegistros, string propOrdenacao = "Codigo", string dirOrdenacao = "desc", bool SomenteProducao = false, Dominio.Enumeradores.TipoAmbiente tipoAmbiente = Dominio.Enumeradores.TipoAmbiente.Producao, string nomeFantasia = "", List<int> codigosEmpresas = null)
        {
            NHibernate.ICriteria criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.Empresa>();

            if (!string.IsNullOrWhiteSpace(status))
                criteria.Add(Restrictions.Eq("Status", status));

            if (!string.IsNullOrWhiteSpace(nome))
                criteria.Add(Restrictions.InsensitiveLike("RazaoSocial", nome, MatchMode.Anywhere));

            if (!string.IsNullOrWhiteSpace(nomeFantasia))
                criteria.Add(Restrictions.InsensitiveLike("NomeFantasia", nomeFantasia, MatchMode.Anywhere));

            if (!string.IsNullOrWhiteSpace(cnpj))
                criteria.Add(Restrictions.InsensitiveLike("CNPJ", cnpj, MatchMode.Anywhere));

            if (SomenteProducao)
                criteria.Add(Restrictions.Eq("TipoAmbiente", tipoAmbiente));

            if (!string.IsNullOrWhiteSpace(placaVeiculo))
            {
                DetachedCriteria subCriteriaVeiculos = DetachedCriteria.ForEntityName("Veiculo");

                subCriteriaVeiculos.Add(Restrictions.InsensitiveLike("Placa", placaVeiculo, MatchMode.Anywhere));
                //subCriteriaVeiculos.Add(Restrictions.Eq("Status", "A"));
                subCriteriaVeiculos.SetProjection(Projections.Property("Empresa.Codigo"));

                criteria.Add(Subqueries.PropertyIn("Codigo", subCriteriaVeiculos));
            }

            if (codigosEmpresas?.Count > 0)
                criteria.Add(Restrictions.In("Codigo", codigosEmpresas));

            if (dirOrdenacao == "asc")
                criteria.AddOrder(Order.Asc(propOrdenacao));
            else
                criteria.AddOrder(Order.Desc(propOrdenacao));

            criteria.SetFirstResult(inicioRegistros);
            criteria.SetMaxResults(maximoRegistros);

            return criteria.List<Dominio.Entidades.Empresa>();
        }

        public int ContarConsulta(string nome, string cnpj, string placaVeiculo, string status, bool SomenteProducao = false, Dominio.Enumeradores.TipoAmbiente tipoAmbiente = Dominio.Enumeradores.TipoAmbiente.Producao, string nomeFantasia = "", List<int> codigosEmpresas = null)
        {
            NHibernate.ICriteria criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.Empresa>();

            if (!string.IsNullOrWhiteSpace(status))
                criteria.Add(Restrictions.Eq("Status", status));

            if (!string.IsNullOrWhiteSpace(nome))
                criteria.Add(Restrictions.InsensitiveLike("RazaoSocial", nome, MatchMode.Anywhere));

            if (!string.IsNullOrWhiteSpace(nomeFantasia))
                criteria.Add(Restrictions.InsensitiveLike("NomeFantasia", nomeFantasia, MatchMode.Anywhere));

            if (!string.IsNullOrWhiteSpace(cnpj))
                criteria.Add(Restrictions.InsensitiveLike("CNPJ", cnpj, MatchMode.Anywhere));

            if (SomenteProducao)
                criteria.Add(Restrictions.Eq("TipoAmbiente", tipoAmbiente));

            if (!string.IsNullOrEmpty(placaVeiculo))
            {
                DetachedCriteria subCriteriaVeiculos = DetachedCriteria.ForEntityName("Veiculo");
                subCriteriaVeiculos.Add(Restrictions.InsensitiveLike("Placa", placaVeiculo, MatchMode.Anywhere));
                //subCriteriaVeiculos.Add(Restrictions.Eq("Status", "A"));
                subCriteriaVeiculos.SetProjection(Projections.Property("Empresa.Codigo"));
                criteria.Add(Subqueries.PropertyIn("Codigo", subCriteriaVeiculos));
            }

            if (codigosEmpresas?.Count > 0)
                criteria.Add(Restrictions.In("Codigo", codigosEmpresas));

            criteria.SetProjection(Projections.RowCount());

            return criteria.UniqueResult<int>();
        }

        public Dominio.Entidades.Empresa BuscarEmpresaPai()
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();

            IQueryable<Dominio.Entidades.Empresa> result = from obj in query where obj.EmpresaPai == null select obj;

            return result.FirstOrDefault();
        }

        public async Task<Dominio.Entidades.Empresa> BuscarEmpresaPaiAsync(CancellationToken cancellationToken = default)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();

            IQueryable<Dominio.Entidades.Empresa> result = from obj in query where obj.EmpresaPai == null select obj;

            return await result.FirstOrDefaultAsync(cancellationToken);
        }

        public Dominio.Entidades.Empresa BuscarEmpresaPadraoRetirada()
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();

            IQueryable<Dominio.Entidades.Empresa> result = from obj in query where obj.EmpresaRetiradaProduto == true select obj;

            return result.FirstOrDefault();
        }

        public bool ContemEmpresaCadastrada(string cnpj)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();

            IQueryable<Dominio.Entidades.Empresa> result = from obj in query where obj.CNPJ == cnpj select obj;

            return result.Any();
        }

        public List<Dominio.Entidades.Empresa> ConsultarEmpresasPai(int codigoEmpresaAdministradora, string nome, string cnpj, string status, int inicioRegistros, int maximoRegistros)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();

            IQueryable<Dominio.Entidades.Empresa> result = from obj in query where obj.EmpresaAdministradora.Codigo == codigoEmpresaAdministradora && obj.EmpresaPai == null select obj;

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            if (!string.IsNullOrWhiteSpace(nome))
                result = result.Where(o => o.RazaoSocial.Contains(nome) || o.NomeFantasia.Contains(nome));

            if (!string.IsNullOrWhiteSpace(cnpj))
                result = result.Where(o => o.CNPJ.Contains(cnpj));

            return result.OrderBy(o => o.RazaoSocial).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultaEmpresasPai(int codigoEmpresaAdministradora, string nome, string cnpj, string status)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();

            IQueryable<Dominio.Entidades.Empresa> result = from obj in query where obj.EmpresaAdministradora.Codigo == codigoEmpresaAdministradora && obj.EmpresaPai == null select obj;

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            if (!string.IsNullOrWhiteSpace(nome))
                result = result.Where(o => o.RazaoSocial.Contains(nome) || o.NomeFantasia.Contains(nome));

            if (!string.IsNullOrWhiteSpace(cnpj))
                result = result.Where(o => o.CNPJ.Contains(cnpj));

            return result.Count();
        }

        public int BuscarCodigoPorCNPJ(string cnpj)
        {
            IQueryable<Dominio.Entidades.Empresa> consultaEmpresa = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>()
                .Where(o => o.CNPJ.Equals(cnpj));

            return consultaEmpresa.Select(o => o.Codigo).FirstOrDefault();
        }

        public Dominio.Entidades.Empresa BuscarPorCNPJ(string cnpj, List<Dominio.Entidades.Empresa> lstEmpresa = null)
        {
            if (lstEmpresa != null && lstEmpresa.Count() > 0)
                return lstEmpresa.Where(obj => obj.CNPJ.Equals(cnpj)).FirstOrDefault();

            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            IQueryable<Dominio.Entidades.Empresa> result = from obj in query where obj.CNPJ.Equals(cnpj) select obj;
            return result.Fetch(o => o.Configuracao)
                         .Fetch(o => o.EmpresaPai)
                         .ThenFetch(o => o.Configuracao).FirstOrDefault();
        }
        public async Task<Dominio.Entidades.Empresa> BuscarPorCNPJAsync(string cnpj, List<Dominio.Entidades.Empresa> lstEmpresa = null)
        {
            if (lstEmpresa != null && lstEmpresa.Count() > 0)
                return lstEmpresa.Where(obj => obj.CNPJ.Equals(cnpj)).FirstOrDefault();

            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            IQueryable<Dominio.Entidades.Empresa> result = from obj in query where obj.CNPJ.Equals(cnpj) select obj;
            return await result.Fetch(o => o.Configuracao)
                         .Fetch(o => o.EmpresaPai)
                         .ThenFetch(o => o.Configuracao).FirstOrDefaultAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Empresa> BuscarPorCNPJs(List<string> lstCpfCnpjProprietario)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            IQueryable<Dominio.Entidades.Empresa> result = from obj in query where lstCpfCnpjProprietario.Contains(obj.CNPJ) select obj;
            return result.Fetch(o => o.Configuracao)
                         .Fetch(o => o.EmpresaPai)
                         .ThenFetch(o => o.Configuracao).ToList();
        }




        public bool ExistePorCNPJ(string cnpj)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            IQueryable<Dominio.Entidades.Empresa> result = from obj in query where obj.CNPJ.Equals(cnpj) select obj;
            return result.Any();
        }

        public Task<bool> ExistePorCNPJAsync(string cnpj)
        {
            if (string.IsNullOrEmpty(cnpj))
                return Task.FromResult(false);

            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            IQueryable<Dominio.Entidades.Empresa> result = from obj in query where obj.CNPJ.Equals(cnpj) select obj;
            return result.AnyAsync();
        }

        public Dominio.Entidades.Empresa BuscarPorDocumentacao(string Codigo)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            IQueryable<Dominio.Entidades.Empresa> result = from obj in query where obj.CodigoDocumento.Equals(Codigo) select obj;
            return result.Fetch(o => o.Configuracao)
                         .Fetch(o => o.EmpresaPai)
                         .ThenFetch(o => o.Configuracao).FirstOrDefault();
        }
        public Dominio.Entidades.Empresa BuscarPorCodigoIntegracao(string codigoIntegracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Transportadores.TransportadorCodigoIntegracao> queryCodigosIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.TransportadorCodigoIntegracao>()
                .Where(obj => obj.CodigoIntegracao == codigoIntegracao);

            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>()
                       .Where(obj => (obj.CodigoIntegracao == codigoIntegracao || queryCodigosIntegracao.Any(ci => ci.Empresa.Codigo == obj.Codigo)));

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Empresa> BuscarPorEmpresasCodigoIntegracao(string codigoIntegracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Transportadores.TransportadorCodigoIntegracao> queryCodigosIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.TransportadorCodigoIntegracao>()
                .Where(obj => obj.CodigoIntegracao == codigoIntegracao);

            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>()
                       .Where(obj => (obj.CodigoIntegracao == codigoIntegracao || queryCodigosIntegracao.Any(ci => ci.Empresa.Codigo == obj.Codigo)));

            return query.ToList();
        }


        public List<Dominio.Entidades.Empresa> BuscarPorCodigosIntegracao(List<string> codigoIntegracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Transportadores.TransportadorCodigoIntegracao> queryCodigosIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.TransportadorCodigoIntegracao>()
                .Where(obj => codigoIntegracao.Contains(obj.CodigoIntegracao));

            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>()
                       .Where(obj => (codigoIntegracao.Contains(obj.CodigoIntegracao) || queryCodigosIntegracao.Any(ci => ci.Empresa.Codigo == obj.Codigo)));

            return query.ToList();
        }

        public Dominio.Entidades.Empresa BuscarPorCodigoIntegracaoComFetch(string codigoIntegracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Transportadores.TransportadorCodigoIntegracao> queryCodigosIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.TransportadorCodigoIntegracao>()
                .Where(obj => obj.CodigoIntegracao == codigoIntegracao);

            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>()
                       .Where(obj => (obj.CodigoIntegracao == codigoIntegracao || queryCodigosIntegracao.Any(ci => ci.Empresa.Codigo == obj.Codigo)));

            query = query
                .Fetch(o => o.Localidade).ThenFetch(o => o.Estado);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Empresa BuscarPorCodigoComercialDistribuidor(string codigoDistribuidor)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            IQueryable<Dominio.Entidades.Empresa> result = from obj in query where obj.CodigosComercialDistribuidor.Contains(codigoDistribuidor) select obj;
            return result.Fetch(o => o.Configuracao)
                         .Fetch(o => o.EmpresaPai)
                         .ThenFetch(o => o.Configuracao).FirstOrDefault();
        }

        public Dominio.Entidades.Empresa BuscarPorCodigoComercialDistribuidorDuplicado(int codigoEmpresa, string codigoDistribuidor)
        {
            Dominio.Entidades.Empresa empresa = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>()
                .Where(o => (o.Codigo != codigoEmpresa) && o.CodigosComercialDistribuidor.Contains(codigoDistribuidor))
                .FirstOrDefault();

            return empresa;
        }

        public Dominio.Entidades.Empresa BuscarPorCNPJ(string cnpjEmpresaPai, string cnpjEmpresaAdministradora)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            IQueryable<Dominio.Entidades.Empresa> result = from obj in query where obj.CNPJ.Equals(cnpjEmpresaPai) && obj.EmpresaAdministradora.CNPJ.Equals(cnpjEmpresaAdministradora) select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Empresa BuscarEmpresaPorCNPJ(string cnpj)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            IQueryable<Dominio.Entidades.Empresa> result = from obj in query where obj.CNPJ.Equals(cnpj) select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Empresa BuscarPrincipalEmissoraTMS()
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            IQueryable<Dominio.Entidades.Empresa> result = from obj in query where obj.Configuracao.PrincipalFilialEmissoraTMS == true && obj.Status == "A" select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Empresa BuscarEmpresaPadraoLancamentoGuarita()
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            IQueryable<Dominio.Entidades.Empresa> result = from obj in query where obj.Configuracao.EmpresaPadraoLancamentoGuarita && obj.Status == "A" select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Empresa BuscarEmpresaMatriz()
        {
            IQueryable<Dominio.Entidades.Embarcador.Filiais.Filial> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.Filial>();
            IQueryable<string> result = from obj in query where obj.TipoFilial == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFilial.Matriz select obj.CNPJ;

            IQueryable<Dominio.Entidades.Empresa> queryEmpresa = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            IQueryable<Dominio.Entidades.Empresa> resultEmpresa = from obj in queryEmpresa where result.Contains(obj.CNPJ) select obj;

            return resultEmpresa.FirstOrDefault();
        }

        public Dominio.Entidades.Empresa BuscarEmpresaFilialEmissoraPadraoPorEstadoOrigemRedespacho(Dominio.Entidades.Estado estado)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();

            IQueryable<Dominio.Entidades.Empresa> result = from obj in query where obj.Localidade.Estado.Sigla == estado.Sigla && obj.UsarComoFilialEmissoraPadraoEmRedespachoIniciadosNoEstadoDaTransportadora && obj.Status == "A" select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Empresa BuscarEmpresaEmissoraEstado(Dominio.Entidades.Estado estado)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();

            IQueryable<Dominio.Entidades.Empresa> result = from obj in query where obj.Configuracao.EstadosDeEmissao.Contains(estado) && obj.Status == "A" select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Empresa BuscarEmpresaEmissoraPorUf(string UF)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();

            IQueryable<Dominio.Entidades.Empresa> result = from obj in query where obj.Localidade.Estado.Sigla == UF && obj.Status == "A" select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Empresa BuscarPorCNPJ(string cnpj, string cnpjEmpresaPai, string cnpjEmpresaAdministradora)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            IQueryable<Dominio.Entidades.Empresa> result = from obj in query where obj.CNPJ.Equals(cnpj) && obj.EmpresaPai.CNPJ.Equals(cnpjEmpresaPai) && obj.EmpresaPai.EmpresaAdministradora.CNPJ.Equals(cnpjEmpresaAdministradora) select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Empresa BuscarPorEmpresaAdministradora(string cnpjEmpresaEmissora, string cnpjEmpresaAdministradora)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            IQueryable<Dominio.Entidades.Empresa> result = from obj in query where obj.EmpresaPai.EmpresaAdministradora.CNPJ.Equals(cnpjEmpresaAdministradora) && obj.CNPJ.Equals(cnpjEmpresaEmissora) select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Empresa> BuscarPorEmpresasDaEmpresaCobradora(string cnpjEmpresaCobradora)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            IQueryable<Dominio.Entidades.Empresa> result = from obj in query where obj.EmpresaCobradora.CNPJ.Equals(cnpjEmpresaCobradora) select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Empresa BuscarPorEmpresaPai(int codigoEmpresaPai, string cnpjEmpresaEmissora)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            IQueryable<Dominio.Entidades.Empresa> result = from obj in query where obj.EmpresaPai.Codigo == codigoEmpresaPai && obj.CNPJ.Equals(cnpjEmpresaEmissora) select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Empresa> BuscarPorRazaoSocial(string razao)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            IQueryable<Dominio.Entidades.Empresa> result = from obj in query where obj.RazaoSocial.Contains(razao) select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Empresa BuscarPrimeiraPorRazaoSocial(string razao)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            IQueryable<Dominio.Entidades.Empresa> result = from obj in query where obj.RazaoSocial.Contains(razao) select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Empresa> BuscarPorDataDeVencimentoDoCertificado(int codigoEmpresaPai, DateTime dataVencimento)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();

            IQueryable<Dominio.Entidades.Empresa> result = from obj in query
                                                           where obj.DataFinalCertificado <= dataVencimento &&
                                                                 obj.Status == "A" &&
                                                                 obj.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao
                                                           select obj;

            if (codigoEmpresaPai > 0)
                result = result.Where(o => o.EmpresaPai.Codigo == codigoEmpresaPai);

            return result.OrderBy(o => o.DataFinalCertificado).ToList();
        }

        public List<Dominio.Entidades.Empresa> BuscarPorVencimentoAnttAlertar(Dominio.ObjetosDeValor.Embarcador.Notificacao.FiltroPesquisaAlerta filtrosPesquisa)
        {
            DateTime dataAtual = DateTime.Now.Date;
            DateTime dataVencimentoAnttAlertar = dataAtual.AddDays(filtrosPesquisa.DiasAlertarAntesVencimento);

            IQueryable<Dominio.Entidades.Empresa> consultaEmpresa = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>()
                .Where(o =>
                    o.Status == "A" &&
                    o.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao &&
                    o.DataValidadeANTT != null &&
                    o.DataValidadeANTT <= dataVencimentoAnttAlertar
                );

            if (!filtrosPesquisa.AlertarAposVencimento)
                consultaEmpresa = consultaEmpresa.Where(o => o.DataValidadeANTT >= dataAtual);

            if (filtrosPesquisa.DiasRepetirAlerta > 0)
            {
                DateTime dataVencimentoRepetirAlerta = dataAtual.AddDays(-filtrosPesquisa.DiasRepetirAlerta);

                consultaEmpresa = consultaEmpresa.Where(o => (o.DataUltimoAlertaVencimentoAntt == null) || (o.DataUltimoAlertaVencimentoAntt <= dataVencimentoRepetirAlerta));
            }
            else
                consultaEmpresa = consultaEmpresa.Where(o => o.DataUltimoAlertaVencimentoAntt == null);

            return consultaEmpresa.ToList();
        }

        public List<Dominio.Entidades.Empresa> BuscarPorVencimentoCertificadoDigitalAlertar(Dominio.ObjetosDeValor.Embarcador.Notificacao.FiltroPesquisaAlerta filtrosPesquisa)
        {
            DateTime dataAtual = DateTime.Now.Date;
            DateTime dataVencimentoCertificadoAlertar = dataAtual.AddDays(filtrosPesquisa.DiasAlertarAntesVencimento);

            IQueryable<Dominio.Entidades.Empresa> consultaEmpresa = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>()
                .Where(o =>
                    o.Status == "A" &&
                    o.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao &&
                    o.NomeCertificado != null &&
                    o.NomeCertificado != "" &&
                    o.DataFinalCertificado != null &&
                    o.DataFinalCertificado <= dataVencimentoCertificadoAlertar
                );

            if (!filtrosPesquisa.AlertarAposVencimento)
                consultaEmpresa = consultaEmpresa.Where(o => o.DataFinalCertificado >= dataAtual);

            if (filtrosPesquisa.DiasRepetirAlerta > 0)
            {
                DateTime dataVigenciaRepetirAlerta = dataAtual.AddDays(-filtrosPesquisa.DiasRepetirAlerta);

                consultaEmpresa = consultaEmpresa.Where(o => (o.DataUltimoAlertaVencimentoCertificado == null) || (o.DataUltimoAlertaVencimentoCertificado <= dataVigenciaRepetirAlerta));
            }
            else
                consultaEmpresa = consultaEmpresa.Where(o => o.DataUltimoAlertaVencimentoCertificado == null);

            return consultaEmpresa.ToList();
        }

        public Dominio.Entidades.Empresa BuscarEmpresaMatriz(Dominio.Entidades.Empresa empresaFilial)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            IQueryable<Dominio.Entidades.Empresa> result = from obj in query where obj.Filiais.Contains(empresaFilial) select obj;
            return result.FirstOrDefault();
        }

        public List<int> BuscarCodigosEmpresasEmissoras()
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();

            return query.Where(o => o.TipoSistema == Dominio.Enumeradores.TipoSistema.MultiCTe && o.Status == "A").Select(o => o.Codigo).ToList();
        }

        public List<Dominio.Entidades.Empresa> BuscarTodas(string status)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(o => o.Status.Equals(status));

            return query.ToList();
        }

        public List<Dominio.Entidades.Empresa> BuscarTodasEmpresaPai(string status, int codigoEmpresaPai)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(o => o.Status.Equals(status));

            if (codigoEmpresaPai > 0)
                query = query.Where(o => o.EmpresaPai.Codigo == codigoEmpresaPai);

            return query.ToList();
        }

        public List<Dominio.Entidades.Empresa> Consultar(bool somenteEmpresaFilho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();

            IQueryable<Dominio.Entidades.Empresa> result = from obj in query select obj;

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Status == "A");

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Status == "I");

            if (somenteEmpresaFilho)
                result = result.Where(obj => obj.EmpresaPai != null);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

        public int ContarConsulta(bool somenteEmpresaFilho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();

            IQueryable<Dominio.Entidades.Empresa> result = from obj in query select obj;

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Status == "A");

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Status == "I");

            if (somenteEmpresaFilho)
                result = result.Where(obj => obj.EmpresaPai != null);

            return result.Count();
        }

        public List<Dominio.Entidades.Empresa> Consultar(string descricao, bool somenteEmpresaFilho, int codigoEmpresa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, bool listarSomenteEmpresasFiliais, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            IQueryable<Dominio.Entidades.Empresa> result = Consultar(descricao, somenteEmpresaFilho, codigoEmpresa, ativo, listarSomenteEmpresasFiliais);

            if (!string.IsNullOrWhiteSpace(propOrdenacao))
                result = result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));

            if (maximoRegistros > 0)
                result = result.Skip(inicioRegistros).Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(string descricao, bool somenteEmpresaFilho, int codigoEmpresa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, bool listarSomenteEmpresasFiliais)
        {
            IQueryable<Dominio.Entidades.Empresa> result = Consultar(descricao, somenteEmpresaFilho, codigoEmpresa, ativo, listarSomenteEmpresasFiliais);

            return result.Count();
        }

        public List<Dominio.ObjetosDeValor.Relatorios.RelatorioCobrancas> RelatorioCobrancasEmpresaPai()
        {
            // Apenas para vincular o dataset no relatorio
            return new List<Dominio.ObjetosDeValor.Relatorios.RelatorioCobrancas>();
        }

        public List<Dominio.Entidades.Empresa> BuscarPorPerfilEmbarcador(int perfil)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            IQueryable<Dominio.Entidades.Empresa> result = from obj in query where obj.PerfilAcessoTransportador.Codigo == perfil select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Empresa> ConsultarEmpresaFaturamento(int codigoGrupoFaturamento, int codigoEmpresaPai, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();

            IQueryable<Dominio.Entidades.Empresa> result = from obj in query select obj;

            result = result.Where(obj => obj.Status == "A");

            if (codigoEmpresaPai > 0)
                result = result.Where(obj => obj.EmpresaPai.Codigo == codigoEmpresaPai);

            if (codigoGrupoFaturamento < 0)
                result = result.Where(obj => obj.Codigo == codigoGrupoFaturamento);

            if (maximoRegistros > 0)
                return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
            else
                return result.ToList();
        }

        public int ContarConsultaEmpresaFaturamento(int codigoGrupoFaturamento, int codigoEmpresaPai)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();

            IQueryable<Dominio.Entidades.Empresa> result = from obj in query select obj;

            result = result.Where(obj => obj.Status == "A");

            if (codigoEmpresaPai > 0)
                result = result.Where(obj => obj.EmpresaPai.Codigo == codigoEmpresaPai);

            if (codigoGrupoFaturamento < 0)
                result = result.Where(obj => obj.Codigo == codigoGrupoFaturamento);

            return result.Count();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.FaturamentoMensal.EmpresasFaturamento> ListaEmpresaFaturamentoMensal(int codigoEmpresa, int codigoGrupoFaturamento, string propOrdenar, string dirOrdena, int inicio, int limite)
        {
            string query = @"SELECT E.EMP_CODIGO CodigoEmpresa,
                CAST(E.EMP_CNPJ AS FLOAT) CNPJCliente,
                ISNULL(F.FMC_CODIGO, 0) CodigoFaturamentoMensalCliente,
                EMP_RAZAO Empresa,
                CAST(G.FMG_DIA_FATURA AS VARCHAR(20)) DiaFaturamento,
                '' ProximoVencimento,
                '' UltimoVencimento,
                '' ValorFaturamento,
                CASE 
	                WHEN F.FMC_CODIGO IS NULL THEN 'NÃO' ELSE 'OK'
                END CadastroFaturamento,
                '' PlanoMensal,
                '' QtdDocumento,
                '' QtdNFe,
                '' QtdNFSe,
                '' QtdBoleto,
                '' QtdTitulo
                 FROM T_EMPRESA E
                 LEFT OUTER JOIN T_CLIENTE C ON C.CLI_CGCCPF = CAST(E.EMP_CNPJ AS FLOAT)
                 LEFT OUTER JOIN T_FATURAMENTO_MENSAL_CLIENTE F ON F.CLI_CGCCPF = C.CLI_CGCCPF AND F.FMC_ATIVO = 1 AND F.FMG_CODIGO = " + codigoGrupoFaturamento.ToString() + @",
                 T_FATURAMENTO_MENSAL_GRUPO G
                 WHERE E.EMP_EMPRESA = " + codigoEmpresa.ToString() + @" AND E.EMP_STATUS = 'A'     
                 AND G.FMG_CODIGO = " + codigoGrupoFaturamento.ToString();

            if (limite > 0)
                query += " order by E.EMP_RAZAO " + dirOrdena + " OFFSET " + inicio.ToString() + " ROWS FETCH FIRST " + limite.ToString() + " ROWS ONLY; ";

            NHibernate.ISQLQuery nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.FaturamentoMensal.EmpresasFaturamento)));

            return nhQuery.SetTimeout(60000).List<Dominio.Relatorios.Embarcador.DataSource.FaturamentoMensal.EmpresasFaturamento>();
        }

        public int ContarListaEmpresaFaturamentoMensal(int codigoEmpresa, int codigoGrupoFaturamento)
        {
            string query = @"SELECT COUNT(0) as CONTADOR 
                 FROM T_EMPRESA E
                 LEFT OUTER JOIN T_CLIENTE C ON C.CLI_CGCCPF = CAST(E.EMP_CNPJ AS FLOAT)
                 LEFT OUTER JOIN T_FATURAMENTO_MENSAL_CLIENTE F ON F.CLI_CGCCPF = C.CLI_CGCCPF AND F.FMG_CODIGO = " + codigoGrupoFaturamento.ToString() + @",
                 T_FATURAMENTO_MENSAL_GRUPO G
                 WHERE E.EMP_EMPRESA = " + codigoEmpresa.ToString() + @" AND E.EMP_STATUS = 'A'                 
                 AND G.FMG_CODIGO = " + codigoGrupoFaturamento.ToString();

            NHibernate.ISQLQuery nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Transportadores.VencimentoCertificado> RelatorioVencimentoCertificado(int codigoEmpresaLogada, int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, string propGrupo, string dirOrdenacaoGrupo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, bool paginar = true, bool todosCNPJdaRaizEmbarcador = false)
        {
            string query = @"   SELECT EMP_CNPJ CNPJEmpresa,
                                EMP_RAZAO Empresa,
                                EMP_FONE Telefone,
                                EMP_EMAIL Email,
                                EMP_EMAILCONTADOR EmailContador,
                                EMP_EMAILADM EmailAdministrativo,
                                EMP_DATA_CERTIFICADOFINAL DataVencimento
                                FROM T_EMPRESA
                                WHERE EMP_STATUS = 'A' AND EMP_DATA_CERTIFICADOFINAL IS NOT NULL ";

            if (codigoEmpresaLogada > 0 && codigoEmpresaLogada != 137)
                query += " AND EMP_EMPRESA = " + codigoEmpresaLogada.ToString();

            if (codigoEmpresa > 0)
                query += " AND EMP_CODIGO = " + codigoEmpresa.ToString();

            if (dataInicial != DateTime.MinValue)
                query += " AND EMP_DATA_CERTIFICADOFINAL >= '" + dataInicial.ToString("MM/dd/yyyy") + "'";

            if (dataFinal != DateTime.MinValue)
                query += " AND EMP_DATA_CERTIFICADOFINAL <= '" + dataFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";

            bool agrup = false;
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

            if (paginar)
                query += " OFFSET " + inicioRegistros + " ROWS FETCH FIRST " + maximoRegistros + " ROWS ONLY";

            NHibernate.ISQLQuery nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Transportadores.VencimentoCertificado)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Transportadores.VencimentoCertificado>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Transportadores.VencimentoCertificado>> RelatorioVencimentoCertificadoAsync(int codigoEmpresaLogada, int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, string propGrupo, string dirOrdenacaoGrupo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, bool paginar = true, bool todosCNPJdaRaizEmbarcador = false)
        {
            string query = @"   SELECT EMP_CNPJ CNPJEmpresa,
                                EMP_RAZAO Empresa,
                                EMP_FONE Telefone,
                                EMP_EMAIL Email,
                                EMP_EMAILCONTADOR EmailContador,
                                EMP_EMAILADM EmailAdministrativo,
                                EMP_DATA_CERTIFICADOFINAL DataVencimento
                                FROM T_EMPRESA
                                WHERE EMP_STATUS = 'A' AND EMP_DATA_CERTIFICADOFINAL IS NOT NULL ";

            if (codigoEmpresaLogada > 0 && codigoEmpresaLogada != 137)
                query += " AND EMP_EMPRESA = " + codigoEmpresaLogada.ToString();

            if (codigoEmpresa > 0)
                query += " AND EMP_CODIGO = " + codigoEmpresa.ToString();

            if (dataInicial != DateTime.MinValue)
                query += " AND EMP_DATA_CERTIFICADOFINAL >= '" + dataInicial.ToString("MM/dd/yyyy") + "'";

            if (dataFinal != DateTime.MinValue)
                query += " AND EMP_DATA_CERTIFICADOFINAL <= '" + dataFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";

            bool agrup = false;
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

            if (paginar)
                query += " OFFSET " + inicioRegistros + " ROWS FETCH FIRST " + maximoRegistros + " ROWS ONLY";

            NHibernate.ISQLQuery nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Transportadores.VencimentoCertificado)));

            return await nhQuery.ListAsync<Dominio.Relatorios.Embarcador.DataSource.Transportadores.VencimentoCertificado>();
        }

        public int ContarRelatorioVencimentoCertificado(int codigoEmpresaLogada, int codigoEmpresa, DateTime dataInicial, DateTime dataFinal)
        {
            string query = @"   SELECT COUNT(0) as CONTADOR
	
                                FROM T_EMPRESA
                                WHERE EMP_STATUS = 'A' AND EMP_DATA_CERTIFICADOFINAL IS NOT NULL ";

            if (codigoEmpresaLogada > 0 && codigoEmpresaLogada != 137)
                query += " AND EMP_EMPRESA = " + codigoEmpresaLogada.ToString();

            if (codigoEmpresa > 0)
                query += " AND EMP_CODIGO = " + codigoEmpresa.ToString();

            if (dataInicial != DateTime.MinValue)
                query += " AND EMP_DATA_CERTIFICADOFINAL >= '" + dataInicial.ToString("MM/dd/yyyy") + "'";

            if (dataFinal != DateTime.MinValue)
                query += " AND EMP_DATA_CERTIFICADOFINAL <= '" + dataFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";

            NHibernate.ISQLQuery nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.UniqueResult<int>();
        }

        public IList<Dominio.ObjetosDeValor.Relatorios.RelatorioContadores> RelatorioContadores(int codigoEmpresa)
        {
            string sql = @"SELECT DISTINCT
	                            empresa.EMP_NOMECONTADOR Nome,
	                            empresa.EMP_FONECONTADOR Telefone,
	                            empresa.EMP_EMAILCONTADOR Email,
	                            localidade.LOC_DESCRICAO Cidade,
	                            localidade.UF_SIGLA UF
                            FROM 
	                            T_EMPRESA empresa
                            JOIN T_LOCALIDADES localidade
	                            ON empresa.LOC_CODIGO = localidade.LOC_CODIGO ";

            List<string> wheres = new List<string>() {
                "AND empresa.EMP_NOMECONTADOR IS NOT NULL",
                "AND empresa.EMP_NOMECONTADOR <> ''",
                //"AND empresa.EMP_STATUS = 'A'"
            };

            if (codigoEmpresa > 0)
                wheres.Add("AND empresa.EMP_CODIGO = " + codigoEmpresa.ToString());

            sql += "WHERE 1 = 1 " + string.Join(" ", wheres);

            sql += @" ORDER BY 
	                    localidade.UF_SIGLA, localidade.LOC_DESCRICAO, empresa.EMP_NOMECONTADOR ASC";


            NHibernate.ISQLQuery nhQuery = this.SessionNHiBernate.CreateSQLQuery(sql);
            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Relatorios.RelatorioContadores)));

            return nhQuery.SetTimeout(120).List<Dominio.ObjetosDeValor.Relatorios.RelatorioContadores>();
        }

        public List<Dominio.Entidades.Empresa> BuscarPorCodigoConfiguracaoCIOT(int configuracaoCIOT)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();

            IQueryable<Dominio.Entidades.Empresa> result = from o in query where o.ConfiguracaoCIOT.Codigo == configuracaoCIOT select o;

            return result.ToList();
        }

        public List<Dominio.Entidades.Empresa> BuscarPorRaizCNPJ(string raizCNPJ)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();

            query = query.Where(o => o.CNPJ.StartsWith(raizCNPJ) && o.Status == "A");

            return query.ToList();
        }

        public IList<int> BuscarCodigosFiliaisVinculadas(IList<int> codigosTransportadores)
        {
            string sqlQuery = @"select filiais.FIL_CODIGO 
                                  from T_EMPRESA_FILIAL filiais 
                                 where filiais.EMP_CODIGO in (:codigosTransportadores)";

            NHibernate.ISQLQuery queryCodCarga = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

            queryCodCarga.SetParameterList("codigosTransportadores", codigosTransportadores);

            return queryCodCarga.List<int>();
        }

        public bool TransportadorFilial(int CodEmpresa)
        {
            string sqlQuery = $"select top 1 filiais.FIL_CODIGO from T_EMPRESA_FILIAL filiais where filiais.FIL_CODIGO = {CodEmpresa}"; // SQL-INJECTION-SAFE

            NHibernate.ISQLQuery queryCodCarga = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

            return queryCodCarga.UniqueResult<int>() > 0;
        }

        public Dominio.Entidades.Estado BuscarPrimeiroEstado()
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();

            query = query.Where(o => o.EmpresaPai != null && o.Status == "A");

            return query.Select(o => o.Localidade.Estado).FirstOrDefault();
        }

        public List<int> BuscarCodigosAptosParaEmissaoPorEstado(string siglaUF)
        {
            Dominio.Enumeradores.TipoSerie[] tiposSeriesValidas = new Dominio.Enumeradores.TipoSerie[] { Dominio.Enumeradores.TipoSerie.CTe, Dominio.Enumeradores.TipoSerie.NFSe };

            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            IQueryable<Dominio.Entidades.EmpresaSerie> querySerie = this.SessionNHiBernate.Query<Dominio.Entidades.EmpresaSerie>();

            query = query.Where(empresa => empresa.EmpresaPai != null &&
                                           empresa.Status == "A" &&
                                           (querySerie.Where(serie => serie.Empresa == empresa && tiposSeriesValidas.Contains(serie.Tipo)).Any() || empresa.EmissaoDocumentosForaDoSistema));

            if (!string.IsNullOrWhiteSpace(siglaUF))
                query = query.Where(o => o.Localidade.Estado.Sigla == siglaUF);

            return query.Select(o => o.Codigo).ToList();
        }

        public List<(int, bool)> BuscarTransportadores(List<int> codigoEmpresa)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>().Where(x => codigoEmpresa.Contains(x.Codigo));

            return query.Select(o => ValueTuple.Create(o.Codigo, o.EmissaoDocumentosForaDoSistema)).ToList();
        }

        public bool ExisteTransportadorFerroviario()
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            query = from obj in query where obj.TransportadorFerroviario select obj;
            return query.Any();
        }

        public List<Dominio.Entidades.Empresa> BuscarTransportadoresParaGerarTermo()
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            IQueryable<Dominio.Entidades.Empresa> sub = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();

            query = query.Where(o => o.Status == "A" && o.TipoGeracaoTermo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGeracaoTermo.Automatico && o.GerarAvisoPeriodico &&
            (o.Filiais.Count > 0 || (o.Filiais.Count == 0 && !sub.Any(x => x.Codigo != o.Codigo && x.Filiais.Contains(o)))));

            return query.ToList();
        }


        public List<int> BuscarCodigosTransportadoresParaAvisoPeriodico()
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            query = query.Where(o => o.Status == "A" && o.GerarAvisoPeriodico && o.TipoGeracaoTermo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGeracaoTermo.Automatico);

            return query.Select(obj => obj.Codigo).ToList();
        }


        public List<int> BuscarCodigosFiliaisTransportador(int CodigoTransportador)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            query = query.Where(o => o.Codigo == CodigoTransportador);
            Dominio.Entidades.Empresa retorno = query.FirstOrDefault();
            return retorno != null ? retorno.Filiais.Select(x => x.Codigo).ToList() : new List<int>();
        }

        public Dominio.Entidades.Empresa BuscarPorUsuario(string cpf)
        {
            Dominio.Entidades.Usuario query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>().Where(obj => obj.CPF == cpf).FirstOrDefault();

            if (query == null)
                return null;
            return BuscarPorCodigo(query.Empresa.Codigo);
        }

        public string BuscarEmailEnvioCTeRejeitado(int codigo)
        {
            Dominio.Entidades.Empresa query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>().FirstOrDefault(obj => obj.Codigo == codigo);
            return query != null ? query.EmailEnvioCTeRejeitado : null;
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Empresa> _Consultar(Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaTransportador filtro)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();

            IQueryable<Dominio.Entidades.Empresa> result = from obj in query select obj;

            if (filtro.Codigos != null && filtro.Codigos.Count > 0)
                result = result.Where(o => filtro.Codigos.Contains(o.Codigo));

            if (filtro.CodigoEmpresa != 137)
            {
                if (filtro.CodigoEmpresa < 0)
                    result = result.Where(o => o.Codigo == (filtro.CodigoEmpresa * -1));
                else
                    result = result.Where(o => o.Codigo == filtro.CodigoEmpresa || o.EmpresaPai.Codigo == filtro.CodigoEmpresa);
            }

            if (filtro.SituacaoPesquisa != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                result = result.Where(o => o.Status == (filtro.SituacaoPesquisa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo ? "A" : "I"));

            if (!string.IsNullOrWhiteSpace(filtro.RazaoSocial))
                result = result.Where(o => o.RazaoSocial.Contains(filtro.RazaoSocial));

            if (!string.IsNullOrWhiteSpace(filtro.NomeFantasia))
                result = result.Where(o => o.NomeFantasia.Contains(filtro.NomeFantasia));

            if (!string.IsNullOrWhiteSpace(filtro.CNPJ))
            {
                if (!filtro.BuscarFiliais)
                    result = result.Where(o => o.CNPJ == filtro.CNPJ);
                else
                    result = result.Where(o => (o.CNPJ == filtro.CNPJ || o.Matriz.Any(m => m.CNPJ == filtro.CNPJ)));
            }

            if (!string.IsNullOrWhiteSpace(filtro.RaizCnpj))
                result = result.Where(o => o.CNPJ.Substring(0, 8) == filtro.RaizCnpj);

            if (filtro.SomenteProducao)
                result = result.Where(o => o.TipoAmbiente == filtro.TipoAmbiente);

            if (filtro.SemEmpresaPai)
                result = result.Where(o => o.EmpresaPai != null);

            if (filtro.SistemaEmissor != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.Todos)
            {
                if (filtro.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.MultiCTe)
                    result = result.Where(o => o.EmissaoDocumentosForaDoSistema == false);
                else
                    result = result.Where(o => o.EmissaoDocumentosForaDoSistema == true);
            }

            if (!string.IsNullOrWhiteSpace(filtro.PlacaVeiculo))
            {
                IQueryable<Dominio.Entidades.Empresa> queryVeiculo = from o in SessionNHiBernate.Query<Dominio.Entidades.Veiculo>() where o.Placa == filtro.PlacaVeiculo && o.Ativo select o.Empresa;
                result = result.Where(o => queryVeiculo.Contains(o));
            }

            if (filtro.CodigoFilial > 0 && !filtro.SomenteTransportadorHabilitadoTransportarParaFilial)
                result = result.Where(o => o.FiliaisEmbarcadorHabilitado.Any(f => f.Codigo == filtro.CodigoFilial) || o.FiliaisEmbarcadorHabilitado.Count() == 0);

            if (filtro.CodigoFilial > 0 && filtro.SomenteTransportadorHabilitadoTransportarParaFilial)
                result = result.Where(o => o.FiliaisEmbarcadorHabilitado.Any(f => f.Codigo == filtro.CodigoFilial));

            if (!string.IsNullOrWhiteSpace(filtro.UFFilialTransportador))
            {
                result = result.Where(o => (!o.Matriz.Any() && !o.Filiais.Any())
                                           || (!o.Matriz.Any() && !o.Filiais.Any(f => f.Localidade.Estado.Sigla == filtro.UFFilialTransportador))
                                           || o.Localidade.Estado.Sigla == filtro.UFFilialTransportador
                                    );
            }

            if (filtro.ListaCodigoTransportadorPermitidos?.Count > 0)
                result = result.Where(o => filtro.ListaCodigoTransportadorPermitidos.Contains(o.Codigo));

            if (filtro.CodigosEmpresa != null && filtro.CodigosEmpresa.Count > 0)
                result = result.Where(o => filtro.CodigosEmpresa.Contains(o.Codigo));

            // Filtrando todas as empresas.. diferente da empresa padrão de contratação;
            if (filtro.SomenteEmpresaNaoTransportadoraPadraoContratacao)
                result = result.Where(x => x.Codigo != filtro.CodigoEmpresaTransportadoraPadraoContratacao);

            // Agora vamos retornar apenas as empresas que são sub=contratadas pela transportadora padrão
            if (filtro.SomenteSubEmpresasTransportadoraPadraoContratacao)
                result = result.Where(x => x.UtilizaTransportadoraPadraoContratacao);

            if (filtro.SomenteTransportadoresPermitidosCadastroAgendamentoColeta)
                result = result.Where(x => x.PermitirUtilizarCadastroAgendamentoColeta);

            if (filtro.SomenteTransportadoresManuais)
            {
                IQueryable<Dominio.Entidades.Empresa> subQuery = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
                result = result.Where(x => x.TipoGeracaoTermo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGeracaoTermo.Manual && x.GerarAvisoPeriodico && (x.Filiais.Count > 0 && !subQuery.Any(a => a.Filiais.Any(q => q.Codigo == x.Codigo)) || (x.Filiais.Count == 0 && !subQuery.Any(a => a.Filiais.Any(q => q.Codigo == x.Codigo)))));
            }

            if (filtro.CodigoEmpresaMatriz > 0)
                result = result.Where(x => x.Filiais.Any(o => o.Codigo == filtro.CodigoEmpresaMatriz));

            if (!string.IsNullOrEmpty(filtro.CodigoIntegracao))
                result = result.Where(x => x.CodigoIntegracao.Contains(filtro.CodigoIntegracao));

            if (!string.IsNullOrEmpty(filtro.Estado))
                result = result.Where(x => x.Localidade.Estado.Sigla == filtro.Estado);

            if (!string.IsNullOrEmpty(filtro.Localidade))
                result = result.Where(x => x.Localidade.Descricao == filtro.Localidade);

            if (filtro.CodigosTransportadores != null && filtro.CodigosTransportadores.Count > 0)
                result = result.Where(x => filtro.CodigosTransportadores.Contains(x.Codigo));

            if (filtro.CodigoTipoOperacao > 0)
            {
                List<int> subQuery = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.ConfiguracaoTipoOperacao>()
                    .Where(sub => sub.TipoOperacao.Codigo == filtro.CodigoTipoOperacao)
                    .Select(sub => sub.Empresa.Codigo)
                    .ToList();

                result = result.Where(x => subQuery.Contains(x.Codigo));
            }

            if (filtro.CodigoGrupoTransportador > 0)
            {
                List<int> subQuery = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.GrupoTransportadorEmpresa>()
                    .Where(obj => obj.GrupoTransportador.Codigo == filtro.CodigoGrupoTransportador)
                    .Select(obj => obj.Empresa.Codigo)
                    .ToList();

                result = result.Where(x => subQuery.Contains(x.Codigo));
            }

            return result;
        }

        private IQueryable<Dominio.Entidades.Empresa> _ConsultarAdmin(Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaTransportador filtro)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();

            IQueryable<Dominio.Entidades.Empresa> result = from obj in query select obj;

            if (filtro.Codigos != null && filtro.Codigos.Count > 0)
                result = result.Where(o => filtro.Codigos.Contains(o.Codigo));

            if (filtro.CodigoEmpresa > 0)
                result = result.Where(o => o.EmpresaPai.Codigo == filtro.CodigoEmpresa || o.Codigo == filtro.CodigoEmpresa);

            if (filtro.SituacaoPesquisa != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                result = result.Where(o => o.Status == (filtro.SituacaoPesquisa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo ? "A" : "I"));

            if (!string.IsNullOrWhiteSpace(filtro.RazaoSocial))
                result = result.Where(o => o.RazaoSocial.Contains(filtro.RazaoSocial) || o.NomeFantasia.Contains(filtro.RazaoSocial));

            if (!string.IsNullOrWhiteSpace(filtro.CNPJ))
            {
                if (!filtro.BuscarFiliais)
                    result = result.Where(o => o.CNPJ.Contains(filtro.CNPJ));
                else
                    result = result.Where(o => (o.CNPJ == filtro.CNPJ || o.Matriz.Any(m => m.CNPJ == filtro.CNPJ)));
            }

            if (!string.IsNullOrWhiteSpace(filtro.RaizCnpj))
                result = result.Where(o => o.CNPJ.Substring(0, 8) == filtro.RaizCnpj);

            if (filtro.SomenteProducao)
                result = result.Where(o => o.TipoAmbiente == filtro.TipoAmbiente);

            if (filtro.SemEmpresaPai)
                result = result.Where(o => o.EmpresaPai != null);

            if (filtro.SistemaEmissor != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.Todos)
            {
                if (filtro.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.MultiCTe)
                    result = result.Where(o => o.EmissaoDocumentosForaDoSistema == false);
                else
                    result = result.Where(o => o.EmissaoDocumentosForaDoSistema == true);
            }

            if (!string.IsNullOrWhiteSpace(filtro.PlacaVeiculo))
            {
                IQueryable<Dominio.Entidades.Empresa> queryVeiculo = from o in SessionNHiBernate.Query<Dominio.Entidades.Veiculo>() where o.Placa == filtro.PlacaVeiculo && o.Ativo select o.Empresa;
                result = result.Where(o => queryVeiculo.Contains(o));
            }

            if (filtro.CodigoFilial > 0)
                result = result.Where(o => o.FiliaisEmbarcadorHabilitado.Any(f => f.Codigo == filtro.CodigoFilial) || o.FiliaisEmbarcadorHabilitado.Count() == 0);

            if (!string.IsNullOrWhiteSpace(filtro.UFFilialTransportador))
            {
                result = result.Where(o => (!o.Matriz.Any() && !o.Filiais.Any())
                                           || (!o.Matriz.Any() && !o.Filiais.Any(f => f.Localidade.Estado.Sigla == filtro.UFFilialTransportador))
                                           || o.Localidade.Estado.Sigla == filtro.UFFilialTransportador
                                    );
            }

            return result;
        }

        private IQueryable<Dominio.Entidades.Empresa> Consultar(string descricao, bool somenteEmpresaFilho, int codigoEmpresa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, bool listarSomenteEmpresasFiliais)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();

            IQueryable<Dominio.Entidades.Empresa> result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.RazaoSocial.Contains(descricao) || obj.CodigoIntegracao == descricao);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Status == "A");
            else if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Status == "I");

            if (codigoEmpresa > 0 && somenteEmpresaFilho && codigoEmpresa != 137)
                result = result.Where(obj => obj.EmpresaPai.Codigo == codigoEmpresa);

            if (listarSomenteEmpresasFiliais)
            {
                IQueryable<Dominio.Entidades.Embarcador.Transportadores.TransportadorFilial> queryTransportadorFilial = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.TransportadorFilial>();
                IQueryable<Dominio.Entidades.Embarcador.Transportadores.TransportadorFilial> resultQueryTransportadorFilial = from obj in queryTransportadorFilial where obj.Empresa.Codigo == codigoEmpresa select obj;

                result = result.Where(o => o.Codigo == codigoEmpresa || resultQueryTransportadorFilial.Where(a => a.CNPJ == o.CNPJ).Any());
            }

            return result;
        }

        #endregion

        #region Relatório de Transportadores

        private IQueryable<Dominio.Entidades.Empresa> _ConsultarRelatorio(Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaRelatorioTransportador filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Empresa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();

            IQueryable<Dominio.Entidades.Empresa> result = from obj in query select obj;

            if (filtrosPesquisa.Localidade > 0)
                result = result.Where(o => o.Localidade.Codigo == filtrosPesquisa.Localidade);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Estado))
                result = result.Where(obj => obj.Localidade.Estado.Sigla.Equals(filtrosPesquisa.Estado));

            if (filtrosPesquisa.CertificadosVencidos)
                result = result.Where(o => o.DataFinalCertificado.Value.Date < DateTime.Now.Date);

            if (filtrosPesquisa.EmiteEmbarcador == Dominio.Enumeradores.OpcaoSimNaoPesquisa.Sim)
                result = result.Where(o => !o.EmissaoDocumentosForaDoSistema);
            else if (filtrosPesquisa.EmiteEmbarcador == Dominio.Enumeradores.OpcaoSimNaoPesquisa.Nao)
                result = result.Where(o => o.EmissaoDocumentosForaDoSistema);

            if (filtrosPesquisa.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(o => o.Status.Equals("A"));
            else if (filtrosPesquisa.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(o => o.Status.Equals("I"));

            if (filtrosPesquisa.PrazoValidade != DateTime.MinValue)
                result = result.Where(o => o.DataFinalCertificado >= filtrosPesquisa.PrazoValidade);

            if (filtrosPesquisa.LiberacaoParaPagamentoAutomatico.HasValue)
                result = result.Where(o => o.LiberacaoParaPagamentoAutomatico == filtrosPesquisa.LiberacaoParaPagamentoAutomatico.Value);

            if (filtrosPesquisa.OptanteSimplesNacional == Dominio.Enumeradores.OpcaoSimNaoPesquisa.Sim)
                result = result.Where(o => o.OptanteSimplesNacional);
            else if (filtrosPesquisa.OptanteSimplesNacional == Dominio.Enumeradores.OpcaoSimNaoPesquisa.Nao)
                result = result.Where(o => !o.OptanteSimplesNacional);

            if (filtrosPesquisa.ConfiguracaoNFSe == Dominio.Enumeradores.OpcaoSimNaoPesquisa.Sim)
                result = result.Where(o => o.TransportadorConfiguracoesNFSe.Count > 0);
            else if (filtrosPesquisa.ConfiguracaoNFSe == Dominio.Enumeradores.OpcaoSimNaoPesquisa.Nao)
                result = result.Where(o => o.TransportadorConfiguracoesNFSe.Count == 0);

            if (filtrosPesquisa.Bloqueado.HasValue)
                result = result.Where(o => o.BloquearTransportador == filtrosPesquisa.Bloqueado.Value);

            if (filtrosPesquisa.DataInicioVencimentoCertificado != DateTime.MinValue)
                result = result.Where(o => o.DataFinalCertificado.Value.Date >= filtrosPesquisa.DataInicioVencimentoCertificado.Date);

            if (filtrosPesquisa.DataFinalVencimentoCertificado != DateTime.MinValue)
                result = result.Where(o => o.DataFinalCertificado.Value.Date <= filtrosPesquisa.DataFinalVencimentoCertificado.Date);

            bool existeDataCriacao = filtrosPesquisa.DataCriacaoInicial.HasValue || filtrosPesquisa.DataCriacaoFinal.HasValue;
            bool existeDataAtualizacao = filtrosPesquisa.DataAlteracaoInicial.HasValue || filtrosPesquisa.DataAlteracaoFinal.HasValue;

            if (filtrosPesquisa.DataCriacaoInicial.HasValue && filtrosPesquisa.DataCriacaoInicial.Value != DateTime.MinValue)
                result = result.Where(o => o.DataCadastro.Value.Date >= filtrosPesquisa.DataCriacaoInicial.Value.Date || (existeDataAtualizacao && !o.DataCadastro.HasValue));

            if (filtrosPesquisa.DataCriacaoFinal.HasValue && filtrosPesquisa.DataCriacaoFinal.Value != DateTime.MinValue)
                result = result.Where(o => o.DataCadastro.Value.Date <= filtrosPesquisa.DataCriacaoFinal.Value.Date || (existeDataAtualizacao && !o.DataCadastro.HasValue));


            if (filtrosPesquisa.DataAlteracaoInicial.HasValue && filtrosPesquisa.DataAlteracaoInicial.Value != DateTime.MinValue)
                result = result.Where(o => o.DataAtualizacao.Value.Date >= filtrosPesquisa.DataAlteracaoInicial.Value.Date || (existeDataCriacao && !o.DataAtualizacao.HasValue));


            if (filtrosPesquisa.DataAlteracaoFinal.HasValue && filtrosPesquisa.DataAlteracaoFinal.Value != DateTime.MinValue)
                result = result.Where(o => o.DataAtualizacao.Value.Date <= filtrosPesquisa.DataAlteracaoFinal.Value.Date || (existeDataCriacao && !o.DataAtualizacao.HasValue));

            return result;
        }

        public List<Dominio.Relatorios.Embarcador.DataSource.Transportadores.ReportTrnasportadores> ConsultarRelatorio(Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaRelatorioTransportador filtrosPesquisa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, Repositorio.UnitOfWork unitOfWork)
        {
            IQueryable<Dominio.Entidades.Empresa> consultaEmpresa = _ConsultarRelatorio(filtrosPesquisa);

            List<Dominio.Entidades.Empresa> empresas = ObterLista(consultaEmpresa, propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros);

            return (
                from o in empresas
                select new Dominio.Relatorios.Embarcador.DataSource.Transportadores.ReportTrnasportadores()
                {
                    CodigoIntegracao = o.CodigoIntegracao,
                    RazaoSocial = o.RazaoSocial,
                    NomeFantasia = o.NomeFantasia,
                    CNPJTransportadorSemFormato = o.CNPJ,
                    InscricaoEstadual = o.InscricaoEstadual,
                    RNTRC = o.RegistroANTT,
                    Cidade = o.Localidade.Descricao ?? string.Empty,
                    Estado = o.Localidade.Estado.Sigla ?? string.Empty,
                    Endereco = o.Endereco,
                    Numero = o.Numero,
                    Email = o.Email,
                    Bairro = o.Bairro,
                    Telefone = o.Telefone,
                    CEP = o.CEP,
                    Status = o.Status,
                    EmiteEmbarcador = o.EmissaoDocumentosForaDoSistema,
                    CertificadoVencido = o.DataFinalCertificado.HasValue ? o.DataFinalCertificado.Value > DateTime.Now : false,
                    DataVencimentoCertificado = o.DataFinalCertificado.HasValue ? o.DataFinalCertificado.Value.ToString("dd/MM/yyyy") : string.Empty,
                    LiberacaoParaPagamentoAutomatico = o.LiberacaoParaPagamentoAutomatico,
                    OptanteSimplesNacional = o.DescricaoOptanteSimplesNacional,
                    ConfiguracaoNFSe = o.TransportadorConfiguracoesNFSe?.Count > 0,
                    Bloqueado = o.BloquearTransportador ? "Sim" : "Não",
                    MotivoBloqueio = o.MotivoBloqueio,
                    SerieCTeFora = o?.Configuracao?.SerieInterestadual?.Numero ?? 0,
                    SerieCTeDentro = o?.Configuracao?.SerieIntraestadual?.Numero ?? 0,
                    SerieMDFe = o?.Configuracao?.SerieIntraestadual?.Numero ?? 0,
                    UsuarioCadastro = o.UsuarioCadastro?.Descricao ?? string.Empty,
                    UsuarioAtualizacao = o.UsuarioAtualizacao?.Descricao ?? string.Empty,
                    DataCadastro = o.DataCadastro?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                    DataAtualizacao = o.DataAtualizacao?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                    OperadoraValePedagio = string.Join(", ", o.TiposIntegracaoValePedagio?.Select(tipoIntegracaoValePedagio => tipoIntegracaoValePedagio.Descricao))
                }
            ).ToList();
        }


        public async Task<int> ContaRegistrosAsync(Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaRelatorioTransportador filtrosPesquisa, Repositorio.UnitOfWork unitOfWork)
        {
            IQueryable<Dominio.Entidades.Empresa> consultaEmpresa = _ConsultarRelatorio(filtrosPesquisa);

            List<Dominio.Entidades.Empresa> empresas = await ObterListaAsync(consultaEmpresa, "", "", 0, 0);

            return empresas?.Count() ?? 0;
        }



        public async Task<List<Dominio.Relatorios.Embarcador.DataSource.Transportadores.ReportTrnasportadores>> ConsultarRelatorioAsync(Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaRelatorioTransportador filtrosPesquisa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, Repositorio.UnitOfWork unitOfWork)
        {
            IQueryable<Dominio.Entidades.Empresa> consultaEmpresa = _ConsultarRelatorio(filtrosPesquisa);

            List<Dominio.Entidades.Empresa> empresas = await ObterListaAsync(consultaEmpresa, propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros);

            return (
                from o in empresas
                select new Dominio.Relatorios.Embarcador.DataSource.Transportadores.ReportTrnasportadores()
                {
                    CodigoIntegracao = o.CodigoIntegracao,
                    RazaoSocial = o.RazaoSocial,
                    NomeFantasia = o.NomeFantasia,
                    CNPJTransportadorSemFormato = o.CNPJ,
                    InscricaoEstadual = o.InscricaoEstadual,
                    RNTRC = o.RegistroANTT,
                    Cidade = o.Localidade.Descricao ?? string.Empty,
                    Estado = o.Localidade.Estado.Sigla ?? string.Empty,
                    Endereco = o.Endereco,
                    Numero = o.Numero,
                    Email = o.Email,
                    Bairro = o.Bairro,
                    Telefone = o.Telefone,
                    CEP = o.CEP,
                    Status = o.Status,
                    EmiteEmbarcador = o.EmissaoDocumentosForaDoSistema,
                    CertificadoVencido = o.DataFinalCertificado.HasValue ? o.DataFinalCertificado.Value > DateTime.Now : false,
                    DataVencimentoCertificado = o.DataFinalCertificado.HasValue ? o.DataFinalCertificado.Value.ToString("dd/MM/yyyy") : string.Empty,
                    LiberacaoParaPagamentoAutomatico = o.LiberacaoParaPagamentoAutomatico,
                    OptanteSimplesNacional = o.DescricaoOptanteSimplesNacional,
                    ConfiguracaoNFSe = o.TransportadorConfiguracoesNFSe?.Count > 0,
                    Bloqueado = o.BloquearTransportador ? "Sim" : "Não",
                    MotivoBloqueio = o.MotivoBloqueio,
                    SerieCTeFora = o?.Configuracao?.SerieInterestadual?.Numero ?? 0,
                    SerieCTeDentro = o?.Configuracao?.SerieIntraestadual?.Numero ?? 0,
                    SerieMDFe = o?.Configuracao?.SerieIntraestadual?.Numero ?? 0,
                    UsuarioCadastro = o.UsuarioCadastro?.Descricao ?? string.Empty,
                    UsuarioAtualizacao = o.UsuarioAtualizacao?.Descricao ?? string.Empty,
                    DataCadastro = o.DataCadastro?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                    DataAtualizacao = o.DataAtualizacao?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                    OperadoraValePedagio = string.Join(", ", o.TiposIntegracaoValePedagio?.Select(tipoIntegracaoValePedagio => tipoIntegracaoValePedagio.Descricao))
                }
            ).ToList();
        }

        public int ContarConsultaRelatorio(Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaRelatorioTransportador filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Empresa> query = _ConsultarRelatorio(filtrosPesquisa);

            return query.Count();
        }

        #endregion
    }
}