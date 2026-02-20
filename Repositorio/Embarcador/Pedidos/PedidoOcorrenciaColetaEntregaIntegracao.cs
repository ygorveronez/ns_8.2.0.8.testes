using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pedidos
{
    public class PedidoOcorrenciaColetaEntregaIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao>
    {
        #region Construtores

        public PedidoOcorrenciaColetaEntregaIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao>()
                .Where(o => o.Codigo == codigo);
            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao> BuscarIntegracoesCargaPorCodigosColeta(List<int> codigosColetaEntrega)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao>();
            var result = from obj in query where codigosColetaEntrega.Contains(obj.PedidoOcorrenciaColetaEntrega.Codigo) select obj;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao BuscarPorCodigoArquivo(int codigoArquivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao>()
                .Where(o => o.ArquivosTransacao.Any(a => a.Codigo == codigoArquivo));
            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao BuscarProximaIntegracaoAgIntegracao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao>()
                .Where(o => o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao)
                .OrderBy(o => o.DataIntegracao);
            return query.FirstOrDefault();
        }

        public List<int> BuscarIntegracoesPorSituacao(int intervaloTempoRejeitadas, int limite, bool efetuarIntegracaoApenasCanhotosDigitalizados)
        {

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao>()
                .Where(o => o.TipoIntegracao.Tipo != TipoIntegracao.NaoPossuiIntegracao && o.TipoIntegracao.Tipo != TipoIntegracao.NaoInformada &&
                            (o.SituacaoIntegracao == SituacaoIntegracao.AgIntegracao || (o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao
                            && o.TipoIntegracao.Tentativas > 0 && o.NumeroTentativas < o.TipoIntegracao.Tentativas && o.DataIntegracao <= DateTime.Now.AddHours(-intervaloTempoRejeitadas))));

            if (efetuarIntegracaoApenasCanhotosDigitalizados)
            {
                var queryCanhotos = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>()
                 .Where(x => x.SituacaoDigitalizacaoCanhoto != SituacaoDigitalizacaoCanhoto.Digitalizado)
                 .Select(x => x.Carga.Codigo);

                query = query.Where(x => !queryCanhotos.Contains(x.PedidoOcorrenciaColetaEntrega.Carga.Codigo));
            }

            query = query.OrderBy(o => o.DataIntegracao);
            return query.Take(limite).Select(o => o.Codigo).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao> BuscarIntegracoesPorSituacaoEData(SituacaoIntegracao situacao, DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao>()
                .Where(obj => obj.SituacaoIntegracao == situacao &&
                       obj.DataIntegracao.Date >= dataInicial.Date &&
                       obj.DataIntegracao.Date <= dataFinal.Date);

            return query
                .ToList();
        }

        public int BuscarQuantidadeIntegracoesPorSituacao(SituacaoIntegracao situacao, DateTime? dataInicial, DateTime? dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao>()
                .Where(o => o.TipoIntegracao.Tipo != TipoIntegracao.NaoPossuiIntegracao && o.TipoIntegracao.Tipo != TipoIntegracao.NaoInformada &&
                            o.SituacaoIntegracao == situacao
                           );

            if (dataInicial.HasValue && dataInicial.Value > DateTime.MinValue)
                query = query.Where(o => o.DataIntegracao >= dataInicial.Value);

            if (dataFinal.HasValue && dataFinal.Value > DateTime.MinValue)
                query = query.Where(o => o.DataIntegracao <= dataFinal.Value);

            return query.Count();
        }

        public List<int> BuscarPorSituacao(SituacaoIntegracao situacao, DateTime? dataInicial, DateTime? dataFinal, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao>()
                .Where(o => o.TipoIntegracao.Tipo != TipoIntegracao.NaoPossuiIntegracao && o.TipoIntegracao.Tipo != TipoIntegracao.NaoInformada &&
                            o.SituacaoIntegracao == situacao);

            if (dataInicial.HasValue && dataInicial.Value > DateTime.MinValue)
                query = query.Where(o => o.DataIntegracao >= dataInicial.Value);

            if (dataFinal.HasValue && dataFinal.Value > DateTime.MinValue)
                query = query.Where(o => o.DataIntegracao <= dataFinal.Value);

            return query.OrderBy(o => o.DataIntegracao).Take(limite).Select(o => o.Codigo).ToList();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaPedidoOcorrenciaColetaEntregaIntegracao filtrosPesquisa, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = this.SessionNHiBernate.CreateSQLQuery(QueryConsulta(filtrosPesquisa, somenteContarNumeroRegistros, parametrosConsulta));

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao)));

            return consulta.SetTimeout(120).List<Dominio.Relatorios.Embarcador.DataSource.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao>();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaPedidoOcorrenciaColetaEntregaIntegracao filtrosPesquisa)
        {
            var consulta = this.SessionNHiBernate.CreateSQLQuery(QueryConsulta(filtrosPesquisa, true));

            return consulta.SetTimeout(600).UniqueResult<int>();
        }

        #endregion

        #region Métodos Privados

        private string QueryConsulta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaPedidoOcorrenciaColetaEntregaIntegracao filtrosPesquisa, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = null)
        {
            string sql;

            if (somenteContarNumeroRegistros)
                sql = "select distinct(count(0) over ()) ";
            else
                sql = @"select
                            pedidoOcorrenciaColetaEntregaIntegracao.INT_CODIGO Codigo, 
	                        pedidoOcorrenciaColetaEntregaIntegracao.LAY_CODIGO LayoutEDI, 
	                        pedidoOcorrenciaColetaEntregaIntegracao.INT_SITUACAO_INTEGRACAO CodigoSituacaoIntegracao, 
	                        pedidoOcorrenciaColetaEntrega.OCO_CODIGO CodigoTipoDeOcorrencia, 
	                        pedidoOcorrenciaColetaEntregaIntegracao.TPI_CODIGO CodigoTipoIntegracao, 
	                        carga.CAR_CODIGO_CARGA_EMBARCADOR CodigoCargaEmbarcador, 
	                        pedido.PED_NUMERO_PEDIDO_EMBARCADOR NumeroPedidoEmbarcador, 
	                        tipoDeOcorrencia.OCO_DESCRICAO DescricaoTipoDeOcorrencia, 

	                        pedidoOcorrenciaColetaEntrega.CAR_CODIGO CargaCodigo,
	                        cargaEmpresa.EMP_CODIGO CargaEmpresaCodigo,

	                        pedidoEmpresa.EMP_CODIGO PedidoEmpresaCodigo,

	                        cargaEmpresa.EMP_CODIGO_EMPRESA CargaCodigoEmpresa, 
	                        cargaEmpresa.EMP_RAZAO CargaRazaoSocial, 
	                        cargaEmpresa.LOC_CODIGO CargaLocalidade,

	                        pedidoEmpresa.EMP_CODIGO_EMPRESA PedidoCodigoEmpresa, 
	                        pedidoEmpresa.EMP_RAZAO PedidoRazaoSocial,
	                        pedidoEmpresa.LOC_CODIGO PedidoLocalidade,

	                        cargaLocalidades.LOC_IBGE CargaCodigoIBGE,
	                        cargaLocalidades.PAI_CODIGO CargaCodigoPais,
	                        cargaLocalidades.LOC_DESCRICAO CargaLocalidadeDescricao, 
	                        cargaEstado.UF_SIGLA CargaEstadoSigla, 
	                        cargaPais.PAI_ABREVIACAO CargaPaisAbreviacao,
	                        cargaPais.PAI_NOME CargaPaisNome,


	                        pedidoLocalidades.LOC_IBGE PedidoCodigoIBGE,
	                        pedidoLocalidades.PAI_CODIGO PedidoCodigoPais,
	                        pedidoLocalidades.LOC_DESCRICAO PedidoLocalidadeDescricao,
	                        pedidoEstado.UF_SIGLA PedidoEstadoSigla,
	                        pedidoPais.PAI_ABREVIACAO PedidoPaisAbreviacao,
	                        pedidoPais.PAI_NOME PedidoPaisNome,


	                        pedido.PED_TIPO_TOMADOR TipoTomador,

	                        clienteRemetente.CLI_NOME ClienteRemetenteNome,
	                        clienteRemetente.CLI_CGCCPF ClienteRemetenteCPF_CNPJ,
	                        clienteRemetente.CLI_PONTO_TRANSBORDO ClienteRemetentePontoTransbordo,
	                        clienteRemetente.CLI_NOMEFANTASIA ClienteRemetenteNomeFantasia,
	                        clienteRemetente.CLI_CODIGO_INTEGRACAO ClienteRemetenteCodigoIntegracao,
	                        clienteRemetente.CLI_FISJUR ClienteRemetenteTipo,


	                        clienteDestinatario.CLI_NOME ClienteDestinatarioNome,
	                        clienteDestinatario.CLI_CGCCPF ClienteDestinatarioCPF_CNPJ,
	                        clienteDestinatario.CLI_PONTO_TRANSBORDO ClienteDestinatarioPontoTransbordo,
	                        clienteDestinatario.CLI_NOMEFANTASIA ClienteDestinatarioNomeFantasia,
	                        clienteDestinatario.CLI_CODIGO_INTEGRACAO ClienteDestinatarioCodigoIntegracao,
	                        clienteDestinatario.CLI_FISJUR ClienteDestinatarioTipo,

	                        clienteTomador.CLI_NOME ClienteTomadorNome,
	                        clienteTomador.CLI_CGCCPF ClienteTomadorCPF_CNPJ,
	                        clienteTomador.CLI_PONTO_TRANSBORDO ClienteTomadorPontoTransbordo,
	                        clienteTomador.CLI_NOMEFANTASIA ClienteTomadorNomeFantasia,
	                        clienteTomador.CLI_CODIGO_INTEGRACAO ClienteTomadorCodigoIntegracao,
	                        clienteTomador.CLI_FISJUR ClienteTomadorTipo,

	                        clienteRecebedor.CLI_NOME ClienteRecebedorNome,
	                        clienteRecebedor.CLI_CGCCPF ClienteRecebedorCPF_CNPJ,
	                        clienteRecebedor.CLI_PONTO_TRANSBORDO ClienteRecebedorPontoTransbordo,
	                        clienteRecebedor.CLI_NOMEFANTASIA ClienteRecebedorNomeFantasia,
	                        clienteRecebedor.CLI_CODIGO_INTEGRACAO ClienteRecebedorCodigoIntegracao,
	                        clienteRecebedor.CLI_FISJUR ClienteRecebedorTipo,

	                        clienteExpedidor.CLI_NOME ClienteExpedidorNome,
	                        clienteExpedidor.CLI_CGCCPF ClienteExpedidorCPF_CNPJ,
	                        clienteExpedidor.CLI_PONTO_TRANSBORDO ClienteExpedidorPontoTransbordo,
	                        clienteExpedidor.CLI_NOMEFANTASIA ClienteExpedidorNomeFantasia,
	                        clienteExpedidor.CLI_CODIGO_INTEGRACAO ClienteExpedidorCodigoIntegracao,
	                        clienteExpedidor.CLI_FISJUR ClienteExpedidorTipo,

	                        tipoIntegracao.TPI_DESCRICAO DescricaoTipoIntegracao,

	                        pedidoOcorrenciaColetaEntregaIntegracao.INT_DATA_INTEGRACAO DataIntegracao, 
	                        pedidoOcorrenciaColetaEntregaIntegracao.INT_NUMERO_TENTATIVAS NumeroTentativas, 
	                        pedidoOcorrenciaColetaEntregaIntegracao.INT_SITUACAO_INTEGRACAO SituacaoIntegracao, 
	                        pedidoOcorrenciaColetaEntregaIntegracao.INT_PROBLEMA_INTEGRACAO ProblemaIntegracao ";

            sql += @"from T_PEDIDO_OCORRENCIA_COLETA_ENTREGA_INTEGRACAO pedidoOcorrenciaColetaEntregaIntegracao
                        left join T_PEDIDO_OCORRENCIA_COLETA_ENTREGA pedidoOcorrenciaColetaEntrega on pedidoOcorrenciaColetaEntrega.POC_CODIGO = pedidoOcorrenciaColetaEntregaIntegracao.POC_CODIGO
                        left join T_OCORRENCIA tipoDeOcorrencia on pedidoOcorrenciaColetaEntrega.OCO_CODIGO = tipoDeOcorrencia.OCO_CODIGO
                        left join T_PEDIDO pedido on pedidoOcorrenciaColetaEntrega.PED_CODIGO = pedido.PED_CODIGO 
                        left join T_CARGA carga	on pedidoOcorrenciaColetaEntrega.CAR_CODIGO = carga.CAR_CODIGO
                        left join T_EMPRESA cargaEmpresa on carga.EMP_CODIGO = cargaEmpresa.EMP_CODIGO
                        left join T_EMPRESA pedidoEmpresa on pedido.EMP_CODIGO = pedidoEmpresa.EMP_CODIGO
                        left join T_LOCALIDADES cargaLocalidades on cargaEmpresa.LOC_CODIGO = cargaLocalidades.LOC_CODIGO
                        left join T_LOCALIDADES pedidoLocalidades on pedidoEmpresa.LOC_CODIGO = pedidoLocalidades.LOC_CODIGO
                        left join T_PAIS cargaPais on cargaLocalidades.PAI_CODIGO = cargaPais.PAI_CODIGO
                        left join T_PAIS pedidoPais on pedidoLocalidades.PAI_CODIGO = pedidoPais.PAI_CODIGO
                        left join T_UF cargaEstado on cargaLocalidades.UF_SIGLA = cargaEstado.UF_SIGLA
                        left join T_UF pedidoEstado on pedidoLocalidades.UF_SIGLA = pedidoEstado.UF_SIGLA
                        left join T_CLIENTE clienteRemetente on pedido.CLI_CODIGO_REMETENTE = clienteRemetente.CLI_CGCCPF
                        left join T_CLIENTE clienteDestinatario on pedido.CLI_CODIGO = clienteDestinatario.CLI_CGCCPF
                        left join T_CLIENTE clienteTomador on pedido.CLI_CODIGO_TOMADOR = clienteTomador.CLI_CGCCPF
                        left join T_CLIENTE clienteRecebedor on pedido.CLI_CODIGO_RECEBEDOR = clienteRecebedor.CLI_CGCCPF
                        left join T_CLIENTE clienteExpedidor on pedido.CLI_CODIGO_EXPEDIDOR = clienteExpedidor.CLI_CGCCPF
                        left join T_TIPO_INTEGRACAO tipoIntegracao on pedidoOcorrenciaColetaEntregaIntegracao.TPI_CODIGO = tipoIntegracao.TPI_CODIGO";

            sql += @" where 1=1 ";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoCargaEmbarcador))
                sql += $"and carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.CodigoCargaEmbarcador}'";
            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedidoEmbarcador))
                sql += $"and pedido.PED_NUMERO_PEDIDO_EMBARCADOR = '{filtrosPesquisa.NumeroPedidoEmbarcador}'";
            if (filtrosPesquisa.CodigoTipoDeOcorrencia > 0)
                sql += $"and pedidoOcorrenciaColetaEntrega.OCO_CODIGO = {filtrosPesquisa.CodigoTipoDeOcorrencia}";
            if (filtrosPesquisa.CodigoTransportador > 0)
                sql += $"and pedidoEmpresa.EMP_CODIGO = {filtrosPesquisa.CodigoTransportador}";
            if (filtrosPesquisa.CodigoTomador > 0)
            {
                sql += $"and ((pedido.PED_TIPO_TOMADOR = {Dominio.Enumeradores.TipoTomador.Outros.GetHashCode()} and clienteTomador.CLI_CGCCPF = {filtrosPesquisa.CodigoTomador})" +
                       $"or  (pedido.PED_TIPO_TOMADOR = {Dominio.Enumeradores.TipoTomador.Remetente.GetHashCode()} and clienteRemetente.CLI_CGCCPF = {filtrosPesquisa.CodigoTomador})" +
                       $"or  (pedido.PED_TIPO_TOMADOR = {Dominio.Enumeradores.TipoTomador.Destinatario.GetHashCode()} and clienteDestinatario.CLI_CGCCPF = {filtrosPesquisa.CodigoTomador}))";
            }
            if (filtrosPesquisa.CodigoTipoIntegracao > 0)
                sql += $"and pedidoOcorrenciaColetaEntregaIntegracao.TPI_CODIGO = {filtrosPesquisa.CodigoTipoIntegracao}";
            if (filtrosPesquisa.DataInicial.HasValue)
                sql += $"and pedidoOcorrenciaColetaEntregaIntegracao.INT_DATA_INTEGRACAO >= '{filtrosPesquisa.DataInicial.Value.Date.ToString("yyyy/MM/dd HH:mm:ss")}'";
            if (filtrosPesquisa.DataFinal.HasValue)
                sql += $"and pedidoOcorrenciaColetaEntregaIntegracao.INT_DATA_INTEGRACAO <= '{filtrosPesquisa.DataFinal.Value.Date.Add(DateTime.MaxValue.TimeOfDay).ToString("yyyy/MM/dd HH:mm:ss")}'";
            if (filtrosPesquisa.SituacaoIntegracao >= 0)
                sql += $"and pedidoOcorrenciaColetaEntregaIntegracao.INT_SITUACAO_INTEGRACAO = {filtrosPesquisa.SituacaoIntegracao.GetHashCode()}";


            if (parametrosConsulta != null && !somenteContarNumeroRegistros)
            {
                sql += $" order by {parametrosConsulta.PropriedadeOrdenar} {parametrosConsulta.DirecaoOrdenar}";

                if ((parametrosConsulta.InicioRegistros > 0) || (parametrosConsulta.LimiteRegistros > 0))
                    sql += $" offset {parametrosConsulta.InicioRegistros} rows fetch next {parametrosConsulta.LimiteRegistros} rows only;";
            }

            return sql;
        }

        #endregion
    }
}
