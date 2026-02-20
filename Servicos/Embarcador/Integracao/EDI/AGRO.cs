using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Integracao.EDI
{
    public class AGRO
    {
        public Dominio.ObjetosDeValor.EDI.AGRO.AGRO ConverterCargaEDIIntegracaoParaAGRO(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unidadeTrabalho)
        {

            Dominio.ObjetosDeValor.EDI.AGRO.AGRO agro = new Dominio.ObjetosDeValor.EDI.AGRO.AGRO();
            agro.ColetasLeite = new List<Dominio.ObjetosDeValor.EDI.AGRO.ColetaLeite>();

            //Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unidadeTrabalho);

            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unidadeTrabalho);

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidos = repCargaEntregaPedido.BuscarPorCarga(carga.Codigo);

            Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade repCargaPedidoProdutoDivisaoCapacidade = new Repositorio.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade(unidadeTrabalho);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos = repCargaPedidoProduto.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade> cargaPedidoProdutoDivisaoCapacidades = repCargaPedidoProdutoDivisaoCapacidade.BuscarPorCarga(carga.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto cargaPedidoProduto in cargaPedidoProdutos)
            {
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido cargaEntregaPedido = (from obj in cargaEntregaPedidos where obj.CargaPedido.Codigo == cargaPedidoProduto.CargaPedido.Codigo && obj.CargaEntrega.Coleta select obj).FirstOrDefault();
                if (cargaEntregaPedido != null)
                {
                    Dominio.ObjetosDeValor.EDI.AGRO.ColetaLeite coletaLeite = new Dominio.ObjetosDeValor.EDI.AGRO.ColetaLeite
                    {
                        CodigoFiliada = cargaEntregaPedido.CargaEntrega.Cliente?.GrupoPessoas?.CodigoIntegracao ?? "",
                        CPFProdutor = cargaEntregaPedido.CargaEntrega.Cliente?.CPF_CNPJ_SemFormato ?? "",
                        DataColeta = cargaEntregaPedido.CargaEntrega.DataConfirmacao?.ToString("dd/MM/yyyy HH:mm:ss") ?? "",
                        DataEmissao = cargaEntregaPedido.CargaEntrega.DataConfirmacao?.ToString("dd/MM/yyyy") ?? "",
                        HoraEmissao = cargaEntregaPedido.CargaEntrega.DataConfirmacao?.ToString("HH:mm:ss") ?? "",
                        CodigoProdutor = cargaEntregaPedido.CargaEntrega.Cliente?.CodigoIntegracao ?? "",
                        NomeProdutor = cargaEntregaPedido.CargaEntrega.Cliente?.Nome ?? "",

                        Adicional1 = cargaEntregaPedido.CargaPedido.Pedido.Adicional1,
                        Adicional2 = cargaEntregaPedido.CargaPedido.Pedido.Adicional2,
                        Adicional3 = cargaEntregaPedido.CargaPedido.Pedido.Adicional3,
                        Adicional4 = cargaEntregaPedido.CargaPedido.Pedido.Adicional4,
                        Observacao = cargaEntregaPedido.CargaEntrega.Observacao,
                        Temperatura = cargaPedidoProduto.Temperatura,
                        SeqRota = cargaEntregaPedido.CargaEntrega.OrdemRealizada + 1,
                        NumeroViagem = carga.CodigoCargaEmbarcador,

                        Placa = cargaEntregaPedido.CargaEntrega.Carga.Veiculo?.Placa ?? "",
                        FlagRejeitado = "N",
                        Alizarol = "1"
                    };

                    if (cargaEntregaPedido.CargaEntrega.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Rejeitado)
                    {
                        coletaLeite.FlagRejeitado = "S";
                        coletaLeite.CodigoRejeitado = cargaEntregaPedido.CargaEntrega.MotivoRejeicao?.CodigoIntegracao ?? "";
                        coletaLeite.Alizarol = "";

                        coletaLeite.DataColeta = cargaEntregaPedido.CargaEntrega.DataRejeitado?.ToString("dd/MM/yyyy HH:mm:ss") ?? "";
                        coletaLeite.DataEmissao = cargaEntregaPedido.CargaEntrega.DataRejeitado?.ToString("dd/MM/yyyy") ?? "";
                        coletaLeite.HoraEmissao = cargaEntregaPedido.CargaEntrega.DataRejeitado?.ToString("HH:mm:ss") ?? "";
                    }

                    if (cargaPedidoProdutoDivisaoCapacidades.Count > 0)
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade> cargaPedidoProdutoDivisaoCapacidadesProduto = (from obj in cargaPedidoProdutoDivisaoCapacidades where obj.CargaPedidoProduto.Codigo == cargaPedidoProduto.Codigo select obj).ToList();
                        List<string> sequencias = (from obj in carga.ModeloVeicularCarga.DivisoesCapacidade orderby obj.Descricao select obj.Descricao).ToList();
                        foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade cargaPedidoProdutoDivisaoCapacidade in cargaPedidoProdutoDivisaoCapacidadesProduto)
                        {
                            int ordem = 0;
                            foreach (string capacidade in sequencias)
                            {
                                if (capacidade == cargaPedidoProdutoDivisaoCapacidade.ModeloVeicularCargaDivisaoCapacidade.Descricao)
                                    break;

                                ordem++;
                            }
                            switch (ordem)
                            {
                                case 0:
                                    coletaLeite.VolumeTanque1 = cargaPedidoProdutoDivisaoCapacidade.Quantidade;
                                    break;
                                case 1:
                                    coletaLeite.VolumeTanque2 = cargaPedidoProdutoDivisaoCapacidade.Quantidade;
                                    break;
                                case 2:
                                    coletaLeite.VolumeTanque3 = cargaPedidoProdutoDivisaoCapacidade.Quantidade;
                                    break;
                                case 3:
                                    coletaLeite.VolumeTanque4 = cargaPedidoProdutoDivisaoCapacidade.Quantidade;
                                    break;
                                case 4:
                                    coletaLeite.VolumeTanque5 = cargaPedidoProdutoDivisaoCapacidade.Quantidade;
                                    break;
                                default:
                                    coletaLeite.VolumeTanque1 = cargaPedidoProdutoDivisaoCapacidade.Quantidade;
                                    break;
                            }
                        }
                    }
                    else
                        coletaLeite.VolumeTanque1 = cargaPedidoProduto.Quantidade;

                    coletaLeite.NumeroOrdemCarregamento = cargaEntregaPedido.CargaPedido.Carga.CodigoCargaEmbarcador;
                    agro.ColetasLeite.Add(coletaLeite);
                }
            }
            agro.ColetasLeite = agro.ColetasLeite.OrderBy(obj => obj.SeqRota).ToList();
            return agro;
        }
    }
}
