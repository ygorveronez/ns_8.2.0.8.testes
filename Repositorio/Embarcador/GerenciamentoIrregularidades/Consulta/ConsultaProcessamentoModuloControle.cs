namespace Repositorio.Embarcador.GerenciamentoIrregularidades
{
    //sealed class ConsultaProcessamentoModuloControle : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.GerenciamentoIrregularidades.FiltroPesquisaRelatorioProcessamentoModuloControle>
    //{
    //    #region Construtores

    //    public ConsultaProcessamentoModuloControle() : base(tabela: "T_PACOTE as Pacote") { }

    //    #endregion

    //    #region Métodos Privados

    //    private void SetarJoinsCargaPedidoPacote(StringBuilder joins)
    //    {
    //        if (!joins.Contains(" CargaPedPacote "))
    //            joins.Append(" left join T_CARGA_PEDIDO_PACOTE as CargaPedPacote on CargaPedPacote.PCT_CODIGO = Pacote.PCT_CODIGO ");
    //    }

    //    private void SetarJoinsCargaPedido(StringBuilder joins)
    //    {
    //        SetarJoinsCargaPedidoPacote(joins);
    //        if (!joins.Contains(" CargaPedido "))
    //            joins.Append(" left join T_CARGA_PEDIDO as CargaPedido on CargaPedido.CPE_CODIGO = CargaPedPacote.CPE_CODIGO ");
    //    }

    //    private void SetarJoinsPedido(StringBuilder joins)
    //    {
    //        SetarJoinsCargaPedido(joins);
    //        if (!joins.Contains(" Pedido "))
    //            joins.Append(" left join T_PEDIDO as Pedido on  Pedido.PED_CODIGO = CargaPedido.CPE_CODIGO  ");
    //    }

    //    private void SetarJoinsPedidoXMLNotaFiscal(StringBuilder joins)
    //    {
    //        SetarJoinsPedido(joins);
    //        SetarJoinsCargaPedido(joins);
    //        if (!joins.Contains(" PedidoXMLNotaFiscal "))
    //            joins.Append(" left join T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal on PedidoXMLNotaFiscal.CPE_CODIGO = CargaPedido.CPE_CODIGO ");
    //    }

    //    private void SetarJoinsXMLNotaFiscal(StringBuilder joins)
    //    {
    //        SetarJoinsPedidoXMLNotaFiscal(joins);
    //        if (!joins.Contains(" XmlNotaFiscal "))
    //            joins.Append(" left join T_XML_NOTA_FISCAL XmlNotaFiscal on XmlNotaFiscal.NFX_CODIGO = PedidoXMLNotaFiscal.NFX_CODIGO ");
    //    }
    //    private void SetarJoinsCarga(StringBuilder joins)
    //    {
    //        SetarJoinsCargaPedido(joins);
    //        if (!joins.Contains(" Carga "))
    //            joins.Append(" left join T_CARGA as Carga on Carga.CAR_CODIGO = CargaPedido.CAR_CODIGO ");
    //    }

    //    private void SetarJoinsTipoOperacao(StringBuilder joins)
    //    {
    //        if (!joins.Contains(" TipoOperacao "))
    //            joins.Append(" left join T_TIPO_OPERACAO as TipoOperacao on TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO ");
    //    }
    //    private void SetarJoinsContratante(StringBuilder joins)
    //    {
    //        if (!joins.Contains(" Contratante "))
    //            joins.Append(" left join T_CLIENTE as Contratante on Contratante.CLI_CGCCPF = Pacote.CLI_CGCCPF_CONTRATANTE ");
    //    }

    //    private void SetarJoinsOrigem(StringBuilder joins)
    //    {
    //        if (!joins.Contains(" Origem "))
    //            joins.Append(" left join T_CLIENTE as Origem on Origem.CLI_CGCCPF = pacote.CLI_CGCCPF_ORIGEM  ");
    //    }

    //    private void SetarJoinsDestino(StringBuilder joins)
    //    {
    //        if (!joins.Contains(" Destino "))
    //            joins.Append(" left join T_CLIENTE as Destino on Destino.CLI_CGCCPF = Pacote.CLI_CGCCPF_DESTINO ");
    //    }

    //    private void SetarJoinsCteAnteiror(StringBuilder joins)
    //    {
    //        if (!joins.Contains(" CteAnterior "))
    //            joins.Append(" left join T_CTE_TERCEIRO as CteAnterior on Pacote.PCT_LOG_KEY = CteAnterior.CPS_IDENTIFICACAO_PACOTE ");
    //    }

    //    private void SetarJoinsCNPJOrigem(StringBuilder joins)
    //    {
    //        SetarJoinsCteAnteiror(joins);
    //        if (!joins.Contains(" CNPJOrigem "))
    //            joins.Append(" left join T_CTE_PARTICIPANTE as CteParticipanteOrigem on CteAnterior.CPS_REMETENTE_CTE = CteParticipanteOrigem.PCT_CODIGO ");
    //    }

    //    private void SetarJoinsCNPJDestino(StringBuilder joins)
    //    {
    //        SetarJoinsCteAnteiror(joins);
    //        if (!joins.Contains(" CNPJDestino "))
    //            joins.Append(" left join T_CTE_PARTICIPANTE as CteParticipanteDestino on CteAnterior.CPS_DESTINATARIO_CTE = CteParticipanteDestino.PCT_CODIGO ");
    //    }


    //    #endregion

    //    //protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioPacotes filtroPesquisa)
    //    //{

    //    //}

    //    //protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioPacotes filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
    //    //{


    //    //}

    //}

}
