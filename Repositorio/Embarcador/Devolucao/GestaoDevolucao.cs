using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Devolucao
{
    public class GestaoDevolucao : RepositorioBase<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao>
    {
        public GestaoDevolucao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao BuscarPorCodigo(long codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao>();
            query = query.Where(o => o.Codigo == codigo);
            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao> BuscarPorCodigos(List<long> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao>();
            query = query.Where(o => codigos.Contains(o.Codigo));
            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao BuscarPorGestaoDevolucaoLaudo(long codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao>();
            query = query.Where(o => o.Laudo.Codigo == codigo);
            return query.FirstOrDefault();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.GestaoDevolucao.GestaoDevolucaoNotaFiscal> BuscarPorChavesNotaOrigem(List<string> codigosNotaFiscal)
        {
            if (codigosNotaFiscal == null || codigosNotaFiscal.Count == 0)
                return new List<Dominio.ObjetosDeValor.Embarcador.GestaoDevolucao.GestaoDevolucaoNotaFiscal>();

            var sql = @"
        SELECT
            GestaoDevolucaoNotaFiscal.GDV_CODIGO CodigoGestaoDevolucao,
            GestaoDevolucaoNotaFiscal.NFX_CODIGO CodigoNotaFiscal,
            XmlNotaFiscal.NF_CHAVE ChaveNotaFiscal
        FROM T_GESTAO_DEVOLUCAO_XML_NOTA_FISCAL GestaoDevolucaoNotaFiscal
            JOIN T_GESTAO_DEVOLUCAO GestaoDevolucao
                on GestaoDevolucao.GDV_CODIGO = GestaoDevolucaoNotaFiscal.GDV_CODIGO
                and GestaoDevolucao.GDV_SITUACAO_DEVOLUCAO <> 2
            JOIN T_XML_NOTA_FISCAL XmlNotaFiscal
                on XmlNotaFiscal.NFX_CODIGO = GestaoDevolucaoNotaFiscal.NFX_CODIGO
        WHERE XmlNotaFiscal.NF_CHAVE in (:chavesNotaFiscal)";

            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);
            consulta.SetParameterList("chavesNotaFiscal", codigosNotaFiscal);
            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.GestaoDevolucao.GestaoDevolucaoNotaFiscal)));
            return consulta.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.GestaoDevolucao.GestaoDevolucaoNotaFiscal>();
        }

        public async Task<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao> BuscarPorChaveNotaDevolucaoAsync(string chaveNotaFiscal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao>();
            query = query.Where(gestaoDevolucao => gestaoDevolucao.NotasFiscaisDevolucao.Any(nota => nota.XMLNotaFiscal.Chave == chaveNotaFiscal));

            return await query.FirstOrDefaultAsync();

        }

        public async Task<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao> BuscarPorChaveNotaOrigemAsync(string chaveNotaFiscal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao>();
            query = query.Where(gestaoDevolucao => gestaoDevolucao.NotasFiscaisDeOrigem.Any(nota => nota.XMLNotaFiscal.Chave == chaveNotaFiscal));

            return await query.FirstOrDefaultAsync();

        }

        public async Task<bool> TodasNotasPalletFinalizadasAsync(long codigoGestaoDevolucao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao>();
            query = query.Where(gestaoDevolucao => gestaoDevolucao.Codigo == codigoGestaoDevolucao &&
                                gestaoDevolucao.NotasFiscaisDeOrigem.All(nota =>
                                    nota.XMLNotaFiscal.TipoNotaFiscalIntegrada == TipoNotaFiscalIntegrada.RemessaPallet &&
                                    nota.ControleFinalizacaoDevolucao == true) &&
                                gestaoDevolucao.NotasFiscaisDevolucao.All(nota =>
                                nota.XMLNotaFiscal.TipoNotaFiscalIntegrada == TipoNotaFiscalIntegrada.RemessaPallet &&
                                nota.ControleFinalizacaoDevolucao == true));

            return await query.AnyAsync();
        }

        public async Task<bool> TodasNotasFinalizadasAsync(long codigoGestaoDevolucao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao>();
            query = query.Where(gestaoDevolucao => gestaoDevolucao.Codigo == codigoGestaoDevolucao &&
                                gestaoDevolucao.NotasFiscaisDeOrigem.All(nota => nota.ControleFinalizacaoDevolucao == true) &&
                                gestaoDevolucao.NotasFiscaisDevolucao.All(nota =>
                                nota.ControleFinalizacaoDevolucao == true));

            return await query.AnyAsync();
        }

        public Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao BuscarPorCodigoCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao>();
            query = query.Where(o => o.CargaDevolucao.Codigo == codigoCarga);
            return query.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao> BuscarPorCodigoCargaAsync(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao>();
            query = query.Where(o => o.CargaDevolucao.Codigo == codigoCarga);
            return query.FirstOrDefaultAsync();
        }

        public bool PossuiCargaDevolucaoPallet(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao>();
            query = query.Where(o => o.CargaDevolucao.Codigo == codigoCarga && o.TipoNotas == TipoNotasGestaoDevolucao.Pallet);
            return query.Any();
        }

        public bool ExisteDevolucaoPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao>();
            query = query.Where(o => o.CargaDevolucao.Codigo == codigoCarga);
            return query.Any();
        }

        public List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.GestaoDevolucao.FiltroPesquisaGestaoDevolucao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = Consultar(filtrosPesquisa);

            return ObterLista(consulta, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Carga.GestaoDevolucao.FiltroPesquisaGestaoDevolucao filtrosPesquisa)
        {
            var consulta = Consultar(filtrosPesquisa);

            return consulta.Count();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.GestaoDevolucao.GestaoDevolucaoPallet> ConsultarNotaPalletGestaoDevolucao(Dominio.ObjetosDeValor.Embarcador.Carga.GestaoDevolucao.FiltroPesquisaGestaoDevolucao filtroPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            NHibernate.ISQLQuery result = QueryGestaoDevolucaoPallet(filtroPesquisa, parametrosConsulta);
            return result.SetTimeout(900).List<Dominio.ObjetosDeValor.Embarcador.GestaoDevolucao.GestaoDevolucaoPallet>();
        }

        public Dominio.ObjetosDeValor.Embarcador.GestaoDevolucao.GestaoDevolucaoPallet ConsultaDetalhesNota(Dominio.ObjetosDeValor.Embarcador.Carga.GestaoDevolucao.FiltroPesquisaGestaoDevolucao filtroPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            NHibernate.ISQLQuery result = QueryGestaoDevolucaoPallet(filtroPesquisa, parametrosConsulta);
            return result.SetTimeout(900).UniqueResult<Dominio.ObjetosDeValor.Embarcador.GestaoDevolucao.GestaoDevolucaoPallet>();
        }

        private NHibernate.ISQLQuery QueryGestaoDevolucaoPallet(Dominio.ObjetosDeValor.Embarcador.Carga.GestaoDevolucao.FiltroPesquisaGestaoDevolucao filtroPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.ObjetosDeValor.Embarcador.GestaoDevolucao.GestaoDevolucaoPallet> query = new List<Dominio.ObjetosDeValor.Embarcador.GestaoDevolucao.GestaoDevolucaoPallet>();

            var result = QueryConsultaNotaPalletGestaoDevolucao(filtroPesquisa, parametrosConsulta);
            result.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.GestaoDevolucao.GestaoDevolucaoPallet)));
            return result;
        }

        public int ContarConsultaNotaPalletGestaoDevolucao(Dominio.ObjetosDeValor.Embarcador.Carga.GestaoDevolucao.FiltroPesquisaGestaoDevolucao filtroPesquisa)
        {
            var query = QueryConsultaNotaPalletGestaoDevolucao(filtroPesquisa, null);
            int total = query.UniqueResult<int>();

            return total;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.GestaoDevolucao.FiltroPesquisaGestaoDevolucao filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao> consultaGestaoDevolucao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao>();
            IQueryable<Dominio.Entidades.Embarcador.Pessoas.ClienteComplementar> consultaClienteComplementar = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteComplementar>();

            if (!string.IsNullOrEmpty(filtrosPesquisa.Carga))
                consultaGestaoDevolucao = consultaGestaoDevolucao.Where(devolucao => devolucao.CargaOrigem.CodigoCargaEmbarcador == filtrosPesquisa.Carga);

            if (filtrosPesquisa.NFOrigem != 0)
                consultaGestaoDevolucao = consultaGestaoDevolucao.Where(devolucao => devolucao.NotasFiscaisDeOrigem.Any(nota => nota.XMLNotaFiscal.Numero == filtrosPesquisa.NFOrigem));

            if (filtrosPesquisa.NFDevolucao != 0)
                consultaGestaoDevolucao = consultaGestaoDevolucao.Where(devolucao => devolucao.NotasFiscaisDevolucao.Any(nota => nota.XMLNotaFiscal.Numero == filtrosPesquisa.NFDevolucao));

            if (filtrosPesquisa.Transportadores.Count > 0)
                consultaGestaoDevolucao = consultaGestaoDevolucao.Where(devolucao => filtrosPesquisa.Transportadores.Contains(devolucao.CargaOrigem.Empresa.Codigo));

            if (filtrosPesquisa.Filiais.Count > 0)
                consultaGestaoDevolucao = consultaGestaoDevolucao.Where(devolucao => filtrosPesquisa.Filiais.Contains(devolucao.CargaOrigem.Filial.Codigo));

            if (filtrosPesquisa.OrigemRecebimento != null)
                consultaGestaoDevolucao = consultaGestaoDevolucao.Where(devolucao => devolucao.OrigemRecebimento == filtrosPesquisa.OrigemRecebimento);

            if (filtrosPesquisa.Cliente > 0)
                consultaGestaoDevolucao = consultaGestaoDevolucao.Where(devolucao => devolucao.NotasFiscaisDevolucao.Any(nota => nota.XMLNotaFiscal.Destinatario.CPF_CNPJ == filtrosPesquisa.Cliente));

            if (filtrosPesquisa.TipoNotasGestaoDevolucao.Count > 0)
                consultaGestaoDevolucao = consultaGestaoDevolucao.Where(devolucao => filtrosPesquisa.TipoNotasGestaoDevolucao.Contains(devolucao.TipoNotas));

            if (!string.IsNullOrEmpty(filtrosPesquisa.CargaDevolucao))
                consultaGestaoDevolucao = consultaGestaoDevolucao.Where(devolucao => devolucao.CargaDevolucao.CodigoCargaEmbarcador == filtrosPesquisa.CargaDevolucao);

            if (filtrosPesquisa.Etapas.Count > 0)
                consultaGestaoDevolucao = consultaGestaoDevolucao.Where(devolucao => filtrosPesquisa.Etapas.Contains(devolucao.EtapaAtual.Etapa));

            if (filtrosPesquisa.TipoFluxoGestaoDevolucao.HasValue)
                consultaGestaoDevolucao = consultaGestaoDevolucao.Where(devolucao => devolucao.TipoFluxoGestaoDevolucao == filtrosPesquisa.TipoFluxoGestaoDevolucao);

            if (filtrosPesquisa.SituacaoDevolucao.Count > 0)
                consultaGestaoDevolucao = consultaGestaoDevolucao.Where(devolucao => filtrosPesquisa.SituacaoDevolucao.Contains(devolucao.SituacaoDevolucao));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.EscritorioVendas))
            {
                consultaClienteComplementar = consultaClienteComplementar.Where(clienteComplementar => clienteComplementar.EscritorioVendas == filtrosPesquisa.EscritorioVendas);

                consultaGestaoDevolucao = consultaGestaoDevolucao
                    .Where(devolucao => devolucao.NotasFiscaisDevolucao
                        .Any(nota => consultaClienteComplementar
                            .Select(c => c.Cliente.CPF_CNPJ)
                            .Contains(nota.XMLNotaFiscal.Destinatario.CPF_CNPJ)));
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.EquipeVendas))
            {
                consultaClienteComplementar = consultaClienteComplementar.Where(clienteComplementar => clienteComplementar.EquipeVendas == filtrosPesquisa.EscritorioVendas);

                consultaGestaoDevolucao = consultaGestaoDevolucao
                    .Where(devolucao => devolucao.NotasFiscaisDevolucao
                        .Any(nota => consultaClienteComplementar
                            .Select(c => c.Cliente.CPF_CNPJ)
                            .Contains(nota.XMLNotaFiscal.Destinatario.CPF_CNPJ)));
            }

            if (filtrosPesquisa.TipoGestaoDevolucao.Count > 0)
                consultaGestaoDevolucao = consultaGestaoDevolucao.Where(devolucao => filtrosPesquisa.TipoGestaoDevolucao.Contains(devolucao.Tipo));

            return consultaGestaoDevolucao;
        }

        public List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao> ConsultaDevolucaoNotificarTransportador(int horalimite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao>();
            query = query.Where(o => o.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGestaoDevolucao.NaoDefinido && o.DataCriacao.AddHours(horalimite) > DateTime.Now && o.Transportador != null);

            return query.Fetch(o => o.Transportador).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao> ConsultaDevolucaoComTempoEsgotadoSemTipo(int horalimite, int diasLimiteDevolucaoPallets)
        {
            DateTime dataAtual = DateTime.Now;
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao>();
            query = query.Where(o => o.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGestaoDevolucao.NaoDefinido &&
                                     o.Transportador != null &&
                                    ((!(o.TipoNotas == TipoNotasGestaoDevolucao.Pallet) && o.DataCriacao.AddHours(horalimite) < dataAtual) ||
                                     ((o.TipoNotas == TipoNotasGestaoDevolucao.Pallet) && o.DataCriacao.AddDays(diasLimiteDevolucaoPallets) < dataAtual)));

            return query.Fetch(o => o.Transportador).ToList();
        }

        private NHibernate.ISQLQuery QueryConsultaNotaPalletGestaoDevolucao(Dominio.ObjetosDeValor.Embarcador.Carga.GestaoDevolucao.FiltroPesquisaGestaoDevolucao filtroPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            string sqlQuerySelect = "SELECT ";
            string sqlWhere = " WHERE ";
            string sqlQuery = "";

            if (parametrosConsulta == null)
                sqlQuerySelect += "COUNT(*)";
            else
            {
                sqlQuerySelect += @"
                    Carga.CAR_CODIGO_CARGA_EMBARCADOR CodigoCargaEmbarcador,
	                Carga.CAR_CODIGO CodigoCarga,
	                NotaFiscal.NF_CHAVE AS Chave,
                    NotaFiscal.NFX_CODIGO AS CodigoNF,
                    NotaFiscal.NF_SERIE AS Serie,
                    NotaFiscal.NF_DATA_EMISSAO AS DataEmissao,
                    NotaFiscal.NF_TIPO_NOTA_FISCAL_INTEGRADA TipoNF,
	                ISNULL(GestaoDevolucaoNotaFiscal.GDV_CODIGO, 0) CodigoGestaoDevolucao,
	                Empresa.EMP_RAZAO Transportador,
	                Empresa.EMP_CNPJ CNPJTransportador,
	                Filial.FIL_DESCRICAO Filial,
	                Filial.FIL_CNPJ CNPJFilial, 
                    Cliente.CLI_CGCCPF CNPJCliente,
                    Cliente.CLI_NOME Cliente,
                    (SELECT CPL_LIMITE_DIAS_PARA_DEVOLUCAO_DE_PALLET FROM T_CONFIGURACAO_PALETES) LimiteDiasDevolucao,
                    NotaFiscal.NF_NUMERO NumeroNF,
                    MovimentacaoPallet.MPT_RESPONSAVEL_MOVIMENTACAO_PALLET ResponsavelMovimentacaoPallet,
                    Chamados.NumerosAtendimentos NumerosAtendimentos,
                    Chamados.MotivoDevolucao MotivoDevolucao,
                    Chamados.DevolucaoTotal DevolucaoTotal,
                    NotaFiscalDevolucao.NF_NUMERO NumeroNFDevolucao,
                  (CASE 
                    WHEN CargaPedido.PED_TIPO_TOMADOR = 0 THEN 'CIF'
                    WHEN CargaPedido.PED_TIPO_TOMADOR = 3 THEN 'FOB'
                    ELSE 'Outro'	
                    END) AS TipoTomadorDescricao
                ";
            }

            sqlQuerySelect += $@"
                 from T_CARGA Carga 
	                join T_CARGA_PEDIDO CargaPedido on CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO
	                join T_PEDIDO_XML_NOTA_FISCAL PedidoNotaFiscal on PedidoNotaFiscal.CPE_CODIGO = CargaPedido.CPE_CODIGO
	                join T_XML_NOTA_FISCAL NotaFiscal on NotaFiscal.NFX_CODIGO = PedidoNotaFiscal.NFX_CODIGO
	                left join T_MOVIMENTACAO_PALLET MovimentacaoPallet on MovimentacaoPallet.NFX_CODIGO = NotaFiscal.NFX_CODIGO and MovimentacaoPallet.CAR_CODIGO = Carga.CAR_CODIGO and MovimentacaoPallet.CPE_CODIGO = CargaPedido.CPE_CODIGO
	                left join T_GESTAO_DEVOLUCAO_XML_NOTA_FISCAL GestaoDevolucaoNotaFiscal on GestaoDevolucaoNotaFiscal.NFX_CODIGO = NotaFiscal.NFX_CODIGO                
	                left join T_GESTAO_DEVOLUCAO GestaoDevolucao on GestaoDevolucao.GDV_CODIGO = GestaoDevolucaoNotaFiscal.GDV_CODIGO and GestaoDevolucao.GDV_SITUACAO_DEVOLUCAO <> 2
	                left join T_EMPRESA Empresa on Empresa.EMP_CODIGO = Carga.EMP_CODIGO
	                left join T_FILIAL Filial on Filial.FIL_CODIGO = Carga.FIL_CODIGO
                    left join T_CLIENTE Cliente on Cliente.CLI_CGCCPF = NotaFiscal.CLI_CODIGO_DESTINATARIO
                    left join T_GESTAO_DEVOLUCAO_NFD_X_NFO NFDxNFO on NFDxNFO.NFX_CODIGO_NFO = NotaFiscal.NFX_CODIGO
                    left join T_XML_NOTA_FISCAL NotaFiscalDevolucao on NotaFiscalDevolucao.NFX_CODIGO = NFDxNFO.NFX_CODIGO_NFD
                    LEFT JOIN (
                        SELECT 
                            NFX_CODIGO,

                            STRING_AGG(CHA_NUMERO, ', ') AS NumerosAtendimentos,
                            STRING_AGG(MDE_DESCRICAO, ', ') AS MotivoDevolucao,
                            STRING_AGG(TipoDevolucao, ', ') AS DevolucaoTotal

                        FROM (
                            SELECT DISTINCT
                                ChamadoNotaFiscal.NFX_CODIGO,
                                Chamado.CHA_NUMERO,
                                MotivoDevolucao.MDE_DESCRICAO,
                                IIF(CargaEntregaNotaChamado.CNC_DEVOLUCAO_TOTAL = 1, 'Total', 'Parcial') AS TipoDevolucao
                            FROM T_CHAMADOS Chamado
                            JOIN T_MOTIVO_CHAMADA MotivoChamado 
                                ON MotivoChamado.MCH_CODIGO = Chamado.MCH_CODIGO
                                AND MotivoChamado.MCH_GERAR_GESTAO_DE_DEVOLUCAO = 1 
                                AND MotivoChamado.MCH_TIPO_MOTIVO_ATENDIMENTO = 1

                            JOIN T_CHAMADO_XML_NOTA_FISCAL ChamadoNotaFiscal 
                                ON ChamadoNotaFiscal.CHA_CODIGO = Chamado.CHA_CODIGO

                            JOIN T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal 
                                ON PedidoXMLNotaFiscal.NFX_CODIGO = ChamadoNotaFiscal.NFX_CODIGO

                            JOIN T_CARGA_ENTREGA_NOTA_FISCAL CargaEntregaNotaFiscal 
                                ON CargaEntregaNotaFiscal.PNF_CODIGO = PedidoXMLNotaFiscal.PNF_CODIGO

                            JOIN T_CARGA_ENTREGA_NOTA_FISCAL_CHAMADO CargaEntregaNotaChamado 
                                ON CargaEntregaNotaChamado.CEF_CODIGO = CargaEntregaNotaFiscal.CEF_CODIGO 
                                AND CargaEntregaNotaChamado.CHA_CODIGO = Chamado.CHA_CODIGO

                            JOIN T_MOTIVO_DEVOLUCAO_ENTREGA MotivoDevolucao 
                                ON MotivoDevolucao.MDE_CODIGO = CargaEntregaNotaChamado.MDE_CODIGO

                            WHERE Chamado.CHA_SITUACAO = 2
                        ) AS DadosUnicos
                        GROUP BY NFX_CODIGO
                    ) AS Chamados 
                    ON Chamados.NFX_CODIGO = NotaFiscal.NFX_CODIGO";

            if (filtroPesquisa.TipoNotasFiscais?.Count > 0)
            {
                sqlWhere += ConstruirClausulaWhereParaTipoNotaFiscalIntegrada(filtroPesquisa.TipoNotasFiscais);
            }
            else
            {
                sqlWhere += $" Chamados.NumerosAtendimentos IS NOT NULL";
            }

            if (!string.IsNullOrEmpty(filtroPesquisa.Carga))
                sqlWhere += $" AND Carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtroPesquisa.Carga}'";

            if (filtroPesquisa.Transportadores?.Count > 0)
                sqlWhere += $" AND Empresa.EMP_CODIGO IN ({string.Join(", ", filtroPesquisa.Transportadores)})";

            if (filtroPesquisa.Filiais?.Count > 0)
                sqlWhere += $" AND Filial.FIL_CODIGO IN ({string.Join(", ", filtroPesquisa.Filiais)})";

            if (filtroPesquisa.DevolucaoGerada != null)
            {
                if (filtroPesquisa.DevolucaoGerada == true)
                    sqlWhere += $" AND GestaoDevolucaoNotaFiscal.GDV_CODIGO IS NOT NULL";

                if (filtroPesquisa.DevolucaoGerada == false)
                    sqlWhere += $" AND GestaoDevolucaoNotaFiscal.GDV_CODIGO IS NULL";
            }

            if (filtroPesquisa.DataEmissaoNFInicial != DateTime.MinValue)
                sqlWhere += $" AND NotaFiscal.NF_DATA_EMISSAO >= '{filtroPesquisa.DataEmissaoNFInicial.ToString("yyyy-MM-dd HH:mm")}'";


            if (filtroPesquisa.DataEmissaoNFFinal != DateTime.MinValue)
                sqlWhere += $" AND NotaFiscal.NF_DATA_EMISSAO <= '{filtroPesquisa.DataEmissaoNFFinal.ToString("yyyy-MM-dd HH:mm")}'";

            if (filtroPesquisa.Cliente > 0)
                sqlWhere += $" AND NotaFiscal.CLI_CODIGO_DESTINATARIO = {filtroPesquisa.Cliente}";

            if (filtroPesquisa.TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                if (filtroPesquisa.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    sqlWhere += $" AND (NotaFiscal.NF_TIPO_NOTA_FISCAL_INTEGRADA <> 2 OR MovimentacaoPallet.MPT_RESPONSAVEL_MOVIMENTACAO_PALLET = 2)";
                else if (filtroPesquisa.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
                    sqlWhere += $" AND (NotaFiscal.NF_TIPO_NOTA_FISCAL_INTEGRADA <> 2 OR MovimentacaoPallet.MPT_RESPONSAVEL_MOVIMENTACAO_PALLET = 1)";
            }

            if (filtroPesquisa.NFDevolucao > 0)
                sqlWhere += $" AND NotaFiscalDevolucao.NF_NUMERO = {filtroPesquisa.NFDevolucao}";


            if (filtroPesquisa.NFOrigem > 0)
                sqlWhere += $" AND NotaFiscal.NF_NUMERO = {filtroPesquisa.NFOrigem}";

            if (filtroPesquisa.CodigoNF > 0)
                sqlWhere += $" AND NotaFiscal.NFX_CODIGO = {filtroPesquisa.CodigoNF}";

            sqlQuery += sqlQuerySelect + sqlWhere;

            if (parametrosConsulta != null)
            {
                if (!string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeOrdenar))
                    sqlQuery += $" ORDER BY  NotaFiscal.NFX_CODIGO desc";

                if (parametrosConsulta.InicioRegistros >= 0)
                    sqlQuery += $" OFFSET {parametrosConsulta.InicioRegistros} ROWS ";

                if (parametrosConsulta.LimiteRegistros > 0)
                    sqlQuery += $" FETCH FIRST {parametrosConsulta.LimiteRegistros} ROWS ONLY";
            }

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

            return query;
        }

        private string ConstruirClausulaWhereParaTipoNotaFiscalIntegrada(List<TipoNotaFiscalIntegrada> tipos)
        {
            StringBuilder where = new(string.Empty);

            for (int i = 0; i < tipos.Count; i++)
            {
                where.Append($" NotaFiscal.NF_TIPO_NOTA_FISCAL_INTEGRADA = {(int)tipos[i]} ");

                if (tipos.Count - i > 1)
                {
                    where.Append($" OR ");
                }
            }

            return tipos.Count > 1 ? @$"( {where} )" : @$" {where} ";
        }
    }
}

