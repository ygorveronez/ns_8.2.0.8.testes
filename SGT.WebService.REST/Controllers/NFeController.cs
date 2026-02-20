using Dominio.ObjetosDeValor.Embarcador.NFe;
using Dominio.ObjetosDeValor.Enumerador;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace SGT.WebService.REST.Controllers
{
    /// <summary>
    /// API para cadastro e manutenção de Notas Fiscais.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class NFeController : BaseService
    {
        #region Construtores
        public NFeController(IMemoryCache memoryCache, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(memoryCache, configuration, webHostEnvironment) { }
        #endregion

        #region Metodos Publicos

        /// <summary>
        /// Envio de XML Nota Fiscal.
        /// </summary>
        /// <param name="xml">XML da nota que sera processado</param>
        /// <returns></returns>
        /// <response code="406">Não foi possível ler o XML da NF-e.</response>
        [HttpPost("EnviarXMLNotaFiscal")]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        public Dominio.ObjetosDeValor.WebService.Retorno<string> EnviarXMLNotaFiscal(IFormFile xml)
        {
            using (var ms = new MemoryStream())
            {
                xml.CopyTo(ms);
                return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
                {
                    return new Servicos.WebService.NFe.NotaFiscal(unitOfWork, TipoServicoMultisoftware, Auditado, Cliente).EnviarXMLNotaFiscal(ms);
                });
            }
        }


        /// <summary>
        /// Vincula as notas fiscais.
        /// </summary>
        /// <param name="protocoloNotasFiscaisChaves">Protocolo da cargar a processar e chaves para vincular</param>
        /// <returns></returns>
        [HttpPost("VincularNotaFiscal")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> VincularNotaFiscal(Dominio.ObjetosDeValor.WebService.Rest.Unilever.NFe.ProtocoloCargaListaChave protocoloNotasFiscaisChaves)
        {

            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.NFe.NotaFiscal(unitOfWork, TipoServicoMultisoftware, Auditado, Cliente).VincularNotaFiscal(protocoloNotasFiscaisChaves.Protocolo, protocoloNotasFiscaisChaves.ListaNotasFiscaisChaves, integradora);
            });

        }

        [HttpPost("IntegrarDadosNotasFiscais")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> IntegrarDadosNotasFiscais(Dominio.ObjetosDeValor.WebService.Rest.NFe.ProtocoloListaNotasFiscais protocoloListaNotasFiscais)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.NFe.NotaFiscal(unitOfWork, TipoServicoMultisoftware, Auditado, Cliente).IntegrarDadosNotasFiscais(protocoloListaNotasFiscais.Protocolo, protocoloListaNotasFiscais.ListaNotasFiscais, integradora, null);
            });
        }

        [HttpPost("IntegrarNotasFiscais")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> IntegrarNotasFiscais(Dominio.ObjetosDeValor.WebService.Rest.NFe.ProtocoloTokenNF protocoloTokenNF)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.NFe.NotaFiscal(unitOfWork, TipoServicoMultisoftware, Auditado, Cliente).IntegrarNotasFiscais(protocoloTokenNF.Protocolo, protocoloTokenNF.TokensXMLNotasFiscais, integradora);
            });
        }

        [HttpPost("IntegrarNotasFiscaisComAverbacaoeValePedagio")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> IntegrarNotasFiscaisComAverbacaoeValePedagio(Dominio.ObjetosDeValor.WebService.Rest.NFe.ProtocoloTokenNF protocoloTokenNF)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.NFe.NotaFiscal(unitOfWork, TipoServicoMultisoftware, Auditado, Cliente).IntegrarNotasFiscaisComAverbacaoeValePedagio(protocoloTokenNF.Protocolo, protocoloTokenNF.TokensXMLNotasFiscais, protocoloTokenNF.Averbacao, protocoloTokenNF.ValePedagio, integradora, protocoloTokenNF.Ciot, protocoloTokenNF.InformacoesPagamento);
            });
        }

        [HttpPost("ConfirmarRecebimentoCargaAguardandoNotasFiscais")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> ConfirmarRecebimentoCargaAguardandoNotasFiscais(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.NFe.NotaFiscal(unitOfWork, TipoServicoMultisoftware, Auditado, Cliente).ConfirmarRecebimentoCargaAguardandoNotasFiscais(protocolo);
            });
        }

        [HttpPost("BuscarCargasAguardandoNotasFiscais")]
        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>> BuscarCargasAguardandoNotasFiscais(int inicio, int limite, string codigoTipoOperacao)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.NFe.NotaFiscal(unitOfWork, TipoServicoMultisoftware, Auditado, Cliente).BuscarCargasAguardandoNotasFiscais(inicio, limite, codigoTipoOperacao, integradora);
            });
        }

        /// <summary>
        /// Informa cancelamento de nota fiscal.
        /// </summary>
        /// <param name="protocoloNFe">Protocolo da nota fiscal</param>
        /// <returns></returns>
        [HttpPost("InformarCancelamentoNotaFiscal")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> InformarCancelamentoNotaFiscal(int protocoloNFe)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.NFe.NotaFiscal(unitOfWork, TipoServicoMultisoftware, Auditado, Cliente).InformarCancelamentoNotaFiscal(protocoloNFe);
            });
        }

        [HttpPost("EnviarArquivoXMLNFe")]
        public Dominio.ObjetosDeValor.WebService.Retorno<string> EnviarArquivoXMLNFe(IFormFile arquivo)
        {
            using (var ms = new MemoryStream())
            {
                arquivo.CopyTo(ms);

                return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
                {
                    return new Servicos.WebService.NFe.NotaFiscal(unitOfWork, TipoServicoMultisoftware, Auditado, Cliente).EnviarArquivoXMLNFe(ms);
                });
            }
        }

        [HttpPost("ConfirmarEtapaNFe")]
        public Dominio.ObjetosDeValor.WebService.Retorno<bool> ConfirmarEtapaNFe(Dominio.ObjetosDeValor.Embarcador.NFe.ConfirmarEtapaNFe confirmarEtapaNFe)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return new Servicos.WebService.NFe.NotaFiscal(unitOfWork, TipoServicoMultisoftware, Auditado, Cliente, WebServiceConsultaCTe).ConfirmarEtapaNFe(confirmarEtapaNFe);
            });
        }

        protected override OrigemAuditado ObterOrigemAuditado()
        {
            return OrigemAuditado.WebServiceNFe;
        }

        #endregion
    }
}
