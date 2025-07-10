namespace TGramHunt.Contract.ViewModels.EditProfile
{
    public class ProfileViewModel : ProfileViewModelBase
    {
        public string? TelegramUserName { get; set; }
        public string? Picture { get; set; }
        public string? PictureIdx41 { get; set; }
        public string? PictureIdx100 { get; set; }
        public int PictureCache { get; set; } = 1;
        public bool IsClosed { get; set; }
        public long MyProductCount { get; set; }
        public long UpvotesCount { get; set; }
    }
}