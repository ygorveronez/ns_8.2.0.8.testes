using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Fretes
{
    [CustomAuthorize("Fretes/TabelaFreteComissaoImportacao")]
    public class TabelaFreteComissaoImportacaoController : BaseController
    {
		#region Construtores

		public TabelaFreteComissaoImportacaoController(Conexao conexao) : base(conexao) { }

		#endregion


        public async Task<IActionResult> Importar()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoContratoOrigem, codigoContratoDestino;
                int.TryParse(Request.Params("ContratoFreteTransportadorOrigem"), out codigoContratoOrigem);
                int.TryParse(Request.Params("ContratoFreteTransportadorDestino"), out codigoContratoDestino);

                Repositorio.Embarcador.Frete.TabelaFreteComissaoGrupoProduto repTabelaFreteComissaoGrupoProduto = new Repositorio.Embarcador.Frete.TabelaFreteComissaoGrupoProduto(unidadeTrabalho);
                Repositorio.Embarcador.Frete.TabelaFreteComissaoProduto repTabelaFreteComissaoProduto = new Repositorio.Embarcador.Frete.TabelaFreteComissaoProduto(unidadeTrabalho);
                Repositorio.Embarcador.Frete.ContratoFreteTransportador repContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unidadeTrabalho);

                if (codigoContratoOrigem <= 0 || codigoContratoOrigem <= 0)
                    return new JsonpResult(false, true, "Contrato de origem ou destino inválido.");

                if (codigoContratoOrigem == codigoContratoDestino)
                    return new JsonpResult(false, true, "O contrato de origem deve ser diferente do contrato de destino.");

                if (repTabelaFreteComissaoGrupoProduto.ContarPorContratoFrete(codigoContratoDestino) > 0 || repTabelaFreteComissaoProduto.ContarPorContratoFrete(codigoContratoDestino) > 0)
                    return new JsonpResult(false, true, "O contrato de destino não deve possuir nenhuma comissão por produto ou grupo de produto para realizar a importação.");

                Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contratoFreteTransportador = repContratoFreteTransportador.BuscarPorCodigo(codigoContratoDestino);

                List<Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoProduto> comissoesProduto = repTabelaFreteComissaoProduto.BuscarPorContratoFrete(codigoContratoOrigem);
                List<Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoGrupoProduto> comissoesGrupoProduto = repTabelaFreteComissaoGrupoProduto.BuscarPorContratoFrete(codigoContratoOrigem);

                unidadeTrabalho.Start();

                foreach(Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoProduto comissaoProduto in comissoesProduto)
                {
                    Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoProduto comissaoProdutoNova = comissaoProduto.Clonar();

                    comissaoProdutoNova.ContratoFreteTransportador = contratoFreteTransportador;

                    repTabelaFreteComissaoProduto.Inserir(comissaoProdutoNova);
                }

                foreach (Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoGrupoProduto comissaoGrupoProduto in comissoesGrupoProduto)
                {
                    Dominio.Entidades.Embarcador.Frete.TabelaFreteComissaoGrupoProduto comissaoGrupoProdutoNova = comissaoGrupoProduto.Clonar();

                    comissaoGrupoProdutoNova.ContratoFreteTransportador = contratoFreteTransportador;

                    repTabelaFreteComissaoGrupoProduto.Inserir(comissaoGrupoProdutoNova);
                }

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao importar os fretes.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }

        }
    }
}
