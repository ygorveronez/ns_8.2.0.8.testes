using Repositorio;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Pessoa
{
    public class GrupoPessoasLayoutEDI : ServicoBase
    {

        #region Construtores        

        public GrupoPessoasLayoutEDI(UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken)
        {
        }

        #endregion

        #region Métodos Públicos

        public async Task<string> ObtemCertificadoChavePrivadaAsync(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI layoutEDI)
        {
            byte[] arquivo = new byte[0];

            Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Pessoas.AnexoGrupoPessoasLayoutEDI, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI> repositorioAnexoGrupoPessoasLayoutEDI = new Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Pessoas.AnexoGrupoPessoasLayoutEDI, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI>(_unitOfWork, _cancellationToken);

            Dominio.Entidades.Embarcador.Pessoas.AnexoGrupoPessoasLayoutEDI anexoEDI = (await repositorioAnexoGrupoPessoasLayoutEDI.BuscarPorEntidadeAsync(layoutEDI.Codigo)).FirstOrDefault();

            if (anexoEDI != null)
            {
                Servicos.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Pessoas.AnexoGrupoPessoasLayoutEDI, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI> servicoAnexoLayoutEDI = new Servicos.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Pessoas.AnexoGrupoPessoasLayoutEDI, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI>(_unitOfWork);
                arquivo = servicoAnexoLayoutEDI.DownloadAnexo(anexoEDI, _unitOfWork);
            }

            return System.Text.Encoding.UTF8.GetString(arquivo);
        }

        #endregion

        #region Métodos Privados
        #endregion
    }
}
