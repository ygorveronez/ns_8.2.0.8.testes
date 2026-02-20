using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Documentos
{
    public class ArquivoMercante : RepositorioBase<Dominio.Entidades.Embarcador.Documentos.ArquivoMercante>
    {
        public ArquivoMercante(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Documentos.ArquivoMercante BuscarPorCodigo(long codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.ArquivoMercante>();
            query = query.Where(o => o.Codigo == codigo);
            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Documentos.ArquivoMercante BuscarPorNavioViagemDirecao(string navio, string numero, string direcao, string tipoArquivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.ArquivoMercante>();
            query = query.Where(o => o.CodigoIMO == navio && o.NumeroViagem == numero && o.DirecaoViagem == direcao && o.TipoArquivo == tipoArquivo);
            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Documentos.ArquivoMercante> Consulta(int codigoPedidoViagemDirecao, int codigoPortoOrigem, int codigoPortoDestino, string tipoArquivo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.ArquivoMercante>();

            if (!string.IsNullOrWhiteSpace(tipoArquivo))
                query = query.Where(obj => obj.TipoArquivo == tipoArquivo);

            if (situacao == SituacaoAtivoPesquisa.Ativo)
                query = query.Where(obj => obj.Integrado == true);
            if (situacao == SituacaoAtivoPesquisa.Inativo)
                query = query.Where(obj => obj.Integrado == false);

            if (codigoPedidoViagemDirecao > 0)
                query = query.Where(obj => obj.PedidoViagemNavio.Codigo == codigoPedidoViagemDirecao);

            if (codigoPortoOrigem > 0)
                query = query.Where(obj => obj.PortoOrigem.Codigo == codigoPortoOrigem);

            if (codigoPortoDestino > 0)
                query = query.Where(obj => obj.PortoDestino.Codigo == codigoPortoDestino);

            return query.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultar(int codigoPedidoViagemDirecao, int codigoPortoOrigem, int codigoPortoDestino, string tipoArquivo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.ArquivoMercante>();

            if (!string.IsNullOrWhiteSpace(tipoArquivo))
                query = query.Where(obj => obj.TipoArquivo == tipoArquivo);

            if (situacao == SituacaoAtivoPesquisa.Ativo)
                query = query.Where(obj => obj.Integrado == true);
            if (situacao == SituacaoAtivoPesquisa.Inativo)
                query = query.Where(obj => obj.Integrado == false);

            if (codigoPedidoViagemDirecao > 0)
                query = query.Where(obj => obj.PedidoViagemNavio.Codigo == codigoPedidoViagemDirecao);

            if (codigoPortoOrigem > 0)
                query = query.Where(obj => obj.PortoOrigem.Codigo == codigoPortoOrigem);

            if (codigoPortoDestino > 0)
                query = query.Where(obj => obj.PortoDestino.Codigo == codigoPortoDestino);

            return query.Count();
        }

        public List<long> ConsultarCodigos(int codigoPedidoViagemDirecao, int codigoPortoOrigem, int codigoPortoDestino, string tipoArquivo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.ArquivoMercante>();

            if (!string.IsNullOrWhiteSpace(tipoArquivo))
                query = query.Where(obj => obj.TipoArquivo == tipoArquivo);

            if (situacao == SituacaoAtivoPesquisa.Ativo)
                query = query.Where(obj => obj.Integrado == true);
            if (situacao == SituacaoAtivoPesquisa.Inativo)
                query = query.Where(obj => obj.Integrado == false);

            if (codigoPedidoViagemDirecao > 0)
                query = query.Where(obj => obj.PedidoViagemNavio.Codigo == codigoPedidoViagemDirecao);

            if (codigoPortoOrigem > 0)
                query = query.Where(obj => obj.PortoOrigem.Codigo == codigoPortoOrigem);

            if (codigoPortoDestino > 0)
                query = query.Where(obj => obj.PortoDestino.Codigo == codigoPortoDestino);

            return query.Select(c => c.Codigo).ToList();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Documentos.ConsultaMercante> BuscarCNPJDespachanteEnvioEmail()
        {
            string sql = @"SELECT distinct ISNULL(Despachante.CLI_CGCCPF, 0) CNPJDespachante, 
            ISNULL(ViagemCinco.PVN_CODIGO,ISNULL(ViagemQuatro.PVN_CODIGO, ISNULL(ViagemTres.PVN_CODIGO, ISNULL(ViagemDois.PVN_CODIGO, ISNULL(ViagemUm.PVN_CODIGO, Viagem.PVN_CODIGO))))) CodigoViagem, 
            PortoDestino.POT_CODIGO CodigoPorto, CTe.CON_EMAIL_DESPACHANTE_SVM EmailDespachante
            FROM T_CTE CTe
            JOIN T_CARGA_CTE CargaCTe on CargaCTe.CON_CODIGO = CTe.CON_CODIGO
            JOIN T_CARGA_PEDIDO CargaPedido on CargaPedido.CAR_CODIGO = CargaCTe.CAR_CODIGO
            JOIN T_CTE_PARTICIPANTE Tomador on Tomador.PCT_CODIGO = CTe.CON_TOMADOR_PAGADOR_CTE
            JOIN T_CLIENTE Cliente on Cliente.CLI_CGCCPF = Tomador.CLI_CODIGO
            JOIN T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule on Schedule.PVS_CODIGO = CTe.PVS_CODIGO
            LEFT OUTER JOIN T_GRUPO_PESSOAS GrupoTomador on GrupoTomador.GRP_CODIGO = Cliente.GRP_CODIGO
            LEFT OUTER JOIN T_CLIENTE Despachante on Despachante.CLI_CGCCPF = GrupoTomador.GRP_DESPACHANTE
            JOIN T_PORTO PortoOrigem on PortoOrigem.POT_CODIGO = CTe.POT_CODIGO_ORIGEM
            LEFT OUTER JOIN T_PORTO PortoDestino on PortoDestino.POT_CODIGO = CTe.POT_CODIGO_DESTINO
            LEFT OUTER JOIN T_PORTO PortoPassagemUm on PortoPassagemUm.POT_CODIGO = CTe.POT_CODIGO_PASSAGEM_UM
            LEFT OUTER JOIN T_PORTO PortoPassagemDois on PortoPassagemDois.POT_CODIGO = CTe.POT_CODIGO_PASSAGEM_DOIS
            LEFT OUTER JOIN T_PORTO PortoPassagemTres on PortoPassagemTres.POT_CODIGO = CTe.POT_CODIGO_PASSAGEM_TRES
            LEFT OUTER JOIN T_PORTO PortoPassagemQuatro on PortoPassagemQuatro.POT_CODIGO = CTe.POT_CODIGO_PASSAGEM_QUATRO
            LEFT OUTER JOIN T_PORTO PortoPassagemCinco on PortoPassagemCinco.POT_CODIGO = CTe.POT_CODIGO_PASSAGEM_CINCO
            LEFT OUTER JOIN T_PEDIDO_VIAGEM_NAVIO Viagem on Viagem.PVN_CODIGO = CTe.CON_VIAGEM
            LEFT OUTER JOIN T_PEDIDO_VIAGEM_NAVIO ViagemUm on ViagemUm.PVN_CODIGO = CTe.CON_VIAGEM_PASSAGEM_UM
            LEFT OUTER JOIN T_PEDIDO_VIAGEM_NAVIO ViagemDois on ViagemDois.PVN_CODIGO = CTe.CON_VIAGEM_PASSAGEM_DOIS
            LEFT OUTER JOIN T_PEDIDO_VIAGEM_NAVIO ViagemTres on ViagemTres.PVN_CODIGO = CTe.CON_VIAGEM_PASSAGEM_TRES
            LEFT OUTER JOIN T_PEDIDO_VIAGEM_NAVIO ViagemQuatro on ViagemQuatro.PVN_CODIGO = CTe.CON_VIAGEM_PASSAGEM_QUATRO
            LEFT OUTER JOIN T_PEDIDO_VIAGEM_NAVIO ViagemCinco on ViagemCinco.PVN_CODIGO = CTe.CON_VIAGEM_PASSAGEM_CINCO
            WHERE CTe.CON_STATUS = 'A'
            AND (CTe.CON_NAO_ENVIAR_PARA_MERCANTE IS NULL OR CTe.CON_NAO_ENVIAR_PARA_MERCANTE = 0)
            AND CTe.CON_TIPO_CTE = 0 AND CTe.CON_TIPO_MODAL = 3            
            AND CargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL != 3 AND CargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL != 7 AND CargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL != 6 AND CargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL != 5 AND CargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL != 4
            AND ((GrupoTomador.GRP_ADICIONAR_DESPACHANTE_COMO_CONSIGNATARIO = 1 AND GrupoTomador.GRP_EMAIL_DESPACHANTE <> '' and GrupoTomador.GRP_EMAIL_DESPACHANTE is not null) or (CTe.CON_EMAIL_DESPACHANTE_SVM <> '' and CTe.CON_EMAIL_DESPACHANTE_SVM is not null))
            AND (PortoDestino.POT_DIAS_ANTES_DO_POD_PARA_ENVIO_DA_DOCUMENTACAO >= 0 OR PortoPassagemUm.POT_DIAS_ANTES_DO_POD_PARA_ENVIO_DA_DOCUMENTACAO >= 0 OR PortoPassagemDois.POT_DIAS_ANTES_DO_POD_PARA_ENVIO_DA_DOCUMENTACAO >= 0 OR PortoPassagemTres.POT_DIAS_ANTES_DO_POD_PARA_ENVIO_DA_DOCUMENTACAO >= 0 OR PortoPassagemQuatro.POT_DIAS_ANTES_DO_POD_PARA_ENVIO_DA_DOCUMENTACAO >= 0 OR PortoPassagemCinco.POT_DIAS_ANTES_DO_POD_PARA_ENVIO_DA_DOCUMENTACAO >= 0)
            AND DATEDIFF(DAY, '" + DateTime.Now.ToString("yyyy-MM-dd") + "', CAST(Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO AS date)) = PortoDestino.POT_DIAS_ANTES_DO_POD_PARA_ENVIO_DA_DOCUMENTACAO";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Documentos.ConsultaMercante)));

            return query.SetTimeout(6000).List<Dominio.ObjetosDeValor.Embarcador.Documentos.ConsultaMercante>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Documentos.Mercante> BuscarDadosMercanteDespachanteEnvioEmail(double cnpjDespachante, int codigoViagem, int codigoPorto, string emailDespachante)
        {
            string sql = @"SELECT distinct Tomador.PCT_NOME Tomador,
                Tomador.PCT_CPF_CNPJ CNPJTomador, 
                SUBSTRING((
                SELECT DISTINCT ', ' + container.CTR_DESCRICAO
                        from T_CONTAINER container 
                        inner join T_CTE_CONTAINER cteContainer on cteContainer.CTR_CODIGO = container.CTR_CODIGO 
                    WHERE cteContainer.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 1000) Container,
                CTe.CON_NUMERO_BOOKING Booking,
                CTe.CON_NUMERO_CONTROLE NumeroControle,
                CASE WHEN CTe.CON_TIPO_MODAL = 3 THEN CTE.CON_CHAVECTE
                ELSE CTeSVM.CON_CHAVECTE END ChaveSVMAAK,
                CTe.CON_NUMERO_CE_MERCANTE NumeroCE,
                CTe.CON_NUMERO_MANIFESTO NumeroManifesto,
                ISNULL(ViagemCinco.PVN_DESCRICAO,ISNULL(ViagemQuatro.PVN_DESCRICAO, ISNULL(ViagemTres.PVN_DESCRICAO, ISNULL(ViagemDois.PVN_DESCRICAO, ISNULL(ViagemUm.PVN_DESCRICAO, Viagem.PVN_DESCRICAO))))) Viagem,
                PortoOrigem.POT_DESCRICAO PortoOrigem,
                PortoDestino.POT_DESCRICAO PortoDestino,
                PortoDestino.POT_CODIGO CodigoPorto,
                GrupoTomador.GRP_EMAIL_DESPACHANTE EmailDespachante
                FROM T_CTE CTe
                JOIN T_CARGA_CTE CargaCTe on CargaCTe.CON_CODIGO = CTe.CON_CODIGO
                JOIN T_CARGA_PEDIDO CargaPedido on CargaPedido.CAR_CODIGO = CargaCTe.CAR_CODIGO
                JOIN T_CTE_PARTICIPANTE Tomador on Tomador.PCT_CODIGO = CTe.CON_TOMADOR_PAGADOR_CTE
                JOIN T_CLIENTE Cliente on Cliente.CLI_CGCCPF = Tomador.CLI_CODIGO
                join T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule on Schedule.PVS_CODIGO = CTe.PVS_CODIGO
                LEFT OUTER JOIN T_GRUPO_PESSOAS GrupoTomador on GrupoTomador.GRP_CODIGO = Cliente.GRP_CODIGO
                LEFT OUTER JOIN T_CLIENTE Despachante on Despachante.CLI_CGCCPF = GrupoTomador.GRP_DESPACHANTE
                JOIN T_PORTO PortoOrigem on PortoOrigem.POT_CODIGO = CTe.POT_CODIGO_ORIGEM
                LEFT OUTER JOIN T_PORTO PortoDestino on PortoDestino.POT_CODIGO = CTe.POT_CODIGO_DESTINO
                LEFT OUTER JOIN T_PORTO PortoPassagemUm on PortoPassagemUm.POT_CODIGO = CTe.POT_CODIGO_PASSAGEM_UM
                LEFT OUTER JOIN T_PORTO PortoPassagemDois on PortoPassagemDois.POT_CODIGO = CTe.POT_CODIGO_PASSAGEM_DOIS
                LEFT OUTER JOIN T_PORTO PortoPassagemTres on PortoPassagemTres.POT_CODIGO = CTe.POT_CODIGO_PASSAGEM_TRES
                LEFT OUTER JOIN T_PORTO PortoPassagemQuatro on PortoPassagemQuatro.POT_CODIGO = CTe.POT_CODIGO_PASSAGEM_QUATRO
                LEFT OUTER JOIN T_PORTO PortoPassagemCinco on PortoPassagemCinco.POT_CODIGO = CTe.POT_CODIGO_PASSAGEM_CINCO
                LEFT OUTER JOIN T_PEDIDO_VIAGEM_NAVIO Viagem on Viagem.PVN_CODIGO = CTe.CON_VIAGEM
                LEFT OUTER JOIN T_PEDIDO_VIAGEM_NAVIO ViagemUm on ViagemUm.PVN_CODIGO = CTe.CON_VIAGEM_PASSAGEM_UM
                LEFT OUTER JOIN T_PEDIDO_VIAGEM_NAVIO ViagemDois on ViagemDois.PVN_CODIGO = CTe.CON_VIAGEM_PASSAGEM_DOIS
                LEFT OUTER JOIN T_PEDIDO_VIAGEM_NAVIO ViagemTres on ViagemTres.PVN_CODIGO = CTe.CON_VIAGEM_PASSAGEM_TRES
                LEFT OUTER JOIN T_PEDIDO_VIAGEM_NAVIO ViagemQuatro on ViagemQuatro.PVN_CODIGO = CTe.CON_VIAGEM_PASSAGEM_QUATRO
                LEFT OUTER JOIN T_PEDIDO_VIAGEM_NAVIO ViagemCinco on ViagemCinco.PVN_CODIGO = CTe.CON_VIAGEM_PASSAGEM_CINCO
                LEFT OUTER JOIN T_CTE CTeSVM on CTeSVM.CON_NUMERO_CONTROLE = CTe.CON_NUMERO_CONTROLE_SVM
                WHERE CTe.CON_STATUS = 'A'
                AND (CTe.CON_NAO_ENVIAR_PARA_MERCANTE IS NULL OR CTe.CON_NAO_ENVIAR_PARA_MERCANTE = 0)
                AND CTe.CON_TIPO_CTE = 0 AND CTe.CON_TIPO_MODAL = 3
                AND CargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL != 3 AND CargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL != 7 AND CargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL != 6 AND CargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL != 5 AND CargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL != 4
                AND ((GrupoTomador.GRP_ADICIONAR_DESPACHANTE_COMO_CONSIGNATARIO = 1 AND GrupoTomador.GRP_EMAIL_DESPACHANTE <> '' and GrupoTomador.GRP_EMAIL_DESPACHANTE is not null) or (CTe.CON_EMAIL_DESPACHANTE_SVM <> '' and CTe.CON_EMAIL_DESPACHANTE_SVM is not null))
                AND (PortoDestino.POT_DIAS_ANTES_DO_POD_PARA_ENVIO_DA_DOCUMENTACAO >= 0 OR PortoPassagemUm.POT_DIAS_ANTES_DO_POD_PARA_ENVIO_DA_DOCUMENTACAO >= 0 OR PortoPassagemDois.POT_DIAS_ANTES_DO_POD_PARA_ENVIO_DA_DOCUMENTACAO >= 0 OR PortoPassagemTres.POT_DIAS_ANTES_DO_POD_PARA_ENVIO_DA_DOCUMENTACAO >= 0 OR PortoPassagemQuatro.POT_DIAS_ANTES_DO_POD_PARA_ENVIO_DA_DOCUMENTACAO >= 0 OR PortoPassagemCinco.POT_DIAS_ANTES_DO_POD_PARA_ENVIO_DA_DOCUMENTACAO >= 0)
                AND DATEDIFF(DAY, '" + DateTime.Now.ToString("yyyy-MM-dd") + @"', CAST(Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO AS date)) = PortoDestino.POT_DIAS_ANTES_DO_POD_PARA_ENVIO_DA_DOCUMENTACAO
                AND ISNULL(ViagemCinco.PVN_CODIGO,ISNULL(ViagemQuatro.PVN_CODIGO, ISNULL(ViagemTres.PVN_CODIGO, ISNULL(ViagemDois.PVN_CODIGO, ISNULL(ViagemUm.PVN_CODIGO, Viagem.PVN_CODIGO))))) = " + codigoViagem + @"
                AND PortoDestino.POT_CODIGO = " + codigoPorto;                

            if (cnpjDespachante > 0)
                sql += " AND Despachante.CLI_CGCCPF = " + cnpjDespachante;

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Documentos.Mercante)));

            return query.SetTimeout(6000).List<Dominio.ObjetosDeValor.Embarcador.Documentos.Mercante>();
        }
    }
}
