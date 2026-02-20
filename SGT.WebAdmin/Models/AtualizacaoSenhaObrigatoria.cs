using System.ComponentModel.DataAnnotations;

namespace SGT.WebAdmin.Models
{
    public class AtualizacaoSenhaObrigatoria
    {
        [Required(ErrorMessageResourceName = "CampoSenhaAtualObrigatorio", ErrorMessageResourceType = typeof(Localization.Resources.AtualizacaoSenhaObrigatoria.AtualizacaoSenhaObrigatoria))]
        [DataType(DataType.Password)]
        public string senhaAtual { get; set; }

        [Required(ErrorMessageResourceName = "CampoNovaSenhaObrigatorio", ErrorMessageResourceType = typeof(Localization.Resources.AtualizacaoSenhaObrigatoria.AtualizacaoSenhaObrigatoria))]
        [DataType(DataType.Password)]
        public string novaSenha { get; set; }

        [Required(ErrorMessageResourceName = "CampoConfirmacaoDeSenhaObrigatorio", ErrorMessageResourceType = typeof(Localization.Resources.AtualizacaoSenhaObrigatoria.AtualizacaoSenhaObrigatoria))]
        [DataType(DataType.Password)]
        public string confirmacaoSenha { get; set; }

        public string ReturnUrl { get; set; }
    }
}