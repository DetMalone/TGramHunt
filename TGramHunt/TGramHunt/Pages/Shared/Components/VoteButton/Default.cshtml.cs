using Microsoft.AspNetCore.Mvc;
using TGramHunt.ViewModels.MainPage;

namespace TGramHunt.Pages.Shared.Components.VoteButton
{
    public class VoteButton : ViewComponent
    {
        public VoteButton()
        {
        }

        public IViewComponentResult Invoke(ProductVoteViewModel productVoteViewModel)
        {
            return View(productVoteViewModel);
        }
    }
}