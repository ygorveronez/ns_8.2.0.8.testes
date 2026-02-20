using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Repositorio.Embarcador.Consulta;

namespace Repositorio.Embarcador.PagamentoAgregado
{
    public class PagamentoAgregado : RepositorioBase<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado>
    {
        public PagamentoAgregado(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado> BuscarPorStatus(Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPagamentoAgregado status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado>();
            var result = from obj in query where obj.StatusPagamentoAgregado == status select obj;
            return result.ToList();
        }

        public int BuscarProximoNumero()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado>();

            int? retorno = query.Max(o => (int?)o.Numero);

            return retorno.HasValue ? retorno.Value + 1 : 1;
        }

        private IQueryable<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado> _Consultar(int numero, int numeroContrato, DateTime dataInicio, DateTime dataFim, double cliente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado? situacao, List<int> codigosEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado>();

            var result = from obj in query select obj;

            // Filtros
            if (dataInicio != DateTime.MinValue)
                result = result.Where(o => o.Data.Date >= dataInicio);

            if (dataFim != DateTime.MinValue)
                result = result.Where(o => o.Data.Date <= dataFim);

            if (cliente > 0)
                result = result.Where(o => o.Cliente.CPF_CNPJ == cliente);

            if (situacao.HasValue)
                result = result.Where(o => o.Situacao == situacao.Value);

            if (numero > 0)
                result = result.Where(o => o.Numero == numero);

            if (numeroContrato > 0)
            {
                var queryContrato = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete>();
                var resultContrato = from obj in queryContrato where obj.NumeroContrato == numeroContrato select obj;

                result = result.Where(o => resultContrato.Select(p => p.PagamentoAgregado).Contains(o));
            }

            if (codigosEmpresa != null && codigosEmpresa.Count > 0)
                result = result.Where(o => codigosEmpresa.Contains(o.Empresa.Codigo));

            return result;
        }

        public List<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado> Consultar(int numero, int numeroContrato, DateTime dataInicio, DateTime dataFim, double cliente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado? situacao, List<int> codigosEmpresa, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(numero, numeroContrato, dataInicio, dataFim, cliente, situacao, codigosEmpresa);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(int numero, int numeroContrato, DateTime dataInicio, DateTime dataFim, double cliente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado? situacao, List<int> codigosEmpresa)
        {
            var result = _Consultar(numero, numeroContrato, dataInicio, dataFim, cliente, situacao, codigosEmpresa);

            return result.Count();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.PagamentoAgregado> RelatorioReciboPagamentoAgregado(int codigo, int codigoEmpresa)
        {
            string query = @"SELECT P.PAA_NUMERO Numero,
                        P.PAA_DATA_PAGAMENTO DataPagamento,
                        E.EMP_RAZAO RazaoEmpresa,
                        P.PAA_DATA DataGeracao,
                        E.EMP_RESPONSAVEL ResponsavelEmpresa,
                        E.EMP_CNPJ CNPJEmpresa,
                        E.EMP_ENDERECO EnderecoEmpresa,
                        E.EMP_BAIRRO BairroEmpresa,
                        L.LOC_DESCRICAO CidadeEmpresa,
                        L.UF_SIGLA EstadoEmpresa,
                        E.EMP_CEP CEPEmpresa,
                        C.CLI_NOME NomeCliente,
                        C.CLI_CGCCPF CNPJCliente,
                        C.CLI_ENDERECO EnderecoCliente,
                        C.CLI_BAIRRO BairroCliente,
                        LC.LOC_DESCRICAO CidadeCliente,
                        LC.UF_SIGLA EstadoCliente,
                        C.CLI_CEP CEPCliente,
                        P.PAA_DATA_FINAL DataFinal,
                        P.PAA_DATA_INICIAL DataInicial,
                        P.PAA_VALOR Valor,
                        P.PAA_OBSERVACAO Observacao,
                        C.CLI_BANCO_AGENCIA AgenciaCliente,
                        C.CLI_BANCO_DIGITO_AGENCIA DigitoAgenciaCliente, 
                        C.CLI_BANCO_NUMERO_CONTA ContaCliente,
                        C.CLI_BANCO_TIPO_CONTA TipoContaCliente,
                        B.BCO_DESCRICAO BancoCliente,
                        (SELECT COUNT(1) FROM T_PAGAMENTO_AGREGADO_ACRESCIMO_DESCONTO AD WHERE AD.PAA_CODIGO = P.PAA_CODIGO) ContemDescontoAcrescimo,
                        (SELECT COUNT(1) FROM T_PAGAMENTO_AGREGADO_ADIANTAMENTO AD WHERE AD.PAA_CODIGO = P.PAA_CODIGO) ContemaAdiantamento
                        FROM T_PAGAMENTO_AGREGADO P
                        JOIN T_CLIENTE C ON C.CLI_CGCCPF = P.CLI_CODIGO
                        LEFT OUTER JOIN T_BANCO B ON B.BCO_CODIGO = C.BCO_CODIGO
                        JOIN T_LOCALIDADES LC ON LC.LOC_CODIGO = C.LOC_CODIGO,
                        T_EMPRESA E
                        JOIN T_LOCALIDADES L ON L.LOC_CODIGO = E.LOC_CODIGO
                        WHERE E.EMP_CODIGO = " + codigoEmpresa.ToString() + @"
                                        AND P.PAA_CODIGO = " + codigo.ToString();

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.PagamentoAgregado)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.PagamentoAgregado>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.PagamentoAgregadoAdiantamento> RelatorioReciboPagamentoAgregadoAdiantamento(int codigo)
        {
            string query = @"SELECT P.PAM_DATA Data, P.PAM_DATA_PAGAMENTO DataPagamento, P.PAM_NUMERO Numero, P.PAM_OBSERVACAO Observacao, P.PAM_VALOR Valor, T.PMT_DESCRICAO Descricao
                FROM T_PAGAMENTO_AGREGADO_ADIANTAMENTO A
                JOIN T_PAGAMENTO_MOTORISTA_TMS P ON P.PAM_CODIGO = A.PAM_CODIGO
                JOIN T_PAGAMENTO_MOTORISTA_TIPO T ON T.PMT_CODIGO = P.PMT_CODIGO
                WHERE A.PAA_CODIGO = " + codigo.ToString();

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.PagamentoAgregadoAdiantamento)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.PagamentoAgregadoAdiantamento>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.PagamentoAgregadoDescontoAcrescimo> RelatorioReciboPagamentoAgregadoDescontoAcrescimo(int codigo)
        {
            string query = @"SELECT AD.PAD_VALOR Valor, J.JUS_DESCRICAO Descricao, J.JUS_TIPO Tipo
                FROM T_PAGAMENTO_AGREGADO_ACRESCIMO_DESCONTO AD
                JOIN T_JUSTIFICATIVA J ON J.JUS_CODIGO = AD.JUS_CODIGO
                WHERE AD.PAA_CODIGO =  " + codigo.ToString();

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.PagamentoAgregadoDescontoAcrescimo)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.PagamentoAgregadoDescontoAcrescimo>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.PagamentoAgregadoDocumento> RelatorioReciboPagamentoAgregadoDocumento(int codigo)
        {
            string query = @" SELECT C.CON_DATAHORAEMISSAO DataEmissao, 
                                    C.CON_NUM Numero, 
                                    P.PDO_VALOR Valor, 
                                    DEST.PCT_NOME Destinatario, 
                                    L.LOC_DESCRICAO Cidade, 
                                    L.UF_SIGLA Estado,

                                   ISNULL(SUBSTRING((
                                       SELECT DISTINCT ', ' + V.CVE_PLACA
                                         FROM T_CTE_VEICULO V
                                        WHERE V.CON_CODIGO = C.CON_CODIGO
                                          FOR XML PATH('')
                                   ), 3, 2000), '') Veiculos,
                                   ISNULL(SUBSTRING((
                                       SELECT DISTINCT ', ' + V.CMO_NOME_MOTORISTA
                                         FROM T_CTE_MOTORISTA V
                                        WHERE V.CON_CODIGO = C.CON_CODIGO
                                          FOR XML PATH('')
                                   ), 3, 2000), '') Motoristas,
                                   ISNULL(SUBSTRING((
                                       SELECT DISTINCT ', ' + O.OCO_DESCRICAO
                                         FROM T_CARGA_OCORRENCIA CO
                                         JOIN T_CARGA_OCORRENCIA_DOCUMENTO D ON D.COC_CODIGO = CO.COC_CODIGO
                                         JOIN T_OCORRENCIA O ON O.OCO_CODIGO = CO.OCO_CODIGO
                                        WHERE D.CON_CODIGO = C.CON_CODIGO
                                          FOR XML PATH('')
                                   ), 3, 2000), '') Ocorrencias,
                                   ISNULL(SUBSTRING((
                                       SELECT DISTINCT ', ' + CAR_CODIGO_CARGA_EMBARCADOR
                                         FROM T_CARGA_CTE CargaCte
                                         JOIN T_CARGA Carga ON Carga.CAR_CODIGO = CargaCte.CAR_CODIGO
                                        WHERE CargaCte.CON_CODIGO = C.CON_CODIGO
                                          AND Carga.CAR_CARGA_TRANSBORDO = 0
                                          FOR XML PATH('')
            	                   ), 3, 2000), '') Cargas,

                                    ISNULL(SUBSTRING((
                                       SELECT DISTINCT ', ' + CAST(CONVERT(VARCHAR(10), CAR_DATA_CRIACAO, 103) AS NVARCHAR(160))
                                           FROM T_CARGA_CTE CargaCte
                                           JOIN T_CARGA Carga ON Carga.CAR_CODIGO = CargaCte.CAR_CODIGO
                                       WHERE CargaCte.CON_CODIGO = C.CON_CODIGO
                                         AND Carga.CAR_CARGA_TRANSBORDO = 0
                                         FOR XML PATH('') ), 3, 2000), '') DatasCargas,
                                    ISNULL(SUBSTRING((
                                        SELECT DISTINCT ', ' + CAST(CONVERT(VARCHAR(10), CAR_DATA_CARREGAMENTO_PEDIDO, 103) AS NVARCHAR(160))
                                           FROM T_CARGA_CTE CargaCte
                                           JOIN T_CARGA Carga ON Carga.CAR_CODIGO = CargaCte.CAR_CODIGO
                                           JOIN T_CARGA_PEDIDO CargaPedido ON CargaPedido.CAR_CODIGO = CargaCte.CAR_CODIGO
                                           JOIN T_PEDIDO Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                                       WHERE CargaCte.CON_CODIGO = C.CON_CODIGO
                                         AND Carga.CAR_CARGA_TRANSBORDO = 0
                                         FOR XML PATH('') ), 3, 2000), '') DatasPedidos,
                                    ISNULL(SUBSTRING((
                                        SELECT DISTINCT ', ' + CAST(PED_NUMERO AS VARCHAR(50))
                                           FROM T_CARGA_CTE CargaCte
                                           JOIN T_CARGA Carga ON Carga.CAR_CODIGO = CargaCte.CAR_CODIGO
                                           JOIN T_CARGA_PEDIDO CargaPedido ON CargaPedido.CAR_CODIGO = CargaCte.CAR_CODIGO
                                           JOIN T_PEDIDO Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                                       WHERE CargaCte.CON_CODIGO = C.CON_CODIGO
                                         AND Carga.CAR_CARGA_TRANSBORDO = 0
                                         FOR XML PATH('') ), 3, 2000), '') NumerosPedidos

                              FROM T_PAGAMENTO_AGREGADO_DOCUMENTO P
                              JOIN T_CTE C ON C.CON_CODIGO = P.CON_CODIGO
                              JOIN T_CTE_PARTICIPANTE DEST ON DEST.PCT_CODIGO = C.CON_DESTINATARIO_CTE
                              JOIN T_LOCALIDADES L ON L.LOC_CODIGO = DEST.LOC_CODIGO
                              WHERE P.PAA_CODIGO = " + codigo.ToString();

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.PagamentoAgregadoDocumento)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.PagamentoAgregadoDocumento>();
        }

        public Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado BuscarPagamentoAbertoPorPessoaData(double cliente, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado>();

            var result = from obj in query where obj.Cliente.CPF_CNPJ == cliente && data >= obj.DataInicial && data <= obj.DataFinal && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado.Iniciada select obj;

            return result.FirstOrDefault();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.FaturaPagamentoAgregado> RelatorioFaturaPagamentoAgregado(int codigoPagamentoAgregado)
        {
            string query = @"SELECT P.PAA_NUMERO Numero,
                        P.PAA_DATA_PAGAMENTO DataPagamento,
                        P.PAA_DATA DataGeracao,
                        P.PAA_VALOR Valor,
                        P.PAA_OBSERVACAO Observacao,
                        B.BCO_DESCRICAO BancoCliente,
                        P.PAA_NUMERO_FATURA NumeroFatura,
                        P.PAA_COMPETENCIA_MES CompetenciaMes,
                        P.PAA_COMPETENCIA_QUINZENA CompetenciaQuinzena,
                        P.PAA_DESCRICAO_COMPETENCIA DescricaoCompetencia,

                        C.CLI_NOME NomeCliente,
                        C.CLI_CGCCPF CNPJCliente,
                        C.CLI_ENDERECO EnderecoCliente,
                        C.CLI_BAIRRO BairroCliente,
                        LC.LOC_DESCRICAO CidadeCliente,
                        LC.UF_SIGLA EstadoCliente,
                        C.CLI_CEP CEPCliente,
                        C.CLI_BANCO_AGENCIA AgenciaCliente,
                        C.CLI_BANCO_DIGITO_AGENCIA DigitoAgenciaCliente,
                        C.CLI_BANCO_NUMERO_CONTA ContaCliente,
                        CAST(ModalidadeTransportadoraPessoas.MOT_CODIGO_PROVEDOR AS VARCHAR(20)) CodigoProvedorCliente,

                        TomadorFatura.CLI_NOME NomeTomadorFatura,
                        TomadorFatura.CLI_CGCCPF CNPJTomadorFatura

                        FROM T_PAGAMENTO_AGREGADO P
                        JOIN T_CLIENTE C ON C.CLI_CGCCPF = P.CLI_CODIGO
                        JOIN T_LOCALIDADES LC ON LC.LOC_CODIGO = C.LOC_CODIGO
                        LEFT OUTER JOIN T_BANCO B ON B.BCO_CODIGO = C.BCO_CODIGO
                        LEFT OUTER JOIN T_CLIENTE TomadorFatura ON TomadorFatura.CLI_CGCCPF = P.CLI_CGCCPF_TOMADOR_FATURA
                        LEFT OUTER JOIN T_CLIENTE_MODALIDADE ModalidadePessoasTransportador on ModalidadePessoasTransportador.CPF_CNPJ = C.CLI_CGCCPF and ModalidadePessoasTransportador.MOD_TIPO = 3
                        LEFT OUTER JOIN T_CLIENTE_MODALIDADE_TRANSPORTADORAS ModalidadeTransportadoraPessoas on ModalidadeTransportadoraPessoas.MOD_CODIGO = ModalidadePessoasTransportador.MOD_CODIGO

                        WHERE P.PAA_CODIGO = " + codigoPagamentoAgregado;

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.FaturaPagamentoAgregado)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.FaturaPagamentoAgregado>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.FaturaPagamentoAgregadoDocumento> RelatorioFaturaPagamentoAgregadoDocumento(int codigoPagamentoAgregado)
        {
            string query = @" SELECT C.CON_DATAHORAEMISSAO DataEmissao, 
                                    C.CON_NUM Numero, 
                                    P.PDO_VALOR Valor, 
                                    DEST.PCT_NOME Destinatario, 
                                    L.LOC_DESCRICAO Cidade, 
                                    L.UF_SIGLA Estado,

                                   ISNULL(SUBSTRING((
                                       SELECT DISTINCT ', ' + V.CVE_PLACA
                                         FROM T_CTE_VEICULO V
                                        WHERE V.CON_CODIGO = C.CON_CODIGO
                                          FOR XML PATH('')
                                   ), 3, 2000), '') Veiculos,
                                   ISNULL(SUBSTRING((
                                       SELECT DISTINCT ', ' + V.CMO_NOME_MOTORISTA
                                         FROM T_CTE_MOTORISTA V
                                        WHERE V.CON_CODIGO = C.CON_CODIGO
                                          FOR XML PATH('')
                                   ), 3, 2000), '') Motoristas,
                                   ISNULL(SUBSTRING((
                                       SELECT DISTINCT ', ' + O.OCO_DESCRICAO
                                         FROM T_CARGA_OCORRENCIA CO
                                         JOIN T_CARGA_OCORRENCIA_DOCUMENTO D ON D.COC_CODIGO = CO.COC_CODIGO
                                         JOIN T_OCORRENCIA O ON O.OCO_CODIGO = CO.OCO_CODIGO
                                        WHERE D.CON_CODIGO = C.CON_CODIGO
                                          FOR XML PATH('')
                                   ), 3, 2000), '') Ocorrencias,
                                   ISNULL(SUBSTRING((
                                       SELECT DISTINCT ', ' + CAR_CODIGO_CARGA_EMBARCADOR
                                         FROM T_CARGA_CTE CargaCte
                                         JOIN T_CARGA Carga ON Carga.CAR_CODIGO = CargaCte.CAR_CODIGO
                                        WHERE CargaCte.CON_CODIGO = C.CON_CODIGO
                                          AND Carga.CAR_CARGA_TRANSBORDO = 0
                                          FOR XML PATH('')
            	                   ), 3, 2000), '') Cargas,

                                    ISNULL(SUBSTRING((
                                       SELECT DISTINCT ', ' + CAST(CONVERT(VARCHAR(10), CAR_DATA_CRIACAO, 103) AS NVARCHAR(160))
                                           FROM T_CARGA_CTE CargaCte
                                           JOIN T_CARGA Carga ON Carga.CAR_CODIGO = CargaCte.CAR_CODIGO
                                       WHERE CargaCte.CON_CODIGO = C.CON_CODIGO
                                         AND Carga.CAR_CARGA_TRANSBORDO = 0
                                         FOR XML PATH('') ), 3, 2000), '') DatasCargas,
                                    ISNULL(SUBSTRING((
                                        SELECT DISTINCT ', ' + CAST(CONVERT(VARCHAR(10), CAR_DATA_CARREGAMENTO_PEDIDO, 103) AS NVARCHAR(160))
                                           FROM T_CARGA_CTE CargaCte
                                           JOIN T_CARGA Carga ON Carga.CAR_CODIGO = CargaCte.CAR_CODIGO
                                           JOIN T_CARGA_PEDIDO CargaPedido ON CargaPedido.CAR_CODIGO = CargaCte.CAR_CODIGO
                                           JOIN T_PEDIDO Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                                       WHERE CargaCte.CON_CODIGO = C.CON_CODIGO
                                         AND Carga.CAR_CARGA_TRANSBORDO = 0
                                         FOR XML PATH('') ), 3, 2000), '') DatasPedidos,
                                    ISNULL(SUBSTRING((
                                        SELECT DISTINCT ', ' + CAST(PED_NUMERO AS VARCHAR(50))
                                           FROM T_CARGA_CTE CargaCte
                                           JOIN T_CARGA Carga ON Carga.CAR_CODIGO = CargaCte.CAR_CODIGO
                                           JOIN T_CARGA_PEDIDO CargaPedido ON CargaPedido.CAR_CODIGO = CargaCte.CAR_CODIGO
                                           JOIN T_PEDIDO Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                                       WHERE CargaCte.CON_CODIGO = C.CON_CODIGO
                                         AND Carga.CAR_CARGA_TRANSBORDO = 0
                                         FOR XML PATH('') ), 3, 2000), '') NumerosPedidos

                              FROM T_PAGAMENTO_AGREGADO_DOCUMENTO P
                              JOIN T_CTE C ON C.CON_CODIGO = P.CON_CODIGO
                              JOIN T_CTE_PARTICIPANTE DEST ON DEST.PCT_CODIGO = C.CON_DESTINATARIO_CTE
                              JOIN T_LOCALIDADES L ON L.LOC_CODIGO = DEST.LOC_CODIGO
                              WHERE P.PAA_CODIGO = " + codigoPagamentoAgregado;

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.FaturaPagamentoAgregadoDocumento)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.PagamentoAgregado.FaturaPagamentoAgregadoDocumento>();
        }

        #endregion

        #region Relatório Pagamento Agregado

        public IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.PagamentoAgregado> ConsultarRelatorioPagamentoAgregado(int numeroInicial, int numeroFinal, DateTime dataPagamentoInicial, DateTime dataPagamentoFinal, int motorista, int veiculo, int tipoOperacao, int numeroCTe, string numeroCarga, double agregado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado? situacao, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var sql = ObterSelectConsultaRelatorioPagamentoAgregado(numeroInicial, numeroFinal, dataPagamentoInicial, dataPagamentoFinal, motorista, veiculo, tipoOperacao, numeroCTe, numeroCarga, agregado, situacao, false, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite);
            var query = sql.CriarSQLQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Financeiros.PagamentoAgregado)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.PagamentoAgregado>();
        }

        public int ContarConsultaRelatorioPagamentoAgregado(int numeroInicial, int numeroFinal, DateTime dataPagamentoInicial, DateTime dataPagamentoFinal, int motorista, int veiculo, int tipoOperacao, int numeroCTe, string numeroCarga, double agregado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado? situacao, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena)
        {
            var sql = ObterSelectConsultaRelatorioPagamentoAgregado(numeroInicial, numeroFinal, dataPagamentoInicial, dataPagamentoFinal, motorista, veiculo, tipoOperacao, numeroCTe, numeroCarga, agregado, situacao, true, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, 0, 0);
            var query = sql.CriarSQLQuery(this.SessionNHiBernate);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        private SQLDinamico ObterSelectConsultaRelatorioPagamentoAgregado(int numeroInicial, int numeroFinal, DateTime dataPagamentoInicial, DateTime dataPagamentoFinal, int motorista, int veiculo, int tipoOperacao, int numeroCTe, string numeroCarga, double agregado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado? situacao, bool count, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string select = string.Empty,
                   groupBy = string.Empty,
                   joins = string.Empty,
                   where = string.Empty,
                   orderBy = string.Empty;

            var parametros = new List<ParametroSQL>();

            for (var i = propriedades.Count - 1; i >= 0; i--)
                SetarSelectRelatorioConsultaRelatorioPagamentoAgregado(propriedades[i].Propriedade, propriedades[i].CodigoDinamico, ref select, ref groupBy, ref joins, count);

            SetarWhereRelatorioConsultaRelatorioPagamentoAgregado(ref where, ref groupBy, ref joins, ref parametros,  numeroInicial, numeroFinal, dataPagamentoInicial, dataPagamentoFinal, motorista, veiculo, tipoOperacao, numeroCTe, numeroCarga, agregado, situacao);

            if (!count)
            {
                if (!string.IsNullOrWhiteSpace(propAgrupa))
                {
                    SetarSelectRelatorioConsultaRelatorioPagamentoAgregado(propAgrupa, 0, ref select, ref groupBy, ref joins, count);

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
            query += " FROM T_PAGAMENTO_AGREGADO Pagamento ";

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

            return new SQLDinamico(query, parametros);
        }

        private void SetarSelectRelatorioConsultaRelatorioPagamentoAgregado(string propriedade, int codigoDinamico, ref string select, ref string groupBy, ref string joins, bool count)
        {
            switch (propriedade)
            {

                case "Numero":
                    if (!select.Contains(" Numero, "))
                    {
                        select += "Pagamento.PAA_NUMERO Numero, ";
                    }
                    break;
                case "DescricaoDataPagamento":
                    if (!select.Contains(" DataPagamento, "))
                    {
                        select += "Pagamento.PAA_DATA_PAGAMENTO DataPagamento, ";
                    }
                    break;
                case "Observacao":
                    if (!select.Contains(" Observacao, "))
                    {
                        select += "Pagamento.PAA_OBSERVACAO Observacao, ";
                    }
                    break;
                case "DescricaoSituacao":
                    if (!select.Contains(" Situacao, "))
                    {
                        select += "Pagamento.PAA_SITUACAO Situacao, ";
                    }
                    break;
                case "DescricaoStatus":
                    if (!select.Contains(" Status, "))
                    {
                        select += "Pagamento.PAA_STATUS Status, ";
                    }
                    break;
                case "Valor":
                    if (!select.Contains(" Valor, "))
                    {
                        select += "Pagamento.PAA_VALOR Valor, ";
                    }
                    break;
                case "DescricaoDataInicialEmissao":
                    if (!select.Contains(" DataInicialEmissao, "))
                    {
                        select += "Pagamento.PAA_DATA_INICIAL DataInicialEmissao, ";
                    }
                    break;
                case "DescricaoDataFinalEmissao":
                    if (!select.Contains(" DataFinalEmissao, "))
                    {
                        select += "Pagamento.PAA_DATA_FINAL DataFinalEmissao, ";
                    }
                    break;
                case "DescricaoDataInicialOcorrencia":
                    if (!select.Contains(" DataInicialOcorrencia, "))
                    {
                        select += "Pagamento.PAA_DATA_INICIAL_OCORRENCIA DataInicialOcorrencia, ";
                    }
                    break;
                case "DescricaoDataFinalOcorrencia":
                    if (!select.Contains(" DataFinalOcorrencia, "))
                    {
                        select += "Pagamento.PAA_DATA_FINAL_OCORRENCIA DataFinalOcorrencia, ";
                    }
                    break;
                case "NomeProprietario":
                    if (!select.Contains(" NomeProprietario, "))
                    {
                        if (!joins.Contains(" Proprietario "))
                            joins += " JOIN T_CLIENTE Proprietario ON Proprietario.CLI_CGCCPF = Pagamento.CLI_CODIGO";

                        select += "Proprietario.CLI_NOME NomeProprietario, ";
                    }
                    break;
                case "CNPJProprietario":
                    if (!select.Contains(" CNPJProprietario, "))
                    {
                        if (!joins.Contains(" Proprietario "))
                            joins += " JOIN T_CLIENTE Proprietario ON Proprietario.CLI_CGCCPF = Pagamento.CLI_CODIGO";

                        select += "Proprietario.CLI_CGCCPF CNPJProprietario, ";
                    }
                    break;
                case "ValorPagamentoDocumento":
                    if (!select.Contains(" ValorPagamentoDocumento, "))
                    {
                        if (!joins.Contains(" Documento "))
                            joins += " JOIN T_PAGAMENTO_AGREGADO_DOCUMENTO Documento ON Documento.PAA_CODIGO = Pagamento.PAA_CODIGO";

                        select += "Documento.PDO_VALOR ValorPagamentoDocumento, ";
                    }
                    break;
                case "NumeroDocumento":
                    if (!select.Contains(" NumeroDocumento, "))
                    {
                        if (!joins.Contains(" Documento "))
                            joins += " JOIN T_PAGAMENTO_AGREGADO_DOCUMENTO Documento ON Documento.PAA_CODIGO = Pagamento.PAA_CODIGO";
                        if (!joins.Contains(" Conhecimento "))
                            joins += " JOIN T_CTE Conhecimento ON Conhecimento.CON_CODIGO = Documento.CON_CODIGO";
                        if (!joins.Contains(" Serie "))
                            joins += " JOIN T_EMPRESA_SERIE Serie ON Serie.ESE_CODIGO = Conhecimento.CON_SERIE";

                        select += "Conhecimento.CON_NUM NumeroDocumento, ";
                    }
                    break;
                case "SerieDocumento":
                    if (!select.Contains(" SerieDocumento, "))
                    {
                        if (!joins.Contains(" Documento "))
                            joins += " JOIN T_PAGAMENTO_AGREGADO_DOCUMENTO Documento ON Documento.PAA_CODIGO = Pagamento.PAA_CODIGO";
                        if (!joins.Contains(" Conhecimento "))
                            joins += " JOIN T_CTE Conhecimento ON Conhecimento.CON_CODIGO = Documento.CON_CODIGO";
                        if (!joins.Contains(" Serie "))
                            joins += " JOIN T_EMPRESA_SERIE Serie ON Serie.ESE_CODIGO = Conhecimento.CON_SERIE";

                        select += "Serie.ESE_NUMERO SerieDocumento, ";
                    }
                    break;
                case "ValorReceberDocumento":
                    if (!select.Contains(" ValorReceberDocumento, "))
                    {
                        if (!joins.Contains(" Documento "))
                            joins += " JOIN T_PAGAMENTO_AGREGADO_DOCUMENTO Documento ON Documento.PAA_CODIGO = Pagamento.PAA_CODIGO";
                        if (!joins.Contains(" Conhecimento "))
                            joins += " JOIN T_CTE Conhecimento ON Conhecimento.CON_CODIGO = Documento.CON_CODIGO";
                        if (!joins.Contains(" Serie "))
                            joins += " JOIN T_EMPRESA_SERIE Serie ON Serie.ESE_CODIGO = Conhecimento.CON_SERIE";

                        select += "Conhecimento.CON_VALOR_RECEBER ValorReceberDocumento, ";
                    }
                    break;
                case "Placas":
                    if (!select.Contains(" Placas, "))
                    {
                        if (!joins.Contains(" Documento "))
                            joins += " JOIN T_PAGAMENTO_AGREGADO_DOCUMENTO Documento ON Documento.PAA_CODIGO = Pagamento.PAA_CODIGO";
                        if (!joins.Contains(" Conhecimento "))
                            joins += " JOIN T_CTE Conhecimento ON Conhecimento.CON_CODIGO = Documento.CON_CODIGO";
                        if (!joins.Contains(" Serie "))
                            joins += " JOIN T_EMPRESA_SERIE Serie ON Serie.ESE_CODIGO = Conhecimento.CON_SERIE";

                        select += @"CAST(SUBSTRING((SELECT DISTINCT ', ' + V.CVE_PLACA + ' Cap. KG: ' + CONVERT(varchar(10), V.CVE_CAPACIDADE_KG)
                                    FROM T_CTE_VEICULO V
                                    WHERE V.CON_CODIGO = Conhecimento.CON_CODIGO  FOR XML PATH('')), 3, 1000) AS VARCHAR(1000)) Placas, ";
                    }
                    break;
                case "Motoristas":
                    if (!select.Contains(" Motoristas, "))
                    {
                        if (!joins.Contains(" Documento "))
                            joins += " JOIN T_PAGAMENTO_AGREGADO_DOCUMENTO Documento ON Documento.PAA_CODIGO = Pagamento.PAA_CODIGO";
                        if (!joins.Contains(" Conhecimento "))
                            joins += " JOIN T_CTE Conhecimento ON Conhecimento.CON_CODIGO = Documento.CON_CODIGO";
                        if (!joins.Contains(" Serie "))
                            joins += " JOIN T_EMPRESA_SERIE Serie ON Serie.ESE_CODIGO = Conhecimento.CON_SERIE";

                        select += @"CAST(SUBSTRING((SELECT DISTINCT ', ' + V.CMO_NOME_MOTORISTA
                                    FROM T_CTE_MOTORISTA V
                                    WHERE V.CON_CODIGO = Conhecimento.CON_CODIGO  FOR XML PATH('')), 3, 1000) AS VARCHAR(1000)) Motoristas, ";
                    }
                    break;
                case "Cargas":
                    if (!select.Contains(" Cargas, "))
                    {
                        if (!joins.Contains(" Documento "))
                            joins += " JOIN T_PAGAMENTO_AGREGADO_DOCUMENTO Documento ON Documento.PAA_CODIGO = Pagamento.PAA_CODIGO";
                        if (!joins.Contains(" Conhecimento "))
                            joins += " JOIN T_CTE Conhecimento ON Conhecimento.CON_CODIGO = Documento.CON_CODIGO";
                        if (!joins.Contains(" Serie "))
                            joins += " JOIN T_EMPRESA_SERIE Serie ON Serie.ESE_CODIGO = Conhecimento.CON_SERIE";

                        select += @"CAST(SUBSTRING((SELECT DISTINCT ', ' + CA.CAR_CODIGO_CARGA_EMBARCADOR
                                    FROM T_CARGA_CTE CC
                                    JOIN T_CARGA CA ON CA.CAR_CODIGO = CC.CAR_CODIGO
                                    WHERE CC.CON_CODIGO = Conhecimento.CON_CODIGO  FOR XML PATH('')), 3, 1000) AS VARCHAR(1000)) Cargas, ";
                    }
                    break;
                case "TipoOperacao":
                    if (!select.Contains(" TipoOperacao, "))
                    {
                        if (!joins.Contains(" Documento "))
                            joins += " JOIN T_PAGAMENTO_AGREGADO_DOCUMENTO Documento ON Documento.PAA_CODIGO = Pagamento.PAA_CODIGO";
                        if (!joins.Contains(" Conhecimento "))
                            joins += " JOIN T_CTE Conhecimento ON Conhecimento.CON_CODIGO = Documento.CON_CODIGO";
                        if (!joins.Contains(" Serie "))
                            joins += " JOIN T_EMPRESA_SERIE Serie ON Serie.ESE_CODIGO = Conhecimento.CON_SERIE";

                        select += @"CAST(SUBSTRING((SELECT DISTINCT ', ' + T.TOP_DESCRICAO
                            FROM T_CARGA_CTE CC
                            JOIN T_CARGA CA ON CA.CAR_CODIGO = CC.CAR_CODIGO
                            JOIN T_TIPO_OPERACAO T ON T.TOP_CODIGO = CA.TOP_CODIGO
                            WHERE CC.CON_CODIGO = Conhecimento.CON_CODIGO  FOR XML PATH('')), 3, 1000) AS VARCHAR(1000)) TipoOperacao, ";
                    }
                    break;
                case "Peso":
                    if (!select.Contains(" Peso, "))
                    {
                        if (!joins.Contains(" Documento "))
                            joins += " JOIN T_PAGAMENTO_AGREGADO_DOCUMENTO Documento ON Documento.PAA_CODIGO = Pagamento.PAA_CODIGO";
                        if (!joins.Contains(" Conhecimento "))
                            joins += " JOIN T_CTE Conhecimento ON Conhecimento.CON_CODIGO = Documento.CON_CODIGO";
                        if (!joins.Contains(" Serie "))
                            joins += " JOIN T_EMPRESA_SERIE Serie ON Serie.ESE_CODIGO = Conhecimento.CON_SERIE";

                        select += @"(SELECT SUM(NFC_PESO) FROM T_CTE_DOCS D
                            WHERE D.CON_CODIGO = Conhecimento.CON_CODIGO) Peso, ";
                    }
                    break;
                case "CodigoTitulo":
                    if (!select.Contains(" CodigoTitulo, "))
                    {
                        if (!joins.Contains(" Contrato "))
                            joins += " LEFT OUTER JOIN T_CONTRATO_FRETE_TERCEIRO Contrato ON Contrato.PAA_CODIGO = Pagamento.PAA_CODIGO";
                        if (!joins.Contains(" Titulo "))
                            joins += " LEFT OUTER JOIN T_TITULO Titulo ON Titulo.CFT_CODIGO = Contrato.CFT_CODIGO";

                        select += "Titulo.TIT_CODIGO CodigoTitulo, ";
                    }
                    break;
                case "DescricaoVencimentoTitulo":
                    if (!select.Contains(" VencimentoTitulo, "))
                    {
                        if (!joins.Contains(" Contrato "))
                            joins += " LEFT OUTER JOIN T_CONTRATO_FRETE_TERCEIRO Contrato ON Contrato.PAA_CODIGO = Pagamento.PAA_CODIGO";
                        if (!joins.Contains(" Titulo "))
                            joins += " LEFT OUTER JOIN T_TITULO Titulo ON Titulo.CFT_CODIGO = Contrato.CFT_CODIGO";

                        select += "Titulo.TIT_DATA_VENCIMENTO VencimentoTitulo, ";
                    }
                    break;
                case "DescricaoStatusTitulo":
                    if (!select.Contains(" StatusTitulo, "))
                    {
                        if (!joins.Contains(" Contrato "))
                            joins += " LEFT OUTER JOIN T_CONTRATO_FRETE_TERCEIRO Contrato ON Contrato.PAA_CODIGO = Pagamento.PAA_CODIGO";
                        if (!joins.Contains(" Titulo "))
                            joins += " LEFT OUTER JOIN T_TITULO Titulo ON Titulo.CFT_CODIGO = Contrato.CFT_CODIGO";

                        select += "Titulo.TIT_STATUS StatusTitulo, ";
                    }
                    break;
                case "ValorPendenteTitulo":
                    if (!select.Contains(" ValorPendenteTitulo, "))
                    {
                        if (!joins.Contains(" Contrato "))
                            joins += " LEFT OUTER JOIN T_CONTRATO_FRETE_TERCEIRO Contrato ON Contrato.PAA_CODIGO = Pagamento.PAA_CODIGO";
                        if (!joins.Contains(" Titulo "))
                            joins += " LEFT OUTER JOIN T_TITULO Titulo ON Titulo.CFT_CODIGO = Contrato.CFT_CODIGO";

                        select += "Titulo.TIT_VALOR_PENDENTE ValorPendenteTitulo, ";
                    }
                    break;
                default:
                    break;
            }
        }

        private void SetarWhereRelatorioConsultaRelatorioPagamentoAgregado(ref string where, ref string groupBy, ref string joins, ref List<ParametroSQL> parametros, int numeroInicial, int numeroFinal, DateTime dataPagamentoInicial, DateTime dataPagamentoFinal, int motorista, int veiculo, int tipoOperacao, int numeroCTe, string numeroCarga, double agregado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoAgregado? situacao)
        {
            string pattern = "yyyy-MM-dd";

            if (numeroInicial > 0 || numeroFinal > 0)
            {
                if (numeroInicial > 0)
                    where += " AND Pagamento.PAA_NUMERO >= " + numeroInicial;

                if (numeroFinal > 0)
                    where += " AND Pagamento.PAA_NUMERO <= " + numeroFinal;
            }
            if (dataPagamentoInicial != DateTime.MinValue || dataPagamentoFinal != DateTime.MinValue)
            {
                if (dataPagamentoInicial != DateTime.MinValue)
                    where += " AND Pagamento.PAA_DATA_PAGAMENTO >= '" + dataPagamentoInicial.ToString(pattern) + "' ";

                if (dataPagamentoFinal != DateTime.MinValue)
                    where += " AND Pagamento.PAA_DATA_PAGAMENTO <= '" + dataPagamentoFinal.ToString(pattern) + "' ";
            }

            if (motorista > 0)
            {
                where += " AND Conhecimento.CON_CODIGO IN (SELECT CC.CON_CODIGO FROM T_CTE_MOTORISTA CC JOIN T_FUNCIONARIO F ON F.FUN_CPF = CC.CMO_CPF_MOTORISTA WHERE F.FUN_CODIGO = " + motorista.ToString() + ") "; // SQL-INJECTION-SAFE
            }
            if (veiculo > 0)
            {
                where += " AND Conhecimento.CON_CODIGO IN (SELECT CC.CON_CODIGO FROM T_CTE_VEICULO CC WHERE CC.VEI_CODIGO = " + veiculo.ToString() + ") "; // SQL-INJECTION-SAFE
            }
            if (tipoOperacao > 0)
            {
                where += " AND Conhecimento.CON_CODIGO IN (SELECT CC.CON_CODIGO FROM T_CARGA_CTE CC JOIN T_CARGA CA ON CA.CAR_CODIGO = CC.CAR_CODIGO WHERE CA.TOP_CODIGO = " + tipoOperacao.ToString() + ") "; // SQL-INJECTION-SAFE
            }
            if (numeroCTe > 0)
            {
                if (!joins.Contains(" Documento "))
                    joins += " JOIN T_PAGAMENTO_AGREGADO_DOCUMENTO Documento ON Documento.PAA_CODIGO = Pagamento.PAA_CODIGO";
                if (!joins.Contains(" Conhecimento "))
                    joins += " JOIN T_CTE Conhecimento ON Conhecimento.CON_CODIGO = Documento.CON_CODIGO";

                where += " AND Conhecimento.CON_NUM = " + numeroCTe.ToString();
            }
            if (!string.IsNullOrWhiteSpace(numeroCarga))
            {
                where += " AND Conhecimento.CON_CODIGO IN (SELECT CC.CON_CODIGO FROM T_CARGA_CTE CC JOIN T_CARGA CA ON CA.CAR_CODIGO = CC.CAR_CODIGO WHERE CA.CAR_CODIGO_CARGA_EMBARCADOR = CA_CAR_CODIGO_CARGA_EMBARCADOR) ";
                parametros.Add(new ParametroSQL("CA_CAR_CODIGO_CARGA_EMBARCADOR ", numeroCarga)); 
            }

            if (agregado > 0)
                where += " AND Pagamento.CLI_CODIGO = " + agregado;

            if (situacao.HasValue)
            {
                where += " AND Pagamento.PAA_SITUACAO = " + situacao.Value.ToString("d");
            }
        }

        #endregion
    }
}
