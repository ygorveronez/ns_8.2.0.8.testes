using System;
using System.Collections.Generic;
using System.IO;

namespace Servicos.Embarcador.Integracao.Sistema
{
    public sealed class IntegracaoSistema : IntegracaoClientBase<WSDisponibilidade.DisponibilidadeClient, WSDisponibilidade.IDisponibilidade>
    {
        #region Propriedades Privadas

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public IntegracaoSistema(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Privados

        private MemoryStream ObterXmlsCompactado(Servicos.Models.Integracao.InspectorBehavior inspector)
        {
            Dictionary<string, string> conteudoCompactar = new Dictionary<string, string>();

            conteudoCompactar.Add("Requisicao.xml", inspector.LastRequestXML);
            conteudoCompactar.Add("Resposta.xml", inspector.LastResponseXML);

            return Utilidades.File.GerarArquivoCompactado(conteudoCompactar);
        }

        #endregion

        #region Métodos Públicos

        public MemoryStream DownloadXmlsTesteDisponibilidade()
        {
            using (WSDisponibilidade.DisponibilidadeClient disponibilidadeClient = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<WSDisponibilidade.DisponibilidadeClient, WSDisponibilidade.IDisponibilidade>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.SGTWebService_Disponibilidade, out Servicos.Models.Integracao.InspectorBehavior inspector))
            {
                disponibilidadeClient.Testar();

                return ObterXmlsCompactado(inspector);
            }
        }

        #endregion
    }
}
