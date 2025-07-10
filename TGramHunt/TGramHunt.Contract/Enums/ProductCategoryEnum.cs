using System.ComponentModel.DataAnnotations;

namespace TGramHunt.Contract.Enums
{
    public enum ProductCategory
    {
        [Display(Name = "Channel")]
        Channel = 1,

        [Display(Name = "Group")]
        Group,

        [Display(Name = "Sticker-pack")]
        StickerPack,

        [Display(Name = "Game")]
        Game,

        [Display(Name = "Bot")]
        Bot
    }
}
