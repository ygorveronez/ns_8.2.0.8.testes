using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using Repositorio.Embarcador.PedidoVenda.Consulta;
using System.Text;

namespace Repositorio.Embarcador.PedidoVenda
{
    public class PedidoVenda : RepositorioBase<Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda>
    {
        public PedidoVenda(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda> BuscarPorCodigos(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda>();
            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda> Consulta(Dominio.ObjetosDeValor.Embarcador.PedidosVendas.FiltroPesquisaPedidoVenda filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda> result = Consulta(filtrosPesquisa);

            result = result
                .Fetch(o => o.Cliente)
                .Fetch(o => o.Veiculo);

            return ObterLista(result, parametroConsulta);
        }

        public int ContaConsulta(Dominio.ObjetosDeValor.Embarcador.PedidosVendas.FiltroPesquisaPedidoVenda filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda> result = Consulta(filtrosPesquisa);

            return result.Count();
        }

        public int BuscarUltimoNumero(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;
            if (result.Count() > 0)
                return result.Max(obj => obj.Numero);
            else
                return 0;
        }

        public List<Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda> ConsultarOrdemServicoPendente(int codigoEmpresa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda>();

            query = query.Where(obj => obj.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPedidoVenda.PendenteOperacional && obj.Veiculo != null && obj.Empresa.Codigo == codigoEmpresa);

            return query.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultarOrdemServicoPendente(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda>();

            query = query.Where(obj => obj.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPedidoVenda.PendenteOperacional && obj.Veiculo != null && obj.Empresa.Codigo == codigoEmpresa);

            return query.Count();
        }


        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda> Consulta(Dominio.ObjetosDeValor.Embarcador.PedidosVendas.FiltroPesquisaPedidoVenda filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda>();

            var result = from obj in query select obj;

            if (filtrosPesquisa.NumeroInicial > 0 && filtrosPesquisa.NumeroFinal > 0)
                result = result.Where(obj => obj.Numero >= filtrosPesquisa.NumeroInicial && obj.Numero <= filtrosPesquisa.NumeroFinal);
            else if (filtrosPesquisa.NumeroInicial > 0)
                result = result.Where(obj => obj.Numero == filtrosPesquisa.NumeroInicial);
            else if (filtrosPesquisa.NumeroFinal > 0)
                result = result.Where(obj => obj.Numero == filtrosPesquisa.NumeroFinal);

            if (filtrosPesquisa.DataEmissaoInicial > DateTime.MinValue && filtrosPesquisa.DataEmissaoFinal > DateTime.MinValue)
                result = result.Where(obj => obj.DataEmissao.Value.Date >= filtrosPesquisa.DataEmissaoInicial && obj.DataEmissao.Value.Date <= filtrosPesquisa.DataEmissaoFinal);
            else if (filtrosPesquisa.DataEmissaoInicial > DateTime.MinValue)
                result = result.Where(obj => obj.DataEmissao.Value.Date == filtrosPesquisa.DataEmissaoInicial);
            else if (filtrosPesquisa.DataEmissaoFinal > DateTime.MinValue)
                result = result.Where(obj => obj.DataEmissao.Value.Date == filtrosPesquisa.DataEmissaoFinal);

            if (filtrosPesquisa.StatusPedidoVenda.HasValue)
            {
                if (filtrosPesquisa.StatusPedidoVenda == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPedidoVenda.AbertaFinalizada)
                    result = result.Where(obj => obj.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPedidoVenda.Aberta || obj.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPedidoVenda.Finalizada
                    || obj.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPedidoVenda.Faturada);
                else
                    result = result.Where(obj => obj.Status == filtrosPesquisa.StatusPedidoVenda.Value);
            }

            int tipoPedidoVenda = (int)filtrosPesquisa.TipoPedidoVenda;

            if (tipoPedidoVenda == -1)
                result = result.Where(obj => obj.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedidoVenda.Todos && obj.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedidoVenda.OrdemServicoPet);
            if (tipoPedidoVenda > 0)
                result = result.Where(obj => obj.Tipo == filtrosPesquisa.TipoPedidoVenda);

            if (filtrosPesquisa.CnpjCpfCliente > 0)
            {
                if (filtrosPesquisa.TipoPedidoVenda == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedidoVenda.OrdemServicoPet)
                    result = result.Where(obj => obj.Pet.Tutor.CPF_CNPJ == filtrosPesquisa.CnpjCpfCliente);
                else
                    result = result.Where(obj => obj.Cliente.CPF_CNPJ == filtrosPesquisa.CnpjCpfCliente);
            }

            if (filtrosPesquisa.CodigoFuncionario > 0)
                result = result.Where(obj => obj.Funcionario.Codigo == filtrosPesquisa.CodigoFuncionario);

            if (filtrosPesquisa.CodigoPet > 0)
                result = result.Where(obj => obj.Pet.Codigo == filtrosPesquisa.CodigoPet);

            if (filtrosPesquisa.CodigoVeiculo > 0)
                result = result.Where(obj => obj.Veiculo.Codigo == filtrosPesquisa.CodigoVeiculo);

            if (filtrosPesquisa.NumeroInternoInicial > 0)
                result = result.Where(obj => obj.NumeroInterno >= filtrosPesquisa.NumeroInternoInicial);
            if (filtrosPesquisa.NumeroInternoFinal > 0)
                result = result.Where(obj => obj.NumeroInterno <= filtrosPesquisa.NumeroInternoFinal);

            result = result.Where(obj => obj.Empresa.Codigo == filtrosPesquisa.CodigoEmpresa);

            return result;
        }

        #endregion

        #region Relatórios

        public IList<Dominio.Relatorios.Embarcador.DataSource.PedidosVendas.RelatorioPedidoOrdemVenda> ConsultarRelatorio(Dominio.ObjetosDeValor.Embarcador.PedidosVendas.FiltroPesquisaRelatorioPedidoVenda filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = new ConsultaPedidoVenda().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.PedidosVendas.RelatorioPedidoOrdemVenda)));

            return consulta.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.PedidosVendas.RelatorioPedidoOrdemVenda>();
        }

        public int ContarConsultaRelatorio(Dominio.ObjetosDeValor.Embarcador.PedidosVendas.FiltroPesquisaRelatorioPedidoVenda filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consulta = new ConsultaPedidoVenda().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consulta.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.PedidosVendas.RelatorioPedidoOrdemVendaItens> ConsultarItensRelatorio(Dominio.ObjetosDeValor.Embarcador.PedidosVendas.FiltroPesquisaRelatorioPedidoVenda filtrosPesquisa)
        {
            var consulta = this.SessionNHiBernate.CreateSQLQuery(new ConsultaPedidoVenda().ObterSqlPesquisaItens(filtrosPesquisa));

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.PedidosVendas.RelatorioPedidoOrdemVendaItens)));

            return consulta.List<Dominio.Relatorios.Embarcador.DataSource.PedidosVendas.RelatorioPedidoOrdemVendaItens>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.PedidosVendas.RelatorioPedidoVenda> RelatorioPedidoVenda(int codigoPedidoVenda)
        {
            string query = @"   SELECT P.PEV_NUMERO Numero,
                                P.PEV_DATA_EMISSAO DataEmissao,
                                P.PEV_DATA_ENTREGA DataEntrega,
                                P.PEV_OBSERVACAO Observacao,
                                P.PEV_TIPO Tipo,
                                P.PEV_STATUS Status,
                                P.PEV_VALOR_TOTAL ValorTotal,
                                P.PEV_VALOR_PRODUTOS ValorProdutos,
                                P.PEV_VALOR_SERVICOS ValorServicos,
                                P.PEV_FORMA_PAGAMENTO FormaPagamento,
                                P.PEV_REFERENCIA Referencia,

                                I.PVI_CODIGO CodigoItemBanco,
                                I.PRO_CODIGO CodigoProduto,
                                I.SER_CODIGO CodigoServico,
                                I.PVI_CODIGO_ITEM CodigoItem,
                                I.PVI_DESCRICAO_ITEM DescricaoItem,
                                ISNULL(PRO.PRO_COD_NCM, '') CodigoNCM,
                                I.PVI_QUANTIDADE QuantidadeItem,
                                I.PVI_VALOR_UNITARIO ValorUnitarioItem,
                                I.PVI_VALOR_TOTAL ValorTotalItem,

                                E.EMP_CNPJ CNPJEmpresa,
                                E.EMP_FANTASIA FantasiaEmpresa,
                                E.EMP_ENDERECO EnderecoEmpresa,
                                E.EMP_NUMERO NumeroEnderecoEmpresa,
                                E.EMP_CEP CEPEmpresa,
                                E.EMP_BAIRRO BairroEmpresa,
                                E.EMP_FONE FoneEmpresa,
                                LEMP.LOC_DESCRICAO CidadeEmpresa,
                                LEMP.UF_SIGLA EstadoEmpresa,
                                E.EMP_TIPO TipoEmpresa,
                                E.EMP_CASAS_QUANTIDADE_PRODUTO_NFE CasasQuantidadeProdutoNFe,
                                E.EMP_CASAS_VALOR_PRODUTO_NFE CasasValorProdutoNFe,

                                C.CLI_CGCCPF CNPJPessoa,
                                CONCAT( C.CLI_NOME, '  ', '(',  C.CLI_CODIGO_INTEGRACAO, ')' ) NomePessoa,
                                C.CLI_ENDERECO EnderecoPessoa,
                                C.CLI_NUMERO NumeroEnderecoPessoa,
                                C.CLI_BAIRRO BairroPessoa,
                                C.CLI_CEP CEPPessoa,
                                LCLI.LOC_DESCRICAO CidadePessoa,
                                LCLI.UF_SIGLA EstadoPessoa,
                                C.CLI_FONE FonePessoa,
                                C.CLI_FISJUR TipoPessoa,

                                F.FUN_NOME NomeFuncionario,
                                (SELECT COUNT(PVP_CODIGO) QtdParcela FROM T_PEDIDO_VENDA_PARCELA A WHERE P.PEV_CODIGO = A.PEV_CODIGO) QtdParcela

                                FROM T_PEDIDO_VENDA P
                                LEFT OUTER JOIN T_PEDIDO_VENDA_ITENS I ON I.PEV_CODIGO = P.PEV_CODIGO
                                JOIN T_EMPRESA E ON E.EMP_CODIGO = P.EMP_CODIGO
                                JOIN T_CLIENTE C ON C.CLI_CGCCPF = P.CLI_CGCCPF
                                JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = P.FUN_CODIGO
                                JOIN T_LOCALIDADES LEMP ON LEMP.LOC_CODIGO = E.LOC_CODIGO
                                JOIN T_LOCALIDADES LCLI ON LCLI.LOC_CODIGO = C.LOC_CODIGO
								LEFT OUTER JOIN T_PRODUTO PRO ON PRO.PRO_CODIGO = I.PRO_CODIGO
                                WHERE 1 = 1";

            if (codigoPedidoVenda > 0)
                query += " AND P.PEV_CODIGO = " + codigoPedidoVenda.ToString();

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.PedidosVendas.RelatorioPedidoVenda)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.PedidosVendas.RelatorioPedidoVenda>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.PedidosVendas.RelatorioOrdemServicoPet> RelatorioOrdemServicoPet(int codigoPedidoVenda)
        {
            string query = @"SELECT P.PEV_NUMERO Numero,
	                        P.PEV_PESO Peso,
	                        P.PEV_DATA_EMISSAO DataEmissao,
                            P.PEV_DATA_ENTREGA DataEntrega,
	                        P.PEV_STATUS_ORDEM_SERVICO_PET StatusOrdemServico,
                            P.PEV_VALOR_PRODUTOS ValorProdutos,
                            P.PEV_VALOR_SERVICOS ValorServicos,
		                    P.PEV_VALOR_TOTAL ValorTotal,
	                        P.PEV_OBSERVACAO Observacao,
        
		                    E.EMP_FANTASIA FantasiaEmpresa,
	                        E.EMP_CNPJ CNPJEmpresa,
	                        E.EMP_FONE FoneEmpresa,
                            E.EMP_ENDERECO EnderecoEmpresa,
	                        E.EMP_BAIRRO BairroEmpresa,
	                        E.EMP_CEP CEPEmpresa,
                            E.EMP_NUMERO NumeroEnderecoEmpresa,
                            E.EMP_TIPO TipoEmpresa,
		                    LEMP.LOC_DESCRICAO CidadeEmpresa,
		                    LEMP.UF_SIGLA EstadoEmpresa,

		                    I.PVI_CODIGO CodigoItemBanco,
	                        I.PRO_CODIGO CodigoProduto,
	                        I.SER_CODIGO CodigoServico,
	                        I.PVI_CODIGO_ITEM CodigoItem,
	                        I.PVI_DESCRICAO_ITEM DescricaoItem,
	                        ISNULL(PRO.PRO_COD_NCM, '') CodigoNCM,
	                        I.PVI_QUANTIDADE QuantidadeItem,
	                        I.PVI_VALOR_UNITARIO ValorUnitarioItem,
	                        I.PVI_VALOR_TOTAL ValorTotalItem,

		                    PET.PET_NOME PetNome,
		                    PET.PET_SEXO SexoAnimal,
	                        PET.PET_DATA_NASCIMENTO DataNascimento,
	                        PET.PET_PORTE PorteAnimal,
		                    ESP.ESP_DESCRICAO Especie,
	                        ESR.ESR_DESCRICAO Raca,
	                        COR.COR_DESCRICAO Cor,

		                    CONCAT( C.CLI_NOME, '  ', '(',  C.CLI_CODIGO_INTEGRACAO, ')' ) NomePessoa,
		                    C.CLI_FONE FonePessoa,
	                        C.CLI_CGCCPF CNPJPessoa,
                            C.CLI_ENDERECO EnderecoPessoa,
                            C.CLI_BAIRRO BairroPessoa,
                            C.CLI_CEP CEPPessoa,
	                        C.CLI_NUMERO NumeroEnderecoPessoa,
                            C.CLI_FISJUR TipoPessoa,
		                    LCLI.LOC_DESCRICAO CidadePessoa,
		                    LCLI.UF_SIGLA EstadoPessoa,

		                    F.FUN_NOME NomeFuncionario
		
		                    FROM T_PEDIDO_VENDA P
		                    JOIN T_EMPRESA E ON E.EMP_CODIGO = P.EMP_CODIGO
		                    JOIN T_LOCALIDADES LEMP ON LEMP.LOC_CODIGO = E.LOC_CODIGO

		                    LEFT JOIN T_PEDIDO_VENDA_ITENS I ON I.PEV_CODIGO = P.PEV_CODIGO
		                    LEFT JOIN T_PRODUTO PRO ON PRO.PRO_CODIGO = I.PRO_CODIGO

		                    JOIN T_PET PET ON PET.PET_CODIGO = P.PET_CODIGO
	                        JOIN T_ESPECIE ESP ON ESP.ESP_CODIGO = PET.ESP_CODIGO
	                        JOIN T_ESPECIE_RACA ESR ON ESR.ESR_CODIGO = PET.ESR_CODIGO
	                        JOIN T_COR_ANIMAL COR ON COR.COR_CODIGO = PET.COR_CODIGO

		                    JOIN T_CLIENTE C ON C.CLI_CGCCPF = PET.CLI_CGCCPF
		                    JOIN T_LOCALIDADES LCLI ON LCLI.LOC_CODIGO = C.LOC_CODIGO

		                    JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = P.FUN_CODIGO

                            WHERE P.PEV_TIPO = 4";

            if (codigoPedidoVenda > 0)
                query += " AND P.PEV_CODIGO = " + codigoPedidoVenda.ToString();

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.PedidosVendas.RelatorioOrdemServicoPet)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.PedidosVendas.RelatorioOrdemServicoPet>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.PedidosVendas.RelatorioOrdemServicoVenda> RelatorioOrdemServicoVenda(int codigoPedidoVenda)
        {
            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(ConsultarRelatorioOrdemServicoVenda(codigoPedidoVenda, false));

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.PedidosVendas.RelatorioOrdemServicoVenda)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.PedidosVendas.RelatorioOrdemServicoVenda>();
        }

        public int ContarRelatorioOrdemServicoVenda(int codigoPedidoVenda)
        {
            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(ConsultarRelatorioOrdemServicoVenda(codigoPedidoVenda, true));

            return nhQuery.UniqueResult<int>();
        }

        private string ConsultarRelatorioOrdemServicoVenda(int codigoPedidoVenda, bool contarRegistros)
        {
            StringBuilder queryBuilder = new StringBuilder();

            if (contarRegistros)
            {
                queryBuilder.Append("SELECT COUNT(1) ");
            }
            else
            {
                queryBuilder.Append(@"SELECT P.PEV_NUMERO Numero,
                                        P.PEV_DATA_EMISSAO DataEmissao,
                                        P.PEV_DATA_ENTREGA DataEntrega,
                                        P.PEV_OBSERVACAO Observacao,
                                        P.PEV_TIPO Tipo,
                                        P.PEV_STATUS Status,
                                        P.PEV_VALOR_TOTAL ValorTotal,
                                        P.PEV_VALOR_PRODUTOS ValorProdutos,
                                        P.PEV_VALOR_SERVICOS ValorServicos,
                                        P.PEV_VALOR_DESCONTO ValorDesconto,
                                        P.PEV_KM KM,
                                        P.PEV_PESSOA_SOLICITANTE PessoaSolicitante,

                                        I.PVI_CODIGO CodigoItemBanco,
                                        I.PVI_CODIGO_ITEM CodigoItem,
                                        I.PVI_DESCRICAO_ITEM DescricaoItem,
                                        I.PVI_QUANTIDADE QuantidadeItem,
                                        I.PVI_VALOR_UNITARIO ValorUnitarioItem,
                                        I.PVI_VALOR_TOTAL ValorTotalItem,
                                        I.PVI_TIPO_SERVICO TipoServico,
                                        CI.CLI_NOME FornecedorServico,
                                        FI.FUN_NOME FuncionarioServico,
                                        CASE WHEN I.PVI_KM_INICIAL = 0 THEN NULL ELSE I.PVI_KM_INICIAL END KMInicial,
                                        CASE WHEN I.PVI_KM_FINAL = 0 THEN NULL ELSE I.PVI_KM_FINAL END KMFinal,
                                        CASE WHEN I.PVI_KM_TOTAL = 0 THEN NULL ELSE I.PVI_KM_TOTAL END KMTotal,
                                        CAST(I.PVI_HORA_INICIAL as VARCHAR(5)) HoraInicial,
                                        CAST(I.PVI_HORA_FINAL as VARCHAR(5)) HoraFinal,
                                        CAST(I.PVI_HORA_TOTAL as VARCHAR(5)) HoraTotal,

                                        CAST(I.PVI_HORA_INICIAL_SECUNDARIA as VARCHAR(5)) HoraInicial2,
                                        CAST(I.PVI_HORA_FINAL_SECUNDARIA as VARCHAR(5)) HoraFinal2,
								        CASE WHEN I.PVI_KM_INICIAL_SECUNDARIO = 0 THEN NULL ELSE I.PVI_KM_INICIAL_SECUNDARIO END KMInicial2,
                                        CASE WHEN I.PVI_KM_FINAL_SECUNDARIO = 0 THEN NULL ELSE I.PVI_KM_FINAL_SECUNDARIO END KMFinal2,
								        I.PVI_VALOR_HORA ValorHora, 
								        I.PVI_VALOR_KM ValorKM,

                                        E.EMP_CNPJ CNPJEmpresa,
                                        E.EMP_FANTASIA FantasiaEmpresa,
                                        E.EMP_ENDERECO EnderecoEmpresa,
                                        E.EMP_NUMERO NumeroEnderecoEmpresa,
                                        E.EMP_CEP CEPEmpresa,
                                        E.EMP_BAIRRO BairroEmpresa,
                                        E.EMP_FONE FoneEmpresa,
                                        LEMP.LOC_DESCRICAO CidadeEmpresa,
                                        LEMP.UF_SIGLA EstadoEmpresa,
                                        E.EMP_TIPO TipoEmpresa,
                                        E.EMP_HABILITAR_TABELA_VALOR_ORDEM_SERVICO_VENDA HabilitarTabelaValorOrdemServicoVenda,

                                        C.CLI_CGCCPF CNPJPessoa,
                                        C.CLI_NOME NomePessoa,
                                        C.CLI_ENDERECO EnderecoPessoa,
                                        C.CLI_NUMERO NumeroEnderecoPessoa,
                                        C.CLI_BAIRRO BairroPessoa,
                                        C.CLI_CEP CEPPessoa,
                                        LCLI.LOC_DESCRICAO CidadePessoa,
                                        LCLI.UF_SIGLA EstadoPessoa,
                                        C.CLI_FONE FonePessoa,
                                        C.CLI_FISJUR TipoPessoa,
                                        F.FUN_NOME NomeFuncionario,
                                        Veiculo.VEI_PLACA Placa,
                                        MarcaVeiculo.VMA_DESCRICAO MarcaVeiculo,
                                        ModeloVeiculo.VMO_DESCRICAO ModeloVeiculo,
                                        (SELECT TOP(1) PVA_GUID_ARQUIVO FROM T_PEDIDO_VENDA_ASSINATURA WHERE PEV_CODIGO = P.PEV_CODIGO ORDER BY PEV_CODIGO DESC ) GuidArquivoAssinatura");
            }

            queryBuilder.Append(@" FROM T_PEDIDO_VENDA P
                                    LEFT OUTER JOIN T_PEDIDO_VENDA_ITENS I ON I.PEV_CODIGO = P.PEV_CODIGO
                                    JOIN T_EMPRESA E ON E.EMP_CODIGO = P.EMP_CODIGO
                                    JOIN T_CLIENTE C ON C.CLI_CGCCPF = P.CLI_CGCCPF
                                    JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = P.FUN_CODIGO
                                    JOIN T_LOCALIDADES LEMP ON LEMP.LOC_CODIGO = E.LOC_CODIGO
                                    JOIN T_LOCALIDADES LCLI ON LCLI.LOC_CODIGO = C.LOC_CODIGO
                                    LEFT OUTER JOIN T_VEICULO Veiculo ON Veiculo.VEI_CODIGO = P.VEI_CODIGO
                                    LEFT OUTER JOIN T_VEICULO_MARCA MarcaVeiculo ON MarcaVeiculo.VMA_CODIGO = Veiculo.VMA_CODIGO
                                    LEFT OUTER JOIN T_VEICULO_MODELO ModeloVeiculo ON ModeloVeiculo.VMO_CODIGO = Veiculo.VMO_CODIGO
                                    LEFT OUTER JOIN T_FUNCIONARIO FI ON FI.FUN_CODIGO = I.FUN_CODIGO
                                    LEFT OUTER JOIN T_CLIENTE CI ON CI.CLI_CGCCPF = I.CLI_CGCCPF                                
                                    WHERE 1 = 1");

            if (codigoPedidoVenda > 0)
                queryBuilder.Append(" AND P.PEV_CODIGO = " + codigoPedidoVenda.ToString());

            return queryBuilder.ToString();
        }


        public IList<Dominio.Relatorios.Embarcador.DataSource.PedidosVendas.RelatorioPedidoVendaContrato> RelatorioPedidoVendaContrato(int codigoPedidoVenda)
        {
            string query = @"   SELECT P.PEV_NUMERO Numero,                                
                                P.PEV_OBSERVACAO Observacao,
                                P.PEV_VALOR_TOTAL ValorTotal,
                                P.PEV_VALOR_PRODUTOS ValorProdutos,
                                P.PEV_VALOR_SERVICOS ValorServicos,
                                P.PEV_FORMA_PAGAMENTO FormaPagamento,
                                P.PEV_REFERENCIA Referencia,

                                SUBSTRING((SELECT ', ' + CAST(I.PVI_DESCRICAO_ITEM AS NVARCHAR(160))
								FROM T_PEDIDO_VENDA_ITENS I
								WHERE I.PEV_CODIGO = P.PEV_CODIGO FOR XML PATH('')), 3, 1000) AS DescricaoItem,
								
								SUBSTRING((SELECT ', R$ ' + CAST(Replace(I.PVI_VALOR_TOTAL, '.', ',') AS NVARCHAR(160))
								FROM T_PEDIDO_VENDA_ITENS I
								WHERE I.PEV_CODIGO = P.PEV_CODIGO FOR XML PATH('')), 3, 1000) AS ValorTotalItem,

                                E.EMP_CNPJ CNPJEmpresa,
                                E.EMP_INSCRICAO IEEmpresa,
                                E.EMP_RAZAO RazaoEmpresa,
                                E.EMP_ENDERECO EnderecoEmpresa,
                                E.EMP_NUMERO NumeroEnderecoEmpresa,
                                E.EMP_CEP CEPEmpresa,
                                E.EMP_BAIRRO BairroEmpresa,
                                E.EMP_FONE FoneEmpresa,
                                LEMP.LOC_DESCRICAO CidadeEmpresa,
                                LEMP.UF_SIGLA EstadoEmpresa,
                                E.EMP_COMPLEMENTO ComplementoEmpresa,
                                ISNULL(E.EMP_URL_SISTEMA, '') SiteEmpresa,

                                C.CLI_CGCCPF CNPJPessoa,
                                C.CLI_NOME NomePessoa,
                                C.CLI_ENDERECO EnderecoPessoa,
                                C.CLI_NUMERO NumeroEnderecoPessoa,
                                C.CLI_BAIRRO BairroPessoa,
                                C.CLI_CEP CEPPessoa,
                                LCLI.LOC_DESCRICAO CidadePessoa,
                                LCLI.UF_SIGLA EstadoPessoa,
                                CASE
									WHEN C.CLI_FISJUR = 'F' THEN C.CLI_RG
									ELSE C.CLI_IERG
								END IEPessoa,
                                C.CLI_EMAIL EmailPessoa,
                                C.CLI_FONE FonePessoa,
                                C.CLI_FAX FoneSecundarioPessoa,

                                SUBSTRING((SELECT ', ' + CAST(convert(varchar(10),PVP.PVP_DATA_VENCIMENTO,103) AS NVARCHAR(160))
								FROM T_PEDIDO_VENDA_PARCELA PVP
								WHERE PVP.PEV_CODIGO = P.PEV_CODIGO FOR XML PATH('')), 3, 1000) AS DataVencimentoParcela,
                                
                                SUBSTRING((SELECT ', ' + CAST(PVP.PVP_SEQUENCIA AS NVARCHAR(160)) + 
                                ' - R$ ' + CAST(Replace(PVP.PVP_VALOR, '.', ',') AS NVARCHAR(160)) +
                                ' - Ven.: ' + CAST(convert(varchar(10),PVP.PVP_DATA_VENCIMENTO,103) AS NVARCHAR(160))
								FROM T_PEDIDO_VENDA_PARCELA PVP
								WHERE PVP.PEV_CODIGO = P.PEV_CODIGO order by PVP.PVP_SEQUENCIA FOR XML PATH('')), 3, 5000) AS SequenciaParcela

                                FROM T_PEDIDO_VENDA P
                                JOIN T_EMPRESA E ON E.EMP_CODIGO = P.EMP_CODIGO
                                JOIN T_CLIENTE C ON C.CLI_CGCCPF = P.CLI_CGCCPF
                                JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = P.FUN_CODIGO
                                JOIN T_LOCALIDADES LEMP ON LEMP.LOC_CODIGO = E.LOC_CODIGO
                                JOIN T_LOCALIDADES LCLI ON LCLI.LOC_CODIGO = C.LOC_CODIGO
                                WHERE 1 = 1";

            if (codigoPedidoVenda > 0)
                query += " AND P.PEV_CODIGO = " + codigoPedidoVenda.ToString();

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.PedidosVendas.RelatorioPedidoVendaContrato)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.PedidosVendas.RelatorioPedidoVendaContrato>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.PedidosVendas.RelatorioPedidoVendaParcela> RelatorioPedidoVendaParcela(int codigoPedidoVenda)
        {
            string query = @"   SELECT 
                                PVP_SEQUENCIA Sequencia,
                                PVP_VALOR Valor,
                                PVP_DATA_VENCIMENTO DataVencimento
                                FROM T_PEDIDO_VENDA_PARCELA 
                                WHERE PEV_CODIGO = " + codigoPedidoVenda.ToString() +
                                " ORDER BY PVP_SEQUENCIA ASC";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.PedidosVendas.RelatorioPedidoVendaParcela)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.PedidosVendas.RelatorioPedidoVendaParcela>();
        }

        #endregion
    }
}
