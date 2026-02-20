using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Carga
{
    public class CTeSimplificado
    {
        public static bool ValidarCTeSimplificado(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            bool encontrouPedidoSimplificado = false;
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasOrigem = (from obj in cargaPedidos select obj.CargaOrigem).Distinct().ToList();

            foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem in cargasOrigem)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosCargaOrigem = null;
                cargaPedidosCargaOrigem = (from obj in cargaPedidos where obj.CargaOrigem.Codigo == cargaOrigem.Codigo && !obj.Pedido.NaoGlobalizarPedido select obj).ToList();

                List<Dominio.Entidades.Cliente> tomadores = (from obj in cargaPedidosCargaOrigem select obj.ObterTomador()).Distinct().ToList();
                foreach (Dominio.Entidades.Cliente tomador in tomadores)
                {
                    if (tomador != null)
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidoTomador = (from obj in cargaPedidosCargaOrigem where obj.ObterTomador().CPF_CNPJ == tomador.CPF_CNPJ select obj).ToList();
                        List<Dominio.Entidades.Cliente> recebedores = (from obj in cargaPedidoTomador select obj.Recebedor).Distinct().ToList();

                        foreach (Dominio.Entidades.Cliente recebedor in recebedores)
                        {
                            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidoRecebedor = (from obj in cargaPedidoTomador where obj.Recebedor?.CPF_CNPJ == recebedor?.CPF_CNPJ select obj).ToList();
                            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidoRecebedor)
                            {
                                cargaPedido.IndicadorCTeSimplificado = false;
                            }

                            bool removerMunicipais = false;
                            bool possuiCTe = false;
                            bool possuiNFS = false;
                            bool possuiNFSManual = false;
                            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosMunicipais = null;

                            if ((cargaOrigem.TipoOperacao?.ConfiguracaoEmissaoDocumento?.GerarCTeSimplificadoQuandoCompativel ?? false))
                            {
                                cargaPedidosMunicipais = (from obj in cargaPedidoRecebedor where obj.Origem.Codigo == obj.Destino.Codigo select obj).ToList();
                                if (cargaPedidosMunicipais.Count > 0)
                                {
                                    Servicos.Embarcador.Carga.CargaPedido serCargaPedido = new Servicos.Embarcador.Carga.CargaPedido(unitOfWork);
                                    Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoIntramunicipal = null;
                                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoMunicipal = cargaPedidosMunicipais.FirstOrDefault();
                                    serCargaPedido.VerificarQuaisDocumentosDeveEmitir(cargaOrigem, cargaPedidoMunicipal, cargaPedidoMunicipal.Origem, cargaPedidoMunicipal.Destino, tipoServicoMultisoftware, unitOfWork, out possuiCTe, out possuiNFS, out possuiNFSManual, out modeloDocumentoIntramunicipal, configuracao, out bool sempreDisponibilizarDocumentoNFSManual);

                                    if (possuiNFS || possuiNFSManual || !possuiCTe)
                                        removerMunicipais = true;
                                }
                            }

                            if ((cargaOrigem.TipoOperacao?.ConfiguracaoEmissaoDocumento?.GerarCTeSimplificadoQuandoCompativel ?? false))
                            {
                                if (cargaPedidoRecebedor.Count > 0)
                                {
                                    var cargaPedidosInterMunicipais = cargaPedidoRecebedor
                                        .Where(obj => obj.Origem.Codigo != obj.Destino.Codigo)
                                        .ToList();

                                    List<double> cnpjsDestinatario = cargaPedidosInterMunicipais
                                        .Where(obj => obj.Pedido.Destinatario != null && !obj.PedidoSemNFe)
                                        .Select(obj => obj.Pedido.Destinatario.CPF_CNPJ)
                                        .ToList();

                                    if (!removerMunicipais)
                                    {
                                        cnpjsDestinatario.AddRange(
                                            cargaPedidosMunicipais
                                                .Where(obj => obj.Pedido.Destinatario != null && !obj.PedidoSemNFe)
                                                .Select(obj => obj.Pedido.Destinatario.CPF_CNPJ)
                                        );
                                    }

                                    cnpjsDestinatario = cnpjsDestinatario.Distinct().ToList();

                                    if (cnpjsDestinatario.Count > 1)
                                    {
                                        var grupos = cargaPedidosInterMunicipais
                                            .GroupBy(obj => new { OrigemUF = obj.Origem.Estado.Sigla, DestinoUF = obj.Destino.Estado.Sigla })
                                            .Where(g => g.Count() > 1)
                                            .ToList();

                                        foreach (var grupo in grupos)
                                        {
                                            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in grupo)
                                            {
                                                encontrouPedidoSimplificado = true;
                                                cargaPedido.IndicadorCTeSimplificado = true;
                                                cargaPedido.TipoRateio = cargaOrigem.TipoOperacao.TipoEmissaoCTeDocumentos;
                                            }

                                            if (!removerMunicipais)
                                            {
                                                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidosMunicipais)
                                                {
                                                    encontrouPedidoSimplificado = true;
                                                    cargaPedido.IndicadorCTeSimplificado = true;
                                                    cargaPedido.TipoRateio = cargaOrigem.TipoOperacao.TipoEmissaoCTeDocumentos;
                                                }
                                            }
                                        }

                                    }
                                }
                            }
                        }
                    }
                }
            }

            return encontrouPedidoSimplificado;
        }
    }
}