using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Carga
{
    public class CargaDadosSumarizados : ServicoBase
    {
        #region Construtores
               

        public CargaDadosSumarizados(UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }

        #endregion Construtores

        #region Métodos Privados

        private string ObterSubContratantesCarga(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos)
        {
            string strSubContratante = "";
            List<Dominio.Entidades.Cliente> subContratantes = (from obj in cargaPedidos select obj.Pedido.SubContratante).Distinct().ToList();

            foreach (Dominio.Entidades.Cliente subContratante in subContratantes)
            {
                if (subContratante != null)
                    strSubContratante += subContratante.Nome + " (" + subContratante.CPF_CNPJ_Formatado + "). ";
            }

            return strSubContratante;
        }

        private string ObterSubContratantesPreCarga(List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos)
        {
            System.Text.StringBuilder aubContratanteDescricao = new System.Text.StringBuilder();
            List<Dominio.Entidades.Cliente> subContratantes = (from pedido in pedidos where (pedido.SubContratante != null) select pedido.SubContratante).Distinct().ToList();

            foreach (Dominio.Entidades.Cliente subContratante in subContratantes)
                aubContratanteDescricao.Append(subContratante.Nome + " (" + subContratante.CPF_CNPJ_Formatado + "). ");

            return aubContratanteDescricao.ToString();
        }

        private List<Dominio.ObjetosDeValor.Localidade> ObterOrigens(List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao> cargaLocaisPrestacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            List<Dominio.ObjetosDeValor.Localidade> origens = new List<Dominio.ObjetosDeValor.Localidade>();

            List<Dominio.Entidades.Localidade> locOrigens = (from obj in cargaLocaisPrestacao where obj.LocalidadeInicioPrestacao != null orderby obj.Codigo ascending select obj.LocalidadeInicioPrestacao).Distinct().ToList();

            foreach (Dominio.Entidades.Localidade origem in locOrigens)
            {
                if (origem.CodigoIBGE != 9999999)
                    origens.Add(new Dominio.ObjetosDeValor.Localidade() { Codigo = origem.Codigo, Descricao = origem.DescricaoCidadeEstado, SiglaUF = origem.Estado.Sigla });
                else
                {
                    origens.Add(new Dominio.ObjetosDeValor.Localidade() { Codigo = origem.Codigo, Descricao = origem.DescricaoCidadeEstado, SiglaUF = origem.Pais?.Abreviacao ?? string.Empty });
                }
            }

            return origens;
        }

        private List<Dominio.ObjetosDeValor.Localidade> ObterDestinos(List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao> cargaLocaisPrestacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            List<Dominio.ObjetosDeValor.Localidade> destinos = new List<Dominio.ObjetosDeValor.Localidade>();
            List<Dominio.Entidades.Localidade> locDestinos = (from obj in cargaLocaisPrestacao where obj.LocalidadeTerminoPrestacao != null orderby obj.Codigo ascending select obj.LocalidadeTerminoPrestacao).Distinct().ToList();

            foreach (Dominio.Entidades.Localidade destino in locDestinos)
            {
                if (destino.CodigoIBGE != 9999999)
                    destinos.Add(new Dominio.ObjetosDeValor.Localidade() { Codigo = destino.Codigo, Descricao = destino.DescricaoCidadeEstado, SiglaUF = destino.Estado.Sigla });
                else
                    destinos.Add(new Dominio.ObjetosDeValor.Localidade() { Codigo = destino.Codigo, Descricao = destino.DescricaoCidadeEstado, SiglaUF = destino.Pais?.Abreviacao ?? "" });

            }
            return destinos;
        }

        private List<Dominio.ObjetosDeValor.Localidade> ObterDestinos(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {

            List<Dominio.ObjetosDeValor.Localidade> destinos = new List<Dominio.ObjetosDeValor.Localidade>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> CargaPedidosOrdenada = cargaPedidos.OrderBy(obj => obj.OrdemEntrega).ToList();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in CargaPedidosOrdenada)
            {
                if (cargaPedido.Destino != null)
                {
                    if (cargaPedido.Recebedor == null || (cargaPedido.Pedido.UsarOutroEnderecoDestino && cargaPedido.Pedido.EnderecoDestino?.ClienteOutroEndereco?.Cliente?.CPF_CNPJ == cargaPedido.Recebedor.CPF_CNPJ && (cargaPedido.Carga.TipoOperacao?.UtilizarRecebedorApenasComoParticipante ?? false)))
                    {
                        if (!destinos.Exists(obj => obj.Codigo == cargaPedido.Destino.Codigo))
                        {
                            if (cargaPedido.Destino.CodigoIBGE != 9999999)
                                destinos.Add(new Dominio.ObjetosDeValor.Localidade() { Codigo = cargaPedido.Destino.Codigo, Descricao = cargaPedido.Destino.DescricaoCidadeEstado, SiglaUF = cargaPedido.Destino.Estado?.Sigla, Pais = new Dominio.ObjetosDeValor.Embarcador.Localidade.Pais() { CodigoPais = cargaPedido.Destino.Pais?.Codigo ?? 0, NomePais = cargaPedido.Destino.Pais?.Nome ?? string.Empty, SiglaPais = cargaPedido.Destino.Pais?.Sigla ?? string.Empty } });
                            else
                                destinos.Add(new Dominio.ObjetosDeValor.Localidade() { Codigo = cargaPedido.Destino.Codigo, Descricao = cargaPedido.Destino.DescricaoCidadeEstado, SiglaUF = cargaPedido.Destino.Pais?.Abreviacao ?? "", Pais = new Dominio.ObjetosDeValor.Embarcador.Localidade.Pais() { CodigoPais = cargaPedido.Destino.Pais?.Codigo ?? 0, NomePais = cargaPedido.Destino.Pais?.Nome ?? string.Empty, SiglaPais = cargaPedido.Destino.Pais?.Sigla ?? string.Empty } });
                        }
                    }
                    else
                    {
                        if (!destinos.Exists(obj => obj.Codigo == cargaPedido.Recebedor.Localidade.Codigo))
                            destinos.Add(new Dominio.ObjetosDeValor.Localidade() { Codigo = cargaPedido.Recebedor.Localidade.Codigo, Descricao = cargaPedido.Recebedor.Localidade.DescricaoCidadeEstado, SiglaUF = cargaPedido.Recebedor.Localidade.Estado.Sigla });
                    }
                }
            }

            return destinos;
        }

        private string ObterDescricaoDestinos(List<Dominio.ObjetosDeValor.Localidade> destinos, Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaFronteira> fronteiras)
        {
            string destino = string.Join(" / ", from obj in destinos select obj.Descricao);

            if (fronteiras.Count > 0)
            {
                destino += " - Fronteira " + string.Join(", ", from o in fronteiras select o.Fronteira.Nome);
            }

            return destino;
        }

        private List<Dominio.ObjetosDeValor.Localidade> ObterOrigens(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            List<Dominio.ObjetosDeValor.Localidade> origens = new List<Dominio.ObjetosDeValor.Localidade>();

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> CargaPedidosOrdenada = cargaPedidos.OrderBy(obj => obj.OrdemColeta).ToList();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in CargaPedidosOrdenada)
            {
                if (cargaPedido.Expedidor == null)
                {
                    if (cargaPedido.Origem != null)
                    {
                        if (!origens.Exists(obj => obj.Codigo == cargaPedido.Origem.Codigo))
                        {
                            if (cargaPedido.Origem.CodigoIBGE != 9999999)
                                origens.Add(new Dominio.ObjetosDeValor.Localidade() { Codigo = cargaPedido.Origem.Codigo, Descricao = cargaPedido.Origem.DescricaoCidadeEstado, SiglaUF = cargaPedido.Origem.Estado?.Sigla, Pais = new Dominio.ObjetosDeValor.Embarcador.Localidade.Pais() { CodigoPais = cargaPedido.Origem.Pais?.Codigo ?? 0, NomePais = cargaPedido.Origem.Pais?.Nome ?? string.Empty, SiglaPais = cargaPedido.Origem.Pais?.Sigla ?? string.Empty } });
                            else
                                origens.Add(new Dominio.ObjetosDeValor.Localidade() { Codigo = cargaPedido.Origem.Codigo, Descricao = cargaPedido.Origem.DescricaoCidadeEstado, SiglaUF = cargaPedido.Origem.Pais?.Abreviacao ?? "", Pais = new Dominio.ObjetosDeValor.Embarcador.Localidade.Pais() { CodigoPais = cargaPedido.Origem.Pais?.Codigo ?? 0, NomePais = cargaPedido.Origem.Pais?.Nome ?? string.Empty, SiglaPais = cargaPedido.Origem.Pais?.Sigla ?? string.Empty } });
                        }
                    }
                }
                else
                {
                    if (!origens.Exists(obj => obj.Codigo == cargaPedido.Expedidor.Localidade.Codigo))
                        origens.Add(new Dominio.ObjetosDeValor.Localidade() { Codigo = cargaPedido.Expedidor.Localidade.Codigo, Descricao = cargaPedido.Expedidor.Localidade.DescricaoCidadeEstado, SiglaUF = cargaPedido.Expedidor.Localidade.Estado.Sigla });
                }
            }
            return origens;
        }

        private string ObterDescricaoOrigens(List<Dominio.ObjetosDeValor.Localidade> origens)
        {
            string origem = string.Join(" / ", from obj in origens select obj.Descricao);

            return origem;
        }

        private string ObterDescricaoPaisLocalidades(List<Dominio.ObjetosDeValor.Localidade> localidades)
        {
            return string.Join(" / ", (from obj in localidades where obj.Pais != null select obj.Pais.NomePais).Distinct());
        }

        private string ObterDescricaoUFLocalidades(List<Dominio.ObjetosDeValor.Localidade> localidades)
        {
            string origem = string.Join(" / ", (from obj in localidades select obj.SiglaUF).Distinct());

            return origem;
        }

        private string ObterLocalParqueamento(Dominio.Entidades.Cliente localParqueamento)
        {
            if (localParqueamento == null)
                return null;

            return $"{localParqueamento.Nome} ({localParqueamento.Localidade.DescricaoCidadeEstado})";
        }

        private string ObterOrigemDestinosTMS(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            string origemDestino = "";

            int numeroEntregas;
            long distancia = 0;
            if (carga?.DadosSumarizados != null)
            {
                distancia = (long)carga.DadosSumarizados.Distancia;
                numeroEntregas = carga.DadosSumarizados.NumeroEntregas;

                origemDestino += carga.DadosSumarizados.Origens + " (" + carga.DadosSumarizados.Remetentes + ") até (" + carga.DadosSumarizados.Destinatarios + ") " + carga.DadosSumarizados.Destinos;
                if (numeroEntregas > 1)
                    origemDestino += " com " + numeroEntregas + " entregas";

                if (distancia > 0)
                    origemDestino += " (" + distancia + " KM)";
            }

            return origemDestino;
        }

        private List<Dominio.Entidades.Cliente> ObterDestinatarios(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXML = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

            List<Dominio.Entidades.Cliente> clientesDestinatarios = new List<Dominio.Entidades.Cliente>();
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos> tiposEmissaoCte = (from obj in cargaPedidos select obj.TipoRateio).Distinct().ToList();

            foreach (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos tipoEmissaoCTeDocumentos in tiposEmissaoCte)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosTipoEmissao = (from obj in cargaPedidos where obj.TipoRateio == tipoEmissaoCTeDocumentos select obj).ToList();

                if (tipoEmissaoCTeDocumentos == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalAgrupadaEntrePedidos || tipoEmissaoCTeDocumentos == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalAgrupada || tipoEmissaoCTeDocumentos == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalIndividual)
                {
                    List<int> codigosCargasPedidos = (from o in cargaPedidosTipoEmissao select o.Codigo).ToList();
                    List<Dominio.Entidades.Cliente> destinatarioXMLNota = repPedidoXML.BuscarDestinatariosPorCargaPedidos(codigosCargasPedidos);
                    clientesDestinatarios.AddRange(destinatarioXMLNota);
                }
                else
                {
                    List<Dominio.Entidades.Cliente> destinatarios = (from obj in cargaPedidosTipoEmissao where obj.Pedido.Destinatario != null select obj.Pedido.Destinatario).ToList();

                    clientesDestinatarios.AddRange(destinatarios);
                }
            }

            return clientesDestinatarios.Distinct().ToList();
        }

        private async Task<List<Dominio.Entidades.Cliente>> ObterDestinatariosAsync(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXML = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

            List<Dominio.Entidades.Cliente> clientesDestinatarios = new List<Dominio.Entidades.Cliente>();
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos> tiposEmissaoCte = (from obj in cargaPedidos select obj.TipoRateio).Distinct().ToList();

            foreach (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos tipoEmissaoCTeDocumentos in tiposEmissaoCte)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosTipoEmissao = (from obj in cargaPedidos where obj.TipoRateio == tipoEmissaoCTeDocumentos select obj).ToList();

                if (tipoEmissaoCTeDocumentos == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalAgrupadaEntrePedidos || tipoEmissaoCTeDocumentos == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalAgrupada || tipoEmissaoCTeDocumentos == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalIndividual)
                {
                    List<int> codigosCargasPedidos = (from o in cargaPedidosTipoEmissao select o.Codigo).ToList();
                    List<Dominio.Entidades.Cliente> destinatarioXMLNota = await repPedidoXML.BuscarDestinatariosPorCargaPedidosAsync(codigosCargasPedidos);
                    clientesDestinatarios.AddRange(destinatarioXMLNota);
                }
                else
                {
                    List<Dominio.Entidades.Cliente> destinatarios = (from obj in cargaPedidosTipoEmissao where obj.Pedido.Destinatario != null select obj.Pedido.Destinatario).ToList();

                    clientesDestinatarios.AddRange(destinatarios);
                }
            }

            return clientesDestinatarios.Distinct().ToList();
        }

        private int ObterNumeroColetasPorPedido(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos)
        {
            List<double> cnpjs = (from obj in cargaPedidos where obj.Expedidor != null select obj.Expedidor.CPF_CNPJ).Distinct().ToList();
            cnpjs.AddRange((from obj in cargaPedidos where obj.Expedidor == null && obj.Pedido.Remetente != null && !obj.Pedido.UsarOutroEnderecoOrigem select obj.Pedido.Remetente.CPF_CNPJ).Distinct().ToList());

            List<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco> clienteOutroEndereco = (from obj in cargaPedidos where obj.Expedidor == null && obj.Pedido.UsarOutroEnderecoOrigem select obj.Pedido.EnderecoOrigem.ClienteOutroEndereco).Distinct().ToList();

            int numeroColetas = cnpjs.Distinct().Count();
            numeroColetas += clienteOutroEndereco.Count();

            return numeroColetas;
        }

        private int ObterNumeroColetasPorPedido(List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos)
        {
            List<double> cnpjs = (from pedido in pedidos where (pedido.Remetente != null) && !pedido.UsarOutroEnderecoOrigem select pedido.Remetente.CPF_CNPJ).Distinct().ToList();
            List<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco> clienteOutroEndereco = (from pedido in pedidos where pedido.UsarOutroEnderecoOrigem select pedido.EnderecoOrigem.ClienteOutroEndereco).Distinct().ToList();
            int numeroColetas = cnpjs.Distinct().Count() + clienteOutroEndereco.Count();

            return numeroColetas;
        }

        private int ObterNumeroEntregasPorPedido(List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos)
        {
            List<double> cnpjs = (from pedido in pedidos where (pedido.Destinatario != null) && !pedido.UsarOutroEnderecoDestino select pedido.Destinatario.CPF_CNPJ).Distinct().ToList();
            List<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco> clientesOutroEndereco = (from pedido in pedidos where pedido.UsarOutroEnderecoDestino select pedido.EnderecoDestino.ClienteOutroEndereco).Distinct().ToList();
            int numeroEntregas = cnpjs.Distinct().Count();

            numeroEntregas += (from clienteOutroEndereco in clientesOutroEndereco where clienteOutroEndereco.CodigoEmbarcador != null select clienteOutroEndereco.CodigoEmbarcador).Distinct().Count();
            numeroEntregas += (from clienteOutroEndereco in clientesOutroEndereco where clienteOutroEndereco.CodigoEmbarcador == null select clienteOutroEndereco).Count();

            if (numeroEntregas == 0)
                numeroEntregas = 1;

            return numeroEntregas;
        }

        private int ObterNumeroEntregasEmissaoPorNota(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Repositorio.UnitOfWork unitOfWork)
        {
            int numeroEntregas = 0;

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosEmissaoPorNota = (
                from obj in cargaPedidos
                where (
                    obj.TipoRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalAgrupada ||
                    obj.TipoRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalAgrupadaEntrePedidos ||
                    obj.TipoRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorNotaFiscalIndividual
                )
                select obj
            ).ToList();

            if (cargaPedidosEmissaoPorNota.Count > 0)
            {
                numeroEntregas += (from obj in cargaPedidosEmissaoPorNota where obj.Recebedor != null select obj.Recebedor.CPF_CNPJ).Distinct().ToList().Count();

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosNota = (from obj in cargaPedidosEmissaoPorNota where obj.Recebedor == null select obj).ToList();

                if (cargaPedidosNota.Count > 0)
                {
                    Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

                    int numeroEntregasXMLNotaFiscal = repositorioPedidoXMLNotaFiscal.BuscarCpfCnpjDestinatariosPorCargaPedido(cargaPedidosNota).Count();

                    if (numeroEntregasXMLNotaFiscal > 0)
                        numeroEntregas += numeroEntregasXMLNotaFiscal;
                    else
                        numeroEntregas += (from obj in cargaPedidosNota where obj.Pedido.Destinatario != null select obj.Pedido.Destinatario.CPF_CNPJ).Distinct().ToList().Count();
                }
            }

            return numeroEntregas;
        }

        private int ObterNumeroEntregasEmissaoPorPedido(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, bool ignoraRecebedor = false)
        {
            int numeroEntregas = 0;

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosEmissaoPorPedido = (
                from obj in cargaPedidos
                where (
                    obj.TipoRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.NaoInformado ||
                    obj.TipoRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorPedidoAgrupado ||
                    obj.TipoRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorPedidoIndividual
                )
                select obj
            ).ToList();

            if (cargaPedidosEmissaoPorPedido.Count == 0)
                return numeroEntregas;

            List<double> cnpjs = new List<double>();

            cnpjs.AddRange((from obj in cargaPedidosEmissaoPorPedido where obj.Recebedor != null select obj.Recebedor.CPF_CNPJ).Distinct().ToList());
            cnpjs.AddRange((from obj in cargaPedidosEmissaoPorPedido where obj.Recebedor == null && obj.Pedido.Destinatario != null && (!obj.Pedido.UsarOutroEnderecoDestino || obj.Pedido.EnderecoDestino?.ClienteOutroEndereco == null) select obj.Pedido.Destinatario.CPF_CNPJ).Distinct().ToList());

            numeroEntregas = cnpjs.Distinct().Count();

            List<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco> clienteOutroEndereco = (from obj in cargaPedidosEmissaoPorPedido
                                                                                                    where obj.Pedido.UsarOutroEnderecoDestino &&
                                                                                                    obj.Pedido.EnderecoDestino != null &&
                                                                                                    obj.Pedido.EnderecoDestino.ClienteOutroEndereco != null &&
                                                                                                    (obj.Recebedor == null || obj.Recebedor?.CPF_CNPJ == obj.Pedido.EnderecoDestino.ClienteOutroEndereco.Cliente.CPF_CNPJ)
                                                                                                    select obj.Pedido.EnderecoDestino.ClienteOutroEndereco).Distinct().ToList();

            if (ignoraRecebedor)
                clienteOutroEndereco = (from obj in cargaPedidosEmissaoPorPedido
                                        where obj.Recebedor == null &&
                                        obj.Pedido.UsarOutroEnderecoDestino &&
                                        obj.Pedido.EnderecoDestino != null &&
                                        obj.Pedido.EnderecoDestino.ClienteOutroEndereco != null
                                        select obj.Pedido.EnderecoDestino.ClienteOutroEndereco).Distinct().ToList();

            if (clienteOutroEndereco?.Count > 0)
            {
                numeroEntregas += (from obj in clienteOutroEndereco where obj.CodigoEmbarcador != null select obj.CodigoEmbarcador).Distinct()?.Count() ?? 0;
                numeroEntregas += (from obj in clienteOutroEndereco where obj.CodigoEmbarcador == null select obj).Count();
            }

            return numeroEntregas;
        }

        private bool PossuiRestricaoHorarioCarregamento(List<Dominio.Entidades.Cliente> destinatarios, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.CentroDescarregamento repositorioCentroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento> centrosDescarregamento = repositorioCentroDescarregamento.BuscarPorDestinatarios(destinatarios.Select(d => d.CPF_CNPJ).ToList());

            return centrosDescarregamento.Any(cd => cd.PeriodosDescarregamento != null && cd.PeriodosDescarregamento.Count > 0);
        }

        private async Task<bool> PossuiRestricaoHorarioCarregamentoAsync(List<Dominio.Entidades.Cliente> destinatarios, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.CentroDescarregamento repositorioCentroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(unitOfWork);
            return await repositorioCentroDescarregamento.PossuiRestricaoHorarioCarregamentoPorDestinatariosAsync(destinatarios.Select(d => d.CPF_CNPJ).ToList());
        }

        private void VerificarDestinatariosFilialDestino(ICollection<Dominio.Entidades.Cliente> clientesDestinatarios, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            if (clientesDestinatarios != null && clientesDestinatarios.Count == 1)
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Dominio.Entidades.Cliente clienteDestinatario = (from destinatario in clientesDestinatarios select destinatario).FirstOrDefault();
                if (clienteDestinatario != null)
                    carga.FilialDestino = repositorioFilial.BuscarPorCNPJ(clienteDestinatario.CPF_CNPJ_SemFormato);

                repCarga.Atualizar(carga);
            }
        }

        private async Task VerificarDestinatariosFilialDestinoAsync(ICollection<Dominio.Entidades.Cliente> clientesDestinatarios, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {

            if (clientesDestinatarios != null && clientesDestinatarios.Count == 1)
            {
                Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Dominio.Entidades.Cliente clienteDestinatario = (from destinatario in clientesDestinatarios select destinatario).FirstOrDefault();
                if (clienteDestinatario != null)
                    carga.FilialDestino = await repositorioFilial.BuscarPorCNPJAsync(clienteDestinatario.CPF_CNPJ_SemFormato);

                await repCarga.AtualizarAsync(carga);
            }
        }

        private string BuscarNumeroFrotasVeiculos(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            List<string> numeroFrotasVeiculosVinculados = new List<string>();

            if (!string.IsNullOrWhiteSpace(carga.Veiculo?.NumeroFrota))
                numeroFrotasVeiculosVinculados.Add(carga.Veiculo.NumeroFrota);

            if (carga.VeiculosVinculados != null && carga.VeiculosVinculados.Count() > 0)
            {
                foreach (Dominio.Entidades.Veiculo veiculo in carga.VeiculosVinculados)
                {
                    if (!string.IsNullOrWhiteSpace(veiculo.NumeroFrota))
                        numeroFrotasVeiculosVinculados.Add(veiculo.NumeroFrota);
                }
            }

            return string.Join(", ", numeroFrotasVeiculosVinculados);
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas ObterGrupPesssoasPrincipal(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaPedidos.FirstOrDefault();

            if (cargaPedido != null)
            {
                Dominio.Entidades.Cliente tomador = cargaPedido.ObterTomador();//todo:rever a melhor forma de buscar o grupo de pessoa principal da carga.

                if (tomador != null)
                    return tomador.GrupoPessoas;
                else
                    return cargaPedido.Pedido.GrupoPessoas;
            }
            return null;
        }

        public void SetarGrupoPrincipalCarga(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = ObterGrupPesssoasPrincipal(cargaPedidos, carga, unitOfWork);
            if (grupoPessoas != null)
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                carga.GrupoPessoaPrincipal = grupoPessoas;
                repCarga.Atualizar(carga);
            }
        }

        public void AtualizarDadosCTesFaturadosIntegrados(int codigoFatura, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repFaturaDocumento.BuscarCargas(codigoFatura);
            if (cargas != null && cargas.Count > 0)
            {
                foreach (var carga in cargas)
                {
                    carga.TodosCTesFaturadosIntegrados = repFaturaDocumento.TodasIntegracoesFaturasFinalizadasDaCarga(carga.Codigo);

                    repCarga.Atualizar(carga);

                    serHubCarga.InformarCargaAtualizada(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, unitOfWork.StringConexao);
                }
            }
        }

        public void AtualizarDadosCTesFaturados(int codigoFatura, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();

            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Intercab);

            if (tipoIntegracao != null)
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unitOfWork);


                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repFaturaDocumento.BuscarCargas(codigoFatura);
                if (cargas != null && cargas.Count > 0)
                {
                    foreach (var carga in cargas)
                    {
                        carga.TodosCTesFaturados = !repFaturaDocumento.TodasFaturasFinalizadasDaCarga(carga.Codigo);

                        repCarga.Atualizar(carga);

                        serHubCarga.InformarCargaAtualizada(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, unitOfWork.StringConexao);
                    }
                }
            }

        }

        public void ConsultarMDFeAquaviarioJaGerado(int codigoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMDFeManual repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiroPedidoPorCarga(codigoCarga);

            if (carga.MDFeAquaviarioVinculado)
                return;

            int codigoViagem = cargaPedido.Pedido.PedidoViagemNavio?.Codigo ?? 0, codigoPortoOrigem = cargaPedido.Pedido.Porto?.Codigo ?? 0, codigoPortoDestino = cargaPedido.Pedido?.PortoDestino?.Codigo ?? 0, codigoTerminalOrigem = cargaPedido.Pedido.TerminalOrigem?.Codigo ?? 0, codigoTerminalDestino = cargaPedido.Pedido.TerminalDestino?.Codigo ?? 0;
            carga.MDFeAquaviarioGeradoMasNaoVinculado = false;

            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual> mdfesManual = repCargaMDFeManual.BuscarPorPortosTerminais(codigoViagem, codigoPortoOrigem, codigoPortoDestino, codigoTerminalOrigem, codigoTerminalDestino);
            if (mdfesManual != null && mdfesManual.Count > 0)
                carga.MDFeAquaviarioGeradoMasNaoVinculado = mdfesManual.Any(c => c.MDFeManualMDFes.Any(p => p.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado));

            if (cargaPedido.Pedido.Transbordos != null && cargaPedido.Pedido.Transbordos.Count > 0)
            {
                foreach (var transbordo in cargaPedido.Pedido.Transbordos)
                {
                    codigoViagem = transbordo.PedidoViagemNavio?.Codigo ?? 0; codigoPortoOrigem = cargaPedido.Pedido.Porto?.Codigo ?? 0; codigoPortoDestino = transbordo.Porto?.Codigo ?? 0; codigoTerminalOrigem = cargaPedido.Pedido.TerminalOrigem?.Codigo ?? 0; codigoTerminalDestino = transbordo.Terminal?.Codigo ?? 0;

                    mdfesManual = repCargaMDFeManual.BuscarPorPortosTerminais(codigoViagem, codigoPortoOrigem, codigoPortoDestino, codigoTerminalOrigem, codigoTerminalDestino);
                    if (mdfesManual != null && mdfesManual.Count > 0)
                        carga.MDFeAquaviarioGeradoMasNaoVinculado = mdfesManual.Any(c => c.MDFeManualMDFes.Any(p => p.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado));
                }
            }

            repCarga.Atualizar(carga);
        }

        public void AtualizarMDFeAquaviarioGeradoMasNaoVinculado(int codigoMDFeManual, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMDFeManual repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual = repCargaMDFeManual.BuscarPorCodigo(codigoMDFeManual);

            if (cargaMDFeManual == null)
                return;

            int codigoViagem = cargaMDFeManual.PedidoViagemNavio?.Codigo ?? 0, codigoPortoOrigem = cargaMDFeManual.PortoOrigem?.Codigo ?? 0, codigoPortoDestino = cargaMDFeManual.PortoDestino?.Codigo ?? 0, codigoTerminalOrigem = cargaMDFeManual.TerminalCarregamento?.FirstOrDefault()?.Codigo ?? 0, codigoTerminalDestino = cargaMDFeManual.TerminalDescarregamento?.FirstOrDefault()?.Codigo ?? 0;

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidos = repCargaPedido.BuscarPorMDFeAquaviarioGeradoMasNaoVinculado(codigoViagem, codigoPortoOrigem, codigoPortoDestino, codigoTerminalOrigem, codigoTerminalDestino);

            foreach (var cargaPedido in cargasPedidos)
            {
                if (cargaPedido.Carga.MDFeAquaviarioVinculado)
                    return;

                cargaPedido.Carga.MDFeAquaviarioGeradoMasNaoVinculado = false;

                List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual> mdfesManual = repCargaMDFeManual.BuscarPorPortosTerminais(codigoViagem, codigoPortoOrigem, codigoPortoDestino, codigoTerminalOrigem, codigoTerminalDestino);
                if (mdfesManual != null && mdfesManual.Count > 0)
                    cargaPedido.Carga.MDFeAquaviarioGeradoMasNaoVinculado = mdfesManual.Any(c => c.MDFeManualMDFes.Any(p => p.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado));

                if (cargaPedido.Pedido.Transbordos != null && cargaPedido.Pedido.Transbordos.Count > 0)
                {
                    foreach (var transbordo in cargaPedido.Pedido.Transbordos)
                    {
                        codigoViagem = transbordo.PedidoViagemNavio?.Codigo ?? 0; codigoPortoOrigem = cargaPedido.Pedido.Porto?.Codigo ?? 0; codigoPortoDestino = transbordo.Porto?.Codigo ?? 0; codigoTerminalOrigem = cargaPedido.Pedido.TerminalOrigem?.Codigo ?? 0; codigoTerminalDestino = transbordo.Terminal?.Codigo ?? 0;

                        mdfesManual = repCargaMDFeManual.BuscarPorPortosTerminais(codigoViagem, codigoPortoOrigem, codigoPortoDestino, codigoTerminalOrigem, codigoTerminalDestino);
                        if (mdfesManual != null && mdfesManual.Count > 0)
                            cargaPedido.Carga.MDFeAquaviarioGeradoMasNaoVinculado = mdfesManual.Any(c => c.MDFeManualMDFes.Any(p => p.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado));
                    }
                }

                repCarga.Atualizar(cargaPedido.Carga);
            }
        }

        public void ConsultarMDFeAquaviarioJaGeradoPorMDFe(int codigoMDFe, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMDFeManual repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repCargaMDFeManual.BuscarCargas(codigoMDFe);
            if (cargas != null && cargas.Count > 0)
            {
                foreach (var carga in cargas)
                {
                    ConsultarMDFeAquaviarioJaGerado(carga.Codigo, unitOfWork);
                }
            }
            AtualizarMDFeAquaviarioGeradoMasNaoVinculado(codigoMDFe, unitOfWork);

        }

        public void AtualizarDadosMDFeAquaviario(int codigoMDFe, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMDFeManual repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repCargaMDFeManual.BuscarCargas(codigoMDFe);
            if (cargas != null && cargas.Count > 0)
            {
                foreach (var carga in cargas)
                {
                    carga.MDFeAquaviarioVinculado = true;

                    repCarga.Atualizar(carga);

                    serHubCarga.InformarCargaAtualizada(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, unitOfWork.StringConexao);
                }
            }
        }

        public void AtualizarDadosMercanteManifesto(int codigoCTe, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCargaCTe.BuscarCargaPorCTe(codigoCTe);
            if (carga != null)
            {
                carga.TodosCTesComMercante = !repCargaCTe.TodosCTesNaoContemMercante(carga.Codigo);
                carga.TodosCTesComManifesto = !repCargaCTe.TodosCTesNaoContemManifesto(carga.Codigo);

                repCarga.Atualizar(carga);

                serHubCarga.InformarCargaAtualizada(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, unitOfWork.StringConexao);
            }
        }

        public void AtualizarPesos(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (carga.DadosSumarizados == null)
                return;

            Repositorio.Embarcador.Cargas.CargaDadosSumarizados repositorioCargaDadosSumarizados = new Repositorio.Embarcador.Cargas.CargaDadosSumarizados(unitOfWork);

            carga.DadosSumarizados.PesoTotal = ObterPesoTotal(carga, cargaPedidos, tipoServicoMultisoftware);
            carga.DadosSumarizados.PesoLiquidoTotal = (from o in cargaPedidos select o.PesoLiquido).Sum();
            carga.DadosSumarizados.PesoTotalReentrega = (from o in cargaPedidos where o.ReentregaSolicitada select o.Peso).Sum();

            repositorioCargaDadosSumarizados.Atualizar(carga.DadosSumarizados);
        }

        public void AtualizarDadosCorreios(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            if (carga.DadosSumarizados != null)
            {
                Repositorio.Embarcador.Cargas.CargaDadosSumarizados repCargaDadosSumarizados = new Repositorio.Embarcador.Cargas.CargaDadosSumarizados(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);
                var listaNumeroEtiquetasCorreios = (from obj in cargaPedidos select obj.Pedido.NumeroEtiquetaCorreios).ToList();
                listaNumeroEtiquetasCorreios.RemoveAll(item => item == null);

                var listaPLPsCorreios = (from obj in cargaPedidos select obj.Pedido.PLPCorreios).ToList();
                listaPLPsCorreios.RemoveAll(item => item == null);

                carga.DadosSumarizados.NumerosEtiquetasCorreios = string.Join(", ", listaNumeroEtiquetasCorreios);
                carga.DadosSumarizados.PLPsCorreios = string.Join(", ", listaPLPsCorreios);
                repCargaDadosSumarizados.Atualizar(carga.DadosSumarizados);
            }
        }

        public void AtualizarInformacaoFormaAssociacaoNF(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosBase, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasCarga = repXMLNotaFiscal.BuscarPorCarga(carga.Codigo);
            if (notasCarga?.Count > 0)
            {
                bool contemManual = notasCarga.Any(c => c.FormaIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaIntegracao.Manual || c.FormaIntegracao == null);
                bool contemOKColeta = notasCarga.Any(c => c.FormaIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaIntegracao.OKColeta);
                bool contemClienteFTPOKColeta = notasCarga.Any(c => c.FormaIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaIntegracao.ClienteFTPOKColeta);
                bool contemClienteFTP = notasCarga.Any(c => c.FormaIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaIntegracao.ClienteFTP);
                bool contemOKColetaManual = notasCarga.Any(c => c.FormaIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaIntegracao.OKColetaManual);
                bool contemClienteFTPManual = notasCarga.Any(c => c.FormaIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaIntegracao.ClienteFTPManual);

                if (contemManual && !contemOKColeta && (contemClienteFTPOKColeta || contemClienteFTP || contemClienteFTPManual) && !contemOKColetaManual)
                    carga.FormaIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaIntegracao.ClienteFTPManual;
                else if (contemManual && (contemOKColeta || contemClienteFTPOKColeta || contemOKColetaManual) && !contemClienteFTP && !contemClienteFTPManual)
                    carga.FormaIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaIntegracao.OKColetaManual;
                else if (!contemManual && (contemOKColeta || contemClienteFTPOKColeta) && !contemClienteFTP && !contemOKColetaManual && contemClienteFTPManual)
                    carga.FormaIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaIntegracao.ClienteFTPOKColeta;
                else if (!contemManual && (contemOKColeta || contemClienteFTPOKColeta) && contemClienteFTP && !contemOKColetaManual && !contemClienteFTPManual)
                    carga.FormaIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaIntegracao.ClienteFTPOKColeta;
                else if (contemManual && contemOKColeta && !contemClienteFTPOKColeta && !contemClienteFTP && !contemOKColetaManual && !contemClienteFTPManual)
                    carga.FormaIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaIntegracao.OKColetaManual;
                else if (contemManual && !contemOKColeta && !contemClienteFTPOKColeta && !contemClienteFTP && !contemOKColetaManual && !contemClienteFTPManual)
                    carga.FormaIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaIntegracao.Manual;
                else if (!contemManual && contemOKColeta && !contemClienteFTPOKColeta && !contemClienteFTP && !contemOKColetaManual && !contemClienteFTPManual)
                    carga.FormaIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaIntegracao.OKColeta;
                else if (!contemManual && !contemOKColeta && contemClienteFTPOKColeta && !contemClienteFTP && !contemOKColetaManual && !contemClienteFTPManual)
                    carga.FormaIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaIntegracao.ClienteFTPOKColeta;
                else if (!contemManual && !contemOKColeta && !contemClienteFTPOKColeta && contemClienteFTP && !contemOKColetaManual && !contemClienteFTPManual)
                    carga.FormaIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaIntegracao.ClienteFTP;
                else if (!contemManual && !contemOKColeta && !contemClienteFTPOKColeta && !contemClienteFTP && contemOKColetaManual && !contemClienteFTPManual)
                    carga.FormaIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaIntegracao.OKColetaManual;
                else if (!contemManual && !contemOKColeta && !contemClienteFTPOKColeta && !contemClienteFTP && !contemOKColetaManual && contemClienteFTPManual)
                    carga.FormaIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaIntegracao.ClienteFTPManual;
                else if (!contemManual && !contemOKColeta && !contemClienteFTPOKColeta && (contemClienteFTP || contemClienteFTPManual) && !contemOKColetaManual)
                    carga.FormaIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaIntegracao.ClienteFTPManual;
                else if (!contemManual && contemOKColeta && contemClienteFTPOKColeta && !contemClienteFTP && !contemOKColetaManual && !contemClienteFTPManual)
                    carga.FormaIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaIntegracao.ClienteFTPOKColeta;
                else
                    carga.FormaIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaIntegracao.Manual;

                repCarga.Atualizar(carga);
            }
        }

        public async Task AtualizarInformacaoFormaAssociacaoNFAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosBase, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

            List<FormaIntegracao?> notasCarga = await repositorioXMLNotaFiscal.BuscarFormaIntegracaoPorCargaAsync(carga.Codigo);

            if (notasCarga?.Count > 0)
            {
                bool contemManual = notasCarga.Any(c => c == Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaIntegracao.Manual || c == null);
                bool contemOKColeta = notasCarga.Any(c => c == Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaIntegracao.OKColeta);
                bool contemClienteFTPOKColeta = notasCarga.Any(c => c == Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaIntegracao.ClienteFTPOKColeta);
                bool contemClienteFTP = notasCarga.Any(c => c == Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaIntegracao.ClienteFTP);
                bool contemOKColetaManual = notasCarga.Any(c => c == Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaIntegracao.OKColetaManual);
                bool contemClienteFTPManual = notasCarga.Any(c => c == Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaIntegracao.ClienteFTPManual);

                if (contemManual)
                {
                    if (!contemOKColeta && (contemClienteFTPOKColeta || contemClienteFTP || contemClienteFTPManual) && !contemOKColetaManual)
                        carga.FormaIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaIntegracao.ClienteFTPManual;
                    else if ((contemOKColeta || contemClienteFTPOKColeta || contemOKColetaManual) && !contemClienteFTP && !contemClienteFTPManual)
                        carga.FormaIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaIntegracao.OKColetaManual;
                    else if (contemOKColeta && !contemClienteFTPOKColeta && !contemClienteFTP && !contemOKColetaManual && !contemClienteFTPManual)
                        carga.FormaIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaIntegracao.OKColetaManual;
                    else if (!contemOKColeta && !contemClienteFTPOKColeta && !contemClienteFTP && !contemOKColetaManual && !contemClienteFTPManual)
                        carga.FormaIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaIntegracao.Manual;
                }
                else
                {
                    if ((contemOKColeta || contemClienteFTPOKColeta) && !contemClienteFTP && !contemOKColetaManual && contemClienteFTPManual)
                        carga.FormaIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaIntegracao.ClienteFTPOKColeta;
                    else if ((contemOKColeta || contemClienteFTPOKColeta) && contemClienteFTP && !contemOKColetaManual && !contemClienteFTPManual)
                        carga.FormaIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaIntegracao.ClienteFTPOKColeta;
                    else if (contemOKColeta && !contemClienteFTPOKColeta && !contemClienteFTP && !contemOKColetaManual && !contemClienteFTPManual)
                        carga.FormaIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaIntegracao.OKColeta;
                    else if (!contemOKColeta && contemClienteFTPOKColeta && !contemClienteFTP && !contemOKColetaManual && !contemClienteFTPManual)
                        carga.FormaIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaIntegracao.ClienteFTPOKColeta;
                    else if (!contemOKColeta && !contemClienteFTPOKColeta && contemClienteFTP && !contemOKColetaManual && !contemClienteFTPManual)
                        carga.FormaIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaIntegracao.ClienteFTP;
                    else if (!contemOKColeta && !contemClienteFTPOKColeta && !contemClienteFTP && contemOKColetaManual && !contemClienteFTPManual)
                        carga.FormaIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaIntegracao.OKColetaManual;
                    else if (!contemOKColeta && !contemClienteFTPOKColeta && !contemClienteFTP && !contemOKColetaManual && contemClienteFTPManual)
                        carga.FormaIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaIntegracao.ClienteFTPManual;
                    else if (!contemOKColeta && !contemClienteFTPOKColeta && (contemClienteFTP || contemClienteFTPManual) && !contemOKColetaManual)
                        carga.FormaIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaIntegracao.ClienteFTPManual;
                    else if (contemOKColeta && contemClienteFTPOKColeta && !contemClienteFTP && !contemOKColetaManual && !contemClienteFTPManual)
                        carga.FormaIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaIntegracao.ClienteFTPOKColeta;
                    else
                        carga.FormaIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaIntegracao.Manual;
                }

                await repositorioCarga.AtualizarAsync(carga);
            }
        }

        public void AtualizarOrigensEDestinos(Dominio.Entidades.Embarcador.Cargas.CargaDadosSumarizados dadosSumarizados, Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosBase, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.Embarcador.Carga.CargaFronteira servicoCargaFronteira = new Servicos.Embarcador.Carga.CargaFronteira(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaLocaisPrestacao repositorioCargaLocaisPrestacao = new Repositorio.Embarcador.Cargas.CargaLocaisPrestacao(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = (from o in cargaPedidosBase where !o.PedidoPallet && o.Pedido.TipoPedido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedido.Coleta select o).ToList();
            List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao> cargaLocaisPrestacao = repositorioCargaLocaisPrestacao.BuscarPorCarga(carga.Codigo);
            List<Dominio.ObjetosDeValor.Localidade> destinos = ObterDestinos(cargaPedidos, tipoServicoMultisoftware, carga);
            List<Dominio.ObjetosDeValor.Localidade> origens = ObterOrigens(cargaPedidos, tipoServicoMultisoftware);
            List<Dominio.Entidades.Embarcador.Cargas.CargaFronteira> fronteiras = servicoCargaFronteira.ObterFronteirasPorCarga(carga);

            if (destinos.Count <= 0 && cargaLocaisPrestacao.Count > 0)
                destinos = ObterDestinos(cargaLocaisPrestacao, tipoServicoMultisoftware);

            if (origens.Count <= 0 && cargaLocaisPrestacao.Count > 0)
                origens = ObterOrigens(cargaLocaisPrestacao, tipoServicoMultisoftware);

            dadosSumarizados.Destinos = ObterDescricaoDestinos(destinos, carga, fronteiras);
            dadosSumarizados.PaisDestinos = Utilidades.String.Left(ObterDescricaoPaisLocalidades(destinos), 300);
            dadosSumarizados.UFDestinos = Utilidades.String.Left(ObterDescricaoUFLocalidades(destinos), 60);

            dadosSumarizados.Origens = ObterDescricaoOrigens(origens);
            dadosSumarizados.PaisOrigens = Utilidades.String.Left(ObterDescricaoPaisLocalidades(origens), 300);
            dadosSumarizados.UFOrigens = Utilidades.String.Left(ObterDescricaoUFLocalidades(origens), 60);
        }

        public async Task AtualizarOrigensEDestinosAsync(Dominio.Entidades.Embarcador.Cargas.CargaDadosSumarizados dadosSumarizados, Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosBase, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.Embarcador.Carga.CargaFronteira servicoCargaFronteira = new Servicos.Embarcador.Carga.CargaFronteira(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaLocaisPrestacao repositorioCargaLocaisPrestacao = new Repositorio.Embarcador.Cargas.CargaLocaisPrestacao(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = (from o in cargaPedidosBase where !o.PedidoPallet && o.Pedido.TipoPedido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedido.Coleta select o).ToList();
            List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao> cargaLocaisPrestacao = await repositorioCargaLocaisPrestacao.BuscarPorCargaAsync(carga.Codigo);
            List<Dominio.ObjetosDeValor.Localidade> destinos = ObterDestinos(cargaPedidos, tipoServicoMultisoftware, carga);
            List<Dominio.ObjetosDeValor.Localidade> origens = ObterOrigens(cargaPedidos, tipoServicoMultisoftware);
            List<Dominio.Entidades.Embarcador.Cargas.CargaFronteira> fronteiras = await servicoCargaFronteira.ObterFronteirasPorCargaAsync(carga);

            if (destinos.Count <= 0 && cargaLocaisPrestacao.Count > 0)
                destinos = ObterDestinos(cargaLocaisPrestacao, tipoServicoMultisoftware);

            if (origens.Count <= 0 && cargaLocaisPrestacao.Count > 0)
                origens = ObterOrigens(cargaLocaisPrestacao, tipoServicoMultisoftware);

            dadosSumarizados.Destinos = ObterDescricaoDestinos(destinos, carga, fronteiras);
            dadosSumarizados.PaisDestinos = Utilidades.String.Left(ObterDescricaoPaisLocalidades(destinos), 300);
            dadosSumarizados.UFDestinos = Utilidades.String.Left(ObterDescricaoUFLocalidades(destinos), 60);

            dadosSumarizados.Origens = ObterDescricaoOrigens(origens);
            dadosSumarizados.PaisOrigens = Utilidades.String.Left(ObterDescricaoPaisLocalidades(origens), 300);
            dadosSumarizados.UFOrigens = Utilidades.String.Left(ObterDescricaoUFLocalidades(origens), 60);
        }

        public void AlterarDadosSumarizadosCarga(ref Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(carga.Codigo);

            AlterarDadosSumarizadosCarga(ref carga, cargaPedidos, configuracaoTMS, unitOfWork, tipoServicoMultisoftware);
        }

        public async Task AlterarDadosSumarizadosCargaAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (!(cargaPedidos?.Count > 0))
            {
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                cargaPedidos = await repositorioCargaPedido.BuscarPorCargaAsync(carga.Codigo);
            }

            await AlterarDadosSumarizadosCargaPadraoAsync(carga, cargaPedidos, configuracaoTMS, unitOfWork, tipoServicoMultisoftware);
        }

        public void AlterarDadosSumarizadosCarga(ref Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosBase, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (carga == null)
                return;

            if (carga.CargaAgrupamento != null)
                return;

            Servicos.Log.TratarErro($"Alterando AlterarDadosSumarizadosCarga, referente a carga código: {carga.CodigoCargaEmbarcador}", "GerarCargaPorDocumentoTransporte_Rastreio");

            Repositorio.Embarcador.Cargas.CargaDadosSumarizados repCargaDadosSumarizados = new Repositorio.Embarcador.Cargas.CargaDadosSumarizados(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPercurso repCargaPercurso = new Repositorio.Embarcador.Cargas.CargaPercurso(unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);
            Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao repApoliceSeguroAverbacao = new Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Logistica.AgendamentoColeta repAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoAdicional repPedidoAdicional = new Repositorio.Embarcador.Pedidos.PedidoAdicional(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repositorioCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoAgendamentoColeta repositorioConfiguracaoAgendamentoColeta = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAgendamentoColeta(unitOfWork);

            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoCarga = repositorioConfiguracaoCarga.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAgendamentoColeta configuracaoAgendamentoColeta = repositorioConfiguracaoAgendamentoColeta.BuscarPrimeiroRegistro();

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.ClassificacaoNFe? classificacaoNotaDesconsiderar = null;
            if ((carga.TipoOperacao?.ConfiguracaoEmissaoDocumento?.ClassificacaoNFeRemessaVenda ?? false) && cargaPedidosBase.Exists(o => o.IndicadorRemessaVenda))
            {
                if (carga.TipoOperacao.ConfiguracaoEmissaoDocumento?.EnviarParaObservacaoCTeNFeRemessa ?? false)
                    classificacaoNotaDesconsiderar = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ClassificacaoNFe.Remessa;
                else if (carga.TipoOperacao.ConfiguracaoEmissaoDocumento?.EnviarParaObservacaoCTeNFeVenda ?? false)
                    classificacaoNotaDesconsiderar = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ClassificacaoNFe.Venda;

                cargaPedidosBase = (from obj in cargaPedidosBase
                                    where obj.NotasFiscais.Any(o => o.XMLNotaFiscal.ClassificacaoNFe != classificacaoNotaDesconsiderar)
                                    select obj).ToList();
            }

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = (from obj in cargaPedidosBase where !obj.PedidoPallet && obj.Pedido.TipoPedido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedido.Coleta select obj).ToList();
            Dominio.Entidades.Embarcador.Cargas.CargaDadosSumarizados dadosSumarizados = carga.DadosSumarizados ?? new Dominio.Entidades.Embarcador.Cargas.CargaDadosSumarizados();
            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = carga.Carregamento;
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = cargaPedidos.Select(o => o.Pedido).ToList();
            List<Dominio.Entidades.Cliente> clientesDestinatarios = new List<Dominio.Entidades.Cliente>();
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional> pedidoAdicional = repPedidoAdicional.BuscarPorPedidos(cargaPedidos.Select(o => o.Pedido.Codigo).ToList());

            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                clientesDestinatarios = ObterDestinatarios(cargaPedidos, unitOfWork);

            bool carregamentoComHorarioFixo = PossuiRestricaoHorarioCarregamento(clientesDestinatarios, unitOfWork);
            List<string> codigosIntegracaoDestinatario = (from obj in clientesDestinatarios where (obj.CodigoIntegracao != null && obj.CodigoIntegracao != "") select obj.CodigoIntegracao).ToList();

            List<string> destinatarios = new List<string>();

            if (configuracaoAgendamentoColeta.UtilizaRazaoSocialNaVisaoDoAgendamento)
            {
                destinatarios = (from obj in cargaPedidos where obj.Pedido.Destinatario != null select obj.Pedido.Destinatario.Descricao).Distinct().ToList();
            }
            else
            {
                destinatarios = (from obj in cargaPedidos where obj.Pedido.Destinatario != null && obj.Pedido.Destinatario.GrupoPessoas == null select obj.Pedido.Destinatario.Descricao).Distinct().ToList();
                destinatarios.AddRange((from obj in cargaPedidos where obj.Pedido.Destinatario != null && obj.Pedido.Destinatario.GrupoPessoas != null select obj.Pedido.Destinatario.GrupoPessoas.Descricao).Distinct());

            }

            List<Dominio.Entidades.Cliente> clientesRemetentes = (from obj in cargaPedidos where obj.Pedido.Remetente != null select obj.Pedido.Remetente).Distinct().ToList();
            List<Dominio.Entidades.Cliente> provedoresOS = (from obj in cargaPedidos where obj.Pedido.ProvedorOS != null select obj.Pedido.ProvedorOS).Distinct().ToList();
            List<string> codigosIntegracaoRemetente = (from obj in clientesRemetentes where (obj.CodigoIntegracao != null && obj.CodigoIntegracao != "") select obj.CodigoIntegracao).ToList();
            List<string> remetentes = (from obj in cargaPedidos where obj.Pedido.Remetente != null && obj.Pedido.Remetente.GrupoPessoas == null select obj.Pedido.Remetente.Descricao).Distinct().ToList();
            remetentes.AddRange((from obj in cargaPedidos where obj.Pedido.Remetente != null && obj.Pedido.Remetente.GrupoPessoas != null select obj.Pedido.Remetente.GrupoPessoas.Descricao).Distinct());

            if (remetentes.Count == 0)
                remetentes.AddRange((from obj in cargaPedidos where obj.Pedido.GrupoPessoas != null select obj.Pedido.GrupoPessoas.Descricao).Distinct());

            List<string> codigosIntegracaoExpedidores = (from obj in cargaPedidos where obj.Expedidor != null && (obj.Expedidor.CodigoIntegracao != null && obj.Expedidor.CodigoIntegracao != "") select obj.Expedidor.CodigoIntegracao).Distinct().ToList();
            List<string> expedidores = (from obj in cargaPedidos where obj.Expedidor != null && obj.Expedidor.GrupoPessoas == null select obj.Expedidor.Descricao).Distinct().ToList();
            expedidores.AddRange((from obj in cargaPedidos where obj.Expedidor != null && obj.Expedidor.GrupoPessoas != null select obj.Expedidor.GrupoPessoas.Descricao).Distinct());

            Servicos.Log.TratarErro($"Adicionado o seguintes expedidores: {string.Join(" / ", expedidores)} AlterarDadosSumarizadosCarga, referente a carga código: {carga.CodigoCargaEmbarcador}", "GerarCargaPorDocumentoTransporte_Rastreio");

            List<string> codigosIntegracaoRecebedores = (from obj in cargaPedidos where obj.Recebedor != null && (obj.Recebedor.CodigoIntegracao != null && obj.Recebedor.CodigoIntegracao != "") select obj.Recebedor.CodigoIntegracao).Distinct().ToList();

            List<string> recebedores = new List<string>();
            //Braveo, carregamento segunda perna.... origem recebedor até o destinatário..
            bool carregamentoSegundoTrecho = (carga?.Carregamento?.SessaoRoteirizador?.RoteirizacaoRedespacho ?? false);
            if (!carregamentoSegundoTrecho)
            {
                recebedores = (from obj in cargaPedidos where obj.Recebedor != null && obj.Recebedor.GrupoPessoas == null select obj.Recebedor.Descricao).Distinct().ToList();
                recebedores.AddRange((from obj in cargaPedidos where obj.Recebedor != null && obj.Recebedor.GrupoPessoas != null select obj.Recebedor.GrupoPessoas.Descricao).Distinct());

                Servicos.Log.TratarErro($"Adicionado o seguintes recebedores: {string.Join(" / ", recebedores)} AlterarDadosSumarizadosCarga, referente a carga código: {carga.CodigoCargaEmbarcador}", "GerarCargaPorDocumentoTransporte_Rastreio");
            }

            IEnumerable<string> destinatariosReais = (
                from obj in cargaPedidos where obj.Pedido.Destinatario != null select obj.Pedido.Destinatario.Descricao).ToList().Concat((
                from obj in cargaPedidos where obj.Recebedor != null select obj.Recebedor.Descricao).ToList()
            ).Distinct();

            IEnumerable<string> remetentesReais = (
                from obj in cargaPedidos where obj.Pedido.Remetente != null select obj.Pedido.Remetente.Descricao).ToList().Concat((
                from obj in cargaPedidos where obj.Expedidor != null select obj.Expedidor.Descricao).ToList()
            ).Distinct();

            List<string> codigosIntegracaoTomadores = (from obj in cargaPedidos where obj.ObterTomador() != null && (obj.ObterTomador().CodigoIntegracao != null && obj.ObterTomador().CodigoIntegracao != "") select obj.ObterTomador().CodigoIntegracao).Distinct().ToList();

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaPedidos.FirstOrDefault()?.Pedido;
            Dominio.Entidades.Cliente tomador = cargaPedidos.FirstOrDefault()?.ObterTomador();

            dadosSumarizados.ObservacaoEmissaoCarga = tomador?.GrupoPessoas?.ObservacaoEmissaoCarga ?? string.Empty;
            dadosSumarizados.ObservacaoEmissaoCargaTomador = tomador?.ObservacaoEmissaoCarga ?? string.Empty;
            dadosSumarizados.ObservacaoEmissaoCargaTipoOperacao = carga.TipoOperacao?.ObservacaoEmissaoCarga ?? string.Empty;

            AtualizarOrigensEDestinos(dadosSumarizados, carga, cargaPedidos, unitOfWork, tipoServicoMultisoftware);
            if (configuracaoTMS.UtilizaEmissaoMultimodal)
                AtualizarInformacaoFormaAssociacaoNF(carga, cargaPedidos, unitOfWork, tipoServicoMultisoftware);

            if (dadosSumarizados.ClientesDestinatarios != null)
                dadosSumarizados.ClientesDestinatarios.Clear();

            dadosSumarizados.ClientesDestinatarios = clientesDestinatarios;

            if (dadosSumarizados.ClientesRemetentes != null)
                dadosSumarizados.ClientesRemetentes.Clear();

            dadosSumarizados.ClientesRemetentes = clientesRemetentes;
            dadosSumarizados.CodigoIntegracaoDestinatarios = string.Join(", ", codigosIntegracaoDestinatario);
            dadosSumarizados.CodigoIntegracaoRemetentes = string.Join(", ", codigosIntegracaoRemetente);
            dadosSumarizados.CodigoIntegracaoExpedidores = string.Join(", ", codigosIntegracaoExpedidores);
            dadosSumarizados.CodigoIntegracaoRecebedores = string.Join(", ", codigosIntegracaoRecebedores);
            dadosSumarizados.ProvedoresOS = string.Join(", ", provedoresOS.Select(obj => obj.NomeFantasia).ToList());
            dadosSumarizados.ZonasTransporte = string.Join(", ", pedidoAdicional.Where(o => o.ZonaTransporte != null).Select(obj => obj.ZonaTransporte.Descricao).Distinct().ToList());

            if (carga.CargaAgrupada)
            {
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasOriginais = repCarga.BuscarCargasOriginais(carga.Codigo);
                dadosSumarizados.Filiais = string.Join(", ", (from obj in cargasOriginais orderby obj.Codigo where obj.Filial != null select obj.Filial.Descricao).Distinct().ToList());
                dadosSumarizados.TiposDeOperacao = string.Join(", ", (from obj in cargasOriginais where obj.TipoOperacao != null select obj.TipoOperacao.Descricao).Distinct().ToList());
                dadosSumarizados.UtilizarCTesAnterioresComoCTeFilialEmissora = cargasOriginais.Exists(o => o.Filial != null && o.Filial.UtilizarCtesAnterioresComoCteFilialEmissora);
            }
            else
            {
                dadosSumarizados.Filiais = carga.Filial?.Descricao ?? "";
                dadosSumarizados.TiposDeOperacao = carga.TipoOperacao?.Descricao ?? "";
                dadosSumarizados.UtilizarCTesAnterioresComoCTeFilialEmissora = carga.Filial?.UtilizarCtesAnterioresComoCteFilialEmissora ?? false;
            }

            Servicos.Embarcador.Carga.CargaFronteira serCargaFronteira = new Servicos.Embarcador.Carga.CargaFronteira(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaFronteira> fronteiras = serCargaFronteira.ObterFronteirasPorCarga(carga);

            dadosSumarizados.Veiculos = Utilidades.String.Left(carga.PlacasVeiculos, 100);
            dadosSumarizados.Motoristas = Utilidades.String.Left(carga.NomeMotoristas, 300);
            dadosSumarizados.Destinatarios = destinatarios.Count > 0 ? string.Join(" / ", destinatarios) : "Não Informado";
            dadosSumarizados.DestinatariosReais = string.Join(" / ", destinatariosReais);
            dadosSumarizados.Remetentes = string.Join(" / ", remetentes);
            dadosSumarizados.RemetentesReais = string.Join(" / ", remetentesReais);
            dadosSumarizados.Expedidores = string.Join(" / ", expedidores);
            dadosSumarizados.Recebedores = string.Join(" / ", recebedores);
            dadosSumarizados.NumeroColetas = ObterNumeroColetasPorPedido(cargaPedidos);
            dadosSumarizados.NumeroEntregas = BuscarNumeroDeEntregasPorPedido(cargaPedidos, unitOfWork);
            dadosSumarizados.NumeroEntregasFinais = pedido?.NumeroEntregasFinais ?? 0;
            dadosSumarizados.Distancia = servicoCarga.ObterDistancia(carga, configuracaoTMS, unitOfWork);
            dadosSumarizados.PossuiCTeAnteriorFilialEmissora = cargaPedidos.Any(o => o.CargaPedidoFilialEmissora && o.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Normal);
            dadosSumarizados.PossuiNFS = cargaPedidos.Any(o => o.PossuiNFS);
            dadosSumarizados.PossuiCTe = cargaPedidos.Any(o => o.PossuiCTe);
            dadosSumarizados.PossuiNFSManual = cargaPedidos.Any(o => o.PossuiNFSManual || o.ValorFreteResidual > 0);
            dadosSumarizados.PossuiRedespacho = cargaPedidos.Any(o => o.Redespacho);
            dadosSumarizados.RotaEmbarcador = string.Join(", ", (from obj in cargaPedidos where !string.IsNullOrWhiteSpace(obj.Pedido.RotaEmbarcador) select obj.Pedido.RotaEmbarcador).Distinct());
            dadosSumarizados.DataUltimaLiberacao = string.Join(", ", (from obj in cargaPedidos where obj.Pedido.DataUltimaLiberacao.HasValue select obj.Pedido.DataUltimaLiberacao?.ToString("dd/MM/yyyy HH:mm")).Distinct());
            dadosSumarizados.UsuarioCriacaoRemessa = string.Join(", ", (from obj in cargaPedidos where !string.IsNullOrWhiteSpace(obj.Pedido.UsuarioCriacaoRemessa) select obj.Pedido.UsuarioCriacaoRemessa).Distinct());
            dadosSumarizados.NumeroOrdem = string.Join(", ", (from obj in cargaPedidos where !string.IsNullOrWhiteSpace(obj.Pedido.NumeroOrdem) select obj.Pedido.NumeroOrdem).Distinct());
            dadosSumarizados.RegiaoDestino = string.Join(", ", (from obj in cargaPedidos where obj.Pedido.RegiaoDestino != null select obj.Pedido.RegiaoDestino.Descricao).Distinct());
            dadosSumarizados.Reentrega = cargaPedidos.Any(o => o.ReentregaSolicitada);
            dadosSumarizados.DiasItinerario = pedido?.DiasItinerario ?? 0;
            dadosSumarizados.PossuiAntecipacaoICMS = cargaPedidos.Any(o => o.Pedido.AntecipacaoICMS);
            dadosSumarizados.PortalRetiraEmpresa = Utilidades.String.Left(carregamento?.NomeTransportadora, 200);
            dadosSumarizados.CodigoPedidoCliente = Utilidades.String.Left(string.Join(", ", (from obj in cargaPedidos where !string.IsNullOrWhiteSpace(obj.Pedido.CodigoPedidoCliente) select obj.Pedido.CodigoPedidoCliente).Distinct()), 100);
            dadosSumarizados.HorarioFixoCarregamento = carregamentoComHorarioFixo;
            dadosSumarizados.DataPrevisaoEntrega = null;
            dadosSumarizados.DataPrevisaoSaida = null;

            if (pedidoAdicional.Any(o => o.ExecaoCab.HasValue))
                dadosSumarizados.ExcecaoCab = pedidoAdicional.Where(o => o.ExecaoCab.HasValue).FirstOrDefault().ExecaoCab.Value;

            if (cargaPedidos.Any(obj => obj.Pedido.PrevisaoEntrega.HasValue))
                dadosSumarizados.DataPrevisaoEntrega = cargaPedidos.Where(obj => obj.Pedido.PrevisaoEntrega.HasValue).Min(obj => obj.Pedido.PrevisaoEntrega.Value);

            if (cargaPedidos.Any(obj => obj.Pedido.DataPrevisaoSaida.HasValue))
                dadosSumarizados.DataPrevisaoSaida = cargaPedidos.Where(obj => obj.Pedido.DataPrevisaoSaida.HasValue).Min(obj => obj.Pedido.DataPrevisaoSaida.Value);

            if (!dadosSumarizados.PossuiCTe)
            {
                carga.EmpresaFilialEmissora = null;

                if (carga.CargaAgrupada)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.Carga> cargaOrigemComFilialEmissora = (from obj in cargaPedidos where !obj.CargaPedidoFilialEmissora && obj.CargaOrigem.EmpresaFilialEmissora != null select obj.CargaOrigem).Distinct().ToList();
                    foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem in cargaOrigemComFilialEmissora)
                    {
                        cargaOrigem.EmpresaFilialEmissora = null;
                        repCarga.Atualizar(cargaOrigem);
                    }
                }
            }
            else
            {
                if (cargaPedidos.Any(obj => obj.CargaPedidoFilialEmissora) && carga.EmpresaFilialEmissora == null)
                {
                    Repositorio.Embarcador.Filiais.EstadoDestinoEmpresaEmissora repositorioEstadoDestinoEmpresaEmissora = new Repositorio.Embarcador.Filiais.EstadoDestinoEmpresaEmissora(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Filiais.EstadoDestinoEmpresaEmissora> estadosDestinoEmpresaEmissora = carga.Filial != null ? repositorioEstadoDestinoEmpresaEmissora.BuscarPorFilial(carga.Filial.Codigo) : new List<Dominio.Entidades.Embarcador.Filiais.EstadoDestinoEmpresaEmissora>();

                    Dominio.Entidades.Embarcador.Filiais.EstadoDestinoEmpresaEmissora estadoDestino = estadosDestinoEmpresaEmissora.Find(e => e.Estado.Codigo == cargaPedidos.FirstOrDefault()?.Destino?.Estado.Codigo);

                    if ((estadosDestinoEmpresaEmissora.Count > 0) && estadoDestino != null)
                        carga.EmpresaFilialEmissora = estadoDestino.Empresa;
                    else
                        carga.EmpresaFilialEmissora = carga.Filial?.EmpresaEmissora;
                }

                if (carga.CargaAgrupada)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.Carga> cargaOrigemSemFilialEmissora = (from obj in cargaPedidos where obj.CargaPedidoFilialEmissora && obj.CargaOrigem.EmpresaFilialEmissora == null select obj.CargaOrigem).Distinct().ToList();
                    foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem in cargaOrigemSemFilialEmissora)
                    {
                        cargaOrigem.EmpresaFilialEmissora = carga.EmpresaFilialEmissora;
                        repCarga.Atualizar(cargaOrigem);
                    }
                }
            }

            dadosSumarizados.SubContratantes = ObterSubContratantesCarga(cargaPedidos); //carga.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada ? ObterSubContratantesCarga(cargaPedidos) : "";

            if (repCargaValePedagio.ContarPorCarga(carga.Codigo) > 0)
                dadosSumarizados.PossuiIntegracaoValePedagio = true;
            else
                dadosSumarizados.PossuiIntegracaoValePedagio = false;

            dadosSumarizados.PesoTotal = ObterPesoTotal(carga, cargaPedidosBase, tipoServicoMultisoftware);
            dadosSumarizados.PesoLiquidoTotal = (from o in cargaPedidosBase select o.PesoLiquido).Sum();
            dadosSumarizados.PesoTotalReentrega = (from o in cargaPedidosBase where o.ReentregaSolicitada select o.Peso).Sum();

            if (repApoliceSeguroAverbacao.ContarAverbacoesCarga(carga.Codigo) > 0)
            {
                dadosSumarizados.PossuiAverbacaoCTe = true;
                dadosSumarizados.PossuiAverbacaoMDFe = ((configuracaoTMS.AverbarMDFe && !configuracaoCarga.AverbarMDFeSomenteEmCargasComCIOT) || (configuracaoTMS.AverbarMDFe && configuracaoCarga.AverbarMDFeSomenteEmCargasComCIOT && repositorioCargaCIOT.ExisteCIOTPorCarga(carga.Codigo)));
            }
            else
            {
                dadosSumarizados.PossuiAverbacaoCTe = false;
                dadosSumarizados.PossuiAverbacaoMDFe = false;
            }

            SetarGrupoPrincipalCarga(cargaPedidos, carga, unitOfWork);
            VerificarDestinatariosFilialDestino(dadosSumarizados.ClientesDestinatarios, carga, unitOfWork);

            dadosSumarizados.Bookings = string.Join(", ", (from obj in cargaPedidos where !string.IsNullOrWhiteSpace(obj.Pedido.NumeroBooking) select obj.Pedido.NumeroBooking).ToList());
            dadosSumarizados.FiliaisVenda = string.Join(", ", (from obj in cargaPedidos where obj.Pedido.FilialVenda != null select obj.Pedido.FilialVenda.Descricao).Distinct().ToList());
            dadosSumarizados.CanalVenda = string.Join(", ", (from obj in cargaPedidos where obj.Pedido.CanalVenda != null select obj.Pedido.CanalVenda.Descricao).Distinct().ToList());
            dadosSumarizados.VolumesTotal = (from obj in cargaPedidosBase select obj.Pedido.QtVolumes).Sum();
            dadosSumarizados.CubagemTotal = (from obj in cargaPedidosBase select obj.Pedido.CubagemTotal).Sum();
            dadosSumarizados.Onda = Utilidades.String.Left(pedido?.Onda ?? "", 150);
            dadosSumarizados.ClusterRota = Utilidades.String.Left(pedido?.ClusterRota ?? "", 150);
            dadosSumarizados.DataPrevisaoInicioViagem = pedido?.DataPrevisaoInicioViagem;
            dadosSumarizados.ValorTotalProdutos = repPedidoXMLNotaFiscal.ObterValorTotalProdutosPorCarga(carga.Codigo, configuracaoTMS.NaoUsarPesoNotasPallet, classificacaoNotaDesconsiderar);

            if (carga.Internacional)
                dadosSumarizados.QuantidadeVolumesNF = repPedidoXMLNotaFiscal.BuscarVolumesPorCargaETipoFatura(carga.Codigo, classificacaoNotaDesconsiderar);
            else
                dadosSumarizados.QuantidadeVolumesNF = repPedidoXMLNotaFiscal.BuscarVolumesPorCarga(carga.Codigo, classificacaoNotaDesconsiderar);

            dadosSumarizados.LocalParqueamento = Utilidades.String.Left(ObterLocalParqueamento(pedido?.LocalParqueamento), 300);
            dadosSumarizados.NumeroPedidoEmbarcador = Utilidades.String.Left(string.Join(", ", (from obj in cargaPedidos select obj.Pedido.NumeroPedidoEmbarcador).Distinct()), 400);
            dadosSumarizados.ValorTotalMercadoriaPedidos = (from obj in cargaPedidosBase select obj.Pedido.ValorTotalNotasFiscais).Sum();

            Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamento = repAgendamentoColeta.BuscarPorCarga(carga.Codigo);
            if (agendamento != null)
            {
                dadosSumarizados.QuantidadeVolumes = agendamento.Volumes;
                dadosSumarizados.PossuiProdutoPerigoso = agendamento.CargaPerigosa;
                dadosSumarizados.ValorTotalMercadoriaPedidos = agendamento.ValorTotalVolumes;
            }

            dadosSumarizados.CentrosDeDistribuicao = Utilidades.String.Left(string.Join(", ", (from obj in pedidos where obj.CDDestino != null select obj.CDDestino.Descricao).Distinct().ToList()), 400);
            dadosSumarizados.CustoFrete = Utilidades.String.Left(string.Join(", ", (from obj in cargaPedidos where !string.IsNullOrWhiteSpace(obj.CustoFrete) select obj.CustoFrete).Distinct().ToList()), 100);
            dadosSumarizados.NumeroFrotasVeiculos = BuscarNumeroFrotasVeiculos(carga);

            if (dadosSumarizados.Codigo > 0)
                repCargaDadosSumarizados.Atualizar(dadosSumarizados);
            else
            {
                repCargaDadosSumarizados.Inserir(dadosSumarizados);
                carga.DadosSumarizados = dadosSumarizados;
                repCarga.Atualizar(carga);
            }

            Servicos.Log.TratarErro($"Atualizou AlterarDadosSumarizadosCarga, referente a carga código: {carga.CodigoCargaEmbarcador}", "GerarCargaPorDocumentoTransporte_Rastreio");
        }

        public async Task AlterarDadosSumarizadosCargaPadraoAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosBase, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (carga == null)
                return;

            if (carga.CargaAgrupamento != null)
                return;

            Repositorio.Embarcador.Cargas.CargaDadosSumarizados repCargaDadosSumarizados = new Repositorio.Embarcador.Cargas.CargaDadosSumarizados(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);
            Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao repApoliceSeguroAverbacao = new Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Logistica.AgendamentoColeta repAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoAdicional repPedidoAdicional = new Repositorio.Embarcador.Pedidos.PedidoAdicional(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repositorioCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);

            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoCarga = await repositorioConfiguracaoCarga.BuscarPrimeiroRegistroAsync();

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.ClassificacaoNFe? classificacaoNotaDesconsiderar = null;
            if ((carga.TipoOperacao?.ConfiguracaoEmissaoDocumento?.ClassificacaoNFeRemessaVenda ?? false) && cargaPedidosBase.Exists(o => o.IndicadorRemessaVenda))
            {
                if (carga.TipoOperacao.ConfiguracaoEmissaoDocumento?.EnviarParaObservacaoCTeNFeRemessa ?? false)
                    classificacaoNotaDesconsiderar = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ClassificacaoNFe.Remessa;
                else if (carga.TipoOperacao.ConfiguracaoEmissaoDocumento?.EnviarParaObservacaoCTeNFeVenda ?? false)
                    classificacaoNotaDesconsiderar = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ClassificacaoNFe.Venda;

                cargaPedidosBase = (from obj in cargaPedidosBase
                                    where obj.NotasFiscais.Any(o => o.XMLNotaFiscal.ClassificacaoNFe != classificacaoNotaDesconsiderar)
                                    select obj).ToList();
            }

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = (from obj in cargaPedidosBase where !obj.PedidoPallet && obj.Pedido.TipoPedido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedido.Coleta select obj).ToList();
            Dominio.Entidades.Embarcador.Cargas.CargaDadosSumarizados dadosSumarizados = carga.DadosSumarizados ?? new Dominio.Entidades.Embarcador.Cargas.CargaDadosSumarizados();
            Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = carga.Carregamento;
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = cargaPedidos.Select(o => o.Pedido).ToList();
            List<Dominio.Entidades.Cliente> clientesDestinatarios = new List<Dominio.Entidades.Cliente>();
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional> pedidoAdicional = await repPedidoAdicional.BuscarPorPedidosAsync(cargaPedidos.Select(o => o.Pedido.Codigo).ToList());

            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                clientesDestinatarios = await ObterDestinatariosAsync(cargaPedidos, unitOfWork);

            bool carregamentoComHorarioFixo = await PossuiRestricaoHorarioCarregamentoAsync(clientesDestinatarios, unitOfWork);
            List<string> codigosIntegracaoDestinatario = (from obj in clientesDestinatarios where (obj.CodigoIntegracao != null && obj.CodigoIntegracao != "") select obj.CodigoIntegracao).ToList();
            List<string> destinatarios = (from obj in cargaPedidos where obj.Pedido.Destinatario != null && obj.Pedido.Destinatario.GrupoPessoas == null select obj.Pedido.Destinatario.Descricao).Distinct().ToList();
            destinatarios.AddRange((from obj in cargaPedidos where obj.Pedido.Destinatario != null && obj.Pedido.Destinatario.GrupoPessoas != null select obj.Pedido.Destinatario.GrupoPessoas.Descricao).Distinct());

            List<Dominio.Entidades.Cliente> clientesRemetentes = (from obj in cargaPedidos where obj.Pedido.Remetente != null select obj.Pedido.Remetente).Distinct().ToList();
            List<Dominio.Entidades.Cliente> provedoresOS = (from obj in cargaPedidos where obj.Pedido.ProvedorOS != null select obj.Pedido.ProvedorOS).Distinct().ToList();
            List<string> codigosIntegracaoRemetente = (from obj in clientesRemetentes where (obj.CodigoIntegracao != null && obj.CodigoIntegracao != "") select obj.CodigoIntegracao).ToList();
            List<string> remetentes = (from obj in cargaPedidos where obj.Pedido.Remetente != null && obj.Pedido.Remetente.GrupoPessoas == null select obj.Pedido.Remetente.Descricao).Distinct().ToList();
            remetentes.AddRange((from obj in cargaPedidos where obj.Pedido.Remetente != null && obj.Pedido.Remetente.GrupoPessoas != null select obj.Pedido.Remetente.GrupoPessoas.Descricao).Distinct());

            if (remetentes.Count == 0)
                remetentes.AddRange((from obj in cargaPedidos where obj.Pedido.GrupoPessoas != null select obj.Pedido.GrupoPessoas.Descricao).Distinct());

            List<string> codigosIntegracaoExpedidores = (from obj in cargaPedidos where obj.Expedidor != null && (obj.Expedidor.CodigoIntegracao != null && obj.Expedidor.CodigoIntegracao != "") select obj.Expedidor.CodigoIntegracao).Distinct().ToList();
            List<string> expedidores = (from obj in cargaPedidos where obj.Expedidor != null && obj.Expedidor.GrupoPessoas == null select obj.Expedidor.Descricao).Distinct().ToList();
            expedidores.AddRange((from obj in cargaPedidos where obj.Expedidor != null && obj.Expedidor.GrupoPessoas != null select obj.Expedidor.GrupoPessoas.Descricao).Distinct());

            List<string> codigosIntegracaoRecebedores = (from obj in cargaPedidos where obj.Recebedor != null && (obj.Recebedor.CodigoIntegracao != null && obj.Recebedor.CodigoIntegracao != "") select obj.Recebedor.CodigoIntegracao).Distinct().ToList();

            List<string> recebedores = new List<string>();
            //Braveo, carregamento segunda perna.... origem recebedor até o destinatário..
            bool carregamentoSegundoTrecho = (carga?.Carregamento?.SessaoRoteirizador?.RoteirizacaoRedespacho ?? false);
            if (!carregamentoSegundoTrecho)
            {
                recebedores = (from obj in cargaPedidos where obj.Recebedor != null && obj.Recebedor.GrupoPessoas == null select obj.Recebedor.Descricao).Distinct().ToList();
                recebedores.AddRange((from obj in cargaPedidos where obj.Recebedor != null && obj.Recebedor.GrupoPessoas != null select obj.Recebedor.GrupoPessoas.Descricao).Distinct());
            }

            IEnumerable<string> destinatariosReais = (
                from obj in cargaPedidos where obj.Pedido.Destinatario != null select obj.Pedido.Destinatario.Descricao).ToList().Concat((
                from obj in cargaPedidos where obj.Recebedor != null select obj.Recebedor.Descricao).ToList()
            ).Distinct();

            IEnumerable<string> remetentesReais = (
                from obj in cargaPedidos where obj.Pedido.Remetente != null select obj.Pedido.Remetente.Descricao).ToList().Concat((
                from obj in cargaPedidos where obj.Expedidor != null select obj.Expedidor.Descricao).ToList()
            ).Distinct();

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaPedidos.FirstOrDefault()?.Pedido;
            Dominio.Entidades.Cliente tomador = cargaPedidos.FirstOrDefault()?.ObterTomador();

            dadosSumarizados.ObservacaoEmissaoCarga = tomador?.GrupoPessoas?.ObservacaoEmissaoCarga ?? string.Empty;
            dadosSumarizados.ObservacaoEmissaoCargaTomador = tomador?.ObservacaoEmissaoCarga ?? string.Empty;
            dadosSumarizados.ObservacaoEmissaoCargaTipoOperacao = carga.TipoOperacao?.ObservacaoEmissaoCarga ?? string.Empty;

            await AtualizarOrigensEDestinosAsync(dadosSumarizados, carga, cargaPedidos, unitOfWork, tipoServicoMultisoftware);

            if (configuracaoTMS.UtilizaEmissaoMultimodal)
                await AtualizarInformacaoFormaAssociacaoNFAsync(carga, cargaPedidos, unitOfWork, tipoServicoMultisoftware);

            if (dadosSumarizados.ClientesDestinatarios != null)
                dadosSumarizados.ClientesDestinatarios.Clear();

            dadosSumarizados.ClientesDestinatarios = clientesDestinatarios;

            if (dadosSumarizados.ClientesRemetentes != null)
                dadosSumarizados.ClientesRemetentes.Clear();

            dadosSumarizados.ClientesRemetentes = clientesRemetentes;
            dadosSumarizados.CodigoIntegracaoDestinatarios = string.Join(", ", codigosIntegracaoDestinatario);
            dadosSumarizados.CodigoIntegracaoRemetentes = string.Join(", ", codigosIntegracaoRemetente);
            dadosSumarizados.CodigoIntegracaoExpedidores = string.Join(", ", codigosIntegracaoExpedidores);
            dadosSumarizados.CodigoIntegracaoRecebedores = string.Join(", ", codigosIntegracaoRecebedores);
            dadosSumarizados.ProvedoresOS = string.Join(", ", provedoresOS.Select(obj => obj.NomeFantasia).ToList());
            dadosSumarizados.ZonasTransporte = string.Join(", ", pedidoAdicional.Where(o => o.ZonaTransporte != null).Select(obj => obj.ZonaTransporte.Descricao).Distinct().ToList());

            if (carga.CargaAgrupada)
            {
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasOriginais = repCarga.BuscarCargasOriginais(carga.Codigo);
                dadosSumarizados.Filiais = string.Join(", ", (from obj in cargasOriginais orderby obj.Codigo where obj.Filial != null select obj.Filial.Descricao).Distinct().ToList());
                dadosSumarizados.TiposDeOperacao = string.Join(", ", (from obj in cargasOriginais where obj.TipoOperacao != null select obj.TipoOperacao.Descricao).Distinct().ToList());
                dadosSumarizados.UtilizarCTesAnterioresComoCTeFilialEmissora = cargasOriginais.Exists(o => o.Filial != null && o.Filial.UtilizarCtesAnterioresComoCteFilialEmissora);
            }
            else
            {
                dadosSumarizados.Filiais = carga.Filial?.Descricao ?? "";
                dadosSumarizados.TiposDeOperacao = carga.TipoOperacao?.Descricao ?? "";
                dadosSumarizados.UtilizarCTesAnterioresComoCTeFilialEmissora = carga.Filial?.UtilizarCtesAnterioresComoCteFilialEmissora ?? false;
            }

            Servicos.Embarcador.Carga.CargaFronteira serCargaFronteira = new Servicos.Embarcador.Carga.CargaFronteira(unitOfWork);

            dadosSumarizados.Veiculos = Utilidades.String.Left(carga.PlacasVeiculos, 100);
            dadosSumarizados.Motoristas = Utilidades.String.Left(carga.NomeMotoristas, 300);
            dadosSumarizados.Destinatarios = destinatarios.Count > 0 ? string.Join(" / ", destinatarios) : "Não Informado";
            dadosSumarizados.DestinatariosReais = string.Join(" / ", destinatariosReais);
            dadosSumarizados.Remetentes = string.Join(" / ", remetentes);
            dadosSumarizados.RemetentesReais = string.Join(" / ", remetentesReais);
            dadosSumarizados.Expedidores = string.Join(" / ", expedidores);
            dadosSumarizados.Recebedores = string.Join(" / ", recebedores);
            dadosSumarizados.NumeroColetas = ObterNumeroColetasPorPedido(cargaPedidos);
            dadosSumarizados.NumeroEntregas = BuscarNumeroDeEntregasPorPedido(cargaPedidos, unitOfWork);
            dadosSumarizados.NumeroEntregasFinais = pedido?.NumeroEntregasFinais ?? 0;
            dadosSumarizados.Distancia = servicoCarga.ObterDistancia(carga, configuracaoTMS, unitOfWork);
            dadosSumarizados.PossuiCTeAnteriorFilialEmissora = cargaPedidos.Any(o => o.CargaPedidoFilialEmissora && o.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Normal);
            dadosSumarizados.PossuiNFS = cargaPedidos.Any(o => o.PossuiNFS);
            dadosSumarizados.PossuiCTe = cargaPedidos.Any(o => o.PossuiCTe);
            dadosSumarizados.PossuiNFSManual = cargaPedidos.Any(o => o.PossuiNFSManual || o.ValorFreteResidual > 0);
            dadosSumarizados.PossuiRedespacho = cargaPedidos.Any(o => o.Redespacho);
            dadosSumarizados.RotaEmbarcador = string.Join(", ", (from obj in cargaPedidos where !string.IsNullOrWhiteSpace(obj.Pedido.RotaEmbarcador) select obj.Pedido.RotaEmbarcador).Distinct());
            dadosSumarizados.DataUltimaLiberacao = string.Join(", ", (from obj in cargaPedidos where obj.Pedido.DataUltimaLiberacao.HasValue select obj.Pedido.DataUltimaLiberacao?.ToString("dd/MM/yyyy HH:mm")).Distinct());
            dadosSumarizados.UsuarioCriacaoRemessa = string.Join(", ", (from obj in cargaPedidos where !string.IsNullOrWhiteSpace(obj.Pedido.UsuarioCriacaoRemessa) select obj.Pedido.UsuarioCriacaoRemessa).Distinct());
            dadosSumarizados.NumeroOrdem = string.Join(", ", (from obj in cargaPedidos where !string.IsNullOrWhiteSpace(obj.Pedido.NumeroOrdem) select obj.Pedido.NumeroOrdem).Distinct());
            dadosSumarizados.RegiaoDestino = string.Join(", ", (from obj in cargaPedidos where obj.Pedido.RegiaoDestino != null select obj.Pedido.RegiaoDestino.Descricao).Distinct());
            dadosSumarizados.Reentrega = cargaPedidos.Any(o => o.ReentregaSolicitada);
            dadosSumarizados.DiasItinerario = pedido?.DiasItinerario ?? 0;
            dadosSumarizados.PossuiAntecipacaoICMS = cargaPedidos.Any(o => o.Pedido.AntecipacaoICMS);
            dadosSumarizados.PortalRetiraEmpresa = Utilidades.String.Left(carregamento?.NomeTransportadora, 200);
            dadosSumarizados.CodigoPedidoCliente = Utilidades.String.Left(string.Join(", ", (from obj in cargaPedidos where !string.IsNullOrWhiteSpace(obj.Pedido.CodigoPedidoCliente) select obj.Pedido.CodigoPedidoCliente).Distinct()), 100);
            dadosSumarizados.HorarioFixoCarregamento = carregamentoComHorarioFixo;
            dadosSumarizados.DataPrevisaoEntrega = null;
            dadosSumarizados.DataPrevisaoSaida = null;

            if (pedidoAdicional.Any(o => o.ExecaoCab.HasValue))
                dadosSumarizados.ExcecaoCab = pedidoAdicional.FirstOrDefault(o => o.ExecaoCab.HasValue).ExecaoCab.Value;

            if (cargaPedidos.Any(obj => obj.Pedido.PrevisaoEntrega.HasValue))
                dadosSumarizados.DataPrevisaoEntrega = cargaPedidos.Where(obj => obj.Pedido.PrevisaoEntrega.HasValue).Min(obj => obj.Pedido.PrevisaoEntrega.Value);

            if (cargaPedidos.Any(obj => obj.Pedido.DataPrevisaoSaida.HasValue))
                dadosSumarizados.DataPrevisaoSaida = cargaPedidos.Where(obj => obj.Pedido.DataPrevisaoSaida.HasValue).Min(obj => obj.Pedido.DataPrevisaoSaida.Value);

            if (!dadosSumarizados.PossuiCTe)
            {
                carga.EmpresaFilialEmissora = null;

                if (carga.CargaAgrupada)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.Carga> cargaOrigemComFilialEmissora = (from obj in cargaPedidos where !obj.CargaPedidoFilialEmissora && obj.CargaOrigem.EmpresaFilialEmissora != null select obj.CargaOrigem).Distinct().ToList();
                    foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem in cargaOrigemComFilialEmissora)
                    {
                        cargaOrigem.EmpresaFilialEmissora = null;
                        await repCarga.AtualizarAsync(cargaOrigem);
                    }
                }
            }
            else
            {
                if (cargaPedidos.Any(obj => obj.CargaPedidoFilialEmissora) && carga.EmpresaFilialEmissora == null)
                {
                    Repositorio.Embarcador.Filiais.EstadoDestinoEmpresaEmissora repositorioEstadoDestinoEmpresaEmissora = new Repositorio.Embarcador.Filiais.EstadoDestinoEmpresaEmissora(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Filiais.EstadoDestinoEmpresaEmissora> estadosDestinoEmpresaEmissora = carga.Filial != null ? await repositorioEstadoDestinoEmpresaEmissora.BuscarPorFilialAsync(carga.Filial.Codigo) : new List<Dominio.Entidades.Embarcador.Filiais.EstadoDestinoEmpresaEmissora>();

                    Dominio.Entidades.Embarcador.Filiais.EstadoDestinoEmpresaEmissora estadoDestino = estadosDestinoEmpresaEmissora.Find(e => e.Estado.Codigo == cargaPedidos.FirstOrDefault()?.Destino?.Estado.Codigo);

                    if ((estadosDestinoEmpresaEmissora.Count > 0) && estadoDestino != null)
                        carga.EmpresaFilialEmissora = estadoDestino.Empresa;
                    else
                        carga.EmpresaFilialEmissora = carga.Filial?.EmpresaEmissora;
                }

                if (carga.CargaAgrupada)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.Carga> cargaOrigemSemFilialEmissora = (from obj in cargaPedidos where obj.CargaPedidoFilialEmissora && obj.CargaOrigem.EmpresaFilialEmissora == null select obj.CargaOrigem).Distinct().ToList();
                    foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem in cargaOrigemSemFilialEmissora)
                    {
                        cargaOrigem.EmpresaFilialEmissora = carga.EmpresaFilialEmissora;
                        await repCarga.AtualizarAsync(cargaOrigem);
                    }
                }
            }

            dadosSumarizados.SubContratantes = ObterSubContratantesCarga(cargaPedidos);

            if (await repCargaValePedagio.ExistePorCargaAsync(carga.Codigo))
                dadosSumarizados.PossuiIntegracaoValePedagio = true;
            else
                dadosSumarizados.PossuiIntegracaoValePedagio = false;

            dadosSumarizados.PesoTotal = ObterPesoTotal(carga, cargaPedidosBase, tipoServicoMultisoftware);
            dadosSumarizados.PesoLiquidoTotal = (from o in cargaPedidosBase select o.PesoLiquido).Sum();
            dadosSumarizados.PesoTotalReentrega = (from o in cargaPedidosBase where o.ReentregaSolicitada select o.Peso).Sum();

            if (await repApoliceSeguroAverbacao.ExistePorCargaAsync(carga.Codigo))
            {
                dadosSumarizados.PossuiAverbacaoCTe = true;
                dadosSumarizados.PossuiAverbacaoMDFe = ((configuracaoTMS.AverbarMDFe && !configuracaoCarga.AverbarMDFeSomenteEmCargasComCIOT) || (configuracaoTMS.AverbarMDFe && configuracaoCarga.AverbarMDFeSomenteEmCargasComCIOT && await repositorioCargaCIOT.ExisteCIOTPorCargaAsync(carga.Codigo)));
            }
            else
            {
                dadosSumarizados.PossuiAverbacaoCTe = false;
                dadosSumarizados.PossuiAverbacaoMDFe = false;
            }

            SetarGrupoPrincipalCarga(cargaPedidos, carga, unitOfWork);
            await VerificarDestinatariosFilialDestinoAsync(dadosSumarizados.ClientesDestinatarios, carga, unitOfWork);

            dadosSumarizados.Bookings = string.Join(", ", (from obj in cargaPedidos where !string.IsNullOrWhiteSpace(obj.Pedido.NumeroBooking) select obj.Pedido.NumeroBooking).ToList());
            dadosSumarizados.FiliaisVenda = string.Join(", ", (from obj in cargaPedidos where obj.Pedido.FilialVenda != null select obj.Pedido.FilialVenda.Descricao).Distinct().ToList());
            dadosSumarizados.CanalVenda = string.Join(", ", (from obj in cargaPedidos where obj.Pedido.CanalVenda != null select obj.Pedido.CanalVenda.Descricao).Distinct().ToList());
            dadosSumarizados.VolumesTotal = (from obj in cargaPedidosBase select obj.Pedido.QtVolumes).Sum();
            dadosSumarizados.CubagemTotal = (from obj in cargaPedidosBase select obj.Pedido.CubagemTotal).Sum();
            dadosSumarizados.Onda = Utilidades.String.Left(pedido?.Onda ?? "", 150);
            dadosSumarizados.ClusterRota = Utilidades.String.Left(pedido?.ClusterRota ?? "", 150);
            dadosSumarizados.DataPrevisaoInicioViagem = pedido?.DataPrevisaoInicioViagem;
            dadosSumarizados.ValorTotalProdutos = await repPedidoXMLNotaFiscal.ObterValorTotalProdutosPorCargaAsync(carga.Codigo, configuracaoTMS.NaoUsarPesoNotasPallet, classificacaoNotaDesconsiderar) ?? 0m;

            if (carga.Internacional)
                dadosSumarizados.QuantidadeVolumesNF = await repPedidoXMLNotaFiscal.BuscarVolumesPorCargaETipoFaturaAsync(carga.Codigo, classificacaoNotaDesconsiderar) ?? 0;
            else
                dadosSumarizados.QuantidadeVolumesNF = await repPedidoXMLNotaFiscal.BuscarVolumesPorCargaAsync(carga.Codigo, classificacaoNotaDesconsiderar) ?? 0;

            dadosSumarizados.LocalParqueamento = Utilidades.String.Left(ObterLocalParqueamento(pedido?.LocalParqueamento), 300);
            dadosSumarizados.NumeroPedidoEmbarcador = Utilidades.String.Left(string.Join(", ", (from obj in cargaPedidos select obj.Pedido.NumeroPedidoEmbarcador).Distinct()), 400);
            dadosSumarizados.ValorTotalMercadoriaPedidos = (from obj in cargaPedidosBase select obj.Pedido.ValorTotalNotasFiscais).Sum();

            Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamento = await repAgendamentoColeta.BuscarPorCargaAsync(carga.Codigo);
            if (agendamento != null)
            {
                dadosSumarizados.QuantidadeVolumes = agendamento.Volumes;
                dadosSumarizados.PossuiProdutoPerigoso = agendamento.CargaPerigosa;
                dadosSumarizados.ValorTotalMercadoriaPedidos = agendamento.ValorTotalVolumes;
            }

            dadosSumarizados.CentrosDeDistribuicao = Utilidades.String.Left(string.Join(", ", (from obj in pedidos where obj.CDDestino != null select obj.CDDestino.Descricao).Distinct().ToList()), 400);
            dadosSumarizados.CustoFrete = Utilidades.String.Left(string.Join(", ", (from obj in cargaPedidos where !string.IsNullOrWhiteSpace(obj.CustoFrete) select obj.CustoFrete).Distinct().ToList()), 100);
            dadosSumarizados.NumeroFrotasVeiculos = BuscarNumeroFrotasVeiculos(carga);

            if (dadosSumarizados.Codigo > 0)
                await repCargaDadosSumarizados.AtualizarAsync(dadosSumarizados);
            else
            {
                await repCargaDadosSumarizados.InserirAsync(dadosSumarizados);
                carga.DadosSumarizados = dadosSumarizados;
                await repCarga.AtualizarAsync(carga);
            }
        }

        public void AlterarDadosSumarizadosPreCarga(Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (preCarga == null)
                return;

            Dominio.Entidades.Embarcador.Cargas.CargaDadosSumarizados dadosSumarizados = preCarga.DadosSumarizados ?? new Dominio.Entidades.Embarcador.Cargas.CargaDadosSumarizados();
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoAgendamentoColeta repositorioConfiguracaoAgendamentoColeta = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAgendamentoColeta(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAgendamentoColeta configuracaoAgendamentoColeta = repositorioConfiguracaoAgendamentoColeta.BuscarPrimeiroRegistro();

            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repositorioPedido.BuscarPorPreCarga(preCarga.Codigo);

            List<Dominio.Entidades.Cliente> clientesDestinatarios = new List<Dominio.Entidades.Cliente>();
            List<string> destinatarios = new List<string>();

            if (configuracaoAgendamentoColeta.UtilizaRazaoSocialNaVisaoDoAgendamento)
            {
                destinatarios.AddRange((from pedido in pedidos where (pedido.Destinatario != null) select pedido.Destinatario.Descricao).Distinct().ToList());
            }
            else
            {
                destinatarios.AddRange((from pedido in pedidos where (pedido.Destinatario != null) && (pedido.Destinatario.GrupoPessoas == null) select pedido.Destinatario.Descricao).Distinct().ToList());
                destinatarios.AddRange((from pedido in pedidos where (pedido.Destinatario != null) && (pedido.Destinatario.GrupoPessoas != null) select pedido.Destinatario.GrupoPessoas.Descricao).Distinct());
            }

            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                clientesDestinatarios = (from pedido in pedidos where (pedido.Destinatario != null) select pedido.Destinatario).Distinct().ToList();

            List<string> codigosIntegracaoDestinatario = (from destinatario in clientesDestinatarios where (destinatario.CodigoIntegracao != null) && (destinatario.CodigoIntegracao != "") select destinatario.CodigoIntegracao).ToList();

            List<Dominio.Entidades.Cliente> clientesRemetentes = (from pedido in pedidos where (pedido.Remetente != null) select pedido.Remetente).Distinct().ToList();
            List<string> codigosIntegracaoRemetente = (from remetente in clientesRemetentes where (remetente.CodigoIntegracao != null) && (remetente.CodigoIntegracao != "") select remetente.CodigoIntegracao).ToList();
            List<string> remetentes = new List<string>();

            remetentes.AddRange((from pedido in pedidos where (pedido.Remetente != null) && (pedido.Remetente.GrupoPessoas == null) select pedido.Remetente.Descricao).Distinct().ToList());
            remetentes.AddRange((from pedido in pedidos where (pedido.Remetente != null) && (pedido.Remetente.GrupoPessoas != null) select pedido.Remetente.GrupoPessoas.Descricao).Distinct());

            if (remetentes.Count == 0)
                remetentes.AddRange((from pedido in pedidos where (pedido.GrupoPessoas != null) select pedido.GrupoPessoas.Descricao).Distinct());

            decimal distancia = pedidos.Sum(o => o.Distancia);

            if (distancia <= 0m)
            {
                if (preCarga.Distancia > 0)
                    distancia = preCarga.Distancia;
                else if (preCarga.Rota != null && preCarga.Rota.Quilometros > 0)
                    distancia = preCarga.Rota.Quilometros;
            }

            if (dadosSumarizados.ClientesDestinatarios != null)
                dadosSumarizados.ClientesDestinatarios.Clear();

            if (dadosSumarizados.ClientesRemetentes != null)
                dadosSumarizados.ClientesRemetentes.Clear();

            Dominio.Entidades.Cliente tomador = pedidos.FirstOrDefault()?.ObterTomador();

            dadosSumarizados.ClientesDestinatarios = clientesDestinatarios;
            dadosSumarizados.ClientesRemetentes = clientesRemetentes;
            dadosSumarizados.CodigoIntegracaoDestinatarios = string.Join(", ", codigosIntegracaoDestinatario);
            dadosSumarizados.CodigoIntegracaoRemetentes = string.Join(", ", codigosIntegracaoRemetente);
            dadosSumarizados.Destinatarios = destinatarios.Count > 0 ? string.Join(" / ", destinatarios) : "Não Informado";
            dadosSumarizados.DestinatariosReais = string.Join(" / ", destinatarios);
            dadosSumarizados.Distancia = distancia;
            dadosSumarizados.NumeroColetas = ObterNumeroColetasPorPedido(pedidos);
            dadosSumarizados.NumeroEntregas = ObterNumeroEntregasPorPedido(pedidos);
            dadosSumarizados.ObservacaoEmissaoCarga = (preCarga.TipoOperacao?.UsarConfiguracaoEmissao ?? false) ? preCarga.TipoOperacao.ObservacaoEmissaoCarga : string.Empty;
            dadosSumarizados.ObservacaoEmissaoCargaTomador = tomador?.ObservacaoEmissaoCarga ?? string.Empty;
            dadosSumarizados.ObservacaoEmissaoCargaTipoOperacao = preCarga.TipoOperacao?.ObservacaoEmissaoCarga ?? string.Empty;
            dadosSumarizados.Remetentes = string.Join(" / ", remetentes);
            dadosSumarizados.RemetentesReais = string.Join(" / ", remetentes);
            dadosSumarizados.SubContratantes = ObterSubContratantesPreCarga(pedidos);

            Repositorio.Embarcador.Cargas.CargaDadosSumarizados repositorioCargaDadosSumarizados = new Repositorio.Embarcador.Cargas.CargaDadosSumarizados(unitOfWork);

            if (dadosSumarizados.Codigo > 0)
                repositorioCargaDadosSumarizados.Atualizar(dadosSumarizados);
            else
            {
                Repositorio.Embarcador.PreCargas.PreCarga repositorioPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork);

                preCarga.DadosSumarizados = dadosSumarizados;

                repositorioCargaDadosSumarizados.Inserir(dadosSumarizados);
                repositorioPreCarga.Atualizar(preCarga);
            }
        }

        public void AtualizarTiposDocumentos(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaDadosSumarizados repCargaDadosSumarizados = new Repositorio.Embarcador.Cargas.CargaDadosSumarizados(unitOfWork);

            if (carga.DadosSumarizados == null)
                carga.DadosSumarizados = new Dominio.Entidades.Embarcador.Cargas.CargaDadosSumarizados();

            carga.DadosSumarizados.PossuiNFS = cargaPedidos.Any(o => o.PossuiNFS);
            carga.DadosSumarizados.PossuiCTe = cargaPedidos.Any(o => o.PossuiCTe);
            carga.DadosSumarizados.PossuiNFSManual = cargaPedidos.Any(o => o.PossuiNFSManual);

            if (carga.DadosSumarizados.Codigo > 0)
                repCargaDadosSumarizados.Atualizar(carga.DadosSumarizados);
            else
                repCargaDadosSumarizados.Inserir(carga.DadosSumarizados);
        }

        public List<Dominio.Entidades.Cliente> ObterDestinatarios(int codigoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(codigoCarga, null, retornarPedidoPallet: false);
            cargaPedidos = (from o in cargaPedidos where o.Pedido.TipoPedido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedido.Coleta select o).ToList();

            if (cargaPedidos.Count == 0)
                return new List<Dominio.Entidades.Cliente>();

            return (
                from o in cargaPedidos where o.Pedido.Destinatario != null select o.Pedido.Destinatario).ToList().Concat((
                from o in cargaPedidos where o.Recebedor != null select o.Recebedor).ToList()
            ).Distinct().ToList();
        }

        public List<Dominio.Entidades.Cliente> ObterRemetentes(int codigoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(codigoCarga, null, retornarPedidoPallet: false);
            cargaPedidos = (from o in cargaPedidos where o.Pedido.TipoPedido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedido.Coleta select o).ToList();

            if (cargaPedidos.Count == 0)
                return new List<Dominio.Entidades.Cliente>();

            return (from o in cargaPedidos where o.Pedido.Remetente != null select o.Pedido.Remetente).Distinct().ToList();
        }

        public string ObterDestino(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            return $"{carga.DadosSumarizados?.Destinos ?? ""} ({carga.DadosSumarizados?.Destinatarios ?? ""})";
        }

        public List<Dominio.ObjetosDeValor.Localidade> ObterDestinos(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.CargaLocaisPrestacao repositorioCargaLocaisPrestacao = new Repositorio.Embarcador.Cargas.CargaLocaisPrestacao(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao> cargaLocaisPrestacao = repositorioCargaLocaisPrestacao.BuscarPorCarga(carga.Codigo);

            if (cargaLocaisPrestacao.Count > 0)
                return ObterDestinos(cargaLocaisPrestacao, tipoServicoMultisoftware);

            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosEntrega = (from o in cargaPedidos where !o.PedidoPallet && o.Pedido.TipoPedido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedido.Coleta select o).ToList();

            return ObterDestinos(cargaPedidosEntrega, tipoServicoMultisoftware, carga);
        }

        public string ObterDestinos(Dominio.Entidades.Embarcador.Cargas.Carga carga, bool retornarNumeroEntregar)
        {
            string destino = "";
            string entrega = "Entrega";
            string front = "";
            int numeroEntregas = 0;

            if (carga.DadosSumarizados != null)
            {
                if (!string.IsNullOrWhiteSpace(carga.DadosSumarizados.Recebedores))
                    destino = " <i style='font-size:10px;'> (Recebedor: " + carga.DadosSumarizados.Recebedores + ") </i>";

                destino += carga.DadosSumarizados.Destinos;
                numeroEntregas = carga.DadosSumarizados.NumeroEntregas;
            }
            if (numeroEntregas > 1)
                entrega = "Entregas";

            string descricaoEntrega = "";
            if (retornarNumeroEntregar)
                descricaoEntrega = " (" + numeroEntregas + " " + entrega + ")";

            return destino + descricaoEntrega + front;
        }

        public string ObterOrigem(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            return $"{carga.DadosSumarizados?.Origens ?? ""} ({carga.DadosSumarizados?.Remetentes ?? ""})";
        }

        public string ObterOrigemDestinos(Dominio.Entidades.Embarcador.Cargas.Carga carga, bool retornarNumeroEntregar, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                string destino = "";
                string entrega = "Entrega";
                string origem = "";
                string coleta = "";
                string front = "";
                int numeroEntregas = 0;
                int numeroColetas = 0;
                string km = "";
                string wan = "";

                if (carga.DadosSumarizados != null)
                {
                    origem = carga.DadosSumarizados.Origens;
                    if (!string.IsNullOrWhiteSpace(carga.DadosSumarizados.Expedidores))
                        origem += " <i style='font-size:10px;'> (Expedidor: " + carga.DadosSumarizados.Expedidores + ") </i>";

                    if (carga.OrigemTrocaNota != null)
                        origem = " <i style='font-size:10px;'> (Troca de Nota: " + carga.OrigemTrocaNota.Descricao + ") </i>";

                    if (carga.Filial != null && carga.Filial.ExibirDescricaoRemetente)
                        origem = "(" + carga.DadosSumarizados.Remetentes + ") " + origem;

                    if (!string.IsNullOrWhiteSpace(carga.DadosSumarizados.Recebedores))
                        destino = " <i style='font-size:10px;'> (Recebedor: " + carga.DadosSumarizados.Recebedores + ") </i>";

                    if (!string.IsNullOrEmpty(carga.DadosSumarizados.CodigoIntegracaoDestinatarios))
                        destino += carga.DadosSumarizados.CodigoIntegracaoDestinatarios + " - ";

                    destino += carga.DadosSumarizados.Destinos;
                    numeroColetas = carga.DadosSumarizados.NumeroColetas;
                    numeroEntregas = carga.DadosSumarizados.NumeroEntregas;

                    if (carga.DadosSumarizados.Distancia > 0)
                        km = " - " + carga.DadosSumarizados.Distancia + " KM";
                }
                if (numeroEntregas > 1)
                    entrega = "Entregas";

                if (numeroColetas > 1)
                    coleta = " (" + numeroColetas + " Coletas)";

                string descricaoEntrega = "";
                if (retornarNumeroEntregar)
                    descricaoEntrega = " (" + numeroEntregas + " " + entrega + ")";

                if (carga.Empresa != null && carga.Empresa.EmissaoDocumentosForaDoSistema)
                    wan = "(WAN) ";

                return wan + origem + coleta + " até " + destino + descricaoEntrega + front + km;

            }
            else
            {
                return ObterOrigemDestinosTMS(carga);
            }
        }

        public string ObterDestino(Dominio.Entidades.Embarcador.Cargas.Carga carga, bool retornarNumeroEntregar, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool ocultarCodigoIntegracao)
        {
            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return ObterDestinoTMS(carga);

            string destino = "";
            int numeroEntregas = 0;
            string km = "";

            if (carga.DadosSumarizados != null)
            {
                if (!string.IsNullOrWhiteSpace(carga.DadosSumarizados.Recebedores))
                    destino = " <i style='font-size:10px;'> (Recebedor: " + carga.DadosSumarizados.Recebedores + ") </i>";

                if (!string.IsNullOrEmpty(carga.DadosSumarizados.CodigoIntegracaoDestinatarios) && !ocultarCodigoIntegracao)
                    destino += carga.DadosSumarizados.CodigoIntegracaoDestinatarios + " - ";

                destino += carga.DadosSumarizados.Destinos;
                numeroEntregas = carga.DadosSumarizados.NumeroEntregas;

                if (carga.DadosSumarizados.Distancia > 0)
                    km = " - " + Math.Round(carga.DadosSumarizados.Distancia, 2) + " KM";
            }

            string descricaoEntrega = "";
            if (retornarNumeroEntregar)
                descricaoEntrega = " (" + numeroEntregas + " " + (numeroEntregas > 1 ? "Entregas" : "Entrega") + ")";

            return destino + descricaoEntrega + km;
        }

        private string ObterDestinoTMS(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            string destino = "";

            int numeroEntregas;
            long distancia = 0;
            if (carga?.DadosSumarizados != null)
            {
                distancia = (long)carga.DadosSumarizados.Distancia;
                numeroEntregas = carga.DadosSumarizados.NumeroEntregas;

                destino += "(" + carga.DadosSumarizados.Destinatarios + ")" + carga.DadosSumarizados.Destinos;

                if (numeroEntregas > 1)
                    destino += " com " + numeroEntregas + " entregas";

                if (distancia > 0)
                    destino += " (" + distancia + " KM)";
            }

            return destino;
        }

        public string ObterOrigem(Dominio.Entidades.Embarcador.Cargas.Carga carga, bool retornarNumeroEntregar, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                string origem = "";
                string coleta = "";
                int numeroColetas = 0;
                string wan = "";

                if (carga.DadosSumarizados != null)
                {
                    origem = carga.DadosSumarizados.Origens;
                    if (!string.IsNullOrWhiteSpace(carga.DadosSumarizados.Expedidores))
                        origem += " <i style='font-size:10px;'> (Expedidor: " + carga.DadosSumarizados.Expedidores + ") </i>";

                    if (carga.OrigemTrocaNota != null)
                        origem = " <i style='font-size:10px;'> (Troca de Nota: " + carga.OrigemTrocaNota.Descricao + ") </i>";

                    if (carga.Filial != null && carga.Filial.ExibirDescricaoRemetente)
                        origem = "(" + carga.DadosSumarizados.Remetentes + ") " + origem;

                    numeroColetas = carga.DadosSumarizados.NumeroColetas;
                }

                if (numeroColetas > 1)
                    coleta = " (" + numeroColetas + " Coletas)";

                if (carga.Empresa != null && carga.Empresa.EmissaoDocumentosForaDoSistema)
                    wan = "(WAN) ";

                return wan + origem + coleta;
            }
            else
            {
                return ObterOrigemTMS(carga);
            }
        }

        public string ObterOrigemTMS(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (carga.DadosSumarizados != null)
                return carga.DadosSumarizados.Remetentes + " (" + carga.DadosSumarizados.Origens + ")";

            return "";
        }

        public decimal ObterPesoTotal(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            decimal peso = (from o in cargaPedidos select o.Peso).Sum();
            decimal pesoCarregamento = carga.Carregamento?.PesoCarregamento ?? 0m;

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                if (pesoCarregamento > 0m)
                    return pesoCarregamento;

                return peso;
            }

            if (peso > 0m)
                return peso;

            return pesoCarregamento;
        }

        public int BuscarNumeroDeEntregasPorPedido(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork).BuscarPrimeiroRegistro();

            int numeroEntregasPedido = ObterNumeroEntregasEmissaoPorPedido(cargaPedidos, configuracaoGeralCarga.NaoConsiderarRecebedorAoCalcularNumeroEntregasEmissaoPorPedido ?? false);
            int numeroEntregasNotas = ObterNumeroEntregasEmissaoPorNota(cargaPedidos, unitOfWork);

            int numeroEntregas = numeroEntregasPedido + numeroEntregasNotas;

            return numeroEntregas > 0 ? numeroEntregas : 1;
        }

        #endregion Métodos Públicos
    }
}
