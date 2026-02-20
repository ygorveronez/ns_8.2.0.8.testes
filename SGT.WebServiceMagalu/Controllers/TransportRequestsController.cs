using SGT.WebServiceMagalu.Base;
using SGT.WebServiceMagalu.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;

namespace SGT.WebServiceMagalu.Controllers
{
    //[Authorize]
    public class TransportRequestsController : ApiController
    {
        private static List<Pedido> _Pedidos { get; set; } = new List<Pedido>();

        // GET api/<controller>
        public IEnumerable<Pedido> Get()
        {
            return _Pedidos;
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public HttpResponseMessage Post([FromBody] Pedido model)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Servicos.Log.TratarErro("Recebido Pedido Magalu " + (model != null ? Newtonsoft.Json.JsonConvert.SerializeObject(model) : string.Empty));

                Dominio.Entidades.WebService.Integradora integradora = ValidarToken();
                if (integradora == null)
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);

                if (string.IsNullOrWhiteSpace(model?.order?.id) || model.packages.Count < 0)
                    return Request.CreateResponse(HttpStatusCode.BadRequest);

                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

                string ip = HttpContext.Current.Request.UserHostAddress;

                Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();

                auditado.Empresa = null;
                auditado.Integradora = integradora;
                auditado.IP = Utilidades.String.Left(ip, 50);
                auditado.OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServiceCargas;
                auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Integradoras;
                auditado.Usuario = null;

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoBase = ObterPedidoBase(unitOfWork);

                foreach (var package in model.packages)
                {
                    Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoExistente = null;

                    pedidoExistente = repPedido.BuscarPorNumeroPedidoEmbarcadorENumeroOrdem(package.id, package.id);

                    if (pedidoExistente == null)
                    {
                        string cpfCnpjDestinatario = "";
                        if (package.destination != null && package.destination.deliveryPlace?.distributionCenter != null && !string.IsNullOrWhiteSpace(package.destination.deliveryPlace?.distributionCenter?.cnpj))
                            cpfCnpjDestinatario = package.destination.deliveryPlace?.distributionCenter?.cnpj ?? "";
                        else if (package.destination != null)
                            cpfCnpjDestinatario = !string.IsNullOrWhiteSpace(package.destination.deliveryPlace?.customer?.cpf ?? "") ? package.destination.deliveryPlace.customer.cpf : package.destination.deliveryPlace?.customer?.cnpj;

                        Dominio.Entidades.Cliente destinatario = null;
                        if (!string.IsNullOrWhiteSpace(cpfCnpjDestinatario))
                        {
                            destinatario = repCliente.BuscarPorCPFCNPJ(cpfCnpjDestinatario.ToDouble());
                            if (destinatario == null)
                            {
                                Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa pessoa = ConverterObjetoRetornoPessoa(package.destination);
                                if (pessoa != null)
                                {
                                    Servicos.Cliente serCliente = new Servicos.Cliente(unitOfWork.StringConexao);
                                    Dominio.ObjetosDeValor.RetornoVerificacaoCliente retorno = serCliente.ConverterObjetoValorPessoa(pessoa, "Destinatario", unitOfWork);
                                    if (retorno.Status)
                                        destinatario = retorno.cliente;
                                    else
                                    {
                                        Servicos.Log.TratarErro("Não foi possível inserir destinatário " + cpfCnpjDestinatario);
                                        return Request.CreateResponse(HttpStatusCode.BadRequest);
                                    }
                                }
                            }
                        }

                        Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoAdicionar = pedidoBase.Clonar();
                        Dominio.Entidades.Cliente remetente = null;

                        if (!string.IsNullOrWhiteSpace(package?.origin?.pickupPlace?.distributionCenter?.cnpj ?? ""))
                        {
                            string cpfCnpjRemetente = package.origin.pickupPlace.distributionCenter.cnpj;
                            remetente = repCliente.BuscarPorCPFCNPJ(cpfCnpjRemetente.ToDouble());
                            if (remetente == null)
                            {
                                Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa pessoa = ConverterObjetoRetornoPessoa(package.origin);
                                if (pessoa != null)
                                {
                                    Servicos.Cliente serCliente = new Servicos.Cliente(unitOfWork.StringConexao);
                                    Dominio.ObjetosDeValor.RetornoVerificacaoCliente retorno = serCliente.ConverterObjetoValorPessoa(pessoa, "Remetente", unitOfWork);
                                    if (retorno.Status)
                                        remetente = retorno.cliente;
                                    else
                                    {
                                        Servicos.Log.TratarErro("Não foi possível inserir remetente " + cpfCnpjRemetente);
                                        return Request.CreateResponse(HttpStatusCode.BadRequest);
                                    }
                                }
                            }
                        }

                        pedidoAdicionar.NumeroPedidoEmbarcador = package?.id ?? "";
                        pedidoAdicionar.NumeroOrdem = package?.id ?? "";
                        pedidoAdicionar.Destinatario = destinatario;
                        pedidoAdicionar.TipoPessoa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.Pessoa;
                        pedidoAdicionar.Remetente = remetente;
                        pedidoAdicionar.Origem = remetente?.Localidade ?? null;
                        pedidoAdicionar.PesoLiquidoTotal = 0m;
                        pedidoAdicionar.Destino = destinatario?.Localidade ?? null;
                        pedidoAdicionar.DataCarregamentoPedido = null;
                        pedidoAdicionar.DataInicialColeta = null;
                        pedidoAdicionar.TipoDeCarga = null;
                        if (package.deadline != null)
                            pedidoAdicionar.PrevisaoEntrega = DateTime.ParseExact(package.deadline.date, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None);
                        pedidoAdicionar.DataCarregamentoPedido = DateTime.Now;
                        pedidoAdicionar.DataInicialColeta = DateTime.Now;

                        pedidoAdicionar.Numero = repPedido.BuscarProximoNumero();
                        pedidoAdicionar.NumeroSequenciaPedido = repPedido.ObterProximoCodigo();

                        pedidoAdicionar.PesoTotal = 0m;
                        pedidoAdicionar.PesoSaldoRestante = 0m;
                        pedidoAdicionar.QtVolumes = 0;
                        pedidoAdicionar.SituacaoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Aberto;
                        pedidoAdicionar.EtapaPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaPedido.Finalizada;

                        if (repPedido.ContemPedidoMesmoNumero(pedidoAdicionar.Numero))
                            pedidoAdicionar.Numero = repPedido.BuscarProximoNumero();

                        repPedido.Inserir(pedidoAdicionar);

                        Servicos.Auditoria.Auditoria.Auditar(auditado, pedidoAdicionar, null, "Pedido recebido pelo WS Magalu", unitOfWork);
                    }
                    else
                    {
                        Servicos.Auditoria.Auditoria.Auditar(auditado, pedidoExistente, null, "Pedido recebido em duplicidade pelo WS Magalu", unitOfWork);
                        Servicos.Log.TratarErro("Pedido já inserido no sistema nº: " + (package?.id ?? "") + " ordem:" + (package?.id ?? ""));
                    }
                }

                return Request.CreateResponse(HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }

        #region Métodos Privativos
        private Dominio.Entidades.WebService.Integradora ValidarToken()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(
                Conexao.StringConexao,
                Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
            {
                HttpContext httpContext = HttpContext.Current;
                string tokenBaerer = httpContext.Request.Headers["Authorization"];

                if (string.IsNullOrWhiteSpace(tokenBaerer))
                    return null;

                tokenBaerer = tokenBaerer.Replace("Bearer", "").Trim();

                Repositorio.WebService.Integradora repIntegracadora = new Repositorio.WebService.Integradora(unitOfWork);
                Dominio.Entidades.WebService.Integradora integradora = repIntegracadora.BuscarPorTokenIntegracao(tokenBaerer);

                return integradora;
            }
        }

        private Dominio.Entidades.Embarcador.Pedidos.Pedido ObterPedidoBase(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = new Dominio.Entidades.Embarcador.Pedidos.Pedido();

            pedido.TipoOperacao = null;
            pedido.PedidoIntegradoEmbarcador = false;
            pedido.GerarAutomaticamenteCargaDoPedido = false;
            pedido.SituacaoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Aberto;
            pedido.UltimaAtualizacao = DateTime.Now;
            pedido.DataCadastro = DateTime.Now;

            if (pedido.QtdEntregas == 0)
                pedido.QtdEntregas = 1;

            pedido.SituacaoAtualPedidoRetirada = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtualPedidoRetirada.LiberacaoFinanceira;

            return pedido;
        }

        private static Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa ConverterObjetoRetornoPessoa(destination destinatario)
        {
            try
            {
                if (destinatario == null || destinatario.deliveryPlace == null || (destinatario.deliveryPlace.customer == null && destinatario.deliveryPlace.distributionCenter == null))
                    return null;

                Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa pessoa = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();

                pessoa.AtualizarEnderecoPessoa = true;
                pessoa.ClienteExterior = false;

                pessoa.CNAE = "";
                pessoa.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
                pessoa.Endereco.Bairro = destinatario.address?.district ?? "";
                pessoa.Endereco.CEP = Utilidades.String.OnlyNumbers(destinatario.address?.zipcode ?? "");
                pessoa.Endereco.CEPSemFormato = destinatario.address?.zipcode ?? "";

                pessoa.Endereco.Cidade = new Dominio.ObjetosDeValor.Localidade();
                pessoa.Endereco.Cidade.Descricao = (destinatario.address?.city ?? "").ToUpper();
                pessoa.Endereco.Cidade.SiglaUF = (destinatario.address?.state ?? "").ToUpper();

                pessoa.Endereco.Complemento = (destinatario.address?.reference ?? "").ToUpper();
                pessoa.Endereco.Logradouro = (destinatario.address?.street ?? "").ToUpper();
                pessoa.Endereco.Numero = (destinatario.address?.number ?? "").ToUpper();

                pessoa.Endereco.Telefone = destinatario.phones != null && destinatario.phones.Count > 0 ? (destinatario.phones.FirstOrDefault().number.ToString("D") ?? "") : "";

                if (pessoa.Endereco.Telefone.Length > 15)
                    pessoa.Endereco.Telefone = "";

                if (destinatario.deliveryPlace != null && destinatario.deliveryPlace.distributionCenter != null && !string.IsNullOrWhiteSpace(destinatario.deliveryPlace?.distributionCenter?.cnpj ?? ""))
                {
                    pessoa.CPFCNPJ = (destinatario.deliveryPlace?.distributionCenter?.cnpj ?? "");
                    pessoa.NomeFantasia = (destinatario.deliveryPlace?.distributionCenter?.name ?? "").ToUpper();
                    pessoa.RazaoSocial = (destinatario.deliveryPlace?.distributionCenter?.name ?? "").ToUpper();
                    pessoa.TipoPessoa = Dominio.Enumeradores.TipoPessoa.Juridica;
                    pessoa.RGIE = (destinatario.deliveryPlace?.distributionCenter?.ie ?? "").ToUpper();
                }
                else if (destinatario.deliveryPlace != null)
                {
                    pessoa.CPFCNPJ = !string.IsNullOrWhiteSpace(destinatario.deliveryPlace?.customer?.cpf ?? "") ? destinatario.deliveryPlace?.customer?.cpf : destinatario.deliveryPlace?.customer?.cnpj;
                    pessoa.NomeFantasia = (destinatario.deliveryPlace?.customer?.name ?? "").ToUpper();
                    pessoa.RazaoSocial = (destinatario.deliveryPlace?.customer?.name ?? "").ToUpper();
                    pessoa.TipoPessoa = !string.IsNullOrWhiteSpace(destinatario.deliveryPlace?.customer?.cpf ?? "") ? Dominio.Enumeradores.TipoPessoa.Fisica : Dominio.Enumeradores.TipoPessoa.Juridica;
                    pessoa.RGIE = "ISENTO";
                }

                return pessoa;
            }
            catch
            {
                return null;
            }
        }

        private static Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa ConverterObjetoRetornoPessoa(origin remetente)
        {
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa pessoa = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();

                pessoa.AtualizarEnderecoPessoa = true;
                pessoa.ClienteExterior = false;

                pessoa.CNAE = "";
                pessoa.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
                pessoa.Endereco.Bairro = remetente.address?.district ?? "";
                pessoa.Endereco.CEP = Utilidades.String.OnlyNumbers(remetente.address?.zipcode ?? "");
                pessoa.Endereco.CEPSemFormato = remetente.address?.zipcode ?? "";

                pessoa.Endereco.Cidade = new Dominio.ObjetosDeValor.Localidade();
                pessoa.Endereco.Cidade.Descricao = (remetente.address?.city ?? "").ToUpper();
                pessoa.Endereco.Cidade.SiglaUF = (remetente.address?.state ?? "").ToUpper();

                pessoa.Endereco.Complemento = (remetente.address?.reference ?? "").ToUpper();
                pessoa.Endereco.Logradouro = (remetente.address?.street ?? "").ToUpper();
                pessoa.Endereco.Numero = (remetente.address?.number ?? "").ToUpper();

                pessoa.Endereco.Telefone = remetente.phones != null && remetente.phones.Count > 0 ? (remetente.phones.FirstOrDefault().number.ToString("D") ?? "") : "";

                if (pessoa.Endereco.Telefone.Length > 15)
                    pessoa.Endereco.Telefone = "";

                pessoa.CPFCNPJ = remetente.pickupPlace?.distributionCenter?.cnpj ?? "";
                pessoa.NomeFantasia = (remetente.pickupPlace?.distributionCenter?.name ?? "").ToUpper();
                pessoa.RazaoSocial = (remetente.pickupPlace?.distributionCenter?.name ?? "").ToUpper();
                pessoa.TipoPessoa = Dominio.Enumeradores.TipoPessoa.Juridica;
                pessoa.RGIE = remetente.pickupPlace?.distributionCenter?.ie ?? "";

                return pessoa;
            }
            catch
            {
                return null;
            }
        }



        #endregion
    }
}