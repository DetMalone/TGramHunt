namespace TGramHunt.ViewModels.MainPage
{
    public class ProductVoteViewModel
    {
        public string? ProductId { get; set; }
        public int VoteCount { get; set; }
        public bool IsVoteDisabled { get; set; }
        public bool IsUserVoted { get; set; } = false;
    }
}