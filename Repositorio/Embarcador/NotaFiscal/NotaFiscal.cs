using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using AdminMultisoftware.Dominio.Enumeradores;
using System.Threading;
using Repositorio.Embarcador.Consulta;

namespace Repositorio.Embarcador.NotaFiscal
{
    public class NotaFiscal : RepositorioBase<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal>
    {
        public NotaFiscal(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public NotaFiscal(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal BuscarPorTitulo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault()?.NotaFiscal;
        }

        public List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal> BuscarPorCodigo(List<int> codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal>();
            var result = from obj in query where codigo.Contains(obj.Codigo) select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal> BuscarPorChave(List<string> chaves)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal>();
            var result = from obj in query where chaves.Contains(obj.Chave) select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal BuscarPorChave(string chave)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal>();
            var result = from obj in query where obj.Chave == chave select obj;
            result = result.Fetch(o => o.EmpresaSerie);
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal BuscarPorChave(string chave, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal>();
            var result = from obj in query where obj.Chave == chave && obj.Empresa.Codigo == codigoEmpresa select obj;
            result = result.Fetch(o => o.EmpresaSerie);
            return result.FirstOrDefault();
        }

        public bool ExisteFaixaNotaPorNumeroSerie(int empresa, int numeroInicial, int numeroFinal, int serie)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal>();
            var result = from obj in query where obj.Numero >= numeroInicial && obj.Numero >= numeroFinal && obj.EmpresaSerie.Numero == serie && obj.Empresa.Codigo == empresa select obj;
            return result.Count() > 0;
        }

        public bool NotaEmitida(int numero, int serie, int empresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal>();
            var result = from obj in query where obj.Numero == numero && obj.EmpresaSerie.Numero == serie && obj.Empresa.Codigo == empresa select obj;
            return result.Count() > 0;
        }

        public bool NumeracaoJaEmitida(int codigoEmpresa, int numero, int serie, Dominio.Enumeradores.TipoAmbiente tipoAmebiente, string modelo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Numero == numero && obj.EmpresaSerie.Numero == serie && obj.TipoAmbiente == tipoAmebiente && obj.ModeloNotaFiscal.Equals(modelo) select obj;
            return result.Count() > 0;
        }

        public int BuscarUltimoNumero(int codigoEmpresa, int serie, Dominio.Enumeradores.TipoAmbiente ambiente, string modelo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.EmpresaSerie.Numero == serie && obj.TipoAmbiente == ambiente && obj.ModeloNotaFiscal.Equals(modelo) select obj;
            if (result.Count() > 0)
                return result.Max(obj => obj.Numero);
            else
                return 0;
        }

        public List<int> BuscarListaCodigosNFes(Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNotasEmitidas filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal>();

            if (filtrosPesquisa.CodigoEmpresa > 0)
                query = query.Where(o => o.Empresa.Codigo == filtrosPesquisa.CodigoEmpresa && o.Empresa.TipoAmbiente == o.TipoAmbiente);

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                query = query.Where(o => o.DataEmissao.Value.Date >= filtrosPesquisa.DataInicial.Date);

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                query = query.Where(o => o.DataEmissao.Value.Date <= filtrosPesquisa.DataFinal.Date);

            if (filtrosPesquisa.NumeroInicial > 0)
                query = query.Where(o => o.Numero >= filtrosPesquisa.NumeroInicial);

            if (filtrosPesquisa.NumeroFinal > 0)
                query = query.Where(o => o.Numero <= filtrosPesquisa.NumeroFinal);

            if (filtrosPesquisa.Serie > 0)
                query = query.Where(o => o.EmpresaSerie.Numero == filtrosPesquisa.Serie);

            if (filtrosPesquisa.CodigoAtividade > 0)
                query = query.Where(o => o.Atividade.Codigo == filtrosPesquisa.CodigoAtividade);

            if (filtrosPesquisa.CodigoNaturezaOperacao > 0)
                query = query.Where(o => o.NaturezaDaOperacao.Codigo == filtrosPesquisa.CodigoNaturezaOperacao);

            if (filtrosPesquisa.CnpjPessoa > 0)
                query = query.Where(o => o.Cliente.CPF_CNPJ == filtrosPesquisa.CnpjPessoa);

            if (filtrosPesquisa.DataProcessamento != DateTime.MinValue)
                query = query.Where(o => o.DataProcessamento.Value.Date == filtrosPesquisa.DataProcessamento.Date);

            if (filtrosPesquisa.DataSaida != DateTime.MinValue)
                query = query.Where(o => o.DataSaida.Value.Date == filtrosPesquisa.DataSaida.Date);

            if (filtrosPesquisa.Status > 0)
                query = query.Where(o => o.Status == filtrosPesquisa.Status);

            if (filtrosPesquisa.TipoEmissao.HasValue)
                query = query.Where(o => o.TipoEmissao == filtrosPesquisa.TipoEmissao.Value);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Chave))
                query = query.Where(o => o.Chave.Equals(filtrosPesquisa.Chave));

            if (filtrosPesquisa.FormaEmissao == 1)
                query = query.Where(o => o.UltimoStatusSEFAZ != null);

            if (filtrosPesquisa.CodigosUsuario.Count > 0)
                query = query.Where(o => filtrosPesquisa.CodigosUsuario.Contains(o.Usuario.Codigo));

            if (filtrosPesquisa.CodigosCFOP.Count > 0)
                query = query.Where(o => o.ItensNFe.Any(i => filtrosPesquisa.CodigosCFOP.Contains(i.CFOP.Codigo)));

            return query.Select(o => o.Codigo).ToList();
        }

        public List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal> ConsultarNFC(int numeroInicial, int numeroFinal, int serie, DateTime dataInicial, DateTime dataFinal, int empresa, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, string chave, string cnpjCpfPessoa, string nomePessoa, Dominio.Enumeradores.StatusNFe status, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal>();

            var result = from obj in query select obj;

            if (numeroInicial > 0 && numeroFinal > 0)
                result = result.Where(obj => obj.Numero >= numeroInicial && obj.Numero <= numeroFinal);
            else if (numeroInicial > 0)
                result = result.Where(obj => obj.Numero == numeroInicial);
            else if (numeroFinal > 0)
                result = result.Where(obj => obj.Numero == numeroFinal);

            result = result.Where(obj => obj.TipoAmbiente == tipoAmbiente);
            result = result.Where(obj => obj.ModeloNotaFiscal.Equals("65"));

            if (dataInicial > DateTime.MinValue && dataFinal > DateTime.MinValue)
                result = result.Where(obj => obj.DataEmissao.Value.Date >= dataInicial && obj.DataEmissao.Value.Date <= dataFinal);
            else if (dataInicial > DateTime.MinValue)
                result = result.Where(obj => obj.DataEmissao.Value.Date >= dataInicial);
            else if (dataFinal > DateTime.MinValue)
                result = result.Where(obj => obj.DataEmissao.Value.Date <= dataFinal);

            if (serie > 0)
                result = result.Where(obj => obj.EmpresaSerie.Numero == serie);

            if (!string.IsNullOrWhiteSpace(chave))
                result = result.Where(obj => obj.Chave.Contains(chave));

            if (!string.IsNullOrWhiteSpace(cnpjCpfPessoa))
                result = result.Where(obj => obj.CPFCNPJConsumidorFinal.Contains(cnpjCpfPessoa));

            if (!string.IsNullOrWhiteSpace(nomePessoa))
                result = result.Where(obj => obj.NomeConsumidorFinal.Contains(nomePessoa));

            if (status > 0)
                result = result.Where(obj => obj.Status == status);

            if (empresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo.Equals(empresa));

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultaNFC(int numeroInicial, int numeroFinal, int serie, DateTime dataInicial, DateTime dataFinal, int empresa, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, string chave, string cnpjCpfPessoa, string nomePessoa, Dominio.Enumeradores.StatusNFe status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal>();

            var result = from obj in query select obj;

            if (numeroInicial > 0 && numeroFinal > 0)
                result = result.Where(obj => obj.Numero >= numeroInicial && obj.Numero <= numeroFinal);
            else if (numeroInicial > 0)
                result = result.Where(obj => obj.Numero == numeroInicial);
            else if (numeroFinal > 0)
                result = result.Where(obj => obj.Numero == numeroFinal);

            result = result.Where(obj => obj.TipoAmbiente == tipoAmbiente);
            result = result.Where(obj => obj.ModeloNotaFiscal.Equals("65"));

            if (dataInicial > DateTime.MinValue && dataFinal > DateTime.MinValue)
                result = result.Where(obj => obj.DataEmissao.Value.Date >= dataInicial && obj.DataEmissao.Value.Date <= dataFinal);
            else if (dataInicial > DateTime.MinValue)
                result = result.Where(obj => obj.DataEmissao.Value.Date >= dataInicial);
            else if (dataFinal > DateTime.MinValue)
                result = result.Where(obj => obj.DataEmissao.Value.Date <= dataFinal);

            if (serie > 0)
                result = result.Where(obj => obj.EmpresaSerie.Numero == serie);

            if (!string.IsNullOrWhiteSpace(chave))
                result = result.Where(obj => obj.Chave.Contains(chave));

            if (!string.IsNullOrWhiteSpace(cnpjCpfPessoa))
                result = result.Where(obj => obj.CPFCNPJConsumidorFinal.Contains(cnpjCpfPessoa));

            if (!string.IsNullOrWhiteSpace(nomePessoa))
                result = result.Where(obj => obj.NomeConsumidorFinal.Contains(nomePessoa));

            if (status > 0)
                result = result.Where(obj => obj.Status == status);

            if (empresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo.Equals(empresa));

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal> Consultar(int numeroInicial, int numeroFinal, int serie, int naturezaOperacao, int atividade, double cnpjcpfPessoa, DateTime dataInicial, DateTime dataFinal, DateTime dataProcessamento, DateTime dataSaida, Dominio.Enumeradores.StatusNFe status, Dominio.Enumeradores.TipoEmissaoNFe tipoEmissao, string chave, int empresa, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal>();

            var result = from obj in query select obj;

            if (numeroInicial > 0 && numeroFinal > 0)
                result = result.Where(obj => obj.Numero >= numeroInicial && obj.Numero <= numeroFinal);
            else if (numeroInicial > 0)
                result = result.Where(obj => obj.Numero == numeroInicial);
            else if (numeroFinal > 0)
                result = result.Where(obj => obj.Numero == numeroFinal);

            if (naturezaOperacao > 0)
                result = result.Where(obj => obj.NaturezaDaOperacao.Codigo == naturezaOperacao);

            if (atividade > 0)
                result = result.Where(obj => obj.Atividade.Codigo == atividade);

            if ((int)tipoAmbiente > 0)
                result = result.Where(obj => obj.TipoAmbiente == tipoAmbiente);
            result = result.Where(obj => obj.ModeloNotaFiscal.Equals("55"));

            if (cnpjcpfPessoa > 0)
                result = result.Where(obj => obj.Cliente.CPF_CNPJ == cnpjcpfPessoa);

            if (dataInicial > DateTime.MinValue && dataFinal > DateTime.MinValue)
                result = result.Where(obj => obj.DataEmissao.Value.Date >= dataInicial && obj.DataEmissao.Value.Date <= dataFinal);
            else if (dataInicial > DateTime.MinValue)
                result = result.Where(obj => obj.DataEmissao.Value.Date == dataInicial);
            else if (dataFinal > DateTime.MinValue)
                result = result.Where(obj => obj.DataEmissao.Value.Date == dataFinal);

            if (dataProcessamento > DateTime.MinValue)
                result = result.Where(obj => obj.DataProcessamento.Value.Date >= dataProcessamento && obj.DataProcessamento <= dataProcessamento.AddDays(1));

            if (dataSaida > DateTime.MinValue)
                result = result.Where(obj => obj.DataSaida.Value.Date == dataSaida);

            if (serie > 0)
                result = result.Where(obj => obj.EmpresaSerie.Numero == serie);

            if (!string.IsNullOrWhiteSpace(chave))
                result = result.Where(obj => obj.Chave.Contains(chave));

            if (status > 0)
                result = result.Where(obj => obj.Status == status);

            if (tipoEmissao >= 0)
                result = result.Where(obj => obj.TipoEmissao == tipoEmissao);

            if (empresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo.Equals(empresa));

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

        public int ContarConsulta(int numeroInicial, int numeroFinal, int serie, int naturezaOperacao, int atividade, double cnpjcpfPessoa, DateTime dataInicial, DateTime dataFinal, DateTime dataProcessamento, DateTime dataSaida, Dominio.Enumeradores.StatusNFe status, Dominio.Enumeradores.TipoEmissaoNFe tipoEmissao, string chave, int empresa, Dominio.Enumeradores.TipoAmbiente tipoAmbiente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal>();

            var result = from obj in query select obj;

            if (numeroInicial > 0 && numeroFinal > 0)
                result = result.Where(obj => obj.Numero >= numeroInicial && obj.Numero <= numeroFinal);
            else if (numeroInicial > 0)
                result = result.Where(obj => obj.Numero == numeroInicial);
            else if (numeroFinal > 0)
                result = result.Where(obj => obj.Numero == numeroFinal);

            if (naturezaOperacao > 0)
                result = result.Where(obj => obj.NaturezaDaOperacao.Codigo == naturezaOperacao);

            if (atividade > 0)
                result = result.Where(obj => obj.Atividade.Codigo == atividade);

            if ((int)tipoAmbiente > 0)
                result = result.Where(obj => obj.TipoAmbiente == tipoAmbiente);
            result = result.Where(obj => obj.ModeloNotaFiscal.Equals("55"));

            if (cnpjcpfPessoa > 0)
                result = result.Where(obj => obj.Cliente.CPF_CNPJ == cnpjcpfPessoa);

            if (dataInicial > DateTime.MinValue && dataFinal > DateTime.MinValue)
                result = result.Where(obj => obj.DataEmissao.Value.Date >= dataInicial && obj.DataEmissao.Value.Date <= dataFinal);
            else if (dataInicial > DateTime.MinValue)
                result = result.Where(obj => obj.DataEmissao.Value.Date == dataInicial);
            else if (dataFinal > DateTime.MinValue)
                result = result.Where(obj => obj.DataEmissao.Value.Date == dataFinal);

            if (dataProcessamento > DateTime.MinValue)
                result = result.Where(obj => obj.DataProcessamento.Value.Date >= dataProcessamento && obj.DataProcessamento <= dataProcessamento.AddDays(1));

            if (dataSaida > DateTime.MinValue)
                result = result.Where(obj => obj.DataSaida.Value.Date == dataSaida);

            if (serie > 0)
                result = result.Where(obj => obj.EmpresaSerie.Numero == serie);

            if (!string.IsNullOrWhiteSpace(chave))
                result = result.Where(obj => obj.Chave.Contains(chave));

            if (status > 0)
                result = result.Where(obj => obj.Status == status);

            if (tipoEmissao >= 0)
                result = result.Where(obj => obj.TipoEmissao == tipoEmissao);

            if (empresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo.Equals(empresa));

            return result.Count();
        }

        public int QuantidadeNotaFiscal(int empresa, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, DateTime dataConsulta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal>();

            var result = from obj in query select obj;

            result = result.Where(obj => obj.TipoAmbiente == tipoAmbiente);
            result = result.Where(obj => obj.ModeloNotaFiscal.Equals("55") || obj.ModeloNotaFiscal.Equals("65"));
            result = result.Where(obj => obj.Status == Dominio.Enumeradores.StatusNFe.Autorizado);
            result = result.Where(obj => obj.UltimoStatusSEFAZ != "");

            if (dataConsulta > DateTime.MinValue)
                result = result.Where(obj => obj.DataEmissao.Value.Month == dataConsulta.Month && obj.DataEmissao.Value.Year == dataConsulta.Year);

            if (empresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo.Equals(empresa));

            return result.Count();
        }

        public int QuantidadeNotaFiscalAguardandoAssinatura(int codigoEmpresa, Dominio.Enumeradores.StatusNFe statusNFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal>();

            var result = from obj in query where obj.Status == statusNFe && obj.Empresa.Codigo == codigoEmpresa select obj;

            return result.Count();
        }

        public int ContarBuscarArquivosPorIntegracao(int codigo)
        {
            var queryNotaFiscalEletronicaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalEletronicaIntegracao>();
            var resultNotaFiscalEletronicaIntegracao = from obj in queryNotaFiscalEletronicaIntegracao where obj.Codigo == codigo select obj;

            var queryNotaFiscalEletronicaIntegracaoArquivo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();
            var resultNotaFiscalEletronicaIntegracaoArquivo = from obj in queryNotaFiscalEletronicaIntegracaoArquivo where resultNotaFiscalEletronicaIntegracao.Any(p => p.ArquivosTransacao.Contains(obj)) orderby obj.Data descending select obj;

            return resultNotaFiscalEletronicaIntegracaoArquivo.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> BuscarArquivosPorIntegracao(int codigo, int inicio, int limite)
        {
            var queryNotaFiscalEletronicaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalEletronicaIntegracao>();
            var resultNotaFiscalEletronicaIntegracao = from obj in queryNotaFiscalEletronicaIntegracao where obj.Codigo == codigo select obj;

            var queryNotaFiscalEletronicaIntegracaoArquivo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();
            var resultNotaFiscalEletronicaIntegracaoArquivo = from obj in queryNotaFiscalEletronicaIntegracaoArquivo where resultNotaFiscalEletronicaIntegracao.Any(p => p.ArquivosTransacao.Contains(obj)) orderby obj.Data descending select obj;

            return resultNotaFiscalEletronicaIntegracaoArquivo.Skip(inicio).Take(limite).ToList();
        }

        #endregion

        #region Relatórios

        public IList<Dominio.Relatorios.Embarcador.DataSource.NFe.DANFE> BuscarDANFE(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal notaFiscal, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite, bool isRelatorio = false)
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectDANFE(false, propriedades, notaFiscal, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite, isRelatorio));

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.NFe.DANFE)));

            return query.List<Dominio.Relatorios.Embarcador.DataSource.NFe.DANFE>();
        }

        private string ObterSelectDANFE(bool count, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal notaFiscal, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite, bool isRelatorio)
        {
            string select = string.Empty;

            select = @" SELECT N.NFI_CODIGO CodigoNota, 
                     N.NFI_NUMERO Numero, 
                     S.ESE_NUMERO Serie, 
                     E.EMP_RAZAO RazaoEmpresa, 
                     E.EMP_ENDERECO RuaEmpresa, 
                     E.EMP_BAIRRO BairroEmpresa, 
                     E.EMP_NUMERO NumeroEmpresa, 
                     E.EMP_COMPLEMENTO ComplementoEmpresa, 
                     LE.LOC_DESCRICAO CidadeEmpresa, 
                     LE.UF_SIGLA EstadoEmpresa, 
                     E.EMP_CEP CEPEmpresa, 
                     E.EMP_FONE FoneEmpresa, 
                     N.NFI_TIPO_EMISSAO TipoNota, 
                     N.NFI_CHAVE Chave, 
                     O.NAT_DESCRICAO NaturezaOperacao, 
                     E.EMP_INSCRICAO IEEmpresa, 
                     E.EMP_INSCRICAO_ST IESTEmpresa, 
                     E.EMP_CNPJ CNPJEmpresa, 
                     ISNULL(E.EMP_TIPO, 'J') TipoEmpresa, 
                     N.NFI_AMBIENTE TipoAmbiente, 
                     N.NFI_PROTOCOLO Protocolo, 
                     N.NFI_DATA_PROCESSAMENTO DataProcessamento, 
                     CASE 
                        WHEN N.NFI_AMBIENTE = 2 THEN 'NF-E EMITIDA EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL' 
                        ELSE C.CLI_NOME 
                      END NomeCliente,   
                     C.CLI_FISJUR TipoCliente, 
                     C.CLI_CGCCPF CNPJCPFCliente, 
                     C.CLI_ENDERECO EnderecoCliente, 
                     C.CLI_NUMERO NumeroCliente, 
                     C.CLI_COMPLEMENTO ComplementoCliente, 
                     C.CLI_BAIRRO BairroCliente, 
                     LC.LOC_DESCRICAO CidadeCliente, 
                     C.CLI_CEP CEPCliente, 
                     C.CLI_FONE FoneCliente, 
                     LC.UF_SIGLA EstadoCliente, 
                     C.CLI_IERG IECliente, 
                     N.NFI_DATA_EMISSAO DataEmissao, 
                     N.NFI_DATA_SAIDA DataSaida, 
                     N.NFI_DATA_SAIDA HoraSaida, 
                     N.NFI_BC_ICMS BCICMS, 
                     N.NFI_VALOR_ICMS ValorICMS, 
                     N.NFI_BC_ICMS_ST BCICMSST, 
                     N.NFI_VALOR_ICMS_ST ValorICMSST, 
                     N.NFI_VALOR_PRODUTOS ValorProduto, 
                     N.NFI_VALOR_FRETE ValorFrete, 
                     N.NFI_VALOR_SEGURO ValorSeguro, 
                     N.NFI_VALOR_DESCONTO ValorDesconto, 
                     N.NFI_VALOR_OUTRAS_DESPESAS ValorOutras, 
                     N.NFI_VALOR_IPI ValorIPI, 
                     N.NFI_VALOR_TOTAL_NOTA ValorTotal, 
                     N.NFI_TRANSP_NOME_TRANSPORTADORA RazaoTransportadora, 
                     N.NFI_TRANSP_TIPO_FRETE TipoFreteTransportadora, 
                     N.NFI_TRANSP_ANTT_VEICULO ANTTTransportadora, 
                     N.NFI_TRANSP_PLACA_VEICULO VeiculoTransportadora, 
                     N.NFI_TRANSP_UF_VEICULO EstadoVeiculoTransportadora, 
                     N.NFI_TRANSP_CNPJ_CPF_TRANSPORTADORA CNPJCPFTransportadora, 
                     N.NFI_TRANSP_ENDERECO_TRANSPORTADORA EnderecoTransportadora, 
                     ISNULL(LT.LOC_DESCRICAO, '') CidadeTransportadora,  
                     N.NFI_TRANSP_UF_TRANSPORTADORA EstadoTransportadora, 
                     N.NFI_TRANSP_IE_TRANSPORTADORA IETransportadora, 
                     N.NFI_TRANSP_QUANTIDADE QuantidadeCargaTransportadora, 
                     N.NFI_TRANSP_ESPECIE EspecieCargaTransportadora, 
                     N.NFI_TRANSP_MARCA MarcaCargaTransportadora, 
                     N.NFI_TRANSP_VOLUME NumeroCargaTransportadora, 
                     N.NFI_TRANSP_PESO_BRUTO PesoBrutoCargaTransportadora, 
                     N.NFI_TRANSP_PESO_LIQUIDO PesoLiquidoCargaTransportadora, 
                     E.EMP_INSCRICAO_MUNICIPAL IMEmpresa, 
                     N.NFI_VALOR_SERVICOS ValorServicos, 
                     N.NFI_BC_ISSQN BCISS, 
                     N.NFI_VALOR_ISSQN ValorISS, 
                     ISNULL(N.NFI_OBSERVACAO_NOTA, '') Observacao, 
                     ISNULL(N.NFI_OBSERVACAO_TRIBUTARIA, '') InformacoesComplementares, 

                     I.NFP_CODIGO CodigoItem, 
                     CASE 
                         WHEN I.PRO_CODIGO IS NULL THEN I.SER_CODIGO 
                         ELSE I.PRO_CODIGO 
                     END  CodigoProduto, 
                     CASE 
                         WHEN I.NFP_CODIGO_ITEM IS NOT NULL AND NFP_CODIGO_ITEM <> '' THEN I.NFP_CODIGO_ITEM 
                         WHEN I.PRO_CODIGO IS NULL THEN SS.SER_CODIGO_TRIBUTACAO 
                         ELSE P.PRO_COD_PRODUTO 
                     END  CodigoProdutoIntegracao, 
                     ISNULL(I.NFP_DESCRICAO_ITEM, 
                     CASE 
                         WHEN I.PRO_CODIGO IS NULL THEN SS.SER_DESCRICAO 
                         ELSE P.PRO_DESCRICAO 
                     END) DescricaoItem, 
                     CASE WHEN P.PRO_COD_NCM IS NULL THEN '00' ELSE P.PRO_COD_NCM END CodigoNCMItem, 
                     I.NFP_CST_CSOSN CSTItem, 
                     ISNULL(I.NFP_ORIGEM_MERCADORIA, ISNULL(P.PRO_ORIGEM_MERCADORIA, 1)) OrigemItem, 
                     CP.CFO_CFOP CodigoCFOPItem, 
                     CASE WHEN P.PRO_UNIDADE_MEDIDA IS NULL THEN 7 ELSE P.PRO_UNIDADE_MEDIDA END UnidadeItem, 
                     I.NFP_QUANTIDADE QuantidadeItem, 
                     I.NFP_VALOR_UNITARIO ValorUnitarioItem, 
                     I.NFP_VALOR_TOTAL ValorTotalItem, 
                     I.NFP_BC_ICMS BCICMSItem, 
                     I.NFP_VALOR_ICMS ValorICMSItem, 
                     I.NFP_VALOR_IPI ValorIPIItem, 
                     I.NFP_ALIQUOTA_ICMS AliquotaICMSItem, 
                     I.NFP_ALIQUOTA_IPI AliquotaIPIItem, 
                     NFI_VALOR_IMPOSTO_OBPT ImpostoIBPT, 
                     I.NFP_INFORMACOES_ADICIONAIS InformacoesAdicionaisItem,
					 CAST(ISNULL(CAST(SUBSTRING((SELECT DISTINCT ', ' + 'LOTE: ' + E.NPL_NUMERO_LOTE + ' QTD: '+ CAST(E.NPL_QUANTIDADE_LOTE AS VARCHAR(20)) + ' VENC.: ' + CONVERT(VARCHAR(10), E.NPL_DATA_VALIDADE, 103)
						FROM T_NOTA_FISCAL_PRODUTOS_LOTES E
						WHERE E.NFP_CODIGO = I.NFP_CODIGO FOR XML PATH('')), 3, 2000) AS VARCHAR(2000)), '') AS VARCHAR(2000)) LotesProduto,
                     E.EMP_CASAS_QUANTIDADE_PRODUTO_NFE CasasDecimaisQuantidade,
                     E.EMP_CASAS_VALOR_PRODUTO_NFE CasasDecimaisValorUnitario,
					 N.NFI_UTILIZAR_ENDERECO_RETIRADA UtilizarEnderecoRetirada,
					 CliRetirada.CLI_NOME  ClienteRetirada,
					 ISNULL(LocRetirada.LOC_DESCRICAO, '') + ' / ' + ISNULL(LocRetirada.UF_SIGLA, '')  LocalidadeRetirada,
					 N.NFI_RETIRADA_LOGRADOURO RetiradaLogradouro,
					 N.NFI_RETIRADA_NUMERO_LOGRADOURO RetiradaNumeroLogradouro,
					 N.NFI_RETIRADA_COMPLEMENTO_LOGRADOURO RetiradaComplementoLogradouro,
					 N.NFI_RETIRADA_BAIRRO  RetiradaBairro,
					 N.NFI_RETIRADA_CEP  RetiradaCEP,
					 N.NFI_RETIRADA_TELEFONE  RetiradaTelefone,
					 N.NFI_RETIRADA_EMAIL RetiradaEmail,
					 N.NFI_RETIRADA_IE RetiradaIE,
					 N.NFI_UTILIZAR_ENDERECO_ENTREGA  UtilizarEnderecoEntrega,
					 CliEntrega.CLI_NOME  ClienteEntrega,
					 ISNULL(LocEntrega.LOC_DESCRICAO, '') + ' / ' + ISNULL(LocEntrega.UF_SIGLA, '') LocalidadeEntrega,
					 N.NFI_ENTREGA_LOGRADOURO EntregaLogradouro,
					 N.NFI_ENTREGA_NUMERO_LOGRADOURO EntregaNumeroLogradouro,
					 N.NFI_ENTREGA_COMPLEMENTO_LOGRADOURO EntregaComplementoLogradouro,
					 N.NFI_ENTREGA_BAIRRO EntregaBairro,
					 N.NFI_ENTREGA_CEP EntregaCEP,
					 N.NFI_ENTREGA_TELEFONE EntregaTelefone,
					 N.NFI_ENTREGA_EMAIL EntregaEmail,
					 N.NFI_ENTREGA_IE EntregaIE,

					 CliRetirada.CLI_FISJUR TipoClienteRetirada,
					 CliRetirada.CLI_CGCCPF CNPJCPFClienteRetirada,
					 CliEntrega.CLI_FISJUR TipoClienteEntrega,
					 CliEntrega.CLI_CGCCPF CNPJCPFClienteEntrega
						
                     FROM T_NOTA_FISCAL N 
                     JOIN T_NOTA_FISCAL_PRODUTOS I ON I.NFI_CODIGO = N.NFI_CODIGO 
                     JOIN T_EMPRESA_SERIE S ON S.ESE_CODIGO = N.ESE_CODIGO 
                     JOIN T_EMPRESA E ON E.EMP_CODIGO = N.EMP_CODIGO 
                     JOIN T_LOCALIDADES LE ON LE.LOC_CODIGO = E.LOC_CODIGO 
                     JOIN T_NATUREZAOPERACAO O ON O.NAT_CODIGO = N.NAT_CODIGO 
                     JOIN T_CLIENTE C ON C.CLI_CGCCPF = N.CLI_CGCCPF 
                     JOIN T_LOCALIDADES LC ON LC.LOC_CODIGO = C.LOC_CODIGO 
                     LEFT OUTER JOIN T_PRODUTO P ON P.PRO_CODIGO = I.PRO_CODIGO 
                     LEFT OUTER JOIN T_SERVICO SS ON SS.SER_CODIGO = I.SER_CODIGO 
                     LEFT OUTER JOIN T_CFOP CP ON CP.CFO_CODIGO = I.CFO_CODIGO 
                     LEFT OUTER JOIN T_CLIENTE CliRetirada ON CliRetirada.CLI_CGCCPF = N.CLI_CGCCPF_RETIRADA 
					 LEFT OUTER JOIN T_LOCALIDADES LocRetirada ON LocRetirada.LOC_CODIGO = N.LOC_CODIGO_RETIRADA 					 
					 LEFT OUTER JOIN T_CLIENTE CliEntrega ON CliEntrega.CLI_CGCCPF = N.CLI_CGCCPF_ENTREGA 
					 LEFT OUTER JOIN T_LOCALIDADES LocEntrega ON LocEntrega.LOC_CODIGO = N.LOC_CODIGO_ENTREGA 
					 LEFT OUTER JOIN T_LOCALIDADES LT ON LT.LOC_CODIGO = N.LOC_CODIGO_TRANSP_MUNICIPIO_TRANSPORTADORA 
                     WHERE N.NFI_CODIGO = " + notaFiscal.Codigo;
            return select;
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.NFe.NotasEmitidas> RelatorioNotasEmitidas(Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNotasEmitidas filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var parametros = new List<ParametroSQL>();

            string queryWhereNFS = "";
            string query = @"SELECT N.NFI_CODIGO Codigo, N.NFI_CODIGO CodigoNFe, 0 CodigoNFSe, 1 TipoNota,
                N.NFI_NUMERO Numero, 
                S.ESE_NUMERO Serie, 
                N.NFI_DATA_EMISSAO DataEmissao, 
                N.NFI_DATA_SAIDA DataSaida, 
                ISNULL(LTRIM(STR(C.CLI_CGCCPF, 25, 0)), ISNULL(N.NFI_CPF_CNPJ_CONSUMIDOR, '')) CpfCnpjPessoa, 
                ISNULL(C.CLI_FISJUR, '') TipoPessoa, 
                ISNULL(C.CLI_NOME, ISNULL(N.NFI_NOME_CONSUMIDOR, '')) Pessoa, 
                CASE 
                    WHEN N.NFI_STATUS = 1 THEN 'Emitido' 
                    WHEN N.NFI_STATUS = 2 THEN 'Inutilizado' 
                    WHEN N.NFI_STATUS = 3 THEN 'Cancelado' 
                    WHEN N.NFI_STATUS = 4 THEN 'Autorizado' 
                    WHEN N.NFI_STATUS = 5 THEN 'Denegado' 
                    WHEN N.NFI_STATUS = 6 THEN 'Rejeitado' 
                    WHEN N.NFI_STATUS = 7 THEN 'Em Processamento' 
                    ELSE 'Indefinido' 
                END DescricaoStatus, 
                ISNULL(O.NAT_DESCRICAO, 'VENDA PARA CONSUMIDOR FINAL (NFC-e)') DescricaoNaturezaOperacao, 
                CASE 
                    WHEN N.NFI_FINALIDADE = 1 THEN 'Normal' 
                    WHEN N.NFI_FINALIDADE = 2 THEN 'Complementar' 
                    WHEN N.NFI_FINALIDADE = 3 THEN 'Ajuste' 
                    WHEN N.NFI_FINALIDADE = 4 THEN 'Devolução' 
                    ELSE 'Indefinido' 
                END DescricaoFinalidade, 
                CASE 
                    WHEN N.NFI_TIPO_EMISSAO = 0 THEN 'Entrada' 
                    WHEN N.NFI_TIPO_EMISSAO = 1 THEN 'Saída' 
                    ELSE 'Indefinido' 
                END DescricaoTipoEmissao, 
                N.NFI_CHAVE Chave, 
                A.ATI_DESCRICAO DescricaoAtividade, 
                N.NFI_VALOR_TOTAL_NOTA ValorTotal, 
                N.NFI_VALOR_PRODUTOS ValorTotalProdutos, 
                N.NFI_VALOR_SERVICOS ValorTotalServicos, 
                CASE 
                    WHEN N.NFI_TRANSP_TIPO_FRETE = 0 THEN 'Remetente (CIF)' 
                    WHEN N.NFI_TRANSP_TIPO_FRETE = 1 THEN 'Destinatário (FOB)' 
                    WHEN N.NFI_TRANSP_TIPO_FRETE = 2 THEN 'Terceiros' 
                    WHEN N.NFI_TRANSP_TIPO_FRETE = 3 THEN 'Próprio do Remetente' 
                    WHEN N.NFI_TRANSP_TIPO_FRETE = 4 THEN 'Próprio do Destinatário' 
                    WHEN N.NFI_TRANSP_TIPO_FRETE = 9 THEN 'Sem Frete' 
                    ELSE 'Indefinido' 
                END DescricaoTipoFrete, 
                N.NFI_TRANSP_PESO_BRUTO PesoBruto, 
                N.NFI_TRANSP_PESO_LIQUIDO PesoLiquido, 
                ISNULL(F.FUN_NOME, 'NÃO INFORMADO') Usuario, 
                N.NFI_OBSERVACAO_NOTA Observacao, 
                N.NFI_OBSERVACAO_TRIBUTARIA ObservacaoTributaria, 
                SUBSTRING((SELECT DISTINCT ', ' + CAST(cfop.CFO_CFOP AS NVARCHAR(10))
		                FROM T_CFOP cfop
                        JOIN T_NOTA_FISCAL_PRODUTOS notaProdutos on notaProdutos.CFO_CODIGO = cfop.CFO_CODIGO
		                WHERE notaProdutos.NFI_CODIGO = N.NFI_CODIGO FOR XML PATH('')), 3, 1000) AS CFOP

                 FROM T_NOTA_FISCAL N 
                 JOIN T_EMPRESA_SERIE S ON S.ESE_CODIGO = N.ESE_CODIGO 
                 LEFT OUTER JOIN T_NATUREZAOPERACAO O ON O.NAT_CODIGO = N.NAT_CODIGO 
                 LEFT OUTER JOIN T_ATIVIDADES A ON A.ATI_CODIGO = N.ATI_CODIGO 
                 LEFT OUTER JOIN T_CLIENTE C ON C.CLI_CGCCPF = N.CLI_CGCCPF 
                 LEFT OUTER JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = N.FUN_CODIGO 
                WHERE 1 = 1 ";

            string queryNFS = @"SELECT N.CON_CODIGO Codigo, 0 CodigoNFe, N.CON_CODIGO CodigoNFSe, 2 TipoNota,
                 N.CON_NUM Numero,  
                 N.CON_SERIE Serie,  
                 N.CON_DATAHORAEMISSAO DataEmissao,  
                 N.CON_DATAHORAEMISSAO DataSaida,  
                 C.PCT_CPF_CNPJ CpfCnpjPessoa, 
                 '' TipoPessoa, 
                 C.PCT_NOME Pessoa,  
                 CASE  
                     WHEN N.CON_STATUS = 'A' THEN 'Autorizado'   
                     WHEN N.CON_STATUS = 'C' THEN 'Cancelado'  
                     WHEN N.CON_STATUS = 'S' THEN 'Em Digitação'  
                     WHEN N.CON_STATUS = 'R' THEN 'Rejeitado'  
                     ELSE 'Indefinido'  
                 END DescricaoStatus,  
                 NN.NAN_DESCRICAO DescricaoNaturezaOperacao,    
                 '' DescricaoFinalidade,   
                 '' DescricaoTipoEmissao,  
                 '' Chave,  
                 '' DescricaoAtividade,  
                 N.CON_VALOR_PREST_SERVICO ValorTotal,  
                 0.00 ValorTotalProdutos,  
                 N.CON_VALOR_PREST_SERVICO ValorTotalServicos,  
                 '' DescricaoTipoFrete,  
                 0.00 PesoBruto,  
                 0.00 PesoLiquido,
                 ISNULL(F.FUN_NOME, 'NÃO INFORMADO') Usuario,
                 N.CON_OBSGERAIS Observacao,
                 '' ObservacaoTributaria,
                 '' CFOP  
                 FROM T_CTE N
                 JOIN T_NFSE_NATUREZA NN ON NN.NAN_CODIGO = N.NAN_CODIGO
                 JOIN T_CTE_PARTICIPANTE C ON C.PCT_CODIGO = N.CON_REMETENTE_CTE
                 JOIN T_MODDOCFISCAL MOD ON MOD.MOD_CODIGO = CON_MODELODOC
                 LEFT OUTER JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = N.CON_USUARIO
                 WHERE MOD.MOD_NUM = '39' ";

            if (filtrosPesquisa.NumeroInicial > 0 && filtrosPesquisa.NumeroFinal == 0)
            {
                query += " AND N.NFI_NUMERO = " + filtrosPesquisa.NumeroInicial.ToString();
                queryWhereNFS += " AND N.CON_NUM = " + filtrosPesquisa.NumeroInicial.ToString();
            }
            else if (filtrosPesquisa.NumeroInicial > 0 && filtrosPesquisa.NumeroFinal > 0)
            {
                query += " AND N.NFI_NUMERO >= " + filtrosPesquisa.NumeroInicial.ToString() + " AND N.NFI_NUMERO <= " + filtrosPesquisa.NumeroFinal.ToString();
                queryWhereNFS += " AND N.CON_NUM >= " + filtrosPesquisa.NumeroInicial.ToString() + " AND N.CON_NUM <= " + filtrosPesquisa.NumeroFinal.ToString();
            }
            else if (filtrosPesquisa.NumeroInicial == 0 && filtrosPesquisa.NumeroFinal > 0)
            {
                query += " AND N.NFI_NUMERO = " + filtrosPesquisa.NumeroFinal.ToString();
                queryWhereNFS += " AND N.CON_NUM = " + filtrosPesquisa.NumeroFinal.ToString();
            }

            if (filtrosPesquisa.CodigoEmpresa > 0)
            {
                query += " AND N.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresa.ToString();
                queryWhereNFS += " AND N.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresa.ToString();
                if (filtrosPesquisa.TipoAmbiente > 0)
                {
                    query += " AND N.NFI_AMBIENTE = " + (int)filtrosPesquisa.TipoAmbiente;
                    queryWhereNFS += " AND CON_TIPO_AMBIENTE = " + (int)filtrosPesquisa.TipoAmbiente;
                }
            }

            if (filtrosPesquisa.Serie > 0)
            {
                query += " AND S.ESE_NUMERO = " + filtrosPesquisa.Serie.ToString();
                queryWhereNFS += " AND N.CON_SERIE = " + filtrosPesquisa.Serie.ToString();
            }

            if (filtrosPesquisa.FormaEmissao == 1)
                query += " AND N.NFI_ULTIMO_STATUS_SEFAZ IS NOT NULL ";

            if (filtrosPesquisa.CodigoAtividade > 0)
                query += " AND N.ATI_CODIGO = " + filtrosPesquisa.CodigoAtividade.ToString();

            if (filtrosPesquisa.CodigoNaturezaOperacao > 0)
                query += " AND N.NAT_CODIGO = " + filtrosPesquisa.CodigoNaturezaOperacao.ToString();

            if (filtrosPesquisa.CnpjPessoa > 0)
            {
                query += " AND N.CLI_CGCCPF = " + filtrosPesquisa.CnpjPessoa.ToString();
                queryWhereNFS += " AND C.PCT_CPF_CNPJ LIKE '%" + filtrosPesquisa.CnpjPessoa.ToString() + "'";
            }

            string pattern = "yyyy-MM-dd";
            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
            {
                query += " AND CAST(N.NFI_DATA_EMISSAO AS DATE) >= '" + filtrosPesquisa.DataInicial.ToString(pattern) + "'";
                queryWhereNFS += " AND CAST(N.CON_DATAHORAEMISSAO AS DATE) >= '" + filtrosPesquisa.DataInicial.ToString(pattern) + "'";
            }

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
            {
                query += " AND CAST(N.NFI_DATA_EMISSAO AS DATE) <= '" + filtrosPesquisa.DataFinal.ToString(pattern) + "'";
                queryWhereNFS += " AND CAST(N.CON_DATAHORAEMISSAO AS DATE) <= '" + filtrosPesquisa.DataFinal.ToString(pattern) + "'";
            }

            if (filtrosPesquisa.DataProcessamento != DateTime.MinValue)
            {
                query += " AND CAST(N.NFI_DATA_PROCESSAMENTO AS DATE) = '" + filtrosPesquisa.DataProcessamento.ToString(pattern) + "'";
                queryWhereNFS += " AND CAST(N.CON_RETORNOCTEDATA AS DATE) = '" + filtrosPesquisa.DataProcessamento.ToString(pattern) + "'";
            }

            if (filtrosPesquisa.DataSaida != DateTime.MinValue)
            {
                query += " AND CAST(N.NFI_DATA_SAIDA AS DATE) = '" + filtrosPesquisa.DataSaida.ToString(pattern) + "'";
                queryWhereNFS += " AND CAST(N.CON_DATAHORAEMISSAO AS DATE) = '" + filtrosPesquisa.DataSaida.ToString(pattern) + "'";
            }

            if (filtrosPesquisa.Status > 0)
                query += " AND N.NFI_STATUS = '" + (int)filtrosPesquisa.Status + "'";

            if (filtrosPesquisa.TipoEmissao.HasValue)
                query += " AND N.NFI_TIPO_EMISSAO = '" + (int)filtrosPesquisa.TipoEmissao.Value + "'";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Chave))
            {
                query += " AND N.NFI_CHAVE = :N_NFI_CHAVE";
                parametros.Add(new ParametroSQL("N_NFI_CHAVE", filtrosPesquisa.Chave));
            }

            if (filtrosPesquisa.CodigosUsuario.Count > 0)
            {
                query += " AND (N.FUN_CODIGO IN (" + string.Join(",", filtrosPesquisa.CodigosUsuario) + "))";
                queryWhereNFS += " AND (N.CON_USUARIO IN (" + string.Join(",", filtrosPesquisa.CodigosUsuario) + "))";
            }

            if (filtrosPesquisa.CodigosCFOP.Count > 0)
            {
                query += @" AND N.NFI_CODIGO IN (SELECT notaProdutos.NFI_CODIGO FROM T_CFOP cfop
                                                    JOIN T_NOTA_FISCAL_PRODUTOS notaProdutos on notaProdutos.CFO_CODIGO = cfop.CFO_CODIGO
		                                            WHERE notaProdutos.NFI_CODIGO = N.NFI_CODIGO and cfop.CFO_CODIGO IN (" + string.Join(",", filtrosPesquisa.CodigosCFOP) + "))";
                queryWhereNFS += " AND 1 = 0";
            }

            if (filtrosPesquisa.TipoDocumento == TipoNota.NFe)
                query = query + "";
            else if (filtrosPesquisa.TipoDocumento == TipoNota.NFSe)
                query = queryNFS + queryWhereNFS;
            else
                query = "SELECT * FROM (" + query + " UNION ALL " + queryNFS + queryWhereNFS + ") AS T"; 

            var agrup = false;
            if (!string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeAgrupar))
            {
                agrup = true;
                query += " order by " + parametrosConsulta.PropriedadeAgrupar + " " + parametrosConsulta.DirecaoAgrupar;
            }

            if (!string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeOrdenar) && parametrosConsulta.PropriedadeAgrupar != parametrosConsulta.PropriedadeOrdenar)
            {
                if (agrup)
                {
                    query += ", " + parametrosConsulta.PropriedadeOrdenar + " " + parametrosConsulta.DirecaoOrdenar;
                }
                else
                {
                    query += " order by " + parametrosConsulta.PropriedadeOrdenar + " " + parametrosConsulta.DirecaoOrdenar;
                }
            }

            if (parametrosConsulta.LimiteRegistros > 0)
                query += " OFFSET " + parametrosConsulta.InicioRegistros + " ROWS FETCH FIRST " + parametrosConsulta.LimiteRegistros + " ROWS ONLY";

            var sqlDinamico = new SQLDinamico(query, parametros);

            var nhQuery = sqlDinamico.CriarSQLQuery(this.SessionNHiBernate);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.NFe.NotasEmitidas)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.NFe.NotasEmitidas>();
        }

        public int ContarRelatorioNotasEmitidas(Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNotasEmitidas filtrosPesquisa)
        {
            var parametros = new List<ParametroSQL>();

            string queryWhereNFS = "";
            string query = @"SELECT N.NFI_CODIGO 
                 FROM T_NOTA_FISCAL N 
                 JOIN T_EMPRESA_SERIE S ON S.ESE_CODIGO = N.ESE_CODIGO 
                 LEFT OUTER JOIN T_NATUREZAOPERACAO O ON O.NAT_CODIGO = N.NAT_CODIGO 
                 LEFT OUTER JOIN T_ATIVIDADES A ON A.ATI_CODIGO = N.ATI_CODIGO 
                 LEFT OUTER JOIN T_CLIENTE C ON C.CLI_CGCCPF = N.CLI_CGCCPF 
                 WHERE 1 = 1 ";

            string queryNFS = @"SELECT N.CON_CODIGO  
                 FROM T_CTE N
                 JOIN T_NFSE_NATUREZA NN ON NN.NAN_CODIGO = N.NAN_CODIGO
                 JOIN T_CTE_PARTICIPANTE C ON C.PCT_CODIGO = N.CON_REMETENTE_CTE
                 JOIN T_MODDOCFISCAL MOD ON MOD.MOD_CODIGO = CON_MODELODOC
                 WHERE MOD.MOD_NUM = '39' ";

            if (filtrosPesquisa.CodigoEmpresa > 0)
            {
                query += " AND N.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresa.ToString();
                queryWhereNFS += " AND N.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresa.ToString();
                if (filtrosPesquisa.TipoAmbiente > 0)
                {
                    query += " AND N.NFI_AMBIENTE = " + (int)filtrosPesquisa.TipoAmbiente;
                    queryWhereNFS += " AND CON_TIPO_AMBIENTE = " + (int)filtrosPesquisa.TipoAmbiente;
                }
            }

            if (filtrosPesquisa.NumeroInicial > 0 && filtrosPesquisa.NumeroFinal == 0)
            {
                query += " AND N.NFI_NUMERO = " + filtrosPesquisa.NumeroInicial.ToString();
                queryWhereNFS += " AND N.CON_NUM = " + filtrosPesquisa.NumeroInicial.ToString();
            }
            else if (filtrosPesquisa.NumeroInicial > 0 && filtrosPesquisa.NumeroFinal > 0)
            {
                query += " AND N.NFI_NUMERO >= " + filtrosPesquisa.NumeroInicial.ToString() + " AND N.NFI_NUMERO <= " + filtrosPesquisa.NumeroFinal.ToString();
                queryWhereNFS += " AND N.CON_NUM >= " + filtrosPesquisa.NumeroInicial.ToString() + " AND N.CON_NUM <= " + filtrosPesquisa.NumeroFinal.ToString();
            }
            else if (filtrosPesquisa.NumeroInicial == 0 && filtrosPesquisa.NumeroFinal > 0)
            {
                query += " AND N.NFI_NUMERO = " + filtrosPesquisa.NumeroFinal.ToString();
                queryWhereNFS += " AND N.CON_NUM = " + filtrosPesquisa.NumeroFinal.ToString();
            }

            if (filtrosPesquisa.Serie > 0)
            {
                query += " AND S.ESE_NUMERO = " + filtrosPesquisa.Serie.ToString();
                queryWhereNFS += " AND N.CON_SERIE = " + filtrosPesquisa.Serie.ToString();
            }

            if (filtrosPesquisa.FormaEmissao == 1)
                query += " AND N.NFI_ULTIMO_STATUS_SEFAZ IS NOT NULL ";

            if (filtrosPesquisa.CodigoAtividade > 0)
                query += " AND N.ATI_CODIGO = " + filtrosPesquisa.CodigoAtividade.ToString();

            if (filtrosPesquisa.CodigoNaturezaOperacao > 0)
                query += " AND N.NAT_CODIGO = " + filtrosPesquisa.CodigoNaturezaOperacao.ToString();

            if (filtrosPesquisa.CnpjPessoa > 0)
            {
                query += " AND N.CLI_CGCCPF = " + filtrosPesquisa.CnpjPessoa.ToString();
                queryWhereNFS += " AND C.PCT_CPF_CNPJ LIKE '%" + filtrosPesquisa.CnpjPessoa.ToString() + "'";
            }

            string pattern = "yyyy-MM-dd";
            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
            {
                query += " AND CAST(N.NFI_DATA_EMISSAO AS DATE) >= '" + filtrosPesquisa.DataInicial.ToString(pattern) + "'";
                queryWhereNFS += " AND CAST(N.CON_DATAHORAEMISSAO AS DATE) >= '" + filtrosPesquisa.DataInicial.ToString(pattern) + "'";
            }

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
            {
                query += " AND CAST(N.NFI_DATA_EMISSAO AS DATE) <= '" + filtrosPesquisa.DataFinal.ToString(pattern) + "'";
                queryWhereNFS += " AND CAST(N.CON_DATAHORAEMISSAO AS DATE) <= '" + filtrosPesquisa.DataFinal.ToString(pattern) + "'";
            }

            if (filtrosPesquisa.DataProcessamento != DateTime.MinValue)
            {
                query += " AND CAST(N.NFI_DATA_PROCESSAMENTO AS DATE) = '" + filtrosPesquisa.DataProcessamento.ToString(pattern) + "'";
                queryWhereNFS += " AND CAST(N.CON_RETORNOCTEDATA AS DATE) = '" + filtrosPesquisa.DataProcessamento.ToString(pattern) + "'";
            }

            if (filtrosPesquisa.DataSaida != DateTime.MinValue)
            {
                query += " AND CAST(N.NFI_DATA_SAIDA AS DATE) = '" + filtrosPesquisa.DataSaida.ToString(pattern) + "'";
                queryWhereNFS += " AND CAST(N.CON_DATAHORAEMISSAO AS DATE) = '" + filtrosPesquisa.DataSaida.ToString(pattern) + "'";
            }

            if (filtrosPesquisa.Status > 0)
                query += " AND N.NFI_STATUS = '" + (int)filtrosPesquisa.Status + "'";

            if (filtrosPesquisa.TipoEmissao.HasValue)
                query += " AND N.NFI_TIPO_EMISSAO = '" + (int)filtrosPesquisa.TipoEmissao.Value + "'";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Chave))
            {
                query += " AND N.NFI_CHAVE = :N_NFI_CHAVE";
                parametros.Add(new ParametroSQL("N_NFI_CHAVE", filtrosPesquisa.Chave));
            }

            if (filtrosPesquisa.CodigosUsuario.Count > 0)
            {
                query += " AND (N.FUN_CODIGO IN (" + string.Join(",", filtrosPesquisa.CodigosUsuario) + "))";
                queryWhereNFS += " AND (N.CON_USUARIO IN (" + string.Join(",", filtrosPesquisa.CodigosUsuario) + "))";
            }

            if (filtrosPesquisa.CodigosCFOP.Count > 0)
            {
                query += @" AND N.NFI_CODIGO IN (SELECT notaProdutos.NFI_CODIGO FROM T_CFOP cfop
                                                    JOIN T_NOTA_FISCAL_PRODUTOS notaProdutos on notaProdutos.CFO_CODIGO = cfop.CFO_CODIGO
		                                            WHERE notaProdutos.NFI_CODIGO = N.NFI_CODIGO and cfop.CFO_CODIGO IN (" + string.Join(",", filtrosPesquisa.CodigosCFOP) + "))";
                queryWhereNFS += " AND 1 = 0";
            }

            if (filtrosPesquisa.TipoDocumento == TipoNota.NFe)
                query = "SELECT COUNT(0) as CONTADOR FROM (" + query + ") AS T"; 
            else if (filtrosPesquisa.TipoDocumento == TipoNota.NFSe)
                query = "SELECT COUNT(0) as CONTADOR FROM (" + queryNFS + queryWhereNFS + ") AS T"; 
            else
                query = "SELECT COUNT(0) as CONTADOR FROM (" + query + " UNION ALL " + queryNFS + queryWhereNFS + ") AS T"; 

            var sqlDinamico = new SQLDinamico(query, parametros);

            var nhQuery = sqlDinamico.CriarSQLQuery(this.SessionNHiBernate);

            return nhQuery.UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.NFe.NotasEmitidasItens> ConsultarItensRelatorioNotasEmitidas(string codigosNFe, string codigosNFSe)
        {
            var parametros = new List<ParametroSQL>();

            string query = @"SELECT NFI_CODIGO CodigoNF, 1 TipoNota,
                                CASE
                                    WHEN IT.PRO_CODIGO IS NOT NULL THEN IT.PRO_CODIGO
                                    ELSE IT.SER_CODIGO
                                END Codigo, 
                                CASE
                                    WHEN P.PRO_DESCRICAO IS NOT NULL THEN P.PRO_DESCRICAO
                                    ELSE S.SER_DESCRICAO
                                END Descricao,
                                CASE
                                    WHEN P.PRO_UNIDADE_MEDIDA IS NOT NULL THEN P.PRO_UNIDADE_MEDIDA
                                    ELSE 7 --SERV
                                END UnidadeMedida,
                                IT.NFP_QUANTIDADE Quantidade, 
                                IT.NFP_VALOR_UNITARIO ValorUnitario, 
                                IT.NFP_VALOR_TOTAL ValorTotal

                                FROM T_NOTA_FISCAL_PRODUTOS IT
                                LEFT OUTER JOIN T_PRODUTO P ON P.PRO_CODIGO = IT.PRO_CODIGO
                                LEFT OUTER JOIN T_SERVICO S ON S.SER_CODIGO = IT.SER_CODIGO";

            if (!string.IsNullOrWhiteSpace(codigosNFe))
                query += " where NFI_CODIGO in (" + codigosNFe + ")";
            else
                query += " where 1 = 0";

            string queryNFS = @"SELECT CON_CODIGO CodigoNF, 2 TipoNota,
                                    IT.SER_CODIGO Codigo,
                                    S.SER_DESCRICAO Descricao,
                                    7 UnidadeMedida, --SERV 
                                    IT.NFI_QUANTIDADE Quantidade, 
                                    IT.NFI_VALOR_SERVICO ValorUnitario, 
                                    IT.NFI_VALOR_TOTAL ValorTotal
                                    FROM T_CTE_ITEM_NFSE IT
                                    JOIN T_NFSE_SERVICO S ON S.SER_CODIGO = IT.SER_CODIGO";

            if (!string.IsNullOrWhiteSpace(codigosNFSe))
                queryNFS += " where CON_CODIGO in (" + codigosNFSe + ")";
            else
                queryNFS += " where 1 = 0";

            query = "SELECT * FROM (" + query + " UNION ALL " + queryNFS + ") AS T";  

            var sqlDinamico = new SQLDinamico(query, parametros);

            var consultaNFItens = sqlDinamico.CriarSQLQuery(this.SessionNHiBernate);

            consultaNFItens.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.NFe.NotasEmitidasItens)));

            return consultaNFItens.List<Dominio.Relatorios.Embarcador.DataSource.NFe.NotasEmitidasItens>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.NFe.Estoque> RelatorioEstoqueProdutos(Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioEstoqueProdutos filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            string pattern = "yyyy-MM-dd";
            string sqlEstoque = " ISNULL(E.PRE_QUANTIDADE, 0) QuantidadeEstoque, ";
            if (filtrosPesquisa.DataPosicaoEstoque > DateTime.MinValue)
                sqlEstoque = @" ISNULL((SELECT SUM(CASE WHEN H.PEH_TIPO = 0 THEN H.PEH_QUANTIDADE ELSE (PEH_QUANTIDADE * -1) END) FROM T_PRODUTO_ESTOQUE_HISTORICO H 
                                WHERE H.PRO_CODIGO = P.PRO_CODIGO AND (H.PRE_CODIGO = E.PRE_CODIGO OR H.PRE_CODIGO IS NULL) AND CAST(H.PEH_DATA AS DATE) <= '" + filtrosPesquisa.DataPosicaoEstoque.ToString(pattern) + "' ), 0) QuantidadeEstoque, ";

            string sqlValorEstoque = @"CASE 
                            WHEN ISNULL(E.PRE_QUANTIDADE, 0) > 0 AND ISNULL(P.PRO_CUSTO_MEDIO, 0) > 0 THEN ISNULL(E.PRE_QUANTIDADE, 0) * ISNULL(P.PRO_CUSTO_MEDIO, 0) 
                            ELSE 0 
                        END ValorEstoque,";
            if (filtrosPesquisa.DataPosicaoEstoque > DateTime.MinValue)
            {
                sqlValorEstoque = @"CASE 
                            WHEN ISNULL(P.PRO_CUSTO_MEDIO, 0) > 0 THEN ISNULL((SELECT SUM(CASE WHEN H.PEH_TIPO = 0 THEN H.PEH_QUANTIDADE ELSE (PEH_QUANTIDADE * -1) END) FROM T_PRODUTO_ESTOQUE_HISTORICO H 
                            WHERE H.PRO_CODIGO = P.PRO_CODIGO AND (H.PRE_CODIGO = E.PRE_CODIGO OR H.PRE_CODIGO IS NULL) AND CAST(H.PEH_DATA AS DATE) <= '" + filtrosPesquisa.DataPosicaoEstoque.ToString(pattern) + @"' ), 0) * ISNULL(P.PRO_CUSTO_MEDIO, 0) 
                            ELSE 0
                        END ValorEstoque,";
            }

            string sqlCustoMedioEstoqueAtual = @"ISNULL((SELECT top(1) PHC_CUSTO_MEDIO_NOVO FROM T_PRODUTO_HISTORICO_CUSTO WHERE T_PRODUTO_HISTORICO_CUSTO.PRO_CODIGO = P.PRO_CODIGO ";
            if (filtrosPesquisa.DataPosicaoEstoque > DateTime.MinValue)
                sqlCustoMedioEstoqueAtual += " AND T_PRODUTO_HISTORICO_CUSTO.PHC_DATA_ATUALIZACAO <= '" + filtrosPesquisa.DataPosicaoEstoque.ToString(pattern) + @"'";
            sqlCustoMedioEstoqueAtual += " ORDER BY T_PRODUTO_HISTORICO_CUSTO.PHC_DATA_ATUALIZACAO DESC), 0) CustoMedioEstoqueAtual,";

            string query = @"SELECT P.PRO_CODIGO Codigo, 
                P.PRO_DESCRICAO Descricao, 
                P.PRO_COD_PRODUTO CodigoProduto, 
                P.PRO_COD_NCM CodigoNCM, 
                P.PRO_COD_CEST CodigoCEST, 
                CASE 
                    WHEN P.PRO_STATUS = 'A' THEN 'Ativo' 
                    WHEN P.PRO_STATUS = 'I' THEN 'Inativo' 
                    ELSE 'Indefinido' 
                END DescricaoStatus, 
                P.PRO_CATEGORIA_PRODUTO Categoria, 
                ISNULL(P.PRO_ULTIMO_CUSTO, 0) UltimoCusto, 
                ISNULL(P.PRO_CUSTO_MEDIO, 0) CustoMedio,
                " + sqlCustoMedioEstoqueAtual + @"
                ISNULL(P.PRO_VALOR_VENDA, 0) ValorVenda, 
                " + sqlEstoque + @"
                " + sqlValorEstoque + @"
                EE.EMP_RAZAO Empresa, 
                GrupoProduto.GRP_DESCRICAO GrupoProduto, 
                LocalArmazenamento.LAP_DESCRICAO LocalArmazenamento, 
                Marca.MAP_DESCRICAO Marca,
                P.PRO_PESO_BRUTO PesoBruto,
				P.PRO_PESO_LIQUIDO PesoLiquido,
				ISNULL((SELECT SUM(PEH_QUANTIDADE) FROM T_PRODUTO_ESTOQUE_HISTORICO H WHERE H.PRO_CODIGO = P.PRO_CODIGO AND (H.PRE_CODIGO = E.PRE_CODIGO OR H.PRE_CODIGO IS NULL) AND H.PEH_TIPO = 0), 0) Entradas,
				ISNULL((SELECT SUM(PEH_QUANTIDADE) FROM T_PRODUTO_ESTOQUE_HISTORICO H WHERE H.PRO_CODIGO = P.PRO_CODIGO AND (H.PRE_CODIGO = E.PRE_CODIGO OR H.PRE_CODIGO IS NULL) AND H.PEH_TIPO = 1), 0) Saidas,
                P.PRO_UNIDADE_MEDIDA UnidadeMedida,
                ((ISNULL((SELECT SUM(PEH_QUANTIDADE) FROM T_PRODUTO_ESTOQUE_HISTORICO H WHERE H.PRO_CODIGO = P.PRO_CODIGO AND (H.PRE_CODIGO = E.PRE_CODIGO OR H.PRE_CODIGO IS NULL) AND H.PEH_TIPO = 0), 0) 
				  - ISNULL((SELECT SUM(PEH_QUANTIDADE) FROM T_PRODUTO_ESTOQUE_HISTORICO H WHERE H.PRO_CODIGO = P.PRO_CODIGO AND (H.PRE_CODIGO = E.PRE_CODIGO OR H.PRE_CODIGO IS NULL) AND H.PEH_TIPO = 1), 0))
				  * (ISNULL((SELECT AVG(PEH_CUSTO) FROM T_PRODUTO_ESTOQUE_HISTORICO H WHERE PEH_CUSTO > 0 AND H.PRO_CODIGO = P.PRO_CODIGO AND (H.PRE_CODIGO = E.PRE_CODIGO OR H.PRE_CODIGO IS NULL)), 0))) ValorEstoqueAcumulado,
                CASE 
                    WHEN ISNULL(E.PRE_QUANTIDADE, 0) > 0 AND ISNULL(P.PRO_PESO_BRUTO, 0) > 0 THEN ISNULL(E.PRE_QUANTIDADE, 0) * ISNULL(P.PRO_PESO_BRUTO, 0) 
                    ELSE 0 
                END PesoBrutoAcumulado, 
                CASE 
                    WHEN ISNULL(E.PRE_QUANTIDADE, 0) > 0 AND ISNULL(P.PRO_PESO_LIQUIDO, 0) > 0 THEN ISNULL(E.PRE_QUANTIDADE, 0) * ISNULL(P.PRO_PESO_LIQUIDO, 0) 
                    ELSE 0 
                END PesoLiquidoAcumulado,
                LocalArmazenamentoEstoque.LAP_DESCRICAO LocalArmazenamentoEstoque,
                E.PRE_QUANTIDADE_ESTOQUE_RESERVADA EstoqueReservado,
                E.PRE_QUANTIDADE EstoqueAtual,
                (E.PRE_QUANTIDADE - E.PRE_QUANTIDADE_ESTOQUE_RESERVADA) EstoqueDisponivel
                FROM T_PRODUTO P 
                JOIN T_PRODUTO_ESTOQUE E ON E.PRO_CODIGO = P.PRO_CODIGO 
                LEFT OUTER JOIN T_EMPRESA EE ON EE.EMP_CODIGO = E.EMP_CODIGO 
                LEFT OUTER JOIN T_GRUPO_PRODUTO_TMS GrupoProduto ON GrupoProduto.GPR_CODIGO = P.GPR_CODIGO 
                LEFT OUTER JOIN T_LOCAL_ARMAZENAMENTO_PRODUTO LocalArmazenamento ON LocalArmazenamento.LAP_CODIGO = P.LAP_CODIGO 
                LEFT OUTER JOIN T_MARCA_PRODUTO Marca ON Marca.MAP_CODIGO = P.MAP_CODIGO 
                LEFT OUTER JOIN T_LOCAL_ARMAZENAMENTO_PRODUTO LocalArmazenamentoEstoque ON LocalArmazenamentoEstoque.LAP_CODIGO = E.LAP_CODIGO 
                WHERE 1 = 1 ";

            if (filtrosPesquisa.CodigoEmpresa > 0)
                query += " AND E.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresa.ToString();

            if (filtrosPesquisa.CodigoProduto > 0)
                query += " AND P.PRO_CODIGO = " + filtrosPesquisa.CodigoProduto.ToString();

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodProduto))
                query += " AND P.PRO_COD_PRODUTO LIKE '%" + filtrosPesquisa.CodProduto + "%'";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoNCM))
                query += " AND P.PRO_COD_NCM LIKE '" + filtrosPesquisa.CodigoNCM + "%'";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                query += " AND P.PRO_DESCRICAO LIKE '%" + filtrosPesquisa.Descricao + "%'";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Status) && filtrosPesquisa.Status != "T")
                query += " AND P.PRO_STATUS LIKE '" + filtrosPesquisa.Status + "'";

            if (filtrosPesquisa.Categoria >= 0)
                query += " AND P.PRO_CATEGORIA_PRODUTO = '" + (int)filtrosPesquisa.Categoria + "'";

            if (filtrosPesquisa.CodigoGrupoProduto > 0)
                query += " AND GrupoProduto.GPR_CODIGO = " + filtrosPesquisa.CodigoGrupoProduto;

            if (filtrosPesquisa.CodigoMarca > 0)
                query += " AND Marca.MAP_CODIGO = " + filtrosPesquisa.CodigoMarca;

            if (filtrosPesquisa.CodigoLocalArmazenamento > 0)
                query += " AND LocalArmazenamento.LAP_CODIGO = " + filtrosPesquisa.CodigoLocalArmazenamento;

            if (filtrosPesquisa.EstoqueReservado == true)
                query += @" AND E.PRE_QUANTIDADE_ESTOQUE_RESERVADA > 0";

            var agrup = false;
            if (!string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeAgrupar))
            {
                agrup = true;
                query += " order by " + parametrosConsulta.PropriedadeAgrupar + " " + parametrosConsulta.DirecaoAgrupar;
            }

            if (!string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeOrdenar) && parametrosConsulta.PropriedadeAgrupar != parametrosConsulta.PropriedadeOrdenar)
            {
                if (agrup)
                {
                    query += ", " + parametrosConsulta.PropriedadeOrdenar + " " + parametrosConsulta.DirecaoOrdenar;
                }
                else
                {
                    query += " order by " + parametrosConsulta.PropriedadeOrdenar + " " + parametrosConsulta.DirecaoOrdenar;
                }
            }

            if (parametrosConsulta.LimiteRegistros > 0)
                query += " OFFSET " + parametrosConsulta.InicioRegistros + " ROWS FETCH FIRST " + parametrosConsulta.LimiteRegistros + " ROWS ONLY";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.NFe.Estoque)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.NFe.Estoque>();
        }

        public int ContarRelatorioEstoqueProdutos(Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioEstoqueProdutos filtrosPesquisa)
        {
            string query = @"SELECT COUNT(0) as CONTADOR 
                FROM T_PRODUTO P 
                JOIN T_PRODUTO_ESTOQUE E ON E.PRO_CODIGO = P.PRO_CODIGO 
                LEFT OUTER JOIN T_EMPRESA EE ON EE.EMP_CODIGO = E.EMP_CODIGO 
                LEFT OUTER JOIN T_GRUPO_PRODUTO_TMS GrupoProduto ON GrupoProduto.GPR_CODIGO = P.GPR_CODIGO 
                LEFT OUTER JOIN T_LOCAL_ARMAZENAMENTO_PRODUTO LocalArmazenamento ON LocalArmazenamento.LAP_CODIGO = P.LAP_CODIGO 
                LEFT OUTER JOIN T_MARCA_PRODUTO Marca ON Marca.MAP_CODIGO = P.MAP_CODIGO 
                LEFT OUTER JOIN T_LOCAL_ARMAZENAMENTO_PRODUTO LocalArmazenamentoEstoque ON LocalArmazenamentoEstoque.LAP_CODIGO = E.LAP_CODIGO 
                WHERE 1 = 1 ";

            if (filtrosPesquisa.CodigoEmpresa > 0)
                query += " AND E.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresa.ToString();

            if (filtrosPesquisa.CodigoProduto > 0)
                query += " AND P.PRO_CODIGO = " + filtrosPesquisa.CodigoProduto.ToString();

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodProduto))
                query += " AND P.PRO_COD_PRODUTO LIKE '%" + filtrosPesquisa.CodProduto + "%'";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoNCM))
                query += " AND P.PRO_COD_NCM LIKE '" + filtrosPesquisa.CodigoNCM + "%'";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                query += " AND P.PRO_DESCRICAO LIKE '%" + filtrosPesquisa.Descricao + "%'";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Status) && filtrosPesquisa.Status != "T")
                query += " AND P.PRO_STATUS LIKE '" + filtrosPesquisa.Status + "'";

            if (filtrosPesquisa.Categoria >= 0)
                query += " AND P.PRO_CATEGORIA_PRODUTO = '" + (int)filtrosPesquisa.Categoria + "'";

            if (filtrosPesquisa.CodigoGrupoProduto > 0)
                query += " AND GrupoProduto.GPR_CODIGO = " + filtrosPesquisa.CodigoGrupoProduto;

            if (filtrosPesquisa.CodigoMarca > 0)
                query += " AND Marca.MAP_CODIGO = " + filtrosPesquisa.CodigoMarca;

            if (filtrosPesquisa.CodigoLocalArmazenamento > 0)
                query += " AND LocalArmazenamento.LAP_CODIGO = " + filtrosPesquisa.CodigoLocalArmazenamento;

            if (filtrosPesquisa.EstoqueReservado == true)
                query += @" AND E.PRE_QUANTIDADE_ESTOQUE_RESERVADA > 0";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.NFe.HistoricoEstoque> RelatorioHistoricoEstoque(Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioHistoricoEstoque filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            string query = @"SELECT P.PRO_CODIGO Codigo, 
                 P.PRO_DESCRICAO Descricao, 
                 P.PRO_COD_PRODUTO CodigoProduto, 
                 CASE 
                     WHEN P.PRO_STATUS = 'A' THEN 'Ativo' 
                     WHEN P.PRO_STATUS = 'I' THEN 'Inativo' 
                     ELSE 'Indefinido' 
                 END DescricaoStatus, 
                 P.PRO_CATEGORIA_PRODUTO Categoria, 
                 H.PEH_DATA Data, 
                 CASE 
                     WHEN H.PEH_TIPO = 0 THEN 'Entrada' 
                     WHEN H.PEH_TIPO = 1 THEN 'Saída' 
                     ELSE 'Indefinido'	 
                 END TipoMovimento, 
                 CASE
					WHEN H.PEH_TIPO = 1 THEN ISNULL(H.PEH_QUANTIDADE, 0) * -1
					ELSE ISNULL(H.PEH_QUANTIDADE, 0)
				 END Quantidade, 
                 H.PEH_DOCUMENTO Documento, 
                 H.PEH_TIPO_DOCUMENTO TipoDocumento, 
                 ISNULL(Estoque.PRE_QUANTIDADE, ISNULL((SELECT TOP 1 E.PRE_QUANTIDADE FROM T_PRODUTO_ESTOQUE E
                                                        WHERE E.PRO_CODIGO = P.PRO_CODIGO AND (E.EMP_CODIGO = H.EMP_CODIGO OR E.EMP_CODIGO IS NULL) AND (E.LAP_CODIGO = H.LAP_CODIGO OR E.LAP_CODIGO IS NULL)
                                                        ORDER BY E.PRE_CODIGO ASC), 0)) QuantidadeEstoque,
                 PE.PRE_CUSTO_MEDIO CustoMedio,
                 CASE
					WHEN H.PEH_TIPO = 1 AND H.PEH_TIPO_DOCUMENTO <> 'ENT' THEN ISNULL((SELECT TOP(1) HH.PEH_CUSTO FROM T_PRODUTO_ESTOQUE_HISTORICO HH WHERE HH.PRO_CODIGO = H.PRO_CODIGO AND HH.PEH_TIPO = 0 AND HH.PEH_DATA < H.PEH_DATA AND PEH_TIPO_DOCUMENTO = 'ENT' ORDER BY PEH_CODIGO DESC), ISNULL(P.PRO_CUSTO_MEDIO, 0))
					ELSE 
						CASE 
							WHEN PEH_TIPO_DOCUMENTO = 'ENT' THEN ISNULL(H.PEH_CUSTO, 0)
							ELSE ISNULL((SELECT TOP(1) HH.PEH_CUSTO FROM T_PRODUTO_ESTOQUE_HISTORICO HH WHERE HH.PRO_CODIGO = H.PRO_CODIGO AND HH.PEH_TIPO = 0 AND HH.PEH_DATA < H.PEH_DATA AND PEH_TIPO_DOCUMENTO = 'ENT' ORDER BY PEH_CODIGO DESC), ISNULL(P.PRO_CUSTO_MEDIO, 0))
						END
				 END Custo, 
                 Empresa.EMP_RAZAO Empresa,
                 GrupoProduto.GRP_DESCRICAO GrupoProduto,
                 LocalArmazenamento.LAP_DESCRICAO LocalArmazenamento

                 FROM T_PRODUTO P 
                 JOIN T_PRODUTO_ESTOQUE_HISTORICO H ON H.PRO_CODIGO = P.PRO_CODIGO
                 JOIN T_PRODUTO_ESTOQUE PE ON PE.PRE_CODIGO = H.PRE_CODIGO
                 LEFT OUTER JOIN T_EMPRESA Empresa ON Empresa.EMP_CODIGO = H.EMP_CODIGO
                 LEFT OUTER JOIN T_PRODUTO_ESTOQUE Estoque ON Estoque.PRE_CODIGO = H.PRE_CODIGO
                 LEFT OUTER JOIN T_GRUPO_PRODUTO_TMS GrupoProduto ON GrupoProduto.GPR_CODIGO = P.GPR_CODIGO
                 LEFT OUTER JOIN T_LOCAL_ARMAZENAMENTO_PRODUTO LocalArmazenamento ON LocalArmazenamento.LAP_CODIGO = H.LAP_CODIGO
                 WHERE 1 = 1 ";

            if (filtrosPesquisa.CodigoEmpresa > 0)
                query += $" AND Empresa.EMP_CODIGO = { filtrosPesquisa.CodigoEmpresa } ";

            if (filtrosPesquisa.CodigoProduto > 0)
                query += " AND P.PRO_CODIGO = " + filtrosPesquisa.CodigoProduto.ToString();

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Status) && filtrosPesquisa.Status != "T")
                query += " AND P.PRO_STATUS LIKE '" + filtrosPesquisa.Status + "'";

            string pattern = "yyyy-MM-dd";
            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                query += " AND CAST(H.PEH_DATA AS DATE) >= '" + filtrosPesquisa.DataInicial.ToString(pattern) + "'";

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                query += " AND CAST(H.PEH_DATA AS DATE) <= '" + filtrosPesquisa.DataFinal.ToString(pattern) + "'";

            if (filtrosPesquisa.Categoria >= 0)
                query += " AND P.PRO_CATEGORIA_PRODUTO = '" + (int)filtrosPesquisa.Categoria + "'";

            if (filtrosPesquisa.CodigoGrupoProduto > 0)
                query += " AND GrupoProduto.GPR_CODIGO = " + filtrosPesquisa.CodigoGrupoProduto.ToString();

            if (filtrosPesquisa.CodigoLocalArmazenamento > 0)
                query += " AND LocalArmazenamento.LAP_CODIGO = " + filtrosPesquisa.CodigoLocalArmazenamento;

            string queryOrdenacao = "";
            var agrup = false;
            if (!string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeAgrupar))
            {
                agrup = true;
                queryOrdenacao += " order by " + parametrosConsulta.PropriedadeAgrupar + " " + parametrosConsulta.DirecaoAgrupar;
            }

            if (!string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeOrdenar) && parametrosConsulta.PropriedadeAgrupar != parametrosConsulta.PropriedadeOrdenar)
            {
                if (agrup)
                {
                    queryOrdenacao += ", " + parametrosConsulta.PropriedadeOrdenar + " " + parametrosConsulta.DirecaoOrdenar;
                }
                else
                {
                    queryOrdenacao += " order by " + parametrosConsulta.PropriedadeOrdenar + " " + parametrosConsulta.DirecaoOrdenar;
                }
            }

            query = @"SELECT T.Codigo, T.Descricao, T.CodigoProduto, T.DescricaoStatus, T.Categoria, T.Data, T.TipoMovimento, T.Quantidade, T.Documento, T.TipoDocumento, T.QuantidadeEstoque, 
                    T.CustoMedio, T.Custo, T.Empresa, T.GrupoProduto, T.LocalArmazenamento, 
                    SUM(T.Quantidade) OVER (PARTITION BY t.Empresa " + queryOrdenacao + @" ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW) AS QuantidadeAcumulada
                    FROM ( " + query + " ) AS T ";

            query += queryOrdenacao;

            if (parametrosConsulta.LimiteRegistros > 0)
                query += " OFFSET " + parametrosConsulta.InicioRegistros + " ROWS FETCH FIRST " + parametrosConsulta.LimiteRegistros + " ROWS ONLY";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.NFe.HistoricoEstoque)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.NFe.HistoricoEstoque>();
        }

        public int ContarRelatorioHistoricoEstoque(Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioHistoricoEstoque filtrosPesquisa)
        {
            string query = @"SELECT COUNT(0) as CONTADOR 
                 FROM T_PRODUTO P 
                 JOIN T_PRODUTO_ESTOQUE_HISTORICO H ON H.PRO_CODIGO = P.PRO_CODIGO
                 JOIN T_PRODUTO_ESTOQUE PE ON PE.PRE_CODIGO = H.PRE_CODIGO
                 LEFT OUTER JOIN T_EMPRESA Empresa ON Empresa.EMP_CODIGO = H.EMP_CODIGO
                 LEFT OUTER JOIN T_PRODUTO_ESTOQUE Estoque ON Estoque.PRE_CODIGO = H.PRE_CODIGO
                 LEFT OUTER JOIN T_GRUPO_PRODUTO_TMS GrupoProduto ON GrupoProduto.GPR_CODIGO = P.GPR_CODIGO
                 LEFT OUTER JOIN T_LOCAL_ARMAZENAMENTO_PRODUTO LocalArmazenamento ON LocalArmazenamento.LAP_CODIGO = H.LAP_CODIGO
                 WHERE 1 = 1 ";

            if (filtrosPesquisa.CodigoEmpresa > 0)
                query += $" AND Empresa.EMP_CODIGO = {filtrosPesquisa.CodigoEmpresa} ";

            if (filtrosPesquisa.CodigoProduto > 0)
                query += " AND P.PRO_CODIGO = " + filtrosPesquisa.CodigoProduto.ToString();

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Status) && filtrosPesquisa.Status != "T")
                query += " AND P.PRO_STATUS LIKE '" + filtrosPesquisa.Status + "'";

            string pattern = "yyyy-MM-dd";
            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                query += " AND CAST(H.PEH_DATA AS DATE) >= '" + filtrosPesquisa.DataInicial.ToString(pattern) + "'";

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                query += " AND CAST(H.PEH_DATA AS DATE) <= '" + filtrosPesquisa.DataFinal.ToString(pattern) + "'";

            if (filtrosPesquisa.Categoria >= 0)
                query += " AND P.PRO_CATEGORIA_PRODUTO = '" + (int)filtrosPesquisa.Categoria + "'";

            if (filtrosPesquisa.CodigoGrupoProduto > 0)
                query += " AND GrupoProduto.GPR_CODIGO = " + filtrosPesquisa.CodigoGrupoProduto.ToString();

            if (filtrosPesquisa.CodigoLocalArmazenamento > 0)
                query += " AND LocalArmazenamento.LAP_CODIGO = " + filtrosPesquisa.CodigoLocalArmazenamento;

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.NFe.NotasEmitidasAdministrativo> RelatorioNotasEmitidasAdministrativo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa statusEmpresa, int codigoEmpresa, int numeroInicial, int numeroFinal, int serie, int empresa, DateTime dataInicial, DateTime dataFinal, DateTime dataProcessamento, DateTime dataSaida, Dominio.Enumeradores.StatusNFe status, Dominio.Enumeradores.TipoEmissaoNFe tipoEmissao, int formaEmissao, DateTime dataInicialCadastro, DateTime dataFinalCadastro, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, bool exibirSomenteClientesComEmissao, string propGrupo, string dirOrdenacaoGrupo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, bool paginar = true, bool todosCNPJdaRaizEmbarcador = false)
        {
            string querySub = "", querySubNFSe = "", querySubTitulo = "", queryDocumentosDestinados = "", queryDocumentosEntrada = "", queryMDFe = "";
            if (numeroInicial > 0 && numeroFinal == 0)
            {
                querySub += " AND N.NFI_NUMERO = " + numeroInicial.ToString();
                querySubNFSe += " AND C.CON_NUM = " + numeroInicial.ToString();
            }
            else if (numeroInicial > 0 && numeroFinal > 0)
            {
                querySub += " AND N.NFI_NUMERO >= " + numeroInicial.ToString() + " AND N.NFI_NUMERO <= " + numeroFinal.ToString();
                querySubNFSe += " AND C.CON_NUM >= " + numeroInicial.ToString() + " AND C.CON_NUM <= " + numeroFinal.ToString();
            }
            else if (numeroInicial == 0 && numeroFinal > 0)
            {
                querySub += " AND N.NFI_NUMERO = " + numeroFinal.ToString();
                querySubNFSe += " AND C.CON_NUM = " + numeroFinal.ToString();
            }

            if (empresa > 0)
            {
                querySub += " AND N.EMP_CODIGO = " + empresa.ToString();
                querySubNFSe += " AND C.EMP_CODIGO = " + empresa.ToString();
                queryDocumentosDestinados += " AND DD.EMP_CODIGO = " + empresa.ToString();
                queryDocumentosEntrada += " AND TDE.EMP_CODIGO = " + empresa.ToString();
                queryMDFe += " AND MDFe.EMP_CODIGO = " + empresa.ToString();
            }

            if (formaEmissao == 1)
                querySub += " AND N.NFI_ULTIMO_STATUS_SEFAZ IS NOT NULL ";

            if (serie > 0)
            {
                querySub += " AND S.ESE_NUMERO = " + serie.ToString();
                querySubNFSe += " AND S.ESE_NUMERO = " + serie.ToString();
            }

            if (dataInicial != DateTime.MinValue)
            {
                querySub += " AND N.NFI_DATA_EMISSAO >= '" + dataInicial.ToString("MM/dd/yyyy") + "'";
                querySubNFSe += " AND C.CON_DATAHORAEMISSAO >= '" + dataInicial.ToString("MM/dd/yyyy") + "'";
                querySubTitulo += " AND T.TIT_DATA_EMISSAO >= '" + dataInicial.ToString("MM/dd/yyyy") + "'";
                queryDocumentosDestinados += " AND DD.DDE_DATA_EMISSAO >= '" + dataInicial.ToString("MM/dd/yyyy") + "'";
                queryDocumentosEntrada += " AND TDE.TDE_DATA_EMISSAO >= '" + dataInicial.ToString("MM/dd/yyyy") + "'";
                queryMDFe += " AND MDFe.MDF_DATA_EMISSAO >= '" + dataInicial.ToString("MM/dd/yyyy") + "'";
            }

            if (dataFinal != DateTime.MinValue)
            {
                querySub += " AND N.NFI_DATA_EMISSAO <= '" + dataFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";
                querySubNFSe += " AND C.CON_DATAHORAEMISSAO <= '" + dataFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";
                querySubTitulo += " AND T.TIT_DATA_EMISSAO <= '" + dataFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";
                queryDocumentosDestinados += " AND DD.DDE_DATA_EMISSAO <= '" + dataFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";
                queryDocumentosEntrada += " AND TDE.TDE_DATA_EMISSAO <= '" + dataFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";
                queryMDFe += " AND MDFe.MDF_DATA_EMISSAO <= '" + dataFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";
            }

            if (dataProcessamento != DateTime.MinValue)
            {
                querySub += " AND N.NFI_DATA_PROCESSAMENTO > '" + dataProcessamento.ToString("MM/dd/yyyy") + "' AND N.NFI_DATA_PROCESSAMENTO < '" + dataProcessamento.AddDays(1).ToString("MM/dd/yyyy") + "' ";
                querySubNFSe += " AND C.CON_DATA_AUTORIZACAO > '" + dataProcessamento.ToString("MM/dd/yyyy") + "' AND C.CON_DATA_AUTORIZACAO < '" + dataProcessamento.AddDays(1).ToString("MM/dd/yyyy") + "' ";
                querySubTitulo += " AND T.TIT_DATA_EMISSAO > '" + dataProcessamento.ToString("MM/dd/yyyy") + "' AND T.TIT_DATA_EMISSAO < '" + dataProcessamento.AddDays(1).ToString("MM/dd/yyyy") + "' ";
                queryDocumentosDestinados += " AND DD.DDE_DATA_EMISSAO > '" + dataProcessamento.ToString("MM/dd/yyyy") + "' AND DD.DDE_DATA_EMISSAO < '" + dataProcessamento.AddDays(1).ToString("MM/dd/yyyy") + "' ";
                queryDocumentosEntrada += " AND TDE.TDE_DATA_EMISSAO > '" + dataProcessamento.ToString("MM/dd/yyyy") + "' AND TDE.TDE_DATA_EMISSAO < '" + dataProcessamento.AddDays(1).ToString("MM/dd/yyyy") + "' ";
                queryMDFe += " AND MDFe.MDF_DATA_EMISSA > '" + dataProcessamento.ToString("MM/dd/yyyy") + "' AND MDFe.MDF_DATA_EMISSA < '" + dataProcessamento.AddDays(1).ToString("MM/dd/yyyy") + "' ";
            }

            if (dataSaida != DateTime.MinValue)
            {
                querySub += " AND N.NFI_DATA_SAIDA > '" + dataSaida.ToString("MM/dd/yyyy") + "' AND N.NFI_DATA_SAIDA < '" + dataSaida.AddDays(1).ToString("MM/dd/yyyy") + "' ";
                querySubNFSe += " AND C.CON_DATA_AUTORIZACAO > '" + dataSaida.ToString("MM/dd/yyyy") + "' AND C.CON_DATA_AUTORIZACAO < '" + dataSaida.AddDays(1).ToString("MM/dd/yyyy") + "' ";
                querySubTitulo += " AND T.TIT_DATA_EMISSAO > '" + dataSaida.ToString("MM/dd/yyyy") + "' AND T.TIT_DATA_EMISSAO < '" + dataSaida.AddDays(1).ToString("MM/dd/yyyy") + "' ";
                queryDocumentosDestinados += " AND DD.DDE_DATA_EMISSAO > '" + dataSaida.ToString("MM/dd/yyyy") + "' AND DD.DDE_DATA_EMISSAO < '" + dataSaida.AddDays(1).ToString("MM/dd/yyyy") + "' ";
                queryDocumentosEntrada += " AND TDE.TDE_DATA_EMISSAO > '" + dataSaida.ToString("MM/dd/yyyy") + "' AND TDE.TDE_DATA_EMISSAO < '" + dataSaida.AddDays(1).ToString("MM/dd/yyyy") + "' ";
                queryMDFe += " AND MDFe.MDF_DATA_EMISSA > '" + dataSaida.ToString("MM/dd/yyyy") + "' AND MDFe.MDF_DATA_EMISSA < '" + dataSaida.AddDays(1).ToString("MM/dd/yyyy") + "' ";
            }

            if (status > 0)
                querySub += " AND N.NFI_STATUS = '" + (int)status + "'";

            if (tipoEmissao >= 0)
                querySub += " AND N.NFI_TIPO_EMISSAO = '" + (int)tipoEmissao + "'";

            var ambiente = (int)Dominio.Enumeradores.TipoAmbiente.Producao;
            querySub += " AND N.NFI_AMBIENTE = " + ambiente;
            querySubNFSe += " AND C.CON_TIPO_AMBIENTE = " + ambiente;
            querySubTitulo += " AND T.TIT_AMBIENTE = " + ambiente;

            string query = @"SELECT T.CodigoEmpresaPai, T.CNPJEmpresaPai, T.NomeEmpresaPai, T.DataCadastro, T.CodigoEmpresa, T.NomeEmpresa, T.CNPJEmpresa, 
                 T.QtdInutilizadas, T.QtdCanceladas, T.QtdEmitidas, T.QtdProcessadas, T.QtdDenegadas, T.QtdRejeitadas, T.QtdAutorizadas,  T.QtdCCe, T.QtdNFSe, 
                 T.QtdBoletos, T.QtdNFDestinada, T.QtdDocumentoEntrada, T.TotalDocumentos, T.QtdMDFe,

                 ISNULL((SELECT TOP 1 PNV_VALOR FROM T_PLANO_EMISSAO_NFE_VALOR PEDV
                    JOIN T_PLANO_EMISSAO_NFE PED ON PED.PEN_CODIGO = PEDV.PEN_CODIGO
                    WHERE PED.EMP_CODIGO = T.CodigoEmpresaPai AND T.TotalDocumentos BETWEEN PNV_QTD_INICIAL AND PNV_QTD_FINAL AND T.TotalDocumentos > 0), 0.00) ValorPlano

                 FROM ( 
                 SELECT EP.EMP_CODIGO CodigoEmpresaPai, 
                 EP.EMP_CNPJ CNPJEmpresaPai, 
                 EP.EMP_RAZAO NomeEmpresaPai, 
                 E.EMP_DATACADASTRO DataCadastro, 
                 E.EMP_CODIGO CodigoEmpresa, 
                 E.EMP_RAZAO NomeEmpresa, 
                 E.EMP_CNPJ CNPJEmpresa, 
                 ISNULL((SELECT COUNT(1) FROM T_NOTA_FISCAL N JOIN T_EMPRESA_SERIE S ON S.ESE_CODIGO = N.ESE_CODIGO 
                 WHERE N.EMP_CODIGO = E.EMP_CODIGO AND N.NFI_STATUS = 2 " + querySub + @"), 0) QtdInutilizadas, 
                 ISNULL((SELECT COUNT(1) FROM T_NOTA_FISCAL N JOIN T_EMPRESA_SERIE S ON S.ESE_CODIGO = N.ESE_CODIGO 
                 WHERE N.EMP_CODIGO = E.EMP_CODIGO AND N.NFI_STATUS = 3 " + querySub + @"), 0) QtdCanceladas, 
                 ISNULL((SELECT COUNT(1) FROM T_NOTA_FISCAL N JOIN T_EMPRESA_SERIE S ON S.ESE_CODIGO = N.ESE_CODIGO 
                 WHERE N.EMP_CODIGO = E.EMP_CODIGO AND N.NFI_STATUS = 1 " + querySub + @"), 0) QtdEmitidas, 
                 ISNULL((SELECT COUNT(1) FROM T_NOTA_FISCAL N JOIN T_EMPRESA_SERIE S ON S.ESE_CODIGO = N.ESE_CODIGO 
                 WHERE N.EMP_CODIGO = E.EMP_CODIGO AND N.NFI_STATUS = 7 " + querySub + @"), 0) QtdProcessadas, 
                 ISNULL((SELECT COUNT(1) FROM T_NOTA_FISCAL N JOIN T_EMPRESA_SERIE S ON S.ESE_CODIGO = N.ESE_CODIGO 
                 WHERE N.EMP_CODIGO = E.EMP_CODIGO AND N.NFI_STATUS = 5 " + querySub + @"), 0) QtdDenegadas, 
                 ISNULL((SELECT COUNT(1) FROM T_NOTA_FISCAL N JOIN T_EMPRESA_SERIE S ON S.ESE_CODIGO = N.ESE_CODIGO 
                 WHERE N.EMP_CODIGO = E.EMP_CODIGO AND N.NFI_STATUS = 6 " + querySub + @"), 0) QtdRejeitadas, 
                 ISNULL((SELECT COUNT(1) FROM T_NOTA_FISCAL N JOIN T_EMPRESA_SERIE S ON S.ESE_CODIGO = N.ESE_CODIGO 
                 WHERE N.EMP_CODIGO = E.EMP_CODIGO AND N.NFI_STATUS = 4 " + querySub + @"), 0) QtdAutorizadas, 
                 ISNULL((select COUNT(1) from T_NOTA_FISCAL_CARTA_CORRECAO C 
                 JOIN T_NOTA_FISCAL N ON N.NFI_CODIGO = C.NFI_CODIGO AND N.EMP_CODIGO = E.EMP_CODIGO  
                 JOIN T_EMPRESA_SERIE S ON S.ESE_CODIGO = N.ESE_CODIGO  
                 WHERE C.NCC_STATUS = 4 " + querySub + @"), 0) QtdCCe, 
                 ISNULL((SELECT COUNT(1) FROM T_CTE C JOIN T_EMPRESA_SERIE S ON S.ESE_CODIGO = C.CON_SERIE 
                 WHERE CON_MODELODOC = ISNULL((SELECT MOD_CODIGO FROM T_MODDOCFISCAL WHERE MOD_NUM = '38'), 31) 
                 AND C.EMP_CODIGO = E.EMP_CODIGO AND C.CON_PROTOCOLO IS NOT NULL " + querySubNFSe + @"), 0) QtdNFSe, 
                 ISNULL((SELECT COUNT(1) FROM T_TITULO T 
                 WHERE T.EMP_CODIGO = E.EMP_CODIGO AND T.TIT_TIPO = 1 AND T.TIT_NOSSO_NUMERO IS NOT NULL " + querySubTitulo + @"), 0) QtdBoletos,
                 ISNULL((SELECT COUNT(DISTINCT DDE_CHAVE) FROM T_DOCUMENTO_DESTINADO_EMPRESA DD WHERE DD.EMP_CODIGO = E.EMP_CODIGO " + queryDocumentosDestinados + @"), 0) QtdNFDestinada,
                 ISNULL((SELECT COUNT(DISTINCT TDE.TDE_CODIGO) FROM T_TMS_DOCUMENTO_ENTRADA TDE WHERE TDE.EMP_CODIGO = E.EMP_CODIGO " + queryDocumentosEntrada + @"), 0) QtdDocumentoEntrada,
                 ISNULL((SELECT COUNT(DISTINCT MDFe.MDF_CODIGO) FROM T_MDFE MDFe WHERE MDFe.EMP_CODIGO = E.EMP_CODIGO " + queryMDFe + @"), 0) QtdMDFe,

                 (ISNULL((SELECT COUNT(1) FROM T_NOTA_FISCAL N JOIN T_EMPRESA_SERIE S ON S.ESE_CODIGO = N.ESE_CODIGO 
                 WHERE N.EMP_CODIGO = E.EMP_CODIGO AND N.NFI_STATUS = 4 " + querySub + @"), 0) + 
                 ISNULL((SELECT COUNT(1) FROM T_CTE C JOIN T_EMPRESA_SERIE S ON S.ESE_CODIGO = C.CON_SERIE 
                 WHERE CON_MODELODOC = ISNULL((SELECT MOD_CODIGO FROM T_MODDOCFISCAL WHERE MOD_NUM = '38'), 31) 
                 AND C.EMP_CODIGO = E.EMP_CODIGO AND C.CON_PROTOCOLO IS NOT NULL " + querySubNFSe + @"), 0) + 
                 ISNULL((SELECT COUNT(DISTINCT TDE.TDE_CODIGO) FROM T_TMS_DOCUMENTO_ENTRADA TDE
                 WHERE TDE.EMP_CODIGO = E.EMP_CODIGO " + queryDocumentosEntrada + @"), 0) +
                 ISNULL((SELECT COUNT(DISTINCT MDFe.MDF_CODIGO) FROM T_MDFE MDFe
                 WHERE MDFe.EMP_CODIGO = E.EMP_CODIGO " + queryMDFe + @"), 0) +
                 ISNULL((SELECT COUNT(1) FROM T_TITULO T 
                 WHERE T.EMP_CODIGO = E.EMP_CODIGO AND T.TIT_TIPO = 1 AND T.TIT_NOSSO_NUMERO IS NOT NULL " + querySubTitulo + @"), 0)) TotalDocumentos

                 FROM T_EMPRESA E 
                 JOIN T_EMPRESA EP ON EP.EMP_CODIGO = E.EMP_EMPRESA 
                 WHERE 1 = 1 ";

            if (codigoEmpresa > 0 && codigoEmpresa != 137)
                query += " AND EP.EMP_CODIGO = " + codigoEmpresa.ToString();
            if (empresa > 0)
                query += " AND E.EMP_CODIGO = " + empresa.ToString();

            if (statusEmpresa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                query += " AND E.EMP_STATUS = 'A'";
            else if (statusEmpresa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                query += " AND E.EMP_STATUS = 'I'";

            if (dataInicialCadastro != DateTime.MinValue)
                query += " AND E.EMP_DATACADASTRO >= '" + dataInicialCadastro.ToString("MM/dd/yyyy") + "'";

            if (dataFinalCadastro != DateTime.MinValue)
                query += " AND E.EMP_DATACADASTRO <= '" + dataFinalCadastro.AddDays(1).ToString("MM/dd/yyyy") + "'";

            query += ") AS T WHERE 1 = 1 ";
            if (exibirSomenteClientesComEmissao)
                query += " AND T.TotalDocumentos > 0 ";
            else
                query += " AND T.QtdInutilizadas > 0 OR T.QtdCanceladas > 0 OR T.QtdEmitidas > 0 OR T.QtdProcessadas > 0 " +
                     " OR T.QtdDenegadas > 0 OR T.QtdRejeitadas > 0 OR T.QtdAutorizadas > 0 OR T.QtdCCe > 0 OR T.QtdNFSe > 0 OR T.QtdBoletos > 0 OR T.QtdNFDestinada > 0 ";

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

            if (paginar)
                query += " OFFSET " + inicioRegistros + " ROWS FETCH FIRST " + maximoRegistros + " ROWS ONLY";


            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.NFe.NotasEmitidasAdministrativo)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.NFe.NotasEmitidasAdministrativo>();
        }

        public int ContarRelatorioNotasEmitidasAdministrativo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa statusEmpresa, int codigoEmpresa, int numeroInicial, int numeroFinal, int serie, int empresa, DateTime dataInicial, DateTime dataFinal, DateTime dataProcessamento, DateTime dataSaida, Dominio.Enumeradores.StatusNFe status, Dominio.Enumeradores.TipoEmissaoNFe tipoEmissao, int formaEmissao, DateTime dataInicialCadastro, DateTime dataFinalCadastro, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, bool exibirSomenteClientesComEmissao)
        {
            string querySub = "", querySubNFSe = "", querySubTitulo = "", queryDocumentosDestinados = "", queryDocumentosEntrada = "";
            if (numeroInicial > 0 && numeroFinal == 0)
            {
                querySub += " AND N.NFI_NUMERO = " + numeroInicial.ToString();
                querySubNFSe += " AND C.CON_NUM = " + numeroInicial.ToString();
            }
            else if (numeroInicial > 0 && numeroFinal > 0)
            {
                querySub += " AND N.NFI_NUMERO >= " + numeroInicial.ToString() + " AND N.NFI_NUMERO <= " + numeroFinal.ToString();
                querySubNFSe += " AND C.CON_NUM >= " + numeroInicial.ToString() + " AND C.CON_NUM <= " + numeroFinal.ToString();
            }
            else if (numeroInicial == 0 && numeroFinal > 0)
            {
                querySub += " AND N.NFI_NUMERO = " + numeroFinal.ToString();
                querySubNFSe += " AND C.CON_NUM = " + numeroFinal.ToString();
            }

            if (empresa > 0)
            {
                querySub += " AND N.EMP_CODIGO = " + empresa.ToString();
                querySubNFSe += " AND C.EMP_CODIGO = " + empresa.ToString();
                queryDocumentosDestinados += " AND DD.EMP_CODIGO = " + empresa.ToString();
                queryDocumentosEntrada += " AND TDE.EMP_CODIGO = " + empresa.ToString();
            }

            if (formaEmissao == 1)
                querySub += " AND N.NFI_ULTIMO_STATUS_SEFAZ IS NOT NULL ";

            if (serie > 0)
            {
                querySub += " AND S.ESE_NUMERO = " + serie.ToString();
                querySubNFSe += " AND S.ESE_NUMERO = " + serie.ToString();
            }

            if (dataInicial != DateTime.MinValue)
            {
                querySub += " AND N.NFI_DATA_EMISSAO >= '" + dataInicial.ToString("MM/dd/yyyy") + "'";
                querySubNFSe += " AND C.CON_DATAHORAEMISSAO >= '" + dataInicial.ToString("MM/dd/yyyy") + "'";
                querySubTitulo += " AND T.TIT_DATA_EMISSAO >= '" + dataInicial.ToString("MM/dd/yyyy") + "'";
                queryDocumentosDestinados += " AND DD.DDE_DATA_EMISSAO >= '" + dataInicial.ToString("MM/dd/yyyy") + "'";
                queryDocumentosEntrada += " AND TDE.TDE_DATA_EMISSAO >= '" + dataInicial.ToString("MM/dd/yyyy") + "'";
            }

            if (dataFinal != DateTime.MinValue)
            {
                querySub += " AND N.NFI_DATA_EMISSAO <= '" + dataFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";
                querySubNFSe += " AND C.CON_DATAHORAEMISSAO <= '" + dataFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";
                querySubTitulo += " AND T.TIT_DATA_EMISSAO <= '" + dataFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";
                queryDocumentosDestinados += " AND DD.DDE_DATA_EMISSAO <= '" + dataFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";
                queryDocumentosEntrada += " AND TDE.TDE_DATA_EMISSAO <= '" + dataFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";
            }

            if (dataProcessamento != DateTime.MinValue)
            {
                querySub += " AND N.NFI_DATA_PROCESSAMENTO > '" + dataProcessamento.ToString("MM/dd/yyyy") + "' AND N.NFI_DATA_PROCESSAMENTO < '" + dataProcessamento.AddDays(1).ToString("MM/dd/yyyy") + "' ";
                querySubNFSe += " AND C.CON_DATA_AUTORIZACAO > '" + dataProcessamento.ToString("MM/dd/yyyy") + "' AND C.CON_DATA_AUTORIZACAO < '" + dataProcessamento.AddDays(1).ToString("MM/dd/yyyy") + "' ";
                querySubTitulo += " AND T.TIT_DATA_EMISSAO > '" + dataProcessamento.ToString("MM/dd/yyyy") + "' AND T.TIT_DATA_EMISSAO < '" + dataProcessamento.AddDays(1).ToString("MM/dd/yyyy") + "' ";
                queryDocumentosDestinados += " AND DD.DDE_DATA_EMISSAO > '" + dataProcessamento.ToString("MM/dd/yyyy") + "' AND DD.DDE_DATA_EMISSAO < '" + dataProcessamento.AddDays(1).ToString("MM/dd/yyyy") + "' ";
                queryDocumentosEntrada += " AND TDE.TDE_DATA_EMISSAO > '" + dataProcessamento.ToString("MM/dd/yyyy") + "' AND TDE.TDE_DATA_EMISSAO < '" + dataProcessamento.AddDays(1).ToString("MM/dd/yyyy") + "' ";
            }

            if (dataSaida != DateTime.MinValue)
            {
                querySub += " AND N.NFI_DATA_SAIDA > '" + dataSaida.ToString("MM/dd/yyyy") + "' AND N.NFI_DATA_SAIDA < '" + dataSaida.AddDays(1).ToString("MM/dd/yyyy") + "' ";
                querySubNFSe += " AND C.CON_DATA_AUTORIZACAO > '" + dataSaida.ToString("MM/dd/yyyy") + "' AND C.CON_DATA_AUTORIZACAO < '" + dataSaida.AddDays(1).ToString("MM/dd/yyyy") + "' ";
                querySubTitulo += " AND T.TIT_DATA_EMISSAO > '" + dataSaida.ToString("MM/dd/yyyy") + "' AND T.TIT_DATA_EMISSAO < '" + dataSaida.AddDays(1).ToString("MM/dd/yyyy") + "' ";
                queryDocumentosDestinados += " AND DD.DDE_DATA_EMISSAO > '" + dataSaida.ToString("MM/dd/yyyy") + "' AND DD.DDE_DATA_EMISSAO < '" + dataSaida.AddDays(1).ToString("MM/dd/yyyy") + "' ";
                queryDocumentosEntrada += " AND TDE.TDE_DATA_EMISSAO > '" + dataSaida.ToString("MM/dd/yyyy") + "' AND TDE.TDE_DATA_EMISSAO < '" + dataSaida.AddDays(1).ToString("MM/dd/yyyy") + "' ";
            }

            if (status > 0)
                querySub += " AND N.NFI_STATUS = '" + (int)status + "'";

            if (tipoEmissao >= 0)
                querySub += " AND N.NFI_TIPO_EMISSAO = '" + (int)tipoEmissao + "'";

            var ambiente = (int)Dominio.Enumeradores.TipoAmbiente.Producao;
            querySub += " AND N.NFI_AMBIENTE = " + ambiente;
            querySubNFSe += " AND C.CON_TIPO_AMBIENTE = " + ambiente;
            querySubTitulo += " AND T.TIT_AMBIENTE = " + ambiente;

            string query = @"SELECT COUNT(0) as CONTADOR FROM ( 
                 SELECT EP.EMP_CODIGO CodigoEmpresaPai, 
                 EP.EMP_CNPJ CNPJEmpresaPai, 
                 EP.EMP_RAZAO NomeEmpresaPai, 
                 E.EMP_DATACADASTRO DataCadastro, 
                 E.EMP_CODIGO CodigoEmpresa, 
                 E.EMP_RAZAO NomeEmpresa, 
                 E.EMP_CNPJ CNPJEmpresa, 
                 ISNULL((SELECT COUNT(1) FROM T_NOTA_FISCAL N JOIN T_EMPRESA_SERIE S ON S.ESE_CODIGO = N.ESE_CODIGO 
                 WHERE N.EMP_CODIGO = E.EMP_CODIGO AND N.NFI_STATUS = 2 " + querySub + @"), 0) QtdInutilizadas, 
                 ISNULL((SELECT COUNT(1) FROM T_NOTA_FISCAL N JOIN T_EMPRESA_SERIE S ON S.ESE_CODIGO = N.ESE_CODIGO 
                 WHERE N.EMP_CODIGO = E.EMP_CODIGO AND N.NFI_STATUS = 3 " + querySub + @"), 0) QtdCanceladas, 
                 ISNULL((SELECT COUNT(1) FROM T_NOTA_FISCAL N JOIN T_EMPRESA_SERIE S ON S.ESE_CODIGO = N.ESE_CODIGO 
                 WHERE N.EMP_CODIGO = E.EMP_CODIGO AND N.NFI_STATUS = 1 " + querySub + @"), 0) QtdEmitidas, 
                 ISNULL((SELECT COUNT(1) FROM T_NOTA_FISCAL N JOIN T_EMPRESA_SERIE S ON S.ESE_CODIGO = N.ESE_CODIGO 
                 WHERE N.EMP_CODIGO = E.EMP_CODIGO AND N.NFI_STATUS = 7 " + querySub + @"), 0) QtdProcessadas, 
                 ISNULL((SELECT COUNT(1) FROM T_NOTA_FISCAL N JOIN T_EMPRESA_SERIE S ON S.ESE_CODIGO = N.ESE_CODIGO 
                 WHERE N.EMP_CODIGO = E.EMP_CODIGO AND N.NFI_STATUS = 5 " + querySub + @"), 0) QtdDenegadas, 
                 ISNULL((SELECT COUNT(1) FROM T_NOTA_FISCAL N JOIN T_EMPRESA_SERIE S ON S.ESE_CODIGO = N.ESE_CODIGO 
                 WHERE N.EMP_CODIGO = E.EMP_CODIGO AND N.NFI_STATUS = 6 " + querySub + @"), 0) QtdRejeitadas, 
                 ISNULL((SELECT COUNT(1) FROM T_NOTA_FISCAL N JOIN T_EMPRESA_SERIE S ON S.ESE_CODIGO = N.ESE_CODIGO 
                 WHERE N.EMP_CODIGO = E.EMP_CODIGO AND N.NFI_STATUS = 4 " + querySub + @"), 0) QtdAutorizadas, 
                 ISNULL((select COUNT(1) from T_NOTA_FISCAL_CARTA_CORRECAO C 
                 JOIN T_NOTA_FISCAL N ON N.NFI_CODIGO = C.NFI_CODIGO AND N.EMP_CODIGO = E.EMP_CODIGO  
                 JOIN T_EMPRESA_SERIE S ON S.ESE_CODIGO = N.ESE_CODIGO  
                 WHERE C.NCC_STATUS = 4 " + querySub + @"), 0) QtdCCe, 
                 ISNULL((SELECT COUNT(1) FROM T_CTE C JOIN T_EMPRESA_SERIE S ON S.ESE_CODIGO = C.CON_SERIE 
                 WHERE CON_MODELODOC = ISNULL((SELECT MOD_CODIGO FROM T_MODDOCFISCAL WHERE MOD_NUM = '38'), 31) 
                 AND C.EMP_CODIGO = E.EMP_CODIGO AND C.CON_PROTOCOLO IS NOT NULL " + querySubNFSe + @"), 0) QtdNFSe, 
                 ISNULL((SELECT COUNT(1) FROM T_TITULO T 
                 WHERE T.EMP_CODIGO = E.EMP_CODIGO AND T.TIT_TIPO = 1 AND T.TIT_NOSSO_NUMERO IS NOT NULL " + querySubTitulo + @"), 0) QtdBoletos,
                 ISNULL((SELECT COUNT(DISTINCT DD.DDE_CHAVE) FROM T_DOCUMENTO_DESTINADO_EMPRESA DD WHERE DD.EMP_CODIGO = E.EMP_CODIGO " + queryDocumentosDestinados + @"), 0) QtdNFDestinada,
                 ISNULL((SELECT COUNT(DISTINCT TDE.TDE_CODIGO) FROM T_TMS_DOCUMENTO_ENTRADA TDE WHERE TDE.EMP_CODIGO = E.EMP_CODIGO " + queryDocumentosEntrada + @"), 0) QtdDocumentoEntrada,

                 (ISNULL((SELECT COUNT(1) FROM T_NOTA_FISCAL N JOIN T_EMPRESA_SERIE S ON S.ESE_CODIGO = N.ESE_CODIGO 
                 WHERE N.EMP_CODIGO = E.EMP_CODIGO AND N.NFI_STATUS = 4 " + querySub + @"), 0) + 
                 ISNULL((SELECT COUNT(1) FROM T_CTE C JOIN T_EMPRESA_SERIE S ON S.ESE_CODIGO = C.CON_SERIE 
                 WHERE CON_MODELODOC = ISNULL((SELECT MOD_CODIGO FROM T_MODDOCFISCAL WHERE MOD_NUM = '38'), 31) 
                 AND C.EMP_CODIGO = E.EMP_CODIGO AND C.CON_PROTOCOLO IS NOT NULL " + querySubNFSe + @"), 0) + 
                 ISNULL((SELECT COUNT(DISTINCT TDE.TDE_CODIGO) FROM T_TMS_DOCUMENTO_ENTRADA TDE
                 WHERE TDE.EMP_CODIGO = E.EMP_CODIGO " + queryDocumentosEntrada + @"), 0) +
                 ISNULL((SELECT COUNT(1) FROM T_TITULO T 
                 WHERE T.EMP_CODIGO = E.EMP_CODIGO AND T.TIT_TIPO = 1 AND T.TIT_NOSSO_NUMERO IS NOT NULL " + querySubTitulo + @"), 0)) TotalDocumentos

                 FROM T_EMPRESA E 
                 JOIN T_EMPRESA EP ON EP.EMP_CODIGO = E.EMP_EMPRESA 
                 WHERE 1 = 1 ";

            if (codigoEmpresa > 0 && codigoEmpresa != 137)
                query += " AND EP.EMP_CODIGO = " + codigoEmpresa.ToString();
            if (empresa > 0)
                query += " AND E.EMP_CODIGO = " + empresa.ToString();

            if (statusEmpresa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                query += " AND E.EMP_STATUS = 'A'";
            else if (statusEmpresa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                query += " AND E.EMP_STATUS = 'I'";

            if (dataInicialCadastro != DateTime.MinValue)
                query += " AND E.EMP_DATACADASTRO >= '" + dataInicialCadastro.ToString("MM/dd/yyyy") + "'";

            if (dataFinalCadastro != DateTime.MinValue)
                query += " AND E.EMP_DATACADASTRO <= '" + dataFinalCadastro.AddDays(1).ToString("MM/dd/yyyy") + "'";

            query += ") AS T WHERE 1 = 1 ";
            if (exibirSomenteClientesComEmissao)
                query += " AND T.TotalDocumentos > 0 ";
            else
                query += " AND T.QtdInutilizadas > 0 OR T.QtdCanceladas > 0 OR T.QtdEmitidas > 0 OR T.QtdProcessadas > 0 " +
                     " OR T.QtdDenegadas > 0 OR T.QtdRejeitadas > 0 OR T.QtdAutorizadas > 0 OR T.QtdCCe > 0 OR T.QtdNFSe > 0 OR T.QtdBoletos > 0 OR T.QtdNFDestinada > 0 ";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.NFe.GiroEstoque> RelatorioGiroEstoque(Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioGiroEstoque filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            string queryParameters = "", queryEntrada = "", querySaida = "";
            if (filtrosPesquisa.CodigoEmpresa > 0)
                queryParameters += " AND E.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresa.ToString();

            if (filtrosPesquisa.CodigoProduto > 0)
                queryParameters += " AND IT.PRO_CODIGO = " + filtrosPesquisa.CodigoProduto.ToString();

            if (filtrosPesquisa.CodigoGrupoProduto > 0)
                queryParameters += " AND G.GPR_CODIGO = " + filtrosPesquisa.CodigoGrupoProduto.ToString();

            string pattern = "yyyy-MM-dd";
            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
            {
                querySaida += " AND CAST(IT.PEH_DATA AS DATE) >= '" + filtrosPesquisa.DataInicial.ToString(pattern) + "'";
                queryEntrada += " AND CAST(IT.PEH_DATA AS DATE) >= '" + filtrosPesquisa.DataInicial.ToString(pattern) + "'";
            }

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
            {
                querySaida += " AND CAST(IT.PEH_DATA AS DATE) <= '" + filtrosPesquisa.DataFinal.ToString(pattern) + "'";
                queryEntrada += " AND CAST(IT.PEH_DATA AS DATE) <= '" + filtrosPesquisa.DataFinal.ToString(pattern) + "'";
            }

            string query = @"   SELECT TT.CodigoProduto, 
                                TT.Produto, 
                                TT.GrupoProduto,
                                SUM(TT.QuantidadeEntrada) QuantidadeEntrada, 
                                SUM(TT.QuantidadeSaida) QuantidadeSaida, 
                                TT.EstoqueAtual, 
                                TT.UnidadeMedida,
                                SUM(TT.ValorEntrada) ValorEntrada, 
                                SUM(TT.ValorSaida) ValorSaida,
                                CASE 
	                                WHEN SUM(TT.QuantidadeSaida) > 0 THEN TT.EstoqueAtual/SUM(TT.QuantidadeSaida)
	                                ELSE 0
                                END Giro,
                                SUM(TT.ValorEntrada) * SUM(TT.QuantidadeEntrada)  TotalEntrada, 
                                SUM(TT.ValorSaida) * SUM(TT.QuantidadeSaida) TotalSaida, TT.Empresa ,
                                (TT.EstoqueAtual + ( TT.EstoqueAtual + ( SUM(TT.QuantidadeEntrada) - SUM(TT.QuantidadeSaida)) / 2 )) MediaMes,
                                ((TT.EstoqueAtual + ( TT.EstoqueAtual + ( SUM(TT.QuantidadeEntrada) - SUM(TT.QuantidadeSaida)) / 2 )) / 12) MediaAno,
                                ISNULL((select SUM(ProdutoOS.OFP_QUANTIDADE_DOCUMENTO) from T_FROTA_ORDEM_SERVICO_FECHAMENTO_PRODUTO ProdutoOS
                                JOIN T_FROTA_ORDEM_SERVICO OrdemServico on OrdemServico.OSE_CODIGO = ProdutoOS.OSE_CODIGO
                                where OrdemServico.OSE_SITUACAO in (3,7) and (OrdemServico.OSE_TIPO_OFICINA = 1 OR OrdemServico.OSE_TIPO_OFICINA IS NULL)
                                AND ProdutoOS.PRO_CODIGO = TT.CodigoProduto), 0) EstoqueReservado

                                FROM (

								SELECT IT.PRO_CODIGO CodigoProduto, 
                                P.PRO_DESCRICAO Produto,
                                GRP_DESCRICAO GrupoProduto,
                                SUM(IT.PEH_QUANTIDADE) QuantidadeEntrada, 
                                AVG(IT.PEH_CUSTO) ValorEntrada, 
                                0 QuantidadeSaida, 
                                0 ValorSaida, 
                                ISNULL(PE.PRE_QUANTIDADE, 0) EstoqueAtual,
                                P.PRO_UNIDADE_MEDIDA UnidadeMedida,
                                E.EMP_RAZAO Empresa,
                                0 MediaMes,
                                0 MediaAno

                                FROM  T_PRODUTO_ESTOQUE_HISTORICO IT                                
                                JOIN T_PRODUTO P ON P.PRO_CODIGO = IT.PRO_CODIGO
                                LEFT OUTER JOIN T_PRODUTO_ESTOQUE PE ON PE.PRO_CODIGO = P.PRO_CODIGO AND PE.EMP_CODIGO = IT.EMP_CODIGO
                                LEFT OUTER JOIN T_EMPRESA E ON E.EMP_CODIGO = IT.EMP_CODIGO
                                LEFT OUTER JOIN T_GRUPO_PRODUTO_TMS G ON G.GPR_CODIGO = P.GPR_CODIGO
						        JOIN T_LOCAL_ARMAZENAMENTO_PRODUTO LAP on LAP.LAP_CODIGO = PE.LAP_CODIGO		
						        JOIN T_LOCAL_ARMAZENAMENTO_PRODUTO LAPE ON LAPE.LAP_CODIGO = PE.LAP_CODIGO 
                                WHERE IT.PEH_TIPO = 0 AND IT.PEH_QUANTIDADE > 0 " + queryParameters + querySaida + @" 
								GROUP BY IT.PRO_CODIGO, P.PRO_DESCRICAO, PE.PRE_QUANTIDADE, P.PRO_UNIDADE_MEDIDA, E.EMP_RAZAO, GRP_DESCRICAO

                                UNION ALL

                                SELECT IT.PRO_CODIGO CodigoProduto, 
                                P.PRO_DESCRICAO Produto,
                                GRP_DESCRICAO GrupoProduto,
                                0 QuantidadeEntrada, 
                                0 ValorEntrada, 
                                SUM(IT.PEH_QUANTIDADE) QuantidadeSaida, 
                                AVG(IT.PEH_CUSTO) ValorSaida, 
                                ISNULL(PE.PRE_QUANTIDADE, 0) EstoqueAtual,
                                P.PRO_UNIDADE_MEDIDA UnidadeMedida,
                                E.EMP_RAZAO Empresa,
                                0 MediaMes,
                                0 MediaAno

                                FROM T_PRODUTO_ESTOQUE_HISTORICO IT                                
                                JOIN T_PRODUTO P ON P.PRO_CODIGO = IT.PRO_CODIGO
                                LEFT OUTER JOIN T_PRODUTO_ESTOQUE PE ON PE.PRO_CODIGO = P.PRO_CODIGO AND PE.EMP_CODIGO = IT.EMP_CODIGO
                                LEFT OUTER JOIN T_EMPRESA E ON E.EMP_CODIGO = IT.EMP_CODIGO
                                LEFT OUTER JOIN T_GRUPO_PRODUTO_TMS G ON G.GPR_CODIGO = P.GPR_CODIGO
						        JOIN T_LOCAL_ARMAZENAMENTO_PRODUTO LAP on LAP.LAP_CODIGO = PE.LAP_CODIGO		
						        JOIN T_LOCAL_ARMAZENAMENTO_PRODUTO LAPE ON LAPE.LAP_CODIGO = PE.LAP_CODIGO 
                                WHERE IT.PEH_TIPO = 1 AND IT.PEH_QUANTIDADE > 0 " + queryParameters + queryEntrada + @"
								GROUP BY IT.PRO_CODIGO, P.PRO_DESCRICAO, PE.PRE_QUANTIDADE, P.PRO_UNIDADE_MEDIDA, E.EMP_RAZAO, GRP_DESCRICAO) AS TT
								GROUP BY TT.CodigoProduto, TT.Produto, TT.GrupoProduto, TT.EstoqueAtual, TT.UnidadeMedida, TT.Empresa ";

            var agrup = false;
            if (!string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeAgrupar))
            {
                agrup = true;
                query += " order by " + parametrosConsulta.PropriedadeAgrupar + " " + parametrosConsulta.DirecaoAgrupar;
            }

            if (!string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeOrdenar) && parametrosConsulta.PropriedadeAgrupar != parametrosConsulta.PropriedadeOrdenar)
            {
                if (agrup)
                {
                    query += ", " + parametrosConsulta.PropriedadeOrdenar + " " + parametrosConsulta.DirecaoOrdenar;
                }
                else
                {
                    query += " order by " + parametrosConsulta.PropriedadeOrdenar + " " + parametrosConsulta.DirecaoOrdenar;
                }
            }

            if (parametrosConsulta.LimiteRegistros > 0)
                query += " OFFSET " + parametrosConsulta.InicioRegistros + " ROWS FETCH FIRST " + parametrosConsulta.LimiteRegistros + " ROWS ONLY";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.NFe.GiroEstoque)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.NFe.GiroEstoque>();
        }

        public int ContarRelatorioGiroEstoque(Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioGiroEstoque filtrosPesquisa)
        {
            string queryParameters = "", queryEntrada = "", querySaida = "";
            if (filtrosPesquisa.CodigoEmpresa > 0)
                queryParameters += " AND E.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresa.ToString();

            if (filtrosPesquisa.CodigoProduto > 0)
                queryParameters += " AND IT.PRO_CODIGO = " + filtrosPesquisa.CodigoProduto.ToString();

            if (filtrosPesquisa.CodigoGrupoProduto > 0)
                queryParameters += " AND G.GPR_CODIGO = " + filtrosPesquisa.CodigoGrupoProduto.ToString();

            string pattern = "yyyy-MM-dd";
            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
            {
                querySaida += " AND CAST(IT.PEH_DATA AS DATE) >= '" + filtrosPesquisa.DataInicial.ToString(pattern) + "'";
                queryEntrada += " AND CAST(IT.PEH_DATA AS DATE) >= '" + filtrosPesquisa.DataInicial.ToString(pattern) + "'";
            }

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
            {
                querySaida += " AND CAST(IT.PEH_DATA AS DATE) <= '" + filtrosPesquisa.DataFinal.ToString(pattern) + "'";
                queryEntrada += " AND CAST(IT.PEH_DATA AS DATE) <= '" + filtrosPesquisa.DataFinal.ToString(pattern) + "'";
            }

            string query = @"   SELECT COUNT(0) as CONTADOR FROM (

                                SELECT COUNT(0) as CONTADOR FROM (

                                SELECT IT.PRO_CODIGO CodigoProduto, 
                                P.PRO_DESCRICAO Produto,
                                GRP_DESCRICAO GrupoProduto,
                                SUM(IT.PEH_QUANTIDADE) QuantidadeEntrada, 
                                AVG(IT.PEH_CUSTO) ValorEntrada, 
                                0 QuantidadeSaida, 
                                0 ValorSaida, 
                                ISNULL(PE.PRE_QUANTIDADE, 0) EstoqueAtual,
                                P.PRO_UNIDADE_MEDIDA UnidadeMedida,
                                E.EMP_RAZAO Empresa

                                FROM  T_PRODUTO_ESTOQUE_HISTORICO IT                                
                                JOIN T_PRODUTO P ON P.PRO_CODIGO = IT.PRO_CODIGO
                                LEFT OUTER JOIN T_PRODUTO_ESTOQUE PE ON PE.PRO_CODIGO = P.PRO_CODIGO AND PE.EMP_CODIGO = IT.EMP_CODIGO
                                LEFT OUTER JOIN T_EMPRESA E ON E.EMP_CODIGO = IT.EMP_CODIGO
                                LEFT OUTER JOIN T_GRUPO_PRODUTO_TMS G ON G.GPR_CODIGO = P.GPR_CODIGO
                                WHERE IT.PEH_TIPO = 0 AND IT.PEH_QUANTIDADE > 0 " + queryParameters + querySaida + @" 
								GROUP BY IT.PRO_CODIGO, P.PRO_DESCRICAO, PE.PRE_QUANTIDADE, P.PRO_UNIDADE_MEDIDA, E.EMP_RAZAO, GRP_DESCRICAO

                                UNION ALL

                                SELECT IT.PRO_CODIGO CodigoProduto, 
                                P.PRO_DESCRICAO Produto,
                                GRP_DESCRICAO GrupoProduto,
                                0 QuantidadeEntrada, 
                                0 ValorEntrada, 
                                SUM(IT.PEH_QUANTIDADE) QuantidadeSaida, 
                                AVG(IT.PEH_CUSTO) ValorSaida, 
                                ISNULL(PE.PRE_QUANTIDADE, 0) EstoqueAtual,
                                P.PRO_UNIDADE_MEDIDA UnidadeMedida,
                                E.EMP_RAZAO Empresa

                                FROM T_PRODUTO_ESTOQUE_HISTORICO IT                                
                                JOIN T_PRODUTO P ON P.PRO_CODIGO = IT.PRO_CODIGO
                                LEFT OUTER JOIN T_PRODUTO_ESTOQUE PE ON PE.PRO_CODIGO = P.PRO_CODIGO AND PE.EMP_CODIGO = IT.EMP_CODIGO
                                LEFT OUTER JOIN T_EMPRESA E ON E.EMP_CODIGO = IT.EMP_CODIGO
                                LEFT OUTER JOIN T_GRUPO_PRODUTO_TMS G ON G.GPR_CODIGO = P.GPR_CODIGO
                                WHERE IT.PEH_TIPO = 1 AND IT.PEH_QUANTIDADE > 0 " + queryParameters + queryEntrada + @"
                                GROUP BY IT.PRO_CODIGO, P.PRO_DESCRICAO, PE.PRE_QUANTIDADE, P.PRO_UNIDADE_MEDIDA, E.EMP_RAZAO, GRP_DESCRICAO

                                ) AS T 

                                GROUP BY T.CodigoProduto, T.Produto, T.EstoqueAtual, T.UnidadeMedida, T.Empresa ) AS TT ";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.NFe.CurvaABCProduto> RelatorioCurvaABCProduto(int codigoEmpresa, TipoCompraVenda tipoMovimento, OrdenacaoCurvaABC ordenar, DateTime dataInicial, DateTime dataFinal, Dominio.Enumeradores.TipoAmbiente? tipoAmbiente, int codigoGrupoProduto, string propGrupo, string dirOrdenacaoGrupo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, bool paginar = true, bool todosCNPJdaRaizEmbarcador = false)
        {
            string query = "", queryParameters = "", queryOrder = "";
            if (codigoEmpresa > 0)
                queryParameters += " AND E.EMP_CODIGO = " + codigoEmpresa.ToString();

            if (codigoGrupoProduto > 0)
                queryParameters += " AND G.GPR_CODIGO = " + codigoGrupoProduto.ToString();

            if ((int)ordenar == 1)
                queryOrder += " ORDER BY Quantidade DESC";
            else if ((int)ordenar == 2)
                queryOrder += " ORDER BY Valor DESC";
            else
                queryOrder += " ORDER BY Vezes DESC";

            if ((int)tipoMovimento == 1) //Compra
            {
                if (dataInicial != DateTime.MinValue)
                    queryParameters += " AND N.TDE_DATA_EMISSAO >= '" + dataInicial.ToString("MM/dd/yyyy") + "'";

                if (dataFinal != DateTime.MinValue)
                    queryParameters += " AND N.TDE_DATA_EMISSAO <= '" + dataFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";

                query = @"   SELECT 0 Posicao,
                                IT.PRO_CODIGO CodigoProduto, 
                                P.PRO_DESCRICAO Produto,
                                ISNULL(PE.PRE_QUANTIDADE, 0) Estoque,
                                COUNT(IT.TDI_CODIGO) Vezes,
                                SUM(IT.TDI_QUANTIDADE) Quantidade, 
                                SUM(IT.TDI_VALOR_TOTAL) Valor, 
                                ((SUM(IT.TDI_VALOR_TOTAL) * 100) / 
	                                (SELECT SUM(IT.TDI_VALOR_TOTAL)
	                                FROM T_TMS_DOCUMENTO_ENTRADA_ITEM IT
	                                JOIN T_TMS_DOCUMENTO_ENTRADA N ON N.TDE_CODIGO = IT.TDE_CODIGO
	                                JOIN T_PRODUTO P ON P.PRO_CODIGO = IT.PRO_CODIGO
	                                JOIN T_EMPRESA E ON E.EMP_CODIGO = N.EMP_CODIGO
                                    LEFT OUTER JOIN T_GRUPO_PRODUTO_TMS G ON G.GPR_CODIGO = P.GPR_CODIGO
	                                WHERE 1 = 1 AND N.TDE_SITUACAO = 3" + queryParameters + @" )) Contribuicao,
                                0.00 Acumulado,
                                E.EMP_RAZAO Empresa,
                                GRP_DESCRICAO GrupoProduto
	
                                FROM T_TMS_DOCUMENTO_ENTRADA_ITEM IT
                                JOIN T_TMS_DOCUMENTO_ENTRADA N ON N.TDE_CODIGO = IT.TDE_CODIGO
                                JOIN T_PRODUTO P ON P.PRO_CODIGO = IT.PRO_CODIGO
                                LEFT OUTER JOIN T_PRODUTO_ESTOQUE PE ON PE.PRO_CODIGO = P.PRO_CODIGO AND PE.EMP_CODIGO = N.EMP_CODIGO
                                JOIN T_EMPRESA E ON E.EMP_CODIGO = N.EMP_CODIGO
                                LEFT OUTER JOIN T_GRUPO_PRODUTO_TMS G ON G.GPR_CODIGO = P.GPR_CODIGO
                                WHERE 1 = 1 AND N.TDE_SITUACAO = 3" + queryParameters + @"

                                GROUP BY IT.PRO_CODIGO, P.PRO_DESCRICAO, PE.PRE_QUANTIDADE, E.EMP_RAZAO, GRP_DESCRICAO " + queryOrder;
            }
            else
            {
                if (dataInicial != DateTime.MinValue)
                    queryParameters += " AND N.NFI_DATA_EMISSAO >= '" + dataInicial.ToString("MM/dd/yyyy") + "'";

                if (dataFinal != DateTime.MinValue)
                    queryParameters += " AND N.NFI_DATA_EMISSAO <= '" + dataFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";

                if (tipoAmbiente != null && tipoAmbiente.HasValue)
                    queryParameters += " AND N.NFI_AMBIENTE = " + (int)tipoAmbiente;

                query = @"   SELECT 0 Posicao,
                                IT.PRO_CODIGO CodigoProduto, 
                                P.PRO_DESCRICAO Produto,
                                ISNULL(PE.PRE_QUANTIDADE, 0) Estoque,
                                COUNT(IT.NFP_CODIGO) Vezes,
                                SUM(IT.NFP_QUANTIDADE) Quantidade, 
                                SUM(IT.NFP_VALOR_TOTAL) Valor, 
                                ((SUM(IT.NFP_VALOR_TOTAL) * 100) / 
	                                (SELECT SUM(IT.NFP_VALOR_TOTAL)
	                                FROM T_NOTA_FISCAL_PRODUTOS IT
	                                JOIN T_NOTA_FISCAL N ON N.NFI_CODIGO = IT.NFI_CODIGO
	                                JOIN T_PRODUTO P ON P.PRO_CODIGO = IT.PRO_CODIGO
	                                JOIN T_EMPRESA E ON E.EMP_CODIGO = N.EMP_CODIGO
                                    LEFT OUTER JOIN T_GRUPO_PRODUTO_TMS G ON G.GPR_CODIGO = P.GPR_CODIGO
	                                WHERE 1 = 1 AND N.NFI_STATUS = 4" + queryParameters + @" )) Contribuicao,
                                0.00 Acumulado,
                                E.EMP_RAZAO Empresa,
                                GRP_DESCRICAO GrupoProduto
	
                                FROM T_NOTA_FISCAL_PRODUTOS IT
                                JOIN T_NOTA_FISCAL N ON N.NFI_CODIGO = IT.NFI_CODIGO
                                JOIN T_PRODUTO P ON P.PRO_CODIGO = IT.PRO_CODIGO
                                LEFT OUTER JOIN T_PRODUTO_ESTOQUE PE ON PE.PRO_CODIGO = P.PRO_CODIGO AND PE.EMP_CODIGO = N.EMP_CODIGO
                                JOIN T_EMPRESA E ON E.EMP_CODIGO = N.EMP_CODIGO
                                LEFT OUTER JOIN T_GRUPO_PRODUTO_TMS G ON G.GPR_CODIGO = P.GPR_CODIGO
                                WHERE 1 = 1 AND N.NFI_STATUS = 4" + queryParameters + @"

                                GROUP BY IT.PRO_CODIGO, P.PRO_DESCRICAO, PE.PRE_QUANTIDADE, E.EMP_RAZAO, GRP_DESCRICAO " + queryOrder;
            }

            if (paginar)
                query += " OFFSET " + inicioRegistros + " ROWS FETCH FIRST " + maximoRegistros + " ROWS ONLY";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.NFe.CurvaABCProduto)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.NFe.CurvaABCProduto>();
        }

        public int ContarRelatorioCurvaABCProduto(int codigoEmpresa, TipoCompraVenda tipoMovimento, OrdenacaoCurvaABC ordenar, DateTime dataInicial, DateTime dataFinal, Dominio.Enumeradores.TipoAmbiente? tipoAmbiente, int codigoGrupoProduto)
        {
            string query = "", queryParameters = "";
            if (codigoEmpresa > 0)
                queryParameters += " AND E.EMP_CODIGO = " + codigoEmpresa.ToString();

            if (codigoGrupoProduto > 0)
                queryParameters += " AND G.GPR_CODIGO = " + codigoGrupoProduto.ToString();

            if ((int)tipoMovimento == 1) //Compra
            {
                if (dataInicial != DateTime.MinValue)
                    queryParameters += " AND N.TDE_DATA_EMISSAO >= '" + dataInicial.ToString("MM/dd/yyyy") + "'";

                if (dataFinal != DateTime.MinValue)
                    queryParameters += " AND N.TDE_DATA_EMISSAO <= '" + dataFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";

                query = @"   SELECT COUNT(0) as CONTADOR FROM(

                                SELECT IT.PRO_CODIGO CodigoProduto, 
                                P.PRO_DESCRICAO Produto,
                                ISNULL(PE.PRE_QUANTIDADE, 0) Estoque
	
                                FROM T_TMS_DOCUMENTO_ENTRADA_ITEM IT
                                JOIN T_TMS_DOCUMENTO_ENTRADA N ON N.TDE_CODIGO = IT.TDE_CODIGO
                                JOIN T_PRODUTO P ON P.PRO_CODIGO = IT.PRO_CODIGO
                                LEFT OUTER JOIN T_PRODUTO_ESTOQUE PE ON PE.PRO_CODIGO = P.PRO_CODIGO AND PE.EMP_CODIGO = N.EMP_CODIGO
                                JOIN T_EMPRESA E ON E.EMP_CODIGO = N.EMP_CODIGO
                                LEFT OUTER JOIN T_GRUPO_PRODUTO_TMS G ON G.GPR_CODIGO = P.GPR_CODIGO
                                WHERE 1 = 1 AND N.TDE_SITUACAO = 3" + queryParameters + @"

                                GROUP BY IT.PRO_CODIGO, P.PRO_DESCRICAO, PE.PRE_QUANTIDADE, E.EMP_RAZAO, GRP_DESCRICAO ) AS T ";
            }
            else
            {
                if (dataInicial != DateTime.MinValue)
                    queryParameters += " AND N.NFI_DATA_EMISSAO >= '" + dataInicial.ToString("MM/dd/yyyy") + "'";

                if (dataFinal != DateTime.MinValue)
                    queryParameters += " AND N.NFI_DATA_EMISSAO <= '" + dataFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";

                if (tipoAmbiente != null && tipoAmbiente.HasValue)
                    queryParameters += " AND N.NFI_AMBIENTE = " + (int)tipoAmbiente;

                query = @"   SELECT COUNT(0) as CONTADOR FROM(

                                SELECT IT.PRO_CODIGO CodigoProduto, 
                                P.PRO_DESCRICAO Produto,
                                ISNULL(PE.PRE_QUANTIDADE, 0) Estoque
	
                                FROM T_NOTA_FISCAL_PRODUTOS IT
                                JOIN T_NOTA_FISCAL N ON N.NFI_CODIGO = IT.NFI_CODIGO
                                JOIN T_PRODUTO P ON P.PRO_CODIGO = IT.PRO_CODIGO
                                LEFT OUTER JOIN T_PRODUTO_ESTOQUE PE ON PE.PRO_CODIGO = P.PRO_CODIGO AND PE.EMP_CODIGO = N.EMP_CODIGO
                                JOIN T_EMPRESA E ON E.EMP_CODIGO = N.EMP_CODIGO
                                LEFT OUTER JOIN T_GRUPO_PRODUTO_TMS G ON G.GPR_CODIGO = P.GPR_CODIGO
                                WHERE 1 = 1 AND N.NFI_STATUS = 4" + queryParameters + @"

                                GROUP BY IT.PRO_CODIGO, P.PRO_DESCRICAO, PE.PRE_QUANTIDADE, E.EMP_RAZAO, GRP_DESCRICAO ) AS T ";
            }

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.NFe.CurvaABCPessoa> RelatorioCurvaABCPessoa(int codigoEmpresa, TipoCompraVenda tipoMovimento, OrdenacaoCurvaABC ordenar, DateTime dataInicial, DateTime dataFinal, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, string propGrupo, string dirOrdenacaoGrupo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, bool paginar = true, bool todosCNPJdaRaizEmbarcador = false)
        {
            string query = "", queryParameters = "", queryOrder = "";
            if (codigoEmpresa > 0)
                queryParameters += " AND E.EMP_CODIGO = " + codigoEmpresa.ToString();

            if ((int)ordenar == 1)
                queryOrder += " ORDER BY Quantidade DESC";
            else
                queryOrder += " ORDER BY Valor DESC";

            if ((int)tipoMovimento == 1) //Compra
            {
                if (dataInicial != DateTime.MinValue)
                    queryParameters += " AND N.TDE_DATA_EMISSAO >= '" + dataInicial.ToString("MM/dd/yyyy") + "'";

                if (dataFinal != DateTime.MinValue)
                    queryParameters += " AND N.TDE_DATA_EMISSAO <= '" + dataFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";

                query = @"   SELECT 0 Posicao, 
                                C.CLI_NOME Pessoa,
                                C.CLI_FONE Telefone,
                                COUNT(N.TDE_CODIGO) Quantidade,
                                SUM(N.TDE_VALOR_TOTAL) Valor,
                                ((SUM(N.TDE_VALOR_TOTAL) * 100) / 
	                                (SELECT SUM(N.TDE_VALOR_TOTAL)
	                                FROM T_TMS_DOCUMENTO_ENTRADA N 
	                                JOIN T_CLIENTE C ON C.CLI_CGCCPF = N.CLI_CGCCPF
	                                JOIN T_EMPRESA E ON E.EMP_CODIGO = N.EMP_CODIGO
	                                WHERE 1 = 1 AND N.TDE_SITUACAO = 3" + queryParameters + @" )) Contribuicao,
                                0.00 Acumulado
	
                                FROM T_TMS_DOCUMENTO_ENTRADA N
                                JOIN T_CLIENTE C ON C.CLI_CGCCPF = N.CLI_CGCCPF
                                JOIN T_EMPRESA E ON E.EMP_CODIGO = N.EMP_CODIGO
                                WHERE 1 = 1 AND N.TDE_SITUACAO = 3" + queryParameters + @"

                                GROUP BY N.CLI_CGCCPF, C.CLI_NOME, C.CLI_FONE " + queryOrder;
            }
            else
            {
                if (dataInicial != DateTime.MinValue)
                    queryParameters += " AND N.NFI_DATA_EMISSAO >= '" + dataInicial.ToString("MM/dd/yyyy") + "'";

                if (dataFinal != DateTime.MinValue)
                    queryParameters += " AND N.NFI_DATA_EMISSAO <= '" + dataFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";

                if (tipoAmbiente != Dominio.Enumeradores.TipoAmbiente.Nenhum)
                    queryParameters += " AND N.NFI_AMBIENTE = " + (int)tipoAmbiente;

                query = @"   SELECT 0 Posicao,
                                C.CLI_NOME Pessoa,
                                C.CLI_FONE Telefone,
                                COUNT(N.NFI_CODIGO) Quantidade,
                                SUM(N.NFI_VALOR_TOTAL_NOTA) Valor,
                                ((SUM(N.NFI_VALOR_TOTAL_NOTA) * 100) / 
	                                (SELECT SUM(N.NFI_VALOR_TOTAL_NOTA)
	                                FROM T_NOTA_FISCAL N 
	                                JOIN T_CLIENTE C ON C.CLI_CGCCPF = N.CLI_CGCCPF
	                                JOIN T_EMPRESA E ON E.EMP_CODIGO = N.EMP_CODIGO
	                                WHERE 1 = 1 AND N.NFI_STATUS = 4" + queryParameters + @" )) Contribuicao,
                                0.00 Acumulado
	
                                FROM T_NOTA_FISCAL N 
                                JOIN T_CLIENTE C ON C.CLI_CGCCPF = N.CLI_CGCCPF
                                JOIN T_EMPRESA E ON E.EMP_CODIGO = N.EMP_CODIGO
                                WHERE 1 = 1 AND N.NFI_STATUS = 4" + queryParameters + @"

                                GROUP BY N.CLI_CGCCPF, C.CLI_NOME, C.CLI_FONE " + queryOrder;
            }

            if (paginar)
                query += " OFFSET " + inicioRegistros + " ROWS FETCH FIRST " + maximoRegistros + " ROWS ONLY";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.NFe.CurvaABCPessoa)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.NFe.CurvaABCPessoa>();
        }

        public int ContarRelatorioCurvaABCPessoa(int codigoEmpresa, TipoCompraVenda tipoMovimento, OrdenacaoCurvaABC ordenar, DateTime dataInicial, DateTime dataFinal, Dominio.Enumeradores.TipoAmbiente tipoAmbiente)
        {
            string query = "", queryParameters = "";
            if (codigoEmpresa > 0)
                queryParameters += " AND E.EMP_CODIGO = " + codigoEmpresa.ToString();

            if ((int)tipoMovimento == 1) //Compra
            {
                if (dataInicial != DateTime.MinValue)
                    queryParameters += " AND N.TDE_DATA_EMISSAO >= '" + dataInicial.ToString("MM/dd/yyyy") + "'";

                if (dataFinal != DateTime.MinValue)
                    queryParameters += " AND N.TDE_DATA_EMISSAO <= '" + dataFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";

                query = @"   SELECT COUNT(0) as CONTADOR FROM(

                                SELECT C.CLI_NOME Pessoa,
                                C.CLI_FONE Telefone
	
                                FROM T_TMS_DOCUMENTO_ENTRADA N
                                JOIN T_CLIENTE C ON C.CLI_CGCCPF = N.CLI_CGCCPF
                                JOIN T_EMPRESA E ON E.EMP_CODIGO = N.EMP_CODIGO
                                WHERE 1 = 1 AND N.TDE_SITUACAO = 3" + queryParameters + @"

                                GROUP BY N.CLI_CGCCPF, C.CLI_NOME, C.CLI_FONE ) AS T ";
            }
            else
            {
                if (dataInicial != DateTime.MinValue)
                    queryParameters += " AND N.NFI_DATA_EMISSAO >= '" + dataInicial.ToString("MM/dd/yyyy") + "'";

                if (dataFinal != DateTime.MinValue)
                    queryParameters += " AND N.NFI_DATA_EMISSAO <= '" + dataFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";

                if (tipoAmbiente != Dominio.Enumeradores.TipoAmbiente.Nenhum)
                    queryParameters += " AND N.NFI_AMBIENTE = " + (int)tipoAmbiente;

                query = @"   SELECT COUNT(0) as CONTADOR FROM(

                                SELECT C.CLI_NOME Pessoa,
                                C.CLI_FONE Telefone
	
                                FROM T_NOTA_FISCAL N 
                                JOIN T_CLIENTE C ON C.CLI_CGCCPF = N.CLI_CGCCPF
                                JOIN T_EMPRESA E ON E.EMP_CODIGO = N.EMP_CODIGO
                                WHERE 1 = 1 AND N.NFI_STATUS = 4" + queryParameters + @"

                                GROUP BY N.CLI_CGCCPF, C.CLI_NOME, C.CLI_FONE ) AS T ";
            }

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.NFe.HistoricoProduto> RelatorioHistoricoProduto(int codigoEmpresa, int codigoProduto, TipoEntradaSaida tipoMovimento, DateTime dataInicial, DateTime dataFinal, Dominio.Enumeradores.TipoAmbiente? tipoAmbiente, int codigoGrupoProduto, string propGrupo, string dirOrdenacaoGrupo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, TipoServicoMultisoftware tipoServicoMultisoftware, bool paginar = true, bool todosCNPJdaRaizEmbarcador = false)
        {
            string query = "", queryParameters = "", queryEntrada = "", querySaida = "", queryParametersReq = "", querySaidaReq = "", queryEstoqueImportado = "";
            if (codigoEmpresa > 0)
            {
                queryParameters += " AND E.EMP_CODIGO = " + codigoEmpresa.ToString();
                queryParametersReq += " AND EE.EMP_CODIGO = " + codigoEmpresa.ToString();
            }

            if (codigoProduto > 0)
            {
                queryParameters += " AND P.PRO_CODIGO = " + codigoProduto.ToString();
                queryParametersReq += " AND P.PRO_CODIGO = " + codigoProduto.ToString();
            }

            if (tipoAmbiente != null && tipoAmbiente.HasValue)
                querySaida += " AND N.NFI_AMBIENTE = " + (int)tipoAmbiente;

            if (dataInicial != DateTime.MinValue)
            {
                querySaidaReq += " AND R.RME_DATA_APROVACAO >= '" + dataInicial.ToString("MM/dd/yyyy") + "'";
                querySaida += " AND N.NFI_DATA_EMISSAO >= '" + dataInicial.ToString("MM/dd/yyyy") + "'";
                queryEntrada += " AND N.TDE_DATA_EMISSAO >= '" + dataInicial.ToString("MM/dd/yyyy") + "'";
                queryEstoqueImportado += " AND PEH_DATA >= '" + dataInicial.ToString("MM/dd/yyyy") + "'";
            }

            if (dataFinal != DateTime.MinValue)
            {
                querySaidaReq += " AND R.RME_DATA_APROVACAO <= '" + dataFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";
                querySaida += " AND N.NFI_DATA_EMISSAO <= '" + dataFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";
                queryEntrada += " AND N.TDE_DATA_EMISSAO <= '" + dataFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";
                queryEstoqueImportado += " AND PEH_DATA <= '" + dataFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";
            }

            if (codigoGrupoProduto > 0)
            {
                queryParameters += " AND G.GPR_CODIGO = " + codigoGrupoProduto.ToString();
                queryParametersReq += " AND G.GPR_CODIGO = " + codigoGrupoProduto.ToString();
            }

            if ((int)tipoMovimento == 1) //Entrada
            {
                query = @"   SELECT IT.PRO_CODIGO CodigoProduto, 
                                P.PRO_DESCRICAO Produto,
                                C.CLI_NOME Pessoa,
                                N.TDE_DATA_EMISSAO Data,
                                'Entrada' DescricaoTipo,
                                N.TDE_NUMERO_LONG Numero,
                                SUM(IT.TDI_QUANTIDADE) Quantidade, 
                                SUM(IT.TDI_VALOR_UNITARIO) ValorUnitario, 
                                SUM(IT.TDI_VALOR_TOTAL) Valor,
                                E.EMP_RAZAO Empresa,
                                GRP_DESCRICAO GrupoProduto
	
                                FROM T_TMS_DOCUMENTO_ENTRADA_ITEM IT
                                JOIN T_TMS_DOCUMENTO_ENTRADA N ON N.TDE_CODIGO = IT.TDE_CODIGO
                                JOIN T_CLIENTE C ON C.CLI_CGCCPF = N.CLI_CGCCPF
                                JOIN T_PRODUTO P ON P.PRO_CODIGO = IT.PRO_CODIGO
                                JOIN T_EMPRESA E ON E.EMP_CODIGO = N.EMP_CODIGO
                                LEFT OUTER JOIN T_GRUPO_PRODUTO_TMS G ON G.GPR_CODIGO = P.GPR_CODIGO
                                WHERE 1 = 1 " + queryParameters + queryEntrada + @"
                                GROUP BY IT.PRO_CODIGO, P.PRO_DESCRICAO, C.CLI_NOME, N.TDE_DATA_EMISSAO, N.TDE_NUMERO_LONG, E.EMP_RAZAO, GRP_DESCRICAO

                                UNION ALL

                                SELECT 
                                P.PRO_CODIGO CodigoProduto, 
                                P.PRO_DESCRICAO Produto,
                                'IMPORTADO SISTEMA ANTERIOR' Pessoa,
                                PEH_DATA Data,
                                'Entrada' DescricaoTipo,
                                PEH_DOCUMENTO Numero,
                                PEH_QUANTIDADE Quantidade, 
                                0 ValorUnitario, 
                                0 Valor,
                                EE.EMP_RAZAO Empresa,
                                GRP_DESCRICAO GrupoProduto

                                FROM T_PRODUTO_ESTOQUE_HISTORICO PEH
                                JOIN T_PRODUTO P ON P.PRO_CODIGO = PEH.PRO_CODIGO
                                JOIN T_EMPRESA EE ON EE.EMP_CODIGO = PEH.EMP_CODIGO
                                LEFT OUTER JOIN T_GRUPO_PRODUTO_TMS G ON G.GPR_CODIGO = P.GPR_CODIGO
                                WHERE PEH_TIPO_DOCUMENTO = 'IMP' AND PEH_TIPO = 0 " + queryParametersReq + queryEstoqueImportado;
            }
            else if ((int)tipoMovimento == 2) //Saída
            {
                query = @"   SELECT IT.PRO_CODIGO CodigoProduto, 
                                P.PRO_DESCRICAO Produto,
                                C.CLI_NOME Pessoa,
                                N.NFI_DATA_EMISSAO Data,
                                'Saída' DescricaoTipo,
                                N.NFI_NUMERO Numero,
                                -SUM(IT.NFP_QUANTIDADE) Quantidade, 
                                SUM(IT.NFP_VALOR_UNITARIO) ValorUnitario, 
                                -SUM(IT.NFP_VALOR_TOTAL) Valor,
                                E.EMP_RAZAO Empresa,
                                GRP_DESCRICAO GrupoProduto

                                FROM T_NOTA_FISCAL_PRODUTOS IT
                                JOIN T_NOTA_FISCAL N ON N.NFI_CODIGO = IT.NFI_CODIGO
                                JOIN T_CLIENTE C ON C.CLI_CGCCPF = N.CLI_CGCCPF
                                JOIN T_PRODUTO P ON P.PRO_CODIGO = IT.PRO_CODIGO
                                JOIN T_EMPRESA E ON E.EMP_CODIGO = N.EMP_CODIGO
                                LEFT OUTER JOIN T_GRUPO_PRODUTO_TMS G ON G.GPR_CODIGO = P.GPR_CODIGO
                                WHERE 1 = 1 " + queryParameters + querySaida + @"

                                GROUP BY IT.PRO_CODIGO, P.PRO_DESCRICAO, C.CLI_NOME, N.NFI_DATA_EMISSAO, N.NFI_NUMERO, E.EMP_RAZAO, GRP_DESCRICAO " +

                                (tipoServicoMultisoftware == TipoServicoMultisoftware.MultiTMS ? @"

                                UNION ALL

                                SELECT 
                                P.PRO_CODIGO CodigoProduto, 
                                P.PRO_DESCRICAO Produto,
                                PEH_TIPO_DOCUMENTO Pessoa,
                                PEH_DATA Data,
                                'Saída' DescricaoTipo,
                                case when isnumeric(PEH_DOCUMENTO) = 1 then PEH_DOCUMENTO else 0 end Numero,
                                PEH_QUANTIDADE Quantidade, 
                                0 ValorUnitario, 
                                0 Valor,
                                E.EMP_RAZAO Empresa,
                                GRP_DESCRICAO GrupoProduto

                                 FROM T_PRODUTO_ESTOQUE_HISTORICO PEH
                                JOIN T_PRODUTO P ON P.PRO_CODIGO = PEH.PRO_CODIGO
                                JOIN T_EMPRESA E ON E.EMP_CODIGO = PEH.EMP_CODIGO
                                LEFT OUTER JOIN T_GRUPO_PRODUTO_TMS G ON G.GPR_CODIGO = P.GPR_CODIGO
                                WHERE PEH_TIPO = 1" + queryParameters + queryEstoqueImportado : "")


                                + @" UNION ALL

                                SELECT P.PRO_CODIGO CodigoProduto, 
                                P.PRO_DESCRICAO Produto,
                                F.FUN_NOME Pessoa,
                                R.RME_DATA_APROVACAO Data,
                                'Saída' DescricaoTipo,
                                R.MER_NUMERO Numero,
                                -SUM((MER_QUANTIDADE - ISNULL(MER_SALDO, 0))) Quantidade, 
                                SUM(P.PRO_CUSTO_MEDIO) ValorUnitario, 
                                -SUM(((MER_QUANTIDADE - ISNULL(MER_SALDO, 0)) * P.PRO_CUSTO_MEDIO)) Valor,
                                EE.EMP_RAZAO Empresa,
                                GRP_DESCRICAO GrupoProduto

                                FROM T_MERCADORIA M
                                JOIN T_REQUISICAO_MERCADORIA R ON R.RME_CODIGO = M.RME_CODIGO
                                JOIN T_PRODUTO_ESTOQUE E ON E.PRE_CODIGO = M.PRE_CODIGO AND E.EMP_CODIGO = R.EMP_CODIGO
                                JOIN T_PRODUTO P ON P.PRO_CODIGO = E.PRO_CODIGO
                                JOIN T_EMPRESA EE ON EE.EMP_CODIGO = R.EMP_CODIGO
                                JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = R.FUN_CODIGO
                                LEFT OUTER JOIN T_GRUPO_PRODUTO_TMS G ON G.GPR_CODIGO = P.GPR_CODIGO
                                WHERE M.MER_MODO = 1 AND R.RME_SITUACAO = 1
                                AND (MER_QUANTIDADE - ISNULL(MER_SALDO, 0)) > 0 " + queryParametersReq + querySaidaReq + @"
                                GROUP BY P.PRO_CODIGO, P.PRO_DESCRICAO,  F.FUN_NOME, R.RME_DATA_APROVACAO, R.MER_NUMERO, EE.EMP_RAZAO, GRP_DESCRICAO ";
            }
            else
            {
                query = @"   SELECT IT.PRO_CODIGO CodigoProduto, 
                                P.PRO_DESCRICAO Produto,
                                C.CLI_NOME Pessoa,
                                N.TDE_DATA_EMISSAO Data,
                                'Entrada' DescricaoTipo,
                                N.TDE_NUMERO_LONG Numero,
                                SUM(IT.TDI_QUANTIDADE) Quantidade, 
                                SUM(IT.TDI_VALOR_UNITARIO) ValorUnitario, 
                                SUM(IT.TDI_VALOR_TOTAL) Valor, 
                                E.EMP_RAZAO Empresa,
                                GRP_DESCRICAO GrupoProduto
	
                                FROM T_TMS_DOCUMENTO_ENTRADA_ITEM IT
                                JOIN T_TMS_DOCUMENTO_ENTRADA N ON N.TDE_CODIGO = IT.TDE_CODIGO
                                JOIN T_CLIENTE C ON C.CLI_CGCCPF = N.CLI_CGCCPF
                                JOIN T_PRODUTO P ON P.PRO_CODIGO = IT.PRO_CODIGO
                                JOIN T_EMPRESA E ON E.EMP_CODIGO = N.EMP_CODIGO
                                LEFT OUTER JOIN T_GRUPO_PRODUTO_TMS G ON G.GPR_CODIGO = P.GPR_CODIGO
                                WHERE 1 = 1 " + queryParameters + queryEntrada + @"
                                GROUP BY IT.PRO_CODIGO, P.PRO_DESCRICAO, C.CLI_NOME, N.TDE_DATA_EMISSAO, N.TDE_NUMERO_LONG, E.EMP_RAZAO, GRP_DESCRICAO " +

                                 (tipoServicoMultisoftware == TipoServicoMultisoftware.MultiTMS ? @"

                                UNION ALL

                                SELECT
                                P.PRO_CODIGO CodigoProduto,
                                P.PRO_DESCRICAO Produto,
                                PEH_TIPO_DOCUMENTO Pessoa,
                                PEH_DATA Data,
                                'Saída' DescricaoTipo,
                                case when isnumeric(PEH_DOCUMENTO) = 1 then PEH_DOCUMENTO else 0 end Numero,
                                PEH_QUANTIDADE Quantidade, 
                                0 ValorUnitario, 
                                0 Valor,
                                E.EMP_RAZAO Empresa,
                                GRP_DESCRICAO GrupoProduto

                                 FROM T_PRODUTO_ESTOQUE_HISTORICO PEH
                                JOIN T_PRODUTO P ON P.PRO_CODIGO = PEH.PRO_CODIGO
                                JOIN T_EMPRESA E ON E.EMP_CODIGO = PEH.EMP_CODIGO
                                LEFT OUTER JOIN T_GRUPO_PRODUTO_TMS G ON G.GPR_CODIGO = P.GPR_CODIGO
                                WHERE PEH_TIPO = 1" + queryParameters + queryEstoqueImportado : "")


                                + @" UNION ALL

                                SELECT IT.PRO_CODIGO CodigoProduto, 
                                P.PRO_DESCRICAO Produto,
                                C.CLI_NOME Pessoa,
                                N.NFI_DATA_EMISSAO Data,
                                'Saída' DescricaoTipo,
                                N.NFI_NUMERO Numero,
                                -SUM(IT.NFP_QUANTIDADE) Quantidade, 
                                SUM(IT.NFP_VALOR_UNITARIO) ValorUnitario, 
                                -SUM(IT.NFP_VALOR_TOTAL) Valor, 
                                E.EMP_RAZAO Empresa,
                                GRP_DESCRICAO GrupoProduto

                                FROM T_NOTA_FISCAL_PRODUTOS IT
                                JOIN T_NOTA_FISCAL N ON N.NFI_CODIGO = IT.NFI_CODIGO
                                JOIN T_CLIENTE C ON C.CLI_CGCCPF = N.CLI_CGCCPF
                                JOIN T_PRODUTO P ON P.PRO_CODIGO = IT.PRO_CODIGO
                                JOIN T_EMPRESA E ON E.EMP_CODIGO = N.EMP_CODIGO
                                LEFT OUTER JOIN T_GRUPO_PRODUTO_TMS G ON G.GPR_CODIGO = P.GPR_CODIGO
                                WHERE 1 = 1 " + queryParameters + querySaida + @"

                                GROUP BY IT.PRO_CODIGO, P.PRO_DESCRICAO, C.CLI_NOME, N.NFI_DATA_EMISSAO, N.NFI_NUMERO, E.EMP_RAZAO, GRP_DESCRICAO

                                UNION ALL

                                SELECT P.PRO_CODIGO CodigoProduto, 
                                P.PRO_DESCRICAO Produto,
                                F.FUN_NOME Pessoa,
                                R.RME_DATA_APROVACAO Data,
                                'Saída' DescricaoTipo,
                                R.MER_NUMERO Numero,
                                -SUM((MER_QUANTIDADE - ISNULL(MER_SALDO, 0))) Quantidade, 
                                SUM(P.PRO_CUSTO_MEDIO) ValorUnitario, 
                                -SUM(((MER_QUANTIDADE - ISNULL(MER_SALDO, 0)) * P.PRO_CUSTO_MEDIO)) Valor, 
                                EE.EMP_RAZAO Empresa,
                                GRP_DESCRICAO GrupoProduto

                                FROM T_MERCADORIA M
                                JOIN T_REQUISICAO_MERCADORIA R ON R.RME_CODIGO = M.RME_CODIGO
                                JOIN T_PRODUTO_ESTOQUE E ON E.PRE_CODIGO = M.PRE_CODIGO AND E.EMP_CODIGO = R.EMP_CODIGO
                                JOIN T_PRODUTO P ON P.PRO_CODIGO = E.PRO_CODIGO
                                JOIN T_EMPRESA EE ON EE.EMP_CODIGO = R.EMP_CODIGO
                                JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = R.FUN_CODIGO
                                LEFT OUTER JOIN T_GRUPO_PRODUTO_TMS G ON G.GPR_CODIGO = P.GPR_CODIGO
                                WHERE M.MER_MODO = 1 AND R.RME_SITUACAO = 1
                                AND (MER_QUANTIDADE - ISNULL(MER_SALDO, 0)) > 0 " + queryParametersReq + querySaidaReq + @"
                                GROUP BY P.PRO_CODIGO, P.PRO_DESCRICAO,  F.FUN_NOME, R.RME_DATA_APROVACAO, R.MER_NUMERO, EE.EMP_RAZAO, GRP_DESCRICAO

                                UNION ALL

                                SELECT 
                                P.PRO_CODIGO CodigoProduto, 
                                P.PRO_DESCRICAO Produto,
                                'IMPORTADO SISTEMA ANTERIOR' Pessoa,
                                PEH_DATA Data,
                                'Entrada' DescricaoTipo,
                                PEH_DOCUMENTO Numero,
                                PEH_QUANTIDADE Quantidade, 
                                0 ValorUnitario, 
                                0 Valor,
                                EE.EMP_RAZAO Empresa,
                                GRP_DESCRICAO GrupoProduto

                                FROM T_PRODUTO_ESTOQUE_HISTORICO PEH
                                JOIN T_PRODUTO P ON P.PRO_CODIGO = PEH.PRO_CODIGO
                                JOIN T_EMPRESA EE ON EE.EMP_CODIGO = PEH.EMP_CODIGO
                                LEFT OUTER JOIN T_GRUPO_PRODUTO_TMS G ON G.GPR_CODIGO = P.GPR_CODIGO
                                WHERE PEH_TIPO_DOCUMENTO = 'IMP' AND PEH_TIPO = 0 " + queryParametersReq + queryEstoqueImportado;
            }

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

            if (paginar)
                query += " OFFSET " + inicioRegistros + " ROWS FETCH FIRST " + maximoRegistros + " ROWS ONLY";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.NFe.HistoricoProduto)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.NFe.HistoricoProduto>();
        }

        public int ContarRelatorioHistoricoProduto(int codigoEmpresa, int codigoProduto, TipoEntradaSaida tipoMovimento, DateTime dataInicial, DateTime dataFinal, Dominio.Enumeradores.TipoAmbiente? tipoAmbiente, int codigoGrupoProduto, TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            string query = "", queryParameters = "", queryEntrada = "", querySaida = "", queryParametersReq = "", querySaidaReq = "", queryEstoqueImportado = "";
            if (codigoEmpresa > 0)
            {
                queryParameters += " AND E.EMP_CODIGO = " + codigoEmpresa.ToString();
                queryParametersReq += " AND EE.EMP_CODIGO = " + codigoEmpresa.ToString();
            }

            if (codigoProduto > 0)
            {
                queryParameters += " AND P.PRO_CODIGO = " + codigoProduto.ToString();
                queryParametersReq += " AND P.PRO_CODIGO = " + codigoProduto.ToString();
            }

            if (tipoAmbiente != null && tipoAmbiente.HasValue)
                querySaida += " AND N.NFI_AMBIENTE = " + (int)tipoAmbiente;

            if (dataInicial != DateTime.MinValue)
            {
                querySaidaReq += " AND R.RME_DATA_APROVACAO >= '" + dataInicial.ToString("MM/dd/yyyy") + "'";
                querySaida += " AND N.NFI_DATA_EMISSAO >= '" + dataInicial.ToString("MM/dd/yyyy") + "'";
                queryEntrada += " AND N.TDE_DATA_EMISSAO >= '" + dataInicial.ToString("MM/dd/yyyy") + "'";
                queryEstoqueImportado += " AND PEH_DATA >= '" + dataInicial.ToString("MM/dd/yyyy") + "'";
            }

            if (dataFinal != DateTime.MinValue)
            {
                querySaidaReq += " AND R.RME_DATA_APROVACAO <= '" + dataFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";
                querySaida += " AND N.NFI_DATA_EMISSAO <= '" + dataFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";
                queryEntrada += " AND N.TDE_DATA_EMISSAO <= '" + dataFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";
                queryEstoqueImportado += " AND PEH_DATA <= '" + dataFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";
            }

            if (codigoGrupoProduto > 0)
            {
                queryParameters += " AND G.GPR_CODIGO = " + codigoGrupoProduto.ToString();
                queryParametersReq += " AND G.GPR_CODIGO = " + codigoGrupoProduto.ToString();
            }

            if ((int)tipoMovimento == 1) //Entrada
            {
                query = @"   SELECT COUNT(0) as CONTADOR FROM(

                                SELECT IT.PRO_CODIGO CodigoProduto, 
                                P.PRO_DESCRICAO Produto,
                                C.CLI_NOME Pessoa,
                                N.TDE_DATA_EMISSAO Data,
                                'Entrada' DescricaoTipo,
                                N.TDE_NUMERO_LONG Numero,
                                SUM(IT.TDI_QUANTIDADE) Quantidade, 
                                SUM(IT.TDI_VALOR_UNITARIO) ValorUnitario, 
                                SUM(IT.TDI_VALOR_TOTAL) Valor
	
                                FROM T_TMS_DOCUMENTO_ENTRADA_ITEM IT
                                JOIN T_TMS_DOCUMENTO_ENTRADA N ON N.TDE_CODIGO = IT.TDE_CODIGO
                                JOIN T_CLIENTE C ON C.CLI_CGCCPF = N.CLI_CGCCPF
                                JOIN T_PRODUTO P ON P.PRO_CODIGO = IT.PRO_CODIGO
                                JOIN T_EMPRESA E ON E.EMP_CODIGO = N.EMP_CODIGO
                                LEFT OUTER JOIN T_GRUPO_PRODUTO_TMS G ON G.GPR_CODIGO = P.GPR_CODIGO
                                WHERE 1 = 1 " + queryParameters + queryEntrada + @"
                                GROUP BY IT.PRO_CODIGO, P.PRO_DESCRICAO, C.CLI_NOME, N.TDE_DATA_EMISSAO, N.TDE_NUMERO_LONG, E.EMP_RAZAO, GRP_DESCRICAO 

                                UNION ALL

                                SELECT 
                                P.PRO_CODIGO CodigoProduto, 
                                P.PRO_DESCRICAO Produto,
                                'IMPORTADO SISTEMA ANTERIOR' Pessoa,
                                PEH_DATA Data,
                                'Entrada' DescricaoTipo,
                                PEH_DOCUMENTO Numero,
                                PEH_QUANTIDADE Quantidade, 
                                0 ValorUnitario, 
                                0 Valor

                                FROM T_PRODUTO_ESTOQUE_HISTORICO PEH
                                JOIN T_PRODUTO P ON P.PRO_CODIGO = PEH.PRO_CODIGO
                                JOIN T_EMPRESA EE ON EE.EMP_CODIGO = PEH.EMP_CODIGO
                                LEFT OUTER JOIN T_GRUPO_PRODUTO_TMS G ON G.GPR_CODIGO = P.GPR_CODIGO
                                WHERE PEH_TIPO_DOCUMENTO = 'IMP' AND PEH_TIPO = 0 " + queryParametersReq + queryEstoqueImportado + @") AS T ";
            }
            else if ((int)tipoMovimento == 2) //Saída
            {
                query = @"   SELECT COUNT(0) as CONTADOR FROM(

                                SELECT IT.PRO_CODIGO CodigoProduto, 
                                P.PRO_DESCRICAO Produto,
                                C.CLI_NOME Pessoa,
                                N.NFI_DATA_EMISSAO Data,
                                'Saída' DescricaoTipo,
                                N.NFI_NUMERO Numero,
                                -SUM(IT.NFP_QUANTIDADE) Quantidade, 
                                SUM(IT.NFP_VALOR_UNITARIO) ValorUnitario, 
                                -SUM(IT.NFP_VALOR_TOTAL) Valor

                                FROM T_NOTA_FISCAL_PRODUTOS IT
                                JOIN T_NOTA_FISCAL N ON N.NFI_CODIGO = IT.NFI_CODIGO
                                JOIN T_CLIENTE C ON C.CLI_CGCCPF = N.CLI_CGCCPF
                                JOIN T_PRODUTO P ON P.PRO_CODIGO = IT.PRO_CODIGO
                                JOIN T_EMPRESA E ON E.EMP_CODIGO = N.EMP_CODIGO
                                LEFT OUTER JOIN T_GRUPO_PRODUTO_TMS G ON G.GPR_CODIGO = P.GPR_CODIGO
                                WHERE 1 = 1 " + queryParameters + querySaida + @"

                                GROUP BY IT.PRO_CODIGO, P.PRO_DESCRICAO, C.CLI_NOME, N.NFI_DATA_EMISSAO, N.NFI_NUMERO, E.EMP_RAZAO, GRP_DESCRICAO " +

                                (tipoServicoMultisoftware == TipoServicoMultisoftware.MultiTMS ? @"

                                UNION ALL

                                SELECT 
                                P.PRO_CODIGO CodigoProduto, 
                                P.PRO_DESCRICAO Produto,
                                PEH_TIPO_DOCUMENTO Pessoa,
                                PEH_DATA Data,
                                'Saída' DescricaoTipo,
                                case when isnumeric(PEH_DOCUMENTO) = 1 then PEH_DOCUMENTO else 0 end Numero,
                                PEH_QUANTIDADE Quantidade, 
                                0 ValorUnitario, 
                                0 Valor

                                 FROM T_PRODUTO_ESTOQUE_HISTORICO PEH
                                JOIN T_PRODUTO P ON P.PRO_CODIGO = PEH.PRO_CODIGO
                                JOIN T_EMPRESA E ON E.EMP_CODIGO = PEH.EMP_CODIGO
                                LEFT OUTER JOIN T_GRUPO_PRODUTO_TMS G ON G.GPR_CODIGO = P.GPR_CODIGO
                                WHERE PEH_TIPO = 1" + queryParameters + queryEstoqueImportado : "")

                                + @" UNION ALL

                                SELECT P.PRO_CODIGO CodigoProduto, 
                                P.PRO_DESCRICAO Produto,
                                F.FUN_NOME Pessoa,
                                R.RME_DATA_APROVACAO Data,
                                'Saída' DescricaoTipo,
                                R.MER_NUMERO Numero,
                                -SUM((MER_QUANTIDADE - ISNULL(MER_SALDO, 0))) Quantidade, 
                                SUM(P.PRO_CUSTO_MEDIO) ValorUnitario, 
                                -SUM(((MER_QUANTIDADE - ISNULL(MER_SALDO, 0)) * P.PRO_CUSTO_MEDIO)) Valor

                                FROM T_MERCADORIA M
                                JOIN T_REQUISICAO_MERCADORIA R ON R.RME_CODIGO = M.RME_CODIGO
                                JOIN T_PRODUTO_ESTOQUE E ON E.PRE_CODIGO = M.PRE_CODIGO AND E.EMP_CODIGO = R.EMP_CODIGO
                                JOIN T_PRODUTO P ON P.PRO_CODIGO = E.PRO_CODIGO
                                JOIN T_EMPRESA EE ON EE.EMP_CODIGO = R.EMP_CODIGO
                                JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = R.FUN_CODIGO
                                LEFT OUTER JOIN T_GRUPO_PRODUTO_TMS G ON G.GPR_CODIGO = P.GPR_CODIGO
                                WHERE M.MER_MODO = 1 AND R.RME_SITUACAO = 1
                                AND (MER_QUANTIDADE - ISNULL(MER_SALDO, 0)) > 0 " + queryParametersReq + querySaidaReq + @"
                                GROUP BY P.PRO_CODIGO, P.PRO_DESCRICAO,  F.FUN_NOME, R.RME_DATA_APROVACAO, R.MER_NUMERO, EE.EMP_RAZAO, GRP_DESCRICAO) AS T ";
            }
            else
            {
                query = @"   SELECT COUNT(0) as CONTADOR FROM(

                                SELECT IT.PRO_CODIGO CodigoProduto, 
                                P.PRO_DESCRICAO Produto,
                                C.CLI_NOME Pessoa,
                                N.TDE_DATA_EMISSAO Data,
                                'Entrada' DescricaoTipo,
                                N.TDE_NUMERO_LONG Numero
	
                                FROM T_TMS_DOCUMENTO_ENTRADA_ITEM IT
                                JOIN T_TMS_DOCUMENTO_ENTRADA N ON N.TDE_CODIGO = IT.TDE_CODIGO
                                JOIN T_CLIENTE C ON C.CLI_CGCCPF = N.CLI_CGCCPF
                                JOIN T_PRODUTO P ON P.PRO_CODIGO = IT.PRO_CODIGO
                                JOIN T_EMPRESA E ON E.EMP_CODIGO = N.EMP_CODIGO
                                LEFT OUTER JOIN T_GRUPO_PRODUTO_TMS G ON G.GPR_CODIGO = P.GPR_CODIGO
                                WHERE 1 = 1 " + queryParameters + queryEntrada + @"
                                GROUP BY IT.PRO_CODIGO, P.PRO_DESCRICAO, C.CLI_NOME, N.TDE_DATA_EMISSAO, N.TDE_NUMERO_LONG, E.EMP_RAZAO, GRP_DESCRICAO" +

                                (tipoServicoMultisoftware == TipoServicoMultisoftware.MultiTMS ? @"

                                UNION ALL

                                SELECT 
                                P.PRO_CODIGO CodigoProduto, 
                                P.PRO_DESCRICAO Produto,
                                PEH_TIPO_DOCUMENTO Pessoa,
                                PEH_DATA Data,
                                'Saída' DescricaoTipo,
                                case when isnumeric(PEH_DOCUMENTO) = 1 then PEH_DOCUMENTO else 0 end Numero

                                 FROM T_PRODUTO_ESTOQUE_HISTORICO PEH
                                JOIN T_PRODUTO P ON P.PRO_CODIGO = PEH.PRO_CODIGO
                                JOIN T_EMPRESA E ON E.EMP_CODIGO = PEH.EMP_CODIGO
                                LEFT OUTER JOIN T_GRUPO_PRODUTO_TMS G ON G.GPR_CODIGO = P.GPR_CODIGO
                                WHERE PEH_TIPO = 1" + queryParameters + queryEstoqueImportado : "")

                                + @" UNION ALL

                                SELECT IT.PRO_CODIGO CodigoProduto, 
                                P.PRO_DESCRICAO Produto,
                                C.CLI_NOME Pessoa,
                                N.NFI_DATA_EMISSAO Data,
                                'Saída' DescricaoTipo,
                                N.NFI_NUMERO Numero

                                FROM T_NOTA_FISCAL_PRODUTOS IT
                                JOIN T_NOTA_FISCAL N ON N.NFI_CODIGO = IT.NFI_CODIGO
                                JOIN T_CLIENTE C ON C.CLI_CGCCPF = N.CLI_CGCCPF
                                JOIN T_PRODUTO P ON P.PRO_CODIGO = IT.PRO_CODIGO
                                JOIN T_EMPRESA E ON E.EMP_CODIGO = N.EMP_CODIGO
                                LEFT OUTER JOIN T_GRUPO_PRODUTO_TMS G ON G.GPR_CODIGO = P.GPR_CODIGO
                                WHERE 1 = 1 " + queryParameters + querySaida + @"

                                GROUP BY IT.PRO_CODIGO, P.PRO_DESCRICAO, C.CLI_NOME, N.NFI_DATA_EMISSAO, N.NFI_NUMERO, E.EMP_RAZAO, GRP_DESCRICAO 

                                UNION ALL

                                SELECT P.PRO_CODIGO CodigoProduto, 
                                P.PRO_DESCRICAO Produto,
                                F.FUN_NOME Pessoa,
                                R.RME_DATA_APROVACAO Data,
                                'Saída' DescricaoTipo,
                                R.MER_NUMERO Numero 

                                FROM T_MERCADORIA M
                                JOIN T_REQUISICAO_MERCADORIA R ON R.RME_CODIGO = M.RME_CODIGO
                                JOIN T_PRODUTO_ESTOQUE E ON E.PRE_CODIGO = M.PRE_CODIGO AND E.EMP_CODIGO = R.EMP_CODIGO
                                JOIN T_PRODUTO P ON P.PRO_CODIGO = E.PRO_CODIGO
                                JOIN T_EMPRESA EE ON EE.EMP_CODIGO = R.EMP_CODIGO
                                JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = R.FUN_CODIGO
                                LEFT OUTER JOIN T_GRUPO_PRODUTO_TMS G ON G.GPR_CODIGO = P.GPR_CODIGO
                                WHERE M.MER_MODO = 1 AND R.RME_SITUACAO = 1
                                AND (MER_QUANTIDADE - ISNULL(MER_SALDO, 0)) > 0 " + queryParametersReq + querySaidaReq + @"
                                GROUP BY P.PRO_CODIGO, P.PRO_DESCRICAO,  F.FUN_NOME, R.RME_DATA_APROVACAO, R.MER_NUMERO, EE.EMP_RAZAO, GRP_DESCRICAO

                                UNION ALL

                                SELECT 
                                P.PRO_CODIGO CodigoProduto, 
                                P.PRO_DESCRICAO Produto,
                                'IMPORTADO SISTEMA ANTERIOR' Pessoa,
                                PEH_DATA Data,
                                'Entrada' DescricaoTipo,
                                PEH_DOCUMENTO Numero

                                FROM T_PRODUTO_ESTOQUE_HISTORICO PEH
                                JOIN T_PRODUTO P ON P.PRO_CODIGO = PEH.PRO_CODIGO
                                JOIN T_EMPRESA EE ON EE.EMP_CODIGO = PEH.EMP_CODIGO
                                LEFT OUTER JOIN T_GRUPO_PRODUTO_TMS G ON G.GPR_CODIGO = P.GPR_CODIGO
                                WHERE PEH_TIPO_DOCUMENTO = 'IMP' AND PEH_TIPO = 0 " + queryParametersReq + queryEstoqueImportado + @") AS T ";
            }

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.NFe.CompraVendaNCM> RelatorioCompraVendaNCM(int codigoEmpresa, int codigoProduto, int codigoCidade, string ncms, string estado, TipoCompraVenda tipoMovimento, DateTime dataInicial, DateTime dataFinal, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, string propGrupo, string dirOrdenacaoGrupo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, bool paginar = true, bool todosCNPJdaRaizEmbarcador = false)
        {
            string query = "", queryParameters = "", queryEntrada = "", querySaida = "";
            if (codigoEmpresa > 0)
                queryParameters += " AND E.EMP_CODIGO = " + codigoEmpresa.ToString();

            if (codigoProduto > 0)
                queryParameters += " AND IT.PRO_CODIGO = " + codigoProduto.ToString();

            if (codigoCidade > 0)
                queryParameters += " AND C.LOC_CODIGO = " + codigoCidade.ToString();

            if (!string.IsNullOrWhiteSpace(ncms))
                queryParameters += " AND P.PRO_COD_NCM IN (" + ncms + ")";

            if (!string.IsNullOrWhiteSpace(estado))
                queryParameters += " AND L.UF_SIGLA = '" + estado + "'";

            querySaida += " AND N.NFI_AMBIENTE = " + (int)tipoAmbiente;

            if (dataInicial != DateTime.MinValue)
            {
                querySaida += " AND N.NFI_DATA_EMISSAO >= '" + dataInicial.ToString("MM/dd/yyyy") + "'";
                queryEntrada += " AND N.TDE_DATA_EMISSAO >= '" + dataInicial.ToString("MM/dd/yyyy") + "'";
            }

            if (dataFinal != DateTime.MinValue)
            {
                querySaida += " AND N.NFI_DATA_EMISSAO <= '" + dataFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";
                queryEntrada += " AND N.TDE_DATA_EMISSAO <= '" + dataFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";
            }

            if ((int)tipoMovimento == 1) //Compra
            {
                query = @"   SELECT IT.PRO_CODIGO CodigoProduto, 
                                P.PRO_DESCRICAO Produto,
                                IT.TDI_CST_COFINS COFINS,
                                IT.TDI_CST_PIS PIS,
                                P.PRO_COD_NCM NCM,
                                'Entrada' DescricaoTipo,
                                N.TDE_NUMERO_LONG Numero,
                                P.PRO_UNIDADE_MEDIDA UnidadeMedida,
                                SUM(IT.TDI_QUANTIDADE) Quantidade, 
                                SUM(IT.TDI_VALOR_UNITARIO) ValorUnitario, 
                                SUM(IT.TDI_VALOR_TOTAL) Valor
	
                                FROM T_TMS_DOCUMENTO_ENTRADA_ITEM IT
                                JOIN T_TMS_DOCUMENTO_ENTRADA N ON N.TDE_CODIGO = IT.TDE_CODIGO
                                JOIN T_CLIENTE C ON C.CLI_CGCCPF = N.CLI_CGCCPF
                                JOIN T_LOCALIDADES L ON L.LOC_CODIGO = C.LOC_CODIGO
                                JOIN T_PRODUTO P ON P.PRO_CODIGO = IT.PRO_CODIGO
                                JOIN T_EMPRESA E ON E.EMP_CODIGO = N.EMP_CODIGO
                                WHERE 1 = 1 " + queryParameters + queryEntrada + @"
                                GROUP BY IT.PRO_CODIGO, P.PRO_DESCRICAO, IT.TDI_CST_COFINS, IT.TDI_CST_PIS, P.PRO_COD_NCM, N.TDE_NUMERO_LONG, P.PRO_UNIDADE_MEDIDA ";
            }
            else if ((int)tipoMovimento == 2) //Venda
            {
                query = @"   SELECT IT.PRO_CODIGO CodigoProduto, 
                                P.PRO_DESCRICAO Produto,
                                CASE 
	                                WHEN IT.NFP_CST_COFINS = 1 THEN '01'
	                                WHEN IT.NFP_CST_COFINS = 2 THEN '02'
                                    WHEN IT.NFP_CST_COFINS = 3 THEN '03'
                                    WHEN IT.NFP_CST_COFINS = 4 THEN '04'
                                    WHEN IT.NFP_CST_COFINS = 5 THEN '05'
                                    WHEN IT.NFP_CST_COFINS = 6 THEN '06'
                                    WHEN IT.NFP_CST_COFINS = 7 THEN '07'
                                    WHEN IT.NFP_CST_COFINS = 8 THEN '08'
                                    WHEN IT.NFP_CST_COFINS = 9 THEN '09'
                                    WHEN IT.NFP_CST_COFINS = 10 THEN '49'
                                    WHEN IT.NFP_CST_COFINS = 11 THEN '50'
                                    WHEN IT.NFP_CST_COFINS = 12 THEN '51'
                                    WHEN IT.NFP_CST_COFINS = 13 THEN '52'
                                    WHEN IT.NFP_CST_COFINS = 14 THEN '53'
                                    WHEN IT.NFP_CST_COFINS = 15 THEN '54'
                                    WHEN IT.NFP_CST_COFINS = 16 THEN '55'
                                    WHEN IT.NFP_CST_COFINS = 17 THEN '56'
                                    WHEN IT.NFP_CST_COFINS = 18 THEN '60'
                                    WHEN IT.NFP_CST_COFINS = 19 THEN '61'
                                    WHEN IT.NFP_CST_COFINS = 20 THEN '62'
                                    WHEN IT.NFP_CST_COFINS = 21 THEN '63'
                                    WHEN IT.NFP_CST_COFINS = 22 THEN '64'
                                    WHEN IT.NFP_CST_COFINS = 23 THEN '65'
                                    WHEN IT.NFP_CST_COFINS = 24 THEN '66'
                                    WHEN IT.NFP_CST_COFINS = 25 THEN '67'
                                    WHEN IT.NFP_CST_COFINS = 26 THEN '70'
                                    WHEN IT.NFP_CST_COFINS = 27 THEN '71'
                                    WHEN IT.NFP_CST_COFINS = 28 THEN '72'
                                    WHEN IT.NFP_CST_COFINS = 29 THEN '73'
                                    WHEN IT.NFP_CST_COFINS = 30 THEN '74'
                                    WHEN IT.NFP_CST_COFINS = 31 THEN '75'
                                    WHEN IT.NFP_CST_COFINS = 32 THEN '98'
                                    WHEN IT.NFP_CST_COFINS = 33 THEN '99'
	                                ELSE ''
                                END COFINS,
                                CASE 
	                                WHEN IT.NFP_CST_PIS = 1 THEN '01'
	                                WHEN IT.NFP_CST_PIS = 2 THEN '02'
                                    WHEN IT.NFP_CST_PIS = 3 THEN '03'
                                    WHEN IT.NFP_CST_PIS = 4 THEN '04'
                                    WHEN IT.NFP_CST_PIS = 5 THEN '05'
                                    WHEN IT.NFP_CST_PIS = 6 THEN '06'
                                    WHEN IT.NFP_CST_PIS = 7 THEN '07'
                                    WHEN IT.NFP_CST_PIS = 8 THEN '08'
                                    WHEN IT.NFP_CST_PIS = 9 THEN '09'
                                    WHEN IT.NFP_CST_PIS = 10 THEN '49'
                                    WHEN IT.NFP_CST_PIS = 11 THEN '50'
                                    WHEN IT.NFP_CST_PIS = 12 THEN '51'
                                    WHEN IT.NFP_CST_PIS = 13 THEN '52'
                                    WHEN IT.NFP_CST_PIS = 14 THEN '53'
                                    WHEN IT.NFP_CST_PIS = 15 THEN '54'
                                    WHEN IT.NFP_CST_PIS = 16 THEN '55'
                                    WHEN IT.NFP_CST_PIS = 17 THEN '56'
                                    WHEN IT.NFP_CST_PIS = 18 THEN '60'
                                    WHEN IT.NFP_CST_PIS = 19 THEN '61'
                                    WHEN IT.NFP_CST_PIS = 20 THEN '62'
                                    WHEN IT.NFP_CST_PIS = 21 THEN '63'
                                    WHEN IT.NFP_CST_PIS = 22 THEN '64'
                                    WHEN IT.NFP_CST_PIS = 23 THEN '65'
                                    WHEN IT.NFP_CST_PIS = 24 THEN '66'
                                    WHEN IT.NFP_CST_PIS = 25 THEN '67'
                                    WHEN IT.NFP_CST_PIS = 26 THEN '70'
                                    WHEN IT.NFP_CST_PIS = 27 THEN '71'
                                    WHEN IT.NFP_CST_PIS = 28 THEN '72'
                                    WHEN IT.NFP_CST_PIS = 29 THEN '73'
                                    WHEN IT.NFP_CST_PIS = 30 THEN '74'
                                    WHEN IT.NFP_CST_PIS = 31 THEN '75'
                                    WHEN IT.NFP_CST_PIS = 32 THEN '98'
                                    WHEN IT.NFP_CST_PIS = 33 THEN '99'
	                                ELSE ''
                                END PIS,
                                P.PRO_COD_NCM NCM,
                                'Saída' DescricaoTipo,
                                N.NFI_NUMERO Numero,
                                P.PRO_UNIDADE_MEDIDA UnidadeMedida,
                                SUM(IT.NFP_QUANTIDADE) Quantidade, 
                                SUM(IT.NFP_VALOR_UNITARIO) ValorUnitario, 
                                SUM(IT.NFP_VALOR_TOTAL) Valor

                                FROM T_NOTA_FISCAL_PRODUTOS IT
                                JOIN T_NOTA_FISCAL N ON N.NFI_CODIGO = IT.NFI_CODIGO
                                JOIN T_CLIENTE C ON C.CLI_CGCCPF = N.CLI_CGCCPF
                                JOIN T_LOCALIDADES L ON L.LOC_CODIGO = C.LOC_CODIGO
                                JOIN T_PRODUTO P ON P.PRO_CODIGO = IT.PRO_CODIGO
                                JOIN T_EMPRESA E ON E.EMP_CODIGO = N.EMP_CODIGO
                                WHERE 1 = 1 " + queryParameters + querySaida + @"

                                GROUP BY IT.PRO_CODIGO, P.PRO_DESCRICAO, IT.NFP_CST_COFINS, IT.NFP_CST_PIS, P.PRO_COD_NCM, N.NFI_NUMERO, P.PRO_UNIDADE_MEDIDA ";
            }
            else
            {
                query = @"   SELECT IT.PRO_CODIGO CodigoProduto, 
                                P.PRO_DESCRICAO Produto,
                                IT.TDI_CST_COFINS COFINS,
                                IT.TDI_CST_PIS PIS,
                                P.PRO_COD_NCM NCM,
                                'Entrada' DescricaoTipo,
                                N.TDE_NUMERO_LONG Numero,
                                P.PRO_UNIDADE_MEDIDA UnidadeMedida,
                                SUM(IT.TDI_QUANTIDADE) Quantidade, 
                                SUM(IT.TDI_VALOR_UNITARIO) ValorUnitario, 
                                SUM(IT.TDI_VALOR_TOTAL) Valor
	
                                FROM T_TMS_DOCUMENTO_ENTRADA_ITEM IT
                                JOIN T_TMS_DOCUMENTO_ENTRADA N ON N.TDE_CODIGO = IT.TDE_CODIGO
                                JOIN T_CLIENTE C ON C.CLI_CGCCPF = N.CLI_CGCCPF
                                JOIN T_LOCALIDADES L ON L.LOC_CODIGO = C.LOC_CODIGO
                                JOIN T_PRODUTO P ON P.PRO_CODIGO = IT.PRO_CODIGO
                                JOIN T_EMPRESA E ON E.EMP_CODIGO = N.EMP_CODIGO
                                WHERE 1 = 1 " + queryParameters + queryEntrada + @"
                                GROUP BY IT.PRO_CODIGO, P.PRO_DESCRICAO, IT.TDI_CST_COFINS, IT.TDI_CST_PIS, P.PRO_COD_NCM, N.TDE_NUMERO_LONG, P.PRO_UNIDADE_MEDIDA

                                UNION ALL

                                SELECT IT.PRO_CODIGO CodigoProduto, 
                                P.PRO_DESCRICAO Produto,
                                CASE 
	                                WHEN IT.NFP_CST_COFINS = 1 THEN '01'
	                                WHEN IT.NFP_CST_COFINS = 2 THEN '02'
                                    WHEN IT.NFP_CST_COFINS = 3 THEN '03'
                                    WHEN IT.NFP_CST_COFINS = 4 THEN '04'
                                    WHEN IT.NFP_CST_COFINS = 5 THEN '05'
                                    WHEN IT.NFP_CST_COFINS = 6 THEN '06'
                                    WHEN IT.NFP_CST_COFINS = 7 THEN '07'
                                    WHEN IT.NFP_CST_COFINS = 8 THEN '08'
                                    WHEN IT.NFP_CST_COFINS = 9 THEN '09'
                                    WHEN IT.NFP_CST_COFINS = 10 THEN '49'
                                    WHEN IT.NFP_CST_COFINS = 11 THEN '50'
                                    WHEN IT.NFP_CST_COFINS = 12 THEN '51'
                                    WHEN IT.NFP_CST_COFINS = 13 THEN '52'
                                    WHEN IT.NFP_CST_COFINS = 14 THEN '53'
                                    WHEN IT.NFP_CST_COFINS = 15 THEN '54'
                                    WHEN IT.NFP_CST_COFINS = 16 THEN '55'
                                    WHEN IT.NFP_CST_COFINS = 17 THEN '56'
                                    WHEN IT.NFP_CST_COFINS = 18 THEN '60'
                                    WHEN IT.NFP_CST_COFINS = 19 THEN '61'
                                    WHEN IT.NFP_CST_COFINS = 20 THEN '62'
                                    WHEN IT.NFP_CST_COFINS = 21 THEN '63'
                                    WHEN IT.NFP_CST_COFINS = 22 THEN '64'
                                    WHEN IT.NFP_CST_COFINS = 23 THEN '65'
                                    WHEN IT.NFP_CST_COFINS = 24 THEN '66'
                                    WHEN IT.NFP_CST_COFINS = 25 THEN '67'
                                    WHEN IT.NFP_CST_COFINS = 26 THEN '70'
                                    WHEN IT.NFP_CST_COFINS = 27 THEN '71'
                                    WHEN IT.NFP_CST_COFINS = 28 THEN '72'
                                    WHEN IT.NFP_CST_COFINS = 29 THEN '73'
                                    WHEN IT.NFP_CST_COFINS = 30 THEN '74'
                                    WHEN IT.NFP_CST_COFINS = 31 THEN '75'
                                    WHEN IT.NFP_CST_COFINS = 32 THEN '98'
                                    WHEN IT.NFP_CST_COFINS = 33 THEN '99'
	                                ELSE ''
                                END COFINS,
                                CASE 
	                                WHEN IT.NFP_CST_PIS = 1 THEN '01'
	                                WHEN IT.NFP_CST_PIS = 2 THEN '02'
                                    WHEN IT.NFP_CST_PIS = 3 THEN '03'
                                    WHEN IT.NFP_CST_PIS = 4 THEN '04'
                                    WHEN IT.NFP_CST_PIS = 5 THEN '05'
                                    WHEN IT.NFP_CST_PIS = 6 THEN '06'
                                    WHEN IT.NFP_CST_PIS = 7 THEN '07'
                                    WHEN IT.NFP_CST_PIS = 8 THEN '08'
                                    WHEN IT.NFP_CST_PIS = 9 THEN '09'
                                    WHEN IT.NFP_CST_PIS = 10 THEN '49'
                                    WHEN IT.NFP_CST_PIS = 11 THEN '50'
                                    WHEN IT.NFP_CST_PIS = 12 THEN '51'
                                    WHEN IT.NFP_CST_PIS = 13 THEN '52'
                                    WHEN IT.NFP_CST_PIS = 14 THEN '53'
                                    WHEN IT.NFP_CST_PIS = 15 THEN '54'
                                    WHEN IT.NFP_CST_PIS = 16 THEN '55'
                                    WHEN IT.NFP_CST_PIS = 17 THEN '56'
                                    WHEN IT.NFP_CST_PIS = 18 THEN '60'
                                    WHEN IT.NFP_CST_PIS = 19 THEN '61'
                                    WHEN IT.NFP_CST_PIS = 20 THEN '62'
                                    WHEN IT.NFP_CST_PIS = 21 THEN '63'
                                    WHEN IT.NFP_CST_PIS = 22 THEN '64'
                                    WHEN IT.NFP_CST_PIS = 23 THEN '65'
                                    WHEN IT.NFP_CST_PIS = 24 THEN '66'
                                    WHEN IT.NFP_CST_PIS = 25 THEN '67'
                                    WHEN IT.NFP_CST_PIS = 26 THEN '70'
                                    WHEN IT.NFP_CST_PIS = 27 THEN '71'
                                    WHEN IT.NFP_CST_PIS = 28 THEN '72'
                                    WHEN IT.NFP_CST_PIS = 29 THEN '73'
                                    WHEN IT.NFP_CST_PIS = 30 THEN '74'
                                    WHEN IT.NFP_CST_PIS = 31 THEN '75'
                                    WHEN IT.NFP_CST_PIS = 32 THEN '98'
                                    WHEN IT.NFP_CST_PIS = 33 THEN '99'
	                                ELSE ''
                                END PIS,
                                P.PRO_COD_NCM NCM,
                                'Saída' DescricaoTipo,
                                N.NFI_NUMERO Numero,
                                P.PRO_UNIDADE_MEDIDA UnidadeMedida,
                                SUM(IT.NFP_QUANTIDADE) Quantidade, 
                                SUM(IT.NFP_VALOR_UNITARIO) ValorUnitario, 
                                SUM(IT.NFP_VALOR_TOTAL) Valor

                                FROM T_NOTA_FISCAL_PRODUTOS IT
                                JOIN T_NOTA_FISCAL N ON N.NFI_CODIGO = IT.NFI_CODIGO
                                JOIN T_CLIENTE C ON C.CLI_CGCCPF = N.CLI_CGCCPF
                                JOIN T_LOCALIDADES L ON L.LOC_CODIGO = C.LOC_CODIGO
                                JOIN T_PRODUTO P ON P.PRO_CODIGO = IT.PRO_CODIGO
                                JOIN T_EMPRESA E ON E.EMP_CODIGO = N.EMP_CODIGO
                                WHERE 1 = 1 " + queryParameters + querySaida + @"

                                GROUP BY IT.PRO_CODIGO, P.PRO_DESCRICAO, IT.NFP_CST_COFINS, IT.NFP_CST_PIS, P.PRO_COD_NCM, N.NFI_NUMERO, P.PRO_UNIDADE_MEDIDA ";
            }

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

            if (paginar)
                query += " OFFSET " + inicioRegistros + " ROWS FETCH FIRST " + maximoRegistros + " ROWS ONLY";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.NFe.CompraVendaNCM)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.NFe.CompraVendaNCM>();
        }

        public int ContarRelatorioCompraVendaNCM(int codigoEmpresa, int codigoProduto, int codigoCidade, string ncms, string estado, TipoCompraVenda tipoMovimento, DateTime dataInicial, DateTime dataFinal, Dominio.Enumeradores.TipoAmbiente tipoAmbiente)
        {
            string query = "", queryParameters = "", queryEntrada = "", querySaida = "";
            if (codigoEmpresa > 0)
                queryParameters += " AND E.EMP_CODIGO = " + codigoEmpresa.ToString();

            if (codigoProduto > 0)
                queryParameters += " AND IT.PRO_CODIGO = " + codigoProduto.ToString();

            if (codigoCidade > 0)
                queryParameters += " AND C.LOC_CODIGO = " + codigoCidade.ToString();

            if (!string.IsNullOrWhiteSpace(ncms))
                queryParameters += " AND P.PRO_COD_NCM IN (" + ncms + ")";

            if (!string.IsNullOrWhiteSpace(estado))
                queryParameters += " AND L.UF_SIGLA = '" + estado + "'";

            querySaida += " AND N.NFI_AMBIENTE = " + (int)tipoAmbiente;

            if (dataInicial != DateTime.MinValue)
            {
                querySaida += " AND N.NFI_DATA_EMISSAO >= '" + dataInicial.ToString("MM/dd/yyyy") + "'";
                queryEntrada += " AND N.TDE_DATA_EMISSAO >= '" + dataInicial.ToString("MM/dd/yyyy") + "'";
            }

            if (dataFinal != DateTime.MinValue)
            {
                querySaida += " AND N.NFI_DATA_EMISSAO <= '" + dataFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";
                queryEntrada += " AND N.TDE_DATA_EMISSAO <= '" + dataFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";
            }

            if ((int)tipoMovimento == 1) //Compra
            {
                query = @"   SELECT COUNT(0) as CONTADOR FROM(

                                SELECT IT.PRO_CODIGO CodigoProduto, 
                                P.PRO_DESCRICAO Produto,
                                IT.TDI_CST_COFINS COFINS,
                                IT.TDI_CST_PIS PIS,
                                P.PRO_COD_NCM NCM,
                                'Entrada' DescricaoTipo,
                                N.TDE_NUMERO_LONG Numero,
                                '' UnidadeMedida,
                                SUM(IT.TDI_QUANTIDADE) Quantidade, 
                                SUM(IT.TDI_VALOR_UNITARIO) ValorUnitario, 
                                SUM(IT.TDI_VALOR_TOTAL) Valor
	
                                FROM T_TMS_DOCUMENTO_ENTRADA_ITEM IT
                                JOIN T_TMS_DOCUMENTO_ENTRADA N ON N.TDE_CODIGO = IT.TDE_CODIGO
                                JOIN T_CLIENTE C ON C.CLI_CGCCPF = N.CLI_CGCCPF
                                JOIN T_LOCALIDADES L ON L.LOC_CODIGO = C.LOC_CODIGO
                                JOIN T_PRODUTO P ON P.PRO_CODIGO = IT.PRO_CODIGO
                                JOIN T_EMPRESA E ON E.EMP_CODIGO = N.EMP_CODIGO
                                WHERE 1 = 1 " + queryParameters + queryEntrada + @"
                                GROUP BY IT.PRO_CODIGO, P.PRO_DESCRICAO, IT.TDI_CST_COFINS, IT.TDI_CST_PIS, P.PRO_COD_NCM, N.TDE_NUMERO_LONG, P.PRO_UNIDADE_MEDIDA ) AS T ";
            }
            else if ((int)tipoMovimento == 2) //Venda
            {
                query = @"   SELECT COUNT(0) as CONTADOR FROM(

                                SELECT IT.PRO_CODIGO CodigoProduto, 
                                P.PRO_DESCRICAO Produto,
                                '' COFINS,
                                '' PIS,
                                P.PRO_COD_NCM NCM,
                                'Saída' DescricaoTipo,
                                N.NFI_NUMERO Numero,
                                '' UnidadeMedida,
                                SUM(IT.NFP_QUANTIDADE) Quantidade, 
                                SUM(IT.NFP_VALOR_UNITARIO) ValorUnitario, 
                                SUM(IT.NFP_VALOR_TOTAL) Valor

                                FROM T_NOTA_FISCAL_PRODUTOS IT
                                JOIN T_NOTA_FISCAL N ON N.NFI_CODIGO = IT.NFI_CODIGO
                                JOIN T_CLIENTE C ON C.CLI_CGCCPF = N.CLI_CGCCPF
                                JOIN T_LOCALIDADES L ON L.LOC_CODIGO = C.LOC_CODIGO
                                JOIN T_PRODUTO P ON P.PRO_CODIGO = IT.PRO_CODIGO
                                JOIN T_EMPRESA E ON E.EMP_CODIGO = N.EMP_CODIGO
                                WHERE 1 = 1 " + queryParameters + querySaida + @"

                                GROUP BY IT.PRO_CODIGO, P.PRO_DESCRICAO, IT.NFP_CST_COFINS, IT.NFP_CST_PIS, P.PRO_COD_NCM, N.NFI_NUMERO, P.PRO_UNIDADE_MEDIDA ) AS T ";
            }
            else
            {
                query = @"   SELECT COUNT(0) as CONTADOR FROM(

                                SELECT IT.PRO_CODIGO CodigoProduto, 
                                P.PRO_DESCRICAO Produto,
                                IT.TDI_CST_COFINS COFINS,
                                IT.TDI_CST_PIS PIS,
                                P.PRO_COD_NCM NCM,
                                'Entrada' DescricaoTipo,    
                                N.TDE_NUMERO_LONG Numero,
                                '' UnidadeMedida,
                                SUM(IT.TDI_QUANTIDADE) Quantidade, 
                                SUM(IT.TDI_VALOR_UNITARIO) ValorUnitario, 
                                SUM(IT.TDI_VALOR_TOTAL) Valor
	
                                FROM T_TMS_DOCUMENTO_ENTRADA_ITEM IT
                                JOIN T_TMS_DOCUMENTO_ENTRADA N ON N.TDE_CODIGO = IT.TDE_CODIGO
                                JOIN T_CLIENTE C ON C.CLI_CGCCPF = N.CLI_CGCCPF
                                JOIN T_LOCALIDADES L ON L.LOC_CODIGO = C.LOC_CODIGO
                                JOIN T_PRODUTO P ON P.PRO_CODIGO = IT.PRO_CODIGO
                                JOIN T_EMPRESA E ON E.EMP_CODIGO = N.EMP_CODIGO
                                WHERE 1 = 1 " + queryParameters + queryEntrada + @"
                                GROUP BY IT.PRO_CODIGO, P.PRO_DESCRICAO, IT.TDI_CST_COFINS, IT.TDI_CST_PIS, P.PRO_COD_NCM, N.TDE_NUMERO_LONG, P.PRO_UNIDADE_MEDIDA

                                UNION ALL

                                SELECT IT.PRO_CODIGO CodigoProduto, 
                                P.PRO_DESCRICAO Produto,
                                '' COFINS,
                                '' PIS,
                                P.PRO_COD_NCM NCM,
                                'Saída' DescricaoTipo,
                                N.NFI_NUMERO Numero,
                                '' UnidadeMedida,
                                SUM(IT.NFP_QUANTIDADE) Quantidade, 
                                SUM(IT.NFP_VALOR_UNITARIO) ValorUnitario, 
                                SUM(IT.NFP_VALOR_TOTAL) Valor

                                FROM T_NOTA_FISCAL_PRODUTOS IT
                                JOIN T_NOTA_FISCAL N ON N.NFI_CODIGO = IT.NFI_CODIGO
                                JOIN T_CLIENTE C ON C.CLI_CGCCPF = N.CLI_CGCCPF
                                JOIN T_LOCALIDADES L ON L.LOC_CODIGO = C.LOC_CODIGO
                                JOIN T_PRODUTO P ON P.PRO_CODIGO = IT.PRO_CODIGO
                                JOIN T_EMPRESA E ON E.EMP_CODIGO = N.EMP_CODIGO
                                WHERE 1 = 1 " + queryParameters + querySaida + @"

                                GROUP BY IT.PRO_CODIGO, P.PRO_DESCRICAO, IT.NFP_CST_COFINS, IT.NFP_CST_PIS, P.PRO_COD_NCM, N.NFI_NUMERO, P.PRO_UNIDADE_MEDIDA ) AS T ";
            }

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.NFe.ProdutoSemMovimentacao> RelatorioProdutoSemMovimentacao(int codigoEmpresa, int codigoProduto, bool estoqueDiferenteZero, DateTime dataInicial, DateTime dataFinal, Dominio.Enumeradores.TipoAmbiente? tipoAmbiente, int codigoGrupoProduto, string propGrupo, string dirOrdenacaoGrupo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, bool paginar = true, bool todosCNPJdaRaizEmbarcador = false)
        {
            string query = "", queryEmpresa = "", queryParameters = "", queryEntrada = "", querySaida = "", queryAmbiente = "";
            if (codigoEmpresa > 0)
                queryEmpresa += " AND E.EMP_CODIGO = " + codigoEmpresa.ToString();

            if (codigoProduto > 0)
                queryParameters += " AND P.PRO_CODIGO = " + codigoProduto.ToString();

            if (estoqueDiferenteZero)
                queryParameters += " AND PE.PRE_QUANTIDADE <> 0 ";

            if (tipoAmbiente != null && tipoAmbiente.HasValue)
                queryAmbiente += " AND N.NFI_AMBIENTE = " + (int)tipoAmbiente;

            if (dataInicial != DateTime.MinValue)
            {
                querySaida += " AND N.NFI_DATA_EMISSAO >= '" + dataInicial.ToString("MM/dd/yyyy") + "'";
                queryEntrada += " AND N.TDE_DATA_EMISSAO >= '" + dataInicial.ToString("MM/dd/yyyy") + "'";
            }

            if (dataFinal != DateTime.MinValue)
            {
                querySaida += " AND N.NFI_DATA_EMISSAO <= '" + dataFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";
                queryEntrada += " AND N.TDE_DATA_EMISSAO <= '" + dataFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";
            }

            if (codigoGrupoProduto > 0)
                queryParameters += " AND G.GPR_CODIGO = " + codigoGrupoProduto.ToString();

            query = @"   SELECT P.PRO_CODIGO CodigoProduto, 
                            P.PRO_DESCRICAO Produto,
                            ISNULL(PE.PRE_QUANTIDADE, 0) Estoque,
                            P.PRO_VALOR_VENDA Preco,
                            P.PRO_CUSTO_MEDIO CustoMedio,
                            CASE
	                            WHEN P.PRO_STATUS = 'I' THEN 'Inativo'
	                            ELSE 'Ativo'
                            END	DescricaoStatus,

                            (SELECT TOP 1 N.TDE_DATA_EMISSAO
                            FROM T_TMS_DOCUMENTO_ENTRADA N 
                            JOIN T_TMS_DOCUMENTO_ENTRADA_ITEM IT ON N.TDE_CODIGO = IT.TDE_CODIGO
                            JOIN T_EMPRESA E ON E.EMP_CODIGO = N.EMP_CODIGO
                            WHERE 1 = 1 AND IT.PRO_CODIGO = P.PRO_CODIGO " + queryEmpresa + @"
                            ORDER BY N.TDE_DATA_EMISSAO DESC) DataUltimaCompra,

                            (SELECT TOP 1 N.NFI_DATA_EMISSAO
                            FROM T_NOTA_FISCAL N 
                            JOIN T_NOTA_FISCAL_PRODUTOS IT ON N.NFI_CODIGO = IT.NFI_CODIGO
                            JOIN T_EMPRESA E ON E.EMP_CODIGO = N.EMP_CODIGO
                            WHERE 1 = 1 AND IT.PRO_CODIGO = P.PRO_CODIGO " + queryEmpresa + queryAmbiente + @"
                            ORDER BY N.NFI_DATA_EMISSAO DESC) DataUltimaVenda, 
                            E.EMP_RAZAO Empresa,
                            GRP_DESCRICAO GrupoProduto

                            FROM T_PRODUTO P
                            LEFT OUTER JOIN T_PRODUTO_ESTOQUE PE ON PE.PRO_CODIGO = P.PRO_CODIGO
                            LEFT OUTER JOIN T_EMPRESA E ON E.EMP_CODIGO = PE.EMP_CODIGO
                            LEFT OUTER JOIN T_GRUPO_PRODUTO_TMS G ON G.GPR_CODIGO = P.GPR_CODIGO
                            WHERE 1 = 1 " + queryEmpresa + queryParameters + @" 
                            AND P.PRO_CODIGO NOT IN (SELECT DISTINCT PRO_CODIGO FROM(

                            SELECT IT.PRO_CODIGO
                            FROM T_TMS_DOCUMENTO_ENTRADA_ITEM IT
                            JOIN T_TMS_DOCUMENTO_ENTRADA N ON N.TDE_CODIGO = IT.TDE_CODIGO
                            JOIN T_PRODUTO P ON P.PRO_CODIGO = IT.PRO_CODIGO
                            JOIN T_EMPRESA E ON E.EMP_CODIGO = N.EMP_CODIGO
                            WHERE 1 = 1 " + queryEmpresa + queryEntrada + @"

                            UNION ALL

                            SELECT IT.PRO_CODIGO
                            FROM T_NOTA_FISCAL_PRODUTOS IT
                            JOIN T_NOTA_FISCAL N  ON N.NFI_CODIGO = IT.NFI_CODIGO
                            JOIN T_PRODUTO P ON P.PRO_CODIGO = IT.PRO_CODIGO
                            JOIN T_EMPRESA E ON E.EMP_CODIGO = N.EMP_CODIGO
                            WHERE 1 = 1 " + queryEmpresa + querySaida + queryAmbiente + @") AS T) ";

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

            if (paginar)
                query += " OFFSET " + inicioRegistros + " ROWS FETCH FIRST " + maximoRegistros + " ROWS ONLY";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.NFe.ProdutoSemMovimentacao)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.NFe.ProdutoSemMovimentacao>();
        }

        public int ContarRelatorioProdutoSemMovimentacao(int codigoEmpresa, int codigoProduto, bool estoqueDiferenteZero, DateTime dataInicial, DateTime dataFinal, Dominio.Enumeradores.TipoAmbiente? tipoAmbiente, int codigoGrupoProduto)
        {
            string query = "", queryEmpresa = "", queryParameters = "", queryEntrada = "", querySaida = "";
            if (codigoEmpresa > 0)
                queryEmpresa += " AND E.EMP_CODIGO = " + codigoEmpresa.ToString();

            if (codigoProduto > 0)
                queryParameters += " AND P.PRO_CODIGO = " + codigoProduto.ToString();

            if (estoqueDiferenteZero)
                queryParameters += " AND PE.PRE_QUANTIDADE <> 0 ";

            if (tipoAmbiente != null && tipoAmbiente.HasValue)
                querySaida += " AND N.NFI_AMBIENTE = " + (int)tipoAmbiente;

            if (dataInicial != DateTime.MinValue)
            {
                querySaida += " AND N.NFI_DATA_EMISSAO >= '" + dataInicial.ToString("MM/dd/yyyy") + "'";
                queryEntrada += " AND N.TDE_DATA_EMISSAO >= '" + dataInicial.ToString("MM/dd/yyyy") + "'";
            }

            if (dataFinal != DateTime.MinValue)
            {
                querySaida += " AND N.NFI_DATA_EMISSAO <= '" + dataFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";
                queryEntrada += " AND N.TDE_DATA_EMISSAO <= '" + dataFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";
            }

            if (codigoGrupoProduto > 0)
                queryParameters += " AND G.GPR_CODIGO = " + codigoGrupoProduto.ToString();

            query = @"   SELECT COUNT(0) as CONTADOR FROM(

                            SELECT P.PRO_CODIGO CodigoProduto

                            FROM T_PRODUTO P
                            LEFT OUTER JOIN T_PRODUTO_ESTOQUE PE ON PE.PRO_CODIGO = P.PRO_CODIGO
                            LEFT OUTER JOIN T_EMPRESA E ON E.EMP_CODIGO = PE.EMP_CODIGO
                            LEFT OUTER JOIN T_GRUPO_PRODUTO_TMS G ON G.GPR_CODIGO = P.GPR_CODIGO
                            WHERE 1 = 1 " + queryEmpresa + queryParameters + @" 
                            AND P.PRO_CODIGO NOT IN (SELECT DISTINCT PRO_CODIGO FROM(

                            SELECT IT.PRO_CODIGO
                            FROM T_TMS_DOCUMENTO_ENTRADA_ITEM IT
                            JOIN T_TMS_DOCUMENTO_ENTRADA N ON N.TDE_CODIGO = IT.TDE_CODIGO
                            JOIN T_PRODUTO P ON P.PRO_CODIGO = IT.PRO_CODIGO
                            JOIN T_EMPRESA E ON E.EMP_CODIGO = N.EMP_CODIGO
                            WHERE 1 = 1 " + queryEmpresa + queryEntrada + @"

                            UNION ALL

                            SELECT IT.PRO_CODIGO
                            FROM T_NOTA_FISCAL_PRODUTOS IT
                            JOIN T_NOTA_FISCAL N  ON N.NFI_CODIGO = IT.NFI_CODIGO
                            JOIN T_PRODUTO P ON P.PRO_CODIGO = IT.PRO_CODIGO
                            JOIN T_EMPRESA E ON E.EMP_CODIGO = N.EMP_CODIGO
                            WHERE 1 = 1 " + queryEmpresa + querySaida + @") AS T)) AS TT ";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.NFe.VendasReduzidas> RelatorioVendasReduzidas(int codigoEmpresa, int codigoProduto, DateTime dataInicial, DateTime dataFinal, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, string propGrupo, string dirOrdenacaoGrupo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, bool paginar = true, bool todosCNPJdaRaizEmbarcador = false)
        {
            string query = "", queryEmpresa = "", queryParameters = "";
            if (codigoEmpresa > 0)
                queryEmpresa += " AND P.EMP_CODIGO = " + codigoEmpresa.ToString();

            if (dataInicial != DateTime.MinValue)
                queryParameters += " AND N.NFI_DATA_EMISSAO >= '" + dataInicial.ToString("MM/dd/yyyy") + "'";

            if (dataFinal != DateTime.MinValue)
                queryParameters += " AND N.NFI_DATA_EMISSAO <= '" + dataFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";

            queryParameters += " AND N.NFI_AMBIENTE = " + (int)tipoAmbiente;

            query = @"   SELECT P.PRO_CODIGO CodigoProduto, 
                                P.PRO_DESCRICAO Produto,
                                CASE 
	                                WHEN T.Mes = 1 THEN 'Janeiro'
	                                WHEN T.Mes = 2 THEN 'Fevereiro'
	                                WHEN T.Mes = 3 THEN 'Março'
	                                WHEN T.Mes = 4 THEN 'Abril'
	                                WHEN T.Mes = 5 THEN 'Maio'
	                                WHEN T.Mes = 6 THEN 'Junho'
	                                WHEN T.Mes = 7 THEN 'Julho'
	                                WHEN T.Mes = 8 THEN 'Agosto'
	                                WHEN T.Mes = 9 THEN 'Setembro'
	                                WHEN T.Mes = 10 THEN 'Outubro'
	                                WHEN T.Mes = 11 THEN 'Novembro'
	                                WHEN T.Mes = 12 THEN 'Dezembro'
	                                ELSE 'Indefinido'
                                END Mes,
                                T.Ano,

                                ISNULL((SELECT SUM(I.NFP_QUANTIDADE) QTD
                                FROM T_NOTA_FISCAL_PRODUTOS I
                                JOIN T_NOTA_FISCAL N ON N.NFI_CODIGO = I.NFI_CODIGO
                                WHERE I.PRO_CODIGO = P.PRO_CODIGO AND MONTH(N.NFI_DATA_EMISSAO) = T.MES AND YEAR(N.NFI_DATA_EMISSAO) = T.ano " + queryEmpresa + queryParameters + @"
                                GROUP BY I.PRO_CODIGO, MONTH(N.NFI_DATA_EMISSAO), YEAR(N.NFI_DATA_EMISSAO)), 0) Quantidade,

                                ISNULL((SELECT AVG(I.NFP_VALOR_UNITARIO) QTD
                                FROM T_NOTA_FISCAL_PRODUTOS I
                                JOIN T_NOTA_FISCAL N ON N.NFI_CODIGO = I.NFI_CODIGO
                                WHERE I.PRO_CODIGO = P.PRO_CODIGO AND MONTH(N.NFI_DATA_EMISSAO) = T.MES AND YEAR(N.NFI_DATA_EMISSAO) = T.ano " + queryEmpresa + queryParameters + @"
                                GROUP BY I.PRO_CODIGO, MONTH(N.NFI_DATA_EMISSAO), YEAR(N.NFI_DATA_EMISSAO)), 0) MediaValorUnitario,

                                ISNULL((SELECT SUM(I.NFP_VALOR_TOTAL) QTD
                                FROM T_NOTA_FISCAL_PRODUTOS I
                                JOIN T_NOTA_FISCAL N ON N.NFI_CODIGO = I.NFI_CODIGO
                                WHERE I.PRO_CODIGO = P.PRO_CODIGO AND MONTH(N.NFI_DATA_EMISSAO) = T.MES AND YEAR(N.NFI_DATA_EMISSAO) = T.ano " + queryEmpresa + queryParameters + @"
                                GROUP BY I.PRO_CODIGO, MONTH(N.NFI_DATA_EMISSAO), YEAR(N.NFI_DATA_EMISSAO)), 0) Valor

                                FROM T_PRODUTO P,
                                (SELECT Meses.mes, anos.ano FROM 
                                (SELECT 1 mes UNION ALL SELECT 2 UNION ALL SELECT 3 UNION ALL SELECT 4
                                UNION ALL SELECT 5 UNION ALL SELECT 6 UNION ALL SELECT 7 UNION ALL SELECT 8
                                UNION ALL SELECT 9 UNION ALL SELECT 10 UNION ALL SELECT 11 UNION ALL SELECT 12) Meses,
                                (SELECT YEAR(GETDATE())-2 ano UNION ALL SELECT YEAR(GETDATE()) -1 UNION ALL SELECT YEAR(GETDATE())) anos) AS T
                                WHERE 1 = 1 " + queryEmpresa;

            if (codigoProduto > 0)
                query += " AND P.PRO_CODIGO = " + codigoProduto.ToString();

            if (dataInicial != DateTime.MinValue)
            {
                query += " AND T.Mes >= MONTH('" + dataInicial.ToString("MM/dd/yyyy") + "')";
                query += " AND T.Ano >= YEAR('" + dataInicial.ToString("MM/dd/yyyy") + "')";
            }

            if (dataFinal != DateTime.MinValue)
            {
                query += " AND T.Mes <= MONTH('" + dataFinal.AddDays(1).ToString("MM/dd/yyyy") + "')";
                query += " AND T.Ano <= YEAR('" + dataFinal.AddDays(1).ToString("MM/dd/yyyy") + "')";
            }

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

            if (paginar)
                query += " OFFSET " + inicioRegistros + " ROWS FETCH FIRST " + maximoRegistros + " ROWS ONLY";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.NFe.VendasReduzidas)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.NFe.VendasReduzidas>();
        }

        public int ContarRelatorioVendasReduzidas(int codigoEmpresa, int codigoProduto, DateTime dataInicial, DateTime dataFinal, Dominio.Enumeradores.TipoAmbiente tipoAmbiente)
        {
            string query = "", queryEmpresa = "";
            if (codigoEmpresa > 0)
                queryEmpresa += " AND P.EMP_CODIGO = " + codigoEmpresa.ToString();

            query = @"   SELECT COUNT(0) as CONTADOR
                                FROM T_PRODUTO P,
                                (SELECT Meses.mes, anos.ano FROM 
                                (SELECT 1 mes UNION ALL SELECT 2 UNION ALL SELECT 3 UNION ALL SELECT 4
                                UNION ALL SELECT 5 UNION ALL SELECT 6 UNION ALL SELECT 7 UNION ALL SELECT 8
                                UNION ALL SELECT 9 UNION ALL SELECT 10 UNION ALL SELECT 11 UNION ALL SELECT 12) Meses,
                                (SELECT YEAR(GETDATE())-2 ano UNION ALL SELECT YEAR(GETDATE()) -1 UNION ALL SELECT YEAR(GETDATE())) anos) AS T
                                WHERE 1 = 1 " + queryEmpresa;

            if (codigoProduto > 0)
                query += " AND P.PRO_CODIGO = " + codigoProduto.ToString();

            if (dataInicial != DateTime.MinValue)
            {
                query += " AND T.Mes >= MONTH('" + dataInicial.ToString("MM/dd/yyyy") + "')";
                query += " AND T.Ano >= YEAR('" + dataInicial.ToString("MM/dd/yyyy") + "')";
            }

            if (dataFinal != DateTime.MinValue)
            {
                query += " AND T.Mes <= MONTH('" + dataFinal.AddDays(1).ToString("MM/dd/yyyy") + "')";
                query += " AND T.Ano <= YEAR('" + dataFinal.AddDays(1).ToString("MM/dd/yyyy") + "')";
            }

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.NFe.NotasDetalhadas> RelatorioNotasDetalhadas(Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNotasDetalhadas filtrosPesquisa, string propGrupo, string dirOrdenacaoGrupo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, bool paginar = true)
        {
            string query = "", queryParameters = "", queryEntrada = "", querySaida = "";
            if (filtrosPesquisa.CodigoEmpresa > 0)
                queryParameters += " AND E.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresa.ToString();

            if (filtrosPesquisa.CnpjPessoa > 0)
                queryParameters += " AND C.CLI_CGCCPF = " + filtrosPesquisa.CnpjPessoa.ToString();

            if (filtrosPesquisa.CodigosNaturezaOperacao?.Count > 0)
                queryParameters += $" AND NO.NAT_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosNaturezaOperacao)})";

            if (filtrosPesquisa.CodigoProduto > 0)
                queryParameters += " AND IT.PRO_CODIGO = " + filtrosPesquisa.CodigoProduto.ToString();

            if (filtrosPesquisa.CodigosGrupoProduto != null && filtrosPesquisa.CodigosGrupoProduto.Count > 0)
                queryParameters += $" AND G.GPR_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosGrupoProduto)})";

            if (filtrosPesquisa.CodigoGrupoPessoa > 0)
                queryParameters += " AND C.GRP_CODIGO = " + filtrosPesquisa.CodigoGrupoPessoa.ToString();

            queryEntrada = @"SELECT N.TDE_NUMERO_LONG Numero,
                                N.TDE_SERIE Serie,
                                'Entrada' DescricaoTipo,
                                N.TDE_DATA_EMISSAO DataEmissao,
                                N.TDE_DATA_ENTRADA DataEntrada,
                                ISNULL(FORMAT(N.TDE_DATA_FINALIZACAO,'dd/MM/yyyy'),'') DataFinalizacao,
                                C.CLI_NOME Pessoa,
                                M.MOD_NUM Modelo,
                                OPL.FUN_NOME OperadorLancamentoDocumento,
                                OPF.FUN_NOME OperadorFinalizaDocumento,
                                NO.NAT_DESCRICAO NaturezaOperacao,
                                CASE
                                    WHEN N.TDE_CHAVE = '' OR N.TDE_CHAVE IS NULL THEN CAST(N.TDE_CODIGO AS VARCHAR(50))
                                    ELSE N.TDE_CHAVE
                                END Chave,
                                CASE
	                                WHEN N.TDE_SITUACAO = 2 THEN 'Cancelado'
	                                WHEN N.TDE_SITUACAO = 3 THEN 'Finalizado'
                                    WHEN N.TDE_SITUACAO = 4 THEN 'Anulado'
	                                ELSE 'Aberto'
                                END DescricaoStatus,
                                N.TDE_VALOR_TOTAL ValorTotal,

                                IT.PRO_CODIGO CodigoProduto, 
                                P.PRO_DESCRICAO Produto,
                                P.PRO_UNIDADE_MEDIDA UnidadeMedida,
                                CAST(CF.CFO_CFOP AS VARCHAR(20)) + '.' + ISNULL(CF.CFO_EXTENSAO, '') CFOP,
                                ISNULL(CF.CFO_DESCRICAO, 'CFOP SEM DESCRICAO') DescricaoCFOP,
                                IT.TDI_QUANTIDADE Quantidade,
                                IT.TDI_VALOR_UNITARIO ValorUnitario,
                                IT.TDI_DESCONTO Desconto,
                                IT.TDI_VALOR_TOTAL Valor,

                                ((IT.TDI_VALOR_TOTAL - ISNULL(TDI_DESCONTO, 0)) / CASE WHEN IT.TDI_QUANTIDADE > 0 THEN IT.TDI_QUANTIDADE ELSE 1 END) ValorUnitarioLiquido,
                                (IT.TDI_VALOR_TOTAL - ISNULL(TDI_DESCONTO, 0)) ValorLiquido,
    
                                IT.TDI_CST CstICMS,
                                IT.TDI_BASE_CALCULO_ICMS BaseICMS,
                                IT.TDI_ALIQUOTA_ICMS AliquotaICMS,
                                IT.TDI_VALOR_ICMS ValorICMS,
                                IT.TDI_BASE_ICMS_FORNECEDOR BaseCalculoICMSFornecedor,
								IT.TDI_ALIQUOTA_ICMS_FORNECEDOR AliquotaICMSFornecedor,
								IT.TDI_CST_FORNECEDOR CstIcmsFornecedor,
								IT.TDI_CFOP_FORNECEDOR CfopFornecedor,
								IT.TDI_VALOR_ICMS_FORNECEDOR ValorICMSFornecedor,


                                IT.TDI_BASE_CALCULO_ICMS_ST BaseICMSST,
                                0.00 MVA,
                                IT.TDI_ALIQUOTA_ICMS_ST AliquotaICMSST,
                                IT.TDI_VALOR_ICMS_ST ValorICMSST,

                                IT.TDI_CST_PIS CstPIS,
                                IT.TDI_BASE_CALCULO_PIS BasePIS,
                                IT.TDI_ALIQUOTA_PIS AliquotaPIS,
                                IT.TDI_VALOR_PIS ValorPIS,

                                IT.TDI_CST_COFINS CstCOFINS,
                                IT.TDI_BASE_CALCULO_COFINS BaseCOFINS,
                                IT.TDI_ALIQUOTA_COFINS AliquotaCOFINS,
                                IT.TDI_VALOR_COFINS ValorCOFINS,

                                IT.TDI_CST_IPI CstIPI,
                                IT.TDI_BASE_CALCULO_IPI BaseIPI,
                                IT.TDI_ALIQUOTA_IPI AliquotaIPI,
                                IT.TDI_VALOR_IPI ValorIPI,

                                IT.TDI_VALOR_RETENCAO_PIS RetencaoPIS,
                                IT.TDI_VALOR_RETENCAO_COFINS RetencaoCOFNIS,
                                IT.TDI_VALOR_RETENCAO_INSS RetencaoINSS,
                                IT.TDE_VALOR_RETENCAO_IPI RetencaoIPI,
                                IT.TDE_VALOR_RETENCAO_CSLL RetencaoCSLL,
                                IT.TDE_VALOR_RETENCAO_OUTRAS RetencaoOUTRAS,
                                IT.TDE_VALOR_RETENCAO_IR RetencaoIR,
                                IT.TDE_VALOR_RETENCAO_ISS RetencaoISS,
                                IT.TDI_KM_ABASTECIMENTO KmAbastecimento,
                                IT.TDI_HORIMETRO Horimetro,

                                IT.TDI_OUTRAS_DESPESAS OutrasDespesas,
								IT.TDI_VALOR_FRETE ValorFrete, 
								IT.TDI_VALOR_SEGURO ValorSeguro, 
								IT.TDI_VALOR_DIFERENCIAL ValorDiferencial,

								V.VEI_PLACA Veiculo,
								V.VEI_CODIGO CodigoVeiculo,
								E.EMP_RAZAO Empresa,
								E.EMP_CODIGO CodigoEmpresa,

								CAST(SUBSTRING((SELECT DISTINCT ', ' + 
                                CASE 
	                                WHEN T.TIT_STATUS = 1 THEN 'Aberto' 
	                                WHEN T.TIT_VALOR_PAGO < T.TIT_VALOR_ORIGINAL THEN 
		                                CASE
			                                WHEN ISNULL((SELECT COUNT(1) FROM T_TITULO TT WHERE TT.TDD_CODIGO = T.TDD_CODIGO AND TT.TIT_STATUS <> 3 AND TT.TIT_STATUS <> 4), 0) >= 1 THEN 'Renegociado' 
                                            WHEN ISNULL((SELECT COUNT(1) FROM T_TITULO_BAIXA_AGRUPADO TBA JOIN T_TITULO_BAIXA_NEGOCIACAO TBN ON TBN.TIB_CODIGO = TBA.TIB_CODIGO JOIN T_TITULO TT ON TT.TBN_CODIGO = TBN.TBN_CODIGO WHERE TT.TIT_STATUS <> 3 AND TT.TIT_STATUS <> 4 AND TBA.TIT_CODIGO = T.TIT_CODIGO), 0) >= 1 THEN 'Renegociado' 
                                            
                                            WHEN ISNULL(
                                                        (SELECT COUNT(1)
                                                        FROM T_TITULO_BAIXA_NEGOCIACAO TBN
                                                        JOIN T_TITULO TT ON TT.TBN_CODIGO = TBN.TBN_CODIGO
                                                        WHERE TT.TIT_STATUS <> 3 AND TT.TIT_STATUS <> 4
                                                            AND TBN.TIB_CODIGO IN
                                                            (SELECT tituloBaixa.TIB_CODIGO
                                                                FROM T_TITULO_BAIXA tituloBaixa
                                                                JOIN T_TITULO_BAIXA_AGRUPADO TBA ON TBA.TIB_CODIGO = tituloBaixa.TIB_CODIGO
                                                                WHERE tituloBaixa.TIB_SITUACAO <> 4 AND TBA.TIT_CODIGO = T.TIT_CODIGO)), 0) >= 1 THEN 'Renegociado'
                                            WHEN ISNULL(
                                                        (SELECT COUNT(1)
                                                        FROM T_TITULO_BAIXA_NEGOCIACAO TBNN
                                                        JOIN T_TITULO TTT ON TTT.TBN_CODIGO = TBNN.TBN_CODIGO
                                                        WHERE TTT.TIT_STATUS <> 3 AND TTT.TIT_STATUS <> 4
                                                            AND TBNN.TIB_CODIGO IN
                                                            (SELECT tituloBaixa2.TIB_CODIGO
                                                                FROM T_TITULO_BAIXA tituloBaixa2
                                                                JOIN T_TITULO_BAIXA_AGRUPADO TBAA ON TBAA.TIB_CODIGO = tituloBaixa2.TIB_CODIGO
                                                                WHERE tituloBaixa2.TIB_SITUACAO <> 4
                                                                AND TBAA.TIT_CODIGO IN
                                                                    (SELECT TT.TIT_CODIGO
                                                                    FROM T_TITULO_BAIXA_NEGOCIACAO TBN
                                                                    JOIN T_TITULO TT ON TT.TBN_CODIGO = TBN.TBN_CODIGO
                                                                    WHERE TBN.TIB_CODIGO IN
                                                                        (SELECT tituloBaixa.TIB_CODIGO
                                                                        FROM T_TITULO_BAIXA tituloBaixa
                                                                        JOIN T_TITULO_BAIXA_AGRUPADO TBA ON TBA.TIB_CODIGO = tituloBaixa.TIB_CODIGO
                                                                        WHERE tituloBaixa.TIB_SITUACAO <> 4 AND TBA.TIT_CODIGO = T.TIT_CODIGO)))), 0) >= 1 THEN 'Renegociado'

			                                ELSE 'Pago' 
		                                END
	                                ELSE 'Pago' 
                                END
                                FROM T_TITULO T
                                JOIN T_TMS_DOCUMENTO_ENTRADA_DUPLICATA D ON D.TDD_CODIGO = T.TDD_CODIGO
                                WHERE T.TIT_STATUS <> 4 AND D.TDE_CODIGO = N.TDE_CODIGO FOR XML PATH('')), 3, 1000) AS VARCHAR(2000)) SituacaoFinanceiraNota,

								CAST(SUBSTRING((SELECT DISTINCT ', ' +  CONVERT(VARCHAR, TT.TIT_DATA_VENCIMENTO, 103) 
								FROM T_TITULO TT
                                JOIN T_TMS_DOCUMENTO_ENTRADA_DUPLICATA D ON D.TDD_CODIGO = TT.TDD_CODIGO
								WHERE TT.TIT_STATUS <> 4 AND D.TDE_CODIGO = N.TDE_CODIGO FOR XML PATH('')), 3, 1000) AS VARCHAR(2000)) DataVencimento,
	
								CAST(SUBSTRING((SELECT DISTINCT ', ' +  CONVERT(VARCHAR, TT.TIT_DATA_LIQUIDACAO, 103) 
								FROM T_TITULO TT
								JOIN T_TMS_DOCUMENTO_ENTRADA_DUPLICATA D ON D.TDD_CODIGO = TT.TDD_CODIGO
								WHERE TT.TIT_STATUS = 3 AND D.TDE_CODIGO = N.TDE_CODIGO FOR XML PATH('')), 3, 1000) AS VARCHAR(2000)) DataPagamento,

								L.UF_SIGLA EstadoPessoa,
                                C.CLI_CGCCPF CPFCNPJPessoa,
								CASE
									WHEN V.VEI_TIPO = 'P' THEN 'Próprio'
									WHEN V.VEI_TIPO = 'T' THEN 'Terceiro'
									ELSE ''
								END TipoVeiculo,
                                C.CLI_FISJUR TipoCliente,
                                L.LOC_DESCRICAO Cidade,
                                ISNULL(N.TDE_BASE_ST_RETIDO, 0) BaseSTRetido,
                                ISNULL(N.TDE_VALOR_ST_RETIDO, 0) ValorSTRetido,
                                N.TDE_CODIGO CodigoNota,
                                GRP_DESCRICAO GrupoProduto, SEG.VSE_DESCRICAO Segmento, IT.TDI_VALOR_IMPOSTOS_FORA ValorImpostosFora, ISNULL(TM.TIM_DESCRICAO, 'NÃO INFORMADO') TipoMovimento,
                                EQP.EQP_DESCRICAO Equipamento,
                                C.CLI_REGIME_TRIBUTARIO RegimeTributario,
                                IT.TDI_UNIDADE_MEDIDA_FORNECEDOR UnidadeMedidaFornecedor,
                                IT.TDI_QUANTIDADE_FORNECEDOR QuantidadeFornecedor,
                                IT.TDI_VALOR_UNITARIO_FORNECEDOR ValorUnitarioFornecedor,
                                P.PRO_COD_PRODUTO ProdutoCodigoProduto,
                                Servico.SER_DESCRICAO Servico,
                                LocalidadePrestacaoServico.LOC_DESCRICAO LocalidadePrestacaoServico,
                                N.TDE_TIPO_DOCUMENTO TipoDocumento,
                                ISNULL(N.TDE_CST_SERVICO, -1) CSTServico,
                                ISNULL(N.TDE_ALIQUOTA_SIMPLES_NACIONAL, 0) AliquotaSimplesNacional,
                                N.TDE_DOCUMENTO_FISCAL_PROVENIENTE_SIMPLES_NACIONAL DocumentoFiscalProvenienteSimplesNacional,
                                N.TDE_TRIBUTA_ISS_NO_MUNICIPIO TributaISSNoMunicipio,
                                IT.TDI_VALOR_ABASTECIMENTO_TABELA_FORNECEDOR ValorAbastecimentoTabelaFornecedor,
                                IT.TDI_VALOR_ABASTECIMENTO_COM_DIVERGENCIA ValorAbastecimentoComDivergencia,
                                LA.LAP_DESCRICAO LocalArmazenamento,
                                N.TDE_DATA_ABASTECIMENTO DataAbastecimento,
                                E.EMP_CNPJ CNPJFilial,
                                N.ORC_CODIGO OrdemCompra,
                                N.OSE_CODIGO OrdemServico,
                                IT.TDI_VALOR_CUSTO_UNITARIO CustoUnitario,
                                IT.TDI_VALOR_CUSTO_TOTAL CustoTotal

                                FROM T_TMS_DOCUMENTO_ENTRADA_ITEM IT
                                JOIN T_TMS_DOCUMENTO_ENTRADA N ON N.TDE_CODIGO = IT.TDE_CODIGO
                                JOIN T_CLIENTE C ON C.CLI_CGCCPF = N.CLI_CGCCPF
								JOIN T_LOCALIDADES L ON L.LOC_CODIGO = C.LOC_CODIGO	
                                LEFT OUTER JOIN T_FUNCIONARIO OPL ON OPL.FUN_CODIGO = N.TDE_OPERADOR_LANCAMENTO_DOCUMENTO
                                LEFT OUTER JOIN T_FUNCIONARIO OPF ON OPF.FUN_CODIGO = N.TDE_OPERADOR_FINALIZA_DOCUMENTO
                                LEFT OUTER JOIN T_MODDOCFISCAL M ON M.MOD_CODIGO = N.MOD_CODIGO
                                LEFT OUTER JOIN T_NATUREZAOPERACAO NO ON NO.NAT_CODIGO = N.NAT_CODIGO
                                LEFT OUTER JOIN T_PRODUTO P ON P.PRO_CODIGO = IT.PRO_CODIGO
                                LEFT OUTER JOIN T_CFOP CF ON CF.CFO_CODIGO = IT.CFO_CODIGO
                                JOIN T_EMPRESA E ON E.EMP_CODIGO = N.EMP_CODIGO
                                JOIN T_LOCALIDADES LE ON LE.LOC_CODIGO = E.LOC_CODIGO
								LEFT OUTER JOIN T_VEICULO V ON V.VEI_CODIGO = IT.VEI_CODIGO
                                LEFT OUTER JOIN T_VEICULO_SEGMENTO SEG ON SEG.VSE_CODIGO = V.VSE_CODIGO
                                LEFT OUTER JOIN T_GRUPO_PRODUTO_TMS G ON G.GPR_CODIGO = P.GPR_CODIGO
                                LEFT OUTER JOIN T_TIPO_MOVIMENTO TM ON TM.TIM_CODIGO = IT.TIM_CODIGO
                                LEFT OUTER JOIN T_EQUIPAMENTO EQP ON EQP.EQP_CODIGO = IT.EQP_CODIGO
                                LEFT OUTER JOIN T_SERVICO Servico ON Servico.SER_CODIGO = N.SER_CODIGO
                                LEFT OUTER JOIN T_LOCALIDADES LocalidadePrestacaoServico ON LocalidadePrestacaoServico.LOC_CODIGO = N.LOC_CODIGO_PRESTACAO_SERVICO
                                LEFT OUTER JOIN T_LOCAL_ARMAZENAMENTO_PRODUTO LA ON LA.LAP_CODIGO = IT.LAP_CODIGO

                                WHERE 1 = 1";

            querySaida = @"SELECT N.NFI_NUMERO Numero,
                            CAST(ES.ESE_NUMERO AS VARCHAR(20)) Serie,
                            'Saída' DescricaoTipo,
                            N.NFI_DATA_EMISSAO DataEmissao,
                            N.NFI_DATA_EMISSAO DataEntrada,
                            '' DataFinalizacao,
                            '' OperadorLancamentoDocumento,
                            '' OperadorFinalizaDocumento,
                            C.CLI_NOME Pessoa,
                            N.NFI_MODELO Modelo,
                            NO.NAT_DESCRICAO NaturezaOperacao,
                            CASE
                                WHEN N.NFI_CHAVE = '' OR N.NFI_CHAVE IS NULL THEN CAST((N.NFI_CODIGO * -1) AS VARCHAR(50))
                                ELSE N.NFI_CHAVE
                            END Chave,
                            CASE
	                            WHEN N.NFI_STATUS = 1 THEN 'Em Digitação'
	                            WHEN N.NFI_STATUS = 2 THEN 'Inutilizado'
	                            WHEN N.NFI_STATUS = 3 THEN 'Cancelado'
	                            WHEN N.NFI_STATUS = 4 THEN 'Autorizado'
	                            WHEN N.NFI_STATUS = 5 THEN 'Denegado'
	                            WHEN N.NFI_STATUS = 6 THEN 'Rejeitado'
	                            WHEN N.NFI_STATUS = 7 THEN 'Em Processamento'
	                            WHEN N.NFI_STATUS = 8 THEN 'Aguardando Assinatura do XML'
	                            WHEN N.NFI_STATUS = 9 THEN 'Aguardando Cancelamento do XML'
	                            WHEN N.NFI_STATUS = 10 THEN 'Aguardando Inutilizacao do XML'
	                            WHEN N.NFI_STATUS = 11 THEN 'Aguardando Carta Correção do XML'
	                            ELSE ''
                            END DescricaoStatus,
                            N.NFI_VALOR_TOTAL_NOTA ValorTotal,

                            CASE
	                            WHEN IT.PRO_CODIGO IS NOT NULL THEN IT.PRO_CODIGO
	                            ELSE IT.SER_CODIGO
                            END CodigoProduto, 
                            CASE
	                            WHEN P.PRO_DESCRICAO IS NOT NULL THEN P.PRO_DESCRICAO
	                            ELSE S.SER_DESCRICAO
                            END Produto,
                            P.PRO_UNIDADE_MEDIDA UnidadeMedida,
                            CAST(CF.CFO_CFOP AS VARCHAR(20)) + '.' + ISNULL(CF.CFO_EXTENSAO, '') CFOP,
                            ISNULL(CF.CFO_DESCRICAO, 'CFOP SEM DESCRICAO') DescricaoCFOP,
                            IT.NFP_QUANTIDADE Quantidade, 
                            IT.NFP_VALOR_UNITARIO ValorUnitario, 
                            IT.NFP_VALOR_DESCONTO Desconto,
                            IT.NFP_VALOR_TOTAL Valor,

                            IT.NFP_VALOR_UNITARIO ValorUnitarioLiquido, 
                            IT.NFP_VALOR_TOTAL ValorLiquido,

                            CASE
	                            WHEN IT.NFP_CST_CSOSN = 1 THEN '00'
	                            WHEN IT.NFP_CST_CSOSN = 2 THEN '10'
	                            WHEN IT.NFP_CST_CSOSN = 3 THEN '20'
	                            WHEN IT.NFP_CST_CSOSN = 4 THEN '30'
	                            WHEN IT.NFP_CST_CSOSN = 5 THEN '40'
	                            WHEN IT.NFP_CST_CSOSN = 6 THEN '41'
	                            WHEN IT.NFP_CST_CSOSN = 7 THEN '50'
	                            WHEN IT.NFP_CST_CSOSN = 8 THEN '51'
	                            WHEN IT.NFP_CST_CSOSN = 9 THEN '60'
	                            WHEN IT.NFP_CST_CSOSN = 10 THEN '70'
	                            WHEN IT.NFP_CST_CSOSN = 11 THEN '90'
	                            WHEN IT.NFP_CST_CSOSN = 12 THEN '101'
	                            WHEN IT.NFP_CST_CSOSN = 13 THEN '102'
	                            WHEN IT.NFP_CST_CSOSN = 14 THEN '103'
	                            WHEN IT.NFP_CST_CSOSN = 15 THEN '201'
	                            WHEN IT.NFP_CST_CSOSN = 16 THEN '202'
	                            WHEN IT.NFP_CST_CSOSN = 17 THEN '203'
	                            WHEN IT.NFP_CST_CSOSN = 18 THEN '300'
	                            WHEN IT.NFP_CST_CSOSN = 19 THEN '400'
	                            WHEN IT.NFP_CST_CSOSN = 20 THEN '500'
	                            WHEN IT.NFP_CST_CSOSN = 21 THEN '900'
	                            ELSE ''
                            END CstICMS,
                            IT.NFP_BC_ICMS BaseICMS,
                            IT.NFP_ALIQUOTA_ICMS AliquotaICMS,
                            IT.NFP_VALOR_ICMS ValorICMS,
                            NULL AS BaseCalculoICMSFornecedor,
							NULL AS AliquotaICMSFornecedor,
							NULL AS CstIcmsFornecedor,
							NULL AS CfopFornecedor,
							NULL AS ValorICMSFornecedor,

                            IT.NFP_BC_ICMS_ST BaseICMSST,
                            IT.NFP_MVA_ICMS_ST MVA,
                            IT.NFP_ALIQUOTA_ICMS_ST AliquotaICMSST,
                            IT.NFP_VALOR_ICMS_ST ValorICMSST,

                            CASE 
	                            WHEN IT.NFP_CST_PIS = 1 THEN '01'
	                            WHEN IT.NFP_CST_PIS = 2 THEN '02'
                                WHEN IT.NFP_CST_PIS = 3 THEN '03'
                                WHEN IT.NFP_CST_PIS = 4 THEN '04'
                                WHEN IT.NFP_CST_PIS = 5 THEN '05'
                                WHEN IT.NFP_CST_PIS = 6 THEN '06'
                                WHEN IT.NFP_CST_PIS = 7 THEN '07'
                                WHEN IT.NFP_CST_PIS = 8 THEN '08'
                                WHEN IT.NFP_CST_PIS = 9 THEN '09'
                                WHEN IT.NFP_CST_PIS = 10 THEN '49'
                                WHEN IT.NFP_CST_PIS = 11 THEN '50'
                                WHEN IT.NFP_CST_PIS = 12 THEN '51'
                                WHEN IT.NFP_CST_PIS = 13 THEN '52'
                                WHEN IT.NFP_CST_PIS = 14 THEN '53'
                                WHEN IT.NFP_CST_PIS = 15 THEN '54'
                                WHEN IT.NFP_CST_PIS = 16 THEN '55'
                                WHEN IT.NFP_CST_PIS = 17 THEN '56'
                                WHEN IT.NFP_CST_PIS = 18 THEN '60'
                                WHEN IT.NFP_CST_PIS = 19 THEN '61'
                                WHEN IT.NFP_CST_PIS = 20 THEN '62'
                                WHEN IT.NFP_CST_PIS = 21 THEN '63'
                                WHEN IT.NFP_CST_PIS = 22 THEN '64'
                                WHEN IT.NFP_CST_PIS = 23 THEN '65'
                                WHEN IT.NFP_CST_PIS = 24 THEN '66'
                                WHEN IT.NFP_CST_PIS = 25 THEN '67'
                                WHEN IT.NFP_CST_PIS = 26 THEN '70'
                                WHEN IT.NFP_CST_PIS = 27 THEN '71'
                                WHEN IT.NFP_CST_PIS = 28 THEN '72'
                                WHEN IT.NFP_CST_PIS = 29 THEN '73'
                                WHEN IT.NFP_CST_PIS = 30 THEN '74'
                                WHEN IT.NFP_CST_PIS = 31 THEN '75'
                                WHEN IT.NFP_CST_PIS = 32 THEN '98'
                                WHEN IT.NFP_CST_PIS = 33 THEN '99'
	                            ELSE ''
                            END CstPIS,
                            IT.NFP_BC_PIS BasePIS,
                            IT.NFP_ALIQUOTA_PIS AliquotaPIS,
                            IT.NFP_VALOR_PIS ValorPIS,

                            CASE 
	                            WHEN IT.NFP_CST_COFINS = 1 THEN '01'
	                            WHEN IT.NFP_CST_COFINS = 2 THEN '02'
                                WHEN IT.NFP_CST_COFINS = 3 THEN '03'
                                WHEN IT.NFP_CST_COFINS = 4 THEN '04'
                                WHEN IT.NFP_CST_COFINS = 5 THEN '05'
                                WHEN IT.NFP_CST_COFINS = 6 THEN '06'
                                WHEN IT.NFP_CST_COFINS = 7 THEN '07'
                                WHEN IT.NFP_CST_COFINS = 8 THEN '08'
                                WHEN IT.NFP_CST_COFINS = 9 THEN '09'
                                WHEN IT.NFP_CST_COFINS = 10 THEN '49'
                                WHEN IT.NFP_CST_COFINS = 11 THEN '50'
                                WHEN IT.NFP_CST_COFINS = 12 THEN '51'
                                WHEN IT.NFP_CST_COFINS = 13 THEN '52'
                                WHEN IT.NFP_CST_COFINS = 14 THEN '53'
                                WHEN IT.NFP_CST_COFINS = 15 THEN '54'
                                WHEN IT.NFP_CST_COFINS = 16 THEN '55'
                                WHEN IT.NFP_CST_COFINS = 17 THEN '56'
                                WHEN IT.NFP_CST_COFINS = 18 THEN '60'
                                WHEN IT.NFP_CST_COFINS = 19 THEN '61'
                                WHEN IT.NFP_CST_COFINS = 20 THEN '62'
                                WHEN IT.NFP_CST_COFINS = 21 THEN '63'
                                WHEN IT.NFP_CST_COFINS = 22 THEN '64'
                                WHEN IT.NFP_CST_COFINS = 23 THEN '65'
                                WHEN IT.NFP_CST_COFINS = 24 THEN '66'
                                WHEN IT.NFP_CST_COFINS = 25 THEN '67'
                                WHEN IT.NFP_CST_COFINS = 26 THEN '70'
                                WHEN IT.NFP_CST_COFINS = 27 THEN '71'
                                WHEN IT.NFP_CST_COFINS = 28 THEN '72'
                                WHEN IT.NFP_CST_COFINS = 29 THEN '73'
                                WHEN IT.NFP_CST_COFINS = 30 THEN '74'
                                WHEN IT.NFP_CST_COFINS = 31 THEN '75'
                                WHEN IT.NFP_CST_COFINS = 32 THEN '98'
                                WHEN IT.NFP_CST_COFINS = 33 THEN '99'
	                            ELSE ''
                            END CstCOFINS,
                            IT.NFP_BC_COFINS BaseCOFINS,
                            IT.NFP_ALIQUOTA_COFINS AliquotaCOFINS,
                            IT.NFP_VALOR_COFINS ValorCOFINS,

                            CASE
	                            WHEN IT.NFP_CST_IPI = 1 THEN '00'
	                            WHEN IT.NFP_CST_IPI = 2 THEN '01'
	                            WHEN IT.NFP_CST_IPI = 3 THEN '02'
	                            WHEN IT.NFP_CST_IPI = 4 THEN '03'
	                            WHEN IT.NFP_CST_IPI = 5 THEN '04'
	                            WHEN IT.NFP_CST_IPI = 6 THEN '05'
	                            WHEN IT.NFP_CST_IPI = 7 THEN '49'
	                            WHEN IT.NFP_CST_IPI = 8 THEN '50'
	                            WHEN IT.NFP_CST_IPI = 9 THEN '51'
	                            WHEN IT.NFP_CST_IPI = 10 THEN '52'
	                            WHEN IT.NFP_CST_IPI = 11 THEN '53'
	                            WHEN IT.NFP_CST_IPI = 12 THEN '54'
	                            WHEN IT.NFP_CST_IPI = 13 THEN '55'
	                            WHEN IT.NFP_CST_IPI = 14 THEN '99'
	                            ELSE ''
                            END CstIPI,
                            IT.NFP_BC_IPI BaseIPI,
                            IT.NFP_ALIQUOTA_IPI AliquotaIPI,
                            IT.NFP_VALOR_IPI ValorIPI,

                            0.0 RetencaoPIS,
                            0.0 RetencaoCOFNIS,
                            0.0 RetencaoINSS,
                            0.0 RetencaoIPI,
                            0.0 RetencaoCSLL,
                            0.0 RetencaoOUTRAS,
                            0.0 RetencaoIR,
                            0.0 RetencaoISS,
                            0 KmAbastecimento,
                            0 Horimetro,

                            0.0 OutrasDespesas,
							0.0 ValorFrete, 
							0.0 ValorSeguro, 
							0.0 ValorDiferencial,
                            
                            '' Veiculo,
							0 CodigoVeiculo,
							E.EMP_RAZAO Empresa,
							E.EMP_CODIGO CodigoEmpresa,

                            CAST(SUBSTRING((SELECT DISTINCT ', ' + 
                            CASE 
	                            WHEN T.TIT_STATUS = 1 THEN 'Aberto' 
	                            WHEN T.TIT_VALOR_PAGO < T.TIT_VALOR_ORIGINAL THEN 
		                            CASE
			                            WHEN ISNULL((SELECT COUNT(1) FROM T_TITULO TT WHERE TT.NFI_CODIGO = N.NFI_CODIGO AND TT.TIT_STATUS <> 3 AND TT.TIT_STATUS <> 4), 0) >= 1 THEN 'Renegociado' 
                                        WHEN ISNULL((SELECT COUNT(1) FROM T_TITULO_BAIXA_AGRUPADO TBA JOIN T_TITULO_BAIXA_NEGOCIACAO TBN ON TBN.TIB_CODIGO = TBA.TIB_CODIGO JOIN T_TITULO TT ON TT.TBN_CODIGO = TBN.TBN_CODIGO WHERE TT.TIT_STATUS <> 3 AND TT.TIT_STATUS <> 4 AND TBA.TIT_CODIGO = T.TIT_CODIGO), 0) >= 1 THEN 'Renegociado' 
                                        
                                        WHEN ISNULL(
                                                    (SELECT COUNT(1)
                                                    FROM T_TITULO_BAIXA_NEGOCIACAO TBN
                                                    JOIN T_TITULO TT ON TT.TBN_CODIGO = TBN.TBN_CODIGO
                                                    WHERE TT.TIT_STATUS <> 3 AND TT.TIT_STATUS <> 4
                                                        AND TBN.TIB_CODIGO IN
                                                        (SELECT tituloBaixa.TIB_CODIGO
                                                            FROM T_TITULO_BAIXA tituloBaixa
                                                            JOIN T_TITULO_BAIXA_AGRUPADO TBA ON TBA.TIB_CODIGO = tituloBaixa.TIB_CODIGO
                                                            WHERE tituloBaixa.TIB_SITUACAO <> 4 AND TBA.TIT_CODIGO = T.TIT_CODIGO)), 0) >= 1 THEN 'Renegociado'
                                        WHEN ISNULL(
                                                    (SELECT COUNT(1)
                                                    FROM T_TITULO_BAIXA_NEGOCIACAO TBNN
                                                    JOIN T_TITULO TTT ON TTT.TBN_CODIGO = TBNN.TBN_CODIGO
                                                    WHERE TTT.TIT_STATUS <> 3 AND TTT.TIT_STATUS <> 4
                                                        AND TBNN.TIB_CODIGO IN
                                                        (SELECT tituloBaixa2.TIB_CODIGO
                                                            FROM T_TITULO_BAIXA tituloBaixa2
                                                            JOIN T_TITULO_BAIXA_AGRUPADO TBAA ON TBAA.TIB_CODIGO = tituloBaixa2.TIB_CODIGO
                                                            WHERE tituloBaixa2.TIB_SITUACAO <> 4
                                                            AND TBAA.TIT_CODIGO IN
                                                                (SELECT TT.TIT_CODIGO
                                                                FROM T_TITULO_BAIXA_NEGOCIACAO TBN
                                                                JOIN T_TITULO TT ON TT.TBN_CODIGO = TBN.TBN_CODIGO
                                                                WHERE TBN.TIB_CODIGO IN
                                                                    (SELECT tituloBaixa.TIB_CODIGO
                                                                    FROM T_TITULO_BAIXA tituloBaixa
                                                                    JOIN T_TITULO_BAIXA_AGRUPADO TBA ON TBA.TIB_CODIGO = tituloBaixa.TIB_CODIGO
                                                                    WHERE tituloBaixa.TIB_SITUACAO <> 4 AND TBA.TIT_CODIGO = T.TIT_CODIGO)))), 0) >= 1 THEN 'Renegociado'

			                            ELSE 'Pago' 
		                            END
	                            ELSE 'Pago' 
                            END
                            FROM T_TITULO T
                            WHERE T.TIT_STATUS <> 4 AND T.NFI_CODIGO = N.NFI_CODIGO FOR XML PATH('')), 3, 1000) AS VARCHAR(2000)) SituacaoFinanceiraNota,

							CAST(SUBSTRING((SELECT DISTINCT ', ' +  CONVERT(VARCHAR, TT.TIT_DATA_VENCIMENTO, 103) 
							FROM T_TITULO TT
							WHERE TT.TIT_STATUS <> 4 AND TT.NFI_CODIGO = N.NFI_CODIGO FOR XML PATH('')), 3, 1000) AS VARCHAR(2000)) DataVencimento,
	
							CAST(SUBSTRING((SELECT DISTINCT ', ' +  CONVERT(VARCHAR, TT.TIT_DATA_LIQUIDACAO, 103) 
							FROM T_TITULO TT
							WHERE TT.TIT_STATUS = 3 AND TT.NFI_CODIGO = N.NFI_CODIGO FOR XML PATH('')), 3, 1000) AS VARCHAR(2000)) DataPagamento,

                            L.UF_SIGLA EstadoPessoa,
                            C.CLI_CGCCPF CPFCNPJPessoa,
                            ''  TipoVeiculo,
                            C.CLI_FISJUR TipoCliente,
                            L.LOC_DESCRICAO Cidade,
                            0.0 BaseSTRetido,
                            0.0 ValorSTRetido,
                            N.NFI_CODIGO CodigoNota,
                            GRP_DESCRICAO GrupoProduto, '' Segmento, 0.0 ValorImpostosFora, 'NÃO INFORMADO' TipoMovimento,
                            '' Equipamento,
                            C.CLI_REGIME_TRIBUTARIO RegimeTributario,
                            '' UnidadeMedidaFornecedor,
                            0.00 QuantidadeFornecedor,
                            0.00 ValorUnitarioFornecedor,
                            P.PRO_COD_PRODUTO ProdutoCodigoProduto,
                            '' Servico,
                            '' LocalidadePrestacaoServico,
                            null TipoDocumento,
                            -1 CSTServico,
                            0.00 AliquotaSimplesNacional,
                            null DocumentoFiscalProvenienteSimplesNacional,
                            null TributaISSNoMunicipio,
                            0.00 ValorAbastecimentoTabelaFornecedor,
                            null ValorAbastecimentoComDivergencia,
                            '' LocalArmazenamento,
                            null DataAbastecimento,
                            E.EMP_CNPJ CNPJFilial,
                            null OrdemCompra,
                            null OrdemServico,
                            null CustoUnitario,
                            null CustoTotal

                            FROM T_NOTA_FISCAL_PRODUTOS IT
                            JOIN T_NOTA_FISCAL N ON N.NFI_CODIGO = IT.NFI_CODIGO
                            LEFT OUTER JOIN T_CLIENTE C ON C.CLI_CGCCPF = N.CLI_CGCCPF
                            LEFT OUTER JOIN T_LOCALIDADES L ON L.LOC_CODIGO = C.LOC_CODIGO	
                            JOIN T_EMPRESA_SERIE ES ON ES.ESE_CODIGO = N.ESE_CODIGO
                            LEFT OUTER JOIN T_NATUREZAOPERACAO NO ON NO.NAT_CODIGO = N.NAT_CODIGO
                            LEFT OUTER JOIN T_PRODUTO P ON P.PRO_CODIGO = IT.PRO_CODIGO
                            LEFT OUTER JOIN T_SERVICO S ON S.SER_CODIGO = IT.SER_CODIGO
                            LEFT OUTER JOIN T_CFOP CF ON CF.CFO_CODIGO = IT.CFO_CODIGO
                            JOIN T_EMPRESA E ON E.EMP_CODIGO = N.EMP_CODIGO
                            JOIN T_LOCALIDADES LE ON LE.LOC_CODIGO = E.LOC_CODIGO
                            LEFT OUTER JOIN T_GRUPO_PRODUTO_TMS G ON G.GPR_CODIGO = P.GPR_CODIGO        
                            LEFT OUTER JOIN T_MODDOCFISCAL M ON M.MOD_CODIGO = N.MOD_CODIGO
                            WHERE 1 = 1";


            if (filtrosPesquisa.OperadorLancamentoEntrada > 0)
                queryEntrada += " AND OPL.FUN_CODIGO = " + filtrosPesquisa.OperadorLancamentoEntrada.ToString();

            if (filtrosPesquisa.OperadorFinalizaEntrada > 0)
                queryEntrada += " AND OPF.FUN_CODIGO = " + filtrosPesquisa.OperadorFinalizaEntrada.ToString();

            if (filtrosPesquisa.CodigoEmpresaFilial > 0)
            {
                querySaida += " AND E.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresaFilial.ToString();
                queryEntrada += " AND E.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresaFilial.ToString();
            }

            string datePattern = "yyyy-MM-dd";
            if (filtrosPesquisa.DataVencimentoInicial != DateTime.MinValue)
            {
                queryEntrada += $@" AND EXISTS (SELECT TOP(1) 1
                                FROM T_TITULO TT
                                JOIN T_TMS_DOCUMENTO_ENTRADA_DUPLICATA D ON D.TDD_CODIGO = TT.TDD_CODIGO
                                WHERE TT.TIT_STATUS <> 4 AND D.TDE_CODIGO = N.TDE_CODIGO
                                AND CAST(TT.TIT_DATA_VENCIMENTO AS DATE) >= '{filtrosPesquisa.DataVencimentoInicial.ToString(datePattern)}') ";

                querySaida += $@" AND EXISTS (SELECT TOP(1) 1
                                FROM T_TITULO TT
                                WHERE TT.TIT_STATUS <> 4 AND TT.NFI_CODIGO = N.NFI_CODIGO
                                AND CAST(TT.TIT_DATA_VENCIMENTO AS DATE) >= '{filtrosPesquisa.DataVencimentoInicial.ToString(datePattern)}') ";
            }

            if (filtrosPesquisa.DataVencimentoFinal != DateTime.MinValue)
            {
                queryEntrada += $@" AND EXISTS (SELECT TOP(1) 1
                                FROM T_TITULO TT
                                JOIN T_TMS_DOCUMENTO_ENTRADA_DUPLICATA D ON D.TDD_CODIGO = TT.TDD_CODIGO
                                WHERE TT.TIT_STATUS <> 4 AND D.TDE_CODIGO = N.TDE_CODIGO
                                AND CAST(TT.TIT_DATA_VENCIMENTO AS DATE) <= '{filtrosPesquisa.DataVencimentoFinal.ToString(datePattern)} ') ";

                querySaida += $@" AND EXISTS (SELECT TOP(1) 1
                                FROM T_TITULO TT
                                WHERE TT.TIT_STATUS <> 4 AND TT.NFI_CODIGO = N.NFI_CODIGO
                                AND CAST(TT.TIT_DATA_VENCIMENTO AS DATE) <= '{filtrosPesquisa.DataVencimentoFinal.ToString(datePattern)}') ";
            }

            if (!string.IsNullOrEmpty(filtrosPesquisa.EstadoEmitente) && filtrosPesquisa.EstadoEmitente != "0")
            {
                querySaida += " AND LE.UF_SIGLA = '" + filtrosPesquisa.EstadoEmitente + "'";
                queryEntrada += " AND L.UF_SIGLA = '" + filtrosPesquisa.EstadoEmitente + "'";
            }

            if (filtrosPesquisa.CodigoVeiculo > 0)
            {
                querySaida += " AND E.EMP_CODIGO IS NULL ";
                queryEntrada += " AND IT.VEI_CODIGO = " + filtrosPesquisa.CodigoVeiculo.ToString();
            }

            if (filtrosPesquisa.CodigoSegmento > 0)
            {
                queryEntrada += " AND V.VSE_CODIGO = " + filtrosPesquisa.CodigoSegmento.ToString();
                querySaida += " AND 1 = 0 ";
            }

            if (filtrosPesquisa.CodigoEquipamento > 0)
            {
                queryEntrada += " AND EQP.EQP_CODIGO = " + filtrosPesquisa.CodigoEquipamento.ToString();
                querySaida += " AND 1 = 0 ";
            }

            if ((int)filtrosPesquisa.StatusNotaEntrada > 0)
            {
                querySaida += " AND E.EMP_CODIGO IS NULL ";
                queryEntrada += " AND N.TDE_SITUACAO = " + filtrosPesquisa.StatusNotaEntrada.ToString("d");
            }

            if (filtrosPesquisa.NumeroInicial > 0 && filtrosPesquisa.NumeroFinal == 0)
            {
                querySaida += " AND N.NFI_NUMERO = " + filtrosPesquisa.NumeroInicial.ToString();
                queryEntrada += " AND N.TDE_NUMERO_LONG = " + filtrosPesquisa.NumeroInicial.ToString();
            }
            else if (filtrosPesquisa.NumeroInicial > 0 && filtrosPesquisa.NumeroFinal > 0)
            {
                querySaida += " AND N.NFI_NUMERO >= " + filtrosPesquisa.NumeroInicial.ToString() + " AND N.NFI_NUMERO <= " + filtrosPesquisa.NumeroFinal.ToString();
                queryEntrada += " AND N.TDE_NUMERO_LONG >= " + filtrosPesquisa.NumeroInicial.ToString() + " AND N.TDE_NUMERO_LONG <= " + filtrosPesquisa.NumeroFinal.ToString();
            }
            else if (filtrosPesquisa.NumeroInicial == 0 && filtrosPesquisa.NumeroFinal > 0)
            {
                querySaida += " AND N.NFI_NUMERO = " + filtrosPesquisa.NumeroFinal.ToString();
                queryEntrada += " AND N.TDE_NUMERO_LONG = " + filtrosPesquisa.NumeroFinal.ToString();
            }

            if (filtrosPesquisa.Serie > 0)
            {
                querySaida += " AND ES.ESE_NUMERO = " + filtrosPesquisa.Serie.ToString();
                queryEntrada += " AND N.TDE_SERIE = " + filtrosPesquisa.Serie.ToString();
            }

            if (filtrosPesquisa.CodigoServico > 0)
            {
                querySaida += " AND IT.SER_CODIGO = " + filtrosPesquisa.CodigoServico.ToString();
                queryEntrada += " AND IT.PRO_CODIGO IS NULL";
            }

            if (filtrosPesquisa.CodigosTipoMovimento != null && filtrosPesquisa.CodigosTipoMovimento.Count > 0)
            {
                querySaida += " AND 1 = 0";
                queryEntrada += " and IT.TIM_CODIGO in (" + string.Join(",", filtrosPesquisa.CodigosTipoMovimento) + ")";
            }

            if (filtrosPesquisa.CodigosModeloDocumentoFiscal != null && filtrosPesquisa.CodigosModeloDocumentoFiscal.Count > 0)
            {
                querySaida += " and M.MOD_CODIGO in (" + string.Join(",", filtrosPesquisa.CodigosModeloDocumentoFiscal) + ")";
                queryEntrada += " and M.MOD_CODIGO in (" + string.Join(",", filtrosPesquisa.CodigosModeloDocumentoFiscal) + ")";
            }
            if (filtrosPesquisa.CodigoModelo > 0)
            {
                queryEntrada += " AND M.MOD_CODIGO = " + filtrosPesquisa.CodigoModelo.ToString();
            }
            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroModeloNF))
            {
                querySaida += " AND N.NFI_MODELO = '" + filtrosPesquisa.NumeroModeloNF + "'";
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Chave))
            {
                querySaida += " AND N.NFI_CHAVE LIKE '%" + filtrosPesquisa.Chave + "%'";
                queryEntrada += " AND N.TDE_CHAVE LIKE '%" + filtrosPesquisa.Chave + "%'";
            }

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
            {
                querySaida += " AND CAST(N.NFI_DATA_EMISSAO AS DATE) >= '" + filtrosPesquisa.DataInicial.ToString(datePattern) + "'";
                queryEntrada += " AND CAST(N.TDE_DATA_EMISSAO AS DATE) >= '" + filtrosPesquisa.DataInicial.ToString(datePattern) + "'";
            }

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
            {
                querySaida += " AND CAST(N.NFI_DATA_EMISSAO AS DATE) <= '" + filtrosPesquisa.DataFinal.ToString(datePattern) + "'";
                queryEntrada += " AND CAST(N.TDE_DATA_EMISSAO AS DATE) <= '" + filtrosPesquisa.DataFinal.ToString(datePattern) + "'";
            }

            if (filtrosPesquisa.DataEntradaInicial != DateTime.MinValue)
            {
                querySaida += " AND CAST(N.NFI_DATA_EMISSAO AS DATE) >= '" + filtrosPesquisa.DataEntradaInicial.ToString(datePattern) + "'";
                queryEntrada += " AND CAST(N.TDE_DATA_ENTRADA AS DATE) >= '" + filtrosPesquisa.DataEntradaInicial.ToString(datePattern) + "'";
            }

            if (filtrosPesquisa.DataEntradaFinal != DateTime.MinValue)
            {
                querySaida += " AND CAST(N.NFI_DATA_EMISSAO AS DATE) <= '" + filtrosPesquisa.DataEntradaFinal.ToString(datePattern) + "'";
                queryEntrada += " AND CAST(N.TDE_DATA_ENTRADA AS DATE) <= '" + filtrosPesquisa.DataEntradaFinal.ToString(datePattern) + "'";
            }

            if (filtrosPesquisa.NotasComDiferencaDeValorTabelaFornecedor)
            {
                querySaida += " AND 1 = 0 ";
                queryEntrada += " AND IT.TDI_VALOR_ABASTECIMENTO_COM_DIVERGENCIA = 1";
            }

            if (filtrosPesquisa.DataFinalizacaoInicial != DateTime.MinValue)
            {
                querySaida += " AND CAST('' AS DATE) >= '" + filtrosPesquisa.DataFinalizacaoInicial.ToString(datePattern) + "'";
                queryEntrada += " AND CAST(N.TDE_DATA_FINALIZACAO AS DATE) >= '" + filtrosPesquisa.DataFinalizacaoInicial.ToString(datePattern) + "'";
            }

            if (filtrosPesquisa.DataFinalizacaoFinal != DateTime.MinValue)
            {
                querySaida += " AND CAST('' AS DATE) <= '" + filtrosPesquisa.DataFinalizacaoFinal.ToString(datePattern) + "'";
                queryEntrada += " AND CAST(N.TDE_DATA_FINALIZACAO AS DATE) <= '" + filtrosPesquisa.DataFinalizacaoFinal.ToString(datePattern) + "'";
            }

            var filtroStatus = "";
            if (filtrosPesquisa.SituacaoFinanceiraNotaEntrada == StatusTitulo.Cancelado)
                filtroStatus = "Cancelado";
            else if (filtrosPesquisa.SituacaoFinanceiraNotaEntrada == StatusTitulo.EmAberto)
                filtroStatus = "Aberto";
            else if (filtrosPesquisa.SituacaoFinanceiraNotaEntrada == StatusTitulo.EmNegociacao)
                filtroStatus = "Renegociado";
            else if (filtrosPesquisa.SituacaoFinanceiraNotaEntrada == StatusTitulo.Quitada)
                filtroStatus = "Pago";

            if (!string.IsNullOrWhiteSpace(filtroStatus))
            {
                queryEntrada += $@" AND N.TDE_CODIGO in (SELECT TT.TDE_CODIGO FROM (
									    SELECT T.Situacao, T.TDE_CODIGO FROM (
									    SELECT
									    (CASE 
										    WHEN T.TIT_STATUS = 1 THEN 'Aberto' 
										    WHEN T.TIT_VALOR_PAGO < T.TIT_VALOR_ORIGINAL THEN 
											    CASE
												    WHEN ISNULL((SELECT COUNT(1) FROM T_TITULO TT WHERE TT.TDD_CODIGO = T.TDD_CODIGO AND TT.TIT_STATUS <> 3 AND TT.TIT_STATUS <> 4), 0) >= 1 THEN 'Renegociado' 
												    WHEN ISNULL((SELECT COUNT(1) FROM T_TITULO_BAIXA_AGRUPADO TBA JOIN T_TITULO_BAIXA_NEGOCIACAO TBN ON TBN.TIB_CODIGO = TBA.TIB_CODIGO JOIN T_TITULO TT ON TT.TBN_CODIGO = TBN.TBN_CODIGO WHERE TT.TIT_STATUS <> 3 AND TT.TIT_STATUS <> 4 AND TBA.TIT_CODIGO = T.TIT_CODIGO), 0) >= 1 THEN 'Renegociado' 
                                                    
                                                    WHEN ISNULL(
                                                                (SELECT COUNT(1)
                                                                FROM T_TITULO_BAIXA_NEGOCIACAO TBN
                                                                JOIN T_TITULO TT ON TT.TBN_CODIGO = TBN.TBN_CODIGO
                                                                WHERE TT.TIT_STATUS <> 3 AND TT.TIT_STATUS <> 4
                                                                    AND TBN.TIB_CODIGO IN
                                                                    (SELECT tituloBaixa.TIB_CODIGO
                                                                        FROM T_TITULO_BAIXA tituloBaixa
                                                                        JOIN T_TITULO_BAIXA_AGRUPADO TBA ON TBA.TIB_CODIGO = tituloBaixa.TIB_CODIGO
                                                                        WHERE tituloBaixa.TIB_SITUACAO <> 4 AND TBA.TIT_CODIGO = T.TIT_CODIGO)), 0) >= 1 THEN 'Renegociado'
                                                    WHEN ISNULL(
                                                                (SELECT COUNT(1)
                                                                FROM T_TITULO_BAIXA_NEGOCIACAO TBNN
                                                                JOIN T_TITULO TTT ON TTT.TBN_CODIGO = TBNN.TBN_CODIGO
                                                                WHERE TTT.TIT_STATUS <> 3 AND TTT.TIT_STATUS <> 4
                                                                    AND TBNN.TIB_CODIGO IN
                                                                    (SELECT tituloBaixa2.TIB_CODIGO
                                                                        FROM T_TITULO_BAIXA tituloBaixa2
                                                                        JOIN T_TITULO_BAIXA_AGRUPADO TBAA ON TBAA.TIB_CODIGO = tituloBaixa2.TIB_CODIGO
                                                                        WHERE tituloBaixa2.TIB_SITUACAO <> 4
                                                                        AND TBAA.TIT_CODIGO IN
                                                                            (SELECT TT.TIT_CODIGO
                                                                            FROM T_TITULO_BAIXA_NEGOCIACAO TBN
                                                                            JOIN T_TITULO TT ON TT.TBN_CODIGO = TBN.TBN_CODIGO
                                                                            WHERE TBN.TIB_CODIGO IN
                                                                                (SELECT tituloBaixa.TIB_CODIGO
                                                                                FROM T_TITULO_BAIXA tituloBaixa
                                                                                JOIN T_TITULO_BAIXA_AGRUPADO TBA ON TBA.TIB_CODIGO = tituloBaixa.TIB_CODIGO
                                                                                WHERE tituloBaixa.TIB_SITUACAO <> 4 AND TBA.TIT_CODIGO = T.TIT_CODIGO)))), 0) >= 1 THEN 'Renegociado'

												    ELSE 'Pago' 
											    END
										    ELSE 'Pago' 
									    END) Situacao, D.TDE_CODIGO
									    FROM T_TITULO T
									    JOIN T_TMS_DOCUMENTO_ENTRADA_DUPLICATA D ON D.TDD_CODIGO = T.TDD_CODIGO
								        WHERE T.TIT_STATUS <> 4) AS T WHERE T.Situacao = '{filtroStatus}') AS TT)";

                querySaida += $@" AND N.NFI_CODIGO in (SELECT TT.NFI_CODIGO FROM (
								        SELECT T.Situacao, T.NFI_CODIGO FROM (
								        SELECT
								        (CASE 
								            WHEN T.TIT_STATUS = 1 THEN 'Aberto' 
								            WHEN T.TIT_VALOR_PAGO < T.TIT_VALOR_ORIGINAL THEN 
								                CASE
									                WHEN ISNULL((SELECT COUNT(1) FROM T_TITULO TT WHERE TT.NFI_CODIGO = N.NFI_CODIGO AND TT.TIT_STATUS <> 3 AND TT.TIT_STATUS <> 4), 0) >= 1 THEN 'Renegociado' 
                                                    WHEN ISNULL((SELECT COUNT(1) FROM T_TITULO_BAIXA_AGRUPADO TBA JOIN T_TITULO_BAIXA_NEGOCIACAO TBN ON TBN.TIB_CODIGO = TBA.TIB_CODIGO JOIN T_TITULO TT ON TT.TBN_CODIGO = TBN.TBN_CODIGO WHERE TT.TIT_STATUS <> 3 AND TT.TIT_STATUS <> 4 AND TBA.TIT_CODIGO = T.TIT_CODIGO), 0) >= 1 THEN 'Renegociado' 
                                                    
                                                    WHEN ISNULL(
                                                                (SELECT COUNT(1)
                                                                FROM T_TITULO_BAIXA_NEGOCIACAO TBN
                                                                JOIN T_TITULO TT ON TT.TBN_CODIGO = TBN.TBN_CODIGO
                                                                WHERE TT.TIT_STATUS <> 3 AND TT.TIT_STATUS <> 4
                                                                    AND TBN.TIB_CODIGO IN
                                                                    (SELECT tituloBaixa.TIB_CODIGO
                                                                        FROM T_TITULO_BAIXA tituloBaixa
                                                                        JOIN T_TITULO_BAIXA_AGRUPADO TBA ON TBA.TIB_CODIGO = tituloBaixa.TIB_CODIGO
                                                                        WHERE tituloBaixa.TIB_SITUACAO <> 4 AND TBA.TIT_CODIGO = T.TIT_CODIGO)), 0) >= 1 THEN 'Renegociado'
                                                    WHEN ISNULL(
                                                                (SELECT COUNT(1)
                                                                FROM T_TITULO_BAIXA_NEGOCIACAO TBNN
                                                                JOIN T_TITULO TTT ON TTT.TBN_CODIGO = TBNN.TBN_CODIGO
                                                                WHERE TTT.TIT_STATUS <> 3 AND TTT.TIT_STATUS <> 4
                                                                    AND TBNN.TIB_CODIGO IN
                                                                    (SELECT tituloBaixa2.TIB_CODIGO
                                                                        FROM T_TITULO_BAIXA tituloBaixa2
                                                                        JOIN T_TITULO_BAIXA_AGRUPADO TBAA ON TBAA.TIB_CODIGO = tituloBaixa2.TIB_CODIGO
                                                                        WHERE tituloBaixa2.TIB_SITUACAO <> 4
                                                                        AND TBAA.TIT_CODIGO IN
                                                                            (SELECT TT.TIT_CODIGO
                                                                            FROM T_TITULO_BAIXA_NEGOCIACAO TBN
                                                                            JOIN T_TITULO TT ON TT.TBN_CODIGO = TBN.TBN_CODIGO
                                                                            WHERE TBN.TIB_CODIGO IN
                                                                                (SELECT tituloBaixa.TIB_CODIGO
                                                                                FROM T_TITULO_BAIXA tituloBaixa
                                                                                JOIN T_TITULO_BAIXA_AGRUPADO TBA ON TBA.TIB_CODIGO = tituloBaixa.TIB_CODIGO
                                                                                WHERE tituloBaixa.TIB_SITUACAO <> 4 AND TBA.TIT_CODIGO = T.TIT_CODIGO)))), 0) >= 1 THEN 'Renegociado'

									                ELSE 'Pago' 
								                END
								            ELSE 'Pago' 
								        END) Situacao, T.NFI_CODIGO
								        FROM T_TITULO T
								        WHERE T.NFI_CODIGO = N.NFI_CODIGO AND T.TIT_STATUS <> 4) AS T WHERE T.Situacao = '{filtroStatus}') AS TT)";
            }

            querySaida += " AND N.NFI_AMBIENTE = " + (int)filtrosPesquisa.TipoAmbiente;

            if (filtrosPesquisa.TipoMovimento == TipoEntradaSaida.Entrada)
                query = queryEntrada + queryParameters;
            else if (filtrosPesquisa.TipoMovimento == TipoEntradaSaida.Saida)
                query = querySaida + queryParameters;
            else
                query = queryEntrada + queryParameters + " UNION ALL " + querySaida + queryParameters;

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

            if (paginar && maximoRegistros > 0)
                query += " OFFSET " + inicioRegistros + " ROWS FETCH FIRST " + maximoRegistros + " ROWS ONLY";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.NFe.NotasDetalhadas)));

            return nhQuery.SetTimeout(60000).List<Dominio.Relatorios.Embarcador.DataSource.NFe.NotasDetalhadas>();
        }

        public int ContarRelatorioNotasDetalhadas(Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNotasDetalhadas filtrosPesquisa)
        {
            var parametros = new List<ParametroSQL>();

            string query = "", queryParameters = "", queryEntrada = "", querySaida = "";
            if (filtrosPesquisa.CodigoEmpresa > 0)
                queryParameters += " AND E.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresa.ToString();

            if (filtrosPesquisa.CnpjPessoa > 0)
                queryParameters += " AND C.CLI_CGCCPF = " + filtrosPesquisa.CnpjPessoa.ToString();

            if (filtrosPesquisa.CodigosNaturezaOperacao?.Count > 0)
                queryParameters += $" AND NO.NAT_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosNaturezaOperacao)})";

            if (filtrosPesquisa.CodigoProduto > 0)
                queryParameters += " AND IT.PRO_CODIGO = " + filtrosPesquisa.CodigoProduto.ToString();

            if (filtrosPesquisa.CodigosGrupoProduto != null && filtrosPesquisa.CodigosGrupoProduto.Count > 0)
                queryParameters += $" AND G.GPR_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosGrupoProduto)})";

            if (filtrosPesquisa.CodigoGrupoPessoa > 0)
                queryParameters += " AND C.GRP_CODIGO = " + filtrosPesquisa.CodigoGrupoPessoa.ToString();

            queryEntrada = @" SELECT COUNT(0) as CONTADOR                                
	
                                FROM T_TMS_DOCUMENTO_ENTRADA_ITEM IT
                                JOIN T_TMS_DOCUMENTO_ENTRADA N ON N.TDE_CODIGO = IT.TDE_CODIGO
                                JOIN T_CLIENTE C ON C.CLI_CGCCPF = N.CLI_CGCCPF
								JOIN T_LOCALIDADES L ON L.LOC_CODIGO = C.LOC_CODIGO	
                                LEFT OUTER JOIN T_FUNCIONARIO OPL ON OPL.FUN_CODIGO = N.TDE_OPERADOR_LANCAMENTO_DOCUMENTO
                                LEFT OUTER JOIN T_FUNCIONARIO OPF ON OPF.FUN_CODIGO = N.TDE_OPERADOR_FINALIZA_DOCUMENTO
                                LEFT OUTER JOIN T_MODDOCFISCAL M ON M.MOD_CODIGO = N.MOD_CODIGO
                                LEFT OUTER JOIN T_NATUREZAOPERACAO NO ON NO.NAT_CODIGO = N.NAT_CODIGO
                                LEFT OUTER JOIN T_PRODUTO P ON P.PRO_CODIGO = IT.PRO_CODIGO
                                LEFT OUTER JOIN T_CFOP CF ON CF.CFO_CODIGO = IT.CFO_CODIGO
                                JOIN T_EMPRESA E ON E.EMP_CODIGO = N.EMP_CODIGO
                                JOIN T_LOCALIDADES LE ON LE.LOC_CODIGO = E.LOC_CODIGO
								LEFT OUTER JOIN T_VEICULO V ON V.VEI_CODIGO = IT.VEI_CODIGO
                                LEFT OUTER JOIN T_VEICULO_SEGMENTO SEG ON SEG.VSE_CODIGO = V.VSE_CODIGO
                                LEFT OUTER JOIN T_GRUPO_PRODUTO_TMS G ON G.GPR_CODIGO = P.GPR_CODIGO
                                LEFT OUTER JOIN T_TIPO_MOVIMENTO TM ON TM.TIM_CODIGO = IT.TIM_CODIGO
                                LEFT OUTER JOIN T_EQUIPAMENTO EQP ON EQP.EQP_CODIGO = IT.EQP_CODIGO
                                LEFT OUTER JOIN T_SERVICO Servico ON Servico.SER_CODIGO = N.SER_CODIGO
                                LEFT OUTER JOIN T_LOCALIDADES LocalidadePrestacaoServico ON LocalidadePrestacaoServico.LOC_CODIGO = N.LOC_CODIGO_PRESTACAO_SERVICO
                                WHERE 1 = 1  ";

            querySaida = @" SELECT COUNT(0) as CONTADOR

                            FROM T_NOTA_FISCAL_PRODUTOS IT
                            JOIN T_NOTA_FISCAL N ON N.NFI_CODIGO = IT.NFI_CODIGO
                            LEFT OUTER JOIN T_CLIENTE C ON C.CLI_CGCCPF = N.CLI_CGCCPF
                            LEFT OUTER JOIN T_LOCALIDADES L ON L.LOC_CODIGO = C.LOC_CODIGO	
                            JOIN T_EMPRESA_SERIE ES ON ES.ESE_CODIGO = N.ESE_CODIGO
                            LEFT OUTER JOIN T_NATUREZAOPERACAO NO ON NO.NAT_CODIGO = N.NAT_CODIGO
                            LEFT OUTER JOIN T_PRODUTO P ON P.PRO_CODIGO = IT.PRO_CODIGO
                            LEFT OUTER JOIN T_SERVICO S ON S.SER_CODIGO = IT.SER_CODIGO
                            LEFT OUTER JOIN T_CFOP CF ON CF.CFO_CODIGO = IT.CFO_CODIGO
                            JOIN T_EMPRESA E ON E.EMP_CODIGO = N.EMP_CODIGO
                            JOIN T_LOCALIDADES LE ON LE.LOC_CODIGO = E.LOC_CODIGO
                            LEFT OUTER JOIN T_GRUPO_PRODUTO_TMS G ON G.GPR_CODIGO = P.GPR_CODIGO
                            LEFT OUTER JOIN T_MODDOCFISCAL M ON M.MOD_CODIGO = N.MOD_CODIGO
                            WHERE 1 = 1 ";

            if (filtrosPesquisa.CodigoEmpresaFilial > 0)
            {
                querySaida += " AND E.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresaFilial.ToString();
                queryEntrada += " AND E.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresaFilial.ToString();
            }

            string datePattern = "yyyy-MM-dd";
            if (filtrosPesquisa.DataVencimentoInicial != DateTime.MinValue)
            {
                queryEntrada += $@" AND EXISTS (SELECT TOP(1) 1
                                FROM T_TITULO TT
                                JOIN T_TMS_DOCUMENTO_ENTRADA_DUPLICATA D ON D.TDD_CODIGO = TT.TDD_CODIGO
                                WHERE TT.TIT_STATUS <> 4 AND D.TDE_CODIGO = N.TDE_CODIGO
                                AND CAST(TT.TIT_DATA_VENCIMENTO AS DATE) >= '{filtrosPesquisa.DataVencimentoInicial.ToString(datePattern)}') ";

                querySaida += $@" AND EXISTS (SELECT TOP(1) 1
                                FROM T_TITULO TT
                                WHERE TT.TIT_STATUS <> 4 AND TT.NFI_CODIGO = N.NFI_CODIGO
                                AND CAST(TT.TIT_DATA_VENCIMENTO AS DATE) >= '{filtrosPesquisa.DataVencimentoInicial.ToString(datePattern)}') ";
            }

            if (filtrosPesquisa.DataVencimentoFinal != DateTime.MinValue)
            {
                queryEntrada += $@" AND EXISTS (SELECT TOP(1) 1
                                FROM T_TITULO TT
                                JOIN T_TMS_DOCUMENTO_ENTRADA_DUPLICATA D ON D.TDD_CODIGO = TT.TDD_CODIGO
                                WHERE TT.TIT_STATUS <> 4 AND D.TDE_CODIGO = N.TDE_CODIGO
                                AND CAST(TT.TIT_DATA_VENCIMENTO AS DATE) <= '{filtrosPesquisa.DataVencimentoFinal.ToString(datePattern)} ') ";

                querySaida += $@" AND EXISTS (SELECT TOP(1) 1
                                FROM T_TITULO TT
                                WHERE TT.TIT_STATUS <> 4 AND TT.NFI_CODIGO = N.NFI_CODIGO
                                AND CAST(TT.TIT_DATA_VENCIMENTO AS DATE) <= '{filtrosPesquisa.DataVencimentoFinal.ToString(datePattern)}') ";
            }

            if (filtrosPesquisa.CodigoSegmento > 0)
            {
                queryEntrada += " AND V.VSE_CODIGO = " + filtrosPesquisa.CodigoSegmento.ToString();
                querySaida += " AND 1 = 0 ";
            }

            if (filtrosPesquisa.CodigoEquipamento > 0)
            {
                queryEntrada += " AND EQP.EQP_CODIGO = " + filtrosPesquisa.CodigoEquipamento.ToString();
                querySaida += " AND 1 = 0 ";
            }

            if (!string.IsNullOrEmpty(filtrosPesquisa.EstadoEmitente) && filtrosPesquisa.EstadoEmitente != "0")
            {
                querySaida += " AND LE.UF_SIGLA = '" + filtrosPesquisa.EstadoEmitente + "'";
                queryEntrada += " AND L.UF_SIGLA = '" + filtrosPesquisa.EstadoEmitente + "'";
            }

            if (filtrosPesquisa.CodigoVeiculo > 0)
            {
                querySaida += " AND E.EMP_CODIGO IS NULL ";
                queryEntrada += " AND IT.VEI_CODIGO = " + filtrosPesquisa.CodigoVeiculo.ToString();
            }

            if ((int)filtrosPesquisa.StatusNotaEntrada > 0)
            {
                querySaida += " AND E.EMP_CODIGO IS NULL ";
                queryEntrada += " AND N.TDE_SITUACAO = " + filtrosPesquisa.StatusNotaEntrada.ToString("d");
            }

            if (filtrosPesquisa.NumeroInicial > 0 && filtrosPesquisa.NumeroFinal == 0)
            {
                querySaida += " AND N.NFI_NUMERO = " + filtrosPesquisa.NumeroInicial.ToString();
                queryEntrada += " AND N.TDE_NUMERO_LONG = " + filtrosPesquisa.NumeroInicial.ToString();
            }
            else if (filtrosPesquisa.NumeroInicial > 0 && filtrosPesquisa.NumeroFinal > 0)
            {
                querySaida += " AND N.NFI_NUMERO >= " + filtrosPesquisa.NumeroInicial.ToString() + " AND N.NFI_NUMERO <= " + filtrosPesquisa.NumeroFinal.ToString();
                queryEntrada += " AND N.TDE_NUMERO_LONG >= " + filtrosPesquisa.NumeroInicial.ToString() + " AND N.TDE_NUMERO_LONG <= " + filtrosPesquisa.NumeroFinal.ToString();
            }
            else if (filtrosPesquisa.NumeroInicial == 0 && filtrosPesquisa.NumeroFinal > 0)
            {
                querySaida += " AND N.NFI_NUMERO = " + filtrosPesquisa.NumeroFinal.ToString();
                queryEntrada += " AND N.TDE_NUMERO_LONG = " + filtrosPesquisa.NumeroFinal.ToString();
            }

            if (filtrosPesquisa.Serie > 0)
            {
                querySaida += " AND ES.ESE_NUMERO = " + filtrosPesquisa.Serie.ToString();
                queryEntrada += " AND N.TDE_SERIE = " + filtrosPesquisa.Serie.ToString();
            }

            if (filtrosPesquisa.CodigoServico > 0)
            {
                querySaida += " AND IT.SER_CODIGO = " + filtrosPesquisa.CodigoServico.ToString();
                queryEntrada += " AND IT.PRO_CODIGO IS NULL";
            }

            if (filtrosPesquisa.CodigosTipoMovimento != null && filtrosPesquisa.CodigosTipoMovimento.Count > 0)
            {
                querySaida += " AND 1 = 0";
                queryEntrada += " and IT.TIM_CODIGO in (" + string.Join(",", filtrosPesquisa.CodigosTipoMovimento) + ")";
            }

            if (filtrosPesquisa.CodigosModeloDocumentoFiscal != null && filtrosPesquisa.CodigosModeloDocumentoFiscal.Count > 0)
            {
                querySaida += " and M.MOD_CODIGO in (" + string.Join(",", filtrosPesquisa.CodigosModeloDocumentoFiscal) + ")";
                queryEntrada += " and M.MOD_CODIGO in (" + string.Join(",", filtrosPesquisa.CodigosModeloDocumentoFiscal) + ")";
            }
            if (filtrosPesquisa.CodigoModelo > 0)
            {
                queryEntrada += " AND M.MOD_CODIGO = " + filtrosPesquisa.CodigoModelo.ToString();
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroModeloNF))
            {
                querySaida += " AND N.NFI_MODELO = '" + filtrosPesquisa.NumeroModeloNF + "'";
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Chave))
            {
                querySaida += " AND N.NFI_CHAVE LIKE '%" + filtrosPesquisa.Chave + "%'";
                queryEntrada += " AND N.TDE_CHAVE LIKE '%" + filtrosPesquisa.Chave + "%'";
            }

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
            {
                querySaida += " AND CAST(N.NFI_DATA_EMISSAO AS DATE) >= '" + filtrosPesquisa.DataInicial.ToString(datePattern) + "'";
                queryEntrada += " AND CAST(N.TDE_DATA_EMISSAO AS DATE) >= '" + filtrosPesquisa.DataInicial.ToString(datePattern) + "'";
            }

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
            {
                querySaida += " AND CAST(N.NFI_DATA_EMISSAO AS DATE) <= '" + filtrosPesquisa.DataFinal.ToString(datePattern) + "'";
                queryEntrada += " AND CAST(N.TDE_DATA_EMISSAO AS DATE) <= '" + filtrosPesquisa.DataFinal.ToString(datePattern) + "'";
            }

            if (filtrosPesquisa.DataEntradaInicial != DateTime.MinValue)
            {
                querySaida += " AND CAST(N.NFI_DATA_EMISSAO AS DATE) >= '" + filtrosPesquisa.DataEntradaInicial.ToString(datePattern) + "'";
                queryEntrada += " AND CAST(N.TDE_DATA_ENTRADA AS DATE) >= '" + filtrosPesquisa.DataEntradaInicial.ToString(datePattern) + "'";
            }

            if (filtrosPesquisa.DataEntradaFinal != DateTime.MinValue)
            {
                querySaida += " AND CAST(N.NFI_DATA_EMISSAO AS DATE) <= '" + filtrosPesquisa.DataEntradaFinal.ToString(datePattern) + "'";
                queryEntrada += " AND CAST(N.TDE_DATA_ENTRADA AS DATE) <= '" + filtrosPesquisa.DataEntradaFinal.ToString(datePattern) + "'";
            }

            if (filtrosPesquisa.DataFinalizacaoInicial != DateTime.MinValue)
            {
                querySaida += " AND CAST('' AS DATE) >= '" + filtrosPesquisa.DataFinalizacaoInicial.ToString(datePattern) + "'";
                queryEntrada += " AND CAST(N.TDE_DATA_FINALIZACAO AS DATE) >= '" + filtrosPesquisa.DataFinalizacaoInicial.ToString(datePattern) + "'";
            }

            if (filtrosPesquisa.DataFinalizacaoFinal != DateTime.MinValue)
            {
                querySaida += " AND CAST('' AS DATE) <= '" + filtrosPesquisa.DataFinalizacaoFinal.ToString(datePattern) + "'";
                queryEntrada += " AND CAST(N.TDE_DATA_FINALIZACAO AS DATE) <= '" + filtrosPesquisa.DataFinalizacaoFinal.ToString(datePattern) + "'";
            }

            if (filtrosPesquisa.NotasComDiferencaDeValorTabelaFornecedor)
            {
                querySaida += " AND 1 = 0 ";
                queryEntrada += " AND IT.TDI_VALOR_ABASTECIMENTO_COM_DIVERGENCIA = 1";
            }

            var filtroStatus = "";
            if (filtrosPesquisa.SituacaoFinanceiraNotaEntrada == StatusTitulo.Cancelado)
                filtroStatus = "Cancelado";
            else if (filtrosPesquisa.SituacaoFinanceiraNotaEntrada == StatusTitulo.EmAberto)
                filtroStatus = "Aberto";
            else if (filtrosPesquisa.SituacaoFinanceiraNotaEntrada == StatusTitulo.EmNegociacao)
                filtroStatus = "Renegociado";
            else if (filtrosPesquisa.SituacaoFinanceiraNotaEntrada == StatusTitulo.Quitada)
                filtroStatus = "Pago";

            if (!string.IsNullOrWhiteSpace(filtroStatus))
            {
                queryEntrada += $@" AND N.TDE_CODIGO in (SELECT TT.TDE_CODIGO FROM (
									    SELECT T.Situacao, T.TDE_CODIGO FROM (
									    SELECT
									    (CASE 
										    WHEN T.TIT_STATUS = 1 THEN 'Aberto' 
										    WHEN T.TIT_VALOR_PAGO < T.TIT_VALOR_ORIGINAL THEN 
											    CASE
												    WHEN ISNULL((SELECT COUNT(1) FROM T_TITULO TT WHERE TT.TDD_CODIGO = T.TDD_CODIGO AND TT.TIT_STATUS <> 3 AND TT.TIT_STATUS <> 4), 0) >= 1 THEN 'Renegociado' 
												    WHEN ISNULL((SELECT COUNT(1) FROM T_TITULO_BAIXA_AGRUPADO TBA JOIN T_TITULO_BAIXA_NEGOCIACAO TBN ON TBN.TIB_CODIGO = TBA.TIB_CODIGO JOIN T_TITULO TT ON TT.TBN_CODIGO = TBN.TBN_CODIGO WHERE TT.TIT_STATUS <> 3 AND TT.TIT_STATUS <> 4 AND TBA.TIT_CODIGO = T.TIT_CODIGO), 0) >= 1 THEN 'Renegociado' 
                                                    WHEN T.TIT_VALOR_PAGO = 0 AND T.TIT_STATUS = 3 THEN 'Renegociado' 
												    ELSE 'Pago' 
											    END
										    ELSE 'Pago' 
									    END) Situacao, D.TDE_CODIGO
									    FROM T_TITULO T
									    JOIN T_TMS_DOCUMENTO_ENTRADA_DUPLICATA D ON D.TDD_CODIGO = T.TDD_CODIGO
								        WHERE T.TIT_STATUS <> 4) AS T WHERE T.Situacao = '{filtroStatus}') AS TT)";

                querySaida += $@" AND N.NFI_CODIGO in (SELECT TT.NFI_CODIGO FROM (
								        SELECT T.Situacao, T.NFI_CODIGO FROM (
								        SELECT
								        (CASE 
								            WHEN T.TIT_STATUS = 1 THEN 'Aberto' 
								            WHEN T.TIT_VALOR_PAGO < T.TIT_VALOR_ORIGINAL THEN 
								                CASE
									                WHEN ISNULL((SELECT COUNT(1) FROM T_TITULO TT WHERE TT.NFI_CODIGO = N.NFI_CODIGO AND TT.TIT_STATUS <> 3 AND TT.TIT_STATUS <> 4), 0) >= 1 THEN 'Renegociado' 
                                                    WHEN ISNULL((SELECT COUNT(1) FROM T_TITULO_BAIXA_AGRUPADO TBA JOIN T_TITULO_BAIXA_NEGOCIACAO TBN ON TBN.TIB_CODIGO = TBA.TIB_CODIGO JOIN T_TITULO TT ON TT.TBN_CODIGO = TBN.TBN_CODIGO WHERE TT.TIT_STATUS <> 3 AND TT.TIT_STATUS <> 4 AND TBA.TIT_CODIGO = T.TIT_CODIGO), 0) >= 1 THEN 'Renegociado' 
                                                    WHEN T.TIT_VALOR_PAGO = 0 AND T.TIT_STATUS = 3 THEN 'Renegociado' 
									                ELSE 'Pago' 
								                END
								            ELSE 'Pago' 
								        END) Situacao, T.NFI_CODIGO
								        FROM T_TITULO T
								        WHERE T.NFI_CODIGO = N.NFI_CODIGO AND T.TIT_STATUS <> 4) AS T WHERE T.Situacao = '{filtroStatus}') AS TT)";
            }

            querySaida += " AND N.NFI_AMBIENTE = " + (int)filtrosPesquisa.TipoAmbiente;

            if (filtrosPesquisa.TipoMovimento == TipoEntradaSaida.Entrada)
                query = queryEntrada + queryParameters;
            else if (filtrosPesquisa.TipoMovimento == TipoEntradaSaida.Saida)
                query = querySaida + queryParameters;
            else
                query = " SELECT SUM(CONTADOR) CONTADOR FROM(" + queryEntrada + queryParameters + " UNION ALL " + querySaida + queryParameters + ") AS T";

            var sqlDinamico = new SQLDinamico(query,parametros);

            var nhQuery = sqlDinamico.CriarSQLQuery(this.SessionNHiBernate);

            return nhQuery.SetTimeout(60000).UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.NFe.PedidoNota> RelatorioPedidoNota(int codigoEmpresa, int codigoProduto, int codigoNota, int codigoPedido, double cnpjPessoa, Dominio.Enumeradores.StatusNFe statusNota, DateTime dataNotaInicial, DateTime dataNotaFinal, DateTime dataPedidoInicial, DateTime dataPedidoFinal, string propGrupo, string dirOrdenacaoGrupo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, bool paginar = true, bool todosCNPJdaRaizEmbarcador = false)
        {
            string queryPedido = "";
            if (codigoPedido > 0)
                queryPedido = " AND PV.PEV_CODIGO = " + codigoPedido.ToString();

            if (dataPedidoInicial != DateTime.MinValue)
                queryPedido += " AND PV.PEV_DATA_EMISSAO >= '" + dataPedidoInicial.ToString("MM/dd/yyyy") + "'";

            if (dataPedidoFinal != DateTime.MinValue)
                queryPedido += " AND PV.PEV_DATA_EMISSAO <= '" + dataPedidoFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";

            string query = @"   SELECT N.NFI_NUMERO NumeroNota,
                                ES.ESE_NUMERO Serie,
                                CASE
	                                WHEN N.NFI_STATUS = 1 THEN 'Em Digitação'
	                                WHEN N.NFI_STATUS = 2 THEN 'Inutilizado'
	                                WHEN N.NFI_STATUS = 3 THEN 'Cancelado'
	                                WHEN N.NFI_STATUS = 4 THEN 'Autorizado'
	                                WHEN N.NFI_STATUS = 5 THEN 'Denegado'
	                                WHEN N.NFI_STATUS = 6 THEN 'Rejeitado'
	                                WHEN N.NFI_STATUS = 7 THEN 'Em Processamento'
	                                WHEN N.NFI_STATUS = 8 THEN 'Aguardando Assinatura do XML'
	                                WHEN N.NFI_STATUS = 9 THEN 'Aguardando Cancelamento do XML'
	                                WHEN N.NFI_STATUS = 10 THEN 'Aguardando Inutilizacao do XML'
	                                WHEN N.NFI_STATUS = 11 THEN 'Aguardando Carta Correção do XML'
	                                ELSE ''
                                END DescricaoStatus,
                                N.NFI_DATA_EMISSAO DataNota,
                                C.CLI_NOME Pessoa,
                                C.CLI_CGCCPF CNPJPessoa,
                                N.NFI_VALOR_TOTAL_NOTA ValorNota,

                                SUBSTRING((SELECT DISTINCT ', ' + CAST(PV.PEV_NUMERO AS NVARCHAR(20))
                                FROM T_PEDIDO_VENDA PV
                                JOIN T_NOTA_FISCAL_PEDIDO NP ON NP.PEV_CODIGO = PV.PEV_CODIGO
                                WHERE NP.NFI_CODIGO = N.NFI_CODIGO AND NP.PEV_CODIGO IS NOT NULL " + queryPedido + @" FOR XML PATH('')), 3, 1000) AS NumerosPedido,

                                SUBSTRING((SELECT DISTINCT ', ' + CONVERT(NVARCHAR(20), PV.PEV_DATA_EMISSAO, 103)
                                FROM T_PEDIDO_VENDA PV
                                JOIN T_NOTA_FISCAL_PEDIDO NP ON NP.PEV_CODIGO = PV.PEV_CODIGO
                                WHERE NP.NFI_CODIGO = N.NFI_CODIGO AND NP.PEV_CODIGO IS NOT NULL " + queryPedido + @" FOR XML PATH('')), 3, 1000) AS DatasPedido,

                                SUBSTRING((SELECT DISTINCT ', ' + CAST(CASE
	                                WHEN PV.PEV_TIPO = 1 THEN 'Cotação'
	                                WHEN PV.PEV_TIPO = 2 THEN 'Pedido'
	                                ELSE 'Ordem de Serviço'
                                END AS NVARCHAR(20))
                                FROM T_PEDIDO_VENDA PV
                                JOIN T_NOTA_FISCAL_PEDIDO NP ON NP.PEV_CODIGO = PV.PEV_CODIGO
                                WHERE NP.NFI_CODIGO = N.NFI_CODIGO AND NP.PEV_CODIGO IS NOT NULL " + queryPedido + @" FOR XML PATH('')), 3, 1000) AS TiposPedido,

                                SUBSTRING((SELECT DISTINCT ', ' + CAST(F.FUN_NOME AS NVARCHAR(20))
                                FROM T_PEDIDO_VENDA PV
                                JOIN T_NOTA_FISCAL_PEDIDO NP ON NP.PEV_CODIGO = PV.PEV_CODIGO
                                JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = PV.FUN_CODIGO
                                WHERE NP.NFI_CODIGO = N.NFI_CODIGO AND NP.PEV_CODIGO IS NOT NULL " + queryPedido + @" FOR XML PATH('')), 3, 1000) AS FuncionariosPedido,

                                (SELECT SUM(PV.PEV_VALOR_TOTAL)
                                FROM T_PEDIDO_VENDA PV
                                JOIN T_NOTA_FISCAL_PEDIDO NP ON NP.PEV_CODIGO = PV.PEV_CODIGO
                                WHERE NP.NFI_CODIGO = N.NFI_CODIGO AND NP.PEV_CODIGO IS NOT NULL " + queryPedido + @") ValorPedidos

                                FROM T_NOTA_FISCAL N
                                JOIN T_CLIENTE C ON C.CLI_CGCCPF = N.CLI_CGCCPF
                                JOIN T_EMPRESA_SERIE ES ON ES.ESE_CODIGO = N.ESE_CODIGO
                                JOIN T_EMPRESA E ON E.EMP_CODIGO = N.EMP_CODIGO
                                WHERE 1 = 1 ";

            if (codigoProduto == 0 || !string.IsNullOrWhiteSpace(queryPedido))
                query += @" AND N.NFI_CODIGO IN 
                            (SELECT DISTINCT NFI_CODIGO FROM T_NOTA_FISCAL_PEDIDO NP
                            JOIN T_PEDIDO_VENDA PV ON PV.PEV_CODIGO = NP.PEV_CODIGO
                            WHERE 1 = 1 " + queryPedido + ") ";

            if (codigoProduto > 0)
                query += @" AND N.NFI_CODIGO IN
                            (SELECT DISTINCT NP.NFI_CODIGO FROM T_NOTA_FISCAL_PEDIDO NP
                            JOIN T_PEDIDO_VENDA PV ON PV.PEV_CODIGO = NP.PEV_CODIGO
                            JOIN T_NOTA_FISCAL_PRODUTOS N ON N.NFI_CODIGO = NP.NFI_CODIGO
                            WHERE N.PRO_CODIGO = " + codigoProduto.ToString() + ") ";

            if (codigoEmpresa > 0)
                query += " AND E.EMP_CODIGO = " + codigoEmpresa.ToString();

            if (codigoNota > 0)
                query += " AND N.NFI_CODIGO = " + codigoNota.ToString();

            if (cnpjPessoa > 0)
                query += " AND C.CLI_CGCCPF = '" + cnpjPessoa.ToString() + "'";

            if (dataNotaInicial != DateTime.MinValue)
                query += " AND N.NFI_DATA_EMISSAO >= '" + dataNotaInicial.ToString("MM/dd/yyyy") + "'";

            if (dataNotaFinal != DateTime.MinValue)
                query += " AND N.NFI_DATA_EMISSAO <= '" + dataNotaFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";

            if (statusNota > 0)
                query += " AND N.NFI_STATUS = " + (int)statusNota;

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

            if (paginar)
                query += " OFFSET " + inicioRegistros + " ROWS FETCH FIRST " + maximoRegistros + " ROWS ONLY";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.NFe.PedidoNota)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.NFe.PedidoNota>();
        }

        public int ContarRelatorioPedidoNota(int codigoEmpresa, int codigoProduto, int codigoNota, int codigoPedido, double cnpjPessoa, Dominio.Enumeradores.StatusNFe statusNota, DateTime dataNotaInicial, DateTime dataNotaFinal, DateTime dataPedidoInicial, DateTime dataPedidoFinal)
        {
            string queryPedido = "";
            if (codigoPedido > 0)
                queryPedido = " AND PV.PEV_CODIGO = " + codigoPedido.ToString();

            if (dataPedidoInicial != DateTime.MinValue)
                queryPedido += " AND PV.PEV_DATA_EMISSAO >= '" + dataPedidoInicial.ToString("MM/dd/yyyy") + "'";

            if (dataPedidoFinal != DateTime.MinValue)
                queryPedido += " AND PV.PEV_DATA_EMISSAO <= '" + dataPedidoFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";

            string query = @"   SELECT COUNT(0) as CONTADOR
	
                                FROM T_NOTA_FISCAL N
                                JOIN T_CLIENTE C ON C.CLI_CGCCPF = N.CLI_CGCCPF
                                JOIN T_EMPRESA_SERIE ES ON ES.ESE_CODIGO = N.ESE_CODIGO
                                JOIN T_EMPRESA E ON E.EMP_CODIGO = N.EMP_CODIGO
                                WHERE 1 = 1 ";

            if (codigoProduto == 0 || !string.IsNullOrWhiteSpace(queryPedido))
                query += @" AND N.NFI_CODIGO IN 
                            (SELECT DISTINCT NFI_CODIGO FROM T_NOTA_FISCAL_PEDIDO NP
                            JOIN T_PEDIDO_VENDA PV ON PV.PEV_CODIGO = NP.PEV_CODIGO
                            WHERE 1 = 1 " + queryPedido + ") ";

            if (codigoProduto > 0)
                query += @" AND N.NFI_CODIGO IN
                            (SELECT DISTINCT NP.NFI_CODIGO FROM T_NOTA_FISCAL_PEDIDO NP
                            JOIN T_PEDIDO_VENDA PV ON PV.PEV_CODIGO = NP.PEV_CODIGO
                            JOIN T_NOTA_FISCAL_PRODUTOS N ON N.NFI_CODIGO = NP.NFI_CODIGO
                            WHERE N.PRO_CODIGO = " + codigoProduto.ToString() + ") ";

            if (codigoEmpresa > 0)
                query += " AND E.EMP_CODIGO = " + codigoEmpresa.ToString();

            if (codigoNota > 0)
                query += " AND N.NFI_CODIGO = " + codigoNota.ToString();

            if (cnpjPessoa > 0)
                query += " AND C.CLI_CGCCPF = '" + cnpjPessoa.ToString() + "'";

            if (dataNotaInicial != DateTime.MinValue)
                query += " AND N.NFI_DATA_EMISSAO >= '" + dataNotaInicial.ToString("MM/dd/yyyy") + "'";

            if (dataNotaFinal != DateTime.MinValue)
                query += " AND N.NFI_DATA_EMISSAO <= '" + dataNotaFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";

            if (statusNota > 0)
                query += " AND N.NFI_STATUS = " + (int)statusNota;

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.UniqueResult<int>();
        }

        public int ContarConsultaRelatorioNFesCargas(Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNFe filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consultaNotaFiscal = new ConsultaNotaFiscal(somenteRegistrosDistintos: false).ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaNotaFiscal.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.NFe.NFes> RelatorioNFesCargas(Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNFe filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaNotaFiscal = new ConsultaNotaFiscal(somenteRegistrosDistintos: false).ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaNotaFiscal.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.NFe.NFes)));

            return consultaNotaFiscal.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.NFe.NFes>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.NFe.Notas> RelatorioNotas(Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNotas filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            string query = "", queryParameters = "", queryEntrada = "", querySaida = "";
            if (filtrosPesquisa.CodigoEmpresa > 0)
                queryParameters += " AND E.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresa.ToString();

            if (filtrosPesquisa.CnpjPessoa > 0)
                queryParameters += " AND C.CLI_CGCCPF = " + filtrosPesquisa.CnpjPessoa.ToString();

            if (filtrosPesquisa.CodigosNaturezaOperacao?.Count > 0)
                queryParameters += $" AND NAT.NAT_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosNaturezaOperacao)})";

            #region Separacao por propriedade selecionadas na tela (codigo sera revisto em outra tarefa)
            if (filtrosPesquisa.TipoMovimento == TipoEntradaSaida.Todos)
            {
                queryEntrada = @" SELECT N.TDE_NUMERO_LONG Numero,
                N.TDE_SERIE Serie,
                'Entrada' DescricaoTipo,
                N.TDE_DATA_EMISSAO DataEmissao,
                N.TDE_DATA_ENTRADA DataEntrada,
                C.CLI_NOME Pessoa,
                M.MOD_NUM Modelo,
                NAT.NAT_DESCRICAO NaturezaOperacao,
                CASE
                    WHEN N.TDE_CHAVE = '' OR N.TDE_CHAVE IS NULL THEN CAST(N.TDE_CODIGO AS VARCHAR(50))
                    ELSE N.TDE_CHAVE
                END Chave,
                CASE
	                WHEN N.TDE_SITUACAO = 2 THEN 'Cancelado'
	                WHEN N.TDE_SITUACAO = 3 THEN 'Finalizado'
                    WHEN N.TDE_SITUACAO = 4 THEN 'Anulado'
	                ELSE 'Aberto'
                END DescricaoStatus,
                N.TDE_VALOR_TOTAL ValorTotal,

                CAST(CF.CFO_CFOP AS VARCHAR(20)) + '.' + ISNULL(CF.CFO_EXTENSAO, '') CFOP,
                N.TDE_VALOR_TOTAL_ICMS ValorICMS,
                N.TDE_VALOR_TOTAL_ICMS_ST ValorICMSST,
                N.TDE_VALOR_TOTAL_PIS ValorPIS,
                N.TDE_VALOR_TOTAL_COFINS ValorCOFINS,
                N.TDE_VALOR_TOTAL_IPI ValorIPI,

                N.TDE_VALOR_TOTAL_RETENCAO_PIS RetencaoPIS,
                N.TDE_VALOR_TOTAL_RETENCAO_COFINS RetencaoCOFNIS,
                N.TDE_VALOR_TOTAL_RETENCAO_INSS RetencaoINSS,
                N.TDE_VALOR_TOTAL_RETENCAO_IPI RetencaoIPI,
                N.TDE_VALOR_TOTAL_RETENCAO_CSLL RetencaoCSLL,
                N.TDE_VALOR_TOTAL_RETENCAO_OUTRAS RetencaoOUTRAS,
                N.TDE_VALOR_TOTAL_RETENCAO_IR RetencaoIR,
                N.TDE_VALOR_TOTAL_RETENCAO_ISS RetencaoISS,

                V.VEI_PLACA Veiculo,
                V.VEI_CODIGO CodigoVeiculo,
                E.EMP_RAZAO Empresa,
                E.EMP_CODIGO CodigoEmpresa,

                CAST(SUBSTRING((SELECT DISTINCT ', ' + 
                CASE 
	                WHEN T.TIT_STATUS = 1 THEN 'Aberto' 
	                WHEN T.TIT_VALOR_PAGO < T.TIT_VALOR_ORIGINAL THEN 
		                CASE
			                WHEN ISNULL((SELECT COUNT(1) FROM T_TITULO TT WHERE TT.TIT_NUMERO_DOCUMENTO_TITULO_ORIGINAL LIKE '%'+CAST(DE.TDE_NUMERO_LONG AS VARCHAR(20))+'%' AND TT.CLI_CGCCPF = DE.CLI_CGCCPF AND TT.TIT_STATUS <> 3 AND TT.TIT_STATUS <> 4), 0) >= 1 THEN 'Renegociado' 
                            WHEN ISNULL((SELECT COUNT(1) FROM T_TITULO_BAIXA_AGRUPADO TBA JOIN T_TITULO_BAIXA_NEGOCIACAO TBN ON TBN.TIB_CODIGO = TBA.TIB_CODIGO JOIN T_TITULO TT ON TT.TBN_CODIGO = TBN.TBN_CODIGO WHERE TT.TIT_STATUS <> 3 AND TT.TIT_STATUS <> 4 AND TBA.TIT_CODIGO = T.TIT_CODIGO), 0) >= 1 THEN 'Renegociado' 
			                ELSE 'Pago' 
		                END
	                ELSE 'Pago' 
                END
                FROM T_TITULO T
                JOIN T_TMS_DOCUMENTO_ENTRADA_DUPLICATA D ON D.TDD_CODIGO = T.TDD_CODIGO
                JOIN T_TMS_DOCUMENTO_ENTRADA DE ON DE.TDE_CODIGO = D.TDE_CODIGO
                WHERE T.TIT_STATUS <> 4 AND D.TDE_CODIGO = N.TDE_CODIGO FOR XML PATH('')), 3, 1000) AS VARCHAR(2000)) SituacaoFinanceiraNota,

                CAST(SUBSTRING((SELECT DISTINCT ', ' +  CONVERT(VARCHAR, TT.TIT_DATA_VENCIMENTO, 103) 
                FROM T_TITULO TT
                WHERE TT.TIT_NUMERO_DOCUMENTO_TITULO_ORIGINAL LIKE '%'+CAST(N.TDE_NUMERO_LONG AS VARCHAR(20))+'%'
                AND TT.CLI_CGCCPF = N.CLI_CGCCPF AND TT.TIT_STATUS <> 4
                AND NOT (TT.TIT_VALOR_PAGO = 0 AND TT.TIT_DATA_LIQUIDACAO IS NOT NULL AND TT.TIT_STATUS = 3) FOR XML PATH('')), 3, 1000) AS VARCHAR(2000)) DataVencimento,
	
                CAST(SUBSTRING((SELECT DISTINCT ', ' +  CONVERT(VARCHAR, TT.TIT_DATA_VENCIMENTO, 103) 
                FROM T_TITULO TT
                WHERE TT.TIT_NUMERO_DOCUMENTO_TITULO_ORIGINAL LIKE '%'+CAST(N.TDE_NUMERO_LONG AS VARCHAR(20))+'%'
                AND TT.CLI_CGCCPF = N.CLI_CGCCPF AND TT.TIT_STATUS = 3
                AND NOT (TT.TIT_VALOR_PAGO = 0 AND TT.TIT_DATA_LIQUIDACAO IS NOT NULL AND TT.TIT_STATUS = 3) FOR XML PATH('')), 3, 1000) AS VARCHAR(2000)) DataPagamento,

                CAST(SUBSTRING((SELECT DISTINCT ', ' +  Resultado.CRE_DESCRICAO
				FROM T_CENTRO_RESULTADO Resultado
				JOIN T_TMS_DOCUMENTO_ENTRADA_CENTRO_RESULTADO Centro on Centro.CRE_CODIGO = Resultado.CRE_CODIGO
				WHERE Centro.TDE_CODIGO = N.TDE_CODIGO FOR XML PATH('')), 3, 1000) AS VARCHAR(2000)) CentroResultado,

                L.UF_SIGLA EstadoPessoa,
                C.CLI_CGCCPF CPFCNPJPessoa,
                CASE
	                WHEN V.VEI_TIPO = 'P' THEN 'Próprio'
	                WHEN V.VEI_TIPO = 'T' THEN 'Terceiro'
	                ELSE ''
                END TipoVeiculo,
                C.CLI_FISJUR TipoCliente,
                L.LOC_DESCRICAO Cidade,
                ISNULL(N.TDE_BASE_ST_RETIDO, 0) BaseSTRetido,
                ISNULL(N.TDE_VALOR_ST_RETIDO, 0) ValorSTRetido,
                N.TDE_CODIGO CodigoNota,
                CAST(SUBSTRING((SELECT DISTINCT ', ' +  EQP.EQP_DESCRICAO
				FROM T_VEICULO_EQUIPAMENTO VEQ
				JOIN T_EQUIPAMENTO EQP ON VEQ.EQP_CODIGO = EQP.EQP_CODIGO
				WHERE VEQ.VEI_CODIGO = V.VEI_CODIGO FOR XML PATH('')), 3, 1000) AS VARCHAR(2000)) Equipamento,		
                SEG.VSE_DESCRICAO Segmento,
                N.TDE_VALOR_TOTAL_IMPOSTOS_FORA ValorImpostosFora,
                OperadorLancamento.FUN_NOME OperadorLancamentoDocumento,
                OperadorFinaliza.FUN_NOME OperadorFinalizouDocumento,
                E.EMP_CNPJ CNPJFilial,
                OS.OSE_NUMERO OrdemServico,
	            OC.ORC_NUMERO OrdemCompra,   
                ISNULL(FORMAT(TDE_DATA_FINALIZACAO,'dd/MM/yyyy HH:mm:ss'),'') DataFinalizacao,
                CASE
                    WHEN N.TDE_DOCUMENTO_FINALIZADO_AUTOMATICAMENTE = 1 THEN 'Sim'
                    ELSE 'Não'
                END DocFinalizadoAutomaticamente, 
                N.TDE_OBSERVACAO Observacao,
                N.TDE_MOTIVO MotivoCancelamento,

                (SELECT ISNULL((SELECT SUM(Abast.ABA_LITROS) FROM T_TMS_DOCUMENTO_ENTRADA_ITEM Item
                join T_TMS_DOCUMENTO_ENTRADA_ITEM_ABASTECIMENTO ItemAbast on ItemAbast.TDI_CODIGO = Item.TDI_CODIGO
                JOIN T_ABASTECIMENTO Abast on Abast.ABA_CODIGO = ItemAbast.ABA_CODIGO
                WHERE Item.TDE_CODIGO = N.TDE_CODIGO), 0)) TotalLitrosAbastecimento,

                ISNULL((SELECT SUM(Abast.ABA_VALOR_UN) FROM T_TMS_DOCUMENTO_ENTRADA_ITEM Item
                join T_TMS_DOCUMENTO_ENTRADA_ITEM_ABASTECIMENTO ItemAbast on ItemAbast.TDI_CODIGO = Item.TDI_CODIGO
                JOIN T_ABASTECIMENTO Abast on Abast.ABA_CODIGO = ItemAbast.ABA_CODIGO
                WHERE Item.TDE_CODIGO = N.TDE_CODIGO), 0) ValorTotalAbastecimento,
                CategoriaPessoa.CTP_DESCRICAO CategoriaPessoa,
                SL.SLC_DESCRICAO StatusLancamento


                FROM T_TMS_DOCUMENTO_ENTRADA N
                JOIN T_CLIENTE C ON C.CLI_CGCCPF = N.CLI_CGCCPF
                JOIN T_LOCALIDADES L ON L.LOC_CODIGO = C.LOC_CODIGO								
                LEFT OUTER JOIN T_MODDOCFISCAL M ON M.MOD_CODIGO = N.MOD_CODIGO
                LEFT OUTER JOIN T_NATUREZAOPERACAO NAT ON NAT.NAT_CODIGO = N.NAT_CODIGO
                LEFT OUTER JOIN T_CFOP CF ON CF.CFO_CODIGO = N.CFO_CODIGO
                JOIN T_EMPRESA E ON E.EMP_CODIGO = N.EMP_CODIGO
                JOIN T_LOCALIDADES LE ON LE.LOC_CODIGO = E.LOC_CODIGO
                LEFT OUTER JOIN T_VEICULO V ON V.VEI_CODIGO = N.VEI_CODIGO
                LEFT OUTER JOIN T_VEICULO_SEGMENTO SEG ON SEG.VSE_CODIGO = V.VSE_CODIGO                
                LEFT OUTER JOIN T_FUNCIONARIO OperadorFinaliza ON OperadorFinaliza.FUN_CODIGO = N.TDE_OPERADOR_FINALIZA_DOCUMENTO
                LEFT OUTER JOIN T_FUNCIONARIO OperadorLancamento ON OperadorLancamento.FUN_CODIGO = N.TDE_OPERADOR_LANCAMENTO_DOCUMENTO
                LEFT OUTER JOIN T_FROTA_ORDEM_SERVICO OS ON N.OSE_CODIGO = OS.OSE_CODIGO
		        LEFT OUTER JOIN T_ORDEM_COMPRA OC ON N.ORC_CODIGO = OC.ORC_CODIGO
		        LEFT JOIN T_CATEGORIA_PESSOA CategoriaPessoa ON CategoriaPessoa.CTP_CODIGO = C.CTP_CODIGO
                LEFT OUTER JOIN T_SITUACAO_LANCAMENTO_DOCUMENTO_ENTRADA SL ON SL.SLC_CODIGO = N.SLC_CODIGO

                WHERE 1 = 1";
            }
            else {
                queryEntrada = @" SELECT N.TDE_NUMERO_LONG Numero,
                N.TDE_SERIE Serie,
                'Entrada' DescricaoTipo,
                N.TDE_DATA_EMISSAO DataEmissao,
                N.TDE_DATA_ENTRADA DataEntrada,
                C.CLI_NOME Pessoa,
                M.MOD_NUM Modelo,
                NAT.NAT_DESCRICAO NaturezaOperacao ";


                foreach (Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento propriedade in propriedades)
                {
                    if (!queryEntrada.Contains("ValorTotal") && (propriedade.Propriedade == "ValorTotal" || tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS))
                    {
                        queryEntrada += @"  ,CASE
                                    WHEN N.TDE_CHAVE = '' OR N.TDE_CHAVE IS NULL THEN CAST(N.TDE_CODIGO AS VARCHAR(50))
                                    ELSE N.TDE_CHAVE
                                         END Chave,
                                         CASE
	                                         WHEN N.TDE_SITUACAO = 2 THEN 'Cancelado'
	                                         WHEN N.TDE_SITUACAO = 3 THEN 'Finalizado'
                                             WHEN N.TDE_SITUACAO = 4 THEN 'Anulado'
	                                         ELSE 'Aberto'
                                         END DescricaoStatus,
                                         N.TDE_VALOR_TOTAL ValorTotal";
                    }
                    if (!queryEntrada.Contains("CFOP") && (propriedade.Propriedade == "CFOP" || tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS))
                    {
                        queryEntrada += @"  ,CAST(CF.CFO_CFOP AS VARCHAR(20)) + '.' + ISNULL(CF.CFO_EXTENSAO, '') CFOP ";
                    }
                    if (!queryEntrada.Contains("SituacaoFinanceiraNota") && (propriedade.Propriedade == "SituacaoFinanceiraNota" || tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS))
                    {
                        queryEntrada += @"          ,CAST(SUBSTRING((SELECT DISTINCT ', ' + 
                                                CASE 
	                                                WHEN T.TIT_STATUS = 1 THEN 'Aberto' 
	                                                WHEN T.TIT_VALOR_PAGO < T.TIT_VALOR_ORIGINAL THEN 
		                                                CASE
			                                                WHEN ISNULL((SELECT COUNT(1) FROM T_TITULO TT WHERE TT.TIT_NUMERO_DOCUMENTO_TITULO_ORIGINAL LIKE '%'+CAST(DE.TDE_NUMERO_LONG AS VARCHAR(20))+'%' AND TT.CLI_CGCCPF = DE.CLI_CGCCPF AND TT.TIT_STATUS <> 3 AND TT.TIT_STATUS <> 4), 0) >= 1 THEN 'Renegociado' 
                                                            WHEN ISNULL((SELECT COUNT(1) FROM T_TITULO_BAIXA_AGRUPADO TBA JOIN T_TITULO_BAIXA_NEGOCIACAO TBN ON TBN.TIB_CODIGO = TBA.TIB_CODIGO JOIN T_TITULO TT ON TT.TBN_CODIGO = TBN.TBN_CODIGO WHERE TT.TIT_STATUS <> 3 AND TT.TIT_STATUS <> 4 AND TBA.TIT_CODIGO = T.TIT_CODIGO), 0) >= 1 THEN 'Renegociado' 
			                                                ELSE 'Pago' 
		                                                END
	                                                ELSE 'Pago' 
                                                END
                                                FROM T_TITULO T
                                                JOIN T_TMS_DOCUMENTO_ENTRADA_DUPLICATA D ON D.TDD_CODIGO = T.TDD_CODIGO
                                                JOIN T_TMS_DOCUMENTO_ENTRADA DE ON DE.TDE_CODIGO = D.TDE_CODIGO
                                                WHERE T.TIT_STATUS <> 4 AND D.TDE_CODIGO = N.TDE_CODIGO FOR XML PATH('')), 3, 1000) AS VARCHAR(2000)) SituacaoFinanceiraNota ";
                    }
                    if (!queryEntrada.Contains("DataVencimento") && (propriedade.Propriedade == "DataVencimento" || tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS))
                    {
                        queryEntrada += @"   ,CAST(SUBSTRING((SELECT DISTINCT ', ' +  CONVERT(VARCHAR, TT.TIT_DATA_VENCIMENTO, 103) 
                                            FROM T_TITULO TT
                                            WHERE TT.TIT_NUMERO_DOCUMENTO_TITULO_ORIGINAL LIKE '%'+CAST(N.TDE_NUMERO_LONG AS VARCHAR(20))+'%'
                                            AND TT.CLI_CGCCPF = N.CLI_CGCCPF AND TT.TIT_STATUS <> 4
                                            AND NOT (TT.TIT_VALOR_PAGO = 0 AND TT.TIT_DATA_LIQUIDACAO IS NOT NULL AND TT.TIT_STATUS = 3) FOR XML PATH('')), 3, 1000) AS VARCHAR(2000)) DataVencimento ";
                    }

                    if (!queryEntrada.Contains("DataPagamento") && (propriedade.Propriedade == "DataPagamento" || tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS))
                    {
                        queryEntrada += @"  , CAST(SUBSTRING((SELECT DISTINCT ', ' +  CONVERT(VARCHAR, TT.TIT_DATA_VENCIMENTO, 103) 
                                            FROM T_TITULO TT
                                            WHERE TT.TIT_NUMERO_DOCUMENTO_TITULO_ORIGINAL LIKE '%'+CAST(N.TDE_NUMERO_LONG AS VARCHAR(20))+'%'
                                            AND TT.CLI_CGCCPF = N.CLI_CGCCPF AND TT.TIT_STATUS = 3
                                            AND NOT (TT.TIT_VALOR_PAGO = 0 AND TT.TIT_DATA_LIQUIDACAO IS NOT NULL AND TT.TIT_STATUS = 3) FOR XML PATH('')), 3, 1000) AS VARCHAR(2000)) DataPagamento ";
                    }

                    if (!queryEntrada.Contains("CentroResultado") && (propriedade.Propriedade == "CentroResultado" || tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS))
                    {
                        queryEntrada += @" ,CAST(SUBSTRING((SELECT DISTINCT ', ' +  Resultado.CRE_DESCRICAO
				                         FROM T_CENTRO_RESULTADO Resultado
				                         JOIN T_TMS_DOCUMENTO_ENTRADA_CENTRO_RESULTADO Centro on Centro.CRE_CODIGO = Resultado.CRE_CODIGO
				                         WHERE Centro.TDE_CODIGO = N.TDE_CODIGO FOR XML PATH('')), 3, 1000) AS VARCHAR(2000))  CentroResultado ";
                    }

                }

                queryEntrada += @"
                ,N.TDE_VALOR_TOTAL_ICMS ValorICMS,
                N.TDE_VALOR_TOTAL_ICMS_ST ValorICMSST,
                N.TDE_VALOR_TOTAL_PIS ValorPIS,
                N.TDE_VALOR_TOTAL_COFINS ValorCOFINS,
                N.TDE_VALOR_TOTAL_IPI ValorIPI,

                N.TDE_VALOR_TOTAL_RETENCAO_PIS RetencaoPIS,
                N.TDE_VALOR_TOTAL_RETENCAO_COFINS RetencaoCOFNIS,
                N.TDE_VALOR_TOTAL_RETENCAO_INSS RetencaoINSS,
                N.TDE_VALOR_TOTAL_RETENCAO_IPI RetencaoIPI,
                N.TDE_VALOR_TOTAL_RETENCAO_CSLL RetencaoCSLL,
                N.TDE_VALOR_TOTAL_RETENCAO_OUTRAS RetencaoOUTRAS,
                N.TDE_VALOR_TOTAL_RETENCAO_IR RetencaoIR,
                N.TDE_VALOR_TOTAL_RETENCAO_ISS RetencaoISS,

                V.VEI_PLACA Veiculo,
                V.VEI_CODIGO CodigoVeiculo,
                E.EMP_RAZAO Empresa,
                E.EMP_CODIGO CodigoEmpresa,
                L.UF_SIGLA EstadoPessoa,
                C.CLI_CGCCPF CPFCNPJPessoa,
                CASE
	                WHEN V.VEI_TIPO = 'P' THEN 'Próprio'
	                WHEN V.VEI_TIPO = 'T' THEN 'Terceiro'
	                ELSE ''
                END TipoVeiculo,
                C.CLI_FISJUR TipoCliente,
                L.LOC_DESCRICAO Cidade,
                ISNULL(N.TDE_BASE_ST_RETIDO, 0) BaseSTRetido,
                ISNULL(N.TDE_VALOR_ST_RETIDO, 0) ValorSTRetido,
                N.TDE_CODIGO CodigoNota,
                CAST(SUBSTRING((SELECT DISTINCT ', ' +  EQP.EQP_DESCRICAO
				FROM T_VEICULO_EQUIPAMENTO VEQ
				JOIN T_EQUIPAMENTO EQP ON VEQ.EQP_CODIGO = EQP.EQP_CODIGO
				WHERE VEQ.VEI_CODIGO = V.VEI_CODIGO FOR XML PATH('')), 3, 1000) AS VARCHAR(2000)) Equipamento,		
                SEG.VSE_DESCRICAO Segmento,
                N.TDE_VALOR_TOTAL_IMPOSTOS_FORA ValorImpostosFora,
                OperadorLancamento.FUN_NOME OperadorLancamentoDocumento,
                OperadorFinaliza.FUN_NOME OperadorFinalizouDocumento,
                E.EMP_CNPJ CNPJFilial,
                OS.OSE_NUMERO OrdemServico,
	            OC.ORC_NUMERO OrdemCompra,   
                ISNULL(FORMAT(TDE_DATA_FINALIZACAO,'dd/MM/yyyy HH:mm:ss'),'') DataFinalizacao,
                CASE
                    WHEN N.TDE_DOCUMENTO_FINALIZADO_AUTOMATICAMENTE = 1 THEN 'Sim'
                    ELSE 'Não'
                END DocFinalizadoAutomaticamente, 
                N.TDE_OBSERVACAO Observacao,
                N.TDE_MOTIVO MotivoCancelamento,

                (SELECT ISNULL((SELECT SUM(Abast.ABA_LITROS) FROM T_TMS_DOCUMENTO_ENTRADA_ITEM Item
                join T_TMS_DOCUMENTO_ENTRADA_ITEM_ABASTECIMENTO ItemAbast on ItemAbast.TDI_CODIGO = Item.TDI_CODIGO
                JOIN T_ABASTECIMENTO Abast on Abast.ABA_CODIGO = ItemAbast.ABA_CODIGO
                WHERE Item.TDE_CODIGO = N.TDE_CODIGO), 0)) TotalLitrosAbastecimento,

                ISNULL((SELECT SUM(Abast.ABA_VALOR_UN) FROM T_TMS_DOCUMENTO_ENTRADA_ITEM Item
                join T_TMS_DOCUMENTO_ENTRADA_ITEM_ABASTECIMENTO ItemAbast on ItemAbast.TDI_CODIGO = Item.TDI_CODIGO
                JOIN T_ABASTECIMENTO Abast on Abast.ABA_CODIGO = ItemAbast.ABA_CODIGO
                WHERE Item.TDE_CODIGO = N.TDE_CODIGO), 0) ValorTotalAbastecimento,
                CategoriaPessoa.CTP_DESCRICAO CategoriaPessoa,
                SL.SLC_DESCRICAO StatusLancamento


                FROM T_TMS_DOCUMENTO_ENTRADA N
                JOIN T_CLIENTE C ON C.CLI_CGCCPF = N.CLI_CGCCPF
                JOIN T_LOCALIDADES L ON L.LOC_CODIGO = C.LOC_CODIGO								
                LEFT OUTER JOIN T_MODDOCFISCAL M ON M.MOD_CODIGO = N.MOD_CODIGO
                LEFT OUTER JOIN T_NATUREZAOPERACAO NAT ON NAT.NAT_CODIGO = N.NAT_CODIGO
                LEFT OUTER JOIN T_CFOP CF ON CF.CFO_CODIGO = N.CFO_CODIGO
                JOIN T_EMPRESA E ON E.EMP_CODIGO = N.EMP_CODIGO
                JOIN T_LOCALIDADES LE ON LE.LOC_CODIGO = E.LOC_CODIGO
                LEFT OUTER JOIN T_VEICULO V ON V.VEI_CODIGO = N.VEI_CODIGO
                LEFT OUTER JOIN T_VEICULO_SEGMENTO SEG ON SEG.VSE_CODIGO = V.VSE_CODIGO                
                LEFT OUTER JOIN T_FUNCIONARIO OperadorFinaliza ON OperadorFinaliza.FUN_CODIGO = N.TDE_OPERADOR_FINALIZA_DOCUMENTO
                LEFT OUTER JOIN T_FUNCIONARIO OperadorLancamento ON OperadorLancamento.FUN_CODIGO = N.TDE_OPERADOR_LANCAMENTO_DOCUMENTO
                LEFT OUTER JOIN T_FROTA_ORDEM_SERVICO OS ON N.OSE_CODIGO = OS.OSE_CODIGO
		        LEFT OUTER JOIN T_ORDEM_COMPRA OC ON N.ORC_CODIGO = OC.ORC_CODIGO
		        LEFT JOIN T_CATEGORIA_PESSOA CategoriaPessoa ON CategoriaPessoa.CTP_CODIGO = C.CTP_CODIGO
                LEFT OUTER JOIN T_SITUACAO_LANCAMENTO_DOCUMENTO_ENTRADA SL ON SL.SLC_CODIGO = N.SLC_CODIGO

                WHERE 1 = 1   ";
            }
           
            #endregion 
           

            querySaida = @"SELECT N.NFI_NUMERO Numero,
                            CAST(ES.ESE_NUMERO AS VARCHAR(20)) Serie,
                            'Saída' DescricaoTipo,
                            N.NFI_DATA_EMISSAO DataEmissao,
                            N.NFI_DATA_EMISSAO DataEntrada,
                            C.CLI_NOME Pessoa,
                            N.NFI_MODELO Modelo,
                            NAT.NAT_DESCRICAO NaturezaOperacao,
                            CASE
                                WHEN N.NFI_CHAVE = '' OR N.NFI_CHAVE IS NULL THEN CAST((N.NFI_CODIGO * -1) AS VARCHAR(50))
                                ELSE N.NFI_CHAVE
                            END Chave,
                            CASE
	                            WHEN N.NFI_STATUS = 1 THEN 'Em Digitação'
	                            WHEN N.NFI_STATUS = 2 THEN 'Inutilizado'
	                            WHEN N.NFI_STATUS = 3 THEN 'Cancelado'
	                            WHEN N.NFI_STATUS = 4 THEN 'Autorizado'
	                            WHEN N.NFI_STATUS = 5 THEN 'Denegado'
	                            WHEN N.NFI_STATUS = 6 THEN 'Rejeitado'
	                            WHEN N.NFI_STATUS = 7 THEN 'Em Processamento'
	                            WHEN N.NFI_STATUS = 8 THEN 'Aguardando Assinatura do XML'
	                            WHEN N.NFI_STATUS = 9 THEN 'Aguardando Cancelamento do XML'
	                            WHEN N.NFI_STATUS = 10 THEN 'Aguardando Inutilizacao do XML'
	                            WHEN N.NFI_STATUS = 11 THEN 'Aguardando Carta Correção do XML'
	                            ELSE ''
                            END DescricaoStatus,
                            N.NFI_VALOR_TOTAL_NOTA ValorTotal,
                            CAST(CF.CFO_CFOP AS VARCHAR(20)) + '.' + ISNULL(CF.CFO_EXTENSAO, '') CFOP,
                            N.NFI_VALOR_ICMS ValorICMS,
                            N.NFI_VALOR_ICMS_ST ValorICMSST,
                            N.NFI_VALOR_PIS ValorPIS,
                            N.NFI_VALOR_COFINS ValorCOFINS,
                            N.NFI_VALOR_IPI ValorIPI,

                            0.0 RetencaoPIS,
                            0.0 RetencaoCOFNIS,
                            0.0 RetencaoINSS,
                            0.0 RetencaoIPI,
                            0.0 RetencaoCSLL,
                            0.0 RetencaoOUTRAS,
                            0.0 RetencaoIR,
                            0.0 RetencaoISS,
                            
                            '' Veiculo,
							0 CodigoVeiculo,
							E.EMP_RAZAO Empresa,
							E.EMP_CODIGO CodigoEmpresa,
                            '' SituacaoFinanceiraNota,
                            '' DataVencimento,
                            '' DataPagamento,
                            '' CentroResultado,
                            L.UF_SIGLA EstadoPessoa,
                            C.CLI_CGCCPF CPFCNPJPessoa,
                            ''  TipoVeiculo,
                            C.CLI_FISJUR TipoCliente,
                            L.LOC_DESCRICAO Cidade,
                            0.0 BaseSTRetido,
                            0.0 ValorSTRetido,
                            N.NFI_CODIGO CodigoNota,
                            '' Segmento, 
                            '' Equipamento,
							0.0 ValorImpostosFora,
                            '' OperadorLancamentoDocumento,
                            '' OperadorFinalizouDocumento,
                            E.EMP_CNPJ CNPJFilial,
                            0 OrdemServico,
	                        0 OrdemCompra,
                            '' DataFinalizacao,
                            'Não' DocFinalizadoAutomaticamente,
                            N.NFI_OBSERVACAO_NOTA Observacao,
                            '' MotivoCancelamento,
                            0.0 TotalLitrosAbastecimento,
                            0.0 ValorTotalAbastecimento,
                            CategoriaPessoa.CTP_DESCRICAO CategoriaPessoa,
                            '' StatusLancamento

                            FROM T_NOTA_FISCAL N
                            LEFT OUTER JOIN T_CLIENTE C ON C.CLI_CGCCPF = N.CLI_CGCCPF
                            LEFT OUTER JOIN T_LOCALIDADES L ON L.LOC_CODIGO = C.LOC_CODIGO	
                            JOIN T_EMPRESA_SERIE ES ON ES.ESE_CODIGO = N.ESE_CODIGO
                            LEFT OUTER JOIN T_NATUREZAOPERACAO NAT ON NAT.NAT_CODIGO = N.NAT_CODIGO                            
                            LEFT OUTER JOIN T_CFOP CF ON CF.CFO_CODIGO = NAT.CFO_CODIGO
                            JOIN T_EMPRESA E ON E.EMP_CODIGO = N.EMP_CODIGO
                            JOIN T_LOCALIDADES LE ON LE.LOC_CODIGO = E.LOC_CODIGO       
		                    LEFT JOIN T_CATEGORIA_PESSOA CategoriaPessoa ON CategoriaPessoa.CTP_CODIGO = C.CTP_CODIGO

                            WHERE 1 = 1 ";

            if (filtrosPesquisa.CodigoEmpresaFilial > 0)
            {
                querySaida += " AND E.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresaFilial.ToString();
                queryEntrada += " AND E.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresaFilial.ToString();
            }

            if (!string.IsNullOrEmpty(filtrosPesquisa.EstadoEmitente) && filtrosPesquisa.EstadoEmitente != "0")
            {
                querySaida += " AND LE.UF_SIGLA = '" + filtrosPesquisa.EstadoEmitente + "'";
                queryEntrada += " AND L.UF_SIGLA = '" + filtrosPesquisa.EstadoEmitente + "'";
            }

            if (filtrosPesquisa.CodigoVeiculo > 0)
            {
                querySaida += " AND E.EMP_CODIGO IS NULL ";
                queryEntrada += " AND V.VEI_CODIGO = " + filtrosPesquisa.CodigoVeiculo.ToString();
            }

            if (filtrosPesquisa.CodigoSegmento > 0)
            {
                queryEntrada += " AND V.VSE_CODIGO = " + filtrosPesquisa.CodigoSegmento.ToString();
                querySaida += " AND 1 = 0 ";
            }

            if (filtrosPesquisa.CodigoEquipamento > 0)
            {
                queryEntrada += " AND V.VEI_CODIGO IN (SELECT VEQ.VEI_CODIGO FROM T_VEICULO_EQUIPAMENTO VEQ LEFT OUTER JOIN T_EQUIPAMENTO EQP ON VEQ.EQP_CODIGO = EQP.EQP_CODIGO WHERE EQP.EQP_CODIGO = " + filtrosPesquisa.CodigoEquipamento.ToString() + " )"; // SQL-INJECTION-SAFE
                querySaida += " AND 1 = 0 ";
            }

            if ((int)filtrosPesquisa.StatusNotaEntrada > 0)
            {
                querySaida += " AND E.EMP_CODIGO IS NULL ";
                queryEntrada += " AND N.TDE_SITUACAO = " + filtrosPesquisa.StatusNotaEntrada.ToString("d");
            }

            if (filtrosPesquisa.NumeroInicial > 0 && filtrosPesquisa.NumeroFinal == 0)
            {
                querySaida += " AND N.NFI_NUMERO = " + filtrosPesquisa.NumeroInicial.ToString();
                queryEntrada += " AND N.TDE_NUMERO_LONG = " + filtrosPesquisa.NumeroInicial.ToString();
            }
            else if (filtrosPesquisa.NumeroInicial > 0 && filtrosPesquisa.NumeroFinal > 0)
            {
                querySaida += " AND N.NFI_NUMERO >= " + filtrosPesquisa.NumeroInicial.ToString() + " AND N.NFI_NUMERO <= " + filtrosPesquisa.NumeroFinal.ToString();
                queryEntrada += " AND N.TDE_NUMERO_LONG >= " + filtrosPesquisa.NumeroInicial.ToString() + " AND N.TDE_NUMERO_LONG <= " + filtrosPesquisa.NumeroFinal.ToString();
            }
            else if (filtrosPesquisa.NumeroInicial == 0 && filtrosPesquisa.NumeroFinal > 0)
            {
                querySaida += " AND N.NFI_NUMERO = " + filtrosPesquisa.NumeroFinal.ToString();
                queryEntrada += " AND N.TDE_NUMERO_LONG = " + filtrosPesquisa.NumeroFinal.ToString();
            }

            if (filtrosPesquisa.Serie > 0)
            {
                querySaida += " AND ES.ESE_NUMERO = " + filtrosPesquisa.Serie.ToString();
                queryEntrada += " AND N.TDE_SERIE = " + filtrosPesquisa.Serie.ToString();
            }

            if (filtrosPesquisa.CodigosModeloDocumentoFiscal != null && filtrosPesquisa.CodigosModeloDocumentoFiscal.Count > 0)
                queryEntrada += " and M.MOD_CODIGO in (" + string.Join(",", filtrosPesquisa.CodigosModeloDocumentoFiscal) + ")";
            if (filtrosPesquisa.CodigoModelo > 0)
            {
                queryEntrada += " AND M.MOD_CODIGO = " + filtrosPesquisa.CodigoModelo.ToString();
            }

            if (filtrosPesquisa.CodigosCentroResultado?.Count > 0)
            {
                queryEntrada += " AND N.TDE_CODIGO IN (SELECT Centro.TDE_CODIGO FROM T_TMS_DOCUMENTO_ENTRADA_CENTRO_RESULTADO Centro WHERE Centro.CRE_CODIGO in (" + string.Join(",", filtrosPesquisa.CodigosCentroResultado) + "))"; // SQL-INJECTION-SAFE
                querySaida += " AND 1 = 0";
            }

            if (filtrosPesquisa.OperadorLancamentoDocumento > 0)
            {
                queryEntrada += " AND N.TDE_OPERADOR_LANCAMENTO_DOCUMENTO = " + filtrosPesquisa.OperadorLancamentoDocumento.ToString();
                querySaida += " AND 1 = 0 ";
            }

            if (filtrosPesquisa.OperadorFinalizouDocumento > 0)
            {
                queryEntrada += " AND N.TDE_OPERADOR_FINALIZA_DOCUMENTO = " + filtrosPesquisa.OperadorFinalizouDocumento.ToString();
                querySaida += " AND 1 = 0 ";
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroModeloNF))
            {
                querySaida += " AND N.NFI_MODELO = '" + filtrosPesquisa.NumeroModeloNF + "'";
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Chave))
            {
                querySaida += " AND N.NFI_CHAVE LIKE '%" + filtrosPesquisa.Chave + "%'";
                queryEntrada += " AND N.TDE_CHAVE LIKE '%" + filtrosPesquisa.Chave + "%'";
            }

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
            {
                querySaida += " AND N.NFI_DATA_EMISSAO >= '" + filtrosPesquisa.DataInicial.ToString("MM/dd/yyyy") + "'";
                queryEntrada += " AND N.TDE_DATA_EMISSAO >= '" + filtrosPesquisa.DataInicial.ToString("MM/dd/yyyy") + "'";
            }

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
            {
                querySaida += " AND N.NFI_DATA_EMISSAO <= '" + filtrosPesquisa.DataFinal.ToString("MM/dd/yyyy 23:59:59") + "'";
                queryEntrada += " AND N.TDE_DATA_EMISSAO <= '" + filtrosPesquisa.DataFinal.ToString("MM/dd/yyyy 23:59:59") + "'";
            }

            if (filtrosPesquisa.DataEntradaInicial != DateTime.MinValue)
            {
                querySaida += " AND N.NFI_DATA_EMISSAO >= '" + filtrosPesquisa.DataEntradaInicial.ToString("MM/dd/yyyy") + "'";
                queryEntrada += " AND N.TDE_DATA_ENTRADA >= '" + filtrosPesquisa.DataEntradaInicial.ToString("MM/dd/yyyy") + "'";
            }

            if (filtrosPesquisa.DataEntradaFinal != DateTime.MinValue)
            {
                querySaida += " AND N.NFI_DATA_EMISSAO <= '" + filtrosPesquisa.DataEntradaFinal.ToString("MM/dd/yyyy 23:59:59") + "'";
                queryEntrada += " AND N.TDE_DATA_ENTRADA <= '" + filtrosPesquisa.DataEntradaFinal.ToString("MM/dd/yyyy 23:59:59") + "'";
            }

            var filtroStatus = "";
            if (filtrosPesquisa.SituacaoFinanceiraNotaEntrada == StatusTitulo.Cancelado)
                filtroStatus = "Cancelado";
            else if (filtrosPesquisa.SituacaoFinanceiraNotaEntrada == StatusTitulo.EmAberto)
                filtroStatus = "Aberto";
            else if (filtrosPesquisa.SituacaoFinanceiraNotaEntrada == StatusTitulo.EmNegociacao)
                filtroStatus = "Renegociado";
            else if (filtrosPesquisa.SituacaoFinanceiraNotaEntrada == StatusTitulo.Quitada)
                filtroStatus = "Pago";

            if (!string.IsNullOrWhiteSpace(filtroStatus))
            {
                querySaida += " AND E.EMP_CODIGO IS NULL ";
                queryEntrada += @"AND
								CAST(SUBSTRING((SELECT DISTINCT ', ' + CAST(TT.TDE_CODIGO AS VARCHAR(20))
								FROM (
								SELECT T.Situacao, T.TDE_CODIGO FROM (
								SELECT
								CASE 
								WHEN T.TIT_STATUS = 1 THEN 'Aberto' 
								WHEN T.TIT_VALOR_PAGO < T.TIT_VALOR_ORIGINAL THEN 
								CASE
									WHEN ISNULL((SELECT COUNT(1) FROM T_TITULO TT WHERE TT.TIT_NUMERO_DOCUMENTO_TITULO_ORIGINAL LIKE '%'+CAST(DE.TDE_NUMERO_LONG AS VARCHAR(20))+'%' AND TT.CLI_CGCCPF = DE.CLI_CGCCPF AND TT.TIT_STATUS <> 3 AND TT.TIT_STATUS <> 4), 0) >= 1 THEN 'Renegociado' 
                                    WHEN ISNULL((SELECT COUNT(1) FROM T_TITULO_BAIXA_AGRUPADO TBA JOIN T_TITULO_BAIXA_NEGOCIACAO TBN ON TBN.TIB_CODIGO = TBA.TIB_CODIGO JOIN T_TITULO TT ON TT.TBN_CODIGO = TBN.TBN_CODIGO WHERE TT.TIT_STATUS <> 3 AND TT.TIT_STATUS <> 4 AND TBA.TIT_CODIGO = T.TIT_CODIGO), 0) >= 1 THEN 'Renegociado' 
									ELSE 'Pago' 
								END
								ELSE 'Pago' 
								END Situacao, DE.TDE_CODIGO
								FROM T_TITULO T
								JOIN T_TMS_DOCUMENTO_ENTRADA_DUPLICATA D ON D.TDD_CODIGO = T.TDD_CODIGO
								JOIN T_TMS_DOCUMENTO_ENTRADA DE ON DE.TDE_CODIGO = D.TDE_CODIGO
								WHERE T.TIT_STATUS <> 4) AS T WHERE T.Situacao = '" + filtroStatus + @"') AS TT
								WHERE 1 = 1 FOR XML PATH('')), 3, 1000) AS VARCHAR(1000)) LIKE '%'+CAST(N.TDE_CODIGO AS VARCHAR(20))+'%'";
            }

            if (filtrosPesquisa.DataInicialFinalizacao != DateTime.MinValue)
                queryEntrada += " AND N.TDE_DATA_FINALIZACAO >= '" + filtrosPesquisa.DataInicialFinalizacao.ToString("MM/dd/yyyy") + "'";

            if (filtrosPesquisa.DataFinalFinalizacao != DateTime.MinValue)
                queryEntrada += " AND N.TDE_DATA_FINALIZACAO <= '" + filtrosPesquisa.DataFinalFinalizacao.ToString("MM/dd/yyyy 23:59:59") + "'";

            if (filtrosPesquisa.Categoria > 0)
            {
                queryEntrada += $" AND C.CTP_CODIGO = {filtrosPesquisa.Categoria} ";
                querySaida += $" AND C.CTP_CODIGO = {filtrosPesquisa.Categoria} ";
            }

            if (filtrosPesquisa.DocFinalizadoAutomaticamente == 1)
            {
                queryEntrada += $" AND N.TDE_DOCUMENTO_FINALIZADO_AUTOMATICAMENTE = {filtrosPesquisa.DocFinalizadoAutomaticamente} ";
                querySaida += " AND 1 = 0 ";
            }
            else if (filtrosPesquisa.DocFinalizadoAutomaticamente == 0)
                queryEntrada += $" AND ( N.TDE_DOCUMENTO_FINALIZADO_AUTOMATICAMENTE = {filtrosPesquisa.DocFinalizadoAutomaticamente} OR N.TDE_DOCUMENTO_FINALIZADO_AUTOMATICAMENTE IS NULL ) ";


            querySaida += " AND N.NFI_AMBIENTE = " + (int)filtrosPesquisa.TipoAmbiente;

            if ((int)filtrosPesquisa.TipoMovimento == 1) //Entrada
                query = queryEntrada + queryParameters;
            else if ((int)filtrosPesquisa.TipoMovimento == 2) //Saída
                query = querySaida + queryParameters;
            else
                query = queryEntrada + queryParameters + " UNION ALL " + querySaida + queryParameters;

            var agrup = false;
            if (!string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeAgrupar))
            {
                agrup = true;
                query += " order by " + parametrosConsulta.PropriedadeAgrupar + " " + parametrosConsulta.DirecaoAgrupar;
            }

            if (!string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeOrdenar) && parametrosConsulta.PropriedadeAgrupar != parametrosConsulta.PropriedadeOrdenar)
            {
                if (agrup)
                {
                    query += ", " + parametrosConsulta.PropriedadeOrdenar + " " + parametrosConsulta.DirecaoOrdenar;
                }
                else
                {
                    query += " order by " + parametrosConsulta.PropriedadeOrdenar + " " + parametrosConsulta.DirecaoOrdenar;
                }
            }

            if (parametrosConsulta.LimiteRegistros > 0)
                query += " OFFSET " + parametrosConsulta.InicioRegistros + " ROWS FETCH FIRST " + parametrosConsulta.LimiteRegistros + " ROWS ONLY";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.NFe.Notas)));

            return nhQuery.SetTimeout(60000).List<Dominio.Relatorios.Embarcador.DataSource.NFe.Notas>();
        }

        public int ContarRelatorioNotas(Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNotas filtrosPesquisa)
        {
            var parametros = new List<ParametroSQL>();

            string query = "", queryParameters = "", queryEntrada = "", querySaida = "";
            if (filtrosPesquisa.CodigoEmpresa > 0)
                queryParameters += " AND E.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresa.ToString();

            if (filtrosPesquisa.CnpjPessoa > 0)
                queryParameters += " AND C.CLI_CGCCPF = " + filtrosPesquisa.CnpjPessoa.ToString();

            if (filtrosPesquisa.CodigosNaturezaOperacao?.Count > 0)
                queryParameters += $" AND NAT.NAT_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosNaturezaOperacao)})";

            queryEntrada = @" SELECT COUNT(0) as CONTADOR                                
	
                                FROM T_TMS_DOCUMENTO_ENTRADA N
                                JOIN T_CLIENTE C ON C.CLI_CGCCPF = N.CLI_CGCCPF
                                JOIN T_LOCALIDADES L ON L.LOC_CODIGO = C.LOC_CODIGO								
                                LEFT OUTER JOIN T_MODDOCFISCAL M ON M.MOD_CODIGO = N.MOD_CODIGO
                                LEFT OUTER JOIN T_NATUREZAOPERACAO NAT ON NAT.NAT_CODIGO = N.NAT_CODIGO
                                LEFT OUTER JOIN T_CFOP CF ON CF.CFO_CODIGO = N.CFO_CODIGO
                                JOIN T_EMPRESA E ON E.EMP_CODIGO = N.EMP_CODIGO
                                JOIN T_LOCALIDADES LE ON LE.LOC_CODIGO = E.LOC_CODIGO
                                LEFT OUTER JOIN T_VEICULO V ON V.VEI_CODIGO = N.VEI_CODIGO
                                LEFT OUTER JOIN T_VEICULO_SEGMENTO SEG ON SEG.VSE_CODIGO = V.VSE_CODIGO                                
                                LEFT OUTER JOIN T_FUNCIONARIO OperadorFinaliza ON OperadorFinaliza.FUN_CODIGO = N.TDE_OPERADOR_FINALIZA_DOCUMENTO
                                LEFT OUTER JOIN T_FUNCIONARIO OperadorLancamento ON OperadorLancamento.FUN_CODIGO = N.TDE_OPERADOR_LANCAMENTO_DOCUMENTO
                                LEFT JOIN T_CATEGORIA_PESSOA CategoriaPessoa ON CategoriaPessoa.CTP_CODIGO = C.CTP_CODIGO
                                WHERE 1 = 1    ";

            querySaida = @" SELECT COUNT(0) as CONTADOR

                            FROM T_NOTA_FISCAL N
                            LEFT OUTER JOIN T_CLIENTE C ON C.CLI_CGCCPF = N.CLI_CGCCPF
                            LEFT OUTER JOIN T_LOCALIDADES L ON L.LOC_CODIGO = C.LOC_CODIGO	
                            JOIN T_EMPRESA_SERIE ES ON ES.ESE_CODIGO = N.ESE_CODIGO
                            LEFT OUTER JOIN T_NATUREZAOPERACAO NAT ON NAT.NAT_CODIGO = N.NAT_CODIGO                            
                            LEFT OUTER JOIN T_CFOP CF ON CF.CFO_CODIGO = NAT.CFO_CODIGO
                            JOIN T_EMPRESA E ON E.EMP_CODIGO = N.EMP_CODIGO
                            JOIN T_LOCALIDADES LE ON LE.LOC_CODIGO = E.LOC_CODIGO                            
                            WHERE 1 = 1 ";

            if (filtrosPesquisa.CodigoEmpresaFilial > 0)
            {
                querySaida += " AND E.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresaFilial.ToString();
                queryEntrada += " AND E.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresaFilial.ToString();
            }

            if (filtrosPesquisa.CodigoSegmento > 0)
            {
                queryEntrada += " AND V.VSE_CODIGO = " + filtrosPesquisa.CodigoSegmento.ToString();
                querySaida += " AND 1 = 0 ";
            }

            if (filtrosPesquisa.CodigoEquipamento > 0)
            {
                queryEntrada += " AND V.VEI_CODIGO IN (SELECT VEQ.VEI_CODIGO FROM T_VEICULO_EQUIPAMENTO VEQ LEFT OUTER JOIN T_EQUIPAMENTO EQP ON VEQ.EQP_CODIGO = EQP.EQP_CODIGO WHERE EQP.EQP_CODIGO = " + filtrosPesquisa.CodigoEquipamento.ToString() + " )"; // SQL-INJECTION-SAFE
                querySaida += " AND 1 = 0 ";
            }

            if (!string.IsNullOrEmpty(filtrosPesquisa.EstadoEmitente) && filtrosPesquisa.EstadoEmitente != "0")
            {
                querySaida += " AND LE.UF_SIGLA = '" + filtrosPesquisa.EstadoEmitente + "'";
                queryEntrada += " AND L.UF_SIGLA = '" + filtrosPesquisa.EstadoEmitente + "'";
            }

            if (filtrosPesquisa.CodigoVeiculo > 0)
            {
                querySaida += " AND E.EMP_CODIGO IS NULL ";
                queryEntrada += " AND V.VEI_CODIGO = " + filtrosPesquisa.CodigoVeiculo.ToString();
            }

            if ((int)filtrosPesquisa.StatusNotaEntrada > 0)
            {
                querySaida += " AND E.EMP_CODIGO IS NULL ";
                queryEntrada += " AND N.TDE_SITUACAO = " + filtrosPesquisa.StatusNotaEntrada.ToString("d");
            }

            if (filtrosPesquisa.NumeroInicial > 0 && filtrosPesquisa.NumeroFinal == 0)
            {
                querySaida += " AND N.NFI_NUMERO = " + filtrosPesquisa.NumeroInicial.ToString();
                queryEntrada += " AND N.TDE_NUMERO_LONG = " + filtrosPesquisa.NumeroInicial.ToString();
            }
            else if (filtrosPesquisa.NumeroInicial > 0 && filtrosPesquisa.NumeroFinal > 0)
            {
                querySaida += " AND N.NFI_NUMERO >= " + filtrosPesquisa.NumeroInicial.ToString() + " AND N.NFI_NUMERO <= " + filtrosPesquisa.NumeroFinal.ToString();
                queryEntrada += " AND N.TDE_NUMERO_LONG >= " + filtrosPesquisa.NumeroInicial.ToString() + " AND N.TDE_NUMERO_LONG <= " + filtrosPesquisa.NumeroFinal.ToString();
            }
            else if (filtrosPesquisa.NumeroInicial == 0 && filtrosPesquisa.NumeroFinal > 0)
            {
                querySaida += " AND N.NFI_NUMERO = " + filtrosPesquisa.NumeroFinal.ToString();
                queryEntrada += " AND N.TDE_NUMERO_LONG = " + filtrosPesquisa.NumeroFinal.ToString();
            }

            if (filtrosPesquisa.Serie > 0)
            {
                querySaida += " AND ES.ESE_NUMERO = " + filtrosPesquisa.Serie.ToString();
                queryEntrada += " AND N.TDE_SERIE = " + filtrosPesquisa.Serie.ToString();
            }

            if (filtrosPesquisa.CodigosModeloDocumentoFiscal != null && filtrosPesquisa.CodigosModeloDocumentoFiscal.Count > 0)
                queryEntrada += " and M.MOD_CODIGO in (" + string.Join(",", filtrosPesquisa.CodigosModeloDocumentoFiscal) + ")";
            if (filtrosPesquisa.CodigoModelo > 0)
            {
                queryEntrada += " AND M.MOD_CODIGO = " + filtrosPesquisa.CodigoModelo.ToString();
            }
            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroModeloNF))
            {
                querySaida += " AND N.NFI_MODELO = '" + filtrosPesquisa.NumeroModeloNF + "'";
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Chave))
            {
                querySaida += " AND N.NFI_CHAVE LIKE '%" + filtrosPesquisa.Chave + "%'";
                queryEntrada += " AND N.TDE_CHAVE LIKE '%" + filtrosPesquisa.Chave + "%'";
            }

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
            {
                querySaida += " AND N.NFI_DATA_EMISSAO >= '" + filtrosPesquisa.DataInicial.ToString("MM/dd/yyyy") + "'";
                queryEntrada += " AND N.TDE_DATA_EMISSAO >= '" + filtrosPesquisa.DataInicial.ToString("MM/dd/yyyy") + "'";
            }

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
            {
                querySaida += " AND N.NFI_DATA_EMISSAO <= '" + filtrosPesquisa.DataFinal.ToString("MM/dd/yyyy 23:59:59") + "'";
                queryEntrada += " AND N.TDE_DATA_EMISSAO <= '" + filtrosPesquisa.DataFinal.ToString("MM/dd/yyyy 23:59:59") + "'";
            }

            if (filtrosPesquisa.DataEntradaInicial != DateTime.MinValue)
            {
                querySaida += " AND N.NFI_DATA_EMISSAO >= '" + filtrosPesquisa.DataEntradaInicial.ToString("MM/dd/yyyy") + "'";
                queryEntrada += " AND N.TDE_DATA_ENTRADA >= '" + filtrosPesquisa.DataEntradaInicial.ToString("MM/dd/yyyy") + "'";
            }

            if (filtrosPesquisa.DataEntradaFinal != DateTime.MinValue)
            {
                querySaida += " AND N.NFI_DATA_EMISSAO <= '" + filtrosPesquisa.DataEntradaFinal.ToString("MM/dd/yyyy 23:59:59") + "'";
                queryEntrada += " AND N.TDE_DATA_ENTRADA <= '" + filtrosPesquisa.DataEntradaFinal.ToString("MM/dd/yyyy 23:59:59") + "'";
            }

            if (filtrosPesquisa.OperadorLancamentoDocumento > 0)
            {
                queryEntrada += " AND N.TDE_OPERADOR_LANCAMENTO_DOCUMENTO = " + filtrosPesquisa.OperadorLancamentoDocumento.ToString();
                querySaida += " AND 1 = 0 ";
            }

            if (filtrosPesquisa.OperadorFinalizouDocumento > 0)
            {
                queryEntrada += " AND N.TDE_OPERADOR_FINALIZA_DOCUMENTO = " + filtrosPesquisa.OperadorFinalizouDocumento.ToString();
                querySaida += " AND 1 = 0 ";
            }

            var filtroStatus = "";
            if (filtrosPesquisa.SituacaoFinanceiraNotaEntrada == StatusTitulo.Cancelado)
                filtroStatus = "Cancelado";
            else if (filtrosPesquisa.SituacaoFinanceiraNotaEntrada == StatusTitulo.EmAberto)
                filtroStatus = "Aberto";
            else if (filtrosPesquisa.SituacaoFinanceiraNotaEntrada == StatusTitulo.EmNegociacao)
                filtroStatus = "Renegociado";
            else if (filtrosPesquisa.SituacaoFinanceiraNotaEntrada == StatusTitulo.Quitada)
                filtroStatus = "Pago";

            if (!string.IsNullOrWhiteSpace(filtroStatus))
            {
                querySaida += " AND E.EMP_CODIGO IS NULL ";
                queryEntrada += @"AND
								CAST(SUBSTRING((SELECT DISTINCT ', ' + CAST(TT.TDE_CODIGO AS VARCHAR(20))
								FROM (
								SELECT T.Situacao, T.TDE_CODIGO FROM (
								SELECT
								CASE 
								WHEN T.TIT_STATUS = 1 THEN 'Aberto' 
								WHEN T.TIT_VALOR_PAGO < T.TIT_VALOR_ORIGINAL THEN 
								CASE
									WHEN ISNULL((SELECT COUNT(1) FROM T_TITULO TT WHERE TT.TIT_NUMERO_DOCUMENTO_TITULO_ORIGINAL LIKE '%'+CAST(DE.TDE_NUMERO_LONG AS VARCHAR(20))+'%' AND TT.CLI_CGCCPF = DE.CLI_CGCCPF AND TT.TIT_STATUS <> 3 AND TT.TIT_STATUS <> 4), 0) >= 1 THEN 'Renegociado' 
                                    WHEN ISNULL((SELECT COUNT(1) FROM T_TITULO_BAIXA_AGRUPADO TBA JOIN T_TITULO_BAIXA_NEGOCIACAO TBN ON TBN.TIB_CODIGO = TBA.TIB_CODIGO JOIN T_TITULO TT ON TT.TBN_CODIGO = TBN.TBN_CODIGO WHERE TT.TIT_STATUS <> 3 AND TT.TIT_STATUS <> 4 AND TBA.TIT_CODIGO = T.TIT_CODIGO), 0) >= 1 THEN 'Renegociado' 
									ELSE 'Pago' 
								END
								ELSE 'Pago' 
								END Situacao, DE.TDE_CODIGO
								FROM T_TITULO T
								JOIN T_TMS_DOCUMENTO_ENTRADA_DUPLICATA D ON D.TDD_CODIGO = T.TDD_CODIGO
								JOIN T_TMS_DOCUMENTO_ENTRADA DE ON DE.TDE_CODIGO = D.TDE_CODIGO
								WHERE T.TIT_STATUS <> 4) AS T WHERE T.Situacao = '" + filtroStatus + @"') AS TT
								WHERE 1 = 1 FOR XML PATH('')), 3, 1000) AS VARCHAR(1000)) LIKE '%'+CAST(N.TDE_CODIGO AS VARCHAR(20))+'%'";
            }

            if (filtrosPesquisa.Categoria > 0)
            {
                queryEntrada += $" AND C.CTP_CODIGO = {filtrosPesquisa.Categoria} ";
                querySaida += $" AND C.CTP_CODIGO = {filtrosPesquisa.Categoria} ";
            }

            if (filtrosPesquisa.DataInicialFinalizacao != DateTime.MinValue)
                queryEntrada += " AND N.TDE_DATA_FINALIZACAO >= '" + filtrosPesquisa.DataInicialFinalizacao.ToString("MM/dd/yyyy") + "'";

            if (filtrosPesquisa.DataFinalFinalizacao != DateTime.MinValue)
                queryEntrada += " AND N.TDE_DATA_FINALIZACAO <= '" + filtrosPesquisa.DataFinalFinalizacao.ToString("MM/dd/yyyy 23:59:59") + "'";

            if (filtrosPesquisa.DocFinalizadoAutomaticamente == 1)
            {
                queryEntrada += $" AND N.TDE_DOCUMENTO_FINALIZADO_AUTOMATICAMENTE = {filtrosPesquisa.DocFinalizadoAutomaticamente} ";
                querySaida += " AND 1 = 0 ";
            }
            else if (filtrosPesquisa.DocFinalizadoAutomaticamente == 0)
                queryEntrada += $" AND ( N.TDE_DOCUMENTO_FINALIZADO_AUTOMATICAMENTE = {filtrosPesquisa.DocFinalizadoAutomaticamente} OR N.TDE_DOCUMENTO_FINALIZADO_AUTOMATICAMENTE IS NULL ) ";

            querySaida += " AND N.NFI_AMBIENTE = " + (int)filtrosPesquisa.TipoAmbiente;

            if ((int)filtrosPesquisa.TipoMovimento == 1) //Entrada
                query = queryEntrada + queryParameters;
            else if ((int)filtrosPesquisa.TipoMovimento == 2) //Saída
                query = querySaida + queryParameters;
            else
                query = " SELECT SUM(CONTADOR) CONTADOR FROM(" + queryEntrada + queryParameters + " UNION ALL " + querySaida + queryParameters + ") AS T";

            var sqlDinamico = new SQLDinamico(query, parametros);

            var nhQuery = sqlDinamico.CriarSQLQuery(this.SessionNHiBernate);

            return nhQuery.SetTimeout(60000).UniqueResult<int>();
        }

        #endregion
    }
}
