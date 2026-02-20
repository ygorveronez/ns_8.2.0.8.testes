using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Servicos.Embarcador.Integracao.EDI
{
    public class VGM
    {
        public MemoryStream GerarArquivoVGM(Dominio.Entidades.Embarcador.Cargas.CargaEDIIntegracao cargaEDIIntegracao, Repositorio.UnitOfWork unidadeTrabalho)
        {
            cargaEDIIntegracao.CodigosCTes = new List<int>();
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaEDIIntegracao repCargaEDIIntegracao = new Repositorio.Embarcador.Cargas.CargaEDIIntegracao(unidadeTrabalho);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unidadeTrabalho);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unidadeTrabalho);
            Repositorio.ContainerCTE repContainerCTE = new Repositorio.ContainerCTE(unidadeTrabalho);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeTrabalho);

            MemoryStream arquivoINPUT = new MemoryStream();
            StreamWriter x = new StreamWriter(arquivoINPUT, Encoding.UTF8);

            string numeroSequencialArquivo = cargaEDIIntegracao.Codigo.ToString("D").PadLeft(10, '0');
            int qtdLinhas = 0;
            int qtdContaineres = 0;
            List<Dominio.Entidades.ContainerCTE> containersCTe = null;
            List<Dominio.Entidades.Embarcador.Pedidos.Container> containeres = new List<Dominio.Entidades.Embarcador.Pedidos.Container> ();
            Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalOrigem = null;

            if (cargaEDIIntegracao.Carga?.TerminalOrigem != null)
                terminalOrigem = cargaEDIIntegracao.Carga?.TerminalOrigem;

            List<int> codigosContainersLigadosACarga = repCargaEDIIntegracao.BuscarCodigosContainersPorCarga(cargaEDIIntegracao.Carga.Codigo);

            if (cargaEDIIntegracao.Carga.CargaSVM)
            {
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidosCTeParaSubContratacao = repPedidoCTeParaSubContratacao.BuscarPorCarga(cargaEDIIntegracao.Carga.Codigo);
                if (pedidosCTeParaSubContratacao != null && pedidosCTeParaSubContratacao.Count > 0)
                    containersCTe = repContainerCTE.BuscarPorContainersEChaveAcesso(codigosContainersLigadosACarga, pedidosCTeParaSubContratacao.Select(o => o.CTeTerceiro.ChaveAcesso).ToList());
            }
            else
                containersCTe = repContainerCTE.BuscarPorContainers(codigosContainersLigadosACarga, cargaEDIIntegracao.Carga.Codigo);

            List<int> codigosCTes = new List<int>();
            if (containersCTe != null && containersCTe.Count > 0)
            {
                codigosCTes = containersCTe.Select(o => o.CTE.Codigo).Distinct().ToList();
                containeres.Add(cargaEDIIntegracao.Container);
                cargaEDIIntegracao.CodigosCTes.AddRange(codigosCTes);
            }
            string numeroBooking = "";
            int codigoCarga = 0;

            if (terminalOrigem == null && containersCTe != null && containersCTe.Count > 0)
            {
                terminalOrigem = containersCTe.FirstOrDefault()?.CTE?.TerminalOrigem;
                numeroBooking = containersCTe.FirstOrDefault()?.CTE?.NumeroBooking;
                codigoCarga = cargaEDIIntegracao.Carga.Codigo;
            }
            else if (containersCTe != null && containersCTe.Count > 0)
            {
                numeroBooking = containersCTe.FirstOrDefault()?.CTE?.NumeroBooking;
                codigoCarga = cargaEDIIntegracao.Carga.Codigo;
            }

            x.WriteLine(
                        "UNB+" + //UNB
                        "UNOA:2+" + //UNOA
                        "ALI-MTMS+" + //ALI
                        "HSD+" +// solicitacao para deixar fixo devido tarefa #74499 ((terminalOrigem?.CodigoTerminal ?? "") + "+") + //Codigo terminal Portuario -- nova mudança fica de acordo com a tarefa #75871
                        (DateTime.Now.ToString("yyyyMMdd:hhMM")) + "+" + //Data e Hora Geração
                        (numeroSequencialArquivo + "'")//Numero sequencial arquivo
                       );
            qtdLinhas += 1;

            x.WriteLine(
                        "UNH+" + //UNH
                        (numeroSequencialArquivo + "+") + //Numero sequencial arquivo
                        "VERMAS:D:16A:UN:SMDG10'"//VERMAS
                       );
            qtdLinhas += 1;

            x.WriteLine(
                        "BGM+" + //BGM
                        "VGM+" + //VGM
                        (numeroSequencialArquivo + "+") + //Numero sequencial arquivo
                        "9'" //Tipo Operação
                       );
            qtdLinhas += 1;

            if (containeres != null && containeres.Count > 0)
            {
                foreach (var container in containeres)
                {
                    qtdContaineres += 1;
                    x.WriteLine(
                        "EQD+" + //EQD
                        "CN+" + //CN
                        ((container?.Numero ?? "") + "'") //Numero Container                        
                       );
                    qtdLinhas += 1;

                    x.WriteLine(
                        "RFF+" + //RFF
                        "BN:" + //BN
                        (numeroBooking + "'") //Numero Booking
                       );
                    qtdLinhas += 1;

                    decimal pesoBruto = 0;// container.CTE.Peso;
                    if (repCargaPedido.CargaTipoFeeder(cargaEDIIntegracao.Carga.Codigo))
                    {
                        Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoFeeder = repCargaPedido.BuscarPedidoPorCargaEContainer(cargaEDIIntegracao.Carga.Codigo, container.Codigo);
                        if (pedidoFeeder != null)
                        {
                            pesoBruto = pedidoFeeder.PesoTotal;
                            if (!string.IsNullOrWhiteSpace(pedidoFeeder.TaraContainer) && pedidoFeeder.TaraContainer.ToDecimal() > 0)
                                pesoBruto -= pedidoFeeder.TaraContainer.ToDecimal();
                        }
                        if (pesoBruto <= 0)
                            pesoBruto = repCTe.BuscarPesoConhecimentos(codigosCTes);
                    }
                    else
                        pesoBruto = repCTe.BuscarPesoNotasConhecimento(container.Codigo, codigosCTes);
                    if (pesoBruto <= 0)
                        pesoBruto = repCTe.BuscarPesoNotasConhecimento(codigosCTes);
                    string taraContainer = repPedido.BuscarTaraContinar(container?.Codigo ?? 0, codigosCTes);
                    if (string.IsNullOrWhiteSpace(taraContainer) && container.Tara > 0)
                        taraContainer = Utilidades.String.OnlyNumbers(container.Tara.ToString("n0"));
                    int taraContainerConvertido = 0;
                    int.TryParse(taraContainer, out taraContainerConvertido);
                    pesoBruto += taraContainerConvertido;
                    x.WriteLine(
                        "MEA+" + //MEA
                        "AAE+" + //AAE
                        "VGM+" + //VGM
                        "KGM:" + //KGM
                        (pesoBruto.ToString("F").Replace(",", ".")) + "'" //Numero Booking
                       );
                    qtdLinhas += 1;

                    x.WriteLine(
                       "DOC+" + //DOC
                       "SHP:" + //SHP
                       "VGM:" + //VGM
                       "306'" //306
                      );
                    qtdLinhas += 1;

                    x.WriteLine(
                      "NAD+" + //NAD
                      "SPC+" + //SPC
                      "++" + //++
                      "ALIANCA NAVEGACAO E LOGISTICA LTDA'" //Empresa
                     );
                    qtdLinhas += 1;

                    x.WriteLine(
                      "CTA+" + //CTA
                      "RP+:" + //RP                      
                      "MARCELO FERNANDES DE ALCANTARA'" //Responsavel
                     );
                    qtdLinhas += 1;
                }
            }

            x.WriteLine(
                      "UNT+" + //UNT
                      (qtdLinhas.ToString("D").PadLeft(6, '0')) + "+" + //SPC
                      (numeroSequencialArquivo + "'")//Numero sequencial arquivo                      
                     );
            x.WriteLine(
                      "UNZ+" + //UNZ
                      "1+" +//Qtd Container
                            //(qtdContaineres.ToString("D") + "+") + //Qtd Container
                      (numeroSequencialArquivo + "'")//Numero sequencial arquivo                      
                     );

            x.Flush();

            arquivoINPUT.Seek(0, SeekOrigin.Begin);
            return arquivoINPUT;
        }

    }
}
