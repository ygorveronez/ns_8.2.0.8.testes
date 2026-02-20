using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pedidos.AlteracaoPedido
{
    public class AlteracaoPedidoController : BaseController
    {
		#region Construtores

		public AlteracaoPedidoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaCamposAlterados()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisaCamposAlterados());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os campos alterados.");
            }
        }

        #endregion

        #region Métodos Privados

        private List<Tuple<string, string, string>> ObterCamposAlteradosPessoa(Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedidoCliente alteracaoPedidoPessoa, Dominio.Entidades.Cliente pessoa, TipoAlteracaoPedidoPessoa tipoPessoa)
        {
            if ((alteracaoPedidoPessoa == null) && (pessoa == null))
                return new List<Tuple<string, string, string>>();

            List<Tuple<string, string, string>> camposAlterados = new List<Tuple<string, string, string>>();

            if (alteracaoPedidoPessoa?.Endereco != null)
            {
                if (alteracaoPedidoPessoa.Endereco.Bairro != pessoa?.Bairro)
                    camposAlterados.Add(new Tuple<string, string, string>($"{tipoPessoa.ObterDescricao()} - Bairro", pessoa?.Bairro, alteracaoPedidoPessoa.Endereco.Bairro));

                if (alteracaoPedidoPessoa.Endereco.Cep != pessoa?.CEP)
                    camposAlterados.Add(new Tuple<string, string, string>($"{tipoPessoa.ObterDescricao()} - CEP", pessoa?.CEP, alteracaoPedidoPessoa.Endereco.Cep));

                if (alteracaoPedidoPessoa.Endereco.Localidade.Codigo != pessoa?.Localidade?.Codigo)
                    camposAlterados.Add(new Tuple<string, string, string>($"{tipoPessoa.ObterDescricao()} - Cidade", pessoa?.Localidade?.Descricao, alteracaoPedidoPessoa.Endereco.Localidade.Descricao));

                if (alteracaoPedidoPessoa.Endereco.Logradouro != pessoa?.Endereco)
                    camposAlterados.Add(new Tuple<string, string, string>($"{tipoPessoa.ObterDescricao()} - Endereço", pessoa?.Endereco, alteracaoPedidoPessoa.Endereco.Logradouro));

                if (alteracaoPedidoPessoa?.IeRg != pessoa?.IE_RG)
                    camposAlterados.Add(new Tuple<string, string, string>($"{tipoPessoa.ObterDescricao()} - IE", pessoa?.IE_RG, alteracaoPedidoPessoa?.IeRg));

                if (alteracaoPedidoPessoa?.Nome != pessoa?.Nome)
                    camposAlterados.Add(new Tuple<string, string, string>($"{tipoPessoa.ObterDescricao()} - Nome", pessoa?.Nome, alteracaoPedidoPessoa?.Nome));

                if (alteracaoPedidoPessoa.Endereco.Numero != pessoa?.Numero)
                    camposAlterados.Add(new Tuple<string, string, string>($"{tipoPessoa.ObterDescricao()} - Número", pessoa?.Numero, alteracaoPedidoPessoa.Endereco.Numero));
            }
            else
            {
                if (alteracaoPedidoPessoa?.IeRg != pessoa?.IE_RG)
                    camposAlterados.Add(new Tuple<string, string, string>($"{tipoPessoa.ObterDescricao()} - IE", pessoa?.IE_RG, alteracaoPedidoPessoa?.IeRg));

                if (alteracaoPedidoPessoa?.Nome != pessoa?.Nome)
                    camposAlterados.Add(new Tuple<string, string, string>($"{tipoPessoa.ObterDescricao()} - Nome", pessoa?.Nome, alteracaoPedidoPessoa?.Nome));
            }

            return camposAlterados;
        }

        private Models.Grid.Grid ObterGridPesquisaCamposAlterados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho(descricao: "Campo", propriedade: "Campo", tamanho: 30, alinhamento: Models.Grid.Align.left);
                grid.AdicionarCabecalho(descricao: "Valor Atual", propriedade: "ValorAtual", tamanho: 35, alinhamento: Models.Grid.Align.left);
                grid.AdicionarCabecalho(descricao: "Valor Novo", propriedade: "ValorNovo", tamanho: 35, alinhamento: Models.Grid.Align.left);

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido repositorio = new Repositorio.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido alteracaoPedido = repositorio.BuscarPorCodigo(codigo, auditavel: false);
                List<Tuple<string, string, string>> camposAlterados = new List<Tuple<string, string, string>>();

                if (alteracaoPedido != null)
                {
                    Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = alteracaoPedido.Pedido;

                    if (pedido.Companhia != alteracaoPedido.Companhia)
                        camposAlterados.Add(new Tuple<string, string, string>("Companhia", pedido.Companhia, alteracaoPedido.Companhia));

                    if (pedido.DataETA != alteracaoPedido.DataETA)
                        camposAlterados.Add(new Tuple<string, string, string>("ETA", pedido.DataETA?.ToString("dd/MM/yyyy HH:mm"), alteracaoPedido.DataETA?.ToString("dd/MM/yyyy HH:mm")));

                    List<Tuple<string, string, string>> camposAlteradosDestinatario = ObterCamposAlteradosPessoa(alteracaoPedido.Destinatario, pedido.Destinatario, TipoAlteracaoPedidoPessoa.Destinatario);

                    if (camposAlteradosDestinatario.Count > 0)
                        camposAlterados.AddRange(camposAlteradosDestinatario);

                    if (pedido.NumeroNavio != alteracaoPedido.NumeroNavio)
                        camposAlterados.Add(new Tuple<string, string, string>("Número do Navio", pedido.NumeroNavio, alteracaoPedido.NumeroNavio));

                    if (pedido.Ordem != alteracaoPedido.Ordem)
                        camposAlterados.Add(new Tuple<string, string, string>("Ordem", pedido.Ordem, alteracaoPedido.Ordem));

                    if (pedido.PesoTotal != alteracaoPedido.PesoTotal)
                        camposAlterados.Add(new Tuple<string, string, string>("Peso Total", pedido.PesoTotal.ToString("N2"), alteracaoPedido.PesoTotal.ToString("N2")));

                    if (pedido.PortoChegada != alteracaoPedido.PortoChegada)
                        camposAlterados.Add(new Tuple<string, string, string>("Porto de Chegada", pedido.PortoChegada, alteracaoPedido.PortoChegada));

                    if (pedido.PortoSaida != alteracaoPedido.PortoSaida)
                        camposAlterados.Add(new Tuple<string, string, string>("Porto de Saida", pedido.PortoSaida, alteracaoPedido.PortoSaida));

                    if (pedido.PrevisaoEntrega != alteracaoPedido.PrevisaoEntrega)
                        camposAlterados.Add(new Tuple<string, string, string>("Previsão de Entrega", pedido.PrevisaoEntrega?.ToString("dd/MM/yyyy HH:mm"), alteracaoPedido.PrevisaoEntrega?.ToString("dd/MM/yyyy HH:mm")));

                    List<Tuple<string, string, string>> camposAlteradosRecebedor = ObterCamposAlteradosPessoa(alteracaoPedido.Recebedor, pedido.Recebedor, TipoAlteracaoPedidoPessoa.Recebedor);

                    if (camposAlteradosRecebedor.Count > 0)
                        camposAlterados.AddRange(camposAlteradosRecebedor);

                    List<Tuple<string, string, string>> camposAlteradosRemetente = ObterCamposAlteradosPessoa(alteracaoPedido.Remetente, pedido.Remetente, TipoAlteracaoPedidoPessoa.Remetente);

                    if (camposAlteradosRemetente.Count > 0)
                        camposAlterados.AddRange(camposAlteradosRemetente);

                    if (pedido.Reserva != alteracaoPedido.Reserva)
                        camposAlterados.Add(new Tuple<string, string, string>("Reserva", pedido.Reserva, alteracaoPedido.Reserva));

                    if (pedido.Resumo != alteracaoPedido.Resumo)
                        camposAlterados.Add(new Tuple<string, string, string>("Resumo", pedido.Resumo, alteracaoPedido.Resumo));

                    if (pedido.Temperatura != alteracaoPedido.Temperatura)
                        camposAlterados.Add(new Tuple<string, string, string>("Temperatura", pedido.Temperatura, alteracaoPedido.Temperatura));

                    if (pedido.TipoEmbarque != alteracaoPedido.TipoEmbarque)
                        camposAlterados.Add(new Tuple<string, string, string>("Tipo de Embarque", pedido.TipoEmbarque, alteracaoPedido.TipoEmbarque));

                    if (pedido.Vendedor != alteracaoPedido.Vendedor)
                        camposAlterados.Add(new Tuple<string, string, string>("Vendedor", pedido.Vendedor, alteracaoPedido.Vendedor));
                }

                var camposAlteradosRetornar = (
                    from o in camposAlterados
                    select new
                    {
                        Campo = o.Item1,
                        ValorAtual = o.Item2,
                        ValorNovo = o.Item3
                    }
                ).ToList();

                grid.AdicionaRows(camposAlteradosRetornar);
                grid.setarQuantidadeTotal(camposAlterados.Count());

                return grid;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
