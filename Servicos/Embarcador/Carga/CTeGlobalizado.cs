using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Carga
{
    public class CTeGlobalizado
    {
        public static void ValidarCTeGlobalizadoPorDestinatario(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasOrigem = (from obj in cargaPedidos select obj.CargaOrigem).Distinct().ToList();

            foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem in cargasOrigem)
            {
                bool sempreGlobalizar = cargaOrigem.TipoOperacao?.SempreUsarIndicadorGlobalizadoDestinatario ?? false;

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosCargaOrigem = null;
                if (!sempreGlobalizar)//por padrão o sefaz só autoriza CT-es globalizados para prestações realizadas dentro do mesmo estado.
                {
                    //if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    //    cargaPedidosCargaOrigem = (from obj in cargaPedidos where obj.CargaOrigem.Codigo == cargaOrigem.Codigo && !obj.Pedido.NaoGlobalizarPedido && obj.Origem.Estado.Sigla == obj.Destino.Estado.Sigla select obj).ToList();
                    //else
                    cargaPedidosCargaOrigem = (from obj in cargaPedidos where obj.CargaOrigem.Codigo == cargaOrigem.Codigo && !obj.Pedido.NaoGlobalizarPedido && (obj.Origem.Estado.Sigla == obj.Destino.Estado.Sigla || !string.IsNullOrEmpty(obj.Pedido.NumeroControle)) select obj).ToList();
                }
                else
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
                                cargaPedido.IndicadorCTeGlobalizadoDestinatario = false;
                                cargaPedido.IndicadorNFSGlobalizado = false;
                            }

                            bool removerMunicipais = false;
                            bool possuiCTe = false;
                            bool possuiNFS = false;
                            bool possuiNFSManual = false;
                            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosMunicipais = null;

                            if ((cargaOrigem.TipoOperacao?.IndicadorGlobalizadoDestinatario ?? false) || (cargaOrigem.TipoOperacao?.IndicadorGlobalizadoDestinatarioNFSe ?? false) || sempreGlobalizar)
                            {
                                cargaPedidosMunicipais = (from obj in cargaPedidoRecebedor where obj.Origem.Codigo == obj.Destino.Codigo select obj).ToList();
                                if (cargaPedidosMunicipais.Count() > 0)
                                {
                                    Servicos.Embarcador.Carga.CargaPedido serCargaPedido = new Servicos.Embarcador.Carga.CargaPedido(unitOfWork);
                                    Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoIntramunicipal = null;
                                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoMunicipal = cargaPedidosMunicipais.FirstOrDefault();
                                    serCargaPedido.VerificarQuaisDocumentosDeveEmitir(cargaOrigem, cargaPedidoMunicipal, cargaPedidoMunicipal.Origem, cargaPedidoMunicipal.Destino, tipoServicoMultisoftware, unitOfWork, out possuiCTe, out possuiNFS, out possuiNFSManual, out modeloDocumentoIntramunicipal, configuracao, out bool sempreDisponibilizarDocumentoNFSManual);

                                    if (possuiNFS || possuiNFSManual || !possuiCTe)
                                        removerMunicipais = true;
                                }
                            }

                            if ((cargaOrigem.TipoOperacao?.IndicadorGlobalizadoDestinatario ?? false) || sempreGlobalizar)
                            {
                                if (cargaPedidoRecebedor.Count() > 0)
                                {
                                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosInterMuniciapais = (from obj in cargaPedidoRecebedor where obj.Origem.Codigo != obj.Destino.Codigo select obj).ToList();

                                    List<double> cnpjsDestintario = new List<double>();
                                    cnpjsDestintario.AddRange((from obj in cargaPedidosInterMuniciapais where obj.Pedido.Destinatario != null && !obj.PedidoSemNFe select obj.Pedido.Destinatario.CPF_CNPJ).ToList());
                                    if (!removerMunicipais)
                                        cnpjsDestintario.AddRange((from obj in cargaPedidosMunicipais where obj.Pedido.Destinatario != null && !obj.PedidoSemNFe select obj.Pedido.Destinatario.CPF_CNPJ).ToList());

                                    cnpjsDestintario = cnpjsDestintario.Distinct().ToList();

                                    if ((cnpjsDestintario.Count() > 4) || sempreGlobalizar)
                                    {
                                        foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidosInterMuniciapais)
                                        {
                                            cargaPedido.IndicadorCTeGlobalizadoDestinatario = true;
                                            cargaPedido.TipoRateio = cargaOrigem.TipoOperacao.TipoEmissaoCTeDocumentos;
                                        }

                                        if (!removerMunicipais)
                                        {
                                            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidosMunicipais)
                                            {
                                                cargaPedido.IndicadorCTeGlobalizadoDestinatario = true;
                                                cargaPedido.TipoRateio = cargaOrigem.TipoOperacao.TipoEmissaoCTeDocumentos;
                                            }
                                        }
                                    }
                                }
                            }

                            if ((cargaOrigem.TipoOperacao?.IndicadorGlobalizadoDestinatarioNFSe ?? false) && possuiNFS && cargaPedidosMunicipais.Count() > 1)
                            {
                                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidosMunicipais)
                                {
                                    cargaPedido.IndicadorCTeGlobalizadoDestinatario = true;
                                    cargaPedido.IndicadorNFSGlobalizado = true;
                                    cargaPedido.TipoRateio = cargaOrigem.TipoOperacao.TipoEmissaoCTeDocumentos;
                                }
                            }

                            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                            {
                                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidoComNotaCobertura = cargaPedidoRecebedor.Where(o => !string.IsNullOrEmpty(o.Pedido.NumeroControle)).ToList();
                                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargasPedidoComNotaCobertura)
                                {
                                    cargaPedido.IndicadorCTeGlobalizadoDestinatario = true;
                                }
                            }
                        }
                    }
                }

            }
        }
    }
}
