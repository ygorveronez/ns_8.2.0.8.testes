using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Linq;

namespace Repositorio
{
    public class NFSe : RepositorioBase<Dominio.Entidades.NFSe>, Dominio.Interfaces.Repositorios.NFSe
    {
        public NFSe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.NFSe BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NFSe>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.NFSe BuscarPorCodigoNFS(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NFSe>();

            var result = from obj in query where obj.NFS.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.NFSe BuscarPorRPSSituacaoAmbiente(int codigoEmpresa, int numeroRPS, Dominio.Enumeradores.StatusNFSe? status, Dominio.Enumeradores.TipoAmbiente ambiente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NFSe>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.RPS.Numero == numeroRPS && obj.Ambiente == ambiente select obj;

            if (status != null)
                result = result.Where(o => o.Status == status);

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.NFSe BuscarPorNumeroEStatus(int codigoEmpresa, int numero, int serie, Dominio.Enumeradores.StatusNFSe? status, Dominio.Enumeradores.TipoAmbiente ambiente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NFSe>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Numero == numero && obj.Serie.Numero == serie && obj.Ambiente == ambiente select obj;

            if (status != null)
                result = result.Where(o => o.Status == status);

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.NFSe> Consultar(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, int numeroInicial, int numeroFinal, int serie, Dominio.Enumeradores.StatusNFSe? status, List<int> series, int numeroCarga, int numeroRPS, string numeroDocumento, string cnpjTomador, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NFSe>();

            var result = from obj in query where obj.Ambiente == obj.Empresa.TipoAmbiente select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao <= dataFinal.AddDays(1).Date);

            if (numeroInicial > 0)
                result = result.Where(o => o.Numero >= numeroInicial);

            if (numeroFinal > 0)
                result = result.Where(o => o.Numero <= numeroFinal);

            if (status != null)
                result = result.Where(o => o.Status == status.Value);

            if (serie > 0)
                result = result.Where(o => o.Serie.Codigo == serie);
            else if (series.Count > 0)
                result = result.Where(o => series.Contains(o.Serie.Codigo));

            if (!string.IsNullOrWhiteSpace(cnpjTomador))
                result = result.Where(o => o.Tomador.CPF_CNPJ == cnpjTomador);

            if (numeroRPS > 0)
                result = result.Where(o => o.RPS.Numero == numeroRPS);

            if (numeroCarga > 0)
            {
                var queryIntegracaoNFSe = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoNFSe>();
                var resultIntegracaoNFSe = from o in queryIntegracaoNFSe select o;
                if (numeroCarga > 0)
                    resultIntegracaoNFSe = resultIntegracaoNFSe.Where(o => o.NumeroDaCarga == numeroCarga);

                result = result.Where(o => resultIntegracaoNFSe.Select(c => c.NFSe.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(numeroDocumento))
            {
                var queryDocumentos = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosNFSe>();

                result = result.Where(o => (from obj in queryDocumentos where obj.NFSe.Codigo == o.Codigo && obj.Numero.Contains(numeroDocumento) select obj.NFSe.Codigo).Contains(o.Codigo));
            }

            return result.OrderByDescending(o => o.Serie.Numero)
                         .ThenByDescending(o => o.Numero)
                         .Skip(inicioRegistros)
                         .Take(maximoRegistros)
                         .ToList();
        }

        public List<Dominio.Entidades.NFSe> ConsultarTodas(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, int numeroInicial, int numeroFinal, int serie, Dominio.Enumeradores.StatusNFSe? status, List<int> series, int numeroCarga, int numeroRPS, string numeroDocumento, string cnpjTomador, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NFSe>();

            var result = from obj in query where obj.Ambiente == obj.Empresa.TipoAmbiente select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao <= dataFinal.AddDays(1).Date);

            if (numeroInicial > 0)
                result = result.Where(o => o.Numero >= numeroInicial);

            if (numeroFinal > 0)
                result = result.Where(o => o.Numero <= numeroFinal);

            if (status != null)
                result = result.Where(o => o.Status == status.Value);

            if (serie > 0)
                result = result.Where(o => o.Serie.Codigo == serie);
            else if (series.Count > 0)
                result = result.Where(o => series.Contains(o.Serie.Codigo));

            if (numeroRPS > 0)
                result = result.Where(o => o.RPS.Numero == numeroRPS);

            if (!string.IsNullOrWhiteSpace(cnpjTomador))
                result = result.Where(o => o.Tomador.CPF_CNPJ == cnpjTomador);

            if (numeroCarga > 0)
            {
                var queryIntegracaoNFSe = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoNFSe>();
                var resultIntegracaoNFSe = from o in queryIntegracaoNFSe select o;
                if (numeroCarga > 0)
                    resultIntegracaoNFSe = resultIntegracaoNFSe.Where(o => o.NumeroDaCarga == numeroCarga);

                result = result.Where(o => resultIntegracaoNFSe.Select(c => c.NFSe.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(numeroDocumento))
            {
                var queryDocumentos = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosNFSe>();

                result = result.Where(o => (from obj in queryDocumentos where obj.NFSe.Codigo == o.Codigo && obj.Numero.Contains(numeroDocumento) select obj.NFSe.Codigo).Contains(o.Codigo));
            }

            return result.OrderBy(o => o.RPS.Numero)
                         .Skip(inicioRegistros)
                         .Take(maximoRegistros)
                         .ToList();
        }

        public List<int> ConsultarSeExisteNFSePendente(int codigoEmpresa, DateTime dataFechamento, Dominio.Enumeradores.StatusNFSe[] status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NFSe>();

            query = query.Where(o => o.DataEmissao < dataFechamento.AddDays(1).Date && status.Contains(o.Status));

            if (codigoEmpresa > 0)
                query = query.Where(c => c.Empresa.Codigo == codigoEmpresa);

            return query.Select(o => o.Numero).ToList();
        }

        public int ContarConsulta(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, int numeroInicial, int numeroFinal, int serie, Dominio.Enumeradores.StatusNFSe? status, List<int> series, int numeroCarga, int numeroRPS, string numeroDocumento, string cnpjTomador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NFSe>();

            var result = from obj in query where obj.Ambiente == obj.Empresa.TipoAmbiente select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao <= dataFinal.AddDays(1).Date);

            if (numeroInicial > 0)
                result = result.Where(o => o.Numero >= numeroInicial);

            if (numeroFinal > 0)
                result = result.Where(o => o.Numero <= numeroFinal);

            if (status != null)
                result = result.Where(o => o.Status == status.Value);

            if (serie > 0)
                result = result.Where(o => o.Serie.Codigo == serie);
            else if (series.Count > 0)
                result = result.Where(o => series.Contains(o.Serie.Codigo));

            if (!string.IsNullOrWhiteSpace(cnpjTomador))
                result = result.Where(o => o.Tomador.CPF_CNPJ == cnpjTomador);

            if (numeroRPS > 0)
                result = result.Where(o => o.RPS.Numero == numeroRPS);

            if (numeroCarga > 0)
            {
                var queryIntegracaoNFSe = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoNFSe>();
                var resultIntegracaoNFSe = from o in queryIntegracaoNFSe select o;
                if (numeroCarga > 0)
                    resultIntegracaoNFSe = resultIntegracaoNFSe.Where(o => o.NumeroDaCarga == numeroCarga);

                result = result.Where(o => resultIntegracaoNFSe.Select(c => c.NFSe.Codigo).Contains(o.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(numeroDocumento))
            {
                var queryDocumentos = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosNFSe>();

                result = result.Where(o => (from obj in queryDocumentos where obj.NFSe.Codigo == o.Codigo && obj.Numero.Contains(numeroDocumento) select obj.NFSe.Codigo).Contains(o.Codigo));
            }


            return result.Count();
        }

        public int ObterUltimoNumero(int codigoEmpresa, int codigoSerie)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NFSe>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Serie.Codigo == codigoSerie select obj;

            int? retorno = result.Max(o => (int?)o.Numero);

            return retorno.HasValue ? retorno.Value : 0;
        }

        public List<Dominio.Entidades.NFSe> BuscarPorStatus(Dominio.Enumeradores.StatusNFSe[] statusNFSe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NFSe>();

            var result = from obj in query where statusNFSe.Contains(obj.Status) select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.NFSe> BuscarNFSesPendentes(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NFSe>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && (obj.Status != Dominio.Enumeradores.StatusNFSe.Autorizado && obj.Status != Dominio.Enumeradores.StatusNFSe.Cancelado) select obj;

            return result.ToList();
        }

        public IList<Dominio.ObjetosDeValor.Relatorios.RelatorioNFSeEmitidasPorEmbarcador> RelatorioNFSeEmitidasPorEmbarcador(int codigoEmpresaPai, int codigoEmpresa, string cpfCnpjEmbarcador, DateTime dataAutorizacaoInicial, DateTime dataAutorizacaoFinal, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, bool todosCNPJdaRaizEmbarcador = false, string cnpjEmbarcadorUsuario = "")
        {//falta terminar--criar relatório
            string query = @"SELECT N.NFSE_CODIGO Codigo, 
                                    E.EMP_CNPJ CNPJTransportador, 
	                                E.EMP_RAZAO Transportador,
	                                R.RPS_NUMERO RPSNumero,
	                                R.RPS_SERIE RPSSerie,
	                                R.RPS_PROTOCOLO RPSProtocolo,
	                                R.RPS_DATA RPSData,
	                                R.RPS_RETORNO_CODIGO RPSRetornoCodigo,
	                                R.RPS_RETORNO_MENSAGEM RPSRetornoMensagem,
	                                R.RPS_PROTOCOLO_CANCELAMENTO RPSProtocoloCancelamento,
	                                R.RPS_DATA_RETORNO_CANCELAMENTO RPSDataCancelamento,
	                                N.NFSE_NUMERO Numero,
	                                ES.ESE_NUMERO Serie,
	                                N.NFSE_NUMERO_SUBSTITUICAO NumeroSubstituicao,
	                                N.NFSE_SERIE_SUBSTITUICAO SerieSubstituicao,
	                                '' [Log],
	                                N.NFSE_DATA_INTEGRACAO DataIntegracao,
	                                N.NFSE_DATA_EMISSAO DataEmissao,
	                                N.NFSE_VALOR_SERVICOS ValorServicos,
	                                N.NFSE_VALOR_DEDUCOES ValorDeducoes,
	                                N.NFSE_VALOR_PIS ValorPIS,
	                                N.NFSE_VALOR_COFINS ValorCOFINS,
	                                N.NFSE_VALOR_INSS ValorINSS,
	                                N.NFSE_VALOR_IR ValorIR,
	                                N.NFSE_VALOR_CSLL ValorCSLL,
	                                N.NFSE_VALOR_ISS_RETIDO ValorISSRetido,
	                                N.NFSE_VALOR_OUTRAS_RETENCOES ValorOutrasRetencoes,
	                                N.NFSE_VALOR_DESC_INCONDICIONADO ValorDescIncondicionado,
	                                N.NFSE_VALOR_DESC_CONDICIONADO ValorDescCondicionado,
	                                N.NFSE_ALIQUOTA_ISS AliquotaISS,
	                                N.NFSE_BASE_CALCULO_ISS BaseCalculoISS,
	                                N.NFSE_VALOR_ISS ValorISS,
	                                N.NFSE_OUTRAS_INFORMACOES OutrasInformacoes,
	                                T.PNF_CPF_CNPJ CPFCNPJTomador,
	                                T.PNF_NOME NomeTomador,
	                                S.SER_DESCRICAO ItemDescricao,
	                                S.SER_NUMERO ItemNumero,
                                    I.NFI_DISCRIMINACAO ItemDiscriminacao,
	                                I.NFI_QUANTIDADE ItemQuantidade,
	                                L.LOC_DESCRICAO + '-' +L.UF_SIGLA ItemMunicipio,
                                    LI.LOC_DESCRICAO + '-' +LI.UF_SIGLA ItemMunicipioIncidencia,
	                                N.NFSE_STATUS [Status]
                             FROM T_NFSE N
                              JOIN T_EMPRESA E ON N.EMP_CODIGO = E.EMP_CODIGO
                              JOIN T_EMPRESA_SERIE ES ON N.ESE_CODIGO = ES.ESE_CODIGO
                              JOIN T_NFSE_RPS R ON N.RPS_CODIGO = R.RPS_CODIGO
                              JOIN T_NFSE_PARTICIPANTE T ON N.PNF_CODIGO_TOMADOR = T.PNF_CODIGO
                              JOIN T_NFSE_ITEM I ON N.NFSE_CODIGO = I.NFSE_CODIGO
                              JOIN T_NFSE_SERVICO S ON I.SER_CODIGO = S.SER_CODIGO
                              JOIN T_LOCALIDADES L ON I.LOC_MUNICIPIO = L.LOC_CODIGO
                              JOIN T_LOCALIDADES LI ON I.LOC_MUNICIPIO_INCIDENCIA = LI.LOC_CODIGO
                             WHERE E.EMP_EMPRESA = " + codigoEmpresaPai.ToString() + " AND N.NFSE_STATUS IN (3,5)";

            if (codigoEmpresa > 0)
                query += " AND N.EMP_CODIGO = " + codigoEmpresa.ToString();

            if (!string.IsNullOrWhiteSpace(cpfCnpjEmbarcador))
            {
                if (todosCNPJdaRaizEmbarcador)
                    query += " AND T.PNF_CPF_CNPJ like '" + cpfCnpjEmbarcador.Substring(0, 8) + "%'";
                else
                    query += " AND T.PNF_CPF_CNPJ = '" + cpfCnpjEmbarcador + "'";
            }

            if (!string.IsNullOrWhiteSpace(cnpjEmbarcadorUsuario))
            {
                if (cnpjEmbarcadorUsuario.Length == 8)
                    query += " AND T.PNF_CPF_CNPJ like '" + cnpjEmbarcadorUsuario + "%'";
                else
                    query += " AND T.PNF_CPF_CNPJ = '" + cnpjEmbarcadorUsuario + "'";
            }

            if (dataAutorizacaoInicial != DateTime.MinValue)
                query += " AND R.RPS_DATA >= '" + dataAutorizacaoInicial.ToString("MM/dd/yyyy") + "'";

            if (dataAutorizacaoFinal != DateTime.MinValue)
                query += " AND R.RPS_DATA < '" + dataAutorizacaoFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";

            if (dataEmissaoInicial != DateTime.MinValue)
                query += " AND N.NFSE_DATA_EMISSAO >= '" + dataEmissaoInicial.ToString("MM/dd/yyyy") + "'";

            if (dataEmissaoFinal != DateTime.MinValue)
                query += " AND N.NFSE_DATA_EMISSAO < '" + dataEmissaoFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Relatorios.RelatorioNFSeEmitidasPorEmbarcador)));

            return nhQuery.List<Dominio.ObjetosDeValor.Relatorios.RelatorioNFSeEmitidasPorEmbarcador>();
        }

        public Dominio.Entidades.NFSe BuscarPorVeiculo(int codigoEmpresa, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, int codigoVeiculo, DateTime dataEmissao, Dominio.Enumeradores.StatusNFSe? status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NFSe>();
            var result = from obj in query
                         where obj.Ambiente == tipoAmbiente &&
                              obj.Empresa.Codigo == codigoEmpresa
                         select obj;

            if (status != null)
                result = result.Where(o => o.Status == status);

            if (dataEmissao > DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataEmissao && o.DataEmissao <= dataEmissao.AddDays(1).Date);

            if (codigoVeiculo > 0)
                result = result.Where(o => o.Veiculo != null && o.Veiculo.Codigo == codigoVeiculo);
            else if (codigoVeiculo < 0)
                result = result.Where(o => o.Veiculo == null);

            return result.Timeout(120).FirstOrDefault();
        }

        public Dominio.Entidades.NFSe BuscarPorTomador(int codigoEmpresa, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, string CNPJTomador, DateTime dataEmissao, Dominio.Enumeradores.StatusNFSe? status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NFSe>();
            var result = from obj in query
                         where obj.Ambiente == tipoAmbiente &&
                              obj.Empresa.Codigo == codigoEmpresa
                         select obj;

            if (status != null)
                result = result.Where(o => o.Status == status);

            if (dataEmissao > DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataEmissao && o.DataEmissao <= dataEmissao.AddDays(1).Date);

            if (!string.IsNullOrWhiteSpace(CNPJTomador))
                result = result.Where(o => o.Tomador != null && o.Tomador.CPF_CNPJ.Equals(CNPJTomador));

            return result.Timeout(120).FirstOrDefault();
        }

        public Dominio.Entidades.NFSe BuscarPorNFe(int codigoEmpresa, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, string chaveNFe, DateTime dataEmissao, Dominio.Enumeradores.StatusNFSe[] status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NFSe>();
            var result = from obj in query
                         where obj.Ambiente == tipoAmbiente &&
                              obj.Empresa.Codigo == codigoEmpresa &&
                              status.Contains(obj.Status)
                         select obj;

            if (dataEmissao > DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataEmissao && o.DataEmissao <= dataEmissao.AddDays(1).Date);

            if (!string.IsNullOrWhiteSpace(chaveNFe))
            {
                var queryDocumentos = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosNFSe>();

                result = result.Where(o => (from obj in queryDocumentos where obj.NFSe.Codigo == o.Codigo && obj.Chave == chaveNFe select obj.NFSe.Codigo).Contains(o.Codigo));
            }

            return result.Timeout(120).FirstOrDefault();
        }


        public List<Dominio.ObjetosDeValor.Relatorios.RelatorioNFSeEmitidasPorEmbarcador> RelatorioNFSeEmitidas(int codigoEmpresa, string cpfCnpjTomador, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, int numeroInicial, int numeroFinal, Dominio.Enumeradores.StatusNFSe? status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NFSe>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (!string.IsNullOrWhiteSpace(cpfCnpjTomador))
                result = result.Where(o => o.Tomador.CPF_CNPJ.Equals(cpfCnpjTomador));

            if (dataEmissaoInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataEmissaoInicial.Date);

            if (dataEmissaoFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao <= dataEmissaoFinal.AddDays(1).Date);

            if (numeroInicial > 0)
                result = result.Where(o => o.Numero >= numeroInicial);

            if (numeroFinal > 0)
                result = result.Where(o => o.Numero <= numeroFinal);

            if (status != null)
                result = result.Where(o => o.Status == status.Value);

            return result.Select(o => new Dominio.ObjetosDeValor.Relatorios.RelatorioNFSeEmitidasPorEmbarcador()
            {
                Codigo = o.Codigo,
                Numero = o.Numero,
                DataEmissao = o.DataEmissao,
                CPFCNPJTomador = o.Tomador.CPF_CNPJ,
                NomeTomador = o.Tomador.Nome,
                ValorServicos = Math.Round(o.ValorServicos, 2, MidpointRounding.ToEven),
                BaseCalculoISS = o.BaseCalculoISS,
                AliquotaISS = o.AliquotaISS,
                ValorISS = Math.Round(o.ValorISS, 2, MidpointRounding.ToEven),
                OutrasInformacoes = o.OutrasInformacoes,
                RPSProtocolo = o.CodigoVerificacao,
                Status = o.Status
            })
            .OrderBy(o => o.Numero)
            .Timeout(120)
            .ToList();
        }

        public List<int> BuscarListaCodigos(int codigoEmpresa, string cpfCnpjTomador, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, int numeroInicial, int numeroFinal, Dominio.Enumeradores.StatusNFSe? status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NFSe>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (!string.IsNullOrWhiteSpace(cpfCnpjTomador))
                result = result.Where(o => o.Tomador.CPF_CNPJ.Equals(cpfCnpjTomador));

            if (dataEmissaoInicial != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataEmissaoInicial.Date);

            if (dataEmissaoFinal != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao <= dataEmissaoFinal.AddDays(1).Date);

            if (numeroInicial > 0)
                result = result.Where(o => o.Numero >= numeroInicial);

            if (numeroFinal > 0)
                result = result.Where(o => o.Numero <= numeroFinal);

            if (status != null)
                result = result.Where(o => o.Status == status.Value);

            return result.Select(o => o.Codigo).Timeout(120).ToList();
        }

        public List<int> BuscarListaProtocolos(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NFSe>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && (obj.Status == Dominio.Enumeradores.StatusNFSe.Autorizado || obj.Status == Dominio.Enumeradores.StatusNFSe.Cancelado) select obj;

            if (dataInicial.Date > DateTime.MinValue)
                result = result.Where(o => o.DataEmissao >= dataInicial.Date);

            if (dataFinal.Date > DateTime.MinValue)
                result = result.Where(o => o.DataEmissao < dataFinal.AddDays(1).Date);

            return result.Select(o => o.Codigo).Timeout(120).ToList();
        }

        public List<Dominio.Entidades.NFSe> BuscarPorCodigosNFSe(List<int> codigosNFes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NFSe>();

            var result = from obj in query where codigosNFes.Contains(obj.Codigo) select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.NFSe> BuscarListaPorChaveNFe(string chaveNFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NFSe>();
            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(chaveNFe))
            {
                var queryDocumentos = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentosNFSe>();

                result = result.Where(o => (from obj in queryDocumentos where obj.NFSe.Codigo == o.Codigo && obj.Chave == chaveNFe select obj.NFSe.Codigo).Contains(o.Codigo));
            }

            return result.OrderByDescending(o => o.Codigo).Timeout(120).ToList();
        }

        public int VerificaNFSeJaAutorizada(int numero, int codigoSerie, int codigoEmpresa, int ano, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, string codigoVerificador)
        {
            IQueryable<Dominio.Entidades.NFSe> query = this.SessionNHiBernate.Query<Dominio.Entidades.NFSe>();

            query = query.Where(o => o.Status == Dominio.Enumeradores.StatusNFSe.Autorizado && o.Numero == numero && o.Serie.Codigo == codigoSerie && o.Empresa.Codigo == codigoEmpresa && o.Ambiente == tipoAmbiente && o.DataEmissao.Year == ano && o.CodigoVerificacao == codigoVerificador);

            return query.Count();
        }
    }
}
